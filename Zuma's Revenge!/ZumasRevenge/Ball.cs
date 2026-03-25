using System;
using System.Collections.Generic;
using System.Linq;
using JeffLib;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class Ball
{
	protected struct Particle
	{
		public float x;

		public float y;

		public float vx;

		public float vy;

		public int mSize;
	}

	protected static int mIdGen = 0;

	protected static int mLaserAnimCel;

	protected Transform mGlobalTransform = new Transform();

	public static Color[] gOverlayColors = new Color[6]
	{
		new Color(0, 0, 255),
		new Color(255, 255, 0),
		new Color(255, 0, 0),
		new Color(0, 255, 0),
		new Color(255, 0, 255),
		new Color(255, 255, 255)
	};

	public static string[] fx_files = new string[6] { "PIEFFECT_NONRESIZE_BPI", "PIEFFECT_NONRESIZE_YPI", "PIEFFECT_NONRESIZE_RPI", "PIEFFECT_NONRESIZE_GPI", "PIEFFECT_NONRESIZE_PPI", "PIEFFECT_NONRESIZE_WPI" };

	private static BlendedImage[] gBlendedBalls = new BlendedImage[8];

	private static BlendedImage[,] gBlendedPowerups = new BlendedImage[15, 8];

	private static BlendedImage[] gBlendedPowerupLights = new BlendedImage[14];

	private static BlendedImage[] gBlendedBombLights = new BlendedImage[6];

	protected bool mInTunnel;

	protected int mMultOverlayAlpha;

	protected PIEffect mMultFX;

	protected int mId;

	protected int mColorType;

	protected int mDisplayType;

	protected float mWayPoint;

	protected float mLastWayPoint;

	protected float mRotation;

	protected float mDestRotation;

	protected float mRotationInc;

	protected float mX;

	protected float mY;

	protected float mLastX;

	protected float mLastY;

	protected float mDrawScale;

	protected float mRadius;

	protected int mPulseState;

	protected int mPulseTimer;

	private List<Component> mOverlayPulse = new List<Component>();

	private List<Component> mElectricOverlay = new List<Component>();

	private ElectricExplodeOverlay mElectricExplodeOverlay = new ElectricExplodeOverlay();

	protected int mElectricOverlayCel;

	protected List<Ball> mList;

	protected CurveMgr mCurve;

	protected bool mCollidesWithNext;

	protected bool mNeedCheckCollision;

	protected bool mSuckPending;

	protected bool mShrinkClear;

	protected bool mSuckFromCompacting;

	protected bool mExplodingInTunnel;

	protected bool mExploding;

	protected bool mExplodingFromLightning;

	protected int mExplodeFrame;

	protected bool mShouldRemove;

	protected bool mSpeedy;

	protected bool mSuckBack;

	protected int mPowerGracePeriod;

	protected PowerType mLastPowerType;

	protected int mCannonFrame;

	protected bool mIsCannon;

	protected bool mDoLaserAnim;

	protected int mUpdateCount;

	protected int mCel;

	public Bullet mBullet;

	protected int mSuckCount;

	protected int mBackwardsCount;

	protected float mBackwardsSpeed;

	protected int mComboCount;

	protected int mComboScore;

	protected int mStartFrame;

	protected int mPowerCount;

	protected int mPowerFade;

	protected ushort mGapBonus;

	protected ushort mNumGaps;

	protected float mIconAppearScale;

	protected float mIconScaleRate;

	protected int mIconCel;

	protected int mMultBallCel;

	protected int mMultBallCel2;

	protected List<Particle> mParticles;

	protected PowerType mPowerType;

	protected PowerType mDestPowerType;

	public bool mHilightPulse;

	public bool mDebugDrawID;

	public bool mDoBossPulse;

	public int mBossBlinkTimer;

	public int mLastFrame;

	public Gun mFrog;

	public void DrawStandardPower(Graphics g, int img_id, int cel, int thePowerType)
	{
		GameApp gApp = GameApp.gApp;
		ResID id = (ResID)(img_id + mColorType);
		if (gApp.mColorblind && mColorType == 3)
		{
			id = ResID.IMAGE_POWERUPS_GREEN_CBM;
		}
		else if (gApp.mColorblind && mColorType == 4)
		{
			id = ResID.IMAGE_POWERUPS_PURPLE_CBM;
		}
		else if (gApp.mColorblind && mColorType == 5)
		{
			id = ResID.IMAGE_POWERUPS_WHITE_CBM;
		}
		Image imageByID = Res.GetImageByID(id);
		Image image = null;
		image = ((mPowerType == PowerType.PowerType_MoveBackwards) ? Res.GetImageByID(ResID.IMAGE_POWERUP_REVERSE_ANYCOLOR) : ((mPowerType != PowerType.PowerType_Laser) ? Res.GetImageByID(ResID.IMAGE_POWERUPS_PULSES) : Res.GetImageByID(ResID.IMAGE_POWERUP_LAZER_ANYCOLOR)));
		float num = Common._S(mX) - (float)(imageByID.GetCelWidth() / 2);
		float num2 = Common._S(mY) - (float)(imageByID.GetCelHeight() / 2);
		float num3 = ((mPowerType == PowerType.PowerType_MoveBackwards) ? 1.570795f : (-1.570795f));
		bool flag = gApp.Is3DAccelerated();
		if (flag)
		{
			Rect celRect = imageByID.GetCelRect(cel);
			g.DrawImageRotatedF(imageByID, (int)num, (int)num2, mRotation + num3, celRect);
		}
		else
		{
			BlendedImage blendedImage = CreateBlendedPowerup(thePowerType, mColorType, imageByID, cel);
			blendedImage.Draw(g, num, num2);
		}
		if (mPowerType == PowerType.PowerType_MoveBackwards || mPowerType == PowerType.PowerType_Laser)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetDrawMode(1);
			g.SetColor(new Color(Common.gBrightBallColors[mColorType]));
			float num4 = (float)image.GetCelWidth() / 2f;
			float num5 = (float)image.GetCelHeight() / 2f;
			num = Common._S(mX) - num4;
			num2 = Common._S(mY) - num5;
			Rect celRect2 = image.GetCelRect(mCel);
			if (flag)
			{
				g.DrawImageRotatedF(image, num, num2 - (float)Common._M(0), mRotation + num3, num4, num5 + (float)Common._M1(0), celRect2);
			}
			g.SetDrawMode(0);
			g.SetColorizeImages(colorizeImages: false);
		}
		else if (mPulseState < 2)
		{
			g.SetColorizeImages(colorizeImages: true);
			int mAlpha = 255 - mPulseTimer * ((mPulseState == 0) ? Common._M(4) : Common._M1(2));
			Color color = new Color(Common.gBrightBallColors[mColorType]);
			if (gApp.mColorblind)
			{
				color = new Color(Color.White);
			}
			color.mAlpha = mAlpha;
			g.SetColor(color);
			g.SetDrawMode(1);
			float num6 = (float)image.GetCelWidth() / 2f;
			float num7 = (float)image.GetCelHeight() / 2f;
			num = Common._S(mX) - num6;
			num2 = Common._S(mY) - num7;
			Rect celRect3 = image.GetCelRect(cel);
			if (flag)
			{
				g.DrawImageRotatedF(image, num, num2 - (float)Common._M(0), mRotation + num3, num6, num7 + (float)Common._M1(0), celRect3);
			}
			g.SetDrawMode(0);
			g.SetColorizeImages(colorizeImages: false);
		}
	}

	public void DrawNewPower(Graphics g, char theLetter, int xoff, int yoff)
	{
		GameApp gApp = GameApp.gApp;
		bool flag = gApp.Is3DAccelerated();
		Image imageByID = Res.GetImageByID(ResID.IMAGE_BALL);
		float num = Common._S(mX + (float)xoff) - (float)(imageByID.mWidth / 2);
		float num2 = Common._S(mY + (float)yoff) - (float)(imageByID.mHeight / 2);
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(new Color(Common.gBallColors[mColorType]));
		if (MathUtils._eq(mRadius, Common.GetDefaultBallRadius()))
		{
			if (flag)
			{
				g.DrawImageF(imageByID, num, num2);
			}
			else
			{
				g.DrawImage(imageByID, (int)num, (int)num2);
			}
		}
		else
		{
			mGlobalTransform.Reset();
			float num3 = mRadius / (float)Common.GetDefaultBallRadius();
			mGlobalTransform.Scale(num3, num3);
			num = mX + (float)xoff;
			num2 = mY + (float)yoff;
			if (flag)
			{
				g.DrawImageTransformF(imageByID, mGlobalTransform, num, num2);
			}
			else
			{
				g.DrawImageTransform(imageByID, mGlobalTransform, num, num2);
			}
		}
		g.SetColorizeImages(colorizeImages: false);
		g.SetColor(new Color(Common._M(16777215)));
		g.SetFont(Res.GetFontByID(ResID.FONT_MAIN22));
		string theString = theLetter.ToString();
		g.DrawString(theString, (int)(Common._S(mX + (float)xoff) - (float)(g.GetFont().CharWidth(theLetter) / 2)), (int)(Common._S(mY + (float)yoff) - (float)(g.GetFont().GetHeight() / 2) + (float)g.GetFont().GetAscent()));
	}

	public void DrawNewPower(Graphics g, char theLetter)
	{
		DrawNewPower(g, theLetter, 0, 0);
	}

	public void DrawPower(Graphics g)
	{
		PowerType powerType = mPowerType;
		switch (powerType)
		{
		case PowerType.PowerType_ProximityBomb:
			DrawStandardPower(g, 870, 3, (int)powerType);
			break;
		case PowerType.PowerType_Accuracy:
			DrawStandardPower(g, 870, 0, (int)powerType);
			break;
		case PowerType.PowerType_MoveBackwards:
			DrawStandardPower(g, 870, 4, (int)powerType);
			break;
		case PowerType.PowerType_SlowDown:
			DrawStandardPower(g, 870, 2, (int)powerType);
			break;
		case PowerType.PowerType_Cannon:
			DrawStandardPower(g, 870, 5, (int)powerType);
			break;
		case PowerType.PowerType_Laser:
			DrawStandardPower(g, 870, 6, (int)powerType);
			break;
		case PowerType.PowerType_ColorNuke:
			DrawStandardPower(g, 870, 1, (int)powerType);
			break;
		case PowerType.PowerType_GauntletMultBall:
			DrawMultPowerup(g);
			break;
		case PowerType.PowerType_Lob:
		case PowerType.PowerType_BombBullet:
		case PowerType.PowerType_BallEater:
		case PowerType.PowerType_Fireball:
		case PowerType.PowerType_ShieldFrog:
		case PowerType.PowerType_FreezeBoss:
			break;
		}
	}

	public void DrawExplosion(Graphics g)
	{
	}

	protected void DoDrawBase(Graphics g, int xoff, int yoff)
	{
		if (mPowerType != PowerType.PowerType_Max)
		{
			DrawPower(g);
			return;
		}
		int num = ((GameApp.gApp.GetBoard().GetHallucinateTimer() > 0) ? mDisplayType : mColorType);
		ResID id = (ResID)(1366 + num);
		if (GameApp.gApp.mColorblind && mColorType == 3)
		{
			id = ResID.IMAGE_GREEN_BALL_CBM;
		}
		else if (GameApp.gApp.mColorblind && mColorType == 4)
		{
			id = ResID.IMAGE_PURPLE_BALL_CBM;
		}
		Image imageByID = Res.GetImageByID(id);
		float x = Common._S(mX + (float)xoff - mRadius);
		float y = Common._S(mY + (float)yoff - mRadius);
		int theCel = (mLastFrame = GetFrame(imageByID));
		if (GameApp.gApp.Is3DAccelerated())
		{
			Rect celRect = imageByID.GetCelRect(theCel);
			mGlobalTransform.Reset();
			mGlobalTransform.RotateRad(mRotation);
			if (mDrawScale != 1f)
			{
				mGlobalTransform.Scale(mDrawScale, mDrawScale);
			}
			g.DrawImageTransformF(imageByID, mGlobalTransform, celRect, Common._S(mX + (float)xoff), Common._S(mY + (float)yoff));
		}
		else
		{
			BlendedImage blendedImage = CreateBlendedBall(num);
			blendedImage.Draw(g, x, y);
		}
	}

	protected void DoDrawAdditive(Graphics g, int xoff, int yoff)
	{
		if (mPowerType != PowerType.PowerType_Max)
		{
			return;
		}
		int num = ((GameApp.gApp.GetBoard().GetHallucinateTimer() > 0) ? mDisplayType : mColorType);
		ResID id = (ResID)(1366 + num);
		if (GameApp.gApp.mColorblind && mColorType == 3)
		{
			id = ResID.IMAGE_GREEN_BALL_CBM;
		}
		else if (GameApp.gApp.mColorblind && mColorType == 4)
		{
			id = ResID.IMAGE_PURPLE_BALL_CBM;
		}
		Image imageByID = Res.GetImageByID(id);
		Common._S(mX + (float)xoff - mRadius);
		Common._S(mY + (float)yoff - mRadius);
		int theCel = (mLastFrame = GetFrame(imageByID));
		if (GameApp.gApp.Is3DAccelerated())
		{
			Rect celRect = imageByID.GetCelRect(theCel);
			mGlobalTransform.Reset();
			mGlobalTransform.RotateRad(mRotation);
			if (mDrawScale != 1f)
			{
				mGlobalTransform.Scale(mDrawScale, mDrawScale);
			}
			if (mHilightPulse)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetDrawMode(1);
				g.SetColor(255, 255, 255);
				g.DrawImageTransformF(imageByID, mGlobalTransform, celRect, Common._S(mX), Common._S(mY));
				g.SetDrawMode(0);
				g.SetColorizeImages(colorizeImages: false);
			}
		}
	}

	public void DoDraw(Graphics g, int xoff, int yoff)
	{
		if (mPowerType != PowerType.PowerType_Max)
		{
			DrawPower(g);
			return;
		}
		int num = ((GameApp.gApp.GetBoard().GetHallucinateTimer() > 0) ? mDisplayType : mColorType);
		ResID id = (ResID)(1366 + num);
		if (GameApp.gApp.mColorblind && mColorType == 3)
		{
			id = ResID.IMAGE_GREEN_BALL_CBM;
		}
		else if (GameApp.gApp.mColorblind && mColorType == 4)
		{
			id = ResID.IMAGE_PURPLE_BALL_CBM;
		}
		Image imageByID = Res.GetImageByID(id);
		float x = Common._S(mX + (float)xoff - mRadius);
		float y = Common._S(mY + (float)yoff - mRadius);
		int theCel = (mLastFrame = GetFrame(imageByID));
		if (GameApp.gApp.Is3DAccelerated())
		{
			Rect celRect = imageByID.GetCelRect(theCel);
			mGlobalTransform.Reset();
			mGlobalTransform.RotateRad(mRotation);
			if (mDrawScale != 1f)
			{
				mGlobalTransform.Scale(mDrawScale, mDrawScale);
			}
			g.DrawImageTransformF(imageByID, mGlobalTransform, celRect, Common._S(mX + (float)xoff), Common._S(mY + (float)yoff));
			if (mHilightPulse)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetDrawMode(1);
				g.SetColor(255, 255, 255);
				g.DrawImageTransformF(imageByID, mGlobalTransform, celRect, Common._S(mX), Common._S(mY));
				g.SetDrawMode(0);
				g.SetColorizeImages(colorizeImages: false);
			}
		}
		else
		{
			BlendedImage blendedImage = CreateBlendedBall(num);
			blendedImage.Draw(g, x, y);
		}
	}

	public void DoDraw(Graphics g)
	{
		DoDraw(g, 0, 0);
	}

	public void DrawMultPowerup(Graphics g)
	{
		GameApp gApp = GameApp.gApp;
		ResID id = (ResID)(1375 + GetColorType());
		bool flag = true;
		if (gApp.mColorblind && mColorType == 3)
		{
			flag = false;
			id = (g.Is3D() ? ResID.IMAGE_GREEN_BALL_CBM : ResID.IMAGE_MULTIPLIER_BALL_GREEN_CBM);
		}
		else if (gApp.mColorblind && mColorType == 4)
		{
			flag = false;
			id = (g.Is3D() ? ResID.IMAGE_PURPLE_BALL_CBM : ResID.IMAGE_MULTIPLIER_BALL_PURPLE_CBM);
		}
		Image imageByID = Res.GetImageByID(id);
		float num = Common._S(mX) - (float)(imageByID.GetCelWidth() / 2);
		float num2 = Common._S(mY) - (float)(imageByID.GetCelHeight() / 2);
		if (flag)
		{
			int multAlpha = GetMultAlpha(mMultBallCel);
			int num3 = Common._M(255);
			if (multAlpha != num3)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, multAlpha);
			}
			BlendedImage blendedImage = null;
			BlendedImage blendedImage2 = null;
			if (!g.Is3D())
			{
				blendedImage = CreateBlendedPowerup(13, mColorType, imageByID, mMultBallCel);
				blendedImage2 = CreateBlendedPowerup(14, mColorType, imageByID, mMultBallCel2);
			}
			Rect celRect = imageByID.GetCelRect(mMultBallCel);
			if (g.Is3D())
			{
				g.DrawImageRotatedF(imageByID, num, num2, mRotation, celRect);
			}
			else
			{
				blendedImage.Draw(g, num, num2);
			}
			g.SetColorizeImages(colorizeImages: false);
			multAlpha = GetMultAlpha(mMultBallCel2);
			if (multAlpha != num3)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, multAlpha);
			}
			celRect = imageByID.GetCelRect(mMultBallCel2);
			if (g.Is3D())
			{
				g.DrawImageRotatedF(imageByID, num, num2, mRotation, celRect);
			}
			else
			{
				blendedImage2.Draw(g, num, num2);
			}
			g.SetColorizeImages(colorizeImages: false);
			g.SetDrawMode(1);
			g.SetColor(255, 255, 255, Common._M(204));
			g.SetColorizeImages(colorizeImages: true);
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_MULTIPLIER_BALL_OUTER);
			celRect = imageByID2.GetCelRect(GetFrame(imageByID2, Common._M(2)));
			if (g.Is3D())
			{
				g.DrawImageRotatedF(imageByID2, num, num2, mRotation, celRect);
			}
			else
			{
				g.DrawImageRotated(imageByID2, (int)num, (int)num2, mRotation, celRect);
			}
			g.SetColorizeImages(colorizeImages: false);
			g.SetDrawMode(0);
		}
		else if (g.Is3D())
		{
			Rect celRect2 = imageByID.GetCelRect(0);
			g.DrawImageRotatedF(imageByID, num, num2, mRotation, celRect2);
		}
		else
		{
			BlendedImage blendedImage3 = CreateBlendedPowerup(13, mColorType, imageByID, 0);
			blendedImage3.Draw(g, num, num2);
		}
	}

	public void UpdateProxmityBombExplosion()
	{
	}

	public void UpdateRotation()
	{
		if (mRotationInc != 0f)
		{
			mRotation += mRotationInc;
			if ((mRotationInc > 0f && mRotation > mDestRotation) || (mRotationInc < 0f && mRotation < mDestRotation))
			{
				mRotation = mDestRotation;
				mRotationInc = 0f;
			}
		}
	}

	public void SetupDefaultOverlayPulse()
	{
		int num = (int)Component.GetComponentValue(mOverlayPulse, 0f, mUpdateCount);
		mOverlayPulse.Clear();
		if (num == 128)
		{
			mOverlayPulse.Add(new Component(128f, 178f, mUpdateCount, mUpdateCount + 50));
			mOverlayPulse.Add(new Component(178f, 255f, mUpdateCount + 51, mUpdateCount + 60));
			mOverlayPulse.Add(new Component(255f, 128f, mUpdateCount + 61, mUpdateCount + 80));
		}
		else
		{
			mOverlayPulse.Add(new Component(num, 128f, mUpdateCount, mUpdateCount + 10));
		}
	}

	public void SetupElectricOverlayPulse(bool force_fade_out)
	{
		int num = (int)Component.GetComponentValue(mOverlayPulse, 0f, mUpdateCount);
		mOverlayPulse.Clear();
		if (force_fade_out)
		{
			if (num != 0)
			{
				mOverlayPulse.Add(new Component(num, 0f, mUpdateCount, mUpdateCount + 20));
			}
		}
		else if (num == 128)
		{
			mOverlayPulse.Add(new Component(128f, 255f, mUpdateCount, mUpdateCount + 20));
			mOverlayPulse.Add(new Component(255f, 128f, mUpdateCount + 21, mUpdateCount + 41));
		}
		else
		{
			mOverlayPulse.Add(new Component(num, 128f, mUpdateCount, mUpdateCount + 20));
		}
	}

	public void SetupElectricOverlayPulse()
	{
		SetupElectricOverlayPulse(force_fade_out: false);
	}

	public static void ResetIdGen()
	{
		mIdGen = 0;
	}

	public Ball()
	{
		mFrog = null;
		mMultOverlayAlpha = 0;
		mMultFX = null;
		mInTunnel = false;
		mCannonFrame = -1;
		mId = ++mIdGen;
		mDoBossPulse = false;
		mBossBlinkTimer = 0;
		mDebugDrawID = false;
		mCurve = null;
		mUpdateCount = 0;
		mHilightPulse = false;
		mSuckFromCompacting = false;
		mX = 0f;
		mY = 0f;
		mColorType = 0;
		mDisplayType = 0;
		mRadius = Common.GetDefaultBallRadius();
		mSuckBack = true;
		mBullet = null;
		mCel = 0;
		mShouldRemove = false;
		mLastFrame = 0;
		mMultBallCel = 0;
		mMultBallCel2 = Common._M(7);
		mIsCannon = false;
		mSpeedy = false;
		mElectricOverlayCel = 0;
		mList = null;
		mCollidesWithNext = false;
		mSuckCount = 0;
		mBackwardsCount = 0;
		mBackwardsSpeed = 0f;
		mComboCount = 0;
		mComboScore = 0;
		mRotation = 0f;
		mRotationInc = 0f;
		mNeedCheckCollision = false;
		mSuckPending = false;
		mShrinkClear = false;
		mIconCel = -1;
		mIconAppearScale = 1f;
		mIconScaleRate = 0f;
		mStartFrame = 0;
		mWayPoint = 0f;
		mPowerType = PowerType.PowerType_Max;
		mDestPowerType = PowerType.PowerType_Max;
		mPowerCount = 0;
		mPowerFade = 0;
		mGapBonus = 0;
		mNumGaps = 0;
		mParticles = null;
		mDrawScale = 1f;
		mExplodeFrame = 0;
		mPowerGracePeriod = 0;
		mLastPowerType = PowerType.PowerType_Max;
		mDoLaserAnim = false;
		mElectricExplodeOverlay.mLoopCount = (mElectricExplodeOverlay.mLayer1Cel = (mElectricExplodeOverlay.mLayer2Cel = 0));
		mExplodingFromLightning = false;
		mExploding = (mExplodingInTunnel = false);
	}

	public virtual void CopyFrom(Ball other)
	{
		mInTunnel = other.mInTunnel;
		mMultOverlayAlpha = other.mMultOverlayAlpha;
		mMultFX = other.mMultFX;
		mColorType = other.mColorType;
		mDisplayType = other.mDisplayType;
		mWayPoint = other.mWayPoint;
		mLastWayPoint = other.mLastWayPoint;
		mRotation = other.mRotation;
		mDestRotation = other.mDestRotation;
		mRotationInc = other.mRotationInc;
		mX = other.mX;
		mY = other.mY;
		mLastX = other.mLastX;
		mLastY = other.mLastY;
		mDrawScale = other.mDrawScale;
		mRadius = other.mRadius;
		mPulseState = other.mPulseState;
		mPulseTimer = other.mPulseTimer;
		mOverlayPulse.Clear();
		mOverlayPulse.AddRange(other.mOverlayPulse.ToArray());
		mElectricOverlay.Clear();
		mElectricOverlay.AddRange(other.mElectricOverlay.ToArray());
		mElectricExplodeOverlay = other.mElectricExplodeOverlay;
		mElectricOverlayCel = other.mElectricOverlayCel;
		mList = other.mList;
		mCurve = other.mCurve;
		mCollidesWithNext = other.mCollidesWithNext;
		mSuckPending = other.mSuckPending;
		mShrinkClear = other.mShrinkClear;
		mSuckFromCompacting = other.mSuckFromCompacting;
		mExplodingInTunnel = other.mExplodingInTunnel;
		mExploding = other.mExploding;
		mExplodingFromLightning = other.mExplodingFromLightning;
		mExplodeFrame = other.mExplodeFrame;
		mShouldRemove = other.mShouldRemove;
		mSpeedy = other.mSpeedy;
		mSuckBack = other.mSuckBack;
		mPowerGracePeriod = other.mPowerGracePeriod;
		mLastPowerType = other.mLastPowerType;
		mCannonFrame = other.mCannonFrame;
		mIsCannon = other.mIsCannon;
		mDoLaserAnim = other.mDoLaserAnim;
		mUpdateCount = other.mUpdateCount;
		mCel = other.mCel;
		mBullet = other.mBullet;
		mSuckCount = other.mSuckCount;
		mBackwardsCount = other.mBackwardsCount;
		mComboCount = other.mComboCount;
		mBackwardsSpeed = other.mBackwardsSpeed;
		mPowerCount = other.mPowerCount;
		mComboScore = other.mComboScore;
		mStartFrame = other.mStartFrame;
		mPowerFade = other.mPowerFade;
		mGapBonus = other.mGapBonus;
		mNumGaps = other.mNumGaps;
		mIconAppearScale = other.mIconAppearScale;
		mIconScaleRate = other.mIconScaleRate;
		mIconCel = other.mIconCel;
		mMultBallCel = other.mMultBallCel;
		mMultBallCel2 = other.mMultBallCel2;
		mParticles = other.mParticles;
		mPowerType = other.mPowerType;
		mDestPowerType = other.mDestPowerType;
		mHilightPulse = other.mHilightPulse;
		mDebugDrawID = other.mDebugDrawID;
		mDoBossPulse = other.mDoBossPulse;
		mBossBlinkTimer = other.mBossBlinkTimer;
		mLastFrame = other.mLastFrame;
		mFrog = other.mFrog;
	}

	public virtual void Dispose()
	{
		if (mCurve != null && mCurve.mBoard != null)
		{
			mCurve.mBoard.BallDeleted(this);
		}
		Board board = GameApp.gApp.GetBoard();
		if (board != null && this == board.GetGuideBall())
		{
			board.GuideBallInvalidated();
		}
		mParticles = null;
		CleanUpMultiplierOverlays();
	}

	public void SetPos(float x, float y)
	{
		mX = x;
		mY = y;
	}

	public void SetWayPoint(float thePoint, bool in_tunnel)
	{
		mWayPoint = thePoint;
		mInTunnel = in_tunnel;
	}

	public int GetFrame(Image img, int div)
	{
		int num = 0;
		int num2 = ((img.mNumCols == 1) ? img.mNumRows : (img.mNumRows * img.mNumCols));
		int num3 = (int)mWayPoint;
		num = (num3 / div + mStartFrame) % num2;
		if (num < 0)
		{
			num = -num;
		}
		else if (num >= num2)
		{
			num = num2 - 1;
		}
		return num;
	}

	public int GetFrame(Image img)
	{
		return GetFrame(img, 1);
	}

	public void CleanUpMultiplierOverlays()
	{
		GameApp.gApp.ReleaseGenericCachedEffect(mMultFX);
		mMultFX = null;
	}

	public void SetRotation(float theRot, bool immediate)
	{
		if (immediate)
		{
			mRotation = theRot;
		}
		else if (!MathUtils._eq(theRot, mRotation, 0.001f))
		{
			while (Math.Abs(theRot - mRotation) > 3.14159f)
			{
				theRot = ((!(theRot > mRotation)) ? (theRot + 6.28318f) : (theRot - 6.28318f));
			}
			mDestRotation = theRot;
			mRotationInc = 0.10471967f;
			if (theRot < mRotation)
			{
				mRotationInc = 0f - mRotationInc;
			}
		}
	}

	public void SetRotation(float theRot)
	{
		SetRotation(theRot, immediate: false);
	}

	public virtual void DrawBase(Graphics g, int xoff, int yoff)
	{
		if (mDrawScale <= 0f || mColorType == -1)
		{
			return;
		}
		if (mExploding && !mShrinkClear && mExplodingInTunnel)
		{
			if (g.Is3D())
			{
				DrawExplosion(g);
			}
		}
		else if (!mExploding || mShrinkClear)
		{
			DoDrawBase(g, xoff, yoff);
		}
		g.SetColorizeImages(colorizeImages: false);
		g.SetDrawMode(0);
	}

	public virtual void DrawAdditive(Graphics g, int xoff, int yoff)
	{
		if (mDrawScale <= 0f || mColorType == -1)
		{
			return;
		}
		if (mExploding && !mShrinkClear && mExplodingInTunnel)
		{
			if (g.Is3D())
			{
				DrawExplosion(g);
			}
		}
		else if (!mExploding || mShrinkClear)
		{
			DoDrawAdditive(g, xoff, yoff);
			if (mPowerFade != 0 && (mCurve == null || mCurve.mPostZumaFlashTimer <= 0))
			{
				int num = ((mPowerType == PowerType.PowerType_GauntletMultBall) ? ((int)Common._M(2f)) : 4);
				if (((mPowerFade >> num) & 1) != 0)
				{
					g.SetDrawMode(1);
					DoDrawBase(g, xoff, yoff);
					DoDrawAdditive(g, xoff, yoff);
				}
			}
			else if ((mDoBossPulse && (float)mBossBlinkTimer < Common._M(10f)) || (mCurve != null && mCurve.mPostZumaFlashTimer > 0))
			{
				g.SetDrawMode(1);
				DoDrawBase(g, xoff, yoff);
				DoDrawAdditive(g, xoff, yoff);
			}
		}
		g.SetColorizeImages(colorizeImages: false);
		g.SetDrawMode(0);
	}

	public virtual void Draw(Graphics g, int xoff, int yoff)
	{
		if (mDrawScale <= 0f || mColorType == -1)
		{
			return;
		}
		if (mExploding && !mShrinkClear && mExplodingInTunnel)
		{
			if (g.Is3D())
			{
				DrawExplosion(g);
			}
		}
		else if (!mExploding || mShrinkClear)
		{
			DoDraw(g, xoff, yoff);
			if (mPowerFade != 0 && (mCurve == null || mCurve.mPostZumaFlashTimer <= 0))
			{
				int num = ((mPowerType == PowerType.PowerType_GauntletMultBall) ? ((int)Common._M(2f)) : 4);
				if (((mPowerFade >> num) & 1) != 0)
				{
					g.SetDrawMode(1);
					DoDraw(g, xoff, yoff);
				}
			}
			else if ((mDoBossPulse && (float)mBossBlinkTimer < Common._M(10f)) || (mCurve != null && mCurve.mPostZumaFlashTimer > 0))
			{
				g.SetDrawMode(1);
				DoDraw(g, xoff, yoff);
				g.SetDrawMode(0);
			}
		}
		g.SetColorizeImages(colorizeImages: false);
		g.SetDrawMode(0);
		if (mDebugDrawID)
		{
			Font fontByID = Res.GetFontByID(ResID.FONT_MAIN22);
			g.SetFont(fontByID);
			g.SetColor(Color.Black);
			g.FillRect((int)Common._S(mX - 12f), (int)Common._S(mY - 8f), Common._S(24), Common._S(16));
			g.SetColor(Color.White);
			g.DrawString($"{mId}", (int)Common._S(mX - 10f), (int)Common._S(mY - 12f) + fontByID.GetAscent());
		}
	}

	public void Draw(Graphics g)
	{
		Draw(g, 0, 0);
	}

	public void DrawProximityBombExplosion(Graphics g)
	{
	}

	public void DrawShadow(Graphics g)
	{
		if (GlobalMembers.gSexyApp.Is3DAccelerated() && !mExploding)
		{
			Transform transform = new Transform();
			float num = Common._S(mX - 3f);
			float num2 = Common._S(mY + 5f);
			if (mDrawScale > 1f)
			{
				num -= Common._S(Common._M(9f)) * (mDrawScale - 1f);
				num2 += Common._S(Common._M(15f)) * (mDrawScale - 1f);
				transform.Scale(mDrawScale, mDrawScale);
			}
			g.DrawImageTransformF(Res.GetImageByID(ResID.IMAGE_BALL_SHADOW), transform, num, num2);
		}
	}

	public void DrawTopLayer(Graphics g)
	{
		Graphics3D graphics3D = g.Get3D();
		if ((mPowerType != PowerType.PowerType_Max || mElectricOverlay.size() > 0 || mOverlayPulse.size() > 0) && !GetIsExploding())
		{
			Image imageByID = Res.GetImageByID(ResID.IMAGE_BALL_GLOW);
			Color color = gOverlayColors[mColorType];
			color.mAlpha = (int)Component.GetComponentValue(mOverlayPulse, 0f, mUpdateCount);
			g.SetColor(color);
			g.SetColorizeImages(colorizeImages: true);
			g.SetDrawMode(1);
			int num = (int)Common._S(mX - mRadius);
			int num2 = (int)Common._S(mY - mRadius);
			num -= (imageByID.mWidth - Common._S(Common.GetDefaultBallSize())) / 2 - 1;
			num2 -= (imageByID.mHeight - Common._S(Common.GetDefaultBallSize())) / 2 - 1;
			if (!GameApp.gApp.mColorblind)
			{
				if (graphics3D != null)
				{
					g.DrawImageF(imageByID, num, num2);
				}
				else
				{
					g.DrawImage(imageByID, num, num2);
				}
			}
			g.SetDrawMode(0);
			g.SetColorizeImages(colorizeImages: false);
		}
		if (mMultFX != null && (!GameApp.gApp.mColorblind || (mColorType != 3 && mColorType != 4)))
		{
			mMultFX.DrawLayer(g, mMultFX.GetLayer("Top"));
			mMultFX.DrawLayerNormal(g, mMultFX.GetLayer("Top"));
			mMultFX.DrawLayerAdditive(g, mMultFX.GetLayer("Top"));
			mMultFX.DrawPhisycalLayer(g, mMultFX.GetLayer("Top"));
		}
		if (mExplodingInTunnel)
		{
			DrawLightningExplosion(g);
		}
		if (mElectricOverlay.size() > 0)
		{
			int num3 = (int)Component.GetComponentValue(mElectricOverlay, 0f, mUpdateCount);
			g.SetDrawMode(1);
			if (num3 != 255)
			{
				g.SetColor(255, 255, 255, num3);
				g.SetColorizeImages(colorizeImages: true);
			}
			g.SetDrawMode(0);
			if (num3 != 255)
			{
				g.SetColorizeImages(colorizeImages: false);
			}
		}
	}

	public void DrawBottomLayer(Graphics g)
	{
		Graphics3D graphics3D = g.Get3D();
		if (!mCurve.mWayPointMgr.InTunnel(this, inFront: true))
		{
			mCurve.mWayPointMgr.InTunnel(this, inFront: false);
		}
		if (mMultFX != null && graphics3D != null)
		{
			mMultFX.DrawLayer(g, mMultFX.GetLayer("Bottom"));
			mMultFX.DrawLayerNormal(g, mMultFX.GetLayer("Bottom"));
			mMultFX.DrawLayerAdditive(g, mMultFX.GetLayer("Bottom"));
			mMultFX.DrawPhisycalLayer(g, mMultFX.GetLayer("Bottom"));
		}
		if (mDoLaserAnim)
		{
			Image imageByID = Res.GetImageByID(ResID.IMAGE_LAZER_BURN);
			Rect celRect = imageByID.GetCelRect(mLaserAnimCel);
			float num = CommonMath.AngleBetweenPoints(mFrog.GetCenterX(), mFrog.GetCenterY(), mX, mY) + 1.570795f;
			g.DrawImageRotated(imageByID, (int)Common._S(mX + (float)Common._M(-38)), (int)Common._S(mY + (float)Common._M1(-52)), num, Common._S(Common._M2(38)), Common._S(Common._M3(52)), celRect);
		}
	}

	public void DrawAboveBalls(Graphics g)
	{
		if (mIconCel != -1 && MathUtils._geq(mIconAppearScale, 1f) && g.Is3D())
		{
			Image imageByID = Res.GetImageByID(ResID.IMAGE_POWERUPS_PULSES);
			float num = ((GetPowerOrDestType() == PowerType.PowerType_MoveBackwards) ? 0f : (-1.570795f));
			int num2 = (int)Common._S(mX);
			int num3 = (int)Common._S(mY);
			g.SetDrawMode(1);
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(new Color(Common.gBallColors[mColorType]));
			mGlobalTransform.Reset();
			mGlobalTransform.Scale(mIconAppearScale, mIconAppearScale);
			mGlobalTransform.RotateRad(mRotation + num);
			g.DrawImageTransform(imageByID, mGlobalTransform, imageByID.GetCelRect(mIconCel), num2, num3);
			g.SetDrawMode(0);
			g.SetColorizeImages(colorizeImages: false);
		}
		if (mExploding && !mShrinkClear && !mExplodingInTunnel)
		{
			DrawExplosion(g);
		}
		if (!mExplodingInTunnel)
		{
			DrawLightningExplosion(g);
		}
	}

	public void DrawLightningExplosion(Graphics g)
	{
		if (mElectricExplodeOverlay.mLayer1Alpha.size() > 0)
		{
			g.SetDrawMode(1);
			int num = (int)Component.GetComponentValue(mElectricExplodeOverlay.mLayer2Alpha, 255f, mUpdateCount);
			if (num != 255)
			{
				g.SetColor(255, 255, 255, num);
				g.SetColorizeImages(colorizeImages: true);
			}
			g.SetColorizeImages(colorizeImages: false);
			num = (int)Component.GetComponentValue(mElectricExplodeOverlay.mLayer1Alpha, 255f, mUpdateCount);
			Component.GetComponentValue(mElectricExplodeOverlay.mLayer1Scale, 1f, mUpdateCount);
			if (num != 255)
			{
				g.SetColor(255, 255, 255, num);
				g.SetColorizeImages(colorizeImages: true);
			}
			g.SetDrawMode(0);
			if (num != 255)
			{
				g.SetColorizeImages(colorizeImages: false);
			}
		}
	}

	public void DoElectricOverlay(bool val)
	{
		if (!val && mElectricOverlay.Count() > 0)
		{
			int num = (int)Component.GetComponentValue(mElectricOverlay, 0f, mUpdateCount);
			mElectricOverlay.Clear();
			int num2 = (int)(2f / 51f * (float)num);
			mElectricOverlay.Add(new Component(num, 0f, mUpdateCount, mUpdateCount + ((num2 < 1) ? 1 : num2)));
			SetupDefaultOverlayPulse();
		}
		else if (val && mElectricOverlay.Count() == 0)
		{
			mElectricOverlay.Add(new Component(0f, 255f, mUpdateCount, mUpdateCount + 10));
		}
		else if (val)
		{
			int num3 = (int)Component.GetComponentValue(mElectricOverlay, 0f, mUpdateCount);
			if (num3 != 255)
			{
				mElectricOverlay.Clear();
				int num4 = (int)(2f / 51f * (float)num3);
				mElectricOverlay.Add(new Component(num3, 255f, mUpdateCount, mUpdateCount + ((num4 < 1) ? 1 : num4)));
				SetupElectricOverlayPulse();
			}
		}
		else if (!val)
		{
			SetupDefaultOverlayPulse();
		}
	}

	public bool CollidesWithPhysically(Ball theBall, int thePad)
	{
		float num = theBall.GetX() - GetX();
		float num2 = theBall.GetY() - GetY();
		float num3 = (float)theBall.GetRadius() + (float)(thePad * 2) + (float)GetRadius();
		return num * num + num2 * num2 < num3 * num3;
	}

	public bool CollidesWithPhysically(Ball theBall)
	{
		return CollidesWithPhysically(theBall, 0);
	}

	public bool CollidesWith(Ball theBall, int thePad)
	{
		return Math.Abs((float)(int)mWayPoint - (float)(int)theBall.mWayPoint) < (float)((Common.GetDefaultBallRadius() + thePad) * 2);
	}

	public bool CollidesWith(Ball theBall)
	{
		return CollidesWith(theBall, 0);
	}

	public bool CollidesWithPhysically(int pointx, int pointy, int radius)
	{
		float num = (float)pointx - GetX();
		float num2 = (float)pointy - GetY();
		float num3 = (float)radius + (float)GetRadius();
		return num * num + num2 * num2 < num3 * num3;
	}

	public bool Intersects(SexyVector3 p1, SexyVector3 v1, ref float t)
	{
		SexyVector3 v2 = new SexyVector3(p1.x - mX, p1.y - mY, 0f);
		float num = mRadius - (float)Common._M(1);
		float num2 = v1.Dot(v1);
		float num3 = 2f * v2.Dot(v1);
		float num4 = v2.Dot(v2) - num * 2f * (num * 2f);
		float num5 = num3 * num3 - 4f * num2 * num4;
		if (num5 < 0f)
		{
			return false;
		}
		num5 = (float)Math.Sqrt(num5);
		t = (0f - num3 - num5) / (2f * num2);
		return true;
	}

	public void SetBullet(Bullet theBullet)
	{
		mBullet = theBullet;
	}

	public void SetCollidesWithPrev(bool collidesWithPrev)
	{
		GetPrevBall()?.SetCollidesWithNext(collidesWithPrev);
	}

	public bool GetCollidesWithPrev()
	{
		return GetPrevBall()?.GetCollidesWithNext() ?? false;
	}

	public void UpdateCollisionInfo(int thePad)
	{
		Ball prevBall = GetPrevBall();
		Ball nextBall = GetNextBall();
		prevBall?.SetCollidesWithNext(prevBall.CollidesWith(this, thePad));
		if (nextBall != null)
		{
			SetCollidesWithNext(nextBall.CollidesWith(this, thePad));
		}
		else
		{
			SetCollidesWithNext(collidesWithNext: false);
		}
	}

	public void UpdateCollisionInfo()
	{
		UpdateCollisionInfo(0);
	}

	public void SetPowerType(PowerType theType, bool delay)
	{
		mDoBossPulse = false;
		if (theType == mPowerType)
		{
			return;
		}
		mPulseState = 0;
		mPulseTimer = 0;
		mIconCel = -1;
		if (theType != PowerType.PowerType_Max)
		{
			mPowerGracePeriod = 0;
			mLastPowerType = PowerType.PowerType_Max;
		}
		if (delay)
		{
			mDestPowerType = theType;
			if (theType == PowerType.PowerType_Max && mPowerType == PowerType.PowerType_GauntletMultBall)
			{
				mPowerFade = 300;
			}
			else
			{
				mPowerFade = 100;
			}
			switch (theType)
			{
			case PowerType.PowerType_Accuracy:
				mIconCel = 0;
				break;
			case PowerType.PowerType_ColorNuke:
				mIconCel = 1;
				break;
			case PowerType.PowerType_SlowDown:
				mIconCel = 2;
				break;
			case PowerType.PowerType_ProximityBomb:
				mIconCel = 3;
				break;
			case PowerType.PowerType_MoveBackwards:
				mIconCel = 4;
				break;
			case PowerType.PowerType_Cannon:
				mIconCel = 5;
				break;
			case PowerType.PowerType_Laser:
				mIconCel = 6;
				break;
			}
			int soundByID = Res.GetSoundByID(ResID.SOUND_MULT_APPEAR);
			int soundByID2 = Res.GetSoundByID(ResID.SOUND_POWERUP_APPEARS);
			int soundByID3 = Res.GetSoundByID(ResID.SOUND_MULT_DISAPPEAR);
			int soundByID4 = Res.GetSoundByID(ResID.SOUND_POWERUP_DISAPPEARS);
			switch (theType)
			{
			case PowerType.PowerType_GauntletMultBall:
				((GameApp)GlobalMembers.gSexyApp).PlaySample(soundByID);
				break;
			default:
				((GameApp)GlobalMembers.gSexyApp).PlaySample(soundByID2);
				break;
			case PowerType.PowerType_Max:
				if (GetPowerOrDestType() != PowerType.PowerType_Max)
				{
					if (GetPowerOrDestType() == PowerType.PowerType_GauntletMultBall)
					{
						((GameApp)GlobalMembers.gSexyApp).PlaySample(soundByID3);
					}
					else
					{
						((GameApp)GlobalMembers.gSexyApp).PlaySample(soundByID4);
					}
				}
				break;
			}
			mIconAppearScale = 5f;
			mIconScaleRate = (mIconAppearScale - 1f) / (float)mPowerFade;
		}
		else
		{
			mDestPowerType = PowerType.PowerType_Max;
			mPowerType = theType;
		}
		if (theType != PowerType.PowerType_Max && mCurve != null)
		{
			mCurve.SetColorHasPowerup(mColorType, val: true);
		}
		SetupDefaultOverlayPulse();
	}

	public void SetPowerType(PowerType theType)
	{
		SetPowerType(theType, delay: true);
	}

	public PowerType GetPowerOrDestType(bool include_grace_period)
	{
		if (mPowerType != PowerType.PowerType_Max)
		{
			return mPowerType;
		}
		if (mPowerGracePeriod > 0 && mLastPowerType != PowerType.PowerType_Max)
		{
			return mLastPowerType;
		}
		return mDestPowerType;
	}

	public PowerType GetPowerOrDestType()
	{
		return GetPowerOrDestType(include_grace_period: true);
	}

	public void RemoveFromList()
	{
		if (mList != null)
		{
			mList.Remove(this);
			mList = null;
		}
	}

	public int InsertInList(List<Ball> theList, int theInsertItr, CurveMgr cm)
	{
		mList = theList;
		theList.Insert(theInsertItr, this);
		mCurve = cm;
		return theInsertItr;
	}

	public SexyVector3 GetSpeed()
	{
		return new SexyVector3(mX - mLastX, mY - mLastY, 0f);
	}

	public float GetWayPointProgress()
	{
		return mWayPoint - mLastWayPoint;
	}

	public Ball GetPrevBall(bool mustCollide)
	{
		if (mList == null)
		{
			return null;
		}
		int listItr = GetListItr();
		if (listItr == 0)
		{
			return null;
		}
		if (!mustCollide)
		{
			return mList[--listItr];
		}
		Ball ball = mList[--listItr];
		if (ball.GetCollidesWithNext())
		{
			return ball;
		}
		return null;
	}

	public Ball GetPrevBall()
	{
		return GetPrevBall(mustCollide: false);
	}

	public Ball GetNextBall(bool mustCollide)
	{
		if (mList == null)
		{
			return null;
		}
		int listItr = GetListItr();
		listItr++;
		if (listItr >= mList.Count())
		{
			return null;
		}
		if (!mustCollide || GetCollidesWithNext())
		{
			return mList[listItr];
		}
		return null;
	}

	public Ball GetNextBall()
	{
		return GetNextBall(mustCollide: false);
	}

	public CurveMgr GetCurve()
	{
		return mCurve;
	}

	public void Explode(bool in_tunnel, bool from_lightning_frog)
	{
		if (!mExploding)
		{
			mExploding = true;
			mExplodingInTunnel = in_tunnel;
			Board board = GameApp.gApp.GetBoard();
			if (!mExplodingInTunnel)
			{
				board.AddBallExplosionParticleEffect(this);
			}
			if (GetPowerOrDestType() == PowerType.PowerType_ProximityBomb)
			{
				PowerEffect powerEffect = new PowerEffect(mX, mY);
				powerEffect.AddDefaultEffectType(0, mColorType, mRotation);
				board.AddPowerEffect(powerEffect);
				board.AddProxBombExplosion(GetX(), GetY());
			}
			else if (GetPowerOrDestType() == PowerType.PowerType_Accuracy)
			{
				PowerEffect powerEffect2 = new PowerEffect(mX, mY);
				powerEffect2.AddDefaultEffectType(1, mColorType, mRotation);
				board.AddPowerEffect(powerEffect2);
			}
			else if (GetPowerOrDestType() == PowerType.PowerType_MoveBackwards)
			{
				PowerEffect powerEffect3 = new ReversePowerEffect(mX, mY, this);
				powerEffect3.AddDefaultEffectType(2, mColorType, mRotation);
				board.AddPowerEffect(powerEffect3);
			}
			else if (GetPowerOrDestType() == PowerType.PowerType_SlowDown)
			{
				PowerEffect powerEffect4 = new PowerEffect(mX, mY);
				powerEffect4.AddDefaultEffectType(3, mColorType, mRotation);
				board.AddPowerEffect(powerEffect4);
			}
			else if (GetPowerOrDestType() == PowerType.PowerType_Cannon)
			{
				PowerEffect powerEffect5 = new CannonPowerEffect(this);
				powerEffect5.AddDefaultEffectType(4, mColorType, mRotation);
				board.AddPowerEffect(powerEffect5);
			}
			else if (GetPowerOrDestType() == PowerType.PowerType_Laser)
			{
				PowerEffect powerEffect6 = new PowerEffect(mX, mY);
				powerEffect6.AddDefaultEffectType(5, mColorType, mRotation);
				board.AddPowerEffect(powerEffect6);
			}
			else if (GetPowerOrDestType() == PowerType.PowerType_GauntletMultBall)
			{
				CleanUpMultiplierOverlays();
			}
			if (GetPowerOrDestType() != PowerType.PowerType_Max)
			{
				mCurve.SetColorHasPowerup(mColorType, val: false);
			}
			if (from_lightning_frog)
			{
				mExplodingFromLightning = true;
				mElectricOverlay.Clear();
				mElectricOverlay.Add(new Component(255f, 0f, mUpdateCount, mUpdateCount + 10));
				mElectricExplodeOverlay.mLayer1Alpha.Add(new Component(0f, 0f, mUpdateCount, mUpdateCount + 20));
				mElectricExplodeOverlay.mLayer1Alpha.Add(new Component(25f, 255f, mUpdateCount + 21, mUpdateCount + 41));
				mElectricExplodeOverlay.mLayer1Scale.Add(new Component(0.5f, 1f, mUpdateCount + 21, mUpdateCount + 41));
				mElectricExplodeOverlay.mLayer2Alpha.Add(new Component(25f, 255f, mUpdateCount, mUpdateCount + 20));
				mElectricExplodeOverlay.mLoopCount = 0;
			}
			else if (mElectricOverlay.size() > 0)
			{
				mElectricOverlay.Clear();
				mElectricOverlay.Add(new Component(255f, 0f, mUpdateCount, mUpdateCount + 5));
			}
		}
	}

	public void Explode(bool in_tunnel)
	{
		Explode(in_tunnel, from_lightning_frog: false);
	}

	public void Explode()
	{
		Explode(in_tunnel: false, from_lightning_frog: false);
	}

	public void Update()
	{
		mUpdateCount++;
		mLastWayPoint = mWayPoint;
		mLastX = mX;
		mLastY = mY;
		GameApp gApp = GameApp.gApp;
		if (gApp.GetBoard().GetHallucinateTimer() > 0 && mUpdateCount % Common._M(25) == 0)
		{
			mDisplayType = MathUtils.SafeRand() % 6;
		}
		if (mDoBossPulse && mBossBlinkTimer == 0)
		{
			mBossBlinkTimer = Common._M(20);
		}
		else if (mBossBlinkTimer > 0)
		{
			mBossBlinkTimer--;
		}
		if (mUpdateCount % Common._M(6) == 0 && (!gApp.mColorblind || (mColorType != 3 && mColorType != 4)))
		{
			Image imageByID = Res.GetImageByID(ResID.IMAGE_MULTIPLIER_BALL_BLUE);
			mMultBallCel = (mMultBallCel + 1) % (imageByID.mNumRows * imageByID.mNumCols);
			mMultBallCel2 = (mMultBallCel2 + 1) % (imageByID.mNumRows * imageByID.mNumCols);
		}
		if (mPowerFade > 0)
		{
			mIconAppearScale -= mIconScaleRate;
			if (mIconAppearScale < 1f)
			{
				mIconAppearScale = 1f;
			}
			if (mPowerType == PowerType.PowerType_GauntletMultBall && mDestPowerType == PowerType.PowerType_Max && mPowerFade < 51)
			{
				mMultOverlayAlpha -= 5;
				if (mMultOverlayAlpha < 0)
				{
					mMultOverlayAlpha = 0;
				}
			}
			mPowerFade--;
			if (mPowerFade == 0)
			{
				mPowerType = mDestPowerType;
				if (mPowerType == PowerType.PowerType_GauntletMultBall)
				{
					int num = mColorType;
					if (gApp.mColorblind && (mColorType == 3 || mColorType == 4))
					{
						num = 5;
					}
					mMultFX = gApp.mResourceManager.GetPIEffect(fx_files[num]).Duplicate();
					mMultFX.mEmitAfterTimeline = true;
					mMultOverlayAlpha = 0;
				}
				else if (mPowerType == PowerType.PowerType_Max)
				{
					CleanUpMultiplierOverlays();
				}
				mIconCel = -1;
				mDestPowerType = PowerType.PowerType_Max;
				if (mPowerType != PowerType.PowerType_Max && mPowerCount <= 0)
				{
					mPowerCount = (int)((float)Common._M(2000) * GameApp.gDDS.mHandheldBalance.mFruitPowerupAdditionalDuration);
				}
			}
		}
		if (mMultFX != null)
		{
			mMultFX.mDrawTransform.LoadIdentity();
			float num2 = GameApp.DownScaleNum(1f);
			mMultFX.mDrawTransform.Scale(num2, num2);
			mMultFX.mDrawTransform.RotateRad(mRotation);
			mMultFX.mDrawTransform.Translate(Common._S(mX), Common._S(mY));
			mMultFX.mColor.mAlpha = mMultOverlayAlpha;
			mMultFX.Update();
		}
		if (mMultFX != null && (mDestPowerType != PowerType.PowerType_Max || mPowerFade >= 51 || mPowerFade == 0))
		{
			int num3 = Common._M(3);
			if (mInTunnel && mMultOverlayAlpha > 0)
			{
				mMultOverlayAlpha -= num3;
			}
			else if (!mInTunnel && mMultOverlayAlpha < 255)
			{
				mMultOverlayAlpha += num3;
			}
		}
		mMultOverlayAlpha = Math.Min(Math.Max(mMultOverlayAlpha, 0), 255);
		if (mDoLaserAnim && mUpdateCount % Common._M(4) == 0)
		{
			mLaserAnimCel = (mLaserAnimCel + 1) % Res.GetImageByID(ResID.IMAGE_LAZER_BURN).mNumCols;
		}
		if (mPowerCount > 0 && !mExploding && --mPowerCount <= 0)
		{
			mPowerGracePeriod = Common._M(150);
			mLastPowerType = GetPowerOrDestType();
			mCurve.PowerupExpired(GetPowerOrDestType());
			mCurve.SetColorHasPowerup(mColorType, val: false);
			SetPowerType(PowerType.PowerType_Max);
		}
		if (mPowerGracePeriod > 0 && --mPowerGracePeriod == 0)
		{
			mLastPowerType = PowerType.PowerType_Max;
		}
		if (mElectricOverlay.size() > 0 && Component.UpdateComponentVec(mElectricOverlay, mUpdateCount) && MathUtils._eq(Component.GetComponentValue(mElectricOverlay, 0f, mUpdateCount), 0f, 0.0001f))
		{
			mElectricOverlay.Clear();
		}
		if (mElectricExplodeOverlay.mLayer1Alpha.size() > 0)
		{
			_ = mUpdateCount % Common._M(7);
		}
		if (mExploding && mElectricExplodeOverlay.mLayer1Alpha.size() > 0)
		{
			Component.UpdateComponentVec(mElectricExplodeOverlay.mLayer2Alpha, mUpdateCount);
			Component.UpdateComponentVec(mElectricExplodeOverlay.mLayer1Scale, mUpdateCount);
			if (Component.UpdateComponentVec(mElectricExplodeOverlay.mLayer1Alpha, mUpdateCount))
			{
				if (++mElectricExplodeOverlay.mLoopCount == 1)
				{
					mElectricExplodeOverlay.mLayer1Alpha.Clear();
					mElectricExplodeOverlay.mLayer1Alpha.Add(new Component(255f, 255f, mUpdateCount, mUpdateCount + 30));
				}
				else if (mElectricExplodeOverlay.mLoopCount == 2)
				{
					mElectricExplodeOverlay.mLayer1Alpha.Clear();
					mElectricExplodeOverlay.mLayer1Alpha.Add(new Component(255f, 0f, mUpdateCount, mUpdateCount + 20));
					mElectricExplodeOverlay.mLayer2Alpha.Clear();
					mElectricExplodeOverlay.mLayer2Alpha.Add(new Component(255f, 0f, mUpdateCount, mUpdateCount + 20));
					mElectricExplodeOverlay.mLayer1Scale.Clear();
					mElectricExplodeOverlay.mLayer1Scale.Add(new Component(1f, 1f, mUpdateCount, mUpdateCount + 4));
					mElectricExplodeOverlay.mLayer1Scale.Add(new Component(1f, 0.2f, mUpdateCount + 5, mUpdateCount + 20));
				}
				else if (mElectricExplodeOverlay.mLoopCount == 3)
				{
					mElectricExplodeOverlay.mLayer1Scale.Clear();
					mElectricExplodeOverlay.mLayer1Alpha.Clear();
					mElectricExplodeOverlay.mLayer2Alpha.Clear();
				}
			}
		}
		if (mPowerType != PowerType.PowerType_Max)
		{
			if (Component.UpdateComponentVec(mOverlayPulse, mUpdateCount))
			{
				if (mElectricOverlay.size() == 0)
				{
					SetupDefaultOverlayPulse();
				}
				else
				{
					SetupElectricOverlayPulse();
				}
			}
			if (!mExploding)
			{
				mPulseTimer++;
				if (mPulseState == 0 && mPulseTimer >= Common._M(30))
				{
					mPulseState++;
					mPulseTimer = 0;
				}
				else if (mPulseState == 1 && mPulseTimer >= 128)
				{
					mPulseTimer = 0;
					mPulseState++;
				}
				else if (mPulseState == 2 && mPulseTimer >= Common._M(25))
				{
					mPulseState = 0;
					mPulseTimer = 0;
				}
			}
		}
		else if (mElectricOverlay.size() > 0 && Component.UpdateComponentVec(mOverlayPulse, mUpdateCount))
		{
			SetupElectricOverlayPulse();
		}
		else if (mElectricOverlay.size() == 0 && mOverlayPulse.size() > 0 && Component.UpdateComponentVec(mOverlayPulse, mUpdateCount))
		{
			mOverlayPulse.Clear();
		}
		UpdateRotation();
		if (mPowerType == PowerType.PowerType_MoveBackwards && mUpdateCount % Common._M(4) == 0)
		{
			mCel = ((mCel == 0) ? (Res.GetImageByID(ResID.IMAGE_POWERUP_REVERSE_ANYCOLOR).mNumCols - 1) : (mCel - 1));
		}
		else if (mPowerType == PowerType.PowerType_Laser && mUpdateCount % Common._M(4) == 0)
		{
			mCel = (mCel + 1) % Res.GetImageByID(ResID.IMAGE_POWERUP_LAZER_ANYCOLOR).mNumRows;
		}
	}

	public void UpdateExplosion()
	{
		if (mExploding)
		{
			if (!mExplodingFromLightning && mUpdateCount % Common._M(2) == 0)
			{
				mExplodeFrame++;
			}
			if (mExplodeFrame >= 20 || mElectricExplodeOverlay.mLoopCount >= 3)
			{
				mShouldRemove = true;
			}
		}
	}

	public void SetFrame(int theFrame)
	{
		ResID id = (ResID)(1366 + mColorType);
		if (GameApp.gApp.mColorblind && mColorType == 3)
		{
			id = ResID.IMAGE_GREEN_BALL_CBM;
		}
		else if (GameApp.gApp.mColorblind && mColorType == 4)
		{
			id = ResID.IMAGE_PURPLE_BALL_CBM;
		}
		Image imageByID = Res.GetImageByID(id);
		int mNumRows = imageByID.mNumRows;
		int num = (int)mWayPoint + theFrame;
		num %= mNumRows;
		mStartFrame = mNumRows - num;
	}

	public void ForceFrame(int theFrame)
	{
		mStartFrame = theFrame;
	}

	public void IncFrame(int theInc)
	{
		ResID id = (ResID)(1366 + mColorType);
		if (GameApp.gApp.mColorblind && mColorType == 3)
		{
			id = ResID.IMAGE_GREEN_BALL_CBM;
		}
		else if (GameApp.gApp.mColorblind && mColorType == 4)
		{
			id = ResID.IMAGE_PURPLE_BALL_CBM;
		}
		Image imageByID = Res.GetImageByID(id);
		int mNumRows = imageByID.mNumRows;
		mStartFrame += theInc;
		mStartFrame %= mNumRows;
		if (mStartFrame < 0)
		{
			mStartFrame = mNumRows + mStartFrame;
		}
	}

	public void RandomizeFrame()
	{
		ResID id = (ResID)(1366 + mColorType);
		if (GameApp.gApp.mColorblind && mColorType == 3)
		{
			id = ResID.IMAGE_GREEN_BALL_CBM;
		}
		else if (GameApp.gApp.mColorblind && mColorType == 4)
		{
			id = ResID.IMAGE_PURPLE_BALL_CBM;
		}
		Image imageByID = Res.GetImageByID(id);
		mStartFrame = MathUtils.SafeRand() % imageByID.mNumRows;
	}

	public static void DeleteBallGlobals()
	{
		int num = 0;
		int num2 = 0;
		for (num = 0; num < 8; num++)
		{
			gBlendedBalls[num] = null;
			if (num < 6)
			{
				gBlendedBombLights[num] = null;
			}
			for (num2 = 0; num2 <= 14; num2++)
			{
				gBlendedPowerups[num2, num] = null;
			}
		}
		for (num2 = 0; num2 < 14; num2++)
		{
			gBlendedPowerupLights[num2] = null;
		}
	}

	public virtual void SyncState(DataSync sync)
	{
		sync.RegisterPointer(this);
		sync.SyncLong(ref mId);
		sync.SyncLong(ref mPowerGracePeriod);
		int theInt = (int)mLastPowerType;
		sync.SyncLong(ref theInt);
		mLastPowerType = (PowerType)theInt;
		sync.SyncLong(ref mColorType);
		sync.SyncFloat(ref mWayPoint);
		sync.SyncFloat(ref mRotation);
		sync.SyncFloat(ref mDestRotation);
		sync.SyncFloat(ref mRotationInc);
		sync.SyncFloat(ref mX);
		sync.SyncFloat(ref mY);
		sync.SyncBoolean(ref mInTunnel);
		sync.SyncLong(ref mMultOverlayAlpha);
		SexyFramework.Misc.Buffer buffer = sync.GetBuffer();
		if (sync.isWrite())
		{
			buffer.WriteBoolean(mMultFX != null);
			if (mMultFX != null)
			{
				Common.SerializePIEffect(mMultFX, sync);
			}
		}
		else
		{
			mMultFX = null;
			if (buffer.ReadBoolean())
			{
				mMultFX = new PIEffect();
				Common.DeserializePIEffect(mMultFX, sync);
			}
		}
		sync.SyncBoolean(ref mDoBossPulse);
		sync.SyncFloat(ref mRadius);
		sync.SyncLong(ref mPulseState);
		sync.SyncLong(ref mPulseTimer);
		sync.SyncLong(ref mCannonFrame);
		sync.SyncBoolean(ref mCollidesWithNext);
		sync.SyncBoolean(ref mNeedCheckCollision);
		sync.SyncBoolean(ref mSuckPending);
		sync.SyncBoolean(ref mShrinkClear);
		sync.SyncBoolean(ref mSuckFromCompacting);
		sync.SyncBoolean(ref mExplodingInTunnel);
		sync.SyncBoolean(ref mExploding);
		sync.SyncLong(ref mExplodeFrame);
		sync.SyncBoolean(ref mShouldRemove);
		sync.SyncBoolean(ref mIsCannon);
		sync.SyncLong(ref mUpdateCount);
		sync.SyncLong(ref mCel);
		sync.SyncLong(ref mSuckCount);
		sync.SyncBoolean(ref mSuckBack);
		sync.SyncLong(ref mBackwardsCount);
		sync.SyncFloat(ref mBackwardsSpeed);
		sync.SyncLong(ref mComboCount);
		sync.SyncLong(ref mComboScore);
		sync.SyncLong(ref mStartFrame);
		sync.SyncLong(ref mPowerCount);
		sync.SyncLong(ref mPowerFade);
		sync.SyncBoolean(ref mSpeedy);
		sync.SyncLong(ref mGapBonus);
		sync.SyncLong(ref mNumGaps);
		sync.SyncLong(ref mElectricOverlayCel);
		sync.SyncBoolean(ref mExplodingFromLightning);
		sync.SyncLong(ref mElectricExplodeOverlay.mLoopCount);
		sync.SyncLong(ref mElectricExplodeOverlay.mLayer2Cel);
		sync.SyncLong(ref mElectricExplodeOverlay.mLayer1Cel);
		SyncListComponents(sync, mOverlayPulse, clear: true);
		SyncListComponents(sync, mElectricOverlay, clear: true);
		SyncListComponents(sync, mElectricExplodeOverlay.mLayer1Alpha, clear: true);
		SyncListComponents(sync, mElectricExplodeOverlay.mLayer2Alpha, clear: true);
		SyncListComponents(sync, mElectricExplodeOverlay.mLayer1Scale, clear: true);
		theInt = (int)mPowerType;
		sync.SyncLong(ref theInt);
		mPowerType = (PowerType)theInt;
		theInt = (int)mDestPowerType;
		sync.SyncLong(ref theInt);
		mDestPowerType = (PowerType)theInt;
		sync.SyncPointer(this);
	}

	private void SyncListComponents(DataSync sync, List<Component> theList, bool clear)
	{
		if (sync.isRead())
		{
			if (clear)
			{
				theList.Clear();
			}
			long num = sync.GetBuffer().ReadLong();
			for (int i = 0; i < num; i++)
			{
				Component component = new Component();
				component.SyncState(sync);
				theList.Add(component);
			}
			return;
		}
		sync.GetBuffer().WriteLong(theList.Count);
		foreach (Component the in theList)
		{
			the.SyncState(sync);
		}
	}

	public void SetColorType(int theType)
	{
		mColorType = theType;
	}

	public void SetCollidesWithNext(bool collidesWithNext)
	{
		mCollidesWithNext = collidesWithNext;
	}

	public void DoLaserAnim(bool d, Gun g)
	{
		mDoLaserAnim = d;
		mFrog = g;
	}

	public void DoLaserAnim(bool d)
	{
		DoLaserAnim(d, null);
	}

	public void SetShrinkClear(bool shrink)
	{
		mShrinkClear = shrink;
	}

	public void SetSuckCount(int theCount, bool suck_back)
	{
		mSuckCount = theCount;
		mSuckBack = suck_back;
	}

	public void SetSuckCount(int theCount)
	{
		SetSuckCount(theCount, suck_back: true);
	}

	public void SetComboCount(int theCount, int theScore)
	{
		mComboCount = theCount;
		mComboScore = theScore;
	}

	public void SetBackwardsCount(int theCount)
	{
		mBackwardsCount = theCount;
	}

	public void SetBackwardsSpeed(float theSpeed)
	{
		mBackwardsSpeed = theSpeed;
	}

	public void SetNeedCheckCollision(bool needCheck)
	{
		mNeedCheckCollision = needCheck;
	}

	public void SetSuckPending(bool pending, bool compact)
	{
		mSuckPending = pending;
		mSuckFromCompacting = compact;
	}

	public void SetSuckPending(bool pending)
	{
		SetSuckPending(pending, compact: false);
	}

	public void SetGapBonus(int theBonus, int theNumGaps)
	{
		mGapBonus = (ushort)theBonus;
		mNumGaps = (ushort)theNumGaps;
	}

	public void SetRadius(float r)
	{
		mRadius = r;
	}

	public void SetIsCannon(bool isCannon)
	{
		mIsCannon = isCannon;
		mCannonFrame = 0;
	}

	public void SetSpeedy(bool speedy)
	{
		mSpeedy = speedy;
	}

	public void SetPowerCount(int c)
	{
		mPowerCount = c;
	}

	public bool GetSuckBack()
	{
		return mSuckBack;
	}

	public bool GetSpeedy()
	{
		return mSpeedy;
	}

	public bool Contains(int x, int y)
	{
		x -= (int)mX;
		y -= (int)mY;
		int num = GetRadius() - 3;
		if (x * x + y * y < num * num)
		{
			return true;
		}
		return false;
	}

	public bool GetShouldRemove()
	{
		return mShouldRemove;
	}

	public bool GetIsExploding()
	{
		return mExploding;
	}

	public bool GetIsCannon()
	{
		return mIsCannon;
	}

	public static int GetIdGen()
	{
		return mIdGen;
	}

	public float GetX()
	{
		return mX;
	}

	public float GetY()
	{
		return mY;
	}

	public float GetWayPoint()
	{
		return mWayPoint;
	}

	public int GetColorType()
	{
		return mColorType;
	}

	public float GetRotation()
	{
		return mRotation;
	}

	public float GetDestRotation()
	{
		return mDestRotation;
	}

	public Bullet GetBullet()
	{
		return mBullet;
	}

	public bool GetCollidesWithNext()
	{
		return mCollidesWithNext;
	}

	public bool GetShrinkClear()
	{
		return mShrinkClear;
	}

	public bool HasOverlays()
	{
		if (mPowerType == PowerType.PowerType_Max && mElectricOverlay.Count() <= 0 && mElectricExplodeOverlay.mLayer1Alpha.Count() <= 0)
		{
			return mMultFX != null;
		}
		return true;
	}

	public bool HasUnderlays()
	{
		if (!mDoLaserAnim || mExploding)
		{
			return mMultFX != null;
		}
		return true;
	}

	public int GetSuckCount()
	{
		return mSuckCount;
	}

	public int GetComboCount()
	{
		return mComboCount;
	}

	public int GetComboScore()
	{
		return mComboScore;
	}

	public int GetBackwardsCount()
	{
		return mBackwardsCount;
	}

	public float GetBackwardsSpeed()
	{
		return mBackwardsSpeed;
	}

	public bool GetNeedCheckCollision()
	{
		return mNeedCheckCollision;
	}

	public bool GetSuckPending()
	{
		return mSuckPending;
	}

	public bool GetSuckFromCompacting()
	{
		return mSuckFromCompacting;
	}

	public PowerType GetPowerType()
	{
		return mPowerType;
	}

	public PowerType GetDestPowerType()
	{
		return mDestPowerType;
	}

	public int GetListItr()
	{
		if (mList == null)
		{
			return -1;
		}
		return mList.IndexOf(this);
	}

	public int GetPowerCount()
	{
		return mPowerCount;
	}

	public int GetGapBonus()
	{
		return mGapBonus;
	}

	public int GetNumGaps()
	{
		return mNumGaps;
	}

	public int GetStartFrame()
	{
		return mStartFrame;
	}

	public int GetId()
	{
		return mId;
	}

	public SexyVector2 GetPos()
	{
		return new SexyVector2(mX, mY);
	}

	public int GetRadius()
	{
		return (int)mRadius;
	}

	public bool GetInTunnel()
	{
		return mInTunnel;
	}

	private static BlendedImage CreateBlendedPowerup(int thePowerupType, int theType, Image theImage, int cel)
	{
		int num = theType;
		if (GameApp.gApp.mColorblind && theType == 3)
		{
			num = 6;
		}
		else if (GameApp.gApp.mColorblind && theType == 4)
		{
			num = 7;
		}
		if (gBlendedPowerups[thePowerupType, num] == null)
		{
			Rect celRect = theImage.GetCelRect(cel);
			gBlendedPowerups[thePowerupType, num] = new BlendedImage((MemoryImage)theImage, celRect, rotated: false);
		}
		return gBlendedPowerups[thePowerupType, num];
	}

	private static BlendedImage CreateBlendedBall(int theType)
	{
		ResID id = (ResID)(1366 + theType);
		int num = theType;
		if (GameApp.gApp.mColorblind && theType == 3)
		{
			id = ResID.IMAGE_GREEN_BALL_CBM;
			num = 6;
		}
		else if (GameApp.gApp.mColorblind && theType == 4)
		{
			num = 7;
			id = ResID.IMAGE_PURPLE_BALL_CBM;
		}
		if (gBlendedBalls[num] == null)
		{
			MemoryImage memoryImage = (MemoryImage)Res.GetImageByID(id);
			_ = memoryImage.mWidth / memoryImage.mNumCols;
			_ = memoryImage.mHeight / memoryImage.mNumRows;
			int theCel = memoryImage.mNumRows / 2;
			Rect celRect = memoryImage.GetCelRect(theCel);
			gBlendedBalls[num] = new BlendedImage(memoryImage, celRect, rotated: false);
		}
		return gBlendedBalls[num];
	}

	private static int GetMultAlpha(int cel)
	{
		int num = Common._M(255);
		int num2 = Common._M(5);
		int num3 = num;
		Image imageByID = Res.GetImageByID(ResID.IMAGE_MULTIPLIER_BALL_BLUE);
		int num4 = imageByID.mNumRows * imageByID.mNumCols - num2;
		if (cel < num2)
		{
			num3 = num / num2 * cel;
		}
		else if (cel > num4)
		{
			num3 = num - num / num2 * (cel - num4);
		}
		if (num3 > num)
		{
			num3 = num;
		}
		else if (num3 < 0)
		{
			num3 = 0;
		}
		return num3;
	}
}
