namespace ZumasRevenge;

public class RockChunk
{
	public int mCol;

	public float mX;

	public float mY;

	public float mVX;

	public float mVY;

	public float mAlpha;

	public void SyncState(DataSync sync)
	{
		sync.SyncLong(ref mCol);
		sync.SyncFloat(ref mX);
		sync.SyncFloat(ref mY);
		sync.SyncFloat(ref mVX);
		sync.SyncFloat(ref mVY);
		sync.SyncFloat(ref mAlpha);
	}
}
