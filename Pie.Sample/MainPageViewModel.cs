using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Pie.Sample
{
	public class MainPageViewModel : INotifyPropertyChanged
    {
        private bool _isHalfCircle;
        public bool IsHalfCircle
        {
            get { return _isHalfCircle; }
            set
            {
                if (_isHalfCircle != value)
                {
                    _isHalfCircle = value;
                    OnPropertyChanged(nameof(IsHalfCircle));
                }
            }
        }

        private List<double> _values;
        public List<double> Values
        {
            get { return _values; }
            set
            {
                if (_values != value)
                {
                    _values = value;
                    OnPropertyChanged(nameof(Values));
                }
            }
        }

        private Color _pieColor;
        public Color PieColor
        {
            get { return _pieColor; }
            set
            {
                if (_pieColor != value)
                {
                    _pieColor = value;
                    OnPropertyChanged(nameof(PieColor));
                }
            }
        }

        private List<Color> _pieColors;
        public List<Color> PieColors
        {
            get { return _pieColors; }
            set
            {
                if (_pieColors != value)
                {
                    _pieColors = value;
                    OnPropertyChanged(nameof(PieColors));
                }
            }
        }

        public MainPageViewModel()
		{
            ChangeHalfCircleCommand = new Command(ChangeHalfCircle);
            ChangeValues1Command = new Command(ChangeValues1);
            ChangeValues2Command = new Command(ChangeValues2);
            ChangeValues3Command = new Command(ChangeValues3);
            ChangeValues4Command = new Command(ChangeValues4);
            ChangeValues5Command = new Command(ChangeValues5);
            ChangeValues6Command = new Command(ChangeValues6);
            ClearValuesCommand = new Command(ClearValues);
        }

        public ICommand ChangeHalfCircleCommand { protected set; get; }
        private void ChangeHalfCircle()
        {
            IsHalfCircle = !IsHalfCircle;
        }

        public ICommand ChangeValues1Command { protected set; get; }
        private void ChangeValues1()
        {
            PieColors = null;
            Values = new List<double>() { 10, 1000, 500, 50 };
            IsHalfCircle = true;
        }

        public ICommand ChangeValues2Command { protected set; get; }
        private void ChangeValues2()
        {
            PieColors = null;
            Values = new List<double>() { 300, 200 };
            IsHalfCircle = true;
        }

        public ICommand ChangeValues3Command { protected set; get; }
        private void ChangeValues3()
        {
            PieColors = null;
            Values = new List<double>() { 230, 8, 25, 300, 2, 90 };
            IsHalfCircle = false;
        }

        public ICommand ChangeValues4Command { protected set; get; }
        private void ChangeValues4()
        {
            PieColors = null;
            PieColor = Color.FromArgb("#84CEB2");
            Values = new List<double>() { 30, 100, 50, 2 };
        }

        public ICommand ChangeValues5Command { protected set; get; }
        private void ChangeValues5()
        {
            PieColors = null;
            PieColor = Color.FromArgb("#B7ADE8");
            Values = new List<double>() { 1000, 50 ,  2, 104};
        }

        public ICommand ChangeValues6Command { protected set; get; }
        private void ChangeValues6()
        {
            PieColors = new List<Color>()
            {
                Colors.Red, Colors.Yellow, Colors.Green, Colors.Blue
            };
            Values = new List<double>() { 23, 80, 30, 23, 90, 30, 50, 60, 90 };
        }

        public ICommand ClearValuesCommand { protected set; get; }
        private void ClearValues()
        {
            Values = null;
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

