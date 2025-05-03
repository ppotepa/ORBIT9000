using EngineTerminal.Bindings;
using EngineTerminal.Processors;
using MessagePack;
using MessagePack.Resolvers;
using ORBIT9000.Core.Models.Pipe;
using System.Buffers;
using System.ComponentModel;
using System.IO.Pipes;
using System.Reflection;
using Terminal.Gui;

namespace Orbit9000.EngineTerminal
{
    public class Program
    {
        private static Dictionary<string, ValueBinding> _bindings;
        private static CancellationTokenSource _cancellationTokenSource;
        private static NamedPipeClientStream _client;
        private static ExampleData _exampleData;
        private static StatusItem _messageStatusItem;

        static void Main(string[] args)
        {
            _exampleData = CreateInitialData();
            _cancellationTokenSource = new CancellationTokenSource();

            InitializeUI();

            try
            {
                StartBackgroundProcessing();
                Application.Run();
            }
            finally
            {
                _cancellationTokenSource.Cancel();
                _client?.Dispose();
            }
        }

        #region Application Setup

        private static ExampleData CreateInitialData()
        {
            var data = new ExampleData
            {
                Frame1 = new SettingsData
                {
                    Setting1 = 1,
                    Setting2 = "Text2"
                },
                Frame2 = new EngineData
                {
                    Setting1 = 100,
                    Setting2 = 200,
                    IsValid = false
                }
            };

            data.Frame1.PropertyChanged += OnPropertyChanged;
            data.Frame2.PropertyChanged += OnPropertyChanged;

            return data;
        }

        private static void InitializeUI()
        {
            Application.Init();

            var topLayer = new FrameView("TOP LAYER")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                CanFocus = true
            };

            SetupColorScheme();
            
            var translator = new Translator(topLayer, _exampleData);            

            _bindings = translator.Translate();
            _messageStatusItem = new StatusItem(Key.Null, "Starting...", null);

            StatusBar statusBar = new StatusBar(
            [
                new StatusItem(Key.F1, "~F1~ Help", () => ShowHelp()),
                _messageStatusItem
            ]);

            Application.Top.Add(topLayer);
            Application.Top.Add(statusBar);
        }

        private static void SetupColorScheme()
        {
            Application.Current.ColorScheme = new ColorScheme
            {
                Normal = Application.Driver.MakeAttribute(Color.White, Color.Blue),
                Focus = Application.Driver.MakeAttribute(Color.Gray, Color.DarkGray),
                HotNormal = Application.Driver.MakeAttribute(Color.BrightBlue, Color.Blue),
                HotFocus = Application.Driver.MakeAttribute(Color.BrightYellow, Color.DarkGray)
            };
        }

        private static void ShowHelp()
        {
            MessageBox.Query("Help", "Engine Terminal\n\nUse menus to navigate between views.\nValues update automatically.", "OK");
        }

        #endregion

        #region Data Processing

        private static async Task ProcessDataFromPipe(CancellationToken cancellationToken)
        {
            _client = new NamedPipeClientStream(".", "OrbitEngine", PipeDirection.In);

            var options = MessagePackSerializerOptions.Standard.WithResolver(
                CompositeResolver.Create(
                    ContractlessStandardResolver.Instance,
                    StandardResolver.Instance
                )
            );

            try
            {
                UpdateStatusMessage("Connecting to engine...");

                await _client.ConnectAsync(cancellationToken);

                UpdateStatusMessage("Connected to engine!");

                byte[] buffer = new byte[4096];

                UpdateStatusMessage("Awaiting data...");

                while (!cancellationToken.IsCancellationRequested)
                {                    
                    try
                    {
                        int bytesRead = await _client.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        if (bytesRead == 0)
                        {
                            UpdateStatusMessage("Server closed connection.");
                            break;
                        }
                        var receivedData = MessagePackSerializer.Deserialize<ExampleData>(
                            new ReadOnlySequence<byte>(buffer, 0, bytesRead),
                            options
                        );

                        UpdateDataAndUI(receivedData, _exampleData);
                        UpdateStatusMessage($"Received Engine state update {new Random().Next(1, 100)}");
                    }
                    catch (IOException ex)
                    {
                        UpdateStatusMessage($"Pipe broken: {ex.Message}");
                        break;
                    }
                    catch (Exception ex)
                    {
                        UpdateStatusMessage($"Error processing data: {ex.Message}");
                    }

                    await Task.Delay(50, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                UpdateStatusMessage($"Error connecting to pipe: {ex.Message}");
            }
        }

        private static void StartBackgroundProcessing()
        {
            Task.Run(() => ProcessDataFromPipe(_cancellationTokenSource.Token));
        }
        #endregion

        #region UI Updates

        private static void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string GetPropertyPath(object obj, string propertyName)
            {
                if (obj == null) return propertyName;

                var parentProperty = _exampleData.GetType().GetProperties()
                    .FirstOrDefault(p => p.GetValue(_exampleData) == obj);

                return parentProperty != null
                    ? $"{parentProperty.Name}.{propertyName}"
                    : propertyName;
            }

            string propertyPath = GetPropertyPath(sender, e.PropertyName);

            if (_bindings.TryGetValue(propertyPath, out var binding))
            {
                object? propertyValue = sender.GetType().GetProperty(e.PropertyName)?.GetValue(sender);

                Application.MainLoop.Invoke(() =>
                {
                    _bindings[propertyPath].Value = propertyValue;

                    binding.View.SetNeedsDisplay();
                    Application.Refresh();
                });
            }
        }

        private static void UpdateDataAndUI(ExampleData source, ExampleData target)
        {
            Application.MainLoop.Invoke(() =>
            {
                UpdateObjectProperties(source.Frame1, target.Frame1, "SettingsData");
                UpdateObjectProperties(source.Frame2, target.Frame2, "EngineData");

                Application.Refresh();
            });
        }

        private static void UpdateObjectProperties(object source, object target, string objectTypeName)
        {
            if (source == null || target == null) return;

            var properties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite)
                    continue;

                var sourceValue = property.GetValue(source);
                property.SetValue(target, sourceValue);
            }

            Application.Refresh();
        }

        private static void UpdateStatusMessage(string message)
        {
            Application.MainLoop.Invoke(() => _messageStatusItem.Title = message);
        }

     
        #endregion
    }
}
