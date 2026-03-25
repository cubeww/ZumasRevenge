using System;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public class WarningLight
{
	public float mX;

	public float mY;

	public float mAlpha;

	public float mAngle;

	public float mPulseAlpha;

	public float mPulseRate;

	public float mWaypoint;

	public int mState;

	public int mUpdateCount;

	public int mPriority;

	public WarningLight(float x, float y)
	{
		mX = x;
		mY = y;
		mAlpha = 0f;
		mUpdateCount = 0;
		mAngle = 0f;
		mState = 0;
		mWaypoint = -1f;
		mPulseAlpha = 0f;
		mPulseRate = 0f;
		mPriority = 0;
	}

	public bool Update()
	{
		mUpdateCount++;
		float num = Common._M(5f);
		if (mState == 1)
		{
			mAlpha = Math.Min(255f, mAlpha + num);
			if (mAlpha >= 255f)
			{
				mState = 0;
			}
		}
		else if (mState == -1)
		{
			mAlpha = Math.Max(0f, mAlpha - num);
			if (mAlpha <= 0f)
			{
				mState = 0;
			}
		}
		else if (mPulseRate != 0f)
		{
			mPulseAlpha += ((mPulseRate > 0f) ? (mPulseRate * 2f) : mPulseRate);
			if (mPulseRate < 0f && mPulseAlpha <= 0f)
			{
				mPulseRate = 0f;
				mPulseAlpha = 0f;
			}
			else if (mPulseAlpha >= 255f && mPulseRate > 0f)
			{
				mPulseRate *= -1f;
				mPulseAlpha = 255f;
				return true;
			}
		}
		return false;
	}

	public void Draw(Graphics g)
	{
		if (mAlpha != 0f)
		{
			g.PushState();
			if (mAlpha != 0f)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)mAlpha);
			}
			Image imageByID = Res.GetImageByID(ResID.IMAGE_SKULL_PATH);
			g.DrawImageRotated(imageByID, (int)(Common._S(mX) - (float)(imageByID.mWidth / 2)), (int)(Common._S(mY) - (float)(imageByID.mHeight / 2)), mAngle + 1.570795f);
			if (mPulseAlpha != 0f)
			{
				Image imageByID2 = Res.GetImageByID(ResID.IMAGE_SKULL_PATH_LIT);
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)mPulseAlpha);
				g.DrawImageRotated(imageByID2, (int)(Common._S(mX) - (float)(imageByID2.mWidth / 2)), (int)(Common._S(mY) - (float)(imageByID2.mHeight / 2)), mAngle + 1.570795f);
			}
			g.SetColorizeImages(colorizeImages: false);
			g.PopState();
		}
	}

	public void SyncState(DataSync sync)
	{
		sync.SyncFloat(ref mAlpha);
		sync.SyncFloat(ref mPulseAlpha);
		sync.SyncFloat(ref mPulseRate);
		sync.SyncLong(ref mState);
		sync.SyncLong(ref mUpdateCount);
	}
}
