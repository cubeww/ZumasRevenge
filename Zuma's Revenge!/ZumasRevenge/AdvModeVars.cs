namespace ZumasRevenge;

public class AdvModeVars
{
	public int mHighestZoneBeat;

	public int mHighestLevelBeat;

	public bool[] mFirstTimeInZone = new bool[6];

	public int mNumDeathsCurLevel;

	public int mNumZumasCurLevel;

	public bool mPerfectZone;

	public int[] mNumTimesZoneBeat = new int[6];

	public int mDDSTier;

	public int mRestartDDSTier;

	public int mCurrentAdvScore;

	public int mCurrentAdvLevel;

	public int mCurrentAdvZone;

	public int mCurrentAdvLives;

	public int[] mBestLevelTime = new int[60];

	public CheckpointScores[] mCheckpointScores = new CheckpointScores[6];

	public AdvModeVars()
	{
		for (int i = 0; i < 6; i++)
		{
			mCheckpointScores[i] = new CheckpointScores();
		}
	}

	public void CopyFrom(AdvModeVars rhs)
	{
		mHighestZoneBeat = rhs.mHighestZoneBeat;
		mHighestLevelBeat = rhs.mHighestLevelBeat;
		for (int i = 0; i < 6; i++)
		{
			mFirstTimeInZone[i] = rhs.mFirstTimeInZone[i];
		}
		mNumDeathsCurLevel = rhs.mNumDeathsCurLevel;
		mNumZumasCurLevel = rhs.mNumZumasCurLevel;
		mPerfectZone = rhs.mPerfectZone;
		for (int j = 0; j < 6; j++)
		{
			mNumTimesZoneBeat[j] = rhs.mNumTimesZoneBeat[j];
		}
		mDDSTier = rhs.mDDSTier;
		mRestartDDSTier = rhs.mRestartDDSTier;
		mCurrentAdvScore = rhs.mCurrentAdvScore;
		mCurrentAdvLevel = rhs.mCurrentAdvLevel;
		mCurrentAdvZone = rhs.mCurrentAdvZone;
		mCurrentAdvLives = rhs.mCurrentAdvLives;
		for (int k = 0; k < 60; k++)
		{
			mBestLevelTime[k] = rhs.mBestLevelTime[k];
		}
		for (int l = 0; l < 6; l++)
		{
			mCheckpointScores[l].CopyFrom(rhs.mCheckpointScores[l]);
		}
	}
}
