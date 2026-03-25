using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class FrogFlyOff
{
	public float mFrogScale;

	public float mFrogX;

	public float mFrogY;

	public float mFrogAngle;

	public float mFrogAngleDelta;

	public float mFrogVX;

	public float mFrogVY;

	public float mScaleDelta;

	public float mDestFrogAngle;

	public int mFrogJumpTime;

	public int mTimer;

	public bool mJumpOut;

	public bool mPlayThud;

	public Gun mFrog;

	protected Transform mGlobalTranform = new Transform();

	private static float FROG_START_SCALE = 0.26f;

	public FrogFlyOff()
	{
		mPlayThud = false;
		mFrogJumpTime = Common._M(80);
	}

	public virtual void Dispose()
	{
	}

	public void JumpOut(Gun frog, int dest_x, int dest_y, int start_x, int start_y, float angle)
	{
		FROG_START_SCALE = Common._M(0.5f);
		mTimer = 0;
		mJumpOut = true;
		mFrog = frog;
		mFrogX = ((start_x == int.MaxValue) ? mFrog.GetCenterX() : start_x);
		mFrogY = ((start_y == int.MaxValue) ? mFrog.GetCenterY() : start_y);
		Image imageByID = Res.GetImageByID(ResID.IMAGE_LARGE_FROG);
		if (dest_x == int.MaxValue)
		{
			dest_x = GlobalMembers.gSexyApp.mWidth - imageByID.mWidth / 2;
		}
		if (dest_y == int.MaxValue)
		{
			dest_y = -(int)mFrogY - imageByID.mHeight / 2;
		}
		dest_x -= (int)mFrogX;
		mFrogVX = (float)dest_x / (float)mFrogJumpTime;
		mFrogVY = (float)dest_y / (float)mFrogJumpTime;
		mScaleDelta = (Common._M(2f) - FROG_START_SCALE) / (float)mFrogJumpTime;
		mFrogScale = FROG_START_SCALE;
		mFrogAngle = (mDestFrogAngle = (MathUtils._eq(angle, float.MaxValue) ? mFrog.GetAngle() : angle));
		mFrogAngleDelta = Common._M(0.15f);
	}

	public void JumpOut(Gun frog, int dest_x, int dest_y, int start_x, int start_y)
	{
		JumpOut(frog, dest_x, dest_y, start_x, start_y, float.MaxValue);
	}

	public void JumpOut(Gun frog, int dest_x, int dest_y, int start_x)
	{
		JumpOut(frog, dest_x, dest_y, start_x, int.MaxValue);
	}

	public void JumpOut(Gun frog, int dest_x, int dest_y)
	{
		JumpOut(frog, dest_x, dest_y, int.MaxValue);
	}

	public void JumpOut(Gun frog, int dest_x)
	{
		JumpOut(frog, dest_x, int.MaxValue, int.MaxValue);
	}

	public void JumpOut(Gun frog)
	{
		JumpOut(frog, int.MaxValue);
	}

	public void JumpIn(Gun frog, int dest_x, int dest_y, bool continue_from_jump_out, int jump_to_x, int jump_to_y)
	{
		FROG_START_SCALE = Common._M(0.5f);
		if (!continue_from_jump_out)
		{
			JumpOut(frog, jump_to_x, jump_to_y);
			mFrogX += mFrogVX * (float)mFrogJumpTime;
			mFrogY += mFrogVY * (float)mFrogJumpTime;
			mFrogAngle += mFrogAngleDelta * (float)mFrogJumpTime;
		}
		mTimer = 0;
		mFrog = frog;
		mJumpOut = false;
		mPlayThud = true;
		mFrogScale = Common._M(2f);
		mScaleDelta *= -1f;
		RehupFrogPosition(dest_x, dest_y);
	}

	public void JumpIn(Gun frog, int dest_x, int dest_y, bool continue_from_jump_out, int jump_to_x)
	{
		JumpIn(frog, dest_x, dest_y, continue_from_jump_out, jump_to_x, int.MaxValue);
	}

	public void JumpIn(Gun frog, int dest_x, int dest_y, bool continue_from_jump_out)
	{
		JumpIn(frog, dest_x, dest_y, continue_from_jump_out, int.MaxValue);
	}

	public void JumpIn(Gun frog, int dest_x, int dest_y)
	{
		JumpIn(frog, dest_x, dest_y, continue_from_jump_out: true);
	}

	public bool HasCompletedFlyOff()
	{
		return mTimer > mFrogJumpTime;
	}

	public void RehupFrogPosition(int dest_x, int dest_y)
	{
		RehupFrogPosition(dest_x, dest_y, mFrog.GetAngle());
	}

	public void RehupFrogPosition(int dest_x, int dest_y, float forced_dest_angle)
	{
		mFrogAngleDelta = (0f - (mFrogAngle - forced_dest_angle)) / (float)mFrogJumpTime;
		mFrogVX = (0f - (mFrogX - (float)dest_x)) / (float)mFrogJumpTime;
		mFrogVY = (0f - (mFrogY - (float)dest_y)) / (float)mFrogJumpTime;
	}

	public void Update()
	{
		if (mTimer > mFrogJumpTime)
		{
			return;
		}
		mTimer++;
		if (mJumpOut)
		{
			if (mFrogScale < 1f)
			{
				if (mTimer == 1)
				{
					GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_FROG_LAUNCH));
				}
				mFrogScale += mScaleDelta;
				if (mFrogScale > 1f)
				{
					mFrogScale = 1f;
				}
			}
			mFrogAngle += mFrogAngleDelta;
			mFrogX += mFrogVX;
			mFrogY += mFrogVY;
			return;
		}
		mFrogAngle += mFrogAngleDelta;
		mFrogX += mFrogVX;
		mFrogY += mFrogVY;
		if (mTimer >= mFrogJumpTime)
		{
			mFrogAngle = mDestFrogAngle;
		}
		if (mFrogScale > FROG_START_SCALE)
		{
			mFrogScale += mScaleDelta;
			PlayFrogLandingSound();
			if (mFrogScale < FROG_START_SCALE)
			{
				mFrogScale = FROG_START_SCALE;
			}
		}
	}

	public void Draw(Graphics g)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_LARGE_FROG);
		if (mFrogY + (float)(imageByID.mHeight / 2) >= 0f)
		{
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_FROG_SHADOW);
			mGlobalTranform.Reset();
			mGlobalTranform.RotateRad(mFrogAngle);
			float num = (float)mTimer / (float)mFrogJumpTime;
			if (num > 1f)
			{
				num = 1f;
			}
			float num2 = Common._M(1f);
			float num3 = Common._M(3f);
			float num4 = Common._M(1f);
			float num5 = Common._M(0f);
			float num6 = Common._M(0f);
			float num7 = Common._M(150f);
			float num8;
			float num9;
			float num10;
			if (mJumpOut)
			{
				num8 = num2 + (num3 - num2) * num;
				num9 = num4 + (num5 - num4) * num;
				num10 = num6 + (num7 - num6) * num;
			}
			else
			{
				num8 = num3 - (num3 - num2) * num;
				num9 = num5 - (num5 - num4) * num;
				num10 = num7 - (num7 - num6) * num;
			}
			mGlobalTranform.Scale(num8, num8);
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(0, 0, 0, (int)(num9 * 255f));
			g.DrawImageTransform(imageByID2, mGlobalTranform, imageByID2.GetCelRect(0), Common._S(mFrogX - num10), Common._S(mFrogY + num10));
			g.SetColorizeImages(colorizeImages: false);
			mGlobalTranform.Reset();
			mGlobalTranform.RotateRad(mFrogAngle);
			mGlobalTranform.Scale(mFrogScale, mFrogScale);
			g.DrawImageTransform(imageByID, mGlobalTranform, Common._S(mFrogX), Common._S(mFrogY));
		}
	}

	public void SyncState(DataSync sync)
	{
		sync.SyncFloat(ref mFrogScale);
		sync.SyncFloat(ref mFrogX);
		sync.SyncFloat(ref mFrogY);
		sync.SyncFloat(ref mFrogAngle);
		sync.SyncFloat(ref mFrogAngleDelta);
		sync.SyncFloat(ref mFrogVX);
		sync.SyncFloat(ref mFrogVY);
		sync.SyncFloat(ref mScaleDelta);
		sync.SyncFloat(ref mDestFrogAngle);
		sync.SyncLong(ref mFrogJumpTime);
		sync.SyncLong(ref mTimer);
		sync.SyncBoolean(ref mJumpOut);
		if (sync.isRead())
		{
			mFrog = GameApp.gApp.mBoard.mFrog;
		}
	}

	private void PlayFrogLandingSound()
	{
		if (mPlayThud && !(mFrogScale > FROG_START_SCALE - mScaleDelta * 15f))
		{
			GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_FROG_FALL));
			mPlayThud = false;
		}
	}
}
