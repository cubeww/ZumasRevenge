using SexyFramework.Graphics;

namespace ZumasRevenge;

public class EyeBullet
{
	public PIEffect mProjectile;

	public PIEffect mSparks;

	public PIEffect mExplosion;

	public bool mSparkFirstFrame;

	public float mInitialAlpha;

	public int mXOff;

	public int mYOff;

	public bool Update(int x, int y, bool do_explosion)
	{
		if (mInitialAlpha < 255f)
		{
			mInitialAlpha += Common._M(15f);
		}
		mProjectile.mDrawTransform.LoadIdentity();
		float num = GameApp.DownScaleNum(1f);
		mProjectile.mDrawTransform.Scale(num, num);
		mProjectile.mDrawTransform.Translate(Common._S(x + mXOff), Common._S(y + mYOff));
		mProjectile.mColor.mAlpha = (int)mInitialAlpha;
		mProjectile.Update();
		if ((mSparks.mFrameNum < (float)mSparks.mLastFrameNum || mSparks.mCurNumParticles > 0) && (mSparks.mFrameNum > 0f || mSparkFirstFrame))
		{
			mSparkFirstFrame = false;
			mSparks.mDrawTransform.LoadIdentity();
			float num2 = GameApp.DownScaleNum(1f);
			mSparks.mDrawTransform.Scale(num2, num2);
			mSparks.mDrawTransform.Translate(Common._S(x + mXOff), Common._S(y + mYOff));
			mSparks.Update();
		}
		if (do_explosion)
		{
			mExplosion.mDrawTransform.LoadIdentity();
			float num3 = GameApp.DownScaleNum(1f);
			mExplosion.mDrawTransform.Scale(num3, num3);
			mExplosion.mDrawTransform.Translate(Common._S(x + mXOff), Common._S(y + mYOff));
			mExplosion.Update();
			if (mExplosion.mFrameNum > (float)mExplosion.mLastFrameNum)
			{
				return true;
			}
		}
		return false;
	}

	public void Draw(Graphics g, int alpha)
	{
		g.PushState();
		mProjectile.mColor.mAlpha = alpha;
		mProjectile.Draw(g);
		g.PopState();
		if (mSparks.mCurNumParticles > 0)
		{
			g.PushState();
			mSparks.mColor.mAlpha = alpha;
			mSparks.Draw(g);
			g.PopState();
		}
		if (mExplosion.mFrameNum > 0f)
		{
			g.PushState();
			mExplosion.mColor.mAlpha = alpha;
			mExplosion.Draw(g);
			g.PopState();
		}
	}

	public void SyncState(DataSync sync)
	{
		sync.SyncLong(ref mXOff);
		sync.SyncLong(ref mYOff);
		sync.SyncFloat(ref mInitialAlpha);
		sync.SyncBoolean(ref mSparkFirstFrame);
		if (sync.isWrite())
		{
			Common.SerializePIEffect(mExplosion, sync);
			Common.SerializePIEffect(mProjectile, sync);
			Common.SerializePIEffect(mSparks, sync);
			return;
		}
		mExplosion = GameApp.gApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_STONEBOSSPROJEXPLOSION").Duplicate();
		mProjectile = GameApp.gApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_STONEBOSSPROJ").Duplicate();
		mSparks = GameApp.gApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_STONEBOSSPROJSPARKS").Duplicate();
		Common.DeserializePIEffect(mExplosion, sync);
		Common.DeserializePIEffect(mProjectile, sync);
		Common.DeserializePIEffect(mSparks, sync);
	}
}
