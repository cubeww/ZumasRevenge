namespace ZumasRevenge;

public class CurveDesc
{
	public string mPath;

	public BasicCurveVals mVals;

	public int mDangerDistance;

	public float mMergeSpeed;

	public float mCurAcceleration;

	public int mCutoffPoint;

	public int mStartDistance;

	public CurveDesc()
	{
		mVals = new BasicCurveVals();
		mVals.mSpeed = 0.5f;
		mStartDistance = 40;
		mVals.mNumColors = 4;
		mVals.mNumBalls = 0;
		mVals.mBallRepeat = 40;
		mVals.mMaxSingle = 10;
		mMergeSpeed = Common._M(0.025f);
		mDangerDistance = 600;
		mVals.mAccelerationRate = 0f;
		mVals.mMaxSpeed = 100f;
		mVals.mScoreTarget = 1000;
		mVals.mSkullRotation = -1;
		mCurAcceleration = 0f;
		mCutoffPoint = Common._M(17);
		for (int i = 0; i < 14; i++)
		{
			mVals.mPowerUpFreq[i] = 5250;
			mVals.mMaxNumPowerUps[i] = int.MaxValue;
		}
		mVals.mPowerUpChance = 100;
		mVals.mSlowFactor = 4f;
		mVals.mSlowDistance = 500;
		mVals.mZumaBack = 300;
		mVals.mZumaSlow = 1100;
		mVals.mDrawPit = true;
		mVals.mDrawTunnels = true;
		mVals.mDestroyAll = true;
		mVals.mDieAtEnd = true;
	}

	public CurveDesc(CurveDesc rhs)
	{
		if (rhs != null)
		{
			mCurAcceleration = rhs.mCurAcceleration;
			mCutoffPoint = rhs.mCutoffPoint;
			mPath = rhs.mPath;
			mMergeSpeed = rhs.mMergeSpeed;
			mDangerDistance = rhs.mDangerDistance;
			mStartDistance = rhs.mStartDistance;
			mVals = new BasicCurveVals();
			mVals.mSpeed = rhs.mVals.mSpeed;
			mVals.mNumColors = rhs.mVals.mNumColors;
			mVals.mNumBalls = rhs.mVals.mNumBalls;
			mVals.mBallRepeat = rhs.mVals.mBallRepeat;
			mVals.mMaxSingle = rhs.mVals.mMaxSingle;
			mVals.mAccelerationRate = rhs.mVals.mAccelerationRate;
			mVals.mMaxSpeed = rhs.mVals.mMaxSpeed;
			mVals.mScoreTarget = rhs.mVals.mScoreTarget;
			mVals.mSkullRotation = rhs.mVals.mSkullRotation;
			mVals.mStartDistance = rhs.mVals.mStartDistance;
			for (int i = 0; i < 14; i++)
			{
				mVals.mPowerUpFreq[i] = rhs.mVals.mPowerUpFreq[i];
				mVals.mMaxNumPowerUps[i] = rhs.mVals.mMaxNumPowerUps[i];
			}
			mVals.mPowerUpChance = rhs.mVals.mPowerUpChance;
			mVals.mSlowFactor = rhs.mVals.mSlowFactor;
			mVals.mSlowDistance = rhs.mVals.mSlowDistance;
			mVals.mZumaBack = rhs.mVals.mZumaBack;
			mVals.mZumaSlow = rhs.mVals.mZumaSlow;
			mVals.mDrawPit = rhs.mVals.mDrawPit;
			mVals.mDrawTunnels = rhs.mVals.mDrawTunnels;
			mVals.mDestroyAll = rhs.mVals.mDestroyAll;
			mVals.mDieAtEnd = rhs.mVals.mDieAtEnd;
			mVals.mMaxClumpSize = rhs.mVals.mMaxClumpSize;
			mVals.mOrgAccelerationRate = rhs.mVals.mOrgAccelerationRate;
			mVals.mOrgMaxSpeed = rhs.mVals.mOrgMaxSpeed;
		}
	}

	public void GetValuesFrom(CurveData data)
	{
		mVals = new BasicCurveVals();
		mVals.mSpeed = data.mVals.mSpeed;
		mVals.mNumColors = data.mVals.mNumColors;
		mVals.mNumBalls = data.mVals.mNumBalls;
		mVals.mBallRepeat = data.mVals.mBallRepeat;
		mVals.mMaxSingle = data.mVals.mMaxSingle;
		mVals.mAccelerationRate = data.mVals.mAccelerationRate;
		mVals.mMaxSpeed = data.mVals.mMaxSpeed;
		mVals.mScoreTarget = data.mVals.mScoreTarget;
		mVals.mSkullRotation = data.mVals.mSkullRotation;
		mVals.mStartDistance = data.mVals.mStartDistance;
		for (int i = 0; i < 14; i++)
		{
			mVals.mPowerUpFreq[i] = data.mVals.mPowerUpFreq[i];
			mVals.mMaxNumPowerUps[i] = data.mVals.mMaxNumPowerUps[i];
		}
		mVals.mPowerUpChance = data.mVals.mPowerUpChance;
		mVals.mSlowFactor = data.mVals.mSlowFactor;
		mVals.mSlowDistance = data.mVals.mSlowDistance;
		mVals.mZumaBack = data.mVals.mZumaBack;
		mVals.mZumaSlow = data.mVals.mZumaSlow;
		mVals.mDrawPit = data.mVals.mDrawPit;
		mVals.mDrawTunnels = data.mVals.mDrawTunnels;
		mVals.mDestroyAll = data.mVals.mDestroyAll;
		mVals.mDieAtEnd = data.mVals.mDieAtEnd;
		mVals.mMaxClumpSize = data.mVals.mMaxClumpSize;
		mVals.mOrgAccelerationRate = data.mVals.mOrgAccelerationRate;
		mVals.mOrgMaxSpeed = data.mVals.mOrgMaxSpeed;
	}
}
