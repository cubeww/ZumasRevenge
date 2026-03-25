using System;
using System.Collections.Generic;
using System.Linq;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.PIL;

namespace ZumasRevenge;

public class BossTiger : BossShoot
{
	private enum Turtorial
	{
		Tutorial_None,
		Tutorial_HitMeNoBalls,
		Tutorial_NoBallsTaunting,
		Tutorial_NoBallsRecoiling,
		Tutorial_NoBallsMovingBack,
		Tutorial_HitMeBalls
	}

	protected static int PAW_LEFT_X_OFF = 0;

	protected static int PAW_RIGHT_X_OFF = 151;

	protected static int SKULL_LEFT_X_OFF = 2;

	protected static int SKULL_LEFT_Y_OFF = 72;

	protected static int SKULL_RIGHT_X_OFF = 147;

	protected static int SKULL_RIGHT_Y_OFF = 73;

	protected static Image[] gTigerBulletImages = new Image[3];

	protected static Image[] gFiringImages = new Image[3];

	protected List<Skull> mSkulls = new List<Skull>();

	protected BossFiringState mLeftPaw = new BossFiringState();

	protected BossFiringState mRightPaw = new BossFiringState();

	protected int mPawModifyingHeadAngle;

	protected int mTutorialState;

	protected float mStompVX;

	protected float mStompVY;

	protected float mStompAY;

	protected float mStompRestingY;

	protected int mStompPause;

	protected int mStompCount;

	protected List<Skull> mSkullsPool = new List<Skull>();

	protected override bool PreBulletUpdate(BossBullet b, int index)
	{
		if (b.mDelay > 0)
		{
			b.mDelay--;
		}
		else if (b.mState == 0)
		{
			int num = (int)(mX - (float)(mWidth / 2));
			int num2 = ((mLevel.mFrog.GetCenterX() > num + PAW_LEFT_X_OFF) ? 1 : (-1));
			int num3 = ((mLevel.mFrog.GetCenterX() > num + PAW_RIGHT_X_OFF) ? 1 : (-1));
			float num4 = 0f;
			BossFiringState bossFiringState = null;
			int num5 = 0;
			int num6 = 0;
			if (num3 == -1 && mRightPaw.mState == 0)
			{
				mRightPaw = new BossFiringState();
				bossFiringState = mRightPaw;
				b.mImageNum = 2;
				num5 = SKULL_RIGHT_X_OFF;
				num6 = SKULL_RIGHT_Y_OFF;
				if (mPawModifyingHeadAngle == 0)
				{
					mPawModifyingHeadAngle = 1;
				}
			}
			else if (num2 == 1 && mLeftPaw.mState == 0)
			{
				mLeftPaw = new BossFiringState();
				bossFiringState = mLeftPaw;
				b.mImageNum = 1;
				num5 = SKULL_LEFT_X_OFF;
				num6 = SKULL_LEFT_Y_OFF;
				if (mPawModifyingHeadAngle == 0)
				{
					mPawModifyingHeadAngle = -1;
				}
			}
			if (bossFiringState != null)
			{
				bossFiringState.mBulletId = b.mId;
				bossFiringState.mState = 1;
				b.mState++;
				Image image = gTigerBulletImages[b.mImageNum];
				num4 = 3.14159f - Common.GetCanonicalAngleRad(FireBulletAtPlayer(b, SexyFramework.Common.FloatRange(base.mMinBulletSpeed, base.mMaxBulletSpeed), mX - (float)(mWidth / 2) + (float)(image.mWidth / 2) + (float)num5, mY - (float)(mHeight / 2) + (float)(image.mHeight / 2) + (float)num6)) + 1.570795f;
				num4 *= -1f;
				b.mTargetVX = b.mVX;
				b.mTargetVY = b.mVY;
				bossFiringState.mTargetSkullAngle = num4;
				bossFiringState.mSkullAngle = 0f;
				bossFiringState.mSkullAngleInc = num4 / (float)Common._M(12);
				Skull skull = GetSkull(bossFiringState == mLeftPaw);
				skull.mImageNum = b.mImageNum;
				skull.mImage = gFiringImages[b.mImageNum];
				b.mData = skull;
			}
		}
		if (b.mState <= 1)
		{
			return true;
		}
		return false;
	}

	private void FreeSkull(Skull temp)
	{
		temp?.Dispose();
	}

	private Skull GetSkull(bool p)
	{
		Skull skull = null;
		if (mSkullsPool.Count > 0)
		{
			skull = mSkullsPool.Last();
			mSkullsPool.RemoveAt(mSkullsPool.Count - 1);
			skull.mIsLeft = p;
		}
		else
		{
			skull = new Skull(p);
		}
		return skull;
	}

