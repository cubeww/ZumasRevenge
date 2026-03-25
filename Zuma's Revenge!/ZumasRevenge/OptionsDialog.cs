using System.Text;
using JeffLib;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

internal class OptionsDialog : ZumaDialog, SliderListener
{
	public enum ControlId
	{
		OptionsDialog_MusicVolume,
		OptionsDialog_SfxVolume,
		OptionsDialog_Help,
		OptionsDialog_ToMainMenu,
		OptionsDialog_Colorblind,
		OptionsDialog_Credits,
		OptionsDialog_Language,
		OptionsDialog_Legal,
		OptionsDialog_BackToGame
	}

	protected enum OptionState
	{
		OptionState_BackToMainMenuPrompt,
		OptionState_OptionToMainMenuPrompt,
		OptionState_Credits,
		OptionState_Help,
		OptionState_Legal,
		OptionState_None
	}

	private const double MUSIC_SLIDER_THRESHOLD = 0.01;

	private static int OPTIONS_BUTTON_WIDTH = Common._DS(372);

	private static int OPTIONS_BUTTON_HEIGHT = Common._DS(157);

	private static int INCLUDE_LANGUAGE_BUTTON = 0;

	public ZumaSlider mMusicVolumeSlider;

	public ZumaSlider mSfxVolumeSlider;

	public ZumaSlideBox mColorBlindSlider;

	public double mOriginMusicVolume;

	public double mOriginSfxVolume;

	public bool mOriginColorBlind;

	public ButtonWidget mHelpButton;

	public ButtonWidget mMainMenuButton;

	public ButtonWidget mBackToGame;

	public ButtonWidget mCreditsButton;

	public ButtonWidget mLanguageButton;

	public bool mInGame;

	public bool mMusicEnabled;

	public bool mMusicSliderOn;

	public int mHeightPad;

	protected OptionState mState;

	public OptionsDialog(bool inGame)
		: base(2, isModal: true, "", "", "", 0)
	{
		mLanguageButton = null;
		mInGame = inGame;
		mMusicEnabled = false;
		mMusicSliderOn = false;
		mHeightPad = Common._S(Common._M(272));
		mState = OptionState.OptionState_None;
		mAllowDrag = false;
		mClip = false;
		LoadResources();
		InitMusicSlider();
		InitSfxSlider();
		InitColorblindSlider();
		InitButtons();
		InitSize();
	}

	~OptionsDialog()
	{
	}

	public override void Resize(int theX, int theY, int theWidth, int theHeight)
	{
		base.Resize(theX, theY, theWidth, theHeight);
		if (mInGame)
		{
			LayoutAdventureDialog();
		}
		else
		{
			LayoutMainMenuDialog();
		}
	}

	public override void Update()
	{
		base.Update();
		if (mMusicVolumeSlider.mDisabled && !GameApp.gApp.mMusicInterface.m_isUserMusicOn)
		{
			mMusicVolumeSlider.mDisabled = false;
			mMusicEnabled = true;
			double value = (mOriginMusicVolume = GameApp.gApp.GetMusicVolume());
			mMusicVolumeSlider.SetValue(value);
		}
		else if (!mMusicVolumeSlider.mDisabled && GameApp.gApp.mMusicInterface.m_isUserMusicOn)
		{
			mMusicVolumeSlider.mDisabled = true;
			mMusicEnabled = false;
			mMusicVolumeSlider.SetValue(0.0);
		}
		if (mState == OptionState.OptionState_OptionToMainMenuPrompt)
		{
			SetVisible(isVisible: false);
		}
		else
		{
			SetVisible(isVisible: true);
		}
	}

