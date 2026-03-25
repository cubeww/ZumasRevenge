using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using JeffLib;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using SexyFramework;
using SexyFramework.AELib;
using SexyFramework.Drivers.App;
using SexyFramework.Drivers.File;
using SexyFramework.Drivers.Profile;
using SexyFramework.File;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;
using ZumasRevenge.Profile;
using ZumasRevenge.Sound;

namespace ZumasRevenge;

public class GameApp : SexyApp, NewUserDialogListener, ProfileEventListener
{
	public enum Metrics_AppEventType
	{
		Metrics_AppEvent_StartNormal = 1,
		Metrics_AppEvent_StartUpgrade,
		Metrics_AppEvent_StartInstall,
		Metrics_AppEvent_MovedToForeground,
		Metrics_AppEvent_StartFromPushNotification
	}

	public delegate void PreBlockCallback();

	public delegate void YesNoDialogDelegate(int buttonId);

	public enum EXLiveWaiting
	{
		E_NONE,
		E_WaitingForSignIn,
		E_WaitingForAchivements,
		E_Ready
	}

	public static bool USE_TRIAL_VERSION = false;

	public static bool NONE_XBOX_LIVE = false;

	public static bool UN_UPDATE_VERSION = false;

	public static bool USE_XBOX_SERVICE = true;

	public WebBrowserTask mWbt;

	public bool mWaitForActive;

	public bool mInitFinished;

	public static bool mDisplayTitleUpdateMessage = false;

	public static bool mExit = false;

	public Image mBackgroundLayer;

	public Thread mInitThread;

	public static GameApp gApp = null;

	public static DDS gDDS = null;

	public static int gSaveGameVersion = 197;

	public static int gNumOptionalGroups = 8;

	public static string[] gOptionalGroups = new string[8] { "CommonBoss", "Boss1", "Boss2", "Boss3", "Boss4", "Boss5", "Boss6Common", "GrottoSounds" };

	public static string gOrgTitle = "";

	public static bool gDidCrashHandler = false;

	public static string gMetricsVersion = "1.0";

	public static int gScreenShakeX = 0;

	public static int gScreenShakeY = 0;

	public static int gLastLevel = 0;

	public static int gLastZone = -1;

	public static bool gNeedsPreCache = true;

	private static int InGameLoadThread_DrawFunc_CallCounter = 0;

	private static int gAddingDlgID = -12345;

	public static bool gInitialProfLoadSuccessful;

	private static string[] gInitialLoadGroups = new string[5] { "Init", "CommonGame", "GamePlay", "MenuRelated", null };

	private PreBlockCallback mDialogCallBack;

	public Game mGameMain;

	public AutoMonkey mAutoMonkey;

	public bool mSavingOrLoadingProfile;

	public float mShotCorrectionAngleToWidthDist;

	public float mShotCorrectionAngleMax;

	public float mShotCorrectionWidthMax;

	public int mGuideStyle;

	public int mShotCorrectionDebugStyle;

	public bool mIronFrogModeIncluded;

	public Board mBoard;

	public LoadingScreen mLoadingScreen;

	public LevelMgr mNormalLevelMgr;

	public Dictionary<string, List<GenericCachedEffect>> mCachedPIEffects = new Dictionary<string, List<GenericCachedEffect>>();

	public MapScreenHackWidget mMapScreenHackWidget;

	public Rect mLoadRect = default(Rect);

	public ZumaDialog mReturnToMMDlg;

	public Dictionary<int, DialogHideInfo> mDialogHideInfoMap;

	public bool mDoingDRM;

	public int mTrialType;

	public int mFramesPlayed;

	public int mCachedLoadState;

	public bool mCachedLoad;

	public bool mInitialLoad;

	public bool mDelayIntro;

	public int mWideScreenXOffset;

	public long mLastMoreGamesUpdate;

	public int mIFLoadingAnimStartCel;

	public Upsell mUpsell;

	public List<CachedTorchEffect> mCachedTorchEffects = new List<CachedTorchEffect>();

	public List<CachedVolcanoEffect> mCachedVolcanoEffects = new List<CachedVolcanoEffect>();

	public Dictionary<string, PIEffect> mPIEffectMap = new Dictionary<string, PIEffect>();

	public bool mClickedHardMode;

	public bool mInGameLoadThreadProcFailed;

	public bool mStartInGameModeThreadProcRunning;

	public bool mContinuedGame;

	public int mForceZoneRestart;

	public string mChallengeLevelId = "";

	public UnderDialogWidget mUnderDialogWidget;

	public float mDialogObscurePct;

	public Dictionary<string, CompositionMgr> mPreloadedComps = new Dictionary<string, CompositionMgr>();

	public Credits mCredits;

	public CreditsHackWidget gCreditsHackWidget;

	public GenericHelp mGenericHelp;

	public MapScreen mMapScreen;

	public List<IdxThumbPair> mLevelThumbnails = new List<IdxThumbPair>();

	public ProxBombManager mProxBombManager;

	public Music mMusic;

	public SoundEffects mSoundPlayer;

	public ZumaProfile mUserProfile;

	public ZumaProfileMgr mProfileMgr;

	public MainMenu mMainMenu;

	public MoreGames mMoreGames;

	public NewUserDialog mNewUserDlg;

	public string mLevelXML;

	public string mHardLevelXML;

	public static string mCompositionResPrefix;

	public bool mHiRes;

	public static int mGameRes;

	public static float mGameDownScale;

	public static float mGameUpScale;

	public static float mGameScreenScale;

	public bool mReInit;

	public bool mFromReInit;

	public bool mDoingAdvModeLoad;

	public int mConfTime;

	public int mLoadType;

	public bool mColorblind;

	public bool mCursorTarget;

	public string mTimeStamp;

	public BambooTransition mBambooTransition;

	public string m_DefaultProfileName = "Player 1";

	public string m_DefaultName = "Player 1";

	public LegalInfo mLegalInfo;

	public AboutInfo mAboutInfo;

	public WidescreenBoardWidget mWidescreenBoardWidget;

	public ZumasRevenge.Profile.Profile m_Profile = new ZumasRevenge.Profile.Profile();

	public YesNoDialogDelegate mYesNoDialogDelegate;

	public ZumaDialog mDialog;

	public int mTouchOffsetX;

	public int mTouchOffsetY;

	private Thread mLoadingThread;

	private ThreadStart mLoadingProc;

	public bool mLoadLevelSuccess;

	public bool StartLoadingComplete;

	public int mBoardOffsetX = 85;

	public int mBoardUIOffsetX = 53;

	public int mOffset160X = 160;

	protected EXLiveWaiting m_XLiveState = EXLiveWaiting.E_WaitingForSignIn;

	protected void PreShowLoadingScreen()
	{
		if (!mResourceManager.IsGroupLoaded("LoadScreen"))
		{
			mResourceManager.LoadResources("LoadScreen");
		}
		mResourceManager.LoadImage("ATLASIMAGE_ATLAS_GAMEPLAY_640_00");
		mResourceManager.LoadImage("ATLASIMAGE_ATLAS_MENURELATED_640_00");
		mMusic.LoadMusic(1, "music/MUSIC_LOADING");
		mMusic.LoadMusic(0, "music/MUSIC_HAWAIIAN");
		mMusic.Enable(inEnable: true);
		StartLoading();
	}

	public void ShowLoadingScreen()
	{
		mLoadingScreen = new LoadingScreen();
		mLoadingScreen.Resize(0, 0, mWidth, mHeight);
		mWidgetManager.AddWidget(mLoadingScreen);
		if (mMusic.IsUserMusicPlaying())
		{
			mLoadingScreen.ProcessBGM();
		}
		mUnderDialogWidget.CreateImages();
		mUnderDialogWidget.Resize(0, 0, mWidth, mHeight);
		mWidgetManager.AddWidget(mUnderDialogWidget);
		mUnderDialogWidget.SetVisible(isVisible: false);
	}

	protected void LoadingScreenCallback()
	{
		mWidgetManager.BringToFront(mLoadingScreen);
	}

	protected void SetupMainMenuDefaults(bool do_load_thread)
	{
		if (do_load_thread)
		{
			mLoadType = 4;
			mStartInGameModeThreadProcRunning = true;
			int num = Common._DS(Common._M(700));
			int num2 = Common._DS(Common._M(650));
			Ratio aspectRatio = mGraphicsDriver.GetAspectRatio();
			if (aspectRatio.mNumerator != 4 && aspectRatio.mDenominator != 3)
			{
				Common._DS(Common._M(160));
			}
			StartMMThreadProc();
			DoCommonInGameLoadThread(new Rect((mWidth - num) / 2, (mHeight - num2) / 2, num, num2));
			mReturnToMMDlg = null;
		}
		else
		{
			StartMMThreadProc();
		}
		mWidgetManager.AddWidget(mMainMenu);
		ClearUpdateBacklog(relaxForASecond: true);
	}

	protected void SetupMainMenuDefaults()
	{
		SetupMainMenuDefaults(do_load_thread: true);
	}

	protected void DoCommonInGameLoadThread(Rect aRect)
	{
		mLoadRect = aRect;
		bool updated = false;
		while (mStartInGameModeThreadProcRunning)
		{
			UpdateAppStep(ref updated);
			SexyFramework.Common.SexySleep(0);
		}
		mWidgetManager.MarkAllDirty();
		if (mInGameLoadThreadProcFailed)
		{
			Popup("There was an error initializing the game.");
			mBoard.Dispose();
			mBoard = null;
			for (int i = 0; i < mNormalLevelMgr.mLevels.size(); i++)
			{
				mNormalLevelMgr.mLevels[i].mBoard = null;
			}
			if (mWidescreenBoardWidget != null)
			{
				mWidgetManager.RemoveWidget(mWidescreenBoardWidget);
				SafeDeleteWidget(mWidescreenBoardWidget);
				mWidescreenBoardWidget = null;
			}
			Shutdown();
		}
		else if (mLoadType != 4)
		{
			mWidgetManager.AddWidget(mBoard);
			mWidgetManager.SetFocus(mBoard);
			if (mWidescreenBoardWidget == null)
			{
				mWidescreenBoardWidget = new WidescreenBoardWidget();
				mWidescreenBoardWidget.Resize(Common._S(-80), 0, mWidth + Common._S(160), mHeight);
				mWidgetManager.AddWidget(mWidescreenBoardWidget);
			}
		}
	}

	public GameApp(Game xnaGame, bool from_reinit)
	{
		gApp = this;
		mGameMain = xnaGame;
		((WP7AppDriver)mAppDriver).InitXNADriver(xnaGame);
		SetBoolean("drivers.ios.use_gles20", boolValue: true);
		SetBoolean("drivers.ios.use_multitouch", boolValue: false);
		SetInteger("compat_AppOrigScreenWidth", mOrigScreenWidth);
		SetInteger("compat_AppOrigScreenHeight", mOrigScreenHeight);
		mSavingOrLoadingProfile = false;
		mWideScreenXOffset = 0;
		mUpsell = null;
		mDoingDRM = false;
		mTrialType = 0;
		mShotCorrectionAngleToWidthDist = 1500f;
		mShotCorrectionAngleMax = 13f;
		mShotCorrectionWidthMax = 65f;
		mGuideStyle = 1;
		mShotCorrectionDebugStyle = 3;
		mIronFrogModeIncluded = false;
		mGenericHelp = null;
		mLegalInfo = null;
		mAboutInfo = null;
		mProdName = "ZumasRevenge";
		mRegKey = "PopCap\\ZumasRevenge";
		mLevelXML = "levels/levels";
		mHardLevelXML = "levels/levels_hard";
		mBoard = null;
		mDebugKeysEnabled = false;
		mAllowSwapScreenImage = false;
		mLoadType = -1;
		mCredits = null;
		mIFLoadingAnimStartCel = 0;
		mDelayIntro = false;
		mReturnToMMDlg = null;
		mDoingAdvModeLoad = false;
		mConfTime = 1500;
		mGameRes = 640;
		mHiRes = false;
		mWidescreenAware = true;
		mWidescreenTranslate = true;
		mAllowWindowResize = true;
		mReInit = false;
		mFromReInit = from_reinit;
		mMapScreen = null;
		mMapScreenHackWidget = null;
		mInGameLoadThreadProcFailed = false;
		mForceZoneRestart = -1;
		mStartInGameModeThreadProcRunning = false;
		mClickedHardMode = false;
		mContinuedGame = false;
		gNeedsPreCache = true;
		mAutoMonkey = null;
		initResolution(640);
		mAutoStartLoadingThread = false;
		mLoadingScreen = null;
		mFramesPlayed = 0;
		mAutoEnable3D = true;
		mNoVSync = true;
		mCachedLoadState = 0;
		mCachedLoad = false;
		mNormalLevelMgr = null;
		mCustomCursorsEnabled = true;
		mCursorTarget = true;
		mColorblind = false;
		mUserProfile = null;
		mProfileMgr = null;
		mMainMenu = null;
		mMoreGames = null;
		mNewUserDlg = null;
		mUnderDialogWidget = new UnderDialogWidget();
		mDialogObscurePct = 0f;
		mFullscreenBits = 32;
		mInitialLoad = true;
		gDDS = new DDS();
		gDDS.mMinLevel = int.MaxValue;
		mBambooTransition = null;
		mProductVersion = GetProductVersion("");
		if (!mFileDriver.InitFileDriver(this))
		{
			Shutdown();
		}
	}

