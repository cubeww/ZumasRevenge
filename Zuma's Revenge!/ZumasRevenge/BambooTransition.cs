using System;
using System.Collections.Generic;
using System.Linq;
using JeffLib;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class BambooTransition : Widget
{
	private enum BambooTransitionState
	{
		BAMBOO_TRANSITION_INIT,
		BAMBOO_TRANSITION_CLOSING,
		BAMBOO_TRANSITION_CLOSED,
		BAMBOO_TRANSITION_PAUSE,
		BAMBOO_TRANSITION_OPENING,
		BAMBOO_TRANSITION_OPEN,
		NUM_BAMBOO_TRANSITION_STATES
	}

	public delegate void BambooTransitionDelegate();

	private BambooTransitionState mState;

	private List<BambooColumn> mBambooColumns = new List<BambooColumn>();

	private ulong mLoadStartTime;

	private ulong mFadeCount;

	private int mBambooCloseWaitCount;

	public BambooTransitionDelegate mTransitionDelegate;

	private Image IMAGE_BAMBOO_PIECE_A;

	private Image IMAGE_BAMBOO_PIECE_B;

	private Image IMAGE_BAMBOO_PIECE_C;

	private Image IMAGE_BAMBOO_PIECE_D;

	private int mUpdateNum;

	public BambooTransition()
	{
		Reset();
		mZOrder = int.MaxValue;
		mUpdateNum = 0;
	}

	public void Reset()
	{
		IMAGE_BAMBOO_PIECE_A = Res.GetImageByID(ResID.IMAGE_BAMBOO_PIECE_A);
		IMAGE_BAMBOO_PIECE_B = Res.GetImageByID(ResID.IMAGE_BAMBOO_PIECE_B);
		IMAGE_BAMBOO_PIECE_C = Res.GetImageByID(ResID.IMAGE_BAMBOO_PIECE_C);
		IMAGE_BAMBOO_PIECE_D = Res.GetImageByID(ResID.IMAGE_BAMBOO_PIECE_D);
		mState = BambooTransitionState.BAMBOO_TRANSITION_INIT;
		mBambooCloseWaitCount = 0;
		mLoadStartTime = ulong.MaxValue;
		if (mBambooColumns.Count() == 0)
		{
			float num = (float)GameApp.gApp.GetScreenRect().mX - 10f;
			for (float num2 = num; num2 <= (float)GameApp.gApp.GetScreenRect().mWidth; num2 += (float)(IMAGE_BAMBOO_PIECE_A.GetWidth() - Common._DS(19)))
			{
				mBambooColumns.Add(new BambooColumn());
				mBambooColumns.Last().SetColumnX(num2);
			}
		}
		else
		{
			for (int i = 0; i < mBambooColumns.Count(); i++)
			{
				mBambooColumns[i].Reset();
			}
		}
		SetupBambooSmoke();
	}

	public override void Draw(Graphics g)
	{
		if (mState != BambooTransitionState.BAMBOO_TRANSITION_INIT)
		{
			DeferOverlay(10);
		}
	}

	public override void DrawOverlay(Graphics g)
	{
		int alpha = (int)(255f * ((float)mFadeCount / 40f));
		g.SetColor(0, 0, 0, alpha);
		g.FillRect(GameApp.gApp.GetScreenRect().mX, GameApp.gApp.GetScreenRect().mY, GameApp.gApp.GetScreenRect().mWidth, GameApp.gApp.GetScreenRect().mHeight);
		if (mBambooColumns.Count > 0)
		{
			for (int i = 0; i < mBambooColumns.Count(); i++)
			{
				mBambooColumns[i].Draw(g);
			}
			for (int j = 0; j < mBambooColumns.Count(); j++)
			{
				mBambooColumns[j].DrawSmoke(g);
			}
		}
		mUpdateNum = 0;
	}

	public override void Update()
	{
		mUpdateNum++;
		for (int i = 0; i < mBambooColumns.Count(); i++)
		{
			mBambooColumns[i].UpdateSmokeParticle();
		}
		if (mUpdateNum > 2)
		{
			return;
		}
		for (int j = 0; j < mBambooColumns.Count(); j++)
		{
			bool sound = false;
			if (j == mBambooColumns.Count() - 1)
			{
				sound = true;
			}
			mBambooColumns[j].Update(sound);
		}
		switch (mState)
		{
		case BambooTransitionState.BAMBOO_TRANSITION_CLOSING:
		{
			if ((float)mFadeCount < 40f)
			{
				mFadeCount++;
			}
			bool flag2 = true;
			if (mBambooColumns.Count() > 0)
			{
				for (int l = 0; l < mBambooColumns.Count(); l++)
				{
					flag2 &= mBambooColumns[l].IsClosed();
				}
			}
			if (flag2)
			{
				mBambooCloseWaitCount++;
				if (mBambooCloseWaitCount >= 10)
				{
					mState = BambooTransitionState.BAMBOO_TRANSITION_CLOSED;
				}
			}
			break;
		}
		case BambooTransitionState.BAMBOO_TRANSITION_CLOSED:
			if (mTransitionDelegate != null)
			{
				mTransitionDelegate();
			}
			mFadeCount = 40uL;
			mState = BambooTransitionState.BAMBOO_TRANSITION_PAUSE;
			mLoadStartTime = SexyFramework.Common.SexyTime();
			break;
		case BambooTransitionState.BAMBOO_TRANSITION_PAUSE:
		{
			ulong num = SexyFramework.Common.SexyTime() - mLoadStartTime;
			if (num < 100)
			{
				break;
			}
			mState = BambooTransitionState.BAMBOO_TRANSITION_OPENING;
			if (mBambooColumns.Count() > 0)
			{
				for (int m = 0; m < mBambooColumns.Count(); m++)
				{
					mBambooColumns[m].Open();
				}
			}
			break;
		}
		case BambooTransitionState.BAMBOO_TRANSITION_OPENING:
		{
			if (mFadeCount != 0)
			{
				mFadeCount--;
			}
			bool flag = true;
			if (mBambooColumns.Count() > 0)
			{
				for (int k = 0; k < mBambooColumns.Count(); k++)
				{
					flag &= mBambooColumns[k].IsOpened();
				}
			}
			if (flag)
			{
				mState = BambooTransitionState.BAMBOO_TRANSITION_OPEN;
			}
			break;
		}
		case BambooTransitionState.BAMBOO_TRANSITION_OPEN:
			mState = BambooTransitionState.BAMBOO_TRANSITION_INIT;
			GameApp.gApp.BambooTransitionOpened();
			break;
		}
	}

	public void StartTransition()
	{
		if (mState != BambooTransitionState.BAMBOO_TRANSITION_INIT)
		{
			Console.WriteLine("\n >>>>> WARNING: Attempting to start bamboo transition while a transition is occurring\n ");
			return;
		}
		mFadeCount = 0uL;
		mState = BambooTransitionState.BAMBOO_TRANSITION_CLOSING;
		if (mBambooColumns.Count() > 0)
		{
			for (int i = 0; i < mBambooColumns.Count(); i++)
			{
				mBambooColumns[i].Close();
			}
		}
	}

	public bool IsInProgress()
	{
		return mState != BambooTransitionState.BAMBOO_TRANSITION_INIT;
	}

	private void SetupBambooSmoke()
	{
		int num = Common._M(4);
		List<int> list = new List<int>();
		for (int i = 0; i < mBambooColumns.Count(); i++)
		{
			list.Add(i);
		}
		while (num > 0)
		{
			int index = SexyFramework.Common.Rand() % list.Count();
			for (int j = 0; j < Common._M(20); j++)
			{
				BambooColumn bambooColumn = mBambooColumns[list[index]];
				bambooColumn.AddSmokeParticle(SpawnSmokeParticle(bambooColumn.GetColumnX(), bambooColumn.GetCollisionY(), fast: false, slow_fade: false));
			}
			list.RemoveAt(index);
			num--;
		}
	}

	public static LTSmokeParticle SpawnSmokeParticle(float x, float y)
	{
		return SpawnSmokeParticle(x, y, fast: false);
	}

	public static LTSmokeParticle SpawnSmokeParticle(float x, float y, bool fast)
	{
		return SpawnSmokeParticle(x, y, fast, slow_fade: false);
	}

	public static LTSmokeParticle SpawnSmokeParticle(float x, float y, bool fast, bool slow_fade)
	{
		LTSmokeParticle lTSmokeParticle = new LTSmokeParticle();
		lTSmokeParticle.mX = x;
		lTSmokeParticle.mY = y;
		lTSmokeParticle.mFadingIn = true;
		lTSmokeParticle.mSize = MathUtils.FloatRange(Common._M(0.22f), Common._M1(0.45f));
		float num = (fast ? MathUtils.FloatRange(Common._M(1.5f), Common._M1(2.5f)) : MathUtils.FloatRange(Common._M2(0.75f), Common._M3(1.5f)));
		float num2 = MathUtils.FloatRange(0f, (float)Math.PI * 2f);
		lTSmokeParticle.mVX = num * (float)Math.Cos(num2);
		lTSmokeParticle.mVY = (0f - num) * (float)Math.Sin(num2);
		lTSmokeParticle.mAlpha.mColor = new FColor(0f, 0f, 0f, 0f);
		lTSmokeParticle.mAlpha.mFadeRate = MathUtils.IntRange(Common._M(10), Common._M1(20));
		lTSmokeParticle.mAlphaFadeOutTime = (slow_fade ? MathUtils.IntRange(Common._M(50), Common._M1(75)) : MathUtils.IntRange(Common._M2(10), Common._M3(20)));
		if (SexyFramework.Common.Rand() % 100 == 0)
		{
			lTSmokeParticle.mColorFader.mColor = (lTSmokeParticle.mColorFader.mMinColor = new FColor(249f, 255f, 249f));
			lTSmokeParticle.mColorFader.mMaxColor = new FColor(205f, 208f, 148f);
		}
		else
		{
			lTSmokeParticle.mColorFader.mColor = (lTSmokeParticle.mColorFader.mMinColor = new FColor(212f, 217f, 212f));
			lTSmokeParticle.mColorFader.mMaxColor = new FColor(153f, 148f, 99f);
		}
		lTSmokeParticle.mColorFader.FadeOverTime((int)((float)lTSmokeParticle.mAlphaFadeOutTime + 255f / lTSmokeParticle.mAlpha.mFadeRate));
		return lTSmokeParticle;
	}

	public static void DrawSmokeParticle(Graphics g, LTSmokeParticle s)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_PARTICLE_FUZZ);
		g.SetColorizeImages(colorizeImages: true);
		Color color = s.mColorFader.mColor.ToColor();
		color.mAlpha = (int)s.mAlpha.mColor.mAlpha;
		g.SetColor(color);
		g.DrawImage(imageByID, (int)Common._S(s.mX), (int)Common._S(s.mY), (int)((float)imageByID.mWidth * s.mSize), (int)((float)imageByID.mHeight * s.mSize));
		g.SetColorizeImages(colorizeImages: false);
	}

	public static bool UpdateSmokeParticle(LTSmokeParticle s)
	{
		s.mAlpha.Update();
		s.mColorFader.Update();
		s.mX += s.mVX;
		s.mY += s.mVY;
		if (!s.mFadingIn && s.mAlpha.mColor.mAlpha <= 0f)
		{
			return true;
		}
		if (s.mAlpha.mColor.mAlpha == (float)s.mAlpha.mMax && s.mFadingIn)
		{
			s.mFadingIn = false;
			s.mAlpha.mMin = 0;
			s.mAlpha.mFadeRate = -255f / (float)s.mAlphaFadeOutTime;
		}
		return false;
	}
}
