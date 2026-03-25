namespace ZumasRevenge;

public class PathPoint
{
	public float x;

	public float y;

	public float mDist;

	public float t;

	public byte mPriority;

	public bool mInTunnel;

	public bool mEndPoint;

	public bool mSplinePoint;

	public bool mSelected;

	public PathPoint(float tx, float ty, float dist)
	{
		x = tx;
		y = ty;
		mDist = dist;
		t = 0f;
		mPriority = 0;
		mInTunnel = false;
		mEndPoint = false;
		mSplinePoint = false;
		mSelected = false;
	}

	public PathPoint(float tx, float ty)
	{
		x = tx;
		y = ty;
		mDist = 0f;
		t = 0f;
		mPriority = 0;
		mInTunnel = false;
		mEndPoint = false;
		mSplinePoint = false;
		mSelected = false;
	}

	public PathPoint()
	{
		x = 0f;
		y = 0f;
		mDist = 0f;
		t = 0f;
		mPriority = 0;
		mInTunnel = false;
		mEndPoint = false;
		mSplinePoint = false;
		mSelected = false;
	}
}
