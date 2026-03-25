using System.Linq;
using JeffLib;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class ReversePowerEffect : PowerEffect
{
	protected float mWaypoint;

	protected float mStartWaypoint;

	protected float mRotation;

	protected float mScale;

	public CurveMgr mCurve;

	public ReversePowerEffect()
	{
	}

	public ReversePowerEffect(float x, float y, Ball b)
		: base(x, y)
	{
		mScale = 1f;
		mCurve = GameApp.gApp.GetBoard().GetCurve(b);
		mStartWaypoint = (mWaypoint = b.GetWayPoint());
		SexyVector2 pointPos = mCurve.mWayPointMgr.GetPointPos(mWaypoint);
		mX = pointPos.x;
		mY = pointPos.y;
		mRotation = mCurve.mWayPointMgr.GetRotationForPoint((int)mWaypoint);
	}

	public override void Update()
	{
		if (IsDone())
		{
			return;
		}
		base.Update();
		if (mDone)
		{
			mWaypoint -= Common._M(20);
			SexyVector2 pointPos = mCurve.mWayPointMgr.GetPointPos(mWaypoint);
			mX = pointPos.x;
			mY = pointPos.y;
			mRotation = mCurve.mWayPointMgr.GetRotationForPoint((int)mWaypoint);
			mScale = mWaypoint / mStartWaypoint;
			if (mScale < 0f)
			{
				mDone = true;
			}
		}
	}

	public override void Draw(Graphics g)
	{
		if (IsDone())
		{
			return;
		}
		g.PushState();
		g.SetColorizeImages(colorizeImages: true);
		g.SetDrawMode(1);
		int num = (mDrawReverse ? (mItems.Count() - 1) : 0);
		int num2 = ((!mDrawReverse) ? mItems.Count() : 0);
		for (int i = num; mDrawReverse ? (i >= num2) : (i < num2); i += ((!mDrawReverse) ? 1 : (-1)))
		{
			EffectItem effectItem = mItems[i];
			Color mColor = effectItem.mColor;
			mColor.mAlpha = (int)Component.GetComponentValue(effectItem.mOpacity, 255f, mUpdateCount);
			if (mColor.mAlpha != 0)
			{
				float num3 = (mDone ? mScale : Component.GetComponentValue(effectItem.mScale, 1f, mUpdateCount));
				g.SetColor(mColor);
				mGlobalTranform.Reset();
				mGlobalTranform.RotateRad(mRotation);
				mGlobalTranform.Scale(num3, num3);
				Rect celRect = effectItem.mImage.GetCelRect(effectItem.mCel);
				if (g.Is3D())
				{
					g.DrawImageTransformF(effectItem.mImage, mGlobalTranform, celRect, Common._S(mX), Common._S(mY));
				}
				else
				{
					g.DrawImageTransform(effectItem.mImage, mGlobalTranform, celRect, Common._S(mX), Common._S(mY));
				}
			}
		}
		g.PopState();
	}

	public override bool IsDone()
	{
		if (mDone)
		{
			return mWaypoint < 0f;
		}
		return false;
	}

	public override void SyncState(DataSync sync)
	{
		base.SyncState(sync);
		sync.SyncFloat(ref mWaypoint);
		sync.SyncFloat(ref mStartWaypoint);
		sync.SyncFloat(ref mRotation);
		sync.SyncFloat(ref mScale);
		sync.SyncPointer(this);
	}
}
