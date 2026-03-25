using System;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public class LavaShader : Effect
{
	public bool mActivateOnMuMu;

	public float mDistAmt;

	public float mOrgDistAmt;

	public float mScroll;

	public float mScale;

	public bool mAffectSkull;

	public bool mApplyFullScene;

	public bool mFadeoutDistortion;

	public bool mDisabled;

	public bool mFadeInFromDeath;

	public bool mNeedFadein;

	public bool mApplyTunnels;

	protected DeviceImage mBuffer;

	protected GameApp mApp;

	protected float mTimer;

	protected override void Init()
	{
		mDisabled = false;
		mFadeInFromDeath = (mFadeoutDistortion = false);
		if (mBuffer == null)
		{
			mBuffer = new DeviceImage();
			mBuffer.mApp = mApp;
			mBuffer.AddImageFlags(24u);
			mBuffer.SetImageMode(hasTrans: false, hasAlpha: false);
		}
	}

	protected void DoShader(Graphics g, DeviceImage buffer)
	{
	}

	public LavaShader()
	{
		mResGroup = "GamePlay";
		mApp = GameApp.gApp;
		mDisabled = false;
		mOrgDistAmt = (mDistAmt = (mScale = (mScroll = 0f)));
		mNeedFadein = false;
		mFadeInFromDeath = false;
		mAffectSkull = true;
		mFadeoutDistortion = false;
		mApplyTunnels = true;
		mApplyFullScene = false;
	}

	public override void Dispose()
	{
		base.Dispose();
		if (mBuffer != null)
		{
			mBuffer.Dispose();
			mBuffer = null;
		}
	}

	public override void LevelStarted(bool from_load)
	{
		if (mApplyFullScene && mOrgDistAmt > 0f)
		{
			mFadeoutDistortion = true;
		}
		else
		{
			mFadeoutDistortion = false;
		}
	}

	public override void Update()
	{
		Update(only_check_shaders_supported: false);
	}

	public void Update(bool only_check_shaders_supported)
	{
		Board board = mApp.GetBoard();
		if (mActivateOnMuMu && !board.mDoMuMuMode)
		{
			return;
		}
		if (mNeedFadein)
		{
			if (mApp.mBoard == null || mApp.mBoard.mTransitionScreenImage == null)
			{
				mDistAmt = Math.Min(mOrgDistAmt, mDistAmt + Common._M(5E-06f));
			}
			mDisabled = (double)mDistAmt <= 1E-09;
			mNeedFadein = mDistAmt < mOrgDistAmt;
		}
		else if (mFadeoutDistortion && mDistAmt > 0f && !board.mDoMuMuMode)
		{
			mDistAmt -= Common._M(1E-06f);
			if (mDistAmt <= 0f)
			{
				mDistAmt = 0f;
				mDisabled = true;
			}
		}
		else if (board.mDoMuMuMode)
		{
			mDistAmt = (mOrgDistAmt = Common._M(0.0005f));
			mScroll = Common._M(0.08f);
			mScale = Common._M(0.2f);
			mDisabled = false;
		}
		if (only_check_shaders_supported)
		{
			if (!mApp.ShadersSupported())
			{
				return;
			}
		}
		else
		{
			bool flag = !mDisabled && mApp.ShadersSupported() && board != null && !board.DoingMainDarkFrogSequence();
			if (mApp.mLoadingThreadStarted && !mApp.mLoadingThreadCompleted)
			{
				flag = false;
			}
			if (!flag)
			{
				return;
			}
		}
		mUpdateCount++;
		mTimer += Common._M(0.02f);
	}

	public override void DrawUnderBackground(Graphics g)
	{
		Board board = mApp.GetBoard();
		bool flag = !mDisabled && mApp.ShadersSupported() && board != null && !board.DoingMainDarkFrogSequence() && (!mActivateOnMuMu || board.mDoMuMuMode);
		if (mApp.mLoadingThreadStarted && !mApp.mLoadingThreadCompleted)
		{
			flag = false;
		}
		int num = 1024;
		int theStretchedHeight = Common._DS(1200);
		g.DrawImage(mApp.mBoard.mBackgroundImage, (Common._S(800) - num) / 2 + GameApp.gScreenShakeX, GameApp.gScreenShakeY, num, theStretchedHeight);
	}

	public override bool DrawTunnel(Graphics g, Image img, int x, int y)
	{
		Board board = mApp.GetBoard();
		if (!mDisabled && mApp.ShadersSupported() && board != null && !board.DoingMainDarkFrogSequence() && mActivateOnMuMu)
		{
			_ = board.mDoMuMuMode;
		}
		return true;
	}

	public bool DrawTunnel(Graphics g, Image img, int x, int y, float dist_amt, float scale, float scroll, float timer, float alpha_mult)
	{
		return false;
	}

	public override void DrawFullScene(Graphics g)
	{
	}

	public override void SetParams(string key, string value)
	{
	}

	public override void NukeParams()
	{
		mActivateOnMuMu = false;
		mApplyTunnels = true;
	}

	public override bool DrawSkullPit(Graphics g, HoleMgr hole)
	{
		Board board = mApp.GetBoard();
		if (!mDisabled && mApp.ShadersSupported())
		{
			board?.DoingMainDarkFrogSequence();
		}
		return false;
	}

	public override void UserDied()
	{
		if (mFadeoutDistortion && mDistAmt < mOrgDistAmt)
		{
			mDisabled = false;
			mFadeInFromDeath = true;
		}
	}

	public override string GetName()
	{
		return "LavaShader";
	}

	public override void CopyFrom(Effect e)
	{
	}
}
