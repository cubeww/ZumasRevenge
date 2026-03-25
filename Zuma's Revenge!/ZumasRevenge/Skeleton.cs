using System;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class Skeleton : IDisposable
{
	public bool mHasPowerup;

	public float mVX;

	public float mVY;

	public float mX;

	public float mY;

	public int mDelay;

	public float mOrbSize;

	public float mOrbSizeDec;

	public float mAlpha;

	public float mFadeAlpha;

	public bool mIncAlpha;

	public bool mActivated;

	public bool mEffectDone;

	public bool mFadeOut;

	public int mRibCel;

	public float mHeadYOff;

	public float mHeadVY;

	public int mHeadBounceCount;

	public int mUpdateCount;

	public int mExplosionCel;

	public OrbPowerRing[] mRings = new OrbPowerRing[2];

	public Skeleton()
	{
		mAlpha = 0f;
		mIncAlpha = true;
		mActivated = false;
		mEffectDone = false;
		mFadeOut = false;
		mFadeAlpha = 255f;
		mOrbSize = 1f;
		mOrbSizeDec = 0f;
		mRibCel = 0;
		mHeadYOff = 0f;
		mHeadVY = 0f;
		mHeadBounceCount = 0;
		mUpdateCount = 0;
		mExplosionCel = 0;
		mRings[0] = (mRings[1] = null);
	}

	public virtual void Dispose()
	{
		if (mRings[0] != null)
		{
			mRings[0].Dispose();
		}
		if (mRings[1] != null)
		{
			mRings[1].Dispose();
		}
	}

	public void Update()
	{
		if (mDelay > 0)
		{
			mDelay--;
			return;
		}
		mUpdateCount++;
		mX += mVX;
		mY += mVY;
		if (mHasPowerup)
		{
			int num = (int)Common._M(14f);
			if (mIncAlpha)
			{
				mAlpha += num;
				if (mAlpha >= 255f)
				{
					mAlpha = 255f;
					mIncAlpha = false;
				}
			}
			else
			{
				mAlpha -= num;
				if (mAlpha <= 0f)
				{
					mAlpha = 0f;
					mIncAlpha = true;
				}
			}
			if (mActivated)
			{
				if (mUpdateCount % Common._M(4) == 0)
				{
					mRibCel++;
				}
				if (mHeadBounceCount < Common._M(5))
				{
					float num2 = Common._M(25);
					float num3 = num2 - (float)Common._M(10);
					if (mHeadBounceCount % 2 == 0)
					{
						mHeadYOff += mHeadVY;
						if (mHeadYOff >= num2)
						{
							mHeadYOff = num2;
							mHeadBounceCount++;
						}
					}
					else
					{
						mHeadYOff -= mHeadVY;
						if (mHeadYOff <= num3)
						{
							mHeadYOff = num3;
							mHeadBounceCount++;
						}
					}
				}
			}
		}
		else if (mActivated)
		{
			if (mUpdateCount % Common._M(2) == 0)
			{
				mExplosionCel++;
			}
			if (mExplosionCel == Common._M(3))
			{
				mFadeOut = true;
			}
			Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_MINI_SKELE_EXPLODE);
			if (mExplosionCel >= imageByID.mNumCols * imageByID.mNumRows)
			{
				mEffectDone = true;
			}
		}
		if (mRings[0] != null)
		{
			if (mOrbSize > 0f)
			{
				mOrbSize -= mOrbSizeDec;
				if (mOrbSize < 0f)
				{
					mOrbSize = 0f;
				}
			}
			mRings[0].Update();
			mRings[1].Update();
			if (mRings[0].IsDone() && mRings[1].IsDone())
			{
				mEffectDone = true;
			}
			else if (!mRings[0].IsExpanding())
			{
				mFadeOut = true;
			}
		}
		if (mFadeOut)
		{
			mFadeAlpha -= 255f / Common._M(15f);
			if (mFadeAlpha < 0f)
			{
				mFadeAlpha = 0f;
			}
		}
	}

	public void DoHit()
	{
		float max_radius = Common._M(30f);
		float alpha_fade = 255f / Common._M(20f);
		float size_fade = 1f / Common._M(50f);
		float angle_inc = Common._M(0.2f);
		mRings[0] = new OrbPowerRing(0f, max_radius, alpha_fade, size_fade, angle_inc);
		mRings[1] = new OrbPowerRing(3.14159f, max_radius, alpha_fade, size_fade, angle_inc);
		mOrbSizeDec = 1f / Common._M(50f);
		mHeadVY = Common._M(2f);
	}

	public void SetupFade(Graphics g)
	{
		if (mFadeOut)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)mFadeAlpha);
		}
	}

	public void Draw(Graphics g)
	{
		if (mDelay > 0 || (mFadeOut && mFadeAlpha <= 0f))
		{
			return;
		}
		SetupFade(g);
		if (!mHasPowerup)
		{
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_MINI_SKELETON), (int)Common._S(mX), (int)Common._S(mY));
		}
		g.SetColorizeImages(colorizeImages: false);
		if (mHasPowerup)
		{
			SetupFade(g);
			if (mRibCel < Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_MINI_SKELE_RIBS).mNumCols)
			{
				g.DrawImageCel(Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_MINI_SKELE_RIBS), (int)Common._S(mX + (float)Common._M(1)), (int)Common._S(mY + (float)Common._M1(34)), mRibCel);
			}
			Image theImage = (mActivated ? Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_MINI_SKELE_HEAD_CLOSED) : Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_MINI_SKELE_HEAD));
			g.DrawImage(theImage, (int)Common._S(mX + (float)Common._M(0)), (int)Common._S(mY + (float)Common._M1(0) + mHeadYOff));
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_MINI_SKELE_JAW), (int)Common._S(mX + (float)Common._M(34)), (int)Common._S(mY + (float)Common._M1(82)));
			Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_GLOWBALL);
			float num = (float)imageByID.GetCelWidth() * mOrbSize;
			float num2 = (float)imageByID.GetCelHeight() * mOrbSize;
			g.DrawImage(imageByID, (int)(Common._S(mX + (float)Common._M(28)) + (float)(imageByID.GetCelWidth() / 2) - num / 2f), (int)(Common._S(mY + (float)Common._M1(40)) + (float)(imageByID.GetCelHeight() / 2) - num2 / 2f), (int)num, (int)num2);
			g.SetColorizeImages(colorizeImages: true);
			g.SetDrawMode(1);
			g.SetColor(255, 255, 255, (int)((mFadeAlpha < mAlpha) ? mFadeAlpha : mAlpha));
			g.DrawImage(imageByID, (int)(Common._S(mX + (float)Common._M(28)) + (float)(imageByID.GetCelWidth() / 2) - num / 2f), (int)(Common._S(mY + (float)Common._M1(40)) + (float)(imageByID.GetCelHeight() / 2) - num2 / 2f), (int)num, (int)num2);
			g.SetColorizeImages(colorizeImages: false);
			g.SetDrawMode(0);
			SetupFade(g);
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_MINI_SKELE_RIBS_SHADOW);
			if (mRibCel < imageByID2.mNumCols)
			{
				g.DrawImageCel(imageByID2, (int)Common._S(mX + (float)Common._M(1)), (int)Common._S(mY + (float)Common._M1(34)), mRibCel);
			}
			theImage = (mActivated ? Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_MINI_SKELE_HEAD_CLOSED) : Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_MINI_SKELE_HEAD_SHADOW));
			g.DrawImage(theImage, (int)Common._S(mX + (float)Common._M(0)), (int)Common._S(mY + (float)Common._M1(0) + mHeadYOff));
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_MINI_SKELE_JAW_SHADOW), (int)Common._S(mX + (float)Common._M(34)), (int)Common._S(mY + (float)Common._M1(82)));
			g.SetColorizeImages(colorizeImages: false);
		}
		SetupFade(g);
		if (!mHasPowerup)
		{
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_MINI_SKELETON_NOSHADOW), (int)Common._S(mX), (int)Common._S(mY));
		}
		g.SetColorizeImages(colorizeImages: false);
		Image imageByID3 = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_MINI_SKELE_EXPLODE);
		if (mActivated && !mHasPowerup && mExplosionCel < imageByID3.mNumCols * imageByID3.mNumRows)
		{
			Rect celRect = imageByID3.GetCelRect(mExplosionCel);
			int theWidth = celRect.mWidth * 4;
			int theHeight = celRect.mHeight * 4;
			g.DrawImage(imageByID3, new Rect((int)Common._S(mX + (float)Common._M(-50)), (int)Common._S(mY + (float)Common._M1(-50)), theWidth, theHeight), celRect);
		}
		if (mRings[0] != null)
		{
			Image imageByID4 = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_MINI_SKELETON);
			for (int i = 0; i < 2; i++)
			{
				mRings[i].Draw(g, Common._S(mX) + (float)(imageByID4.GetCelWidth() / 2), Common._S(mY) + (float)(imageByID4.GetCelHeight() / 2));
			}
		}
	}

	public void SyncState(DataSync sync)
	{
		sync.SyncBoolean(ref mHasPowerup);
		sync.SyncFloat(ref mVX);
		sync.SyncFloat(ref mVY);
		sync.SyncFloat(ref mX);
		sync.SyncFloat(ref mY);
		sync.SyncLong(ref mDelay);
		sync.SyncFloat(ref mOrbSize);
		sync.SyncFloat(ref mOrbSizeDec);
		sync.SyncFloat(ref mFadeAlpha);
		sync.SyncBoolean(ref mFadeOut);
		sync.SyncLong(ref mRibCel);
		sync.SyncFloat(ref mHeadYOff);
		sync.SyncFloat(ref mHeadVY);
		sync.SyncLong(ref mHeadBounceCount);
		sync.SyncLong(ref mUpdateCount);
		sync.SyncLong(ref mExplosionCel);
		sync.SyncBoolean(ref mActivated);
		SexyFramework.Misc.Buffer buffer = sync.GetBuffer();
		if (sync.isRead())
		{
			if (buffer.ReadBoolean())
			{
				mRings[0] = new OrbPowerRing(0f, 0f, 0f, 0f, 0f);
				mRings[1] = new OrbPowerRing(0f, 0f, 0f, 0f, 0f);
				for (int i = 0; i < 2; i++)
				{
					mRings[i].SyncState(sync);
				}
			}
		}
		else if (mRings[0] == null)
		{
			buffer.WriteBoolean(theBool: false);
		}
		else
		{
			buffer.WriteBoolean(theBool: true);
			for (int j = 0; j < 2; j++)
			{
				mRings[j].SyncState(sync);
			}
		}
	}
}
