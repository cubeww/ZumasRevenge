using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class CreditsHackWidget : Widget, ButtonListener
{
	public ButtonWidget mContinueBtn;

	public CreditsHackWidget()
	{
		Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_HOME_SELECT);
		Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_HOME);
		mPriority = 2147483646;
		mZOrder = 2147483646;
		mHasAlpha = (mHasTransparencies = true);
		mClip = false;
	}

	public override void Dispose()
	{
		RemoveAllWidgets(doDelete: true, recursive: true);
	}

	public virtual void ButtonPress(int theId, int theClickCount)
	{
		ButtonPress(theId);
	}

	public virtual void ButtonPress(int theId)
	{
		if (GameApp.gApp.mCredits != null)
		{
			GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BUTTON2));
		}
	}

	public virtual void ButtonDepress(int theId)
	{
		if (GameApp.gApp.mCredits != null)
		{
			GameApp.gApp.ReturnFromCredits();
		}
	}

	public override void Update()
	{
		if (GameApp.gApp.mCredits != null && GameApp.gApp.mHasFocus)
		{
			MarkDirty();
			GameApp.gApp.mCredits.Update();
		}
	}

	public override void Draw(Graphics g)
	{
		if (GameApp.gApp.mCredits != null)
		{
			GameApp.gApp.mCredits.Draw(g);
		}
	}

	public override void MouseUp(int x, int y)
	{
		if (GameApp.gApp.mCredits == null)
		{
			return;
		}
		GameApp.gApp.mCredits.mSpeedUp = false;
		if (GameApp.gApp.mCredits.AtEnd() && GameApp.gApp.mCredits.mTapDown)
		{
			GameApp.gApp.mCredits.mTapDown = false;
			if (GameApp.gApp.GetDialog(2) is OptionsDialog optionsDialog)
			{
				optionsDialog.OnCreditsHided();
			}
			GameApp.gApp.ReturnFromCredits();
		}
	}

	public override void MouseDown(int x, int y, int theClickCount)
	{
		if (GameApp.gApp.mCredits != null)
		{
			GameApp.gApp.mCredits.mSpeedUp = true;
			if (GameApp.gApp.mCredits.AtEnd())
			{
				GameApp.gApp.mCredits.mTapDown = true;
			}
		}
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
}
