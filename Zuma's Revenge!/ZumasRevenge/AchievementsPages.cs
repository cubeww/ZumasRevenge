using System.Collections.Generic;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;
using ZumasRevenge.Achievement;

namespace ZumasRevenge;

public class AchievementsPages : Widget
{
	private List<AchievementText> mText;

	private Font mHeaderFont;

	private Font mStatsFont;

	private Font mDescFont;

	private Font mPointFont;

	private Achievements mAchievements;

	public int mNumPages;

	private Image IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES);

	private Image IMAGE_UI_CHALLENGESCREEN_DIVIDER = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_DIVIDER);

	public Image IMAGE_UI_LEADERBOARDS_SHADOW = Res.GetImageByID(ResID.IMAGE_UI_LEADERBOARDS_SHADOW2);

	public AchievementsPages(Achievements theAchievements)
	{
		mAchievements = theAchievements;
		mNumPages = 0;
		mHeaderFont = Res.GetFontByID(ResID.FONT_SHAGLOUNGE45_YELLOW);
		mStatsFont = Res.GetFontByID(ResID.FONT_SHAGEXOTICA38_BLACK_GLOW);
		mDescFont = Res.GetFontByID(ResID.FONT_SHAGEXOTICA38_BLACK);
		mPointFont = Res.GetFontByID(ResID.FONT_SHAGLOUNGE45_GAUNTLET);
		mText = new List<AchievementText>();
		for (int i = 0; i < mText.Count; i++)
		{
			mText[i].mAlpha = 255f;
		}
		Resize(0, 0, IMAGE_UI_LEADERBOARDS_SHADOW.GetWidth(), (IMAGE_UI_LEADERBOARDS_SHADOW.GetHeight() + 30) * mNumPages);
	}

	public int NumPages()
	{
		return mNumPages;
	}

	public void AddPage()
	{
		int theStartY = (IMAGE_UI_LEADERBOARDS_SHADOW.GetHeight() - 40) * mNumPages;
		SetupAchievementsText(ref theStartY);
		mNumPages++;
	}

	public override void Draw(Graphics g)
	{
		if (GameApp.gApp.mBambooTransition != null && GameApp.gApp.mBambooTransition.IsInProgress())
		{
			return;
		}
		int num = 8;
		int num2 = 0;
		int num3 = 0;
		Graphics3D graphics3D = g.Get3D();
		graphics3D.SetMasking(Graphics3D.EMaskMode.MASKMODE_TEST_OUTSIDE);
		for (int i = 0; i < mText.Count; i++)
		{
			if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_RU || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_PL)
			{
				num = -2;
				num2 = -4;
			}
			if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_RU && num3++ != 10)
			{
				num = 4;
				num2 = 0;
			}
			AchievementText achievementText = mText[i];
			int num4 = 0;
			int num5 = 0;
			int num6 = 96;
			if (achievementText.mIcon != null)
			{
				num4 = num6 + 5;
				if (!achievementText.mUnlocked)
				{
					g.SetColor(120, 120, 120, 140);
					g.SetColorizeImages(colorizeImages: true);
				}
				g.DrawImage(achievementText.mIcon, achievementText.mX, achievementText.mY, num6, num6);
				g.SetColorizeImages(colorizeImages: false);
			}
			if (achievementText.mValueStr.Length == 0)
			{
				g.SetFont(mHeaderFont);
				if (!achievementText.mUnlocked)
				{
					g.SetColor(120, 120, 120, 140);
				}
				else
				{
					g.SetColor(0, 0, 0);
				}
				g.DrawString(achievementText.mHeaderStr, num4 + achievementText.mX + (int)mAchievements.mXOff, num5 + achievementText.mY + g.GetFont().GetAscent());
				g.SetFont(mDescFont);
				if (!achievementText.mUnlocked)
				{
					g.SetColor(120, 120, 120, 140);
				}
				else
				{
					g.SetColor(0, 0, 0);
				}
				Rect theRect = new Rect(num4 + achievementText.mX + (int)mAchievements.mXOff, num + achievementText.mY + g.GetFont().GetAscent(), 360, mDescFont.GetHeight() * 2);
				g.WriteWordWrapped(theRect, achievementText.mDescStr, 25 + num2, -1);
				g.SetFont(mPointFont);
				if (!achievementText.mUnlocked)
				{
					g.SetColor(120, 120, 120, 140);
				}
				else
				{
					g.SetColor(255, 249, 255, 255);
				}
				g.DrawString(achievementText.mPointStr, num4 + achievementText.mX + (int)mAchievements.mXOff, 15 + achievementText.mY + g.GetFont().GetAscent());
				g.SetColorizeImages(colorizeImages: false);
			}
			else
			{
				g.SetFont(mStatsFont);
				g.SetColor(0, 0, 0);
				g.WriteString(achievementText.mValueStr, num4 + achievementText.mX + Common._DS(Common._M(20)) + (int)mAchievements.mXOff, num5 + achievementText.mY + mStatsFont.GetAscent(), mWidth, -1);
			}
		}
		int num7 = NumPages() - 1;
		int num8 = IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetWidth() + 30;
		int theY = (IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetHeight() - IMAGE_UI_CHALLENGESCREEN_DIVIDER.GetHeight()) / 2;
		for (int j = 0; j < num7; j++)
		{
			g.DrawImage(IMAGE_UI_CHALLENGESCREEN_DIVIDER, num8 - IMAGE_UI_CHALLENGESCREEN_DIVIDER.GetWidth(), theY);
			num8 += IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetWidth() + 30;
		}
		graphics3D.SetMasking(Graphics3D.EMaskMode.MASKMODE_NONE);
	}

	public override void Update()
	{
		float num = Common._M(10f);
		for (int i = 0; i < mText.size(); i++)
		{
			AchievementText achievementText = mText[i];
			if (achievementText.mFadeIn && achievementText.mAlpha < 255f)
			{
				MarkDirty();
				achievementText.mAlpha += num;
				if (achievementText.mAlpha > 255f)
				{
					achievementText.mAlpha = 255f;
				}
			}
			else if (!achievementText.mFadeIn && achievementText.mAlpha > 0f)
			{
				MarkDirty();
				achievementText.mAlpha -= num;
				if (achievementText.mAlpha <= 0f)
				{
					mText.RemoveAt(i);
					i--;
				}
			}
		}
	}

	private void SetupAchievementsText(ref int theStartY)
	{
		int totalNum = 0;
		int unlockedNum = 0;
		int totalGPoint = 0;
		int unlockedGPoint = 0;
		GameApp.gApp.mUserProfile.m_AchievementMgr.getAchievementsInfo(ref unlockedNum, ref totalNum, ref unlockedGPoint, ref totalGPoint);
		int num = Common._DS(Common._M(80));
		int num2 = Common._DS(Common._M(200));
		AchievementText achievementText = null;
		int num3 = Common._DS(Common._M(0));
		num3 += 32;
		for (int i = 0; i < 3; i++)
		{
			int num4 = i + mNumPages * 3;
			if (num4 >= totalNum)
			{
				break;
			}
			mText.Add(new AchievementText());
			achievementText = mText.back();
			AchievementEntry achievementEntry = GameApp.gApp.mUserProfile.m_AchievementMgr.GetAchievementEntry((EAchievementType)num4);
			achievementText.mIcon = Res.GetImageByID((ResID)achievementEntry.m_IconResID);
			achievementText.mUnlocked = achievementEntry.m_Unlocked;
			achievementText.mHeaderStr = TextManager.getInstance().getString(achievementEntry.m_NameResID).ToUpper();
			achievementText.mValueStr = "";
			achievementText.mDescStr = TextManager.getInstance().getString(achievementEntry.m_DescriptionResID);
			achievementText.mX = num + 10;
			achievementText.mY = num3 + theStartY;
			mText.Add(new AchievementText());
			achievementText = mText.back();
			achievementText.mUnlocked = achievementEntry.m_Unlocked;
			achievementText.mHeaderStr = "";
			achievementText.mValueStr = "";
			achievementText.mPointStr = $"{achievementEntry.m_GPoints}G";
			achievementText.mX = num + 495;
			achievementText.mY = num3 + theStartY + 10;
			num3 += num2;
		}
	}
}
