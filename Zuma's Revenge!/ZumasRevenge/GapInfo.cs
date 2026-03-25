namespace ZumasRevenge;

public class GapInfo
{
	public int mCurve;

	public int mDist;

	public int mBallId;

	public void SyncState(DataSync sync)
	{
		sync.SyncLong(ref mCurve);
		sync.SyncLong(ref mDist);
		sync.SyncLong(ref mBallId);
	}
}
