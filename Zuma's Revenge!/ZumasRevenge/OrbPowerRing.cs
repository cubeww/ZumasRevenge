using System;
using System.Collections.Generic;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public class OrbPowerRing : IDisposable
{
	protected List<OrbParticle> mParticles = new List<OrbParticle>();

	protected float mAngle;

	protected float mRadius;

	protected float mMaxRadius;

	protected float mAlphaFade;

	protected float mSizeFade;

	protected float mAngleInc;

	protected bool mExpanding;

	protected bool mDone;

	protected int mUpdateCount;

	public OrbPowerRing()
	{
	}

	public OrbPowerRing(float angle, float max_radius, float alpha_fade, float size_fade, float angle_inc)
	{
		mAlphaFade = alpha_fade;
		mSizeFade = size_fade;
		mAngle = angle;
		mRadius = 0f;
		mMaxRadius = max_radius;
		mExpanding = true;
		mUpdateCount = 0;
		mDone = false;
		mAngleInc = angle_inc;
	}

	public virtual void Dispose()
	{
		for (int i = 0; i < mParticles.Count; i++)
		{
			mParticles[i] = null;
		}
		mParticles.Clear();
	}

	public void Update()
	{
		if (mDone)
		{
			return;
		}
		mAngle += mAngleInc;
		mUpdateCount++;
		if (mUpdateCount > Common._M(50))
		{
			mExpanding = false;
		}
		if (mExpanding && mRadius < mMaxRadius)
		{
			mRadius += mMaxRadius / Common._M(30f);
		}
		else if (!mExpanding && mRadius > 0f)
		{
			mRadius -= mMaxRadius / Common._M(15f);
			if (mRadius < 0f)
			{
				mRadius = 0f;
			}
		}
		if ((mExpanding || mRadius > 0f) && mUpdateCount % Common._M(1) == 0)
		{
			mParticles.Add(new OrbParticle(mAngle, mRadius, mAlphaFade, mSizeFade));
		}
		bool flag = true;
		for (int i = 0; i < mParticles.Count; i++)
		{
			OrbParticle orbParticle = mParticles[i];
			orbParticle.Update();
			if (!orbParticle.IsDone())
			{
				flag = false;
				continue;
			}
			orbParticle = null;
			mParticles.RemoveAt(i);
			i--;
		}
		if (!mExpanding && flag)
		{
			mDone = true;
		}
	}

	public void Draw(Graphics g, float x, float y)
	{
		if (!mDone)
		{
			for (int i = 0; i < mParticles.Count; i++)
			{
				mParticles[i].Draw(g, x, y);
			}
		}
	}

	public bool IsDone()
	{
		return mDone;
	}

	public bool IsExpanding()
	{
		return mExpanding;
	}

	public void SyncState(DataSync sync)
	{
		sync.SyncFloat(ref mAngle);
		sync.SyncFloat(ref mRadius);
		sync.SyncFloat(ref mMaxRadius);
		sync.SyncFloat(ref mAlphaFade);
		sync.SyncFloat(ref mSizeFade);
		sync.SyncFloat(ref mAngleInc);
		sync.SyncBoolean(ref mExpanding);
		sync.SyncBoolean(ref mDone);
		sync.SyncLong(ref mUpdateCount);
		SyncListOrbParticles(sync, clear: true);
	}

	private void SyncListOrbParticles(DataSync sync, bool clear)
	{
		if (sync.isRead())
		{
			if (clear)
			{
				mParticles.Clear();
			}
			long num = sync.GetBuffer().ReadLong();
			for (int i = 0; i < num; i++)
			{
				OrbParticle orbParticle = new OrbParticle();
				orbParticle.SyncState(sync);
				mParticles.Add(orbParticle);
			}
			return;
		}
		sync.GetBuffer().WriteLong(mParticles.Count);
		foreach (OrbParticle mParticle in mParticles)
		{
			mParticle.SyncState(sync);
		}
	}
}
