using SexyFramework.Graphics;

namespace ZumasRevenge;

public class Bubble
{
	protected float mX;

	protected float mY;

	protected float mVX;

	protected float mVY;

	protected float mJiggleSpeed;

	protected bool mJiggleLeft;

	protected int mJiggleTimer;

	protected int mDefJiggleTimer;

	protected int mDelay;

	protected float mAlpha;

	protected float mAlphaDec;

	public void Init(float vx, float vy, float jiggle_speed, int jiggle_timer)
	{
		mVX = vx;
		mVY = vy;
		mJiggleSpeed = jiggle_speed;
		mDefJiggleTimer = (mJiggleTimer = jiggle_timer);
	}

	public void Update()
	{
		if (mDelay > 0)
		{
			mDelay--;
			return;
		}
		mX += mVX;
		mY += mVY;
		if (mJiggleLeft)
		{
			mX -= mJiggleSpeed;
		}
		else
		{
			mX += mJiggleSpeed;
		}
		if (--mJiggleTimer <= 0)
		{
			mJiggleLeft = !mJiggleLeft;
			mJiggleTimer = mDefJiggleTimer;
		}
		mAlpha -= mAlphaDec;
	}

	public void Draw(Graphics g)
	{
	}

	public void SetX(float x)
	{
		mX = x;
	}

	public void SetY(float y)
	{
		mY = y;
	}

	public void SetAlphaFade(float f)
	{
		mAlphaDec = f;
	}

	public void SetDelay(int d)
	{
		mDelay = d;
	}

	public float GetAlpha()
	{
		return mAlpha;
	}
}
