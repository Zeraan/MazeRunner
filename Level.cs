using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MazeRunner
{
	public enum DoorType { ARCH, DOOR, LOCKED, TRAPPED, SECRET, PORTC }
	//public enum TileType { Nothing, Blocked, Room, Corridor, Perimeter, Entrance, Arch, Door, Locked, Trapped, Secret, Portc, StairsDown, StairsUp, OutOfBounds }
	public class Tile
	{
		public const uint NOTHING		= 0x00000000;
		public const uint BLOCKED		= 0x00000001;
		public const uint ROOM			= 0x00000002;
		public const uint CORRIDOR		= 0x00000004;
		public const uint PERIMETER		= 0x00000010;
		public const uint ENTRANCE		= 0x00000020;
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
		public int Room_ID { get; set; }
		public char Character { get; set; }
		
		//TODO: Add items
		//public List<Item> ItemsOnFloor { get; private set; }

		//TODO: Add reference to entities (enemies or player)
		//Entity Entity { get; set; }
	}

	public static class Direction
	{
		public static Dictionary<string, int> DI = new Dictionary<string,int> {
			{ "north", -1 },
			{ "south", 1 },
			{ "west", 0},
			{ "east", 0}
		};

		public static Dictionary<string, int> DJ = new Dictionary<string, int> {
			{ "north", 0 },
			{ "south", 0 },
			{ "west", -1},
			{ "east", 1}
		};
	}

	public static class CorridorLayout
	{
		public const int LABYRINTH = 0;
		public const int BENT = 50;
		public const int STRAIGHT = 100;
	}

	public class Door
	{
		public int Row;
		public int Column;
		public DoorType Type;
		public string TypeName;
		public uint RoomID;
	}

	public class Room
	{
		public int ID;
		public int Row;
		public int Column;
		public int North;
		public int South;
		public int West;
		public int East;
		public int Height;
		public int Width;
		public int Area;

		public List<Door> Doors = new List<Door>();
	}

	public class Sill
	{
		public int Sill_R;
		public int Sill_C;
		public int CDir;
		public int RDir;
		public int Door_R;
		public int Door_C;
		public int Out_ID;
	}

	public static class ExtensionMethods
	{
		public static void Shuffle<T>(this IList<T> list)
		{
			Random rng = new Random();
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}

	public class Level
	{
		public Tile[,] Tiles;
		public Dictionary<int, Room> Rooms;
		public int Width;
		public int Height;
		public int N_I;
		public int N_J;
		public int MinRoomSize { get; set; }
		public int MaxRoomSize { get; set; }
		public int RoomBase { get { return (MinRoomSize + 1) / 2; } }
		public int RoomRadix { get { return ((MaxRoomSize - MinRoomSize) / 2) + 1; } }
		public int N_Rooms { get; set; }
		public int LastRoomID { get; set; }
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

		public int? CorridorLayout { get; set; }
		public string RoomLayout { get; set; }
		public int RoomCount { get; set; }
		public Point StartingPoint { get; set; }
		// TODO: Add list of entities and items on floor to level 


		public Level(int levelNumber)
		{
			Random r = new Random();
			Rooms = new Dictionary<int, Room>();
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

			if (row1 < 1 || row2 >= Height)
			{
				return;
			}
			if (column1 < 1 || column2 >= Width)
			{
				return;
			}

			if (!SoundRoom(row1, column1, row2, column2))
			{
				//Room is not in a valid location, exit
				return;
			}

			int roomID = N_Rooms + 1;
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
					Tiles[r,c].Flags |= Tile.ROOM;
					Tiles[r,c].Room_ID = roomID;
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
			var doorSills = DoorSills(room);
			if (doorSills.Count == 0)
			{
				return;
			}
			int n_opens = AllocOpens(room);
			Random random = new Random();
			Tile doorTile = new Tile() { Row = -1, Column = -1 };
			Sill aSill = null;
			for (int i = 0; i < n_opens; i++)
			{
				int door_R = 0;
				int door_C = 0;
				do
				{
					if (doorSills.Count == 0)
					{
						aSill = null;
						break;
					}
					aSill = doorSills[random.Next(doorSills.Count)];
					doorSills.Remove(aSill);
					door_R = aSill.Door_R;
					door_C = aSill.Door_C;
					doorTile = Tiles[door_R, door_C];
				} while ((doorTile.Flags & Tile.DOORSPACE) == Tile.DOORSPACE);
				if (aSill == null)
				{
					break;
				}
				int out_id = aSill.Out_ID;
				if (out_id > 0)
				{

				}
				/*my $out_id; if ($out_id = $sill->{'out_id'}) {
      my $connect = join(',',(sort($room->{'id'},$out_id)));
      redo if ($dungeon->{'connect'}{$connect}++);
    }*/

				int open_r = aSill.Sill_R;
				int open_c = aSill.Sill_C;
				int open_cDir = aSill.CDir;
				int open_rDir = aSill.RDir;

				for (int x = 0; x < 3; x++)
				{
					int r = open_r + (open_rDir * x);
					int c = open_c + (open_cDir * x);

					var tile = Tiles[r,c];
					tile.Flags &= ~Tile.PERIMETER;
					tile.Flags |= Tile.ENTRANCE;
				}

				var doorType = DoorType();
				var door = new Door
				{
					Column = door_C,
					Row = door_R,
					Type = doorType
				};
				switch (doorType)
				{
					case MazeRunner.DoorType.ARCH:
						Tiles[door_R,door_C].Flags |= Tile.ARCH;
						door.TypeName = "ArchWay";
						break;
					case MazeRunner.DoorType.DOOR:
						Tiles[door_R,door_C].Flags |= Tile.DOOR;
						door.TypeName = "Unlocked Door";
						break;
					case MazeRunner.DoorType.LOCKED:
						Tiles[door_R,door_C].Flags |= Tile.LOCKED;
						door.TypeName = "Locked Door";
						break;
					case MazeRunner.DoorType.TRAPPED:
						Tiles[door_R,door_C].Flags |= Tile.TRAPPED;
						door.TypeName = "Trapped Door";
						break;
					case MazeRunner.DoorType.SECRET:
						Tiles[door_R,door_C].Flags |= Tile.SECRET;
						door.TypeName = "Secret Door";
						break;
					case MazeRunner.DoorType.PORTC:
						Tiles[door_R,door_C].Flags |= Tile.PORTC;
						door.TypeName = "Portcullis";
						break;
				}
				//$door->{'out_id'} = $out_id if ($out_id);
				room.Doors.Add(door);
			}
		}

		private int AllocOpens(Room room)
		{
			var room_h = (room.South - room.North) / 2 + 1;
			var room_w = (room.East - room.West) / 2 + 1;
			var flumph = (int)Math.Sqrt(room_w * room_h);
			
			var random = new Random();
			return flumph + random.Next(flumph);
		}

		//List available sills (doorways?)
		private List<Sill> DoorSills(Room room)
		{
			var doorSills = new List<Sill>();
			if (room.North >= 3)
			{
				for (int c = room.West; c <= room.East; c += 2)
				{
					var sill = CheckSill(room, c, room.North, 0, -1);
					if (sill != null)
					{
						doorSills.Add(sill);
					}
				}
			}
			if (room.South <= Height - 3)
			{
				for (int c = room.West; c <= room.East; c += 2)
				{
					var sill = CheckSill(room, c, room.South, 0, 1);
					if (sill != null)
					{
						doorSills.Add(sill);
					}
				}
			}
			if (room.West >= 3)
			{
				for (int r = room.North; r <= room.South; r += 2)
				{
					var sill = CheckSill(room, room.West, r, -1, 0);
					if (sill != null)
					{
						doorSills.Add(sill);
					}
				}
			}
			if (room.East <= Width - 3)
			{
				for (int r = room.North; r <= room.South; r += 2)
				{
					var sill = CheckSill(room, room.East, r, 1, 0);
					if (sill != null)
					{
						doorSills.Add(sill);
					}
				}
			}
			doorSills.Shuffle();
			return doorSills;
		}

		private Sill CheckSill(Room room, int sill_c, int sill_r, int cDir, int rDir)
		{
			int door_r = sill_r + rDir;
			int door_c = sill_c + cDir;

			var tile = Tiles[door_r,door_c];
			if ((tile.Flags & Tile.PERIMETER) != Tile.PERIMETER)
			{
				return null;
			}
			if ((tile.Flags & Tile.BLOCK_DOOR) == Tile.BLOCK_DOOR)
			{
				return null;
			}

			int out_r = door_r + rDir;
			int out_c = door_c + cDir;

			var outTile = Tiles[out_r, out_c];
			if ((outTile.Flags & Tile.BLOCKED) == Tile.BLOCKED)
			{
				return null;
			}
			int out_id = 0;
			if ((outTile.Flags & Tile.ROOM) == Tile.ROOM)
			{
				out_id = outTile.Room_ID;
			}
			if (out_id == room.ID)
			{
				return null;
			}
			return new Sill
			{
				Sill_C = sill_c,
				Sill_R = sill_r,
				CDir = cDir,
				RDir = rDir,
				Door_C = door_c,
				Door_R = door_r,
				Out_ID = out_id
			};
		}

		private static DoorType DoorType()
		{
			Random r = new Random();

			var value = r.Next(110);
			if (value < 15)
			{
				return MazeRunner.DoorType.ARCH;
			}
			if (value < 60)
			{
				return MazeRunner.DoorType.DOOR;
			}
			if (value < 75)
			{
				return MazeRunner.DoorType.LOCKED;
			}
			if (value < 90)
			{
				return MazeRunner.DoorType.TRAPPED;
			}
			if (value < 100)
			{
				return MazeRunner.DoorType.SECRET;
			}
			return MazeRunner.DoorType.PORTC;
		}

		private void LabelRooms()
		{
			
		}

		private void Corridors()
		{
			for (int i = 1; i < N_I; i++)
			{
				int row = (i * 2) + 1;
				for (int j = 1; j < N_J; j++)
				{
					int column = (j * 2) + 1;
					if ((Tiles[row,column].Flags & Tile.CORRIDOR) == Tile.CORRIDOR)
					{
						continue;
					}
					Tunnel(i, j);
				}
			}
		}

		private void Tunnel(int i, int j, string lastDirection = "")
		{
			string[] directions = TunnelDirections(lastDirection);
			foreach (string dir in directions)
			{
				if (OpenTunnel(i, j, dir))
				{
					int nextI = i + Direction.DI[dir];
					int nextJ = j + Direction.DJ[dir];
					Tunnel(nextI, nextJ, dir);
				}
			}
		}

		private string[] TunnelDirections(string lastDirection)
		{
			var directionKeysCopy = new List<string>(Direction.DJ.Keys.ToList<string>());
			if (!string.IsNullOrEmpty(lastDirection) && CorridorLayout != null)
			{
				Random random = new Random();
				if (random.Next(MazeRunner.CorridorLayout.STRAIGHT) < CorridorLayout)
				{
					directionKeysCopy.Remove(lastDirection);
				}
			}
			return directionKeysCopy.ToArray();
		}

		private bool OpenTunnel(int i, int j, string dir)
		{
			int thisRow = (i * 2) + 1;
			int thisColumn = (j * 2) + 1;
			int nextRow = ((i + Direction.DI[dir]) * 2) + 1;
			int nextColumn = ((j + Direction.DJ[dir] * 2)) + 1;
			int midRow = (thisRow + nextRow) / 2;
			int midColumn = (thisColumn + nextColumn) / 2;

			if (SoundTunnel(midRow, midColumn, nextRow, nextColumn))
			{
				return DelveTunnel(thisRow, thisColumn, nextRow, nextColumn);
			}
			else
			{
				return false;
			}
		}

		private bool DelveTunnel(int thisRow, int thisColumn, int nextRow, int nextColumn)
		{
			int r1 = Math.Min(thisRow, nextRow);
			int r2 = Math.Max(thisRow, nextRow);
			int c1 = Math.Min(thisColumn, nextColumn);
			int c2 = Math.Max(thisColumn, nextColumn);

			for (int r = r1; r <= r2; r++)
			{
				for (int c = c1; c <= c2; c++)
				{
					Tiles[r, c].Flags &=~ Tile.ENTRANCE;
					Tiles[r, c].Flags |= Tile.CORRIDOR;
				}
			}
			return true;
		}

		private bool SoundTunnel(int midRow, int midColumn, int nextRow, int nextColumn)
		{
			if (nextRow < 0 || nextRow > Height)
			{
				return false;
			}
			if (nextColumn< 0 || nextColumn > Width)
			{
				return false;
			}

			int r1 = Math.Min(midRow, nextRow);
			int r2 = Math.Max(midRow, nextRow);
			int c1 = Math.Min(midColumn, nextColumn);
			int c2 = Math.Max(midColumn, nextColumn);

			for (int r = r1; r <= r2; r++)
			{
				for (int c = c1; c <= c2; c++)
				{
					if ((Tiles[r,c].Flags & Tile.BLOCK_CORR) == Tile.BLOCK_CORR)
					{
						return false;
					}
				}
			}
				return true;
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
