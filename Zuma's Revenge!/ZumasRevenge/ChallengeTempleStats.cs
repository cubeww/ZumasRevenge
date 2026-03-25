namespace ZumasRevenge;

public class ChallengeTempleStats
{
	public int mHighestScore;

	public int mNumTimesHitScoreTarget;

	public int mHighestMult;

	public int[] mNumTimesPlayedCurve = new int[70];

	public int mTotalTime;

	public void CopyFrom(ChallengeTempleStats rhs)
	{
		mHighestScore = rhs.mHighestScore;
		mNumTimesHitScoreTarget = rhs.mNumTimesHitScoreTarget;
		mHighestMult = rhs.mHighestMult;
		for (int i = 0; i < 70; i++)
		{
			mNumTimesPlayedCurve[i] = rhs.mNumTimesPlayedCurve[i];
		}
		mTotalTime = rhs.mTotalTime;
	}

	public void Sync(DataSync theSync)
	{
		theSync.SyncLong(ref mHighestScore);
		theSync.SyncLong(ref mNumTimesHitScoreTarget);
		theSync.SyncLong(ref mHighestMult);
		for (int i = 0; i < 70; i++)
		{
			theSync.SyncLong(ref mNumTimesPlayedCurve[i]);
		}
		theSync.SyncLong(ref mTotalTime);
	}
}
