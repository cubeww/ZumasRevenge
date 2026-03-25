namespace SexyFramework.Misc;

public class MTAutoDisallowRand
{
	public MTAutoDisallowRand()
	{
		MTRand.SetRandAllowed(allowed: false);
	}

	public void Dispose()
	{
		MTRand.SetRandAllowed(allowed: true);
	}
}
