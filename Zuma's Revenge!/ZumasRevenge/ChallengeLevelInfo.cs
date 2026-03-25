using System;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class ChallengeLevelInfo : ZumaDialog, ButtonListener
{
	private ChallengeMenu mChallengeMenu;

	private int mChallengeLevelNum;

	private string mChallengeLevelName;

	private int mChallengeZone;

	private CSDisplayItem mLevelInfo;

	private static float thumbScale = 2.3f;

	private static int borderXOff = 12;

	private static int borderYOff = 12;

	private int lang_offset;

	public ChallengeLevelInfo(ChallengeMenu aMenu)
		: base(10, isModal: false, "", "", "", 2)
	{
		mChallengeMenu = aMenu;
		mChallengeLevelNum = -1;
		mChallengeZone = -1;
		mLevelInfo = new CSDisplayItem();
		mLevelInfo.mLevelStr = mChallengeMenu.mDefaultStringContainer.NothingSelected();
		mChallengeLevelName = "";
		if (mYesButton != null)
		{
			mYesButton.mLabel = TextManager.getInstance().getString(455);
		}
		if (mNoButton != null)
		{
			mNoButton.mLabel = TextManager.getInstance().getString(458);
		}
		if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_RU || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_PL || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_PGB || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_PG || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_SP || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_SPC || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_GR)
		{
			lang_offset = -65;
		}
	}

	public void SetLevelInfo(string aLevelStr, string aScoreStr, string aAceStr, int aNum)
	{
		mLevelInfo.mLevelStr = aLevelStr;
		mLevelInfo.mScoreStr = aScoreStr;
		mLevelInfo.mAceStr = aAceStr;
		mLevelInfo.mNum = aNum;
	}

	public override void Update()
	{
		if (mLevelInfo.mFadeIn && mLevelInfo.mAlpha < 255f)
		{
			mLevelInfo.mAlpha += Common._M(10);
			MarkDirty();
			if (mLevelInfo.mAlpha > 255f)
			{
				mLevelInfo.mAlpha = 255f;
			}
		}
		else if (!mLevelInfo.mFadeIn)
		{
			MarkDirty();
			mLevelInfo.mAlpha -= Common._M(10);
			if (mLevelInfo.mAlpha <= 0f)
			{
				mLevelInfo.mAlpha = 0f;
			}
		}
		base.Update();
	}

	public override void Draw(Graphics g)
	{
		g.PushState();
		base.Draw(g);
		if (mChallengeLevelNum != -1)
		{
			Image levelThumbnail = GameApp.gApp.GetLevelThumbnail(mChallengeLevelNum);
			int num = (int)(Common._DS(thumbScale) * (float)levelThumbnail.mWidth * 1.8f);
			int num2 = (int)(Common._DS(thumbScale) * (float)levelThumbnail.mHeight * 1.8f);
			int num3 = mWidth - num - Common._DS(100);
			int num4 = (mHeight - num2) / 2 + Common._DS(30);
			g.DrawImage(levelThumbnail, num3, num4, num, num2);
			Common.DrawCommonDialogBorder(g, num3 - Common._DS(16), num4 - Common._DS(16), num + Common._DS(32), num2 + Common._DS(32));
			int num5 = 0;
			int num6 = Common._DS(Common._M(435)) + num5 + lang_offset;
			int num7 = num4 + g.GetFont().GetHeight();
			_ = mLevelInfo.mAlpha;
			_ = 0f;
			g.SetColor(new Color(Common._M(214), Common._M1(143), Common._M2(7), (int)mLevelInfo.mAlpha));
			int num8 = Common._DS(Common._M(10)) - mX / 2 + (GameApp.gApp.GetScreenWidth() - GameApp.gApp.mScreenBounds.mWidth);
			if (SexyFramework.Common.StrEquals(mLevelInfo.mLevelStr, mChallengeMenu.mDefaultStringContainer.mDefaultStr))
			{
				if (GameApp.gLastZone != -1)
				{
					if (GameApp.gLastZone == 7 && GameApp.gApp.mUserProfile.mChallengeUnlockState[GameApp.gLastZone - 1, 0] == 0)
					{
						g.WriteWordWrapped(new Rect(num6 + num8, num7 + ((GameApp.gLastZone == 7) ? Common._DS(Common._M(-40)) : Common._DS(Common._M1(-40))), Common._DS(Common._M2(320)), 10000), mLevelInfo.mLevelStr, -1, 0);
					}
				}
				else
				{
					g.WriteWordWrapped(new Rect(num6 + num8, num7 + ((GameApp.gLastZone == 7) ? Common._DS(Common._M(-22)) : Common._DS(Common._M1(-40))), Common._DS(Common._M2(362)), 10000), mLevelInfo.mLevelStr, -1, 0);
				}
			}
			else if (SexyFramework.Common.StrEquals(mLevelInfo.mLevelStr, mChallengeMenu.mDefaultStringContainer.NothingSelected()))
			{
				g.WriteWordWrapped(new Rect(Common._DS(Common._M(608)) + num5 + num8, num7 + Common._DS(Common._M1(0)), Common._DS(Common._M2(362)), 10000), mLevelInfo.mLevelStr, -1, 0);
			}
			else
			{
				int value = -210;
				float value2 = 75f;
				int value3 = 50;
				string theString = TextManager.getInstance().getString(420);
				string theString2 = TextManager.getInstance().getString(421);
				string theString3 = TextManager.getInstance().getString(422);
				float num9 = g.GetFont().StringWidth(theString);
				float num10 = g.GetFont().StringWidth(theString2);
				float num11 = g.GetFont().StringWidth(theString3);
				float num12 = Math.Max(num11, Math.Max(num9, num10));
				float num13 = num6 + Common._DS(value);
				float num14 = g.GetFont().StringWidth(mLevelInfo.mLevelStr);
				float num15 = num13 + (num12 - num9) / 2f;
				float num16 = num15 + (num9 - num14) / 2f;
				g.DrawString(theString, (int)num15, num7);
				g.DrawString(mLevelInfo.mLevelStr, (int)num16, num7 + Common._DS(value3));
				float num17 = g.GetFont().StringWidth(mLevelInfo.mAceStr);
				float num18 = num13 + (num12 - num10) / 2f;
				float num19 = num18 + (num10 - num17) / 2f;
				g.DrawString(theString2, (int)num18, num7 + g.GetFont().GetHeight() + (int)Common._DS(value2));
				g.DrawString(mLevelInfo.mAceStr, (int)num19, num7 + g.GetFont().GetHeight() + (int)Common._DS(value2) + Common._DS(value3));
				g.PushState();
				g.SetColor(new Color(Common._M(220), Common._M1(220), 0, (int)mLevelInfo.mAlpha));
				float num20 = g.GetFont().StringWidth(mLevelInfo.mScoreStr);
				float num21 = num13 + (num12 - num11) / 2f;
				float num22 = num21 + (num11 - num20) / 2f;
				g.DrawString(theString3, (int)num21, num7 + g.GetFont().GetHeight() * 2 + (int)Common._DS(value2) * 2);
				g.DrawString(mLevelInfo.mScoreStr, (int)num22, num7 + g.GetFont().GetHeight() * 2 + (int)(Common._DS(value2) * 2f + (float)Common._DS(value3)));
				g.PopState();
			}
			DrawLevelName(g);
		}
		g.PopState();
	}

	public void DrawLevelName(Graphics g)
	{
		int index = mChallengeLevelNum % 10 + mChallengeZone * 10 + mChallengeZone;
		g.SetFont(Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_STROKE));
		g.SetColor(Color.White);
		string theString = mChallengeLevelNum + 1 + " - " + GameApp.gApp.GetLevelMgr().mLevels[index].mDisplayName;
		int num = g.GetFont().StringWidth(theString);
		g.DrawString(theString, (mWidth - num) / 2, Common._DS(320));
	}

	public new virtual void ButtonDepress(int theId)
	{
		if (GameApp.gApp.mBambooTransition != null && GameApp.gApp.mBambooTransition.IsInProgress())
		{
			return;
		}
		if (theId == 2000 + mId || theId == 1000)
		{
			if (mChallengeLevelNum != -1)
			{
				GameApp.gLastLevel = mChallengeLevelNum + 1;
				GameApp.gLastZone = mChallengeMenu.mChallengeScrollWidget.GetPageHorizontal() - 1;
				GameApp.gApp.mMainMenu.mGauntletModLevel_id = GetChallengeLevelName();
				GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.mMainMenu.StartChallengeGame;
				GameApp.gApp.ToggleBambooTransition();
			}
		}
		else if (theId == 3000 + mId || theId == 1001)
		{
			mChallengeMenu.HideChallengeLevelInfo();
		}
	}

	public void SetLevel(int theZoneNum, int theLevelNum, string theLevelName)
	{
		mChallengeLevelNum = theLevelNum;
		mChallengeLevelName = theLevelName;
		mChallengeZone = theZoneNum;
	}

	public string GetChallengeLevelName()
	{
		return mChallengeLevelName;
	}
}
