using System;
using JeffLib;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class IndexMedal : ButtonWidget
{
	public struct AceSparkle
	{
		public PIEffect mEffect;

		public float mOffsetX;

		public float mOffsetY;
	}

	private static int MAX_NUM_BUTTON_SPARKLES = 2;

	public AceSparkle[] mSparkles = new AceSparkle[MAX_NUM_BUTTON_SPARKLES];

	public bool mIsAced;

	public float mRadius;

	public IndexMedal(bool theIsAced, int theId, ButtonListener theButtonListener)
		: base(theId, theButtonListener)
	{
		mIsAced = theIsAced;
		SexyFramework.Common.SRand(SexyFramework.Common.SexyTime());
		for (int i = 0; i < MAX_NUM_BUTTON_SPARKLES; i++)
		{
			mSparkles[i].mEffect = null;
			mSparkles[i].mOffsetX = -1f;
			mSparkles[i].mOffsetY = -1f;
		}
	}

	public override void Dispose()
	{
		base.Dispose();
		for (int i = 0; i < MAX_NUM_BUTTON_SPARKLES; i++)
		{
			mSparkles[i].mEffect?.Dispose();
			mSparkles[i].mEffect = null;
		}
	}

	public void FindRandomOffsetsInRadius(float theRadius, ref float theOffsetX, ref float theOffsetY)
	{
		float num = (float)SexyFramework.Common.Rand() / (float)QRand.RAND_MAX * (theRadius * 0.9f);
		int num2 = ((SexyFramework.Common.Rand() % 2 != 0) ? 1 : (-1));
		float num3 = (float)SexyFramework.Common.Rand() / (float)QRand.RAND_MAX * ((float)Math.PI * 2f);
		float num4 = num * (float)Math.Cos(num3) * (float)num2;
		float num5 = num * (float)Math.Sin(num3) * (float)num2;
		theOffsetX = theRadius + num4;
		theOffsetY = theRadius + num5;
	}

	public override void Update()
	{
		base.Update();
		if (!mIsAced)
		{
			return;
		}
		for (int i = 0; i < MAX_NUM_BUTTON_SPARKLES; i++)
		{
			PIEffect mEffect = mSparkles[i].mEffect;
			if (mEffect != null)
			{
				mEffect.mDrawTransform.LoadIdentity();
				mEffect.mDrawTransform.Scale(Common._DS(1.4f), Common._DS(1.4f));
				mEffect.mDrawTransform.Translate(mSparkles[i].mOffsetX, mSparkles[i].mOffsetY);
				mEffect.Update();
				if (SexyFramework.Common.Rand(500) == 0 && mEffect.mCurNumParticles == 0 && MathUtils._geq(mEffect.mFrameNum, mEffect.mLastFrameNum))
				{
					mEffect.ResetAnim();
					mEffect.mRandSeeds.Clear();
					mEffect.mRandSeeds.Add(SexyFramework.Common.Rand(1000));
					FindRandomOffsetsInRadius((float)mButtonImage.mWidth / 2f, ref mSparkles[i].mOffsetX, ref mSparkles[i].mOffsetY);
				}
			}
			else if (SexyFramework.Common.Rand(500) == 0)
			{
				mSparkles[i].mEffect = GameApp.gApp.mResourceManager.GetPIEffect("PIEFFECT_MM_SPARKLE").Duplicate();
				mSparkles[i].mEffect.mEmitAfterTimeline = false;
				Common.SetFXNumScale(mSparkles[i].mEffect, 3f);
				FindRandomOffsetsInRadius((float)mButtonImage.mWidth / 2f, ref mSparkles[i].mOffsetX, ref mSparkles[i].mOffsetY);
			}
		}
	}

	public override void Draw(Graphics g)
	{
		base.Draw(g);
		if (mIsAced)
		{
			for (int i = 0; i < MAX_NUM_BUTTON_SPARKLES; i++)
			{
				mSparkles[i].mEffect?.Draw(g);
			}
		}
	}

	public void SetAced()
	{
		mIsAced = true;
		for (int i = 0; i < MAX_NUM_BUTTON_SPARKLES; i++)
		{
			mSparkles[i].mEffect = GameApp.gApp.mResourceManager.GetPIEffect("PIEFFECT_MM_SPARKLE").Duplicate();
			mSparkles[i].mEffect.mEmitAfterTimeline = true;
			Common.SetFXNumScale(mSparkles[i].mEffect, 3f);
			FindRandomOffsetsInRadius((float)mButtonImage.mWidth / 2f, ref mSparkles[i].mOffsetX, ref mSparkles[i].mOffsetY);
		}
	}

	public void Init()
	{
		if (mIsAced)
		{
			for (int i = 0; i < MAX_NUM_BUTTON_SPARKLES; i++)
			{
				FindRandomOffsetsInRadius((float)mButtonImage.mWidth / 2f, ref mSparkles[i].mOffsetX, ref mSparkles[i].mOffsetY);
			}
		}
	}
}
