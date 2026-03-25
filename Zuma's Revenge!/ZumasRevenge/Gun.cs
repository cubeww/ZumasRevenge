using System;
using System.Collections.Generic;
using System.Linq;
using JeffLib;
using Microsoft.Xna.Framework;
using SexyFramework;
using SexyFramework.AELib;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.PIL;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class Gun : PopAnimListener
{
	public enum BossState
	{
		Inked,
		Plagued
	}

	public class BallShotInfo
	{
		public Ball mBall;

		public SexyVector2 mShiftedPos;

		public BallShotInfo(Ball ball, SexyVector2 vShiftedPos)
		{
			mBall = ball;
			mShiftedPos = vShiftedPos;
		}
	}

	public static int TONGUE_Y1 = 50;

	public static int TONGUE_Y2 = 57;

	public static int TONGUE_YNOBALL = 60;

	public static int FROG_WIDTH = 147;

	public static int FROG_HEIGHT = 134;

	protected static int NUM_CANNON_SHADOWS = 5;

	protected static float aChevronSpeed = 0f;

	protected Transform mGlobalTranform = new Transform();

	private FPoint[] mVels = new FPoint[3]
	{
		new FPoint(0f, 0f),
		new FPoint(0f, 0f),
		new FPoint(0f, 0f)
	};

	private CumulativeTransform mCumTran = new CumulativeTransform();

	private SexyFramework.Misc.Point mCP = new SexyFramework.Misc.Point();

	private SexyFramework.Misc.Point mGp = new SexyFramework.Misc.Point();

	public BonusText mTempText = new BonusText();

	public List<LTSmokeParticle> mSmokeParticles = new List<LTSmokeParticle>();

	public PIEffect mCannonBlast;

	public Board mBoard;

	protected PopAnim mDarkFrogStun;

	protected bool mDarkFrogStunShort;

	protected PopAnim mFlameStun;

	protected bool mFlameStunShort;

	private SexyFramework.PIL.System mDizzyStars;

	private Composition mSickAnim;

	protected List<BeamComponent>[] mBeams = new List<BeamComponent>[3]
	{
		new List<BeamComponent>(),
		new List<BeamComponent>(),
		new List<BeamComponent>()
	};

	protected BeamComponent mElectricOrb = new BeamComponent();

	protected Bullet mBullet;

	protected Bullet mNextBullet;

	protected GunState mState;

	protected DeviceImage mLazer;

	protected List<Bullet> mCannonBullets = new List<Bullet>();

	protected List<Component> mLazerPulse = new List<Component>();

	protected List<ConfusionMark> mConfusionMarks = new List<ConfusionMark>();

	protected List<Bubble> mBubbles = new List<Bubble>();

	protected List<OrbPowerRing> mPowerRings = new List<OrbPowerRing>();

	protected List<SkeletonPowerOrb> mPowerOrbs = new List<SkeletonPowerOrb>();

	public PopAnim mLightningEffect;

	public int mBlinkCount;

	public int mBlinkTimer;

	public int[] mCannonBallShadows = new int[NUM_CANNON_SHADOWS];

	public int mCannonBallShadowPos;

	public int mBX;

	public int mBY;

	public int mDestX1;

	public int mDestY1;

	public int mDestX2;

	public int mDestY2;

	public int mDestTime;

	public int mDestCount;

	public float mCenterX;

	public float mCenterY;

	public float mCurX;

	public float mCurY;

	public float mSpitX;

	public float mSpitY;

	public float mSpitAlpha;

	public float mSpitVX;

	public float mSpitVY;

	public float mSpitAngle;

	public float[] mLazerFrogBackPulseAlpha = new float[4];

	public int mWidth;

	public int mHeight;

	public int mCannonCount;

	public int mLazerCount;

	public int mUpdateCount;

	public int mLastGuideX;

	public int mLastGuideY;

	public int mStunTimer;

	public int mShieldAnimCel;

	public int mSlowTimer;

	public int mStunSpinFrame;

	public int mStartingStunTime;

	public float mBossStateAlpha;

	public int mBossStateAlphaDir;

	public int mBossStateHoldTimer;

	public int mBossState;

	public float mAngle;

	public float mDestAngle;

	public float mStatePercent;

	public float mFireVel;

	public float mRecoilAmt;

	public float mLazerPercent;

	public float mBeamProjectedEndX;

	public float mBeamProjectedEndY;

	public float mBeamDistToTarget;

	public float mFarthestDistance;

	public float mCannonAngle;

	public float mBossDeathTX;

	public float mBossDeathTY;

	public float mBossDeathVX;

	public float mBossDeathVY;

	public bool mShowNextBall;

	public bool mDoingHop;

	public bool mDoElectricBeamShit;

	public bool mDoingCannonBlast;

	public List<FrogBody> mFrogStack = new List<FrogBody>();

	public FrogBody mCurrentBody = new FrogBody();

	public int mReloadPoint;

	public int mFirePoint;

	public int mBallPoint;

	public int mBallXOff;

	public int mBallYOff;

	public int mCannonRuneColor;

	public int mCannonRuneAlpha;

	public int mCannonLightness;

	public int mCannonState;

	public SexyVector2 mShotCorrectionTarget = default(SexyVector2);

	public float mShotCorrectionRad;

	public static void PreLayerDraw(Graphics g, Layer l, object data)
	{
		Gun gun = (Gun)data;
		gun.SickFrogPreLayerDraw(g, l);
	}

	public void PopAnimPlaySample(string theSampleName, int thePan, double theVolume, double theNumSteps)
	{
	}

	public PIEffect PopAnimLoadParticleEffect(string theEffectName)
	{
		return null;
	}

	public bool PopAnimObjectPredraw(int theId, Graphics g, PASpriteInst theSpriteInst, PAObjectInst theObjectInst, PATransform theTransform, SexyFramework.Graphics.Color theColor)
	{
		return true;
	}

	public bool PopAnimObjectPostdraw(int theId, Graphics g, PASpriteInst theSpriteInst, PAObjectInst theObjectInst, PATransform theTransform, SexyFramework.Graphics.Color theColor)
	{
		return true;
	}

	public ImagePredrawResult PopAnimImagePredraw(int theId, PASpriteInst theSpriteInst, PAObjectInst theObjectInst, PATransform theTransform, Image theImage, Graphics g, int theDrawCount)
	{
		return ImagePredrawResult.ImagePredraw_Normal;
	}

	public void PopAnimStopped(int theId)
	{
		if (mBullet == null || !mBullet.GetIsCannon())
		{
			mLightningEffect = null;
		}
		else
		{
			mLightningEffect.Play("Main");
		}
	}

	public void PopAnimCommand(int theId, string theCommand, string theParam)
	{
	}

	public bool PopAnimCommand(int theId, PASpriteInst theSpriteInst, string theCommand, string theParam)
	{
		PopAnimCommand(theId, theCommand, theParam);
		return true;
	}

	private static void RotateXY(ref float x, ref float y, float cx, float cy, float rad)
	{
		float num = x - cx;
		float num2 = y - cy;
		x = cx + num * (float)Math.Cos(rad) + num2 * (float)Math.Sin(rad);
		y = cy + num2 * (float)Math.Cos(rad) - num * (float)Math.Sin(rad);
	}

	protected void CalcAngle()
	{
		if (mBullet == null)
		{
			return;
		}
		float num = mCurY + (float)mReloadPoint;
		float num2 = mCurY + (float)mFirePoint;
		float num3 = mCurY + (float)mBallPoint;
		float x = mCurX - 2f;
		float y;
		if (mState == GunState.GunState_Normal)
		{
			y = num3;
		}
		else if (mState == GunState.GunState_Reloading)
		{
			y = num + (num3 - num) * mStatePercent;
		}
		else
		{
			if (!(mStatePercent <= 0.6f))
			{
				return;
			}
			y = num3 + (num2 - num3) * mStatePercent / 0.6f;
		}
		RotateXY(ref x, ref y, mCurX, mCurY, mAngle);
		if (mState == GunState.GunState_Reloading && Common.gSuckMode && (mBX != 0 || mBY != 0))
		{
			x = x * mStatePercent + (float)mBX * (1f - mStatePercent);
			y = y * mStatePercent + (float)mBY * (1f - mStatePercent);
		}
		mBullet.SetPos(x, y);
		mBullet.SetRotation(mAngle);
	}

	protected void SetAngleToDestAngle()
	{
		while (mDestAngle < 0f)
		{
			mDestAngle += 6.28318f;
		}
		while (mDestAngle > 6.28318f)
		{
			mDestAngle -= 6.28318f;
		}
		mAngle = mDestAngle;
	}

	protected void UpdateCannonFadeIn()
	{
		if (mCannonState == 1)
		{
			mCannonRuneAlpha += (int)Common._M(15f);
			if (mCannonRuneAlpha >= 255)
			{
				mCannonRuneAlpha = 255;
				mCannonState++;
			}
		}
		else if (mCannonState == 2)
		{
			mCannonLightness += (int)Common._M(5f);
			if (mCannonLightness >= 100)
			{
				mCannonLightness = 100;
				mCannonState++;
			}
		}
		else if (mCannonState == 3)
		{
			mCannonLightness -= (int)Common._M(10f);
			if (mCannonLightness <= 0)
			{
				mCannonLightness = 0;
				mCannonState = -1;
			}
		}
	}

	protected void SetupLazerBackPulse()
	{
		mLazerPulse.Clear();
		int num = Common._M(40);
		mLazerPulse.Add(new Component(0f, 255f, mUpdateCount, mUpdateCount + num));
		mLazerPulse.Add(new Component(255f, 0f, mUpdateCount + num + 1, mUpdateCount + num * 2 + 1));
	}

	protected void SetFrogType(FrogType t, bool current)
	{
		if (!GameApp.gApp.mShutdown)
		{
			GameApp.gApp.mSoundPlayer.Stop(Res.GetSoundByID(ResID.SOUND_LIGHTNING_LOOP));
		}
		FrogBody frogBody = null;
		if (current)
		{
			frogBody = mCurrentBody;
			frogBody.mAlpha = 255;
		}
		else
		{
			if (mFrogStack.Count() > 0 && mFrogStack[mFrogStack.Count() - 1].mType == t)
			{
				return;
			}
			if (mCurrentBody.mType == t)
			{
				mFrogStack.Clear();
				if (t == FrogType.FrogType_Lightning)
				{
					ResetBeams();
				}
				if (mDoElectricBeamShit)
				{
					mDoElectricBeamShit = false;
					mBoard.mLevel.DeactivateLightningEffects();
				}
				return;
			}
			frogBody = new FrogBody();
			mFrogStack.Add(frogBody);
			frogBody.mAlpha = 0;
		}
		frogBody.mTongueX = 52;
		frogBody.mCX = FROG_WIDTH / 2;
		frogBody.mCY = FROG_HEIGHT / 2;
		mReloadPoint = -20;
		mFirePoint = 8;
		mBallPoint = 31;
		mBallXOff = (mBallYOff = 0);
		frogBody.mNextBallX = (int)Common._M(62f);
		frogBody.mNextBallY = (int)Common._M(25f);
		frogBody.mShadow = Res.GetImageByID(ResID.IMAGE_FROG_SHADOW);
		frogBody.mTongue = Res.GetImageByID(ResID.IMAGE_FROG_TONGUE);
		frogBody.mType = t;
		if (mBullet != null && !mBullet.GetJustFired())
		{
			mBullet.SetIsCannon(isCannon: false);
		}
		if (mNextBullet != null && !mNextBullet.GetJustFired())
		{
			mNextBullet.SetIsCannon(isCannon: false);
		}
		if (t != FrogType.FrogType_Lazer && mBoard.GetGuideBall() != null)
		{
			mBoard.GetGuideBall().DoLaserAnim(d: false);
		}
		switch (t)
		{
		case FrogType.FrogType_Normal:
			frogBody.mEyes = Res.GetImageByID(ResID.IMAGE_FROG_NORMAL_EYES);
			frogBody.mLegs = Res.GetImageByID(ResID.IMAGE_FROG_NORMAL_LEGS);
			frogBody.mMouth = Res.GetImageByID(ResID.IMAGE_FROG_NORMAL_MOUTH);
			frogBody.mBody = Res.GetImageByID(ResID.IMAGE_FROG_NORMAL_BODY);
			frogBody.mMouthOffset = new SexyFramework.Misc.Point(Common._M(26), (t == FrogType.FrogType_Normal) ? ((int)Common._M1(79f)) : 82);
			if (t == FrogType.FrogType_Normal)
			{
				frogBody.mLegsOffset = new SexyFramework.Misc.Point(Common._M(2), (int)Common._M1(38f));
				mBallYOff = (int)Common._M(0f);
			}
			else
			{
				frogBody.mLegsOffset = new SexyFramework.Misc.Point(1, (int)Common._M(41f));
			}
			frogBody.mBodyOffset = new SexyFramework.Misc.Point((int)Common._M(16f), (int)Common._M1(4f));
			frogBody.mEyesOffset = new SexyFramework.Misc.Point(32, (int)Common._M(47f));
			mCannonState = -1;
			if (t == FrogType.FrogType_Lightning)
			{
				if (!GameApp.gApp.GetBoard().IsLoading())
				{
					GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_LIGHTNING_TRIGGER));
					GameApp.gApp.mSoundPlayer.Loop(Res.GetSoundByID(ResID.SOUND_LIGHTNING_LOOP));
				}
				InitBeams();
			}
			break;
		case FrogType.FrogType_Lightning:
			frogBody.mEyes = null;
			frogBody.mLegs = Res.GetImageByID(ResID.IMAGE_FROG_LIGHTNING_BOTTOM);
			frogBody.mMouth = null;
			frogBody.mBody = Res.GetImageByID(ResID.IMAGE_FROG_LIGHTNING_TOP);
			frogBody.mLegsOffset = new SexyFramework.Misc.Point((int)Common._M(11f), (int)Common._M1(78f));
			frogBody.mBodyOffset = new SexyFramework.Misc.Point((int)Common._M(0f), (int)Common._M1(0f));
			frogBody.mEyesOffset = new SexyFramework.Misc.Point((int)Common._M(32f), (int)Common._M1(47f));
			mCannonState = -1;
			if (!GameApp.gApp.GetBoard().IsLoading())
			{
				GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_LIGHTNING_TRIGGER));
				GameApp.gApp.mSoundPlayer.Loop(Res.GetSoundByID(ResID.SOUND_LIGHTNING_LOOP));
			}
			InitBeams();
			break;
		case FrogType.FrogType_Lazer:
		{
			for (int j = 0; j < 4; j++)
			{
				mLazerFrogBackPulseAlpha[j] = 0f;
			}
			if (!GameApp.gApp.GetBoard().IsLoading())
			{
				GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_LASER_TRIGGER));
			}
			mDoElectricBeamShit = false;
			frogBody.mEyes = null;
			frogBody.mLegs = null;
			frogBody.mMouth = null;
			frogBody.mBody = Res.GetImageByID(ResID.IMAGE_FROG_LAZER_BODY);
			frogBody.mBodyOffset = new SexyFramework.Misc.Point((int)Common._M(0f), (int)Common._M1(6f));
			frogBody.mEyesOffset = new SexyFramework.Misc.Point((int)Common._M(31f), (int)Common._M1(45f));
			frogBody.mLazerEyeLoop = Res.GetImageByID(ResID.IMAGE_FROG_LAZER_EYE_LOOP);
			frogBody.mCel = 0;
			if (mLazerPulse.Count == 0)
			{
				SetupLazerBackPulse();
			}
			mCannonState = -1;
			break;
		}
		case FrogType.FrogType_Cannon:
		{
			if (!GameApp.gApp.GetBoard().IsLoading())
			{
				GameApp.gApp.PlaySample(mBoard.LevelIsSkeletonBoss() ? Res.GetSoundByID(ResID.SOUND_SKELETONHIT_POWERUP) : Res.GetSoundByID(ResID.SOUND_CANNON_POWERUP));
			}
			mDoElectricBeamShit = false;
			frogBody.mEyes = Res.GetImageByID(ResID.IMAGE_FROG_CANNON_EYES);
			frogBody.mLegs = Res.GetImageByID(ResID.IMAGE_FROG_CANNON_LEGS);
			frogBody.mMouth = Res.GetImageByID(ResID.IMAGE_FROG_CANNON_MOUTH);
			frogBody.mBody = Res.GetImageByID(ResID.IMAGE_FROG_CANNON_BODY);
			frogBody.mLegsOffset = new SexyFramework.Misc.Point(1, 41);
			frogBody.mMouthOffset = new SexyFramework.Misc.Point((int)Common._M(26f), (int)Common._M1(74f));
			frogBody.mBodyOffset = new SexyFramework.Misc.Point(16, 5);
			frogBody.mEyesOffset = new SexyFramework.Misc.Point(32, 47);
			frogBody.mNextBallX = (int)Common._M(62f);
			frogBody.mNextBallY = (int)Common._M(25f);
			for (int i = 0; i < NUM_CANNON_SHADOWS; i++)
			{
				mCannonBallShadows[i] = 0;
			}
			mCannonBallShadowPos = 0;
			break;
		}
		}
	}

	protected void DrawFrogBase(Graphics g, FrogBody fb)
	{
		int num = fb.mAlpha;
		if (mBoard.GetDarkFrogLevelFadeInAlpha() >= 0)
		{
			num = 255 - mBoard.GetDarkFrogLevelFadeInAlpha();
			num = Math.Min(Math.Max(num, 0), 255);
		}
		g.PushState();
		if (num != 255 && num != -1)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, fb.mAlpha);
		}
		int num2 = (int)mCurX - fb.mCX + GameApp.gScreenShakeX;
		int num3 = (int)mCurY - fb.mCY + GameApp.gScreenShakeY;
		Rect celRect = fb.mShadow.GetCelRect(0);
		if (g.Is3D())
		{
			g.DrawImageRotatedF(fb.mShadow, Common._S(num2 + Common._M(-2)), Common._S((float)num3 - mRecoilAmt / 2f + (float)Common._M1(3)), mAngle, Common._S(fb.mCX), Common._S((float)fb.mCY + mRecoilAmt / 2f), celRect);
		}
		else
		{
			g.DrawImageRotated(fb.mShadow, Common._S(num2 + Common._M(-2)), (int)Common._S((float)num3 - mRecoilAmt / 2f + (float)Common._M1(3)), mAngle, Common._S(fb.mCX), (int)Common._S((float)fb.mCY + mRecoilAmt / 2f), celRect);
		}
		GameApp.gApp.GetBoard().DrawGuide(g);
		if (fb.mLegs != null)
		{
			if (mBossStateAlpha > 0f)
			{
				g.PushState();
				g.SetColorizeImages(colorizeImages: true);
				int val = (int)(255f - mBossStateAlpha);
				val = Math.Min(Math.Max(0, val), 255);
				g.SetColor(255, 255, 255, val);
			}
			celRect = fb.mLegs.GetCelRect(0);
			if (g.Is3D())
			{
				g.DrawImageRotated(fb.mLegs, Common._S(num2 + fb.mLegsOffset.mX), Common._S(num3 + fb.mLegsOffset.mY), mAngle, Common._S(fb.mCX - fb.mLegsOffset.mX), Common._S(fb.mCY - fb.mLegsOffset.mY), celRect);
			}
			else
			{
				g.DrawImageRotated(fb.mLegs, Common._S(num2 + fb.mLegsOffset.mX), Common._S(num3 + fb.mLegsOffset.mY), mAngle, Common._S(fb.mCX - fb.mLegsOffset.mX), Common._S(fb.mCY - fb.mLegsOffset.mY), celRect);
			}
			if (mBossStateAlpha > 0f)
			{
				g.PopState();
			}
		}
		if (mLightningEffect != null)
		{
			mLightningEffect.Draw(g);
		}
		if (fb.mMouth != null)
		{
			int num4 = 0;
			if (fb.mMouth.mNumRows > 1 && mState != GunState.GunState_Normal)
			{
				if (fb.mType == FrogType.FrogType_Cannon)
				{
					num4 = ((mState == GunState.GunState_Reloading) ? 1 : 0);
				}
				else
				{
					num4 = 1;
					if (num4 >= fb.mMouth.mNumRows)
					{
						num4 = fb.mMouth.mNumRows - 1;
					}
				}
			}
			if (mBossStateAlpha > 0f)
			{
				g.PushState();
				g.SetColorizeImages(colorizeImages: true);
				int num5 = (int)(255f - mBossStateAlpha);
				num5 = Math.Min(Math.Max(0, num), 255);
				g.SetColor(255, 255, 255, num5);
			}
			celRect = fb.mMouth.GetCelRect(num4);
			if (g.Is3D())
			{
				g.DrawImageRotatedF(fb.mMouth, Common._S(num2 + fb.mMouthOffset.mX), Common._S((float)(num3 + fb.mMouthOffset.mY) - mRecoilAmt), mAngle, Common._S(fb.mCX - fb.mMouthOffset.mX), Common._S((float)(fb.mCY - fb.mMouthOffset.mY) + mRecoilAmt), celRect);
			}
			else
			{
				g.DrawImageRotated(fb.mMouth, Common._S(num2 + fb.mMouthOffset.mX), (int)Common._S((float)(num3 + fb.mMouthOffset.mY) - mRecoilAmt), mAngle, Common._S(fb.mCX - fb.mMouthOffset.mX), (int)Common._S((float)(fb.mCY - fb.mMouthOffset.mY) + mRecoilAmt), celRect);
			}
			if (mBossStateAlpha > 0f)
			{
				g.PopState();
				if (mBossStateAlpha < 255f)
				{
					g.SetColorizeImages(colorizeImages: true);
				}
				g.SetColor(255, 255, 255, (int)mBossStateAlpha);
				Image[] array = new Image[2]
				{
					Res.GetImageByID(ResID.IMAGE_FROG_INKED_BOTTOM),
					Res.GetImageByID(ResID.IMAGE_PLAGUE_FROG_BOTTOM)
				};
				SexyFramework.Misc.Point[] array2 = new SexyFramework.Misc.Point[2]
				{
					new SexyFramework.Misc.Point(Common._M(42), Common._M1(16)),
					new SexyFramework.Misc.Point(Common._M2(42), Common._M3(16))
				};
				int mX = array2[mBossState].mX;
				int mY = array2[mBossState].mY;
				if (g.Is3D())
				{
					g.DrawImageRotatedF(array[mBossState], Common._S(num2 + mX), Common._S((float)(num3 + mY) - mRecoilAmt), mAngle, Common._S(fb.mCX - mX), Common._S((float)(fb.mCY - mY) + mRecoilAmt));
				}
				else
				{
					g.DrawImageRotatedF(array[mBossState], Common._S(num2 + mX), Common._S((float)(num3 + mY) - mRecoilAmt), mAngle, Common._S(fb.mCX - mX), Common._S((float)(fb.mCY - mY) + mRecoilAmt));
				}
				g.SetColorizeImages(colorizeImages: false);
			}
		}
		g.PopState();
	}

	protected void DrawFrogTongue(Graphics g, FrogBody fb)
	{
		if (fb.mType != FrogType.FrogType_Cannon && fb.mType != FrogType.FrogType_Lazer)
		{
			int num = fb.mAlpha;
			if (mBoard.GetDarkFrogLevelFadeInAlpha() >= 0)
			{
				num = 255 - mBoard.GetDarkFrogLevelFadeInAlpha();
				num = Math.Min(Math.Max(num, 0), 255);
			}
			g.PushState();
			if (num != 255 && num != -1)
			{
				g.SetColor(255, 255, 255, fb.mAlpha);
				g.SetColorizeImages(colorizeImages: true);
			}
			int num2 = int.MaxValue;
			int num3 = (int)mCurX - fb.mCX + GameApp.gScreenShakeX;
			int num4 = (int)mCurY - fb.mCY + GameApp.gScreenShakeY;
			switch (mState)
			{
			case GunState.GunState_Normal:
				num2 = TONGUE_Y2;
				break;
			case GunState.GunState_Firing:
				num2 = (int)((float)TONGUE_Y1 * mStatePercent + (float)TONGUE_Y2 * (1f - mStatePercent));
				break;
			case GunState.GunState_Reloading:
				num2 = (int)((float)TONGUE_Y2 * mStatePercent + (float)TONGUE_Y1 * (1f - mStatePercent));
				break;
			}
			if (mBullet == null)
			{
				num2 = TONGUE_YNOBALL;
			}
			if (g.Is3D())
			{
				g.DrawImageRotatedF(fb.mTongue, Common._S(num3 + fb.mTongueX), Common._S((float)(num4 + num2) - mRecoilAmt), mAngle, Common._S(fb.mCX - fb.mTongueX), Common._S((float)(fb.mCY - num2) + mRecoilAmt));
			}
			else
			{
				g.DrawImageRotated(fb.mTongue, Common._S(num3 + fb.mTongueX), (int)Common._S((float)(num4 + num2) - mRecoilAmt), mAngle, Common._S(fb.mCX - fb.mTongueX), (int)Common._S((float)(fb.mCY - num2) + mRecoilAmt));
			}
			g.PopState();
		}
	}

	protected void DrawFrogTop(Graphics g, FrogBody fb)
	{
		int num = (int)mCurX - fb.mCX + GameApp.gScreenShakeX;
		int num2 = (int)mCurY - fb.mCY + GameApp.gScreenShakeY;
		g.PushState();
		int num3 = fb.mAlpha;
		if (mBoard.GetDarkFrogLevelFadeInAlpha() >= 0)
		{
			num3 = 255 - mBoard.GetDarkFrogLevelFadeInAlpha();
			num3 = Math.Min(Math.Max(num3, 0), 255);
		}
		if (mDoingCannonBlast && mCannonBlast.IsActive())
		{
			g.PushState();
			mCannonBlast.Draw(g);
			g.PopState();
		}
		if (num3 != 255 && num3 != -1)
		{
			g.SetColor(255, 255, 255, fb.mAlpha);
			g.SetColorizeImages(colorizeImages: true);
		}
		else if (mFrogStack.size() > 0 && mFrogStack.back().mAlpha > 0)
		{
			g.SetColor(255, 255, 255, 255 - mFrogStack.back().mAlpha);
			g.SetColorizeImages(colorizeImages: true);
		}
		if (mBossStateAlpha > 0f)
		{
			g.PushState();
			g.SetColorizeImages(colorizeImages: true);
			int val = (int)(255f - mBossStateAlpha);
			val = Math.Min(Math.Max(0, val), 255);
			g.SetColor(255, 255, 255, val);
		}
		if (GameApp.gApp.mBoard != null && GameApp.gApp.mBoard.mIsHotFrogEnabled)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 242, 0, 255);
		}
		if (g.Is3D())
		{
			g.DrawImageRotatedF(fb.mBody, Common._S(num + fb.mBodyOffset.mX), Common._S((float)(num2 + fb.mBodyOffset.mY) - mRecoilAmt), mAngle, Common._S(fb.mCX - fb.mBodyOffset.mX), Common._S((float)(fb.mCY - fb.mBodyOffset.mY) + mRecoilAmt));
		}
		else
		{
			g.DrawImageRotated(fb.mBody, Common._S(num + fb.mBodyOffset.mX), (int)Common._S((float)(num2 + fb.mBodyOffset.mY) - mRecoilAmt), mAngle, Common._S(fb.mCX - fb.mBodyOffset.mX), (int)Common._S((float)(fb.mCY - fb.mBodyOffset.mY) + mRecoilAmt));
		}
		if (GameApp.gApp.mBoard != null && GameApp.gApp.mBoard.mIsHotFrogEnabled)
		{
			g.SetColorizeImages(colorizeImages: false);
		}
		if (mBossStateAlpha > 0f)
		{
			g.PopState();
		}
		if (mFrogStack.size() > 0 && mFrogStack.back().mAlpha > 0)
		{
			g.SetColorizeImages(colorizeImages: false);
		}
		if (mBossStateAlpha > 0f)
		{
			if (mBossStateAlpha < 255f)
			{
				g.SetColorizeImages(colorizeImages: true);
			}
			g.SetColor(255, 255, 255, (int)mBossStateAlpha);
			Image[] array = new Image[2]
			{
				Res.GetImageByID(ResID.IMAGE_FROG_INKED_TOP),
				Res.GetImageByID(ResID.IMAGE_PLAGUE_FROG_TOP)
			};
			SexyFramework.Misc.Point[] array2 = new SexyFramework.Misc.Point[2]
			{
				new SexyFramework.Misc.Point(Common._M(0), Common._M1(4)),
				new SexyFramework.Misc.Point(Common._M2(0), Common._M3(4))
			};
			int mX = array2[mBossState].mX;
			int mY = array2[mBossState].mY;
			if (g.Is3D())
			{
				g.DrawImageRotatedF(array[mBossState], Common._S(num + mX), Common._S((float)(num2 + mY) - mRecoilAmt), mAngle, Common._S(fb.mCX - mX), Common._S((float)(fb.mCY - mY) + mRecoilAmt));
			}
			else
			{
				g.DrawImageRotatedF(array[mBossState], Common._S(num + mX), Common._S((float)(num2 + mY) - mRecoilAmt), mAngle, Common._S(fb.mCX - mX), Common._S((float)(fb.mCY - mY) + mRecoilAmt));
			}
			g.SetColorizeImages(colorizeImages: false);
		}
		if (fb.mType == FrogType.FrogType_Cannon)
		{
			if (mState == GunState.GunState_Reloading)
			{
				Rect celRect = fb.mMouth.GetCelRect(2);
				if (g.Is3D())
				{
					g.DrawImageRotatedF(fb.mMouth, Common._S(num + fb.mMouthOffset.mX), Common._S((float)(num2 + fb.mMouthOffset.mY) - mRecoilAmt), mAngle, Common._S(fb.mCX - fb.mMouthOffset.mX), Common._S((float)(fb.mCY - fb.mMouthOffset.mY) + mRecoilAmt), celRect);
				}
				else
				{
					g.DrawImageRotated(fb.mMouth, Common._S(num + fb.mMouthOffset.mX), (int)Common._S((float)(num2 + fb.mMouthOffset.mY) - mRecoilAmt), mAngle, Common._S(fb.mCX - fb.mMouthOffset.mX), (int)Common._S((float)(fb.mCY - fb.mMouthOffset.mY) + mRecoilAmt), celRect);
				}
			}
		}
		else if (fb.mType == FrogType.FrogType_Lightning)
		{
			float num4 = Common._M(50) + fb.mBodyOffset.mX;
			float num5 = Common._M(-6) + fb.mBodyOffset.mY;
			int num6 = JeffLib.Common.GetAlphaFromUpdateCount(mUpdateCount, Common._M(64)) * Common._M1(4);
			if (num6 > 255)
			{
				num6 = 255;
			}
			if (g.Is3D())
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, num6);
				if (Common._M(0) == 0)
				{
					g.SetDrawMode(1);
				}
				g.DrawImageRotatedF(Res.GetImageByID(ResID.IMAGE_FROG_BOLT), Common._S((float)num + num4), Common._S((float)num2 + num5 - mRecoilAmt), mAngle, Common._S((float)fb.mCX - num4), Common._S((float)fb.mCY - num5 + mRecoilAmt));
				g.SetColorizeImages(colorizeImages: false);
				g.SetDrawMode(0);
			}
			else
			{
				g.DrawImageRotated(Res.GetImageByID(ResID.IMAGE_FROG_BOLT), (int)Common._S((float)num + num4), (int)Common._S((float)num2 + num5 - mRecoilAmt), mAngle, (int)Common._S((float)fb.mCX - num4), (int)Common._S((float)fb.mCY - num5 + mRecoilAmt));
			}
		}
		if ((mState != GunState.GunState_Normal || IsStunned() || mBlinkCount >= 0) && fb.mEyes != null && fb.mEyes.mNumRows > 1 && mBossStateAlpha <= 0f)
		{
			int num7 = 0;
			if (mBlinkCount >= 0)
			{
				num7 = ((mBlinkCount % 2 != 0) ? 1 : (-1));
			}
			else if (IsStunned())
			{
				num7 = ((fb.mEyes.mNumCols > fb.mEyes.mNumRows) ? (fb.mEyes.mNumCols - 1) : (fb.mEyes.mNumRows - 1));
			}
			else if (mState != GunState.GunState_Firing)
			{
				num7 = 1;
			}
			if (num7 >= 0)
			{
				Rect celRect2 = fb.mEyes.GetCelRect(num7);
				if (g.Is3D())
				{
					g.DrawImageRotatedF(fb.mEyes, Common._S(num + fb.mEyesOffset.mX), Common._S((float)(num2 + fb.mEyesOffset.mY) - mRecoilAmt), mAngle, Common._S(fb.mCX - fb.mEyesOffset.mX), Common._S((float)(fb.mCY - fb.mEyesOffset.mY) + mRecoilAmt), celRect2);
				}
				else
				{
					g.DrawImageRotated(fb.mEyes, Common._S(num + fb.mEyesOffset.mX), (int)Common._S((float)(num2 + fb.mEyesOffset.mY) - mRecoilAmt), mAngle, Common._S(fb.mCX - fb.mEyesOffset.mX), (int)Common._S((float)(fb.mCY - fb.mEyesOffset.mY) + mRecoilAmt), celRect2);
				}
			}
		}
		else if (fb.mType == FrogType.FrogType_Lazer)
		{
			bool flag = fb == mCurrentBody && mFrogStack.size() > 0 && mFrogStack.back().mAlpha > 0 && mFrogStack.back().mType != mCurrentBody.mType;
			int num8 = 0;
			if (flag)
			{
				num8 = 255 - mFrogStack.back().mAlpha;
			}
			int val2 = (int)Component.GetComponentValue(mLazerPulse, 0f, mUpdateCount);
			if (!flag)
			{
				if (fb.mAlpha >= 0)
				{
					Math.Min(fb.mAlpha, val2);
				}
			}
			else
			{
				Math.Min(num8, val2);
			}
			int num9 = Common._M(0);
			int num10 = Common._M(-4);
			Rect rect = default(Rect);
			g.PushState();
			if (flag)
			{
				g.SetColor(255, 255, 255, num8);
				g.SetColorizeImages(colorizeImages: true);
			}
			else
			{
				num8 = ((fb.mAlpha >= 0) ? fb.mAlpha : 255);
			}
			int num11 = Common._M(19);
			int num12 = Common._M(33);
			g.SetColorizeImages(colorizeImages: true);
			g.SetDrawMode(1);
			g.SetColor(255, 255, 0, num8);
			rect = Res.GetImageByID(ResID.IMAGE_FROG_LAZER_EYE_LOOP).GetCelRect(fb.mCel);
			if (g.Is3D())
			{
				g.DrawImageRotatedF(fb.mLazerEyeLoop, Common._S(num + num11), Common._S((float)(num2 + num12) - mRecoilAmt), mAngle, Common._S(fb.mCX - num11), Common._S((float)(fb.mCY - num12) + mRecoilAmt), rect);
			}
			else
			{
				g.DrawImageRotated(fb.mLazerEyeLoop, Common._S(num + num11), (int)Common._S((float)(num2 + num12) - mRecoilAmt), mAngle, Common._S(fb.mCX - num11), (int)Common._S((float)(fb.mCY - num12) + mRecoilAmt), rect);
			}
			g.PopState();
			if (flag)
			{
				g.SetColor(255, 255, 255, num8);
				g.SetColorizeImages(colorizeImages: true);
			}
			num9 = Common._M(40);
			num10 = Common._M(6);
			Image imageByID = Res.GetImageByID(ResID.IMAGE_FROG_BACKFLASH);
			int num13 = imageByID.mNumCols * imageByID.mNumRows;
			int num14 = num13 - mLazerCount;
			if (num14 < 0)
			{
				num14 = 0;
			}
			if (num14 < num13)
			{
				rect = imageByID.GetCelRect(num14);
				if (g.Is3D())
				{
					g.DrawImageRotatedF(imageByID, Common._S(num + num9), Common._S((float)(num2 + num10) - mRecoilAmt), mAngle, Common._S(fb.mCX - num9), Common._S((float)(fb.mCY - num10) + mRecoilAmt), rect);
				}
				else
				{
					g.DrawImageRotated(imageByID, Common._S(num + num9), (int)Common._S((float)(num2 + num10) - mRecoilAmt), mAngle, Common._S(fb.mCX - num9), (int)Common._S((float)(fb.mCY - num10) + mRecoilAmt), rect);
				}
			}
			g.SetColorizeImages(colorizeImages: false);
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_FROG_BACKPULSE);
			for (int i = 0; i < 4; i++)
			{
				if (mLazerFrogBackPulseAlpha[i] != 0f)
				{
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(255, 255, 255, (int)(flag ? Math.Min(mLazerFrogBackPulseAlpha[i], num8) : mLazerFrogBackPulseAlpha[i]));
					g.SetDrawMode(Common._M(1));
					rect = imageByID2.GetCelRect(i);
					num9 = Common._M(40) + Common._DS(Res.GetOffsetXByID(ResID.IMAGE_FROG_BACKPULSE));
					num10 = Common._M(6) + Common._DS(Res.GetOffsetXByID(ResID.IMAGE_FROG_BACKPULSE));
					if (g.Is3D())
					{
						g.DrawImageRotatedF(imageByID2, Common._S(num + num9), Common._S((float)(num2 + num10) - mRecoilAmt), mAngle, Common._S(fb.mCX - num9), Common._S((float)(fb.mCY - num10) + mRecoilAmt), rect);
					}
					else
					{
						g.DrawImageRotated(imageByID2, Common._S(num + num9), (int)Common._S((float)(num2 + num10) - mRecoilAmt), mAngle, Common._S(fb.mCX - num9), (int)Common._S((float)(fb.mCY - num10) + mRecoilAmt), rect);
					}
					g.SetColorizeImages(colorizeImages: false);
					g.SetDrawMode(0);
				}
			}
		}
		if ((mCannonState > 0 || fb.mType == FrogType.FrogType_Cannon) && fb.mType != FrogType.FrogType_Lazer)
		{
			int num15 = Common._M(0);
			int num16 = Common._M(-8);
			g.PushState();
			if (fb.mType != FrogType.FrogType_Lazer)
			{
				if (mCannonRuneAlpha != 255)
				{
					g.SetColor(255, 255, 255, mCannonRuneAlpha);
					g.SetColorizeImages(colorizeImages: true);
				}
				else
				{
					g.SetColorizeImages(colorizeImages: false);
				}
				num15 = Common._M(21);
				num16 = Common._M(6);
				ResID resID = ResID.IMAGE_FROG_RUNE_BLUE;
				Image imageByID3 = Res.GetImageByID(resID + mCannonRuneColor);
				if (g.Is3D())
				{
					g.DrawImageRotatedF(imageByID3, Common._S(num + num15), Common._S((float)(num2 + num16) - mRecoilAmt), mAngle, Common._S(fb.mCX - num15), Common._S((float)(fb.mCY - num16) + mRecoilAmt));
				}
				else
				{
					g.DrawImageRotated(imageByID3, Common._S(num + num15), (int)Common._S((float)(num2 + num16) - mRecoilAmt), mAngle, Common._S(fb.mCX - num15), (int)Common._S((float)(fb.mCY - num16) + mRecoilAmt));
				}
			}
			g.PopState();
		}
		g.PopState();
	}

	protected float GetBeamAngle()
	{
		return GetAngle() - 1.570795f;
	}

	protected void UpdateBeamVec(List<BeamComponent> v, bool remove)
	{
		for (int i = 0; i < v.size(); i++)
		{
			BeamComponent beamComponent = v[i];
			beamComponent.mX += beamComponent.mVX;
			beamComponent.mY += beamComponent.mVY;
			beamComponent.mDistTraveled += beamComponent.mV0;
			if (beamComponent.mAlphaDelta != 0 && ((mElectricOrb.mVX == 0f && mElectricOrb.mVY == 0f) || beamComponent.mDistTraveled <= mElectricOrb.mDistTraveled))
			{
				beamComponent.mColor.mAlpha += beamComponent.mAlphaDelta;
				if (beamComponent.mColor.mAlpha > 255 && beamComponent.mAlphaDelta > 0)
				{
					beamComponent.mAlphaDelta *= -1;
					beamComponent.mColor.mAlpha = 255;
				}
				else if (beamComponent.mColor.mAlpha < beamComponent.mMinAlpha && beamComponent.mAlphaDelta < 0)
				{
					if (mElectricOrb.mVX == 0f && mElectricOrb.mVY == 0f)
					{
						beamComponent.mAlphaDelta *= -1;
					}
					beamComponent.mColor.mAlpha = beamComponent.mMinAlpha;
				}
			}
			if (remove && ShouldRemoveBeamComponent(beamComponent))
			{
				v.RemoveAt(i);
				i--;
			}
		}
	}

	protected void UpdateBeamVec(List<BeamComponent> v)
	{
		UpdateBeamVec(v, remove: true);
	}

	protected void DrawBeamVec(Graphics g, List<BeamComponent> v, bool overlay)
	{
		for (int i = 0; i < v.size(); i++)
		{
			BeamComponent beamComponent = v[i];
			if (!(beamComponent.mDistTraveled >= mBeamDistToTarget) && (!overlay || !(beamComponent.mDistTraveled > mElectricOrb.mDistTraveled)))
			{
				g.PushState();
				if (beamComponent.mAdditive)
				{
					g.SetDrawMode(1);
				}
				if (beamComponent.mColor != SexyFramework.Graphics.Color.White)
				{
					g.SetColor(beamComponent.mColor);
					g.SetColorizeImages(colorizeImages: true);
				}
				SexyTransform2D sexyTransform2D = new SexyTransform2D(init: false);
				sexyTransform2D.RotateRad(GetBeamAngle());
				g.DrawImageMatrix(beamComponent.mImage, sexyTransform2D, Common._S(beamComponent.mX), Common._S(beamComponent.mY));
				g.PopState();
			}
		}
	}

	protected void DrawBeamVec(Graphics g, List<BeamComponent> v)
	{
		DrawBeamVec(g, v, overlay: false);
	}

	protected void EmitBeamA()
	{
		float num = 1.6f;
		float mVX = num * (float)Math.Cos(GetBeamAngle());
		float mVY = num * (0f - (float)Math.Sin(GetBeamAngle()));
		BeamComponent beamComponent = new BeamComponent();
		beamComponent.mX = GetCenterX();
		beamComponent.mY = GetCenterY();
		beamComponent.mDistTraveled = 0f;
		beamComponent.mV0 = num;
		beamComponent.mAdditive = true;
		beamComponent.mImage = (MemoryImage)Res.GetImageByID(ResID.IMAGE_FROG_BEAM_A);
		beamComponent.mColor = new SexyFramework.Graphics.Color(SexyFramework.Graphics.Color.White);
		beamComponent.mVY = mVY;
		beamComponent.mVX = mVX;
		beamComponent.mAlphaDelta = 0;
		mBeams[0].Insert(0, beamComponent);
	}

	protected void EmitBeamB()
	{
		float num = 2.5f;
		float mVX = num * (float)Math.Cos(GetBeamAngle());
		float mVY = num * (0f - (float)Math.Sin(GetBeamAngle()));
		BeamComponent beamComponent = new BeamComponent();
		beamComponent.mV0 = num;
		beamComponent.mAdditive = true;
		beamComponent.mImage = (MemoryImage)Res.GetImageByID(ResID.IMAGE_FROG_BEAM_B);
		beamComponent.mColor = new SexyFramework.Graphics.Color(255, 255, 255, 200);
		beamComponent.mVY = mVY;
		beamComponent.mVX = mVX;
		beamComponent.mAlphaDelta = 1;
		beamComponent.mMinAlpha = 200;
		beamComponent.mX = GetCenterX();
		beamComponent.mY = GetCenterY();
		beamComponent.mDistTraveled = 0f;
		mBeams[1].Insert(0, beamComponent);
	}

	protected void EmitBeamC()
	{
		float num = 0.5f;
		float mVX = num * (float)Math.Cos(GetBeamAngle());
		float mVY = num * (0f - (float)Math.Sin(GetBeamAngle()));
		BeamComponent beamComponent = new BeamComponent();
		beamComponent.mV0 = num;
		beamComponent.mImage = (MemoryImage)Res.GetImageByID(ResID.IMAGE_FROG_BEAM_C);
		beamComponent.mColor = new SexyFramework.Graphics.Color(96, 0, 150);
		beamComponent.mVY = mVY;
		beamComponent.mVX = mVX;
		beamComponent.mAlphaDelta = 0;
		beamComponent.mX = GetCenterX();
		beamComponent.mY = GetCenterY();
		beamComponent.mDistTraveled = 0f;
		beamComponent.mAdditive = true;
		mBeams[2].Insert(0, beamComponent);
	}

	protected void StepBeamUpdate(int count, bool remove)
	{
		for (int i = 0; i < 3; i++)
		{
			UpdateBeamVec(mBeams[i], remove);
		}
		if (mElectricOrb.mVX == 0f && mElectricOrb.mVY == 0f)
		{
			if ((float)count % Common._M(15f) == 0f)
			{
				EmitBeamA();
			}
			if ((float)count % (1f + (float)SexyFramework.Common.Rand() % Common._M(6f)) == 0f)
			{
				EmitBeamB();
			}
			if ((float)count % Common._M(75f) == 0f)
			{
				EmitBeamC();
			}
		}
	}

	protected void StepBeamUpdate(int count)
	{
		StepBeamUpdate(count, remove: true);
	}

	protected bool ShouldRemoveBeamComponent(BeamComponent bc)
	{
		if (((!(bc.mX >= mBeamProjectedEndX) || !MathUtils._geq(bc.mVX, 0f)) && (!(bc.mX <= mBeamProjectedEndX) || !MathUtils._leq(bc.mVX, 0f))) || ((!(bc.mY >= mBeamProjectedEndY) || !MathUtils._geq(bc.mVY, 0f)) && (!(bc.mY <= mBeamProjectedEndY) || !MathUtils._leq(bc.mVY, 0f))))
		{
			return bc.mDistTraveled > Common._SS(Common._M(1400f));
		}
		return true;
	}

	protected void RepositionBeamVec(List<BeamComponent> v, float delta)
	{
		if (v.Count() != 0)
		{
			float num = (float)Math.Cos(GetBeamAngle());
			float num2 = (float)Math.Sin(GetBeamAngle());
			float mVX = v[0].mV0 * num;
			float mVY = v[0].mV0 * (0f - num2);
			for (int i = 0; i < v.Count(); i++)
			{
				BeamComponent beamComponent = v[i];
				beamComponent.mVX = mVX;
				beamComponent.mVY = mVY;
				beamComponent.mX = (float)GetCenterX() + num * beamComponent.mDistTraveled;
				beamComponent.mY = (float)GetCenterY() - num2 * beamComponent.mDistTraveled;
			}
		}
	}

	protected void InitBeams()
	{
		for (int i = 0; i < 3; i++)
		{
			mBeams[i].Clear();
		}
		GameApp.gApp.GetBoard().GetGuideTargetCenter(out var x, out var y, lazer: true);
		mBeamDistToTarget = MathUtils.Distance(GetCenterX(), GetCenterY(), x, y);
		ResetBeams();
		mBeamProjectedEndX = (float)GetCenterX() + (float)Math.Cos(GetBeamAngle()) * mFarthestDistance;
		mBeamProjectedEndY = (float)GetCenterY() - (float)Math.Sin(GetBeamAngle()) * mFarthestDistance;
		int num = 0;
		while (mBeams[2].Count() == 0 || !ShouldRemoveBeamComponent(mBeams[2][mBeams[2].Count() - 1]))
		{
			StepBeamUpdate(num++, remove: false);
		}
	}

	protected void ResetBeams()
	{
		mDoElectricBeamShit = true;
		mElectricOrb.mImage = (MemoryImage)Res.GetImageByID(ResID.IMAGE_FROG_ELECTRIC_ORB);
		mElectricOrb.mX = GetCenterX();
		mElectricOrb.mY = GetCenterY();
		mElectricOrb.mDistTraveled = (mElectricOrb.mV0 = (mElectricOrb.mVX = (mElectricOrb.mVY = 0f)));
		mElectricOrb.mAdditive = true;
		mElectricOrb.mAlphaDelta = 0;
		mElectricOrb.mColor = new SexyFramework.Graphics.Color(SexyFramework.Graphics.Color.White);
		mElectricOrb.mCel = (mElectricOrb.mMinAlpha = 0);
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < mBeams[i].Count(); j++)
			{
				mBeams[i][j].mAlphaDelta = 0;
				mBeams[i][j].mMinAlpha = 0;
				mBeams[i][j].mColor.mAlpha = 255;
			}
		}
	}

	protected void DrawBeams(Graphics g)
	{
		DrawBeamVec(g, mBeams[2]);
		DrawBeamVec(g, mBeams[1]);
		DrawBeamVec(g, mBeams[0]);
		if (mElectricOrb.mVX != 0f || mElectricOrb.mVY != 0f)
		{
			DrawBeamVec(g, mBeams[2], overlay: true);
			DrawBeamVec(g, mBeams[1], overlay: true);
			DrawBeamVec(g, mBeams[0], overlay: true);
		}
		g.SetDrawMode(1);
		SexyTransform2D sexyTransform2D = new SexyTransform2D(init: false);
		sexyTransform2D.LoadIdentity();
		sexyTransform2D.Scale(2f, 2f);
		g.DrawImageMatrix(mElectricOrb.mImage, sexyTransform2D, mElectricOrb.mImage.GetCelRect(mElectricOrb.mCel), Common._S(mElectricOrb.mX), Common._S(mElectricOrb.mY));
		g.SetDrawMode(0);
	}

	protected void UpdateBeams()
	{
		StepBeamUpdate(mUpdateCount);
		if (mUpdateCount % Common._M(6) == 0)
		{
			mElectricOrb.mCel = (mElectricOrb.mCel + 1) % mElectricOrb.mImage.mNumRows;
		}
		mElectricOrb.mX += mElectricOrb.mVX;
		mElectricOrb.mY += mElectricOrb.mVY;
		mElectricOrb.mDistTraveled += mElectricOrb.mV0;
		if (mElectricOrb.mDistTraveled > mBeamDistToTarget)
		{
			mDoElectricBeamShit = false;
		}
	}

	protected void RetargetBeams(float angle_delta)
	{
		float x = -1f;
		float y = -1f;
		GameApp.gApp.GetBoard().GetGuideTargetCenter(out x, out y, lazer: true);
		x = (int)x;
		y = (int)y;
		if (x != (float)mLastGuideX || y != (float)mLastGuideY)
		{
			mLastGuideX = (int)x;
			mLastGuideY = (int)y;
			mBeamDistToTarget = MathUtils.Distance(GetCenterX(), GetCenterY(), x, y);
		}
		else if (angle_delta == 0f)
		{
			return;
		}
		float num = (float)Math.Cos(GetBeamAngle());
		float num2 = (float)Math.Sin(GetBeamAngle());
		mBeamProjectedEndX = (float)GetCenterX() + num * mFarthestDistance;
		mBeamProjectedEndY = (float)GetCenterY() - num2 * mFarthestDistance;
		for (int i = 0; i < 3; i++)
		{
			RepositionBeamVec(mBeams[i], angle_delta);
		}
		mElectricOrb.mVX = mElectricOrb.mV0 * num;
		mElectricOrb.mVY = mElectricOrb.mV0 * (0f - num2);
		mElectricOrb.mX = (float)GetCenterX() + num * (mElectricOrb.mDistTraveled + Common._M(20f));
		mElectricOrb.mY = (float)GetCenterY() - num2 * (mElectricOrb.mDistTraveled + Common._M(20f));
	}

	protected void DoBubbles(int num)
	{
		if ((mBoard.mLevel.mBoss == null || mBoard.mLevel.mZone == 5) && !mBoard.DoingLevelTransition() && mBoard.GetGameState() == GameState.GameState_Playing)
		{
			for (int i = 0; i < num; i++)
			{
				Bubble bubble = new Bubble();
				bubble.Init(Common._M(0), MathUtils.FloatRange(Common._M1(-1.2f), Common._M2(-0.5f)), MathUtils.FloatRange(Common._M3(0.05f), Common._M4(0.2f)), (int)MathUtils.FloatRange(Common._M5(15f), Common._M6(25f)));
				bubble.SetAlphaFade(Common._M(2f));
				float num2 = mCurX - (float)mCurrentBody.mCX + (float)mCurrentBody.mTongueX + (float)MathUtils.IntRange((int)Common._M(0f), (int)Common._M1(20f));
				float num3 = mCurY - (float)mCurrentBody.mCY + (float)TONGUE_Y2 + (float)Common._M(0);
				num2 += (float)Math.Cos((double)mAngle - 1.5707950592041016) * (float)MathUtils.IntRange((int)Common._M(30f), (int)Common._M1(50f));
				num3 -= (float)Math.Sin((double)mAngle - 1.5707950592041016) * (float)MathUtils.IntRange((int)Common._M(30f), (int)Common._M1(50f));
				bubble.SetX(num2);
				bubble.SetY(num3);
				mBubbles.Add(bubble);
			}
		}
	}

	public Gun(Board b)
	{
		mBoard = b;
		for (int i = 0; i < 4; i++)
		{
			mLazerFrogBackPulseAlpha[i] = 0f;
		}
		mCannonBlast = null;
		mDarkFrogStunShort = true;
		mDarkFrogStun = null;
		mLightningEffect = null;
		mFlameStunShort = false;
		mFlameStun = null;
		mCenterX = 320f;
		mCenterY = 240f;
		mStunSpinFrame = -1;
		mStartingStunTime = 0;
		mBX = 0;
		mBY = 0;
		mDestCount = 0;
		mBlinkTimer = 0;
		mDestTime = 1;
		mRecoilAmt = 0f;
		mCannonCount = 0;
		mLazerPercent = 0f;
		mLazerCount = 0;
		mSpitVX = (mSpitVY = (mSpitAngle = 0f));
		mSlowTimer = 0;
		mDoElectricBeamShit = false;
		mSpitX = (mSpitY = (mSpitAlpha = 0f));
		mBlinkCount = -1;
		mFarthestDistance = 0f;
		mLastGuideX = (mLastGuideY = -999);
		mElectricOrb.mDistTraveled = (mElectricOrb.mV0 = (mElectricOrb.mVX = (mElectricOrb.mVY = 0f)));
		mStunTimer = 0;
		mDizzyStars = null;
		mDoingCannonBlast = false;
		mBossDeathTX = (mBossDeathTY = (mBossDeathVX = (mBossDeathVY = 0f)));
		mLazer = new DeviceImage();
		mLazer.mApp = GameApp.gApp;
		mLazer.SetImageMode(hasTrans: true, hasAlpha: true);
		mLazer.AddImageFlags(16u);
		mLazer.Create(Common._S(20), Common._S(1000));
		Graphics graphics = new Graphics(mLazer);
		graphics.SetColor(0, 0, 0, 0);
		graphics.FillRect(0, 0, mLazer.mWidth, mLazer.mHeight);
		for (int j = 0; j < mLazer.mHeight; j += Common._S(2))
		{
			graphics.DrawImage(Res.GetImageByID(ResID.IMAGE_FROG_LAZER), 0, j);
		}
		graphics.ClearRenderContext();
		mWidth = 108;
		mHeight = 108;
		mAngle = 0f;
		mDestAngle = 0f;
		mDoingHop = false;
		mFireVel = 8f;
		mState = GunState.GunState_Normal;
		mShieldAnimCel = 0;
		mBullet = null;
		mNextBullet = null;
		mShowNextBall = true;
		mUpdateCount = 0;
		mBossStateAlpha = 0f;
		mBossStateAlphaDir = 0;
		mBossStateHoldTimer = 0;
		mBossState = -1;
		mCannonRuneColor = -1;
		mCannonState = 0;
		mCannonRuneAlpha = (mCannonLightness = 0);
		mSickAnim = new Composition();
		mSickAnim.mLoadImageFunc = GameApp.CompositionLoadFunc;
		mSickAnim.mPostLoadImageFunc = GameApp.CompositionPostLoadFunc;
		GameApp.mCompositionResPrefix = "_BOSS_DARKFROG";
		mSickAnim.LoadFromFile("pax\\SICKO");
		mSickAnim.mPreLayerDrawData = this;
		mSickAnim.mPreLayerDrawFunc = PreLayerDraw;
		GameApp.mCompositionResPrefix = "";
		SetFrogType(FrogType.FrogType_Normal, current: true);
		for (int k = 0; k < NUM_CANNON_SHADOWS; k++)
		{
			mCannonBallShadows[k] = 0;
		}
		mCannonBallShadowPos = 0;
		Res.GetImageByID(ResID.IMAGE_FROG_SPIN_FRAMES);
		Res.GetSoundByID(ResID.SOUND_FROG_STUNNED);
		Res.GetSoundByID(ResID.SOUND_NEW_BURNINGFROGLOOP);
		Res.GetSoundByID(ResID.SOUND_NEW_FIREHITFROG);
	}

	public virtual void Dispose()
	{
		mDizzyStars = null;
		mSickAnim = null;
		mFlameStun = null;
		mLazer = null;
		EmptyBullets();
		for (int i = 0; i < mBubbles.Count; i++)
		{
			mBubbles[i] = null;
		}
		mBubbles.Clear();
		for (int j = 0; j < mPowerRings.Count; j++)
		{
			mPowerRings[j] = null;
		}
		mPowerRings.Clear();
		for (int k = 0; k < mSmokeParticles.Count; k++)
		{
			mSmokeParticles[k] = null;
		}
	}

	public bool NeedsReload()
	{
		if (mNextBullet != null)
		{
			return mBullet == null;
		}
		return true;
	}

	public void DeleteBullet()
	{
		if (mBullet != null)
		{
			mBullet = mNextBullet;
			mNextBullet = null;
		}
	}

	public void ClearBubbles()
	{
		for (int i = 0; i < mBubbles.Count; i++)
		{
			mBubbles[i] = null;
		}
		mBubbles.Clear();
	}

	public void LevelReset()
	{
		mBossStateAlpha = 0f;
		mBossState = -1;
		mBossStateAlphaDir = 0;
		mBossStateHoldTimer = 0;
		mDizzyStars = null;
		SetCannonCount(0, stack: false, -1);
		SetFrogType(FrogType.FrogType_Normal, current: true);
		DoLightningFrog(is_lightning: false);
		mDoElectricBeamShit = false;
		mFrogStack.Clear();
		mLazerCount = 0;
		mLazerPercent = 0f;
		mStunTimer = 0;
		mSlowTimer = 0;
		mState = GunState.GunState_Normal;
		mConfusionMarks.Clear();
		ref SexyVector2 reference = ref mShotCorrectionTarget;
		float x = (mShotCorrectionTarget.y = 0f);
		reference.x = x;
		mShotCorrectionRad = 0f;
		mPowerRings.Clear();
		ClearBubbles();
	}

	public void SickFrogPreLayerDraw(Graphics g, Layer l)
	{
		int num = (int)mCurX - mCurrentBody.mCX + GameApp.gScreenShakeX;
		int num2 = (int)mCurY - mCurrentBody.mCY + GameApp.gScreenShakeY;
		if (mShowNextBall && mBoard.mLevel.mBoss.AllowFrogToFire() && mNextBullet != null && mState != GunState.GunState_Reloading && JeffLib.Common.StrFindNoCase(l.mLayerName, "top") != -1)
		{
			if (mBullet != null)
			{
				mBullet.Draw(g, mBallXOff + GameApp.gScreenShakeX - Common._DS(Common._M(3)), mBallYOff + GameApp.gScreenShakeY);
			}
			Image imageByID = Res.GetImageByID(ResID.IMAGE_NEXT_BALL);
			int theCel = mNextBullet.GetColorType();
			if (GameApp.gApp.mColorblind && mNextBullet.GetColorType() == 3)
			{
				theCel = 6;
			}
			else if (GameApp.gApp.mColorblind && mNextBullet.GetColorType() == 4)
			{
				theCel = 7;
			}
			Rect celRect = imageByID.GetCelRect(theCel);
			if (g.Is3D())
			{
				g.DrawImageRotatedF(imageByID, Common._S(num + mCurrentBody.mNextBallX), Common._S((float)(num2 + mCurrentBody.mNextBallY) - mRecoilAmt), mAngle, Common._S(mCurrentBody.mCX - mCurrentBody.mNextBallX), Common._S((float)(mCurrentBody.mCY - mCurrentBody.mNextBallY) + mRecoilAmt), celRect);
			}
			else
			{
				g.DrawImageRotated(imageByID, Common._S(num + mCurrentBody.mNextBallX), (int)Common._S((float)(num2 + mCurrentBody.mNextBallY) - mRecoilAmt), mAngle, Common._S(mCurrentBody.mCX - mCurrentBody.mNextBallX), (int)Common._S((float)(mCurrentBody.mCY - mCurrentBody.mNextBallY) + mRecoilAmt), celRect);
			}
		}
	}

	public void DoInkedState()
	{
		mBossStateAlphaDir = 1;
		mBossStateHoldTimer = Common._M(500);
		mBossState = 0;
	}

	public void DoPlaguedState()
	{
		mBossStateAlphaDir = 1;
		mBossStateHoldTimer = Common._M(300);
		mBossState = 1;
	}

	public void DrawConfusionMarks(Graphics g)
	{
		if (mBoard.GetGameState() != GameState.GameState_Losing)
		{
			g.SetColorizeImages(colorizeImages: true);
			for (int i = 0; i < mConfusionMarks.size(); i++)
			{
				ConfusionMark confusionMark = mConfusionMarks[i];
				g.SetColor(255, 255, 255, (int)confusionMark.mAlpha);
				g.DrawImage(confusionMark.mImage, (int)Common._S(confusionMark.mX + (float)GetCenterX()), (int)Common._S(confusionMark.mY + (float)GetCenterY()), (int)(confusionMark.mSize * (float)confusionMark.mImage.mWidth), (int)(confusionMark.mSize * (float)confusionMark.mImage.mHeight));
			}
			g.SetColorizeImages(colorizeImages: false);
		}
	}

	public void MoveToBossDeathPosition(float x, float y)
	{
		mBossDeathTX = x;
		mBossDeathTY = y;
		mBossDeathVX = (x - mCurX) / Common._M(200f);
		mBossDeathVY = (y - mCurY) / Common._M(200f);
	}

	public void Reload(int theType, bool delay, PowerType thePower)
	{
		Bullet bullet = new Bullet();
		bullet.mFrog = this;
		bullet.SetColorType(theType);
		bullet.SetPowerType(thePower, delay: false);
		mStatePercent = 0f;
		mBullet = null;
		mBullet = mNextBullet;
		if (mCannonCount > 0 && mBullet != null && !mBullet.GetIsCannon())
		{
			mBullet.SetIsCannon(isCannon: true);
			mCannonCount--;
		}
		mNextBullet = bullet;
		mState = GunState.GunState_Reloading;
		if (!delay)
		{
			mStatePercent = 1f;
			mState = GunState.GunState_Normal;
		}
		CalcAngle();
	}

	public void Reload(int theType)
	{
		Reload(theType, delay: false, PowerType.PowerType_Max);
	}

	public void Reload2(int theType, bool delay, PowerType thePower, int bx, int by)
	{
		Bullet bullet = new Bullet();
		bullet.mFrog = this;
		bullet.SetColorType(theType);
		bullet.SetPowerType(thePower, delay: false);
		if (thePower == PowerType.PowerType_Cannon)
		{
			bullet.SetIsCannon(isCannon: true);
		}
		mBX = bx;
		mBY = by;
		mStatePercent = 0f;
		mBullet = null;
		mBullet = bullet;
		mState = GunState.GunState_Reloading;
		if (!delay)
		{
			mStatePercent = 1f;
			mState = GunState.GunState_Normal;
		}
		CalcAngle();
	}

	public void Reload2(int theType)
	{
		Reload2(theType, delay: false, PowerType.PowerType_Max, 0, 0);
	}

	public void Reload3()
	{
		if (mBullet == null && mNextBullet != null)
		{
			mBX = 0;
			mBY = 0;
			mBullet = mNextBullet;
			mNextBullet = null;
			mStatePercent = 0f;
			mState = GunState.GunState_Reloading;
			CalcAngle();
		}
	}

	public void ClearLaserState()
	{
		if (mLazerCount > 0)
		{
			mLazerCount = 0;
			if (mFrogStack.Count() > 0 && mFrogStack[mFrogStack.Count() - 1].mType == FrogType.FrogType_Lazer)
			{
				mFrogStack.Clear();
			}
		}
	}

	public void SetAngle(float theAngle)
	{
		mAngle = (mDestAngle = theAngle);
		CalcAngle();
	}

	public void SetDestAngle(float theAngle)
	{
		while (mAngle < 0f)
		{
			mAngle += 6.28318f;
		}
		while (mAngle > 6.28318f)
		{
			mAngle -= 6.28318f;
		}
		float num = Math.Abs(theAngle - mAngle);
		if (num > 3.14159f)
		{
			theAngle = ((!(theAngle < mAngle)) ? (theAngle - 6.28318f) : (theAngle + 6.28318f));
		}
		mDestAngle = theAngle;
	}

	public void SetDestPos(int x, int y, int theSpeed, bool doingHop)
	{
		mDoingHop = doingHop;
		mDestX1 = (int)mCenterX;
		mDestY1 = (int)mCenterY;
		mDestX2 = x;
		mDestY2 = y;
		float num = new SexyVector2(mDestX2 - mDestX1, mDestY2 - mDestY1).Magnitude();
		mDestCount = (int)(num / (float)theSpeed);
		if (mDestCount < 1)
		{
			mDestCount = 1;
		}
		mDestTime = mDestCount;
		if (mBoard.mLevel.mZone == 5 && mBoard.mLevel.mNum != 10 && mDoingHop && mBoard.mLevel.mMoveType == 0)
		{
			DoBubbles((int)Common._M(5f));
		}
	}

	public void SetDestPos(int x, int y, int theSpeed)
	{
		SetDestPos(x, y, theSpeed, doingHop: false);
	}

	public void Draw(Graphics g, int clip_height)
	{
		if (!mBoard.CanDrawFrog())
		{
			return;
		}
		for (int i = 0; i < mSmokeParticles.size(); i++)
		{
			BambooTransition.DrawSmokeParticle(g, mSmokeParticles[i]);
		}
		int num = (int)mCurX - mCurrentBody.mCX + GameApp.gScreenShakeX;
		int num2 = (int)mCurY - mCurrentBody.mCY + GameApp.gScreenShakeY;
		if (GameApp.gApp.GetBoard() != null)
		{
			GameApp.gApp.GetBoard().DrawFatFingerGuide(g);
		}
		if (mBullet != null && GetType() == 1 && !mBoard.IsPaused() && !mBoard.LevelIsSkeletonBoss())
		{
			g.SetColor(Common._M(50), Common._M1(50), Common._M2(50), 100);
			float num3 = mDestAngle - 1.570795f;
			float num4 = num3 - mCannonAngle;
			float num5 = num3 + mCannonAngle;
			float num6 = (float)Math.Cos(num4);
			float num7 = 0f - (float)Math.Sin(num4);
			float num8 = (float)Math.Cos(num5);
			float num9 = 0f - (float)Math.Sin(num5);
			int num10 = (int)Common._S(mBullet.GetX() + (float)mBallXOff);
			int num11 = (int)Common._S(mBullet.GetY() + (float)mBallYOff);
			if (GameApp.mGameRes != 768)
			{
				num10 += (int)((float)mCurrentBody.mBody.mHeight * 0.5f);
			}
			Graphics3D graphics3D = g.Get3D();
			if (graphics3D == null)
			{
				SexyFramework.Misc.Point point = new SexyFramework.Misc.Point((int)((float)Common._S(Common._M(1000)) * num6 + (float)num10), (int)((float)Common._S(Common._M1(1000)) * num7 + (float)num11));
				SexyFramework.Misc.Point point2 = new SexyFramework.Misc.Point((int)((float)Common._S(Common._M(1000)) * num8 + (float)num10), (int)((float)Common._S(Common._M1(1000)) * num9 + (float)num11));
				SexyFramework.Misc.Point[] theVertexList = new SexyFramework.Misc.Point[3]
				{
					new SexyFramework.Misc.Point(num10, num11),
					point,
					point2
				};
				if (mCannonAngle != 0f)
				{
					g.PolyFill(theVertexList, 3, convex: false);
				}
			}
			else
			{
				DrawCannonPaths(g, num3, num10 + mBoard.mApp.mBoardOffsetX / 2 - 10, num11);
			}
			if (!g.Is3D())
			{
				g.SetColorizeImages(colorizeImages: true);
				mVels[0].mX = num6;
				mVels[0].mY = num7;
				mVels[1].mX = (float)Math.Cos(num3);
				mVels[1].mY = 0f - (float)Math.Sin(num3);
				mVels[2].mX = num8;
				mVels[2].mY = num9;
				Image imageByID = Res.GetImageByID(ResID.IMAGE_CANNON_BALL);
				for (int j = 0; j < NUM_CANNON_SHADOWS; j++)
				{
					if (mCannonBallShadows[j] > 0)
					{
						g.SetColor(Common._M(128), Common._M1(128), Common._M2(128), mCannonBallShadows[j]);
						for (int k = 0; k < 3; k++)
						{
							g.DrawImage(imageByID, (int)((float)(num10 - imageByID.mWidth / 2) + mVels[k].mX * (float)(j + 1) * (float)Common._S(Common._M(80))), (int)((float)(num11 - imageByID.mHeight / 2) + mVels[k].mY * (float)(j + 1) * (float)Common._S(Common._M(80))));
						}
					}
				}
				g.SetColorizeImages(colorizeImages: false);
			}
		}
		if ((mStunTimer > 0 && mBlinkCount < 0) || mFlameStun != null || mDarkFrogStun != null)
		{
			if (mFlameStun != null)
			{
				mFlameStun.Draw(g);
			}
			else if (mDarkFrogStun != null)
			{
				mDarkFrogStun.Draw(g);
			}
			else
			{
				Image imageByID2 = Res.GetImageByID(ResID.IMAGE_FROG_SPIN_FRAMES);
				Rect celRect = imageByID2.GetCelRect(mStunSpinFrame);
				g.DrawImageRotated(imageByID2, (int)(Common._S(mCenterX) - (float)(celRect.mWidth / 2) + (float)Common._M(0)), (int)(Common._S(mCenterY) - (float)(celRect.mHeight / 2)), mAngle, celRect);
			}
		}
		else if (!IsPoisoned())
		{
			g.PushState();
			if (clip_height != 0)
			{
				g.ClipRect(0, 0, GameApp.gApp.mWidth, (int)Common._S(mCenterY + (float)mHeight + (float)clip_height));
			}
			DrawFrogBase(g, mCurrentBody);
			for (int l = 0; l < mFrogStack.size(); l++)
			{
				if (mFrogStack[l].mAlpha > 0)
				{
					DrawFrogBase(g, mFrogStack[l]);
				}
			}
			DrawFrogTongue(g, mCurrentBody);
			for (int m = 0; m < mFrogStack.size(); m++)
			{
				if (mFrogStack[m].mAlpha > 0)
				{
					DrawFrogTongue(g, mFrogStack[m]);
				}
			}
			g.PopState();
		}
		else
		{
			Rect celRect2 = mCurrentBody.mShadow.GetCelRect(0);
			Common._S(mCenterX - mCurX);
			if (g.Is3D())
			{
				g.DrawImageRotatedF(mCurrentBody.mShadow, Common._S(num + Common._M(-2)), Common._S((float)num2 - mRecoilAmt / 2f + (float)Common._M1(3)), mAngle, Common._S(mCurrentBody.mCX), Common._S((float)mCurrentBody.mCY + mRecoilAmt / 2f), celRect2);
			}
			else
			{
				g.DrawImageRotated(mCurrentBody.mShadow, Common._S(num + Common._M(-2)), (int)Common._S((float)num2 - mRecoilAmt / 2f + (float)Common._M1(3)), mAngle, Common._S(mCurrentBody.mCX), (int)Common._S((float)mCurrentBody.mCY + mRecoilAmt / 2f), celRect2);
			}
			float tx = Common._S(mCurX) + (float)Common._DS(Common._M(0));
			float ty = Common._S(mCurY) + (float)Common._DS(Common._M(8));
			mCumTran.Reset();
			mCumTran.mTrans.Translate((float)(-mSickAnim.mWidth) * Common._DS(1f), (float)(-mSickAnim.mHeight) * Common._DS(1f));
			mCumTran.mTrans.RotateRad(mAngle);
			mCumTran.mTrans.Translate(tx, ty);
			mSickAnim.Draw(g, mCumTran, -1, Common._DS(1f));
		}
		if (mSpitAlpha > 0f && mFlameStun == null && mDarkFrogStun == null)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)mSpitAlpha);
			mGlobalTranform.Reset();
			mGlobalTranform.RotateRad(mSpitAngle - (float)Math.PI * Common._M(0.5f));
			g.DrawImageTransform(Res.GetImageByID(ResID.IMAGE_FROG_SLOBBER), mGlobalTranform, Common._S(mSpitX), Common._S(mSpitY));
			g.SetColorizeImages(colorizeImages: false);
		}
		if (mDoElectricBeamShit && mBoard.mChallengeHelp == null && GameApp.gApp.mGenericHelp == null && !mBoard.IsPaused())
		{
			DrawBeams(g);
		}
		g.PushState();
		if (clip_height != 0)
		{
			g.ClipRect(0, 0, GameApp.gApp.mWidth, (int)Common._S(mCenterY + (float)mHeight + (float)clip_height));
		}
		if ((mStunTimer <= 0 || (mBlinkCount >= 0 && mFlameStun == null && mDarkFrogStun == null)) && !IsPoisoned())
		{
			if (mBullet != null && !LaserMode() && !LightningMode() && (mBoard.mLevel.mBoss == null || mBoard.mLevel.mBoss.AllowFrogToFire()))
			{
				mBullet.Draw(g, mBallXOff + GameApp.gScreenShakeX, mBallYOff + GameApp.gScreenShakeY);
			}
			g.DrawImageRotated(Res.GetImageByID(ResID.IMAGE_FROG_BALLBACK), Common._S(num + mCurrentBody.mNextBallX), (int)Common._S((float)(num2 + mCurrentBody.mNextBallY) - mRecoilAmt), mAngle, Common._S(mCurrentBody.mCX - mCurrentBody.mNextBallX), (int)Common._S((float)(mCurrentBody.mCY - mCurrentBody.mNextBallY) + mRecoilAmt));
			if (mShowNextBall && (mBoard.mLevel.mBoss == null || mBoard.mLevel.mBoss.AllowFrogToFire()) && !LaserMode() && !LightningMode() && mNextBullet != null && (mState != GunState.GunState_Reloading || Common.gSuckMode))
			{
				Image imageByID3 = Res.GetImageByID(ResID.IMAGE_NEXT_BALL);
				int theCel = mNextBullet.GetColorType();
				if (GameApp.gApp.mColorblind && mNextBullet.GetColorType() == 3)
				{
					theCel = 6;
				}
				else if (GameApp.gApp.mColorblind && mNextBullet.GetColorType() == 4)
				{
					theCel = 7;
				}
				Rect celRect3 = imageByID3.GetCelRect(theCel);
				if (g.Is3D())
				{
					g.DrawImageRotatedF(imageByID3, Common._S(num + mCurrentBody.mNextBallX), Common._S((float)(num2 + mCurrentBody.mNextBallY) - mRecoilAmt), mAngle, Common._S(mCurrentBody.mCX - mCurrentBody.mNextBallX), Common._S((float)(mCurrentBody.mCY - mCurrentBody.mNextBallY) + mRecoilAmt), celRect3);
				}
				else
				{
					g.DrawImageRotated(imageByID3, Common._S(num + mCurrentBody.mNextBallX), (int)Common._S((float)(num2 + mCurrentBody.mNextBallY) - mRecoilAmt), mAngle, Common._S(mCurrentBody.mCX - mCurrentBody.mNextBallX), (int)Common._S((float)(mCurrentBody.mCY - mCurrentBody.mNextBallY) + mRecoilAmt), celRect3);
				}
			}
			DrawFrogTop(g, mCurrentBody);
			for (int n = 0; n < mFrogStack.size(); n++)
			{
				if (mFrogStack[n].mAlpha > 0)
				{
					DrawFrogTop(g, mFrogStack[n]);
				}
			}
		}
		g.PopState();
		if (LaserMode() && !mBoard.IsPaused())
		{
			PIEffect[] array = new PIEffect[2]
			{
				mBoard.mLazerBeam[0],
				mBoard.mLazerBeam[1]
			};
			GameApp.gApp.GetBoard().GetGuideTargetCenter(out var x, out var y, lazer: true);
			float pAngle = GetAngle() - 3.14159f;
			mGp.mX = (int)Common._S(x);
			mGp.mY = (int)Common._S(y);
			float x2 = Common._S(GetCenterX()) - Common._S(Common._M(24));
			float y2 = Common._S(GetCenterY()) - Common._S(Common._M(0));
			JeffLib.Common.RotatePoint(pAngle, ref x2, ref y2, Common._S(GetCenterX()), Common._S(GetCenterY()));
			mCP.mX = (int)x2;
			mCP.mY = (int)y2;
			float num12 = MathUtils.Distance(mCP, mGp, sqrt: true);
			float num13 = num12;
			float num14 = num12 * mLazerPercent;
			Rect theSrcRect = new Rect(0, 0, Common._S(20), (int)num14);
			if (num14 > (float)mLazer.mHeight)
			{
				num14 = mLazer.mHeight;
			}
			theSrcRect.mHeight = (int)num14;
			float[] array2 = new float[2]
			{
				CommonMath.AngleBetweenPoints(mCP, mGp),
				0f
			};
			g.SetDrawMode(1);
			if (g.Is3D())
			{
				g.DrawImageRotatedF(mLazer, Common._S(GetCenterX() - Common._M(32)), (float)Common._S(GetCenterY()) - num14, array2[0] - 1.570795f, Common._S(Common._M1(32)), num14, theSrcRect);
			}
			else
			{
				g.DrawImageRotated(mLazer, Common._S(GetCenterX() - Common._M(32)), (int)((float)Common._S(GetCenterY()) - num14), array2[0] - 1.570795f, Common._S(Common._M1(32)), (int)num14, theSrcRect);
			}
			PIEmitterInstance emitter = array[0].GetLayer(0).GetEmitter(0);
			emitter.mEmitterInstanceDef.mPoints[1].mValuePoint2DVector[0].mValue = new Vector2(0f, 0f - num13);
			x2 = Common._S(GetCenterX() - Common._M(-26));
			y2 = Common._S(GetCenterY() - Common._M(0));
			JeffLib.Common.RotatePoint(pAngle, ref x2, ref y2, Common._S(GetCenterX()), Common._S(GetCenterY()));
			mCP.mX = (int)x2;
			mCP.mY = (int)y2;
			array2[1] = CommonMath.AngleBetweenPoints(mCP, mGp);
			if (g.Is3D())
			{
				g.DrawImageRotatedF(mLazer, Common._S(GetCenterX() - Common._M(-15)), (float)Common._S(GetCenterY()) - num14, array2[1] - 1.570795f, Common._S(Common._M1(-15)), num14, theSrcRect);
			}
			else
			{
				g.DrawImageRotated(mLazer, Common._S(GetCenterX() - Common._M(-15)), (int)((float)Common._S(GetCenterY()) - num14), array2[1] - 1.570795f, Common._S(Common._M1(-15)), (int)num14, theSrcRect);
			}
			g.SetDrawMode(0);
			emitter = array[1].GetLayer(0).GetEmitter(0);
			emitter.mEmitterInstanceDef.mPoints[1].mValuePoint2DVector[0].mValue = new Vector2(0f, 0f - num13);
			g.SetColor(SexyFramework.Graphics.Color.White);
			g.FillRect(mGp.mX, mGp.mY, 4, 4);
			for (int num15 = 0; num15 <= 1; num15++)
			{
				emitter = array[num15].GetLayer(0).GetEmitter(0);
				for (PIParticleInstance pIParticleInstance = emitter.mParticleGroup.mHead; pIParticleInstance != null; pIParticleInstance = pIParticleInstance.mNext)
				{
					if (MathUtils.Distance(new SexyFramework.Misc.Point(0, 0), new SexyFramework.Misc.Point((int)pIParticleInstance.mEmittedPos.X, (int)pIParticleInstance.mEmittedPos.Y), sqrt: true) > num13)
					{
						pIParticleInstance.mLife = 0f;
					}
				}
				array[num15].mDrawTransform.LoadIdentity();
				float num16 = GameApp.DownScaleNum(1f);
				array[num15].mDrawTransform.Scale(num16, num16);
				array[num15].mDrawTransform.Translate(Common._S((num15 == 0) ? (-22) : 24), 0f);
				array[num15].mDrawTransform.RotateRad(array2[num15] - 1.5705f);
				array[num15].mDrawTransform.Translate(Common._S(GetCenterX()), Common._S(GetCenterY()));
				array[num15].Draw(g);
			}
			mBoard.mLazerBurn.mDrawTransform.LoadIdentity();
			mBoard.mLazerBurn.mDrawTransform.Scale(Common._DS(1.4f), Common._DS(1.4f));
			mBoard.mLazerBurn.mDrawTransform.Translate(mGp.mX, mGp.mY);
			mBoard.mLazerBurn.Draw(g);
		}
		else if (LightningMode() && !mBoard.IsPaused())
		{
			GameApp.gApp.GetBoard().GetGuideTargetCenter(out var _, out var _, lazer: true);
			g.SetColor(0, 0, 255);
			float pAngle2 = GetAngle() - 3.14159f;
			float x4 = Common._S(GetCenterX() + Common._M(0));
			float y4 = Common._S(GetCenterY() + Common._M(-30));
			JeffLib.Common.RotatePoint(pAngle2, ref x4, ref y4, GetCenterX(), GetCenterY());
		}
		if (IsFuckedUp() && !mBoard.IsPaused())
		{
			DrawConfusionMarks(g);
		}
		if (!mBoard.IsPaused() && mBoard.GetGameState() != GameState.GameState_Losing)
		{
			Font fontByID = Res.GetFontByID(ResID.FONT_MAIN22);
			if (mBoard.mLevel.mInvertMouseTimer > 0)
			{
				g.SetFont(fontByID);
				g.SetColor(SexyFramework.Graphics.Color.White);
				g.DrawString(TextManager.getInstance().getString(471), Common._S(GetCenterX() - Common._M(30)), Common._S(GetCenterY() - Common._M1(80)));
			}
			if (mSlowTimer > 0)
			{
				g.SetFont(fontByID);
				g.SetColor(SexyFramework.Graphics.Color.White);
				g.DrawString(TextManager.getInstance().getString(472), Common._S(GetCenterX() - Common._M(47)), Common._S(GetCenterY() - Common._M1(80)));
			}
			if (mBoard.GetHallucinateTimer() > 0)
			{
				mTempText.Draw(g);
			}
		}
		if (mBoard.GetGameState() != GameState.GameState_Losing)
		{
			for (int num17 = 0; num17 < mBubbles.size(); num17++)
			{
				mBubbles[num17].Draw(g);
			}
		}
		for (int num18 = 0; num18 < mPowerOrbs.size(); num18++)
		{
			SkeletonPowerOrb skeletonPowerOrb = mPowerOrbs[num18];
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)skeletonPowerOrb.mAlpha);
			Image imageByID4 = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_GLOWBALL);
			int num19 = Common._M(-25);
			int num20 = Common._M(-20);
			float num21 = (float)imageByID4.GetCelWidth() * skeletonPowerOrb.mSize;
			float num22 = (float)imageByID4.GetCelHeight() * skeletonPowerOrb.mSize;
			g.DrawImage(imageByID4, (int)(Common._S(mCurX + (float)num19) + (float)(imageByID4.GetCelWidth() / 2) - num21 / 2f), (int)(Common._S(mCurY + (float)num20) + (float)(imageByID4.GetCelHeight() / 2) - num22 / 2f), (int)num21, (int)num22);
			g.SetDrawMode(1);
			g.DrawImage(imageByID4, (int)(Common._S(mCurX + (float)num19) + (float)(imageByID4.GetCelWidth() / 2) - num21 / 2f), (int)(Common._S(mCurY + (float)num20) + (float)(imageByID4.GetCelHeight() / 2) - num22 / 2f), (int)num21, (int)num22);
			g.SetColorizeImages(colorizeImages: false);
			g.SetDrawMode(0);
		}
		for (int num23 = 0; num23 < mPowerRings.size(); num23++)
		{
			mPowerRings[num23].Draw(g, (int)Common._S(mCurX), (int)Common._S(mCurY));
		}
		if (mDizzyStars != null && mBoard.GetGameState() != GameState.GameState_Losing)
		{
			mDizzyStars.Draw(g);
		}
	}

	public void Draw(Graphics g)
	{
		Draw(g, 0);
	}

	public void DrawCannonPaths(Graphics g, float inAngle, int inX, int inY)
	{
		FPoint pathOrigin = GetPathOrigin(inAngle, inX, inY);
		float num = Common._M(0.21f);
		Graphics3D graphics3D = g.Get3D();
		g.SetColorizeImages(colorizeImages: true);
		graphics3D.SetTexture(0, Res.GetImageByID(ResID.IMAGE_FROG_CANNON_CHEVRONS));
		graphics3D.SetTextureWrap(0, inWrap: true);
		DrawBulletPath(graphics3D, inAngle - num, pathOrigin.mX, pathOrigin.mY);
		DrawBulletPath(graphics3D, inAngle, pathOrigin.mX, pathOrigin.mY);
		DrawBulletPath(graphics3D, inAngle + num, pathOrigin.mX, pathOrigin.mY);
		graphics3D.SetTextureWrap(0, inWrap: false);
		g.SetColorizeImages(colorizeImages: false);
	}

	public FPoint GetPathOrigin(float inAngle, int inX, int inY)
	{
		float num = (float)Math.Pow(mCurrentBody.mBody.mWidth / 2, 2.0);
		float num2 = (float)Math.Pow(mCurrentBody.mBody.mHeight / 2, 2.0);
		float num3 = (float)Math.Sqrt(num + num2);
		return new FPoint((float)inX - num3 * (float)Math.Cos(inAngle), (float)inY + num3 * (float)Math.Sin(inAngle));
	}

	public void DrawBulletPath(Graphics3D g3D, float inAngle, float inX, float inY)
	{
		float num = Common._M(0.035f);
		float num2 = Common._M(6f);
		float num3 = Common._DS(Common._M(1300));
		float num4 = Common._M(0.95f);
		aChevronSpeed += Common._M(0.006f);
		SexyVertex2D sexyVertex2D = new SexyVertex2D(inX, inY, 0f, num2 + aChevronSpeed);
		SexyVertex2D sexyVertex2D2 = new SexyVertex2D(inX + num3 * (float)Math.Cos(inAngle), inY - num3 * (float)Math.Sin(inAngle), 1f, aChevronSpeed);
		SexyVertex2D sexyVertex2D3 = new SexyVertex2D(inX + num3 * num4 * (float)Math.Cos(inAngle + num), inY - num3 * num4 * (float)Math.Sin(inAngle + num), 0f, aChevronSpeed);
		SexyVertex2D sexyVertex2D4 = new SexyVertex2D(inX + num3 * num4 * (float)Math.Cos(inAngle - num), inY - num3 * num4 * (float)Math.Sin(inAngle - num), 0f, aChevronSpeed);
		SexyVertex2D[] theVertices = new SexyVertex2D[6] { sexyVertex2D, sexyVertex2D2, sexyVertex2D3, sexyVertex2D, sexyVertex2D4, sexyVertex2D2 };
		g3D.DrawPrimitiveEx((uint)SexyVertex2D.FVF, Graphics3D.EPrimitiveType.PT_TriangleList, theVertices, 2, new SexyFramework.Graphics.Color(255, 255, 255, Common._M(160)), 0, 0f, 0f, blend: true, 0u);
	}

	public bool StartFire()
	{
		if (mState != GunState.GunState_Normal)
		{
			return false;
		}
		if (mBullet == null)
		{
			return false;
		}
		if (!LaserMode() && !LightningMode())
		{
			mStatePercent = 0f;
			mState = GunState.GunState_Firing;
			Bullet bullet = mBullet;
			bullet.SetJustFired(fired: true);
			if (mCannonCount > 0 && !bullet.GetIsCannon())
			{
				bullet.SetIsCannon(isCannon: true);
				mCannonCount--;
			}
			if (bullet.GetIsCannon())
			{
				GameApp.gApp.PlaySample(mBoard.LevelIsSkeletonBoss() ? Res.GetSoundByID(ResID.SOUND_ENERGYWEAPONFIRE) : Res.GetSoundByID(ResID.SOUND_CANNON_FIRE));
				mDoingCannonBlast = true;
			}
			float num = Common._M(28f);
			float num2 = (IsPoisoned() ? mAngle : mDestAngle) - 1.570795f;
			if (mShotCorrectionTarget.x != 0f || mShotCorrectionTarget.y != 0f)
			{
				num2 = mShotCorrectionRad;
			}
			float num3 = (float)Math.Cos(num2);
			float num4 = 0f - (float)Math.Sin(num2);
			float num5 = GetFireSpeed();
			if (mBoard.mLevel.mZone == 3 && mBoard.mLevel.mBoss != null && bullet.GetIsCannon())
			{
				num5 = num;
			}
			bullet.SetVelocity(num5 * num3, num5 * num4);
			bullet.mAngleFired = num2;
			if (mBoard.mLevel.mZone == 5 && mBoard.mLevel.mNum != 10)
			{
				DoBubbles(bullet.GetIsCannon() ? 15 : 5);
			}
			if (bullet.GetIsCannon() && mCannonAngle != 0f)
			{
				for (int i = 0; i < 2; i++)
				{
					float num6 = (float)((i != 0) ? 1 : (-1)) * mCannonAngle;
					num3 = (float)Math.Cos(num2 + num6);
					num4 = 0f - (float)Math.Sin(num2 + num6);
					Bullet bullet2 = new Bullet(bullet);
					bullet2.mFrog = this;
					bullet2.mAngleFired = num2 + num6;
					bullet2.SetVelocity(num5 * num3, num5 * num4);
					mCannonBullets.Add(bullet2);
				}
			}
			if (bullet.GetIsCannon() && !CannonMode())
			{
				SetFrogType(FrogType.FrogType_Normal, current: false);
				mBoard.CannonDisabled();
			}
			CalcAngle();
		}
		return true;
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

	private void SyncListOrbPowerRings(DataSync sync, List<OrbPowerRing> theList, bool clear)
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
				OrbPowerRing orbPowerRing = new OrbPowerRing();
				orbPowerRing.SyncState(sync);
				theList.Add(orbPowerRing);
			}
			return;
		}
		sync.GetBuffer().WriteLong(theList.Count);
		foreach (OrbPowerRing the in theList)
		{
			the.SyncState(sync);
		}
	}

	private void SyncListSkeletonPowerOrbs(DataSync sync, List<SkeletonPowerOrb> theList, bool clear)
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
				SkeletonPowerOrb skeletonPowerOrb = new SkeletonPowerOrb();
				skeletonPowerOrb.SyncState(sync);
				theList.Add(skeletonPowerOrb);
			}
			return;
		}
		sync.GetBuffer().WriteLong(theList.Count);
		foreach (SkeletonPowerOrb the in theList)
		{
			the.SyncState(sync);
		}
	}

	private void SyncListBeamComponents(DataSync sync, List<BeamComponent> theList, bool clear)
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
				BeamComponent beamComponent = new BeamComponent();
				beamComponent.SyncState(sync);
				theList.Add(beamComponent);
			}
			return;
		}
		sync.GetBuffer().WriteLong(theList.Count);
		foreach (BeamComponent the in theList)
		{
			the.SyncState(sync);
		}
	}

	public void SyncState(DataSync theSync)
	{
		SexyFramework.Misc.Buffer buffer = theSync.GetBuffer();
		theSync.SyncFloat(ref mBossStateAlpha);
		theSync.SyncLong(ref mBossStateHoldTimer);
		theSync.SyncLong(ref mBossStateAlphaDir);
		theSync.SyncLong(ref mBossState);
		theSync.SyncLong(ref mSickAnim.mUpdateCount);
		theSync.SyncFloat(ref mBossDeathVY);
		theSync.SyncFloat(ref mBossDeathVX);
		theSync.SyncFloat(ref mBossDeathTX);
		theSync.SyncFloat(ref mBossDeathTY);
		theSync.SyncFloat(ref mCannonAngle);
		theSync.SyncLong(ref mStunTimer);
		theSync.SyncFloat(ref mAngle);
		theSync.SyncFloat(ref mDestAngle);
		theSync.SyncFloat(ref mCenterX);
		theSync.SyncFloat(ref mCenterY);
		theSync.SyncLong(ref mDestX1);
		theSync.SyncLong(ref mDestY1);
		theSync.SyncLong(ref mDestX2);
		theSync.SyncLong(ref mDestY2);
		theSync.SyncLong(ref mSlowTimer);
		theSync.SyncLong(ref mDestTime);
		theSync.SyncLong(ref mDestCount);
		theSync.SyncFloat(ref mCurX);
		theSync.SyncFloat(ref mCurY);
		theSync.SyncLong(ref mWidth);
		theSync.SyncLong(ref mHeight);
		theSync.SyncLong(ref mCannonCount);
		theSync.SyncLong(ref mLazerCount);
		theSync.SyncLong(ref mBX);
		theSync.SyncLong(ref mBY);
		theSync.SyncFloat(ref mRecoilAmt);
		theSync.SyncFloat(ref mStatePercent);
		theSync.SyncFloat(ref mFireVel);
		theSync.SyncLong(ref mStunSpinFrame);
		theSync.SyncLong(ref mStartingStunTime);
		theSync.SyncLong(ref mBlinkCount);
		theSync.SyncLong(ref mBlinkTimer);
		theSync.SyncFloat(ref mLazerPercent);
		theSync.SyncBoolean(ref mDoingHop);
		theSync.SyncLong(ref mUpdateCount);
		int theInt = (int)mState;
		theSync.SyncLong(ref theInt);
		mState = (GunState)theInt;
		SyncListComponents(theSync, mLazerPulse, clear: true);
		if (theSync.isRead())
		{
			mPowerRings.Clear();
			if (buffer.ReadBoolean())
			{
				mFlameStun = (buffer.ReadBoolean() ? Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_FIREBREATHDE150) : Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_FIREBREATHDE250)).Duplicate();
				Transform transform = new Transform();
				float num = GameApp.DownScaleNum(1f);
				int num2 = (int)((float)Common._DS(Common._M(400)) * num);
				int num3 = (int)((float)Common._DS(Common._M(330)) * num);
				transform.Translate(-num2 / 2, -num3 / 2);
				transform.RotateRad(mAngle);
				transform.Translate(num2 / 2, num3 / 2);
				transform.Translate(Common._S(GetCenterX() + Common._DS(Common._M(-70))), Common._S(GetCenterY() + Common._DS(Common._M1(-55))));
				mFlameStun.SetTransform(transform.GetMatrix());
				mFlameStun.Play((int)buffer.ReadLong());
			}
			if (buffer.ReadBoolean())
			{
				mDarkFrogStun = (buffer.ReadBoolean() ? Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_FROG_HIT_125) : Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_FROG_HIT_175));
				mDarkFrogStun.ResetAnim();
				Transform transform2 = new Transform();
				float num4 = Common._DS(1f);
				int num5 = (int)((float)Common._M(400) * num4);
				int num6 = (int)((float)Common._M(330) * num4);
				transform2.Translate(-num5 / 2, -num6 / 2);
				transform2.RotateRad((float)Math.PI);
				transform2.Translate(num5 / 2, num6 / 2);
				transform2.Translate(Common._S(GetCenterX() + Common._M(-52)), Common._S(GetCenterY() + Common._M1(-13)));
				mDarkFrogStun.SetTransform(transform2.GetMatrix());
				mDarkFrogStun.Play((int)buffer.ReadLong());
			}
		}
		else
		{
			buffer.WriteBoolean(mFlameStun != null);
			if (mFlameStun != null)
			{
				buffer.WriteBoolean(mFlameStunShort);
				buffer.WriteLong((int)mFlameStun.mMainSpriteInst.mFrameNum);
			}
			buffer.WriteBoolean(mDarkFrogStun != null);
			if (mDarkFrogStun != null)
			{
				buffer.WriteBoolean(mDarkFrogStunShort);
				buffer.WriteLong((int)mDarkFrogStun.mMainSpriteInst.mFrameNum);
			}
		}
		SyncListOrbPowerRings(theSync, mPowerRings, clear: true);
		SyncListSkeletonPowerOrbs(theSync, mPowerOrbs, clear: true);
		theSync.SyncLong(ref mCannonRuneAlpha);
		theSync.SyncLong(ref mCannonLightness);
		theSync.SyncLong(ref mCannonRuneColor);
		theSync.SyncLong(ref mCannonState);
		theInt = (int)mCurrentBody.mType;
		theSync.SyncLong(ref theInt);
		mCurrentBody.mType = (FrogType)theInt;
		if (theSync.isWrite())
		{
			mCurrentBody.SyncState(theSync);
			buffer.WriteShort((short)mFrogStack.Count);
			for (int i = 0; i < mFrogStack.size(); i++)
			{
				buffer.WriteLong((long)mFrogStack[i].mType);
				mFrogStack[i].SyncState(theSync);
			}
			buffer.WriteBoolean(mDizzyStars != null);
			if (mDizzyStars != null)
			{
				Common.SerializeParticleSystem(mDizzyStars, theSync);
			}
		}
		else
		{
			SetFrogType(mCurrentBody.mType, current: true);
			mCurrentBody.SyncState(theSync);
			int num7 = buffer.ReadShort();
			for (int j = 0; j < num7; j++)
			{
				SetFrogType((FrogType)buffer.ReadLong(), current: false);
				mFrogStack.back().SyncState(theSync);
			}
			mDizzyStars = null;
			if (buffer.ReadBoolean())
			{
				mDizzyStars = Common.DeserializeParticleSystem(theSync);
			}
		}
		if (theSync.isRead())
		{
			EmptyBullets(reset_frog_type: false);
			if (buffer.ReadBoolean())
			{
				mBullet = new Bullet();
				mBullet.mFrog = this;
				mBullet.SyncState(theSync);
			}
			if (buffer.ReadBoolean())
			{
				mNextBullet = new Bullet();
				mNextBullet.mFrog = this;
				mNextBullet.SyncState(theSync);
			}
			int num8 = (int)buffer.ReadLong();
			for (int k = 0; k < num8; k++)
			{
				Bullet bullet = new Bullet();
				bullet.mFrog = this;
				bullet.SyncState(theSync);
				mCannonBullets.Add(bullet);
			}
		}
		else
		{
			buffer.WriteBoolean(mBullet != null);
			if (mBullet != null)
			{
				mBullet.SyncState(theSync);
			}
			buffer.WriteBoolean(mNextBullet != null);
			if (mNextBullet != null)
			{
				mNextBullet.SyncState(theSync);
			}
			buffer.WriteLong(mCannonBullets.Count);
			for (int l = 0; l < mCannonBullets.Count; l++)
			{
				mCannonBullets[l].SyncState(theSync);
			}
		}
		mElectricOrb.SyncState(theSync);
		if (theSync.isRead())
		{
			mElectricOrb.mImage = (MemoryImage)Res.GetImageByID(ResID.IMAGE_FROG_ELECTRIC_ORB);
		}
		for (int m = 0; m < 3; m++)
		{
			SyncListBeamComponents(theSync, mBeams[m], clear: true);
			if (theSync.isRead())
			{
				Image image = null;
				switch (m)
				{
				case 0:
					image = Res.GetImageByID(ResID.IMAGE_FROG_BEAM_A);
					break;
				case 1:
					image = Res.GetImageByID(ResID.IMAGE_FROG_BEAM_B);
					break;
				case 2:
					image = Res.GetImageByID(ResID.IMAGE_FROG_BEAM_C);
					break;
				}
				for (int n = 0; n < mBeams[m].size(); n++)
				{
					mBeams[m][n].mImage = (MemoryImage)image;
				}
			}
		}
		theSync.SyncLong(ref mLastGuideX);
		theSync.SyncLong(ref mLastGuideY);
		theSync.SyncFloat(ref mBeamProjectedEndX);
		theSync.SyncFloat(ref mBeamProjectedEndY);
		theSync.SyncFloat(ref mBeamDistToTarget);
		theSync.SyncFloat(ref mFarthestDistance);
		theSync.SyncBoolean(ref mDoElectricBeamShit);
		if (theSync.isRead() && mBoard.LevelIsSkeletonBoss() && mBullet != null && mBullet.GetIsCannon())
		{
			SetCannonCount(1, stack: false, 0, 0f);
		}
		if (theSync.isRead() && GetType() == 1)
		{
			mCannonBlast = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_CANNONBLAST).Duplicate();
		}
		if (theSync.isRead() && GetType() == 3)
		{
			GameApp.gApp.mSoundPlayer.Loop(Res.GetSoundByID(ResID.SOUND_LIGHTNING_LOOP));
		}
	}

	public void FireElectricOrb()
	{
		float num = Common._M(20f);
		mElectricOrb.mVX = num * (float)Math.Cos(GetBeamAngle());
		mElectricOrb.mVY = num * (0f - (float)Math.Sin(GetBeamAngle()));
		mElectricOrb.mV0 = num;
		int mAlphaDelta = (int)Common._M(-20f);
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < mBeams[i].Count(); j++)
			{
				mBeams[i][j].mMinAlpha = 0;
				mBeams[i][j].mAlphaDelta = mAlphaDelta;
			}
		}
	}

	public void AddBubble(Bubble b)
	{
		if ((mBoard.mLevel.mBoss == null || mBoard.mLevel.mZone == 5) && !mBoard.DoingLevelTransition() && mBoard.GetGameState() == GameState.GameState_Playing)
		{
			mBubbles.Add(b);
		}
	}

	public Bullet GetFiredBullet()
	{
		if (mState == GunState.GunState_Firing && mStatePercent >= Common._M(0.9f))
		{
			if (mBullet.mSkip)
			{
				mBullet.mSkip = false;
				return null;
			}
			Bullet bullet = mBullet;
			if (mCannonBullets.size() > 0)
			{
				mBullet = mCannonBullets.First();
				mCannonBullets.Remove(mBullet);
			}
			else
			{
				mState = GunState.GunState_Normal;
				if (bullet.GetIsCannon())
				{
					bullet = new Bullet(mBullet);
					bullet.mFrog = this;
					mBullet.mSkip = true;
					mBullet.SetJustFired(fired: false);
					mBullet.SetIsCannon(isCannon: false);
				}
				else
				{
					mBullet = null;
				}
			}
			return bullet;
		}
		return null;
	}

	public void SetPos(int theX, int theY)
	{
		mCenterX = theX;
		mCenterY = theY;
		mDestCount = 0;
		mCurX = theX;
		mCurY = theY;
		CalcAngle();
		int num = ((GetCenterX() <= Common._SS(GameApp.gApp.mWidth) - GetCenterX()) ? Common._SS(GameApp.gApp.mWidth) : 0);
		int num2 = ((GetCenterY() <= Common._SS(GameApp.gApp.mHeight) - GetCenterY()) ? Common._SS(GameApp.gApp.mHeight) : 0);
		mFarthestDistance = MathUtils.Distance(GetCenterX(), GetCenterY(), num, num2);
	}

	public void Update()
	{
		if (mBossStateAlphaDir != 0)
		{
			mBossStateAlpha += Common._M(1.5f) * (float)mBossStateAlphaDir;
			if (mBossStateAlpha > 255f)
			{
				mBossStateAlphaDir = 0;
				mBossStateAlpha = 255f;
			}
			else if (mBossStateAlpha < 0f)
			{
				mBossStateAlphaDir = 0;
				mBossStateAlpha = 0f;
			}
		}
		else if (mBossStateHoldTimer > 0 && --mBossStateHoldTimer == 0)
		{
			mBossStateAlphaDir = -1;
		}
		if (mBoard.mLevel.mNum == 10 && mBoard.mLevel.mZone == 5 && mBubbles.Count() > 0)
		{
			mBubbles.Clear();
		}
		if (mLightningEffect != null)
		{
			mGlobalTranform.Reset();
			mGlobalTranform.Translate(Common._S(mCenterX + (float)Common._M(-45)), 0f);
			mLightningEffect.SetTransform(mGlobalTranform.GetMatrix());
			mLightningEffect.Update();
		}
		if (IsPoisoned())
		{
			int num = (int)Common._M(134f);
			int num2 = mSickAnim.GetMaxDuration() - num;
			if (mBoard.mLevel.mInvertMouseTimer <= num2 || mSickAnim.GetUpdateCount() < num)
			{
				mSickAnim.Update();
			}
		}
		for (int i = 0; i < 4; i++)
		{
			if (mLazerFrogBackPulseAlpha[i] > 0f)
			{
				float num3 = mLazerFrogBackPulseAlpha[i];
				mLazerFrogBackPulseAlpha[i] -= Common._M(10f);
				if (mLazerFrogBackPulseAlpha[i] < 0f)
				{
					mLazerFrogBackPulseAlpha[i] = 0f;
				}
				if (num3 > (float)Common._M(200) && mLazerFrogBackPulseAlpha[i] < (float)Common._M1(200) && i + 1 < 4 && mLazerFrogBackPulseAlpha[i + 1] == 0f)
				{
					mLazerFrogBackPulseAlpha[i + 1] = 255f;
				}
				if (mLazerFrogBackPulseAlpha[i] > (float)Common._M(200))
				{
					break;
				}
			}
		}
		for (int j = 0; j < mSmokeParticles.Count(); j++)
		{
			if (BambooTransition.UpdateSmokeParticle(mSmokeParticles[j]))
			{
				mSmokeParticles[j] = null;
				mSmokeParticles.RemoveAt(j);
				j--;
			}
		}
		if (mDoingCannonBlast && mCannonBlast.IsActive())
		{
			mCannonBlast.Update();
			if (mCannonBlast.mFrameNum > (float)mCannonBlast.mLastFrameNum)
			{
				mDoingCannonBlast = false;
			}
			else
			{
				mCannonBlast.mDrawTransform.LoadIdentity();
				float num4 = GameApp.DownScaleNum(1f);
				mCannonBlast.mDrawTransform.Scale(num4, num4);
				mCannonBlast.mDrawTransform.RotateRad(mAngle);
				float x = Common._DS(Common._M(0));
				float y = Common._DS(Common._M(185));
				JeffLib.Common.RotatePoint(mAngle, ref x, ref y, 0f, 0f);
				mCannonBlast.mDrawTransform.Translate(Common._S(mCurX) + x, Common._S(mCurY) + y);
			}
		}
		if (mDizzyStars != null)
		{
			mDizzyStars.Update();
			mDizzyStars.SetPos(mCenterX, mCenterY);
			if (mDizzyStars.GetUpdateCount() > 50 && mDizzyStars.GetTotalParticles() == 0)
			{
				mDizzyStars = null;
			}
		}
		mUpdateCount++;
		mCurX = mCenterX;
		mCurY = mCenterY;
		if (mFlameStun != null)
		{
			mGlobalTranform.Reset();
			float num5 = Common._DS(1f);
			int num6 = (int)(Common._M(400f) * num5);
			int num7 = (int)(Common._M(330f) * num5);
			mGlobalTranform.Translate(-num6 / 2, -num7 / 2);
			mGlobalTranform.RotateRad(mAngle);
			mGlobalTranform.Translate(num6 / 2, num7 / 2);
			mGlobalTranform.Translate(Common._S(GetCenterX() + Common._M(-100)), Common._S(GetCenterY() + Common._M(-80)));
			mFlameStun.SetTransform(mGlobalTranform.GetMatrix());
			mFlameStun.Update();
			if (!mFlameStun.IsActive())
			{
				GameApp.gApp.mSoundPlayer.Fade(Res.GetSoundByID(ResID.SOUND_NEW_BURNINGFROGLOOP));
				mFlameStun = null;
				mSpitAlpha = 0f;
				mStunTimer = 0;
			}
		}
		else if (mDarkFrogStun != null)
		{
			SexyTransform2D transform = new SexyTransform2D(init: false);
			float num8 = Common._DS(1f);
			int num9 = (int)((float)Common._M(400) * num8);
			int num10 = (int)((float)Common._M(330) * num8);
			transform.Translate(-num9 / 2, -num10 / 2);
			transform.RotateRad((float)Math.PI);
			transform.Translate(num9 / 2, num10 / 2);
			transform.Translate(Common._S(GetCenterX() + Common._M(-52)), Common._S(GetCenterY() + Common._M1(-13)));
			mDarkFrogStun.SetTransform(transform);
			mDarkFrogStun.Update();
			if (!mDarkFrogStun.IsActive() || mDarkFrogStun.mMainSpriteInst.mFrameNum >= (float)(mDarkFrogStun.mMainSpriteInst.mDef.mFrames.Count() - 1))
			{
				GameApp.gApp.mSoundPlayer.Fade(Res.GetSoundByID(ResID.SOUND_NEW_BURNINGFROGLOOP));
				mAngle = (float)Math.PI;
				mDarkFrogStun = null;
				mSpitAlpha = 0f;
				mStunTimer = 0;
			}
		}
		if (mBossDeathVX != 0f || mBossDeathVY != 0f)
		{
			mCenterX += mBossDeathVX;
			mCenterY += mBossDeathVY;
			if ((mBossDeathVX > 0f && mCenterX >= mBossDeathTX) || (mBossDeathVX < 0f && mCenterX <= mBossDeathTX))
			{
				mCenterX = mBossDeathTX;
				mBossDeathVX = 0f;
			}
			if ((mBossDeathVY > 0f && mCenterY >= mBossDeathTY) || (mBossDeathVY < 0f && mCenterY <= mBossDeathTY))
			{
				mCenterY = mBossDeathTY;
				mBossDeathVY = 0f;
			}
		}
		if (mBoard.GetHallucinateTimer() > 0)
		{
			mTempText.Update();
			mTempText.SetX(Common._S(GetCenterX() - Common._M(32)));
			mTempText.SetY(Common._S(GetCenterY() - Common._M(100)));
		}
		for (int k = 0; k < mPowerRings.Count(); k++)
		{
			mPowerRings[k].Update();
		}
		for (int l = 0; l < mPowerOrbs.Count(); l++)
		{
			SkeletonPowerOrb skeletonPowerOrb = mPowerOrbs[l];
			float num11 = Common._M(2f);
			float num12 = Common._M(0.1f);
			if (skeletonPowerOrb.mSize < num11)
			{
				skeletonPowerOrb.mSize += num12;
				if (skeletonPowerOrb.mSize > num11)
				{
					skeletonPowerOrb.mSize = num11;
				}
			}
			else if (skeletonPowerOrb.mAlpha > 0f)
			{
				skeletonPowerOrb.mAlpha -= Common._M(3f);
				if (skeletonPowerOrb.mAlpha <= 0f)
				{
					mPowerOrbs.RemoveAt(l);
					l--;
				}
			}
		}
		if (mBullet != null)
		{
			mBullet.Update();
		}
		if (mNextBullet != null)
		{
			mNextBullet.Update();
		}
		for (int m = 0; m < mBubbles.Count(); m++)
		{
			Bubble bubble = mBubbles[m];
			bubble.Update();
			if (bubble.GetAlpha() <= 0f)
			{
				mBubbles.RemoveAt(m);
				m--;
			}
		}
		if (mSlowTimer > 0)
		{
			mSlowTimer--;
		}
		if (IsFuckedUp() && !IsStunned() && mUpdateCount % Common._M(10) == 0)
		{
			ConfusionMark confusionMark = new ConfusionMark();
			mConfusionMarks.Add(confusionMark);
			confusionMark.mImage = Res.GetImageByID(ResID.IMAGE_QUESTION_MARK);
			confusionMark.mX = 0f;
			confusionMark.mY = 0f;
			confusionMark.mSize = Common._M(0.1f);
			confusionMark.mAlpha = 0f;
			confusionMark.mAlphaInc = Common._M(6f);
			confusionMark.mVX = (Common._M(0.1f) + (float)(SexyFramework.Common.Rand() % Common._M1(1000)) / Common._M2(1000f)) * (float)((SexyFramework.Common.Rand() % 100 < 50) ? 1 : (-1));
			confusionMark.mVY = (Common._M(0.1f) + (float)(SexyFramework.Common.Rand() % Common._M1(1000)) / Common._M2(1000f)) * (float)((SexyFramework.Common.Rand() % 100 < 50) ? 1 : (-1));
		}
		if (mBlinkCount >= 0 && --mBlinkTimer == 0)
		{
			mBlinkCount--;
			mBlinkTimer = (int)Common._M(15f);
		}
		if (mSpitAlpha > 0f)
		{
			mSpitAlpha -= Common._M(6f);
			mSpitX += mSpitVX;
			mSpitY += mSpitVY;
		}
		if (mStunTimer > 0 && mFlameStun == null && mDarkFrogStun == null && mBlinkCount < 0)
		{
			int num13 = (int)Common._M(12f);
			if (mStunSpinFrame == 1)
			{
				num13 *= 2;
			}
			Image imageByID = Res.GetImageByID(ResID.IMAGE_FROG_SPIN_FRAMES);
			if (mUpdateCount % num13 == 0)
			{
				if (mStunTimer >= mStartingStunTime - Common._M(50) && mStunSpinFrame == 2)
				{
					mStunSpinFrame = 0;
				}
				else if (mStunTimer - 3 * num13 <= 0 && mStunSpinFrame < imageByID.mNumRows - 1)
				{
					mStunSpinFrame = ((mStunSpinFrame == 0) ? 1 : 2);
					if (mStunSpinFrame == 1)
					{
						mAngle = (mDestAngle = (float)Math.PI);
						mSpitAlpha = 255f;
						mSpitAngle = MathUtils.DegreesToRadians(Common._M(-60));
						mSpitVX = (float)Common._M(1) * (float)Math.Cos(mSpitAngle);
						mSpitVY = (float)(-Common._M(1)) * (float)Math.Sin(mSpitAngle);
						mSpitX = mCenterX + (float)Common._M(50);
						mSpitY = mCenterY + (float)Common._M(30);
					}
				}
			}
			if (--mStunTimer == 0)
			{
				mAngle = mDestAngle;
				for (int n = 0; n < mConfusionMarks.Count(); n++)
				{
					mConfusionMarks[n].mAlphaInc = Common._M(-4f);
				}
				mBlinkCount = 3;
			}
		}
		for (int num14 = 0; num14 < mConfusionMarks.Count(); num14++)
		{
			ConfusionMark confusionMark2 = mConfusionMarks[num14];
			if (!IsFuckedUp())
			{
				confusionMark2.mAlphaInc = Common._M(-4f);
			}
			confusionMark2.mAlpha += confusionMark2.mAlphaInc;
			confusionMark2.mX += confusionMark2.mVX;
			confusionMark2.mY += confusionMark2.mVY;
			confusionMark2.mSize += Common._M(0.01f);
			if (confusionMark2.mAlpha >= 255f && confusionMark2.mAlphaInc > 0f)
			{
				confusionMark2.mAlpha = 255f;
				confusionMark2.mAlphaInc *= Common._M(-1f);
			}
			else if (confusionMark2.mAlpha <= 0f && confusionMark2.mAlphaInc < 0f)
			{
				mConfusionMarks.RemoveAt(num14);
				num14--;
			}
		}
		if (mFrogStack.Count() > 0 && mFrogStack[mFrogStack.Count() - 1].mAlpha != 255 && mFrogStack[mFrogStack.Count() - 1].mAlpha != -1)
		{
			FrogBody frogBody = mFrogStack[mFrogStack.Count() - 1];
			frogBody.mAlpha += (int)Common._M(20f);
			if (frogBody.mAlpha >= 255)
			{
				frogBody.mAlpha = -1;
				mCurrentBody = frogBody;
				mFrogStack.Clear();
				if (!LaserMode())
				{
					mLazerPulse.Clear();
				}
			}
		}
		float num15 = Common._M(1f);
		if (mLazerCount > 0 && mLazerPercent < num15)
		{
			mLazerPercent += Common._M(0.07f);
			if (mLazerPercent > num15)
			{
				mLazerPercent = num15;
			}
		}
		else if (mLazerCount == 0 && mLazerPercent > 0f)
		{
			mLazerPercent -= Common._M(0.06f);
			if (mLazerPercent < 0f)
			{
				mLazerPercent = 0f;
				if (mFrogStack.Count() == 0 && GetType() == 2)
				{
					SetFrogType(FrogType.FrogType_Normal, current: false);
				}
			}
		}
		if (LaserMode())
		{
			FrogBody frogBody2 = ((mFrogStack.Count() <= 0 || mFrogStack[mFrogStack.Count() - 1].mType != FrogType.FrogType_Lazer) ? mCurrentBody : mFrogStack[mFrogStack.Count() - 1]);
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_FROG_LAZER_EYE_LOOP);
			if (mUpdateCount % Common._M(6) == 0)
			{
				frogBody2.mCel = (frogBody2.mCel + 1) % imageByID2.mNumRows * imageByID2.mNumCols;
			}
			if (Component.UpdateComponentVec(mLazerPulse, mUpdateCount))
			{
				SetupLazerBackPulse();
			}
		}
		else if (mDoElectricBeamShit)
		{
			if (mElectricOrb.mVX == mElectricOrb.mVY && mElectricOrb.mVX == 0f)
			{
				RetargetBeams(0f);
			}
			UpdateBeams();
		}
		if (IsStunned() && mStunSpinFrame == 0)
		{
			mAngle += Common._M(-0.15f);
		}
		else if (!IsStunned() && mAngle != mDestAngle)
		{
			float num16 = 100f;
			if (mBoard.mLevel.mMoveType != 0)
			{
				num16 = Common._M(0.87f);
			}
			float num17 = num16;
			float num18 = mAngle;
			if (mAngle < mDestAngle)
			{
				mAngle += num17;
				if (mAngle > mDestAngle)
				{
					SetAngleToDestAngle();
				}
			}
			else
			{
				mAngle -= num17;
				if (mAngle < mDestAngle)
				{
					SetAngleToDestAngle();
				}
			}
			if (mDoElectricBeamShit && mElectricOrb.mVX == mElectricOrb.mVY && mElectricOrb.mVX == 0f)
			{
				RetargetBeams(mAngle - num18);
			}
		}
		if (mDestCount > 0)
		{
			mDestCount--;
			float num19 = (float)mDestCount / (float)mDestTime;
			mCenterX = num19 * (float)mDestX1 + (1f - num19) * (float)mDestX2;
			mCenterY = num19 * (float)mDestY1 + (1f - num19) * (float)mDestY2;
			if (mBoard.mLevel.mZone == 5 && mBoard.mLevel.mNum != 10 && mBoard.mLevel.mMoveType == 0)
			{
				if (mDoingHop)
				{
					DoBubbles(1);
				}
				else if (mUpdateCount % Common._M(10) == 0)
				{
					DoBubbles(1);
				}
			}
			if (mDoElectricBeamShit)
			{
				RetargetBeams(0f);
			}
			if (mDestCount == 0)
			{
				mDoingHop = false;
			}
		}
		float num20 = ((mSlowTimer > 0) ? Common._M(4f) : 1f);
		int num21 = 1;
		GameApp gameApp = (GameApp)GlobalMembers.gSexyAppBase;
		if (gameApp.mBoard != null && gameApp.mBoard.mIsHotFrogEnabled)
		{
			num21 = 2;
		}
		if (mState == GunState.GunState_Firing)
		{
			mStatePercent += 0.15f * (float)num21;
			if (mStatePercent > 0.6f)
			{
				mBullet.Update();
				mRecoilAmt += Common._M(2.33f);
			}
		}
		else
		{
			mStatePercent += 0.07f / num20 * (float)num21;
			if (mState == GunState.GunState_Reloading && (mRecoilAmt -= Common._M(0.7f) / num20) < 0f)
			{
				mRecoilAmt = 0f;
			}
		}
		if (mStatePercent > 1f)
		{
			mStatePercent = 1f;
			if (mState == GunState.GunState_Reloading)
			{
				mState = GunState.GunState_Normal;
				mRecoilAmt = 0f;
			}
		}
		if (mState == GunState.GunState_Normal && mRecoilAmt > 0f && (mRecoilAmt -= Common._M(0.7f) / num20) < 0f)
		{
			mRecoilAmt = 0f;
		}
		if (CannonMode() || (mBullet != null && mBullet.GetIsCannon()))
		{
			if (mUpdateCount % Common._M(18) == 0)
			{
				mCannonBallShadowPos = (mCannonBallShadowPos + 1) % NUM_CANNON_SHADOWS;
				mCannonBallShadows[mCannonBallShadowPos] = 255;
			}
			for (int num22 = 0; num22 < NUM_CANNON_SHADOWS; num22++)
			{
				if (mCannonBallShadows[num22] > 0)
				{
					mCannonBallShadows[num22] = Math.Max(0, mCannonBallShadows[num22] - 6);
				}
			}
		}
		if (mCannonState > 0)
		{
			UpdateCannonFadeIn();
		}
		CalcAngle();
	}

	public void SwapBullets(bool playSound)
	{
		if (mState == GunState.GunState_Normal && mBullet != null && mNextBullet != null && mBullet.GetColorType() != mNextBullet.GetColorType())
		{
			if (playSound)
			{
				GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BALLSWAP));
			}
			Bullet bullet = mBullet;
			mBullet = mNextBullet;
			mNextBullet = bullet;
			mBullet.SetIsCannon(mNextBullet.GetIsCannon());
			mNextBullet.SetIsCannon(isCannon: false);
			CalcAngle();
		}
	}

	public void SwapBullets()
	{
		SwapBullets(playSound: true);
	}

	public void EmptyBullets(bool reset_frog_type)
	{
		if (reset_frog_type)
		{
			mDoElectricBeamShit = false;
			SetFrogType(FrogType.FrogType_Normal, current: true);
		}
		mNextBullet = null;
		mBullet = null;
		mCannonBullets.Clear();
	}

	public void EmptyBullets()
	{
		EmptyBullets(reset_frog_type: true);
	}

	public void SetBulletType(int theType)
	{
		if (mBullet != null && theType != -1)
		{
			mBullet.SetColorType(theType);
		}
	}

	public void SetNextBulletType(int theType)
	{
		if (mNextBullet != null && theType != -1)
		{
			mNextBullet.SetColorType(theType);
		}
	}

	public Rect GetRect()
	{
		int num = mWidth - 10;
		int num2 = mHeight - 10;
		return new Rect(GetCenterX() - num / 2, GetCenterY() - num2 / 2, num, num2);
	}

	public void SetCannonCount(int c, bool stack, int color_type, float cannon_angle)
	{
		if (LightningMode())
		{
			return;
		}
		mCannonBlast = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_CANNONBLAST).Duplicate();
		if (cannon_angle < 0f)
		{
			mCannonAngle = GameApp.gApp.GetLevelMgr().mCannonAngle;
		}
		else
		{
			mCannonAngle = cannon_angle;
		}
		if (c != 0)
		{
			if (mCannonCount == 0)
			{
				mCannonRuneColor = color_type;
				mCannonState = 1;
				mCannonRuneAlpha = (mCannonLightness = 0);
				if (mBoard.LevelIsSkeletonBoss())
				{
					mCannonBlast = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_BOSS3CANNONBLAST);
					mCannonRuneColor = 0;
					mLightningEffect = Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_BOLTEFFECT);
					mGlobalTranform.Reset();
					mGlobalTranform.Translate(Common._S(mCenterX + (float)Common._M(-45)), 0f);
					mLightningEffect.SetTransform(mGlobalTranform.GetMatrix());
					mLightningEffect.mListener = this;
					mLightningEffect.Play("Main");
				}
			}
			if (stack)
			{
				mCannonCount += c - 1;
			}
			else
			{
				mCannonCount = c - 1;
			}
		}
		else
		{
			mCannonCount = 0;
		}
		if (c == 0)
		{
			if (mBullet != null)
			{
				mBullet.SetIsCannon(isCannon: false);
			}
			if (mNextBullet != null)
			{
				mNextBullet.SetIsCannon(isCannon: false);
			}
		}
		else
		{
			if (mBullet == null)
			{
				return;
			}
			if (!mBullet.GetIsCannon())
			{
				SetFrogType(FrogType.FrogType_Cannon, current: false);
				if (!mBullet.GetJustFired())
				{
					mBullet.SetIsCannon(isCannon: true);
				}
				else if (!mNextBullet.GetJustFired())
				{
					mNextBullet.SetIsCannon(isCannon: true);
				}
				else
				{
					mCannonCount++;
				}
			}
			else if (mNextBullet != null && !mNextBullet.GetIsCannon() && mState != GunState.GunState_Normal)
			{
				SetFrogType(FrogType.FrogType_Cannon, current: false);
				mNextBullet.SetIsCannon(isCannon: true);
			}
			else if (mState != GunState.GunState_Normal)
			{
				mCannonCount++;
			}
		}
	}

	public void SetCannonCount(int c, bool stack, int color_type)
	{
		SetCannonCount(c, stack, color_type, -100f);
	}

	public void DoLightningFrog(bool is_lightning)
	{
		if (is_lightning)
		{
			SetFrogType(FrogType.FrogType_Lightning, current: false);
		}
		else
		{
			SetFrogType(FrogType.FrogType_Normal, current: false);
		}
	}

	public void DoLightningFrog()
	{
		DoLightningFrog(is_lightning: true);
	}

	public void DoLazerFrog(int c, bool stack)
	{
		if (!LightningMode())
		{
			if (!stack)
			{
				mLazerCount = c;
			}
			else
			{
				mLazerCount += c - 1;
			}
			SetFrogType(FrogType.FrogType_Lazer, current: false);
		}
	}

	public void DecLazerCount()
	{
		if (mLazerCount > 0)
		{
			mLazerCount--;
			for (int i = 0; i < 4; i++)
			{
				mLazerFrogBackPulseAlpha[i] = 0f;
			}
			mLazerFrogBackPulseAlpha[0] = 255f;
		}
	}

	public void ResetFrogType()
	{
		SetFrogType(FrogType.FrogType_Normal, current: false);
	}

	public void PlayerDied()
	{
		mState = GunState.GunState_Normal;
		mRecoilAmt = 0f;
		mStatePercent = 1f;
	}

	public bool CanSpawnPowerUp(PowerType p)
	{
		if (p == PowerType.PowerType_Cannon && (CannonMode() || (mBullet != null && mBullet.GetIsCannon())))
		{
			return false;
		}
		if (p == PowerType.PowerType_ColorNuke && LightningMode())
		{
			return false;
		}
		if (p == PowerType.PowerType_Laser && LaserMode())
		{
			return false;
		}
		if (p == PowerType.PowerType_Accuracy)
		{
			return false;
		}
		return true;
	}

	public int GetCenterX()
	{
		return (int)mCenterX;
	}

	public int GetCenterY()
	{
		return (int)mCenterY;
	}

	public int GetCurX()
	{
		return (int)mCurX;
	}

	public int GetCurY()
	{
		return (int)mCurY;
	}

	public int GetCenterXDest()
	{
		return (int)((mDestCount != 0) ? ((float)mDestX2) : mCenterX);
	}

	public int GetCenterYDest()
	{
		return (int)((mDestCount != 0) ? ((float)mDestY2) : mCenterY);
	}

	public int GetWidth()
	{
		return mWidth;
	}

	public int GetHeight()
	{
		return mHeight;
	}

	public new int GetType()
	{
		if (mFrogStack.Count() <= 0)
		{
			return (int)mCurrentBody.mType;
		}
		return (int)mFrogStack[mFrogStack.Count() - 1].mType;
	}

	public int GetLazerCount()
	{
		return mLazerCount;
	}

	public Bullet GetBullet()
	{
		return mBullet;
	}

	public Bullet GetNextBullet()
	{
		return mNextBullet;
	}

	public float GetAngle()
	{
		return mAngle;
	}

	public float GetDestAngle()
	{
		return mDestAngle;
	}

	public bool IsInked()
	{
		return mBossStateAlpha > 0f;
	}

	public bool IsFiring()
	{
		return mState == GunState.GunState_Firing;
	}

	public bool IsHopping()
	{
		if (mDoingHop)
		{
			return mDestCount > 0;
		}
		return false;
	}

	public bool IsMovingToDest()
	{
		return mDestCount > 0;
	}

	public bool CannonMode()
	{
		return mCannonCount > 0;
	}

	public bool LaserMode()
	{
		if (GetType() == 2)
		{
			return mLazerPercent > 0f;
		}
		return false;
	}

	public bool LightningMode()
	{
		return GetType() == 3;
	}

	public bool IsCannon()
	{
		return GetType() == 1;
	}

	public bool IsStunned()
	{
		return mStunTimer > 0;
	}

	public bool IsPoisoned()
	{
		return mBoard.mLevel.mInvertMouseTimer > 0;
	}

	public bool HasSmokeParticles()
	{
		return mSmokeParticles.Count() > 0;
	}

	public bool IsSlow()
	{
		return mSlowTimer > 0;
	}

	public void ClearStun()
	{
		GameApp.gApp.mSoundPlayer.Fade(Res.GetSoundByID(ResID.SOUND_NEW_BURNINGFROGLOOP));
		mFlameStun = null;
		mFlameStun = null;
		mSpitAlpha = 0f;
		mStunTimer = 0;
		mDarkFrogStun = null;
	}

	public bool IsFuckedUp()
	{
		if (!IsStunned() && mSlowTimer <= 0 && !IsPoisoned())
		{
			return GameApp.gApp.GetBoard().GetHallucinateTimer() > 0;
		}
		return true;
	}

	public bool StunnedFromBoss6()
	{
		if (mFlameStun == null)
		{
			return mDarkFrogStun != null;
		}
		return true;
	}

	public void ToggleShowNextBall()
	{
		mShowNextBall = !mShowNextBall;
	}

	public float GetFireSpeed()
	{
		float num = 0f;
		num = ((mBoard.mAccuracyCount <= 0) ? 9.6f : 19f);
		if (mSlowTimer > 0)
		{
			num *= 0.25f;
		}
		return num;
	}

	public void SetFireSpeed(float theSpeed)
	{
		mFireVel = theSpeed;
	}

	public void Stun(int timer)
	{
		if (mStunTimer <= 0)
		{
			mBlinkCount = 1;
			mBlinkTimer = Common._M(15);
			mStartingStunTime = timer;
			mStunSpinFrame = Res.GetImageByID(ResID.IMAGE_FROG_SPIN_FRAMES).mNumRows - 1;
			mStunTimer = timer;
			GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_FROG_STUNNED));
			if (mBoard.mLevel.mEndSequence == 5)
			{
				mDarkFrogStun = ((timer == 125) ? Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_FROG_HIT_125).Duplicate() : Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_FROG_HIT_175).Duplicate());
				mDarkFrogStun.ResetAnim();
				mGlobalTranform.Reset();
				float num = Common._DS(1f);
				int num2 = (int)((float)Common._M(400) * num);
				int num3 = (int)((float)Common._M(330) * num);
				mGlobalTranform.Translate(-num2 / 2, -num3 / 2);
				mGlobalTranform.RotateRad((float)Math.PI);
				mGlobalTranform.Translate(num2 / 2, num3 / 2);
				mGlobalTranform.Translate(Common._S(GetCenterX() + Common._M(-52)), Common._S(GetCenterY() + Common._M1(-13)));
				mDarkFrogStun.SetTransform(mGlobalTranform.GetMatrix());
				mDarkFrogStun.Play("Hit");
				mDarkFrogStunShort = timer == 125;
			}
			else if (mBoard.mLevel.mEndSequence != 3 && mDizzyStars == null)
			{
				mDizzyStars = new SexyFramework.PIL.System(50, 50);
				mDizzyStars.mScale = Common._S(1f);
				mDizzyStars.WaitForEmitters(w: true);
				mDizzyStars.SetPos(mCenterX, mCenterY);
				Emitter emitter = new Emitter();
				emitter.mPreloadFrames = Common._M(100);
				emitter.mCullingRect = new Rect(0, 0, Common._SS(GameApp.gApp.mWidth), Common._SS(GameApp.gApp.mHeight));
				emitter.mDeleteInvisParticles = true;
				EmitterScale emitterScale = new EmitterScale();
				emitterScale.mLifeScale = Common._M(0.5f);
				emitterScale.mNumberScale = Common._M(1.61f);
				emitterScale.mSizeXScale = Common._M(1.4f);
				emitterScale.mWeightScale = 0f;
				emitterScale.mSpinScale = Common._M(0.2f);
				emitterScale.mMotionRandScale = 0f;
				emitterScale.mZoom = Common._M(0.14f);
				emitter.AddScaleKeyFrame(0, emitterScale);
				EmitterSettings emitterSettings = new EmitterSettings();
				emitterSettings.mVisibility = 0f;
				emitter.AddSettingsKeyFrame(0, emitterSettings);
				emitterSettings = new EmitterSettings(emitterSettings);
				emitterSettings.mVisibility = 1f;
				emitter.AddSettingsKeyFrame(22, emitterSettings);
				emitterSettings = new EmitterSettings(emitterSettings);
				emitter.AddSettingsKeyFrame(mStunTimer, emitterSettings);
				emitterSettings = new EmitterSettings(emitterSettings);
				emitterSettings.mVisibility = 0f;
				emitter.AddSettingsKeyFrame(mStunTimer + Common._M(150), emitterSettings);
				ParticleType particleType = new ParticleType();
				particleType.mEmitterAttachPct = Common._M(1f);
				particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_STAR);
				particleType.mRefXOff = Common._M(-162);
				particleType.mRefYOff = Common._M(-162);
				particleType.mXOff = Common._S(Common._M(20));
				particleType.mYOff = Common._S(Common._M(10));
				particleType.mColorKeyManager.AddColorKey(0f, new SexyFramework.Graphics.Color(59, 167, 93));
				particleType.mColorKeyManager.AddColorKey(0.2f, new SexyFramework.Graphics.Color(116, 228, 19));
				particleType.mColorKeyManager.AddColorKey(0.4f, new SexyFramework.Graphics.Color(233, 233, 0));
				particleType.mColorKeyManager.AddColorKey(0.8f, new SexyFramework.Graphics.Color(20, 140, 233));
				particleType.mColorKeyManager.AddColorKey(1f, new SexyFramework.Graphics.Color(26, 31, 167));
				particleType.mAlphaKeyManager.AddAlphaKey(0f, 0);
				particleType.mAlphaKeyManager.AddAlphaKey(0.25f, 255);
				particleType.mAlphaKeyManager.AddAlphaKey(0.75f, 255);
				particleType.mAlphaKeyManager.AddAlphaKey(1f, 0);
				ParticleSettings particleSettings = new ParticleSettings();
				particleSettings.mLife = Common._M(46);
				particleSettings.mNumber = Common._M(15);
				particleSettings.mXSize = Common._M(139);
				particleSettings.mWeight = Common._M(101);
				particleSettings.mSpin = SexyFramework.Common.DegreesToRadians(Common._M(40));
				particleType.AddSettingsKeyFrame(0, particleSettings);
				particleSettings = new ParticleSettings(particleSettings);
				particleType.AddSettingsKeyFrame(mStunTimer + Common._M(100), particleSettings);
				particleSettings = new ParticleSettings(particleSettings);
				particleSettings.mLife = 0;
				particleType.AddSettingsKeyFrame(mStunTimer + Common._M(150), particleSettings);
				ParticleVariance particleVariance = new ParticleVariance();
				particleVariance.mLifeVar = Common._M(54);
				particleVariance.mSizeXVar = Common._M(9);
				particleVariance.mWeightVar = Common._M(37);
				particleVariance.mSpinVar = SexyFramework.Common.DegreesToRadians(Common._M(80));
				particleType.AddVarianceKeyFrame(0, particleVariance);
				LifetimeSettings lifetimeSettings = new LifetimeSettings();
				lifetimeSettings.mSizeXMult = Common._M(0.4f);
				lifetimeSettings.mSpinMult = Common._M(1.4f);
				particleType.AddSettingAtLifePct(0f, lifetimeSettings);
				lifetimeSettings = new LifetimeSettings(lifetimeSettings);
				lifetimeSettings.mSizeXMult = Common._M(1.1f);
				lifetimeSettings.mSpinMult = Common._M(1.19f);
				particleType.AddSettingAtLifePct(0.42f, lifetimeSettings);
				lifetimeSettings = new LifetimeSettings(lifetimeSettings);
				lifetimeSettings.mSizeXMult = Common._M(0.4f);
				lifetimeSettings.mSpinMult = Common._M(0.8f);
				particleType.AddSettingAtLifePct(1f, lifetimeSettings);
				emitter.AddParticleType(particleType);
				mDizzyStars.AddEmitter(emitter);
			}
			else if (mBoard.mLevel.mEndSequence == 3)
			{
				mFlameStun = ((timer < 200) ? Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_FIREBREATHDE150).Duplicate() : Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_FIREBREATHDE250).Duplicate());
				mFlameStunShort = timer < 200;
				mFlameStun.ResetAnim();
				mGlobalTranform.Reset();
				GameApp.DownScaleNum(1f);
				int num4 = Common._DS(Common._M(400));
				int num5 = Common._DS(Common._M(330));
				mGlobalTranform.Translate(-num4 / 2, -num5 / 2);
				mGlobalTranform.RotateRad(mAngle);
				mGlobalTranform.Translate(num4 / 2, num5 / 2);
				mGlobalTranform.Translate(Common._S(GetCenterX() + Common._DS(Common._M(-70))), Common._S(GetCenterY() + Common._DS(Common._M1(-55))));
				mFlameStun.SetTransform(mGlobalTranform.GetMatrix());
				mFlameStun.Play("Main");
			}
		}
	}

	public void Poison(int timer)
	{
		if (GameApp.gApp.GetBoard().mLevel.mInvertMouseTimer <= 0)
		{
			Board board = GameApp.gApp.GetBoard();
			board.ForceFlipFrog();
			GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_MINDWARP2));
			board.mLevel.mInvertMouseTimer = timer;
			mSickAnim.Reset();
		}
	}

	public void SetSlowTimer(int t)
	{
		mSlowTimer = t;
		Board board = GameApp.gApp.GetBoard();
		board.ForceFlipFrog();
	}

	public void AddPowerRing(OrbPowerRing p)
	{
		mPowerRings.Add(p);
	}

	public void AddPowerOrb(SkeletonPowerOrb o)
	{
		mPowerOrbs.Add(o);
	}

	public void ForceX(int x)
	{
		mCenterX = (mCurX = x);
	}

	public void ForceY(int y)
	{
		mCenterY = (mCurY = y);
	}

	public bool IsFrogShowingBall()
	{
		bool result = false;
		if (mShowNextBall && (mBoard.mLevel.mBoss == null || mBoard.mLevel.mBoss.AllowFrogToFire()) && !LaserMode() && !LightningMode() && mNextBullet != null && (mState != GunState.GunState_Reloading || Common.gSuckMode))
		{
			result = true;
		}
		return result;
	}

	public void UpdateShotCorrectionTarget()
	{
		if (mBoard.mApp.mAutoMonkey.IsEnabled())
		{
			return;
		}
		mShotCorrectionTarget = new SexyVector2(0f, 0f);
		SexyVector3 sexyVector = new SexyVector3(mBoard.mFrog.GetCurX(), mBoard.mFrog.GetCurY(), 0f);
		SexyVector3 mGuideBallPoint = mBoard.mGuideBallPoint;
		float num = (float)Math.Atan2(sexyVector.y - mGuideBallPoint.y, mGuideBallPoint.x - sexyVector.x);
		bool flag = true && mBoard.mBulletList.size() == 0 && mBullet != null && GetType() == 0 && !IsSlow() && mBoard.mLevel.mMoveType == 0;
		float t = 1000000f;
		if (flag && mBoard.mCurTreasure != null && mBoard.LazerHitTreasure(sexyVector, mGuideBallPoint - sexyVector, ref t))
		{
			flag = false;
		}
		if (flag && mBoard.LazerHitTorch(sexyVector, mGuideBallPoint - sexyVector, ref t, mBullet.GetRadius()))
		{
			flag = false;
		}
		if (!flag || (mBoard.mGuideBall != null && mBoard.mGuideBall.GetColorType() == mBullet.GetColorType()))
		{
			return;
		}
		float num2 = mBoard.mApp.mShotCorrectionAngleMax * 0.01745328f;
		List<BallShotInfo> list = new List<BallShotInfo>();
		for (int i = 0; i < mBoard.mLevel.mNumCurves; i++)
		{
			List<Ball>.Enumerator enumerator = mBoard.mLevel.mCurveMgr[i].mBallList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Ball current = enumerator.Current;
				if (current.GetInTunnel() || current.GetIsExploding())
				{
					continue;
				}
				if (current.GetColorType() == mBullet.GetColorType())
				{
					bool bShiftedInTrackDir = false;
					SexyVector2 vShiftedPos = current.GetPos() + ShiftTargetByEverything(current, ref bShiftedInTrackDir);
					SexyVector3 vDir = new SexyVector3(vShiftedPos.x - sexyVector.x, vShiftedPos.y - sexyVector.y, 0f);
					if (!IsTargetWithinShotCorrectionRange(vShiftedPos, num))
					{
						continue;
					}
					if ((bShiftedInTrackDir && (current.GetNextBall() == null || !current.CollidesWithPhysically(current.GetNextBall(), 1))) || (!bShiftedInTrackDir && (current.GetPrevBall() == null || !current.CollidesWithPhysically(current.GetPrevBall(), 1))))
					{
						SexyVector2 sexyVector2 = current.GetPos() + ShiftTargetTowardFrog(current);
						SexyVector3 vDir2 = new SexyVector3(sexyVector2.x - sexyVector.x, sexyVector2.y - sexyVector.y, 0f);
						Ball ball = CheckPotentialShot(vDir2);
						if (ball == current)
						{
							list.Add(new BallShotInfo(current, vShiftedPos));
						}
					}
					else
					{
						Ball ball2 = CheckPotentialShot(vDir);
						if (ball2 == current || (bShiftedInTrackDir && ball2 == current.GetNextBall()) || (!bShiftedInTrackDir && ball2 == current.GetPrevBall()))
						{
							list.Add(new BallShotInfo(current, vShiftedPos));
						}
					}
					continue;
				}
				bool flag2 = false;
				bool bForward = true;
				Ball nextBall = current.GetNextBall();
				while (nextBall != null && nextBall.GetIsExploding())
				{
					nextBall = nextBall.GetNextBall();
				}
				if (nextBall != null && nextBall.GetColorType() != current.GetColorType() && !current.CollidesWithPhysically(nextBall, 1) && nextBall.GetColorType() == mBullet.GetColorType())
				{
					flag2 = true;
				}
				Ball prevBall = current.GetPrevBall();
				while (prevBall != null && prevBall.GetIsExploding())
				{
					prevBall = prevBall.GetPrevBall();
				}
				if (prevBall != null && prevBall.GetColorType() != current.GetColorType() && !current.CollidesWithPhysically(prevBall, 1) && prevBall.GetColorType() == mBullet.GetColorType())
				{
					flag2 = true;
					bForward = false;
				}
				if (!flag2)
				{
					continue;
				}
				SexyVector2 sexyVector3 = current.GetPos() + ShiftTargetTowardFrog(current);
				SexyVector3 vDir3 = new SexyVector3(sexyVector3.x - sexyVector.x, sexyVector3.y - sexyVector.y, 0f);
				Ball ball3 = CheckPotentialShot(vDir3);
				if (ball3 == current)
				{
					sexyVector3 = current.GetPos() + ShiftTargetTowardFrog(current) + ShiftTargetTowardBall(current, bForward) + ShiftTargetByBallSpeed(current);
					if (IsTargetWithinShotCorrectionRange(sexyVector3, num))
					{
						list.Add(new BallShotInfo(current, sexyVector3));
					}
				}
			}
		}
		float num3 = 4.14159f;
		for (int j = 0; j < list.size(); j++)
		{
			Ball mBall = list[j].mBall;
			SexyVector2 mShiftedPos = list[j].mShiftedPos;
			float num4 = (float)Math.Atan2(sexyVector.y - mShiftedPos.y, mShiftedPos.x - sexyVector.x);
			num4 -= num;
			if (num4 < -3.14159f)
			{
				num4 += 6.28318f;
			}
			else if (num4 > 3.14159f)
			{
				num4 -= 6.28318f;
			}
			if (Math.Abs(num4) < num3)
			{
				bool bShiftedInTrackDir2 = false;
				SexyVector2 sexyVector4 = mBall.GetPos() + ShiftTargetByEverything(mBall, ref bShiftedInTrackDir2);
				SexyVector3 vDir4 = new SexyVector3(sexyVector4.x - sexyVector.x, sexyVector4.y - sexyVector.y, 0f);
				Ball ball4 = CheckPotentialShot(vDir4, bWidthCheck: true);
				if (ball4 == mBall || (ball4 != null && ((ball4.GetNextBall() != null && ball4.GetNextBall() == mBall) || (ball4.GetPrevBall() != null && ball4.GetPrevBall() == mBall))))
				{
					mShotCorrectionTarget = mShiftedPos;
					num3 = Math.Abs(num4);
				}
			}
		}
		if (mShotCorrectionTarget.x != 0f && mShotCorrectionTarget.y != 0f)
		{
			mShotCorrectionRad = (float)Math.Atan2(sexyVector.y - mShotCorrectionTarget.y, mShotCorrectionTarget.x - sexyVector.x);
		}
	}

	public void UpdateAutoMonkeyShotCorrection()
	{
		mShotCorrectionTarget = new SexyVector2(0f, 0f);
		SexyVector3 sexyVector = new SexyVector3(mBoard.mFrog.GetCurX(), mBoard.mFrog.GetCurY(), 0f);
		bool bDoNormalShot = true;
		if (mBoard.mCurTreasure != null && !mBoard.mTreasureWasHit)
		{
			CheckMonkeyShot(mBoard.mCurTreasure.x, mBoard.mCurTreasure.y, ref bDoNormalShot);
			SexyVector3 sexyVector2 = new SexyVector3((float)mBoard.mCurTreasure.x - sexyVector.x, (float)mBoard.mCurTreasure.y - sexyVector.y, 0f);
			float num = sexyVector2.Magnitude();
			sexyVector2 = sexyVector2.Normalize();
			for (float num2 = 0f; num2 < num; num2 += Common._M(5f))
			{
				if (bDoNormalShot)
				{
					break;
				}
				SexyVector3 sexyVector3 = sexyVector + sexyVector2 * num2;
				if (mBoard.mLevel.PointIntersectsWall(sexyVector3.x, sexyVector3.y))
				{
					bDoNormalShot = true;
					mShotCorrectionTarget = new SexyVector2(0f, 0f);
				}
			}
		}
		foreach (Torch mTorch in mBoard.mLevel.mTorches)
		{
			if (!mTorch.mWasHit)
			{
				CheckMonkeyShot(mTorch.mX, mTorch.mY, ref bDoNormalShot);
				if (!bDoNormalShot)
				{
					break;
				}
			}
		}
		if (mBoard.mLevel.mBoss != null)
		{
			foreach (Tiki mTiki in mBoard.mLevel.mBoss.mTikis)
			{
				if (!mTiki.mWasHit)
				{
					CheckMonkeyShot(mTiki.mX, mTiki.mY, ref bDoNormalShot);
					if (!bDoNormalShot)
					{
						break;
					}
				}
			}
			if (bDoNormalShot && !mBoard.mLevel.mBoss.mEatsBalls)
			{
				CheckMonkeyShot(mBoard.mLevel.mBoss.GetX(), mBoard.mLevel.mBoss.GetY(), ref bDoNormalShot);
			}
		}
		int num3 = 0;
		int num4 = 0;
		bool flag = false;
		Ball ball = null;
		Ball ball2 = null;
		SexyVector2 sexyVector4 = new SexyVector2(0f, 0f);
		SexyVector2 sexyVector5 = new SexyVector2(0f, 0f);
		for (int i = 0; i < mBoard.mLevel.mNumCurves; i++)
		{
			if (!bDoNormalShot)
			{
				break;
			}
			foreach (Ball mBall in mBoard.mLevel.mCurveMgr[i].mBallList)
			{
				if (mBall != null && mBullet != null && !mBall.GetIsExploding() && mBall.GetColorType() == mBullet.GetColorType())
				{
					num3++;
					Ball nextBall = mBall.GetNextBall();
					while (nextBall != null && nextBall.GetIsExploding())
					{
						nextBall = nextBall.GetNextBall();
					}
					if (nextBall != null && nextBall.GetColorType() == mBall.GetColorType() && !mBall.CollidesWithPhysically(nextBall, 1))
					{
						flag = true;
					}
					Ball prevBall = mBall.GetPrevBall();
					while (prevBall != null && prevBall.GetIsExploding())
					{
						prevBall = prevBall.GetPrevBall();
					}
					if (prevBall != null && prevBall.GetColorType() == mBall.GetColorType() && !mBall.CollidesWithPhysically(prevBall, 1))
					{
						flag = true;
					}
					if (!flag)
					{
						SexyVector2 sexyVector6 = mBall.GetPos() + ShiftTargetTowardFrog(mBall);
						SexyVector3 vDir = new SexyVector3(sexyVector6.x - sexyVector.x, sexyVector6.y - sexyVector.y, 0f);
						Ball ball3 = CheckPotentialShot(vDir);
						if (mBall == ball3 && ball2 == null && !mBall.GetInTunnel())
						{
							bool bShiftedInTrackDir = false;
							sexyVector5 = mBall.GetPos() + ShiftTargetByEverything(mBall, ref bShiftedInTrackDir);
							ball2 = mBall;
						}
					}
				}
				else
				{
					if (!flag && num3 > num4 && ball2 != null)
					{
						sexyVector4 = sexyVector5;
						num4 = num3;
						ball = ball2;
					}
					num3 = 0;
					ball2 = null;
					sexyVector5 = new SexyVector2(0f, 0f);
					flag = false;
				}
			}
			if (num3 > num4 && ball2 != null)
			{
				sexyVector4 = sexyVector5;
				num4 = num3;
				ball = ball2;
			}
		}
		if (ball != null)
		{
			mShotCorrectionTarget = sexyVector4;
		}
		if (mShotCorrectionTarget != SexyVector2.Zero)
		{
			mShotCorrectionRad = (float)Math.Atan2(sexyVector.y - mShotCorrectionTarget.y, mShotCorrectionTarget.x - sexyVector.x);
		}
	}

	public void CheckMonkeyShot(float x, float y, ref bool bDoNormalShot)
	{
		SexyVector3 sexyVector = new SexyVector3(mBoard.mFrog.GetCurX(), mBoard.mFrog.GetCurY(), 0f);
		SexyVector2 sexyVector2 = new SexyVector2(x - sexyVector.x, y - sexyVector.y);
		Ball ball = CheckPotentialShot(new SexyVector3(sexyVector2.x, sexyVector2.y, 0f));
		SexyVector2 sexyVector3 = new SexyVector2(0f, 0f);
		if (ball != null)
		{
			sexyVector3 = new SexyVector2(ball.GetX() - sexyVector.x, ball.GetY() - sexyVector.y);
		}
		if (ball == null || sexyVector2.MagnitudeSquared() < sexyVector3.MagnitudeSquared())
		{
			mShotCorrectionTarget.x = x;
			mShotCorrectionTarget.y = y;
			bDoNormalShot = false;
		}
	}

	public Ball CheckPotentialShot(SexyVector3 vDir, bool bWidthCheck)
	{
		SexyVector3 sexyVector = new SexyVector3(mBoard.mFrog.GetCurX(), mBoard.mFrog.GetCurY(), 0f);
		SexyVector3 sexyVector2 = vDir.Normalize();
		SexyVector3 sexyVector3 = new SexyVector3(sexyVector2.y, sexyVector2.x, sexyVector2.z);
		Ball ball = null;
		float t = 1000000f;
		float t2 = t;
		float t3 = t;
		for (int i = 0; i < mBoard.mLevel.mNumCurves; i++)
		{
			Ball ball2 = mBoard.mLevel.mCurveMgr[i].CheckBallIntersection(sexyVector, sexyVector2, ref t, skip_exploding: true);
			if (ball2 == null)
			{
				continue;
			}
			Ball prevBall = ball2.GetPrevBall();
			Ball nextBall = ball2.GetNextBall();
			if (bWidthCheck)
			{
				Ball ball3 = mBoard.mLevel.mCurveMgr[i].CheckBallIntersection(sexyVector - sexyVector3 * ((float)ball2.GetRadius() + 1f), sexyVector2, ref t2, skip_exploding: true);
				Ball ball4 = mBoard.mLevel.mCurveMgr[i].CheckBallIntersection(sexyVector + sexyVector3 * ((float)ball2.GetRadius() + 1f), sexyVector2, ref t3, skip_exploding: true);
				if ((ball3 == ball2 || ball3 == prevBall || ball3 == nextBall || t < t2) && (ball4 == ball2 || ball4 == prevBall || ball4 == nextBall || t < t3))
				{
					ball = ball2;
				}
			}
			else
			{
				ball = ball2;
			}
		}
		for (float num = 0f; num < t; num += Common._M(5f))
		{
			if (ball == null)
			{
				break;
			}
			SexyVector3 sexyVector4 = sexyVector + sexyVector2 * num;
			if (mBoard.mLevel.PointIntersectsWall(sexyVector4.x, sexyVector4.y))
			{
				ball = null;
			}
			if (bWidthCheck)
			{
				SexyVector3 sexyVector5 = sexyVector - sexyVector3 * ball.GetRadius() + sexyVector2 * num;
				if (mBoard.mLevel.PointIntersectsWall(sexyVector5.x, sexyVector5.y))
				{
					ball = null;
				}
				SexyVector3 sexyVector6 = sexyVector + sexyVector3 * ball.GetRadius() + sexyVector2 * num;
				if (mBoard.mLevel.PointIntersectsWall(sexyVector6.x, sexyVector6.y))
				{
					ball = null;
				}
			}
		}
		return ball;
	}

	public Ball CheckPotentialShot(SexyVector3 vDir)
	{
		return CheckPotentialShot(vDir, bWidthCheck: false);
	}

	public bool IsTargetWithinShotCorrectionRange(SexyVector2 vShiftedPos, float targetRad)
	{
		SexyVector2 sexyVector = new SexyVector2(mBoard.mFrog.GetCurX(), mBoard.mFrog.GetCurY());
		SexyVector2 sexyVector2 = new SexyVector2(mBoard.mGuideBallPoint.x, mBoard.mGuideBallPoint.y);
		if ((sexyVector - sexyVector2).Magnitude() < mBoard.mApp.mShotCorrectionAngleToWidthDist)
		{
			float num = (float)Math.Atan2(sexyVector.y - vShiftedPos.y, vShiftedPos.x - sexyVector.x);
			num -= targetRad;
			if (num < -3.14159f)
			{
				num += 6.28318f;
			}
			else if (num > 3.14159f)
			{
				num -= 6.28318f;
			}
			float num2 = mBoard.mApp.mShotCorrectionAngleMax * 0.01745328f;
			return Math.Abs(num) <= num2;
		}
		float mShotCorrectionWidthMax = mBoard.mApp.mShotCorrectionWidthMax;
		return (vShiftedPos - sexyVector2).Magnitude() <= mShotCorrectionWidthMax * mShotCorrectionWidthMax;
	}

	public SexyVector2 ShiftTargetTowardFrog(Ball aBall)
	{
		SexyVector2 sexyVector = new SexyVector2(mBoard.mFrog.GetCurX(), mBoard.mFrog.GetCurY());
		SexyVector2 sexyVector2 = default(SexyVector2);
		SexyVector2 sexyVector3 = default(SexyVector2);
		SexyVector2 sexyVector4 = default(SexyVector2);
		aBall.GetCurve().GetXYFromWaypoint((int)aBall.GetWayPoint(), out sexyVector2.mVector.X, out sexyVector2.mVector.Y);
		aBall.GetCurve().GetXYFromWaypoint((int)aBall.GetWayPoint() - 1, out sexyVector3.mVector.X, out sexyVector3.mVector.Y);
		sexyVector4 = sexyVector2 - sexyVector3;
		sexyVector4.Normalize();
		SexyVector2 sexyVector5 = new SexyVector2(0f - sexyVector4.y, sexyVector4.x);
		sexyVector2 = sexyVector5 * aBall.GetRadius();
		sexyVector3 = -sexyVector5 * aBall.GetRadius();
		if ((aBall.GetPos() + sexyVector2 - sexyVector).Magnitude() < (aBall.GetPos() + sexyVector3 - sexyVector).Magnitude())
		{
			return sexyVector2;
		}
		return sexyVector3;
	}

	public SexyVector2 ShiftTargetTowardGuide(Ball aBall, ref bool bShiftedInTrackDir)
	{
		SexyVector2 sexyVector = new SexyVector2(mBoard.mFrog.GetCurX(), mBoard.mFrog.GetCurY());
		SexyVector2 sexyVector2 = default(SexyVector2);
		SexyVector2 sexyVector3 = default(SexyVector2);
		SexyVector2 sexyVector4 = default(SexyVector2);
		aBall.GetCurve().GetXYFromWaypoint((int)aBall.GetWayPoint(), out sexyVector2.mVector.X, out sexyVector2.mVector.Y);
		aBall.GetCurve().GetXYFromWaypoint((int)aBall.GetWayPoint() - 1, out sexyVector3.mVector.X, out sexyVector3.mVector.Y);
		sexyVector4 = sexyVector2 - sexyVector3;
		sexyVector4.Normalize();
		sexyVector2 = sexyVector4 * aBall.GetRadius();
		sexyVector3 = -sexyVector4 * aBall.GetRadius();
		SexyVector2 sexyVector5 = new SexyVector2(mBoard.mGuideBallPoint.x, mBoard.mGuideBallPoint.y);
		(aBall.GetPos() + sexyVector2 - sexyVector).Normalize();
		(sexyVector5 - sexyVector).Normalize();
		(aBall.GetPos() + sexyVector3 - sexyVector).Normalize();
		float value = (aBall.GetPos() + sexyVector2 - sexyVector).Normalize().Dot((sexyVector5 - sexyVector).Normalize());
		float value2 = (aBall.GetPos() + sexyVector3 - sexyVector).Normalize().Dot((sexyVector5 - sexyVector).Normalize());
		if (Math.Abs(value) > Math.Abs(value2))
		{
			bShiftedInTrackDir = true;
			return sexyVector2;
		}
		bShiftedInTrackDir = false;
		return sexyVector3;
	}

	public SexyVector2 ShiftTargetTowardBall(Ball aBall, bool bForward)
	{
		SexyVector2 sexyVector = default(SexyVector2);
		SexyVector2 sexyVector2 = default(SexyVector2);
		SexyVector2 sexyVector3 = default(SexyVector2);
		aBall.GetCurve().GetXYFromWaypoint((int)aBall.GetWayPoint(), out sexyVector.mVector.X, out sexyVector.mVector.Y);
		aBall.GetCurve().GetXYFromWaypoint((int)aBall.GetWayPoint() - 1, out sexyVector2.mVector.X, out sexyVector2.mVector.Y);
		sexyVector3 = sexyVector - sexyVector2;
		sexyVector3.Normalize();
		if (bForward)
		{
			return sexyVector3 * aBall.GetRadius();
		}
		return -sexyVector3 * aBall.GetRadius();
	}

	public SexyVector2 ShiftTargetByBallSpeed(Ball aBall)
	{
		SexyVector2 sexyVector = new SexyVector2(mBoard.mFrog.GetCurX(), mBoard.mFrog.GetCurY());
		float num = (aBall.GetPos() - sexyVector).Magnitude();
		float fireSpeed = GetFireSpeed();
		float num2 = num / fireSpeed;
		if (aBall.GetWayPointProgress() != 0f)
		{
			float num3 = aBall.GetWayPoint() + aBall.GetWayPointProgress() * num2;
			SexyVector2 sexyVector2 = new SexyVector2(0f, 0f);
			SexyVector2 sexyVector3 = new SexyVector2(0f, 0f);
			aBall.GetCurve().GetXYFromWaypoint((int)aBall.GetWayPoint(), out sexyVector3.mVector.X, out sexyVector3.mVector.Y);
			aBall.GetCurve().GetXYFromWaypoint((int)num3, out sexyVector2.mVector.X, out sexyVector2.mVector.Y);
			return sexyVector2 - sexyVector3;
		}
		return new SexyVector2(0f, 0f);
	}

	public SexyVector2 ShiftTargetByEverything(Ball aBall, ref bool bShiftedInTrackDir)
	{
		return ShiftTargetTowardFrog(aBall) + ShiftTargetTowardGuide(aBall, ref bShiftedInTrackDir) + ShiftTargetByBallSpeed(aBall);
	}
}
