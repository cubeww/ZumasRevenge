using System;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class Torch : IDisposable
{
	public PIEffect mFlame;

	public PIEffect mFlameOut;

	public int mX;

	public int mY;

	public int mWidth;

	public int mHeight;

	public int mOverlayAlpha;

	public bool mActive;

	public bool mDraw = true;

	public bool mWasHit;

	public Torch()
	{
	}

	public Torch(Torch rhs)
	{
		mOverlayAlpha = rhs.mOverlayAlpha;
		mWasHit = rhs.mWasHit;
		mDraw = rhs.mDraw;
		mX = rhs.mX;
		mY = rhs.mY;
		mWidth = rhs.mWidth;
		mHeight = rhs.mHeight;
		mActive = rhs.mActive;
	}

	public virtual void Dispose()
	{
		GameApp.gApp.ReleaseTorchEffect(mFlame);
		GameApp.gApp.ReleaseTorchEffect(mFlameOut);
	}

	public void Update()
	{
		if (mFlame == null)
		{
			mFlame = GameApp.gApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_TORCHFLAME").Duplicate();
			mFlame.mEmitAfterTimeline = true;
			Common.SetFXNumScale(mFlame, GameApp.gApp.Is3DAccelerated() ? 1f : Common._M(0.5f));
		}
		if (mFlameOut == null)
		{
			mFlameOut = GameApp.gApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_TORCHFLAMEOUT").Duplicate();
			Common.SetFXNumScale(mFlameOut, GameApp.gApp.Is3DAccelerated() ? 1f : Common._M(0.5f));
		}
		if (!mDraw)
		{
			return;
		}
		if (mActive)
		{
			mFlame.mDrawTransform.LoadIdentity();
			float num = GameApp.DownScaleNum(1f);
			mFlame.mDrawTransform.Scale(num, num);
			if (mX > Common._DS(600))
			{
				mFlame.mDrawTransform.RotateDeg(Common._M(-75));
			}
			mFlame.mDrawTransform.Translate(Common._S(mX) + Common._DS(Common._M(50)), Common._S(mY) + Common._DS(Common._M1(130)));
			mFlame.Update();
		}
		else if (mFlameOut.mFrameNum <= (float)mFlameOut.mLastFrameNum)
		{
			mFlameOut.mDrawTransform.LoadIdentity();
			float num2 = GameApp.DownScaleNum(1f);
			mFlameOut.mDrawTransform.Scale(num2, num2);
			mFlameOut.mDrawTransform.Translate(Common._S(mX) + Common._DS(Common._M(400)), Common._S(mY) + Common._DS(Common._M1(320)));
			mFlameOut.Update();
		}
	}

	public void Draw(Graphics g)
	{
		if (mDraw && mActive && mFlame != null)
		{
			g.PushState();
			mFlame.Draw(g);
			g.PopState();
		}
	}

	public void DrawAbove(Graphics g)
	{
		if (mDraw && mFlameOut != null && !mActive && mFlameOut.mFrameNum <= (float)mFlameOut.mLastFrameNum)
		{
			g.PushState();
			mFlameOut.Draw(g);
			g.PopState();
		}
	}

	public bool CheckCollision(Rect r)
	{
		if (mActive && r.Intersects(new Rect(mX, mY, mWidth, mHeight)))
		{
			GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_TORCH_EXTINGUISHED));
			mActive = false;
			mWasHit = true;
			mFlame.mEmitAfterTimeline = false;
			mFlameOut.ResetAnim();
			return true;
		}
		return false;
	}

	public void SyncState(DataSync sync)
	{
		sync.SyncBoolean(ref mActive);
		sync.SyncLong(ref mX);
		sync.SyncLong(ref mY);
		sync.SyncLong(ref mWidth);
		sync.SyncLong(ref mHeight);
		sync.SyncBoolean(ref mWasHit);
		sync.SyncBoolean(ref mDraw);
		sync.SyncLong(ref mOverlayAlpha);
		if (sync.isRead() && mWasHit)
		{
			mDraw = (mActive = false);
		}
	}
}
