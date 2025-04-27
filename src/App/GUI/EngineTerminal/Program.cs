using EngineTerminal.Views;
using MessagePack;
using ORBIT9000.Engine.Configuration;
using System.Buffers;
using System.IO.Pipes;
using Terminal.Gui;

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
