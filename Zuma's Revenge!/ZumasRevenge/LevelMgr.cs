using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JeffLib;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class LevelMgr
{
	public const int TARGET_BAR_SIZE = 330;

	public const int FRED_TONGUE_X = 541;

	public const int STARTING_TORCH_TEXT_ALPHA = 700;

	public static bool gForceTreasure;

	public static int gBossNum = 0;

	private static string[] zones = new string[6] { "jungle1", "village1", "city1", "coast1", "grotto1", "volcano1" };

	public List<Point> mMapPoints = new List<Point>();

	public bool mHasFailed;

	public bool mIsHardConfig;

	public string mCurDir;

	public EffectManager mEffectManager;

	public List<string> mLevelTips = new List<string>();

	public List<int> mLevelTipIds = new List<int>();

	public List<ScoreTip> mScoreTips = new List<ScoreTip>();

	public string mLevelXML = "";

	public int mGauntletSessionLength;

	public int mGauntletNumForMultBase;

	public int mGauntletNumForMultInc;

	public int mGauntletTFreq;

	public int mMaxGauntletNumForMult;

	public int mMultiplierTimeAdd;

	public int mPointTimeAdd;

	public int mNumPointsForTimeAdd;

	public int mMultiplierDuration;

	public int mCannonShots;

	public int mBossTauntChance;

	public int mMultBallLife;

	public int mMultBallPoints;

	public int mPointsForBronze;

	public int mPointsForSilver;

	public int mPointsForGold;

	public float mPowerupIncAtZumaPct;

	public float mPowerIncPct;

	public int mClearCurvePoints;

	public float mClearCurveSpeedMult;

	public float mClearCurveRolloutPct;

	public float mCannonAngle;

	public float mMaxZumaPctForColorNuke;

	public int mLazerShots;

	public bool mCannonStacks;

	public bool mLazerStacks;

	public int mPowerDelay;

	public int mPowerCooldown;

	public int mPowerupSpawnDelay;

	public bool mAllowColorNukeAfterZuma;

	public int mColorNukeTimeAfterZuma;

	public bool mUniquePowerupColor;

	public bool mCapAffectsPowerupsSpawned;

	public int mPostZumaTime;

	public float mPostZumaTimeSpeedInc;

	public float mPostZumaTimeSlowInc;

	public float mMinMultBallDistance;

	public int mPointsForLife;

	public bool mBossesCanAttackFuckedFrog;

	public int mAttackDelayAfterHittingFrog;

	public int mBeatGamePointsForLife;

	public int mNumDDSTiers;

	public List<float> mDDSPowerupPctInc = new List<float>();

	public List<float> mDDSSpeedPct = new List<float>();

	public List<int> mDDSSlowAdd = new List<int>();

	public List<float> mDDSZumaPointDecPct = new List<float>();

	public string mError = "";

	public List<Level> mLevels = new List<Level>();

	public BXMLParser mXMLParser = new BXMLParser();

	public ZoneInfo[] mZones = new ZoneInfo[7];

	public int mFirstIronFrogLevel;

	public int mLastIronFrogLevel;

	protected bool SetupCurveInfoFromXML(string curve_str, string value, Level l)
	{
		string text = curve_str.Substring(5);
		if (text.Length == 0)
		{
			return Fail("Expected \"curve\" followed by a number, like \"curve1\" instead of just \"curve\".");
		}
		int num = Convert.ToInt32(text, 10);
		if (num - 1 >= 4 || num - 1 < 0)
		{
			return Fail($"Curve number must be in the range from 1-{4}");
		}
		if (l.mCurveMgr[num - 1] != null)
		{
			return Fail($"Curve number {num} is already defined");
		}
		if (num > l.mNumCurves)
		{
			l.mNumCurves = num;
		}
		l.mCurveMgr[num - 1] = new CurveMgr(null, num - 1);
		l.mCurveMgr[num - 1].mLevel = l;
		string text2 = value;
		if (mIsHardConfig)
		{
			text2 += "_hard";
		}
		l.mCurveMgr[num - 1].SetPath(text2);
		return true;
	}

	protected bool Fail(string theErrorText)
	{
		if (!mHasFailed)
		{
			Console.WriteLine("LevelMrg::Failed parsing binary .xml\n");
		}
		return false;
	}

	protected bool DoParseLevels()
	{
		int num = -1;
		int num2 = 0;
		gBossNum = 0;
		bool flag = false;
		int num3 = -1;
		if (!mXMLParser.HasFailed())
		{
			while (true)
			{
				BXMLElement theElement = new BXMLElement();
				if (!mXMLParser.NextElement(ref theElement))
				{
					break;
				}
				if (theElement.mType == 1)
				{
					if (!flag && theElement.mValue == "Dot")
					{
						int num4 = int.Parse(theElement.mAttributes["num"]);
						int theX = int.Parse(theElement.mAttributes["x"]);
						int theY = int.Parse(theElement.mAttributes["y"]);
						if (num4 > mMapPoints.size())
						{
							mMapPoints.Resize(num4);
						}
						mMapPoints[num4 - 1] = new Point(theX, theY);
					}
					else if (!flag && theElement.mValue == "DDS")
					{
						if (!DoParseDDS(theElement))
						{
							return false;
						}
					}
					else if (!flag && theElement.mValue == "HandheldBalance")
					{
						while (mXMLParser.NextElement(ref theElement))
						{
							_ = theElement.mValue;
							if (theElement.mType == 1)
							{
								if (theElement.mValue == "Tablet" && GameApp.IsTablet())
								{
									if (!DoParseHandheldBalance(theElement))
									{
										return false;
									}
								}
								else if (theElement.mValue == "Phone" && !GameApp.IsTablet() && !DoParseHandheldBalance(theElement))
								{
									return false;
								}
							}
							else if (theElement.mType == 2 && theElement.mValue == "HandheldBalance")
							{
								break;
							}
						}
					}
					else if (!flag && theElement.mValue == "Level")
					{
						flag = true;
						Level level = null;
						if (theElement.mValue == "Level")
						{
							level = new Level();
							mLevels.Add(level);
							level.mIndex = mLevels.Count - 1;
						}
						level.mChallengePoints = -1;
						level.mChallengeAcePoints = -1;
						foreach (KeyValuePair<string, string> mAttribute in theElement.mAttributes)
						{
							if (level == null)
							{
								break;
							}
							if (mAttribute.Key == "id")
							{
								level.mId = mAttribute.Value;
							}
							else if (mAttribute.Key == "challengepoints")
							{
								level.mChallengePoints = int.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "acechallenge")
							{
								level.mChallengeAcePoints = int.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "OffscreenClearBonus")
							{
								level.mOffscreenClearBonus = bool.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "gauntlet" || mAttribute.Key == "gauntletlevel")
							{
								level.mStartingGauntletLevel = int.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "dispname")
							{
								level.mDisplayName = mAttribute.Value;
							}
							else if (mAttribute.Key == "endsequence")
							{
								level.mEndSequence = int.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "hurryamt")
							{
								level.mHurryToRolloutAmt = float.Parse(mAttribute.Value, NumberStyles.Float, CultureInfo.InvariantCulture);
							}
							else if (mAttribute.Key == "ironfrog")
							{
								level.mIronFrog = bool.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "torchtime")
							{
								level.mTorchTimer = int.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "bossfreeze")
							{
								level.mBossFreezePowerupTime = int.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "frogshield")
							{
								level.mFrogShieldPowerupCount = int.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "drawcurve")
							{
								level.mDrawCurves = bool.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "background")
							{
								level.mImagePath = mAttribute.Value;
							}
							else if (mAttribute.Key == "psd")
							{
								level.mBGFromPSD = bool.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "nobackground")
							{
								level.mNoBackground = bool.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "noflip")
							{
								level.mNoFlip = bool.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "edgerotate")
							{
								level.mSliderEdgeRotate = bool.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "suckmode")
							{
								level.mSuckMode = bool.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "curvedata")
							{
								num3 = int.Parse(mAttribute.Value) - 1;
							}
							else if (mAttribute.Key == "tfreq")
							{
								level.mTreasureFreq = int.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "time")
							{
								level.mTimeToComplete = int.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "partime" || mAttribute.Key == "par")
							{
								level.mParTime = int.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "endless")
							{
								level.mIsEndless = bool.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "loopatend" || mAttribute.Key == "loop")
							{
								level.mLoopAtEnd = bool.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "inverttime")
							{
								level.mMaxInvertMouseTimer = int.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "potpct")
							{
								level.mPotPct = float.Parse(mAttribute.Value, NumberStyles.Float, CultureInfo.InvariantCulture) / 100f;
							}
							else if (mAttribute.Key == "NextLevelText")
							{
								level.mPreviewText = mAttribute.Value;
							}
							else if (mAttribute.Key == "finallevel")
							{
								level.mFinalLevel = bool.Parse(mAttribute.Value);
							}
							else if (mAttribute.Key == "popup")
							{
								level.mPopupText = mAttribute.Value;
							}
							else if (JeffLib.Common.StrFindNoCase(mAttribute.Key, "effect") != -1)
							{
								string value = mAttribute.Value;
								if (value != "ShadowCanopy1" && value != "ShadowCanopy2" && value != "ShadowCanopy3")
								{
									level.mEffectNames.Add(value);
								}
							}
							else if (mAttribute.Key.StartsWith("curve"))
							{
								if (mAttribute.Key.IndexOf("skullangle") != -1)
								{
									int num5 = mAttribute.Key[5] - 48 - 1;
									level.mCurveSkullAngleOverrides[num5] = MathUtils.DegreesToRadians(float.Parse(mAttribute.Value, NumberStyles.Float, CultureInfo.InvariantCulture));
								}
								else if (!SetupCurveInfoFromXML(mAttribute.Key, mAttribute.Value, level))
								{
									return false;
								}
							}
							else
							{
								level.ParseUnknownAttribute(mAttribute.Key, mAttribute.Value);
							}
						}
						if (level.mIronFrog && level.mChallengePoints == -1)
						{
							level.mChallengePoints = mZones[6].mChallengePoints;
						}
						if (level.mIronFrog && level.mChallengeAcePoints == -1)
						{
							level.mChallengeAcePoints = mZones[6].mChallengeAcePoints;
						}
						if (!level.mIronFrog && JeffLib.Common.StrFindNoCase(level.mId, "debug") == -1)
						{
							bool flag2 = false;
							for (int i = 0; i < 6; i++)
							{
								if (mZones[i].mStartLevel == level.mId)
								{
									num = i;
									num2 = 0;
									break;
								}
								if (JeffLib.Common.StrFindNoCase(level.mId, mZones[i].mBossPrefix) != -1)
								{
									num = i;
									flag2 = true;
									break;
								}
							}
							level.mZone = num + 1;
							if (!flag2)
							{
								num2 = (level.mNum = num2 + 1);
							}
							else
							{
								level.mNum = int.MaxValue;
								num2 = 0;
								num = -1;
							}
							if (level.mChallengePoints == -1)
							{
								level.mChallengePoints = mZones[level.mZone - 1].mChallengePoints;
							}
							if (level.mChallengeAcePoints == -1)
							{
								level.mChallengeAcePoints = mZones[level.mZone - 1].mChallengeAcePoints;
							}
						}
						if (level.mFinalLevel || JeffLib.Common.StrFindNoCase(level.mId, "boss6") != -1)
						{
							level.mNum = int.MaxValue;
							level.mZone = 6;
						}
					}
					else if (flag && theElement.mValue == "TreasurePoint")
					{
						Level level2 = mLevels.back();
						level2.mTreasurePoints.Add(new TreasurePoint());
						if (!DoParseTreasure(theElement, level2.mTreasurePoints.back()))
						{
							return false;
						}
					}
					else if (flag && theElement.mValue == "SetEffectParams")
					{
						Level l = mLevels.back();
						if (!DoParseSetEffectParams(theElement, l))
						{
							return false;
						}
					}
					else if (!flag && theElement.mValue == "Gauntlet")
					{
						if (!DoParseGauntletMode(theElement))
						{
							return false;
						}
					}
					else if (!flag && theElement.mValue == "Tip")
					{
						string theValue = "";
						if (!GetAttribute(theElement, "text", ref theValue))
						{
							return Fail("<Tip> section must contain /text/ parameter only");
						}
						mLevelTips.Add(theValue);
					}
					else if (!flag && theElement.mValue == "ScoreTip")
					{
						string theValue2 = "";
						if (!GetAttribute(theElement, "text", ref theValue2))
						{
							return Fail("<ScoreTip> section must contain /text/ parameter only");
						}
						int l2 = -1;
						string theValue3 = "";
						if (GetAttribute(theElement, "minlevel", ref theValue3))
						{
							l2 = int.Parse(theValue3);
						}
						mScoreTips.Add(new ScoreTip(theValue2, l2));
					}
					else if (!flag && theElement.mValue == "Zone")
					{
						string theValue4 = "";
						if (!GetAttribute(theElement, "num", ref theValue4))
						{
							return Fail("<Zone> section must contain /num/ parameter");
						}
						int num6 = int.Parse(theValue4);
						string theValue5 = "";
						if (!GetAttribute(theElement, "start", ref theValue5))
						{
							return Fail("<Zone> section must contain /start/ parameter");
						}
						int mChallengePoints = 100;
						if (GetAttribute(theElement, "challengepoints", ref theValue4))
						{
							mChallengePoints = int.Parse(theValue4);
						}
						int mChallengeAcePoints = 1000;
						if (GetAttribute(theElement, "acechallenge", ref theValue4))
						{
							mChallengeAcePoints = int.Parse(theValue4);
						}
						string theValue6 = "";
						if (!GetAttribute(theElement, "boss", ref theValue6))
						{
							return Fail("<Zone> section must contain /boss/ parameter");
						}
						string theValue7 = "";
						string theValue8 = "";
						GetAttribute(theElement, "rating", ref theValue7);
						GetAttribute(theElement, "cup", ref theValue8);
						if (mZones[num6 - 1] == null)
						{
							mZones[num6 - 1] = new ZoneInfo();
						}
						mZones[num6 - 1].mBossPrefix = theValue6;
						mZones[num6 - 1].mStartLevel = theValue5;
						mZones[num6 - 1].mNum = num6;
						mZones[num6 - 1].mChallengePoints = mChallengePoints;
						mZones[num6 - 1].mChallengeAcePoints = mChallengeAcePoints;
						mZones[num6 - 1].mDifficulty = theValue7;
						mZones[num6 - 1].mCupName = theValue8;
						string theValue9 = "";
						for (int j = 1; j <= 10; j++)
						{
							if (GetAttribute(theElement, "BossTaunt" + j, ref theValue9))
							{
								mZones[num6 - 1].mBossTaunts[j - 1] = theValue9;
							}
							else
							{
								mZones[num6 - 1].mBossTaunts[j - 1] = "";
							}
						}
						string theValue10 = "";
						if (!GetAttribute(theElement, "fruit", ref theValue10))
						{
							return Fail("<Zone> section must contain /fruit/ parameter");
						}
						mZones[num6 - 1].mFruitId = theValue10;
					}
					else if (!flag && theElement.mValue == "Defaults")
					{
						if (!DoParseDefaults(theElement))
						{
							return false;
						}
					}
					else if (flag && theElement.mValue == "Tunnel")
					{
						if (!DoParseTunnel(theElement, mLevels.back()))
						{
							return false;
						}
					}
					else if (flag && theElement.mValue == "Gun")
					{
						if (!DoParseGun(theElement, mLevels.back()))
						{
							return false;
						}
					}
					else if (flag && theElement.mValue == "PowerupRegion")
					{
						if (!DoParsePowerupRegion(theElement, mLevels.back()))
						{
							return false;
						}
					}
					else if (flag && theElement.mValue == "Torch")
					{
						if (!DoParseTorch(theElement, mLevels.back()))
						{
							return false;
						}
					}
					else if (flag && theElement.mValue == "Wall")
					{
						if (!DoParseWall(theElement, mLevels.back()))
						{
							return false;
						}
					}
					else if (flag && theElement.mValue == "MovingWall")
					{
						if (!DoParseMovingWall(theElement, mLevels.back()))
						{
							return false;
						}
					}
					else
					{
						if (!flag || !(theElement.mValue == "Boss"))
						{
							Fail("Invalid Section '" + theElement.mValue + "'");
							break;
						}
						if (!DoParseBoss(theElement, mLevels.back()))
						{
							return false;
						}
					}
				}
				else if (theElement.mType == 2)
				{
					if (!flag || (!(theElement.mValue == "Level") && !(theElement.mValue == "Challenge")))
					{
						continue;
					}
					Level level3 = mLevels.back();
					flag = false;
					if (GameApp.gApp != null)
					{
						if (level3.mImagePath == "")
						{
							level3.mImagePath = GameApp.gApp.GetResImagesDir() + "levels/" + level3.mId + "/" + level3.mId;
						}
						else if (!level3.mImagePath.StartsWith("images/levels"))
						{
							level3.mImagePath = GameApp.gApp.GetResImagesDir() + "levels/" + level3.mId + "/" + level3.mImagePath;
						}
						for (int k = 0; k < 5; k++)
						{
							if (level3.mFrogImages[k].mFilename != "")
							{
								level3.mFrogImages[k].mFilename = GameApp.gApp.GetResImagesDir() + "levels/" + level3.mId + "/" + level3.mFrogImages[k].mFilename;
							}
						}
					}
					if (num3 != -1)
					{
						if (num3 < 0 || num3 >= level3.mNumCurves)
						{
							return Fail("Invalid number set for parameter \"curvedata\":" + num3);
						}
						for (int m = 0; m < level3.mNumCurves; m++)
						{
							if (m != num3)
							{
								level3.mCurveMgr[m].CopyCurveDataFrom(level3.mCurveMgr[num3]);
							}
						}
						num3 = -1;
					}
					if (!mHasFailed)
					{
						level3.SetupHiddenHoles();
					}
					bool flag3 = false;
					for (int n = 0; n < level3.mEffectNames.size(); n++)
					{
						if (SexyFramework.Common.StrEquals(level3.mEffectNames[n], "Lavashader", pIgnoreCase: true))
						{
							flag3 = true;
							break;
						}
					}
					if (!flag3)
					{
						level3.mEffectNames.Add("LavaShader");
						level3.mEffectParams.Add(new EffectParams("fullscene", "true", level3.mEffectNames.size() - 1));
						level3.mEffectParams.Add(new EffectParams("distamt", "0.001", level3.mEffectNames.size() - 1));
						level3.mEffectParams.Add(new EffectParams("scale", "0.5", level3.mEffectNames.size() - 1));
						level3.mEffectParams.Add(new EffectParams("scroll", "0.05", level3.mEffectNames.size() - 1));
						level3.mEffectParams.Add(new EffectParams("mumu", "true", level3.mEffectNames.size() - 1));
					}
				}
				else if (theElement.mType == 3)
				{
					Fail("Element Not Expected '" + theElement.mValue + "'");
					break;
				}
			}
		}
		mXMLParser = null;
		return !mHasFailed;
	}

	protected bool DoParseDDS(BXMLElement elem)
	{
		string theValue = "";
		if (!GetAttribute(elem, "tier", ref theValue))
		{
			return Fail("Expected /tier/ tag in <DDS> section");
		}
		int num = StrToInt(theValue);
		if (!GetAttribute(elem, _S("powerpct"), ref theValue))
		{
			return Fail("Expected /powerpct/ tag in <DDS> section");
		}
		float num2 = StrToFloat(theValue);
		if (!GetAttribute(elem, _S("slowadd"), ref theValue))
		{
			return Fail("Expected /slowadd/ tag in <DDS> section");
		}
		int item = StrToInt(theValue);
		if (num != mNumDDSTiers + 1)
		{
			return Fail("You must add DDS tiers in order, starting with tier 1");
		}
		float num3 = 100f;
		if (GetAttribute(elem, _S("speedpct"), ref theValue))
		{
			num3 = StrToFloat(theValue);
		}
		float item2 = 0f;
		if (GetAttribute(elem, _S("zumapct"), ref theValue))
		{
			item2 = StrToFloat(theValue) / 100f;
		}
		mNumDDSTiers = num;
		mDDSPowerupPctInc.Add(num2 / 100f);
		mDDSSlowAdd.Add(item);
		mDDSSpeedPct.Add(num3 / 100f);
		mDDSZumaPointDecPct.Add(item2);
		return true;
	}

	protected bool DoParseGauntletMode(BXMLElement elem)
	{
		string theValue = "";
		int num = 0;
		if (!GetAttribute(elem, _S("NumCurves"), ref theValue))
		{
			return Fail("Unable to find \"NumCurves\" in \"Gauntlet\" tag");
		}
		num = StrToInt(theValue);
		int mHurryDist = 25;
		float mHurryMaxSpeed = 0f;
		if (GetAttribute(elem, _S("HurryDist"), ref theValue))
		{
			mHurryDist = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("HurrySpeed"), ref theValue))
		{
			mHurryMaxSpeed = StrToFloat(theValue);
		}
		while (mXMLParser.NextElement(ref elem))
		{
			if (elem.mType == 1)
			{
				if (!StrEquals(elem.mValue, _S("Difficulty")))
				{
					continue;
				}
				Gauntlet_Vals gauntlet_Vals = new Gauntlet_Vals();
				if (GetAttribute(elem, _S("time"), ref theValue))
				{
					gauntlet_Vals.mDifficultyLevel = StrToInt(theValue);
					gauntlet_Vals.mTimeBaseDifficulty = true;
				}
				else
				{
					if (!GetAttribute(elem, _S("points"), ref theValue))
					{
						return Fail("Unable to find \"time\" or \"points\" tag in \"Difficulty\" section for gauntlet mode.");
					}
					gauntlet_Vals.mDifficultyLevel = StrToInt(theValue);
					gauntlet_Vals.mTimeBaseDifficulty = false;
				}
				GET_GAUNTLET_ELEM(elem, _S("speed"), ref theValue);
				gauntlet_Vals.mSpeed = StrToFloat(theValue);
				GET_GAUNTLET_ELEM(elem, _S("startdist"), ref theValue);
				gauntlet_Vals.mStartDistance = StrToInt(theValue);
				GET_GAUNTLET_ELEM(elem, _S("zumascore"), ref theValue);
				gauntlet_Vals.mZumaScore = StrToInt(theValue);
				GET_GAUNTLET_ELEM(elem, _S("ballrepeat"), ref theValue);
				gauntlet_Vals.mBallRepeat = StrToInt(theValue);
				GET_GAUNTLET_ELEM(elem, _S("powerup"), ref theValue);
				gauntlet_Vals.mPowerupChance = StrToInt(theValue);
				GET_GAUNTLET_ELEM(elem, _S("colors"), ref theValue);
				gauntlet_Vals.mNumColors = StrToInt(theValue);
				GET_GAUNTLET_ELEM(elem, _S("rollback"), ref theValue);
				gauntlet_Vals.mRollbackPct = StrToInt(theValue);
				GET_GAUNTLET_ELEM(elem, _S("dangerratio"), ref theValue);
				gauntlet_Vals.mSlowFactor = StrToFloat(theValue);
				GET_GAUNTLET_ELEM(elem, _S("maxclumps"), ref theValue);
				gauntlet_Vals.mMaxClumpSize = StrToInt(theValue);
				GET_GAUNTLET_ELEM(elem, _S("maxsingles"), ref theValue);
				gauntlet_Vals.mMaxSingle = StrToInt(theValue);
				GET_GAUNTLET_ELEM(elem, _S("rollbackduration"), ref theValue);
				gauntlet_Vals.mRollbackTime = StrToInt(theValue);
				gauntlet_Vals.mHurryDist = mHurryDist;
				gauntlet_Vals.mHurryMaxSpeed = mHurryMaxSpeed;
				GameApp.gDDS.AddGauntletVals(gauntlet_Vals, num);
			}
			else if (elem.mType == 2 && StrEquals(elem.mValue, _S("Gauntlet")))
			{
				break;
			}
		}
		return true;
	}

	protected bool DoParseBossDDS(BXMLElement elem, string boss_name, Level l)
	{
		string theValue = "";
		if (!GetAttribute(elem, _S("value"), ref theValue))
		{
			return Fail("Expected /value/ tag in <DDS> section");
		}
		string theValue2 = "";
		if (!GetAttribute(elem, _S("min"), ref theValue2))
		{
			return Fail("Expected /min/ tag in <DDS> section");
		}
		float min = StrToFloat(theValue2);
		if (!GetAttribute(elem, _S("max"), ref theValue2))
		{
			return Fail("Expected /max/ tag in <DDS> section");
		}
		float max = StrToFloat(theValue2);
		float range_min = -1f;
		float range_max = -1f;
		if (GetAttribute(elem, _S("ddsmin"), ref theValue2))
		{
			range_min = StrToFloat(theValue2) / 100f;
		}
		if (GetAttribute(elem, _S("ddsmax"), ref theValue2))
		{
			range_max = StrToFloat(theValue2) / 100f;
		}
		GameApp.gDDS.AddBossParam(ToString(boss_name), theValue, min, max, range_min, range_max);
		return true;
	}

	protected bool DoParseTunnel(BXMLElement elem, Level l)
	{
		string theValue = "";
		string mImageName = "";
		string text = "";
		int mX = 0;
		int mY = 0;
		if (!GetAttribute(elem, _S("pri"), ref theValue))
		{
			return Fail("Unable to find \"pri\" (priority) parameter in \"Tunnel\" tag");
		}
		int num = sexyatoi(theValue);
		bool mAboveShadows = false;
		if (!l.mBGFromPSD)
		{
			if (!GetAttribute(elem, _S("image"), ref theValue))
			{
				return Fail("Unable to find \"image\" parameter in \"Tunnel\" tag");
			}
			mImageName = ToString(theValue);
			if (!GetAttribute(elem, _S("x"), ref theValue))
			{
				return Fail("Unable to find \"x\" parameter in \"Tunnel\" tag");
			}
			mX = sexyatoi(theValue);
			if (!GetAttribute(elem, _S("y"), ref theValue))
			{
				return Fail("Unable to find \"y\" parameter in \"Tunnel\" tag");
			}
			mY = sexyatoi(theValue);
		}
		else
		{
			if (!GetAttribute(elem, _S("layer"), ref theValue))
			{
				return Fail("Unable to find \"layer\" parameter in \"Tunnel\" tag");
			}
			text = StringToUpper(theValue);
			if (GetAttribute(elem, _S("aboveshadows"), ref theValue))
			{
				mAboveShadows = StrToBool(theValue);
			}
		}
		if (num < 0 || num >= 5)
		{
			return Fail("Priority must be in the range of 0 to " + 4);
		}
		l.mTunnelData.Add(new TunnelData());
		TunnelData tunnelData = l.mTunnelData.back();
		tunnelData.mImageName = mImageName;
		if (text.Length > 0)
		{
			tunnelData.mLayerId = "IMAGE_LEVELS_" + l.mId.ToUpper() + "_CHUTE" + ToString(text);
		}
		tunnelData.mAboveShadows = mAboveShadows;
		tunnelData.mX = mX;
		tunnelData.mY = mY;
		tunnelData.mPriority = num;
		if (GetAttribute(elem, _S("nothumb"), ref theValue))
		{
			tunnelData.mNoThumb = StrToBool(theValue);
		}
		return true;
	}

	protected bool DoParseTorch(BXMLElement elem, Level l)
	{
		string theValue = "";
		if (!GetAttribute(elem, _S("x"), ref theValue))
		{
			return false;
		}
		int x = StrToInt(theValue);
		if (!GetAttribute(elem, _S("y"), ref theValue))
		{
			return false;
		}
		int y = StrToInt(theValue);
		if (!GetAttribute(elem, _S("w"), ref theValue))
		{
			return false;
		}
		int w = StrToInt(theValue);
		if (!GetAttribute(elem, _S("h"), ref theValue))
		{
			return false;
		}
		int h = StrToInt(theValue);
		l.AddTorch(x, y, w, h);
		return true;
	}

	protected bool DoParseDefaults(BXMLElement elem)
	{
		string theValue = "";
		if (!GetAttribute(elem, _S("cannon"), ref theValue))
		{
			mCannonShots = 3;
		}
		else
		{
			mCannonShots = sexyatoi(theValue);
		}
		if (!GetAttribute(elem, _S("cannonangle"), ref theValue))
		{
			mCannonAngle = 30f;
		}
		else
		{
			mCannonAngle = sexyatof(theValue) * 3.14159f / 180f;
		}
		if (GetAttribute(elem, _S("MaxZumaPctForColorNuke"), ref theValue))
		{
			mMaxZumaPctForColorNuke = StrToFloat(theValue) / 100f;
		}
		if (GetAttribute(elem, _S("BeatGamePointsForLife"), ref theValue))
		{
			mBeatGamePointsForLife = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("PointsForExtraLife"), ref theValue))
		{
			mPointsForLife = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("BossesCanAttackAffectedFrog"), ref theValue))
		{
			mBossesCanAttackFuckedFrog = StrToBool(theValue);
		}
		if (GetAttribute(elem, _S("BossAttackDelayAfterHitFrog"), ref theValue))
		{
			mAttackDelayAfterHittingFrog = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("PointsForBronze"), ref theValue))
		{
			mPointsForBronze = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("PointsForSilver"), ref theValue))
		{
			mPointsForSilver = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("PointsForGold"), ref theValue))
		{
			mPointsForGold = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("PowerIncAtZumaPct"), ref theValue))
		{
			mPowerupIncAtZumaPct = StrToFloat(theValue) / 100f;
		}
		if (GetAttribute(elem, _S("PowerInc"), ref theValue))
		{
			mPowerIncPct = StrToFloat(theValue) / 100f;
		}
		if (GetAttribute(elem, _S("ClearCurvePoints"), ref theValue))
		{
			mClearCurvePoints = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("ClearCurveRolloutPct"), ref theValue))
		{
			mClearCurveRolloutPct = StrToFloat(theValue) / 100f;
		}
		if (GetAttribute(elem, _S("ClearCurveSpeedMult"), ref theValue))
		{
			mClearCurveSpeedMult = StrToFloat(theValue);
		}
		if (GetAttribute(elem, _S("gauntletsessionlength"), ref theValue))
		{
			mGauntletSessionLength = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("NumForMultBase"), ref theValue))
		{
			mGauntletNumForMultBase = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("NumForMultInc"), ref theValue))
		{
			mGauntletNumForMultInc = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("MultiplierDuration"), ref theValue))
		{
			mMultiplierDuration = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("MultTimeAdd"), ref theValue))
		{
			mMultiplierTimeAdd = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("MaxNumForMult"), ref theValue))
		{
			mMaxGauntletNumForMult = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("PointTimeAdd"), ref theValue))
		{
			mPointTimeAdd = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("NumPointsForTimeAdd"), ref theValue))
		{
			mNumPointsForTimeAdd = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("ChallengeTFreq"), ref theValue))
		{
			mGauntletTFreq = StrToInt(theValue);
		}
		if (!GetAttribute(elem, _S("cannonstack"), ref theValue))
		{
			mCannonStacks = true;
		}
		else
		{
			mCannonStacks = StrToBool(theValue);
		}
		if (GetAttribute(elem, _S("MinMultSpawnDist"), ref theValue))
		{
			mMinMultBallDistance = StrToFloat(theValue) / 100f;
		}
		if (GetAttribute(elem, _S("MultBallPoints"), ref theValue))
		{
			mMultBallPoints = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("MultBallLife"), ref theValue))
		{
			mMultBallLife = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("BossTauntChance"), ref theValue))
		{
			mBossTauntChance = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("powerspawndelay"), ref theValue))
		{
			mPowerupSpawnDelay = StrToInt(theValue);
		}
		else
		{
			mPowerupSpawnDelay = 0;
		}
		if (!GetAttribute(elem, _S("lazer"), ref theValue))
		{
			mLazerShots = 3;
		}
		else
		{
			mLazerShots = sexyatoi(theValue);
		}
		if (!GetAttribute(elem, _S("lazerstack"), ref theValue))
		{
			mLazerStacks = false;
		}
		else
		{
			mLazerStacks = StrToBool(theValue);
		}
		if (GetAttribute(elem, _S("powerdelay"), ref theValue))
		{
			mPowerDelay = sexyatoi(theValue);
		}
		if (GetAttribute(elem, _S("powercooldown"), ref theValue))
		{
			mPowerCooldown = sexyatoi(theValue);
		}
		if (GetAttribute(elem, _S("colornukeafterzuma"), ref theValue))
		{
			mAllowColorNukeAfterZuma = StrToBool(theValue);
		}
		if (GetAttribute(elem, _S("colornuketimeafterzuma"), ref theValue))
		{
			mColorNukeTimeAfterZuma = sexyatoi(theValue);
		}
		if (GetAttribute(elem, _S("UniquePowerupColor"), ref theValue))
		{
			mUniquePowerupColor = StrToBool(theValue);
		}
		if (GetAttribute(elem, _S("PowerupCapAffectsTriggered"), ref theValue))
		{
			mCapAffectsPowerupsSpawned = StrToBool(theValue);
		}
		if (GetAttribute(elem, _S("PZT"), ref theValue))
		{
			mPostZumaTime = StrToInt(theValue);
		}
		if (GetAttribute(elem, _S("PZTSpeedInc"), ref theValue))
		{
			mPostZumaTimeSpeedInc = StrToFloat(theValue) / 100f;
		}
		if (GetAttribute(elem, _S("PZTSlowInc"), ref theValue))
		{
			mPostZumaTimeSlowInc = StrToFloat(theValue) / 100f;
		}
		return true;
	}

	protected bool ParseCommonWallShit(BXMLElement elem, Wall w)
	{
		string theValue = "";
		if (!GetAttribute(elem, _S("x"), ref theValue))
		{
			COMMON_WALL_FAIL("x");
		}
		int num = sexyatoi(theValue);
		if (!GetAttribute(elem, _S("y"), ref theValue))
		{
			COMMON_WALL_FAIL("y");
		}
		int num2 = sexyatoi(theValue);
		if (!GetAttribute(elem, _S("width"), ref theValue) && !GetAttribute(elem, _S("w"), ref theValue))
		{
			COMMON_WALL_FAIL("width");
		}
		int num3 = sexyatoi(theValue);
		if (!GetAttribute(elem, _S("height"), ref theValue) && !GetAttribute(elem, _S("h"), ref theValue))
		{
			COMMON_WALL_FAIL("height");
		}
		int num4 = sexyatoi(theValue);
		w.mX = num;
		w.mY = num2;
		w.mWidth = num3;
		w.mHeight = num4;
		if (GetAttribute(elem, _S("color"), ref theValue))
		{
			w.mColor = new Color((int)JeffLib.Common.StrToHex(ToString(theValue)));
			if (theValue[0] == '0' && theValue[1] == '0')
			{
				w.mColor.mAlpha = 0;
			}
		}
		else
		{
			w.mColor = new Color(255, 0, 0);
		}
		w.mCurRespawnTimer = (w.mCurLifeTimer = 0);
		w.mMinRespawnTimer = (w.mMaxRespawnTimer = (w.mMinLifeTimer = (w.mMaxLifeTimer = 0)));
		if (GetAttribute(elem, _S("minrespawntime"), ref theValue))
		{
			w.mMinRespawnTimer = sexyatoi(theValue);
		}
		if (GetAttribute(elem, _S("maxrespawntime"), ref theValue))
		{
			w.mMaxRespawnTimer = sexyatoi(theValue);
		}
		if (GetAttribute(elem, _S("minlifetime"), ref theValue))
		{
			w.mMinLifeTimer = sexyatoi(theValue);
		}
		if (GetAttribute(elem, _S("maxlifetime"), ref theValue))
		{
			w.mMaxLifeTimer = sexyatoi(theValue);
		}
		w.mCurLifeTimer = MathUtils.IntRange(w.mMinLifeTimer, w.mMaxLifeTimer);
		return true;
	}

	protected bool DoParseWall(BXMLElement elem, Level l)
	{
		string theValue = "";
		int mOrgStrength = (GetAttribute(elem, _S("strength"), ref theValue) ? sexyatoi(theValue) : (-1));
		l.mWalls.Add(new Wall());
		ParseCommonWallShit(elem, l.mWalls[l.mWalls.size() - 1]);
		Wall wall = l.mWalls.back();
		wall.mStrength = (wall.mOrgStrength = mOrgStrength);
		if (GetAttribute(elem, _S("id"), ref theValue))
		{
			wall.mId = sexyatoi(theValue);
		}
		else
		{
			wall.mId = -1;
		}
		wall.mVX = (wall.mVY = 0f);
		return true;
	}

	protected bool DoParseMovingWall(BXMLElement elem, Level l)
	{
		string theValue = "";
		float mVX = 0f;
		float mVY = 0f;
		if (GetAttribute(elem, _S("vx"), ref theValue))
		{
			mVX = sexyatof(theValue);
		}
		if (GetAttribute(elem, _S("vy"), ref theValue))
		{
			mVY = sexyatof(theValue);
		}
		int num = 0;
		if (GetAttribute(elem, _S("spacing"), ref theValue))
		{
			num = sexyatoi(theValue);
		}
		l.mMovingWallDefaults.Add(new Wall());
		ParseCommonWallShit(elem, l.mMovingWallDefaults[l.mMovingWallDefaults.size() - 1]);
		Wall wall = l.mMovingWallDefaults.back();
		wall.mVX = mVX;
		wall.mVY = mVY;
		wall.mStrength = -1;
		wall.mSpacing = num * num;
		wall.mId = l.mMovingWallDefaults.size();
		return true;
	}

	protected bool DoParseTreasure(BXMLElement theElem, TreasurePoint thePoint)
	{
		string theValue = "";
		if (GetAttribute(theElem, _S("x"), ref theValue))
		{
			thePoint.x = StrToInt(theValue);
		}
		if (GetAttribute(theElem, _S("y"), ref theValue))
		{
			thePoint.y = StrToInt(theValue);
		}
		for (int i = 0; i < 4; i++)
		{
			if (GetAttribute(theElem, "dist" + (i + 1), ref theValue))
			{
				thePoint.mCurveDist[i] = StrToInt(theValue);
			}
			else
			{
				thePoint.mCurveDist[i] = 0;
			}
		}
		return true;
	}

	protected bool DoParseSetEffectParams(BXMLElement elem, Level l)
	{
		string theValue = "";
		if (!GetAttribute(elem, _S("num"), ref theValue))
		{
			return false;
		}
		int num = StrToInt(theValue);
		foreach (KeyValuePair<string, string> mAttribute in elem.mAttributes)
		{
			l.mEffectParams.Add(new EffectParams());
			EffectParams effectParams = l.mEffectParams.back();
			effectParams.mKey = ToString(mAttribute.Key);
			effectParams.mValue = ToString(mAttribute.Value);
			effectParams.mEffectIndex = num - 1;
		}
		return true;
	}

	protected bool DoParseGun(BXMLElement elem, Level l)
	{
		string theValue = "";
		if (GetAttribute(elem, _S("type"), ref theValue))
		{
			if (StrEquals(theValue, _S("normal")))
			{
				l.mMoveType = 0;
			}
			else if (StrEquals(theValue, _S("horiz")) || StrEquals(theValue, _S("horizontal")))
			{
				l.mMoveType = 1;
			}
			else
			{
				if (!StrEquals(theValue, _S("vert")) && !StrEquals(theValue, _S("vertical")))
				{
					return Fail("Invalid gun type");
				}
				l.mMoveType = 2;
			}
		}
		if (l.mMoveType == 0)
		{
			for (int i = 0; i < 5; i++)
			{
				string theValue2 = "";
				bool flag = false;
				bool flag2 = false;
				if (GetAttribute(elem, "gx" + (i + 1), ref theValue2))
				{
					flag = true;
					l.mFrogX[i] = sexyatoi(theValue2);
				}
				if (GetAttribute(elem, "gy" + (i + 1), ref theValue2))
				{
					flag2 = true;
					l.mFrogY[i] = sexyatoi(theValue2);
				}
				if (GetAttribute(elem, "image" + (i + 1), ref theValue2))
				{
					l.mFrogImages[i].mFilename = ToString(theValue2);
				}
				if (GetAttribute(elem, "resid" + (i + 1), ref theValue2))
				{
					l.mFrogImages[i].mResId = "IMAGE_LEVELS_" + StringToUpper(l.mId) + "_" + ToString(theValue2);
				}
				if (flag && flag2)
				{
					l.mNumFrogPoints = i + 1;
					continue;
				}
				if (!flag && !flag2)
				{
					break;
				}
				return Fail("For every gx, there must also be a gy: A mistmatch was detected for gun point " + (i + 1));
			}
			string theValue3 = "";
			if (GetAttribute(elem, _S("jumpspeed"), ref theValue3))
			{
				l.mMoveSpeed = sexyatoi(theValue3);
			}
			if (GetAttribute(elem, _S("image"), ref theValue3))
			{
				for (int j = 0; j < 5; j++)
				{
					l.mFrogImages[j].mFilename = ToString(theValue3);
				}
			}
			else if (GetAttribute(elem, _S("resid"), ref theValue3))
			{
				for (int k = 0; k < 5; k++)
				{
					l.mFrogImages[k].mResId = "IMAGE_LEVELS_" + StringToUpper(l.mId) + "_" + ToString(StringToUpper(theValue3));
				}
			}
		}
		else
		{
			int num = -1;
			int num2 = -1;
			string theValue4 = "";
			if (GetAttribute(elem, _S("startx"), ref theValue4))
			{
				num = sexyatoi(theValue4);
			}
			if (GetAttribute(elem, _S("starty"), ref theValue4))
			{
				num2 = sexyatoi(theValue4);
			}
			if (num == -1 || num2 == -1)
			{
				return Fail("You must specify a startx and starty for this gun type");
			}
			l.mNumFrogPoints = 1;
			l.mFrogX[0] = num;
			l.mFrogY[0] = num2;
			if (l.mMoveType == 1)
			{
				if (!GetAttribute(elem, _S("width"), ref theValue4))
				{
					return Fail("\"width\" expected for horizontal gun type");
				}
				l.mBarWidth = sexyatoi(theValue4);
			}
			else if (l.mMoveType == 2)
			{
				if (!GetAttribute(elem, _S("height"), ref theValue4))
				{
					return Fail("\"height\" expected for vertical gun type");
				}
				l.mBarHeight = sexyatoi(theValue4);
			}
		}
		return true;
	}

	protected bool DoParsePowerupRegion(BXMLElement elem, Level l)
	{
		string theValue = "";
		if (!GetAttribute(elem, _S("start"), ref theValue))
		{
			return Fail("Need /start/ tag in PowerupRegion");
		}
		float num = StrToFloat(theValue);
		if (!GetAttribute(elem, _S("end"), ref theValue))
		{
			return Fail("Need /end/ tag in PowerupRegion");
		}
		float num2 = StrToFloat(theValue);
		bool mDebugDraw = false;
		if (GetAttribute(elem, _S("debugdraw"), ref theValue))
		{
			mDebugDraw = StrToBool(theValue);
		}
		int num3 = 0;
		if (!GetAttribute(elem, _S("chance"), ref theValue))
		{
			return Fail("need /chance/ tag in PowerupRegion");
		}
		num3 = StrToInt(theValue);
		int mCurveNum = 0;
		if (!GetAttribute(elem, _S("curve"), ref theValue))
		{
			mCurveNum = StrToInt(theValue) - 1;
		}
		l.mPowerupRegions.Add(new PowerupRegion());
		PowerupRegion powerupRegion = l.mPowerupRegions.back();
		powerupRegion.mCurvePctStart = num / 100f;
		powerupRegion.mCurvePctEnd = num2 / 100f;
		powerupRegion.mChance = num3;
		powerupRegion.mCurveNum = mCurveNum;
		powerupRegion.mDebugDraw = mDebugDraw;
		return true;
	}

	protected bool DoParseBoss(BXMLElement elem, Level l)
	{
		string str = "";
		GET_BOSS_ELEM(elem, _S("type"), ref str);
		Boss boss = null;
		int num = sexyatoi(str);
		bool flag = false;
		if (GetAttribute(elem, _S("art"), ref str))
		{
			if (StrEquals(str, _S("tiger")))
			{
				boss = new BossTiger();
			}
			else if (StrEquals(str, _S("skeleton")))
			{
				flag = true;
				boss = new BossSkeleton();
			}
			else if (StrEquals(str, _S("doctor")))
			{
				boss = new BossDoctor();
			}
			else if (StrEquals(str, _S("squid")))
			{
				boss = new BossSquid();
			}
			else if (StrEquals(str, _S("mosquito")))
			{
				boss = new BossMosquito();
			}
			else if (StrEquals(str, _S("stonehead")))
			{
				boss = new BossStoneHead();
			}
			else if (StrEquals(str, _S("lame")))
			{
				boss = new BossLame();
			}
			else if (StrEquals(str, _S("volcano")))
			{
				boss = new BossVolcano();
			}
			else if (StrEquals(str, _S("darkfrog")))
			{
				boss = new BossDarkFrog();
			}
		}
		if (GetAttribute(elem, _S("sepia"), ref str))
		{
			boss.mSepiaImagePath = ToString(str);
		}
		if (boss == null)
		{
			GameApp.gApp.Popup("Invalid boss type: " + num + ". Only type 3 is valid");
			return false;
		}
		if (l.mBoss == null)
		{
			l.mBoss = (l.mOrgBoss = boss);
		}
		else
		{
			l.mSecondaryBoss = boss;
		}
		if (JeffLib.Common.StrFindNoCase(l.mId, "debug") == -1)
		{
			boss.mNum = ++gBossNum;
		}
		else
		{
			boss.mNum = gBossNum;
		}
		if (GetAttribute(elem, _S("wordbubble"), ref str))
		{
			boss.mWordBubbleText = ToString(str);
		}
		GET_BOSS_ELEM(elem, _S("hpdec"), ref str);
		float num2 = StrToFloat(str);
		boss.SetHPDecPerHit(num2);
		if (GetAttribute(elem, _S("HPDecProxBomb"), ref str))
		{
			boss.SetHPDecPerHitProxBomb(StrToFloat(str));
		}
		else
		{
			boss.SetHPDecPerHitProxBomb(num2);
		}
		if (GetAttribute(elem, _S("cancompact"), ref str))
		{
			boss.mAllowCompacting = StrToBool(str);
		}
		if (GetAttribute(elem, _S("WpnHPDec"), ref str) && flag)
		{
			(boss as BossSkeleton).mSpecialWpnHPDec = StrToInt(str);
		}
		if (GetAttribute(elem, _S("SpawnPowerupWhilePoweredUp"), ref str) && flag)
		{
			(boss as BossSkeleton).mSpawnPowerupWhilePoweredUp = StrToBool(str);
		}
		if (GetAttribute(elem, _S("ChanceToSpawnPowerup"), ref str) && flag)
		{
			(boss as BossSkeleton).mChanceToSpawnPowerup = StrToInt(str);
		}
		if (GetAttribute(elem, _S("drawradius"), ref str))
		{
			boss.mDrawRadius = StrToBool(str);
		}
		if (GetAttribute(elem, _S("RadiusColorMode"), ref str))
		{
			boss.mRadiusColorChangeMode = StrToInt(str);
		}
		if (GetAttribute(elem, _S("AllowLevelDDS"), ref str))
		{
			boss.mAllowLevelDDS = StrToBool(str);
		}
		if (GetAttribute(elem, _S("xshake"), ref str))
		{
			boss.mShakeXAmt = sexyatoi(str);
		}
		if (GetAttribute(elem, _S("yshake"), ref str))
		{
			boss.mShakeYAmt = sexyatoi(str);
		}
		if (GetAttribute(elem, _S("heartxoff"), ref str))
		{
			boss.mHeartXOff = StrToInt(str);
		}
		if (GetAttribute(elem, _S("heartyoff"), ref str))
		{
			boss.mHeartYOff = StrToInt(str);
		}
		if (GetAttribute(elem, _S("ImpatientTimer"), ref str))
		{
			boss.mImpatientTimer = StrToInt(str);
		}
		if (GetAttribute(elem, _S("ResetWallsOnHit"), ref str))
		{
			boss.mResetWallsOnBossHit = StrToBool(str);
		}
		if (GetAttribute(elem, _S("ResetTimerOnTikiHit"), ref str))
		{
			boss.mResetWallTimerOnTikiHit = StrToBool(str);
		}
		bool flag2 = false;
		if (GetAttribute(elem, _S("WallDownTime"), ref str))
		{
			flag2 = true;
			boss.mWallDownTime = StrToInt(str);
		}
		if (GetAttribute(elem, _S("TikiHealthRespawn"), ref str))
		{
			if (flag2)
			{
				return Fail("You can't use both /WallDownTime/ and /TikiHealthRespawn/");
			}
			boss.mResetWallsOnBossHit = false;
			boss.mTikiHealthRespawnAmt = StrToInt(str);
		}
		for (int i = 1; GetAttribute(elem, _S("wall" + i + "x"), ref str); i++)
		{
			int x = StrToInt(str);
			if (!GetAttribute(elem, _S("wall" + i + "y"), ref str))
			{
				return false;
			}
			int y = StrToInt(str);
			if (!GetAttribute(elem, _S("wall" + i + "w"), ref str))
			{
				return false;
			}
			int w = StrToInt(str);
			if (!GetAttribute(elem, _S("wall" + i + "h"), ref str))
			{
				return false;
			}
			int h = StrToInt(str);
			boss.AddWall(x, y, w, h, i);
		}
		for (int i = 1; GetAttribute(elem, _S("tiki" + i + "x"), ref str); i++)
		{
			int x2 = StrToInt(str);
			if (!GetAttribute(elem, _S("tiki" + i + "y"), ref str))
			{
				return false;
			}
			int y2 = StrToInt(str);
			int rail_w = 0;
			int rail_h = 0;
			int travel_time = 0;
			if (GetAttribute(elem, _S("tiki" + i + "w"), ref str))
			{
				rail_w = StrToInt(str);
			}
			if (GetAttribute(elem, _S("tiki" + i + "h"), ref str))
			{
				rail_h = StrToInt(str);
			}
			if (GetAttribute(elem, _S("tiki" + i + "time"), ref str))
			{
				travel_time = StrToInt(str);
			}
			boss.AddTiki(x2, y2, i, rail_w, rail_h, travel_time);
		}
		BossShoot bossShoot = boss as BossShoot;
		if (GetAttribute(elem, _S("bossradius"), ref str))
		{
			bossShoot.mBossRadius = StrToInt(str);
		}
		if (GetAttribute(elem, _S("maxbounces"), ref str))
		{
			bossShoot.mMaxShotBounces = StrToInt(str);
		}
		if (GetAttribute(elem, _S("BallShieldDamage"), ref str))
		{
			bossShoot.mBallShieldDamage = StrToInt(str);
		}
		if (GetAttribute(elem, _S("ShieldHP"), ref str))
		{
			bossShoot.mShieldHP = StrToInt(str);
		}
		if (GetAttribute(elem, _S("TeleportMinTime"), ref str))
		{
			bossShoot.mTeleportMinTime = StrToInt(str);
		}
		if (GetAttribute(elem, _S("TeleportMaxTime"), ref str))
		{
			bossShoot.mTeleportMaxTime = StrToInt(str);
		}
		if (GetAttribute(elem, _S("BombFreqMin"), ref str))
		{
			bossShoot.mBombFreqMin = StrToInt(str);
		}
		if (GetAttribute(elem, _S("BombFreqMax"), ref str))
		{
			bossShoot.mBombFreqMax = StrToInt(str);
		}
		if (GetAttribute(elem, _S("BombDuration"), ref str))
		{
			bossShoot.mBombDuration = StrToInt(str);
		}
		if (GetAttribute(elem, _S("MinInkNum"), ref str))
		{
			bossShoot.mMinSpots = StrToInt(str);
		}
		if (GetAttribute(elem, _S("MaxInkNum"), ref str))
		{
			bossShoot.mMaxSpots = StrToInt(str);
		}
		if (GetAttribute(elem, _S("MinInkRad"), ref str))
		{
			bossShoot.mMinSpotRad = StrToInt(str);
		}
		if (GetAttribute(elem, _S("MaxInkRad"), ref str))
		{
			bossShoot.mMaxSpotRad = StrToInt(str);
		}
		if (GetAttribute(elem, _S("MinInkFade"), ref str))
		{
			bossShoot.mMinSpotFade = StrToFloat(str);
		}
		if (GetAttribute(elem, _S("MaxInkFade"), ref str))
		{
			bossShoot.mMaxSpotFade = StrToFloat(str);
		}
		if (GetAttribute(elem, _S("InkFadeDelay"), ref str))
		{
			bossShoot.mSpotFadeDelay = StrToInt(str);
		}
		if (GetAttribute(elem, _S("InkTargetsBalls"), ref str))
		{
			bossShoot.mInkTargetMode = (StrToBool(str) ? 1 : 0);
		}
		if (GetAttribute(elem, _S("InkTargetsScreen"), ref str) && StrToBool(str))
		{
			bossShoot.mInkTargetMode = 2;
		}
		if (GetAttribute(elem, _S("EnrageDelay"), ref str))
		{
			bossShoot.mEnrageDelay = StrToInt(str);
		}
		if (GetAttribute(elem, _S("BombDelay"), ref str))
		{
			bossShoot.mBombAppearDelay = StrToInt(str);
		}
		if (GetAttribute(elem, _S("startx"), ref str))
		{
			bossShoot.mStartX = sexyatoi(str);
		}
		if (GetAttribute(elem, _S("endx"), ref str))
		{
			bossShoot.mEndX = StrToInt(str);
		}
		if (GetAttribute(elem, _S("starty"), ref str))
		{
			bossShoot.mStartY = StrToInt(str);
		}
		if (GetAttribute(elem, _S("endy"), ref str))
		{
			bossShoot.mEndY = StrToInt(str);
		}
		if (GetAttribute(elem, _S("x"), ref str))
		{
			bossShoot.SetX(StrToInt(str));
		}
		if (GetAttribute(elem, _S("y"), ref str))
		{
			bossShoot.SetY(StrToInt(str));
		}
		if (GetAttribute(elem, _S("UseShield"), ref str))
		{
			bossShoot.mUseShield = StrToBool(str);
		}
		if (GetAttribute(elem, _S("ShieldRotSpeed"), ref str))
		{
			bossShoot.mShieldRotateSpeed = MathUtils.DegreesToRadians(StrToFloat(str));
		}
		if (GetAttribute(elem, _S("ShieldRespawnTime"), ref str))
		{
			bossShoot.mShieldQuadRespawnTime = StrToInt(str);
		}
		if (GetAttribute(elem, _S("ShieldPauseTime"), ref str))
		{
			bossShoot.mShieldPauseTime = StrToInt(str);
		}
		if (GetAttribute(elem, _S("EnrageShieldRestore"), ref str))
		{
			bossShoot.mEnrageShieldRestore = StrToBool(str);
		}
		for (int i = 1; GetAttribute(elem, "x" + i, ref str); i++)
		{
			Point point = new Point();
			point.mX = StrToInt(str);
			if (!GetAttribute(elem, "y" + i, ref str))
			{
				return Fail("You must have an x<num> for every y<num> and v.v");
			}
			point.mY = StrToInt(str);
			bossShoot.mPoints.Add(point);
		}
		if (GetAttribute(elem, _S("CanShootBullets"), ref str))
		{
			bossShoot.mCanShootBullets = StrToBool(str);
		}
		if (GetAttribute(elem, _S("ShotType"), ref str))
		{
			if (StrEquals(str, _S("straight")))
			{
				bossShoot.mShotType = 0;
			}
			else if (StrEquals(str, _S("sine")))
			{
				bossShoot.mShotType = 2;
			}
			else if (StrEquals(str, _S("target")) || StrEquals(str, _S("targeted")))
			{
				bossShoot.mShotType = 1;
			}
			else if (StrEquals(str, _S("homing")))
			{
				bossShoot.mShotType = 3;
			}
			else if (StrEquals(str, _S("volcano")))
			{
				bossShoot.mShotType = 4;
			}
			else
			{
				if (!StrEquals(str, _S("any")) && !StrEquals(str, _S("all")))
				{
					return Fail("Invalid /shottype/ entered for boss: " + ToString(str));
				}
				bossShoot.mShotType = 5;
			}
		}
		if (GetAttribute(elem, _S("VolcanoOffscreenDelay"), ref str))
		{
			bossShoot.mVolcanoOffscreenDelay = StrToInt(str);
		}
		if (GetAttribute(elem, _S("art"), ref str) && bossShoot.mShotType == 3 && GetAttribute(elem, _S("homingspeed"), ref str))
		{
			bossShoot.mHomingCorrectionAmt = StrToFloat(str);
		}
		if (GetAttribute(elem, _S("retalspeedmin"), ref str))
		{
			bossShoot.mMinRetalSpeed = StrToFloat(str);
		}
		if (GetAttribute(elem, _S("retalspeedmax"), ref str))
		{
			bossShoot.mMaxRetalSpeed = StrToFloat(str);
		}
		if (GetAttribute(elem, _S("shotdelay"), ref str))
		{
			bossShoot.mShotDelay = StrToInt(str);
		}
		if (GetAttribute(elem, _S("retalshotdelay"), ref str))
		{
			bossShoot.mRetalShotDelay = StrToInt(str);
		}
		if (GetAttribute(elem, _S("EndHoverOnHit"), ref str))
		{
			bossShoot.mEndHoverOnHit = StrToBool(str);
		}
		if (GetAttribute(elem, _S("FlightSpeed"), ref str))
		{
			bossShoot.mFlightSpeed = StrToFloat(str);
		}
		if (GetAttribute(elem, _S("MinFlightDist"), ref str))
		{
			bossShoot.mFlightMinDist = StrToInt(str);
		}
		if (GetAttribute(elem, _S("colorvampire"), ref str))
		{
			bossShoot.mColorVampire = StrToBool(str);
		}
		if (GetAttribute(elem, _S("strafe"), ref str))
		{
			bossShoot.mStrafe = StrToBool(str);
		}
		if (GetAttribute(elem, _S("enrageamt"), ref str))
		{
			bossShoot.mIncMaxShotHealthAmt = StrToInt(str);
		}
		if (GetAttribute(elem, _S("retalenrageamt"), ref str))
		{
			bossShoot.mIncRetalMaxShotHealthAmt = StrToInt(str);
		}
		if (bossShoot.mColorVampire)
		{
			GET_BOSS_ELEM(elem, _S("avoidcolor"), ref str);
			bossShoot.mAvoidColor = StrToBool(str);
			GET_BOSS_ELEM(elem, _S("vamphealthinc"), ref str);
			bossShoot.mColorVampHealthInc = StrToInt(str);
			GET_BOSS_ELEM(elem, _S("vampcolorchangemin"), ref str);
			bossShoot.mMinColorChangeTime = StrToInt(str);
			GET_BOSS_ELEM(elem, _S("vampcolorchangemax"), ref str);
			bossShoot.mMaxColorChangeTime = StrToInt(str);
		}
		if (GetAttribute(elem, _S("ColorHelp"), ref str))
		{
			bossShoot.mColorVampChanceToMatch2ndBall = StrToInt(str);
			if (bossShoot.mColorVampire && bossShoot.mAvoidColor)
			{
				GameApp.gApp.MsgBox("You have a color vamp boss with /avoidcolor/ set to true,\nbut you also defined /ColorHelp/: these aren't used together, fyi.", "Yo", 0);
			}
		}
		bool flag3 = true;
		if (GetAttribute(elem, _S("MoveMode"), ref str))
		{
			bossShoot.mMovementMode = StrToInt(str);
			if (bossShoot.mMovementMode < 0 || bossShoot.mMovementMode > 2)
			{
				return Fail("/MoveMode/ must be between 0 and 2");
			}
			if (bossShoot.mMovementMode != 0)
			{
				flag3 = false;
			}
		}
		if (GetAttribute(elem, _S("Accel"), ref str))
		{
			bossShoot.mMovementAccel = StrToFloat(str);
		}
		if (GetAttribute(elem, _S("MoveDelay"), ref str))
		{
			bossShoot.mDefaultMovementUpdateDelay = StrToInt(str);
		}
		if (GetAttribute(elem, _S("minhover"), ref str))
		{
			bossShoot.mMinHoverTime = sexyatoi(str);
		}
		else if (flag3)
		{
			return Fail("you must specify /minhover/ and /maxhover/ if you aren't doing MirrorPlayer or OppositePlayer");
		}
		if (GetAttribute(elem, _S("maxhover"), ref str))
		{
			bossShoot.mMaxHoverTime = StrToInt(str);
		}
		else if (flag3)
		{
			return Fail("you must specify /minhover/ and /maxhover/ if you aren't doing MirrorPlayer or OppositePlayer");
		}
		GET_BOSS_ELEM(elem, _S("minfire"), ref str);
		bossShoot.mMinFireDelay = StrToInt(str);
		GET_BOSS_ELEM(elem, _S("maxfire"), ref str);
		bossShoot.mMaxFireDelay = StrToInt(str);
		if (GetAttribute(elem, _S("NormalPassUnder"), ref str))
		{
			bossShoot.mEatsBalls = StrToBool(str);
		}
		string theValue = "";
		string theValue2 = "";
		string theValue3 = "";
		string theValue4 = "";
		GetAttribute(elem, _S("stun"), ref theValue);
		GetAttribute(elem, _S("poison"), ref theValue2);
		GetAttribute(elem, _S("hallucinate"), ref theValue3);
		GetAttribute(elem, _S("SlowShot"), ref theValue4);
		if (theValue.Length > 0)
		{
			bossShoot.mFrogStunTime = StrToInt(theValue);
		}
		else if (theValue2.Length > 0)
		{
			bossShoot.mFrogPoisonTime = StrToInt(theValue2);
		}
		else if (theValue3.Length > 0)
		{
			bossShoot.mFrogHallucinateTime = StrToInt(theValue3);
		}
		else if (theValue4.Length > 0)
		{
			bossShoot.mFrogSlowTimer = StrToInt(theValue4);
		}
		GET_BOSS_ELEM(elem, _S("minbullet"), ref str);
		bossShoot.mMinBulletSpeed = StrToFloat(str);
		GET_BOSS_ELEM(elem, _S("maxbullet"), ref str);
		bossShoot.mMaxBulletSpeed = StrToFloat(str);
		if (GetAttribute(elem, _S("subtype"), ref str))
		{
			bossShoot.mSubType = StrToInt(str) - 1;
		}
		if (GetAttribute(elem, _S("maxbullets"), ref str))
		{
			bossShoot.mMaxBulletsToFire = StrToInt(str);
		}
		if (GetAttribute(elem, _S("retaliation"), ref str))
		{
			bossShoot.mMaxRetaliationBullets = StrToInt(str);
		}
		if (GetAttribute(elem, _S("SineShotsTargetPlayer"), ref str))
		{
			bossShoot.mSineShotsTargetPlayer = StrToBool(str);
		}
		if (GetAttribute(elem, _S("MinSineShotTime"), ref str))
		{
			bossShoot.mMinSineShotTime = StrToInt(str);
		}
		if (GetAttribute(elem, _S("MaxSineShotTime"), ref str))
		{
			bossShoot.mMaxSineShotTime = StrToInt(str);
		}
		if (GetAttribute(elem, _S("retaltype"), ref str))
		{
			bossShoot.mSinusoidalRetaliation = StrEquals(str, _S("sine")) || StrEquals(str, _S("sinusoidal"));
		}
		if (GetAttribute(elem, _S("minamp"), ref str))
		{
			bossShoot.mMinAmp = StrToFloat(str);
		}
		if (GetAttribute(elem, _S("maxamp"), ref str))
		{
			bossShoot.mMaxAmp = StrToFloat(str);
		}
		if (GetAttribute(elem, _S("minfreq"), ref str))
		{
			bossShoot.mMinFreq = StrToFloat(str);
		}
		if (GetAttribute(elem, _S("maxfreq"), ref str))
		{
			bossShoot.mMaxFreq = StrToFloat(str);
		}
		if (GetAttribute(elem, _S("MinSineYInc"), ref str))
		{
			bossShoot.mMinYInc = StrToFloat(str);
		}
		if (GetAttribute(elem, _S("MaxSineYInc"), ref str))
		{
			bossShoot.mMaxYInc = StrToFloat(str);
		}
		if (GetAttribute(elem, _S("MinSineXInc"), ref str))
		{
			bossShoot.mMinXInc = StrToFloat(str);
		}
		if (GetAttribute(elem, _S("MaxSineXInc"), ref str))
		{
			bossShoot.mMaxXInc = StrToFloat(str);
		}
		GET_BOSS_ELEM(elem, _S("movespeed"), ref str);
		bossShoot.mSpeed = StrToFloat(str);
		bossShoot.mDefaultSpeed = bossShoot.mSpeed;
		if (GetAttribute(elem, _S("decminhover"), ref str))
		{
			bossShoot.mDecMinHover = StrToInt(str);
		}
		if (GetAttribute(elem, _S("decmaxhover"), ref str))
		{
			bossShoot.mDecMaxHover = StrToInt(str);
		}
		if (GetAttribute(elem, _S("decminfire"), ref str))
		{
			bossShoot.mDecMinFire = StrToInt(str);
		}
		if (GetAttribute(elem, _S("decmaxfire"), ref str))
		{
			bossShoot.mDecMaxFire = StrToInt(str);
		}
		while (mXMLParser.NextElement(ref elem))
		{
			if (elem.mType == 1)
			{
				if (StrEquals(elem.mValue, _S("DDS")) && !DoParseBossDDS(elem, l.mId, l))
				{
					return false;
				}
				if (StrEquals(elem.mValue, _S("Berserk")) && !DoParseBossBerserk(elem, boss))
				{
					return false;
				}
				if (StrEquals(elem.mValue, _S("Skeleton")) && !DoParseBossSkeletonEmitter(elem, boss as BossSkeleton))
				{
					return false;
				}
				if (StrEquals(elem.mValue, _S("Hint")) && !DoParseBossHintText(elem, boss))
				{
					return false;
				}
				if (StrEquals(elem.mValue, _S("Hula")) && !DoParseHula(elem, boss))
				{
					return false;
				}
				if (StrEquals(elem.mValue, _S("DeathText")))
				{
					if (!GetAttribute(elem, _S("line"), ref str) && !GetAttribute(elem, _S("value"), ref str))
					{
						return Fail("<DeathText> section needs a /line/ or /value/ tag");
					}
					boss.mDeathText.Add(new BossText(str));
				}
			}
			else if (elem.mType == 2 && StrEquals(elem.mValue, _S("Boss")))
			{
				break;
			}
		}
		return true;
	}

	protected bool DoParseHandheldBalance(BXMLElement elem)
	{
		string theValue = "";
		if (!GetAttribute(elem, _S("FruitPowerupDuration"), ref theValue))
		{
			return Fail("Expected /FruitPowerupDuration/ tag in <HandheldBalance> section");
		}
		float num = StrToFloat(theValue);
		if (!GetAttribute(elem, _S("SameColorChance"), ref theValue))
		{
			return Fail("Expected /SameColorChance/ tag in <HandheldBalance> section");
		}
		float num2 = StrToFloat(theValue);
		float[] array = new float[DDS.NUM_ADVENTURE_ZONES];
		for (int i = 1; i < DDS.NUM_ADVENTURE_ZONES + 1; i++)
		{
			string theName = "Adventure" + i;
			if (!GetAttribute(elem, theName, ref theValue))
			{
				return Fail("Expected /AdventureX/ tag in <HandheldBalance> section");
			}
			array[i - 1] = StrToFloat(theValue);
		}
		float[] array2 = new float[DDS.NUM_CHALLENGE_LEVELS];
		for (int j = 1; j < DDS.NUM_CHALLENGE_LEVELS + 1; j++)
		{
			string theName2 = "Challenge" + j;
			if (!GetAttribute(elem, theName2, ref theValue))
			{
				return Fail("Expected /ChallengeX/ tag in <HandheldBalance> section");
			}
			array2[j - 1] = StrToFloat(theValue);
		}
		GameApp.gDDS.mHandheldBalance.mFruitPowerupAdditionalDuration = 1f + num / 100f;
		GameApp.gDDS.mHandheldBalance.mChanceOfSameColorBallIncrease = 1f + num2 / 100f;
		for (uint num3 = 0u; num3 < DDS.NUM_ADVENTURE_ZONES; num3++)
		{
			GameApp.gDDS.mHandheldBalance.mAdventureModeSpeedDelta[num3] = array[num3] / 100f;
		}
		for (uint num4 = 0u; num4 < DDS.NUM_CHALLENGE_LEVELS; num4++)
		{
			GameApp.gDDS.mHandheldBalance.mChallengeModeSpeedDelta[num4] = array2[num4] / 100f;
		}
		return true;
	}

	protected bool DoParseBossSkeletonEmitter(BXMLElement elem, BossSkeleton bs)
	{
		if (bs == null)
		{
			return Fail("Found <Skeleton> section but boss isn't a skeleton boss");
		}
		string theValue = "";
		if (!GetAttribute(elem, _S("NumPerGroup"), ref theValue))
		{
			return Fail("Expected /NumPerGroup/ tag in <Skeleton> section");
		}
		bs.mNumSkeleToEmit = StrToInt(theValue);
		if (!GetAttribute(elem, _S("GroupDelay"), ref theValue))
		{
			return Fail("Expected /GroupDelay/ tag in <Skeleton> section");
		}
		bs.mSkeleDelay = StrToInt(theValue);
		if (!GetAttribute(elem, _S("DelayAfterEmit"), ref theValue))
		{
			return Fail("Expected /DelayAfterEmit/ tag in <Skeleton> section");
		}
		bs.mDelayAfterSkeleEmit = StrToInt(theValue);
		string theValue2 = "";
		string theValue3 = "";
		GetAttribute(elem, _S("vx"), ref theValue2);
		GetAttribute(elem, _S("vy"), ref theValue3);
		if (theValue2.Length == 0 && theValue3.Length == 0)
		{
			return Fail("Expected /vx/ or /vy/ tag in <Skeleton> section");
		}
		bs.mSkeletonVX = StrToFloat(theValue2);
		bs.mSkeletonVY = StrToFloat(theValue3);
		if (!GetAttribute(elem, _S("x"), ref theValue))
		{
			return Fail("Expected /x/ tag in <Skeleton> section");
		}
		bs.mSkeletonEmitX = StrToFloat(theValue);
		if (!GetAttribute(elem, _S("y"), ref theValue))
		{
			return Fail("Expected /y/ tag in <Skeleton> section");
		}
		bs.mSkeletonEmitY = StrToFloat(theValue);
		return true;
	}

	protected bool DoParseBossBerserk(BXMLElement elem, Boss b)
	{
		string theValue = "";
		if (!GetAttribute(elem, _S("value"), ref theValue))
		{
			return Fail("Expected /value/ tag in <Berserk> section");
		}
		string theValue2 = "";
		bool flag = StrEquals(theValue, _S("movement"));
		if (!flag && !GetAttribute(elem, _S("amount"), ref theValue2))
		{
			return Fail("Expected /amount/ tag in <Berserk> section");
		}
		string theValue3 = "";
		if (!GetAttribute(elem, _S("HealthLimit"), ref theValue3))
		{
			return Fail("Expected /HealthLimit/ tag in <Berserk> section");
		}
		int num = StrToInt(theValue3);
		string theValue4 = "";
		string theValue5 = "";
		string theValue6 = "";
		GetAttribute(elem, _S("min"), ref theValue4);
		GetAttribute(elem, _S("max"), ref theValue5);
		bool flag2 = false;
		if (GetAttribute(elem, _S("override"), ref theValue6))
		{
			flag2 = StrToBool(theValue6);
		}
		if (StrEquals(theValue, _S("ShotType")))
		{
			if (StrEquals(theValue2, _S("straight")))
			{
				theValue2 = _S(string.Concat(0));
			}
			else if (StrEquals(theValue2, _S("sine")))
			{
				theValue2 = _S(string.Concat(2));
			}
			else if (StrEquals(theValue2, _S("target")) || StrEquals(theValue3, _S("targeted")))
			{
				theValue2 = _S(string.Concat(1));
			}
			else if (StrEquals(theValue2, _S("homing")))
			{
				theValue2 = _S(string.Concat(3));
			}
			else if (StrEquals(theValue2, _S("volcano")))
			{
				theValue2 = _S(string.Concat(4));
			}
			else if (StrEquals(theValue2, _S("any")) || StrEquals(theValue3, _S("all")))
			{
				theValue2 = _S(string.Concat(5));
			}
		}
		if (!flag)
		{
			string minval = ToString(theValue4);
			string maxval = ToString(theValue5);
			b.AddBerserkValue(num, theValue, ToString(theValue2), ref minval, ref maxval, flag2);
			theValue4 = ToString(minval);
			theValue5 = ToString(maxval);
		}
		else
		{
			BossBerserkMovement bossBerserkMovement = new BossBerserkMovement();
			if (GetAttribute(elem, _S("startx"), ref theValue3))
			{
				bossBerserkMovement.mStartX = StrToInt(theValue3);
			}
			if (GetAttribute(elem, _S("endx"), ref theValue3))
			{
				bossBerserkMovement.mEndX = StrToInt(theValue3);
			}
			if (GetAttribute(elem, _S("starty"), ref theValue3))
			{
				bossBerserkMovement.mStartY = StrToInt(theValue3);
			}
			if (GetAttribute(elem, _S("endy"), ref theValue3))
			{
				bossBerserkMovement.mEndY = StrToInt(theValue3);
			}
			if (GetAttribute(elem, _S("x"), ref theValue3))
			{
				bossBerserkMovement.mX = StrToInt(theValue3);
			}
			if (GetAttribute(elem, _S("y"), ref theValue3))
			{
				bossBerserkMovement.mY = StrToInt(theValue3);
			}
			for (int i = 1; GetAttribute(elem, _S("x" + i), ref theValue3); i++)
			{
				Point point = new Point();
				point.mX = StrToInt(theValue3);
				if (!GetAttribute(elem, _S("y" + i), ref theValue3))
				{
					return Fail("You must have an x<num> for every y<num> and v.v (error in <Berserk value=\"movement\"...> tag)");
				}
				point.mY = StrToInt(theValue3);
				bossBerserkMovement.mPoints.Add(point);
			}
			if (!(b is BossShoot bossShoot))
			{
				return Fail("Unable to cast from Boss* to BossShoot* while parsing <Berserk value=\"movement\"...> tag.");
			}
			bossBerserkMovement.mHealthLimit = num;
			bossShoot.AddBerserkMovement(bossBerserkMovement);
			b.AddBerserkValue(num, _S(""), "");
		}
		return true;
	}

	protected bool DoParseBossHintText(BXMLElement elem, Boss b)
	{
		string theValue = "";
		if (!GetAttribute(elem, _S("text"), ref theValue))
		{
			return Fail("Expected /text/ tag in <Hint> section");
		}
		TauntText tauntText = new TauntText();
		tauntText.mText = theValue;
		if (!GetAttribute(elem, _S("condition"), ref theValue))
		{
			return Fail("Expected /condition/ tag in <Hint> section");
		}
		tauntText.mCondition = StrToInt(theValue);
		if (!GetAttribute(elem, _S("deaths"), ref theValue))
		{
			return Fail("Expected /deaths/ tag in <Hint> section");
		}
		tauntText.mMinDeaths = StrToInt(theValue);
		if (!GetAttribute(elem, _S("delay"), ref theValue))
		{
			return Fail("Expected /delay/ tag in <Hint> section");
		}
		tauntText.mDelay = StrToInt(theValue);
		theValue = _S("0");
		if (tauntText.mCondition > 0 && !GetAttribute(elem, _S("mintime"), ref theValue))
		{
			return Fail("Expected /mintime/ tag for <Hint> sections using a /condition/ value greater than 0");
		}
		tauntText.mMinTime = StrToInt(theValue);
		b.mTauntText.Add(tauntText);
		return true;
	}

	protected bool DoParseHula(BXMLElement elem, Boss b)
	{
		string theValue = "";
		if (!GetAttribute(elem, _S("vx"), ref theValue))
		{
			return false;
		}
		float vx = StrToFloat(theValue);
		if (!GetAttribute(elem, _S("projvy"), ref theValue))
		{
			return false;
		}
		float projvy = StrToFloat(theValue);
		if (!GetAttribute(elem, _S("spawn"), ref theValue))
		{
			return false;
		}
		int spawn = StrToInt(theValue);
		if (!GetAttribute(elem, _S("spawny"), ref theValue))
		{
			return false;
		}
		int spawny = StrToInt(theValue);
		if (!GetAttribute(elem, _S("projchance"), ref theValue))
		{
			return false;
		}
		int proj_chance = StrToInt(theValue);
		int proj_range = (GetAttribute(elem, _S("projrange"), ref theValue) ? StrToInt(theValue) : 0);
		if (!GetAttribute(elem, _S("berserk"), ref theValue))
		{
			return false;
		}
		int berserk_amt = StrToInt(theValue);
		int atime = 0;
		int atype = 0;
		if (GetAttribute(elem, _S("hallucinate"), ref theValue))
		{
			atime = StrToInt(theValue);
			atype = 3;
		}
		else if (GetAttribute(elem, _S("stun"), ref theValue))
		{
			atime = StrToInt(theValue);
			atype = 1;
		}
		else if (GetAttribute(elem, _S("poison"), ref theValue))
		{
			atime = StrToInt(theValue);
			atype = 2;
		}
		else if (GetAttribute(elem, _S("slow"), ref theValue))
		{
			atime = StrToInt(theValue);
			atype = 4;
		}
		int amnesty = 0;
		if (GetAttribute(elem, _S("amnesty"), ref theValue))
		{
			amnesty = StrToInt(theValue);
		}
		b.AddHulaEntry(vx, projvy, spawn, spawny, proj_chance, berserk_amt, proj_range, atype, atime, amnesty);
		return true;
	}

	protected void CopyLevel(Level src, ref Level dst, Board b)
	{
		dst = src.Instantiate();
		for (int i = 0; i < dst.mNumCurves; i++)
		{
			dst.mCurveMgr[i] = new CurveMgr(b, src.mCurveMgr[i].mCurveNum);
			dst.mCurveMgr[i].Copy(src.mCurveMgr[i], dst, b);
		}
		if (src.mBoss != null)
		{
			dst.mBoss = src.mBoss.Instantiate();
			dst.mBoss.mName = src.mDisplayName;
			dst.mBoss.PostInstantiationHook(src.mBoss);
			dst.mBoss.mLevel = dst;
			dst.mOrgBoss = dst.mBoss;
		}
		if (src.mSecondaryBoss != null)
		{
			dst.mSecondaryBoss = src.mSecondaryBoss.Instantiate();
			dst.mSecondaryBoss.mName = src.mDisplayName;
			dst.mSecondaryBoss.PostInstantiationHook(src.mSecondaryBoss);
			dst.mSecondaryBoss.mLevel = dst;
		}
		dst.mHoleMgr = new HoleMgr(src.mHoleMgr);
		dst.CopyFrom(src);
		for (int j = 0; j < dst.mNumCurves; j++)
		{
			if (!dst.mCurveMgr[j].mIsLoaded)
			{
				dst.mCurveMgr[j].LoadCurve();
			}
		}
	}

	public LevelMgr()
	{
		mBeatGamePointsForLife = 1000;
		mGauntletNumForMultBase = 2;
		mGauntletNumForMultInc = 0;
		mMultiplierDuration = 100;
		mHasFailed = false;
		mNumDDSTiers = 0;
		mXMLParser = null;
		mCannonStacks = true;
		mLazerStacks = false;
		mBossTauntChance = 0;
		mMultBallLife = 1000;
		mMultBallPoints = 100;
		mGauntletTFreq = 1000;
		mCannonShots = (mLazerShots = 3);
		mPowerDelay = 1500;
		mPowerCooldown = 1000;
		mPowerupIncAtZumaPct = 1f;
		mPowerIncPct = 0f;
		mMaxGauntletNumForMult = 0;
		mMultiplierTimeAdd = 0;
		mPointTimeAdd = (mNumPointsForTimeAdd = 0);
		mAllowColorNukeAfterZuma = true;
		mColorNukeTimeAfterZuma = -1;
		mBossesCanAttackFuckedFrog = true;
		mAttackDelayAfterHittingFrog = 0;
		mPointsForBronze = 5000;
		mPointsForSilver = 10000;
		mPointsForGold = 15000;
		mUniquePowerupColor = false;
		mCapAffectsPowerupsSpawned = true;
		mFirstIronFrogLevel = (mLastIronFrogLevel = -1);
		mEffectManager = null;
		mPowerupSpawnDelay = 0;
		mMaxZumaPctForColorNuke = -1f;
		mClearCurveRolloutPct = 0.1f;
		mGauntletSessionLength = 0;
		mIsHardConfig = false;
		mClearCurvePoints = 1000;
		mMinMultBallDistance = 0f;
		mPointsForLife = 10000;
		mPostZumaTime = 0;
		mPostZumaTimeSlowInc = (mPostZumaTimeSpeedInc = 0f);
		MapScreen.gZoneNames[0] = TextManager.getInstance().getString(839);
		MapScreen.gZoneNames[1] = TextManager.getInstance().getString(840);
		MapScreen.gZoneNames[2] = TextManager.getInstance().getString(841);
		MapScreen.gZoneNames[3] = TextManager.getInstance().getString(842);
		MapScreen.gZoneNames[4] = TextManager.getInstance().getString(843);
		MapScreen.gZoneNames[5] = TextManager.getInstance().getString(844);
	}

	public void Init()
	{
		mEffectManager = new EffectManager();
	}

	public virtual void Dispose()
	{
		mXMLParser = null;
		mEffectManager = null;
		Reset();
	}

	public void Reset()
	{
		gBossNum = 0;
		mError = "";
		mNumDDSTiers = 0;
		mDDSSlowAdd.Clear();
		mDDSPowerupPctInc.Clear();
		mDDSZumaPointDecPct.Clear();
		mDDSSpeedPct.Clear();
		mHasFailed = false;
		mLevels.Clear();
		mCannonStacks = true;
		mLazerStacks = false;
		mCannonShots = (mLazerShots = 3);
		mEffectManager.Reset();
	}

	public bool doLoadLevels()
	{
		bool flag = false;
		BXMLElement theElement = new BXMLElement();
		while (!mXMLParser.HasFailed())
		{
			if (!mXMLParser.NextElement(ref theElement))
			{
				Fail("Failed loading levels");
				mXMLParser = null;
				return false;
			}
			if (theElement.mType != 1)
			{
				continue;
			}
			if (theElement.mValue != "Config")
			{
				break;
			}
			flag = DoParseLevels();
			mXMLParser = null;
			mFirstIronFrogLevel = (mLastIronFrogLevel = -1);
			if (flag)
			{
				for (int i = 0; i < mLevels.size(); i++)
				{
					if (mFirstIronFrogLevel == -1 && mLevels[i].mIronFrog)
					{
						mFirstIronFrogLevel = i;
					}
					else if (mLevels[i].mIronFrog && i > mLastIronFrogLevel)
					{
						mLastIronFrogLevel = i;
					}
					if (mFirstIronFrogLevel != -1 && mLevels[i].mIronFrog)
					{
						mLevels[i].mZone = 7;
						mLevels[i].mNum = i - mFirstIronFrogLevel + 1;
					}
				}
			}
			return flag;
		}
		Fail("Expecting Config tag");
		flag = DoParseLevels();
		mXMLParser = null;
		return flag;
	}

	public bool LoadLevels(string theFilename)
	{
		mLevels.Clear();
		mHasFailed = false;
		mError = "";
		mXMLParser = new BXMLParser();
		string filename = theFilename + ".dat";
		mXMLParser.OpenStream(filename);
		return doLoadLevels();
	}

	public bool LoadLevels(byte[] data)
	{
		mLevels.Clear();
		mHasFailed = false;
		mError = "";
		mXMLParser = new BXMLParser();
		SexyFramework.Misc.Buffer buffer = new SexyFramework.Misc.Buffer();
		buffer.SetData(data, data.Length);
		mXMLParser.OpenBuffer(buffer);
		return doLoadLevels();
	}

	public string GetErrorText()
	{
		return mError;
	}

	public bool HadError()
	{
		return mError.Length != 0;
	}

	public static string GetZoneName(int zone_num)
	{
		return MapScreen.gZoneNames[zone_num];
	}

	public static string GetTerseZoneName(int zone_num)
	{
		return zone_num switch
		{
			1 => "Jungle", 
			2 => "Village", 
			3 => "City", 
			4 => "Coast", 
			5 => "Grotto", 
			6 => "Volcano", 
			7 => "Iron Frog", 
			_ => "", 
		};
	}

	public int GetScoreTipIdx(int level_num)
	{
		List<int> list = new List<int>();
		for (int i = 7; i < mScoreTips.Count(); i++)
		{
			if (mScoreTips[i].mMinLevel <= level_num)
			{
				list.Add(i);
			}
		}
		if (list.Count() == 0)
		{
			return 0;
		}
		return list[MathUtils.SafeRand() % list.Count()];
	}

	public bool GetLevelById(string id, ref Level desc, Board b)
	{
		for (int i = 0; i < mLevels.Count(); i++)
		{
			if (Common.StrICaseEquals(id, mLevels[i].mId))
			{
				CopyLevel(mLevels[i], ref desc, b);
				return true;
			}
		}
		return false;
	}

	public bool GetLevelByIndex(int index, ref Level desc, Board b)
	{
		if (index < 0 || index >= mLevels.Count())
		{
			return false;
		}
		CopyLevel(mLevels[index], ref desc, b);
		return true;
	}

	public Level GetLevelByIndex(int index)
	{
		if (index < 0 || index >= mLevels.Count())
		{
			return null;
		}
		return mLevels[index];
	}

	public Level GetLevelById(string id)
	{
		for (int i = 0; i < mLevels.Count(); i++)
		{
			if (Common.StrICaseEquals(id, mLevels[i].mId))
			{
				return mLevels[i];
			}
		}
		return null;
	}

	public string GetLevelId(int index)
	{
		if (index < 0 || index >= mLevels.Count())
		{
			return "";
		}
		return mLevels[index].mId;
	}

	public Level GetLevelByZone(int zone, int num)
	{
		for (int i = 0; i < mLevels.Count(); i++)
		{
			if (mLevels[i].mZone == zone && mLevels[i].mNum == num)
			{
				return mLevels[i];
			}
		}
		return null;
	}

	public int GetLevelIndex(string id)
	{
		for (int i = 0; i < mLevels.Count(); i++)
		{
			if (Common.StrEquals(id, mLevels[i].mId))
			{
				return i;
			}
		}
		return -1;
	}

	public int GetStartingGauntletLevel(string id)
	{
		for (int i = 0; i < mLevels.Count(); i++)
		{
			if (Common.StrEquals(id, mLevels[i].mId))
			{
				return mLevels[i].mStartingGauntletLevel;
			}
		}
		return -1;
	}

	public void GetZoneInfo(string id, out int zone, out int levelnum)
	{
		for (int i = 0; i < mLevels.Count(); i++)
		{
			if (Common.StrEquals(id, mLevels[i].mId))
			{
				zone = mLevels[i].mZone;
				levelnum = mLevels[i].mNum;
				return;
			}
		}
		zone = -1;
		levelnum = -1;
	}

	public string GetZoneFruitId(int zone)
	{
		if (zone > 6)
		{
			zone = 6;
		}
		return mZones[zone - 1].mFruitId;
	}

	public string GetZoneStartId(int zone)
	{
		return zones[zone - 1];
	}

	public int GetFirstIronFrogLevel()
	{
		return mFirstIronFrogLevel;
	}

	public int GetLastIronFrogLevel()
	{
		return mLastIronFrogLevel;
	}

	public bool GetLevelStrData(int index, ref string level_id, ref string level_disp_name)
	{
		if (index < 0 || index >= mLevels.Count())
		{
			return false;
		}
		if (level_id != null)
		{
			level_id = mLevels[index].mId;
		}
		if (level_disp_name != null)
		{
			level_disp_name = mLevels[index].mDisplayName;
		}
		return true;
	}

	private bool GetAttribute(BXMLElement elem, string theName, ref string theValue)
	{
		string key = theName.ToLower();
		if (elem.mAttributes.ContainsKey(key))
		{
			theValue = elem.mAttributes[key];
			return true;
		}
		return false;
	}

	private float StrToFloat(string str)
	{
		if (str == "")
		{
			return 0f;
		}
		return float.Parse(str, NumberStyles.Float, CultureInfo.InvariantCulture);
	}

	private int StrToInt(string str)
	{
		if (str == "")
		{
			return 0;
		}
		return int.Parse(str);
	}

	private bool StrToBool(string str)
	{
		if (str == "")
		{
			return false;
		}
		return bool.Parse(str);
	}

	private string ToString(string str)
	{
		return str;
	}

	private string _S(string str)
	{
		return str;
	}

	private int sexyatoi(string str)
	{
		return StrToInt(str);
	}

	private float sexyatof(string str)
	{
		return StrToFloat(str);
	}

	private bool StrEquals(string str, string cmp)
	{
		return str == cmp;
	}

	private string StringToUpper(string str)
	{
		return str.ToUpper();
	}

	private string StringToLower(string str)
	{
		return str.ToLower();
	}

	private bool COMMON_WALL_FAIL(string str)
	{
		return Fail("Unabled to find \"" + str + "\" parameter in \"Wall\" or \"MovingWall\" tag");
	}

	private bool GET_BOSS_ELEM(BXMLElement elem, string ename, ref string str)
	{
		if (!GetAttribute(elem, ename, ref str))
		{
			return Fail("Unable to find \"" + ToString(str) + "\" in \"Boss\" tag");
		}
		return true;
	}

	private bool GET_DESTPT_ELEM(BXMLElement elem, string ename, ref string str)
	{
		if (!GetAttribute(elem, ename, ref str))
		{
			return Fail("Unable to find \"" + ToString(str) + "\" in \"Boss\".\"Waypoint\" tag");
		}
		return true;
	}

	private bool GET_ATTACK_ELEM(BXMLElement elem, string ename, ref string str)
	{
		if (!GetAttribute(elem, ename, ref str))
		{
			return Fail("Unable to find \"" + ToString(str) + "\" in \"Boss\".\"Atttack\" tag");
		}
		return true;
	}

	private bool GET_GAUNTLET_ELEM(BXMLElement elem, string ename, ref string str)
	{
		if (!GetAttribute(elem, ename, ref str))
		{
			return Fail("Unable to find \"" + ename + "\" in \"Difficulty\" tag for Gauntlet mode setting");
		}
		return true;
	}
}
