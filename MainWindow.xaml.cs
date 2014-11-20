using System;
using System.Collections.Generic;
using System.Linq;
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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Game _game;

		private int ColumnsVisible
		{
			get { return (int)(MazeCanvas.ActualWidth / 32 + 1); }
		}
		private int RowsVisible
		{
			get { return (int)(MazeCanvas.ActualWidth / 32 + 1); }
		}

		public MainWindow()
		{
			InitializeComponent();
			_game = new Game();
			_game.CenterMap = CenterMap;
			_game.GenerateNextLevel();
			DataContext = _game;
			MazeCanvas.Level = _game.CurrentLevel;
			horizontalScrollbar.Maximum = MazeCanvas.Level.Width;
			verticalScrollbar.Maximum = MazeCanvas.Level.Height;
			verticalScrollbar.Value = verticalScrollbar.Maximum;
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
			horizontalScrollbar.Value = _game.MainPlayer.Column - (ColumnsVisible / 2);
			verticalScrollbar.Value = _game.MainPlayer.Row - (RowsVisible / 2);
			MazeCanvas.InvalidateVisual();
		}

		private void centerButton_Click(object sender, RoutedEventArgs e)
		{
			CenterMap();
		}
	}
}
