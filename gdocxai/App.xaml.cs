using Indexai.Helpers;
using Indexai.Services;
using NamesServiceLib;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using BertEngine;
using Indexai;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

namespace Login2
{
    /// <summary>
    /// Lógica para App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            NamesService.Load();
            //registra el id de uso para syncfusion
            //Versión 17: MjExMjExQDMxMzcyZTM0MmUzMGpiQTBRc011allMMmRpaTZGSWI5b1g1WURtSSsraDJXcDl2QUEyREx0Zms9
            //Versión 18: MjM2MjIyQDMxMzgyZTMxMmUzMFYxVjYrUWNFV3cyZm5tbVF6WE9aRmY4SWc0WkIrOFFaS3JjaTlHNXJIaVk9
            //Version 2: MjkxNTgyQDMxMzgyZTMyMmUzMFRHakRMM3V1RlFIUWVzZlVROUk3cGowU1hYZ3RvVXUyRlVsM0ZlYzhtdTA9
            //Version 4: Mzc0Nzk0QDMxMzgyZTM0MmUzMGJJU09UWTNFZDlpckJnNWNNWXpFM1RMNUZubUgwQ1hOL21jK3FLeFcwc1k9
            //19.1
            //NDE5NTcwQDMxMzkyZTMxMmUzMFhnTWxJM2lrTThwSlJva2k5Yis1eStiYmdOQlRkT1UxV2FXNjZCWXJIUEk9
            Syncfusion.Licensing.SyncfusionLicenseProvider
               .RegisterLicense("NDE5NTcwQDMxMzkyZTMxMmUzMFhnTWxJM2lrTThwSlJva2k5Yis1eStiYmdOQlRkT1UxV2FXNjZCWXJIUEk9");

            //v1
            //Syncfusion.Licensing.SyncfusionLicenseProvider
            //   .RegisterLicense("MjM2MjIyQDMxMzgyZTMxMmUzMFYxVjYrUWNFV3cyZm5tbVF6WE9aRmY4SWc0WkIrOFFaS3JjaTlHNXJIaVk9");
#if (!DEBUG)
                Console.WriteLine("Registrando telemetría");
                SetupExceptionHandling();
            Telemetry.Initialize(); //inicia la telemetría

#else
            Console.WriteLine("Telemetría desahabilitada.");
#endif

            CacheHelper.SetCacheDirectory("pdf-cache");

            //GlobalClass.BertController = new BertEngineController();
            //GlobalClass.BertController.LoadModel("indexador-bert.onnx", "vocab.txt");
            //var doc = new PdfLoadedDocument(@"C:\Users\Carlos\Downloads\Prueba Juzgado1\Prueba Juzgado1/11001650005120190464500.pdf");

            //foreach (PdfLoadedPage page in doc.Pages)
            //{
            //    var text = page.ExtractText();
            //    string clean = TextCleaner.CleanBert(text);
            //    var (classType, probability) = GlobalClass.BertController.Predict(clean);


            //}

            //bert init


        }

        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };
            //registra la captura de errores no controlados.
            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Telemetry.Flush(); //libera la telemetría.
            base.OnExit(e);
        }

        private void LogUnhandledException(Exception exception, string source)
        {
            //captura todas los errores no controlados
            try
            {
                Telemetry.TrackException(exception);
                AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            }
            catch (Exception)
            {
                if (!Directory.Exists("logs"))
                {
                    Directory.CreateDirectory("logs");
                }
                File.WriteAllText($"logs/{Guid.NewGuid()}-report.txt", exception.StackTrace + Environment.NewLine + exception.Message);
            }
        }
    }
}