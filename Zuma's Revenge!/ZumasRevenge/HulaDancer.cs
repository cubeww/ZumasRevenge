using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.PIL;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class HulaDancer
{
	protected bool mHasProjectile;

	protected bool mFireProjectile;

	protected bool mProjectileDestroyed;

	protected float mX;

	protected float mY;

	protected float mProjX;

	protected float mProjY;

	protected float mProjVY;

	protected float mFadeAlpha;

	protected int mCel;

	protected int mUpdateCount;

	protected Rect mRect = default(Rect);

	protected Image mImage;

	protected SexyFramework.PIL.System mSystem;

	protected PopAnim mDeathEffect;

	protected Transform mGlobalTranform = new Transform();

	public bool mFadeOut;

	public HulaDancer()
	{
		mHasProjectile = false;
		mX = 0f;
		mY = 0f;
		mProjX = 0f;
		mProjY = 0f;
		mProjVY = 0f;
		mFireProjectile = false;
		mProjectileDestroyed = false;
		mCel = 0;
		mUpdateCount = 0;
		mSystem = null;
		mFadeOut = false;
		mFadeAlpha = 255f;
		mDeathEffect = null;
	}

	public HulaDancer(HulaDancer rhs)
	{
		mHasProjectile = rhs.mHasProjectile;
		mX = rhs.mX;
		mY = rhs.mY;
		mProjX = rhs.mProjX;
		mProjY = rhs.mProjY;
		mProjVY = rhs.mProjVY;
		mFireProjectile = rhs.mFireProjectile;
		mProjectileDestroyed = rhs.mProjectileDestroyed;
		mCel = rhs.mCel;
		mUpdateCount = rhs.mUpdateCount;
		mSystem = rhs.mSystem;
		mFadeOut = rhs.mFadeOut;
		mFadeAlpha = rhs.mFadeAlpha;
		mDeathEffect = rhs.mDeathEffect;
		mRect = new Rect(rhs.mRect);
		mImage = rhs.mImage;
	}

	public virtual void Dispose()
	{
		if (mSystem != null)
		{
			mSystem.Dispose();
			mSystem = null;
		}
	}

	public void Setup(bool has_proj, float y, float proj_vy)
	{
		mHasProjectile = has_proj;
		if (mHasProjectile)
		{
			mCel = 3;
		}
		mY = y;
		mProjVY = proj_vy;
		mImage = Res.GetImageByID(ResID.IMAGE_BOSS_STONEHEAD_HULA_GIRL);
		mRect = new Rect((int)mX, (int)mY, Common._SS(mImage.GetCelWidth()), Common._SS(mImage.GetCelHeight()));
		mRect.Inflate(Common._M(-5), Common._M1(-5));
		mX = -mImage.GetCelWidth();
		mSystem = new SexyFramework.PIL.System(100, 50);
		mSystem.mScale = Common._S(1f);
		mSystem.WaitForEmitters(w: true);
		Emitter emitter = new Emitter();
		emitter.mPreloadFrames = Common._M(200);
		emitter.mCullingRect = new Rect(-100, -100, Common._SS(GameApp.gApp.mWidth) + 200, Common._SS(GameApp.gApp.mHeight) + 200);
		emitter.mEmissionCoordsAreOffsets = true;
		emitter.SetEmitterType(2);
		EmitterScale emitterScale = new EmitterScale();
		emitterScale.mLifeScale = Common._M(1f);
		emitterScale.mNumberScale = Common._M(1f);
		emitterScale.mSizeXScale = Common._M(1.27f);
		emitterScale.mSizeYScale = Common._M(0.9f);
		emitterScale.mVelocityScale = Common._M(1f);
		emitterScale.mWeightScale = Common._M(0.48f);
		emitterScale.mZoom = Common._M(1.66f);
		emitter.AddScaleKeyFrame(0, emitterScale);
		EmitterSettings emitterSettings = new EmitterSettings();
		emitterSettings.mVisibility = Common._M(0.51f);
		emitterSettings.mXRadius = Common._M(10);
		emitterSettings.mYRadius = Common._M(12);
		emitter.AddSettingsKeyFrame(0, emitterSettings);
		ParticleType particleType = new ParticleType();
		particleType.mLockSizeAspect = false;
		particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_FUZZY_CIRCLE);
		particleType.mColorKeyManager.AddColorKey(0f, new Color(16, 255, 0));
		particleType.mColorKeyManager.AddColorKey(0.5f, new Color(255, 233, 0));
		particleType.mColorKeyManager.AddColorKey(1f, new Color(0, 255, 42));
		particleType.mAdditive = true;
		particleType.mEmitterAttachPct = Common._M(1f);
		ParticleSettings particleSettings = new ParticleSettings();
		particleSettings.mLife = Common._M(10);
		particleSettings.mNumber = Common._M(40);
		int mXSize = (particleSettings.mYSize = Common._M(13));
		particleSettings.mXSize = mXSize;
		particleSettings.mVelocity = Common._M(5);
		particleSettings.mWeight = Common._M(0);
		particleSettings.mGlobalVisibility = Common._M(1f);
		particleType.AddSettingsKeyFrame(0, particleSettings);
		ParticleVariance particleVariance = new ParticleVariance();
		particleVariance.mLifeVar = Common._M(5);
		particleVariance.mNumberVar = Common._M(2);
		int mSizeXVar = (particleVariance.mSizeYVar = Common._M(3));
		particleVariance.mSizeXVar = mSizeXVar;
		particleVariance.mVelocityVar = Common._M(5);
		particleType.AddVarianceKeyFrame(0, particleVariance);
		LifetimeSettings lifetimeSettings = new LifetimeSettings();
		lifetimeSettings.mSizeXMult = (lifetimeSettings.mSizeYMult = Common._M(0.2f));
		particleType.AddSettingAtLifePct(0f, lifetimeSettings);
		lifetimeSettings = new LifetimeSettings(lifetimeSettings);
		lifetimeSettings.mSizeXMult = (lifetimeSettings.mSizeYMult = Common._M(1.9f));
		particleType.AddSettingAtLifePct(0.75f, lifetimeSettings);
		lifetimeSettings = new LifetimeSettings(lifetimeSettings);
		lifetimeSettings.mSizeXMult = (lifetimeSettings.mSizeYMult = 0f);
		particleType.AddSettingAtLifePct(1f, lifetimeSettings);
		emitter.AddParticleType(particleType);
		mSystem.AddEmitter(emitter);
	}

	public void Setup(bool has_proj, float y)
	{
		Setup(has_proj, y, 0f);
	}

	public bool CanRemove()
	{
		if (mDeathEffect != null)
		{
			return false;
		}
		if (mFadeAlpha <= 0f)
		{
			return true;
		}
		if ((!mHasProjectile && !mFireProjectile) || mProjectileDestroyed)
		{
			return mX > (float)(Common._SS(GameApp.gApp.mWidth) + 80);
		}
		if (mX > (float)(Common._SS(GameApp.gApp.mWidth) + 80))
		{
			return mProjY > (float)(Common._SS(GameApp.gApp.mHeight) + Common._M(100));
		}
		return false;
	}

	public void Update(float vx)
	{
		if (mFadeOut)
		{
			mFadeAlpha -= Common._M(2f);
		}
		mUpdateCount++;
		if (mFireProjectile && !mProjectileDestroyed && !mFadeOut)
		{
			mProjY += mProjVY;
		}
		if (mSystem != null)
		{
			mSystem.Update();
			if (mFireProjectile)
			{
				mSystem.SetPos(mProjX + (float)Common._M(10), mProjY + (float)Common._M1(10));
			}
			else
			{
				mSystem.SetPos(mX + (float)Common._M(50), mY + (float)Common._M1(10));
			}
		}
		if (mUpdateCount % Common._M(5) == 0)
		{
			if (!mHasProjectile)
			{
				mCel = (mCel + 1) % 3;
			}
			else if (++mCel >= mImage.mNumCols)
			{
				mCel = 3;
			}
		}
		if (mDeathEffect != null)
		{
			mGlobalTranform.Reset();
			mGlobalTranform.Translate(Common._S(mX + (float)Common._M(0)), Common._S(mY + (float)Common._M1(0)));
			mDeathEffect.SetTransform(mGlobalTranform.GetMatrix());
			mDeathEffect.Update();
			if (!mDeathEffect.IsActive())
			{
				mDeathEffect = null;
				mX = 9999999f;
			}
		}
		mX += vx;
		mRect.mX = (int)mX;
	}

	public void Draw(Graphics g)
	{
		if (mFadeAlpha <= 0f)
		{
			return;
		}
		if (mFadeAlpha != 255f)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)mFadeAlpha);
		}
		if (g.Is3D())
		{
			if (mDeathEffect == null)
			{
				g.DrawImageCel(mImage, (int)Common._S(mX), (int)Common._S(mY), mCel);
			}
			else
			{
				mDeathEffect.Draw(g);
			}
			if (mFireProjectile && !mProjectileDestroyed && !GameApp.gApp.GetBoard().IsPaused())
			{
				g.DrawImageF(Res.GetImageByID(ResID.IMAGE_BOSS_STONEHEAD_COCONUT), (int)Common._S(mProjX), (int)Common._S(mProjY));
			}
		}
		else
		{
			if (mDeathEffect == null)
			{
				g.DrawImageCel(mImage, (int)Common._S(mX), (int)Common._S(mY), mCel);
			}
			else
			{
				mDeathEffect.Draw(g);
			}
			if (mFireProjectile && !mProjectileDestroyed && !GameApp.gApp.GetBoard().IsPaused())
			{
				g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_STONEHEAD_COCONUT), (int)Common._S(mProjX), (int)Common._S(mProjY));
			}
		}
		if (mSystem != null && !GameApp.gApp.GetBoard().IsPaused())
		{
			mSystem.mAlphaPct = mFadeAlpha / 255f;
			mSystem.Draw(g);
		}
		g.SetColorizeImages(colorizeImages: false);
	}

	public bool Collided(Rect r)
	{
		if (r.Intersects(mRect) && !mFadeOut)
		{
			return mDeathEffect == null;
		}
		return false;
	}

	public bool ProjectileCollided(Rect gun_rect)
	{
		if (!mFireProjectile || mProjectileDestroyed)
		{
			return false;
		}
		Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_STONEHEAD_COCONUT);
		bool flag = gun_rect.Intersects(new Rect((int)mProjX, (int)mProjY, Common._SS(imageByID.mWidth), Common._SS(imageByID.mHeight)));
		if (flag && mSystem != null)
		{
			mSystem.ForceStopEmitting(f: true);
		}
		return flag;
	}

	public bool HasFired()
	{
		return mFireProjectile;
	}

	public void Fire()
	{
		if (!mFireProjectile && mHasProjectile && !mProjectileDestroyed && mDeathEffect == null)
		{
			GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BOSS_STONE_COCONUT_DROP));
			mHasProjectile = false;
			mCel -= 3;
			mFireProjectile = true;
			mProjX = mX + (float)Common._M(40);
			mProjY = mY + (float)Common._M(0);
		}
	}

	public void SyncState(DataSync sync)
	{
		sync.SyncBoolean(ref mFadeOut);
		sync.SyncFloat(ref mFadeAlpha);
		sync.SyncFloat(ref mX);
		sync.SyncFloat(ref mY);
		sync.SyncFloat(ref mProjX);
		sync.SyncFloat(ref mProjY);
		sync.SyncFloat(ref mProjVY);
		sync.SyncBoolean(ref mFireProjectile);
		sync.SyncBoolean(ref mHasProjectile);
		sync.SyncBoolean(ref mProjectileDestroyed);
		sync.SyncLong(ref mCel);
		sync.SyncLong(ref mUpdateCount);
		if (sync.isRead())
		{
			mImage = Res.GetImageByID(ResID.IMAGE_BOSS_STONEHEAD_HULA_GIRL);
			mRect = new Rect((int)mX, (int)mY, mImage.GetCelWidth(), mImage.GetCelWidth());
			mRect.Inflate(Common._M(-5), Common._M1(-5));
		}
		Buffer buffer = sync.GetBuffer();
		if (sync.isWrite())
		{
			Common.SerializeParticleSystem(mSystem, sync);
			buffer.WriteBoolean(mDeathEffect != null);
			if (mDeathEffect != null)
			{
				buffer.WriteLong((int)mDeathEffect.mMainSpriteInst.mFrameNum);
			}
			return;
		}
		mSystem = Common.DeserializeParticleSystem(sync);
		if (buffer.ReadBoolean())
		{
			mDeathEffect = Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_HULAGIRLDEATH);
			mDeathEffect.ResetAnim();
			mDeathEffect.Play((int)buffer.ReadLong());
		}
	}

	public void Disable()
	{
		if (mDeathEffect == null)
		{
			mDeathEffect = Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_HULAGIRLDEATH);
			mDeathEffect.ResetAnim();
			mDeathEffect.Play("Main");
			if (!mFireProjectile)
			{
				mFadeOut = true;
			}
		}
	}

	public float GetX()
	{
		return mX;
	}

	public void DestroyBullet()
	{
		mProjectileDestroyed = true;
	}
}
