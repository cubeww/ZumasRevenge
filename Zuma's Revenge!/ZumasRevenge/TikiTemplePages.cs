using System.Collections.Generic;
using System.Text;
using JeffLib;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class TikiTemplePages : Widget
{
	private enum PageInfo
	{
		TikiTemple_PageStats,
		TikiTemple_PageMoreStats,
		TikiTemple_PageChallenge,
		TikiTemple_PageAdventure
	}

	private List<TempleText> mText;

	private Font mHeaderFont;

	private Font mStatsFont;

	private TikiTemple mTikiTemple;

	private int mNumPages;

	private Image IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES);

	private Image IMAGE_UI_CHALLENGESCREEN_DIVIDER = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_DIVIDER);

	public TikiTemplePages(TikiTemple theTikiTemple)
	{
		mTikiTemple = theTikiTemple;
		mNumPages = 0;
		mHeaderFont = Res.GetFontByID(ResID.FONT_SHAGEXOTICA38_BLACK_GLOW);
		mStatsFont = Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_STROKE);
		mText = new List<TempleText>();
		AddPage(PageInfo.TikiTemple_PageAdventure);
		AddPage(PageInfo.TikiTemple_PageChallenge);
		AddPage(PageInfo.TikiTemple_PageStats);
		AddPage(PageInfo.TikiTemple_PageMoreStats);
		for (int i = 0; i < mText.Count; i++)
		{
			mText[i].mAlpha = 255f;
		}
		Resize(0, 0, (IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetWidth() + 30) * mNumPages, IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetHeight());
	}

	public int NumPages()
	{
		return mNumPages;
	}

	private void AddPage(PageInfo thePage)
	{
		int theStartX = (IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetWidth() + 30) * mNumPages + (int)mTikiTemple.GetTitleXOffset() + 55;
		switch (thePage)
		{
		case PageInfo.TikiTemple_PageStats:
			SetupStatsText(ref theStartX);
			break;
		case PageInfo.TikiTemple_PageChallenge:
			SetupChallengeText(ref theStartX);
			break;
		case PageInfo.TikiTemple_PageAdventure:
			SetupAdventureText(hard_mode: false, ref theStartX);
			break;
		case PageInfo.TikiTemple_PageMoreStats:
			SetupMoreStatsText(ref theStartX);
			break;
		}
		mNumPages++;
	}

	public override void Draw(Graphics g)
	{
		for (int i = 0; i < mText.Count; i++)
		{
			TempleText templeText = mText[i];
			if (templeText.mValueStr.Length == 0)
			{
				g.SetFont(mHeaderFont);
				g.SetColor(255, 249, 161);
				g.DrawString(templeText.mHeaderStr, templeText.mX + (int)mTikiTemple.mXOff, templeText.mY + g.GetFont().GetAscent());
			}
			else
			{
				g.SetFont(mStatsFont);
				g.SetColor(166, 158, 255);
				g.WriteString(templeText.mHeaderStr, templeText.mX + (int)mTikiTemple.mXOff, templeText.mY + mStatsFont.GetAscent(), 0, 1);
				g.SetColor(89, 187, 149);
				g.WriteString(templeText.mValueStr, templeText.mX + Common._DS(Common._M(20)) + (int)mTikiTemple.mXOff, templeText.mY + mStatsFont.GetAscent(), mWidth, -1);
			}
		}
		int num = NumPages() - 1;
		int num2 = IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetWidth() + 30;
		int theY = (IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetHeight() - IMAGE_UI_CHALLENGESCREEN_DIVIDER.GetHeight()) / 2;
		for (int j = 0; j < num; j++)
		{
			g.DrawImage(IMAGE_UI_CHALLENGESCREEN_DIVIDER, num2 - IMAGE_UI_CHALLENGESCREEN_DIVIDER.GetWidth(), theY);
			num2 += IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetWidth() + 30;
		}
	}

	public override void Update()
	{
		float num = Common._M(10f);
		for (int i = 0; i < mText.size(); i++)
		{
			TempleText templeText = mText[i];
			if (templeText.mFadeIn && templeText.mAlpha < 255f)
			{
				MarkDirty();
				templeText.mAlpha += num;
				if (templeText.mAlpha > 255f)
				{
					templeText.mAlpha = 255f;
				}
			}
			else if (!templeText.mFadeIn && templeText.mAlpha > 0f)
			{
				MarkDirty();
				templeText.mAlpha -= num;
				if (templeText.mAlpha <= 0f)
				{
					mText.RemoveAt(i);
					i--;
				}
			}
		}
	}

	private void SetupIronFrogText(ref int theStartX)
	{
		int num = Common._DS(Common._M(832)) - mX + (GameApp.gApp.GetScreenWidth() - GameApp.gApp.mScreenBounds.mWidth);
		int num2 = Common._DS(Common._M(6)) + mStatsFont.mHeight;
		IronFrogTempleStats mIronFrogStats = GameApp.gApp.mUserProfile.mIronFrogStats;
		mText.Add(new TempleText());
		TempleText templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(791);
		templeText.mX = (GameApp.gApp.GetScreenWidth() - mHeaderFont.StringWidth(templeText.mHeaderStr)) / 2;
		templeText.mY = Common._DS(Common._M(64));
		int num3 = Common._DS(Common._M(416));
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(792);
		templeText.mValueStr = $"{mIronFrogStats.mNumAttempts:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(793);
		templeText.mValueStr = $"{mIronFrogStats.mNumVictories:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(794);
		templeText.mValueStr = ((mIronFrogStats.mBestTime == 0) ? "None" : JeffLib.Common.UpdateToTimeStr(mIronFrogStats.mBestTime, use_hour_field: false));
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(795);
		templeText.mValueStr = SexyFramework.Common.CommaSeperate(mIronFrogStats.mBestScore);
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(796);
		templeText.mValueStr = $"{mIronFrogStats.mHighestLevel:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(797);
		int num4 = 0;
		int num5 = -1;
		for (int i = 0; i < 10; i++)
		{
			if (mIronFrogStats.mLevelDeaths[i] > num4)
			{
				num4 = mIronFrogStats.mLevelDeaths[i];
				num5 = i;
			}
		}
		if (num5 == -1)
		{
			templeText.mValueStr = TextManager.getInstance().getString(771);
		}
		else
		{
			StringBuilder stringBuilder = new StringBuilder(TextManager.getInstance().getString(798));
			stringBuilder.Replace("$1", (num5 + 1).ToString());
			stringBuilder.Replace("$2", num4.ToString());
			templeText.mValueStr = stringBuilder.ToString();
		}
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(799);
		templeText.mValueStr = JeffLib.Common.UpdateToTimeStr(mIronFrogStats.mTotalTimePlayed, use_hour_field: true);
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
	}

	private void SetupStatsText(ref int theStartX)
	{
		int num = theStartX + Common._DS(Common._M(732));
		int num2 = Common._DS(Common._M(6)) + mStatsFont.mHeight;
		_ = GameApp.gApp.mUserProfile.mChallengeStats;
		mText.Add(new TempleText());
		TempleText templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(800);
		templeText.mX = theStartX - Common._DS(60) + (IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetWidth() + 30 - mHeaderFont.StringWidth(templeText.mHeaderStr)) / 2;
		templeText.mY = Common._DS(Common._M(64));
		int num3 = Common._DS(Common._M(140));
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(801);
		templeText.mValueStr = $"{GameApp.gApp.mUserProfile.mLargestChainShot:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(802);
		templeText.mValueStr = $"{GameApp.gApp.mUserProfile.mLargestCombo:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(803);
		templeText.mValueStr = $"{GameApp.gApp.mUserProfile.mHighestGapShotScore:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(804);
		templeText.mValueStr = $"{GameApp.gApp.mUserProfile.mNumGapShots:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(805);
		templeText.mValueStr = $"{GameApp.gApp.mUserProfile.mNumDoubleGapShots:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(806);
		templeText.mValueStr = $"{GameApp.gApp.mUserProfile.mNumTripleGapShots:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(807);
		templeText.mValueStr = $"{GameApp.gApp.mUserProfile.mNumFruits:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(808);
		templeText.mValueStr = $"{GameApp.gApp.mUserProfile.mNumTimesActivatedPowerup[0]:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(809);
		templeText.mValueStr = $"{GameApp.gApp.mUserProfile.mNumTimesActivatedPowerup[9]:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(810);
		templeText.mValueStr = $"{GameApp.gApp.mUserProfile.mNumTimesActivatedPowerup[8]:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(811);
		templeText.mValueStr = $"{GameApp.gApp.mUserProfile.mNumTimesActivatedPowerup[7]:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
	}

	private void SetupMoreStatsText(ref int theStartX)
	{
		int num = theStartX + Common._DS(Common._M(732));
		int num2 = Common._DS(Common._M(6)) + mStatsFont.mHeight;
		_ = GameApp.gApp.mUserProfile.mChallengeStats;
		mText.Add(new TempleText());
		TempleText templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(812);
		templeText.mX = theStartX - Common._DS(60) + (IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetWidth() + 30 - mHeaderFont.StringWidth(templeText.mHeaderStr)) / 2;
		templeText.mY = Common._DS(Common._M(64));
		int num3 = Common._DS(Common._M(140));
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(813);
		templeText.mValueStr = $"{GameApp.gApp.mUserProfile.mBallsSwapped:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(814);
		templeText.mValueStr = $"{GameApp.gApp.mUserProfile.mBallsFired:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		int num4 = 0;
		int t = 0;
		for (int i = 0; i < 14; i++)
		{
			if (!Common.IsDeprecatedPowerUp((PowerType)i) && GameApp.gApp.mUserProfile.mNumTimesActivatedPowerup[i] > num4)
			{
				num4 = GameApp.gApp.mUserProfile.mNumTimesActivatedPowerup[i];
				t = i;
			}
		}
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(815);
		templeText.mValueStr = ((num4 <= 0) ? TextManager.getInstance().getString(771) : $"{Common.PowerupToStr((PowerType)t, all_caps: false)} ({num4:D}x)");
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(816);
		templeText.mValueStr = SexyFramework.Common.CommaSeperate(GameApp.gApp.mUserProfile.mPointsFromCombos);
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(817);
		templeText.mValueStr = SexyFramework.Common.CommaSeperate(GameApp.gApp.mUserProfile.mPointsFromChainShots);
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(818);
		templeText.mValueStr = SexyFramework.Common.CommaSeperate(GameApp.gApp.mUserProfile.mPointsFromGapShots);
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		int theValue = GameApp.gApp.mUserProfile.mPointsFromCannon + GameApp.gApp.mUserProfile.mPointsFromColorNuke + GameApp.gApp.mUserProfile.mPointsFromLaser + GameApp.gApp.mUserProfile.mPointsFromProxBomb;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(819);
		templeText.mValueStr = SexyFramework.Common.CommaSeperate(theValue);
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		int u = GameApp.gApp.mUserProfile.mAdventureStats.mTotalTimePlayed + GameApp.gApp.mUserProfile.mHeroicStats.mTotalTimePlayed + GameApp.gApp.mUserProfile.mIronFrogStats.mTotalTimePlayed + GameApp.gApp.mUserProfile.mChallengeStats.mTotalTime;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(820);
		templeText.mValueStr = JeffLib.Common.UpdateToTimeStr(u, use_hour_field: true);
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(821);
		templeText.mValueStr = $"{GameApp.gApp.mUserProfile.mFruitBombed:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(822);
		templeText.mValueStr = $"{GameApp.gApp.mUserProfile.mBallsTossed:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(823);
		templeText.mValueStr = $"{GameApp.gApp.mUserProfile.mDeathsAfterZuma:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
	}

	private void SetupChallengeText(ref int theStartX)
	{
		int num = theStartX + Common._DS(Common._M(732));
		int num2 = Common._DS(Common._M(6)) + mStatsFont.mHeight;
		ChallengeTempleStats mChallengeStats = GameApp.gApp.mUserProfile.mChallengeStats;
		mText.Add(new TempleText());
		TempleText templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(782);
		templeText.mX = theStartX - Common._DS(60) + (IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetWidth() + 30 - mHeaderFont.StringWidth(templeText.mHeaderStr)) / 2;
		templeText.mY = Common._DS(Common._M(64));
		int num3 = Common._DS(Common._M(140));
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(783);
		templeText.mValueStr = SexyFramework.Common.CommaSeperate(mChallengeStats.mHighestScore);
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		int num4 = 0;
		int num5 = 0;
		for (int i = 0; i < 7; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				if (GameApp.gApp.mUserProfile.mChallengeUnlockState[i, j] == 4)
				{
					num4++;
				}
				else if (GameApp.gApp.mUserProfile.mChallengeUnlockState[i, j] == 5)
				{
					num4++;
					num5++;
				}
			}
		}
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(784);
		templeText.mValueStr = $"{num4:D} / 60";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(785);
		templeText.mValueStr = $"{num5:D} / 60";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		int num6 = 0;
		int num7 = 0;
		for (int k = 1; k < 7; k++)
		{
			if (GameApp.gApp.mUserProfile.ChallengeCupComplete(k) == 2)
			{
				num6++;
				num7++;
			}
			else if (GameApp.gApp.mUserProfile.ChallengeCupComplete(k) == 1)
			{
				num6++;
			}
		}
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(786);
		templeText.mValueStr = $"{num6:D} / 6";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(787);
		templeText.mValueStr = $"{num7:D} / 6";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(788);
		templeText.mValueStr = $"x{mChallengeStats.mHighestMult:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(789);
		int num8 = 0;
		int num9 = 0;
		for (int l = 0; l < 70; l++)
		{
			if (mChallengeStats.mNumTimesPlayedCurve[l] > num8)
			{
				num8 = mChallengeStats.mNumTimesPlayedCurve[l];
				num9 = l + 1;
			}
		}
		StringBuilder stringBuilder = new StringBuilder(TextManager.getInstance().getString(790));
		stringBuilder.Replace("$1", num9.ToString());
		stringBuilder.Replace("$2", num8.ToString());
		templeText.mValueStr = stringBuilder.ToString();
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(780);
		templeText.mValueStr = JeffLib.Common.UpdateToTimeStr(mChallengeStats.mTotalTime, use_hour_field: true);
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
	}

	private void SetupAdventureText(bool hard_mode, ref int theStartX)
	{
		int num = theStartX + Common._DS(Common._M(732));
		int num2 = Common._DS(Common._M(6)) + mStatsFont.mHeight;
		AdvModeTempleStats advModeTempleStats = (hard_mode ? GameApp.gApp.mUserProfile.mHeroicStats : GameApp.gApp.mUserProfile.mAdventureStats);
		mText.Add(new TempleText());
		TempleText templeText = mText.back();
		templeText.mHeaderStr = (hard_mode ? TextManager.getInstance().getString(766) : TextManager.getInstance().getString(767));
		templeText.mX = theStartX - Common._DS(60) + (IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetWidth() + 30 - mHeaderFont.StringWidth(templeText.mHeaderStr)) / 2;
		templeText.mY = Common._DS(Common._M(64));
		int num3 = Common._DS(Common._M(140));
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(768);
		templeText.mValueStr = $"{advModeTempleStats.mHighestLevel:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(769);
		templeText.mValueStr = ((advModeTempleStats.mBestTime <= 0 || advModeTempleStats.mBestTime == int.MaxValue) ? TextManager.getInstance().getString(771) : JeffLib.Common.UpdateToTimeStr(advModeTempleStats.mBestTime, use_hour_field: true));
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(770);
		templeText.mValueStr = ((advModeTempleStats.mBestScore <= 0) ? TextManager.getInstance().getString(771) : SexyFramework.Common.CommaSeperate(advModeTempleStats.mBestScore));
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(772);
		templeText.mValueStr = $"{advModeTempleStats.mNumLevelsAced:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(773);
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		for (int i = 0; i < 60; i++)
		{
			num4 += advModeTempleStats.mLevelDeaths[i];
			if (advModeTempleStats.mLevelDeaths[i] > num6)
			{
				num6 = advModeTempleStats.mLevelDeaths[i];
				num5 = i + 1;
			}
		}
		templeText.mValueStr = $"{num4:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(774);
		templeText.mValueStr = $"{advModeTempleStats.mNumPerfectLevels:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(775);
		templeText.mValueStr = $"{advModeTempleStats.mNumClearCurves:D}";
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(776);
		int num7 = 0;
		int num8 = 0;
		for (int j = 0; j < 6; j++)
		{
			if (advModeTempleStats.mBossDeaths[j] > num8)
			{
				num7 = j + 1;
				num8 = advModeTempleStats.mBossDeaths[j];
			}
		}
		if (num7 == 0)
		{
			templeText.mValueStr = TextManager.getInstance().getString(771);
		}
		else
		{
			StringBuilder stringBuilder = new StringBuilder(TextManager.getInstance().getString(778));
			stringBuilder.Replace("$1", num7.ToString());
			stringBuilder.Replace("$2", num8.ToString());
			templeText.mValueStr = stringBuilder.ToString();
		}
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(777);
		if (num5 == 0)
		{
			templeText.mValueStr = TextManager.getInstance().getString(771);
		}
		else
		{
			StringBuilder stringBuilder2 = new StringBuilder(TextManager.getInstance().getString(779));
			stringBuilder2.Replace("$1", num5.ToString());
			stringBuilder2.Replace("$2", num6.ToString());
			templeText.mValueStr = stringBuilder2.ToString();
		}
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
		mText.Add(new TempleText());
		templeText = mText.back();
		templeText.mHeaderStr = TextManager.getInstance().getString(780);
		templeText.mValueStr = JeffLib.Common.UpdateToTimeStr(advModeTempleStats.mTotalTimePlayed, use_hour_field: true);
		templeText.mX = num;
		templeText.mY = num3;
		num3 += num2;
	}
}
