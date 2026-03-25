namespace ZumasRevenge;

public class AdvModeTempleStats
{
	public int mHighestLevel;

	public int mBestTime = int.MaxValue;

	public int mBestScore;

	public int mNumLevelsAced;

	public int mNumPerfectLevels;

	public int mNumClearCurves;

	public int[] mBossDeaths = new int[6];

	public int[] mLevelDeaths = new int[60];

	public int mTotalTimePlayed;

	public int mCurrentTime;

	public void CopyFrom(AdvModeTempleStats rhs)
	{
		mHighestLevel = rhs.mHighestLevel;
		mBestTime = rhs.mBestTime;
		mBestScore = rhs.mBestScore;
		mNumLevelsAced = rhs.mNumLevelsAced;
		mNumPerfectLevels = rhs.mNumPerfectLevels;
		mNumClearCurves = rhs.mNumClearCurves;
		for (int i = 0; i < 6; i++)
		{
			mBossDeaths[i] = rhs.mBossDeaths[i];
		}
		for (int j = 0; j < 60; j++)
		{
			mLevelDeaths[j] = rhs.mLevelDeaths[j];
		}
		mTotalTimePlayed = rhs.mTotalTimePlayed;
		mCurrentTime = rhs.mCurrentTime;
	}

	public void Sync(DataSync theSync)
	{
		theSync.SyncLong(ref mHighestLevel);
		theSync.SyncLong(ref mBestTime);
		theSync.SyncLong(ref mBestScore);
		theSync.SyncLong(ref mNumLevelsAced);
		theSync.SyncLong(ref mNumPerfectLevels);
		theSync.SyncLong(ref mNumClearCurves);
		for (int i = 0; i < 6; i++)
		{
			theSync.SyncLong(ref mBossDeaths[i]);
		}
		for (int j = 0; j < 60; j++)
		{
			theSync.SyncLong(ref mLevelDeaths[j]);
		}
		theSync.SyncLong(ref mTotalTimePlayed);
		theSync.SyncLong(ref mCurrentTime);
	}
}
