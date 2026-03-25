using System;
using System.Collections.Generic;
using SexyFramework;
using SexyFramework.AELib;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class BossLame : BossShoot
{
	private enum EState
	{
		State_Crying,
		State_FlagWave
	}

	protected float mAngleOff;

	protected float mAngleDir;

	protected int mNumHeartsVisible;

	protected bool mIsFiring;

	protected int mHitTimer;

	protected int mDeathTextTimer;

	protected bool mBlink;

	protected bool mBlinkClosed;

	protected bool mRunRight;

	protected int mEyeFrame;

	protected int mCryLoopCount;

	protected int mDeathState;

	protected int mDeathStateTimer;

	protected List<Feather> mFeathers = new List<Feather>();

	protected List<EggFragment> mEggFragments = new List<EggFragment>();

	protected CompositionMgr mDeathComp;

	protected PIEffect mPoof;

	protected override void DrawBossSpecificArt(Graphics g)
	{
		int num = (int)(mX - (float)(Common._M(183) / 2));
		int num2 = (int)(mY - (float)(Common._M(213) / 2));
		Composition composition = null;
		bool flag = mDeathState == 0 || mPoof.mFrameNum < 81f;
		g.PushState();
		if (!MathUtils._eq(mAlphaOverride, 255f))
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)mAlphaOverride);
		}
		if (mDeathState <= 1 && mDeathState >= 0)
		{
			composition = mDeathComp.GetComposition(flag ? "CRY" : "FLAG WAVE");
			CumulativeTransform cumulativeTransform = new CumulativeTransform();
			cumulativeTransform.mTrans.Translate(Common._S(num) + Common._DS(Common._M(-442)), Common._S(num2) + Common._DS(Common._M1(-354)));
			int frame = -1;
			if (flag && mDeathState != 0)
			{
				frame = 0;
			}
			composition.Draw(g, cumulativeTransform, frame, Common._DS(1f));
		}
		else if (mHitTimer > 0)
		{
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_LAME_TIKI_HIT), Common._S(num), Common._S(num2));
		}
		else if (mIsFiring)
		{
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_LAME_TIKI_THROW), Common._S(num), Common._S(num2));
		}
		else
		{
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_LAME_TIKI_IDLE), Common._S(num), Common._S(num2));
			if (mBlink)
			{
				g.DrawImageCel(Res.GetImageByID(ResID.IMAGE_BOSS_LAME_EYES), Common._S(num + Common._M(38)), Common._S(num2 + Common._M1(60)), mEyeFrame);
			}
		}
		int num3 = (mIsFiring ? Common._M(10) : 0);
		if (mDeathState == -1 || flag)
		{
			g.DrawImageRotated(Res.GetImageByID(ResID.IMAGE_BOSS_LAME_CHICKEN_LEFT), Common._S(num + Common._M(-20)), Common._S(num2 + Common._M1(68) + num3), mAngleOff);
			g.DrawImageRotated(Res.GetImageByID(ResID.IMAGE_BOSS_LAME_CHICKEN_RIGHT), Common._S(num + Common._M(100)), Common._S(num2 + Common._M1(67) + num3), 0f - mAngleOff);
		}
		if (mDeathState == 1 && mPoof.mFrameNum < (float)mPoof.mLastFrameNum)
		{
			g.PushState();
			mPoof.Draw(g);
			g.PopState();
		}
		if (mTeleportDir != 0)
		{
			g.PushState();
			g.ClearClipRect();
		}
		if (!mLevel.mBoard.IsPaused())
		{
			for (int i = 0; i < mBullets.Count; i++)
			{
				BossBullet bossBullet = mBullets[i];
				if (bossBullet.mState != 0)
				{
					Egg egg = (Egg)bossBullet.mData;
					mGlobalTranform.Reset();
					mGlobalTranform.RotateRad(egg.mAngle);
					mGlobalTranform.Scale(egg.mSize, egg.mSize);
					g.DrawImageTransform(Res.GetImageByID(ResID.IMAGE_BOSS_LAME_EGG), mGlobalTranform, Common._S(bossBullet.mX), Common._S(bossBullet.mY));
				}
			}
		}
		for (int j = 0; j < mEggFragments.Count; j++)
		{
			EggFragment eggFragment = mEggFragments[j];
			if (eggFragment.mAlpha != 255f)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)Math.Min(eggFragment.mAlpha, mAlphaOverride));
			}
			g.DrawImageCel(Res.GetImageByID(ResID.IMAGE_BOSS_LAME_ROCKS), (int)Common._S(eggFragment.mX), (int)Common._S(eggFragment.mY), eggFragment.mCol);
			g.SetColorizeImages(colorizeImages: false);
		}
		if (mTeleportDir != 0)
		{
			g.PopState();
		}
		for (int k = 0; k < mFeathers.Count; k++)
		{
			Feather feather = mFeathers[k];
			float num4 = Math.Min(feather.mAlpha, mAlphaOverride);
			if (num4 != 255f)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)num4);
			}
			g.DrawImageRotated(feather.mImage, (int)Common._S(feather.mX), (int)Common._S(feather.mY), feather.mAngleOsc.GetVal());
			g.SetColorizeImages(colorizeImages: false);
		}
		g.PopState();
	}

	protected override bool DoHit(Bullet b, bool from_prox_bomb)
	{
		mHitTimer = Common._M(100);
		AddFeathers();
		bool flag = base.DoHit(b, from_prox_bomb);
		if (flag)
		{
			mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_CHICKEN_SURRENDER));
			if (Common._S(mX) < (float)(GameApp.gApp.mWidth / 2))
			{
				mRunRight = true;
			}
			else
			{
				mRunRight = false;
			}
			mPauseMovement = true;
			mDeathTextTimer = Common._M(600);
			mApp.GetBoard().mPreventBallAdvancement = true;
			mDeathState = 0;
			mApp.GetBoard().mContinueNextLevelOnLoadProfile = true;
			mCryLoopCount = 0;
		}
		return flag;
	}

	protected override void DidFire()
	{
		base.DidFire();
		mIsFiring = true;
	}

	protected override bool CanFire()
	{
		return SexyFramework.Common._eq(mHP, mMaxHP);
	}

	protected override bool CanRetaliate()
	{
		return false;
	}

	protected override bool PreBulletUpdate(BossBullet b, int index)
	{
		Egg egg = (Egg)b.mData;
		if (b.mState <= 2)
		{
			b.mX = mX - (float)(Common._M(183) / 2) + (float)Common._M1(72);
			b.mY = mY - (float)(Common._M(213) / 2) + (float)Common._M1(110);
		}
		if (b.mState == 0)
		{
			return true;
		}
		if (b.mState == 1)
		{
			egg.mSize += Common._M(0.04f);
			if (egg.mSize >= Common._M(0.75f))
			{
				b.mState++;
			}
			return true;
		}
		if (b.mState == 2)
		{
			egg.mSize -= Common._M(0.03f);
			if (egg.mSize <= Common._M(0.5f))
			{
				egg.mSize = Common._M(0.5f);
				b.mState++;
				if (b.mShotType == 1 || b.mShotType == 3)
				{
					FireBulletAtPlayer(b, SexyFramework.Common.FloatRange(base.mMinBulletSpeed, base.mMaxBulletSpeed), b.mX, b.mY);
					b.mTargetVX = b.mVX;
					b.mTargetVY = b.mVY;
				}
			}
			return true;
		}
		if (egg.mSize < 1f)
		{
			egg.mSize += Common._M(0.02f);
			if (egg.mSize > 1f)
			{
				egg.mSize = 1f;
			}
		}
		egg.mAngle += Common._M(0.05f);
		return false;
	}

	protected override BossBullet CreateBossBullet()
	{
		BossBullet bossBullet = new BossBullet();
		Egg mData = new Egg();
		bossBullet.mData = mData;
		return bossBullet;
	}

	protected override void BossBulletDestroyed(BossBullet b, bool outofscreen)
	{
		if (b.mData != null)
		{
			b.mData = null;
		}
	}

	protected override void BulletHitPlayer(BossBullet b)
	{
		base.BulletHitPlayer(b);
		int num = Common._M(40);
		float num2 = 6.28318f / (float)num;
		for (int i = 0; i < num; i++)
		{
			EggFragment eggFragment = new EggFragment();
			mEggFragments.Add(eggFragment);
			eggFragment.mCol = SexyFramework.Common.Rand() % Res.GetImageByID(ResID.IMAGE_BOSS_LAME_ROCKS).mNumCols;
			eggFragment.mX = b.mX;
			eggFragment.mY = b.mY;
			eggFragment.mAlpha = 255f;
			float value = (float)i * num2;
			float num3 = SexyFramework.Common.FloatRange(Common._M(4f), Common._M1(6f));
			eggFragment.mVX = SexyMath.CosF(value) * num3;
			eggFragment.mVY = (0f - SexyMath.SinF(value)) * num3;
			eggFragment.mDecVX = eggFragment.mVX / Common._M(40f);
			eggFragment.mDecVY = eggFragment.mVY / Common._M(40f);
		}
		if (GameApp.gApp.GetLevelMgr().mBossesCanAttackFuckedFrog)
		{
			return;
		}
		for (int j = 0; j < mBullets.Count; j++)
		{
			BossBullet bossBullet = mBullets[j];
			if (bossBullet.mState == 0)
			{
				bossBullet.mDeleteInstantly = true;
			}
		}
	}

	protected override void DrawHearts(Graphics g)
	{
		if (mHP <= 0f || mDoDeathExplosions || mNumHeartsVisible <= 0 || mLevel.mBoard.DoingBossIntro())
		{
			return;
		}
		g.PushState();
		if (!SexyFramework.Common._eq(mAlphaOverride, 255f))
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)mAlphaOverride);
		}
		int num = Boss.NUM_HEARTS - mNumHeartsVisible;
		int num2 = Boss.NUM_HEARTS * 2 - num - 1;
		for (int i = 0; i < Boss.NUM_HEARTS * 2; i++)
		{
			if (i >= num && i <= num2)
			{
				int theCel = ((i < Boss.NUM_HEARTS) ? mHeartCels[i] : 0);
				if (mHP < mMaxHP - 1f)
				{
					theCel = 4;
				}
				g.DrawImageCel(Res.GetImageByID(ResID.IMAGE_BOSS_HEARTS), (int)(Common._S(mX + (float)mHeartXOff) + (float)(i * Res.GetImageByID(ResID.IMAGE_BOSS_HEARTS).GetCelWidth())), (int)Common._S(mY + (float)mHeartYOff), theCel);
			}
		}
		g.PopState();
	}

	protected virtual void AddFeathers()
	{
		AddFeathers(do_more: false);
	}

	protected virtual void AddFeathers(bool do_more)
	{
		float num = SexyFramework.Common.DegreesToRadians(Common._M(45));
		float num2 = SexyFramework.Common.DegreesToRadians(Common._M(165));
		int num3 = Common._M(7);
		if (do_more)
		{
			num3 += Common._M(15);
		}
		float num4 = (num2 - num) / (float)num3;
		float num5 = Common._M(10f);
		int num6 = (int)(mX - (float)(Common._M(183) / 2));
		int num7 = (int)(mY - (float)(Common._M(213) / 2));
		for (int i = 0; i < num3; i++)
		{
			Feather feather = new Feather();
			mFeathers.Add(feather);
			feather.mImgNum = 1 + SexyFramework.Common.Rand() % 4;
			feather.mImage = Res.GetImageByID((ResID)(1023 + (feather.mImgNum - 1)));
			feather.mX = num6 + ((mX < mDestX) ? Common._M(90) : Common._M1(30));
			feather.mY = num7 + Common._M(100);
			feather.mVX = SexyMath.CosF(num + (float)i * num4) * num5;
			feather.mVY = (0f - SexyMath.SinF(num + (float)i * num4)) * num5;
			feather.mDecVX = feather.mVX / Common._M(25f);
			feather.mDecVY = feather.mVY / Common._M(25f);
			feather.mAlpha = 255f;
			feather.mAngleOsc.Init(SexyFramework.Common.DegreesToRadians(Common._M(-45)), SexyFramework.Common.DegreesToRadians(Common._M1(45)), SexyFramework.Common.Rand() % 2 == 0, SexyFramework.Common.FloatRange(Common._M2(0.0001f), Common._M3(0.0003f)));
		}
	}

	protected override bool BulletIntersectsBoss(Bullet b)
	{
		return MathUtils.CirclesIntersect(b.GetX(), b.GetY(), mX - (float)Common._DS(Common._M(30)), mY + (float)mBossRadiusYOff, mBossRadius + b.GetRadius() + Common._DS(Common._M1(0)));
	}

	public BossLame()
		: base(null)
	{
		Initialize();
	}

	public BossLame(Level l)
		: base(l)
	{
		Initialize();
	}

	private void Initialize()
	{
		mAngleOff = 0f;
		mAngleDir = Common._M(0.2f);
		mIsFiring = false;
		mBlink = false;
		mBlinkClosed = true;
		mEyeFrame = 0;
		mHitTimer = 0;
		mResGroup = "Boss6_Lame";
		mDeathTextTimer = 0;
		mBossRadius = Common._M(70);
		mNumHeartsVisible = Boss.NUM_HEARTS;
		mBulletRadius = Common._M(25);
		mDeathStateTimer = 0;
		mDeathComp = null;
		mPoof = null;
		mCryLoopCount = 0;
		mDeathState = -1;
		mDrawDeathBGTikis = false;
		mDrawHeartsBelowBoss = true;
	}

	public override void DrawTopLevel(Graphics g)
	{
		base.DrawTopLevel(g);
		g.SetFont(Res.GetFontByID(ResID.FONT_BOSS_TAUNT));
		int num = mDeathTextTimer;
		if (num > 255)
		{
			num = 255;
		}
		else if (num <= 0)
		{
			return;
		}
		g.SetColor(0, 0, 0, num);
		string[] array = new string[3]
		{
			TextManager.getInstance().getString(384),
			TextManager.getInstance().getString(385),
			TextManager.getInstance().getString(386)
		};
		if (mLevel.mBoard.IsHardAdventureMode())
		{
			array[0] = TextManager.getInstance().getString(387);
			array[1] = TextManager.getInstance().getString(388);
			array[2] = "";
		}
		for (int i = 0; i < array.Length; i++)
		{
			g.WriteString(array[i], -GameApp.gApp.mBoardOffsetX, Common._DS(Common._M(650)) + i * g.GetFont().GetHeight(), 1024);
		}
	}

	public override bool AllowFrogToFire()
	{
		if (base.AllowFrogToFire())
		{
			return mDeathTextTimer <= 0;
		}
		return false;
	}

	public override void Update()
	{
		Update(1f);
	}

	public override void Update(float f)
	{
		base.Update(f);
		if (!mLevel.AllCurvesAtRolloutPoint() && mLevel.mCanDrawBoss && mUpdateCount % Common._M(40) == 0 && mNumHeartsVisible < Boss.NUM_HEARTS)
		{
			mNumHeartsVisible++;
		}
		if (mFireDelay <= Common._M(100))
		{
			mIsFiring = true;
			mAngleOff += mAngleDir;
			if ((mAngleDir > 0f && mAngleOff >= Common._M(0.26f)) || (mAngleDir < 0f && mAngleOff <= 0f))
			{
				mAngleDir *= -1f;
			}
		}
		else
		{
			mAngleOff = 0f;
		}
		if (mDeathTextTimer > 0)
		{
			mDeathTextTimer--;
		}
		if (mDeathTextTimer <= 0 && mApp.GetBoard().GetGameState() != GameState.GameState_Boss6FakeCredits && mDeathState == 1 && ((mRunRight && Common._S(mX) > (float)(GameApp.gApp.mWidth + Common._DS(Common._M(200)))) || (!mRunRight && Common._S(mX) < (float)Common._DS(Common._M1(-200)))))
		{
			mHP = 0f;
			mApp.GetBoard().BossDied();
			return;
		}
		if (mDeathState == 0)
		{
			Composition composition = mDeathComp.GetComposition("CRY");
			composition.Update();
			if (composition.Done())
			{
				mCryLoopCount++;
				composition.mUpdateCount = 1;
			}
			if (mCryLoopCount == Common._M(2))
			{
				mPoof.ResetAnim();
				mDeathState = 1;
			}
		}
		else if (mDeathState == 1)
		{
			mPoof.mDrawTransform.LoadIdentity();
			float num = GameApp.DownScaleNum(1f);
			mPoof.mDrawTransform.Scale(num, num);
			int value = (int)(mX - (float)(Common._M(183) / 2));
			int value2 = (int)(mY - (float)(Common._M(213) / 2));
			mPoof.mDrawTransform.Translate(Common._S(value) + Common._DS(Common._M(0)), Common._S(value2) + Common._DS(Common._M1(200)));
			mPoof.Update();
			Composition composition2 = mDeathComp.GetComposition("FLAG WAVE");
			composition2.mLoop = true;
			composition2.mMaxFrame = Common._M(32);
			if (mPoof.mFrameNum >= (float)mPoof.mLastFrameNum)
			{
				composition2.Update();
			}
			if (mDeathStateTimer == Common._M(299))
			{
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_CHICKEN_FLEE));
				AddFeathers(do_more: true);
			}
			if (++mDeathStateTimer >= Common._M(300))
			{
				mX += (mRunRight ? Common._M(16) : Common._M1(-16));
			}
		}
		int num2 = -1;
		bool flag = true;
		for (int i = 0; i < mBullets.Count; i++)
		{
			BossBullet bossBullet = mBullets[i];
			if (bossBullet.mState == 1 || bossBullet.mState == 2)
			{
				flag = false;
			}
			else if (bossBullet.mState == 0)
			{
				num2 = i;
			}
		}
		if (flag && num2 != -1)
		{
			BossBullet bossBullet2 = mBullets[num2];
			bossBullet2.mState++;
		}
		else if (flag && num2 == -1)
		{
			mIsFiring = false;
		}
		if (mHitTimer > 0)
		{
			mHitTimer--;
		}
		if (!mIsFiring && !mBlink && mHitTimer == 0 && SexyFramework.Common.Rand() % Common._M(100) == 0)
		{
			mBlink = (mBlinkClosed = true);
			mEyeFrame = 0;
		}
		if (mBlink && mUpdateCount % Common._M(4) == 0)
		{
			if (mBlinkClosed && ++mEyeFrame >= Res.GetImageByID(ResID.IMAGE_BOSS_LAME_EYES).mNumCols)
			{
				mBlinkClosed = false;
				mEyeFrame = Res.GetImageByID(ResID.IMAGE_BOSS_LAME_EYES).mNumCols - 1;
			}
			else if (!mBlinkClosed && --mEyeFrame < 0)
			{
				mBlink = false;
			}
		}
		for (int j = 0; j < mFeathers.Count; j++)
		{
			Feather feather = mFeathers[j];
			feather.mX += feather.mVX;
			feather.mY += feather.mVY;
			if (feather.mVX != 0f)
			{
				int num3 = (int)SexyFramework.Common.Sign(feather.mVX);
				feather.mVX -= feather.mDecVX;
				int num4 = (int)SexyFramework.Common.Sign(feather.mVX);
				if (num4 != num3)
				{
					feather.mVX = 0f;
				}
			}
			if (feather.mVY != 0f)
			{
				int num5 = (int)SexyFramework.Common.Sign(feather.mVY);
				feather.mVY -= feather.mDecVY;
				int num6 = (int)SexyFramework.Common.Sign(feather.mVY);
				if (num6 != num5)
				{
					feather.mVY = 0f;
				}
			}
			feather.mY += Common._M(0.5f);
			if (feather.mVX == feather.mVY && feather.mVX == 0f)
			{
				feather.mAngleOsc.Update();
				feather.mAlpha -= Common._M(2.5f);
				if (feather.mAlpha <= 0f)
				{
					mFeathers.RemoveAt(j);
					j--;
				}
			}
		}
		for (int k = 0; k < mEggFragments.Count; k++)
		{
			EggFragment eggFragment = mEggFragments[k];
			eggFragment.mX += eggFragment.mVX;
			eggFragment.mY += eggFragment.mVY + Common._M(0f);
			eggFragment.mVX -= eggFragment.mDecVX;
			eggFragment.mVY -= eggFragment.mDecVY;
			eggFragment.mAlpha -= Common._M(8f);
			if (eggFragment.mAlpha <= 0f)
			{
				mEggFragments.RemoveAt(k);
				k--;
			}
		}
	}

	public override void Init(Level l)
	{
		mWidth = Common._M(151);
		mHeight = Common._M(201);
		base.Init(l);
		mNumHeartsVisible = Common._M(-1);
		mX = Common._SS(GameApp.gApp.mWidth) / 2 + Common._DS(Common._M(32)) - GameApp.gApp.mBoardOffsetX;
		if (GameApp.gApp.mUserProfile.mBoss6Part2DialogSeen == 0)
		{
			GameApp.gApp.mUserProfile.mBoss6Part2DialogSeen++;
			TauntText tauntText = new TauntText();
			mTauntQueue.Add(tauntText);
			tauntText.mText = TextManager.getInstance().getString(389);
			tauntText.mTextId = 389;
			tauntText.mDelay = Common._M(500);
		}
		mDeathComp = mApp.LoadComposition("pax\\Lametiki_flagwave1", "_BOSS_LAME");
		mPoof = mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_GENERICBOSSPOOF").Duplicate();
		Common.SetFXNumScale(mPoof, mApp.Is3DAccelerated() ? 1f : Common._M(0.15f));
		for (int i = 0; i < Boss.NUM_HEARTS; i++)
		{
			mHeartCels[i] = 0;
		}
	}

	public override Boss Instantiate()
	{
		BossLame bossLame = new BossLame(mLevel);
		bossLame.CopyFrom(this);
		return bossLame;
	}

	public void CopyFrom(BossLame rhs)
	{
		CopyFrom((BossShoot)rhs);
		mAngleOff = rhs.mAngleOff;
		mAngleDir = rhs.mAngleDir;
		mIsFiring = rhs.mIsFiring;
		mBlink = rhs.mBlink;
		mBlinkClosed = rhs.mBlinkClosed;
		mEyeFrame = rhs.mEyeFrame;
		mHitTimer = rhs.mHitTimer;
		mResGroup = rhs.mResGroup;
		mDeathTextTimer = rhs.mDeathTextTimer;
		mBossRadius = rhs.mBossRadius;
		mNumHeartsVisible = rhs.mNumHeartsVisible;
		mBulletRadius = rhs.mBulletRadius;
		mDeathStateTimer = rhs.mDeathStateTimer;
		mDeathComp = rhs.mDeathComp;
		mPoof = rhs.mPoof;
		mCryLoopCount = rhs.mCryLoopCount;
		mDeathState = rhs.mDeathState;
		mDrawDeathBGTikis = rhs.mDrawDeathBGTikis;
		mDrawHeartsBelowBoss = rhs.mDrawHeartsBelowBoss;
	}

	public override void SyncState(DataSync sync)
	{
		base.SyncState(sync);
		sync.SyncLong(ref mNumHeartsVisible);
		sync.SyncLong(ref mDeathTextTimer);
		sync.SyncFloat(ref mAngleOff);
		sync.SyncFloat(ref mAngleDir);
		sync.SyncBoolean(ref mIsFiring);
		sync.SyncLong(ref mHitTimer);
		sync.SyncBoolean(ref mBlink);
		sync.SyncBoolean(ref mBlinkClosed);
		sync.SyncLong(ref mEyeFrame);
		SyncListFeathers(sync, mFeathers, clear: true);
		SyncListEggFragments(sync, mEggFragments, clear: true);
		for (int i = 0; i < mBullets.Count; i++)
		{
			if (sync.isWrite())
			{
				Egg egg = (Egg)mBullets[i].mData;
				egg.SyncState(sync);
			}
			else
			{
				Egg egg2 = new Egg();
				egg2.SyncState(sync);
				mBullets[i].mData = egg2;
			}
		}
	}

	private void SyncListFeathers(DataSync sync, List<Feather> theList, bool clear)
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
				Feather feather = new Feather();
				feather.SyncState(sync);
				theList.Add(feather);
			}
			return;
		}
		sync.GetBuffer().WriteLong(theList.Count);
		foreach (Feather the in theList)
		{
			the.SyncState(sync);
		}
	}

	private void SyncListEggFragments(DataSync sync, List<EggFragment> theList, bool clear)
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
				EggFragment eggFragment = new EggFragment();
				eggFragment.SyncState(sync);
				theList.Add(eggFragment);
			}
			return;
		}
		sync.GetBuffer().WriteLong(theList.Count);
		foreach (EggFragment the in theList)
		{
			the.SyncState(sync);
		}
	}
}
