using System;
using System.Collections.Generic;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class BossDarkFrog : BossShoot
{
	private enum FireState
	{
		FiringState_NULL,
		FiringState_ShowTongue,
		FiringState_Launching,
		FiringState_HideTongue
	}

	private static int CANVAS_W = 293;

	private static int CANVAS_H = 268;

	protected List<FXCache> mFXCache = new List<FXCache>();

	protected List<DarkFrogBulletFX> mBulletFX = new List<DarkFrogBulletFX>();

	protected PIEffect mDeathAura;

	protected PopAnim mHitAnim;

	protected int mTimer;

	protected int mFiringState;

	protected int mBlinkFrame = -1;

	protected float mTongueYOff;

	protected bool mBlinkForward = true;

	protected float mAlpha;

	protected float mRecoilY;

	protected float mBodyAlpha = 255f;

	protected float mTattooAlpha = 255f;

	private static bool AnimPlaying(PopAnim p)
	{
		if (p.mMainSpriteInst.mDef == null)
		{
			return false;
		}
		return p.mMainSpriteInst.mFrameNum < (float)(p.mMainSpriteInst.mDef.mFrames.Count - 1);
	}

	protected override void DidFire()
	{
		mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BOSS_DARK_FROG_FIRES));
	}

	protected override bool DoHit(Bullet b, bool from_prox_bomb)
	{
		bool flag = base.DoHit(b, from_prox_bomb);
		if (flag)
		{
			mHitAnim.ResetAnim();
			mHitAnim.Play("MAIN");
			mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BOSS_DARK_FROG_HIT));
			if (mHP <= 0f)
			{
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BOSS_DARK_FROG_DIES));
				mLevel.mFrog.ClearStun();
			}
		}
		return flag;
	}

	protected override void DrawBossSpecificArt(Graphics g)
	{
		if (g.Is3D())
		{
			mDeathAura.mColor.mAlpha = (int)mAlphaOverride;
			mDeathAura.DrawLayer(g, mDeathAura.GetLayer("lower"));
		}
		int num = (int)mX * 2 - CANVAS_W / 2;
		int num2 = (int)mY * 2 - CANVAS_H / 2;
		int num3 = (int)Math.Min(mBodyAlpha, mAlphaOverride);
		g.PushState();
		g.SetColor(255, 255, 255, num3);
		if (num3 < 255)
		{
			g.SetColorizeImages(colorizeImages: true);
		}
		if (!AnimPlaying(mHitAnim))
		{
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_BACK), Common._DS(num + Res.GetOffsetXByID(ResID.IMAGE_BOSS_DARKFROG_BACK)), (int)(Common._S(mRecoilY) + (float)Common._DS(num2 + Res.GetOffsetYByID(ResID.IMAGE_BOSS_DARKFROG_BACK))));
		}
		g.PopState();
		if (mAlpha > 0f && !AnimPlaying(mHitAnim))
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)Math.Min(mAlpha, mAlphaOverride));
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_TAT2), Common._DS(num + Res.GetOffsetXByID(ResID.IMAGE_BOSS_DARKFROG_TAT2)), (int)((float)Common._DS(num2 + Res.GetOffsetYByID(ResID.IMAGE_BOSS_DARKFROG_TAT2)) + Common._S(mRecoilY)));
			g.SetColorizeImages(colorizeImages: false);
		}
		g.PushState();
		g.SetColor(255, 255, 255, num3);
		if (num3 < 255)
		{
			g.SetColorizeImages(colorizeImages: true);
		}
		if (!AnimPlaying(mHitAnim))
		{
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_TONGUE), Common._DS(num + Res.GetOffsetXByID(ResID.IMAGE_BOSS_DARKFROG_TONGUE)), (int)((float)Common._DS(num2 + Res.GetOffsetYByID(ResID.IMAGE_BOSS_DARKFROG_TONGUE)) + Common._S(mTongueYOff + mRecoilY)));
		}
		g.PopState();
		if (mHP > 0f && !mDoDeathExplosions && !mLevel.mBoard.IsPaused())
		{
			Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_LB_HALO);
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_LB_TWIRLLIGHT);
			Image imageByID3 = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_LAVABALL_BOTTOM_ADDITIVE);
			Image imageByID4 = Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_LAVABALL_TOP_NORMAL);
			for (int i = 0; i < mBulletFX.Count; i++)
			{
				DarkFrogBulletFX darkFrogBulletFX = mBulletFX[i];
				if (!darkFrogBulletFX.mExploding)
				{
					g.PushState();
					darkFrogBulletFX.mBallEffect.mColor.mAlpha = (int)mAlphaOverride;
					darkFrogBulletFX.mBallEffect.Draw(g);
					g.PopState();
					g.PushState();
					if (!SexyFramework.Common._eq(mAlphaOverride, 255f))
					{
						g.SetColorizeImages(colorizeImages: true);
						g.SetColor(255, 255, 255, (int)mAlphaOverride);
					}
					g.SetDrawMode(1);
					g.DrawImage(imageByID, (int)(darkFrogBulletFX.mX - (float)(imageByID.mWidth / 2)), (int)(darkFrogBulletFX.mY - (float)(imageByID.mHeight / 2)));
					g.DrawImageRotated(imageByID2, (int)(darkFrogBulletFX.mX - (float)(imageByID2.mWidth / 2)), (int)(darkFrogBulletFX.mY - (float)(imageByID2.mHeight / 2)), darkFrogBulletFX.mTwirlAngle);
					g.DrawImage(imageByID3, (int)(darkFrogBulletFX.mX - (float)(imageByID3.mWidth / 2)), (int)(darkFrogBulletFX.mY - (float)(imageByID3.mHeight / 2)));
					g.SetDrawMode(0);
					g.DrawImage(imageByID4, (int)(darkFrogBulletFX.mX - (float)(imageByID4.mWidth / 2)), (int)(darkFrogBulletFX.mY - (float)(imageByID4.mHeight / 2)));
					g.PopState();
				}
				if (darkFrogBulletFX.mExploding)
				{
					g.PushState();
					darkFrogBulletFX.mBallExplosion.mColor.mAlpha = (int)mAlphaOverride;
					darkFrogBulletFX.mBallExplosion.Draw(g);
					g.PopState();
				}
			}
		}
		g.PushState();
		g.SetColor(255, 255, 255, num3);
		if (num3 < 255)
		{
			g.SetColorizeImages(colorizeImages: true);
		}
		if (!AnimPlaying(mHitAnim))
		{
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_TOP), Common._DS(num + Res.GetOffsetXByID(ResID.IMAGE_BOSS_DARKFROG_TOP)), (int)(Common._S(mRecoilY) + (float)Common._DS(num2 + Res.GetOffsetYByID(ResID.IMAGE_BOSS_DARKFROG_TOP))));
		}
		if (mBlinkFrame != -1 && !AnimPlaying(mHitAnim))
		{
			ResID id = ((mBlinkFrame == 0) ? ResID.IMAGE_BOSS_DARKFROG_BLINK2 : ResID.IMAGE_BOSS_DARKFROG_BLINK1);
			Image imageByID5 = Res.GetImageByID(id);
			g.DrawImage(imageByID5, Common._DS(num + Res.GetOffsetXByID(id)), (int)((float)Common._DS(num2 + Res.GetOffsetYByID(id)) + Common._S(mRecoilY)));
		}
		g.PopState();
		if (mAlpha > 0f && !AnimPlaying(mHitAnim))
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)Math.Min(mAlpha, mAlphaOverride));
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_DARKFROG_TAT1), Common._DS(num + Res.GetOffsetXByID(ResID.IMAGE_BOSS_DARKFROG_TAT1)), (int)((float)Common._DS(num2 + Res.GetOffsetYByID(ResID.IMAGE_BOSS_DARKFROG_TAT1)) + Common._S(mRecoilY)));
			g.SetColorizeImages(colorizeImages: false);
		}
		if (AnimPlaying(mHitAnim))
		{
			mHitAnim.mColor.mAlpha = (int)mAlphaOverride;
			mHitAnim.Draw(g);
		}
		if (g.Is3D())
		{
			mDeathAura.mColor.mAlpha = (int)mAlphaOverride;
			mDeathAura.DrawLayer(g, mDeathAura.GetLayer("upper"));
		}
	}

	protected override bool PreBulletUpdate(BossBullet b, int index)
	{
		if (mFiringState != 0 && b.mState < 2)
		{
			return true;
		}
		if (b.mDelay > 0 && b.mState == 0)
		{
			b.mDelay--;
			return true;
		}
		if (b.mState == 0)
		{
			b.mState = 1;
			mFiringState = 1;
			mTongueYOff = 0f;
			mTimer = 0;
			b.mX = (b.mY = 0f);
		}
		if (b.mState < 2)
		{
			return true;
		}
		return false;
	}

	protected override void BulletHitPlayer(BossBullet b)
	{
		SoundAttribs soundAttribs = new SoundAttribs();
		soundAttribs.fadeout = 0.1f;
		mApp.mSoundPlayer.Loop(Res.GetSoundByID(ResID.SOUND_NEW_BURNINGFROGLOOP), soundAttribs);
		mApp.mSoundPlayer.Play(Res.GetSoundByID(ResID.SOUND_NEW_FIREHITFROG));
		for (int i = 0; i < mBulletFX.Count; i++)
		{
			DarkFrogBulletFX darkFrogBulletFX = mBulletFX[i];
			if (darkFrogBulletFX.mBulletId == b.mId)
			{
				darkFrogBulletFX.mExploding = false;
				darkFrogBulletFX.mBallExplosion = GetFreeBallExplosion(mApp.Is3DAccelerated());
				darkFrogBulletFX.mBallExplosion.mDrawTransform.LoadIdentity();
				float num = GameApp.DownScaleNum(1f);
				darkFrogBulletFX.mBallExplosion.mDrawTransform.Scale(num, num);
				darkFrogBulletFX.mBallExplosion.mDrawTransform.Translate(Common._S(mLevel.mBoard.GetGun().GetCenterX() + Common._M(0)), Common._S(mLevel.mBoard.GetGun().GetCenterY() + Common._M1(0)));
			}
		}
		if (mApp.GetLevelMgr().mBossesCanAttackFuckedFrog)
		{
			return;
		}
		for (int j = 0; j < mBullets.Count; j++)
		{
			BossBullet bossBullet = mBullets[j];
			if (bossBullet.mState < 2)
			{
				bossBullet.mDeleteInstantly = true;
			}
		}
	}

	protected override void BossBulletDestroyed(BossBullet b, bool outofscreen)
	{
		for (int i = 0; i < mBulletFX.Count; i++)
		{
			DarkFrogBulletFX darkFrogBulletFX = mBulletFX[i];
			if (darkFrogBulletFX.mBulletId == b.mId && !darkFrogBulletFX.mExploding)
			{
				ReleaseEffects(darkFrogBulletFX);
				mBulletFX.RemoveAt(i);
				break;
			}
		}
	}

	protected PIEffect GetFreeBallEffect(bool particle_3d)
	{
		for (int i = 0; i < mFXCache.Count; i++)
		{
			FXCache fXCache = mFXCache[i];
			if (!fXCache.mBallEffectInUse)
			{
				fXCache.mBallEffectInUse = true;
				if (!particle_3d)
				{
					return fXCache.mBallEffect2D;
				}
				return fXCache.mBallEffect;
			}
		}
		return null;
	}

	protected PIEffect GetFreeBallExplosion(bool particle_3d)
	{
		for (int i = 0; i < mFXCache.Count; i++)
		{
			FXCache fXCache = mFXCache[i];
			if (!fXCache.mBallExplosionInUse)
			{
				fXCache.mBallExplosionInUse = true;
				if (!particle_3d)
				{
					return fXCache.mBallExplosion2D;
				}
				return fXCache.mBallExplosion;
			}
		}
		return null;
	}

	protected void ReleaseEffects(DarkFrogBulletFX fx)
	{
		for (int i = 0; i < mFXCache.Count; i++)
		{
			FXCache fXCache = mFXCache[i];
			if (fx.mBallEffect == fXCache.mBallEffect || fx.mBallEffect == fXCache.mBallEffect2D)
			{
				fx.mBallEffect = null;
				fXCache.mBallEffectInUse = false;
				fXCache.mBallEffect.ResetAnim();
			}
			if (fx.mBallExplosion == fXCache.mBallExplosion || fx.mBallExplosion == fXCache.mBallExplosion2D)
			{
				fx.mBallExplosion = null;
				fXCache.mBallExplosionInUse = false;
				fXCache.mBallExplosion.ResetAnim();
			}
			if (fx.mBallExplosion == null && fx.mBallEffect == null)
			{
				break;
			}
		}
	}

	public BossDarkFrog(Level l)
		: base(l)
	{
		mDrawHeartsBelowBoss = true;
		mBossRadius = Common._M(55);
		mBulletRadius = Common._M(10);
		mResGroup = "Boss6_DarkFrog";
		mTongueYOff = Common._M(0);
	}

	public BossDarkFrog()
		: this(null)
	{
	}

	public override void Dispose()
	{
		base.Dispose();
		for (int i = 0; i < mFXCache.Count; i++)
		{
			if (mFXCache[i].mBallEffect != null)
			{
				mFXCache[i].mBallEffect.Dispose();
				mFXCache[i].mBallEffect = null;
			}
			if (mFXCache[i].mBallExplosion != null)
			{
				mFXCache[i].mBallExplosion.Dispose();
				mFXCache[i].mBallExplosion = null;
			}
			if (mFXCache[i].mBallEffect2D != null)
			{
				mFXCache[i].mBallEffect2D.Dispose();
				mFXCache[i].mBallEffect2D = null;
			}
			if (mFXCache[i].mBallExplosion2D != null)
			{
				mFXCache[i].mBallExplosion2D.Dispose();
				mFXCache[i].mBallExplosion2D = null;
			}
		}
		mFXCache.Clear();
	}

	public void CopyFrom(BossDarkFrog rhs)
	{
		CopyFrom((BossShoot)rhs);
		mDeathAura = rhs.mDeathAura;
		mHitAnim = rhs.mHitAnim;
		mTimer = rhs.mTimer;
		mFiringState = rhs.mFiringState;
		mBlinkFrame = rhs.mBlinkFrame;
		mTongueYOff = rhs.mTongueYOff;
		mBlinkForward = rhs.mBlinkForward;
		mAlpha = rhs.mAlpha;
		mRecoilY = rhs.mRecoilY;
		mBodyAlpha = rhs.mBodyAlpha;
		mTattooAlpha = rhs.mTattooAlpha;
	}

	public override void Update(float f)
	{
		base.Update(f);
		if (mHP <= 0f && !mDoDeathExplosions && mDeathExplosions.Count == 0 && mBodyAlpha > 0f)
		{
			mBodyAlpha -= Common._M(1.5f);
			if (mBodyAlpha < 0f)
			{
				mBodyAlpha = 0f;
			}
			mAlpha = 255f - mBodyAlpha;
		}
		if (mHP <= 0f)
		{
			mAlphaOverride = 255f;
		}
		if (AnimPlaying(mHitAnim))
		{
			mGlobalTranform.Reset();
			mGlobalTranform.Translate(Common._S(mX + (float)Common._M(-185)), Common._S(mY + (float)Common._M(-160)));
			mHitAnim.SetTransform(mGlobalTranform.GetMatrix());
			mHitAnim.Update();
		}
		mDeathAura.mDrawTransform.LoadIdentity();
		float num = GameApp.DownScaleNum(1f);
		mDeathAura.mDrawTransform.Scale(num, num);
		mDeathAura.mDrawTransform.Translate(Common._S(mX + (float)Common._M(0)), Common._S(mY + (float)Common._M1(0)));
		if (!mLevel.mBoard.HasDarkFrogSequence())
		{
			mDeathAura.Update();
		}
		for (int i = 0; i < mBulletFX.Count; i++)
		{
			PIEffect pIEffect = (mBulletFX[i].mExploding ? mBulletFX[i].mBallExplosion : mBulletFX[i].mBallEffect);
			pIEffect.Update();
			if (mBulletFX[i].mBulletId != -1 && !mBulletFX[i].mExploding)
			{
				for (int j = 0; j < mBullets.Count; j++)
				{
					if (mBullets[j].mId == mBulletFX[i].mBulletId)
					{
						mBulletFX[i].mX = Common._S(mBullets[j].mX + (float)Common._M(0));
						mBulletFX[i].mY = Common._S(mBullets[j].mY + (float)Common._M(0));
						pIEffect.mDrawTransform.LoadIdentity();
						float num2 = GameApp.DownScaleNum(1f);
						pIEffect.mDrawTransform.Scale(num2, num2);
						pIEffect.mDrawTransform.Translate(Common._S(mBullets[j].mX + (float)Common._M(0)), Common._S(mBullets[j].mY + (float)Common._M1(0)));
						break;
					}
				}
			}
			else if (mBulletFX[i].mExploding)
			{
				pIEffect.mDrawTransform.LoadIdentity();
				float num3 = GameApp.DownScaleNum(1f);
				pIEffect.mDrawTransform.Scale(num3, num3);
				pIEffect.mDrawTransform.Translate(Common._S(mLevel.mBoard.GetGun().GetCenterX() + Common._M(0)), Common._S(mLevel.mBoard.GetGun().GetCenterY() + Common._M1(0)));
			}
			mBulletFX[i].mTwirlAngle += Common._M(0.1f);
			if ((mBulletFX[i].mExploding && pIEffect.mFrameNum >= (float)pIEffect.mLastFrameNum) || mBulletFX[i].mY > (float)(mApp.mHeight + 150))
			{
				ReleaseEffects(mBulletFX[i]);
				mBulletFX.RemoveAt(i);
				i--;
			}
		}
		int num4 = Common._M(14);
		int num5 = (int)Common._M(10f);
		if (mFiringState == 1)
		{
			if (mAlpha < 255f && mHP > 0f)
			{
				mAlpha = Math.Min(mAlpha + (float)num5, 255f);
			}
			mTongueYOff += Common._M(4f);
			if (mTongueYOff >= (float)num4)
			{
				mTongueYOff = num4;
				mFiringState = 2;
				for (int k = 0; k < mBullets.Count; k++)
				{
					BossBullet bossBullet = mBullets[k];
					if (bossBullet.mState == 1)
					{
						bossBullet.mState = 2;
						bossBullet.mX = mX + (float)Common._S(Common._M(0));
						bossBullet.mY = mY + (float)Common._S(Common._M(18));
						DarkFrogBulletFX darkFrogBulletFX = new DarkFrogBulletFX();
						darkFrogBulletFX.mBallEffect = GetFreeBallEffect(mApp.Is3DAccelerated());
						darkFrogBulletFX.mBulletId = bossBullet.mId;
						darkFrogBulletFX.mX = Common._S(bossBullet.mX);
						darkFrogBulletFX.mY = Common._S(bossBullet.mY);
						mBulletFX.Add(darkFrogBulletFX);
					}
				}
			}
		}
		else if (mFiringState == 2 && ++mTimer >= Common._M(50))
		{
			if (mAlpha < 255f && mHP > 0f)
			{
				mAlpha = Math.Min(mAlpha + (float)num5, 255f);
			}
			mFiringState = 3;
			mTimer = 0;
		}
		else if (mFiringState == 3)
		{
			mTongueYOff -= Common._M(4f);
			if (mTongueYOff <= 0f)
			{
				mTongueYOff = 0f;
				mFiringState = 0;
			}
		}
		if ((mFiringState == 3 || mFiringState == 0) && mAlpha > 0f && mHP > 0f)
		{
			mAlpha = Math.Max(mAlpha - (float)num5, 0f);
		}
		if (mBlinkFrame == -1 && mUpdateCount % Common._M(500) == 0)
		{
			mBlinkFrame = 0;
			mBlinkForward = true;
		}
		if (mBlinkFrame != -1 && mUpdateCount % Common._M(10) == 0)
		{
			if (mBlinkForward && ++mBlinkFrame >= 2)
			{
				mBlinkFrame = 1;
				mBlinkForward = false;
			}
			else if (!mBlinkForward)
			{
				mBlinkFrame--;
			}
		}
	}

	public override void Init(Level l)
	{
		for (int i = 0; i < 5; i++)
		{
			FXCache fXCache = new FXCache();
			fXCache.mBallEffect = mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_LB_PARTICLES").Duplicate();
			fXCache.mBallEffect.mEmitAfterTimeline = true;
			fXCache.mBallExplosion = mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_LB_EXPLOSION").Duplicate();
			fXCache.mBallEffect2D = mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_LB_PARTICLES_2D").Duplicate();
			fXCache.mBallEffect2D.mEmitAfterTimeline = true;
			fXCache.mBallExplosion2D = mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_LB_EXPLOSION_2D").Duplicate();
			mFXCache.Add(fXCache);
		}
		mWidth = Common._M(110);
		mHeight = Common._M(120);
		base.Init(l);
		mHitAnim = Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_SHADOW_DAMAGE);
		mDeathAura = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_DEATH_AURA);
		mDeathAura.ResetAnim();
		mDeathAura.mEmitAfterTimeline = true;
		Common.SetFXNumScale(mDeathAura, mApp.Is3DAccelerated() ? 1f : Common._M(0.25f));
	}

	public override Boss Instantiate()
	{
		BossDarkFrog bossDarkFrog = new BossDarkFrog(mLevel);
		bossDarkFrog.CopyFrom(this);
		return bossDarkFrog;
	}

	public override void SyncState(DataSync sync)
	{
		base.SyncState(sync);
		sync.SyncLong(ref mFiringState);
		sync.SyncLong(ref mBlinkFrame);
		sync.SyncBoolean(ref mBlinkForward);
		sync.SyncFloat(ref mAlpha);
		sync.SyncFloat(ref mRecoilY);
		sync.SyncFloat(ref mTongueYOff);
		sync.SyncFloat(ref mTattooAlpha);
		sync.SyncFloat(ref mBodyAlpha);
		SexyFramework.Misc.Buffer buffer = sync.GetBuffer();
		if (sync.isWrite())
		{
			buffer.WriteBoolean(GameApp.gApp.Is3DAccelerated());
			buffer.WriteLong(mBulletFX.Count);
			for (int i = 0; i < mBulletFX.Count; i++)
			{
				DarkFrogBulletFX darkFrogBulletFX = mBulletFX[i];
				buffer.WriteLong(darkFrogBulletFX.mBulletId);
				buffer.WriteBoolean(darkFrogBulletFX.mExploding);
				buffer.WriteFloat(darkFrogBulletFX.mX);
				buffer.WriteFloat(darkFrogBulletFX.mY);
				buffer.WriteFloat(darkFrogBulletFX.mTwirlAngle);
				buffer.WriteBoolean(darkFrogBulletFX.mBallEffect != null);
				if (darkFrogBulletFX.mBallEffect != null)
				{
					Common.SerializePIEffect(darkFrogBulletFX.mBallEffect, sync);
				}
				buffer.WriteBoolean(darkFrogBulletFX.mBallExplosion != null);
				if (darkFrogBulletFX.mBallExplosion != null)
				{
					Common.SerializePIEffect(darkFrogBulletFX.mBallExplosion, sync);
				}
			}
			return;
		}
		mBulletFX.Clear();
		bool particle_3d = buffer.ReadBoolean();
		int num = (int)buffer.ReadLong();
		for (int j = 0; j < num; j++)
		{
			DarkFrogBulletFX darkFrogBulletFX2 = new DarkFrogBulletFX();
			mBulletFX.Add(darkFrogBulletFX2);
			darkFrogBulletFX2.mBulletId = (int)buffer.ReadLong();
			darkFrogBulletFX2.mExploding = buffer.ReadBoolean();
			darkFrogBulletFX2.mX = buffer.ReadFloat();
			darkFrogBulletFX2.mY = buffer.ReadFloat();
			darkFrogBulletFX2.mTwirlAngle = buffer.ReadFloat();
			if (buffer.ReadBoolean())
			{
				darkFrogBulletFX2.mBallEffect = GetFreeBallEffect(particle_3d);
				Common.DeserializePIEffect(darkFrogBulletFX2.mBallEffect, sync);
			}
			if (buffer.ReadBoolean())
			{
				darkFrogBulletFX2.mBallExplosion = GetFreeBallExplosion(particle_3d);
				Common.DeserializePIEffect(darkFrogBulletFX2.mBallExplosion, sync);
			}
		}
	}
}
