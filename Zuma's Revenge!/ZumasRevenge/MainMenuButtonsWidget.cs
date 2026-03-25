using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.Tasks;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class MainMenuButtonsWidget : Widget, ButtonListener
{
	public struct MenuButtonFrame
	{
		public ButtonWidget mButton;

		public ButtonWidget mAttachButton;

		public float mX;

		public float mY;
	}

	public GameApp mApp;

	public MainMenu mMenu;

	public int mCurrentlySelectedButton;

	public Queue<MenuButtonFrame> mButtonFrames = new Queue<MenuButtonFrame>();

	public int mButtonWidth;

	public int mPlankHeight;

	public int mButtonOriginX;

	public int mButtonOriginY;

	public int mVisibleButtonX;

	private Image IMAGE_UI_MAINMENU_TIKI;

	private Image IMAGE_UI_MAINMENU_SCROLLMENU_BORDER;

	private Image IMAGE_UI_MAINMENU_SCROLLMENU_BUTTON;

	private Image IMAGE_UI_MAINMENU_ADVENTURE_OFF_STATE;

	private Image IMAGE_UI_MAINMENU_ADVENTURE_CLICK;

	private Image IMAGE_UI_MAINMENU_CHALLENGE_CLICK;

	private Image IMAGE_UI_MAINMENU_CHALLENGE_OFF_STATE;

	private Image IMAGE_UI_MAINMENU_LEADERBOARD_OFF_STATE;

	private Image IMAGE_UI_MAINMENU_LEADERBOARD_CLICK;

	private Image IMAGE_UI_MAINMENU_ACHIEVEMENT_CLICK;

	private Image IMAGE_UI_MAINMENU_ACHIEVEMENT_OFF_STATE;

	private Image IMAGE_UI_MAINMENU_HELP_CLICK;

	private Image IMAGE_UI_MAINMENU_HELP_OFF_STATE;

	private Image IMAGE_UI_MAINMENU_TIKI_CLICK;

	private Image IMAGE_UI_MAINMENU_TIKI_OFF_STATE;

	private Image IMAGE_UI_MAINMENU_SCROLLMENU_WOODTEXTURE;

	private Image IMAGE_UI_MAINMENU_SCROLLMENU_DECORATION;

	private Image IMAGE_UI_MAINMENU_SCROLLMENU_BORDERBOT;

	private Image IMAGE_UI_MAINMENU_SCROLLMENU_DECORATIONBOT;

	private Image IMAGE_UI_MM_FLOWER;

	private Image IMAGE_UI_MM_FLOWERBOT;

	private static int aTRSideOffsetX = 20;

	private static int flowerXOff = 20;

	private static int flowerYOff = 20;

	private bool EnsureMenuNavigationReady()
	{
		if (mApp == null)
		{
			return false;
		}
		if (mApp.mBambooTransition == null)
		{
			mApp.mBambooTransition = new BambooTransition();
		}
		if (mApp.mUserProfile == null && mApp.mProfileMgr != null)
		{
			ZumaProfile zumaProfile = null;
			if (!string.IsNullOrWhiteSpace(mApp.m_DefaultProfileName))
			{
				zumaProfile = (ZumaProfile)mApp.mProfileMgr.GetProfile(mApp.m_DefaultProfileName);
			}
			if (zumaProfile == null)
			{
				zumaProfile = (ZumaProfile)mApp.mProfileMgr.GetAnyProfile();
			}
			if (zumaProfile == null)
			{
				string text = string.IsNullOrWhiteSpace(mApp.m_DefaultProfileName) ? "Player 1" : mApp.m_DefaultProfileName;
				zumaProfile = (ZumaProfile)mApp.mProfileMgr.AddProfile(text);
				if (zumaProfile == null)
				{
					zumaProfile = (ZumaProfile)mApp.mProfileMgr.GetProfile(text);
				}
			}
			if (zumaProfile != null)
			{
				mApp.mUserProfile = zumaProfile;
				if (GameApp.gDDS != null)
				{
					GameApp.gDDS.ChangeProfile(zumaProfile);
				}
				mApp.RegistryWriteString("LastUser", zumaProfile.GetName());
			}
		}
		return mApp.mUserProfile != null;
	}

	private void StartMenuTransition(BambooTransition.BambooTransitionDelegate transitionDelegate)
	{
		if (mApp.mBambooTransition == null)
		{
			mApp.mBambooTransition = new BambooTransition();
		}
		mApp.mBambooTransition.mTransitionDelegate = transitionDelegate;
		mApp.ToggleBambooTransition();
	}

	public void ButtonPress(int theId)
	{
	}

	public void ButtonPress(int theId, int theClickCount)
	{
	}

	public void ButtonDepress(int theId)
	{
		GameApp gameApp = mApp;
		if (mMenu.mFirstTimeAlpha > 0 || mMenu.mIFUnlockAnim != null || mApp.mGenericHelp != null || mMenu.mDelayedIFStartState > 0 || mMenu.ShowingTikiTemple() || mApp.mMapScreen != null || mMenu.mState == MainMenu_State.State_Scroll || (gameApp.mBambooTransition != null && gameApp.mBambooTransition.IsInProgress()) || (mMenu.mChallengeMenu != null && (mMenu.mUpdateCnt - mMenu.mChallengeMenu.mCSVisFrame < 10 || mMenu.mChallengeMenu.mCrownZoomType >= 0 || mMenu.mChallengeMenu.mDoBounceTrophy || mMenu.mChallengeMenu.mCrossFadeTrophies)))
		{
			return;
		}
		Dialog dialog = gameApp.GetDialog(2);
		if (dialog != null)
		{
			return;
		}
		if (!EnsureMenuNavigationReady())
		{
			return;
		}
		mMenu.mTip = null;
		mApp.mClickedHardMode = false;
		GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BUTTON3));
		switch (theId)
		{
		case 0:
			if (!mApp.IsRegistered() && mApp.mTrialType == 1 && (mApp.mUserProfile.mAdvModeVars.mCurrentAdvZone > 2 || mApp.mUserProfile.mAdvModeVars.mHighestZoneBeat >= 2))
			{
				mApp.DoUpsell(from_exit: false);
			}
			else if (!mApp.mUserProfile.mNeedsFirstTimeIntro)
			{
				StartMenuTransition(mApp.ShowAdventureModeMapScreen);
			}
			else
			{
				mMenu.mFirstTimeAlpha = 1;
			}
			break;
		case 1:
			if (GameApp.USE_TRIAL_VERSION)
			{
				mMenu.mState = MainMenu_State.State_QuitPrompt;
				string message = TextManager.getInstance().getString(836);
				int width_pad = Common._DS(Common._M(20));
				GameApp.gApp.DoYesNoDialog(TextManager.getInstance().getString(835), message, block: true, TextManager.getInstance().getString(446), TextManager.getInstance().getString(447), drag: false, Common._S(Common._M(50)), 1, width_pad);
				GameApp.gApp.mYesNoDialogDelegate = ProcessUnlock;
			}
			else if (!mApp.ChallengeModeUnlocked())
			{
				mMenu.mState = MainMenu_State.State_UnlockPrompt;
				mApp.DoGenericDialog(TextManager.getInstance().getString(837), TextManager.getInstance().getString(838), block: true, ChangeMainMenuState, Common._DS(100));
				mMenu.mSkipEnterSound = true;
			}
			else
			{
				StartMenuTransition(mMenu.ShowChallengeMenuFromMainMenu);
				mApp.mUserProfile.mDoChallengeAceCupComplete = (mApp.mUserProfile.mDoChallengeCupComplete = false);
				mApp.mUserProfile.mDoChallengeAceTrophyZoom = (mApp.mUserProfile.mDoChallengeTrophyZoom = false);
				mApp.mUserProfile.mNewChallengeCupUnlocked = false;
			}
			break;
		case 9:
			StartMenuTransition(gameApp.mMainMenu.ShowTikiTemple);
			break;
		case 3:
			if (GameApp.USE_TRIAL_VERSION)
			{
				ProcessLocked(unlock: false);
				break;
			}
			if (GameApp.UN_UPDATE_VERSION)
			{
				GameApp.gApp.HandleGameUpdateRequired(null);
				break;
			}
			StartMenuTransition(GameApp.gApp.mMainMenu.ShowAchievements);
			break;
		case 2:
			if (GameApp.USE_TRIAL_VERSION)
			{
				ProcessLocked(unlock: false);
				break;
			}
			if (GameApp.UN_UPDATE_VERSION)
			{
				GameApp.gApp.HandleGameUpdateRequired(null);
				break;
			}
			StartMenuTransition(GameApp.gApp.mMainMenu.ShowLeaderBoards);
			break;
		case 4:
			GameApp.gApp.OpenURL("http://mg.eamobile.com/?rId=1560");
			break;
		case 14:
			if (GameApp.USE_TRIAL_VERSION)
			{
				ProcessLocked(unlock: true);
			}
			break;
		}
	}

	public void ChangeMainMenuState()
	{
		mMenu.mState = MainMenu_State.State_MainMenu;
	}

	public void ProcessLocked(bool unlock)
	{
		mMenu.mState = MainMenu_State.State_QuitPrompt;
		string text = "";
		text = ((!unlock) ? TextManager.getInstance().getString(836) : TextManager.getInstance().getString(834));
		int width_pad = Common._DS(Common._M(20));
		GameApp.gApp.DoYesNoDialog(TextManager.getInstance().getString(835), text, block: true, TextManager.getInstance().getString(446), TextManager.getInstance().getString(447), drag: false, Common._S(Common._M(50)), 1, width_pad);
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

	public void ProcessUpdateLocked()
	{
		mMenu.mState = MainMenu_State.State_QuitPrompt;
		string message = TextManager.getInstance().getString(62);
		int width_pad = Common._DS(Common._M(20));
		GameApp.gApp.DoYesNoDialog(TextManager.getInstance().getString(62), message, block: true, TextManager.getInstance().getString(446), TextManager.getInstance().getString(447), drag: false, Common._S(Common._M(50)), 1, width_pad);
		GameApp.gApp.mYesNoDialogDelegate = ProcessUpdateUnlock;
	}

	public void ProcessUpdateUnlock(int theId)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected O, but got Unknown
		if (theId == 1000)
		{
			MarketplaceDetailTask val = new MarketplaceDetailTask();
			val.ContentType = (MarketplaceContentType)1;
			val.Show();
		}
		mMenu.mState = MainMenu_State.State_MainMenu;
	}

	public void UpdateLeaderboard()
	{
		mMenu.mState = MainMenu_State.State_QuitPrompt;
		string message = TextManager.getInstance().getString(61);
		int width_pad = Common._DS(Common._M(20));
		GameApp.gApp.DoYesNoDialog(TextManager.getInstance().getString(61), message, block: true, TextManager.getInstance().getString(446), TextManager.getInstance().getString(447), drag: false, Common._S(Common._M(50)), 1, width_pad);
		GameApp.gApp.mYesNoDialogDelegate = ProcessLeaderboard;
	}

	public void ProcessLeaderboard(int theId)
	{
		if (theId == 1000 && EnsureMenuNavigationReady())
		{
			StartMenuTransition(GameApp.gApp.mMainMenu.ShowLeaderBoards);
		}
		mMenu.mState = MainMenu_State.State_MainMenu;
	}

	public void UpdateAchievement()
	{
		mMenu.mState = MainMenu_State.State_QuitPrompt;
		string message = TextManager.getInstance().getString(61);
		int width_pad = Common._DS(Common._M(20));
		GameApp.gApp.DoYesNoDialog(TextManager.getInstance().getString(61), message, block: true, TextManager.getInstance().getString(446), TextManager.getInstance().getString(447), drag: false, Common._S(Common._M(50)), 1, width_pad);
		GameApp.gApp.mYesNoDialogDelegate = ProcessAchievement;
	}

	public void ProcessAchievement(int theId)
	{
		if (theId == 1000 && EnsureMenuNavigationReady())
		{
			StartMenuTransition(GameApp.gApp.mMainMenu.ShowAchievements);
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

	public MainMenuButtonsWidget(MainMenu theMenu, GameApp theApp)
	{
		mApp = theApp;
		mMenu = theMenu;
		mWidth = mApp.GetScreenWidth();
		IMAGE_UI_MAINMENU_TIKI = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_TIKI);
		IMAGE_UI_MAINMENU_SCROLLMENU_BORDER = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_BORDER);
		IMAGE_UI_MAINMENU_SCROLLMENU_BUTTON = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_BUTTON);
		IMAGE_UI_MAINMENU_ADVENTURE_OFF_STATE = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_ADVENTURE_OFF_STATE);
		IMAGE_UI_MAINMENU_ADVENTURE_CLICK = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_ADVENTURE_CLICK);
		IMAGE_UI_MAINMENU_CHALLENGE_CLICK = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_CHALLENGE_CLICK);
		IMAGE_UI_MAINMENU_CHALLENGE_OFF_STATE = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_CHALLENGE_OFF_STATE);
		IMAGE_UI_MAINMENU_TIKI_CLICK = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_TIKI_CLICK);
		IMAGE_UI_MAINMENU_TIKI_OFF_STATE = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_TIKI_OFF_STATE);
		IMAGE_UI_MAINMENU_SCROLLMENU_WOODTEXTURE = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_WOODTEXTURE);
		IMAGE_UI_MAINMENU_SCROLLMENU_DECORATION = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_DECORATION);
		IMAGE_UI_MAINMENU_SCROLLMENU_BORDERBOT = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_BORDERBOT);
		IMAGE_UI_MAINMENU_SCROLLMENU_DECORATIONBOT = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_DECORATIONBOT);
		IMAGE_UI_MM_FLOWER = Res.GetImageByID(ResID.IMAGE_UI_MM_FLOWER);
		IMAGE_UI_MM_FLOWERBOT = Res.GetImageByID(ResID.IMAGE_UI_MM_FLOWERBOT);
		IMAGE_UI_MAINMENU_LEADERBOARD_OFF_STATE = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_LEADERBOARD_OFF_STATE);
		IMAGE_UI_MAINMENU_LEADERBOARD_CLICK = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_LEADERBOARD_CLICK);
		IMAGE_UI_MAINMENU_ACHIEVEMENT_CLICK = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_ACHIEVEMENT_CLICK);
		IMAGE_UI_MAINMENU_ACHIEVEMENT_OFF_STATE = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_ACHIEVEMENT_OFF_STATE);
		IMAGE_UI_MAINMENU_HELP_CLICK = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_HELP_CLICK);
		IMAGE_UI_MAINMENU_HELP_OFF_STATE = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_HELP_OFF_STATE);
		mHeight = IMAGE_UI_MAINMENU_TIKI.GetHeight() + Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_TIKI) - Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_TIKI)) + Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_BORDER) + IMAGE_UI_MAINMENU_SCROLLMENU_BORDER.GetHeight() - Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_TIKI) + IMAGE_UI_MAINMENU_TIKI.GetHeight())));
		mButtonWidth = IMAGE_UI_MAINMENU_TIKI.GetWidth() * 2 + IMAGE_UI_MAINMENU_SCROLLMENU_BUTTON.GetWidth() + IMAGE_UI_MAINMENU_SCROLLMENU_BORDER.GetWidth() / 2;
		mButtonOriginX = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_CHALLENGE_OFF_STATE)) - Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_TIKI)) - IMAGE_UI_MAINMENU_TIKI.GetWidth();
		mButtonOriginY = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_CHALLENGE_OFF_STATE)) - Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_TIKI));
		mPlankHeight = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_TIKI)) + IMAGE_UI_MAINMENU_TIKI.GetHeight() - Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_WOODTEXTURE));
		IMAGE_UI_MAINMENU_CHALLENGE_CLICK.GetWidth();
		IMAGE_UI_MAINMENU_CHALLENGE_CLICK.GetHeight();
		int num = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_CHALLENGE_OFF_STATE));
		int num2 = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_CHALLENGE_OFF_STATE)) + 8;
		AddNewButtonFrame(0, IMAGE_UI_MAINMENU_ADVENTURE_OFF_STATE, IMAGE_UI_MAINMENU_ADVENTURE_CLICK, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_ADVENTURE_OFF_STATE)) - num, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_ADVENTURE_OFF_STATE)) - num2);
		AddNewButtonFrame(1, IMAGE_UI_MAINMENU_CHALLENGE_OFF_STATE, IMAGE_UI_MAINMENU_CHALLENGE_CLICK, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_CHALLENGE_OFF_STATE)) - num, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_CHALLENGE_OFF_STATE)) - num2);
		AddNewButtonFrame(9, IMAGE_UI_MAINMENU_TIKI_OFF_STATE, IMAGE_UI_MAINMENU_TIKI_CLICK, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_TIKI_OFF_STATE)) - num, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_TIKI_OFF_STATE)) - num2);
		AddNewButtonFrame(3, IMAGE_UI_MAINMENU_ACHIEVEMENT_OFF_STATE, IMAGE_UI_MAINMENU_ACHIEVEMENT_CLICK, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_ACHIEVEMENT_OFF_STATE)) - num, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_ACHIEVEMENT_OFF_STATE)) - num2);
		AddNewButtonFrame(2, IMAGE_UI_MAINMENU_LEADERBOARD_OFF_STATE, IMAGE_UI_MAINMENU_LEADERBOARD_CLICK, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_LEADERBOARD_OFF_STATE)) - num, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_LEADERBOARD_OFF_STATE)) - num2);
		AddNewButtonFrame(4, IMAGE_UI_MAINMENU_HELP_OFF_STATE, IMAGE_UI_MAINMENU_HELP_CLICK, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_HELP_OFF_STATE)) - num, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_HELP_OFF_STATE)) - num2);
	}

	public override void Dispose()
	{
		if (mButtonFrames.Count > 0)
		{
			mButtonFrames.Clear();
		}
	}

	public int GetButtonCount()
	{
		return mButtonFrames.Count;
	}

	public void AddNewButtonFrame(int theButtonID, Image theButtonImage, Image theButtonDownImage, int x, int y)
	{
		MenuButtonFrame item = default(MenuButtonFrame);
		if (mButtonFrames.Count == 0)
		{
			item.mX = 0f;
			item.mY = 0f;
		}
		else
		{
			item.mX = mButtonFrames.Last().mX + (float)mButtonWidth;
			item.mY = 0f;
		}
		item.mButton = new ButtonWidget(theButtonID, this);
		item.mButton.mButtonImage = theButtonImage;
		item.mButton.mDownImage = theButtonDownImage;
		float num = (float)(theButtonDownImage.GetWidth() - theButtonImage.GetWidth()) / 2f;
		float num2 = (float)(theButtonDownImage.GetHeight() - theButtonImage.GetHeight()) / 2f;
		item.mButton.Resize((int)(item.mX + (float)mButtonOriginX + num) + x, (int)(item.mY + (float)mButtonOriginY + num2) + y, theButtonDownImage.GetWidth(), theButtonDownImage.GetHeight());
		item.mButton.mNormalRect = new Rect(0, 0, theButtonImage.GetWidth(), theButtonImage.GetHeight());
		item.mButton.mDownRect = new Rect((int)num, (int)num2, (int)((float)theButtonDownImage.GetWidth() - num), (int)((float)theButtonDownImage.GetHeight() - num2));
		AddWidget(item.mButton);
		mButtonFrames.Enqueue(item);
		Resize((int)mButtonFrames.First().mX, 0, mButtonFrames.Count * mButtonWidth, mHeight);
	}

	public void DrawButtonFrame(Graphics g, MenuButtonFrame theFrame)
	{
		int num = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_TIKI)) + IMAGE_UI_MAINMENU_TIKI.GetWidth() - Common._DS(11);
		int num2 = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_TIKI));
		int num3 = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_WOODTEXTURE)) - num2;
		g.ClearClipRect();
		g.SetClipRect((int)theFrame.mX, num3, mButtonWidth, mPlankHeight - Common._DS(15));
		int i = (int)theFrame.mX;
		int num4 = num3;
		bool flag = false;
		while (num4 <= num3 + mPlankHeight)
		{
			for (; (float)i <= theFrame.mX + (float)mButtonWidth; i += IMAGE_UI_MAINMENU_SCROLLMENU_WOODTEXTURE.GetWidth())
			{
				if (flag)
				{
					g.DrawImageMirror(IMAGE_UI_MAINMENU_SCROLLMENU_WOODTEXTURE, i, num4);
				}
				else
				{
					g.DrawImage(IMAGE_UI_MAINMENU_SCROLLMENU_WOODTEXTURE, i, num4);
				}
			}
			i = (int)theFrame.mX;
			num4 += IMAGE_UI_MAINMENU_SCROLLMENU_WOODTEXTURE.GetHeight();
			flag = false;
		}
		g.ClearClipRect();
		int num5 = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_BORDER)) - num2;
		int num6 = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_DECORATION)) - num;
		int num7 = mButtonWidth - IMAGE_UI_MAINMENU_TIKI.GetWidth() - IMAGE_UI_MAINMENU_SCROLLMENU_DECORATION.GetWidth() + Common._DS(aTRSideOffsetX);
		int num8 = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_DECORATION)) - num2;
		g.DrawImage(IMAGE_UI_MAINMENU_SCROLLMENU_BORDER, (int)theFrame.mX, (int)(theFrame.mY + (float)num5), mButtonWidth, IMAGE_UI_MAINMENU_SCROLLMENU_BORDER.GetHeight());
		g.DrawImage(IMAGE_UI_MAINMENU_SCROLLMENU_DECORATION, (int)(theFrame.mX + (float)num6), (int)(theFrame.mY + (float)num8));
		g.DrawImageMirror(IMAGE_UI_MAINMENU_SCROLLMENU_DECORATION, (int)(theFrame.mX + (float)num7), (int)(theFrame.mY + (float)num8));
		int num9 = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_BORDERBOT)) - num2;
		int num10 = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_DECORATIONBOT)) - num2;
		int num11 = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_DECORATIONBOT)) - num + Common._DS(-10);
		int num12 = mButtonWidth - IMAGE_UI_MAINMENU_TIKI.GetWidth() - IMAGE_UI_MAINMENU_SCROLLMENU_DECORATION.GetWidth() + Common._DS(27);
		g.DrawImage(IMAGE_UI_MAINMENU_SCROLLMENU_BORDERBOT, (int)theFrame.mX, (int)(theFrame.mY + (float)num9), mButtonWidth, IMAGE_UI_MAINMENU_SCROLLMENU_BORDER.GetHeight());
		g.DrawImage(IMAGE_UI_MAINMENU_SCROLLMENU_DECORATIONBOT, (int)(theFrame.mX + (float)num11), (int)(theFrame.mY + (float)num10));
		g.DrawImageMirror(IMAGE_UI_MAINMENU_SCROLLMENU_DECORATIONBOT, (int)(theFrame.mX + (float)num12), (int)(theFrame.mY + (float)num10));
		int num13 = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_BUTTON)) - num;
		int num14 = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_BUTTON)) - num2;
		g.DrawImage(IMAGE_UI_MAINMENU_SCROLLMENU_BUTTON, (int)(theFrame.mX + (float)num13), (int)(theFrame.mY + (float)num14));
		int num15 = 0;
		g.DrawImageMirror(IMAGE_UI_MAINMENU_TIKI, (int)theFrame.mX, (int)(theFrame.mY + (float)num15));
		int num16 = mButtonWidth - IMAGE_UI_MAINMENU_TIKI.GetWidth();
		g.DrawImage(IMAGE_UI_MAINMENU_TIKI, (int)(theFrame.mX + (float)num16), (int)(theFrame.mY + (float)num15));
		g.DrawImage(IMAGE_UI_MM_FLOWER, (int)(theFrame.mX + (float)num13 - (float)Common._DS(flowerXOff)), (int)(theFrame.mY + (float)num14 - (float)Common._DS(flowerYOff)));
		g.DrawImageMirror(IMAGE_UI_MM_FLOWER, (int)(theFrame.mX + (float)num13 + (float)IMAGE_UI_MAINMENU_SCROLLMENU_BUTTON.GetWidth() - (float)IMAGE_UI_MM_FLOWER.GetWidth() + (float)Common._DS(flowerXOff)), (int)(theFrame.mY + (float)num14 - (float)Common._DS(flowerYOff)));
		g.DrawImage(IMAGE_UI_MM_FLOWERBOT, (int)(theFrame.mX + (float)num13 - (float)Common._DS(flowerXOff)), (int)(theFrame.mY + (float)num14 + (float)IMAGE_UI_MAINMENU_SCROLLMENU_BUTTON.GetHeight() - (float)IMAGE_UI_MM_FLOWERBOT.GetHeight() + (float)Common._DS(flowerYOff)));
		g.DrawImageMirror(IMAGE_UI_MM_FLOWERBOT, (int)(theFrame.mX + (float)num13 + (float)IMAGE_UI_MAINMENU_SCROLLMENU_BUTTON.GetWidth() - (float)IMAGE_UI_MM_FLOWERBOT.GetWidth() + (float)Common._DS(flowerXOff)), (int)(theFrame.mY + (float)num14 + (float)IMAGE_UI_MAINMENU_SCROLLMENU_BUTTON.GetHeight() - (float)IMAGE_UI_MM_FLOWERBOT.GetHeight() + (float)Common._DS(flowerYOff)));
	}

	public override void Draw(Graphics g)
	{
		base.Draw(g);
		if ((mApp == null || mApp.mMapScreen == null) && (mMenu == null || mMenu.mChallengeMenu == null) && mButtonFrames.Count > 0)
		{
			Queue<MenuButtonFrame>.Enumerator enumerator = mButtonFrames.GetEnumerator();
			while (enumerator.MoveNext())
			{
				DrawButtonFrame(g, enumerator.Current);
			}
		}
	}

	public override void Update()
	{
		base.Update();
		int pageHorizontal = mMenu.mMainMenuButtonsScrollWidget.GetPageHorizontal();
		int num = 0;
		Queue<MenuButtonFrame>.Enumerator enumerator = mButtonFrames.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (num == pageHorizontal)
			{
				enumerator.Current.mButton.SetDisabled(isDisabled: false);
			}
			else
			{
				enumerator.Current.mButton.SetDisabled(isDisabled: true);
			}
			num++;
		}
	}

	public void HideScrollButtons()
	{
		Queue<MenuButtonFrame>.Enumerator enumerator = mButtonFrames.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.mButton.SetVisible(isVisible: false);
		}
	}

	public void ShowScrollButtons()
	{
		Queue<MenuButtonFrame>.Enumerator enumerator = mButtonFrames.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.mButton.SetVisible(isVisible: true);
		}
	}

	public int GetNumButtons()
	{
		return mButtonFrames.Count;
	}
}
