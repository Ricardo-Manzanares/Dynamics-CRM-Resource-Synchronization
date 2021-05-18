using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace DynamicsCRMResourceSynchronization.Controls
{
    /// <summary>
    /// Interaction logic for MainControl.
    /// </summary>
    public partial class LoadingControl : UserControl, INotifyPropertyChanged
    {
        private int _MaxSize;

        public int MaxSize
        {
            get
            {
                return _MaxSize;
            }
            set
            {
                _MaxSize = value;
                OnPropertyChanged("MaxSize");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareResourceWindowControl"/> class.
        /// </summary>
        public LoadingControl()
        {
            this.InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private void Loading_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MaxSize = int.Parse(this.ActualWidth.ToString());
        }
    }
}