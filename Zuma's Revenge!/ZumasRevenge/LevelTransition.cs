using System;
using System.Collections.Generic;
using System.Linq;
using SexyFramework;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public class LevelTransition : IDisposable
{
	public enum State
	{
		BambooClose,
		Delay,
		BambooOpen
	}

	public List<LTSmokeParticle> mFrogSmoke = new List<LTSmokeParticle>();

	public FrogFlyOff mFrogEffect;

	public Gun mFrog;

	public int mBambooTime;

	public int mDelay;

	public int mState;

	public int mTimer;

	public bool mDone;

	public bool mIntro;

	public float mBGAlpha;

	public List<BambooColumn> mBambooColumns = new List<BambooColumn>();

	public int mIntroDelay;

	public int mNextLevelOverride;

	public bool mDontRecordStats;

	public bool mTransitionToStats;

	public bool mDrawFrogEffect;

	public bool mSilent;

	public bool mDidFirstBounce;

	protected void SetupBambooSmoke()
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
				bambooColumn.AddSmokeParticle(BambooTransition.SpawnSmokeParticle(bambooColumn.GetColumnX(), bambooColumn.GetCollisionY(), fast: false, slow_fade: false));
			}
			list.RemoveAt(index);
			num--;
		}
	}

	public LevelTransition(int next_level_override, bool dont_record_stats)
	{
		if (!GameApp.gApp.mResourceManager.IsGroupLoaded("AdventureStats"))
		{
			GameApp.gApp.mResourceManager.LoadResources("AdventureStats");
		}
		mFrog = GameApp.gApp.GetBoard().GetGun();
		mFrogEffect = new FrogFlyOff();
		Reset(intro: true);
	}

	public LevelTransition(int next_level_override)
		: this(next_level_override, dont_record_stats: false)
	{
	}

	public LevelTransition()
		: this(-1, dont_record_stats: false)
	{
	}

	public void Dispose()
	{
		GameApp.gApp.mResourceManager.DeleteResources("AdventureStats");
		mFrogEffect = null;
	}

	public bool Update()
	{
		mTimer++;
		if (mDone)
		{
			return false;
		}
		if (mBambooColumns.Count() > 0)
		{
			for (int i = 0; i < mBambooColumns.Count(); i++)
			{
				bool sound = false;
				if (i == mBambooColumns.Count() - 1)
				{
					sound = true;
				}
				mBambooColumns[i].Update(sound);
			}
		}
		if (mState == 0)
		{
			if (mIntro)
			{
				mFrogEffect.Update();
				for (int j = 0; j < mFrogSmoke.Count(); j++)
				{
					LTSmokeParticle s = mFrogSmoke[j];
					if (BambooTransition.UpdateSmokeParticle(s))
					{
						mFrogSmoke.RemoveAt(j);
						j--;
					}
				}
			}
			if ((mFrogEffect.mTimer >= mFrogEffect.mFrogJumpTime / 2 && mIntro) || !mIntro)
			{
				if (mFrogEffect.HasCompletedFlyOff() && mBambooColumns.Count() > 0)
				{
					for (int k = 0; k < mBambooColumns.Count(); k++)
					{
						mBambooColumns[k].Close();
					}
				}
				mBGAlpha += 255f / (float)mBambooTime;
				if (mBGAlpha > 255f)
				{
					mBGAlpha = 255f;
				}
				bool flag = true;
				if (mBambooColumns.Count() > 0)
				{
					for (int l = 0; l < mBambooColumns.Count(); l++)
					{
						flag &= mBambooColumns[l].IsClosed();
					}
				}
				if (flag)
				{
					mTimer = 0;
					mState++;
					if (GameApp.gApp.mBoard.mLevel.mNum != 10)
					{
						int mNum = GameApp.gApp.mBoard.mLevel.mNum;
						int num = GameApp.gApp.mBoard.mLevel.mZone - 1;
						int num2 = num;
						int index = mNum + num * 10 + num2;
						string levelId = GameApp.gApp.GetLevelMgr().GetLevelId(index);
						levelId = char.ToUpper(levelId[0]) + levelId.Substring(1);
						string theGroup = "Levels_" + levelId;
						if (!GameApp.gApp.mResourceManager.IsGroupLoaded(theGroup))
						{
							GameApp.gApp.mResourceManager.PrepareLoadResources(theGroup);
						}
					}
				}
			}
		}
		else
		{
			if (mState == 1)
			{
				if (++mDelay == Common._M(20))
				{
					mTimer = 0;
				}
				mFrogEffect.Update();
				if (GameApp.gApp.mBoard.mGameState == GameState.GameState_BossIntro && GameApp.gApp.mBoard.mBossIntroBGAlpha.GetOutVal() == 1.0)
				{
					mDone = true;
				}
				return mDelay == Common._M(19);
			}
			if (mState == 2)
			{
				mFrogEffect.Update();
				if ((!mIntro && mFrogEffect.mTimer >= mFrogEffect.mFrogJumpTime / 2) || mIntro)
				{
					mBGAlpha -= 255f / (float)mBambooTime;
					if (mBGAlpha < 0f)
					{
						mBGAlpha = 0f;
					}
					bool flag2 = true;
					if (mBambooColumns.Count() > 0)
					{
						for (int m = 0; m < mBambooColumns.Count(); m++)
						{
							flag2 &= mBambooColumns[m].IsOpened();
						}
					}
					if (flag2 && (mIntro || mFrogEffect.mTimer >= mFrogEffect.mFrogJumpTime))
					{
						GameApp.gApp.mBoard.CueLevelTransition();
						mDone = true;
						if (!mIntro && mDrawFrogEffect)
						{
							for (int n = 0; n < Common._M(20); n++)
							{
								mFrog.mSmokeParticles.Add(BambooTransition.SpawnSmokeParticle(mFrog.GetCenterX(), mFrog.GetCenterY(), fast: false, slow_fade: true));
							}
						}
					}
				}
			}
		}
		return false;
	}

	public void DrawOverlay(Graphics g)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_LARGE_FROG);
		if (mDone)
		{
			return;
		}
		if (mBGAlpha > 0f)
		{
			g.SetColor(0, 0, 0, (int)mBGAlpha);
			g.FillRect(GameApp.gApp.GetScreenRect());
		}
		if (mBambooColumns.Count() > 0)
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
		if (((mState != 0 || !mIntro) && (mState != 2 || mIntro)) || !mDrawFrogEffect || !(mFrogEffect.mFrogY + (float)(imageByID.mHeight / 2) + (float)Common._M(0) >= 0f))
		{
			return;
		}
		if (mIntro)
		{
			for (int k = 0; k < mFrogSmoke.Count(); k++)
			{
				BambooTransition.DrawSmokeParticle(g, mFrogSmoke[k]);
			}
		}
		mFrogEffect.Draw(g);
	}

	public void Draw(Graphics g)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_LARGE_FROG);
		if (mDone)
		{
			return;
		}
		if (mBGAlpha > 0f)
		{
			g.SetColor(0, 0, 0, (int)mBGAlpha);
			g.FillRect(GameApp.gApp.GetScreenRect());
		}
		if (mBambooColumns.Count() > 0)
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
		if (((mState != 0 || !mIntro) && (mState != 2 || mIntro)) || !mDrawFrogEffect || !(mFrogEffect.mFrogY + (float)(imageByID.mHeight / 2) + (float)Common._M(0) >= 0f))
		{
			return;
		}
		if (mIntro)
		{
			for (int k = 0; k < mFrogSmoke.Count(); k++)
			{
				BambooTransition.DrawSmokeParticle(g, mFrogSmoke[k]);
			}
		}
		mFrogEffect.Draw(g);
	}

	public void Reset(bool intro)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_BAMBOO_PIECE_A);
		mDrawFrogEffect = true;
		mIntro = intro;
		mState = 0;
		mDone = false;
		mDelay = 0;
		mIntroDelay = 0;
		mFrogSmoke.Clear();
		mSilent = false;
		if (mIntro)
		{
			mFrogEffect.JumpOut(mFrog);
			for (int i = 0; i < Common._M(20); i++)
			{
				mFrogSmoke.Add(BambooTransition.SpawnSmokeParticle(mFrogEffect.mFrogX, mFrogEffect.mFrogY, fast: true, slow_fade: false));
			}
		}
		mBGAlpha = 0f;
		if (mBambooColumns.Count() == 0)
		{
			float num = (float)GameApp.gApp.GetScreenRect().mX - 10f;
			for (float num2 = num; num2 <= (float)GameApp.gApp.GetScreenRect().mWidth; num2 += (float)(imageByID.GetWidth() - Common._DS(19)))
			{
				mBambooColumns.Add(new BambooColumn());
				mBambooColumns.Last().SetColumnX(num2);
			}
		}
		else
		{
			for (int j = 0; j < mBambooColumns.Count(); j++)
			{
				mBambooColumns[j].Reset();
			}
		}
		SetupBambooSmoke();
		mTimer = 0;
	}

	public void RehupFrogPosition()
	{
		mFrogEffect.RehupFrogPosition(mFrog.GetCenterX(), mFrog.GetCenterY());
	}

	public void Open()
	{
		mState = 2;
		if (mBambooColumns.Count() > 0)
		{
			for (int i = 0; i < mBambooColumns.Count(); i++)
			{
				mBambooColumns[i].Open();
			}
		}
	}

	public bool IsDone()
	{
		return mDone;
	}

	public int GetState()
	{
		return mState;
	}
}
