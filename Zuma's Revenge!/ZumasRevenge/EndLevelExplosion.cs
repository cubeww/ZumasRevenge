using System;
using SexyFramework;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public class EndLevelExplosion : IDisposable
{
	public int mDelay;

	public int mX;

	public int mY;

	public PIEffect mPIEffect;

	public EndLevelExplosion()
	{
		mPIEffect = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_END_LEVEL_EXPLOSION).Duplicate();
		Common.SetFXNumScale(mPIEffect, GlobalMembers.gSexyAppBase.Is3DAccelerated() ? 1f : Common._M(0.5f));
	}

	public virtual void Dispose()
	{
		if (mPIEffect != null)
		{
			mPIEffect.Dispose();
		}
		mPIEffect = null;
	}

	public void SetPos(int x, int y)
	{
		mPIEffect.mDrawTransform.LoadIdentity();
		float num = GameApp.DownScaleNum(1f);
		mPIEffect.mDrawTransform.Scale(num, num);
		mPIEffect.mDrawTransform.Translate(Common._S(x), Common._S(y));
	}
}
