namespace ZumasRevenge;

public class GameStats
{
	public int mTimePlayed;

	public int mDangerTimePlayed;

	public int mNumBallsCleared;

	public int mNumGemsCleared;

	public int mNumGaps;

	public int mNumCombos;

	public int mMaxCombo;

	public int mMaxComboScore;

	public int mMaxInARow;

	public int mMaxInARowScore;

	public int mTotalShots;

	public int mNumMisses;

	public GameStats()
	{
		mTimePlayed = 0;
		mNumBallsCleared = 0;
		mNumGemsCleared = 0;
		mNumGaps = 0;
		mNumCombos = 0;
		mMaxCombo = -1;
		mMaxComboScore = 0;
		mMaxInARow = 0;
		mMaxInARowScore = 0;
		mDangerTimePlayed = 0;
		mTotalShots = (mNumMisses = 0);
	}

	public void Reset()
	{
		mTimePlayed = 0;
		mNumBallsCleared = 0;
		mNumGemsCleared = 0;
		mNumGaps = 0;
		mNumCombos = 0;
		mMaxCombo = -1;
		mMaxComboScore = 0;
		mMaxInARow = 0;
		mMaxInARowScore = 0;
		mDangerTimePlayed = 0;
		mTotalShots = (mNumMisses = 0);
	}

	public void Add(GameStats theStats)
	{
		mTimePlayed += theStats.mTimePlayed;
		mNumBallsCleared += theStats.mNumBallsCleared;
		mNumGemsCleared += theStats.mNumGemsCleared;
		mNumCombos += theStats.mNumCombos;
		mNumGaps += theStats.mNumGaps;
		if (theStats.mMaxCombo > mMaxCombo || (theStats.mMaxCombo == mMaxCombo && theStats.mMaxComboScore > mMaxComboScore))
		{
			mMaxCombo = theStats.mMaxCombo;
			mMaxComboScore = theStats.mMaxComboScore;
		}
		if (theStats.mMaxInARow > mMaxInARow)
		{
			mMaxInARow = theStats.mMaxInARow;
			mMaxInARowScore = theStats.mMaxInARowScore;
		}
	}

	public void SyncState(DataSync theSync)
	{
		theSync.SyncLong(ref mTimePlayed);
		theSync.SyncLong(ref mNumBallsCleared);
		theSync.SyncLong(ref mNumGemsCleared);
		theSync.SyncLong(ref mNumGaps);
		theSync.SyncLong(ref mNumCombos);
		theSync.SyncLong(ref mMaxCombo);
		theSync.SyncLong(ref mMaxComboScore);
		theSync.SyncLong(ref mMaxInARow);
		theSync.SyncLong(ref mMaxInARowScore);
		theSync.SyncLong(ref mDangerTimePlayed);
		theSync.SyncLong(ref mTotalShots);
		theSync.SyncLong(ref mNumMisses);
	}
}
