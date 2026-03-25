using System;
using System.Collections.Generic;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class BossMosquito : BossShoot
{
	private static float MIN_RADIUS = 1f;

	private static float MAX_RADIUS = 20f;

	protected int mNumBallsEaten;

	protected int mBallEatTimer = -1;

	protected int mHitTimer;

	protected int mThrowTimer;

	protected int mEyeFrame;

	protected bool mBlink;

	protected bool mBlinkClosed = true;

	protected bool mChewing;

	protected int mChewFrame = -1;

	protected int mChewCount;

	protected int mBallType = -1;

	protected int mBallTimer;

	protected int mBallCel;

	protected float mBallSize = 1f;

	protected float mBallX;

	protected float mBallY;

	protected float mBallVX;

	protected float mBallVY;

	protected List<RockParticle> mRockParticles = new List<RockParticle>();

	protected PIEffect mFlies;

	protected bool mDoFlyAnim;

	protected override void DrawBossSpecificArt(Graphics g)
	{
		if (mAlphaOverride < 255f)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)mAlphaOverride);
		}
		Image image = ((mHitTimer > 0) ? Res.GetImageByID(ResID.IMAGE_BOSS_MOSQUITO_HIT) : ((mThrowTimer <= 0) ? Res.GetImageByID(ResID.IMAGE_BOSS_MOSQUITO_IDLE) : Res.GetImageByID(ResID.IMAGE_BOSS_MOSQUITO_THROW)));
		g.DrawImage(image, (int)(Common._S(mX) - (float)(image.mWidth / 2) + (float)Common._S(mShakeXOff)), (int)(Common._S(mY) - (float)(image.mHeight / 2) + (float)Common._S(mShakeYOff)));
		Image[] array = new Image[3]
		{
			Res.GetImageByID(ResID.IMAGE_BOSS_MOSQUITO_EYES_WIDE),
			Res.GetImageByID(ResID.IMAGE_BOSS_MOSQUITO_EYES_HALF),
			Res.GetImageByID(ResID.IMAGE_BOSS_MOSQUITO_EYES_CLOSED)
		};
		if (mHitTimer <= 0)
		{
			g.DrawImage(array[mEyeFrame], (int)(Common._S(mX) - (float)(image.mWidth / 2) + (float)Common._S(mShakeXOff + Common._M(37))), (int)(Common._S(mY) - (float)(image.mHeight / 2) + (float)Common._S(mShakeYOff + Common._M1(60))));
		}
		if (mHP > 0f && !mDoDeathExplosions)
		{
			if (mTeleportDir != 0)
			{
				g.PushState();
				g.ClearClipRect();
			}
			if (!mLevel.mBoard.IsPaused())
			{
				for (int i = 0; i < mBullets.Count; i++)
				{
					if (mBullets[i].mDelay <= 0)
					{
						BossBullet bossBullet = mBullets[i];
						MosquitoBall mosquitoBall = (MosquitoBall)bossBullet.mData;
						for (int j = 0; j < mosquitoBall.mMosquitoes.Count; j++)
						{
							Mosquito mosquito = mosquitoBall.mMosquitoes[j];
							g.DrawImage(mosquito.mImage, (int)(Common._S(bossBullet.mX - (float)Common._M(20) + mosquito.mRadius * (float)Math.Cos(mosquito.mAngle)) - (float)(mosquito.mImage.mWidth / 2)), (int)(Common._S(bossBullet.mY - (float)Common._M1(20) - mosquito.mRadius * (float)Math.Sin(mosquito.mAngle)) - (float)(mosquito.mImage.mHeight / 2)));
						}
					}
				}
			}
			if (mTeleportDir != 0)
			{
				g.PopState();
			}
			if (mChewing)
			{
				Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_MOSQUITO_CHEW);
				g.DrawImageCel(imageByID, (int)(Common._S(mX) - (float)(imageByID.GetCelWidth() / 2) + (float)Common._S(mShakeXOff + Common._M(0))), (int)(Common._S(mY) - (float)(imageByID.GetCelHeight() / 2) + (float)Common._S(mShakeYOff + Common._M1(0))), mChewFrame);
			}
			if (mBallType != -1)
			{
				ResID id = (ResID)(1366 + mBallType);
				if (mApp.mColorblind && mBallType == 3)
				{
					id = ResID.IMAGE_GREEN_BALL_CBM;
				}
				else if (mApp.mColorblind && mBallType == 4)
				{
					id = ResID.IMAGE_PURPLE_BALL_CBM;
				}
				Image imageByID2 = Res.GetImageByID(id);
				int theWidth = (int)((float)imageByID2.GetCelWidth() * mBallSize);
				int theHeight = (int)((float)imageByID2.GetCelHeight() * mBallSize);
				Rect theDestRect = new Rect((int)Common._S(mBallX - (float)Common.GetDefaultBallRadius()), (int)Common._S(mBallY - (float)Common.GetDefaultBallRadius()), theWidth, theHeight);
				g.DrawImage(imageByID2, theDestRect, imageByID2.GetCelRect(mBallCel));
			}
			Image imageByID3 = Res.GetImageByID(ResID.IMAGE_BOSS_MOSQUITO_ROCK);
			for (int k = 0; k < mRockParticles.Count; k++)
			{
				RockParticle rockParticle = mRockParticles[k];
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)rockParticle.mAlpha);
				g.DrawImageCel(imageByID3, (int)Common._S(rockParticle.mX), (int)Common._S(rockParticle.mY), rockParticle.mCel);
				g.SetColorizeImages(colorizeImages: false);
			}
		}
		if (mDoFlyAnim)
		{
			mFlies.Draw(g);
		}
		g.SetColorizeImages(colorizeImages: false);
	}

	protected override Rect GetBulletRect(BossBullet b)
	{
		int num = Common._M(20);
		int num2 = Common._M(20);
		return new Rect((int)(b.mX - (float)(num / 2)), (int)(b.mY - (float)(num2 / 2)), num, num2);
	}

	protected override Rect GetFrogRect()
	{
		int centerX = mLevel.mFrog.GetCenterX();
		int centerY = mLevel.mFrog.GetCenterY();
		return new Rect(centerX - mLevel.mFrog.GetWidth() / 2 + Common._M(32), centerY - mLevel.mFrog.GetHeight() / 2 + Common._M1(12), Common._M2(78), Common._M3(110));
	}

	protected override bool DoHit(Bullet b, bool from_prox_bomb)
	{
		bool result = base.DoHit(b, from_prox_bomb);
		if (from_prox_bomb)
		{
			mThrowTimer = 0;
			mHitTimer = Common._M(100);
		}
		return result;
	}

	protected override void DidFire()
	{
		base.DidFire();
		if (mHitTimer == 0)
		{
			mThrowTimer = Common._M(100);
		}
	}

	protected override BossBullet CreateBossBullet()
	{
		BossBullet bossBullet = new BossBullet();
		bossBullet.mData = MakeMosquitoBall();
		return bossBullet;
	}

	protected override void BossBulletDestroyed(BossBullet b, bool outofscreen)
	{
		if (b.mData != null)
		{
			b.mData = null;
		}
	}

	protected override void BallEaten(Bullet b)
	{
		mNumBallsEaten++;
		if (mBallEatTimer <= 0)
		{
			mBallEatTimer = 2000;
		}
		if (mNumBallsEaten >= 20)
		{
			mApp.SetAchievement("foie_gras");
		}
		mChewing = true;
		if (mChewFrame < 0)
		{
			mChewFrame = 0;
		}
		mChewCount = 0;
		mBallType = b.GetColorType();
		mBallSize = 1f;
		mBallTimer = Common._M(20);
		mBallVX = (mX + (float)Common._M(15) - b.GetX()) / (float)mBallTimer;
		mBallVY = (mY + (float)Common._M(40) - b.GetY()) / (float)mBallTimer;
		mBallX = b.GetX();
		mBallY = b.GetY();
		mBallCel = b.mLastFrame;
		if (!mApp.IsHardMode())
		{
			mTauntQueue.Clear();
			TauntText tauntText = new TauntText();
			mTauntQueue.Add(tauntText);
			tauntText.mText = TextManager.getInstance().getString(390);
			tauntText.mTextId = 390;
			tauntText.mDelay = Common._M(500);
		}
	}

	protected override void BulletHitPlayer(BossBullet b)
	{
		base.BulletHitPlayer(b);
		if (mApp.GetLevelMgr().mBossesCanAttackFuckedFrog)
		{
			return;
		}
		for (int i = 0; i < mBullets.Count; i++)
		{
			BossBullet bossBullet = mBullets[i];
			if (bossBullet.mDelay > 0)
			{
				bossBullet.mDeleteInstantly = true;
			}
		}
	}

	protected virtual MosquitoBall MakeMosquitoBall()
	{
		MosquitoBall mosquitoBall = new MosquitoBall();
		Image[] array = new Image[3]
		{
			Res.GetImageByID(ResID.IMAGE_BOSS_MOSQUITO_BUG1),
			Res.GetImageByID(ResID.IMAGE_BOSS_MOSQUITO_BUG2),
			Res.GetImageByID(ResID.IMAGE_BOSS_MOSQUITO_BUG3)
		};
		for (int i = 0; i < Common._M(30); i++)
		{
			Mosquito mosquito = new Mosquito();
			mosquitoBall.mMosquitoes.Add(mosquito);
			mosquito.mImage = array[SexyFramework.Common.Rand() % 3];
			mosquito.mRadius = SexyFramework.Common.FloatRange(MIN_RADIUS, MAX_RADIUS);
			mosquito.mAngle = SexyFramework.Common.FloatRange(0f, 6.28318f);
			mosquito.mAngleInc = SexyFramework.Common.FloatRange(Common._M(0.07f), Common._M1(0.1f)) * (float)((SexyFramework.Common.Rand() % 2 == 0) ? 1 : (-1));
			mosquito.mRadInc = SexyFramework.Common.FloatRange(Common._M(0.4f), Common._M1(0.8f));
		}
		return mosquitoBall;
	}

	protected override void AppliedSlowTimer()
	{
		mLevel.mFrog.DoPlaguedState();
		mFlies.ResetAnim();
		mDoFlyAnim = true;
	}

	public BossMosquito(Level l)
		: base(l)
	{
		mBandagedXOff = Common._M(-8);
		mResGroup = "Boss4";
		mBossRadius = Common._M(50);
		mBulletRadius = Common._M(20);
		mResPrefix = "IMAGE_BOSS_MOSQUITO_";
		mBulletsUseSphereColl = false;
	}

	public BossMosquito()
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
				mBullets[i].mData = null;
			}
		}
		mBullets.Clear();
	}

	public void CopyFrom(BossMosquito rhs)
	{
		CopyFrom((BossShoot)rhs);
		mNumBallsEaten = rhs.mNumBallsEaten;
		mBallEatTimer = rhs.mBallEatTimer;
		mHitTimer = rhs.mHitTimer;
		mThrowTimer = rhs.mThrowTimer;
		mEyeFrame = rhs.mEyeFrame;
		mBlink = rhs.mBlink;
		mBlinkClosed = rhs.mBlinkClosed;
		mChewing = rhs.mChewing;
		mChewFrame = rhs.mChewFrame;
		mChewCount = rhs.mChewCount;
		mBallType = rhs.mBallType;
		mBallTimer = rhs.mBallTimer;
		mBallCel = rhs.mBallCel;
		mBallSize = rhs.mBallSize;
		mBallX = rhs.mBallX;
		mBallY = rhs.mBallY;
		mBallVX = rhs.mBallVX;
		mBallVY = rhs.mBallVY;
		mFlies = rhs.mFlies;
		mDoFlyAnim = rhs.mDoFlyAnim;
		for (int i = 0; i < rhs.mRockParticles.Count; i++)
		{
			RockParticle item = new RockParticle(rhs.mRockParticles[i]);
			mRockParticles.Add(item);
		}
	}

	public override void Update(float f)
	{
		base.Update(f);
		if (mDoFlyAnim)
		{
			mFlies.mDrawTransform.LoadIdentity();
			float num = GameApp.DownScaleNum(1f);
			mFlies.mDrawTransform.Scale(num, num);
			mFlies.mDrawTransform.Translate(Common._S(mLevel.mFrog.GetCurX() + Common._M(0)), Common._S(mLevel.mFrog.GetCurY() + Common._M1(0)));
			mFlies.Update();
			if (mFlies.mFrameNum >= (float)mFlies.mLastFrameNum && mFlies.mCurNumParticles == 0)
			{
				mDoFlyAnim = false;
			}
		}
		if (mBallEatTimer > 0 && --mBallEatTimer == 0)
		{
			mNumBallsEaten = 0;
		}
		if (mHitTimer > 0)
		{
			mHitTimer--;
		}
		if (mThrowTimer > 0)
		{
			mThrowTimer--;
		}
		if (mChewing && mUpdateCount % Common._M(7) == 0)
		{
			mChewFrame++;
			Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_MOSQUITO_CHEW);
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BOSS_MOSQUITO_ROCK);
			if (mChewFrame >= imageByID.mNumCols)
			{
				for (int i = 0; i < Common._M(10); i++)
				{
					RockParticle rockParticle = new RockParticle();
					mRockParticles.Add(rockParticle);
					rockParticle.mAlpha = 255f;
					rockParticle.mCel = SexyFramework.Common.Rand() % imageByID2.mNumCols;
					rockParticle.mX = mX + (float)SexyFramework.Common.IntRange(Common._M(-30), Common._M1(30));
					rockParticle.mY = mY + (float)Common._M(10);
					rockParticle.mVX = SexyFramework.Common.FloatRange(Common._M(-2.5f), Common._M1(2.5f));
					rockParticle.mVY = SexyFramework.Common.FloatRange(Common._M(1.5f), Common._M1(2.5f));
				}
				mChewCount++;
				mChewFrame = 1;
				if (mChewCount >= Common._M(4))
				{
					mChewing = false;
				}
			}
		}
		for (int j = 0; j < mRockParticles.Count; j++)
		{
			RockParticle rockParticle2 = mRockParticles[j];
			if ((rockParticle2.mAlpha -= Common._M(6f)) <= 0f)
			{
				mRockParticles.RemoveAt(j);
				j--;
			}
			else
			{
				rockParticle2.mX += rockParticle2.mVX;
				rockParticle2.mY += rockParticle2.mVY;
			}
		}
		if (mBallType != -1)
		{
			mBallX += mBallVX;
			mBallY += mBallVY;
			mBallSize -= 1f / (float)mBallTimer;
			if (SexyFramework.Common._leq(mBallSize, 0f, 0.0001f))
			{
				mBallType = -1;
			}
		}
		for (int k = 0; k < mBullets.Count; k++)
		{
			BossBullet bossBullet = mBullets[k];
			MosquitoBall mosquitoBall = (MosquitoBall)bossBullet.mData;
			for (int l = 0; l < mosquitoBall.mMosquitoes.Count; l++)
			{
				Mosquito mosquito = mosquitoBall.mMosquitoes[l];
				mosquito.mRadius += mosquito.mRadInc;
				if (mosquito.mRadInc > 0f && mosquito.mRadius >= MAX_RADIUS)
				{
					mosquito.mRadius = MAX_RADIUS;
					mosquito.mRadInc *= -1f;
				}
				else if (mosquito.mRadInc < 0f && mosquito.mRadius <= MIN_RADIUS)
				{
					mosquito.mRadius = MIN_RADIUS;
					mosquito.mAngleInc = SexyFramework.Common.FloatRange(Common._M(0.07f), Common._M1(0.1f)) * (float)((SexyFramework.Common.Rand() % 2 == 0) ? 1 : (-1));
					mosquito.mRadInc = SexyFramework.Common.FloatRange(Common._M(0.4f), Common._M1(0.8f));
				}
				mosquito.mAngle += mosquito.mAngleInc;
			}
		}
		if (mHitTimer == 0 && !mBlink && SexyFramework.Common.Rand() % Common._M(400) == 0)
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
	}

	public override void Init(Level l)
	{
		mWidth = Common._M(139);
		mHeight = Common._M(149);
		base.Init(l);
		mBandagedImg = Res.GetImageByID(ResID.IMAGE_BOSS_MOSQUITO_BANDAGED);
		mFlies = mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_FLYSWARM");
		mFlies.ResetAnim();
	}

	public override void SyncState(DataSync sync)
	{
		base.SyncState(sync);
		if (sync.isRead())
		{
			for (int i = 0; i < mBullets.Count; i++)
			{
				mBullets[i].mData = MakeMosquitoBall();
			}
		}
		sync.SyncBoolean(ref mChewing);
		sync.SyncLong(ref mChewFrame);
		sync.SyncLong(ref mChewCount);
		sync.SyncLong(ref mBallType);
		sync.SyncLong(ref mBallTimer);
		sync.SyncLong(ref mBallCel);
		sync.SyncFloat(ref mBallSize);
		sync.SyncFloat(ref mBallX);
		sync.SyncFloat(ref mBallY);
		sync.SyncFloat(ref mBallVX);
		sync.SyncFloat(ref mBallVY);
		sync.SyncBoolean(ref mDoFlyAnim);
		if (sync.isWrite())
		{
			Common.SerializePIEffect(mFlies, sync);
		}
		else
		{
			if (mFlies == null)
			{
				mFlies = mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_FLYSWARM");
			}
			Common.DeserializePIEffect(mFlies, sync);
		}
		SyncListRockParticles(sync, mRockParticles, clear: true);
	}

	private void SyncListRockParticles(DataSync sync, List<RockParticle> theList, bool clear)
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
				RockParticle rockParticle = new RockParticle();
				rockParticle.SyncState(sync);
				theList.Add(rockParticle);
			}
			return;
		}
		sync.GetBuffer().WriteLong(theList.Count);
		foreach (RockParticle the in theList)
		{
			the.SyncState(sync);
		}
	}

	public override Boss Instantiate()
	{
		BossMosquito bossMosquito = new BossMosquito(mLevel);
		bossMosquito.CopyFrom(this);
		return bossMosquito;
	}
}
