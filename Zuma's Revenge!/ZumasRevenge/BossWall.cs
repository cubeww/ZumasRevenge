namespace ZumasRevenge;

public class BossWall
{
	public int mX;

	public int mY;

	public int mWidth;

	public int mHeight;

	public int mId;

	public int mAlphaFadeDir;

	public int mAlpha;

	public BossWall()
	{
	}

	public BossWall(BossWall rhs)
	{
		mX = rhs.mX;
		mY = rhs.mY;
		mWidth = rhs.mWidth;
		mHeight = rhs.mHeight;
		mId = rhs.mId;
		mAlphaFadeDir = rhs.mAlphaFadeDir;
		mAlpha = rhs.mAlpha;
	}
}
