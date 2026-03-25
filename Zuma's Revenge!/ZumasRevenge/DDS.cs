using System;
using System.Collections.Generic;
using System.Linq;

namespace ZumasRevenge;

public class DDS
{
	public static int NUM_ADVENTURE_ZONES = 6;

	public static int NUM_CHALLENGE_LEVELS = 16;

	public GameApp mApp;

	public Board mBoard;

	public ZumaProfile mProfile;

	public int mCurGauntletDiffIdx;

	public int mGauntletTime;

	public int mGauntletTimeAdd;

	public float mMaxPowerupPct;

	public float mMaxSlowPct;

	public float mMaxSpeedPct;

	public float mMaxSameColorPct;

	public float mMaxStartDistPct;

	public int mMaxZumaBackAdd;

	public int mMaxZumaSlowAdd;

	public int mMinLevel;

	public HandheldBalance mHandheldBalance = new HandheldBalance();

	protected Gauntlet_Vals mCurrentGauntletVals;

	protected DDS_Vals[] mVals = new DDS_Vals[4];

	protected List<Gauntlet_Vals>[] mGauntletVals = new List<Gauntlet_Vals>[4];

	protected Dictionary<string, Boss_DDS_Vals> mBossVals = new Dictionary<string, Boss_DDS_Vals>();

	private static int LERPint(int vmin, int vmax, int level, int range)
	{
		return (int)((float)(vmax - vmin) / (float)range) * level + vmin;
	}

	private static float LERPfloat(float vmin, float vmax, int level, int range)
	{
		return (vmax - vmin) / (float)range * (float)level + vmin;
	}

	protected void GetLERPedValues()
	{
		if (mGauntletTime == -1 || mBoard.mLevel.mNumCurves == 0)
		{
			return;
		}
		List<Gauntlet_Vals> list = mGauntletVals[mBoard.mLevel.mNumCurves - 1];
		Gauntlet_Vals gauntlet_Vals = null;
		Gauntlet_Vals gauntlet_Vals2 = null;
		for (int i = 0; i < list.Count(); i++)
		{
			if (gauntlet_Vals == null && mGauntletTime == list[i].mDifficultyLevel)
			{
				mCurrentGauntletVals = list[i];
				return;
			}
			if (gauntlet_Vals == null && mGauntletTime < list[i].mDifficultyLevel)
			{
				gauntlet_Vals = list[i - 1];
				gauntlet_Vals2 = list[i];
				break;
			}
			if (gauntlet_Vals != null && mGauntletTime < list[i].mDifficultyLevel)
			{
				gauntlet_Vals2 = list[i];
				break;
			}
		}
		if (gauntlet_Vals == null && gauntlet_Vals2 == null)
		{
			_ = list[list.Count() - 1].mDifficultyLevel;
			gauntlet_Vals = list[list.Count() - 2];
			gauntlet_Vals2 = list[list.Count() - 1];
		}
		int level = mGauntletTime - gauntlet_Vals.mDifficultyLevel;
		int range = Math.Abs(gauntlet_Vals2.mDifficultyLevel - gauntlet_Vals.mDifficultyLevel);
		mCurrentGauntletVals.mSpeed = LERPfloat(gauntlet_Vals.mSpeed, gauntlet_Vals2.mSpeed, level, range);
		mCurrentGauntletVals.mStartDistance = LERPint(gauntlet_Vals.mStartDistance, gauntlet_Vals2.mStartDistance, level, range);
		mCurrentGauntletVals.mZumaScore = LERPint(gauntlet_Vals.mZumaScore, gauntlet_Vals2.mZumaScore, level, range);
		mCurrentGauntletVals.mBallRepeat = LERPint(gauntlet_Vals.mBallRepeat, gauntlet_Vals2.mBallRepeat, level, range);
		mCurrentGauntletVals.mPowerupChance = LERPint(gauntlet_Vals.mPowerupChance, gauntlet_Vals2.mPowerupChance, level, range);
		mCurrentGauntletVals.mRollbackPct = LERPint(gauntlet_Vals.mRollbackPct, gauntlet_Vals2.mRollbackPct, level, range);
		mCurrentGauntletVals.mSlowFactor = LERPfloat(gauntlet_Vals.mSlowFactor, gauntlet_Vals2.mSlowFactor, level, range);
		mCurrentGauntletVals.mMaxClumpSize = LERPint(gauntlet_Vals.mMaxClumpSize, gauntlet_Vals2.mMaxClumpSize, level, range);
		mCurrentGauntletVals.mMaxSingle = LERPint(gauntlet_Vals.mMaxSingle, gauntlet_Vals2.mMaxSingle, level, range);
		mCurrentGauntletVals.mRollbackTime = LERPint(gauntlet_Vals.mRollbackTime, gauntlet_Vals2.mRollbackTime, level, range);
		mCurrentGauntletVals.mDifficultyLevel = mGauntletTime;
		mCurrentGauntletVals.mNumColors = gauntlet_Vals.mNumColors;
		if (mCurrentGauntletVals.mPowerupChance < 1)
		{
			mCurrentGauntletVals.mPowerupChance = 1;
		}
		if (mCurrentGauntletVals.mBallRepeat < 1)
		{
			mCurrentGauntletVals.mBallRepeat = 1;
		}
		if (mCurrentGauntletVals.mStartDistance > 95)
		{
			mCurrentGauntletVals.mStartDistance = 95;
		}
	}

