using Microsoft.ML.Models.BERT.Onnx;

namespace Microsoft.ML.Models.BERT
{
    public class BertModelConfiguration : IOnnxModel
    {
        public int MaxSequenceLength { get; set; } = 512;

        public int MaxAnswerLength { get; set; } = 30;

        public int BestResultSize { get; set; } = 20;

        public string VocabularyFile { get; set; }

        public string ModelPath { get; set; }

        public string[] ModelInput => new [] { "input.1", "attention_mask", "input.2" };

        public string[] ModelOutput => new [] { "1612" };
    }
}
