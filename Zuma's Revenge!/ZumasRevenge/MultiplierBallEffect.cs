using System;
using System.Collections.Generic;
using JeffLib;
using Microsoft.Xna.Framework;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.PIL;

namespace ZumasRevenge;

public class MultiplierBallEffect
{
	public static SpawnColors[] gSpawnColors = new SpawnColors[6]
	{
		new SpawnColors(new SexyFramework.Graphics.Color(1, 108, 222), new SexyFramework.Graphics.Color(12, 0, 255), new SexyFramework.Graphics.Color(0, 195, 255)),
		new SpawnColors(new SexyFramework.Graphics.Color(246, 236, 4), new SexyFramework.Graphics.Color(206, 151, 15), new SexyFramework.Graphics.Color(254, 255, 0)),
		new SpawnColors(new SexyFramework.Graphics.Color(222, 0, 0), new SexyFramework.Graphics.Color(250, 14, 124), new SexyFramework.Graphics.Color(255, 131, 0)),
		new SpawnColors(new SexyFramework.Graphics.Color(0, 236, 51), new SexyFramework.Graphics.Color(25, 125, 115), new SexyFramework.Graphics.Color(135, 235, 15)),
		new SpawnColors(new SexyFramework.Graphics.Color(155, 17, 236), new SexyFramework.Graphics.Color(11, 10, 255), new SexyFramework.Graphics.Color(216, 21, 255)),
		new SpawnColors(new SexyFramework.Graphics.Color(250, 240, 238), new SexyFramework.Graphics.Color(SexyFramework.Graphics.Color.White), new SexyFramework.Graphics.Color(SexyFramework.Graphics.Color.White))
	};

	public static int[] gMaxParticles = new int[7];

	public static int gTrailHandle = 0;

	public static float MULTIPLIER_BALL_EFFECT_PARTICLE_COUNT_SCALE = 0.5f;

	protected Ball mMultBall;

	protected SpawnEffect mSpawnEffect;

	protected TriggeredEffect mTriggeredEffect;

	protected List<AlphaFadeInfo> mBeamAlphas = new List<AlphaFadeInfo>();

	protected float mLastBallX;

	protected float mLastBallY;

	protected int mSpawnTimer;

	protected int mTriggerTimer;

	protected int mState;

	protected bool mDoMultFlash;

	protected Transform mGlobalTranform = new Transform();

