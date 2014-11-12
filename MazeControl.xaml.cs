using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MazeRunner
{
	/// <summary>
	/// Interaction logic for MazeControl.xaml
	/// </summary>
	public partial class MazeControl : INotifyPropertyChangedHandler
	{
		private static readonly DependencyProperty TileSizeProperty = DependencyProperty.Register("TileSize", typeof(double), typeof(MazeControl), new PropertyMetadata(50d, OnTileSizeChanged));
		public double TileSize
		{
			get { return (double)GetValue(TileSizeProperty); }
			set { SetValue(TileSizeProperty, value); }
		}


		private static readonly DependencyProperty NumHorizontalTilesProperty = DependencyProperty.Register("NumHorizontalTiles", typeof(double), typeof(MazeControl), new PropertyMetadata(21d, OnNumHorizontalChanged));

		public double NumHorizontalTiles
		{
			get { return (double)GetValue(NumHorizontalTilesProperty); }
			set { SetValue(NumHorizontalTilesProperty, value); }
		}

		private static readonly DependencyProperty NumVerticalTilesProperty = DependencyProperty.Register("NumVerticalTiles", typeof(double), typeof(MazeControl), new PropertyMetadata(21d, OnNumVerticalChanged));
		public double NumVerticalTiles
		{
			get { return (double)GetValue(NumVerticalTilesProperty); }
			set { SetValue(NumVerticalTilesProperty, value); }
		}

		public double ControlWidth
		{
			get { return NumHorizontalTiles * TileSize; }
		}

		public double ControlHeight
		{
			get { return NumVerticalTiles * TileSize; }
		}




		public MazeControl()
		{
			InitializeComponent();
		}

		#region INotifyPropertyChangedHandler

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnChange<T>(Expression<Func<T>> propertyExpression)
		{
			((INotifyPropertyChangedHandler)this).OnChange(propertyExpression);
		}

		public virtual void OnChange(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion INotifyPropertyChangedHandler

		#region DependencyPropertyChanged handlers

		private static void OnNumHorizontalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var view = d as MazeControl;
			if (view == null) return;
			view.OnChange(() => view.ControlWidth);
		}

		private static void OnNumVerticalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var view = d as MazeControl;
			if (view == null) return;
			view.OnChange(() => view.ControlHeight);
		}

		private static void OnTileSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var view = d as MazeControl;
			if (view == null) return;
			view.OnChange(() => view.ControlHeight);
			view.OnChange(() => view.ControlWidth);
		}

		#endregion DependencyPropertyChanged handlers
	}
}
