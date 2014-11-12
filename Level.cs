using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeRunner
{
	public enum TileType { Nothing, Blocked, Room, Corridor, Perimeter, Entrance, Arch, Door, Locked, Trapped, Secret, Portc, StairsDown, StairsUp }
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
		public string RoomLayout { get; set; }

		public Level(int levelNumber)
		{
			Random r = new Random();
			Width = r.Next(20,101);
			Height = r.Next(20, 101);
			GenerateRandomLevel(levelNumber);
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
			if (RoomLayout == "Packed")
			{
				PackRooms();
			}
			else
			{
				ScatterRooms();
			}
		}

		private void ScatterRooms()
		{
			throw new NotImplementedException();
		}

		private void PackRooms()
		{
			Random random = new Random();
			for (int i = 0; i < Tiles.GetLength(0);  ++i)
			{
				int rowNumber = i * 2 + 1;
				for (int j = 0; j < Tiles.GetLength(1); ++j)
				{
					int colNumber = (j * 2) + 1;
					if (Tiles[rowNumber, colNumber].TileType == TileType.Room)
					{
						continue;
					}
					if ((i == 0 || j == 0) && random.Next(2) > 0)
					{
						continue;
					}
				}
			}
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
