using System.Collections.Generic;
using System.Text;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class ZoneFrame : Widget, ButtonListener
{
	private CSButton[] mChallengeLevelBtns = new CSButton[10];

	private ChallengeMenu mChallengeMenu;

	private int mDebugBGColor;

	private int mZoneNum;

	private string mZoneName;

	private string mZoneDifficulty;

	private int mZoneNameStrWidth;

	private int mZoneDifficultyWidth;

	private Image IMAGE_UI_MAIN_MENU_CS_LOCK_ANIMATION;

	private Image IMAGE_UI_CHALLENGESCREEN_LARGE_CROWN;

	private Image IMAGE_UI_CHALLENGESCREEN_LARGE_ACECROWN;

	public ZoneFrame(ChallengeMenu aChallengeMenu, int aZone, int aDebugBGColor)
	{
		mChallengeMenu = aChallengeMenu;
		mZoneNum = aZone;
		mDebugBGColor = aDebugBGColor;
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES);
		Resize(0, 0, imageByID.GetWidth(), imageByID.GetHeight());
		IMAGE_UI_CHALLENGESCREEN_LARGE_CROWN = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_LARGE_CROWN);
		IMAGE_UI_CHALLENGESCREEN_LARGE_ACECROWN = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_LARGE_ACECROWN);
		IMAGE_UI_MAIN_MENU_CS_LOCK_ANIMATION = Res.GetImageByID(ResID.IMAGE_UI_MAIN_MENU_CS_LOCK_ANIMATION);
		for (int i = 0; i < 10; i++)
		{
			if (mChallengeLevelBtns[i] != null)
			{
				mChallengeLevelBtns[i].mLevel = -1;
			}
		}
		for (int j = 0; j < 10; j++)
		{
			if (mChallengeLevelBtns[j] == null)
			{
				mChallengeLevelBtns[j] = new CSButton(3 + j + mZoneNum * 10, mChallengeMenu, this);
				mChallengeLevelBtns[j].mDoFinger = true;
				AddWidget(mChallengeLevelBtns[j]);
			}
		}
		int theWidth = Common._DS(GlobalChallenge.CS_BTN_WIDTH);
		int theHeight = Common._DS(GlobalChallenge.CS_BTN_HEIGHT);
		for (int k = 0; k < GlobalChallenge.NUM_CHALLENGE_BUTTON_ROWS; k++)
		{
			for (int l = 0; l < GlobalChallenge.NUM_CHALLENGE_BUTTON_COLS; l++)
			{
				int num = k * GlobalChallenge.NUM_CHALLENGE_BUTTON_COLS + l;
				if (num != 8 && num != 11)
				{
					int num2 = ((num > 8) ? (num - 1) : num);
					CSButton cSButton = mChallengeLevelBtns[num2];
					if (cSButton != null)
					{
						int theX = Common._DS(GlobalChallenge.FIRST_X + l * GlobalChallenge.HORIZ_SPACE) - Common._DS(160) + GameApp.gApp.GetScreenRect().mX / 2;
						int theY = Common._DS(GlobalChallenge.FIRST_Y + k * GlobalChallenge.VERT_SPACE);
						cSButton.Resize(theX, theY, theWidth, theHeight);
					}
				}
			}
		}
		SetupChallengeZone(mZoneNum);
		mZoneName = GameApp.gApp.GetLevelMgr().mZones[mZoneNum].mCupName;
		mZoneDifficulty = GameApp.gApp.GetLevelMgr().mZones[mZoneNum].mDifficulty;
		mZoneNameStrWidth = -1;
		mZoneDifficultyWidth = -1;
	}

	public override void Dispose()
	{
		for (uint num = 0u; num < 10; num++)
		{
			RemoveWidget(mChallengeLevelBtns[num]);
		}
	}

	public override void Draw(Graphics g)
	{
		if (g.mClipRect.mWidth > 0 && g.mClipRect.mHeight > 0)
		{
			Common._S(0);
			_ = GlobalChallenge.gScreenShake;
			g.SetFont(Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_GAUNTLET));
			g.SetColor(Color.White);
			if (mZoneNameStrWidth == -1)
			{
				mZoneNameStrWidth = g.GetFont().StringWidth(mZoneName);
			}
			g.WriteString(mZoneName, Common._DS(100), Common._DS(Common._M(120)), mZoneNameStrWidth, 0);
			g.SetFont(Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_GAUNTLET));
			g.SetColor(Color.White);
			string text = TextManager.getInstance().getString(423) + " " + mZoneDifficulty;
			float num = Common._DS(1280) + GameApp.gApp.GetScreenRect().mX / 2;
			float num2 = g.GetFont().StringWidth(text);
			float num3 = num - num2;
			if (num3 <= 450f)
			{
				Rect theRect = new Rect(450, 38, 250, 300);
				g.WriteWordWrapped(theRect, text, 20);
			}
			else
			{
				g.DrawString(text, (int)num3, Common._DS(120));
			}
		}
	}

	public ButtonWidget GetButton(int id)
	{
		for (int i = 0; i < 10; i++)
		{
			if (mChallengeLevelBtns[i] != null && mChallengeLevelBtns[i].mId == id)
			{
				return mChallengeLevelBtns[i];
			}
		}
		return null;
	}

	public virtual void ButtonDepress(int id)
	{
		if (GameApp.gApp.mBambooTransition != null && GameApp.gApp.mBambooTransition.IsInProgress())
		{
			return;
		}
		GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BUTTON1));
		int num = mZoneNum * 10 + 3;
		int num2 = mZoneNum * 10 + 13;
		if (id >= num && id < num2)
		{
			CSButton cSButton = (CSButton)GetButton(id);
			if (cSButton.mMedal == IMAGE_UI_MAIN_MENU_CS_LOCK_ANIMATION)
			{
				GameApp.gApp.DoGenericDialog("", cSButton.mLevelStr, block: true, null, Common._DS(100));
				GameApp.gApp.mWidgetManager.SetFocus(mChallengeMenu);
				return;
			}
			int num3 = id - 3;
			int num4 = num3 - mZoneNum * 10;
			mChallengeMenu.mSelectedLevel = num3;
			mChallengeMenu.ShowChallengeLevelInfo(mZoneNum, num3, mChallengeLevelBtns[num4].mLevelId);
			mChallengeMenu.mChallengeLevelInfoWidget.SetLevelInfo(mChallengeLevelBtns[num4].mLevelStr, mChallengeLevelBtns[num4].mScoreStr, mChallengeLevelBtns[num4].mAceStr, mChallengeLevelBtns[num4].mId);
			GameApp.gLastZone = mZoneNum;
			mChallengeMenu.mChallengeScrollWidget.SetPageHorizontal(mZoneNum + 1, animated: true);
		}
	}

	public void InitCS()
	{
	}

	public void RehupChallengeButtons()
	{
		int num = Common._S(0);
		if (GameApp.gApp.mUserProfile.mUnlockSparklesIdx1 != -1)
		{
			int num2 = mZoneNum * 10;
			int num3 = num2 + 9;
			if (GameApp.gApp.mUserProfile.mUnlockSparklesIdx1 >= num2 && GameApp.gApp.mUserProfile.mUnlockSparklesIdx1 <= num3)
			{
				int num4 = -1;
				num4 = ((mZoneNum <= 0) ? GameApp.gApp.mUserProfile.mUnlockSparklesIdx1 : (GameApp.gApp.mUserProfile.mUnlockSparklesIdx1 % (mZoneNum * 10)));
				PIEffect pIEffect = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_GOLDSPARKLE_CHALLENGE).Duplicate();
				CSButton cSButton = mChallengeLevelBtns[num4];
				cSButton.mUnlockSparkles = pIEffect;
				cSButton.mUnlockAlpha = 255;
				float num5 = GameApp.DownScaleNum(1f);
				pIEffect.mDrawTransform.Scale(num5, num5);
				pIEffect.mDrawTransform.Translate(cSButton.mX - num + Common._DS(GlobalChallenge.CS_BTN_WIDTH) / 2, cSButton.mY + Common._DS(GlobalChallenge.CS_BTN_HEIGHT) / 2);
				GameApp.gApp.mUserProfile.mUnlockSparklesIdx1 = -1;
			}
		}
		if (GameApp.gApp.mUserProfile.mUnlockSparklesIdx2 != -1)
		{
			int num6 = mZoneNum * 10;
			int num7 = num6 + 10;
			if (GameApp.gApp.mUserProfile.mUnlockSparklesIdx2 >= num6 && GameApp.gApp.mUserProfile.mUnlockSparklesIdx2 <= num7)
			{
				int num8 = -1;
				num8 = ((mZoneNum <= 0) ? GameApp.gApp.mUserProfile.mUnlockSparklesIdx2 : (GameApp.gApp.mUserProfile.mUnlockSparklesIdx2 % (mZoneNum * 10)));
				PIEffect pIEffect2 = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_GOLDSPARKLE_CHALLENGE).Duplicate();
				CSButton cSButton2 = mChallengeLevelBtns[num8];
				cSButton2.mUnlockSparkles = pIEffect2;
				cSButton2.mUnlockAlpha = 255;
				float num9 = GameApp.DownScaleNum(1f);
				pIEffect2.mDrawTransform.Scale(num9, num9);
				pIEffect2.mDrawTransform.Translate(cSButton2.mX - num + Common._DS(GlobalChallenge.CS_BTN_WIDTH) / 2, cSButton2.mY + Common._DS(GlobalChallenge.CS_BTN_HEIGHT) / 2);
				GameApp.gApp.mUserProfile.mUnlockSparklesIdx2 = -1;
			}
		}
	}

	public void PreLoadButtonsImage()
	{
		for (int i = 0; i < mChallengeLevelBtns.Length; i++)
		{
			if (mChallengeLevelBtns[i] != null)
			{
				mChallengeLevelBtns[i].PreLoadImage();
			}
		}
	}

	private void SetupChallengeZone(int zone)
	{
		mChallengeMenu.mLoopTrophyFlare = false;
		mChallengeMenu.mTrophyFlare = null;
		bool flag = GameApp.gApp.mUserProfile.mChallengeUnlockState[zone, 0] == 0;
		mChallengeMenu.mShowFullAceFX = false;
		if (flag)
		{
			mChallengeMenu.mDefaultStringContainer.mDefaultStr = ((zone == 7) ? mChallengeMenu.mDefaultStringContainer.IfLocked() : mChallengeMenu.mDefaultStringContainer.NonIfLocked());
		}
		else if (GameApp.gApp.mUserProfile.mChallengeUnlockState[zone, 0] == 1)
		{
			mChallengeMenu.mDefaultStringContainer.mDefaultStr = ((zone == 7) ? mChallengeMenu.mDefaultStringContainer.IfLocked() : mChallengeMenu.mDefaultStringContainer.ZoneUnlocked());
		}
		else
		{
			mChallengeMenu.mDefaultStringContainer.mDefaultStr = ((zone == 7) ? mChallengeMenu.mDefaultStringContainer.IfLocked() : mChallengeMenu.mDefaultStringContainer.CanPlayZone());
		}
		Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_TROPHYFLARE_Z1);
		Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_TROPHYFLARE_Z2);
		Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_TROPHYFLARE_Z3);
		Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_TROPHYFLARE_Z4);
		Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_TROPHYFLARE_Z5);
		Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_TROPHYFLARE_Z6);
		Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_TROPHYFLARE_Z7);
		for (int i = 0; i < 10; i++)
		{
			int num = zone * 10 + i;
			mChallengeLevelBtns[i].mLevel = num;
			string level_disp_name = "";
			int first = GameApp.gApp.mLevelThumbnails[num].first;
			GameApp.gApp.GetLevelMgr().GetLevelStrData(first, ref mChallengeLevelBtns[i].mLevelId, ref level_disp_name);
			int num2 = 0;
			mChallengeLevelBtns[i].mUnlockSparkles = null;
			if (GameApp.gApp.mUserProfile == null)
			{
				continue;
			}
			List<GauntletHSInfo> scores = new List<GauntletHSInfo>();
			int num3 = 0;
			GameApp.gApp.mUserProfile.GetGauntletHighScores(num + 1, ref scores);
			if (scores.Count > 0)
			{
				for (int j = 0; j < scores.Count; j++)
				{
					if (scores[j].mProfileName == GameApp.gApp.mUserProfile.GetName() && scores[j].mScore > num2)
					{
						num2 = scores[j].mScore;
					}
					if (scores[j].mScore > num3)
					{
						num3 = scores[j].mScore;
					}
				}
			}
			int num4 = GameApp.gApp.mUserProfile.mChallengeUnlockState[zone, i];
			if (num4 < 2)
			{
				mChallengeLevelBtns[i].mMedal = IMAGE_UI_MAIN_MENU_CS_LOCK_ANIMATION;
			}
			else
			{
				switch (num4)
				{
				case 4:
					mChallengeLevelBtns[i].mMedal = IMAGE_UI_CHALLENGESCREEN_LARGE_CROWN;
					break;
				case 5:
					mChallengeLevelBtns[i].mMedal = IMAGE_UI_CHALLENGESCREEN_LARGE_ACECROWN;
					break;
				default:
					mChallengeLevelBtns[i].mMedal = null;
					break;
				}
			}
			mChallengeLevelBtns[i].mOpaque = flag;
			mChallengeLevelBtns[i].mUnlockAlpha = 0;
			if (mChallengeLevelBtns[i].mMedal != IMAGE_UI_MAIN_MENU_CS_LOCK_ANIMATION)
			{
				Level levelById = GameApp.gApp.GetLevelMgr().GetLevelById(mChallengeLevelBtns[i].mLevelId);
				if (num2 > 9999999)
				{
					num2 = 9999999;
				}
				mChallengeLevelBtns[i].mScoreStr = SexyFramework.Common.CommaSeperate(num2);
				mChallengeLevelBtns[i].mLevelStr = SexyFramework.Common.CommaSeperate(levelById.mChallengePoints);
				mChallengeLevelBtns[i].mAceStr = SexyFramework.Common.CommaSeperate(levelById.mChallengeAcePoints);
			}
			else if (flag)
			{
				StringBuilder stringBuilder = new StringBuilder(TextManager.getInstance().getString(424));
				stringBuilder.Replace("$1", ((mZoneNum + 1) * 10).ToString());
				mChallengeLevelBtns[i].mLevelStr = stringBuilder.ToString();
			}
			else
			{
				mChallengeLevelBtns[i].mLevelStr = TextManager.getInstance().getString(425);
			}
		}
		MarkDirty();
	}

	public virtual void ButtonDownTick(int x)
	{
	}

	public virtual void ButtonMouseEnter(int x)
	{
	}

	public virtual void ButtonMouseLeave(int x)
	{
	}

	public virtual void ButtonMouseMove(int x, int y, int z)
	{
	}

	public virtual void ButtonPress(int id)
	{
	}

	public virtual void ButtonPress(int id, int count)
	{
	}
}
