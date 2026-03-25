using System;
using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class Upsell : Widget, ButtonListener, IDisposable
{
	private static float gZoomStart = 4f;

	private static int gScreenshotTimer = 300;

	private static int gScreenshotFade = 50;

	private static readonly int MAX_SCREENSHOTS = 9;

	protected float mBlock1X;

	protected float mBlock2X;

	protected float mZoom;

	protected ButtonWidget mMenuBtn;

	protected ButtonWidget mBuyBtn;

	protected int mState;

	protected int mScreenshotIdx;

	protected int mScreenshotTimer;

	public bool mFromExit;

	~Upsell()
	{
	}

	public override void Dispose()
	{
		base.RemoveAllWidgets(doDelete: true, recursive: false);
		mBuyBtn.Dispose();
		mMenuBtn.Dispose();
	}

	public Upsell(bool from_exit)
	{
		mClip = false;
		mPriority = (mZOrder = int.MaxValue);
		gZoomStart = Common._M(4f);
		mBlock2X = GameApp.gApp.mWidth + Common._DS(160);
		mState = 1;
		mZoom = gZoomStart;
		mMenuBtn = new ButtonWidget(1, this);
		mMenuBtn.mDoFinger = true;
		mMenuBtn.mNormalRect = mMenuBtn.mButtonImage.GetCelRect(0);
		mMenuBtn.mOverRect = mMenuBtn.mButtonImage.GetCelRect(1);
		mMenuBtn.mDownRect = mMenuBtn.mButtonImage.GetCelRect(2);
		AddWidget(mMenuBtn);
		mBuyBtn = new ButtonWidget(2, this);
		mBuyBtn.mDoFinger = true;
		mBuyBtn.mNormalRect = mBuyBtn.mButtonImage.GetCelRect(0);
		mBuyBtn.mOverRect = mBuyBtn.mButtonImage.GetCelRect(1);
		mBuyBtn.mDownRect = mBuyBtn.mButtonImage.GetCelRect(2);
		AddWidget(mBuyBtn);
		mScreenshotTimer = gScreenshotTimer;
		mScreenshotIdx = 0;
	}

	public override void Update()
	{
		float num = Common._M(50f);
		if (mState == 1)
		{
			mUpdateCnt++;
			if (mUpdateCnt >= Common._M(25))
			{
				mBlock1X += num;
				int num2 = 0;
				if (mBlock1X >= (float)num2)
				{
					mBlock1X = num2;
					mState++;
					mUpdateCnt = 0;
				}
			}
		}
		else if (mState == 2)
		{
			mUpdateCnt++;
			int num3 = 0;
			if (mUpdateCnt >= Common._M(25))
			{
				mBlock2X -= num;
				if (mBlock2X <= (float)num3)
				{
					mBlock2X = num3;
					mState++;
					mUpdateCnt = 0;
				}
			}
		}
		else if (mState == 3)
		{
			mUpdateCnt++;
			if (mUpdateCnt >= Common._M(25))
			{
				int num4 = Common._M(20);
				float num5 = (gZoomStart - 1f) / (float)num4;
				mZoom -= num5;
				if (mZoom <= 1f)
				{
					mZoom = 1f;
					mState++;
					mUpdateCnt = 0;
				}
			}
		}
		else if (mState == 4)
		{
			mUpdateCnt++;
			if (mUpdateCnt >= Common._M(25))
			{
				int num6 = Common._M(15);
				mMenuBtn.Move(mMenuBtn.mX, mMenuBtn.mY - num6);
				mBuyBtn.Move(mBuyBtn.mX, mBuyBtn.mY - num6);
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				if (mMenuBtn.mY <= num7)
				{
					mMenuBtn.mY = num7;
					num9++;
				}
				if (mBuyBtn.mY <= num8)
				{
					mBuyBtn.mY = num8;
					num9++;
				}
				if (num9 == 2)
				{
					mState++;
				}
			}
		}
		else if (mState == 5)
		{
			mScreenshotTimer--;
			if (mScreenshotTimer == 0)
			{
				mScreenshotTimer = gScreenshotTimer;
				mScreenshotIdx = (mScreenshotIdx + 1) % MAX_SCREENSHOTS;
			}
		}
		if (mState < 5 || mScreenshotTimer <= gScreenshotFade)
		{
			MarkDirty();
		}
	}

	public override void Draw(Graphics g)
	{
	}

	public void ButtonDepress(int id)
	{
		if (id == mMenuBtn.mId && mFromExit)
		{
			GameApp.gApp.mDoingDRM = false;
			GameApp.gApp.Shutdown();
		}
		else if (id == mMenuBtn.mId)
		{
			_ = GameApp.gApp.mBoard;
			GameApp.gApp.mWidgetManager.RemoveWidget(this);
			GameApp.gApp.mDoingDRM = false;
		}
	}

	public virtual void ButtonPress(int id)
	{
		if (id == mMenuBtn.mId && mFromExit)
		{
			GameApp.gApp.mDoingDRM = false;
			GameApp.gApp.Shutdown();
		}
		else if (id == mMenuBtn.mId)
		{
			_ = GameApp.gApp.mBoard;
			GameApp.gApp.mWidgetManager.RemoveWidget(this);
			GameApp.gApp.mDoingDRM = false;
		}
	}

	public virtual void ButtonPress(int theId, int theClickCount)
	{
	}

	public virtual void ButtonDownTick(int theId)
	{
	}

	public virtual void ButtonMouseEnter(int theId)
	{
	}

	public virtual void ButtonMouseLeave(int theId)
	{
	}

	public virtual void ButtonMouseMove(int theId, int theX, int theY)
	{
	}
}
