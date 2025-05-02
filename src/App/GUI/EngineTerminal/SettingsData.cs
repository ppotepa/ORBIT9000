using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Orbit9000.EngineTerminal
{
    public class EngineData : INotifyPropertyChanged
    {
        private int setting1;
        private int setting2;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsValid { get; set; }

        [MaxValue(100)]
        public int Setting1
        {
            get => setting1;
            set => SetProperty(ref setting1, value);
        }

        [MaxValue(15)]
        public int Setting2
        {
            get => setting2;
            set => SetProperty(ref setting2, value);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }
    }

    public class SettingsData : INotifyPropertyChanged
    {
        private string setting1;
        private string setting2;
        private string setting3;
        private string setting4;
        private string setting5;
        private string setting6;
        private int setting7;
        private int setting8;
        private int setting9;
        private int setting10;
        private int setting11;
        private int setting12;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Setting1
        {
            get => setting1;
            set => SetProperty(ref setting1, value);
        }

        public string Setting2
        {
            get => setting2;
            set => SetProperty(ref setting2, value);
        }

        public string Setting3
        {
            get => setting3;
            set => SetProperty(ref setting3, value);
        }

        public string Setting4
        {
            get => setting4;
            set => SetProperty(ref setting4, value);
        }

        public string Setting5
        {
            get => setting5;
            set => SetProperty(ref setting5, value);
        }

        public string Setting6
        {
            get => setting6;
            set => SetProperty(ref setting6, value);
        }

        public int Setting7
        {
            get => setting7;
            set => SetProperty(ref setting7, value);
        }

        public int Setting8
        {
            get => setting8;
            set => SetProperty(ref setting8, value);
        }

        public int Setting9
        {
            get => setting9;
            set => SetProperty(ref setting9, value);
        }

        public int Setting10
        {
            get => setting10;
            set => SetProperty(ref setting10, value);
        }

        public int Setting11
        {
            get => setting11;
            set => SetProperty(ref setting11, value);
        }

        public int Setting12
        {
            get => setting12;
            set => SetProperty(ref setting12, value);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }

            return false;
        }
    }
}