using SexyFramework.Widget;

namespace ZumasRevenge;

public class WidescreenBoardWidget : Widget
{
	public GameApp mApp;

	public WidescreenBoardWidget()
	{
		mWidgetFlagsMod.mRemoveFlags |= 5;
		mApp = GameApp.gApp;
		mZOrder = 2147483646;
	}

	public override void MouseDown(int x, int y, int theClickCount)
	{
		if (mApp.GetBoard() != null)
		{
			mApp.GetBoard().MouseDown(x - Common._S(80), y, theClickCount);
		}
	}

	public override void MouseUp(int x, int y, int theClickCount)
	{
		if (mApp.GetBoard() != null)
		{
			mApp.GetBoard().MouseUp(x - Common._S(80), y, theClickCount);
		}
	}

	public override bool IsPointVisible(int x, int y)
	{
		if (mApp.GetBoard() == null || (x >= Common._S(80) && x <= mApp.mWidth + Common._S(80)))
		{
			return false;
		}
		return true;
	}

	public override void MouseMove(int x, int y)
	{
		if (mApp.GetBoard() != null)
		{
			mApp.GetBoard().MouseMove(x - Common._S(80), y);
		}
	}

	public override void MouseDrag(int x, int y)
	{
		if (mApp.GetBoard() != null)
		{
			mApp.GetBoard().MouseMove(x - Common._S(80), y);
		}
	}
}
