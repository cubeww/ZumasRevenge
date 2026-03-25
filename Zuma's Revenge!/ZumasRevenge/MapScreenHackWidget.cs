using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class MapScreenHackWidget : Widget
{
	public GameApp mApp;

	public int mDelay;

	public bool mToggledAdventureMode;

	public MapScreenHackWidget()
	{
		mClip = false;
		mApp = GameApp.gApp;
		mDelay = 0;
		mToggledAdventureMode = false;
	}

	public override void Update()
	{
		if (mApp.mMapScreen != null && mApp.mMapScreen.mDirty)
		{
			MarkDirty();
		}
		if (mDelay == 0)
		{
			mApp.mMapScreen.Update();
			if (mApp.mMapScreen != null && mApp.mMapScreen.mRemove)
			{
				if (mApp.mMapScreen.mSelectedZone == -1)
				{
					mDelay = Common._M(10);
				}
				else
				{
					mDelay = Common._M(40);
				}
			}
		}
		else if (mApp.mMapScreen != null)
		{
			mDelay--;
			if (mDelay == 0 && !mToggledAdventureMode)
			{
				mToggledAdventureMode = true;
				mApp.mMapScreen.CleanButtons();
				mApp.mForceZoneRestart = mApp.mMapScreen.mSelectedZone;
				mApp.mBambooTransition.mTransitionDelegate = mApp.StartAdventureMode;
				mApp.ToggleBambooTransition();
			}
		}
	}

	public override void Draw(Graphics g)
	{
		if (mApp.mMapScreen != null)
		{
			mApp.mMapScreen.Draw(g);
		}
	}

	public override void DrawAll(ModalFlags theFlags, Graphics g)
	{
		g?.Get3D();
		base.DrawAll(theFlags, g);
	}

	public override void MouseMove(int x, int y)
	{
		if (mApp.mMapScreen != null && mApp.mDialogMap.Count <= 0 && mDelay <= 0)
		{
			mApp.mMapScreen.MouseMove(x, y);
		}
	}

	public override void MouseDrag(int x, int y)
	{
		if (mApp.mMapScreen != null && mApp.mDialogMap.Count <= 0 && mDelay <= 0)
		{
			mApp.mMapScreen.MouseMove(x, y);
		}
	}

	public override void MouseDown(int x, int y, int cc)
	{
		if (mApp.mMapScreen != null && mApp.mDialogMap.Count <= 0 && mDelay <= 0)
		{
			mApp.mMapScreen.MouseDown(x, y);
		}
	}

	public override void MouseUp(int x, int y)
	{
		if (mApp.mMapScreen != null && mApp.mDialogMap.Count <= 0 && mDelay <= 0)
		{
			mApp.mMapScreen.MouseUp(x, y);
		}
	}

	public override void MouseLeave()
	{
		mApp.mMapScreen.MouseLeave();
	}

	public override void KeyChar(char theChar)
	{
	}

	public override void GotFocus()
	{
		base.GotFocus();
		if (mWidgetManager != null && mApp.mMapScreen != null && mApp.mMapScreen.mContinueBtn != null)
		{
			mWidgetManager.SetGamepadSelection(mApp.mMapScreen.mContinueBtn, WidgetLinkDir.LINK_DIR_NONE);
		}
	}
}