	protected void InitSpawnEffects()
	{
		if (mMultBall != null)
		{
			mSpawnEffect = new SpawnEffect();
			SexyFramework.PIL.System mRings = mSpawnEffect.mRings;
			mRings.SetLife(Common._M(100));
			Emitter emitter = new Emitter();
			emitter.mDeleteInvisParticles = true;
			emitter.mCullingRect = new Rect(0, 0, Common._SS(GlobalMembers.gSexyAppBase.mWidth), Common._SS(GlobalMembers.gSexyAppBase.mHeight));
			EmitterScale emitterScale = new EmitterScale();
			emitter.mTintColor = gSpawnColors[mMultBall.GetColorType()].mRings;
			emitterScale.mLifeScale = Common._M(0.125f);
			emitterScale.mNumberScale = Common._M(0.96f);
			emitterScale.mSizeXScale = Common._M(0.5f);
			emitterScale.mVelocityScale = Common._M(6.52f);
			emitter.AddScaleKeyFrame(0, emitterScale);
			emitterScale = new EmitterScale(emitterScale);
			emitterScale.mSizeXScale = Common._M(1.87f);
			emitter.AddScaleKeyFrame(Common._M(52), emitterScale);
			EmitterSettings emitterSettings = new EmitterSettings();
			emitterSettings.mTintStrength = 1f;
			emitterSettings.mVisibility = Common._M(0.76f);
			emitter.AddSettingsKeyFrame(0, emitterSettings);
			emitterSettings = new EmitterSettings(emitterSettings);
			emitterSettings.mVisibility = Common._M(0.69f);
			emitter.AddSettingsKeyFrame(Common._M(21), emitterSettings);
			emitterSettings = new EmitterSettings(emitterSettings);
			emitterSettings.mVisibility = 0f;
			emitter.AddSettingsKeyFrame(Common._M(60), emitterSettings);
			ParticleType particleType = new ParticleType();
			particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_RING);
			particleType.mAdditive = true;
			particleType.mEmitterAttachPct = 1f;
			particleType.mColorKeyManager.AddColorKey(0f, SexyFramework.Graphics.Color.White);
			particleType.mColorKeyManager.AddColorKey(1f, SexyFramework.Graphics.Color.Black);
			particleType.mAlphaKeyManager.AddAlphaKey(0f, 255);
			particleType.mAlphaKeyManager.AddAlphaKey(Common._M(0.5f), 255);
			particleType.mAlphaKeyManager.AddAlphaKey(1f, 0);
			ParticleSettings particleSettings = new ParticleSettings();
			particleSettings.mLife = Common._M(96);
			particleSettings.mNumber = (int)((float)Common._M(6) * MULTIPLIER_BALL_EFFECT_PARTICLE_COUNT_SCALE);
			particleSettings.mXSize = Common._M(69);
			particleType.AddSettingsKeyFrame(0, particleSettings);
			LifetimeSettings lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = Common._M(2f);
			lifetimeSettings.mVelocityMult = Common._M(1.6f);
			particleType.AddSettingAtLifePct(0f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings(lifetimeSettings);
			lifetimeSettings.mVelocityMult = Common._M(1f);
			particleType.AddSettingAtLifePct(1f, lifetimeSettings);
			emitter.AddParticleType(particleType);
			mRings.AddEmitter(emitter);
			mRings = mSpawnEffect.mSwirl;
			mRings.SetLife(Common._M(56));
			emitter = new Emitter();
			emitter.mDeleteInvisParticles = true;
			emitter.mEmissionCoordsAreOffsets = true;
			emitter.mCullingRect = new Rect(0, 0, Common._SS(GlobalMembers.gSexyAppBase.mWidth), Common._SS(GlobalMembers.gSexyAppBase.mHeight));
			emitterScale = new EmitterScale();
			emitter.mTintColor = gSpawnColors[mMultBall.GetColorType()].mSwirl;
			emitter.SetEmitterType(2);
			emitter.mEmitDir = 1;
			emitter.mEmitAtXPoints = Common._M(20);
			emitter.mLinearEmitAtPoints = true;
			emitterScale.mLifeScale = Common._M(0.1f);
			emitterScale.mNumberScale = Common._M(20f);
			emitterScale.mSizeXScale = Common._M(5f);
			emitterScale.mVelocityScale = Common._M(2f);
			emitterScale.mZoom = Common._M(0.5f);
			emitter.AddScaleKeyFrame(0, emitterScale);
			emitterSettings = new EmitterSettings();
			emitterSettings.mTintStrength = Common._M(0.81f);
			emitterSettings.mEmissionAngle = 0f;
			emitterSettings.mEmissionRange = SexyFramework.Common.DegreesToRadians(1f);
			EmitterSettings emitterSettings2 = emitterSettings;
			float mXRadius = (emitterSettings.mYRadius = 5f);
			emitterSettings2.mXRadius = mXRadius;
			emitter.AddSettingsKeyFrame(0, emitterSettings);
			particleType = new ParticleType();
			particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_STARBURST);
			particleType.mAdditive = true;
			particleType.mEmitterAttachPct = 1f;
			particleType.mColorKeyManager.AddColorKey(0f, SexyFramework.Graphics.Color.White);
			particleType.mColorKeyManager.AddColorKey(Common._M(0.9f), SexyFramework.Graphics.Color.White);
			particleType.mColorKeyManager.AddColorKey(1f, SexyFramework.Graphics.Color.Black);
			particleType.mAlphaKeyManager.AddAlphaKey(0f, 255);
			particleType.mAlphaKeyManager.AddAlphaKey(Common._M(0.9f), 255);
			particleType.mAlphaKeyManager.AddAlphaKey(1f, 0);
			particleSettings = new ParticleSettings();
			particleSettings.mLife = Common._M(70);
			particleSettings.mNumber = (int)((float)Common._M(10) * MULTIPLIER_BALL_EFFECT_PARTICLE_COUNT_SCALE);
			particleSettings.mXSize = Common._M(11);
			particleSettings.mVelocity = Common._M(83);
			particleSettings.mWeight = Common._M(2);
			particleType.AddSettingsKeyFrame(0, particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleSettings.mLife = Common._M(70);
			particleType.AddSettingsKeyFrame(Common._M(10), particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleSettings.mLife = 0;
			particleType.AddSettingsKeyFrame(Common._M(22), particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleSettings.mGlobalVisibility = 1f;
			particleType.AddSettingsKeyFrame(Common._M(48), particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleSettings.mGlobalVisibility = 0f;
			particleType.AddSettingsKeyFrame(Common._M(56), particleSettings);
			lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mVelocityMult = Common._M(2f);
			particleType.AddSettingAtLifePct(0f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings(lifetimeSettings);
			particleType.AddSettingAtLifePct(Common._M(0.78f), lifetimeSettings);
			lifetimeSettings = new LifetimeSettings(lifetimeSettings);
			lifetimeSettings.mSizeXMult = Common._M(2f);
			lifetimeSettings.mVelocityMult = Common._M(1.7f);
			particleType.AddSettingAtLifePct(Common._M(0.92f), lifetimeSettings);
			lifetimeSettings = new LifetimeSettings(lifetimeSettings);
			lifetimeSettings.mVelocityMult = Common._M(1.65f);
			particleType.AddSettingAtLifePct(Common._M(0.94f), lifetimeSettings);
			lifetimeSettings = new LifetimeSettings(lifetimeSettings);
			lifetimeSettings.mVelocityMult = 0f;
			particleType.AddSettingAtLifePct(1f, lifetimeSettings);
			emitter.AddParticleType(particleType);
			mRings.AddEmitter(emitter);
		}
	}

	protected void UpdateStateSpawn()
	{
		mSpawnTimer++;
		if (mSpawnTimer % Common._M(30) == 0 && mMultBall != null && mSpawnTimer < Common._M1(50))
		{
			AlphaFadeInfo alphaFadeInfo = new AlphaFadeInfo();
			alphaFadeInfo.second = false;
			alphaFadeInfo.first = new AlphaFader();
			alphaFadeInfo.first.mColor = new FColor(gSpawnColors[mMultBall.GetColorType()].mBeam);
			alphaFadeInfo.first.mColor.mAlpha = 0f;
			alphaFadeInfo.first.mMin = 0;
			alphaFadeInfo.first.mMax = 255;
			alphaFadeInfo.first.mFadeRate = Common._M(6f);
			mBeamAlphas.Add(alphaFadeInfo);
		}
		for (int i = 0; i < mBeamAlphas.size(); i++)
		{
			AlphaFadeInfo alphaFadeInfo2 = mBeamAlphas[i];
			alphaFadeInfo2.first.Update();
			if (!alphaFadeInfo2.second && SexyFramework.Common._eq(alphaFadeInfo2.first.mColor.mAlpha, alphaFadeInfo2.first.mMax))
			{
				alphaFadeInfo2.second = true;
				alphaFadeInfo2.first.mFadeRate = Common._M(-10f);
			}
			else if (alphaFadeInfo2.second && SexyFramework.Common._leq(alphaFadeInfo2.first.mColor.mAlpha, alphaFadeInfo2.first.mMin))
			{
				mBeamAlphas.RemoveAt(i);
				i--;
			}
		}
		if (mSpawnTimer >= Common._M(50))
		{
			mSpawnEffect.mRings.SetPos(mLastBallX, mLastBallY);
			mSpawnEffect.mRings.Update();
			mSpawnEffect.mSwirl.SetPos(mLastBallX, mLastBallY);
			mSpawnEffect.mSwirl.Update();
		}
	}

	protected void DrawStateSpawn(Graphics g)
	{
		if (mSpawnTimer >= Common._M(50))
		{
			mSpawnEffect.mRings.Draw(g);
			mSpawnEffect.mSwirl.Draw(g);
		}
		int num = Common._M(228);
		int num2 = Common._M(21);
		float rot = SexyFramework.Common.AngleBetweenPoints(mLastBallX, mLastBallY, num, num2) - SexyFramework.Common.JL_PI / 2f;
		Image imageByID = Res.GetImageByID(ResID.IMAGE_PARTICLE_BEAM);
		_ = imageByID.mWidth;
		int mHeight = imageByID.mHeight;
		float num3 = Common._S(SexyFramework.Common.Distance(mLastBallX, mLastBallY, num, num2) + (float)Common._M(17));
		float num4 = num3 / (float)mHeight;
		float num5 = (float)mHeight * num4;
		mGlobalTranform.Reset();
		mGlobalTranform.Scale(1f, num4);
		mGlobalTranform.Translate(Common._M(0), num5 / 2f);
		mGlobalTranform.RotateRad(rot);
		mGlobalTranform.Translate(Common._M(0), (0f - num5) / 2f);
		for (int i = 0; i < mBeamAlphas.size(); i++)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(mBeamAlphas[i].first.mColor);
			g.DrawImageTransform(imageByID, mGlobalTranform, Common._S(num), (float)Common._S(num2) + num3 / 2f);
			g.SetDrawMode(1);
			g.DrawImageTransform(imageByID, mGlobalTranform, Common._S(num), (float)Common._S(num2) + num3 / 2f);
			g.SetDrawMode(0);
			g.SetColorizeImages(colorizeImages: false);
		}
	}

	protected void InitTriggeredEffects()
	{
		if (mTriggeredEffect == null)
		{
			mTriggeredEffect = new TriggeredEffect();
			SexyFramework.PIL.System mRings = mTriggeredEffect.mRings;
			mRings.SetLife(Common._M(44));
			Emitter emitter = new Emitter();
			emitter.mDeleteInvisParticles = true;
			emitter.mCullingRect = new Rect(0, 0, Common._SS(GlobalMembers.gSexyAppBase.mWidth), Common._SS(GlobalMembers.gSexyAppBase.mHeight));
			emitter.mDeleteInvisParticles = true;
			EmitterScale emitterScale = new EmitterScale();
			emitterScale.mLifeScale = Common._M(0.3f);
			emitterScale.mNumberScale = Common._M(1.06f);
			emitterScale.mSizeXScale = Common._M(1.55f);
			emitterScale.mVelocityScale = Common._M(0.79f);
			emitter.AddScaleKeyFrame(0, emitterScale);
			emitterScale = new EmitterScale(emitterScale);
			emitterScale.mSizeXScale = Common._M(2.79f);
			emitter.AddScaleKeyFrame(Common._M(29), emitterScale);
			EmitterSettings emitterSettings = new EmitterSettings();
			emitterSettings.mVisibility = Common._M(0.36f);
			emitter.AddSettingsKeyFrame(0, emitterSettings);
			emitterSettings = new EmitterSettings(emitterSettings);
			emitterSettings.mVisibility = Common._M(1f);
			emitter.AddSettingsKeyFrame(Common._M(19), emitterSettings);
			emitterSettings = new EmitterSettings(emitterSettings);
			emitterSettings.mVisibility = 0f;
			emitter.AddSettingsKeyFrame(Common._M(44), emitterSettings);
			ParticleType particleType = new ParticleType();
			particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_RING);
			particleType.mName = "Rings";
			particleType.mAdditive = true;
			particleType.mEmitterAttachPct = 1f;
			particleType.mColorKeyManager.AddColorKey(0f, new SexyFramework.Graphics.Color(96, 255, 139));
			particleType.mColorKeyManager.AddColorKey(0.12f, new SexyFramework.Graphics.Color(213, 255, 87));
			particleType.mColorKeyManager.AddColorKey(0.28f, new SexyFramework.Graphics.Color(255, 0, 0));
			particleType.mColorKeyManager.AddColorKey(0.54f, new SexyFramework.Graphics.Color(0, 72, 255));
			particleType.mColorKeyManager.AddColorKey(1f, new SexyFramework.Graphics.Color(12, 0, 255));
			particleType.mAlphaKeyManager.AddAlphaKey(0f, 255);
			particleType.mAlphaKeyManager.AddAlphaKey(Common._M(0.6f), 255);
			particleType.mAlphaKeyManager.AddAlphaKey(1f, 0);
			ParticleSettings particleSettings = new ParticleSettings();
			particleSettings.mLife = Common._M(6);
			particleSettings.mNumber = (int)((float)Common._M(10) * MULTIPLIER_BALL_EFFECT_PARTICLE_COUNT_SCALE);
			particleSettings.mXSize = Common._S(Common._M(69));
			particleType.AddSettingsKeyFrame(0, particleSettings);
			LifetimeSettings lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = Common._M(0.38f);
			lifetimeSettings.mVelocityMult = Common._M(1.7f);
			particleType.AddSettingAtLifePct(0f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = Common._M(1.4f);
			lifetimeSettings.mVelocityMult = Common._M(1f);
			particleType.AddSettingAtLifePct(1f, lifetimeSettings);
			emitter.AddParticleType(particleType);
			mRings.AddEmitter(emitter);
			int num = Common._M(100);
			mRings = mTriggeredEffect.mRainbow;
			mRings.SetLife(num);
			emitter = new Emitter();
			emitter.mDeleteInvisParticles = true;
			emitter.mCullingRect = new Rect(0, 0, Common._SS(GlobalMembers.gSexyAppBase.mWidth), Common._SS(GlobalMembers.gSexyAppBase.mHeight));
			emitterScale = new EmitterScale();
			emitterScale.mLifeScale = Common._M(1f);
			emitterScale.mNumberScale = Common._M(0.5f);
			emitterScale.mSpinScale = Common._M(0.19f);
			emitterScale.mSizeXScale = Common._M(1.24f);
			emitterScale.mSizeYScale = Common._M(0.9f);
			emitter.AddScaleKeyFrame(0, emitterScale);
			emitterScale = new EmitterScale(emitterScale);
			emitter.AddScaleKeyFrame(Common._M(9), emitterScale);
			emitterScale = new EmitterScale(emitterScale);
			emitterScale.mSizeXScale = Common._M(1.24f);
			emitterScale.mSizeYScale = Common._M(0.71f);
			emitter.AddScaleKeyFrame(Common._M(15), emitterScale);
			emitterScale = new EmitterScale(emitterScale);
			emitterScale.mSizeXScale = Common._M(1f);
			emitterScale.mSizeYScale = 0f;
			emitter.AddScaleKeyFrame(Common._M(33), emitterScale);
			emitterScale = new EmitterScale(emitterScale);
			emitterScale.mSizeXScale = 0f;
			emitter.AddScaleKeyFrame(num, emitterScale);
			emitterSettings = new EmitterSettings();
			emitterSettings.mVisibility = Common._M(0.58f);
			emitter.AddSettingsKeyFrame(0, emitterSettings);
			emitterSettings = new EmitterSettings();
			emitterSettings.mVisibility = Common._M(1f);
			emitter.AddSettingsKeyFrame(Common._M(7), emitterSettings);
			emitterSettings = new EmitterSettings();
			emitter.AddSettingsKeyFrame(Common._M(19), emitterSettings);
			emitterSettings = new EmitterSettings();
			emitterSettings.mVisibility = Common._M(0.29f);
			emitter.AddSettingsKeyFrame(Common._M(50), emitterSettings);
			emitterSettings = new EmitterSettings();
			emitterSettings.mVisibility = 0f;
			emitter.AddSettingsKeyFrame(num, emitterSettings);
			particleType = new ParticleType();
			particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_BEAM);
			particleType.mName = "Rainbow beam crap";
			particleType.mAdditive = true;
			particleType.mAdditiveWithNormal = true;
			particleType.mRefYOff = (int)(4f * (float)Common._M(-300));
			particleType.mInitAngle = (float)Math.PI;
			particleType.mAngleRange = (float)Math.PI * 2f;
			particleType.mInitAngleStep = Common._M(0.5f);
			particleType.mLockSizeAspect = false;
			particleType.mAlphaKeyManager.AddAlphaKey(0f, 0);
			particleType.mAlphaKeyManager.AddAlphaKey(Common._M(0.25f), 255);
			particleType.mAlphaKeyManager.AddAlphaKey(Common._M(0.75f), 255);
			particleType.mAlphaKeyManager.AddAlphaKey(1f, 0);
			particleType.mColorKeyManager.AddColorKey(0f, new SexyFramework.Graphics.Color(0, 220, 250));
			particleType.mColorKeyManager.AddColorKey(0.25f, new SexyFramework.Graphics.Color(51, 0, 255));
			particleType.mColorKeyManager.AddColorKey(0.375f, new SexyFramework.Graphics.Color(225, 0, 255));
			particleType.mColorKeyManager.AddColorKey(0.5f, new SexyFramework.Graphics.Color(255, 0, 0));
			particleType.mColorKeyManager.AddColorKey(0.675f, new SexyFramework.Graphics.Color(225, 123, 0));
			particleType.mColorKeyManager.AddColorKey(0.75f, new SexyFramework.Graphics.Color(7, 255, 12));
			particleType.mColorKeyManager.AddColorKey(1f, new SexyFramework.Graphics.Color(229, 255, 0));
			particleSettings = new ParticleSettings();
			particleSettings.mLife = Common._M(9);
			particleSettings.mNumber = (int)((float)Common._M(80) * MULTIPLIER_BALL_EFFECT_PARTICLE_COUNT_SCALE);
			particleSettings.mXSize = Common._M(50);
			particleSettings.mYSize = Common._M(159);
			particleSettings.mSpin = SexyFramework.Common.DegreesToRadians(Common._M(-70));
			particleType.AddSettingsKeyFrame(0, particleSettings);
			ParticleVariance particleVariance = new ParticleVariance();
			particleVariance.mSpinVar = SexyFramework.Common.DegreesToRadians(Common._M(68));
			particleType.AddVarianceKeyFrame(0, particleVariance);
			lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = 1f;
			particleType.AddSettingAtLifePct(0f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings(lifetimeSettings);
			particleType.AddSettingAtLifePct(0.7f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings(lifetimeSettings);
			lifetimeSettings.mSizeXMult = 0f;
			particleType.AddSettingAtLifePct(1f, lifetimeSettings);
			emitter.AddParticleType(particleType);
			mRings.AddEmitter(emitter);
			mRings = mTriggeredEffect.mGas;
			mRings.SetLife(Common._M(200));
			emitter = new Emitter();
			emitter.mDeleteInvisParticles = true;
			emitter.mPreloadFrames = Common._M(0);
			emitter.mCullingRect = new Rect(0, 0, Common._SS(GlobalMembers.gSexyAppBase.mWidth), Common._SS(GlobalMembers.gSexyAppBase.mHeight));
			emitterScale = new EmitterScale();
			emitterScale.mLifeScale = Common._M(0.56f);
			emitterScale.mNumberScale = Common._M(0.45f);
			emitterScale.mSizeXScale = Common._M(2.87f);
			emitterScale.mVelocityScale = Common._M(0.18f);
			emitterScale.mZoom = Common._M(0.33f);
			emitterScale.mSpinScale = Common._M(1.75f);
			emitter.AddScaleKeyFrame(0, emitterScale);
			emitterScale = new EmitterScale(emitterScale);
			emitterScale.mZoom = Common._M(0.5f);
			emitter.AddScaleKeyFrame(Common._M(25), emitterScale);
			emitterScale = new EmitterScale(emitterScale);
			emitter.AddScaleKeyFrame(Common._M(75), emitterScale);
			emitterScale = new EmitterScale(emitterScale);
			emitterScale.mVelocityScale = Common._M(0.48f);
			emitter.AddScaleKeyFrame(Common._M(87), emitterScale);
			emitterSettings = new EmitterSettings();
			emitterSettings.mVisibility = Common._M(0.56f);
			emitter.AddSettingsKeyFrame(0, emitterSettings);
			particleType = new ParticleType();
			particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_SPIKEYCIRCLE);
			particleType.mAdditive = true;
			particleType.mColorKeyManager.AddColorKey(0f, new SexyFramework.Graphics.Color(232, 255, 32));
			particleType.mColorKeyManager.AddColorKey(Common._M(0.375f), new SexyFramework.Graphics.Color(98, 254, 255));
			particleType.mColorKeyManager.AddColorKey(Common._M(0.675f), new SexyFramework.Graphics.Color(255, 101, 206));
			particleType.mColorKeyManager.AddColorKey(1f, new SexyFramework.Graphics.Color(21, 9, 34));
			particleType.mAlphaKeyManager.AddAlphaKey(0f, 255);
			particleType.mAlphaKeyManager.AddAlphaKey(Common._M(0.75f), 255);
			particleType.mAlphaKeyManager.AddAlphaKey(1f, 0);
			particleType.mColorKeyManager.SetColorMode(2);
			particleType.mName = "clouds";
			particleSettings = new ParticleSettings();
			particleSettings.mLife = 0;
			particleSettings.mNumber = (int)((float)Common._M(10) * MULTIPLIER_BALL_EFFECT_PARTICLE_COUNT_SCALE);
			particleSettings.mXSize = Common._M(30);
			particleSettings.mVelocity = Common._M(243);
			particleSettings.mMotionRand = Common._M(57);
			particleSettings.mGlobalVisibility = Common._M(0.56f);
			particleType.AddSettingsKeyFrame(0, particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleSettings.mVelocity = Common._M(281);
			particleType.AddSettingsKeyFrame(Common._M(12), particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleSettings.mLife = Common._M(30);
			particleSettings.mVelocity = Common._M(333);
			particleType.AddSettingsKeyFrame(Common._M(30), particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleSettings.mLife = 0;
			particleSettings.mVelocity = Common._M(346);
			particleType.AddSettingsKeyFrame(Common._M(39), particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleSettings.mVelocity = Common._M(419);
			particleType.AddSettingsKeyFrame(Common._M(89), particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleSettings.mGlobalVisibility = 0f;
			particleSettings.mVelocity = Common._M(435);
			particleType.AddSettingsKeyFrame(Common._M(100), particleSettings);
			particleVariance = new ParticleVariance();
			particleVariance.mSizeXVar = Common._M(20);
			particleVariance.mVelocityVar = Common._M(26);
			particleVariance.mWeightVar = Common._M(9);
			particleType.AddVarianceKeyFrame(0, particleVariance);
			lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = 0f;
			lifetimeSettings.mVelocityMult = Common._M(0.85f);
			particleType.AddSettingAtLifePct(0f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings(lifetimeSettings);
			lifetimeSettings.mSizeXMult = Common._M(1.8f);
			lifetimeSettings.mVelocityMult = Common._M(1.2f);
			particleType.AddSettingAtLifePct(Common._M(0.3f), lifetimeSettings);
			lifetimeSettings = new LifetimeSettings(lifetimeSettings);
			lifetimeSettings.mSizeXMult = Common._M(0.3f);
			lifetimeSettings.mVelocityMult = Common._M(1.6f);
			particleType.AddSettingAtLifePct(Common._M(0.68f), lifetimeSettings);
			lifetimeSettings = new LifetimeSettings(lifetimeSettings);
			lifetimeSettings.mSizeXMult = 0f;
			lifetimeSettings.mVelocityMult = 2f;
			particleType.AddSettingAtLifePct(1f, lifetimeSettings);
			emitter.AddParticleType(particleType);
			particleType = new ParticleType();
			particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_GAS);
			particleType.mRefXOff = (int)(4f * (float)Common._M(-3));
			particleType.mRefYOff = (int)(4f * (float)Common._M(-12));
			particleType.mColorKeyManager.AddColorKey(0f, new SexyFramework.Graphics.Color(255, 0, 0));
			particleType.mColorKeyManager.AddColorKey(0.125f, new SexyFramework.Graphics.Color(148, 0, 255));
			particleType.mColorKeyManager.AddColorKey(0.375f, new SexyFramework.Graphics.Color(0, 33, 255));
			particleType.mColorKeyManager.AddColorKey(0.5f, new SexyFramework.Graphics.Color(7, 222, 255));
			particleType.mColorKeyManager.AddColorKey(0.675f, new SexyFramework.Graphics.Color(0, 255, 42));
			particleType.mColorKeyManager.AddColorKey(0.75f, new SexyFramework.Graphics.Color(9, 156, 26));
			particleType.mColorKeyManager.AddColorKey(0.9f, new SexyFramework.Graphics.Color(255, 144, 0));
			particleType.mColorKeyManager.AddColorKey(1f, new SexyFramework.Graphics.Color(255, 255, 255));
			particleType.mAlphaKeyManager.AddAlphaKey(0f, 255);
			particleType.mAlphaKeyManager.AddAlphaKey(Common._M(0.8f), 255);
			particleType.mAlphaKeyManager.AddAlphaKey(1f, 150);
			particleType.mColorKeyManager.SetColorMode(2);
			particleSettings = new ParticleSettings();
			particleSettings.mLife = Common._M(40);
			particleSettings.mNumber = (int)((float)Common._M(29) * MULTIPLIER_BALL_EFFECT_PARTICLE_COUNT_SCALE);
			particleSettings.mXSize = Common._M(60);
			particleSettings.mVelocity = Common._M(3);
			particleSettings.mWeight = Common._M(0);
			particleSettings.mSpin = SexyFramework.Common.DegreesToRadians(Common._M(6));
			particleSettings.mGlobalVisibility = 0f;
			particleType.AddSettingsKeyFrame(0, particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleSettings.mNumber = (int)((float)Common._M(8) * MULTIPLIER_BALL_EFFECT_PARTICLE_COUNT_SCALE);
			particleSettings.mGlobalVisibility = Common._M(0.22f);
			particleType.AddSettingsKeyFrame(Common._M(12), particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleSettings.mNumber = 0;
			particleSettings.mLife = Common._M(53);
			particleSettings.mGlobalVisibility = Common._M(0.3f);
			particleType.AddSettingsKeyFrame(Common._M(16), particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleSettings.mGlobalVisibility = 1f;
			particleSettings.mLife = Common._M(40);
			particleType.AddSettingsKeyFrame(Common._M(29), particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleSettings.mLife = Common._M(16);
			particleType.AddSettingsKeyFrame(Common._M(51), particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleSettings.mGlobalVisibility = Common._M(0.2f);
			particleSettings.mLife = 0;
			particleType.AddSettingsKeyFrame(Common._M(70), particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleSettings.mGlobalVisibility = 0f;
			particleType.AddSettingsKeyFrame(Common._M(90), particleSettings);
			particleVariance = new ParticleVariance();
			particleVariance.mLifeVar = Common._M(63);
			particleVariance.mSizeXVar = Common._M(7);
			particleVariance.mWeightVar = Common._M(0);
			particleVariance.mSpinVar = SexyFramework.Common.DegreesToRadians(Common._M(30));
			particleVariance.mMotionRandVar = Common._M(0);
			particleType.AddVarianceKeyFrame(0, particleVariance);
			particleVariance = new ParticleVariance(particleVariance);
			particleType.AddVarianceKeyFrame(Common._M(19), particleVariance);
			particleVariance = new ParticleVariance(particleVariance);
			particleVariance.mLifeVar = 0;
			particleType.AddVarianceKeyFrame(Common._M(35), particleVariance);
			lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = 1f;
			lifetimeSettings.mVelocityMult = 0f;
			particleType.AddSettingAtLifePct(0f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings(lifetimeSettings);
			lifetimeSettings.mSizeXMult = Common._M(1.8f);
			lifetimeSettings.mVelocityMult = Common._M(0.6f);
			particleType.AddSettingAtLifePct(0.1f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings(lifetimeSettings);
			lifetimeSettings.mSizeXMult = Common._M(1.9f);
			lifetimeSettings.mVelocityMult = Common._M(1.2f);
			particleType.AddSettingAtLifePct(0.21f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings(lifetimeSettings);
			lifetimeSettings.mSizeXMult = Common._M(2f);
			lifetimeSettings.mVelocityMult = Common._M(1.3f);
			particleType.AddSettingAtLifePct(0.28f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings(lifetimeSettings);
			lifetimeSettings.mVelocityMult = Common._M(1.4f);
			particleType.AddSettingAtLifePct(0.42f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings(lifetimeSettings);
			lifetimeSettings.mSizeXMult = Common._M(1f);
			lifetimeSettings.mVelocityMult = Common._M(1.8f);
			particleType.AddSettingAtLifePct(0.75f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings(lifetimeSettings);
			lifetimeSettings.mSizeXMult = Common._M(0.4f);
			lifetimeSettings.mVelocityMult = Common._M(1.9f);
			particleType.AddSettingAtLifePct(0.88f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings(lifetimeSettings);
			lifetimeSettings.mSizeXMult = Common._M(0.2f);
			lifetimeSettings.mVelocityMult = Common._M(2f);
			particleType.AddSettingAtLifePct(1f, lifetimeSettings);
			emitter.AddParticleType(particleType);
			mRings.AddEmitter(emitter);
			mRings = mTriggeredEffect.mFlare;
			emitter = new Emitter();
			emitter.mCullingRect = new Rect(0, 0, Common._SS(GlobalMembers.gSexyAppBase.mWidth), Common._SS(GlobalMembers.gSexyAppBase.mHeight));
			emitter.mDeleteInvisParticles = true;
			emitterScale = new EmitterScale();
			emitterScale.mLifeScale = Common._M(1.5f);
			emitterScale.mNumberScale = Common._M(3.15f);
			emitterScale.mZoom = Common._M(3.12f);
			emitterScale.mSizeXScale = Common._M(2.03f);
			emitter.AddScaleKeyFrame(0, emitterScale);
			emitterScale = new EmitterScale(emitterScale);
			emitterScale.mSizeXScale = Common._M(2.37f);
			emitter.AddScaleKeyFrame(Common._M(9), emitterScale);
			emitterScale = new EmitterScale(emitterScale);
			emitterScale.mSizeXScale = Common._M(2.06f);
			emitter.AddScaleKeyFrame(Common._M(16), emitterScale);
			emitterScale = new EmitterScale(emitterScale);
			emitterScale.mSizeXScale = Common._M(0);
			emitter.AddScaleKeyFrame(Common._M(60), emitterScale);
			emitterSettings = new EmitterSettings();
			emitterSettings.mVisibility = Common._M(0.5f);
			emitter.AddSettingsKeyFrame(0, emitterSettings);
			emitterSettings = new EmitterSettings();
			emitterSettings.mVisibility = Common._M(1f);
			emitter.AddSettingsKeyFrame(20, emitterSettings);
			emitterSettings = new EmitterSettings();
			emitterSettings.mVisibility = Common._M(0f);
			emitter.AddSettingsKeyFrame(35, emitterSettings);
			particleType = new ParticleType();
			particleType.mAdditive = true;
			particleType.mEmitterAttachPct = 1f;
			particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_STARBURST);
			particleType.mColorKeyManager.AddColorKey(0f, new SexyFramework.Graphics.Color(10, 255, 88));
			particleType.mColorKeyManager.AddColorKey(0.25f, new SexyFramework.Graphics.Color(25, 0, 255));
			particleType.mColorKeyManager.AddColorKey(0.5f, new SexyFramework.Graphics.Color(255, 0, 161));
			particleType.mColorKeyManager.AddColorKey(0.75f, new SexyFramework.Graphics.Color(255, 0, 0));
			particleType.mColorKeyManager.AddColorKey(1f, new SexyFramework.Graphics.Color(254, 255, 0));
			particleType.mAlphaKeyManager.AddAlphaKey(0f, 255);
			particleSettings = new ParticleSettings();
			particleSettings.mLife = Common._M(8);
			particleSettings.mNumber = (int)((float)Common._M(5) * MULTIPLIER_BALL_EFFECT_PARTICLE_COUNT_SCALE);
			particleSettings.mXSize = Common._M(28);
			particleSettings.mVelocity = Common._M(4);
			particleSettings.mGlobalVisibility = Common._M(0.76f);
			particleType.AddSettingsKeyFrame(0, particleSettings);
			particleVariance = new ParticleVariance();
			particleVariance.mLifeVar = Common._M(14);
			particleVariance.mVelocityVar = Common._M(6);
			particleType.AddVarianceKeyFrame(0, particleVariance);
			lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = 0f;
			particleType.AddSettingAtLifePct(0f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = Common._M(1.5f);
			particleType.AddSettingAtLifePct(0.25f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = Common._M(0.6f);
			particleType.AddSettingAtLifePct(0.37f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = Common._M(0.1f);
			particleType.AddSettingAtLifePct(1f, lifetimeSettings);
			emitter.AddParticleType(particleType);
			particleType = new ParticleType();
			particleType.mAdditive = true;
			particleType.mEmitterAttachPct = 1f;
			particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_BIG_STAR);
			particleType.mAlphaKeyManager.AddAlphaKey(0f, 255);
			particleType.mColorKeyManager.AddColorKey(0f, new SexyFramework.Graphics.Color(255, 0, 0));
			particleType.mColorKeyManager.AddColorKey(0.4f, new SexyFramework.Graphics.Color(255, 246, 1));
			particleType.mColorKeyManager.AddColorKey(0.675f, new SexyFramework.Graphics.Color(242, 255, 0));
			particleType.mColorKeyManager.AddColorKey(0.8f, new SexyFramework.Graphics.Color(SexyFramework.Graphics.Color.White));
			particleType.mColorKeyManager.AddColorKey(1f, new SexyFramework.Graphics.Color(SexyFramework.Graphics.Color.White));
			particleSettings = new ParticleSettings();
			particleSettings.mLife = Common._M(8);
			particleSettings.mNumber = (int)((float)Common._M(2) * MULTIPLIER_BALL_EFFECT_PARTICLE_COUNT_SCALE);
			particleSettings.mXSize = Common._M(16);
			particleSettings.mGlobalVisibility = Common._M(0.41f);
			particleType.AddSettingsKeyFrame(0, particleSettings);
			particleVariance = new ParticleVariance();
			particleVariance.mLifeVar = Common._M(14);
			particleType.AddVarianceKeyFrame(0, particleVariance);
			lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = 0f;
			particleType.AddSettingAtLifePct(0f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = Common._M(1.5f);
			particleType.AddSettingAtLifePct(0.25f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = Common._M(0.6f);
			particleType.AddSettingAtLifePct(0.37f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = Common._M(0.1f);
			particleType.AddSettingAtLifePct(1f, lifetimeSettings);
			emitter.AddParticleType(particleType);
			mRings.AddEmitter(emitter);
			mRings = mTriggeredEffect.mTrail;
			mRings.SetLife(Common._M(200));
			emitter = new Emitter();
			emitter.mCullingRect = new Rect(0, 0, Common._SS(GlobalMembers.gSexyAppBase.mWidth), Common._SS(GlobalMembers.gSexyAppBase.mHeight));
			emitter.mDeleteInvisParticles = true;
			emitterScale = new EmitterScale();
			emitterScale.mLifeScale = Common._M(1f);
			emitterScale.mNumberScale = Common._M(1f);
			emitterScale.mSizeXScale = Common._M(0.59f);
			emitterScale.mVelocityScale = Common._M(1f);
			emitterScale.mWeightScale = Common._M(3f);
			emitterScale.mSpinScale = Common._M(0.54f);
			emitterScale.mZoom = Common._M(3.63f);
			emitter.AddScaleKeyFrame(0, emitterScale);
			emitterScale = new EmitterScale(emitterScale);
			emitter.AddScaleKeyFrame(Common._M(36), emitterScale);
			emitterScale = new EmitterScale(emitterScale);
			emitterScale.mLifeScale = 0f;
			emitter.AddScaleKeyFrame(Common._M(51), emitterScale);
			emitterSettings = new EmitterSettings();
			emitterSettings.mVisibility = 1f;
			emitter.AddSettingsKeyFrame(0, emitterSettings);
			emitterSettings = new EmitterSettings(emitterSettings);
			emitter.AddSettingsKeyFrame(Common._M(37), emitterSettings);
			emitterSettings = new EmitterSettings(emitterSettings);
			emitterSettings.mVisibility = 0f;
			emitter.AddSettingsKeyFrame(Common._M(96), emitterSettings);
			particleType = new ParticleType();
			particleType.mAdditive = true;
			particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_BIG_STAR);
			particleType.mColorKeyManager.AddColorKey(0f, new SexyFramework.Graphics.Color(0, 255, 4));
			particleType.mColorKeyManager.AddColorKey(0.25f, new SexyFramework.Graphics.Color(242, 255, 0));
			particleType.mColorKeyManager.AddColorKey(0.45f, new SexyFramework.Graphics.Color(255, 0, 0));
			particleType.mColorKeyManager.AddColorKey(0.65f, new SexyFramework.Graphics.Color(38, 0, 255));
			particleType.mColorKeyManager.AddColorKey(1f, new SexyFramework.Graphics.Color(113, 38, 255));
			particleType.mAlphaKeyManager.SetFixedColor(new SexyFramework.Graphics.Color(SexyFramework.Graphics.Color.White));
			particleType.mColorKeyManager.SetColorMode(2);
			particleSettings = new ParticleSettings();
			particleSettings.mLife = Common._M(29);
			particleSettings.mNumber = (int)((float)Common._M(83) * MULTIPLIER_BALL_EFFECT_PARTICLE_COUNT_SCALE);
			particleSettings.mXSize = Common._M(10);
			particleSettings.mVelocity = Common._M(3);
			particleSettings.mWeight = Common._M(-8);
			particleSettings.mSpin = SexyFramework.Common.DegreesToRadians(Common._M(3));
			particleType.AddSettingsKeyFrame(0, particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleType.AddSettingsKeyFrame(Common._M(34), particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleSettings.mVelocity = Common._M(72);
			particleType.AddSettingsKeyFrame(Common._M(39), particleSettings);
			particleVariance = new ParticleVariance();
			particleVariance.mNumberVar = Common._M(28);
			particleVariance.mSizeXVar = Common._M(7);
			particleVariance.mVelocityVar = Common._M(24);
			particleVariance.mSpinVar = SexyFramework.Common.DegreesToRadians(Common._M(210));
			particleVariance.mMotionRandVar = Common._M(44);
			particleType.AddVarianceKeyFrame(0, particleVariance);
			lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = Common._M(1.6f);
			particleType.AddSettingAtLifePct(0f, lifetimeSettings);
			lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = Common._M(1.4f);
			particleType.AddSettingAtLifePct(Common._M(0.43f), lifetimeSettings);
			lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = Common._M(1.2f);
			particleType.AddSettingAtLifePct(Common._M(0.63f), lifetimeSettings);
			lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = Common._M(0.7f);
			particleType.AddSettingAtLifePct(Common._M(0.8f), lifetimeSettings);
			lifetimeSettings = new LifetimeSettings();
			lifetimeSettings.mSizeXMult = 0f;
			particleType.AddSettingAtLifePct(1f, lifetimeSettings);
			emitter.AddParticleType(particleType);
			particleType = new ParticleType();
			particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_ROUND);
			particleType.mColorKeyManager.SetFixedColor(new SexyFramework.Graphics.Color(21, 0, 211));
			particleType.mAlphaKeyManager.SetFixedColor(new SexyFramework.Graphics.Color(255, 255, 255, 211));
			particleType.mAdditive = true;
			particleType.mSingle = true;
			particleSettings = new ParticleSettings();
			particleSettings.mLife = Common._M(100);
			particleSettings.mNumber = (int)((float)Common._M(33) * MULTIPLIER_BALL_EFFECT_PARTICLE_COUNT_SCALE);
			particleSettings.mXSize = Common._M(100);
			particleType.AddSettingsKeyFrame(0, particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleType.AddSettingsKeyFrame(Common._M(24), particleSettings);
			particleSettings = new ParticleSettings(particleSettings);
			particleSettings.mXSize = Common._M(266);
			particleType.AddSettingsKeyFrame(Common._M(45), particleSettings);
			emitter.AddParticleType(particleType);
			float num2 = -1f;
			float num3 = -1f;
			if (mMultBall.GetX() < (float)(Common._SS(GlobalMembers.gSexyAppBase.mWidth) / 2))
			{
				num2 = 1f;
			}
			if (mMultBall.GetY() < (float)(Common._SS(GlobalMembers.gSexyAppBase.mHeight) / 2))
			{
				num3 = 1f;
			}
			Vector2 c = new Vector2(num2 * (float)Common._M(150) + mMultBall.GetX(), num3 * (float)Common._M1(250) + mMultBall.GetY());
			emitter.mWaypointManager.AddPoint(0, new Vector2(mMultBall.GetX(), mMultBall.GetY()), linear: false, c);
			c.X = Common._M(649);
			c.Y = Common._M(169);
			Gun gun = GameApp.gApp.GetBoard().GetGun();
			emitter.mWaypointManager.AddPoint(Common._M(45), new Vector2(Common._M1(0) + gun.GetCenterX(), Common._M2(-50) + gun.GetCenterY()), linear: false, c);
			bool make_curve_image = false;
			emitter.mWaypointManager.Init(make_curve_image);
			gTrailHandle = mRings.AddEmitter(emitter);
		}
	}

	protected void UpdateStateTriggered()
	{
		mTriggerTimer++;
		mTriggeredEffect.mRings.SetPos(mLastBallX, mLastBallY);
		mTriggeredEffect.mRings.Update();
		if (mTriggerTimer >= Common._M(17))
		{
			mTriggeredEffect.mRainbow.SetPos(mLastBallX, mLastBallY);
			mTriggeredEffect.mRainbow.Update();
		}
		mTriggeredEffect.mGas.SetPos(mLastBallX, mLastBallY);
		mTriggeredEffect.mGas.Update();
		mTriggeredEffect.mFlare.SetPos(mLastBallX, mLastBallY);
		mTriggeredEffect.mFlare.Update();
		mTriggeredEffect.mTrail.SetPos(mLastBallX, mLastBallY);
		mTriggeredEffect.mTrail.Update();
		if (mDoMultFlash && mTriggeredEffect.mTrail.GetEmitter(gTrailHandle).mWaypointManager.AtEnd())
		{
			mDoMultFlash = false;
		}
	}

	protected void DrawStateTriggered(Graphics g)
	{
		mTriggeredEffect.mRings.Draw(g);
		mTriggeredEffect.mRainbow.Draw(g);
		mTriggeredEffect.mGas.Draw(g);
		mTriggeredEffect.mFlare.Draw(g);
		mTriggeredEffect.mTrail.Draw(g);
	}

	public MultiplierBallEffect(Ball mult_ball, bool spawn)
	{
		mMultBall = mult_ball;
		mSpawnTimer = 0;
		mTriggerTimer = 0;
		mSpawnEffect = null;
		mDoMultFlash = false;
		mTriggeredEffect = null;
		if (spawn)
		{
			InitSpawnEffects();
		}
		else
		{
			InitTriggeredEffects();
		}
	}

	public virtual void Dispose()
	{
		if (mSpawnEffect != null)
		{
			mSpawnEffect.Dispose();
			mSpawnEffect = null;
		}
		if (mTriggeredEffect != null)
		{
			mTriggeredEffect.Dispose();
			mTriggeredEffect = null;
		}
	}

	public void Update()
	{
		if (mMultBall != null)
		{
			mLastBallX = mMultBall.GetX();
			mLastBallY = mMultBall.GetY();
		}
		if (mSpawnEffect != null)
		{
			UpdateStateSpawn();
			if (mSpawnEffect.mRings.Done() && mSpawnEffect.mSwirl.Done())
			{
				mSpawnEffect.Dispose();
				mSpawnEffect = null;
			}
		}
		if (mTriggeredEffect != null)
		{
			UpdateStateTriggered();
			if (mTriggeredEffect.mRings.Done() && mTriggeredEffect.mRainbow.Done() && mTriggeredEffect.mGas.Done() && mTriggeredEffect.mTrail.Done())
			{
				mTriggeredEffect.Dispose();
				mTriggeredEffect = null;
			}
		}
	}

	public void Draw(Graphics g)
	{
		if (mSpawnEffect != null)
		{
			DrawStateSpawn(g);
		}
		if (mTriggeredEffect != null)
		{
			DrawStateTriggered(g);
		}
	}

	public void BallDestroyed(Ball mult_ball)
	{
		mMultBall = mult_ball;
		mLastBallX = mMultBall.GetX();
		mLastBallY = mMultBall.GetY();
		InitTriggeredEffects();
		mMultBall = null;
	}

	public Ball GetBall()
	{
		return mMultBall;
	}

	public void SyncState(DataSync sync)
	{
		sync.SyncBoolean(ref mDoMultFlash);
		sync.SyncLong(ref mState);
		sync.SyncLong(ref mSpawnTimer);
		sync.SyncLong(ref mTriggerTimer);
		sync.SyncFloat(ref mLastBallX);
		sync.SyncFloat(ref mLastBallY);
		SexyFramework.Misc.Buffer buffer = sync.GetBuffer();
		if (sync.isWrite())
		{
			buffer.WriteLong(mBeamAlphas.Count);
			for (int i = 0; i < mBeamAlphas.Count; i++)
			{
				buffer.WriteBoolean(mBeamAlphas[i].second);
				AlphaFader first = mBeamAlphas[i].first;
				buffer.WriteFloat(first.mFadeRate);
				buffer.WriteLong(first.mFadeCount);
				buffer.WriteLong(first.mMin);
				buffer.WriteLong(first.mMax);
				buffer.WriteFloat(first.mColor.mRed);
				buffer.WriteFloat(first.mColor.mGreen);
				buffer.WriteFloat(first.mColor.mBlue);
				buffer.WriteFloat(first.mColor.mAlpha);
			}
			buffer.WriteBoolean(mTriggeredEffect != null);
			if (mTriggeredEffect != null)
			{
				Common.SerializeParticleSystem(mTriggeredEffect.mRings, sync);
				Common.SerializeParticleSystem(mTriggeredEffect.mRainbow, sync);
				Common.SerializeParticleSystem(mTriggeredEffect.mGas, sync);
				Common.SerializeParticleSystem(mTriggeredEffect.mFlare, sync);
				Common.SerializeParticleSystem(mTriggeredEffect.mTrail, sync);
			}
			buffer.WriteBoolean(mSpawnEffect != null);
			if (mSpawnEffect != null)
			{
				Common.SerializeParticleSystem(mSpawnEffect.mRings, sync);
				Common.SerializeParticleSystem(mSpawnEffect.mSwirl, sync);
			}
			buffer.WriteBoolean(mMultBall != null);
			if (mMultBall != null)
			{
				buffer.WriteLong(mMultBall.GetId());
			}
			return;
		}
		int num = (int)buffer.ReadLong();
		mBeamAlphas.Clear();
		for (int j = 0; j < num; j++)
		{
			AlphaFadeInfo alphaFadeInfo = new AlphaFadeInfo(new AlphaFader(), s: false);
			alphaFadeInfo.second = buffer.ReadBoolean();
			alphaFadeInfo.first.mFadeRate = buffer.ReadFloat();
			alphaFadeInfo.first.mFadeCount = (int)buffer.ReadLong();
			alphaFadeInfo.first.mMin = (int)buffer.ReadLong();
			alphaFadeInfo.first.mMax = (int)buffer.ReadLong();
			alphaFadeInfo.first.mColor.mRed = buffer.ReadFloat();
			alphaFadeInfo.first.mColor.mGreen = buffer.ReadFloat();
			alphaFadeInfo.first.mColor.mBlue = buffer.ReadFloat();
			alphaFadeInfo.first.mColor.mAlpha = buffer.ReadFloat();
			mBeamAlphas.Add(alphaFadeInfo);
		}
		mTriggeredEffect = null;
		mSpawnEffect = null;
		if (buffer.ReadBoolean())
		{
			mTriggeredEffect = new TriggeredEffect(create: false);
			mTriggeredEffect.mRings = Common.DeserializeParticleSystem(sync);
			mTriggeredEffect.mRainbow = Common.DeserializeParticleSystem(sync);
			mTriggeredEffect.mGas = Common.DeserializeParticleSystem(sync);
			mTriggeredEffect.mFlare = Common.DeserializeParticleSystem(sync);
			mTriggeredEffect.mTrail = Common.DeserializeParticleSystem(sync);
		}
		if (buffer.ReadBoolean())
		{
			mSpawnEffect = new SpawnEffect(create: false);
			mSpawnEffect.mRings = Common.DeserializeParticleSystem(sync);
			mSpawnEffect.mSwirl = Common.DeserializeParticleSystem(sync);
		}
		if (buffer.ReadBoolean())
		{
			int id = (int)buffer.ReadLong();
			mMultBall = ((GameApp)GlobalMembers.gSexyApp).GetBoard().mLevel.GetBallById(id);
		}
	}

	public bool Done()
	{
		if (mSpawnEffect == null)
		{
			return mTriggeredEffect == null;
		}
		return false;
	}
}
