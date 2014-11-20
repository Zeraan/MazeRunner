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


		public LevelManager LevelManager { get; set; }

		public Person MainPlayer { get; set; }

		public Action CenterMap;

		public Level CurrentLevel
		{
			get { return _currentLevel; }
			set
			{
				_currentLevel = value;
				MainPlayer.Column = CurrentLevel.StartingPoint.C;
				MainPlayer.Row = CurrentLevel.StartingPoint.R;
			}
		}

		public Game()
		{
			MainPlayer = new Person();
		}

		public void GenerateNextLevel()
		{
			if (LevelManager == null)
			{
				LevelManager = new LevelManager();
				CurrentLevel = LevelManager.Levels[0];
			}
			else
			{
				LevelManager.GenerateNextLevel();
			}
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
