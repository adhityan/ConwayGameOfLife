using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

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
			var shadow = DeepClone<bool[][]>(state);

			for (int i = 0; i < state.Length; i++)
			{
				for (int j = 0; j < state[i].Length; j++)
				{
					var whatIsNext = whatHappensToThisCell(new Tuple<int, int>(i, j));
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

		public evolution whatHappensToThisCell(Tuple<int, int> current)
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

		public int getNeighbourCount(Tuple<int, int> current) {
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

		public bool isCellAlive(Tuple<int, int> current)
		{
			return state[current.Item1][current.Item2];
		}

		public Tuple<int, int> getLeft(Tuple<int, int> current)
		{
			if (current.Item2 > 0) return new Tuple<int, int>(current.Item1, current.Item2 - 1);
			else return new Tuple<int, int>(current.Item1, state.First().Length - 1);
		}

		public Tuple<int, int> getRight(Tuple<int, int> current)
		{
			if (current.Item2 < state.First().Length - 1) return new Tuple<int, int>(current.Item1, current.Item2 + 1);
			else return new Tuple<int, int>(current.Item1, 0);
		}

		public Tuple<int, int> getTop(Tuple<int, int> current)
		{
			if (current.Item1 > 0) return new Tuple<int, int>(current.Item1 - 1, current.Item2);
			else return new Tuple<int, int>(state.Length - 1, current.Item2);
		}

		public Tuple<int, int> getBottom(Tuple<int, int> current)
		{
			if (current.Item1 < state.Length - 1) return new Tuple<int, int>(current.Item1 + 1, current.Item2);
			else return new Tuple<int, int>(0, current.Item2);
		}

		public Tuple<int, int> getBottomLeft(Tuple<int, int> current)
		{
			return getBottom(getLeft(current));
		}

		public Tuple<int, int> getBottomRight(Tuple<int, int> current)
		{
			return getBottom(getRight(current));
		}

		public Tuple<int, int> getTopLeft(Tuple<int, int> current)
		{
			return getTop(getLeft(current));
		}

		public Tuple<int, int> getTopRight(Tuple<int, int> current)
		{
			return getTop(getRight(current));
		}

		public enum evolution { DEATH, BIRTH, SURVIVE, INERT };

		public static T DeepClone<T>(T obj)
		{
			using (var ms = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(ms, obj);
				ms.Position = 0;

				return (T)formatter.Deserialize(ms);
			}
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