using System;
using System.Collections.Generic;
using JeffLib;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class BossShoot : Boss, IDisposable
{
	public enum Move
	{
		Move_Default,
		Move_MirrorPlayer,
		Move_OppositePlayer
	}

	public enum ShotType
	{
		ShotType_Straight,
		ShotType_TargetedLinear,
		ShotType_Sine,
		ShotType_Homing,
		ShotType_Volcano,
		ShotType_Any
	}

	public enum SubType
	{
		SubType_SingleShot,
		SubType_MultiShot
	}

	public Transform mGlobalTranform = new Transform();

	protected static int gLastBulletId = 0;

	protected static int PATROL_FLASH_TIMER = 10;

	protected static int PATROL_FLASH_COUNT = 20;

	protected static int DEFAULT_BULLET_SIZE = 40;

	protected static int gVolcanoBulletCounter = 0;

	protected ParamData<int> mDColorVampChanceToMatch2ndBall = new ParamData<int>();

	protected ParamData<int> mDFrogStunTime = new ParamData<int>();

	protected ParamData<int> mDFrogPoisonTime = new ParamData<int>();

	protected ParamData<int> mDFrogHallucinateTime = new ParamData<int>();

	protected ParamData<int> mDFrogSlowTimer = new ParamData<int>();

	protected ParamData<int> mDShotDelay = new ParamData<int>();

	protected ParamData<float> mDFlightSpeed = new ParamData<float>();

	protected ParamData<int> mDFlightMinDist = new ParamData<int>();

	protected ParamData<int> mDColorVampHealthInc = new ParamData<int>();

	protected ParamData<int> mDMinColorChangeTime = new ParamData<int>();

	protected ParamData<int> mDMaxColorChangeTime = new ParamData<int>();

	protected ParamData<float> mDHomingCorrectionAmt = new ParamData<float>();

	protected ParamData<int> mDMinHoverTime = new ParamData<int>();

	protected ParamData<int> mDMaxHoverTime = new ParamData<int>();

	protected ParamData<int> mDMinFireDelay = new ParamData<int>();

	protected ParamData<int> mDMaxFireDelay = new ParamData<int>();

	protected ParamData<float> mDMinBulletSpeed = new ParamData<float>();

	protected ParamData<float> mDMaxBulletSpeed = new ParamData<float>();

	protected ParamData<int> mDMaxBulletsToFire = new ParamData<int>();

	protected ParamData<int> mDMaxRetaliationBullets = new ParamData<int>();

	protected ParamData<int> mDMinSineShotTime = new ParamData<int>();

	protected ParamData<int> mDMaxSineShotTime = new ParamData<int>();

	protected ParamData<float> mDMinAmp = new ParamData<float>();

	protected ParamData<float> mDMaxAmp = new ParamData<float>();

	protected ParamData<float> mDMinFreq = new ParamData<float>();

	protected ParamData<float> mDMaxFreq = new ParamData<float>();

	protected ParamData<float> mDMaxYInc = new ParamData<float>();

	protected ParamData<float> mDMinYInc = new ParamData<float>();

	protected ParamData<float> mDMaxXInc = new ParamData<float>();

	protected ParamData<float> mDMinXInc = new ParamData<float>();

	protected ParamData<float> mDDefaultSpeed = new ParamData<float>();

	protected ParamData<bool> mDStrafe = new ParamData<bool>();

	protected ParamData<bool> mDEndHoverOnHit = new ParamData<bool>();

	protected ParamData<float> mDMinRetalSpeed = new ParamData<float>();

	protected ParamData<float> mDMaxRetalSpeed = new ParamData<float>();

	protected ParamData<int> mDShotType = new ParamData<int>();

	protected ParamData<int> mDTeleportMinTime = new ParamData<int>();

	protected ParamData<int> mDTeleportMaxTime = new ParamData<int>();

	protected ParamData<float> mDMovementAccel = new ParamData<float>();

	protected ParamData<int> mDDefaultMovementUpdateDelay = new ParamData<int>();

	protected ParamData<int> mDMovementMode = new ParamData<int>();

	protected ParamData<bool> mDUseShield = new ParamData<bool>();

	protected ParamData<float> mDShieldRotateSpeed = new ParamData<float>();

	protected ParamData<int> mDShieldQuadRespawnTime = new ParamData<int>();

	protected ParamData<int> mDShieldPauseTime = new ParamData<int>();

	protected ParamData<int> mDShieldHP = new ParamData<int>();

	protected ParamData<int> mDBallShieldDamage = new ParamData<int>();

	protected int mHitEffectYOff;

	protected bool mPauseMovement;

	protected bool mPauseShieldRegen;

	protected bool mDrawHeartsBelowMisc;

	protected bool mDrawHeartsBelowBoss;

	protected List<BossBerserkMovement> mBerserkMovementVec = new List<BossBerserkMovement>();

	protected List<BossBullet> mBullets = new List<BossBullet>();

	protected float mDestX;

	protected float mDestY;

	protected float mTargetDestX;

	protected float mShieldAngle;

	protected float mShieldTargetAngle;

	protected ShieldQuadrant[] mShieldQuadrant = new ShieldQuadrant[4]
	{
		new ShieldQuadrant(),
		new ShieldQuadrant(),
		new ShieldQuadrant(),
		new ShieldQuadrant()
	};

	protected int mShieldRadius;

	protected int mHoverTime;

	protected int mFireDelay;

	protected int mXOff;

	protected int mYOff;

	protected int mCurShieldPauseTime;

	protected int mEndHoverCountdown;

	protected int mEnrageDelayTimer;

	protected int mMovementUpdateDelay;

	protected int mAttackDelayAfterHitFrog;

	public List<Point> mPoints = new List<Point>();

	public int mCurrentLocPoint;

	public int mMaxShotBounces;

	public int mTeleportDir;

	public float mTeleportPct;

	public int mTeleportTime;

	public int mEnrageDelay;

	public int mBombAppearDelay;

	public int mRetalShotDelay;

	public int mStartX;

	public int mStartY;

	public int mEndX;

	public int mEndY;

	public int mDecMinHover;

	public int mDecMaxHover;

	public int mDecMinFire;

	public int mDecMaxFire;

	public int mSubType;

	public int mColorVampShotType;

	public int mColorChangeTimer;

	public int mColorVampHealthIncPerHit;

	public int mIncMaxShotHealthAmt;

	public int mIncRetalMaxShotHealthAmt;

	public int mMaxShotIncCounter;

	public int mRetalShotIncCounter;

	public int mMinSpots;

	public int mMaxSpots;

	public int mMinSpotRad;

	public int mMaxSpotRad;

	public float mMinSpotFade;

	public float mMaxSpotFade;

	public int mInkTargetMode;

	public int mSpotFadeDelay;

	public int mBulletRadius;

	public bool mSinusoidalRetaliation;

	public bool mCanShootBullets;

	public bool mSineShotsTargetPlayer;

	public bool mColorVampire;

	public bool mAvoidColor;

	public bool mEnrageShieldRestore;

	public bool mBulletsUseSphereColl;

	public float mSpeed;

	public int mColorVampChanceToMatch2ndBall
	{
		get
		{
			return mDColorVampChanceToMatch2ndBall.value;
		}
		set
		{
			mDColorVampChanceToMatch2ndBall.value = value;
		}
	}

	public int mFrogStunTime
	{
		get
		{
			return mDFrogStunTime.value;
		}
		set
		{
			mDFrogStunTime.value = value;
		}
	}

	public int mFrogPoisonTime
	{
		get
		{
			return mDFrogPoisonTime.value;
		}
		set
		{
			mDFrogPoisonTime.value = value;
		}
	}

	public int mFrogHallucinateTime
	{
		get
		{
			return mDFrogHallucinateTime.value;
		}
		set
		{
			mDFrogHallucinateTime.value = value;
		}
	}

	public int mFrogSlowTimer
	{
		get
		{
			return mDFrogSlowTimer.value;
		}
		set
		{
			mDFrogSlowTimer.value = value;
		}
	}

	public int mShotDelay
	{
		get
		{
			return mDShotDelay.value;
		}
		set
		{
			mDShotDelay.value = value;
		}
	}

	public float mFlightSpeed
	{
		get
		{
			return mDFlightSpeed.value;
		}
		set
		{
			mDFlightSpeed.value = value;
		}
	}

	public int mFlightMinDist
	{
		get
		{
			return mDFlightMinDist.value;
		}
		set
		{
			mDFlightMinDist.value = value;
		}
	}

	public int mColorVampHealthInc
	{
		get
		{
			return mDColorVampHealthInc.value;
		}
		set
		{
			mDColorVampHealthInc.value = value;
		}
	}

	public int mMinColorChangeTime
	{
		get
		{
			return mDMinColorChangeTime.value;
		}
		set
		{
			mDMinColorChangeTime.value = value;
		}
	}

	public int mMaxColorChangeTime
	{
		get
		{
			return mDMaxColorChangeTime.value;
		}
		set
		{
			mDMaxColorChangeTime.value = value;
		}
	}

	public float mHomingCorrectionAmt
	{
		get
		{
			return mDHomingCorrectionAmt.value;
		}
		set
		{
			mDHomingCorrectionAmt.value = value;
		}
	}

	public int mMinHoverTime
	{
		get
		{
			return mDMinHoverTime.value;
		}
		set
		{
			mDMinHoverTime.value = value;
		}
	}

	public int mMaxHoverTime
	{
		get
		{
			return mDMaxHoverTime.value;
		}
		set
		{
			mDMaxHoverTime.value = value;
		}
	}

	public int mMinFireDelay
	{
		get
		{
			return mDMinFireDelay.value;
		}
		set
		{
			mDMinFireDelay.value = value;
		}
	}

	public int mMaxFireDelay
	{
		get
		{
			return mDMaxFireDelay.value;
		}
		set
		{
			mDMaxFireDelay.value = value;
		}
	}

	public float mMinBulletSpeed
	{
		get
		{
			return mDMinBulletSpeed.value;
		}
		set
		{
			mDMinBulletSpeed.value = value;
		}
	}

	public float mMaxBulletSpeed
	{
		get
		{
			return mDMaxBulletSpeed.value;
		}
		set
		{
			mDMaxBulletSpeed.value = value;
		}
	}

	public int mMaxBulletsToFire
	{
		get
		{
			return mDMaxBulletsToFire.value;
		}
		set
		{
			mDMaxBulletsToFire.value = value;
		}
	}

	public int mMaxRetaliationBullets
	{
		get
		{
			return mDMaxRetaliationBullets.value;
		}
		set
		{
			mDMaxRetaliationBullets.value = value;
		}
	}

	public int mMinSineShotTime
	{
		get
		{
			return mDMinSineShotTime.value;
		}
		set
		{
			mDMinSineShotTime.value = value;
		}
	}

	public int mMaxSineShotTime
	{
		get
		{
			return mDMaxSineShotTime.value;
		}
		set
		{
			mDMaxSineShotTime.value = value;
		}
	}

	public float mMinAmp
	{
		get
		{
			return mDMinAmp.value;
		}
		set
		{
			mDMinAmp.value = value;
		}
	}

	public float mMaxAmp
	{
		get
		{
			return mDMaxAmp.value;
		}
		set
		{
			mDMaxAmp.value = value;
		}
	}

	public float mMinFreq
	{
		get
		{
			return mDMinFreq.value;
		}
		set
		{
			mDMinFreq.value = value;
		}
	}

	public float mMaxFreq
	{
		get
		{
			return mDMaxFreq.value;
		}
		set
		{
			mDMaxFreq.value = value;
		}
	}

	public float mMaxYInc
	{
		get
		{
			return mDMaxYInc.value;
		}
		set
		{
			mDMaxYInc.value = value;
		}
	}

	public float mMinYInc
	{
		get
		{
			return mDMinYInc.value;
		}
		set
		{
			mDMinYInc.value = value;
		}
	}

	public float mMaxXInc
	{
		get
		{
			return mDMaxXInc.value;
		}
		set
		{
			mDMaxXInc.value = value;
		}
	}

	public float mMinXInc
	{
		get
		{
			return mDMinXInc.value;
		}
		set
		{
			mDMinXInc.value = value;
		}
	}

	public float mDefaultSpeed
	{
		get
		{
			return mDDefaultSpeed.value;
		}
		set
		{
			mDDefaultSpeed.value = value;
		}
	}

	public bool mStrafe
	{
		get
		{
			return mDStrafe.value;
		}
		set
		{
			mDStrafe.value = value;
		}
	}

	public bool mEndHoverOnHit
	{
		get
		{
			return mDEndHoverOnHit.value;
		}
		set
		{
			mDEndHoverOnHit.value = value;
		}
	}

	public float mMinRetalSpeed
	{
		get
		{
			return mDMinRetalSpeed.value;
		}
		set
		{
			mDMinRetalSpeed.value = value;
		}
	}

	public float mMaxRetalSpeed
	{
		get
		{
			return mDMaxRetalSpeed.value;
		}
		set
		{
			mDMaxRetalSpeed.value = value;
		}
	}

	public int mShotType
	{
		get
		{
			return mDShotType.value;
		}
		set
		{
			mDShotType.value = value;
		}
	}

	public int mTeleportMinTime
	{
		get
		{
			return mDTeleportMinTime.value;
		}
		set
		{
			mDTeleportMinTime.value = value;
		}
	}

	public int mTeleportMaxTime
	{
		get
		{
			return mDTeleportMaxTime.value;
		}
		set
		{
			mDTeleportMaxTime.value = value;
		}
	}

	public float mMovementAccel
	{
		get
		{
			return mDMovementAccel.value;
		}
		set
		{
			mDMovementAccel.value = value;
		}
	}

	public int mDefaultMovementUpdateDelay
	{
		get
		{
			return mDDefaultMovementUpdateDelay.value;
		}
		set
		{
			mDDefaultMovementUpdateDelay.value = value;
		}
	}

	public int mMovementMode
	{
		get
		{
			return mDMovementMode.value;
		}
		set
		{
			mDMovementMode.value = value;
		}
	}

	public bool mUseShield
	{
		get
		{
			return mDUseShield.value;
		}
		set
		{
			mDUseShield.value = value;
		}
	}

	public float mShieldRotateSpeed
	{
		get
		{
			return mDShieldRotateSpeed.value;
		}
		set
		{
			mDShieldRotateSpeed.value = value;
		}
	}

	public int mShieldQuadRespawnTime
	{
		get
		{
			return mDShieldQuadRespawnTime.value;
		}
		set
		{
			mDShieldQuadRespawnTime.value = value;
		}
	}

	public int mShieldPauseTime
	{
		get
		{
			return mDShieldPauseTime.value;
		}
		set
		{
			mDShieldPauseTime.value = value;
		}
	}

	public int mShieldHP
	{
		get
		{
			return mDShieldHP.value;
		}
		set
		{
			mDShieldHP.value = value;
		}
	}

	public int mBallShieldDamage
	{
		get
		{
			return mDBallShieldDamage.value;
		}
		set
		{
			mDBallShieldDamage.value = value;
		}
	}

	protected void CalcDestX(int min_dist)
	{
		if (mStrafe)
		{
			if (mX < (float)mEndX)
			{
				mDestX = mEndX;
			}
			else
			{
				mDestX = mStartX;
			}
		}
		else
		{
			mDestX = GetMinXDist(min_dist);
		}
		if (mDestX > mX)
		{
			mSpeed = mDefaultSpeed;
		}
		else
		{
			mSpeed = 0f - mDefaultSpeed;
		}
	}

	protected void CalcDestX()
	{
		CalcDestX(100);
	}

	protected void CalcDestY(int min_dist)
	{
		if (mStrafe)
		{
			if (mY < (float)mEndY)
			{
				mDestY = mEndY;
			}
			else
			{
				mDestY = mStartY;
			}
		}
		else
		{
			mDestY = GetMinYDist(min_dist);
		}
		if (mDestY > mY)
		{
			mSpeed = mDefaultSpeed;
		}
		else
		{
			mSpeed = 0f - mDefaultSpeed;
		}
	}

	protected void CalcDestY()
	{
		CalcDestY(100);
	}

	protected int GetMinXDist(int min_dist)
	{
		int num;
		int num2;
		if (mX - (float)min_dist - (float)(mWidth / 2) <= (float)mStartX)
		{
			num = (int)(mX + (float)(mWidth / 2) + (float)min_dist);
			num2 = mEndX;
		}
		else if (mX + (float)min_dist + (float)(mWidth / 2) >= (float)mEndX)
		{
			num2 = (int)(mX - (float)min_dist - (float)(mWidth / 2));
			num = mStartX;
		}
		else if (SexyFramework.Common.Rand() % 100 < 50)
		{
			num = mStartX;
			num2 = (int)(mX - (float)min_dist - (float)(mWidth / 2));
		}
		else
		{
			num = (int)(mX + (float)min_dist + (float)(mWidth / 2));
			num2 = mEndX;
		}
		if (num + mWidth / 2 > mEndX)
		{
			num = mEndX - 10;
		}
		else if (num - mWidth / 2 < mStartX)
		{
			num = mStartX + 10;
		}
		if (num > num2)
		{
			num = num2;
			num2 = num;
		}
		return SexyFramework.Common.IntRange(num, num2);
	}

	protected int GetMinXDist()
	{
		return GetMinXDist(100);
	}

	protected int GetMinYDist(int min_dist)
	{
		int num;
		int num2;
		if (mY - (float)min_dist - (float)(mHeight / 2) <= (float)mStartY)
		{
			num = (int)(mY + (float)(mHeight / 2) + (float)min_dist);
			num2 = mEndY;
		}
		else if (mY + (float)min_dist + (float)(mHeight / 2) >= (float)mEndY)
		{
			num2 = (int)(mY - (float)min_dist - (float)(mHeight / 2));
			num = mStartY;
		}
		else if (SexyFramework.Common.Rand() % 100 < 50)
		{
			num = mStartY;
			num2 = (int)(mY - (float)min_dist - (float)(mHeight / 2));
		}
		else
		{
			num = (int)(mY + (float)min_dist + (float)(mHeight / 2));
			num2 = mEndX;
		}
		if (num > num2)
		{
			num = num2;
			num2 = num;
		}
		return SexyFramework.Common.IntRange(num, num2);
	}

	protected int GetMinYDist()
	{
		return GetMinYDist(100);
	}

	protected bool AtDest()
	{
		if (mSpeed > 0f)
		{
			if (mStartX <= 0 || !(mX >= mDestX))
			{
				if (mStartY > 0)
				{
					return mY >= mDestY;
				}
				return false;
			}
			return true;
		}
		if (mStartX <= 0 || !(mX <= mDestX))
		{
			if (mStartY > 0)
			{
				return mY <= mDestY;
			}
			return false;
		}
		return true;
	}

	protected override bool DoHit(Bullet b, bool from_prox_bomb)
	{
		int num = (int)mHP;
		base.DoHit(b, from_prox_bomb);
		num = (int)((float)num - mHP);
		if (num <= 0)
		{
			return false;
		}
		mMaxShotIncCounter += num;
		if (CanRetaliate())
		{
			mRetalShotIncCounter += num;
		}
		if (mMaxShotIncCounter >= mIncMaxShotHealthAmt && mIncMaxShotHealthAmt > 0)
		{
			mMaxShotIncCounter = 0;
			mMaxBulletsToFire++;
		}
		if (mRetalShotIncCounter >= mIncRetalMaxShotHealthAmt && mIncRetalMaxShotHealthAmt > 0)
		{
			mRetalShotIncCounter = 0;
			mMaxRetaliationBullets++;
		}
		if (mColorVampire)
		{
			int num2 = mColorVampShotType;
			if (mAvoidColor || mColorVampChanceToMatch2ndBall <= 0 || SexyFramework.Common.Rand() % 100 > mColorVampChanceToMatch2ndBall || mLevel.mFrog.GetNextBullet() == null)
			{
				while (num2 == mColorVampShotType)
				{
					num2 = SexyFramework.Common.Rand() % 4;
				}
			}
			else
			{
				num2 = mLevel.mFrog.GetNextBullet().GetColorType();
			}
			mColorVampShotType = num2;
			mColorChangeTimer = SexyFramework.Common.IntRange(mMinColorChangeTime, mMaxColorChangeTime);
		}
		if (mEndHoverOnHit)
		{
			EndHoverOnHit();
		}
		mMinHoverTime -= mDecMinHover;
		mMaxHoverTime -= mDecMaxHover;
		mMinFireDelay -= mDecMinFire;
		mMaxFireDelay -= mDecMaxFire;
		if (mMaxRetaliationBullets > 0 && CanRetaliate() && !IsStunned())
		{
			int num3 = 0;
			for (int i = 0; i < mMaxRetaliationBullets; i++)
			{
				BossBullet bossBullet = new BossBullet();
				mBullets.Add(bossBullet);
				bossBullet.mX = mX;
				bossBullet.mY = mY;
				bossBullet.mId = ++gLastBulletId;
				bossBullet.mDelay = i * mRetalShotDelay;
				if (mSinusoidalRetaliation)
				{
					if (!FireSinusoidalBullet(bossBullet, (mMaxRetaliationBullets == 1) ? (SexyFramework.Common.Rand() % 100 < 50) : ((i + 1) % 2 == 0)))
					{
						mBullets.RemoveAt(mBullets.Count - 1);
						continue;
					}
					bossBullet.mTargetVX = bossBullet.mVX;
					bossBullet.mTargetVY = bossBullet.mVY;
					num3++;
					mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BOSS1_FIRE));
				}
				else
				{
					num3++;
					FireBulletAtPlayer(bossBullet, SexyFramework.Common.FloatRange(mMinRetalSpeed, mMaxRetalSpeed));
					bossBullet.mTargetVX = bossBullet.mVX;
					bossBullet.mTargetVY = bossBullet.mVY;
					mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BOSS1_FIRE));
				}
			}
			PlaySound(2);
			DidRetaliate(num3);
		}
		return true;
	}

	protected void GetTargetedVelocity(float speed, float x, float y, ref float vx, ref float vy)
	{
		float num = SexyFramework.Common.AngleBetweenPoints(x, y, mLevel.mFrog.GetCenterX(), mLevel.mFrog.GetCenterY());
		vx = (float)Math.Cos(num) * speed;
		vy = (0f - (float)Math.Sin(num)) * speed;
	}

	protected float FireBulletAtPlayer(BossBullet b, float speed, float x, float y)
	{
		float num = SexyFramework.Common.AngleBetweenPoints(x, y, mLevel.mFrog.GetCenterX(), mLevel.mFrog.GetCenterY());
		b.mVX = (float)Math.Cos(num) * speed;
		b.mVY = (0f - (float)Math.Sin(num)) * speed;
		b.mSineMotion = false;
		b.mGravity = 0f;
		b.mInitialSpeed = speed;
		return num;
	}

	protected float FireBulletAtPlayer(BossBullet b, float speed)
	{
		return FireBulletAtPlayer(b, speed, mX, mY);
	}

	protected bool FireSinusoidalBullet(BossBullet b, bool negative)
	{
		b.mVX = (b.mVY = 0f);
		if (!mSineShotsTargetPlayer)
		{
			b.mSineMotion = true;
			b.mAmp = SexyFramework.Common.FloatRange(mMinAmp, mMaxAmp);
			b.mFreq = SexyFramework.Common.FloatRange(mMinFreq, mMaxFreq);
			if (negative)
			{
				b.mAmp *= -1f;
			}
			if (mStartX > 0)
			{
				b.mVY = SexyFramework.Common.FloatRange(mMinYInc, mMaxYInc);
			}
			else
			{
				b.mVX = SexyFramework.Common.FloatRange(mMinXInc, mMaxXInc);
			}
			b.mGravity = 0f;
			return true;
		}
		if (mStartY > 0)
		{
			b.mSineMotion = false;
			b.mGravity = Common._M(0.04f);
			if (negative)
			{
				b.mGravity *= -1f;
			}
			int num = SexyFramework.Common.IntRange(mMinSineShotTime, mMaxSineShotTime);
			b.mVX = ((float)mLevel.mFrog.GetCenterX() - mX) / (float)num;
			b.mVY = ((float)mLevel.mFrog.GetCenterY() - mY - 0.5f * b.mGravity * (float)num * (float)num) / (float)num;
			int num2 = (int)Math.Abs(b.mVY / b.mGravity);
			bool flag = true;
			float num3 = b.mVY * (float)num2 + 0.5f * b.mGravity * (float)num2 * (float)num2 + mY;
			while (((num3 < (float)(DEFAULT_BULLET_SIZE * 4) && b.mGravity > 0f) || (num3 > (float)(Common._SS(GameApp.gApp.mHeight) - DEFAULT_BULLET_SIZE * 4) && b.mGravity < 0f)) && num > mMinSineShotTime)
			{
				if (flag)
				{
					flag = false;
					num = mMaxSineShotTime;
				}
				num -= Common._M(5);
				b.mVX = ((float)mLevel.mFrog.GetCenterX() - mX) / (float)num;
				b.mVY = ((float)mLevel.mFrog.GetCenterY() - mY - 0.5f * b.mGravity * (float)num * (float)num) / (float)num;
				num2 = (int)Math.Abs(b.mVY / b.mGravity);
				num3 = b.mVY * (float)num2 + 0.5f * b.mGravity * (float)num2 * (float)num2 + mY;
			}
			if (num < mMinSineShotTime)
			{
				return false;
			}
		}
		else
		{
			b.mSineMotion = false;
			b.mGravity = Common._M(-0.08f);
			if (negative)
			{
				b.mGravity *= -1f;
			}
			int num4 = SexyFramework.Common.IntRange(mMinSineShotTime, mMaxSineShotTime);
			b.mVX = (0f - ((float)mLevel.mFrog.GetCenterX() - mX + 0.5f * b.mGravity * (float)num4 * (float)num4)) / (float)num4;
			b.mVY = ((float)mLevel.mFrog.GetCenterY() - mY) / (float)num4;
			int num5 = (int)Math.Abs(b.mVX / b.mGravity);
			bool flag2 = true;
			float num6 = b.mVX * (float)num5 + 0.5f * b.mGravity * (float)num5 * (float)num5 + mX;
			while (((num6 < (float)(DEFAULT_BULLET_SIZE * 4) && b.mVX < 0f) || (num6 > (float)(Common._SS(GameApp.gApp.mWidth) - DEFAULT_BULLET_SIZE * 4) && b.mVX > 0f)) && num4 > mMinSineShotTime)
			{
				if (flag2)
				{
					flag2 = false;
					num4 = mMaxSineShotTime;
				}
				num4 -= Common._M(5);
				b.mVX = (0f - ((float)mLevel.mFrog.GetCenterX() - mX + 0.5f * b.mGravity * (float)num4 * (float)num4)) / (float)num4;
				b.mVY = ((float)mLevel.mFrog.GetCenterY() - mY) / (float)num4;
				num5 = (int)Math.Abs(b.mVX / b.mGravity);
				num6 = b.mVX * (float)num5 + 0.5f * b.mGravity * (float)num5 * (float)num5 + mX;
			}
			if (num4 <= mMinSineShotTime)
			{
				return false;
			}
		}
		return true;
	}

	protected override void ReInit()
	{
		base.ReInit();
		if (mColorVampire)
		{
			mColorVampShotType = SexyFramework.Common.Rand() % 4;
			mColorChangeTimer = SexyFramework.Common.IntRange(mMinColorChangeTime, mMaxColorChangeTime);
		}
		if (mColorVampHealthInc > 0)
		{
			mColorVampHealthIncPerHit = (int)((float)(Boss.NUM_HEARTS * 4) / (mMaxHP / (float)mColorVampHealthInc));
		}
		if (mStrafe)
		{
			mFireDelay = SexyFramework.Common.IntRange(mMinFireDelay, mMaxFireDelay);
		}
		mSpeed = (float)Math.Sign(mSpeed) * mDefaultSpeed;
		if (mMinRetalSpeed == 0f)
		{
			mMinRetalSpeed = mMinBulletSpeed;
		}
		if (mMaxRetalSpeed == 0f)
		{
			mMaxRetalSpeed = mMaxBulletSpeed;
		}
		mCurShieldPauseTime = mShieldPauseTime;
		if (mTeleportMinTime != 0 && mTeleportMaxTime != 0)
		{
			mTeleportTime = SexyFramework.Common.IntRange(mTeleportMinTime, mTeleportMaxTime);
		}
	}

	protected virtual void DrawBossSpecificArt(Graphics g)
	{
		_ = DEFAULT_BULLET_SIZE;
		_ = DEFAULT_BULLET_SIZE;
		if (!(mHP > 0f) || mDoDeathExplosions)
		{
			return;
		}
		for (int i = 0; i < mBullets.Count; i++)
		{
			if (mBullets[i].mDelay <= 0)
			{
				g.SetColor(Color.White);
				CommonGraphics.DrawCircle(g, Common._S(mBullets[i].mX), Common._S(mBullets[i].mY), Common._S(Common._M(24)), 30);
			}
		}
	}

	protected override void DrawMisc(Graphics g)
	{
		if (mIsBerserk)
		{
			DrawBerserk(g);
		}
		if (mTeleportDir != 0)
		{
			g.PushState();
			int theX = (int)Common._S(mX) - mWidth / 2;
			int theY = (int)Common._S(mY) - mHeight / 2;
			int theWidth = Common._S(mWidth);
			int theHeight = ((mTeleportDir != -1) ? ((int)((float)Common._S(mHeight) * mTeleportPct)) : ((int)((float)Common._S(mHeight) - (float)Common._S(mHeight) * mTeleportPct)));
			g.ClipRect(theX, theY, theWidth, theHeight);
		}
		if (mUseShield)
		{
			DrawShield(g);
		}
		if (mTeleportDir != 0)
		{
			g.PopState();
		}
		base.DrawMisc(g);
	}

	protected virtual bool PreBulletUpdate(BossBullet b, int index)
	{
		if (b.mDelay > 0)
		{
			if (--b.mDelay == 0)
			{
				b.mX = mX;
				b.mY = mY;
				PlaySound(2);
			}
			return true;
		}
		if (b.mOffscreenPause > 0 && b.mY < (float)Common._M(-305))
		{
			if (--b.mOffscreenPause == 0)
			{
				b.mVY *= -1f;
				int centerX = mLevel.mFrog.GetCenterX();
				b.mX = centerX;
			}
			return true;
		}
		return false;
	}

	protected virtual void BulletErased(int index)
	{
	}

	protected virtual void DidFire()
	{
	}

	protected virtual void DidRetaliate(int num_shot)
	{
	}

	protected virtual Rect GetBulletRect(BossBullet b)
	{
		int num = (int)((float)DEFAULT_BULLET_SIZE * 0.75f);
		int num2 = (int)((float)DEFAULT_BULLET_SIZE * 0.75f);
		return new Rect((int)b.mX - num / 2, (int)b.mY - num2 / 2, num, num2);
	}

	protected virtual Rect GetFrogRect()
	{
		int num = Common._M(1);
		return new Rect(mLevel.mFrog.GetCenterX() - num, mLevel.mFrog.GetCenterY() - num, num * 2, num * 2);
	}

	protected virtual bool CanFire()
	{
		return true;
	}

	protected virtual void BulletHitPlayer(BossBullet b)
	{
		PlaySound(4);
	}

	protected void WarpToPoint(bool play_sound)
	{
		if (mPoints.Count != 0)
		{
			int num;
			do
			{
				num = SexyFramework.Common.Rand() % mPoints.Count;
			}
			while (num == mCurrentLocPoint && mPoints.Count > 1);
			mX = mPoints[num].mX;
			mY = mPoints[num].mY;
			mCurrentLocPoint = num;
			if (play_sound)
			{
				PlaySound(8);
			}
		}
	}

	protected void WarpToPoint()
	{
		WarpToPoint(play_sound: true);
	}

	protected void EndHoverOnHit()
	{
		mHoverTime = 0;
		if (mTeleportTime != -1)
		{
			mTeleportDir = -1;
			mTeleportPct = 0f;
			mTeleportTime = 0;
		}
		else if (mPoints.Count == 0)
		{
			mHoverTime = 0;
			mXOff = (mYOff = 0);
			if (mStartX > 0)
			{
				CalcDestX(mFlightMinDist);
			}
			else
			{
				CalcDestY(mFlightMinDist);
			}
			if (mFlightSpeed > 0f)
			{
				mSpeed = mFlightSpeed * (float)Math.Sign(mSpeed);
			}
		}
		else if (mEndHoverCountdown == 0)
		{
			mEndHoverCountdown = Common._M(300);
		}
	}

	protected override void BerserkActivated(int health_limit)
	{
		base.BerserkActivated(health_limit);
		if (mEnrageShieldRestore)
		{
			for (int i = 0; i < 4; i++)
			{
				mShieldQuadrant[i].mTimer = 0;
				mShieldQuadrant[i].mHP = mShieldHP;
			}
		}
		mEnrageDelayTimer = mEnrageDelay;
		for (int j = 0; j < mBerserkMovementVec.Count; j++)
		{
			BossBerserkMovement bossBerserkMovement = mBerserkMovementVec[j];
			if (bossBerserkMovement.mHealthLimit != health_limit)
			{
				continue;
			}
			mStartX = bossBerserkMovement.mStartX;
			mStartY = bossBerserkMovement.mStartY;
			mEndX = bossBerserkMovement.mEndX;
			mEndY = bossBerserkMovement.mEndY;
			bool flag = mPoints.Count > 0;
			mPoints.Clear();
			if (bossBerserkMovement.mPoints.Count > 0)
			{
				mPoints.AddRange(bossBerserkMovement.mPoints.ToArray());
			}
			if (bossBerserkMovement.mX != int.MaxValue)
			{
				SetX(bossBerserkMovement.mX);
			}
			if (bossBerserkMovement.mY != int.MaxValue)
			{
				SetY(bossBerserkMovement.mY);
			}
			if (mPoints.Count == 0)
			{
				mCurrentLocPoint = -1;
				mHoverTime = 0;
				if (mStartY <= 0)
				{
					CalcDestX();
				}
				else
				{
					CalcDestY();
				}
			}
			else if (!flag)
			{
				mHoverTime = SexyFramework.Common.IntRange(mMinHoverTime, mMaxHoverTime);
				WarpToPoint();
			}
			break;
		}
	}

	protected virtual void DrawBerserk(Graphics g)
	{
		if (!(mHP <= 0f) && !mDoDeathExplosions)
		{
			mLevel.mBoard.DoingBossIntro();
		}
	}

	protected virtual void DrawShield(Graphics g)
	{
		if (!(mHP <= 0f) && !mDoDeathExplosions)
		{
			mLevel.mBoard.DoingBossIntro();
		}
	}

	protected virtual BossBullet CreateBossBullet()
	{
		return new BossBullet();
	}

	protected virtual void BossBulletDestroyed(BossBullet b, bool outofscreen)
	{
	}

	protected virtual bool CheckBulletHitPlayer(BossBullet b)
	{
		if (mMaxShotBounces > 0 && b.mBouncesLeft <= 0)
		{
			return false;
		}
		Rect bulletRect = GetBulletRect(b);
		mLevel.mFrog.GetWidth();
		mLevel.mFrog.GetHeight();
		float y = mLevel.mFrog.GetCenterY() - 5;
		float x = mLevel.mFrog.GetCenterX() + 2;
		if (b.mCanHitPlayer && ((mBulletsUseSphereColl && MathUtils.CirclesIntersect(x, y, bulletRect.mX + bulletRect.mWidth / 2, bulletRect.mY + bulletRect.mHeight / 2, 40 + mBulletRadius)) || (!mBulletsUseSphereColl && bulletRect.Intersects(GetFrogRect()))))
		{
			return true;
		}
		return false;
	}

	protected virtual void ShieldQuadrantHit(int quad)
	{
	}

	protected virtual bool CanRetaliate()
	{
		return true;
	}

	protected virtual void GetShotBounceOffs(BossBullet b, ref int x, ref int y)
	{
		x = 0;
		y = 0;
	}

	protected virtual void QuadHitByProxBomb(int quad)
	{
	}

	protected virtual void ShotBounced(BossBullet b)
	{
	}

	protected virtual void AppliedSlowTimer()
	{
	}

	protected void CopyFrom(BossShoot rhs)
	{
		CopyFrom((Boss)rhs);
		mHitEffectYOff = rhs.mHitEffectYOff;
		mPauseMovement = rhs.mPauseMovement;
		mPauseShieldRegen = rhs.mPauseShieldRegen;
		mDrawHeartsBelowMisc = rhs.mDrawHeartsBelowMisc;
		mDrawHeartsBelowBoss = rhs.mDrawHeartsBelowBoss;
		mDestX = rhs.mDestX;
		mDestY = rhs.mDestY;
		mTargetDestX = rhs.mTargetDestX;
		mShieldAngle = rhs.mShieldAngle;
		mShieldTargetAngle = rhs.mShieldTargetAngle;
		mShieldRadius = rhs.mShieldRadius;
		mHoverTime = rhs.mHoverTime;
		mFireDelay = rhs.mFireDelay;
		mXOff = rhs.mXOff;
		mYOff = rhs.mYOff;
		mCurShieldPauseTime = rhs.mCurShieldPauseTime;
		mEndHoverCountdown = rhs.mEndHoverCountdown;
		mEnrageDelayTimer = rhs.mEnrageDelayTimer;
		mMovementUpdateDelay = rhs.mMovementUpdateDelay;
		mAttackDelayAfterHitFrog = rhs.mAttackDelayAfterHitFrog;
		mCurrentLocPoint = rhs.mCurrentLocPoint;
		mMaxShotBounces = rhs.mMaxShotBounces;
		mShieldPauseTime = rhs.mShieldPauseTime;
		mShieldRotateSpeed = rhs.mShieldRotateSpeed;
		mUseShield = rhs.mUseShield;
		mShieldQuadRespawnTime = rhs.mShieldQuadRespawnTime;
		mShieldHP = rhs.mShieldHP;
		mBallShieldDamage = rhs.mBallShieldDamage;
		mTeleportDir = rhs.mTeleportDir;
		mTeleportPct = rhs.mTeleportPct;
		mTeleportMinTime = rhs.mTeleportMinTime;
		mTeleportMaxTime = rhs.mTeleportMaxTime;
		mTeleportTime = rhs.mTeleportTime;
		mEnrageDelay = rhs.mEnrageDelay;
		mBombAppearDelay = rhs.mBombAppearDelay;
		mShotDelay = rhs.mShotDelay;
		mBombAppearDelay = rhs.mBombAppearDelay;
		mShotType = rhs.mShotType;
		mStartX = rhs.mStartX;
		mStartY = rhs.mStartY;
		mEndX = rhs.mEndX;
		mEndY = rhs.mEndY;
		mMinHoverTime = rhs.mMinHoverTime;
		mMaxHoverTime = rhs.mMaxHoverTime;
		mMinFireDelay = rhs.mMinFireDelay;
		mMaxFireDelay = rhs.mMaxFireDelay;
		mFrogStunTime = rhs.mFrogStunTime;
		mFrogPoisonTime = rhs.mFrogPoisonTime;
		mFrogHallucinateTime = rhs.mFrogHallucinateTime;
		mDecMinHover = rhs.mDecMinHover;
		mDecMaxHover = rhs.mDecMaxHover;
		mDecMinFire = rhs.mDecMinFire;
		mDecMaxFire = rhs.mDecMaxFire;
		mSubType = rhs.mSubType;
		mMaxBulletsToFire = rhs.mMaxBulletsToFire;
		mMaxRetaliationBullets = rhs.mMaxRetaliationBullets;
		mMinSineShotTime = rhs.mMinSineShotTime;
		mColorVampShotType = rhs.mColorVampShotType;
		mColorChangeTimer = rhs.mColorChangeTimer;
		mMinColorChangeTime = rhs.mMinColorChangeTime;
		mMaxColorChangeTime = rhs.mMaxColorChangeTime;
		mColorVampHealthInc = rhs.mColorVampHealthInc;
		mColorVampHealthIncPerHit = rhs.mColorVampHealthIncPerHit;
		mIncMaxShotHealthAmt = rhs.mIncMaxShotHealthAmt;
		mIncRetalMaxShotHealthAmt = rhs.mIncRetalMaxShotHealthAmt;
		mMaxShotIncCounter = rhs.mMaxShotIncCounter;
		mRetalShotIncCounter = rhs.mRetalShotIncCounter;
		mDefaultMovementUpdateDelay = rhs.mDefaultMovementUpdateDelay;
		mMovementMode = rhs.mMovementMode;
		mMovementAccel = rhs.mMovementAccel;
		mMinSpots = rhs.mMinSpots;
		mMaxSpots = rhs.mMaxSpots;
		mMinSpotRad = rhs.mMinSpotRad;
		mMaxSpotRad = rhs.mMaxSpotRad;
		mMinSpotFade = rhs.mMinSpotFade;
		mMaxSpotFade = rhs.mMaxSpotFade;
		mInkTargetMode = rhs.mInkTargetMode;
		mSpotFadeDelay = rhs.mSpotFadeDelay;
		mFlightSpeed = rhs.mFlightSpeed;
		mHomingCorrectionAmt = rhs.mHomingCorrectionAmt;
		mFlightMinDist = rhs.mFlightMinDist;
		mColorVampChanceToMatch2ndBall = rhs.mColorVampChanceToMatch2ndBall;
		mBulletRadius = rhs.mBulletRadius;
		mSinusoidalRetaliation = rhs.mSinusoidalRetaliation;
		mCanShootBullets = rhs.mCanShootBullets;
		mSineShotsTargetPlayer = rhs.mSineShotsTargetPlayer;
		mEndHoverOnHit = rhs.mEndHoverOnHit;
		mColorVampire = rhs.mColorVampire;
		mAvoidColor = rhs.mAvoidColor;
		mStrafe = rhs.mStrafe;
		mEnrageShieldRestore = rhs.mEnrageShieldRestore;
		mBulletsUseSphereColl = rhs.mBulletsUseSphereColl;
		mMinBulletSpeed = rhs.mMinBulletSpeed;
		mMaxBulletSpeed = rhs.mMaxBulletSpeed;
		mMinRetalSpeed = rhs.mMinRetalSpeed;
		mMaxRetalSpeed = rhs.mMaxRetalSpeed;
		mSpeed = rhs.mSpeed;
		mDefaultSpeed = rhs.mDefaultSpeed;
		mVolcanoOffscreenDelay = rhs.mVolcanoOffscreenDelay;
		mMinAmp = rhs.mMinAmp;
		mMaxAmp = rhs.mMaxAmp;
		mMinYInc = rhs.mMinYInc;
		mMaxYInc = rhs.mMaxYInc;
		mMinXInc = rhs.mMinXInc;
		mMaxXInc = rhs.mMaxXInc;
		mMinFreq = rhs.mMinFreq;
		mMaxFreq = rhs.mMaxFreq;
		mFrogSlowTimer = rhs.mFrogSlowTimer;
		for (int i = 0; i < 4; i++)
		{
			mShieldQuadrant[i] = rhs.mShieldQuadrant[i];
		}
		mPoints.Clear();
		for (int j = 0; j < rhs.mPoints.Count; j++)
		{
			mPoints.Add(new Point(rhs.mPoints[j]));
		}
		mBerserkMovementVec.Clear();
		for (int k = 0; k < rhs.mBerserkMovementVec.Count; k++)
		{
			mBerserkMovementVec.Add(new BossBerserkMovement(rhs.mBerserkMovementVec[k]));
		}
		mBullets.Clear();
	}

	public BossShoot(Level l)
		: base(l)
	{
		mDestX = 0f;
		mDestY = 0f;
		mSpeed = 0f;
		mHoverTime = 0;
		mFireDelay = 0;
		mStartX = 0;
		mEndX = 0;
		mStartY = 0;
		mEndY = 0;
		mMinHoverTime = 0;
		mMaxHoverTime = 0;
		mPauseShieldRegen = false;
		mMinFireDelay = 0;
		mMaxFireDelay = 0;
		mMinBulletSpeed = 0f;
		mMaxBulletSpeed = 0f;
		mFrogStunTime = 0;
		mDecMinHover = 0;
		mDecMaxHover = 0;
		mDecMinFire = 0;
		mDecMaxFire = 0;
		mXOff = 0;
		mYOff = 0;
		mSubType = 0;
		mMaxBulletsToFire = 1;
		mMaxRetaliationBullets = 0;
		mSinusoidalRetaliation = false;
		mMinAmp = 2f;
		mMaxAmp = 4f;
		mMinYInc = 2f;
		mMaxYInc = 4f;
		mMinXInc = 2f;
		mMaxXInc = 4f;
		mMinFreq = 0.04f;
		mMaxFreq = 0.04f;
		mPauseMovement = false;
		mCanShootBullets = false;
		mSineShotsTargetPlayer = false;
		mMinSineShotTime = 200;
		mMaxSineShotTime = 400;
		mShotType = 5;
		mEndHoverOnHit = false;
		mColorVampShotType = -1;
		mColorVampire = false;
		mAvoidColor = false;
		mMinColorChangeTime = 0;
		mMaxColorChangeTime = 0;
		mColorChangeTimer = 0;
		mColorVampHealthInc = 0;
		mColorVampHealthIncPerHit = 0;
		mStrafe = false;
		mMaxShotIncCounter = 0;
		mRetalShotIncCounter = 0;
		mIncMaxShotHealthAmt = 0;
		mIncRetalMaxShotHealthAmt = 0;
		mRetalShotDelay = 0;
		mMinRetalSpeed = 0f;
		mMaxRetalSpeed = 0f;
		mColorVampChanceToMatch2ndBall = 0;
		mFrogPoisonTime = 0;
		mFlightSpeed = 0f;
		mFlightMinDist = 100;
		mFrogHallucinateTime = 0;
		mHomingCorrectionAmt = 0.05f;
		mFrogSlowTimer = 0;
		mBombAppearDelay = 0;
		mCurrentLocPoint = -1;
		mUseShield = false;
		mShieldPauseTime = 0;
		mShieldRotateSpeed = 0f;
		mCurShieldPauseTime = 0;
		mShieldAngle = 0f;
		mShieldTargetAngle = 0f;
		mShieldQuadRespawnTime = 0;
		mBallShieldDamage = 0;
		mShieldHP = 1;
		mEnrageShieldRestore = false;
		mEndHoverCountdown = 0;
		mMinSpotRad = 0;
		mMaxSpotRad = 0;
		mMinSpots = 0;
		mMaxSpots = 0;
		mMinSpotFade = 0f;
		mMaxSpotFade = 0f;
		mSpotFadeDelay = 0;
		mInkTargetMode = 0;
		mEnrageDelay = 0;
		mEnrageDelayTimer = 0;
		mMovementMode = 0;
		mMovementAccel = 999999f;
		mDefaultMovementUpdateDelay = 0;
		mMovementUpdateDelay = 0;
		mTargetDestX = 0f;
		mVolcanoOffscreenDelay = 0;
		mBulletsUseSphereColl = true;
		mBulletRadius = 0;
		mTeleportPct = 0f;
		mTeleportTime = -1;
		mTeleportMinTime = 0;
		mTeleportMaxTime = 0;
		mTeleportDir = 0;
		mShieldRadius = 120;
		mDrawHeartsBelowMisc = true;
		mAttackDelayAfterHitFrog = 0;
		mMaxShotBounces = 0;
		mHitEffectYOff = 40;
		mDrawHeartsBelowBoss = false;
		mBossRadius = 24;
		mWidth = 146;
		mHeight = 124;
		mBullets.Reserve(100);
	}

	public BossShoot()
		: this(null)
	{
	}

	public override void Dispose()
	{
	}

	public virtual void DeleteAllBullets()
	{
		for (int i = 0; i < mBullets.Count; i++)
		{
			BossBulletDestroyed(mBullets[i], outofscreen: true);
			BulletErased(i);
		}
		mBullets.Clear();
	}

	public virtual void SetDestX(float dx)
	{
		mTargetDestX = dx;
		if (mMovementUpdateDelay <= 0)
		{
			mMovementUpdateDelay = mDefaultMovementUpdateDelay;
		}
	}

	public virtual void AddBerserkMovement(BossBerserkMovement bbm)
	{
		mBerserkMovementVec.Add(bbm);
	}

	public List<BossBerserkMovement> getBerserkMovementList()
	{
		return mBerserkMovementVec;
	}

	public override void PostInstantiationHook(Boss source_boss)
	{
		base.PostInstantiationHook(source_boss);
		AddParamPointer("ColorHelp", mDColorVampChanceToMatch2ndBall);
		AddParamPointer("FrogStun", mDFrogStunTime);
		AddParamPointer("stun", mDFrogStunTime);
		AddParamPointer("FrogPoison", mDFrogPoisonTime);
		AddParamPointer("FrogHallucinate", mDFrogHallucinateTime);
		AddParamPointer("hallucinate", mDFrogHallucinateTime);
		AddParamPointer("poison", mDFrogPoisonTime);
		AddParamPointer("SlowShot", mDFrogSlowTimer);
		AddParamPointer("ShotDelay", mDShotDelay);
		AddParamPointer("flightspeed", mDFlightSpeed);
		AddParamPointer("minflightdist", mDFlightMinDist);
		AddParamPointer("VampHealthInc", mDColorVampHealthInc);
		AddParamPointer("VampColorChangeMin", mDMinColorChangeTime);
		AddParamPointer("VampColorChangeMax", mDMaxColorChangeTime);
		AddParamPointer("HomingSpeed", mDHomingCorrectionAmt);
		AddParamPointer("MinHover", mDMinHoverTime);
		AddParamPointer("MaxHover", mDMaxHoverTime);
		AddParamPointer("MinFire", mDMinFireDelay);
		AddParamPointer("MaxFire", mDMaxFireDelay);
		AddParamPointer("MinBullet", mDMinBulletSpeed);
		AddParamPointer("MaxBullet", mDMaxBulletSpeed);
		AddParamPointer("MaxBullets", mDMaxBulletsToFire);
		AddParamPointer("Retaliation", mDMaxRetaliationBullets);
		AddParamPointer("MinSineShotTime", mDMinSineShotTime);
		AddParamPointer("MaxSineShotTime", mDMaxSineShotTime);
		AddParamPointer("MinAmp", mDMinAmp);
		AddParamPointer("MaxAmp", mDMaxAmp);
		AddParamPointer("MinFreq", mDMinFreq);
		AddParamPointer("MaxFreq", mDMaxFreq);
		AddParamPointer("MaxSineYInc", mDMaxYInc);
		AddParamPointer("MinSineYInc", mDMinYInc);
		AddParamPointer("MaxSineXInc", mDMaxXInc);
		AddParamPointer("MinSineXInc", mDMinXInc);
		AddParamPointer("MoveSpeed", mDDefaultSpeed);
		AddParamPointer("Strafe", mDStrafe);
		AddParamPointer("EndHoverOnHit", mDEndHoverOnHit);
		AddParamPointer("RetalSpeedMin", mDMinRetalSpeed);
		AddParamPointer("RetalSpeedMax", mDMaxRetalSpeed);
		AddParamPointer("ShotType", mDShotType);
		AddParamPointer("TeleportMinTime", mDTeleportMinTime);
		AddParamPointer("TeleportMaxTime", mDTeleportMaxTime);
		AddParamPointer("Accel", mDMovementAccel);
		AddParamPointer("MoveDelay", mDDefaultMovementUpdateDelay);
		AddParamPointer("MoveMode", mDMovementMode);
		AddParamPointer("UseShield", mDUseShield);
		AddParamPointer("ShieldRotSpeed", mDShieldRotateSpeed);
		AddParamPointer("ShieldRespawnTime", mDShieldQuadRespawnTime);
		AddParamPointer("ShieldPauseTime", mDShieldPauseTime);
		AddParamPointer("ShieldHP", mDShieldHP);
		AddParamPointer("BallShieldDamage", mDBallShieldDamage);
	}

	public override void Init(Level l)
	{
		base.Init(l);
		if (mMovementMode == 0)
		{
			if (mPoints.Count == 0)
			{
				if (mStartY <= 0)
				{
					mX = (float)(mEndX - mStartX) / 2f + (float)mStartX;
					CalcDestX();
				}
				else
				{
					mY = (float)(mEndY - mStartY) / 2f + (float)mStartY;
					CalcDestY();
				}
			}
			else
			{
				WarpToPoint(play_sound: false);
				mHoverTime = SexyFramework.Common.IntRange(mMinHoverTime, mMaxHoverTime);
			}
		}
		else
		{
			mTargetDestX = (mDestX = (mX = (float)Common._SS(GameApp.gApp.mWidth) / 2f - (float)GameApp.gApp.mBoardOffsetX));
		}
		for (int i = 0; i < 4; i++)
		{
			mShieldQuadrant[i].mHP = mShieldHP;
		}
		ReInit();
	}

	public override void Update(float f)
	{
		gVolcanoBulletCounter = 0;
		base.Update(f);
		bool flag = AtDest();
		if (mDoDeathExplosions || mHP <= 0f)
		{
			return;
		}
		if (mTeleportTime > 0)
		{
			mTeleportTime--;
		}
		bool flag2 = mLevel.AllCurvesAtRolloutPoint();
		if (mTeleportTime == 0 && SexyFramework.Common._geq(mAlphaOverride, 255f) && mFireDelay > 0 && !IsStunned() && flag2 && mTeleportDir == 0)
		{
			mTeleportDir = -1;
			mTeleportPct = 0f;
		}
		if (mTeleportDir != 0)
		{
			mTeleportPct += Common._M(0.05f);
			if (mTeleportPct >= 1f)
			{
				mTeleportPct = 0f;
				if (mTeleportDir == -1)
				{
					mTeleportDir = 1;
					mX = GetMinXDist();
					CalcDestX();
				}
				else
				{
					mTeleportTime = SexyFramework.Common.IntRange(mTeleportMinTime, mTeleportMaxTime);
					mTeleportDir = 0;
				}
			}
		}
		if (!IsStunned() && flag2 && SexyFramework.Common._geq(mAlphaOverride, 255f))
		{
			if (IsImpatient())
			{
				float num = Common._M(0.0003f);
				mMinBulletSpeed += num;
				mMaxBulletSpeed += num;
				mMinRetalSpeed += num;
				mMaxRetalSpeed += num;
				if (mFireDelay > 100)
				{
					mFireDelay--;
				}
			}
			if (mUseShield)
			{
				if (mShieldPauseTime > 0 && mCurShieldPauseTime > 0 && --mCurShieldPauseTime == 0)
				{
					mShieldTargetAngle = mShieldAngle + 1.570795f;
				}
				if (mShieldPauseTime > 0 && mCurShieldPauseTime == 0 && mShieldAngle < mShieldTargetAngle)
				{
					mShieldAngle += 1.570795f / (float)Common._M(25);
					if (mShieldAngle > mShieldTargetAngle)
					{
						mCurShieldPauseTime = mShieldPauseTime;
						mShieldAngle = Common.GetCanonicalAngleRad(mShieldTargetAngle);
					}
				}
				if (mShieldPauseTime == 0 && (mShieldAngle += mShieldRotateSpeed) > 6.28318f)
				{
					mShieldAngle = Common.GetCanonicalAngleRad(mShieldAngle);
				}
				if (!mPauseShieldRegen)
				{
					for (int i = 0; i < 4; i++)
					{
						if (mShieldQuadrant[i].mTimer > 0)
						{
							mShieldQuadrant[i].mTimer--;
						}
					}
				}
			}
			if (mColorChangeTimer > 0 && mColorVampire && --mColorChangeTimer == 0)
			{
				int num2;
				for (num2 = mColorVampShotType; num2 == mColorVampShotType; num2 = SexyFramework.Common.Rand() % 4)
				{
				}
				mColorVampShotType = num2;
				mColorChangeTimer = SexyFramework.Common.IntRange(mMinColorChangeTime, mMaxColorChangeTime);
			}
			if (mTeleportDir == 0 && !mPauseMovement)
			{
				if (mMovementMode != 0)
				{
					if (mMovementUpdateDelay >= 0 && --mMovementUpdateDelay <= 0)
					{
						mDestX = mTargetDestX;
					}
					if (!SexyFramework.Common._eq(mX, mDestX))
					{
						float num3 = mX;
						if (mX < mDestX)
						{
							mX += mMovementAccel;
						}
						else
						{
							mX -= mMovementAccel;
						}
						if ((num3 < mDestX && mX >= mDestX) || (num3 > mDestX && mX <= mDestX))
						{
							mX = mDestX;
						}
					}
				}
				if (mHoverTime == 0 && mEndHoverCountdown == 0 && mEnrageDelayTimer == 0 && !flag && !mStrafe && mMovementMode == 0)
				{
					if (mStartX > 0)
					{
						mX += mSpeed;
					}
					else
					{
						mY += mSpeed;
					}
					if (AtDest())
					{
						if (mStartX > 0)
						{
							mX = mDestX;
						}
						else
						{
							mY = mDestY;
						}
						mHoverTime = SexyFramework.Common.IntRange(mMinHoverTime, mMaxHoverTime);
						mFireDelay = SexyFramework.Common.IntRange(mMinFireDelay, mMaxFireDelay);
						if (mFireDelay > mHoverTime)
						{
							mFireDelay = mHoverTime / 2;
						}
					}
				}
				else if ((mHoverTime > 0 || mStrafe || mMovementMode != 0) && mEndHoverCountdown == 0 && mEnrageDelayTimer == 0)
				{
					if (!mStrafe)
					{
						mXOff = (int)((double)Common._M(-9) * Math.Cos((float)(Common._M1(1) * mUpdateCount) * 3.14159f / 180f) + (double)Common._M2(2));
						mYOff = (int)((double)Common._M(6) * Math.Sin((float)(Common._M1(2) * mUpdateCount) * 3.14159f / 180f) + (double)Common._M2(3));
					}
					else if (mPoints.Count == 0)
					{
						if (mStartX > 0)
						{
							mX += mSpeed;
						}
						else
						{
							mY += mSpeed;
						}
						if (AtDest())
						{
							if (mStartX > 0)
							{
								mX = mDestX;
								CalcDestX();
							}
							else
							{
								mY = mDestY;
								CalcDestY();
							}
						}
					}
					if (!mStrafe && mMovementMode == 0 && --mHoverTime == 0)
					{
						if (mPoints.Count == 0)
						{
							mXOff = (mYOff = 0);
							if (mStartX > 0)
							{
								CalcDestX();
							}
							else
							{
								CalcDestY();
							}
						}
						else
						{
							WarpToPoint();
							mHoverTime = SexyFramework.Common.IntRange(mMinHoverTime, mMaxHoverTime);
						}
					}
					if (mFireDelay > 0)
					{
						mFireDelay--;
					}
					if (mAttackDelayAfterHitFrog > 0)
					{
						mAttackDelayAfterHitFrog--;
					}
					bool flag3 = (GameApp.gApp.GetLevelMgr().mBossesCanAttackFuckedFrog || !mLevel.mFrog.IsFuckedUp()) && mAttackDelayAfterHitFrog == 0;
					if (mFireDelay == 0 && flag3 && CanFire())
					{
						if (mStrafe || mMovementMode != 0)
						{
							mFireDelay = SexyFramework.Common.IntRange(mMinFireDelay, mMaxFireDelay);
						}
						BossBullet bossBullet = null;
						BossBullet bossBullet2 = null;
						if (mSubType == 0)
						{
							bossBullet = CreateBossBullet();
							mBullets.Add(bossBullet);
							bossBullet.mBossShoot = this;
							bossBullet.mX = mX;
							bossBullet.mY = mY;
							FireBulletAtPlayer(bossBullet, SexyFramework.Common.FloatRange(mMinBulletSpeed, mMaxBulletSpeed));
							bossBullet.mId = ++gLastBulletId;
						}
						else
						{
							List<int> list = new List<int>();
							if (mShotType == 5)
							{
								for (int j = 0; j < 5; j++)
								{
									list.Add(j);
								}
							}
							else
							{
								for (int k = 0; k < mMaxBulletsToFire; k++)
								{
									list.Add(mShotType);
								}
							}
							int num4 = 0;
							while (num4 < mMaxBulletsToFire)
							{
								int index = SexyFramework.Common.Rand() % list.Count;
								int num5 = list[index];
								list.RemoveAt(index);
								num4++;
								bossBullet = CreateBossBullet();
								mBullets.Add(bossBullet);
								if (bossBullet2 == null)
								{
									bossBullet2 = bossBullet;
								}
								bossBullet.mBossShoot = this;
								bossBullet.mX = mX;
								bossBullet.mY = mY;
								bossBullet.mId = ++gLastBulletId;
								bossBullet.mDelay = (num4 - 1) * mShotDelay;
								bossBullet.mShotType = num5;
								bossBullet.mBouncesLeft = mMaxShotBounces;
								switch (num5)
								{
								case 1:
								case 3:
									FireBulletAtPlayer(bossBullet, SexyFramework.Common.FloatRange(mMinBulletSpeed, mMaxBulletSpeed));
									bossBullet.mHoming = num5 == 3;
									bossBullet.mTargetVX = bossBullet.mVX;
									bossBullet.mTargetVY = bossBullet.mVY;
									continue;
								case 2:
									if (!FireSinusoidalBullet(bossBullet, num5 == 1))
									{
										if (bossBullet2 == bossBullet)
										{
											bossBullet2 = null;
										}
										bossBullet = null;
										mBullets.RemoveAt(mBullets.Count - 1);
										continue;
									}
									break;
								}
								switch (num5)
								{
								case 0:
									bossBullet.mSineMotion = false;
									if (mStartY > 0)
									{
										bossBullet.mVX = SexyFramework.Common.FloatRange(mMinBulletSpeed, mMaxBulletSpeed);
										bossBullet.mVY = 0f;
										if (mX > (float)mLevel.mFrog.GetCenterX())
										{
											bossBullet.mVX *= -1f;
										}
									}
									else
									{
										bossBullet.mVX = 0f;
										bossBullet.mVY = SexyFramework.Common.FloatRange(mMinBulletSpeed, mMaxBulletSpeed);
										if (mY > (float)mLevel.mFrog.GetCenterY())
										{
											bossBullet.mVY *= -1f;
										}
									}
									break;
								case 4:
									bossBullet.mVX = 0f;
									bossBullet.mVY = 0f - SexyFramework.Common.FloatRange(mMinBulletSpeed, mMinBulletSpeed);
									bossBullet.mOffscreenPause = mVolcanoOffscreenDelay;
									bossBullet.mVolcanoShot = true;
									break;
								}
							}
						}
						if (bossBullet != null)
						{
							bossBullet.mUpdateCount = 0;
							mFireDelay = SexyFramework.Common.IntRange(mMinFireDelay, mMaxFireDelay);
							if (bossBullet2.mDelay == 0)
							{
								PlaySound(2);
							}
							DidFire();
						}
					}
				}
			}
		}
		if (!SexyFramework.Common._geq(mAlphaOverride, 255f))
		{
			return;
		}
		for (int l = 0; l < mBullets.Count; l++)
		{
			BossBullet bossBullet3 = mBullets[l];
			if (bossBullet3.mDeleteInstantly)
			{
				BossBulletDestroyed(bossBullet3, outofscreen: false);
				mBullets.RemoveAt(l);
				BulletErased(l);
				l--;
			}
			else
			{
				if (PreBulletUpdate(bossBullet3, l))
				{
					continue;
				}
				bossBullet3.mUpdateCount++;
				if (mStartY > 0)
				{
					bossBullet3.mVY += bossBullet3.mGravity;
				}
				else
				{
					bossBullet3.mVX += bossBullet3.mGravity;
				}
				if (!bossBullet3.mSineMotion)
				{
					float num6 = bossBullet3.mX;
					float num7 = bossBullet3.mY;
					bossBullet3.mX += bossBullet3.mVX;
					bossBullet3.mY += bossBullet3.mVY;
					if (mMaxShotBounces > 0)
					{
						bool flag4 = false;
						int x = 0;
						int y = 0;
						GetShotBounceOffs(bossBullet3, ref x, ref y);
						if (num7 + (float)y < (float)Common._SS(GameApp.gApp.mHeight) && bossBullet3.mY + (float)y >= (float)Common._SS(GameApp.gApp.mHeight) && bossBullet3.mVY > 0f)
						{
							bossBullet3.mVY = 0f - Math.Abs(bossBullet3.mVY);
							bossBullet3.mY = num7;
							flag4 = true;
						}
						else if (num7 > (float)Common._DS(Common._M(-50)) && bossBullet3.mY <= (float)Common._DS(Common._M1(-50)) && bossBullet3.mVY < 0f)
						{
							bossBullet3.mVY = Math.Abs(bossBullet3.mVY);
							bossBullet3.mY = num7;
							flag4 = true;
						}
						if (num6 + (float)x < (float)Common._SS(GameApp.gApp.mWidth) && bossBullet3.mX + (float)x >= (float)Common._SS(GameApp.gApp.mWidth) && bossBullet3.mVX > 0f)
						{
							bossBullet3.mVX = 0f - Math.Abs(bossBullet3.mVX);
							bossBullet3.mX = num6;
							flag4 = true;
						}
						else if (num6 > (float)Common._DS(Common._M(-40)) && bossBullet3.mX <= (float)Common._DS(Common._M1(-40)) && bossBullet3.mVX < 0f)
						{
							bossBullet3.mVX = Math.Abs(bossBullet3.mVX);
							bossBullet3.mX = num6;
							flag4 = true;
						}
						if (mMaxShotBounces > 0 && bossBullet3.mBouncesLeft <= 0)
						{
							continue;
						}
						if (flag4)
						{
							bossBullet3.mTargetVX = bossBullet3.mVX;
							bossBullet3.mTargetVY = bossBullet3.mVY;
							bossBullet3.mBouncesLeft--;
							ShotBounced(bossBullet3);
						}
						if (bossBullet3.mBouncesLeft <= 0)
						{
							continue;
						}
					}
				}
				else if (mStartX > 0)
				{
					bossBullet3.mX += bossBullet3.mAmp * (float)Math.Cos((float)bossBullet3.mUpdateCount * bossBullet3.mFreq);
					bossBullet3.mY += bossBullet3.mVY;
				}
				else
				{
					bossBullet3.mY += bossBullet3.mAmp * (float)Math.Cos((float)bossBullet3.mUpdateCount * bossBullet3.mFreq);
					bossBullet3.mX += bossBullet3.mVX;
				}
				if (bossBullet3.mHoming && bossBullet3.mY < (float)mLevel.mFrogY[0])
				{
					float vy = 0f;
					GetTargetedVelocity(bossBullet3.mInitialSpeed, bossBullet3.mX, bossBullet3.mY, ref bossBullet3.mTargetVX, ref vy);
				}
				if (!SexyFramework.Common._eq(bossBullet3.mVX, bossBullet3.mTargetVX))
				{
					bool flag5 = bossBullet3.mVX < bossBullet3.mTargetVX;
					bossBullet3.mVX += mHomingCorrectionAmt * (float)((bossBullet3.mVX < bossBullet3.mTargetVX) ? 1 : (-1));
					if ((flag5 && bossBullet3.mVX > bossBullet3.mTargetVX) || (!flag5 && bossBullet3.mVX < bossBullet3.mTargetVX))
					{
						bossBullet3.mVX = bossBullet3.mTargetVX;
					}
				}
				Rect bulletRect = GetBulletRect(bossBullet3);
				if (CheckBulletHitPlayer(bossBullet3))
				{
					mAttackDelayAfterHitFrog = GameApp.gApp.GetLevelMgr().mAttackDelayAfterHittingFrog;
					mHulaAmnesty = mAttackDelayAfterHitFrog;
					if (mFrogStunTime > 0)
					{
						mLevel.mFrog.Stun(mFrogStunTime);
					}
					else if (mFrogPoisonTime > 0)
					{
						mLevel.mFrog.Poison(mFrogPoisonTime);
					}
					else if (mFrogHallucinateTime > 0)
					{
						mLevel.mBoard.SetHallucinateTimer(mFrogHallucinateTime);
					}
					else if (mFrogSlowTimer > 0)
					{
						mLevel.mFrog.SetSlowTimer(mFrogSlowTimer);
						AppliedSlowTimer();
					}
					else if (mMinSpots > 0 && mMaxSpots > 0 && mMinSpotRad > 0 && mMaxSpotRad > 0)
					{
						for (int m = 0; m < mLevel.mNumCurves; m++)
						{
							mLevel.mCurveMgr[m].AddInkSpots(SexyFramework.Common.IntRange(mMinSpots, mMaxSpots), mMinSpotRad, mMaxSpotRad, mMinSpotFade, mMaxSpotFade, mSpotFadeDelay, mInkTargetMode);
						}
					}
					BulletHitPlayer(bossBullet3);
					BossBulletDestroyed(bossBullet3, outofscreen: false);
					mBullets.RemoveAt(l);
					BulletErased(l);
					l--;
				}
				else if ((!bulletRect.Intersects(new Rect(0, 0, Common._SS(GameApp.gApp.mWidth), Common._SS(GameApp.gApp.mHeight + 200))) && !bossBullet3.mVolcanoShot && mMaxShotBounces == 0) || (bossBullet3.mVolcanoShot && bossBullet3.mY > (float)(Common._SS(GameApp.gApp.mHeight) + 300)))
				{
					BossBulletDestroyed(bossBullet3, outofscreen: true);
					mBullets.RemoveAt(l);
					BulletErased(l);
					l--;
				}
			}
		}
		if (mEndHoverCountdown > 0 && --mEndHoverCountdown == 0)
		{
			WarpToPoint();
			mHoverTime = SexyFramework.Common.IntRange(mMinHoverTime, mMaxHoverTime);
		}
		if (mEnrageDelayTimer > 0)
		{
			mEnrageDelayTimer--;
		}
	}

	public override void Update()
	{
		Update(1f);
	}

	public override void Draw(Graphics g)
	{
		base.Draw(g);
		if (mTeleportDir != 0)
		{
			int num = Common._S(mWidth + Common._M(200));
			g.PushState();
			int theX = (int)(Common._S(mX) - (float)(num / 2));
			int theY = (int)(Common._S(mY) - (float)(Common._S(mHeight + Common._M(10)) / 2));
			int theHeight = ((mTeleportDir != -1) ? ((int)((float)Common._S(mHeight) * mTeleportPct)) : ((int)((float)Common._S(mHeight) - (float)Common._S(mHeight) * mTeleportPct)));
			g.ClipRect(theX, theY, num, theHeight);
		}
		if (mDrawHeartsBelowBoss)
		{
			DrawHearts(g);
		}
		DrawBossSpecificArt(g);
		if (!mStrafe && mHoverTime <= Common._M(100) && mHoverTime > 0 && mPoints.Count > 1 && mHoverTime / Common._M1(10) % 2 == 0)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, Common._M(128));
			g.SetDrawMode(1);
			DrawBossSpecificArt(g);
			g.SetDrawMode(0);
			g.SetColorizeImages(colorizeImages: false);
		}
		if (mDoExplosion && mShouldDoDeathExplosions)
		{
			mHitEffect.mDrawTransform.LoadIdentity();
			float num2 = GameApp.DownScaleNum(1f);
			mHitEffect.mDrawTransform.Scale(num2, num2);
			mHitEffect.mDrawTransform.Translate(Common._S(mX) + (float)Common._S(Common._M(9)), Common._S(mY) + (float)Common._S(mHitEffectYOff));
			mHitEffect.Draw(g);
		}
		if (mTeleportDir != 0)
		{
			g.PopState();
		}
		if (mDrawHeartsBelowMisc && !mDrawHeartsBelowBoss)
		{
			DrawHearts(g);
		}
		DrawMisc(g);
		if (!mDrawHeartsBelowMisc && !mDrawHeartsBelowBoss)
		{
			DrawHearts(g);
		}
	}

	public override void SyncState(DataSync sync)
	{
		base.SyncState(sync);
		sync.SyncBoolean(ref mPauseShieldRegen);
		sync.SyncLong(ref mCurrentLocPoint);
		sync.SyncFloat(ref mSpeed);
		sync.SyncFloat(ref mDDefaultSpeed.value);
		sync.SyncFloat(ref mDMaxXInc.value);
		sync.SyncFloat(ref mDMinXInc.value);
		sync.SyncFloat(ref mDMaxYInc.value);
		sync.SyncFloat(ref mDMinYInc.value);
		sync.SyncFloat(ref mDMinAmp.value);
		sync.SyncFloat(ref mDMaxAmp.value);
		sync.SyncFloat(ref mDMinFreq.value);
		sync.SyncFloat(ref mDMaxFreq.value);
		sync.SyncFloat(ref mDHomingCorrectionAmt.value);
		sync.SyncLong(ref mDFrogSlowTimer.value);
		sync.SyncLong(ref mBombAppearDelay);
		sync.SyncLong(ref mEndHoverCountdown);
		sync.SyncLong(ref mEnrageDelayTimer);
		sync.SyncLong(ref gLastBulletId);
		sync.SyncBoolean(ref mPauseMovement);
		sync.SyncLong(ref mAttackDelayAfterHitFrog);
		sync.SyncLong(ref mTeleportDir);
		sync.SyncFloat(ref mTeleportPct);
		sync.SyncLong(ref mTeleportTime);
		sync.SyncLong(ref mVolcanoOffscreenDelay);
		sync.SyncLong(ref mDMovementMode.value);
		sync.SyncFloat(ref mDMovementAccel.value);
		sync.SyncLong(ref mDDefaultMovementUpdateDelay.value);
		sync.SyncLong(ref mMovementUpdateDelay);
		sync.SyncFloat(ref mTargetDestX);
		sync.SyncLong(ref mDBallShieldDamage.value);
		sync.SyncLong(ref mDShieldHP.value);
		sync.SyncBoolean(ref mDUseShield.value);
		sync.SyncFloat(ref mDShieldRotateSpeed.value);
		sync.SyncLong(ref mDShieldPauseTime.value);
		sync.SyncFloat(ref mShieldAngle);
		for (int i = 0; i < 4; i++)
		{
			sync.SyncLong(ref mShieldQuadrant[i].mTimer);
			sync.SyncLong(ref mShieldQuadrant[i].mHP);
		}
		sync.SyncLong(ref mCurShieldPauseTime);
		sync.SyncFloat(ref mShieldTargetAngle);
		sync.SyncLong(ref mDShieldQuadRespawnTime.value);
		SexyFramework.Misc.Buffer buffer = sync.GetBuffer();
		if (sync.isRead())
		{
			mPoints.Clear();
			int num = (int)buffer.ReadLong();
			for (int j = 0; j < num; j++)
			{
				int theX = (int)buffer.ReadLong();
				int theY = (int)buffer.ReadLong();
				mPoints.Add(new Point(theX, theY));
			}
		}
		else
		{
			buffer.WriteLong(mPoints.Count);
			for (int k = 0; k < mPoints.Count; k++)
			{
				buffer.WriteLong(mPoints[k].mX);
				buffer.WriteLong(mPoints[k].mY);
			}
		}
		sync.SyncLong(ref mStartX);
		sync.SyncLong(ref mEndX);
		sync.SyncLong(ref mStartY);
		sync.SyncLong(ref mEndY);
		sync.SyncLong(ref mDColorVampChanceToMatch2ndBall.value);
		sync.SyncLong(ref mDShotType.value);
		sync.SyncLong(ref mDMinSineShotTime.value);
		sync.SyncLong(ref mDMaxSineShotTime.value);
		sync.SyncLong(ref mDMaxBulletsToFire.value);
		sync.SyncLong(ref mDMaxRetaliationBullets.value);
		sync.SyncFloat(ref mDMinBulletSpeed.value);
		sync.SyncFloat(ref mDMaxBulletSpeed.value);
		sync.SyncFloat(ref mDMinRetalSpeed.value);
		sync.SyncFloat(ref mDMaxRetalSpeed.value);
		sync.SyncLong(ref mDMinColorChangeTime.value);
		sync.SyncLong(ref mDMaxColorChangeTime.value);
		sync.SyncLong(ref mDFrogStunTime.value);
		sync.SyncLong(ref mDShotDelay.value);
		sync.SyncLong(ref mDColorVampHealthInc.value);
		sync.SyncFloat(ref mDestY);
		sync.SyncFloat(ref mDestX);
		sync.SyncLong(ref mHoverTime);
		sync.SyncLong(ref mFireDelay);
		sync.SyncLong(ref mXOff);
		sync.SyncLong(ref mYOff);
		sync.SyncLong(ref mDMinHoverTime.value);
		sync.SyncLong(ref mDMaxHoverTime.value);
		sync.SyncLong(ref mDMinFireDelay.value);
		sync.SyncLong(ref mDMaxFireDelay.value);
		sync.SyncLong(ref mColorChangeTimer);
		sync.SyncLong(ref mColorVampShotType);
		sync.SyncLong(ref mMaxShotIncCounter);
		sync.SyncLong(ref mRetalShotIncCounter);
		sync.SyncLong(ref mDMaxBulletsToFire.value);
		sync.SyncLong(ref mDMaxRetaliationBullets.value);
		SyncListBossBullets(sync, mBullets, clear: true);
	}

	private void SyncListBossBullets(DataSync sync, List<BossBullet> theList, bool clear)
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
				BossBullet bossBullet = new BossBullet();
				bossBullet.SyncState(sync);
				theList.Add(bossBullet);
			}
			return;
		}
		sync.GetBuffer().WriteLong(theList.Count);
		foreach (BossBullet the in theList)
		{
			the.SyncState(sync);
		}
	}

	public override Boss Instantiate()
	{
		BossShoot bossShoot = new BossShoot(mLevel);
		bossShoot.CopyFrom(this);
		bossShoot.mBullets.Clear();
		return bossShoot;
	}

	public override bool Collides(Bullet b)
	{
		float pt1_x = 0f;
		float pt1_y = 0f;
		float pt2_x = 0f;
		float pt2_y = 0f;
		if (mUseShield && CommonMath.CircleCircleIntersection(mX, mY, mShieldRadius, b.GetX(), b.GetY(), b.GetRadius(), ref pt1_x, ref pt1_y, ref pt2_x, ref pt2_y))
		{
			float num = SexyFramework.Common.AngleBetweenPoints(pt1_x, pt1_y, mX, mY) + 3.14159f;
			float num2 = SexyFramework.Common.AngleBetweenPoints(pt2_x, pt2_y, mX, mY) + 3.14159f;
			for (int i = 0; i < 4; i++)
			{
				if (mShieldQuadrant[i].mTimer > 0)
				{
					continue;
				}
				float num3 = mShieldAngle + (float)i * 3.14159f / 2f;
				float num4 = mShieldAngle + (float)(i + 1) * 3.14159f / 2f;
				if (num3 > 6.28318f && num4 > 6.28318f)
				{
					num3 = Common.GetCanonicalAngleRad(num3);
					num4 = Common.GetCanonicalAngleRad(num4);
				}
				if (num3 > num4)
				{
					float num5 = num3;
					num3 = num4;
					num4 = num5;
				}
				if ((num >= num3 && num <= num4) || (num2 >= num3 && num2 <= num4))
				{
					ShieldQuadrantHit(i);
					if (mBallShieldDamage > 0 && --mShieldQuadrant[i].mHP == 0)
					{
						mShieldQuadrant[i].mTimer = mShieldQuadRespawnTime;
						mShieldQuadrant[i].mHP = mShieldHP;
						PlaySound(9);
					}
					return true;
				}
			}
		}
		float num6 = (float)b.GetRadius() * Common._M(0.75f);
		new Rect((int)(b.GetX() - num6), (int)(b.GetY() - num6), (int)(num6 * 2f), (int)(num6 * 2f));
		bool flag = AllowFrogToFire() && BulletIntersectsBoss(b);
		if (mColorVampire && flag && mAvoidColor && b.GetColorType() == mColorVampShotType)
		{
			if (mColorVampHealthInc > 0)
			{
				mHP += mColorVampHealthInc;
				if (mHP > mMaxHP)
				{
					mHP = mMaxHP;
				}
				int num7 = mColorVampHealthIncPerHit;
				for (int num8 = Boss.NUM_HEARTS - 1; num8 >= 0; num8--)
				{
					if (mHeartCels[num8] > 0)
					{
						int num9 = mHeartCels[num8];
						if ((mHeartCels[num8] -= num7) >= 0)
						{
							break;
						}
						mHeartCels[num8] = 0;
						num7 -= num9;
					}
				}
			}
			return true;
		}
		if (mColorVampire && flag && !mAvoidColor && b.GetColorType() != mColorVampShotType)
		{
			return true;
		}
		return base.Collides(b);
	}

	public override void ProximityBombActivated(float x, float y, int radius)
	{
		if (mUseShield)
		{
			float pt1_x = 0f;
			float pt1_y = 0f;
			float pt2_x = 0f;
			float pt2_y = 0f;
			if (CommonMath.CircleCircleIntersection(mX, mY, mProxBombRadius, x, y, radius + Common.GetDefaultBallRadius(), ref pt1_x, ref pt1_y, ref pt2_x, ref pt2_y))
			{
				float num = SexyFramework.Common.AngleBetweenPoints(pt1_x, pt1_y, mX, mY) + 3.14159f;
				float num2 = SexyFramework.Common.AngleBetweenPoints(pt2_x, pt2_y, mX, mY) + 3.14159f;
				for (int i = 0; i < 4; i++)
				{
					if (mShieldQuadrant[i].mTimer <= 50)
					{
						float num3 = mShieldAngle + (float)i * 3.14159f / 2f;
						float num4 = mShieldAngle + (float)(i + 1) * 3.14159f / 2f;
						if (num3 > 6.28318f && num4 > 6.28318f)
						{
							num3 = Common.GetCanonicalAngleRad(num3);
							num4 = Common.GetCanonicalAngleRad(num4);
						}
						if (num3 > num4)
						{
							float num5 = num3;
							num3 = num4;
							num4 = num5;
						}
						if ((num >= num3 && num <= num4) || (num2 >= num3 && num2 <= num4))
						{
							mShieldQuadrant[i].mTimer = mShieldQuadRespawnTime;
							mShieldQuadrant[i].mHP = mShieldHP;
							QuadHitByProxBomb(i);
						}
					}
				}
			}
		}
		base.ProximityBombActivated(x, y, radius);
	}

	public override void FrogInitialized(Gun g)
	{
		base.FrogInitialized(g);
		if (mMovementMode != 0)
		{
			mTargetDestX = (mDestX = (mX = Common._SS(GameApp.gApp.mWidth) / 2 - GameApp.gApp.mBoardOffsetX));
		}
	}

	public virtual void DisableShields()
	{
		for (int i = 0; i < 4; i++)
		{
			mShieldQuadrant[i].mTimer = mShieldQuadRespawnTime;
			mShieldQuadrant[i].mHP = mShieldHP;
		}
	}
}
