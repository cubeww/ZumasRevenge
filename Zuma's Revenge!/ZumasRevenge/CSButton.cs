using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class CSButton : ButtonWidget
{
	public enum BtnType
	{
		Btn_CS_Back,
		Btn_CS_PrevSet,
		Btn_CS_NextSet,
		Btn_First_Challenge
	}

	private static int last_uc;

	public PIEffect mUnlockSparkles;

	public int mUnlockAlpha;

	public int mLockCel;

	public Image mMedal;

	public string mScoreStr = "";

	public string mLevelStr = "";

	public string mAceStr = "";

	public string mLevelId = "";

	public bool mMouseOver;

	public bool mOpaque = true;

	public int mLevel = -1;

	public ChallengeMenu mChallengeMenu;

	public CSButton(int id, ChallengeMenu theChallengeMenu, ButtonListener listener)
		: base(id, listener)
	{
		mChallengeMenu = theChallengeMenu;
	}

	public override void Dispose()
	{
		if (mUnlockSparkles != null)
		{
			mUnlockSparkles.Dispose();
			mUnlockSparkles = null;
		}
	}

	public override void Draw(Graphics g)
	{
		if (g.mClipRect.mWidth <= 0 || g.mClipRect.mHeight <= 0)
		{
			return;
		}
		last_uc = mUpdateCnt;
		bool flag = mIsDown && mIsOver && !mDisabled;
		flag ^= mInverted;
		bool flag2 = mId - 3 + 1 == GameApp.gLastLevel && mChallengeMenu.mCrownZoomType >= 0;
		int num = (flag ? Common._DS(Common._M(0)) : 0);
		int num2 = (flag ? Common._DS(Common._M(0)) : 0);
		Image image = null;
		if (mLevel != -1)
		{
			image = GameApp.gApp.GetLevelThumbnail(mLevel);
		}
		if (image != null)
		{
			g.DrawImage(image, GlobalChallenge.gScreenShake + num, GlobalChallenge.gScreenShake + num2, Common._DS(GlobalChallenge.CS_BTN_WIDTH), Common._DS(GlobalChallenge.CS_BTN_HEIGHT));
			if (mMouseOver)
			{
				g.PushState();
				g.SetColor(new Color(255, 255, 255, Common._M(100)));
				g.SetColorizeImages(colorizeImages: true);
				g.SetDrawMode(1);
				g.DrawImage(image, GlobalChallenge.gScreenShake + num, GlobalChallenge.gScreenShake + num2, Common._DS(GlobalChallenge.CS_BTN_WIDTH), Common._DS(GlobalChallenge.CS_BTN_HEIGHT));
				g.PopState();
			}
			if (flag)
			{
				g.DrawImage(Res.GetImageByID(ResID.IMAGE_UI_MAIN_MENU_CH_THUMBNAILOVERLAY), GlobalChallenge.gScreenShake, GlobalChallenge.gScreenShake, Common._DS(GlobalChallenge.CS_BTN_WIDTH + Common._M(0)), Common._DS(GlobalChallenge.CS_BTN_HEIGHT + Common._M1(0)));
			}
		}
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_MAIN_MENU_CS_LOCK_ANIMATION);
		if (mOpaque)
		{
			g.SetColor(new Color(0, 0, 0, Common._M(191)));
			g.FillRect(0, 0, Common._DS(GlobalChallenge.CS_BTN_WIDTH), Common._DS(GlobalChallenge.CS_BTN_HEIGHT));
		}
		else if (mMedal == imageByID)
		{
			g.SetColor(new Color(0, 0, 0, 120));
			g.FillRect(0, 0, Common._DS(GlobalChallenge.CS_BTN_WIDTH), Common._DS(GlobalChallenge.CS_BTN_HEIGHT));
		}
		Common.DrawCommonDialogBorder(g, GlobalChallenge.gScreenShake - Common._DS(15), GlobalChallenge.gScreenShake - Common._DS(15), mWidth + Common._DS(30), mHeight + Common._DS(30));
		if (mUnlockAlpha > 0)
		{
			Image image2 = imageByID;
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(new Color(255, 255, 255, mUnlockAlpha));
			g.DrawImageCel(image2, (mWidth - image2.GetCelWidth()) / 2 + GlobalChallenge.gScreenShake, (mHeight - image2.GetCelHeight()) / 2 + GlobalChallenge.gScreenShake, mLockCel);
			g.SetColorizeImages(colorizeImages: false);
		}
		if (mMedal != null)
		{
			if (!flag2 || !g.Is3D())
			{
				if (mMedal == imageByID)
				{
					g.DrawImageCel(mMedal, (mWidth - mMedal.GetCelWidth()) / 2 + GlobalChallenge.gScreenShake + Common._DS(10), (mHeight - mMedal.GetCelHeight()) / 2 + GlobalChallenge.gScreenShake, 0);
				}
				else
				{
					g.DrawImageCel(mMedal, (mWidth - mMedal.GetCelWidth()) / 2 + GlobalChallenge.gScreenShake, (mHeight - mMedal.GetCelHeight()) / 2 + GlobalChallenge.gScreenShake, 0);
				}
			}
			else if (mMedal != null)
			{
				g.PushState();
				g.ClearClipRect();
				g.Translate(-mX, -mY);
				Image imageByID2 = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_LARGE_CROWN);
				if (mChallengeMenu.mCrownZoomType == 1)
				{
					g.DrawImage(imageByID2, mX + (mWidth - imageByID2.mWidth) / 2, mY + (mHeight - imageByID2.mHeight) / 2);
					imageByID2 = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_LARGE_ACECROWN);
				}
				g.SetColor(new Color(255, 255, 255, (int)mChallengeMenu.mCrownAlpha));
				g.SetColorizeImages(colorizeImages: true);
				SexyTransform2D sexyTransform2D = new SexyTransform2D(init: false);
				sexyTransform2D.Scale(mChallengeMenu.mCrownSize, mChallengeMenu.mCrownSize);
				sexyTransform2D.Translate((float)mX + ((float)mWidth - (float)imageByID2.mWidth * mChallengeMenu.mCrownSize) / 2f, (float)mY + ((float)mHeight - (float)imageByID2.mHeight * mChallengeMenu.mCrownSize) / 2f);
				g.DrawImageMatrix(imageByID2, sexyTransform2D, (float)imageByID2.mWidth * mChallengeMenu.mCrownSize / 2f, (float)imageByID2.mHeight * mChallengeMenu.mCrownSize / 2f);
				g.PopState();
			}
		}
		if (!flag2 && mUnlockSparkles != null)
		{
			g.Is3D();
		}
	}

	public override void Update()
	{
		mUpdateCnt++;
		bool flag = mChallengeMenu.mCrownZoomType >= 0;
		if (mUnlockSparkles != null && !flag)
		{
			mUnlockSparkles.Update();
			MarkDirty();
			if (mUnlockSparkles.mCurNumParticles == 0 && mUnlockSparkles.mFrameNum > 10f)
			{
				mUnlockSparkles.Dispose();
				mUnlockSparkles = null;
			}
		}
		if (!flag)
		{
			if (mLockCel < Res.GetImageByID(ResID.IMAGE_UI_MAIN_MENU_CS_LOCK_ANIMATION).mNumCols - 1 && mUpdateCnt % Common._M(8) == 0)
			{
				mLockCel++;
			}
			else if (mUnlockAlpha > 0)
			{
				mUnlockAlpha -= Common._M(2);
			}
		}
	}

	public override void MouseEnter()
	{
	}

	public override void MouseLeave()
	{
	}

	public void PreLoadImage()
	{
		if (mLevel != -1)
		{
			GameApp.gApp.GetLevelThumbnail(mLevel);
		}
	}
}
