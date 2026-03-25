namespace ZumasRevenge;

public struct ScoreTip
{
	public string mTip;

	public int mTipId;

	public int mMinLevel;

	public ScoreTip(string t, int l)
	{
		mTip = t;
		mMinLevel = l;
		mTipId = -1;
	}

	public ScoreTip(string t)
	{
		mTip = t;
		mMinLevel = -1;
		mTipId = -1;
	}
}
