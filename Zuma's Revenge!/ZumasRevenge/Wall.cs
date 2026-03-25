using SexyFramework.Graphics;

namespace ZumasRevenge;

public class Wall
{
	public float mX;

	public float mY;

	public float mWidth;

	public float mHeight;

	public float mVX;

	public float mVY;

	public int mSpacing;

	public int mStrength;

	public int mOrgStrength;

	public int mMinRespawnTimer;

	public int mMaxRespawnTimer;

	public int mCurRespawnTimer;

	public int mMinLifeTimer;

	public int mMaxLifeTimer;

	public int mCurLifeTimer;

	public int mId;

	public int mUpdateCount;

	public int mState;

	public int mSize = 1;

	public int mMaxSize = 1;

	public Color mColor = default(Color);

	public Image mImage;

	public int mCel;

	public int mExpCel;

	public int mType = 1;

	public void SyncState(DataSync sync)
	{
		sync.SyncFloat(ref mX);
		sync.SyncFloat(ref mY);
		sync.SyncFloat(ref mWidth);
		sync.SyncFloat(ref mHeight);
		sync.SyncLong(ref mStrength);
		sync.SyncLong(ref mOrgStrength);
		sync.SyncLong(ref mMinRespawnTimer);
		sync.SyncLong(ref mMaxRespawnTimer);
		sync.SyncLong(ref mCurRespawnTimer);
		sync.SyncLong(ref mMinLifeTimer);
		sync.SyncLong(ref mMaxLifeTimer);
		sync.SyncLong(ref mCurLifeTimer);
		sync.SyncLong(ref mId);
		sync.SyncLong(ref mColor.mRed);
		sync.SyncLong(ref mColor.mGreen);
		sync.SyncLong(ref mColor.mBlue);
		sync.SyncLong(ref mColor.mAlpha);
		sync.SyncLong(ref mUpdateCount);
		sync.SyncLong(ref mState);
		sync.SyncLong(ref mSize);
		sync.SyncLong(ref mMaxSize);
		sync.SyncLong(ref mCel);
		sync.SyncLong(ref mExpCel);
		sync.SyncLong(ref mType);
		sync.SyncFloat(ref mVX);
		sync.SyncFloat(ref mVY);
		sync.SyncLong(ref mSpacing);
	}

	public void Update()
	{
		if (mCurLifeTimer > 0 && --mCurLifeTimer == 0)
		{
			mType = 0;
			mCurRespawnTimer = MathUtils.IntRange(mMinRespawnTimer, mMaxRespawnTimer);
			return;
		}
		if (mCurRespawnTimer > 0 && --mCurRespawnTimer == 0)
		{
			mType = 1;
			mCurLifeTimer = MathUtils.IntRange(mMinLifeTimer, mMaxLifeTimer);
		}
		if (mType != 0 || mCurRespawnTimer > 0)
		{
			mUpdateCount++;
			if (mVX != 0f)
			{
				mX += mVX;
			}
			if (mVY != 0f)
			{
				mY += mVY;
			}
		}
	}

	public void Draw(Graphics g)
	{
		if (mStrength != 0)
		{
			_ = mType;
		}
	}

	public bool Hit()
	{
		return false;
	}
}
