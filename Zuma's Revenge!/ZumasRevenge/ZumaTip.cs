using System;
using System.Collections.Generic;
using JeffLib;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class ZumaTip
{
	public enum Dir
	{
		Left,
		Right,
		Up,
		Down
	}

	public static readonly int MAX_ALPHA = 128;

	private static readonly int NUM_LINES = 10;

	protected List<MaskedRect> mMaskedRects = new List<MaskedRect>();

	protected MemoryImage mCutoutImage;

	protected Image mMaskImage;

	protected string mText = "";

	protected Rect mBoxRect = default(Rect);

	protected float mArrowAngle;

	protected int mArrowX;

	protected int mArrowY;

	protected int mTextHeight;

	protected int mWidth;

	protected int mHeight;

	protected int mCutoutX;

	protected int mCutoutY;

	protected int mCutoutW;

	protected int mCutoutH;

	protected float mArrowAlpha;

	protected int mArrowAlphaDir = 1;

	protected float mArrowYOff;

	protected int mArrowYOffDir = 1;

	public bool mDoArrowAnim;

	public bool mBlockUpdates = true;

	public bool mClickDismiss = true;

	public bool mDrawArrow = true;

	public int mId;

	public int mUpdateCount;

	public int mAppearDelay;

	public ZumaTip(string text, int width, int height, Rect cutout_region, int id)
	{
		mCutoutX = cutout_region.mX;
		mCutoutY = cutout_region.mY;
		mCutoutW = cutout_region.mWidth;
		mCutoutH = cutout_region.mHeight;
		mText = text;
		mId = id;
		mWidth = width + Common._DS(100);
		mHeight = height + Common._DS(20);
		if (mCutoutX < 0 && id != ZumaProfile.FRUIT_HINT)
		{
			mCutoutX = 0;
		}
		if (id != ZumaProfile.CHALLENGE_HINT)
		{
			if (id == ZumaProfile.FIRST_SHOT_HINT)
			{
				mMaskImage = Res.GetImageByID(ResID.IMAGE_UI_CONE);
				mCutoutW = mMaskImage.mWidth * 4;
				mCutoutH = mMaskImage.mHeight * 4;
			}
			else if (id == ZumaProfile.ZUMA_BAR_HINT)
			{
				SetZumaBarBoundingBox();
				CreateCutoutImage();
			}
			else
			{
				mMaskImage = Res.GetImageByID(ResID.IMAGE_UI_CIRCLE);
			}
		}
		int theMaxWidth = 0;
		Graphics graphics = new Graphics();
		graphics.SetFont(Res.GetFontByID(ResID.FONT_MAIN22));
		mTextHeight = graphics.GetWordWrappedHeight(mWidth - Common._DS(100), mText, -1, ref theMaxWidth, ref theMaxWidth);
		CommonGraphics.SetNonMaskedArea(mCutoutX, mCutoutY, mCutoutW, mCutoutH, mMaskedRects, MAX_ALPHA);
		if (mMaskedRects.Count == 4)
		{
			mMaskedRects[0].r.mX = -GameApp.gApp.mBoardOffsetX;
			mMaskedRects[0].r.mWidth += GameApp.gApp.mBoardOffsetX;
		}
		else if (mMaskedRects.Count == 3)
		{
			mMaskedRects.Add(new MaskedRect(new Rect(-GameApp.gApp.mBoardOffsetX, 0, GameApp.gApp.mBoardOffsetX, GlobalMembers.gSexyApp.mScreenBounds.mHeight), MAX_ALPHA));
		}
	}

	public virtual void Dispose()
	{
		mCutoutImage = null;
	}

	public void PointAt(int x, int y, int dir)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_ARROW);
		int num = Common._DS(Common._M(175));
		switch (dir)
		{
		case 0:
			mArrowAngle = (float)Math.PI;
			mArrowX = x + Common._DS(Common._M(24));
			mArrowY = y - imageByID.mHeight / 2;
			mBoxRect = new Rect(x + num, y - mHeight / 2, mWidth, mHeight);
			if (mBoxRect.mY < 0)
			{
				mBoxRect.mY = 0;
			}
			else if (mBoxRect.mY + mBoxRect.mHeight > GlobalMembers.gSexyApp.mHeight)
			{
				mBoxRect.mY = GlobalMembers.gSexyApp.mHeight - mBoxRect.mHeight;
			}
			break;
		case 1:
			mArrowAngle = 0f;
			mArrowX = x - imageByID.mWidth - Common._DS(Common._M(24));
			mArrowY = y - imageByID.mHeight / 2;
			mBoxRect = new Rect(x - num - mWidth, y - mHeight / 2, mWidth, mHeight);
			if (mBoxRect.mY < 0)
			{
				mBoxRect.mY = 0;
			}
			else if (mBoxRect.mY + mBoxRect.mHeight > GlobalMembers.gSexyApp.mHeight)
			{
				mBoxRect.mY = GlobalMembers.gSexyApp.mHeight - mBoxRect.mHeight;
			}
			break;
		case 2:
			mArrowAngle = (float)Math.PI / 2f;
			mArrowX = x - imageByID.mWidth / 2;
			mArrowY = y + Common._DS(Common._M(48));
			mBoxRect = new Rect(x - mWidth / 2, y + num, mWidth, mHeight);
			if (mBoxRect.mX < 0)
			{
				mBoxRect.mX = 0;
			}
			else if (mBoxRect.mX + mBoxRect.mWidth > GlobalMembers.gSexyApp.mWidth)
			{
				mBoxRect.mX = GlobalMembers.gSexyApp.mWidth - mBoxRect.mWidth;
			}
			break;
		case 3:
			mArrowAngle = -(float)Math.PI / 2f;
			mArrowX = x - imageByID.mWidth / 2;
			mArrowY = y - imageByID.mHeight - Common._DS(Common._M(46));
			mBoxRect = new Rect(x - mWidth / 2, y - num - mHeight, mWidth, mHeight);
			if (mBoxRect.mX < 0)
			{
				mBoxRect.mX = 0;
			}
			else if (mBoxRect.mX + mBoxRect.mWidth > GlobalMembers.gSexyApp.mWidth)
			{
				mBoxRect.mX = GlobalMembers.gSexyApp.mWidth - mBoxRect.mWidth;
			}
			break;
		}
	}

	public void AutoPointAt(int x, int y, int region_w, int region_h)
	{
		int num = GlobalMembers.gSexyApp.mWidth - (x + region_w);
		int num2 = GlobalMembers.gSexyApp.mHeight - (y + region_h);
		int[] array = new int[4] { num, x, num2, y };
		int num3 = 0;
		for (int i = 1; i < 4; i++)
		{
			if (array[i] > array[num3])
			{
				num3 = i;
			}
		}
		switch (num3)
		{
		case 0:
			PointAt(x + region_w, y + region_h / 2, num3);
			break;
		case 1:
			PointAt(x, y + region_h / 2, num3);
			break;
		case 2:
			PointAt(x + region_w / 2, y + region_h, num3);
			break;
		case 3:
			PointAt(x + region_w / 2, y, num3);
			break;
		}
	}

	public void AutoPointAtCutoutRegion()
	{
		AutoPointAt(mCutoutX, mCutoutY, mCutoutW, mCutoutH);
	}

	public void Draw(Graphics g)
	{
		if (mUpdateCount < mAppearDelay)
		{
			return;
		}
		if (mCutoutImage != null)
		{
			g.DrawImage(mCutoutImage, mCutoutX, mCutoutY);
		}
		else if (mMaskImage != null)
		{
			g.DrawImage(mMaskImage, mCutoutX, mCutoutY, mCutoutW, mCutoutH);
		}
		if (mMaskImage != null || mCutoutImage != null)
		{
			if (mCutoutX >= 0)
			{
				Common._S(80);
			}
			else
			{
				Common._S(80);
			}
			g.SetColor(0, 0, 0, MAX_ALPHA);
			for (int i = 0; i < mMaskedRects.size(); i++)
			{
				g.FillRect(mMaskedRects[i].r);
			}
		}
		Common.DrawCommonDialogBacking(g, mBoxRect.mX, mBoxRect.mY, mBoxRect.mWidth, mBoxRect.mHeight);
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_ARROW);
		if (mDrawArrow)
		{
			g.DrawImageRotated(imageByID, mArrowX, (int)((float)mArrowY + mArrowYOff), mArrowAngle);
			if (mDoArrowAnim)
			{
				g.PushState();
				g.SetColorizeImages(colorizeImages: true);
				g.SetDrawMode(1);
				g.SetColor(255, 255, 255, (int)mArrowAlpha);
				g.DrawImageRotated(imageByID, mArrowX, (int)((float)mArrowY + mArrowYOff), mArrowAngle);
				g.PopState();
				if (mId == ZumaProfile.FIRST_SHOT_HINT)
				{
					g.SetFont(Res.GetFontByID(ResID.FONT_SHAGLOUNGE45_GAUNTLET));
					g.SetColor(255, 253, 99);
					g.DrawString(TextManager.getInstance().getString(824), Common._DS(Common._M(140)), Common._DS(Common._M1(540)));
				}
			}
		}
		g.SetColor(255, 220, 135);
		g.SetFont(Res.GetFontByID(ResID.FONT_MAIN22));
		int value = Common._M(50);
		int value2 = Common._M(0);
		value = Common._DS(value);
		value2 = Common._DS(value2);
		Rect theRect = new Rect(mBoxRect.mX + value, mBoxRect.mY + value2, mBoxRect.mWidth - value * 2, mBoxRect.mHeight - value2 * 2);
		theRect.mY += (theRect.mHeight - mTextHeight) / 2;
		g.WriteWordWrapped(theRect, mText, -1, 0);
	}

	public void Update()
	{
		mUpdateCount++;
		if (mDoArrowAnim)
		{
			float num = Common._M(10.5f);
			float num2 = Common._M(0.5f);
			float num3 = Common._M(10);
			mArrowAlpha += num * (float)mArrowAlphaDir;
			if (mArrowAlpha >= 255f && mArrowAlphaDir == 1)
			{
				mArrowAlpha = 255f;
				mArrowAlphaDir = -1;
			}
			else if (mArrowAlpha <= 0f && mArrowAlphaDir == -1)
			{
				mArrowAlphaDir = 1;
				mArrowAlpha = 0f;
			}
			mArrowYOff += num2 * (float)mArrowYOffDir;
			if (mArrowYOff >= num3 && mArrowYOffDir == 1)
			{
				mArrowYOff = num3;
				mArrowYOffDir = -1;
			}
			else if (mArrowYOff <= 0f && mArrowYOffDir == -1)
			{
				mArrowYOff = 0f;
				mArrowYOffDir = 1;
			}
		}
	}

	public bool CutoutContainsPoint(int x, int y)
	{
		return new Rect(mCutoutX, mCutoutY, mCutoutW, mCutoutH).Contains(x, y);
	}

	private void SetZumaBarBoundingBox()
	{
		GameApp gApp = GameApp.gApp;
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_WOOD);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_RIGHT_MOUTH_UPPER);
		Image imageByID3 = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_LEFT_MOUTH_LOWER);
		int num = ((!gApp.IsWideScreen()) ? ((int)((float)imageByID.mWidth * 0.05f)) : 0);
		int wideScreenAdjusted = gApp.GetWideScreenAdjusted(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GUI_INGAME_UI_LEFT_MOUTH_LOWER)) + num);
		int wideScreenAdjusted2 = gApp.GetWideScreenAdjusted(Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GUI_INGAME_UI_RIGHT_MOUTH_UPPER)) - num);
		mCutoutX = wideScreenAdjusted + Common._DS(25);
		mCutoutY = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_GUI_INGAME_UI_LEFT_MOUTH_LOWER));
		mCutoutW = wideScreenAdjusted2 - wideScreenAdjusted + imageByID2.mWidth - Common._DS(50);
		mCutoutH = imageByID3.mHeight;
	}

	private void CreateCutoutImage()
	{
		mCutoutImage = new DeviceImage();
		mCutoutImage.mApp = GameApp.gApp;
		mCutoutImage.SetImageMode(hasTrans: true, hasAlpha: true);
		mCutoutImage.AddImageFlags(16u);
		mCutoutImage.Create(mCutoutW, mCutoutH);
		Graphics graphics = new Graphics(mCutoutImage);
		graphics.Get3D().ClearColorBuffer(new Color(0, 0));
		float num = MAX_ALPHA;
		float num2 = num / (float)NUM_LINES;
		int num3 = 0;
		while (num > 0f)
		{
			graphics.SetColor(0, 0, 0, (int)num);
			graphics.FillRect(num3, num3, mCutoutW - num3 * 2, 1);
			graphics.FillRect(num3, num3 + 1, 1, mCutoutH - 1 - num3 * 2);
			graphics.FillRect(num3 + 1, mCutoutH - 1 - num3, mCutoutW - 1 - num3 * 2, 1);
			graphics.FillRect(mCutoutW - 1 - num3, num3 + 1, 1, mCutoutH - 2 - num3 * 2);
			num -= num2;
			num3++;
		}
		graphics.ClearRenderContext();
	}
}
