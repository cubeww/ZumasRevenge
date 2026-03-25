using SexyFramework;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class BetaStats
{
	public enum Mode
	{
		Mode_Challenge,
		Mode_IronFrog,
		Mode_Adventure,
		Mode_HardAdventure,
		Mode_None
	}

	protected int mSessionID;

	protected Mode mMode;

	protected int mNumDeathsThisLevel;

	protected string mLevelName;

	protected int mLevelZone;

	protected int mLevelNum;

	protected int mLevelTime;

	protected int mAceTime;

	protected int mNumGapShots;

	protected int mLargestGapShot;

	protected int mHighestGapShotScore;

	protected int mLargestChainShot;

	protected int mHighestChainShotPoints;

	protected int mLargestCombo;

	protected int mHighestComboPoints;

	protected float mFurthestRolloutPct;

	protected int mNumClearCurveBonuses;

	protected int mPerfectLevelBonus;

	protected int mAceBonus;

	protected int mPointsFromClearCurve;

	protected int mPointsFromGapShots;

	protected int mPointsFromCombos;

	protected int mPointsFromChainShots;

	protected int mNumFruits;

	protected int mLives;

	protected int mBossHP;

	protected int mPointsFromFruit;

	protected int mMaxFruitMultiplier;

	protected int mNumTimesLaserCanceled;

	protected bool mWasFromCheckpoint;

	protected bool mWasFromZoneRestart;

	protected int mLevelScore;

	protected int mTotalScore;

	protected int mPointsFromLaser;

	protected int mPointsFromCannon;

	protected int mPointsFromColorNuke;

	protected int mPointsFromProxBomb;

	protected int[] mNumTimesActivatedPowerup = new int[14];

	protected int[] mNumTimesSpawnedPowerup = new int[14];

	public ZumaProfile mProfile;

	protected void Reset()
	{
		mBossHP = 0;
		mLives = 0;
		mNumDeathsThisLevel = 0;
		mLevelTime = (mAceTime = 0);
		mHighestGapShotScore = 0;
		mHighestChainShotPoints = 0;
		mHighestComboPoints = 0;
		mFurthestRolloutPct = 0f;
		mMaxFruitMultiplier = 0;
		mPerfectLevelBonus = 0;
		mAceBonus = 0;
		mLevelScore = (mTotalScore = 0);
		mLargestGapShot = 0;
		mNumGapShots = 0;
		mPointsFromGapShots = 0;
		mLargestChainShot = 0;
		mPointsFromChainShots = 0;
		mLargestCombo = 0;
		mPointsFromCombos = 0;
		mNumClearCurveBonuses = 0;
		mPointsFromClearCurve = 0;
		mNumFruits = 0;
		mPointsFromFruit = 0;
		mNumTimesLaserCanceled = 0;
		mPointsFromLaser = 0;
		mPointsFromCannon = 0;
		mPointsFromColorNuke = 0;
		mPointsFromProxBomb = 0;
		mWasFromCheckpoint = (mWasFromZoneRestart = false);
		mNumTimesActivatedPowerup = new int[14];
		mNumTimesSpawnedPowerup = new int[14];
	}

	protected void SaveCSVFile(int challenge_level, int challenge_mult)
	{
	}

	protected void SaveCSVFile(int challenge_level)
	{
		SaveCSVFile(challenge_level, -1);
	}

	protected void SaveCSVFile()
	{
		SaveCSVFile(-1, -1);
	}

	protected void Serialize(Buffer b)
	{
	}

	protected bool Deserialize(Buffer b)
	{
		return true;
	}

	public BetaStats()
	{
		mSessionID = 0;
		mMode = Mode.Mode_None;
		mLevelZone = -1;
		mLevelNum = -1;
		mProfile = null;
		Reset();
	}

	public void CopyFrom(BetaStats rhs)
	{
		mProfile = rhs.mProfile;
		mSessionID = rhs.mSessionID;
		mMode = rhs.mMode;
		mNumDeathsThisLevel = rhs.mNumDeathsThisLevel;
		mLevelName = rhs.mLevelName;
		mLevelZone = rhs.mLevelZone;
		mLevelNum = rhs.mLevelNum;
		mLevelTime = rhs.mLevelTime;
		mAceTime = rhs.mAceTime;
		mNumGapShots = rhs.mNumGapShots;
		mLargestGapShot = rhs.mLargestGapShot;
		mHighestGapShotScore = rhs.mHighestGapShotScore;
		mLargestChainShot = rhs.mLargestChainShot;
		mHighestChainShotPoints = rhs.mHighestChainShotPoints;
		mLargestCombo = rhs.mLargestCombo;
		mHighestComboPoints = rhs.mHighestComboPoints;
		mFurthestRolloutPct = rhs.mFurthestRolloutPct;
		mNumClearCurveBonuses = rhs.mNumClearCurveBonuses;
		mPerfectLevelBonus = rhs.mPerfectLevelBonus;
		mAceBonus = rhs.mAceBonus;
		mPointsFromClearCurve = rhs.mPointsFromClearCurve;
		mPointsFromGapShots = rhs.mPointsFromGapShots;
		mPointsFromCombos = rhs.mPointsFromCombos;
		mPointsFromChainShots = rhs.mPointsFromChainShots;
		mNumFruits = rhs.mNumFruits;
		mLives = rhs.mLives;
		mBossHP = rhs.mBossHP;
		mPointsFromFruit = rhs.mPointsFromFruit;
		mMaxFruitMultiplier = rhs.mMaxFruitMultiplier;
		mNumTimesLaserCanceled = rhs.mNumTimesLaserCanceled;
		mWasFromCheckpoint = rhs.mWasFromCheckpoint;
		mWasFromZoneRestart = rhs.mWasFromZoneRestart;
		mLevelScore = rhs.mLevelScore;
		mTotalScore = rhs.mTotalScore;
		mPointsFromLaser = rhs.mPointsFromLaser;
		mPointsFromCannon = rhs.mPointsFromCannon;
		mPointsFromColorNuke = rhs.mPointsFromColorNuke;
		mPointsFromProxBomb = rhs.mPointsFromProxBomb;
		for (int i = 0; i < 14; i++)
		{
			mNumTimesActivatedPowerup[i] = rhs.mNumTimesActivatedPowerup[i];
			mNumTimesSpawnedPowerup[i] = rhs.mNumTimesSpawnedPowerup[i];
		}
	}

	public string GetCSVFileName()
	{
		return mMode switch
		{
			Mode.Mode_Adventure => SexyFramework.Common.GetAppDataFolder() + "ADVENTURE STATS DO NOT DELETE.csv", 
			Mode.Mode_HardAdventure => SexyFramework.Common.GetAppDataFolder() + "HEROIC STATS DO NOT DELETE.csv", 
			Mode.Mode_Challenge => SexyFramework.Common.GetAppDataFolder() + "CHALLENGE STATS DO NOT DELETE.csv", 
			Mode.Mode_IronFrog => SexyFramework.Common.GetAppDataFolder() + "IRON FROG STATS DO NOT DELETE.csv", 
			_ => "ERROR.csv", 
		};
	}

	public string GetDATFileName()
	{
		string arg = "";
		switch (mMode)
		{
		case Mode.Mode_Adventure:
			arg = "adv";
			break;
		case Mode.Mode_HardAdventure:
			arg = "hard_adv";
			break;
		case Mode.Mode_Challenge:
			arg = "challenge";
			break;
		case Mode.Mode_IronFrog:
			arg = "if";
			break;
		}
		return SexyFramework.Common.GetAppDataFolder() + $"users/user{mProfile.GetId()}_{arg}_stats.dat";
	}

	public void Init(ZumaProfile p, int session_id, int mode)
	{
		mProfile = p;
		mSessionID = session_id;
		Reset();
		mMode = (Mode)mode;
		mLevelZone = -1;
		mLevelNum = -1;
	}

	public void LevelStarted(string level_name, int zone, int num, bool from_checkpoint, bool zone_restart)
	{
		Reset();
		mLevelName = level_name;
		mLevelZone = zone;
		mLevelNum = num;
		mWasFromCheckpoint = from_checkpoint;
		mWasFromZoneRestart = zone_restart;
	}

	public void BeatLevel(int level_time, int ace_time, int ace_bonus, int perfect_bonus, float rollout_pct, int level_score, int total_score, int lives)
	{
		mLives = lives;
		mLevelTime = level_time;
		mAceTime = ace_time;
		mAceBonus = ace_bonus;
		mPerfectLevelBonus = perfect_bonus;
		mFurthestRolloutPct = rollout_pct;
		mLevelScore = level_score;
		mTotalScore = total_score;
		SaveCSVFile();
	}

	public void DiedOnLevel(int level_time, int level_score, int total_score, int lives_left, int challenge_level, int challenge_multiplier, int boss_hp)
	{
		mBossHP = boss_hp;
		mLevelScore = level_score;
		mTotalScore = total_score;
		mLives = lives_left;
		mFurthestRolloutPct = 1f;
		mNumDeathsThisLevel++;
		mLevelTime = level_time;
		SaveCSVFile(challenge_level, challenge_multiplier);
	}

	public void DiedOnLevel(int level_time, int level_score, int total_score, int lives_left, int challenge_level, int challenge_multiplier)
	{
		DiedOnLevel(level_time, level_score, total_score, lives_left, challenge_level, challenge_multiplier, 0);
	}

	public void DiedOnLevel(int level_time, int level_score, int total_score, int lives_left, int challenge_level)
	{
		DiedOnLevel(level_time, level_score, total_score, lives_left, challenge_level, -1);
	}

	public void DiedOnLevel(int level_time, int level_score, int total_score, int lives_left)
	{
		DiedOnLevel(level_time, level_score, total_score, lives_left, -1, -1);
	}

	public void LoadData()
	{
		string dATFileName = GetDATFileName();
		Buffer theBuffer = new Buffer();
		if (GameApp.gApp.ReadBufferFromFile(dATFileName, ref theBuffer) && !Deserialize(theBuffer))
		{
			GameApp.gApp.EraseFile(dATFileName);
			GameApp.gApp.EraseFile(GetCSVFileName());
		}
	}

	public void SaveData()
	{
		Buffer buffer = new Buffer();
		Serialize(buffer);
		GameApp.gApp.WriteBufferToFile(GetDATFileName(), buffer);
	}

	public void SetFruitMultiplier(int m)
	{
		if (m > mMaxFruitMultiplier)
		{
			mMaxFruitMultiplier = m;
		}
	}

	public void GapShot(int points, int size)
	{
		mNumGapShots++;
		mProfile.mNumGapShots++;
		if (size > mLargestGapShot)
		{
			mLargestGapShot = size;
		}
		if (points > mHighestGapShotScore)
		{
			mHighestGapShotScore = points;
		}
		if (points > mProfile.mHighestGapShotScore)
		{
			mProfile.mHighestGapShotScore = points;
		}
		if (mLargestGapShot > mProfile.mLargestGapShot)
		{
			mProfile.mLargestGapShot = mLargestGapShot;
		}
		mPointsFromGapShots += points;
		mProfile.mPointsFromGapShots += points;
	}

	public void ChainShot(int points, int size)
	{
		if (size > mLargestChainShot)
		{
			mLargestChainShot = size;
		}
		if (points > mHighestChainShotPoints)
		{
			mHighestChainShotPoints = points;
		}
		mPointsFromChainShots += points;
		mProfile.mPointsFromChainShots += points;
		if (mLargestChainShot > mProfile.mLargestChainShot)
		{
			mProfile.mLargestChainShot = mLargestChainShot;
		}
	}

	public void Combo(int points, int size)
	{
		if (points > mHighestComboPoints)
		{
			mHighestComboPoints = points;
		}
		if (size > mLargestCombo)
		{
			mLargestCombo = size;
		}
		mPointsFromCombos += points;
		mProfile.mPointsFromCombos += points;
		if (mLargestCombo > mProfile.mLargestCombo)
		{
			mProfile.mLargestCombo = mLargestCombo;
		}
	}

	public void ClearedCurve(int points)
	{
		mNumClearCurveBonuses++;
		mPointsFromClearCurve += points;
		mProfile.mNumClearCurveBonuses++;
		mProfile.mPointsFromClearCurve += points;
		if (mNumClearCurveBonuses >= 2)
		{
			GameApp.gApp.SetAchievement("clear_2x");
		}
	}

	public void HitFruit(int points)
	{
		mNumFruits++;
		mPointsFromFruit += points;
		mProfile.mNumFruits++;
		mProfile.mPointsFromFruit += points;
	}

	public void CanceledLaser()
	{
		mNumTimesLaserCanceled++;
		mProfile.mNumTimesLaserCanceled++;
	}

	public void BallExplodedFromPowerup(int power_type)
	{
		switch ((PowerType)power_type)
		{
		case PowerType.PowerType_Laser:
			mPointsFromLaser += 10;
			mProfile.mPointsFromLaser += 10;
			break;
		case PowerType.PowerType_ColorNuke:
			mPointsFromColorNuke += 10;
			mProfile.mPointsFromColorNuke += 10;
			break;
		case PowerType.PowerType_Cannon:
			mPointsFromCannon += 10;
			mProfile.mPointsFromCannon += 10;
			break;
		case PowerType.PowerType_ProximityBomb:
			mPointsFromProxBomb += 10;
			mProfile.mPointsFromProxBomb += 10;
			break;
		}
	}

	public void ActivatedPowerup(int power_type)
	{
		mNumTimesActivatedPowerup[power_type]++;
		mProfile.mNumTimesActivatedPowerup[power_type]++;
	}

	public void SpawnedPowerup(int power_type)
	{
		mNumTimesSpawnedPowerup[power_type]++;
	}
}
