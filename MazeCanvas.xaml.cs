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
	/// Interaction logic for MazeCanvas.xaml
	/// </summary>
	public partial class MazeCanvas : Canvas
	{
		BitmapImage img;
		CroppedBitmap Wall;
		CroppedBitmap Floor;
		CroppedBitmap OpenDoor;
		CroppedBitmap CloseDoor;
		CroppedBitmap LockedDoor;
		CroppedBitmap StairsUp;
		CroppedBitmap StairsDown;
		CroppedBitmap Perimeter;
		CroppedBitmap Blocked;

		public Level Level { get; set; }

		public int Top { get; set; }
		public int Left { get; set; }

		public MazeCanvas()
		{
			InitializeComponent();
			img = new BitmapImage(new Uri("tiles.png", UriKind.Relative));
			Wall = new CroppedBitmap(img, new Int32Rect(0,0,32,32));
			CloseDoor = new CroppedBitmap(img, new Int32Rect(32, 0, 32, 32));
			OpenDoor = new CroppedBitmap(img, new Int32Rect(64, 0, 32, 32));
			LockedDoor = new CroppedBitmap(img, new Int32Rect(96, 0, 32, 32));
			Floor = new CroppedBitmap(img, new Int32Rect(128, 0, 32, 32));
			StairsUp = new CroppedBitmap(img, new Int32Rect(160, 0, 32, 32));
			StairsDown = new CroppedBitmap(img, new Int32Rect(192, 0, 32, 32));
			Perimeter = new CroppedBitmap(img, new Int32Rect(128, 32, 32, 32));
			Blocked = new CroppedBitmap(img, new Int32Rect(160, 32, 32, 32));
		}

		protected override void OnRender(DrawingContext dc)
		{
			base.OnRender(dc);
			if (Level == null)
			{
				return;
			}

			/*
			for (int i = 0; i < 17; i++)
			{
				for (int j = 0; j < 17; j++)
				{
					if (i + Top >= Level.Height || j + Left >= Level.Width)
					{
						continue;
					}
					var tile = Level.Tiles[i + Top,j + Left];
					switch (tile.TileType)
					{
						case TileType.FLOOR:
							dc.DrawImage(Floor, new Rect(j * 32, i * 32, 32, 32));
							break;
						case TileType.WALL:
							dc.DrawImage(Wall, new Rect(j * 32, i * 32, 32, 32));
							break;
						case TileType.DOOR:
							dc.DrawImage(CloseDoor, new Rect(j * 32, i * 32, 32, 32));
							break;
						case TileType.OPEN_DOOR:
							dc.DrawImage(OpenDoor, new Rect(j * 32, i * 32, 32, 32));
							break;
						case TileType.LOCKED_DOOR:
							dc.DrawImage(LockedDoor, new Rect(j * 32, i * 32, 32, 32));
							break;
						case TileType.STAIRS_DOWN:
							dc.DrawImage(StairsDown, new Rect(j * 32, i * 32, 32, 32));
							break;
						case TileType.STAIRS_UP:
							dc.DrawImage(StairsUp, new Rect(j * 32, i * 32, 32, 32));
							break;
					}
				}
			}*/
			for (int i = 0; i < 34; i++)
			{
				for (int j = 0; j < 34; j++)
				{
					if (i + Top >= Level.Height || j + Left >= Level.Width)
					{
						continue;
					}
					var tile = Level.Tiles[i + Top,j + Left];
					switch (tile.TileType)
					{
						case TileType.FLOOR:
							dc.DrawImage(Floor, new Rect(j * 16, i * 16, 16, 16));
							break;
						case TileType.WALL:
							dc.DrawImage(Wall, new Rect(j * 16, i * 16, 16, 16));
							break;
						case TileType.DOOR:
							dc.DrawImage(CloseDoor, new Rect(j * 16, i * 16, 16, 16));
							break;
						case TileType.OPEN_DOOR:
							dc.DrawImage(OpenDoor, new Rect(j * 16, i * 16, 16, 16));
							break;
						case TileType.LOCKED_DOOR:
							dc.DrawImage(LockedDoor, new Rect(j * 16, i * 16, 16, 16));
							break;
						case TileType.STAIRS_DOWN:
							dc.DrawImage(StairsDown, new Rect(j * 16, i * 16, 16, 16));
							break;
						case TileType.STAIRS_UP:
							dc.DrawImage(StairsUp, new Rect(j * 16, i * 16, 16, 16));
							break;
						case TileType.PERIMETER:
							dc.DrawImage(Perimeter, new Rect(j * 16, i * 16, 16, 16));
							break;
						case TileType.BLOCKED:
							dc.DrawImage(Blocked, new Rect(j * 16, i * 16, 16, 16));
							break;
					}
				}
			}
		}
	}
}
