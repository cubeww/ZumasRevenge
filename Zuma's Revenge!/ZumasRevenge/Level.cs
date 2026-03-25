using System;
using System.Collections.Generic;
using System.Linq;
using JeffLib;
using SexyFramework;
using SexyFramework.AELib;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Resource;

namespace ZumasRevenge;

public class Level : IDisposable
{
	public enum TorchState
	{
		TorchState_FlyIn,
		TorchState_Bounce,
		TorchState_TossEgg,
		TorchState_Disappear,
		TorchState_RaiseDais,
		TorchState_FrogFlyIn,
		TorchState_IntroDone,
		TorchState_ShakeDais,
		TorchState_FrogDisappear,
		TorchState_DoFade,
		TorchState_DropInToNextLevel,
		TorchState_CloakedBossAppear,
		TorchState_CloakedBossTransform,
		TorchState_Complete
	}

	public const int TARGET_BAR_SIZE = 330;

	public const int FRED_TONGUE_X = 541;

	public const int STARTING_TORCH_TEXT_ALPHA = 700;

	protected float[] mCloakedBossTextAlpha = new float[3];

	public List<DaisRock> mDaisRocks = new List<DaisRock>();

	public List<TorchLevelEgg> mEggs = new List<TorchLevelEgg>();

	public List<Wall> mMovingWallDefaults = new List<Wall>();

	public List<Effect> mEffects = new List<Effect>();

	protected bool mAllCurvesAtRolloutPoint;

	protected bool mHasReachedCruisingSpeed;

	protected float mCurGauntletMultPct;

	protected Transform mGlobalTranform = new Transform();

	private CompositionMgr mTorchCompMgr;

	public float mTorchBossX;

	public float mTorchBossY;

	public float mTorchBossDestX;

	public float mTorchBossDestY;

	public float mTorchBossVX;

	public float mTorchBossVY;

	public float mTorchDaisScale;

	public int mChallengePoints;

	public int mChallengeAcePoints;

	public int mCloakClapFrame;

	public PIEffect mCloakPoof;

	public FrogFlyOff mFrogFlyOff;

	public List<PowerupRegion> mPowerupRegions = new List<PowerupRegion>();

	public List<Torch> mTorches = new List<Torch>();

	public List<string> mEffectNames = new List<string>();

	public List<EffectParams> mEffectParams = new List<EffectParams>();

	public List<TreasurePoint> mTreasurePoints = new List<TreasurePoint>();

	public string mId = "";

	public string mDisplayName = "";

	public int mDisplayNameId = -1;

	public string mPopupText = "";

	public string mImagePath = "";

	public string mSoundscapeId = "";

	public MirrorType mMirrorType;

	public CurveMgr[] mCurveMgr = new CurveMgr[4];

	public float[] mCurveSkullAngleOverrides = new float[4];

	public HoleMgr mHoleMgr;

	public List<TunnelData> mTunnelData = new List<TunnelData>();

	public List<Wall> mWalls = new List<Wall>();

	public Boss mBoss;

	public Boss mSecondaryBoss;

	public Boss mOrgBoss;

	public Gun mFrog;

	public Board mBoard;

	public GameApp mApp;

	public SharedImageRef mBossIntroBG;

	public string mPreviewText = "";

	public int mPreviewTextId = -1;

	public LillyPadImageInfo[] mFrogImages = new LillyPadImageInfo[5];

	public bool mCanDrawBoss;

	public int mTorchStageState;

	public int mTorchStageTimer;

	public float mTorchStageAlpha;

	public int mTorchStageShakeAmt;

	public int mEndSequence;

	public int mIndex;

	public bool mOffscreenClearBonus;

	public bool mNoBackground;

	public bool mFinalLevel;

	public bool mBGFromPSD;

	public float mPotPct;

	public float mFireSpeed;

	public float mHurryToRolloutAmt;

	public bool mDoTorchCrap;

	public bool mHasDoneTorchCrap;

	public float mTorchTextAlpha;

	public bool mDrawCurves;

	public bool mSuckMode;

	public bool mIsEndless;

	public bool mLoopAtEnd;

	public bool mDoingPadHints;

	public bool mNoFlip;

	public bool mSliderEdgeRotate;

	public bool mIronFrog;

	public int mReloadDelay;

	public int mNumCurves;

	public int mNumFrogPoints;

	public int mCurFrogPoint;

	public int[] mFrogX = new int[5];

	public int[] mFrogY = new int[5];

	public int mBarWidth;

	public int mBarHeight;

	public int mTreasureFreq;

	public int mParTime;

	public int mMoveType;

	public int mMoveSpeed;

	public int mUpdateCount;

	public int mTimer;

	public int mTimeToComplete;

	public int mInvertMouseTimer;

	public int mMaxInvertMouseTimer;

	public int mTempSpeedupTimer;

	public int mBossFreezePowerupTime;

	public int mFrogShieldPowerupCount;

	public int mStartingGauntletLevel;

	public int mTorchTimer;

	public int mFurthestBallDistance;

	public int mIntroTorchDelay;

	public int mIntroTorchIndex;

	public int mGauntletCurTime;

	public int mGauntletMultipliersEarned;

	public int mNumGauntletBallsBroke;

	public int mGauntletCurNumForMult;

	public int mCurMultiplierTimeLeft;

	public int mMaxMultiplierTime;

	public float mGauntletTimeRedAmt;

	public int mZone;

	public int mNum;

	public int mPostZumaTimeCounter;

	public float mPostZumaTimeSpeedInc;

	public float mPostZumaTimeSlowInc;

	public string mBossBGID = "";

	public int m_OriginX = -1;

	public int m_OriginY = -1;

	public bool m_canGetAchievementNoMove;

	public bool m_canGetAchievementNoJump;

	public bool mHaveReachedTarget;

	public int mCurBarSize;

	public int mCurBarSizeInc;

	public int mTargetBarSize;

	public int mZumaBallFrame;

	public float mBarLightness;

	public int mZumaPulseUCStart;

	public float mGingerMouthX;

	public float mGingerMouthVX;

	public float mGingerMouthXStart;

	public float mFredMouthX;

	public float mFredMouthVX;

	public float mFredMouthXStart;

	public float mFredTongueX;

	public float mFredTongueVX;

	public float mZumaBallPct;

	public int mZumaBarState;

	public float mGoldBallXOff;

	public int mBarXOffset;

	public int mZumaBarX;

	public int mZumaBarWidth;

	private static int last_sound_idx;

	private static bool torchChangeState;

	protected void InitFinalBossLevel()
	{
		if (!mApp.mResourceManager.IsGroupLoaded("CloakedBoss") && !mApp.mResourceManager.LoadResources("CloakedBoss"))
		{
			mApp.ShowResourceError(doExit: true);
			mApp.Shutdown();
		}
		mDoTorchCrap = true;
		mBoard.mPreventBallAdvancement = true;
		mTorchTextAlpha = 700f;
		mTorchStageState = 0;
		mTorchStageTimer = Common._M(150);
		mTorchDaisScale = 1f;
		mTorchCompMgr = mApp.LoadComposition("pax\\cloakedboss", "_BOSSES");
		Composition composition = mTorchCompMgr.GetComposition("squish");
		mTorchBossX = -Common._DS(composition.mWidth) - Common._DS(Common._M(500));
		mTorchBossY = Common._DS(Common._M(-920));
		mTorchBossDestX = Common._DS(Common._M(-520));
		mTorchBossDestY = Common._DS(Common._M(-462));
		int num = Common._M(50);
		mTorchBossVX = (mTorchBossDestX - mTorchBossX) / (float)num;
		mTorchBossVY = (mTorchBossDestY - mTorchBossY) / (float)num;
		for (int i = 0; i < mTorches.Count; i++)
		{
			mTorches[i].mActive = (mTorches[i].mDraw = true);
		}
		for (int j = 0; j < 3; j++)
		{
			mCloakedBossTextAlpha[j] = 0f;
		}
	}

