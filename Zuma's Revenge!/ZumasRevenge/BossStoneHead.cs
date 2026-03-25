using System;
using System.Collections.Generic;
using SexyFramework;
using SexyFramework.AELib;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class BossStoneHead : BossShoot
{
	protected static float MAX_STONE_HEAD_STRETCH = 1.001f;

	protected static int LEFT_EYE_XOFF = -16;

	protected static int RIGHT_EYE_XOFF = 30;

	protected static int EYE_YOFF = -35;

	protected int mShakeTime = 300;

	protected float mEyeFlameAlpha;

	protected bool mBlink;

	protected bool mBlinkClosed = true;

	protected bool mFiring;

	protected bool mDoingExplodeAnim;

	protected float mTextAlpha;

	protected bool mShowText;

	protected int mEyeFrame;

	protected int mHitTimer;

	protected bool mLeftInUse;

	protected bool mRightInUse;

	protected EyeAnim mLeftEye = new EyeAnim();

	protected EyeAnim mRightEye = new EyeAnim();

	protected List<Steam> mSteam = new List<Steam>();

	protected List<RockChunk> mRocks = new List<RockChunk>();

	protected Composition mExplodeComp;

	protected BossVolcano mVolcanoBoss;

	private Image IMAGE_BOSS_STONEHEAD_FACES;

	private Image IMAGE_BOSS_STONEHEAD_EYES;

	private Image IMAGE_BOSS_STONEHEAD_ROCKS;

	public float mStretchPct = 1f;

	protected override void DrawBossSpecificArt(Graphics g)
	{
		if (mStretchPct >= MAX_STONE_HEAD_STRETCH)
		{
			if (mExplodeComp.GetUpdateCount() < Common._M(150))
			{
				int num = -(mApp.mWidth / 2 - Common._S(GetX())) + Common._S(Common._M(-193)) + mApp.mBoardOffsetX;
				int num2 = -(mApp.mHeight / 2 - Common._S(GetY())) + Common._S(Common._M(-91));
				CumulativeTransform cumulativeTransform = new CumulativeTransform();
				cumulativeTransform.mTrans.Translate(num, num2);
				int frame = ((mExplodeComp.mUpdateCount >= mExplodeComp.GetMaxDuration()) ? (mExplodeComp.GetMaxDuration() - 1) : (-1));
				mExplodeComp.Draw(g, cumulativeTransform, frame, Common._DS(1f));
			}
			else
			{
				mVolcanoBoss.Draw(g);
			}
		}
		float value = mX - (float)mWidth * mStretchPct / 2f + (float)mShakeXOff;
		float value2 = mY - (float)mHeight * mStretchPct / 2f + (float)mShakeYOff;
		for (int i = 0; i < mSteam.Count; i++)
		{
			Steam steam = mSteam[i];
			int num3 = (int)Math.Min(steam.mAlpha, mAlphaOverride);
			if (num3 != 255)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, num3);
			}
			if (!g.Is3D())
			{
				g.DrawImage(steam.mImage, (int)Common._S(mX + steam.mXOff + (float)Common._M(0)), (int)Common._S(mY + steam.mYOff + (float)Common._M1(0)), (int)(steam.mSize * (float)steam.mImage.mWidth), (int)(steam.mSize * (float)steam.mImage.mHeight));
			}
			else
			{
				mGlobalTranform.Reset();
				mGlobalTranform.Scale(steam.mSize, steam.mSize);
				mGlobalTranform.RotateRad(steam.mAngle);
				if (g.Is3D())
				{
					g.DrawImageTransformF(steam.mImage, mGlobalTranform, Common._S(mX + steam.mXOff + (float)Common._M(0)), Common._S(mY + (float)Common._M1(0) + steam.mYOff));
				}
				else
				{
					g.DrawImageTransform(steam.mImage, mGlobalTranform, Common._S(mX + steam.mXOff + (float)Common._M(0)), Common._S(mY + (float)Common._M1(0) + steam.mYOff));
				}
			}
			g.SetColorizeImages(colorizeImages: false);
		}
		if (mStretchPct < MAX_STONE_HEAD_STRETCH)
		{
			int theCel = 0;
			if (mHP <= 0f)
			{
				theCel = 1;
			}
			else if (mHitTimer > Common._M(194))
			{
				theCel = 1;
			}
			else if (mHitTimer > 0)
			{
				theCel = 2;
			}
			if (IMAGE_BOSS_STONEHEAD_FACES != null)
			{
				if (mHP <= 0f)
				{
					g.DrawImage(theDestRect: new Rect((int)Common._S(value), (int)Common._S(value2), (int)((float)IMAGE_BOSS_STONEHEAD_FACES.GetCelWidth() * mStretchPct), (int)((float)IMAGE_BOSS_STONEHEAD_FACES.GetCelHeight() * mStretchPct)), theImage: IMAGE_BOSS_STONEHEAD_FACES, theSrcRect: IMAGE_BOSS_STONEHEAD_FACES.GetCelRect(theCel));
				}
				else
				{
					g.PushState();
					if (!SexyFramework.Common._geq(mAlphaOverride, 255f))
					{
						g.SetColorizeImages(colorizeImages: true);
						g.SetColor(255, 255, 255, (int)mAlphaOverride);
					}
					g.DrawImageCel(IMAGE_BOSS_STONEHEAD_FACES, (int)Common._S(value), (int)Common._S(value2), theCel);
					g.PopState();
				}
			}
		}
		g.PushState();
		if (!SexyFramework.Common._geq(mAlphaOverride, 255f))
		{
			g.SetColor(255, 255, 255, (int)mAlphaOverride);
			g.SetColorizeImages(colorizeImages: true);
		}
		if (mHitTimer == 0 && !mDoingExplodeAnim && SexyFramework.Common._geq(mAlphaOverride, 255f))
		{
			int num4 = Common._M(-1);
			int num5 = Common._M(0);
			if (IMAGE_BOSS_STONEHEAD_EYES != null)
			{
				g.DrawImageCel(IMAGE_BOSS_STONEHEAD_EYES, (int)(Common._S(value) + Common._DSA(50f, num4)), (int)(Common._S(value2) + Common._DSA(58f, num5)), mEyeFrame);
			}
		}
		if (!mDoingExplodeAnim)
		{
			g.PushState();
			mLeftEye.Draw(g);
			g.PopState();
			g.PushState();
			mRightEye.Draw(g);
			g.PopState();
		}
		for (int j = 0; j < mRocks.Count; j++)
		{
			RockChunk rockChunk = mRocks[j];
			if (rockChunk.mAlpha != 255f)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)rockChunk.mAlpha);
			}
			if (IMAGE_BOSS_STONEHEAD_ROCKS != null)
			{
				g.DrawImage(IMAGE_BOSS_STONEHEAD_ROCKS, new Rect((int)Common._S(rockChunk.mX), (int)Common._S(rockChunk.mY), (int)((float)IMAGE_BOSS_STONEHEAD_ROCKS.GetCelWidth() * Common._M(0.5f)), (int)((float)IMAGE_BOSS_STONEHEAD_ROCKS.GetCelHeight() * Common._M1(0.5f))), IMAGE_BOSS_STONEHEAD_ROCKS.GetCelRect(rockChunk.mCol));
			}
			g.SetColorizeImages(colorizeImages: false);
		}
		if (mTeleportDir != 0)
		{
			g.PushState();
			g.ClearClipRect();
		}
		if (!mDoingExplodeAnim && !mLevel.mBoard.IsPaused())
		{
			for (int k = 0; k < mBullets.Count; k++)
			{
				BossBullet bossBullet = mBullets[k];
				if (bossBullet.mDelay <= 0 && bossBullet.mState != 0)
				{
					EyeBullet eyeBullet = bossBullet.mData as EyeBullet;
					eyeBullet.Draw(g, (int)mAlphaOverride);
				}
			}
		}
		else if (mTextAlpha > 0f && mShowText)
		{
			g.SetFont(Res.GetFontByID(ResID.FONT_BOSS_TAUNT));
			g.SetColor(0, 0, 0, (int)Math.Min(mTextAlpha, 255f));
			float mTransX = g.mTransX;
			g.mTransX = 0f;
			if (!mLevel.mBoard.IsHardAdventureMode())
			{
				g.WriteString(TextManager.getInstance().getString(393), 0, Common._DS(Common._M(530)), mApp.mWidth, 0);
				g.WriteString(TextManager.getInstance().getString(394), 0, Common._DS(Common._M(630)), mApp.mWidth, 0);
				g.WriteString(TextManager.getInstance().getString(395), 0, Common._DS(Common._M(730)), mApp.mWidth, 0);
			}
			else
			{
				g.WriteString(TextManager.getInstance().getString(396), 0, Common._DS(Common._M(530)), mApp.mWidth, 0);
				g.WriteString(TextManager.getInstance().getString(397), 0, Common._DS(Common._M(630)), mApp.mWidth, 0);
				g.WriteString(TextManager.getInstance().getString(398), 0, Common._DS(Common._M(730)), mApp.mWidth, 0);
			}
			g.mTransX = mTransX;
		}
		if (mTeleportDir != 0)
		{
			g.PopState();
		}
		g.PopState();
	}

	protected override bool DoHit(Bullet b, bool from_prox_bomb)
	{
		if (mDoingExplodeAnim)
		{
			return false;
		}
		mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BOSS_STONE_HIT));
		mHitTimer = Common._M(200);
		int num = Common._M(6);
		int num2 = (int)(mX - (float)(mWidth / 2));
		int num3 = (int)(mY - (float)(mHeight / 2));
		int num4 = num3 - Common._M(0);
		int num5 = num3 + Common._M(150);
		int num6 = (int)((float)(num5 - num4) / ((float)num / 2f));
		int num7 = num2 - Common._M(10);
		int num8 = Common._M(100);
		for (int i = 0; i < num; i++)
		{
			RockChunk rockChunk = new RockChunk();
			mRocks.Add(rockChunk);
			rockChunk.mCol = SexyFramework.Common.Rand() % IMAGE_BOSS_STONEHEAD_ROCKS.mNumCols;
			rockChunk.mAlpha = 255f;
			rockChunk.mVX = 0f;
			rockChunk.mVY = SexyFramework.Common.FloatRange(Common._M(3f), Common._M1(4f));
			rockChunk.mY = num4 + i / 2 * num6;
			rockChunk.mX = num7 + ((i % 2 == 0) ? num8 : 0);
		}
		bool flag = base.DoHit(b, from_prox_bomb);
		if (flag && SexyFramework.Common._leq(mHP, 50f))
		{
			mVolcanoBoss = mLevel.mSecondaryBoss as BossVolcano;
			mVolcanoBoss.mIntro = true;
			mVolcanoBoss.SetXY(GetX(), GetY());
			mApp.mBoard.mDrawBossUI = false;
			mApp.mBoard.mMenuButton.SetVisible(isVisible: false);
			SoundAttribs soundAttribs = new SoundAttribs();
			soundAttribs.delay = 130;
			mApp.mSoundPlayer.Play(Res.GetSoundByID(ResID.SOUND_BOSS_STONE_TRANSFORM), soundAttribs);
			mApp.GetBoard().mPreventBallAdvancement = true;
			mPauseMovement = true;
			mDoingExplodeAnim = true;
			for (int j = 0; j < mBullets.Count; j++)
			{
				mBullets[j].mDeleteInstantly = true;
			}
			mTextAlpha = 0f;
			for (int k = 0; k < mHulaDancers.Count; k++)
			{
				mHulaDancers[k].mFadeOut = true;
			}
		}
		return flag;
	}

	protected override void DidFire()
	{
		base.DidFire();
		mFiring = true;
		if (mLeftEye.mEyeFlame.mCurNumParticles == 0)
		{
			mLeftEye.mEyeFlame.ResetAnim();
		}
		if (mRightEye.mEyeFlame.mCurNumParticles == 0)
		{
			mRightEye.mEyeFlame.ResetAnim();
		}
		mLeftEye.mFiring = (mRightEye.mFiring = true);
		mLeftEye.mEyeFlame.mEmitAfterTimeline = (mRightEye.mEyeFlame.mEmitAfterTimeline = true);
	}

	protected override bool PreBulletUpdate(BossBullet b, int index)
	{
		if (b.mState == 0)
		{
			return true;
		}
		if (b.mDelay > 0)
		{
			b.mDelay--;
			return true;
		}
		if (b.mData != null && ((EyeBullet)b.mData).Update((int)b.mX, (int)b.mY, b.mBouncesLeft <= 0))
		{
			b.mDeleteInstantly = true;
		}
		return false;
	}

	protected override BossBullet CreateBossBullet()
	{
		BossBullet bossBullet = new BossBullet();
		EyeBullet eyeBullet = (EyeBullet)(bossBullet.mData = new EyeBullet());
		eyeBullet.mExplosion = mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_STONEBOSSPROJEXPLOSION").Duplicate();
		eyeBullet.mProjectile = mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_STONEBOSSPROJ").Duplicate();
		eyeBullet.mProjectile.mEmitAfterTimeline = true;
		eyeBullet.mSparks = mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_STONEBOSSPROJSPARKS").Duplicate();
		return bossBullet;
	}

	protected override void BossBulletDestroyed(BossBullet b, bool outofscreen)
	{
		if (b.mData != null)
		{
			EyeBullet eyeBullet = (EyeBullet)b.mData;
			mApp.ReleaseGenericCachedEffect(eyeBullet.mSparks);
			mApp.ReleaseGenericCachedEffect(eyeBullet.mProjectile);
			mApp.ReleaseGenericCachedEffect(eyeBullet.mExplosion);
			b.mData = null;
		}
	}

	protected override Rect GetBulletRect(BossBullet b)
	{
		if (b.mData == null)
		{
			return new Rect(0, 0, 0, 0);
		}
		EyeBullet eyeBullet = (EyeBullet)b.mData;
		return new Rect((int)(b.mX + (float)eyeBullet.mXOff), (int)(b.mY + (float)eyeBullet.mYOff), Common._DS(Common._M(14)), Common._DS(Common._M1(20)));
	}

	protected override void BulletHitPlayer(BossBullet b)
	{
		SoundAttribs soundAttribs = new SoundAttribs();
		soundAttribs.fadeout = 0.1f;
		mApp.mSoundPlayer.Loop(Res.GetSoundByID(ResID.SOUND_NEW_BURNINGFROGLOOP), soundAttribs);
		mApp.mSoundPlayer.Play(Res.GetSoundByID(ResID.SOUND_NEW_FIREHITFROG));
		if (mApp.GetLevelMgr().mBossesCanAttackFuckedFrog)
		{
			return;
		}
		mLevel.mFrog.SetSlowTimer(0);
		mLevel.mBoard.SetHallucinateTimer(0);
		for (int i = 0; i < mBullets.Count; i++)
		{
			BossBullet bossBullet = mBullets[i];
			if (bossBullet.mDelay > 0 || bossBullet.mState == 0)
			{
				bossBullet.mDeleteInstantly = true;
			}
		}
	}

	protected override void GetShotBounceOffs(BossBullet b, ref int x, ref int y)
	{
		x = (y = 0);
		if (b.mData != null)
		{
			EyeBullet eyeBullet = (EyeBullet)b.mData;
			x += eyeBullet.mXOff + Common._DS(Common._M(0));
			y += eyeBullet.mYOff + Common._DS(Common._M(0));
		}
	}

	protected override bool CanFire()
	{
		return !mDoingExplodeAnim;
	}

	protected override bool CanSpawnHulaDancers()
	{
		return !mDoingExplodeAnim;
	}

	protected override void ShotBounced(BossBullet b)
	{
		EyeBullet eyeBullet = (EyeBullet)b.mData;
		mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BOSS_STONE_EYE_LASER_BOUNCE));
		if (b.mBouncesLeft == 0)
		{
			eyeBullet.mExplosion.ResetAnim();
			eyeBullet.mProjectile.mEmitAfterTimeline = false;
		}
		else if (eyeBullet.mSparks.mCurNumParticles == 0)
		{
			eyeBullet.mSparkFirstFrame = true;
			eyeBullet.mSparks.ResetAnim();
		}
	}

	protected override void BerserkActivated(int health_limit)
	{
		base.BerserkActivated(health_limit);
		mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BOSS_STONE_BERSERK));
	}

	protected override bool CanTaunt()
	{
		if (SexyFramework.Common._leq(mHP, 50f))
		{
			return false;
		}
		return base.CanTaunt();
	}

	public BossStoneHead(Level l)
		: base(l)
	{
		mShouldDoDeathExplosions = false;
		mBossRadius = Common._M(70);
		mBulletRadius = Common._M(5);
		mResGroup = "Boss6Common";
		mDrawDeathBGTikis = false;
	}

	public BossStoneHead()
		: this(null)
	{
	}

	public override void Dispose()
	{
		if (mExplodeComp != null)
		{
			mExplodeComp.Dispose();
			mExplodeComp = null;
		}
		for (int i = 0; i < mBullets.Count; i++)
		{
			if (mBullets[i].mData != null)
			{
				EyeBullet eyeBullet = (EyeBullet)mBullets[i].mData;
				mApp.ReleaseGenericCachedEffect(eyeBullet.mSparks);
				mApp.ReleaseGenericCachedEffect(eyeBullet.mProjectile);
				mApp.ReleaseGenericCachedEffect(eyeBullet.mExplosion);
				eyeBullet = null;
			}
		}
		mApp.ReleaseGenericCachedEffect(mLeftEye.mEyeFlame);
		mApp.ReleaseGenericCachedEffect(mRightEye.mEyeFlame);
	}

	public void CopyForm(BossStoneHead rhs)
	{
		CopyFrom(rhs);
		mShakeTime = rhs.mShakeTime;
		mEyeFlameAlpha = rhs.mEyeFlameAlpha;
		mBlink = rhs.mBlink;
		mBlinkClosed = rhs.mBlinkClosed;
		mFiring = rhs.mFiring;
		mDoingExplodeAnim = rhs.mDoingExplodeAnim;
		mTextAlpha = rhs.mTextAlpha;
		mShowText = rhs.mShowText;
		mEyeFrame = rhs.mEyeFrame;
		mHitTimer = rhs.mHitTimer;
		mLeftInUse = rhs.mLeftInUse;
		mRightInUse = rhs.mRightInUse;
		mExplodeComp = rhs.mExplodeComp;
		mVolcanoBoss = rhs.mVolcanoBoss;
		mLeftEye = new EyeAnim(rhs.mLeftEye);
		mRightEye = new EyeAnim(rhs.mRightEye);
	}

	public override void Update(float f)
	{
		base.Update(f);
		if (mHitTimer > 0 && SexyFramework.Common._geq(mAlphaOverride, 255f))
		{
			mHitTimer--;
			if (mUpdateCount % Common._M(2) == 0)
			{
				Steam steam = new Steam();
				mSteam.Add(steam);
				steam.mAlphaDec = Common._M(4f);
				steam.mAngleInc = SexyFramework.Common.FloatRange(Common._M(0.01f), Common._M1(0.1f));
				steam.mVX = Common._M(-2f);
				steam.mVY = Common._M(-0.02f);
				steam.mImgNum = SexyFramework.Common.Rand() % 2;
				steam.mImage = ((steam.mImgNum == 0) ? Res.GetImageByID(ResID.IMAGE_BOSS_STONEHEAD_FOG1) : Res.GetImageByID(ResID.IMAGE_BOSS_STONEHEAD_FOG2));
				steam = new Steam();
				mSteam.Add(steam);
				steam.mAlphaDec = Common._M(4f);
				steam.mAngleInc = SexyFramework.Common.FloatRange(Common._M(0.01f), Common._M1(0.1f));
				steam.mVX = Common._M(2f);
				steam.mVY = Common._M(-0.02f);
				steam.mImgNum = SexyFramework.Common.Rand() % 2;
				steam.mImage = ((steam.mImgNum == 0) ? Res.GetImageByID(ResID.IMAGE_BOSS_STONEHEAD_FOG1) : Res.GetImageByID(ResID.IMAGE_BOSS_STONEHEAD_FOG2));
			}
		}
		for (int i = 0; i < mRocks.Count; i++)
		{
			RockChunk rockChunk = mRocks[i];
			rockChunk.mY += rockChunk.mVY;
			rockChunk.mX += rockChunk.mVX;
			rockChunk.mAlpha -= Common._M(4.5f);
			if (rockChunk.mAlpha <= 0f)
			{
				mRocks.RemoveAt(i);
				i--;
			}
		}
		for (int j = 0; j < mSteam.Count; j++)
		{
			Steam steam2 = mSteam[j];
			steam2.mXOff += steam2.mVX;
			steam2.mYOff += steam2.mVY;
			steam2.mAngle += steam2.mAngleInc;
			steam2.mSize += Common._M(0.01f);
			if (Math.Abs(steam2.mXOff) >= Common._M(-1f))
			{
				steam2.mAlpha -= steam2.mAlphaDec;
				if (steam2.mAlpha <= 0f)
				{
					mSteam.RemoveAt(j);
					j--;
				}
			}
		}
		if (!mBlink && !mFiring && SexyFramework.Common.Rand() % Common._M(400) == 0)
		{
			mBlink = (mBlinkClosed = true);
		}
		else if (mBlink && mUpdateCount % Common._M(5) == 0)
		{
			if (mBlinkClosed && ++mEyeFrame >= 3)
			{
				mBlinkClosed = false;
				mEyeFrame = 2;
			}
			else if (!mBlinkClosed && --mEyeFrame < 0)
			{
				mBlink = false;
				mEyeFrame = 0;
			}
		}
		if (mEyeFlameAlpha >= 255f && SexyFramework.Common._geq(mAlphaOverride, 255f))
		{
			float num = mX - (float)mWidth / 2f + (float)mShakeXOff;
			float num2 = mY - (float)mHeight / 2f + (float)mShakeYOff;
			for (int k = 0; k < mBullets.Count; k++)
			{
				BossBullet bossBullet = mBullets[k];
				EyeBullet eyeBullet = (EyeBullet)bossBullet.mData;
				if (bossBullet.mState == 0)
				{
					if (!mLeftInUse)
					{
						mLeftInUse = true;
						bossBullet.mState = -1;
						bossBullet.mX = num;
						bossBullet.mY = num2;
						eyeBullet.mXOff = Common._M(49);
						eyeBullet.mYOff = Common._M(55);
					}
					else if (!mRightInUse)
					{
						mRightInUse = true;
						bossBullet.mState = 1;
						bossBullet.mX = num;
						bossBullet.mY = num2;
						eyeBullet.mXOff = Common._M(89);
						eyeBullet.mYOff = Common._M(55);
					}
					mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BOSS_STONE_EYE_LASER));
					if (bossBullet.mState != 0 && (bossBullet.mShotType == 1 || bossBullet.mShotType == 3))
					{
						FireBulletAtPlayer(bossBullet, SexyFramework.Common.FloatRange(base.mMinBulletSpeed, base.mMaxBulletSpeed), bossBullet.mX + (float)eyeBullet.mXOff, bossBullet.mY + (float)eyeBullet.mYOff);
						bossBullet.mTargetVX = bossBullet.mVX;
						bossBullet.mTargetVY = bossBullet.mVY;
					}
				}
				else if (bossBullet.mUpdateCount > Common._M(50))
				{
					if (bossBullet.mState < 0)
					{
						mLeftInUse = false;
					}
					else
					{
						mRightInUse = false;
					}
				}
			}
			if (!mLeftInUse && !mRightInUse)
			{
				mLeftEye.mEyeFlame.mEmitAfterTimeline = (mRightEye.mEyeFlame.mEmitAfterTimeline = false);
				mLeftEye.mFiring = (mRightEye.mFiring = false);
				mFiring = false;
			}
		}
		if (mFiring)
		{
			if (mEyeFlameAlpha < 255f)
			{
				mEyeFlameAlpha += Common._M(4f);
				if (mEyeFlameAlpha > 255f)
				{
					mEyeFlameAlpha = 255f;
				}
			}
		}
		else if (mEyeFlameAlpha > 0f)
		{
			mEyeFlameAlpha -= Common._M(5f);
			if (mEyeFlameAlpha < 0f)
			{
				mEyeFlameAlpha = 0f;
			}
		}
		mLeftEye.Update((int)mX + LEFT_EYE_XOFF, (int)mY + EYE_YOFF, (int)mAlphaOverride);
		mRightEye.Update((int)mX + RIGHT_EYE_XOFF, (int)mY + EYE_YOFF, (int)mAlphaOverride);
		if (mDoingExplodeAnim && UpdateDeathSequence())
		{
			mTextAlpha -= Common._M(1f);
			mExplodeComp.Update();
			if (mExplodeComp.GetUpdateCount() == 35)
			{
				DoDeathRockExplosionThing();
			}
			if (mExplodeComp.GetUpdateCount() >= Common._M(150))
			{
				mVolcanoBoss.Update();
			}
			if (mExplodeComp.Done() && mTextAlpha <= 0f)
			{
				mLevel.SwitchToSecondaryBoss();
				mVolcanoBoss.mIntro = false;
				mApp.GetBoard().mPreventBallAdvancement = false;
				mApp.mBoard.mDrawBossUI = true;
				mApp.mBoard.mMenuButton.SetVisible(isVisible: true);
			}
		}
	}

	public override void Init(Level l)
	{
		mWidth = Common._M(120);
		mHeight = Common._M(182);
		base.Init(l);
		if (mExplodeComp != null)
		{
			mExplodeComp.Dispose();
			mExplodeComp = null;
		}
		mExplodeComp = new Composition();
		mExplodeComp.mLoadImageFunc = GameApp.CompositionLoadFunc;
		mExplodeComp.mPostLoadImageFunc = GameApp.CompositionPostLoadFunc;
		mExplodeComp.LoadFromFile("pax\\BreakEasterIsland_FINAL");
		mLeftEye.mEyeFlame = mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_STONEBOSSEYES").Duplicate();
		mRightEye.mEyeFlame = mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_STONEBOSSEYES").Duplicate();
		IMAGE_BOSS_STONEHEAD_FACES = Res.GetImageByID(ResID.IMAGE_BOSS_STONEHEAD_FACES);
		IMAGE_BOSS_STONEHEAD_ROCKS = Res.GetImageByID(ResID.IMAGE_BOSS_STONEHEAD_ROCKS);
		IMAGE_BOSS_STONEHEAD_EYES = Res.GetImageByID(ResID.IMAGE_BOSS_STONEHEAD_EYES);
	}

	public override Boss Instantiate()
	{
		BossStoneHead bossStoneHead = new BossStoneHead(mLevel);
		bossStoneHead.CopyFrom(this);
		bossStoneHead.mSteam.Clear();
		bossStoneHead.mRocks.Clear();
		return bossStoneHead;
	}

	public override void SyncState(DataSync sync)
	{
		base.SyncState(sync);
		SexyFramework.Misc.Buffer buffer = sync.GetBuffer();
		sync.SyncBoolean(ref mDoingExplodeAnim);
		sync.SyncFloat(ref mTextAlpha);
		sync.SyncBoolean(ref mShowText);
		sync.SyncLong(ref mExplodeComp.mUpdateCount);
		sync.SyncFloat(ref mStretchPct);
		sync.SyncLong(ref mShakeTime);
		sync.SyncFloat(ref mEyeFlameAlpha);
		sync.SyncBoolean(ref mBlink);
		sync.SyncBoolean(ref mBlinkClosed);
		sync.SyncBoolean(ref mFiring);
		sync.SyncLong(ref mEyeFrame);
		sync.SyncLong(ref mHitTimer);
		sync.SyncBoolean(ref mLeftInUse);
		sync.SyncBoolean(ref mRightInUse);
		mLeftEye.SyncState(sync);
		mRightEye.SyncState(sync);
		SyncListRockChunks(sync, mRocks, clear: true);
		SyncListSteams(sync, mSteam, clear: true);
		for (int i = 0; i < mBullets.Count; i++)
		{
			if (sync.isWrite())
			{
				EyeBullet eyeBullet = (EyeBullet)mBullets[i].mData;
				eyeBullet.SyncState(sync);
			}
			else
			{
				EyeBullet eyeBullet2 = new EyeBullet();
				eyeBullet2.SyncState(sync);
				mBullets[i].mData = eyeBullet2;
			}
		}
		if (sync.isWrite())
		{
			buffer.WriteBoolean(mVolcanoBoss != null);
			if (mVolcanoBoss != null)
			{
				buffer.WriteLong(mVolcanoBoss.GetX());
				buffer.WriteLong(mVolcanoBoss.GetY());
			}
		}
		else if (sync.isRead())
		{
			if (buffer.ReadBoolean())
			{
				mVolcanoBoss = (BossVolcano)mLevel.mSecondaryBoss;
				mVolcanoBoss.mIntro = true;
				int num = (int)buffer.ReadLong();
				int num2 = (int)buffer.ReadLong();
				mVolcanoBoss.SetXY(num, num2);
			}
			else
			{
				mVolcanoBoss = null;
			}
		}
	}

	private void SyncListRockChunks(DataSync sync, List<RockChunk> theList, bool clear)
	{
		if (sync.isRead())
		{
			if (clear)
			{
				theList.Clear();
			}
			long num = sync.GetBuffer().ReadLong();
			for (int i = 0; i < num; i++)
			{
				RockChunk rockChunk = new RockChunk();
				rockChunk.SyncState(sync);
				theList.Add(rockChunk);
			}
			return;
		}
		sync.GetBuffer().WriteLong(theList.Count);
		foreach (RockChunk the in theList)
		{
			the.SyncState(sync);
		}
	}

	private void SyncListSteams(DataSync sync, List<Steam> theList, bool clear)
	{
		if (sync.isRead())
		{
			if (clear)
			{
				theList.Clear();
			}
			long num = sync.GetBuffer().ReadLong();
			for (int i = 0; i < num; i++)
			{
				Steam steam = new Steam();
				steam.SyncState(sync);
				theList.Add(steam);
			}
			return;
		}
		sync.GetBuffer().WriteLong(theList.Count);
		foreach (Steam the in theList)
		{
			the.SyncState(sync);
		}
	}

	public override bool AllowFrogToFire()
	{
		if (base.AllowFrogToFire())
		{
			return !mDoingExplodeAnim;
		}
		return false;
	}

	public bool UpdateDeathSequence()
	{
		if (mShakeTime > 0)
		{
			if (mShakeTime == Common._M(150))
			{
				mShowText = true;
			}
			else if (mShakeTime < Common._M(150))
			{
				mTextAlpha += Common._M(2.8f);
			}
			mShakeTime--;
			if (mShakeTime % Common._M(50) == 0)
			{
				mShakeXAmt++;
				mShakeYAmt++;
			}
			mShakeXOff = SexyFramework.Common.IntRange(0, mShakeXAmt);
			mShakeYOff = SexyFramework.Common.IntRange(0, mShakeYAmt);
		}
		else
		{
			mShakeXOff = (mShakeYOff = 0);
		}
		for (int i = 0; i < mRocks.Count; i++)
		{
			RockChunk rockChunk = mRocks[i];
			rockChunk.mY += rockChunk.mVY;
			rockChunk.mX += rockChunk.mVX;
			rockChunk.mVY += Common._M(0.2f);
			rockChunk.mAlpha -= Common._M(4.5f);
			if (rockChunk.mAlpha <= 0f)
			{
				mRocks.RemoveAt(i);
				i--;
			}
		}
		if (Boss.gBerserkTextAlpha > 0f)
		{
			Boss.gBerserkTextAlpha -= Common._M(1f);
			Boss.gBerserkTextY -= Common._M(1f);
		}
		for (int j = 0; j < mSteam.Count; j++)
		{
			Steam steam = mSteam[j];
			steam.mXOff += steam.mVX;
			steam.mYOff += steam.mVY;
			steam.mAngle += steam.mAngleInc;
			steam.mSize += Common._M(0.01f);
			if (Math.Abs(steam.mXOff) >= Common._M(-1f))
			{
				steam.mAlpha -= steam.mAlphaDec;
				if (steam.mAlpha <= 0f)
				{
					mSteam.RemoveAt(j);
					j--;
				}
			}
		}
		if (mShakeTime <= 0)
		{
			mStretchPct += (MAX_STONE_HEAD_STRETCH - 1f) / Common._M(25f);
			if (mStretchPct >= MAX_STONE_HEAD_STRETCH)
			{
				mStretchPct = MAX_STONE_HEAD_STRETCH;
				return true;
			}
		}
		return false;
	}

	public void DoDeathRockExplosionThing()
	{
		for (int i = 45; i < 135; i += Common._M(2))
		{
			RockChunk rockChunk = new RockChunk();
			mRocks.Add(rockChunk);
			rockChunk.mCol = SexyFramework.Common.Rand() % IMAGE_BOSS_STONEHEAD_ROCKS.mNumCols;
			rockChunk.mAlpha = 255f;
			float num = SexyFramework.Common.DegreesToRadians(i);
			float num2 = SexyFramework.Common.FloatRange(Common._M(4f), Common._M1(6f));
			rockChunk.mVX = num2 * (float)Math.Cos(num);
			rockChunk.mVY = (0f - num2) * (float)Math.Sin(num);
			rockChunk.mX = mX;
			rockChunk.mY = mY;
		}
	}

	public override int GetTopLeftX()
	{
		return (int)(mX - (float)mWidth * mStretchPct / 2f);
	}

	public override int GetTopLeftY()
	{
		return (int)(mY - (float)mHeight * mStretchPct / 2f);
	}
}
