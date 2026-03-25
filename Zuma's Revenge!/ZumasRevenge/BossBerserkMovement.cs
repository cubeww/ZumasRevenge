using System.Collections.Generic;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class BossBerserkMovement
{
	public int mStartX;

	public int mEndX;

	public int mStartY;

	public int mEndY;

	public int mX;

	public int mY;

	public int mHealthLimit;

	public List<Point> mPoints = new List<Point>();

	public BossBerserkMovement()
	{
		mStartX = 0;
		mStartY = 0;
		mEndX = 0;
		mEndY = 0;
		mHealthLimit = -1;
		mX = int.MaxValue;
		mY = int.MaxValue;
	}

	public BossBerserkMovement(BossBerserkMovement rhs)
	{
		mStartX = rhs.mStartX;
		mStartY = rhs.mStartY;
		mEndX = rhs.mEndX;
		mEndY = rhs.mEndY;
		mHealthLimit = rhs.mHealthLimit;
		mX = rhs.mX;
		mY = rhs.mY;
		mPoints.Clear();
		for (int i = 0; i < rhs.mPoints.Count; i++)
		{
			mPoints.Add(new Point(rhs.mPoints[i]));
		}
	}
}
