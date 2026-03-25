namespace ZumasRevenge;

public class EffectParams
{
	public string mKey;

	public string mValue;

	public int mEffectIndex;

	public EffectParams()
	{
		mEffectIndex = -1;
	}

	public EffectParams(string k, string v, int i)
	{
		mKey = k;
		mValue = v;
		mEffectIndex = i;
	}
}
