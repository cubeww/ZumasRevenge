using System;

namespace ZumasRevenge;

public class BossBullet : IDisposable
{
	public float mVX;

	public float mVY;

	public float mInitialSpeed;

	public float mTargetVX;

	public float mTargetVY;

	public float mX;

	public float mY;

	public float mAmp;

	public float mFreq;

	public float mGravity;

	public float mAngle;

	public float mSize;

	public float mAlpha;

	public bool mSineMotion;

	public bool mHoming;

	public bool mCanHitPlayer;

	public bool mDeleteInstantly;

	public int mBouncesLeft;

	public int mId;

	public int mUpdateCount;

	public int mDelay;

	public int mState;

	public int mImageNum;

	public int mOffscreenPause;

	public int mShotType;

	public int mCel;

	public bool mVolcanoShot;

	public object mData;

	public BossShoot mBossShoot;

	public BossBullet()
	{
		mDelay = (mBouncesLeft = (mUpdateCount = (mOffscreenPause = 0)));
		mGravity = (mTargetVX = (mTargetVY = 0f));
		mDeleteInstantly = false;
		mSize = 1f;
		mShotType = 0;
		mId = -1;
		mInitialSpeed = 0f;
		mVolcanoShot = (mHoming = false);
		mAmp = (mFreq = 0f);
		mSineMotion = false;
		mCanHitPlayer = true;
		mState = 0;
		mImageNum = 0;
		mAngle = 0f;
		mAlpha = 255f;
		mCel = 0;
		mData = null;
		mBossShoot = null;
	}

	public BossBullet(BossBullet rhs)
		: this()
	{
		if (rhs != null)
		{
			mDelay = rhs.mDelay;
			mBouncesLeft = rhs.mBouncesLeft;
			mUpdateCount = rhs.mUpdateCount;
			mOffscreenPause = rhs.mOffscreenPause;
			mGravity = rhs.mGravity;
			mTargetVX = rhs.mTargetVX;
			mTargetVY = rhs.mTargetVY;
			mDeleteInstantly = rhs.mDeleteInstantly;
			mSize = rhs.mSize;
			mShotType = rhs.mShotType;
			mId = rhs.mId;
			mInitialSpeed = rhs.mInitialSpeed;
			mVolcanoShot = rhs.mVolcanoShot;
			mHoming = rhs.mHoming;
			mAmp = rhs.mAmp;
			mFreq = rhs.mFreq;
			mSineMotion = rhs.mSineMotion;
			mCanHitPlayer = rhs.mCanHitPlayer;
			mState = rhs.mState;
			mImageNum = rhs.mImageNum;
			mAngle = rhs.mAngle;
			mAlpha = rhs.mAlpha;
			mCel = rhs.mCel;
			mData = rhs.mData;
			mBossShoot = rhs.mBossShoot;
		}
	}

	public virtual void Dispose()
	{
	}

	public void SyncState(DataSync sync)
	{
		sync.SyncFloat(ref mVX);
		sync.SyncFloat(ref mVY);
		sync.SyncFloat(ref mX);
		sync.SyncFloat(ref mY);
		sync.SyncFloat(ref mAmp);
		sync.SyncFloat(ref mFreq);
		sync.SyncBoolean(ref mSineMotion);
		sync.SyncLong(ref mUpdateCount);
		sync.SyncLong(ref mDelay);
		sync.SyncLong(ref mState);
		sync.SyncLong(ref mImageNum);
		sync.SyncFloat(ref mAngle);
		sync.SyncBoolean(ref mHoming);
		sync.SyncFloat(ref mTargetVX);
		sync.SyncBoolean(ref mCanHitPlayer);
		sync.SyncFloat(ref mTargetVY);
		sync.SyncFloat(ref mInitialSpeed);
		sync.SyncLong(ref mOffscreenPause);
		sync.SyncBoolean(ref mVolcanoShot);
		sync.SyncFloat(ref mSize);
		sync.SyncFloat(ref mAlpha);
		sync.SyncLong(ref mShotType);
		sync.SyncLong(ref mCel);
		sync.SyncLong(ref mBouncesLeft);
		sync.SyncLong(ref mId);
	}
}
