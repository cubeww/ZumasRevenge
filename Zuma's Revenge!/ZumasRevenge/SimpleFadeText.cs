namespace ZumasRevenge;

public class SimpleFadeText
{
	public string mString;

	public float mAlpha;

	public bool mFadeIn;

	public SimpleFadeText()
	{
		mAlpha = 0f;
		mFadeIn = true;
	}

	public SimpleFadeText(string str)
		: this()
	{
		mString = str;
	}
}
