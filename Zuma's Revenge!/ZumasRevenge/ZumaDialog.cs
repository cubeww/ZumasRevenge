using System;
using System.Collections.Generic;
using System.Linq;
using JeffLib;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class ZumaDialog : DialogEx
{
	public bool mCenterInitially;

	public bool mAllowDrag;

	public List<ZumaDialogLine> mCustomLines = new List<ZumaDialogLine>();

	protected int mMinWidth;

	protected int mMinHeight;

	protected int mTargetWidth;

	protected int mTargetHeight;

	protected int mNumWidthSpacers;

	protected int mNumHeightSpacers;

	protected Widget mLastFocusWidget;

	private Image IMAGE_GUI_D11;

	private Image IMAGE_GUI_D12;

	private Image IMAGE_GUI_D13;

	private Image IMAGE_GUI_D01;

	private Image IMAGE_GUI_D02;

	private Image IMAGE_GUI_D03;

	private Image IMAGE_GUI_D04;

	private Image IMAGE_GUI_D05;

	private Image IMAGE_GUI_D06;

	private Image IMAGE_GUI_D07;

	private Image IMAGE_GUI_D08;

	private Image IMAGE_GUI_D09;

	private Image IMAGE_GUI_D10;

	public ZumaDialog(int id, bool isModal, string header, string lines, string footer, int btn_mode)
		: base(Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BUTTON), Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BUTTON), id, isModal, header, lines, footer, btn_mode)
	{
		IMAGE_GUI_D11 = Res.GetImageByID(ResID.IMAGE_GUI_D11);
		IMAGE_GUI_D12 = Res.GetImageByID(ResID.IMAGE_GUI_D12);
		IMAGE_GUI_D13 = Res.GetImageByID(ResID.IMAGE_GUI_D13);
		IMAGE_GUI_D01 = Res.GetImageByID(ResID.IMAGE_GUI_D01);
		IMAGE_GUI_D02 = Res.GetImageByID(ResID.IMAGE_GUI_D02);
		IMAGE_GUI_D03 = Res.GetImageByID(ResID.IMAGE_GUI_D03);
		IMAGE_GUI_D04 = Res.GetImageByID(ResID.IMAGE_GUI_D04);
		IMAGE_GUI_D05 = Res.GetImageByID(ResID.IMAGE_GUI_D05);
		IMAGE_GUI_D06 = Res.GetImageByID(ResID.IMAGE_GUI_D06);
		IMAGE_GUI_D07 = Res.GetImageByID(ResID.IMAGE_GUI_D07);
		IMAGE_GUI_D08 = Res.GetImageByID(ResID.IMAGE_GUI_D08);
		IMAGE_GUI_D09 = Res.GetImageByID(ResID.IMAGE_GUI_D09);
		IMAGE_GUI_D10 = Res.GetImageByID(ResID.IMAGE_GUI_D10);
		mMinWidth = IMAGE_GUI_D10.mWidth + IMAGE_GUI_D12.mWidth + IMAGE_GUI_D02.mWidth;
		mMinHeight = IMAGE_GUI_D12.mHeight + IMAGE_GUI_D13.mHeight;
		mTargetWidth = mMinWidth;
		mTargetHeight = mMinHeight;
		mCenterInitially = true;
		mNumWidthSpacers = 0;
		mNumHeightSpacers = 0;
		mLastFocusWidget = null;
		if (!GameApp.gApp.mResourceManager.IsGroupLoaded("CommonGame") && !GameApp.gApp.mResourceManager.LoadResources("CommonGame"))
		{
			GameApp.gApp.Shutdown();
		}
		mAllowDrag = false;
		mPriority = 2;
		mBackgroundInsets = new Insets(Common._S(Common._M(16)), Common._S(Common._M1(61)), Common._S(Common._M2(18)), Common._S(Common._M3(50)));
		mContentInsets = new Insets(Common._S(Common._M(14)), Common._S(Common._M1(50)), Common._S(Common._M2(14)), Common._S(Common._M3(10)));
		mHasAlpha = (mHasTransparencies = true);
		mDrawScale.SetCurve(Common._MP("b+0,2,0.033333,1,####        cY### >P###"));
	}

	~ZumaDialog()
	{
	}

	public override void Resize(int x, int y, int w, int h)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_D11);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_GUI_D09);
		int num = Math.Max(0, w - mMinWidth);
		int num2 = imageByID.mWidth * 2;
		mNumWidthSpacers = ((num % num2 == 0) ? (num / num2) : (num / num2 + 1));
		w = mMinWidth + mNumWidthSpacers * imageByID.mWidth * 2;
		num = Math.Max(0, h - mMinHeight);
		num2 = imageByID2.mHeight;
		mNumHeightSpacers = ((num % num2 == 0) ? (num / num2) : (num / num2 + 1));
		h = mMinHeight + mNumHeightSpacers * imageByID2.mHeight;
		if (mCenterInitially)
		{
			x = (GlobalMembers.gSexyApp.mWidth - w) / 2;
			y = (GlobalMembers.gSexyApp.mHeight - h) / 2;
			mCenterInitially = false;
		}
		mTargetWidth = w;
		mTargetHeight = h;
		mButtonSidePadding = Common._S(Common._M(30));
		mButtonHorzSpacing = Common._S(Common._M(100));
		base.Resize(x, y, w, h);
		SizeButtons();
	}

	public override void Update()
	{
		base.Update();
	}

	public override void Draw(Graphics g)
	{
		g.ClearClipRect();
		g.PushState();
		g.Translate(-mX, -mY);
		g.SetColor(0, 0, 0, 130);
		g.FillRect(Common._S(-80), 0, GameApp.gApp.mWidth + Common._S(160), GameApp.gApp.mHeight);
		g.PopState();
		base.Draw(g);
		if (mCustomLines.Count() > 0)
		{
			mDialogLines = "";
			mDialogHeader = "";
		}
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BOX_MAINMENU_FRAME_WOOD);
		g.ClearClipRect();
		g.ClipRect(IMAGE_GUI_D11.mWidth, IMAGE_GUI_D12.mHeight / 2 + 10, mWidth - IMAGE_GUI_D11.mWidth * 2, mHeight - IMAGE_GUI_D12.mHeight);
		int num = 0;
		int i = 0;
		bool flag = false;
		for (; i <= mHeight; i += imageByID.GetHeight())
		{
			while (num < mWidth)
			{
				if (flag)
				{
					g.DrawImageMirror(imageByID, num, i);
				}
				else
				{
					g.DrawImage(imageByID, num, i);
				}
				num += imageByID.GetWidth();
				flag = !flag;
			}
			num = 0;
		}
		g.ClearClipRect();
		g.ClipRect(0, 0, mWidth, mHeight + Common._S(10));
		int num2 = (mWidth - IMAGE_GUI_D12.mWidth) / 2;
		g.DrawImage(IMAGE_GUI_D12, num2, Common._S(Common._M(7)));
		g.DrawImage(IMAGE_GUI_D13, (mWidth - IMAGE_GUI_D13.mWidth) / 2, mHeight - IMAGE_GUI_D13.mHeight + Common._S(Common._M(8)));
		int num3 = mHeight - IMAGE_GUI_D13.mHeight - Common._S(Common._M(13));
		g.DrawImage(IMAGE_GUI_D06, num2, num3);
		int num4 = num2;
		for (int j = 0; j < mNumWidthSpacers; j++)
		{
			num4 -= IMAGE_GUI_D11.mWidth;
			g.DrawImage(IMAGE_GUI_D11, num4, Common._S(Common._M(54)));
			g.DrawImage(IMAGE_GUI_D07, num4, num3 + IMAGE_GUI_D06.mHeight - IMAGE_GUI_D07.mHeight);
		}
		g.DrawImage(IMAGE_GUI_D10, num4 - IMAGE_GUI_D10.mWidth, Common._S(Common._M(54)));
		g.DrawImage(IMAGE_GUI_D08, num4 - IMAGE_GUI_D08.mWidth, num3 + IMAGE_GUI_D06.mHeight - IMAGE_GUI_D08.mHeight);
		num4 = num2 + IMAGE_GUI_D12.mWidth;
		for (int k = 0; k < mNumWidthSpacers; k++)
		{
			g.DrawImage(IMAGE_GUI_D01, num4, Common._S(Common._M(54)));
			g.DrawImage(IMAGE_GUI_D05, num4, num3 + IMAGE_GUI_D06.mHeight - IMAGE_GUI_D05.mHeight);
			num4 += IMAGE_GUI_D01.mWidth;
		}
		g.DrawImage(IMAGE_GUI_D02, num4, Common._S(Common._M(54)));
		g.DrawImage(IMAGE_GUI_D04, num4, num3 + IMAGE_GUI_D06.mHeight - IMAGE_GUI_D04.mHeight);
		int num5 = Common._S(Common._M(54)) + IMAGE_GUI_D10.mHeight;
		for (int l = 0; l < mNumHeightSpacers; l++)
		{
			g.DrawImage(IMAGE_GUI_D09, 0, num5);
			g.DrawImage(IMAGE_GUI_D03, mWidth - IMAGE_GUI_D03.mWidth, num5);
			num5 += IMAGE_GUI_D09.mHeight;
		}
		if (mCustomLines.Count() > 0)
		{
			int num6 = mContentInsets.mTop + mBackgroundInsets.mTop + Common._DS(Common._M(0));
			for (int m = 0; m < mCustomLines.Count(); m++)
			{
				ZumaDialogLine zumaDialogLine = mCustomLines[m];
				num6 += zumaDialogLine.mYPadding;
				g.SetFont(zumaDialogLine.mFont);
				g.SetColor(zumaDialogLine.mColor);
				g.WriteString(zumaDialogLine.mLine, mContentInsets.mLeft + mBackgroundInsets.mLeft, num6 + zumaDialogLine.mFont.GetAscent(), mWidth - mContentInsets.mLeft - mContentInsets.mRight - mBackgroundInsets.mLeft - mBackgroundInsets.mRight, 0);
				num6 += zumaDialogLine.mFont.GetHeight();
			}
			return;
		}
		int num7 = mContentInsets.mTop + mBackgroundInsets.mTop;
		if (mDialogHeader.Length > 0)
		{
			num7 += mHeaderFont.GetAscent() - mHeaderFont.GetAscentPadding();
			g.SetFont(mHeaderFont);
			g.SetColor(mColors[0]);
			WriteCenteredLine(g, num7, mDialogHeader);
			num7 += mHeaderFont.GetHeight() - mHeaderFont.GetAscent();
			num7 += mSpaceAfterHeader;
		}
		g.SetFont(mLinesFont);
		g.SetColor(mColors[1]);
		Rect theRect = new Rect(mBackgroundInsets.mLeft + mContentInsets.mLeft + 2, num7, mWidth - mContentInsets.mLeft - mContentInsets.mRight - mBackgroundInsets.mLeft - mBackgroundInsets.mRight - 4, 0);
		num7 += WriteWordWrapped(g, theRect, mDialogLines, mLinesFont.GetLineSpacing() + mLineSpacingOffset, mTextAlign);
		if (mDialogFooter.Length != 0 && mButtonMode != 3)
		{
			num7 += 8;
			num7 += mHeaderFont.GetLineSpacing();
			g.SetFont(mHeaderFont);
			g.SetColor(mColors[2]);
			WriteCenteredLine(g, num7, mDialogFooter);
		}
	}

	public override bool IsPointVisible(int x, int y)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_D12);
		int num = (mWidth - imageByID.mWidth) / 2;
		if ((y < Common._S(Common._M(54)) || y > mHeight - Common._S(Common._M1(30))) && (x < num || x > num + imageByID.mWidth))
		{
			return false;
		}
		return true;
	}

	public virtual void GetSize(ref int w, ref int h)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BUTTON);
		int num = mContentInsets.mLeft + mContentInsets.mRight + mBackgroundInsets.mLeft + mBackgroundInsets.mRight + 4;
		int num2 = mBackgroundInsets.mTop + mBackgroundInsets.mBottom + mContentInsets.mTop + mContentInsets.mBottom + mSpaceAfterHeader + imageByID.GetCelHeight() + Common._S(Common._M(40));
		w += num;
		h += num2;
	}

	public override void KeyDown(KeyCode key)
	{
		base.KeyDown(key);
		if (mButtonMode != 0)
		{
			switch (key)
			{
			case KeyCode.KEYCODE_ESCAPE:
				ButtonDepress(1001);
				break;
			case KeyCode.KEYCODE_RETURN:
				ButtonDepress(1000);
				break;
			}
		}
	}

	public override void AddedToManager(WidgetManager wm)
	{
		base.AddedToManager(wm);
		mLastFocusWidget = wm.mFocusWidget;
	}

	public override void RemovedFromManager(WidgetManager wm)
	{
		base.RemovedFromManager(wm);
		if (mLastFocusWidget != wm.mFocusWidget && !GlobalMembers.gSexyApp.mShutdown && mLastFocusWidget != null)
		{
			wm.SetFocus(mLastFocusWidget);
		}
	}

	public override void MouseDrag(int x, int y)
	{
		if (mAllowDrag)
		{
			base.MouseDrag(x, y);
		}
	}

	public override void ButtonPress(int inButtonID)
	{
		base.ButtonPress(inButtonID);
		GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BUTTON1));
	}

	public void SetFocusWidgetToBoard()
	{
		mLastFocusWidget = ((GameApp)GlobalMembers.gSexyApp).GetBoard();
	}

	public void SizeButtons()
	{
		int inWidth = Common._S(Common._M(120));
		if (mYesButton != null)
		{
			EnsureButtonMeetsWidth(mYesButton, inWidth);
			if (mNoButton == null)
			{
				int num = Common._S(Common._M(120));
				mYesButton.Resize((mWidth - num) / 2, mHeight - mContentInsets.mBottom - mBackgroundInsets.mBottom - mButtonHeight - Common._S(Common._M(7)), num, mButtonHeight);
			}
		}
		if (mNoButton != null)
		{
			EnsureButtonMeetsWidth(mNoButton, inWidth);
		}
	}

	public void EnsureButtonMeetsWidth(DialogButton inButton, int inWidth)
	{
		if (inButton.mWidth < inWidth)
		{
			inButton.Resize((int)((float)inButton.mX - (float)(inWidth - inButton.mWidth) * 0.5f), inButton.mY, inWidth, inButton.mHeight);
		}
	}

	public int GetLeft()
	{
		return mX + mContentInsets.mLeft + mBackgroundInsets.mLeft;
	}

	public int GetTop()
	{
		return mY + mContentInsets.mTop + mBackgroundInsets.mTop + Common._S(54);
	}

	public int GetWidth()
	{
		return mWidth - mContentInsets.mLeft - mContentInsets.mRight - mBackgroundInsets.mLeft - mBackgroundInsets.mRight;
	}

	public void Kill()
	{
		mDrawScale.SetCurve(Common._MP("b+0,1,0.05,1,~###         ~#A5t"));
		mWidgetFlagsMod.mRemoveFlags |= 16;
	}

	internal void WaitForResult()
	{
	}
}
