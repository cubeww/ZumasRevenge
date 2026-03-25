namespace ZumasRevenge;

public class CheckpointScores
{
	public int mZoneStart;

	public int mMidpoint;

	public int mBoss;

	public void CopyFrom(CheckpointScores rhs)
	{
		mZoneStart = rhs.mZoneStart;
		mMidpoint = rhs.mMidpoint;
		mBoss = rhs.mBoss;
	}
}
