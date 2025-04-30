using NStack;
using Orbit9000.EngineTerminal;
using System.Reflection;
using Terminal.Gui;

namespace EngineTerminal
{
    public class ActionPipelineBuilder
    {
        private ValueBinding targetBinding = default;
        private TextField valueField = default;

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

        public ActionPipelineBuilder Create(TextField valueField, ValueBinding targetBinding, object parent, FieldInfo info)
        {
            this.valueField = valueField;
            this.targetBinding = targetBinding;      
            
            Actions.Add((s) =>
            {
                info.SetValue(parent, this.valueField.Text.ToString());
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