using System;
using System.Collections.Generic;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.PIL;

namespace ZumasRevenge;

public class BossSkeleton : BossShoot
{
	public enum State
	{
		State_Idle,
		State_Firing,
		State_Hit
	}

	private static Image[] gHeadImages = new Image[2];

	protected List<PSystemIntPair> mParticles = new List<PSystemIntPair>();

	protected List<int> mHeadExplosions = new List<int>();

	protected List<Skeleton> mSkeletons = new List<Skeleton>();

	protected int mState;

	protected int mCel;

	protected int mJawCel;

	protected int mTimer;

	protected float mShrunkHeadYOff;

	protected float mHitAlpha;

	public bool mSpawnPowerupWhilePoweredUp;

	public int mSpecialWpnHPDec = 5;

	public int mNumSkeleToEmit;

	public int mSkeleDelay;

	public int mDelayAfterSkeleEmit;

	public int mCurSkeleDelay;

	public int mChanceToSpawnPowerup = 4;

	public float mSkeletonVX;

	public float mSkeletonVY;

	public float mSkeletonEmitX;

	public float mSkeletonEmitY;

	protected void DrawHit(Graphics g, Image img, int x, int y, int cel)
	{
		int num = (int)Math.Min(mHitAlpha, mAlphaOverride);
		if (num > 0 && mHP > 0f && g.Is3D())
		{
			g.PushState();
			g.SetColorizeImages(colorizeImages: true);
			g.SetDrawMode(1);
			g.SetColor(255, 255, 255, num);
			Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_THROW);
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_HIT);
			if (cel == -1)
			{
				g.DrawImage(img, x, y);
			}
			else if (img == imageByID2)
			{
				g.DrawImageCel(img, new Rect(x, y, imageByID.GetCelWidth(), imageByID.GetCelHeight()), cel);
			}
			else
			{
				g.DrawImageCel(img, new Rect(x, y, img.GetCelWidth(), img.GetCelHeight()), cel);
			}
			g.PopState();
		}
	}

	protected void DrawHit(Graphics g, Image img, int x, int y)
	{
		DrawHit(g, img, x, y, -1);
	}

	protected override void DrawBossSpecificArt(Graphics g)
	{
		int num = (int)(mX - (float)(mWidth / 2) + (float)mShakeXOff);
		int num2 = (int)(mY - (float)(mWidth / 2) + (float)mShakeYOff);
		if (mHP > 0f && !mDoDeathExplosions && !mLevel.mBoard.IsPaused())
		{
			for (int i = 0; i < mSkeletons.size(); i++)
			{
				Skeleton skeleton = mSkeletons[i];
				skeleton.Draw(g);
			}
		}
		if (mAlphaOverride < 255f)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, (int)mAlphaOverride);
		}
		if (mState == 0 || mState == 1)
		{
			Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_BODY);
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_JAW);
			Image imageByID3 = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_THROW);
			g.DrawImage(imageByID, Common._S(num + Common._M(38)), Common._S(num2 + Common._M1(78)));
			DrawHit(g, imageByID, Common._S(num + Common._M(38)), Common._S(num2 + Common._M1(78)));
			g.DrawImageCel(imageByID2, Common._S(num + Common._M(73)), Common._S(num2 + Common._M1(96)), mJawCel);
			DrawHit(g, imageByID2, Common._S(num + Common._M(73)), Common._S(num2 + Common._M1(96)), mJawCel);
			DrawBullets(g);
			if (mState == 1 && mCel < 7)
			{
				g.PushState();
				g.ClipRect(Common._S(num), Common._S(num2 + Common._M(38)), Common._S(200), Common._S(200));
				Image imageByID4 = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_SHRUNK_HEAD1);
				g.DrawImage(imageByID4, Common._S(num + Common._M(70)), (int)((float)(Common._S(num2 + Common._M1(94)) - imageByID4.GetHeight()) + mShrunkHeadYOff));
				g.PopState();
			}
			g.DrawImageCel(imageByID3, Common._S(num), Common._S(num2), mCel);
			DrawHit(g, imageByID3, Common._S(num), Common._S(num2), mCel);
		}
		else
		{
			Image imageByID5 = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_HIT);
			Image imageByID6 = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_THROW);
			g.DrawImageCel(imageByID5, new Rect(Common._S(num), Common._S(num2), imageByID6.GetCelWidth(), imageByID6.GetCelHeight()), mCel);
			DrawHit(g, imageByID5, Common._S(num + Common._M(0)), Common._S(num2 + Common._M1(0)), mCel);
			DrawBullets(g);
		}
		g.SetColorizeImages(colorizeImages: false);
	}

	protected override bool PreBulletUpdate(BossBullet b, int index)
	{
		if (b.mDelay > 0)
		{
			b.mDelay--;
		}
		else if (b.mState == 0 && mState == 0 && mJawCel == 0)
		{
			mState = 1;
			mTimer = 0;
			b.mState = 1;
			PlaySound(2);
		}
		if (b.mState <= 1)
		{
			return true;
		}
		return false;
	}

	protected virtual void DrawBullets(Graphics g)
	{
		g.PushState();
		if (mTeleportDir != 0)
		{
			g.PushState();
			g.ClearClipRect();
		}
		if (mHP <= 0f || mDoDeathExplosions || mLevel.mBoard.IsPaused())
		{
			return;
		}
		for (int i = 0; i < mParticles.size(); i++)
		{
			mParticles[i].first.Draw(g);
		}
		for (int j = 0; j < mBullets.size(); j++)
		{
			BossBullet bossBullet = mBullets[j];
			if (bossBullet.mDelay <= 0 && bossBullet.mState >= 2)
			{
				g.DrawImage(gHeadImages[bossBullet.mImageNum], (int)(Common._S(bossBullet.mX) - (float)(gHeadImages[bossBullet.mImageNum].mWidth / 2)), (int)(Common._S(bossBullet.mY) - (float)(gHeadImages[bossBullet.mImageNum].mHeight / 2)));
			}
		}
		if (SexyFramework.Common._geq(mAlphaOverride, 255f))
		{
			Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_HEAD_BURST);
			for (int k = 0; k < mHeadExplosions.size(); k++)
			{
				Rect celRect = imageByID.GetCelRect(mHeadExplosions[k]);
				int theWidth = celRect.mWidth * 10;
				int theHeight = celRect.mHeight * 10;
				g.DrawImage(imageByID, new Rect(Common._S(mLevel.mFrog.GetCenterX()) - imageByID.GetCelWidth() / 2, Common._S(mLevel.mFrog.GetCenterY()) - imageByID.GetCelHeight() / 2, theWidth, theHeight), celRect);
			}
		}
		if (mTeleportDir != 0)
		{
			g.PopState();
		}
		g.PopState();
	}

	protected override Rect GetBulletRect(BossBullet b)
	{
		int num = (int)((float)gHeadImages[b.mImageNum].mWidth * 0.75f);
		int num2 = (int)((float)gHeadImages[b.mImageNum].mHeight * 0.75f);
		return new Rect((int)b.mX - num / 2, (int)b.mY - num2 / 2, num, num2);
	}

	protected override bool DoHit(Bullet b, bool from_prox_bomb)
	{
		if (mState != 2)
		{
			mState = 2;
			mCel = 0;
			mJawCel = 0;
			mShrunkHeadYOff = 0f;
			for (int i = 0; i < mBullets.size(); i++)
			{
				if (mBullets[i].mState == 1)
				{
					mBullets[i].mState = 0;
				}
			}
		}
		int num = (int)base.mHPDecPerHit;
		int num2 = mHeartPieceDecAmt;
		if (b != null && b.GetIsCannon())
		{
			mHitAlpha = 255f;
			base.mHPDecPerHit = mSpecialWpnHPDec;
			mHeartPieceDecAmt = (int)((float)(Boss.NUM_HEARTS * 4) / (mMaxHP / base.mHPDecPerHit));
		}
		bool result = base.DoHit(b, from_prox_bomb);
		base.mHPDecPerHit = num;
		mHeartPieceDecAmt = num2;
		return result;
	}

	protected override void BulletHitPlayer(BossBullet b)
	{
		base.BulletHitPlayer(b);
		mHeadExplosions.Add(0);
		for (int i = 0; i < mParticles.size(); i++)
		{
			if (mParticles[i].second == b.mId)
			{
				if (mParticles[i].first != null)
				{
					mParticles[i].first.Dispose();
					mParticles[i].first = null;
				}
				mParticles.RemoveAt(i);
				break;
			}
		}
		if (mApp.GetLevelMgr().mBossesCanAttackFuckedFrog)
		{
			return;
		}
		for (int j = 0; j < mBullets.size(); j++)
		{
			BossBullet bossBullet = mBullets[j];
			if (bossBullet.mDelay > 0)
			{
				bossBullet.mDeleteInstantly = true;
			}
		}
	}

	protected override bool CanFire()
	{
		return mState != 2;
	}

	protected virtual void AddParticleSystem(BossBullet b)
	{
		SexyFramework.PIL.System system = new SexyFramework.PIL.System(75, 50);
		system.mScale = Common._S(1f);
		system.WaitForEmitters(w: true);
		mParticles.Add(new PSystemIntPair(system, b.mId));
		Emitter emitter = new Emitter();
		emitter.mCullingRect = new Rect(0, 0, Common._SS(mApp.mWidth), Common._SS(mApp.mHeight));
		emitter.mEmissionCoordsAreOffsets = true;
		EmitterScale emitterScale = new EmitterScale();
		emitterScale.mLifeScale = Common._M(1f);
		emitterScale.mNumberScale = Common._M(1f);
		emitterScale.mSizeXScale = Common._M(0.91f);
		emitterScale.mVelocityScale = Common._M(1.5f);
		emitterScale.mWeightScale = Common._M(3f);
		emitterScale.mZoom = Common._M(3.63f);
		emitter.AddScaleKeyFrame(0, emitterScale);
		EmitterSettings emitterSettings = new EmitterSettings();
		emitterSettings.mAngle = SexyFramework.Common.DegreesToRadians(Common._M(-73));
		emitter.SetEmitterType(1);
		emitter.AddLineEmitterKeyFrame(0, 0, new Point(0, 0));
		emitter.AddLineEmitterKeyFrame(1, 0, new Point(Common._M(32), 0));
		ParticleType particleType = new ParticleType();
		particleType.mImage = Res.GetImageByID(ResID.IMAGE_PARTICLE_FUZZY_CIRCLE);
		ParticleSettings particleSettings = new ParticleSettings();
		particleSettings.mLife = Common._M(5);
		particleSettings.mNumber = Common._M(100);
		particleSettings.mXSize = Common._M(8);
		particleSettings.mWeight = Common._M(0);
		particleType.AddSettingsKeyFrame(0, particleSettings);
		ParticleVariance particleVariance = new ParticleVariance();
		particleVariance.mNumberVar = Common._M(0);
		particleVariance.mSizeXVar = Common._M(7);
		particleVariance.mMotionRandVar = Common._M(44);
		particleType.AddVarianceKeyFrame(0, particleVariance);
		LifetimeSettings lifetimeSettings = new LifetimeSettings();
		lifetimeSettings.mSizeXMult = 1.6f;
		particleType.AddSettingAtLifePct(0f, lifetimeSettings);
		lifetimeSettings = new LifetimeSettings(lifetimeSettings);
		lifetimeSettings.mSizeXMult = 1.4f;
		particleType.AddSettingAtLifePct(0.42f, lifetimeSettings);
		lifetimeSettings = new LifetimeSettings(lifetimeSettings);
		lifetimeSettings.mSizeXMult = 1.2f;
		particleType.AddSettingAtLifePct(0.62f, lifetimeSettings);
		lifetimeSettings = new LifetimeSettings(lifetimeSettings);
		lifetimeSettings.mSizeXMult = 0f;
		particleType.AddSettingAtLifePct(1f, lifetimeSettings);
		particleType.mAdditive = true;
		particleType.mColorKeyManager.AddColorKey(0f, new Color(128, 124, 60));
		particleType.mColorKeyManager.AddColorKey(0.5f, new Color(57, 158, 44));
		particleType.mColorKeyManager.AddColorKey(1f, new Color(253, 253, 0));
		particleType.mAlphaKeyManager.AddAlphaKey(0f, 56);
		particleType.mAlphaKeyManager.AddAlphaKey(Common._M(0.9f), 255);
		particleType.mAlphaKeyManager.AddAlphaKey(1f, 0);
		emitter.AddParticleType(particleType);
		system.AddEmitter(emitter);
	}

	protected override void BossBulletDestroyed(BossBullet b, bool outofscreen)
	{
		for (int i = 0; i < mParticles.size(); i++)
		{
			if (mParticles[i].second == b.mId)
			{
				mParticles[i].first.ForceStopEmitting(f: true);
				mParticles[i].second = -1;
				break;
			}
		}
	}

	public BossSkeleton(Level l)
		: base(l)
	{
		mSkeletonEmitX = Common._S(Common._M(-200));
		mWidth = 196;
		mHeight = 194;
		mBossRadius = Common._M(58);
		mBulletRadius = Common._M(25);
		mResGroup = "Boss3";
		mResPrefix = "IMAGE_BOSS_SKELETON_";
		mExplosionRate = Common._M(4);
		mHitEffectYOff = Common._M(10);
		mDrawHeartsBelowBoss = true;
		mBandagedXOff = Common._M(-8);
	}

	public BossSkeleton()
		: this(null)
	{
	}

	public override void Dispose()
	{
		base.Dispose();
		for (int i = 0; i < mSkeletons.Count; i++)
		{
			if (mSkeletons[i] != null)
			{
				mSkeletons[i].Dispose();
				mSkeletons[i] = null;
			}
		}
		for (int j = 0; j < mParticles.Count; j++)
		{
			if (mParticles[j].first != null)
			{
				mParticles[j].first.Dispose();
				mParticles[j].first = null;
			}
		}
		mSkeletons.Clear();
		mParticles.Clear();
	}

	public void CopyFrom(BossSkeleton rhs)
	{
		CopyFrom((BossShoot)rhs);
		mHeadExplosions.AddRange(rhs.mHeadExplosions.ToArray());
		mState = rhs.mState;
		mCel = rhs.mCel;
		mJawCel = rhs.mJawCel;
		mTimer = rhs.mTimer;
		mShrunkHeadYOff = rhs.mShrunkHeadYOff;
		mHitAlpha = rhs.mHitAlpha;
		mSpawnPowerupWhilePoweredUp = rhs.mSpawnPowerupWhilePoweredUp;
		mSpecialWpnHPDec = rhs.mSpecialWpnHPDec;
		mNumSkeleToEmit = rhs.mNumSkeleToEmit;
		mSkeleDelay = rhs.mSkeleDelay;
		mDelayAfterSkeleEmit = rhs.mDelayAfterSkeleEmit;
		mCurSkeleDelay = rhs.mCurSkeleDelay;
		mChanceToSpawnPowerup = rhs.mChanceToSpawnPowerup;
		mSkeletonVX = rhs.mSkeletonVX;
		mSkeletonVY = rhs.mSkeletonVY;
		mSkeletonEmitX = rhs.mSkeletonEmitX;
		mSkeletonEmitY = rhs.mSkeletonEmitY;
	}

	public override void MouseDownDuringNoFire(int x, int y)
	{
	}

	public override bool AllowFrogToFire()
	{
		return base.AllowFrogToFire();
	}

	public override void SyncState(DataSync sync)
	{
		base.SyncState(sync);
		sync.SyncLong(ref mState);
		sync.SyncLong(ref mCurSkeleDelay);
		sync.SyncLong(ref mCel);
		sync.SyncLong(ref mTimer);
		sync.SyncLong(ref mJawCel);
		sync.SyncFloat(ref mShrunkHeadYOff);
		if (sync.isRead())
		{
			mSkeletons.Clear();
		}
		SyncListSkeletons(sync, mSkeletons, clear: true);
		SexyFramework.Misc.Buffer buffer = sync.GetBuffer();
		if (sync.isWrite())
		{
			buffer.WriteLong(mParticles.Count);
			for (int i = 0; i < mParticles.Count; i++)
			{
				buffer.WriteLong(mParticles[i].second);
				Common.SerializeParticleSystem(mParticles[i].first, sync);
			}
			return;
		}
		mParticles.Clear();
		int num = (int)buffer.ReadLong();
		for (int j = 0; j < num; j++)
		{
			int s = (int)buffer.ReadLong();
			SexyFramework.PIL.System f = Common.DeserializeParticleSystem(sync);
			mParticles.Add(new PSystemIntPair(f, s));
		}
	}

	private void SyncListSkeletons(DataSync sync, List<Skeleton> theList, bool clear)
	{
		if (sync.isRead())
		{
			if (clear)
			{
				theList.Clear();
			}
			long num = sync.GetBuffer().ReadLong();
			for (int i = 0; i < num; i++)
			{
				Skeleton skeleton = new Skeleton();
				skeleton.SyncState(sync);
				theList.Add(skeleton);
			}
			return;
		}
		sync.GetBuffer().WriteLong(theList.Count);
		foreach (Skeleton the in theList)
		{
			the.SyncState(sync);
		}
	}

	public override bool Collides(Bullet b)
	{
		float num = (float)b.GetRadius() * Common._M(0.75f);
		Rect rect = new Rect((int)(b.GetX() - num), (int)(b.GetY() - num), (int)(num * 2f), (int)(num * 2f));
		Skeleton skeleton = null;
		for (int i = 0; i < mSkeletons.size(); i++)
		{
			skeleton = mSkeletons[i];
			if (skeleton.mActivated)
			{
				continue;
			}
			Rect theTRect = new Rect((int)(skeleton.mX + (float)Common._M(32)), (int)(skeleton.mY + (float)Common._M1(11)), Common._M2(45), Common._M3(79));
			if (rect.Intersects(theTRect))
			{
				if (skeleton.mHasPowerup)
				{
					mLevel.mFrog.SetCannonCount(1, stack: false, SexyFramework.Common.Rand() % 4, Common._M(0));
					skeleton.DoHit();
					mLevel.mFrog.AddPowerOrb(new SkeletonPowerOrb());
					mLevel.mBoard.ForceFlipFrog();
					float max_radius = Common._M(50);
					float alpha_fade = Common._M(8f);
					float size_fade = Common._M(0.01f);
					float angle_inc = Common._M(0.1f);
					mLevel.mFrog.AddPowerRing(new OrbPowerRing(0f, max_radius, alpha_fade, size_fade, angle_inc));
					mLevel.mFrog.AddPowerRing(new OrbPowerRing(3.14159f, max_radius, alpha_fade, size_fade, angle_inc));
				}
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_SKELETON_HIT));
				skeleton.mActivated = true;
				return !b.GetIsCannon();
			}
		}
		return base.Collides(b);
	}

	public override void Update(float f)
	{
		base.Update(f);
		for (int i = 0; i < mParticles.size(); i++)
		{
			SexyFramework.PIL.System first = mParticles[i].first;
			first.Update();
			if (mParticles[i].second != -1)
			{
				for (int j = 0; j < mBullets.size(); j++)
				{
					if (mBullets[j].mId == mParticles[i].second)
					{
						first.SetPos(mBullets[j].mX + (float)Common._M(-15), mBullets[j].mY + (float)Common._M1(0));
						break;
					}
				}
			}
			if (first.Done())
			{
				first.Dispose();
				mParticles.RemoveAt(i);
				i--;
			}
		}
		if (mHitAlpha > 0f)
		{
			mHitAlpha -= Common._M(2f);
		}
		if (mDoDeathExplosions || mHP <= 0f || mLevel.mBoard.DoingBossIntro() || !mLevel.AllCurvesAtRolloutPoint())
		{
			return;
		}
		Bullet bullet = mLevel.mFrog.GetBullet();
		if (mNumSkeleToEmit != 0 && --mCurSkeleDelay <= 0 && SexyFramework.Common._geq(mAlphaOverride, 255f))
		{
			mCurSkeleDelay = mDelayAfterSkeleEmit;
			int num = ((mNumSkeleToEmit == -1) ? 1 : mNumSkeleToEmit);
			for (int k = 0; k < num; k++)
			{
				Skeleton skeleton = new Skeleton();
				skeleton.mDelay = k * mSkeleDelay;
				skeleton.mX = mSkeletonEmitX;
				skeleton.mY = mSkeletonEmitY;
				skeleton.mVX = mSkeletonVX;
				skeleton.mVY = mSkeletonVY;
				if (!mSpawnPowerupWhilePoweredUp && bullet != null && bullet.GetIsCannon())
				{
					skeleton.mHasPowerup = false;
				}
				else if (SexyFramework.Common.Rand(100) < mChanceToSpawnPowerup)
				{
					skeleton.mHasPowerup = true;
				}
				else
				{
					skeleton.mHasPowerup = false;
				}
				mSkeletons.Add(skeleton);
			}
		}
		else if (mAlphaOverride < 255f && mLevel.mBoard.GetGameState() == GameState.GameState_Losing)
		{
			for (int l = 0; l < mSkeletons.size(); l++)
			{
				mSkeletons[l].mFadeOut = true;
				if (mSkeletons[l].mDelay > 0)
				{
					mSkeletons.RemoveAt(l);
					l--;
				}
			}
		}
		Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_MINI_SKELETON);
		for (int m = 0; m < mSkeletons.size(); m++)
		{
			Skeleton skeleton2 = mSkeletons[m];
			if (!SexyFramework.Common._geq(mAlphaOverride, 255f))
			{
				skeleton2.mFadeOut = true;
			}
			skeleton2.Update();
			if ((skeleton2.mVX > 0f && skeleton2.mX > (float)Common._SS(mApp.mWidth)) || (skeleton2.mVX < 0f && skeleton2.mX + (float)imageByID.GetCelWidth() < 0f) || (skeleton2.mVY > 0f && skeleton2.mY > (float)Common._SS(mApp.mHeight)) || (skeleton2.mVY < 0f && skeleton2.mY + (float)imageByID.GetCelHeight() < 0f) || skeleton2.mEffectDone)
			{
				skeleton2.Dispose();
				mSkeletons.RemoveAt(m);
				m--;
			}
		}
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_HEAD_BURST);
		for (int n = 0; n < mHeadExplosions.size(); n++)
		{
			if (mUpdateCount % Common._M(3) == 0)
			{
				mHeadExplosions[n]++;
			}
			if (mHeadExplosions[n] >= imageByID2.GetCelCount())
			{
				mHeadExplosions.RemoveAt(n);
				n--;
			}
		}
		mTimer++;
		Image imageByID3 = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_JAW);
		Image imageByID4 = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_THROW);
		Image imageByID5 = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_HIT);
		if (mState == 1 && SexyFramework.Common._geq(mAlphaOverride, 255f))
		{
			if (mJawCel < imageByID3.mNumCols - 1)
			{
				if (mTimer % Common._M(6) == 0)
				{
					mJawCel++;
				}
				return;
			}
			if (mCel == 0)
			{
				mCel = 1;
			}
			imageByID4.GetCelCount();
			_ = imageByID5.mNumCols;
			_ = imageByID5.mNumRows;
			float num2 = Common._DS(Common._M(140f));
			if (mShrunkHeadYOff < num2)
			{
				mShrunkHeadYOff += Common._DS(Common._M(5f));
				if (mShrunkHeadYOff >= num2)
				{
					mShrunkHeadYOff = num2;
					mCel = 2;
				}
			}
			else if (mCel != 6 && mTimer % Common._M(3) == 0)
			{
				mCel++;
				if (mCel == 3)
				{
					mShrunkHeadYOff += Common._M(5);
				}
				else if (mCel != 4)
				{
					if (mCel == 5)
					{
						mShrunkHeadYOff -= Common._M(3);
					}
					else if (mCel == 6)
					{
						mShrunkHeadYOff -= Common._M(2);
					}
					else if (mCel == imageByID4.GetCelCount())
					{
						mCel = 0;
						mShrunkHeadYOff = 0f;
						mState = 0;
					}
				}
			}
			else
			{
				if (mCel != 6 || mTimer % Common._M(12) != 0)
				{
					return;
				}
				mCel++;
				for (int num3 = 0; num3 < mBullets.size(); num3++)
				{
					BossBullet bossBullet = mBullets[num3];
					if (bossBullet.mState == 1)
					{
						int num4 = (int)(mX - (float)(mWidth / 2));
						int num5 = (int)(mY - (float)(mHeight / 2));
						bossBullet.mState++;
						Image image = gHeadImages[bossBullet.mImageNum];
						bossBullet.mX = num4 + Common._M(70) + imageByID.GetWidth() / 2;
						bossBullet.mY = (float)(num5 + Common._M(94) + (imageByID.GetHeight() / 2 - image.GetHeight())) + mShrunkHeadYOff;
						AddParticleSystem(bossBullet);
						break;
					}
				}
			}
		}
		else if (mState == 0)
		{
			if (mJawCel > 0 && mTimer % Common._M(12) == 0)
			{
				mJawCel--;
			}
		}
		else if (mState == 2 && mTimer % Common._M(2) == 0 && ++mCel >= imageByID5.mNumCols * imageByID5.mNumRows)
		{
			mCel = 0;
			mState = 0;
		}
	}

	public override void Draw(Graphics g)
	{
		base.Draw(g);
	}

	public override void Init(Level l)
	{
		base.Init(l);
		mBandagedImg = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_BANDAGED);
		gHeadImages[0] = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_SHRUNK_HEAD1);
		gHeadImages[1] = Res.GetImageByID(ResID.IMAGE_BOSS_SKELETON_SHRUNK_HEAD2);
	}

	public override Boss Instantiate()
	{
		BossSkeleton bossSkeleton = new BossSkeleton(mLevel);
		bossSkeleton.CopyFrom(this);
		bossSkeleton.mParticles.Clear();
		bossSkeleton.mSkeletons.Clear();
		return bossSkeleton;
	}
}