	public string GetProductVersion(string thePath)
	{
		string fullName = Assembly.GetCallingAssembly().FullName;
		string text = "v" + fullName.Split('=')[1].Split(',')[0];
		return text.Substring(0, text.Length - 2);
	}

	public override void Dispose()
	{
		if (mSoundManager != null)
		{
			mSoundManager.ReleaseChannels();
			mSoundManager.ReleaseSounds();
		}
		if (mGenericHelp != null)
		{
			KillDialog(mGenericHelp);
			mGenericHelp = null;
		}
		if (mLegalInfo != null)
		{
			KillDialog(mLegalInfo);
			mLegalInfo = null;
		}
		if (mAboutInfo != null)
		{
			KillDialog(mAboutInfo);
			mAboutInfo = null;
		}
		if (mBambooTransition != null)
		{
			mWidgetManager.RemoveWidget(mBambooTransition);
			mBambooTransition = null;
		}
		if (mUpsell != null)
		{
			mWidgetManager.RemoveWidget(mUpsell);
			mUpsell = null;
		}
		if (gCreditsHackWidget != null)
		{
			mWidgetManager.RemoveWidget(gCreditsHackWidget);
		}
		gCreditsHackWidget = null;
		mWidgetManager.RemoveWidget(mUnderDialogWidget);
		mUnderDialogWidget = null;
		mCredits = null;
		Ball.DeleteBallGlobals();
		if (mBoard != null)
		{
			if (mBoard.NeedSaveGame() && mUserProfile != null)
			{
				mBoard.SaveGame(mUserProfile.GetSaveGameName(IsHardMode()), null);
			}
			mWidgetManager.RemoveWidget(mBoard);
		}
		mReturnToMMDlg = null;
		mProxBombManager = null;
		mLevelThumbnails.Clear();
		mMusic = null;
		mSoundPlayer = null;
		mBoard = null;
		if (mNormalLevelMgr != null)
		{
			for (int i = 0; i < mNormalLevelMgr.mLevels.size(); i++)
			{
				mNormalLevelMgr.mLevels[i].mBoard = null;
			}
		}
		if (mMapScreen != null)
		{
			mMapScreen.CleanButtons();
		}
		if (mMapScreenHackWidget != null)
		{
			mWidgetManager.RemoveWidget(mMapScreenHackWidget);
		}
		mMapScreenHackWidget = null;
		mMapScreen = null;
		if (mMainMenu != null)
		{
			mWidgetManager.RemoveWidget(mMainMenu);
		}
		mMainMenu = null;
		if (mMoreGames != null)
		{
			mWidgetManager.RemoveWidget(mMoreGames);
		}
		mMoreGames = null;
		if (mLoadingScreen != null)
		{
			mWidgetManager.RemoveWidget(mLoadingScreen);
		}
		mLoadingScreen = null;
		mNormalLevelMgr = null;
		if (mNewUserDlg != null)
		{
			KillDialog(mNewUserDlg.mId, removeWidget: true, deleteWidget: false);
		}
		mNewUserDlg = null;
		gDDS = null;
		for (int j = 0; j < mCachedTorchEffects.size(); j++)
		{
			mCachedTorchEffects[j].mTorchFlame = null;
			mCachedTorchEffects[j].mTorchFlameOut = null;
		}
		for (int k = 0; k < mCachedVolcanoEffects.size(); k++)
		{
			mCachedVolcanoEffects[k].mExplosion = null;
			mCachedVolcanoEffects[k].mProjectile = null;
		}
		mResourceManager.DeleteResources("");
		mProfileMgr = null;
		RegistryWriteBoolean("LastShutdownOK", theValue: true);
	}

	public bool IsWideScreen()
	{
		return true;
	}

	public int GetWideScreenAdjusted(int x)
	{
		return x;
	}

	public int GetWidthAdjusted(int x)
	{
		return x - Common._DS(125);
	}

	public bool LoadMoreGamesInfo()
	{
		SexyFramework.Misc.Buffer theBuffer = new SexyFramework.Misc.Buffer();
		if (ReadBufferFromFile(SexyFramework.Common.GetAppDataFolder() + "users/mg.dat", ref theBuffer))
		{
			mLastMoreGamesUpdate = theBuffer.ReadLong();
			return true;
		}
		return false;
	}

	public void SaveMoreGamesInfo()
	{
	}

	public void ConsoleCallback(string cmd, List<string> _params)
	{
	}

	public void SaveProfile()
	{
		if (!mSavingOrLoadingProfile && mUserProfile != null)
		{
			mSavingOrLoadingProfile = true;
			mUserProfile.SaveDetails();
			mSavingOrLoadingProfile = false;
		}
	}

	public bool HasSaveGame()
	{
		if (mUserProfile == null)
		{
			return false;
		}
		string saveGameName = mUserProfile.GetSaveGameName(IsHardMode());
		if (StorageFile.FileExists(saveGameName))
		{
			return true;
		}
		return false;
	}

	public void HandleCrash(bool from_assert)
	{
	}

	public void SaveGlobalConfig()
	{
		SexyFramework.Misc.Buffer buffer = new SexyFramework.Misc.Buffer();
		buffer.WriteDouble(mMusicVolume);
		buffer.WriteDouble(mSfxVolume);
		buffer.WriteBoolean(mColorblind);
		StorageFile.WriteBufferToFile("users/OptionConfig.sav", buffer);
	}

	public void LoadGlobalConfig()
	{
		SexyFramework.Misc.Buffer buffer = new SexyFramework.Misc.Buffer();
		if (StorageFile.ReadBufferFromFile("users/OptionConfig.sav", buffer))
		{
			mMusicVolume = buffer.ReadDouble();
			mSfxVolume = buffer.ReadDouble();
			mColorblind = buffer.ReadBoolean();
			SetMusicVolume(mMusicVolume);
			SetSfxVolume(mSfxVolume);
		}
	}

	public void RevertOptionsChanges()
	{
		bool theValue = false;
		bool theValue2 = false;
		bool theValue3 = false;
		RegistryReadBoolean("PreHiRes", ref theValue);
		RegistryReadBoolean("Pre3D", ref theValue2);
		RegistryReadBoolean("PreWindowed", ref theValue3);
		RegistryWriteBoolean("NeedsConfirmation", theValue: false);
		SwitchScreenMode(theValue3, theValue2, force: true);
		mPreferredWidth = (mPreferredHeight = -1);
		RegistryWriteBoolean("HiRes", theValue);
		mReInit = true;
		Shutdown();
	}

	public void InGameLoadThread_DrawFunc()
	{
		InGameLoadThread_DrawFunc_CallCounter++;
		Font fontByName = GetFontByName("FONT_SHAGLOUNGE38_STROKE");
		Image imageByName = GetImageByName("IMAGE_BLUE_BALL");
		Graphics graphics = new Graphics(mWidgetManager.mImage);
		string text = ((mLoadType == 4) ? TextManager.getInstance().getString(726) : TextManager.getInstance().getString(581));
		string text2 = "";
		int num = InGameLoadThread_DrawFunc_CallCounter % 40;
		if (num >= 30)
		{
			text2 = "...";
		}
		else if (num >= 20)
		{
			text2 = "..";
		}
		else if (num >= 10)
		{
			text2 = ".";
		}
		text += text2;
		if (mLoadType == 1 || mLoadType == 0)
		{
			Ratio aspectRatio = mGraphicsDriver.GetAspectRatio();
			int num2 = ((aspectRatio.mNumerator != 4 && aspectRatio.mDenominator != 3) ? Common._DS(Common._M(160)) : 0);
			if (mLoadType == 1)
			{
				int num3 = ((aspectRatio.mNumerator != 4 && aspectRatio.mDenominator != 3) ? Common._DS(Common._M(80)) : 0);
				int num4 = mLoadRect.mX + (mLoadRect.mWidth - fontByName.StringWidth("Loading...")) / 2;
				num4 += num3;
				int theY = mLoadRect.mY + (mLoadRect.mHeight - fontByName.mHeight) / 2 + Common._DS(Common._M(50));
				graphics.SetColor(250, 124, 0);
				graphics.SetFont(fontByName);
				graphics.DrawString(text, num4, theY);
			}
			else if (mLoadType == 0)
			{
				int num4 = Common._DS(Common._M(656)) + (Common._DS(Common._M1(330)) - fontByName.StringWidth("Loading...")) / 2 - 2;
				num4 += num2;
				int theY = Common._DS(Common._M(697)) + (Common._DS(Common._M1(500)) - fontByName.mHeight) / 2 - 2;
				graphics.SetColor(SexyFramework.Graphics.Color.White);
				graphics.SetFont(fontByName);
				graphics.DrawString(text, num4 + 2, theY + graphics.GetFont().GetAscent() + 2);
			}
		}
		else if (mLoadType == 2)
		{
			graphics.SetFont(fontByName);
			graphics.SetColor(250, 124, 0);
			graphics.DrawString(text, mLoadRect.mX + (mLoadRect.mWidth - graphics.GetFont().StringWidth("Loading...")) / 2, mLoadRect.mY + (mLoadRect.mHeight - graphics.GetFont().mHeight) / 2 + graphics.GetFont().GetAscent());
		}
		else if (mLoadType == 3)
		{
			graphics.SetFont(fontByName);
			graphics.SetColor(SexyFramework.Graphics.Color.White);
		}
		else
		{
			if (mLoadType != 4)
			{
				return;
			}
			graphics.Translate(mReturnToMMDlg.mX, mReturnToMMDlg.mY);
			mReturnToMMDlg.Draw(graphics);
			graphics.SetFont(fontByName);
			graphics.SetColor(250, 124, 0);
			graphics.DrawString(text, (mLoadRect.mWidth - graphics.GetFont().StringWidth("Returning to Menu...")) / 2 + Common._DS(Common._M(20)), (mLoadRect.mHeight - graphics.GetFont().mHeight) / 2 - Common._DS(Common._M1(30)) + graphics.GetFont().GetAscent());
			int theY2 = Common._DS(Common._M(400));
			int num5 = mLoadRect.mWidth - imageByName.GetCelWidth() * 4 - Common._DS(Common._M(-100));
			int num6 = num5 / 4;
			Image[] array = new Image[4]
			{
				GetImageByName("IMAGE_BLUE_BALL"),
				GetImageByName("IMAGE_RED_BALL"),
				GetImageByName("IMAGE_YELLOW_BALL"),
				GetImageByName("IMAGE_GREEN_BALL")
			};
			int[] array2 = new int[4]
			{
				SexyFramework.Common.Rand(50),
				SexyFramework.Common.Rand(50),
				SexyFramework.Common.Rand(50),
				SexyFramework.Common.Rand(50)
			};
			for (int i = 0; i < 4; i++)
			{
				int num7 = array[i].mNumCols * array[i].mNumRows;
				int num8 = (array2[i] + InGameLoadThread_DrawFunc_CallCounter) % num7;
				if (num8 < 0)
				{
					num8 = -num8;
				}
				else if (num8 >= num7)
				{
					num8 = num7 - 1;
				}
				graphics.DrawImageRotated(theSrcRect: new Rect(array[i].GetCelRect(num8)), theImage: array[i], theX: num6 + num5 / 4 * i, theY: theY2, theRot: -1.5707963705062866);
			}
		}
	}

