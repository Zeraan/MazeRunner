using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MazeRunner
{
	//public enum TileType { Nothing, Blocked, Room, Corridor, Perimeter, Entrance, Arch, Door, Locked, Trapped, Secret, Portc, StairsDown, StairsUp, OutOfBounds }
	public class Tile
	{
		public const uint NOTHING		= 0x00000000;
		public const uint BLOCKED		= 0x00000001;
		public const uint ROOM			= 0x00000002;
		public const uint CORRIDOR		= 0x00000004;
		public const uint PERIMETER		= 0x00000010;
		public const uint ENTRANCE		= 0x00000020;
		public const uint ROOM_ID		= 0x0000FFC0;
		public const uint ARCH			= 0x00010000;
		public const uint DOOR			= 0x00020000;
		public const uint LOCKED		= 0x00040000;
		public const uint TRAPPED		= 0x00080000;
		public const uint SECRET		= 0x00100000;
		public const uint PORTC			= 0x00200000;
		public const uint STAIR_DN		= 0x00400000;
		public const uint STAIR_UP		= 0x00800000;
		public const uint LABEL			= 0xFF000000;

		public const uint OPENSPACE		= ROOM | CORRIDOR;
		public const uint DOORSPACE		= ARCH | DOOR | LOCKED | TRAPPED | SECRET | PORTC;
		public const uint ESPACE		= ENTRANCE | DOORSPACE | 0xFF000000;
		public const uint STAIRS		= STAIR_DN | STAIR_UP;

		public const uint BLOCK_ROOM	= BLOCKED | ROOM;
		public const uint BLOCK_CORR	= BLOCKED | PERIMETER | CORRIDOR;
		public const uint BLOCK_DOOR	= BLOCKED | DOORSPACE;

		public uint Flags;

		//public TileType TileType { get; set; }
		public bool Opened { get; set; } //For use with doors
		public bool Unlocked { get; set; }
		public int Row { get; set; }
		public int Column { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		
		//TODO: Add items
		//public List<Item> ItemsOnFloor { get; private set; }

		//TODO: Add reference to entities (enemies or player)
		//Entity Entity { get; set; }
	}

	public class Room
	{
		public uint ID;
		public int Row;
		public int Column;
		public int North;
		public int South;
		public int West;
		public int East;
		public int Height;
		public int Width;
		public int Area;
	}

	public class Level
	{
		public Tile[,] Tiles;
		public Dictionary<uint, Room> Rooms;
		public int Width;
		public int Height;
		public int N_I;
		public int N_J;
		public int MaxRow { get; set; }
		public int MaxColumn { get; set; }
		public int MinRoomSize { get; set; }
		public int MaxRoomSize { get; set; }
		public int RoomBase { get { return (MinRoomSize + 1) / 2; } }
		public int RoomRadix { get { return ((MaxRoomSize - MinRoomSize) / 2) + 1; } }
		public uint N_Rooms { get; set; }
		public const int MAX_ROOMS = 9;

		public int AllocRooms
		{
			get
			{
				var area = Width * Height;
				var roomArea = MAX_ROOMS * MAX_ROOMS;
				return area / roomArea;
			}
		}

		public string RoomLayout { get; set; }
		public int RoomCount { get; set; }
		public Point StartingPoint { get; set; }
		// TODO: Add list of entities and items on floor to level 


		public Level(int levelNumber)
		{
			Random r = new Random();
			Rooms = new Dictionary<uint, Room>();
			Width = r.Next(20,101);
			Height = r.Next(20, 101);
			MinRoomSize = 3;
			MaxRoomSize = 9;
			if (Width % 2 == 0)
			{
				Width++; //Must be odd number
			}
			if (Height % 2 == 0)
			{
				Height++; //Must be odd number
			}
			N_I = Width / 2;
			N_J = Height / 2;
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
					Tiles[i,j].Flags = Tile.NOTHING;
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
			var numOfRooms = AllocRooms;

			for (int i = 0; i < numOfRooms; i++)
			{
				PlaceRoom();
			}
		}

		private void PackRooms()
		{
			Random random = new Random();
			for (int i = 0; i < N_I;  ++i)
			{
				int rowNumber = i * 2 + 1;
				for (int j = 0; j < N_J; ++j)
				{
					int colNumber = (j * 2) + 1;
					if ((Tiles[rowNumber, colNumber].Flags & Tile.ROOM) == Tile.ROOM)
					{
						continue;
					}
					if ((i == 0 || j == 0) && random.Next(2) > 0)
					{
						continue;
					}
					PlaceRoom(i, j);
				}
			}
		}

		private void PlaceRoom(int i = -1, int j = -1, int width = 0, int height = 0)
		{
			if (RoomCount == 999)
			{
				return;
			}

			SetRoom(ref i, ref j, ref width, ref height);

			int row1 = i * 2 + 1;
			int column1	= j * 2 + 1;
			int row2 = (i + height) * 2 - 1;
			int column2	= (j + width) * 2 - 1;

			if (row1 < 1 || row2 > MaxRow)
			{
				return;
			}
			if (column1 < 1 || column2 > MaxColumn)
			{
				return;
			}

			if (!SoundRoom(row1, column1, row2, column2))
			{
				//Room is not in a valid location, exit
				return;
			}

			uint roomID = N_Rooms + 1;
			N_Rooms = roomID;
			LastRoomID = roomID;

			//place room
			for (int r = row1; r <= row2; r++)
			{
				for (int c = column1; c <= column2; c++)
				{
					if ((Tiles[r,c].Flags & Tile.ENTRANCE) == Tile.ENTRANCE)
					{
						Tiles[r,c].Flags &= ~Tile.ESPACE;
					}
					else if ((Tiles[r,c].Flags & Tile.PERIMETER) == Tile.PERIMETER)
					{
						Tiles[r,c].Flags &= ~Tile.PERIMETER;
					}
					Tiles[r,c].Flags |= Tile.ROOM | roomID << 6;
				}
			}
			Room room = new Room 
			{
				ID = roomID,
				Row = row1,
				Column = column1,
				North = row1,
				South = row2,
				West = column1,
				East = column2,
				Height = height,
				Width = width,
				Area = height * width
			};
			Rooms[roomID] = room;

			for (int r = row1 - 1; r <= row2 + 1; r++)
			{
				if ((Tiles[r,column1 - 1].Flags & Tile.ROOM) != Tile.ROOM && (Tiles[r,column1 - 1].Flags & Tile.ENTRANCE) != Tile.ENTRANCE)
				{
					Tiles[r,column1 - 1].Flags |= Tile.PERIMETER;
				}
				if ((Tiles[r, column2 + 1].Flags & Tile.ROOM) != Tile.ROOM && (Tiles[r, column2 + 1].Flags & Tile.ENTRANCE) != Tile.ENTRANCE)
				{
					Tiles[r, column2 + 1].Flags |= Tile.PERIMETER;
				}
			}
			for (int c = column1 - 1; c <= column2 + 1; c++)
			{
				if ((Tiles[row1 - 1, c].Flags & Tile.ROOM) != Tile.ROOM && (Tiles[row1 - 1, c].Flags & Tile.ENTRANCE) != Tile.ENTRANCE)
				{
					Tiles[row1 - 1, c].Flags |= Tile.PERIMETER;
				}
				if ((Tiles[row2 + 1, c].Flags & Tile.ROOM) != Tile.ROOM && (Tiles[row2 + 1, c].Flags & Tile.ENTRANCE) != Tile.ENTRANCE)
				{
					Tiles[row2 + 1, c].Flags |= Tile.PERIMETER;
				}
			}
		}

		private void SetRoom(ref int i, ref int j, ref int width, ref int height)
		{
			var random = new Random();
			var roomBase = RoomBase;
			var radix = RoomRadix;

			if (height == 0)
			{
				if (i != -1)
				{
					var a = N_I - roomBase - i;
					if (a < 0)
					{
						a = 0;
					}
					var r = a < radix ? a : radix;
					height = random.Next(r) + roomBase;
				}
				else
				{
					height = random.Next(radix) + roomBase;
				}
			}
			if (width == 0)
			{
				if (j != -1)
				{
					var a = N_J - roomBase - j;
					if (a < 0)
					{
						a = 0;
					}
					var r = a < radix ? a : radix;
					width = random.Next(r) + roomBase;
				}
				else
				{
					width = random.Next(radix) + roomBase;
				}
			}
			if (i == -1)
			{
				i = random.Next(N_I - height);
			}
			if (j == -1)
			{
				j = random.Next(N_J - width);
			}
		}

		private bool SoundRoom(int row1, int column1, int row2, int column2)
		{
			for (int r = row1; r <= row2; r++)
			{
				for (int c = column1; c <= column2; c++)
				{
					if ((Tiles[c,r].Flags & Tile.BLOCKED) == Tile.BLOCKED)
					{
						return false;
					}
					else if ((Tiles[c,r].Flags & Tile.ROOM) == Tile.ROOM)
					{
						return false;
					}
				}
			}
			//No blocked or overlapping rooms, so can place room here
			return true;
		}

		//Places openings for doors and corridors
		private void OpenRooms()
		{
			foreach (var room in Rooms)
			{
				OpenRoom(room.Value);
			}
		}

		private void OpenRoom(Room room)
		{

		}

		private void AllocOpens(Room room)
		{
			throw new NotImplementedException();
		}

		//List available sills (doorways?)
		private void DoorSills(Room room)
		{
			throw new NotImplementedException();
		}

		private void CheckSill()
		{
			throw new NotImplementedException();
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

		public uint LastRoomID { get; set; }
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