	protected override void DrawBossSpecificArt(Graphics g)
	{
		int num = (int)(mX - (float)(mWidth / 2) + (float)mShakeXOff);
		int num2 = (int)(mY - (float)(mHeight / 2) + (float)mShakeYOff);
		if (mHP > 0f && !mDoDeathExplosions && !mLevel.mBoard.IsPaused())
		{
			if (mTeleportDir != 0)
			{
				g.PushState();
				g.ClearClipRect();
			}
			if (mAlphaOverride < 255f)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)mAlphaOverride);
			}
			for (int i = 0; i < mBullets.Count; i++)
			{
				BossBullet bossBullet = mBullets[i];
				if (bossBullet.mDelay <= 0 && bossBullet.mState >= 2)
				{
					Image image = gTigerBulletImages[bossBullet.mImageNum];
					int num3 = image.mWidth;
					int num4 = image.mHeight;
					Skull skull = (Skull)bossBullet.mData;
					if (skull == null || skull.mAlpha < 255f)
					{
						g.DrawImageRotated(image, (int)(Common._S(bossBullet.mX) - (float)(num3 / 2)), (int)(Common._S(bossBullet.mY) - (float)(num4 / 2)), bossBullet.mAngle);
					}
					skull?.Draw(g, Common._S(bossBullet.mX) - (float)(num3 / 2), Common._S(bossBullet.mY) - (float)(num4 / 2), bossBullet.mAngle);
				}
			}
			g.SetColorizeImages(colorizeImages: false);
			if (mTeleportDir != 0)
			{
				g.PopState();
			}
		}
		for (int j = 0; j < mSkulls.Count; j++)
		{
			mSkulls[j].mParticleAlpha = mAlphaOverride / 255f;
			mSkulls[j].mAlpha = mAlphaOverride;
			mSkulls[j].Draw(g, 0f, 0f, 0f);
		}
		if (mAlphaOverride < 255f)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)mAlphaOverride);
		}
		Image image2 = null;
		if (mLeftPaw.mState < 4 || mLeftPaw.mState >= 6)
		{
			image2 = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_PAWBACK_LEFT);
			g.DrawImage(image2, Common._S(num + PAW_LEFT_X_OFF), (int)Common._S((float)(num2 + Common._M(53)) + mLeftPaw.mPawYOffset));
		}
		else if (mLeftPaw.mState < 6)
		{
			image2 = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_SKULL_LEFT);
			if (mLeftPaw.mState == 4 && mLeftPaw.mTimer == 0)
			{
				g.DrawImageRotated(image2, (int)Common._S((float)(num + SKULL_LEFT_X_OFF) + mLeftPaw.mSkullXOffset), (int)Common._S((float)(num2 + SKULL_LEFT_Y_OFF) + mLeftPaw.mSkullYOffset), mLeftPaw.mSkullAngle);
			}
			image2 = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_SWIPE);
			g.DrawImageMirror(image2, Common._S(num + Common._M(15)), Common._S(num2 + Common._M1(71)), image2.GetCelRect(mLeftPaw.mSwipeFrame));
		}
		if (mRightPaw.mState < 4 || mRightPaw.mState >= 6)
		{
			image2 = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_PAWBACK_RIGHT);
			g.DrawImage(image2, Common._S(num + PAW_RIGHT_X_OFF), (int)Common._S((float)(num2 + Common._M(55)) + mRightPaw.mPawYOffset));
		}
		else if (mRightPaw.mState < 6)
		{
			image2 = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_SKULL_RIGHT);
			if (mRightPaw.mState == 4 && mRightPaw.mTimer == 0)
			{
				g.DrawImageRotated(image2, (int)Common._S((float)(num + SKULL_RIGHT_X_OFF) + mRightPaw.mSkullXOffset), (int)Common._S((float)(num2 + SKULL_RIGHT_Y_OFF) + mRightPaw.mSkullYOffset), mRightPaw.mSkullAngle);
			}
			image2 = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_SWIPE);
			g.DrawImage(image2, Common._S(num + Common._M(125)), Common._S(num2 + Common._M1(71)), image2.GetCelRect(mRightPaw.mSwipeFrame));
		}
		int num5;
		int num6;
		Image imageByID;
		if (mDoExplosion)
		{
			num5 = Common._S(Common._M(33));
			num6 = Common._S(Common._M(-8));
			imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_HIT);
		}
		else
		{
			num5 = Common._S(Common._M(44));
			num6 = Common._S(Common._M(0));
			imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_HEAD);
		}
		if (mPawModifyingHeadAngle == 0)
		{
			g.DrawImage(imageByID, Common._S(num) + num5, Common._S(num2) + num6);
		}
		else
		{
			g.DrawImageRotated(imageByID, Common._S(num) + num5, Common._S(num2) + num6, (mPawModifyingHeadAngle == -1) ? mLeftPaw.mHeadAngle : mRightPaw.mHeadAngle);
		}
		float num7 = 0f;
		if (mLeftPaw.mState < 4 || mLeftPaw.mState >= 6)
		{
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_SKULL_LEFT);
			g.PushState();
			if (!SexyFramework.Common._eq(mLeftPaw.mSkullGrowPct, 1f, 0.001f))
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)Math.Min(mAlphaOverride, Math.Min(mLeftPaw.mSkullGrowPct * 255f, 255f)));
				num7 = (1f - mLeftPaw.mSkullGrowPct) * (float)imageByID2.mWidth / 2f;
			}
			float num8 = (float)imageByID2.mWidth * mLeftPaw.mSkullGrowPct;
			float num9 = (float)imageByID2.mHeight * mLeftPaw.mSkullGrowPct;
			g.DrawImage(imageByID2, (int)((float)Common._S(num + SKULL_LEFT_X_OFF) + num7 + Common._S(mLeftPaw.mSkullXOffset)), (int)Common._S((float)(num2 + SKULL_LEFT_Y_OFF) + mLeftPaw.mSkullYOffset), (int)num8, (int)num9);
			g.PopState();
		}
		if (mLeftPaw.mState >= 2 && mLeftPaw.mState <= 4)
		{
			BossBullet bulletById = GetBulletById(mLeftPaw.mBulletId);
			Skull skull2 = (Skull)bulletById.mData;
			if (skull2 != null && (mLeftPaw.mState != 4 || bulletById.mDelay > 0 || bulletById.mState < 2))
			{
				skull2.Draw(g, (float)Common._S(num + SKULL_LEFT_X_OFF) + num7 + Common._S(mLeftPaw.mSkullXOffset), (int)Common._S((float)(num2 + SKULL_LEFT_Y_OFF) + mLeftPaw.mSkullYOffset), mLeftPaw.mSkullAngle);
			}
		}
		num7 = 0f;
		if (mRightPaw.mState < 4 || mRightPaw.mState >= 6)
		{
			Image imageByID3 = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_SKULL_RIGHT);
			g.PushState();
			if (!SexyFramework.Common._eq(mRightPaw.mSkullGrowPct, 1f, 0.001f))
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)Math.Min(mAlphaOverride, Math.Min(mRightPaw.mSkullGrowPct * 255f, 255f)));
				num7 = (1f - mRightPaw.mSkullGrowPct) * (float)imageByID3.mWidth / 2f;
			}
			float num10 = (float)imageByID3.mWidth * mRightPaw.mSkullGrowPct;
			float num11 = (float)imageByID3.mHeight * mRightPaw.mSkullGrowPct;
			g.DrawImage(imageByID3, (int)((float)Common._S(num + SKULL_RIGHT_X_OFF) + num7 + (float)(int)Common._S(mRightPaw.mSkullXOffset)), (int)Common._S((float)(num2 + SKULL_RIGHT_Y_OFF) + mRightPaw.mSkullYOffset), (int)num10, (int)num11);
			g.PopState();
		}
		if (mRightPaw.mState >= 2 && mRightPaw.mState <= 4)
		{
			BossBullet bulletById2 = GetBulletById(mRightPaw.mBulletId);
			Skull skull3 = (Skull)bulletById2.mData;
			if (skull3 != null && (mRightPaw.mState != 4 || bulletById2.mDelay > 0 || bulletById2.mState < 2))
			{
				skull3.Draw(g, (float)Common._S(num + SKULL_RIGHT_X_OFF) + num7 + Common._S(mRightPaw.mSkullXOffset), (int)Common._S((float)(num2 + SKULL_RIGHT_Y_OFF) + mRightPaw.mSkullYOffset), mRightPaw.mSkullAngle);
			}
		}
		if (mLeftPaw.mState < 4 || mLeftPaw.mState >= 6)
		{
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_PAWFRONT_LEFT), Common._S(num), (int)Common._S((float)(num2 + Common._M(53)) + mLeftPaw.mPawYOffset));
		}
		if (mRightPaw.mState < 4 || mRightPaw.mState >= 6)
		{
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_PAWFRONT_RIGHT), Common._S(num + Common._M(163)), (int)Common._S((float)(num2 + Common._M1(55)) + mRightPaw.mPawYOffset));
		}
		if (mLeftPaw.mStreaksAlpha > 0f)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)Math.Min(mLeftPaw.mStreaksAlpha, mAlphaOverride));
			Image image3 = null;
			int num12;
			int num13;
			if (mLeftPaw.mState == 4)
			{
				num12 = Common._S(Common._M(11));
				num13 = Common._S(Common._M(74));
				image3 = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_STREAKS_DOWN);
			}
			else
			{
				num12 = Common._S(Common._M(3));
				num13 = Common._S(Common._M(80));
				image3 = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_STREAKS_UP);
			}
			g.DrawImage(image3, Common._S(num) + num12, Common._S(num2) + num13);
			g.SetColorizeImages(colorizeImages: false);
		}
		if (mRightPaw.mStreaksAlpha > 0f)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)Math.Min(mRightPaw.mStreaksAlpha, mAlphaOverride));
			Image image4 = null;
			int num14;
			int num15;
			if (mRightPaw.mState == 4)
			{
				num14 = Common._S(Common._M(119));
				num15 = Common._S(Common._M(75));
				image4 = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_STREAKS_DOWN);
			}
			else
			{
				num14 = Common._S(Common._M(130));
				num15 = Common._S(Common._M(80));
				image4 = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_STREAKS_UP);
			}
			g.DrawImageMirror(image4, Common._S(num) + num14, Common._S(num2) + num15);
			g.SetColorizeImages(colorizeImages: false);
		}
		if (mAlphaOverride < 255f)
		{
			g.SetColorizeImages(colorizeImages: false);
		}
	}

	protected override void BulletErased(int index)
	{
	}

	protected override Rect GetBulletRect(BossBullet b)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_SKULL_LEFT);
		int num = (int)((float)imageByID.mWidth * 0.75f);
		int num2 = (int)((float)imageByID.mHeight * 0.75f);
		return new Rect((int)(b.mX - (float)(num / 2)), (int)(b.mY - (float)(num2 / 2)), num, num2);
	}

	protected override void BulletHitPlayer(BossBullet b)
	{
		base.BulletHitPlayer(b);
		if (b.mData != null)
		{
			Skull skull = (Skull)b.mData;
			skull.mHitPlayer = true;
			skull.mChunks.SetPos(mLevel.mFrog.GetCenterX() + Common._M(0), mLevel.mFrog.GetCenterY() + Common._M1(0));
			skull.mClouds.SetPos(mLevel.mFrog.GetCenterX() + Common._M(0), mLevel.mFrog.GetCenterY() + Common._M1(0));
		}
		if (GameApp.gApp.GetLevelMgr().mBossesCanAttackFuckedFrog)
		{
			return;
		}
		for (int i = 0; i < mBullets.Count; i++)
		{
			BossBullet bossBullet = mBullets[i];
			if (bossBullet.mDelay > 0 || bossBullet.mState < 2)
			{
				bossBullet.mDeleteInstantly = true;
			}
		}
	}

	protected override void BossBulletDestroyed(BossBullet b, bool outofscreen)
	{
		base.BossBulletDestroyed(b, outofscreen);
		if (mLeftPaw.mBulletId == b.mId)
		{
			mLeftPaw = new BossFiringState();
		}
		if (mRightPaw.mBulletId == b.mId)
		{
			mRightPaw = new BossFiringState();
		}
		if (b.mData != null)
		{
			Skull skull = (Skull)b.mData;
			if (outofscreen)
			{
				FreeSkull(skull);
			}
			else
			{
				skull.mTrail.ForceStopEmitting(f: true);
				skull.mLeftEye.ForceStopEmitting(f: true);
				skull.mRightEye.ForceStopEmitting(f: true);
				skull.mUseLastPos = true;
				mSkulls.Add(skull);
			}
			b.mData = null;
		}
	}

	protected void SyncSkull(DataSync sync, Skull s)
	{
		SexyFramework.Misc.Buffer buffer = sync.GetBuffer();
		if (sync.isWrite())
		{
			bool flag = false;
			for (int i = 0; i < mBullets.Count; i++)
			{
				if (mBullets[i].mData != null && (Skull)mBullets[i].mData == s)
				{
					flag = true;
					buffer.WriteLong(i);
					break;
				}
			}
			if (!flag)
			{
				buffer.WriteLong(-1L);
			}
			buffer.WriteBoolean(s.mIsLeft);
			buffer.WriteLong(s.mImageNum);
			buffer.WriteLong(s.mLeftEyeHandle);
			buffer.WriteLong(s.mRightEyeHandle);
			buffer.WriteLong(s.mTrailPTHandle);
			buffer.WriteLong(s.mTrailHandle);
			buffer.WriteFloat(s.mLastX);
			buffer.WriteFloat(s.mLastY);
			buffer.WriteFloat(s.mLastAngle);
			buffer.WriteFloat(s.mAlpha);
			buffer.WriteBoolean(s.mLaunched);
			buffer.WriteBoolean(s.mUseLastPos);
			buffer.WriteBoolean(s.mHitPlayer);
			Common.SerializeParticleSystem(s.mLeftEye, sync);
			Common.SerializeParticleSystem(s.mRightEye, sync);
			Common.SerializeParticleSystem(s.mTrail, sync);
			Common.SerializeParticleSystem(s.mChunks, sync);
			Common.SerializeParticleSystem(s.mClouds, sync);
		}
		else
		{
			int num = (int)buffer.ReadLong();
			Skull skull = new Skull(buffer.ReadBoolean());
			skull.mImageNum = (int)buffer.ReadLong();
			skull.mImage = gFiringImages[skull.mImageNum];
			skull.mLeftEyeHandle = (int)buffer.ReadLong();
			skull.mRightEyeHandle = (int)buffer.ReadLong();
			skull.mTrailPTHandle = (int)buffer.ReadLong();
			skull.mTrailHandle = (int)buffer.ReadLong();
			skull.mLastX = buffer.ReadFloat();
			skull.mLastY = buffer.ReadFloat();
			skull.mLastAngle = buffer.ReadFloat();
			skull.mAlpha = buffer.ReadFloat();
			skull.mLaunched = buffer.ReadBoolean();
			skull.mUseLastPos = buffer.ReadBoolean();
			skull.mHitPlayer = buffer.ReadBoolean();
			if (num != -1)
			{
				mBullets[num].mData = skull;
			}
			else
			{
				mSkulls.Add(skull);
			}
			skull.mLeftEye = Common.DeserializeParticleSystem(sync);
			skull.mRightEye = Common.DeserializeParticleSystem(sync);
			skull.mTrail = Common.DeserializeParticleSystem(sync);
			skull.mFatHead = new SexyFramework.PIL.System(50, 50);
			skull.mFatHead.mScale = Common._S(1f);
			skull.mChunks = Common.DeserializeParticleSystem(sync);
			skull.mClouds = Common.DeserializeParticleSystem(sync);
		}
	}

	protected override bool CanTaunt()
	{
		return mTutorialState == 0;
	}

	public override bool Collides(Bullet b)
	{
		bool flag = base.Collides(b);
		if (mTutorialState == 2)
		{
			if (flag)
			{
				mCleanHeart = false;
				mTauntQueue.Clear();
				mTutorialState = 3;
			}
		}
		else if (mTutorialState == 5 && flag)
		{
			mTauntQueue.Clear();
			mTutorialState = 0;
			GameApp.gApp.GetBoard().mPreventBallAdvancement = false;
			TauntText tauntText = new TauntText();
			mTauntQueue.Add(tauntText);
			tauntText.mText = TextManager.getInstance().getString(399);
			tauntText.mDelay = Common._M(200);
			tauntText.mTextId = 399;
			tauntText = new TauntText();
			mTauntQueue.Add(tauntText);
			tauntText.mText = TextManager.getInstance().getString(400);
			tauntText.mDelay = Common._M(200);
			tauntText.mTextId = 400;
		}
		return flag;
	}

	protected override bool CanFire()
	{
		return mTutorialState == 0;
	}

	public override void DeleteAllBullets()
	{
		base.DeleteAllBullets();
		mLeftPaw = new BossFiringState();
		mRightPaw = new BossFiringState();
	}

	protected BossBullet GetBulletById(int id)
	{
		for (int i = 0; i < mBullets.Count; i++)
		{
			if (mBullets[i].mId == id)
			{
				return mBullets[i];
			}
		}
		return mBullets[0];
	}

	public BossTiger(Level l)
		: base(l)
	{
		mPawModifyingHeadAngle = 0;
		mStompVX = 0f;
		mStompVY = 0f;
		mStompAY = 0f;
		mStompRestingY = 0f;
		mStompPause = 0;
		mStompCount = 0;
		mDrawHeartsBelowBoss = true;
		mBossRadius = Common._M(100);
		mBulletRadius = Common._M(25);
		mBossRadiusYOff = Common._M(-30);
		mResGroup = "Boss1";
		mResPrefix = "IMAGE_BOSS_TIGER_";
		if (GameApp.gApp != null && (GameApp.gApp.IsHardMode() || (GameApp.gApp.mUserProfile != null && GameApp.gApp.mUserProfile.GetAdvModeVars().mNumTimesZoneBeat[0] > 0)))
		{
			mTutorialState = 0;
		}
		else
		{
			mTutorialState = 1;
		}
	}

	public BossTiger()
		: this(null)
	{
	}

	public override void Dispose()
	{
		base.Dispose();
		for (int i = 0; i < mBullets.Count; i++)
		{
			if (mBullets[i].mData != null)
			{
				((Skull)mBullets[i].mData).Dispose();
				mBullets[i].mData = null;
			}
		}
		for (int j = 0; j < mSkulls.Count; j++)
		{
			if (mSkulls[j] != null)
			{
				mSkulls[j].Dispose();
				mSkulls[j] = null;
			}
		}
		mSkulls.Clear();
		gTigerBulletImages[0] = null;
		gTigerBulletImages[1] = null;
		gTigerBulletImages[2] = null;
		gFiringImages[0] = null;
		gFiringImages[1] = null;
		gFiringImages[2] = null;
	}

	public bool ShouldEraseBullets()
	{
		if (mTutorialState <= 2 && mTutorialState > 0)
		{
			return mTauntQueue.Count <= 1;
		}
		return false;
	}

	public override bool AllowFrogToFire()
	{
		if (mTutorialState == 0)
		{
			return base.AllowFrogToFire();
		}
		if (mTutorialState == 2 && mTauntQueue.Count <= 1)
		{
			return true;
		}
		if (mTutorialState == 5 && GameApp.gApp.GetBoard().mPreventBallAdvancement && mLevel.AllCurvesAtRolloutPoint())
		{
			return true;
		}
		return false;
	}

	public override int GetFrogReloadType()
	{
		if (mTutorialState == 2)
		{
			return SexyFramework.Common.Rand() % 4;
		}
		return base.GetFrogReloadType();
	}

	public override void Update(float f)
	{
		GameApp gApp = GameApp.gApp;
		if (gApp.GetBoard().IsAboutToDoCheckpointEffect())
		{
			return;
		}
		PAW_LEFT_X_OFF = Common._M(-4);
		PAW_RIGHT_X_OFF = Common._M(151);
		SKULL_LEFT_X_OFF = Common._M(2);
		SKULL_LEFT_Y_OFF = Common._M(72);
		SKULL_RIGHT_X_OFF = Common._M(154);
		SKULL_RIGHT_Y_OFF = Common._M(73);
		if (SexyFramework.Common._geq(mAlphaOverride, 255f))
		{
			for (int i = 0; i < mBullets.Count; i++)
			{
				BossBullet bossBullet = mBullets[i];
				if (bossBullet.mData != null)
				{
					((Skull)bossBullet.mData).Update(bossBullet.mX, bossBullet.mY, bossBullet.mAngle);
				}
			}
			for (int j = 0; j < mSkulls.Count; j++)
			{
				Skull skull = mSkulls[j];
				skull.Update(0f, 0f, 0f);
				if (skull.mLeftEye.Done() && skull.mRightEye.Done() && skull.mTrail.Done() && (!skull.mHitPlayer || (skull.mFatHead.Done() && skull.mChunks.Done() && skull.mClouds.Done())))
				{
					FreeSkull(skull);
					mSkulls.RemoveAt(j);
					j--;
				}
			}
		}
		if (mDoDeathExplosions || mHP <= 0f || mLevel.mBoard.DoingBossIntro())
		{
			base.Update(f);
			return;
		}
		if (mTutorialState == 1)
		{
			mAlphaOverride = 255f;
			if (mStompPause == 0)
			{
				mX += mStompVX;
				mY += mStompVY;
				mStompVY += mStompAY;
				if (mY >= mStompRestingY && mStompVY > 0f)
				{
					mStompCount++;
					gApp.GetBoard().ShakeScreen(Common._M(50), Common._M1(2), Common._M2(2));
					gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_TIGER_BOSS_SMASH));
					if (mStompCount == 1)
					{
						mY = mStompRestingY;
						mStompPause = Common._M(25);
						float num = Common._M(50);
						mStompVX = ((float)(Common._SS(gApp.mWidth) / 2) - mX) / num;
						mStompVY = Common._M(-2f);
						mStompAY = ((float)Common._M(150) - mStompVY * num) / (num * num);
					}
					else
					{
						mTutorialState = 2;
						TauntText tauntText = new TauntText();
						mTauntQueue.Add(tauntText);
						tauntText.mText = TextManager.getInstance().getString(401);
						tauntText.mDelay = Common._M(300);
						tauntText.mTextId = 401;
						tauntText = new TauntText();
						mTauntQueue.Add(tauntText);
						tauntText.mText = TextManager.getInstance().getString(402);
						tauntText.mDelay = Common._M(500);
						tauntText.mTextId = 402;
					}
				}
			}
			else
			{
				mStompPause--;
			}
		}
		else if (mTutorialState == 3)
		{
			if (GameApp.USE_TRIAL_VERSION)
			{
				if (GameApp.gApp.mBoard != null)
				{
					GameApp.gApp.mBoard.Pause(pause: true, becauseOfDialog: true);
				}
				string message = TextManager.getInstance().getString(833);
				int width_pad = Common._DS(Common._M(20));
				GameApp.gApp.DoYesNoDialog(TextManager.getInstance().getString(448), message, block: true, TextManager.getInstance().getString(446), TextManager.getInstance().getString(447), drag: false, Common._S(Common._M(50)), 1, width_pad);
				GameApp.gApp.mYesNoDialogDelegate = ProcessYesNo;
				return;
			}
			float num2 = Common._M(-120);
			if (mY > num2)
			{
				mY -= Common._M(12f);
				if (mY <= num2)
				{
					mY = num2;
					gApp.GetBoard().ShakeScreen(Common._M(100), Common._M1(3), Common._M2(3));
					mStompCount = 0;
					mTutorialState = 4;
					mStompPause = Common._M(150);
				}
			}
		}
		else if (mTutorialState == 4)
		{
			if (mStompPause > 0)
			{
				if (--mStompPause == 0 && mStompCount == 0)
				{
					mTauntQueue.Clear();
					TauntText tauntText2 = new TauntText();
					mTauntQueue.Add(tauntText2);
					tauntText2.mText = TextManager.getInstance().getString(403);
					tauntText2.mDelay = Common._M(300);
					tauntText2.mTextId = 403;
				}
			}
			else
			{
				mY += Common._M(10f);
				if (mStompCount == 0 && mY >= mStompRestingY / Common._M(1.8f))
				{
					gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_TIGER_BOSS_SMASH));
					mY = mStompRestingY / Common._M(1.8f);
					mStompPause = Common._M(25);
					gApp.GetBoard().ShakeScreen(Common._M(50), Common._M1(3), Common._M2(3));
					mStompCount++;
				}
				else if (mStompCount == 1 && mY >= mStompRestingY)
				{
					gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_TIGER_BOSS_SMASH));
					gApp.GetBoard().ShakeScreen(Common._M(30), Common._M1(3), Common._M2(3));
					mY = mStompRestingY;
					TauntText tauntText3 = new TauntText();
					mTauntQueue.Add(tauntText3);
					tauntText3.mText = TextManager.getInstance().getString(404);
					tauntText3.mDelay = Common._M(400);
					tauntText3.mTextId = 404;
					mTutorialState = 5;
				}
			}
		}
		else if (mTutorialState == 5 && mTauntQueue.Count <= 1 && !mLevel.AllCurvesAtRolloutPoint())
		{
			gApp.GetBoard().mPreventBallAdvancement = false;
		}
		if (mLevel.AllCurvesAtRolloutPoint())
		{
			if (mTutorialState == 5)
			{
				if (!gApp.GetBoard().mPreventBallAdvancement)
				{
					gApp.GetBoard().mPreventBallAdvancement = true;
					TauntText tauntText4 = new TauntText();
					mTauntQueue.Add(tauntText4);
					tauntText4.mText = TextManager.getInstance().getString(405);
					tauntText4.mDelay = Common._M(1000);
					tauntText4.mTextId = 405;
				}
			}
			else if (SexyFramework.Common._geq(mAlphaOverride, 255f))
			{
				BossFiringState[] array = new BossFiringState[2] { mLeftPaw, mRightPaw };
				int[] array2 = new int[2] { -1, 1 };
				for (int k = 0; k < 2; k++)
				{
					if (array[k].mState == 1)
					{
						array[k].mPawYOffset -= Common._M(3f);
						array[k].mSkullYOffset -= Common._M(3f);
						float num3 = Common._M(-15f);
						if (array[k].mPawYOffset < num3)
						{
							array[k].mSkullYOffset = num3;
							array[k].mPawYOffset = num3;
							array[k].mState++;
						}
						if (mPawModifyingHeadAngle == array2[k])
						{
							array[k].mHeadAngle += (float)array2[k] * Common._M(0.005f);
						}
					}
					else if (array[k].mState == 2)
					{
						if (++array[k].mTimer >= Common._M(5))
						{
							array[k].mTimer = 0;
							array[k].mState++;
						}
						if (mPawModifyingHeadAngle == array2[k])
						{
							array[k].mHeadAngle += (float)array2[k] * Common._M(0.005f);
						}
					}
					else if (array[k].mState == 3)
					{
						array[k].mSkullYOffset += Common._M(2f);
						if ((array[k].mPawYOffset += Common._M(2f)) >= 0f)
						{
							array[k].mPawYOffset = 0f;
							array[k].mSkullYOffset = 0f;
							array[k].mState++;
						}
						if (mPawModifyingHeadAngle == array2[k])
						{
							array[k].mHeadAngle -= (float)array2[k] * Common._M(0.01f);
						}
					}
					else if (array[k].mState == 4)
					{
						int num4 = Common._M(15);
						float num5 = 255f / (float)(num4 + Common._M(4));
						if (array[k].mTimer == 0)
						{
							if (mPawModifyingHeadAngle == array2[k])
							{
								array[k].mHeadAngle -= (float)array2[k] * Common._M(0.01f);
							}
							BossBullet bossBullet2 = null;
							for (int l = 0; l < mBullets.Count; l++)
							{
								if (mBullets[l].mId == array[k].mBulletId)
								{
									bossBullet2 = mBullets[l];
									break;
								}
							}
							array[k].mSkullYOffset += Common._M(7.5f);
							array[k].mSkullXOffset += bossBullet2.mVX;
							array[k].mSkullAngle += array[k].mSkullAngleInc;
							if (mUpdateCount % Common._M(4) == 0)
							{
								if (array[k].mSwipeFrame == 1)
								{
									array[k].mStreaksAlpha = 255f;
								}
								if (++array[k].mSwipeFrame >= Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_SWIPE).mNumCols)
								{
									array[k].mSwipeFrame--;
									Image image = gTigerBulletImages[bossBullet2.mImageNum];
									int num6 = Common._SS(image.mWidth);
									int num7 = Common._SS(image.mHeight);
									bossBullet2.mState++;
									if (bossBullet2.mData != null)
									{
										((Skull)bossBullet2.mData).mLaunched = true;
									}
									bossBullet2.mAngle = array[k].mSkullAngle;
									bossBullet2.mX = mX - (float)(mWidth / 2) + (float)(num6 / 2) + (float)((bossBullet2.mImageNum == 2) ? SKULL_RIGHT_X_OFF : SKULL_LEFT_X_OFF) + array[k].mSkullXOffset;
									bossBullet2.mY = mY - (float)(mHeight / 2) + (float)(num7 / 2) + (float)((bossBullet2.mImageNum == 2) ? SKULL_RIGHT_Y_OFF : SKULL_LEFT_Y_OFF) + array[k].mSkullYOffset;
									array[k].mBulletId = mBullets[0].mId;
									array[k].mSkullYOffset = (array[k].mSkullXOffset = 0f);
									array[k].mSkullAngle = (array[k].mTargetSkullAngle = (array[k].mSkullAngleInc = 0f));
									array[k].mSkullGrowPct = 0f;
									array[k].mTimer = num4;
								}
							}
						}
						else if (--array[k].mTimer == 0)
						{
							array[k].mState++;
						}
						if (array[k].mSwipeFrame == 2 && (array[k].mStreaksAlpha -= num5) < 0f)
						{
							array[k].mStreaksAlpha = 0f;
						}
					}
					else if (array[k].mState == 5)
					{
						int num8 = Common._M(5);
						float num9 = 255f / (float)(num8 + Common._M(5));
						if (array[k].mTimer > 0)
						{
							array[k].mTimer--;
						}
						else if (mUpdateCount % Common._M(5) == 0)
						{
							if (--array[k].mSwipeFrame < 0)
							{
								array[k].mState++;
							}
							else if (array[k].mSwipeFrame == 0)
							{
								array[k].mTimer = num8;
								array[k].mStreaksAlpha = 255f;
							}
						}
						if (array[k].mSwipeFrame == 0 && (array[k].mStreaksAlpha -= num9) < 0f)
						{
							array[k].mStreaksAlpha = 0f;
						}
						if (array[k].mTimer == 0 && mPawModifyingHeadAngle == array2[k] && array[k].mSwipeFrame < 2)
						{
							array[k].mHeadAngle += (float)array2[k] * Common._M(0.04f);
							if ((array2[k] > 0 && array[k].mHeadAngle >= 0f) || (array2[k] < 0 && array[k].mHeadAngle <= 0f))
							{
								array[k].mHeadAngle = 0f;
							}
						}
					}
					else if (array[k].mState == 6)
					{
						array[k].mSkullGrowPct += Common._M(0.075f);
						if (array[k].mSkullGrowPct >= Common._M(1.1f))
						{
							array[k].mState++;
						}
					}
					else
					{
						if (array[k].mState != 7)
						{
							continue;
						}
						array[k].mSkullGrowPct -= Common._M(0.008f);
						if (array[k].mSkullGrowPct <= 1f)
						{
							array[k].mSkullGrowPct = 1f;
							array[k].mState = 0;
							if (mPawModifyingHeadAngle == array2[k])
							{
								mPawModifyingHeadAngle = 0;
							}
						}
					}
				}
			}
		}
		base.Update(f);
	}

	public override void SyncState(DataSync sync)
	{
		base.SyncState(sync);
		sync.SyncLong(ref mPawModifyingHeadAngle);
		BossFiringState[] array = new BossFiringState[2] { mLeftPaw, mRightPaw };
		for (int i = 0; i < 2; i++)
		{
			sync.SyncLong(ref array[i].mState);
			sync.SyncFloat(ref array[i].mPawYOffset);
			sync.SyncFloat(ref array[i].mSkullXOffset);
			sync.SyncFloat(ref array[i].mSkullYOffset);
			sync.SyncFloat(ref array[i].mSkullAngle);
			sync.SyncFloat(ref array[i].mHeadAngle);
			sync.SyncFloat(ref array[i].mSkullGrowPct);
			sync.SyncFloat(ref array[i].mTargetSkullAngle);
			sync.SyncFloat(ref array[i].mSkullAngleInc);
			sync.SyncFloat(ref array[i].mStreaksAlpha);
			sync.SyncLong(ref array[i].mSwipeFrame);
			sync.SyncLong(ref array[i].mTimer);
			sync.SyncLong(ref array[i].mBulletId);
		}
		sync.SyncLong(ref mTutorialState);
		sync.SyncFloat(ref mStompVX);
		sync.SyncFloat(ref mStompVY);
		sync.SyncFloat(ref mStompRestingY);
		sync.SyncFloat(ref mStompAY);
		sync.SyncLong(ref mStompPause);
		sync.SyncLong(ref mStompCount);
		SexyFramework.Misc.Buffer buffer = sync.GetBuffer();
		if (sync.isWrite())
		{
			int num = 0;
			for (int j = 0; j < mBullets.Count; j++)
			{
				if (mBullets[j].mData != null)
				{
					num++;
				}
			}
			buffer.WriteLong(num);
			for (int k = 0; k < mBullets.Count; k++)
			{
				if (mBullets[k].mData != null)
				{
					SyncSkull(sync, (Skull)mBullets[k].mData);
				}
			}
			buffer.WriteLong(mSkulls.Count);
			for (int l = 0; l < mSkulls.Count; l++)
			{
				SyncSkull(sync, mSkulls[l]);
			}
		}
		else
		{
			int num2 = (int)buffer.ReadLong();
			for (int m = 0; m < num2; m++)
			{
				SyncSkull(sync, null);
			}
			int num3 = (int)buffer.ReadLong();
			for (int n = 0; n < num3; n++)
			{
				SyncSkull(sync, null);
			}
		}
		if (mTutorialState == 2)
		{
			mTauntQueue.Clear();
			TauntText tauntText = new TauntText();
			mTauntQueue.Add(tauntText);
			tauntText.mText = TextManager.getInstance().getString(406);
			tauntText.mDelay = Common._M(300);
			tauntText.mTextId = 406;
			tauntText = new TauntText();
			mTauntQueue.Add(tauntText);
			tauntText.mText = TextManager.getInstance().getString(407);
			tauntText.mDelay = Common._M(300);
			tauntText.mTextId = 407;
			tauntText = new TauntText();
			mTauntQueue.Add(tauntText);
			tauntText.mText = TextManager.getInstance().getString(408);
			tauntText.mDelay = Common._M(500);
			tauntText.mTextId = 408;
		}
	}

	public override void Init(Level l)
	{
		mWidth = 197;
		mHeight = 134;
		GameApp gApp = GameApp.gApp;
		base.Init(l);
		mBandagedImg = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_BANDAGED);
		gTigerBulletImages[0] = null;
		gTigerBulletImages[1] = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_SKULL_LEFT);
		gTigerBulletImages[2] = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_SKULL_RIGHT);
		gFiringImages[0] = null;
		gFiringImages[1] = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_SKULL_LEFT_PARTICLE);
		gFiringImages[2] = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_SKULL_RIGHT_PARTICLE);
		if (mTutorialState != 0)
		{
			gApp.GetBoard().mPreventBallAdvancement = true;
			if (mTutorialState == 1)
			{
				mStompRestingY = mY;
				mX = Common._M(0);
				mY = Common._M(-150);
				float num = Common._SS(gApp.mWidth) / 2 - Common._M(150);
				float num2 = Common._M(40);
				mStompVX = (num - mX) / num2;
				mStompVY = (mStompRestingY - mY) / num2;
				mStompAY = 0f;
			}
			mTauntQueue.Clear();
		}
	}

	public override Boss Instantiate()
	{
		BossTiger bossTiger = new BossTiger(mLevel);
		bossTiger.CopyFrom(this);
		if (GameApp.gApp.IsHardMode() || (GameApp.gApp.mUserProfile != null && GameApp.gApp.mUserProfile.GetAdvModeVars().mNumTimesZoneBeat[0] > 0))
		{
			bossTiger.mTutorialState = 0;
		}
		else
		{
			bossTiger.mTutorialState = 1;
		}
		bossTiger.mSkulls.Clear();
		return bossTiger;
	}

	protected void CopyFrom(BossTiger rhs)
	{
		CopyFrom((BossShoot)rhs);
		mPawModifyingHeadAngle = rhs.mPawModifyingHeadAngle;
		mTutorialState = rhs.mTutorialState;
		mStompVX = rhs.mStompVX;
		mStompVY = rhs.mStompVY;
		mStompAY = rhs.mStompAY;
		mStompRestingY = rhs.mStompRestingY;
		mStompPause = rhs.mStompPause;
		mStompCount = rhs.mStompCount;
		mSkulls.Clear();
		mSkulls.AddRange(rhs.mSkulls.ToArray());
		mLeftPaw = new BossFiringState(rhs.mLeftPaw);
		mRightPaw = new BossFiringState(rhs.mRightPaw);
	}

	public void ProcessYesNo(int theId)
	{
		switch (theId)
		{
		case 1000:
			GameApp.gApp.ToMarketPlace();
			break;
		case 1001:
			GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.DoDeferredEndGame;
			GameApp.gApp.ToggleBambooTransition();
			GameApp.gApp.mMusic.StopAll();
			break;
		}
	}
}
