using System;
using SexyFramework.AELib;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class Tiki : IDisposable
{
	protected int mUpdateCount;

	protected Rect mCollRect = default(Rect);

	protected bool mDoExplosion;

	protected Boss mBoss;

	public int mRailStartX;

	public int mRailStartY;

	public int mRailEndX;

	public int mRailEndY;

	public int mTravelTime;

	public int mId = -1;

	public int mAlphaFadeDir = 1;

	public int mAlpha;

	public float mX;

	public float mY;

	public bool mWasHit;

	public bool mIsLeftTiki;

	public float mVX;

	public Composition mComp;

	public PIEffect mExplosion;

	public Tiki()
	{
	}

	public Tiki(Tiki rhs)
		: this()
	{
		CopyFrom(rhs);
	}

	public void CopyFrom(Tiki rhs)
	{
		mUpdateCount = rhs.mUpdateCount;
		mCollRect = new Rect(rhs.mCollRect);
		mDoExplosion = rhs.mDoExplosion;
		mBoss = rhs.mBoss;
		mRailStartX = rhs.mRailStartX;
		mRailStartY = rhs.mRailStartY;
		mRailEndX = rhs.mRailEndX;
		mRailEndY = rhs.mRailEndY;
		mTravelTime = rhs.mTravelTime;
		mId = rhs.mId;
		mAlphaFadeDir = rhs.mAlphaFadeDir;
		mAlpha = rhs.mAlpha;
		mX = rhs.mX;
		mY = rhs.mY;
		mWasHit = rhs.mWasHit;
		mIsLeftTiki = rhs.mIsLeftTiki;
		mVX = rhs.mVX;
		mComp = rhs.mComp;
		mExplosion = rhs.mExplosion;
	}

	public virtual void Dispose()
	{
		if (mExplosion != null)
		{
			mExplosion.Dispose();
			mExplosion = null;
		}
	}

	public void Init(Boss b)
	{
		mBoss = b;
	}

	public void Update()
	{
		if (mDoExplosion)
		{
			mExplosion.mDrawTransform.LoadIdentity();
			float num = GameApp.DownScaleNum(1f);
			mExplosion.mDrawTransform.Scale(num, num);
			mExplosion.mDrawTransform.Translate(Common._S(mX) + (float)Common._DS(Common._M(80)), Common._S(mY) + (float)Common._DS(Common._M1(150)));
			mExplosion.Update();
			if (mExplosion.mFrameNum > (float)mExplosion.mLastFrameNum)
			{
				mDoExplosion = false;
			}
		}
		mComp.Update();
		mAlpha += mAlphaFadeDir * Common._M(12);
		if (mAlpha < 0)
		{
			mAlpha = 0;
		}
		else if (mAlpha > 255)
		{
			mAlpha = 255;
		}
		if (!mDoExplosion && ((mVX > 0f && mX + (float)mCollRect.mX > (float)mRailEndX) || (mVX < 0f && mX + (float)mCollRect.mX < (float)mRailStartX)))
		{
			mX = ((mVX > 0f) ? (mRailEndX - mCollRect.mX) : (mRailStartX - mCollRect.mX));
			mVX *= -1f;
		}
		mX += mVX;
		mUpdateCount++;
	}

	public void Draw(Graphics g)
	{
		if (mAlpha > 0)
		{
			CumulativeTransform cumulativeTransform = new CumulativeTransform();
			cumulativeTransform.mOpacity = (float)mAlpha / 255f;
			if (mBoss != null && mBoss.mAlphaOverride <= 254f)
			{
				cumulativeTransform.mOpacity = mBoss.mAlphaOverride / 255f;
			}
			cumulativeTransform.mTrans.Translate(Common._S(mX - (float)mCollRect.mX), Common._S(mY - (float)mCollRect.mY));
			mComp.Draw(g, cumulativeTransform, -1, Common._DS(1f));
		}
		if (g.Is3D() && mDoExplosion)
		{
			if (mBoss != null && mBoss.mAlphaOverride <= 254f)
			{
				mExplosion.mColor.mAlpha = (int)(mBoss.mAlphaOverride / 255f);
			}
			mExplosion.Draw(g);
		}
	}

	public void SetIsLeft(bool l)
	{
		mIsLeftTiki = l;
		mCollRect = new Rect(Common._M(74), Common._M1(70), Common._M2(75), Common._M4(104));
		mExplosion = null;
		mExplosion = (mIsLeftTiki ? GameApp.gApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_CIRCLEEXPLOSIONTIKI").Duplicate() : GameApp.gApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_TRIANGLEEXPLOSIONTIKI").Duplicate());
	}

	public bool Collides(Bullet b, ref bool should_destroy)
	{
		Rect rect = new Rect(mCollRect);
		rect.mX = (int)mX;
		rect.mY = (int)mY;
		Rect theTRect = new Rect((int)b.GetX() - b.GetRadius(), (int)b.GetY() - b.GetRadius(), b.GetRadius() * 2, b.GetRadius() * 2);
		should_destroy = false;
		if (mWasHit || mAlphaFadeDir < 0 || !rect.Intersects(theTRect))
		{
			return false;
		}
		should_destroy = true;
		mWasHit = true;
		mAlphaFadeDir = -1;
		mDoExplosion = true;
		mExplosion.ResetAnim();
		return true;
	}
}
