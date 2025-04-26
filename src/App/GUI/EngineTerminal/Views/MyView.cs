
using Terminal.Gui;

namespace EngineTerminal.Views
{
    public partial class MyView
    {
        public MyView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            ColorScheme = new ColorScheme
            {
                Normal = Application.Driver.MakeAttribute(Color.White, Color.Blue),
                Focus = Application.Driver.MakeAttribute(Color.Black, Color.Blue),
                HotNormal = Application.Driver.MakeAttribute(Color.BrightBlue, Color.Blue),
                HotFocus = Application.Driver.MakeAttribute(Color.BrightBlue, Color.Blue)
            };

            this.Width = Dim.Fill(0);
            this.Height = Dim.Fill(0);
            this.X = null;
            this.Y = null;
            this.Visible = true;
            this.Modal = false;
            this.IsMdiContainer = false;
            this.Border.BorderStyle = Terminal.Gui.BorderStyle.Single;
            this.Border.Effect3D = true;
            this.Border.Effect3DBrush = null;
            this.Border.DrawMarginFrame = true;
            this.TextAlignment = Terminal.Gui.TextAlignment.Left;
            this.Title = "";
        }
    }

    class MainWindow : Window
    {
        public MainWindow() : base("My First Window")
        {
            X = 0;
            Y = 1; // Leave space for the top menu
            Width = Dim.Fill();
            Height = Dim.Fill();

            ColorScheme = new ColorScheme
            {
                Normal = Application.Driver.MakeAttribute(Color.White, Color.Blue),
                Focus = Application.Driver.MakeAttribute(Color.Black, Color.Blue),
                HotNormal = Application.Driver.MakeAttribute(Color.BrightBlue, Color.Blue),
                HotFocus = Application.Driver.MakeAttribute(Color.BrightBlue, Color.Blue)
            };

            var label = new Label("Hello, OrbitEngine!")
            {
                X = Pos.Center(),
                Y = Pos.Center()
            };

            Add(label);
        }
    }

    class Program
    {
        static void Main()
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

            var mainWindow = new MainWindow();
            top.Add(mainWindow);

            Application.Run();
        }
    }
}