	public DDS()
	{
		mApp = GameApp.gApp;
		mBoard = null;
		mProfile = null;
		mMaxPowerupPct = 0.25f;
		mMaxSlowPct = 2f;
		mMaxSpeedPct = 0.1f;
		mMaxSameColorPct = 0.1f;
		mMaxStartDistPct = 0.02f;
		mMaxZumaBackAdd = 100;
		mMaxZumaSlowAdd = 100;
		mMinLevel = 3;
		mGauntletTime = -1;
		mCurGauntletDiffIdx = -1;
		mGauntletTimeAdd = 0;
		mHandheldBalance.mFruitPowerupAdditionalDuration = 1f;
		mHandheldBalance.mChanceOfSameColorBallIncrease = 1f;
		for (int i = 0; i < NUM_ADVENTURE_ZONES; i++)
		{
			mHandheldBalance.mAdventureModeSpeedDelta[i] = 1f;
		}
		for (int j = 0; j < NUM_CHALLENGE_LEVELS; j++)
		{
			mHandheldBalance.mChallengeModeSpeedDelta[j] = 1f;
		}
		for (int k = 0; k < 4; k++)
		{
			mGauntletVals[k] = new List<Gauntlet_Vals>();
		}
		for (int l = 0; l < 4; l++)
		{
			mVals[l] = new DDS_Vals();
		}
	}

	public void StartLevel(Level l)
	{
		if (l.mBoss != null)
		{
			mProfile.BossLevelStarted();
		}
		LevelMgr levelMgr = mApp.GetLevelMgr();
		int num = mApp.mUserProfile.GetAdvModeVars().mDDSTier;
		if (num >= levelMgr.mNumDDSTiers)
		{
			num = levelMgr.mNumDDSTiers - 1;
		}
		int num2 = 0;
		if (num >= 0)
		{
			num2 = levelMgr.mDDSSlowAdd[num];
		}
		float num3 = 1f;
		if (num >= 0)
		{
			num3 = levelMgr.mDDSSpeedPct[num];
		}
		for (int i = 0; i < l.mNumCurves; i++)
		{
			DDS_Vals dDS_Vals = mVals[i];
			CurveDesc mCurveDesc = l.mCurveMgr[i].mCurveDesc;
			dDS_Vals.mSlowDistance = mCurveDesc.mVals.mSlowDistance + num2;
			dDS_Vals.mSpeed = mCurveDesc.mVals.mSpeed * num3;
			dDS_Vals.mBallRepeat = mCurveDesc.mVals.mBallRepeat;
			dDS_Vals.mZumaBack = mCurveDesc.mVals.mZumaBack;
			dDS_Vals.mZumaSlow = mCurveDesc.mVals.mZumaSlow;
			dDS_Vals.mStartDistance = mCurveDesc.mVals.mStartDistance;
			for (int j = 0; j < 14; j++)
			{
				dDS_Vals.mPowerUpFreq[j] = mCurveDesc.mVals.mPowerUpFreq[j];
			}
		}
		if (mBoard.GauntletMode())
		{
			GetLERPedValues();
		}
	}

	public void ChangeProfile(ZumaProfile p)
	{
		mProfile = p;
	}

