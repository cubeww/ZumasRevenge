using SexyFramework.Graphics;

namespace ZumasRevenge;

public class EyeAnim
{
	public PIEffect mEyeFlame;

	public bool mFiring;

	public int mUpdateCount;

	public EyeAnim()
	{
	}

	public EyeAnim(EyeAnim rhs)
	{
		if (rhs != null && rhs != this)
		{
			mEyeFlame = rhs.mEyeFlame;
			mFiring = rhs.mFiring;
			mUpdateCount = rhs.mUpdateCount;
		}
	}

	public void Update(int x, int y, int alpha)
	{
		if (mFiring || mEyeFlame.mCurNumParticles != 0)
		{
			mUpdateCount++;
			mEyeFlame.mDrawTransform.LoadIdentity();
			float num = GameApp.DownScaleNum(1f);
			mEyeFlame.mDrawTransform.Scale(num, num);
			mEyeFlame.mDrawTransform.Translate(Common._S(x), Common._S(y));
			mEyeFlame.mColor.mAlpha = alpha;
			mEyeFlame.Update();
		}
	}

	public void Draw(Graphics g)
	{
		if (mFiring || mEyeFlame.mCurNumParticles != 0)
		{
			mEyeFlame.Draw(g);
		}
	}

	public void SyncState(DataSync sync)
	{
		sync.SyncLong(ref mUpdateCount);
		sync.SyncBoolean(ref mFiring);
		if (sync.isWrite())
		{
			Common.SerializePIEffect(mEyeFlame, sync);
		}
		else
		{
			Common.DeserializePIEffect(mEyeFlame, sync);
		}
	}
}
