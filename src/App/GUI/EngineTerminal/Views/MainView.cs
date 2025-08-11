
using Terminal.Gui;

namespace EngineTerminal.Views
{
    public partial class MainView
    {
        public MainView()
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
}