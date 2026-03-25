using System;
using System.Collections.Generic;
using System.Linq;
using JeffLib;
using Microsoft.Xna.Framework;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.PIL;

namespace ZumasRevenge;

public class DarkFrogSequence : IDisposable
{
	public enum State
	{
		State_MoveToPosition,
		State_FreakingOut,
		State_MovingForDialog,
		State_Dialog,
		State_Done
	}

	public static float MOVE_TIME = 200f;

	public static float DEST_X = 400f;

	public static float DEST_Y = 532f;

	public static float FROG_CENTERX = 400f;

	public static float DARK_FROG_CENTERY = 98f;

	public static int gDebugEmitterHandle = 0;

	public static float frame_mult = 1.5f;

	public static float GENIE_SMOKE_TRAIL_PARTICLE_REDUCTION_PERCENT = 0.5f;

	protected List<BGElementParams> mBGElementParams = new List<BGElementParams>();

	protected Gun mFrog;

	protected int mUpdateCount;

	protected int mState;

	protected int mBlinkCel;

	protected int mTimer;

	protected float mXDist;

	protected float mYDist;

	protected float mCurXDist;

	protected float mCurYDist;

	protected float mVX;

	protected float mVY;

	protected float mDarkFrogAlpha;

	protected float mDarkFrogX;

	protected float mDarkFrogY;

	protected float mDarkFrogVX;

	protected float mDarkFrogVY;

	protected float mXTrans;

	protected float mYTrans;

	protected float mTongueYOff;

	protected float mTattooAlpha;

	protected bool mMoveTongueDown;

	protected bool mDoTongueFlick;

	protected bool mFadingOut;

	protected bool mStartNextLevel;

	protected LavaShader mBGShader;

	protected List<AfterEffectsTimeline> mTimeline = new List<AfterEffectsTimeline>();

	protected Component mSceneRotation;

	protected SexyFramework.PIL.System mGenieSmoke;

	protected SexyFramework.PIL.System mBoilingSmoke;

	protected PIEffect mTransportFlash;

	protected List<SimpleFadeText> mText = new List<SimpleFadeText>();

	public int mInitialDelay;

	public int mInitialDelayTarget;

	private static float timer = 0f;

	private static int CANVAS_W = 293;

	private static int CANVAS_H = 268;

	public static float GetScale()
	{
		return Common._M(1f);
	}

	public static float FS(float x)
	{
		return x * GetScale();
	}