	public void AddBossParam(string boss_name, string param_name, float _min, float _max, float range_min, float range_max)
	{
		Boss_DDS_Vals boss_DDS_Vals = null;
		if (mBossVals.ContainsKey(boss_name))
		{
			boss_DDS_Vals = mBossVals[boss_name];
		}
		else
		{
			Boss_DDS_Vals boss_DDS_Vals2 = new Boss_DDS_Vals();
			boss_DDS_Vals2.mBossName = boss_name;
			mBossVals[boss_name] = boss_DDS_Vals2;
			boss_DDS_Vals = boss_DDS_Vals2;
		}
		Boss_Param_Range boss_Param_Range = new Boss_Param_Range();
		boss_Param_Range.mMin = _min;
		boss_Param_Range.mMax = _max;
		boss_Param_Range.mRatingMin = range_min;
		boss_Param_Range.mRatingMax = range_max;
		boss_DDS_Vals.mParams[param_name].Add(boss_Param_Range);
	}

	public Dictionary<string, Boss_DDS_Vals> getBossParams()
	{
		return mBossVals;
	}

	public void AddBossParam(string boss_name, string param_name, float _min, float _max)
	{
		AddBossParam(boss_name, param_name, -1f, -1f);
	}

	public float GetBossParam(string param_name)
	{
		string mName = mBoard.mLevel.mBoss.mName;
		Boss_DDS_Vals value = null;
		mBossVals.TryGetValue(mName, out value);
		List<Boss_Param_Range> value2 = null;
		value.mParams.TryGetValue(param_name, out value2);
		List<Boss_Param_Range> list = value2;
		float num = 0.5f;
		for (int i = 0; i < list.Count(); i++)
		{
			Boss_Param_Range boss_Param_Range = list[i];
			if (!boss_Param_Range.InRange(num))
			{
				continue;
			}
			float num2 = (MathUtils._geq(boss_Param_Range.mRatingMax, 1f) ? 1f : boss_Param_Range.mRatingMax) - boss_Param_Range.mRatingMin;
			float num3 = ((num2 == 0f || boss_Param_Range.mRatingMin < 0f || boss_Param_Range.mRatingMax < 0f) ? num : ((num - boss_Param_Range.mRatingMin) / num2));
			float num4 = boss_Param_Range.mMin * (1f - num3) + num3 * boss_Param_Range.mMax;
			if (boss_Param_Range.mMin < boss_Param_Range.mMax)
			{
				if (num4 < boss_Param_Range.mMin)
				{
					num4 = boss_Param_Range.mMin;
				}
				else if (num4 > boss_Param_Range.mMax)
				{
					num4 = boss_Param_Range.mMax;
				}
			}
			else if (num4 < boss_Param_Range.mMax)
			{
				num4 = boss_Param_Range.mMax;
			}
			else if (num4 > boss_Param_Range.mMin)
			{
				num4 = boss_Param_Range.mMin;
			}
			return num4;
		}
		Boss_Param_Range boss_Param_Range2 = list[list.Count() - 1];
		return boss_Param_Range2.mMin * (1f - num) + boss_Param_Range2.mMax * num;
	}

	public bool HasBossParam(string param_name)
	{
		string mName = mBoard.mLevel.mBoss.mName;
		if (!mBossVals.ContainsKey(mName))
		{
			return false;
		}
		Boss_DDS_Vals boss_DDS_Vals = mBossVals[mName];
		return boss_DDS_Vals.mParams.ContainsKey("param_name");
	}

	public void UserLostBossLevel(float boss_health)
	{
	}

	public void BossLevelComplete()
	{
		mProfile.BossLevelComplete();
	}

