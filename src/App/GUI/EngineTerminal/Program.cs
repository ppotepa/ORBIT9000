<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
﻿using EngineTerminal.Contracts;
using EngineTerminal.Managers;
using Microsoft.Extensions.DependencyInjection;
using ORBIT9000.Core.Models.Pipe;
using ORBIT9000.Engine;
using System.Threading.Channels;

namespace EngineTerminal
{
    public static class Program
    {
        #region Methods

        public static async Task Main(string[] args)
        {
            if (args is not null)
            {
                Channel<ExampleData> dataChannel = Channel.CreateUnbounded<ExampleData>();
                Channel<string> statusChannel = Channel.CreateUnbounded<string>();

                ServiceCollection services = new();

                services.AddSingleton(dataChannel);
                services.AddSingleton(statusChannel);

                services.AddSingleton<IDataManager, DataManager>();
                services.AddSingleton<IUIManager, UIManager>();

                services.AddSingleton<IPipeManager>(provider =>
                {
                    Channel<ExampleData> dataChannel = provider.GetRequiredService<Channel<ExampleData>>();
                    Channel<string> propertyChannel = provider.GetRequiredService<Channel<string>>();

                    return new NamedPipeManager(dataChannel, propertyChannel, ".", nameof(OrbitEngine));
                });

                services.AddSingleton<ApplicationController>();

                ServiceProvider provider = services.BuildServiceProvider();
                ApplicationController app = provider.GetRequiredService<ApplicationController>();

                await app.RunAsync();
            }
            else
            {
                throw new ArgumentNullException(nameof(args));
            }
        }

        #endregion Methods
    }
}
=======
﻿using System.IO.Pipes;
using System.Text;
=======
﻿using MessagePack;
using ORBIT9000.Engine.Configuration;
using System.Buffers;
using System.IO.Pipes;
>>>>>>> 1aafd5a (Add Basic Messaging)
=======
﻿using EngineTerminal.Views;
using MessagePack;
using ORBIT9000.Engine.Configuration;
using System.Buffers;
using System.IO.Pipes;
using Terminal.Gui;
>>>>>>> 4502f33 (Add GUI BoilerPlate)
=======
﻿using Terminal.Gui;
>>>>>>> f9f63ea (Add Simple Html View Parsing to Terminal PoC)
=======
﻿using System.ComponentModel;
=======
﻿using EngineTerminal.Bindings;
using EngineTerminal.Data;
using EngineTerminal.Processors;
using MessagePack;
using ORBIT9000.Engine.Configuration;
using System.Buffers;
using System.ComponentModel;
<<<<<<< HEAD
>>>>>>> 9942610 (Update Project Structure)
=======
using System.IO.Pipes;
>>>>>>> 122b62b (Fix Engine Main Thread)
using Terminal.Gui;
>>>>>>> 72c40c3 (Add Basic Event Handling for Settings)

namespace Orbit9000.EngineTerminal
{
    class Program
    {
        static void Main(string[] args)
        {
            ExampleData exampleData = new ExampleData
            {
                Frame1 = new SettingsData
                {
                    Setting1 = "Text1",
                    Setting2 = "Text2"

                },

                Frame2 = new EngineData
                {
                    Setting1 = 100,
                    Setting2 = 200,
                    IsValid = false
                }
            };

            exampleData.Frame2.PropertyChanged += Notification;
            exampleData.Frame1.PropertyChanged += Notification;

            Application.Init();

            Application.Current.ColorScheme = new ColorScheme
            {
                Normal = Application.Driver.MakeAttribute(Color.White, Color.Blue),
                Focus = Application.Driver.MakeAttribute(Color.Gray, Color.DarkGray),
                HotNormal = Application.Driver.MakeAttribute(Color.BrightBlue, Color.Blue),
                HotFocus = Application.Driver.MakeAttribute(Color.BrightYellow, Color.DarkGray)
            };

            var translator = new Translator(Application.Top, exampleData);
            IReadOnlyDictionary<string, ValueBinding> bindings = translator.Translate();

            var messageItem = new StatusItem(Key.Null, "NO MESSAGES", null);
            var statusBar = new StatusBar(new StatusItem[]
            {
                new StatusItem(Key.F1, "~F1~ Help", null),
                messageItem
            });

            Application.Top.Add(statusBar);

            var backgroundThread = new Thread(() =>
            {
                var client = new NamedPipeClientStream(".", "OrbitEngine", PipeDirection.In);

                try
                {
                    client.Connect();
                    Application.MainLoop.Invoke(() => messageItem.Title = "Connected to engine!");
                    Application.Refresh();

                    var buffer = new byte[4096];

                    while (true)
                    {
                        try
                        {
                            int bytesRead = client.Read(buffer, 0, buffer.Length);
                            if (bytesRead == 0)
                            {
                                Application.MainLoop.Invoke(() => messageItem.Title = "Server closed connection.");
                                Application.Refresh();
                                break;
                            }

                            List<PluginInfo> @object = MessagePackSerializer.Deserialize<List<PluginInfo>>(new ReadOnlySequence<byte>(buffer, 0, bytesRead));

                            Application.MainLoop.Invoke(() =>
                            {
                                messageItem.Title = $"Received Engine state: {@object.Count}";

                                foreach (var pluginInfo in @object)
                                {
                                    messageItem.Title = $"Plugin: {pluginInfo.PluginType}, Activated: {pluginInfo.Activated}";
                                }

                                Application.Refresh();
                            });
                        }
                        catch (IOException ex)
                        {
                            Application.MainLoop.Invoke(() => messageItem.Title = $"Pipe broken: {ex.Message}");
                            Application.Refresh();
                            break;
                        }
                        
                        Thread.Sleep(50);
                    }
                }
                catch (Exception ex)
                {
                    Application.MainLoop.Invoke(() => messageItem.Title = $"Error connecting to pipe: {ex.Message}");
                }

            });

            backgroundThread.Start();          
            Application.Run();
        }

        private static void Notification(object? sender, PropertyChangedEventArgs e)
        {
            
        }
    }
<<<<<<< HEAD
}
>>>>>>> 590e002 (Add Temporary NamedPipe and Receiving Console App)
=======
}
>>>>>>> f9f63ea (Add Simple Html View Parsing to Terminal PoC)
