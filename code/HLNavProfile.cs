namespace Sandbox.Debug
{
	public static class Profile
	{
		//static Stopwatch sw = Stopwatch.StartNew();
		static Entry Root = new Entry();
		static TimeSince timeSince;

		internal class Entry
		{
			public string Name;
			public int Calls;
			public double Times;

			List<Entry> Children;

			public Entry GetOrCreateChild(string name)
			{
				Children ??= new();

				for (int i = 0; i < Children.Count; i++)
				{
					if (Children[i].Name == name)
						return Children[i];
				}

				var e = new Entry();
				e.Name = name;

				Children.Add(e);
				return e;
			}

			public void Add(double v)
			{
				Calls++;
				Times += v;
			}

			public void Wipe()
			{
				Calls = 0;
				Times = 0;

				if (Children == null) return;

				for (int i = 0; i < Children.Count; i++)
				{
					Children[i].Wipe();
				}
			}

			public string GetString(int indent = 0)
			{
				var str = $"{new string(' ', indent * 2)}{Times:0.00}ms  {Calls} - {Name}\n";

				if (indent == 0)
					str = "";

				if (Children == null)
					return str;

				foreach (var child in Children.OrderByDescending(x => x.Times))
				{
					if (child.Calls == 0) continue;
					str += child.GetString(indent + 1);
				}

				return str;
			}
		}

		public static IDisposable Scope(string name)
		{
			var scope = new ProfileScope(name);
			return scope;
		}

		[Event.Hotload]
		public static void Hotloaded()
		{
			Root = new Entry();
		}

		[Event.Tick]
		static void Frame()
		{
			if (timeSince >= 0.5f)
			{
				timeSince = 0;

				DebugOverlay.ScreenText(Root.GetString(), 20, 0.5f);
			}

			Root.Wipe();
		}

		internal struct ProfileScope : System.IDisposable
		{
			internal Entry Parent;
			internal Entry Me;
			public double StartTime;

			public ProfileScope(string name)
			{
				Parent = Profile.Root;

				Me = Parent.GetOrCreateChild(name);
				StartTime = 1;
				Profile.Root = Me;
			}

			public void Dispose()
			{
				//Me.Add(sw.Elapsed.TotalMilliseconds - StartTime);
				Profile.Root = Parent;
			}
		}
	}
}