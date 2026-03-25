using System;
using System.Collections.Generic;
using SexyFramework;
using SexyFramework.AELib;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class BossSquid : BossShoot
{
	public enum State
	{
		State_Idle,
		State_Throwing,
		State_Tutorial
	}

	public CompositionMgr mInkFrogComp;

	protected SquidAnim mBody = new SquidAnim();

	protected SquidAnim mLeftArm = new SquidAnim();

	protected SquidAnim mRightArm = new SquidAnim();

	protected List<SquidSweat> mSweat = new List<SquidSweat>();

	protected List<InkParticle> mInk = new List<InkParticle>();

	protected List<InkCloud> mInkClouds = new List<InkCloud>();

	protected bool mHasBeenHitByProxBomb;

	protected int mState;

	protected int mJawCount;

	protected int mJawDelay = 1 + SexyFramework.Common.SafeRand() % 50;

	protected int mBlinkDelay = 1 + SexyFramework.Common.SafeRand() % 150;

	protected int mBlinkCel = -1;

	protected int mLeftThrowCel = -1;

	protected int mRightThrowCel = -1;

	protected int mHitTimer;

	protected float mTutorialShieldRotateAmt;

	public static int FPS_ADJUST(float fps)
	{
		return (int)(fps * 100f / 30f);
	}

	protected override void DrawBossSpecificArt(Graphics g)
	{
		int num = (int)mX - mWidth / 2;
		int num2 = (int)mY - mHeight / 2;
		if (mAlphaOverride < 255f)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)mAlphaOverride);
		}
		g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_HEAD), Common._S(num + Common._M(40) + mShakeXOff), Common._S(num2 + Common._M1(11) + mShakeYOff));
		if (mHitTimer > 0)
		{
			if (mHitTimer < 255)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)Math.Min(mHitTimer, mAlphaOverride));
			}
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_HEAD_DARK), Common._S(num + Common._M(40) + mShakeXOff), Common._S(num2 + Common._M1(11) + mShakeYOff));
			g.SetColorizeImages(colorizeImages: false);
		}
		if (mAlphaOverride < 255f)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)mAlphaOverride);
		}
		mBody.Draw(g, Common._S(num + mShakeXOff), Common._S(num2 + mShakeYOff));
		if (mState != 1)
		{
			mLeftArm.Draw(g, Common._S(num + mShakeXOff + Common._M(0)), Common._S(num2 + mShakeYOff + Common._M1(0)));
			mRightArm.Draw(g, Common._S(num + mShakeXOff + Common._M(0)), Common._S(num2 + mShakeYOff + Common._M1(0)));
		}
		else if (mState == 1)
		{
			if (mLeftThrowCel != -1)
			{
				g.DrawImageCel(Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_THROW_LEFT), Common._S(num + Common._M(3) + mShakeXOff), Common._S(num2 + Common._M1(45) + mShakeYOff), mLeftThrowCel);
			}
			else
			{
				mLeftArm.Draw(g, Common._S(num + mShakeXOff + Common._M(0)), Common._S(num2 + mShakeYOff + Common._M1(0)));
			}
			if (mRightThrowCel != -1)
			{
				g.DrawImageCel(Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_THROW_RIGHT), Common._S(num + Common._M(102) + mShakeXOff), Common._S(num2 + Common._M1(58) + mShakeYOff), mRightThrowCel);
			}
			else
			{
				mRightArm.Draw(g, Common._S(num + mShakeXOff + Common._M(0)), Common._S(num2 + mShakeYOff + Common._M1(0)));
			}
		}
		if (mHP > 0f)
		{
			if (mTeleportDir != 0)
			{
				g.PushState();
				g.ClearClipRect();
			}
			List<InkParticle> list = new List<InkParticle>();
			list.AddRange(mInk.ToArray());
			if (!mLevel.mBoard.IsPaused())
			{
				for (int i = 0; i < mBullets.Count; i++)
				{
					BossBullet bossBullet = mBullets[i];
					if (bossBullet.mData != null)
					{
						list.Add((InkParticle)bossBullet.mData);
					}
				}
			}
			Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_GLOBULE1);
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_GLOBULE2);
			float[] array = new float[3]
			{
				Common._M(0.75f),
				Common._M1(0.5f),
				Common._M2(0.84f)
			};
			float[] array2 = new float[3]
			{
				Common._M(20),
				Common._M1(30),
				Common._M2(30)
			};
			float[] array3 = new float[3]
			{
				Common._M(0.5f),
				0f,
				Common._M1(-0.5f)
			};
			mGlobalTranform.Reset();
			for (int j = 0; j < list.Count; j++)
			{
				InkParticle inkParticle = list[j];
				if (inkParticle.mAlpha != 255f)
				{
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(255, 255, 255, (int)inkParticle.mAlpha);
				}
				if (inkParticle.mImage == imageByID && inkParticle.mPostHitCount < Common._M(5))
				{
					for (int k = 0; k < 3; k++)
					{
						mGlobalTranform.Reset();
						mGlobalTranform.Scale(inkParticle.mWidthPct * array[k], inkParticle.mHeightPct * array[k]);
						mGlobalTranform.RotateRad(inkParticle.mAngle);
						float num3 = inkParticle.mX + array2[k] * (float)Math.Cos(inkParticle.mAngle + (float)Math.PI / 2f + array3[k]);
						float num4 = inkParticle.mY - array2[k] * (float)Math.Sin(inkParticle.mAngle + (float)Math.PI / 2f + array3[k]);
						num3 += (float)inkParticle.mPostHitCount * (float)Math.Cos(inkParticle.mAngle - (float)Math.PI / 2f) * inkParticle.mInitSpeed;
						num4 -= (float)inkParticle.mPostHitCount * (float)Math.Sin(inkParticle.mAngle - (float)Math.PI / 2f) * inkParticle.mInitSpeed;
						g.DrawImageTransform(imageByID2, mGlobalTranform, Common._S(num3), Common._S(num4));
					}
				}
				mGlobalTranform.Reset();
				mGlobalTranform.Scale(inkParticle.mWidthPct, inkParticle.mHeightPct);
				mGlobalTranform.RotateRad(inkParticle.mAngle);
				g.DrawImageTransform(inkParticle.mImage, mGlobalTranform, Common._S(inkParticle.mX), Common._S(inkParticle.mY));
				g.SetColorizeImages(colorizeImages: false);
			}
			if (mTeleportDir != 0)
			{
				g.PopState();
			}
		}
		Gun gun = mApp.GetBoard().GetGun();
		if (gun.IsInked())
		{
			Composition composition = mInkFrogComp.GetComposition("Main");
			if (!composition.Done())
			{
				CumulativeTransform cumulativeTransform = new CumulativeTransform();
				float num5 = Common._M(0.5f);
				cumulativeTransform.mTrans.Scale(num5, num5);
				cumulativeTransform.mTrans.RotateDeg(90f);
				cumulativeTransform.mTrans.Translate(Common._S(gun.GetCenterX() + Common._M(-146)), Common._S(gun.GetCenterY() + Common._M1(200)));
				composition.Draw(g, cumulativeTransform, -1, Common._DS(1f));
			}
		}
		Image imageByID3 = Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_DARK_CLOUD);
		for (int l = 0; l < mInkClouds.Count; l++)
		{
			InkCloud inkCloud = mInkClouds[l];
			mGlobalTranform.Reset();
			mGlobalTranform.Scale(inkCloud.mSize * 4f, inkCloud.mSize * 4f);
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)inkCloud.mAlpha);
			g.DrawImageTransform(imageByID3, mGlobalTranform, Common._S(inkCloud.mX), Common._S(inkCloud.mY));
			g.SetColorizeImages(colorizeImages: false);
		}
		if (mAlphaOverride < 255f)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)mAlphaOverride);
		}
		if (mJawCount > 0)
		{
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_MOUTH_OPEN), Common._S(num + Common._M(53) + mShakeXOff), Common._S(num2 + Common._M1(89) + mShakeYOff));
			if (mHitTimer > 0)
			{
				if (mHitTimer < 255)
				{
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(255, 255, 255, (int)Math.Min(mHitTimer, mAlphaOverride));
				}
				g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_MOUTH_OPEN_DARK), Common._S(num + Common._M(51) + mShakeXOff), Common._S(num2 + Common._M1(88) + mShakeYOff));
				g.SetColorizeImages(colorizeImages: false);
			}
		}
		if (mAlphaOverride < 255f)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)mAlphaOverride);
		}
		if (mBlinkCel >= 0)
		{
			g.DrawImageCel(Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_EYES), Common._S(num + Common._M(59) + mShakeXOff), Common._S(num2 + Common._M1(72) + mShakeYOff), mBlinkCel);
		}
		g.SetColorizeImages(colorizeImages: false);
		if (mHP > 0f)
		{
			Image imageByID4 = Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_SQUIRT);
			for (int m = 0; m < mSweat.Count; m++)
			{
				SquidSweat squidSweat = mSweat[m];
				Rect celRect = imageByID4.GetCelRect(squidSweat.mCel);
				g.DrawImageRotated(imageByID4, (int)Common._S(squidSweat.mX), (int)Common._S(squidSweat.mY), squidSweat.mAngle, celRect);
			}
		}
	}

	protected override void DrawShield(Graphics g)
	{
		if (mHP <= 0f || mDoDeathExplosions || mLevel.mBoard.DoingBossIntro())
		{
			return;
		}
		for (int i = 0; i < Common._M(4); i++)
		{
			ShieldQuadrantData shieldQuadrantData = (ShieldQuadrantData)mShieldQuadrant[i].mData;
			if (mShieldQuadrant[i].mTimer > 51 && !shieldQuadrantData.mDoExplodeAnim)
			{
				continue;
			}
			float rot = mShieldAngle + (float)i * 3.14159f / 2f;
			float num = Common._M(0);
			float num2 = Common._M(0);
			if (!shieldQuadrantData.mDoExplodeAnim)
			{
				int mTimer = mShieldQuadrant[i].mTimer;
				Composition composition = shieldQuadrantData.mCompMgr.GetComposition("NormalQuad");
				CumulativeTransform cumulativeTransform = new CumulativeTransform();
				cumulativeTransform.mOpacity = ((mTimer > 0) ? ((float)(255 - mTimer * 5) / 255f) : 1f);
				if (!SexyFramework.Common._eq(mAlphaOverride, 255f))
				{
					cumulativeTransform.mOpacity = mAlphaOverride / 255f;
				}
				cumulativeTransform.mTrans.Translate((float)(-composition.mWidth) * Common._DS(1f), (float)(-composition.mHeight) * Common._DS(1f));
				cumulativeTransform.mTrans.RotateRad(rot);
				cumulativeTransform.mTrans.Translate(Common._S(mX - num2), Common._S(mY - num));
				composition.Draw(g, cumulativeTransform, -1, Common._DS(1f));
				if (g.Is3D())
				{
					g.PushState();
					if (!SexyFramework.Common._eq(mAlphaOverride, 255f))
					{
						shieldQuadrantData.mSparkles.mColor.mAlpha = (int)mAlphaOverride;
					}
					shieldQuadrantData.mSparkles.Draw(g);
					g.PopState();
				}
			}
			else
			{
				Composition composition2 = shieldQuadrantData.mCompMgr.GetComposition("ExplodeQuad");
				CumulativeTransform cumulativeTransform2 = new CumulativeTransform();
				cumulativeTransform2.mTrans.Translate((float)(-composition2.mWidth) * Common._DS(1f), (float)(-composition2.mHeight) * Common._DS(1f));
				cumulativeTransform2.mTrans.RotateRad(rot);
				cumulativeTransform2.mTrans.Translate(Common._S(mX - num2), Common._S(mY - num));
				if (!SexyFramework.Common._eq(mAlphaOverride, 255f))
				{
					cumulativeTransform2.mOpacity = mAlphaOverride / 255f;
				}
				composition2.Draw(g, cumulativeTransform2, -1, Common._DS(1f));
			}
			if (shieldQuadrantData.mDoHitAnim)
			{
				Composition composition3 = shieldQuadrantData.mCompMgr.GetComposition("HitQuad");
				CumulativeTransform cumulativeTransform3 = new CumulativeTransform();
				cumulativeTransform3.mTrans.Translate((float)(-composition3.mWidth) * Common._DS(1f), (float)(-composition3.mHeight) * Common._DS(1f));
				cumulativeTransform3.mTrans.RotateRad(rot);
				cumulativeTransform3.mTrans.Translate(Common._S(mX - num2), Common._S(mY - num));
				if (!SexyFramework.Common._eq(mAlphaOverride, 255f))
				{
					cumulativeTransform3.mOpacity = mAlphaOverride / 255f;
				}
				composition3.Draw(g, cumulativeTransform3, -1, Common._DS(1f));
			}
		}
	}

	public override void DrawBelowBalls(Graphics g)
	{
	}

	protected override bool PreBulletUpdate(BossBullet b, int index)
	{
		if (b.mState == 0)
		{
			if (mLeftThrowCel == -1 && mRightThrowCel == -1)
			{
				if ((float)mLevel.mFrog.GetCenterX() > mX)
				{
					mLeftThrowCel = 0;
					b.mState = -1;
				}
				else
				{
					mRightThrowCel = 0;
					b.mState = 1;
				}
			}
			else if (mLeftThrowCel == -1)
			{
				mLeftThrowCel = 0;
				b.mState = -1;
			}
			else
			{
				if (mRightThrowCel != -1)
				{
					return true;
				}
				mLeftThrowCel = 0;
				b.mState = 1;
			}
			mState = 1;
			b.mVX = (b.mVY = 0f);
		}
		else if (Math.Abs(b.mState) == 2)
		{
			b.mState += Math.Sign(b.mState);
			b.mX = mX - (float)(mWidth / 2) + (float)((b.mState < 0) ? Common._M(15) : Common._M1(153));
			b.mY = mY - (float)(mHeight / 2) + (float)((b.mState < 0) ? Common._M(105) : Common._M1(105)) + b.mY;
			float num = 0f;
			if (b.mShotType == 1)
			{
				num = FireBulletAtPlayer(b, SexyFramework.Common.FloatRange(base.mMinBulletSpeed, base.mMaxBulletSpeed), b.mX, b.mY);
				b.mTargetVX = b.mVX;
				b.mTargetVY = b.mVY;
			}
			else if (b.mShotType == 0)
			{
				b.mVY = SexyFramework.Common.FloatRange(base.mMinBulletSpeed, base.mMaxBulletSpeed);
			}
			InkParticle inkParticle = (InkParticle)(b.mData = new InkParticle());
			inkParticle.mX = b.mX;
			inkParticle.mY = b.mY;
			inkParticle.mWidthPct = Common._M(0.57f);
			inkParticle.mHeightPct = Common._M(0.27f);
			inkParticle.mAngle = ((b.mShotType == 0) ? (-(float)Math.PI / Common._M(2f)) : num);
			if (b.mVX < 0f)
			{
				inkParticle.mAngle += (float)Math.PI * Common._M(0.5f);
			}
			else
			{
				inkParticle.mAngle += (float)Math.PI * Common._M(0.5f);
			}
			inkParticle.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_GLOBULE1);
			inkParticle.mVX = (inkParticle.mVY = (inkParticle.mGravity = 0f));
			inkParticle.mAlpha = 255f;
			inkParticle.mAlphaRate = 0f;
			inkParticle.mInitSpeed = b.mInitialSpeed;
		}
		return b.mState == 0;
	}

	protected override Rect GetBulletRect(BossBullet b)
	{
		int num = Common._M(17);
		int num2 = Common._M(30);
		return new Rect((int)b.mX - num / 2, (int)b.mY - num2 / 2, num, num2);
	}

	protected override bool DoHit(Bullet b, bool from_prox_bomb)
	{
		bool flag = base.DoHit(b, from_prox_bomb);
		if (from_prox_bomb)
		{
			if (from_prox_bomb && !mHasBeenHitByProxBomb && !mApp.IsHardMode() && mApp.mUserProfile.GetAdvModeVars().mNumTimesZoneBeat[4] == 0)
			{
				mHasBeenHitByProxBomb = true;
				mState = 2;
				mApp.GetBoard().mLevel.FadeInkSpots();
				mApp.GetBoard().mPreventBallAdvancement = true;
				if (mTauntQueue.Count > 1)
				{
					mTauntQueue.RemoveRange(1, mTauntQueue.Count - 1);
				}
				TauntText tauntText = new TauntText();
				mTauntQueue.Add(tauntText);
				tauntText.mText = TextManager.getInstance().getString(391);
				tauntText.mDelay = Common._M(300);
				tauntText.mTextId = 391;
				tauntText = new TauntText();
				mTauntQueue.Add(tauntText);
				tauntText.mText = TextManager.getInstance().getString(392);
				tauntText.mDelay = Common._M(1000);
				tauntText.mTextId = 392;
				mPauseMovement = true;
				mPauseShieldRegen = true;
			}
			else if (from_prox_bomb && mApp.mUserProfile.GetAdvModeVars().mNumTimesZoneBeat[4] > 0)
			{
				mHasBeenHitByProxBomb = true;
			}
			return flag;
		}
		mHitTimer = Common._M(300);
		if (flag && mState == 2)
		{
			mState = 0;
			mPauseShieldRegen = (mPauseMovement = false);
			base.mShieldPauseTime = 0;
			mApp.GetBoard().mPreventBallAdvancement = false;
			for (int i = 0; i < 4; i++)
			{
				mShieldQuadrant[i].mTimer = 0;
				mShieldQuadrant[i].mHP = base.mShieldHP;
			}
		}
		return flag;
	}

	protected override void BulletHitPlayer(BossBullet b)
	{
		if (b.mData != null)
		{
			InkParticle inkParticle = (InkParticle)b.mData;
			b.mData = null;
			mInk.Add(inkParticle);
			int num = Common._M(8);
			float num2 = SexyFramework.Common.DegreesToRadians(Common._M(45));
			float num3 = SexyFramework.Common.DegreesToRadians(Common._M(235));
			float num4 = (num3 - num2) / (float)num;
			Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_GLOBULE2);
			for (int i = 0; i < num; i++)
			{
				InkParticle inkParticle2 = new InkParticle();
				mInk.Add(inkParticle2);
				inkParticle2.mImage = imageByID;
				inkParticle2.mX = inkParticle.mX;
				inkParticle2.mY = inkParticle.mY;
				inkParticle2.mWidthPct = (inkParticle2.mHeightPct = SexyFramework.Common.FloatRange(Common._M(0.25f), 0.75f));
				inkParticle2.mGravity = SexyFramework.Common.FloatRange(Common._M(0.08f), Common._M1(0.13f));
				inkParticle2.mAngle = num2 + num4 * (float)i;
				float num5 = Common._M(3.5f);
				inkParticle2.mVX = num5 * (float)Math.Cos(inkParticle2.mAngle);
				inkParticle2.mVY = (0f - num5) * (float)Math.Sin(inkParticle2.mAngle);
				inkParticle2.mAlpha = 255f;
				inkParticle2.mAlphaRate = Common._M(2f);
				inkParticle2.mJiggleDir = ((i % 2 == 0) ? 1 : (-1));
				inkParticle2.mJiggleRate = SexyFramework.Common.FloatRange(Common._M(0.02f), Common._M1(0.03f));
			}
			InkCloud inkCloud = new InkCloud();
			mInkClouds.Add(inkCloud);
			inkCloud.mAlpha = 0f;
			inkCloud.mFadeIn = true;
			inkCloud.mSize = Common._M(0.2f);
			inkCloud.mX = b.mX;
			inkCloud.mY = b.mY;
			mInkFrogComp.GetComposition("Main").Reset();
			mApp.GetBoard().GetGun().DoInkedState();
		}
		if (!mApp.GetLevelMgr().mBossesCanAttackFuckedFrog)
		{
			for (int j = 0; j < mBullets.Count; j++)
			{
				BossBullet bossBullet = mBullets[j];
				if (bossBullet.mData == null)
				{
					bossBullet.mDeleteInstantly = true;
				}
			}
		}
		base.BulletHitPlayer(b);
	}

	protected override void ShieldQuadrantHit(int quad)
	{
		ShieldQuadrantData shieldQuadrantData = (ShieldQuadrantData)mShieldQuadrant[quad].mData;
		shieldQuadrantData.mDoHitAnim = true;
		PlaySound(9);
	}

	protected override void BossBulletDestroyed(BossBullet b, bool outofscreen)
	{
		b.mData = null;
	}

	protected override bool CanFire()
	{
		if (mApp.mUserProfile.GetAdvModeVars().mNumTimesZoneBeat[4] > 0)
		{
			return true;
		}
		if (!mHasBeenHitByProxBomb && !mApp.IsHardMode())
		{
			return false;
		}
		return true;
	}

	protected override void QuadHitByProxBomb(int quad)
	{
		((ShieldQuadrantData)mShieldQuadrant[quad].mData).mDoExplodeAnim = true;
	}

	public BossSquid(Level l)
		: base(l)
	{
		mShieldRadius = Common._M(100);
		mProxBombRadius = Common._M(140);
		mResGroup = "Boss5";
		mResPrefix = "IMAGE_BOSS_SQUID_";
		mBossRadius = Common._M(70);
		mDrawHeartsBelowMisc = false;
	}

	public BossSquid()
		: this(null)
	{
	}

	public override void Dispose()
	{
		base.Dispose();
		if (mInkFrogComp != null)
		{
			mInkFrogComp = null;
		}
		if (mApp.mResourceManager.IsGroupLoaded("Underwater"))
		{
			mApp.mResourceManager.DeleteResources("Underwater");
		}
		for (int i = 0; i < mBullets.Count; i++)
		{
			mBullets[i].mData = null;
		}
		for (int j = 0; j < mInk.Count; j++)
		{
			mInk[j] = null;
		}
		for (int k = 0; k < 4; k++)
		{
			mShieldQuadrant[k].mData = null;
		}
	}

	protected void CopyFrom(BossSquid rhs)
	{
		CopyFrom((BossShoot)rhs);
		mBody = new SquidAnim(rhs.mBody);
		mLeftArm = new SquidAnim(rhs.mLeftArm);
		mRightArm = new SquidAnim(rhs.mRightArm);
		mHasBeenHitByProxBomb = rhs.mHasBeenHitByProxBomb;
		mState = rhs.mState;
		mJawCount = rhs.mJawCount;
		mJawDelay = rhs.mJawDelay;
		mBlinkDelay = rhs.mBlinkDelay;
		mBlinkCel = rhs.mBlinkCel;
		mLeftThrowCel = rhs.mLeftThrowCel;
		mRightThrowCel = rhs.mRightThrowCel;
		mHitTimer = rhs.mHitTimer;
		mTutorialShieldRotateAmt = rhs.mTutorialShieldRotateAmt;
	}

	public override void SyncState(DataSync sync)
	{
		base.SyncState(sync);
		SexyFramework.Misc.Buffer buffer = sync.GetBuffer();
		sync.SyncBoolean(ref mHasBeenHitByProxBomb);
		sync.SyncLong(ref mState);
		SyncListInkClouds(sync, mInkClouds, clear: true);
		SyncListInkParticles(sync, mInk, clear: true);
		for (int i = 0; i < 4; i++)
		{
			ShieldQuadrantData shieldQuadrantData = (ShieldQuadrantData)mShieldQuadrant[i].mData;
			sync.SyncBoolean(ref shieldQuadrantData.mDoHitAnim);
			sync.SyncBoolean(ref shieldQuadrantData.mDoExplodeAnim);
		}
		for (int j = 0; j < mBullets.Count; j++)
		{
			if (sync.isWrite())
			{
				InkParticle inkParticle = (InkParticle)mBullets[j].mData;
				if (inkParticle == null)
				{
					buffer.WriteBoolean(theBool: false);
					continue;
				}
				buffer.WriteBoolean(theBool: true);
				inkParticle.SyncState(sync);
			}
			else if (buffer.ReadBoolean())
			{
				InkParticle inkParticle2 = new InkParticle();
				inkParticle2.SyncState(sync);
				mBullets[j].mData = inkParticle2;
			}
		}
	}

	private void SyncListInkClouds(DataSync sync, List<InkCloud> theList, bool clear)
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
				InkCloud inkCloud = new InkCloud();
				inkCloud.SyncState(sync);
				theList.Add(inkCloud);
			}
			return;
		}
		sync.GetBuffer().WriteLong(theList.Count);
		foreach (InkCloud the in theList)
		{
			the.SyncState(sync);
		}
	}

	private void SyncListInkParticles(DataSync sync, List<InkParticle> theList, bool clear)
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
				InkParticle inkParticle = new InkParticle();
				inkParticle.SyncState(sync);
				theList.Add(inkParticle);
			}
			return;
		}
		sync.GetBuffer().WriteLong(theList.Count);
		foreach (InkParticle the in theList)
		{
			the.SyncState(sync);
		}
	}

	public override void Update(float f)
	{
		base.Update(f);
		if (mDoDeathExplosions || mHP <= 0f || mLevel.mBoard.DoingBossIntro())
		{
			return;
		}
		mBody.Update();
		mRightArm.Update();
		mLeftArm.Update();
		if (mApp.GetBoard().GetGun().IsInked())
		{
			Composition composition = mInkFrogComp.GetComposition("Main");
			if (!composition.Done())
			{
				composition.Update();
			}
		}
		if (mHitTimer > 0)
		{
			mHitTimer--;
			if (mUpdateCount % Common._M(20) == 0 && mHitTimer > Common._M1(128))
			{
				int num = 2 + SexyFramework.Common.Rand() % Common._M(1);
				for (int i = 0; i < num; i++)
				{
					SquidSweat squidSweat = new SquidSweat();
					mSweat.Add(squidSweat);
					squidSweat.mAngle = SexyFramework.Common.DegreesToRadians(Common._M(45) + SexyFramework.Common.Rand() % Common._M1(90));
					float num2 = SexyFramework.Common.FloatRange(Common._M(2.5f), Common._M1(3f));
					squidSweat.mVX = (float)Math.Cos(squidSweat.mAngle) * num2;
					squidSweat.mVY = (0f - (float)Math.Sin(squidSweat.mAngle)) * num2;
					squidSweat.mX = mX - (float)Common._M(30);
					squidSweat.mY = mY - (float)Common._M(70);
					squidSweat.mX += squidSweat.mVX * (float)Common._M(20);
					squidSweat.mY += squidSweat.mVY * (float)Common._M(0);
				}
			}
		}
		Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_SQUIRT);
		for (int j = 0; j < mSweat.Count; j++)
		{
			SquidSweat squidSweat2 = mSweat[j];
			squidSweat2.mX += squidSweat2.mVX;
			squidSweat2.mY += squidSweat2.mVY;
			if (mUpdateCount % Common._M(6) == 0 && ++squidSweat2.mCel >= imageByID.mNumCols)
			{
				mSweat.RemoveAt(j);
				j--;
			}
		}
		if (mState == 1)
		{
			int[] array = new int[2] { mLeftThrowCel, mRightThrowCel };
			Image[] array2 = new Image[2]
			{
				Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_THROW_LEFT),
				Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_THROW_RIGHT)
			};
			for (int k = 0; k < 2; k++)
			{
				if (array[k] < 0 || mUpdateCount % Common._M(6) != 0)
				{
					continue;
				}
				if (++array[k] >= array2[k].mNumCols)
				{
					array[k] = -1;
				}
				if (array[k] != 1)
				{
					continue;
				}
				for (int l = 0; l < mBullets.Count; l++)
				{
					BossBullet bossBullet = mBullets[l];
					if (bossBullet.mState == ((k != 0) ? 1 : (-1)))
					{
						bossBullet.mState = ((k == 0) ? (-2) : 2);
						bossBullet.mX = (bossBullet.mY = 0f);
						break;
					}
				}
			}
			mLeftThrowCel = array[0];
			mRightThrowCel = array[1];
			if (mLeftThrowCel == mRightThrowCel && mLeftThrowCel == -1)
			{
				mState = 0;
			}
		}
		else
		{
			_ = mState;
			_ = 2;
		}
		float num3 = Common._M(0.7f);
		float num4 = Common._M(0.8f);
		for (int m = 0; m < mBullets.Count; m++)
		{
			if (mBullets[m].mData != null)
			{
				InkParticle inkParticle = (InkParticle)mBullets[m].mData;
				float num5 = Common._M(0.04f);
				float num6 = Common._M(0.04f);
				inkParticle.mWidthPct += num5;
				inkParticle.mHeightPct += num6;
				if (inkParticle.mWidthPct > num3)
				{
					inkParticle.mWidthPct = num3;
				}
				if (inkParticle.mHeightPct > num4)
				{
					inkParticle.mHeightPct = num4;
				}
				inkParticle.mX = mBullets[m].mX;
				inkParticle.mY = mBullets[m].mY;
			}
		}
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_GLOBULE1);
		for (int n = 0; n < mInk.Count; n++)
		{
			InkParticle inkParticle2 = mInk[n];
			if (inkParticle2.mImage == imageByID2)
			{
				inkParticle2.mPostHitCount++;
				float num7 = Common._M(1.11f);
				float num8 = Common._M(0f);
				float num9 = (num7 - num3) / Common._M(10f);
				float num10 = (num8 - num4) / Common._M(10f);
				inkParticle2.mWidthPct += num9;
				inkParticle2.mHeightPct += num10;
				if (inkParticle2.mWidthPct > num7)
				{
					inkParticle2.mWidthPct = num7;
				}
				if (inkParticle2.mHeightPct < num8)
				{
					inkParticle2.mHeightPct = num8;
				}
				if (inkParticle2.mWidthPct >= num7 && inkParticle2.mHeightPct <= num8)
				{
					mInk.RemoveAt(n);
					n--;
				}
				continue;
			}
			inkParticle2.mX += inkParticle2.mVX;
			inkParticle2.mY += inkParticle2.mVY;
			inkParticle2.mVY += inkParticle2.mGravity;
			inkParticle2.mAlpha -= inkParticle2.mAlphaRate;
			if (inkParticle2.mJiggleDir > 0)
			{
				inkParticle2.mWidthPct += inkParticle2.mJiggleRate;
				inkParticle2.mHeightPct += inkParticle2.mJiggleRate;
				if (inkParticle2.mWidthPct > 0.75f || inkParticle2.mHeightPct > 0.75f)
				{
					inkParticle2.mWidthPct = (inkParticle2.mHeightPct = 0.75f);
					inkParticle2.mJiggleDir *= -1;
				}
			}
			else
			{
				inkParticle2.mWidthPct -= inkParticle2.mJiggleRate;
				inkParticle2.mHeightPct -= inkParticle2.mJiggleRate;
				if (inkParticle2.mWidthPct < 0.25f || inkParticle2.mHeightPct < 0.25f)
				{
					inkParticle2.mWidthPct = (inkParticle2.mHeightPct = 0.25f);
					inkParticle2.mJiggleDir *= -1;
				}
			}
			if (inkParticle2.mAlpha <= 0f)
			{
				mInk.RemoveAt(n);
				n--;
			}
		}
		for (int num11 = 0; num11 < mInkClouds.Count; num11++)
		{
			InkCloud inkCloud = mInkClouds[num11];
			inkCloud.mSize += Common._M(0.004f);
			if (inkCloud.mFadeIn)
			{
				inkCloud.mAlpha += Common._M(5f);
				if (inkCloud.mAlpha >= 255f)
				{
					inkCloud.mAlpha = 255f;
					inkCloud.mFadeIn = false;
				}
			}
			else
			{
				inkCloud.mAlpha -= Common._M(4f);
				if (inkCloud.mAlpha <= 0f)
				{
					mInkClouds.RemoveAt(num11);
					num11--;
				}
			}
		}
		if (mJawCount > 0)
		{
			if (--mJawCount == 0)
			{
				mJawDelay = Common._M(400) + SexyFramework.Common.Rand() % Common._M1(400);
			}
		}
		else if (mUpdateCount % mJawDelay == 0)
		{
			mJawCount = Common._M(20) + SexyFramework.Common.Rand() % Common._M1(50);
		}
		if (mBlinkCel < 0 && mUpdateCount % mBlinkDelay == 0)
		{
			mBlinkCel = 0;
			mBlinkDelay = Common._M(300) + SexyFramework.Common.Rand() % Common._M1(200);
		}
		else if (mBlinkCel >= 0 && mUpdateCount % Common._M(15) == 0 && ++mBlinkCel >= Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_EYES).mNumCols)
		{
			mBlinkCel = -1;
		}
		for (int num12 = 0; num12 < 4; num12++)
		{
			ShieldQuadrantData shieldQuadrantData = (ShieldQuadrantData)mShieldQuadrant[num12].mData;
			float rot = mShieldAngle + (float)num12 * 3.14159f / 2f;
			int num13 = Common._DS(Common._M(200));
			shieldQuadrantData.mSparkles.mDrawTransform.LoadIdentity();
			float num14 = GameApp.DownScaleNum(1f);
			shieldQuadrantData.mSparkles.mDrawTransform.Scale(num14, num14);
			shieldQuadrantData.mSparkles.mDrawTransform.Translate(0f, -num13);
			shieldQuadrantData.mSparkles.mDrawTransform.RotateRad(rot);
			shieldQuadrantData.mSparkles.mDrawTransform.Translate(Common._S(mX), (float)num13 + Common._S(mY - (float)Common._M(100)));
			shieldQuadrantData.mSparkles.Update();
			if (!shieldQuadrantData.mDoExplodeAnim && mShieldQuadrant[num12].mTimer <= 0)
			{
				Composition composition2 = shieldQuadrantData.mCompMgr.GetComposition("NormalQuad");
				composition2.mLoop = true;
				composition2.Update();
			}
			else if (shieldQuadrantData.mDoExplodeAnim)
			{
				Composition composition3 = shieldQuadrantData.mCompMgr.GetComposition("ExplodeQuad");
				composition3.Update();
				if (composition3.Done())
				{
					shieldQuadrantData.mDoExplodeAnim = false;
					composition3.Reset();
				}
			}
			if (shieldQuadrantData.mDoHitAnim)
			{
				Composition composition4 = shieldQuadrantData.mCompMgr.GetComposition("HitQuad");
				composition4.Update();
				if (composition4.Done())
				{
					shieldQuadrantData.mDoHitAnim = false;
					composition4.Reset();
				}
			}
		}
	}

	public override void Init(Level l)
	{
		mWidth = Common._M(185);
		mHeight = Common._M(172);
		base.Init(l);
		if (!mApp.mResourceManager.IsGroupLoaded("Underwater") && !mApp.mResourceManager.LoadResources("Underwater"))
		{
			mApp.ShowResourceError(doExit: true);
			mApp.Shutdown();
			return;
		}
		mBandagedImg = Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_BANDAGED);
		for (int i = 0; i < 4; i++)
		{
			mShieldQuadrant[i].mData = new ShieldQuadrantData(mApp.LoadComposition("pax\\SquidBossShields", "_BOSS_SQUID"), mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_SHIELDSPARKLES").Duplicate());
			ShieldQuadrantData shieldQuadrantData = (ShieldQuadrantData)mShieldQuadrant[i].mData;
			shieldQuadrantData.mSparkles.mEmitAfterTimeline = true;
		}
		mInkFrogComp = mApp.LoadComposition("pax\\ink frog", "_BOSS_SQUID");
		mBody.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_BODY);
		mBody.mX = 42f;
		mBody.mY = 102f;
		mBody.AddAnimInfo(0, FPS_ADJUST(3f));
		mBody.AddAnimInfo(1, FPS_ADJUST(3f));
		mBody.AddAnimInfo(2, FPS_ADJUST(5f));
		mBody.AddAnimInfo(1, FPS_ADJUST(3f));
		mBody.AddAnimInfo(0, FPS_ADJUST(10f));
		mLeftArm.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_LEG_LEFT);
		mLeftArm.mX = 7f;
		mLeftArm.mY = 43f;
		mLeftArm.AddAnimInfo(0, FPS_ADJUST(2f));
		mLeftArm.AddAnimInfo(1, FPS_ADJUST(2f));
		mLeftArm.AddAnimInfo(2, FPS_ADJUST(2f));
		mLeftArm.AddAnimInfo(3, FPS_ADJUST(5f));
		mLeftArm.AddAnimInfo(2, FPS_ADJUST(2f));
		mLeftArm.AddAnimInfo(1, FPS_ADJUST(2f));
		mLeftArm.AddAnimInfo(0, FPS_ADJUST(2f));
		mLeftArm.AddAnimInfo(4, FPS_ADJUST(5f));
		mRightArm.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_LEG_RIGHT);
		mRightArm.mX = 102f;
		mRightArm.mY = 58f;
		mRightArm.AddAnimInfo(0, FPS_ADJUST(2f));
		mRightArm.AddAnimInfo(1, FPS_ADJUST(2f));
		mRightArm.AddAnimInfo(2, FPS_ADJUST(2f));
		mRightArm.AddAnimInfo(3, FPS_ADJUST(2f));
		mRightArm.AddAnimInfo(4, FPS_ADJUST(5f));
		mRightArm.AddAnimInfo(3, FPS_ADJUST(2f));
		mRightArm.AddAnimInfo(2, FPS_ADJUST(2f));
		mRightArm.AddAnimInfo(1, FPS_ADJUST(2f));
		mRightArm.AddAnimInfo(5, FPS_ADJUST(2f));
		mRightArm.AddAnimInfo(6, FPS_ADJUST(2f));
		mRightArm.AddAnimInfo(5, FPS_ADJUST(2f));
	}

	public override Boss Instantiate()
	{
		BossSquid bossSquid = new BossSquid(mLevel);
		bossSquid.CopyFrom(this);
		return bossSquid;
	}

	protected override void ReInit()
	{
		base.ReInit();
		for (int i = 0; i < 4; i++)
		{
			mShieldQuadrant[i].mTimer = 0;
			mShieldQuadrant[i].mHP = base.mShieldHP;
		}
	}
}
