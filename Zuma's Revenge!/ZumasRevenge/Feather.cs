using JeffLib;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public class Feather
{
	public Image mImage;

	public float mX;

	public float mY;

	public float mVX;

	public float mVY;

	public float mDecVX;

	public float mDecVY;

	public float mAlpha;

	public int mImgNum;

	public Oscillator mAngleOsc = new Oscillator();

	public void SyncState(DataSync sync)
	{
		sync.SyncFloat(ref mX);
		sync.SyncFloat(ref mY);
		sync.SyncFloat(ref mVX);
		sync.SyncFloat(ref mVY);
		sync.SyncFloat(ref mDecVX);
		sync.SyncFloat(ref mDecVY);
		sync.SyncFloat(ref mAlpha);
		sync.SyncLong(ref mImgNum);
		if (sync.isRead())
		{
			mImage = Res.GetImageByID((ResID)(1023 + (mImgNum - 1)));
		}
		sync.SyncFloat(ref mAngleOsc.mVal);
		sync.SyncFloat(ref mAngleOsc.mMinVal);
		sync.SyncFloat(ref mAngleOsc.mMaxVal);
		sync.SyncFloat(ref mAngleOsc.mInc);
		sync.SyncFloat(ref mAngleOsc.mAccel);
		sync.SyncBoolean(ref mAngleOsc.mForward);
	}
}
