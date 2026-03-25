using System;
using System.Collections.Generic;
using JeffLib;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class WillOWisp : FlyingBug
{
	protected static int mSpawnCounter = 0;

	protected static int NUM_WISP_COLORS = 4;

	protected static FColor[] WISP_COLORS = new FColor[4]
	{
		new FColor(255f, 255f, 0f),
		new FColor(141f, 141f, 255f),
		new FColor(0f, 0f, 255f),
		new FColor(255f, 179f, 179f)
	};

	protected static float GetVel()
	{
		return Common._M(0.05f) + (float)(SexyFramework.Common.SafeRand() % Common._M1(10)) / Common._M2(100f);
	}

	protected static FColor GetTargetColor(FColor curr_color)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < NUM_WISP_COLORS; i++)
		{
			if (WISP_COLORS[i] != curr_color)
			{
				list.Add(i);
			}
		}
		return WISP_COLORS[list[SexyFramework.Common.SafeRand() % list.Count]];
	}

	protected override void SetupBug(Critter d)
	{
		d.mImage = null;
		base.SetupBug(d);
		if (++mSpawnCounter < Common._M(2) && mBugs.Count > 1)
		{
			Critter critter = mBugs[mBugs.Count - 2];
			d.mX = critter.mX;
			d.mY = critter.mY;
			d.mAngle = critter.mAngle;
			mSpawnCounter = 0;
		}
		d.mInitVel = GetVel();
		d.mVX = (float)Math.Cos(d.mAngle) * d.mInitVel;
		d.mVY = (0f - (float)Math.Sin(d.mAngle)) * d.mInitVel;
		d.mFader.mColor = (d.mFader.mMinColor = GetTargetColor(new FColor(0f, 0f, 0f)));
		d.mFader.mMaxColor = GetTargetColor(d.mFader.mColor);
		d.mFader.FadeOverTime(Common._M(200) + SexyFramework.Common.SafeRand() % Common._M1(300));
	}

	public WillOWisp()
	{
		mMinBugs = Common._M(10);
		mMaxBugs = Common._M(15);
		mNoEnterRect = new Rect(Common._S(Common._M(100)), Common._S(Common._M1(100)), Common._S(Common._M2(600)), Common._S(Common._M3(400)));
		mReverseTimer = Common._M(100);
		mReverseRotateDelay = Common._M(100);
		mRotateMinTimer = Common._M(300);
		mRotateMaxTimer = Common._M(400);
		mFlyingMinTimer = Common._M(10);
		mFlyingMaxTimer = Common._M(25);
		mDefaultAnimRate = Common._M(12);
		mRestingMinTimer = Common._M(100);
		mRestingMaxTimer = Common._M(500);
		mMaxRotateDegrees = Common._M(360);
		mAllowRest = false;
		mNoRotateUntilOnScreen = true;
	}

	public override void Update()
	{
		base.Update();
		for (int i = 0; i < mBugs.Count; i++)
		{
			Critter critter = mBugs[i];
			if (critter.mFader.Update())
			{
				if (critter.mFader.mForward)
				{
					critter.mFader.mMaxColor = GetTargetColor(critter.mFader.mColor);
				}
				else
				{
					critter.mFader.mMinColor = GetTargetColor(critter.mFader.mColor);
				}
				critter.mFader.FadeOverTime(Common._M(200) + SexyFramework.Common.SafeRand() % Common._M1(300));
			}
		}
	}

	public override void DrawBug(Graphics g, Critter d, Transform t)
	{
	}

	public override string GetName()
	{
		return "WillOWisp";
	}
}
