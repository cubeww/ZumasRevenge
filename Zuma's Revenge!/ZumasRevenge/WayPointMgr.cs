using System;
using System.Collections.Generic;
using System.Linq;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class WayPointMgr
{
	protected List<WayPoint> mWayPoints = new List<WayPoint>();

	protected void DrawCurvePiece(Graphics g, int theWayPoint, float theThickness)
	{
		if (theWayPoint != 0)
		{
			WayPoint wayPoint = mWayPoints[theWayPoint - 1];
			WayPoint wayPoint2 = mWayPoints[theWayPoint];
			if (!(Math.Abs(wayPoint.x - wayPoint2.x) > 5f) && !(Math.Abs(wayPoint.y - wayPoint2.y) > 5f))
			{
				SexyVector3 sexyVector = CalcPerpendicular(theWayPoint - 1) * theThickness;
				SexyVector3 sexyVector2 = CalcPerpendicular(theWayPoint) * theThickness;
				SexyVector3 sexyVector3 = new SexyVector3(Common._S(wayPoint.x), Common._S(wayPoint.y), 0f) - sexyVector;
				SexyVector3 sexyVector4 = new SexyVector3(Common._S(wayPoint.x), Common._S(wayPoint.y), 0f) + sexyVector;
				SexyVector3 sexyVector5 = new SexyVector3(Common._S(wayPoint2.x), Common._S(wayPoint2.y), 0f) + sexyVector2;
				SexyVector3 sexyVector6 = new SexyVector3(Common._S(wayPoint2.x), Common._S(wayPoint2.y), 0f) - sexyVector2;
				Point[] theVertexList = new Point[4]
				{
					new Point((int)sexyVector3.x, (int)sexyVector3.y),
					new Point((int)sexyVector4.x, (int)sexyVector4.y),
					new Point((int)sexyVector5.x, (int)sexyVector5.y),
					new Point((int)sexyVector6.x, (int)sexyVector6.y)
				};
				g.PolyFill(theVertexList, 4, convex: false);
			}
		}
	}

	public WayPointMgr()
	{
	}

	public WayPointMgr(WayPointMgr rhs)
	{
		if (rhs != null)
		{
			mWayPoints.AddRange(rhs.mWayPoints.ToArray());
		}
	}

	public virtual void Dispose()
	{
		mWayPoints.Clear();
	}

	public void SetWayPoint(Ball theBall, float thePoint, bool loop_at_end)
	{
		if (mWayPoints.Count == 0)
		{
			return;
		}
		int num = (int)thePoint;
		int num2;
		if (num < 0)
		{
			num = 0;
			num2 = 1;
		}
		else if (num >= mWayPoints.Count)
		{
			if (!loop_at_end)
			{
				num = mWayPoints.Count - 1;
				num2 = num + 1;
			}
			else
			{
				num = (int)thePoint % mWayPoints.Count;
				num2 = ((int)thePoint + 1) % mWayPoints.Count;
			}
		}
		else
		{
			num2 = num + 1;
		}
		WayPoint wayPoint = mWayPoints[num];
		WayPoint wayPoint2 = wayPoint;
		if (num2 < mWayPoints.Count)
		{
			wayPoint2 = mWayPoints[num2];
		}
		float x = theBall.GetX();
		float y = theBall.GetY();
		if (Math.Abs(wayPoint2.x - wayPoint.x) > 5f || Math.Abs(wayPoint2.y - wayPoint.y) > 5f)
		{
			theBall.SetPos(wayPoint.x, wayPoint.y);
		}
		else
		{
			float num3 = thePoint - (float)(int)thePoint;
			theBall.SetPos(num3 * (wayPoint2.x - wayPoint.x) + wayPoint.x, num3 * (wayPoint2.y - wayPoint.y) + wayPoint.y);
		}
		bool immediate = Math.Abs(theBall.GetX() - x) + Math.Abs(theBall.GetY() - y) > 10f;
		CalcAvgRotationForPoint(num);
		theBall.SetRotation(wayPoint.mAvgRotation, immediate);
		theBall.SetWayPoint(thePoint, wayPoint.mInTunnel);
	}

	public void SetWayPointInt(Ball theBall, int thePoint, bool loop_at_end)
	{
		if (mWayPoints.Count != 0)
		{
			int num = thePoint;
			if (num < 0)
			{
				num = 0;
			}
			else if (num >= mWayPoints.Count)
			{
				num = ((!loop_at_end) ? (mWayPoints.Count - 1) : (thePoint % mWayPoints.Count));
			}
			WayPoint wayPoint = mWayPoints[num];
			CalcAvgRotationForPoint(num);
			theBall.SetPos(wayPoint.x, wayPoint.y);
			theBall.SetWayPoint(thePoint, wayPoint.mInTunnel);
			theBall.SetRotation(wayPoint.mAvgRotation, immediate: false);
		}
	}

	public void FindFreeWayPoint(Ball theExistingBall, Ball theNewBall, bool inFront, bool loop_at_end, int thePad)
	{
		int num = (inFront ? 1 : (-1));
		int num2 = (int)theExistingBall.GetWayPoint();
		if (inFront && theNewBall.GetWayPoint() > (float)num2)
		{
			num2 = (int)theNewBall.GetWayPoint();
		}
		else if (!inFront && theNewBall.GetWayPoint() < (float)num2)
		{
			num2 = (int)theNewBall.GetWayPoint();
		}
		WayPoint wayPoint = null;
		while (num2 >= 0 && (loop_at_end || num2 < mWayPoints.Count))
		{
			wayPoint = mWayPoints[num2 % mWayPoints.Count];
			theNewBall.SetPos(wayPoint.x, wayPoint.y);
			if (!theExistingBall.CollidesWithPhysically(theNewBall, thePad))
			{
				break;
			}
			int num3 = num2 % mWayPoints.Count;
			num2 += num;
			if (loop_at_end && num3 + num < 0)
			{
				num2 -= num;
				break;
			}
		}
		SetWayPointInt(theNewBall, num2, loop_at_end);
	}

	public void FindFreeWayPoint(Ball theExistingBall, Ball theNewBall, bool inFront, bool loop_at_end)
	{
		FindFreeWayPoint(theExistingBall, theNewBall, inFront, loop_at_end, 0);
	}

	public SexyVector3 CalcPerpendicular(float theWayPoint)
	{
		int num = (int)theWayPoint;
		if (num < 0)
		{
			num = 0;
		}
		if (num >= mWayPoints.Count)
		{
			num = mWayPoints.Count - 1;
		}
		WayPoint wayPoint = mWayPoints[num];
		CalcPerpendicularForPoint(num);
		return wayPoint.mPerpendicular;
	}

	public SexyVector2 GetPointPos(float thePoint)
	{
		int num = (int)thePoint;
		if (num < 0)
		{
			num = 0;
		}
		else if (num >= mWayPoints.Count)
		{
			num = mWayPoints.Count - 1;
		}
		WayPoint wayPoint = mWayPoints[num];
		return new SexyVector2(wayPoint.x, wayPoint.y);
	}

	public float GetRotationForPoint(int theWayPoint)
	{
		if (theWayPoint < 0)
		{
			theWayPoint = 0;
		}
		if (theWayPoint >= mWayPoints.Count() - 1)
		{
			theWayPoint = mWayPoints.Count() - 1;
		}
		WayPoint wayPoint = mWayPoints[theWayPoint];
		CalcPerpendicularForPoint(theWayPoint);
		return wayPoint.mRotation;
	}

	public void CalcPerpendicularForPoint(int theWayPoint)
	{
		WayPoint wayPoint = mWayPoints[theWayPoint];
		if (wayPoint.mHavePerpendicular)
		{
			return;
		}
		WayPoint wayPoint2 = wayPoint;
		bool flag = false;
		if (theWayPoint + 1 < mWayPoints.Count())
		{
			wayPoint2 = mWayPoints[theWayPoint + 1];
			if ((Math.Abs(wayPoint.x - wayPoint2.x) > 5f || Math.Abs(wayPoint.y - wayPoint2.y) > 5f) && theWayPoint > 0)
			{
				flag = true;
				wayPoint2 = mWayPoints[theWayPoint - 1];
			}
		}
		else
		{
			wayPoint2 = mWayPoints[theWayPoint - 1];
			if ((Math.Abs(wayPoint.x - wayPoint2.x) > 5f || Math.Abs(wayPoint.y - wayPoint2.y) > 5f) && theWayPoint + 1 < mWayPoints.Count())
			{
				wayPoint2 = mWayPoints[theWayPoint + 1];
			}
			else
			{
				flag = true;
			}
		}
		if (flag)
		{
			wayPoint.mPerpendicular = new SexyVector3(wayPoint.y - wayPoint2.y, wayPoint2.x - wayPoint.x, 0f);
		}
		else
		{
			wayPoint.mPerpendicular = new SexyVector3(wayPoint2.y - wayPoint.y, wayPoint.x - wayPoint2.x, 0f);
		}
		wayPoint.mPerpendicular = wayPoint.mPerpendicular.Normalize();
		wayPoint.mRotation = (float)Math.Acos(wayPoint.mPerpendicular.Dot(new SexyVector3(1f, 0f, 0f)));
		if (wayPoint.mPerpendicular.y > 0f)
		{
			wayPoint.mRotation *= -1f;
		}
		if (wayPoint.mRotation < 0f)
		{
			wayPoint.mRotation += (float)Math.PI * 2f;
		}
		wayPoint.mHavePerpendicular = true;
	}

	public void CalcAvgRotationForPoint(int theWayPoint)
	{
		WayPoint wayPoint = mWayPoints[theWayPoint];
		if (wayPoint.mHaveAvgRotation)
		{
			return;
		}
		CalcPerpendicularForPoint(theWayPoint);
		wayPoint.mHaveAvgRotation = true;
		wayPoint.mAvgRotation = wayPoint.mRotation;
		int num = theWayPoint - 10;
		int num2 = theWayPoint + 10;
		if (num < 0)
		{
			num = 0;
		}
		if (num2 >= mWayPoints.Count())
		{
			num2 = mWayPoints.Count() - 1;
		}
		float num3 = 0f;
		CalcPerpendicularForPoint(num);
		for (int i = num + 1; i < num2; i++)
		{
			CalcPerpendicularForPoint(i);
			num3 = WayPoint.GetCanonicalAngle(mWayPoints[i].mRotation - mWayPoints[i - 1].mRotation);
			if (num3 > 0.1f || num3 < -0.1f)
			{
				WayPoint wayPoint2 = mWayPoints[i];
				WayPoint wayPoint3 = mWayPoints[i - 1];
				if (!(Math.Abs(wayPoint2.x - wayPoint3.x) > 5f) && !(Math.Abs(wayPoint2.y - wayPoint3.y) > 5f))
				{
					float num4 = 1f - (float)(i - num) / (float)(num2 - num);
					wayPoint.mAvgRotation = mWayPoints[num].mRotation + num4 * num3;
					break;
				}
			}
		}
	}

	public int GetPriority(Ball theBall)
	{
		int priority = GetPriority((int)(theBall.GetWayPoint() - (float)theBall.GetRadius()));
		int priority2 = GetPriority((int)(theBall.GetWayPoint() + (float)theBall.GetRadius()));
		return Math.Max(priority, priority2);
	}

	public int GetPriority(int thePoint)
	{
		if (thePoint < 0 || thePoint >= mWayPoints.Count())
		{
			return 0;
		}
		return mWayPoints[thePoint].mPriority;
	}

	public int GetPriority(Bullet theBullet)
	{
		if (theBullet.GetWayPoint() == 0f || theBullet.GetHitPercent() < 0.7f)
		{
			return 4;
		}
		return GetPriority((Ball)theBullet);
	}

	public bool LoadCurve(string theFile, CurveDesc desc, MirrorType theMirror)
	{
		string text = theFile;
		if (-1 != text.LastIndexOf('.'))
		{
			text = text.Substring(0, text.LastIndexOf('.'));
		}
		mWayPoints.Clear();
		CurveData curveData = new CurveData();
		if (!curveData.Load(text))
		{
			Console.WriteLine("FAILED TO OPEN FILE %s\n", theFile);
			for (int i = 0; i < 400; i++)
			{
				mWayPoints.Add(new WayPoint(i, 100f));
			}
		}
		desc.GetValuesFrom(curveData);
		bool flag = false;
		List<PathPoint> mPointList = curveData.mPointList;
		bool flag2 = mPointList.First().mInTunnel;
		int num = 15;
		foreach (PathPoint item in mPointList)
		{
			float x = item.x;
			float y = item.y;
			if (!flag && x >= 0f && y >= 0f && x <= 800f && y <= 600f)
			{
				flag = true;
			}
			WayPoint wayPoint = new WayPoint(item.x, item.y);
			mWayPoints.Add(wayPoint);
			wayPoint.mInTunnel = item.mInTunnel;
			if (flag2 && item.mInTunnel)
			{
				num = mWayPoints.Count;
			}
			else
			{
				flag2 = false;
			}
			int num2 = item.mPriority;
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num2 >= 5)
			{
				num2 = 4;
			}
			wayPoint.mPriority = item.mPriority;
		}
		desc.mCutoffPoint = num - Common.GetDefaultBallRadius();
		if (theMirror != MirrorType.MirrorType_None)
		{
			for (int j = 0; j < mWayPoints.Count(); j++)
			{
				WayPoint wayPoint2 = mWayPoints[j];
				Common.MirrorPoint(ref wayPoint2.x, ref wayPoint2.y, theMirror);
			}
		}
		return true;
	}

	public void DrawCurve(Graphics g, Color theColor, int theDangerPoint)
	{
		if (mWayPoints.Count != 0)
		{
			float theThickness = 5f;
			for (int i = 1; i < mWayPoints.Count; i++)
			{
				g.SetColor(theColor);
				DrawCurvePiece(g, i, theThickness);
			}
		}
	}

	public void DrawTunnel(Graphics g, int priority)
	{
		if (mWayPoints.Count == 0)
		{
			return;
		}
		for (int i = 1; i < mWayPoints.Count; i++)
		{
			WayPoint wayPoint = mWayPoints[i];
			if (wayPoint.mInTunnel && wayPoint.mPriority == priority)
			{
				g.SetColor(0, 0, 0, 160);
				DrawCurvePiece(g, i, 25f);
			}
		}
	}

	public bool InTunnel(int theWayPoint)
	{
		if (theWayPoint < 0)
		{
			return true;
		}
		if (theWayPoint >= mWayPoints.Count())
		{
			return false;
		}
		return mWayPoints[theWayPoint].mInTunnel;
	}

	public bool InTunnel(Ball theBall, bool inFront)
	{
		int num = (int)theBall.GetWayPoint();
		num = ((!inFront) ? (num - theBall.GetRadius()) : (num + theBall.GetRadius()));
		if (InTunnel(num))
		{
			return true;
		}
		return false;
	}

	public bool InTunnel(Bullet theBullet)
	{
		Ball hitBall = theBullet.GetHitBall();
		if (hitBall == null)
		{
			return false;
		}
		int num = (int)hitBall.GetWayPoint();
		num = ((!theBullet.GetHitInFront()) ? (num - 3 * hitBall.GetRadius()) : (num + 3 * hitBall.GetRadius()));
		return InTunnel(num);
	}

	public bool CheckDiscontinuity(int thePoint, int theDist)
	{
		int i = thePoint;
		int num = thePoint + theDist;
		if (i < 0)
		{
			i = 0;
		}
		if (i > mWayPoints.Count())
		{
			i = mWayPoints.Count();
		}
		if (num < 0)
		{
			num = 0;
		}
		if (num > mWayPoints.Count())
		{
			num = mWayPoints.Count();
		}
		if (i >= num)
		{
			return false;
		}
		WayPoint wayPoint = mWayPoints[i++];
		for (; i < num; i++)
		{
			WayPoint wayPoint2 = mWayPoints[i];
			float num2 = Math.Abs(wayPoint.x - wayPoint2.x) + Math.Abs(wayPoint.y - wayPoint2.y);
			if (num2 > 10f)
			{
				return true;
			}
			wayPoint = wayPoint2;
		}
		return false;
	}

	public int GetNumPoints()
	{
		return mWayPoints.Count();
	}

	public int GetEndPoint()
	{
		return mWayPoints.Count() - 1;
	}

	public List<WayPoint> GetWayPointList()
	{
		return mWayPoints;
	}
}
