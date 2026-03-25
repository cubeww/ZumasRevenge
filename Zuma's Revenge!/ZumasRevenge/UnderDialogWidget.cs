using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class UnderDialogWidget : Widget
{
	public DeviceImage mShrunkScreen1;

	public DeviceImage mShrunkScreen2;

	public UnderDialogWidget()
	{
		mMouseVisible = false;
		mHasAlpha = true;
		mShrunkScreen1 = null;
		mShrunkScreen2 = null;
		mClip = false;
	}

	~UnderDialogWidget()
	{
	}

	public void CreateImages()
	{
	}

	public void DrawPaused(Graphics g)
	{
	}

	public override void Update()
	{
		base.Update();
		if (GameApp.gApp.mDialogObscurePct > 0f && GlobalMembers.gSexyAppBase.mHasFocus)
		{
			MarkDirty();
		}
	}

	public override void Draw(Graphics g)
	{
	}
}
