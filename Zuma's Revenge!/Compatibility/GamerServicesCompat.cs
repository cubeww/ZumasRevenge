using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;

namespace Microsoft.Xna.Framework.GamerServices;

public enum MessageBoxIcon
{
	Alert
}

public enum LeaderboardKey
{
	BestScoreLifeTime
}

public sealed class GameUpdateRequiredException : Exception
{
	public GameUpdateRequiredException()
	{
	}

	public GameUpdateRequiredException(string message)
		: base(message)
	{
	}
}

public sealed class GamerServicesComponent : GameComponent
{
	public GamerServicesComponent(Game game)
		: base(game)
	{
	}
}

public static class Guide
{
	public static bool SimulateTrialMode { get; set; }

	public static bool IsVisible => false;

	public static bool IsTrialMode => SimulateTrialMode;

	public static IAsyncResult BeginShowMessageBox(string title, string text, IList<string> buttons, int focusButton, MessageBoxIcon icon, AsyncCallback callback, object state)
	{
		var result = new CompletedAsyncResult<int?>(focusButton, state);
		callback?.Invoke(result);
		return result;
	}

	public static int? EndShowMessageBox(IAsyncResult result)
	{
		return ((CompletedAsyncResult<int?>)result).Result;
	}

	public static void ShowMarketplace(PlayerIndex player)
	{
	}
}

public static class Gamer
{
	public static SignedInGamerCollection SignedInGamers { get; } = new SignedInGamerCollection();
}

public sealed class SignedInGamerCollection
{
	public SignedInGamer this[PlayerIndex player] => null;
}

public class GamerProfile
{
	public Stream GetGamerPicture()
	{
		return new MemoryStream();
	}
}

public class SignedInEventArgs : EventArgs
{
	public SignedInEventArgs(SignedInGamer gamer)
	{
		Gamer = gamer;
	}

	public SignedInGamer Gamer { get; }
}

public class SignedInGamer
{
	public static event EventHandler<SignedInEventArgs> SignedIn;

	public string Gamertag { get; set; } = "Player 1";

	public bool IsSignedInToLive { get; set; }

	public LeaderboardWriter LeaderboardWriter { get; } = new LeaderboardWriter();

	public GamerProfile GetProfile()
	{
		return new GamerProfile();
	}

	public IAsyncResult BeginGetAchievements(AsyncCallback callback, object state)
	{
		var result = new CompletedAsyncResult<AchievementCollection>(new AchievementCollection(), state);
		callback?.Invoke(result);
		return result;
	}

	public AchievementCollection EndGetAchievements(IAsyncResult result)
	{
		return ((CompletedAsyncResult<AchievementCollection>)result).Result;
	}

	public IAsyncResult BeginAwardAchievement(string achievementKey, AsyncCallback callback, object state)
	{
		var result = new CompletedAsyncResult<bool>(true, state);
		callback?.Invoke(result);
		return result;
	}

	public void EndAwardAchievement(IAsyncResult result)
	{
	}

	internal static void RaiseSignedIn(SignedInGamer gamer)
	{
		SignedIn?.Invoke(null, new SignedInEventArgs(gamer));
	}
}

public class Achievement
{
	public string Key { get; set; } = string.Empty;

	public bool IsEarned { get; set; }
}

public class AchievementCollection : List<Achievement>
{
}

public sealed class LeaderboardIdentity
{
	public static LeaderboardIdentity Create(LeaderboardKey key, int value)
	{
		return new LeaderboardIdentity();
	}
}

public sealed class LeaderboardColumnCollection
{
	private readonly Dictionary<string, int> m_values = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

	public int GetValueInt32(string key)
	{
		if (m_values.TryGetValue(key, out int value))
		{
			return value;
		}
		return 0;
	}

	public void SetValue(string key, int value)
	{
		m_values[key] = value;
	}
}

public sealed class LeaderboardEntry
{
	private int m_rating;

	public SignedInGamer Gamer { get; set; } = new SignedInGamer();

	public LeaderboardColumnCollection Columns { get; } = new LeaderboardColumnCollection();

	public int Rating
	{
		get => m_rating;
		set
		{
			m_rating = value;
			Columns.SetValue("BestScore", value);
		}
	}
}

public sealed class LeaderboardWriter
{
	public LeaderboardEntry GetLeaderboard(LeaderboardIdentity leaderboardId)
	{
		return new LeaderboardEntry();
	}
}

public sealed class LeaderboardReader
{
	public List<LeaderboardEntry> Entries { get; } = new List<LeaderboardEntry>();

	public int PageStart { get; private set; }

	public bool CanPageUp => false;

	public bool CanPageDown => false;

	public static IAsyncResult BeginRead(LeaderboardIdentity leaderboardId, int pageStart, int maxEntries, AsyncCallback callback, object state)
	{
		var reader = new LeaderboardReader
		{
			PageStart = pageStart
		};
		var result = new CompletedAsyncResult<LeaderboardReader>(reader, state);
		callback?.Invoke(result);
		return result;
	}

	public static LeaderboardReader EndRead(IAsyncResult result)
	{
		return ((CompletedAsyncResult<LeaderboardReader>)result).Result;
	}

	public IAsyncResult BeginPageUp(AsyncCallback callback, object state)
	{
		var result = new CompletedAsyncResult<LeaderboardReader>(this, state);
		callback?.Invoke(result);
		return result;
	}

	public void EndPageUp(IAsyncResult result)
	{
	}

	public IAsyncResult BeginPageDown(AsyncCallback callback, object state)
	{
		var result = new CompletedAsyncResult<LeaderboardReader>(this, state);
		callback?.Invoke(result);
		return result;
	}

	public void EndPageDown(IAsyncResult result)
	{
	}
}

internal sealed class CompletedAsyncResult<TResult> : IAsyncResult
{
	private readonly ManualResetEvent m_waitHandle = new ManualResetEvent(true);

	public CompletedAsyncResult(TResult result, object asyncState)
	{
		Result = result;
		AsyncState = asyncState;
	}

	public TResult Result { get; }

	public object AsyncState { get; }

	public WaitHandle AsyncWaitHandle => m_waitHandle;

	public bool CompletedSynchronously => true;

	public bool IsCompleted => true;
}
