using SexyFramework.AELib;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public class ShieldQuadrantData
{
	public CompositionMgr mCompMgr;

	public PIEffect mSparkles;

	public bool mDoHitAnim;

	public bool mDoExplodeAnim;

	public ShieldQuadrantData()
	{
	}

	public ShieldQuadrantData(CompositionMgr cm, PIEffect s)
	{
		mCompMgr = cm;
		mSparkles = s;
	}

	public virtual void Dispose()
	{
		if (mCompMgr != null)
		{
			mCompMgr = null;
		}
		if (mSparkles != null)
		{
			mSparkles.Dispose();
			mSparkles = null;
		}
	}
}
