namespace ZumasRevenge;

public class EggFragment
{
	public float mVX;

	public float mVY;

	public float mDecVX;

	public float mDecVY;

	public float mAlpha;

	public int mCol;

	public float mX;

	public float mY;

	public void SyncState(DataSync sync)
	{
		sync.SyncFloat(ref mVX);
		sync.SyncFloat(ref mVY);
		sync.SyncFloat(ref mDecVX);
		sync.SyncFloat(ref mDecVY);
		sync.SyncFloat(ref mAlpha);
		sync.SyncLong(ref mCol);
		sync.SyncFloat(ref mX);
		sync.SyncFloat(ref mY);
	}
}
