using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MazeRunner
{
	public enum TileType { Nothing, Blocked, Room, Corridor, Perimeter, Entrance, Arch, Door, Locked, Trapped, Secret, Portc, StairsDown, StairsUp, OutOfBounds }
	public class Tile
	{
		public TileType TileType { get; set; }
		public bool Opened { get; set; } //For use with doors
		public bool Unlocked { get; set; }
		
		//TODO: Add items
		//public List<Item> ItemsOnFloor { get; private set; }

		//TODO: Add reference to entities (enemies or player)
		//Entity Entity { get; set; }
	}
	public class Level
	{
		public Tile[,] Tiles;
		public int Width;
		public int Height;
		public Point StartingPoint { get; set; }
		// TODO: Add list of entities and items on floor to level 


		public Level(int levelNumber)
		{
			Random r = new Random();
			Width = r.Next(20,101);
			Height = r.Next(20, 101);
			GenerateRandomLevel(levelNumber);
			SetStartingPoint();
		}

		public void SetStartingPoint()
		{
			Random r = new Random();
			// TODO: when placing player, check to make sure location is a valid position without monsters close by
			int x = r.Next(0, Width - 1);
			int y = r.Next(0, Height - 1);

			StartingPoint = new Point(x,y);
		}

		private void GenerateRandomLevel(int levelNumber)
		{
			InitCells();
			PlaceRooms();
			OpenRooms();
			LabelRooms();
			Corridors();
			PlaceStairs();
			CleanDungeon();
		}

		private void InitCells()
		{
			Tiles = new Tile[Width, Height];
			for (int i = 0; i < Width; i++)
			{
				for (int j = 0; j < Height; j++)
				{
					Tiles[i,j] = new Tile();
					Tiles[i,j].TileType = TileType.Nothing;
				}
			}
		}

		private void PlaceRooms()
		{

		}

		private void OpenRooms()
		{

		}

		private void LabelRooms()
		{

		}

		private void Corridors()
		{

		}

		private void PlaceStairs()
		{

		}

		private void CleanDungeon()
		{

		}
	}

	public class LevelManager
	{
		public List<Level> Levels { get; private set; }

		public LevelManager()
		{
			Levels = new List<Level>();
			Level firstLevel = new Level(1);
			Levels.Add(firstLevel);
		}
	}
}
