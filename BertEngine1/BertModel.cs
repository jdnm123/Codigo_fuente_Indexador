using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.ML.Models.BERT.Extensions;
using Microsoft.ML.Models.BERT.Input;
using Microsoft.ML.Models.BERT.Onnx;
using Microsoft.ML.Models.BERT.Output;
using Microsoft.ML.Models.BERT.Tokenizers;
using System.Diagnostics;

namespace Microsoft.ML.Models.BERT
{
    public class BertModel : IDisposable
    {
        private readonly BertModelConfiguration _bertModelConfiguration;
        private readonly List<string> _types;
        private List<string> _vocabulary;
        private WordPieceTokenizer _wordPieceTokenizer;
        private PredictionEngine<BertFeature, BertPredictionResult> _predictionEngine;

        public BertModel(BertModelConfiguration bertModelConfiguration, List<string> types)
        {
            _bertModelConfiguration = bertModelConfiguration;
            _types = types;
        }

        public void Initialize()
        {
            _vocabulary = ReadVocabularyFile(_bertModelConfiguration.VocabularyFile);
            _wordPieceTokenizer = new WordPieceTokenizer(_vocabulary);
            var onnxModelConfigurator = new OnnxModelConfigurator<BertFeature>(_bertModelConfiguration);
            
            _predictionEngine = onnxModelConfigurator.GetMlNetPredictionEngine<BertPredictionResult>();
        }

        public (string className, float probability) Predict(string text)
        {
            var tokens = _wordPieceTokenizer.Tokenize(text);
            var encodedFeature = Encode(tokens);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            BertPredictionResult result = _predictionEngine.Predict(encodedFeature);
            float maxValue =  result.EndLogits.Max();
            int maxIndex = result.EndLogits.ToList().IndexOf(maxValue);



            stopWatch.Stop();
           

            return (_types[maxIndex], 0);
        }

        private List<string> StitchSentenceBackTogether(List<string> tokens)
        {
            var currentToken = string.Empty;

            tokens.Reverse();

            var tokensStitched = new List<string>();

            foreach (var token in tokens)
            {
                if (!token.StartsWith("##"))
                {
                    currentToken = token + currentToken;
                    tokensStitched.Add(currentToken);
                    currentToken = string.Empty;
                } else
                {
                    currentToken = token.Replace("##", "") + currentToken;
                }
            }

            tokensStitched.Reverse();

            return tokensStitched;
        }

        private (int StartIndex, int EndIndex, float Probability) GetBestPredictionFromResult(BertPredictionResult result, int minIndex)
        {
            var bestStartLogits = new int[0] { }
                .Select((logit, index) => (Logit: logit, Index: index))
                .OrderByDescending(o => o.Logit);

            var bestEndLogits = result.EndLogits
                .Select((logit, index) => (Logit: logit, Index: index))
                .OrderByDescending(o => o.Logit)
                ;

            var bestResultsWithScore = bestStartLogits
                .SelectMany(startLogit =>
                    bestEndLogits
                    .Select(endLogit =>
                        (
                            StartLogit: startLogit.Index,
                            EndLogit: endLogit.Index,
                            Score: startLogit.Logit + endLogit.Logit
                        )
                     )
                );

            var (item, probability) = bestResultsWithScore
                .Softmax(o => o.Score)
                .OrderByDescending(o => o.Probability)
                .FirstOrDefault();

            return (StartIndex: item.StartLogit, EndIndex: item.EndLogit, probability);
        }

        private BertFeature Encode(List<(string Token, int Index)> tokens)
        {
            tokens = tokens.Take(512).ToList();
            var padding = Enumerable
                .Repeat(0L, _bertModelConfiguration.MaxSequenceLength - tokens.Count)
                .ToList();

            var tokenIndexes = tokens
                .Select(token => (long)token.Index)
                .Concat(padding)
                .ToArray();

            var segmentIndexes = GetSegmentIndexes(tokens)
                .Concat(padding)
                .ToArray();

            var inputMask =
                tokens.Select(o => 1L)
                .Concat(padding)
                .ToArray();

            BertFeature bertFeature = new BertFeature()
            {
                InputIds = tokenIndexes,
                //SegmentIds = segmentIndexes,
                InputMask = inputMask,
                token_type_ids = new long[512]
            };
            return bertFeature;
        }

        private IEnumerable<long> GetSegmentIndexes(List<(string token, int index)> tokens)
        {
            var segmentIndex = 0;
            var segmentIndexes = new List<long>();

            foreach (var (token, index) in tokens)
            {
                segmentIndexes.Add(segmentIndex);

                if (token == WordPieceTokenizer.DefaultTokens.Separation)
                {
                    segmentIndex++;
                }
            }

            return segmentIndexes;
        }

        private static List<string> ReadVocabularyFile(string filename)
        {
            var vocabulary = new List<string>();

            using (var reader = new StreamReader(filename))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        vocabulary.Add(line);
                    }
                }
            }

            return vocabulary;
        }

        public void Dispose()
        {
            _predictionEngine.Dispose();
        }
    }
}
