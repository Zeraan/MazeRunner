using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MazeRunner
{
	public class Game : INotifyPropertyChangedHandler
	{
		#region Data Members

		private Level _currentLevel;
		private List<Tile> _minimapTiles, _currentGameTiles;

		#endregion Data Members

		public int NumMapHorizontalTilesVisible { get { return 11; } }
		public int NumMapVerticalTilesVisible { get { return 11; } }
		public int NumMiniMapHorizontalTilesVisible { get { return 21; } }
		public int NumMiniMapVerticalTilesVisible { get { return 21; } }

		public List<Level> Levels { get; set; }

		public Point PlayerPosition { get; set; }

		public Level CurrentLevel
		{
			get { return _currentLevel; }
			set
			{
				_currentLevel = value;
				PlayerPosition = CurrentLevel.StartingPoint;
				UpdateGameTiles();
				UpdateMiniMapTiles();
				OnChange(() => CurrentLevel);
			}
		}

		public List<Tile> VisibleTiles
		{
			get { return _currentGameTiles; }
			set
			{
				_currentGameTiles = value;
				OnChange(() => VisibleTiles);
			}
		}


		public List<Tile> MiniMapTiles
		{
			get { return _minimapTiles; }
			set
			{
				_minimapTiles = value;
				OnChange(() => MiniMapTiles);
			}
		}

		public Game()
		{
			InitLevel();
		}

		private void InitLevel()
		{
			if (Levels == null)
			{
				Levels = new List<Level>();
				Level firstLevel = new Level(1);
				Levels.Add(firstLevel);
				CurrentLevel = firstLevel;
			}
		}

		public void TraverseLevel(int level)
		{
			// Only accept valid levels
			if (level <= 0 || level > Levels.Count + 1) return;

			if (Levels.Count == Levels.Count + 1)
			{
				Level newLevel = new Level(Levels.Count + 1);
				Levels.Add(newLevel);
			}
			else
			{
				CurrentLevel = Levels[level];
			}
		}

		public void UpdateGameTiles()
		{
			Point startPoint = CalcFirstPoint(NumMapHorizontalTilesVisible, NumMapVerticalTilesVisible);
			Point endPoint = CalcLastPoint(NumMapHorizontalTilesVisible, NumMapVerticalTilesVisible);
			VisibleTiles = new List<Tile>();

			for (int x = (int)startPoint.X; x <= endPoint.X; x++)
			{
				for (int y = (int) startPoint.Y; y <= endPoint.Y; y++)
				{
					if (x < 0 || x >= CurrentLevel.Height || y < 0 || y >= CurrentLevel.Width)
						VisibleTiles.Add(new Tile() { Flags = Tile.BLOCKED});
					else VisibleTiles.Add(CurrentLevel.Tiles[x, y]);
				}
			}
		}

		public void UpdateMiniMapTiles()
		{
			Point startPoint = CalcFirstPoint(NumMiniMapHorizontalTilesVisible, NumMiniMapVerticalTilesVisible);
			Point endPoint = CalcLastPoint(NumMiniMapHorizontalTilesVisible, NumMiniMapVerticalTilesVisible);
			VisibleTiles = new List<Tile>();

			for (int x = (int)startPoint.X; x <= endPoint.X; x++)
			{
				for (int y = (int)startPoint.Y; y <= endPoint.Y; y++)
				{
					if (x < 0 || x >= CurrentLevel.Height || y < 0 || y >= CurrentLevel.Width)
						VisibleTiles.Add(new Tile() { Flags = Tile.BLOCKED });
					else VisibleTiles.Add(CurrentLevel.Tiles[x, y]);
				}
			}
		}

		public Point CalcFirstPoint(int width, int height)
		{
			return new Point(PlayerPosition.X - (width / 2), PlayerPosition.Y - (height / 2));
		}

		public Point CalcLastPoint(int width, int height)
		{
			return new Point(PlayerPosition.X + (width / 2), PlayerPosition.Y + (height / 2));
		}

		#region INotifyPropertyChangedHandler

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnChange<T>(Expression<Func<T>> propertyExpression)
		{
			((INotifyPropertyChangedHandler)this).OnChange(propertyExpression);
		}

		public virtual void OnChange(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion INotifyPropertyChangedHandler
	}

	public interface INotifyPropertyChangedHandler : INotifyPropertyChanged
	{
		void OnChange(string propertyName);
	}

	public static class INotifyPropertyChangedExtensions
	{
		/// <summary>
		///		OnChange handles the notification for Properties allowing for notification to the UI on said properties
		///		a presenter can use this as a means of communicating to the UI that something in the
		///		presenter has changed with the need of creating a new event.
		/// </summary>
		/// <typeparam name="T">
		///		The type of the property accessed in the <paramref name="propertyExpression"/>.
		/// </typeparam>
		/// <param name="handler">
		///		The object that implements <see cref="INotifyPropertyChangedHandler"/>.
		/// </param>
		/// <param name="propertyExpression">
		///		A lambda expression that access the property you wish to have the UI update.
		///	</param>
		/// <example>
		///		<code>
		///			OnChange(() => ExampleProperty);
		///		</code>
		/// </example>
		public static void OnChange<T>(
			this INotifyPropertyChangedHandler handler,
			Expression<Func<T>> propertyExpression)
		{
			var value = default(string);

			if (propertyExpression != null)
			{
				System.Diagnostics.Debug.Assert(
					propertyExpression.Body.NodeType == ExpressionType.MemberAccess,
					"The expression should access a property or field");

				value = ((MemberExpression)propertyExpression.Body).Member.Name;
			}

			handler.OnChange(value);
		}

	}
}
