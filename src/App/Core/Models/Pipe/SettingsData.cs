using ORBIT9000.Core.Attributes.UI;
using ReactiveUI;

namespace ORBIT9000.Core.Models.Pipe
{
    public class EngineData : ReactiveObject
    {
        private bool _isValid;
        private int _setting1;
        private int _setting2;

        public bool IsValid
        {
            get => _isValid;
            set => this.RaiseAndSetIfChanged(ref _isValid, value);
        }

        [MaxValue(100)]
        public int Setting1
        {
            get => _setting1;
            set => this.RaiseAndSetIfChanged(ref _setting1, value);
        }

        [MaxValue(15)]
        public int Setting2
        {
            get => _setting2;
            set => this.RaiseAndSetIfChanged(ref _setting2, value);
        }
    }

    public class SettingsData : ReactiveObject
    {
        private int _setting1;
        private string _setting2;

        public int Setting1
        {
            get => _setting1;
            set => this.RaiseAndSetIfChanged(ref _setting1, value);
        }

        public string Setting2
        {
            get => _setting2;
            set => this.RaiseAndSetIfChanged(ref _setting2, value);
        }
    }
}