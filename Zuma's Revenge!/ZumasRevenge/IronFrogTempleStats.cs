namespace ZumasRevenge;

public class IronFrogTempleStats
{
	public int mNumAttempts;

	public int mNumVictories;

	public int mBestTime;

	public int mCurTime;

	public int mBestScore;

	public int mHighestLevel;

	public int[] mLevelDeaths = new int[10];

	public int mTotalTimePlayed;

	public void CopyFrom(IronFrogTempleStats rhs)
	{
		mNumAttempts = rhs.mNumAttempts;
		mNumVictories = rhs.mNumVictories;
		mBestTime = rhs.mBestTime;
		mCurTime = rhs.mCurTime;
		mBestScore = rhs.mBestScore;
		mHighestLevel = rhs.mHighestLevel;
		for (int i = 0; i < 10; i++)
		{
			mLevelDeaths[i] = rhs.mLevelDeaths[i];
		}
		mTotalTimePlayed = rhs.mTotalTimePlayed;
	}

	public void Sync(DataSync theSync)
	{
		theSync.SyncLong(ref mNumAttempts);
		theSync.SyncLong(ref mNumVictories);
		theSync.SyncLong(ref mBestTime);
		theSync.SyncLong(ref mCurTime);
		theSync.SyncLong(ref mBestScore);
		theSync.SyncLong(ref mHighestLevel);
		for (int i = 0; i < 10; i++)
		{
			theSync.SyncLong(ref mLevelDeaths[i]);
		}
		theSync.SyncLong(ref mTotalTimePlayed);
	}
}
