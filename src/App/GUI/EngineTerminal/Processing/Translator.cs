using EngineTerminal.Bindings;
using EngineTerminal.Builders;
using NStack;
using ORBIT9000.Core.Models.Pipe;
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
                PropertyInfo pi = props[i];
                FrameView frame = new FrameView(pi.Name) { X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() };

                items[i] = new MenuBarItem(pi.Name, "", () =>
                {
                    _main.RemoveAll();
                    _main.Add(frame);
                    Application.Refresh();
                });

                BuildFrame(frame, pi);
            }

            MenuBar menu = new MenuBar(items) { X = 0 };
            _top.Add(menu, _main);
            return _bindings;
        }

        private void BuildFrame(FrameView container, PropertyInfo pi)
        {
            var val = pi.GetValue(_data);
            var index = 0;
            if (pi.PropertyType.IsClass && pi.PropertyType.GetProperties(NOT_INHERITED).Length > 0 && val != null)
            {
                foreach (var subProperty in pi.PropertyType.GetProperties(NOT_INHERITED))
                {
                    var subValue = subProperty.GetValue(val);
                    var route = $"{pi.Name}.{subProperty.Name}";
                    var row = index / _cols;
                    var col = index % _cols;

                    var frameItem = new FrameView(route)
                    {
                        X = col * 30,
                        Y = row * 5,
                        Width = Dim.Percent(20),
                        Height = Dim.Percent(20)
                    };

                    var label = new Label(0, 0, "Value:");
                    var text = new TextField(15, 0, 20, subValue?.ToString() ?? "");

                    text.Id = route;

                    ValueBinding binding = new ValueBinding(text, ref subValue!);

                    _bindings[route] = binding;

                    text.TextChanged += PipelineFactory.Instance.Builder
                        .Create(text, binding, val!, subProperty)
                        .AddIf(() => text.Text == "2137",
                               _ => MessageBox.Query("Secret", SECRET, "OK"))
                        .Build();

                    frameItem.Add(label, text);
                    container.Add(frameItem);

                    index++;
                }
            }
        }
    }
}
