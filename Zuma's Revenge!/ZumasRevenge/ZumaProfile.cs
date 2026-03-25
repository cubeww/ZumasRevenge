using System;
using System.Collections.Generic;
using SexyFramework.Drivers.Profile;
using SexyFramework.File;
using SexyFramework.Misc;
using ZumasRevenge.Achievement;

namespace ZumasRevenge;

public class ZumaProfile : UserProfile
{
	public enum ChallengeState
	{
		ChallengeState_Locked,
		ChallengeState_ZoneUnlocked,
		ChallengeState_CanPlay,
		ChallengeState_LevelComplete,
		ChallengeState_GoalComplete,
		ChallengeState_AceComplete
	}

	public static int MAX_LEVEL_SAMPLES = 4;

	public static int MAX_BOSS_SAMPLES = 4;

	public static int FIRST_SHOT_HINT = 1;

	public static int ZUMA_BAR_HINT = 2;

	public static int SKULL_PIT_HINT = 4;

	public static int LILLY_PAD_HINT = 8;

	public static int FRUIT_HINT = 16;

	public static int CHALLENGE_HINT = 32;

	public static int SWAP_BALL_HINT = 64;

	public long mLastSeenMoreGames;

	public AdvModeTempleStats mAdventureStats = new AdvModeTempleStats();

	public AdvModeTempleStats mHeroicStats = new AdvModeTempleStats();

	public IronFrogTempleStats mIronFrogStats = new IronFrogTempleStats();

	public ChallengeTempleStats mChallengeStats = new ChallengeTempleStats();

	public BetaStats mAdvBetaStats = new BetaStats();

	public BetaStats mHardAdvBetaStats = new BetaStats();

	public BetaStats mChallengeBetaStats = new BetaStats();

	public BetaStats mIronFrogBetaStats = new BetaStats();

	public bool mNewChallengeCupUnlocked;

	public int[,] mChallengeUnlockState = new int[7, 10];

	public bool mFirstTimeReplayingNormalMode;

	public bool mFirstTimeReplayingHardMode;

	public bool mUnlockTrial;

	public bool mNeedsFirstTimeIntro = true;

	public bool mWantsChallengeHelp;

	public bool mDoChallengeTrophyZoom;

	public bool mDoChallengeAceTrophyZoom;

	public bool mDoChallengeCupComplete;

	public bool mDoChallengeAceCupComplete;

	public bool mDoAceCupXFade;

	public bool mNeedsChallengeUnlockHint;

	public int mUnlockSparklesIdx1;

	public int mUnlockSparklesIdx2;

	public bool mHasDoneIFUnlockEffect;

	public bool mHasDoneChallengeUnlockEffect;

	public bool mHasDoneHeroicUnlockEffect;

	public bool mHasBeatIronFrogMode;

	public int mHighestIronFrogLevel;

	public int mHighestIronFrogScore;

	public int mNumDoubleGapShots;

	public int mNumTripleGapShots;

	public int mMatchesMade;

	public int mBallsBroken;

	public int mBallsSwapped;

	public int mBallsFired;

	public int mFruitBombed;

	public int mBallsTossed;

	public int mDeathsAfterZuma;

	public AdvModeVars mAdvModeVars = new AdvModeVars();

	public AdvModeVars mHeroicModeVars = new AdvModeVars();

	public int mHighestAdvModeScore;

	public int mAdvModeHSLevel;

	public int mAdvModeHSZone;

	public int mBoss6Part2DialogSeen;

	public int mLargestGapShot;

	public int mHighestGapShotScore;

	public int mNumGapShots;

	public int mLargestChainShot;

	public int mLargestCombo;

	public int mNumClearCurveBonuses;

	public int mPointsFromClearCurve;

	public int mPointsFromGapShots;

	public int mPointsFromCombos;

	public int mPointsFromChainShots;

	public int mNumFruits;

	public int mPointsFromFruit;

	public int mNumTimesLaserCanceled;

	public int mPointsFromLaser;

	public int mPointsFromCannon;

	public int mPointsFromColorNuke;

	public int mPointsFromProxBomb;

	public int[] mNumTimesActivatedPowerup = new int[14];

	public int[] mNumTimesSpawnedPowerup = new int[14];

	public static int MAX_GAUNTLET_HIGH_SCORES = 5;

	private bool mUserSharingEnabled;

	protected Dictionary<int, List<GauntletHSInfo>> mGauntletHSMap = new Dictionary<int, List<GauntletHSInfo>>();

