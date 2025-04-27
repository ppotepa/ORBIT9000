
using Terminal.Gui;

namespace EngineTerminal.Views
{
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

        public void AddText(string text)
        {
            var label = new Label(text)
            {
                X = Pos.Center(),
                Y = Pos.Center()
            };

            Add(label);
        }
    }
}