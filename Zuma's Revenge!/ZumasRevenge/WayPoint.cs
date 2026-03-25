using System;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class WayPoint
{
	public float x;

	public float y;

	public bool mHavePerpendicular;

	public bool mHaveAvgRotation;

	public SexyVector3 mPerpendicular = default(SexyVector3);

	public float mRotation;

	public float mAvgRotation;

	public bool mInTunnel;

	public byte mPriority;

	public WayPoint()
	{
		mHavePerpendicular = false;
		mHaveAvgRotation = false;
		mInTunnel = false;
		mHavePerpendicular = false;
		mPriority = 0;
	}

	public WayPoint(float theX, float theY)
	{
		x = theX;
		y = theY;
		mHavePerpendicular = false;
		mHaveAvgRotation = false;
		mInTunnel = false;
		mHavePerpendicular = false;
		mPriority = 0;
	}

	public static float GetCanonicalAngle(float r)
	{
		if (r > 0f)
		{
			while (r > (float)Math.PI)
			{
				r -= (float)Math.PI * 2f;
			}
		}
		else if (r < 0f)
		{
			while (r < -(float)Math.PI)
			{
				r += (float)Math.PI * 2f;
			}
		}
		return r;
	}
}
