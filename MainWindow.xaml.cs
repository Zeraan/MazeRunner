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
		public MainWindow()
		{
			InitializeComponent();
			Game game = new Game();
			DataContext = game;
			MazeCanvas.Level = game.CurrentLevel;
			horizontalScrollbar.Maximum = MazeCanvas.Level.Width - 17;
			verticalScrollbar.Maximum = MazeCanvas.Level.Height - 17;
		}

		private void horizontalScrollbar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
		{
			MazeCanvas.Left = (int)e.NewValue;
			MazeCanvas.InvalidateVisual();
		}

		private void verticalScrollbar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
		{
			MazeCanvas.Top = (int)e.NewValue;
			MazeCanvas.InvalidateVisual();
		}

		private void _mazeRunnerWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			horizontalScrollbar.Maximum = MazeCanvas.Level.Width - MazeCanvas.ActualWidth / 32 + 1;
			verticalScrollbar.Maximum = MazeCanvas.Level.Height - MazeCanvas.ActualHeight / 32 + 1;
		}
	}
}
