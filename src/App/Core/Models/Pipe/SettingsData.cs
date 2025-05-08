using ReactiveUI;

namespace ORBIT9000.Core.Models.Pipe
{
    public class EngineData : ReactiveObject
    {
        private bool _isValid;
        private int _setting1;
        private int _setting2;
        private int _setting3;
        private int _setting4;
        private int _setting5;
        private int _setting6;
        private int _setting7;
        private int _setting8;
        private int _setting9;
        private int _setting10;
        private int _setting11;
        private int _setting12;

        public bool IsValid
        {
            get => _isValid;
            set => this.RaiseAndSetIfChanged(ref _isValid, value);
        }

        //[MaxValue(100)]
        public int Setting1
        {
            get => _setting1;
            set => this.RaiseAndSetIfChanged(ref _setting1, value);
        }

        //[MaxValue(15)]
        public int Setting2
        {
            get => _setting2;
            set => this.RaiseAndSetIfChanged(ref _setting2, value);
        }

        public int Setting3
        {
            get => _setting3;
            set => this.RaiseAndSetIfChanged(ref _setting3, value);
        }

        public int Setting4
        {
            get => _setting4;
            set => this.RaiseAndSetIfChanged(ref _setting4, value);
        }

        public int Setting5
        {
            get => _setting5;
            set => this.RaiseAndSetIfChanged(ref _setting5, value);
        }

        public int Setting6
        {
            get => _setting6;
            set => this.RaiseAndSetIfChanged(ref _setting6, value);
        }

        public int Setting7
        {
            get => _setting7;
            set => this.RaiseAndSetIfChanged(ref _setting7, value);
        }

        public int Setting8
        {
            get => _setting8;
            set => this.RaiseAndSetIfChanged(ref _setting8, value);
        }

        public int Setting9
        {
            get => _setting9;
            set => this.RaiseAndSetIfChanged(ref _setting9, value);
        }

        public int Setting10
        {
            get => _setting10;
            set => this.RaiseAndSetIfChanged(ref _setting10, value);
        }

        public int Setting11
        {
            get => _setting11;
            set => this.RaiseAndSetIfChanged(ref _setting11, value);
        }

        public int Setting12
        {
            get => _setting12;
            set => this.RaiseAndSetIfChanged(ref _setting12, value);
        }
    }

    public class SettingsData : ReactiveObject
    {
        private int _setting1;
        private string _setting2;
        private string _setting3;
        private string _setting4;
        private string _setting5;
        private string _setting6;
        private string _setting7;
        private string _setting8;
        private string _setting9;
        private string _setting10;
        private string _setting11;
        private string _setting12;

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

        public string Setting3
        {
            get => _setting3;
            set => this.RaiseAndSetIfChanged(ref _setting3, value);
        }

        public string Setting4
        {
            get => _setting4;
            set => this.RaiseAndSetIfChanged(ref _setting4, value);
        }

        public string Setting5
        {
            get => _setting5;
            set => this.RaiseAndSetIfChanged(ref _setting5, value);
        }

        public string Setting6
        {
            get => _setting6;
            set => this.RaiseAndSetIfChanged(ref _setting6, value);
        }

        public string Setting7
        {
            get => _setting7;
            set => this.RaiseAndSetIfChanged(ref _setting7, value);
        }

        public string Setting8
        {
            get => _setting8;
            set => this.RaiseAndSetIfChanged(ref _setting8, value);
        }

        public string Setting9
        {
            get => _setting9;
            set => this.RaiseAndSetIfChanged(ref _setting9, value);
        }

        public string Setting10
        {
            get => _setting10;
            set => this.RaiseAndSetIfChanged(ref _setting10, value);
        }

        public string Setting11
        {
            get => _setting11;
            set => this.RaiseAndSetIfChanged(ref _setting11, value);
        }

        public string Setting12
        {
            get => _setting12;
            set => this.RaiseAndSetIfChanged(ref _setting12, value);
        }
    }
}