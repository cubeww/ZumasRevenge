using SexyFramework.PIL;

namespace ZumasRevenge;

public class SpawnEffect
{
	public SexyFramework.PIL.System mRings;

	public SexyFramework.PIL.System mSwirl;

	public SpawnEffect(bool create)
	{
		if (create)
		{
			mRings = new SexyFramework.PIL.System(100, 50);
			mRings.mParticleScale2D = 0.3f;
			mRings.mScale = Common._S(1f);
			mRings.mHighWatermark = Common._M(80);
			mRings.mLowWatermark = Common._M(60);
			mRings.mFPSCallback = SexyFramework.PIL.System.FadeParticlesFPSCallback;
			mRings.WaitForEmitters(w: true);
			mSwirl = new SexyFramework.PIL.System(100, 50);
			mSwirl.mHighWatermark = Common._M(80);
			mSwirl.mLowWatermark = Common._M(60);
			mSwirl.mFPSCallback = SexyFramework.PIL.System.FadeParticlesFPSCallback;
			mSwirl.mParticleScale2D = 0.3f;
			mSwirl.mScale = Common._S(1f);
			mSwirl.WaitForEmitters(w: true);
		}
	}

	public SpawnEffect()
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
		if (mSwirl != null)
		{
			mSwirl.Dispose();
			mSwirl = null;
		}
	}
}
