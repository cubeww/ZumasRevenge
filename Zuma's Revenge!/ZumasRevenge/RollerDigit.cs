namespace ZumasRevenge;

public class RollerDigit
{
	public int mNum = -1;

	public float mX;

	public float mY;

	public float mVY;

	public int mDelay;

	public int mBounceState;

	public int mRestingY;

	public void SyncState(DataSync sync)
	{
		sync.SyncLong(ref mNum);
		sync.SyncFloat(ref mX);
		sync.SyncFloat(ref mY);
		sync.SyncFloat(ref mVY);
		sync.SyncLong(ref mDelay);
		sync.SyncLong(ref mBounceState);
		sync.SyncLong(ref mRestingY);
	}
}
