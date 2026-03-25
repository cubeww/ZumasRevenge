using SexyFramework.Graphics;

namespace ZumasRevenge;

public class AchievementText
{
	public string mHeaderStr = "";

	public string mValueStr = "";

	public string mDescStr = "";

	public string mPointStr = "";

	public float mAlpha;

	public bool mFadeIn;

	public int mX;

	public int mY;

	public Image mIcon;

	public bool mUnlocked;

	public AchievementText()
	{
		mAlpha = 0f;
		mFadeIn = true;
		mX = 0;
		mY = 0;
		mUnlocked = false;
	}
}
