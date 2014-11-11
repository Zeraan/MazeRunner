using System;
using System.Windows;
using System.Windows.Media;

namespace MazeRunner
{
	/// <summary>
	/// I looked at custom progress bar options, and though this all is pretty basic, 
	/// I figured I'd include a link to the stackoverflow comment
	/// http://stackoverflow.com/questions/11967898/progress-bar-style-in-wpf-is-old-fashioned-increments-in-bars-how-to-implement
	/// Interaction logic for CustomProgressBar.xaml
	/// </summary>
	public partial class CustomBar
	{
		public CustomBar()
		{
			InitializeComponent();
		}

		private static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(CustomBar), new PropertyMetadata(100d, OnMaximumChanged));
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }


		private static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(CustomBar), new PropertyMetadata(0d, OnMinimumChanged));
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

		private static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(CustomBar), new PropertyMetadata(0d, OnValueChanged));
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

		private static readonly DependencyProperty CustomBarWidthProperty = DependencyProperty.Register("CustomBarWidth", typeof(double), typeof(CustomBar), null);
        private double CustomBarWidth
        {
			get { return (double)GetValue(CustomBarWidthProperty); }
			set { SetValue(CustomBarWidthProperty, value); }
        }

		public Color BackgroundColor
		{
			get { return (Color)GetValue(BackgroundColorProperty); }
			set { SetValue(BackgroundColorProperty, value); }
		}


		public static DependencyProperty BackgroundColorProperty = DependencyProperty.Register("BackgroundColor",
																							typeof(Color),
																							typeof(CustomBar),
																							new PropertyMetadata(Colors.White));

		public Color BarColor
		{
			get { return (Color)GetValue(BarColorProperty); }
			set { SetValue(BarColorProperty, value); }
		}


		public static DependencyProperty BarColorProperty = DependencyProperty.Register("BarColor",
																							typeof(Color),
																							typeof(CustomBar),
																							new PropertyMetadata(Color.FromRgb(169, 169, 169)));

        static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
			(o as CustomBar).Update();
        }

        static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
			(o as CustomBar).Update();
        }

        static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
			(o as CustomBar).Update();
        }


        void Update()
        {
	        if (Maximum <= 0 || Maximum + Minimum <= 0)
	        {
		        CustomBarWidth = ActualWidth;
	        }
	        else
	        {
				CustomBarWidth = Math.Min((Value / (Maximum + Minimum) * ActualWidth) - _progressBar.BorderThickness.Right,
			        ActualWidth - _progressBar.BorderThickness.Right);
	        }
        }

		private void CustomBar_OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			Update();
		}
	}
}
