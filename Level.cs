using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MazeRunner
{
	public enum DoorType { ARCH, DOOR, LOCKED/*, TRAPPED, SECRET, PORTC*/ }
	public enum TileType { FLOOR, WALL, OPEN_DOOR, DOOR, LOCKED_DOOR, STAIRS_DOWN, STAIRS_UP, PERIMETER, BLOCKED }
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
		/*public const uint TRAPPED		= 0x00080000;
		public const uint SECRET		= 0x00100000;
		public const uint PORTC			= 0x00200000;*/
		public const uint STAIR_DN		= 0x00400000;
		public const uint STAIR_UP		= 0x00800000;
		public const uint LABEL			= 0xFF000000;

		public const uint OPENSPACE		= ROOM | CORRIDOR;
		public const uint DOORSPACE		= ARCH | DOOR | LOCKED; //| TRAPPED | SECRET | PORTC;
		public const uint ESPACE		= ENTRANCE | DOORSPACE | 0xFF000000;
		public const uint STAIRS		= STAIR_DN | STAIR_UP;

		public const uint BLOCK_ROOM	= BLOCKED | ROOM;
		public const uint BLOCK_CORR	= BLOCKED | PERIMETER | CORRIDOR;
		public const uint BLOCK_DOOR	= BLOCKED | DOORSPACE;

		public uint Flags;

		public TileType TileType { get; set; }
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
		public static List<string> Directions = new List<string> {
			"north",
			"south",
			"west",
			"east"
		};
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

	public static class StairData
	{
		public static Dictionary<string, Dictionary<string, List<int[]>>> StairEnd = new Dictionary<string,Dictionary<string,List<int[]>>>
		{
			{ "north", new Dictionary<string, List<int[]>>{
					 {"walled", new List<int[]>{new[]{1,-1},new[]{0,-1},new[]{-1,-1},new[]{-1,0},new[]{-1,1},new[]{0,1},new[]{1,1}}},
					 {"corridor", new List<int[]>{new[]{0,0},new[]{1,0},new[]{2,0}}},
					 {"stair", new List<int[]>{new[]{0,0}}},
					 {"next", new List<int[]>{new[]{1,0}}}
				}
			},
			{ "south", new Dictionary<string, List<int[]>>{
					 {"walled", new List<int[]>{new[]{-1,-1},new[]{0,-1},new[]{1,-1},new[]{1,0},new[]{1,1},new[]{0,1},new[]{-1,1}}},
					 {"corridor", new List<int[]>{new[]{0,0},new[]{-1,0},new[]{-2,0}}},
					 {"stair", new List<int[]>{new[]{0,0}}},
					 {"next", new List<int[]>{new[]{-1,0}}}
				}
			},
			{ "west", new Dictionary<string, List<int[]>>{
					 {"walled", new List<int[]>{new[]{-1,1},new[]{-1,0},new[]{-1,-1},new[]{0,-1},new[]{1,-1},new[]{1,0},new[]{1,1}}},
					 {"corridor", new List<int[]>{new[]{0,0},new[]{0,1},new[]{0,2}}},
					 {"stair", new List<int[]>{new[]{0,0}}},
					 {"next", new List<int[]>{new[]{0,1}}}
				}
			},
			{ "east", new Dictionary<string, List<int[]>>{
					 {"walled", new List<int[]>{new[]{-1,-1},new[]{-1,0},new[]{-1,1},new[]{0,1},new[]{1,1},new[]{1,0},new[]{1,-1}}},
					 {"corridor", new List<int[]>{new[]{0,0},new[]{0,-1},new[]{0,-2}}},
					 {"stair", new List<int[]>{new[]{0,0}}},
					 {"next", new List<int[]>{new[]{0,-1}}}
				}
			}
		};
	}

	public static class CloseData
	{
		public static Dictionary<string, Dictionary<string, List<int[]>>> CloseEnd = new Dictionary<string, Dictionary<string, List<int[]>>>
		{
			{ "north", new Dictionary<string, List<int[]>>{
					 {"walled", new List<int[]>{new[]{0,-1},new[]{1,-1},new[]{1,0},new[]{1,1},new[]{0,1}}},
					 {"close", new List<int[]>{new[]{0,0}}},
					 {"recurse", new List<int[]>{new[]{-1,0}}},
				}
			},
			{ "south", new Dictionary<string, List<int[]>>{
					 {"walled", new List<int[]>{new[]{0,-1},new[]{-1,-1},new[]{-1,0},new[]{-1,1},new[]{0,1}}},
					 {"close", new List<int[]>{new[]{0,0}}},
					 {"recurse", new List<int[]>{new[]{1,0}}},
				}
			},
			{ "west", new Dictionary<string, List<int[]>>{
					 {"walled", new List<int[]>{new[]{-1,0},new[]{-1,1},new[]{0,1},new[]{1,1},new[]{1,0}}},
					 {"close", new List<int[]>{new[]{0,0}}},
					 {"recurse", new List<int[]>{new[]{0,-1}}},
				}
			},
			{ "east", new Dictionary<string, List<int[]>>{
					 {"walled", new List<int[]>{new[]{-1,0},new[]{-1,-1},new[]{0,-1},new[]{1,-1},new[]{1,0}}},
					 {"close", new List<int[]>{new[]{0,0}}},
					 {"recurse", new List<int[]>{new[]{0,1}}},
				}
			}
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
	public class Stair
	{
		public int Row;
		public int Column;
		public int RowNext;
		public int ColumnNext;
		public bool IsDownward;
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
		public List<Stair> Stairs;
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

		public List<Door> Doors { get; set; }

		public int? CorridorLayout { get; set; }
		public string RoomLayout { get; set; }
		public int RoomCount { get; set; }
		public Point StartingPoint { get; set; }
		// TODO: Add list of entities and items on floor to level 

		private Random r;

		public Level(int levelNumber)
		{
			CorridorLayout = MazeRunner.CorridorLayout.BENT;
			r = new Random();
			Rooms = new Dictionary<int, Room>();
			Stairs = new List<Stair>();
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
			N_I = Height / 2;
			N_J = Width / 2;
			GenerateRandomLevel(levelNumber);
			SetStartingPoint();
		}

		public void SetStartingPoint()
		{
			// TODO: when placing player, check to make sure location is a valid position without monsters close by
			int x = r.Next(0, Width - 1);
			int y = r.Next(0, Height - 1);

			StartingPoint = new Point(x,y);
		}

		private void GenerateRandomLevel(int levelNumber)
		{
			//RoomLayout = "Packed";
			InitCells();
			PlaceRooms();
			OpenRooms();
			LabelRooms();
			Corridors();
			PlaceStairs();
			CleanDungeon();
			ConvertTiles(); //Just sets the enum value for simplicity
		}

		private void InitCells()
		{
			Tiles = new Tile[Height, Width];
			for (int i = 0; i < Height; i++)
			{
				for (int j = 0; j < Width; j++)
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
					if ((i == 0 || j == 0) && r.Next(2) > 0)
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
					height = this.r.Next(r) + roomBase;
				}
				else
				{
					height = r.Next(radix) + roomBase;
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
					width = this.r.Next(r) + roomBase;
				}
				else
				{
					width = r.Next(radix) + roomBase;
				}
			}
			if (i == -1)
			{
				i = r.Next(N_I - height);
			}
			if (j == -1)
			{
				j = r.Next(N_J - width);
			}
		}

		private bool SoundRoom(int row1, int column1, int row2, int column2)
		{
			for (int r = row1; r <= row2; r++)
			{
				for (int c = column1; c <= column2; c++)
				{
					if ((Tiles[r,c].Flags & Tile.BLOCKED) == Tile.BLOCKED)
					{
						return false;
					}
					else if ((Tiles[r,c].Flags & Tile.ROOM) == Tile.ROOM)
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
					aSill = doorSills[r.Next(doorSills.Count)];
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
					/*case MazeRunner.DoorType.TRAPPED:
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
						break;*/
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
			
			return flumph + r.Next(flumph);
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
			if ((tile.Flags & Tile.BLOCK_DOOR) > 0)
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

		private DoorType DoorType()
		{
			var value = r.Next(75);
			if (value < 15)
			{
				return MazeRunner.DoorType.ARCH;
			}
			if (value < 60)
			{
				return MazeRunner.DoorType.DOOR;
			}
			//if (value < 75)
			{
				return MazeRunner.DoorType.LOCKED;
			}
			/*if (value < 90)
			{
				return MazeRunner.DoorType.TRAPPED;
			}
			if (value < 100)
			{
				return MazeRunner.DoorType.SECRET;
			}
			return MazeRunner.DoorType.PORTC;*/
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
			var directionKeysCopy = new List<string>(Direction.Directions);
			directionKeysCopy.Shuffle();
			if (!string.IsNullOrEmpty(lastDirection) && CorridorLayout != null)
			{
				if (r.Next(MazeRunner.CorridorLayout.STRAIGHT) < CorridorLayout)
				{
					directionKeysCopy.Insert(0, lastDirection);
				}
			}
			return directionKeysCopy.ToArray();
		}

		private bool OpenTunnel(int i, int j, string dir)
		{
			int thisRow = (i * 2) + 1;
			int thisColumn = (j * 2) + 1;
			int nextRow = thisRow + (Direction.DI[dir] * 2);
			int nextColumn = thisColumn + (Direction.DJ[dir] * 2);
			int midRow = thisRow + Direction.DI[dir];
			int midColumn = thisColumn + Direction.DJ[dir];

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
					if ((Tiles[r, c].Flags & Tile.PERIMETER) == Tile.PERIMETER)
					{
						continue;
					}
					Tiles[r, c].Flags &= ~Tile.ENTRANCE;
					Tiles[r, c].Flags |= Tile.CORRIDOR;
				}
			}
			return true;
		}

		private bool SoundTunnel(int midRow, int midColumn, int nextRow, int nextColumn)
		{
			if (nextRow < 0 || nextRow >= Height)
			{
				return false;
			}
			if (nextColumn< 0 || nextColumn >= Width)
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
					if ((Tiles[r,c].Flags & Tile.BLOCK_CORR) > 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		private void PlaceStairs()
		{
			int n = 2; //For now, we have an entry and exit stairs, so totalling of 2 stairs
			var possibleStairs = StairEnds();
			if (possibleStairs.Count == 0) //No valid locations
			{
				return;
			}
			for (int i = 0; i < n; i++)
			{
				var stair = possibleStairs[this.r.Next(possibleStairs.Count)];
				possibleStairs.Remove(stair);
				
				int r = stair.Row;
				int c = stair.Column;
				int type = i < 2 ? i : this.r.Next(2);

				if (type == 0)
				{
					Tiles[r,c].Flags |= Tile.STAIR_DN;
					stair.IsDownward = true;
				}
				else
				{
					Tiles[r, c].Flags |= Tile.STAIR_UP;
					stair.IsDownward = false;
				}
				Stairs.Add(stair);
			}
		}

		private List<Stair> StairEnds()
		{
			List<Stair> possibleStairs = new List<Stair>();
			for (int i = 0; i < N_I; i++)
			{
				int r = i * 2 + 1;
				for (int j = 0; j < N_J; j++)
				{
					int c = j * 2 + 1;
					if (Tiles[r,c].Flags != Tile.CORRIDOR)
					{
						continue;
					}
					if ((Tiles[r,c].Flags & Tile.STAIRS) != 0)
					{
						continue;
					}
					foreach (var direction in Direction.Directions)
					{
						if (CheckTunnel(r, c, StairData.StairEnd[direction]))
						{
							possibleStairs.Add(new Stair
							{
													 Row = r,
													 Column = c,
													 RowNext = r + StairData.StairEnd[direction]["next"][0][0],
													 ColumnNext = c + StairData.StairEnd[direction]["next"][0][1]
							});
							break;
						}
					}
				}
			}
			return possibleStairs;
		}

		private void CleanDungeon()
		{
			if (RemoveDeadEndsPercent > 0)
			{
				RemoveDeadEnds();
			}
			FixDoors();
			EmptyBlocks();
		}

		private void EmptyBlocks()
		{
			for (int r = 0; r < Height; r++)
			{
				for (int c = 0; c < Width; c++)
				{
					if ((Tiles[r,c].Flags & Tile.BLOCKED) == Tile.BLOCKED)
					{
						Tiles[r,c].Flags = Tile.NOTHING;
					}
				}
			}
		}

		private bool CheckTunnel(int r, int c, Dictionary<string, List<int[]>> validSchema)
		{
			foreach (var value in validSchema)
			{
				if (value.Key == "corridor")
				{
					foreach (var position in value.Value)
					{
						if ((Tiles[r + position[0], c + position[1]].Flags & Tile.CORRIDOR) != Tile.CORRIDOR)
						{
							return false;
						}
					}
				}
				if (value.Key == "walled")
				{
					foreach (var position in value.Value)
					{
						if ((Tiles[r + position[0], c + position[1]].Flags & Tile.OPENSPACE) == Tile.OPENSPACE)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		private void FixDoors()
		{
			foreach (Room room in Rooms.Values)
			{
				List<Door> shiny = new List<Door>();
				foreach (Door door in room.Doors)
				{
					int doorR = door.Row;
					int doorC = door.Column;
					Tile doorTile = Tiles[doorR, doorC];
					if ((doorTile.Flags & Tile.OPENSPACE) != Tile.OPENSPACE)
					{
						continue;
					}

					shiny.Add(door);
				}

				if (shiny.Count > 0)
				{
					room.Doors = shiny.ToList<Door>();
					Doors.AddRange(shiny);
				}
				else
				{
					room.Doors.Clear();
				}
			}
		}

		private void RemoveDeadEnds()
		{
			CollapseTunnels();
		}

		private void CollapseTunnels()
		{
			bool all = RemoveDeadEndsPercent == 100;
			for (int i = 0; i < N_I; i++)
			{
				int r = (i * 2) + 1;
				for (int j = 0; j < N_J; j++)
				{
					int c = (j * 2) + 1;
					if ((Tiles[r,c].Flags & Tile.OPENSPACE) != Tile.OPENSPACE)
					{
						continue;
					}
					if ((Tiles[r,c].Flags & Tile.STAIRS) == Tile.STAIRS)
					{
						continue;
					}
					if (all || this.r.Next(100) < RemoveDeadEndsPercent)
					{
						Collapse(r, c);
					}
				}
			}
		}

		private void Collapse(int r, int c)
		{
			if ((Tiles[r,c].Flags & Tile.OPENSPACE) != Tile.OPENSPACE)
			{
				return;
			}

			var xc = CloseData.CloseEnd;
			foreach(string direction in xc.Keys)
			{
				if (CheckTunnel(r, c, xc[direction]))
				{
					foreach(var pair in xc[direction]["close"])
					{
						Tiles[r + pair[0], c + pair[1]].Flags = Tile.NOTHING;
					}

					int[] somePair;
					string theKey = "open";
					if (xc[direction].ContainsKey(theKey))
					{
						somePair = xc[direction][theKey][0];
						Tiles[r + somePair[0], c + somePair[1]].Flags |= Tile.CORRIDOR;
					}

					theKey = "recurse";
					if (xc[direction].ContainsKey(theKey))
					{
						somePair = xc[direction][theKey][0];
						Collapse(r + somePair[0], c + somePair[1]);
					}
				}
			}
		}

		private void ConvertTiles()
		{
			for (int i = 0; i < Height; i++)
			{
				for (int j = 0; j < Width; j++)
				{
					var tile = Tiles[i,j];
					if ((tile.Flags & Tile.OPENSPACE) > 0)
					{
						tile.TileType = TileType.FLOOR;
					}
					else if ((tile.Flags & Tile.ARCH) == Tile.ARCH)
					{
						tile.TileType = TileType.OPEN_DOOR;
					}
					else if ((tile.Flags & Tile.DOOR) == Tile.DOOR)
					{
						tile.TileType = TileType.DOOR;
					}
					else if ((tile.Flags & Tile.LOCKED) == Tile.LOCKED)
					{
						tile.TileType = TileType.LOCKED_DOOR;
					}
					else if ((tile.Flags & Tile.STAIR_UP) == Tile.STAIR_UP)
					{
						tile.TileType = TileType.STAIRS_UP;
					}
					else if ((tile.Flags & Tile.STAIR_DN) == Tile.STAIR_DN)
					{
						tile.TileType = TileType.STAIRS_DOWN;
					}
					else
					{
						tile.TileType = TileType.WALL; //Just fill it up with wall
					}
				}
			}
		}

		public int RemoveDeadEndsPercent { get; set; }
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
