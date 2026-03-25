using SexyFramework.Graphics;

namespace ZumasRevenge;

public class BossText
{
	public string mText = "";

	public int mTextId = -1;

	public float mAlpha;

	public Color mColor = default(Color);

	public BossText()
	{
	}

	public BossText(string t)
	{
		mAlpha = 0f;
		mText = t;
	}
}
