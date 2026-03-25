using System;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class OrbParticle
{
	protected float mAlpha;

	protected float mAngle;

	protected float mRadius;

	protected float mRotation;

	protected float mSize;

	protected float mAlphaFade;

	protected float mSizeFade;

	protected float mRedFade;

	protected float mGreenFade;

	protected float mRed;

	protected float mGreen;

	protected Transform mGlobalTranform = new Transform();

	public OrbParticle()
	{
	}

	public OrbParticle(float angle, float radius, float alpha_fade, float size_fade)
	{
		mAngle = angle;
		mAlphaFade = alpha_fade;
		mSizeFade = size_fade;
		mAlpha = 255f;
		mRadius = radius;
		mSize = 1f;
		mRotation = 0f;
		mRed = 255f;
		mGreen = 255f;
		float num = 255f / mAlphaFade;
		mRedFade = 255f / num;
		mGreenFade = Common._M(54f) / num;
	}

	public void Update()
	{
		mAlpha -= mAlphaFade;
		mSize -= mSizeFade;
		mRed -= mRedFade;
		mGreen -= mGreenFade;
		if (mRed < 0f)
		{
			mRed = 0f;
		}
		if (mGreen < 0f)
		{
			mGreen = 0f;
		}
		if (mAlpha < 0f)
		{
			mAlpha = 0f;
		}
		if (mSize < 0f)
		{
			mSize = 0f;
		}
		mRotation += Common._M(0.1f);
	}

	public void Draw(Graphics g, float x, float y)
	{
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor((int)mRed, (int)mGreen, 255, (int)mAlpha);
		mGlobalTranform.Reset();
		mGlobalTranform.RotateRad(mRotation);
		mGlobalTranform.Scale(mSize, mSize);
		Image imageByID = Res.GetImageByID(ResID.IMAGE_PART_FAT);
		g.DrawImageTransform(imageByID, mGlobalTranform, x + mRadius * (float)Math.Cos(mAngle), y - mRadius * (float)Math.Sin(mAngle));
		g.SetColorizeImages(colorizeImages: false);
	}

	public bool IsDone()
	{
		return mAlpha <= 0f;
	}

	public void SyncState(DataSync sync)
	{
		sync.SyncFloat(ref mAlpha);
		sync.SyncFloat(ref mAngle);
		sync.SyncFloat(ref mRadius);
		sync.SyncFloat(ref mRotation);
		sync.SyncFloat(ref mSize);
		sync.SyncFloat(ref mAlphaFade);
		sync.SyncFloat(ref mSizeFade);
		sync.SyncFloat(ref mRedFade);
		sync.SyncFloat(ref mGreenFade);
		sync.SyncFloat(ref mRed);
		sync.SyncFloat(ref mGreen);
	}
}
