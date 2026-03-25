using SexyFramework.Graphics;

namespace ZumasRevenge;

public class LeaderBoardText
{
	public string mHeaderStr = "";

	public string mValueStr = "";

	public float mAlpha;

	public bool mFadeIn;

	public int mX;

	public int mY;

	public Image mIcon;

	public bool mShowIcon;

	public LeaderBoardText()
	{
		mAlpha = 0f;
		mFadeIn = true;
		mX = 0;
		mY = 0;
	}
}
