using System;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.PIL;

namespace ZumasRevenge;

public class Skull
{
	public float mParticleAlpha;

	public int mImageNum;

	public int mLeftEyeHandle;

	public int mRightEyeHandle;

	public int mTrailPTHandle;

	public int mTrailHandle;

	public float mLastX;

	public float mLastY;

	public float mLastAngle;

	public bool mIsLeft;

	public float mAlpha;

	public bool mLaunched;

	public bool mUseLastPos;

	public bool mHitPlayer;

	public Image mImage;

	public SexyFramework.PIL.System mLeftEye;

	public SexyFramework.PIL.System mRightEye;

	public SexyFramework.PIL.System mTrail;

	public SexyFramework.PIL.System mFatHead;

	public SexyFramework.PIL.System mChunks;

	public SexyFramework.PIL.System mClouds;

	public Skull(bool left)
	{
		mParticleAlpha = 1f;
		mLaunched = false;
		mUseLastPos = false;
		mLastX = (mLastY = (mLastAngle = 0f));
		mLeftEye = new SexyFramework.PIL.System(50, 50);
		mLeftEye.mScale = Common._S(1f);
		mLeftEye.WaitForEmitters(w: true);
		mRightEye = new SexyFramework.PIL.System(50, 50);
		mRightEye.mScale = Common._S(1f);
		mRightEye.WaitForEmitters(w: true);
		Emitter emitter = new Emitter();
		emitter.mCullingRect = new Rect(0, 0, Common._SS(GameApp.gApp.mWidth), Common._SS(GameApp.gApp.mHeight));
		emitter.mEmissionCoordsAreOffsets = true;
		emitter.AddScaleKeyFrame(0, new EmitterScale
		{
			mLifeScale = Common._M(0.3f),
			mNumberScale = Common._M(2.01f),
			mSizeXScale = Common._M(1.27f),
			mSizeYScale = Common._M(0.9f),
			mVelocityScale = Common._M(0.24f),
			mWeightScale = Common._M(0.48f),
			mZoom = Common._M(1.66f)
		});
		emitter.AddSettingsKeyFrame(0, new EmitterSettings
		{
			mVisibility = Common._M(0.51f)
		});
		ParticleType particleType = new ParticleType();
		particleType.mEmitterAttachPct = Common._M(1f);
		particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_FUZZY_CIRCLE);
		particleType.mAdditive = true;
		particleType.mColorKeyManager.AddColorKey(0f, new Color(1, 255, 6));
		particleType.mColorKeyManager.AddColorKey(0.5f, new Color(216, 255, 216));
		particleType.mColorKeyManager.AddColorKey(1f, new Color(67, 255, 0));
		particleType.AddSettingsKeyFrame(0, new ParticleSettings
		{
			mLife = Common._M(21),
			mNumber = Common._M(18),
			mXSize = Common._M(15),
			mVelocity = Common._M(8),
			mWeight = Common._M(-31),
			mGlobalVisibility = Common._M(0.5f)
		});
		particleType.AddVarianceKeyFrame(0, new ParticleVariance
		{
			mLifeVar = Common._M(39),
			mNumberVar = Common._M(2),
			mSizeXVar = Common._M(3),
			mVelocityVar = Common._M(5)
		});
		LifetimeSettings lifetimeSettings = new LifetimeSettings
		{
			mSizeXMult = Common._M(0.2f),
			mVelocityMult = Common._M(0.6f)
		};
		particleType.AddSettingAtLifePct(0f, lifetimeSettings);
		lifetimeSettings = new LifetimeSettings(lifetimeSettings)
		{
			mSizeXMult = Common._M(1.3f),
			mVelocityMult = Common._M(0.77f)
		};
		particleType.AddSettingAtLifePct(0.77f, lifetimeSettings);
		particleType.AddSettingAtLifePct(1f, new LifetimeSettings(lifetimeSettings)
		{
			mVelocityMult = Common._M(0.8f)
		});
		emitter.AddParticleType(particleType);
		particleType = new ParticleType();
		particleType.mAdditive = true;
		particleType.mEmitterAttachPct = 1f;
		particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_FUZZY_CIRCLE);
		particleType.mColorKeyManager.AddColorKey(0f, new Color(1, 255, 6));
		particleType.mColorKeyManager.AddColorKey(0.5f, new Color(255, 233, 1));
		particleType.mColorKeyManager.AddColorKey(1f, new Color(0, 255, 42));
		ParticleSettings particleSettings = new ParticleSettings
		{
			mLife = Common._M(9),
			mNumber = Common._M(40),
			mXSize = Common._M(13),
			mVelocity = Common._M(20),
			mWeight = Common._M(-11),
			mGlobalVisibility = Common._M(0f)
		};
		particleType.AddSettingsKeyFrame(0, particleSettings);
		particleSettings = new ParticleSettings(particleSettings)
		{
			mGlobalVisibility = Common._M(0.3f)
		};
		particleType.AddSettingsKeyFrame(Common._M(200), particleSettings);
		particleType.AddVarianceKeyFrame(0, new ParticleVariance
		{
			mLifeVar = Common._M(39),
			mVelocityVar = Common._M(5)
		});
		lifetimeSettings = new LifetimeSettings
		{
			mSizeXMult = Common._M(0.2f)
		};
		particleType.AddSettingAtLifePct(0f, lifetimeSettings);
		particleType.AddSettingAtLifePct(0.75f, new LifetimeSettings(lifetimeSettings)
		{
			mSizeXMult = Common._M(1.9f)
		});
		particleType.AddSettingAtLifePct(1f, new LifetimeSettings
		{
			mSizeXMult = 0f
		});
		emitter.AddParticleType(particleType);
		emitter.mPreloadFrames = Common._M(100);
		mLeftEyeHandle = mLeftEye.AddEmitter(emitter);
		mRightEyeHandle = mRightEye.AddEmitter(new Emitter(emitter));
		mTrail = new SexyFramework.PIL.System(50, 50);
		mTrail.mScale = Common._S(1f);
		mTrail.WaitForEmitters(w: true);
		emitter = new Emitter();
		emitter.mCullingRect = new Rect(0, 0, Common._SS(GameApp.gApp.mWidth), Common._SS(GameApp.gApp.mHeight));
		emitter.mEmissionCoordsAreOffsets = true;
		emitter.AddScaleKeyFrame(0, new EmitterScale
		{
			mNumberScale = Common._M(0.7f),
			mSizeXScale = Common._M(0.89f)
		});
		particleType = new ParticleType();
		particleType.mImage = (mIsLeft ? Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_SKULL_LEFT_PARTICLE) : Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_SKULL_RIGHT_PARTICLE));
		particleType.mAdditive = true;
		particleType.mEmitterAttachPct = Common._M(0f);
		particleType.mColorKeyManager.AddColorKey(0f, new Color(1, 255, 6));
		particleType.mColorKeyManager.AddColorKey(0.5f, new Color(134, 255, 149));
		particleType.mColorKeyManager.AddColorKey(1f, new Color(7, 255, 49));
		particleType.AddSettingsKeyFrame(0, new ParticleSettings
		{
			mLife = Common._M(10),
			mNumber = Common._M(10),
			mXSize = Common._M(30),
			mGlobalVisibility = Common._M(0.4f)
		});
		particleType.AddSettingAtLifePct(0f, new LifetimeSettings
		{
			mSizeXMult = Common._M(0.4f)
		});
		particleType.AddSettingAtLifePct(0.13f, new LifetimeSettings
		{
			mSizeXMult = Common._M(2f)
		});
		particleType.AddSettingAtLifePct(0.52f, new LifetimeSettings
		{
			mSizeXMult = Common._M(1.2f)
		});
		particleType.AddSettingAtLifePct(1f, new LifetimeSettings
		{
			mSizeXMult = 0f
		});
		mTrailPTHandle = emitter.AddParticleType(particleType);
		mTrailHandle = mTrail.AddEmitter(emitter);
		mFatHead = new SexyFramework.PIL.System(50, 50);
		mFatHead.mScale = Common._S(1f);
		mFatHead.WaitForEmitters(w: true);
		emitter = new Emitter();
		emitter.mCullingRect = new Rect(0, 0, Common._SS(GameApp.gApp.mWidth), Common._SS(GameApp.gApp.mHeight));
		emitter.mEmissionCoordsAreOffsets = true;
		emitter.AddScaleKeyFrame(0, new EmitterScale
		{
			mZoom = Common._M(1.61f)
		});
		EmitterSettings settings = new EmitterSettings();
		emitter.AddSettingsKeyFrame(0, settings);
		settings = new EmitterSettings
		{
			mActive = false
		};
		emitter.AddSettingsKeyFrame(Common._M(99), settings);
		particleType = new ParticleType();
		particleType.mImage = (mIsLeft ? Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_SKULL_LEFT_PARTICLE) : Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_SKULL_RIGHT_PARTICLE));
		particleType.mAdditive = false;
		particleType.mEmitterAttachPct = Common._M(0f);
		particleSettings = new ParticleSettings
		{
			mLife = Common._M(99),
			mNumber = Common._M(17),
			mXSize = Common._M(9)
		};
		particleType.AddSettingsKeyFrame(0, particleSettings);
		particleSettings = new ParticleSettings(particleSettings)
		{
			mXSize = Common._M(150),
			mGlobalVisibility = Common._M(0.16f)
		};
		particleType.AddSettingsKeyFrame(Common._M(70), particleSettings);
		particleSettings = new ParticleSettings(particleSettings)
		{
			mGlobalVisibility = 0f
		};
		particleType.AddSettingsKeyFrame(Common._M(99), particleSettings);
		emitter.mPreloadFrames = Common._M(10);
		emitter.AddParticleType(particleType);
		mFatHead.AddEmitter(emitter);
		mChunks = new SexyFramework.PIL.System(50, 50);
		mChunks.mScale = Common._S(1f);
		mChunks.WaitForEmitters(w: true);
		emitter = new Emitter();
		emitter.mCullingRect = new Rect(0, 0, Common._SS(GameApp.gApp.mWidth), Common._SS(GameApp.gApp.mHeight));
		emitter.mEmissionCoordsAreOffsets = true;
		int frame = Common._M(55);
		EmitterScale emitterScale = new EmitterScale
		{
			mNumberScale = Common._M(0.6f),
			mZoom = Common._M(1.6f)
		};
		emitter.AddScaleKeyFrame(0, emitterScale);
		emitterScale = new EmitterScale(emitterScale)
		{
			mZoom = 1f
		};
		emitter.AddScaleKeyFrame(Common._M(15), emitterScale);
		settings = new EmitterSettings
		{
			mVisibility = 1f
		};
		emitter.AddSettingsKeyFrame(0, settings);
		settings = new EmitterSettings(settings);
		emitter.AddSettingsKeyFrame(Common._M(25), settings);
		emitter.AddSettingsKeyFrame(frame, new EmitterSettings(settings)
		{
			mVisibility = 0f,
			mActive = false
		});
		particleType = new ParticleType();
		particleType.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_CHUNK1);
		particleType.mAdditive = false;
		particleType.AddSettingsKeyFrame(0, new ParticleSettings
		{
			mLife = Common._M(20),
			mNumber = Common._M(15),
			mXSize = Common._M(20),
			mVelocity = Common._M(250),
			mSpin = SexyFramework.Common.DegreesToRadians(Common._M(9))
		});
		particleType.AddVarianceKeyFrame(0, new ParticleVariance
		{
			mSizeXVar = Common._M(11)
		});
		emitter.AddParticleType(particleType);
		mChunks.AddEmitter(emitter);
		emitter = new Emitter(emitter);
		emitter.GetParticleTypeByIndex(0).mImage = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_CHUNK2);
		mChunks.AddEmitter(emitter);
		emitter = new Emitter(emitter);
		emitter.GetParticleTypeByIndex(0).mImage = Res.GetImageByID(ResID.IMAGE_BOSS_TIGER_CHUNK3);
		mChunks.AddEmitter(emitter);
		mClouds = new SexyFramework.PIL.System(50, 50);
		mClouds.mScale = Common._S(1f);
		mClouds.WaitForEmitters(w: true);
		emitter = new Emitter();
		emitter.mCullingRect = new Rect(0, 0, Common._SS(GameApp.gApp.mWidth), Common._SS(GameApp.gApp.mHeight));
		emitter.mEmissionCoordsAreOffsets = true;
		frame = Common._M(70);
		emitter.AddScaleKeyFrame(0, new EmitterScale
		{
			mLifeScale = Common._M(0.8f),
			mNumberScale = Common._M(0.48f),
			mSizeXScale = Common._M(3.7f),
			mVelocityScale = Common._M(0.82f)
		});
		settings = new EmitterSettings
		{
			mVisibility = Common._M(0.5f)
		};
		emitter.AddSettingsKeyFrame(0, settings);
		emitter.AddSettingsKeyFrame(Common._M(50), new EmitterSettings(settings));
		emitter.AddSettingsKeyFrame(frame, new EmitterSettings(settings)
		{
			mVisibility = 0f,
			mActive = false
		});
		particleType = new ParticleType();
		particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_ROUND);
		particleType.mColorKeyManager.SetFixedColor(new Color(65, 70, 49));
		particleType.mAlphaKeyManager.SetFixedColor(new Color(0, 0, 0, Common._M(98)));
		particleType.AddSettingsKeyFrame(0, new ParticleSettings
		{
			mLife = Common._M(59),
			mNumber = Common._M(10),
			mXSize = Common._M(30),
			mVelocity = Common._M(99)
		});
		emitter.AddParticleType(particleType);
		particleType = new ParticleType(particleType);
		particleType.mColorKeyManager.SetFixedColor(new Color(Color.White));
		particleType.mAlphaKeyManager.SetFixedColor(new Color(Color.White));
		emitter.AddParticleType(particleType);
		mClouds.AddEmitter(emitter);
	}

	public virtual void Dispose()
	{
		if (mLeftEye != null)
		{
			mLeftEye.Dispose();
			mLeftEye = null;
		}
		if (mRightEye != null)
		{
			mRightEye.Dispose();
			mRightEye = null;
		}
		if (mTrail != null)
		{
			mTrail.Dispose();
			mTrail = null;
		}
		if (mFatHead != null)
		{
			mFatHead.Dispose();
			mFatHead = null;
		}
		if (mChunks != null)
		{
			mChunks.Dispose();
			mChunks = null;
		}
		if (mClouds != null)
		{
			mClouds.Dispose();
			mClouds = null;
		}
	}

	public void Reset()
	{
		mParticleAlpha = 1f;
		mLaunched = false;
		mUseLastPos = false;
		mLastX = (mLastY = (mLastAngle = 0f));
		mHitPlayer = false;
		mAlpha = 0f;
		if (mLeftEye != null)
		{
			mLeftEye.ResetForReuse();
			mLeftEye.mScale = Common._S(1f);
			mLeftEye.WaitForEmitters(w: true);
		}
		if (mRightEye != null)
		{
			mRightEye.ResetForReuse();
			mRightEye.mScale = Common._S(1f);
			mRightEye.WaitForEmitters(w: true);
		}
		if (mTrail != null)
		{
			mTrail.ResetForReuse();
			mTrail.mScale = Common._S(1f);
			mTrail.WaitForEmitters(w: true);
		}
		if (mFatHead != null)
		{
			mFatHead.ResetForReuse();
			mFatHead.mScale = Common._S(1f);
			mFatHead.WaitForEmitters(w: true);
		}
		if (mChunks != null)
		{
			mChunks.ResetForReuse();
			mChunks.mScale = Common._S(1f);
			mChunks.WaitForEmitters(w: true);
		}
		if (mClouds != null)
		{
			mClouds.ResetForReuse();
			mClouds.mScale = Common._S(1f);
			mClouds.WaitForEmitters(w: true);
		}
	}

	public void Update(float x, float y, float angle)
	{
		if (mAlpha < 255f)
		{
			mAlpha += Common._M(5f);
			if (mAlpha > 255f)
			{
				mAlpha = 255f;
			}
		}
		if (mLaunched)
		{
			if (!mUseLastPos)
			{
				mLastX = x;
				mLastY = y;
				mLastAngle = angle;
			}
			else
			{
				x = mLastX;
				y = mLastY;
				angle = mLastAngle;
			}
			float mInitAngle = 0f - Common.GetCanonicalAngleRad(angle);
			for (int i = 0; i < mLeftEye.GetEmitter(mLeftEyeHandle).GetNumParticleTypes(); i++)
			{
				mLeftEye.GetEmitter(mLeftEyeHandle).GetParticleTypeByIndex(i).mInitAngle = mInitAngle;
			}
			for (int j = 0; j < mRightEye.GetEmitter(mRightEyeHandle).GetNumParticleTypes(); j++)
			{
				mRightEye.GetEmitter(mRightEyeHandle).GetParticleTypeByIndex(j).mInitAngle = mInitAngle;
			}
			float num = (float)Common._M(11) * (float)Math.Cos(angle);
			float num2 = (float)Common._M(4) * (0f - (float)Math.Sin(angle));
			mLeftEye.SetPos(x - num, y - num2);
			mLeftEye.Update();
			num = (float)Common._M(10) * (float)Math.Cos(angle);
			num2 = (float)Common._M(16) * (0f - (float)Math.Sin(angle));
			mRightEye.SetPos(x + num, y + num2);
			mRightEye.Update();
			mTrail.GetEmitter(mTrailHandle).GetParticleType(mTrailPTHandle).mInitAngle = mInitAngle;
			mTrail.SetPos(x, y);
			mTrail.Update();
			if (mHitPlayer)
			{
				Gun gun = GameApp.gApp.GetBoard().GetGun();
				mFatHead.SetPos(gun.GetCenterX() + Common._M(0), gun.GetCenterY() + Common._M1(0));
				mFatHead.Update();
				mChunks.SetPos(gun.GetCenterX() + Common._M(0), gun.GetCenterY() + Common._M1(0));
				mClouds.SetPos(gun.GetCenterX() + Common._M(0), gun.GetCenterY() + Common._M1(0));
				mChunks.Update();
				mClouds.Update();
			}
		}
	}

	public void Draw(Graphics g, float x, float y, float angle)
	{
		g.PushState();
		mTrail.mAlphaPct = mParticleAlpha;
		mLeftEye.mAlphaPct = mParticleAlpha;
		mRightEye.mAlphaPct = mParticleAlpha;
		mFatHead.mAlphaPct = mParticleAlpha;
		mClouds.mAlphaPct = mParticleAlpha;
		mChunks.mAlphaPct = mParticleAlpha;
		if (mLaunched)
		{
			mTrail.Draw(g);
		}
		if (!mUseLastPos)
		{
			if (mAlpha < 255f)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)mAlpha);
			}
			g.DrawImageRotated(mImage, (int)x, (int)y, angle);
			g.SetColorizeImages(colorizeImages: false);
		}
		if (mLaunched)
		{
			mLeftEye.Draw(g);
			mRightEye.Draw(g);
		}
		if (mHitPlayer)
		{
			mFatHead.Draw(g);
			mClouds.Draw(g);
			mChunks.Draw(g);
		}
		g.PopState();
	}
}
