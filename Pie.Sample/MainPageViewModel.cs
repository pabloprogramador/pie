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

        public MainPageViewModel()
		{
            ChangeHalfCircleCommand = new Command(ChangeHalfCircle);
		}

        public ICommand ChangeHalfCircleCommand { protected set; get; }
        private void ChangeHalfCircle()
        {
            IsHalfCircle = !IsHalfCircle;   
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

