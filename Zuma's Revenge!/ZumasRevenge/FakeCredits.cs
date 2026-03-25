using System;
using System.Collections.Generic;
using System.Linq;
using JeffLib;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class FakeCredits : IDisposable
{
	public enum State
	{
		State_FrogFlyOut,
		State_WoodClose,
		State_FrogFlyIn,
		State_WoodOpen,
		State_TextDropIn,
		State_ScrollCredits,
		State_DropIn,
		State_TextSmashed,
		State_FadeOut,
		State_TauntText,
		State_NextLevel
	}

	private static int MAX_CREDITS = 10;

	private static int MAX_HEADER_LETTERS = 8;

	private static int MAX_BOTTOM_WOOD_COUNT = 5;

	protected List<TikiComponent> mTikis = new List<TikiComponent>();

	protected List<SimpleFadeText> mTauntText = new List<SimpleFadeText>();

	protected List<RockBits> mRockBits = new List<RockBits>();

	protected FrogFlyOff mFrogEffect;

	protected FakeCreditsText[] mCredits = new FakeCreditsText[MAX_CREDITS];

	protected HeaderLetter[] mHeaderLetters = new HeaderLetter[MAX_HEADER_LETTERS];

	protected float mTopWoodY;

	protected float mBottomWoodY;

	protected float mTopWoodShakeY;

	protected float mBGAlpha;

	protected int mBottomWoodBounceDir;

	protected int mBottomWoodBounceCount;

	protected int mState;

	protected int mTimer;

	protected int mUpdateCount;

	protected int mScreenShakeTimer;

	protected float mCreditsGameY;

	protected float mCreditsWinY;

	protected float mBossY;

	protected float mFadeOutAlpha;

	protected bool mDone;

	protected bool mCanDoNextLevel;

	protected bool mNextLevelLoaded;

	public int mBossX;

	public FakeCredits()
	{
		mUpdateCount = 0;
		mFrogEffect = null;
		for (int i = 0; i < MAX_CREDITS; i++)
		{
			mCredits[i] = new FakeCreditsText();
		}
		for (int j = 0; j < MAX_HEADER_LETTERS; j++)
		{
			mHeaderLetters[j] = new HeaderLetter();
		}
	}

	public virtual void Dispose()
	{
		if (mFrogEffect != null)
		{
			mFrogEffect.Dispose();
			mFrogEffect = null;
		}
		for (int i = 0; i < mTikis.Count; i++)
		{
			if (mTikis[i].mSmoke != null)
			{
				mTikis[i].mSmoke.Dispose();
				mTikis[i].mSmoke = null;
			}
		}
		mTikis.Clear();
	}

	public void Init(Gun frog)
	{
		mTauntText.Clear();
		string[] array = new string[3]
		{
			TextManager.getInstance().getString(459),
			TextManager.getInstance().getString(460),
			TextManager.getInstance().getString(461)
		};
		if (GameApp.gApp.GetBoard().IsHardAdventureMode())
		{
			array[0] = TextManager.getInstance().getString(462);
			array[1] = TextManager.getInstance().getString(463);
			array[2] = TextManager.getInstance().getString(464);
		}
		for (int i = 0; i < array.Length; i++)
		{
			SimpleFadeText simpleFadeText = new SimpleFadeText();
			mTauntText.Add(simpleFadeText);
			simpleFadeText.mString = array[i];
			simpleFadeText.mAlpha = 0f;
			simpleFadeText.mFadeIn = true;
		}
		mBossX = (GameApp.gApp.mWidth - Res.GetImageByID(ResID.IMAGE_BOSS_CREDITS_STONE_BOSS).GetCelWidth()) / 2 - GameApp.gApp.mBoardOffsetX;
		if (mFrogEffect != null)
		{
			mFrogEffect.Dispose();
			mFrogEffect = null;
		}
		mFrogEffect = new FrogFlyOff();
		mFrogEffect.JumpOut(frog);
		mTopWoodY = -Res.GetImageByID(ResID.IMAGE_BOSS_CREDITS_WOOD_TOP).mHeight;
		mBottomWoodY = GameApp.gApp.mHeight;
		mTopWoodShakeY = 0f;
		mState = 0;
		mBottomWoodBounceDir = 0;
		mBottomWoodBounceCount = 0;
		mBGAlpha = 0f;
		mTimer = 0;
		mScreenShakeTimer = 0;
		mCanDoNextLevel = (mDone = false);
		mFadeOutAlpha = 0f;
		mBossY = -Res.GetImageByID(ResID.IMAGE_BOSS_CREDITS_STONE_BOSS).mHeight * 3;
		for (int j = 0; j < MAX_CREDITS; j++)
		{
			mCredits[j].mImage = Res.GetImageByID((ResID)(1055 + j));
			mCredits[j].mX = ((j == 0) ? Common._M(206) : Common._M1(136));
			mCredits[j].mY = ((j == 0) ? Common._M(286) : (Common._M1(362) + (j - 1) * Common._M2(50)));
			mCredits[j].mAngle = 0f;
			mCredits[j].mFalling = false;
		}
		mRockBits.Clear();
		Point[] array2 = new Point[8]
		{
			new Point(Common._M(170), Common._M1(45)),
			new Point(Common._M2(225), Common._M3(35)),
			new Point(Common._M4(327), Common._M5(31)),
			new Point(Common._M6(384), Common._M7(20)),
			new Point(Common._M8(474), Common._M9(12)),
			new Point(Common._M(274), Common._M1(112)),
			new Point(Common._M2(231), Common._M3(108)),
			new Point(Common._M4(455), Common._M5(126))
		};
		for (int k = 0; k < MAX_HEADER_LETTERS; k++)
		{
			mHeaderLetters[k] = new HeaderLetter(Res.GetImageByID((ResID)(1070 + k)));
			mHeaderLetters[k].mAngleInc = SexyFramework.Common.DegreesToRadians(SexyFramework.Common.FloatRange(Common._M(10f), Common._M1(15f))) * (float)((SexyFramework.Common.Rand(100) < 50) ? 1 : (-1));
			mHeaderLetters[k].mX = array2[k].mX;
			mHeaderLetters[k].mY = array2[k].mY;
			if (k == MAX_HEADER_LETTERS - 1)
			{
				mHeaderLetters[k].mHinge = true;
				mHeaderLetters[k].mAngleInc = Common._M(-0.1f);
				mHeaderLetters[k].mAngleAccel = Common._M(0.01f);
			}
			float num = SexyFramework.Common.FloatRange(Common._M(8f), Common._M1(12f));
			float num2 = (float)Math.PI * SexyFramework.Common.FloatRange(Common._M(0f), Common._M1(2f));
			mHeaderLetters[k].mVX = num * (float)Math.Cos(num2);
			mHeaderLetters[k].mVY = (0f - num) * (float)Math.Sin(num2);
		}
		mNextLevelLoaded = false;
		mCreditsGameY = -Res.GetImageByID(ResID.IMAGE_BOSS_CREDITS_TEXT_GAME).mHeight;
		mCreditsWinY = -Res.GetImageByID(ResID.IMAGE_BOSS_CREDITS_TEXT_YOU).mHeight;
		Point[] array3 = new Point[16]
		{
			new Point(1852, 296),
			new Point(2046, 1264),
			new Point(106, 708),
			new Point(1070, 712),
			new Point(1378, 70),
			new Point(502, 832),
			new Point(842, 724),
			new Point(1806, 782),
			new Point(112, 1274),
			new Point(420, 1372),
			new Point(960, 194),
			new Point(1518, 1240),
			new Point(486, 284),
			new Point(6, 98),
			new Point(956, 1260),
			new Point(1396, 648)
		};
		float[] array4 = new float[16]
		{
			0.08f, 0.08f, 0.09f, 0.07f, 0.07f, 0.12f, 0.08f, 0.08f, 0.09f, 0.09f,
			0.09f, 0.09f, 0.1f, 0.07f, 0.08f, 0.1f
		};
		int[] array5 = new int[16]
		{
			1, 1, 2, 3, 3, 4, 5, 5, 5, 6,
			6, 7, 7, 8, 8, 8
		};
		Point[] array6 = new Point[16]
		{
			new Point(1854, 326),
			new Point(int.MaxValue, int.MaxValue),
			new Point(112, 788),
			new Point(int.MaxValue, int.MaxValue),
			new Point(int.MaxValue, int.MaxValue),
			new Point(498, 852),
			new Point(846, 814),
			new Point(1814, 875),
			new Point(int.MaxValue, int.MaxValue),
			new Point(int.MaxValue, int.MaxValue),
			new Point(968, 326),
			new Point(int.MaxValue, int.MaxValue),
			new Point(492, 374),
			new Point(16, 170),
			new Point(int.MaxValue, int.MaxValue),
			new Point(1400, 709)
		};
		FPoint[] array7 = new FPoint[16]
		{
			new FPoint(1f, 1f),
			new FPoint(1f, 1f),
			new FPoint(1f, 1f),
			new FPoint(0.56f, 0.88f),
			new FPoint(1f, 1f),
			new FPoint(0.8f, 0.8f),
			new FPoint(0.65f, 0.65f),
			new FPoint(1f, 0.59f),
			new FPoint(1f, 1f),
			new FPoint(0.59f, 1f),
			new FPoint(1f, 1f),
			new FPoint(1f, 1f),
			new FPoint(1f, 1f),
			new FPoint(1f, 1f),
			new FPoint(1f, 1f),
			new FPoint(1f, 1f)
		};
		for (int l = 0; l < array4.Length; l++)
		{
			TikiComponent tikiComponent = new TikiComponent();
			mTikis.Add(tikiComponent);
			if (array6[l].mX != int.MaxValue)
			{
				float num3 = Common._M(2f) * array7[l].mX;
				tikiComponent.mSmoke = GameApp.gApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_SMOKEYBREATH").Duplicate();
				float num4 = GameApp.DownScaleNum(1f);
				tikiComponent.mSmoke.mDrawTransform.Scale(num4, num4);
				tikiComponent.mSmoke.mDrawTransform.Scale(num3, num3);
				int num5 = Common._DS(array6[l].mX - 160);
				int num6 = Common._DS(array6[l].mY);
				tikiComponent.mSmoke.mDrawTransform.Translate(num5, num6);
			}
			tikiComponent.mBreathTimeline.mHoldLastFrame = true;
			tikiComponent.mBreathTimeline.mImage = Res.GetImageByID((ResID)(483 + (array5[l] - 1)));
			tikiComponent.mBreathTimeline.AddPosX(new Component(Common._DS(array3[l].mX - 160)));
			tikiComponent.mBreathTimeline.AddPosY(new Component(Common._DS(array3[l].mY)));
			tikiComponent.mBreathTimeline.AddOpacity(new Component(array4[l]));
			float num7 = Common._M(1.1f);
			tikiComponent.mBreathTimeline.mOverallXScale = array7[l].mX * num7;
			tikiComponent.mBreathTimeline.mOverallYScale = array7[l].mY * num7;
			int num8 = Common._M(16);
			int num9 = 36 * num8;
			tikiComponent.mBreathTimeline.AddScaleX(new Component(0.8f, 0.81f, 0, 11 * num8));
			tikiComponent.mBreathTimeline.AddScaleX(new Component(0.81f, 0.83f, 11 * num8, 16 * num8));
			tikiComponent.mBreathTimeline.AddScaleX(new Component(0.83f, 0.84f, 16 * num8, 23 * num8));
			tikiComponent.mBreathTimeline.AddScaleX(new Component(0.84f, 0.8f, 23 * num8, num9));
			tikiComponent.mBreathTimeline.AddScaleY(new Component(0.8f, 0.83f, 0, 11 * num8));
			tikiComponent.mBreathTimeline.AddScaleY(new Component(0.83f, 0.84f, 11 * num8, 16 * num8));
			tikiComponent.mBreathTimeline.AddScaleY(new Component(0.84f, 0.85f, 16 * num8, 23 * num8));
			tikiComponent.mBreathTimeline.AddScaleY(new Component(0.85f, 0.8f, 23 * num8, num9));
			tikiComponent.mBreathTimeline.mEndFrame = num9;
		}
	}

	public void Update()
	{
		if (mDone)
		{
			return;
		}
		mUpdateCount++;
		if (mScreenShakeTimer > 0)
		{
			if (--mScreenShakeTimer == 0)
			{
				mTopWoodShakeY = 0f;
			}
			else
			{
				mTopWoodShakeY = SexyFramework.Common.Rand(Common._M(8));
			}
		}
		if (mState > 1 && mBGAlpha < 255f)
		{
			mBGAlpha += Common._M(4f);
			if (mBGAlpha > 255f)
			{
				mBGAlpha = 255f;
			}
		}
		if (mState == 0)
		{
			mFrogEffect.Update();
			if (mFrogEffect.mTimer >= mFrogEffect.mFrogJumpTime)
			{
				mState++;
			}
		}
		else if (mState == 1)
		{
			int num = GameApp.gApp.mHeight - Res.GetImageByID(ResID.IMAGE_BOSS_CREDITS_WOOD_BOTTOM).mHeight;
			if (mBottomWoodBounceDir == 0)
			{
				mBottomWoodY -= Common._M(15f);
				mTopWoodY += Common._M(30f);
				if (mBottomWoodY <= (float)num)
				{
					mBottomWoodY = num;
				}
				if (mTopWoodY >= (float)Common._M(0))
				{
					mTopWoodY = Common._M(0);
					mBottomWoodBounceDir = 1;
				}
			}
			else
			{
				mBottomWoodBounceCount++;
				if (mBottomWoodBounceDir == 1 && mBottomWoodBounceCount == MAX_BOTTOM_WOOD_COUNT)
				{
					mBottomWoodBounceDir = -1;
				}
				else if (mBottomWoodBounceDir == -1 && mBottomWoodBounceCount == MAX_BOTTOM_WOOD_COUNT * 2)
				{
					GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_FAKE_CREDITS_MOUTH_CLOSE));
					mState++;
					mFrogEffect.mFrog.SetAngle((float)Math.PI * Common._M(1f));
					mFrogEffect.JumpIn(mFrogEffect.mFrog, Common._SS(GameApp.gApp.mWidth) / 2 - GameApp.gApp.mBoardOffsetX, Common._M(532));
				}
			}
		}
		else if (mState == 2 && ++mTimer >= Common._M(50))
		{
			mFrogEffect.Update();
			if (mFrogEffect.mTimer >= mFrogEffect.mFrogJumpTime)
			{
				mState++;
			}
		}
		else if (mState == 3)
		{
			mTopWoodY -= Common._M(10f);
			if (mTopWoodY <= (float)Common._S(Common._M(-210)))
			{
				mTopWoodY = Common._S(Common._M(-210));
				mState++;
				mTimer = 0;
				GameApp.gApp.mSoundPlayer.Loop(Res.GetSoundByID(ResID.SOUND_NEW_FAKE_CREDITS_MUSIC));
			}
		}
		else if (mState == 4 && ++mTimer >= Common._M(20))
		{
			TransitionHeaders();
		}
		else if (mState == 5)
		{
			for (int i = 0; i < MAX_CREDITS; i++)
			{
				mCredits[i].mY -= Common._M(0.6f);
			}
			if (mCredits[0].mY <= (float)Common._M(-100))
			{
				mTimer = 0;
				mState++;
			}
		}
		else if (mState == 6)
		{
			mBossY += Common._M(10f);
			if (mBossY >= (float)Common._M(7))
			{
				GameApp.gApp.mSoundPlayer.Stop(Res.GetSoundByID(ResID.SOUND_NEW_FAKE_CREDITS_MUSIC));
				GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_FAKE_CREDITS_IDOL));
				mBossY = Common._M(7);
				mScreenShakeTimer = Common._M(100);
				mState++;
				mCredits[0].mFalling = true;
				mCredits[0].mAngleInc = SexyFramework.Common.DegreesToRadians(SexyFramework.Common.FloatRange(Common._M(-0.2f), Common._M1(0.2f)));
				for (int j = 0; j < Common._M(30); j++)
				{
					RockBits rockBits = new RockBits();
					mRockBits.Add(rockBits);
					rockBits.mImage = Res.GetImageByID((ResID)(1078 + SexyFramework.Common.Rand(11)));
					rockBits.mX = Common._M(431);
					rockBits.mY = Common._M(166);
					float num2 = SexyFramework.Common.FloatRange(Common._M(6f), Common._M1(8f));
					float num3 = SexyFramework.Common.FloatRange(0f, (float)Math.PI * 2f);
					rockBits.mVX = num2 * (float)Math.Cos(num3);
					rockBits.mVY = num2 * (0f - (float)Math.Sin(num3));
					rockBits.mAlpha = 255f;
					rockBits.mGravity = SexyFramework.Common.FloatRange(Common._M(0.2f), Common._M1(0.4f));
				}
			}
		}
		else if (mState == 7)
		{
			float num4 = Common._M(4f);
			for (int k = 0; k < MAX_CREDITS && mCredits[k].mFalling; k++)
			{
				mCredits[k].mY += num4;
				mCredits[k].mAngle += mCredits[k].mAngleInc;
				if (k < MAX_CREDITS - 1 && !mCredits[k + 1].mFalling && mCredits[k].mY + (float)mCredits[k].mImage.mHeight >= mCredits[k + 1].mY)
				{
					mCredits[k + 1].mFalling = true;
					mCredits[k + 1].mAngleInc = SexyFramework.Common.DegreesToRadians(SexyFramework.Common.FloatRange(Common._M(-0.2f), Common._M1(0.2f)));
				}
			}
		}
		else if (mState == 8)
		{
			mFadeOutAlpha += Common._M(2.5f);
			if (mFadeOutAlpha >= 255f)
			{
				mFadeOutAlpha = 255f;
				mState++;
				mTimer = 0;
				SoundAttribs soundAttribs = new SoundAttribs();
				soundAttribs.fadeout = 0.01f;
				GameApp.gApp.mSoundPlayer.Loop(Res.GetSoundByID(ResID.SOUND_NEW_FAKE_CREDITS_IDOL_TALKING), soundAttribs);
			}
		}
		else if (mState == 9)
		{
			for (int l = 0; l < mTauntText.Count; l++)
			{
				SimpleFadeText simpleFadeText = mTauntText[l];
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
						mCanDoNextLevel = true;
						mState = 10;
						mTimer = 0;
					}
				}
			}
			if (mTauntText.Last().mFadeIn && mTauntText.Last().mAlpha >= 255f && ++mTimer >= Common._M(300))
			{
				for (int m = 0; m < mTauntText.Count; m++)
				{
					mTauntText[m].mFadeIn = false;
				}
			}
		}
		else if (mState == 10)
		{
			mFadeOutAlpha -= Common._M(2.5f);
			if (mFadeOutAlpha <= 0f)
			{
				mFadeOutAlpha = 0f;
				mDone = true;
				GameApp.gApp.GetBoard().mContinueNextLevelOnLoadProfile = false;
				GameApp.gApp.mSoundPlayer.Fade(Res.GetSoundByID(ResID.SOUND_NEW_FAKE_CREDITS_IDOL_TALKING));
			}
		}
		for (int n = 0; n < mRockBits.Count; n++)
		{
			RockBits rockBits2 = mRockBits[n];
			rockBits2.mVY += rockBits2.mGravity;
			rockBits2.mX += rockBits2.mVX;
			rockBits2.mY += rockBits2.mVY;
			rockBits2.mAlpha -= Common._M(2f);
			if (rockBits2.mAlpha <= 0f)
			{
				mRockBits.RemoveAt(n);
				n--;
			}
		}
		if (mState >= 7)
		{
			for (int num5 = 0; num5 < MAX_HEADER_LETTERS; num5++)
			{
				mHeaderLetters[num5].mAngle += mHeaderLetters[num5].mAngleInc;
				mHeaderLetters[num5].mUpdateCount++;
				int num6 = Common._M(20);
				if (num5 == MAX_HEADER_LETTERS - 1 && mHeaderLetters[num5].mAngleInc == 0f)
				{
					mHeaderLetters[num5].mY += Common._M(10f);
				}
				else if (num5 == MAX_HEADER_LETTERS - 1 && mHeaderLetters[num5].mUpdateCount == num6)
				{
					mHeaderLetters[num5].mSwingCount++;
					mHeaderLetters[num5].mUpdateCount = 0;
					mHeaderLetters[num5].mAngleInc = ((mHeaderLetters[num5].mAngleInc < 0f) ? ((0f - mHeaderLetters[num5].mAngleInc) / Common._M(2f)) : (0f - mHeaderLetters[num5].mAngleInc));
					if (SexyFramework.Common._eq(mHeaderLetters[num5].mAngleInc, 0f, Common._M(0.001f)))
					{
						mHeaderLetters[num5].mAngleInc = 0f;
					}
				}
				else if (num5 < MAX_HEADER_LETTERS - 1)
				{
					mHeaderLetters[num5].mX += mHeaderLetters[num5].mVX;
					mHeaderLetters[num5].mY += mHeaderLetters[num5].mVY;
				}
			}
		}
		if (mHeaderLetters[MAX_HEADER_LETTERS - 1].mY > (float)GameApp.gApp.mHeight && mState < 8)
		{
			mState = 8;
			mFadeOutAlpha = 0f;
		}
		if (mState < 9)
		{
			return;
		}
		List<int> list = new List<int>();
		for (int num7 = 0; num7 < mTikis.Count; num7++)
		{
			TikiComponent tikiComponent = mTikis[num7];
			if (tikiComponent.mCanUpdate)
			{
				if (tikiComponent.mBreathTimeline.Done() || tikiComponent.mBreathTimeline.GetUpdateCount() >= Common._M(138))
				{
					tikiComponent.mSmoke.Update();
				}
				tikiComponent.mBreathTimeline.Update();
				if (tikiComponent.mSmoke.mFrameNum >= (float)tikiComponent.mSmoke.mLastFrameNum && tikiComponent.mBreathTimeline.Done())
				{
					tikiComponent.mCanUpdate = false;
				}
			}
			else if (tikiComponent.mSmoke != null)
			{
				list.Add(num7);
			}
		}
		if (list.Count > 0 && SexyFramework.Common.Rand(Common._M(10)) == 0)
		{
			TikiComponent tikiComponent2 = mTikis[list[SexyFramework.Common.Rand(list.Count)]];
			tikiComponent2.mCanUpdate = true;
			tikiComponent2.mSmoke.ResetAnim();
			tikiComponent2.mBreathTimeline.Reset();
		}
	}

	public void TransitionHeaders()
	{
		int num = 8;
		int num2 = 108;
		float num3 = 5f;
		float num4 = 7.5f;
		if (mCreditsGameY < (float)num)
		{
			mCreditsGameY += num3;
			if (mCreditsGameY >= (float)num)
			{
				int aDelay = (int)((float)(num2 - Res.GetImageByID(ResID.IMAGE_BOSS_CREDITS_TEXT_YOU).mHeight) / num4);
				GameApp.gApp.mSoundPlayer.PlayChained(Res.GetSoundByID(ResID.SOUND_NEW_FAKE_CREDITS_GAMEOVER), Res.GetSoundByID(ResID.SOUND_NEW_FAKE_CREDITS_YOUWIN), aDelay);
				mCreditsGameY = num;
				mScreenShakeTimer = Common._M(20);
			}
		}
		else if (mCreditsWinY < (float)num2)
		{
			mCreditsWinY += num4;
			if (mCreditsWinY >= (float)num2)
			{
				mCreditsWinY = num2;
				mScreenShakeTimer = Common._M(20);
				mState++;
			}
		}
	}

	public void Draw(Graphics g)
	{
		if (mDone)
		{
			return;
		}
		if (!mNextLevelLoaded)
		{
			if (mState < 9)
			{
				if (mState > 1 || mBottomWoodBounceDir != 0)
				{
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(255, 255, 255, (int)mBGAlpha);
					g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_CREDITS_BACKGROUND), Common._S(-80), 0);
					for (int i = 0; i < MAX_CREDITS; i++)
					{
						if (mCredits[i].mY > 200f && mCredits[i].mY < 460f)
						{
							g.DrawImageRotated(mCredits[i].mImage, (int)Common._S(mCredits[i].mX), (int)Common._S(mCredits[i].mY), mCredits[i].mAngle);
						}
					}
					g.SetColorizeImages(colorizeImages: false);
				}
				int num = ((mBottomWoodBounceCount > MAX_BOTTOM_WOOD_COUNT) ? (MAX_BOTTOM_WOOD_COUNT * 2 - mBottomWoodBounceCount) : mBottomWoodBounceCount);
				g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_CREDITS_WOOD_TOP), Common._S(-80), (int)(mTopWoodY + mTopWoodShakeY));
				g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_CREDITS_WOOD_BOTTOM), Common._S(-80), (int)(mBottomWoodY + (float)num * Common._M(5f)));
			}
			if (mState >= 4 && mState < 7)
			{
				g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_CREDITS_TEXT_GAME), Common._S(Common._M(170)), (int)Common._S(mCreditsGameY));
				g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_CREDITS_TEXT_YOU), Common._S(Common._M(231)), (int)Common._S(mCreditsWinY));
			}
			else if (mState >= 7)
			{
				for (int j = 0; j < MAX_HEADER_LETTERS; j++)
				{
					if (j < MAX_HEADER_LETTERS - 1)
					{
						g.DrawImageRotated(mHeaderLetters[j].mImage, (int)Common._S(mHeaderLetters[j].mX), (int)Common._S(mHeaderLetters[j].mY), mHeaderLetters[j].mAngle);
						continue;
					}
					Transform transform = new Transform();
					transform.Translate(mHeaderLetters[j].mImage.mWidth / 2, -mHeaderLetters[j].mImage.mHeight / 2);
					transform.RotateRad(mHeaderLetters[j].mAngle);
					transform.Translate(-mHeaderLetters[j].mImage.mWidth / 2, mHeaderLetters[j].mImage.mHeight / 2);
					g.DrawImageTransform(mHeaderLetters[j].mImage, transform, Common._S(mHeaderLetters[j].mX) + (float)(mHeaderLetters[j].mImage.mWidth / 2), Common._S(mHeaderLetters[j].mY) + (float)(mHeaderLetters[j].mImage.mHeight / 2));
				}
			}
			for (int k = 0; k < mRockBits.Count; k++)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)mRockBits[k].mAlpha);
				g.DrawImage(mRockBits[k].mImage, (int)Common._S(mRockBits[k].mX), (int)Common._S(mRockBits[k].mY));
				g.SetColorizeImages(colorizeImages: false);
			}
		}
		if (mFadeOutAlpha != 0f)
		{
			g.SetColor(0, 0, 0, (int)mFadeOutAlpha);
			g.FillRect(Common._S(-80), 0, GameApp.gApp.mWidth + Common._S(160), GameApp.gApp.mHeight);
		}
		if (mState >= 8)
		{
			for (int l = 0; l < mTikis.Count; l++)
			{
				mTikis[l].mBreathTimeline.mOverallAlphaPct = mFadeOutAlpha / 255f;
				mTikis[l].mBreathTimeline.Draw(g);
			}
			if (mState >= 9)
			{
				if (g.Is3D())
				{
					for (int m = 0; m < mTikis.Count; m++)
					{
						TikiComponent tikiComponent = mTikis[m];
						if (tikiComponent.mCanUpdate)
						{
							g.PushState();
							tikiComponent.mSmoke.Draw(g);
							g.PopState();
						}
					}
				}
				Font fontByID = Res.GetFontByID(ResID.FONT_BOSS_TAUNT);
				for (int n = 0; n < mTauntText.Count; n++)
				{
					if (mTauntText[n].mAlpha > 0f)
					{
						g.SetFont(fontByID);
						g.SetColor(255, 255, 255, (int)mTauntText[n].mAlpha);
						g.DrawString(mTauntText[n].mString, (GameApp.gApp.mWidth - fontByID.StringWidth(mTauntText[n].mString)) / 2 - GameApp.gApp.mBoardOffsetX, Common._S(Common._M(300)) + n * fontByID.mHeight);
					}
				}
			}
		}
		if (!mNextLevelLoaded)
		{
			Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_CREDITS_STONE_BOSS);
			g.DrawImage(imageByID, mBossX, (int)Common._S(mBossY), imageByID.GetCelRect(0));
			int num2 = Common._M(-1);
			int num3 = Common._M(0);
			g.DrawImageCel(Res.GetImageByID(ResID.IMAGE_BOSS_CREDITS_STONE_BOSS_EYES), (int)((float)mBossX + Common._DSA(Common._M(50), num2)), (int)(Common._S(mBossY) + Common._DSA(Common._M1(58), num3)), 0);
		}
		else
		{
			GameApp.gApp.GetBoard().mLevel.mBoss.Draw(g);
		}
		if (mState == 0 || mState >= 2)
		{
			mFrogEffect.Draw(g);
		}
	}

	public bool Done()
	{
		return mDone;
	}

	public bool CanStartNextLevel()
	{
		bool flag = mCanDoNextLevel;
		if (flag)
		{
			mNextLevelLoaded = true;
		}
		mCanDoNextLevel = false;
		return flag;
	}

	public bool IsFullyOpaque()
	{
		if (mState > 2 || mBGAlpha >= 255f)
		{
			return mState < 10;
		}
		return false;
	}

	public bool HasClosedScene()
	{
		return mState >= 2;
	}
}
