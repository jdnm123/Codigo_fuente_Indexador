using Microsoft.ML.Models.BERT;
using System.IO;
using System.Linq;
using System.Text;

namespace BertEngine
{
    public class BertEngineController
    {
        private BertModel _model;

        public BertEngineController()
        {
        }

        public (string classType, float probability) Predict(string text)
        {
            return _model.Predict(text);
        }

        public void LoadModel(string modelPath, string vocabPath)
        {
            var modelConfig = new BertModelConfiguration()
            {
                VocabularyFile = vocabPath,
                ModelPath = modelPath
            };

            _model = new BertModel(modelConfig, File.ReadAllLines(
                @"train.csv",
                Encoding.UTF8).Skip(1).Select(x => x.Split(',')[1]).Distinct().ToList());
            _model.Initialize();
        }
    }
}
