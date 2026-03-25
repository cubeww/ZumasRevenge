using System;
using System.Collections.Generic;
using System.Linq;
using JeffLib;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public class Bullet : Ball
{
	public Ball mHitBall;

	public float mVelX;

	public float mVelY;

	public float mHitX;

	public float mHitY;

	public float mHitDX;

	public float mHitDY;

	public float mDestX;

	public float mDestY;

	public float mHitPercent;

	public float mMergeSpeed;

	public float mAngleFired;

	public new int mUpdateCount;

	public bool mHitInFront;

	public bool mHaveSetPrevBall;

	public bool mJustFired;

	public bool mDoNewMerge;

	public bool mSkip;

	public List<GapInfo> mGapInfo = new List<GapInfo>();

	public int[] mCurCurvePoint = new int[4];

	public Bullet()
	{
		mVelX = 0f;
		mVelY = 0f;
		mHitBall = null;
		mHitPercent = 0f;
		mMergeSpeed = Common._M(0.025f);
		mJustFired = false;
		mDoNewMerge = false;
		mUpdateCount = 0;
		mHitDX = 0f;
		mHitDY = 0f;
		mAngleFired = 0f;
		mSkip = false;
	}

	public Bullet(Bullet other)
	{
		base.CopyFrom(other);
		mHitBall = other.mHitBall;
		mVelX = other.mVelX;
		mVelY = other.mVelY;
		mHitX = other.mHitX;
		mHitY = other.mHitY;
		mHitDX = other.mHitDX;
		mHitDY = other.mHitDY;
		mDestX = other.mDestX;
		mDestY = other.mDestY;
		mHitPercent = other.mHitPercent;
		mMergeSpeed = other.mMergeSpeed;
		mAngleFired = other.mAngleFired;
		mUpdateCount = other.mUpdateCount;
		mHitInFront = other.mHitInFront;
		mHaveSetPrevBall = other.mHaveSetPrevBall;
		mJustFired = other.mJustFired;
		mDoNewMerge = other.mDoNewMerge;
		mSkip = other.mSkip;
		mGapInfo.AddRange(other.mGapInfo.ToArray());
		Array.Copy(other.mCurCurvePoint, mCurCurvePoint, mCurCurvePoint.Length);
	}

	public override void Dispose()
	{
		SetBallInfo(null);
	}

	public void SetBallInfo(Bullet theBullet)
	{
		if (mHitBall != null)
		{
			mHitBall.SetBullet(theBullet);
		}
	}

	public void SetVelocity(float vx, float vy)
	{
		mVelX = vx;
		mVelY = vy;
	}

	public void SetHitBall(Ball theBall, bool hitInFront)
	{
		SetBallInfo(null);
		mHaveSetPrevBall = false;
		mHitBall = theBall;
		mHitX = mX;
		mHitY = mY;
		mHitDX = mX - theBall.GetX();
		mHitDY = mY - theBall.GetY();
		mHitPercent = 0f;
		mHitInFront = hitInFront;
		SetBallInfo(this);
	}

	public void CheckSetHitBallToPrevBall()
	{
		if (!mHaveSetPrevBall && mHitBall != null)
		{
			Ball prevBall = mHitBall.GetPrevBall();
			if (prevBall != null && prevBall.CollidesWithPhysically(this) && !prevBall.GetIsExploding())
			{
				mHaveSetPrevBall = true;
				SetBallInfo(null);
				mHitBall = prevBall;
				mHitInFront = true;
				mHitX = mX;
				mHitY = mY;
				mHitDX = mX - prevBall.GetX();
				mHitDY = mY - prevBall.GetY();
				mHitPercent = 0f;
				SetBallInfo(this);
			}
		}
	}

	public void SetDestPos(float x, float y)
	{
		mDestX = x;
		mDestY = y;
	}

	public void SetDXPos()
	{
		float num = 1f - mHitPercent;
		mX += mHitDX * num;
		mY += mHitDY * num;
	}

	public void Update(float theAmount)
	{
		mUpdateCount++;
		mDisplayType = mColorType;
		if (mHitBall == null)
		{
			float num = mVelX * theAmount;
			float num2 = mVelY * theAmount;
			mX += num;
			mY += num2;
		}
		else if (!mExploding)
		{
			mHitPercent += mMergeSpeed;
			if (mHitPercent > 1f)
			{
				mHitPercent = 1f;
			}
			if (!mDoNewMerge)
			{
				mX = mHitX + mHitPercent * (mDestX - mHitX);
				mY = mHitY + mHitPercent * (mDestY - mHitY);
			}
		}
		UpdateRotation();
	}

	public new void Update()
	{
		Update(1f);
	}

	public void MergeFully()
	{
		mHitPercent = 1f;
		Update();
	}

	public Ball GetPushBall()
	{
		if (mHitBall == null)
		{
			return null;
		}
		Ball ball = null;
		ball = (mHitInFront ? mHitBall.GetNextBall() : mHitBall);
		if (ball != null && (mDoNewMerge || ball.CollidesWithPhysically(this)))
		{
			return ball;
		}
		return null;
	}

	public void UpdateHitPos()
	{
		mHitX = mX;
		mHitY = mY;
	}

	public void SetCurCurvePoint(int theCurveNum, int thePoint)
	{
		mCurCurvePoint[theCurveNum] = thePoint;
	}

	public int GetCurCurvePoint(int theCurveNum)
	{
		return mCurCurvePoint[theCurveNum];
	}

	public bool AddGapInfo(int theCurve, int theDist, int theBallId)
	{
		foreach (GapInfo item in mGapInfo)
		{
			if (item.mBallId == theBallId)
			{
				return false;
			}
		}
		GapInfo gapInfo = new GapInfo();
		gapInfo.mBallId = theBallId;
		gapInfo.mDist = theDist;
		gapInfo.mCurve = theCurve;
		mGapInfo.Add(gapInfo);
		return true;
	}

	public int GetCurGapBall(int theCurveNum)
	{
		int result = 0;
		foreach (GapInfo item in mGapInfo)
		{
			if (item.mCurve == theCurveNum)
			{
				result = item.mBallId;
			}
		}
		return result;
	}

	public int GetMinGapDist()
	{
		int num = 0;
		foreach (GapInfo item in mGapInfo)
		{
			if (num == 0 || item.mDist < num)
			{
				num = item.mDist;
			}
		}
		return num;
	}

	public void RemoveGapInfoForBall(int theBallId)
	{
		int num = 0;
		while (num != mGapInfo.Count())
		{
			if (mGapInfo[num].mBallId == theBallId)
			{
				mGapInfo.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
	}

	public override void SyncState(DataSync theSync)
	{
		base.SyncState(theSync);
		theSync.SyncFloat(ref mVelX);
		theSync.SyncFloat(ref mVelY);
		theSync.SyncBoolean(ref mHitInFront);
		theSync.SyncBoolean(ref mHaveSetPrevBall);
		theSync.SyncFloat(ref mHitX);
		theSync.SyncFloat(ref mHitY);
		theSync.SyncFloat(ref mDestX);
		theSync.SyncFloat(ref mDestY);
		theSync.SyncFloat(ref mHitDX);
		theSync.SyncFloat(ref mHitDY);
		theSync.SyncLong(ref mUpdateCount);
		theSync.SyncBoolean(ref mHitInFront);
		theSync.SyncBoolean(ref mHaveSetPrevBall);
		theSync.SyncBoolean(ref mJustFired);
		theSync.SyncBoolean(ref mDoNewMerge);
		theSync.SyncFloat(ref mHitPercent);
		theSync.SyncFloat(ref mMergeSpeed);
		theSync.SyncFloat(ref mAngleFired);
		theSync.SyncBoolean(ref mSkip);
		for (int i = 0; i < 4; i++)
		{
			theSync.SyncLong(ref mCurCurvePoint[i]);
		}
		theSync.SyncPointer(this);
		SyncListGapInfos(theSync, clear: true);
	}

	private void SyncListGapInfos(DataSync sync, bool clear)
	{
		if (sync.isRead())
		{
			if (clear)
			{
				mGapInfo.Clear();
			}
			long num = sync.GetBuffer().ReadLong();
			for (int i = 0; i < num; i++)
			{
				GapInfo gapInfo = new GapInfo();
				gapInfo.SyncState(sync);
				mGapInfo.Add(gapInfo);
			}
			return;
		}
		sync.GetBuffer().WriteLong(mGapInfo.Count);
		foreach (GapInfo item in mGapInfo)
		{
			item.SyncState(sync);
		}
	}

	public override void Draw(Graphics g, int xoff, int yoff)
	{
		if (mIsCannon)
		{
			if (mFrog.mBoard.LevelIsSkeletonBoss())
			{
				Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_GLOWBALL);
				float num = Common._S(mX) - (float)(imageByID.mWidth / 2);
				float num2 = Common._S(mY) - (float)(imageByID.mHeight / 2);
				g.DrawImage(imageByID, (int)num, (int)num2);
				g.PushState();
				g.SetColorizeImages(colorizeImages: true);
				g.SetDrawMode(1);
				int alphaFromUpdateCount = JeffLib.Common.GetAlphaFromUpdateCount(mUpdateCount, Common._M(64));
				g.SetColor(255, 255, 255, alphaFromUpdateCount);
				g.DrawImage(imageByID, (int)num, (int)num2);
				g.PopState();
			}
			else
			{
				Image imageByID2 = Res.GetImageByID(ResID.IMAGE_CANNON_BALL);
				float num3 = Common._S(mX) - (float)(imageByID2.mWidth / 2);
				float num4 = Common._S(mY) - (float)(imageByID2.mHeight / 2);
				if (g.Is3D())
				{
					g.DrawImageRotatedF(imageByID2, num3, num4, mRotation + 3.14159f);
				}
				else
				{
					g.DrawImageRotated(imageByID2, (int)num3, (int)num4, mRotation + 3.14159f);
				}
			}
		}
		else
		{
			float num5 = mWayPoint;
			mWayPoint = 0f;
			base.Draw(g, xoff, yoff);
			mWayPoint = num5;
		}
	}

	public new void Draw(Graphics g)
	{
		Draw(g, 0, 0);
	}

	public Ball GetHitBall()
	{
		return mHitBall;
	}

	public float GetHitPercent()
	{
		return mHitPercent;
	}

	public float GetVelX()
	{
		return mVelX;
	}

	public float GetVelY()
	{
		return mVelY;
	}

	public bool GetHitInFront()
	{
		return mHitInFront;
	}

	public bool GetJustFired()
	{
		return mJustFired;
	}

	public new int GetNumGaps()
	{
		return mGapInfo.Count();
	}

	public int GetUpdateCount()
	{
		return mUpdateCount;
	}

	public void SetJustFired(bool fired)
	{
		mJustFired = fired;
	}

	public void SetMergeSpeed(float theSpeed)
	{
		mMergeSpeed = theSpeed;
	}
}
