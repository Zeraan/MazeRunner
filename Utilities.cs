using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeRunner
{
	public class Utilities
	{
		public MazeOptions GetMazeOptions()
		{
			Random random = new Random();

			return new MazeOptions {
				Seed = random.Next(1, 100).ToString(),
				RowCount		= 39,
				ColCount		= 39,
				MazeLayout	= "None",
				RoomSizeMin	= 3,
				RoomSizeMax	= 9,
				RoomLayout	= "Scattered",
				CorridorLayout = "Bent",
				RemoveDeadends = 50,		//percentage
				AddStairs		= 2,
				MapStyle		= "Standard",
				CellSize		= 18			//pixels
			};
		}
	}

	

	public class MazeOptions
	{
		public string Seed { get; set; }

		public int RowCount { get; set; }

		public int ColCount { get; set; }

		public string MazeLayout { get; set; }

		public int RoomSizeMin { get; set; }

		public int RoomSizeMax { get; set; }

		public string RoomLayout { get; set; }

		public string CorridorLayout { get; set; }

		public int RemoveDeadends { get; set; }

		public int AddStairs { get; set; }

		public string MapStyle { get; set; }

		public int CellSize { get; set; }
	}
}
