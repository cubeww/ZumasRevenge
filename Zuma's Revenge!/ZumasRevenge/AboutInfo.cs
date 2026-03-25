using JeffLib;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class AboutInfo : DialogEx, SliderListener
{
	private DialogButton mOKBtn;

	private int mVersionTextY;

	private string mMetricsSharingText;

	private Font FONT_SHAGLOUNGE28_GREEN;

	private Font FONT_SHAGEXOTICA68_BASE;

	private Font FONT_SHAGLOUNGE28_BROWN;

	private Image IMAGE_GUI_DIALOG_BOX_MAINMENU_CROWN_BOX;

	private Image IMAGE_GUI_DIALOG_BOX_MAINMENU_SLIDEBOXBACK;

	private Localization.LanguageType mCurrentLanguage;

	public AboutInfo()
		: base(null, null, 12, isModal: true, "", "", "", 0)
	{
		FONT_SHAGLOUNGE28_GREEN = Res.GetFontByID(ResID.FONT_SHAGLOUNGE28_GREEN);
		FONT_SHAGEXOTICA68_BASE = Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_BASE);
		FONT_SHAGLOUNGE28_BROWN = Res.GetFontByID(ResID.FONT_SHAGLOUNGE28_BROWN);
		IMAGE_GUI_DIALOG_BOX_MAINMENU_CROWN_BOX = Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BOX_MAINMENU_CROWN_BOX);
		IMAGE_GUI_DIALOG_BOX_MAINMENU_SLIDEBOXBACK = Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BOX_MAINMENU_SLIDEBOXBACK);
		mOKBtn = null;
		int num = Common._DS(Common._M(304));
		int num2 = Common._DS(Common._M(162));
		Common._DS(85);
		Common._DS(25);
		Common._DS(75);
		int num3 = 840;
		int num4 = 640;
		Resize((GameApp.gApp.mWidth - num3) / 2, (GameApp.gApp.GetScreenRect().mHeight - num4) / 2, num3, num4);
		mVersionTextY = 530;
		mOKBtn = Common.MakeButton(0, this, TextManager.getInstance().getString(483));
		mOKBtn.SetFont(FONT_SHAGLOUNGE28_GREEN);
		int num5 = 10;
		mOKBtn.Resize((mWidth - num) / 2, mHeight - num2 - num5, num, num2);
		AddWidget(mOKBtn);
		mMetricsSharingText = TextManager.getInstance().getString(861);
		mHasTransparencies = (mHasAlpha = true);
		mClip = false;
		mDrawScale.SetCurve(Common._MP("b+0,2,0.033333,1,####        cY### >P###"));
		mCurrentLanguage = Localization.GetCurrentLanguage();
	}

	public override void Dispose()
	{
		RemoveAllWidgets(doDelete: false, recursive: true);
	}

	public override void RemoveAllWidgets(bool doDelete, bool recursive)
	{
		base.RemoveAllWidgets(doDelete, recursive);
		mOKBtn = null;
	}

	public override void Draw(Graphics g)
	{
		Common.DrawCommonDialogBacking(g, 0, 0, mWidth, mHeight);
		g.SetFont(FONT_SHAGEXOTICA68_BASE);
		g.SetColor(new Color(205, 151, 57));
		g.WriteString(TextManager.getInstance().getString(862), 0, 60, mWidth, 0);
		g.SetFont(FONT_SHAGLOUNGE28_GREEN);
		string text = "";
		string theString = TextManager.getInstance().getString(485) + " " + GameApp.gApp.mProductVersion + text;
		g.WriteString(theString, 0, mVersionTextY, mWidth, 0);
		int num = Common._DS(30);
		int theMaxWidth = 0;
		int theLineCount = 0;
		g.GetWordWrappedHeight(num * 2, mMetricsSharingText, -1, ref theMaxWidth, ref theLineCount);
		Common._DS(50);
		Rect theDest = new Rect(15, 70, 810, 430);
		g.DrawImageBox(theDest, IMAGE_GUI_DIALOG_BOX_MAINMENU_CROWN_BOX);
		Rect theRect = new Rect(theDest.mX + num, theDest.mY + num, theDest.mWidth - num * 2, theDest.mHeight - num * 2);
		g.SetColor(Color.White);
		g.WriteWordWrapped(theRect, mMetricsSharingText);
		g.SetFont(FONT_SHAGLOUNGE28_BROWN);
		g.SetColor(Color.White);
	}

	public override void ButtonPress(int inButtonID)
	{
		GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BUTTON1));
		base.ButtonPress(inButtonID);
	}

	public void ProcessHardwareBackButton()
	{
		ButtonDepress(3);
		GameApp.gApp.OnHardwareBackButtonPressProcessed();
	}

	public override void ButtonDepress(int inButtonID)
	{
		mDrawScale.SetCurve(Common._MP("b+0,1,0.05,1,~###         ~#A5t"));
		mWidgetFlagsMod.mRemoveFlags |= 16;
		GameApp.gApp.HideAbout();
	}

	public override void MouseDrag(int x, int y)
	{
	}

	public void SliderVal(int theId, double theVal)
	{
	}

	public void SliderReleased(int theId, double theVal)
	{
	}
}
