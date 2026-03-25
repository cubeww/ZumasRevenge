using SexyFramework.Graphics;

namespace ZumasRevenge;

public class HeaderLetter
{
	public Image mImage;

	public float mAngle;

	public float mAngleInc;

	public float mVX;

	public float mVY;

	public float mX;

	public float mY;

	public float mAngleAccel;

	public bool mHinge;

	public int mSwingCount;

	public int mUpdateCount;

	public HeaderLetter(Image img)
	{
		mImage = img;
	}

	public HeaderLetter()
	{
		mImage = null;
	}
}
