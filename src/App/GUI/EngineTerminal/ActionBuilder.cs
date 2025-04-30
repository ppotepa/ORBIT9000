using NStack;
using Orbit9000.EngineTerminal;
using Terminal.Gui;

namespace EngineTerminal
{
    public class ActionBuilder
    {
        private ValueBinding targetBinding;
        private TextField valueField;

        private List<Action<ustring>> Actions { get; set; } = new List<Action<ustring>>();

        public ActionBuilder AddPost(Action<ustring> action)
        {
            this.Actions.Add(action);
            return this;
        }

        public ActionBuilder AddPre(Action<ustring> action)
        {
            this.Actions = this.Actions.Prepend(action).ToList();
            return this;
        }

        public Action<ustring> Build()
        {
            return (s) =>
            {
                Actions.ForEach(action =>
                {
                    action(s);
                });
            };
        }

        public ActionBuilder Default(TextField valueField, ValueBinding targetBinding)
        {
            this.valueField = valueField;
            this.targetBinding = targetBinding;

            Actions.Add((s) =>
            {
                Console.Title += targetBinding.GetHashCode().ToString();
                targetBinding.Value = valueField.Text.ToString();
            });

            return this;
        }

        internal ActionBuilder AddIf(bool v, Func<object, int> value)
        {
            Actions.Add((s) =>
            {
                if (v)
                {
                    value(null);
                }
            });

            return this;
        }
    }
}