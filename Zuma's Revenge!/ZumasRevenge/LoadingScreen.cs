using System;
using System.Collections.Generic;
using System.Linq;
using JeffLib;
using Microsoft.Xna.Framework.Graphics;
using SexyFramework;
using SexyFramework.Drivers;
using SexyFramework.Drivers.Graphics;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class LoadingScreen : Widget, ButtonListener
{
	internal static class RandomNumbers
	{
		private static Random r;

		internal static int NextNumber()
		{
			if (r == null)
			{
				Seed();
			}
			return r.Next();
		}

		internal static int NextNumber(int ceiling)
		{
			if (r == null)
			{
				Seed();
			}
			return r.Next(ceiling);
		}

		internal static void Seed()
		{
			r = new Random();
		}

		internal static void Seed(int seed)
		{
			r = new Random(seed);
		}
	}

	private enum State
	{
		State_LogoIntro,
		State_Lightning,
		State_Loading,
		State_Final
	}

	internal class LoadingTextContainer
	{
		private List<string> mLoadingText = new List<string>();

		private List<string> mBackStoryText = new List<string>();

		private static string _(string s)
		{
			return s;
		}

		public LoadingTextContainer()
		{
			int num = 582;
			int num2 = 29;
			for (int i = num; i < num + num2; i++)
			{
				mLoadingText.Add(TextManager.getInstance().getString(i));
			}
			num += num2;
			num2 = 3;
			for (int j = num; j < num + num2; j++)
			{
				mBackStoryText.Add(TextManager.getInstance().getString(j));
			}
		}

		public List<string> GetLoadingText()
		{
			return mLoadingText;
		}

		public List<string> GetBackStoryText()
		{
			return mBackStoryText;
		}
	}

	protected const float mCenterCloudImageScale = 2f;

	protected long updateTimes;

	protected long drawTimes;

	public static int MAX_VOLCANO_PROJECTILES = 4;

	private static float max_angle = 0.08726f;

	private static int LOADING_TEXT_TIME = 200;

	protected List<PartnerLogo> mPartnerLogos = new List<PartnerLogo>();

	protected float mLoadStarRotateAngle;

	protected float mLavaAlpha;

	protected bool mIncLavaAlpha;

	protected List<LogoLightning> mLogoLightning = new List<LogoLightning>();

	protected LoadingWave[] mWaves = new LoadingWave[5];

	protected LoadingWave[] mCalmWaves = new LoadingWave[4];

	protected LoadingCloud[] mClouds = new LoadingCloud[3];

	protected float mLoadingBarAlpha;

	protected float mCompleteLoadingBarAlpha;

	protected float mFlashAlpha;

	protected float mExtraProgress;

	protected float mFrogAngle;

	protected float mFrogAngleDivisor;

	protected float mFrogAngleDelta;

	protected float mZumaY;

	protected float mRevengeY;

	protected float mRevengeStretch;

	protected float mBlackFadeAlpha;

	protected bool mBlackFadeIn;

	protected bool mCanShowMenu;

	protected bool mFrogPitchForward;

	protected bool mHasShown;

	protected bool mLightningOn;

	protected bool mLoadingComplete;

	protected bool mFadeToMainMenu;

	protected bool mFirstRun;

	protected int mLightningTimer;

	protected int mLightningFrame;

	protected int mLogoHoldTime;

	protected int mState;

	protected int mFrogWave;

	protected int mStormTimer;

	protected int mClearTimer;

	protected int mLoadingCompleteDelay;

	protected float mFrogScale;

	protected float mFrogPct;

	protected int mDarkIslandAlpha;

	protected int mLoadingCompleteTime;

	protected int mCloudUpdateCount;

	protected CurvedVal mPantaloonFlopPct = new CurvedVal();

	protected CurvedVal mPantalookRipplePct = new CurvedVal();

	protected float mRippleCnt;

	protected int mLoadingOffset;

	protected int mLoadingTextIdx;

	protected int mLoadingTextTime;

	protected List<int> mSeenLoadingTextIndices = new List<int>();

	protected int mAndroidLightningWarmupFrames;

	protected int mAndroidLightningFlashHoldFrames;

	protected int mAndroidLightningSequenceStep;

	protected int mAndroidLightningSequenceTimer;

	protected int mOffsetParticle = 85;

	protected bool mUserProfileLoaded;

	protected bool mLoading;

	protected PIEffect mLeftTorch;

	protected PIEffect mRightTorch;

	protected PIEffect mVolcanoSmoke;

	protected VolcanoProjectile[] mVolcanoProjectiles = new VolcanoProjectile[MAX_VOLCANO_PROJECTILES];

	public PIEffectBatch mEffectBatch;

	protected Transform mGlobalTransform = new Transform();

	protected int mLoadingXOffset;

	protected int mLoadingYOffset;

	protected Rect mLoadingTextFrame = default(Rect);

	protected Image IMAGE_LS_LOGO1 = Res.GetImageByID(ResID.IMAGE_LS_LOGO1);

	protected Image IMAGE_LS_LIGHT1_ID;

	protected Image IMAGE_LS_REDLOADINGBAR = Res.GetImageByID(ResID.IMAGE_LS_REDLOADINGBAR);

	protected Image IMAGE_LS_CLICKTXT = Res.GetImageByID(ResID.IMAGE_LS_CLICKTXT);

	protected Image IMAGE_LS_BACKING = Res.GetImageByID(ResID.IMAGE_LS_BACKING);

	protected Image IMAGE_LS_STARFISH = Res.GetImageByID(ResID.IMAGE_LS_STARFISH);

	protected Image IMAGE_LS_R_TIKI02 = Res.GetImageByID(ResID.IMAGE_LS_R_TIKI02);

	protected Image IMAGE_LS_R_TIKI01 = Res.GetImageByID(ResID.IMAGE_LS_R_TIKI01);

	protected Image IMAGE_LS_L_TIKI02 = Res.GetImageByID(ResID.IMAGE_LS_L_TIKI02);

	protected Image IMAGE_LS_L_TIKI01 = Res.GetImageByID(ResID.IMAGE_LS_L_TIKI01);

	protected Image IMAGE_LS_GREENLOADEDBAR = Res.GetImageByID(ResID.IMAGE_LS_GREENLOADEDBAR);

	protected Image IMAGE_LS_BAR = Res.GetImageByID(ResID.IMAGE_LS_BAR);

	protected Image IMAGE_LS_HAPPYSKY_BKGRND = Res.GetImageByID(ResID.IMAGE_LS_HAPPYSKY_BKGRND);

	protected Image IMAGE_LS_HAPPYSKY_LAVA = Res.GetImageByID(ResID.IMAGE_LS_HAPPYSKY_LAVA);

	protected Image IMAGE_LS_CENTER_CLOUD = Res.GetImageByID(ResID.IMAGE_LS_CENTER_CLOUD);

	protected Image IMAGE_LS_RAFTANIM_PANTS01 = Res.GetImageByID(ResID.IMAGE_LS_RAFTANIM_PANTS01);

	protected Point[] pts = new Point[5];

	protected Image[] mAndroidLogoFrames = new Image[5];

	protected Image[] mAndroidLightningImages = new Image[2];

	protected Image mAndroidBlackOverlay;

	protected Image mAndroidWhiteOverlay;

	protected int mCenterOffX;

	protected int mCenterOffY;

	protected int mFrogXOffset = Common._S(Common._M(100));

	protected int mFrogYOffset = Common._S(Common._M(120));

	protected int mWaveImgResScale = 2;

	public bool mLockBGM;

	protected int[] mFrogYOffs = new int[5]
	{
		Common._DS(Common._M(100)),
		Common._DS(Common._M1(20)),
		Common._DS(Common._M2(-40)),
		Common._DS(Common._M3(0)),
		Common._DS(Common._M4(60))
	};

	private LoadingTextContainer mLoadingTextContainer = new LoadingTextContainer();

	public bool mWaitingForConfirmation;

	private ButtonWidget mHelpButton;

	private ButtonWidget mStartButton;

	protected void DrawLightning(Graphics g, int x, int cloud_num)
	{
		if (OperatingSystem.IsAndroid() && mState < 2)
		{
			return;
		}
		LoadingCloud loadingCloud = mClouds[cloud_num];
		if (loadingCloud.mLightning != null)
		{
			int num = 2;
			int num2 = 2;
			Image imageByID = Res.GetImageByID((ResID)(1132 + cloud_num * 3));
			float num3 = loadingCloud.mLightningScale * (float)(loadingCloud.mLightning.mWidth * num2);
			float num4 = loadingCloud.mLightningScale * (float)(loadingCloud.mLightning.mHeight * num2);
			g.PushState();
			int alpha = (int)(255f * ((float)loadingCloud.mLightningTimer / (float)loadingCloud.mTimerTarget));
			g.SetColor(255, 255, 255, alpha);
			g.SetColorizeImages(colorizeImages: true);
			Image imageByID2 = Res.GetImageByID((ResID)(1133 + cloud_num * 3));
			int num5 = 10;
			g.DrawImage(imageByID2, x, Common._DS((int)loadingCloud.mY + Common._M(0)), imageByID2.GetWidth() * num5, imageByID2.GetHeight() * num5);
			g.DrawImage(loadingCloud.mLightning, x + (imageByID.mWidth * num - (int)num3) / 2, (int)Common._DS(loadingCloud.mY) + imageByID.mHeight * num / 3, (int)num3, (int)num4);
			g.PopState();
		}
	}

	public override void DrawOverlay(Graphics g)
	{
		if (mBlackFadeAlpha > 0f && !(OperatingSystem.IsAndroid() && mState == 0 && !mFadeToMainMenu))
		{
			g.PushState();
			if (OperatingSystem.IsAndroid() && mState < 2 && mAndroidBlackOverlay != null)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)mBlackFadeAlpha);
				g.DrawImage(mAndroidBlackOverlay, 0, 0, GlobalMembers.gSexyApp.mScreenBounds.mWidth, GlobalMembers.gSexyApp.mScreenBounds.mHeight);
			}
			else
			{
				g.SetColor(0, 0, 0, (int)mBlackFadeAlpha);
				g.FillRect(GlobalMembers.gSexyApp.mScreenBounds);
			}
			g.PopState();
		}
		g.PushState();
		if (mFadeToMainMenu && !mBlackFadeIn)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)mBlackFadeAlpha);
		}
		g.PopState();
	}

	public bool CanLoad()
	{
		if (mState == 2)
		{
			return !mWaitingForConfirmation;
		}
		return false;
	}

	public bool Done()
	{
		if (mFadeToMainMenu)
		{
			return mBlackFadeAlpha <= 0f;
		}
		return false;
	}

	public bool CanShowMenu()
	{
		if (mFadeToMainMenu && mCanShowMenu && (!mBlackFadeIn || GameApp.gApp.mMinimized) && mUserProfileLoaded)
		{
			mCanShowMenu = false;
			return true;
		}
		return false;
	}

	public void LoadingComplete()
	{
		mPantalookRipplePct.SetCurve(Common._MP("b;0,1,0.003333,1,~###         ~####"));
		mPantaloonFlopPct.SetCurve(Common._MP("b;0,1,0.002857,1,####    b####     ?~d,o"));
		mLoadingComplete = true;
		for (int i = 0; i < MAX_VOLCANO_PROJECTILES; i++)
		{
			mVolcanoProjectiles[i].mProjectile = GameApp.gApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_PARTICLES_VOLCANO_PROJECTILE").Duplicate();
			mVolcanoProjectiles[i].mProjectile.mEmitAfterTimeline = true;
			mVolcanoProjectiles[i].mProjectile.mOptimizeValue = 2;
			mVolcanoProjectiles[i].mProjectile.mInUse = false;
			mVolcanoProjectiles[i].mProjectile.mDrawTransform.LoadIdentity();
			mVolcanoProjectiles[i].mProjectile.mDrawTransform.Scale(Common._DS(1.4f), Common._DS(1.4f));
			mVolcanoProjectiles[i].mProjectile.mDrawTransform.Translate(Common._DS(Common._M(790)) + mOffsetParticle, Common._DS(Common._M1(150)));
			Common.SetFXNumScale(mVolcanoProjectiles[i].mProjectile, 3f);
			mEffectBatch.AddEffect(mVolcanoProjectiles[i].mProjectile);
		}
	}

	public override void MouseDown(int x, int y, int theClickCount)
	{
		if (!(mCompleteLoadingBarAlpha >= 255f) || mFadeToMainMenu)
		{
			return;
		}
		mUserProfileLoaded = true;
		if (GameApp.gApp.mUserProfile == null && mLoadingComplete && !mFadeToMainMenu)
		{
			GameApp.gApp.mUserProfile = (ZumaProfile)GameApp.gApp.mProfileMgr.GetProfile(GameApp.gApp.m_DefaultProfileName);
			if (GameApp.gApp.mUserProfile == null)
			{
				GameApp.gApp.mUserProfile = (ZumaProfile)GameApp.gApp.mProfileMgr.GetProfile(0);
			}
			if (GameApp.gApp.mUserProfile != null)
			{
				mBlackFadeAlpha = 0.0001f;
			}
		}
		if (GameApp.USE_XBOX_SERVICE && !GameApp.USE_TRIAL_VERSION && GameApp.gApp.mUserProfile != null)
		{
			GameApp.gApp.mUserProfile.m_AchievementMgr.SyncAchievementsXLive();
		}
		mFadeToMainMenu = true;
	}

	public override void GotFocus()
	{
		base.GotFocus();
	}

	public override void Resize(int x, int y, int width, int height)
	{
		base.Resize(x, y, width, height);
		pts[0] = new Point((mWidth - IMAGE_LS_LOGO1.mWidth) / 2, (mHeight - IMAGE_LS_LOGO1.mHeight) / 2);
		pts[1] = new Point((mWidth - IMAGE_LS_LOGO1.mWidth) / 2, (mHeight - IMAGE_LS_LOGO1.mHeight) / 2 - 77);
		pts[2] = new Point((mWidth - IMAGE_LS_LOGO1.mWidth) / 2, (mHeight - IMAGE_LS_LOGO1.mHeight) / 2 - 77);
		pts[3] = new Point((mWidth - IMAGE_LS_LOGO1.mWidth) / 2, (mHeight - IMAGE_LS_LOGO1.mHeight) / 2 - 77);
		pts[4] = new Point((mWidth - IMAGE_LS_LOGO1.mWidth) / 2 - 135, (mHeight - IMAGE_LS_LOGO1.mHeight) / 2 - 77);
	}

	public override void GamepadButtonDown(GamepadButton theButton, int thePlayer, uint theFlags)
	{
		_ = theFlags & 1;
	}

	private float GetMainMenuAlpha()
	{
		return mBlackFadeAlpha;
	}

	public LoadingScreen()
	{
		mDarkIslandAlpha = 255;
		mLogoHoldTime = 200;
		Init();
		BuildAndroidLogoFrames();
		mWaitingForConfirmation = false;
		if (GameApp.gApp.mFromReInit)
		{
			mState = 2;
			mFlashAlpha = 0f;
			SoundAttribs inAttribs = new SoundAttribs
			{
				fadein = 0.01f,
				fadeout = 0.005f
			};
			GameApp.gApp.mSoundPlayer.Loop(Res.GetSoundByID(ResID.SOUND_LS_STORM_LOOP), inAttribs);
		}
		mClip = false;
		mFirstRun = false;
		mLoadingCompleteTime = 0;
		if (mFirstRun)
		{
			mLoadingTextIdx = 0;
		}
		else
		{
			mLoadingTextIdx = SexyFramework.Common.Rand() % mLoadingTextContainer.GetLoadingText().Count();
		}
		mSeenLoadingTextIndices.Add(mLoadingTextIdx);
		mLoadingTextTime = LOADING_TEXT_TIME;
		mCloudUpdateCount = 0;
		mRippleCnt = 0f;
		mPantalookRipplePct.SetConstant(1.0);
		mUserProfileLoaded = true;
		mLoading = false;
		mLeftTorch = null;
		mRightTorch = null;
		mVolcanoSmoke = null;
		mLoadingYOffset = Common._DS(Common._M(998));
		mLoadingTextFrame.mWidth = IMAGE_LS_REDLOADINGBAR.GetWidth();
		mLoadingTextFrame.mHeight = IMAGE_LS_REDLOADINGBAR.GetHeight();
		mLoadingTextFrame.mX = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_LS_REDLOADINGBAR) - mLoadingXOffset);
		mLoadingTextFrame.mY = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_LS_REDLOADINGBAR)) + mLoadingYOffset;
		for (int i = 0; i < MAX_VOLCANO_PROJECTILES; i++)
		{
			mVolcanoProjectiles[i] = new VolcanoProjectile();
			mVolcanoProjectiles[i].mProjectile = null;
		}
	}

	private void BuildAndroidLogoFrames()
	{
		if (!OperatingSystem.IsAndroid() || GlobalMembers.gSexyAppBase.mGraphicsDriver is not XNAGraphicsDriver xNAGraphicsDriver)
		{
			return;
		}
		for (int i = 0; i < mAndroidLogoFrames.Length; i++)
		{
			Image imageByID = Res.GetImageByID((ResID)(1154 + i));
			MemoryImage memoryImage = imageByID?.AsMemoryImage();
			if (memoryImage == null)
			{
				continue;
			}
			uint[] bits = memoryImage.GetBits();
			if (bits == null || bits.Length == 0)
			{
				continue;
			}
			uint[] array = new uint[bits.Length];
			Array.Copy(bits, array, bits.Length);
			Texture2D texture2D = new Texture2D(xNAGraphicsDriver.mXNARenderDevice.mDevice.GraphicsDevice, memoryImage.mWidth, memoryImage.mHeight, false, SurfaceFormat.Color);
			texture2D.SetData(array);
			texture2D.Name = imageByID.mNameForRes;
			DeviceImage optimizedImage = xNAGraphicsDriver.mXNARenderDevice.GetOptimizedImage(texture2D, commitBits: false, allowTriReps: false);
			optimizedImage.mNameForRes = imageByID.mNameForRes;
			optimizedImage.mFileName = imageByID.mFileName;
			optimizedImage.mFilePath = imageByID.mFilePath;
			mAndroidLogoFrames[i] = optimizedImage;
		}
		for (int j = 0; j < mAndroidLightningImages.Length; j++)
		{
			Image imageByID2 = Res.GetImageByID((ResID)(1151 + j));
			MemoryImage memoryImage2 = imageByID2?.AsMemoryImage();
			if (memoryImage2 == null)
			{
				continue;
			}
			uint[] bits2 = memoryImage2.GetBits();
			if (bits2 == null || bits2.Length == 0)
			{
				continue;
			}
			uint[] array2 = new uint[bits2.Length];
			Array.Copy(bits2, array2, bits2.Length);
			Texture2D texture2D2 = new Texture2D(xNAGraphicsDriver.mXNARenderDevice.mDevice.GraphicsDevice, memoryImage2.mWidth, memoryImage2.mHeight, false, SurfaceFormat.Color);
			texture2D2.SetData(array2);
			texture2D2.Name = imageByID2.mNameForRes;
			DeviceImage optimizedImage2 = xNAGraphicsDriver.mXNARenderDevice.GetOptimizedImage(texture2D2, commitBits: false, allowTriReps: false);
			optimizedImage2.mNameForRes = imageByID2.mNameForRes;
			optimizedImage2.mFileName = imageByID2.mFileName;
			optimizedImage2.mFilePath = imageByID2.mFilePath;
			mAndroidLightningImages[j] = optimizedImage2;
		}
		if (mAndroidBlackOverlay == null)
		{
			Texture2D texture2D3 = new Texture2D(xNAGraphicsDriver.mXNARenderDevice.mDevice.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			texture2D3.SetData(new uint[1] { 4278190080u });
			texture2D3.Name = "AndroidBlackOverlay";
			mAndroidBlackOverlay = xNAGraphicsDriver.mXNARenderDevice.GetOptimizedImage(texture2D3, commitBits: false, allowTriReps: false);
			mAndroidBlackOverlay.mNameForRes = "AndroidBlackOverlay";
		}
		if (mAndroidWhiteOverlay == null)
		{
			Texture2D texture2D4 = new Texture2D(xNAGraphicsDriver.mXNARenderDevice.mDevice.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			texture2D4.SetData(new uint[1] { uint.MaxValue });
			texture2D4.Name = "AndroidWhiteOverlay";
			mAndroidWhiteOverlay = xNAGraphicsDriver.mXNARenderDevice.GetOptimizedImage(texture2D4, commitBits: false, allowTriReps: false);
			mAndroidWhiteOverlay.mNameForRes = "AndroidWhiteOverlay";
		}
	}

	private Image GetLogoFrame(int frame)
	{
		if ((uint)frame < (uint)mAndroidLogoFrames.Length && mAndroidLogoFrames[frame] != null)
		{
			return mAndroidLogoFrames[frame];
		}
		return Res.GetImageByID((ResID)(1154 + frame));
	}

	private Image GetLightningImage(int variant)
	{
		if ((uint)variant < (uint)mAndroidLightningImages.Length && mAndroidLightningImages[variant] != null)
		{
			return mAndroidLightningImages[variant];
		}
		return Res.GetImageByID((ResID)(1151 + variant));
	}

	private bool IsAndroidManualLightningSequenceActive()
	{
		return OperatingSystem.IsAndroid() && mState == 1;
	}

	private bool IsAndroidLightningFrameVisible()
	{
		return mAndroidLightningSequenceStep == 1 || mAndroidLightningSequenceStep == 3 || mAndroidLightningSequenceStep == 5;
	}

	private void DrawLogoFrame(Graphics g, Image logoFrame, int x, int y)
	{
		DrawLogoFrame(g, logoFrame, x, y, 255);
	}

	private void DrawLogoFrame(Graphics g, Image logoFrame, int x, int y, int alpha)
	{
		if (logoFrame == null || alpha <= 0)
		{
			return;
		}
		g.PushState();
		g.SetDrawMode(0);
		g.SetColorizeImages(alpha < 255);
		g.SetColor(255, 255, 255, alpha);
		g.DrawImage(logoFrame, x, y);
		g.PopState();
	}

	private void DrawAndroidFullscreenOverlay(Graphics g, Image overlay, int alpha)
	{
		if (overlay == null || alpha <= 0)
		{
			return;
		}
		g.PushState();
		g.SetDrawMode(0);
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(255, 255, 255, alpha);
		g.DrawImage(overlay, 0, 0, GlobalMembers.gSexyApp.mScreenBounds.mWidth, GlobalMembers.gSexyApp.mScreenBounds.mHeight);
		g.PopState();
	}

	private void Init()
	{
		int seed = (int)SexyFramework.Common.SexyTime();
		RandomNumbers.Seed(seed);
		MathUtils.Seed(seed);
		SexyApp gSexyApp = GlobalMembers.gSexyApp;
		mLoadingXOffset = 0;
		mLavaAlpha = 0f;
		mIncLavaAlpha = true;
		mLoadingCompleteDelay = 0;
		mState = 0;
		mHasShown = false;
		mLightningOn = true;
		mLightningTimer = 0;
		mLightningFrame = 0;
		mCanShowMenu = true;
		mLoadingBarAlpha = 0f;
		mCompleteLoadingBarAlpha = 0f;
		mBlackFadeAlpha = 255f;
		mAndroidLightningWarmupFrames = 0;
		mAndroidLightningFlashHoldFrames = 0;
		mAndroidLightningSequenceStep = 0;
		mAndroidLightningSequenceTimer = 0;
		mLoadingComplete = false;
		mBlackFadeIn = true;
		mStormTimer = (mClearTimer = Common._M(100));
		mFadeToMainMenu = false;
		mWaves[0] = new LoadingWave();
		mWaves[1] = new LoadingWave();
		mWaves[2] = new LoadingWave();
		mWaves[3] = new LoadingWave();
		mWaves[4] = new LoadingWave();
		mWaves[0].mRadius = Common._M(20f);
		mWaves[0].mAngle = Common._M1(1f);
		mWaves[0].mAngleRate = Common._M2(-0.035f);
		mWaves[0].mY = Common._M3(860);
		mWaves[1].mRadius = Common._M(16f);
		mWaves[1].mAngle = Common._M1(-1f);
		mWaves[1].mAngleRate = Common._M2(-0.025f);
		mWaves[1].mY = Common._M3(700);
		mWaves[2].mRadius = Common._M(12f);
		mWaves[2].mAngle = Common._M1(2f);
		mWaves[2].mAngleRate = Common._M2(-0.015f);
		mWaves[2].mY = Common._M3(620);
		mWaves[3].mRadius = Common._M(10f);
		mWaves[3].mAngle = Common._M1(-2f);
		mWaves[3].mAngleRate = Common._M2(-0.01f);
		mWaves[3].mY = Common._M3(520);
		mWaves[4].mRadius = Common._M(6f);
		mWaves[4].mAngle = Common._M1(3f);
		mWaves[4].mAngleRate = Common._M2(-0.005f);
		mWaves[4].mY = Common._M3(460);
		mCalmWaves[0] = new LoadingWave();
		mCalmWaves[1] = new LoadingWave();
		mCalmWaves[2] = new LoadingWave();
		mCalmWaves[3] = new LoadingWave();
		mCalmWaves[0].mRadius = Common._M(2f);
		mCalmWaves[0].mAngle = Common._M1(1f);
		mCalmWaves[0].mAngleRate = Common._M2(-0.035f) / 2f;
		mCalmWaves[0].mY = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_LS_CALM_WAVE_1));
		mCalmWaves[1].mRadius = Common._M(8f);
		mCalmWaves[1].mAngle = Common._M1(-1f);
		mCalmWaves[1].mAngleRate = Common._M2(-0.025f) / 2f;
		mCalmWaves[1].mY = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_LS_CALM_WAVE_2));
		mCalmWaves[2].mRadius = Common._M(6f);
		mCalmWaves[2].mAngle = Common._M1(2f);
		mCalmWaves[2].mAngleRate = Common._M2(-0.015f) / 2f;
		mCalmWaves[2].mY = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_LS_CALM_WAVE_3));
		mCalmWaves[3].mRadius = Common._M(5f);
		mCalmWaves[3].mAngle = Common._M1(-2f);
		mCalmWaves[3].mAngleRate = Common._M2(-0.01f) / 2f;
		mCalmWaves[3].mY = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_LS_CALM_WAVE_4));
		for (int i = 0; i < 4; i++)
		{
			mCalmWaves[i].mVX = 0.5f / (float)(i + 1);
			mCalmWaves[i].mXOff = 0f;
			mCalmWaves[i].mMaxXOff = Common._DS(20f) / (float)(i + 1);
			mCalmWaves[i].mY = (float)(gSexyApp.mHeight - Common._DS(452)) + mCalmWaves[i].mY;
			if (i % 2 == 0)
			{
				mCalmWaves[i].mIncVX = true;
			}
		}
		mFrogAngleDivisor = 1f;
		mFlashAlpha = 0f;
		mExtraProgress = 0f;
		mFrogAngle = 0f;
		mFrogPitchForward = true;
		mFrogAngleDelta = Common._M(0.004f);
		mClouds[0] = new LoadingCloud();
		mClouds[1] = new LoadingCloud();
		mClouds[2] = new LoadingCloud();
		mClouds[0].mStartX = Common._DS(Common._M(-400));
		mClouds[0].mY = Common._M1(-400);
		mClouds[0].mShadowOffset = Common._DS(Common._M2(-30)) - Common._DS(160);
		mClouds[0].mShadowY = Common._DS(Common._M3(450));
		mClouds[1].mStartX = Common._DS(Common._M(0));
		mClouds[1].mY = Common._M1(0);
		mClouds[1].mShadowOffset = Common._DS(Common._M2(-19)) - Common._DS(160);
		mClouds[1].mShadowY = Common._DS(Common._M3(550));
		mClouds[2].mStartX = Common._DS(Common._M(-320));
		mClouds[2].mY = Common._M1(25);
		mClouds[2].mShadowOffset = Common._DS(Common._M2(-70)) - Common._DS(160);
		mClouds[2].mShadowY = Common._DS(Common._M3(650));
		mFrogWave = 0;
		mFrogPct = 0f;
		mFrogScale = 1f;
		mEffectBatch = new PIEffectBatch();
	}

	public override void Update()
	{
		if (GameApp.gApp.IsHardwareBackButtonPressed())
		{
			ProcessHardwareBackButton();
		}
		base.Update();
		if (!mLoadingComplete && GameApp.gApp.GetLoadingThreadProgress() >= 1.0)
		{
			GameApp.gApp.LoadingThreadCompleted();
		}
		if (GameApp.gApp.StartLoadingComplete)
		{
			GameApp.gApp.LoadLevelXML();
			GameApp.gApp.StartLoadingComplete = false;
		}
		if (mBlackFadeIn && mState == 0)
		{
			mBlackFadeAlpha -= 2f;
			if (mBlackFadeAlpha <= 0f)
			{
				mBlackFadeAlpha = 0f;
			}
		}
		if (mLockBGM)
		{
			return;
		}
		if (mFlashAlpha > 0f)
		{
			if (!IsAndroidManualLightningSequenceActive() && !(OperatingSystem.IsAndroid() && mState == 1 && mAndroidLightningFlashHoldFrames > 0))
			{
				mFlashAlpha -= ((mState == 2) ? Common._M(1.5f) : Common._M1(10f));
			}
			if (mFlashAlpha < 0f)
			{
				mFlashAlpha = 0f;
			}
		}
		if (!mLoading && GameApp.gApp.mLoadLevelSuccess)
		{
			GameApp.gApp.LoadingThreadProc();
			mLoading = true;
		}
		if (!mLoadingComplete)
		{
			if (--mLoadingTextTime == 0)
			{
				if (mSeenLoadingTextIndices.Capacity == mLoadingTextContainer.GetLoadingText().Count())
				{
					mSeenLoadingTextIndices.Clear();
				}
				List<int> list = new List<int>();
				for (int i = 0; i < mLoadingTextContainer.GetLoadingText().Count(); i++)
				{
					bool flag = false;
					for (int j = 0; j < mSeenLoadingTextIndices.Count(); j++)
					{
						if (mSeenLoadingTextIndices[j] == i)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list.Add(i);
					}
				}
				if (list.Count == 0)
				{
					mLoadingTextIdx = 0;
					mSeenLoadingTextIndices.Clear();
				}
				else if (mFirstRun)
				{
					mLoadingTextIdx = (mLoadingTextIdx + 1) % mLoadingTextContainer.GetBackStoryText().Count();
				}
				else
				{
					int index = RandomNumbers.NextNumber() % list.Count;
					mLoadingTextIdx = list[index];
				}
				mSeenLoadingTextIndices.Add(mLoadingTextIdx);
				mLoadingTextTime = LOADING_TEXT_TIME;
			}
		}
		else
		{
			mLoadingCompleteTime++;
			if (mLoadingCompleteTime == 200)
			{
				GameApp.gApp.PlaySong(0);
			}
		}
		if (mFrogPitchForward)
		{
			mFrogAngle -= mFrogAngleDelta / mFrogAngleDivisor;
			if (mFrogAngle <= 0f - max_angle)
			{
				mFrogAngle = 0f - max_angle;
				mFrogAngleDelta *= -1f;
				mFrogPitchForward = false;
			}
		}
		else
		{
			mFrogAngle -= mFrogAngleDelta / mFrogAngleDivisor;
			if (mFrogAngle >= max_angle)
			{
				mFrogAngle = max_angle;
				mFrogAngleDelta = MathUtils.FloatRange(Common._M(0.0015f), Common._M1(0.003f));
				if (mLoadingComplete && MathUtils._geq(mExtraProgress, 1f, 0.01f))
				{
					max_angle = 0.04313f;
				}
				mFrogPitchForward = true;
			}
		}
		if (mLoadingComplete && mState == 2)
		{
			if (mStormTimer > 0)
			{
				if (--mStormTimer == 0)
				{
					GameApp.gApp.mSoundPlayer.Fade(Res.GetSoundByID(ResID.SOUND_LS_STORM_LOOP));
				}
			}
			else if (++mLoadingCompleteDelay >= Common._M(5))
			{
				mExtraProgress += Common._M(0.003f);
				if (mExtraProgress > 1f)
				{
					mExtraProgress = 1f;
					if (--mClearTimer <= 0 && !GameApp.gApp.mFromReInit)
					{
						mState++;
						SoundAttribs soundAttribs = new SoundAttribs();
						soundAttribs.fadeout = 0.1f;
						GameApp.gApp.mSoundPlayer.Loop(Res.GetSoundByID(ResID.SOUND_SEAGULLS), soundAttribs);
					}
				}
			}
		}
		if (MathUtils._geq(mExtraProgress, 0.5f) && mDarkIslandAlpha > 0)
		{
			mDarkIslandAlpha -= Common._M(2);
		}
		if (mState >= 2 && !mFadeToMainMenu)
		{
			if (mLoadingBarAlpha < 255f)
			{
				mLoadingBarAlpha += Common._M(2f);
			}
			if (mLoadingBarAlpha > 255f)
			{
				mLoadingBarAlpha = 255f;
			}
			if (mFrogPct < Common._M(0.8f))
			{
				mFrogPct += Common._M(0.0001f) / (float)(mFrogWave + 1);
			}
		}
		if (mState == 0)
		{
			if (mHasShown)
			{
				if (mPartnerLogos.Capacity > 0)
				{
					PartnerLogo partnerLogo = mPartnerLogos[0];
					if (partnerLogo.mAlpha < 255 && partnerLogo.mTime == partnerLogo.mOrgTime)
					{
						partnerLogo.mAlpha += Common._M(5);
						if (partnerLogo.mAlpha >= 255)
						{
							partnerLogo.mAlpha = 255;
						}
					}
					else if (--partnerLogo.mTime <= 0)
					{
						partnerLogo.mAlpha -= Common._M(5);
						if (partnerLogo.mAlpha <= 0)
						{
							mPartnerLogos.RemoveAt(0);
						}
					}
				}
				else if (--mLogoHoldTime == 0)
				{
					GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_LS_THUNDERSTRIKE));
					mState++;
					if (OperatingSystem.IsAndroid())
					{
						mAndroidLightningWarmupFrames = 0;
						mAndroidLightningFlashHoldFrames = 0;
						mAndroidLightningSequenceStep = 1;
						mAndroidLightningSequenceTimer = Common._M2(10);
						mLightningFrame = 2;
						mLightningTimer = 0;
						mLightningOn = false;
						mFlashAlpha = 255f;
						mLogoLightning.Clear();
					}
					GameApp.gApp.InitMetricsManager();
				}
			}
		}
		else if (mState == 1)
		{
			if (OperatingSystem.IsAndroid())
			{
				if (mFlashAlpha > 0f)
				{
					mFlashAlpha -= Common._M1(10f);
					if (mFlashAlpha < 0f)
					{
						mFlashAlpha = 0f;
					}
				}
				switch (mAndroidLightningSequenceStep)
				{
				case 0:
					if (--mAndroidLightningSequenceTimer <= 0)
					{
						mAndroidLightningSequenceStep = 1;
						mAndroidLightningSequenceTimer = Common._M2(10);
						mLightningFrame = 2;
						mFlashAlpha = 255f;
					}
					return;
				case 1:
					if (--mAndroidLightningSequenceTimer <= 0)
					{
						mAndroidLightningSequenceStep = 2;
						mAndroidLightningSequenceTimer = Common._M3(15);
					}
					return;
				case 2:
					if (--mAndroidLightningSequenceTimer <= 0)
					{
						mAndroidLightningSequenceStep = 3;
						mAndroidLightningSequenceTimer = Common._M4(15);
						mLightningFrame = 3;
						mFlashAlpha = 255f;
					}
					return;
				case 3:
					if (--mAndroidLightningSequenceTimer <= 0)
					{
						mAndroidLightningSequenceStep = 4;
						mAndroidLightningSequenceTimer = Common._M(10);
					}
					return;
				case 4:
					if (--mAndroidLightningSequenceTimer <= 0)
					{
						mAndroidLightningSequenceStep = 5;
						mAndroidLightningSequenceTimer = Common._M1(10);
						mLightningFrame = 4;
						mFlashAlpha = 255f;
					}
					return;
				case 5:
					if (--mAndroidLightningSequenceTimer <= 0)
					{
						mAndroidLightningSequenceStep = 6;
						mAndroidLightningSequenceTimer = Common._M2(10);
					}
					return;
				case 6:
					if (--mAndroidLightningSequenceTimer <= 0)
					{
						mFlashAlpha = 0f;
						mState++;
					}
					return;
				}
			}
			if (mAndroidLightningWarmupFrames > 0)
			{
				mAndroidLightningWarmupFrames--;
				if (mAndroidLightningWarmupFrames == 0)
				{
					mAndroidLightningFlashHoldFrames = 6;
					mFlashAlpha = 255f;
					mLogoLightning.Clear();
				}
				return;
			}
			if (OperatingSystem.IsAndroid() && mAndroidLightningFlashHoldFrames > 0)
			{
				mAndroidLightningFlashHoldFrames--;
				mFlashAlpha = 255f;
				mLogoLightning.Clear();
				if (mAndroidLightningFlashHoldFrames == 0)
				{
					mLightningFrame = 2;
					mLightningTimer = 0;
					mLightningOn = true;
				}
				return;
			}
			int[] array = new int[5]
			{
				Common._M(5),
				Common._M1(10),
				Common._M2(10),
				Common._M3(15),
				Common._M4(10)
			};
			int[] array2 = new int[5]
			{
				Common._M(5),
				Common._M1(5),
				Common._M2(15),
				Common._M3(10),
				Common._M4(10)
			};
			int num = (mLightningOn ? array[mLightningFrame] : array2[mLightningFrame]);
			if (++mLightningTimer == num)
			{
				mLightningTimer = 0;
				mLightningOn = !mLightningOn;
				if (mLightningOn && mFlashAlpha <= 0f)
				{
					mFlashAlpha = 255f;
				}
				if (mLightningOn && ++mLightningFrame == 5)
				{
					mFlashAlpha = 255f;
					mState++;
				}
			}
			if ((!OperatingSystem.IsAndroid() || mAndroidLightningFlashHoldFrames <= 0) && mLogoLightning.Count() < Common._M(3) && MathUtils.SafeRand() % Common._M1(20) == 0)
			{
				mLogoLightning.Add(new LogoLightning());
				LogoLightning logoLightning = mLogoLightning[mLogoLightning.Count() - 1];
				logoLightning.mImage = GetLightningImage(MathUtils.SafeRand() % 2);
				logoLightning.mTimer = (logoLightning.mTimerTarget = MathUtils.IntRange(Common._M(5), Common._M1(25)));
			}
			for (int k = 0; k < mLogoLightning.Count(); k++)
			{
				LogoLightning logoLightning2 = mLogoLightning[k];
				if (--logoLightning2.mTimer == 0)
				{
					mLogoLightning.RemoveAt(k);
					k--;
				}
			}
		}
		if (mLoadingComplete && !GameApp.gApp.mFromReInit && mCompleteLoadingBarAlpha < 255f && mLoadingCompleteTime >= 400)
		{
			mCompleteLoadingBarAlpha += Common._M(3f);
			if (mCompleteLoadingBarAlpha > 255f)
			{
				mCompleteLoadingBarAlpha = 255f;
			}
		}
		if (mFadeToMainMenu)
		{
			if (mBlackFadeIn)
			{
				mBlackFadeAlpha += Common._M(5f);
				if (mBlackFadeAlpha >= 255f)
				{
					mBlackFadeAlpha = 255f;
					mBlackFadeIn = false;
				}
			}
			else
			{
				mBlackFadeAlpha -= Common._M(2f);
				if (mBlackFadeAlpha <= 0f)
				{
					mBlackFadeAlpha = 0f;
				}
			}
		}
		if (mLoadingComplete && mExtraProgress < 0.99f)
		{
			mFrogAngleDivisor += Common._M(0.018f);
		}
		for (int l = 0; l < 5; l++)
		{
			LoadingWave loadingWave = mWaves[l];
			loadingWave.mAngle += loadingWave.mAngleRate;
			if (l >= 4)
			{
				continue;
			}
			float num2 = Common._M(0.0005f) / (float)(l + 1);
			mCalmWaves[l].mAngle += mCalmWaves[l].mAngleRate;
			if (!mCalmWaves[l].mIncVX)
			{
				mCalmWaves[l].mVX -= num2;
			}
			else
			{
				mCalmWaves[l].mVX += num2;
			}
			float num3 = Common._M(0.2f);
			if (mCalmWaves[l].mVX > num3)
			{
				mCalmWaves[l].mVX = num3;
			}
			else if (mCalmWaves[l].mVX < 0f - num3)
			{
				mCalmWaves[l].mVX = 0f - num3;
			}
			mCalmWaves[l].mXOff += mCalmWaves[l].mVX;
			if (mCalmWaves[l].mXOff >= mCalmWaves[l].mMaxXOff)
			{
				if (mCalmWaves[l].mVX > 0f)
				{
					mCalmWaves[l].mVX /= Common._M(4f);
				}
				mCalmWaves[l].mIncVX = false;
			}
			else if (mCalmWaves[l].mXOff <= 0f - mCalmWaves[l].mMaxXOff)
			{
				if (mCalmWaves[l].mVX < 0f)
				{
					mCalmWaves[l].mVX /= Common._M(4f);
				}
				mCalmWaves[l].mIncVX = true;
			}
		}
		for (int m = 0; m < 3; m++)
		{
			LoadingCloud loadingCloud = mClouds[m];
			if (OperatingSystem.IsAndroid() && mState < 2)
			{
				loadingCloud.mLightning = null;
				loadingCloud.mLightningTimer = 0;
				continue;
			}
			if (loadingCloud.mLightning == null && mExtraProgress == 0f && MathUtils.SafeRand() % Common._M(200) == 0)
			{
				loadingCloud.mLightning = GetLightningImage(MathUtils.SafeRand() % 1);
				loadingCloud.mLightningTimer = (loadingCloud.mTimerTarget = MathUtils.IntRange(Common._M(10), Common._M1(25)));
				loadingCloud.mLightningScale = Common._M(0.75f) - (float)m * Common._M1(0.15f);
			}
			if (loadingCloud.mLightningTimer > 0 && --loadingCloud.mLightningTimer <= 0)
			{
				loadingCloud.mLightning = null;
			}
		}
		if (mLoadingComplete && mExtraProgress > 0f)
		{
			if (mIncLavaAlpha)
			{
				mLavaAlpha += Common._M(0.5f);
				if (mLavaAlpha >= 255f)
				{
					mLavaAlpha = 255f;
					mIncLavaAlpha = false;
				}
			}
			else
			{
				mLavaAlpha -= Common._M(0.5f);
				if (mLavaAlpha <= 0f)
				{
					mLavaAlpha = 0f;
					mIncLavaAlpha = true;
				}
			}
		}
		if (mLoadingComplete)
		{
			if (mLeftTorch == null)
			{
				mLeftTorch = GameApp.gApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_PARTICLES_LS_TIKITORCH_FLAME").Duplicate();
				mLeftTorch.mEmitAfterTimeline = true;
				mLeftTorch.mDrawTransform.LoadIdentity();
				mLeftTorch.mDrawTransform.Scale(Common._DS(1.4f), Common._DS(1.4f));
				mLeftTorch.mDrawTransform.Translate(Common._S(mX) + Common._DS(Common._M(264)) + mOffsetParticle, Common._S(mY) + Common._DS(Common._M1(430)));
				Common.SetFXNumScale(mLeftTorch, 4f);
				mEffectBatch.AddEffect(mLeftTorch);
			}
			if (mRightTorch == null)
			{
				mRightTorch = GameApp.gApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_PARTICLES_LS_TIKITORCH_FLAME").Duplicate();
				mRightTorch.mEmitAfterTimeline = true;
				mRightTorch.mDrawTransform.LoadIdentity();
				mRightTorch.mDrawTransform.Scale(Common._DS(1.4f), Common._DS(1.4f));
				mRightTorch.mDrawTransform.RotateDeg(Common._M(-20));
				mRightTorch.mDrawTransform.Translate(Common._S(mX) + Common._DS(Common._M(1357)) + mOffsetParticle, Common._S(mY) + Common._DS(Common._M1(430)));
				Common.SetFXNumScale(mRightTorch, 4f);
				mEffectBatch.AddEffect(mRightTorch);
			}
			if (mVolcanoSmoke == null)
			{
				mVolcanoSmoke = GameApp.gApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_PARTICLES_VOLCANO_SMOKE").Duplicate();
				mVolcanoSmoke.mEmitAfterTimeline = true;
				mVolcanoSmoke.mDrawTransform.LoadIdentity();
				mVolcanoSmoke.mDrawTransform.Scale(Common._DS(1.4f), Common._DS(1.4f));
				mVolcanoSmoke.mDrawTransform.Translate(Common._S(mX) + Common._DS(Common._M(790)) + mOffsetParticle, Common._S(mY) + Common._DS(Common._M1(90)));
				Common.SetFXNumScale(mVolcanoSmoke, 3f);
				mEffectBatch.AddEffect(mVolcanoSmoke);
			}
			if (mLeftTorch != null)
			{
				mLeftTorch.Update();
			}
			if (mRightTorch != null)
			{
				mRightTorch.Update();
			}
			if (mVolcanoSmoke != null)
			{
				mVolcanoSmoke.Update();
			}
		}
		if (SexyFramework.Common.Rand(Common._M(100)) == 0)
		{
			for (int n = 0; n < MAX_VOLCANO_PROJECTILES; n++)
			{
				VolcanoProjectile volcanoProjectile = mVolcanoProjectiles[n];
				if (volcanoProjectile.mProjectile != null && !volcanoProjectile.mProjectile.mInUse)
				{
					volcanoProjectile.mProjectile.mInUse = true;
					volcanoProjectile.mProjectile.ResetAnim();
					volcanoProjectile.mProjectile.mRandSeeds.Clear();
					volcanoProjectile.mProjectile.mRandSeeds.Add(SexyFramework.Common.Rand(1000));
					break;
				}
			}
		}
		for (int num4 = 0; num4 < MAX_VOLCANO_PROJECTILES; num4++)
		{
			VolcanoProjectile volcanoProjectile2 = mVolcanoProjectiles[num4];
			if (volcanoProjectile2.mProjectile != null && volcanoProjectile2.mProjectile.mInUse && volcanoProjectile2.mProjectile != null)
			{
				volcanoProjectile2.mProjectile.Update();
				if (volcanoProjectile2.mProjectile.mCurNumParticles == 0 && MathUtils._geq(volcanoProjectile2.mProjectile.mFrameNum, volcanoProjectile2.mProjectile.mLastFrameNum))
				{
					volcanoProjectile2.mProjectile.mInUse = false;
				}
			}
		}
		if (mLoadingComplete)
		{
			for (int num5 = 0; num5 < 3; num5++)
			{
				LoadingCloud loadingCloud2 = mClouds[num5];
				loadingCloud2.mVX += Common._M(0.0004f);
			}
		}
		if (GameApp.gApp.mUserProfile != null && GameApp.gApp.mUserProfile.IsLoaded())
		{
			mUserProfileLoaded = true;
		}
		mRippleCnt += (float)(double)mPantalookRipplePct;
		MarkDirty();
	}

	public override void Dispose()
	{
		mAndroidBlackOverlay?.Dispose();
		mAndroidBlackOverlay = null;
		mAndroidWhiteOverlay?.Dispose();
		mAndroidWhiteOverlay = null;
		for (int i = 0; i < mAndroidLogoFrames.Length; i++)
		{
			mAndroidLogoFrames[i]?.Dispose();
			mAndroidLogoFrames[i] = null;
		}
		for (int j = 0; j < mAndroidLightningImages.Length; j++)
		{
			mAndroidLightningImages[j]?.Dispose();
			mAndroidLightningImages[j] = null;
		}
		RemoveAllWidgets(doDelete: true, recursive: true);
		mLeftTorch.Dispose();
		mRightTorch.Dispose();
		mVolcanoSmoke.Dispose();
		for (int i = 0; i < MAX_VOLCANO_PROJECTILES; i++)
		{
			mVolcanoProjectiles[i].mProjectile.Dispose();
		}
	}

	public override void Draw(Graphics g)
	{
		if (mState >= 2 && mState == 2)
		{
			GameApp.gApp.GetLoadingThreadProgress();
		}
		g.SetDrawMode(0);
		g.SetColorizeImages(colorizeImages: false);
		g.SetColor(255, 255, 255, 255);
		mHasShown = true;
		if (mWaitingForConfirmation)
		{
			Image logoFrame = GetLogoFrame(0);
			if (OperatingSystem.IsAndroid() && mState < 2 && mAndroidBlackOverlay != null)
			{
				DrawAndroidFullscreenOverlay(g, mAndroidBlackOverlay, 255);
			}
			else
			{
				g.SetColor(Color.Black);
				g.FillRect(GlobalMembers.gSexyApp.mScreenBounds);
			}
			DrawLogoFrame(g, logoFrame, pts[0].mX, pts[0].mY);
			return;
		}
		if (mState < 2)
		{
			if (OperatingSystem.IsAndroid() && mAndroidBlackOverlay != null)
			{
				DrawAndroidFullscreenOverlay(g, mAndroidBlackOverlay, 255);
			}
			else
			{
				g.SetColor(Color.Black);
				g.FillRect(GlobalMembers.gSexyApp.mScreenBounds);
			}
		}
		float num = 255f - 255f * mExtraProgress;
		int num2 = ((num < (float)Common._M(128)) ? ((int)num) : Common._M1(128));
		if (mState >= 2 && !mFadeToMainMenu)
		{
			g.SetColorizeImages(colorizeImages: true);
			int num3 = (int)((float)Common._M(51) + (255f - num) * Common._M1(0.8f));
			g.SetColor(num3, num3, num3, 255);
			g.DrawImage(IMAGE_LS_HAPPYSKY_BKGRND, Common._S(0), 0, GameApp.gApp.GetScreenRect().mWidth, IMAGE_LS_HAPPYSKY_BKGRND.GetHeight());
			g.SetColorizeImages(colorizeImages: false);
			if (num > 0f)
			{
				g.PushState();
				g.PopState();
			}
			for (int i = 0; i < 3; i++)
			{
				if (i == 1)
				{
					g.DrawImage(IMAGE_LS_HAPPYSKY_LAVA, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_LS_HAPPYSKY_LAVA) - mLoadingXOffset), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_LS_HAPPYSKY_LAVA)));
					g.SetColor(255, 255, 255, (int)mLavaAlpha);
					g.SetDrawMode(1);
					g.SetColorizeImages(colorizeImages: true);
					g.DrawImage(IMAGE_LS_HAPPYSKY_LAVA, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_LS_HAPPYSKY_LAVA) - mLoadingXOffset), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_LS_HAPPYSKY_LAVA)));
					g.SetColorizeImages(colorizeImages: false);
					g.SetDrawMode(0);
				}
				ResID id = (ResID)(1149 - i);
				int theX = Common._DS(Res.GetOffsetXByID(id) - mLoadingXOffset);
				int theY = Common._DS(Res.GetOffsetYByID(id));
				g.PushState();
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, 255 - (int)num);
				if (num < 255f)
				{
					g.DrawImage(Res.GetImageByID(id), theX, theY);
				}
				g.PopState();
			}
			if (mLoadingComplete && num != 255f)
			{
				mEffectBatch.DrawBatch(g);
			}
			g.PushState();
			if (mDarkIslandAlpha < 255)
			{
				g.SetColorizeImages(colorizeImages: true);
			}
			g.SetColor(255, 255, 255, mDarkIslandAlpha);
			g.PopState();
			if (num != 255f)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)num);
			}
			g.DrawImage(IMAGE_LS_CENTER_CLOUD, (int)((float)GameApp.gApp.mWidth - (float)IMAGE_LS_CENTER_CLOUD.mWidth * 2f) / 2, Common._DS(Common._M(240)), (int)((float)IMAGE_LS_CENTER_CLOUD.GetWidth() * 2f), (int)((float)IMAGE_LS_CENTER_CLOUD.GetHeight() * 2f));
			g.SetColorizeImages(colorizeImages: false);
			if (num < 255f && mLoadingComplete)
			{
				if (255f - num > 0f)
				{
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(255, 255, 255, (int)(255f - num));
				}
				for (int num4 = 3; num4 >= 0; num4--)
				{
					ResID id2 = (ResID)(1142 + num4);
					Image imageByID = Res.GetImageByID(id2);
					LoadingWave loadingWave = mCalmWaves[num4];
					float num5 = (float)((mWidth - imageByID.mWidth) / 2) + Common._DS(loadingWave.mRadius * (float)Math.Cos(loadingWave.mAngle)) + loadingWave.mXOff;
					float num6 = loadingWave.mY - Common._DS(loadingWave.mRadius * (float)Math.Sin(loadingWave.mAngle));
					g.DrawImage(imageByID, (int)num5, (int)num6);
				}
				g.SetColorizeImages(colorizeImages: false);
			}
			for (int num7 = 2; num7 >= 0; num7--)
			{
				if (num7 == mFrogWave)
				{
					int num8 = 255;
					if (mLoadingComplete && mLoadingCompleteTime >= 200)
					{
						num8 = 255 - (mLoadingCompleteTime - 200);
					}
					if (num8 < 0)
					{
						num8 = 0;
					}
					if (num8 > 0)
					{
						if (mLoadingComplete)
						{
							mCloudUpdateCount++;
						}
						for (int num9 = 2; num9 >= 0; num9--)
						{
							_ = mWidth;
							_ = mClouds[num9].mStartX;
							float value = mClouds[num9].mStartX + (float)((num9 == 1) ? 1 : (-1)) * mClouds[num9].mVX * (float)mCloudUpdateCount;
							DrawLightning(g, (int)Common._DS(value), num9);
							g.PushState();
							g.SetColorizeImages(colorizeImages: true);
							g.SetColor(255, 255, 255, num8);
							Image imageByID2 = Res.GetImageByID((ResID)(1132 + num9 * 3));
							int num10 = 2;
							g.DrawImage(imageByID2, (int)Common._DS(value), (int)Common._DS(mClouds[num9].mY), imageByID2.GetWidth() * num10, imageByID2.GetHeight() * num10);
							g.PopState();
						}
					}
					float num11 = ((mFrogWave == 0) ? ((float)Common._DS(Common._M(200)) + mFrogPct * (float)Common._DS(Common._M1(1200))) : ((mFrogWave % 2 != 1) ? ((float)Common._DS(Common._M(-700)) + mFrogPct * (float)Common._DS(Common._M1(2800))) : ((float)Common._DS(Common._M(2000)) - mFrogPct * (float)Common._DS(Common._M1(2500)))));
					mGlobalTransform.Reset();
					if (g.Is3D())
					{
						float num12 = 0f;
						float num13 = 0f;
						mGlobalTransform.Translate(0f - num12, 0f - num13);
						if (g.Is3D())
						{
							mGlobalTransform.Scale(mFrogScale, mFrogScale);
							mGlobalTransform.RotateRad(mFrogAngle);
						}
						mGlobalTransform.Translate(num12, num13);
					}
					float num14 = Common._DS(mWaves[mFrogWave].mY - (float)mFrogYOffs[mFrogWave] + (float)Common._M(70) - mWaves[mFrogWave].mRadius / mFrogAngleDivisor * (float)Math.Sin(mWaves[mFrogWave].mAngle));
					num11 -= (float)mFrogXOffset;
					num14 -= (float)mFrogYOffset;
					double num15 = mPantaloonFlopPct;
					num15 *= 18.0;
					_ = (((int)num15 + 1 > 9) ? "" : "0") + ((int)num15 + 1);
					ResID[] array = new ResID[3]
					{
						ResID.IMAGE_LS_RAFTANIM_RAFT,
						(ResID)(1160 + (int)num15),
						ResID.IMAGE_LS_RAFTANIM_FROG
					};
					for (int j = 0; j < 3; j++)
					{
						ResID resID = array[j];
						if (resID == ResID.IMAGE_LS_RAFTANIM_PANTS01 && GlobalMembers.gIs3D)
						{
							Graphics3D graphics3D = g.Get3D();
							SexyVertex2D[] array2 = new SexyVertex2D[20];
							for (int k = 0; k < 10; k++)
							{
								float num16 = (float)(Math.Sin(mRippleCnt * Common._M(0.35f) + (float)k * Common._M1(0.75f)) * (double)Common._S(Common._M2(1.2f)) * (double)k / 9.0 * (double)mPantalookRipplePct);
								float theY2 = (float)((double)Common._S(Common._M(10)) + Math.Sin(mFrogAngle) * (double)Common._S(Common._M1(-50)) + (double)num16 + Math.Sin(mFrogAngle) * (double)Common._S(Common._M2(-80)) * (double)k / 9.0);
								float num17 = (float)((double)IMAGE_LS_RAFTANIM_PANTS01.mWidth + Math.Sin(mFrogAngle) * (double)Common._S(Common._M(10)) + Math.Sin(mRippleCnt * Common._M1(0.2f)) * (double)Common._S(Common._M2(1.2f)) * (double)mPantalookRipplePct);
								float num18 = (float)((double)IMAGE_LS_RAFTANIM_PANTS01.mWidth + Math.Sin(mFrogAngle) * (double)Common._S(Common._M(80)) + Math.Sin(mRippleCnt * Common._M1(0.2f)) * (double)Common._S(Common._M2(1.2f)) * (double)mPantalookRipplePct);
								float num19 = (float)((double)IMAGE_LS_RAFTANIM_PANTS01.mHeight + Math.Max(0.0, Math.Sin(mFrogAngle) * (double)Common._S(Common._M(-70))) * (double)k / 9.0);
								float theY3 = (float)((double)Common._S(Common._M(10)) + Math.Sin(mFrogAngle) * (double)Common._S(Common._M1(-50)) + (double)Math.Max(0f, num16 - Common._M2(0.1f)) + (double)num19 + Math.Sin(mFrogAngle) * (double)Common._S(Common._M3(-80)) * (double)k / 9.0);
								ref SexyVertex2D reference = ref array2[k * 2];
								reference = new SexyVertex2D((float)((double)Common._S(Common._M(47)) + Math.Sin(mFrogAngle) * (double)Common._S(Common._M1(20)) + (double)(num17 * (float)k / 9f) + (double)g.mTransX), theY2, (float)k / 9f, 0f);
								ref SexyVertex2D reference2 = ref array2[k * 2 + 1];
								reference2 = new SexyVertex2D((float)((double)Common._S(Common._M(47)) + Math.Sin(mFrogAngle) * (double)Common._S(Common._M1(85)) + (double)(num18 * (float)k / 9f) + (double)g.mTransX), theY3, (float)k / 9f, 1f);
							}
							graphics3D.SetTexture(0, IMAGE_LS_RAFTANIM_PANTS01);
							graphics3D.DrawPrimitive(0u, Graphics3D.EPrimitiveType.PT_TriangleStrip, array2, Common._M(18), Color.White, 0, num11, num14, blend: true, 0u);
						}
						else
						{
							Image imageByID3 = Res.GetImageByID(resID);
							mGlobalTransform.Reset();
							mGlobalTransform.Translate(Common._DS(Res.GetOffsetXByID(resID)) + imageByID3.mWidth / 2, Common._DS(Res.GetOffsetYByID(resID)) + imageByID3.mHeight / 2);
							mGlobalTransform.Translate(-mCenterOffX, -mCenterOffY);
							if (GlobalMembers.gIs3D)
							{
								mGlobalTransform.Scale(mFrogScale, mFrogScale);
								mGlobalTransform.RotateRad(mFrogAngle);
							}
							mGlobalTransform.Translate(mCenterOffX, mCenterOffY);
							if (GlobalMembers.gIs3D)
							{
								g.DrawImageTransformF(imageByID3, mGlobalTransform, num11, num14);
							}
							else
							{
								g.DrawImageTransform(imageByID3, mGlobalTransform, (int)num11, (int)num14);
							}
						}
					}
				}
				if ((int)num > 0)
				{
					if (num < 255f)
					{
						g.SetColorizeImages(colorizeImages: true);
						g.SetColor(255, 255, 255, (int)num);
					}
					Image imageByID4 = Res.GetImageByID((ResID)(1180 + num7));
					LoadingWave loadingWave2 = mWaves[num7];
					float num20 = (float)((mWidth - imageByID4.mWidth * mWaveImgResScale) / 2) + Common._DS(loadingWave2.mRadius * (float)Math.Cos(loadingWave2.mAngle));
					g.DrawImage(imageByID4, (int)num20, (int)Common._DS(loadingWave2.mY - loadingWave2.mRadius * (float)Math.Sin(loadingWave2.mAngle)), imageByID4.GetWidth() * mWaveImgResScale, imageByID4.GetHeight() * mWaveImgResScale);
					g.SetColorizeImages(colorizeImages: false);
				}
			}
			if (num2 > 0)
			{
				for (int num21 = 2; num21 >= 0; num21--)
				{
					int num22 = (int)((float)mWidth - mClouds[num21].mStartX);
					_ = mClouds[num21].mStartX;
					_ = (float)((num21 == 1) ? 1 : (-1)) * mExtraProgress / 3f;
					g.PushState();
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(255, 255, 255, num2);
					Image imageByID5 = Res.GetImageByID((ResID)(1134 + num21 * 3));
					g.DrawImage(imageByID5, (int)Common._DS(mClouds[num21].mStartX + mExtraProgress / 3f / (float)(num21 + 1) * (float)Common._M(2000) + mClouds[num21].mShadowOffset), (int)Common._DS(mClouds[num21].mShadowY), imageByID5.GetWidth() * 10, imageByID5.GetHeight() * 10);
					g.PopState();
				}
			}
			_ = mLoadingComplete;
		}
		if (!mFadeToMainMenu)
		{
			if (mState == 0)
			{
				if (mPartnerLogos.Count() == 0)
				{
					Image logoFrame2 = GetLogoFrame(0);
					if (OperatingSystem.IsAndroid())
					{
						int alpha = 255 - Math.Clamp((int)mBlackFadeAlpha, 0, 255);
						DrawLogoFrame(g, logoFrame2, pts[0].mX, pts[0].mY, alpha);
					}
					else
					{
						DrawLogoFrame(g, logoFrame2, pts[0].mX, pts[0].mY);
					}
				}
				else
				{
					g.PushState();
					PartnerLogo partnerLogo = mPartnerLogos[0];
					if (partnerLogo.mAlpha != 255)
					{
						g.SetColorizeImages(colorizeImages: true);
					}
					g.SetColor(255, 255, 255, partnerLogo.mAlpha);
					g.DrawImage(partnerLogo.mImage, (mWidth - partnerLogo.mImage.mWidth) / 2, (mHeight - partnerLogo.mImage.mHeight) / 2);
					g.PopState();
				}
			}
			else if (mState == 1)
			{
				Image imageByID6 = GetLogoFrame(mLightningFrame);
				if (OperatingSystem.IsAndroid())
				{
					if (mAndroidLightningSequenceStep <= 0)
					{
						DrawLogoFrame(g, GetLogoFrame(0), pts[0].mX, pts[0].mY);
					}
					else if (IsAndroidLightningFrameVisible())
					{
						DrawLogoFrame(g, imageByID6, pts[mLightningFrame].mX, (mLightningFrame == 0) ? pts[mLightningFrame].mY : 0);
					}
				}
				else if (OperatingSystem.IsAndroid() && (mAndroidLightningWarmupFrames > 0 || mAndroidLightningFlashHoldFrames > 0))
				{
					DrawLogoFrame(g, GetLogoFrame(0), pts[0].mX, pts[0].mY);
				}
				else if (mLightningOn)
				{
					DrawLogoFrame(g, imageByID6, pts[mLightningFrame].mX, (mLightningFrame == 0) ? pts[mLightningFrame].mY : 0);
				}
				for (int l = 0; l < mLogoLightning.Count() && mAndroidLightningWarmupFrames <= 0 && mAndroidLightningFlashHoldFrames <= 0; l++)
				{
					LogoLightning logoLightning = mLogoLightning[l];
					g.SetColor(255, 255, 255, (int)(255f * ((float)logoLightning.mTimer / (float)logoLightning.mTimerTarget)));
					g.SetColorizeImages(colorizeImages: true);
					g.DrawImage(logoLightning.mImage, (mWidth - logoLightning.mImage.mWidth * 2) / 2, 0, logoLightning.mImage.GetWidth() * 2, logoLightning.mImage.GetHeight() * 2);
					g.SetColorizeImages(colorizeImages: false);
				}
			}
		}
		if (mLoadingBarAlpha > 0f && (!mFadeToMainMenu || mBlackFadeIn))
		{
			if (mLoadingBarAlpha < 255f)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)mLoadingBarAlpha);
			}
			g.DrawImage(IMAGE_LS_BACKING, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_LS_BACKING)), mLoadingYOffset + Common._DS(Res.GetOffsetYByID(ResID.IMAGE_LS_BACKING)), GameApp.gApp.GetScreenRect().mWidth, IMAGE_LS_BACKING.GetHeight());
			if (mCompleteLoadingBarAlpha < 255f)
			{
				float num23 = (float)GlobalMembers.gSexyApp.GetLoadingThreadProgress();
				int num24 = (int)((float)IMAGE_LS_REDLOADINGBAR.mWidth * num23);
				if (num24 > IMAGE_LS_REDLOADINGBAR.mWidth)
				{
					num24 = IMAGE_LS_REDLOADINGBAR.mWidth;
				}
				Rect theSrcRect = new Rect(IMAGE_LS_REDLOADINGBAR.mWidth - num24, 0, num24, IMAGE_LS_REDLOADINGBAR.mHeight);
				int num25 = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_LS_REDLOADINGBAR) - mLoadingXOffset);
				g.DrawImage(IMAGE_LS_REDLOADINGBAR, num25, mLoadingYOffset + Common._DS(Res.GetOffsetYByID(ResID.IMAGE_LS_REDLOADINGBAR)), theSrcRect);
				mLoadStarRotateAngle += -0.1f;
				g.DrawImageRotated(IMAGE_LS_STARFISH, num25 + num24 - Common._DS(Common._M(40)), mLoadingYOffset + Common._DS(Res.GetOffsetYByID(ResID.IMAGE_LS_STARFISH)), mLoadStarRotateAngle);
			}
			g.SetColorizeImages(colorizeImages: false);
			if (mCompleteLoadingBarAlpha > 0f)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)mCompleteLoadingBarAlpha);
				g.DrawImage(IMAGE_LS_GREENLOADEDBAR, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_LS_GREENLOADEDBAR) - mLoadingXOffset), mLoadingYOffset + Common._DS(Res.GetOffsetYByID(ResID.IMAGE_LS_GREENLOADEDBAR)));
				if (mCompleteLoadingBarAlpha >= 255f)
				{
					int alpha = 127 + JeffLib.Common.GetAlphaFromUpdateCount(mUpdateCnt, Common._M(128));
					g.SetColor(255, 255, 255, alpha);
				}
				g.SetColorizeImages(colorizeImages: false);
			}
			if (mLoadingBarAlpha < 255f)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)mLoadingBarAlpha);
			}
			g.DrawImage(IMAGE_LS_BAR, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_LS_BAR) - mLoadingXOffset), mLoadingYOffset + Common._DS(Res.GetOffsetYByID(ResID.IMAGE_LS_BAR)));
			if (GameApp.gApp.GetLoadingThreadProgress() < (double)Common._M(0.06f))
			{
				g.DrawImage(IMAGE_LS_L_TIKI01, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_LS_L_TIKI01) - mLoadingXOffset), mLoadingYOffset + Common._DS(Res.GetOffsetYByID(ResID.IMAGE_LS_L_TIKI01)));
			}
			else
			{
				g.DrawImage(IMAGE_LS_L_TIKI02, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_LS_L_TIKI02) - mLoadingXOffset), mLoadingYOffset + Common._DS(Res.GetOffsetYByID(ResID.IMAGE_LS_L_TIKI02)));
			}
			if (GameApp.gApp.GetLoadingThreadProgress() < (double)Common._M(0.95f))
			{
				g.DrawImage(IMAGE_LS_R_TIKI01, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_LS_R_TIKI01) - mLoadingXOffset), mLoadingYOffset + Common._DS(Res.GetOffsetYByID(ResID.IMAGE_LS_R_TIKI01)));
			}
			else
			{
				g.DrawImage(IMAGE_LS_R_TIKI02, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_LS_R_TIKI02) - mLoadingXOffset), mLoadingYOffset + Common._DS(Res.GetOffsetYByID(ResID.IMAGE_LS_R_TIKI02)));
			}
			g.SetColorizeImages(colorizeImages: false);
			if (mCompleteLoadingBarAlpha < 255f)
			{
				g.PushState();
				g.SetColorizeImages(colorizeImages: true);
				int alpha2 = ((mLoadingBarAlpha < 255f) ? ((int)mLoadingBarAlpha) : ((!(mCompleteLoadingBarAlpha > 0f)) ? (127 + JeffLib.Common.GetAlphaFromUpdateCount(mUpdateCnt, Common._M(128))) : (255 - (int)mCompleteLoadingBarAlpha)));
				g.SetColor(200, 200, 200, alpha2);
				Font fontByID = Res.GetFontByID(ResID.FONT_SHAGLOUNGE28_STROKE);
				g.SetFont(fontByID);
				string theLine = ((!mFirstRun) ? mLoadingTextContainer.GetLoadingText()[mLoadingTextIdx] : mLoadingTextContainer.GetBackStoryText()[mLoadingTextIdx]);
				Rect theRect = mLoadingTextFrame;
				theRect.mY = 368;
				theRect.mX = 110;
				g.SetScale(1.5f, 1.5f, g.mScaleOrigX, g.mScaleOrigY);
				g.WriteWordWrapped(theRect, theLine, -1, 0);
				g.PopState();
			}
			if (mCompleteLoadingBarAlpha > 0f)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)mCompleteLoadingBarAlpha);
				int num26 = 369;
				if (Localization.GetCurrentLanguage() != Localization.LanguageType.Language_EN)
				{
					num26 = IMAGE_LS_GREENLOADEDBAR.GetWidth() - 51;
				}
				int num27 = 30;
				int num28 = (num26 - IMAGE_LS_CLICKTXT.GetWidth()) / 2;
				int num29 = (num27 - IMAGE_LS_CLICKTXT.GetHeight()) / 2;
				int num30 = Common._DS(Common._M(605));
				if (Localization.GetCurrentLanguage() != Localization.LanguageType.Language_EN)
				{
					num30 = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_LS_GREENLOADEDBAR)) + 51;
				}
				int num31 = Common._DS(Common._M1(1042));
				g.DrawImage(IMAGE_LS_CLICKTXT, num30 + num28, num31 + num29);
				g.SetColorizeImages(colorizeImages: false);
			}
		}
		if (mFlashAlpha > 0f)
		{
			if (OperatingSystem.IsAndroid() && mState < 2 && mAndroidWhiteOverlay != null)
			{
				DrawAndroidFullscreenOverlay(g, mAndroidWhiteOverlay, (int)mFlashAlpha);
			}
			else
			{
				g.SetColor(255, 255, 255, (int)mFlashAlpha);
				g.FillRect(GlobalMembers.gSexyApp.mScreenBounds);
			}
		}
		if (OperatingSystem.IsAndroid() && mState < 2)
		{
			DrawOverlay(g);
		}
		else
		{
			DeferOverlay(20);
		}
	}

	public void ButtonPress(int id)
	{
	}

	public void ButtonPress(int theId, int theClickCount)
	{
	}

	public void ButtonDepress(int theId)
	{
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
		if (mLockBGM)
		{
			Dialog dialog = GameApp.gApp.GetDialog(1);
			dialog.ButtonDepress(1001);
			GameApp.gApp.OnHardwareBackButtonPressProcessed();
		}
		else if (mState <= 1)
		{
			GameApp.gApp.Shutdown();
			GameApp.gApp.OnHardwareBackButtonPressProcessed();
		}
		else if (!mLoadingComplete)
		{
			GameApp.gApp.OnHardwareBackButtonPressProcessed();
		}
		else if (GameApp.gApp.GetDialog(1) != null)
		{
			GameApp.gApp.GetDialog(1).ButtonDepress(1001);
			GameApp.gApp.OnHardwareBackButtonPressProcessed();
		}
		else
		{
			GameApp.gApp.DoQuitPromptDialog();
			GameApp.gApp.mYesNoDialogDelegate = ProcessYesNo;
			GameApp.gApp.OnHardwareBackButtonPressProcessed();
		}
	}

	public void ProcessYesNo(int theId)
	{
		if (theId == 1000)
		{
			if (!GameApp.gApp.IsRegistered() && GameApp.gApp.mTrialType == 1 && GameApp.gApp.GetBoolean("UpsellExit", theDefault: false))
			{
				GameApp.gApp.DoUpsell(from_exit: true);
			}
			else
			{
				GameApp.gApp.Shutdown();
			}
		}
	}

	public void ProcessBGM()
	{
		string message = TextManager.getInstance().getString(58);
		int width_pad = Common._DS(Common._M(20));
		GameApp.gApp.DoYesNoDialog(TextManager.getInstance().getString(58), message, block: true, TextManager.getInstance().getString(446), TextManager.getInstance().getString(447), drag: false, Common._S(Common._M(50)), 1, width_pad);
		GameApp.gApp.mYesNoDialogDelegate = ProcessBGMlock;
		mLockBGM = true;
	}

	public void ProcessBGMlock(int theId)
	{
		mLockBGM = false;
		if (theId == 1000)
		{
			GameApp.gApp.mMusicInterface.stopUserMusic();
		}
	}
}
