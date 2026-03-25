using SexyFramework.PIL;

namespace ZumasRevenge;

public class TriggeredEffect
{
	public SexyFramework.PIL.System mRings;

	public SexyFramework.PIL.System mRainbow;

	public SexyFramework.PIL.System mGas;

	public SexyFramework.PIL.System mFlare;

	public SexyFramework.PIL.System mTrail;

	public TriggeredEffect(bool create)
	{
		if (create)
		{
			mRings = new SexyFramework.PIL.System(50, 50);
			mRings.mHighWatermark = Common._M(80);
			mRings.mLowWatermark = Common._M(60);
			mRings.mFPSCallback = SexyFramework.PIL.System.FadeParticlesFPSCallback;
			mRings.mScale = Common._S(1f);
			mRings.WaitForEmitters(w: true);
			mRainbow = new SexyFramework.PIL.System(50, 50);
			mRainbow.mHighWatermark = Common._M(80);
			mRainbow.mLowWatermark = Common._M(60);
			mRainbow.mFPSCallback = SexyFramework.PIL.System.FadeParticlesFPSCallback;
			mRainbow.mScale = Common._S(1f);
			mRainbow.WaitForEmitters(w: true);
			mGas = new SexyFramework.PIL.System(50, 50);
			mGas.mHighWatermark = Common._M(80);
			mGas.mLowWatermark = Common._M(60);
			mGas.mFPSCallback = SexyFramework.PIL.System.FadeParticlesFPSCallback;
			mGas.mScale = Common._S(1f);
			mGas.WaitForEmitters(w: true);
			mFlare = new SexyFramework.PIL.System(50, 50);
			mFlare.mHighWatermark = Common._M(80);
			mFlare.mLowWatermark = Common._M(60);
			mFlare.mFPSCallback = SexyFramework.PIL.System.FadeParticlesFPSCallback;
			mFlare.mScale = Common._S(1f);
			mFlare.WaitForEmitters(w: true);
			mTrail = new SexyFramework.PIL.System(150, 50);
			mTrail.mHighWatermark = Common._M(80);
			mTrail.mLowWatermark = Common._M(60);
			mTrail.mFPSCallback = SexyFramework.PIL.System.FadeParticlesFPSCallback;
			mTrail.mScale = Common._S(1f);
			mTrail.WaitForEmitters(w: true);
		}
	}

	public TriggeredEffect()
		: this(create: true)
	{
	}

	public virtual void Dispose()
	{
		if (mRings != null)
		{
			mRings.Dispose();
			mRings = null;
		}
		if (mRainbow != null)
		{
			mRainbow.Dispose();
			mRainbow = null;
		}
		if (mGas != null)
		{
			mGas.Dispose();
			mGas = null;
		}
		if (mFlare != null)
		{
			mFlare.Dispose();
			mFlare = null;
		}
		if (mTrail != null)
		{
			mTrail.Dispose();
			mTrail = null;
		}
	}
}
