using System;
using JeffLib;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class LegalInfo : DialogEx, SliderListener
{
	private enum LegalButtonIDs
	{
		Legal_EndUserLicenseAgreementID,
		Legal_PrivacyPolicyID,
		Legal_TermsOfServiceID,
		Legal_OKID,
		Legal_AboutID,
		Legal_HelpID,
		Legal_MetricsSharingID
	}

	private class ExternalLinkDialog : ZumaDialog
	{
		private string mURL;

		private LegalInfo mLegalInfo;

		public ExternalLinkDialog(LegalInfo theLegalInfo, string theURL)
			: base(13, isModal: true, TextManager.getInstance().getString(486), TextManager.getInstance().getString(487), "", 2)
		{
			mLegalInfo = theLegalInfo;
			mURL = theURL;
		}

		~ExternalLinkDialog()
		{
		}

		public override void Resize(int x, int y, int w, int h)
		{
			base.Resize(x, y, w, h);
			ButtonWidget[] inButtons = new ButtonWidget[2] { mYesButton, mNoButton };
			Common.SizeButtonsToLabel(inButtons, 2, Common._S(20));
		}

		public override void ButtonDepress(int id)
		{
			if (id == 2000 + mId || id == 1000)
			{
				GameApp.gApp.OpenURL(mURL);
				mLegalInfo.HideExternalLinkInfo();
			}
			else if (id == 3000 + mId || id == 1001)
			{
				mLegalInfo.HideExternalLinkInfo();
			}
		}
	}

	private DialogButton mEndUserLicenseAgreement;

	private DialogButton mPrivacyPolicy;

	private DialogButton mTermsOfService;

	private DialogButton mAboutBtn;

	private DialogButton mHelpBtn;

	private DialogButton mOKBtn;

	private int mVersionTextY;

	private ExternalLinkDialog mExternalLinkDialog;

	private Font FONT_SHAGLOUNGE28_GREEN;

	private Font FONT_SHAGEXOTICA68_BASE;

	private Font FONT_SHAGLOUNGE28_BROWN;

	private Image IMAGE_GUI_DIALOG_BOX_MAINMENU_CROWN_BOX;

	private Image IMAGE_GUI_DIALOG_BOX_MAINMENU_SLIDEBOXBACK;

	private Localization.LanguageType mCurrentLanguage;

	public LegalInfo()
		: base(null, null, 11, isModal: true, "", "", "", 0)
	{
		FONT_SHAGLOUNGE28_GREEN = Res.GetFontByID(ResID.FONT_SHAGLOUNGE28_GREEN);
		FONT_SHAGEXOTICA68_BASE = Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_BASE);
		FONT_SHAGLOUNGE28_BROWN = Res.GetFontByID(ResID.FONT_SHAGLOUNGE28_BROWN);
		IMAGE_GUI_DIALOG_BOX_MAINMENU_CROWN_BOX = Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BOX_MAINMENU_CROWN_BOX);
		IMAGE_GUI_DIALOG_BOX_MAINMENU_SLIDEBOXBACK = Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BOX_MAINMENU_SLIDEBOXBACK);
		mEndUserLicenseAgreement = null;
		mPrivacyPolicy = null;
		mTermsOfService = null;
		mAboutBtn = null;
		mHelpBtn = null;
		mOKBtn = null;
		mExternalLinkDialog = null;
		int num = Common._DS(Common._M(304));
		int num2 = Common._DS(Common._M(162));
		int num3 = Common._DS(85);
		int num4 = Common._DS(25);
		int num5 = Common._DS(75);
		int num6 = 0;
		string theText = TextManager.getInstance().getString(1);
		string theText2 = TextManager.getInstance().getString(862);
		string text = TextManager.getInstance().getString(480);
		int val = FONT_SHAGLOUNGE28_GREEN.StringWidth(text);
		string text2 = TextManager.getInstance().getString(481);
		int val2 = FONT_SHAGLOUNGE28_GREEN.StringWidth(text2);
		string text3 = TextManager.getInstance().getString(482);
		int val3 = FONT_SHAGLOUNGE28_GREEN.StringWidth(text3);
		int num7 = Math.Max(val, Math.Max(val2, val3));
		mAboutBtn = Common.MakeButton(4, this, theText2);
		mAboutBtn.SetFont(FONT_SHAGLOUNGE28_GREEN);
		mAboutBtn.Resize(num5, Common._DS(Common._M(30)) + num4, num7 + num3, num2);
		AddWidget(mAboutBtn);
		mEndUserLicenseAgreement = Common.MakeButton(0, this, text);
		mEndUserLicenseAgreement.SetFont(FONT_SHAGLOUNGE28_GREEN);
		mEndUserLicenseAgreement.Resize(num5, mAboutBtn.mY + mAboutBtn.mHeight + num4, num7 + num3, num2);
		num6 = mEndUserLicenseAgreement.mWidth;
		AddWidget(mEndUserLicenseAgreement);
		mPrivacyPolicy = Common.MakeButton(1, this, text2);
		mPrivacyPolicy.SetFont(FONT_SHAGLOUNGE28_GREEN);
		mPrivacyPolicy.Resize(num5, mEndUserLicenseAgreement.mY + mEndUserLicenseAgreement.mHeight + num4, num7 + num3, num2);
		num6 = ((num6 < mPrivacyPolicy.mWidth) ? mPrivacyPolicy.mWidth : num6);
		AddWidget(mPrivacyPolicy);
		mTermsOfService = Common.MakeButton(2, this, text3);
		mTermsOfService.SetFont(FONT_SHAGLOUNGE28_GREEN);
		mTermsOfService.Resize(num5, mPrivacyPolicy.mY + mPrivacyPolicy.mHeight + num4, num7 + num3, num2);
		num6 = ((num6 < mTermsOfService.mWidth) ? mTermsOfService.mWidth : num6);
		AddWidget(mTermsOfService);
		mHelpBtn = Common.MakeButton(5, this, theText);
		mHelpBtn.SetFont(FONT_SHAGLOUNGE28_GREEN);
		mHelpBtn.Resize(num5, mTermsOfService.mY + mTermsOfService.mHeight + num4, num7 + num3, num2);
		AddWidget(mHelpBtn);
		int num8 = num6 + num5 * 2;
		int num9 = (int)((float)(mTermsOfService.mY + num4) + (float)num2 * 3f) + 20;
		Resize((GameApp.gApp.mWidth - num8) / 2, (GameApp.gApp.GetScreenRect().mHeight - num9) / 2, num8, num9);
		mVersionTextY = mHelpBtn.mY + mHelpBtn.mHeight + num4 + 29;
		mOKBtn = Common.MakeButton(3, this, TextManager.getInstance().getString(483));
		mOKBtn.SetFont(FONT_SHAGLOUNGE28_GREEN);
		int num10 = 10;
		mOKBtn.Resize((mWidth - num) / 2, mHeight - num2 - num10, num, num2);
		AddWidget(mOKBtn);
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
		mEndUserLicenseAgreement = null;
		mPrivacyPolicy = null;
		mTermsOfService = null;
		mOKBtn = null;
		mAboutBtn = null;
		mHelpBtn = null;
		mExternalLinkDialog = null;
	}

	public override void Draw(Graphics g)
	{
		Common.DrawCommonDialogBacking(g, 0, 0, mWidth, mHeight);
	}

	public override void ButtonPress(int inButtonID)
	{
		GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BUTTON1));
		base.ButtonPress(inButtonID);
	}

	public void ProcessHardwareBackButton()
	{
		if (mExternalLinkDialog != null)
		{
			mExternalLinkDialog.ButtonDepress(1001);
		}
		else if (GameApp.gApp.mGenericHelp != null)
		{
			GameApp.gApp.mGenericHelp.ButtonDepress(1000);
		}
		else
		{
			ButtonDepress(3);
		}
		GameApp.gApp.OnHardwareBackButtonPressProcessed();
	}

	public override void ButtonDepress(int inButtonID)
	{
		if (mExternalLinkDialog == null)
		{
			string text = "";
			text = (int)mCurrentLanguage switch
			{
				1 => text + "fr", 
				2 => text + "it", 
				3 => text + "de", 
				4 => text + "es", 
				5 => text + "sc", 
				10 => text + "tc", 
				8 => text + "pt", 
				11 => text + "br", 
				7 => text + "pl", 
				6 => text + "ru", 
				9 => text + "es", 
				_ => text + "en", 
			};
			if (mOKBtn != null && inButtonID == mOKBtn.mId)
			{
				mDrawScale.SetCurve(Common._MP("b+0,1,0.05,1,~###         ~#A5t"));
				mWidgetFlagsMod.mRemoveFlags |= 16;
				GameApp.gApp.HideLegal();
			}
			else if (mEndUserLicenseAgreement != null && inButtonID == mEndUserLicenseAgreement.mId)
			{
				ShowExternalLinkInfo("http://tos.ea.com/legalapp/mobileeula/US/" + text + "/GM");
			}
			else if (mTermsOfService != null && inButtonID == mTermsOfService.mId)
			{
				ShowExternalLinkInfo("http://tos.ea.com/legalapp/WEBTERMS/US/" + text + "/PC");
			}
			else if (mPrivacyPolicy != null && inButtonID == mPrivacyPolicy.mId)
			{
				ShowExternalLinkInfo("http://tos.ea.com/legalapp/WEBPRIVACY/US/" + text + "/PC/");
			}
			else if (mAboutBtn != null && inButtonID == mAboutBtn.mId)
			{
				GameApp.gApp.ShowAbout();
			}
			else if (mHelpBtn != null && inButtonID == mHelpBtn.mId)
			{
				GameApp.gApp.mGenericHelp = new GenericHelp();
				GameApp.gApp.AddDialog(GameApp.gApp.mGenericHelp);
			}
		}
	}

	public override void MouseDrag(int x, int y)
	{
	}

	private void ShowExternalLinkInfo(string theURL)
	{
		if (mExternalLinkDialog == null)
		{
			mExternalLinkDialog = new ExternalLinkDialog(this, theURL);
			Common.SetupDialog(mExternalLinkDialog);
			GameApp.gApp.AddDialog(mExternalLinkDialog);
		}
	}

	public void HideExternalLinkInfo()
	{
		mExternalLinkDialog.mDrawScale.SetCurve(Common._MP("b+0,1,0.05,1,~###         ~#A5t"));
		mExternalLinkDialog.mWidgetFlagsMod.mRemoveFlags |= 16;
		mExternalLinkDialog = null;
	}

	public void SliderVal(int theId, double theVal)
	{
	}

	public void SliderReleased(int theId, double theVal)
	{
	}
}
