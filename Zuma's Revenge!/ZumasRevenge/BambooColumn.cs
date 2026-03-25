using System.Collections.Generic;
using System.Linq;
using SexyFramework;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public class BambooColumn
{
	private enum BambooState
	{
		Init,
		Falling,
		Bouncing,
		Closed,
		Opening,
		Open
	}

	private struct BambooEnd
	{
		public float mY;

		public float mFinalY;

		public float mVelocityY;
	}

	public const int BAMBOO_TRANSITION_FADE_TIME = 100;

	public const int BAMBOO_TRANSITION_PAUSE_TIME = 100;

	public const float BAMBOO_TRANSITION_FALL_TIME = 20f;

	public const float BAMBOO_BOUNCE_GRAVITY = 0.1f;

	public const int BAMBOO_CLOSE_UPDATE_WAIT_COUNT = 10;

	public const float BAMBOO_V_DIV = 10f;

	private BambooEnd mTopEnd = default(BambooEnd);

	private BambooEnd mBotEnd = default(BambooEnd);

	private BambooState mState;

	private float mX;

	private float mGravity;

	private List<LTSmokeParticle> mSmoke = new List<LTSmokeParticle>();

	private Image IMAGE_BAMBOO_PIECE_A;

	private Image IMAGE_BAMBOO_PIECE_B;

	private Image IMAGE_BAMBOO_PIECE_C;

	private Image IMAGE_BAMBOO_PIECE_D;

	private bool mDrawed;

	public BambooColumn()
	{
		Reset();
	}

	public void Reset()
	{
		IMAGE_BAMBOO_PIECE_A = Res.GetImageByID(ResID.IMAGE_BAMBOO_PIECE_A);
		IMAGE_BAMBOO_PIECE_B = Res.GetImageByID(ResID.IMAGE_BAMBOO_PIECE_B);
		IMAGE_BAMBOO_PIECE_C = Res.GetImageByID(ResID.IMAGE_BAMBOO_PIECE_C);
		IMAGE_BAMBOO_PIECE_D = Res.GetImageByID(ResID.IMAGE_BAMBOO_PIECE_D);
		mState = BambooState.Init;
		float num = (float)GameApp.gApp.GetScreenRect().mHeight / 2f;
		float num2 = SexyFramework.Common.Rand() % Common._DS(400) - Common._DS(200);
		mTopEnd.mFinalY = num + num2;
		mTopEnd.mY = -IMAGE_BAMBOO_PIECE_C.GetHeight();
		mTopEnd.mVelocityY = (mTopEnd.mFinalY - mTopEnd.mY) / 20f;
		mBotEnd.mFinalY = mTopEnd.mFinalY + (float)IMAGE_BAMBOO_PIECE_C.GetHeight();
		mBotEnd.mY = GameApp.gApp.GetScreenRect().mHeight + IMAGE_BAMBOO_PIECE_D.GetHeight();
		mBotEnd.mVelocityY = (mBotEnd.mFinalY - mBotEnd.mY) / 20f;
		mGravity = 0.1f;
		mSmoke.Clear();
	}

	public void Draw(Graphics g)
	{
		mDrawed = true;
		g.DrawImage(IMAGE_BAMBOO_PIECE_C, (int)(mX + (float)Common._DS(4)), (int)mTopEnd.mY);
		float num = mTopEnd.mY;
		bool flag = false;
		while (num >= 0f)
		{
			Image image = null;
			if (flag)
			{
				image = IMAGE_BAMBOO_PIECE_B;
				num -= (float)image.GetHeight();
			}
			else
			{
				image = IMAGE_BAMBOO_PIECE_A;
				num -= (float)image.GetHeight();
			}
			g.DrawImage(image, (int)mX, (int)num);
			flag = !flag;
		}
		g.DrawImage(IMAGE_BAMBOO_PIECE_D, (int)mX, (int)mBotEnd.mY);
		float num2 = mBotEnd.mY;
		flag = false;
		while (num2 <= (float)GameApp.gApp.GetScreenRect().mHeight)
		{
			Image image2 = null;
			if (flag)
			{
				image2 = IMAGE_BAMBOO_PIECE_B;
				num2 += (float)image2.GetHeight();
			}
			else
			{
				image2 = IMAGE_BAMBOO_PIECE_A;
				num2 += (float)image2.GetHeight();
			}
			g.DrawImage(image2, (int)mX, (int)num2);
			flag = !flag;
		}
	}

	public void DrawSmoke(Graphics g)
	{
		if (mSmoke.Count() > 0)
		{
			for (int i = 0; i < mSmoke.Count(); i++)
			{
				BambooTransition.DrawSmokeParticle(g, mSmoke[i]);
			}
		}
	}

	public void Update(bool sound)
	{
		switch (mState)
		{
		case BambooState.Falling:
			mTopEnd.mY += mTopEnd.mVelocityY;
			mBotEnd.mY += mBotEnd.mVelocityY;
			if (mTopEnd.mY + (float)IMAGE_BAMBOO_PIECE_C.GetHeight() >= mBotEnd.mY)
			{
				mTopEnd.mY = mBotEnd.mY - (float)IMAGE_BAMBOO_PIECE_C.GetHeight() - 1f;
				mState = BambooState.Bouncing;
				if (sound)
				{
					PlayBambooSound(0.2f);
				}
			}
			break;
		case BambooState.Bouncing:
		{
			float num = 0f - (mTopEnd.mVelocityY / Common._M(10f) - mGravity);
			float num2 = 0f - (mBotEnd.mVelocityY / Common._M(10f) + mGravity);
			mTopEnd.mY += num;
			mBotEnd.mY += num2;
			mGravity += 0.1f;
			if (mTopEnd.mY + (float)IMAGE_BAMBOO_PIECE_C.GetHeight() >= mBotEnd.mY)
			{
				mTopEnd.mY = mBotEnd.mY - (float)IMAGE_BAMBOO_PIECE_C.GetHeight() + (float)Common._DS(7);
				mState = BambooState.Closed;
				if (sound)
				{
					PlayBambooSound(0.1f);
				}
			}
			break;
		}
		case BambooState.Opening:
		{
			mTopEnd.mY -= mTopEnd.mVelocityY;
			mBotEnd.mY -= mBotEnd.mVelocityY;
			bool flag = mTopEnd.mY + (float)IMAGE_BAMBOO_PIECE_C.GetHeight() < -20f;
			bool flag2 = mBotEnd.mY >= (float)(GameApp.gApp.GetScreenRect().mHeight + 20);
			if (flag && flag2 && mDrawed)
			{
				mState = BambooState.Open;
			}
			break;
		}
		}
		mDrawed = false;
	}

	public void UpdateSmokeParticle()
	{
		if (mState == BambooState.Init || mState == BambooState.Falling)
		{
			return;
		}
		for (int i = 0; i < mSmoke.Count(); i++)
		{
			LTSmokeParticle s = mSmoke[i];
			if (BambooTransition.UpdateSmokeParticle(s))
			{
				mSmoke.RemoveAt(i);
				i--;
			}
		}
	}

	public void SetColumnX(float theX)
	{
		mX = theX;
	}

	public void Close()
	{
		if (mState == BambooState.Open)
		{
			Reset();
		}
		if (mState == BambooState.Init)
		{
			mState = BambooState.Falling;
		}
	}

	public void Open()
	{
		if (mState == BambooState.Closed)
		{
			mState = BambooState.Opening;
		}
	}

	public bool IsClosed()
	{
		return mState == BambooState.Closed;
	}

	public bool IsOpened()
	{
		return mState == BambooState.Open;
	}

	public float GetColumnX()
	{
		return mX;
	}

	public float GetCollisionY()
	{
		return mTopEnd.mFinalY;
	}

	public void AddSmokeParticle(LTSmokeParticle s)
	{
		mSmoke.Add(s);
	}

	private void PlayBambooSound(float inVolume)
	{
		SoundAttribs soundAttribs = new SoundAttribs();
		soundAttribs.volume = inVolume;
		GameApp.gApp.mSoundPlayer.Play(Res.GetSoundByID(ResID.SOUND_BAMBOO_CLOSE), soundAttribs);
	}
}
