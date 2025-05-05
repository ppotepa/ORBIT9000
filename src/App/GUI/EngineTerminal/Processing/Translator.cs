using EngineTerminal.Bindings;
using EngineTerminal.Builders;
using NStack;
using ORBIT9000.Core.Models.Pipe;
using System.ComponentModel;
using System.Reflection;
using Terminal.Gui;

namespace EngineTerminal.Processing
{
    public class Translator
    {
        private const BindingFlags NOT_INHERITED = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
        private readonly Dictionary<string, ValueBinding> _bindings = new();
        private readonly int _cols = 5, _rows = 5;
        private readonly ExampleData _data;
        private readonly List<FrameView> _frames = new();
        private readonly View _top;
        #region secret
        private readonly ustring SECRET =
@"
┌──────────────────────────────────────────────┐
│ WHEN YOU’RE POLISH, LOVE KARATE,            │
│ AND VISIT 104 COUNTRIES AS POPE            │
└──────────────────────────────────────────────┘

             .-""""-.
           .'        '.
          /   _  _     \
         |   (o)(o)     |
         |     __       |
         |   .'  '.     |
         |  |      |    |
         |   '.  .'     |
          \    '--'    /
           '.        .'
             '-.__.-'

             /|     |\
            /_|_____|_\
              |     |
              |     |
              |     |
             /|     |\
            /_|_____|_\

┌──────────────────────────────────────────────┐
│ “I TRAVELLED MORE THAN YOUR AVERAGE PILGRIM”│
└──────────────────────────────────────────────┘";

        #endregion secret
        private View _main;

        public Translator(View top, ExampleData data, int rows = 5, int cols = 5)
        {
            _top = top;
            _data = data;
            _rows = rows;
            _cols = cols;
            _main = new FrameView("Main") { X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() };
        }

        public Dictionary<string, ValueBinding> Translate()
        {
            PropertyInfo[] props = _data.GetType().GetProperties(NOT_INHERITED);
            MenuBarItem[] items = new MenuBarItem[props.Length];

            for (int i = 0; i < props.Length; i++)
            {
                PropertyInfo info = props[i];
                FrameView frame = new FrameView(info.Name) { X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() };

                items[i] = new MenuBarItem(info.Name, "", () =>
                {
                    _main.RemoveAll();
                    _main.Add(frame);
                    Application.Refresh();
                });

                GeneratePropertyGrid(frame, info);                
            }

            MenuBar menu = new MenuBar(items) { X = 0 };

            _top.Add(menu, _main);
            return _bindings;
        }

        private void GeneratePropertyGrid(FrameView container, PropertyInfo info)
        {
            var val = info.GetValue(_data);

            if (info.PropertyType.IsClass && info.PropertyType.GetProperties(NOT_INHERITED).Length > 0 && val != null)
            {
                foreach (var subProperty in info.PropertyType.GetProperties(NOT_INHERITED))
                {
                    var subValue = subProperty.GetValue(val);
                    var route = $"{info.Name}.{subProperty.Name}";

                    var index = container.Subviews[0].Subviews.Count;

                    var frameItem = new View()
                    {
                        X = Pos.Function(() => (index / 10) * 30),
                        Y = Pos.Function(() => index % 10),

                        Width = 30,
                        Height = 1,
                    };

                    var label = new Label(0, 0, subProperty.Name, true);
                    var text = new TextField(15, 0, 30, subValue?.ToString() ?? "");

                    text.Id = route;

                    ValueBinding binding = new ValueBinding(text, ref subValue!);

                    _bindings[route] = binding;

                    text.TextChanged += PipelineFactory.Instance.Builder
                        .Create(text, binding, val!, subProperty)
                        .AddIf(() => text.Text == "2137", _ => MessageBox.Query("Secret", SECRET, "OK"))
                        .Build();

                    frameItem.Add(label, text);
                    container.Add(frameItem);
                }
            }
        }
    }
}
