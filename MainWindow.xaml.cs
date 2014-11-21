using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Game _game;

		private int ColumnsVisible
		{
			get { return (int)(MazeCanvas.ActualWidth / 32); }
		}
		private int RowsVisible
		{
			get { return (int)(MazeCanvas.ActualHeight / 32); }
		}

		public MainWindow()
		{
			InitializeComponent();
			_game = new Game();
			_game.CenterMap = CenterMap;
			_game.GenerateNextLevel();
			DataContext = _game;
			MazeCanvas.Level = _game.CurrentLevel;
			horizontalScrollbar.Maximum = MazeCanvas.Level.Width - ColumnsVisible;
			verticalScrollbar.Maximum = MazeCanvas.Level.Height - RowsVisible;
			horizontalScrollbar.SetThumbLength(10);
			verticalScrollbar.SetThumbLength(10);
			CenterMap();
		}

		private void _mazeRunnerWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			horizontalScrollbar.Maximum = MazeCanvas.Level.Width - ColumnsVisible;
			verticalScrollbar.Maximum = MazeCanvas.Level.Height - RowsVisible;
		}

		private void horizontalScrollbar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
		{
			MazeCanvas.Left = (int)e.NewValue;
			MazeCanvas.InvalidateVisual();
		}

		private void verticalScrollbar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
		{
			MazeCanvas.Top = (int)(e.NewValue);
			MazeCanvas.InvalidateVisual();
		}

		private void CenterMap() //centers to main player
		{
			int col = _game.MainPlayer.Column - (ColumnsVisible / 2);
			int row = _game.MainPlayer.Row - (RowsVisible / 2) - 5;
			if (col + ColumnsVisible > horizontalScrollbar.Maximum)
			{
				col = (int)horizontalScrollbar.Maximum - ColumnsVisible;
			}
			if (row + RowsVisible > verticalScrollbar.Maximum)
			{
				row = (int)verticalScrollbar.Maximum - RowsVisible;
			}
			horizontalScrollbar.SetThumbCenter(col);
			verticalScrollbar.SetThumbCenter(row);
			MazeCanvas.Top = (int)(verticalScrollbar.Value);
			MazeCanvas.Left = (int)horizontalScrollbar.Value;
			/*horizontalScrollbar.Value = _game.MainPlayer.Column - (ColumnsVisible / 2);
			verticalScrollbar.Value = _game.MainPlayer.Row - (RowsVisible / 2);*/
			MazeCanvas.InvalidateVisual();
		}

		private void centerButton_Click(object sender, RoutedEventArgs e)
		{
			CenterMap();
		}

		 
	}

	public static class ScrollBarExtensions
	{
		public static void SetThumbCenter(this ScrollBar s, double thumbCenter)
		{
			double thumbLength = GetThumbLength(s);
			double trackLength = s.Maximum - s.Minimum;

			if (thumbCenter >= s.Maximum - thumbLength / 2)
			{
				s.Value = s.Maximum;
			}
			else if (thumbCenter <= s.Minimum + thumbLength / 2)
			{
				s.Value = s.Minimum;
			}
			else if (thumbLength >= trackLength)
			{
				s.Value = s.Minimum;
			}
			else
			{
				s.Value = s.Minimum + trackLength *
					((thumbCenter - s.Minimum - thumbLength / 2)
					/ (trackLength - thumbLength));
			}
		}

		public static double GetThumbLength(this ScrollBar s)
		{
			double trackLength = s.Maximum - s.Minimum;
			return trackLength * s.ViewportSize /
				(trackLength + s.ViewportSize);
		}

		public static void SetThumbLength(this ScrollBar s, double thumbLength)
		{
			double trackLength = s.Maximum - s.Minimum;

			if (thumbLength < 0)
			{
				s.ViewportSize = 0;
			}
			else if (thumbLength < trackLength)
			{
				s.ViewportSize = trackLength * thumbLength / (trackLength - thumbLength);
			}
			else
			{
				s.ViewportSize = double.MaxValue;
			}
		} 
	}
}