	public void StartAdvModeThreadProc()
	{
		mBoard = new Board(this, -1);
		mBoard.mAdventureMode = true;
		mBoard.mIsHardMode = mClickedHardMode;
		if (!mBoard.Init())
		{
			mInGameLoadThreadProcFailed = true;
			mStartInGameModeThreadProcRunning = false;
			return;
		}
		mBoard.Resize(0, 0, mWidth, mHeight);
		mContinuedGame = false;
		if (HasSaveGame() && mForceZoneRestart == -1)
		{
			if (!mBoard.LoadGame(mUserProfile.GetSaveGameName(IsHardMode())))
			{
				StorageFile.DeleteFile(mUserProfile.GetSaveGameName(IsHardMode()));
				mUserProfile.ClearAdventureModeDetails();
			}
			else
			{
				mContinuedGame = true;
			}
		}
		else
		{
			PlaySong(12);
			if (mForceZoneRestart != -1)
			{
				mBoard.RestartFromZone(mForceZoneRestart);
			}
			else if (!mBoard.StartLevel(1))
			{
				mInGameLoadThreadProcFailed = true;
				mStartInGameModeThreadProcRunning = false;
				return;
			}
		}
		mBoard.MakeCachedBackground();
		mInGameLoadThreadProcFailed = false;
		mForceZoneRestart = -1;
		mStartInGameModeThreadProcRunning = false;
		mAutoMonkey.SetState(MonkeyState.Playing);
	}

	public void StartChallengeModeThreadProc()
	{
		mBoard = new Board(this, mNormalLevelMgr.GetStartingGauntletLevel(mChallengeLevelId));
		if (!mBoard.Init())
		{
			mInGameLoadThreadProcFailed = true;
			mStartInGameModeThreadProcRunning = false;
			return;
		}
		mBoard.Resize(0, 0, mWidth, mHeight);
		if (!mBoard.StartLevel(mChallengeLevelId))
		{
			mInGameLoadThreadProcFailed = true;
			mStartInGameModeThreadProcRunning = false;
		}
		else
		{
			mInGameLoadThreadProcFailed = false;
			mStartInGameModeThreadProcRunning = false;
			mAutoMonkey.SetState(MonkeyState.Playing);
		}
	}

	public void StartIronFrogModeThreadProc()
	{
		mBoard = new Board(this, -1);
		if (!mBoard.Init())
		{
			mInGameLoadThreadProcFailed = true;
			mStartInGameModeThreadProcRunning = false;
			return;
		}
		mBoard.Resize(0, 0, mWidth, mHeight);
		if (!mBoard.StartLevel(mNormalLevelMgr.GetFirstIronFrogLevel() + 1))
		{
			mInGameLoadThreadProcFailed = true;
			mStartInGameModeThreadProcRunning = false;
		}
		else
		{
			mInGameLoadThreadProcFailed = false;
			mStartInGameModeThreadProcRunning = false;
		}
	}

	public void StartMMThreadProc()
	{
		if (!mResourceManager.IsGroupLoaded("MenuRelated") && !mResourceManager.LoadResources("MenuRelated"))
		{
			mStartInGameModeThreadProcRunning = false;
			mInGameLoadThreadProcFailed = true;
			return;
		}
		if (mResourceManager.IsGroupLoaded("GrottoSounds"))
		{
			mResourceManager.DeleteResources("GrottoSounds");
		}
		if (mResourceManager.IsGroupLoaded("Boss6Common"))
		{
			mResourceManager.DeleteResources("Boss6Common");
		}
		mMainMenu = new MainMenu(this);
		mMainMenu.Init();
		mMainMenu.Resize(GetScreenRect());
		mInGameLoadThreadProcFailed = false;
		mStartInGameModeThreadProcRunning = false;
	}

	public void DoUpsell(bool from_exit)
	{
	}

	public bool IsRegistered()
	{
		return true;
	}

	public bool IsSafeForLockout()
	{
		if (mLoadingThreadStarted)
		{
			return mLoadingThreadCompleted;
		}
		return true;
	}

	public void DoLockout()
	{
		mDoingDRM = true;
		if (mBoard != null)
		{
			mBoard.DoShutdownSaveGame();
		}
	}

	public void DoCredits(bool isFromMainMenu)
	{
		if (!mResourceManager.IsGroupLoaded("Credits") && !mResourceManager.LoadResources("Credits"))
		{
			ShowResourceError(doExit: true);
			Shutdown();
			return;
		}
		mCredits = new Credits(isFromMainMenu);
		mCredits.Init(mBoard != null && !mBoard.IsHardAdventureMode());
		gCreditsHackWidget = new CreditsHackWidget();
		gCreditsHackWidget.Resize(0, 0, mWidth, mHeight);
		gCreditsHackWidget.mClip = false;
		mWidgetManager.AddWidget(gCreditsHackWidget);
		if (!isFromMainMenu)
		{
			EndCurrentGame();
		}
	}

	public void ReturnFromCredits()
	{
		if (!mCredits.mFromMainMenu)
		{
			ShowMainMenu();
		}
		mWidgetManager.RemoveWidget(gCreditsHackWidget);
		SafeDeleteWidget(gCreditsHackWidget);
		mCredits.Dispose();
		mCredits = null;
		gCreditsHackWidget = null;
	}

	public void GenericHelpClosed()
	{
		mGenericHelp = null;
	}

	public void SetStat(string stat_name, int val)
	{
	}

	public int GetStat(string stat_name)
	{
		return 0;
	}

	public void SetAchievement(string achievement_name)
	{
	}

	public void ResetAchievements()
	{
	}

	public void RehupAchievements()
	{
	}

	public virtual void ConvertResources()
	{
	}

	public virtual void ConvertLevels()
	{
	}

	public void OnHardwareBackButtonPressed()
	{
		GlobalMembers.IsBackButtonPressed = true;
	}

	public void OnHardwareBackButtonPressProcessed()
	{
		GlobalMembers.IsBackButtonPressed = false;
	}

	public void OnExiting()
	{
		if (mBoard != null)
		{
			mBoard.ProcessExitingEvent();
		}
	}

	public void OnDeactivated()
	{
		if (mMusicInterface != null)
		{
			mMusicInterface.PauseAllMusic();
			mMusicInterface.OnDeactived();
		}
		if (mBoard != null)
		{
			mBoard.ProcessOnDeactiveEvent();
		}
	}

	public void OnActivated()
	{
		if (mMusicInterface != null)
		{
			mMusicInterface.OnActived();
			mMusicInterface.ResumeAllMusic();
		}
		USE_TRIAL_VERSION = Guide.IsTrialMode;
	}

	public void OnServiceActivated()
	{
		if (mMusicInterface != null)
		{
			mMusicInterface.OnServiceActived();
		}
	}

	public void OnServiceDeactivated()
	{
		if (mMusicInterface != null)
		{
			mMusicInterface.OnServiceDeactived();
		}
	}

	public bool IsHardwareBackButtonPressed()
	{
		return GlobalMembers.IsBackButtonPressed;
	}

	public void InitText()
	{
		TextManager.getInstance().init();
	}

	public override void Init()
	{
		switch (mGameRes)
		{
		case 320:
		case 640:
			mWideScreenXOffset = 0;
			break;
		case 768:
			mWideScreenXOffset = Common._DS(160);
			break;
		}
		mProfileMgr = new ZumaProfileMgr();
		mProfileManager = mProfileMgr;
		mAutoMonkey = new AutoMonkey(this);
		base.Init();
		Res.InitResources(this);
		mResourceManager.mBaseArtRes = mGameRes;
		mResourceManager.mLeadArtRes = 1200;
		mResourceManager.mCurArtRes = mGameRes;
		SetString("DIALOG_BUTTON_YES", TextManager.getInstance().getString(446));
		SetString("DIALOG_BUTTON_NO", TextManager.getInstance().getString(447));
		SetString("DIALOG_BUTTON_OK", TextManager.getInstance().getString(675));
		SetString("DIALOG_BUTTON_CANCEL", TextManager.getInstance().getString(454));
		mCachedLoad = false;
		InitAudio();
		PreShowLoadingScreen();
		LoadGlobalConfig();
		mInitFinished = true;
	}

	public void StartThreadInit()
	{
		if (mInitFinished)
		{
			return;
		}
		mInitThread = Thread.CurrentThread;
		Init();
	}

	public override void InitHook()
	{
	}

	public override string NotifyCrashHook()
	{
		return base.NotifyCrashHook();
	}

	public string GetCrashZipName(int num_override)
	{
		return "";
	}

	public string GetCrashZipName()
	{
		return GetCrashZipName(-1);
	}

	protected void GamerSignedInCallback(object sender, SignedInEventArgs args)
	{
		SignedInGamer gamer = args.Gamer;
		if (gamer != null)
		{
			m_DefaultProfileName = gamer.Gamertag;
		}
		if (gamer.IsSignedInToLive)
		{
			if (m_XLiveState == EXLiveWaiting.E_WaitingForSignIn)
			{
				gamer.BeginGetAchievements(GetAchievementsCallback, gamer);
				m_XLiveState = EXLiveWaiting.E_WaitingForAchivements;
			}
		}
		else
		{
			m_XLiveState = EXLiveWaiting.E_NONE;
			if (IsFirstGameLoad(m_DefaultProfileName))
			{
				gInitialProfLoadSuccessful = true;
				mUserProfile = (ZumaProfile)mProfileMgr.AddProfile(m_DefaultProfileName);
				gDDS.ChangeProfile(mUserProfile);
			}
			else
			{
				mUserProfile = (ZumaProfile)gApp.mProfileMgr.GetProfile(gApp.m_DefaultProfileName);
			}
		}
		USE_TRIAL_VERSION = Guide.IsTrialMode;
	}

	protected void GetAchievementsCallback(IAsyncResult result)
	{
		if (result.AsyncState is SignedInGamer signedInGamer)
		{
			if (mUserProfile == null)
			{
				mUserProfile = (ZumaProfile)gApp.mProfileMgr.GetProfile(0);
			}
			try
			{
				mUserProfile.m_AchievementMgr.m_AchievementsXLive = signedInGamer.EndGetAchievements(result);
			}
			catch (Exception)
			{
			}
			m_XLiveState = EXLiveWaiting.E_Ready;
		}
	}

