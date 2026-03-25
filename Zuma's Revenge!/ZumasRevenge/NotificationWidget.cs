using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class NotificationWidget : Widget
{
	private enum SLIDE_STATE
	{
		SLIDE_ON,
		SLIDE_ONSCREEN,
		SLIDE_OFF,
		SLIDE_OFFSCREEN,
		NUM_SLIDE_STATES
	}

	private Board mBoard;

	private float mYStart;

	private float mYEnd;

	private ulong mDisplayTime;

	private ulong mDisplayStart;

	private SLIDE_STATE mSlideState;

	private bool mIsFinished;

	private CurvedVal mSlideVal = new CurvedVal();

	private string mNotification;

	private int mNotificationStringWidth;

	private Font mFont;

	public int mSoundID;

	public NotificationWidget(Board theBoard, string theStringInfo)
	{
		mBoard = theBoard;
		mDisplayTime = 2000uL;
		mDisplayStart = ulong.MaxValue;
		mSoundID = -1;
		mSlideState = SLIDE_STATE.SLIDE_ON;
		mIsFinished = false;
		mSlideVal.mAppUpdateCountSrc = mBoard.mUpdateCnt;
		mSlideVal.SetCurve(Common._MP("b70,1,0.02,1,#     $P    }~"));
		mFont = Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_YELLOW);
		mNotification = theStringInfo;
		mNotificationStringWidth = mFont.StringWidth(theStringInfo);
		int num = Common._DS(600);
		int num2 = Common._DS(100);
		int num3 = ((mNotificationStringWidth + num2 > num) ? (mNotificationStringWidth + num2) : num);
		int num4 = Common._DS(150);
		mYStart = GameApp.gApp.GetScreenRect().mHeight + num4;
		mYEnd = GameApp.gApp.GetScreenRect().mHeight - num4 / 2;
		Resize(GameApp.gApp.GetScreenRect().mX + (GameApp.gApp.GetScreenRect().mWidth - num3) / 2, (int)mYStart, num3, num4);
	}

	public override void Draw(Graphics g)
	{
		Common.DrawCommonDialogBacking(g, 0, 0, mWidth, mHeight * 2);
		g.SetFont(mFont);
		g.SetColor(Color.White);
		if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CH || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CHT)
		{
			g.DrawString(mNotification, (mWidth - mNotificationStringWidth) / 2, Common._DS(60) + 3);
		}
		else
		{
			g.DrawString(mNotification, (mWidth - mNotificationStringWidth) / 2, Common._DS(60));
		}
	}

	public override void Update()
	{
		switch (mSlideState)
		{
		case SLIDE_STATE.SLIDE_OFF:
			if (mSlideVal.IsDoingCurve())
			{
				float num2 = (float)mSlideVal.GetOutVal() * (mYEnd - mYStart);
				Move(mX, (int)(mYStart + num2));
			}
			else
			{
				mSlideState = SLIDE_STATE.SLIDE_OFFSCREEN;
			}
			break;
		case SLIDE_STATE.SLIDE_ON:
			if (mSlideVal.IsDoingCurve())
			{
				float num3 = (float)mSlideVal.GetOutVal() * (mYEnd - mYStart);
				Move(mX, (int)(mYStart + num3));
			}
			else
			{
				PlaySound();
				mSlideState = SLIDE_STATE.SLIDE_ONSCREEN;
				mDisplayStart = SexyFramework.Common.SexyTime();
			}
			break;
		case SLIDE_STATE.SLIDE_ONSCREEN:
		{
			ulong num = SexyFramework.Common.SexyTime();
			if (num - mDisplayStart >= mDisplayTime)
			{
				mSlideVal.SetCurve(Common._MP("b70,1,0.02,1,#     $P    }~"));
				mSlideState = SLIDE_STATE.SLIDE_OFF;
				mYStart = mY;
				mYEnd = GameApp.gApp.GetScreenRect().mHeight + mHeight;
			}
			break;
		}
		case SLIDE_STATE.SLIDE_OFFSCREEN:
			mIsFinished = true;
			break;
		}
	}

	public override void MouseDown(int x, int y, int theClickCount)
	{
		if (mBoard != null)
		{
			mBoard.MouseDown(x, y, theClickCount);
		}
	}

	public override void MouseUp(int x, int y, int theClickCount)
	{
		if (mBoard != null)
		{
			mBoard.MouseDown(x, y, theClickCount);
		}
	}

	public override void MouseMove(int x, int y)
	{
		if (mBoard != null)
		{
			mBoard.MouseMove(x, y);
		}
	}

	public override void MouseDrag(int x, int y)
	{
		if (mBoard != null)
		{
			mBoard.MouseMove(x, y);
		}
	}

	public bool IsFinished()
	{
		return mIsFinished;
	}

	private void PlaySound()
	{
		if (mSoundID != -1)
		{
			mBoard.mApp.mSoundPlayer.Play(mSoundID);
			mSoundID = -1;
		}
	}
}
