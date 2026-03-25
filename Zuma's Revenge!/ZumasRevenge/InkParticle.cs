using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class InkParticle
{
	public float mWidthPct;

	public float mHeightPct;

	public float mAngle;

	public float mX;

	public float mY;

	public Image mImage;

	public float mInitSpeed;

	public float mVX;

	public float mVY;

	public float mGravity;

	public float mAlpha;

	public float mAlphaRate;

	public float mJiggleRate;

	public int mJiggleDir;

	public int mPostHitCount;

	public void SyncState(DataSync s)
	{
		Buffer buffer = s.GetBuffer();
		if (s.isRead())
		{
			buffer.WriteBoolean(mImage == Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_GLOBULE1));
		}
		else if (buffer.ReadBoolean())
		{
			mImage = Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_GLOBULE1);
		}
		else
		{
			mImage = Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_GLOBULE2);
		}
		s.SyncFloat(ref mWidthPct);
		s.SyncFloat(ref mHeightPct);
		s.SyncFloat(ref mX);
		s.SyncFloat(ref mY);
		s.SyncFloat(ref mAngle);
		s.SyncFloat(ref mInitSpeed);
		s.SyncFloat(ref mVX);
		s.SyncFloat(ref mVY);
		s.SyncFloat(ref mGravity);
		s.SyncFloat(ref mAlpha);
		s.SyncFloat(ref mAlphaRate);
		s.SyncFloat(ref mJiggleRate);
		s.SyncLong(ref mJiggleDir);
		s.SyncLong(ref mPostHitCount);
	}
}
