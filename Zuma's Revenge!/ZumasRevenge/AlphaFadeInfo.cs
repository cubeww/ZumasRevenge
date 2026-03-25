using JeffLib;

namespace ZumasRevenge;

public class AlphaFadeInfo
{
	public AlphaFader first;

	public bool second;

	public AlphaFadeInfo()
	{
	}

	public AlphaFadeInfo(AlphaFader f, bool s)
	{
		first = f;
		second = s;
	}
}