	public override void Draw(Graphics g)
	{
		if (GameApp.gApp.mCredits != null)
		{
			return;
		}
		Font fontByID = Res.GetFontByID(ResID.FONT_SHAGEXOTICA100_STROKE);
		Font fontByID2 = Res.GetFontByID(ResID.FONT_SHAGLOUNGE45_GAUNTLET);
		Font fontByID3 = Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_GAUNTLET);
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_LARGE_CROWN);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BOX_MAINMENU_CROWN_BOX);
		Image imageByID3 = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_LARGE_ACECROWN);
		Image imageByID4 = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_CROWN_HOLE);
		Image imageByID5 = Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BOX_ADVENTURE);
		g.PushState();
		g.Translate(-mX, -mY);
		g.SetColor(0, 0, 0, 130);
		g.FillRect(Common._S(-80), 0, GameApp.gApp.mWidth + Common._S(160), GameApp.gApp.mHeight);
		g.PopState();
		base.Draw(g);
		if (!mInGame)
		{
			return;
		}
		g.SetFont(fontByID);
		g.SetColor(255, 255, 255);
		Board board = GameApp.gApp.GetBoard();
		if (GameApp.gApp.GetBoard().GauntletMode())
		{
			g.SetFont(fontByID2);
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(Color.White);
			int num = Common._S(100);
			int theY = Common._S(120);
			if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_SP || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_SPC)
			{
				num -= 35;
			}
			string theString = TextManager.getInstance().getString(669);
			float num2 = fontByID2.StringWidth(theString);
			g.DrawString(theString, num, theY);
			string theString2 = SexyFramework.Common.CommaSeperate(board.mScore);
			fontByID2.StringWidth(theString2);
			float num3 = (float)num + num2 + (float)Common._DS(20);
			g.DrawString(theString2, (int)num3, theY);
			float num4 = Common._S(190);
			float num5 = Common._S(150);
			float num6 = Common._DS(15);
			float num7 = Common._DS(10);
			float num8 = 0.5f;
			float num9 = (float)imageByID.GetWidth() * num8;
			float num10 = (float)imageByID.GetHeight() * num8;
			float num11 = (float)imageByID.GetWidth() * num8;
			imageByID.GetHeight();
			g.DrawImage(imageByID2, Common._S(40), Common._S(129));
			g.DrawImage(imageByID, (int)num4, (int)num5, (int)num9, (int)num10);
			string theString3 = SexyFramework.Common.UCommaSeparate((uint)board.mLevel.mChallengePoints);
			g.DrawString(theString3, (int)(num4 + num9 + num7), (int)(num5 + (float)fontByID2.mAscent));
			g.DrawImage(imageByID3, (int)num4, (int)(num5 + num10 + num6), (int)num11, (int)num10);
			string theString4 = SexyFramework.Common.UCommaSeparate((uint)board.mLevel.mChallengeAcePoints);
			g.DrawString(theString4, (int)(num4 + num11 + num7), (int)(num5 + num10 + num6 + (float)fontByID2.mAscent));
			string arg = JeffLib.Common.UpdateToTimeStr(board.mLevel.mGauntletCurTime);
			string arg2 = JeffLib.Common.UpdateToTimeStr(((GameApp)GlobalMembers.gSexyApp).GetLevelMgr().mGauntletSessionLength);
			string text = $" {arg} / {arg2}";
			g.DrawString(TextManager.getInstance().getString(679) + text, Common._S(45), Common._S(310));
			if (GameApp.gApp.mUserProfile != null && GameApp.gApp.mBoard != null && GameApp.gApp.mBoard.mLevel != null)
			{
				float num12 = Common._S(60);
				float num13 = Common._S(132);
				Image image = null;
				string text2 = "";
				if (board.mScore >= board.mLevel.mChallengePoints)
				{
					image = ((board.mScore >= board.mLevel.mChallengeAcePoints) ? imageByID3 : imageByID);
				}
				else
				{
					image = imageByID4;
					text2 = TextManager.getInstance().getString(681);
				}
				if (image != null)
				{
					if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_RU)
					{
						g.DrawImage(image, (int)num12 - 24, (int)num13 - 28, (int)((double)imageByID4.GetWidth() * 1.35), (int)((double)imageByID4.GetHeight() * 1.35));
					}
					else if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_SP || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_SPC || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_PL)
					{
						g.DrawImage(image, (int)num12 - 13, (int)num13 - 15, (int)((double)imageByID4.GetWidth() * 1.2), (int)((double)imageByID4.GetHeight() * 1.2));
					}
					else
					{
						g.DrawImage(image, (int)num12, (int)num13, imageByID4.GetWidth(), imageByID4.GetHeight());
					}
					g.SetColor(136, 156, 43, 255);
					g.SetFont(fontByID3);
					g.GetFont().StringWidth(text2);
					float num14 = num12 + (float)Common._S(7);
					float num15 = num13 + (float)Common._S(38);
					g.PushState();
					g.SetScale(0.7f, 0.7f, num14, num15);
					g.WriteWordWrapped(new Rect((int)num14 + Common._S(15), (int)num15, imageByID4.GetWidth(), imageByID4.GetHeight()), text2, -1, 0);
					g.PopState();
				}
			}
			g.SetColorizeImages(colorizeImages: false);
			return;
		}
		g.SetFont(fontByID2);
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(Color.White);
		g.DrawString(TextManager.getInstance().getString(670), Common._S(120), Common._S(120));
		int num16 = Common._S(80);
		int num17 = Common._S(130);
		g.DrawImage(imageByID5, num16, num17);
		int num18 = board.GetNumLives() - 1;
		if (num18 < 0)
		{
			num18 = 0;
		}
		else if (num18 > 99)
		{
			num18 = 99;
		}
		string theString5 = $"x {num18}";
		fontByID2.StringWidth(theString5);
		float num19 = num16 + imageByID5.GetWidth() + Common._S(10);
		float num20 = num17 + imageByID5.GetHeight() / 2;
		g.DrawString(theString5, (int)num19, (int)num20);
		if (GameApp.gApp.mBoard.mGameState != GameState.GameState_Losing)
		{
			string theString6 = TextManager.getInstance().getString(679);
			float num21 = fontByID2.StringWidth(theString6);
			Level mLevel = board.mLevel;
			int num22 = 65;
			if (mLevel != null && mLevel.mBoss == null && mLevel.mIndex != num22)
			{
				StringBuilder stringBuilder = new StringBuilder(TextManager.getInstance().getString(671));
				stringBuilder.Replace("$1", JeffLib.Common.UpdateToTimeStr(mLevel.mParTime));
				string theString7 = stringBuilder.ToString();
				float num23 = fontByID2.StringWidth(theString7);
				g.DrawString(theString7, Common._S(120) + (int)(num21 - num23) / 2, (int)num20 + Common._S(160));
			}
			string theString8 = ((board.mGameState == GameState.GameState_Playing) ? JeffLib.Common.UpdateToTimeStr(board.mStateCount - board.mIgnoreCount) : JeffLib.Common.UpdateToTimeStr(board.mEndLevelStats.mTimePlayed));
			float num24 = fontByID2.StringWidth(theString8);
			g.DrawString(theString6, Common._S(120), (int)num20 + Common._S(80));
			g.DrawString(theString8, Common._S(120) + (int)(num21 - num24) / 2, (int)num20 + Common._S(120));
		}
	}

	public virtual void DrawAll(ref ModalFlags theFlags, Graphics g)
	{
		g.PushState();
		g.Translate(-mX, -mY);
		g.SetColor(0, 0, 0, 130);
		g.FillRect(Common._S(-80), 0, GameApp.gApp.mWidth + Common._S(160), GameApp.gApp.mHeight);
		g.PopState();
		DrawAll(theFlags, g);
	}

	public override void AddedToManager(WidgetManager theWidgetManager)
	{
		base.AddedToManager(theWidgetManager);
		AddWidget(mMusicVolumeSlider);
		AddWidget(mSfxVolumeSlider);
		AddWidget(mHelpButton);
		AddWidget(mMainMenuButton);
		AddWidget(mBackToGame);
		AddWidget(mCreditsButton);
		AddWidget(mColorBlindSlider);
	}

	public override void RemovedFromManager(WidgetManager theWidgetManager)
	{
		base.RemovedFromManager(theWidgetManager);
		RemoveWidget(mMusicVolumeSlider);
		RemoveWidget(mSfxVolumeSlider);
		RemoveWidget(mHelpButton);
		RemoveWidget(mMainMenuButton);
		RemoveWidget(mBackToGame);
		RemoveWidget(mCreditsButton);
		RemoveWidget(mColorBlindSlider);
	}

	public void SliderVal(int theId, double theVal)
	{
		switch ((ControlId)theId)
		{
		case ControlId.OptionsDialog_MusicVolume:
			if (GameApp.gApp.mMusicInterface.isPlayingUserMusic() && theVal > 0.0)
			{
				GameApp.gApp.mMusicInterface.stopUserMusic();
			}
			SetMusicSlider(theVal);
			break;
		case ControlId.OptionsDialog_SfxVolume:
			SetSfxSlider(theVal);
			break;
		}
	}

	public void ProcessYesNo(int theId)
	{
		GameApp gameApp = (GameApp)GlobalMembers.gSexyApp;
		if (theId == 1000)
		{
			gameApp.KillDialog(this);
			gameApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.DoDeferredEndGame;
			gameApp.ToggleBambooTransition();
			gameApp.mMusic.StopAll();
		}
		mState = OptionState.OptionState_None;
	}

	public override void ButtonDepress(int theId)
	{
		base.ButtonDepress(theId);
		GameApp gameApp = (GameApp)GlobalMembers.gSexyApp;
		switch (theId)
		{
		case 3:
		{
			mState = OptionState.OptionState_OptionToMainMenuPrompt;
			int width_pad = Common._DS(Common._M(20));
			string message = (((GameApp)GlobalMembers.gSexyApp).GetBoard().GauntletMode() ? TextManager.getInstance().getString(449) : ((!GameApp.gApp.GetBoard().IronFrogMode()) ? TextManager.getInstance().getString(451) : TextManager.getInstance().getString(450)));
			SetVisible(isVisible: false);
			gameApp.DoYesNoDialog(TextManager.getInstance().getString(448), message, block: true, TextManager.getInstance().getString(446), TextManager.getInstance().getString(447), drag: false, Common._S(Common._M(50)), 1, width_pad);
			gameApp.mYesNoDialogDelegate = ProcessYesNo;
			SetVisible(isVisible: true);
			break;
		}
		case 8:
			mState = OptionState.OptionState_None;
			GameApp.gApp.FinishOptionsDialog(doSave: true);
			break;
		case 2:
		{
			mState = OptionState.OptionState_Help;
			Board board = GameApp.gApp.GetBoard();
			GameApp.gApp.mColorblind = mColorBlindSlider.IsOn();
			if (board != null && board.GauntletMode())
			{
				board.ShowChallengeHelpScreen();
				break;
			}
			GameApp.gApp.mGenericHelp = new GenericHelp();
			GameApp.gApp.AddDialog(GameApp.gApp.mGenericHelp);
			break;
		}
		case 5:
			mState = OptionState.OptionState_Credits;
			GameApp.gApp.DoCredits(isFromMainMenu: true);
			break;
		case 7:
			mState = OptionState.OptionState_Legal;
			GameApp.gApp.ShowLegal();
			break;
		}
	}

	public void DetectMusicSettings()
	{
		mMusicEnabled = GameApp.gApp.MusicEnabled();
		double num = (mOriginMusicVolume = (mMusicEnabled ? GameApp.gApp.GetMusicVolume() : 0.0));
		mMusicVolumeSlider.SetValue(num);
		SetMusicSlider(num);
	}

	private void LoadResources()
	{
		if (!GameApp.gApp.mResourceManager.IsGroupLoaded("CommonGame") && !GameApp.gApp.mResourceManager.LoadResources("CommonGame"))
		{
			GameApp.gApp.Shutdown();
		}
	}

	private void InitMusicSlider()
	{
		mMusicVolumeSlider = new ZumaSlider(0, this, TextManager.getInstance().getString(672));
		mMusicVolumeSlider.mFeedbackSoundID = Res.GetSoundByID(ResID.SOUND_BALLCLICK1);
		DetectMusicSettings();
	}

	private void InitSfxSlider()
	{
		mSfxVolumeSlider = new ZumaSlider(1, this, TextManager.getInstance().getString(673));
		mSfxVolumeSlider.mFeedbackSoundID = Res.GetSoundByID(ResID.SOUND_BALLCLICK1);
		mOriginSfxVolume = GlobalMembers.gSexyApp.GetSfxVolume();
		mSfxVolumeSlider.SetValue(mOriginSfxVolume);
	}

	private void InitColorblindSlider()
	{
		mColorBlindSlider = new ZumaSlideBox(this, 4, TextManager.getInstance().getString(680));
		mOriginColorBlind = GameApp.gApp.mColorblind;
		mColorBlindSlider.SetOnOff(mOriginColorBlind);
	}

	private void InitButtons()
	{
		mMainMenuButton = InitButton(3, TextManager.getInstance().getString(676));
		mHelpButton = InitButton(2, TextManager.getInstance().getString(674));
		mBackToGame = InitButton(8, TextManager.getInstance().getString(675));
		mCreditsButton = InitButton(5, TextManager.getInstance().getString(677));
		HideButton(mMainMenuButton, !mInGame);
		HideButton(mCreditsButton, mInGame);
	}

	private void InitSize()
	{
		if (mInGame)
		{
			Resize(0, 0, Common._S(Common._M(690)), Common._S(Common._M1(230)) + mHeightPad);
		}
		else
		{
			Resize(0, 0, Common._S(Common._M(600)), Common._S(Common._M1(230)) + mHeightPad - 80);
		}
	}

	private ButtonWidget InitButton(int inButtonID, string inButtonName)
	{
		ButtonWidget buttonWidget = Common.MakeButton(inButtonID, this, inButtonName);
		buttonWidget.mDoFinger = true;
		return buttonWidget;
	}

	private void HideButton(ButtonWidget inButton, bool inHide)
	{
		inButton.SetVisible(!inHide);
		inButton.mDisabled = inHide;
	}

	private void LayoutMainMenuDialog()
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BOX_MAINMENU_SLIDEBOXBACK);
		int num = GetLeft() - mX;
		int num2 = GetTop() - mY;
		int width = GetWidth();
		int num3 = width / 2;
		int num4 = Common._DS(Common._M(8));
		int theY = num2 - Common._DS(Common._M(70));
		mMusicVolumeSlider.Resize(num + num4 / 2 + Common._DS(Common._M(10)), theY, num3 - Common._DS(Common._M1(24)), Common._DS(Common._M2(94)));
		mSfxVolumeSlider.Layout(17411, mMusicVolumeSlider, Common._DS(Common._M(25)), 0, 0, 0);
		mColorBlindSlider.Resize((mMusicVolumeSlider.mX + mSfxVolumeSlider.mX + mSfxVolumeSlider.mWidth) / 2 - imageByID.GetWidth() / 2, mMusicVolumeSlider.mY + Common._S(45), imageByID.GetWidth(), imageByID.GetHeight());
		int num5 = Common._DS(10);
		int theX = (mWidth - (OPTIONS_BUTTON_WIDTH * 3 + num5)) / 2;
		mCreditsButton.Resize(theX, mColorBlindSlider.mY + Common._S(90), OPTIONS_BUTTON_WIDTH, OPTIONS_BUTTON_HEIGHT);
		mHelpButton.Resize(mCreditsButton.mX + mCreditsButton.mWidth + num5, mCreditsButton.mY, OPTIONS_BUTTON_WIDTH, OPTIONS_BUTTON_HEIGHT);
		HideButton(mHelpButton, inHide: true);
		int num6 = 200;
		mBackToGame.Resize(mHelpButton.mX + num6, mHelpButton.mY, OPTIONS_BUTTON_WIDTH, OPTIONS_BUTTON_HEIGHT);
		mMainMenuButton.Layout(16387, mBackToGame, 0, 0, 0, 0);
	}

	private void LayoutAdventureDialog()
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BOX_MAINMENU_SLIDEBOXBACK);
		int num = GetLeft() - mX;
		int num2 = GetTop() - mY;
		int width = GetWidth();
		int num3 = width / 2;
		Common._S(Common._M(8));
		int num4 = num2 - Common._S(Common._M(40));
		mMusicVolumeSlider.Resize(num + Common._S(Common._M(340)), num4 - 7, num3 - Common._S(Common._M1(24)), Common._S(Common._M2(44)));
		mSfxVolumeSlider.Layout(4611, mMusicVolumeSlider, Common._S(Common._M(0)), Common._S(37), 0, 0);
		mColorBlindSlider.Resize(mSfxVolumeSlider.mX - Common._S(80), mSfxVolumeSlider.mY + Common._S(45), imageByID.GetWidth(), imageByID.GetHeight());
		mHelpButton.Resize(10 + mSfxVolumeSlider.mX + Common._S(115), mColorBlindSlider.mY + Common._S(100), OPTIONS_BUTTON_WIDTH, OPTIONS_BUTTON_HEIGHT);
		mBackToGame.Resize(10 + mHelpButton.mX - mHelpButton.mWidth - Common._S(50), mHelpButton.mY, OPTIONS_BUTTON_WIDTH, OPTIONS_BUTTON_HEIGHT);
		mMainMenuButton.Resize(10 + mBackToGame.mX - mBackToGame.mWidth + Common._S(-50), mBackToGame.mY, OPTIONS_BUTTON_WIDTH, OPTIONS_BUTTON_HEIGHT);
	}

	private void SetMusicSlider(double inVolume)
	{
		if (mMusicEnabled)
		{
			GameApp.gApp.SetMusicVolume(inVolume);
		}
		if (!mMusicVolumeSlider.mDragging)
		{
			mMusicSliderOn = mMusicEnabled && inVolume > 0.0;
			mMusicVolumeSlider.Label = (mMusicSliderOn ? TextManager.getInstance().getString(672) : TextManager.getInstance().getString(682));
			mMusicVolumeSlider.mDisabled = !mMusicEnabled;
			GameApp.gApp.mMusic.Enable(mMusicSliderOn);
		}
	}

	private void SetSfxSlider(double inVolume)
	{
		GameApp.gApp.SetSfxVolume(inVolume);
	}

	public void SliderReleased(int theId, double theVal)
	{
	}

	public void OnLegalInfoHided()
	{
		mState = OptionState.OptionState_None;
	}

	public void OnCreditsHided()
	{
		mState = OptionState.OptionState_None;
	}

	public void OnHelpHided()
	{
		mState = OptionState.OptionState_None;
	}

	public void ProcessHardwareBackButton()
	{
		switch (mState)
		{
		case OptionState.OptionState_OptionToMainMenuPrompt:
			mState = OptionState.OptionState_None;
			GameApp.gApp.GetDialog(1)?.ButtonDepress(1001);
			GameApp.gApp.OnHardwareBackButtonPressProcessed();
			break;
		case OptionState.OptionState_Help:
		{
			mState = OptionState.OptionState_None;
			Board board = GameApp.gApp.GetBoard();
			if (board != null && board.GauntletMode())
			{
				board.ChallengeHelpClosed();
			}
			else
			{
				GameApp.gApp.mGenericHelp.ButtonDepress(0);
			}
			GameApp.gApp.OnHardwareBackButtonPressProcessed();
			break;
		}
		case OptionState.OptionState_Credits:
			mState = OptionState.OptionState_None;
			GameApp.gApp.ReturnFromCredits();
			GameApp.gApp.OnHardwareBackButtonPressProcessed();
			break;
		case OptionState.OptionState_Legal:
			GameApp.gApp.mLegalInfo.ProcessHardwareBackButton();
			if (GameApp.gApp.mLegalInfo == null)
			{
				mState = OptionState.OptionState_None;
			}
			break;
		default:
			mState = OptionState.OptionState_None;
			SetMusicSlider(mOriginMusicVolume);
			SetSfxSlider(mOriginSfxVolume);
			GameApp.gApp.FinishOptionsDialog(doSave: false);
			GameApp.gApp.OnHardwareBackButtonPressProcessed();
			break;
		}
	}
}
