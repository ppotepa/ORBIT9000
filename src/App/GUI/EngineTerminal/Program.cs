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

namespace Orbit9000.EngineTerminal
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Application.Init();

            var top = Application.Top;

            var menu = new MenuBar(new MenuBarItem[]
            {
                    new MenuBarItem("Engine", new MenuItem[]
                    {
                        new MenuItem("Start", "", () => {}),
                        new MenuItem("Stop", "", () => {})
                    }),

                    new MenuBarItem("Plugins", new MenuItem[]
                    {
                        new MenuItem("Load", "", () => {}),
                        new MenuItem("Unload", "", () => {}),
                        new MenuItem("Activity", "", () => {})
                    }),
                    new MenuBarItem("Diagnostics", new MenuItem[]
                    {
                        new MenuItem("UpTime", "", () => {})
                    })
            });

            top.Add(menu);

            var textView = new TextView
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true
            };

            var mainWindow = new MainWindow();
            top.Add(mainWindow);

            var window = new Window("Engine Terminal")
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            window.Add(textView);
            top.Add(window);

            var backgroundThread = new Thread(() =>
            {
                var client = new NamedPipeClientStream(".", "OrbitEngine", PipeDirection.In);

                try
                {
                    client.Connect();
                    Application.MainLoop.Invoke(() => textView.Text += "Connected to engine!\n");

                    var buffer = new byte[4096];

                    while (true)
                    {
                        try
                        {
                            int bytesRead = client.Read(buffer, 0, buffer.Length);
                            if (bytesRead == 0)
                            {
                                Application.MainLoop.Invoke(() => textView.Text += "Server closed connection.\n");
                                break;
                            }

                            List<PluginInfo> @object = MessagePackSerializer.Deserialize<List<PluginInfo>>(new ReadOnlySequence<byte>(buffer, 0, bytesRead));

                            Application.MainLoop.Invoke(() =>
                            {
                                mainWindow.AddText($"Received Engine state: {@object.Count}\n");

                                foreach (var pluginInfo in @object)
                                {
                                    textView.Text += $"Plugin: {pluginInfo.PluginType}, Activated: {pluginInfo.Activated}\n";
                                }
                            });
                        }
                        catch (IOException ex)
                        {
                            Application.MainLoop.Invoke(() => textView.Text += $"Pipe broken: {ex.Message}\n");
                            break;
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Application.MainLoop.Invoke(() => textView.Text += $"Error connecting to pipe: {ex.Message}\n");
                }
            });

            backgroundThread.IsBackground = true;
            backgroundThread.Start();

            Application.Run();
        }
    }
}
>>>>>>> 590e002 (Add Temporary NamedPipe and Receiving Console App)