	protected int mSessionID;

	protected bool mFirstTimeAtBoss;

	protected int mHintsSeen;

	public AchievementManager m_AchievementMgr = new AchievementManager();

	protected void InitBossLevel()
	{
	}

	public ZumaProfile(ZumaProfile rhs)
	{
		CopyFrom(rhs);
	}

	public ZumaProfile()
	{
		mLastSeenMoreGames = 0L;
		mWantsChallengeHelp = true;
		mNeedsFirstTimeIntro = true;
		mUnlockTrial = true;
		mHasDoneHeroicUnlockEffect = (mHasDoneChallengeUnlockEffect = (mHasDoneIFUnlockEffect = false));
		mLargestGapShot = 0;
		mHighestGapShotScore = 0;
		mNumGapShots = 0;
		mLargestChainShot = 0;
		mLargestCombo = 0;
		mNumClearCurveBonuses = 0;
		mNeedsChallengeUnlockHint = true;
		mPointsFromClearCurve = 0;
		mPointsFromGapShots = 0;
		mPointsFromCombos = 0;
		mDoChallengeAceTrophyZoom = (mDoChallengeTrophyZoom = false);
		mDoChallengeCupComplete = (mDoChallengeAceCupComplete = false);
		mUnlockSparklesIdx1 = (mUnlockSparklesIdx2 = -1);
		mPointsFromChainShots = 0;
		mNumFruits = 0;
		mPointsFromFruit = 0;
		mNumTimesLaserCanceled = 0;
		mPointsFromLaser = 0;
		mPointsFromCannon = 0;
		mPointsFromColorNuke = 0;
		mPointsFromProxBomb = 0;
		mDoAceCupXFade = false;
		mNewChallengeCupUnlocked = false;
		for (int i = 0; i < 14; i++)
		{
			mNumTimesActivatedPowerup[i] = 0;
			mNumTimesSpawnedPowerup[i] = 0;
		}
		mAdvModeVars.mDDSTier = (mAdvModeVars.mRestartDDSTier = -1);
		mHeroicModeVars.mDDSTier = (mHeroicModeVars.mRestartDDSTier = -1);
		mFirstTimeReplayingNormalMode = false;
		mFirstTimeReplayingHardMode = false;
		mBoss6Part2DialogSeen = 0;
		mSessionID = 0;
		mAdvModeVars.mHighestZoneBeat = 0;
		mAdvModeVars.mHighestLevelBeat = 0;
		mAdvModeVars.mNumDeathsCurLevel = 0;
		mAdvModeVars.mPerfectZone = true;
		mAdvModeVars.mNumZumasCurLevel = 0;
		mHeroicModeVars.mHighestZoneBeat = 0;
		mHeroicModeVars.mHighestLevelBeat = 0;
		mHeroicModeVars.mNumDeathsCurLevel = 0;
		mHeroicModeVars.mPerfectZone = true;
		mHeroicModeVars.mNumZumasCurLevel = 0;
		mHighestIronFrogLevel = (mHighestIronFrogScore = 0);
		mHighestAdvModeScore = (mAdvModeHSLevel = (mAdvModeHSZone = 0));
		mHasBeatIronFrogMode = false;
		for (int j = 0; j < 6; j++)
		{
			mHeroicModeVars.mNumTimesZoneBeat[j] = 0;
			mAdvModeVars.mNumTimesZoneBeat[j] = 0;
		}
		for (int k = 0; k < 60; k++)
		{
			mHeroicModeVars.mBestLevelTime[k] = (mAdvModeVars.mBestLevelTime[k] = int.MaxValue);
		}
		ClearAdventureModeDetails();
		mSessionID++;
		mAdvBetaStats.Init(this, mSessionID, 2);
		mHardAdvBetaStats.Init(this, mSessionID, 3);
		mChallengeBetaStats.Init(this, mSessionID, 0);
		mIronFrogBetaStats.Init(this, mSessionID, 1);
		for (int l = 0; l < 7; l++)
		{
			for (int m = 0; m < 10; m++)
			{
				mChallengeUnlockState[l, m] = 0;
			}
		}
		mNumDoubleGapShots = (mNumTripleGapShots = (mMatchesMade = (mBallsBroken = (mBallsSwapped = (mBallsFired = (mFruitBombed = (mBallsTossed = (mDeathsAfterZuma = 0))))))));
		SexyFramework.Misc.Buffer buffer = new SexyFramework.Misc.Buffer();
		mGauntletHSMap.Clear();
		if (StorageFile.ReadBufferFromFile("users/hs.dat", buffer))
		{
			long num = buffer.ReadLong();
			if (num != GameApp.gSaveGameVersion)
			{
				StorageFile.DeleteFile("users/hs.dat");
			}
			else
			{
				long num2 = buffer.ReadLong();
				for (int n = 0; n < num2; n++)
				{
					long num3 = buffer.ReadLong();
					long num4 = buffer.ReadLong();
					List<GauntletHSInfo> list = new List<GauntletHSInfo>();
					for (int num5 = 0; num5 < num4; num5++)
					{
						long num6 = buffer.ReadLong();
						string n2 = buffer.ReadString();
						list.Add(new GauntletHSInfo((int)num6, n2));
					}
					mGauntletHSMap.Add((int)num3, list);
				}
			}
		}
		mUserSharingEnabled = true;
	}