	public override void LoadingThreadProc()
	{
		if (mCachedLoadState > 1)
		{
			return;
		}
		gInitialProfLoadSuccessful = mProfileMgr.Init();
		SignedInGamer.SignedIn += GamerSignedInCallback;
		int num = 70;
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			Level level = null;
			do
			{
				level = mNormalLevelMgr.GetLevelByIndex(num2++);
			}
			while (level != null && (level.mBoss != null || JeffLib.Common.StrFindNoCase(level.mId, "boss") != -1));
			if (level == null)
			{
				break;
			}
			_ = "levelthumbs\\" + level.mId.ToLower() + "_thumb";
			IdxThumbPair idxThumbPair = new IdxThumbPair();
			idxThumbPair.first = num2 - 1;
			idxThumbPair.second = null;
			mLevelThumbnails.Add(idxThumbPair);
		}
		mResourceManager.PrepareLoadResourcesList(gInitialLoadGroups);
		mMusic.LoadMusic(12, "music/MUSIC_TUNE1");
		mMusic.LoadMusic(24, "music/MUSIC_TUNE2");
		mMusic.LoadMusic(35, "music/MUSIC_TUNE3");
		mMusic.LoadMusic(45, "music/MUSIC_TUNE4");
		mMusic.LoadMusic(58, "music/MUSIC_TUNE5");
		mMusic.LoadMusic(71, "music/MUSIC_TUNE6");
		mMusic.LoadMusic(120, "music/MUSIC_WON1");
		mMusic.LoadMusic(121, "music/MUSIC_WON2");
		mMusic.LoadMusic(122, "music/MUSIC_WON3");
		mMusic.LoadMusic(123, "music/MUSIC_WON4");
		mMusic.LoadMusic(124, "music/MUSIC_WON5");
		mMusic.LoadMusic(125, "music/MUSIC_WON6");
		mMusic.LoadMusic(127, "music/MUSIC_BOSS");
		mMusic.LoadMusic(144, "music/MUSIC_WON_GAME");
		mMusic.LoadMusic(126, "music/MUSIC_GAME_OVER");
	}

	public override void LoadingThreadCompleted()
	{
		base.LoadingThreadCompleted();
		gInitialLoadGroups.Count();
		mBambooTransition = new BambooTransition();
		mProxBombManager = new ProxBombManager();
		if (mCachedLoad)
		{
			mLoadingThreadCompleted = true;
			mLoaded = true;
			ShowMainMenu();
		}
		else if (!mLoadingFailed && mCachedLoadState <= 1)
		{
			mLoadingScreen.LoadingComplete();
		}
	}

	public bool IsFinishedLoading()
	{
		return true;
	}

	public void GameFinishedLoading()
	{
	}

	public void StartLoading()
	{
		if (!mResourceManager.IsGroupLoaded("MainSounds"))
		{
			mResourceManager.LoadResources("MainSounds");
		}
		if (!mResourceManager.IsGroupLoaded("Text"))
		{
			mResourceManager.LoadResources("Text");
		}
		Font fontByName = GetFontByName("FONT_SHAGEXOTICA68_BASE");
		((ImageFont)fontByName).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		((ImageFont)fontByName).PushLayerColor("Shadow", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		Font fontByName2 = GetFontByName("FONT_SHAGEXOTICA68_BLACK");
		((ImageFont)fontByName2).PushLayerColor("Main", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		Font fontByName3 = GetFontByName("FONT_SHAGEXOTICA68_STROKE");
		((ImageFont)fontByName3).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		Font fontByName4 = GetFontByName("FONT_SHAGLOUNGE28_STROKE");
		((ImageFont)fontByName4).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		Font fontByName5 = GetFontByName("FONT_SHAGEXOTICA38_BASE");
		((ImageFont)fontByName5).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		((ImageFont)fontByName5).PushLayerColor("Shadow", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		Font fontByName6 = GetFontByName("FONT_SHAGEXOTICA38_BLACK");
		((ImageFont)fontByName6).PushLayerColor("Main", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		Font fontByName7 = GetFontByName("FONT_SHAGEXOTICA38_BLACK_GLOW");
		((ImageFont)fontByName7).PushLayerColor("Glow", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		Font fontByName8 = GetFontByName("FONT_SHAGEXOTICA38_GREEN_STROKE");
		((ImageFont)fontByName8).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(79, 91, 66, 255));
		Font fontByName9 = GetFontByName("FONT_SHAGEXOTICA100_BASE");
		((ImageFont)fontByName9).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		((ImageFont)fontByName9).PushLayerColor("Shadow", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		Font fontByName10 = GetFontByName("FONT_SHAGEXOTICA100_STROKE");
		((ImageFont)fontByName10).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		((ImageFont)fontByName10).PushLayerColor("Shadow", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		Font fontByName11 = GetFontByName("FONT_SHAGEXOTICA100_GAUNTLET");
		((ImageFont)fontByName11).PushLayerColor("Main", new SexyFramework.Graphics.Color(85, 50, 160, 255));
		((ImageFont)fontByName11).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(248, 238, 195, 255));
		((ImageFont)fontByName11).PushLayerColor("Shadow", new SexyFramework.Graphics.Color(235, 131, 130, 255));
		Font fontByName12 = GetFontByName("FONT_SHAGLOUNGE28_BASE");
		((ImageFont)fontByName12).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		if (Localization.GetCurrentLanguage() != Localization.LanguageType.Language_RU && Localization.GetCurrentLanguage() != Localization.LanguageType.Language_PL)
		{
			((ImageFont)fontByName12).PushLayerColor("Shadow", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		}
		Font fontByName13 = GetFontByName("FONT_SHAGLOUNGE28_SHADOW");
		((ImageFont)fontByName13).PushLayerColor("Shadow", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		Font fontByName14 = GetFontByName("FONT_SHAGLOUNGE28_STROKE_GREEN");
		((ImageFont)fontByName14).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(80, 92, 67, 255));
		Font fontByName15 = GetFontByName("FONT_SHAGLOUNGE28_BROWN");
		((ImageFont)fontByName15).PushLayerColor("Main", new SexyFramework.Graphics.Color(193, 145, 54, 255));
		((ImageFont)fontByName15).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(66, 45, 14, 255));
		((ImageFont)fontByName15).PushLayerColor("Shadow", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		Font fontByName16 = GetFontByName("FONT_SHAGLOUNGE28_GREEN");
		((ImageFont)fontByName16).PushLayerColor("Main", new SexyFramework.Graphics.Color(165, 232, 25, 255));
		((ImageFont)fontByName16).PushLayerColor("Glow", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		Font fontByName17 = GetFontByName("FONT_SHAGLOUNGE38_BASE");
		((ImageFont)fontByName17).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		((ImageFont)fontByName17).PushLayerColor("Shadow", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		Font fontByName18 = GetFontByName("FONT_SHAGLOUNGE38_STROKE");
		((ImageFont)fontByName18).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		Font fontByName19 = GetFontByName("FONT_SHAGLOUNGE38_RED_STROKE_YELLOW");
		((ImageFont)fontByName19).PushLayerColor("Main", new SexyFramework.Graphics.Color(218, 10, 9, 255));
		((ImageFont)fontByName19).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(248, 241, 135, 255));
		Font fontByName20 = GetFontByName("FONT_SHAGLOUNGE38_YELLOW");
		((ImageFont)fontByName20).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(247, 207, 0, 255));
		Font fontByName21 = GetFontByName("FONT_SHAGLOUNGE38_GAUNTLET");
		((ImageFont)fontByName21).PushLayerColor("Main", new SexyFramework.Graphics.Color(249, 245, 188, 255));
		((ImageFont)fontByName21).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(88, 51, 159, 255));
		Font fontByName22 = GetFontByName("FONT_SHAGLOUNGE38_GAUNTLET2");
		((ImageFont)fontByName22).PushLayerColor("Main", new SexyFramework.Graphics.Color(251, 245, 189, 255));
		((ImageFont)fontByName22).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(228, 39, 226, 255));
		Font fontByName23 = GetFontByName("FONT_SHAGLOUNGE45_BASE");
		((ImageFont)fontByName23).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		((ImageFont)fontByName23).PushLayerColor("Shadow", new SexyFramework.Graphics.Color(0, 0, 0, 255));
		Font fontByName24 = GetFontByName("FONT_SHAGLOUNGE45_GAUNTLET");
		((ImageFont)fontByName24).PushLayerColor("Main", new SexyFramework.Graphics.Color(249, 245, 188, 255));
		((ImageFont)fontByName24).PushLayerColor("Stroke", new SexyFramework.Graphics.Color(88, 51, 159, 255));
		Font fontByName25 = GetFontByName("FONT_SHAGLOUNGE45_RED");
		((ImageFont)fontByName25).PushLayerColor("Main", new SexyFramework.Graphics.Color(183, 61, 47, 255));
		Font fontByName26 = GetFontByName("FONT_SHAGLOUNGE45_YELLOW");
		((ImageFont)fontByName26).PushLayerColor("Main", new SexyFramework.Graphics.Color(222, 180, 8, 255));
		StartLoadingComplete = true;
	}

	public override void LostFocus()
	{
		if (mBoard != null && Board.gPauseOnLostFocus)
		{
			mBoard.Pause(pause: true);
		}
		mMusic.Enable(inEnable: false);
		SaveProfile();
	}

	public override void GotFocus()
	{
		DetectMusicSettings();
		if (mBoard != null && Board.gPauseOnLostFocus)
		{
			mBoard.Pause(pause: false);
			mBoard.mNumPauseUpdatesToDo = Common._M(50);
			mBoard.MarkDirty();
		}
		ReportAppLaunchInfo(4);
	}

	public override bool DebugKeyDown(int key)
	{
		return false;
	}

	public override void UpdateFrames()
	{
		mMusic.Update();
		mSoundPlayer.Update();
		base.UpdateFrames();
		TransitionFromLoadingScreen();
		if (mDialogMap.Count > 0)
		{
			if (mMainMenu != null && mMainMenu.mUserSelDlg != null)
			{
				mWidgetManager.PutBehind(mUnderDialogWidget, mMainMenu.mUserSelDlg);
			}
			else
			{
				mWidgetManager.PutBehind(mUnderDialogWidget, mDialogList.Last.Value);
			}
			if (mDialogObscurePct < 1f)
			{
				if (mBoard != null && mBoard.mDoingFirstTimeIntro)
				{
					mDialogObscurePct = Math.Min(Common._M(0.9f), mDialogObscurePct + Common._M1(0.06f));
				}
				else
				{
					mDialogObscurePct = Math.Min(1f, mDialogObscurePct + Common._M(0.06f));
				}
			}
		}
		else
		{
			if (mBoard != null && mBoard.mDoingFirstTimeIntro)
			{
				mDialogObscurePct = Math.Max(0f, mDialogObscurePct - Common._M(0.015f));
			}
			else
			{
				mDialogObscurePct = Math.Max(0f, mDialogObscurePct - Common._M(0.06f));
			}
			if (mDialogObscurePct == 0f && mUnderDialogWidget.mVisible)
			{
				mUnderDialogWidget.SetVisible(isVisible: false);
			}
		}
		if (m_XLiveState != EXLiveWaiting.E_Ready)
		{
			return;
		}
		m_XLiveState = EXLiveWaiting.E_NONE;
		SignedInGamer signedInGamer = Gamer.SignedInGamers[PlayerIndex.One];
		if (signedInGamer != null)
		{
			m_DefaultProfileName = signedInGamer.Gamertag;
		}
		if (!IsFirstGameLoad(m_DefaultProfileName) || !IsFirstGameLoad(m_DefaultName))
		{
			if (!IsFirstGameLoad(m_DefaultName))
			{
				gApp.mProfileMgr.RenameProfile(m_DefaultName, gApp.m_DefaultProfileName);
			}
			mUserProfile = (ZumaProfile)gApp.mProfileMgr.GetProfile(gApp.m_DefaultProfileName);
		}
		else
		{
			gInitialProfLoadSuccessful = true;
			mUserProfile = (ZumaProfile)mProfileMgr.AddProfile(m_DefaultProfileName);
			gDDS.ChangeProfile(mUserProfile);
		}
	}

	public virtual void PlaySamplePan(int theSoundNum, int thePan, int min_time)
	{
		SoundAttribs soundAttribs = new SoundAttribs();
		soundAttribs.pan = thePan;
		mSoundPlayer.Play(theSoundNum, soundAttribs);
	}

	public virtual void PlaySamplePan(int theSoundNum, int thePan)
	{
		PlaySamplePan(theSoundNum, thePan, 5);
	}

	public override void PlaySample(int theSoundNum, int min_time)
	{
		mSoundPlayer.Play(theSoundNum);
	}

	public override void PlaySample(int theSoundNum)
	{
		PlaySample(theSoundNum, 5);
	}

	public override void DialogButtonDepress(int dialog_id, int button_id)
	{
		switch (dialog_id)
		{
		case 1:
			if (mYesNoDialogDelegate != null)
			{
				mYesNoDialogDelegate(button_id);
				mDialog.Kill();
				if (mBoard != null)
				{
					mBoard.Pause(pause: false, becauseOfDialog: true);
				}
				if (mDialogMap.Count() == 1)
				{
					mDialog.SetFocusWidgetToBoard();
				}
				mDialog.Kill();
			}
			break;
		case 0:
			((ZumaDialog)GetDialog(dialog_id)).Kill();
			if (mBoard != null)
			{
				mBoard.Pause(pause: false, becauseOfDialog: true);
			}
			if (mDialogCallBack != null)
			{
				mDialogCallBack();
				mDialogCallBack = null;
			}
			break;
		}
	}

	public override void SwitchScreenMode(bool wantWindowed, bool is3d, bool force)
	{
		base.SwitchScreenMode(wantWindowed, is3d, force);
		RegistryWriteBoolean("Is3D", is3d);
		if (mBoard != null)
		{
			mBoard.mNumPauseUpdatesToDo = Common._M(10);
			mBoard.MarkDirty();
		}
	}

	public override MusicInterface CreateMusicInterface()
	{
		if (mNoSoundNeeded)
		{
			return new MusicInterface();
		}
		return base.CreateMusicInterface();
	}

	public override void HandleCmdLineParam(string theParamName, string theParamValue)
	{
		base.HandleCmdLineParam(theParamName, theParamValue);
	}

	public override void AddDialog(int id, Dialog d)
	{
		gAddingDlgID = id;
		base.AddDialog(id, d);
		gAddingDlgID = -12345;
		if (id == 6)
		{
			return;
		}
		foreach (Dialog mDialog in mDialogList)
		{
			if (mDialog == d)
			{
				continue;
			}
			DialogHideInfo dialogHideInfo = new DialogHideInfo();
			dialogHideInfo.mDialog = mDialog;
			dialogHideInfo.mHideCount = 1;
			new KeyValuePair<int, DialogHideInfo>(mDialog.mId, dialogHideInfo);
			DialogHideInfo value = null;
			if (mDialogHideInfoMap != null)
			{
				if (mDialogHideInfoMap.TryGetValue(mDialog.mId, out value))
				{
					value.mHideCount++;
				}
				else
				{
					mDialogHideInfoMap.Add(mDialog.mId, dialogHideInfo);
				}
			}
		}
	}

	public override void AddDialog(Dialog theDialog)
	{
		base.AddDialog(theDialog);
	}

	public override bool KillDialog(int id, bool removeWidget, bool deleteWidget)
	{
		if (id != gAddingDlgID)
		{
			List<int> list = new List<int>();
			if (mDialogHideInfoMap != null)
			{
				foreach (KeyValuePair<int, DialogHideInfo> item in mDialogHideInfoMap)
				{
					if (--item.Value.mHideCount == 0)
					{
						list.Add(item.Key);
					}
				}
				for (int i = 0; i < list.Count(); i++)
				{
					mDialogHideInfoMap.Remove(list[i]);
				}
			}
		}
		return base.KillDialog(id, removeWidget, deleteWidget);
	}

	public override bool KillDialog(int theDialogId)
	{
		return base.KillDialog(theDialogId);
	}

	public override bool KillDialog(Dialog theDialog)
	{
		return base.KillDialog(theDialog);
	}

	public void InitAudio()
	{
		mMusic = new Music(mMusicInterface);
		mMusic.RegisterCallBack();
		mSoundPlayer = new SoundEffects(mSoundManager);
	}

	public bool MusicEnabled()
	{
		return !mMusicInterface.isPlayingUserMusic();
	}

	public void DetectMusicSettings()
	{
		Dialog dialog = GetDialog(2);
		if (dialog != null)
		{
			((OptionsDialog)dialog).DetectMusicSettings();
		}
		else
		{
			mMusic.Enable(MusicEnabled() && GetMusicVolume() > 0.0);
		}
	}

	public void TransitionFromLoadingScreen()
	{
		if (mLoadingScreen != null)
		{
			if (mDelayIntro)
			{
				LoadBoard();
			}
			else if (mLoadingScreen.CanShowMenu() && !TriggerFirstProfileDialog())
			{
				ShowMainMenu();
				mSoundPlayer.Stop(Res.GetSoundByID(ResID.SOUND_SEAGULLS));
				mWidgetManager.BringToFront(mLoadingScreen);
			}
			else if (mLoadingScreen.Done() && mNewUserDlg == null)
			{
				KillLoadingScreen();
				mSoundPlayer.Stop(GetSoundIDByName("SOUND_SEAGULLS"), inUnload: true);
			}
		}
	}

	public void LoadBoard()
	{
		mDelayIntro = false;
		if (mBoard != null)
		{
			mWidgetManager.AddWidget(mBoard);
		}
		KillLoadingScreen();
	}

	public void KillLoadingScreen()
	{
		if (mLoadingScreen != null)
		{
			mWidgetManager.RemoveWidget(mLoadingScreen);
			mLoadingScreen.Dispose();
			mLoadingScreen = null;
			if (mResourceManager.IsGroupLoaded("LoadScreen"))
			{
				mResourceManager.DeleteResources("LoadScreen");
			}
		}
	}

	public bool TriggerFirstProfileDialog()
	{
		return false;
	}

	public bool IsFirstGameLoad()
	{
		return mProfileMgr.GetNumProfiles() == 0;
	}

	public bool IsFirstGameLoad(string name)
	{
		return !mProfileMgr.HasProfile(name);
	}

	public LevelMgr GetLevelMgr()
	{
		if (mUserProfile == null || mBoard == null)
		{
			return mNormalLevelMgr;
		}
		return mNormalLevelMgr;
	}

	public void ResetAllLevelMgrs()
	{
		mNormalLevelMgr.Reset();
	}

	public bool ReloadAllLevelMgrs()
	{
		LevelMgr[] array = new LevelMgr[1] { mNormalLevelMgr };
		for (int i = 0; i < 1; i++)
		{
			if (!array[i].LoadLevels(array[i].mLevelXML))
			{
				Popup(array[i].GetErrorText());
				Popup("Your boss DDS parameters were all reset. You should quit and restart.");
				return false;
			}
		}
		return true;
	}

	public void ShowMainMenu(bool do_load_thread)
	{
		mClickedHardMode = false;
		PlaySong(1);
		if (mInitialLoad)
		{
			if (!gApp.mResourceManager.IsGroupLoaded("MenuRelated") && !mResourceManager.LoadResources("MenuRelated"))
			{
				mStartInGameModeThreadProcRunning = false;
				mInGameLoadThreadProcFailed = true;
				return;
			}
			mMainMenu = new MainMenu(this);
			mMainMenu.Init();
			mMainMenu.Resize(GetScreenRect());
			mWidgetManager.AddWidget(mMainMenu);
			mWidgetManager.SetFocus(mMainMenu);
			mLoadingScreen.mMouseVisible = false;
			mInitialLoad = false;
			CheckForAppUpdate();
		}
		else
		{
			if (mUserProfile != null)
			{
				mUserProfile.mDoChallengeTrophyZoom = (mUserProfile.mDoChallengeAceTrophyZoom = false);
				mUserProfile.mDoChallengeAceCupComplete = (mUserProfile.mDoChallengeCupComplete = false);
				mUserProfile.mUnlockSparklesIdx1 = (mUserProfile.mUnlockSparklesIdx2 = -1);
			}
			SetupMainMenuDefaults(do_load_thread);
		}
		if (!gApp.mResourceManager.IsGroupLoaded("Map"))
		{
			mResourceManager.PrepareLoadResources("Map");
		}
		if (!gApp.mResourceManager.IsGroupLoaded("CommonGame"))
		{
			mResourceManager.PrepareLoadResources("CommonGame");
		}
		if (mUserProfile == null)
		{
			ZumaProfile zumaProfile = (ZumaProfile)mProfileMgr.GetAnyProfile();
			string theString = "";
			if (zumaProfile != null)
			{
				theString = zumaProfile.GetName();
			}
			RegistryReadString("LastUser", ref theString);
			if (theString.Length > 0)
			{
				if (!gInitialProfLoadSuccessful || !ChangeUser(theString))
				{
					if (mProfileMgr.GetNumProfiles() != 0)
					{
						zumaProfile = (ZumaProfile)mProfileMgr.GetAnyProfile();
						if (zumaProfile != null)
						{
							mUserProfile = zumaProfile;
							ChangeUser(zumaProfile.GetName());
						}
						mMainMenu.DoChangeUserDialog();
						ClearUpdateBacklog(relaxForASecond: false);
					}
					else
					{
						if (!gInitialProfLoadSuccessful && !mCachedLoad)
						{
							DoGenericDialog("ERROR", "One or more of your saved game files is\nincompatible with this version of the game.\nThey have been deleted.", block: true, null, 0);
						}
						DoNewUserDialog();
					}
				}
				mMainMenu.MarkDirty();
			}
			else
			{
				DoNewUserDialog();
				mMainMenu.MarkDirty();
			}
			mMainMenu.RehupButtons();
		}
		if (mUserProfile != null && mMainMenu.mChallengeMenu != null)
		{
			mMainMenu.mChallengeMenu.InitCS();
		}
		mMainMenu.RehupButtons();
	}

	public void ShowMainMenu()
	{
		ShowMainMenu(do_load_thread: true);
	}

	public void HideChallengeMenu()
	{
		if (mMainMenu != null && mMainMenu.mChallengeMenu != null)
		{
			mMainMenu.HideChallengeMenu();
		}
	}

	public void HideMainMenu(bool delete_resources)
	{
		if (mMainMenu != null)
		{
			if (mMainMenu.mChallengeMenu != null)
			{
				mMainMenu.HideChallengeMenu();
			}
			mWidgetManager.RemoveWidget(mMainMenu);
			SafeDeleteWidget(mMainMenu);
			mMainMenu = null;
		}
		HideAdventureModeMapScreen();
		if (mResourceManager.IsGroupLoaded("MenuRelated"))
		{
			mResourceManager.DeleteResources("MenuRelated");
		}
	}

	public void ShowMoreGames()
	{
		mMoreGames = new MoreGames(this);
		mMoreGames.Init();
		mMoreGames.Resize(gApp.GetScreenRect());
		mWidgetManager.AddWidget(mMoreGames);
		mMainMenu.DoMoreGamesSlide(isSlidingIn: false);
		mMoreGames.DoSlide(p: true);
	}

	public void HideMoreGames()
	{
		if (mMoreGames != null)
		{
			mMoreGames.DoSlide(p: false);
		}
		mMainMenu.DoMoreGamesSlide(isSlidingIn: true);
	}

	public void DeleteMoreGames(bool delete_resources)
	{
		if (mMoreGames != null)
		{
			mWidgetManager.RemoveWidget(mMoreGames);
			SafeDeleteWidget(mMoreGames);
			mMoreGames = null;
		}
		if (delete_resources)
		{
			mResourceManager.DeleteResources("MoreGames");
		}
	}

	public void ShowIronFrog()
	{
		SetupMainMenuDefaults();
		mMainMenu.mChallengeMenu.InitCS();
		mMainMenu.RehupButtons();
		mMainMenu.DoIronFrog(scroll: false);
		PlaySong(1);
	}

	public void ShowChallengeSelector()
	{
		SetupMainMenuDefaults();
		mMainMenu.ShowChallengeMenu();
		mMainMenu.mChallengeMenu.mCueMainSong = true;
	}

	public void ShowAdventureModeMapScreen()
	{
		if (!mResourceManager.IsGroupLoaded("Map") && !mResourceManager.LoadResources("Map"))
		{
			ShowResourceError(doExit: true);
			Shutdown();
			return;
		}
		if (!mResourceManager.IsGroupLoaded("GamePlay"))
		{
			mResourceManager.PrepareLoadResources("GamePlay");
		}
		mMapScreen = new MapScreen();
		mMapScreenHackWidget = new MapScreenHackWidget();
		mMapScreen.mParent = mMapScreenHackWidget;
		mWidgetManager.AddWidget(mMapScreenHackWidget);
		mMapScreenHackWidget.Resize(0, 0, mWidth, mHeight);
		mMapScreen.Init(zone_completed: false, mUserProfile.GetAdvModeVars().mCurrentAdvZone, mUserProfile.GetAdvModeVars().mCurrentAdvLevel, from_checkpoint: false, from_load: true);
		mWidgetManager.SetFocus(mMapScreenHackWidget);
		mMapScreen.DoSlide(slide_in: true);
		if (mMainMenu != null)
		{
			mMainMenu.HideScrollButtons();
		}
	}

	public void HideAdventureModeMapScreen()
	{
		if (mMapScreenHackWidget != null)
		{
			mWidgetManager.RemoveWidget(mMapScreenHackWidget);
			SafeDeleteWidget(mMapScreenHackWidget);
			mMapScreenHackWidget = null;
		}
		if (mMapScreen != null)
		{
			mMapScreen.Dispose();
			mMapScreen = null;
		}
		if ((mUserProfile == null || !mUserProfile.mNeedsFirstTimeIntro) && mResourceManager.IsGroupLoaded("Map"))
		{
			mResourceManager.DeleteResources("Map");
		}
		if (mMainMenu != null)
		{
			mMainMenu.ShowScrollButtons();
			PlaySong(1);
		}
	}

	public void StartAdventureMode()
	{
		if (!mStartInGameModeThreadProcRunning)
		{
			PlaySong(12);
			mLoadType = ((mForceZoneRestart != -1) ? 1 : 0);
			if (IsHardMode())
			{
				mUserProfile.mFirstTimeReplayingHardMode = false;
			}
			else
			{
				mUserProfile.mFirstTimeReplayingNormalMode = false;
			}
			mStartInGameModeThreadProcRunning = true;
			StartAdvModeThreadProc();
			Rect aRect;
			if (mLoadType == 1)
			{
				int mX = mMapScreen.mCards[mMapScreen.mSelectedZone - 1].mX;
				int mY = mMapScreen.mCards[mMapScreen.mSelectedZone - 1].mY;
				Image imageByName = GetImageByName("IMAGE_UI_CHALLENGESCREEN_HOME_SELECT");
				aRect = new Rect(mX, mY, (int)(0.4f * (float)imageByName.mWidth), (int)(0.4f * (float)imageByName.mHeight));
			}
			else
			{
				aRect = new Rect(Common._DS(Common._M(624)), Common._DS(Common._M1(697)), Common._DS(Common._M2(700)), Common._DS(Common._M3(500)));
			}
			Ratio aspectRatio = mGraphicsDriver.GetAspectRatio();
			int num = ((aspectRatio.mNumerator != 4 && aspectRatio.mDenominator != 3) ? Common._DS(Common._M(160)) : 0);
			aRect.mWidth += num;
			DoCommonInGameLoadThread(aRect);
			mBoard.AdventureModeSetupComplete(mContinuedGame);
			HideMainMenu(delete_resources: true);
		}
	}

	public void StartAdvModeFirstTime()
	{
		if (!mResourceManager.IsGroupLoaded("MapZoom") && !mResourceManager.LoadResources("MapZoom"))
		{
			ShowResourceError(doExit: true);
			Shutdown();
			return;
		}
		if (!mResourceManager.IsGroupLoaded("Text") && !mResourceManager.LoadResources("Text"))
		{
			ShowResourceError(doExit: true);
			Shutdown();
			return;
		}
		mMusic.FadeOut();
		HideMainMenu(delete_resources: true);
		mBoard = new Board(this, -1);
		if (mLoadingScreen == null)
		{
			mWidgetManager.AddWidget(mBoard);
		}
		mBoard.mAdventureMode = true;
		if (!mBoard.Init(do_first_time_intro: true))
		{
			mInGameLoadThreadProcFailed = true;
			mStartInGameModeThreadProcRunning = false;
			return;
		}
		mBoard.Resize(0, 0, mWidth, mHeight);
		mContinuedGame = false;
		mBoard.StartLevel(1);
		mBoard.MakeCachedBackground();
		mWidgetManager.SetFocus(mBoard);
		if (mWidescreenBoardWidget == null)
		{
			mWidescreenBoardWidget = new WidescreenBoardWidget();
			mWidescreenBoardWidget.Resize(Common._S(-80), 0, mWidth + Common._S(160), mHeight);
			mWidgetManager.AddWidget(mWidescreenBoardWidget);
		}
		mBoard.SetMenuBtnEnabled(enabled: false);
		mAutoMonkey.SetState(MonkeyState.Playing);
	}

	public void DoDeferredEndGame()
	{
		if (mBoard != null)
		{
			mBoard.mNumDrawFramesLeft = Common._M(2);
			mBoard.mReturnToMainMenu = true;
		}
	}

	public void EndCurrentGame()
	{
		mBoard.DoShutdownSaveGame();
		mBoard.mSkipShutdownSave = true;
		mWidgetManager.RemoveWidget(mBoard);
		SafeDeleteWidget(mBoard);
		mBoard = null;
	}

	public void StartGauntletMode(string normal_level_id, Rect thumb_rect)
	{
		if (!mStartInGameModeThreadProcRunning)
		{
			mLoadType = 2;
			mChallengeLevelId = normal_level_id;
			mStartInGameModeThreadProcRunning = true;
			StartChallengeModeThreadProc();
			Rect aRect = new Rect(thumb_rect);
			Ratio aspectRatio = mGraphicsDriver.GetAspectRatio();
			int num = ((aspectRatio.mNumerator != 4 && aspectRatio.mDenominator != 3) ? Common._DS(Common._M(320)) : 0);
			aRect.mWidth += num;
			DoCommonInGameLoadThread(aRect);
			HideMainMenu(delete_resources: true);
			PlaySong(12);
			mBoard.GauntletModeSetupComplete();
			mAutoMonkey.SetState(MonkeyState.Playing);
		}
	}

	public void StartIronFrogMode()
	{
		if (!mStartInGameModeThreadProcRunning)
		{
			mUserProfile.mIronFrogStats.mCurTime = 0;
			mLoadType = 3;
			mStartInGameModeThreadProcRunning = true;
			StartIronFrogModeThreadProc();
			int num = Common._DS(Common._M(700));
			int num2 = Common._DS(Common._M(650));
			DoCommonInGameLoadThread(new Rect((mWidth - num) / 2, (mHeight - num2) / 2, num, num2));
			HideMainMenu(delete_resources: true);
			PlaySong(12);
		}
	}

	public void PlaySong(int song, float fade_speed)
	{
		bool inLoop = true;
		switch (song)
		{
		case 0:
		case 120:
		case 121:
		case 122:
		case 123:
		case 124:
		case 125:
			inLoop = false;
			break;
		case 126:
		case 137:
			inLoop = false;
			break;
		}
		mMusic.PlaySongNoDelay(song, inLoop);
	}

	public void PlaySong(int song)
	{
		PlaySong(song, 0.005f);
	}

	public void DoOptionsDialog(bool ingame)
	{
		if (mBoard != null)
		{
			mBoard.Pause(pause: true, becauseOfDialog: true);
		}
		OptionsDialog optionsDialog = new OptionsDialog(ingame);
		Common.SetupDialog(optionsDialog);
		AddDialog(optionsDialog);
		if (ingame)
		{
			optionsDialog.Move(optionsDialog.mX, optionsDialog.mY + Common._S(30));
		}
	}

	public void FinishOptionsDialog(bool doSave)
	{
		OptionsDialog optionsDialog = GetDialog(2) as OptionsDialog;
		bool wantWindowed = false;
		bool flag = true;
		bool flag2 = true;
		_ = mIsWindowed;
		Is3DAccelerated();
		if (flag2)
		{
			flag = true;
		}
		bool flag3 = false;
		EnableCustomCursors(enabled: false);
		mCursorTarget = false;
		RegistryWriteBoolean("Z2Cursor", mCursorTarget);
		if (doSave)
		{
			mColorblind = optionsDialog.mColorBlindSlider.IsOn();
			SaveGlobalConfig();
		}
		if (flag3)
		{
			RegistryWriteBoolean("PreHiRes", mHiRes);
			RegistryWriteBoolean("Pre3D", Is3DAccelerated());
			RegistryWriteBoolean("PreWindowed", mIsWindowed);
			RegistryWriteBoolean("NeedsConfirmation", theValue: true);
			mPreferredWidth = (mPreferredHeight = -1);
			RegistryWriteBoolean("HiRes", theValue: true);
			mReInit = true;
			Shutdown();
			if (!flag)
			{
				RegistryWriteBoolean("Is3D", theValue: false);
			}
			else
			{
				RegistryEraseValue("Is3D");
			}
		}
		else
		{
			SwitchScreenMode(wantWindowed, flag, force: true);
			ClearUpdateBacklog(relaxForASecond: false);
		}
		optionsDialog.mDrawScale.SetCurve(Common._MP("b+0,1,0.05,1,~###         ~#A5t"));
		optionsDialog.mWidgetFlagsMod.mRemoveFlags |= 16;
		optionsDialog.Kill();
		if (mBoard != null)
		{
			mBoard.Pause(pause: false, becauseOfDialog: true);
			if (mBoard.mMenuButton != null)
			{
				mBoard.mMenuButton.mDisabled = false;
			}
		}
	}

	public int DoQuitPromptDialog()
	{
		return DoYesNoDialog(TextManager.getInstance().getString(448), TextManager.getInstance().getString(453), block: true);
	}

	public void TakeScreenshot(string prefix)
	{
	}

	public static SharedImageRef CompositionLoadFunc(string file_dir, string file_name)
	{
		int num = file_name.IndexOf('\\');
		string text = "";
		string text2 = "";
		if (num != -1)
		{
			text = file_name.Substring(0, num);
			text2 = file_name.Substring(num + 1);
		}
		string text3;
		string text4;
		if (text.Length == 0)
		{
			text3 = JeffLib.Common.PathToResName(file_dir, "images", "IMAGE") + mCompositionResPrefix + "_" + JeffLib.Common.StripFileExtension(file_name).ToUpper();
			text4 = JeffLib.Common.PathToResName(file_dir, "images", "IMAGE") + "_" + JeffLib.Common.StripFileExtension(file_name).ToUpper();
		}
		else
		{
			text3 = JeffLib.Common.PathToResName(file_dir, "images", "IMAGE") + mCompositionResPrefix + "_" + (text + "_" + text2).ToUpper();
			text4 = JeffLib.Common.PathToResName(file_dir, "images", "IMAGE") + "_" + (text + "_" + text2).ToUpper();
		}
		text3 = text3.Replace(' ', '_');
		text3 = text3.Replace('-', '_');
		text4 = text4.Replace(' ', '_');
		text4 = text4.Replace('-', '_');
		SharedImageRef sharedImageRef = gApp.mResourceManager.LoadImage(text3);
		if (sharedImageRef == null || (sharedImageRef != null && sharedImageRef.GetImage() == null))
		{
			sharedImageRef = gApp.mResourceManager.LoadImage(text4);
			sharedImageRef.mSharedImage.mImage.mFilePath = text4;
		}
		else
		{
			sharedImageRef.mSharedImage.mImage.mFilePath = text3;
		}
		return sharedImageRef;
	}

	public static void CompositionPostLoadFunc(SharedImageRef img, Layer l)
	{
		l.mXOff = Common._DS(gApp.mResourceManager.GetImageOffset(l.GetImage().mFilePath).mX);
		l.mYOff = Common._DS(gApp.mResourceManager.GetImageOffset(l.GetImage().mFilePath).mY);
	}

	public bool ChangeUser(string user_name)
	{
		if (mProfileMgr == null || string.IsNullOrWhiteSpace(user_name))
		{
			return false;
		}
		ZumaProfile zumaProfile = (ZumaProfile)mProfileMgr.GetProfile(user_name);
		if (zumaProfile == null)
		{
			return false;
		}
		mUserProfile = zumaProfile;
		if (gDDS != null)
		{
			gDDS.ChangeProfile(mUserProfile);
		}
		RegistryWriteString("LastUser", mUserProfile.GetName());
		return true;
	}

	public bool DeleteUser(string user_name)
	{
		if (mProfileMgr == null || string.IsNullOrWhiteSpace(user_name))
		{
			return false;
		}
		if (mUserProfile != null && string.Equals(mUserProfile.GetName(), user_name, StringComparison.OrdinalIgnoreCase))
		{
			mUserProfile = null;
		}
		return mProfileMgr.DeleteProfile(user_name);
	}

	public bool ShadersSupported()
	{
		return true;
	}

	public void DoNewUserDialog(int button_mode, bool isIntro)
	{
		if (mUserProfile != null || mProfileMgr == null)
		{
			return;
		}
		string text = string.IsNullOrWhiteSpace(m_DefaultProfileName) ? "Player 1" : m_DefaultProfileName;
		ZumaProfile zumaProfile = (ZumaProfile)mProfileMgr.GetProfile(text);
		if (zumaProfile == null)
		{
			zumaProfile = (ZumaProfile)mProfileMgr.AddProfile(text);
		}
		if (zumaProfile == null)
		{
			zumaProfile = (ZumaProfile)mProfileMgr.GetAnyProfile();
		}
		if (zumaProfile != null)
		{
			mUserProfile = zumaProfile;
			gInitialProfLoadSuccessful = true;
			if (gDDS != null)
			{
				gDDS.ChangeProfile(mUserProfile);
			}
			RegistryWriteString("LastUser", mUserProfile.GetName());
		}
	}

	public void DoNewUserDialog()
	{
		DoNewUserDialog(3, isIntro: false);
	}

	public void DoNewUserDialog(int button_mode)
	{
		DoNewUserDialog(button_mode, isIntro: false);
	}

	public Rect GetNewUserDialogFrame()
	{
		return Rect.ZERO_RECT;
	}

	public void BlankNameEntered()
	{
	}

	public void NameIsAllSpaces()
	{
	}

	public void FinishedNewUser(bool canceled)
	{
	}

	public void DoGenericDialog(string header, string message, bool block, PreBlockCallback pre_block_callback, int width_pad)
	{
		Font fontByName = GetFontByName("FONT_SHAGLOUNGE38_YELLOW");
		ZumaDialog zumaDialog = new ZumaDialog(0, isModal: true, "", message, TextManager.getInstance().getString(483), 3);
		zumaDialog.mSpaceAfterHeader = 0;
		if (mBoard != null)
		{
			mBoard.Pause(pause: true, becauseOfDialog: true);
		}
		zumaDialog.mContentInsets.mTop += Common._S(Common._M(30));
		int widest = 0;
		int height = 0;
		JeffLib.Common.StringDimensions(message, fontByName, out widest, out height);
		zumaDialog.mAllowDrag = false;
		zumaDialog.GetSize(ref widest, ref height);
		widest += width_pad;
		zumaDialog.Resize((mWidth - widest) / 2, (mHeight - height) / 2, widest, height);
		Common.SetupDialog(zumaDialog);
		AddDialog(zumaDialog);
		mDialogCallBack = pre_block_callback;
	}

	public int DoYesNoDialog(string header, string message, bool block, string btn_yes, string btn_no, bool drag, int header_space, int id)
	{
		return DoYesNoDialog(header, message, block, btn_yes, btn_no, drag, header_space, id, 0);
	}

	public int DoYesNoDialog(string header, string message, bool block, string btn_yes, string btn_no, bool drag, int header_space)
	{
		return DoYesNoDialog(header, message, block, btn_yes, btn_no, drag, header_space, 1, 0);
	}

	public int DoYesNoDialog(string header, string message, bool block, string btn_yes, string btn_no, bool drag)
	{
		return DoYesNoDialog(header, message, block, btn_yes, btn_no, drag, -1, 1, 0);
	}

	public int DoYesNoDialog(string header, string message, bool block, string btn_yes, string btn_no)
	{
		return DoYesNoDialog(header, message, block, btn_yes, btn_no, drag: true, -1, 1, 0);
	}

	public int DoYesNoDialog(string header, string message, bool block, string btn_yes)
	{
		return DoYesNoDialog(header, message, block, btn_yes, TextManager.getInstance().getString(447), drag: true, -1, 1, 0);
	}

	public int DoYesNoDialog(string header, string message, bool block)
	{
		return DoYesNoDialog(header, message, block, TextManager.getInstance().getString(446), TextManager.getInstance().getString(447), drag: true, -1, 1, 0);
	}

	public int DoYesNoDialog(string header, string message)
	{
		return DoYesNoDialog(header, message, block: false, TextManager.getInstance().getString(446), TextManager.getInstance().getString(447), drag: true, -1, 1, 0);
	}

	public int DoYesNoDialog(string header, string message, bool block, string btn_yes, string btn_no, bool drag, int header_space, int id, int width_pad)
	{
		Font fontByName = GetFontByName("FONT_SHAGLOUNGE38_YELLOW");
		mDialog = new ZumaDialog(id, isModal: true, "", message, "", 1);
		mDialog.mSpaceAfterHeader = 0;
		if (mBoard != null)
		{
			mBoard.Pause(pause: true, becauseOfDialog: true);
		}
		mDialog.mContentInsets.mTop += Common._S(Common._M(30));
		JeffLib.Common.StringDimensions(message, fontByName, out var widest, out var height);
		mDialog.mAllowDrag = false;
		mDialog.GetSize(ref widest, ref height);
		widest += width_pad;
		mDialog.Resize((mWidth - widest) / 2, (mHeight - height) / 2, widest, height);
		mDialog.mYesButton.mLabel = btn_yes;
		mDialog.mNoButton.mLabel = btn_no;
		mDialog.mAllowDrag = false;
		Common.SetupDialog(mDialog);
		AddDialog(mDialog);
		mWidgetManager.SetFocus(mDialog);
		if (block)
		{
			return mDialog.WaitForResult(autoKill: false);
		}
		return -1;
	}

	public void EndYesNoDialog(int ButtonId)
	{
		if (mYesNoDialogDelegate != null)
		{
			mYesNoDialogDelegate(ButtonId);
		}
	}

	public int GetPan(int thePos)
	{
		return 3000 * (thePos - 400) / 400;
	}

	public CompositionMgr LoadComposition(string file_name, string res_prefix)
	{
		string key = SexyLocale.StringToUpper(file_name);
		if (mPreloadedComps.ContainsKey(key) && mPreloadedComps[key].isValid())
		{
			return new CompositionMgr(mPreloadedComps[key]);
		}
		CompositionMgr compositionMgr = new CompositionMgr();
		compositionMgr.mLoadImageFunc = CompositionLoadFunc;
		compositionMgr.mPostLoadImageFunc = CompositionPostLoadFunc;
		mCompositionResPrefix = res_prefix;
		bool flag = compositionMgr.LoadFromFile(file_name);
		mCompositionResPrefix = "";
		if (!flag)
		{
			compositionMgr = null;
		}
		mPreloadedComps[key] = compositionMgr;
		return new CompositionMgr(compositionMgr);
	}

	public PIEffect GetPIEffect(string file_name, bool create_copy)
	{
		if (mLoadingThreadCompleted)
		{
			switch (file_name)
			{
			case "TorchFlame":
			{
				for (int l = 0; l < mCachedTorchEffects.size(); l++)
				{
					CachedTorchEffect cachedTorchEffect2 = mCachedTorchEffects[l];
					if (!cachedTorchEffect2.mFlameInUse)
					{
						cachedTorchEffect2.mFlameInUse = true;
						cachedTorchEffect2.mTorchFlame.ResetAnim();
						return cachedTorchEffect2.mTorchFlame;
					}
				}
				break;
			}
			case "TorchFlameOut":
			{
				for (int j = 0; j < mCachedTorchEffects.size(); j++)
				{
					CachedTorchEffect cachedTorchEffect = mCachedTorchEffects[j];
					if (!cachedTorchEffect.mFlameOutInUse)
					{
						cachedTorchEffect.mFlameOutInUse = true;
						cachedTorchEffect.mTorchFlameOut.ResetAnim();
						return cachedTorchEffect.mTorchFlameOut;
					}
				}
				break;
			}
			case "Devil Projectile":
			{
				for (int k = 0; k < mCachedVolcanoEffects.size(); k++)
				{
					CachedVolcanoEffect cachedVolcanoEffect2 = mCachedVolcanoEffects[k];
					if (!cachedVolcanoEffect2.mProjectileInUse)
					{
						cachedVolcanoEffect2.mProjectileInUse = true;
						cachedVolcanoEffect2.mProjectile.ResetAnim();
						cachedVolcanoEffect2.mProjectile.mEmitAfterTimeline = true;
						return cachedVolcanoEffect2.mProjectile;
					}
				}
				break;
			}
			case "Devil Explosion":
			{
				for (int i = 0; i < mCachedVolcanoEffects.size(); i++)
				{
					CachedVolcanoEffect cachedVolcanoEffect = mCachedVolcanoEffects[i];
					if (!cachedVolcanoEffect.mExplosionInUse)
					{
						cachedVolcanoEffect.mExplosionInUse = true;
						cachedVolcanoEffect.mExplosion.ResetAnim();
						return cachedVolcanoEffect.mExplosion;
					}
				}
				break;
			}
			}
		}
		if (mCachedPIEffects.ContainsKey(file_name))
		{
			for (int m = 0; m < mCachedPIEffects[file_name].Count; m++)
			{
				GenericCachedEffect genericCachedEffect = mCachedPIEffects[file_name][m];
				if (!genericCachedEffect.mInUse)
				{
					genericCachedEffect.mInUse = true;
					genericCachedEffect.mEffect.ResetAnim();
					return genericCachedEffect.mEffect;
				}
			}
		}
		string theFileName = GetBaseResImagesDir() + "particles\\" + file_name + "\\" + file_name + ".ppf";
		PIEffect pIEffect = new PIEffect();
		PIEffect pIEffect2 = new PIEffect();
		if (!pIEffect2.LoadEffect(theFileName))
		{
			return null;
		}
		pIEffect = pIEffect2;
		if (!create_copy)
		{
			return pIEffect;
		}
		return pIEffect.Duplicate();
	}

	public PIEffect GetPIEffect(string file_name)
	{
		return GetPIEffect(file_name, create_copy: true);
	}

	public bool IsHardMode()
	{
		return false;
	}

	public MemoryImage GenerateLevelThumbnail(string thumb_path, Level l)
	{
		return null;
	}

	public bool IronFrogUnlocked()
	{
		if (mUserProfile != null)
		{
			return mUserProfile.mAdvModeVars.mNumTimesZoneBeat[5] > 0;
		}
		return false;
	}

	public bool ChallengeModeUnlocked()
	{
		if (mUserProfile != null)
		{
			return mUserProfile.mChallengeUnlockState[0, 0] > 0;
		}
		return false;
	}

	public bool HSScreenUnlocked()
	{
		if (mUserProfile != null)
		{
			return mUserProfile.mAdvModeVars.mNumTimesZoneBeat[0] >= 1;
		}
		return false;
	}

	public void ReleaseTorchEffect(PIEffect fx)
	{
		if (fx == null)
		{
			return;
		}
		for (int i = 0; i < mCachedTorchEffects.size(); i++)
		{
			CachedTorchEffect cachedTorchEffect = mCachedTorchEffects[i];
			if (cachedTorchEffect.mTorchFlame == fx)
			{
				cachedTorchEffect.mFlameInUse = false;
				break;
			}
			if (cachedTorchEffect.mTorchFlameOut == fx)
			{
				cachedTorchEffect.mFlameOutInUse = false;
				break;
			}
		}
	}

	public void ReleaseVolcanoEffect(PIEffect fx)
	{
		if (fx == null)
		{
			return;
		}
		for (int i = 0; i < mCachedVolcanoEffects.size(); i++)
		{
			CachedVolcanoEffect cachedVolcanoEffect = mCachedVolcanoEffects[i];
			if (cachedVolcanoEffect.mProjectile == fx)
			{
				cachedVolcanoEffect.mProjectileInUse = false;
				break;
			}
			if (cachedVolcanoEffect.mExplosion == fx)
			{
				cachedVolcanoEffect.mExplosionInUse = false;
				break;
			}
		}
	}

	public void ReleaseGenericCachedEffect(PIEffect fx)
	{
		if (fx == null)
		{
			return;
		}
		Dictionary<string, List<GenericCachedEffect>>.Enumerator enumerator = mCachedPIEffects.GetEnumerator();
		while (enumerator.MoveNext())
		{
			foreach (GenericCachedEffect item in enumerator.Current.Value)
			{
				if (item.mEffect == fx)
				{
					item.mInUse = false;
					break;
				}
			}
		}
	}

	public Board GetBoard()
	{
		return mBoard;
	}

	public bool ShowingLoadingScreen()
	{
		return mLoadingScreen != null;
	}

	public void IncFramesPlayed()
	{
		mFramesPlayed++;
	}

	public string GetResImagesDir()
	{
		return $"images\\{mGameRes}\\";
	}

	public string GetBaseResImagesDir()
	{
		return $"images\\{mResourceManager.mBaseArtRes}\\";
	}

	public static int ScaleNum(int theNum, int theAdd)
	{
		return (int)((float)theNum * mGameUpScale) + theAdd;
	}

	public static int ScaleNum(int theNum)
	{
		return ScaleNum(theNum, 0);
	}

	public static float ScaleNum(float theNum, float theAdd)
	{
		return theNum * mGameUpScale + theAdd;
	}

	public static float ScaleNum(float theNum)
	{
		return ScaleNum(theNum, 0f);
	}

	public static double ScaleNum(double theNum, double theAdd)
	{
		return theNum * (double)mGameUpScale + theAdd;
	}

	public static double ScaleNum(double theNum)
	{
		return ScaleNum(theNum, 0.0);
	}

	public static int DownScaleNum(int theNum, int theAdd)
	{
		return (int)((float)theNum * mGameDownScale) + theAdd;
	}

	public static int DownScaleNum(int theNum)
	{
		return DownScaleNum(theNum, 0);
	}

	public static float DownScaleNum(float theNum, float theAdd)
	{
		return theNum * mGameDownScale + theAdd;
	}

	public static float DownScaleNum(float theNum)
	{
		return DownScaleNum(theNum, 0f);
	}

	public static double DownScaleNum(double theNum, double theAdd)
	{
		return theNum * (double)mGameDownScale + theAdd;
	}

	public static double DownScaleNum(double theNum)
	{
		return DownScaleNum(theNum, 0.0);
	}

	public static int ScreenScaleNum(int theNum, int theAdd)
	{
		return (int)((float)theNum * mGameScreenScale) + theAdd;
	}

	public static int ScreenScaleNum(int theNum)
	{
		return ScreenScaleNum(theNum, 0);
	}

	public static float ScreenScaleNum(float theNum, float theAdd)
	{
		return theNum * mGameScreenScale + theAdd;
	}

	public static float ScreenScaleNum(float theNum)
	{
		return ScreenScaleNum(theNum, 0f);
	}

	public static double ScreenScaleNum(double theNum, double theAdd)
	{
		return theNum * (double)mGameScreenScale + theAdd;
	}

	public static double ScreenScaleNum(double theNum)
	{
		return ScreenScaleNum(theNum, 0.0);
	}

	public virtual uint GetProfileVersion()
	{
		return 0u;
	}

	public virtual void NotifyProfileChanged(UserProfile player)
	{
	}

	public virtual UserProfile CreateUserProfile()
	{
		return new ZumaProfile();
	}

	public virtual void OnProfileLoad(UserProfile player, SexyFramework.Misc.Buffer buffer)
	{
	}

	public virtual void OnProfileSave(UserProfile player, SexyFramework.Misc.Buffer buffer)
	{
	}

	public Rect GetScreenRect()
	{
		return mWidgetManager.mMouseDestRect;
	}

	public int GetScreenWidth()
	{
		return mWidgetManager.mMouseDestRect.mWidth - mWidgetManager.mMouseDestRect.mX;
	}

	public static bool IsTablet()
	{
		return true;
	}

	public Image GetLevelThumbnail(int theLevelNum)
	{
		Image second = mLevelThumbnails[theLevelNum].second;
		string[] array = new string[6] { "jungle", "village", "city", "coast", "grotto", "volcano" };
		if (second == null)
		{
			int num = theLevelNum / 10;
			int num2 = theLevelNum % 10 + 1;
			string text = array[num] + $"{num2}";
			string theFileName = "levelthumbs\\" + text + "_thumb";
			IdxThumbPair idxThumbPair = mLevelThumbnails[theLevelNum];
			idxThumbPair.second = GetImage(theFileName, commitBits: true, allowTriReps: true, isInAtlas: false);
			if (idxThumbPair.second != null)
			{
				second = idxThumbPair.second;
			}
		}
		return second;
	}

	public void DeleteLevelThumbnail(int theLevel)
	{
		if (theLevel >= 0 && theLevel <= mLevelThumbnails.Count())
		{
			IdxThumbPair idxThumbPair = mLevelThumbnails[theLevel];
			if (idxThumbPair.second != null)
			{
				idxThumbPair.second.Dispose();
				idxThumbPair.second = null;
			}
		}
	}

	public void DeleteZoneThumbnails(int theZone)
	{
		if (theZone < 0 || theZone > 6)
		{
			return;
		}
		int num = theZone * 10;
		for (int i = 0; i < 10; i++)
		{
			IdxThumbPair idxThumbPair = mLevelThumbnails[num + i];
			if (idxThumbPair.second != null)
			{
				idxThumbPair.second.Dispose();
				idxThumbPair.second = null;
			}
		}
	}

	public void LoadAllThumbnails()
	{
		for (int i = 0; i < 6; i++)
		{
			int num = i * 10;
			for (int j = 0; j < 10; j++)
			{
				GetLevelThumbnail(num + j);
			}
		}
	}

	public void AppEnteredBackground()
	{
		if (mBoard != null && mBoard.NeedSaveGame() && mUserProfile != null)
		{
			mBoard.SaveGame(mUserProfile.GetSaveGameName(IsHardMode()), null);
		}
	}

	public override double GetLoadingThreadProgress()
	{
		return mResourceManager.GetLoadResourcesListProgress(gInitialLoadGroups);
	}

	public void ToggleBambooTransition()
	{
		if (mBambooTransition != null && !mBambooTransition.IsInProgress())
		{
			mBambooTransition.Reset();
			mBambooTransition.SetVisible(isVisible: true);
			mBambooTransition.SetDisabled(isDisabled: false);
			mWidgetManager.AddWidget(mBambooTransition);
			mWidgetManager.BringToFront(mBambooTransition);
			mBambooTransition.StartTransition();
		}
	}

	public void BambooTransitionOpened()
	{
		mBambooTransition.Reset();
		mBambooTransition.SetVisible(isVisible: false);
		mBambooTransition.SetDisabled(isDisabled: true);
		mWidgetManager.RemoveWidget(mBambooTransition);
	}

	public void EndChallengeModeGame()
	{
		EndCurrentGame();
		ShowChallengeSelector();
	}

	public void InitMetricsManager()
	{
	}

	public void HideHelp()
	{
		if (GetDialog(2) is OptionsDialog optionsDialog)
		{
			optionsDialog.OnHelpHided();
		}
	}

	public void ShowAbout()
	{
		mAboutInfo = new AboutInfo();
		AddDialog(mAboutInfo);
	}

	public void HideAbout()
	{
		mAboutInfo.mDrawScale.SetCurve(Common._MP("b+0,1,0.05,1,~###         ~#A5t"));
		mAboutInfo.mWidgetFlagsMod.mRemoveFlags |= 16;
		mAboutInfo = null;
	}

	public void ShowLegal()
	{
		mLegalInfo = new LegalInfo();
		AddDialog(mLegalInfo);
	}

	public void HideLegal()
	{
		mLegalInfo.mDrawScale.SetCurve(Common._MP("b+0,1,0.05,1,~###         ~#A5t"));
		mLegalInfo.mWidgetFlagsMod.mRemoveFlags |= 16;
		mLegalInfo = null;
	}

	public void ShowMetricsDebug()
	{
	}

	public void HideMetricsDebug()
	{
	}

	public void ReportAppLaunchInfo(int theAppEvent)
	{
	}

	public void ReportEndOfLevelMetrics(Board theBoard, bool theLevelSuccess, bool theAcedLevel)
	{
	}

	public void ReportEndOfLevelMetrics(Board theBoard, bool theLevelSuccess)
	{
		ReportEndOfLevelMetrics(theBoard, theLevelSuccess, theAcedLevel: false);
	}

	public void ReportEndOfLevelMetrics(Board theBoard)
	{
		ReportEndOfLevelMetrics(theBoard, theLevelSuccess: false, theAcedLevel: false);
	}

	public void CheckForAppUpdate()
	{
	}

	public void GetTouchInputOffset(ref int x, ref int y)
	{
		x = mTouchOffsetX;
		y = mTouchOffsetY;
	}

	public void SetTouchInputOffset(int x, int y)
	{
		mTouchOffsetX = x;
		mTouchOffsetY = y;
	}

	public void LoadLevelXML()
	{
		mLoadingProc = LoadingLevel;
		mLoadingThread = new Thread(mLoadingProc);
		mLoadLevelSuccess = false;
		mLoadingThread.Start();
	}

	private void LoadingLevel()
	{
		mNormalLevelMgr = ((XNAFileDriver)mFileDriver).GetContentManager().Load<LevelMgr>(mLevelXML);
		mNormalLevelMgr.Init();
		mNormalLevelMgr.mLevelXML = mLevelXML;
		mLoadLevelSuccess = true;
	}

	public void OpenURL(string url)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		try
		{
			WebBrowserTask val = new WebBrowserTask();
			val.Uri = new Uri(url);
			val.Show();
		}
		catch (Exception)
		{
		}
	}

	public void HandleGameUpdateRequired(GameUpdateRequiredException ex)
	{
		UN_UPDATE_VERSION = true;
		USE_XBOX_SERVICE = false;
		mDisplayTitleUpdateMessage = true;
		DisplayTitleUpdateMessage();
	}

	public static void DisplayTitleUpdateMessage()
	{
		List<string> list = new List<string>();
		string item = TextManager.getInstance().getString(446);
		string item2 = TextManager.getInstance().getString(447);
		list.Add(item2);
		list.Add(item);
		if (mDisplayTitleUpdateMessage && !Guide.IsVisible)
		{
			mDisplayTitleUpdateMessage = false;
			string text = TextManager.getInstance().getString(62);
			Guide.BeginShowMessageBox("   ", text, list, 1, MessageBoxIcon.Alert, UpdateDialogGetMBResult, null);
		}
	}

	public static void UpdateDialogGetMBResult(IAsyncResult userResult)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		int? num = Guide.EndShowMessageBox(userResult);
		if (num.HasValue && num.Value > 0)
		{
			if (Guide.IsTrialMode)
			{
				Guide.ShowMarketplace(PlayerIndex.One);
				return;
			}
			MarketplaceDetailTask val = new MarketplaceDetailTask();
			val.ContentType = (MarketplaceContentType)1;
			val.ContentIdentifier = "43f34364-9df4-4d95-b9cf-e48b3c85cda9";
			val.Show();
		}
	}

	public void ToMarketPlace()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		if (Guide.IsTrialMode)
		{
			Guide.ShowMarketplace(PlayerIndex.One);
			return;
		}
		MarketplaceDetailTask val = new MarketplaceDetailTask();
		val.ContentType = (MarketplaceContentType)1;
		val.Show();
	}

	public static void initResolution(int param1)
	{
		mGameRes = param1;
		int num = mGameRes;
		if (num <= 640)
		{
			switch (num)
			{
			case 600:
				mGameUpScale = 1f;
				mGameDownScale = 0.5f;
				mGameScreenScale = 1f;
				return;
			case 320:
				mGameUpScale = 0.5333334f;
				mGameDownScale = 0.2666667f;
				mGameScreenScale = 1f / mGameUpScale;
				return;
			case 640:
				mGameUpScale = 1.0666668f;
				mGameDownScale = 0.5333334f;
				mGameScreenScale = 1f / mGameUpScale;
				return;
			}
		}
		else
		{
			if (num == 720)
			{
				mGameUpScale = 1.2f;
				mGameDownScale = 0.6f;
				mGameScreenScale = 1f / mGameUpScale;
				return;
			}
			if (num == 768)
			{
				mGameUpScale = 1.28f;
				mGameDownScale = 0.64f;
				mGameScreenScale = 1f / mGameUpScale;
				return;
			}
			_ = 1200;
		}
		mGameUpScale = 2f;
		mGameDownScale = 1f;
		mGameScreenScale = 0.5f;
	}

	public void SetOrientation(int Orientation)
	{
		((WP7AppDriver)mAppDriver).SetOrientation(Orientation);
	}
}
