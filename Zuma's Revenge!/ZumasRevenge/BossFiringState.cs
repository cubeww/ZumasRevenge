namespace ZumasRevenge;

public class BossFiringState
{
	public int mState;

	public float mPawYOffset;

	public float mSkullXOffset;

	public float mSkullYOffset;

	public float mSkullAngle;

	public float mHeadAngle;

	public float mSkullGrowPct = 1f;

	public float mTargetSkullAngle;

	public float mSkullAngleInc;

	public int mSwipeFrame;

	public int mTimer;

	public float mStreaksAlpha;

	public int mBulletId;

	public BossFiringState()
	{
	}

	public BossFiringState(BossFiringState rhs)
	{
		mState = rhs.mState;
		mPawYOffset = rhs.mPawYOffset;
		mSkullXOffset = rhs.mSkullXOffset;
		mSkullYOffset = rhs.mSkullYOffset;
		mSkullAngle = rhs.mSkullAngle;
		mHeadAngle = rhs.mHeadAngle;
		mSkullGrowPct = rhs.mSkullGrowPct;
		mTargetSkullAngle = rhs.mTargetSkullAngle;
		mSkullAngleInc = rhs.mSkullAngleInc;
		mSwipeFrame = rhs.mSwipeFrame;
		mTimer = rhs.mTimer;
		mStreaksAlpha = rhs.mStreaksAlpha;
		mBulletId = rhs.mBulletId;
	}
}
