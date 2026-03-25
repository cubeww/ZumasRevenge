using SexyFramework.Graphics;

namespace ZumasRevenge;

public class GenericCachedEffect
{
	public bool mInUse;

	public PIEffect mEffect;

	public GenericCachedEffect(PIEffect e)
	{
		mInUse = false;
		mEffect = e;
	}
}
