using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JeffLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using SexyFramework;
using SexyFramework.AELib;
using SexyFramework.Drivers;
using SexyFramework.File;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;
using ZumasRevenge.Achievement;

namespace ZumasRevenge;

public class Board : Widget, ButtonListener
{
	private enum State
	{
		StatsState_AceTime,
		StatsState_Points,
		StatsState_Done
	}

	protected enum BoardState
	{
		BoardState_Game,
		BoardState_Option,
		BoardState_BackToMainMenuPrompt,
		BoardState_OptionToMainMenuPrompt,
		BoardState_Credits,
		BoardState_Help
	}

	protected class GamepadControls
	{
		public static int ACCEL_MAX = 4;

		public float accel = 1f;

		public SexyVector2 axis = default(SexyVector2);

		public SexyVector2 lastAxis = default(SexyVector2);
	}

	private enum iOS_Button
	{
		Button_SwapBalls,
		NUM_BOARD_BUTTONS
	}

	private enum CONTROL_MODE
	{
		CONTROL_MODE_NONE,
		CONTROL_MODE_SWAPPING,
		CONTROL_MODE_DODGING,
		CONTROL_MODE_AIMING
	}

	protected static bool gNeedsVortexSound = true;

	protected static float UpdateVortex_last_angle = 0f;

	protected static int END_BOSS_FROG_JUMP_TIME = 50;

	public static int gNumColors = 3;

	public static bool gUseGlobalColorNum = false;

	public static bool gUseChangeColor = false;

	public static int gTuneNum = 0;

	public static bool gShowText = true;

	public static bool gHideBalls = false;

	public static bool gUpdateBalls = true;

	public static bool gMouseCheatMode = false;

	public static bool gForceTreasure = false;

	public static bool gPauseOnLostFocus = true;

	public static int gIntroRibbitTimer = 0;

	public static int FROG_DEATH_X = 396;

	public static int FROG_DEATH_Y = 460;

	public static readonly int TREASURE_LIFE = 1000;

	public static CurveData gDebugCurveData = null;

	public static string gDebugCurveFile;

	public static bool gNewStyleBallChooser = true;

	public static bool gShowStats = true;

	private static bool gNeedsGauntletHSSound = true;

	private static bool gCheatReload = false;

	private static int gEndGauntletExtraTime = 0;

	private static int gMultTimeLeftDecAmt = 0;

	private static int delay_after_skull = 0;

	private static int gLastUnderwaterSound = 0;

	private static BallDrawer drawer = null;

	private static int end_delay = 0;

	private static int END_GAUNTLET_TIME = 350;

	private static bool gNeedBossIntroSound = true;

	private static int gTextHandle = 0;

	private static float SLIDER_FROG_DEVICE_SPEEDUP = 3f;

	private static int gEssenceDrawFrame = 55;

	public static float MAX_STONE_HEAD_STRETCH = 1.001f;

	protected BoardState mBoardState;

	protected GamepadControls mGamepadControls = new GamepadControls();

	public GameApp mApp;

	public List<Ball> mNeedComboCount = new List<Ball>();

	public Level mLevel;

	public Level mNextLevel;

	public List<ZumaTip> mZumaTips = new List<ZumaTip>();

	public TreasurePoint mCurTreasure;

	public char[] mMuMuMode = new char[4];

	public bool mDoMuMuMode;

	public PIEffect[] mLazerBeam = new PIEffect[2];

	public PIEffect mLazerBurn;

	public Transform mGlobalTranform = new Transform();

	public ObjectPool<BallExplosion> mBallExplosionsPool = new ObjectPool<BallExplosion>(20);

	public ObjectPool<EndLevelExplosion> mEndLevelExplosionPool = new ObjectPool<EndLevelExplosion>(20);

	public bool mContinueNextLevelOnLoadProfile;

	public int mNextLevelOverrideOnLoadProfile;

	public int mGauntletPointsForDiffInc;

	public int mGauntletPointsFromMult;

	public int mFullScreenAlpha;

	public int mFullScreenAlphaRate;

	public int[] mBallColorMap = new int[6];

	public int[] mNewBallDelay = new int[2];

	public int mScore;

	public int mScoreTarget;

	public int mFlashAlpha;

	public int mLevelEndFrame;

	public int mMouseOverGunPos;

	public int mCurTreasureNum;

	public int mZoneTipIdx;

	public int mScoreTipIdx;

	public int mGauntletHSTarget;

	public int mScoreMultiplier;

	public int mFruitMultiplier;

	public float mMinTreasureY;

	public float mMaxTreasureY;

	public float mGuideT;

	private SexyVector3 mGuideWallPoint = default(SexyVector3);

	public int mTheNextLevel;

	public ChallengeHelp mChallengeHelp;

	public float mEndBossFadeAmt;

	public bool mAdventureWinScreen;

	public bool mAdventureMode;

	public bool mIsHardMode;

	public bool mTreasureWasHit;

	public bool mIsWinning;

	public bool mSkipToNextLevelOnNextUpdate;

	public bool mPreventBallAdvancement;

	public bool mRollingInDangerZone;

	public int mNumZumaBalls;

	public int mNumPauseUpdatesToDo;

	public int mNumDrawFramesLeft;

	public bool mReturnToMainMenu;

	public bool mDoingFirstTimeIntro;

	public bool mDoingFirstTimeIntroZoomToGame;

	public int mEndLevelAceTimeBonus;

	public int mEndLevelNum;

	public int mEndLevelParTime;

	public GameStats mEndLevelStats = new GameStats();

	public string mEndLevelDisplayName = "";

	public int mPrevIFBestScore;

	public bool mSkipShutdownSave;

	public List<LTSmokeParticle> mSmokeParticles = new List<LTSmokeParticle>();

	public PIEffect mSmokePoof;

	public int mCloakBossIntroAlpha;

	public Image mIntroBG;

	public Image mIntroWater;

	public List<SimpleFadeText> mIntroDialog = new List<SimpleFadeText>();

	public float mIntroFadeAmt;

	public int mCloakedBossFrame;

	public CurvedVal mIntroMidAlpha = new CurvedVal();

	public CurvedVal mIntroMidScale = new CurvedVal();

	public CurvedVal mIntroMidTransX = new CurvedVal();

	public CurvedVal mIntroMapAlpha = new CurvedVal();

	public CurvedVal mIntroMapPinAlpha = new CurvedVal();

	public CurvedVal mIntroMapScale = new CurvedVal();

	public CurvedVal mIntroMapTransX = new CurvedVal();

	public CurvedVal mIntroRotate = new CurvedVal();

	public CurvedVal mIntroFrogScale = new CurvedVal();

	public int mTimeToBeatAdvMode;

	public int mBeatGameTotalScoreTally;

	public int mBeatGameLives;

	public int mBeatGameNormalScore;

	public int mIntroTimer;

	public bool mDoIntroFrogJump;

	public bool mIsRestarting;

	public ButtonWidget mAdvWinBtn;

	public ButtonWidget mStatsContinueBtn;

	public PIEffect mChallengeCupUnlockedFX;

	public DeathSkull mDeathSkull;

	public List<BallExplosion> mBallExplosions = new List<BallExplosion>();

	public List<PIEffect> mLazerBlasts = new List<PIEffect>();

	public PIEffectBatch mEffectBatch = new PIEffectBatch();

	public Composition mBoss6StoneBurst;

	public Composition mBoss6VolcanoMelt;

	public Bouncy mFruitBounceEffect = new Bouncy();

	public uint mLastBallClickTick;

	public uint mLastSmallExplosionTick;

	public uint mLastExplosionTick;

	public Image mBackgroundImage;

	public Gun mFrog;

	public MemoryImage mCachedCurveImage;

	public Checkpoint mCheckpointEffect;

	public List<Bullet> mBulletList = new List<Bullet>();

	public Ball mGuideBall;

	public SexyVector3 mGuideBallPoint = default(SexyVector3);

	public GameState mGameState;

	public SexyVector3 mGuideCenter = default(SexyVector3);

	public SexyVector3 mLazerGuideCenter = default(SexyVector3);

	public SexyFramework.Misc.Point[] mGuide = new SexyFramework.Misc.Point[4]
	{
		new SexyFramework.Misc.Point(),
		new SexyFramework.Misc.Point(),
		new SexyFramework.Misc.Point(),
		new SexyFramework.Misc.Point()
	};

	public SexyFramework.Misc.Point[] mLazerGuide = new SexyFramework.Misc.Point[4]
	{
		new SexyFramework.Misc.Point(),
		new SexyFramework.Misc.Point(),
		new SexyFramework.Misc.Point(),
		new SexyFramework.Misc.Point()
	};

	public List<Tunnel>[] mTunnels = new List<Tunnel>[5];

	public MemoryImage[] mCachedTunnelImages = new MemoryImage[5];

	public List<SimpleFadeText> mSimpleFadeText = new List<SimpleFadeText>();

	public List<BonusTextElement> mText = new List<BonusTextElement>();

	public List<VortexFace> mVortexFaces = new List<VortexFace>();

	public List<VortexBeam> mVortexBeams = new List<VortexBeam>();

	public QRand mQRand;

	public List<PowerEffect> mPowerEffects = new List<PowerEffect>();

	public RollerScore mRollerScore;

	public ButtonWidget mMenuButton;

	public int mMenuButtonX;

	public string mNextLevelIdOverride = "";

	public CursorBloom[] mCursorBlooms = new CursorBloom[2];

	public LevelTransition mLevelTransition;

	public string mStatsString = "";

	public MapScreen mMapScreen;

	public FwooshImage[] mLevelNameText = new FwooshImage[2];

	public FwooshImage mLevelCompleteText = new FwooshImage();

	public FwooshImage mChallengeHeaderText = new FwooshImage();

	public FwooshImage mChallengePtsText = new FwooshImage();

	public float mChallengeTextAlpha;

	public Image mFruitImg;

	public Image mFruitGlow;

	public FruitExplode mFruitExplodeEffect;

	public List<EndLevelExplosion> mEndLevelExplosions = new List<EndLevelExplosion>();

	public List<MultiplierBallEffect> mMultiplierBallEffects = new List<MultiplierBallEffect>();

	public FrogFlyOff mFrogFlyOff;

	public DeviceImage mTransitionScreen;

	public DeviceImage mTransitionScreenImage;

	public bool mDoingTransition;

	public bool mPlayThud;

	public CurvedVal mStatsBubbleScale = new CurvedVal();

	public CurvedVal mTransitionScreenHolePct = new CurvedVal();

	public CurvedVal mTransitionScreenScale = new CurvedVal();

	public CurvedVal mTransitionFrogRotPct = new CurvedVal();

	public CurvedVal mTransitionFrogScale = new CurvedVal();

	public CurvedVal mTransitionFrogPosPct = new CurvedVal();

	public SexyFramework.Misc.Point mTransitionCenter = new SexyFramework.Misc.Point();

	public PIEffect mVolcanoBossEssence;

	public PIEffect mEssenceExplBottom;

	public PIEffect mEssenceExplTop;

	public float mEssenceXScale;

	public float mEssenceYScale;

	public int mEssenceScaleTimer;

	public int mEndBossFrogTimer;

	public bool mDoingEndBossFrogEffect;

	public PIEffect mBossSmokePoof;

	public FakeCredits mFakeCredits;

	public DarkFrogSequence mDarkFrogSequence;

	public float mCurrentSatPct;

	public float mIronFrogAlpha;

	public ExtraSexyButton mIronFrogBtn;

	public bool mDoingIronFrogWin;

	public int mIronFrogWinDelay;

	public bool mNeedsBossExtraLife;

	public bool mDoingBossIntroText;

	public float mBossIntroAlpha;

	public float mBossIntroAlphaRate;

	public float mBossTextY;

	public float mBattleTextY;

	public float mBossTextVY;

	public float mBattleTextVY;

	public int mBossIntroDirection;

	public int mBossIntroFramesLeft;

	public int mBossIntroDelay;

	public FwooshImage mFightImage = new FwooshImage();

	public bool mDoingBossIntroFightText;

	public CurvedVal mBossIntroBGAlpha = new CurvedVal();

	public CurvedVal mBossSmScale = new CurvedVal();

	public CurvedVal mBossSmPosPct = new CurvedVal();

	public CurvedVal mBossRedPct = new CurvedVal();

	public int mAdvStatsTime;

	public int mPreCheckpointLives;

	public float mDarkFrogBulletX;

	public float mDarkFrogBulletY;

	public float mDarkFrogBulletVX;

	public float mDarkFrogBulletVY;

	public int mDarkFrogTimer;

	public bool mForceRestartInAdvMode;

	public bool mForceToNextLevelInAdvMode;

	public bool mDisplayAceTime;

	public float mVortexBGAlpha;

	public bool mVortexAppear;

	public float mVortexFrogRadius;

	public float mVortexFrogAngle;

	public float mVortexFrogScale;

	public bool mVortexFrogRadiusExpand;

	public float mAdventureWinExtraAlpha;

	public float mAdventureWinAlpha;

	public float mAdventureWinDoorYOff;

	public float mAdventureWinTimer;

	public float mTreasureVY;

	public float mTreasureDefaultVY;

	public float mTreasureAccel;

	public float mTreasureYBob;

	public int mLastIntroPad;

	public int mLastIntroPadDelay;

	public int mIntroPadHopCount;

	public int mTreasureGlowAlpha;

	public int mTreasureGlowAlphaRate;

	public int mTreasureStarAlpha;

	public int mTreasureEndFrame;

	public int mTreasureCel;

	public float mTreasureStarAngle;

	public int mLevelNum;

	public int mStateCount;

	public int mIgnoreCount;

	public int mAccuracyCount;

	public int mAccuracyBackupCount;

	public int mScreenShakeTime;

	public int mScreenShakeXMax;

	public int mScreenShakeYMax;

	public int mDestroyCount;

	public int mPauseCount;

	public int mLastPauseTick;

	public int mPauseUpdateCnt;

	public int mPauseFade;

	public int mDialogCount;

	public int mLevelBeginScore;

	public int mHallucinateTimer;

	public int mCurStatsPointCounter;

	public int mCurStatsPointTarget;

	public int mCurStatsPointInc;

	public int mLevelPoints;

	public int mStatsState;

	public int mStatsDelay;

	public bool mWasPerfectLevel;

	public bool mNeedsCheckpointIntro;

	public bool mHasSeenCheckpointIntro;

	public int mNumDeaths;

	public int mStatsHue;

	public int mUnpauseFrame;

	public float mGauntletAlpha;

	public float mGauntletTrophyDropRate;

	public int mGauntletTrophyBounceCount;

	public float mGauntletTrophyY;

	public Image mGauntletTrophyImg;

	public int mEndGauntletTimer;

	public float mGauntletMultBarAlpha;

	public int mGauntletLastFrogX;

	public int mGauntletLastFrogY;

	public bool mGauntletMultTextFlashOn;

	public int mGauntletMultTextFlashTimer;

	public float mGauntletMultTextVX;

	public float mGauntletMultTextVY;

	public int mGauntletMultTextMoveLastFrame;

	public bool mGauntletModeOver;

	public int mLives;

	public int mPointsLeftForExtraLife;

	public ButtonWidget mGauntletRetryBtn;

	public ButtonWidget mGauntletQuitBtn;

	public int mStartingGauntletLevel;

	public int mGauntletTikiUnlocked;

	public bool mNewGauntletHS;

	public int mGauntletHSIndex;

	public int mGauntletFinalScorePreBonus;

	public float mScoreDisplayPos;

	public List<ScoreLetterEffect> mScoreLetterEffectVector = new List<ScoreLetterEffect>();

	public string[] mScoreBreakStrings = new string[2];

	public SexyFramework.Misc.Point[] mScoreBreakPositions = new SexyFramework.Misc.Point[2];

	public int mGauntletWidestNameLen;

	public bool mNewIronFrogHS;

	public int mNumClearsInARow;

	public int mCurInARowBonus;

	public int mCurComboScore;

	public int mCurComboCount;

	public int mNumCleared;

	public GameStats mLevelStats = new GameStats();

	public GameStats mGameStats = new GameStats();

	public bool mGauntletMode;

	public bool mIsEndless;

	public bool mDoGuide;

	public bool mShowGuide;

	public bool mRecalcGuide;

	public bool mRecalcLazerGuide;

	public bool mShowBallsDuringPause;

	public bool mDestroyAll;

	public bool mLevelBeginning;

	public bool mForceTreasure;

	public bool mLazerHitTreasure;

	public bool mHasDoneIntroSounds;

	public bool mAllowBulletDetection;

	public bool mIsLoading;

	public bool mShowMapScreen;

	public bool mDoPostBossMapScreen;

	public bool mWasShowingCheckpoint;

	public bool mDbgHurry;

	public bool mShowDDSWindow;

	public bool mShowBossDDSWindow;

	public bool mCanDeleteEffectResources;

	public Rect mAStatsFrame;

	public SexyFramework.Misc.Point mBossOffset = new SexyFramework.Misc.Point();

	public Rect mCStatsFrame = default(Rect);

	public bool mIsTryAndBuyDialogShowing;

	public int mFatFingerGuideAlpha;

	public bool mFatFingerGuideEnabled;

	public bool mIsHotFrogEnabled;

	public int mBallPowerupCheat;

	public ButtonWidget mSwapBallButton;

	public bool mInvalidateTouchUp;

	public bool mFinishHaloSwap;

	public bool mDrawHaloSwap;

	public CurvedVal mHaloSwapCurve = new CurvedVal();

	public SexyFramework.Graphics.Color mHaloSwapColor = default(SexyFramework.Graphics.Color);

	public MemoryImage mAimGuide;

	public LivesInfo mLivesInfo;

	public bool gDrawAutoAimAssistInfo;

	public List<KeyValuePair<string, int>> m_NotificationQuene = new List<KeyValuePair<string, int>>();

	public NotificationWidget mNotificationWidget;

	public string prevLevelID = "";

	private CONTROL_MODE mControlMode;

	private bool mIsMouseDown;

	private SexyFramework.Misc.Point mInitialTouchPoint = new SexyFramework.Misc.Point();

	private int mCurveClearBonus;

	private int mTouchCount;

	public bool mDrawBossUI;

	private static SexyFramework.Misc.Point gPt1;

	private static SexyFramework.Misc.Point gPt2;

	private static SexyFramework.Misc.Point gCenter;

	private static bool gCheckCollision = true;

	protected void ConsoleCallback(string cmd, List<string> @params)
	{
	}

	protected void DoHitTreasure()
	{
		mTreasureWasHit = true;
		mTreasureGlowAlphaRate = Common._M(12);
		mLevelStats.mNumGemsCleared++;
		int num = (mScoreTarget - mLevelBeginScore) / 600 * 100;
		if (mIsEndless)
		{
			if (num < 200)
			{
				num = 200;
			}
		}
		else if (num < 500)
		{
			num = 500;
		}
		mFruitExplodeEffect.Reset();
		StringBuilder stringBuilder = new StringBuilder(TextManager.getInstance().getString(95));
		if (GauntletMode())
		{
			num = Common._M(500);
			stringBuilder.Replace("$1", num.ToString());
			if (mFruitMultiplier > 1)
			{
				AddText(stringBuilder.ToString(), mCurTreasure.x, mCurTreasure.y, Common._M(1.5f), -1, null);
			}
			else
			{
				AddText(stringBuilder.ToString(), mCurTreasure.x, mCurTreasure.y, Common._M(1.5f), -1, null);
			}
			mFruitMultiplier++;
			GetBetaStats().SetFruitMultiplier(mFruitMultiplier);
		}
		else
		{
			stringBuilder.Replace("$1", num.ToString());
			AddText(stringBuilder.ToString(), mCurTreasure.x, mCurTreasure.y, Common._M(1.5f), -1, null);
		}
		GetBetaStats().HitFruit(num);
		IncScore(num, from_balls: false);
		mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_TIKI_HIT));
	}

	protected void SetupEndOfGauntletTransition(bool hit_max_time)
	{
		if (!GameApp.gApp.mResourceManager.IsGroupLoaded("AdventureStats") && !GameApp.gApp.mResourceManager.LoadResources("AdventureStats"))
		{
			GameApp.gApp.ShowResourceError(doExit: true);
		}
		mGauntletHSIndex = mApp.mUserProfile.AddGauntletHighScore((mLevel.mZone - 1) * 10 + mLevel.mNum, mScore, mApp.mUserProfile.GetName());
		mNewGauntletHS = mGauntletHSIndex == 0;
		mScoreDisplayPos = Common._M(5f);
		mGauntletWidestNameLen = 0;
		mGauntletAlpha = 0f;
		mGauntletTrophyBounceCount = 0;
		mGauntletTrophyDropRate = 0f;
		mGauntletTrophyImg = null;
		mChallengeTextAlpha = 0f;
		CreateChallengeStatsButtons();
		CreateChallengeScoreImage();
		CreateChallengeHeaderImage();
	}

	protected void CreateChallengeStatsButtons()
	{
		int num = (int)((float)mCStatsFrame.mWidth * 0.5f);
		int num2 = (int)((float)mCStatsFrame.mHeight * 0.18f);
		int num3 = mCStatsFrame.mX;
		int theY = mCStatsFrame.mY + (mCStatsFrame.mHeight - num2);
		mGauntletQuitBtn = CreateChallengeStatsButton(11, TextManager.getInstance().getString(456), new Rect(num3, theY, num, num2));
		mGauntletRetryBtn = CreateChallengeStatsButton(10, TextManager.getInstance().getString(457), new Rect(num3 + num, theY, num, num2));
		ButtonWidget[] inButtons = new ButtonWidget[2] { mGauntletQuitBtn, mGauntletRetryBtn };
		Common.SizeButtonsToLabel(inButtons, 2, Common._S(30));
		SetMenuBtnEnabled(enabled: false);
		AddWidget(mGauntletRetryBtn);
		AddWidget(mGauntletQuitBtn);
	}

	protected ButtonWidget CreateChallengeStatsButton(int inButtonID, string inButtonText, Rect inFrame)
	{
		ButtonWidget buttonWidget = Common.MakeButton(inButtonID, this, inButtonText);
		buttonWidget.mBtnNoDraw = true;
		buttonWidget.mVisible = false;
		buttonWidget.mPriority = 2;
		buttonWidget.mDoFinger = true;
		buttonWidget.mHasAlpha = true;
		buttonWidget.mHasTransparencies = true;
		int num = Common._DS(330);
		int num2 = Common._DS(140);
		int theX = (int)((float)inFrame.mX + (float)(inFrame.mWidth - num) * 0.5f);
		int theY = (int)((float)inFrame.mY + (float)(inFrame.mHeight - num2) * 0.5f);
		buttonWidget.Resize(theX, theY, num, num2);
		return buttonWidget;
	}

	protected void CreateChallengeScoreImage()
	{
		Font fontByID = Res.GetFontByID(ResID.FONT_SHAGEXOTICA100_GAUNTLET);
		if (mChallengePtsText != null)
		{
			if (mChallengePtsText.mImage != null)
			{
				mChallengePtsText.mImage.Dispose();
				mChallengePtsText.mImage = null;
			}
			mChallengePtsText = null;
		}
		mChallengePtsText = new FwooshImage();
		string theString = SexyFramework.Common.CommaSeperate(mScore);
		int num = fontByID.StringWidth(theString) + 5;
		mChallengePtsText.mImage = new DeviceImage();
		mChallengePtsText.mImage.mApp = mApp;
		mChallengePtsText.mImage.SetImageMode(hasTrans: true, hasAlpha: true);
		mChallengePtsText.mImage.AddImageFlags(16u);
		mChallengePtsText.mImage.Create(num + 15, fontByID.GetHeight() + 5);
		mChallengePtsText.mAlphaDec = 0f;
		mChallengePtsText.mMaxSize = Common._M(1.8f);
		mChallengePtsText.mSizeInc = Common._M(0.13f);
		Graphics graphics = new Graphics(mChallengePtsText.mImage);
		graphics.Get3D().ClearColorBuffer(new SexyFramework.Graphics.Color(0, 0));
		graphics.SetFont(fontByID);
		graphics.SetColor(SexyFramework.Graphics.Color.White);
		graphics.DrawString(theString, 5, fontByID.GetAscent());
		graphics.ClearRenderContext();
	}

	protected void CreateChallengeHeaderImage()
	{
		mChallengeHeaderText = new FwooshImage();
		mChallengeHeaderText.mMaxSize = Common._M(1.8f);
		mChallengeHeaderText.mSizeInc = Common._M(0.13f);
		if (mScore < mLevel.mChallengePoints)
		{
			if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_RU)
			{
				mChallengeHeaderText.mMaxSize = Common._M(0.6f);
				mChallengeHeaderText.mSizeInc = Common._M(0.008f);
			}
			mChallengeHeaderText.mImage = Res.GetImageByID(ResID.IMAGE_UI_GAUNTLET_TRYAGAIN) as MemoryImage;
		}
		else if (mScore < mLevel.mChallengeAcePoints)
		{
			mChallengeHeaderText.mImage = Res.GetImageByID(ResID.IMAGE_UI_GAUNTLET_SUCCESS) as MemoryImage;
		}
		else
		{
			mChallengeHeaderText.mImage = Res.GetImageByID(ResID.IMAGE_UI_GAUNTLET_VICTORY) as MemoryImage;
		}
		mChallengeHeaderText.mAlphaDec = 0f;
		mChallengeHeaderText.mDelay = (mChallengePtsText.mDelay = Common._M(20));
	}

	protected bool ShouldShowCheckpointPostcard()
	{
		return mLevel.mNum == 6;
	}

	protected void UpdateVortex(bool do_frog_fly_off)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_VORTEX_FACE1);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BOSS_VORTEX_FACE2);
		Image imageByID3 = Res.GetImageByID(ResID.IMAGE_BOSS_VORTEX_STREAK);
		if (mVortexAppear)
		{
			mVortexBGAlpha += Common._M(0.75f);
			if (mVortexBGAlpha >= 255f)
			{
				mVortexBGAlpha = 0f;
				mVortexAppear = false;
			}
		}
		if (!mVortexAppear && mUpdateCnt % Common._M(75) == 0)
		{
			VortexFace vortexFace = new VortexFace();
			mVortexFaces.Add(vortexFace);
			vortexFace.mUpdateCount = 0;
			vortexFace.mAngle = MathUtils.FloatRange(0f, (float)Math.PI * 2f);
			vortexFace.mPct = 0f;
			vortexFace.mImage = ((SexyFramework.Common.Rand() % 2 == 0) ? imageByID : imageByID2);
		}
		float num = Common._M(5f);
		float num2 = Common._M(0.035f);
		for (int i = 0; i < mVortexFaces.Count(); i++)
		{
			mVortexFaces[i].mUpdateCount++;
			if ((mVortexFaces[i].mPct += num2) > num)
			{
				mVortexFaces.RemoveAt(i);
				i--;
			}
			else
			{
				mVortexFaces[i].mAngle += Common._M(0.01f);
			}
		}
		int num3 = (GameApp.gApp.Is3DAccelerated() ? Common._M(2) : Common._M1(8));
		if (mUpdateCnt % num3 == 0)
		{
			VortexBeam vortexBeam = new VortexBeam();
			mVortexBeams.Add(vortexBeam);
			float num4 = MathUtils.Distance(mWidth / 2, mHeight / 2, Common._S(-80), 0f);
			vortexBeam.mPct = num4 / (float)imageByID3.mHeight;
			vortexBeam.mColor = new SexyFramework.Graphics.Color((int)mApp.HSLToRGB(SexyFramework.Common.Rand() % 255, 255, 128));
			float num5 = MathUtils.FloatRange(0f, (float)Math.PI * 2f);
			if (Math.Abs(num5 - UpdateVortex_last_angle) <= Common._M(0.35f))
			{
				num5 += Common._M(0.35f);
			}
			UpdateVortex_last_angle = num5;
			float num6 = MathUtils.FloatRange(Common._M(5f), Common._M1(10f));
			vortexBeam.mVX = num6 * (0f - (float)Math.Cos(num5));
			vortexBeam.mVY = num6 * (float)Math.Sin(num5);
			vortexBeam.mAngle = num5;
			vortexBeam.mX = (num4 + num4 * 0.5f) * (float)Math.Cos(num5);
			vortexBeam.mY = (num4 + num4 * 0.5f) * (0f - (float)Math.Sin(num5));
			vortexBeam.mPctDec = vortexBeam.mPct / ((num4 + num4 * 0.5f) / num6);
		}
		for (int j = 0; j < mVortexBeams.Count(); j++)
		{
			VortexBeam vortexBeam2 = mVortexBeams[j];
			vortexBeam2.mX += vortexBeam2.mVX;
			vortexBeam2.mY += vortexBeam2.mVY;
			vortexBeam2.mPct -= vortexBeam2.mPctDec;
			if (vortexBeam2.mPct <= 0f)
			{
				mVortexBeams.RemoveAt(j);
				j--;
			}
		}
		if (gNeedsVortexSound && (mFrogFlyOff == null || !do_frog_fly_off) && mVortexAppear)
		{
			gNeedsVortexSound = false;
		}
		if ((mFrogFlyOff == null || !do_frog_fly_off) && (!mVortexAppear || mVortexBGAlpha >= 255f))
		{
			float num7 = (mAdventureWinScreen ? Common._DS(Common._M(340)) : Common._DS(Common._M1(600)));
			float num8 = Common._M(500f);
			float num9 = num7 / num8;
			mVortexFrogAngle += Common._M(0.05f);
			if (mVortexFrogRadiusExpand)
			{
				if ((mVortexFrogRadius += Common._M(4)) >= num7)
				{
					mVortexFrogRadius = num7;
					mVortexFrogRadiusExpand = false;
				}
			}
			else if (mVortexFrogScale > 0f)
			{
				mVortexFrogRadius -= num9;
				mVortexFrogScale -= 1f / num8;
				if (mVortexFrogScale <= 0f)
				{
					mVortexFrogScale = 0f;
				}
			}
			else
			{
				if (!(mVortexBGAlpha < 255f) || mVortexAppear)
				{
					return;
				}
				mVortexBGAlpha += Common._M(1.5f);
				if (!(mVortexBGAlpha >= 255f))
				{
					return;
				}
				mVortexBGAlpha = 255f;
				mApp.PlaySong(144, Common._M(0.005f));
				if (do_frog_fly_off)
				{
					ContinueToNextLevel();
					if (mLevel.mBossIntroBG.GetImage() != null)
					{
						InitBossIntroState();
					}
					mGameState = GameState.GameState_FinalBossPart1Finished;
					mFrog.SetAngle(-(float)Math.PI);
					mFrogFlyOff = new FrogFlyOff();
					mFrogFlyOff.JumpIn(mFrog, mLevel.mFrog.GetCenterX(), mLevel.mFrog.GetCenterY(), continue_from_jump_out: false);
				}
			}
		}
		else if (mFrogFlyOff != null)
		{
			float num10 = 255f / (float)mFrogFlyOff.mFrogJumpTime;
			mFrogFlyOff.Update();
			mVortexBGAlpha -= num10;
			if (mVortexBGAlpha < 0f)
			{
				mVortexBGAlpha = 0f;
			}
			if (mFrogFlyOff.mTimer > mFrogFlyOff.mFrogJumpTime)
			{
				mGameState = GameState.GameState_Playing;
				mFrogFlyOff.Dispose();
				mFrogFlyOff = null;
				mVortexFaces.Clear();
				mVortexBeams.Clear();
				UpdateGunPos(level_begin: true, Common._SS(mWidth) / 2, Common._SS(mHeight) / 2);
			}
		}
	}

	protected void InitVortex()
	{
		mFrog.SetSlowTimer(Common._M(300));
		mVortexAppear = true;
		mVortexBGAlpha = 0f;
		mVortexFrogRadius = 0f;
		mVortexFrogAngle = mFrog.GetAngle();
		mVortexFrogScale = 1f;
		mVortexFrogRadiusExpand = true;
		gNeedsVortexSound = true;
	}

	protected void InitEndOfTorchLevel()
	{
		mFullScreenAlpha = 0;
		mFullScreenAlphaRate = Common._M(2);
		mLevel.mTorchStageState = 7;
		mLevel.mTorchStageAlpha = 0f;
		mLevel.mTorchStageTimer = Common._M(150);
		mContinueNextLevelOnLoadProfile = true;
	}

	protected void UpdateEndOfTorchLevel()
	{
		mLevel.Update(1f);
		if (mLevel.mTorchStageState == 9)
		{
			mFullScreenAlpha += mFullScreenAlphaRate;
			if (mFullScreenAlpha >= 255 && mFullScreenAlphaRate > 0)
			{
				mFullScreenAlpha = Common._M(305);
				mFullScreenAlphaRate *= -1;
				GameState gameState = mGameState;
				ContinueToNextLevel();
				mLevel.mTorchStageState = 9;
				mGameState = gameState;
				mContinueNextLevelOnLoadProfile = false;
			}
			else if (mFullScreenAlpha <= 0 && mFullScreenAlphaRate < 0)
			{
				mFullScreenAlphaRate = (mFullScreenAlpha = 0);
				if (mLevel.mFrogFlyOff == null)
				{
					mLevel.mTorchStageState = 10;
					mLevel.mFrogFlyOff = new FrogFlyOff();
					mLevel.mFrogFlyOff.JumpIn(mFrog, Common._SS(mWidth / 2) - GameApp.gApp.mBoardOffsetX, mFrog.GetCenterY(), continue_from_jump_out: false, Common._SS(mWidth / 2));
				}
			}
		}
		else if (mLevel.mTorchStageState == 13)
		{
			mGameState = GameState.GameState_Playing;
		}
	}

	protected void UpdateFinalBossPart1Finished()
	{
		UpdateEndOfTorchLevel();
	}

	protected void UpdateBoss6Transition()
	{
		if (mStateCount == 255)
		{
			ContinueToNextLevel();
			mStateCount = 255;
			mGameState = GameState.GameState_Boss6Transition;
		}
		else if (mStateCount == 510)
		{
			mGameState = GameState.GameState_Playing;
		}
	}

	protected void UpdateBoss6FakeCredits()
	{
		mFakeCredits.Update();
		if (mFakeCredits.CanStartNextLevel())
		{
			ContinueToNextLevel();
			mLevel.mBoss.SetX(Common._SS(mFakeCredits.mBossX) + Common._M(60));
			mGameState = GameState.GameState_Boss6FakeCredits;
		}
		else if (mFakeCredits.Done())
		{
			SetMenuBtnEnabled(enabled: true);
			mGameState = GameState.GameState_Playing;
			mFakeCredits = null;
			mApp.mMusic.PlaySong(127, 0.005f, inLoop: true, inForce: true);
			mHasDoneIntroSounds = false;
		}
	}

	protected void UpdateBoss6DarkFrog()
	{
		if (mBoss6VolcanoMelt != null)
		{
			mVolcanoBossEssence.mDrawTransform.LoadIdentity();
			float num = GameApp.DownScaleNum(1f);
			mVolcanoBossEssence.mDrawTransform.Scale(num, num);
			if (mBoss6VolcanoMelt.mUpdateCount >= gEssenceDrawFrame)
			{
				int num2 = Common._M(20);
				int num3 = Common._M(32);
				float num4 = 2.64f;
				float num5 = 0.28f;
				if (mEssenceScaleTimer < num2)
				{
					mEssenceXScale += (num4 - 0.74f) / (float)num2;
					mEssenceYScale += (num5 - 0.74f) / (float)num2;
				}
				else if (mEssenceScaleTimer < num3)
				{
					mEssenceXScale += (1f - num4) / (float)(num3 - num2);
					mEssenceYScale += (1f - num5) / (float)(num3 - num2);
				}
				else
				{
					mEssenceXScale = (mEssenceYScale = 1f);
				}
				mVolcanoBossEssence.mDrawTransform.Scale(mEssenceXScale, mEssenceYScale);
				mEssenceScaleTimer++;
			}
			mVolcanoBossEssence.mDrawTransform.Translate(mDarkFrogBulletX, mDarkFrogBulletY);
			mVolcanoBossEssence.Update();
			mBoss6VolcanoMelt.Update();
			if (mBoss6VolcanoMelt.Done())
			{
				if (mDarkFrogTimer <= Common._M(50))
				{
					mDarkFrogBulletX += mDarkFrogBulletVX;
					mDarkFrogBulletY += mDarkFrogBulletVY;
				}
				if (--mDarkFrogTimer == 0)
				{
					mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BOSS_DEVIL_ESSENCE_HIT));
					mBoss6VolcanoMelt.Dispose();
					mBoss6VolcanoMelt = null;
					UpdateVolcanoBossEssenceShit();
					mDarkFrogBulletX = 0f;
					mDarkFrogBulletY = 0f;
				}
			}
		}
		else
		{
			UpdateVolcanoBossEssenceShit();
			if (mDarkFrogSequence.mInitialDelay < mDarkFrogSequence.mInitialDelayTarget || mDarkFrogSequence.FadingOut())
			{
				mLevel.mBoss.Update();
			}
			mDarkFrogSequence.Update();
			if (mDarkFrogSequence.CanStartNextLevel())
			{
				float num6 = mFrog.GetCenterX();
				float num7 = mFrog.GetCenterY();
				ContinueToNextLevel();
				mGameState = GameState.GameState_Boss6DarkFrog;
				mFrog.SetPos((int)num6, (int)num7);
			}
			else if (mDarkFrogSequence.Done())
			{
				SetMenuBtnEnabled(enabled: true);
				mGameState = GameState.GameState_Playing;
				mDarkFrogSequence.Dispose();
				mDarkFrogSequence = null;
				mDrawBossUI = true;
				mApp.mMusic.PlaySong(127, 0.005f, inLoop: true, inForce: true);
			}
		}
	}

	protected void UpdateBoss6StoneHeadBurst()
	{
		mLevel.UpdateEffects();
		if (mBoss6StoneBurst != null)
		{
			BossStoneHead bossStoneHead = mLevel.mBoss as BossStoneHead;
			bool flag = true;
			if (bossStoneHead != null)
			{
				flag = bossStoneHead.UpdateDeathSequence();
			}
			if (!flag)
			{
				return;
			}
			mBoss6StoneBurst.Update();
			if (mBoss6StoneBurst.GetUpdateCount() == Common._M(35))
			{
				bossStoneHead.DoDeathRockExplosionThing();
			}
			if (mBoss6StoneBurst.Done())
			{
				ContinueToNextLevel();
				mBoss6StoneBurst.Dispose();
				mBoss6StoneBurst = null;
				mGameState = GameState.GameState_Boss6StoneHeadBurst;
				mSimpleFadeText.Clear();
				string[] array = new string[2]
				{
					TextManager.getInstance().getString(137),
					TextManager.getInstance().getString(138)
				};
				for (int i = 0; i < array.Count(); i++)
				{
					mSimpleFadeText.Add(new SimpleFadeText());
					SimpleFadeText simpleFadeText = mSimpleFadeText.Last();
					simpleFadeText.mString = array[i];
					simpleFadeText.mAlpha = ((i == 0) ? 255 : 0);
					simpleFadeText.mFadeIn = true;
					mStateCount = 0;
					ShakeScreen(Common._M(100), Common._M1(10), Common._M2(10));
				}
			}
		}
		else if (mStateCount == Common._M(400))
		{
			mSimpleFadeText.Last().mAlpha = 255f;
			ShakeScreen(Common._M(100), Common._M1(10), Common._M2(10));
		}
		else if (mStateCount == Common._M(800))
		{
			for (int j = 0; j < mSimpleFadeText.Count(); j++)
			{
				mSimpleFadeText[j].mFadeIn = false;
			}
		}
		else
		{
			if (mStateCount <= Common._M(800))
			{
				return;
			}
			for (int k = 0; k < mSimpleFadeText.size(); k++)
			{
				mSimpleFadeText[k].mAlpha -= Common._M(2f);
				if (mSimpleFadeText[k].mAlpha <= 0f)
				{
					mGameState = GameState.GameState_Playing;
				}
			}
		}
	}

	protected void UpdateBossDeath()
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_ADVENTURE_WIN_DOOR);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_ADVENTURE_WIN_CONTINUE_BUTTON);
		Image imageByID3 = Res.GetImageByID(ResID.IMAGE_LARGE_FROG);
		PIEffect pIEffectByID = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_TORCHFLAME);
		END_BOSS_FROG_JUMP_TIME = Common._M(50);
		mLevel.mBoss.Update();
		if (mStateCount == Common._M(400))
		{
			mLevel.mBoss.mDoDeathExplosions = false;
		}
		int num = mLevel.mBoss.mDeathText.size();
		if (num == 0)
		{
			return;
		}
		if (mLevel.mBoss.mDeathText[num - 1].mAlpha > 0f && mLevel.mFinalLevel && mAdventureWinScreen)
		{
			UpdateVortex(do_frog_fly_off: false);
			pIEffectByID.mDrawTransform.LoadIdentity();
			float num2 = GameApp.DownScaleNum(1f);
			pIEffectByID.mDrawTransform.Scale(num2, num2);
			pIEffectByID.mDrawTransform.Translate(Common._DS(Common._M(370)), Common._DS(Common._M1(300)));
			pIEffectByID.Update();
			if (!mVortexAppear && mVortexBGAlpha >= 255f)
			{
				if (mAdventureWinAlpha < 255f)
				{
					mAdventureWinAlpha += Common._M(3f);
					if (mAdventureWinAlpha >= 255f)
					{
						mAdventureWinAlpha = 255f;
					}
				}
				if (mAdventureWinAlpha >= 255f)
				{
					if (mAdventureWinTimer == (float)Common._M(0))
					{
						mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_STONE_DRAG));
					}
					if ((mAdventureWinTimer -= 1f) <= 0f)
					{
						mAdventureWinDoorYOff -= Common._M(2f);
						if (mAdventureWinDoorYOff <= (float)(-imageByID.mHeight))
						{
							mAdventureWinDoorYOff = -imageByID.mHeight;
							if (mAdventureWinExtraAlpha < 255f)
							{
								mAdventureWinExtraAlpha += Common._M(8f);
								if (mAdventureWinExtraAlpha >= 255f)
								{
									mAdventureWinExtraAlpha = 255f;
									mAdvWinBtn = new ButtonWidget(5, this);
									mAdvWinBtn.mButtonImage = (mAdvWinBtn.mOverImage = (mAdvWinBtn.mDownImage = imageByID2));
									mAdvWinBtn.mNormalRect = mAdvWinBtn.mButtonImage.GetCelRect(0);
									mAdvWinBtn.mOverRect = mAdvWinBtn.mButtonImage.GetCelRect(1);
									mAdvWinBtn.mDownRect = mAdvWinBtn.mButtonImage.GetCelRect(2);
									mAdvWinBtn.mDoFinger = true;
									mAdvWinBtn.mPriority = (mAdvWinBtn.mZOrder = int.MaxValue);
									int num3 = 15;
									if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CH || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CHT)
									{
										num3 = 25;
									}
									mAdvWinBtn.Resize(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_BOSS_DARKFROG_ADVENTURE_WIN_CONTINUE_BUTTON) + num3), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_BOSS_DARKFROG_ADVENTURE_WIN_CONTINUE_BUTTON)), mAdvWinBtn.mButtonImage.GetCelWidth(), mAdvWinBtn.mButtonImage.GetCelHeight());
									AddWidget(mAdvWinBtn);
								}
							}
							else if (mAdventureWinTimer <= (float)Common._M(0))
							{
								int num4 = mBeatGameLives * mApp.GetLevelMgr().mBeatGamePointsForLife;
								int num5 = (num4 + mBeatGameNormalScore) / Common._M(400);
								if (num5 == 0)
								{
									num5 = 1;
								}
								mBeatGameTotalScoreTally += num5;
								if (mBeatGameTotalScoreTally > mBeatGameNormalScore + num4)
								{
									mBeatGameTotalScoreTally = mBeatGameNormalScore + num4;
								}
							}
						}
					}
				}
			}
		}
		if (!mDoingEndBossFrogEffect)
		{
			return;
		}
		mEndBossFrogTimer++;
		if (mEndBossFrogTimer > END_BOSS_FROG_JUMP_TIME)
		{
			if (mBossSmokePoof.mFrameNum < (float)mBossSmokePoof.mLastFrameNum || mBossSmokePoof.mCurNumParticles > 0)
			{
				CheckForExtraLifeFromBoss();
				mBossSmokePoof.mDrawTransform.LoadIdentity();
				float num6 = GameApp.DownScaleNum(1f);
				mBossSmokePoof.mDrawTransform.Scale(num6, num6);
				mBossSmokePoof.mDrawTransform.Translate(Common._S(mLevel.mBoss.GetX() + Common._M(0)), Common._S(mLevel.mBoss.GetY() + Common._M1(0)));
				mBossSmokePoof.Update();
			}
			if (mBossSmokePoof.mFrameNum >= (float)Common._M(79) && mFrogFlyOff == null)
			{
				mFrogFlyOff = new FrogFlyOff();
				mFrogFlyOff.mFrogJumpTime = Common._M(200);
				mFrogFlyOff.JumpOut(mFrog, mWidth + Common._S(80) + imageByID3.mWidth, mHeight);
			}
		}
		else
		{
			float num7 = (float)(mLevel.mBoss.GetX() - FROG_DEATH_X + Common._M(0)) / (float)END_BOSS_FROG_JUMP_TIME;
			float num8 = (float)(mLevel.mBoss.GetY() - FROG_DEATH_Y + Common._M(20)) / (float)END_BOSS_FROG_JUMP_TIME;
			mFrog.ForceX(mFrog.GetCenterX() + (int)num7);
			mFrog.ForceY(mFrog.GetCenterY() + (int)num8);
		}
		UpdateBossOutTransition();
	}

	protected void UpdateBossOutTransition()
	{
		if (mFrogFlyOff == null)
		{
			return;
		}
		mFrogFlyOff.Update();
		if (mFrogFlyOff.mTimer <= 40)
		{
			return;
		}
		mEndBossFadeAmt += 5f;
		if (!(mEndBossFadeAmt < 265f))
		{
			if (mLivesInfo != null)
			{
				mLivesInfo.Dispose();
				mLivesInfo = null;
			}
			mEndBossFadeAmt = 255f;
			mDoingEndBossFrogEffect = false;
			ContinueToNextLevel();
			mFrogFlyOff.Dispose();
			mFrogFlyOff = null;
			if (mLevel.mZone <= 6)
			{
				SetupMapScreen(completed: true);
			}
			else
			{
				mEndBossFadeAmt = 0f;
			}
		}
	}

	protected bool DisplayingEndOfLevelStats()
	{
		int num = ((mLevelTransition == null) ? (-1) : mLevelTransition.GetState());
		if (mLevelTransition != null)
		{
			if (!mLevelTransition.mTransitionToStats || num < 1)
			{
				if (!mLevelTransition.mTransitionToStats)
				{
					return num <= 1;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	protected void DrawGauntletWidget(Graphics g)
	{
		if (mLevel.mMaxMultiplierTime <= 0)
		{
			return;
		}
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGE_GAUGE_EMPTY);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGE_GAUGE_FILL);
		Font fontByID = Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_STROKE);
		if (!GauntletMode() || (mLevel.mCurMultiplierTimeLeft <= 0 && !(mGauntletMultBarAlpha > 0f) && mStateCount >= mGauntletMultTextMoveLastFrame))
		{
			return;
		}
		float num = (float)mLevel.mCurMultiplierTimeLeft / (float)mLevel.mMaxMultiplierTime;
		int num2 = Common._S(mFrog.GetCurX()) - imageByID.mWidth / 2 + Common._DS(Common._M(0));
		int num3 = Common._S(mFrog.GetCurY()) - imageByID.mHeight / 2 + Common._DS(Common._M(-108));
		if (mGauntletMultBarAlpha > 0f || mLevel.mCurMultiplierTimeLeft > 0)
		{
			if ((int)mGauntletMultBarAlpha != 255)
			{
				g.SetColorizeImages(colorizeImages: true);
			}
			g.SetColor(255, 255, 255, (int)Math.Min(mGauntletMultBarAlpha, 255f));
			g.DrawImage(imageByID, num2, num3);
			int num4 = Common._M(150);
			if (mLevel.mCurMultiplierTimeLeft <= num4 && mGameState != GameState.GameState_Losing && mEndGauntletTimer <= 0 && mLevel.mCurMultiplierTimeLeft > 0 && mLevel.mCurMultiplierTimeLeft % Common._M(12) <= Common._M1(6))
			{
				g.SetDrawMode(1);
				g.DrawImage(imageByID, num2, num3);
				g.SetDrawMode(0);
			}
			if (num > 0f)
			{
				Rect theSrcRect = new Rect(0, 0, (int)((float)imageByID2.mWidth * num), imageByID2.mHeight);
				g.DrawImage(imageByID2, num2, num3, theSrcRect);
			}
			g.SetFont(fontByID);
			g.SetColor(Common._M(255), Common._M1(255), Common._M2(255), (int)Math.Max(0f, Math.Min(mGauntletMultBarAlpha, 255f)));
			g.DrawString(mScoreMultiplier + "x", num2 + Common._DS(Common._M(110)), num3 + Common._DS(Common._M1(20)));
			g.SetColorizeImages(colorizeImages: false);
		}
	}

	protected int GetAceTimeBonus()
	{
		if (mLevelStats.mTimePlayed > mLevel.mParTime + 99)
		{
			return 0;
		}
		int num = (int)(1f - (float)mLevelStats.mTimePlayed / (float)mLevel.mParTime) * Common._M(25000);
		num = num / 100 * 100;
		num += 100;
		if (num < 100)
		{
			num = 100;
		}
		return num;
	}

	protected void SetupMapScreen(bool completed, bool from_load)
	{
		mShowMapScreen = true;
		SetMenuBtnEnabled(enabled: false);
		if (from_load)
		{
			mEndBossFadeAmt = 0f;
		}
		if (!GameApp.gApp.mResourceManager.IsGroupLoaded("Map") && !GameApp.gApp.mResourceManager.LoadResources("Map"))
		{
			GameApp.gApp.ShowResourceError(doExit: true);
			GameApp.gApp.Shutdown();
		}
		mMapScreen.Init(completed, mLevel.mZone, mLevel.mNum, mWasShowingCheckpoint, from_load);
	}

	protected void SetupMapScreen(bool completed)
	{
		SetupMapScreen(completed, from_load: false);
	}

	protected void CheckIfGotExtraLife(int theInc)
	{
		if (GauntletMode() || IronFrogMode())
		{
			return;
		}
		while (theInc > 0)
		{
			if (mPointsLeftForExtraLife - theInc <= 0)
			{
				int num = mPointsLeftForExtraLife;
				mPointsLeftForExtraLife = mApp.GetLevelMgr().mPointsForLife;
				theInc -= num;
				LivesChanged(1);
			}
			else
			{
				mPointsLeftForExtraLife -= theInc;
				theInc = 0;
			}
		}
	}

	protected void DoReload()
	{
		for (int i = 0; i < mZumaTips.Count; i++)
		{
			mZumaTips[i] = null;
		}
		mZumaTips.Clear();
		GameApp.gDDS.Reset();
		string mId = mLevel.mId;
		mLevel.NukeEffects();
		mApp.ResetAllLevelMgrs();
		mScore = (mLevelBeginScore = 0);
		mRollerScore.Reset(mGauntletMode);
		if (!mApp.ReloadAllLevelMgrs())
		{
			mFrog.LevelReset();
		}
		else
		{
			Reset(game_over: false, level_reset: true, first_time_init: true, delete_bg: true);
			gCheatReload = true;
			StartLevel(mId);
			gCheatReload = false;
			UpdateGunPos(level_begin: true);
			DoAccuracy(accuracy: false);
		}
		MakeUIWidgets();
	}

	protected void MakeUIWidgets()
	{
		if (mMenuButton != null)
		{
			RemoveWidget(mMenuButton);
			mMenuButton.Dispose();
			mMenuButton = null;
		}
		mMenuButton = new ButtonWidget(1, this);
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_CHALLENGE_UI_PAUSE);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_CHALLENGE_UI_PAUSEDOWN);
		mMenuButton.mButtonImage = imageByID;
		mMenuButton.mDownImage = imageByID2;
		mMenuButton.Resize(mApp.GetWideScreenAdjusted(mMenuButtonX), 0, imageByID.GetWidth(), imageByID.GetHeight());
		mMenuButton.mDoFinger = true;
		AddWidget(mMenuButton);
	}

	protected void EraseTunnels()
	{
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < mTunnels[i].Count(); j++)
			{
				mTunnels[i][j].mImage = null;
			}
			mTunnels[i].Clear();
		}
	}

	protected void UpdateVolcanoBossEssenceShit()
	{
		if (mEssenceExplBottom.mFrameNum < (float)mEssenceExplBottom.mLastFrameNum)
		{
			mEssenceExplBottom.mDrawTransform.LoadIdentity();
			float num = GameApp.DownScaleNum(1f);
			mEssenceExplBottom.mDrawTransform.Scale(num, num);
			mEssenceExplBottom.mDrawTransform.Translate(Common._S(mDarkFrogSequence.GetMoveXAmt()), Common._S(mDarkFrogSequence.GetMoveYAmt()));
			mEssenceExplBottom.Update();
		}
		if (mEssenceExplTop.mFrameNum < (float)mEssenceExplTop.mLastFrameNum)
		{
			mEssenceExplTop.mDrawTransform.LoadIdentity();
			float num2 = GameApp.DownScaleNum(1f);
			mEssenceExplTop.mDrawTransform.Scale(num2, num2);
			mEssenceExplTop.mDrawTransform.Translate(Common._S(mDarkFrogSequence.GetMoveXAmt()), Common._S(mDarkFrogSequence.GetMoveYAmt()));
			mEssenceExplTop.Update();
		}
	}

	protected bool NeedsLillyPadHint()
	{
		if (mLevel.mNumFrogPoints > 1 && !mApp.mUserProfile.HasSeenHint(ZumaProfile.LILLY_PAD_HINT))
		{
			return mGameState == GameState.GameState_Playing;
		}
		return false;
	}

	protected void DoCheckpointEffect(bool game_over)
	{
		if (!game_over)
		{
			ToggleNotification(TextManager.getInstance().getString(432), Res.GetSoundByID(ResID.SOUND_MIDZONE_NOTIFY));
			if (mLevel.mBoss == null || mApp.IsHardMode() || mLevel.mZone > 1)
			{
				mPreventBallAdvancement = false;
			}
		}
		else
		{
			mCheckpointEffect = new Checkpoint(mLevel, GetCheckpointScore(), game_over);
			mApp.mUserProfile.GetAdvModeVars().mCurrentAdvScore = GetCheckpointScore();
			mApp.mUserProfile.GetAdvModeVars().mCurrentAdvLevel = mCheckpointEffect.mLevelNum - (mApp.mUserProfile.GetAdvModeVars().mCurrentAdvZone - 1) * 10;
			mCheckpointEffect.Resize(Common._S(-80), 0, mApp.mWidth + Common._S(mApp.mOffset160X), mApp.mHeight);
			AddWidget(mCheckpointEffect);
		}
	}

	protected void DoAccuracy(bool accuracy)
	{
		mDoGuide = (mShowGuide = (mRecalcLazerGuide = (mRecalcGuide = accuracy)));
		if (accuracy)
		{
			if ((mFrog.LaserMode() && mFrog.GetLazerCount() > 0) || mFrog.LightningMode() || mFrog.CannonMode())
			{
				mAccuracyBackupCount = mAccuracyCount;
				mAccuracyCount = 0;
				return;
			}
			if (mAccuracyBackupCount > 0)
			{
				mAccuracyBackupCount = mAccuracyCount;
			}
			mFrog.SetFireSpeed(19f);
			return;
		}
		if (mGuideBall != null)
		{
			mGuideBall.mHilightPulse = false;
		}
		mGuideBall = null;
		mAccuracyCount = 0;
		if (mLevel != null)
		{
			mFrog.SetFireSpeed(mLevel.mFireSpeed);
		}
	}

	protected void DestroyAllBalls()
	{
		mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_EXPLODE));
		for (int i = 0; i < mLevel.mNumCurves; i++)
		{
			mLevel.mCurveMgr[i].DetonateBalls();
		}
	}

	protected bool MakeBoss6StoneBurstComp()
	{
		mBoss6StoneBurst = new Composition();
		mBoss6StoneBurst.mLoadImageFunc = GameApp.CompositionLoadFunc;
		mBoss6StoneBurst.mPostLoadImageFunc = GameApp.CompositionPostLoadFunc;
		return mBoss6StoneBurst.LoadFromFile("pax\\BreakEasterIsland_FINAL");
	}

	protected bool MakeBoss6VolcanoMeltComp()
	{
		mVolcanoBossEssence = mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_BOSSESSENCE").Duplicate();
		mVolcanoBossEssence.mEmitAfterTimeline = true;
		mVolcanoBossEssence.ResetAnim();
		mEssenceExplTop = mApp.mResourceManager.GetPIEffect(mApp.Is3DAccelerated() ? "PIEFFECT_NONRESIZE_ESSENCEEXPLTOP" : "PIEFFECT_NONRESIZE_ESSENCEEXPLTOP2D").Duplicate();
		mEssenceExplTop.ResetAnim();
		mEssenceExplTop.GetLayer("Main").GetEmitter("Essence Area front").mMaskImage = mApp.mResourceManager.LoadImage("IMAGE_BOSS_DARKFROG_FRAME_4");
		mEssenceExplBottom = mApp.mResourceManager.GetPIEffect(mApp.Is3DAccelerated() ? "PIEFFECT_NONRESIZE_ESSENCEEXPLBOTTOM" : "PIEFFECT_NONRESIZE_ESSENCEEXPLBOTTOM2D").Duplicate();
		mEssenceExplBottom.ResetAnim();
		Common.SetFXNumScale(mEssenceExplTop, mApp.Is3DAccelerated() ? 1f : Common._M(0.3f));
		Common.SetFXNumScale(mEssenceExplBottom, mApp.Is3DAccelerated() ? 1f : Common._M(0.3f));
		mBoss6VolcanoMelt = new Composition();
		mBoss6VolcanoMelt.mLoadImageFunc = GameApp.CompositionLoadFunc;
		mBoss6VolcanoMelt.mPostLoadImageFunc = GameApp.CompositionPostLoadFunc;
		return mBoss6VolcanoMelt.LoadFromFile("pax\\Boss6_MELT");
	}

	protected void UpdateBullets()
	{
		int theBulletItr = 0;
		while (theBulletItr < mBulletList.Count)
		{
			AdvanceFreeBullet(ref theBulletItr);
		}
	}

	protected void UpdateGuide(bool lazer)
	{
		mGuideT = -1f;
		float num = mFrog.GetAngle() - 1.570795f;
		float num2 = (float)Math.Sin(num);
		float num3 = (float)Math.Cos(num);
		float num4 = num2;
		float num5 = num3;
		float num6 = num2 * (float)Common._M(28);
		float num7 = num3 * (float)Common._M(28);
		int num8 = Common._M(33);
		if (GameApp.gApp.Is3DAccelerated())
		{
			num4 *= (float)Common._M(15);
			num5 *= (float)Common._M(15);
		}
		SexyVector3 sexyVector = new SexyVector3((float)mFrog.GetCenterX() - (float)Common._M(-1) * num2 + (float)num8 * num3, (float)mFrog.GetCenterY() - (float)Common._M1(-1) * num3 - (float)num8 * num2, 0f);
		SexyVector3 sexyVector2 = new SexyVector3(sexyVector.x - num6, sexyVector.y - num7, 0f);
		SexyVector3 sexyVector3 = new SexyVector3(sexyVector.x + num6, sexyVector.y + num7, 0f);
		SexyVector3 sexyVector4 = new SexyVector3((float)Math.Cos(num), (float)(0.0 - Math.Sin(num)), 0f);
		int num9 = Common._M(5);
		SexyVector3 sexyVector5 = new SexyVector3(sexyVector.x + (float)num9 * num2, sexyVector.y, 0f);
		SexyVector3 sexyVector6 = new SexyVector3(sexyVector.x - (float)num9 * num2, sexyVector.y, 0f);
		SexyVector3 sexyVector7 = new SexyVector3(sexyVector.x, sexyVector.y, 0f);
		SexyVector3 p;
		SexyVector3 p2;
		if (lazer)
		{
			p = sexyVector5;
			p2 = sexyVector6;
		}
		else
		{
			p = sexyVector2;
			p2 = sexyVector3;
		}
		gPt1 = new SexyFramework.Misc.Point((int)p.x, (int)p.y);
		gPt2 = new SexyFramework.Misc.Point((int)p2.x, (int)p2.y);
		gCenter = new SexyFramework.Misc.Point((int)sexyVector7.x, (int)sexyVector7.y);
		mLazerHitTreasure = false;
		float t = 10000000f;
		Ball ball = null;
		if (Common.gSuckMode && mFrog.GetBullet() == null)
		{
			for (int i = 0; i < mLevel.mNumCurves; i++)
			{
				Ball ball2 = mLevel.mCurveMgr[i].CheckBallIntersection(sexyVector7, sexyVector4, ref t, mFrog.LaserMode());
				if (ball2 != null && !ball2.GetIsExploding())
				{
					ball = ball2;
				}
			}
		}
		else
		{
			for (int i = 0; i < mLevel.mNumCurves; i++)
			{
				Ball ball3 = mLevel.mCurveMgr[i].CheckBallIntersection(sexyVector7, sexyVector4, ref t, skip_exploding: true);
				if (ball3 != null && !ball3.GetIsExploding())
				{
					ball = ball3;
				}
			}
			if (Common._M(1) == 0 && ball == null)
			{
				for (int i = 0; i < mLevel.mNumCurves; i++)
				{
					Ball ball4 = mLevel.mCurveMgr[i].CheckBallIntersection(p, sexyVector4, ref t, skip_exploding: true);
					if (ball4 != null && !ball4.GetIsExploding())
					{
						ball = ball4;
					}
				}
				for (int i = 0; i < mLevel.mNumCurves; i++)
				{
					Ball ball5 = mLevel.mCurveMgr[i].CheckBallIntersection(p2, sexyVector4, ref t, skip_exploding: true);
					if (ball5 != null && !ball5.GetIsExploding())
					{
						ball = ball5;
					}
				}
			}
			if (mCurTreasure != null && !mTreasureWasHit && mFrog.LaserMode() && (LazerHitTreasure(p, sexyVector4, ref t) || LazerHitTreasure(p2, sexyVector4, ref t)))
			{
				mLazerHitTreasure = true;
			}
		}
		if (!mLazerHitTreasure)
		{
			if (ball == null)
			{
				t = 1000f / sexyVector4.Magnitude();
			}
			if (mGuideBall != null)
			{
				mGuideBall.mHilightPulse = false;
				mGuideBall.DoLaserAnim(d: false);
			}
			for (float num10 = 0f; num10 < t; num10 += Common._M(5f))
			{
				SexyVector3 sexyVector8 = sexyVector + sexyVector4 * num10;
				if (mLevel.PointIntersectsWall(sexyVector8.x, sexyVector8.y))
				{
					t = (mGuideT = num10);
					mGuideWallPoint = sexyVector8;
					ball = null;
					break;
				}
			}
			if (mFrog.LightningMode())
			{
				int num11 = ((mGuideBall != null) ? mGuideBall.GetColorType() : (-1));
				int num12 = ball?.GetColorType() ?? (-1);
				int num13 = ((mGuideBall != null) ? mLevel.GetOwningCurve(mGuideBall) : (-1));
				int num14 = ((ball != null) ? mLevel.GetOwningCurve(ball) : (-1));
				if (num11 != num12 || num13 != num14)
				{
					if (num11 != -1)
					{
						for (int j = 0; j < mLevel.mNumCurves; j++)
						{
							mLevel.mCurveMgr[j].ElectrifyBalls(num11, val: false);
						}
					}
					if (num12 != -1)
					{
						mLevel.mCurveMgr[num14].ElectrifyBalls(num12, val: true);
					}
				}
			}
			mGuideBall = ball;
			mGuideBallPoint = sexyVector7 + sexyVector4 * t;
			if (mGuideBall != null)
			{
				mGuideBall.mHilightPulse = false;
				if (mFrog.LaserMode())
				{
					mGuideBall.DoLaserAnim(d: true, mFrog);
				}
			}
		}
		else if (mGuideBall != null)
		{
			mGuideBall.mHilightPulse = false;
			mGuideBall.DoLaserAnim(d: false);
			mGuideBall = null;
		}
		SexyVector3 sexyVector9 = sexyVector + sexyVector4 * (t + Common._M(20f));
		SexyVector3 sexyVector10 = (lazer ? mLazerGuideCenter : mGuideCenter);
		if (!(lazer ? mRecalcLazerGuide : mRecalcGuide) && mShowGuide && (sexyVector10 - sexyVector9).Magnitude() < 20f)
		{
			return;
		}
		new SexyVector3(sexyVector.x + sexyVector4.x * t, sexyVector.y + sexyVector4.y * t, 0f);
		new SexyVector3(sexyVector.x + sexyVector4.x * t + num2 * (float)num9, sexyVector.y + sexyVector4.y * t, 0f);
		sexyVector10.CopyFrom(sexyVector9);
		mShowGuide = true;
		bool flag = false;
		if (lazer)
		{
			mLazerGuideCenter = sexyVector9;
			mRecalcLazerGuide = false;
		}
		else
		{
			mGuideCenter = sexyVector9;
			mRecalcGuide = false;
		}
		SexyFramework.Misc.Point[] array = (lazer ? mLazerGuide : mGuide);
		array[0].mX = (int)Common._S(p.x + num6 / 2f);
		array[0].mY = (int)Common._S(p.y + num7 / 2f);
		array[1].mX = (int)Common._S(p2.x - num6 / 2f);
		array[1].mY = (int)Common._S(p2.y - num7 / 2f);
		if (mApp.mGuideStyle == 0)
		{
			SexyVector3 sexyVector11 = sexyVector + sexyVector4 * (t * 0.95f + Common._M(20f));
			array[2].mX = (int)Common._S(sexyVector11.x);
			array[2].mY = (int)Common._S(sexyVector11.y);
			array[3].mX = (int)Common._S(sexyVector11.x);
			array[3].mY = (int)Common._S(sexyVector11.y);
		}
		else if (mApp.mGuideStyle == 1)
		{
			SexyVector3 sexyVector12;
			SexyVector3 sexyVector13;
			if ((new SexyVector3(mFrog.mCenterX, mFrog.mCenterY, 0f) - mGuideBallPoint).Magnitude() < mApp.mShotCorrectionAngleToWidthDist)
			{
				float num15 = mApp.mShotCorrectionAngleMax * 0.01745328f;
				sexyVector12 = sexyVector + new SexyVector3((float)Math.Cos(num + num15), 0f - (float)Math.Sin(num + num15), 0f) * (t + Common._M(20f));
				sexyVector13 = sexyVector + new SexyVector3((float)Math.Cos(num - num15), 0f - (float)Math.Sin(num - num15), 0f) * (t + Common._M(20f));
			}
			else
			{
				float mShotCorrectionWidthMax = mApp.mShotCorrectionWidthMax;
				sexyVector12 = sexyVector9 - new SexyVector3(num2 * mShotCorrectionWidthMax, num3 * mShotCorrectionWidthMax, 0f);
				sexyVector13 = sexyVector9 + new SexyVector3(num2 * mShotCorrectionWidthMax, num3 * mShotCorrectionWidthMax, 0f);
			}
			array[2].mX = (int)Common._S(sexyVector12.x);
			array[2].mY = (int)Common._S(sexyVector12.y);
			array[3].mX = (int)Common._S(sexyVector13.x);
			array[3].mY = (int)Common._S(sexyVector13.y);
		}
	}

	protected void UpdateTreasure()
	{
		if (mLevel.mNum == 1 && mLevel.mZone == 1 && !GauntletMode() && !IronFrogMode())
		{
			return;
		}
		if (mCurTreasure != null && mStateCount >= mTreasureEndFrame)
		{
			mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_FRUITDISAPPEARS));
			mCurTreasure = null;
			mCurTreasureNum = 0;
			return;
		}
		int num = (GauntletMode() ? mApp.GetLevelMgr().mGauntletTFreq : mLevel.mTreasureFreq);
		if (gForceTreasure || (mCurTreasure == null && (mScore < mScoreTarget || GauntletMode()) && mStateCount - mTreasureEndFrame > num))
		{
			if (mLevel.CheckFruitActivation(-1))
			{
				mApp.PlaySamplePan(Res.GetSoundByID(ResID.SOUND_TIKI_APPEAR), mApp.GetPan(mCurTreasure.x), 5);
				mTreasureEndFrame = mStateCount + (int)((float)TREASURE_LIFE * GameApp.gDDS.mHandheldBalance.mFruitPowerupAdditionalDuration);
				mTreasureStarAlpha = 255;
				mTreasureGlowAlpha = 0;
				mTreasureGlowAlphaRate = Common._M(12);
				mTreasureWasHit = false;
				mTreasureVY = (mTreasureDefaultVY = Common._M(0.25f));
				mMinTreasureY = (mMaxTreasureY = float.MaxValue);
				mTreasureYBob = 0f;
				mTreasureAccel = Common._M(-0.01f);
				mFruitBounceEffect.Reset();
			}
			gForceTreasure = false;
		}
	}

	protected void UpdateTreasureAnim()
	{
		if (mStateCount == mTreasureEndFrame - Common._M(200))
		{
			mTreasureGlowAlphaRate *= Common._M(4);
		}
		mTreasureStarAngle += Common._M(0.01f);
		if (mUpdateCnt % Common._M(3) == 0)
		{
			mTreasureCel = (mTreasureCel + 1) % (mFruitImg.mNumCols * mFruitImg.mNumRows);
		}
		if (mTreasureWasHit)
		{
			mFruitExplodeEffect.Update();
			if (mTreasureStarAlpha > 0)
			{
				mTreasureStarAlpha -= Common._M(8);
				if (mTreasureStarAlpha < 0)
				{
					mTreasureStarAlpha = 0;
				}
			}
			mTreasureGlowAlpha += mTreasureGlowAlphaRate;
			if (mTreasureGlowAlphaRate > 0 && mTreasureGlowAlpha >= 255)
			{
				mTreasureGlowAlpha = 255;
				mTreasureGlowAlphaRate *= -1;
			}
			else if (mTreasureGlowAlphaRate < 0 && mTreasureGlowAlpha <= 0)
			{
				mTreasureGlowAlphaRate = (mTreasureGlowAlpha = 0);
			}
			if (mFruitExplodeEffect.mDone)
			{
				mCurTreasure = null;
				mCurTreasureNum = 0;
			}
			return;
		}
		mTreasureVY += mTreasureAccel;
		if (mTreasureAccel < 0f && mTreasureVY <= 0f - mTreasureDefaultVY)
		{
			mTreasureAccel *= -1f;
			if (MathUtils._eq(mMinTreasureY, float.MaxValue, 1f))
			{
				mMinTreasureY = mTreasureYBob;
			}
			else
			{
				mTreasureYBob = mMinTreasureY;
			}
		}
		else if (mTreasureAccel > 0f && mTreasureVY > mTreasureDefaultVY)
		{
			mTreasureAccel *= -1f;
			if (MathUtils._eq(mMaxTreasureY, float.MaxValue, 1f))
			{
				mMaxTreasureY = mTreasureYBob;
			}
			else
			{
				mTreasureYBob = mMaxTreasureY;
			}
		}
		mTreasureYBob += mTreasureVY;
		mTreasureGlowAlpha += mTreasureGlowAlphaRate;
		if (mTreasureGlowAlphaRate > 0 && mTreasureGlowAlpha >= 255)
		{
			mTreasureGlowAlpha = 255;
			mTreasureGlowAlphaRate *= -1;
		}
		else if (mTreasureGlowAlphaRate < 0 && mTreasureGlowAlpha <= 0)
		{
			mTreasureGlowAlphaRate *= -1;
			mTreasureGlowAlpha = 0;
		}
	}

	protected void UpdateSuckMode()
	{
		if (mIsEndless)
		{
			return;
		}
		if (mLevel.mHaveReachedTarget && mDestroyCount == 0)
		{
			bool flag = false;
			for (int i = 0; i < 6; i++)
			{
				if (mBallColorMap[i] >= 3)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				mDestroyCount = 1;
			}
		}
		if (mDestroyCount > 0)
		{
			mDestroyCount++;
			if (mDestroyCount == 50)
			{
				DestroyAllBalls();
			}
		}
	}

	protected void UpdatePlaying()
	{
		ResetBallColorMap();
		if (mFrog != null && mApp.mDialogMap.Count == 0 && mGameState == GameState.GameState_Playing)
		{
			mFrog.GetBullet();
		}
		if (GauntletMode() && mNewBallDelay[0] > 0)
		{
			mNewBallDelay[0]--;
		}
		if (GauntletMode() && mNewBallDelay[1] > 0)
		{
			mNewBallDelay[1]--;
		}
		if (!mLevel.DoingInitialPathHilite() && mZumaTips.Count == 0 && !mPreventBallAdvancement && !IsPaused())
		{
			if (mAdventureMode)
			{
				AdvModeTempleStats advModeTempleStats = GetAdvModeTempleStats();
				advModeTempleStats.mCurrentTime++;
				advModeTempleStats.mTotalTimePlayed++;
			}
			else if (IronFrogMode())
			{
				mApp.mUserProfile.mIronFrogStats.mCurTime++;
				mApp.mUserProfile.mIronFrogStats.mTotalTimePlayed++;
			}
			else if (GauntletMode())
			{
				mApp.mUserProfile.mChallengeStats.mTotalTime++;
			}
		}
		if (mGauntletMultTextFlashTimer > 0)
		{
			mGauntletMultTextFlashTimer--;
			if (mGauntletMultTextFlashTimer % Common._M(5) == 0)
			{
				mGauntletMultTextFlashOn = !mGauntletMultTextFlashOn;
			}
		}
		if (mLevel.mCurMultiplierTimeLeft <= 0 && mGauntletMultBarAlpha > 0f)
		{
			mGauntletMultBarAlpha -= Common._M(5f);
		}
		float barPercent = mLevel.GetBarPercent();
		if (mCurrentSatPct < barPercent)
		{
			mCurrentSatPct += Common._M(0.01f);
			if (mCurrentSatPct > barPercent)
			{
				mCurrentSatPct = barPercent;
			}
		}
		if (mHallucinateTimer > 0)
		{
			mHallucinateTimer--;
		}
		if (mLevel.mNumFrogPoints > 1 && mIntroPadHopCount < Common._M(6) && !mFrog.IsHopping() && !NeedsLillyPadHint() && mLevel.DoingInitialPathHilite() && ++mLastIntroPadDelay >= Common._M1(25))
		{
			mIntroPadHopCount++;
			mLastIntroPadDelay = 0;
			mLastIntroPad = (mLastIntroPad + 1) % mLevel.mNumFrogPoints;
			mFrog.SetDestPos(mLevel.mFrogX[mLastIntroPad], mLevel.mFrogY[mLastIntroPad], mLevel.mMoveSpeed, doingHop: true);
			mLevel.mCurFrogPoint = mLastIntroPad;
		}
		PlayUnderwaterSound();
		Bullet bullet = null;
		while ((bullet = mFrog.GetFiredBullet()) != null)
		{
			AddFiredBullet(bullet);
			mLevel.BulletFired(bullet);
		}
		float num = Common._M(0.05f);
		int num2 = Common._M(2);
		float num3 = Common._M(2f);
		if (mCursorBlooms[0].mScale == 0f && mCursorBlooms[1].mScale == 0f)
		{
			mCursorBlooms[0].mScale += num;
			mCursorBlooms[0].mX = mApp.mWidgetManager.mLastMouseX;
			mCursorBlooms[0].mY = mApp.mWidgetManager.mLastMouseY;
		}
		else if (mCursorBlooms[0].mScale > mCursorBlooms[1].mScale)
		{
			mCursorBlooms[0].mScale += num;
			if (mCursorBlooms[0].mScale > num3)
			{
				mCursorBlooms[0].mAlpha -= num2;
				if (mCursorBlooms[1].mScale == 0f)
				{
					mCursorBlooms[1].mX = mApp.mWidgetManager.mLastMouseX;
					mCursorBlooms[1].mY = mApp.mWidgetManager.mLastMouseY;
				}
				mCursorBlooms[1].mScale += num;
				if (mCursorBlooms[1].mScale > num3)
				{
					mCursorBlooms[1].mAlpha -= num2;
				}
			}
			if (mCursorBlooms[0].mAlpha <= 0)
			{
				mCursorBlooms[0].Reset();
			}
		}
		else if (mCursorBlooms[1].mScale > mCursorBlooms[0].mScale)
		{
			mCursorBlooms[1].mScale += num;
			if (mCursorBlooms[1].mScale > num3)
			{
				mCursorBlooms[1].mAlpha -= num2;
				if (mCursorBlooms[0].mScale == 0f)
				{
					mCursorBlooms[0].mX = mApp.mWidgetManager.mLastMouseX;
					mCursorBlooms[0].mY = mApp.mWidgetManager.mLastMouseY;
				}
				mCursorBlooms[0].mScale += num;
				if (mCursorBlooms[0].mScale > num3)
				{
					mCursorBlooms[0].mAlpha -= num2;
				}
			}
			if (mCursorBlooms[1].mAlpha <= 0)
			{
				mCursorBlooms[1].Reset();
			}
		}
		UpdatePlayingFX();
		UpdateBullets();
		UpdateTreasure();
		if (mLevelBeginning)
		{
			bool flag = false;
			for (int i = 0; i < mLevel.mNumCurves; i++)
			{
				if (!mLevel.mCurveMgr[i].HasReachedCruisingSpeed())
				{
					flag = true;
				}
			}
			if (!flag || mStateCount > 500)
			{
				mLevelBeginning = false;
			}
		}
		bool flag2 = false;
		if (mDbgHurry)
		{
			for (int j = 0; j < 10; j++)
			{
				for (int i = 0; i < mLevel.mNumCurves; i++)
				{
					mLevel.mCurveMgr[i].UpdatePlaying();
					if (mDbgHurry && mLevel.mCurveMgr[i].IsInDanger())
					{
						flag2 = true;
					}
				}
			}
		}
		else if (gUpdateBalls)
		{
			mLevel.UpdatePlaying();
		}
		mLevel.Update(1f);
		if (!mApp.mUserProfile.HasSeenHint(ZumaProfile.FIRST_SHOT_HINT) && !GauntletMode() && mLevel.mZone == 1 && mLevel.mNum == 1 && mZumaTips.Count == 0 && mLevel.HasReachedCruisingSpeed())
		{
			mPreventBallAdvancement = true;
			ZumaTip zumaTip = new ZumaTip(cutout_region: new Rect(Common._DS(-22), Common._DS(350), 0, 0), text: TextManager.getInstance().getString(825), width: Common._S(Common._M(150)), height: Common._S(Common._M1(100)), id: ZumaProfile.FIRST_SHOT_HINT);
			zumaTip.mBlockUpdates = false;
			zumaTip.mClickDismiss = false;
			zumaTip.mDoArrowAnim = true;
			zumaTip.PointAt(Common._S(Common._M(43)), Common._S(Common._M1(268)), 3);
			mZumaTips.Add(zumaTip);
			zumaTip = new ZumaTip(cutout_region: new Rect(Common._S(190), 0, 0, 0), text: TextManager.getInstance().getString(826), width: Common._S(250), height: Common._S(175), id: ZumaProfile.ZUMA_BAR_HINT);
			zumaTip.mBlockUpdates = false;
			zumaTip.AutoPointAtCutoutRegion();
			mZumaTips.Add(zumaTip);
		}
		if (flag2)
		{
			mDbgHurry = false;
		}
		CheckEndConditions();
		CheckReload();
		if (Common.gSuckMode)
		{
			UpdateSuckMode();
		}
		mLevel.UpdateUI();
		if (GauntletMode() && mApp.GetLevelMgr().mGauntletSessionLength - mLevel.mGauntletCurTime == 1100)
		{
			AddText(TextManager.getInstance().getString(96), Common._SS(mWidth) / 2, Common._SS(mHeight) / 2, Common._M(2f), -1, Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_STROKE));
		}
		if (mRollingInDangerZone)
		{
			mLevelStats.mDangerTimePlayed++;
		}
	}

	protected void UpdateLosing()
	{
		if (mWasShowingCheckpoint)
		{
			return;
		}
		UpdatePlayingFX();
		if (mLevel.mBoss != null)
		{
			mLevel.mBoss.Update();
		}
		mLevel.mHoleMgr.Update();
		gMultTimeLeftDecAmt++;
		mLevel.mCurMultiplierTimeLeft -= gMultTimeLeftDecAmt;
		if (GauntletMode() && mGauntletMultBarAlpha > 0f)
		{
			mGauntletMultBarAlpha -= Common._M(5f);
		}
		mGauntletMultTextFlashTimer--;
		bool flag = true;
		for (int i = 0; i < mLevel.mNumCurves; i++)
		{
			mLevel.mCurveMgr[i].UpdateLosing();
			if (!mLevel.mCurveMgr[i].CanRestart())
			{
				flag = false;
			}
		}
		float angle = mFrog.GetAngle();
		if (mDeathSkull != null && flag)
		{
			mDeathSkull.Update();
			if (mLevel.mBoss != null)
			{
				mDeathSkull.mFrogTX = mLevel.mFrogX[mLevel.mCurFrogPoint] + mLevel.mBarWidth / 2;
				mDeathSkull.mFrogTY = mLevel.mFrogY[mLevel.mCurFrogPoint] + Common._M(6);
			}
			if (mDeathSkull.mDone)
			{
				if (mLevel.mBoss != null)
				{
					mFrog.ForceX(mLevel.mFrogX[mLevel.mCurFrogPoint] + mLevel.mBarWidth / 2);
					mFrog.ForceY(mLevel.mFrogY[mLevel.mCurFrogPoint] + Common._M(6));
				}
				angle = mDeathSkull.mLastFrogAngle;
				mDeathSkull.Dispose();
				mDeathSkull = null;
				delay_after_skull = Common._M(20);
			}
		}
		if (!flag || mStateCount <= 150 || mDeathSkull != null)
		{
			return;
		}
		mLevel.PlayerLostLevel();
		mHasDoneIntroSounds = false;
		if (!GauntletMode())
		{
			if (mLevel.mBoss != null)
			{
				gTuneNum = 6;
			}
			PlayLevelMusic(0.008f);
		}
		if (!mGauntletMode && !IronFrogMode())
		{
			if (mLevel.mBoss == null && !mLevel.IsFinalBossLevel())
			{
				if (!IsCheckpointLevel() || mLives > 3)
				{
					LivesChanged(-1);
				}
				else
				{
					mPreCheckpointLives = (mLives = 3);
				}
				mApp.mUserProfile.GetAdvModeVars().mDDSTier++;
				int num = (mLevel.mZone - 1) * 10 + mLevel.mNum - 1;
				GetAdvModeTempleStats().mLevelDeaths[num]++;
			}
			else if (mLevel.mBoss != null && !mLevel.IsFinalBossLevel())
			{
				GetAdvModeTempleStats().mBossDeaths[mLevel.mZone - 1]++;
			}
			if (mLevel.mBoss != null || mLevel.IsFinalBossLevel() || mLives > 0)
			{
				RestartLevel(from_checkpoint: false, mLevel);
				mFrog.SetAngle(angle);
				mApp.ClearUpdateBacklog(relaxForASecond: true);
				for (int j = 0; j < mLevel.mNumCurves; j++)
				{
					mLevel.mCurveMgr[j].mInkSpots.Clear();
					mLevel.mHoleMgr.GetHole(j).mDoDeathFade = false;
					for (int k = 0; k < 3; k++)
					{
						mLevel.mHoleMgr.GetHole(j).mRing[k].mAlpha = 0f;
					}
				}
				if (mLevel.mBoss != null)
				{
					mFrog.ForceX(mLevel.mFrogX[mLevel.mCurFrogPoint] + mLevel.mBarWidth / 2);
					mFrog.ForceY(mLevel.mFrogY[mLevel.mCurFrogPoint]);
				}
			}
			else
			{
				DoCheckpointEffect(game_over: true);
			}
		}
		else if (mGauntletMode && --delay_after_skull <= 0)
		{
			EndGauntletMode(hit_max_time: false);
		}
		else if (IronFrogMode())
		{
			mApp.DoGenericDialog("", "", block: false, null, 0);
			ZumaDialog zumaDialog = (ZumaDialog)mApp.GetDialog(0);
			ZumaDialogLine zumaDialogLine = new ZumaDialogLine();
			zumaDialogLine.mFont = Res.GetFontByID(ResID.FONT_SHAGEXOTICA38_BLACK_GLOW);
			zumaDialogLine.mColor = new SexyFramework.Graphics.Color(240, 117, 0);
			zumaDialogLine.mYPadding = Common._DS(Common._M(0));
			if (mLevel.mNum == 1)
			{
				zumaDialogLine.mLine = TextManager.getInstance().getString(130);
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder(TextManager.getInstance().getString(131));
				stringBuilder.Replace("$1", (mLevel.mNum - 1).ToString());
				zumaDialogLine.mLine = stringBuilder.ToString();
			}
			zumaDialog.mCustomLines.Add(zumaDialogLine);
			zumaDialogLine = new ZumaDialogLine();
			zumaDialogLine.mFont = Res.GetFontByID(ResID.FONT_SHAGEXOTICA38_BLACK_GLOW);
			zumaDialogLine.mColor = new SexyFramework.Graphics.Color(3, 239, 66);
			zumaDialogLine.mYPadding = Common._DS(Common._M(0));
			StringBuilder stringBuilder2 = new StringBuilder(TextManager.getInstance().getString(132));
			stringBuilder2.Replace("$1", SexyFramework.Common.CommaSeperate(mRollerScore.GetTargetScore()));
			zumaDialogLine.mLine = stringBuilder2.ToString();
			zumaDialog.mCustomLines.Add(zumaDialogLine);
			zumaDialogLine = new ZumaDialogLine();
			zumaDialogLine.mFont = Res.GetFontByID(ResID.FONT_SHAGEXOTICA38_BLACK_GLOW);
			zumaDialogLine.mColor = new SexyFramework.Graphics.Color(240, 117, 0);
			zumaDialogLine.mYPadding = Common._DS(Common._M(0));
			int num2 = mPrevIFBestScore;
			if (mRollerScore.GetTargetScore() > num2)
			{
				zumaDialogLine.mLine = TextManager.getInstance().getString(133);
			}
			else if (num2 == 0)
			{
				zumaDialogLine.mLine = TextManager.getInstance().getString(134);
			}
			else
			{
				StringBuilder stringBuilder3 = new StringBuilder(TextManager.getInstance().getString(135));
				stringBuilder3.Replace("$1", SexyFramework.Common.CommaSeperate(num2));
				zumaDialogLine.mLine = stringBuilder3.ToString();
			}
			zumaDialog.mCustomLines.Add(zumaDialogLine);
			int num3 = Common._DS(Common._M(700));
			int num4 = Common._DS(Common._M(760));
			zumaDialog.Resize((mWidth - num3) / 2, (mHeight - num4) / 2, num3, num4);
			zumaDialog.WaitForResult();
			mApp.DoDeferredEndGame();
		}
	}

	protected void UpdateMiscStuff()
	{
		mRollerScore.Update();
		if (mAccuracyCount > 0)
		{
			if (mGameState == GameState.GameState_Losing)
			{
				mAccuracyCount -= 3;
			}
			else if (mAccuracyBackupCount == 0)
			{
				mAccuracyCount--;
			}
			else
			{
				mAccuracyCount -= 2;
			}
			if (mAccuracyCount <= 0)
			{
				DoAccuracy(accuracy: false);
			}
		}
		if (mFlashAlpha > 0 && (mFlashAlpha -= Common._M(4)) < 0)
		{
			mFlashAlpha = 0;
		}
	}

	protected void UpdateBossIntro()
	{
		if (gNeedBossIntroSound && mBossIntroAlpha >= (float)Common._M(20))
		{
			gNeedBossIntroSound = false;
			mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_BOSS_BATTLE_INTRO));
		}
		if (mDoingBossIntroText)
		{
			if (--mBossIntroDelay <= 0)
			{
				if (mBossIntroFramesLeft > 0)
				{
					mBossIntroAlpha += mBossIntroAlphaRate;
					mBossTextY += mBossTextVY;
					mBattleTextY += mBattleTextVY;
				}
				mBossIntroFramesLeft--;
				if (mBossIntroDirection == 1 && mBossIntroFramesLeft <= 0)
				{
					for (int i = 0; i < mSmokeParticles.size(); i++)
					{
						if (BambooTransition.UpdateSmokeParticle(mSmokeParticles[i]))
						{
							mSmokeParticles[i] = null;
							mSmokeParticles.RemoveAt(i);
							i--;
						}
					}
					mBossIntroAlpha = 255f;
					if (mSmokeParticles.size() == 0)
					{
						ulong num = SexyFramework.Common.SexyTime();
						ContinueToNextLevel();
						if (mLevel.mBossIntroBG.GetImage() != null)
						{
							InitBossIntroState();
						}
						ulong num2 = SexyFramework.Common.SexyTime();
						mApp.ClearUpdateBacklog(relaxForASecond: true);
						int num3 = Common._M(150);
						if (num2 - num < (ulong)(10 * num3))
						{
							mBossIntroDelay = num3 - (int)(num2 - num) / 10;
						}
						mDoingBossIntroText = true;
						mBossIntroDirection = -1;
						mBossTextVY *= -1f;
						mBattleTextVY *= -1f;
						mBossIntroAlphaRate *= -1f;
						mBossIntroFramesLeft = Common._M(20);
						mContinueNextLevelOnLoadProfile = false;
					}
				}
				else if (mBossIntroDirection == -1 && mBossIntroFramesLeft == 0)
				{
					mDoingBossIntroText = false;
					mGameState = GameState.GameState_Playing;
					SetMenuBtnEnabled(enabled: true);
					if (mLevelTransition != null)
					{
						mLevelTransition.Dispose();
					}
					mLevelTransition = null;
				}
			}
		}
		else if (mDoingBossIntroFightText)
		{
			mFightImage.Update();
			mBossIntroAlpha += mBossIntroAlphaRate;
			if (mFightImage.mForward)
			{
				if (mFightImage.mDelay <= 0)
				{
					mFightImage.Reverse();
					mBossIntroAlphaRate *= -1f;
					mBossIntroAlpha = 255f;
				}
			}
			else if (mFightImage.mSize <= 0f)
			{
				mDoingBossIntroFightText = false;
				mGameState = GameState.GameState_Playing;
				SetMenuBtnEnabled(enabled: true);
			}
		}
		mLevel.UpdateBossIntro();
	}

	protected void UpdateBeatLevelBonus()
	{
		UpdatePlayingFX();
		AnimateBonus100s();
		if (mEndLevelExplosions.size() != 0)
		{
			return;
		}
		if (Common.StrEquals(mLevel.mId, mApp.GetLevelMgr().GetLevelId(mApp.GetLevelMgr().GetLastIronFrogLevel())))
		{
			mLevelPoints = mScore - mLevelBeginScore;
			GetBetaStats().BeatLevel(mLevelStats.mTimePlayed, mLevel.mParTime, GetAceTimeBonus(), GetPerfectBonus(), (float)mLevel.mFurthestBallDistance / 100f, mScore - mLevelBeginScore, mScore, -1);
			if (mScore > mApp.mUserProfile.mHighestIronFrogScore)
			{
				mNewIronFrogHS = true;
				mApp.mUserProfile.mHighestIronFrogScore = mScore;
				mApp.mUserProfile.mHighestIronFrogLevel = mApp.GetLevelMgr().GetLevelIndex(mLevel.mId) - mApp.GetLevelMgr().GetFirstIronFrogLevel() + 1;
			}
			mApp.SaveProfile();
			mDoingIronFrogWin = true;
			mIronFrogAlpha = 0f;
			mIronFrogBtn = new ExtraSexyButton(3234, this);
			mIronFrogBtn.mUsesAnimators = false;
			mIronFrogBtn.mNormalRect = mIronFrogBtn.mButtonImage.GetCelRect(0);
			mIronFrogBtn.mOverRect = mIronFrogBtn.mButtonImage.GetCelRect(1);
			mIronFrogBtn.mDownRect = mIronFrogBtn.mButtonImage.GetCelRect(2);
			mIronFrogBtn.mDoFinger = true;
			mIronFrogBtn.SetVisible(isVisible: false);
			mIronFrogBtn.SetDisabled(isDisabled: true);
			mIronFrogBtn.mBtnNoDraw = true;
			mIronFrogWinDelay = Common._M(10);
			AddWidget(mIronFrogBtn);
			SetMenuBtnEnabled(enabled: false);
			return;
		}
		bool theAcedLevel = false;
		if (GauntletMode())
		{
			if (mScore > mLevel.mChallengeAcePoints)
			{
				theAcedLevel = true;
			}
		}
		else if (mEndLevelStats.mTimePlayed < mLevel.mParTime)
		{
			theAcedLevel = true;
		}
		GameApp.gApp.ReportEndOfLevelMetrics(this, theLevelSuccess: true, theAcedLevel);
		CueLevelTransition();
	}

	protected void UpdateHole()
	{
	}

	protected void AdvanceFreeBullet(ref int theBulletItr)
	{
		Bullet bullet = mBulletList[theBulletItr];
		for (int i = 0; i < 2; i++)
		{
			if (bullet.GetJustFired())
			{
				bullet.SetJustFired(fired: false);
			}
			else
			{
				if (i == 0)
				{
					continue;
				}
				bullet.Update(1f);
			}
			if (mCurTreasure != null && !mTreasureWasHit)
			{
				float num = bullet.GetX() - (float)(mCurTreasure.x + Common._SS(mFruitImg.GetCelWidth()) / 2);
				float num2 = bullet.GetY() - ((float)(mCurTreasure.y + Common._SS(mFruitImg.GetCelHeight()) / 2) + mTreasureYBob);
				float num3 = bullet.GetRadius() + (Common._SS(mFruitImg.GetCelHeight()) / 2 - Common._M(0));
				if (num * num + num2 * num2 < num3 * num3)
				{
					DoHitTreasure();
					mLevel.BulletHit(bullet);
					if (!bullet.GetIsCannon())
					{
						bullet.Dispose();
						mBulletList.RemoveAt(theBulletItr);
						return;
					}
				}
			}
			if (gCheckCollision)
			{
				for (int j = 0; j < mLevel.mNumCurves; j++)
				{
					if (mLevel.mCurveMgr[j].CheckCollision(bullet))
					{
						mLevel.BulletHit(bullet);
						if (bullet.GetIsCannon())
						{
							break;
						}
						bullet.Dispose();
						mBulletList.RemoveAt(theBulletItr);
						return;
					}
				}
				if (mLevel.CollidedWithWall(bullet))
				{
					mApp.mUserProfile.mBallsTossed++;
					mLevel.BulletHit(bullet);
					ResetInARowBonus();
					bullet.Dispose();
					mBulletList.RemoveAt(theBulletItr);
					return;
				}
				if (mLevel.mBoss != null && mLevel.mBoss.AllowFrogToFire() && mLevel.mBoss.Collides(bullet))
				{
					mLevel.BulletHit(bullet);
					ResetInARowBonus();
					bullet.Dispose();
					mBulletList.RemoveAt(theBulletItr);
					return;
				}
			}
			for (int j = 0; j < mLevel.mNumCurves; j++)
			{
				mLevel.mCurveMgr[j].CheckGapShot(bullet);
			}
			int num4 = 800;
			int num5 = 600;
			if (bullet.GetX() < (float)Common._S(-80) || bullet.GetY() < 0f || bullet.GetX() - (float)bullet.GetRadius() > (float)(num4 + Common._S(80)) || bullet.GetY() - (float)bullet.GetRadius() > (float)num5)
			{
				if (!bullet.GetIsCannon())
				{
					mApp.mUserProfile.mBallsTossed++;
					ResetInARowBonus();
				}
				if (Common.gSuckMode && !bullet.GetIsCannon() && mLevel.mHaveReachedTarget && !mIsEndless && mLevel.mNumCurves > 0)
				{
					int num6 = MathUtils.SafeRand() % mLevel.mNumCurves;
					Ball ball = new Ball();
					ball.SetColorType(bullet.GetColorType());
					ball.SetPowerType(bullet.GetPowerType(), delay: false);
					ball.SetSpeedy(speedy: true);
					mLevel.mCurveMgr[num6].AddPendingBall(ball);
				}
				mLevelStats.mNumMisses++;
				bullet.Dispose();
				mBulletList.RemoveAt(theBulletItr);
				return;
			}
			mBallColorMap[bullet.GetColorType()]++;
		}
		theBulletItr++;
	}

	protected void PlayUnderwaterSound()
	{
		if (mLevel.mZone == 5 && mUpdateCnt - gLastUnderwaterSound > 1000 && MathUtils.SafeRand() % 2500 == 0)
		{
			int id = 1658 + SexyFramework.Common.Rand() % 3;
			mApp.PlaySample(Res.GetSoundByID((ResID)id));
			gLastUnderwaterSound = mUpdateCnt;
		}
	}

	protected void AnimateBonus100s()
	{
		bool flag = false;
		for (int i = 0; i < mEndLevelExplosions.Count; i++)
		{
			EndLevelExplosion endLevelExplosion = mEndLevelExplosions[i];
			if (endLevelExplosion.mDelay > 0)
			{
				flag = true;
				StartBonus(endLevelExplosion);
			}
			else if (EndBonus(endLevelExplosion))
			{
				mEndLevelExplosionPool.Free(endLevelExplosion);
				mEndLevelExplosions.Remove(endLevelExplosion);
				mEffectBatch.Remove(endLevelExplosion.mPIEffect);
				i--;
			}
		}
		if (!flag && mApp.mSoundPlayer.IsLooping(Res.GetSoundByID(ResID.SOUND_BONUS100LOOP)))
		{
			mApp.mSoundPlayer.Stop(Res.GetSoundByID(ResID.SOUND_BONUS100LOOP));
		}
	}

	protected void StartBonus(EndLevelExplosion inBonus)
	{
		if (--inBonus.mDelay == 0)
		{
			int num = 100;
			IncScore(num, from_balls: false);
			mRollerScore.ForceScore(mScore + mCurveClearBonus);
			AddText($"+{num}", inBonus.mX, inBonus.mY);
		}
	}

	protected bool EndBonus(EndLevelExplosion inBonus)
	{
		inBonus.mPIEffect.Update();
		if (inBonus.mPIEffect.IsActive())
		{
			return false;
		}
		return true;
	}

	protected void InitBossIntroState()
	{
		mGameState = GameState.GameState_BossIntro;
		mStateCount = 0;
		mMenuButton.SetDisabled(isDisabled: true);
		if (mSwapBallButton != null)
		{
			mSwapBallButton.SetDisabled(isDisabled: true);
		}
		if (mLevel.mZone == 6)
		{
			if (mBoss6StoneBurst != null)
			{
				mBoss6StoneBurst.Dispose();
				mBoss6StoneBurst = null;
			}
			MakeBoss6StoneBurstComp();
		}
		if (mLevel.mBoss != null && mLevel.mBoss.mSepiaImage == null && mLevel.mBoss.mSepiaImagePath.Length > 0)
		{
			mLevel.mBoss.mSepiaImage = mApp.GetImage(mLevel.mBoss.mSepiaImagePath, commitBits: true, allowTriReps: true, isInAtlas: false);
		}
	}

	protected void SetLosing(int from_curve)
	{
		GameApp.gApp.mSoundPlayer.Fade(Res.GetSoundByID(ResID.SOUND_NEW_BURNINGFROGLOOP));
		if (!GauntletMode())
		{
			mApp.PlaySong(126, Common._M(0.006f));
		}
		if (IronFrogMode())
		{
			mApp.mUserProfile.mIronFrogStats.mLevelDeaths[mLevel.mNum - 1]++;
		}
		if (mAdventureMode && HasAchievedZuma())
		{
			mApp.mUserProfile.mDeathsAfterZuma++;
		}
		mPreventBallAdvancement = false;
		mDeathSkull = null;
		if (from_curve != -1)
		{
			mDeathSkull = new DeathSkull();
			HoleInfo hole = mLevel.mHoleMgr.GetHole(from_curve);
			mDeathSkull.Init(hole.mRotation + (float)Math.PI, hole.mX, hole.mY, mFrog.GetCenterX(), mFrog.GetCenterY());
			if (mLevel.mBoss != null)
			{
				((BossShoot)mLevel.mBoss).DeleteAllBullets();
			}
			mLevel.mInvertMouseTimer = 0;
			mHallucinateTimer = 0;
			DoAccuracy(accuracy: false);
			mFrog.ClearStun();
		}
		mCurTreasure = null;
		mCurTreasureNum = 0;
		mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_GAME_OVER));
		ResetInARowBonus();
		mLevelStats.mTimePlayed = mStateCount - mIgnoreCount;
		mGameStats.Add(mLevelStats);
		bool theAcedLevel = false;
		if (GauntletMode() && mScore > mLevel.mChallengeAcePoints)
		{
			theAcedLevel = true;
		}
		mWasPerfectLevel = false;
		GameApp.gApp.ReportEndOfLevelMetrics(this, theLevelSuccess: false, theAcedLevel);
		mLevelStats.Reset();
		DeleteBullets();
		mShowGuide = false;
		mFrog.EmptyBullets();
		mFrog.mFrogStack.Clear();
		mFrog.PlayerDied();
		mAccuracyBackupCount = 0;
		if (mAccuracyCount > 300)
		{
			mAccuracyCount = 300;
		}
		if (!GauntletMode())
		{
			mApp.mUserProfile.GetAdvModeVars().mNumDeathsCurLevel++;
		}
		for (int i = 0; i < mLevel.mNumCurves; i++)
		{
			mLevel.mHoleMgr.GetHole(i).mDoDeathFade = true;
			mLevel.mCurveMgr[i].SetLosing();
		}
		if (mLevel.mBoss != null)
		{
			GameApp.gDDS.UserLostBossLevel(mLevel.mBoss.GetHP());
		}
		int challenge_multiplier = (GauntletMode() ? mScoreMultiplier : (-1));
		GetBetaStats().DiedOnLevel(mStateCount, mScore - mLevelBeginScore, GauntletMode() ? mRollerScore.GetTargetScore() : mScore, mLives - 1, -1, challenge_multiplier, (int)((mLevel.mBoss != null) ? mLevel.mBoss.GetHP() : 0f));
		mGameState = GameState.GameState_Losing;
		mStateCount = 0;
		if (mLevel.mCurMultiplierTimeLeft > 0 && GauntletMode())
		{
			GauntletMultiplierEnded();
			mGauntletMultBarAlpha = Common._M(700);
		}
		if (mGauntletMode)
		{
			mApp.SaveProfile();
			if (mRollerScore.GetTargetScore() > mApp.mUserProfile.mChallengeStats.mHighestScore)
			{
				mApp.mUserProfile.mChallengeStats.mHighestScore = mRollerScore.GetTargetScore();
			}
		}
		else if (IronFrogMode() && mScore > mApp.mUserProfile.mHighestIronFrogScore)
		{
			mNewIronFrogHS = true;
			mApp.mUserProfile.mHighestIronFrogScore = mScore;
			mApp.mUserProfile.mHighestIronFrogLevel = mApp.GetLevelMgr().GetLevelIndex(mLevel.mId) - mApp.GetLevelMgr().GetFirstIronFrogLevel() + 1;
			mApp.SaveProfile();
		}
	}

	protected void SetLosing()
	{
		SetLosing(-1);
	}

	protected void SetupLevelText()
	{
		for (int i = 0; i < mLevelNameText.Length; i++)
		{
			if (mLevelNameText[i] != null)
			{
				if (mLevelNameText[i].mImage != null)
				{
					mLevelNameText[i].mImage.Dispose();
				}
				mLevelNameText[i].mImage = null;
			}
		}
		if (mLevel.mBoss == null && !IronFrogMode() && mLevel.mNum < int.MaxValue && !GauntletMode())
		{
			FwooshImage fwooshImage = mLevelNameText[0];
			string text = "";
			text = ((!GauntletMode()) ? (TextManager.getInstance().getString(136) + "  " + ((mLevel.mZone - 1) * 10 + mLevel.mNum)) : ("x" + mScoreMultiplier + "!"));
			Font fontByID = Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_STROKE);
			Font fontByID2 = Res.GetFontByID(ResID.FONT_SHAGEXOTICA100_STROKE);
			fwooshImage.mDelay = Common._M(200);
			int num = fontByID.StringWidth(text) + 5;
			fwooshImage.mImage = new DeviceImage();
			fwooshImage.mImage.mApp = mApp;
			fwooshImage.mImage.SetImageMode(hasTrans: true, hasAlpha: true);
			fwooshImage.mImage.AddImageFlags(16u);
			fwooshImage.mImage.Create(num + 15, fontByID.GetHeight() + 5);
			fwooshImage.mAlpha = 255f;
			fwooshImage.mIncText = true;
			fwooshImage.mSize = 0f;
			fwooshImage.mDelay = 0;
			if (!GauntletMode())
			{
				fwooshImage.mX = mWidth / 2;
				fwooshImage.mY = mHeight - Common._M(10) - fontByID2.mHeight / 2 - Common._M1(40) - fontByID2.mHeight / 4;
			}
			else
			{
				fwooshImage.mX = Common._M(50);
				fwooshImage.mY = Common._M(100);
			}
			Graphics graphics = new Graphics(fwooshImage.mImage);
			graphics.Get3D().ClearColorBuffer(new SexyFramework.Graphics.Color(0, 0, 0, 0));
			graphics.SetFont(fontByID);
			graphics.SetColor(SexyFramework.Graphics.Color.White);
			graphics.DrawString(text, 5, fontByID.GetAscent());
			graphics.ClearRenderContext();
			if (!GauntletMode())
			{
				FwooshImage fwooshImage2 = mLevelNameText[1];
				text = mLevel.mDisplayName;
				num = fontByID2.StringWidth(text);
				fwooshImage2.mDelay = Common._M(200);
				fwooshImage2.mImage = new DeviceImage();
				fwooshImage.mImage.mApp = mApp;
				fwooshImage2.mImage.SetImageMode(hasTrans: true, hasAlpha: true);
				fwooshImage2.mImage.AddImageFlags(16u);
				fwooshImage2.mImage.Create(num + 15, fontByID2.GetHeight() + 5);
				fwooshImage2.mAlpha = 255f;
				fwooshImage2.mIncText = true;
				fwooshImage2.mSize = 0f;
				fwooshImage2.mDelay = 0;
				fwooshImage2.mX = mWidth / 2;
				if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CH || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CHT)
				{
					fwooshImage2.mY = mHeight - Common._M(15) - fontByID2.mHeight / 2;
				}
				else
				{
					fwooshImage2.mY = mHeight - Common._M(25) - fontByID2.mHeight / 2;
				}
				Graphics graphics2 = new Graphics(fwooshImage2.mImage);
				graphics2.Get3D().ClearColorBuffer(new SexyFramework.Graphics.Color(0, 0, 0, 0));
				graphics2.SetFont(fontByID2);
				graphics2.SetColor(SexyFramework.Graphics.Color.White);
				graphics2.DrawString(text, 5, fontByID2.GetAscent());
				graphics2.ClearRenderContext();
			}
		}
		else if (mLevel.mBossIntroBG != null && mLevel.mBossIntroBG.GetImage() != null)
		{
			gNeedBossIntroSound = true;
		}
	}

	protected void SetupTunnels(Level theLevel)
	{
		for (int i = 0; i < theLevel.mTunnelData.Count(); i++)
		{
			int num = theLevel.mTunnelData[i].mPriority;
			Tunnel tunnel = new Tunnel();
			mTunnels[num].Add(tunnel);
			TunnelData tunnelData = theLevel.mTunnelData[i];
			if (tunnelData.mImageName.Length > 0)
			{
				string theFileName = mApp.GetResImagesDir() + "levels/" + theLevel.mId + "/" + theLevel.mTunnelData[i].mImageName;
				tunnel.mImage = mApp.GetImage(theFileName, commitBits: true, allowTriReps: true, isInAtlas: false);
				tunnel.mX = theLevel.mTunnelData[i].mX;
				tunnel.mY = theLevel.mTunnelData[i].mY;
			}
			else
			{
				SharedImageRef sharedImageRef = mApp.mResourceManager.LoadImage(tunnelData.mLayerId);
				if (sharedImageRef != null)
				{
					tunnel.mImage = sharedImageRef.GetImage();
				}
				tunnel.mLayerId = tunnelData.mLayerId;
				string text = tunnelData.mLayerId;
				int num2 = text.IndexOf("BOSS6PART4");
				if (num2 != -1)
				{
					text = text.Substring(0, 8) + '3' + text.Substring(10);
				}
				SexyFramework.Misc.Point imageOffset = mApp.mResourceManager.GetImageOffset(text);
				tunnel.mX = imageOffset.mX;
				tunnel.mY = imageOffset.mY;
			}
			tunnel.mAboveShadows = tunnelData.mAboveShadows;
		}
	}

	protected void DrawBoss6FakeCredits(Graphics g)
	{
		if (!mFakeCredits.HasClosedScene())
		{
			mLevel.mBoss.Draw(g);
		}
		mFakeCredits.Draw(g);
	}

	protected void DrawBoss6DarkFrog(Graphics g)
	{
		if (mBoss6VolcanoMelt != null)
		{
			if (mBoss6VolcanoMelt.Done() || mBoss6VolcanoMelt.mUpdateCount >= gEssenceDrawFrame)
			{
				mVolcanoBossEssence.Draw(g);
			}
			if (!mBoss6VolcanoMelt.Done())
			{
				int theTransX = -(Common._DS(mBoss6VolcanoMelt.mWidth) / 2 - Common._S(mLevel.mBoss.GetX())) + Common._S(Common._M(5));
				int theTransY = -(Common._DS(mBoss6VolcanoMelt.mHeight) / 2 - Common._S(mLevel.mBoss.GetY())) + Common._S(Common._M1(35));
				g.PushState();
				g.Translate(theTransX, theTransY);
				mBoss6VolcanoMelt.Draw(g, null, -1, Common._DS(1f));
				g.PopState();
				return;
			}
			int num = mDarkFrogTimer;
			if (num > 255)
			{
				num = 255;
			}
			if (num > 0)
			{
				Font fontByID = Res.GetFontByID(ResID.FONT_BOSS_TAUNT);
				if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CH)
				{
					fontByID.mAscent = 25;
				}
				g.SetFont(fontByID);
				g.SetColor(0, 0, 0, num);
				string[] array = new string[3];
				if (IsHardAdventureMode())
				{
					array[0] = TextManager.getInstance().getString(108);
					array[1] = TextManager.getInstance().getString(109);
					array[2] = TextManager.getInstance().getString(110);
				}
				else
				{
					array[0] = TextManager.getInstance().getString(106);
					array[1] = TextManager.getInstance().getString(107);
					array[2] = "";
				}
				for (int i = 0; i < array.Length; i++)
				{
					g.WriteString(array[i], -GameApp.gApp.mBoardOffsetX, Common._DS(Common._M(550)) + g.GetFont().GetHeight() * i, mWidth);
				}
			}
		}
		else
		{
			if (mDarkFrogSequence.FadingOut())
			{
				mLevel.mBoss.Draw(g);
			}
			g.PushState();
			mEssenceExplBottom.Draw(g);
			g.PopState();
			mDarkFrogSequence.Draw(g);
			g.PushState();
			mEssenceExplTop.Draw(g);
			g.PopState();
		}
	}

	protected void DrawBoss6StoneHeadBurst(Graphics g)
	{
		if (mBoss6StoneBurst != null)
		{
			BossStoneHead bossStoneHead = (BossStoneHead)mLevel.mBoss;
			bool flag = true;
			if (bossStoneHead != null)
			{
				flag = bossStoneHead.mStretchPct >= MAX_STONE_HEAD_STRETCH;
			}
			if (flag)
			{
				int num = mWidth / 2 - Common._S(Common._M(45));
				int num2 = Common._S(mApp.GetLevelMgr().GetLevelByIndex(mLevel.mIndex + 1).mBoss.GetY()) - Common._S(Common._M(5));
				int num3 = Common._M(56);
				int maxDuration = mBoss6StoneBurst.GetMaxDuration();
				int num4 = -(mWidth / 2 - Common._S(mLevel.mBoss.GetX())) + Common._S(Common._M(110));
				int num5 = -(mHeight / 2 - Common._S(mLevel.mBoss.GetY())) + Common._S(Common._M(110));
				if (mBoss6StoneBurst.GetUpdateCount() >= num3)
				{
					num4 -= (int)((float)(Common._S(mLevel.mBoss.GetX()) - num) * ((float)(mBoss6StoneBurst.GetUpdateCount() - num3) / (float)(maxDuration - num3)));
					num5 -= (int)((float)(Common._S(mLevel.mBoss.GetY()) - num2) * ((float)(mBoss6StoneBurst.GetUpdateCount() - num3) / (float)(maxDuration - num3)));
				}
				g.PushState();
				g.Translate(num4, num5);
				mBoss6StoneBurst.Draw(g, null, -1, Common._DS(1f));
				g.PopState();
			}
		}
		else
		{
			Font fontByID = Res.GetFontByID(ResID.FONT_BOSS_TAUNT);
			if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CH)
			{
				fontByID.mAscent = 25;
			}
			g.SetFont(fontByID);
			for (int i = 0; i < mSimpleFadeText.size(); i++)
			{
				SimpleFadeText simpleFadeText = mSimpleFadeText[i];
				if (!(simpleFadeText.mAlpha <= 0f))
				{
					g.SetColor(0, 0, 0, (int)(simpleFadeText.mAlpha * 2f / 3f));
					int num6 = Common._S(Common._M(250)) + i * (fontByID.mHeight + Common._S(Common._M1(6)));
					g.FillRect(Common._S(-80), num6, mWidth + Common._S(160), fontByID.mHeight + Common._S(Common._M(6)));
					g.SetColor(0, 0, 0, (int)simpleFadeText.mAlpha);
					g.DrawString(simpleFadeText.mString, (mWidth - fontByID.StringWidth(simpleFadeText.mString)) / 2, num6 + Common._S(Common._M(3)) + fontByID.GetAscent());
				}
			}
		}
		if (mLevel.mCanDrawBoss)
		{
			mLevel.mBoss.Draw(g);
		}
	}

	protected void DrawBullets(Graphics g)
	{
		List<Bullet>.Enumerator enumerator = mBulletList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.DrawShadow(g);
		}
		enumerator = mBulletList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Draw(g);
		}
	}

	protected void DrawTreasure(Graphics g)
	{
		if (IsPaused() || mCurTreasure == null)
		{
			return;
		}
		Image imageByID = Res.GetImageByID(ResID.IMAGE_FRUIT_GENERIC_GLOW);
		int num = Common._S(mCurTreasure.x);
		int num2 = Common._S(mCurTreasure.y);
		int num3 = (imageByID.mWidth - mFruitImg.GetCelWidth()) / 2;
		int num4 = (imageByID.mHeight - mFruitImg.GetCelHeight()) / 2;
		float pct = mFruitBounceEffect.GetPct();
		if (mFruitBounceEffect.GetCount() > 0)
		{
			g.SetDrawMode(1);
			SexyFramework.Graphics.Color color = (mTreasureWasHit ? new SexyFramework.Graphics.Color(255, 0, 0) : new SexyFramework.Graphics.Color(SexyFramework.Graphics.Color.White));
			color.mAlpha = mTreasureStarAlpha;
			if (color.mAlpha != 255)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(color);
			}
			if (g.Is3D())
			{
				g.DrawImageRotatedF(imageByID, num - num3, (float)(num2 - num4) + mTreasureYBob, mTreasureStarAngle);
				g.DrawImageRotatedF(imageByID, num - num3, (float)(num2 - num4) + mTreasureYBob, 0f - mTreasureStarAngle);
			}
			else
			{
				g.DrawImageRotated(imageByID, num - num3, (int)((float)(num2 - num4) + mTreasureYBob), mTreasureStarAngle);
				g.DrawImageRotated(imageByID, num - num3, (int)((float)(num2 - num4) + mTreasureYBob), 0f - mTreasureStarAngle);
			}
			g.SetColorizeImages(colorizeImages: false);
			g.SetDrawMode(0);
		}
		Rect celRect = mFruitImg.GetCelRect(mTreasureCel % mFruitImg.mNumCols, mTreasureCel / mFruitImg.mNumCols);
		float num5 = pct * (float)mFruitImg.GetCelWidth();
		float num6 = pct * (float)mFruitImg.GetCelHeight();
		Rect theDestRect = new Rect((int)((float)num - num5 / 2f + (float)(mFruitImg.GetCelWidth() / 2)), (int)((float)num2 - num6 / 2f + mTreasureYBob + (float)(mFruitImg.GetCelHeight() / 2)), (int)num5, (int)num6);
		if (!mTreasureWasHit)
		{
			g.DrawImage(mFruitImg, theDestRect, celRect);
			if (mTreasureGlowAlpha != 0 && mFruitGlow != null)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, mTreasureGlowAlpha);
				g.DrawImage(mFruitGlow, theDestRect, celRect);
				g.SetColorizeImages(colorizeImages: false);
			}
		}
		else
		{
			mFruitExplodeEffect.Draw(g);
		}
	}

	protected void DrawPlaying(Graphics g)
	{
		if (mBackgroundImage != null)
		{
			if ((mGameState != GameState.GameState_BossDead || mStateCount < 255) && (mGameState != GameState.GameState_Boss6DarkFrog || (mDarkFrogSequence != null && mDarkFrogSequence.GetBGAlpha() < 255f)) && (mFakeCredits == null || !mFakeCredits.IsFullyOpaque()))
			{
				mLevel.DrawUnderBackground(g);
				if (!mLevel.mNoBackground && !mDoMuMuMode)
				{
					int num = 1024;
					int theStretchedHeight = Common._DS(1200);
					g.DrawImage(mBackgroundImage, (Common._S(800) - num) / 2 + GameApp.gScreenShakeX, GameApp.gScreenShakeY, num, theStretchedHeight);
				}
			}
		}
		else
		{
			g.SetColor(0, 128, 128);
			g.FillRect(0, 0, mWidth, mHeight);
		}
		mLevel.DrawBottomLevel(g);
		if (mLevel.mDrawCurves)
		{
			g.DrawImage(mCachedCurveImage, 0, 0);
		}
		DrawTunnels(g, 0, below_shadow: true);
		mLevel.DrawSkullPit(g);
		mLevel.Draw(g);
		if (mLevel.mNum == 5 && mLevel.mZone == 1)
		{
			DrawGauntletWidget(g);
		}
		if (drawer == null)
		{
			drawer = new BallDrawer();
		}
		drawer.Reset();
		if ((mPauseCount == 0 || mShowBallsDuringPause || mZumaTips.size() > 0) && mChallengeHelp == null && mApp.mGenericHelp == null)
		{
			for (int i = 0; i < mLevel.mNumCurves; i++)
			{
				mLevel.mCurveMgr[i].DrawBalls(drawer);
			}
		}
		drawer.Draw(g, this);
		for (int j = 0; j < mLevel.mWalls.size(); j++)
		{
			Wall wall = mLevel.mWalls[j];
			wall.Draw(g);
		}
		mLevel.DrawGunPoints(g);
		if (mLevel.mNum != 5 || mLevel.mZone != 1)
		{
			DrawGauntletWidget(g);
		}
		if (!IsPaused())
		{
			for (int k = 0; k < mBallExplosions.size(); k++)
			{
				mBallExplosions[k].Draw(g);
			}
			for (int l = 0; l < mLazerBlasts.size(); l++)
			{
				mLazerBlasts[l].Draw(g);
			}
		}
		mLevel.DrawFullSceneNoFrog(g);
		if (mLevel.mBoss == null || mDeathSkull == null)
		{
			if (mDeathSkull != null)
			{
				mDeathSkull.DrawBelowFrog(g);
			}
			if (mGameState != GameState.GameState_BossDead && (mGameState != GameState.GameState_Losing || mDeathSkull != null) && mFrogFlyOff == null && !mDoingEndBossFrogEffect && (mCheckpointEffect == null || !mCheckpointEffect.mFromGameOver) && (mGameState != GameState.GameState_FinalBossPart1Finished || mLevel.mTorchStageState >= 11) && mLevel.CanDrawFrog())
			{
				SexyTransform2D sexyTransform2D = new SexyTransform2D(init: false);
				bool flag = GameApp.gApp.Is3DAccelerated() && (double)mTransitionFrogScale > 0.0 && (double)mTransitionFrogScale != 1.0 && mTransitionFrogScale.GetInVal() < 1.0;
				if (flag)
				{
					if (mPlayThud && mTransitionFrogScale.GetInVal() > 0.800000011920929)
					{
						mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_FROG_FALL));
						mPlayThud = false;
					}
					sexyTransform2D.Translate(0f - Common._S(mFrog.mCenterX), 0f - Common._S(mFrog.mCenterY));
					sexyTransform2D.Scale((float)mTransitionFrogScale.GetOutVal(), (float)mTransitionFrogScale.GetOutVal());
					sexyTransform2D.Translate(Common._S(mFrog.mCenterX), Common._S(mFrog.mCenterY));
					g.Get3D().PushTransform(sexyTransform2D);
				}
				mFrog.Draw(g, (mDeathSkull != null) ? mDeathSkull.mFrogClipHeight : 0);
				if (flag)
				{
					g.Get3D().PopTransform();
				}
			}
			if (mDeathSkull != null)
			{
				mDeathSkull.DrawAboveFrog(g);
			}
		}
		DrawTreasure(g);
		DrawBullets(g);
		mLevel.DrawAboveBalls(g);
		if (mLevel.mBoss != null && mLevel.mCanDrawBoss && mGameState != GameState.GameState_BossDead && (mGameState != GameState.GameState_Boss6StoneHeadBurst || mBoss6StoneBurst == null) && (mGameState != GameState.GameState_Boss6DarkFrog || (mBoss6VolcanoMelt == null && mDarkFrogSequence != null && !mDarkFrogSequence.FadingIn())))
		{
			mLevel.mBoss.Draw(g);
		}
		if (mLevel.mBoss != null && mDeathSkull != null)
		{
			if (mDeathSkull != null)
			{
				mDeathSkull.DrawBelowFrog(g);
			}
			if (mGameState != GameState.GameState_BossDead && mFrogFlyOff == null && !mDoingEndBossFrogEffect && (mCheckpointEffect == null || !mCheckpointEffect.mFromGameOver) && (mGameState != GameState.GameState_FinalBossPart1Finished || mLevel.mTorchStageState >= 11) && mLevel.CanDrawFrog())
			{
				mFrog.Draw(g, (mDeathSkull != null) ? mDeathSkull.mFrogClipHeight : 0);
			}
			if (mDeathSkull != null)
			{
				mDeathSkull.DrawAboveFrog(g);
			}
		}
		if (g.Is3D() && !IsPaused())
		{
			for (int m = 0; m < mPowerEffects.size(); m++)
			{
				mPowerEffects[m].Draw(g);
			}
		}
		if (!mPreventBallAdvancement && mCheckpointEffect == null && mGameState == GameState.GameState_Playing && ((mHasSeenCheckpointIntro && ShouldShowCheckpointPostcard()) || !ShouldShowCheckpointPostcard()))
		{
			SetBoardOffset(g, enable: false);
			for (int n = 0; n < 2; n++)
			{
				if (mLevelNameText[n] != null && mLevelNameText[n].mImage != null)
				{
					mLevelNameText[n].Draw(g);
				}
			}
			SetBoardOffset(g, enable: true);
		}
		if (!IsPaused() && mApp.mProxBombManager != null)
		{
			mApp.mProxBombManager.Draw(g);
			mApp.mProxBombManager.DrawOverlay(g);
		}
		if ((mCheckpointEffect == null && mGameState == GameState.GameState_Playing && mHasSeenCheckpointIntro && ShouldShowCheckpointPostcard() && !IronFrogMode() && !GauntletMode() && mPreCheckpointLives < 3) || (mLevel.mNum == 1 && mLevel.mZone > 1 && mPreCheckpointLives < 3 && mLives == 3))
		{
			int num2 = ((mStateCount < Common._M(200)) ? 255 : (255 - (int)((float)(mStateCount - Common._M1(200)) / Common._M2(1f))));
			if (num2 > 0)
			{
				g.SetColor(255, 0, 0, (num2 > 255) ? 255 : num2);
				g.SetFont(Res.GetFontByID(ResID.FONT_SHAGEXOTICA100_STROKE));
				g.WriteString(TextManager.getInstance().getString(124), 0, (mApp.mHeight - g.GetFont().mHeight) / 2, mApp.mWidth, 0);
			}
		}
		mLevel.DrawFullScene(g);
		if (mLevel.mBoss != null && mLevel.mCanDrawBoss)
		{
			mLevel.mBoss.DrawWordBubble(g);
		}
		mLevel.DrawTorchLighting(g);
		if (!mLevel.mBGFromPSD)
		{
			g.SetColor(SexyFramework.Graphics.Color.Black);
			g.FillRect(Common._S(-80), 0, Common._S(80), mHeight);
			g.FillRect(mWidth, 0, Common._S(80), mHeight);
		}
		if (gDrawAutoAimAssistInfo && mApp.mShotCorrectionDebugStyle == 3)
		{
			g.SetFont(Res.GetFontByID(ResID.FONT_MAIN22));
			g.SetColor(255, 255, 255, 255);
			if (mApp.mBoard.mGuideBall != null)
			{
				Ball ball = mApp.mBoard.mGuideBall;
				_ = mApp.mBoard.mGuideBallPoint;
				SexyVector3 speed = ball.GetSpeed();
				float wayPoint = ball.GetWayPoint();
				float wayPointProgress = ball.GetWayPointProgress();
				float x = ball.GetX();
				float y = ball.GetY();
				float fireSpeed = mApp.mBoard.GetGun().GetFireSpeed();
				SexyVector3 sexyVector = new SexyVector3(ball.GetX() - (float)mApp.mBoard.mFrog.GetCenterX(), ball.GetY() - (float)mApp.mBoard.mFrog.GetCenterY(), 0f);
				float num3 = (float)Math.Sqrt(sexyVector.x * sexyVector.x + sexyVector.y * sexyVector.y);
				string theLine = string.Format("WayPoint = {0}\nWayPointProgress = {0}\nX = {0}\nY = {0}\nMoveVec = {0}, {0}\nFireSpeed = {0}\nGuideLength = {0}", wayPoint, wayPointProgress, x, y, speed.x, speed.y, fireSpeed, num3);
				g.WriteWordWrapped(new Rect(Common._S(20), Common._S(100), Common._S(500), Common._S(500)), theLine);
			}
		}
	}

	protected void DrawBossIntro(Graphics g)
	{
		Level level = ((mNextLevel != null) ? mNextLevel : mLevel);
		if ((!mDoingBossIntroText || mBossIntroDirection == 1) && (!mDoingBossIntroFightText || mFightImage.mForward))
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(mBossIntroBGAlpha);
			g.DrawImage(level.mBossIntroBG.GetImage(), Common._S(-80), 0, 1024, 640);
		}
		if (mDoingBossIntroText)
		{
			int num = (int)Math.Min(Math.Max(mBossIntroAlpha, 0f), 255f);
			g.SetColor(0, 0, 0, num);
			g.FillRect(mApp.GetScreenRect());
			g.PushState();
			int alpha = num * Common._M(40) / 255;
			g.SetColor(255, 255, 255, alpha);
			g.SetColorizeImages(colorizeImages: true);
			for (int i = 0; i < 13; i++)
			{
				int id = 1268 + i;
				Image imageByID = Res.GetImageByID((ResID)id);
				int num2 = Common._DS(Res.GetOffsetXByID((ResID)id) - 160);
				int num3 = Common._DS(Res.GetOffsetYByID((ResID)id));
				g.DrawImage(imageByID, num2, num3);
				int theX = num2 + imageByID.GetWidth();
				int theY = num3;
				g.DrawImageMirror(imageByID, theX, theY);
			}
			g.PopState();
			g.SetColorizeImages(colorizeImages: false);
			for (int j = 0; j < mSmokeParticles.size(); j++)
			{
				BambooTransition.DrawSmokeParticle(g, mSmokeParticles[j]);
			}
		}
		else if (mDoingBossIntroFightText)
		{
			int alpha2 = (int)Math.Min(Math.Max(mBossIntroAlpha, 0f), 255f);
			g.SetColor(0, 0, 0, alpha2);
			g.FillRect(mApp.GetScreenRect());
			mFightImage.mX = mWidth / 2;
			mFightImage.mY = mHeight / 2 + Common._DS(Common._M(0));
			mFightImage.Draw(g);
		}
	}

	protected void DrawVortex(Graphics g, bool draw_overlay)
	{
		g.SetColor(0, 0, 0, mVortexAppear ? ((int)mVortexBGAlpha) : 255);
		g.mTransX = 0f;
		g.FillRect(mApp.GetScreenRect());
		g.mTransX = mApp.mBoardOffsetX;
		for (int num = mVortexFaces.size() - 1; num >= 0; num--)
		{
			float mPct = mVortexFaces[num].mPct;
			float mAngle = mVortexFaces[num].mAngle;
			mGlobalTranform.Reset();
			mGlobalTranform.Scale(mPct, mPct);
			mGlobalTranform.RotateRad(mAngle);
			int num4;
			int num5;
			int num2;
			int num3;
			if (mVortexFaces[num].mImage == Res.GetImageByID(ResID.IMAGE_BOSS_VORTEX_FACE1))
			{
				num2 = Common._M(800);
				num3 = Common._M(600);
				num4 = Common._M(400);
				num5 = Common._M(500);
			}
			else
			{
				num2 = Common._M(600);
				num3 = Common._M(650);
				num4 = Common._M(400);
				num5 = Common._M(650);
			}
			float num6 = (float)mVortexFaces[num].mUpdateCount / Common._M(100f);
			if (!(num6 >= 1f))
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, 255 - (int)(255f * num6));
				num2 = (int)((1f - num6) * (float)num2 + num6 * (float)num4);
				num3 = (int)((1f - num6) * (float)num3 + num6 * (float)num5);
				if (g.Is3D())
				{
					g.DrawImageTransformF(mVortexFaces[num].mImage, mGlobalTranform, Common._DS(num2), Common._DS(num3));
				}
				else
				{
					g.DrawImageTransform(mVortexFaces[num].mImage, mGlobalTranform, Common._DS(num2), Common._DS(num3));
				}
				g.SetColorizeImages(colorizeImages: false);
			}
		}
		int centerX = mLevel.mFrog.GetCenterX();
		int centerY = mLevel.mFrog.GetCenterY();
		Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_VORTEX_STREAK);
		for (int i = 0; i < mVortexBeams.size(); i++)
		{
			VortexBeam vortexBeam = mVortexBeams[i];
			mGlobalTranform.Reset();
			mGlobalTranform.Scale(vortexBeam.mPct, vortexBeam.mPct);
			mGlobalTranform.RotateRad(vortexBeam.mAngle - (float)Math.PI / 2f);
			g.SetColorizeImages(colorizeImages: true);
			SexyFramework.Graphics.Color mColor = vortexBeam.mColor;
			mColor.mAlpha = (mVortexAppear ? ((int)mVortexBGAlpha) : 255);
			g.SetColor(mColor);
			g.SetDrawMode(1);
			if (g.Is3D())
			{
				g.DrawImageTransformF(imageByID, mGlobalTranform, vortexBeam.mX + (float)Common._S(centerX), vortexBeam.mY + (float)Common._S(centerY));
			}
			else
			{
				g.DrawImageTransform(imageByID, mGlobalTranform, vortexBeam.mX + (float)Common._S(centerX), vortexBeam.mY + (float)Common._S(centerY));
			}
			g.SetDrawMode(0);
			g.SetColorizeImages(colorizeImages: false);
		}
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_1);
		mGlobalTranform.Reset();
		mGlobalTranform.Scale(mVortexFrogScale, mVortexFrogScale);
		mGlobalTranform.RotateRad(mVortexFrogAngle);
		float x = (float)((double)mVortexFrogRadius * Math.Cos(mVortexFrogAngle) + (double)Common._S(centerX));
		float y = (float)((double)mVortexFrogRadius * (0.0 - Math.Sin(mVortexFrogAngle)) + (double)Common._S(centerY));
		if (g.Is3D())
		{
			g.DrawImageTransformF(imageByID2, mGlobalTranform, x, y);
		}
		else
		{
			g.DrawImageTransform(imageByID2, mGlobalTranform, x, y);
		}
		mLevel.mFrog.DrawConfusionMarks(g);
		if (draw_overlay && !mVortexAppear && mVortexBGAlpha > 0f)
		{
			g.SetColor(0, 0, 0, (int)mVortexBGAlpha);
			g.mTransX = 0f;
			g.FillRect(mApp.GetScreenRect());
			g.mTransX = mApp.mBoardOffsetX;
		}
	}

	protected void DrawIronFrogWin(Graphics g)
	{
		Font fontByID = Res.GetFontByID(ResID.FONT_SHAGEXOTICA100_STROKE);
		Font fontByID2 = Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_STROKE);
		Font fontByID3 = Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_RED_STROKE_YELLOW);
		g.PushState();
		if (mIronFrogAlpha < 255f)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)mIronFrogAlpha);
		}
		g.SetFont(fontByID);
		g.SetColor(255, 255, 255, (int)mIronFrogAlpha);
		g.WriteWordWrapped(new Rect(Common._DS(Common._M(316)), Common._DS(Common._M1(0)), Common._DS(Common._M2(700)), 1000), "VICTORY!", -1, 0);
		g.SetFont(fontByID2);
		g.SetColor(50, 255, 162, (int)mIronFrogAlpha);
		g.WriteWordWrapped(new Rect(Common._DS(Common._M(316)), Common._DS(Common._M1(170)), Common._DS(Common._M2(700)), 1000), "You have beaten the\nIron Frog Gauntlet!", Common._DS(Common._M3(70)), 0);
		g.SetFont(fontByID2);
		g.SetColor(255, 252, 157, (int)mIronFrogAlpha);
		g.WriteString(TextManager.getInstance().getString(114), Common._DS(Common._M(650)), Common._DS(Common._M1(640)));
		g.SetColor(255, 255, 255, (int)mIronFrogAlpha);
		g.WriteString(JeffLib.Common.UpdateToTimeStr(mApp.mUserProfile.mIronFrogStats.mCurTime, use_hour_field: true, 1), Common._DS(Common._M(780)), Common._DS(Common._M1(640)), -1, -1);
		g.SetColor(255, 252, 157, (int)mIronFrogAlpha);
		g.WriteString(TextManager.getInstance().getString(115), Common._DS(Common._M(846)), Common._DS(Common._M1(790)));
		g.SetColor(255, 255, 255, (int)mIronFrogAlpha);
		g.WriteString(SexyFramework.Common.CommaSeperate(mScore), Common._DS(Common._M(1000)), Common._DS(Common._M1(790)), -1, -1);
		g.SetFont(fontByID3);
		if (mApp.mUserProfile.mIronFrogStats.mBestTime == mApp.mUserProfile.mIronFrogStats.mCurTime)
		{
			g.DrawString(TextManager.getInstance().getString(139), Common._DS(Common._M(780)), Common._DS(Common._M1(690)));
		}
		if (mApp.mUserProfile.mIronFrogStats.mBestScore == mScore)
		{
			g.DrawString(TextManager.getInstance().getString(140), Common._DS(Common._M(1000)), Common._DS(Common._M1(850)));
		}
		if (mIronFrogAlpha < 255f)
		{
			g.SetColor(255, 255, 255, (int)mIronFrogAlpha);
			mIronFrogBtn.mBtnNoDraw = false;
			g.Translate(mIronFrogBtn.mX, mIronFrogBtn.mY);
			mIronFrogBtn.Draw(g);
			mIronFrogBtn.mBtnNoDraw = true;
		}
		g.SetColorizeImages(colorizeImages: false);
		g.PopState();
	}

	protected void DrawAdventureStats(Graphics g)
	{
		if (!mDoingTransition && mLevel.mBoss == null && mLevel.mNum != int.MaxValue && DisplayingEndOfLevelStats() && !IronFrogMode())
		{
			bool flag = g.Is3D() && (double)mBossSmScale > 1.0;
			SexyTransform2D sexyTransform2D = new SexyTransform2D(init: false);
			if (flag)
			{
				SexyFramework.Misc.Point zoomPoint = GetZoomPoint();
				sexyTransform2D.Translate(0f - g.mTransX - (float)zoomPoint.mX, -zoomPoint.mY);
				sexyTransform2D.Scale((float)(1.0 + ((double)mBossSmScale - 1.0) * 0.25), (float)(1.0 + ((double)mBossSmScale - 1.0) * 0.25));
				sexyTransform2D.Translate(g.mTransX + (float)zoomPoint.mX, zoomPoint.mY);
				g.Get3D().PushTransform(sexyTransform2D);
			}
			SetBaseBossOffset();
			Common.DrawCommonDialogBacking(g, mAStatsFrame.mX, mAStatsFrame.mY, mAStatsFrame.mWidth, mAStatsFrame.mHeight);
			DrawAdventureStatsBanner(g);
			DrawLilyPadTrail(g);
			DrawBossCircle(g);
			DrawAdventureStatsFrog(g);
			DrawStatsData(g);
			DrawPointsData(g);
			if (flag)
			{
				DrawBossTaunt(g);
				ZoomInOnBoss(g);
			}
			else
			{
				ZoomInOnBoss(g);
				DrawBossTaunt(g);
			}
			if (flag)
			{
				g.Get3D().PopTransform();
			}
		}
	}

	protected SexyFramework.Misc.Point GetZoomPoint()
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_ADVENTURE_STATS_RED_BOSS_FLASH);
		SexyFramework.Misc.Point point = new SexyFramework.Misc.Point();
		point.mX = mApp.GetWideScreenAdjusted(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_ADVENTURE_STATS_RED_BOSS_FLASH)) + imageByID.GetWidth());
		point.mY = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_ADVENTURE_STATS_RED_BOSS_FLASH)) + imageByID.GetHeight() / 2;
		return point;
	}

	protected void SetBaseBossOffset()
	{
		if (mBossOffset.mX == 0 && mBossOffset.mY == 0)
		{
			Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_ADVENTURE_STATS_PIP);
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_UI_ADVENTURE_STATS_BOSS_CIRCLE);
			switch (GameApp.mGameRes)
			{
			case 768:
				mBossOffset.mX = (int)((float)imageByID.GetWidth() * 0.52f);
				mBossOffset.mY = (int)((float)imageByID2.GetHeight() * 0.02f);
				break;
			case 640:
				mBossOffset.mX = (int)((float)imageByID2.GetHeight() * -0.012f);
				mBossOffset.mY = (int)((float)imageByID2.GetHeight() * 0.01f);
				break;
			case 320:
				mBossOffset.mX = (int)((float)imageByID2.GetHeight() * 0.05f);
				mBossOffset.mY = (int)((float)imageByID2.GetHeight() * 0.02f);
				break;
			}
		}
	}

	protected void DrawAdventureStatsBanner(Graphics g)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_ADVENTURE_STATS_BANNER);
		int num = Common._DS(10);
		Rect bannerFrame = GetBannerFrame();
		Rect rect = new Rect(bannerFrame);
		rect.mY += num;
		rect.mHeight = (int)((float)rect.mHeight * 0.5f);
		Rect theRect = new Rect(rect);
		theRect.mY += theRect.mHeight;
		string theLine = TextManager.getInstance().getString(683) + " " + ((mLevel.mZone - 1) * 10 + mLevel.mNum);
		Font fontByID = Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_STROKE);
		Font fontByID2 = Res.GetFontByID(ResID.FONT_SHAGEXOTICA38_BLACK);
		g.DrawImage(imageByID, bannerFrame.mX, bannerFrame.mY, bannerFrame.mWidth, bannerFrame.mHeight);
		g.SetFont(fontByID);
		g.SetColor(SexyFramework.Graphics.Color.White);
		g.WriteWordWrapped(rect, mEndLevelDisplayName, -1, 0);
		g.SetFont(fontByID2);
		g.WriteWordWrapped(theRect, theLine, -1, 0);
	}

	protected Rect GetBannerFrame()
	{
		float num = (float)Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_STROKE).StringWidth(mEndLevelDisplayName) * 1.25f;
		float num2 = Res.GetImageByID(ResID.IMAGE_UI_ADVENTURE_STATS_BANNER).GetWidth();
		float num3 = ((num > num2) ? num : num2);
		float num4 = Res.GetImageByID(ResID.IMAGE_UI_ADVENTURE_STATS_BANNER).GetHeight();
		float num5 = (float)mAStatsFrame.mX + ((float)mAStatsFrame.mWidth - num3) / 2f - (float)Common._DS(150);
		float num6 = (float)mAStatsFrame.mY - num4 * 0.33f;
		return new Rect((int)num5, (int)num6, (int)num3, (int)num4);
	}

	protected void DrawContinueButton(Graphics g)
	{
		if (mStatsContinueBtn == null)
		{
			Rect continueButtonRect = GetContinueButtonRect();
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_GUI_ADVENTURESTATS_CONTINUE), continueButtonRect.mX, continueButtonRect.mY);
		}
	}

	protected void DrawLilyPadTrail(Graphics g)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_ADVENTURE_STATS_PIP);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_UI_ADVENTURE_STATS_PIP_END);
		int theY = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_ADVENTURE_STATS_PIP));
		g.DrawImage(imageByID2, mAStatsFrame.mX + Common._DS(10), theY);
		for (int i = 0; i < 10; i++)
		{
			g.DrawImage(imageByID, mAStatsFrame.mX + imageByID.mWidth / 2 + i * imageByID.mWidth, theY);
		}
	}

	protected void DrawBossCircle(Graphics g)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_ADVENTURE_STATS_BOSS_CIRCLE);
		int x = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_ADVENTURE_STATS_BOSS_CIRCLE)) + mBossOffset.mX;
		int theY = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_ADVENTURE_STATS_BOSS_CIRCLE)) + mBossOffset.mY;
		g.DrawImage(imageByID, GameApp.gApp.GetWideScreenAdjusted(x) - 63, theY);
	}

	protected void DrawAdventureStatsFrog(Graphics g)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_ADVENTURE_STATS_PIP);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_UI_ADVENTURE_STATS_FROG);
		int num = (int)((float)(mAStatsFrame.mX + imageByID.mWidth / 2 + (mLevel.mNum - 1) * imageByID.mWidth) - (float)imageByID2.mWidth * 0.5f);
		int num2 = ((mLevel.mNum == 10) ? num : (num + imageByID.mWidth));
		float frogLeapProgress = GetFrogLeapProgress();
		int theX = (int)((float)num + (float)(num2 - num) * frogLeapProgress);
		int theY = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_ADVENTURE_STATS_FROG));
		float num3 = 1f;
		float num4 = 1.2f;
		num3 = ((!(frogLeapProgress < 0.5f)) ? (num4 - (num4 - 1f) * (frogLeapProgress - 0.5f) * 2f) : (1f + (num4 - 1f) * frogLeapProgress * 2f));
		g.DrawImage(imageByID2, theX, theY, (int)(num3 * (float)imageByID2.mWidth), (int)(num3 * (float)imageByID2.mHeight));
	}

	protected float GetFrogLeapProgress()
	{
		float num = 50f;
		float num2 = 40f;
		float num3 = ((float)mAdvStatsTime - num) / num2;
		if (num3 > 1f)
		{
			num3 = 1f;
		}
		else if (num3 < 0f)
		{
			num3 = 0f;
		}
		if (mLevel.mNum == 10)
		{
			num3 = 0f;
		}
		return num3;
	}

	protected void DrawBossTaunt(Graphics g)
	{
		if (mAdvStatsTime >= 100)
		{
			Font fontByID = Res.GetFontByID(ResID.FONT_MAIN22);
			string text = mApp.GetLevelMgr().mZones[mLevel.mZone - 1].mBossTaunts[mLevel.mNum - 1];
			Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_ADVENTURE_SPEECHBALLOON_YELLOW);
			int width = imageByID.GetWidth();
			int height = imageByID.GetHeight();
			int num = (int)((float)(mAStatsFrame.mX + mAStatsFrame.mWidth) - (float)width * 0.95f);
			int num2 = (int)((float)mAStatsFrame.mY - (float)height * 0.2f);
			float num3 = 0.4f;
			int num4 = (int)((float)height * num3);
			int num5 = Common._DS(30);
			Rect rect = new Rect(num, num2, width, height);
			Rect theRect = new Rect(num + num5, num2 + num5, width - num5 * 2, height - num4 - num5 * 2);
			int num6 = Common._GetWordWrappedHeight(text, fontByID, theRect.mWidth);
			if (num6 > theRect.mHeight)
			{
				int num7 = num6 - theRect.mHeight;
				theRect.mHeight += num7;
				rect.mHeight += (int)((float)num7 + (float)num7 * num3);
			}
			theRect.mY += (int)((float)(theRect.mHeight - num6) * 0.5f);
			g.DrawImage(imageByID, rect.mX, rect.mY, rect.mWidth, rect.mHeight);
			g.SetFont(fontByID);
			g.SetColor(SexyFramework.Graphics.Color.Black);
			g.WriteWordWrapped(theRect, text, -1, 0);
		}
	}

	protected void DrawStatsData(Graphics g)
	{
		List<Rect> statsDataTable = GetStatsDataTable();
		string theLine = TextManager.getInstance().getString(147);
		string theLine2 = "x" + (mEndLevelStats.mMaxCombo + 1) + "\nx" + mEndLevelStats.mMaxInARow + "\nx" + mEndLevelStats.mNumGaps + "\nx" + mEndLevelStats.mNumGemsCleared;
		string theLine3 = TextManager.getInstance().getString(148);
		string theLine4 = JeffLib.Common.UpdateToTimeStr(mEndLevelParTime) + "\n" + JeffLib.Common.UpdateToTimeStr(mEndLevelStats.mTimePlayed) + "\n" + GetBonusPointsString() + "\n" + JeffLib.Common.UpdateToTimeStr(mApp.mUserProfile.GetAdvModeVars().mBestLevelTime[mEndLevelNum - 1]);
		Font fontByID = Res.GetFontByID(ResID.FONT_SHAGEXOTICA38_BLACK_GLOW);
		g.SetFont(fontByID);
		g.SetColor(255, 253, 98);
		g.WriteWordNoAutoWrapped(theLine, statsDataTable[0].mX, statsDataTable[0].mY);
		g.WriteWordNoAutoWrapped(theLine3, statsDataTable[2].mX, statsDataTable[2].mY);
		g.SetColor(253, 126, 0);
		g.WriteWordWrapped(statsDataTable[1], theLine2, -1, 1);
		g.WriteWordWrapped(statsDataTable[3], theLine4, -1, 1);
		DrawRedBurst(g, statsDataTable);
	}

	protected List<Rect> GetStatsDataTable()
	{
		int num = (int)((float)mAStatsFrame.mWidth * 0.7f);
		int num2 = (int)((float)mAStatsFrame.mHeight * 0.25f);
		int theX = (int)((float)mAStatsFrame.mX + (float)(mAStatsFrame.mWidth - num) * 0.5f);
		int theY = (int)((float)mAStatsFrame.mY + (float)(mAStatsFrame.mHeight - num2) * 0.5f);
		int theWidth = (int)((float)num * 0.245f);
		int theWidth2 = (int)((float)num * 0.25f);
		int num3 = (int)((float)num * 0.05f);
		List<Rect> list = new List<Rect>();
		for (int i = 0; i < 4; i++)
		{
			list.Add(default(Rect));
		}
		for (int j = 0; j < list.Count(); j++)
		{
			if (j == 0)
			{
				list[j] = new Rect(theX, theY, theWidth, num2);
			}
			else if (j % 2 == 0)
			{
				list[j] = new Rect(list[j - 1].mX + list[j - 1].mWidth + num3, theY, theWidth, num2);
			}
			else
			{
				list[j] = new Rect(list[j - 1].mX + list[j - 1].mWidth, theY, theWidth2, num2);
			}
		}
		return list;
	}

	protected string GetBonusPointsString()
	{
		int theValue = ((mStatsState >= 0) ? ((mStatsState != 0) ? mEndLevelAceTimeBonus : mCurStatsPointCounter) : 0);
		return SexyFramework.Common.CommaSeperate(theValue);
	}

	protected void DrawRedBurst(Graphics g, List<Rect> aColumns)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_STATSCREEN_BURST);
		int theX = (int)((float)aColumns[2].mX + ((float)(aColumns[2].mWidth + aColumns[3].mWidth) - (float)imageByID.GetWidth() * 1.5f) / 2f);
		int num = aColumns[1].mY + aColumns[1].mHeight / 5;
		int num2 = 0;
		int num3 = 0;
		if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CH || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CHT || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_PL || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_RU)
		{
			num2 = -10;
			num3 = 10;
		}
		g.SetDrawMode(1);
		g.DrawImage(imageByID, theX, num + num2, num3 + (int)((float)imageByID.GetWidth() * 1.5f), imageByID.GetHeight());
		g.SetDrawMode(0);
	}

	protected void DrawPointsData(Graphics g)
	{
		Font fontByID = Res.GetFontByID(ResID.FONT_SHAGEXOTICA38_BLACK_GLOW);
		int num = fontByID.GetHeight() + Common._DS(10);
		int num2 = 100;
		int num3 = fontByID.StringWidth(TextManager.getInstance().getString(146)) + num2;
		int num4 = (int)((float)mAStatsFrame.mWidth * 0.5f);
		num4 = ((num4 > num3) ? num4 : num3);
		int num5 = (int)((float)mAStatsFrame.mHeight * 0.33f);
		int theX = (int)((float)mAStatsFrame.mX + (float)(mAStatsFrame.mWidth - num4) * 0.5f);
		int num6 = mAStatsFrame.mY + num5 * 2;
		Rect[] array = new Rect[3];
		for (int i = 0; i < 3; i++)
		{
			ref Rect reference = ref array[i];
			reference = new Rect(theX, num6 + num * i, num4, num);
		}
		g.SetFont(fontByID);
		DrawPerfectLevelBonus(g, array[0]);
		DrawPointsThisLevel(g, array[1]);
		DrawPointsDataString(g, array[2], TextManager.getInstance().getString(145), mScore, new SexyFramework.Graphics.Color(254, 255, 101), SexyFramework.Graphics.Color.White);
	}

	protected void DrawPerfectLevelBonus(Graphics g, Rect inFrame)
	{
		if (mLevel.mNum <= 10 && mWasPerfectLevel)
		{
			DrawPointsDataString(g, inFrame, TextManager.getInstance().getString(146), GetPerfectBonus(), new SexyFramework.Graphics.Color(255, 231, 40), new SexyFramework.Graphics.Color(233, 105, 61));
		}
	}

	protected void DrawPointsDataString(Graphics g, Rect inFrame, string inLabel, int inPoints, SexyFramework.Graphics.Color inLabelColor, SexyFramework.Graphics.Color inPointsColor)
	{
		g.SetColor(inLabelColor);
		g.WriteWordWrapped(inFrame, inLabel);
		g.SetColor(inPointsColor);
		g.WriteWordWrapped(inFrame, SexyFramework.Common.CommaSeperate(inPoints), -1, 1);
	}

	protected void DrawPointsThisLevel(Graphics g, Rect inFrame)
	{
		int num = 0;
		if (mStatsState == 1)
		{
			num = mCurStatsPointCounter;
		}
		else if (mStatsState == 2)
		{
			num = mLevelPoints + mEndLevelAceTimeBonus + GetPerfectBonus(mLevel.mZone, mLevel.mNum);
		}
		num += mCurveClearBonus;
		DrawPointsDataString(g, inFrame, TextManager.getInstance().getString(144), num, new SexyFramework.Graphics.Color(255, 215, 0), new SexyFramework.Graphics.Color(233, 96, 0));
	}

	protected void ZoomInOnBoss(Graphics g)
	{
		g.SetColorizeImages(colorizeImages: false);
		SexyFramework.Misc.Point point = new SexyFramework.Misc.Point(Common._DS(70), Common._DS(50)) * mBossSmPosPct;
		SexyFramework.Misc.Point zoomPoint = GetZoomPoint();
		zoomPoint += point;
		g.PushState();
		SexyTransform2D sexyTransform2D = new SexyTransform2D(init: false);
		if (g.Is3D())
		{
			sexyTransform2D.Translate(0f - g.mTransX - (float)zoomPoint.mX, -zoomPoint.mY);
			sexyTransform2D.Scale((float)(1.0 + ((double)mBossSmScale - 1.0) * 0.30000001192092896), (float)(1.0 + ((double)mBossSmScale - 1.0) * 0.30000001192092896));
			sexyTransform2D.Translate(g.mTransX + (float)zoomPoint.mX, zoomPoint.mY);
			g.Get3D().PushTransform(sexyTransform2D);
		}
		else
		{
			g.SetScale((float)(double)mBossSmScale, (float)(double)mBossSmScale, zoomPoint.mX, zoomPoint.mY);
		}
		point.mX -= 63;
		DrawRedBossCircle(g, point);
		DrawBossPortrait(g, point);
		if (g.Is3D())
		{
			g.Get3D().PopTransform();
		}
		g.PopState();
		g.mTransX = 0f;
		ReddenScreen(g);
	}

	protected void ReddenScreen(Graphics g)
	{
		if (g.Is3D() && !((double)mBossRedPct <= 0.0))
		{
			g.PushState();
			g.SetColor(255, 0, 0, (int)(255.0 * (double)mBossRedPct));
			g.mClipRect = new Rect(0, 0, GameApp.gApp.GetScreenRect().mWidth, GameApp.gApp.GetScreenRect().mHeight);
			g.FillRect(g.mClipRect);
			g.PopState();
		}
	}

	protected void DrawRedBossCircle(Graphics g, SexyFramework.Misc.Point inOffset)
	{
		if (mLevel.mNum == 10)
		{
			int x = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_ADVENTURE_STATS_RED_BOSS_FLASH)) + mBossOffset.mX + inOffset.mX;
			int theY = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_ADVENTURE_STATS_RED_BOSS_FLASH)) + mBossOffset.mY + inOffset.mY + Common._DS(10);
			g.PushState();
			g.SetColorizeImages(colorizeImages: true);
			if ((double)mBossRedPct > 0.0)
			{
				g.SetColor(255, 255, 255, (int)(255.0 * (double)mBossRedPct));
			}
			else
			{
				int alpha = Math.Min(255, 127 + JeffLib.Common.GetAlphaFromUpdateCount(mUpdateCnt, 128) + ((int)(double)mBossSmScale - 1) * 64);
				g.SetColor(255, 255, 255, alpha);
			}
			Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_ADVENTURE_STATS_RED_BOSS_FLASH);
			g.DrawImage(imageByID, GameApp.gApp.GetWideScreenAdjusted(x), theY);
			g.PopState();
		}
	}

	protected void DrawBossPortrait(Graphics g, SexyFramework.Misc.Point inOffset)
	{
		ResID id = ResID.RESOURCE_MAX;
		switch (mLevel.mZone)
		{
		case 1:
			id = ResID.IMAGE_UI_ADVENTURE_STATS_TIGER;
			break;
		case 2:
			id = ResID.IMAGE_UI_ADVENTURE_STATS_DOCTOR;
			break;
		case 3:
			id = ResID.IMAGE_UI_ADVENTURE_STATS_SKULL;
			break;
		case 4:
			id = ResID.IMAGE_UI_ADVENTURE_STATS_MOSQUITO;
			break;
		case 5:
			id = ResID.IMAGE_UI_ADVENTURE_STATS_SQUID;
			break;
		case 6:
			id = ResID.IMAGE_UI_ADVENTURE_STATS_CLOAK;
			break;
		}
		int x = Common._DS(Res.GetOffsetXByID(id)) + mBossOffset.mX + inOffset.mX;
		int theY = Common._DS(Res.GetOffsetYByID(id)) + mBossOffset.mY + inOffset.mY;
		Image imageByID = Res.GetImageByID(id);
		g.DrawImage(imageByID, GameApp.gApp.GetWideScreenAdjusted(x), theY);
	}

	protected void CheckEndConditions()
	{
		if (mFrog.IsFiring() || mBulletList.Count > 0)
		{
			return;
		}
		if (mLevel.IsFinalBossLevel() && mLevel.AllTorchesOut() && mLevel.AllCurvesAtRolloutPoint())
		{
			mGameState = GameState.GameState_FinalBossPart1Finished;
			GetBetaStats().BeatLevel(mLevelStats.mTimePlayed, mLevel.mParTime, GetAceTimeBonus(), GetPerfectBonus(), (float)mLevel.mFurthestBallDistance / 100f, mScore - mLevelBeginScore, mScore, mLives);
			InitEndOfTorchLevel();
			mStateCount = 0;
			DeleteBullets();
			return;
		}
		int num = 0;
		bool flag = false;
		if (mIsWinning)
		{
			flag = true;
			mGameState = GameState.GameState_BeatLevelBonus;
			return;
		}
		int num2 = 0;
		while (num2 < mLevel.mNumCurves && mLevel.mCurveMgr[num2].IsWinning())
		{
			num2++;
			num++;
		}
		int num3 = 1;
		if (num == mLevel.mNumCurves && (mLevel.mNumCurves > 0 || mLevel.mHaveReachedTarget))
		{
			mIsWinning = true;
			mBossRedPct.SetConstant(0.0);
			mBossIntroBGAlpha.SetConstant(0.0);
			mBossSmScale.SetConstant(1.0);
			mBossSmPosPct.SetConstant(0.0);
			mBossRedPct.SetConstant(0.0);
			mLevelStats.mTimePlayed = mStateCount - mIgnoreCount;
			if (mLevel.mBoss == null && !IronFrogMode() && !GauntletMode() && !mLevel.IsFinalBossLevel())
			{
				if (mLevel.mZone < 7 && mLevel.mNum <= 10)
				{
					int num4 = mApp.mUserProfile.GetAdvModeVars().mBestLevelTime[(mLevel.mZone - 1) * 10 + mLevel.mNum - 1];
					if (mLevelStats.mTimePlayed < num4)
					{
						mApp.mUserProfile.GetAdvModeVars().mBestLevelTime[(mLevel.mZone - 1) * 10 + mLevel.mNum - 1] = mLevelStats.mTimePlayed;
					}
				}
				SetupLevelCompleteText();
			}
			mLevel.AllBallsDestroyed();
			mFrog.EmptyBullets();
			if ((mLevel.mNum == 5 && !IronFrogMode() && !GauntletMode()) || mLevel.mNum == 10 || mLevel.mBoss != null)
			{
				mApp.mUserProfile.GetAdvModeVars().mDDSTier = (mApp.mUserProfile.GetAdvModeVars().mRestartDDSTier = -1);
			}
			int num5 = (mLevel.mZone - 1) * 10 + mLevel.mNum;
			if (num5 > mApp.mUserProfile.GetAdvModeVars().mHighestLevelBeat && !IronFrogMode() && mLevel.mNum != int.MaxValue && !mLevel.IsFinalBossLevel())
			{
				mApp.mUserProfile.GetAdvModeVars().mHighestLevelBeat = num5;
			}
			if (Common.StrEquals(mLevel.mId, mApp.GetLevelMgr().GetLevelId(mApp.GetLevelMgr().GetLastIronFrogLevel())))
			{
				mApp.mUserProfile.mHasBeatIronFrogMode = true;
			}
			mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_LEVEL_COMPLETE));
			mApp.mSoundPlayer.Loop(Res.GetSoundByID(ResID.SOUND_BONUS100LOOP));
			mGameState = GameState.GameState_BeatLevelBonus;
			if (!mApp.mResourceManager.IsGroupLoaded("AdventureStats"))
			{
				mApp.mResourceManager.PrepareLoadResources("AdventureStats");
			}
			if (Common.StrEquals(mLevel.mId, mApp.GetLevelMgr().GetLevelId(mApp.GetLevelMgr().GetLastIronFrogLevel())))
			{
				mApp.PlaySong(144);
			}
			else if (!IronFrogMode())
			{
				int song = 120;
				switch (gTuneNum)
				{
				case 0:
					song = 120;
					break;
				case 1:
					song = 121;
					break;
				case 2:
					song = 122;
					break;
				case 3:
					song = 123;
					break;
				case 4:
					song = 124;
					break;
				case 5:
					song = 125;
					break;
				}
				mApp.PlaySong(song, Common._M(0.0045f));
			}
			if (mAdventureMode)
			{
				if (GetAceTimeBonus() > 0)
				{
					GetAdvModeTempleStats().mNumLevelsAced++;
				}
				if (GetPerfectBonus() > 0)
				{
					GetAdvModeTempleStats().mNumPerfectLevels++;
				}
				if (mLevel.mNum != int.MaxValue && !flag)
				{
					mEndLevelNum = (mLevel.mZone - 1) * 10 + mLevel.mNum;
					mEndLevelAceTimeBonus = GetAceTimeBonus();
					mEndLevelDisplayName = mLevel.mDisplayName;
					mEndLevelParTime = mLevel.mParTime;
					mEndLevelStats = mLevelStats;
				}
				UnlockAchievement(EAchievementType.YOU_WIN);
				if (mLevel.m_canGetAchievementNoMove)
				{
					UnlockAchievement(EAchievementType.FROZEN_FROG);
				}
				if (mLevel.m_canGetAchievementNoJump)
				{
					UnlockAchievement(EAchievementType.FROG_STATUE);
				}
			}
			else if (IronFrogMode() && mLevel.mNum == 10)
			{
				mApp.mUserProfile.mIronFrogStats.mNumVictories++;
				if (mApp.mUserProfile.mIronFrogStats.mCurTime < mApp.mUserProfile.mIronFrogStats.mBestTime || mApp.mUserProfile.mIronFrogStats.mBestTime == 0)
				{
					mApp.mUserProfile.mIronFrogStats.mBestTime = mApp.mUserProfile.mIronFrogStats.mCurTime;
				}
				if (mApp.mUserProfile.mIronFrogStats.mCurTime <= 150099)
				{
					mApp.SetAchievement("iron_will");
				}
			}
			if (mApp.GetLevelMgr().mScoreTips.size() > 0)
			{
				mScoreTipIdx = mApp.GetLevelMgr().GetScoreTipIdx((mLevel.mZone - 1) * 10 + mLevel.mNum);
			}
			DoAccuracy(accuracy: false);
			for (int i = 0; i < mLevel.mNumCurves; i++)
			{
				CurveMgr curveMgr = mLevel.mCurveMgr[i];
				int j = curveMgr.mLastClearedBallPoint;
				int curveLength = curveMgr.GetCurveLength();
				int num6 = (curveLength - j) % 70;
				if (num6 != 0)
				{
					j += num6;
				}
				for (; j <= curveLength; j += 70)
				{
					int x = 0;
					int y = 0;
					int pri = 0;
					curveMgr.GetPoint(j, out x, out y, out pri);
					EndLevelExplosion endLevelExplosion = mEndLevelExplosionPool.Alloc();
					endLevelExplosion.mPIEffect.ResetAnim();
					endLevelExplosion.mPIEffect.mOptimizeValue = 2;
					mEndLevelExplosions.Add(endLevelExplosion);
					mEffectBatch.AddEffect(endLevelExplosion.mPIEffect);
					endLevelExplosion.SetPos(x, y);
					endLevelExplosion.mDelay = num3;
					endLevelExplosion.mX = x;
					endLevelExplosion.mY = y;
					num3 += Common._M(6);
				}
			}
			return;
		}
		num = 0;
		int losing = -1;
		int num7 = 0;
		while (num7 < mLevel.mNumCurves)
		{
			if (mLevel.mCurveMgr[num7].IsLosing())
			{
				losing = num7;
				break;
			}
			num7++;
			num++;
		}
		if (num != mLevel.mNumCurves)
		{
			SetLosing(losing);
		}
		else if (mLevel.mTimer == 0)
		{
			SetLosing();
		}
	}

	public void UnlockAchievement(EAchievementType type)
	{
		if (!mApp.mUserProfile.m_AchievementMgr.isAchievementUnlocked(type))
		{
			mApp.mUserProfile.m_AchievementMgr.UnlockAchievement(type);
			if (!GameApp.UN_UPDATE_VERSION && !GameApp.USE_XBOX_SERVICE && !GameApp.USE_TRIAL_VERSION)
			{
				AchievementEntry achievementEntry = mApp.mUserProfile.m_AchievementMgr.GetAchievementEntry(type);
				ToggleNotification(TextManager.getInstance().getString(92) + TextManager.getInstance().getString(achievementEntry.m_NameResID), Res.GetSoundByID(ResID.SOUND_MIDZONE_NOTIFY));
			}
		}
	}

	protected void SetupLevelCompleteText()
	{
		Font fontByID = Res.GetFontByID(ResID.FONT_SHAGEXOTICA100_STROKE);
		string theString = ((GetAceTimeBonus() > 0) ? TextManager.getInstance().getString(141) : TextManager.getInstance().getString(143));
		int num = fontByID.StringWidth(theString) + 5;
		if (mLevelCompleteText.mImage != null)
		{
			mLevelCompleteText.mImage.Dispose();
		}
		mLevelCompleteText = new FwooshImage();
		mLevelCompleteText.mImage = new DeviceImage();
		mLevelCompleteText.mImage.mApp = mApp;
		mLevelCompleteText.mImage.SetImageMode(hasTrans: true, hasAlpha: true);
		mLevelCompleteText.mImage.AddImageFlags(16u);
		mLevelCompleteText.mImage.Create(num + 15, fontByID.GetHeight() + 5);
		mLevelCompleteText.mDelay = Common._M(150);
		Graphics graphics = new Graphics(mLevelCompleteText.mImage);
		graphics.Get3D().ClearColorBuffer(new SexyFramework.Graphics.Color(0, 0));
		graphics.SetFont(fontByID);
		graphics.SetColor(Common._M(255), Common._M1(255), Common._M2(255));
		graphics.DrawString(theString, 5, fontByID.GetAscent());
		graphics.ClearRenderContext();
	}

	protected void SetSuckMode(bool s)
	{
		Common.gSuckMode = s;
		if (Common.gSuckMode)
		{
			mFrog.EmptyBullets();
			mFrog.SetCannonCount(0, stack: false, -1);
		}
	}

	protected void RestartLevel(bool from_checkpoint, Level copy_effects_from)
	{
		mIsRestarting = true;
		gNeedsGauntletHSSound = true;
		string text = mLevel.mId;
		int num = 0;
		mNeedsBossExtraLife = true;
		bool flag = false;
		if (from_checkpoint)
		{
			mApp.mUserProfile.GetAdvModeVars().mDDSTier = ++mApp.mUserProfile.GetAdvModeVars().mRestartDDSTier;
			mLives = 3;
			int levelIndex = mApp.GetLevelMgr().GetLevelIndex(mLevel.mId);
			levelIndex = ((mLevel.mNum > 5) ? (levelIndex - (mLevel.mNum - 6)) : (levelIndex - (mLevel.mNum - 1)));
			text = mApp.GetLevelMgr().GetLevelId(levelIndex);
			mLevelNum = levelIndex + 1;
			num = mApp.GetLevelMgr().GetLevelById(text).mNum;
			mApp.mUserProfile.GetAdvModeVars().mNumDeathsCurLevel = 0;
			mApp.mUserProfile.GetAdvModeVars().mNumZumasCurLevel = 0;
		}
		else if (mLevel.mZone == 1 && mLevel.mNum <= 5 && mLives <= 0)
		{
			mApp.mUserProfile.GetAdvModeVars().mDDSTier = ++mApp.mUserProfile.GetAdvModeVars().mRestartDDSTier;
			mLives = 3;
			text = "jungle1";
			mLevelNum = 1;
			flag = true;
		}
		bool flag2 = !from_checkpoint && mLevel != null && Common.StrEquals(mLevel.mId, text);
		Reset(game_over: true, flag2, first_time_init: false, !flag2);
		mFrog.LevelReset();
		if (from_checkpoint)
		{
			switch (num)
			{
			case int.MaxValue:
				mScore = mApp.mUserProfile.GetAdvModeVars().mCheckpointScores[mLevel.mZone - 1].mBoss;
				break;
			case 1:
				mScore = mApp.mUserProfile.GetAdvModeVars().mCheckpointScores[mLevel.mZone - 1].mZoneStart;
				break;
			default:
				mScore = mApp.mUserProfile.GetAdvModeVars().mCheckpointScores[mLevel.mZone - 1].mMidpoint;
				break;
			}
			mLevelBeginScore = mScore;
			mRollerScore.ForceScore(mScore);
			int mPointsForLife = mApp.GetLevelMgr().mPointsForLife;
			int num2 = mScore / mPointsForLife;
			mPointsLeftForExtraLife = (num2 + 1) * mPointsForLife - mScore;
		}
		else if (flag)
		{
			mScore = 0;
			mApp.mUserProfile.GetAdvModeVars().mCurrentAdvScore = 0;
			mLevelBeginScore = mScore;
			mRollerScore.ForceScore(mScore);
			mPointsLeftForExtraLife = mApp.GetLevelMgr().mPointsForLife;
		}
		StartLevel(text, from_load: false, from_checkpoint, zone_restart: false, copy_effects_from);
		mHasSeenCheckpointIntro = true;
		if (mLevel.mBoss != null)
		{
			mLevel.mBoss.mAlphaOverride = 0f;
		}
		UpdateGunPos(level_begin: true);
		MakeCachedBackground();
		mIsRestarting = false;
		mTheNextLevel = mLevelNum + 1;
	}

	protected void RestartLevel()
	{
		RestartLevel(from_checkpoint: false, null);
	}

	protected bool ShouldBlockInput()
	{
		bool flag = false;
		flag |= mGauntletRetryBtn != null;
		flag |= mGauntletQuitBtn != null;
		flag |= mDoingIronFrogWin;
		flag |= mApp.mCredits != null;
		flag |= mDoingBossIntroText;
		flag |= mDoingEndBossFrogEffect;
		if (mLevelTransition != null)
		{
			flag |= !mDoingTransition;
			flag |= mLevelTransition.mState != 1;
		}
		flag |= mCheckpointEffect != null;
		flag |= DoingIntros();
		flag |= mGameState == GameState.GameState_Boss6DarkFrog;
		flag |= mGameState == GameState.GameState_FinalBossPart1Finished;
		flag |= mGameState == GameState.GameState_Boss6StoneHeadBurst;
		flag |= mGameState == GameState.GameState_Losing;
		flag |= mGameState == GameState.GameState_BeatLevelBonus;
		if (mLevel != null)
		{
			flag |= mLevel.mFrogFlyOff != null;
		}
		flag |= mChallengeHelp != null;
		flag |= mApp.mGenericHelp != null;
		if (mFrog != null)
		{
			flag |= mFrog.IsHopping();
		}
		return flag | (mFakeCredits != null);
	}

	protected void GetMultTextXY(out int x, out int y, ref int w, ref string comma_seperated_str)
	{
		string text = "";
		if (comma_seperated_str == null)
		{
			comma_seperated_str = text;
		}
		comma_seperated_str = SexyFramework.Common.CommaSeperate(mGauntletPointsFromMult);
		int num = 0;
		if (w == 0)
		{
			w = num;
		}
		Font fontByID = Res.GetFontByID(ResID.FONT_MAIN22);
		w = fontByID.StringWidth(comma_seperated_str);
		x = Common._S(mFrog.GetCenterX()) - w / 2;
		y = Common._S(mFrog.GetCenterY());
		if (mLevel.mMoveType == 1 && y > mHeight / 2)
		{
			y -= Common._S(mFrog.GetHeight() / 2) + Common._DS(Common._M(-190));
		}
		else
		{
			y += Common._S(mFrog.GetHeight() / 2) + Common._DS(Common._M(60));
		}
	}

	protected void GetMultTextXY(out int x, out int y)
	{
		int w = 0;
		string comma_seperated_str = "";
		GetMultTextXY(out x, out y, ref w, ref comma_seperated_str);
	}

	protected void ClearPIEffects()
	{
		for (int i = 0; i < mEndLevelExplosions.Count; i++)
		{
			if (mEndLevelExplosions[i] != null)
			{
				mEndLevelExplosionPool.Free(mEndLevelExplosions[i]);
			}
		}
		mEndLevelExplosions.Clear();
		for (int j = 0; j < mBallExplosions.Count; j++)
		{
			if (mBallExplosions[j] != null)
			{
				mBallExplosions[j].Dispose();
			}
		}
		mBallExplosions.Clear();
		for (int k = 0; k < mLazerBlasts.Count; k++)
		{
			if (mLazerBlasts[k] != null)
			{
				mLazerBlasts[k].Dispose();
			}
		}
		mLazerBlasts.Clear();
		mEffectBatch.Clear();
	}

	public Board(GameApp app, int gauntlet_level)
	{
		if (!app.mResourceManager.IsGroupLoaded("CommonGame") && !app.mResourceManager.LoadResources("CommonGame"))
		{
			app.ShowResourceError(doExit: true);
			app.Shutdown();
		}
		if (!app.mResourceManager.IsGroupLoaded("GamePlay") && !app.mResourceManager.LoadResources("GamePlay"))
		{
			app.ShowResourceError(doExit: true);
			app.Shutdown();
		}
		mIsMouseDown = false;
		mLivesInfo = null;
		mRollerScore = new RollerScore(gauntlet_level != -1);
		mNewBallDelay[0] = (mNewBallDelay[1] = -1);
		mEndLevelNum = 0;
		mEndLevelAceTimeBonus = 0;
		mIsRestarting = false;
		mAdvWinBtn = null;
		mBeatGameTotalScoreTally = (mBeatGameLives = (mBeatGameNormalScore = 0));
		mTimeToBeatAdvMode = 0;
		mNeedsBossExtraLife = true;
		mDoMuMuMode = false;
		mDoingBossIntroText = false;
		mDoingBossIntroFightText = false;
		mBossIntroAlpha = (mBossIntroAlphaRate = (mBossTextY = (mBattleTextY = (mBossTextVY = (mBattleTextVY = 0f)))));
		mBossIntroDirection = (mBossIntroFramesLeft = 0);
		mCloakBossIntroAlpha = 255;
		mChallengeTextAlpha = 0f;
		mIronFrogAlpha = 0f;
		mIronFrogBtn = null;
		mDoingIronFrogWin = false;
		mIronFrogWinDelay = 0;
		mBossIntroDelay = 0;
		mReturnToMainMenu = false;
		mNumDrawFramesLeft = 0;
		mAdvStatsTime = 0;
		mLevelPoints = 0;
		mVortexBGAlpha = 0f;
		mStatsContinueBtn = null;
		mNumPauseUpdatesToDo = 0;
		mDoingFirstTimeIntro = false;
		mDoingFirstTimeIntroZoomToGame = false;
		mChallengeCupUnlockedFX = null;
		mGauntletLastFrogX = (mGauntletLastFrogY = 0);
		mGauntletMultBarAlpha = 255f;
		mGauntletPointsForDiffInc = 0;
		mDoingEndBossFrogEffect = false;
		mEndBossFadeAmt = 0f;
		mEndBossFrogTimer = 0;
		mBossSmokePoof = app.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_GENERICBOSSPOOF").Duplicate();
		mGauntletPointsFromMult = 0;
		mVolcanoBossEssence = null;
		mEssenceExplBottom = (mEssenceExplTop = null);
		mDarkFrogBulletX = 0f;
		mDarkFrogBulletY = 0f;
		mDarkFrogBulletVX = 0f;
		mDarkFrogBulletVY = 0f;
		mDarkFrogTimer = 0;
		mEssenceScaleTimer = 0;
		mEssenceXScale = (mEssenceYScale = 0f);
		mPreCheckpointLives = 3;
		mChallengeHelp = null;
		mSmokePoof = null;
		mIntroBG = null;
		mIntroWater = null;
		mIntroFadeAmt = 0f;
		mGauntletModeOver = false;
		mEndGauntletTimer = 0;
		mFullScreenAlpha = (mFullScreenAlphaRate = 0);
		mForceRestartInAdvMode = false;
		mGuideT = 2000f;
		mMinTreasureY = (mMaxTreasureY = float.MaxValue);
		mCanDeleteEffectResources = true;
		mDeathSkull = null;
		mDisplayAceTime = false;
		mIgnoreCount = 0;
		mScoreTipIdx = -1;
		mGauntletAlpha = 0f;
		mFrogFlyOff = null;
		mTransitionScreenImage = null;
		mDoingTransition = false;
		mPlayThud = false;
		mAdventureWinScreen = false;
		mAdventureWinAlpha = (mAdventureWinExtraAlpha = 0f);
		mAdventureWinDoorYOff = 0f;
		mNeedsCheckpointIntro = false;
		mWasShowingCheckpoint = false;
		mCheckpointEffect = null;
		mFakeCredits = null;
		mDarkFrogSequence = null;
		mScreenShakeXMax = (mScreenShakeYMax = (mScreenShakeTime = 0));
		mWasPerfectLevel = true;
		mIsLoading = false;
		mGauntletMode = gauntlet_level >= 1;
		mApp = app;
		mFrog = null;
		mLevel = null;
		mNextLevel = null;
		mCachedCurveImage = null;
		mBackgroundImage = null;
		mBoss6StoneBurst = null;
		mBoss6VolcanoMelt = null;
		mContinueNextLevelOnLoadProfile = false;
		mNextLevelOverrideOnLoadProfile = -1;
		mCurrentSatPct = 0f;
		mDoPostBossMapScreen = false;
		mNewIronFrogHS = false;
		mHallucinateTimer = 0;
		mHasDoneIntroSounds = false;
		mLastIntroPad = 0;
		mLastIntroPadDelay = 99999;
		mCurStatsPointCounter = (mCurStatsPointTarget = (mCurStatsPointInc = 0));
		mStatsState = -1;
		mStatsDelay = 0;
		mStatsHue = 0;
		mNumDeaths = 0;
		mUnpauseFrame = 0;
		mFruitMultiplier = 1;
		mScoreMultiplier = 1;
		mGauntletHSTarget = 0;
		mHasSeenCheckpointIntro = false;
		mAdventureMode = false;
		mIsHardMode = false;
		mFruitBounceEffect.SetTargetPercents(Common._M(0.5f), Common._M1(1.2f), 1f);
		mFruitBounceEffect.SetRate(Common._M(0.15f));
		mFruitBounceEffect.SetNumBounces(Common._M(6));
		mFruitBounceEffect.SetPct(0f, inc: true);
		mFruitBounceEffect.SetRateDivFactor(Common._M(1.25f));
		mPreventBallAdvancement = false;
		mIntroPadHopCount = 0;
		mMenuButton = null;
		mGameState = GameState.GameState_None;
		mQRand = null;
		for (int i = 0; i < 5; i++)
		{
			mCachedTunnelImages[i] = null;
		}
		for (int j = 0; j < 2; j++)
		{
			mLazerBeam[j] = app.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_LAZER_BEAM").Duplicate();
			mLazerBeam[j].mEmitAfterTimeline = true;
		}
		PIEffect pIEffectByID = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_LAZER_BURN);
		mLazerBurn = pIEffectByID.Duplicate();
		mLazerBurn.mEmitAfterTimeline = false;
		Common.SetFXNumScale(mLazerBurn, 5f);
		mRecalcLazerGuide = (mDoGuide = (mShowGuide = (mRecalcGuide = false)));
		mGuideBall = null;
		mMapScreen = new MapScreen();
		mShowMapScreen = false;
		mFruitExplodeEffect = new FruitExplode(this);
		mShowBossDDSWindow = (mShowDDSWindow = false);
		Reset(game_over: false, level_reset: false, first_time_init: true, delete_bg: true);
		mScore = 0;
		mLevelBeginScore = 0;
		mShowBallsDuringPause = false;
		mDialogCount = 0;
		mPauseUpdateCnt = 0;
		mLives = 3;
		mLevelNum = 1;
		mPointsLeftForExtraLife = mApp.GetLevelMgr().mPointsForLife;
		mPauseCount = 0;
		mCurTreasureNum = -1;
		mPauseUpdateCnt = 0;
		mLastPauseTick = 0;
		mDbgHurry = false;
		mSkipToNextLevelOnNextUpdate = false;
		mForceTreasure = false;
		mLevelTransition = null;
		mTreasureStarAlpha = 255;
		mTreasureGlowAlpha = (mTreasureGlowAlphaRate = 0);
		mTreasureCel = 0;
		mTreasureStarAngle = 0f;
		mGauntletTikiUnlocked = 0;
		mNewGauntletHS = false;
		mGauntletFinalScorePreBonus = 0;
		mGauntletHSIndex = 0;
		mTreasureWasHit = false;
		mTreasureVY = (mTreasureDefaultVY = Common._M(0.25f));
		mTreasureAccel = Common._M(-0.01f);
		mTreasureYBob = 0f;
		mZoneTipIdx = 0;
		mScoreTipIdx = 0;
		GameApp.gDDS.mBoard = this;
		mStartingGauntletLevel = gauntlet_level;
		mSwapBallButton = null;
		MakeUIWidgets();
		mGauntletRetryBtn = (mGauntletQuitBtn = null);
		mClip = false;
		mSkipShutdownSave = false;
		mFatFingerGuideAlpha = 0;
		mFatFingerGuideEnabled = false;
		mIsHotFrogEnabled = false;
		gDrawAutoAimAssistInfo = false;
		mInvalidateTouchUp = false;
		DisableHaloSwap(finishAnim: false);
		mFinishHaloSwap = false;
		DisableBallPowerupCheat();
		mAimGuide = null;
		mNotificationWidget = null;
		prevLevelID = "";
		mInitialTouchPoint.mX = 0;
		mInitialTouchPoint.mY = 0;
		mControlMode = CONTROL_MODE.CONTROL_MODE_NONE;
		mCurveClearBonus = 0;
		mAllowBulletDetection = true;
		mAStatsFrame = new Rect(Common._DS(50), Common._DS(75), Common._DS(1500), Common._DS(1050));
		mCStatsFrame = new Rect(0, Common._DS(100), 965, Common._DS(1000));
		mCStatsFrame.mX = 50;
		mMenuButtonX = Common._DS(1480);
		mDrawBossUI = true;
		for (int k = 0; k < mTunnels.Length; k++)
		{
			mTunnels[k] = new List<Tunnel>();
		}
		for (int l = 0; l < mLevelNameText.Length; l++)
		{
			mLevelNameText[l] = new FwooshImage();
		}
	}

	public override void Dispose()
	{
		mApp.mSoundPlayer.StopAll();
		DoShutdownSaveGame();
		for (int i = 0; i < mSmokeParticles.Count; i++)
		{
			mSmokeParticles[i] = null;
		}
		ClearPIEffects();
		if (mNotificationWidget != null)
		{
			RemoveWidget(mNotificationWidget);
			mNotificationWidget.Dispose();
			mNotificationWidget = null;
		}
		mLazerBeam[0].Dispose();
		mLazerBeam[0] = null;
		mLazerBeam[1].Dispose();
		mLazerBeam[1] = null;
		mLazerBurn = null;
		mBallExplosionsPool.Dispose();
		mEndLevelExplosionPool.Dispose();
		if (mApp.mResourceManager.IsGroupLoaded("BossIntro"))
		{
			mApp.mResourceManager.DeleteResources("BossIntro");
		}
		if (mApp.mResourceManager.IsGroupLoaded("CloakedBoss"))
		{
			mApp.mResourceManager.DeleteResources("CloakedBoss");
		}
		if (mApp.mResourceManager.IsGroupLoaded("GamePlay"))
		{
			mApp.mResourceManager.DeleteResources("GamePlay");
		}
		if (mApp.mResourceManager.IsGroupLoaded("Bosses"))
		{
			mApp.mResourceManager.DeleteResources("Bosses");
		}
		if (mApp.mResourceManager.IsGroupLoaded("CommonBoss"))
		{
			mApp.mResourceManager.DeleteResources("CommonBoss");
		}
		if (mApp.mResourceManager.IsGroupLoaded("Underwater"))
		{
			mApp.mResourceManager.DeleteResources("Underwater");
		}
		if (prevLevelID.Length != 0)
		{
			string theGroup = "Levels_" + prevLevelID.ToUpper()[0] + prevLevelID.Substring(1);
			if (mApp.mResourceManager.IsGroupLoaded(theGroup))
			{
				mApp.mResourceManager.DeleteResources(theGroup);
			}
		}
		if (mLevelCompleteText.mImage != null)
		{
			mLevelCompleteText.mImage.Dispose();
		}
		if (mChallengePtsText.mImage != null)
		{
			mChallengePtsText.mImage.Dispose();
		}
		if (mSmokePoof != null)
		{
			mSmokePoof.Dispose();
		}
		if (mBossSmokePoof != null)
		{
			mBossSmokePoof.Dispose();
		}
		if (mVolcanoBossEssence != null)
		{
			mVolcanoBossEssence.Dispose();
		}
		if (mEssenceExplTop != null)
		{
			mEssenceExplTop.Dispose();
		}
		if (mEssenceExplBottom != null)
		{
			mEssenceExplBottom.Dispose();
		}
		if (mFakeCredits != null)
		{
			mFakeCredits.Dispose();
		}
		if (mDarkFrogSequence != null)
		{
			mDarkFrogSequence.Dispose();
		}
		for (int j = 0; j < mMultiplierBallEffects.Count; j++)
		{
			if (mMultiplierBallEffects[j] != null)
			{
				mMultiplierBallEffects[j].Dispose();
			}
			mMultiplierBallEffects[j] = null;
		}
		if (mDarkFrogSequence != null)
		{
			mDarkFrogSequence.Dispose();
		}
		if (mFruitExplodeEffect != null)
		{
			mFruitExplodeEffect.Dispose();
		}
		if (mBoss6StoneBurst != null)
		{
			mBoss6StoneBurst.Dispose();
		}
		if (mBoss6VolcanoMelt != null)
		{
			mBoss6VolcanoMelt.Dispose();
		}
		if (mDeathSkull != null)
		{
			mDeathSkull.Dispose();
		}
		if (mAimGuide != null)
		{
			mAimGuide.Dispose();
		}
		if (mApp.GetBoard() == null)
		{
			GameApp.gDDS.mBoard = null;
		}
		if (mFrog != null)
		{
			mFrog.Dispose();
		}
		mFrog = null;
		if (mNextLevel != null)
		{
			mNextLevel.Dispose();
		}
		mNextLevel = null;
		if (mLevel != null)
		{
			mLevel.Dispose();
		}
		mLevel = null;
		mBackgroundImage = null;
		if (gDebugCurveData != null)
		{
			gDebugCurveData.Dispose();
		}
		gDebugCurveData = null;
		if (mQRand != null)
		{
			mQRand = null;
		}
		if (mLevelTransition != null)
		{
			mLevelTransition.Dispose();
		}
		mLevelTransition = null;
		if (mCachedCurveImage != null)
		{
			mCachedCurveImage = null;
		}
		mMapScreen.Dispose();
		if (mLevelNameText[0] != null)
		{
			mLevelNameText[0].mImage = null;
		}
		if (mLevelNameText[1] != null)
		{
			mLevelNameText[1].mImage = null;
		}
		RemoveAllWidgets(doDelete: true, recursive: true);
		for (int k = 0; k < mPowerEffects.size(); k++)
		{
			mPowerEffects[k] = null;
		}
		for (int l = 0; l < mZumaTips.size(); l++)
		{
			mZumaTips[l] = null;
		}
		for (int m = 0; m < mText.size(); m++)
		{
			mText[m].mBonus = null;
		}
		EraseTunnels();
		DeleteBullets();
	}

	public void GauntletModeSetupComplete()
	{
		if (mApp.mUserProfile.mWantsChallengeHelp)
		{
			ShowChallengeHelpScreen();
		}
	}

	public void ChallengeHelpClosed()
	{
		mApp.mUserProfile.mWantsChallengeHelp = false;
		mApp.mWidgetManager.RemoveWidget(mChallengeHelp);
		mApp.SafeDeleteWidget(mChallengeHelp);
		mChallengeHelp = null;
	}

	public void ShowChallengeHelpScreen()
	{
		if (mChallengeHelp == null)
		{
			mChallengeHelp = new ChallengeHelp(mApp.mDialogMap.ContainsKey(2));
			mChallengeHelp.mBoard = this;
			mApp.mWidgetManager.AddWidget(mChallengeHelp);
			MarkDirty();
			mApp.mWidgetManager.SetFocus(mChallengeHelp);
			Dialog dialog = mApp.GetDialog(2);
			if (dialog != null)
			{
				mApp.mWidgetManager.PutInfront(mChallengeHelp, mApp.GetDialog(2));
			}
			else
			{
				mApp.mWidgetManager.PutInfront(mChallengeHelp, this);
			}
		}
	}

	public bool IsCheckpointLevel()
	{
		if ((mLevel.mNum != 1 || mLevel.mZone <= 1) && mLevel.mNum != int.MaxValue)
		{
			return mLevel.mNum == 6;
		}
		return true;
	}

	public void AddBallExplosionParticleEffect(Ball b, float angle, float range)
	{
		BallExplosion ballExplosion = mBallExplosionsPool.Alloc();
		mBallExplosions.Add(ballExplosion);
		ballExplosion.SetPos((int)b.GetX(), (int)b.GetY());
		PIEffect mPIEffect = ballExplosion.mPIEffect;
		mPIEffect.ResetAnim();
		PILayer pILayer = null;
		for (int i = 1; i < 7; i++)
		{
			PILayer layer = mPIEffect.GetLayer(i);
			if (!b.GetIsCannon() && 6 - b.GetColorType() == i)
			{
				layer.SetVisible(isVisible: true);
				pILayer = layer;
			}
			else
			{
				layer.SetVisible(isVisible: false);
			}
		}
		if (b.GetIsCannon())
		{
			pILayer = mPIEffect.GetLayer("Cannon");
		}
		else
		{
			mPIEffect.GetLayer("Cannon").SetVisible(isVisible: false);
		}
		if (range >= 0f)
		{
			float mNumberScale = 1f;
			PIEmitterInstance emitter = pILayer.GetEmitter("Large fragments");
			emitter.mEmitterInstanceDef.mValues[11].mValuePointVector[0].mValue = angle;
			emitter.mEmitterInstanceDef.mValues[12].mValuePointVector[0].mValue = range;
			emitter.mNumberScale = mNumberScale;
			emitter = pILayer.GetEmitter("Small fragments");
			emitter.mEmitterInstanceDef.mValues[11].mValuePointVector[0].mValue = angle;
			emitter.mEmitterInstanceDef.mValues[12].mValuePointVector[0].mValue = range;
			emitter.mNumberScale = mNumberScale;
		}
	}

	public void AddBallExplosionParticleEffect(Ball b)
	{
		AddBallExplosionParticleEffect(b, 0f, -1f);
	}

	public bool Init(bool do_first_time_intro)
	{
		mIntroMidAlpha = new CurvedVal();
		mIntroMidScale = new CurvedVal();
		mIntroMidTransX = new CurvedVal();
		mIntroMapAlpha = new CurvedVal();
		mIntroMapPinAlpha = new CurvedVal();
		mIntroMapScale = new CurvedVal();
		mIntroMapTransX = new CurvedVal();
		mIntroRotate = new CurvedVal();
		mIntroFrogScale = new CurvedVal();
		if (mApp.mUserProfile != null)
		{
			GameApp.gDDS.ChangeProfile(mApp.mUserProfile);
		}
		mDoingFirstTimeIntro = do_first_time_intro;
		if (do_first_time_intro)
		{
			mDoingFirstTimeIntroZoomToGame = false;
			if (!mApp.mResourceManager.IsGroupLoaded("CloakedBoss") && !mApp.mResourceManager.LoadResources("CloakedBoss"))
			{
				mApp.ShowResourceError(doExit: true);
				mApp.Shutdown();
			}
			if (!mApp.mResourceManager.IsGroupLoaded("IntroScreen") && !mApp.mResourceManager.LoadResources("IntroScreen"))
			{
				mApp.ShowResourceError(doExit: true);
				mApp.Shutdown();
			}
			if (!mApp.mResourceManager.IsGroupLoaded("Map") && !mApp.mResourceManager.LoadResources("Map"))
			{
				mApp.ShowResourceError(doExit: true);
				mApp.Shutdown();
			}
			Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_LAME_CLOAKEDBOSS_CLAP);
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_LEVELS_INTROSCREEN_BKGRND);
			mCloakBossIntroAlpha = 0;
			gIntroRibbitTimer = Common._M(50);
			mIntroTimer = 0;
			mSmokePoof = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_CLOAKTOLAMEEXPLOSION01).Duplicate();
			mCloakedBossFrame = imageByID.mNumRows * imageByID.mNumCols;
			mIntroFadeAmt = 255f;
			mIntroBG = imageByID2;
			Common.SetFXNumScale(mSmokePoof, 4f);
			mDoIntroFrogJump = false;
			mIntroDialog.Add(new SimpleFadeText(TextManager.getInstance().getString(111)));
			mIntroDialog.Add(new SimpleFadeText(TextManager.getInstance().getString(112)));
			mIntroDialog.Add(new SimpleFadeText(TextManager.getInstance().getString(113)));
			mFrogFlyOff = new FrogFlyOff();
			mFrogFlyOff.JumpOut(mFrog, int.MaxValue, int.MaxValue, Common._SS(mApp.mWidth / 2), Common._M(505), (float)Math.PI);
			PlaySeagulls();
		}
		mHaloSwapCurve.SetConstant(0.0);
		mHaloSwapCurve.mAppUpdateCountSrc = mUpdateCnt;
		Image imageByID3 = Res.GetImageByID(ResID.IMAGE_GUI_GUIDE);
		mAimGuide = new DeviceImage();
		mAimGuide.mApp = mApp;
		mAimGuide.AddImageFlags(16u);
		mAimGuide.SetImageMode(hasTrans: true, hasAlpha: true);
		mAimGuide.Create(mApp.GetScreenRect().mWidth, imageByID3.GetHeight());
		Graphics graphics = new Graphics(mAimGuide);
		for (int i = 0; i < mAimGuide.mWidth; i += imageByID3.GetWidth())
		{
			graphics.DrawImage(imageByID3, i, 0);
		}
		graphics.ClearRenderContext();
		return true;
	}

	public bool Init()
	{
		return Init(do_first_time_intro: false);
	}

	public void AdventureModeSetupComplete(bool continued_game)
	{
		JeffLib.Common.StrFindNoCase(mLevel.mId, "debug");
		if (mGameState != GameState.GameState_BossDead)
		{
			SetMenuBtnEnabled(enabled: true);
		}
		else
		{
			mFrog.SetPos(Common._M(396), Common._M1(460));
		}
	}

	public void MakeCachedBackground()
	{
		mCachedCurveImage = null;
	}

	public void PlaySeagulls()
	{
		int soundByID = Res.GetSoundByID(ResID.SOUND_SEAGULLS);
		if (!mApp.mSoundPlayer.IsLooping(soundByID))
		{
			SoundAttribs soundAttribs = new SoundAttribs();
			soundAttribs.fadeout = 0.1f;
			mApp.mSoundPlayer.Loop(soundByID, soundAttribs);
		}
	}

	public bool ShouldBypassFinalSequenceOnLoad()
	{
		if (mLevel.mFinalLevel && (mVortexAppear || mVortexBGAlpha >= 255f || mAdventureWinScreen))
		{
			return mGameState == GameState.GameState_BossDead;
		}
		return false;
	}

	public void UpdatePlayingFX()
	{
		UpdateTreasureAnim();
		for (int i = 0; i < mText.size(); i++)
		{
			mText[i].mBonus.Update();
			if (mText[i].mBonus.IsDone())
			{
				mText[i].mBonus = null;
				mText.RemoveAt(i);
				i--;
			}
		}
		for (int j = 0; j < mBallExplosions.size(); j++)
		{
			BallExplosion ballExplosion = mBallExplosions[j];
			if (ballExplosion.Update())
			{
				mBallExplosionsPool.Free(ballExplosion);
				mBallExplosions.RemoveAt(j);
				j--;
			}
		}
		for (int k = 0; k < mLazerBlasts.size(); k++)
		{
			mLazerBlasts[k].Update();
			if (!mLazerBlasts[k].IsActive())
			{
				mLazerBlasts[k] = null;
				mLazerBlasts.RemoveAt(k);
				k--;
			}
		}
		mLazerBurn.Update();
		if (mFrog.LaserMode())
		{
			mLazerBeam[0].Update();
			mLazerBeam[1].Update();
			mLazerBurn.mEmitAfterTimeline = true;
		}
		else
		{
			mLazerBurn.mEmitAfterTimeline = false;
		}
		mFruitBounceEffect.Update();
		UpdateLevelCompleteText();
		if (!mPreventBallAdvancement)
		{
			for (int l = 0; l < 2; l++)
			{
				FwooshImage fwooshImage = mLevelNameText[l];
				if (fwooshImage != null && fwooshImage.mImage != null && fwooshImage.mAlpha != 0f)
				{
					fwooshImage.Update();
					if (l == 0 && (MathUtils._geq(fwooshImage.mAlpha, 255f, 0.01f) || fwooshImage.mIncText) && !fwooshImage.mIsDelaying)
					{
						break;
					}
				}
			}
		}
		for (int m = 0; m < mMultiplierBallEffects.size(); m++)
		{
			mMultiplierBallEffects[m].Update();
			if (mMultiplierBallEffects[m].Done())
			{
				mMultiplierBallEffects[m] = null;
				mMultiplierBallEffects.RemoveAt(m);
				m--;
			}
		}
	}

	public void UpdateLevelCompleteText()
	{
		if (mGameState == GameState.GameState_BeatLevelBonus && mLevelCompleteText.mImage != null)
		{
			if (mLevelCompleteText.mAlpha <= 0f || (mLevelTransition != null && !mLevelTransition.IsDone()))
			{
				mLevelCompleteText.mImage = null;
				return;
			}
			mLevelCompleteText.Update();
			mLevelCompleteText.mX = mWidth / 2 - mApp.mBoardOffsetX;
			mLevelCompleteText.mY = Common._DS(Common._M(600));
		}
	}

	public void Reset(bool game_over, bool level_reset, bool first_time_init, bool delete_bg)
	{
		mCurveClearBonus = 0;
		ClearPIEffects();
		mPrevIFBestScore = mApp.mUserProfile.mIronFrogStats.mBestScore;
		mNewBallDelay[0] = (mNewBallDelay[1] = -1);
		mNeedsBossExtraLife = true;
		mDoingBossIntroText = false;
		mAdvStatsTime = 0;
		gEndGauntletExtraTime = 0;
		gMultTimeLeftDecAmt = 0;
		mGauntletPointsForDiffInc = 0;
		mGauntletMultTextFlashOn = true;
		mGauntletMultTextFlashTimer = 0;
		mGauntletMultTextVX = (mGauntletMultTextVY = 0f);
		mGauntletMultTextMoveLastFrame = 0;
		mDoMuMuMode = false;
		System.Buffer.SetByte(mMuMuMode, 0, 0);
		mGauntletModeOver = false;
		mEndGauntletTimer = 0;
		mGauntletPointsFromMult = 0;
		mHallucinateTimer = 0;
		mCurrentSatPct = 0f;
		mTreasureCel = 0;
		if (mApp.mProxBombManager != null)
		{
			mApp.mProxBombManager.Clear();
		}
		mIgnoreCount = 0;
		mMultiplierBallEffects.Clear();
		if (!level_reset)
		{
			mFrog = new Gun(this);
		}
		mText.Clear();
		mFruitExplodeEffect.Reset();
		mNumZumaBalls = 0;
		mCursorBlooms[0] = new CursorBloom();
		mCursorBlooms[1] = new CursorBloom();
		if (mLevel != null && mLevel.mBGFromPSD)
		{
			_ = "IMAGE_LEVELS_" + mLevel.mId.ToUpper() + "_BKGRND";
		}
		if (delete_bg)
		{
			mBackgroundImage = null;
		}
		mNumClearsInARow = (mCurInARowBonus = (mCurComboScore = (mNumCleared = (mCurComboCount = 0))));
		mIsEndless = false;
		mStateCount = 0;
		mScoreTarget = 0;
		mLastBallClickTick = (mLastSmallExplosionTick = 0u);
		mAccuracyCount = 0;
		mAccuracyBackupCount = 0;
		mFlashAlpha = 0;
		for (int i = 0; i < 6; i++)
		{
			mBallColorMap[i] = 0;
		}
		DeleteBullets();
		if (!first_time_init)
		{
			DoAccuracy(accuracy: false);
		}
		if (game_over)
		{
			mGameStats.Reset();
			mLevel.mCurBarSize = 0;
			mFrog.EmptyBullets();
			if (GauntletMode() || IronFrogMode())
			{
				mScore = mLevelBeginScore;
			}
			mRollerScore.ForceScore(mScore);
		}
		else if (!level_reset)
		{
			mLevelNum++;
		}
		else
		{
			mFrog.LevelReset();
		}
		if (first_time_init)
		{
			mFruitMultiplier = 1;
			mScoreMultiplier = 1;
		}
		mForceTreasure = false;
		mLazerHitTreasure = false;
		mGameState = GameState.GameState_Playing;
		mLevelStats.Reset();
		mQRand = new QRand();
		mLevelBeginScore = mScore;
		Common.gDieAtEnd = true;
		Common.gSuckMode = false;
		Common.gAddBalls = true;
		mLevelEndFrame = 0;
		if (!first_time_init)
		{
			UpdateGunPos(level_begin: true);
		}
		mIsWinning = false;
		mCurTreasure = null;
		mTreasureEndFrame = 0;
		mDestroyCount = 0;
		mLastExplosionTick = 0u;
		mDestroyAll = true;
	}

	public void Reset(bool game_over, bool level_reset)
	{
		Reset(game_over, level_reset, first_time_init: false, delete_bg: true);
	}

	public void Pause(bool pause, bool becauseOfDialog)
	{
		if (mShowMapScreen && !mMapScreen.mClosing && !mMapScreen.mIntroClosing)
		{
			return;
		}
		if (pause)
		{
			if (mPauseCount == 0)
			{
				mPauseFade = 0;
			}
			if (becauseOfDialog)
			{
				mDialogCount++;
			}
			mPauseCount++;
			if (mPauseCount == 1)
			{
				mApp.mSoundPlayer.PauseLoopingSounds(p: true);
			}
		}
		else
		{
			if (becauseOfDialog)
			{
				mDialogCount--;
			}
			mPauseCount--;
			if (mPauseCount < 0)
			{
				mPauseCount = 0;
			}
			if (mDialogCount < 0)
			{
				mDialogCount = 0;
			}
			mUnpauseFrame = mUpdateCnt;
			if (mPauseCount == 0 && mDialogCount == 0)
			{
				mApp.mSoundPlayer.PauseLoopingSounds(p: false);
			}
		}
		if (mPauseCount != 0)
		{
			mShowBallsDuringPause = false;
			mPauseUpdateCnt = mUpdateCnt;
			MouseMove(mApp.mWidgetManager.mLastMouseX, mApp.mWidgetManager.mLastMouseY);
		}
	}

	public void Pause(bool pause)
	{
		Pause(pause, becauseOfDialog: false);
	}

	public void LoadLevelBkg(Level theLevel, bool bg_was_from_psd, string psd_bg_id)
	{
		string theName = "IMAGE_LEVELS_" + theLevel.mId.ToUpper() + "_BKGRND";
		if (!bg_was_from_psd)
		{
			mBackgroundImage = null;
		}
		if (!theLevel.mBGFromPSD)
		{
			mBackgroundImage = mApp.mResourceManager.LoadImage(mApp.mResourceManager.GetIdByPath(theLevel.mImagePath)).GetImage();
		}
		else
		{
			mBackgroundImage = mApp.mResourceManager.LoadImage(theName).GetImage();
		}
	}

	public bool StartLevel(string level_id, bool from_load, bool from_checkpoint, bool zone_restart, Level copy_effects_from)
	{
		mCurveClearBonus = 0;
		if (!mApp.mResourceManager.IsGroupLoaded("GamePlay") && !mApp.mResourceManager.LoadResources("GamePlay"))
		{
			mApp.ShowResourceError(doExit: true);
			mApp.Shutdown();
			return false;
		}
		if (prevLevelID.Length != 0 && prevLevelID != level_id)
		{
			string theGroup = "Levels_" + prevLevelID.ToUpper()[0] + prevLevelID.Substring(1);
			if (mApp.mResourceManager.IsGroupLoaded(theGroup))
			{
				mApp.mResourceManager.DeleteResources(theGroup);
			}
		}
		prevLevelID = level_id;
		string theGroup2 = "Levels_" + level_id.ToUpper()[0] + level_id.Substring(1);
		if (!mApp.mResourceManager.IsGroupLoaded(theGroup2))
		{
			mApp.mResourceManager.LoadResources(theGroup2);
		}
		bool flag = !from_checkpoint && !zone_restart && !gCheatReload && mLevel != null && Common.StrEquals(mLevel.mId, level_id);
		if (flag)
		{
			mLevel.ReInit();
		}
		mNeedsBossExtraLife = true;
		RemoveWidget(mStatsContinueBtn);
		mApp.SafeDeleteWidget(mStatsContinueBtn);
		mStatsContinueBtn = null;
		if (mLevelCompleteText.mImage != null)
		{
			mLevelCompleteText.mImage.Dispose();
			mLevelCompleteText.mImage = null;
		}
		mPlayThud = true;
		mChallengeCupUnlockedFX = null;
		mCurTreasure = null;
		mCurTreasureNum = 0;
		mLastIntroPadDelay = 99999;
		mIntroPadHopCount = 0;
		mLastIntroPad = 0;
		for (int i = 0; i < mEndLevelExplosions.Count; i++)
		{
			mEndLevelExplosionPool.Free(mEndLevelExplosions[i]);
		}
		mEndLevelExplosions.Clear();
		mEffectBatch.Clear();
		mVortexFrogRadiusExpand = true;
		mVortexFrogRadius = 0f;
		mVortexFrogAngle = 0f;
		mVortexFrogScale = 1f;
		mHasSeenCheckpointIntro = false;
		bool bg_was_from_psd = false;
		string psd_bg_id = "";
		if (mLevel != null && !flag && mLevel.mBGFromPSD)
		{
			bg_was_from_psd = true;
			psd_bg_id = "IMAGE_LEVELS_" + mLevel.mId.ToUpper() + "_BKGRND";
		}
		int num = ((mLevel == null) ? (-1) : mLevel.mEndSequence);
		if (mLevel != copy_effects_from && !flag)
		{
			mLevel.Dispose();
			mLevel = null;
		}
		for (int j = 0; j < mPowerEffects.size(); j++)
		{
			mPowerEffects[j] = null;
		}
		mPowerEffects.Clear();
		mQRand.Clear();
		bool flag2 = false;
		if (mNextLevel != null)
		{
			mLevel = mNextLevel;
			mNextLevel = null;
		}
		else if (!flag && !mApp.GetLevelMgr().GetLevelById(level_id, ref mLevel, this))
		{
			copy_effects_from = null;
			return false;
		}
		mLevel.mBoard = this;
		mLevel.mApp = mApp;
		string text = mApp.GetLevelMgr().GetZoneFruitId(mLevel.mZone).ToUpper();
		mFruitImg = mApp.mResourceManager.LoadImage("IMAGE_FRUIT_" + text).GetImage();
		mFruitGlow = mApp.mResourceManager.LoadImage("IMAGE_FRUIT_" + text + "_GLOW")?.GetImage();
		if (mDarkFrogSequence == null)
		{
			mMenuButton.SetDisabled(isDisabled: false);
		}
		if (mSwapBallButton != null)
		{
			mSwapBallButton.SetDisabled(isDisabled: false);
		}
		GameApp.gDDS.SetGauntletTime(0);
		GameApp.gDDS.SetGauntletPoints(0);
		GameApp.gDDS.StartLevel(mLevel);
		SetSuckMode(mLevel.mSuckMode);
		if (mLevel.mZone == 6 && (mLevel.IsFinalBossLevel() || mLevel.mEndSequence != -1 || mLevel.mBoss != null))
		{
			if (!mApp.mResourceManager.IsGroupLoaded("Boss6Common") && !mApp.mResourceManager.LoadResources("Boss6Common"))
			{
				mApp.ShowResourceError(doExit: true);
				mApp.Shutdown();
			}
			if (!mApp.mResourceManager.IsGroupLoaded("Bosses") && !mApp.mResourceManager.LoadResources("Bosses"))
			{
				mApp.ShowResourceError(doExit: true);
				mApp.Shutdown();
			}
			if (mLevel.mEndSequence <= 2 && !mApp.mResourceManager.IsGroupLoaded("CloakedBoss") && !mApp.mResourceManager.LoadResources("CloakedBoss"))
			{
				mApp.ShowResourceError(doExit: true);
				mApp.Shutdown();
			}
		}
		if (!flag && !flag2 && (num != 3 || mLevel.mEndSequence != 4))
		{
			LoadLevelBkg(mLevel, bg_was_from_psd, psd_bg_id);
		}
		if (!flag && !flag2)
		{
			EraseTunnels();
			SetupTunnels(mLevel);
		}
		mLevel.StartLevel(from_load, flag);
		if (mLevel.mZone == 6 && mLevel.mEndSequence == 3)
		{
			MakeBoss6VolcanoMeltComp();
		}
		mLevelBeginning = true;
		int num2 = 0;
		for (int k = 0; k < mLevel.mNumCurves; k++)
		{
			if (GameApp.gDDS.GetZumaScore(k) > num2)
			{
				num2 = GameApp.gDDS.GetZumaScore(k);
			}
			mDestroyAll = mLevel.mCurveMgr[k].mCurveDesc.mVals.mDestroyAll;
			if (Common.gSuckMode)
			{
				mLevel.mCurveMgr[k].mCurveDesc.mVals.mPowerUpFreq[2] = 0;
			}
		}
		if (GauntletMode())
		{
			mScoreTarget = int.MaxValue;
		}
		else if (num2 > 0)
		{
			mScoreTarget = mScore + num2;
		}
		else
		{
			mScoreTarget = 0;
		}
		MakeCachedBackground();
		if (mLevel.mBoss != null)
		{
			mIsEndless = true;
			if (mLevel.mBoss.mResGroup != "Boss6_DarkFrog")
			{
				mApp.PlaySong(127);
			}
		}
		else
		{
			mIsEndless = mLevel.mIsEndless;
		}
		if (!mIsRestarting)
		{
			UpdateGunPos(level_begin: true);
		}
		mLevel.SetFrog(mFrog);
		SetupLevelText();
		if (mLevel.mBoss == null)
		{
			if (!mApp.mUserProfile.m_AchievementMgr.isAchievementUnlocked(EAchievementType.FROZEN_FROG) && (mLevel.mMoveType == 1 || mLevel.mMoveType == 2))
			{
				mLevel.m_canGetAchievementNoMove = true;
				mLevel.m_OriginX = mLevel.mFrog.mDestX2;
				mLevel.m_OriginY = mLevel.mFrog.mDestY2;
			}
			else
			{
				mLevel.m_canGetAchievementNoMove = false;
			}
			if (!mApp.mUserProfile.m_AchievementMgr.isAchievementUnlocked(EAchievementType.FROG_STATUE) && mLevel.mNumFrogPoints > 1 && mLevel.CanRotateFrog())
			{
				mLevel.m_canGetAchievementNoJump = true;
			}
			else
			{
				mLevel.m_canGetAchievementNoJump = false;
			}
		}
		_ = mLevel.mNum;
		_ = mLevel.mZone;
		_ = (mLevel.mNum - mLevel.mZone) % 10;
		if (GauntletMode() && mLevel != null)
		{
			List<GauntletHSInfo> scores = new List<GauntletHSInfo>();
			mApp.mUserProfile.GetGauntletHighScores((mLevel.mZone - 1) * 10 + mLevel.mNum, ref scores);
			if (scores.size() == 0)
			{
				mGauntletHSTarget = 0;
			}
			else
			{
				mGauntletHSTarget = scores[0].mScore;
			}
		}
		if (!mIsLoading)
		{
			GetBetaStats().LevelStarted(mLevel.mId, mLevel.mZone, mLevel.mNum, from_checkpoint, zone_restart);
		}
		if (mAdventureMode)
		{
			mApp.mUserProfile.GetAdvModeVars().mCurrentAdvLevel = mLevel.mNum;
			mApp.mUserProfile.GetAdvModeVars().mCurrentAdvZone = mLevel.mZone;
			if (mLevel.mNum != int.MaxValue && !mLevel.IsFinalBossLevel())
			{
				int num3 = (mLevel.mZone - 1) * 10 + mLevel.mNum;
				AdvModeTempleStats advModeTempleStats = GetAdvModeTempleStats();
				if (num3 > advModeTempleStats.mHighestLevel)
				{
					advModeTempleStats.mHighestLevel = num3;
				}
			}
		}
		else if (IronFrogMode())
		{
			if (mLevel.mNum > mApp.mUserProfile.mIronFrogStats.mHighestLevel)
			{
				mApp.mUserProfile.mIronFrogStats.mHighestLevel = mLevel.mNum;
			}
			if (mLevel.mNum == 1)
			{
				mApp.mUserProfile.mIronFrogStats.mNumAttempts++;
			}
		}
		else if (GauntletMode())
		{
			mApp.mUserProfile.mChallengeStats.mNumTimesPlayedCurve[(mLevel.mZone - 1) * 10 + mLevel.mNum - 1]++;
		}
		if (!GauntletMode() && !IronFrogMode() && IsCheckpointLevel() && mLevel.mNum != int.MaxValue && mLives < 3)
		{
			mPreCheckpointLives = mLives;
			mLives = 3;
		}
		else
		{
			mPreCheckpointLives = mLives;
		}
		if (!IronFrogMode() && !GauntletMode())
		{
			if (mLevel.mNum == 10)
			{
				if (!mApp.mResourceManager.IsGroupLoaded("BossIntro"))
				{
					mApp.mResourceManager.LoadResources("BossIntro");
				}
			}
			else if (mLevel.mNum != int.MaxValue && mApp.mResourceManager.IsGroupLoaded("BossIntro"))
			{
				mApp.mResourceManager.DeleteResources("BossIntro");
			}
		}
		else if (IronFrogMode() && mLevel.mNum == 10)
		{
			mApp.mResourceManager.LoadResources("IronFrogWin");
		}
		if (IronFrogMode() && mApp.mUserProfile.mChallengeUnlockState[mLevel.mZone - 1, mLevel.mNum - 1] < 2)
		{
			mApp.mUserProfile.mChallengeUnlockState[mLevel.mZone - 1, mLevel.mNum - 1] = 2;
		}
		mApp.mUserProfile.GetAdvModeVars().mFirstTimeInZone[mLevel.mZone - 1] = false;
		PositionMenuButton();
		if (!Common.BossLevel(mLevel) && mApp.mResourceManager.IsGroupLoaded("Bosses"))
		{
			mApp.mResourceManager.DeleteResources("Bosses");
		}
		GC.Collect();
		return true;
	}

	public bool StartLevel(string level_id)
	{
		return StartLevel(level_id, from_load: false, from_checkpoint: false, zone_restart: false, null);
	}

	public bool StartLevel(int level_num, bool from_load, bool from_checkpoint, bool zone_restart)
	{
		string levelId = mApp.GetLevelMgr().GetLevelId(level_num - 1);
		if (levelId.Length == 0)
		{
			return false;
		}
		mLevelNum = level_num;
		mTheNextLevel = mLevelNum + 1;
		return StartLevel(levelId, from_load, from_checkpoint, zone_restart, null);
	}

	public bool StartLevel(int level_num)
	{
		return StartLevel(level_num, from_load: false, from_checkpoint: false, zone_restart: false);
	}

	public bool RestartFromZone(int zone)
	{
		mApp.mUserProfile.GetAdvModeVars().mCheckpointScores[zone - 1].mBoss = (mApp.mUserProfile.GetAdvModeVars().mCheckpointScores[zone - 1].mMidpoint = (mApp.mUserProfile.GetAdvModeVars().mCheckpointScores[zone - 1].mZoneStart = 0));
		if (mApp.IsHardMode())
		{
			mApp.mUserProfile.mFirstTimeReplayingHardMode = false;
		}
		else
		{
			mApp.mUserProfile.mFirstTimeReplayingNormalMode = false;
		}
		gNeedsGauntletHSSound = true;
		Reset(game_over: false, level_reset: true, first_time_init: true, delete_bg: true);
		mLives = 3;
		mScore = 0;
		mApp.mUserProfile.GetAdvModeVars().mCurrentAdvScore = 0;
		mApp.mUserProfile.GetAdvModeVars().mNumDeathsCurLevel = 0;
		mApp.mUserProfile.GetAdvModeVars().mNumZumasCurLevel = 0;
		mApp.mUserProfile.GetAdvModeVars().mDDSTier = (mApp.mUserProfile.GetAdvModeVars().mRestartDDSTier = -1);
		mPointsLeftForExtraLife = mApp.GetLevelMgr().mPointsForLife;
		mLevelBeginScore = mScore;
		mRollerScore.ForceScore(mScore);
		bool result = StartLevel(mApp.GetLevelMgr().GetZoneStartId(zone), from_load: false, from_checkpoint: false, zone_restart: true, null);
		mLevelNum = mApp.GetLevelMgr().GetLevelIndex(mLevel.mId) + 1;
		UpdateGunPos(level_begin: true);
		MakeCachedBackground();
		mFrog.DeleteBullet();
		mFrog.DeleteBullet();
		mHasDoneIntroSounds = false;
		mTheNextLevel = mLevelNum + 1;
		return result;
	}

	public void PositionMenuButton()
	{
		int num = 0;
		if (mApp.IsWideScreen())
		{
			num = (GauntletMode() ? 42 : Common._DS(-40));
		}
		else if (GauntletMode())
		{
			num = Common._DS(23);
		}
		mMenuButton.Move(mApp.GetWideScreenAdjusted(mMenuButtonX + num), 0);
	}

	public void DoShutdownSaveGame()
	{
		if (mSkipShutdownSave)
		{
			return;
		}
		if (!IronFrogMode() && !GauntletMode())
		{
			if (mLevel.mBoss != null && mLevel.mZone == 1)
			{
				BossTiger bossTiger = (BossTiger)mLevel.mBoss;
				if (bossTiger != null && bossTiger.ShouldEraseBullets())
				{
					DeleteBullets();
				}
			}
			if (mLevel != null)
			{
				mApp.mUserProfile.GetAdvModeVars().mFirstTimeInZone[mLevel.mZone - 1] = false;
			}
			mApp.mUserProfile.GetAdvModeVars().mCurrentAdvScore = mRollerScore.GetTargetScore();
			mApp.mUserProfile.GetAdvModeVars().mCurrentAdvLives = mLives;
			SaveGame(mApp.mUserProfile.GetSaveGameName(mIsHardMode), null);
			if (GameApp.USE_XBOX_SERVICE && !GameApp.USE_TRIAL_VERSION)
			{
				SignedInGamer signedInGamer = Gamer.SignedInGamers[PlayerIndex.One];
				if (signedInGamer != null && signedInGamer.LeaderboardWriter != null)
				{
					LeaderboardIdentity leaderboardId = LeaderboardIdentity.Create(LeaderboardKey.BestScoreLifeTime, 0);
					LeaderboardEntry leaderboard = signedInGamer.LeaderboardWriter.GetLeaderboard(leaderboardId);
					if (leaderboard != null)
					{
						leaderboard.Rating = mApp.mUserProfile.GetAdvModeVars().mCurrentAdvScore;
					}
				}
			}
		}
		else
		{
			mApp.mUserProfile.SaveDetails();
		}
	}

	public void UpdateGunPos(bool level_begin, int _x, int _y)
	{
		if ((mLevel.mBoss != null && mLevel.mBoss.mAlphaOverride <= 254f) || mControlMode == CONTROL_MODE.CONTROL_MODE_SWAPPING)
		{
			return;
		}
		if (level_begin)
		{
			int mCurFrogPoint = mLevel.mCurFrogPoint;
			if (mLevel.mMoveType == 0)
			{
				mFrog.SetPos(mLevel.mFrogX[mCurFrogPoint], mLevel.mFrogY[mCurFrogPoint]);
			}
			else if (mLevel.mMoveType == 1)
			{
				mFrog.SetPos(mLevel.mFrogX[mCurFrogPoint] + mLevel.mBarWidth / 2, mLevel.mFrogY[mCurFrogPoint]);
			}
			else
			{
				mFrog.SetPos(mLevel.mFrogX[mCurFrogPoint], mLevel.mFrogY[mCurFrogPoint] + mLevel.mBarHeight / 2);
			}
			if (mLevelTransition != null)
			{
				mLevelTransition.RehupFrogPosition();
			}
		}
		int num = ((mLevel.mBoss == null) ? (-1) : ((BossShoot)mLevel.mBoss).mMovementMode);
		int num2 = ((_x == -1) ? Common._SS(mApp.mWidgetManager.mLastMouseX) : _x);
		int num3 = ((_y == -1) ? Common._SS(mApp.mWidgetManager.mLastMouseY) : _y);
		int num4 = mFrog.GetCenterX() + mApp.mBoardOffsetX;
		int centerY = mFrog.GetCenterY();
		IGamepad gamepad = mApp.mGamepadDriver.GetGamepad(mApp.mUserProfile.GetGamepadIndex());
		if (gamepad != null && gamepad.IsConnected())
		{
			if (mLevel.mMoveType == 0)
			{
				num2 = (int)((float)num4 + gamepad.GetAxisXPosition() * 256f);
				num3 = (int)((float)centerY + gamepad.GetAxisYPosition() * -256f);
			}
			else if (mLevel.mMoveType == 1)
			{
				GamepadControls gamepadControls = mGamepadControls;
				gamepadControls.axis = new SexyVector2(gamepad.GetAxisXPosition(), gamepad.GetAxisYPosition());
				float num5 = gamepadControls.axis.Dot(gamepadControls.lastAxis);
				if (Math.Abs(gamepadControls.axis.x) <= 0.2f || num5 < 0f)
				{
					gamepadControls.accel = 1f;
				}
				else
				{
					gamepadControls.accel += gamepadControls.accel * 0.05f;
				}
				gamepadControls.accel = Math.Min(GamepadControls.ACCEL_MAX, gamepadControls.accel);
				if (Math.Abs(gamepadControls.axis.y) <= 0.8f)
				{
					gamepadControls.axis.y = 1f;
				}
				num2 = (int)((float)num4 + gamepadControls.axis.x * gamepadControls.accel * 2f);
				num3 = (int)((float)centerY - gamepadControls.axis.y * 2f);
				gamepadControls.lastAxis = gamepadControls.axis;
			}
		}
		int num6 = num2 - num4;
		int num7 = centerY - num3;
		float num8 = (float)Math.Atan2(num7, num6) + 1.570795f;
		if (mLevel.mInvertMouseTimer > 0)
		{
			num8 *= -1f;
		}
		bool flag = !mLevel.mNoFlip;
		if ((mLevel.mBoss != null && LevelIsSkeletonBoss() && mFrog.GetBullet() != null && mFrog.GetBullet().GetIsCannon()) || (mFrog.IsStunned() && !mFrog.StunnedFromBoss6()))
		{
			flag = false;
		}
		if (mLevel.mBoss != null && mLevel.mZone == 2 && mFrog.IsPoisoned())
		{
			flag = false;
		}
		if (mLevel.mBoss != null && (mLevel.mZone == 4 || mLevel.mZone == 6) && mFrog.IsSlow())
		{
			flag = false;
		}
		if ((mGameState != GameState.GameState_Playing && mGameState != GameState.GameState_LevelBegin && mGameState != GameState.GameState_Boss6Transition && mGameState != GameState.GameState_BossDead) || mLevel.mMoveType == 0)
		{
			mFrog.SetDestAngle(num8);
		}
		else if (mLevel.mMoveType == 1)
		{
			int num9 = mLevel.mFrogX[0];
			int num10 = num9 + mLevel.mBarWidth;
			num2 -= mApp.mBoardOffsetX;
			if (flag)
			{
				if (mLevel.mSliderEdgeRotate && num9 >= 0 && ((num2 < num9 && num4 < num9 + 10) || (num2 > num10 && num4 > num10 - 10)))
				{
					mFrog.SetDestAngle(num8);
				}
				else if (num3 < centerY)
				{
					mFrog.SetDestAngle(-3.14159f);
				}
				else
				{
					mFrog.SetDestAngle(0f);
				}
			}
			if (num9 >= 0)
			{
				if (num2 < num9)
				{
					num2 = num9;
				}
				else if (num2 > num10)
				{
					num2 = num10;
				}
			}
			int num11 = mLevel.mFrogY[mLevel.mCurFrogPoint];
			float destX = num2;
			float num12 = num2;
			if (mLevel.mInvertMouseTimer > 0 || num == 2)
			{
				float num13 = (num12 - (float)num9) / (float)mLevel.mBarWidth;
				num12 = (1f - num13) * (float)mLevel.mBarWidth + (float)num9;
				if (num9 >= 0)
				{
					if (num12 < (float)num9)
					{
						num12 = num9;
					}
					else if (num12 > (float)num10)
					{
						num12 = num10;
					}
				}
				if (num == 2)
				{
					destX = num12;
				}
				if (mLevel.mInvertMouseTimer > 0)
				{
					num2 = (int)num12;
				}
			}
			if (_x == -1 || _y == -1)
			{
				mFrog.SetDestPos(num2, num11, (int)((float)mLevel.mMoveSpeed * SLIDER_FROG_DEVICE_SPEEDUP));
				if (mLevel.m_canGetAchievementNoMove && Math.Abs(num2 - mLevel.m_OriginX) > 0)
				{
					mLevel.m_canGetAchievementNoMove = false;
				}
			}
			else
			{
				mFrog.SetPos(num2, num11);
			}
			if (num != -1)
			{
				((BossShoot)mLevel.mBoss).SetDestX(destX);
			}
		}
		else if (mLevel.mMoveType == 2)
		{
			int num14 = mLevel.mFrogY[0];
			int num15 = num14 + mLevel.mBarHeight;
			if (flag)
			{
				if (mLevel.mSliderEdgeRotate && num14 >= 0 && ((num3 < num14 && centerY < num14 + 10) || (num3 > num15 && centerY > num15 - 10)))
				{
					mFrog.SetDestAngle(num8);
				}
				else if (num2 < num4)
				{
					mFrog.SetDestAngle(-1.570795f);
				}
				else
				{
					mFrog.SetDestAngle(1.570795f);
				}
			}
			if (num14 >= 0)
			{
				if (num3 < num14)
				{
					num3 = num14;
				}
				else if (num3 > num15)
				{
					num3 = num15;
				}
			}
			if (_x == -1 || _y == -1)
			{
				mFrog.SetDestPos(mLevel.mFrogX[mLevel.mCurFrogPoint], num3, (int)((float)mLevel.mMoveSpeed * SLIDER_FROG_DEVICE_SPEEDUP));
				if (mLevel.m_canGetAchievementNoMove && Math.Abs(num3 - mLevel.m_OriginY) > 0)
				{
					mLevel.m_canGetAchievementNoMove = false;
				}
			}
			else
			{
				mFrog.SetPos(mLevel.mFrogX[mLevel.mCurFrogPoint], num3);
			}
		}
		mRecalcLazerGuide = (mRecalcGuide = true);
	}

	public void UpdateGunPos(bool level_begin)
	{
		UpdateGunPos(level_begin, -1, -1);
	}

	public void UpdateGunPos()
	{
		UpdateGunPos(level_begin: false, -1, -1);
	}

	public void GauntletMultiplierEnded()
	{
		int num = ((mGameState == GameState.GameState_Losing || mEndGauntletTimer > 0) ? Common._M(75) : Common._M1(1));
		mGauntletMultTextFlashOn = true;
		mGauntletMultTextFlashTimer = num;
		mGauntletLastFrogX = Common._S(mFrog.GetCenterX());
		mGauntletLastFrogY = Common._S(mFrog.GetCenterY());
		int num2 = Common._M(35);
		mGauntletMultTextMoveLastFrame = mStateCount + num + num2;
		mGauntletMultTextVX = (Common._DS(Common._M(740)) - mGauntletLastFrogX) / num2;
		mGauntletMultTextVY = (Common._DS(Common._M(0)) - mGauntletLastFrogY) / num2;
	}

	public void DeleteBullets()
	{
		List<Bullet>.Enumerator enumerator = mBulletList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Dispose();
		}
		mBulletList.Clear();
	}

	public void ClearFirstIntroForBack()
	{
		if (mDoingFirstTimeIntro)
		{
			if (mSmokePoof != null)
			{
				mSmokePoof.Dispose();
				mSmokePoof = null;
			}
			mIntroDialog.Clear();
			if (mFrogFlyOff != null)
			{
				mFrogFlyOff.Dispose();
				mFrogFlyOff = null;
			}
			GameApp.gApp.ShowMainMenu();
			GameApp.gApp.mBoard.mSkipShutdownSave = true;
			mWidgetManager.RemoveWidget(GameApp.gApp.mBoard);
			GameApp.gApp.SafeDeleteWidget(GameApp.gApp.mBoard);
			GameApp.gApp.mBoard = null;
		}
	}

	public override void Update()
	{
		if (GameApp.gApp.IsHardwareBackButtonPressed())
		{
			ProcessHardwareBackButton();
		}
		IGamepad gamepad = mApp.mGamepadDriver.GetGamepad(mApp.mUserProfile.GetGamepadIndex());
		if (gamepad != null && gamepad.IsConnected() && new SexyVector2(gamepad.GetAxisXPosition(), gamepad.GetAxisYPosition()).Magnitude() < 0.2f)
		{
			mGamepadControls.accel = 1f;
		}
		if (mApp.mDoingDRM || mApp.mUpsell != null)
		{
			return;
		}
		base.Update();
		UpdateExtraLivesInfo();
		if (mApp.mMapScreen != null && !mApp.mMapScreen.mDirty)
		{
			return;
		}
		if (mNumDrawFramesLeft > 0)
		{
			MarkDirty();
			return;
		}
		if (mReturnToMainMenu)
		{
			mReturnToMainMenu = false;
			if (mLevel.mBoss == null && isResultPageInAdvMode())
			{
				mForceToNextLevelInAdvMode = true;
				if (mTheNextLevel > mLevelNum)
				{
					mLevelNum++;
				}
			}
			else if (mGameState == GameState.GameState_BossIntro)
			{
				mForceToNextLevelInAdvMode = true;
				if (mTheNextLevel > mLevelNum)
				{
					mLevelNum++;
				}
			}
			mApp.EndCurrentGame();
			mForceToNextLevelInAdvMode = false;
			if (!IronFrogMode())
			{
				if (mApp.mResourceManager.IsGroupLoaded("Bosses"))
				{
					mApp.mResourceManager.DeleteResources("Bosses");
				}
				if (mApp.mResourceManager.IsGroupLoaded("CommonBoss"))
				{
					mApp.mResourceManager.DeleteResources("CommonBoss");
				}
				if (mApp.mResourceManager.IsGroupLoaded("CloakedBoss"))
				{
					mApp.mResourceManager.DeleteResources("CloakedBoss");
				}
				mApp.ShowMainMenu();
			}
			else
			{
				mApp.ShowIronFrog();
			}
			return;
		}
		mRollingInDangerZone = false;
		if (gHideBalls)
		{
			MarkDirty();
		}
		else
		{
			if (mChallengeHelp != null || mApp.mGenericHelp != null || mApp.mDialogMap.ContainsKey(2))
			{
				return;
			}
			if (mZumaTips.size() > 0)
			{
				mZumaTips[0].Update();
			}
			if (mZumaTips.size() > 0 && mZumaTips[0].mBlockUpdates)
			{
				return;
			}
			if (mPauseCount > 0)
			{
				if (mPauseFade <= 50)
				{
					mPauseFade++;
					MarkDirty();
				}
				if (mNumPauseUpdatesToDo > 0)
				{
					MarkDirty();
					mNumPauseUpdatesToDo--;
				}
				return;
			}
			if (mTransitionScreenImage != null && !mTransitionScreenHolePct.IncInVal())
			{
				mTransitionScreenImage = null;
			}
			if (!mTransitionScreenScale.IncInVal())
			{
				if (mDoingTransition)
				{
					mFrog.mDestCount = 0;
					mFrog.ForceX(mFrog.GetCurX());
					mFrog.ForceY(mFrog.GetCurY());
					if (mLevel.mMoveType == 1)
					{
						mFrog.SetDestAngle(-3.14159f);
					}
				}
				mDoingTransition = false;
			}
			mIntroMidAlpha.IncInVal();
			mTransitionFrogRotPct.IncInVal();
			if (mTransitionFrogRotPct.CheckInThreshold(Common._M(0.35f)))
			{
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_FROG_MENU_LEAP));
			}
			mBossIntroBGAlpha.IncInVal();
			mStatsBubbleScale.IncInVal();
			if (mDoingFirstTimeIntro)
			{
				if (mShowMapScreen)
				{
					mMapScreen.Update();
				}
				MarkDirty();
				if (mIntroFadeAmt > 0f && !mDoIntroFrogJump)
				{
					if (mApp.mNewUserDlg == null || mIntroFadeAmt >= (float)Common._M(180))
					{
						mIntroFadeAmt -= Common._M(1f);
					}
					return;
				}
				if (mSmokePoof != null)
				{
					int num = 175;
					mSmokePoof.mDrawTransform.LoadIdentity();
					float num2 = GameApp.DownScaleNum(1f);
					mSmokePoof.mDrawTransform.Scale(num2, num2);
					mSmokePoof.mDrawTransform.Translate(mWidth / 2 + Common._DS(Common._M(0)), Common._DS(Common._M1(180)));
					mSmokePoof.Update();
					if (mSmokePoof.mFrameNum >= (float)Common._M(108))
					{
						mCloakBossIntroAlpha += 2;
					}
					if (mCloakBossIntroAlpha > 255)
					{
						mCloakBossIntroAlpha = 255;
					}
					if (MathUtils._eq(mSmokePoof.mFrameNum, Common._M(80), 0.1f))
					{
						mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_SHADOW_GUY_APPEARS));
					}
					if (mSmokePoof.mFrameNum >= (float)num && mUpdateCnt % Common._M(6) == 0 && mCloakedBossFrame > 0)
					{
						mCloakedBossFrame--;
					}
					if (mSmokePoof.mFrameNum >= (float)mSmokePoof.mLastFrameNum)
					{
						mSmokePoof.Dispose();
						mSmokePoof = null;
					}
					return;
				}
				if (mUpdateCnt % Common._M(6) == 0 && mCloakedBossFrame > 0)
				{
					mCloakedBossFrame--;
				}
				if (mCloakedBossFrame != 0)
				{
					return;
				}
				for (int i = 0; i < mIntroDialog.size(); i++)
				{
					SimpleFadeText simpleFadeText = mIntroDialog[i];
					if (!(simpleFadeText.mAlpha < 255f))
					{
						continue;
					}
					simpleFadeText.mAlpha += Common._M(1.5f);
					if (simpleFadeText.mAlpha >= 255f)
					{
						simpleFadeText.mAlpha = 255f;
						if (i != mIntroDialog.size() - 1)
						{
						}
					}
					else if (simpleFadeText.mAlpha < (float)Common._M(200))
					{
						break;
					}
				}
				if (mIntroDialog.back().mAlpha >= 255f && !mShowMapScreen)
				{
					gIntroRibbitTimer--;
				}
				if (!mDoIntroFrogJump)
				{
					return;
				}
				if (mFrogFlyOff.mJumpOut)
				{
					mFrogFlyOff.Update();
					mIntroFadeAmt += 255f / (float)mFrogFlyOff.mFrogJumpTime;
					if (mIntroFadeAmt > 255f)
					{
						mIntroFadeAmt = 255f;
					}
					if (mFrogFlyOff.mTimer >= mFrogFlyOff.mFrogJumpTime)
					{
						mShowMapScreen = false;
						mFrogFlyOff.JumpIn(mFrog, mFrog.GetCenterX() + mApp.mBoardOffsetX, mFrog.GetCenterY(), continue_from_jump_out: false);
					}
					return;
				}
				mLevel.UpdateEffects();
				mFrogFlyOff.Update();
				mIntroFadeAmt -= 255f / (float)mFrogFlyOff.mFrogJumpTime;
				if (mIntroFadeAmt < 0f)
				{
					mIntroFadeAmt = 0f;
				}
				if (mFrogFlyOff.mTimer >= mFrogFlyOff.mFrogJumpTime)
				{
					mDoingFirstTimeIntro = false;
					mDoingFirstTimeIntroZoomToGame = false;
					mShowMapScreen = false;
					mFrogFlyOff.Dispose();
					mFrogFlyOff = null;
					mDoIntroFrogJump = false;
					SetMenuBtnEnabled(enabled: true);
					mApp.mWidgetManager.SetFocus(this);
					if (mApp.mResourceManager.IsGroupLoaded("IntroScreen"))
					{
						mApp.mResourceManager.DeleteResources("IntroScreen");
					}
					if (mApp.mResourceManager.IsGroupLoaded("MapZoom"))
					{
						mApp.mResourceManager.DeleteResources("MapZoom");
					}
					if (mApp.mResourceManager.IsGroupLoaded("Map"))
					{
						mApp.mResourceManager.DeleteResources("Map");
					}
					if (mApp.mResourceManager.IsGroupLoaded("CloakedBoss"))
					{
						mApp.mResourceManager.DeleteResources("CloakedBoss");
					}
					if (mApp.mResourceManager.IsGroupLoaded("MenuRelated"))
					{
						mApp.mResourceManager.DeleteResources("MenuRelated");
					}
				}
				return;
			}
			if (DisplayingEndOfLevelStats() && mChallengeCupUnlockedFX != null)
			{
				mChallengeCupUnlockedFX.mDrawTransform.LoadIdentity();
				float num3 = GameApp.DownScaleNum(1f);
				mChallengeCupUnlockedFX.mDrawTransform.Scale(num3, num3);
				mChallengeCupUnlockedFX.mDrawTransform.Translate(Common._DS(Common._M(840)), Common._DS(Common._M1(49)));
				mChallengeCupUnlockedFX.Update();
			}
			if (mScreenShakeTime > 0)
			{
				if (--mScreenShakeTime == 0)
				{
					GameApp.gScreenShakeX = (GameApp.gScreenShakeY = 0);
				}
				else
				{
					GameApp.gScreenShakeX = -mScreenShakeXMax + SexyFramework.Common.Rand(mScreenShakeXMax * 2);
					GameApp.gScreenShakeY = -mScreenShakeYMax + SexyFramework.Common.Rand(mScreenShakeYMax * 2);
				}
			}
			if (mNotificationWidget != null)
			{
				if (mNotificationWidget.IsFinished())
				{
					RemoveWidget(mNotificationWidget);
					mNotificationWidget.Dispose();
					mNotificationWidget = null;
				}
			}
			else if (m_NotificationQuene.Count > 0)
			{
				mNotificationWidget = new NotificationWidget(this, m_NotificationQuene[0].Key);
				mNotificationWidget.mSoundID = m_NotificationQuene[0].Value;
				AddWidget(mNotificationWidget);
				m_NotificationQuene.RemoveAt(0);
			}
			if (mGameState != GameState.GameState_ScorePage)
			{
				UpdateHaloSwap();
			}
			mApp.mProxBombManager.Update();
			for (int j = 0; j < mPowerEffects.size(); j++)
			{
				mPowerEffects[j].Update();
				if (mPowerEffects[j].IsDone())
				{
					mPowerEffects[j] = null;
					mPowerEffects.RemoveAt(j);
					j--;
				}
			}
			if (mGameState != GameState.GameState_Playing && mGameState != GameState.GameState_FinalBossPart1Finished && mGameState != GameState.GameState_Boss6StoneHeadBurst && mCheckpointEffect == null)
			{
				mLevel.UpdateEffects();
			}
			if (mLevelTransition != null)
			{
				mStatsHue = (mStatsHue + Common._M(5)) % 255;
				if (mLevelTransition.Update())
				{
					if (mLevelTransition.mTransitionToStats)
					{
						SetMenuBtnEnabled(enabled: false);
						SetupStatsScreen();
						if (mLevel.mBoss == null && !mLevel.IsFinalBossLevel())
						{
							string levelId = mApp.GetLevelMgr().GetLevelId(mLevelNum);
							mApp.GetLevelMgr().GetLevelById(levelId, ref mNextLevel, this);
							mNextLevel.Preload();
						}
					}
					else
					{
						RemoveWidget(mStatsContinueBtn);
						mApp.SafeDeleteWidget(mStatsContinueBtn);
						mStatsContinueBtn = null;
						if (mLevel.mBoss == null && !mLevel.IsFinalBossLevel())
						{
							if (IronFrogMode())
							{
								ContinueToNextLevel();
							}
							SetMenuBtnEnabled(enabled: true);
						}
						if (mDoPostBossMapScreen)
						{
							if (mApp.mResourceManager.IsGroupLoaded("Bosses"))
							{
								mApp.mResourceManager.DeleteResources("Bosses");
							}
							if (mApp.mResourceManager.IsGroupLoaded("CommonBoss"))
							{
								mApp.mResourceManager.DeleteResources("CommonBoss");
							}
							SetupMapScreen(completed: true);
							MarkDirty();
						}
						mDoPostBossMapScreen = false;
					}
				}
				else if (!mLevelTransition.mTransitionToStats)
				{
					mLevel.UpdateEffects();
				}
				else if (mLevelTransition.mTransitionToStats)
				{
					for (int k = 0; k < mText.size(); k++)
					{
						mText[k].mBonus.Update();
						if (mText[k].mBonus.IsDone())
						{
							mText[k].mBonus = null;
							mText.RemoveAt(k);
							k--;
						}
					}
				}
				if (!mLevelTransition.mTransitionToStats && GlobalMembers.gIs3D && !IronFrogMode() && !mLevel.IsFinalBossLevel())
				{
					float num4 = mFrog.mDestAngle;
					if (num4 >= 3.14159f)
					{
						num4 -= 6.28318f;
					}
					mFrog.mAngle = (float)((double)num4 * mTransitionFrogRotPct.GetOutVal()) + Common._M(7.853975f) * (float)(1.0 - mTransitionFrogRotPct.GetOutVal());
					mFrog.mCenterX = (mFrog.mCurX = (float)((double)mFrog.mDestX2 * mTransitionFrogPosPct.GetOutVal() + (double)(mTransitionCenter.mX - Common._SS(mApp.mScreenBounds.mX) / 2) * (1.0 - mTransitionFrogPosPct.GetOutVal())));
					mFrog.mCenterY = (mFrog.mCurY = (float)((double)mFrog.mDestY2 * mTransitionFrogPosPct.GetOutVal() + (double)mTransitionCenter.mY * (1.0 - mTransitionFrogPosPct.GetOutVal())));
				}
				if (mLevelTransition.IsDone() && !mLevelTransition.mTransitionToStats && !mDoingTransition)
				{
					mLevelTransition.Dispose();
					mLevelTransition = null;
					if (mGameState != GameState.GameState_BossIntro)
					{
						mMenuButton.SetDisabled(isDisabled: false);
						if (mSwapBallButton != null)
						{
							mSwapBallButton.SetDisabled(isDisabled: false);
						}
					}
				}
				else
				{
					MarkDirty();
					if (DisplayingEndOfLevelStats() && !mDoingTransition)
					{
						if (DisplayingEndOfLevelStats() && !IronFrogMode())
						{
							mAdvStatsTime++;
							if (mAdvStatsTime == Common._M(50) && mLevel.mBoss == null && mLevel.mNum != 10 && mLevel.mNum != int.MaxValue)
							{
								mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_LILLYPAD_JUMP));
							}
						}
						UnlockChallengeMode();
						if (mStatsDelay > 0)
						{
							mStatsDelay--;
						}
						else if (mGameState != GameState.GameState_BossIntro)
						{
							mCurStatsPointCounter += mCurStatsPointInc;
							mCurStatsPointInc++;
							if (mCurStatsPointCounter < mCurStatsPointTarget && !mApp.mSoundPlayer.IsLooping(Res.GetSoundByID(ResID.SOUND_NEW_ADV_STATS_TALLY)))
							{
								mApp.mSoundPlayer.Loop(Res.GetSoundByID(ResID.SOUND_NEW_ADV_STATS_TALLY));
							}
							if (mCurStatsPointCounter >= mCurStatsPointTarget)
							{
								mCurStatsPointCounter = mCurStatsPointTarget;
								if (mStatsState == 0)
								{
									mStatsState = 1;
									mCurStatsPointCounter = 0;
									mCurStatsPointTarget = mLevelPoints + GetAceTimeBonus() + GetPerfectBonus();
									mStatsDelay = 50;
									mCurStatsPointInc = mCurStatsPointTarget / Common._M(300);
								}
								else
								{
									mStatsState = 2;
								}
								mApp.mSoundPlayer.Stop(Res.GetSoundByID(ResID.SOUND_NEW_ADV_STATS_TALLY));
							}
						}
					}
					if (!mShowMapScreen)
					{
						if (mGameState == GameState.GameState_BossIntro)
						{
							UpdateBossIntro();
						}
						return;
					}
				}
			}
			if (mShowMapScreen)
			{
				mMapScreen.Update();
				if (mMapScreen.mDirty)
				{
					MarkDirty();
				}
				if (mMapScreen.mClosing)
				{
					mIntroFadeAmt = (float)Math.Min(255.0, mIntroFadeAmt + Common._M(3.5f));
					mMapScreen.mRemove |= mIntroMapScale.HasBeenTriggered();
				}
				if (!mMapScreen.mRemove)
				{
					return;
				}
				if (mMapScreen.mClosing)
				{
					if (!mApp.IsRegistered() && mApp.mTrialType == 1 && mLevel.mZone == 3 && mLevel.mNum == 1)
					{
						mApp.DoUpsell(from_exit: false);
					}
					mMapScreen.CloseDone();
					if (mFrog.GetType() == 3)
					{
						mApp.mSoundPlayer.Loop(Res.GetSoundByID(ResID.SOUND_LIGHTNING_LOOP));
					}
					SetMenuBtnEnabled(enabled: true);
					if (mLevel.mNum == 1 && mLevel.mZone != 1)
					{
						ToggleNotification(TextManager.getInstance().getString(432));
					}
					if (ShouldShowCheckpointPostcard() && !GauntletMode() && !IronFrogMode() && mLevel.mNum == 1 && mLevel.mZone > 1)
					{
						mNeedsCheckpointIntro = true;
						mPreventBallAdvancement = true;
					}
				}
				if (mDarkFrogSequence == null)
				{
					SetMenuBtnEnabled(enabled: true);
				}
				mShowMapScreen = false;
				if (mFrog.GetType() == 3)
				{
					mApp.mSoundPlayer.Loop(Res.GetSoundByID(ResID.SOUND_LIGHTNING_LOOP));
				}
				mWasShowingCheckpoint = false;
				if (mApp.mResourceManager.IsGroupLoaded("Map"))
				{
					mApp.mResourceManager.DeleteResources("Map");
				}
				return;
			}
			if (!mDoingFirstTimeIntro)
			{
				mIntroFadeAmt = (float)Math.Max(0.0, mIntroFadeAmt - Common._M(5f));
			}
			if (mCheckpointEffect != null)
			{
				mCheckpointEffect.Update();
				MarkDirty();
				if (mCheckpointEffect.mDone)
				{
					bool mContinuePressed = mCheckpointEffect.mContinuePressed;
					bool mShowMap = mCheckpointEffect.mShowMap;
					mCheckpointEffect.mContinuePressed = (mCheckpointEffect.mShowMap = false);
					if (mLevel.mBoss == null || mApp.IsHardMode() || mLevel.mZone > 1)
					{
						mPreventBallAdvancement = false;
					}
					if (mContinuePressed)
					{
						RemoveWidget(mCheckpointEffect);
						mCheckpointEffect.Dispose();
						mCheckpointEffect = null;
						RestartLevel(from_checkpoint: true, null);
					}
					else if (mShowMap)
					{
						mWasShowingCheckpoint = true;
						SetupMapScreen(completed: false);
						mCheckpointEffect.Disable(d: true);
					}
					else if (!mCheckpointEffect.mFromGameOver)
					{
						RemoveWidget(mCheckpointEffect);
						mCheckpointEffect.Dispose();
						mCheckpointEffect = null;
					}
					mWasShowingCheckpoint = false;
				}
			}
			else if (mGauntletRetryBtn != null)
			{
				if (mGauntletAlpha < 255f && (mGauntletAlpha += Common._M(10f)) >= 255f)
				{
					mGauntletRetryBtn.mVisible = true;
					mGauntletQuitBtn.mVisible = true;
					if (mScore >= mLevel.mChallengeAcePoints)
					{
						mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_CHALLENGE_ACE_VICTORY));
						mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_CHALLENGE_SCORE_VICTORY));
					}
					else if (mScore >= mLevel.mChallengePoints)
					{
						mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_CHALLENGE_SCORE_VICTORY));
					}
				}
				else if (mGauntletAlpha >= 255f)
				{
					mGauntletAlpha += Common._M(10f);
				}
				if (mGauntletHSIndex <= 4 && mScoreDisplayPos != (float)mGauntletHSIndex && mGauntletAlpha >= (float)Common._M(600))
				{
					int num5 = (int)mScoreDisplayPos;
					mScoreDisplayPos += Common._M(-0.025f) + ((float)mGauntletHSIndex - mScoreDisplayPos) * Common._M1(0.0025f);
					int num6 = (int)mScoreDisplayPos;
					if (num6 != num5 || mScoreDisplayPos < 0f)
					{
						if (num5 == 4)
						{
							Font fontByID = Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_GAUNTLET2);
							for (int l = 0; l < 2; l++)
							{
								int num7 = mScoreBreakPositions[l].mX;
								char thePrevChar = '\0';
								for (int m = 0; m < mScoreBreakStrings[l].Length; m++)
								{
									ScoreLetterEffect scoreLetterEffect = new ScoreLetterEffect();
									scoreLetterEffect.mChar = mScoreBreakStrings[l][m];
									scoreLetterEffect.mX = num7;
									scoreLetterEffect.mY = mScoreBreakPositions[l].mY;
									scoreLetterEffect.mVelX = (float)((double)(((float)m / Math.Max(1f, (float)mScoreBreakStrings[l].Length - 1f) - 0.5f) * Common._S(Common._M(1.5f))) + ((double)(SexyFramework.Common.Rand() % 1000) - 500.0) / 500.0 * (double)Common._S(Common._M(0.25f)));
									scoreLetterEffect.mVelY = (float)((double)(SexyFramework.Common.Rand() % 1000) / 1000.0 * (double)Common._S(Common._M(-1f)));
									scoreLetterEffect.mRot = 0f;
									scoreLetterEffect.mRotAdd = (float)((double)(SexyFramework.Common.Rand() % 1000) / 500.0 * (double)Common._S(Common._M(0.005f)));
									scoreLetterEffect.mUpdateCnt = 0f;
									num7 += fontByID.CharWidthKern(scoreLetterEffect.mChar, thePrevChar);
									thePrevChar = scoreLetterEffect.mChar;
									mScoreLetterEffectVector.Add(scoreLetterEffect);
								}
							}
						}
						if (num5 == mGauntletHSIndex)
						{
							mScoreDisplayPos = mGauntletHSIndex;
						}
						if (num5 < 5)
						{
							mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_CANNON_FIRE));
						}
					}
				}
				else if (mGauntletAlpha >= (float)Common._M(10))
				{
					mChallengeHeaderText.Update();
					mChallengePtsText.Update();
					mChallengeHeaderText.mX = (mWidth - mChallengeHeaderText.mImage.mWidth) / 2 + mChallengeHeaderText.mImage.mWidth / 2;
					mChallengeHeaderText.mY = Common._DS(Common._M(150));
					mChallengePtsText.mX = (mWidth - mChallengePtsText.mImage.mWidth) / 2 + mChallengePtsText.mImage.mWidth / 2;
					if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CH || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CHT || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_PL)
					{
						mChallengePtsText.mY = Common._DS(Common._M(370));
					}
					else
					{
						mChallengePtsText.mY = Common._DS(Common._M(350));
					}
					if (MathUtils._eq(mChallengePtsText.mSize, 1f) && mChallengePtsText.mDelay == 0)
					{
						mChallengeTextAlpha += Common._M(20f);
						if (mChallengeTextAlpha > 255f)
						{
							mChallengeTextAlpha = 255f;
						}
					}
				}
				for (int n = 0; n < mScoreLetterEffectVector.size(); n++)
				{
					ScoreLetterEffect scoreLetterEffect2 = mScoreLetterEffectVector[n];
					scoreLetterEffect2.mVelY += Common._S(Common._M(0.05f));
					scoreLetterEffect2.mX += scoreLetterEffect2.mVelX;
					scoreLetterEffect2.mY += scoreLetterEffect2.mVelY;
					scoreLetterEffect2.mRot += scoreLetterEffect2.mRotAdd;
					if (scoreLetterEffect2.mY > (float)Common._S(Common._M(610)))
					{
						mScoreLetterEffectVector.RemoveAt(n);
						n--;
					}
				}
				MarkDirty();
			}
			else
			{
				if (!mLevel.CanUpdate())
				{
					return;
				}
				if (mLevel.mZone == 1 && mLevel.mBoss == null && !GauntletMode() && !mLevel.DoingInitialPathHilite() && mGameState == GameState.GameState_Playing && mApp.mUserProfile.GetAdvModeVars().mNumDeathsCurLevel == 1 && !mApp.mUserProfile.HasSeenHint(ZumaProfile.SKULL_PIT_HINT))
				{
					HoleInfo hole = mLevel.mHoleMgr.GetHole(0);
					int theX = Common._S(hole.mX + Common._M(-28));
					int theY = Common._S(hole.mY + Common._M(-30));
					int num8 = Common._S(Common._M(150));
					ZumaTip zumaTip = new ZumaTip(TextManager.getInstance().getString(827), Common._S(Common._M(200)), Common._S(Common._M1(100)), new Rect(theX, theY, num8, num8), ZumaProfile.SKULL_PIT_HINT);
					zumaTip.AutoPointAtCutoutRegion();
					mZumaTips.Add(zumaTip);
					mApp.mUserProfile.MarkHintAsSeen(ZumaProfile.SKULL_PIT_HINT);
					MarkDirty();
					return;
				}
				if (NeedsLillyPadHint() && !GauntletMode() && mZumaTips.size() == 0 && !mLevel.DoingInitialPathHilite())
				{
					int num9 = ((mLevel.mCurFrogPoint == 0) ? 1 : 0);
					int num10 = Common._S(mLevel.mFrogX[num9]);
					int num11 = Common._S(mLevel.mFrogY[num9]);
					ZumaTip zumaTip2 = new ZumaTip(TextManager.getInstance().getString(828), Common._S(Common._M(200)), Common._S(Common._M1(100)), new Rect(num10 - Common._S(Common._M2(100)), num11 - Common._S(Common._M3(100)), Common._S(Common._M4(200)), Common._S(Common._M5(200))), ZumaProfile.LILLY_PAD_HINT);
					zumaTip2.AutoPointAtCutoutRegion();
					zumaTip2.mBlockUpdates = false;
					mPreventBallAdvancement = true;
					mZumaTips.Add(zumaTip2);
					MarkDirty();
					return;
				}
				if (mLevel.mZone == 1 && !GauntletMode() && mLevel.mNum == 3 && !mApp.mUserProfile.HasSeenHint(ZumaProfile.SWAP_BALL_HINT) && mZumaTips.size() == 0 && !mLevel.DoingInitialPathHilite())
				{
					mPreventBallAdvancement = true;
					ZumaTip zumaTip3 = new ZumaTip(TextManager.getInstance().getString(829), Common._DS(mFrog.mWidth * 3), Common._DS(mFrog.mHeight * 2), new Rect((int)Common._S(mFrog.mCurX - 75f), (int)Common._S(mFrog.mCurY - 100f), Common._S(150), Common._S(200)), ZumaProfile.SWAP_BALL_HINT);
					zumaTip3.AutoPointAtCutoutRegion();
					zumaTip3.mBlockUpdates = false;
					mZumaTips.Add(zumaTip3);
					MarkDirty();
					return;
				}
				if (mLevel.mZone == 1 && !GauntletMode() && mLevel.mNum == 2 && !mApp.mUserProfile.HasSeenHint(ZumaProfile.FRUIT_HINT) && mZumaTips.size() == 0 && !mLevel.DoingInitialPathHilite())
				{
					mPreventBallAdvancement = true;
					mLevel.ForceTreasure(Common._M(0));
					mApp.PlaySamplePan(Res.GetSoundByID(ResID.SOUND_TIKI_APPEAR), mApp.GetPan(mCurTreasure.x), 5);
					mTreasureEndFrame = 2147483646;
					mTreasureStarAlpha = 255;
					mTreasureGlowAlpha = 0;
					mTreasureGlowAlphaRate = Common._M(12);
					mTreasureWasHit = false;
					mTreasureVY = (mTreasureDefaultVY = Common._M(0.25f));
					mTreasureYBob = 0f;
					mTreasureAccel = Common._M(-0.01f);
					mFruitBounceEffect.Reset();
					ZumaTip zumaTip4 = new ZumaTip(TextManager.getInstance().getString(830), Common._S(Common._M(200)), Common._S(Common._M1(140)), new Rect(Common._S(mCurTreasure.x - Common._M2(50)), Common._S(mCurTreasure.y - Common._M3(40)), Common._S(Common._M4(150)), Common._S(Common._M5(150))), ZumaProfile.FRUIT_HINT);
					zumaTip4.AutoPointAtCutoutRegion();
					zumaTip4.mBlockUpdates = false;
					mPreventBallAdvancement = true;
					mZumaTips.Add(zumaTip4);
					MarkDirty();
					return;
				}
				if (!mHasDoneIntroSounds)
				{
					if (mLevel.mBoss != null)
					{
						mLevel.mBoss.mNeedsIntroSound = true;
						if (!mLevel.DoingInitialPathHilite())
						{
							mHasDoneIntroSounds = true;
							SoundAttribs soundAttribs = new SoundAttribs();
							soundAttribs.fadeout = 0.008f;
							mApp.mSoundPlayer.Loop((mLevel.mZone == 5) ? Res.GetSoundByID(ResID.SOUND_UNDERWATER_ROLLOUT) : Res.GetSoundByID(ResID.SOUND_ROLLING), soundAttribs);
						}
					}
					else if (!mLevel.DoingInitialPathHilite() && mGameState != GameState.GameState_Losing && mZumaTips.size() == 0 && (!ShouldShowCheckpointPostcard() || mHasSeenCheckpointIntro || GauntletMode() || IronFrogMode()))
					{
						mHasDoneIntroSounds = true;
						SoundAttribs soundAttribs2 = new SoundAttribs();
						soundAttribs2.fadeout = 0.008f;
						mApp.mSoundPlayer.Loop((mLevel.mZone == 5) ? Res.GetSoundByID(ResID.SOUND_UNDERWATER_ROLLOUT) : Res.GetSoundByID(ResID.SOUND_ROLLING), soundAttribs2);
					}
				}
				if (mSkipToNextLevelOnNextUpdate)
				{
					mSkipToNextLevelOnNextUpdate = false;
					ContinueToNextLevel(-1, did_level_transition: true);
				}
				mApp.IncFramesPlayed();
				mFrog.Update();
				if (mAccuracyBackupCount > 0 && !mFrog.LaserMode() && mFrog.GetLazerCount() == 0 && !mFrog.LightningMode() && !mFrog.CannonMode() && mFrog.GetType() != 1)
				{
					mAccuracyCount = mAccuracyBackupCount;
					mAccuracyBackupCount = 0;
					DoAccuracy(accuracy: true);
				}
				if (mFrog.IsMovingToDest())
				{
					mRecalcGuide = (mRecalcLazerGuide = true);
				}
				if (!mHasSeenCheckpointIntro && ShouldShowCheckpointPostcard() && !GauntletMode() && !IronFrogMode() && mLevel.mNum > 1 && mCheckpointEffect == null)
				{
					if (mFrog.HasSmokeParticles())
					{
						mPreventBallAdvancement = true;
					}
					else
					{
						mNeedsCheckpointIntro = true;
					}
				}
				if (mNeedsCheckpointIntro)
				{
					mHasSeenCheckpointIntro = true;
					mNeedsCheckpointIntro = false;
					DoCheckpointEffect(game_over: false);
				}
				if (mGameState != GameState.GameState_BossIntro)
				{
					mStateCount++;
					if (mZumaTips.size() != 0 || mLevel.DoingInitialPathHilite())
					{
						mIgnoreCount++;
					}
				}
				if (GauntletMode() && mEndGauntletTimer >= 0 && mGauntletModeOver)
				{
					UpdatePlayingFX();
					mLevel.UpdateEffects();
					if (mEndGauntletTimer > 0)
					{
						gMultTimeLeftDecAmt++;
						mLevel.mCurMultiplierTimeLeft -= gMultTimeLeftDecAmt;
						if (mGauntletMultBarAlpha > 0f)
						{
							mGauntletMultBarAlpha -= Common._M(5f);
						}
						mGauntletMultTextFlashTimer--;
						if (mGauntletMultBarAlpha <= 0f && --mEndGauntletTimer == 0)
						{
							mGauntletFinalScorePreBonus = mScore;
							mScore += (int)((float)mScoreMultiplier / 100f * (float)mScore);
							mRollerScore.SetTargetScore(mScore);
						}
						if (mEndGauntletTimer == 0)
						{
							end_delay = Common._M(25);
						}
					}
					if (mEndGauntletTimer == 0 && mRollerScore.mAtTarget && --end_delay == 0)
					{
						SetupEndOfGauntletTransition(hit_max_time: true);
					}
					Bullet bullet = null;
					while ((bullet = mFrog.GetFiredBullet()) != null)
					{
						AddFiredBullet(bullet);
						mLevel.BulletFired(bullet);
					}
				}
				else if (!GauntletMode() || !mGauntletModeOver)
				{
					if (mDoingIronFrogWin)
					{
						if (--mIronFrogWinDelay <= 0 && mIronFrogAlpha < 255f)
						{
							mIronFrogAlpha += Common._M(10f);
							if (mIronFrogAlpha >= 255f)
							{
								mIronFrogAlpha = 255f;
								mIronFrogBtn.SetVisible(isVisible: true);
								mIronFrogBtn.SetDisabled(isDisabled: false);
								mIronFrogBtn.mBtnNoDraw = false;
							}
						}
					}
					else
					{
						switch (mGameState)
						{
						case GameState.GameState_Playing:
							UpdatePlaying();
							break;
						case GameState.GameState_Losing:
							UpdateLosing();
							break;
						case GameState.GameState_LevelUp:
							CueLevelTransition();
							break;
						case GameState.GameState_BossDead:
							UpdateBossDeath();
							break;
						case GameState.GameState_FinalBossPart1Finished:
							UpdateFinalBossPart1Finished();
							break;
						case GameState.GameState_BossIntro:
							UpdateBossIntro();
							break;
						case GameState.GameState_Boss6Transition:
							UpdateBoss6Transition();
							break;
						case GameState.GameState_Boss6FakeCredits:
							UpdateBoss6FakeCredits();
							break;
						case GameState.GameState_Boss6StoneHeadBurst:
							UpdateBoss6StoneHeadBurst();
							break;
						case GameState.GameState_Boss6DarkFrog:
							UpdateBoss6DarkFrog();
							break;
						case GameState.GameState_BeatLevelBonus:
							UpdateBeatLevelBonus();
							break;
						}
					}
				}
				bool flag = mIsMouseDown;
				if (flag || mDoGuide || mAccuracyCount > 0 || Common.gSuckMode || (mFrog.GetType() == 2 && mFrog.GetLazerCount() > 0) || mFrog.GetType() == 3)
				{
					if (flag || mDoGuide || Common.gSuckMode || mAccuracyCount > 0)
					{
						UpdateGuide(lazer: false);
					}
					if ((mFrog.GetType() == 2 && mFrog.GetLazerCount() > 0) || mFrog.GetType() == 3)
					{
						UpdateGuide(lazer: true);
					}
				}
				else
				{
					mGuideBall = null;
				}
				UpdateMiscStuff();
				MarkDirty();
			}
		}
	}

	public override void Draw(Graphics g)
	{
		SetBoardOffset(g, enable: true);
		if (mNumDrawFramesLeft > 0 && mApp.mDialogMap.Count == 0)
		{
			mNumDrawFramesLeft--;
		}
		bool flag = mApp.mCredits != null && MathUtils._geq(mApp.mCredits.mAlpha, 255f);
		bool flag2 = (mDoingIronFrogWin && MathUtils._geq(mIronFrogAlpha, 255f)) || flag;
		if (mDoingFirstTimeIntro)
		{
			if ((double)mIntroMidAlpha < 1.0 && mFrogFlyOff.mJumpOut)
			{
				SetBoardOffset(g, enable: false);
				g.DrawImage(mIntroBG, 0, 0, mApp.GetScreenRect().mWidth, mApp.GetScreenRect().mHeight);
			}
			DeferOverlay(9);
			if (mFrogFlyOff.mJumpOut)
			{
				return;
			}
		}
		bool flag3 = mLevelTransition != null && !mLevelTransition.IsDone();
		if ((mPauseCount > 0 || mNumDrawFramesLeft > 0 || mReturnToMainMenu || mFullScreenAlpha > 0 || mFlashAlpha > 0 || mShowDDSWindow || mShowBossDDSWindow || mZumaTips.size() > 0 || flag3 || mShowMapScreen || mIntroFadeAmt > 0f) && !flag2)
		{
			DeferOverlay(1);
			if (mShowMapScreen && mNumPauseUpdatesToDo <= 0)
			{
				return;
			}
		}
		if ((mGameState != GameState.GameState_BossIntro || (double)mBossIntroAlpha < 255.0 || (mDoingBossIntroFightText && !mFightImage.mForward)) && !flag2 && (mGameState != GameState.GameState_BossDead || mStateCount < 255) && mGameState != GameState.GameState_ScorePage)
		{
			SexyTransform2D sexyTransform2D = new SexyTransform2D(init: false);
			if (GlobalMembers.gIs3D && (double)mBossSmScale > 1.0)
			{
				sexyTransform2D.Translate(0f - g.mTransX - (float)Common._S(Common._M(870)), -Common._S(Common._M1(175)));
				sexyTransform2D.Scale((float)(1.0 + ((double)mBossSmScale - 1.0) * (double)Common._M(0.25f)), (float)(1.0 + ((double)mBossSmScale - 1.0) * (double)Common._M1(0.25f)));
				sexyTransform2D.Translate(g.mTransX + (float)Common._S(Common._M(870)), Common._S(Common._M1(175)));
				g.Get3D().PushTransform(sexyTransform2D);
			}
			DrawPlaying(g);
			if (GlobalMembers.gIs3D && (double)mBossSmScale > 1.0)
			{
				g.Get3D().PopTransform();
			}
		}
		if (mGameState == GameState.GameState_BeatLevelBonus)
		{
			for (int i = 0; i < mEndLevelExplosions.size(); i++)
			{
				EndLevelExplosion endLevelExplosion = mEndLevelExplosions[i];
				if (endLevelExplosion.mDelay > 0)
				{
					endLevelExplosion.mPIEffect.mInUse = false;
				}
				else
				{
					endLevelExplosion.mPIEffect.mInUse = true;
				}
			}
			if (mEndLevelExplosions.Count > 0)
			{
				mEffectBatch.DrawBatch(g);
			}
		}
		Font fontByID = Res.GetFontByID(ResID.FONT_MAIN22);
		g.SetFont(fontByID);
		g.SetColor(SexyFramework.Graphics.Color.White);
		if (gShowText && !DisplayingEndOfLevelStats() && !IsPaused())
		{
			for (int j = 0; j < mText.size(); j++)
			{
				mText[j].mBonus.Draw(g);
			}
		}
		g.mTransX = mApp.mBoardUIOffsetX;
		if (mGameState != GameState.GameState_BossDead && !flag2 && (mGameState != GameState.GameState_BossIntro || (mDoingBossIntroFightText && !mFightImage.mForward)) && (mLevelTransition == null || mDoingTransition || IronFrogMode() || (mLevelTransition.mTransitionToStats && mLevelTransition.GetState() < 1) || (!mLevelTransition.mTransitionToStats && !mLevelTransition.IsDone() && mLevelTransition.GetState() == 2)))
		{
			mLevel.DrawUI(g);
		}
		g.mTransX = 0f;
		g.mTransX = mApp.mBoardOffsetX;
		g.SetFont(fontByID);
		g.SetColor(SexyFramework.Graphics.Color.White);
		if (!IsPaused())
		{
			for (int k = 0; k < mMultiplierBallEffects.size(); k++)
			{
				mMultiplierBallEffects[k].Draw(g);
			}
		}
		if (!flag2 && mGameState != GameState.GameState_ScorePage)
		{
			mLevel.DrawToplevel(g);
		}
		if (mLevelCompleteText.mImage != null && mGameState != GameState.GameState_ScorePage)
		{
			mLevelCompleteText.Draw(g);
		}
		if (!flag && mGameState != GameState.GameState_ScorePage)
		{
			if (mGameState == GameState.GameState_Boss6Transition)
			{
				g.SetColor(0, 0, 0, (mStateCount <= 255) ? mStateCount : (510 - mStateCount));
				g.mTransX = 0f;
				g.FillRect(mApp.GetScreenRect());
				g.mTransX = mApp.mBoardOffsetX;
			}
			else if (mGameState == GameState.GameState_BossIntro)
			{
				DrawBossIntro(g);
			}
			else if (mGameState == GameState.GameState_Boss6FakeCredits)
			{
				DrawBoss6FakeCredits(g);
			}
			else if (mGameState == GameState.GameState_Boss6StoneHeadBurst)
			{
				DrawBoss6StoneHeadBurst(g);
			}
			else if (mGameState == GameState.GameState_Boss6DarkFrog)
			{
				DrawBoss6DarkFrog(g);
			}
			else if (mDoingIronFrogWin)
			{
				DrawIronFrogWin(g);
			}
			else if (mGameState == GameState.GameState_BossDead || (mGameState == GameState.GameState_LevelUp && mApp.mCredits != null))
			{
				int num = mLevel.mBoss.mDeathText.size();
				g.SetColor(0, 0, 0, (mStateCount < 255) ? mStateCount : 255);
				g.mTransX = 0f;
				g.FillRect(mApp.GetScreenRect());
				g.mTransX = mApp.mBoardOffsetX;
				if (mLevel.mCanDrawBoss && (!mLevel.mFinalLevel || !mAdventureWinScreen || mVortexAppear))
				{
					mLevel.mBoss.DrawDeathBGTikis(g);
				}
				if (!mAdventureWinScreen && !mDoingEndBossFrogEffect && mFrogFlyOff == null)
				{
					mFrog.Draw(g);
				}
				int num2 = -1;
				if (mLevel.mCanDrawBoss)
				{
					if (!mDoingEndBossFrogEffect || mBossSmokePoof.mFrameNum < (float)Common._M(96))
					{
						mLevel.mBoss.Draw(g);
					}
					if (mDoingEndBossFrogEffect)
					{
						num2 = 255 - mEndBossFrogTimer * Common._M(25);
						if (num2 < 0)
						{
							num2 = 0;
						}
					}
					g.mTransX = 0f;
					mLevel.mBoss.DrawDeathText(g, num2);
					g.mTransX = mApp.mBoardOffsetX;
					if (mDoingEndBossFrogEffect)
					{
						if (mDoingEndBossFrogEffect && mBossSmokePoof.mCurNumParticles > 0)
						{
							g.PushState();
							mBossSmokePoof.Draw(g);
							g.PopState();
						}
						if (mFrogFlyOff == null)
						{
							Graphics3D graphics3D = g.Get3D();
							bool flag4 = graphics3D != null && mEndBossFrogTimer <= END_BOSS_FROG_JUMP_TIME;
							float num3 = 1f;
							float num4 = Common._M(0.5f);
							num3 = ((mEndBossFrogTimer > END_BOSS_FROG_JUMP_TIME / 2) ? (1f + num4 - ((float)mEndBossFrogTimer - (float)END_BOSS_FROG_JUMP_TIME / 2f) / ((float)END_BOSS_FROG_JUMP_TIME / 2f) * num4) : (1f + (float)mEndBossFrogTimer / ((float)END_BOSS_FROG_JUMP_TIME / 2f) * num4));
							SexyTransform2D sexyTransform2D2 = new SexyTransform2D(init: false);
							if (flag4)
							{
								sexyTransform2D2.Translate(-mApp.mWidth / 2, -mApp.mHeight / 2);
								sexyTransform2D2.Scale(num3, num3);
								sexyTransform2D2.Translate(mApp.mWidth / 2, mApp.mHeight / 2);
								graphics3D.PushTransform(sexyTransform2D2);
							}
							mFrog.Draw(g);
							if (flag4)
							{
								graphics3D.PopTransform();
							}
						}
						else
						{
							mFrogFlyOff.Draw(g);
						}
					}
				}
				if ((num == 0 || mLevel.mBoss.mDeathText[num - 1].mAlpha >= 254f) && mLevel.mFinalLevel && mAdventureWinScreen)
				{
					if (mAdventureWinAlpha < 255f)
					{
						DrawVortex(g, draw_overlay: true);
					}
					if (mAdventureWinAlpha > 0f)
					{
						Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_ADVENTURE_WIN_BKGRND);
						Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_ADVENTURE_WIN_MS_ZUMA);
						Image imageByID3 = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_BURGER);
						Image imageByID4 = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_ADVENTURE_WIN_DOOR_MASK);
						Image imageByID5 = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_ADVENTURE_WIN_DOOR);
						Image imageByID6 = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_ADVENTURE_WIN_STAT);
						int theX = 520;
						int theY = 340;
						g.DrawImage(imageByID, Common._S(-80), 0);
						g.DrawImage(imageByID6, theX, theY);
						if (!mApp.IsHardMode())
						{
							g.DrawImage(imageByID2, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_BOSS_DARKFROG_ADVENTURE_WIN_MS_ZUMA) - 160), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_BOSS_DARKFROG_ADVENTURE_WIN_MS_ZUMA)));
						}
						else
						{
							g.DrawImage(imageByID3, Common._DS(Common._M(1087) - 160), Common._DS(Common._M1(176)));
						}
						g.DrawImage(imageByID5, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_BOSS_DARKFROG_ADVENTURE_WIN_DOOR) - 160), (int)((float)Common._DS(Res.GetOffsetYByID(ResID.IMAGE_BOSS_DARKFROG_ADVENTURE_WIN_DOOR)) + mAdventureWinDoorYOff));
						g.DrawImage(imageByID4, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_BOSS_DARKFROG_ADVENTURE_WIN_DOOR_MASK) - 160), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_BOSS_DARKFROG_ADVENTURE_WIN_DOOR_MASK)));
						if (g.Is3D())
						{
							Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_TORCHFLAME).Draw(g);
						}
						if (mAdventureWinAlpha < 255f)
						{
							g.mTransX = 0f;
							g.SetColor(0, 0, 0, 255 - (int)mAdventureWinAlpha);
							g.FillRect(mApp.GetScreenRect());
							g.mTransX = mApp.mBoardOffsetX;
						}
						List<ResID> list = new List<ResID>();
						list.Add(ResID.IMAGE_BOSS_DARKFROG_ADVENTURE_WIN_CONGRATS);
						list.Add(ResID.IMAGE_BOSS_DARKFROG_ADVENTURE_WIN_SECRET);
						int num5 = 0;
						int num6 = 0;
						num5 = 10;
						num6 = 4;
						g.PushState();
						if (mAdventureWinExtraAlpha < 255f)
						{
							g.SetColorizeImages(colorizeImages: true);
						}
						g.SetColor(255, 255, 255, (int)mAdventureWinExtraAlpha);
						for (int l = 0; l < list.size(); l++)
						{
							g.DrawImage(Res.GetImageByID(list[l]), Common._DS(Res.GetOffsetXByID(list[l]) - 160), Common._DS(Res.GetOffsetYByID(list[l])));
						}
						Font fontByID2 = Res.GetFontByID(ResID.FONT_SHAGLOUNGE45_YELLOW);
						g.SetFont(fontByID2);
						int num7 = Common._DS(Common._M(1386) - mApp.mOffset160X);
						g.DrawString(JeffLib.Common.UpdateToTimeStr(mTimeToBeatAdvMode, use_hour_field: true), num7 + num5, num6 + Common._DS(Common._M(824)));
						g.DrawString(SexyFramework.Common.CommaSeperate(mBeatGameNormalScore), num7 + num5, num6 + Common._DS(Common._M(890)));
						g.DrawString("+" + SexyFramework.Common.CommaSeperate(mBeatGameLives * mApp.GetLevelMgr().mBeatGamePointsForLife), num7 + num5, num6 + Common._DS(Common._M(950)));
						Font fontByID3 = Res.GetFontByID(ResID.FONT_SHAGLOUNGE45_RED);
						int num8 = ((mBeatGameLives > 99) ? 99 : mBeatGameLives);
						g.SetFont(fontByID3);
						g.DrawString("x" + num8, Common._DS(Common._M(1114)), num6 + Common._DS(Common._M1(950)));
						if (mBeatGameTotalScoreTally > 0)
						{
							g.DrawString(SexyFramework.Common.CommaSeperate(mBeatGameTotalScoreTally), Common._DS(Common._M(1440) - mApp.mOffset160X) + num5, num6 + Common._DS(Common._M1(1046)));
						}
						g.SetColorizeImages(colorizeImages: false);
						g.PopState();
					}
				}
			}
			if (!mLevel.IsFinalBossLevel() && mFakeCredits == null && mLevel.mEndSequence != 5 && mLevel.mEndSequence != 3 && mGameState != GameState.GameState_BossIntro && !mAdventureWinScreen && (mGameState != GameState.GameState_BossDead || mLevel.mEndSequence != 5))
			{
				g.SetFont(fontByID);
				g.SetColor(0, 0, 0, 128);
				Ratio aspectRatio = mApp.mGraphicsDriver.GetAspectRatio();
				int num9 = ((aspectRatio.mNumerator != 4 && aspectRatio.mDenominator != 3) ? Common._S(-80) : 0);
				int num10 = Common._S(Common._M(25));
				int num11 = mApp.mHeight - num10;
				if (!GauntletMode())
				{
					Common._S(Common._M1(65));
				}
				else
				{
					Common._S(Common._M(70));
				}
				if (mDisplayAceTime && !GauntletMode() && !IronFrogMode() && mAdventureMode)
				{
					Common._S(Common._M(35));
				}
				Common._M(1);
				if (!IronFrogMode() && mDisplayAceTime && mLevel.mBoss == null)
				{
					StringBuilder stringBuilder = new StringBuilder(TextManager.getInstance().getString(142));
					stringBuilder.Replace("$1", SexyFramework.Common.CommaSeperate(mLevel.mChallengePoints));
					stringBuilder.Replace("$2", SexyFramework.Common.CommaSeperate(mLevel.mChallengeAcePoints));
					string theString = stringBuilder.ToString();
					g.GetFont().StringWidth(theString);
					g.SetColor(SexyFramework.Graphics.Color.White);
					g.DrawString(theString, num9 + Common._S(Common._M(0)), num11 + Common._S(Common._M1(20)));
				}
			}
			int num12 = Common._M(300);
			if (GauntletMode() && mStateCount < num12 && mGameState != GameState.GameState_Losing)
			{
				float mTransX = g.mTransX;
				g.mTransX = 0f;
				Font fontByID4 = Res.GetFontByID(ResID.FONT_SHAGEXOTICA100_STROKE);
				Font fontByID5 = Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_STROKE);
				float num13 = (float)mStateCount * Common._M(0.5f);
				float num14 = (float)(mHeight / 2) - num13;
				int alpha = ((num12 - mStateCount > 127) ? 255 : ((num12 - mStateCount) * 2));
				g.SetFont(fontByID4);
				g.SetColor(255, 255, 255, alpha);
				g.WriteString(TextManager.getInstance().getString(125), 0, (int)num14, mWidth);
				num14 = ((Localization.GetCurrentLanguage() != Localization.LanguageType.Language_CH && Localization.GetCurrentLanguage() != Localization.LanguageType.Language_CHT) ? (num14 + (float)(g.GetFont().GetHeight() + Common._DS(Common._M(-80)))) : (num14 + (float)(g.GetFont().GetHeight() + Common._DS(Common._M(10)))));
				g.SetFont(fontByID5);
				StringBuilder stringBuilder2 = new StringBuilder(TextManager.getInstance().getString(126));
				stringBuilder2.Replace("$1", SexyFramework.Common.CommaSeperate(mLevel.mChallengePoints));
				g.WriteString(stringBuilder2.ToString(), 0, (int)num14, mWidth);
				g.mTransX = mTransX;
			}
			else if (GauntletMode() && mEndGauntletTimer >= 0 && mGauntletModeOver && mGauntletMultBarAlpha <= 0f)
			{
				float mTransX2 = g.mTransX;
				g.mTransX = 0f;
				int num15 = ((mEndGauntletTimer <= 50) ? ((int)((float)mEndGauntletTimer * 5.1f)) : 255);
				if (num15 > 255)
				{
					num15 = 255;
				}
				g.SetColor(255, 255, 255, num15);
				int num16 = END_GAUNTLET_TIME + gEndGauntletExtraTime - mEndGauntletTimer;
				float num17 = Common._DS(Common._M(500));
				string theString2 = TextManager.getInstance().getString(127);
				Font fontByID6 = Res.GetFontByID(ResID.FONT_SHAGEXOTICA100_STROKE);
				g.SetFont(fontByID6);
				float num18 = (mWidth - fontByID6.StringWidth(theString2)) / 2;
				float num19 = (float)num16 * num18 / Common._M(10f);
				if (num19 > num18)
				{
					num19 = num18;
				}
				g.WriteString(theString2, (int)num19, (int)num17, -1, -1);
				num17 += (float)Common._DS(Common._M(100));
				int num20 = Common._M(50);
				if (num16 > num20)
				{
					fontByID6 = Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_STROKE);
					g.SetFont(fontByID6);
					StringBuilder stringBuilder3 = new StringBuilder(TextManager.getInstance().getString(128));
					stringBuilder3.Replace("$1", mScoreMultiplier.ToString());
					stringBuilder3.Replace("%%", "%");
					theString2 = stringBuilder3.ToString();
					num18 = (mWidth - fontByID6.StringWidth(theString2)) / 2;
					num19 = (float)mWidth - (float)(num16 - num20) * num18 / Common._M(5f);
					if (num19 < num18)
					{
						num19 = num18;
					}
					g.WriteString(theString2, (int)num19, (int)num17, -1, -1);
					num17 += (float)fontByID6.mHeight;
				}
				int num21 = Common._M(50);
				if (num16 > num20 + num21)
				{
					StringBuilder stringBuilder4 = new StringBuilder(TextManager.getInstance().getString(129));
					stringBuilder4.Replace("$1", SexyFramework.Common.CommaSeperate((int)((float)mScoreMultiplier / 100f * (float)mScore)));
					theString2 = stringBuilder4.ToString();
					num18 = (mWidth - fontByID6.StringWidth(theString2)) / 2;
					float num22 = (float)mHeight - (float)(num16 - num20 - num21) * num17 / Common._M(15f);
					if (num22 < num17)
					{
						num22 = num17;
					}
					g.SetColor(SexyFramework.Graphics.Color.White);
					int num23 = Common._M(20);
					if (mEndGauntletTimer < num23)
					{
						num22 -= (float)(num23 - mEndGauntletTimer) * Common._M(40f);
					}
					g.WriteString(theString2, (int)num18, (int)num22, -1, -1);
				}
				g.mTransX = mTransX2;
			}
			if (mEndBossFadeAmt > 0f)
			{
				DeferOverlay(5);
			}
		}
		if (mTransitionScreenHolePct.mRamp == 6 && !mTransitionScreenHolePct.HasBeenTriggered())
		{
			DeferOverlay(0);
		}
		if (mGameState != GameState.GameState_ScorePage)
		{
			DrawHaloSwap(g);
		}
		if (mGauntletRetryBtn != null)
		{
			DeferOverlay(1);
		}
		bool flag5 = false;
		bool flag6 = false;
		if (mLevelTransition != null)
		{
			flag6 = true;
			if (mLevelTransition.mFrogEffect != null)
			{
				flag5 = !mLevelTransition.mFrogEffect.HasCompletedFlyOff();
			}
		}
		if (!mShowMapScreen && (!flag6 || flag5 || mGameState == GameState.GameState_BossIntro))
		{
			SetBoardOffset(g, enable: false);
			Image imageByID7 = Res.GetImageByID(ResID.IMAGE_UI_POLE);
			if (imageByID7 != null)
			{
				g.DrawImage(imageByID7, 0, 0);
				g.DrawImageMirror(imageByID7, 1013, 0);
			}
			SetBoardOffset(g, enable: true);
		}
		if (mLivesInfo != null)
		{
			g.mTransX = 0f;
			mLivesInfo.Draw(g);
			g.mTransX = mApp.mBoardOffsetX;
		}
		if (mLevelTransition != null && !mLevelTransition.IsDone() && !mDoingTransition)
		{
			SetBoardOffset(g, enable: false);
			if (mLevelTransition.GetState() == 2)
			{
				g.SetColor(SexyFramework.Graphics.Color.Black);
				g.FillRect(mApp.GetScreenRect());
			}
			mLevelTransition.Draw(g);
			if (mLevelTransition.mState == 1)
			{
				SetBoardOffset(g, enable: true);
				DrawAdventureStats(g);
			}
		}
	}

	public bool isResultPageInAdvMode()
	{
		if (mAdventureMode && mLevelTransition != null && !mLevelTransition.IsDone() && !mDoingTransition)
		{
			return mLevelTransition.mState == 1;
		}
		return false;
	}

	public override void DrawOverlay(Graphics g, int priority)
	{
		DrawChallengeStats(g);
		if (mFlashAlpha > 0)
		{
			g.SetColor(255, 255, 255, mFlashAlpha);
			g.FillRect(mApp.GetScreenRect());
		}
		if (mShowMapScreen && !mDoingFirstTimeIntro)
		{
			Graphics3D graphics3D = g.Get3D();
			SexyTransform2D sexyTransform2D = new SexyTransform2D(init: false);
			if (mMapScreen.mClosing && graphics3D != null)
			{
				SexyFramework.Misc.Point zoneCenter = mMapScreen.GetZoneCenter(mMapScreen.mDisplayedZone);
				int num = Common._DS(zoneCenter.mX) - Common._S(80) + (int)mMapScreen.mUnlockScrollAmt - mApp.mScreenBounds.mX;
				int num2 = Common._DS(zoneCenter.mY);
				sexyTransform2D.LoadIdentity();
				sexyTransform2D.Translate(-num, -num2);
				sexyTransform2D.Scale((float)(double)mIntroMapScale, (float)(double)mIntroMapScale);
				sexyTransform2D.Translate(num, num2);
				graphics3D.PushTransform(sexyTransform2D);
			}
			mMapScreen.Draw(g);
			if (mMapScreen.mClosing)
			{
				graphics3D?.PopTransform();
			}
		}
		g.mTransX = mApp.mBoardOffsetX;
		if (mFullScreenAlpha > 0)
		{
			g.mTransX = 0f;
			g.SetColor(0, 0, 0, Math.Min(mFullScreenAlpha, 255));
			g.FillRect(mApp.GetScreenRect());
			g.mTransX = mApp.mBoardOffsetX;
			if (mLevel.IsFinalBossLevel() || mLevel.mEndSequence == 2)
			{
				mLevel.DrawDaisRocks(g);
			}
		}
		if ((mPauseCount > 0 || mNumDrawFramesLeft > 0 || mReturnToMainMenu) && mChallengeHelp == null && mApp.mGenericHelp == null && !mDoingFirstTimeIntro && mEndBossFadeAmt <= 0f && mGauntletRetryBtn == null && !mShowMapScreen && mZumaTips.size() == 0 && mLevel.CanUpdate() && mCheckpointEffect == null && mLevelTransition == null)
		{
			if (!GameApp.USE_TRIAL_VERSION && !mShowBallsDuringPause && !IsPaused())
			{
				g.mTransX = 0f;
				g.SetColor(0, 0, 0, 255 * mPauseFade / 100);
				g.FillRect(mApp.GetScreenRect());
				g.mTransX = mApp.mBoardOffsetX;
			}
			if (mLevel.mBoss == null && !DoingIntros())
			{
				if (mLevel.mNum != int.MaxValue)
				{
					float mTransX = g.mTransX;
					g.mTransX = 0f;
					Font fontByID = Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_STROKE);
					g.SetFont(fontByID);
					g.SetColor(252, 244, 159);
					int theY = (GauntletMode() ? Common._S(Common._M(55)) : Common._S(Common._M1(55)));
					int theX = Common._S(Common._M(0));
					StringBuilder stringBuilder = new StringBuilder("$1-$2");
					stringBuilder.Replace("$1", ((mLevel.mZone - 1) * 10 + mLevel.mNum).ToString());
					stringBuilder.Replace("$2", mLevel.mDisplayName);
					g.WriteString(stringBuilder.ToString(), theX, theY, mWidth, 0);
					g.mTransX = mTransX;
				}
			}
			else if (mLevel.mBoss != null && !DoingIntros())
			{
				Boss mBoss = mLevel.mBoss;
				float mTransX2 = g.mTransX;
				g.mTransX = 0f;
				Font fontByID2 = Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_STROKE);
				g.SetFont(fontByID2);
				g.SetColor(252, 244, 159);
				int theY2 = (GauntletMode() ? Common._S(Common._M(55)) : Common._S(Common._M1(55)));
				int theX2 = Common._S(Common._M(0));
				g.WriteString(mBoss.mName, theX2, theY2, mWidth, 0);
				g.mTransX = mTransX2;
			}
			if (mDialogCount == 0)
			{
				Image imageByID = Res.GetImageByID(ResID.IMAGE_PAUSED);
				if (mLevel.mBoss == null && !DoingIntros())
				{
					if (IronFrogMode() || mLevel.IsFinalBossLevel())
					{
						g.DrawImage(imageByID, (mWidth - imageByID.mWidth) / 2, (mHeight - imageByID.mHeight) / 2);
					}
					else
					{
						int theY3 = (GauntletMode() ? Common._DS(Common._M(320)) : Common._DS(Common._M1(150)));
						g.DrawImage(imageByID, (mWidth - imageByID.mWidth) / 2, theY3);
						Font fontByID3 = Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_STROKE);
						g.SetFont(fontByID3);
						g.SetColor(252, 244, 159);
						int num3 = (GauntletMode() ? Common._S(Common._M(370)) : Common._S(Common._M1(325)));
						int num4 = Common._S(Common._M(0));
						if (mLevel.mNum != int.MaxValue)
						{
							if (mLevel.mIronFrog)
							{
								StringBuilder stringBuilder2 = new StringBuilder(TextManager.getInstance().getString(116));
								stringBuilder2.Replace("$1", mLevel.mNum.ToString());
								stringBuilder2.Replace("$2", mLevel.mDisplayName);
								g.WriteString(stringBuilder2.ToString(), num4, num3, mWidth, 0);
							}
							else
							{
								StringBuilder stringBuilder3 = new StringBuilder(TextManager.getInstance().getString(117));
								stringBuilder3.Replace("$1", ((mLevel.mZone - 1) * 10 + mLevel.mNum).ToString());
								stringBuilder3.Replace("$2", mLevel.mDisplayName);
								g.WriteString(stringBuilder3.ToString(), num4, num3, mWidth, 0);
							}
						}
						else
						{
							num3 -= fontByID3.GetHeight();
						}
						if (!GauntletMode() && !IronFrogMode())
						{
							num4 += Common._S(Common._M(436));
							num3 += fontByID3.GetHeight() + Common._S(Common._M(0));
							int num5 = ((mGameState == GameState.GameState_BeatLevelBonus) ? mLevelStats.mTimePlayed : (mStateCount - mIgnoreCount));
							if (num5 < 0)
							{
								num5 = 0;
							}
							if (mGameState != GameState.GameState_Losing)
							{
								StringBuilder stringBuilder4 = new StringBuilder("^8683a5^");
								stringBuilder4.Append(TextManager.getInstance().getString(118));
								g.WriteString(stringBuilder4.ToString(), num4, num3, -1, 1);
								g.WriteString("^85e6c3^" + JeffLib.Common.UpdateToTimeStr(num5), num4 + Common._S(Common._M(20)), num3, -1, -1);
								num3 += fontByID3.GetHeight() + Common._S(Common._M(0));
							}
							if (!mLevel.IsFinalBossLevel())
							{
								StringBuilder stringBuilder5 = new StringBuilder("^8683a5^");
								stringBuilder5.Append(TextManager.getInstance().getString(119));
								g.WriteString(stringBuilder5.ToString(), num4, num3, -1, 1);
								g.WriteString("^85e6c3^" + JeffLib.Common.UpdateToTimeStr(mLevel.mParTime), num4 + Common._S(Common._M(20)), num3, -1, -1);
								num3 += fontByID3.GetHeight() + Common._S(Common._M(0));
							}
							StringBuilder stringBuilder6 = new StringBuilder("^8683a5^");
							stringBuilder6.Append(TextManager.getInstance().getString(120));
							g.WriteString(stringBuilder6.ToString(), num4, num3, -1, 1);
							g.WriteString("^85e6c3^" + mApp.mUserProfile.GetAdvModeVars().mNumDeathsCurLevel, num4 + Common._S(Common._M(20)), num3, -1, -1);
							num3 += fontByID3.GetHeight() + Common._S(Common._M(0));
							StringBuilder stringBuilder7 = new StringBuilder("^8683a5^");
							stringBuilder7.Append(TextManager.getInstance().getString(121));
							g.WriteString(stringBuilder7.ToString(), num4, num3, -1, 1);
							g.WriteString("^85e6c3^" + (mLives - 1), num4 + Common._S(Common._M(20)), num3, -1, -1);
						}
						else if (GauntletMode())
						{
							num4 += Common._S(Common._M(436));
							num3 += fontByID3.GetHeight() + Common._S(Common._M(0));
							StringBuilder stringBuilder8 = new StringBuilder("^8683a5^");
							stringBuilder8.Append(TextManager.getInstance().getString(115));
							g.WriteString(stringBuilder8.ToString(), num4, num3, -1, 1);
							g.WriteString("^85e6c3^" + SexyFramework.Common.CommaSeperate(mRollerScore.GetTargetScore()), num4 + Common._S(Common._M(20)), num3, -1, -1);
							num3 += fontByID3.GetHeight() + Common._S(Common._M(0));
							StringBuilder stringBuilder9 = new StringBuilder("^8683a5^");
							stringBuilder9.Append(TextManager.getInstance().getString(122));
							g.WriteString(stringBuilder9.ToString(), num4, num3, -1, 1);
							g.WriteString("^85e6c3^" + SexyFramework.Common.CommaSeperate(mLevel.mChallengePoints), num4 + Common._S(Common._M(20)), num3, -1, -1);
							num3 += fontByID3.GetHeight() + Common._S(Common._M(0));
							StringBuilder stringBuilder10 = new StringBuilder("^8683a5^");
							stringBuilder10.Append(TextManager.getInstance().getString(123));
							g.WriteString(stringBuilder10.ToString(), num4, num3, -1, 1);
							g.WriteString("^85e6c3^" + SexyFramework.Common.CommaSeperate(mLevel.mChallengeAcePoints), num4 + Common._S(Common._M(20)), num3, -1, -1);
							num3 += fontByID3.GetHeight() + Common._S(Common._M(0));
						}
					}
				}
				else if (mLevel.mBoss != null)
				{
					g.DrawImage(imageByID, (mWidth - imageByID.mWidth) / 2, (mHeight - imageByID.mHeight) / 2);
				}
			}
		}
		else if (mZumaTips.size() > 0 && mPauseCount == 0)
		{
			mZumaTips[0].Draw(g);
		}
		if (mEndBossFadeAmt > 0f)
		{
			g.mTransX = 0f;
			g.SetColor(0, 0, 0, (int)Math.Min(mEndBossFadeAmt, 255f));
			g.FillRect(mApp.GetScreenRect());
			g.mTransX = mApp.mBoardOffsetX;
		}
		if (mShowDDSWindow)
		{
			g.SetColor(0, 0, 255, 200);
			int num6 = Common._DS(Common._M(110));
			int theWidth = Common._DS(Common._M(600));
			string text = "";
			if (GauntletMode())
			{
				num6 += Common._DS(Common._M(50));
				text = "\nGauntlet Difficulty Tier: " + (GameApp.gDDS.mCurGauntletDiffIdx + 1) + " (t=" + GameApp.gDDS.GetGauntletDiffLevel() + ")";
			}
			g.FillRect(Common._M(0), mHeight - num6, theWidth, num6);
			g.SetFont(Res.GetFontByID(ResID.FONT_MAIN22));
			g.SetColor(SexyFramework.Graphics.Color.White);
			int theX3 = Common._M(5);
			int theY4 = mHeight - num6 + g.mFont.GetAscent();
			g.WriteWordWrapped(new Rect(theX3, theY4, theWidth, num6), GameApp.gDDS.GetStatsString() + text);
		}
		if (mShowBossDDSWindow)
		{
			g.SetColor(0, 0, 255, 200);
			g.SetFont(Res.GetFontByID(ResID.FONT_MAIN22));
			string str = "";
			GameApp.gDDS.GetBossDebugString(ref str, colorize: true);
			int num7 = Common._M(250);
			int theMaxWidth = 0;
			int num8 = g.GetWordWrappedHeight(num7, str, -1, ref theMaxWidth, ref theMaxWidth) + g.GetFont().GetHeight();
			g.FillRect(mWidth - num7, mHeight - num8, num7, num8);
			g.SetColor(SexyFramework.Graphics.Color.White);
			int theX4 = mWidth - num7 + Common._M(5);
			int theY5 = mHeight - num8 + g.mFont.GetAscent();
			g.WriteWordWrapped(new Rect(theX4, theY5, num7, num8), str);
		}
		g.mTransX = 0f;
		if (mDoingFirstTimeIntro)
		{
			SexyTransform2D sexyTransform2D2 = new SexyTransform2D(init: false);
			if (mFrogFlyOff != null && mFrogFlyOff.mJumpOut)
			{
				if (mSmokePoof == null || mSmokePoof.mFrameNum >= (float)Common._M(108))
				{
					int num9 = mWidth / 2 + Common._DS(Common._M(-154));
					int num10 = Common._DS(Common._M(40));
					g.PushState();
					if (mCloakBossIntroAlpha < 255)
					{
						g.SetColorizeImages(colorizeImages: true);
						g.SetColor(255, 255, 255, mCloakBossIntroAlpha);
					}
					Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BOSS_LAME_CLOAKEDBOSS_ARMDOWN_SHADOW);
					Image imageByID3 = Res.GetImageByID(ResID.IMAGE_BOSS_LAME_CLOAKEDBOSS_ARMDOWN_REST);
					g.DrawImage(imageByID2, num9 + Common._DS(Res.GetOffsetXByID(ResID.IMAGE_BOSS_LAME_CLOAKEDBOSS_ARMDOWN_SHADOW)), num10 + Common._DS(Res.GetOffsetYByID(ResID.IMAGE_BOSS_LAME_CLOAKEDBOSS_ARMDOWN_SHADOW)));
					g.DrawImage(imageByID3, num9 + Common._DS(Res.GetOffsetXByID(ResID.IMAGE_BOSS_LAME_CLOAKEDBOSS_ARMDOWN_REST)), num10 + Common._DS(Res.GetOffsetYByID(ResID.IMAGE_BOSS_LAME_CLOAKEDBOSS_ARMDOWN_REST)));
					g.SetColorizeImages(colorizeImages: false);
					g.PopState();
				}
				if (mSmokePoof != null)
				{
					mSmokePoof.Draw(g);
				}
				bool flag = false;
				Font fontByID4 = Res.GetFontByID(ResID.FONT_BOSS_TAUNT);
				if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CH)
				{
					fontByID4.mAscent = 25;
				}
				g.SetFont(fontByID4);
				for (int i = 0; i < mIntroDialog.size(); i++)
				{
					SimpleFadeText simpleFadeText = mIntroDialog[i];
					if (simpleFadeText.mAlpha <= 0f)
					{
						break;
					}
					g.SetColor(255, 255, 255, (int)simpleFadeText.mAlpha);
					g.WriteString(simpleFadeText.mString, 0, mHeight / 2 + Common._DS(Common._M(-60)) + i * Common._DS(Common._M1(65)), mWidth, 0);
					if (i == mIntroDialog.size() - 1 && simpleFadeText.mAlpha >= 255f)
					{
						flag = true;
					}
				}
				if (flag && !mDoIntroFrogJump && gIntroRibbitTimer <= 0)
				{
					g.DrawImage(Res.GetImageByID(ResID.IMAGE_FROG_RIBBIT), (int)(Common._S(mFrogFlyOff.mFrogX) + (float)Common._S(Common._M(-60))), (int)(Common._S(mFrogFlyOff.mFrogY) - (float)Common._S(Common._M1(120))));
				}
			}
			Graphics3D graphics3D2 = g.Get3D();
			g.SetColorizeImages(colorizeImages: true);
			bool flag2 = Common._M(1) == 1;
			if ((double)mIntroMidAlpha > 0.0 && mShowMapScreen && GlobalMembers.gIs3D)
			{
				Common._S(1f);
				Common._DS(1f);
				float num11 = ((GameApp.mGameRes == 768) ? 0.36667f : ((GameApp.mGameRes == 640) ? 0.26667f : 0.53333f));
				float num12 = ((GameApp.mGameRes == 768) ? 360f : ((GameApp.mGameRes == 640) ? 210f : 0f));
				sexyTransform2D2.Translate(Common._S(-469.5f) + (float)mApp.mScreenBounds.mX, Common._S(-382.5f));
				sexyTransform2D2.RotateRad((float)mIntroRotate.GetOutVal());
				sexyTransform2D2.Scale((float)mIntroMidScale.GetOutVal(), (float)mIntroMidScale.GetOutVal());
				sexyTransform2D2.Translate(Common._S(469.5f) - (float)mApp.mScreenBounds.mX, Common._S(382.5f));
				sexyTransform2D2.Translate((float)(mIntroMidTransX.GetOutVal() * (double)num11) + num12, 0f);
				graphics3D2.PushTransform(sexyTransform2D2);
				g.SetColor(mIntroMidAlpha);
				g.DrawImage(Res.GetImageByID(ResID.IMAGE_UI_MAP_ZOOM), Common._S(-80), 0);
				graphics3D2.PopTransform();
				if (flag2)
				{
					sexyTransform2D2.LoadIdentity();
					sexyTransform2D2.Translate(Common._S(-450), Common._S(-500));
					sexyTransform2D2.Scale((float)mIntroFrogScale.GetOutVal(), (float)mIntroFrogScale.GetOutVal());
					sexyTransform2D2.Translate(Common._S(450), Common._S(500));
					sexyTransform2D2.Translate(Common._S(-468.75f) + (float)mApp.mScreenBounds.mX, Common._S(-382.5f));
					sexyTransform2D2.Scale((float)(mIntroMidScale.GetOutVal() / 4.0), (float)(mIntroMidScale.GetOutVal() / 4.0));
					sexyTransform2D2.RotateRad((float)(mIntroRotate.GetOutVal() + (double)Common._M(0.265f)));
					sexyTransform2D2.Translate(Common._S(468.75f) - (float)mApp.mScreenBounds.mX, Common._S(382.5f));
					float num13 = ((GameApp.mGameRes == 768) ? 2.5f : ((GameApp.mGameRes == 640) ? 2.5f : 0.53333f));
					sexyTransform2D2.Translate((float)(mIntroMidTransX.GetOutVal() * (double)num13), 0f);
					sexyTransform2D2.Translate(Common._S(0.25f), Common._S(0.25f));
					graphics3D2.PushTransform(sexyTransform2D2);
				}
			}
			if (mFrogFlyOff != null)
			{
				mFrogFlyOff.Draw(g);
			}
			if ((double)mIntroMidAlpha > 0.0 && mShowMapScreen && GlobalMembers.gIs3D && flag2)
			{
				graphics3D2.PopTransform();
			}
			if (mIntroDialog.back().mAlpha >= 255f && !mDoIntroFrogJump && gIntroRibbitTimer <= 0 && !mShowMapScreen)
			{
				g.SetFont(Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_STROKE));
				g.SetColor(Common._M(255), Common._M1(255), Common._M2(255));
				g.WriteString(TextManager.getInstance().getString(433), 0, Common._DS(Common._M(1170)), mWidth, 0);
			}
			if ((double)mIntroMidAlpha > 0.0 && mShowMapScreen && !GlobalMembers.gIs3D && (double)mIntroMidAlpha > 0.0 && ((double)mIntroMidAlpha < 1.0 || (double)mIntroMapAlpha == 0.0))
			{
				g.SetColor(0, 0, 0, (int)(255.0 * (double)mIntroMidAlpha));
				g.FillRect(mApp.GetScreenRect());
			}
			if (mShowMapScreen)
			{
				g.mTransX = 0f;
				if (GlobalMembers.gIs3D)
				{
					sexyTransform2D2.LoadIdentity();
					float num15;
					if (mDoingFirstTimeIntro && mDoingFirstTimeIntroZoomToGame && mShowMapScreen)
					{
						float num14 = ((GameApp.mGameRes == 768) ? 0.26667f : ((GameApp.mGameRes == 640) ? 0.26667f : 0.53333f));
						num15 = Math.Max((float)(double)mIntroMapScale * (num14 * 5f), 1f);
					}
					else
					{
						float num16 = ((GameApp.mGameRes == 768) ? 0.26667f : ((GameApp.mGameRes == 640) ? 0.26667f : 0.53333f));
						num15 = Math.Max((float)(double)mIntroMapScale * (num16 * 16f), 1f);
					}
					_ = mApp.mScreenBounds.mX;
					sexyTransform2D2.Translate(Common._S(-710.75f), Common._S(-372.5f));
					sexyTransform2D2.Scale(num15, num15);
					sexyTransform2D2.Translate(Common._S(710.75f), Common._S(372.5f));
					sexyTransform2D2.Translate(Common._S((float)mIntroMapTransX.GetOutVal()), 0f);
					graphics3D2.PushTransform(sexyTransform2D2);
				}
				if (graphics3D2 == null && (double)mIntroMapAlpha < 1.0 && (double)mIntroMapAlpha > 0.0)
				{
					g.SetColor(0, 0, 0, 255);
					g.FillRect(mApp.GetScreenRect());
				}
				if (graphics3D2 != null || mIntroMapAlpha.GetOutVal() > 0.0)
				{
					g.SetColorizeImages(GlobalMembers.gIs3D);
					mMapScreen.mAlpha.SetConstant(mIntroMapAlpha);
					mMapScreen.Draw(g);
				}
				if (GlobalMembers.gIs3D)
				{
					graphics3D2.PopTransform();
				}
			}
		}
		if (mIntroFadeAmt > 0f)
		{
			g.SetColor(0, 0, 0, (int)mIntroFadeAmt);
			g.SetClipRect(new Rect(-1, -1, 1067, 641));
			g.FillRect(new Rect(-1, -1, 1067, 641));
			g.SetClipRect(new Rect(0, 0, 1066, 640));
		}
		if (mTransitionScreenHolePct.mRamp == 6 && !mTransitionScreenHolePct.HasBeenTriggered())
		{
			Graphics3D graphics3D3 = g.Get3D();
			SexyTransform2D sexyTransform2D3 = new SexyTransform2D(init: false);
			sexyTransform2D3.Translate(-Common._S(mTransitionCenter.mX) - mApp.mScreenBounds.mX, -Common._S(mTransitionCenter.mY));
			sexyTransform2D3.Scale((float)mTransitionScreenScale.GetOutVal(), (float)mTransitionScreenScale.GetOutVal());
			sexyTransform2D3.Translate(Common._S(mTransitionCenter.mX) + mApp.mScreenBounds.mX, Common._S(mTransitionCenter.mY));
			graphics3D3.PushTransform(sexyTransform2D3);
			int num17 = mApp.mScreenBounds.mWidth;
			int num18 = mApp.mScreenBounds.mHeight;
			int num19 = Common._S(mTransitionCenter.mX);
			int num20 = Common._S(mTransitionCenter.mY);
			float num21 = (float)((double)Common._M(0.01f) + (double)Common._M1(0.4f) / ((double)mTransitionScreenHolePct + (double)Common._M2(0.003f)));
			SexyVertex2D[] theVertices = new SexyVertex2D[4]
			{
				new SexyVertex2D(-1f, -1f, (float)(0.5 + (double)(0f - ((float)num19 + g.mTransX)) / (double)num17 * (double)num21), (float)(0.5 + (double)(-num20) / (double)num17 * (double)num21)),
				new SexyVertex2D(num17, -1f, (float)(0.5 + (double)((float)(num17 - num19) - g.mTransX) / (double)num17 * (double)num21), (float)(0.5 + (double)(-num20) / (double)num17 * (double)num21)),
				new SexyVertex2D(-1f, num18, (float)(0.5 + (double)(0f - ((float)num19 + g.mTransX)) / (double)num17 * (double)num21), (float)(0.5 + (double)(num18 - num20) / (double)num17 * (double)num21)),
				new SexyVertex2D(num17, num18, (float)(0.5 + (double)((float)(num17 - num19) - g.mTransX) / (double)num17 * (double)num21), (float)(0.5 + (double)(num18 - num20) / (double)num17 * (double)num21))
			};
			graphics3D3.SetTextureWrap(0, inWrap: false);
			graphics3D3.SetTexture(0, Res.GetImageByID(ResID.IMAGE_TRANSPARENT_HOLE));
			graphics3D3.SetTextureLinearFilter(0, inLinear: true);
			graphics3D3.DrawPrimitive(0u, Graphics3D.EPrimitiveType.PT_TriangleStrip, theVertices, 2, new SexyFramework.Graphics.Color(255, 255, 255, 255), 0, 0f, 0f, blend: false, 0u);
			graphics3D3.PopTransform();
		}
	}

	public void CheckForExtraLifeFromBoss()
	{
		if (mNeedsBossExtraLife)
		{
			mNeedsBossExtraLife = false;
			if (mLevel.mZone != 6)
			{
				LivesChanged(1);
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_STOMP));
			}
		}
	}

	public void UpdateExtraLivesInfo()
	{
		if (mLivesInfo != null)
		{
			if (mLivesInfo.IsDone())
			{
				mLivesInfo.Dispose();
				mLivesInfo = null;
			}
			else
			{
				mLivesInfo.Update();
			}
		}
	}

	public void DrawChallengeStats(Graphics g)
	{
		if (mGauntletRetryBtn != null)
		{
			if (mGauntletAlpha < 255f)
			{
				g.SetColorizeImages(colorizeImages: true);
			}
			DrawChallengeStatsBackground(g);
			DrawChallengeStatsButtons(g);
			g.SetColorizeImages(colorizeImages: false);
			DrawChallengeGoalScores(g);
			mChallengeHeaderText.Draw(g);
			DrawChallengeScoreDetails(g);
			DrawChallengeHighScores(g);
		}
	}

	public void DrawChallengeStatsBackground(Graphics g)
	{
		g.SetColor(0, 0, 0, 128);
		g.FillRect(mApp.GetScreenRect());
		g.SetColor(255, 255, 255, (int)Math.Min(255f, mGauntletAlpha));
		Common.DrawCommonDialogBacking(g, mCStatsFrame.mX, mCStatsFrame.mY, mCStatsFrame.mWidth, mCStatsFrame.mHeight);
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_ADVENTURE_STATS_BANNER);
		g.DrawImage(imageByID, mCStatsFrame.mX + (mCStatsFrame.mWidth - imageByID.GetWidth()) / 2, Common._DS(140) - imageByID.GetHeight() / 2);
	}

	public void DrawChallengeStatsButtons(Graphics g)
	{
		if ((mGauntletRetryBtn != null || mGauntletQuitBtn != null) && MathUtils._geq(mGauntletAlpha, 255f))
		{
			g.PushState();
			g.Translate(mGauntletRetryBtn.mX, mGauntletRetryBtn.mY);
			mGauntletRetryBtn.mBtnNoDraw = false;
			mGauntletRetryBtn.Draw(g);
			mGauntletRetryBtn.mBtnNoDraw = true;
			g.PopState();
			g.PushState();
			g.Translate(mGauntletQuitBtn.mX, mGauntletQuitBtn.mY);
			mGauntletQuitBtn.mBtnNoDraw = false;
			mGauntletQuitBtn.Draw(g);
			mGauntletQuitBtn.mBtnNoDraw = true;
			g.PopState();
		}
	}

	public void DrawChallengeGoalScores(Graphics g)
	{
		float num = 0.75f;
		int num2 = 0;
		int num3 = 0;
		if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_PL)
		{
			num = 0.55f;
			num2 = 180;
			num3 = 40;
		}
		int num4 = (int)((float)mCStatsFrame.mWidth * 0.48f);
		int theHeight = (int)((float)mCStatsFrame.mHeight * 0.44f);
		int num5 = (int)((float)mCStatsFrame.mX + (float)mCStatsFrame.mWidth * 0.07f);
		int num6 = (int)((float)mCStatsFrame.mY + (float)mCStatsFrame.mHeight * 0.39f);
		g.PushState();
		g.SetColor(255, 255, 255, (int)Math.Min(255f, mGauntletAlpha));
		g.SetFont(Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_STROKE));
		g.SetScale(num, num, num5, num6);
		g.WriteWordWrapped(new Rect(num5, num6 + num3, num4 + num2, theHeight), GetGoalScoresInfo(), -1, 0);
		g.PopState();
	}

	public string GetGoalScoresInfo()
	{
		int num = (mLevel.mZone - 1) * 10 + mLevel.mNum;
		if (mLevel.mIronFrog)
		{
			num = 1;
			return "Iron Frog " + num + "\nChallenge:\n" + SexyFramework.Common.CommaSeperate(mLevel.mChallengePoints) + "\nAce:\n" + SexyFramework.Common.CommaSeperate(mLevel.mChallengeAcePoints);
		}
		StringBuilder stringBuilder = new StringBuilder(TextManager.getInstance().getString(684));
		stringBuilder.Replace("$1", num.ToString());
		stringBuilder.Replace("$2", SexyFramework.Common.CommaSeperate(mLevel.mChallengePoints));
		stringBuilder.Replace("$3", SexyFramework.Common.CommaSeperate(mLevel.mChallengeAcePoints));
		return stringBuilder.ToString();
	}

	public void DrawChallengeScoreDetails(Graphics g)
	{
		int theWidth = mCStatsFrame.mWidth;
		int num = (int)((float)mCStatsFrame.mHeight * 0.25f);
		int theX = mCStatsFrame.mX;
		int theY = (int)((float)mCStatsFrame.mY + (float)mCStatsFrame.mHeight * 0.13f);
		Rect rect = new Rect(theX, theY, theWidth, (int)((float)num * 0.25f));
		Rect theRect = new Rect(rect);
		theRect.mY += rect.mHeight * 3;
		string challengeScoreFlavorText = GetChallengeScoreFlavorText();
		StringBuilder stringBuilder = new StringBuilder(TextManager.getInstance().getString(685));
		stringBuilder.Replace("$1", SexyFramework.Common.CommaSeperate(mGauntletPointsFromMult));
		stringBuilder.Replace("$2", mScoreMultiplier.ToString());
		string theLine = stringBuilder.ToString();
		g.SetColor(255, 255, 255, (int)mChallengeTextAlpha);
		g.SetFont(Res.GetFontByID(ResID.FONT_SHAGLOUNGE45_GAUNTLET));
		g.WriteWordWrapped(rect, challengeScoreFlavorText, -1, 0);
		mChallengePtsText.Draw(g);
		g.SetFont(Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_GAUNTLET));
		g.WriteWordWrapped(theRect, theLine, -1, 0);
	}

	public string GetChallengeScoreFlavorText()
	{
		if (mScore < mLevel.mChallengePoints)
		{
			return TextManager.getInstance().getString(686);
		}
		if (mScore < mLevel.mChallengeAcePoints)
		{
			return TextManager.getInstance().getString(687);
		}
		return TextManager.getInstance().getString(688);
	}

	public void DrawChallengeHighScores(Graphics g)
	{
		List<Rect> challengeScoresTable = GetChallengeScoresTable();
		List<GauntletHSInfo> scores = new List<GauntletHSInfo>();
		mApp.mUserProfile.GetGauntletHighScores((mLevel.mZone - 1) * 10 + mLevel.mNum, ref scores);
		int num = ZumaProfile.MAX_GAUNTLET_HIGH_SCORES;
		if (mGauntletHSIndex < 5)
		{
			num++;
		}
		g.SetFont(Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_GAUNTLET2));
		DrawHighScoresHeader(g, challengeScoresTable.back());
		for (int i = 0; i < num; i++)
		{
			if (i != 5 || !(mScoreDisplayPos <= 4f))
			{
				List<Rect> challengeScoresTableRow = GetChallengeScoresTableRow(i, challengeScoresTable);
				DrawHighScoreEntry(g, i, scores, challengeScoresTableRow);
			}
		}
		DrawScoreExplosion(g, GetScoreRowColor(4));
	}

	public List<Rect> GetChallengeScoresTable()
	{
		int num = (int)((float)mCStatsFrame.mWidth * 0.48f);
		int num2 = (int)((float)mCStatsFrame.mHeight * 0.4f);
		int num3 = (int)((float)mCStatsFrame.mX + (float)mCStatsFrame.mWidth * 0.45f);
		int num4 = (int)((float)mCStatsFrame.mY + (float)mCStatsFrame.mHeight * 0.42f);
		num3 -= 60;
		if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CH || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CHT)
		{
			num3 -= 40;
		}
		Rect item = new Rect(num3, num4, num, (int)((float)num2 * 0.2f));
		int theY = num4 + item.mHeight;
		int theHeight = num2 - item.mHeight;
		int num5 = (int)((float)num * 0.15f);
		int num6 = (int)((float)num * 0.325f);
		int num7 = (int)((float)num * 0.05f);
		List<Rect> list = new List<Rect>();
		list.Resize(4);
		int num8 = 340;
		int num9 = 45;
		if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_RU || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_PL)
		{
			num8 = 350;
			num9 = 55;
		}
		if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CH || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CHT)
		{
			num8 = 380;
		}
		for (int i = 0; i < 4; i++)
		{
			if (i == 0)
			{
				list[i] = new Rect(num3, theY, num5, theHeight);
				continue;
			}
			int num10 = list[i - 1].mX + list[i - 1].mWidth;
			int num11 = ((i == 3) ? num5 : num6);
			num11 = ((i == 1) ? num8 : num11);
			if (i == 1)
			{
				num10 += num7;
			}
			if (i == 2)
			{
				num10 -= num9;
			}
			list[i] = new Rect(num10, theY, num11, theHeight);
		}
		list.Add(item);
		return list;
	}

	public void DrawHighScoresHeader(Graphics g, Rect inFrame)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_GAUNTLET_HIGHSCORES);
		int num = (int)((float)inFrame.mX + (float)(inFrame.mWidth - imageByID.GetWidth()) * 0.5f);
		int theY = inFrame.mY + (inFrame.mHeight - imageByID.GetHeight());
		g.DrawImage(imageByID, num + 80, theY);
	}

	public List<Rect> GetChallengeScoresTableRow(int inRowIdx, List<Rect> inColumns)
	{
		int num = (int)((float)inColumns[0].mHeight / (float)ZumaProfile.MAX_GAUNTLET_HIGH_SCORES);
		int scoreRowYPosition = GetScoreRowYPosition(inRowIdx, num, inColumns[0].mY);
		List<Rect> list = new List<Rect>();
		for (int i = 0; i < 4; i++)
		{
			list.Add(new Rect(inColumns[i].mX, scoreRowYPosition, inColumns[i].mWidth, num));
		}
		return list;
	}

	public int GetScoreRowYPosition(int inRowIdx, int inRowHeight, int inTableY)
	{
		float scoreRowBumpOffset = GetScoreRowBumpOffset();
		float num = 0f;
		int num2 = inRowIdx;
		if (inRowIdx == mGauntletHSIndex)
		{
			num2 = (int)mScoreDisplayPos;
			num = scoreRowBumpOffset;
		}
		else if (inRowIdx > mGauntletHSIndex)
		{
			int num3 = (int)mScoreDisplayPos + 1;
			if (inRowIdx == num3 && inRowIdx < 5)
			{
				num = 1f - scoreRowBumpOffset;
			}
			if (inRowIdx <= num3)
			{
				num2--;
			}
		}
		int num4 = inTableY;
		if (inRowIdx == mGauntletHSIndex && mScoreDisplayPos > 4f)
		{
			return num4 + (int)((float)(4 * inRowHeight) + (mScoreDisplayPos - 4f) * Common._S(200f));
		}
		return num4 + (int)(((float)num2 + num) * (float)inRowHeight);
	}

	public float GetScoreRowBumpOffset()
	{
		float num = mScoreDisplayPos - (float)(int)mScoreDisplayPos;
		return (float)((1.0 - Math.Cos(Math.Min(0.5, num * 1.25f) * 3.14159 * 2.0)) * 0.5);
	}

	public void DrawHighScoreEntry(Graphics g, int inEntryIdx, List<GauntletHSInfo> inEntries, List<Rect> inCells)
	{
		Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_GAUNTLET2);
		string theLine = (inEntryIdx + 1).ToString();
		string mProfileName = inEntries[inEntryIdx].mProfileName;
		string text = SexyFramework.Common.CommaSeperate(inEntries[inEntryIdx].mScore);
		if (inEntryIdx == 5)
		{
			ApplyExplosionEffectToScore(mProfileName, text, inCells);
		}
		g.PushState();
		g.SetColor(GetScoreRowColor(inEntryIdx));
		g.WriteWordWrapped(inCells[0], theLine, -1, 1);
		g.WriteWordWrapped(inCells[1], mProfileName);
		g.WriteWordWrapped(inCells[2], text, -1, 1);
		g.PopState();
		DrawCrown(g, inCells[3], inEntries[inEntryIdx].mScore);
	}

	public void ApplyExplosionEffectToScore(string inName, string inScore, List<Rect> inCells)
	{
		mScoreBreakStrings[0] = inName;
		mScoreBreakPositions[0] = new SexyFramework.Misc.Point(inCells[1].mX, inCells[1].mY);
		mScoreBreakStrings[1] = inScore;
		mScoreBreakPositions[1] = new SexyFramework.Misc.Point(inCells[2].mX, inCells[2].mY);
	}

	public SexyFramework.Graphics.Color GetScoreRowColor(int inRowIdx)
	{
		SexyFramework.Graphics.Color result;
		if (inRowIdx == mGauntletHSIndex)
		{
			result = new SexyFramework.Graphics.Color((int)mApp.HSLToRGB((int)((float)mUpdateCnt / 0.2f) % 255, 255, 220));
			result.mAlpha = (int)Math.Min(255f, mGauntletAlpha);
		}
		else
		{
			result = new SexyFramework.Graphics.Color(255, 255, 255, (int)Math.Min(255f, mGauntletAlpha));
		}
		return result;
	}

	public void DrawCrown(Graphics g, Rect inFrame, int inScore)
	{
		Image image = null;
		if (inScore >= mLevel.mChallengeAcePoints)
		{
			image = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_LARGE_ACECROWN);
		}
		else
		{
			if (inScore < mLevel.mChallengePoints)
			{
				return;
			}
			image = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_LARGE_CROWN);
		}
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_LARGE_CROWN);
		float num = 0.3f;
		int num2 = (int)((float)imageByID.GetWidth() * num);
		int num3 = (int)((float)imageByID.GetHeight() * num);
		int theX = (int)((float)inFrame.mX + (float)(inFrame.mWidth - num2) * 0.5f);
		int num4 = inFrame.mY + (inFrame.mHeight - num3);
		g.DrawImage(image, theX, num4 - Common._DS(8), num2, num3);
	}

	public void DrawScoreExplosion(Graphics g, SexyFramework.Graphics.Color inScoreColor)
	{
		for (int i = 0; i < mScoreLetterEffectVector.size(); i++)
		{
			ScoreLetterEffect scoreLetterEffect = mScoreLetterEffectVector[i];
			g.SetColor(inScoreColor);
			Graphics3D graphics3D = g.Get3D();
			SexyTransform2D sexyTransform2D = new SexyTransform2D(init: false);
			if (graphics3D != null)
			{
				sexyTransform2D.Translate(0f - scoreLetterEffect.mX - Common._S(5f), 0f - scoreLetterEffect.mY - Common._S(-8f));
				sexyTransform2D.RotateRad(scoreLetterEffect.mRot);
				sexyTransform2D.Translate(scoreLetterEffect.mX + Common._S(5f), scoreLetterEffect.mY + Common._S(-8f));
				graphics3D.PushTransform(sexyTransform2D);
			}
			g.DrawString(string.Concat(scoreLetterEffect.mChar), (int)scoreLetterEffect.mX, (int)scoreLetterEffect.mY);
			graphics3D?.PopTransform();
		}
	}

	public void DrawGuide(Graphics g)
	{
		if (!gDrawAutoAimAssistInfo || mGameState != GameState.GameState_Playing || mFrog.GetBullet() == null || IsPaused() || mFrog.GetType() != 0 || (!mShowGuide && mAccuracyCount <= 0))
		{
			return;
		}
		int num = 128;
		int theColor = 65535;
		Graphics3D graphics3D = g.Get3D();
		if (graphics3D == null || Common._M(1) == 0)
		{
			if (mFrog.GetBullet() != null)
			{
				int colorType = mFrog.GetBullet().GetColorType();
				theColor = Common.gBallColors[colorType];
				if (mApp.mColorblind && colorType == 3)
				{
					theColor = 8421504;
				}
				else if (mApp.mColorblind && colorType == 4)
				{
					theColor = 1973790;
				}
			}
			g.SetColor(new SexyFramework.Graphics.Color(theColor, num));
			g.PolyFill(mGuide, 4, convex: false);
			return;
		}
		SexyVertex2D[] array = new SexyVertex2D[3];
		array[0].x = mGuide[0].mX;
		array[0].y = mGuide[0].mY;
		array[1].x = mGuide[1].mX;
		array[1].y = mGuide[1].mY;
		array[2].x = mGuide[2].mX;
		array[2].y = mGuide[2].mY;
		for (int i = 0; i < 3; i++)
		{
			array[i].u = 0.5f;
			array[i].v = 0.5f;
		}
		SexyFramework.Graphics.Color[] array2 = new SexyFramework.Graphics.Color[6]
		{
			new SexyFramework.Graphics.Color(0, 80, 255),
			new SexyFramework.Graphics.Color(255, 255, 0),
			new SexyFramework.Graphics.Color(255, 0, 0),
			new SexyFramework.Graphics.Color(Common._M(0), Common._M1(200), Common._M2(0)),
			new SexyFramework.Graphics.Color(175, 0, 255),
			new SexyFramework.Graphics.Color(255, 255, 255)
		};
		SexyFramework.Graphics.Color theColor2 = ((mFrog.GetBullet() == null) ? new SexyFramework.Graphics.Color(SexyFramework.Graphics.Color.White) : array2[mFrog.GetBullet().GetColorType()]);
		if (mApp.mColorblind && mFrog.GetBullet().GetColorType() == 3)
		{
			theColor2 = new SexyFramework.Graphics.Color(128, 128, 128);
		}
		else if (mApp.mColorblind && mFrog.GetBullet().GetColorType() == 4)
		{
			theColor2 = new SexyFramework.Graphics.Color(30, 30, 30);
		}
		if (mFrog.GetBullet() != null && mFrog.GetBullet().GetColorType() == 1)
		{
			num /= Common._M(2);
		}
		theColor2.mAlpha = num;
		Ratio aspectRatio = mApp.mGraphicsDriver.GetAspectRatio();
		int num2 = ((aspectRatio.mNumerator != 4 && aspectRatio.mDenominator != 3) ? Common._S(80) : 0);
		num2 += Common._DS(Common._M(0));
		Common._DS(Common._M(0));
		if (mFrog.mShotCorrectionTarget.x != 0f || mFrog.mShotCorrectionTarget.y != 0f)
		{
			SexyFramework.Misc.Point[] theVertexList = new SexyFramework.Misc.Point[3]
			{
				mGuide[0],
				mGuide[1],
				new SexyFramework.Misc.Point((int)Common._S(mFrog.mShotCorrectionTarget.x), (int)Common._S(mFrog.mShotCorrectionTarget.y))
			};
			g.SetColor(200, 200, 200, 200);
			g.PolyFill(theVertexList, 3, convex: false);
			g.SetColor(255, 255, 255, 255);
			g.DrawRect((int)Common._S(mFrog.mShotCorrectionTarget.x - 2f), (int)Common._S(mFrog.mShotCorrectionTarget.y - 2f), (int)Common._S(4f), (int)Common._S(4f));
		}
		Image imageByID = Res.GetImageByID(ResID.IMAGE_FROG_ACCURACY_GUIDE);
		g.DrawTrianglesTexStrip(imageByID, array, 1, theColor2, 0, g.mTransX, g.mTransY, blend: true);
		g.DrawTrianglesTexStrip(imageByID, array, 1, theColor2, 1, g.mTransX, g.mTransY, blend: true);
		array[0].x = mGuide[3].mX;
		array[0].y = mGuide[3].mY;
		g.DrawTrianglesTexStrip(imageByID, array, 1, theColor2, 0, g.mTransX, g.mTransY, blend: true);
		g.DrawTrianglesTexStrip(imageByID, array, 1, theColor2, 1, g.mTransX, g.mTransY, blend: true);
	}

	public override void MouseLeave()
	{
		base.MouseLeave();
		mApp.SetCursor(ECURSOR.CURSOR_POINTER);
	}

	public override void MouseMove(int x, int y)
	{
		base.MouseMove(x, y);
		if (ShouldBlockInput())
		{
			return;
		}
		if (mShowMapScreen)
		{
			mMapScreen.MouseMove(x, y);
		}
		if (mPauseCount > 0 || mLevel == null || !mLevel.CanRotateFrog() || ShouldBlockInput() || mDoingFirstTimeIntro || mGameState == GameState.GameState_BossIntro || mGameState == GameState.GameState_Losing || mGameState == GameState.GameState_BossDead)
		{
			return;
		}
		if (mControlMode == CONTROL_MODE.CONTROL_MODE_DODGING)
		{
			if (IsPointAlongSlider(x, y))
			{
				mFatFingerGuideEnabled = false;
			}
			else
			{
				mControlMode = CONTROL_MODE.CONTROL_MODE_AIMING;
			}
		}
		else if (mControlMode == CONTROL_MODE.CONTROL_MODE_SWAPPING)
		{
			if (IsTouchOnFrogGun(x, y))
			{
				if (!mInvalidateTouchUp)
				{
					EnableHaloSwap(forceReset: false);
				}
				mFatFingerGuideEnabled = false;
				return;
			}
			if (IsPointAlongSlider(x, y))
			{
				mControlMode = CONTROL_MODE.CONTROL_MODE_DODGING;
				DisableHaloSwap(finishAnim: false);
			}
			else
			{
				mControlMode = CONTROL_MODE.CONTROL_MODE_AIMING;
				DisableHaloSwap(finishAnim: false);
			}
		}
		if (mControlMode == CONTROL_MODE.CONTROL_MODE_AIMING)
		{
			mFatFingerGuideEnabled = true;
			DisableHaloSwap(finishAnim: false);
		}
		float num = mFrog.GetDestAngle();
		float angle = mFrog.GetAngle();
		if (mControlMode != CONTROL_MODE.CONTROL_MODE_SWAPPING && mControlMode != CONTROL_MODE.CONTROL_MODE_NONE)
		{
			UpdateGunPos();
		}
		if (mTransitionScreenImage != null)
		{
			if (num >= 3.14159f)
			{
				num -= 6.28318f;
			}
			if (mFrog.mDestAngle >= 3.14159f)
			{
				mFrog.mDestAngle -= 6.28318f;
			}
			if (num < -3.14159f && mFrog.GetDestAngle() > 3.14159f)
			{
				mFrog.mDestAngle -= 6.28318f;
			}
			else if (num > 3.14159f && mFrog.GetDestAngle() < -3.14159f)
			{
				mFrog.mDestAngle += 6.28318f;
			}
			mFrog.mAngle = angle;
		}
		if (mFatFingerGuideEnabled || mInvalidateTouchUp)
		{
			return;
		}
		if (mLevel.mNumFrogPoints > 1 && mGameState == GameState.GameState_Playing)
		{
			int gunPointFromPos = mLevel.GetGunPointFromPos(Common._SS(x), Common._SS(y));
			if (gunPointFromPos >= 0 && gunPointFromPos != mLevel.mCurFrogPoint)
			{
				mMouseOverGunPos = gunPointFromPos;
			}
		}
		mMouseOverGunPos = -1;
		int num2 = ((mLevel.mBoss != null) ? mLevel.mBoss.mDeathText.size() : 0);
		if (mGameState != GameState.GameState_BossDead || num2 == 0 || mLevel.mBoss.mDeathText[num2 - 1].mAlpha < 255f)
		{
			mApp.SetCursor((mMouseOverGunPos >= 0) ? ECURSOR.CURSOR_HAND : ECURSOR.CURSOR_POINTER);
		}
	}

	public override void MouseDrag(int x, int y)
	{
		base.MouseDrag(x, y);
		MouseMove(x, y);
	}

	public override void MouseDown(int x, int y, int theClickCount)
	{
		if (ShouldBlockInput() || DoingLilyPadTutorial(x, y) || mGameState == GameState.GameState_BossDead)
		{
			return;
		}
		mIsMouseDown = true;
		DisableHaloSwap(finishAnim: false);
		if (IsTouchOnFrogGun(x, y))
		{
			mFatFingerGuideEnabled = false;
			EnableHaloSwap(forceReset: true);
			mInitialTouchPoint.mX = x;
			mInitialTouchPoint.mY = y;
			mControlMode = CONTROL_MODE.CONTROL_MODE_SWAPPING;
		}
		else if (IsPointAlongSlider(x - mApp.mBoardOffsetX, y))
		{
			UpdateGunPos();
			mControlMode = CONTROL_MODE.CONTROL_MODE_DODGING;
		}
		else if (mLevel.mNumFrogPoints > 1 && mLevel.CanRotateFrog())
		{
			mFatFingerGuideEnabled = true;
			mControlMode = CONTROL_MODE.CONTROL_MODE_AIMING;
			int gunPointFromPos = mLevel.GetGunPointFromPos(Common._SS(x - 80), Common._SS(y));
			if (gunPointFromPos >= 0 && gunPointFromPos != mLevel.mCurFrogPoint)
			{
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_LILLYPAD_JUMP));
				mLevel.mCurFrogPoint = gunPointFromPos;
				mFrog.SetDestPos(mLevel.mFrogX[gunPointFromPos], mLevel.mFrogY[gunPointFromPos], mLevel.mMoveSpeed, doingHop: true);
				mLevel.MouseDown(Common._SS(x), Common._SS(y), theClickCount);
				mLevel.ChangedPad(gunPointFromPos);
				mInvalidateTouchUp = true;
				mMouseOverGunPos = gunPointFromPos;
				mLevel.m_canGetAchievementNoJump = false;
			}
		}
		else
		{
			mFatFingerGuideEnabled = true;
			mControlMode = CONTROL_MODE.CONTROL_MODE_AIMING;
		}
		if (mControlMode != CONTROL_MODE.CONTROL_MODE_SWAPPING)
		{
			UpdateGunPos();
		}
		base.MouseDown(x, y, theClickCount);
	}

	public override void MouseUp(int x, int y, int theClickCount)
	{
		mMouseOverGunPos = -1;
		mFatFingerGuideEnabled = false;
		if (mUpdateCnt < 10)
		{
			return;
		}
		base.MouseUp(x, y, theClickCount);
		if ((mZumaTips.size() > 0 && mZumaTips[0].mClickDismiss && mZumaTips[0].mId == ZumaProfile.FRUIT_HINT && JeffLib.Common.RightClick(theClickCount)) || mPauseCount > 0)
		{
			return;
		}
		if (mDoingFirstTimeIntro)
		{
			if (mShowMapScreen)
			{
				if ((double)mIntroMapScale == 0.0 && !mDoIntroFrogJump)
				{
					mDoingFirstTimeIntroZoomToGame = true;
					mIntroMapScale.SetCurve(Common._MP("b;0,1,0.005714,1,####         ~~]L'"));
					mMapScreen.mClickToEnterAlpha.SetCurve(Common._MP("b;0,1,0.04,1,~###         ~####"));
					if (GlobalMembers.gIs3D)
					{
						mMapScreen.mExtrasAlpha.SetCurve(Common._MP("b;0,1,0.04,1,~###         ~####"));
					}
					mMapScreen.mIntroClosing = true;
					mMapScreen.mZoneOver = false;
					mApp.SetCursor(ECURSOR.CURSOR_POINTER);
					mApp.mUserProfile.mNeedsFirstTimeIntro = false;
					mDoIntroFrogJump = true;
				}
			}
			else if (gIntroRibbitTimer <= 0 && !mDoIntroFrogJump)
			{
				mApp.mSoundPlayer.Fade(Res.GetSoundByID(ResID.SOUND_SEAGULLS), inUnload: true);
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_MAPZOOMUP));
				mApp.PlaySong(12);
				mApp.SetCursor(ECURSOR.CURSOR_POINTER);
				SetupMapScreen(completed: false, from_load: false);
				mIntroMidAlpha.SetCurve(Common._MP("b+0,1,0.01,1,####   @~###      a~###"));
				mIntroMidScale.SetCurve(Common._MP("b+0.8,4,0,1,~###  D~###     p-=t>V#### 7#P##"), mIntroMidAlpha);
				mIntroMidTransX.SetCurve(Common._MP("b+0,410,0,1,####  T####     GLZ_]  (YUq?"), mIntroMidAlpha);
				mIntroMapTransX.SetCurve(Common._MP("b+0,0,0,1,####         ~~###"), mIntroMidAlpha);
				if (GlobalMembers.gIs3D)
				{
					mIntroMapAlpha.SetCurve(Common._MP("b+0,1,0,1,####    }####   M~### T~###"), mIntroMidAlpha);
				}
				else
				{
					mIntroMapAlpha.SetCurve(Common._MP("b+0,1,0,1,####    i#### z~###   <~###"), mIntroMidAlpha);
				}
				mIntroMapScale.SetCurve(Common._MP("b+0,1,0,1,~###  @~###    [KmD-   (#P##"), mIntroMidAlpha);
				mIntroRotate.SetCurve(Common._MP("b+0.1,-0.265,0,1,~###  D~###       ]####"), mIntroMidAlpha);
				mIntroMapPinAlpha.SetCurve(Common._MP("b+0,1,0,1,####       W####  I~###"), mIntroMidAlpha);
				mIntroFrogScale.SetCurve(Common._MP("b+0,1,0,1,~###      J~###   V9###"), mIntroMidAlpha);
			}
			return;
		}
		if (mShowMapScreen && !mMapScreen.mClosing)
		{
			mMapScreen.MouseDown(x, y);
			if (mMapScreen.mClosing)
			{
				mHasDoneIntroSounds = false;
				mIntroFadeAmt = 0f;
				mIntroMapScale.SetCurve(Common._MP("b;1,4,0.008,1,####         ~~Z{$"));
				if ((double)mMapScreen.mClickToEnterAlpha > 0.0)
				{
					mMapScreen.mClickToEnterAlpha.SetCurve(Common._MP("b;0,1,0.04,1,~###         ~####"));
				}
				if (GlobalMembers.gIs3D)
				{
					mMapScreen.mExtrasAlpha.SetCurve(Common._MP("b;0,1,0.04,1,~###         ~####"));
				}
				mMapScreen.mIntroClosing = true;
				mMapScreen.mZoneOver = false;
				mApp.SetCursor(ECURSOR.CURSOR_POINTER);
				return;
			}
		}
		if (mLevelTransition != null && mLevelTransition.IsDone() && mLevelTransition.mTransitionToStats)
		{
			if (mStatsState < 2)
			{
				mStatsState = 2;
			}
			else if ((mLevel.mNum < 10 || mGameState == GameState.GameState_BossIntro) && mGameState == GameState.GameState_BossIntro)
			{
				if (mDoingBossIntroText)
				{
					return;
				}
				if ((double)mBossIntroBGAlpha >= 0.5)
				{
					mDoingBossIntroText = true;
					mDoingBossIntroFightText = false;
					mBossIntroFramesLeft = Common._M(20);
					mBossIntroDirection = 1;
					mBossIntroAlpha = 0f;
					mBossIntroAlphaRate = 255f / (float)mBossIntroFramesLeft;
					mBattleTextY = mHeight;
					mBattleTextVY = ((float)(mHeight / 2 - Common._DS(Common._M(0))) - mBattleTextY) / (float)mBossIntroFramesLeft;
					mBossSmScale.SetConstant(1.0);
				}
				mLevelTransition.mTransitionToStats = false;
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_BOSS_BATTLE_INTRO));
				ContinueToNextLevel();
			}
		}
		bool outAllowBallFire = false;
		if (BlockInputForTutorial(x, y, out outAllowBallFire))
		{
			return;
		}
		if (mInvalidateTouchUp)
		{
			mInvalidateTouchUp = false;
		}
		else
		{
			if (mPauseUpdateCnt == mUpdateCnt || ShouldBlockInput() || mDoingEndBossFrogEffect)
			{
				return;
			}
			if (mGameState == GameState.GameState_BossDead && !mLevel.mBoss.mDoDeathExplosions)
			{
				int num = mLevel.mBoss.mDeathText.size();
				if (num == 0 || mLevel.mBoss.mDeathText[num - 1].mAlpha >= 254f)
				{
					if (mLevel.mFinalLevel)
					{
						if (mAdventureWinScreen)
						{
							return;
						}
						SetMenuBtnEnabled(enabled: false);
						InitVortex();
						mAdventureWinScreen = true;
						mAdventureWinTimer = Common._M(50);
						mApp.mUserProfile.GetAdvModeVars().mCurrentAdvLevel = 1;
						mApp.mUserProfile.GetAdvModeVars().mCurrentAdvZone = 1;
					}
					else
					{
						mApp.SetCursor(ECURSOR.CURSOR_POINTER);
						mDoingEndBossFrogEffect = true;
						mEndBossFrogTimer = 0;
						mBossSmokePoof.ResetAnim();
						Common.SetFXNumScale(mBossSmokePoof, mApp.Is3DAccelerated() ? 1f : Common._M(0.25f));
						mEndBossFadeAmt = 0f;
					}
				}
				else
				{
					mLevel.mBoss.ShowAllDeathText();
					if (mStateCount < 400)
					{
						mStateCount = 399;
					}
					mLevel.mBoss.mDoDeathExplosions = false;
				}
			}
			if (mGameState != GameState.GameState_Playing)
			{
				return;
			}
			if (mLevel.mBoss != null && !mLevel.mBoss.AllowFrogToFire() && !GauntletMode())
			{
				mLevel.mBoss.MouseDownDuringNoFire(x, y);
				return;
			}
			bool flag = false;
			if (!Common.gAddBalls && !mNeedsCheckpointIntro && !DoingIntros() && !mLevel.IsFinalBossLevel() && (mLevel.mBoss == null || !mLevel.mBoss.AllowFrogToFire()))
			{
				flag = true;
				mLevel.SkipInitialPathHilite();
				for (int i = 0; i < 2; i++)
				{
					FwooshImage fwooshImage = mLevelNameText[i];
					if (fwooshImage.mImage != null && fwooshImage.mAlpha != 0f && fwooshImage.mDelay > 0)
					{
						fwooshImage.mDelay = 1;
					}
				}
			}
			for (int j = 0; j < mLevel.mNumCurves; j++)
			{
				if (!mLevel.mCurveMgr[j].CanFire())
				{
					mLevel.MouseDown(x, y, theClickCount);
					return;
				}
			}
			if (mFrog.IsStunned())
			{
				mLevel.MouseDown(x, y, theClickCount);
			}
			else
			{
				if (flag && mLevel.CanFireBall() && (mZumaTips.size() == 0 || mZumaTips[0].mId != ZumaProfile.FIRST_SHOT_HINT))
				{
					return;
				}
				mLevel.MouseDown(x, y, theClickCount);
				if (IsTouchOnFrogGun(x, y))
				{
					DisableHaloSwap(finishAnim: true);
				}
				else
				{
					DisableHaloSwap(finishAnim: false);
				}
				if (mGameState != GameState.GameState_Playing || (GauntletMode() && mGauntletModeOver))
				{
					return;
				}
				if (Common.gSuckMode && mFrog.GetBullet() == null && mGuideBall != null)
				{
					int colorType = mGuideBall.GetColorType();
					PowerType powerType = mGuideBall.GetPowerType();
					float x2 = mGuideBall.GetX();
					float y2 = mGuideBall.GetY();
					for (int k = 0; k < mLevel.mNumCurves; k++)
					{
						if (mLevel.mCurveMgr[k].RemoveBall(mGuideBall))
						{
							mFrog.Reload2(colorType, delay: true, powerType, (int)x2, (int)y2);
							mGuideBall = null;
							break;
						}
					}
				}
				else if (mControlMode == CONTROL_MODE.CONTROL_MODE_AIMING)
				{
					UpdateGunPos();
					if ((mLevel.CanFireBall() || outAllowBallFire) && mFrog.StartFire())
					{
						mAllowBulletDetection = true;
						mLevelStats.mTotalShots++;
						mApp.mUserProfile.mBallsFired++;
						mLevel.PlayerStartedFiring();
						KillActiveTutorial(ZumaProfile.FIRST_SHOT_HINT);
						if (!mFrog.LaserMode() && !mFrog.LightningMode())
						{
							mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BALLFIRE));
							if (mLevel.mZone == 5)
							{
								mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_UNDERWATER_FROGFIRE));
							}
						}
						else if (mGuideBall != null && !mGuideBall.GetIsExploding())
						{
							if (mFrog.LaserMode())
							{
								mFrog.DecLazerCount();
								if (mFrog.GetLazerCount() <= 0)
								{
									mShowGuide = false;
								}
								for (int l = 0; l < mLevel.mNumCurves; l++)
								{
									int colorType2 = mGuideBall.GetColorType();
									float x3 = mGuideBall.GetX();
									float y3 = mGuideBall.GetY();
									if (!mLevel.mCurveMgr[l].DoLazerExplosion(mGuideBall))
									{
										continue;
									}
									mFrog.SetBulletType(colorType2);
									mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_LAZER));
									PIEffect pIEffect = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_LAZER_BLAST).Duplicate();
									Common.SetFXNumScale(pIEffect, 1f);
									pIEffect.mDrawTransform.LoadIdentity();
									pIEffect.mDrawTransform.Scale(Common._DS(1.4f), Common._DS(1.4f));
									pIEffect.mDrawTransform.Translate(Common._S(x3), Common._S(y3));
									mLazerBlasts.Add(pIEffect);
									if (mLevel.mZone == 5 && mLevel.mNum != 10 && mGameState == GameState.GameState_Playing)
									{
										for (int m = 0; m < Common._M(5); m++)
										{
											Bubble bubble = new Bubble();
											bubble.Init(Common._M(0), MathUtils.FloatRange(Common._M1(-1.5f), Common._M2(-0.75f)), MathUtils.FloatRange(Common._M3(0.05f), Common._M4(0.2f)), (int)MathUtils.FloatRange(Common._M5(15), Common._M6(25)));
											bubble.SetAlphaFade(Common._M(2f));
											bubble.SetX(x3 + (float)(-10 + MathUtils.SafeRand() % 20));
											bubble.SetY(y3);
											mFrog.AddBubble(bubble);
										}
									}
									break;
								}
							}
							else if (mFrog.LightningMode())
							{
								mFrog.DoLightningFrog(is_lightning: false);
								mFrog.FireElectricOrb();
								mShowGuide = false;
								mLevel.DeactivateLightningEffects();
								int colorType3 = mGuideBall.GetColorType();
								int num2 = -1;
								for (int n = 0; n < mLevel.mNumCurves; n++)
								{
									if (mLevel.mCurveMgr[n].HasBall(mGuideBall))
									{
										num2 = n;
										break;
									}
								}
								mLevel.mCurveMgr[num2].DetonateBalls(colorType3, from_lightning_frog: true, allow_powerups: true);
								mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_LIGHTNING_FIRED));
							}
						}
						else if (mGuideBall == null && mFrog.LaserMode() && mLazerHitTreasure)
						{
							if (mFrog.LaserMode())
							{
								mFrog.DecLazerCount();
								if (mFrog.GetLazerCount() <= 0)
								{
									mShowGuide = false;
								}
							}
							DoHitTreasure();
						}
					}
				}
				else if (mControlMode == CONTROL_MODE.CONTROL_MODE_SWAPPING)
				{
					SwapFrogBalls();
				}
				mControlMode = CONTROL_MODE.CONTROL_MODE_NONE;
				mIsMouseDown = false;
			}
		}
	}

	public void UnlockChallengeMode()
	{
		if (mLevel.mNum != 10 || mApp.mUserProfile.mChallengeUnlockState[mLevel.mZone - 1, 0] != 0)
		{
			return;
		}
		mApp.mUserProfile.mNewChallengeCupUnlocked = true;
		for (int i = 0; i < 10; i++)
		{
			if (i < 2 && mApp.mUserProfile.mChallengeUnlockState[mLevel.mZone - 1, i] == 0)
			{
				mApp.mUserProfile.mChallengeUnlockState[mLevel.mZone - 1, i] = 2;
			}
			else if (i >= 2 && mApp.mUserProfile.mChallengeUnlockState[mLevel.mZone - 1, i] == 0)
			{
				mApp.mUserProfile.mChallengeUnlockState[mLevel.mZone - 1, i] = 1;
			}
		}
		if (!GameApp.USE_TRIAL_VERSION)
		{
			mChallengeCupUnlockedFX = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_GOLDSPARKLE_AREA_STATS);
			mChallengeCupUnlockedFX.ResetAnim();
			mChallengeCupUnlockedFX.mEmitAfterTimeline = true;
			mChallengeCupUnlockedFX.mDrawTransform.LoadIdentity();
			float num = GameApp.DownScaleNum(1f);
			mChallengeCupUnlockedFX.mDrawTransform.Scale(num, num);
			mChallengeCupUnlockedFX.mDrawTransform.Translate(Common._DS(Common._M(1060)), Common._DS(Common._M1(906)));
			if (mLevel.mZone == 1)
			{
				ToggleNotification(TextManager.getInstance().getString(689), Res.GetSoundByID(ResID.SOUND_MIDZONE_NOTIFY));
			}
			else
			{
				ToggleNotification(TextManager.getInstance().getString(690), Res.GetSoundByID(ResID.SOUND_MIDZONE_NOTIFY));
			}
		}
	}

	public void ButtonPress(int id)
	{
		if (mMenuButton != null && mMenuButton.mId == id)
		{
			if (mLevelTransition == null && mApp.mBambooTransition != null && !mApp.mBambooTransition.IsInProgress())
			{
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BUTTON1));
			}
		}
		else
		{
			mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BUTTON2));
		}
	}

	public void ButtonPress(int theId, int count)
	{
		ButtonPress(theId);
	}

	public void ButtonDepress(int id)
	{
		if ((mApp.mBambooTransition != null && mApp.mBambooTransition.IsInProgress()) || mCheckpointEffect != null)
		{
			return;
		}
		if (mLevelTransition == null && mMenuButton != null && mMenuButton.mId == id)
		{
			mApp.DoOptionsDialog(ingame: true);
			mMenuButton.mDisabled = true;
		}
		else if (mIronFrogBtn != null && id == mIronFrogBtn.mId)
		{
			mApp.DoDeferredEndGame();
		}
		else if (mAdvWinBtn != null && id == mAdvWinBtn.mId)
		{
			mAdvWinBtn = null;
			mApp.DoCredits(isFromMainMenu: false);
			mForceRestartInAdvMode = true;
			mGameState = GameState.GameState_LevelUp;
			mLevelNum = 0;
			mApp.mUserProfile.GetAdvModeVars().mHighestZoneBeat = 6;
			if (!mApp.IsHardMode())
			{
				mApp.mUserProfile.mFirstTimeReplayingNormalMode = true;
			}
			else
			{
				mApp.mUserProfile.mFirstTimeReplayingHardMode = true;
			}
			mLives = 3;
			mScore = 0;
			mPointsLeftForExtraLife = mApp.GetLevelMgr().mPointsForLife;
			mRollerScore.ForceScore(0);
		}
		else if (mStatsContinueBtn != null && id == mStatsContinueBtn.mId)
		{
			mApp.mSoundPlayer.Stop(Res.GetSoundByID(ResID.SOUND_NEW_ADV_STATS_TALLY));
			if (mLevel.mNum == 10 || mGameState == GameState.GameState_BossIntro)
			{
				if (GameApp.USE_TRIAL_VERSION)
				{
					if (GameApp.gApp.mBoard != null)
					{
						GameApp.gApp.mBoard.Pause(pause: true, becauseOfDialog: true);
					}
					string message = TextManager.getInstance().getString(832);
					int width_pad = Common._DS(Common._M(20));
					GameApp.gApp.DoYesNoDialog(TextManager.getInstance().getString(448), message, block: true, TextManager.getInstance().getString(446), TextManager.getInstance().getString(447), drag: false, Common._S(Common._M(50)), 1, width_pad);
					GameApp.gApp.mYesNoDialogDelegate = ProcessTrialYesNo;
					mIsTryAndBuyDialogShowing = true;
					return;
				}
				for (int i = 0; i < 40; i++)
				{
					int num = ((i < 20) ? Common._M(230) : Common._M1(575));
					int num2 = Common._M(300);
					mSmokeParticles.Add(BambooTransition.SpawnSmokeParticle(num, num2, fast: false, slow_fade: false));
				}
				if (mGameState != GameState.GameState_BossIntro)
				{
					mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_FIGHT));
					gNeedBossIntroSound = true;
					mGameState = GameState.GameState_BossIntro;
					mStatsState = 2;
					mStatsBubbleScale.SetConstant(mStatsBubbleScale);
					mBossIntroBGAlpha.SetCurve(Common._MP("b+0,1,0.005,1,####    r####    Y~###S~###"));
					mBossSmScale.SetCurve(Common._MP("b+1,7,0,1,####         ~~###"), mBossIntroBGAlpha);
					mBossSmPosPct.SetCurve(Common._MP("b+0,1,0,1,#/05      }~###   $~###"), mBossIntroBGAlpha);
					mBossRedPct.SetCurve(Common._MP("b+0,1,0,1,####         ~jWDM"), mBossIntroBGAlpha);
					RemoveWidget(mStatsContinueBtn);
					mApp.SafeDeleteWidget(mStatsContinueBtn);
					mStatsContinueBtn = null;
				}
			}
			else if (mLevelTransition.mState == 1)
			{
				mLevelTransition.Open();
				if (mStatsContinueBtn != null)
				{
					mStatsContinueBtn.SetVisible(isVisible: false);
				}
			}
		}
		else if (mGauntletRetryBtn != null && id == mGauntletRetryBtn.mId)
		{
			SetNextLevelMusic(isBossLevel: false);
			PlayLevelMusic(0.008f);
			mStartingGauntletLevel = mApp.GetLevelMgr().GetStartingGauntletLevel(mLevel.mId);
			mScore = (mLevelBeginScore = 0);
			GameApp.gDDS.SetGauntletTime(0);
			GameApp.gDDS.SetGauntletPoints(0);
			mLevel.UpdateChallengeModeDifficulty();
			RestartLevel();
			mFruitMultiplier = 1;
			mScoreMultiplier = 1;
			RemoveWidget(mGauntletRetryBtn);
			RemoveWidget(mGauntletQuitBtn);
			mApp.SafeDeleteWidget(mGauntletQuitBtn);
			mApp.SafeDeleteWidget(mGauntletRetryBtn);
			mGauntletRetryBtn = (mGauntletQuitBtn = null);
			SetMenuBtnEnabled(enabled: true);
			mHasDoneIntroSounds = false;
			for (int j = 0; j < mLevel.mNumCurves; j++)
			{
				mLevel.mHoleMgr.GetHole(j).mDoDeathFade = false;
			}
		}
		else if (mGauntletQuitBtn != null && id == mGauntletQuitBtn.mId)
		{
			mApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.EndChallengeModeGame;
			mApp.ToggleBambooTransition();
		}
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

	public void ProcessHardwareBackButton()
	{
		if (mShowMapScreen)
		{
			return;
		}
		if (mDoingFirstTimeIntro && !mShowMapScreen)
		{
			GameApp.gApp.mBambooTransition.mTransitionDelegate = ClearFirstIntroForBack;
			GameApp.gApp.ToggleBambooTransition();
			GameApp.gApp.mMusic.StopAll();
			GameApp.gApp.OnHardwareBackButtonPressProcessed();
			return;
		}
		if (mGauntletMode && mChallengeHelp != null)
		{
			ChallengeHelpClosed();
			GameApp.gApp.OnHardwareBackButtonPressProcessed();
			return;
		}
		if (mIsTryAndBuyDialogShowing)
		{
			Dialog dialog = GameApp.gApp.GetDialog(1);
			if (dialog != null)
			{
				dialog.ButtonDepress(1001);
				GameApp.gApp.OnHardwareBackButtonPressProcessed();
				return;
			}
		}
		Dialog dialog2 = GameApp.gApp.GetDialog(2);
		if (dialog2 != null)
		{
			(dialog2 as OptionsDialog).ProcessHardwareBackButton();
		}
		else if (mBoardState == BoardState.BoardState_BackToMainMenuPrompt)
		{
			mBoardState = BoardState.BoardState_Game;
			Pause(pause: false, becauseOfDialog: true);
			GameApp.gApp.GetDialog(1).ButtonDepress(1001);
			GameApp.gApp.OnHardwareBackButtonPressProcessed();
		}
		else if (mBoardState == BoardState.BoardState_Game)
		{
			if (GauntletMode() && mChallengeHelp != null)
			{
				ChallengeHelpClosed();
				GameApp.gApp.OnHardwareBackButtonPressProcessed();
			}
			else
			{
				mApp.DoOptionsDialog(ingame: true);
				GameApp.gApp.OnHardwareBackButtonPressProcessed();
			}
		}
	}

	public void ProcessYesNo(int theId)
	{
		Pause(pause: false, becauseOfDialog: true);
		mBoardState = BoardState.BoardState_Game;
		if (theId == 1000)
		{
			GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.DoDeferredEndGame;
			GameApp.gApp.ToggleBambooTransition();
			GameApp.gApp.mMusic.StopAll();
		}
	}

	public void ProcessExitingEvent()
	{
		if (mShowMapScreen || mDoingFirstTimeIntro || mDoingTransition)
		{
			return;
		}
		if (mLevel.mBoss == null && isResultPageInAdvMode())
		{
			mForceToNextLevelInAdvMode = true;
			if (mTheNextLevel > mLevelNum)
			{
				mLevelNum++;
			}
		}
		else if (mGameState == GameState.GameState_BossIntro)
		{
			mForceToNextLevelInAdvMode = true;
			if (mTheNextLevel > mLevelNum)
			{
				mLevelNum++;
			}
		}
		DoShutdownSaveGame();
		mForceToNextLevelInAdvMode = false;
	}

	public void ProcessOnDeactiveEvent()
	{
		if (!mShowMapScreen && !mDoingFirstTimeIntro && !mDoingTransition && mPauseCount <= 0)
		{
			ButtonDepress(mMenuButton.mId);
		}
	}

	public override void KeyChar(char c)
	{
		if (((!mLevel.CanUseKeyboard() || ShouldBlockInput() || mDoingFirstTimeIntro || mTransitionScreenImage != null) && c != '@') || (c != ' ' && IsPaused()))
		{
			return;
		}
		if (c == ' ' && mDialogCount == 0)
		{
			mLastPauseTick = mUpdateCnt;
			Pause(mPauseCount == 0);
			return;
		}
		switch (c)
		{
		case 'T':
		case 't':
			mDisplayAceTime = !mDisplayAceTime;
			break;
		case 'J':
		case 'j':
			if (mLevel.mNumFrogPoints > 1 && !mFrog.IsHopping() && mZumaTips.size() == 0 && mLevel.CanRotateFrog() && !DoingIntros() && mLevelTransition == null && !ShouldBlockInput() && mGameState == GameState.GameState_Playing)
			{
				int num = 0;
				if (mLevel.mCurFrogPoint == 0)
				{
					num = 1;
				}
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_LILLYPAD_JUMP));
				mLevel.mCurFrogPoint = num;
				mFrog.SetDestPos(mLevel.mFrogX[num], mLevel.mFrogY[num], mLevel.mMoveSpeed, doingHop: true);
				mLevel.ChangedPad(num);
			}
			break;
		}
	}

	public new void KeyDown(KeyCode k)
	{
	}

	public override void AddedToManager(WidgetManager theWidgetManager)
	{
		base.AddedToManager(theWidgetManager);
		if (mLevel != null && mGameState != GameState.GameState_Boss6DarkFrog)
		{
			if (mGameState != GameState.GameState_Losing)
			{
				UpdateGunPos(level_begin: true);
			}
			mLevel.AfterBoardAdded();
		}
	}

	public override void RemovedFromManager(WidgetManager theWidgetManager)
	{
		base.RemovedFromManager(theWidgetManager);
		if (mSwapBallButton != null)
		{
			mWidgetManager.RemoveWidget(mSwapBallButton);
			mSwapBallButton.Dispose();
			mSwapBallButton = null;
		}
	}

	public override void GotFocus()
	{
		base.GotFocus();
		mWidgetManager.SetGamepadSelection(this, WidgetLinkDir.LINK_DIR_NONE);
	}

	public bool DoingLilyPadTutorial(int x, int y)
	{
		if (mZumaTips.size() == 0)
		{
			return false;
		}
		ZumaTip zumaTip = mZumaTips.First();
		if (zumaTip.mId == ZumaProfile.LILLY_PAD_HINT)
		{
			return !zumaTip.CutoutContainsPoint(x, y);
		}
		return false;
	}

	public void KillActiveTutorial(int inTipId)
	{
		if (mZumaTips.Count != 0 && mZumaTips.First().mId == inTipId)
		{
			mZumaTips.RemoveAt(0);
			mApp.mUserProfile.MarkHintAsSeen(inTipId);
		}
	}

	public bool BlockInputForTutorial(int x, int y, out bool outAllowBallFire)
	{
		outAllowBallFire = false;
		if (mZumaTips.size() == 0 || !mZumaTips.First().mClickDismiss)
		{
			return false;
		}
		ZumaTip zumaTip = mZumaTips.First();
		if (zumaTip.mUpdateCount < 50)
		{
			return true;
		}
		bool result = true;
		SoundAttribs soundAttribs = new SoundAttribs();
		soundAttribs.fadeout = 0.008f;
		DisableHaloSwap(finishAnim: true);
		if (zumaTip.mId == ZumaProfile.FRUIT_HINT)
		{
			if (mFrog.GetAngle() < 4.161289f || (double)mFrog.GetAngle() > 4.317596)
			{
				return true;
			}
			outAllowBallFire = true;
			result = false;
			mHasDoneIntroSounds = true;
			mApp.mSoundPlayer.Loop((mLevel.mZone == 5) ? Res.GetSoundByID(ResID.SOUND_UNDERWATER_ROLLOUT) : Res.GetSoundByID(ResID.SOUND_ROLLING), soundAttribs);
		}
		else if (zumaTip.mId == ZumaProfile.LILLY_PAD_HINT)
		{
			if (!zumaTip.CutoutContainsPoint(x, y))
			{
				return true;
			}
			result = false;
		}
		else if (zumaTip.mId == ZumaProfile.SWAP_BALL_HINT)
		{
			if (!IsTouchOnFrogGun(x, y))
			{
				return true;
			}
			mAllowBulletDetection = false;
			SwapFrogBalls();
		}
		mApp.mUserProfile.MarkHintAsSeen(zumaTip.mId);
		mZumaTips.RemoveAt(0);
		MarkDirty();
		if (mZumaTips.size() == 0)
		{
			mPreventBallAdvancement = false;
		}
		return result;
	}

	public new void GamepadButtonDown(GamepadButton theButton, int thePlayer, uint theFlags)
	{
	}

	public new void GamepadAxisMove(GamepadAxis theAxis, int thePlayer, float theAxisValue)
	{
	}

	public BetaStats GetBetaStats()
	{
		if (GauntletMode())
		{
			return mApp.mUserProfile.mChallengeBetaStats;
		}
		if (IronFrogMode())
		{
			return mApp.mUserProfile.mIronFrogBetaStats;
		}
		if (IsHardAdventureMode())
		{
			return mApp.mUserProfile.mHardAdvBetaStats;
		}
		return mApp.mUserProfile.mAdvBetaStats;
	}

	public void MultiplierBallAdded(Ball b)
	{
		MultiplierBallEffect item = new MultiplierBallEffect(b, spawn: true);
		mMultiplierBallEffects.Add(item);
	}

	public void DrawTunnels(Graphics g, int priority, bool below_shadow)
	{
		for (int i = 0; i < mTunnels[priority].size(); i++)
		{
			Tunnel tunnel = mTunnels[priority][i];
			if (tunnel.mAboveShadows == below_shadow)
			{
				continue;
			}
			float num = (float)GameApp.mGameRes / 640f;
			if (tunnel.mImage != null)
			{
				int w = (int)(num * (float)tunnel.mImage.mWidth);
				int h = (int)(num * (float)tunnel.mImage.mHeight);
				if (tunnel.mLayerId.Length == 0)
				{
					mLevel.DrawTunnel(g, tunnel.mImage, Common._S(tunnel.mX + GameApp.gScreenShakeX - 160), Common._S(tunnel.mY + GameApp.gScreenShakeY), w, h);
				}
				else
				{
					mLevel.DrawTunnel(g, tunnel.mImage, Common._DS(tunnel.mX + GameApp.gScreenShakeX - 160), Common._DS(tunnel.mY + GameApp.gScreenShakeY), w, h);
				}
			}
		}
	}

	public bool NeedSaveGame()
	{
		if (mGauntletMode || IronFrogMode() || (!mApp.mUserProfile.HasSeenHint(ZumaProfile.FIRST_SHOT_HINT) && mLevelNum == 1))
		{
			return false;
		}
		if (mGameState == GameState.GameState_None)
		{
			return false;
		}
		return true;
	}

	public void ResetInARowBonus()
	{
		if (mNumClearsInARow > mLevelStats.mMaxInARow)
		{
			mLevelStats.mMaxInARow = mNumClearsInARow;
			mLevelStats.mMaxInARowScore = mCurInARowBonus;
		}
		mNumClearsInARow = 0;
		mCurInARowBonus = 0;
		mLevel.ClearedInARowBonus();
	}

	public void ResetBallColorMap()
	{
		for (int i = 0; i < 6; i++)
		{
			mBallColorMap[i] = 0;
		}
	}

	public void ShakeScreen(int t, int xmax, int ymax)
	{
		mScreenShakeTime = t;
		mScreenShakeXMax = xmax;
		mScreenShakeYMax = ymax;
	}

	public void SyncState(DataSync theSync, bool onlyLife)
	{
		if (mGauntletMode)
		{
			return;
		}
		if (!theSync.isRead() && ShouldBypassFinalSequenceOnLoad())
		{
			mForceRestartInAdvMode = true;
			mLevelNum = 0;
		}
		SexyFramework.Misc.Buffer buffer = theSync.GetBuffer();
		theSync.SyncLong(ref mLives);
		theSync.SyncLong(ref mScore);
		theSync.SyncLong(ref mScoreTarget);
		theSync.SyncLong(ref mLevelBeginScore);
		mRollerScore.SyncState(theSync);
		if (onlyLife)
		{
			return;
		}
		mLevel.SyncState(theSync);
		if (theSync.isRead() && mLevel.mTorchStageState >= 8 && mLevel.mTorchStageState <= 9)
		{
			mFullScreenAlphaRate = 2;
		}
		mQRand.SyncState(theSync);
		mFrog.SyncState(theSync);
		if (theSync.isWrite() && mPreventBallAdvancement)
		{
			if (mZumaTips.Count > 0 && mZumaTips[0].mId == ZumaProfile.ZUMA_BAR_HINT)
			{
				buffer.WriteBoolean(theBool: false);
			}
			else
			{
				buffer.WriteBoolean(mPreventBallAdvancement);
			}
		}
		else
		{
			theSync.SyncBoolean(ref mPreventBallAdvancement);
		}
		theSync.SyncBoolean(ref mDoingEndBossFrogEffect);
		theSync.SyncLong(ref mEndBossFrogTimer);
		theSync.SyncFloat(ref mEndBossFadeAmt);
		if (theSync.isWrite())
		{
			Common.SerializePIEffect(mBossSmokePoof, theSync);
		}
		else
		{
			Common.DeserializePIEffect(mBossSmokePoof, theSync);
		}
		theSync.SyncLong(ref mAccuracyBackupCount);
		theSync.SyncFloat(ref mAdventureWinAlpha);
		theSync.SyncFloat(ref mAdventureWinDoorYOff);
		theSync.SyncBoolean(ref mHasSeenCheckpointIntro);
		theSync.SyncBoolean(ref mNeedsCheckpointIntro);
		theSync.SyncLong(ref mHallucinateTimer);
		theSync.SyncBoolean(ref mHasDoneIntroSounds);
		theSync.SyncLong(ref mScreenShakeTime);
		theSync.SyncLong(ref mScreenShakeXMax);
		theSync.SyncLong(ref mScreenShakeYMax);
		theSync.SyncLong(ref GameApp.gScreenShakeX);
		theSync.SyncLong(ref GameApp.gScreenShakeY);
		theSync.SyncLong(ref mIgnoreCount);
		if (theSync.isRead())
		{
			ClearPIEffects();
			int num = (int)buffer.ReadLong();
			for (int i = 0; i < num; i++)
			{
				EndLevelExplosion endLevelExplosion = mEndLevelExplosionPool.Alloc();
				endLevelExplosion.mDelay = (int)buffer.ReadLong();
				endLevelExplosion.mX = (int)buffer.ReadLong();
				endLevelExplosion.mY = (int)buffer.ReadLong();
				mEndLevelExplosions.Add(endLevelExplosion);
				Common.DeserializePIEffect(endLevelExplosion.mPIEffect, theSync);
			}
			num = (int)buffer.ReadLong();
			for (int j = 0; j < num; j++)
			{
				BallExplosion ballExplosion = mBallExplosionsPool.Alloc();
				ballExplosion.mPIEffect.ResetAnim();
				mBallExplosions.Add(ballExplosion);
				Common.DeserializePIEffect(ballExplosion.mPIEffect, theSync);
			}
			num = (int)buffer.ReadLong();
			for (int k = 0; k < num; k++)
			{
				PIEffect pIEffect = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_LAZER_BLAST).Duplicate();
				mLazerBlasts.Add(pIEffect);
				Common.DeserializePIEffect(pIEffect, theSync);
				Common.SetFXNumScale(pIEffect, 1f);
			}
			mPowerEffects.Clear();
			short num2 = buffer.ReadShort();
			for (int l = 0; l < num2; l++)
			{
				switch ((int)buffer.ReadLong())
				{
				case 2:
					mPowerEffects.Add(new ReversePowerEffect());
					break;
				case 4:
					mPowerEffects.Add(new CannonPowerEffect());
					break;
				default:
					mPowerEffects.Add(new PowerEffect());
					break;
				}
				mPowerEffects.back().SyncState(theSync);
			}
			mContinueNextLevelOnLoadProfile = buffer.ReadBoolean();
			if (buffer.ReadBoolean())
			{
				mNextLevelOverrideOnLoadProfile = (int)buffer.ReadLong();
			}
		}
		else
		{
			buffer.WriteLong(mEndLevelExplosions.Count);
			for (int m = 0; m < mEndLevelExplosions.Count; m++)
			{
				EndLevelExplosion endLevelExplosion2 = mEndLevelExplosions[m];
				buffer.WriteLong(endLevelExplosion2.mDelay);
				buffer.WriteLong(endLevelExplosion2.mX);
				buffer.WriteLong(endLevelExplosion2.mY);
				Common.SerializePIEffect(endLevelExplosion2.mPIEffect, theSync);
			}
			buffer.WriteLong(mBallExplosions.Count);
			for (int n = 0; n < mBallExplosions.Count; n++)
			{
				Common.SerializePIEffect(mBallExplosions[n].mPIEffect, theSync);
			}
			buffer.WriteLong(mLazerBlasts.Count);
			for (int num3 = 0; num3 < mLazerBlasts.Count; num3++)
			{
				Common.SerializePIEffect(mLazerBlasts[num3], theSync);
			}
			buffer.WriteShort((short)mPowerEffects.Count);
			for (int num4 = 0; num4 < mPowerEffects.Count; num4++)
			{
				buffer.WriteLong(mPowerEffects[num4].GetType());
				mPowerEffects[num4].SyncState(theSync);
			}
			bool flag = isResultPageInAdvMode();
			buffer.WriteBoolean(flag);
			buffer.WriteBoolean(mLevelTransition != null);
			if (mLevelTransition != null)
			{
				buffer.WriteLong(mLevelTransition.mNextLevelOverride);
				if (flag && mLevel.mNum != int.MaxValue)
				{
					int num5 = mLevel.mNum + 1;
					int num6 = mLevel.mZone;
					if (num5 == 11)
					{
						num5 = int.MaxValue;
					}
					else if (num5 > 11 || mLevel.mNum == int.MaxValue)
					{
						num5 = 1;
						num6++;
						if (num6 > 6)
						{
							num6 = 6;
							num5 = int.MaxValue;
						}
					}
					mApp.mUserProfile.GetAdvModeVars().mCurrentAdvLevel = num5;
					mApp.mUserProfile.GetAdvModeVars().mCurrentAdvZone = num6;
				}
			}
		}
		if (theSync.isRead())
		{
			int mPointsForLife = mApp.GetLevelMgr().mPointsForLife;
			int num7 = mScore / mPointsForLife;
			mPointsLeftForExtraLife = (num7 + 1) * mPointsForLife - mScore;
		}
		theSync.SyncLong(ref mFlashAlpha);
		theSync.SyncBoolean(ref mIsWinning);
		mLevelStats.SyncState(theSync);
		mGameStats.SyncState(theSync);
		theSync.SyncLong(ref mFruitMultiplier);
		theSync.SyncLong(ref mTreasureEndFrame);
		theSync.SyncLong(ref mStateCount);
		theSync.SyncLong(ref mAccuracyCount);
		theSync.SyncLong(ref mDestroyCount);
		theSync.SyncLong(ref mPauseFade);
		theSync.SyncLong(ref mMouseOverGunPos);
		theSync.SyncLong(ref mLevelEndFrame);
		theSync.SyncBoolean(ref mWasPerfectLevel);
		theSync.SyncLong(ref mNumDeaths);
		theSync.SyncLong(ref mLastIntroPad);
		theSync.SyncLong(ref mLastIntroPadDelay);
		theSync.SyncLong(ref mIntroPadHopCount);
		theSync.SyncLong(ref mNumClearsInARow);
		theSync.SyncLong(ref mCurInARowBonus);
		theSync.SyncLong(ref mCurComboScore);
		theSync.SyncLong(ref mCurComboCount);
		theSync.SyncLong(ref mNumCleared);
		theSync.SyncLong(ref mScoreMultiplier);
		theSync.SyncBoolean(ref mIsEndless);
		theSync.SyncBoolean(ref mDoGuide);
		theSync.SyncBoolean(ref mRecalcGuide);
		theSync.SyncBoolean(ref mRecalcLazerGuide);
		theSync.SyncBoolean(ref mDestroyAll);
		theSync.SyncBoolean(ref mLevelBeginning);
		theSync.SyncBoolean(ref mForceTreasure);
		theSync.SyncBoolean(ref mLazerHitTreasure);
		theSync.SyncLong(ref mNumZumaBalls);
		int theInt = (int)mGameState;
		theSync.SyncLong(ref theInt);
		mGameState = (GameState)theInt;
		if (mGameState == GameState.GameState_BeatLevelBonus)
		{
			mGameState = GameState.GameState_Playing;
		}
		if (mGameState == GameState.GameState_Playing && theSync.isRead())
		{
			mEndBossFadeAmt = 0f;
		}
		theSync.SyncLong(ref mLastBallClickTick);
		theSync.SyncLong(ref mLastExplosionTick);
		theSync.SyncLong(ref mLastSmallExplosionTick);
		if (theSync.isRead())
		{
			if (mGameState == GameState.GameState_BossIntro)
			{
				mGameState = GameState.GameState_Playing;
			}
			mGuideBall = null;
			mShowGuide = false;
			mRecalcLazerGuide = (mRecalcGuide = true);
			DeleteBullets();
			short num8 = buffer.ReadShort();
			for (int num9 = 0; num9 < num8; num9++)
			{
				Bullet bullet = new Bullet();
				bullet.SyncState(theSync);
				bullet.mFrog = mFrog;
				mBulletList.Add(bullet);
			}
			mCurTreasureNum = buffer.ReadShort();
			if (mCurTreasureNum > 0)
			{
				mMinTreasureY = (mMaxTreasureY = float.MaxValue);
				mCurTreasure = mLevel.mTreasurePoints[mCurTreasureNum - 1];
			}
			else
			{
				mCurTreasure = null;
			}
		}
		else
		{
			buffer.WriteShort((short)mBulletList.Count);
			for (int num10 = 0; num10 < mBulletList.Count; num10++)
			{
				mBulletList[num10].SyncState(theSync);
			}
			buffer.WriteShort((short)((mCurTreasure != null) ? mCurTreasureNum : (-1)));
		}
		theSync.SyncLong(ref mTreasureGlowAlpha);
		theSync.SyncLong(ref mTreasureGlowAlphaRate);
		theSync.SyncLong(ref mTreasureStarAlpha);
		theSync.SyncLong(ref mTreasureCel);
		theSync.SyncFloat(ref mTreasureStarAngle);
		theSync.SyncBoolean(ref mTreasureWasHit);
		if (theSync.isRead())
		{
			if (buffer.ReadBoolean())
			{
				mDeathSkull = new DeathSkull();
				mDeathSkull.SyncState(theSync);
			}
			if (mGameState == GameState.GameState_Boss6FakeCredits)
			{
				mFakeCredits = null;
				if (mLevel.mEndSequence == 3)
				{
					mGameState = GameState.GameState_Playing;
				}
				else
				{
					mFakeCredits = new FakeCredits();
					mFakeCredits.Init(mFrog);
					SetMenuBtnEnabled(enabled: false);
				}
			}
			else if (mGameState == GameState.GameState_Boss6StoneHeadBurst)
			{
				mBoss6StoneBurst = null;
				if (mLevel.mEndSequence == 4)
				{
					mGameState = GameState.GameState_Playing;
				}
				else
				{
					MakeBoss6StoneBurstComp();
				}
			}
			else if (mGameState == GameState.GameState_Boss6DarkFrog)
			{
				mBoss6VolcanoMelt = null;
				mDarkFrogSequence = null;
				if (mLevel.mEndSequence == 5)
				{
					mGameState = GameState.GameState_Playing;
				}
				else
				{
					mDarkFrogSequence = new DarkFrogSequence();
					mDarkFrogSequence.Init();
					SetMenuBtnEnabled(enabled: false);
				}
			}
			else if (mGameState == GameState.GameState_FinalBossPart1Finished)
			{
				if (mLevel.mEndSequence == 2)
				{
					mGameState = GameState.GameState_Playing;
				}
				else
				{
					mFrog.SetSlowTimer(Common._M(300));
					mVortexAppear = true;
					mVortexBGAlpha = 0f;
					mVortexFrogRadius = 0f;
					mVortexFrogAngle = mFrog.GetAngle();
					mVortexFrogScale = 1f;
					mVortexFrogRadiusExpand = true;
				}
			}
			mWasShowingCheckpoint = buffer.ReadBoolean();
			if (buffer.ReadBoolean())
			{
				mPreventBallAdvancement = false;
			}
			if (mGameState == GameState.GameState_BossIntro)
			{
				InitBossIntroState();
			}
			if (mWasShowingCheckpoint && mLives == 0)
			{
				DoCheckpointEffect(game_over: true);
			}
			if (mFruitExplodeEffect != null)
			{
				mFruitExplodeEffect.Reset();
			}
		}
		else
		{
			buffer.WriteBoolean(mDeathSkull != null);
			if (mDeathSkull != null)
			{
				mDeathSkull.SyncState(theSync);
			}
			buffer.WriteBoolean(mCheckpointEffect != null && mCheckpointEffect.mFromGameOver);
			if (mPreventBallAdvancement && mCheckpointEffect != null && !mCheckpointEffect.mFromGameOver && (mLevel.mBoss == null || mApp.IsHardMode() || mLevel.mZone > 1))
			{
				buffer.WriteBoolean(theBool: true);
			}
			else
			{
				buffer.WriteBoolean(theBool: false);
			}
		}
		theSync.SyncBoolean(ref mNeedsBossExtraLife);
		theSync.SyncLong(ref mEndLevelAceTimeBonus);
		theSync.SyncLong(ref mEndLevelNum);
		theSync.SyncLong(ref mEndLevelParTime);
		mEndLevelStats.SyncState(theSync);
		if (theSync.isRead())
		{
			if (mLevel != null && mLevel.mNum != int.MaxValue)
			{
				mEndLevelDisplayName = mLevel.mDisplayName;
			}
			else
			{
				mEndLevelDisplayName = "";
			}
		}
	}

	public void SaveGame(string fname, SexyFramework.Misc.Buffer w)
	{
		if (!mGauntletMode && !IronFrogMode())
		{
			SexyFramework.Misc.Buffer buffer = new SexyFramework.Misc.Buffer();
			bool flag = true;
			if (w == null)
			{
				w = buffer;
			}
			else
			{
				flag = false;
			}
			w.WriteLong(GameApp.gSaveGameVersion);
			w.WriteLong(mLevelNum);
			w.WriteString(mNextLevelIdOverride);
			if (ShouldBypassFinalSequenceOnLoad())
			{
				mForceRestartInAdvMode = true;
			}
			w.WriteBoolean(mForceRestartInAdvMode);
			w.WriteBoolean(mForceToNextLevelInAdvMode);
			if (mCheckpointEffect != null && mCheckpointEffect.mFromGameOver)
			{
				w.WriteBoolean(theBool: true);
				mApp.mUserProfile.GetAdvModeVars().mCurrentAdvScore = GetCheckpointScore();
			}
			else
			{
				w.WriteBoolean(theBool: false);
			}
			if (ShouldBypassFinalSequenceOnLoad())
			{
				w.WriteString("jungle1");
			}
			else
			{
				w.WriteString(mLevel.mId);
			}
			w.WriteBoolean(mApp.GetDialog(2) != null);
			DataSync dataSync = new DataSync(w, isRead: false);
			SyncState(dataSync, onlyLife: false);
			dataSync.SyncPointers();
			if (flag)
			{
				StorageFile.MakeDir(mApp.mUserProfile.GetSaveGameNameFolder());
				StorageFile.WriteBufferToFile(fname, buffer);
				mApp.SaveProfile();
				mApp.ClearUpdateBacklog(relaxForASecond: false);
			}
		}
	}

	public bool LoadGame(string fname)
	{
		if (mGauntletMode || (mLevel != null && IronFrogMode()))
		{
			return false;
		}
		SexyFramework.Misc.Buffer buffer = new SexyFramework.Misc.Buffer();
		StorageFile.MakeDir(mApp.mUserProfile.GetSaveGameNameFolder());
		if (!StorageFile.ReadBufferFromFile(fname, buffer))
		{
			return false;
		}
		return LoadGame(buffer);
	}

	public bool LoadGame(SexyFramework.Misc.Buffer b)
	{
		if (mGauntletMode)
		{
			return false;
		}
		mIsLoading = true;
		DataSync dataSync = new DataSync(b, isRead: true);
		int num = (int)b.ReadLong();
		if (num != GameApp.gSaveGameVersion)
		{
			mLevelNum = 1;
			Reset(game_over: true, level_reset: false);
			StartLevel(mLevelNum);
			UpdateGunPos(level_begin: true);
			mIsLoading = false;
			return false;
		}
		mLevelNum = (int)b.ReadLong();
		mNextLevelIdOverride = b.ReadString();
		bool flag = b.ReadBoolean();
		bool flag2 = b.ReadBoolean();
		b.ReadBoolean();
		string level_id = b.ReadString();
		if (flag)
		{
			mLevelNum = 1;
			StartLevel(mLevelNum);
			UpdateGunPos(level_begin: true);
			mIsLoading = false;
			return true;
		}
		if (flag2)
		{
			b.ReadBoolean();
			SyncState(dataSync, onlyLife: true);
			mLevelBeginScore = mScore;
			StartLevel(mLevelNum);
			UpdateGunPos(level_begin: true);
			mIsLoading = false;
			return true;
		}
		StartLevel(level_id, from_load: true, from_checkpoint: false, zone_restart: false, null);
		b.ReadBoolean();
		if (mLevelNameText[0] != null)
		{
			mLevelNameText[0].mDelay = 0;
		}
		if (mLevelNameText[1] != null)
		{
			mLevelNameText[1].mDelay = 0;
		}
		SyncState(dataSync, onlyLife: false);
		dataSync.SyncPointers();
		if (mHallucinateTimer > 0)
		{
			Font fontByID = Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_STROKE);
			mLevel.mFrog.mTempText = new BonusText(TextManager.getInstance().getString(691), fontByID, Common._S(mLevel.mFrog.GetCenterX() - Common._M(30)), Common._S(mLevel.mFrog.GetCenterY() - Common._M1(70)), Common._M2(0), Common._M3(0));
			mLevel.mFrog.mTempText.SetAlphaDecRate(0f);
		}
		if (!mLevel.mCanDrawBoss && mGameState == GameState.GameState_Playing)
		{
			mLevel.mCanDrawBoss = true;
		}
		mIsLoading = false;
		if (mGameState == GameState.GameState_BossDead)
		{
			SetMenuBtnEnabled(enabled: false);
		}
		mTheNextLevel = mLevelNum + 1;
		return true;
	}

	public void PlayBallClick(int theSound)
	{
		ulong num = SexyFramework.Common.SexyTime();
		if (num - mLastBallClickTick >= 250)
		{
			mApp.PlaySample(theSound);
			mLastBallClickTick = (uint)num;
		}
	}

	public void PlaySmallExplosionSound()
	{
		ulong num = SexyFramework.Common.SexyTime();
		if (num - mLastSmallExplosionTick > 100)
		{
			mLastSmallExplosionTick = (uint)num;
			mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BALLDESTROYED3));
		}
	}

	public void PlayExplosionSound()
	{
		ulong num = SexyFramework.Common.SexyTime();
		if (num - mLastExplosionTick > 250)
		{
			mLastExplosionTick = (uint)num;
			mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_EXPLODE));
		}
	}

	public int GetGoodBallColor()
	{
		int[] array = new int[6];
		int num = 0;
		bool flag = true;
		bool flag2 = false;
		for (int i = 0; i < 6; i++)
		{
			if (mBallColorMap[i] <= 0)
			{
				continue;
			}
			if (GauntletMode() && i == 4 && mNewBallDelay[0] == -1)
			{
				mNewBallDelay[0] = Common._M(1000);
			}
			else if (GauntletMode() && i == 5 && mNewBallDelay[1] == -1)
			{
				mNewBallDelay[1] = Common._M(1000);
			}
			else if (!GauntletMode() || (i != 5 && i != 4) || mNewBallDelay[i - 4] <= 0)
			{
				array[num++] = i;
				if (flag2 && mBallColorMap[i] >= 1)
				{
					flag = false;
				}
				if (mBallColorMap[i] >= 1)
				{
					flag2 = true;
				}
			}
		}
		if (num > 0)
		{
			bool flag3 = false;
			List<float> list = new List<float>();
			for (int j = 0; j < 6; j++)
			{
				if (mBallColorMap[j] > 0)
				{
					if (GauntletMode() && (j == 5 || j == 4) && mNewBallDelay[j - 4] > 0)
					{
						list.Add(0f);
					}
					else
					{
						list.Add(mLevel.GetRandomFrogBulletColor(num, j));
					}
				}
				else
				{
					list.Add(0f);
				}
				if ((mBallColorMap[j] > 0 && !mQRand.HasWeight(j)) || (mBallColorMap[j] == 0 && mQRand.HasWeight(j)))
				{
					flag3 = true;
				}
			}
			if (flag3)
			{
				mQRand.Clear();
				mQRand.SetWeights(list);
			}
		}
		if (mLevel.mNumCurves == 0 || (flag && !mLevel.DoingInitialPathHilite() && mScore == mLevelBeginScore))
		{
			Bullet bullet = mFrog.GetBullet();
			if (bullet == null)
			{
				bullet = mFrog.GetNextBullet();
			}
			return bullet?.GetColorType() ?? (MathUtils.SafeRand() % 4);
		}
		if (num <= 0)
		{
			return -1;
		}
		if (gNewStyleBallChooser)
		{
			return mQRand.Next();
		}
		return array[MathUtils.SafeRand() % num];
	}

	public int GetNumBallColors()
	{
		int num = 0;
		for (int i = 0; i < 6; i++)
		{
			if (mBallColorMap[i] > 0)
			{
				num++;
			}
		}
		return num;
	}

	public void CheckReload()
	{
		if (mFrog.GetBullet() != null && mFrog.GetNextBullet() != null)
		{
			if (AddBulletColorsToBoard() || PrepGunForBallSwapTutorial())
			{
				return;
			}
			EnsureBulletsAreUseful();
		}
		LoadEmptyGun();
	}

	public bool AddBulletColorsToBoard()
	{
		if (!Common.gSuckMode)
		{
			return false;
		}
		mBallColorMap[mFrog.GetBullet().GetColorType()]++;
		mBallColorMap[mFrog.GetNextBullet().GetColorType()]++;
		return true;
	}

	public bool PrepGunForBallSwapTutorial()
	{
		if (GauntletMode() || mLevelNum != 3 || mLevel.mZone != 1 || mApp.mUserProfile.HasSeenHint(ZumaProfile.SWAP_BALL_HINT))
		{
			return false;
		}
		while (mFrog.GetBullet().GetColorType() == mFrog.GetNextBullet().GetColorType())
		{
			mFrog.SetNextBulletType(SexyFramework.Common.Rand() % 4);
		}
		return true;
	}

	public void EnsureBulletsAreUseful()
	{
		if (!mAllowBulletDetection || mLevel.mNumCurves <= 0 || (mLevel.mNum == 1 && mLevel.mZone == 1 && !mApp.mUserProfile.HasSeenHint(ZumaProfile.FIRST_SHOT_HINT)))
		{
			return;
		}
		if (mBallColorMap[mFrog.GetBullet().GetColorType()] <= 0)
		{
			int goodBallColor = GetGoodBallColor();
			if (goodBallColor == -1)
			{
				return;
			}
			mFrog.SetBulletType(goodBallColor);
		}
		if (mBallColorMap[mFrog.GetNextBullet().GetColorType()] <= 0)
		{
			int goodBallColor2 = GetGoodBallColor();
			if (goodBallColor2 != -1)
			{
				mFrog.SetNextBulletType(goodBallColor2);
			}
		}
	}

	public void LoadEmptyGun()
	{
		while (mFrog.NeedsReload())
		{
			int num = mLevel.GetFrogReloadType();
			if (num == -1)
			{
				num = GetGoodBallColor();
			}
			if (num == -1)
			{
				break;
			}
			PowerType thePower = PowerType.PowerType_Max;
			mFrog.Reload(num, delay: true, thePower);
		}
	}

	public void ActivatePower(Ball theBall)
	{
		PowerType powerOrDestType = theBall.GetPowerOrDestType();
		ActivatePower(powerOrDestType, theBall.GetColorType(), (int)theBall.GetX(), (int)theBall.GetY());
		if ((powerOrDestType != PowerType.PowerType_ProximityBomb || mLevel.mBoss == null) && powerOrDestType != PowerType.PowerType_GauntletMultBall)
		{
			GetBetaStats().ActivatedPowerup((int)powerOrDestType);
		}
		for (int i = 0; i < mLevel.mNumCurves; i++)
		{
			mLevel.mCurveMgr[i].ActivatePower(theBall);
		}
		if (powerOrDestType != PowerType.PowerType_GauntletMultBall)
		{
			return;
		}
		mGauntletMultBarAlpha = 255f;
		mGauntletMultTextFlashOn = true;
		mGauntletMultTextFlashTimer = 0;
		mGauntletMultTextVX = (mGauntletMultTextVY = 0f);
		mGauntletMultTextMoveLastFrame = 0;
		if (mApp.GetLevelMgr().mMultBallPoints > 0)
		{
			IncScore(mApp.GetLevelMgr().mMultBallPoints, from_balls: false);
		}
		MultiplierBallEffect multiplierBallEffect = null;
		for (int j = 0; j < mMultiplierBallEffects.size(); j++)
		{
			MultiplierBallEffect multiplierBallEffect2 = mMultiplierBallEffects[j];
			if (multiplierBallEffect2.GetBall() == theBall)
			{
				multiplierBallEffect = multiplierBallEffect2;
				break;
			}
		}
		if (multiplierBallEffect == null)
		{
			multiplierBallEffect = new MultiplierBallEffect(theBall, spawn: false);
			mMultiplierBallEffects.Add(multiplierBallEffect);
		}
		mLevel.MultiplierActivated();
		mScoreMultiplier++;
		if (mScoreMultiplier == 11)
		{
			mApp.SetAchievement("score_mult_11x");
		}
		if (mScoreMultiplier > mApp.mUserProfile.mChallengeStats.mHighestMult)
		{
			mApp.mUserProfile.mChallengeStats.mHighestMult = mScoreMultiplier;
		}
		multiplierBallEffect.BallDestroyed(theBall);
		AddText(mScoreMultiplier + TextManager.getInstance().getString(97), mFrog.GetCenterX(), mFrog.GetCenterY(), Common._M(2f), -1, null);
	}

	public void ActivatePower(PowerType p, int color_type, int x, int y)
	{
		if (x == -1)
		{
			x = mWidth / 2;
		}
		if (y == -1)
		{
			y = mHeight / 2;
		}
		if ((p == PowerType.PowerType_Laser || p == PowerType.PowerType_Cannon || p == PowerType.PowerType_ColorNuke) && mAccuracyCount > 300)
		{
			mAccuracyBackupCount = mAccuracyCount;
			DoAccuracy(accuracy: false);
			mAccuracyCount = 300;
		}
		string t = Common.PowerupToStr(p, all_caps: true) + "!";
		if (p != PowerType.PowerType_ColorNuke && mFrog.LightningMode())
		{
			mLevel.DeactivateLightningEffects();
		}
		switch (p)
		{
		case PowerType.PowerType_ProximityBomb:
			PlayExplosionSound();
			if (mCurTreasure != null && MathUtils.CirclesIntersect(mCurTreasure.x, mCurTreasure.y, x, y, 108f))
			{
				mApp.mUserProfile.mFruitBombed++;
				if (mApp.mUserProfile.mFruitBombed >= 8)
				{
					mApp.SetAchievement("fruit_bomb_8x");
				}
				DoHitTreasure();
			}
			break;
		case PowerType.PowerType_MoveBackwards:
			mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BACKWARDS_BALL));
			AddText(t, x, y - 40);
			break;
		case PowerType.PowerType_SlowDown:
			mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_SLOWDOWN_BALL));
			AddText(t, x, y - 40);
			break;
		case PowerType.PowerType_Accuracy:
			AddText(t, x, y - 40);
			mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_ACCURACY_BALL));
			mAccuracyCount = 2000;
			DoAccuracy(accuracy: true);
			break;
		case PowerType.PowerType_Cannon:
			AddText(t, x, y - 40);
			mFrog.SetCannonCount(mApp.GetLevelMgr().mCannonShots, mApp.GetLevelMgr().mCannonStacks, color_type);
			break;
		case PowerType.PowerType_Laser:
			AddText(t, x, y - 40);
			mFrog.DoLazerFrog(mApp.GetLevelMgr().mLazerShots, mApp.GetLevelMgr().mLazerStacks);
			break;
		case PowerType.PowerType_ColorNuke:
			AddText(t, x, y - 40);
			UpdateGuide(lazer: true);
			mFrog.DoLightningFrog(is_lightning: true);
			break;
		case PowerType.PowerType_GauntletMultBall:
			mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_MULT_ACTIVATED));
			break;
		}
	}

	public void ActivatePower(PowerType p)
	{
		ActivatePower(p, -1, -1, -1);
	}

	public int IncScore(int theInc, bool from_balls, bool counts_towards_zuma)
	{
		if (theInc <= 0 || (from_balls && !mLevel.AllowPointsFromBalls()) || mLevel.IsFinalBossLevel() || (from_balls && mLevel.mBoss != null))
		{
			return 0;
		}
		int num = mScore;
		if (mLevel.mZumaBarState == -1)
		{
			mLevel.mZumaBarState = 0;
		}
		if (GauntletMode() && mLevel.mCurMultiplierTimeLeft > 0)
		{
			theInc *= mScoreMultiplier;
			mGauntletPointsFromMult += theInc;
		}
		if (counts_towards_zuma || GauntletMode())
		{
			mScore += theInc;
		}
		int result = theInc;
		if (GauntletMode())
		{
			if (num < mLevel.mChallengePoints && mScore >= mLevel.mChallengePoints)
			{
				ToggleNotification(TextManager.getInstance().getString(692));
				mApp.mUserProfile.mChallengeStats.mNumTimesHitScoreTarget++;
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_CHALLENGE_SCORE_MET));
			}
			else if (num < mLevel.mChallengeAcePoints && mScore >= mLevel.mChallengeAcePoints)
			{
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_CHALLENGE_ACE_MET));
				ToggleNotification(TextManager.getInstance().getString(693));
			}
			mApp.GetLevelMgr();
			mRollerScore.SetTargetScore(mScore);
			if (GameApp.gDDS.SetGauntletPoints(mScore))
			{
				mLevel.UpdateChallengeModeDifficulty();
			}
			if (mRollerScore.GetTargetScore() > mApp.mUserProfile.mChallengeStats.mHighestScore)
			{
				mApp.mUserProfile.mChallengeStats.mHighestScore = mRollerScore.GetTargetScore();
			}
			while (theInc > 0)
			{
				if (mGauntletPointsForDiffInc + theInc >= mApp.GetLevelMgr().mNumPointsForTimeAdd)
				{
					theInc -= mApp.GetLevelMgr().mNumPointsForTimeAdd - mGauntletPointsForDiffInc;
					mGauntletPointsForDiffInc = 0;
					if (GameApp.gDDS.AddMultiplierTime(mApp.GetLevelMgr().mPointTimeAdd))
					{
						mLevel.UpdateChallengeModeDifficulty();
					}
				}
				else
				{
					mGauntletPointsForDiffInc += theInc;
					theInc = 0;
				}
			}
			if (mRollerScore.GetTargetScore() > mGauntletHSTarget && gNeedsGauntletHSSound && mGauntletHSTarget > 0)
			{
				gNeedsGauntletHSSound = false;
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_CHALLENGE_BEST_SCORE));
				AddText(TextManager.getInstance().getString(98), mFrog.GetCenterX() - 60, mFrog.GetCenterY() + 60, 3f, -1, null);
			}
		}
		else
		{
			mRollerScore.SetTargetScore(mRollerScore.GetTargetScore() + theInc);
			if (mAdventureMode)
			{
				mApp.mUserProfile.GetAdvModeVars().mCurrentAdvScore = mRollerScore.GetTargetScore();
				CheckIfGotExtraLife(theInc);
			}
			else if (IronFrogMode() && mScore > mApp.mUserProfile.mIronFrogStats.mBestScore)
			{
				mPrevIFBestScore = mApp.mUserProfile.mIronFrogStats.mBestScore;
				mApp.mUserProfile.mIronFrogStats.mBestScore = mScore;
			}
		}
		return result;
	}

	public int IncScore(int theInc, bool from_balls)
	{
		return IncScore(theInc, from_balls, counts_towards_zuma: true);
	}

	public int GetPerfectBonus(int zone_override, int level_override)
	{
		if (zone_override == -1)
		{
			zone_override = mLevel.mZone;
		}
		if (level_override == -1)
		{
			level_override = mLevel.mNum;
		}
		if (level_override <= 10 && mWasPerfectLevel && !IronFrogMode())
		{
			return 1000 * zone_override;
		}
		return 0;
	}

	public int GetPerfectBonus()
	{
		return GetPerfectBonus(-1, -1);
	}

	public Ball GetGuideBall()
	{
		return mGuideBall;
	}

	public void GuideBallInvalidated()
	{
		mGuideBall = null;
	}

	public void GetGuideTargetCenter(out float x, out float y, bool lazer)
	{
		if (!lazer)
		{
			x = mGuideCenter.x;
			y = mGuideCenter.y;
		}
		else
		{
			x = mLazerGuideCenter.x;
			y = mLazerGuideCenter.y;
		}
	}

	public BonusTextElement AddText(string t, int x, int y, float bulge_pct, int attach_handle, Font theFont)
	{
		if (mLevel.mBoss != null || mLevel.IsFinalBossLevel())
		{
			return null;
		}
		if (bulge_pct > 3f)
		{
			bulge_pct = 3f;
		}
		Font font = ((theFont == null) ? Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_STROKE) : theFont);
		x = Common._S(x);
		y = Common._S(y);
		BonusText bonusText = new BonusText(t, font, x, y, Common._M(1f), Common._M1(200));
		bonusText.SetAlphaDecRate(Common._M(25f));
		bonusText.NoHSL();
		BonusTextElement bonusTextElement = new BonusTextElement();
		mText.Add(bonusTextElement);
		bonusTextElement.mParentHandle = attach_handle;
		bonusTextElement.mBonus = bonusText;
		bonusTextElement.mHandle = ++gTextHandle;
		BonusTextElement bonusTextElement2 = null;
		if (attach_handle != -1)
		{
			for (int i = 0; i < mText.size(); i++)
			{
				if (mText[i].mHandle != attach_handle)
				{
					continue;
				}
				bonusTextElement2 = mText[i];
				mText[i].mAttachedTo.Add(bonusTextElement.mHandle);
				if (bonusTextElement2.mAttachedTo.size() == 1)
				{
					bonusText.SetY(bonusTextElement2.mBonus.GetY() + (float)font.GetHeight());
					break;
				}
				int num = bonusTextElement2.mAttachedTo[bonusTextElement2.mAttachedTo.size() - 2];
				for (int j = 0; j < mText.size(); j++)
				{
					if (mText[j].mHandle == num)
					{
						bonusText.SetY(mText[j].mBonus.GetY() + (float)font.GetHeight());
						break;
					}
				}
				break;
			}
			int num2 = (int)bonusTextElement2.mBonus.GetX();
			int num3 = font.StringWidth(bonusTextElement2.mBonus.GetString());
			bonusText.SetX(num2 + (num3 - font.StringWidth(t)) / 2);
		}
		bool flag = true;
		int num4 = ((!flag) ? Common._S(-80) : 0);
		int num5 = (flag ? mWidth : (mWidth + Common._S(80)));
		List<BonusText> list = new List<BonusText>();
		Rect rect;
		if (bonusTextElement2 == null)
		{
			rect = new Rect(x, y, font.StringWidth(t), font.GetHeight());
			list.Add(bonusText);
		}
		else
		{
			list.Add(bonusTextElement2.mBonus);
			rect = new Rect((int)bonusTextElement2.mBonus.GetX(), (int)bonusTextElement2.mBonus.GetY(), font.StringWidth(bonusTextElement2.mBonus.GetString()), font.GetHeight() * 2);
			for (int k = 0; k < bonusTextElement2.mAttachedTo.size(); k++)
			{
				int num6 = bonusTextElement2.mAttachedTo[k];
				for (int l = 0; l < mText.size(); l++)
				{
					if (mText[l].mHandle == num6)
					{
						BonusText mBonus = mText[l].mBonus;
						list.Add(mBonus);
						rect.mHeight += font.GetHeight();
						int num7 = font.StringWidth(mBonus.GetString());
						if (num7 > rect.mWidth)
						{
							rect.mWidth = num7;
						}
						if (mBonus.GetX() < (float)rect.mX)
						{
							rect.mX = (int)mBonus.GetX();
						}
						break;
					}
				}
			}
		}
		Rect rect2 = rect;
		rect2.mWidth *= (int)bulge_pct;
		rect2.mHeight *= (int)bulge_pct;
		int num8 = 0;
		int num9 = 0;
		if (rect.mX < num4)
		{
			num8 = num4 - rect.mX;
		}
		else if (rect.mX + rect.mWidth > num5)
		{
			num8 = num5 - (rect.mWidth + rect.mX);
		}
		if (rect.mY < 0)
		{
			num9 = -rect.mY;
		}
		else if (rect.mY + rect.mHeight > mHeight)
		{
			num9 = mHeight - (rect.mHeight + rect.mY);
		}
		for (int m = 0; m < list.size(); m++)
		{
			list[m].SetX(list[m].GetX() + (float)num8);
			list[m].SetY(list[m].GetY() + (float)num9);
		}
		if (!MathUtils._eq(bulge_pct, 1f))
		{
			float num10 = Common._M(2);
			if (bulge_pct > 1.51f)
			{
				num10 += 1f;
			}
			float num11 = Common._M(1f);
			float num12 = num11 / num10;
			float num13 = (6f * num11 - num12 * 6f) / 0.1f;
			num11 = bulge_pct - 1f;
			num12 = num11 / num10;
			float rate = (6f * num11 - num12 * 6f) / num13;
			bonusText.Bulge(bulge_pct, rate, (int)num10);
			if (attach_handle != -1 && bonusTextElement2 != null)
			{
				bonusTextElement2.mBonus.Bulge(bulge_pct, rate, (int)num10);
				for (int n = 0; n < bonusTextElement2.mAttachedTo.size(); n++)
				{
					for (int num14 = 0; num14 < mText.size(); num14++)
					{
						if (mText[num14].mHandle == bonusTextElement2.mAttachedTo[n])
						{
							mText[num14].mBonus.Bulge(bulge_pct, rate, (int)num10);
							break;
						}
					}
				}
			}
		}
		return bonusTextElement;
	}

	public BonusTextElement AddText(string t, int x, int y)
	{
		return AddText(t, x, y, 1f, -1, null);
	}

	public void DrawRollerScore(Graphics g)
	{
		mRollerScore.Draw(g);
	}

	public void CheckShouldClearGuideBall(Ball b)
	{
		if (mGuideBall != null && mGuideBall == b)
		{
			mGuideBall = null;
			mRecalcGuide = true;
			mRecalcLazerGuide = true;
		}
	}

	public CurveMgr GetCurve(Ball b)
	{
		if (mLevel == null)
		{
			return null;
		}
		for (int i = 0; i < mLevel.mNumCurves; i++)
		{
			CurveMgr curveMgr = mLevel.mCurveMgr[i];
			if (curveMgr.mBallList.Contains(b))
			{
				return curveMgr;
			}
			if (curveMgr.mPendingBalls.Contains(b))
			{
				return curveMgr;
			}
		}
		return null;
	}

	public void CueLevelTransition(int next_level_override, bool dont_record_stats)
	{
		mBossSmScale.SetConstant(1.0);
		mStatsBubbleScale.SetCurve(Common._MP("b+0,2,0.003077,1,####         %####fW###9P###"));
		if (mLevelTransition == null)
		{
			mFrog.ResetFrogType();
			SetMenuBtnEnabled(enabled: false);
			if (!mLevel.mFinalLevel)
			{
				mLevelTransition = new LevelTransition(next_level_override, dont_record_stats);
				mLevelTransition.mTransitionToStats = !IronFrogMode();
				mLevelTransition.mIntroDelay = Common._M(150);
				if (!mLevelTransition.mDontRecordStats && mLevel.mBoss != null)
				{
					GameApp.gDDS.BossLevelComplete();
				}
			}
			else
			{
				GameApp.gDDS.BossLevelComplete();
			}
			mLevelPoints = mScore - mLevelBeginScore;
			if (mLevel.mBoss != null)
			{
				if (mLevel.mFinalLevel)
				{
					return;
				}
			}
			else if (IronFrogMode() && !GauntletMode())
			{
				GetBetaStats().BeatLevel(mLevelStats.mTimePlayed, mLevel.mParTime, GetAceTimeBonus(), GetPerfectBonus(), (float)mLevel.mFurthestBallDistance / 100f, mScore - mLevelBeginScore, mScore, -1);
				if (mScore > mApp.mUserProfile.mHighestIronFrogScore)
				{
					mNewIronFrogHS = true;
					mApp.mUserProfile.mHighestIronFrogScore = mScore;
					mApp.mUserProfile.mHighestIronFrogLevel = mApp.GetLevelMgr().GetLevelIndex(mLevel.mId) - mApp.GetLevelMgr().GetFirstIronFrogLevel() + 1;
					mApp.SaveProfile();
				}
				if (Common.StrEquals(mLevel.mId, mApp.GetLevelMgr().GetLevelId(mApp.GetLevelMgr().GetLastIronFrogLevel())))
				{
					return;
				}
			}
			else if (!IronFrogMode() && !GauntletMode())
			{
				mCurStatsPointCounter = (mCurStatsPointTarget = (mCurStatsPointInc = 0));
				int num = mLevel.mParTime - mLevelStats.mTimePlayed;
				if (num > 0)
				{
					mCurStatsPointInc = (mCurStatsPointTarget = GetAceTimeBonus()) / Common._M(200);
					if (mCurStatsPointInc <= 0)
					{
						mCurStatsPointInc = 1;
					}
				}
				mStatsState = 0;
				mStatsDelay = 0;
				if (mCurStatsPointInc <= 0)
				{
					mCurStatsPointInc = 1;
				}
				int aceTimeBonus = GetAceTimeBonus();
				int perfectBonus = GetPerfectBonus();
				CheckIfGotExtraLife(aceTimeBonus + perfectBonus);
				mScore += aceTimeBonus;
				mScore += perfectBonus;
				mScore += mCurveClearBonus;
				mRollerScore.ForceScore(mScore);
			}
			mHasDoneIntroSounds = false;
			mWasPerfectLevel = mApp.mUserProfile.GetAdvModeVars().mNumDeathsCurLevel == 0;
			mNumDeaths = mApp.mUserProfile.GetAdvModeVars().mNumDeathsCurLevel;
			_ = mApp.mUserProfile.GetAdvModeVars().mNumZumasCurLevel;
			_ = mApp.mUserProfile.GetAdvModeVars().mNumDeathsCurLevel;
			mApp.mUserProfile.GetAdvModeVars().mNumDeathsCurLevel = 0;
			mApp.mUserProfile.GetAdvModeVars().mNumZumasCurLevel = 0;
			if (!IronFrogMode() && !mGauntletMode && mScore > mApp.mUserProfile.mHighestAdvModeScore && mLevel.mBoss == null)
			{
				mApp.mUserProfile.mHighestAdvModeScore = mScore;
				mApp.mUserProfile.mAdvModeHSLevel = mLevel.mNum;
				mApp.mUserProfile.mAdvModeHSZone = mLevel.mZone;
				mApp.SaveProfile();
			}
			ResetInARowBonus();
			mGameStats.Add(mLevelStats);
			mStatsString = mLevel.GetStatsScreenText(mLevelStats, mLevelPoints);
			GetBetaStats().BeatLevel(mLevelStats.mTimePlayed, mLevel.mParTime, GetAceTimeBonus(), GetPerfectBonus(), (float)mLevel.mFurthestBallDistance / 100f, mScore - mLevelBeginScore, mScore, mLives);
			if (mLevel.mPreviewText != null && mLevel.mPreviewText.Length > 0)
			{
				string text = mStatsString;
				mStatsString = text + "\n\n^FFFFFF^" + TextManager.getInstance().getString(694) + "\n" + mLevel.mPreviewText;
			}
		}
		else
		{
			if (!mLevelTransition.mTransitionToStats)
			{
				return;
			}
			if (GlobalMembers.gIs3D)
			{
				mTransitionScreen = mApp.mGraphicsDriver.GetScreenImage();
				Graphics graphics = new Graphics(mTransitionScreen);
				graphics.Translate(-mApp.mScreenBounds.mX, 0);
				mWidgetManager.DrawWidgetsTo(graphics);
				mTransitionScreenImage = mTransitionScreen;
				mDoingTransition = true;
				int num2 = Common._M(35);
				int num3 = Res.GetOffsetXByID(ResID.IMAGE_UI_ADVENTURE_STATS_FROG) / 2 - Common._M(63) + (mLevel.mNum - 1) * num2;
				int num4 = ((mLevel.mNum == 10) ? num3 : (num3 + num2));
				int theX = num4;
				int theY = Res.GetOffsetYByID(ResID.IMAGE_UI_ADVENTURE_STATS_FROG) / 2 + Common._M(20);
				mTransitionCenter = new SexyFramework.Misc.Point(theX, theY);
				mTransitionScreenHolePct.SetCurve(Common._MP("b+0,1,0.006667,1,####    r.-0S     /d_0g"));
				mTransitionScreenScale.SetCurve(Common._MP("b+1,1.5,0.005,1,####   T####      L~_T6"));
				mTransitionFrogRotPct.SetCurve(Common._MP("b+0,1,0.005,1,####   L####      T~P##"));
				mTransitionFrogScale.SetCurve(Common._MP("b+0,4,0.005,1,-###   T.###M53*v   IeZ_]  V:;R-"), mTransitionFrogRotPct);
				mTransitionFrogPosPct.SetCurve(Common._MP("b+0,1,0,1,####   T#P##   *M2h3   E~###"), mTransitionFrogRotPct);
			}
			mLevelTransition.mTransitionToStats = false;
			mLevelTransition.Reset(intro: false);
			if (GlobalMembers.gIs3D)
			{
				mLevelTransition.mSilent = true;
			}
			if (mLevel.IsFinalBossLevel())
			{
				mLevelTransition.mDrawFrogEffect = false;
			}
			if (mLevel.mBoss == null && !mLevel.IsFinalBossLevel())
			{
				ContinueToNextLevel();
				UpdateGunPos();
				if (mFrog.mDestCount == 0)
				{
					mFrog.mDestX2 = (int)mFrog.mCurX;
					mFrog.mDestY2 = (int)mFrog.mCurY;
				}
				else
				{
					mFrog.mDestX1 = mFrog.mDestX2;
					mFrog.mDestY1 = mFrog.mDestY2;
				}
				mFrog.mCenterX = mFrog.mDestX2;
				mFrog.mCenterY = mFrog.mDestY2;
			}
		}
	}

	public void CueLevelTransition()
	{
		CueLevelTransition(-1, dont_record_stats: false);
	}

	public void SetupStatsScreen()
	{
		mStatsContinueBtn = new ButtonWidget(2, this);
		mStatsContinueBtn.mOverImage = Res.GetImageByID(ResID.IMAGE_GUI_ADVENTURESTATS_CONTINUE);
		mStatsContinueBtn.mButtonImage = Res.GetImageByID(ResID.IMAGE_GUI_ADVENTURESTATS_CONTINUE);
		mStatsContinueBtn.mDownImage = Res.GetImageByID(ResID.IMAGE_GUI_ADVENTURESTATS_CONTINUE_DOWN);
		mStatsContinueBtn.mDoFinger = true;
		Rect continueButtonRect = GetContinueButtonRect();
		mStatsContinueBtn.Resize(continueButtonRect);
		mBossIntroBGAlpha.SetConstant(0.0);
		mBossSmScale.SetConstant(1.0);
		mBossSmPosPct.SetConstant(0.0);
		mBossRedPct.SetConstant(0.0);
		AddWidget(mStatsContinueBtn);
	}

	public Rect GetContinueButtonRect()
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_ADVENTURESTATS_CONTINUE);
		int width = imageByID.GetWidth();
		int height = imageByID.GetHeight();
		int theX = (int)((float)(mAStatsFrame.mX + mAStatsFrame.mWidth) - (float)width * 0.84f);
		int theY = (int)((float)(mAStatsFrame.mY + mAStatsFrame.mHeight) - (float)height * 0.8f);
		return new Rect(theX, theY, width, height);
	}

	public void ContinueToNextLevel(int next_level_override, bool did_level_transition)
	{
		if (!did_level_transition)
		{
			ResetInARowBonus();
			mLevelStats.mTimePlayed = mStateCount - mIgnoreCount;
			mGameStats.Add(mLevelStats);
		}
		else if (mDarkFrogSequence == null)
		{
			SetMenuBtnEnabled(enabled: true);
		}
		DoAccuracy(accuracy: false);
		for (int i = 0; i < mLevel.mNumCurves; i++)
		{
			mLevel.mCurveMgr[i].DeleteBalls();
		}
		DeleteBullets();
		mFrog.EmptyBullets();
		mFrog.ClearBubbles();
		mStateCount = 0;
		mFruitMultiplier = 1;
		mScoreMultiplier = 1;
		Reset(game_over: false, level_reset: true, first_time_init: false, delete_bg: false);
		mDoPostBossMapScreen = false;
		if (next_level_override == -1)
		{
			int level_num = mLevelNum;
			if (mLevelNum < mTheNextLevel)
			{
				level_num = mLevelNum + 1;
			}
			if (mNextLevelIdOverride.Length > 0)
			{
				StartLevel(mNextLevelIdOverride, mIsLoading, from_checkpoint: false, zone_restart: false, null);
			}
			else if (!StartLevel(level_num, mIsLoading, from_checkpoint: false, zone_restart: false))
			{
				StartLevel(mLevelNum, mIsLoading, from_checkpoint: false, zone_restart: false);
			}
		}
		else
		{
			StartLevel(next_level_override, mIsLoading, from_checkpoint: false, zone_restart: false);
		}
		if (IsCheckpointLevel() && !mGauntletMode && !IronFrogMode())
		{
			switch (mLevel.mNum)
			{
			case int.MaxValue:
				mApp.mUserProfile.GetAdvModeVars().mCheckpointScores[mLevel.mZone - 1].mBoss = mScore;
				break;
			case 1:
				mApp.mUserProfile.GetAdvModeVars().mCheckpointScores[mLevel.mZone - 1].mZoneStart = mScore;
				break;
			default:
				mApp.mUserProfile.GetAdvModeVars().mCheckpointScores[mLevel.mZone - 1].mMidpoint = mScore;
				break;
			}
		}
		mTheNextLevel = mLevelNum + 1;
		bool flag = mLevel.mBoss != null;
		SetNextLevelMusic(flag);
		if (!flag || mLevel.mBoss.mResGroup != "Boss6_DarkFrog")
		{
			PlayLevelMusic(0.005f);
		}
		mApp.mSoundPlayer.Stop(Res.GetSoundByID(ResID.SOUND_NEW_ADV_STATS_TALLY));
		SaveGame(mApp.mUserProfile.GetSaveGameName(mApp.IsHardMode()), null);
		UpdateGunPos(level_begin: true);
	}

	public void ContinueToNextLevel()
	{
		ContinueToNextLevel(-1, did_level_transition: true);
	}

	public void SetNextLevelMusic(bool isBossLevel)
	{
		if (isBossLevel)
		{
			gTuneNum = 6;
			return;
		}
		int num;
		do
		{
			num = SexyFramework.Common.Rand(6);
		}
		while (gTuneNum == num);
		gTuneNum = num;
	}

	public void PlayLevelMusic(float inFadeSpeed)
	{
		int song = 12;
		switch (gTuneNum)
		{
		case 0:
			song = 12;
			break;
		case 1:
			song = 24;
			break;
		case 2:
			song = 35;
			break;
		case 3:
			song = 45;
			break;
		case 4:
			song = 58;
			break;
		case 5:
			song = 71;
			break;
		case 6:
			song = 127;
			break;
		}
		mApp.PlaySong(song, inFadeSpeed);
	}

	public void EndGauntletMode(bool hit_max_time)
	{
		if (mGauntletRetryBtn != null || mGauntletQuitBtn != null || mEndGauntletTimer > 0 || mGauntletModeOver)
		{
			return;
		}
		mApp.mSoundPlayer.Stop(Res.GetSoundByID(ResID.SOUND_LIGHTNING_LOOP));
		mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_TIME_UP_CHALLENGE));
		GetBetaStats().BeatLevel(mLevelStats.mTimePlayed, mLevel.mParTime, mScoreMultiplier, hit_max_time ? (mScore - mGauntletFinalScorePreBonus) : 0, (float)mLevel.mFurthestBallDistance / 100f, mScore - mLevelBeginScore, mScore, -1);
		mGauntletModeOver = true;
		mApp.PlaySong(138);
		int num = 0;
		if (hit_max_time)
		{
			mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_TIME_UP_CHALLENGE));
			mEndGauntletTimer = END_GAUNTLET_TIME;
			if (mLevel.mCurMultiplierTimeLeft > 0)
			{
				GauntletMultiplierEnded();
				mGauntletMultBarAlpha = Common._M(600);
				if (mRollerScore.GetTargetScore() > mApp.mUserProfile.mChallengeStats.mHighestScore)
				{
					mApp.mUserProfile.mChallengeStats.mHighestScore = mRollerScore.GetTargetScore();
				}
			}
			num = (int)((float)mScoreMultiplier / 100f * (float)mScore);
			if (num >= 25000)
			{
				mApp.SetAchievement("survival_25k");
			}
			int num2 = num + mRollerScore.GetTargetScore();
			if (num2 > mApp.mUserProfile.mChallengeStats.mHighestScore)
			{
				mApp.mUserProfile.mChallengeStats.mHighestScore = num2;
			}
			mApp.mUserProfile.mChallengeUnlockState[mLevel.mZone - 1, mLevel.mNum - 1] = 3;
		}
		int num3 = mApp.mUserProfile.mChallengeUnlockState[mLevel.mZone - 1, mLevel.mNum - 1];
		bool flag = num3 > 3;
		bool flag2 = false;
		int num4 = mApp.mUserProfile.ChallengeCupComplete(mLevel.mZone);
		if (mScore + num >= mLevel.mChallengeAcePoints)
		{
			flag2 = true;
			mApp.mUserProfile.mChallengeUnlockState[mLevel.mZone - 1, mLevel.mNum - 1] = 5;
			int num5 = 0;
			for (int i = 1; i <= 7; i++)
			{
				if (mApp.mUserProfile.ChallengeCupComplete(i) == 2)
				{
					num5++;
				}
			}
			if (num5 >= 4)
			{
				mApp.SetAchievement("ace_4_cups");
			}
		}
		else if (mScore + num >= mLevel.mChallengePoints && num3 != 5)
		{
			flag2 = true;
			mApp.mUserProfile.mChallengeUnlockState[mLevel.mZone - 1, mLevel.mNum - 1] = 4;
		}
		int num6 = mApp.mUserProfile.ChallengeCupComplete(mLevel.mZone);
		if (num4 != num6)
		{
			switch (num6)
			{
			case 1:
				mApp.mUserProfile.mDoChallengeCupComplete = true;
				break;
			case 2:
				mApp.mUserProfile.mDoChallengeAceCupComplete = true;
				break;
			default:
				if (num6 == 2)
				{
					mApp.mUserProfile.mDoAceCupXFade = true;
				}
				break;
			}
		}
		if (mScore >= mLevel.mChallengePoints && num3 <= 2)
		{
			mApp.mUserProfile.mDoChallengeTrophyZoom = true;
		}
		if (mScore >= mLevel.mChallengeAcePoints && num3 <= 4)
		{
			mApp.mUserProfile.mDoChallengeAceTrophyZoom = true;
		}
		if (!mLevel.mIronFrog && mScore + num >= mLevel.mChallengePoints && !flag && flag2)
		{
			UnlockAchievement(EAchievementType.CHALLENGE_ACCEPTED);
			int num7 = 0;
			for (int j = 0; j < 10; j++)
			{
				if (num7 >= 2)
				{
					break;
				}
				if (mApp.mUserProfile.mChallengeUnlockState[mLevel.mZone - 1, j] == 1)
				{
					mApp.mUserProfile.mChallengeUnlockState[mLevel.mZone - 1, j] = 2;
					if (num7 == 0)
					{
						mApp.mUserProfile.mUnlockSparklesIdx1 = j + (mLevel.mZone - 1) * 10;
					}
					else
					{
						mApp.mUserProfile.mUnlockSparklesIdx2 = j + (mLevel.mZone - 1) * 10;
					}
					num7++;
				}
			}
		}
		if (!hit_max_time)
		{
			SetupEndOfGauntletTransition(hit_max_time: false);
		}
		for (int k = 0; k < 6; k++)
		{
			for (int l = 0; l < 10; l++)
			{
				if (mApp.mUserProfile.mChallengeUnlockState[k, l] < 3)
				{
					return;
				}
			}
		}
		UnlockAchievement(EAchievementType.FE_RROG);
	}

	public void BossDied()
	{
		if (mLevel.mBoss == null || !(mLevel.mBoss.GetHP() <= 0f))
		{
			return;
		}
		mLevelStats.mTimePlayed = mStateCount - mIgnoreCount;
		mApp.ReportEndOfLevelMetrics(this, theLevelSuccess: true, theAcedLevel: false);
		mApp.mMusic.FadeOut();
		GetBetaStats().BeatLevel(mLevelStats.mTimePlayed, mLevel.mParTime, GetAceTimeBonus(), GetPerfectBonus(), (float)mLevel.mFurthestBallDistance / 100f, mScore - mLevelBeginScore, mScore, mLives);
		SetHallucinateTimer(0);
		mFrog.EmptyBullets();
		mLevel.mInvertMouseTimer = 0;
		mApp.mUserProfile.GetAdvModeVars().mPerfectZone = true;
		mApp.mUserProfile.GetAdvModeVars().mNumDeathsCurLevel = 0;
		mApp.mUserProfile.GetAdvModeVars().mNumZumasCurLevel = 0;
		int num = mLevel.mZone * 10;
		if (num > mApp.mUserProfile.GetAdvModeVars().mHighestLevelBeat)
		{
			mApp.mUserProfile.GetAdvModeVars().mHighestLevelBeat = num;
		}
		if (mLevel.mBoss.mDeathText.size() > 0)
		{
			mLevel.mBoss.MoveToDeathPosition(Common._M(400), Common._M1(100));
			mFrog.MoveToBossDeathPosition(FROG_DEATH_X, FROG_DEATH_Y);
		}
		if (mLevel.mBoss.mDeathText.size() == 0)
		{
			if (mLevel.mZone == 6)
			{
				mStateCount = 0;
				if (mLevel.mEndSequence == 2)
				{
					mGameState = GameState.GameState_Boss6FakeCredits;
					if (mFakeCredits != null)
					{
						mFakeCredits.Dispose();
						mFakeCredits = null;
					}
					mFakeCredits = new FakeCredits();
					mFakeCredits.Init(mFrog);
					SetMenuBtnEnabled(enabled: false);
				}
				else if (mLevel.mEndSequence == 3)
				{
					mGameState = GameState.GameState_Boss6DarkFrog;
					mDarkFrogSequence = new DarkFrogSequence();
					mDarkFrogSequence.Init();
					SetMenuBtnEnabled(enabled: false);
					mDrawBossUI = false;
					mDarkFrogTimer = Common._M(600);
					MakeBoss6VolcanoMeltComp();
					mDarkFrogBulletX = (float)(-(Common._DS(mBoss6VolcanoMelt.mWidth) / 2 - Common._S(mLevel.mBoss.GetX()))) + mDarkFrogBulletX + (float)Common._DS(Common._M(315));
					mDarkFrogBulletY = (float)(-(Common._DS(mBoss6VolcanoMelt.mHeight) / 2 - Common._S(mLevel.mBoss.GetY()))) + mDarkFrogBulletY + (float)Common._DS(Common._M(305));
					mDarkFrogBulletVX = ((float)Common._S(mFrog.GetCenterX()) - mDarkFrogBulletX) / (float)Common._M(50);
					mDarkFrogBulletVY = ((float)Common._S(mFrog.GetCenterY()) - mDarkFrogBulletY) / (float)Common._M(50);
					mEssenceScaleTimer = 0;
					mEssenceXScale = (mEssenceYScale = 0.74f);
				}
				else
				{
					mGameState = GameState.GameState_Boss6Transition;
				}
			}
			else
			{
				mGameState = GameState.GameState_LevelUp;
				if (mApp.GetLevelMgr().mScoreTips.size() > 0)
				{
					mScoreTipIdx = mApp.GetLevelMgr().GetScoreTipIdx((mLevel.mZone - 1) * 10 + mLevel.mNum);
				}
			}
		}
		else
		{
			if (mApp.mUserProfile.GetAdvModeVars().mNumTimesZoneBeat[5] == 0 && JeffLib.Common.StrFindNoCase(mLevel.mId, "debug") == -1)
			{
				mApp.mUserProfile.GetAdvModeVars().mHighestZoneBeat = mLevel.mBoss.mNum;
			}
			if (JeffLib.Common.StrFindNoCase(mLevel.mId, "debug") == -1)
			{
				mApp.mUserProfile.GetAdvModeVars().mNumTimesZoneBeat[mLevel.mZone - 1]++;
			}
			if (mLevel.mEndSequence == 5)
			{
				AdvModeTempleStats advModeTempleStats = GetAdvModeTempleStats();
				if (advModeTempleStats.mCurrentTime < advModeTempleStats.mBestTime)
				{
					advModeTempleStats.mBestTime = advModeTempleStats.mCurrentTime;
				}
				mTimeToBeatAdvMode = advModeTempleStats.mCurrentTime;
				advModeTempleStats.mCurrentTime = 0;
				if (mApp.mUserProfile.mChallengeUnlockState[6, 0] < 2)
				{
					mApp.mUserProfile.mChallengeUnlockState[6, 0] = 2;
				}
				int num2 = mScore + mLives * mApp.GetLevelMgr().mBeatGamePointsForLife;
				if (num2 > advModeTempleStats.mBestScore)
				{
					advModeTempleStats.mBestScore = num2;
				}
				PIEffect pIEffectByID = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_TORCHFLAME);
				pIEffectByID.ResetAnim();
				pIEffectByID.mEmitAfterTimeline = true;
				mBeatGameLives = mLives - 1;
				if (mBeatGameLives < 0)
				{
					mBeatGameLives = 0;
				}
				mBeatGameNormalScore = mScore;
				mBeatGameTotalScoreTally = 0;
				mForceRestartInAdvMode = true;
				mLevelNum = 0;
				mApp.mUserProfile.GetAdvModeVars().mHighestZoneBeat = 6;
				if (!mApp.IsHardMode())
				{
					mApp.SetAchievement("beat_adventure");
					mApp.mUserProfile.mFirstTimeReplayingNormalMode = true;
				}
				else
				{
					mApp.SetAchievement("beat_heroic");
					mApp.mUserProfile.mFirstTimeReplayingHardMode = true;
				}
				mLives = 3;
				mScore = 0;
				mPointsLeftForExtraLife = mApp.GetLevelMgr().mPointsForLife;
				mRollerScore.ForceScore(0);
				mApp.mUserProfile.GetAdvModeVars().mCurrentAdvScore = 0;
				mApp.mUserProfile.GetAdvModeVars().mNumTimesZoneBeat[5]++;
			}
			SetMenuBtnEnabled(enabled: false);
			mGameState = GameState.GameState_BossDead;
			mLevel.mBoss.mDoDeathExplosions = true;
			mStateCount = 0;
			mFrog.LevelReset();
		}
		if (!mAdventureMode)
		{
			return;
		}
		switch (mLevel.mZone)
		{
		case 1:
			UnlockAchievement(EAchievementType.JAW_BREAKER);
			break;
		case 2:
			UnlockAchievement(EAchievementType.TIKI_TRAMPLER);
			break;
		case 3:
			UnlockAchievement(EAchievementType.BONE_PICKER);
			break;
		case 4:
			UnlockAchievement(EAchievementType.PESTILENCE_PACIFIER);
			break;
		case 5:
			UnlockAchievement(EAchievementType.CEPHALOPOD_SMASHE);
			break;
		case 6:
			if (mLevel.mEndSequence > 3)
			{
				UnlockAchievement(EAchievementType.TIME_FOR_TADPOLES);
			}
			break;
		}
	}

	public void CannonDisabled()
	{
		mShowGuide = false;
	}

	public void DoClearCurveBonus(int curve_num)
	{
		if (GauntletMode())
		{
			_ = mLevel.mGauntletCurTime;
		}
		else if (mGameState != GameState.GameState_Playing)
		{
			_ = mEndLevelStats.mTimePlayed;
		}
		if (mAdventureMode)
		{
			GetAdvModeTempleStats().mNumClearCurves++;
		}
		int num = (GauntletMode() ? Common._M(1000) : (mApp.GetLevelMgr().mClearCurvePoints * mLevel.mZone));
		IncScore(num, from_balls: false, counts_towards_zuma: false);
		mCurveClearBonus += num;
		BonusTextElement bonusTextElement = AddText(TextManager.getInstance().getString(99), mFrog.GetCenterX(), mFrog.GetCenterY());
		if (bonusTextElement != null)
		{
			bonusTextElement = AddText("+" + num + "!", 0, 0, Common._M(1.55f), bonusTextElement.mHandle, null);
			if (!GauntletMode() && !IronFrogMode())
			{
				LivesChanged(1);
			}
			GetBetaStats().ClearedCurve(num);
		}
	}

	public void DrawTunnelMasks(Graphics g, int pri)
	{
		Graphics3D graphics3D = g.Get3D();
		if (graphics3D == null || pri >= 5 || mTunnels[pri].size() == 0)
		{
			return;
		}
		for (int i = 0; i < mTunnels[pri].size(); i++)
		{
			Tunnel tunnel = mTunnels[pri][i];
			if (tunnel.mLayerId.Length == 0)
			{
				g.DrawImage(tunnel.mImage, Common._S(tunnel.mX + GameApp.gScreenShakeX), Common._S(tunnel.mY + GameApp.gScreenShakeY));
			}
			else
			{
				g.DrawImage(tunnel.mImage, Common._DS(tunnel.mX + GameApp.gScreenShakeX - mApp.mOffset160X), Common._DS(tunnel.mY + GameApp.gScreenShakeY));
			}
		}
	}

	public bool DoingMainDarkFrogSequence()
	{
		if (mDarkFrogSequence == null || mGameState != GameState.GameState_Boss6DarkFrog || mDarkFrogSequence.FadingIn() || mDarkFrogSequence.mInitialDelay < mDarkFrogSequence.mInitialDelayTarget || mDarkFrogSequence.Done() || mDarkFrogSequence.FadingOut())
		{
			return false;
		}
		return true;
	}

	public bool HasDarkFrogSequence()
	{
		return mDarkFrogSequence != null;
	}

	public void BallDeleted(Ball b)
	{
		for (int i = 0; i < mMultiplierBallEffects.size(); i++)
		{
			MultiplierBallEffect multiplierBallEffect = mMultiplierBallEffects[i];
			if (multiplierBallEffect.GetBall() == b)
			{
				multiplierBallEffect.BallDestroyed(b);
				break;
			}
		}
	}

	public void ForceFlipFrog()
	{
		mFrog.SetDestAngle(-3.14159f);
	}

	public int GetDarkFrogLevelFadeInAlpha()
	{
		if (mDarkFrogSequence != null && mDarkFrogSequence.FadingToLevel())
		{
			return (int)mDarkFrogSequence.GetBGAlpha();
		}
		return -1;
	}

	public bool LazerHitTreasure(SexyVector3 p1, SexyVector3 v1, ref float t)
	{
		float num = Common._DS(Common._M(40));
		float num2 = p1.x - ((float)mCurTreasure.x + Common._SS((float)mFruitImg.GetCelWidth() / Common._M(2f)));
		float num3 = p1.y - ((float)mCurTreasure.y + Common._SS((float)mFruitImg.GetCelHeight() / Common._M(2f)) + mTreasureYBob);
		float num4 = v1.x * v1.x + v1.y * v1.y;
		float num5 = 2f * (v1.x * num2 + v1.y * num3);
		float num6 = num2 * num2 + num3 * num3 - num * num;
		float num7 = num5 * num5 - 4f * num4 * num6;
		if (num7 < 0f)
		{
			return false;
		}
		num7 = (float)Math.Sqrt(num7);
		float num8 = (0f - num5 - num7) / (2f * num4);
		if (num8 > 0f && num8 < t)
		{
			t = num8;
			return true;
		}
		return false;
	}

	public bool LazerHitTorch(SexyVector3 p1, SexyVector3 v1, ref float t, float ballRadius)
	{
		if (mLevel == null)
		{
			return false;
		}
		bool result = false;
		for (int i = 0; i < mLevel.mTorches.size(); i++)
		{
			if (mLevel.mTorches[i].mWasHit)
			{
				continue;
			}
			float num = ((mLevel.mTorches[i].mWidth > mLevel.mTorches[i].mHeight) ? mLevel.mTorches[i].mWidth : mLevel.mTorches[i].mHeight);
			num /= 2f;
			num += ballRadius;
			Common._SS((float)mLevel.mTorches[i].mWidth / Common._M(2f));
			Common._SS((float)mLevel.mTorches[i].mHeight / Common._M(2f));
			float num2 = p1.x - ((float)mLevel.mTorches[i].mX + Common._SS((float)mLevel.mTorches[i].mWidth / Common._M(2f)));
			float num3 = p1.y - ((float)mLevel.mTorches[i].mY + Common._SS((float)mLevel.mTorches[i].mHeight / Common._M(2f)));
			float num4 = v1.x * v1.x + v1.y * v1.y;
			float num5 = 2f * (v1.x * num2 + v1.y * num3);
			float num6 = num2 * num2 + num3 * num3 - num * num;
			float num7 = num5 * num5 - 4f * num4 * num6;
			if (!(num7 < 0f))
			{
				num7 = (float)Math.Sqrt(num7);
				float num8 = (0f - num5 - num7) / (2f * num4);
				if (num8 > 0f && num8 < t)
				{
					t = num8;
					result = true;
				}
			}
		}
		return result;
	}

	public int GetCheckpointScore()
	{
		if (mLevel.mNum > 5)
		{
			return mApp.mUserProfile.GetAdvModeVars().mCheckpointScores[mLevel.mZone - 1].mMidpoint;
		}
		return mApp.mUserProfile.GetAdvModeVars().mCheckpointScores[mLevel.mZone - 1].mZoneStart;
	}

	public Level GetCheckpointLevel()
	{
		int levelIndex = mApp.GetLevelMgr().GetLevelIndex(mLevel.mId);
		levelIndex = ((mLevel.mNum > 5) ? (levelIndex - (mLevel.mNum - 6)) : (levelIndex - (mLevel.mNum - 1)));
		return mApp.GetLevelMgr().GetLevelByIndex(levelIndex);
	}

	public int GetLevel()
	{
		return mLevelNum;
	}

	public int GetNumClearsInARow()
	{
		return mNumClearsInARow;
	}

	public int GetCurInARowBonus()
	{
		return mCurInARowBonus;
	}

	public int GetCurComboScore()
	{
		return mCurComboScore;
	}

	public int GetNumCleared()
	{
		return mNumCleared;
	}

	public int GetCurComboCount()
	{
		return mCurComboCount;
	}

	public int GetStateCount()
	{
		return mStateCount;
	}

	public int GetTickCount()
	{
		return GetStateCount() * 10;
	}

	public int GetLevelScore()
	{
		return mScore - mLevelBeginScore;
	}

	public int GetLevelBeginScore()
	{
		return mLevelBeginScore;
	}

	public int GetHallucinateTimer()
	{
		return mHallucinateTimer;
	}

	public int GetTreasureGlowAlpha()
	{
		return mTreasureGlowAlpha;
	}

	public int GetCurRollerScore()
	{
		return mRollerScore.GetCurrentScore();
	}

	public int GetNumLives()
	{
		return mLives;
	}

	public bool IsEndless()
	{
		return mIsEndless;
	}

	public bool IsGameOver()
	{
		if (mGameState != GameState.GameState_Losing)
		{
			return mIsWinning;
		}
		return true;
	}

	public bool IsPaused()
	{
		return mPauseCount != 0;
	}

	public bool HasBackgroundArt()
	{
		return mBackgroundImage != null;
	}

	public bool HasGuideBall()
	{
		return mGuideBall != null;
	}

	public bool DestroyAll()
	{
		return mDestroyAll;
	}

	public bool HasAchievedZuma()
	{
		if (mScore >= mScoreTarget)
		{
			return !GauntletMode();
		}
		return false;
	}

	public bool GauntletMode()
	{
		return mGauntletMode;
	}

	public bool IronFrogMode()
	{
		if (mLevel.mIronFrog && !GauntletMode() && !mAdventureMode)
		{
			return true;
		}
		return false;
	}

	public bool DoingIntros()
	{
		return mShowMapScreen;
	}

	public bool DoingLevelTransition()
	{
		return mLevelTransition != null;
	}

	public bool CanDrawFrog()
	{
		if ((!DoingLevelTransition() || mDoingTransition || (IronFrogMode() && mLevelTransition.GetState() == 2)) && mGameState != GameState.GameState_Boss6FakeCredits)
		{
			if (mGameState == GameState.GameState_Boss6DarkFrog)
			{
				if (mDarkFrogSequence != null)
				{
					if (!mDarkFrogSequence.FadingToLevel())
					{
						return mDarkFrogSequence.mInitialDelay < mDarkFrogSequence.mInitialDelayTarget;
					}
					return true;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public bool HasTunnels(int pri)
	{
		return mTunnels[pri].Count() > 0;
	}

	public bool DisplayingTip()
	{
		return mZumaTips.Count() > 0;
	}

	public bool WasShowingCheckpoint()
	{
		return mWasShowingCheckpoint;
	}

	public bool IsLoading()
	{
		return mIsLoading;
	}

	public bool IsAboutToDoCheckpointEffect()
	{
		if (mFrog.HasSmokeParticles() && !mHasSeenCheckpointIntro && ShouldShowCheckpointPostcard() && !GauntletMode() && !IronFrogMode() && mLevel.mNum > 1 && mCheckpointEffect == null)
		{
			return true;
		}
		if (mCheckpointEffect != null)
		{
			return true;
		}
		return false;
	}

	public bool LevelIsSkeletonBoss()
	{
		if (mLevel != null && mLevel.mBoss != null && mLevel.mZone == 3 && (object)mLevel.mBoss.GetType() == typeof(BossSkeleton))
		{
			return true;
		}
		return false;
	}

	public bool IsHardAdventureMode()
	{
		return mApp.IsHardMode();
	}

	public GameStats GetLevelStats()
	{
		return mLevelStats;
	}

	public GameStats GetGameStats()
	{
		return mGameStats;
	}

	public Gun GetGun()
	{
		return mFrog;
	}

	public bool DoingBossIntro()
	{
		return mGameState == GameState.GameState_BossIntro;
	}

	public Image GetFruitImage()
	{
		return mFruitImg;
	}

	public Image GetFruitGlow()
	{
		return mFruitGlow;
	}

	public bool CanDeleteEffectResources()
	{
		return mCanDeleteEffectResources;
	}

	public GameState GetGameState()
	{
		return mGameState;
	}

	public AdvModeTempleStats GetAdvModeTempleStats()
	{
		if (!mAdventureMode)
		{
			return null;
		}
		if (!IsHardAdventureMode())
		{
			return mApp.mUserProfile.mAdventureStats;
		}
		return mApp.mUserProfile.mHeroicStats;
	}

	public void SetMenuBtnEnabled(bool enabled)
	{
		mMenuButton.mDisabled = !enabled;
		mMenuButton.mVisible = enabled;
		if (mSwapBallButton != null)
		{
			mSwapBallButton.mDisabled = !enabled;
			mSwapBallButton.mVisible = enabled;
		}
	}

	public void AddFiredBullet(Bullet b)
	{
		mBulletList.Add(b);
	}

	public bool HasFiredBullets()
	{
		return mBulletList.Count() > 0;
	}

	public void SetNumClearsInARow(int val)
	{
		mNumClearsInARow = val;
		if (mNumClearsInARow > 1)
		{
			mLevel.MadeConsecutiveClear(mNumClearsInARow);
		}
	}

	public void SetCurInARowBonus(int val)
	{
		mCurInARowBonus = val;
	}

	public void SetCurComboScore(int val)
	{
		mCurComboScore = val;
	}

	public void IncNumClearsInARow(int val)
	{
		mNumClearsInARow += val;
		if (mNumClearsInARow >= 20)
		{
			mApp.SetAchievement("chain_20x");
		}
		if (mNumClearsInARow >= 15)
		{
			UnlockAchievement(EAchievementType.C_C_C_C_CHAIN_BONUS);
		}
		if (mNumClearsInARow > 1)
		{
			mLevel.MadeConsecutiveClear(mNumClearsInARow);
		}
	}

	public void IncCurInARowBonus(int val)
	{
		mCurInARowBonus += val;
	}

	public void IncCurComboScore(int val)
	{
		mCurComboScore += val;
	}

	public void SetNumCleared(int val)
	{
		mNumCleared = val;
	}

	public void IncNumCleared(int val)
	{
		mNumCleared += val;
		mLevel.IncNumBallsExploded(val);
	}

	public void SetCurComboCount(int val)
	{
		mCurComboCount = val;
	}

	public void AddPowerEffect(PowerEffect p)
	{
		mPowerEffects.Add(p);
	}

	public void SetRollingInDangerZone()
	{
		mRollingInDangerZone = true;
	}

	public void SetHallucinateTimer(int t)
	{
		if (t == 0)
		{
			mHallucinateTimer = 0;
		}
		else if (mHallucinateTimer <= 0)
		{
			Font fontByID = Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_STROKE);
			mLevel.mFrog.mTempText = new BonusText(TextManager.getInstance().getString(691), fontByID, Common._S(mLevel.mFrog.GetCenterX() - Common._M(30)), Common._S(mLevel.mFrog.GetCenterY() - Common._M1(70)), Common._M2(0), Common._M3(0));
			mLevel.mFrog.mTempText.SetAlphaDecRate(0f);
			mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_MINDWARP1));
			mHallucinateTimer = t;
		}
	}

	public void AddProxBombExplosion(float x, float y)
	{
		mApp.mProxBombManager.AddBomb(x, y);
	}

	public void ToggleNotification(string theNotification, int inSoundID)
	{
		m_NotificationQuene.Add(new KeyValuePair<string, int>(theNotification, inSoundID));
	}

	public void ToggleNotification(string theNotification)
	{
		ToggleNotification(theNotification, -1);
	}

	public void DrawFatFingerGuide(Graphics g)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_GUIDE);
		imageByID.GetWidth();
		Common._S(2);
		if ((mFrog != null && (mFrog.LaserMode() || mFrog.mLightningEffect != null || mFrog.IsCannon())) || mInvalidateTouchUp || ShouldBlockInput() || mFrog.IsStunned() || mGameState != GameState.GameState_Playing || mFrog.LightningMode() || (mZumaTips.Count() > 0 && mZumaTips[0].mClickDismiss && mZumaTips[0].mId != ZumaProfile.FIRST_SHOT_HINT) || mPauseCount != 0 || (mLevel.mBoss != null && !mLevel.mBoss.AllowFrogToFire()) || (!mFatFingerGuideEnabled && mFatFingerGuideAlpha <= 0))
		{
			return;
		}
		if (mFatFingerGuideEnabled && mFatFingerGuideAlpha < 255)
		{
			mFatFingerGuideAlpha += 15;
			if (mFatFingerGuideAlpha >= 255)
			{
				mFatFingerGuideAlpha = 255;
			}
		}
		else if (!mFatFingerGuideEnabled)
		{
			mFatFingerGuideAlpha -= 25;
			if (mFatFingerGuideAlpha <= 0)
			{
				mFatFingerGuideAlpha = 0;
			}
		}
		if (mFrog.GetBullet() != null)
		{
			int colorType = mFrog.GetBullet().GetColorType();
			int theColor = Common.gBallColors[colorType];
			if (mApp.mColorblind && colorType == 3)
			{
				theColor = 8421504;
			}
			else if (mApp.mColorblind && colorType == 4)
			{
				theColor = 1973790;
			}
			g.SetColorizeImages(colorizeImages: true);
			g.SetDrawMode(1);
			g.SetColor(new SexyFramework.Graphics.Color(theColor, mFatFingerGuideAlpha));
			g.DrawImageRotatedF(mAimGuide, Common._S(mFrog.mCurX), Common._S(mFrog.mCurY) - (float)(mAimGuide.mHeight / 2), (double)mFrog.GetAngle() - Math.PI / 2.0, 0f, mAimGuide.mHeight / 2);
			g.SetColorizeImages(colorizeImages: false);
			g.SetDrawMode(0);
		}
	}

	public bool IsTouchOnFrogGun(int x, int y)
	{
		bool result = false;
		float num = (float)mFrog.mWidth / 2f;
		num *= num;
		float num2 = Common._S(mFrog.mCurX) + (float)mApp.mBoardOffsetX;
		float num3 = Common._S(mFrog.mCurY);
		float num4 = (float)x - num2;
		num4 *= num4;
		float num5 = (float)y - num3;
		num5 *= num5;
		if (num4 + num5 < num)
		{
			result = true;
		}
		return result;
	}

	public bool CycleBallAt(int x, int y)
	{
		bool result = false;
		if (mLevel != null)
		{
			Ball ballAtXY = mLevel.GetBallAtXY(Common._SS(x), Common._SS(y));
			if (ballAtXY != null)
			{
				if (mBallPowerupCheat != -1)
				{
					if (mBallPowerupCheat == -2)
					{
						ballAtXY.GetCurve().RemoveBall(ballAtXY);
					}
					else
					{
						ballAtXY.SetPowerType((PowerType)mBallPowerupCheat, delay: false);
					}
					mBallPowerupCheat = -1;
				}
				else
				{
					int colorType = ballAtXY.GetColorType();
					if (++colorType >= 6)
					{
						colorType = 0;
					}
					ballAtXY.SetColorType(colorType);
				}
				result = true;
			}
		}
		return result;
	}

	public bool CycleFrogPowerupAt(int x, int y)
	{
		bool result = false;
		if (mFrog != null && IsTouchOnFrogGun(x, y))
		{
			int type = mFrog.GetType();
			if (++type >= 5)
			{
				type = 0;
			}
			int p = 0;
			mFrog.ResetFrogType();
			switch ((FrogType)type)
			{
			case FrogType.FrogType_Lazer:
				p = 9;
				break;
			case FrogType.FrogType_Cannon:
				p = 7;
				break;
			case FrogType.FrogType_Lightning:
				p = 8;
				break;
			}
			ActivatePower((PowerType)p);
			result = true;
		}
		return result;
	}

	public void SetBallPowerupCheat(char c)
	{
		switch (c)
		{
		case '*':
			if (GauntletMode())
			{
				mBallPowerupCheat = 13;
			}
			break;
		case '#':
			mBallPowerupCheat = 0;
			break;
		case '%':
			mBallPowerupCheat = 1;
			break;
		case '@':
			mBallPowerupCheat = 9;
			break;
		case '<':
			mBallPowerupCheat = 3;
			break;
		case '|':
			mBallPowerupCheat = 7;
			break;
		case '~':
			mBallPowerupCheat = 8;
			break;
		case 'D':
			mBallPowerupCheat = -2;
			break;
		default:
			mBallPowerupCheat = -1;
			break;
		}
	}

	public void DisableBallPowerupCheat()
	{
		mBallPowerupCheat = -1;
	}

	public void SwapFrogBalls()
	{
		if (mLevel.CanSwapBalls() && !mFrog.LaserMode())
		{
			Bullet bullet = mFrog.GetBullet();
			int num = -1;
			int num2 = -1;
			if (bullet != null)
			{
				num = bullet.GetColorType();
			}
			mFrog.SwapBullets();
			bullet = mFrog.GetBullet();
			if (bullet != null)
			{
				num2 = bullet.GetColorType();
			}
			if (num != num2)
			{
				mApp.mUserProfile.mBallsSwapped++;
			}
			SwapBallButtonImage(num);
		}
		if (mFrog.LaserMode())
		{
			mFrog.ClearLaserState();
			GetBetaStats().CanceledLaser();
			mShowGuide = false;
			if (mGuideBall != null)
			{
				mGuideBall.mHilightPulse = false;
				mGuideBall.DoLaserAnim(d: false);
			}
		}
	}

	public void SwapBallButtonImage(int theBallColor)
	{
	}

	public void LivesChanged(int theLivesDelta)
	{
		if (mLivesInfo != null)
		{
			mLivesInfo = null;
		}
		mLives += theLivesDelta;
		mLivesInfo = new LivesInfo(this, theLivesDelta);
	}

	public bool IsPointAlongSlider(int x, int y)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_FROG_NORMAL_BODY);
		bool result = false;
		int num = (int)Common._S(mFrog.mCurX);
		int num2 = (int)Common._S(mFrog.mCurY);
		if (mLevel.mMoveType == 1)
		{
			int num3 = num2 - imageByID.GetHeight() / 2;
			int num4 = num2 + imageByID.GetHeight() / 2;
			if (y >= num3 && y <= num4)
			{
				result = true;
			}
		}
		else if (mLevel.mMoveType == 2)
		{
			int num5 = num - imageByID.GetHeight() / 2;
			int num6 = num + imageByID.GetHeight() / 2;
			if (x >= num5 && x <= num6)
			{
				result = true;
			}
		}
		return result;
	}

	public void DrawHaloSwap(Graphics g)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_SWAP_HALO);
		if (mGameState == GameState.GameState_Playing && !mFrog.IsStunned() && (!GauntletMode() || !mGauntletModeOver) && !mFrog.IsCannon() && !mFrog.LaserMode() && !mFrog.LightningMode() && mFrog.IsFrogShowingBall() && (mDrawHaloSwap || mFinishHaloSwap))
		{
			int num = (int)Common._S(mFrog.mCenterX);
			int num2 = (int)Common._S(mFrog.mCenterY);
			if (mFrog != null && mFrog.GetNextBullet() != null)
			{
				float num3 = (float)mHaloSwapCurve.GetOutVal();
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(mHaloSwapColor.mRed, mHaloSwapColor.mGreen, mHaloSwapColor.mBlue, (int)(num3 * 255f));
				g.SetDrawMode(1);
				float num4 = num3 * (float)imageByID.GetHeight();
				float num5 = num3 * (float)imageByID.GetHeight();
				float num6 = (float)num - num4 / 2f;
				float num7 = (float)num2 - num5 / 2f;
				g.DrawImage(imageByID, (int)num6, (int)num7, (int)num4, (int)num5);
				g.SetColorizeImages(colorizeImages: false);
				g.SetDrawMode(0);
			}
		}
	}

	public void EnableHaloSwap(bool forceReset)
	{
		if (mFrog != null && mFrog.GetNextBullet() != null && mGameState == GameState.GameState_Playing && (!GauntletMode() || !mGauntletModeOver) && !mFrog.IsStunned())
		{
			mDrawHaloSwap = true;
			int colorType = mFrog.GetNextBullet().GetColorType();
			int theColor = Common.gBallColors[colorType];
			if (mApp.mColorblind && colorType == 3)
			{
				theColor = 8421504;
			}
			else if (mApp.mColorblind && colorType == 4)
			{
				theColor = 1973790;
			}
			mHaloSwapColor = new SexyFramework.Graphics.Color(theColor);
			bool flag = mHaloSwapCurve.IsDoingCurve();
			bool flag2 = mHaloSwapCurve.HasBeenTriggered();
			if (forceReset || (!flag && !flag2))
			{
				mHaloSwapCurve.SetCurve(Common._MP("b#0,1,0.01,1,# ##  *~      w~"));
			}
		}
	}

	public void DisableHaloSwap(bool finishAnim)
	{
		mFinishHaloSwap = finishAnim;
		mDrawHaloSwap = false;
		if (!finishAnim)
		{
			mHaloSwapCurve.SetConstant(0.0);
			mHaloSwapCurve.ClearTrigger();
		}
	}

	public void UpdateHaloSwap()
	{
		if (mFinishHaloSwap)
		{
			if (!mHaloSwapCurve.IncInVal())
			{
				mHaloSwapCurve.SetConstant(0.0);
				mHaloSwapCurve.ClearTrigger();
				mFinishHaloSwap = false;
			}
		}
		else if (mDrawHaloSwap)
		{
			mHaloSwapCurve.IncInVal();
		}
	}

	public string GetLevelDisplayName()
	{
		if (mLevel.mBoss != null || mLevel.IsFinalBossLevel())
		{
			return mLevel.mDisplayName;
		}
		if (mLevel.mZone - 1 > 0)
		{
			if (mLevel.mNum >= 10)
			{
				return $"{mLevel.mZone * mLevel.mNum} - {mLevel.mDisplayName}";
			}
			return $"{mLevel.mZone - 1}{mLevel.mNum} - {mLevel.mDisplayName}";
		}
		return $"{mLevel.mNum} - {mLevel.mDisplayName}";
	}

	public string GetMetricsLevelName()
	{
		if (mLevel.mBoss != null || mLevel.IsFinalBossLevel())
		{
			return mLevel.mDisplayName;
		}
		if (mLevel.mZone - 1 > 0)
		{
			if (mLevel.mNum >= 10)
			{
				return $"{mLevel.mZone * mLevel.mNum}";
			}
			return $"{mLevel.mZone - 1}{mLevel.mNum}";
		}
		return $"{mLevel.mNum}";
	}

	public void SetBoardOffset(Graphics g, bool enable)
	{
		if (enable)
		{
			g.mTransX = mApp.mBoardOffsetX;
		}
		else
		{
			g.mTransX = 0f;
		}
	}

	public void ProcessTrialYesNo(int theId)
	{
		switch (theId)
		{
		case 1000:
			GameApp.gApp.ToMarketPlace();
			GameApp.gApp.mBoard.Pause(pause: false, becauseOfDialog: true);
			mIsTryAndBuyDialogShowing = false;
			break;
		case 1001:
			GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.DoDeferredEndGame;
			GameApp.gApp.ToggleBambooTransition();
			GameApp.gApp.mMusic.StopAll();
			mIsTryAndBuyDialogShowing = false;
			break;
		}
	}
}