	public Level()
	{
		mCurMultiplierTimeLeft = (mMaxMultiplierTime = 0);
		mTorchStageState = -1;
		mTorchBossX = (mTorchBossY = -1f);
		mTorchDaisScale = 1f;
		mTorchCompMgr = null;
		mTorchStageTimer = 0;
		mTorchBossVX = (mTorchBossVY = (mTorchBossDestX = (mTorchBossDestY = 0f)));
		mFrogFlyOff = null;
		mTorchStageShakeAmt = 0;
		mNumGauntletBallsBroke = 0;
		mBossBGID = "";
		mZumaPulseUCStart = 0;
		mCurGauntletMultPct = 0f;
		mChallengePoints = 100;
		mChallengeAcePoints = 1000;
		mTorchStageAlpha = 0f;
		mGauntletCurTime = 0;
		mCloakPoof = null;
		mCloakClapFrame = -1;
		mCanDrawBoss = true;
		mIndex = -1;
		mIronFrog = false;
		mStartingGauntletLevel = 1;
		mAllCurvesAtRolloutPoint = false;
		mHasReachedCruisingSpeed = false;
		mPotPct = 1f;
		mFrog = null;
		mUpdateCount = 0;
		mFireSpeed = 8f;
		mBGFromPSD = false;
		mCurBarSizeInc = 1;
		mEndSequence = -1;
		mDoTorchCrap = false;
		mHasDoneTorchCrap = false;
		mTorchTextAlpha = 0f;
		mReloadDelay = 0;
		mTreasureFreq = 300;
		mParTime = 0;
		mBoss = (mOrgBoss = null);
		mSecondaryBoss = null;
		for (int i = 0; i < 4; i++)
		{
			mCurveSkullAngleOverrides[i] = float.MaxValue;
		}
		mLoopAtEnd = false;
		mIsEndless = false;
		mInvertMouseTimer = (mMaxInvertMouseTimer = 0);
		mTimer = (mTimeToComplete = -1);
		mBossFreezePowerupTime = (mFrogShieldPowerupCount = 300);
		mSliderEdgeRotate = false;
		mTorchTimer = 0;
		mFinalLevel = false;
		mNoBackground = false;
		mFurthestBallDistance = 0;
		mOffscreenClearBonus = false;
		mIntroTorchDelay = 0;
		mIntroTorchIndex = -1;
		for (int j = 0; j < 5; j++)
		{
			mFrogImages[j] = new LillyPadImageInfo();
			mFrogImages[j].mImage = null;
		}
		mPostZumaTimeCounter = 0;
		mPostZumaTimeSlowInc = (mPostZumaTimeSpeedInc = 0f);
		mZone = (mNum = -1);
		mApp = GameApp.gApp;
		mBoard = ((mApp != null) ? mApp.GetBoard() : null);
		mHurryToRolloutAmt = 0f;
		mTempSpeedupTimer = 0;
		mSuckMode = false;
		mMoveType = 0;
		mMoveSpeed = 25;
		mNumFrogPoints = 0;
		mCurFrogPoint = 0;
		mFrogX[0] = 320;
		mFrogY[0] = 240;
		mDoingPadHints = false;
		mBarWidth = (mBarHeight = 0);
		mNoFlip = false;
		mHoleMgr = new HoleMgr();
		mDrawCurves = false;
		for (int k = 0; k < 4; k++)
		{
			mCurveMgr[k] = null;
		}
		mNumCurves = 0;
		mMirrorType = MirrorType.MirrorType_None;
		if (mApp != null)
		{
			mGingerMouthXStart = mApp.GetWideScreenAdjusted(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GUI_INGAME_UI_LEFT_JAW)));
			mFredMouthXStart = mApp.GetWideScreenAdjusted(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GUI_INGAME_UI_RIGHT_JAW)));
		}
		mZumaBarX = 344;
		mZumaBarWidth = int.MaxValue;
		Reset();
	}

	public virtual Level Clone()
	{
		Level level = (Level)MemberwiseClone();
		level.mCurveMgr = new CurveMgr[4];
		level.mCloakedBossTextAlpha = (float[])mCloakedBossTextAlpha.Clone();
		level.mDaisRocks = new List<DaisRock>();
		if (mDaisRocks != null)
		{
			level.mDaisRocks.AddRange(mDaisRocks.ToArray());
		}
		level.mEggs = new List<TorchLevelEgg>();
		if (mEggs != null)
		{
			level.mEggs.AddRange(mEggs.ToArray());
		}
		level.mMovingWallDefaults = new List<Wall>();
		if (mMovingWallDefaults != null)
		{
			level.mMovingWallDefaults.AddRange(mMovingWallDefaults.ToArray());
		}
		level.mEffects = new List<Effect>();
		if (mEffects != null)
		{
			level.mEffects.AddRange(mEffects.ToArray());
		}
		level.mCloakPoof = mCloakPoof;
		level.mFrogFlyOff = mFrogFlyOff;
		level.mPowerupRegions = new List<PowerupRegion>();
		if (mPowerupRegions != null)
		{
			level.mPowerupRegions.AddRange(mPowerupRegions.ToArray());
		}
		level.mTorches = new List<Torch>();
		if (mTorches != null)
		{
			level.mTorches.AddRange(mTorches.ToArray());
		}
		level.mEffectNames = new List<string>();
		if (mEffectNames != null)
		{
			level.mEffectNames.AddRange(mEffectNames.ToArray());
		}
		level.mEffectParams = new List<EffectParams>();
		if (mEffectParams != null)
		{
			level.mEffectParams.AddRange(mEffectParams.ToArray());
		}
		level.mTreasurePoints = new List<TreasurePoint>();
		if (mTreasurePoints != null)
		{
			level.mTreasurePoints.AddRange(mTreasurePoints.ToArray());
		}
		level.mCurveMgr = (CurveMgr[])mCurveMgr.Clone();
		level.mCurveSkullAngleOverrides = (float[])mCurveSkullAngleOverrides.Clone();
		level.mHoleMgr = mHoleMgr;
		level.mTunnelData = new List<TunnelData>();
		if (mTunnelData != null)
		{
			level.mTunnelData.AddRange(mTunnelData.ToArray());
		}
		level.mWalls = new List<Wall>();
		if (mWalls != null)
		{
			level.mWalls.AddRange(mWalls.ToArray());
		}
		level.mFrogImages = (LillyPadImageInfo[])mFrogImages.Clone();
		level.mFrogX = (int[])mFrogX.Clone();
		level.mFrogY = (int[])mFrogY.Clone();
		return level;
	}

	public virtual void Dispose()
	{
		if (mCloakPoof != null)
		{
			mCloakPoof.Dispose();
			mCloakPoof = null;
		}
		if (mFrogFlyOff != null)
		{
			mFrogFlyOff.Dispose();
			mFrogFlyOff = null;
		}
		if (mTorchCompMgr != null)
		{
			mTorchCompMgr = null;
		}
		for (int i = 0; i < 4; i++)
		{
			if (mCurveMgr[i] != null)
			{
				mCurveMgr[i].Dispose();
				mCurveMgr[i] = null;
			}
		}
		for (int j = 0; j < 5; j++)
		{
			if (mFrogImages[j].mFilename.Length > 0)
			{
				if (mFrogImages[j].mImage != null)
				{
					mFrogImages[j].mImage.Dispose();
				}
			}
			else
			{
				mApp.mResourceManager.DeleteImage(mFrogImages[j].mResId);
			}
		}
		if (mHoleMgr != null)
		{
			mHoleMgr = null;
		}
		if (mOrgBoss != null && mOrgBoss.mResGroup.Length > 0 && !SexyFramework.Common.StrEquals(mOrgBoss.mResGroup, "Boss6Common") && GameApp.gApp.mResourceManager.IsGroupLoaded(mOrgBoss.mResGroup))
		{
			GameApp.gApp.mResourceManager.DeleteResources(mOrgBoss.mResGroup);
		}
		if (mSecondaryBoss != null && mSecondaryBoss.mResGroup.Length > 0 && !SexyFramework.Common.StrEquals(mOrgBoss.mResGroup, "Boss6Common") && GameApp.gApp.mResourceManager.IsGroupLoaded(mSecondaryBoss.mResGroup))
		{
			GameApp.gApp.mResourceManager.DeleteResources(mSecondaryBoss.mResGroup);
		}
		if (mBossBGID != "")
		{
			BaseRes baseRes = GameApp.gApp.mResourceManager.GetBaseRes(0, mBossBGID);
			string text = baseRes.mCompositeResGroup;
			if (text.Length == 0)
			{
				text = baseRes.mResGroup;
			}
			if (text.Length > 0 && GameApp.gApp.mResourceManager.IsGroupLoaded(text))
			{
				GameApp.gApp.mResourceManager.DeleteResources(text);
			}
		}
		if (mOrgBoss != null)
		{
			mOrgBoss.Dispose();
			mOrgBoss = null;
		}
		if (mSecondaryBoss != null)
		{
			mSecondaryBoss.Dispose();
			mSecondaryBoss = null;
		}
	}

	public virtual int GetNumCurves()
	{
		return mNumCurves;
	}

	public virtual int GetGunPointFromPos(int x, int y)
	{
		for (int i = 0; i < mNumFrogPoints; i++)
		{
			int num = x - mFrogX[i];
			int num2 = y - mFrogY[i];
			if (num * num + num2 * num2 < 3136)
			{
				return i;
			}
		}
		return -1;
	}

	public virtual void Preload()
	{
		if (mZone == 6 && (IsFinalBossLevel() || mEndSequence != -1) && IsFinalBossLevel())
		{
			mBossIntroBG = mApp.mResourceManager.GetResourceRef(0, "IMAGE_BOSS6_INTRO_BG").GetSharedImageRef();
			mBossBGID = "IMAGE_BOSS6_INTRO_BG";
		}
		if (mBoss != null && mZone != 6)
		{
			mBossIntroBG = mApp.mResourceManager.GetResourceRef(0, mBoss.mResPrefix + "INTRO_BG").GetSharedImageRef();
			mBossBGID = mBoss.mResPrefix + "INTRO_BG";
		}
		if (!(mBossBGID != ""))
		{
			return;
		}
		BaseRes baseRes = mApp.mResourceManager.GetBaseRes(0, mBossBGID);
		string text = baseRes.mCompositeResGroup;
		if (text != "")
		{
			text = baseRes.mResGroup;
		}
		if (text != "" && !mApp.mResourceManager.IsGroupLoaded(text))
		{
			mApp.mResourceManager.LoadResources(text);
			if (!mApp.mResourceManager.LoadResources(text))
			{
				mApp.ShowResourceError(doExit: true);
			}
		}
	}

	public virtual void StartLevel(bool from_load, bool needs_reinit)
	{
		new Stopwatch("Level::StartLevel");
		Preload();
		if (mZone == 5 && !mApp.mResourceManager.IsGroupLoaded("GrottoSounds") && !mApp.mResourceManager.LoadResources("GrottoSounds"))
		{
			mApp.ShowResourceError(doExit: true);
			mApp.Shutdown();
			return;
		}
		if (mZone != 5 && mApp.mResourceManager.IsGroupLoaded("GrottoSounds"))
		{
			mApp.mResourceManager.DeleteResources("GrottoSounds");
		}
		else if (mZone != 6 && mApp.mResourceManager.IsGroupLoaded("Boss6Common"))
		{
			mApp.mResourceManager.DeleteResources("Boss6Common");
		}
		if (!needs_reinit)
		{
			new Stopwatch("Level::StartLevel::GetImage - FrogImages");
			for (int i = 0; i < 5; i++)
			{
				if (mFrogImages[i].mFilename.Length != 0)
				{
					string pathFrom = SexyFramework.Common.GetPathFrom(mFrogImages[i].mFilename, "");
					string idByPath = GameApp.gApp.mResourceManager.GetIdByPath(pathFrom);
					mFrogImages[i].mImage = (DeviceImage)mApp.mResourceManager.LoadImage(idByPath).GetImage();
					mFrogImages[i].mImage.mNumCols = 2;
				}
				else if (mFrogImages[i].mResId.Length != 0)
				{
					mFrogImages[i].mImage = (DeviceImage)mApp.mResourceManager.LoadImage(mFrogImages[i].mResId).GetImage();
					if (mFrogImages[i].mImage != null)
					{
						mFrogImages[i].mImage.mNumCols = 2;
					}
				}
			}
		}
		new Stopwatch("Level::StartLevel::LoadCurve");
		for (int j = 0; j < mNumCurves; j++)
		{
			if (!mCurveMgr[j].mIsLoaded && !mCurveMgr[j].LoadCurve())
			{
				mApp.Popup("Unable to load curve for " + mCurveMgr[j].GetPath());
			}
			if (mBoard.GauntletMode())
			{
				mCurveMgr[j].mCurveDesc.mVals.mNumColors = GameApp.gDDS.GetNumGauntletBalls(mNumCurves);
			}
			mCurveMgr[j].StartLevel(from_load);
			if (j == 0)
			{
				mCurveMgr[j].mInitialPathHilite = true;
			}
		}
		for (int k = 0; k < mHoleMgr.GetNumHoles(); k++)
		{
			for (int l = 0; l < 4; l++)
			{
				if (mHoleMgr.GetHole(k).mCurveNum == l)
				{
					if (mCurveSkullAngleOverrides[l] < float.MaxValue)
					{
						mHoleMgr.GetHole(k).mRotation = mCurveSkullAngleOverrides[l];
					}
					break;
				}
			}
		}
		Common.gAddBalls = false;
		if (!needs_reinit)
		{
			mEffects.Clear();
			InitEffects();
			if (IsFinalBossLevel() && !mHasDoneTorchCrap && !mDoTorchCrap)
			{
				InitFinalBossLevel();
			}
			else if (mEndSequence == 3)
			{
				mBoard.mPreventBallAdvancement = false;
			}
			ResetEffects();
		}
		mPostZumaTimeCounter = mApp.GetLevelMgr().mPostZumaTime;
		mPostZumaTimeSlowInc = 0f;
		mPostZumaTimeSpeedInc = 0f;
		mTimer = mTimeToComplete;
		if (mBoss != null)
		{
			mBoss.Init(this);
			if (mSecondaryBoss != null)
			{
				mSecondaryBoss.Init(this);
			}
			if (!needs_reinit && mEndSequence == 2 && mBoard.GetGameState() == GameState.GameState_Playing)
			{
				mCloakClapFrame = -1;
				mCanDrawBoss = false;
				Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_LAME_CLOAKEDBOSS_ARMDOWN_REST);
				mTorchBossY = -imageByID.mHeight - Common._DS(Common._M(100));
				mCloakPoof = mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_CLOAKTOLAMEEXPLOSION01").Duplicate();
				Common.SetFXNumScale(mCloakPoof, GameApp.gApp.Is3DAccelerated() ? 1f : Common._M(0.15f));
				for (int m = 0; m < 3; m++)
				{
					mCloakedBossTextAlpha[m] = 0f;
				}
			}
			if (GameApp.gDDS.HasBossParam("HurryAmt"))
			{
				mHurryToRolloutAmt = GameApp.gDDS.GetBossParam("HurryAmt");
			}
			mGingerMouthX = mGingerMouthXStart + 20f;
			mFredMouthX = mFredMouthXStart - 20f;
			mFredTongueX = 505f;
			mTargetBarSize = 330;
			mCurBarSize = 0;
		}
		else if (mTimeToComplete > 0)
		{
			mGingerMouthX = mGingerMouthXStart + 20f;
			mFredMouthX = mFredMouthXStart - 20f;
			mFredTongueX = 505f;
			mTargetBarSize = 330;
			mCurBarSize = 0;
		}
		if (IsFinalBossLevel())
		{
			if (!mApp.mResourceManager.IsGroupLoaded("Bosses") && !mApp.mResourceManager.LoadResources("Bosses"))
			{
				mApp.ShowResourceError(doExit: true);
				mApp.Shutdown();
				return;
			}
			mIntroTorchDelay = 0;
			mIntroTorchIndex = -1;
		}
		if (!needs_reinit)
		{
			for (int n = 0; n < mEffects.size(); n++)
			{
				mEffects[n].LevelStarted(from_load);
			}
		}
		if (mBoard.GauntletMode())
		{
			mGauntletCurNumForMult = mApp.GetLevelMgr().mGauntletNumForMultBase;
		}
	}

	public virtual void StartLevel()
	{
		StartLevel(from_load: false, needs_reinit: false);
	}

	public string GetCurvePath(int curve_num)
	{
		string text = "levels/";
		if (mCurveMgr[curve_num].mCurveDesc.mPath.IndexOf('/') != -1 || mCurveMgr[curve_num].mCurveDesc.mPath.IndexOf('\\') != -1)
		{
			return text + mCurveMgr[curve_num].mCurveDesc.mPath;
		}
		return text + mId + "/" + mCurveMgr[curve_num].mCurveDesc.mPath;
	}

	public bool CanDrawFrog()
	{
		if (IsFinalBossLevel() && mTorchStageState != 6)
		{
			return mTorchStageState > 10;
		}
		return true;
	}

	public virtual void Reset(bool reset_effects)
	{
		mGauntletTimeRedAmt = 0f;
		mCurMultiplierTimeLeft = (mMaxMultiplierTime = 0);
		mGauntletCurNumForMult = 0;
		mGauntletCurTime = 0;
		mAllCurvesAtRolloutPoint = false;
		mHasReachedCruisingSpeed = false;
		mZumaBallPct = 0f;
		mZumaBallFrame = 0;
		mTargetBarSize = 0;
		mCurBarSize = 0;
		mBarLightness = 0f;
		mHaveReachedTarget = false;
		mNumGauntletBallsBroke = 0;
		mCurGauntletMultPct = 0f;
		mGauntletMultipliersEarned = 0;
		mGingerMouthX = mGingerMouthXStart;
		mFredMouthX = mFredMouthXStart;
		mGingerMouthVX = 0.5f;
		mFredMouthVX = 0f;
		mFredTongueX = 541f;
		mFredTongueVX = 0f;
		mZumaBallPct = 0f;
		mZumaBarState = -1;
		mFurthestBallDistance = 0;
		mGoldBallXOff = 0f;
		for (int i = 0; i < mNumCurves; i++)
		{
			mCurveMgr[i].Reset();
		}
		if (mApp != null && reset_effects && ((mBoard != null && !mBoard.GauntletMode()) || (mBoard == null && mApp.mLoadingThreadStarted && !mApp.mLoadingThreadCompleted)))
		{
			for (int j = 0; j < mEffects.Count(); j++)
			{
				mEffects[j].NukeParams();
			}
			for (int k = 0; k < mEffectParams.Count(); k++)
			{
				mEffects[mEffectParams[k].mEffectIndex].SetParams(mEffectParams[k].mKey, mEffectParams[k].mValue);
			}
			for (int l = 0; l < mEffects.Count(); l++)
			{
				mEffects[l].Reset(mId);
			}
		}
	}

	public virtual void Reset()
	{
		Reset(reset_effects: true);
	}

	public virtual void ReInit()
	{
		for (int i = 0; i < mNumCurves; i++)
		{
			mCurveMgr[i].SetFarthestBall(0);
		}
		mPotPct = 1f;
		mCurBarSizeInc = 1;
		mInvertMouseTimer = (mMaxInvertMouseTimer = 0);
		mTimer = (mTimeToComplete = -1);
		mFurthestBallDistance = 0;
		mOffscreenClearBonus = false;
		mPostZumaTimeCounter = 0;
		mPostZumaTimeSlowInc = (mPostZumaTimeSpeedInc = 0f);
		mTempSpeedupTimer = 0;
		Boss boss = null;
		if (mOrgBoss != null)
		{
			int x = mBoss.GetX();
			int y = mBoss.GetY();
			Boss boss2 = mApp.GetLevelMgr().GetLevelById(mId).mBoss;
			boss = boss2.Instantiate();
			boss.mName = mDisplayName;
			boss.PostInstantiationHook(boss2);
			boss.mLevel = this;
			mOrgBoss = null;
			mBoss = boss;
			mBoss.SetXY(x, y);
			mOrgBoss = mBoss;
		}
		if (mSecondaryBoss != null)
		{
			Boss boss3 = mApp.GetLevelMgr().GetLevelById(mId).mSecondaryBoss;
			boss = boss3.Instantiate();
			boss.mName = mDisplayName;
			boss.PostInstantiationHook(boss3);
			boss.mLevel = this;
			mSecondaryBoss = null;
			mSecondaryBoss = boss;
		}
		for (int j = 0; j < mTorches.size(); j++)
		{
			if (mTorches[j].mFlame != null)
			{
				mTorches[j].mFlame.ResetAnim();
				mTorches[j].mFlame.mEmitAfterTimeline = true;
			}
			mTorches[j].mActive = true;
			mTorches[j].mDraw = true;
			mTorches[j].mWasHit = false;
		}
		Reset(reset_effects: false);
		for (int k = 0; k < mNumCurves; k++)
		{
			mCurveMgr[k].Reset();
		}
	}

	public virtual void AfterBoardAdded()
	{
	}

	public virtual bool CollidedWithWall(Bullet b)
	{
		float num = (float)b.GetRadius() * Common._M(0.75f);
		FRect theTRect = new FRect(b.GetX() - num, b.GetY() - num, num * 2f, num * 2f);
		for (int i = 0; i < mWalls.size(); i++)
		{
			Wall wall = mWalls[i];
			if (wall.mStrength == 0 || wall.mType == 0)
			{
				continue;
			}
			int num2 = ((wall.mImage == null) ? ((int)wall.mWidth) : wall.mImage.GetCelWidth());
			int num3 = ((wall.mImage == null) ? ((int)wall.mHeight) : wall.mImage.GetCelHeight());
			int num4 = ((wall.mImage != null) ? (num2 / 2) : 0);
			int num5 = ((wall.mImage != null) ? (num3 / 2) : 0);
			FRect fRect = new FRect(wall.mX - (float)num4, wall.mY - (float)num5, num2, num3);
			if (!fRect.Intersects(theTRect))
			{
				continue;
			}
			if (wall.mStrength > 0)
			{
				wall.mStrength--;
			}
			if (wall.mStrength == 0)
			{
				wall.mCurRespawnTimer = 0;
			}
			fRect.Inflate(theTRect.mWidth / 2f, theTRect.mHeight / 2f);
			FPoint a = new FPoint(b.GetX() + b.mVelX, b.GetY() + b.mVelY);
			FPoint a2 = new FPoint(b.GetX() - b.mVelX, b.GetY() - b.mVelY);
			float num6 = 0f;
			if (Common.LinesIntersect(a, a2, new FPoint(fRect.mX, fRect.mY), new FPoint(fRect.mX + fRect.mWidth, fRect.mY)))
			{
				num6 = 90f;
			}
			else if (Common.LinesIntersect(a, a2, new FPoint(fRect.mX, fRect.mY + fRect.mHeight), new FPoint(fRect.mX + fRect.mWidth, fRect.mY + fRect.mHeight)))
			{
				num6 = 270f;
			}
			else if (Common.LinesIntersect(a, a2, new FPoint(fRect.mX, fRect.mY), new FPoint(fRect.mX, fRect.mY + fRect.mHeight)))
			{
				num6 = 180f;
			}
			else
			{
				if (!Common.LinesIntersect(a, a2, new FPoint(fRect.mX + fRect.mWidth, fRect.mY), new FPoint(fRect.mX + fRect.mWidth, fRect.mY + fRect.mHeight)))
				{
					return true;
				}
				num6 = 0f;
			}
			mBoard.AddBallExplosionParticleEffect(b, num6, 180f);
			GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_WALLBALL));
			return true;
		}
		Rect r = new Rect((int)theTRect.mX, (int)theTRect.mY, (int)theTRect.mWidth, (int)theTRect.mHeight);
		for (int j = 0; j < mTorches.size(); j++)
		{
			Torch torch = mTorches[j];
			torch.CheckCollision(r);
		}
		return false;
	}

	public virtual void CopyEffectsFrom(Level l)
	{
		for (int i = 0; i < mEffects.size(); i++)
		{
			for (int j = 0; j < l.mEffects.size(); j++)
			{
				if (Common.StrEquals(l.mEffects[j].GetName(), mEffects[i].GetName()))
				{
					mEffects[i].CopyFrom(l.mEffects[j]);
					break;
				}
			}
		}
	}

	public virtual string GetStatsScreenText(GameStats stats, int score)
	{
		return "";
	}

	public void AddTorch(int x, int y, int w, int h)
	{
		Torch torch = new Torch();
		torch.mX = x;
		torch.mY = y;
		torch.mWidth = w;
		torch.mHeight = h;
		mTorches.Add(torch);
	}

	public bool PointIntersectsWall(float x, float y)
	{
		if (mWalls.Count() == 0)
		{
			return false;
		}
		for (int i = 0; i < mWalls.Count(); i++)
		{
			Wall wall = mWalls[i];
			Rect rect = new Rect((int)wall.mX, (int)wall.mY, (int)wall.mWidth, (int)wall.mHeight);
			if (wall.mStrength != 0 && rect.Contains((int)x, (int)y))
			{
				return true;
			}
		}
		return false;
	}

	public void DrawDaisRocks(Graphics g)
	{
		for (int i = 0; i < mDaisRocks.size(); i++)
		{
			DaisRock daisRock = mDaisRocks[i];
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)daisRock.mAlpha);
			mGlobalTranform.Reset();
			mGlobalTranform.Scale(daisRock.mSize, daisRock.mSize);
			float rot = (255f - daisRock.mAlpha) / 255f * Common._M(2.5f) * (float)Math.PI;
			mGlobalTranform.RotateRad(rot);
			g.DrawImageTransform(daisRock.mImg, mGlobalTranform, daisRock.mX, daisRock.mY);
		}
	}

	public void FadeInkSpots()
	{
		for (int i = 0; i < mNumCurves; i++)
		{
			mCurveMgr[i].QuicklyFadeInkSpots();
		}
	}

	public void MultiplierActivated()
	{
		mGauntletCurNumForMult += mApp.GetLevelMgr().mGauntletNumForMultInc;
		if (mGauntletCurNumForMult > mApp.GetLevelMgr().mMaxGauntletNumForMult)
		{
			mGauntletCurNumForMult = mApp.GetLevelMgr().mMaxGauntletNumForMult;
		}
		int mMultiplierDuration = mApp.GetLevelMgr().mMultiplierDuration;
		if (mCurMultiplierTimeLeft == 0)
		{
			mCurMultiplierTimeLeft = (mMaxMultiplierTime = mMultiplierDuration);
		}
		else
		{
			mCurMultiplierTimeLeft += mMultiplierDuration;
			mMaxMultiplierTime = mCurMultiplierTimeLeft;
		}
		if (GameApp.gDDS.AddMultiplierTime(mApp.GetLevelMgr().mMultiplierTimeAdd))
		{
			UpdateChallengeModeDifficulty();
		}
	}

	public void UpdateChallengeModeDifficulty()
	{
		for (int i = 0; i < mNumCurves; i++)
		{
			mCurveMgr[i].mCurveDesc.mVals.mNumColors = GameApp.gDDS.GetNumGauntletBalls(mNumCurves);
		}
	}

	public virtual void SkipInitialPathHilite()
	{
		bool flag = false;
		for (int i = 0; i < mNumCurves; i++)
		{
			if (mCurveMgr[i].mSparkles.Count() > 0 || mCurveMgr[i].mInitialPathHilite)
			{
				flag = true;
				mCurveMgr[i].mSparkles.Clear();
				mCurveMgr[i].mInitialPathHilite = false;
			}
		}
		if (flag)
		{
			Common.gAddBalls = true;
		}
	}

	public virtual bool DoingInitialPathHilite()
	{
		for (int i = 0; i < mNumCurves; i++)
		{
			if (mCurveMgr[i].mInitialPathHilite)
			{
				return true;
			}
		}
		return false;
	}

	public virtual void SwitchToSecondaryBoss()
	{
		int x = mBoss.GetX();
		int y = mBoss.GetY();
		float hP = mBoss.GetHP();
		mBoss = mSecondaryBoss;
		mBoss.SetHP(hP);
		mBoss.SetXY(x, y);
	}

	public virtual void Update(float f)
	{
		mUpdateCount++;
		if (mTimer > 0)
		{
			mTimer--;
		}
		if (mInvertMouseTimer > 0 && --mInvertMouseTimer == 0)
		{
			GameApp.gApp.GetBoard().UpdateGunPos();
		}
		if (mTorchStageState == 10 || mTorchStageState == 11 || (mTorchStageState == 9 && mBoard.mFullScreenAlphaRate < 0))
		{
			if (mTorchStageState != 11 && mUpdateCount % Common._M(2) == 0)
			{
				List<Image> list = new List<Image>();
				for (int i = 0; i < 3; i++)
				{
					list.Add(Res.GetImageByID((ResID)(825 + i)));
					list.Add(Res.GetImageByID((ResID)(828 + i)));
				}
				mDaisRocks.Add(new DaisRock());
				DaisRock daisRock = mDaisRocks.back();
				daisRock.mImg = list[SexyFramework.Common.Rand(list.size())];
				daisRock.mX = Common._DS(MathUtils.IntRange(Common._M(400), Common._M1(1200)));
				daisRock.mY = -daisRock.mImg.mHeight / 2;
			}
			for (int j = 0; j < mDaisRocks.size(); j++)
			{
				DaisRock daisRock2 = mDaisRocks[j];
				daisRock2.mY += Common._M(15f);
				daisRock2.mSize -= Common._M(0.002f);
				daisRock2.mAlpha -= Common._M(0.1f);
				if (daisRock2.mSize <= 0f || daisRock2.mAlpha <= 0f)
				{
					mDaisRocks.RemoveAt(j);
					j--;
				}
			}
		}
		if ((IsFinalBossLevel() && mTorchStageState != 6) || mTorchStageState >= 10)
		{
			string[] array = new string[3] { "start", "squish", "rattle" };
			Composition composition = null;
			int num = -1;
			num = mTorchStageState switch
			{
				0 => 0, 
				1 => 1, 
				_ => 2, 
			};
			if (mTorchStageState < 6)
			{
				composition = mTorchCompMgr.GetComposition(array[num]);
			}
			float num2 = Common._M(0.97f);
			int num3 = Common._M(15);
			if (mTorchStageState == 0)
			{
				if (--mTorchStageTimer <= 0)
				{
					composition.Update();
					mTorchBossX += mTorchBossVX;
					mTorchBossY += mTorchBossVY;
					if (mTorchBossX >= mTorchBossDestX)
					{
						mTorchBossX = mTorchBossDestX;
						mTorchBossVX = 0f;
					}
					if (mTorchBossY >= mTorchBossDestY)
					{
						mTorchBossY = mTorchBossDestY;
						mTorchBossVY = 0f;
					}
					if (mTorchBossVX == 0f && mTorchBossVY == 0f)
					{
						mTorchStageState = 1;
						mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_CLOAKED_DAIS_LANDING));
					}
				}
			}
			else if (mTorchStageState == 1)
			{
				composition.Update();
				if (composition.mUpdateCount == num3)
				{
					mTorchDaisScale = num2;
				}
				float num4 = (1f - num2) / (float)(composition.GetMaxDuration() - num3);
				mTorchDaisScale += num4;
				if (mTorchDaisScale > 1f)
				{
					mTorchDaisScale = 1f;
				}
				if (composition.mUpdateCount >= composition.GetMaxDuration() && SexyFramework.Common._eq(mTorchDaisScale, 1f))
				{
					mTorchStageState = 2;
					mTorchStageTimer = Common._M(100);
				}
			}
			else if (mTorchStageState == 2)
			{
				int num5 = Common._M(100);
				if (mTorchStageTimer > 0)
				{
					mTorchStageTimer--;
				}
				if (mTorchStageTimer == 0 && mEggs.size() < 4)
				{
					composition.Update();
				}
				float[] array2 = new float[4] { 38f, 38f, 1421f, 1423f };
				float[] array3 = new float[4] { 82f, 952f, 85f, 949f };
				if (composition.mUpdateCount >= composition.GetMaxDuration() && mTorchStageTimer <= 0 && mEggs.size() < 4)
				{
					mTorchStageTimer = num5;
					composition.Reset();
					mEggs.Add(new TorchLevelEgg());
					TorchLevelEgg torchLevelEgg = mEggs.back();
					torchLevelEgg.mX = Common._DS(Common._M(545));
					torchLevelEgg.mY = Common._DS(Common._M(208));
					torchLevelEgg.mAlpha = 0f;
					torchLevelEgg.mDestX = Common._DS(array2[mEggs.Count - 1]);
					torchLevelEgg.mDestY = Common._DS(array3[mEggs.Count - 1] + (float)Common._M(60));
					float num6 = Common._M(60f);
					torchLevelEgg.mVX = (torchLevelEgg.mDestX - torchLevelEgg.mX) / num6;
					torchLevelEgg.mVY = (torchLevelEgg.mDestY - torchLevelEgg.mY) / num6;
					torchLevelEgg.mDestAngle = (float)Math.PI * Common._M(3f);
					if (torchLevelEgg.mDestX > torchLevelEgg.mX)
					{
						torchLevelEgg.mDestAngle *= -1f;
					}
					torchLevelEgg.mAngleInc = torchLevelEgg.mDestAngle / num6;
				}
				for (int k = 0; k < mEggs.size(); k++)
				{
					TorchLevelEgg torchLevelEgg2 = mEggs[k];
					if (torchLevelEgg2.mAlpha < 255f && (torchLevelEgg2.mVX != 0f || torchLevelEgg2.mVY != 0f))
					{
						torchLevelEgg2.mAlpha += Common._M(8);
						if (torchLevelEgg2.mAlpha > 255f)
						{
							torchLevelEgg2.mAlpha = 255f;
						}
					}
					torchLevelEgg2.mX += torchLevelEgg2.mVX;
					torchLevelEgg2.mY += torchLevelEgg2.mVY;
					int num7 = 0;
					if ((torchLevelEgg2.mVX < 0f && torchLevelEgg2.mX <= torchLevelEgg2.mDestX) || (torchLevelEgg2.mVX > 0f && torchLevelEgg2.mX >= torchLevelEgg2.mDestX))
					{
						num7++;
					}
					if ((torchLevelEgg2.mVY < 0f && torchLevelEgg2.mY <= torchLevelEgg2.mDestY) || (torchLevelEgg2.mVY > 0f && torchLevelEgg2.mY >= torchLevelEgg2.mDestY))
					{
						num7++;
					}
					if (num7 == 2)
					{
						if (mTorches[k].mActive)
						{
							mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_TORCH_EXTINGUISHED));
						}
						mTorches[k].mActive = false;
					}
					torchLevelEgg2.mAngle += torchLevelEgg2.mAngleInc;
				}
				Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSSES_EGG);
				if (mEggs.size() == 4 && !new Rect(-80, 0, mApp.mWidth + Common._S(160), mApp.mHeight).Intersects(new Rect((int)mEggs.Last().mX, (int)mEggs.Last().mY, imageByID.mWidth, imageByID.mHeight)))
				{
					mTorchStageState = 3;
					mTorchStageTimer = Common._M(50);
				}
			}
			else if (mTorchStageState == 7 || (mTorchStageState == 9 && mBoard.mFullScreenAlphaRate < 0))
			{
				mBoard.UpdatePlayingFX();
				List<Image> list2 = new List<Image>();
				for (int l = 0; l < 3; l++)
				{
					list2.Add(Res.GetImageByID((ResID)(825 + l)));
					list2.Add(Res.GetImageByID((ResID)(828 + l)));
				}
				mTorchStageAlpha += Common._M(1.5f);
				mTorchStageShakeAmt = SexyFramework.Common.Rand(Common._M(5));
				if (mUpdateCount % Common._M(10) == 0)
				{
					Image imageByID2 = Res.GetImageByID(ResID.IMAGE_LEVELS_BOSS6PART1_DIAS);
					mDaisRocks.Add(new DaisRock());
					DaisRock daisRock3 = mDaisRocks.back();
					float num8 = Common._DS(660) + Common._DS(Common._M(30));
					float num9 = num8 + (float)Common._DS(Common._M(40));
					float num10 = num8 + (float)imageByID2.mWidth - (float)Common._DS(Common._M(100));
					float num11 = num10 + (float)Common._DS(Common._M(35));
					float mY = Common._DS(417) + imageByID2.mHeight - Common._DS(Common._M(100));
					daisRock3.mImg = list2[SexyFramework.Common.Rand(list2.size())];
					float num12 = SexyFramework.Common.IntRange((int)num8, (int)num9) - daisRock3.mImg.mWidth / 2;
					float num13 = SexyFramework.Common.IntRange((int)num10, (int)num11) + daisRock3.mImg.mWidth / 2;
					daisRock3.mX = ((SexyFramework.Common.Rand(2) == 0) ? num12 : num13);
					daisRock3.mY = mY;
				}
				if (mUpdateCount % Common._M(50) == 0)
				{
					mApp.mSoundPlayer.Loop(Res.GetSoundByID(ResID.SOUND_NEW_DAIS_RUMBLE));
					if (++last_sound_idx >= 2)
					{
						last_sound_idx = 0;
					}
				}
				for (int m = 0; m < mDaisRocks.size(); m++)
				{
					DaisRock daisRock4 = mDaisRocks[m];
					daisRock4.mY += Common._M(1f);
					daisRock4.mSize -= Common._M(0.02f);
					daisRock4.mAlpha -= Common._M(1f);
					if (daisRock4.mSize <= 0f || daisRock4.mAlpha <= 0f)
					{
						mDaisRocks.RemoveAt(m);
						m--;
					}
				}
				if (mTorchStageAlpha >= 255f && --mTorchStageTimer <= 0)
				{
					mTorchStageAlpha = 255f;
					mTorchStageState = 8;
					mTorchStageShakeAmt = 0;
					mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_DAIS_LOWERING));
					mApp.SetCursor(ECURSOR.CURSOR_POINTER);
				}
			}
			else if (mTorchStageState == 3 || mTorchStageState == 8)
			{
				for (int n = 0; n < mDaisRocks.size(); n++)
				{
					DaisRock daisRock5 = mDaisRocks[n];
					daisRock5.mY += Common._M(1f);
					daisRock5.mSize -= Common._M(0.02f);
					daisRock5.mAlpha -= Common._M(1f);
					if (daisRock5.mSize <= 0f || daisRock5.mAlpha <= 0f)
					{
						mDaisRocks.RemoveAt(n);
						n--;
					}
				}
				if (mTorchStageState == 8 && mUpdateCount % Common._M(250) == 0)
				{
					mApp.mSoundPlayer.Loop(Res.GetSoundByID(ResID.SOUND_NEW_DAIS_RUMBLE));
				}
				if (mTorchStageTimer == 1 && mTorchStageState == 3)
				{
					mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_DAIS_LOWERING));
				}
				if (--mTorchStageTimer <= 0)
				{
					mTorchDaisScale -= Common._M(0.01f);
					if (mTorchDaisScale <= 0f)
					{
						mTorchDaisScale = 0f;
						if (mTorchStageState == 3)
						{
							mApp.mSoundPlayer.Stop(Res.GetSoundByID(ResID.SOUND_NEW_DAIS_RUMBLE));
							mTorchStageState = 4;
							mTorchStageTimer = Common._M(150);
						}
						else
						{
							mTorchStageState = 9;
						}
					}
				}
			}
			else if (mTorchStageState == 4)
			{
				if (--mTorchStageTimer <= 0)
				{
					if (mTorchStageTimer == 0)
					{
						for (int num14 = 0; num14 < mTorches.size(); num14++)
						{
							mTorches[num14].mFlame.ResetAnim();
							mTorches[num14].mActive = true;
							mTorches[num14].mDraw = true;
						}
					}
					mTorchDaisScale += Common._M(0.02f);
					if (mTorchDaisScale >= 1f)
					{
						mTorchDaisScale = 1f;
						if (mFrogFlyOff != null)
						{
							mFrogFlyOff.Dispose();
							mFrogFlyOff = null;
						}
						mFrogFlyOff = new FrogFlyOff();
						mFrogFlyOff.JumpIn(mFrog, mFrog.GetCenterX(), mFrog.GetCenterY(), continue_from_jump_out: false);
						mTorchStageState = 5;
					}
				}
			}
			else if (mTorchStageState == 10)
			{
				mFrogFlyOff.Update();
				if (mFrogFlyOff.mTimer >= mFrogFlyOff.mFrogJumpTime)
				{
					mApp.mSoundPlayer.Stop(Res.GetSoundByID(ResID.SOUND_NEW_DAIS_RUMBLE));
					mTorchStageState = 11;
					mTorchStageTimer = Common._M(100);
					mFrog.SetPos((int)mFrogFlyOff.mFrogX, mFrog.GetCurY());
					mFrogFlyOff.Dispose();
					mFrogFlyOff = null;
				}
			}
			else if (mTorchStageState == 5)
			{
				mFrogFlyOff.Update();
				if (mFrogFlyOff.mTimer > mFrogFlyOff.mFrogJumpTime)
				{
					mFrog.SetAngle((int)mFrogFlyOff.mFrogAngle);
					mFrogFlyOff.Dispose();
					mFrogFlyOff = null;
					mTorchStageState = 6;
					mBoard.mPreventBallAdvancement = false;
					mDoTorchCrap = false;
					mHasDoneTorchCrap = true;
				}
			}
			else if (mTorchStageState == 11)
			{
				if (--mTorchStageTimer <= 0 && (mTorchBossY += Common._M(10)) >= (float)Common._M1(0))
				{
					mTorchStageState = 12;
					mTorchStageTimer = 0;
				}
			}
			else if (mTorchStageState == 12)
			{
				int num15 = Common._M(500);
				mTorchStageTimer++;
				for (int num16 = 0; num16 < 3; num16++)
				{
					if (mTorchStageTimer >= num15)
					{
						mCloakedBossTextAlpha[num16] -= Common._M(2f);
						if (mCloakedBossTextAlpha[num16] < 0f)
						{
							mCloakedBossTextAlpha[num16] = 0f;
						}
						continue;
					}
					mCloakedBossTextAlpha[num16] += Common._M(2f);
					if (mCloakedBossTextAlpha[num16] > 255f)
					{
						mCloakedBossTextAlpha[num16] = 255f;
					}
					else if (mCloakedBossTextAlpha[num16] < (float)Common._M(128))
					{
						break;
					}
				}
				Image imageByID3 = Res.GetImageByID(ResID.IMAGE_BOSS_LAME_CLOAKEDBOSS_CLAP);
				int num17 = Common._M(6);
				int num18 = imageByID3.mNumRows * imageByID3.mNumCols;
				if (mTorchStageTimer >= num15 && mTorchStageTimer % num17 == 0)
				{
					mCloakClapFrame++;
					if (mCloakClapFrame == Common._M(5))
					{
						mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_CLOAKED_CLAP));
					}
				}
				if (mTorchStageTimer >= num15 + Common._M(15))
				{
					mCloakPoof.mDrawTransform.LoadIdentity();
					float num19 = GameApp.DownScaleNum(1f);
					mCloakPoof.mDrawTransform.Scale(num19, num19);
					mCloakPoof.mDrawTransform.Translate(Common._DS(Common._M(812)), Common._DS(Common._M1(220)));
					mCloakPoof.Update();
					if (SexyFramework.Common._eq(mCloakPoof.mFrameNum, Common._M(135), 0.5f))
					{
						mCanDrawBoss = true;
					}
					else if (mCloakPoof.mFrameNum >= (float)mCloakPoof.mLastFrameNum)
					{
						mBoard.mContinueNextLevelOnLoadProfile = false;
						mTorchStageState = 13;
						mBoard.mHasDoneIntroSounds = false;
						if (mApp.mResourceManager.IsGroupLoaded("CloakedBoss"))
						{
							mApp.mResourceManager.DeleteResources("CloakedBoss");
						}
					}
				}
			}
		}
		if (mTorchStageState > 4 && mBoard.GetGameState() != GameState.GameState_BossIntro && mTorchStageState != 12 && mTorchTextAlpha > 0f)
		{
			mTorchTextAlpha -= Common._M(1.3f);
			if (mTorchTextAlpha < 0f)
			{
				mTorchTextAlpha = 0f;
			}
		}
		UpdateEffects();
		for (int num20 = 0; num20 < mWalls.size(); num20++)
		{
			Wall wall = mWalls[num20];
			wall.Update();
			if ((wall.mVX > 0f && wall.mX > (float)Common._SS(mApp.mWidth)) || (wall.mVX < 0f && wall.mX + wall.mWidth < 0f) || (wall.mVY > 0f && wall.mY > (float)Common._SS(mApp.mHeight)) || (wall.mVY < 0f && wall.mY + wall.mHeight < 0f))
			{
				mWalls.RemoveAt(num20);
				num20--;
			}
		}
		for (int num21 = 0; num21 < mMovingWallDefaults.size(); num21++)
		{
			Wall wall2 = mMovingWallDefaults[num21];
			int num22 = int.MaxValue;
			bool flag = false;
			for (int num23 = 0; num23 < mWalls.size(); num23++)
			{
				Wall wall3 = mWalls[num23];
				if (wall3.mId == wall2.mId)
				{
					flag = true;
					int num24 = ((!(wall3.mVX > 0f)) ? ((int)((wall3.mX + wall3.mWidth > wall2.mX) ? 0f : (wall2.mX - (wall3.mX + wall3.mWidth)))) : ((int)((wall3.mX < 0f) ? 0f : (wall3.mX - wall2.mX))));
					int num25 = ((!(wall3.mVY > 0f)) ? ((int)((wall3.mY + wall3.mHeight > wall2.mY) ? 0f : (wall2.mY - (wall3.mY + wall3.mHeight)))) : ((int)((wall3.mY < 0f) ? 0f : (wall3.mY - wall2.mY))));
					int num26 = num24 * num24 + num25 * num25;
					if (num26 < num22)
					{
						num22 = num26;
					}
				}
			}
			if (num22 > wall2.mSpacing || !flag)
			{
				mWalls.Add(wall2);
				mWalls.back().mCurLifeTimer = MathUtils.IntRange(wall2.mMinLifeTimer, wall2.mMaxLifeTimer);
			}
		}
		mHoleMgr.Update();
		if (mBoss != null)
		{
			mBoss.Update(f);
		}
	}

	public virtual void Draw(Graphics g)
	{
		for (int i = 0; i < mNumCurves; i++)
		{
			mCurveMgr[i].DrawUnderBalls(g);
		}
		for (int j = 0; j < mTorches.size(); j++)
		{
			mTorches[j].Draw(g);
		}
		for (int k = 0; k < mEffects.size(); k++)
		{
			mEffects[k].DrawUnderBalls(g);
		}
		if (mBoss != null && mCanDrawBoss)
		{
			mBoss.DrawBelowBalls(g);
		}
	}

	public virtual void DrawBottomLevel(Graphics g)
	{
		if (mBoss != null && mCanDrawBoss)
		{
			mBoss.DrawBottomLevel(g);
		}
	}

	public virtual void DrawToplevel(Graphics g)
	{
		if (mBoss != null && mCanDrawBoss)
		{
			mBoss.DrawTopLevel(g);
		}
		for (int i = 0; i < mNumCurves; i++)
		{
			mCurveMgr[i].DrawTopLevel(g);
		}
		if (mTorchTextAlpha > 0f && mBoard.GetGameState() != GameState.GameState_BossIntro && mTorchStageState > 5)
		{
			int centerX = mBoard.GetGun().GetCenterX();
			int centerY = mBoard.GetGun().GetCenterY();
			string theString = (mBoard.IsHardAdventureMode() ? TextManager.getInstance().getString(495) : TextManager.getInstance().getString(496));
			string theString2 = (mBoard.IsHardAdventureMode() ? TextManager.getInstance().getString(497) : TextManager.getInstance().getString(498));
			g.SetFont(Res.GetFontByID(ResID.FONT_BOSS_TAUNT));
			int num = (int)mTorchTextAlpha - 350;
			if (num > 0)
			{
				g.SetColor(0, 0, 0, (num > 255) ? 255 : num);
				g.DrawString(theString, Common._S(centerX) - g.GetFont().StringWidth(theString) / 2, Common._S(centerY - Common._M(90)));
			}
			g.SetColor(0, 0, 0, ((int)mTorchTextAlpha > 255) ? 255 : ((int)mTorchTextAlpha));
			g.DrawString(theString2, Common._S(centerX) - g.GetFont().StringWidth(theString2) / 2, Common._S(centerY + Common._M(120)));
		}
		for (int j = 0; j < mPowerupRegions.size(); j++)
		{
			PowerupRegion powerupRegion = mPowerupRegions[j];
			if (powerupRegion.mDebugDraw)
			{
				g.SetColor(255, 0, 0);
				int numPoints = mCurveMgr[powerupRegion.mCurveNum].mWayPointMgr.GetNumPoints();
				mCurveMgr[powerupRegion.mCurveNum].GetXYFromWaypoint((int)(powerupRegion.mCurvePctStart * (float)numPoints), out var x, out var y);
				mCurveMgr[powerupRegion.mCurveNum].GetXYFromWaypoint((int)(powerupRegion.mCurvePctEnd * (float)numPoints), out var x2, out var y2);
				g.FillRect(Common._S((int)x) - 2, Common._S((int)y) - 2, 4, 4);
				g.SetColor(0, 255, 0);
				g.FillRect(Common._S((int)x2) - 2, Common._S((int)y2) - 2, 4, 4);
			}
		}
		if (mTorchStageState < 11)
		{
			return;
		}
		if (mTorchStageTimer > 0 || (mCloakPoof != null && mCloakPoof.mFrameNum < (float)Common._M(135)))
		{
			int num2 = Common._DS(Common._M(32));
			if (mTorchStageTimer < Common._M(570))
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, 128);
				g.SetColorizeImages(colorizeImages: false);
			}
			Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_LAME_CLOAKEDBOSS_ARMDOWN_REST);
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BOSS_LAME_CLOAKEDBOSS_CLAP);
			if (mCloakClapFrame < 0)
			{
				g.DrawImage(imageByID, Common._S(mBoss.GetX()) - imageByID.mWidth / 2 + Common._DS(Res.GetOffsetXByID(ResID.IMAGE_BOSS_LAME_CLOAKEDBOSS_ARMDOWN_REST)) - num2, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_BOSS_LAME_CLOAKEDBOSS_ARMDOWN_REST)) + Common._S(mBoss.GetY()) + (int)mTorchBossY - imageByID.mHeight / 2);
			}
			else if (mTorchStageTimer < Common._M(570))
			{
				int theCel = Math.Min(mCloakClapFrame, imageByID2.mNumRows * imageByID2.mNumCols - 1);
				g.DrawImageCel(imageByID2, Common._S(mBoss.GetX()) - imageByID2.GetCelWidth() / 2 - num2, Common._S(mBoss.GetY()) - imageByID2.GetCelHeight() / 2 + (int)mTorchBossY, theCel);
			}
			g.SetFont(Res.GetFontByID(ResID.FONT_BOSS_TAUNT));
			bool flag = mBoard.IsHardAdventureMode();
			if (mTorchStageState == 12)
			{
				string[] array = new string[3]
				{
					TextManager.getInstance().getString(490),
					TextManager.getInstance().getString(491),
					TextManager.getInstance().getString(492)
				};
				if (flag)
				{
					array[0] = TextManager.getInstance().getString(493);
					array[1] = TextManager.getInstance().getString(494);
					array[2] = "";
				}
				for (int k = 0; k < array.Length; k++)
				{
					if (!(mCloakedBossTextAlpha[k] <= 0f))
					{
						g.SetColor(0, 0, 0, (int)mCloakedBossTextAlpha[k]);
						g.WriteString(array[k].ToString(), -GameApp.gApp.mBoardOffsetX, Common._DS(Common._M(550)) + k * g.GetFont().GetHeight(), 1024);
					}
				}
			}
		}
		if (mTorchStageState == 12 && mCloakPoof.mFrameNum < (float)mCloakPoof.mLastFrameNum && mCloakPoof.mFrameNum > 0f)
		{
			mCloakPoof.Draw(g);
		}
	}

	public virtual void DrawAboveBalls(Graphics g)
	{
		for (int i = 0; i < mEffects.size(); i++)
		{
			mEffects[i].DrawAboveBalls(g);
		}
	}

	public virtual void DrawUnderBackground(Graphics g)
	{
		for (int i = 0; i < mEffects.size(); i++)
		{
			mEffects[i].DrawUnderBackground(g);
		}
	}

	public virtual void DrawFullScene(Graphics g)
	{
		for (int i = 0; i < mEffects.size(); i++)
		{
			mEffects[i].DrawFullScene(g);
		}
	}

	public virtual void DrawFullSceneNoFrog(Graphics g)
	{
		for (int i = 0; i < mEffects.size(); i++)
		{
			mEffects[i].DrawFullSceneNoFrog(g);
		}
	}

	public virtual void DrawPriority(Graphics g, int priority)
	{
		for (int i = 0; i < mEffects.size(); i++)
		{
			mEffects[i].DrawPriority(g, priority);
		}
	}

	public virtual void DrawTorchLighting(Graphics g)
	{
		if (mTorches.size() == 0)
		{
			return;
		}
		Image imageByID = Res.GetImageByID(ResID.IMAGE_LEVELS_BOSS6PART1_QUADRANT);
		float[] array = new float[4] { 1f, 1f, -1f, -1f };
		float[] array2 = new float[4] { 1f, -1f, 1f, -1f };
		int[] array3 = new int[4]
		{
			Common._DS(-160),
			Common._DS(-160),
			mApp.mWidth + Common._DS(320) - imageByID.mWidth,
			mApp.mWidth + Common._DS(320) - imageByID.mWidth
		};
		int[] array4 = new int[4]
		{
			0,
			mApp.mHeight - imageByID.mHeight,
			0,
			mApp.mHeight - imageByID.mHeight
		};
		for (int i = 0; i < mTorches.size(); i++)
		{
			int mOverlayAlpha = mTorches[i].mOverlayAlpha;
			if (mOverlayAlpha != 0)
			{
				if (mOverlayAlpha != 255)
				{
					g.SetColorizeImages(colorizeImages: true);
				}
				g.SetColor(255, 255, 255, mOverlayAlpha);
				mGlobalTranform.Reset();
				mGlobalTranform.Scale(array[i], array2[i]);
				g.DrawImageTransform(imageByID, mGlobalTranform, array3[i] + imageByID.mWidth / 2, array4[i] + imageByID.mHeight / 2);
				g.SetColorizeImages(colorizeImages: false);
			}
		}
	}

	public virtual void DrawSkullPit(Graphics g)
	{
		bool flag = false;
		for (int i = 0; i < mEffects.size(); i++)
		{
			if (mEffects[i].DrawSkullPit(g, mHoleMgr))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			mHoleMgr.DrawRings(g);
			mHoleMgr.Draw(g);
		}
	}

	public virtual void DrawTunnel(Graphics g, Image img, int x, int y, int w, int h)
	{
		if (mNum != int.MaxValue || mZone != 4 || mBoss == null)
		{
			for (int i = 0; i < mEffects.size(); i++)
			{
				if (!mEffects[i].DrawTunnel(g, img, x, y))
				{
					return;
				}
			}
		}
		g.DrawImage(img, x, y, w, h);
	}

	public void DrawGauntletUI(Graphics g)
	{
		Common._S(Common._M(465));
		Common._S(Common._M(22));
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GAUNTLET_MAIN_BAR_BONUS_OFF);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_GAUNTLET_MAIN_BAR_BONUS_ON);
		Image imageByID3 = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_CHALLENGE_UI_TIMER_FRAME);
		Image imageByID4 = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_CHALLENGE_UI_GAUNTLETFRAMECENTER);
		Image imageByID5 = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_CHALLENGE_UI_GAUNTLETFRAMELEFT);
		g.DrawImage(imageByID, GameApp.gApp.GetWideScreenAdjusted(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GAUNTLET_MAIN_BAR_BONUS_OFF))) - Common._S(27), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_GAUNTLET_MAIN_BAR_BONUS_OFF)) + Common._S(7));
		g.DrawImage(imageByID2, GameApp.gApp.GetWideScreenAdjusted(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GAUNTLET_MAIN_BAR_BONUS_ON))) - Common._S(27), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_GAUNTLET_MAIN_BAR_BONUS_ON)) + Common._S(7), new Rect(0, 0, (int)((float)imageByID2.mWidth * mCurGauntletMultPct), imageByID2.mHeight));
		g.DrawImage(imageByID3, GameApp.gApp.GetWideScreenAdjusted(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GUI_INGAME_CHALLENGE_UI_TIMER_FRAME))), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_GUI_INGAME_CHALLENGE_UI_TIMER_FRAME)));
		int num = (int)mGauntletTimeRedAmt;
		if (mGauntletTimeRedAmt > 0f)
		{
			g.SetColorizeImages(colorizeImages: true);
			num = ((num <= 128) ? (num * 2) : ((255 - num) * 2));
			if (num > 255)
			{
				num = 255;
			}
			else if (num < 0)
			{
				num = 0;
			}
		}
		int num2 = mApp.GetLevelMgr().mGauntletSessionLength - mGauntletCurTime;
		if (num2 < 0)
		{
			num2 = 0;
		}
		Font fontByID = Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_STROKE);
		g.SetFont(fontByID);
		g.SetColor(192, 230, 99);
		int theY = Common._DS(Common._M(93)) + (Common._DS(Common._M1(35)) - g.GetFont().mHeight) / 2;
		g.WriteString(JeffLib.Common.UpdateToTimeStr(num2), GameApp.gApp.GetWideScreenAdjusted(Common._DS(Common._M(225))), theY, Common._DS(Common._M1(141)), 0);
		if (num > 0)
		{
			g.SetColor(255, 0, 0, num);
			g.WriteString(JeffLib.Common.UpdateToTimeStr(num2), GameApp.gApp.GetWideScreenAdjusted(Common._DS(Common._M(225))), theY, Common._DS(Common._M1(141)), 0);
		}
		g.SetColorizeImages(colorizeImages: false);
		int wideScreenAdjusted = GameApp.gApp.GetWideScreenAdjusted(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GUI_INGAME_CHALLENGE_UI_GAUNTLETFRAMECENTER)));
		int theY2 = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_GUI_INGAME_CHALLENGE_UI_GAUNTLETFRAMECENTER));
		int wideScreenAdjusted2 = GameApp.gApp.GetWideScreenAdjusted(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GUI_INGAME_CHALLENGE_UI_GAUNTLETFRAMELEFT)));
		g.DrawImage(imageByID4, wideScreenAdjusted, theY2);
		g.DrawImage(imageByID5, wideScreenAdjusted2, Common._S(0));
		g.DrawImageMirror(imageByID4, wideScreenAdjusted + imageByID4.GetWidth() + Common._S(60), theY2);
	}

	public void InitEffects(Level copy_effects_from)
	{
		for (int i = 0; i < mEffectNames.size(); i++)
		{
			Effect effect = mApp.GetLevelMgr().mEffectManager.GetEffect(mEffectNames[i], mId, copy_effects_from);
			if (effect != null)
			{
				mEffects.Add(effect);
			}
			mEffects[i].NukeParams();
		}
	}

	public void InitEffects()
	{
		InitEffects(null);
	}

	public void ResetEffects()
	{
		for (int i = 0; i < mEffects.size(); i++)
		{
			mEffects[i].LoadResources();
			mEffects[i].Reset(mId);
		}
	}

	public void ForceTreasure(int tnum)
	{
		mBoard.mCurTreasureNum = tnum;
		mBoard.mCurTreasure = mTreasurePoints[tnum];
		mBoard.mMinTreasureY = (mBoard.mMaxTreasureY = float.MaxValue);
	}

	public Ball GetBallById(int id)
	{
		for (int i = 0; i < mNumCurves; i++)
		{
			foreach (Ball mBall in mCurveMgr[i].mBallList)
			{
				if (mBall.GetId() == id)
				{
					return mBall;
				}
			}
			foreach (Ball mPendingBall in mCurveMgr[i].mPendingBalls)
			{
				if (mPendingBall.GetId() == id)
				{
					return mPendingBall;
				}
			}
		}
		return null;
	}

	public bool AllTorchesOut()
	{
		if (mTorchStageState != 6)
		{
			return false;
		}
		int num = 0;
		for (int i = 0; i < mTorches.Count(); i++)
		{
			if (!mTorches[i].mActive)
			{
				num++;
			}
		}
		if (num == mTorches.Count())
		{
			return mTorches.Count() > 0;
		}
		return false;
	}

	public bool IsFinalBossLevel()
	{
		return mTorches.Count() > 0;
	}

	public virtual void UpdateUI()
	{
		if (mZumaBarState >= Common._M(2) && !mBoard.GauntletMode() && mCurBarSize < mTargetBarSize)
		{
			mCurBarSize++;
		}
		if (mZumaBarState == 0)
		{
			mGingerMouthX += mGingerMouthVX;
			int num = (int)mGingerMouthXStart + Common._S(15);
			if (mGingerMouthX >= (float)num)
			{
				mGingerMouthX = num;
				mZumaBarState++;
				mGingerMouthVX = 0f;
			}
		}
		else if (mZumaBarState == 1)
		{
			mGoldBallXOff += Common._S(0.75f);
			if ((mZumaBallPct += 0.05f) >= 1.2f)
			{
				mZumaBallPct = 1.2f;
				mZumaBarState++;
			}
		}
		else if (mZumaBarState == 2)
		{
			if ((mZumaBallPct -= 0.05f) <= 1f)
			{
				mZumaBallPct = 1f;
			}
			if (mGingerMouthVX == 0f && mZumaBallPct <= 1f)
			{
				mZumaBarState++;
			}
		}
		else if (mZumaBarState == 4)
		{
			mFredMouthX += mFredMouthVX;
			int num2 = Common._S(15);
			if (mFredMouthX <= mFredMouthXStart - (float)num2)
			{
				mFredMouthX = mFredMouthXStart - (float)num2;
				mFredMouthVX *= -1f;
				mZumaBarState++;
				mFredTongueVX = Common._S(-2.5f);
			}
		}
		else if (mZumaBarState == 5)
		{
			mFredTongueX += mFredTongueVX;
			int num3 = Common._S(36);
			if (mFredTongueX <= (float)(541 - num3))
			{
				mFredTongueX = 541 - num3;
				mFredTongueVX *= -1f;
				mZumaBarState++;
			}
		}
		else if (mZumaBarState == 6)
		{
			mFredTongueX += mFredTongueVX;
			mGoldBallXOff += Common._S(2.5f);
			if ((mZumaBallPct += 0.05f) >= 1.2f)
			{
				mZumaBallPct = 1.2f;
				mZumaBarState++;
			}
		}
		else if (mZumaBarState == 7)
		{
			mFredTongueX += mFredTongueVX;
			if ((mZumaBallPct -= 0.05f) <= 1f)
			{
				mZumaBallPct = 1f;
			}
			mGoldBallXOff += Common._S(0.75f);
			if (mFredTongueX >= 541f)
			{
				mFredTongueX = 541f;
				mFredTongueVX = 0f;
			}
			if (mFredTongueX >= 541f)
			{
				mZumaBarState++;
				int num4 = (int)Common._S(2.5f);
				mFredMouthVX = num4;
				mGingerMouthVX = -num4;
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BAR_FULL));
			}
		}
		else if (mZumaBarState >= 8 && mZumaBarState < 12)
		{
			mFredMouthX += mFredMouthVX;
			mGingerMouthX += mGingerMouthVX;
			int num5 = 0;
			if (mFredMouthX >= mFredMouthXStart && mFredMouthVX > 0f)
			{
				num5++;
				mFredMouthX = mFredMouthXStart;
			}
			else if (mFredMouthX <= mFredMouthXStart - (float)Common._S(15) && mFredMouthVX < 0f)
			{
				num5++;
				mFredMouthX = mFredMouthXStart - (float)Common._S(15);
			}
			if (mGingerMouthX <= mGingerMouthXStart && mGingerMouthVX < 0f)
			{
				mGingerMouthX = mGingerMouthXStart;
				num5++;
			}
			else if (mGingerMouthX >= mGingerMouthXStart + (float)Common._S(15) && mGingerMouthVX > 0f)
			{
				mGingerMouthX = mGingerMouthXStart + (float)Common._S(15);
				num5++;
			}
			if (num5 == 2)
			{
				mZumaBarState++;
				mFredMouthVX *= -1f;
				mGingerMouthVX *= -1f;
			}
		}
		else if (mZumaBarState == 12)
		{
			if ((mBarLightness += 18f) >= 255f)
			{
				mBarLightness = 255f;
				mZumaBarState++;
			}
		}
		else if (mZumaBarState == 13)
		{
			if ((mBarLightness -= 18f) <= 0f)
			{
				mBarLightness = 0f;
				mZumaBarState++;
				mZumaPulseUCStart = mUpdateCount;
			}
		}
		else if (mZumaBarState == 14 && mBoard.GauntletMode())
		{
			mCurBarSize -= 2;
			if (mCurBarSize <= 0)
			{
				Reset();
				mCurBarSize = 0;
			}
		}
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_GOLD_BALL);
		if (mCurBarSize != mTargetBarSize)
		{
			mZumaBallFrame = (mZumaBallFrame + 1) % imageByID.mNumCols;
		}
		if (!mHaveReachedTarget && !mBoard.GauntletMode() && ShouldUpdateZumaBar() && mNumCurves > 0 && mCurBarSize == 330 && mBoard.mScore >= mBoard.mScoreTarget && mBoss == null)
		{
			mZumaBarState = 4;
			mFredMouthVX = Common._S(-2.5f);
			if (!mBoard.IsEndless())
			{
				mHaveReachedTarget = true;
				for (int i = 0; i < mNumCurves; i++)
				{
					mCurveMgr[i].ZumaAchieved(stop: true);
					if (!mBoard.DestroyAll())
					{
						mCurveMgr[i].DetonateBalls();
					}
				}
				mApp.mUserProfile.GetAdvModeVars().mNumZumasCurLevel++;
				mBoard.mNumZumaBalls = GetTotalBallsOnLevel();
			}
			if (!mBoard.DestroyAll())
			{
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_EXPLODE));
			}
		}
		int num6 = mBoard.mScoreTarget - mBoard.GetLevelBeginScore();
		if (num6 > 0)
		{
			int num7 = mBoard.mScoreTarget - mBoard.mScore;
			if (num7 < 0)
			{
				num7 = 0;
				if (mBoard.mLevelEndFrame == 0)
				{
					if (mBoard.GetNumBallColors() <= 2)
					{
						mBoard.mLevelEndFrame = mBoard.GetStateCount();
					}
				}
				else if (mBoard.GetStateCount() - mBoard.mLevelEndFrame == 3000)
				{
					for (int j = 0; j < mNumCurves; j++)
					{
						mCurveMgr[j].mCurveDesc.mVals.mPowerUpFreq[0] = 500;
						mCurveMgr[j].mCurveDesc.mVals.mPowerUpFreq[1] = 0;
						mCurveMgr[j].mCurveDesc.mVals.mPowerUpFreq[2] = 0;
						mCurveMgr[j].mCurveDesc.mVals.mPowerUpFreq[3] = 0;
						mCurveMgr[j].mCurveDesc.mVals.mAccelerationRate = 0.0003f;
					}
				}
			}
			if (mBoss == null && !mBoard.GauntletMode())
			{
				mTargetBarSize = 330 - 330 * num7 / num6;
			}
		}
		if (mBoss != null)
		{
			mTargetBarSize = (int)(330f - (1f - mBoss.GetHP() / 100f) * 330f);
		}
		if (mBoard.GauntletMode() && !DoingInitialPathHilite())
		{
			if (mGauntletCurTime < mApp.GetLevelMgr().mGauntletSessionLength)
			{
				mGauntletCurTime++;
				if (mGauntletCurTime % 100 == 0 && mApp.GetLevelMgr().mGauntletSessionLength - mGauntletCurTime <= 1100)
				{
					mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_CHALLENGE_COUNTDOWN));
				}
				if (GameApp.gDDS.SetGauntletTime(mGauntletCurTime))
				{
					UpdateChallengeModeDifficulty();
				}
				int num8 = mApp.GetLevelMgr().mGauntletSessionLength - mGauntletCurTime;
				if (num8 <= 1100 && num8 % 100 == 0)
				{
					mGauntletTimeRedAmt = 255f;
				}
			}
			if (mGauntletCurTime >= mApp.GetLevelMgr().mGauntletSessionLength && CurvesAtRest())
			{
				mBoard.EndGauntletMode(hit_max_time: true);
				bool theAcedLevel = false;
				if (mBoard.mScore > mChallengeAcePoints)
				{
					theAcedLevel = true;
				}
				GameApp.gApp.ReportEndOfLevelMetrics(mBoard, theLevelSuccess: true, theAcedLevel);
			}
		}
		if (mGauntletTimeRedAmt > 0f)
		{
			mGauntletTimeRedAmt -= Common._M(2.6f);
		}
	}

	public virtual void UpdatePlaying()
	{
		if (mCurMultiplierTimeLeft > 0 && --mCurMultiplierTimeLeft == 0)
		{
			mBoard.GauntletMultiplierEnded();
		}
		if (mDoingPadHints)
		{
			if (mBoard.mZumaTips.size() != 0)
			{
				return;
			}
			mDoingPadHints = false;
		}
		for (int i = 0; i < mTorches.size(); i++)
		{
			mTorches[i].Update();
			if (!mTorches[i].mActive)
			{
				mTorches[i].mOverlayAlpha += Common._M(2);
				if (mTorches[i].mOverlayAlpha > 255)
				{
					mTorches[i].mOverlayAlpha = 255;
				}
			}
			else
			{
				mTorches[i].mOverlayAlpha -= Common._M(2);
				if (mTorches[i].mOverlayAlpha < 0)
				{
					mTorches[i].mOverlayAlpha = 0;
				}
			}
		}
		if (mBoard.GauntletMode())
		{
			float num = (float)mNumGauntletBallsBroke / (float)mGauntletCurNumForMult;
			float num2 = num - mCurGauntletMultPct;
			float num3 = mCurGauntletMultPct;
			if (mCurGauntletMultPct < num || num2 < -0.001f)
			{
				mCurGauntletMultPct += Common._M(0.01f);
				if (num3 < num && mCurGauntletMultPct > num)
				{
					mCurGauntletMultPct = num;
				}
				else if (mCurGauntletMultPct > 1f)
				{
					mCurGauntletMultPct = 0f;
				}
			}
		}
		bool flag = mHasReachedCruisingSpeed;
		mHasReachedCruisingSpeed = true;
		mAllCurvesAtRolloutPoint = true;
		Common._M(20f);
		bool flag2 = false;
		if (!IsFinalBossLevel() || mTorchStageState == 6)
		{
			for (int j = 0; j < mNumCurves; j++)
			{
				if (mCurveMgr[j].UpdatePlaying() && j + 1 < mNumCurves)
				{
					mCurveMgr[j + 1].mInitialPathHilite = true;
				}
				if (mCurveMgr[j].mSparkles.size() > 0)
				{
					flag2 = true;
				}
				if (!mCurveMgr[j].HasReachedCruisingSpeed())
				{
					mHasReachedCruisingSpeed = false;
				}
				if (!mCurveMgr[j].HasReachedRolloutPoint())
				{
					mAllCurvesAtRolloutPoint = false;
				}
				if (mTempSpeedupTimer == 1)
				{
					mCurveMgr[j].mOverrideSpeed = -1f;
				}
				int farthestBallPercent = mCurveMgr[j].GetFarthestBallPercent();
				if (farthestBallPercent > mFurthestBallDistance)
				{
					mFurthestBallDistance = farthestBallPercent;
				}
			}
		}
		if (!flag && mHasReachedCruisingSpeed)
		{
			mApp.mSoundPlayer.Fade((mZone == 5) ? Res.GetSoundByID(ResID.SOUND_UNDERWATER_ROLLOUT) : Res.GetSoundByID(ResID.SOUND_ROLLING));
		}
		if (!Common.gAddBalls && !flag2 && !mBoard.mPreventBallAdvancement)
		{
			Common.gAddBalls = true;
			for (int k = 0; k < mNumCurves; k++)
			{
				mCurveMgr[k].mInitialPathHilite = false;
			}
		}
		if (mTempSpeedupTimer > 0)
		{
			mTempSpeedupTimer--;
		}
		if (mBoard.HasAchievedZuma() && mPostZumaTimeCounter > 0)
		{
			mPostZumaTimeCounter--;
			float num4 = (float)(mApp.GetLevelMgr().mPostZumaTime - mPostZumaTimeCounter) / (float)mApp.GetLevelMgr().mPostZumaTime;
			mPostZumaTimeSlowInc = num4 * mApp.GetLevelMgr().mPostZumaTimeSlowInc;
			mPostZumaTimeSpeedInc = num4 * mApp.GetLevelMgr().mPostZumaTimeSlowInc;
		}
	}

	public virtual void UpdateBossIntro()
	{
		if (mBoss != null)
		{
			mBoss.Update();
		}
	}

	public virtual void DrawUI(Graphics g)
	{
		if (mBoss != null || IsFinalBossLevel())
		{
			g.mTransX = 0f;
			DrawBossUI(g);
			g.mTransX = mApp.mBoardOffsetX;
			return;
		}
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_WOOD);
		mBarXOffset = (int)((float)imageByID.mWidth * 0.05f);
		DrawWoodPanel(g);
		mBoard.DrawRollerScore(g);
		if (mBoard.GauntletMode())
		{
			DrawGauntletUI(g);
		}
		else
		{
			DrawScoreFrame(g);
			DrawZumaBar(g);
			DrawFredAndGinger(g);
		}
		DrawTikiEnds(g);
	}

	public virtual void DrawGunPoints(Graphics g)
	{
		if (mNumFrogPoints > 1)
		{
			for (int i = 0; i < mNumFrogPoints; i++)
			{
				if (mFrogImages[i].mImage != null)
				{
					int theCel = ((mBoard.mMouseOverGunPos == i) ? 1 : 0);
					g.DrawImageCel(mFrogImages[i].mImage, Common._S(mFrogX[i]) - mFrogImages[i].mImage.GetCelWidth() / 2 + GameApp.gScreenShakeX, Common._S(mFrogY[i]) - mFrogImages[i].mImage.GetCelHeight() / 2 + GameApp.gScreenShakeY, theCel);
				}
			}
		}
		float num = (1f - mTorchDaisScale) * (float)Common._DS(Common._M(189));
		if (!IsFinalBossLevel() && mTorchStageState < 10)
		{
			return;
		}
		for (int j = 0; j < mTorches.size(); j++)
		{
			mTorches[j].DrawAbove(g);
		}
		if (mTorchStageAlpha > 0f)
		{
			g.SetColor(0, 0, 0, (int)Math.Min(255f, mTorchStageAlpha));
			g.FillRect(Common._S(-80), 0, GameApp.gApp.mWidth + Common._S(160), GameApp.gApp.mHeight);
		}
		Image imageByID = Res.GetImageByID(ResID.IMAGE_LEVELS_BOSS6PART1_BASE);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_LEVELS_BOSS6PART1_DIAS);
		Image imageByID3 = Res.GetImageByID(ResID.IMAGE_LARGE_FROG);
		Image imageByID4 = Res.GetImageByID(ResID.IMAGE_FROG_SHADOW);
		if (IsFinalBossLevel())
		{
			g.DrawImage(imageByID, Common._DS(690 - mApp.mOffset160X), Common._DS(330));
		}
		string[] array = new string[3] { "start", "squish", "rattle" };
		int num2 = imageByID2.mWidth * (int)mTorchDaisScale;
		int num3 = imageByID2.mHeight * (int)mTorchDaisScale;
		int num4 = Common._DS(793 - mApp.mOffset160X);
		int num5 = Common._DS(395);
		num4 += (imageByID2.mWidth - num2) / 2;
		num5 += (imageByID2.mHeight - num3) / 2;
		if (mTorchStageState < 9)
		{
			g.DrawImage(imageByID2, num4 + Common._DS(mTorchStageShakeAmt), num5 - Common._DS(mTorchStageShakeAmt) + (int)num, num2, num3);
		}
		for (int k = 0; k < mDaisRocks.size(); k++)
		{
			DaisRock daisRock = mDaisRocks[k];
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)daisRock.mAlpha);
			mGlobalTranform.Reset();
			mGlobalTranform.Scale(daisRock.mSize, daisRock.mSize);
			float rot = (255f - daisRock.mAlpha) / 255f * Common._M(2.5f) * (float)Math.PI;
			mGlobalTranform.RotateRad(rot);
			g.DrawImageTransform(daisRock.mImg, mGlobalTranform, daisRock.mX, daisRock.mY);
			g.SetColorizeImages(colorizeImages: false);
		}
		if (mTorchStageState < 4)
		{
			Composition composition = null;
			int num6 = -1;
			num6 = mTorchStageState switch
			{
				0 => 2, 
				1 => 2, 
				_ => 2, 
			};
			composition = mTorchCompMgr.GetComposition(array[num6]);
			int num7 = composition.mUpdateCount;
			if (num7 >= composition.GetMaxDuration())
			{
				num7 = composition.GetMaxDuration() - 1;
			}
			if (mTorchStageState == 0)
			{
				num7 = 1;
			}
			if (num7 == 0)
			{
				num7 = 1;
			}
			CumulativeTransform cumulativeTransform = new CumulativeTransform();
			cumulativeTransform.mTrans.Translate(mTorchBossX, mTorchBossY);
			if (mTorchStageState == 3)
			{
				cumulativeTransform.mTrans.Scale(mTorchDaisScale, mTorchDaisScale);
				cumulativeTransform.mTrans.Translate(((float)Common._DS(composition.mWidth) - (float)Common._DS(composition.mWidth) * mTorchDaisScale) / 1.5f + (float)Common._DS(Common._M(80)) * (1f - mTorchDaisScale), ((float)Common._DS(composition.mHeight) - (float)Common._DS(composition.mHeight) * mTorchDaisScale) / Common._M1(1.5f) + num);
			}
			composition.Draw(g, cumulativeTransform, num7, Common._DS(1f));
			Image imageByID5 = Res.GetImageByID(ResID.IMAGE_BOSSES_EGG_ADD);
			Image imageByID6 = Res.GetImageByID(ResID.IMAGE_BOSSES_EGG);
			for (int l = 0; l < mEggs.size(); l++)
			{
				TorchLevelEgg torchLevelEgg = mEggs[l];
				int num8 = (int)(torchLevelEgg.mAlpha * mTorchDaisScale);
				if (num8 != 255)
				{
					g.SetColorizeImages(colorizeImages: true);
				}
				g.SetColor(255, 255, 255, num8);
				g.SetDrawMode(1);
				g.DrawImageRotated(imageByID5, (int)(torchLevelEgg.mX + (float)Common._DS(Common._M(-30))), (int)(torchLevelEgg.mY + (float)Common._DS(Common._M1(-30))), torchLevelEgg.mAngle);
				g.SetDrawMode(0);
				g.DrawImageRotated(imageByID6, (int)torchLevelEgg.mX, (int)torchLevelEgg.mY, torchLevelEgg.mAngle);
				g.SetColorizeImages(colorizeImages: false);
			}
		}
		else if (mTorchStageState == 8 || mTorchStageState == 7)
		{
			float num9 = mTorchDaisScale * Common._M(0.5f);
			SexyTransform2D sexyTransform2D = new SexyTransform2D(init: false);
			sexyTransform2D.Scale(mTorchDaisScale, mTorchDaisScale);
			sexyTransform2D.Translate(Common._S(Common._M(-2)), Common._S(Common._M1(3)));
			sexyTransform2D.RotateRad(mFrog.GetAngle());
			sexyTransform2D.Translate(Common._S(Common._M(2)), Common._S(Common._M1(-3)));
			float num10 = Common._DS(Common._M(20f)) * (1f - mTorchDaisScale);
			g.DrawImageMatrix(imageByID4, sexyTransform2D, imageByID4.GetCelRect(1), (float)Common._S(mFrog.GetCurX()) + (float)Common._DS(Common._M(-2)) * (1f - mTorchDaisScale) + (float)Common._DS(mTorchStageShakeAmt), (float)Common._S(mFrog.GetCurY()) + num10 + num - (float)Common._DS(mTorchStageShakeAmt));
			num10 = Common._DS(Common._M(30f)) * (1f - mTorchDaisScale);
			sexyTransform2D.LoadIdentity();
			sexyTransform2D.Scale(num9, num9);
			sexyTransform2D.RotateRad(mFrog.GetAngle());
			g.DrawImageMatrix(imageByID3, sexyTransform2D, Common._S(mFrog.GetCenterX()) - Common._DS(Common._M(2)) + Common._DS(mTorchStageShakeAmt), (float)(Common._S(mFrog.GetCenterY()) - Common._DS(Common._M1(10))) + num10 + num - (float)Common._DS(mTorchStageShakeAmt));
		}
		else if (mFrogFlyOff != null)
		{
			mFrogFlyOff.Draw(g);
		}
	}

	public void DrawBossUI(Graphics g)
	{
		GameApp gApp = GameApp.gApp;
		if (gApp.mBoard == null || gApp.mBoard.mDrawBossUI)
		{
			Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_BOSSUI);
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_RIGHTFRAMESIDE);
			if (gApp.IsWideScreen())
			{
				g.DrawImage(imageByID, (int)((double)gApp.GetScreenRect().mWidth - (double)imageByID.GetWidth() * 1.5 - (double)imageByID2.GetWidth()), 0, (int)((float)imageByID.GetWidth() * 1.5f), imageByID.GetHeight());
				g.DrawImage(imageByID2, gApp.GetWideScreenAdjusted(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GUI_INGAME_UI_RIGHTFRAMESIDE))) - (gApp.GetScreenWidth() - gApp.mScreenBounds.mWidth), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_GUI_INGAME_UI_RIGHTFRAMESIDE)));
			}
			else
			{
				g.DrawImage(imageByID, (int)((double)gApp.GetScreenRect().mWidth - (double)imageByID.GetWidth() * 1.5), 0, (int)((double)imageByID.GetWidth() * 1.5), imageByID.GetHeight());
			}
		}
	}

	public void DrawWoodPanel(Graphics g)
	{
		int num = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GUI_INGAME_UI_WOOD));
		int theY = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_GUI_INGAME_UI_WOOD));
		int x = num - mBarXOffset;
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_WOOD);
		g.DrawImage(imageByID, GameApp.gApp.GetWideScreenAdjusted(x), theY);
		g.DrawImageMirror(imageByID, GameApp.gApp.GetWideScreenAdjusted(mBarXOffset), theY);
	}

	public void DrawScoreFrame(Graphics g)
	{
		int num = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GUI_INGAME_UI_SCORE_FRAME)) + mBarXOffset;
		int theY = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_GUI_INGAME_UI_SCORE_FRAME));
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_SCORE_FRAME);
		if (!GameApp.gApp.IsWideScreen())
		{
			num -= Common._S(10);
		}
		g.DrawImage(imageByID, GameApp.gApp.GetWideScreenAdjusted(num), theY);
	}

	public void DrawTikiEnds(Graphics g)
	{
		if (GameApp.gApp.IsWideScreen())
		{
			Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_LEFTFRAMESIDE);
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_RIGHTFRAMESIDE);
			g.DrawImage(imageByID, GameApp.gApp.GetWideScreenAdjusted(0), 0);
			g.DrawImage(imageByID2, GameApp.gApp.GetWidthAdjusted(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GUI_INGAME_UI_RIGHTFRAMESIDE))) - (GameApp.gApp.GetScreenWidth() - GameApp.gApp.mScreenBounds.mWidth), 0);
		}
	}

	public void DrawZumaBar(Graphics g)
	{
		if (mTimer < 0)
		{
			Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_PROGRESSLITEWOOD);
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_GUI_PROGRESS_LIGHT);
			Image imageByID3 = Res.GetImageByID(ResID.IMAGE_GUI_PROGRESS_TOP);
			g.DrawImage(imageByID, mZumaBarX, Common._S(9));
			SetZumaBarProgress();
			if (mZumaBarState >= 2)
			{
				DrawZumaBarProgress(g, imageByID2);
				DrawZumaBarProgressPulse(g);
				DrawZumaBarProgress(g, imageByID3);
			}
		}
	}

	public void DrawFredAndGinger(Graphics g)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_CONNECT_BAR);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_LEFT_JAW);
		Image imageByID3 = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_RIGHT_JAW);
		Image imageByID4 = Res.GetImageByID(ResID.IMAGE_GUI_PROGRESS_TOP);
		Image imageByID5 = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_LEFT_MOUTH_LOWER);
		Image imageByID6 = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_RIGHT_MOUTH_UPPER);
		g.DrawImage(imageByID, GameApp.gApp.GetWideScreenAdjusted(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GUI_INGAME_UI_CONNECT_BAR)) - mBarXOffset), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_GUI_INGAME_UI_CONNECT_BAR)), imageByID4.GetWidth(), imageByID.GetHeight());
		g.DrawImage(imageByID2, (int)mGingerMouthX + mBarXOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_GUI_INGAME_UI_LEFT_JAW)) - Common._S(3));
		g.DrawImage(imageByID3, (int)mFredMouthX - mBarXOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_GUI_INGAME_UI_RIGHT_JAW)) - Common._S(2));
		DrawGoldBall(g);
		int x = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GUI_INGAME_UI_LEFT_MOUTH_LOWER)) + mBarXOffset;
		int x2 = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GUI_INGAME_UI_RIGHT_MOUTH_UPPER)) - mBarXOffset;
		g.DrawImage(imageByID5, GameApp.gApp.GetWideScreenAdjusted(x), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_GUI_INGAME_UI_LEFT_MOUTH_LOWER)));
		g.DrawImage(imageByID6, GameApp.gApp.GetWideScreenAdjusted(x2), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_GUI_INGAME_UI_RIGHT_MOUTH_UPPER)));
	}

	public void SetZumaBarProgress()
	{
		int num = Res.GetImageByID(ResID.IMAGE_GUI_PROGRESS_TOP).mWidth - mBarXOffset * 2;
		if (mZumaBarState >= 7 && mZumaBarState < 14)
		{
			mZumaBarWidth = num;
		}
		else
		{
			mZumaBarWidth = (int)((float)num * (float)mCurBarSize / 330f + (float)Common._S(8));
		}
		mZumaBarWidth = Math.Min(mZumaBarWidth, num);
	}

	public void DrawZumaBarProgress(Graphics g, Image inImage)
	{
		g.DrawImage(inImage, mZumaBarX, Common._S(9), new Rect(0, 0, mZumaBarWidth, inImage.mHeight));
		if (mZumaBarState >= 12 && mZumaBarState <= 13 && !(mBarLightness <= 0f))
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetDrawMode(1);
			g.SetColor(255, 255, 255, (int)mBarLightness);
			g.DrawImage(inImage, mZumaBarX, Common._S(9), new Rect(0, 0, mZumaBarWidth, inImage.mHeight));
			g.SetColorizeImages(colorizeImages: false);
			g.SetDrawMode(0);
		}
	}

	public void DrawZumaBarProgressPulse(Graphics g)
	{
		if (mZumaBarState >= 14)
		{
			g.PushState();
			g.SetDrawMode(1);
			int num = Common._M(0) + JeffLib.Common.GetAlphaFromUpdateCount(mUpdateCount - mZumaPulseUCStart, Common._M1(255));
			if (num > 255)
			{
				num = 255;
			}
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, num);
			Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_PROGRESS_LIGHT);
			g.DrawImage(imageByID, mZumaBarX, Common._S(9), new Rect(0, 0, mZumaBarWidth, imageByID.mHeight));
			g.PopState();
		}
	}

	public void DrawGoldBall(Graphics g)
	{
		if (mTimer < 0 && mZumaBarState < 8 && !(mZumaBallPct <= 0f))
		{
			Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_GOLD_BALL);
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_GUI_PROGRESS_TOP);
			int theWidth = (int)((float)imageByID.GetCelHeight() * mZumaBallPct);
			int num = (int)((float)imageByID.GetCelWidth() * mZumaBallPct);
			int num2 = mZumaBarX - Common._S(20) + mZumaBarWidth - imageByID.mHeight / 2 + Common._S((int)mGoldBallXOff);
			if (num2 < mZumaBarX)
			{
				num2 = mZumaBarX;
			}
			Rect theDestRect = new Rect(num2, Common._S(9) + (imageByID2.mHeight - num) / 2, theWidth, num);
			g.DrawImageCel(imageByID, theDestRect, mZumaBallFrame);
		}
	}

	public virtual int GetFarthestBallPercent(ref int farthest_curve, bool ignore_gaps)
	{
		int num = 0;
		for (int i = 0; i < mNumCurves; i++)
		{
			int farthestBallPercent = mCurveMgr[i].GetFarthestBallPercent(ignore_gaps);
			if (farthestBallPercent > num)
			{
				farthest_curve = i;
				num = farthestBallPercent;
			}
		}
		return num;
	}

	public virtual int GetFarthestBallPercent()
	{
		int farthest_curve = 0;
		return GetFarthestBallPercent(ref farthest_curve, ignore_gaps: true);
	}

	public virtual void NukeEffects()
	{
		for (int i = 0; i < mEffects.size(); i++)
		{
			mEffects[i].DeleteResources();
		}
		mEffects.Clear();
	}

	public virtual void BulletFired(Bullet b)
	{
		for (int i = 0; i < mEffects.Count(); i++)
		{
			mEffects[i].BulletFired(b);
		}
	}

	public virtual void BulletHit(Bullet b)
	{
		for (int i = 0; i < mEffects.Count(); i++)
		{
			mEffects[i].BulletHit(b);
		}
	}

	public virtual void ReactivateWalls(int wall_id)
	{
		for (int i = 0; i < mWalls.Count(); i++)
		{
			if (mWalls[i].mId == wall_id || wall_id == -1)
			{
				mWalls[i].mStrength = mWalls[i].mOrgStrength;
			}
		}
	}

	public virtual void ReactivateWalls()
	{
		ReactivateWalls(-1);
	}

	public virtual bool CompactCurves()
	{
		if (!CanCompactCurves())
		{
			return false;
		}
		for (int i = 0; i < mNumCurves; i++)
		{
			mCurveMgr[i].CompactCurve();
		}
		return true;
	}

	public virtual bool CanCompactCurves()
	{
		for (int i = 0; i < mNumCurves; i++)
		{
			if (!mCurveMgr[i].CanCompact())
			{
				return false;
			}
		}
		return true;
	}

	public virtual void SetupHiddenHoles()
	{
		if (mHoleMgr.GetNumHoles() < 2)
		{
			return;
		}
		int num = 0;
		HoleInfo hole;
		while ((hole = mHoleMgr.GetHole(num)) != null)
		{
			if (!hole.mVisible)
			{
				HoleInfo holeInfo = null;
				int num2 = 0;
				Rect theTRect = new Rect(hole.mX, hole.mY, 96, 96);
				theTRect.Inflate(-4, -4);
				while ((holeInfo = mHoleMgr.GetHole(num2)) != null)
				{
					if (!holeInfo.mVisible)
					{
						num2++;
						continue;
					}
					Rect rect = new Rect(holeInfo.mX, holeInfo.mY, 96, 96);
					rect.Inflate(-4, -4);
					if (rect.Intersects(theTRect))
					{
						hole.mShared.Add(num2);
					}
					num2++;
				}
			}
			num++;
		}
	}

	public virtual void PlayerLostLevel()
	{
	}

	public void DeactivateLightningEffects()
	{
		for (int i = 0; i < mNumCurves; i++)
		{
			mCurveMgr[i].ElectrifyBalls(-1, val: false);
		}
	}

	public bool HasPowerup(PowerType p)
	{
		for (int i = 0; i < mNumCurves; i++)
		{
			if (mCurveMgr[i].HasPowerup(p))
			{
				return true;
			}
		}
		return false;
	}

	public virtual int GetRandomPendingBallColor(int max_curve_colors)
	{
		return MathUtils.SafeRand() % max_curve_colors;
	}

	public virtual float GetRandomFrogBulletColor(int max_curve_colors, int color_num)
	{
		return 1f / (float)max_curve_colors;
	}

	public virtual Ball GetBallAtXY(int x, int y)
	{
		for (int i = 0; i < mNumCurves; i++)
		{
			foreach (Ball mBall in mCurveMgr[i].mBallList)
			{
				if (mBall.Contains(x, y))
				{
					return mBall;
				}
			}
		}
		return null;
	}

	public Ball GetRandomBall()
	{
		int num = 0;
		int num2 = MathUtils.SafeRand() % mNumCurves;
		if (mCurveMgr[num2].mBallList.Count > 3)
		{
			num = MathUtils.SafeRand() % (mCurveMgr[num2].mBallList.Count - 2);
			Ball ball = mCurveMgr[num2].mBallList[num];
			if (ball.GetPowerType() == PowerType.PowerType_Max)
			{
				return ball;
			}
		}
		return null;
	}

	public virtual void ParseUnknownAttribute(string key, string val)
	{
	}

	public virtual void CopyFrom(Level src)
	{
		for (int i = 0; i < mHoleMgr.GetNumHoles(); i++)
		{
			HoleInfo hole = mHoleMgr.GetHole(i);
			hole.mCurve = mCurveMgr[hole.mCurveNum];
		}
		mApp = GameApp.gApp;
		mBoard = mApp.GetBoard();
	}

	public int GetTotalBallsOnLevel()
	{
		int num = 0;
		for (int i = 0; i < mNumCurves; i++)
		{
			num += mCurveMgr[i].mBallList.Count();
			num += mCurveMgr[i].mPendingBalls.Count();
		}
		return num;
	}

	public int GetMaxBallsForLevel()
	{
		int num = 0;
		for (int i = 0; i < mNumCurves; i++)
		{
			num += mCurveMgr[i].mCurveDesc.mVals.mNumBalls;
		}
		return num;
	}

	public bool CheckFruitActivation(int curve_num)
	{
		if (mBoard.mPreventBallAdvancement)
		{
			return false;
		}
		int num = (mBoard.GauntletMode() ? mApp.GetLevelMgr().mGauntletTFreq : mTreasureFreq);
		if (!Board.gForceTreasure && (mBoard.mCurTreasure != null || MathUtils.SafeRand() % num != 0))
		{
			return false;
		}
		List<int> list = new List<int>();
		int num2;
		int num3;
		if (curve_num == -1)
		{
			num2 = 0;
			num3 = mNumCurves;
		}
		else
		{
			num2 = (num3 = curve_num);
		}
		for (int i = num2; i < num3; i++)
		{
			int farthestBallPercent = mCurveMgr[i].GetFarthestBallPercent();
			for (int j = 0; j < mTreasurePoints.Count(); j++)
			{
				TreasurePoint treasurePoint = mTreasurePoints[j];
				if (treasurePoint.mCurveDist[i] > 0 && farthestBallPercent >= treasurePoint.mCurveDist[i])
				{
					list.Add(j);
				}
			}
		}
		if (list.Count() == 0)
		{
			return false;
		}
		int index = MathUtils.SafeRand() % list.Count();
		mBoard.mCurTreasureNum = list[index];
		mBoard.mCurTreasure = mTreasurePoints[list[index]];
		mBoard.mMinTreasureY = (mBoard.mMaxTreasureY = float.MaxValue);
		return true;
	}

	public bool CurvesAtRest()
	{
		if (mBoard.HasFiredBullets() || mBoard.GetGun().IsFiring())
		{
			return false;
		}
		for (int i = 0; i < mNumCurves; i++)
		{
			if (!mCurveMgr[i].AtRest())
			{
				return false;
			}
		}
		return true;
	}

	public virtual void MadeCombo(int combo_size)
	{
	}

	public virtual void MadeGapShot(int gap_size)
	{
	}

	public virtual void MadeConsecutiveClear(int clear_size)
	{
	}

	public virtual void ClearedInARowBonus()
	{
	}

	public virtual void AllBallsDestroyed()
	{
	}

	public virtual void BallExploded(int ball_type)
	{
	}

	public virtual bool ShouldUpdateZumaBar()
	{
		return true;
	}

	public virtual bool AllowPointsFromBalls()
	{
		return true;
	}

	public virtual bool CanAdvanceBalls()
	{
		if (mBoss != null)
		{
			return mBoss.CanAdvanceBalls();
		}
		return true;
	}

	public virtual bool BeatLevelOverride()
	{
		return false;
	}

	public virtual void TemporarilySpeedUpCurves(float max_speed, int time_count)
	{
		mTempSpeedupTimer = time_count;
		for (int i = 0; i < mNumCurves; i++)
		{
			mCurveMgr[i].mOverrideSpeed = max_speed;
		}
	}

	public virtual void BallCreatedCallback(Ball b, int num_created)
	{
	}

	public virtual void MouseDown(int x, int y, int cc)
	{
	}

	public virtual void ChangedPad(int new_pad)
	{
		if (mDoingPadHints)
		{
			mBoard.mZumaTips[0] = null;
			mBoard.mZumaTips.RemoveAt(0);
			mBoard.MarkDirty();
		}
	}

	public virtual int GetFrogReloadType()
	{
		if (!mApp.mUserProfile.HasSeenHint(ZumaProfile.FIRST_SHOT_HINT) && !mBoard.GauntletMode() && mNum == 1 && mZone == 1)
		{
			return 2;
		}
		if (mBoss != null)
		{
			return mBoss.GetFrogReloadType();
		}
		return -1;
	}

	public virtual void PlayerStartedFiring()
	{
		if (mBoss != null)
		{
			mBoss.PlayerStartedFiring();
		}
	}

	public float GetPowerIncPct()
	{
		if (mBoard.IronFrogMode() || mBoard.GauntletMode())
		{
			return 0f;
		}
		float num = (float)mCurBarSize / 330f;
		if (num >= mApp.GetLevelMgr().mPowerupIncAtZumaPct)
		{
			return mApp.GetLevelMgr().mPowerIncPct;
		}
		return 0f;
	}

	public void IncNumBallsExploded(int val)
	{
		mApp.mUserProfile.mBallsBroken++;
		if (mBoard.GauntletMode())
		{
			mNumGauntletBallsBroke += val;
			if (mNumGauntletBallsBroke >= mGauntletCurNumForMult)
			{
				mNumGauntletBallsBroke %= mGauntletCurNumForMult;
				int num = SexyFramework.Common.Rand() % mNumCurves;
				mCurveMgr[num].mNumMultBallsToSpawn++;
				mGauntletMultipliersEarned++;
			}
		}
	}

	public virtual bool CanUpdate()
	{
		return true;
	}

	public int GetOwningCurve(Ball b)
	{
		for (int i = 0; i < mNumCurves; i++)
		{
			if (mCurveMgr[i].HasBall(b))
			{
				return i;
			}
		}
		return -1;
	}

	public void UpdateEffects()
	{
		for (int i = 0; i < mEffects.size(); i++)
		{
			mEffects[i].Update();
		}
	}

	public virtual bool CanRotateFrog()
	{
		if (mEndSequence != 2 || ((mTorchStageState == 13 || mTorchStageState == -1) && mBoss.GetHP() > 0f))
		{
			return true;
		}
		return false;
	}

	public virtual bool CanFireBall()
	{
		int num = 0;
		int num2 = 0;
		while (num2 < mNumCurves && mCurveMgr[num2].IsWinning())
		{
			num2++;
			num++;
		}
		if (mBoard.GauntletMode() && mGauntletCurTime >= mApp.GetLevelMgr().mGauntletSessionLength)
		{
			return false;
		}
		if (num == mNumCurves)
		{
			return false;
		}
		if (mDoTorchCrap && !mHasDoneTorchCrap)
		{
			return false;
		}
		if (mApp.mUserProfile.HasSeenHint(ZumaProfile.FIRST_SHOT_HINT))
		{
			return true;
		}
		if (!HasReachedCruisingSpeed() && !mBoard.GauntletMode() && (mBoss == null || !mBoss.AllowFrogToFire()))
		{
			return false;
		}
		if (mBoard.mZumaTips.Count() == 0)
		{
			return true;
		}
		if (mBoard.mZumaTips[0].mId == ZumaProfile.FIRST_SHOT_HINT && mFrog.GetAngle() >= Common._M(4.504f) && mFrog.GetAngle() <= Common._M1(4.9049f))
		{
			return true;
		}
		return false;
	}

	public virtual bool CanUseKeyboard()
	{
		return true;
	}

	public virtual bool CanSwapBalls()
	{
		return true;
	}

	public virtual Level Instantiate()
	{
		Level level = Clone();
		level.mHoleMgr = null;
		return level;
	}

	public virtual void SetFrog(Gun g)
	{
		mFrog = g;
		if (mBoss != null)
		{
			mBoss.FrogInitialized(g);
		}
		if (mSecondaryBoss != null)
		{
			mSecondaryBoss.FrogInitialized(g);
		}
		if (mMoveType == 0 || mCurveMgr[0] == null)
		{
			return;
		}
		int endPoint = mCurveMgr[0].mWayPointMgr.GetEndPoint();
		mCurveMgr[0].GetXYFromWaypoint(endPoint, out var x, out var y);
		if (mMoveType == 1)
		{
			if (y < (float)g.GetCenterY())
			{
				g.SetDestAngle(-3.14159f);
			}
			else
			{
				g.SetDestAngle(0f);
			}
		}
		else if (x < (float)g.GetCenterX())
		{
			g.SetDestAngle(-1.570795f);
		}
		else
		{
			g.SetDestAngle(1.570795f);
		}
	}

	public virtual void SyncState(DataSync sync)
	{
		SyncWalls(sync, clear: true);
		bool theBool = mBoss != null;
		bool theBool2 = mBoss == mSecondaryBoss;
		sync.SyncBoolean(ref theBool);
		sync.SyncBoolean(ref theBool2);
		sync.SyncLong(ref mInvertMouseTimer);
		sync.SyncBoolean(ref mCanDrawBoss);
		if (theBool)
		{
			bool theBool3 = false;
			if (mBoard.ShouldBypassFinalSequenceOnLoad())
			{
				theBool3 = true;
			}
			sync.SyncBoolean(ref theBool3);
			if (!theBool3)
			{
				if (sync.isWrite())
				{
					mBoss.SyncState(sync);
				}
				else
				{
					if (theBool2)
					{
						mBoss = mSecondaryBoss;
					}
					mBoss.SyncState(sync);
				}
			}
		}
		sync.SyncFloat(ref mTorchDaisScale);
		sync.SyncLong(ref mTorchStageState);
		sync.SyncLong(ref mTorchStageTimer);
		sync.SyncFloat(ref mTorchStageAlpha);
		if (sync.isRead())
		{
			if (mTorchStageState >= 9 && mTorchStageState < 13)
			{
				mTorchStageState = 13;
				mBoard.mPreventBallAdvancement = false;
				mTorchStageTimer = 0;
				mTorchDaisScale = 0f;
				mCanDrawBoss = true;
			}
			mTorches.Clear();
			SexyFramework.Misc.Buffer buffer = sync.GetBuffer();
			int num = (int)buffer.ReadLong();
			for (int i = 0; i < num; i++)
			{
				Torch torch = new Torch();
				torch.SyncState(sync);
				mTorches.Add(torch);
			}
			if (mTorchStageState != -1 && mTorchStageState < 6)
			{
				InitFinalBossLevel();
			}
			else if (mTorchStageState == 6)
			{
				mBoard.mPreventBallAdvancement = false;
				mDoTorchCrap = false;
				mHasDoneTorchCrap = true;
				mTorchDaisScale = 1f;
				mTorchTextAlpha = 0f;
			}
		}
		else
		{
			SexyFramework.Misc.Buffer buffer2 = sync.GetBuffer();
			buffer2.WriteLong(mTorches.Count);
			for (int j = 0; j < mTorches.Count; j++)
			{
				mTorches[j].SyncState(sync);
			}
		}
		sync.SyncBoolean(ref mDoTorchCrap);
		sync.SyncBoolean(ref mHasDoneTorchCrap);
		sync.SyncFloat(ref mTorchTextAlpha);
		sync.SyncLong(ref mFurthestBallDistance);
		sync.SyncLong(ref mCurFrogPoint);
		sync.SyncLong(ref mTempSpeedupTimer);
		sync.SyncBoolean(ref mHaveReachedTarget);
		sync.SyncFloat(ref mBarLightness);
		sync.SyncFloat(ref mZumaBallPct);
		sync.SyncLong(ref mZumaBarState);
		sync.SyncFloat(ref mGoldBallXOff);
		sync.SyncFloat(ref mGingerMouthX);
		sync.SyncFloat(ref mGingerMouthVX);
		sync.SyncFloat(ref mFredMouthX);
		sync.SyncFloat(ref mFredMouthVX);
		sync.SyncFloat(ref mFredTongueX);
		sync.SyncFloat(ref mFredTongueVX);
		sync.SyncLong(ref mCurBarSize);
		sync.SyncLong(ref mTargetBarSize);
		SyncWalls(sync, clear: true);
		for (int k = 0; k < mNumCurves; k++)
		{
			mCurveMgr[k].SyncState(sync);
		}
		sync.SyncBoolean(ref m_canGetAchievementNoMove);
		sync.SyncBoolean(ref m_canGetAchievementNoJump);
		sync.SyncLong(ref m_OriginX);
		sync.SyncLong(ref m_OriginY);
	}

	private void SyncWalls(DataSync sync, bool clear)
	{
		if (sync.isRead())
		{
			if (clear)
			{
				mWalls.Clear();
			}
			long num = sync.GetBuffer().ReadLong();
			for (int i = 0; i < num; i++)
			{
				Wall wall = new Wall();
				wall.SyncState(sync);
				mWalls.Add(wall);
			}
			return;
		}
		sync.GetBuffer().WriteLong(mWalls.Count);
		foreach (Wall mWall in mWalls)
		{
			mWall.SyncState(sync);
		}
	}

	public bool AllCurvesAtRolloutPoint()
	{
		return mAllCurvesAtRolloutPoint;
	}

	public bool HasReachedCruisingSpeed()
	{
		return mHasReachedCruisingSpeed;
	}

	public float GetBarPercent()
	{
		return (float)mCurBarSize / (float)mTargetBarSize;
	}

	public int GetBossBombDelay()
	{
		if (mBoss == null)
		{
			return 0;
		}
		if (!(mBoss is BossShoot bossShoot))
		{
			return 0;
		}
		return bossShoot.mBombAppearDelay;
	}

	public void ProximityBombActivated(float x, float y, int radius)
	{
		if (mBoss != null && mBoss.IsHitByExplosion(x, y, radius))
		{
			mBoss.ProximityBombActivated(x, y, radius);
		}
	}

	public void UserDied()
	{
		for (int i = 0; i < mEffects.size(); i++)
		{
			mEffects[i].UserDied();
		}
	}
}
