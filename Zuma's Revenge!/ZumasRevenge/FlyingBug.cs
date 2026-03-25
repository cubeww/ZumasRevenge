using System;
using System.Collections.Generic;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class FlyingBug : Effect
{
	protected enum State
	{
		State_Flying,
		State_Rotating,
		State_SlowingDown,
		State_Resting,
		State_Accelerating
	}

	protected List<Critter> mBugs = new List<Critter>();

	protected Board mBoard;

	protected Rect mNoEnterRect;

	protected int mMinBugs;

	protected int mMaxBugs;

	protected int mReverseTimer;

	protected int mReverseRotateDelay;

	protected int mRotateMinTimer;

	protected int mRotateMaxTimer;

	protected int mTimeToSlowDown;

	protected int mFlyingMinTimer;

	protected int mFlyingMaxTimer;

	protected int mDefaultAnimRate;

	protected int mRestingMinTimer;

	protected int mRestingMaxTimer;

	protected int mAccelerationTime;

	protected int mMaxRotateDegrees;

	protected bool mAllowRest;

	protected bool mNoRotateUntilOnScreen;

	protected Color mDrawColor = default(Color);

	protected Rect mTestRect = default(Rect);

	protected Transform mDrawTransform = new Transform();

	protected override void Init()
	{
		mBoard = GameApp.gApp.GetBoard();
		if (mBoard != null)
		{
			mBugs.Clear();
			int num = mMaxBugs - mMinBugs;
			if (num <= 0)
			{
				num = 1;
			}
			int num2 = mMinBugs + SexyFramework.Common.Rand() % num;
			for (int i = 0; i < num2; i++)
			{
				Critter critter = new Critter();
				mBugs.Add(critter);
				SetupBug(critter);
			}
		}
	}

	protected virtual void SetupBug(Critter d)
	{
		d.mCel = 0;
		switch (SexyFramework.Common.Rand() % 4)
		{
		case 0:
			d.mX = -d.mImage.GetCelWidth();
			d.mY = SexyFramework.Common.Rand() % Common._SS(GameApp.gApp.mHeight);
			d.mAngle = Common.GetCanonicalAngleRad(SexyFramework.Common.DegreesToRadians(-45f) + SexyFramework.Common.DegreesToRadians(SexyFramework.Common.Rand() % 90));
			break;
		case 1:
			d.mX = Common._SS(GameApp.gApp.mWidth);
			d.mY = SexyFramework.Common.Rand() % Common._SS(GameApp.gApp.mHeight);
			d.mAngle = SexyFramework.Common.DegreesToRadians(135f) + SexyFramework.Common.DegreesToRadians(SexyFramework.Common.Rand() % 90);
			break;
		case 2:
			d.mX = SexyFramework.Common.Rand() % Common._SS(GameApp.gApp.mWidth);
			d.mY = -d.mImage.GetCelHeight();
			d.mAngle = SexyFramework.Common.DegreesToRadians(225f) + SexyFramework.Common.DegreesToRadians(SexyFramework.Common.Rand() % 90);
			break;
		default:
			d.mX = SexyFramework.Common.Rand() % Common._SS(GameApp.gApp.mWidth);
			d.mY = Common._SS(GameApp.gApp.mHeight);
			d.mAngle = SexyFramework.Common.DegreesToRadians(45f) + SexyFramework.Common.DegreesToRadians(SexyFramework.Common.Rand() % 90);
			break;
		}
		d.mUpdateCount = 0;
		d.mAX = (d.mAY = 0f);
		d.mAngleInc = 0f;
		d.mTargetAngle = d.mAngle;
		d.mState = 0;
		d.mAlpha = 255f;
		d.mFadeOut = false;
		d.mSize = 1f;
		d.mRotateDelay = 0;
		d.mAnimRate = mDefaultAnimRate;
		int num = mFlyingMaxTimer - mFlyingMinTimer;
		if (num <= 0)
		{
			num = 1;
		}
		d.mTimer = mFlyingMinTimer + SexyFramework.Common.Rand() % num;
	}

	protected virtual void UpdateStateFlying(Critter d)
	{
		d.mX += d.mVX;
		d.mY += d.mVY;
		d.mTimer--;
		Rect theTRect = new Rect((int)d.mX, (int)d.mY, d.mImage.GetCelWidth(), d.mImage.GetCelHeight());
		Rect rect = new Rect(theTRect.mWidth, theTRect.mHeight, GameApp.gApp.mWidth - theTRect.mWidth * 2, GameApp.gApp.mHeight - theTRect.mHeight * 2);
		if ((d.mTimer > 0 || (mNoRotateUntilOnScreen && !rect.Intersects(theTRect))) && !mNoEnterRect.Intersects(theTRect))
		{
			return;
		}
		if (d.mTimer > 0)
		{
			d.mTimer = mReverseTimer;
			d.mTargetAngle = d.mAngle - 3.14159f;
			d.mRotateDelay = mReverseRotateDelay;
		}
		else
		{
			int num = mRotateMaxTimer - mRotateMinTimer;
			if (num <= 0)
			{
				num = 1;
			}
			d.mTimer = mRotateMinTimer + SexyFramework.Common.Rand() % num;
			d.mTargetAngle = SexyFramework.Common.DegreesToRadians(SexyFramework.Common.Rand() % mMaxRotateDegrees) * (float)((SexyFramework.Common.Rand() % 2 == 0) ? 1 : (-1));
		}
		d.mAngleInc = (d.mTargetAngle - d.mAngle) / (float)d.mTimer;
		d.mState = 1;
	}

	protected virtual void UpdateStateRotating(Critter d)
	{
		d.mTimer--;
		if (d.mRotateDelay == 0 || d.mTimer > 0)
		{
			d.mAngle += d.mAngleInc;
			d.mVX = (float)Math.Cos(d.mAngle) * d.mInitVel;
			d.mVY = (0f - (float)Math.Sin(d.mAngle)) * d.mInitVel;
		}
		d.mX += d.mVX;
		d.mY += d.mVY;
		if (d.mTimer != 0)
		{
			return;
		}
		if (d.mRotateDelay > 0)
		{
			d.mRotateDelay--;
			return;
		}
		if (mAllowRest && SexyFramework.Common.Rand() % 2 != 0)
		{
			d.mState = 2;
			d.mTimer = mTimeToSlowDown;
			d.mAX = (0f - d.mVX) / (float)d.mTimer;
			d.mAY = (0f - d.mVY) / (float)d.mTimer;
			return;
		}
		int num = mFlyingMaxTimer - mFlyingMinTimer;
		if (num <= 0)
		{
			num = 1;
		}
		d.mTimer = mFlyingMinTimer + SexyFramework.Common.Rand() % num;
		d.mState = 0;
	}

	protected virtual void UpdateStateSlowingDown(Critter d)
	{
		d.mVX += d.mAX;
		d.mVY += d.mAY;
		d.mX += d.mVX;
		d.mY += d.mVY;
		float num = (float)d.mTimer / (float)mTimeToSlowDown;
		float num2 = 1f - num;
		d.mAnimRate = (int)((float)mDefaultAnimRate + (float)(mDefaultAnimRate * 2) * num2);
		d.mSize = 0.75f + 0.25f * num;
		if (--d.mTimer == 0)
		{
			d.mState = 3;
			int num3 = mRestingMaxTimer - mRestingMinTimer;
			if (num3 <= 0)
			{
				num3 = 1;
			}
			d.mTimer = mRestingMinTimer + SexyFramework.Common.Rand() % num3;
			d.mVX = (d.mVY = 0f);
			d.mCel = (d.mCel + 1) % d.mImage.mNumCols;
		}
	}

	protected virtual void UpdateStateResting(Critter d)
	{
		if (--d.mTimer == 0)
		{
			float num = mAccelerationTime;
			d.mState = 4;
			d.mAX = (float)Math.Cos(d.mAngle) * d.mInitVel / num;
			d.mAY = (0f - (float)Math.Sin(d.mAngle)) * d.mInitVel / num;
			d.mTimer = (int)num;
		}
	}

	protected virtual void UpdateStateAccelerating(Critter d)
	{
		d.mVX += d.mAX;
		d.mVY += d.mAY;
		d.mX += d.mVX;
		d.mY += d.mVY;
		float num = (float)d.mTimer / Common._M(100f);
		float num2 = 1f - num;
		d.mSize = 1f - 0.25f * num;
		if (d.mSize > 1f)
		{
			d.mSize = 1f;
		}
		d.mAnimRate = (int)((float)(mDefaultAnimRate * 3) - (float)(mDefaultAnimRate * 2) * num2);
		if (d.mAnimRate < mDefaultAnimRate)
		{
			d.mAnimRate = mDefaultAnimRate;
		}
		if (--d.mTimer == 0)
		{
			d.mState = 0;
			int num3 = mFlyingMaxTimer - mFlyingMinTimer;
			if (num3 <= 0)
			{
				num3 = 1;
			}
			d.mTimer = mFlyingMinTimer + SexyFramework.Common.Rand() % num3;
		}
	}

	public virtual void DrawBug(Graphics g, Critter d, Transform t)
	{
		g.SetColorizeImages(colorizeImages: true);
		mDrawColor.SetColor(0, 0, 0, (int)d.mAlpha);
		g.SetColor(mDrawColor);
		if (g.Is3D())
		{
			g.DrawImageTransformF(d.mImage, t, d.mImage.GetCelRect(d.mCel), Common._S(d.mX + 3f), Common._S(d.mY + 3f));
		}
		else
		{
			g.DrawImageTransform(d.mImage, t, d.mImage.GetCelRect(d.mCel), Common._S(d.mX + 3f), Common._S(d.mY + 3f));
		}
		g.SetColorizeImages(colorizeImages: false);
		if (d.mAlpha != 255f)
		{
			g.SetColorizeImages(colorizeImages: true);
			mDrawColor.SetColor(255, 255, 255, (int)d.mAlpha);
			g.SetColor(mDrawColor);
		}
		if (g.Is3D())
		{
			g.DrawImageTransformF(d.mImage, t, d.mImage.GetCelRect(d.mCel), Common._S(d.mX), Common._S(d.mY));
		}
		else
		{
			g.DrawImageTransform(d.mImage, t, d.mImage.GetCelRect(d.mCel), Common._S(d.mX), Common._S(d.mY));
		}
		g.SetColorizeImages(colorizeImages: false);
	}

	public FlyingBug()
	{
		mResGroup = "LandCritters";
		mMinBugs = (mMaxBugs = 0);
		mNoEnterRect = new Rect(0, 0, 0, 0);
		mReverseTimer = (mReverseRotateDelay = 0);
		mMaxRotateDegrees = 360;
		mAllowRest = true;
		mNoRotateUntilOnScreen = false;
	}

	public override void Update()
	{
		if (!GameApp.gApp.mGraphicsDriver.Is3D())
		{
			return;
		}
		mUpdateCount++;
		Rect rect = mBoard.GetGun().GetRect();
		for (int i = 0; i < mBugs.Count; i++)
		{
			Critter critter = mBugs[i];
			critter.mUpdateCount++;
			if (mUpdateCount % critter.mAnimRate == 0 && critter.mState != 3)
			{
				critter.mCel = (critter.mCel + 1) % critter.mImage.mNumCols;
			}
			critter.mFadeOut = false;
			bool is_tunnel = false;
			for (int j = 0; j < mBoard.mLevel.mNumCurves; j++)
			{
				int waypointFromXY = mBoard.mLevel.mCurveMgr[j].GetWaypointFromXY(critter.mX, critter.mY, ref is_tunnel, (uint)Common._M(0), (uint)Common._M1(1200));
				if (waypointFromXY != -1 && mBoard.mLevel.mCurveMgr[j].GetBallFromWaypoint(waypointFromXY) != null)
				{
					critter.mFadeOut = true;
					break;
				}
			}
			if (rect.Intersects((int)critter.mX, (int)critter.mY, critter.mImage.GetCelWidth(), critter.mImage.GetCelHeight()))
			{
				critter.mFadeOut = true;
			}
			int num = Common._M(4);
			if (critter.mFadeOut && critter.mAlpha > (float)Common._M(64))
			{
				critter.mAlpha = Math.Max(0f, critter.mAlpha - (float)num);
			}
			else if (!critter.mFadeOut && critter.mAlpha < 255f)
			{
				critter.mAlpha = Math.Min(255f, critter.mAlpha + (float)num);
			}
			switch ((State)critter.mState)
			{
			case State.State_Flying:
				UpdateStateFlying(critter);
				break;
			case State.State_Rotating:
				UpdateStateRotating(critter);
				break;
			case State.State_SlowingDown:
				UpdateStateSlowingDown(critter);
				break;
			case State.State_Resting:
				UpdateStateResting(critter);
				break;
			case State.State_Accelerating:
				UpdateStateAccelerating(critter);
				break;
			}
			mTestRect.SetValue(-50, -50, GameApp.gApp.mWidth + 100, GameApp.gApp.mHeight + 100);
			if (mTestRect.Intersects((int)critter.mX, (int)critter.mY, critter.mImage.GetCelWidth(), critter.mImage.GetCelHeight()))
			{
				SetupBug(critter);
			}
		}
	}

	public override void DrawAboveBalls(Graphics g)
	{
		if (!g.Is3D())
		{
			return;
		}
		for (int i = 0; i < mBugs.Count; i++)
		{
			Critter critter = mBugs[i];
			mDrawTransform.Reset();
			mDrawTransform.RotateRad(critter.mAngle - 1.570795f);
			if (!SexyFramework.Common._eq(critter.mSize, 1f, 0.0001f))
			{
				mDrawTransform.Scale(critter.mSize, critter.mSize);
			}
			DrawBug(g, critter, mDrawTransform);
		}
	}

	public override string GetName()
	{
		return "FlyingBug";
	}
}
