using SexyFramework.Graphics;

namespace ZumasRevenge;

public class Steam
{
	public float mAlpha = 255f;

	public float mAlphaDec;

	public float mAngle;

	public float mAngleInc;

	public float mXOff;

	public float mYOff;

	public float mSize = 0.1f;

	public float mVX;

	public float mVY;

	public int mImgNum;

	public Image mImage;

	public void SyncState(DataSync sync)
	{
		sync.SyncFloat(ref mAlpha);
		sync.SyncFloat(ref mAlphaDec);
		sync.SyncFloat(ref mAngle);
		sync.SyncFloat(ref mAngleInc);
		sync.SyncFloat(ref mXOff);
		sync.SyncFloat(ref mYOff);
		sync.SyncFloat(ref mSize);
		sync.SyncFloat(ref mVX);
		sync.SyncFloat(ref mVY);
		sync.SyncLong(ref mImgNum);
		if (sync.isRead())
		{
			mImage = ((mImage == null) ? Res.GetImageByID(ResID.IMAGE_BOSS_STONEHEAD_FOG1) : Res.GetImageByID(ResID.IMAGE_BOSS_STONEHEAD_FOG2));
		}
	}
}
