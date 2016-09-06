using System;
using System.Collections.Generic;
using System.Linq;

namespace ConwayGameOfLife
{
	public class Conway
	{
		private List<int> birthAdjacent;
		private List<int> surviveAdjacent;
		private bool[][] state;

		public string variation
		{
			get
			{
				var birthAdjacentString = birthAdjacent.Aggregate<int, string>(String.Empty, (a, b) => String.Format("{0}{1}", a, b));
				var surviveAdjacentString = surviveAdjacent.Aggregate<int, string>(String.Empty, (a, b) => String.Format("{0}{1}", a, b));
				return "B"+birthAdjacentString+"S"+surviveAdjacentString;
			}
			private set
			{
				var parts = value.Split(new Char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
				var birthAdjacentString = parts[0].Substring(1);
				var surviveAdjacentString = parts[1].Substring(1);

				birthAdjacent = new List<int>();
				foreach (var a in birthAdjacentString)
				{
					birthAdjacent.Add(Int16.Parse(a.ToString()));
				}

				surviveAdjacent = new List<int>();
				foreach (var a in surviveAdjacentString)
				{
					surviveAdjacent.Add(Int16.Parse(a.ToString()));
				}
			}
		}

		public string[] stringState
		{
			get
			{
				string[] stateHolder = new string[state.Length];
				for (int i = 0; i < state.Length; i++)
				{
					string rowHolder = String.Empty;
					foreach (var entry in state[i])
					{
						rowHolder += (entry) ? "#" : " ";
					}
					stateHolder[i] = rowHolder;
				}
				return stateHolder;
			}
			private set
			{
				state = new bool[value.Length][];
				for (int i = 0; i < value.Length; i++)
				{
					state[i] = new bool[value[i].Length];
					for (int j = 0; j < value[i].Length; j++)
					{
						if (value[i][j] == ' ') state[i][j] = false;
						else state[i][j] = true;
					}
				}
			}
		}

		public int living
		{
			get
			{
				int count = 0;
				for (int i = 0; i < state.Length; i++)
				{
					for (int j = 0; j < state[i].Length; j++)
					{
						if (state[i][j]) count++;
					}
				}
				return count;
			}
		}

		public int dead
		{
			get
			{
				int count = 0;
				for (int i = 0; i < state.Length; i++)
				{
					for (int j = 0; j < state[i].Length; j++)
					{
						if (!state[i][j]) count++;
					}
				}
				return count;
			}
		}

		public Conway() : this("B3/S23") { }

		public Conway(string variation) : this(variation, new string[] {"   ", " # ", "   "}) { }

		public Conway(string variation, string[] initialState)
		{
			this.variation = variation;
			this.stringState = initialState;
		}

		public void nextIteration()
		{
			var shadow = deepClone();

			for (int i = 0; i < state.Length; i++)
			{
				for (int j = 0; j < state[i].Length; j++)
				{
					var whatIsNext = whatHappensToThisCell(new KeyValuePair<int, int>(i, j));
					switch (whatIsNext)
					{
						case evolution.BIRTH:
							shadow[i][j] = true;
							break;

						case evolution.DEATH:
							shadow[i][j] = false;
							break;
					}
				}
			}

			for (int i = 0; i < shadow.Length; i++)
			{
				for (int j = 0; j < shadow[i].Length; j++)
				{
					state[i][j] = shadow[i][j];
				}
			}
		}

		public evolution whatHappensToThisCell(KeyValuePair<int, int> current)
		{
			bool alive = isCellAlive(current);
			int neighbours = getNeighbourCount(current);

			if (alive)
			{
				if (canCellLive(neighbours)) return evolution.SURVIVE;
				else return evolution.DEATH;
			}
			else 
			{
				if (canCellBeBorn(neighbours)) return evolution.BIRTH;
				else return evolution.INERT;
			}
		}

		public bool canCellBeBorn(int neighbourCount)
		{
			if (birthAdjacent.Contains(neighbourCount)) return true;
			else return false;
		}

		public bool canCellLive(int neighbourCount)
		{
			if (surviveAdjacent.Contains(neighbourCount)) return true;
			else return false;
		}

		public int getNeighbourCount(KeyValuePair<int, int> current) {
			int count = 0;
			if (isCellAlive(getLeft(current))) count++;
			if (isCellAlive(getRight(current))) count++;
			if (isCellAlive(getTop(current))) count++;
			if (isCellAlive(getBottom(current))) count++;
			if (isCellAlive(getBottomLeft(current))) count++;
			if (isCellAlive(getBottomRight(current))) count++;
			if (isCellAlive(getTopLeft(current))) count++;
			if (isCellAlive(getTopRight(current))) count++;
			return count;
		}

		public bool isCellAlive(KeyValuePair<int, int> current)
		{
			return state[current.Key][current.Value];
		}

		public KeyValuePair<int, int> getLeft(KeyValuePair<int, int> current)
		{
			if (current.Value > 0) return new KeyValuePair<int, int>(current.Key, current.Value - 1);
			else return new KeyValuePair<int, int>(current.Key, state.First().Length - 1);
		}

		public KeyValuePair<int, int> getRight(KeyValuePair<int, int> current)
		{
			if (current.Value < state.First().Length - 1) return new KeyValuePair<int, int>(current.Key, current.Value + 1);
			else return new KeyValuePair<int, int>(current.Key, 0);
		}

		public KeyValuePair<int, int> getTop(KeyValuePair<int, int> current)
		{
			if (current.Key > 0) return new KeyValuePair<int, int>(current.Key - 1, current.Value);
			else return new KeyValuePair<int, int>(state.Length - 1, current.Value);
		}

		public KeyValuePair<int, int> getBottom(KeyValuePair<int, int> current)
		{
			if (current.Key < state.Length - 1) return new KeyValuePair<int, int>(current.Key + 1, current.Value);
			else return new KeyValuePair<int, int>(0, current.Value);
		}

		public KeyValuePair<int, int> getBottomLeft(KeyValuePair<int, int> current)
		{
			return getBottom(getLeft(current));
		}

		public KeyValuePair<int, int> getBottomRight(KeyValuePair<int, int> current)
		{
			return getBottom(getRight(current));
		}

		public KeyValuePair<int, int> getTopLeft(KeyValuePair<int, int> current)
		{
			return getTop(getLeft(current));
		}

		public KeyValuePair<int, int> getTopRight(KeyValuePair<int, int> current)
		{
			return getTop(getRight(current));
		}

		public enum evolution { DEATH, BIRTH, SURVIVE, INERT };

		public bool[][] deepClone()
		{
			bool[][] shadow = new bool[state.Length][];
			for (int i = 0; i < state.Length; i++)
			{
				shadow[i] = new bool[state[i].Length];
				for (int j = 0; j < state[i].Length; j++)
				{
					shadow[i][j] = state[i][j];
				}
			}
			return shadow;
		}

		public override string ToString() 
		{
			string format = "";
			foreach (var row in stringState)
			{
				format += row + "\n";
			}
			return format;
		}
	}
}