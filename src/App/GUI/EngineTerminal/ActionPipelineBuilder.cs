using NStack;
using Orbit9000.EngineTerminal;
using Terminal.Gui;

namespace EngineTerminal
{
    public class ActionPipelineBuilder
    {
        private ValueBinding targetBinding;
        private TextField valueField;

        private List<Action<ustring>> Actions { get; set; } = new List<Action<ustring>>();

        public ActionPipelineBuilder AddPost(Action<ustring> action)
        {
            this.Actions.Add(action);
            return this;
        }

        public ActionPipelineBuilder AddPre(Action<ustring> action)
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

        public ActionPipelineBuilder Default(TextField valueField, ValueBinding targetBinding)
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

        internal ActionPipelineBuilder AddIf(Func<bool> condition, Func<ValueBinding, int> action)
        {
            Actions.Add((s) =>
            {
                if (condition())
                {
                    action(targetBinding);
                }
            });

            return this;
        }
    }
}