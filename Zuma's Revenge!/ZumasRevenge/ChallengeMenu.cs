using System.Collections.Generic;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class ChallengeMenu : Widget, ButtonListener, PopAnimListener
{
	public enum Zoom
	{
		Zooming_Crown,
		Zooming_AceCrown
	}

	protected enum EChallengeState
	{
		State_Challenge,
		State_LevelInfo
	}

	public class DefaultStringContainer
	{
		public string mDefaultStr;

		public DefaultStringContainer()
		{
			mDefaultStr = NonIfLocked();
		}

		public string NonIfLocked()
		{
			return TextManager.getInstance().getString(427);
		}

		public string IfLocked()
		{
			return TextManager.getInstance().getString(428);
		}

		public string CanPlayZone()
		{
			return TextManager.getInstance().getString(429);
		}

		public string ZoneUnlocked()
		{
			return TextManager.getInstance().getString(430);
		}

		public string NothingSelected()
		{
			return TextManager.getInstance().getString(431);
		}
	}

	protected EChallengeState mChallengeState;

	public GameApp mApp;

	public PIEffect mAceFX;

	public PopAnim mTrophyFlare;

	public Rect mCSOverRect;

	public Image mTrophy;

	public Image mRegularTrophy;

	public float mTrophyY;

	public bool mDoBounceTrophy;

	public float mTrophyVY;

	public int mTrophyBounceCount;

	public int mTrophyBounceDelay;

	public bool mCrossFadeTrophies;

	public bool mIsAceTrophy;

	public bool mShowFullAceFX;

	public int mCurrentChallengeZone;

	public float mCrownSize;

	public float mCrownAlpha;

	public int mCrownZoomType;

	public int mCrownZoomDelay;

	public int mCSVisFrame;

	public bool mLoopTrophyFlare;

	public float mXFadeAlpha;

	public int mTimer;

	public int mSelectedLevel;

	public List<ButtonWidget> mButtons;

	public MainMenu mMainMenu;

	public float mAceTrophyAlpha;

	public bool mFadeInAceTrophy;

	public int mSlideDir;

	public ChallengeMenuScrollContainer mChallengePages;

	public PageControl mChallengeScrollPageControl;

	public ScrollWidget mChallengeScrollWidget;

	public ButtonWidget mHomeButton;

	public ChallengeLevelInfo mChallengeLevelInfoWidget;

	public float mTitleXOffset;

	public bool mFromMainMenu;

	public bool mCueMainSong;

	public DefaultStringContainer mDefaultStringContainer;

	private Image IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE;

	private Image IMAGE_UI_CHALLENGESCREEN_BG_FLOOR;

	private Image IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES;

	private Image IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END;

	private Image IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_UP;

	private Image IMAGE_UI_CHALLENGESCREEN_WOOD;

	private Image IMAGE_UI_CHALLENGESCREEN_BG_SIDE;

	private Image IMAGE_GUI_TIKITEMPLE_PEDESTAL;

	private Image IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1;

	private Image IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2;

	private Image IMAGE_UI_CHALLENGESCREEN_DRUMS;

	private Image IMAGE_UI_CHALLENGESCREEN_FRUIT;

	private Image IMAGE_UI_LEADERBOARDS_LEAVES2;

	private Image IMAGE_UI_CHALLENGESCREEN_HOME_BACKING;

	private Image IMAGE_UI_CHALLENGESCREEN_HOME_SELECT;

	private Image IMAGE_UI_CHALLENGESCREEN_HOME;

	private Image IMAGE_UI_CHALLENGE_PAGE_INDICATOR;

	public ChallengeMenu(GameApp theApp, MainMenu theMainMenu, bool fromMainMenu)
	{
		mApp = theApp;
		mMainMenu = theMainMenu;
		mCurrentChallengeZone = 0;
		mCrownSize = 1f;
		mCrownAlpha = 255f;
		mCrownZoomType = -1;
		mCrownZoomDelay = 0;
		mTrophy = null;
		mTrophyY = 0f;
		mDoBounceTrophy = false;
		mTrophyVY = 0f;
		mTrophyBounceCount = 0;
		mAceFX = null;
		mRegularTrophy = null;
		mIsAceTrophy = false;
		mCrossFadeTrophies = false;
		mAceTrophyAlpha = 0f;
		mFadeInAceTrophy = false;
		mTrophyBounceDelay = 0;
		mTrophyFlare = null;
		mShowFullAceFX = false;
		mCSVisFrame = 0;
		mLoopTrophyFlare = false;
		mSlideDir = 0;
		mXFadeAlpha = 255f;
		mTimer = 0;
		mSelectedLevel = -1;
		mButtons = new List<ButtonWidget>();
		mDefaultStringContainer = new DefaultStringContainer();
		mHomeButton = null;
		mChallengeLevelInfoWidget = null;
		mChallengeScrollWidget = null;
		if (GameApp.mGameRes == 768)
		{
			mTitleXOffset = 30f;
		}
		else
		{
			mTitleXOffset = 0f;
		}
		mFromMainMenu = fromMainMenu;
		mCueMainSong = false;
		IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE);
		IMAGE_UI_CHALLENGESCREEN_BG_FLOOR = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_BG_FLOOR);
		IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES);
		IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END);
		IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_UP = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_UP);
		IMAGE_UI_CHALLENGESCREEN_WOOD = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_WOOD);
		IMAGE_UI_CHALLENGESCREEN_BG_SIDE = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_BG_SIDE);
		IMAGE_GUI_TIKITEMPLE_PEDESTAL = Res.GetImageByID(ResID.IMAGE_GUI_TIKITEMPLE_PEDESTAL);
		IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1 = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1);
		IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2 = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2);
		IMAGE_UI_CHALLENGESCREEN_DRUMS = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_DRUMS);
		IMAGE_UI_CHALLENGESCREEN_FRUIT = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_FRUIT);
		IMAGE_UI_LEADERBOARDS_LEAVES2 = Res.GetImageByID(ResID.IMAGE_UI_LEADERBOARDS_LEAVES2);
		IMAGE_UI_CHALLENGESCREEN_HOME_BACKING = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_HOME_BACKING);
		IMAGE_UI_CHALLENGESCREEN_HOME_SELECT = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_HOME_SELECT);
		IMAGE_UI_CHALLENGESCREEN_HOME = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_HOME);
		IMAGE_UI_CHALLENGE_PAGE_INDICATOR = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGE_PAGE_INDICATOR);
	}

	public override void Dispose()
	{
		RemoveAllWidgets(doDelete: true, recursive: true);
		mChallengePages = null;
		mChallengeScrollWidget = null;
		mHomeButton = null;
		mChallengeLevelInfoWidget = null;
		mChallengeScrollPageControl = null;
		for (int i = 0; i < GlobalChallenge.NUM_CHALLENGE_ZONES; i++)
		{
			GameApp.gApp.DeleteZoneThumbnails(i);
		}
	}

	public override void Draw(Graphics g)
	{
		g.Translate(-GameApp.gApp.GetScreenRect().mX / 2, 0);
		int gScreenShake = GlobalChallenge.gScreenShake;
		int num = 0;
		if (GameApp.gLastZone != -1)
		{
			num = ((GameApp.gLastZone == 7 && mApp.mUserProfile.mChallengeUnlockState[GameApp.gLastZone - 1, 0] == 0) ? Common._DS(Common._M(635)) : Common._DS(Common._M1(608))) + gScreenShake;
		}
		int num2 = Common._DS(Common._M(500));
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE, -GameApp.gApp.mWideScreenXOffset, 0);
		g.DrawImageMirror(IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE, -GameApp.gApp.mWideScreenXOffset + IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE.GetWidth() + 21, 0, IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE.GetWidth() + 21, IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE.GetHeight());
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_BG_FLOOR, 0, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_BG_FLOOR)), GameApp.gApp.GetScreenWidth(), IMAGE_UI_CHALLENGESCREEN_BG_FLOOR.GetHeight());
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES)) - GameApp.gApp.mWideScreenXOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES)));
		int num3 = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END));
		int num4 = GameApp.gApp.GetScreenRect().mWidth - GameApp.gApp.GetScreenRect().mX - num3;
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_UP, num3 + IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END.GetWidth(), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_UP)), num4 - (num3 + IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END.GetWidth()), IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_UP.GetHeight());
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END, num3, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END)));
		g.DrawImageMirror(IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END, num4, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END)));
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_WOOD, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_WOOD)) - GameApp.gApp.mWideScreenXOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_WOOD)));
		if (mTrophy != null)
		{
			int num5 = (mApp.mHiRes ? 2 : 0);
			int[] array = new int[7]
			{
				Common._M(194),
				Common._M1(184) + num5,
				Common._M2(188),
				Common._M3(194),
				Common._M4(188),
				Common._M5(194),
				Common._M6(194)
			};
			Point[] array2 = new Point[7]
			{
				new Point(Common._M(-250), Common._M1(-830)),
				new Point(Common._M2(-246), Common._M3(-850)),
				new Point(Common._M4(-246), Common._M5(-850)),
				new Point(Common._M6(-252), Common._M7(-876)),
				new Point(Common._M(-246), Common._M1(-850)),
				new Point(Common._M2(-252), Common._M3(-860)),
				new Point(Common._M4(-248), Common._M5(-838))
			};
			g.DrawImage(mTrophy, num + Common._DS(array[GameApp.gLastZone - 1]) - mTrophy.mWidth / 2, (int)(mTrophyY + (float)GlobalChallenge.gScreenShake));
			if (g.Is3D() && mTrophyFlare != null && !mDoBounceTrophy)
			{
				Transform transform = new Transform();
				transform.Translate(num + Common._DS(array[GameApp.gLastZone - 1] + array2[GameApp.gLastZone - 1].mX), num2 + Common._DS(array2[GameApp.gLastZone - 1].mY));
				mTrophyFlare.SetTransform(transform.GetMatrix());
				mTrophyFlare.Draw(g);
			}
			if (g.Is3D() && mAceFX != null)
			{
				g.PushState();
				g.ClipRect(Common._DS(Common._M(540)) + gScreenShake, Common._DS(Common._M1(40)), Common._DS(Common._M2(530)), Common._DS(Common._M3(1200)));
				if (mShowFullAceFX)
				{
					mAceFX.Draw(g);
				}
				else
				{
					mAceFX.DrawLayer(g, mAceFX.GetLayer("mask"));
				}
				g.PopState();
			}
		}
		DeferOverlay(9);
		g.Translate(GameApp.gApp.GetScreenRect().mX / 2, 0);
	}

	public override void DrawOverlay(Graphics g)
	{
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_BG_SIDE, -GameApp.gApp.mWideScreenXOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_BG_SIDE)));
		g.DrawImageMirror(IMAGE_UI_CHALLENGESCREEN_BG_SIDE, GameApp.gApp.GetScreenWidth() + GameApp.gApp.mWideScreenXOffset - IMAGE_UI_CHALLENGESCREEN_BG_SIDE.GetWidth() + GameApp.gApp.GetScreenRect().mX, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_BG_SIDE)));
		g.DrawImageMirror(IMAGE_GUI_TIKITEMPLE_PEDESTAL, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1)) - GameApp.gApp.mWideScreenXOffset - Common._DS(30), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1)) + IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1.GetHeight() + Common._DS(15));
		g.DrawImage(IMAGE_GUI_TIKITEMPLE_PEDESTAL, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2)) - GameApp.gApp.mWideScreenXOffset - Common._DS(20) + GameApp.gApp.GetScreenRect().mX, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2)) + IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2.GetHeight() - Common._DS(15));
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1)) - GameApp.gApp.mWideScreenXOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1)) + Common._DS(120));
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2)) - GameApp.gApp.mWideScreenXOffset + GameApp.gApp.GetScreenRect().mX, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2)) + Common._DS(120));
		g.SetColor(new Color(255, 255, 255, 255));
		g.SetFont(Res.GetFontByID(ResID.FONT_SHAGEXOTICA100_GAUNTLET));
		string theString = TextManager.getInstance().getString(782);
		int num = g.GetFont().StringWidth(theString);
		g.DrawString(theString, (int)(mTitleXOffset + (float)Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_WOOD)) - (float)GameApp.gApp.mWideScreenXOffset + (float)((IMAGE_UI_CHALLENGESCREEN_WOOD.GetWidth() - num) / 2)), Common._DS(135));
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_DRUMS, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_DRUMS)) - GameApp.gApp.mWideScreenXOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_DRUMS)));
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_FRUIT, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_FRUIT)) - GameApp.gApp.mWideScreenXOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_FRUIT)));
		g.DrawImage(IMAGE_UI_LEADERBOARDS_LEAVES2, 42 + Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_LEADERBOARDS_LEAVES2)) - GameApp.gApp.mWideScreenXOffset + GameApp.gApp.GetScreenRect().mX, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_LEADERBOARDS_LEAVES2)));
		if (mHomeButton != null)
		{
			g.DrawImage(IMAGE_UI_CHALLENGESCREEN_HOME_BACKING, 0, 0);
			if (mHomeButton.IsButtonDown())
			{
				float num2 = (IMAGE_UI_CHALLENGESCREEN_HOME.GetWidth() - IMAGE_UI_CHALLENGESCREEN_HOME_SELECT.GetWidth()) / 2;
				float num3 = (IMAGE_UI_CHALLENGESCREEN_HOME.GetHeight() - IMAGE_UI_CHALLENGESCREEN_HOME_SELECT.GetHeight()) / 2;
				g.DrawImage(IMAGE_UI_CHALLENGESCREEN_HOME_SELECT, (int)((float)mHomeButton.mX + num2 + (float)GameApp.gApp.GetScreenRect().mX), (int)((float)mHomeButton.mY + num3));
			}
			else
			{
				g.DrawImage(IMAGE_UI_CHALLENGESCREEN_HOME, mHomeButton.mX + GameApp.gApp.GetScreenRect().mX, mHomeButton.mY);
			}
		}
	}

	public override void Update()
	{
		Common._M(0);
		if (mTrophyFlare != null && GameApp.gApp.Is3DAccelerated() && !mDoBounceTrophy)
		{
			MarkDirty();
			mTrophyFlare.Update();
		}
		if (GameApp.gApp.mBambooTransition != null && GameApp.gApp.mBambooTransition.IsInProgress())
		{
			mChallengeScrollWidget.SetVisible(isVisible: false);
		}
		else
		{
			mChallengeScrollWidget.SetVisible(isVisible: true);
		}
		if (mFromMainMenu && !GameApp.gApp.mBambooTransition.IsInProgress())
		{
			if (mChallengeScrollWidget == null)
			{
				Init();
			}
			if (mChallengeScrollWidget != null)
			{
				mChallengeScrollWidget.SetPageHorizontal(1, animated: false);
				mChallengeScrollWidget.SetPageHorizontal(0, animated: true);
				mChallengePages.PreloadButtonImage(0);
			}
			mFromMainMenu = false;
		}
		if (mCueMainSong && GameApp.gApp.mBambooTransition != null && !GameApp.gApp.mBambooTransition.IsInProgress())
		{
			mApp.PlaySong(1);
			mCueMainSong = false;
		}
		if (GlobalChallenge.gScreenShakeTimer > 0)
		{
			MarkDirty();
			GlobalChallenge.gScreenShakeTimer--;
			GlobalChallenge.gScreenShake = SexyFramework.Common.Rand(Common._M(10));
			if (GlobalChallenge.gScreenShakeTimer == 0)
			{
				GlobalChallenge.gScreenShake = 0;
			}
		}
		if (mCrownZoomType >= 0 && --mCrownZoomDelay <= 0)
		{
			MarkDirty();
			mTimer++;
			int num = Common._M(75) - mTimer;
			float num2 = 255f / (float)num;
			mCrownAlpha += (int)num2;
			if (mCrownAlpha > 255f)
			{
				mCrownAlpha = 255f;
			}
			num2 = Common._M(15f) / (float)num;
			mCrownSize -= num2;
			if (!(mCrownSize <= 1f))
			{
				return;
			}
			if (mCrownZoomType == 0)
			{
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_MINI_CROWN_IMPACT));
			}
			else
			{
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_ACE_MINI_CROWN_IMPACT));
			}
			GlobalChallenge.gScreenShakeTimer = Common._M(15);
			mCrownSize = 1f;
			mCrownAlpha = 255f;
			if (mApp.mUserProfile.mDoChallengeAceTrophyZoom)
			{
				if (mApp.mUserProfile.mDoChallengeTrophyZoom)
				{
					mCrownZoomType = 1;
					mCrownSize = Common._M(16f);
					mCrownAlpha = 0f;
					mCrownZoomDelay = Common._M(20);
					mTimer = 0;
				}
				else
				{
					mCrownZoomType = -1;
				}
				mApp.mUserProfile.mDoChallengeTrophyZoom = (mApp.mUserProfile.mDoChallengeAceTrophyZoom = false);
			}
			else
			{
				mApp.mUserProfile.mDoChallengeTrophyZoom = false;
				mCrownZoomType = -1;
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_LEVELS_UNLOCKED));
			}
		}
		else if (mCrownZoomType == -1 && --mCrownZoomDelay <= 0 && mDoBounceTrophy)
		{
			MarkDirty();
		}
	}

	public void ShowChallengeLevelInfo(int theZoneNum, int theLevelNum, string theLevelName)
	{
		if (mChallengeLevelInfoWidget != null)
		{
			mChallengeState = EChallengeState.State_LevelInfo;
			mChallengeLevelInfoWidget.SetLevel(theZoneNum, theLevelNum, theLevelName);
			mChallengeLevelInfoWidget.SetVisible(isVisible: true);
			mChallengeLevelInfoWidget.SetDisabled(isDisabled: false);
			SetFocus(mChallengeLevelInfoWidget);
			int theNewX = ((!GameApp.gApp.IsWideScreen()) ? ((int)((float)(GameApp.gApp.GetScreenRect().mWidth - mChallengeLevelInfoWidget.mWidth) * 0.5f)) : ((int)((float)(GameApp.gApp.GetScreenRect().mWidth - GameApp.gApp.GetScreenRect().mX - mChallengeLevelInfoWidget.GetWidth()) * 0.5f)));
			int theNewY = mChallengeLevelInfoWidget.mY;
			mChallengeLevelInfoWidget.Move(theNewX, theNewY);
		}
	}

	public void HideChallengeLevelInfo()
	{
		mChallengeState = EChallengeState.State_Challenge;
		if (mChallengeLevelInfoWidget != null)
		{
			mChallengeLevelInfoWidget.SetLevel(-1, -1, "");
			mChallengeLevelInfoWidget.SetVisible(isVisible: false);
			mChallengeLevelInfoWidget.SetDisabled(isDisabled: true);
			SetFocus(this);
		}
	}

	public override void MouseUp(int x, int y)
	{
	}

	public void Init()
	{
		if (!GameApp.gApp.mResourceManager.IsGroupLoaded("CommonGame") && !GameApp.gApp.mResourceManager.LoadResources("CommonGame"))
		{
			GameApp.gApp.ShowResourceError(doExit: true);
			GameApp.gApp.Shutdown();
		}
		Common._M(0);
		mChallengeState = EChallengeState.State_Challenge;
		mChallengePages = new ChallengeMenuScrollContainer(this);
		mChallengeScrollWidget = new ScrollWidget();
		mChallengeScrollWidget.Resize(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES)) - GameApp.gApp.mWideScreenXOffset - GameApp.gApp.GetScreenRect().mX, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES)), IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetWidth(), IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetHeight());
		mChallengeScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_HORIZONTAL);
		mChallengeScrollWidget.EnableBounce(enable: true);
		mChallengeScrollWidget.EnablePaging(enable: true);
		mChallengeScrollWidget.AddWidget(mChallengePages);
		mChallengeScrollPageControl = new PageControl(IMAGE_UI_CHALLENGE_PAGE_INDICATOR);
		IMAGE_UI_CHALLENGE_PAGE_INDICATOR.GetCelWidth();
		mChallengePages.NumPages();
		mChallengeScrollPageControl.SetNumberOfPages(mChallengePages.NumPages());
		mChallengeScrollPageControl.Move((int)(mTitleXOffset + (float)((mWidth - mChallengeScrollPageControl.mWidth) / 2) - (float)GameApp.gApp.GetScreenRect().mX), Common._DS(145));
		mChallengeScrollPageControl.SetCurrentPage(0);
		AddWidget(mChallengeScrollPageControl);
		mChallengeScrollWidget.SetPageControl(mChallengeScrollPageControl);
		AddWidget(mChallengeScrollWidget);
		InitHomeButton();
		if (mFromMainMenu)
		{
			mChallengeScrollWidget.SetPageHorizontal(mChallengePages.NumPages(), animated: false);
		}
		else
		{
			mChallengeScrollWidget.SetPageHorizontal(0, animated: false);
		}
		mChallengeLevelInfoWidget = new ChallengeLevelInfo(this);
		int theWidth = IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetWidth() + Common._DS(100);
		IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetHeight();
		Common._DS(150);
		mChallengeLevelInfoWidget.Resize(0, 0, theWidth, GameApp.gApp.GetScreenRect().mHeight);
		Common.SetupDialog(mChallengeLevelInfoWidget);
		mChallengeLevelInfoWidget.mPriority = 2147483645;
		mChallengeLevelInfoWidget.SetVisible(isVisible: false);
		mChallengeLevelInfoWidget.SetDisabled(isDisabled: true);
		AddWidget(mChallengeLevelInfoWidget);
	}

	private void InitHomeButton()
	{
		mHomeButton = new ButtonWidget(0, this);
		mHomeButton.mDoFinger = true;
		mHomeButton.mPriority = 2;
		mHomeButton.mButtonImage = IMAGE_UI_CHALLENGESCREEN_HOME;
		mHomeButton.mDownImage = IMAGE_UI_CHALLENGESCREEN_HOME_SELECT;
		float num = (float)(IMAGE_UI_CHALLENGESCREEN_HOME_SELECT.GetWidth() - IMAGE_UI_CHALLENGESCREEN_HOME.GetWidth()) / 2f;
		float num2 = (float)(IMAGE_UI_CHALLENGESCREEN_HOME_SELECT.GetHeight() - IMAGE_UI_CHALLENGESCREEN_HOME.GetHeight()) / 2f;
		mHomeButton.Resize((int)num, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_HOME_SELECT)) + (int)num2, IMAGE_UI_CHALLENGESCREEN_HOME_SELECT.GetWidth(), IMAGE_UI_CHALLENGESCREEN_HOME_SELECT.GetHeight());
		mHomeButton.mNormalRect = new Rect(0, 0, IMAGE_UI_CHALLENGESCREEN_HOME.GetWidth(), IMAGE_UI_CHALLENGESCREEN_HOME.GetHeight());
		mHomeButton.mDownRect = new Rect((int)num, (int)num2, IMAGE_UI_CHALLENGESCREEN_HOME_SELECT.GetWidth() - (int)num, IMAGE_UI_CHALLENGESCREEN_HOME_SELECT.GetHeight() - (int)num2);
		AddWidget(mHomeButton);
	}

	public void RehupChallengeButtons()
	{
		if (mChallengePages != null)
		{
			mChallengePages.RehupChallengeButtons();
		}
	}

	public void InitCS()
	{
		if (mApp.mUserProfile.mAdvModeVars.mHighestLevelBeat >= 10)
		{
			mChallengePages.RehupChallengeButtons();
			mMainMenu.RehupButtons();
			mCrownZoomType = -1;
			if ((mApp.mUserProfile.mDoChallengeTrophyZoom || mApp.mUserProfile.mDoChallengeAceTrophyZoom) && GameApp.gApp.Is3DAccelerated())
			{
				mCrownZoomType = ((!mApp.mUserProfile.mDoChallengeTrophyZoom) ? 1 : 0);
				mCrownSize = Common._M(16f);
				mCrownAlpha = 0f;
				mTimer = 0;
				mCrownZoomDelay = 0;
			}
			bool flag = GameApp.gApp.mUserProfile.mDoChallengeCupComplete || GameApp.gApp.mUserProfile.mDoChallengeAceCupComplete;
			bool flag2 = GameApp.gApp.mUserProfile.mUnlockSparklesIdx1 != -1 || GameApp.gApp.mUserProfile.mUnlockSparklesIdx2 != -1;
			if (flag && !flag2)
			{
				mChallengeScrollWidget.SetPageHorizontal(0, animated: false);
				mChallengeScrollPageControl.SetCurrentPage(0);
				mChallengePages.AwardMedal(GameApp.gLastZone, GameApp.gApp.mUserProfile.mDoChallengeAceCupComplete);
			}
			else
			{
				SetupChallengeZone(GameApp.gLastZone);
			}
			if (mFromMainMenu && mChallengeScrollWidget != null)
			{
				mChallengeScrollWidget.SetPageHorizontal(mChallengePages.NumPages(), animated: false);
			}
		}
	}

	public void StartChallengeGame()
	{
		mApp.StartGauntletMode(mChallengeLevelInfoWidget.GetChallengeLevelName(), mCSOverRect);
	}

	public virtual void ButtonPress(int id)
	{
		ButtonPress(id, 1);
	}

	public virtual void ButtonPress(int id, int cc)
	{
		if (GameApp.gApp.mBambooTransition == null || !GameApp.gApp.mBambooTransition.IsInProgress())
		{
			mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BUTTON2));
		}
	}

	public virtual void ButtonDepress(int id)
	{
		if ((GameApp.gApp.mBambooTransition == null || !GameApp.gApp.mBambooTransition.IsInProgress()) && mSlideDir == 0 && id == 0)
		{
			GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.HideChallengeMenu;
			GameApp.gApp.ToggleBambooTransition();
		}
	}

	public void SetupChallengeZone(int zone)
	{
		if (mAceFX != null)
		{
			mAceFX.mEmitAfterTimeline = false;
		}
		if (mChallengeScrollWidget != null)
		{
			mChallengeScrollWidget.SetPageHorizontal(zone + 1, animated: false);
			mChallengeScrollPageControl.SetCurrentPage(zone + 1);
		}
	}

	public bool ProcessHardwareBackButton()
	{
		GameApp.gApp.OnHardwareBackButtonPressProcessed();
		if (GameApp.gApp.GetDialog(0) != null)
		{
			GameApp.gApp.DialogButtonDepress(0, 0);
			return false;
		}
		if (mChallengeLevelInfoWidget != null && mChallengeLevelInfoWidget.mVisible && !mChallengeLevelInfoWidget.mDisabled)
		{
			HideChallengeLevelInfo();
			return false;
		}
		if (mChallengeScrollWidget.GetPageHorizontal() > 0)
		{
			mChallengeScrollWidget.SetPageHorizontal(0, animated: true);
			return false;
		}
		GameApp.gApp.ToggleBambooTransition();
		GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.mMainMenu.HideChallengeMenu;
		return true;
	}

	public bool IsReceivingAward()
	{
		bool flag = GameApp.gApp.mUserProfile.mUnlockSparklesIdx1 != -1 || GameApp.gApp.mUserProfile.mUnlockSparklesIdx2 != -1;
		bool result = GameApp.gApp.mUserProfile.mDoChallengeCupComplete || GameApp.gApp.mUserProfile.mDoChallengeAceCupComplete;
		if (!flag)
		{
			return result;
		}
		return true;
	}

	public bool HasAcedZone(int theZoneNum)
	{
		if (GameApp.gApp.mUserProfile.mChallengeUnlockState[theZoneNum, 0] == 0)
		{
			return false;
		}
		bool result = true;
		if (GameApp.gApp.mUserProfile.mDoChallengeAceCupComplete && GameApp.gLastZone == theZoneNum)
		{
			result = false;
		}
		else
		{
			for (int i = 0; i < 10; i++)
			{
				int num = GameApp.gApp.mUserProfile.mChallengeUnlockState[theZoneNum, i];
				if (num != 5)
				{
					result = false;
					break;
				}
			}
		}
		return result;
	}

	public bool HasBeatZone(int theZoneNum)
	{
		if (GameApp.gApp.mUserProfile.mChallengeUnlockState[theZoneNum, 0] == 0)
		{
			return false;
		}
		bool result = true;
		if ((GameApp.gApp.mUserProfile.mDoChallengeCupComplete || GameApp.gApp.mUserProfile.mDoChallengeAceCupComplete) && GameApp.gLastZone == theZoneNum)
		{
			result = false;
		}
		else
		{
			for (int i = 0; i < 10; i++)
			{
				int num = GameApp.gApp.mUserProfile.mChallengeUnlockState[theZoneNum, i];
				if (num != 4 && num != 5)
				{
					result = false;
					break;
				}
			}
		}
		return result;
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

	public void PopAnimStopped(int id)
	{
		if (mTrophyFlare != null && id == mTrophyFlare.mId)
		{
			if (mLoopTrophyFlare)
			{
				mTrophyFlare.Play("Main");
			}
			else
			{
				mTrophyFlare = null;
			}
		}
	}

	public void PopAnimPlaySample(string theSampleName, int thePan, double theVolume, double theNumSteps)
	{
	}

	public PIEffect PopAnimLoadParticleEffect(string theEffectName)
	{
		return null;
	}

	public bool PopAnimObjectPredraw(int theId, Graphics g, PASpriteInst theSpriteInst, PAObjectInst theObjectInst, PATransform theTransform, Color theColor)
	{
		return true;
	}

	public bool PopAnimObjectPostdraw(int theId, Graphics g, PASpriteInst theSpriteInst, PAObjectInst theObjectInst, PATransform theTransform, Color theColor)
	{
		return true;
	}

	public ImagePredrawResult PopAnimImagePredraw(int theId, PASpriteInst theSpriteInst, PAObjectInst theObjectInst, PATransform theTransform, Image theImage, Graphics g, int theDrawCount)
	{
		return ImagePredrawResult.ImagePredraw_Normal;
	}

	public void PopAnimCommand(int theId, string theCommand, string theParam)
	{
	}

	public bool PopAnimCommand(int theId, PASpriteInst theSpriteInst, string theCommand, string theParam)
	{
		PopAnimCommand(theId, theCommand, theParam);
		return true;
	}
}
