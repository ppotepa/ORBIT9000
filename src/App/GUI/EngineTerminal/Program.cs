using Terminal.Gui;

namespace Orbit9000.EngineTerminal
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.Init();
         
            var colorScheme = new ColorScheme
            {
                Normal = Application.Driver.MakeAttribute(Color.White, Color.Blue),
                Focus = Application.Driver.MakeAttribute(Color.Gray, Color.DarkGray),
                HotNormal = Application.Driver.MakeAttribute(Color.BrightBlue, Color.Blue),
                HotFocus = Application.Driver.MakeAttribute(Color.BrightYellow, Color.DarkGray)
            };

            Application.Current.ColorScheme = colorScheme;

            var top = Application.Top;

            var html = System.IO.File.ReadAllText("./views/index.html");

            var translator = new Translator();
            var mainView = translator.Translate(html);

            top.Add(mainView);

            Application.Run();
        }

        #region PipeRegion
        //var backgroundThread = new Thread(() =>
        //    {
        //        var client = new NamedPipeClientStream(".", "OrbitEngine", PipeDirection.In);

        //        try
        //        {
        //            client.Connect();
        //            Application.MainLoop.Invoke(() => textView.Text += "Connected to engine!\n");

        //            var buffer = new byte[4096];

        //            while (true)
        //            {
        //                try
        //                {
        //                    int bytesRead = client.Read(buffer, 0, buffer.Length);
        //                    if (bytesRead == 0)
        //                    {
        //                        Application.MainLoop.Invoke(() => textView.Text += "Server closed connection.\n");
        //                        break;
        //                    }

        //                    List<PluginInfo> @object = MessagePackSerializer.Deserialize<List<PluginInfo>>(new ReadOnlySequence<byte>(buffer, 0, bytesRead));

        //                    Application.MainLoop.Invoke(() =>
        //                    {
        //                        textView.Text += ($"Received Engine state: {@object.Count}\n");

        //                        foreach (var pluginInfo in @object)
        //                        {
        //                            textView.Text += $"Plugin: {pluginInfo.PluginType}, Activated: {pluginInfo.Activated}\n";
        //                        }
        //                    });
        //                }
        //                catch (IOException ex)
        //                {
        //                    Application.MainLoop.Invoke(() => textView.Text += $"Pipe broken: {ex.Message}\n");
        //                    break;
        //                }

        //                Thread.Sleep(1000);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Application.MainLoop.Invoke(() => textView.Text += $"Error connecting to pipe: {ex.Message}\n");
        //        }
        //    }); 
        #endregion
    }
}