using System.Threading;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class Achievements : Widget, ButtonListener
{
	private enum ButtonState
	{
		AdvStats_Btn,
		HardAdvStats_Btn,
		Challenge_Btn,
		IronFrog_Btn,
		MoreStats_Btn,
		Back_Btn,
		Next_Btn,
		Prev_Btn
	}

	private int mSelectedScreenState;

	protected ButtonWidget mHomeButton;

	protected ButtonWidget mUpButton;

	protected ButtonWidget mDownButton;

	protected int mDisplayMode;

	protected int mBounceCount;

	protected AchievementsPages mAchievementsPages;

	protected PageControl mAchievementsPageControl;

	protected ScrollWidget mAchievementsScrollWidget;

	protected bool mNeedsInitScroll;

	protected float mTitleXOffset;

	protected int mAspectOffset = 30;

	protected Image IMAGE_UI_CHALLENGE_PAGE_INDICATOR = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGE_PAGE_INDICATOR);

	protected Image IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE);

	protected Image IMAGE_UI_CHALLENGESCREEN_BG_FLOOR = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_BG_FLOOR);

	protected Image IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END);

	protected Image IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_UP = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_UP);

	protected Image IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES);

	protected Image IMAGE_UI_CHALLENGESCREEN_WOOD = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_WOOD);

	protected Image IMAGE_UI_LEADERBOARDS_LEAVES2 = Res.GetImageByID(ResID.IMAGE_UI_LEADERBOARDS_LEAVES2);

	protected Image IMAGE_UI_CHALLENGESCREEN_BG_SIDE = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_BG_SIDE);

	protected Image IMAGE_GUI_Achievements_PEDESTAL = Res.GetImageByID(ResID.IMAGE_GUI_TIKITEMPLE_PEDESTAL);

	protected Image IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1 = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1);

	protected Image IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2 = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2);

	protected Image IMAGE_UI_LEADERBOARDS_BOSSES = Res.GetImageByID(ResID.IMAGE_UI_LEADERBOARDS_BOSSES);

	protected Image IMAGE_UI_CHALLENGESCREEN_FRUIT = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_FRUIT);

	protected Image IMAGE_UI_LEADERBOARDS_SHADOW = Res.GetImageByID(ResID.IMAGE_UI_LEADERBOARDS_SHADOW2);

	public float mXOff;

	protected int mCurrentPage;

	private Thread mLoadDataThread;

	private ThreadStart mLoadingProc;

	private bool mLoadingData;

	private bool mLoadingDataComplete;

	private Font mLoadingFont = Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_BASE);

	private Font mTitleFont = Res.GetFontByID(ResID.FONT_SHAGEXOTICA38_BLACK);

	private string loadingDot = "";

	private ulong mTicker = SexyFramework.Common.SexyTime();

	private bool mEnterScreneLoad;

	public Achievements()
	{
		if (!GameApp.gApp.mResourceManager.IsGroupLoaded("CommonGame") && !GameApp.gApp.mResourceManager.LoadResources("CommonGame"))
		{
			GameApp.gApp.Shutdown();
		}
		mDisplayMode = -1;
		mClip = false;
		mSelectedScreenState = 0;
		mHomeButton = null;
		mUpButton = null;
		mDownButton = null;
		if (GameApp.mGameRes == 768)
		{
			mTitleXOffset = 30f;
		}
		else
		{
			mTitleXOffset = 20f;
		}
		mNeedsInitScroll = true;
	}

	public override void Dispose()
	{
		RemoveAllWidgets(doDelete: true, recursive: true);
	}

	public void StartLoading()
	{
		mLoadingProc = LoadingRank;
		mLoadDataThread = new Thread(mLoadingProc);
		mLoadingData = true;
		mLoadingDataComplete = false;
		mLoadDataThread.Start();
	}

	private void LoadingRank()
	{
		mAchievementsPages.AddPage();
		mAchievementsPages.AddPage();
		mAchievementsPages.AddPage();
		mAchievementsPages.AddPage();
		mAchievementsPages.AddPage();
		mAchievementsPages.Resize(0, 0, mAchievementsPages.IMAGE_UI_LEADERBOARDS_SHADOW.GetWidth(), (mAchievementsPages.IMAGE_UI_LEADERBOARDS_SHADOW.GetHeight() + 30) * mAchievementsPages.mNumPages - 100);
		mAchievementsScrollWidget.AddWidget(mAchievementsPages);
		mLoadingDataComplete = true;
	}

	public void Init()
	{
		mAchievementsPages = new AchievementsPages(this);
		mAchievementsScrollWidget = new ScrollWidget();
		mAchievementsScrollWidget.Resize(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_LEADERBOARDS_SHADOW)) - GameApp.gApp.mWideScreenXOffset + Common._DS(10), 20 + Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_LEADERBOARDS_SHADOW)), IMAGE_UI_LEADERBOARDS_SHADOW.GetWidth() + 30, IMAGE_UI_LEADERBOARDS_SHADOW.GetHeight() - 40);
		mAchievementsScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_VERTICAL);
		mAchievementsScrollWidget.EnableBounce(enable: true);
		mAchievementsScrollWidget.EnablePaging(enable: true);
		mAchievementsPageControl = new PageControl(IMAGE_UI_CHALLENGE_PAGE_INDICATOR);
		IMAGE_UI_CHALLENGE_PAGE_INDICATOR.GetCelWidth();
		mAchievementsPages.NumPages();
		mAchievementsPageControl.SetNumberOfPages(mAchievementsPages.NumPages());
		mAchievementsPageControl.Move((int)mTitleXOffset + (mWidth - mAchievementsPageControl.mWidth) / 2, Common._DS(145));
		mAchievementsPageControl.SetCurrentPage(0);
		AddWidget(mAchievementsPageControl);
		mAchievementsScrollWidget.SetPageControl(mAchievementsPageControl);
		AddWidget(mAchievementsScrollWidget);
		mUpButton = new ButtonWidget(7, this);
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_LEADERBOARDS_ARROW_LIGHT);
		mUpButton.mButtonImage = imageByID;
		mUpButton.mDownImage = Res.GetImageByID(ResID.IMAGE_UI_LEADERBOARDS_ARROW_LIGHT_ON);
		float num = 0f;
		float num2 = 0f;
		mUpButton.Resize(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_LEADERBOARDS_ARROW_LIGHT)) - GameApp.gApp.mWideScreenXOffset + mAspectOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_LEADERBOARDS_ARROW_LIGHT)), imageByID.GetWidth(), imageByID.GetHeight());
		mUpButton.mNormalRect = new Rect(0, 0, imageByID.GetWidth(), imageByID.GetHeight());
		mUpButton.mDownRect = new Rect((int)num, (int)num2, imageByID.GetWidth() - (int)num, imageByID.GetHeight() - (int)num2);
		mUpButton.mDoFinger = true;
		mUpButton.mVisible = true;
		AddWidget(mUpButton);
		mUpButton.SetVisible(isVisible: false);
		mUpButton.SetDisabled(isDisabled: true);
		mDownButton = new ButtonWidget(6, this);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_UI_LEADERBOARDS_ARROW_LIGHTF);
		mDownButton.mButtonImage = imageByID2;
		mDownButton.mDownImage = Res.GetImageByID(ResID.IMAGE_UI_LEADERBOARDS_ARROW_LIGHTF_ON);
		float num3 = 0f;
		float num4 = 0f;
		mDownButton.Resize(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_LEADERBOARDS_ARROW_LIGHTF)) - GameApp.gApp.mWideScreenXOffset + mAspectOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_LEADERBOARDS_ARROW_LIGHTF)), imageByID2.GetWidth(), imageByID2.GetHeight());
		mDownButton.mNormalRect = new Rect(0, 0, imageByID2.GetWidth(), imageByID2.GetHeight());
		mDownButton.mDownRect = new Rect((int)num3, (int)num4, imageByID2.GetWidth() - (int)num3, imageByID2.GetHeight() - (int)num4);
		mDownButton.mDoFinger = true;
		mDownButton.mVisible = true;
		mDownButton.SetDisabled(isDisabled: true);
		AddWidget(mDownButton);
		mAchievementsScrollWidget.SetPageVertical(1, animated: false);
		mDownButton.SetVisible(isVisible: true);
		mUpButton.SetVisible(isVisible: true);
		mCurrentPage = 0;
		mEnterScreneLoad = false;
	}

	public override void Update()
	{
		if (GameApp.gApp.mBambooTransition != null && GameApp.gApp.mBambooTransition.IsInProgress())
		{
			return;
		}
		if (!mEnterScreneLoad && GameApp.gApp.mBambooTransition != null && !GameApp.gApp.mBambooTransition.IsInProgress())
		{
			mEnterScreneLoad = true;
			StartLoading();
		}
		if (mAchievementsScrollWidget != null)
		{
			if (mAchievementsScrollWidget.GetPageVertical() == 0)
			{
				mUpButton.SetVisible(isVisible: false);
			}
			else
			{
				mUpButton.SetVisible(isVisible: true);
			}
			if (mAchievementsScrollWidget.GetPageVertical() == 4)
			{
				mDownButton.SetVisible(isVisible: false);
			}
			else
			{
				mDownButton.SetVisible(isVisible: true);
			}
		}
		if (GameApp.gApp.mBambooTransition != null && GameApp.gApp.mBambooTransition.IsInProgress())
		{
			mAchievementsScrollWidget.SetVisible(isVisible: false);
		}
		else
		{
			mAchievementsScrollWidget.SetVisible(isVisible: true);
		}
		if (!mLoadingDataComplete)
		{
			ulong num = SexyFramework.Common.SexyTime();
			if (num - mTicker > 500)
			{
				if (loadingDot.Length < 6)
				{
					loadingDot += ".";
				}
				else
				{
					loadingDot = "";
				}
				mTicker = num;
			}
		}
		if (!GameApp.gApp.mBambooTransition.IsInProgress() && mNeedsInitScroll)
		{
			mAchievementsScrollWidget.SetPageVertical(0, animated: true);
			mNeedsInitScroll = false;
		}
	}

	public float GetTitleXOffset()
	{
		return mTitleXOffset;
	}

	public override void Draw(Graphics g)
	{
		Graphics3D graphics3D = g?.Get3D();
		g.Translate(mX / 2, 0);
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE, -GameApp.gApp.mWideScreenXOffset, 0, IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE.GetWidth() + 21, IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE.GetHeight());
		g.DrawImageMirror(IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE, -GameApp.gApp.mWideScreenXOffset + IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE.GetWidth() + 21, 0, IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE.GetWidth() + 21, IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE.GetHeight());
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_BG_FLOOR, 0, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_BG_FLOOR)), GameApp.gApp.GetScreenWidth(), IMAGE_UI_CHALLENGESCREEN_BG_FLOOR.GetHeight());
		g.SetFont(Res.GetFontByID(ResID.FONT_SHAGLOUNGE28_STROKE));
		int num = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END));
		int num2 = GameApp.gApp.GetScreenWidth() - num - IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END.GetWidth() + GameApp.gApp.mWideScreenXOffset;
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_UP, num + IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END.GetWidth(), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_UP)), num2 - num - IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END.GetWidth(), IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_UP.GetHeight());
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END, num, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END)));
		g.DrawImageMirror(IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END, num2, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END)));
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES)) - GameApp.gApp.mWideScreenXOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES)), IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetWidth() + 30, IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetHeight());
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_WOOD, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_WOOD)) - GameApp.gApp.mWideScreenXOffset + mAspectOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_WOOD)));
		g.DrawImage(IMAGE_UI_LEADERBOARDS_SHADOW, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_LEADERBOARDS_SHADOW)) - GameApp.gApp.mWideScreenXOffset + mAspectOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_LEADERBOARDS_SHADOW)));
		int totalNum = 0;
		int unlockedNum = 0;
		int totalGPoint = 0;
		int unlockedGPoint = 0;
		int num3 = 195;
		GameApp.gApp.mUserProfile.m_AchievementMgr.getAchievementsInfo(ref unlockedNum, ref totalNum, ref unlockedGPoint, ref totalGPoint);
		int num4 = 270;
		Common._DS(Common._M(200));
		int num5 = Common._DS(Common._M(0));
		string theString = unlockedNum + " / " + totalNum + " " + TextManager.getInstance().getString(93);
		int theX = num4;
		int theY = num5 + num3;
		g.SetColor(255, 255, 255, 255);
		g.SetFont(Res.GetFontByID(ResID.FONT_SHAGEXOTICA38_BLACK));
		g.DrawString(theString, theX, theY);
		theString = " G : " + unlockedGPoint + " / " + totalGPoint;
		int num6 = 810;
		theX = num6 - mTitleFont.StringWidth(theString);
		theY = num5 + num3;
		num5 += 32;
		g.DrawString(theString, theX, theY);
		graphics3D.SetMasking(Graphics3D.EMaskMode.MASKMODE_WRITE_MASKONLY);
		g.FillRect(260, 160, 588, 40);
		graphics3D.SetMasking(Graphics3D.EMaskMode.MASKMODE_NONE);
		g.Translate(-mX / 2, 0);
		DeferOverlay(9);
	}

	public override void DrawOverlay(Graphics g)
	{
		g.Translate(mX / 2, 0);
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_BG_SIDE, -GameApp.gApp.mWideScreenXOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_BG_SIDE)));
		g.DrawImageMirror(IMAGE_UI_CHALLENGESCREEN_BG_SIDE, GameApp.gApp.GetScreenWidth() + GameApp.gApp.mWideScreenXOffset - IMAGE_UI_CHALLENGESCREEN_BG_SIDE.GetWidth(), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_BG_SIDE)));
		g.DrawImageMirror(IMAGE_GUI_Achievements_PEDESTAL, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1)) - GameApp.gApp.mWideScreenXOffset - Common._DS(30), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1)) + IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1.GetHeight() + Common._DS(15));
		g.DrawImage(IMAGE_GUI_Achievements_PEDESTAL, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2)) - GameApp.gApp.mWideScreenXOffset - Common._DS(20) + mAspectOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2)) + IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2.GetHeight() - Common._DS(15));
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1)) - GameApp.gApp.mWideScreenXOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1)) + Common._DS(120));
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2)) - GameApp.gApp.mWideScreenXOffset + mAspectOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2)) + Common._DS(120));
		g.SetColor(255, 255, 255, 255);
		g.SetFont(Res.GetFontByID(ResID.FONT_SHAGEXOTICA100_GAUNTLET));
		string theString = TextManager.getInstance().getString(860);
		float num = g.GetFont().StringWidth(theString);
		int num2 = 0;
		if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_RU || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_PL)
		{
			num2 = 15;
		}
		else if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CH || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CHT)
		{
			num2 = 20;
		}
		g.DrawString(theString, (int)mTitleXOffset + Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_WOOD)) - GameApp.gApp.mWideScreenXOffset + (int)(((float)IMAGE_UI_CHALLENGESCREEN_WOOD.GetWidth() - num) / 2f), Common._DS(135) + num2);
		g.DrawImage(IMAGE_UI_LEADERBOARDS_BOSSES, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_LEADERBOARDS_BOSSES)) - GameApp.gApp.mWideScreenXOffset + mAspectOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_LEADERBOARDS_BOSSES)));
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_FRUIT, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_FRUIT)) - GameApp.gApp.mWideScreenXOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_FRUIT)));
		g.DrawImage(IMAGE_UI_LEADERBOARDS_LEAVES2, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_LEADERBOARDS_LEAVES2)) - GameApp.gApp.mWideScreenXOffset + GameApp.gApp.GetScreenRect().mX / 2 + mAspectOffset + 10, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_LEADERBOARDS_LEAVES2)));
		g.Translate(-mX / 2, 0);
		if (!mLoadingDataComplete)
		{
			g.PushState();
			g.Translate(-mX, -mY);
			g.SetColor(0, 0, 0, 130);
			g.FillRect(Common._S(-80), 0, GameApp.gApp.mWidth + Common._S(160), GameApp.gApp.mHeight);
			g.PopState();
			g.SetFont(mLoadingFont);
			g.DrawString(TextManager.getInstance().getString(581) + loadingDot, GameApp.gApp.GetScreenWidth() / 2 - 100, GameApp.gApp.mHeight / 2);
		}
	}

	public bool ProcessHardwareBackButton()
	{
		GameApp.gApp.OnHardwareBackButtonPressProcessed();
		if (GameApp.gApp.mBambooTransition != null && GameApp.gApp.mBambooTransition.IsInProgress())
		{
			return true;
		}
		Dialog dialog = GameApp.gApp.GetDialog(0);
		if (dialog != null)
		{
			GameApp.gApp.DialogButtonDepress(0, 0);
			return false;
		}
		GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.mMainMenu.HideAchievements;
		GameApp.gApp.ToggleBambooTransition();
		return true;
	}

	public void ButtonDepress(int id)
	{
		if (GameApp.gApp.mBambooTransition == null || !GameApp.gApp.mBambooTransition.IsInProgress())
		{
			if (mHomeButton != null && mHomeButton.mId == id)
			{
				GameApp.gApp.ToggleBambooTransition();
				GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.mMainMenu.HideAchievements;
			}
			else if (mUpButton != null && mUpButton.mId == id)
			{
				mAchievementsScrollWidget.PreviousVertPage();
			}
			else if (mDownButton != null && mDownButton.mId == id)
			{
				mAchievementsScrollWidget.NextVertPage();
			}
		}
	}

	public void ButtonPress(int id)
	{
		if (GameApp.gApp.mBambooTransition == null || !GameApp.gApp.mBambooTransition.IsInProgress())
		{
			GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BUTTON2));
		}
	}

	public void ButtonPress(int theId, int theClickCount)
	{
		if (GameApp.gApp.mBambooTransition == null || !GameApp.gApp.mBambooTransition.IsInProgress())
		{
			GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BUTTON2));
		}
	}

	public void ButtonMouseEnter(int id)
	{
	}

	public void ButtonDownTick(int theId)
	{
	}

	public void ButtonMouseLeave(int theId)
	{
	}

	public void ButtonMouseMove(int theId, int theX, int theY)
	{
	}

	public new virtual void TouchEnded(SexyAppBase.Touch touch)
	{
		int x = touch.location.mX;
		int y = touch.location.mY;
		MouseUp(x, y, 1);
	}
}
