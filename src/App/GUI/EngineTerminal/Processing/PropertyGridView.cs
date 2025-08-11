using EngineTerminal.Bindings;
using EngineTerminal.Builders;
using NStack;
using ORBIT9000.Core.Models.Pipe;
using System.Reflection;
using Terminal.Gui;

namespace EngineTerminal.Processing
{
    public class PropertyGridView : View
    {
        public event EventHandler<BindingChangedEventArgs>? BindingChanged;

        protected virtual void OnBindingChanged(string propertyName, object? oldValue, object? newValue)
        {
            BindingChanged?.Invoke(this, new BindingChangedEventArgs(propertyName, oldValue, newValue));
        }

        public class BindingChangedEventArgs : EventArgs
        {
            public string PropertyName { get; }
            public object? OldValue { get; }
            public object? NewValue { get; }

            public BindingChangedEventArgs(string propertyName, object? oldValue, object? newValue)
            {
                PropertyName = propertyName;
                OldValue = oldValue;
                NewValue = newValue;
            }
        }
        private const BindingFlags NOT_INHERITED = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

        private readonly int _cols, _rows;
        private readonly object _data;        
        private readonly View _top;

     
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
        public readonly Dictionary<string, ValueBinding> Bindings = new Dictionary<string, ValueBinding>();

      
        private View _main;

        public PropertyGridView(View top, object data, int rows = 5, int cols = 5)
        {
            _top = top;
            _data = data;
            _rows = rows;
            _cols = cols;

            _main = new FrameView("Main") 
            {
                X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() 
            };

            this.Bindings = TranslateView();
        }

        public override void Redraw(Rect bounds)
        {
            TranslateView();
            base.Redraw(bounds);
        }
        private Dictionary<Type, PropertyInfo[]> _propertyInfoCache = new();

        public Dictionary<string, ValueBinding> TranslateView()
        {
            Type dataType = _data.GetType();
            if (!_propertyInfoCache.TryGetValue(dataType, out PropertyInfo[] properties))
            {
                properties = dataType.GetProperties(NOT_INHERITED);
                _propertyInfoCache[dataType] = properties;
            }

            MenuBarItem[] items = new MenuBarItem[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo info = properties[i];
                FrameView frame = new FrameView(info.Name) { X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() };

                items[i] = new MenuBarItem(info.Name, "", () =>
                {
                    _main.RemoveAll();
                    _main.Add(frame);

                    Application.Refresh();
                });

                GeneratePropertyGrid(frame, info);
            }

            var menuBar = Application.Top.Subviews.OfType<MenuBar>()
              .FirstOrDefault();

            if (menuBar is not null)
            {
                menuBar.Data = items;
                _top.Add(_main);
            }
            else
            {
                menuBar = new MenuBar(items);
                _top.Add(menuBar, _main);
            }

            return this.Bindings;
        }

        private void GeneratePropertyGrid(FrameView container, PropertyInfo info)
        {
            var val = info.GetValue(_data);

            if (info.PropertyType.IsClass && info.PropertyType.GetProperties(NOT_INHERITED).Length > 0 && val != null)
            {
                int index = 0;
                foreach (var subProperty in info.PropertyType.GetProperties(NOT_INHERITED))
                {
                    var subValue = subProperty.GetValue(val);
                    var route = $"{info.Name}.{subProperty.Name}";

                    var frameItem = new View()
                    {
                        X = Pos.Percent((index % _cols) * (100 / _cols)),
                        Y = Pos.Percent((index / _cols) * (100 / _rows)),

                        Width = Dim.Percent(100 / _cols),
                        Height = Dim.Percent(100 / _rows),
                    };

                    var label = new Label(0, 0, subProperty.Name, true);
                    var text = new TextField(15, 0, 20, subValue?.ToString() ?? "");

                    text.Id = route;

                    ValueBinding binding = new ValueBinding(text, ref subValue!);

                    Bindings[route] = binding;

                    text.TextChanged += PipelineFactory.Instance.Builder
                        .Create(text, binding, val!, subProperty)
                        .AddIf(() => text.Text == "2137", _ => MessageBox.Query("Secret", SECRET, "OK"))
                        .Build();

                    frameItem.Add(label, text);
                    container.Add(frameItem);

                    index++;
                }
            }
        }
    }
}
