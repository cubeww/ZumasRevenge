using System;
using System.Collections.Generic;
using System.Text;
using JeffLib;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class MainMenu : Widget, ButtonListener, DialogListener, PopAnimListener
{
	private const string WatermarkText = "by bilibili NyaCube";

	public class VolcanoProjectile
	{
		public PIEffect mProjectile;

		public bool mInUse;
	}

	public class MMText
	{
		public float mAlpha;

		public string mText;

		public string mExtraText;

		public bool mFadingIn;

		public bool mShowChallengeCrowns;

		public int mYOff;
	}

	private static int MAX_VOLCANO_PROJECTILES = 6;

	private static bool gNeedsIFUnlockSound = false;

	private static bool gNeedsOtherModeUnlockSound = false;

	public static int gScreenShake = 0;

	public static int gScreenShakeTimer = 0;

	public float mAdjust;

	private Point[] mPts = new Point[6]
	{
		new Point((int)Common._DSA(1100f, 0f), Common._S(Common._M(253))),
		new Point((int)Common._DSA(1102f, 0f), Common._S(Common._M1(315))),
		new Point(Common._DS(Common._M2(0)), Common._S(Common._M3(376))),
		new Point((int)Common._DSA(1100f, 0f), Common._S(Common._M4(490))),
		new Point((int)Common._DSA(1288f, 0f), Common._S(Common._M5(490))),
		new Point((int)Common._DSA(1100f, 0f), Common._S(Common._M6(430)))
	};

	public GameApp mApp;

	public ZumaTip mTip;

	public List<MMText> mText = new List<MMText>();

	public PIEffect mHeroicSparkle;

	public PIEffect mIFSparkle;

	public PIEffect mChallengeSparkle;

	public PIEffectBatch mEffectBatch;

	public PopAnim mIFUnlockAnim;

	public float mDistance;

	public float mAddAcc;

	public float mLavaAlpha;

	public MainMenu_State mState;

	public int mDelayedIFStartState;

	public int mFirstTimeAlpha;

	public bool mIncLavaAlpha;

	public bool mDrawHat;

	public bool mDrawMoustache;

	public bool mDrawTuxedo;

	public bool mDrawFro;

	public bool mSkipEnterSound;

	public Achievements mAchievements;

	public ChallengeMenu mChallengeMenu;

	public TikiTemple mTikiTemple;

	public LeaderBoards mLeaderBoards;

	public ZumaUserSelDlg mUserSelDlg;

	private List<ButtonWidget> mButtons = new List<ButtonWidget>();

	private ButtonWidget mUpsellBtn;

	private ButtonWidget mChangeProfileBtn;

	public ScrollWidget mMainMenuButtonsScrollWidget;

	public MainMenuButtonsWidget mMainMenuButtonsWidget;

	public ButtonWidget mMoreGamesButton;

	public ButtonWidget mOptionsButton;

	public ButtonWidget mUnlockButton;

	public int mMenuScrollOriginY;

	public int mMenuScrollDestY;

	public int mMenuScrollStartY;

	public int mMenuTikiStartX;

	public int mMenuTikiOriginX;

	public int mMenuTikiDestX;

	public int mMenuTikiX;

	public int mMenuFrogStartX;

	public int mMenuFrogOriginX;

	public int mMenuFrogDestX;

	public int mMenuFrogX;

	public int mMenuTikiDudeStartX;

	public int mMenuTikiDudeOriginX;

	public int mMenuTikiDudeDestX;

	public int mMenuTikiDudeX;

	public ButtonWidget mMonkeyButton;

	public ButtonWidget mLogButton;

	private MainMenuOverlayWidget mMainMenuOverlayWidget;

	private CurvedVal mMenuScrollPct = new CurvedVal();

	private List<string> mTalkingBubbleTextOptions = new List<string>();

	private VolcanoProjectile[] mVolcanoProjectiles;

	private PIEffect mVolcanoSmoke;

	private PIEffect mTikiTeethSparkle;

	private float mLavaXOff;

	private float mLavaXScale;

	private float mLavaProjectileXOff;

	private float mLavaSmokeXOff;

	private float mTeethSparkleXOff;

	private Image IMAGE_UI_MAINMENU_TIKI;

	public Rect mCSOverRect;

	public string mGauntletModLevel_id;

	private static int tailXOff = 5;

	public void ButtonPress(int theId)
	{
	}

	public void ButtonPress(int theId, int theClickCount)
	{
		if (!ShowingTikiTemple() && mApp.mGenericHelp == null && mApp.mMapScreen == null && mApp.mCredits == null)
		{
			if (theId != 8 && mMainMenuOverlayWidget != null)
			{
				mMainMenuOverlayWidget.ButtonPress(theId);
			}
			GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BUTTON1));
		}
	}

	public void ButtonDepress(int theId)
	{
		if (mFirstTimeAlpha > 0 || mIFUnlockAnim != null || mApp.mGenericHelp != null || mDelayedIFStartState > 0 || ShowingTikiTemple() || mApp.mMapScreen != null || mState == MainMenu_State.State_Scroll || (mApp.mBambooTransition != null && mApp.mBambooTransition.IsInProgress()))
		{
			return;
		}
		_ = mChallengeMenu;
		if (mApp.GetDialog(2) != null)
		{
			return;
		}
		mTip = null;
		mApp.mClickedHardMode = false;
		switch (theId)
		{
		case 6:
		case 8:
		case 11:
		case 13:
		case 17:
			break;
		case 7:
			mSkipEnterSound = true;
			if (mApp.DoYesNoDialog(TextManager.getInstance().getString(448), TextManager.getInstance().getString(453), block: true) == 1000)
			{
				if (!mApp.IsRegistered() && mApp.mTrialType == 1 && mApp.GetBoolean("UpsellExit", theDefault: false))
				{
					mApp.DoUpsell(from_exit: true);
				}
				else
				{
					mApp.Shutdown();
				}
			}
			break;
		case 1:
			if (!mApp.ChallengeModeUnlocked())
			{
				mState = MainMenu_State.State_UnlockPrompt;
				mApp.DoGenericDialog(TextManager.getInstance().getString(837), TextManager.getInstance().getString(838), block: true, ChangeMainMenuState, Common._DS(100));
				mSkipEnterSound = true;
			}
			else
			{
				mApp.mUserProfile.mDoChallengeAceCupComplete = (mApp.mUserProfile.mDoChallengeCupComplete = false);
				mApp.mUserProfile.mDoChallengeAceTrophyZoom = (mApp.mUserProfile.mDoChallengeTrophyZoom = false);
				mApp.mUserProfile.mNewChallengeCupUnlocked = false;
				ShowChallengeMenu();
			}
			break;
		case 16:
			DoMainMenu(scroll: true);
			break;
		case 15:
		{
			ButtonWidget buttonWidget = null;
			for (int i = 0; i < mButtons.Count; i++)
			{
				if (mButtons[i].mId == theId)
				{
					buttonWidget = mButtons[i];
					break;
				}
			}
			buttonWidget.SetVisible(isVisible: false);
			MarkDirty();
			mDelayedIFStartState = 1;
			break;
		}
		case 12:
			if (mApp.mAutoMonkey != null)
			{
				mApp.mAutoMonkey.mEnableAutoMonkey = !mApp.mAutoMonkey.mEnableAutoMonkey;
			}
			break;
		default:
			if (mMainMenuButtonsWidget != null)
			{
				mMainMenuButtonsWidget.ButtonDepress(theId);
			}
			if (mMainMenuOverlayWidget != null)
			{
				mMainMenuOverlayWidget.ButtonDepress(theId);
			}
			break;
		}
	}

	public void ButtonDownTick(int theId)
	{
	}

	public void ButtonMouseEnter(int theId)
	{
		if (mApp.mCredits == null && mApp.mGenericHelp == null)
		{
			mSkipEnterSound = false;
		}
	}

	public void ButtonMouseLeave(int theId)
	{
		if (mApp.mCredits == null)
		{
			MarkDirty();
		}
	}

	public void ButtonMouseMove(int theId, int theX, int theY)
	{
	}

	public void ChangeMainMenuState()
	{
		mState = MainMenu_State.State_MainMenu;
	}

	public void DialogButtonPress(int theDialogId, int theButtonId)
	{
	}

	public void DialogButtonDepress(int theDialogId, int theButtonId)
	{
	}

	public void PopAnimPlaySample(string theSampleName, int thePan, double theVolume, double theNumSteps)
	{
	}

	public PIEffect PopAnimLoadParticleEffect(string theEffectName)
	{
		return null;
	}

	public bool PopAnimObjectPredraw(int theId, Graphics g, PASpriteInst theSpriteInst, PAObjectInst theObjectInst, PATransform theTransform, Color theColor)
	{
		return true;
	}

	public bool PopAnimObjectPostdraw(int theId, Graphics g, PASpriteInst theSpriteInst, PAObjectInst theObjectInst, PATransform theTransform, Color theColor)
	{
		return true;
	}

	public ImagePredrawResult PopAnimImagePredraw(int theId, PASpriteInst theSpriteInst, PAObjectInst theObjectInst, PATransform theTransform, Image theImage, Graphics g, int theDrawCount)
	{
		return ImagePredrawResult.ImagePredraw_Normal;
	}

	public void PopAnimStopped(int theId)
	{
	}

	public void PopAnimCommand(int theId, string theCommand, string theParam)
	{
	}

	public bool PopAnimCommand(int theId, PASpriteInst theSpriteInst, string theCommand, string theParam)
	{
		PopAnimCommand(theId, theCommand, theParam);
		return true;
	}

	public MainMenu(GameApp app)
	{
		mState = MainMenu_State.State_MainMenu;
		mUserSelDlg = null;
		mApp = app;
		mDistance = 0f;
		mAddAcc = 0f;
		mIFSparkle = null;
		mIFUnlockAnim = null;
		mHeroicSparkle = null;
		mTip = null;
		mFirstTimeAlpha = 0;
		mDelayedIFStartState = 0;
		mIncLavaAlpha = true;
		mUpsellBtn = null;
		mMainMenuButtonsWidget = null;
		mMenuScrollOriginY = -1;
		mMenuScrollDestY = -1;
		mMenuScrollStartY = -1;
		mMenuTikiStartX = -1;
		mMenuTikiDestX = -1;
		mMenuTikiOriginX = -1;
		mMenuTikiX = -1;
		mMenuFrogX = -1;
		mMenuFrogOriginX = -1;
		mMenuFrogStartX = -1;
		mMenuFrogDestX = -1;
		mMenuTikiDudeX = -1;
		mMenuTikiDudeDestX = -1;
		mMenuTikiDudeStartX = -1;
		mMenuTikiDudeOriginX = -1;
		mChallengeSparkle = null;
		mChallengeMenu = null;
		mTikiTemple = null;
		mMonkeyButton = null;
		mLogButton = null;
		mChangeProfileBtn = null;
		mClip = false;
		mMainMenuOverlayWidget = null;
		mTikiTeethSparkle = null;
		mVolcanoSmoke = null;
		mVolcanoProjectiles = null;
		mEffectBatch = new PIEffectBatch();
		mMoreGamesButton = null;
		mOptionsButton = null;
		mUnlockButton = null;
		mMenuScrollPct.SetConstant(0.0);
		mMenuScrollPct.mAppUpdateCountSrc = mUpdateCnt;
	}

	public override void Dispose()
	{
		RemoveAllWidgets(doDelete: true, recursive: true);
		if (mTikiTemple != null)
		{
			mWidgetManager.RemoveWidget(mTikiTemple);
		}
		if (mChallengeMenu != null)
		{
			mWidgetManager.RemoveWidget(mChallengeMenu);
		}
		mVolcanoSmoke.Dispose();
		mTikiTeethSparkle.Dispose();
	}

	public void DoIronFrog(bool scroll)
	{
		if (!scroll)
		{
			mState = MainMenu_State.State_IF;
			List<ButtonWidget>.Enumerator enumerator = mButtons.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.Move(enumerator.Current.mX + Common._S(960), enumerator.Current.mY);
			}
		}
		else
		{
			mState = MainMenu_State.State_Scroll;
			mDistance = 960f;
		}
		mAddAcc = 0f;
	}

	public void DoMainMenu(bool scroll)
	{
		if (mMainMenuButtonsWidget != null)
		{
			mMainMenuButtonsWidget.SetVisible(isVisible: true);
		}
		if (!scroll)
		{
			mState = MainMenu_State.State_MainMenu;
		}
		else
		{
			mDistance = 960f;
			mState = MainMenu_State.State_Scroll;
		}
		mAddAcc = 0f;
	}

	public void InitSparkles()
	{
		if (mApp.mUserProfile != null && mHeroicSparkle == null && mApp.mUserProfile.mAdvModeVars.mNumTimesZoneBeat[5] > 0 && !mApp.mUserProfile.mHasDoneHeroicUnlockEffect)
		{
			gNeedsOtherModeUnlockSound = true;
			mApp.mUserProfile.mHasDoneHeroicUnlockEffect = true;
			mHeroicSparkle = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_GOLDSPARKLE_AREA).Duplicate();
			float num = GameApp.DownScaleNum(1f);
			mHeroicSparkle.mDrawTransform.Scale(num, num);
			mHeroicSparkle.mDrawTransform.Translate(mPts[5].mX + Common._DS(Common._M(180)), mPts[5].mY - Common._DS(Common._M1(60)));
		}
		if (mApp.mUserProfile != null && mChallengeSparkle == null && mApp.mUserProfile.mAdvModeVars.mHighestLevelBeat >= 10 && !mApp.mUserProfile.mHasDoneChallengeUnlockEffect)
		{
			gNeedsOtherModeUnlockSound = true;
			mApp.mUserProfile.mHasDoneChallengeUnlockEffect = true;
			mChallengeSparkle = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_GOLDSPARKLE_AREA).Duplicate();
			float num2 = GameApp.DownScaleNum(1f);
			mChallengeSparkle.mDrawTransform.Scale(num2, num2);
			mChallengeSparkle.mDrawTransform.Translate(mPts[1].mX + Common._DS(Common._M(180)), mPts[1].mY - Common._DS(Common._M1(-60)));
		}
		if (mApp.mUserProfile != null && mIFSparkle == null && mApp.IronFrogUnlocked() && !mApp.mUserProfile.mHasDoneIFUnlockEffect)
		{
			gNeedsIFUnlockSound = true;
			mApp.mUserProfile.mHasDoneIFUnlockEffect = true;
			mIFSparkle = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_GOLDSPARKLE_AREA).Duplicate();
			float num3 = GameApp.DownScaleNum(1f);
			mIFSparkle.mDrawTransform.Scale(num3, num3);
			mIFSparkle.mDrawTransform.Scale(Common._M(3f), Common._M1(2f));
			mIFSparkle.mDrawTransform.Translate(Common._DS(Common._M(-30)), Common._DS(Common._M1(900)));
		}
	}

	public void DoMoreGamesSlide(bool isSlidingIn)
	{
		mMenuScrollStartY = mMainMenuButtonsScrollWidget.mY;
		mMenuScrollPct.SetCurve(Common._MP("b30,1,0.02,1,#  ,#  tO  o~  3~"));
		mMenuTikiStartX = mMenuTikiX;
		mMenuFrogStartX = mMenuFrogX;
		mMenuTikiDudeStartX = mMenuTikiDudeX;
		if (isSlidingIn)
		{
			mMenuScrollDestY = mMenuScrollOriginY;
			mMenuTikiDestX = mMenuTikiOriginX;
			mMenuFrogDestX = mMenuFrogOriginX;
			mMenuTikiDudeDestX = mMenuTikiDudeOriginX;
		}
		else
		{
			mMenuScrollDestY = mApp.mScreenBounds.mHeight + Common._S(150);
			mMenuTikiDudeDestX = (mMenuTikiDestX = -Common._S(300));
			mMenuFrogDestX = mApp.GetScreenWidth() + Common._S(300);
		}
		mMainMenuOverlayWidget.DoMoreGamesSlide(isSlidingIn);
	}

	public void Init()
	{
		IMAGE_UI_MAINMENU_TIKI = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_TIKI);
		mLavaXOff = 0f;
		mLavaXScale = 1.09f;
		mLavaProjectileXOff = ((GameApp.mGameRes == 768) ? 0f : ((float)((GameApp.mGameRes == 640) ? 150 : 60)));
		mLavaSmokeXOff = ((GameApp.mGameRes == 768) ? 0f : ((float)((GameApp.mGameRes == 640) ? 150 : 60)));
		mTeethSparkleXOff = ((GameApp.mGameRes == 768) ? 0f : ((float)((GameApp.mGameRes == 640) ? 144 : 57)));
		mUpdateCnt = 0;
		LoadTalkingBubbleText();
		if (mTalkingBubbleTextOptions.Count > 0)
		{
			Random random = new Random();
			int index = random.Next(0, mTalkingBubbleTextOptions.Count - 1);
			AddText(mTalkingBubbleTextOptions[index]);
		}
		InitSparkles();
		mMainMenuButtonsWidget = new MainMenuButtonsWidget(this, mApp);
		mMainMenuButtonsWidget.Resize(0, 0, mMainMenuButtonsWidget.mWidth, mMainMenuButtonsWidget.mHeight);
		mMainMenuButtonsScrollWidget = new ScrollWidget();
		mMainMenuButtonsScrollWidget.EnableBounce(enable: false);
		mMenuScrollOriginY = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_TIKI));
		mMainMenuButtonsScrollWidget.Resize(0, mMenuScrollOriginY, mApp.GetScreenWidth(), mApp.GetScreenRect().mHeight);
		mMainMenuButtonsScrollWidget.AddWidget(mMainMenuButtonsWidget);
		mMainMenuButtonsScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_HORIZONTAL);
		mMainMenuButtonsScrollWidget.EnablePaging(enable: true);
		AddWidget(mMainMenuButtonsScrollWidget);
		mMenuTikiOriginX = (mMenuTikiX = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_TIKIHEAD)) - mApp.mWideScreenXOffset);
		mMenuFrogOriginX = (mMenuFrogX = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_RIBBIT)) - mApp.mWideScreenXOffset + 42);
		mMenuTikiDudeOriginX = (mMenuTikiDudeX = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_GUY)) - mApp.mWideScreenXOffset);
		Insets insets = new Insets();
		insets.mLeft = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_TIKI)) - IMAGE_UI_MAINMENU_TIKI.GetWidth() - mApp.GetScreenRect().mX + 67;
		insets.mRight = mApp.GetScreenWidth() - (insets.mLeft + mMainMenuButtonsWidget.mWidth / mMainMenuButtonsWidget.GetNumButtons());
		insets.mTop = 0;
		insets.mBottom = 0;
		mMainMenuOverlayWidget = new MainMenuOverlayWidget(this);
		mMainMenuOverlayWidget.Init();
		AddWidget(mMainMenuOverlayWidget);
		mMainMenuButtonsScrollWidget.SetScrollInsets(insets);
		mMainMenuButtonsScrollWidget.SetPageHorizontal(2, animated: false);
		mMainMenuButtonsScrollWidget.SetPageHorizontal(0, animated: true);
		if (mVolcanoSmoke == null)
		{
			mVolcanoSmoke = mApp.GetPIEffect("ls_volcano_smoke");
			mVolcanoSmoke.mEmitAfterTimeline = true;
			Common.SetFXNumScale(mVolcanoSmoke, 4f);
			mEffectBatch.AddEffect(mVolcanoSmoke);
		}
		if (mTikiTeethSparkle == null)
		{
			mTikiTeethSparkle = mApp.mResourceManager.GetPIEffect("PIEFFECT_MM_SPARKLE").Duplicate();
			mTikiTeethSparkle.mEmitAfterTimeline = true;
			Common.SetFXNumScale(mTikiTeethSparkle, 3f);
			mEffectBatch.AddEffect(mTikiTeethSparkle);
		}
		CreateChangeProfileButton();
		RehupButtons();
	}

	public void CreateChangeProfileButton()
	{
	}

	public override void AddedToManager(WidgetManager mgr)
	{
		base.AddedToManager(mgr);
	}

	public bool ShouldShowUpsellBtn()
	{
		if (mApp.mUserProfile == null || mApp.mTrialType == 0)
		{
			return false;
		}
		return mApp.mUserProfile.mAdvModeVars.mCurrentAdvZone > 2;
	}

	public void CloseUserSelDialog()
	{
	}

	public void RehupButtons()
	{
		GetButton(10);
		mDrawHat = (mDrawFro = (mDrawTuxedo = (mDrawMoustache = false)));
		if (mApp.mUserProfile != null)
		{
			mDrawHat = mApp.mUserProfile.mHeroicModeVars.mHighestZoneBeat >= 6;
			mDrawMoustache = mApp.mUserProfile.mIronFrogStats.mBestTime > 0;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					if (mApp.mUserProfile.mChallengeUnlockState[i, j] == 4)
					{
						num++;
					}
					else if (mApp.mUserProfile.mChallengeUnlockState[i, j] == 5)
					{
						num++;
						num2++;
					}
				}
			}
			mDrawTuxedo = num == 70;
			mDrawFro = num2 == 70;
		}
		ButtonWidget button = GetButton(17);
		if (button != null)
		{
			if (ShouldShowUpsellBtn())
			{
				button.mVisible = true;
				button.mDisabled = false;
			}
			else
			{
				button.mVisible = false;
				button.mDisabled = true;
			}
		}
		if (mChallengeMenu != null)
		{
			mChallengeMenu.RehupChallengeButtons();
		}
	}

	public ButtonWidget GetButton(int id)
	{
		for (int i = 0; i < mButtons.Count; i++)
		{
			if (mButtons[i].mId == id)
			{
				return mButtons[i];
			}
		}
		return null;
	}

	public override void MouseDown(int x, int y, int theClickCount)
	{
		if (mApp.mBambooTransition == null || !mApp.mBambooTransition.IsInProgress())
		{
			mTip = null;
		}
	}

	public void RemoveUpsellButton()
	{
		if (mUpsellBtn == null)
		{
			return;
		}
		for (int i = 0; i < mButtons.Count; i++)
		{
			if (mButtons[i].mId == 17)
			{
				mButtons.Remove(mButtons[i]);
				i--;
			}
		}
		RemoveWidget(mUpsellBtn);
		mApp.SafeDeleteWidget(mUpsellBtn);
		mUpsellBtn = null;
	}

	public void DoChangeUserDialog()
	{
		_ = mUserSelDlg;
	}

	public void RehupUserList()
	{
		RehupButtons();
		if (mUserSelDlg != null)
		{
			_ = mApp.mProfileMgr;
		}
	}

	public override void Update()
	{
		if (mApp.mMapScreen != null && !mApp.mMapScreen.mDirty)
		{
			return;
		}
		if (mApp.IsHardwareBackButtonPressed())
		{
			ProcessHardwareBackButton();
		}
		if (mApp.mCredits != null && MathUtils._geq(mApp.mCredits.mAlpha, 255f))
		{
			return;
		}
		if (mDelayedIFStartState > 0)
		{
			if (mDelayedIFStartState != 2)
			{
				return;
			}
			ButtonWidget buttonWidget = null;
			for (int i = 0; i < mButtons.Count; i++)
			{
				if (mButtons[i].mId == 15)
				{
					buttonWidget = mButtons[i];
					break;
				}
			}
			mApp.mIFLoadingAnimStartCel = ((ExtraSexyButton)buttonWidget).mDownAnimation.GetFrame();
			mApp.StartIronFrogMode();
			return;
		}
		mUpdateCnt++;
		float num = Common._M(10f);
		if (mApp.ShowingLoadingScreen() || (mApp.mUserProfile != null && mApp.mUserProfile.mNewChallengeCupUnlocked) || mFirstTimeAlpha > 0)
		{
			MarkDirty();
		}
		if (mFirstTimeAlpha > 0 && mFirstTimeAlpha < 255)
		{
			mFirstTimeAlpha += Common._M(3);
			if (mFirstTimeAlpha >= 255)
			{
				mFirstTimeAlpha = 255;
				mApp.StartAdvModeFirstTime();
			}
		}
		if (mHeroicSparkle != null)
		{
			mHeroicSparkle.mDrawTransform.LoadIdentity();
			float num2 = GameApp.DownScaleNum(1f);
			mHeroicSparkle.mDrawTransform.Scale(num2, num2);
			mHeroicSparkle.mDrawTransform.Translate(mPts[5].mX + Common._DS(Common._M(180)), mPts[5].mY - Common._DS(Common._M1(-60)));
			mHeroicSparkle.Update();
			if (mHeroicSparkle.mCurNumParticles > 0)
			{
				MarkDirty();
			}
			else if (mHeroicSparkle.mFrameNum > 2f)
			{
				mHeroicSparkle = null;
			}
		}
		if (mIFSparkle != null)
		{
			mIFSparkle.mDrawTransform.LoadIdentity();
			float num3 = GameApp.DownScaleNum(1f);
			mIFSparkle.mDrawTransform.Scale(num3, num3);
			mIFSparkle.mDrawTransform.Scale(Common._M(3f), Common._M1(2f));
			mIFSparkle.mDrawTransform.Translate(Common._DS(Common._M(-30)), Common._DS(Common._M1(900)));
			mIFSparkle.Update();
			if (mIFSparkle.mCurNumParticles > 0)
			{
				MarkDirty();
			}
			else if (mIFSparkle.mFrameNum > 2f)
			{
				mIFSparkle = null;
			}
		}
		if (mChallengeSparkle != null)
		{
			mChallengeSparkle.mDrawTransform.LoadIdentity();
			float num4 = GameApp.DownScaleNum(1f);
			mChallengeSparkle.mDrawTransform.Scale(num4, num4);
			mChallengeSparkle.mDrawTransform.Translate(mPts[1].mX + Common._DS(Common._M(180)) + Common._S(960), mPts[1].mY - Common._DS(Common._M1(-60)));
			mChallengeSparkle.Update();
			if (mChallengeSparkle.mCurNumParticles > 0)
			{
				MarkDirty();
			}
			else if (mChallengeSparkle.mFrameNum > 2f)
			{
				mChallengeSparkle = null;
			}
		}
		if (mIFUnlockAnim != null)
		{
			mIFUnlockAnim.Update();
			if (mIFUnlockAnim.mMainSpriteInst.mFrameNum >= (float)(mIFUnlockAnim.mMainSpriteInst.mDef.mFrames.Count - 1))
			{
				mIFUnlockAnim = null;
				RehupButtons();
			}
		}
		if (mUpdateCnt >= Common._M(50))
		{
			if (gNeedsIFUnlockSound)
			{
				gNeedsIFUnlockSound = false;
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_IRON_FROG_UNLOCKED));
			}
			if (gNeedsOtherModeUnlockSound)
			{
				gNeedsOtherModeUnlockSound = false;
			}
		}
		if (mApp.Is3DAccelerated())
		{
			if (mApp.mHasFocus)
			{
				MarkDirty();
			}
			if (mIncLavaAlpha)
			{
				mLavaAlpha += Common._M(1.4f);
				if (mLavaAlpha >= 255f)
				{
					mLavaAlpha = 255f;
					mIncLavaAlpha = false;
				}
			}
			else
			{
				mLavaAlpha -= Common._M(1.4f);
				if (mLavaAlpha <= 0f)
				{
					mLavaAlpha = 0f;
					mIncLavaAlpha = true;
				}
			}
		}
		if (mMenuScrollPct.IsDoingCurve())
		{
			float num5 = (float)mMenuScrollPct.GetOutVal();
			float num6 = num5 * (float)(mMenuScrollDestY - mMenuScrollStartY);
			mMainMenuButtonsScrollWidget.Resize(mMainMenuButtonsScrollWidget.mX, (int)((float)mMenuScrollStartY + num6), mMainMenuButtonsScrollWidget.mWidth, mMainMenuButtonsScrollWidget.mHeight);
			float num7 = num5 * (float)(mMenuTikiDestX - mMenuTikiStartX);
			mMenuTikiX = (int)((float)mMenuTikiStartX + num7);
			float num8 = num5 * (float)(mMenuFrogDestX - mMenuFrogStartX);
			mMenuFrogX = (int)((float)mMenuFrogStartX + num8);
			float num9 = num5 * (float)(mMenuTikiDudeDestX - mMenuTikiDudeStartX);
			mMenuTikiDudeX = (int)((float)mMenuTikiDudeStartX + num9);
			if (mMainMenuOverlayWidget != null)
			{
				mMainMenuOverlayWidget.UpdateOverlaySlide(num5);
			}
		}
		if (mApp.mMoreGames != null && mApp.mMoreGames.IsReadyForDelete())
		{
			mApp.DeleteMoreGames(delete_resources: false);
		}
		if (mState == MainMenu_State.State_MainMenu && mApp.Is3DAccelerated() && mApp.mHasFocus)
		{
			MarkDirty();
		}
		for (int j = 0; j < mText.Count; j++)
		{
			MMText mMText = mText[j];
			if (mMText.mFadingIn && mMText.mAlpha < 255f)
			{
				MarkDirty();
				mMText.mAlpha += num;
				if (mMText.mAlpha >= 255f)
				{
					mMText.mAlpha = 255f;
				}
			}
			else if (!mMText.mFadingIn)
			{
				MarkDirty();
				mMText.mAlpha -= num;
				if (mMText.mAlpha <= 0f)
				{
					mText.Remove(mMText);
					j--;
				}
			}
		}
		if (mVolcanoSmoke != null)
		{
			mVolcanoSmoke.mDrawTransform.LoadIdentity();
			mVolcanoSmoke.mDrawTransform.Scale(Common._DS(1.4f), Common._DS(1.4f));
			mVolcanoSmoke.mDrawTransform.Translate((float)(Common._S(mX) + Common._DS(Common._M(1440))) + mLavaSmokeXOff, Common._S(mY) + Common._DS(Common._M1(115)));
			mVolcanoSmoke.Update();
		}
		if (mTikiTeethSparkle != null)
		{
			mTikiTeethSparkle.mDrawTransform.LoadIdentity();
			mTikiTeethSparkle.mDrawTransform.Scale(Common._DS(1.4f), Common._DS(1.4f));
			mTikiTeethSparkle.mDrawTransform.Translate((float)(Common._S(mX) + Common._DS(Common._M(165))) + mTeethSparkleXOff, Common._S(mY) + Common._DS(Common._M1(269)));
			mTikiTeethSparkle.Update();
			if (SexyFramework.Common.Rand(2000) == 0 && mTikiTeethSparkle.mCurNumParticles == 0 && mTikiTeethSparkle.mFrameNum >= (float)mTikiTeethSparkle.mLastFrameNum)
			{
				mTikiTeethSparkle.ResetAnim();
				mTikiTeethSparkle.mRandSeeds.Clear();
				mTikiTeethSparkle.mRandSeeds.Add(SexyFramework.Common.Rand(1000));
			}
		}
	}

	public MMText AddText(string txt)
	{
		if (txt.Length <= 0)
		{
			return null;
		}
		MMText mMText = new MMText();
		mMText.mAlpha = 0f;
		mMText.mFadingIn = true;
		mMText.mText = txt;
		mText.Insert(0, mMText);
		FadeOutText(1);
		return mText[0];
	}

	public void FadeOutText(int start)
	{
		for (int i = start; i < mText.Count; i++)
		{
			mText[i].mFadingIn = false;
		}
	}

	public void DrawTalkingBubble(Graphics g, int x, int y, int width, int height)
	{
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(new Color(255, 255, 255, 179));
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_TALK_BUBBLE_TL);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_TALK_BUBBLE_TOP);
		Image imageByID3 = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_TALK_BUBBLE_TAIL);
		Image imageByID4 = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_TALK_BUBBLE_BOT);
		Image imageByID5 = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_TALK_BUBBLE_BL);
		Image imageByID6 = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_TALK_BUBBLE_SIDE);
		g.DrawImage(imageByID, x, y);
		int i = x + imageByID.GetWidth();
		int num = x + width - imageByID.GetWidth();
		g.ClearClipRect();
		g.SetClipRect(i, y, num - i, imageByID2.GetHeight());
		for (; i < num; i += imageByID2.GetWidth())
		{
			g.DrawImage(imageByID2, i, y);
		}
		g.ClearClipRect();
		g.DrawImageMirror(imageByID, num, y);
		g.DrawImage(imageByID3, x - imageByID3.GetWidth() + Common._DS(tailXOff), y + imageByID.GetHeight());
		int j = y + imageByID.GetHeight() + imageByID3.GetHeight();
		int num2 = y + height - imageByID4.GetHeight();
		g.ClearClipRect();
		g.SetClipRect(x, y + imageByID.GetHeight(), width, y + height - imageByID5.GetHeight() - (y + imageByID.GetHeight()));
		for (; j < num2; j += imageByID6.GetHeight())
		{
			g.DrawImage(imageByID6, x, j);
		}
		for (j = y + imageByID.GetHeight(); j < num2; j += imageByID6.GetHeight())
		{
			g.DrawImageMirror(imageByID6, x + width - imageByID6.GetWidth(), j);
		}
		g.ClearClipRect();
		g.DrawImage(imageByID5, x, num2);
		i = x + imageByID5.GetWidth();
		num = x + width - imageByID5.GetWidth();
		g.ClearClipRect();
		g.SetClipRect(i, num2, num - i, imageByID4.GetHeight());
		for (; i < num; i += imageByID4.GetWidth())
		{
			g.DrawImage(imageByID4, i, num2);
		}
		g.ClearClipRect();
		g.DrawImageMirror(imageByID5, num, num2);
		g.SetColorizeImages(colorizeImages: false);
		g.SetColor(new Color(255, 255, 255, 179));
		int num3 = x + imageByID6.GetWidth();
		int num4 = y + imageByID.GetHeight();
		int theWidth = x + width - imageByID6.GetWidth() - num3;
		int theHeight = y + height - imageByID5.GetHeight() - num4;
		g.FillRect(num3, num4, theWidth, theHeight);
		g.FillRect(x + Common._DS(tailXOff), num4, imageByID6.GetWidth() - Common._DS(tailXOff), imageByID3.GetHeight());
	}

	public void LoadTalkingBubbleText()
	{
		mTalkingBubbleTextOptions.Capacity = 45;
		for (int i = 614; i <= 658; i++)
		{
			mTalkingBubbleTextOptions.Add(TextManager.getInstance().getString(i));
		}
	}

	public override void Draw(Graphics g)
	{
		if (mApp.mCredits != null && MathUtils._geq(mApp.mCredits.mAlpha, 255f))
		{
			return;
		}
		if (mChallengeMenu != null)
		{
			_ = mApp.mBambooTransition;
		}
		else
		{
			if (mTikiTemple != null || (mApp != null && mApp.mMapScreen != null))
			{
				return;
			}
			Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_BG);
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_LAVA);
			Image imageByID3 = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_GUY);
			Image imageByID4 = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_SCROLLMENU_RIGHTENDPIECE);
			Image imageByID5 = Res.GetImageByID(ResID.IMAGE_UI_MAINMENU_ENDCAP);
			if (mDelayedIFStartState == 1)
			{
				mDelayedIFStartState = 2;
			}
			_ = (float)imageByID.GetWidth() / (float)mApp.GetScreenRect().mWidth;
			_ = (float)imageByID.GetHeight() / (float)mApp.GetScreenRect().mHeight;
			g.DrawImage(imageByID, 0, 0, mApp.GetScreenRect().mWidth, mApp.GetScreenRect().mHeight);
			g.SetDrawMode(1);
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(new Color(255, 255, 255, (int)mLavaAlpha));
			g.DrawImage(imageByID2, (int)((float)(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAINMENU_LAVA)) - mApp.mWideScreenXOffset + mApp.GetScreenRect().mX) + mLavaXOff), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_LAVA)), (int)((float)imageByID2.GetWidth() * mLavaXScale), imageByID2.GetHeight());
			g.SetColorizeImages(colorizeImages: false);
			g.SetDrawMode(0);
			g.DrawImage(imageByID3, mMenuTikiDudeX, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAINMENU_GUY)));
			if (mMainMenuButtonsWidget != null && mMainMenuButtonsWidget.mVisible)
			{
				if (mMainMenuButtonsWidget.mX >= 0)
				{
					g.DrawImageMirror(imageByID4, mMainMenuButtonsWidget.mX - imageByID4.GetWidth(), mMainMenuButtonsScrollWidget.mY);
					g.DrawImageMirror(imageByID5, mMainMenuButtonsWidget.mX - imageByID4.GetWidth(), mMainMenuButtonsScrollWidget.mY - Common._S(42));
					g.DrawImage(IMAGE_UI_MAINMENU_TIKI, mMainMenuButtonsWidget.mX - IMAGE_UI_MAINMENU_TIKI.GetWidth(), mMainMenuButtonsScrollWidget.mY);
				}
				else if (mMainMenuButtonsWidget.mX + mMainMenuButtonsWidget.mWidth <= mApp.GetScreenWidth())
				{
					g.DrawImage(imageByID4, mMainMenuButtonsWidget.mX + mMainMenuButtonsWidget.mWidth, mMainMenuButtonsScrollWidget.mY);
					g.DrawImage(imageByID5, mMainMenuButtonsWidget.mX + mMainMenuButtonsWidget.mWidth + imageByID4.GetWidth() + mApp.GetScreenRect().mX - imageByID5.GetWidth(), mMainMenuButtonsScrollWidget.mY - Common._S(42));
					g.DrawImageMirror(IMAGE_UI_MAINMENU_TIKI, mMainMenuButtonsWidget.mX + mMainMenuButtonsWidget.mWidth, mMainMenuButtonsScrollWidget.mY);
				}
			}
			Font fontByID = Res.GetFontByID(ResID.FONT_SHAGLOUNGE45_GAUNTLET);
			Font fontByID2 = Res.GetFontByID(ResID.FONT_SHAGEXOTICA38_BLACK);
			g.SetFont(fontByID);
			g.SetColor(Color.White);
			if (GameApp.USE_XBOX_SERVICE)
			{
				string text = "";
				if (mApp.mUserProfile != null)
				{
					StringBuilder stringBuilder = new StringBuilder(TextManager.getInstance().getString(660));
					stringBuilder.Replace("$1", " " + mApp.mUserProfile.GetName());
					text = stringBuilder.ToString();
				}
				else
				{
					text = TextManager.getInstance().getString(659);
				}
				g.WriteString(text, 0, Common._S(Common._M(30)), mWidth);
				g.SetFont(fontByID2);
				DrawChangeProfileString(g);
			}
			if (mState == MainMenu_State.State_Scroll || mState == MainMenu_State.State_MainMenu)
			{
				Common._S(Common._M(375));
				Common._S(Common._M(75));
				Common._S(Common._M(1));
				DrawTikiTalk(g);
				mEffectBatch.DrawBatch(g);
			}
			if (mTip != null)
			{
				mTip.Draw(g);
			}
			DeferOverlay(20);
		}
	}

	public override void DrawOverlay(Graphics g)
	{
		if (mApp.mCredits != null && MathUtils._geq(mApp.mCredits.mAlpha, 255f))
		{
			return;
		}
		if (mChallengeMenu != null || mTikiTemple != null || (mApp != null && mApp.mMapScreen != null))
		{
			return;
		}
		DrawWatermark(g);
	}

	private void DrawWatermark(Graphics g)
	{
		Font fontByID = Res.GetFontByID(ResID.FONT_SHAGLOUNGE28_BASE);
		if (fontByID == null)
		{
			return;
		}
		Rect screenRect = mApp.GetScreenRect();
		int num = Common._DS(Common._M(24));
		int num2 = Common._DS(Common._M1(20));
		int num3 = fontByID.StringWidth(WatermarkText);
		int num4 = Math.Max(screenRect.mX + screenRect.mWidth - num3 - num, screenRect.mX + Common._DS(Common._M(8)));
		int num5 = Math.Max(screenRect.mY + screenRect.mHeight - fontByID.GetHeight() - num2, screenRect.mY + Common._DS(Common._M(8))) + fontByID.GetAscent();
		g.SetFont(fontByID);
		g.SetColor(new Color(255, 255, 255, 184));
		g.WriteString(WatermarkText, num4, num5);
	}

	public void DrawTikiTalk(Graphics g)
	{
		if (mApp.mMoreGames == null && mText.Count != 0)
		{
			Font fontByID = Res.GetFontByID(ResID.FONT_SHAGLOUNGE45_BASE);
			string text = mText[0].mText;
			int num = Common._DS(800);
			int num2 = Common._DS(100);
			int num3 = (int)((float)(mApp.GetScreenRect().mWidth - num) * 0.5f);
			int num4 = Common._DS(75);
			int num5 = Common._DS(15);
			int num6 = num - num5 * 2;
			int num7 = num2 - num5 * 2;
			int num8 = Common._GetWordWrappedHeight(text, fontByID, num6);
			if (num8 > num7)
			{
				int num9 = num8 - num7;
				num7 += num9;
				num2 += num9;
			}
			Rect theRect = new Rect(num3 + num5, num4 + num5, num6, num7);
			theRect.mY += (int)((float)(num7 - num8) * 0.5f);
			DrawTalkingBubble(g, num3, num4, num, num2 + 10);
			g.SetFont(fontByID);
			g.SetColor(Color.Black);
			g.WriteWordWrapped(theRect, text, -1, 0);
		}
	}

	public void DrawChangeProfileString(Graphics g)
	{
	}

	public void SelectUser(string user_name)
	{
	}

	public void HideChallengeMenu()
	{
		RemoveWidget(mChallengeMenu);
		mApp.SafeDeleteWidget(mChallengeMenu);
		mChallengeMenu = null;
		mState = MainMenu_State.State_MainMenu;
		ShowScrollButtons();
	}

	public void ShowTikiTemple()
	{
		mTikiTemple = new TikiTemple();
		mTikiTemple.Resize(mApp.GetScreenRect());
		mTikiTemple.Init();
		mWidgetManager.AddWidget(mTikiTemple);
		if (mMainMenuButtonsWidget != null)
		{
			mMainMenuButtonsWidget.SetVisible(isVisible: false);
		}
		mState = MainMenu_State.State_TikiTemple;
	}

	public void ShowAchievements()
	{
		mAchievements = new Achievements();
		mAchievements.Resize(mApp.GetScreenRect());
		mAchievements.Init();
		mWidgetManager.AddWidget(mAchievements);
		if (mMainMenuButtonsWidget != null)
		{
			mMainMenuButtonsWidget.SetVisible(isVisible: false);
		}
		mState = MainMenu_State.State_Achievement;
	}

	public void ShowLeaderBoards()
	{
		mLeaderBoards = new LeaderBoards();
		mLeaderBoards.Resize(mApp.GetScreenRect());
		mLeaderBoards.Init();
		mWidgetManager.AddWidget(mLeaderBoards);
		if (mMainMenuButtonsWidget != null)
		{
			mMainMenuButtonsWidget.SetVisible(isVisible: false);
		}
		mState = MainMenu_State.State_LeaderBoards;
	}

	public void HideTikiTemple()
	{
		mWidgetManager.RemoveWidget(mTikiTemple);
		mApp.SafeDeleteWidget(mTikiTemple);
		mTikiTemple = null;
		if (mMainMenuButtonsWidget != null)
		{
			mMainMenuButtonsWidget.SetVisible(isVisible: true);
		}
		mState = MainMenu_State.State_MainMenu;
	}

	public void HideLeaderBoards()
	{
		mWidgetManager.RemoveWidget(mLeaderBoards);
		mApp.SafeDeleteWidget(mLeaderBoards);
		mLeaderBoards = null;
		if (mMainMenuButtonsWidget != null)
		{
			mMainMenuButtonsWidget.SetVisible(isVisible: true);
		}
		mState = MainMenu_State.State_MainMenu;
	}

	public void HideAchievements()
	{
		mWidgetManager.RemoveWidget(mAchievements);
		RemoveWidget(mAchievements);
		mApp.SafeDeleteWidget(mAchievements);
		mAchievements = null;
		if (mMainMenuButtonsWidget != null)
		{
			mMainMenuButtonsWidget.SetVisible(isVisible: true);
		}
		mState = MainMenu_State.State_MainMenu;
	}

	public void ShowChallengeMenuFromMainMenu()
	{
		mApp.LoadAllThumbnails();
		mChallengeMenu = new ChallengeMenu(mApp, this, fromMainMenu: true);
		mChallengeMenu.Resize(mApp.GetScreenRect().mX, mApp.GetScreenRect().mY, mApp.GetScreenRect().mWidth - mApp.GetScreenRect().mX, mApp.GetScreenRect().mHeight - mApp.GetScreenRect().mY);
		mChallengeMenu.Init();
		AddWidget(mChallengeMenu);
		mChallengeMenu.InitCS();
		RehupButtons();
		mChallengeMenu.mCSVisFrame = mUpdateCnt;
		mState = MainMenu_State.State_CS;
		HideScrollButtons();
	}

	public void ShowChallengeMenu()
	{
		mChallengeMenu = new ChallengeMenu(mApp, this, fromMainMenu: false);
		mChallengeMenu.Resize(mApp.GetScreenRect().mX, mApp.GetScreenRect().mY, mApp.GetScreenRect().mWidth - mApp.GetScreenRect().mX, mApp.GetScreenRect().mHeight - mApp.GetScreenRect().mY);
		mChallengeMenu.Init();
		AddWidget(mChallengeMenu);
		mChallengeMenu.InitCS();
		RehupButtons();
		mChallengeMenu.mCSVisFrame = mUpdateCnt;
		mState = MainMenu_State.State_CS;
	}

	public void HideScrollButtons()
	{
		mMainMenuButtonsWidget.HideScrollButtons();
	}

	public void ShowScrollButtons()
	{
		mMainMenuButtonsWidget.ShowScrollButtons();
	}

	public bool ShowingTikiTemple()
	{
		return mTikiTemple != null;
	}

	public void ProcessHardwareBackButton()
	{
		if (mApp.mMapScreen != null)
		{
			return;
		}
		Dialog dialog = mApp.GetDialog(2);
		if (dialog != null)
		{
			(dialog as OptionsDialog).ProcessHardwareBackButton();
			return;
		}
		if (GameApp.gApp.mAboutInfo != null)
		{
			GameApp.gApp.mAboutInfo.ProcessHardwareBackButton();
			return;
		}
		if (GameApp.gApp.mLegalInfo != null)
		{
			GameApp.gApp.mLegalInfo.ProcessHardwareBackButton();
			return;
		}
		if (GameApp.gApp.mLegalInfo != null)
		{
			GameApp.gApp.mLegalInfo.ProcessHardwareBackButton();
			return;
		}
		if (GameApp.gApp.mBambooTransition != null && GameApp.gApp.mBambooTransition.IsInProgress())
		{
			mApp.OnHardwareBackButtonPressProcessed();
			return;
		}
		switch (mState)
		{
		case MainMenu_State.State_CS:
			if (mChallengeMenu.ProcessHardwareBackButton())
			{
				mState = MainMenu_State.State_MainMenu;
			}
			return;
		case MainMenu_State.State_TikiTemple:
			mState = MainMenu_State.State_MainMenu;
			mTikiTemple.ProcessHardwareBackButton();
			return;
		case MainMenu_State.State_Achievement:
			if (mAchievements.ProcessHardwareBackButton())
			{
				mState = MainMenu_State.State_MainMenu;
			}
			return;
		case MainMenu_State.State_LeaderBoards:
			if (mLeaderBoards.ProcessHardwareBackButton())
			{
				mState = MainMenu_State.State_MainMenu;
			}
			return;
		case MainMenu_State.State_MapScreen:
			mState = MainMenu_State.State_MainMenu;
			mApp.OnHardwareBackButtonPressProcessed();
			return;
		case MainMenu_State.State_QuitPrompt:
			mState = MainMenu_State.State_MainMenu;
			mApp.GetDialog(1).ButtonDepress(1001);
			mApp.OnHardwareBackButtonPressProcessed();
			return;
		case MainMenu_State.State_UnlockPrompt:
			mState = MainMenu_State.State_MainMenu;
			mApp.GetDialog(0).ButtonDepress(1000);
			mApp.OnHardwareBackButtonPressProcessed();
			return;
		}
		if (GameApp.gApp.mGenericHelp != null)
		{
			mState = MainMenu_State.State_MainMenu;
			GameApp.gApp.mGenericHelp.ForceCloseDialog();
			mApp.OnHardwareBackButtonPressProcessed();
		}
		else
		{
			mState = MainMenu_State.State_QuitPrompt;
			mApp.DoQuitPromptDialog();
			mApp.mYesNoDialogDelegate = ProcessYesNo;
			mApp.OnHardwareBackButtonPressProcessed();
		}
	}

	public void ProcessYesNo(int theId)
	{
		mState = MainMenu_State.State_MainMenu;
		if (theId == 1000)
		{
			if (!mApp.IsRegistered() && mApp.mTrialType == 1 && mApp.GetBoolean("UpsellExit", theDefault: false))
			{
				mApp.DoUpsell(from_exit: true);
			}
			else
			{
				mApp.SaveProfile();
			}
			mApp.Shutdown();
		}
	}

	public void StartChallengeGame()
	{
		mCSOverRect = default(Rect);
		mApp.StartGauntletMode(mGauntletModLevel_id, mCSOverRect);
	}
}
