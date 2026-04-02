using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class TikiTemple : Widget, ButtonListener
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

	protected int mDisplayMode;

	protected int mBounceCount;

	protected TikiTemplePages mTikiTemplePages;

	protected PageControl mTikiTemplePageControl;

	protected ScrollWidget mTikiTempleScrollWidget;

	protected bool mNeedsInitScroll;

	protected float mTitleXOffset;

	protected int mAspectOffset = 30;

	protected Image IMAGE_UI_CHALLENGESCREEN_HOME_SELECT = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_HOME_SELECT);

	protected Image IMAGE_UI_CHALLENGESCREEN_HOME = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_HOME);

	protected Image IMAGE_UI_CHALLENGE_PAGE_INDICATOR = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGE_PAGE_INDICATOR);

	protected Image IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_CEILING_PIECE);

	protected Image IMAGE_UI_CHALLENGESCREEN_BG_FLOOR = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_BG_FLOOR);

	protected Image IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_END);

	protected Image IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_UP = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_POLE_BROKEN_UP);

	protected Image IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES);

	protected Image IMAGE_UI_CHALLENGESCREEN_WOOD = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_WOOD);

	protected Image IMAGE_UI_LEADERBOARDS_LEAVES2 = Res.GetImageByID(ResID.IMAGE_UI_LEADERBOARDS_LEAVES2);

	protected Image IMAGE_UI_CHALLENGESCREEN_BG_SIDE = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_BG_SIDE);

	protected Image IMAGE_GUI_TIKITEMPLE_PEDESTAL = Res.GetImageByID(ResID.IMAGE_GUI_TIKITEMPLE_PEDESTAL);

	protected Image IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1 = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1);

	protected Image IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2 = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2);

	protected Image IMAGE_UI_CHALLENGESCREEN_DRUMS = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_DRUMS);

	protected Image IMAGE_UI_CHALLENGESCREEN_FRUIT = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_FRUIT);

	protected Image IMAGE_UI_CHALLENGESCREEN_HOME_BACKING = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_HOME_BACKING);

	public float mXOff;

	public TikiTemple()
	{
		if (!GameApp.gApp.mResourceManager.IsGroupLoaded("CommonGame") && !GameApp.gApp.mResourceManager.LoadResources("CommonGame"))
		{
			GameApp.gApp.Shutdown();
		}
		mDisplayMode = -1;
		mClip = false;
		mSelectedScreenState = 0;
		mHomeButton = null;
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
		mTikiTemplePages = new TikiTemplePages(this);
		mTikiTempleScrollWidget = new ScrollWidget();
		mTikiTempleScrollWidget.Resize(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES)) - GameApp.gApp.mWideScreenXOffset + Common._DS(30), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES)), IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetWidth() + 30, IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetHeight());
		mTikiTempleScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_HORIZONTAL);
		mTikiTempleScrollWidget.EnableBounce(enable: true);
		mTikiTempleScrollWidget.EnablePaging(enable: true);
		mTikiTempleScrollWidget.AddWidget(mTikiTemplePages);
		mTikiTemplePageControl = new PageControl(IMAGE_UI_CHALLENGE_PAGE_INDICATOR);
		IMAGE_UI_CHALLENGE_PAGE_INDICATOR.GetCelWidth();
		mTikiTemplePages.NumPages();
		mTikiTemplePageControl.SetNumberOfPages(mTikiTemplePages.NumPages());
		mTikiTemplePageControl.Move((int)mTitleXOffset + (mWidth - mTikiTemplePageControl.mWidth) / 2, Common._DS(145));
		mTikiTemplePageControl.SetCurrentPage(0);
		AddWidget(mTikiTemplePageControl);
		mTikiTempleScrollWidget.SetPageControl(mTikiTemplePageControl);
		AddWidget(mTikiTempleScrollWidget);
		InitHomeButton();
		mTikiTempleScrollWidget.SetPageHorizontal(0, animated: false);
	}

	private void InitHomeButton()
	{
		mHomeButton = new ButtonWidget((int)ButtonState.Back_Btn, this);
		mHomeButton.mDoFinger = true;
		mHomeButton.mPriority = 2;
		mHomeButton.mButtonImage = IMAGE_UI_CHALLENGESCREEN_HOME;
		mHomeButton.mDownImage = IMAGE_UI_CHALLENGESCREEN_HOME_SELECT;
		float num = (float)(IMAGE_UI_CHALLENGESCREEN_HOME_SELECT.GetWidth() - IMAGE_UI_CHALLENGESCREEN_HOME.GetWidth()) / 2f;
		float num2 = (float)(IMAGE_UI_CHALLENGESCREEN_HOME_SELECT.GetHeight() - IMAGE_UI_CHALLENGESCREEN_HOME.GetHeight()) / 2f;
		mHomeButton.Resize(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_HOME_SELECT)) + (int)num, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_HOME_SELECT)) + (int)num2, IMAGE_UI_CHALLENGESCREEN_HOME_SELECT.GetWidth(), IMAGE_UI_CHALLENGESCREEN_HOME_SELECT.GetHeight());
		mHomeButton.mNormalRect = new Rect(0, 0, IMAGE_UI_CHALLENGESCREEN_HOME.GetWidth(), IMAGE_UI_CHALLENGESCREEN_HOME.GetHeight());
		mHomeButton.mDownRect = new Rect((int)num, (int)num2, IMAGE_UI_CHALLENGESCREEN_HOME_SELECT.GetWidth() - (int)num, IMAGE_UI_CHALLENGESCREEN_HOME_SELECT.GetHeight() - (int)num2);
		AddWidget(mHomeButton);
	}

	public override void Update()
	{
		if (!GameApp.gApp.mBambooTransition.IsInProgress() && mNeedsInitScroll)
		{
			mTikiTempleScrollWidget.SetPageHorizontal(0, animated: true);
			mNeedsInitScroll = false;
		}
		if (GameApp.gApp.mBambooTransition != null && GameApp.gApp.mBambooTransition.IsInProgress())
		{
			mTikiTempleScrollWidget.SetVisible(isVisible: false);
		}
		else
		{
			mTikiTempleScrollWidget.SetVisible(isVisible: true);
		}
	}

	public float GetTitleXOffset()
	{
		return mTitleXOffset;
	}

	public override void Draw(Graphics g)
	{
		g?.Get3D();
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
		g.Translate(-mX / 2, 0);
		DeferOverlay(9);
	}

	public override void DrawOverlay(Graphics g)
	{
		g.Translate(mX / 2, 0);
		if (mHomeButton != null)
		{
			g.DrawImage(IMAGE_UI_CHALLENGESCREEN_HOME_BACKING, 0, 0);
			if (mHomeButton.IsButtonDown())
			{
				float homeXOff = (IMAGE_UI_CHALLENGESCREEN_HOME.GetWidth() - IMAGE_UI_CHALLENGESCREEN_HOME_SELECT.GetWidth()) / 2;
				float homeYOff = (IMAGE_UI_CHALLENGESCREEN_HOME.GetHeight() - IMAGE_UI_CHALLENGESCREEN_HOME_SELECT.GetHeight()) / 2;
				g.DrawImage(IMAGE_UI_CHALLENGESCREEN_HOME_SELECT, (int)((float)mHomeButton.mX + homeXOff), (int)((float)mHomeButton.mY + homeYOff));
			}
			else
			{
				g.DrawImage(IMAGE_UI_CHALLENGESCREEN_HOME, mHomeButton.mX, mHomeButton.mY);
			}
		}
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_BG_SIDE, -GameApp.gApp.mWideScreenXOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_BG_SIDE)));
		g.DrawImageMirror(IMAGE_UI_CHALLENGESCREEN_BG_SIDE, GameApp.gApp.GetScreenWidth() + GameApp.gApp.mWideScreenXOffset - IMAGE_UI_CHALLENGESCREEN_BG_SIDE.GetWidth(), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_BG_SIDE)));
		g.DrawImageMirror(IMAGE_GUI_TIKITEMPLE_PEDESTAL, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1)) - GameApp.gApp.mWideScreenXOffset - Common._DS(30), Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1)) + IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1.GetHeight() + Common._DS(15));
		g.DrawImage(IMAGE_GUI_TIKITEMPLE_PEDESTAL, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2)) - GameApp.gApp.mWideScreenXOffset - Common._DS(20) + mAspectOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2)) + IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2.GetHeight() - Common._DS(15));
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1)) - GameApp.gApp.mWideScreenXOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_1)) + Common._DS(120));
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2)) - GameApp.gApp.mWideScreenXOffset + mAspectOffset, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_TIKI_POLE_2)) + Common._DS(120));
		g.SetColor(255, 255, 255, 255);
		g.SetFont(Res.GetFontByID(ResID.FONT_SHAGEXOTICA100_GAUNTLET));
		string theString = TextManager.getInstance().getString(781);
		float num = g.GetFont().StringWidth(theString);
		g.DrawString(theString, (int)mTitleXOffset + Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_WOOD)) - GameApp.gApp.mWideScreenXOffset + (int)(((float)IMAGE_UI_CHALLENGESCREEN_WOOD.GetWidth() - num) / 2f), Common._DS(135));
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_DRUMS, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_DRUMS)) - GameApp.gApp.mWideScreenXOffset + mAspectOffset + 85, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_DRUMS)));
		g.DrawImage(IMAGE_UI_CHALLENGESCREEN_FRUIT, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_CHALLENGESCREEN_FRUIT)) - GameApp.gApp.mWideScreenXOffset - 66, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_CHALLENGESCREEN_FRUIT)));
		g.DrawImage(IMAGE_UI_LEADERBOARDS_LEAVES2, Common._DS(Res.GetOffsetXByID(ResID.IMAGE_UI_LEADERBOARDS_LEAVES2)) - GameApp.gApp.mWideScreenXOffset + GameApp.gApp.GetScreenRect().mX / 2 + mAspectOffset + 10, Common._DS(Res.GetOffsetYByID(ResID.IMAGE_UI_LEADERBOARDS_LEAVES2)));
		g.Translate(-mX / 2, 0);
	}

	public void ProcessHardwareBackButton()
	{
		GameApp.gApp.ToggleBambooTransition();
		GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.mMainMenu.HideTikiTemple;
		GameApp.gApp.OnHardwareBackButtonPressProcessed();
	}

	public void ButtonDepress(int id)
	{
		if ((GameApp.gApp.mBambooTransition == null || !GameApp.gApp.mBambooTransition.IsInProgress()) && mHomeButton != null && mHomeButton.mId == id)
		{
			GameApp.gApp.ToggleBambooTransition();
			GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.mMainMenu.HideTikiTemple;
		}
	}

	public void ButtonPress(int id)
	{
		if (GameApp.gApp.mBambooTransition == null || !GameApp.gApp.mBambooTransition.IsInProgress())
		{
			GameApp.gApp.PlaySample(1768);
		}
	}

	public void ButtonPress(int theId, int theClickCount)
	{
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
}