	protected void SetupStart()
	{
		int num = (int)Common._S(DEST_X);
		int num2 = (int)Common._S(DEST_Y);
		AfterEffectsTimeline afterEffectsTimeline = new AfterEffectsTimeline();
		afterEffectsTimeline.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_1);
		afterEffectsTimeline.mStartFrame = 0;
		afterEffectsTimeline.mEndFrame = (int)FS(73f);
		afterEffectsTimeline.AddPosX(new Component(num, num, 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
		afterEffectsTimeline.AddPosY(new Component(num2, num2, 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
		mTimeline.Add(afterEffectsTimeline);
		int num3 = Common._S(Common._M(-1));
		int num4 = Common._S(Common._M(-5));
		afterEffectsTimeline = new AfterEffectsTimeline();
		afterEffectsTimeline.mImage = Res.GetImageByID(ResID.IMAGE_FROG_NORMAL_EYES);
		afterEffectsTimeline.mCel = 1;
		afterEffectsTimeline.mStartFrame = (int)FS(31f);
		afterEffectsTimeline.mEndFrame = (int)FS(42f);
		afterEffectsTimeline.AddPosX(new Component(num + num3, num + num3, 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
		afterEffectsTimeline.AddPosY(new Component(num2 + num4, num2 + num4, 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
		mTimeline.Add(afterEffectsTimeline);
		afterEffectsTimeline = new AfterEffectsTimeline();
		afterEffectsTimeline.mImage = Res.GetImageByID(ResID.IMAGE_FROG_NORMAL_EYES);
		afterEffectsTimeline.mCel = 1;
		afterEffectsTimeline.mStartFrame = (int)FS(59f);
		afterEffectsTimeline.mEndFrame = (int)FS(73f);
		afterEffectsTimeline.AddPosX(new Component(num + num3, num + num3, 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
		afterEffectsTimeline.AddPosY(new Component(num2 + num4, num2 + num4, 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
		mTimeline.Add(afterEffectsTimeline);
		SetupFrogLooks(0, hold: false);
	}

	protected void SetupShakeItOff()
	{
		int num = (int)Common._S(DEST_X);
		int num2 = (int)Common._S(DEST_Y);
		int num3 = (int)((float)Common._M(261) * GetScale());
		Image[] array = new Image[13]
		{
			Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_3),
			Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_7),
			Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_3),
			Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_2),
			Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_7),
			Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_2),
			Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_3),
			Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_7),
			Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_3),
			Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_2),
			Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_7),
			Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_2),
			Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_3)
		};
		int[] array2 = new int[13]
		{
			(int)FS(20f),
			(int)FS(6f),
			(int)FS(6f),
			(int)FS(4f),
			(int)FS(6f),
			(int)FS(4f),
			(int)FS(4f),
			(int)FS(6f),
			(int)FS(6f),
			(int)FS(4f),
			(int)FS(6f),
			(int)FS(4f),
			(int)FS(4f)
		};
		int num4 = 0;
		for (int i = 0; i < array.Length; i++)
		{
			AfterEffectsTimeline afterEffectsTimeline = new AfterEffectsTimeline();
			afterEffectsTimeline.mImage = array[i];
			afterEffectsTimeline.mStartFrame = num3 + num4;
			afterEffectsTimeline.mEndFrame = afterEffectsTimeline.mStartFrame + array2[i];
			afterEffectsTimeline.AddPosX(new Component(num));
			afterEffectsTimeline.AddPosY(new Component(num2));
			if (i == 4 || i == 10)
			{
				afterEffectsTimeline.mMirror = true;
			}
			num4 += array2[i];
			mTimeline.Add(afterEffectsTimeline);
		}
	}

	protected void SetupFrogLooks(int start_time, bool hold)
	{
		int num = (int)Common._S(DEST_X);
		int num2 = (int)Common._S(DEST_Y);
		float num3 = Common._S(Common._M(0));
		float num4 = (hold ? Common._S(Common._M(-12)) : 0);
		AfterEffectsTimeline afterEffectsTimeline = new AfterEffectsTimeline();
		afterEffectsTimeline.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_4);
		afterEffectsTimeline.mStartFrame = (int)((float)start_time + FS(73f));
		afterEffectsTimeline.mEndFrame = (int)((float)start_time + FS(137f));
		afterEffectsTimeline.mHoldLastFrame = hold;
		afterEffectsTimeline.AddPosX(new Component((float)num + num3, (float)num + num3, 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
		afterEffectsTimeline.AddPosY(new Component((float)num2 + num4, (float)num2 + num4, 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
		mTimeline.Add(afterEffectsTimeline);
		afterEffectsTimeline = new AfterEffectsTimeline();
		afterEffectsTimeline.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_4_PUPIL);
		afterEffectsTimeline.mStartFrame = (int)((float)start_time + FS(73f));
		afterEffectsTimeline.mEndFrame = (int)((float)start_time + FS(137f));
		SexyFramework.Misc.Point[] array = new SexyFramework.Misc.Point[8]
		{
			new SexyFramework.Misc.Point(Common._S(Common._M(-22)), Common._S(Common._M1(-5))),
			new SexyFramework.Misc.Point(Common._S(Common._M2(-26)), Common._S(Common._M3(-8))),
			new SexyFramework.Misc.Point(Common._S(Common._M4(-21)), Common._S(Common._M5(-12))),
			new SexyFramework.Misc.Point(Common._S(Common._M6(-13)), Common._S(Common._M7(-3))),
			new SexyFramework.Misc.Point(Common._S(Common._M(-22)), Common._S(Common._M1(-3))),
			new SexyFramework.Misc.Point(Common._S(Common._M2(-26)), Common._S(Common._M3(-9))),
			new SexyFramework.Misc.Point(Common._S(Common._M(-16)), Common._S(Common._M1(-7))),
			new SexyFramework.Misc.Point(Common._S(Common._M2(-22)), Common._S(Common._M3(-5)))
		};
		int[] array2 = new int[9]
		{
			(int)FS(0f),
			(int)FS(7f),
			(int)FS(16f),
			(int)FS(28f),
			(int)FS(34f),
			(int)FS(40f),
			(int)FS(48f),
			(int)FS(60f),
			afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame
		};
		int num5 = (hold ? 1 : array.Length);
		for (int i = 0; i < num5; i++)
		{
			float val = num + array[0].mX;
			float num6 = num2 + array[0].mY;
			if (i > 0)
			{
				val = num + array[i - 1].mX;
				num6 = num2 + array[i - 1].mY;
			}
			afterEffectsTimeline.AddPosX(new Component(val, num + array[i].mX, array2[i], array2[i + 1]));
			afterEffectsTimeline.AddPosY(new Component(num6 + num4, (float)(num2 + array[i].mY) + num4, array2[i], array2[i + 1]));
		}
		afterEffectsTimeline.mHoldLastFrame = hold;
		mTimeline.Add(afterEffectsTimeline);
		afterEffectsTimeline = new AfterEffectsTimeline();
		afterEffectsTimeline.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_4_PUPIL);
		afterEffectsTimeline.mStartFrame = (int)((float)start_time + FS(73f));
		afterEffectsTimeline.mEndFrame = (int)((float)start_time + FS(137f));
		SexyFramework.Misc.Point[] array3 = new SexyFramework.Misc.Point[8]
		{
			new SexyFramework.Misc.Point(Common._S(Common._M(20)), Common._S(Common._M1(-5))),
			new SexyFramework.Misc.Point(Common._S(Common._M2(16)), Common._S(Common._M3(-8))),
			new SexyFramework.Misc.Point(Common._S(Common._M4(21)), Common._S(Common._M5(-12))),
			new SexyFramework.Misc.Point(Common._S(Common._M6(29)), Common._S(Common._M7(-3))),
			new SexyFramework.Misc.Point(Common._S(Common._M(20)), Common._S(Common._M1(-3))),
			new SexyFramework.Misc.Point(Common._S(Common._M2(16)), Common._S(Common._M3(-9))),
			new SexyFramework.Misc.Point(Common._S(Common._M(26)), Common._S(Common._M1(-7))),
			new SexyFramework.Misc.Point(Common._S(Common._M2(20)), Common._S(Common._M3(-5)))
		};
		for (int j = 0; j < num5; j++)
		{
			float val2 = num + array3[0].mX;
			float num7 = num2 + array3[0].mY;
			if (j > 0)
			{
				val2 = num + array3[j - 1].mX;
				num7 = num2 + array3[j - 1].mY;
			}
			afterEffectsTimeline.AddPosX(new Component(val2, num + array3[j].mX, array2[j], array2[j + 1]));
			afterEffectsTimeline.AddPosY(new Component(num7 + num4, (float)(num2 + array3[j].mY) + num4, array2[j], array2[j + 1]));
		}
		afterEffectsTimeline.mHoldLastFrame = hold;
		mTimeline.Add(afterEffectsTimeline);
	}

	protected void SetupInflato(int start_time, int end_time, bool fade, bool blink)
	{
		int num = (int)Common._S(DEST_X);
		int num2 = (int)Common._S(DEST_Y);
		AfterEffectsTimeline afterEffectsTimeline = new AfterEffectsTimeline();
		afterEffectsTimeline.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_4);
		afterEffectsTimeline.mStartFrame = start_time;
		afterEffectsTimeline.mEndFrame = (int)((float)start_time + FS(36f));
		afterEffectsTimeline.AddPosX(new Component(num, num, 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
		afterEffectsTimeline.AddPosY(new Component(num2, num2, 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
		mTimeline.Add(afterEffectsTimeline);
		float num3 = Common._S(Common._M(-22));
		float num4 = Common._S(Common._M(-5));
		afterEffectsTimeline = new AfterEffectsTimeline();
		afterEffectsTimeline.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_4_PUPIL);
		afterEffectsTimeline.mStartFrame = start_time;
		afterEffectsTimeline.mEndFrame = (int)((float)start_time + FS(16f));
		afterEffectsTimeline.AddPosX(new Component((float)num + num3, (float)num + num3, 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
		afterEffectsTimeline.AddPosY(new Component((float)num2 + num4, (float)num2 + num4, 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
		mTimeline.Add(afterEffectsTimeline);
		num3 = Common._S(Common._M(20));
		afterEffectsTimeline = new AfterEffectsTimeline();
		afterEffectsTimeline.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_4_PUPIL);
		afterEffectsTimeline.mStartFrame = start_time;
		afterEffectsTimeline.mEndFrame = (int)((float)start_time + FS(16f));
		afterEffectsTimeline.AddPosX(new Component((float)num + num3, (float)num + num3, 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
		afterEffectsTimeline.AddPosY(new Component((float)num2 + num4, (float)num2 + num4, 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
		mTimeline.Add(afterEffectsTimeline);
		afterEffectsTimeline = new AfterEffectsTimeline();
		afterEffectsTimeline.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_5);
		afterEffectsTimeline.mStartFrame = (int)((float)start_time + FS(36f));
		afterEffectsTimeline.mEndFrame = (int)((float)start_time + FS(124f));
		afterEffectsTimeline.AddScaleY(new Component(1f, Common._M(0.75f), (int)FS(39f), (int)FS(88f)));
		afterEffectsTimeline.AddPosX(new Component(num, num, 0, (int)FS(124f)));
		afterEffectsTimeline.AddPosY(new Component(num2, num2, 0, (int)FS(75f)));
		if (fade)
		{
			afterEffectsTimeline.AddOpacity(new Component(1f, 0f, (int)FS(84f), (int)FS(88f)));
		}
		mTimeline.Add(afterEffectsTimeline);
		afterEffectsTimeline = new AfterEffectsTimeline();
		afterEffectsTimeline.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_BUGEYE);
		afterEffectsTimeline.mStartFrame = (int)((float)start_time + FS(16f));
		afterEffectsTimeline.mEndFrame = (int)((float)start_time + FS(107f));
		float[] array = new float[6]
		{
			Common._M(0.26f),
			Common._M1(0.45f),
			Common._M2(1.5f),
			Common._M3(1.5f),
			Common._M4(0.169f),
			Common._M5(0.169f)
		};
		float[] array2 = new float[6]
		{
			FS(33f),
			FS(40f),
			FS(46f),
			FS(75f),
			FS(107f),
			afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame
		};
		for (int i = 0; i < array.Length - 1; i++)
		{
			float target = 1f + array[i + 1] - 0.26f;
			float val = 1f + array[i] - 0.26f;
			afterEffectsTimeline.AddScaleX(new Component(val, target, (int)(array2[i] - FS(16f)), (int)(array2[i + 1] - FS(16f))));
		}
		SexyFramework.Misc.Point[] array3 = new SexyFramework.Misc.Point[6]
		{
			new SexyFramework.Misc.Point(Common._S(Common._M(-23)), Common._S(Common._M1(-2))),
			new SexyFramework.Misc.Point(Common._S(Common._M2(-29)), Common._S(Common._M3(-2))),
			new SexyFramework.Misc.Point(Common._S(Common._M4(-29)), Common._S(Common._M5(-3))),
			new SexyFramework.Misc.Point(Common._S(Common._M6(-29)), Common._S(Common._M7(-9))),
			new SexyFramework.Misc.Point(Common._S(Common._M(-29)), Common._S(Common._M1(-9))),
			new SexyFramework.Misc.Point(Common._S(Common._M2(-28)), Common._S(Common._M3(-9)))
		};
		int[] array4 = new int[7]
		{
			(int)FS(35f),
			(int)FS(38f),
			(int)FS(75f),
			(int)FS(95f),
			(int)FS(104f),
			(int)FS(106f),
			(int)FS(106f)
		};
		for (int j = 0; j < array3.Length; j++)
		{
			float val2 = num + array3[0].mX;
			float val3 = num2 + array3[0].mY;
			if (j > 0)
			{
				val2 = num + array3[j - 1].mX;
				val3 = num2 + array3[j - 1].mY;
			}
			afterEffectsTimeline.AddPosX(new Component(val2, num + array3[j].mX, array4[j] - (int)FS(16f), array4[j + 1] - (int)FS(16f)));
			afterEffectsTimeline.AddPosY(new Component(val3, num2 + array3[j].mY, array4[j] - (int)FS(16f), array4[j + 1] - (int)FS(16f)));
		}
		mTimeline.Add(afterEffectsTimeline);
		afterEffectsTimeline = new AfterEffectsTimeline();
		afterEffectsTimeline.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_BUGEYE);
		afterEffectsTimeline.mStartFrame = (int)((float)start_time + FS(16f));
		afterEffectsTimeline.mEndFrame = (int)((float)start_time + FS(107f));
		for (int k = 0; k < array.Length - 1; k++)
		{
			float target2 = 1f + array[k + 1] - 0.26f;
			float val4 = 1f + array[k] - 0.26f;
			afterEffectsTimeline.AddScaleX(new Component(val4, target2, (int)(array2[k] - FS(16f)), (int)(array2[k + 1] - FS(16f))));
		}
		SexyFramework.Misc.Point[] array5 = new SexyFramework.Misc.Point[6]
		{
			new SexyFramework.Misc.Point(Common._S(Common._M(21)), Common._S(Common._M1(-2))),
			new SexyFramework.Misc.Point(Common._S(Common._M2(29)), Common._S(Common._M3(-2))),
			new SexyFramework.Misc.Point(Common._S(Common._M4(29)), Common._S(Common._M5(-3))),
			new SexyFramework.Misc.Point(Common._S(Common._M6(29)), Common._S(Common._M7(-9))),
			new SexyFramework.Misc.Point(Common._S(Common._M(29)), Common._S(Common._M1(-9))),
			new SexyFramework.Misc.Point(Common._S(Common._M2(-27)), Common._S(Common._M3(-9)))
		};
		for (int l = 0; l < array5.Length; l++)
		{
			float val5 = num + array5[0].mX;
			float val6 = num2 + array5[0].mY;
			if (l > 0)
			{
				val5 = num + array5[l - 1].mX;
				val6 = num2 + array5[l - 1].mY;
			}
			afterEffectsTimeline.AddPosX(new Component(val5, num + array5[l].mX, array4[l] - (int)FS(16f), array4[l + 1] - (int)FS(16f)));
			afterEffectsTimeline.AddPosY(new Component(val6, num2 + array5[l].mY, array4[l] - (int)FS(16f), array4[l + 1] - (int)FS(16f)));
		}
		mTimeline.Add(afterEffectsTimeline);
		if (blink)
		{
			int num5 = Common._S(Common._M(-6));
			int num6 = Common._S(Common._M(12));
			int num7 = (int)((float)Common._M(125) * GetScale());
			int num8 = (int)((float)Common._M(153) * GetScale());
			afterEffectsTimeline = new AfterEffectsTimeline();
			afterEffectsTimeline.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_FRAME_4);
			afterEffectsTimeline.mStartFrame = start_time + num7;
			afterEffectsTimeline.mEndFrame = start_time + num8;
			afterEffectsTimeline.AddPosX(new Component(num, num, 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
			afterEffectsTimeline.AddPosY(new Component(num2 + num5, num2 - num6, 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
			afterEffectsTimeline.AddScaleX(new Component(Common._M(1.03f), Common._M1(1f), 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
			afterEffectsTimeline.AddScaleY(new Component(Common._M(0.887f), Common._M1(1f), 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
			mTimeline.Add(afterEffectsTimeline);
			afterEffectsTimeline = new AfterEffectsTimeline();
			afterEffectsTimeline.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_4_BLINK);
			afterEffectsTimeline.mStartFrame = start_time + num7;
			afterEffectsTimeline.mEndFrame = start_time + num8;
			num3 = Common._S(Common._M(-1));
			num4 = Common._S(Common._M(-5));
			afterEffectsTimeline.AddPosX(new Component((float)num + num3, (float)num + num3, 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
			afterEffectsTimeline.AddPosY(new Component((float)(num2 + num5) + num4, (float)num2 + num4 - (float)num6, 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
			afterEffectsTimeline.AddScaleX(new Component(Common._M(1.03f), Common._M1(1f), 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
			afterEffectsTimeline.AddScaleY(new Component(Common._M(0.887f), Common._M1(1f), 0, afterEffectsTimeline.mEndFrame - afterEffectsTimeline.mStartFrame));
			mTimeline.Add(afterEffectsTimeline);
		}
	}

	protected void SetupGenieSmokeTrail()
	{
		mGenieSmoke = new SexyFramework.PIL.System(350, 50);
		if (!GameApp.gApp.Is3DAccelerated())
		{
			mGenieSmoke.mHighWatermark = Common._M(80);
			mGenieSmoke.mLowWatermark = Common._M(30);
			mGenieSmoke.mFPSCallback = SexyFramework.PIL.System.FadeParticlesFPSCallback;
		}
		mGenieSmoke.mScale = Common._S(1f);
		mGenieSmoke.WaitForEmitters(w: true);
		mGenieSmoke.SetLife((int)((float)Common._M(350) * frame_mult));
		Emitter emitter = new Emitter();
		emitter.mCullingRect = new Rect(0, 0, Common._SS(GameApp.gApp.mWidth), Common._SS(GameApp.gApp.mHeight));
		emitter.mEmissionCoordsAreOffsets = true;
		SetupPaths(emitter.mWaypointManager, Common._M(2f));
		emitter.mPreloadFrames = Common._M(0);
		EmitterScale emitterScale = new EmitterScale();
		emitterScale.mNumberScale = Common._M(1f);
		emitterScale.mSizeXScale = Common._M(1.5f);
		emitter.AddScaleKeyFrame(0, emitterScale);
		EmitterSettings emitterSettings = new EmitterSettings();
		emitterSettings.mVisibility = Common._M(0.5f);
		emitterSettings.mEmissionAngle = SexyFramework.Common.DegreesToRadians(Common._M(90));
		emitterSettings.mEmissionRange = SexyFramework.Common.DegreesToRadians(Common._M(333));
		emitter.AddSettingsKeyFrame(0, emitterSettings);
		ParticleType particleType = new ParticleType();
		particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_SMOKE_ANIM);
		particleType.mRandomStartCel = true;
		particleType.mImageRate = 0;
		particleType.mAlignAngleToMotion = Common._M(0) == 1;
		particleType.mColorKeyManager.AddColorKey(0f, new SexyFramework.Graphics.Color(84, 0, 0));
		particleType.mColorKeyManager.AddColorKey(0.125f, new SexyFramework.Graphics.Color(SexyFramework.Graphics.Color.Black));
		particleType.mColorKeyManager.AddColorKey(0.25f, new SexyFramework.Graphics.Color(255, 0, 0));
		particleType.mColorKeyManager.AddColorKey(0.375f, new SexyFramework.Graphics.Color(14, 0, 0));
		particleType.mColorKeyManager.AddColorKey(0.5f, new SexyFramework.Graphics.Color(63, 29, 255));
		particleType.mColorKeyManager.AddColorKey(0.75f, new SexyFramework.Graphics.Color(148, 0, 255));
		particleType.mColorKeyManager.AddColorKey(1f, new SexyFramework.Graphics.Color(SexyFramework.Graphics.Color.Black));
		particleType.mAlphaKeyManager.AddAlphaKey(0f, 255);
		particleType.mAlphaKeyManager.AddAlphaKey(Common._M(0.5f), 255);
		particleType.mAlphaKeyManager.AddAlphaKey(1f, 0);
		ParticleSettings particleSettings = new ParticleSettings();
		particleSettings.mLife = Common._M(30);
		particleSettings.mNumber = (int)((float)Common._M(50) * GENIE_SMOKE_TRAIL_PARTICLE_REDUCTION_PERCENT);
		particleSettings.mXSize = Common._M(18);
		particleSettings.mVelocity = Common._M(5);
		particleSettings.mWeight = Common._M(-4);
		particleType.AddSettingsKeyFrame(0, particleSettings);
		particleSettings = new ParticleSettings(particleSettings);
		particleSettings.mNumber = (int)((float)Common._M(81) * GENIE_SMOKE_TRAIL_PARTICLE_REDUCTION_PERCENT);
		particleType.AddSettingsKeyFrame(Common._M(15), particleSettings);
		particleSettings = new ParticleSettings(particleSettings);
		particleSettings.mNumber = (int)((float)Common._M(46) * GENIE_SMOKE_TRAIL_PARTICLE_REDUCTION_PERCENT);
		particleSettings.mXSize = Common._M(36);
		particleType.AddSettingsKeyFrame(Common._M(101), particleSettings);
		particleSettings = new ParticleSettings(particleSettings);
		particleSettings.mNumber = (int)((float)Common._M(83) * GENIE_SMOKE_TRAIL_PARTICLE_REDUCTION_PERCENT);
		particleSettings.mXSize = Common._M(43);
		particleType.AddSettingsKeyFrame(Common._M(134), particleSettings);
		particleSettings = new ParticleSettings(particleSettings);
		particleSettings.mXSize = Common._M(21);
		particleType.AddSettingsKeyFrame(Common._M(148), particleSettings);
		particleSettings = new ParticleSettings(particleSettings);
		particleSettings.mXSize = Common._M(55);
		particleType.AddSettingsKeyFrame(Common._M(199), particleSettings);
		ParticleVariance particleVariance = new ParticleVariance();
		particleVariance.mLifeVar = Common._M(9);
		particleVariance.mNumberVar = Common._M(44);
		particleVariance.mSizeXVar = Common._M(3);
		particleVariance.mVelocityVar = Common._M(10);
		particleVariance.mWeightVar = Common._M(6);
		particleType.AddVarianceKeyFrame(0, particleVariance);
		LifetimeSettings lifetimeSettings = new LifetimeSettings();
		lifetimeSettings.mSizeXMult = Common._M(2f);
		particleType.AddSettingAtLifePct(0f, lifetimeSettings);
		lifetimeSettings = new LifetimeSettings(lifetimeSettings);
		lifetimeSettings.mSizeXMult = Common._M(1.3f);
		particleType.AddSettingAtLifePct(0.62f, lifetimeSettings);
		lifetimeSettings = new LifetimeSettings(lifetimeSettings);
		lifetimeSettings.mSizeXMult = 0f;
		particleType.AddSettingAtLifePct(1f, lifetimeSettings);
		emitter.AddParticleType(particleType);
		mGenieSmoke.AddEmitter(emitter);
	}

	protected void SetupBoilingSmoke()
	{
		mBoilingSmoke = new SexyFramework.PIL.System(100, 50);
		if (!GameApp.gApp.Is3DAccelerated())
		{
			mBoilingSmoke.mHighWatermark = Common._M(80);
			mBoilingSmoke.mLowWatermark = Common._M(30);
			mBoilingSmoke.mFPSCallback = SexyFramework.PIL.System.FadeParticlesFPSCallback;
		}
		mBoilingSmoke.mScale = Common._S(1f);
		mBoilingSmoke.WaitForEmitters(w: true);
		mBoilingSmoke.SetLife((int)((float)Common._M(240) * frame_mult));
		Emitter emitter = new Emitter();
		emitter.mCullingRect = new Rect(0, 0, Common._SS(GameApp.gApp.mWidth), Common._SS(GameApp.gApp.mHeight));
		emitter.mEmissionCoordsAreOffsets = true;
		SetupPaths(emitter.mWaypointManager, Common._M(2f));
		emitter.mPreloadFrames = Common._M(0);
		EmitterScale emitterScale = new EmitterScale();
		emitterScale.mLifeScale = Common._M(0.79f);
		emitterScale.mNumberScale = Common._M(0.45f);
		emitterScale.mSizeXScale = Common._M(0.31f);
		emitterScale.mZoom = Common._M(1.49f);
		emitter.AddScaleKeyFrame(0, emitterScale);
		emitterScale = new EmitterScale(emitterScale);
		emitterScale.mSizeXScale = Common._M(2.04f);
		emitter.AddScaleKeyFrame((int)((float)Common._M(110) * frame_mult), emitterScale);
		EmitterSettings emitterSettings = new EmitterSettings();
		emitterSettings.mEmissionAngle = SexyFramework.Common.DegreesToRadians(Common._M(92));
		emitter.AddSettingsKeyFrame(0, emitterSettings);
		ParticleType particleType = new ParticleType();
		particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_SMOKE_COLOR);
		particleType.mAngleRange = (float)Math.PI * 2f;
		particleType.mFlipY = true;
		particleType.mColorKeyManager.AddColorKey(0f, new SexyFramework.Graphics.Color(120, 120, 120));
		particleType.mColorKeyManager.AddColorKey(1f, new SexyFramework.Graphics.Color(SexyFramework.Graphics.Color.Black));
		particleType.mAlphaKeyManager.AddAlphaKey(0f, 0);
		particleType.mAlphaKeyManager.AddAlphaKey(Common._M(0.1f), 255);
		particleType.mAlphaKeyManager.AddAlphaKey(Common._M(0.75f), 255);
		particleType.mAlphaKeyManager.AddAlphaKey(1f, 0);
		ParticleSettings particleSettings = new ParticleSettings();
		particleSettings.mLife = Common._M(9);
		particleSettings.mNumber = Common._M(60);
		particleSettings.mXSize = Common._M(30);
		particleSettings.mVelocity = Common._M(16);
		particleSettings.mWeight = Common._M(-13);
		particleType.AddSettingsKeyFrame(0, particleSettings);
		ParticleVariance particleVariance = new ParticleVariance();
		particleVariance.mLifeVar = Common._M(9);
		particleVariance.mNumberVar = Common._M(48);
		particleVariance.mSizeXVar = Common._M(3);
		particleVariance.mVelocityVar = Common._M(10);
		particleVariance.mSpinVar = SexyFramework.Common.DegreesToRadians(Common._M(12));
		particleVariance.mMotionRandVar = Common._M(18);
		particleType.AddVarianceKeyFrame(0, particleVariance);
		LifetimeSettings lifetimeSettings = new LifetimeSettings();
		lifetimeSettings.mSizeXMult = Common._M(0.6f);
		particleType.AddSettingAtLifePct(0f, lifetimeSettings);
		lifetimeSettings = new LifetimeSettings(lifetimeSettings);
		lifetimeSettings.mSizeXMult = Common._M(2f);
		lifetimeSettings.mWeightMult = 0f;
		particleType.AddSettingAtLifePct(0.5f, lifetimeSettings);
		lifetimeSettings = new LifetimeSettings(lifetimeSettings);
		lifetimeSettings.mWeightMult = 1f;
		particleType.AddSettingAtLifePct(1f, lifetimeSettings);
		emitter.AddParticleType(particleType);
		particleType = new ParticleType();
		particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_BLOTCHES);
		particleType.mFlipY = true;
		particleType.mRandomStartCel = true;
		particleType.mImageRate = Common._M(4);
		particleType.mAngleRange = (float)Math.PI * 2f;
		particleType.mColorKeyManager.AddColorKey(0f, new SexyFramework.Graphics.Color(56, 56, 56));
		particleType.mColorKeyManager.AddColorKey(1f, new SexyFramework.Graphics.Color(SexyFramework.Graphics.Color.Black));
		particleType.mAlphaKeyManager.AddAlphaKey(0f, 0);
		particleType.mAlphaKeyManager.AddAlphaKey(Common._M(0.1f), 255);
		particleType.mAlphaKeyManager.AddAlphaKey(Common._M(0.75f), 255);
		particleType.mAlphaKeyManager.AddAlphaKey(1f, 0);
		particleSettings = new ParticleSettings();
		particleSettings.mLife = Common._M(9);
		particleSettings.mNumber = Common._M(60);
		particleSettings.mXSize = Common._M(20);
		particleSettings.mVelocity = Common._M(16);
		particleSettings.mWeight = Common._M(-13);
		particleType.AddSettingsKeyFrame(0, particleSettings);
		particleVariance = new ParticleVariance(particleVariance);
		particleType.AddVarianceKeyFrame(0, particleVariance);
		lifetimeSettings = new LifetimeSettings();
		lifetimeSettings.mSizeXMult = Common._M(0.6f);
		particleType.AddSettingAtLifePct(0f, lifetimeSettings);
		lifetimeSettings = new LifetimeSettings(lifetimeSettings);
		lifetimeSettings.mSizeXMult = Common._M(2f);
		lifetimeSettings.mWeightMult = 0f;
		particleType.AddSettingAtLifePct(0.5f, lifetimeSettings);
		lifetimeSettings = new LifetimeSettings(lifetimeSettings);
		lifetimeSettings.mWeightMult = 1f;
		particleType.AddSettingAtLifePct(1f, lifetimeSettings);
		emitter.AddParticleType(particleType);
		gDebugEmitterHandle = mBoilingSmoke.AddEmitter(emitter);
	}

	protected void SetupPaths(WaypointManager w, float mult_override)
	{
		float num = ((mult_override == 0f) ? frame_mult : mult_override);
		int num2 = Common._M(0);
		int num3 = Common._M(0);
		SexyVector2[] array = new SexyVector2[7]
		{
			new SexyVector2(Common._M(400), Common._M1(530)),
			new SexyVector2(Common._M2(553), Common._M3(554)),
			new SexyVector2(Common._M4(638), Common._M5(467)),
			new SexyVector2(Common._M6(619), Common._M7(327)),
			new SexyVector2(Common._M8(558), Common._M9(244)),
			new SexyVector2(Common._M(439), Common._M1(199)),
			new SexyVector2(Common._M2(400), Common._M3(98))
		};
		int[] array2 = new int[7]
		{
			Common._M(0),
			Common._M1(38),
			Common._M2(76),
			Common._M3(114),
			Common._M4(152),
			Common._M5(190),
			Common._M6(228)
		};
		for (int i = 0; i < array.Length; i++)
		{
			w.AddPoint((int)((float)array2[i] * num), new Vector2(array[i].x + (float)num2, array[i].y + (float)num3), linear: true);
		}
		w.Init(make_curve_image: true);
	}

	protected void SetupPaths(WaypointManager w)
	{
		SetupPaths(w, 0f);
	}

	public DarkFrogSequence()
	{
		mUpdateCount = 0;
		mFrog = null;
		mState = 0;
		mVX = (mVY = 0f);
		mTimer = 0;
		mGenieSmoke = null;
		mBoilingSmoke = null;
		mTransportFlash = null;
		mFadingOut = false;
		mInitialDelayTarget = 1;
		mBGShader = null;
	}

	public virtual void Dispose()
	{
		if (mGenieSmoke != null)
		{
			mGenieSmoke.Dispose();
			mGenieSmoke = null;
		}
		if (mBoilingSmoke != null)
		{
			mBoilingSmoke.Dispose();
			mBoilingSmoke = null;
		}
		if (mTransportFlash != null)
		{
			mTransportFlash.Dispose();
			mTransportFlash = null;
		}
		if (mBGShader != null)
		{
			mBGShader = null;
		}
	}

	public void Update()
	{
		mInitialDelay++;
		if (mInitialDelay < mInitialDelayTarget)
		{
			return;
		}
		if (mState == 3 && !mText.Last().mFadeIn && mText.Last().mAlpha <= 0f)
		{
			for (int i = 0; i < mBGElementParams.Count; i++)
			{
				BGElementParams bGElementParams = mBGElementParams[i];
				bGElementParams.mDistAmt += bGElementParams.mDistAmtInc;
				bGElementParams.mScroll += bGElementParams.mScrollAmtInc;
				bGElementParams.mScale += bGElementParams.mScaleAmtInc;
			}
		}
		if (mState == 0)
		{
			mCurXDist += Math.Abs(mVX);
			mCurYDist += Math.Abs(mVY);
			if (++mTimer == (int)MOVE_TIME || (mCurXDist >= Math.Abs(mXDist) && mCurYDist >= Math.Abs(mYDist)))
			{
				mState = 1;
				mFrog.SetPos((int)DEST_X, (int)DEST_Y);
			}
			mXTrans = 0f - mXDist + mVX * (float)mTimer;
			mYTrans = 0f - mYDist + mVY * (float)mTimer;
		}
		else if (mState == 1)
		{
			mUpdateCount++;
			if (mSceneRotation.Active(mUpdateCount))
			{
				mSceneRotation.Update();
			}
			for (int j = 0; j < mTimeline.Count; j++)
			{
				mTimeline[j].Update();
			}
		}
		else if (mState == 2)
		{
			mUpdateCount++;
			mDarkFrogX += mDarkFrogVX;
			mDarkFrogY += mDarkFrogVY;
			if (++mTimer == Common._M(10))
			{
				mFadingOut = true;
				mState++;
				mTimer = 0;
			}
			mXTrans += mVX;
			mYTrans += mVY;
		}
		else if (mState == 3)
		{
			for (int k = 0; k < mText.Count; k++)
			{
				SimpleFadeText simpleFadeText = mText[k];
				if (simpleFadeText.mFadeIn)
				{
					simpleFadeText.mAlpha += Common._M(1.5f);
					if (simpleFadeText.mAlpha > 255f)
					{
						simpleFadeText.mAlpha = 255f;
					}
					if (simpleFadeText.mAlpha < (float)Common._M(128))
					{
						break;
					}
				}
				else
				{
					simpleFadeText.mAlpha -= Common._M(2f);
					if (simpleFadeText.mAlpha <= 0f)
					{
						simpleFadeText.mAlpha = 0f;
					}
				}
			}
			if (!mText.Last().mFadeIn && mText.Last().mAlpha <= 0f && ++mTimer == Common._M(170))
			{
				mState = 4;
			}
			if (mText.Last().mFadeIn && mText.Last().mAlpha >= 255f && ++mTimer >= Common._M(300))
			{
				for (int l = 0; l < mText.Count; l++)
				{
					mText[l].mFadeIn = false;
				}
				mTimer = 0;
			}
		}
		if ((float)mUpdateCount > (float)Common._M(450) * GetScale())
		{
			mGenieSmoke.Update();
			mBoilingSmoke.Update();
		}
		if ((float)mUpdateCount > (float)Common._M(1250) * GetScale())
		{
			mTattooAlpha -= Common._M(3f);
		}
		if (!((float)mUpdateCount > (float)Common._M(500) * GetScale()))
		{
			return;
		}
		if ((float)mUpdateCount > (float)Common._M(1000) * GetScale())
		{
			mDarkFrogAlpha += Common._M(2f);
		}
		if ((float)mUpdateCount > (float)Common._M(1200) * GetScale() && (float)mUpdateCount < (float)Common._M1(1250) * GetScale() && mUpdateCount % Common._M2(5) == 0)
		{
			mBlinkCel--;
		}
		if ((float)mUpdateCount > (float)Common._M(950) * GetScale())
		{
			mTransportFlash.mDrawTransform.LoadIdentity();
			float num = GameApp.DownScaleNum(1f);
			mTransportFlash.mDrawTransform.Scale(num, num);
			mTransportFlash.mDrawTransform.Translate(Common._DS(Common._M(800)), Common._DS(Common._M1(220)));
			mTransportFlash.Update();
		}
		if ((float)mUpdateCount == (float)Common._M(1300) * GetScale())
		{
			mBlinkCel = 0;
			mDoTongueFlick = true;
		}
		if (mDoTongueFlick)
		{
			float num2 = Common._M(2f);
			if (mMoveTongueDown && (mTongueYOff += num2) >= (float)Common._M(60))
			{
				mBlinkCel = -1;
				mMoveTongueDown = false;
			}
			else if (!mMoveTongueDown && (mTongueYOff -= num2) <= 0f)
			{
				mDoTongueFlick = false;
				mState = 2;
				mTimer = 0;
				mCurXDist = (mCurYDist = 0f);
				mVX = (FROG_CENTERX - DEST_X) / MOVE_TIME;
				mVY = 0f;
				mDarkFrogVX = (FROG_CENTERX - mDarkFrogX) / MOVE_TIME;
				mDarkFrogVY = (DARK_FROG_CENTERY - mDarkFrogY) / MOVE_TIME;
			}
		}
	}

	public void Draw(Graphics g)
	{
		if (mInitialDelay < mInitialDelayTarget)
		{
			return;
		}
		float bGAlpha = GetBGAlpha();
		g.SetColor(0, 0, 0, (int)bGAlpha);
		if (Common._M(1) == 1)
		{
			g.FillRect(Common._S(-80), 0, GameApp.gApp.mWidth + Common._S(160), GameApp.gApp.mHeight);
		}
		timer += Common._M(0.01f);
		for (int i = 0; i < 9; i++)
		{
			BGElementParams bGElementParams = mBGElementParams[i];
			if (bGAlpha != 255f && bGAlpha != 0f)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)bGAlpha);
			}
			g.DrawImage(bGElementParams.mImg, bGElementParams.mX, bGElementParams.mY);
			g.SetColorizeImages(colorizeImages: false);
		}
		Graphics3D graphics3D = g.Get3D();
		mBoilingSmoke.Draw(g);
		mGenieSmoke.Draw(g);
		float num = (mSceneRotation.Active(mUpdateCount) ? mSceneRotation.mValue : 0f);
		SexyTransform2D sexyTransform2D = new SexyTransform2D(init: false);
		if (num != 0f)
		{
			float num2 = Common._DS(100);
			Ratio aspectRatio = GameApp.gApp.mGraphicsDriver.GetAspectRatio();
			if (aspectRatio.mNumerator == 3 && aspectRatio.mDenominator == 4)
			{
				num2 = 0f;
			}
			if (graphics3D != null)
			{
				sexyTransform2D.Translate((float)Common._S(-mFrog.GetCenterX()) - num2, Common._S(-mFrog.GetCenterY()));
				sexyTransform2D.RotateDeg(num);
				sexyTransform2D.Translate((float)Common._S(mFrog.GetCenterX()) + num2, Common._S(mFrog.GetCenterY()));
				graphics3D.PushTransform(sexyTransform2D);
			}
		}
		SexyTransform2D sexyTransform2D2 = new SexyTransform2D(init: false);
		if (mState != 1)
		{
			if (graphics3D != null)
			{
				sexyTransform2D2.Translate(Common._S(mXTrans), Common._S(mYTrans));
				graphics3D.PushTransform(sexyTransform2D2);
			}
			else
			{
				g.PushState();
				g.Translate((int)Common._S(mXTrans), (int)Common._S(mYTrans));
			}
		}
		for (int j = 0; j < mTimeline.Count; j++)
		{
			mTimeline[j].Draw(g, (int)bGAlpha);
		}
		if (graphics3D != null)
		{
			if (num != 0f)
			{
				graphics3D.PopTransform();
			}
			if (mState != 1)
			{
				graphics3D.PopTransform();
			}
		}
		else if (mState != 1)
		{
			g.PopState();
		}
		int num3 = ((mDarkFrogAlpha < 255f) ? ((int)mDarkFrogAlpha) : 255);
		if (num3 != 255)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, num3);
		}
		int num4 = (int)(mDarkFrogX * 2f - (float)(CANVAS_W / 2));
		int num5 = (int)(mDarkFrogY * 2f - (float)(CANVAS_H / 2));
		Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_BACK);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_TOP);
		g.DrawImage(imageByID, Common._DS(num4 + Res.GetOffsetXByID(ResID.IMAGE_BOSS_DARKFROG_BACK)), Common._DS(num5 + Res.GetOffsetYByID(ResID.IMAGE_BOSS_DARKFROG_BACK)));
		g.DrawImage(imageByID2, Common._DS(num4 + Res.GetOffsetXByID(ResID.IMAGE_BOSS_DARKFROG_TOP)), Common._DS(num5 + Res.GetOffsetYByID(ResID.IMAGE_BOSS_DARKFROG_TOP)));
		if (mBlinkCel >= 0)
		{
			ResID id = ((mBlinkCel == 0) ? ResID.IMAGE_BOSS_DARKFROG_BLINK2 : ResID.IMAGE_BOSS_DARKFROG_BLINK1);
			Image imageByID3 = Res.GetImageByID(id);
			g.DrawImage(imageByID3, Common._DS(num4 + Res.GetOffsetXByID(id)), Common._DS(num5 + Res.GetOffsetYByID(id)));
		}
		g.SetColorizeImages(colorizeImages: false);
		g.PushState();
		mTransportFlash.Draw(g);
		g.PopState();
		if (mDarkFrogAlpha >= 255f)
		{
			num3 = (int)mTattooAlpha;
			if (num3 < 0)
			{
				num3 = 0;
			}
		}
		if (num3 != 255)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, num3);
		}
		Image imageByID4 = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_TAT1);
		Image imageByID5 = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_TAT2);
		Image imageByID6 = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_TONGUE);
		g.DrawImage(imageByID4, Common._DS(num4 + Res.GetOffsetXByID(ResID.IMAGE_BOSS_DARKFROG_TAT1)), Common._DS(num5 + Res.GetOffsetYByID(ResID.IMAGE_BOSS_DARKFROG_TAT1)));
		g.DrawImage(imageByID5, Common._DS(num4 + Res.GetOffsetXByID(ResID.IMAGE_BOSS_DARKFROG_TAT2)), Common._DS(num5 + Res.GetOffsetYByID(ResID.IMAGE_BOSS_DARKFROG_TAT2)));
		num3 = ((mDarkFrogAlpha < 255f) ? ((int)mDarkFrogAlpha) : 255);
		g.SetColor(255, 255, 255, num3);
		g.DrawImage(imageByID6, Common._DS(num4 + Res.GetOffsetXByID(ResID.IMAGE_BOSS_DARKFROG_TONGUE)), Common._DS(Common._M(0) + num5 + Res.GetOffsetYByID(ResID.IMAGE_BOSS_DARKFROG_TONGUE)));
		g.SetColorizeImages(colorizeImages: false);
		if (mState != 3)
		{
			return;
		}
		Font fontByID = Res.GetFontByID(ResID.FONT_BOSS_TAUNT);
		for (int k = 0; k < mText.Count; k++)
		{
			if (mText[k].mAlpha > 0f)
			{
				g.SetFont(fontByID);
				g.SetColor(255, 255, 255, (int)mText[k].mAlpha);
				g.DrawString(mText[k].mString, (GameApp.gApp.mWidth - fontByID.StringWidth(mText[k].mString)) / 2 - GameApp.gApp.mBoardOffsetX, Common._S(Common._M(300)) + k * fontByID.mHeight);
			}
		}
	}

	public void Init()
	{
		string[] array = new string[3]
		{
			TextManager.getInstance().getString(436),
			TextManager.getInstance().getString(437),
			TextManager.getInstance().getString(438)
		};
		if (GameApp.gApp.GetBoard().IsHardAdventureMode())
		{
			array[0] = TextManager.getInstance().getString(439);
			array[1] = TextManager.getInstance().getString(440);
			array[2] = TextManager.getInstance().getString(441);
		}
		for (int i = 0; i < array.Length; i++)
		{
			SimpleFadeText simpleFadeText = new SimpleFadeText();
			mText.Add(simpleFadeText);
			simpleFadeText.mString = array[i];
			simpleFadeText.mAlpha = 0f;
			simpleFadeText.mFadeIn = true;
		}
		mInitialDelay = 0;
		mFadingOut = false;
		DARK_FROG_CENTERY = Common._M(98f);
		FROG_CENTERX = Common._M(400f);
		MOVE_TIME = Common._M(200f);
		mTimer = 0;
		mStartNextLevel = true;
		mDoTongueFlick = false;
		mDarkFrogAlpha = 0f;
		mDarkFrogX = FROG_CENTERX;
		mDarkFrogY = DARK_FROG_CENTERY;
		mXTrans = (mYTrans = 0f);
		mDarkFrogVX = (mDarkFrogVY = 0f);
		mBlinkCel = 1;
		mTongueYOff = 0f;
		mTattooAlpha = 255f;
		mMoveTongueDown = true;
		mState = 0;
		mUpdateCount = 0;
		mFrog = GameApp.gApp.GetBoard().GetGun();
		mTimeline.Clear();
		mCurXDist = (mCurYDist = 0f);
		mXDist = DEST_X - (float)mFrog.GetCenterX();
		mYDist = DEST_Y - (float)mFrog.GetCenterY();
		mVX = mXDist / MOVE_TIME;
		mVY = mYDist / MOVE_TIME;
		SetupStart();
		SetupShakeItOff();
		SetupInflato((int)FS(137f), (int)FS(244f), fade: false, blink: false);
		SetupFrogLooks((int)((float)Common._M(269) * GetScale()), hold: false);
		int num = Common._M(374);
		SetupInflato((int)((float)num * GetScale()), (int)FS(num + 153), fade: false, blink: true);
		SetupFrogLooks((int)((float)num + (float)Common._M(80) * GetScale()), hold: true);
		mSceneRotation = new Component(0f, Common._M(360), (int)FS(179f), (int)FS(261f));
		SetupGenieSmokeTrail();
		SetupBoilingSmoke();
		mTransportFlash = GameApp.gApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_FROGFOG").Duplicate();
		for (int j = 0; j < 9; j++)
		{
			BGElementParams bGElementParams = new BGElementParams();
			mBGElementParams.Add(bGElementParams);
			ResID id = (ResID)(418 + j);
			bGElementParams.mImg = Res.GetImageByID(id);
			bGElementParams.mX = Common._DS(Res.GetOffsetXByID(id) - 160);
			bGElementParams.mY = Common._DS(Res.GetOffsetYByID(id));
			bGElementParams.mDistAmt = SexyFramework.Common.FloatRange(Common._M(0.0005f), Common._M1(0.001f));
			bGElementParams.mScale = SexyFramework.Common.FloatRange(Common._M(0.05f), Common._M1(0.1f));
			bGElementParams.mScroll = SexyFramework.Common.FloatRange(Common._M(0.1f), Common._M1(0.15f));
			float num2 = 170f;
			bGElementParams.mDistAmtInc = (Common._M(0.01f) - bGElementParams.mDistAmt) / num2;
			bGElementParams.mScaleAmtInc = (Common._M(0.01f) - bGElementParams.mScale) / num2;
			bGElementParams.mScrollAmtInc = (Common._M(0.5f) - bGElementParams.mScroll) / num2;
		}
	}

	public float GetBGAlpha()
	{
		int num = 0;
		num = ((mState == 3 && !mText.Last().mFadeIn) ? (255 - (int)((float)mTimer * Common._M(1.5f))) : ((mState != 0) ? 255 : ((int)((float)mTimer * Common._M(1.5f)))));
		if (num < 0)
		{
			num = 0;
		}
		else if (num > 255)
		{
			num = 255;
		}
		return num;
	}

	public float GetMoveXAmt()
	{
		if (mState == 0)
		{
			return DEST_X - mXDist + mVX * (float)mTimer;
		}
		if (mState > 0)
		{
			return DEST_X - mXDist + mVX * MOVE_TIME;
		}
		return 0f;
	}

	public float GetMoveYAmt()
	{
		if (mState == 0)
		{
			return DEST_Y - mYDist + mVY * (float)mTimer;
		}
		if (mState > 0)
		{
			return DEST_Y - mYDist + mVY * MOVE_TIME;
		}
		return 0f;
	}

	public bool Done()
	{
		return mState == 4;
	}

	public bool CanStartNextLevel()
	{
		if (mFadingOut && mStartNextLevel)
		{
			mStartNextLevel = false;
			mFrog.SetPos((int)FROG_CENTERX, (int)DEST_Y);
			mFrog.SetDestAngle(-3.14159f);
			GameApp.gApp.GetBoard().mContinueNextLevelOnLoadProfile = false;
			return true;
		}
		return false;
	}

	public bool FadingOut()
	{
		return mFadingOut;
	}

	public bool FadingIn()
	{
		if (mState == 0)
		{
			return (float)mTimer < 255f / Common._M(1.5f);
		}
		return false;
	}

	public bool FadingToLevel()
	{
		if (mState == 3)
		{
			return GetBGAlpha() < 255f;
		}
		return false;
	}
}
