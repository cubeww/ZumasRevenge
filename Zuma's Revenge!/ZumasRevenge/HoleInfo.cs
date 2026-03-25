using System;
using System.Collections.Generic;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class HoleInfo
{
	public float mPercentOpen;

	public float mPercentTarget = -1f;

	public int mUpdateCount;

	public int mX;

	public int mY;

	public int mFrame;

	public float mRotation;

	public bool mVisible = true;

	public int mCurveNum = -1;

	public CurveMgr mCurve;

	public FireRing[] mRing = new FireRing[3];

	public float mDeathAlpha;

	public bool mDoDeathFade;

	public List<int> mShared = new List<int>();

	public void DrawRings(Graphics g)
	{
		if (mDeathAlpha >= 255f || (!mDoDeathFade && mDeathAlpha != 0f))
		{
			return;
		}
		float num = Common._S(96);
		float num2 = num / 2f;
		float num3 = num / 2f;
		g.SetDrawMode(1);
		g.SetColorizeImages(colorizeImages: true);
		Image imageByID = Res.GetImageByID(ResID.IMAGE_INFERNO_RING);
		int num4 = (int)((num - (float)imageByID.GetCelWidth()) / 2f - (float)Common._S(Common._M(0)));
		int num5 = (int)((num - (float)imageByID.GetCelHeight()) / 2f - (float)Common._S(Common._M(1)));
		for (int i = 0; i < 3; i++)
		{
			FireRing fireRing = mRing[i];
			if (fireRing.mCel != -1 && fireRing.mAlpha != 0f)
			{
				int num6 = (int)fireRing.mAlpha;
				if (mDoDeathFade)
				{
					num6 = (int)Math.Min(num6, 255f - mDeathAlpha);
				}
				g.SetColor(255, 255, 255, num6);
				Rect celRect = imageByID.GetCelRect(fireRing.mCel);
				if (g.Is3D())
				{
					g.DrawImageRotatedF(imageByID, Common._S(mX) + num4, Common._S(mY) + num5, mRotation + 3.14159f, num2 - (float)num4, num3 - (float)num5, celRect);
				}
				else
				{
					g.DrawImageRotated(imageByID, Common._S(mX) + num4, Common._S(mY) + num5, mRotation + 3.14159f, (int)(num2 - (float)num4), (int)(num3 - (float)num5), celRect);
				}
			}
		}
		g.SetColorizeImages(colorizeImages: false);
		g.SetDrawMode(0);
	}

	public void DrawMain(Graphics g, bool is_gray)
	{
		Image[] array = new Image[4]
		{
			is_gray ? Res.GetImageByID(ResID.IMAGE_HOLE_BASE_GRAY) : Res.GetImageByID(ResID.IMAGE_HOLE_BASE),
			is_gray ? Res.GetImageByID(ResID.IMAGE_HOLE_GRAY) : Res.GetImageByID(ResID.IMAGE_HOLE),
			is_gray ? Res.GetImageByID(ResID.IMAGE_HOLE_HEAD_GRAY) : Res.GetImageByID(ResID.IMAGE_HOLE_HEAD),
			is_gray ? Res.GetImageByID(ResID.IMAGE_HOLE_JAW_GRAY) : Res.GetImageByID(ResID.IMAGE_HOLE_JAW)
		};
		g.PushState();
		if (mDeathAlpha > 0f)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, is_gray ? ((int)mDeathAlpha) : ((int)Math.Max(0f, 255f - mDeathAlpha)));
		}
		float num = (float)array[0].mWidth / 2f;
		float num2 = (float)array[0].mHeight / 2f;
		g.PushState();
		g.SetClipRect(Common._S(mX) + Common._S(Common._M(19)), Common._S(mY) + Common._S(Common._M1(15)), array[0].mWidth - Common._S(Common._M2(38)), array[0].mHeight - Common._S(Common._M3(33)));
		float num3 = mRotation + 3.14159f;
		float num4 = Common._S(Common._M(7)) + GameApp.gScreenShakeX;
		float num5 = Common._S(Common._M(7)) + GameApp.gScreenShakeY;
		if (g.Is3D())
		{
			g.DrawImageRotatedF(array[1], (float)Common._S(mX) + num4, (float)Common._S(mY) + num5, num3, num - num4, num2 - num5);
		}
		else
		{
			g.DrawImageRotated(array[1], (int)((float)Common._S(mX) + num4), (int)((float)Common._S(mY) + num5), num3, (int)(num - num4), (int)(num2 - num5));
		}
		float num6 = (float)Common._S(Common._M(17)) - mPercentOpen * Common._S(Common._M1(30f)) + (float)GameApp.gScreenShakeX;
		float num7 = Common._S(Common._M(17)) + GameApp.gScreenShakeY;
		if (g.Is3D())
		{
			g.DrawImageRotatedF(array[2], (float)Common._S(mX) + num7, (float)Common._S(mY) + num6, num3, num - num7, num2 - num6);
		}
		else
		{
			g.DrawImageRotated(array[2], (int)((float)Common._S(mX) + num7), (int)((float)Common._S(mY) + num6), num3, (int)(num - num7), (int)(num2 - num6));
		}
		num6 = (float)Common._S(Common._M(48)) + mPercentOpen * Common._S(Common._M1(6f)) + (float)GameApp.gScreenShakeX;
		num7 = Common._S(Common._M(17)) + GameApp.gScreenShakeY;
		if (g.Is3D())
		{
			g.DrawImageRotatedF(array[3], (float)Common._S(mX) + num7, (float)Common._S(mY) + num6, num3, num - num7, num2 - num6);
		}
		else
		{
			g.DrawImageRotated(array[3], (int)((float)Common._S(mX) + num7), (int)((float)Common._S(mY) + num6), num3, (int)(num - num7), (int)(num2 - num6));
		}
		g.PopState();
		g.DrawImageRotated(array[0], Common._S(mX) + GameApp.gScreenShakeX, Common._S(mY) + GameApp.gScreenShakeY, num3);
		g.PopState();
		if (mVisible)
		{
			DrawRings(g);
		}
	}

	public HoleInfo()
	{
		mFrame = 0;
		mVisible = true;
		mPercentOpen = 0f;
		mPercentTarget = -1f;
		mUpdateCount = 0;
		mCurveNum = -1;
		mDoDeathFade = false;
		mDeathAlpha = 0f;
		mRing[2].mCel = -1;
		mRing[1].mCel = -1;
	}

	public HoleInfo(HoleInfo rhs)
	{
		mFrame = 0;
		mVisible = true;
		mPercentOpen = 0f;
		mPercentTarget = -1f;
		mUpdateCount = 0;
		mCurveNum = -1;
		mDoDeathFade = false;
		mDeathAlpha = 0f;
		mRing[2].mCel = -1;
		mRing[1].mCel = -1;
		if (rhs != null)
		{
			mPercentOpen = rhs.mPercentOpen;
			mPercentTarget = rhs.mPercentTarget;
			mUpdateCount = rhs.mUpdateCount;
			mX = rhs.mX;
			mY = rhs.mY;
			mFrame = rhs.mFrame;
			mRotation = rhs.mRotation;
			mVisible = rhs.mVisible;
			mCurveNum = rhs.mCurveNum;
			mCurve = rhs.mCurve;
			ref FireRing reference = ref mRing[0];
			reference = rhs.mRing[0];
			ref FireRing reference2 = ref mRing[1];
			reference2 = rhs.mRing[1];
			ref FireRing reference3 = ref mRing[2];
			reference3 = rhs.mRing[2];
			mDeathAlpha = rhs.mDeathAlpha;
			mDoDeathFade = rhs.mDoDeathFade;
			mShared.AddRange(rhs.mShared.ToArray());
		}
	}

	public void Update()
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_INFERNO_RING);
		if (mDoDeathFade)
		{
			mDeathAlpha += Common._M(2f);
			if (mDeathAlpha > 255f)
			{
				mDeathAlpha = 255f;
			}
		}
		else if (mDeathAlpha > 0f)
		{
			mDeathAlpha -= Common._M(1f);
			if (mDeathAlpha < 0f)
			{
				mDeathAlpha = 0f;
			}
		}
		mUpdateCount++;
		if (mPercentOpen > mPercentTarget && mPercentTarget >= 0f)
		{
			mPercentOpen -= Common._M(0.01f);
			if (mPercentOpen < mPercentTarget)
			{
				mPercentOpen = mPercentTarget;
				mPercentTarget = -1f;
			}
		}
		if (mPercentOpen > 0f && mVisible)
		{
			int num = imageByID.mNumRows * imageByID.mNumCols;
			int num2 = 7 - (int)(mPercentOpen / 20f);
			if (num2 < 3)
			{
				num2 = 3;
			}
			else if (num2 > 7)
			{
				num2 = 7;
			}
			float num3 = 255f / (24f * (float)num * 2f);
			if (mRing[0].mCel != -1)
			{
				mRing[0].mAlpha += num3;
				if (mRing[0].mAlpha >= 255f)
				{
					mRing[0].mAlpha = 255f;
				}
				if (mUpdateCount % num2 == 0 && ++mRing[0].mCel >= num)
				{
					mRing[0].mCel = -1;
				}
			}
			float num4 = 128f / ((float)num2 * Common._M(80f));
			if (mRing[1].mAlpha == 0f && mRing[0].mCel == 4)
			{
				mRing[1].mCel = 0;
				mRing[1].mAlpha = 128f;
			}
			else if (mRing[1].mCel != -1)
			{
				mRing[1].mAlpha += num4;
				if (mRing[1].mAlpha >= 255f)
				{
					mRing[1].mAlpha = 255f;
				}
				if (mUpdateCount % num2 == 0 && ++mRing[1].mCel >= num)
				{
					mRing[1].mCel = 0;
					mRing[1].mAlpha = 128f;
				}
			}
			if (mRing[2].mAlpha == 0f && mRing[1].mCel == num / 2)
			{
				mRing[2].mCel = 0;
				mRing[2].mAlpha = 128f;
			}
			else if (mRing[2].mCel != -1)
			{
				mRing[2].mAlpha += num4;
				if (mRing[2].mAlpha >= 255f)
				{
					mRing[2].mAlpha = 255f;
				}
				if (mUpdateCount % num2 == 0 && ++mRing[2].mCel >= num)
				{
					mRing[2].mCel = 0;
					mRing[2].mAlpha = 128f;
				}
			}
		}
		else
		{
			if (!mVisible)
			{
				return;
			}
			bool flag = true;
			int num5 = 7 - (int)(mPercentOpen / 20f);
			if (num5 < 3)
			{
				num5 = 3;
			}
			else if (num5 > 7)
			{
				num5 = 7;
			}
			for (int i = 0; i < 3; i++)
			{
				if (mRing[i].mAlpha > 0f)
				{
					if (mUpdateCount % num5 == 0 && ++mRing[i].mCel >= imageByID.mNumRows * imageByID.mNumCols)
					{
						mRing[i].mCel = 0;
					}
					mRing[i].mAlpha -= Common._M(2);
					if (mRing[i].mAlpha <= 0f)
					{
						mRing[i].mAlpha = 0f;
					}
					else
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				for (int j = 0; j < 3; j++)
				{
					mRing[j].mCel = ((j != 0) ? (-1) : 0);
					mRing[j].mAlpha = 0f;
				}
			}
		}
	}

	public void Draw(Graphics g, float hilite_override)
	{
		DrawMain(g, is_gray: false);
		if (mDeathAlpha > 0f)
		{
			DrawMain(g, is_gray: true);
		}
		float num = ((hilite_override != 0f || mCurve == null) ? hilite_override : mCurve.mSkullHilite);
		if (mCurve != null && mDeathAlpha <= 0f && mCurve.mInitialPathHilite && num != 0f)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)num);
			g.SetDrawMode(1);
			DrawMain(g, is_gray: false);
			g.SetColorizeImages(colorizeImages: false);
			g.SetDrawMode(0);
		}
	}

	public void Draw(Graphics g)
	{
		Draw(g, 0f);
	}

	public void SetPctOpen(float pct)
	{
		if (pct < mPercentOpen && mVisible)
		{
			mPercentTarget = pct;
			return;
		}
		mPercentOpen = pct;
		mPercentTarget = -1f;
	}

	public float GetPctOpen()
	{
		return mPercentOpen;
	}
}
