using JeffLib;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class ZumaSlideBox : Widget, ScrollWidgetListener
{
	public Rect mLabelFrame;

	public string mLabel;

	public bool mIsOff;

	public ScrollWidget mScrollBox;

	public ZumaSlideBoxButton mSlideBoxButton;

	public DialogEx mDialog;

	public ZumaSlideBox(DialogEx theDialog, int id, string label)
	{
		mLabel = label;
		mDialog = theDialog;
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BOX_MAINMENU_RED_LIGHT);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BOX_MAINMENU_SLIDEBOXBACK);
		Font fontByID = Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_GAUNTLET);
		Rect theRect = new Rect
		{
			mX = 0,
			mY = 0,
			mWidth = imageByID.GetWidth() * 2,
			mHeight = imageByID.GetHeight()
		};
		mLabelFrame = default(Rect);
		mLabelFrame.mWidth = imageByID2.GetWidth() - theRect.mWidth - Common._S(9);
		mLabelFrame.mHeight = imageByID2.GetHeight();
		mLabelFrame.mX = 0;
		mLabelFrame.mY = (int)((float)(mLabelFrame.mHeight - fontByID.GetHeight()) * 0.5f);
		mSlideBoxButton = new ZumaSlideBoxButton(this);
		mSlideBoxButton.Resize(theRect);
		mScrollBox = new ScrollWidget(this);
		mScrollBox.Resize(mLabelFrame.mWidth, (mLabelFrame.mHeight - theRect.mHeight) / 2, theRect.mWidth, theRect.mHeight);
		mScrollBox.AddWidget(mSlideBoxButton);
		mScrollBox.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_HORIZONTAL);
		mScrollBox.EnablePaging(enable: true);
		AddWidget(mScrollBox);
		Insets scrollInsets = new Insets
		{
			mLeft = 0,
			mRight = mSlideBoxButton.mWidth / 2,
			mTop = 0,
			mBottom = 0
		};
		mScrollBox.SetScrollInsets(scrollInsets);
		mScrollBox.SetPageHorizontal(0, animated: false);
		mScrollBox.EnableBounce(enable: false);
	}

	~ZumaSlideBox()
	{
	}

	public override void Draw(Graphics g)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BOX_MAINMENU_SLIDEBOXBACK);
		Font fontByID = Res.GetFontByID(ResID.FONT_SHAGLOUNGE38_GAUNTLET);
		g.DrawImage(imageByID, 0, 0);
		g.SetFont(fontByID);
		g.SetColor(255, 255, 45, 255);
		g.WriteWordWrapped(mLabelFrame, mLabel, -1, 0);
	}

	public override void DrawOverlay(Graphics g)
	{
	}

	public void ScrollTargetReached(ScrollWidget scrollWidget)
	{
		mIsOff = scrollWidget.GetPageHorizontal() == 1;
		GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BUTTON1));
	}

	public void ScrollTargetInterrupted(ScrollWidget scrollWidget)
	{
	}

	public void SetOnOff(bool isOn)
	{
		mIsOff = !isOn;
		mScrollBox.SetPageHorizontal(mIsOff ? 1 : 0, animated: false);
	}

	public bool IsOn()
	{
		return !mIsOff;
	}
}
