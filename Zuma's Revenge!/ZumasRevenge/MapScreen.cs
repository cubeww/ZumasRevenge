using System;
using System.Linq;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class MapScreen : ButtonListener, IDisposable
{
	internal const float POSTCARD_PCT = 0.55f;

	internal static readonly string[] gZoneNames = new string[6] { "Jungle of Mystery", "Quiet Village", "Lost City", "Mosquito Coast", "Underwater Grotto", "Volcano Temple" };

	protected Transform mGlobalTranform = new Transform();

	private Point[] mAreaCoords = new Point[6]
	{
		new Point(0, 0),
		new Point(Common._DS(Common._M(1150)), Common._DS(Common._M1(120))),
		new Point(Common._DS(Common._M2(1080)), Common._DS(Common._M3(396))),
		new Point(Common._DS(Common._M4(660)), Common._DS(Common._M5(341))),
		new Point(Common._DS(Common._M(600)), Common._DS(Common._M1(305))),
		new Point(Common._DS(Common._M2(720)), Common._DS(Common._M3(40)))
	};

	private Point[] mZoneCenters = new Point[6]
	{
		new Point(Common._M(1458), Common._M1(653)),
		new Point(Common._M2(1120), Common._M3(305)),
		new Point(Common._M4(1077), Common._M5(585)),
		new Point(Common._M6(692), Common._M7(532)),
		new Point(Common._M(600), Common._M1(830)),
		new Point(Common._M2(728), Common._M3(290))
	};

	public PIEffect mZoneEffect;

	public MemoryImage mNewZoneTextImg;

	public float mNewZoneTextSize;

	public int mNewZoneTextBounceCount;

	public MapButton mContinueBtn;

	public MapGenericButton mZoneBtn;

	public ButtonWidget mBackBtn;

	public MapGenericButton mSelectZoneBackBtn;

	public MapOverlay[] mOverlays = SexyFramework.Common.CreateObjectArray<MapOverlay>(5);

	public bool mDisplayingZones;

	public bool mFromCheckpoint;

	public bool mFromIntro;

	public bool mContinueGoesToCheckpoint;

	public bool mBeatGame;

	public int mHighestDot;

	public int mUpdateCount;

	public int mSlideDir;

	public float mXOff;

	public float mUnlockScrollAmt;

	public int mLastMouseX;

	public int mLastMouseY;

	public bool mHasPlayedZoneUnlockedSound;

	public CurvedVal mUnlockNameAlpha = new CurvedVal();

	public CurvedVal mUnlockNameHilite = new CurvedVal();

	public CurvedVal mUnlockOutlineAlpha = new CurvedVal();

	public CurvedVal mUnlockIconAlpha = new CurvedVal();

	public CurvedVal mClickToEnterAlpha = new CurvedVal();

	public CurvedVal mExtrasAlpha = new CurvedVal();

	public CurvedVal mDotSubtract = new CurvedVal();

	public bool mDisableInput;

	public bool mFadingOut;

	public float mDisplayZoneAlpha;

	public bool mIncDisplayZoneAlpha;

	public WidgetContainer mParent;

	public CurvedVal mAlpha = new CurvedVal();

	public Rect[] mCards = SexyFramework.Common.CreateObjectArray<Rect>(6);

	public bool mCompletedZone;

	public bool mDirty;

	public bool mRemove;

	public bool mContinueFromCheckpoint;

	public int mSelectedZone;

	public int mDisplayedZone;

	public int mMapOffsetX = 53;

	public bool mIntroClosing;

	public bool mClosing;

	public bool mZoneOver;

	public float mZoneOverPct;

	public PIEffect[] zone_effects = new PIEffect[6];

	public bool mIsTryAndBuyDialogShowing;

	public bool mIsTrialEnd;

	protected bool MouseOverCard(int idx)
	{
		if (mCards[idx].Contains(mLastMouseX, mLastMouseY) && (idx == 0 || mOverlays[idx - 1].mUnlocked))
		{
			return GameApp.gApp.mDialogMap.Count() == 0;
		}
		return false;
	}

	protected void DrawDesaturatedZone(Graphics g, int theZoneId, float theDesaturationPct)
	{
		if (!(theDesaturationPct <= 0f))
		{
			g.Get3D();
			ResID id = (ResID)(1294 + theZoneId - 1);
			Image imageByID = Res.GetImageByID(id);
			g.SetColor(255, 255, 255, (int)((double)(255f * theDesaturationPct) * (double)mAlpha));
			g.DrawImage(imageByID, Common._DS(Res.GetOffsetXByID(id)) + Common._S(0) + (int)mUnlockScrollAmt, Common._DS(Res.GetOffsetYByID(id)));
		}
	}

	public MapScreen()
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_MAP_OPENBOOK_PAGES);
		if (!GameApp.gApp.mResourceManager.IsGroupLoaded("CommonGame") && !GameApp.gApp.mResourceManager.LoadResources("CommonGame"))
		{
			GameApp.gApp.ShowResourceError(doExit: true);
			GameApp.gApp.Shutdown();
		}
		if (!GameApp.gApp.mResourceManager.IsGroupLoaded("Map") && !GameApp.gApp.mResourceManager.LoadResources("Map"))
		{
			GameApp.gApp.ShowResourceError(doExit: true);
			GameApp.gApp.Shutdown();
		}
		mParent = GlobalMembers.gSexyAppBase.mWidgetManager;
		mAlpha.SetConstant(1.0);
		mDirty = true;
		mUnlockScrollAmt = 0f;
		mHighestDot = 1;
		mSelectedZone = -1;
		mNewZoneTextSize = 1f;
		mNewZoneTextImg = null;
		mZoneEffect = null;
		mSlideDir = 0;
		mXOff = 0f;
		SetupClouds();
		mContinueBtn = null;
		mBackBtn = null;
		mSelectZoneBackBtn = null;
		mZoneBtn = null;
		mZoneOverPct = 0f;
		mExtrasAlpha.SetConstant(1.0);
		mIsTrialEnd = false;
		int num = (GameApp.gApp.mWidth - imageByID.mWidth) / 2 + Common._DS(Common._M(0)) - GameApp.gApp.mWideScreenXOffset / 2;
		int num2 = (GameApp.gApp.mHeight - imageByID.mHeight) / 2;
		int num3 = Common._DS(Common._M(200));
		for (int i = 0; i < 6; i++)
		{
			int theX = -46 + num + ((i % 2 == 0) ? Common._DS(Common._M(250)) : Common._DS(Common._M1(845)));
			int num4 = Common._DS(Common._M(290)) + 10;
			int theY = num2 + num3 + num4 * (i / 2) - 8;
			ref Rect reference = ref mCards[i];
			reference = new Rect(theX, theY, (int)(266f * Common._S(0.55f)), (int)(200f * Common._S(0.55f)));
		}
		mDisplayZoneAlpha = 0f;
		mIncDisplayZoneAlpha = true;
	}

	public virtual void Dispose()
	{
		if (mContinueBtn != null)
		{
			mParent.RemoveWidget(mContinueBtn);
		}
		if (mZoneBtn != null)
		{
			mParent.RemoveWidget(mZoneBtn);
		}
		if (mBackBtn != null)
		{
			mParent.RemoveWidget(mBackBtn);
		}
		if (mSelectZoneBackBtn != null)
		{
			mParent.RemoveWidget(mSelectZoneBackBtn);
		}
	}

	public void SetupClouds()
	{
		FPoint[] array = new FPoint[15]
		{
			new FPoint(Common._M(900), Common._M1(125)),
			new FPoint(Common._M2(870), Common._M3(183)),
			new FPoint(Common._M4(850), Common._M5(285)),
			new FPoint(Common._M(815), Common._M1(380)),
			new FPoint(Common._M2(828), Common._M3(477)),
			new FPoint(Common._M4(850), Common._M5(570)),
			new FPoint(Common._M(430), Common._M1(330)),
			new FPoint(Common._M2(470), Common._M3(459)),
			new FPoint(Common._M4(544), Common._M5(569)),
			new FPoint(Common._M(340), Common._M1(530)),
			new FPoint(Common._M2(390), Common._M3(596)),
			new FPoint(Common._M4(390), Common._M5(704)),
			new FPoint(Common._M(540), Common._M1(91)),
			new FPoint(Common._M2(463), Common._M3(165)),
			new FPoint(Common._M4(460), Common._M5(269))
		};
		FPoint[] array2 = new FPoint[15]
		{
			new FPoint(Common._M(1.2f), Common._M1(1f)),
			new FPoint(Common._M2(1.1f), Common._M3(1.1f)),
			new FPoint(Common._M4(1.4f), Common._M5(1.3f)),
			new FPoint(Common._M(1.3f), Common._M1(1.35f)),
			new FPoint(Common._M2(1.1f), Common._M3(1f)),
			new FPoint(Common._M4(1.2f), Common._M5(1.25f)),
			new FPoint(Common._M(1.17f), Common._M1(1.4f)),
			new FPoint(Common._M2(0.9f), Common._M3(1f)),
			new FPoint(Common._M4(0.9f), Common._M5(1.2f)),
			new FPoint(Common._M(0.5f), Common._M1(1f)),
			new FPoint(Common._M2(0.5f), Common._M3(1.2f)),
			new FPoint(Common._M4(1f), Common._M5(1.5f)),
			new FPoint(Common._M(1.05f), Common._M1(1.1f)),
			new FPoint(Common._M2(1.05f), Common._M3(1.1f)),
			new FPoint(Common._M4(1.25f), Common._M5(1.15f))
		};
		for (int i = 0; i < 5; i++)
		{
			string theStringId = $"IMAGE_UI_MAP_{LevelMgr.GetTerseZoneName(i + 2).ToUpper()}";
			Common.GetIdByStringId(theStringId);
			mOverlays[i].mAlpha = Common._M(255);
			mOverlays[i].mUnlocked = false;
			for (int j = 0; j < 3; j++)
			{
				mOverlays[i].mCloudSizes[j] = array2[i * 3 + j];
				mOverlays[i].mCloudPoints[j] = array[i * 3 + j];
			}
		}
	}

	public void CloseDone()
	{
		mClosing = false;
		CleanButtons();
		mRemove = true;
	}

	public void CleanButtons()
	{
		mParent.RemoveWidget(mContinueBtn);
		GameApp.gApp.SafeDeleteWidget(mContinueBtn);
		mContinueBtn = null;
		mParent.RemoveWidget(mZoneBtn);
		GameApp.gApp.SafeDeleteWidget(mZoneBtn);
		mZoneBtn = null;
		mParent.RemoveWidget(mBackBtn);
		GameApp.gApp.SafeDeleteWidget(mBackBtn);
		mBackBtn = null;
		mParent.RemoveWidget(mSelectZoneBackBtn);
		GameApp.gApp.SafeDeleteWidget(mSelectZoneBackBtn);
		mSelectZoneBackBtn = null;
	}

	public void Hide(bool h)
	{
		if (mContinueBtn != null)
		{
			mContinueBtn.SetVisible(!h);
			mContinueBtn.SetDisabled(h);
		}
		if (mZoneBtn != null)
		{
			mZoneBtn.SetVisible(!h);
			mZoneBtn.SetDisabled(h);
		}
		_ = mBackBtn;
		_ = mSelectZoneBackBtn;
	}

	public void Init(bool zone_completed, int disp_zone, int disp_level, bool from_checkpoint, bool from_load)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_HOME);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_HOME_SELECT);
		Image imageByID3 = Res.GetImageByID(ResID.IMAGE_UI_MAP_OPENBOOK_BACK);
		Image imageByID4 = Res.GetImageByID(ResID.IMAGE_UI_MAP_OPENBOOK_BACK_DWN);
		Image imageByID5 = Res.GetImageByID(ResID.IMAGE_UI_MAP_OPENBOOK_PAGES);
		Image imageByID6 = Res.GetImageByID(ResID.IMAGE_UI_MAP_CONTINUE_BUTTON);
		Image imageByID7 = Res.GetImageByID(ResID.IMAGE_UI_MAP_BOOK_ANIM);
		Font fontByID = Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_STROKE);
		mClosing = false;
		mIntroClosing = false;
		mExtrasAlpha.SetConstant(1.0);
		mUpdateCount = 0;
		mFromIntro = !zone_completed && disp_zone == 1 && disp_level == 1 && !from_load;
		mHasPlayedZoneUnlockedSound = false;
		int num = (GameApp.gApp.mClickedHardMode ? GameApp.gApp.mUserProfile.mHeroicModeVars.mHighestZoneBeat : GameApp.gApp.mUserProfile.mAdvModeVars.mHighestZoneBeat);
		mBeatGame = num >= 6;
		mUnlockScrollAmt = 0f;
		mDisplayedZone = disp_zone;
		mCompletedZone = zone_completed;
		mContinueFromCheckpoint = false;
		mFromCheckpoint = from_checkpoint && !from_load;
		mContinueGoesToCheckpoint = from_load && from_checkpoint;
		if (mContinueBtn != null)
		{
			mParent.RemoveWidget(mContinueBtn);
		}
		if (mZoneBtn != null)
		{
			mParent.RemoveWidget(mZoneBtn);
		}
		if (mBackBtn != null)
		{
			mParent.RemoveWidget(mBackBtn);
		}
		if (mSelectZoneBackBtn != null)
		{
			mParent.RemoveWidget(mSelectZoneBackBtn);
		}
		mBackBtn = null;
		mContinueBtn = null;
		mZoneBtn = null;
		mSelectZoneBackBtn = null;
		mBackBtn = null;
		mContinueBtn = null;
		mZoneBtn = null;
		mSelectZoneBackBtn = null;
		mDisplayingZones = false;
		int theValue = ((GameApp.gApp.GetBoard() == null) ? GameApp.gApp.mUserProfile.GetAdvModeVars().mCurrentAdvScore : GameApp.gApp.GetBoard().mScore);
		if (from_checkpoint && from_load)
		{
			Level checkpointLevel = GameApp.gApp.GetBoard().GetCheckpointLevel();
			disp_level = checkpointLevel.mNum;
			disp_zone = checkpointLevel.mZone;
			theValue = GameApp.gApp.GetBoard().GetCheckpointScore();
		}
		int[] array = new int[6] { 25, 51, 85, 115, 145, 175 };
		if (disp_level == int.MaxValue || disp_level == 10)
		{
			mHighestDot = array[disp_zone - 1];
		}
		else if (disp_zone == 1)
		{
			mHighestDot = (int)((float)disp_level / 10f * (float)array[0]);
		}
		else
		{
			mHighestDot = (int)((float)disp_level / 10f * (float)(array[disp_zone - 1] - array[disp_zone - 2]) + (float)array[disp_zone - 2]);
		}
		if (!zone_completed && !mFromIntro)
		{
			mContinueBtn = new MapButton(1, this);
			mContinueBtn.mUsesAnimators = false;
			mContinueBtn.mMapScreen = this;
			mContinueBtn.mDoFinger = true;
			mContinueBtn.mPriority = 2;
			mContinueBtn.Resize(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAP_CONTINUE_BUTTON) - 80) + Common._DS(14), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAP_CONTINUE_BUTTON)) - Common._DS(Common._M1(6)), imageByID6.GetCelWidth(), imageByID6.GetCelHeight() + Common._DS(Common._M3(40)));
			mParent.AddWidget(mContinueBtn);
			string text = TextManager.getInstance().getString(778);
			string text2 = TextManager.getInstance().getString(798);
			if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CH || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CHT)
			{
				string text3 = text.Substring(0, text.IndexOf("$"));
				string text4 = text2.Substring(0, text2.IndexOf("（"));
				text4 = text4.Replace("$1", "{0}");
				mContinueBtn.mLevel = ((disp_level == int.MaxValue) ? string.Format(text3 + " {0}", disp_zone) : string.Format(text4, (disp_zone - 1) * 10 + disp_level));
			}
			else
			{
				string text5 = text.Substring(0, text.IndexOf(" "));
				string text6 = text2.Substring(0, text2.IndexOf(" "));
				mContinueBtn.mLevel = ((disp_level == int.MaxValue) ? string.Format(text5 + " {0}", disp_zone) : string.Format(text6 + " {0}", (disp_zone - 1) * 10 + disp_level));
			}
			if (GameApp.USE_TRIAL_VERSION && disp_level == int.MaxValue)
			{
				mIsTrialEnd = true;
			}
			string text7 = TextManager.getInstance().getString(863);
			mContinueBtn.mScore = string.Format("{0} " + text7, SexyFramework.Common.CommaSeperate(theValue));
			int num2 = ((GameApp.gApp.GetBoard() == null) ? GameApp.gApp.mUserProfile.GetAdvModeVars().mCurrentAdvLives : GameApp.gApp.GetBoard().GetNumLives());
			if (num2 > 99)
			{
				num2 = 99;
			}
			num2--;
			if (num2 < 0)
			{
				num2 = 2;
			}
			mContinueBtn.mLives = $"x{num2}";
			mZoneBtn = new MapGenericButton(2, this);
			mZoneBtn.mUsesAnimators = false;
			mZoneBtn.mDoFinger = true;
			mZoneBtn.mPriority = 2;
			mZoneBtn.mButtonImage = (mZoneBtn.mOverImage = (mZoneBtn.mDownImage = imageByID7));
			mZoneBtn.mNormalRect = mZoneBtn.mButtonImage.GetCelRect(0);
			mZoneBtn.mOverRect = mZoneBtn.mButtonImage.GetCelRect(0);
			mZoneBtn.mDownRect = mZoneBtn.mButtonImage.GetCelRect(1);
			mZoneBtn.Resize(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAP_BOOK_ANIM) - 80), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAP_BOOK_ANIM)), mZoneBtn.mButtonImage.GetCelWidth(), mZoneBtn.mButtonImage.GetCelHeight());
			mParent.AddWidget(mZoneBtn);
			mBackBtn = new ButtonWidget(3, this);
			mBackBtn.SetVisible(isVisible: false);
			mBackBtn.SetDisabled(isDisabled: true);
			mBackBtn.mDoFinger = true;
			mBackBtn.mPriority = 2;
			mBackBtn.mButtonImage = imageByID;
			mBackBtn.mDownImage = imageByID2;
			float num3 = (float)(imageByID2.GetWidth() - imageByID.GetWidth()) / 2f;
			float num4 = (float)(imageByID2.GetHeight() - imageByID.GetHeight()) / 2f;
			mBackBtn.Resize(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_HOME_SELECT)) - Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_HOME_SELECT)) + (int)num3, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_HOME_SELECT)) + (int)num4, imageByID2.GetWidth(), imageByID2.GetHeight());
			mBackBtn.mNormalRect = new Rect(0, 0, imageByID.GetWidth(), imageByID.GetHeight());
			float num5 = (imageByID2.GetWidth() - imageByID.GetWidth()) / 2;
			float num6 = (imageByID2.GetHeight() - imageByID.GetHeight()) / 2;
			mBackBtn.mDownRect = new Rect((int)num5, (int)num6, imageByID2.GetWidth() - (int)num5, imageByID2.GetHeight() - (int)num6);
			mParent.AddWidget(mBackBtn);
			mSelectZoneBackBtn = new MapGenericButton(4, this);
			mSelectZoneBackBtn.SetVisible(isVisible: false);
			mSelectZoneBackBtn.SetDisabled(isDisabled: true);
			mSelectZoneBackBtn.mUsesAnimators = false;
			mSelectZoneBackBtn.mDoFinger = true;
			mSelectZoneBackBtn.mPriority = 2;
			mSelectZoneBackBtn.mButtonImage = imageByID3;
			mSelectZoneBackBtn.mDownImage = imageByID4;
			int num7 = (GameApp.gApp.GetScreenRect().mWidth - imageByID5.mWidth) / 2;
			mSelectZoneBackBtn.Resize(num7 + Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_MAP_OPENBOOK_BACK)), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_MAP_OPENBOOK_BACK)), imageByID3.GetWidth(), imageByID3.GetHeight());
			mParent.AddWidget(mSelectZoneBackBtn);
			mSelectZoneBackBtn.SetVisible(isVisible: false);
			mSelectZoneBackBtn.SetDisabled(isDisabled: true);
			gZoneNames[0] = TextManager.getInstance().getString(839);
			gZoneNames[1] = TextManager.getInstance().getString(840);
			gZoneNames[2] = TextManager.getInstance().getString(841);
			gZoneNames[3] = TextManager.getInstance().getString(842);
			gZoneNames[4] = TextManager.getInstance().getString(843);
			gZoneNames[5] = TextManager.getInstance().getString(844);
			int num8 = ((disp_zone == 0) ? disp_zone : (disp_zone - 1));
			string theString = gZoneNames[num8];
			mNewZoneTextImg = new DeviceImage();
			mNewZoneTextImg.SetImageMode(hasTrans: true, hasAlpha: true);
			mNewZoneTextImg.AddImageFlags(16u);
			mNewZoneTextImg.Create(fontByID.StringWidth(theString) + 60, fontByID.mHeight * 2);
			Graphics graphics = new Graphics(mNewZoneTextImg);
			graphics.Get3D().ClearColorBuffer(new Color(0, 0));
			graphics.SetFont(fontByID);
			graphics.SetColor(Color.White);
			graphics.DrawString(theString, 0, fontByID.GetAscent());
			graphics.ClearRenderContext();
		}
		else if (disp_zone > 1 || mFromIntro)
		{
			mNewZoneTextImg = new DeviceImage();
			mNewZoneTextImg.SetImageMode(hasTrans: true, hasAlpha: true);
			mNewZoneTextImg.AddImageFlags(16u);
			mNewZoneTextImg.Create(fontByID.StringWidth("Underwater Grotto!") + 60, fontByID.mHeight * 2);
			Graphics graphics2 = new Graphics(mNewZoneTextImg);
			graphics2.Get3D().ClearColorBuffer(new Color(0, 0));
			graphics2.SetFont(fontByID);
			graphics2.SetColor(Color.White);
			string theString2 = string.Format("{0} {1}", LevelMgr.GetZoneName(disp_zone - 1), mBeatGame ? "..." : "");
			if (mFromIntro)
			{
				graphics2.DrawString(TextManager.getInstance().getString(661), (mNewZoneTextImg.mWidth - fontByID.StringWidth(TextManager.getInstance().getString(661))) / 2, fontByID.GetAscent());
				graphics2.DrawString(TextManager.getInstance().getString(662), (mNewZoneTextImg.mWidth - fontByID.StringWidth(TextManager.getInstance().getString(662))) / 2, fontByID.GetAscent() + fontByID.mHeight - Common._DS(Common._M(30)) + Localization.GetCurrentFontOffsetY());
			}
			else if (!mBeatGame)
			{
				graphics2.DrawString(theString2, (mNewZoneTextImg.mWidth - fontByID.StringWidth(theString2)) / 2, fontByID.GetAscent());
				graphics2.DrawString(TextManager.getInstance().getString(663), (mNewZoneTextImg.mWidth - fontByID.StringWidth(TextManager.getInstance().getString(663))) / 2, fontByID.GetAscent() + fontByID.mHeight - Common._DS(Common._M(10)));
			}
			else
			{
				graphics2.DrawString(TextManager.getInstance().getString(661), (mNewZoneTextImg.mWidth - fontByID.StringWidth(TextManager.getInstance().getString(661))) / 2, fontByID.GetAscent());
				graphics2.DrawString(theString2, (mNewZoneTextImg.mWidth - fontByID.StringWidth(theString2)) / 2, fontByID.GetAscent() + fontByID.mHeight - Common._DS(Common._M(30)));
			}
			mNewZoneTextSize = 0f;
			mNewZoneTextBounceCount = 0;
			graphics2.ClearRenderContext();
		}
		for (int i = 0; i < 5; i++)
		{
			mOverlays[i].mUnlocked = i + 1 <= num;
			if (mOverlays[i].mUnlocked)
			{
				mOverlays[i].mAlpha = 0f;
			}
		}
		zone_effects[0] = null;
		zone_effects[1] = GameApp.gApp.GetPIEffect("goldsparkle_area_L2");
		zone_effects[2] = GameApp.gApp.GetPIEffect("goldsparkle_area_L3");
		zone_effects[3] = GameApp.gApp.GetPIEffect("goldsparkle_area_L4");
		zone_effects[4] = GameApp.gApp.GetPIEffect("goldsparkle_area_L5");
		zone_effects[5] = GameApp.gApp.GetPIEffect("goldsparkle_area_L6");
		mUnlockNameAlpha.SetConstant(1.0);
		mUnlockNameHilite.SetConstant(1.0);
		mUnlockOutlineAlpha.SetConstant(1.0);
		mUnlockIconAlpha.SetConstant(1.0);
		mClickToEnterAlpha.SetConstant(1.0);
		if (zone_completed)
		{
			mUnlockScrollAmt = Common._DS(Common._M(-206));
			mZoneEffect = zone_effects[disp_zone - 1];
			mZoneEffect.ResetAnim();
			mZoneEffect.mEmitAfterTimeline = true;
			ResID resID = (ResID)(1294 + disp_zone - 1);
			mZoneEffect.GetLayer("general sparkle").GetEmitter("sparkle area").mMaskImage = GameApp.gApp.mResourceManager.GetResourceRef(0, resID.ToString()).GetSharedImageRef();
			mZoneEffect.mDrawTransform.LoadIdentity();
			float num9 = GameApp.DownScaleNum(1f);
			mZoneEffect.mDrawTransform.Scale(num9, num9);
			mZoneEffect.mDrawTransform.Translate(Common._DS(Res.GetOffsetXByID(resID)) - Common._DS(80), Common._DS(Res.GetOffsetYByID(resID)));
			for (int j = 0; j < disp_zone - 1; j++)
			{
				mOverlays[j].mUnlocked = true;
			}
			if (disp_zone > 1 && num < 6)
			{
				mOverlays[disp_zone - 2].mAlpha = 255f;
			}
			mUnlockNameAlpha.SetCurve(Common._MP("b;0,1,0.002,1,#########   z#### K~###  (~###  T~###"));
			mUnlockNameHilite.SetCurve(Common._MP("b+0,1,0,1,####  R#### =~m&F    a#### Q####"), mUnlockNameAlpha);
			mUnlockOutlineAlpha.SetCurve(Common._MP("b;0,1,0.001429,1,#########   }%###      $f###"));
			mUnlockIconAlpha.SetCurve(Common._MP("b+0,1,0,1,####  b#### .?### R#### oL###jL###  (####"), mUnlockNameAlpha);
			mClickToEnterAlpha.SetCurve(Common._MP("b+0,1,0,1,####     y#### b~###  D~###"), mUnlockNameAlpha);
			mDisableInput = true;
		}
		if (mFromIntro)
		{
			mUnlockNameAlpha.SetCurve(Common._MP("b+0,1,0.002,1,#########      :#### &~###  d~###"));
			mUnlockNameHilite.SetCurve(Common._MP("b+0,1,0,1,####     r#### T~### h####o####"), mUnlockNameAlpha);
			mUnlockOutlineAlpha.SetCurve(Common._MP("b+0,1,0.001429,1,#########   }%###      $N###"));
			mUnlockIconAlpha.SetCurve(Common._MP("b+0,1,0,1,####  b#### .?### R#### oL###jL###  (####"), mUnlockNameAlpha);
			mClickToEnterAlpha.SetCurve(Common._MP("b+0,1,0,1,####       w####h~### @~####~###"), mUnlockNameAlpha);
		}
		mRemove = false;
		mSelectedZone = -1;
		if (mFromCheckpoint)
		{
			ButtonDepress(mZoneBtn.mId);
		}
	}

	public void Update()
	{
		if (GameApp.gApp.IsHardwareBackButtonPressed())
		{
			ProcessHardwareBackButton();
		}
		mDirty = false;
		int num = Common._DS(Common._M(-206));
		foreach (Widget mWidget in mParent.mWidgets)
		{
			mWidget.SetDisabled(mAlpha.mRamp == 6);
		}
		if (mAlpha.mRamp == 6)
		{
			mDirty = true;
			if (!mAlpha.IncInVal())
			{
				if ((double)mAlpha == 0.0)
				{
					CleanButtons();
					GameApp.gApp.mClickedHardMode = false;
					GameApp.gApp.HideAdventureModeMapScreen();
					return;
				}
				mAlpha.SetConstant(mAlpha);
			}
			GameApp.gApp.mMainMenu.MarkAllDirty();
			return;
		}
		if (mSlideDir != 0)
		{
			mDirty = true;
			float num2 = Common._M(60f);
			mXOff += (float)mSlideDir * num2;
			mZoneBtn.mX += (int)(num2 * (float)mSlideDir);
			mBackBtn.mX += (int)(num2 * (float)mSlideDir);
			mContinueBtn.mX += (int)(num2 * (float)mSlideDir);
			if (mSlideDir == -1)
			{
				if (mXOff <= 0f)
				{
					mSlideDir = 0;
					float num3 = 0f - mXOff;
					mXOff = 0f;
					mZoneBtn.mX += (int)num3;
					mBackBtn.mX += (int)num3;
					mContinueBtn.mX += (int)num3;
				}
			}
			else if (mSlideDir == 1 && mXOff >= (float)(GameApp.gApp.mWidth + Common._S(80)))
			{
				mXOff = GameApp.gApp.mWidth + Common._S(80);
				mSlideDir = 0;
				CleanButtons();
				GameApp.gApp.mClickedHardMode = false;
				GameApp.gApp.HideAdventureModeMapScreen();
			}
			GameApp.gApp.mMainMenu.MarkAllDirty();
			return;
		}
		if (mIncDisplayZoneAlpha)
		{
			mDisplayZoneAlpha += 5f;
			if (mDisplayZoneAlpha >= 255f)
			{
				mDisplayZoneAlpha = 255f;
				mIncDisplayZoneAlpha = false;
			}
		}
		else
		{
			mDisplayZoneAlpha -= 5f;
			if (mDisplayZoneAlpha <= 0f)
			{
				mDisplayZoneAlpha = 0f;
				mIncDisplayZoneAlpha = true;
			}
		}
		Board board = GameApp.gApp.GetBoard();
		if (board != null && board.mEndBossFadeAmt > 0f)
		{
			mDirty = true;
			board.mEndBossFadeAmt -= Common._M(2f);
			if (!(board.mEndBossFadeAmt < 0f))
			{
				return;
			}
			board.mEndBossFadeAmt = 0f;
		}
		mUpdateCount++;
		Common._M(50);
		Common._M(60);
		if (mUnlockScrollAmt <= (float)num && !mHasPlayedZoneUnlockedSound && mNewZoneTextImg != null && !mBeatGame && mUpdateCount >= Common._M(150))
		{
			mHasPlayedZoneUnlockedSound = true;
		}
		if (mFromIntro && mUpdateCount >= Common._M(100))
		{
			mUnlockNameAlpha.IncInVal();
			mUnlockOutlineAlpha.IncInVal();
		}
		if (mUpdateCount >= Common._M(60) && (mUnlockScrollAmt <= (float)num || (mFromIntro && mUpdateCount >= Common._M(130))))
		{
			if (mNewZoneTextBounceCount < Common._M(5))
			{
				mDirty = true;
				float num4 = Common._M(0.1f) * (float)((mNewZoneTextBounceCount % 2 == 0) ? 1 : (-1));
				float num5 = Common._M(1.5f);
				if (mNewZoneTextBounceCount >= 2)
				{
					num5 /= 2f * (float)(mNewZoneTextBounceCount / 2);
				}
				mNewZoneTextSize += num4;
				if (num4 > 0f && mNewZoneTextSize > 1f + num5)
				{
					mNewZoneTextSize = 1f + num5;
					mNewZoneTextBounceCount++;
				}
				else if (num4 < 0f && mNewZoneTextSize <= 1f)
				{
					mNewZoneTextSize = 1f;
					mNewZoneTextBounceCount++;
				}
			}
			else if (mDisableInput)
			{
				mDisableInput = false;
			}
			if (!mFromIntro)
			{
				mZoneEffect.mDrawTransform.LoadIdentity();
				float num6 = GameApp.DownScaleNum(1f);
				mZoneEffect.mDrawTransform.Scale(num6, num6);
				mZoneEffect.mDrawTransform.Translate((float)(mAreaCoords[mDisplayedZone - 1].mX - Common._DS(80)) + mUnlockScrollAmt, mAreaCoords[mDisplayedZone - 1].mY + Common._DS(mZoneEffect.mHeight / 2));
				mZoneEffect.Update();
				if (mZoneEffect.mCurNumParticles > 0)
				{
					mDirty = true;
				}
			}
		}
		if (mUpdateCount >= Common._M(25))
		{
			for (int i = 0; i < 5; i++)
			{
				if (!mOverlays[i].mUnlocked || !(mOverlays[i].mAlpha > 0f))
				{
					continue;
				}
				if (mOverlays[i].mAlpha > 0f)
				{
					mDirty = true;
				}
				mOverlays[i].mAlpha -= Common._M(0.95f);
				if (mOverlays[i].mAlpha < 0f)
				{
					mOverlays[i].mAlpha = 0f;
				}
				for (int j = 0; j < 3; j++)
				{
					float num7 = Common._M(0.35f) + (float)j * Common._M1(0.15f);
					if (j == 1)
					{
						num7 *= -1f;
					}
					mOverlays[i].mCloudPoints[j].mX -= num7;
				}
			}
		}
		if (mZoneOver)
		{
			mZoneOverPct = Math.Min(1f, mZoneOverPct + Common._M(0.05f));
		}
		else
		{
			mZoneOverPct = Math.Max(0f, mZoneOverPct - Common._M(0.075f));
		}
		if (GameApp.gApp.mHasFocus)
		{
			mDirty = true;
		}
	}

	public void Draw(Graphics g)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_MAP_BKGRND);
		Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_HOME);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_HOME_BACKING);
		Font fontByID = Res.GetFontByID(ResID.FONT_SHAGLOUNGE28_BASE);
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(mAlpha);
		g.DrawImage(imageByID, (int)((float)Common._S(0) + mXOff + mUnlockScrollAmt), 0);
		for (int i = 1; i < 7; i++)
		{
			if (i == mDisplayedZone)
			{
				g.SetColor(mAlpha);
				int id = 1294 + i - 1;
				int theX = Common._DS(Res.GetOffsetXByID((ResID)id)) + Common._S(0) + (int)mUnlockScrollAmt;
				int theY = Common._DS(Res.GetOffsetYByID((ResID)id));
				Image imageByID3 = Res.GetImageByID((ResID)id);
				double val = 0.0;
				g.SetColor(255, 255, 255, (int)(255.0 * Math.Min(1.0, val)));
				g.DrawImage(imageByID3, theX, theY);
				if (mBackBtn != null)
				{
					_ = mBackBtn.mVisible;
				}
				bool flag = false;
				if (GameApp.gApp.mBoard != null && GameApp.gApp.mBoard.mDoingFirstTimeIntro && GameApp.gApp.mBoard.mShowMapScreen && (double)GameApp.gApp.mBoard.mIntroMapScale == 0.0 && !GameApp.gApp.mBoard.mDoIntroFrogJump)
				{
					flag = false;
				}
				else if (GameApp.gApp.mBoard != null && GameApp.gApp.mBoard.mDoingFirstTimeIntro && GameApp.gApp.mBoard.mShowMapScreen)
				{
					flag = true;
				}
				bool flag2 = false;
				if (GameApp.gApp.mBoard != null)
				{
					flag2 = GameApp.gApp.mBoard.mDoingFirstTimeIntro;
				}
				if ((mCompletedZone || flag2) && !flag)
				{
					g.SetDrawMode(1);
					g.SetColor(255, 255, 255, (int)mDisplayZoneAlpha);
					g.DrawImage(imageByID3, theX, theY);
					g.SetDrawMode(0);
				}
			}
			else
			{
				DrawDesaturatedZone(g, i, Common._M(1f));
			}
		}
		g.SetColor(mAlpha);
		for (int j = 0; j < 5; j++)
		{
			if (mOverlays[j].mAlpha > 0f)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)((double)mOverlays[j].mAlpha * (double)mAlpha));
				for (int k = 0; k < 3; k++)
				{
					Image imageByID4 = Res.GetImageByID((ResID)(1304 + k));
					float num = (float)imageByID4.mWidth * mOverlays[j].mCloudSizes[k].mX * 2f;
					float num2 = (float)imageByID4.mHeight * mOverlays[j].mCloudSizes[k].mY * 2f;
					g.DrawImage(imageByID4, (int)(Common._DS(mOverlays[j].mCloudPoints[k].mX - 0f) + mXOff + mUnlockScrollAmt), (int)Common._DS(mOverlays[j].mCloudPoints[k].mY), (int)num, (int)num2);
				}
			}
		}
		if (mDisplayingZones)
		{
			g.SetFont(fontByID);
			DrawZoneSelectBackground(g);
			for (int l = 0; l < 6; l++)
			{
				GetZoneImage(l, out var outZoneImage, out var outZoneRect);
				DrawZoneImage(g, l, outZoneImage, outZoneRect);
				DrawZoneName(g, l, outZoneRect);
				DrawZoneLockedOverlay(g, l, outZoneRect);
				Common.DrawCommonDialogBorder(g, outZoneRect.mX - Common._DS(10), outZoneRect.mY - Common._DS(7), outZoneRect.mWidth + Common._DS(20), outZoneRect.mHeight + Common._DS(14));
			}
		}
		else
		{
			DrawMapZoneName(g);
		}
		if (mBackBtn != null && mBackBtn.mVisible)
		{
			g.DrawImage(imageByID2, -84, 0);
		}
	}

	public void MouseMove(int x, int y)
	{
	}

	public void ButtonPress(int theId, int theClickCount)
	{
	}

	public void MouseDown(int x, int y)
	{
		if (mDisableInput || (GameApp.gApp.mBambooTransition != null && GameApp.gApp.mBambooTransition.IsInProgress()))
		{
			return;
		}
		mLastMouseX = x;
		mLastMouseY = y;
		if (mFromIntro)
		{
			if (mZoneOver)
			{
				GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_MAPZOOMIN));
				GameApp.gApp.mUserProfile.mNeedsFirstTimeIntro = false;
				GameApp.gApp.PlaySong(12);
			}
		}
		else if (mSlideDir == 0 && !mRemove)
		{
			if (mCompletedZone)
			{
				GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_MAPZOOMIN));
				GameApp.gApp.SetCursor(ECURSOR.CURSOR_POINTER);
				mClosing = true;
			}
			OnZoneCardSelected();
		}
	}

	public void MouseLeave()
	{
	}

	public void ButtonMouseMove(int theId, int theX, int theY)
	{
	}

	public void ButtonDownTick(int theId)
	{
	}

	public void ButtonMouseLeave(int theId)
	{
	}

	public void MouseUp(int x, int y)
	{
		if (!mDisableInput && (GameApp.gApp.mBambooTransition == null || !GameApp.gApp.mBambooTransition.IsInProgress()) && mSelectedZone != -1)
		{
			if (mCards[mSelectedZone - 1].Contains(x, y) && (mSelectedZone - 1 == 0 || mOverlays[mSelectedZone - 2].mUnlocked) && GameApp.gApp.mDialogMap.Count() == 0)
			{
				GameApp.gApp.DoYesNoDialog("", TextManager.getInstance().getString(452), block: true, TextManager.getInstance().getString(446), TextManager.getInstance().getString(447), drag: false, -1, 1, 0);
				GameApp.gApp.mYesNoDialogDelegate = ProcessYesNo;
			}
			else
			{
				mSelectedZone = -1;
			}
		}
	}

	public void ProcessYesNo(int theId)
	{
		_ = (GameApp)GlobalMembers.gSexyApp;
		if (theId == 1000)
		{
			mRemove = true;
			CleanButtons();
		}
		else
		{
			mSelectedZone = -1;
		}
	}

	public void DoSlide(bool slide_in)
	{
		if (slide_in)
		{
			mAlpha.SetCurve(Common._MP("b-0,1,0.02,1,####        n~### 3~###"));
			return;
		}
		mFadingOut = true;
		mAlpha.SetCurve(Common._MP("b-0,1,0.02,1,~###         ~####"));
	}

	public Point GetZoneCenter(int theZoneNum)
	{
		return mZoneCenters[theZoneNum - 1];
	}

	public virtual void ButtonDepress(int id)
	{
		if ((GameApp.gApp.mBambooTransition != null && GameApp.gApp.mBambooTransition.IsInProgress()) || mSlideDir != 0)
		{
			return;
		}
		if (id == mContinueBtn.mId)
		{
			if (GameApp.USE_TRIAL_VERSION && mIsTrialEnd)
			{
				if (GameApp.gApp.mBoard != null)
				{
					GameApp.gApp.mBoard.Pause(pause: true, becauseOfDialog: true);
				}
				string message = TextManager.getInstance().getString(832);
				int width_pad = Common._DS(Common._M(20));
				GameApp.gApp.DoYesNoDialog(TextManager.getInstance().getString(448), message, block: true, TextManager.getInstance().getString(446), TextManager.getInstance().getString(447), drag: false, Common._S(Common._M(50)), 1, width_pad);
				GameApp.gApp.mYesNoDialogDelegate = ProcessTrialYesNo;
				mIsTryAndBuyDialogShowing = true;
				return;
			}
			if (GameApp.gApp.mResourceManager.IsGroupLoaded("MenuRelated"))
			{
				GameApp.gApp.mResourceManager.DeleteResources("MenuRelated");
			}
			mContinueBtn.mLevel = "";
			mContinueBtn.mScore = "";
			mContinueBtn.mLives = "";
			mRemove = true;
			if (mContinueGoesToCheckpoint)
			{
				mContinueFromCheckpoint = true;
			}
		}
		else if (id == mBackBtn.mId)
		{
			if (GameApp.gApp.GetBoard() != null)
			{
				CleanButtons();
				GameApp.gApp.mClickedHardMode = false;
				GameApp.gApp.EndCurrentGame();
				GameApp.gApp.ShowMainMenu();
			}
			else
			{
				CleanButtons();
				GameApp.gApp.mClickedHardMode = false;
				GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.HideAdventureModeMapScreen;
			}
			GameApp.gApp.ToggleBambooTransition();
		}
		else if (id == mSelectZoneBackBtn.mId)
		{
			if (!mFromCheckpoint)
			{
				mZoneBtn.mDisabled = (mContinueBtn.mDisabled = false);
				mZoneBtn.mVisible = (mContinueBtn.mVisible = true);
				mDisplayingZones = false;
			}
			else
			{
				CleanButtons();
				mRemove = true;
			}
		}
		else if (id == mZoneBtn.mId)
		{
			mDisplayingZones = true;
			mZoneBtn.mDisabled = (mContinueBtn.mDisabled = true);
			mZoneBtn.mVisible = (mContinueBtn.mVisible = false);
		}
	}

	public void ButtonPress(int id)
	{
		if (GameApp.gApp.mBambooTransition == null || !GameApp.gApp.mBambooTransition.IsInProgress())
		{
			int soundByID = Res.GetSoundByID(ResID.SOUND_BUTTON1);
			int soundByID2 = Res.GetSoundByID(ResID.SOUND_BUTTON2);
			int soundByID3 = Res.GetSoundByID(ResID.SOUND_BUTTON3);
			if (mContinueBtn.mId == id)
			{
				GameApp.gApp.PlaySample(soundByID3);
			}
			else if (mBackBtn.mId == id || mZoneBtn.mId == id)
			{
				GameApp.gApp.PlaySample(soundByID2);
			}
			else
			{
				GameApp.gApp.PlaySample(soundByID);
			}
		}
	}

	public virtual void ButtonMouseEnter(int id)
	{
		mLastMouseX = (mLastMouseY = -1);
	}

	public void ProcessHardwareBackButton()
	{
		if (GameApp.gApp.mBambooTransition != null && GameApp.gApp.mBambooTransition.IsInProgress())
		{
			GameApp.gApp.OnHardwareBackButtonPressProcessed();
			return;
		}
		if (mIsTryAndBuyDialogShowing)
		{
			Dialog dialog = GameApp.gApp.GetDialog(1);
			if (dialog != null)
			{
				dialog.ButtonDepress(1001);
				GameApp.gApp.OnHardwareBackButtonPressProcessed();
				return;
			}
		}
		if (mDisplayingZones)
		{
			if (mSelectedZone != -1)
			{
				Dialog dialog2 = GameApp.gApp.GetDialog(1);
				if (dialog2 != null)
				{
					dialog2.ButtonDepress(1001);
					GameApp.gApp.OnHardwareBackButtonPressProcessed();
					return;
				}
			}
			mZoneBtn.mDisabled = (mContinueBtn.mDisabled = false);
			mZoneBtn.mVisible = (mContinueBtn.mVisible = true);
			mDisplayingZones = false;
			GameApp.gApp.OnHardwareBackButtonPressProcessed();
		}
		else
		{
			if (GameApp.gApp.GetBoard() != null)
			{
				CleanButtons();
				GameApp.gApp.mClickedHardMode = false;
				GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.DoDeferredEndGame;
			}
			else
			{
				CleanButtons();
				GameApp.gApp.mClickedHardMode = false;
				GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.HideAdventureModeMapScreen;
			}
			GameApp.gApp.ToggleBambooTransition();
			GameApp.gApp.OnHardwareBackButtonPressProcessed();
		}
	}

	private void DrawZoneSelectBackground(Graphics g)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_MAP_OPENBOOK_PAGES);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BOOKTEXT);
		int theX = (GameApp.gApp.GetScreenRect().mWidth - imageByID.mWidth) / 2 + Common._DS(Common._M(0));
		int theY = (GameApp.gApp.mHeight - imageByID.mHeight) / 2;
		g.DrawImage(imageByID, theX, theY);
		int num = 4;
		g.DrawImage(imageByID2, theX, theY, imageByID2.mWidth - num, imageByID2.mHeight);
	}

	private void GetZoneImage(int inZoneID, out Image outZoneImage, out Rect outZoneRect)
	{
		Image levelThumbnail = GameApp.gApp.GetLevelThumbnail(inZoneID * 10);
		Rect rect = mCards[inZoneID];
		rect.mWidth = (int)(Common._S(2f) * Common._S(0.55f) * (float)levelThumbnail.mWidth);
		rect.mHeight = (int)(Common._S(2f) * Common._S(0.55f) * (float)levelThumbnail.mHeight);
		outZoneImage = levelThumbnail;
		outZoneRect = rect;
	}

	private void DrawZoneImage(Graphics g, int inZoneID, Image inZoneImage, Rect inZoneRect)
	{
		g.PushState();
		g.SetColorizeImages(colorizeImages: false);
		g.DrawImage(inZoneImage, inZoneRect.mX, inZoneRect.mY, inZoneRect.mWidth, inZoneRect.mHeight);
		if (mSelectedZone - 1 == inZoneID)
		{
			g.PushState();
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)((double)Common._M(100) * (double)mAlpha));
			g.SetDrawMode(1);
			g.DrawImage(inZoneImage, inZoneRect.mX, inZoneRect.mY, inZoneRect.mWidth, inZoneRect.mHeight);
			g.PopState();
		}
		g.PopState();
	}

	private void DrawZoneName(Graphics g, int inZoneID, Rect inZoneRect)
	{
		string theString = $"{inZoneID + 1} - {gZoneNames[inZoneID]}";
		int num = g.GetFont().StringWidth(theString);
		int theX = inZoneRect.mX + (inZoneRect.mWidth - num) / 2;
		int theY = inZoneRect.mY + inZoneRect.mHeight + Common._DS(50);
		g.SetColor(Color.Black);
		g.DrawString(theString, theX, theY);
	}

	private void DrawZoneLockedOverlay(Graphics g, int inZoneID, Rect inZoneRect)
	{
		if (inZoneID <= 0 || mOverlays[inZoneID - 1].mUnlocked)
		{
			return;
		}
		g.SetColor(0, 0, 0, 191);
		g.FillRect(inZoneRect);
		string theString = TextManager.getInstance().getString(664);
		g.SetColor(Common._M(255), Common._M1(0), Common._M2(0), (int)((double)Common._M3(255) * (double)mAlpha));
		int num = g.GetFont().StringWidth(theString);
		int mHeight = g.GetFont().mHeight;
		int num2 = inZoneRect.mX + (inZoneRect.mWidth - num) / 2 + Common._DS(15);
		int num3 = inZoneRect.mY + Common._DS(Common._M(119));
		float num4 = 0f;
		if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_RU)
		{
			num4 = 0.8f;
		}
		Graphics3D graphics3D = g.Get3D();
		if (graphics3D != null)
		{
			num2 += Common._DS(Common._M(20));
			SexyTransform2D sexyTransform2D = new SexyTransform2D(init: false);
			sexyTransform2D.Translate(-num2 - num / 2 + GlobalMembers.gSexyApp.mScreenBounds.mX, -num3 - mHeight / 2);
			sexyTransform2D.RotateDeg(Common._M(45));
			if (num4 != 0f)
			{
				sexyTransform2D.Scale(num4, num4);
			}
			sexyTransform2D.Translate(num2 + num / 2 - GlobalMembers.gSexyApp.mScreenBounds.mX, num3 + mHeight / 2);
			graphics3D.PushTransform(sexyTransform2D);
			g.DrawString(theString, num2, num3);
			graphics3D.PopTransform();
		}
		else
		{
			g.DrawString(theString, num2, num3);
		}
	}

	private void DrawMapZoneName(Graphics g)
	{
		g.SetColor(255, 255, 255, (int)(255.0 * (double)mExtrasAlpha * (double)mAlpha));
		if (mNewZoneTextSize > 0f)
		{
			mGlobalTranform.Reset();
			if (g.Is3D())
			{
				mGlobalTranform.Scale(mNewZoneTextSize, mNewZoneTextSize);
			}
			int num = (int)((mXOff + (float)GameApp.gApp.GetScreenRect().mWidth + (float)GameApp.gApp.GetScreenRect().mX) / 2f);
			if (mFromIntro)
			{
				g.DrawImageTransform(mNewZoneTextImg, mGlobalTranform, num, Common._DS(Common._M1(1000)));
			}
			else if (mCompletedZone)
			{
				g.DrawImageTransform(mNewZoneTextImg, mGlobalTranform, num, Common._DS(Common._M1(960)));
			}
			else
			{
				g.DrawImageTransform(mNewZoneTextImg, mGlobalTranform, Common._DS(1000) + GameApp.gApp.GetScreenRect().mX - GameApp.gApp.mWideScreenXOffset, Common._DS(Common._M1(150)));
			}
		}
		if (mZoneEffect != null && mUpdateCount > Common._M(50) && !mFromIntro)
		{
			mZoneEffect.Draw(g);
		}
	}

	private void OnZoneCardSelected()
	{
		if (!mDisplayingZones)
		{
			return;
		}
		for (int i = 0; i < 6; i++)
		{
			if (MouseOverCard(i))
			{
				mSelectedZone = i + 1;
				GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BUTTON1));
				break;
			}
		}
	}

	public void ProcessTrialYesNo(int theId)
	{
		switch (theId)
		{
		case 1000:
			GameApp.gApp.ToMarketPlace();
			mIsTryAndBuyDialogShowing = false;
			break;
		case 1001:
			if (GameApp.gApp.GetBoard() != null)
			{
				CleanButtons();
				GameApp.gApp.mClickedHardMode = false;
				GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.DoDeferredEndGame;
			}
			else
			{
				CleanButtons();
				GameApp.gApp.mClickedHardMode = false;
				GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.HideAdventureModeMapScreen;
			}
			GameApp.gApp.ToggleBambooTransition();
			mIsTryAndBuyDialogShowing = false;
			break;
		}
	}
}
