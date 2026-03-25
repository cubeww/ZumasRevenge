using SexyFramework.Graphics;

namespace ZumasRevenge;

public class BeamComponent
{
	public MemoryImage mImage;

	public float mX;

	public float mY;

	public float mVX;

	public float mVY;

	public float mV0;

	public float mDistTraveled;

	public bool mAdditive;

	public int mAlphaDelta;

	public int mMinAlpha;

	public int mCel;

	public Color mColor = default(Color);

	public void SyncState(DataSync sync)
	{
		sync.SyncFloat(ref mX);
		sync.SyncFloat(ref mY);
		sync.SyncFloat(ref mVX);
		sync.SyncFloat(ref mVY);
		sync.SyncFloat(ref mV0);
		sync.SyncFloat(ref mDistTraveled);
		sync.SyncBoolean(ref mAdditive);
		sync.SyncLong(ref mAlphaDelta);
		sync.SyncLong(ref mMinAlpha);
		sync.SyncLong(ref mCel);
		sync.SyncLong(ref mColor.mAlpha);
		sync.SyncLong(ref mColor.mRed);
		sync.SyncLong(ref mColor.mGreen);
		sync.SyncLong(ref mColor.mBlue);
	}
}