	public virtual void CopyFrom(ZumaProfile rhs)
	{
		mNeedsFirstTimeIntro = rhs.mNeedsFirstTimeIntro;
		mUnlockTrial = rhs.mUnlockTrial;
		mAdvModeVars.CopyFrom(rhs.mAdvModeVars);
		mHeroicModeVars.CopyFrom(rhs.mHeroicModeVars);
		mUnlockSparklesIdx1 = rhs.mUnlockSparklesIdx1;
		mUnlockSparklesIdx2 = rhs.mUnlockSparklesIdx2;
		mDoChallengeAceTrophyZoom = rhs.mDoChallengeAceTrophyZoom;
		mDoChallengeTrophyZoom = rhs.mDoChallengeTrophyZoom;
		mDoChallengeCupComplete = rhs.mDoChallengeCupComplete;
		mDoChallengeAceCupComplete = rhs.mDoChallengeAceCupComplete;
		mDoAceCupXFade = rhs.mDoAceCupXFade;
		mNeedsChallengeUnlockHint = rhs.mNeedsChallengeUnlockHint;
		mHasDoneHeroicUnlockEffect = rhs.mHasDoneHeroicUnlockEffect;
		mHasDoneIFUnlockEffect = rhs.mHasDoneIFUnlockEffect;
		mHasDoneChallengeUnlockEffect = rhs.mHasDoneChallengeUnlockEffect;
		mNewChallengeCupUnlocked = rhs.mNewChallengeCupUnlocked;
		mWantsChallengeHelp = rhs.mWantsChallengeHelp;
		mAdvBetaStats.CopyFrom(rhs.mAdvBetaStats);
		mHardAdvBetaStats.CopyFrom(rhs.mHardAdvBetaStats);
		mChallengeBetaStats.CopyFrom(rhs.mChallengeBetaStats);
		mIronFrogBetaStats.CopyFrom(rhs.mIronFrogBetaStats);
		mSessionID = rhs.mSessionID;
		mSessionID++;
		mAdvBetaStats.Init(this, mSessionID, 2);
		mHardAdvBetaStats.Init(this, mSessionID, 3);
		mChallengeBetaStats.Init(this, mSessionID, 0);
		mIronFrogBetaStats.Init(this, mSessionID, 1);
		mHasBeatIronFrogMode = rhs.mHasBeatIronFrogMode;
		mHighestIronFrogLevel = rhs.mHighestIronFrogLevel;
		mHighestIronFrogScore = rhs.mHighestIronFrogScore;
		for (int i = 0; i < 7; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				mChallengeUnlockState[i, j] = rhs.mChallengeUnlockState[i, j];
			}
		}
		mHighestAdvModeScore = rhs.mHighestAdvModeScore;
		mAdvModeHSLevel = rhs.mAdvModeHSLevel;
		mAdvModeHSZone = rhs.mAdvModeHSZone;
		mBoss6Part2DialogSeen = rhs.mBoss6Part2DialogSeen;
		mFirstTimeAtBoss = rhs.mFirstTimeAtBoss;
		mHintsSeen = rhs.mHintsSeen;
		mFirstTimeReplayingNormalMode = rhs.mFirstTimeReplayingNormalMode;
		mFirstTimeReplayingHardMode = rhs.mFirstTimeReplayingHardMode;
		mAdventureStats.CopyFrom(rhs.mAdventureStats);
		mHeroicStats.CopyFrom(rhs.mHeroicStats);
		mIronFrogStats.CopyFrom(rhs.mIronFrogStats);
		mChallengeStats.CopyFrom(rhs.mChallengeStats);
		mNumDoubleGapShots = rhs.mNumDoubleGapShots;
		mNumTripleGapShots = rhs.mNumTripleGapShots;
		mMatchesMade = rhs.mMatchesMade;
		mBallsBroken = rhs.mBallsBroken;
		mBallsSwapped = rhs.mBallsSwapped;
		mBallsFired = rhs.mBallsFired;
		mFruitBombed = rhs.mFruitBombed;
		mBallsTossed = rhs.mBallsTossed;
		mDeathsAfterZuma = rhs.mDeathsAfterZuma;
		mLargestGapShot = rhs.mLargestGapShot;
		mHighestGapShotScore = rhs.mHighestGapShotScore;
		mNumGapShots = rhs.mNumGapShots;
		mLargestChainShot = rhs.mLargestChainShot;
		mLargestCombo = rhs.mLargestCombo;
		mNumClearCurveBonuses = rhs.mNumClearCurveBonuses;
		mPointsFromClearCurve = rhs.mPointsFromClearCurve;
		mPointsFromGapShots = rhs.mPointsFromGapShots;
		mPointsFromCombos = rhs.mPointsFromCombos;
		mPointsFromChainShots = rhs.mPointsFromChainShots;
		mNumFruits = rhs.mNumFruits;
		mPointsFromFruit = rhs.mPointsFromFruit;
		mNumTimesLaserCanceled = rhs.mNumTimesLaserCanceled;
		mPointsFromLaser = rhs.mPointsFromLaser;
		mPointsFromCannon = rhs.mPointsFromCannon;
		mPointsFromColorNuke = rhs.mPointsFromColorNuke;
		mPointsFromProxBomb = rhs.mPointsFromProxBomb;
		for (int k = 0; k < 14; k++)
		{
			mNumTimesActivatedPowerup[k] = rhs.mNumTimesActivatedPowerup[k];
			mNumTimesSpawnedPowerup[k] = rhs.mNumTimesSpawnedPowerup[k];
		}
		mLastSeenMoreGames = rhs.mLastSeenMoreGames;
		mUserSharingEnabled = rhs.mUserSharingEnabled;
	}

	public void ClearAdventureModeDetails()
	{
		mAdvModeVars.mCurrentAdvLevel = (mAdvModeVars.mCurrentAdvZone = 1);
		mAdvModeVars.mCurrentAdvScore = 0;
		mAdvModeVars.mCurrentAdvLives = 3;
		mHeroicModeVars.mCurrentAdvLevel = (mHeroicModeVars.mCurrentAdvZone = 1);
		mHeroicModeVars.mCurrentAdvScore = 0;
		mHeroicModeVars.mCurrentAdvLives = 3;
		for (int i = 0; i < 7; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				mChallengeUnlockState[i, j] = 0;
			}
		}
		mDoChallengeAceTrophyZoom = (mDoChallengeTrophyZoom = false);
		mDoAceCupXFade = (mDoChallengeCupComplete = (mDoChallengeAceCupComplete = false));
		mHintsSeen = 0;
		mAdvModeVars.mPerfectZone = true;
		mHeroicModeVars.mPerfectZone = true;
		mFirstTimeAtBoss = true;
		mAdvModeVars.mNumDeathsCurLevel = 0;
		mAdvModeVars.mNumZumasCurLevel = 0;
		mHeroicModeVars.mNumDeathsCurLevel = 0;
		mHeroicModeVars.mNumZumasCurLevel = 0;
		mBoss6Part2DialogSeen = 0;
		mAdvModeVars.mDDSTier = (mAdvModeVars.mRestartDDSTier = -1);
		mHeroicModeVars.mDDSTier = (mHeroicModeVars.mRestartDDSTier = -1);
		for (int k = 0; k < 6; k++)
		{
			mHeroicModeVars.mFirstTimeInZone[k] = true;
			mAdvModeVars.mFirstTimeInZone[k] = true;
		}
	}

	public int ChallengeCupComplete(int zone)
	{
		zone--;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < 10; i++)
		{
			if (mChallengeUnlockState[zone, i] == 4)
			{
				num++;
			}
			else if (mChallengeUnlockState[zone, i] == 5)
			{
				num++;
				num2++;
			}
		}
		if (num2 == 10)
		{
			return 2;
		}
		if (num == 10)
		{
			return 1;
		}
		return 0;
	}

	public string GetSaveGameNameFolder()
	{
		return "users";
	}

	public string GetSaveGameName(bool hard_mode)
	{
		return GetSaveGameNameFolder() + string.Format("/{0}_in_game{1}.sav", hard_mode ? "heroic" : "adv", GetId());
	}

	public AdvModeVars GetAdvModeVars()
	{
		if (GameApp.gApp.IsHardMode())
		{
			return mHeroicModeVars;
		}
		return mAdvModeVars;
	}

	public void BossLevelStarted()
	{
		if (mFirstTimeAtBoss)
		{
			mFirstTimeAtBoss = false;
			InitBossLevel();
		}
	}

	public bool SyncDetails(DataSync theSync)
	{
		int theInt = GameApp.gSaveGameVersion;
		theSync.SyncLong(ref theInt);
		if (theInt != GameApp.gSaveGameVersion)
		{
			return false;
		}
		theSync.SyncLong(ref mAdvModeVars.mCurrentAdvLives);
		theSync.SyncLong(ref mAdvModeVars.mCurrentAdvScore);
		theSync.SyncLong(ref mAdvModeVars.mCurrentAdvLevel);
		theSync.SyncLong(ref mAdvModeVars.mCurrentAdvZone);
		theSync.SyncLong(ref mHeroicModeVars.mCurrentAdvLives);
		theSync.SyncLong(ref mHeroicModeVars.mCurrentAdvScore);
		theSync.SyncLong(ref mHeroicModeVars.mCurrentAdvLevel);
		theSync.SyncLong(ref mHeroicModeVars.mCurrentAdvZone);
		theSync.SyncBoolean(ref mWantsChallengeHelp);
		theSync.SyncBoolean(ref mNeedsFirstTimeIntro);
		theSync.SyncBoolean(ref mUnlockTrial);
		theSync.SyncLong(ref mSessionID);
		theSync.SyncLong(ref mAdvModeVars.mDDSTier);
		theSync.SyncLong(ref mAdvModeVars.mRestartDDSTier);
		theSync.SyncLong(ref mHeroicModeVars.mDDSTier);
		theSync.SyncLong(ref mHeroicModeVars.mRestartDDSTier);
		theSync.SyncBoolean(ref mHasDoneChallengeUnlockEffect);
		theSync.SyncBoolean(ref mHasDoneIFUnlockEffect);
		theSync.SyncBoolean(ref mHasDoneHeroicUnlockEffect);
		theSync.SyncBoolean(ref mFirstTimeAtBoss);
		for (int i = 0; i < 6; i++)
		{
			theSync.SyncBoolean(ref mAdvModeVars.mFirstTimeInZone[i]);
			theSync.SyncBoolean(ref mHeroicModeVars.mFirstTimeInZone[i]);
		}
		for (int j = 0; j < 6; j++)
		{
			theSync.SyncLong(ref mAdvModeVars.mCheckpointScores[j].mBoss);
			theSync.SyncLong(ref mAdvModeVars.mCheckpointScores[j].mMidpoint);
			theSync.SyncLong(ref mAdvModeVars.mCheckpointScores[j].mZoneStart);
			theSync.SyncLong(ref mHeroicModeVars.mCheckpointScores[j].mBoss);
			theSync.SyncLong(ref mHeroicModeVars.mCheckpointScores[j].mMidpoint);
			theSync.SyncLong(ref mHeroicModeVars.mCheckpointScores[j].mZoneStart);
		}
		theSync.SyncBoolean(ref mNeedsChallengeUnlockHint);
		theSync.SyncLong(ref mBoss6Part2DialogSeen);
		theSync.SyncLong(ref mHighestAdvModeScore);
		theSync.SyncLong(ref mAdvModeHSZone);
		theSync.SyncLong(ref mAdvModeHSLevel);
		theSync.SyncLong(ref mHintsSeen);
		theSync.SyncLong(ref mLargestGapShot);
		theSync.SyncLong(ref mHighestGapShotScore);
		theSync.SyncLong(ref mNumGapShots);
		theSync.SyncLong(ref mLargestChainShot);
		theSync.SyncLong(ref mLargestCombo);
		theSync.SyncLong(ref mNumClearCurveBonuses);
		theSync.SyncLong(ref mPointsFromClearCurve);
		theSync.SyncLong(ref mPointsFromGapShots);
		theSync.SyncLong(ref mPointsFromCombos);
		theSync.SyncLong(ref mPointsFromChainShots);
		theSync.SyncLong(ref mNumFruits);
		theSync.SyncLong(ref mPointsFromFruit);
		theSync.SyncLong(ref mNumTimesLaserCanceled);
		theSync.SyncLong(ref mPointsFromLaser);
		theSync.SyncLong(ref mPointsFromCannon);
		theSync.SyncLong(ref mPointsFromColorNuke);
		theSync.SyncLong(ref mPointsFromProxBomb);
		theSync.SyncLong(ref mNumDoubleGapShots);
		theSync.SyncLong(ref mNumTripleGapShots);
		theSync.SyncLong(ref mMatchesMade);
		theSync.SyncLong(ref mBallsBroken);
		theSync.SyncLong(ref mBallsSwapped);
		theSync.SyncLong(ref mBallsFired);
		theSync.SyncLong(ref mFruitBombed);
		theSync.SyncLong(ref mBallsTossed);
		theSync.SyncLong(ref mDeathsAfterZuma);
		for (int k = 0; k < 14; k++)
		{
			theSync.SyncLong(ref mNumTimesActivatedPowerup[k]);
			theSync.SyncLong(ref mNumTimesSpawnedPowerup[k]);
		}
		theSync.SyncLong(ref mAdvModeVars.mNumDeathsCurLevel);
		theSync.SyncLong(ref mAdvModeVars.mNumZumasCurLevel);
		theSync.SyncBoolean(ref mAdvModeVars.mPerfectZone);
		theSync.SyncLong(ref mHeroicModeVars.mNumDeathsCurLevel);
		theSync.SyncLong(ref mHeroicModeVars.mNumZumasCurLevel);
		theSync.SyncBoolean(ref mHeroicModeVars.mPerfectZone);
		theSync.SyncBoolean(ref mFirstTimeReplayingNormalMode);
		theSync.SyncBoolean(ref mFirstTimeReplayingHardMode);
		theSync.SyncLong(ref mAdvModeVars.mHighestZoneBeat);
		theSync.SyncLong(ref mAdvModeVars.mHighestLevelBeat);
		theSync.SyncLong(ref mHeroicModeVars.mHighestZoneBeat);
		theSync.SyncLong(ref mHeroicModeVars.mHighestLevelBeat);
		for (int l = 0; l < 6; l++)
		{
			theSync.SyncLong(ref mHeroicModeVars.mNumTimesZoneBeat[l]);
			theSync.SyncLong(ref mAdvModeVars.mNumTimesZoneBeat[l]);
		}
		for (int m = 0; m < 60; m++)
		{
			theSync.SyncLong(ref mHeroicModeVars.mBestLevelTime[m]);
			theSync.SyncLong(ref mAdvModeVars.mBestLevelTime[m]);
		}
		theSync.SyncBoolean(ref mHasBeatIronFrogMode);
		mAdventureStats.Sync(theSync);
		mHeroicStats.Sync(theSync);
		mIronFrogStats.Sync(theSync);
		mChallengeStats.Sync(theSync);
		for (int n = 0; n < 7; n++)
		{
			for (int num = 0; num < 10; num++)
			{
				theSync.SyncLong(ref mChallengeUnlockState[n, num]);
			}
		}
		theSync.SyncBoolean(ref mNewChallengeCupUnlocked);
		theSync.SyncLong(ref mLastSeenMoreGames);
		theSync.SyncBoolean(ref mUserSharingEnabled);
		m_AchievementMgr.Sync(theSync);
		return true;
	}

	public override bool ReadProfileSettings(SexyFramework.Misc.Buffer theData)
	{
		try
		{
			DataSync theSync = new DataSync(theData, isRead: true);
			SyncDetails(theSync);
			mSessionID++;
			mAdvBetaStats.Init(this, mSessionID, 2);
			mHardAdvBetaStats.Init(this, mSessionID, 3);
			mChallengeBetaStats.Init(this, mSessionID, 0);
			mIronFrogBetaStats.Init(this, mSessionID, 1);
			mAdvBetaStats.LoadData();
			mHardAdvBetaStats.LoadData();
			mChallengeBetaStats.LoadData();
			mIronFrogBetaStats.LoadData();
		}
		catch (Exception)
		{
			ClearAdventureModeDetails();
			return false;
		}
		return true;
	}

	public override bool WriteProfileSettings(SexyFramework.Misc.Buffer theData)
	{
		DataSync theSync = new DataSync(theData, isRead: false);
		SyncDetails(theSync);
		return true;
	}

	public void BossLevelComplete()
	{
		mFirstTimeAtBoss = true;
	}

	public bool HasSeenHint(int hint_num)
	{
		return (mHintsSeen & hint_num) == hint_num;
	}

	public void MarkHintAsSeen(int hint_num)
	{
		mHintsSeen |= hint_num;
	}

	public int AddGauntletHighScore(int level, int score, string profile_name)
	{
		if (!mGauntletHSMap.ContainsKey(level))
		{
			mGauntletHSMap.Add(level, new List<GauntletHSInfo>());
		}
		List<GauntletHSInfo> list = mGauntletHSMap[level];
		string[] array = new string[7]
		{
			TextManager.getInstance().getString(712),
			TextManager.getInstance().getString(713),
			TextManager.getInstance().getString(714),
			TextManager.getInstance().getString(715),
			TextManager.getInstance().getString(716),
			TextManager.getInstance().getString(717),
			TextManager.getInstance().getString(718)
		};
		int num = 0;
		while (list.Count < 5)
		{
			list.Insert(list.Count, new GauntletHSInfo(15000 - num * 1000, array[num++]));
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (score < list[i].mScore)
			{
				continue;
			}
			list.Insert(i, new GauntletHSInfo(score, profile_name));
			SexyFramework.Misc.Buffer buffer = new SexyFramework.Misc.Buffer();
			buffer.WriteLong(GameApp.gSaveGameVersion);
			buffer.WriteLong(mGauntletHSMap.Count);
			foreach (KeyValuePair<int, List<GauntletHSInfo>> item in mGauntletHSMap)
			{
				buffer.WriteLong(item.Key);
				buffer.WriteLong(item.Value.Count);
				for (int j = 0; j < item.Value.Count; j++)
				{
					buffer.WriteLong(item.Value[j].mScore);
					buffer.WriteString(item.Value[j].mProfileName);
				}
			}
			StorageFile.WriteBufferToFile("users/hs.dat", buffer);
			return i;
		}
		list.Insert(list.Count, new GauntletHSInfo(score, profile_name));
		SexyFramework.Misc.Buffer buffer2 = new SexyFramework.Misc.Buffer();
		buffer2.WriteLong(GameApp.gSaveGameVersion);
		buffer2.WriteLong(mGauntletHSMap.Count);
		foreach (KeyValuePair<int, List<GauntletHSInfo>> item2 in mGauntletHSMap)
		{
			buffer2.WriteLong(item2.Key);
			buffer2.WriteLong(item2.Value.Count);
			for (int k = 0; k < item2.Value.Count; k++)
			{
				buffer2.WriteLong(item2.Value[k].mScore);
				buffer2.WriteString(item2.Value[k].mProfileName);
			}
		}
		StorageFile.WriteBufferToFile("users/hs.dat", buffer2);
		return list.Count - 1;
	}

	private static int HSSort(GauntletHSInfo hs1, GauntletHSInfo hs2)
	{
		if (hs1.mScore > hs2.mScore)
		{
			return -1;
		}
		if (hs1.mScore < hs2.mScore)
		{
			return 1;
		}
		return 0;
	}

	public void GetGauntletHighScores(int level, ref List<GauntletHSInfo> scores)
	{
		if (mGauntletHSMap.ContainsKey(level))
		{
			scores = mGauntletHSMap[level];
		}
		string[] array = new string[7]
		{
			TextManager.getInstance().getString(719),
			TextManager.getInstance().getString(720),
			TextManager.getInstance().getString(721),
			TextManager.getInstance().getString(722),
			TextManager.getInstance().getString(723),
			TextManager.getInstance().getString(724),
			TextManager.getInstance().getString(725)
		};
		int num = 0;
		while (scores.Count < 5)
		{
			scores.Insert(scores.Count, new GauntletHSInfo(15000 - num * 1000, array[num++]));
		}
		scores.Sort(HSSort);
	}

	public void DisableUserSharing()
	{
		mUserSharingEnabled = false;
	}

	public void EnableUserSharing()
	{
		mUserSharingEnabled = true;
	}

	public bool IsUserSharingEnabled()
	{
		return mUserSharingEnabled;
	}

	internal new int GetGamepadIndex()
	{
		return 0;
	}
}
