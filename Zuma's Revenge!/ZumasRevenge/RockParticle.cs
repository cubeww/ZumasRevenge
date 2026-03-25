namespace ZumasRevenge;

public class RockParticle
{
	public float mAlpha;

	public int mCel;

	public float mX;

	public float mY;

	public float mVX;

	public float mVY;

	public RockParticle()
	{
	}

	public RockParticle(RockParticle rhs)
	{
		mAlpha = rhs.mAlpha;
		mCel = rhs.mCel;
		mX = rhs.mX;
		mY = rhs.mY;
		mVX = rhs.mVX;
		mVY = rhs.mVY;
	}

	public void SyncState(DataSync sync)
	{
		sync.SyncLong(ref mCel);
		sync.SyncFloat(ref mAlpha);
		sync.SyncFloat(ref mX);
		sync.SyncFloat(ref mY);
		sync.SyncFloat(ref mVX);
		sync.SyncFloat(ref mVY);
	}
}
