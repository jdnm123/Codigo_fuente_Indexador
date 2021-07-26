using System;
using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace Indexai.Helpers
{
    public static class MatchHelmper
    {
        private static int ProcessImage(Image<Gray, byte> template, Image<Gray, byte> sceneImage)
        {
            try
            {
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

                    featureDetector.DetectAndCompute(template, null, templateKeyPoints, tempalteDescriptor, false);
                    featureDetector.DetectAndCompute(sceneImage, null, sceneKeyPoints, sceneDescriptor, false);

                    matcher.Add(tempalteDescriptor);
                    matcher.KnnMatch(sceneDescriptor, matches, k);

                    mask = new Mat(matches.Size, 1, Emgu.CV.CvEnum.DepthType.Cv8U, 1);
                    mask.SetTo(new MCvScalar(255));

                    Features2DToolbox.VoteForUniqueness(matches, uniquenessthreshold, mask);

                    int count = Features2DToolbox.VoteForSizeAndOrientation(templateKeyPoints, sceneKeyPoints, matches, mask, 1.5, 20);

                    if (count >= 4)
                    {
                        homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(templateKeyPoints,
                            sceneKeyPoints, matches, mask, 5);
                    }

                    mask.Dispose();
                    return count;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
