using System;
using System.Collections.Generic;
using SexyFramework;
using SexyFramework.AELib;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.PIL;

namespace ZumasRevenge;

public class BossDoctor : BossShoot
{
	private enum ETutorial
	{
		Tutorial_None,
		Tutorial_StateNoShield,
		Tutorial_StatePhaseIn
	}

	public static Image[] gBulletImages = new Image[2];

	public static int MAX_SPEAR_Y_OFF = -50;

	public static float MAX_ANGLE = 0.2617992f;

	public static int MIN_SPAWN_TIMER = 100;

	protected PIEffect mTriangleExplosion;

	protected PIEffect mCircleExplosion;

	protected List<PIEffect> mShieldZaps = new List<PIEffect>();

	protected CompositionMgr mTikiCompMgr;

	protected CompositionMgr mShieldCompMgr;

	protected DoctorStaff[] mStaff = new DoctorStaff[2];

	protected List<BossBulletParticleSystem> mParticles = new List<BossBulletParticleSystem>();

	protected bool mIsFiring;

	protected bool mIsRetaliating;

	protected bool mDoingHitAnim;

	protected bool mHitAnimUp;

	protected bool mDoCircleExplosion;

	protected bool mDoTriangleExplosion;

	protected int mHairCel;

	protected int mFaceCel;

	protected int mBerserkEyeFrame;

	protected int mBerserkCounter;

	protected int mTutorialState;

	protected int mMaterializeTimer;

	protected float mBossYOff;

	protected float mBerserkBaseAlpha;

	protected float mBerserkBaseAlphaDir;

