using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class ZumaSlider : Slider
{
	public string mLabel;

	public int mLabelWidth;

	public int mFeedbackSoundID;

	public string Label
	{
		get
		{
			return mLabel;
		}
		set
		{
			mLabel = value;
			Font fontByID = Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_BASE);
			mLabelWidth = fontByID.StringWidth(mLabel);
		}
	}

	public ZumaSlider(int id, SliderListener listener, string label)
		: base(Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BOX_MAINMENU_THUMB), Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BOX_MAINMENU_SLIDER), id, listener)
	{
		Font fontByID = Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_BASE);
		mFeedbackSoundID = -1;
		mLabel = label;
		mLabelWidth = fontByID.StringWidth(mLabel);
		mHasAlpha = (mHasTransparencies = true);
	}

	public override void Draw(Graphics g)
	{
		Font fontByID = Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_BASE);
		g.PushState();
		g.ClearClipRect();
		g.SetFont(fontByID);
		g.SetColor(255, 255, 64, 255);
		int num = Common._S(Common._M(20));
		int num2 = Common._S(Common._M(-35));
		g.DrawString(mLabel, (mWidth + num - mLabelWidth) / 2, g.mFont.mAscent + mHeight + num2 - Common._S(Common._M(12)) - g.mFont.mHeight);
		g.PopState();
		base.Draw(g);
	}

	public override void MouseEnter()
	{
		base.MouseEnter();
		MarkDirty();
	}

	public override void MouseLeave()
	{
		base.MouseLeave();
		MarkDirty();
	}

	public override void MouseUp(int x, int y)
	{
		base.MouseUp(x, y);
		if (mFeedbackSoundID >= 0)
		{
			GameApp.gApp.PlaySample(mFeedbackSoundID);
		}
	}
}
