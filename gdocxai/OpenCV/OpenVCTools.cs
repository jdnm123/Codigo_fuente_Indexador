using System;
using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Indexai.OpenCV
{
    public class OpenVCTools
    {
        private static List<TipoDocumentalCV> _tiposDocumento;

        public OpenVCTools()
        {

        }

        public void LoadTemplates()
        {
            var dirs = Directory.EnumerateDirectories(@"Plantillas/").ToList();

            _tiposDocumento = new List<TipoDocumentalCV>();

            foreach (var dir in dirs)
            {
                var id = new DirectoryInfo(dir).Name.Split('-')[0];
                var count = new DirectoryInfo(dir).Name.Split('-')[1];
                TipoDocumentalCV tipoDocumentaleCV = new TipoDocumentalCV
                {
                    Id = id,
                    Images = new List<Image<Gray, byte>>(),
                    Count = Convert.ToInt32(count)
                };


                var pdfs = Directory.EnumerateFiles(dir, "*.bmp").ToList();
                if (pdfs.Count != 0)
                {
                    _tiposDocumento.Add(tipoDocumentaleCV);
                    foreach (var pdfFile in pdfs.Take(3))
                    {
                        var bitmapL = new List<Bitmap>() { new Bitmap(pdfFile) };
                        tipoDocumentaleCV.Images.AddRange(bitmapL.ToList().Select(x =>
                        x.ToImage<Gray, byte>()));
                    }
                }
            }
        }

        public bool ProcessImage(Bitmap sceneBitmap, string classType)
        {
            var cType = _tiposDocumento.FirstOrDefault(x => x.Id == classType);
            if (cType != null)
            {
                if (cType.Count == 0)
                {
                    return true;
                }
                else
                {
                    using (Image<Gray, byte> sceneImage = sceneBitmap.ToImage<Gray, byte>())
                    {
                        List<int> scores = new List<int>();

                        //templates de los tipos
                        foreach (var template in cType.Images)
                        {
                            try
                            {
                                // initialization
                                VectorOfPoint finalPoints = null;
                                Mat homography = null;
                                using (VectorOfKeyPoint templateKeyPoints = new VectorOfKeyPoint())
                                using (VectorOfKeyPoint sceneKeyPoints = new VectorOfKeyPoint())
                                using (Mat sceneDescriptor = new Mat())
                                using (Brisk featureDetector = new Brisk())
                                using (Mat tempalteDescriptor = new Mat())
                                using (VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch())
                                using (BFMatcher matcher = new BFMatcher(DistanceType.Hamming))
                                {
                                    Mat mask;
                                    int k = 2;
                                    double uniquenessthreshold = 0.80;

                                    // feature detectino and description

                                    featureDetector.DetectAndCompute(template, null, templateKeyPoints, tempalteDescriptor, false);
                                    featureDetector.DetectAndCompute(sceneImage, null, sceneKeyPoints, sceneDescriptor, false);


                                    // Matching
                                    matcher.Add(tempalteDescriptor);
                                    matcher.KnnMatch(sceneDescriptor, matches, k);

                                    mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                                    mask.SetTo(new MCvScalar(255));

                                    Features2DToolbox.VoteForUniqueness(matches, uniquenessthreshold, mask);

                                    int count = Features2DToolbox.VoteForSizeAndOrientation(templateKeyPoints, sceneKeyPoints, matches, mask, 1.5, 20);
                                    scores.Add(count);
                                    //if (count >= 4)
                                    //{
                                    //    homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(templateKeyPoints,
                                    //        sceneKeyPoints, matches, mask, 5);
                                    //}

                                    mask.Dispose();

                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }

                        var bestmatch = scores.OrderByDescending(x => x).ToList().First();
                        return bestmatch >= cType.Count;
                    }
                }
            }
            return true;
        }
    }
}