	protected override void DrawBossSpecificArt(Graphics g)
	{
		int num = (int)(mX - (float)(mWidth / 2) + (float)mShakeXOff);
		int num2 = (int)(mY - (float)(mHeight / 2) + (float)mShakeYOff);
		if (mAlphaOverride != 0f)
		{
			g.PushState();
			if (mAlphaOverride < 255f)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)mAlphaOverride);
			}
			g.DrawImageCel(Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_HAIR), Common._S(num + Common._M(3)), (int)((float)Common._S(num2 + Common._M1(2)) + mBossYOff), mHairCel);
			if (g.Is3D())
			{
				g.DrawImageRotatedF(Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_ROD_RIGHT), Common._S(num + Common._M(130)), Common._S((float)(num2 + Common._M1(52)) + mStaff[0].mYOff), mStaff[0].mAngle);
			}
			else
			{
				g.DrawImageRotated(Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_ROD_RIGHT), Common._S(num + Common._M(130)), (int)Common._S((float)(num2 + Common._M1(52)) + mStaff[0].mYOff), mStaff[0].mAngle);
			}
			if (g.Is3D())
			{
				g.DrawImageRotatedF(Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_ROD_LEFT), Common._S(num + Common._M(0)), Common._S((float)(num2 + Common._M1(52)) + mStaff[1].mYOff), mStaff[1].mAngle);
			}
			else
			{
				g.DrawImageRotated(Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_ROD_LEFT), Common._S(num + Common._M(0)), (int)Common._S((float)(num2 + Common._M1(52)) + mStaff[1].mYOff), mStaff[1].mAngle);
			}
			g.DrawImageCel(Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_FACES), Common._S(num + Common._M(55)), (int)Common._S((float)(num2 + Common._M1(37)) + mBossYOff), mFaceCel);
			DrawGlowingRings(g, mStaff[0].mRingSize, Common._S(mStaff[0].mYOff), Common._S(Common._M(160)), Common._S(Common._M1(70)), new Color(Common._M2(255), Common._M3(224), Common._M4(0), (int)mStaff[0].mRingAlpha), new Color(Common._M(255), Common._M1(255), Common._M2(0), (int)(mStaff[0].mRingAlpha / Common._M3(2f))), new Color(Common._M4(255), Common._M5(0), Common._M6(0), (int)(mStaff[0].mRingAlpha / Common._M7(3f))));
			DrawGlowingRings(g, mStaff[1].mRingSize, Common._S(mStaff[1].mYOff), Common._S(Common._M(15)), Common._S(Common._M1(70)), new Color(Common._M2(36), Common._M3(18), Common._M4(53), (int)mStaff[1].mRingAlpha), new Color(Common._M(36), Common._M1(18), Common._M2(53), (int)(mStaff[1].mRingAlpha / Common._M3(2f))), new Color(Common._M4(255), Common._M5(0), Common._M6(0), (int)(mStaff[1].mRingAlpha / Common._M7(3f))));
			g.PopState();
		}
	}

	protected override void DrawBerserk(Graphics g)
	{
	}

	protected override Rect GetBulletRect(BossBullet b)
	{
		Image image = gBulletImages[b.mImageNum];
		int num = (int)((float)image.mWidth * b.mSize);
		int num2 = (int)((float)image.mHeight * b.mSize);
		return new Rect((int)(b.mX - (float)(num / 2)), (int)(b.mY - (float)(num2 / 2)), num, num2).Inflate(Common._M(0), Common._M1(0));
	}

	protected override void DidFire()
	{
		base.DidFire();
		if (mHairCel == 0 && mBossYOff == 0f && !mDoingHitAnim)
		{
			mStaff[0].mControlsExtras = true;
			mStaff[1].mControlsExtras = false;
		}
		if (!mIsFiring)
		{
			int index = mBullets.size() - base.mMaxBulletsToFire;
			AddParticleSystem(mBullets[index].mId, (int)mBullets[index].mX, (int)mBullets[index].mY, 0, mBullets[index].mDelay);
		}
		mIsFiring = true;
		mStaff[0].mNumBullets += base.mMaxBulletsToFire;
		for (int num = mBullets.size() - 1; num >= mBullets.size() - base.mMaxBulletsToFire; num--)
		{
			mBullets[num].mImageNum = 0;
			mBullets[num].mSize = 0f;
			mBullets[num].mAlpha = 0f;
		}
	}

	protected override void DidRetaliate(int num_shot)
	{
		if (!mIsRetaliating)
		{
			int index = mBullets.size() - num_shot;
			AddParticleSystem(mBullets[index].mId, (int)mBullets[index].mX, (int)mBullets[index].mY, 1, mBullets[index].mDelay);
		}
		mIsRetaliating = true;
		mStaff[1].mNumBullets += num_shot;
		if (mHairCel == 0 && mBossYOff == 0f && !mDoingHitAnim)
		{
			mStaff[0].mControlsExtras = false;
			mStaff[1].mControlsExtras = true;
		}
		for (int num = mBullets.size() - 1; num >= mBullets.size() - num_shot; num--)
		{
			mBullets[num].mImageNum = 1;
			mBullets[num].mSize = 0f;
			mBullets[num].mAlpha = 0f;
		}
	}

	protected override bool PreBulletUpdate(BossBullet b, int index)
	{
		if (b.mState == 0)
		{
			if (!PreState0Helper(0, b) && !PreState0Helper(1, b))
			{
				return true;
			}
		}
		else
		{
			if (b.mDelay > 0)
			{
				b.mDelay--;
				if (!PreDelayHelper(0, b))
				{
					PreDelayHelper(1, b);
				}
				return true;
			}
			if (b.mDelay == 0 && b.mState > 0)
			{
				float num = Common._M(12f);
				float num2 = Common._M(0.1f);
				if (b.mSize < 1f)
				{
					b.mSize = Math.Min(b.mSize + num2, 1f);
					if (b.mImageNum == 0)
					{
						b.mX = mX + (float)Common._M(74);
					}
				}
				if (b.mAlpha < 255f)
				{
					b.mAlpha = Math.Min(b.mAlpha + num, 255f);
				}
			}
		}
		return false;
	}

	protected void UpdateStaffState(ref bool state_bool, float ring_size_max, DoctorStaff d)
	{
		float num = Common._M(3f);
		float num2 = Common._M(4f);
		float num3 = Common._M(-15);
		float num4 = 0f - (float)MAX_SPEAR_Y_OFF / num;
		float num5 = 0f - (float)MAX_SPEAR_Y_OFF / num2;
		float num6 = num3 / (0f - num4);
		float num7 = num3 / (0f - num5);
		int num8 = (int)(num5 / (float)(Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_HAIR).mNumCols - 2));
		float num9 = MAX_ANGLE / num4;
		int num10 = (int)(num4 / 4f);
		int num11 = (int)(num5 / 4f);
		if (state_bool)
		{
			if (d.mNumBullets > 0 && d.mYOff > (float)MAX_SPEAR_Y_OFF)
			{
				if (d.mControlsExtras && !mDoingHitAnim && mBossYOff > num3)
				{
					mBossYOff = Math.Max(mBossYOff - num6, num3);
				}
				d.mYOff = Math.Max(d.mYOff - num, MAX_SPEAR_Y_OFF);
				d.mAngle = Math.Min(d.mAngle + num9, MAX_ANGLE);
				if (d.mControlsExtras && !mDoingHitAnim && mHairCel == 0 && d.mYOff <= (float)(MAX_SPEAR_Y_OFF / 3))
				{
					mHairCel++;
				}
				else if (d.mControlsExtras && !mDoingHitAnim && mHairCel == 1 && d.mYOff <= (float)(2 * MAX_SPEAR_Y_OFF / 3))
				{
					mHairCel++;
				}
				if (d.mControlsExtras && !mDoingHitAnim && mUpdateCount % num10 == 0 && mFaceCel < 3)
				{
					mFaceCel++;
				}
			}
			else if (d.mNumBullets > 0 && d.mYOff <= (float)MAX_SPEAR_Y_OFF)
			{
				int num12 = Common._M(8);
				if (d.mRingAlpha < 255f)
				{
					d.mRingAlpha = Math.Min(d.mRingAlpha + (float)num12, 255f);
				}
				if (d.mRingSize < ring_size_max)
				{
					d.mRingSize += Common._M(0.05f);
					if (d.mRingSize > ring_size_max)
					{
						d.mRingSize = ring_size_max;
					}
				}
			}
			else
			{
				if (d.mNumBullets != 0 || !(d.mYOff < 0f))
				{
					return;
				}
				d.mYOff = Math.Min(d.mYOff + num2, 0f);
				d.mAngle = Math.Max(d.mAngle - num9, 0f);
				if (d.mRingSize != 0f && mBossYOff < 0f)
				{
					mBossYOff = Math.Min(mBossYOff + num7, 0f);
				}
				if (d.mControlsExtras && !mDoingHitAnim && mUpdateCount % num8 == 0)
				{
					mHairCel++;
				}
				if (d.mControlsExtras && !mDoingHitAnim && mHairCel >= Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_HAIR).mNumCols)
				{
					mHairCel = 0;
				}
				if (d.mControlsExtras && !mDoingHitAnim && mFaceCel > 0 && mUpdateCount % num11 == 0)
				{
					mFaceCel--;
				}
				int num13 = Common._M(12);
				if (d.mRingAlpha > 0f)
				{
					d.mRingAlpha = Math.Max(0f, d.mRingAlpha - (float)num13);
				}
				if (d.mYOff >= 0f)
				{
					if (d.mRingAlpha <= 0f)
					{
						d.mRingSize = 0f;
					}
					state_bool = false;
					if (d.mControlsExtras && !mDoingHitAnim)
					{
						mHairCel = 0;
					}
				}
			}
		}
		else if (d.mRingAlpha > 0f)
		{
			d.mRingAlpha = Math.Max(0f, d.mRingAlpha - (float)Common._M(6));
			if (d.mRingAlpha <= 0f)
			{
				d.mRingSize = 0f;
			}
		}
	}

	protected bool PreState0Helper(int staffnum, BossBullet b)
	{
		if (b.mImageNum == staffnum && mStaff[staffnum].mYOff <= (float)MAX_SPEAR_Y_OFF && mStaff[staffnum].mRingAlpha >= 255f)
		{
			if (b.mState != 1)
			{
				bool flag = false;
				for (int i = 0; i < mParticles.size(); i++)
				{
					if (mParticles[i].mBulletId == b.mId)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					AddParticleSystem(b.mId, (int)b.mX, (int)b.mY, -1, b.mDelay);
				}
			}
			b.mState = 1;
			if (b.mDelay == 0)
			{
				mStaff[staffnum].mNumBullets--;
				b.mX = mX + (float)((staffnum == 0) ? Common._M(74) : Common._M1(-65));
				b.mY = mY + mStaff[staffnum].mYOff + (float)Common._M(16);
				for (int j = 0; j < mParticles.Count; j++)
				{
					if (mParticles[j].mBulletId == b.mId)
					{
						mParticles[j].mSystem.SetPos(b.mX - (float)Common._M(5), b.mY - (float)Common._M1(0));
						mParticles[j].mAttachedToStaff = -1;
						break;
					}
				}
				if (b.mShotType == 1)
				{
					FireBulletAtPlayer(b, SexyFramework.Common.FloatRange(base.mMinBulletSpeed, base.mMaxBulletSpeed), b.mX, b.mY);
					b.mTargetVX = b.mVX;
					b.mTargetVY = b.mVY;
				}
			}
			return true;
		}
		return false;
	}

	protected bool PreDelayHelper(int staffnum, BossBullet b)
	{
		if (b.mImageNum == staffnum && b.mDelay == 0)
		{
			mStaff[staffnum].mNumBullets--;
			b.mX = mX + (float)((staffnum == 0) ? Common._M(74) : Common._M1(-45));
			b.mY = mY + mStaff[staffnum].mYOff + (float)Common._M(24);
			for (int i = 0; i < mParticles.Count; i++)
			{
				if (mParticles[i].mBulletId == b.mId)
				{
					mParticles[i].mSystem.SetPos(b.mX - (float)Common._M(5), b.mY - (float)Common._M1(0));
					break;
				}
			}
			if (b.mShotType == 1)
			{
				FireBulletAtPlayer(b, SexyFramework.Common.FloatRange(base.mMinBulletSpeed, base.mMaxBulletSpeed), b.mX, b.mY);
				b.mTargetVX = b.mVX;
				b.mTargetVY = b.mVY;
			}
			return true;
		}
		return false;
	}

	protected override bool DoHit(Bullet b, bool from_prox_bomb)
	{
		if (mHP != mMaxHP && mTutorialState != 0)
		{
			return false;
		}
		if (!mIsFiring && !mIsRetaliating)
		{
			mDoingHitAnim = true;
			mFaceCel = 4;
			mHitAnimUp = true;
		}
		if (mTutorialState == 1)
		{
			((GameApp)GlobalMembers.gSexyApp).GetBoard().mPreventBallAdvancement = true;
			TauntText tauntText = new TauntText();
			mTauntQueue.Add(tauntText);
			tauntText.mText = TextManager.getInstance().getString(309);
			tauntText.mTextId = 309;
			tauntText.mDelay = Common._M(1000);
			tauntText = new TauntText();
			mTauntQueue.Add(tauntText);
			tauntText.mText = TextManager.getInstance().getString(310);
			tauntText.mTextId = 310;
			tauntText.mDelay = Common._M(1000);
			mMaterializeTimer = Common._M(100);
			mWalls[0].mAlphaFadeDir = 1;
			mTikis[0].mWasHit = false;
			mTikis[0].mAlphaFadeDir = 1;
			mWalls[1].mAlphaFadeDir = 1;
			mTikis[1].mWasHit = false;
			mTikis[1].mAlphaFadeDir = 1;
			mTutorialState = 2;
			mPauseMovement = true;
		}
		return base.DoHit(b, from_prox_bomb);
	}

	protected override void DrawWalls(Graphics g)
	{
		int[] array = new int[2]
		{
			Common._DS(Common._M(-225)),
			Common._DS(Common._M1(-225))
		};
		int[] array2 = new int[2]
		{
			Common._DS(Common._M(-50)),
			Common._DS(Common._M1(0))
		};
		for (int i = 0; i < mWalls.Count; i++)
		{
			BossWall bossWall = mWalls[i];
			if (bossWall.mAlpha > 0)
			{
				Composition composition = mShieldCompMgr.GetComposition((i == 1) ? "TriangleShieldAnim" : "CircleShieldAnim");
				int num = (int)(Common._S(mX) - (float)Common._DS(composition.mWidth / 2) + (float)array[i]);
				int num2 = (int)(Common._S(mY) + (float)Common._DS(mHeight / 2) + (float)array2[i]);
				CumulativeTransform cumulativeTransform = new CumulativeTransform();
				cumulativeTransform.mOpacity = (float)bossWall.mAlpha / 255f;
				if (mAlphaOverride <= 254f)
				{
					cumulativeTransform.mOpacity = mAlphaOverride / 255f;
				}
				cumulativeTransform.mTrans.Translate(num, num2);
				composition.Draw(g, cumulativeTransform, -1, Common._DS(1f));
			}
		}
		if (mDoCircleExplosion && g.Is3D())
		{
			mCircleExplosion.Draw(g);
		}
		if (mDoTriangleExplosion && g.Is3D())
		{
			mTriangleExplosion.Draw(g);
		}
		for (int j = 0; j < mShieldZaps.Count; j++)
		{
			mShieldZaps[j].mDrawTransform.LoadIdentity();
			float num3 = GameApp.DownScaleNum(1f);
			mShieldZaps[j].mDrawTransform.Scale(num3, num3);
			int num4 = 0;
			if (mWalls[1].mAlpha <= 0)
			{
				num4 -= Common._M(30);
			}
			mShieldZaps[j].mDrawTransform.Translate(Common._S(mX), Common._S(mY) + (float)Common._S(mHeight / 2 + 30 + num4));
			mShieldZaps[j].Draw(g);
		}
	}

	protected override bool CollidesWithWall(Bullet b)
	{
		bool flag = false;
		for (int i = 0; i < mWalls.Count; i++)
		{
			if (mWalls[i].mAlphaFadeDir >= 0 && mWalls[i].mAlpha >= 1)
			{
				flag = true;
				break;
			}
		}
		if (flag && mTutorialState == 0)
		{
			if (MathUtils.CirclesIntersect(b.GetX(), b.GetY(), mX, mY, b.GetRadius() + Common._M(125)))
			{
				PIEffect pIEffect = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_SHIELD_ZAP).Duplicate();
				if (GameApp.gApp.mHiRes)
				{
					pIEffect.mEmitterTransform.Translate(Common._S(b.GetX() - mX), 0f);
				}
				else
				{
					pIEffect.mEmitterTransform.Translate(Common._S((b.GetX() - mX) * 2f), 0f);
				}
				Common.SetFXNumScale(pIEffect, GameApp.gApp.Is3DAccelerated() ? 1f : Common._M(0.5f));
				mShieldZaps.Add(pIEffect);
				GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BOSS2_SHIELD_HIT));
				mTauntQueue.Clear();
				TauntText tauntText = new TauntText();
				mTauntQueue.Add(tauntText);
				tauntText.mText = TextManager.getInstance().getString(311);
				tauntText.mTextId = 311;
				tauntText.mDelay = Common._M(500);
				return true;
			}
			return false;
		}
		return false;
	}

	protected override void BulletHitPlayer(BossBullet b)
	{
		base.BulletHitPlayer(b);
		int num = Common._M(150);
		for (int i = 0; i < mParticles.Count; i++)
		{
			BossBulletParticleSystem bossBulletParticleSystem = mParticles[i];
			if (bossBulletParticleSystem.mBulletId == b.mId)
			{
				bossBulletParticleSystem.mBulletId = -1;
				bossBulletParticleSystem.mSystem.SetLife(bossBulletParticleSystem.mSystem.GetUpdateCount() + num);
				List<Particle> particles = new List<Particle>();
				bossBulletParticleSystem.mSystem.GetEmitter(bossBulletParticleSystem.mEmitterHandle).GetParticlesOfType(bossBulletParticleSystem.mHead1Handle, ref particles);
				bossBulletParticleSystem.mSystem.GetEmitter(bossBulletParticleSystem.mEmitterHandle).GetParticlesOfType(bossBulletParticleSystem.mHead2Handle, ref particles);
				for (int j = 0; j < particles.Count; j++)
				{
					Particle particle = particles[j];
					particle.mLife = num;
					particle.mUpdateCount = 0;
					particle.ClearLifetimeFrames();
					LifetimeSettings lifetimeSettings = new LifetimeSettings();
					lifetimeSettings.mSpinMult = Common._M(2f);
					lifetimeSettings.mSizeXMult = Common._M(4f);
					particle.AddLifetimeKeyFrame(1f, lifetimeSettings);
					particle.mAlphaKeyManager.ForceTransition(num, new Color(255, 255, 255, 0));
				}
				break;
			}
		}
		if (GameApp.gApp.GetLevelMgr().mBossesCanAttackFuckedFrog)
		{
			return;
		}
		for (int k = 0; k < mBullets.Count; k++)
		{
			BossBullet bossBullet = mBullets[k];
			if (bossBullet.mDelay > 0)
			{
				bossBullet.mDeleteInstantly = true;
			}
		}
	}

	public override void DeleteAllBullets()
	{
		base.DeleteAllBullets();
		mParticles.Clear();
	}

	protected void AddParticleSystem(int bullet_id, int x, int y, int attached)
	{
		AddParticleSystem(bullet_id, x, y, attached, 0);
	}

	protected void AddParticleSystem(int bullet_id, int x, int y, int attached, int delay)
	{
		BossBulletParticleSystem bossBulletParticleSystem = new BossBulletParticleSystem();
		mParticles.Add(bossBulletParticleSystem);
		bossBulletParticleSystem.mBulletId = bullet_id;
		bossBulletParticleSystem.mAttachedToStaff = attached;
		bossBulletParticleSystem.mSystem = new SexyFramework.PIL.System(50, 50);
		bossBulletParticleSystem.mSystem.mScale = Common._S(1f);
		bossBulletParticleSystem.mSystem.SetMinSpawnFrame(delay + 1);
		bossBulletParticleSystem.mSystem.WaitForEmitters(w: true);
		Emitter emitter = new Emitter();
		emitter.mCullingRect = new Rect(0, 0, Common._SS(GlobalMembers.gSexyApp.mWidth), Common._SS(GlobalMembers.gSexyApp.mHeight));
		emitter.mEmissionCoordsAreOffsets = true;
		EmitterScale emitterScale = new EmitterScale();
		emitterScale.mLifeScale = Common._M(0.27f);
		emitterScale.mNumberScale = Common._M(2f);
		emitterScale.mSizeXScale = Common._M(0.8f);
		emitterScale.mVelocityScale = Common._M(1.5f);
		emitterScale.mWeightScale = Common._M(3f);
		emitterScale.mSpinScale = Common._M(0.54f);
		emitterScale.mZoom = Common._M(3.63f);
		emitter.AddScaleKeyFrame(0, emitterScale);
		EmitterSettings emitterSettings = new EmitterSettings();
		emitterSettings.mEmissionRange = SexyFramework.Common.DegreesToRadians(163f);
		float mXRadius = (emitterSettings.mYRadius = Common._M(3));
		emitterSettings.mXRadius = mXRadius;
		emitter.AddSettingsKeyFrame(0, emitterSettings);
		emitter.SetEmitterType(2);
		ParticleType particleType = new ParticleType();
		ParticleSettings particleSettings = new ParticleSettings();
		particleSettings.mLife = Common._M(6);
		particleSettings.mNumber = Common._M(9999);
		particleSettings.mXSize = Common._M(14);
		particleSettings.mSpin = SexyFramework.Common.DegreesToRadians(Common._M(70));
		particleType.AddSettingsKeyFrame(0, particleSettings);
		particleType.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_RUNE_LEFT);
		particleType.mSingle = true;
		bossBulletParticleSystem.mHead1Handle = emitter.AddParticleType(particleType);
		ParticleType particleType2 = new ParticleType(particleType);
		particleType2.mAdditive = true;
		particleType2.GetSettingsKeyFrame(0).mSpin *= Common._M(-1f);
		particleType2.GetSettingsKeyFrame(0).mXSize = Common._M(20);
		particleType2.mColorKeyManager.AddColorKey(0f, new Color(Common._M(50), Common._M1(100), Common._M2(255)));
		bossBulletParticleSystem.mHead2Handle = emitter.AddParticleType(particleType2);
		ParticleType particleType3 = new ParticleType();
		particleType3.mImage = Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_RUNE_LEFT_OUTLINE);
		particleType3.mColorKeyManager.AddColorKey(0f, new Color(Common._M(121), Common._M1(12), Common._M2(255)));
		particleType3.mColorKeyManager.AddColorKey(Common._M(0.2f), new Color(Common._M1(121), Common._M2(12), Common._M3(255)));
		particleType3.mColorKeyManager.AddColorKey(1f, new Color(255, 0, 0));
		particleType3.mAlphaKeyManager.AddAlphaKey(0f, 0);
		particleType3.mAlphaKeyManager.AddAlphaKey(Common._M(0.15f), 255);
		particleType3.mAlphaKeyManager.AddAlphaKey(Common._M(0.85f), 255);
		particleType3.mAlphaKeyManager.AddAlphaKey(1f, 0);
		particleType3.mAdditive = true;
		particleSettings = new ParticleSettings();
		particleSettings.mLife = Common._M(13);
		particleSettings.mNumber = Common._M(27);
		particleSettings.mXSize = Common._M(9);
		particleSettings.mSpin = SexyFramework.Common.DegreesToRadians(Common._M(50));
		particleType3.AddSettingsKeyFrame(0, particleSettings);
		ParticleVariance particleVariance = new ParticleVariance();
		particleVariance.mSizeXVar = Common._M(10);
		particleVariance.mSpinVar = SexyFramework.Common.DegreesToRadians(Common._M(50));
		particleType3.AddVarianceKeyFrame(0, particleVariance);
		LifetimeSettings lifetimeSettings = new LifetimeSettings();
		particleType3.AddSettingAtLifePct(0f, lifetimeSettings);
		lifetimeSettings = new LifetimeSettings(lifetimeSettings);
		lifetimeSettings.mSizeXMult = Common._M(1.5f);
		particleType3.AddSettingAtLifePct(1f, lifetimeSettings);
		emitter.AddParticleType(particleType3);
		bossBulletParticleSystem.mEmitterHandle = bossBulletParticleSystem.mSystem.AddEmitter(emitter);
		bossBulletParticleSystem.mSystem.SetPos(x, y);
	}

	protected void DrawGlowingRings(Graphics g, float size, float yoff, float ring_xoff, float ring_yoff, Color c1, Color c2, Color c3)
	{
		int num = (int)Common._S(mX - (float)(mWidth / 2) + (float)mShakeXOff);
		int num2 = (int)Common._S(mY - (float)(mHeight / 2) + (float)mShakeYOff);
		Color color = c1;
		Color color2 = c2;
		Color color3 = c3;
		color.mAlpha = (int)Math.Min(color.mAlpha, mAlphaOverride);
		color2.mAlpha = (int)Math.Min(color2.mAlpha, mAlphaOverride);
		color3.mAlpha = (int)Math.Min(color3.mAlpha, mAlphaOverride);
		if (Common._M(0) != 0)
		{
			g.SetDrawMode(1);
		}
		mGlobalTranform.Reset();
		mGlobalTranform.Scale(size, size);
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(color);
		if (g.Is3D())
		{
			g.DrawImageTransformF(Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_GLOW_RINGS), mGlobalTranform, (float)num + ring_xoff, (float)num2 + ring_yoff + yoff);
		}
		else
		{
			g.DrawImageTransform(Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_GLOW_RINGS), mGlobalTranform, (float)num + ring_xoff, (float)num2 + ring_yoff + yoff);
		}
		mGlobalTranform.Reset();
		float num3 = size / Common._M(0.51f);
		mGlobalTranform.GetMatrix().Scale(num3, num3);
		g.SetColor(color2);
		if (g.Is3D())
		{
			g.DrawImageTransformF(Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_GLOW_CIRCLE), mGlobalTranform, (float)num + ring_xoff, (float)num2 + ring_yoff + yoff);
		}
		else
		{
			g.DrawImageTransform(Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_GLOW_CIRCLE), mGlobalTranform, (float)num + ring_xoff, (float)num2 + ring_yoff + yoff);
		}
		num3 = size / Common._M(1.1f);
		mGlobalTranform.Reset();
		mGlobalTranform.Scale(num3, num3);
		g.SetColor(color3);
		if (g.Is3D())
		{
			g.DrawImageTransformF(Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_GLOW_CIRCLE), mGlobalTranform, (float)num + ring_xoff, (float)num2 + ring_yoff + yoff);
		}
		else
		{
			g.DrawImageTransform(Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_GLOW_CIRCLE), mGlobalTranform, (float)num + ring_xoff, (float)num2 + ring_yoff + yoff);
		}
		g.SetDrawMode(0);
		g.SetColorizeImages(colorizeImages: false);
	}

	protected override void BossBulletDestroyed(BossBullet b, bool outofscreen)
	{
		base.BossBulletDestroyed(b, outofscreen);
		for (int i = 0; i < mParticles.Count; i++)
		{
			if (mParticles[i].mBulletId == b.mId)
			{
				mParticles[i].mSystem.ForceStopEmitting(f: true);
				break;
			}
		}
	}

	protected override bool CanFire()
	{
		return mTutorialState == 0;
	}

	protected override bool CanTaunt()
	{
		if (mTutorialState != 0)
		{
			return false;
		}
		return base.CanTaunt();
	}

	protected override bool CanRetaliate()
	{
		return mTutorialState == 0;
	}

	protected override bool CanDecTikiHealthSpawnAmt()
	{
		return mTutorialState == 0;
	}

	protected override void ResetWallAndTikis(int wall_index)
	{
		if (mHP <= 0f)
		{
			return;
		}
		string text = TextManager.getInstance().getString(695);
		mWalls[wall_index].mAlphaFadeDir = 1;
		if (mWalls.Count == mTikis.Count)
		{
			mTikis[wall_index].mWasHit = false;
			mTikis[wall_index].mAlphaFadeDir = 1;
			if (mHP > 0f && mTutorialState == 0 && (mTauntQueue.Count == 0 || !string.Equals(mTauntQueue.back().mText, text)) && mHP < 100f)
			{
				mTauntQueue.Clear();
				TauntText tauntText = new TauntText();
				mTauntQueue.Add(tauntText);
				tauntText.mText = text;
				tauntText.mTextId = 695;
				tauntText.mDelay = Common._M(500);
			}
		}
		GlobalMembers.gSexyApp.PlaySample(Res.GetSoundByID(ResID.SOUND_TIKI_APPEAR), Common._M(50));
	}

	protected override void TikiHit(int idx)
	{
		if (mTikis[idx].mIsLeftTiki)
		{
			mDoCircleExplosion = true;
			mCircleExplosion.ResetAnim();
		}
		else
		{
			mDoTriangleExplosion = true;
			mTriangleExplosion.ResetAnim();
		}
	}

	public BossDoctor()
		: base(null)
	{
		Initialize();
	}

	public BossDoctor(Level l)
		: base(l)
	{
		Initialize();
	}

	private void Initialize()
	{
		mMaterializeTimer = MIN_SPAWN_TIMER;
		mTikiCompMgr = null;
		mShieldCompMgr = null;
		mTriangleExplosion = null;
		mCircleExplosion = null;
		mDoCircleExplosion = false;
		mDoTriangleExplosion = false;
		mIsFiring = (mIsRetaliating = (mDoingHitAnim = false));
		mHairCel = (mFaceCel = 0);
		mHitAnimUp = true;
		mBossYOff = 0f;
		mBerserkBaseAlpha = 0f;
		mBerserkEyeFrame = 0;
		mBerserkCounter = 0;
		mBerserkBaseAlphaDir = Common._M(4.6f);
		mResGroup = "Boss2";
		mResPrefix = "IMAGE_BOSS_DOCTOR_";
		mBossRadius = Common._M(85);
		mBulletRadius = Common._M(25);
		mBandagedXOff = Common._M(10);
		mBandagedYOff = Common._M(15);
		if (GameApp.gApp != null && (GameApp.gApp.IsHardMode() || (GameApp.gApp.mUserProfile != null && GameApp.gApp.mUserProfile.GetAdvModeVars().mNumTimesZoneBeat[1] > 0)))
		{
			mTutorialState = 0;
		}
		else
		{
			mTutorialState = 1;
		}
		for (int i = 0; i < mStaff.Length; i++)
		{
			mStaff[i] = new DoctorStaff();
		}
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public override void Update()
	{
		Update(1f);
	}

	public override void Update(float f)
	{
		base.Update(f);
		if (mTutorialState == 2 && mWalls[0].mAlpha == 255 && mWalls[1].mAlpha == 255 && mTikis[0].mAlpha == 255 && mTikis[1].mAlpha == 255)
		{
			mTutorialState = 0;
			mPauseMovement = false;
			((GameApp)GlobalMembers.gSexyApp).GetBoard().mPreventBallAdvancement = false;
			mMaterializeTimer = Common._M(20);
		}
		mShieldCompMgr.UpdateAll();
		mTikiCompMgr.UpdateAll();
		if (mDoCircleExplosion)
		{
			int num = (int)(Common._S(mX) + (float)Common._DS(Common._M(22)));
			int num2 = (int)(Common._S(mY) + (float)Common._DS(Common._M(105)));
			mCircleExplosion.mDrawTransform.LoadIdentity();
			float num3 = GameApp.ScaleNum(1f);
			mCircleExplosion.mDrawTransform.Scale(num3, num3);
			mCircleExplosion.mDrawTransform.Translate(num, num2);
			mCircleExplosion.Update();
			if (mCircleExplosion.mFrameNum > (float)mCircleExplosion.mLastFrameNum)
			{
				mDoCircleExplosion = false;
			}
		}
		if (mDoTriangleExplosion)
		{
			int num4 = (int)(Common._S(mX) + (float)Common._DS(Common._M(30)));
			int num5 = (int)(Common._S(mY) + (float)Common._DS(Common._M(210)));
			mTriangleExplosion.mDrawTransform.LoadIdentity();
			float num6 = GameApp.ScaleNum(1f);
			mTriangleExplosion.mDrawTransform.Scale(num6, num6);
			mTriangleExplosion.mDrawTransform.Translate(num4, num5);
			mTriangleExplosion.Update();
			if (mTriangleExplosion.mFrameNum > (float)mTriangleExplosion.mLastFrameNum)
			{
				mDoTriangleExplosion = false;
			}
		}
		for (int i = 0; i < mParticles.Count; i++)
		{
			BossBulletParticleSystem bossBulletParticleSystem = mParticles[i];
			BossBullet bossBullet = null;
			for (int j = 0; j < mBullets.Count; j++)
			{
				if (mBullets[j].mId == bossBulletParticleSystem.mBulletId)
				{
					bossBullet = mBullets[j];
					break;
				}
			}
			if (bossBullet != null && bossBullet.mDelay == 0 && bossBullet.mState > 0)
			{
				bossBulletParticleSystem.mSystem.Move(bossBullet.mVX, bossBullet.mVY);
			}
			else if (bossBulletParticleSystem.mBulletId == -1)
			{
				int centerX = mLevel.mFrog.GetCenterX();
				int centerY = mLevel.mFrog.GetCenterY();
				float xamt = 0f;
				float yamt = 0f;
				float num7 = Common._M(10f);
				if (!SexyFramework.Common._eq(bossBulletParticleSystem.mSystem.GetLastX(), centerX, 0.1f))
				{
					xamt = ((float)centerX - bossBulletParticleSystem.mSystem.GetLastX()) / num7;
				}
				if (!SexyFramework.Common._eq(bossBulletParticleSystem.mSystem.GetLastY(), centerY, 0.1f))
				{
					yamt = ((float)centerY - bossBulletParticleSystem.mSystem.GetLastY()) / num7;
				}
				bossBulletParticleSystem.mSystem.Move(xamt, yamt);
			}
			bossBulletParticleSystem.mSystem.Update();
			if (bossBulletParticleSystem.mSystem.Done())
			{
				mParticles.RemoveAt(i);
				i--;
			}
		}
		if (mDoDeathExplosions || mHP <= 0f || mLevel.mBoard.DoingBossIntro())
		{
			return;
		}
		if (mDoingHitAnim)
		{
			if (mUpdateCount % Common._M(6) == 0 && mFaceCel < Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_FACES).mNumCols - 1)
			{
				mFaceCel++;
			}
			if (mUpdateCount % Common._M(6) == 0 && mHairCel < Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_HAIR).mNumCols - 1)
			{
				mHairCel++;
			}
			if (mHitAnimUp)
			{
				mBossYOff -= Common._M(1f);
				if (mBossYOff <= (float)Common._M(-15))
				{
					mHitAnimUp = false;
					mBossYOff = Common._M(-15);
				}
			}
			else
			{
				mBossYOff += Common._M(1.5f);
				if (mBossYOff >= 0f)
				{
					mBossYOff = 0f;
					mFaceCel = 0;
					mHairCel = 0;
					mDoingHitAnim = false;
				}
			}
		}
		else
		{
			UpdateStaffState(ref mIsFiring, Common._M(0.75f), mStaff[0]);
			UpdateStaffState(ref mIsRetaliating, Common._M(0.75f), mStaff[1]);
		}
		for (int k = 0; k < mParticles.size(); k++)
		{
			if (mParticles[k].mAttachedToStaff != -1)
			{
				mParticles[k].mSystem.SetPos(mX + (float)((mParticles[k].mAttachedToStaff == 0) ? Common._M(74) : Common._M1(-65)), mY + mStaff[mParticles[k].mAttachedToStaff].mYOff + (float)Common._M2(16));
			}
		}
		if (mIsBerserk)
		{
			mBerserkCounter++;
			mBerserkBaseAlpha += mBerserkBaseAlphaDir;
			if (mBerserkBaseAlphaDir > 0f && mBerserkBaseAlpha >= (float)Common._M(102))
			{
				mBerserkBaseAlpha = Common._M(102);
				mBerserkBaseAlphaDir *= -1f;
			}
			else if (mBerserkBaseAlphaDir < 0f && mBerserkBaseAlpha <= 0f)
			{
				mBerserkBaseAlpha = 0f;
				mBerserkBaseAlphaDir *= -1f;
			}
		}
		for (int l = 0; l < mShieldZaps.Count; l++)
		{
			mShieldZaps[l].Update();
		}
	}

	public override void SyncState(DataSync sync)
	{
		base.SyncState(sync);
		sync.SyncLong(ref mTutorialState);
		sync.SyncLong(ref mMaterializeTimer);
		sync.SyncBoolean(ref mIsFiring);
		sync.SyncBoolean(ref mIsRetaliating);
		sync.SyncBoolean(ref mDoingHitAnim);
		sync.SyncBoolean(ref mHitAnimUp);
		sync.SyncLong(ref mHairCel);
		sync.SyncLong(ref mFaceCel);
		sync.SyncLong(ref mBerserkEyeFrame);
		sync.SyncLong(ref mBerserkCounter);
		sync.SyncFloat(ref mBossYOff);
		sync.SyncFloat(ref mBerserkBaseAlpha);
		sync.SyncFloat(ref mBerserkBaseAlphaDir);
		for (int i = 0; i < 2; i++)
		{
			DoctorStaff doctorStaff = mStaff[i];
			sync.SyncFloat(ref doctorStaff.mYOff);
			sync.SyncFloat(ref doctorStaff.mAngle);
			sync.SyncFloat(ref doctorStaff.mRingSize);
			sync.SyncFloat(ref doctorStaff.mRingAlpha);
			sync.SyncBoolean(ref doctorStaff.mControlsExtras);
			sync.SyncLong(ref doctorStaff.mNumBullets);
		}
		SexyFramework.Misc.Buffer buffer = sync.GetBuffer();
		if (sync.isWrite())
		{
			buffer.WriteLong(mParticles.Count);
			for (int j = 0; j < mParticles.Count; j++)
			{
				BossBulletParticleSystem bossBulletParticleSystem = mParticles[j];
				buffer.WriteLong(bossBulletParticleSystem.mAttachedToStaff);
				buffer.WriteLong(bossBulletParticleSystem.mBulletId);
				buffer.WriteLong(bossBulletParticleSystem.mEmitterHandle);
				buffer.WriteLong(bossBulletParticleSystem.mHead1Handle);
				buffer.WriteLong(bossBulletParticleSystem.mHead2Handle);
				Common.SerializeParticleSystem(bossBulletParticleSystem.mSystem, sync);
			}
			buffer.WriteLong(mShieldZaps.size());
			for (int k = 0; k < mShieldZaps.size(); k++)
			{
				Common.SerializePIEffect(mShieldZaps[k], sync);
			}
			return;
		}
		int num = (int)buffer.ReadLong();
		mParticles.Clear();
		for (int l = 0; l < num; l++)
		{
			BossBulletParticleSystem bossBulletParticleSystem2 = new BossBulletParticleSystem();
			bossBulletParticleSystem2.mAttachedToStaff = (int)buffer.ReadLong();
			bossBulletParticleSystem2.mBulletId = (int)buffer.ReadLong();
			bossBulletParticleSystem2.mEmitterHandle = (int)buffer.ReadLong();
			bossBulletParticleSystem2.mHead1Handle = (int)buffer.ReadLong();
			bossBulletParticleSystem2.mHead2Handle = (int)buffer.ReadLong();
			bossBulletParticleSystem2.mSystem = Common.DeserializeParticleSystem(sync);
			mParticles.Add(bossBulletParticleSystem2);
		}
		num = (int)buffer.ReadLong();
		mShieldZaps.Clear();
		for (int m = 0; m < num; m++)
		{
			PIEffect pIEffect = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_SHIELD_ZAP).Duplicate();
			mShieldZaps.Add(pIEffect);
			Common.DeserializePIEffect(pIEffect, sync);
			Common.SetFXNumScale(pIEffect, GameApp.gApp.Is3DAccelerated() ? 1f : Common._M(0.5f));
		}
	}

	public override void DrawTopLevel(Graphics g)
	{
		base.DrawTopLevel(g);
		if (mHP > 0f && !mDoDeathExplosions && !mLevel.mBoard.IsPaused())
		{
			if (mTeleportDir != 0)
			{
				g.PushState();
				g.ClearClipRect();
			}
			int num = 0;
			for (int i = 0; i < mParticles.Count; i++)
			{
				num += mParticles[i].mSystem.GetTotalParticles();
				mParticles[i].mSystem.mAlphaPct = mAlphaOverride / 255f;
				mParticles[i].mSystem.Draw(g);
			}
			if (mTeleportDir != 0)
			{
				g.PopState();
			}
		}
	}

	public override void Init(Level l)
	{
		mWidth = 185;
		mHeight = 153;
		base.Init(l);
		gBulletImages[0] = Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_BALL);
		gBulletImages[1] = Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_RUNE_LEFT);
		if (mTutorialState == 1)
		{
			for (int i = 0; i < mWalls.Count; i++)
			{
				mWalls[i].mAlphaFadeDir = -1;
				mWalls[i].mAlpha = 0;
			}
			for (int j = 0; j < mTikis.Count; j++)
			{
				mTikis[j].mAlphaFadeDir = -1;
				mTikis[j].mAlpha = 0;
			}
			mTauntQueue.Clear();
		}
		mShieldCompMgr = GameApp.gApp.LoadComposition("pax\\BossDoctorShields", "_BOSS_DOCTOR");
		mTikiCompMgr = GameApp.gApp.LoadComposition(GameApp.gApp.Is3DAccelerated() ? "pax\\Tikis" : "pax\\Tikis2D", "_BOSS_DOCTOR");
		List<Composition> list = new List<Composition>();
		mShieldCompMgr.GetAllCompositions(list);
		mTikiCompMgr.GetAllCompositions(list);
		for (int k = 0; k < list.Count; k++)
		{
			list[k].mLoop = true;
		}
		for (int m = 0; m < mTikis.Count; m++)
		{
			mTikis[m].mComp = mTikiCompMgr.GetComposition(mTikis[m].mIsLeftTiki ? "CircleTikiAnim" : "TriangleTikiAnim");
		}
		mTriangleExplosion = GameApp.gApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_TRIANGLEEXPLOSIONLINE").Duplicate();
		mCircleExplosion = GameApp.gApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_CIRCLEEXPLOSIONLINE").Duplicate();
		mTriangleExplosion.ResetAnim();
		mCircleExplosion.ResetAnim();
		mBandagedImg = Res.GetImageByID(ResID.IMAGE_BOSS_DOCTOR_BANDAGED);
	}

	public override Boss Instantiate()
	{
		BossDoctor bossDoctor = new BossDoctor(mLevel);
		int num = bossDoctor.mTutorialState;
		bossDoctor.CopyFrom(this);
		bossDoctor.mTutorialState = num;
		bossDoctor.mParticles.Clear();
		bossDoctor.mTikis.Clear();
		return bossDoctor;
	}

	public void CopyFrom(BossDoctor rhs)
	{
		CopyFrom((BossShoot)rhs);
		mMaterializeTimer = rhs.mMaterializeTimer;
		mTikiCompMgr = rhs.mTikiCompMgr;
		mShieldCompMgr = rhs.mShieldCompMgr;
		mTriangleExplosion = rhs.mTriangleExplosion;
		mCircleExplosion = rhs.mCircleExplosion;
		mDoCircleExplosion = rhs.mDoCircleExplosion;
		mDoTriangleExplosion = rhs.mDoTriangleExplosion;
		mIsFiring = rhs.mIsFiring;
		mIsRetaliating = rhs.mIsRetaliating;
		mDoingHitAnim = rhs.mDoingHitAnim;
		mHairCel = rhs.mHairCel;
		mFaceCel = rhs.mFaceCel;
		mHitAnimUp = rhs.mHitAnimUp;
		mBossYOff = rhs.mBossYOff;
		mBerserkBaseAlpha = rhs.mBerserkBaseAlpha;
		mBerserkEyeFrame = rhs.mBerserkEyeFrame;
		mBerserkCounter = rhs.mBerserkCounter;
		mBerserkBaseAlphaDir = rhs.mBerserkBaseAlphaDir;
		mResGroup = rhs.mResGroup;
		mResPrefix = rhs.mResPrefix;
		mBossRadius = rhs.mBossRadius;
		mBulletRadius = rhs.mBulletRadius;
		mBandagedXOff = rhs.mBandagedXOff;
		mBandagedYOff = rhs.mBandagedYOff;
		mTutorialState = rhs.mTutorialState;
	}

	public override bool AllowFrogToFire()
	{
		if (mTutorialState == 0)
		{
			return base.AllowFrogToFire();
		}
		if (mTutorialState == 1)
		{
			return mLevel.HasReachedCruisingSpeed();
		}
		return false;
	}
}
