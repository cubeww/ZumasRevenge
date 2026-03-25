using System;
using JeffLib;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public class CannonPowerEffect : PowerEffect
{
	protected class CannonRing
	{
		public float mX;

		public float mY;

		public float mVX;

		public float mVY;

		public float mTX;

		public float mTY;

		public float mSizePct;

		public float mAlpha = 255f;
	}

	protected CannonRing[] mRings = new CannonRing[3]
	{
		new CannonRing(),
		new CannonRing(),
		new CannonRing()
	};

	protected float mBallRotation;

	public CannonPowerEffect(Ball b)
	{
		int radius = b.GetRadius();
		int num = (int)b.GetX() - radius;
		int num2 = (int)b.GetY() - radius;
		mRings[0].mX = num + 18;
		mRings[0].mY = num2 + 11;
		mRings[1].mX = num + 11;
		mRings[1].mY = num2 + 23;
		mRings[2].mX = num + 24;
		mRings[2].mY = num2 + 22;
		mBallRotation = b.GetRotation();
		for (int i = 0; i < 3; i++)
		{
			JeffLib.Common.RotatePoint(mBallRotation - 1.5707645f, ref mRings[i].mX, ref mRings[i].mY, num + radius, num2 + radius);
		}
	}

	public CannonPowerEffect()
	{
	}

	public override void Update()
	{
		if (IsDone())
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			if (mRings[i].mSizePct < 1f)
			{
				mRings[i].mSizePct += 1f / 15f;
				if (mRings[i].mSizePct >= 1f)
				{
					mRings[i].mSizePct = 1f;
					float num2 = MathUtils.DegreesToRadians(mBallRotation + (float)(120 * i));
					mRings[i].mVX = (float)Math.Cos(num2) * 1.2f;
					mRings[i].mVY = (0f - (float)Math.Sin(num2)) * 1.2f;
					mRings[i].mTX = mRings[i].mX + mRings[i].mVX * 15f;
					mRings[i].mTY = mRings[i].mY + mRings[i].mVY * 15f;
				}
			}
			else if (mRings[i].mVX != 0f || mRings[i].mVY != 0f)
			{
				mRings[i].mX += mRings[i].mVX;
				mRings[i].mY += mRings[i].mVY;
				if (JeffLib.Common.DoneMoving(mRings[i].mX, mRings[i].mVX, mRings[i].mTX))
				{
					mRings[i].mX = mRings[i].mTX;
					mRings[i].mVX = 0f;
				}
				if (JeffLib.Common.DoneMoving(mRings[i].mY, mRings[i].mVY, mRings[i].mTY))
				{
					mRings[i].mY = mRings[i].mTY;
					mRings[i].mVY = 0f;
				}
			}
			else if (mRings[i].mAlpha != 0f)
			{
				mRings[i].mSizePct += 1f / (float)Common._M(25);
				mRings[i].mAlpha -= Common._M(6f);
				if (mRings[i].mAlpha < 0f)
				{
					num++;
					mRings[i].mAlpha = 0f;
				}
			}
			else
			{
				num++;
			}
		}
		if (num == 3)
		{
			mDone = true;
		}
	}

	public override void Draw(Graphics g)
	{
		Image imageByID = Res.GetImageByID((ResID)(1394 + mColorType));
		g.SetDrawMode(1);
		for (int i = 0; i < 3; i++)
		{
			int num = (int)(mRings[i].mSizePct * (float)imageByID.mWidth);
			int num2 = (int)(mRings[i].mSizePct * (float)imageByID.mHeight);
			if (mRings[i].mAlpha != 255f)
			{
				g.SetColor(255, 255, 255, (int)mRings[i].mAlpha);
				g.SetColorizeImages(colorizeImages: true);
			}
			g.DrawImage(imageByID, (int)(Common._S(mRings[i].mX) - (float)(num / 2)), (int)(Common._S(mRings[i].mY) - (float)(num2 / 2)), num, num2);
			g.SetColorizeImages(colorizeImages: false);
		}
		g.SetDrawMode(0);
	}

	public override void SyncState(DataSync sync)
	{
		base.SyncState(sync);
		sync.SyncFloat(ref mBallRotation);
		for (int i = 0; i < 3; i++)
		{
			sync.SyncFloat(ref mRings[i].mX);
			sync.SyncFloat(ref mRings[i].mY);
			sync.SyncFloat(ref mRings[i].mVX);
			sync.SyncFloat(ref mRings[i].mVY);
			sync.SyncFloat(ref mRings[i].mTX);
			sync.SyncFloat(ref mRings[i].mTY);
			sync.SyncFloat(ref mRings[i].mSizePct);
			sync.SyncFloat(ref mRings[i].mAlpha);
		}
	}
}
