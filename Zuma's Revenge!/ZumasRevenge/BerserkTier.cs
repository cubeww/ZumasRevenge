using System.Collections.Generic;

namespace ZumasRevenge;

public class BerserkTier
{
	public int mHealthLimit;

	public List<BerserkModifier> mParams;

	public BerserkTier()
	{
		mHealthLimit = 0;
		mParams = new List<BerserkModifier>();
	}

	public BerserkTier(int hl)
	{
		mHealthLimit = hl;
		mParams = new List<BerserkModifier>();
	}

	public BerserkTier(BerserkTier rhs)
	{
		mHealthLimit = rhs.mHealthLimit;
		mParams = new List<BerserkModifier>();
		for (int i = 0; i < rhs.mParams.Count; i++)
		{
			mParams.Add(new BerserkModifier(rhs.mParams[i]));
		}
	}
}
