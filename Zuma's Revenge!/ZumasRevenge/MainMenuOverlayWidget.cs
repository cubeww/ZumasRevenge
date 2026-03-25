using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class MainMenuOverlayWidget : Widget, ButtonListener
{
	private MainMenu mMenu;

	private int mMenuMoreGamesStartY;

	private int mMenuMoreGamesOriginY;

	private int mMenuMoreGamesDestY;

	private int mMenuMoreGamesSignStartY;

	private int mMenuMoreGamesSignOriginY;

	private int mMenuMoreGamesSignDestY;

	private int mMenuMoreGamesSignY;

	private int mMenuOptionsStartX;

	private int mMenuOptionsDestX;

	private int mMenuOptionsOriginX;

	private Image IMAGE_UI_MAINMENU_MORE_GAMES_SIGN;

	private Image IMAGE_UI_MAINMENU_OPTIONS_DOWN;

	private Image IMAGE_UI_MAINMENU_MORE_GAMES;

	private Image IMAGE_UI_MAINMENU_MORE_GAMES_DOWN;

	private Image IMAGE_UI_MAINMENU_OPTIONS;

	private Image IMAGE_UI_MAINMENU_SCROLLMENU_SHADOW;

	private Image IMAGE_UI_MAINMENU_SCROLLMENU_BORDER;

	private Image IMAGE_UI_MAINMENU_RIBBIT;

	private Image IMAGE_UI_MAINMENU_BOTRIGHT_FOLIAGE;

	private Image IMAGE_UI_MAINMENU_BOTLEFT_FOLIAGE;

	private Image IMAGE_UI_MAINMENU_TIKIHEAD;

	private Image IMAGE_UI_MAINMENU_UNLOCK;

	private Image IMAGE_UI_MAINMENU_UNLOCK_ON;

	public void ButtonPress(int theId)
	{
		if (theId == 11 || theId == 6 || theId == 14)
		{
			GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BUTTON1));
		}
	}

	public void ButtonPress(int theId, int theClickCount)
	{
		ButtonPress(theId);
	}

	public void ButtonDepress(int theId)
	{
		GameApp mApp = mMenu.mApp;
		if (mMenu.mFirstTimeAlpha > 0 || mMenu.mIFUnlockAnim != null || mApp.mGenericHelp != null || mMenu.mDelayedIFStartState > 0 || mMenu.ShowingTikiTemple() || mApp.mMapScreen != null || (mApp.mBambooTransition != null && mApp.mBambooTransition.IsInProgress()) || mApp.GetDialog(2) != null)
		{
			return;
		}
		switch (theId)
		{
		case 11:
			GameApp.gApp.ShowLegal();
			break;
		case 6:
			mApp.DoOptionsDialog(ingame: false);
			break;
		case 14:
			if (GameApp.USE_TRIAL_VERSION)
			{
				ProcessLocked();
			}
			break;
		}
	}

	public void ProcessLocked()
	{
		mMenu.mState = MainMenu_State.State_QuitPrompt;
		string message = TextManager.getInstance().getString(834);
		int width_pad = Common._DS(Common._M(20));
		GameApp.gApp.DoYesNoDialog(TextManager.getInstance().getString(835), message, block: true, TextManager.getInstance().getString(446), TextManager.getInstance().getString(447), drag: false, Common._S(Common._M(50)), 1, width_pad);
		GameApp.gApp.mYesNoDialogDelegate = ProcessUnlock;
	}

	public void ProcessUnlock(int theId)
	{
		if (theId == 1000 && GameApp.USE_TRIAL_VERSION)
		{
			GameApp.gApp.ToMarketPlace();
		}
		mMenu.mState = MainMenu_State.State_MainMenu;
	}

	public void ButtonDownTick(int theId)
	{
	}

	public void ButtonMouseEnter(int theId)
	{
	}

	public void ButtonMouseLeave(int theId)
	{
	}

	public void ButtonMouseMove(int theId, int theX, int theY)
	{
	}

	public MainMenuOverlayWidget(MainMenu theMainMenu)
	{
		mMenu = theMainMenu;
		mMenuMoreGamesStartY = -1;
		mMenuMoreGamesOriginY = -1;
		mMenuMoreGamesDestY = -1;
		mMenuMoreGamesSignStartY = -1;
		mMenuMoreGamesSignOriginY = -1;
		mMenuMoreGamesSignDestY = -1;
		mMenuMoreGamesSignY = -1;
		mMenuOptionsStartX = -1;
		mMenuOptionsDestX = -1;
		mMenuOptionsOriginX = -1;
		mHasTransparencies = true;
		mWidgetFlagsMod.mRemoveFlags |= 49;
		GameApp mApp = mMenu.mApp;
		Resize(mApp.GetScreenRect().mX, mApp.GetScreenRect().mY, mApp.GetScreenRect().mWidth - mApp.GetScreenRect().mX, mApp.GetScreenRect().mHeight - mApp.GetScreenRect().mY);
	}

	public void Init()
	{
		_ = mMenu.mApp;
		IMAGE_UI_MAINMENU_MORE_GAMES_SIGN = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_MORE_GAMES_SIGN);
		IMAGE_UI_MAINMENU_OPTIONS_DOWN = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_OPTIONS_DOWN);
		IMAGE_UI_MAINMENU_MORE_GAMES = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_MORE_GAMES);
		IMAGE_UI_MAINMENU_MORE_GAMES_DOWN = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_MORE_GAMES_DOWN);
		IMAGE_UI_MAINMENU_OPTIONS = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_OPTIONS);
		IMAGE_UI_MAINMENU_SCROLLMENU_SHADOW = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_SHADOW);
		IMAGE_UI_MAINMENU_SCROLLMENU_BORDER = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_BORDER);
		IMAGE_UI_MAINMENU_RIBBIT = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_RIBBIT);
		IMAGE_UI_MAINMENU_BOTRIGHT_FOLIAGE = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_BOTRIGHT_FOLIAGE);
		IMAGE_UI_MAINMENU_BOTLEFT_FOLIAGE = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_BOTLEFT_FOLIAGE);
		IMAGE_UI_MAINMENU_TIKIHEAD = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_TIKIHEAD);
		IMAGE_UI_MAINMENU_UNLOCK = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_UNLOCK);
		IMAGE_UI_MAINMENU_UNLOCK_ON = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_UNLOCK_ON);
		AddOptionsButton();
		AddMoreGamesButton();
		mMenuMoreGamesSignY = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_MORE_GAMES_SIGN));
		mMenuMoreGamesSignStartY = mMenuMoreGamesSignY;
		mMenuMoreGamesSignOriginY = mMenuMoreGamesSignY;
	}

	public void DoMoreGamesSlide(bool isSlidingIn)
	{
		mMenuMoreGamesSignStartY = mMenuMoreGamesSignY;
		mMenuMoreGamesStartY = mMenu.mMoreGamesButton.mY;
		mMenuOptionsStartX = mMenu.mOptionsButton.mX;
		if (isSlidingIn)
		{
			mMenuMoreGamesDestY = mMenuMoreGamesOriginY;
			mMenuOptionsDestX = mMenuOptionsOriginX;
			mMenuMoreGamesSignDestY = mMenuMoreGamesSignOriginY;
		}
		else
		{
			mMenuMoreGamesDestY = (mMenuMoreGamesSignDestY = mMenu.mApp.mScreenBounds.mHeight + Common._S(150));
			mMenuOptionsDestX = -Common._S(300);
		}
	}

	public void AddMoreGamesButton()
	{
		GameApp mApp = mMenu.mApp;
		int width = IMAGE_UI_MAINMENU_MORE_GAMES_SIGN.GetWidth();
		int height = IMAGE_UI_MAINMENU_MORE_GAMES_SIGN.GetHeight();
		float num = (float)width * 0.64f;
		float num2 = (float)height * 0.68f;
		float num3 = (float)Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_MORE_GAMES_SIGN)) + (float)width * 0.18f;
		float num4 = (float)Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_MORE_GAMES_SIGN)) + (float)height * 0.16f;
		mMenuMoreGamesStartY = (int)num4;
		mMenuMoreGamesOriginY = (int)num4;
		mMenu.mMoreGamesButton = new ButtonWidget(11, this);
		mMenu.mMoreGamesButton.mButtonImage = IMAGE_UI_MAINMENU_MORE_GAMES;
		mMenu.mMoreGamesButton.mOverImage = IMAGE_UI_MAINMENU_MORE_GAMES;
		mMenu.mMoreGamesButton.mDownImage = IMAGE_UI_MAINMENU_MORE_GAMES_DOWN;
		mMenu.mMoreGamesButton.mBtnNoDraw = true;
		mMenu.mMoreGamesButton.mDoFinger = true;
		mMenu.mMoreGamesButton.Resize(mApp.GetWideScreenAdjusted((int)num3), (int)num4, (int)num, (int)num2);
		mMenu.AddWidget(mMenu.mMoreGamesButton);
	}

	public void AddOptionsButton()
	{
		_ = mMenu.mApp;
		mMenu.mOptionsButton = new ButtonWidget(6, this);
		mMenu.mOptionsButton.mButtonImage = IMAGE_UI_MAINMENU_OPTIONS;
		mMenu.mOptionsButton.mOverImage = IMAGE_UI_MAINMENU_OPTIONS;
		mMenu.mOptionsButton.mDownImage = IMAGE_UI_MAINMENU_OPTIONS_DOWN;
		mMenu.AddWidget(mMenu.mOptionsButton);
		mMenu.mOptionsButton.mBtnNoDraw = true;
		mMenu.mOptionsButton.mDoFinger = true;
		mMenuOptionsOriginX = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_OPTIONS_DOWN));
		mMenu.mOptionsButton.Resize(mMenuOptionsOriginX, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_OPTIONS_DOWN)), IMAGE_UI_MAINMENU_OPTIONS_DOWN.GetWidth(), IMAGE_UI_MAINMENU_OPTIONS_DOWN.GetHeight());
		mMenu.mUnlockButton = new ButtonWidget(14, this);
		mMenu.mUnlockButton.mButtonImage = IMAGE_UI_MAINMENU_OPTIONS;
		mMenu.mUnlockButton.mOverImage = IMAGE_UI_MAINMENU_OPTIONS;
		mMenu.mUnlockButton.mDownImage = IMAGE_UI_MAINMENU_OPTIONS_DOWN;
		mMenu.AddWidget(mMenu.mUnlockButton);
		mMenu.mUnlockButton.mBtnNoDraw = true;
		mMenu.mUnlockButton.mDoFinger = true;
		mMenu.mUnlockButton.Resize(325, 230, IMAGE_UI_MAINMENU_UNLOCK.GetWidth(), IMAGE_UI_MAINMENU_UNLOCK.GetHeight());
		if (GameApp.USE_TRIAL_VERSION)
		{
			mMenu.mUnlockButton.SetVisible(isVisible: true);
			return;
		}
		mMenu.mUnlockButton.SetVisible(isVisible: false);
		mMenu.mUnlockButton.SetDisabled(isDisabled: true);
	}

	public void UpdateOverlaySlide(float theSlidePct)
	{
		float num = theSlidePct * (float)(mMenuMoreGamesDestY - mMenuMoreGamesStartY);
		mMenu.mMoreGamesButton.Move(mMenu.mMoreGamesButton.mX, (int)((float)mMenuMoreGamesStartY + num));
		float num2 = theSlidePct * (float)(mMenuMoreGamesSignDestY - mMenuMoreGamesSignStartY);
		mMenuMoreGamesSignY = (int)((float)mMenuMoreGamesSignStartY + num2);
		float num3 = theSlidePct * (float)(mMenuOptionsDestX - mMenuOptionsStartX);
		mMenu.mOptionsButton.Move((int)((float)mMenuOptionsStartX + num3), mMenu.mOptionsButton.mY);
	}

	public void DrawOptionsButton(Graphics g)
	{
		Image image = IMAGE_UI_MAINMENU_OPTIONS;
		int width = image.GetWidth();
		int height = image.GetHeight();
		if (mMenu.mOptionsButton.mIsDown)
		{
			image = IMAGE_UI_MAINMENU_OPTIONS_DOWN;
			width = image.GetWidth();
			height = image.GetHeight();
		}
		int theX = (int)((float)mMenu.mOptionsButton.mX + (float)(mMenu.mOptionsButton.mWidth - width) / 2f);
		int theY = (int)((float)mMenu.mOptionsButton.mY + (float)(mMenu.mOptionsButton.mHeight - height) / 2f);
		if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_GR || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_RU || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_SP || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_SPC || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_IT)
		{
			g.DrawImage(image, theX, theY, (int)((float)image.mWidth * 0.9f), (int)((float)image.mHeight * 0.9f));
		}
		else
		{
			g.DrawImage(image, theX, theY);
		}
		if (GameApp.USE_TRIAL_VERSION)
		{
			image = IMAGE_UI_MAINMENU_UNLOCK;
			width = image.GetWidth();
			height = image.GetHeight();
			if (mMenu.mUnlockButton.mIsDown)
			{
				image = IMAGE_UI_MAINMENU_UNLOCK_ON;
				width = image.GetWidth();
				height = image.GetHeight();
			}
			theX = mMenu.mUnlockButton.mX + 35;
			theY = mMenu.mUnlockButton.mY;
			g.DrawImage(image, theX, theY);
		}
	}

	public void DrawMoreGamesButton(Graphics g)
	{
		Image iMAGE_UI_MAINMENU_MORE_GAMES = IMAGE_UI_MAINMENU_MORE_GAMES;
		int width = iMAGE_UI_MAINMENU_MORE_GAMES.GetWidth();
		int height = iMAGE_UI_MAINMENU_MORE_GAMES.GetHeight();
		if (mMenu.mMoreGamesButton.mIsDown)
		{
			iMAGE_UI_MAINMENU_MORE_GAMES = IMAGE_UI_MAINMENU_MORE_GAMES_DOWN;
			width = iMAGE_UI_MAINMENU_MORE_GAMES.GetWidth() + Common._DS(20);
			height = iMAGE_UI_MAINMENU_MORE_GAMES.GetHeight();
		}
		else
		{
			iMAGE_UI_MAINMENU_MORE_GAMES = IMAGE_UI_MAINMENU_MORE_GAMES;
			width = iMAGE_UI_MAINMENU_MORE_GAMES.GetWidth();
			height = iMAGE_UI_MAINMENU_MORE_GAMES.GetHeight();
		}
		float num = (float)mMenu.mMoreGamesButton.mX + (float)(mMenu.mMoreGamesButton.mWidth - width) * 0.5f;
		float num2 = (float)mMenu.mMoreGamesButton.mY + (float)(mMenu.mMoreGamesButton.mHeight - height) * 0.5f;
		g.DrawImage(iMAGE_UI_MAINMENU_MORE_GAMES, (int)num, (int)num2);
	}

	public override void Draw(Graphics g)
	{
		GameApp mApp = mMenu.mApp;
		if ((mApp.mCredits == null || !MathUtils._geq(mApp.mCredits.mAlpha, 255f)) && mMenu.mChallengeMenu == null && mApp.mMapScreen == null && mMenu.mTikiTemple == null)
		{
			g.DrawImage(IMAGE_UI_MAINMENU_SCROLLMENU_SHADOW, 0, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_BORDER)) + (mMenu.mMainMenuButtonsScrollWidget.mY - mMenu.mMenuScrollOriginY));
			g.DrawImageMirror(IMAGE_UI_MAINMENU_SCROLLMENU_SHADOW, mApp.GetScreenRect().mWidth - mApp.GetScreenRect().mX - IMAGE_UI_MAINMENU_SCROLLMENU_SHADOW.GetWidth(), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_BORDER)) + (mMenu.mMainMenuButtonsScrollWidget.mY - mMenu.mMenuScrollOriginY));
			g.DrawImage(IMAGE_UI_MAINMENU_RIBBIT, mMenu.mMenuFrogX, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_RIBBIT)));
			g.DrawImage(IMAGE_UI_MAINMENU_BOTRIGHT_FOLIAGE, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_BOTRIGHT_FOLIAGE)) - mApp.mWideScreenXOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_BOTRIGHT_FOLIAGE)));
			g.DrawImage(IMAGE_UI_MAINMENU_BOTLEFT_FOLIAGE, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_BOTLEFT_FOLIAGE)) - mApp.mWideScreenXOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_BOTLEFT_FOLIAGE)));
			g.DrawImage(IMAGE_UI_MAINMENU_TIKIHEAD, mMenu.mMenuTikiX, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_TIKIHEAD)));
			g.DrawImage(IMAGE_UI_MAINMENU_MORE_GAMES_SIGN, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_MORE_GAMES_SIGN)) - mApp.mWideScreenXOffset + mApp.GetScreenRect().mX, mMenuMoreGamesSignY);
			DrawOptionsButton(g);
			DrawMoreGamesButton(g);
			if (mMenu.mFirstTimeAlpha > 0)
			{
				g.SetColor(new Color(0, 0, 0, mMenu.mFirstTimeAlpha));
				g.FillRect(Common._S(-80), 0, mWidth + Common._S(160), mHeight);
			}
		}
	}
}
