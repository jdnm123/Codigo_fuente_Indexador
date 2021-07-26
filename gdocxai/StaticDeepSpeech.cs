using CSCore.CoreAudioAPI;
using DeepSpeechClient;
using Indexai.Models;
using Indexai.Services;
using Newtonsoft.Json;
using NumbersHelper;
using Syncfusion.Windows.Controls.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Indexai
{
    /// <summary>
    /// Lista de comandos.
    /// </summary>
    public enum Comandos
    {
        Guardar
    }

    public delegate void ComandoEventHandler(object sender, ComandoEventArgs e);

    public class ComandoEventArgs : EventArgs
    {
        public Comandos Comando { get; set; }
    }

    public static class StaticDeepSpeech
    {
        /// <summary>
        /// Evento que se ejecuta cuando se detecta un comando.
        /// </summary>
        public static event ComandoEventHandler OnComando;

        /// <summary>
        /// Nombre del archivo con las configuraciones del micrófono.
        /// </summary>
        private const string ConfigurationFile = "micro-config.json";
        private static DeepSpeech _sttClient;

        /// <summary>
        /// Cliente DeepSpeech.
        /// </summary>
        public static DeepSpeechStreamClient DeepSpeechClient;
        /// <summary>
        /// Lista de sugerencias del reconocimiento.
        /// </summary>
        private static ListBox _suggestionsList;

        private static Dispatcher _dispatcher;
        /// <summary>
        /// Control de fecha con reconocimiento.
        /// </summary>
        private static SfDatePicker _dateControl;
        /// <summary>
        /// Control actual del reconocimiento.
        /// </summary>
        private static TextBox _textControl;
        /// <summary>
        /// Texto previo al reconocimiento.
        /// </summary>
        private static string _prevText;
        private static bool _recording = false;
        private static MMDeviceCollection _devices;
        private static bool _append;
        private static string _lastText;

        /// <summary>
        /// Alternativas del reconocimiento.
        /// </summary>
        public static List<string> Alternatives { get; private set; } = new List<string>();
        /// <summary>
        /// Resultado del reconocimiento.
        /// </summary>
        public static RecognitionCompletedEventArgs Result { get; set; }
        public static MMDevice SelectedDevice { get; private set; }

        /// <summary>
        /// Inicia la instancia DeepSpeech con el micrófono configurado.
        /// </summary>
        public static void Initialize()
        {
            if (File.Exists(ConfigurationFile))
            {
                MicroConfig microConfig = JsonConvert.DeserializeObject<MicroConfig>(File.ReadAllText(ConfigurationFile, Encoding.UTF8));
                string selectedMicrophone = microConfig.SelectedMicrophone;
                _devices = MMDeviceEnumerator.EnumerateDevices(DataFlow.Capture, DeviceState.Active);
                if (_devices != null && _devices.Count != 0)
                {
                    //Register instance of DeepSpeech
                    _sttClient = new DeepSpeech();
                    //
                    _sttClient.CreateModel(SpeechEngineConfiguration.ModelPath, SpeechEngineConfiguration.BeamWidth);


                    DeepSpeechClient = new DeepSpeechStreamClient(_sttClient);
                    DeepSpeechClient.EnableLM(SpeechEngineConfiguration.LMPath,
                             SpeechEngineConfiguration.TriePath);
                    DeepSpeechClient.OnIntermediateResult += DeepSpeechClient_OnIntermediateResult;
                    DeepSpeechClient.OnRecognitionCompleted += DeepSpeechClient_OnRecognitionCompleted;
                }
                int selectedIndex = _devices.ToList().FindIndex(x => selectedMicrophone == x.FriendlyName);
                if (!string.IsNullOrEmpty(selectedMicrophone) && selectedIndex != -1) //verifica si el micrófono seleccionado es válido.
                {
                    SetMicrophone(selectedMicrophone);
                    //DeepSpeechClient.InitDeepSpeech();
                    //SpeechEngineConfiguration.ModelPath,



                    //DeepSpeechClient.StartRecordingFromButton();
                    //DeepSpeechClient.StopFromButtonAsync().GetAwaiter().GetResult();

                }
            }
        }

        private static void DeepSpeechClient_OnRecognitionCompleted(object sender, RecognitionCompletedEventArgs e)
        {
            var transcription = e.Transcription.Trim();
            if (transcription.ToLower() == "guardar")
            {
                OnComando?.Invoke(sender, new ComandoEventArgs { Comando = Comandos.Guardar });
            }
            else
            {
                if (e.ExtraTranscriptions.Count != 0 && !string.IsNullOrWhiteSpace(transcription))
                {
                    if (_textControl != null)
                    {
                        _dispatcher.Invoke(() =>
                        {
                            Alternatives = e.ExtraTranscriptions.ToList();
                            UpdateAlternatives(); //atualiza las alternativas

                            if (_textControl != null) //verifica si el control seleccionado es un texbox
                            {
                                if (_textControl.Name.Contains("numerico"))
                                {
                                    if (_append)
                                    {
                                        _dispatcher.Invoke(() =>
                                        {
                                            _lastText = _textControl.Text;
                                            return _textControl.Text = _textControl.Text.Trim() + " " + new string(transcription.ToNumber().ReplaceSymbols().ToCharArray().Where(x => char.IsNumber(x)).ToArray()).ToUpper();
                                        });
                                    }
                                    else _dispatcher.Invoke(() => _textControl.Text = new string(transcription.ToNumber().ReplaceSymbols().ToCharArray().Where(x => char.IsNumber(x)).ToArray()).ToUpper());
                                }
                                else if (_textControl.Name.EndsWith("_parse")) //verifica si en control requiere convertir pronunciación de letras
                                {
                                    if (_append)
                                    {
                                        _dispatcher.Invoke(() =>
                                        {
                                            _lastText = _textControl.Text;
                                            return _textControl.Text = _textControl.Text.Trim() + " " + transcription.ParseToChar().ReplaceSymbols().ToUpper();
                                        });
                                    }
                                    else _dispatcher.Invoke(() => _textControl.Text = transcription.ParseToChar().ReplaceSymbols().ToUpper());
                                }
                                else
                                {
                                    if (_append)
                                    {
                                        _dispatcher.Invoke(() =>
                                        {
                                            _lastText = _textControl.Text;
                                            return _textControl.Text = _textControl.Text.Trim() + " " + transcription.ToNumber().ReplaceSymbols().ToUpper();
                                        });
                                    }
                                    else _dispatcher.Invoke(() => _textControl.Text = transcription.ToNumber().ReplaceSymbols().ToUpper());
                                }
                                SaveText(transcription);
                            }
                        });
                    }
                    else if (_dateControl != null)
                    {
                        _dispatcher.Invoke(() =>
                        {
                            string join = string.Empty;
                            try
                            {
                                _dateControl.IsDropDownOpen = false;
                                ///transcription no se verifica que sea empty porque ya lo hace el evento internamente.
                                string[] split = transcription.ToDateFormat().Split(' ');
                                if (split.Length == 0)
                                {
                                    MessageBox.Show($"La transcripción {transcription} no cumple con los requisitos de la fecha.");
                                }
                                var fechaPart = split.Select(x => Convert.ToInt32(x)).ToArray();

                                if (fechaPart.Length == 3)
                                {
                                    join = $"{fechaPart[0]}/{fechaPart[1]}/{fechaPart[2]}";
                                }
                                else if (fechaPart.Length == 4)
                                {
                                    join = $"{fechaPart[0]}/{fechaPart[1]}/{fechaPart[2].ToString() + fechaPart[3].ToString()}";
                                }
                                _dateControl.Value = DateTime.Parse(join);
                            }
                            catch (FormatException ex) //se captura errores de formato en las fechas.
                            {
                                ex.Data.Add("transcription", transcription);
                                ex.Data.Add("fecha", join);
                                MessageBox.Show($"No se reconoce el formato de la fecha: {join}", "Error de formato.", MessageBoxButton.OK);
                                Telemetry.TrackException(ex);
                            }
                            catch (Exception ex)
                            {
                                Telemetry.TrackException(ex);
                            }
                        });
                        SaveText(transcription);
                    }
                    else
                    {
                        Result = e;
                    }
                }
            }
        }

        private static void DeepSpeechClient_OnIntermediateResult(object sender, IntermediateResultEventArgs e)
        {
            try
            {
                Console.WriteLine(e.Transcription);
                //if (_textControl != null) //muestra en tiempo real la transcripción si el elemento es un textbox
                //{
                //    if (!string.IsNullOrWhiteSpace(e.Transcription))
                //    {
                //        _dispatcher.Invoke(() => _textControl.Text = e.Transcription);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }
        }


        /// <summary>
        /// Detiene el reconocimiento y asigna la transcripción al elemento del view seleccionado.
        /// </summary>
        public static void StopRecording(bool append = false)
        {
            _append = append;
            try
            {
                if (_recording)
                {
                    if (DeepSpeechClient != null)
                    {
                        var result = DeepSpeechClient.StopFromButtonAsync();
                    }
                    _recording = false;
                }
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Cierra las alternativas.
        /// </summary>
        internal static void CloseAlternatives()
        {
            if (_suggestionsList != null)
            {
                _suggestionsList.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Inicia el reconocimiento.
        /// </summary>
        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void StartRecording()
        {
            if (_devices != null && _devices.Count != 0)
            {

                _recording = true;
                DeepSpeechClient.StartRecordingFromButton();
            }
        }

        /// <summary>
        /// Selecciona el micrófono para ejecutar el reconocimiento.
        /// </summary>
        /// <param name="name"></param>
        public static void SetMicrophone(string name)
        {
            var recordDevices = new List<MMDevice>(
                MMDeviceEnumerator.EnumerateDevices(DataFlow.Capture, DeviceState.Active));

            if (recordDevices?.Count != 0)
            {
                SelectedDevice = recordDevices.Find(x => x.FriendlyName == name);
                DeepSpeechClient.SetMicrophone(SelectedDevice);
            }
        }

        /// <summary>
        /// Agina el elemento que tiene el focus.
        /// </summary>
        /// <param name="sender">Elemento con el focus.</param>
        /// <param name="dispatcher">Dispatcher del UI principal.</param>
        internal static void SetFocus(object sender, Dispatcher dispatcher)
        {
            //if (_textControl != null) _textControl.KeyDown -= _textControl_KeyDown;
            if (sender is TextBox)
            {
                if (_suggestionsList != null)
                {
                    _suggestionsList.SelectionChanged -= _suggestionsList_SelectionChanged;
                }
                if (_textControl != null)
                {
                    _textControl.TextChanged -= new TextChangedEventHandler(txtAuto_TextChanged);
                }

                _suggestionsList = null;
                _textControl = null;
                _dispatcher = dispatcher;
                _textControl = (TextBox)sender;
                _prevText = _textControl.Text;
                _textControl.KeyDown += _textControl_KeyDown;
                _dateControl = null;
            }
            else if (sender is SfDatePicker)
            {
                _dispatcher = dispatcher;
                _dateControl = (SfDatePicker)sender;
                _textControl = null;
                Console.WriteLine("SfDatePicker focus");
            }

            if (sender == null)
            {
                _dateControl = null;
                _textControl = null;
            }

        }

        private static void _textControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Tab)
            {
                StopRecording(); //Detiene el reconocimiento.

            }
        }

        /// <summary>
        /// Guarda el texto del reconocimiento.
        /// </summary>
        /// <param name="textControl"></param>
        private static void SaveText(TextBox textControl)
        {
            if (textControl != null && !string.IsNullOrWhiteSpace(GlobalSpeech.FileId))
            {
                try
                {
                    File.WriteAllText($"{GlobalSpeech.FileId}.txt", textControl.Text.ToLower(), Encoding.UTF8);
                }
                catch (Exception ex) //se captura porque si falla se requiere que ejecutar código faltante del método padre
                {
                    Telemetry.TrackException(ex);
                }
            }
        }


        /// <summary>
        /// Guarda el texto del reconocimiento.
        /// </summary>
        private static void SaveText(string text)
        {
            if (text != null && !string.IsNullOrWhiteSpace(GlobalSpeech.FileId))
            {
                try
                {
                    File.WriteAllText($"{GlobalSpeech.FileId}.txt", text.ToLower(), Encoding.UTF8);
                }
                catch (Exception ex) //se captura porque si falla se requiere que ejecutar código faltante del método padre
                {
                    Telemetry.TrackException(ex);
                }
            }
        }


        /// <summary>
        /// Agina el elemento que tiene el focus.
        /// </summary>
        /// <param name="sender">Elemento con el focus.</param>
        /// <param name="dispatcher">Dispatcher del UI principal.</param>
        /// <param name="suggestionsList">Listbox anidado del textbox para para mostrar las alternativas.</param>
        internal static void SetFocus(object sender, Dispatcher dispatcher, ListBox suggestionsList)
        {
            if (DeepSpeechClient != null)
            {
                var task = Task.Run(() => DeepSpeechClient?.StopFromButtonAsync());
                task.Wait(millisecondsTimeout: 2000);
            }

            if (_suggestionsList != null)
            {
                _suggestionsList.SelectionChanged -= _suggestionsList_SelectionChanged;
            }
            _suggestionsList = suggestionsList;
            if (_suggestionsList != null)
            {
                _suggestionsList.SelectionChanged += _suggestionsList_SelectionChanged;
            }
            if (_textControl != null)
            {
                _textControl.TextChanged -= new TextChangedEventHandler(txtAuto_TextChanged);
            }
            _dispatcher = dispatcher;
            _textControl = (TextBox)sender;
        }

        private static void _suggestionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suggestionsList.ItemsSource != null) //cuando se selecciona una transcripción alternativa actualiza el textbox.
            {
                _suggestionsList.Visibility = Visibility.Collapsed;
                _textControl.TextChanged -= new TextChangedEventHandler(txtAuto_TextChanged);
                if (_suggestionsList.SelectedIndex != -1)
                {
                    if (_append) _textControl.Text = _lastText?.Trim() + " " + _suggestionsList.SelectedItem.ToString();
                    else _textControl.Text = _suggestionsList.SelectedItem.ToString();
                    _textControl.Focus();
                    SaveText(_textControl);
                    _suggestionsList.Visibility = Visibility.Collapsed;
                }
                _textControl.TextChanged += new TextChangedEventHandler(txtAuto_TextChanged);
            }
        }

        private static void txtAuto_TextChanged(object sender, TextChangedEventArgs e)
        {
            SaveText(_textControl);
            UpdateAlternatives();
        }

        /// <summary>
        /// Actualiza las alternativas.
        /// </summary>
        private static void UpdateAlternatives()
        {
            _textControl.Dispatcher.Invoke(() =>
            {
                if (_textControl != null)
                {
                    if (_suggestionsList != null)
                    {

                        try
                        {
                            string typedString = _textControl.Text;
                            List<string> autoList = new List<string>();
                            autoList.Clear();

                            foreach (string item in Alternatives)
                            {
                                if (!string.IsNullOrEmpty(_textControl.Text))
                                {
                                    autoList.Add(item.ToNumber().ReplaceSymbols().ToUpper());
                                }
                            }

                            if (autoList.Count > 0)
                            {
                                _suggestionsList.ItemsSource = autoList;
                                _suggestionsList.Visibility = Visibility.Visible;
                            }
                            else if (_textControl.Text.Equals(""))
                            {
                                _suggestionsList.Visibility = Visibility.Collapsed;
                                _suggestionsList.ItemsSource = null;
                            }
                            else
                            {
                                _suggestionsList.Visibility = Visibility.Collapsed;
                                _suggestionsList.ItemsSource = null;
                            }

                        }
                        catch (Exception x)
                        {

                            throw x;
                        }
                    }
                }
            });
        }
    }
}