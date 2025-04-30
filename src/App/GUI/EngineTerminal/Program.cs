using Terminal.Gui;

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
                    Setting2 = "Text2",
                    Setting3 = "Text3",
                    Setting4 = "Text4",
                    Setting5 = "Text5",
                    Setting6 = "Text6",

                    Setting7 = 5_00,
                    Setting8 = 5_00,
                    Setting9 = 5_00,
                    Setting10 = 5_00,
                    Setting11 = 5_00,
                    Setting12 = 5_00,
                },

                Frame2 = new EngineData
                {
                    Setting1 = 100,
                    Setting2 = 200
                }
            };

            #region example_data
            //var exampleData = new
            //{
            //    Plugins = new
            //    {
            //        BasicInfo = "Info about the engine",
            //        TestInfo = "TestInfo about the engine"

            //        //ActivePlugins = new[]
            //        //{
            //        //        new { Name = "Plugin1", Activated = true },
            //        //        new { Name = "Plugin2", Activated = false },
            //        //        new { Name = "Plugin3", Activated = true }
            //        //}
            //    },

            //    //Threads = new[]
            //    //{
            //    //    new { Name = "Thread1", State = "Running" },
            //    //    new { Name = "Thread2", State = "Stopped" },
            //    //    new { Name = "Thread3", State = "Running" }
            //    //},

            //    //Diagnostics = new[]
            //    //{
            //    //    new { Name = "Diagnostic1", Value = 42 },
            //    //    new { Name = "Diagnostic2", Value = 100 },
            //    //    new { Name = "Diagnostic3", Value = 75 }
            //    //},

            //    //Frame1 = new
            //    //{
            //    //    Colour = new
            //    //    {
            //    //        Text = "ColourSettings",
            //    //        Items = new[] {
            //    //            new {
            //    //                Background = "Black",
            //    //                Text = "White"
            //    //            }
            //    //        }
            //    //    }
            //    //}
            //};
            #endregion

            Application.Init();

            Application.Current.ColorScheme = new ColorScheme
            {
                Normal = Application.Driver.MakeAttribute(Color.White, Color.Blue),
                Focus = Application.Driver.MakeAttribute(Color.Gray, Color.DarkGray),
                HotNormal = Application.Driver.MakeAttribute(Color.BrightBlue, Color.Blue),
                HotFocus = Application.Driver.MakeAttribute(Color.BrightYellow, Color.DarkGray)
            };

            var translator = new Translator(Application.Top,  exampleData);
            IReadOnlyDictionary<string, ValueBinding> bindigs = translator.Translate();

            Application.Init();
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