	public void GetBossDebugString(ref string str, bool colorize)
	{
		if (!mBossVals.ContainsKey(mBoard.mLevel.mId))
		{
			str = "No Boss DDS defined";
			return;
		}
		Boss_DDS_Vals boss_DDS_Vals = mBossVals[mBoard.mLevel.mId];
		Dictionary<string, List<Boss_Param_Range>>.Enumerator enumerator = boss_DDS_Vals.mParams.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (!colorize)
			{
				object obj = str;
				str = string.Concat(obj, enumerator.Current.Key, ": ", GetBossParam(enumerator.Current.Key));
			}
			else
			{
				object obj2 = str;
				str = string.Concat(obj2, enumerator.Current.Key, ": ", GetBossParam(enumerator.Current.Key));
			}
		}
	}

	public void AddGauntletVals(Gauntlet_Vals vals, int num_curves)
	{
		num_curves--;
		_ = mGauntletVals[num_curves].Count;
		_ = 0;
		mGauntletVals[num_curves].Add(vals);
	}

	public List<Gauntlet_Vals>[] getGauntletVals()
	{
		return mGauntletVals;
	}

	public bool SetGauntletTime(int l)
	{
		List<Gauntlet_Vals> list = mGauntletVals[mBoard.mLevel.mNumCurves - 1];
		if (!list[0].mTimeBaseDifficulty)
		{
			return false;
		}
		if (l == 0)
		{
			mCurGauntletDiffIdx = 0;
			mCurrentGauntletVals = list[0];
			mGauntletTime = (mGauntletTimeAdd = 0);
			return false;
		}
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (l + mGauntletTimeAdd >= list[num].mDifficultyLevel)
			{
				mCurrentGauntletVals = list[num];
				mGauntletTime = l;
				int num2 = mCurGauntletDiffIdx;
				mCurGauntletDiffIdx = num;
				return num != num2;
			}
		}
		return false;
	}

	public bool SetGauntletPoints(int p)
	{
		List<Gauntlet_Vals> list = mGauntletVals[mBoard.mLevel.mNumCurves - 1];
		if (list[0].mTimeBaseDifficulty)
		{
			return false;
		}
		if (p == 0)
		{
			mCurGauntletDiffIdx = 0;
			mCurrentGauntletVals = list[0];
			return false;
		}
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (p >= list[num].mDifficultyLevel)
			{
				mCurrentGauntletVals = list[num];
				int num2 = mCurGauntletDiffIdx;
				mCurGauntletDiffIdx = num;
				return num != num2;
			}
		}
		return false;
	}

	public bool AddMultiplierTime(int t)
	{
		mGauntletTimeAdd += t;
		return SetGauntletTime(mGauntletTime);
	}

	public int GetGauntletLevel()
	{
		return mGauntletTime;
	}

	public void Reset()
	{
		mBossVals.Clear();
		for (int i = 0; i < 4; i++)
		{
			mGauntletVals[i].Clear();
		}
	}

	public int GetOverallPowerupChance(int curve_num)
	{
		if (!mBoard.GauntletMode())
		{
			LevelMgr levelMgr = mApp.GetLevelMgr();
			int num = mApp.mUserProfile.GetAdvModeVars().mDDSTier;
			if (num >= levelMgr.mNumDDSTiers)
			{
				num = levelMgr.mNumDDSTiers - 1;
			}
			float num2 = 0f;
			if (num >= 0)
			{
				num2 = levelMgr.mDDSPowerupPctInc[num];
			}
			float num3 = mBoard.mLevel.mCurveMgr[curve_num].mCurveDesc.mVals.mPowerUpChance;
			return (int)(num3 + num3 * num2);
		}
		return mCurrentGauntletVals.mPowerupChance;
	}

	public int GetPowerFreq(int powertype, int curve_num)
	{
		if (!mBoard.GauntletMode())
		{
			return mVals[curve_num].mPowerUpFreq[powertype];
		}
		if (!Common.IsDeprecatedPowerUp((PowerType)powertype) && powertype != 13)
		{
			return 100;
		}
		return 0;
	}

	public int GetSlowDistance(int curve_num)
	{
		if (!mBoard.GauntletMode())
		{
			return mVals[curve_num].mSlowDistance;
		}
		return mBoard.mLevel.mCurveMgr[curve_num].mCurveDesc.mVals.mSlowDistance;
	}

	public int GetBallRepeat(int curve_num)
	{
		if (!mBoard.GauntletMode())
		{
			return mVals[curve_num].mBallRepeat;
		}
		return mCurrentGauntletVals.mBallRepeat;
	}

	public int GetStartDistance(int curve_num)
	{
		if (!mBoard.GauntletMode())
		{
			return mVals[curve_num].mStartDistance;
		}
		return mCurrentGauntletVals.mStartDistance;
	}

	public int GetZumaBack(int curve_num)
	{
		if (!mBoard.GauntletMode())
		{
			return mVals[curve_num].mZumaBack;
		}
		return mCurrentGauntletVals.mRollbackTime;
	}

	public int GetZumaSlow(int curve_num)
	{
		if (!mBoard.GauntletMode())
		{
			return mVals[curve_num].mZumaSlow;
		}
		return mBoard.mLevel.mCurveMgr[curve_num].mCurveDesc.mVals.mZumaSlow;
	}

	public int GetZumaScore(int curve_num)
	{
		if (!mBoard.GauntletMode())
		{
			float num = 0f;
			LevelMgr levelMgr = mApp.GetLevelMgr();
			int num2 = mApp.mUserProfile.GetAdvModeVars().mDDSTier;
			if (num2 >= levelMgr.mNumDDSTiers)
			{
				num2 = levelMgr.mNumDDSTiers - 1;
			}
			if (num2 >= 0)
			{
				num = levelMgr.mDDSZumaPointDecPct[num2];
			}
			int mScoreTarget = mBoard.mLevel.mCurveMgr[curve_num].mCurveDesc.mVals.mScoreTarget;
			return mScoreTarget - (int)(num * (float)mScoreTarget);
		}
		return mCurrentGauntletVals.mZumaScore;
	}

	public int GetNumGauntletBalls(int curve_num)
	{
		return mCurrentGauntletVals.mNumColors;
	}

	public int GetGauntletDiffLevel()
	{
		return mCurrentGauntletVals.mDifficultyLevel;
	}

	public float GetSpeed(int curve_num, bool ignore_gauntlet_dds_level)
	{
		if (!mBoard.GauntletMode() || ignore_gauntlet_dds_level)
		{
			return mVals[curve_num].mSpeed;
		}
		return mCurrentGauntletVals.mSpeed;
	}

	public float GetSpeed(int curve_num)
	{
		return GetSpeed(curve_num, ignore_gauntlet_dds_level: false);
	}

	public int GetGauntletHurryDist(int curve_num)
	{
		return mGauntletVals[curve_num - 1][0].mHurryDist;
	}

	public float GetGauntletHurryMaxSpeed(int curve_num)
	{
		return mGauntletVals[curve_num - 1][0].mHurryMaxSpeed;
	}

	public float GetSlowFactor(int curve_num)
	{
		if (!mBoard.GauntletMode())
		{
			return mBoard.mLevel.mCurveMgr[curve_num].mCurveDesc.mVals.mSlowFactor;
		}
		return mCurrentGauntletVals.mSlowFactor;
	}

	public int GetRollbackPct(int curve_num)
	{
		if (!mBoard.GauntletMode())
		{
			return -9999;
		}
		return mCurrentGauntletVals.mRollbackPct;
	}

	public int GetMaxClumps(int curve_num)
	{
		if (!mBoard.GauntletMode())
		{
			return mBoard.mLevel.mCurveMgr[curve_num].mCurveDesc.mVals.mMaxClumpSize;
		}
		return mCurrentGauntletVals.mMaxClumpSize;
	}

	public int GetMaxSingles(int curve_num)
	{
		if (!mBoard.GauntletMode())
		{
			return mBoard.mLevel.mCurveMgr[curve_num].mCurveDesc.mVals.mMaxSingle;
		}
		return mCurrentGauntletVals.mMaxSingle;
	}

	public string GetStatsString(bool colorize)
	{
		if (mBoard.mLevel.mCurveMgr[0] == null)
		{
			return "";
		}
		LevelMgr levelMgr = mApp.GetLevelMgr();
		int num = mApp.mUserProfile.GetAdvModeVars().mDDSTier;
		if (num >= levelMgr.mNumDDSTiers)
		{
			num = levelMgr.mNumDDSTiers - 1;
		}
		int num2 = 0;
		if (num >= 0)
		{
			num2 = levelMgr.mDDSSlowAdd[num];
		}
		float num3 = 1f;
		if (num >= 0)
		{
			num3 = levelMgr.mDDSSpeedPct[num];
		}
		float num4 = 0f;
		if (num >= 0)
		{
			num4 = levelMgr.mDDSPowerupPctInc[num];
		}
		float num5 = 0f;
		if (num >= 0)
		{
			num5 = levelMgr.mDDSZumaPointDecPct[num];
		}
		CurveDesc mCurveDesc = mBoard.mLevel.mCurveMgr[0].mCurveDesc;
		float num6 = mCurveDesc.mVals.mPowerUpChance;
		num6 += num6 * num4;
		int mScoreTarget = mCurveDesc.mVals.mScoreTarget;
		mScoreTarget -= (int)(num5 * (float)mScoreTarget);
		return string.Format("DDS Tier: {0}, Slow dist: {0}, Speed: {0}\nPowerup chance: {0}, Zuma score:{0}", mApp.mUserProfile.GetAdvModeVars().mDDSTier, mCurveDesc.mVals.mSlowDistance + num2, mCurveDesc.mVals.mSpeed * num3, num6, mScoreTarget);
	}

	public string GetStatsString()
	{
		return GetStatsString(colorize: true);
	}
}
