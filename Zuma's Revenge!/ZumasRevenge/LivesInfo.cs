using System;
using JeffLib;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class LivesInfo : IDisposable
{
	private enum SLIDE_STATE
	{
		SLIDE_ON,
		SLIDE_ONSCREEN,
		SLIDE_OFF,
		SLIDE_OFFSCREEN,
		SLIDE_WAIT,
		NUM_SLIDE_STATES
	}

	private Font mFont;

	private Rect mFrame = default(Rect);

	private Rect mInset = default(Rect);

	private int mTextXOffset;

	private int mXOffset;

	private FwooshImage mLivesText = new FwooshImage();

	private int mLivesCount;

	private int mLivesDelta;

	private float mXStart;

	private float mXEnd;

	private ulong mDisplayTime;

	private ulong mDisplayStart;

	private ulong mWaitTime;

	private SLIDE_STATE mSlideState;

	private CurvedVal mSlideVal = new CurvedVal();

	public LivesInfo(Board board, int theLivesDelta)
	{
		mFont = Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_BASE);
		mLivesDelta = theLivesDelta;
		mDisplayTime = 1200uL;
		mDisplayStart = ulong.MaxValue;
		mWaitTime = 150uL;
		mLivesCount = board.GetNumLives() - 1;
		mSlideVal.SetConstant(0.0);
		mSlideVal.mAppUpdateCountSrc = board.mUpdateCnt;
		InitLayout();
		StartSliding(SLIDE_STATE.SLIDE_ON, 0);
		SoundAttribs inAttribs = new SoundAttribs
		{
			delay = 50
		};
		GameApp.gApp.mSoundPlayer.Play(Res.GetSoundByID(ResID.SOUND_NEW_EXTRA_LIFE), inAttribs);
	}

	public virtual void Dispose()
	{
		if (mLivesText.mImage != null)
		{
			mLivesText.mImage.Dispose();
		}
	}

	public void Draw(Graphics g)
	{
		mFrame.mX = (mInset.mX = (int)((float)mFrame.mX - g.mTransX));
		DrawPlank(g);
		DrawLivesCount(g);
	}

	public void Update()
	{
		switch (mSlideState)
		{
		case SLIDE_STATE.SLIDE_OFF:
			if (!IsSliding())
			{
				mSlideState = SLIDE_STATE.SLIDE_OFFSCREEN;
			}
			break;
		case SLIDE_STATE.SLIDE_ON:
			DisplayOldCount();
			break;
		case SLIDE_STATE.SLIDE_WAIT:
			DisplayCount();
			break;
		case SLIDE_STATE.SLIDE_ONSCREEN:
			SlideOff();
			break;
		}
		if (mLivesText.mImage != null)
		{
			mLivesText.Update();
		}
	}

	public bool IsDone()
	{
		return mSlideState == SLIDE_STATE.SLIDE_OFFSCREEN;
	}

	private void InitLayout()
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_LIVESFRAME);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_FROG_LIVES);
		Image imageByID3 = Res.GetImageByID(ResID.IMAGE_UI_POLE);
		mXOffset = imageByID3.GetWidth();
		int height = imageByID.GetHeight();
		int theX = GameApp.gApp.GetScreenRect().mX - GameApp.gApp.mWideScreenXOffset;
		int theY = GameApp.gApp.GetScreenRect().mHeight - height;
		int num = (int)((float)height * 0.13f);
		mTextXOffset = imageByID2.GetWidth() + Common._S(50);
		mFrame = new Rect(theX, theY, 0, height);
		mFrame.mWidth = mTextXOffset + mFont.StringWidth("x 00");
		mFrame.mX -= mFrame.mWidth;
		mInset = mFrame;
		mInset.mWidth -= num;
		mInset.mHeight -= num;
		mInset.mY += num;
	}

	private void StartSliding(SLIDE_STATE inSlideState, int inXPos)
	{
		mSlideState = inSlideState;
		mXStart = mFrame.mX + mXOffset;
		mXEnd = GameApp.gApp.GetScreenRect().mX + inXPos + mXOffset;
		mSlideVal.SetCurve(Common._MP("b70,1,0.04,1,#     $P    }~"));
	}

	private void DrawPlank(Graphics g)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_FROG_LIVES);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_LIVESFRAME);
		int mX = mInset.mX;
		int theY = (int)((float)mInset.mY + (float)(mInset.mHeight - imageByID.GetHeight()) * 0.5f);
		g.DrawImageBox(mFrame, imageByID2);
		g.DrawImage(imageByID, mX, theY);
	}

	private void DrawLivesCount(Graphics g)
	{
		if (mSlideState != SLIDE_STATE.SLIDE_ONSCREEN)
		{
			bool flag = mSlideState == SLIDE_STATE.SLIDE_ON || mSlideState == SLIDE_STATE.SLIDE_WAIT;
			int num = CapAt99(flag ? (mLivesCount - mLivesDelta) : mLivesCount);
			string theString = "x  " + num;
			g.SetFont(mFont);
			g.SetColor(Color.White);
			g.WriteString(theString, mInset.mX + mTextXOffset, mInset.mY + mFont.GetHeight());
		}
		else if (mLivesText.mImage != null)
		{
			mLivesText.Draw(g);
		}
	}

	private void DisplayOldCount()
	{
		if (!IsSliding())
		{
			mSlideState = SLIDE_STATE.SLIDE_WAIT;
			mDisplayStart = SexyFramework.Common.SexyTime();
		}
	}

	private void DisplayCount()
	{
		if (SexyFramework.Common.SexyTime() - mDisplayStart >= mWaitTime)
		{
			mSlideState = SLIDE_STATE.SLIDE_ONSCREEN;
			mDisplayStart = SexyFramework.Common.SexyTime();
			InitLivesText();
			PreDrawLivesText(CapAt99(mLivesCount));
		}
	}

	private void SlideOff()
	{
		if (SexyFramework.Common.SexyTime() - mDisplayStart >= mDisplayTime)
		{
			mLivesText.mImage = null;
			StartSliding(SLIDE_STATE.SLIDE_OFF, -mFrame.mWidth);
		}
	}

	private bool IsSliding()
	{
		if (mSlideVal.IsDoingCurve())
		{
			float num = (float)mSlideVal.GetOutVal() * (mXEnd - mXStart);
			mFrame.mX = (mInset.mX = (int)(mXStart + num));
			return true;
		}
		return false;
	}

	private int CapAt99(int inLivesCount)
	{
		if (inLivesCount < 0)
		{
			return 0;
		}
		if (inLivesCount > 99)
		{
			return 99;
		}
		return inLivesCount;
	}

	private void InitLivesText()
	{
		int theWidth = mFont.StringWidth("x 00") + Common._S(20);
		int theHeight = mFont.GetHeight() + Common._S(10);
		mLivesText = new FwooshImage();
		mLivesText.mAlphaDec = 0f;
		mLivesText.mImage = new DeviceImage();
		mLivesText.mImage.mApp = GameApp.gApp;
		mLivesText.mImage.SetImageMode(hasTrans: true, hasAlpha: true);
		mLivesText.mImage.AddImageFlags(16u);
		mLivesText.mImage.Create(theWidth, theHeight);
		mLivesText.mX = mInset.mX + mTextXOffset;
		mLivesText.mY = mInset.mY + (int)((float)mInset.mHeight * 0.5f) + Common._S(5);
	}

	private void PreDrawLivesText(int inLivesCount)
	{
		Graphics graphics = new Graphics(mLivesText.mImage);
		graphics.Get3D().ClearColorBuffer(new Color(0, 0, 0, 0));
		graphics.SetFont(mFont);
		graphics.SetColor(Color.White);
		graphics.WriteString("x " + inLivesCount + " ", 0, mFont.GetAscent(), mLivesText.mImage.GetWidth());
		graphics.ClearRenderContext();
	}
}
