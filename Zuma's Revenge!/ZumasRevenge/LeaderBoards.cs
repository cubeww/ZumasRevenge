using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class LeaderBoards : Widget, ButtonListener
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

	protected LeaderBoardsPages mLeaderBoardsPages;

	protected PageControl mLeaderBoardsPageControl;

	protected ScrollWidget mLeaderBoardsScrollWidget;

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

	protected Image IMAGE_GUI_LeaderBoards_PEDESTAL = Res.GetImageByID(ResID.IMAGE_GUI_TIKITEMPLE_PEDESTAL);

	protected Image IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1 = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1);

	protected Image IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2 = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2);

	protected Image IMAGE_UI_LEADERBOARDS_BOSSES = Res.GetImageByID(ResID.IMAGE_UI_LEADERBOARDS_BOSSES);

	protected Image IMAGE_UI_CHALLENGESCREEN_FRUIT = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_FRUIT);

	protected Image IMAGE_UI_LEADERBOARDS_SHADOW = Res.GetImageByID(ResID.IMAGE_UI_LEADERBOARDS_SHADOW);

	public float mXOff;

	protected int mCurrentPage;

	protected int mDataPage;

	private Thread mLoadDataThread;

	private ThreadStart mLoadingProc;

	private bool mLoadingData;

	private bool mLoadingDataComplete;

	private Font mLoadingFont = Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_BASE);

	private string loadingDot = "";

	private ulong mTicker = SexyFramework.Common.SexyTime();

	private LeaderboardReader mLeaderboardReader;

	private bool mCanPageUp;

	private bool mCanPageDown;

	private bool mEnterScreneLoad;

	private bool mPageUp;

	private bool mPageDown;

	private int mLoadPage;

	private Font mStatsFont = Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_STROKE);

	private string mFrogStr;

	private string mScoreStr;

	public LeaderBoards()
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

	public void Init()
	{
		mLeaderBoardsPages = new LeaderBoardsPages(this);
		mLeaderBoardsScrollWidget = new ScrollWidget();
		mLeaderBoardsScrollWidget.Resize(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_LEADERBOARDS_SHADOW)) - GameApp.gApp.mWideScreenXOffset + Common._DS(10), 20 + Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_LEADERBOARDS_SHADOW)), IMAGE_UI_LEADERBOARDS_SHADOW.GetWidth() + 30, IMAGE_UI_LEADERBOARDS_SHADOW.GetHeight() - 40);
		mLeaderBoardsScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_VERTICAL);
		mLeaderBoardsScrollWidget.EnableBounce(enable: true);
		mLeaderBoardsScrollWidget.EnablePaging(enable: true);
		mLeaderBoardsScrollWidget.mLoadPage = PageLoading;
		mLeaderBoardsPageControl = new PageControl(IMAGE_UI_CHALLENGE_PAGE_INDICATOR);
		IMAGE_UI_CHALLENGE_PAGE_INDICATOR.GetCelWidth();
		mLeaderBoardsPages.NumPages();
		mLeaderBoardsPageControl.SetNumberOfPages(mLeaderBoardsPages.NumPages());
		mLeaderBoardsPageControl.Move((int)mTitleXOffset + (mWidth - mLeaderBoardsPageControl.mWidth) / 2, Common._DS(145));
		mLeaderBoardsPageControl.SetCurrentPage(0);
		AddWidget(mLeaderBoardsPageControl);
		mLeaderBoardsScrollWidget.SetPageControl(mLeaderBoardsPageControl);
		AddWidget(mLeaderBoardsScrollWidget);
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
		mLeaderBoardsScrollWidget.SetPageVertical(1, animated: false);
		mDownButton.SetVisible(isVisible: true);
		mUpButton.SetVisible(isVisible: true);
		mCurrentPage = 0;
		mDataPage = 0;
		mEnterScreneLoad = false;
		mFrogStr = TextManager.getInstance().getString(57);
		mScoreStr = TextManager.getInstance().getString(669);
		mScoreStr = mScoreStr.Substring(0, mScoreStr.Length - 1);
	}

	public void PageLoading(int ranking)
	{
		bool flag = false;
		if (ranking != 0)
		{
			if (!mCanPageDown)
			{
				mPageDown = false;
				mLeaderBoardsScrollWidget.SetPage(0, 1, animated: true);
				return;
			}
			mCurrentPage++;
			mDataPage++;
			mPageDown = true;
			flag = true;
		}
		else
		{
			if (!mCanPageUp)
			{
				mPageUp = false;
				mLeaderBoardsScrollWidget.SetPage(0, 1, animated: true);
				return;
			}
			mCurrentPage--;
			mDataPage--;
			mPageUp = true;
			flag = true;
		}
		if (flag)
		{
			mLoadPage = 1;
		}
		mLeaderBoardsScrollWidget.SetPageVertical(ranking, animated: true);
	}

	public void StartLoading(int ranking)
	{
		mCurrentPage = ranking;
		mLoadingProc = LoadingRank;
		mLoadDataThread = new Thread(mLoadingProc);
		mLoadingData = true;
		mLoadingDataComplete = false;
		mLeaderBoardsScrollWidget.SetVisible(isVisible: false);
		mLoadDataThread.Start();
	}

	private void LoadingRank()
	{
		if (GameApp.USE_XBOX_SERVICE && !GameApp.USE_TRIAL_VERSION)
		{
			readLeaderboard();
			mLeaderBoardsScrollWidget.SetDisabled(isDisabled: true);
		}
		else
		{
			mLoadingDataComplete = true;
			mLeaderBoardsScrollWidget.SetVisible(isVisible: true);
		}
	}

	public void LoadOfflineRanking()
	{
		ulong num = SexyFramework.Common.SexyTime();
		ulong num2;
		do
		{
			num2 = SexyFramework.Common.SexyTime();
		}
		while (num2 - num <= 1000);
		if (!mLeaderBoardsScrollWidget.HasWidget(mLeaderBoardsPages))
		{
			mLeaderBoardsPages.AddPage(mCurrentPage, isUpdate: false);
			mLeaderBoardsPages.Resize(0, 0, mLeaderBoardsPages.IMAGE_UI_LEADERBOARDS_SHADOW.GetWidth(), (mLeaderBoardsPages.IMAGE_UI_LEADERBOARDS_SHADOW.GetHeight() + 30) * mLeaderBoardsPages.mNumPages * 3);
			mLeaderBoardsScrollWidget.AddWidget(mLeaderBoardsPages);
		}
		else
		{
			mLeaderBoardsPages.AddPage(mCurrentPage, isUpdate: true);
		}
		mLoadingDataComplete = true;
		mLeaderBoardsScrollWidget.SetVisible(isVisible: true);
		mLeaderBoardsScrollWidget.SetPageVertical(1, animated: false);
		mCurrentPage = 1;
	}

	public void readLeaderboard()
	{
		try
		{
			SignedInGamer asyncState = Gamer.SignedInGamers[PlayerIndex.One];
			LeaderboardIdentity leaderboardId = LeaderboardIdentity.Create(LeaderboardKey.BestScoreLifeTime, 0);
			if (mCurrentPage == 0)
			{
				LeaderboardReader.BeginRead(leaderboardId, 0, 4, LeaderboardReadCallback, asyncState);
			}
			else if (mPageUp && mLeaderboardReader.CanPageUp)
			{
				mPageUp = false;
				mLeaderboardReader.BeginPageUp(LeaderboardPageUpCallback, asyncState);
			}
			else if (mPageDown && mLeaderboardReader.CanPageDown)
			{
				mPageDown = false;
				mLeaderboardReader.BeginPageDown(LeaderboardPageDownCallback, asyncState);
			}
		}
		catch (Exception)
		{
			if (GameApp.gApp.mMainMenu != null && GameApp.gApp.mMainMenu.mState == MainMenu_State.State_LeaderBoards)
			{
				GameApp.gApp.DoGenericDialog("", TextManager.getInstance().getString(59), block: true, ReturnMain, Common._DS(100));
			}
		}
	}

	protected void LeaderboardPageDownCallback(IAsyncResult result)
	{
		if (result.AsyncState is SignedInGamer)
		{
			try
			{
				mLeaderboardReader.EndPageDown(result);
				mCanPageUp = mLeaderboardReader.CanPageUp;
				mCanPageDown = mLeaderboardReader.CanPageDown;
				mUpButton.SetVisible(mCanPageUp);
				mDownButton.SetVisible(mCanPageDown);
				if (!mLeaderBoardsScrollWidget.HasWidget(mLeaderBoardsPages))
				{
					mLeaderBoardsPages.AddPage(mCurrentPage, isUpdate: false, mLeaderboardReader);
					mLeaderBoardsPages.Resize(0, 0, mLeaderBoardsPages.IMAGE_UI_LEADERBOARDS_SHADOW.GetWidth(), (mLeaderBoardsPages.IMAGE_UI_LEADERBOARDS_SHADOW.GetHeight() + 30) * mLeaderBoardsPages.mNumPages * 3);
					mLeaderBoardsScrollWidget.AddWidget(mLeaderBoardsPages);
				}
				else
				{
					mLeaderBoardsPages.AddPage(mCurrentPage, isUpdate: true, mLeaderboardReader);
				}
			}
			catch (Exception)
			{
				if (GameApp.gApp.mMainMenu != null && GameApp.gApp.mMainMenu.mState == MainMenu_State.State_LeaderBoards)
				{
					GameApp.gApp.DoGenericDialog("", TextManager.getInstance().getString(59), block: true, ReturnMain, Common._DS(100));
				}
			}
		}
		mLoadingDataComplete = true;
		mLeaderBoardsScrollWidget.SetDisabled(isDisabled: false);
		mLeaderBoardsScrollWidget.SetVisible(isVisible: true);
		mLeaderBoardsScrollWidget.SetPageVertical(1, animated: false);
	}

	protected void LeaderboardPageUpCallback(IAsyncResult result)
	{
		if (result.AsyncState is SignedInGamer)
		{
			try
			{
				mLeaderboardReader.EndPageUp(result);
				mCanPageUp = mLeaderboardReader.CanPageUp;
				mCanPageDown = mLeaderboardReader.CanPageDown;
				mUpButton.SetVisible(mCanPageUp);
				mDownButton.SetVisible(mCanPageDown);
				if (!mLeaderBoardsScrollWidget.HasWidget(mLeaderBoardsPages))
				{
					mLeaderBoardsPages.AddPage(mCurrentPage, isUpdate: false, mLeaderboardReader);
					mLeaderBoardsPages.Resize(0, 0, mLeaderBoardsPages.IMAGE_UI_LEADERBOARDS_SHADOW.GetWidth(), (mLeaderBoardsPages.IMAGE_UI_LEADERBOARDS_SHADOW.GetHeight() + 30) * mLeaderBoardsPages.mNumPages * 3);
					mLeaderBoardsScrollWidget.AddWidget(mLeaderBoardsPages);
				}
				else
				{
					mLeaderBoardsPages.AddPage(mCurrentPage, isUpdate: true, mLeaderboardReader);
				}
			}
			catch (Exception)
			{
				if (GameApp.gApp.mMainMenu != null && GameApp.gApp.mMainMenu.mState == MainMenu_State.State_LeaderBoards)
				{
					GameApp.gApp.DoGenericDialog("", TextManager.getInstance().getString(59), block: true, ReturnMain, Common._DS(100));
				}
			}
		}
		mLoadingDataComplete = true;
		mLeaderBoardsScrollWidget.SetDisabled(isDisabled: false);
		mLeaderBoardsScrollWidget.SetVisible(isVisible: true);
		mLeaderBoardsScrollWidget.SetPageVertical(1, animated: false);
	}

	protected void LeaderboardReadCallback(IAsyncResult result)
	{
		if (result.AsyncState is SignedInGamer)
		{
			try
			{
				mLeaderboardReader = LeaderboardReader.EndRead(result);
				mCanPageUp = mLeaderboardReader.CanPageUp;
				mCanPageDown = mLeaderboardReader.CanPageDown;
				mUpButton.SetVisible(mCanPageUp);
				mDownButton.SetVisible(mCanPageDown);
				if (!mLeaderBoardsScrollWidget.HasWidget(mLeaderBoardsPages))
				{
					mLeaderBoardsPages.AddPage(mCurrentPage, isUpdate: false, mLeaderboardReader);
					mLeaderBoardsPages.Resize(0, 0, mLeaderBoardsPages.IMAGE_UI_LEADERBOARDS_SHADOW.GetWidth(), (mLeaderBoardsPages.IMAGE_UI_LEADERBOARDS_SHADOW.GetHeight() + 30) * mLeaderBoardsPages.mNumPages * 3);
					mLeaderBoardsScrollWidget.AddWidget(mLeaderBoardsPages);
				}
				else
				{
					mLeaderBoardsPages.AddPage(mCurrentPage, isUpdate: true, mLeaderboardReader);
				}
			}
			catch (Exception)
			{
				ShowXboxErrorMessage();
			}
		}
		mLeaderBoardsScrollWidget.SetDisabled(isDisabled: false);
		mLoadingDataComplete = true;
		mLeaderBoardsScrollWidget.SetVisible(isVisible: true);
		mLeaderBoardsScrollWidget.SetPageVertical(1, animated: false);
	}

	private void ShowXboxErrorMessage()
	{
		if (GameApp.gApp.mMainMenu != null && GameApp.gApp.mMainMenu.mState == MainMenu_State.State_LeaderBoards)
		{
			GameApp.gApp.DoGenericDialog("", TextManager.getInstance().getString(59), block: true, ReturnMain, Common._DS(100));
		}
	}

	public override void Update()
	{
		if (mLoadPage > 0 && mLoadPage < 60)
		{
			mLoadPage++;
			if (mLoadPage == 60)
			{
				StartLoading(mCurrentPage);
				mLoadPage = 0;
			}
		}
		if (!mEnterScreneLoad && GameApp.gApp.mBambooTransition != null && !GameApp.gApp.mBambooTransition.IsInProgress())
		{
			mEnterScreneLoad = true;
			StartLoading(0);
		}
		if (GameApp.gApp.mBambooTransition != null && GameApp.gApp.mBambooTransition.IsInProgress())
		{
			mLeaderBoardsScrollWidget.SetVisible(isVisible: false);
		}
		else
		{
			mLeaderBoardsScrollWidget.SetVisible(isVisible: true);
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
			mLeaderBoardsScrollWidget.SetPageVertical(0, animated: true);
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
		int num3 = 410;
		int num4 = 165;
		g.SetFont(mStatsFont);
		g.SetColor(89, 187, 149);
		g.WriteString("#", num3 - 130 + (int)mXOff, num4 + mStatsFont.GetAscent(), 0, 1);
		int num5 = 25;
		g.WriteString(mFrogStr, num3 + (int)mXOff + num5, num4 + mStatsFont.GetAscent(), 0, 1);
		int num6 = mStatsFont.StringWidth(mScoreStr);
		g.WriteString(mScoreStr, 800 - num6, num4 + mStatsFont.GetAscent(), num6, -1);
		graphics3D.SetMasking(Graphics3D.EMaskMode.MASKMODE_WRITE_MASKONLY);
		g.FillRect(250, 160, 588, 40);
		graphics3D.SetMasking(Graphics3D.EMaskMode.MASKMODE_NONE);
		g.Translate(-mX / 2, 0);
		DeferOverlay(9);
	}

	public override void DrawOverlay(Graphics g)
	{
		g.Translate(mX / 2, 0);
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_BG_SIDE, -GameApp.gApp.mWideScreenXOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_BG_SIDE)));
		g.DrawImageMirror(IMAGE_UI_CHALLENGESCREEN_BG_SIDE, GameApp.gApp.GetScreenWidth() + GameApp.gApp.mWideScreenXOffset - IMAGE_UI_CHALLENGESCREEN_BG_SIDE.GetWidth(), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_BG_SIDE)));
		g.DrawImageMirror(IMAGE_GUI_LeaderBoards_PEDESTAL, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1)) - GameApp.gApp.mWideScreenXOffset - Common._DS(30), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1)) + IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1.GetHeight() + Common._DS(15));
		g.DrawImage(IMAGE_GUI_LeaderBoards_PEDESTAL, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2)) - GameApp.gApp.mWideScreenXOffset - Common._DS(20) + mAspectOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2)) + IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2.GetHeight() - Common._DS(15));
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1)) - GameApp.gApp.mWideScreenXOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1)) + Common._DS(120));
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2)) - GameApp.gApp.mWideScreenXOffset + mAspectOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2)) + Common._DS(120));
		g.SetColor(255, 255, 255, 255);
		g.SetFont(Res.GetFontByID(ResID.FONT_SHAGEXOTICA100_GAUNTLET));
		string theString = TextManager.getInstance().getString(859);
		float num = g.GetFont().StringWidth(theString);
		int num2 = 0;
		int num3 = 0;
		if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_RU || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_PL)
		{
			num2 = 15;
			num3 = 15;
		}
		else if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CH || Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CHT)
		{
			num2 = 20;
		}
		g.DrawString(theString, num3 + (int)mTitleXOffset + Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_WOOD)) - GameApp.gApp.mWideScreenXOffset + (int)(((float)IMAGE_UI_CHALLENGESCREEN_WOOD.GetWidth() - num) / 2f), Common._DS(135) + num2);
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
		Dialog dialog = GameApp.gApp.GetDialog(0);
		if (dialog != null)
		{
			GameApp.gApp.DialogButtonDepress(0, 0);
			return false;
		}
		GameApp.gApp.ToggleBambooTransition();
		GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.mMainMenu.HideLeaderBoards;
		return true;
	}

	public void ReturnMain()
	{
		GameApp.gApp.ToggleBambooTransition();
		GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.mMainMenu.HideLeaderBoards;
	}

	public void ButtonDepress(int id)
	{
		if ((GameApp.gApp.mBambooTransition != null && GameApp.gApp.mBambooTransition.IsInProgress()) || !mLoadingDataComplete)
		{
			return;
		}
		if (mHomeButton != null && mHomeButton.mId == id)
		{
			GameApp.gApp.ToggleBambooTransition();
			GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.mMainMenu.HideLeaderBoards;
		}
		else if (mUpButton != null && mUpButton.mId == id)
		{
			if (mCanPageUp)
			{
				mCurrentPage--;
				mPageUp = true;
				StartLoading(mCurrentPage);
			}
		}
		else if (mDownButton != null && mDownButton.mId == id && mCanPageDown)
		{
			mCurrentPage++;
			mPageDown = true;
			StartLoading(mCurrentPage);
		}
	}

	public void ButtonPress(int id)
	{
		if ((GameApp.gApp.mBambooTransition == null || !GameApp.gApp.mBambooTransition.IsInProgress()) && mLoadingDataComplete)
		{
			GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BUTTON2));
		}
	}

	public void ButtonPress(int theId, int theClickCount)
	{
		if ((GameApp.gApp.mBambooTransition == null || !GameApp.gApp.mBambooTransition.IsInProgress()) && mLoadingDataComplete)
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

	public override void TouchBegan(SexyAppBase.Touch theTouch)
	{
		if (mLoadingDataComplete && mDataPage == 0)
		{
			base.TouchMoved(theTouch);
		}
	}
}
