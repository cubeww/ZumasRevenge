using System;
using SexyFramework;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public class BallExplosion : IDisposable
{
	public PIEffect mPIEffect;

	public BallExplosion()
	{
		mPIEffect = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_BALL_EXPLODE).Duplicate();
		Common.SetFXNumScale(mPIEffect, GlobalMembers.gSexyAppBase.Is3DAccelerated() ? 1f : Common._M(0.3f));
	}

	public virtual void Dispose()
	{
		if (mPIEffect != null)
		{
			mPIEffect.Dispose();
		}
		mPIEffect = null;
	}

	public void Init()
	{
	}

	public void Release()
	{
	}

	public bool Update()
	{
		if (mPIEffect == null)
		{
			return true;
		}
		mPIEffect.Update();
		if (mPIEffect.IsActive())
		{
			return false;
		}
		return true;
	}

	public void Draw(Graphics g)
	{
		mPIEffect.Draw(g);
	}

	public void SetPos(int x, int y)
	{
		mPIEffect.mDrawTransform.LoadIdentity();
		float num = GameApp.DownScaleNum(1f);
		mPIEffect.mDrawTransform.Scale(num, num);
		mPIEffect.mDrawTransform.Translate(Common._S(x), Common._S(y));
	}
}
