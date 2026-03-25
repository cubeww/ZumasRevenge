using System;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public abstract class Effect : IDisposable
{
	protected int mUpdateCount;

	protected bool mIs3D;

	protected string mResGroup;

	protected string mLevelId;

	protected virtual void Init()
	{
	}

	public Effect()
	{
		mUpdateCount = 0;
		mIs3D = GameApp.gApp.mGraphicsDriver.Is3D();
	}

	public virtual void Dispose()
	{
	}

	public abstract void Update();

	public abstract string GetName();

	public virtual void DrawUnderBalls(Graphics g)
	{
	}

	public virtual void DrawAboveBalls(Graphics g)
	{
	}

	public virtual void DrawUnderBackground(Graphics g)
	{
	}

	public virtual void LevelStarted(bool from_load)
	{
	}

	public virtual void DrawFullScene(Graphics g)
	{
	}

	public virtual void DrawFullSceneNoFrog(Graphics g)
	{
	}

	public virtual void DrawPriority(Graphics g, int priority)
	{
	}

	public virtual bool DrawTunnel(Graphics g, Image img, int x, int y)
	{
		return true;
	}

	public virtual void Reset(string level_id)
	{
		if (level_id.Length != 0)
		{
			char c = level_id[0];
			if (c >= 'a' && c <= 'z')
			{
				c = (char)(c - 32);
				mLevelId = c + level_id.Substring(1);
			}
			else
			{
				mLevelId = level_id;
			}
			if (mResGroup.Length > 0 && GameApp.gApp.mResourceManager.IsGroupLoaded(mResGroup))
			{
				Init();
			}
		}
	}

	public virtual void LoadResources()
	{
		if (mResGroup.Length != 0 && !GameApp.gApp.mResourceManager.IsGroupLoaded(mResGroup))
		{
			GameApp.gApp.mResourceManager.LoadResources(mResGroup);
			Init();
		}
	}

	public virtual void DeleteResources()
	{
		if (mResGroup.Length != 0 && GameApp.gApp.mResourceManager.IsGroupLoaded(mResGroup))
		{
			GameApp.gApp.mResourceManager.DeleteResources(mResGroup);
		}
	}

	public virtual void BulletFired(Bullet b)
	{
	}

	public virtual bool DrawSkullPit(Graphics g, HoleMgr hole)
	{
		return false;
	}

	public virtual void UserDied()
	{
	}

	public virtual void NukeParams()
	{
	}

	public virtual void SetParams(string key, string value)
	{
	}

	public virtual void BulletHit(Bullet b)
	{
	}

	public virtual void CopyFrom(Effect e)
	{
		Reset(mLevelId);
		mUpdateCount = e.mUpdateCount;
		mIs3D = e.mIs3D;
		mResGroup = e.mResGroup;
		mLevelId = e.mLevelId;
	}
}
