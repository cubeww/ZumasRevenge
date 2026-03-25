using System.Collections.Generic;
using System.Text;
using JeffLib;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class ChallengeHelp : Widget, ButtonListener
{
	public Board mBoard;

	public MemoryImage mCutoutImage;

	public DialogButton mOKBtn;

	public List<MaskedRect> mMaskedRects = new List<MaskedRect>();

	public PIEffect mMultFX;

	public bool mFromHelp;

	public CurvedVal mDrawScale;

	public bool mClosing;

	private Font FONT_SHAGLOUNGE28_STROKE;

	private Image IMAGE_GUI_ARROW_RED;

	private Image IMAGE_GUI_BARIMAGE;

	private Image IMAGE_GUI_EQUALIMAGE;

	private Image IMAGE_GUI_BALLIMAGE;

	private Image IMAGE_GUI_DIALOG_MARQUE_BOX;

	private Image IMAGE_UI_CHALLENGE_GAUGE_EMPTY;

	private Image IMAGE_UI_CHALLENGE_GAUGE_FILL;

	public ChallengeHelp(bool from_help)
	{
		mBoard = GameApp.gApp.mBoard;
		int num = Common._DS(Common._M(434));
		int num2 = Common._DS(Common._M(80));
		int x = Common._DS(Common._M(518)) - GameApp.gApp.mWideScreenXOffset + GameApp.gApp.GetScreenRect().mX;
		int y = Common._DS(10);
		mFromHelp = from_help;
		mClip = false;
		mHasTransparencies = (mHasAlpha = true);
		mCutoutImage = new DeviceImage();
		mCutoutImage.SetImageMode(hasTrans: true, hasAlpha: true);
		mCutoutImage.AddImageFlags(16u);
		mCutoutImage.Create(num, num2);
		Graphics graphics = new Graphics(mCutoutImage);
		graphics.Get3D().ClearColorBuffer(new Color(0, 0));
		float num3 = 128f;
		float num4 = num3 / 10f;
		int num5 = 0;
		while (num3 > 0f)
		{
			graphics.SetColor(new Color(0, 0, 0, (int)num3));
			graphics.FillRect(num5, num5, mCutoutImage.mWidth - num5 * 2, 1);
			graphics.FillRect(num5, num5 + 1, 1, mCutoutImage.mHeight - 1 - num5 * 2);
			graphics.FillRect(num5 + 1, mCutoutImage.mHeight - 1 - num5, mCutoutImage.mWidth - 1 - num5 * 2, 1);
			graphics.FillRect(mCutoutImage.mWidth - 1 - num5, num5 + 1, 1, mCutoutImage.mHeight - 2 - num5 * 2);
			num3 -= num4;
			num5++;
		}
		CommonGraphics.SetNonMaskedArea(x, y, num, num2, mMaskedRects, 128);
		mPriority = 2147483646;
		Resize(0, 0, GameApp.gApp.mWidth, GameApp.gApp.mHeight);
		mOKBtn = Common.MakeButton(0, this, from_help ? TextManager.getInstance().getString(483) : TextManager.getInstance().getString(455));
		mOKBtn.SetFont(Res.GetFontByID(ResID.FONT_SHAGLOUNGE28_GREEN));
		AddWidget(mOKBtn);
		int num6 = Common._DS(Common._M(254));
		int theHeight = Common._DS(Common._M(125));
		int theY = Common._DS(Common._M(1000));
		mOKBtn.Resize((GameApp.gApp.mWidth - num6) / 2, theY, num6, theHeight);
		mMultFX = GameApp.gApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_RPI").Duplicate();
		mMultFX.mEmitAfterTimeline = true;
		mDrawScale = new CurvedVal();
		mDrawScale.SetCurve(Common._MP("b+0,2,0.033333,1,####        cY### >P###"));
		mClosing = false;
		FONT_SHAGLOUNGE28_STROKE = Res.GetFontByID(ResID.FONT_SHAGLOUNGE28_STROKE);
		IMAGE_GUI_ARROW_RED = Res.GetImageByID(ResID.IMAGE_GUI_ARROW_RED);
		IMAGE_GUI_BARIMAGE = Res.GetImageByID(ResID.IMAGE_GUI_BARIMAGE);
		IMAGE_GUI_EQUALIMAGE = Res.GetImageByID(ResID.IMAGE_GUI_EQUALIMAGE);
		IMAGE_GUI_BALLIMAGE = Res.GetImageByID(ResID.IMAGE_GUI_BALLIMAGE);
		IMAGE_GUI_DIALOG_MARQUE_BOX = Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_MARQUE_BOX);
		IMAGE_UI_CHALLENGE_GAUGE_EMPTY = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGE_GAUGE_EMPTY);
		IMAGE_UI_CHALLENGE_GAUGE_FILL = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGE_GAUGE_FILL);
	}

	public override void Dispose()
	{
		mMultFX = null;
		RemoveAllWidgets(doDelete: true, recursive: true);
	}

	public override void RemoveAllWidgets(bool doDelete, bool recursive)
	{
		base.RemoveAllWidgets(doDelete, recursive);
		mOKBtn = null;
	}

	public override void Update()
	{
		base.Update();
		if (GameApp.gApp.Is3DAccelerated())
		{
			if (!mDrawScale.HasBeenTriggered())
			{
				MarkDirty();
			}
			if (!mDrawScale.IncInVal())
			{
				_ = (double)mDrawScale;
				_ = 0.0;
			}
			MarkDirty();
			mMultFX.mDrawTransform.LoadIdentity();
			float num = GameApp.DownScaleNum(1f);
			mMultFX.mDrawTransform.Scale(num, num);
			mMultFX.mDrawTransform.Translate(Common._DS(Common._M(988)), Common._DS(Common._M1(470)));
			mMultFX.Update();
			if (mClosing && (double)mDrawScale == 0.0)
			{
				mBoard.ChallengeHelpClosed();
			}
		}
	}

	public override void Draw(Graphics g)
	{
		g?.Get3D();
		int num = GameApp.gApp.mWidth;
		Font fontByID = Res.GetFontByID(ResID.FONT_SHAGEXOTICA38_BLACK_GLOW);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(TextManager.getInstance().getString(410));
		stringBuilder.Append("^d8d8d8^ ");
		stringBuilder.Append(TextManager.getInstance().getString(411));
		stringBuilder.Append("^oldclr^ ");
		stringBuilder.Append(TextManager.getInstance().getString(412));
		stringBuilder.Append("^d8d8d8^ ");
		stringBuilder.Append(TextManager.getInstance().getString(413));
		stringBuilder.Append("^oldclr^ ");
		int num2 = fontByID.StringWidth(TextManager.getInstance().getString(410)) + fontByID.StringWidth(TextManager.getInstance().getString(411)) + fontByID.StringWidth(TextManager.getInstance().getString(412)) + fontByID.StringWidth(TextManager.getInstance().getString(413)) + fontByID.CharWidth(' ') * 3;
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.Append(TextManager.getInstance().getString(414));
		stringBuilder2.Append("^d8d8d8^ ");
		stringBuilder2.Append(TextManager.getInstance().getString(415));
		stringBuilder2.Append("^oldclr^ ");
		stringBuilder2.Append(TextManager.getInstance().getString(416));
		int num3 = fontByID.StringWidth(TextManager.getInstance().getString(414)) + fontByID.StringWidth(TextManager.getInstance().getString(415)) + fontByID.StringWidth(TextManager.getInstance().getString(416)) + fontByID.CharWidth(' ') * 2;
		StringBuilder stringBuilder3 = new StringBuilder();
		stringBuilder3.Append(TextManager.getInstance().getString(417));
		stringBuilder3.Append("^d8d8d8^ ");
		stringBuilder3.Append(TextManager.getInstance().getString(418));
		stringBuilder3.Append("^oldclr^");
		int num4 = fontByID.StringWidth(TextManager.getInstance().getString(417)) + fontByID.StringWidth(TextManager.getInstance().getString(418)) + fontByID.CharWidth(' ');
		int num5 = ((num2 > num3) ? num2 : num3);
		num5 = ((num5 > num4) ? num5 : num4);
		num5 += 40;
		int num6 = ((num5 + 100 < Common._DS(Common._M(1000))) ? Common._DS(Common._M(1000)) : (num5 + 100));
		int height = Common._DS(Common._M(996));
		int x = (num - num6) / 2;
		int num7 = Common._DS(Common._M(170));
		int num8 = ((num5 < Common._DS(Common._M(900))) ? Common._DS(Common._M(900)) : num5);
		int num9 = (num - num8) / 2;
		Common.DrawCommonDialogBacking(g, x, num7, num6, height);
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(new Color(255, 255, 255, 200));
		g.DrawImageBox(new Rect(num9, num7 + Common._DS(Common._M(168)), num8, Common._DS(Common._M1(200))), IMAGE_GUI_DIALOG_MARQUE_BOX);
		g.DrawImageBox(new Rect(num9, num7 + Common._DS(Common._M(390)), num8, Common._DS(Common._M1(290))), IMAGE_GUI_DIALOG_MARQUE_BOX);
		g.DrawImageBox(new Rect(num9, num7 + Common._DS(Common._M(704)), num8, Common._DS(Common._M1(108))), IMAGE_GUI_DIALOG_MARQUE_BOX);
		g.SetColorizeImages(colorizeImages: false);
		g.SetFont(Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_BASE));
		g.SetColor(new Color(205, 151, 57));
		g.WriteString(TextManager.getInstance().getString(409), 0, num7 - g.GetFont().mHeight / 2 + Common._DS(Common._M(190)), GameApp.gApp.mWidth, 0);
		float mTransX = g.mTransX;
		g.mTransX = GameApp.gApp.mBoardOffsetX + 10;
		int num10 = (int)(0f - g.mTransX) + 10;
		int num11 = 4;
		Common._DS(Common._M(382));
		Common._DS(Common._M1(420));
		g.SetFont(fontByID);
		g.SetColor(new Color(205, 151, 57));
		g.WriteWordWrapped(new Rect(num9 + num10, num7 + Common._DS(Common._M(168)) + num11, num8 - num10 * 2, Common._DS(Common._M1(200)) - num11 * 2), stringBuilder.ToString());
		g.DrawImage(IMAGE_GUI_BARIMAGE, Common._DS(Common._M(430)), Common._DS(Common._M1(436)));
		g.DrawImage(IMAGE_GUI_EQUALIMAGE, Common._DS(Common._M(810)), Common._DS(Common._M1(456)));
		g.DrawImage(IMAGE_GUI_BALLIMAGE, Common._DS(Common._M(950)), Common._DS(Common._M1(434)));
		if (mMultFX != null)
		{
			mMultFX.Draw(g);
		}
		Common._DS(Common._M(398));
		Common._DS(Common._M1(638));
		g.SetColor(new Color(205, 151, 57));
		g.WriteWordWrapped(new Rect(num9 + num10, num7 + Common._DS(Common._M(390)) + num11, num8 - num10 * 2, Common._DS(Common._M1(290)) - num11 * 2), stringBuilder2.ToString());
		int num12 = (num - IMAGE_UI_CHALLENGE_GAUGE_EMPTY.mWidth) / 2;
		g.DrawImage(IMAGE_UI_CHALLENGE_GAUGE_EMPTY, num12, Common._DS(Common._M(675)));
		g.DrawImage(IMAGE_UI_CHALLENGE_GAUGE_FILL, num12, Common._DS(Common._M(675)));
		g.SetFont(Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_STROKE));
		g.SetColor(Color.White);
		g.DrawString("2x", num12 + Common._DS(Common._M(105)), Common._DS(Common._M1(800)));
		Common._DS(Common._M(442));
		Common._DS(Common._M1(944));
		g.SetColor(new Color(205, 151, 57));
		g.SetFont(fontByID);
		g.WriteWordWrapped(new Rect(num9 + num10, num7 + Common._DS(Common._M(704)) + num11, num8 - num10 * 2, Common._DS(Common._M1(108)) - num11 * 2), stringBuilder3.ToString());
		g.mTransX = mTransX;
	}

	public virtual void ButtonPress(int id)
	{
		GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BUTTON1));
	}

	public virtual void ButtonDepress(int id)
	{
		mDrawScale.SetCurve(Common._MP("b+0,1,0.05,1,~###         ~#A5t"));
		mWidgetFlagsMod.mRemoveFlags |= 16;
		mClosing = true;
		GameApp.gApp.HideHelp();
	}

	public void PreDraw(Graphics g)
	{
		g.SetDrawMode(1);
		g.DrawImage(mCutoutImage, Common._DS(Common._M(400) - 160), Common._DS(Common._M1(0)));
		g.SetDrawMode(0);
		g.SetColor(new Color(0, 0, 0, 128));
		for (int i = 0; i < mMaskedRects.Count; i++)
		{
			g.FillRect(mMaskedRects[i].r);
		}
		float num = (float)(double)mDrawScale;
		if (num > 1f)
		{
			num = 1f;
		}
		Graphics3D graphics3D = g?.Get3D();
		if ((double)mDrawScale != 1.0 && graphics3D != null)
		{
			SexyTransform2D sexyTransform2D = new SexyTransform2D(init: false);
			sexyTransform2D.Translate(0f - g.mTransX - (float)(mWidth / 2), 0f - g.mTransY - (float)(mHeight / 2));
			sexyTransform2D.Scale((float)(double)mDrawScale, (float)(double)mDrawScale);
			sexyTransform2D.Translate(g.mTransX + (float)(mWidth / 2), g.mTransY + (float)(mHeight / 2));
			graphics3D.PushTransform(sexyTransform2D);
		}
	}

	private void DrawBonusBar(Graphics g)
	{
		float num = (float)(double)mDrawScale;
		if (num > 1f)
		{
			num = 1f;
		}
		float num2 = num * 255f;
		g.SetFont(FONT_SHAGLOUNGE28_STROKE);
		g.SetColor(new Color(255, 0, 0, (int)num2));
		g.DrawString(TextManager.getInstance().getString(419), Common._DS(Common._M(80)) + GameApp.gApp.mBoardOffsetX, (int)((float)Common._DS(Common._M1(160)) + (float)FONT_SHAGLOUNGE28_STROKE.GetHeight() * 0.5f - 10f));
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(new Color(255, 255, 255, (int)num2));
		g.DrawImageRotatedF(IMAGE_GUI_ARROW_RED, Common._DS(Common._M(390) - 160) + GameApp.gApp.mBoardOffsetX, Common._DS(Common._M1(40)), SexyFramework.Common.DegreesToRadians(Common._M2(30)));
		g.SetColorizeImages(colorizeImages: false);
	}

	public override void DrawAll(ModalFlags theFlags, Graphics g)
	{
		PreDraw(g);
		Draw(g);
		if (mOKBtn != null)
		{
			g.Translate(mOKBtn.mX, mOKBtn.mY);
			mOKBtn.Draw(g);
			g.Translate(-mOKBtn.mX, -mOKBtn.mY);
		}
		PostDraw(g);
	}

	public virtual void PostDraw(Graphics g)
	{
		Graphics3D graphics3D = g?.Get3D();
		if ((double)mDrawScale != 1.0)
		{
			graphics3D?.PopTransform();
		}
		DrawBonusBar(g);
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

	public virtual void ButtonPress(int z, int y)
	{
	}
}
