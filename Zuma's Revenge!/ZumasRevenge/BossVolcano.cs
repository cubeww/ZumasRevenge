using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class BossVolcano : BossShoot
{
	private static int BV_WIDTH = 225;

	private static int BV_HEIGHT = 225;

	private static int NUM_WING_FRAMES = 4;

	private static int[] WING_CELS = new int[4] { 1, 2, 1, 0 };

	private static int NUM_HAND_FRAMES = 4;

	private static int[] HAND_CELS = new int[4] { 1, 2, 3, 0 };

	private static int NUM_HIT_FRAMES = 4;

	private static int[] HIT_TIMES = new int[4] { 8, 8, 8, 15 };

	public bool mIntro;

	protected PIEffect mBoilingLava;

	protected int mWingIndex;

	protected int mLeftHandIndex;

	protected int mRightHandIndex = 2;

	protected int mJawCount;

	protected int mHitCel = -1;

	protected float mJawYOff;

	protected float mJawRate;

	protected bool mAnimateHands;

	protected override void BossBulletDestroyed(BossBullet b, bool outofscreen)
	{
		if (b.mData != null)
		{
			PIEffect fx = (PIEffect)b.mData;
			mApp.ReleaseVolcanoEffect(fx);
		}
	}

	protected override void DrawBossSpecificArt(Graphics g)
	{
		float num = mX - (float)mWidth / 2f + (float)mShakeXAmt;
		float num2 = mY - (float)mHeight / 2f + (float)mShakeYAmt;
		g.PushState();
		if (!SexyFramework.Common._geq(mAlphaOverride, 255f))
		{
			g.SetColor(255, 255, 255, (int)mAlphaOverride);
			g.SetColorizeImages(colorizeImages: true);
		}
		if (mHitCel == -1)
		{
			g.DrawImageCel(Res.GetImageByID(ResID.IMAGE_BOSS_VOLCANO_WINGS), (int)Common._S(num + (float)Common._M(28)), (int)Common._S(num2 + (float)Common._M1(39)), WING_CELS[mWingIndex]);
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_VOLCANO_HEAD_BOWL), (int)Common._S(num + (float)Common._M(77)), (int)Common._S(num2 + (float)Common._M1(36)));
			g.PushState();
			if (mBoilingLava == null)
			{
				mBoilingLava = mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_BOILING_DEVIL_HEAD");
				mBoilingLava.mEmitAfterTimeline = true;
			}
			mBoilingLava.mColor.mAlpha = (int)mAlphaOverride;
			mBoilingLava.Draw(g);
			g.PopState();
			g.DrawImageCel(Res.GetImageByID(ResID.IMAGE_BOSS_VOLCANO_HAND), (int)Common._S(num + (float)Common._M(55)), (int)Common._S(num2 + (float)Common._M1(87)), HAND_CELS[mLeftHandIndex]);
			g.DrawImageMirror(Res.GetImageByID(ResID.IMAGE_BOSS_VOLCANO_HAND), (int)Common._S(num + (float)Common._M(135)), (int)Common._S(num2 + (float)Common._M1(87)), Res.GetImageByID(ResID.IMAGE_BOSS_VOLCANO_HAND).GetCelRect(HAND_CELS[mRightHandIndex]));
		}
		if (mTeleportDir != 0)
		{
			g.PushState();
			g.ClearClipRect();
		}
		if (!mLevel.mBoard.IsPaused())
		{
			for (int i = 0; i < mBullets.Count; i++)
			{
				BossBullet bossBullet = mBullets[i];
				if (bossBullet.mDelay <= 0 && bossBullet.mOffscreenPause > 0)
				{
					PIEffect pIEffect = (PIEffect)bossBullet.mData;
					g.PushState();
					g.ClipRect(0, 0, GameApp.gApp.mWidth, Common._DS(Common._M(200)));
					pIEffect.mColor.mAlpha = (int)mAlphaOverride;
					pIEffect.Draw(g);
					g.PopState();
				}
			}
		}
		if (mTeleportDir != 0)
		{
			g.PopState();
		}
		if (mHitCel == -1)
		{
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_VOLCANO_HEAD), (int)Common._S(num + (float)Common._M(55)), (int)Common._S(num2 + (float)Common._M1(23)));
			g.DrawImage(Res.GetImageByID(ResID.IMAGE_BOSS_VOLCANO_JAW), (int)Common._S(num + (float)Common._M(79)), (int)(Common._S(num2 + (float)Common._M1(143)) + mJawYOff));
		}
		else
		{
			g.DrawImageCel(Res.GetImageByID(ResID.IMAGE_BOSS_VOLCANO_HIT), (int)Common._S(num + (float)Common._M(-12)), (int)Common._S(num2 + (float)Common._M1(19)), mHitCel);
		}
		if (!mLevel.mBoard.IsPaused())
		{
			for (int j = 0; j < mBullets.Count; j++)
			{
				BossBullet bossBullet2 = mBullets[j];
				if (bossBullet2.mDelay <= 0 && bossBullet2.mOffscreenPause <= 0)
				{
					PIEffect pIEffect2 = (PIEffect)bossBullet2.mData;
					g.PushState();
					pIEffect2.mColor.mAlpha = 255;
					pIEffect2.Draw(g);
					g.PopState();
				}
			}
		}
		g.PopState();
	}

	protected override bool DoHit(Bullet b, bool from_prox_bomb)
	{
		if (mHitCel == -1)
		{
			mHitCel = 0;
		}
		bool flag = base.DoHit(b, from_prox_bomb);
		if (flag)
		{
			mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BOSS_DEVIL_HIT));
		}
		if (flag && mHP <= 0f)
		{
			mApp.GetBoard().mContinueNextLevelOnLoadProfile = true;
			mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BOSS_DEVIL_DEATH));
		}
		return flag;
	}

	protected override Rect GetBulletRect(BossBullet b)
	{
		return new Rect((int)b.mX - Common._M(15), (int)b.mY + Common._M1(20), Common._M2(20), Common._M3(55));
	}

	protected override bool CheckBulletHitPlayer(BossBullet b)
	{
		if (!b.mCanHitPlayer)
		{
			return false;
		}
		float y = mLevel.mFrog.GetCenterY() - 5;
		float x = mLevel.mFrog.GetCenterX() + 2;
		return MathUtils.CirclesIntersect(x, y, b.mX, b.mY, mBossRadius + Common._M(10));
	}

	protected override void BulletHitPlayer(BossBullet b)
	{
		SoundAttribs soundAttribs = new SoundAttribs();
		soundAttribs.fadeout = 0.1f;
		mApp.mSoundPlayer.Loop(Res.GetSoundByID(ResID.SOUND_NEW_BURNINGFROGLOOP), soundAttribs);
		mApp.mSoundPlayer.Play(Res.GetSoundByID(ResID.SOUND_NEW_FIREHITFROG));
		if (mApp.GetLevelMgr().mBossesCanAttackFuckedFrog)
		{
			return;
		}
		for (int i = 0; i < mBullets.Count; i++)
		{
			BossBullet bossBullet = mBullets[i];
			if (bossBullet.mOffscreenPause > 0)
			{
				bossBullet.mDeleteInstantly = true;
			}
		}
	}

	protected override BossBullet CreateBossBullet()
	{
		BossBullet bossBullet = base.CreateBossBullet();
		bossBullet.mData = mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_DEVIL_PROJECTILE").Duplicate();
		return bossBullet;
	}

	protected override void DidFire()
	{
		mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BOSS_DEVIL_FIRES));
	}

	public BossVolcano(Level l)
		: base(l)
	{
		mTauntTextYOff = Common._DS(Common._M(20));
		mBoilingLava = null;
		mBulletsUseSphereColl = true;
		mBossRadius = Common._M(70);
		mBulletRadius = Common._M(25);
		mDrawDeathBGTikis = false;
	}

	public BossVolcano()
		: this(null)
	{
	}

	public override void Dispose()
	{
		base.Dispose();
		for (int i = 0; i < mBullets.Count; i++)
		{
			BossBullet bossBullet = mBullets[i];
			if (bossBullet.mData != null)
			{
				mApp.ReleaseVolcanoEffect((PIEffect)bossBullet.mData);
			}
		}
		mBoilingLava = null;
	}

	protected void CopyFrom(BossVolcano rhs)
	{
		CopyFrom((BossShoot)rhs);
		mBoilingLava = rhs.mBoilingLava;
		mWingIndex = rhs.mWingIndex;
		mLeftHandIndex = rhs.mLeftHandIndex;
		mRightHandIndex = rhs.mRightHandIndex;
		mJawCount = rhs.mJawCount;
		mHitCel = rhs.mHitCel;
		mJawYOff = rhs.mJawYOff;
		mJawRate = rhs.mJawRate;
		mAnimateHands = rhs.mAnimateHands;
		mIntro = rhs.mIntro;
	}

	public override void Update(float f)
	{
		if (!mIntro)
		{
			base.Update(f);
		}
		else
		{
			mUpdateCount++;
		}
		Common._M(0.3f);
		Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_VOLCANO_HIT);
		if (mHitCel >= 0 && !mIntro && mUpdateCount % HIT_TIMES[mHitCel] == 0 && ++mHitCel >= imageByID.mNumCols)
		{
			mHitCel = -1;
		}
		if (mBoilingLava == null)
		{
			mBoilingLava = mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_BOILING_DEVIL_HEAD");
			mBoilingLava.mEmitAfterTimeline = true;
		}
		mBoilingLava.mDrawTransform.LoadIdentity();
		float num = GameApp.DownScaleNum(1.4f);
		mBoilingLava.mDrawTransform.Scale(num, num);
		mBoilingLava.mDrawTransform.Translate(Common._S(mX + (float)Common._M(10)), Common._S(mY + (float)Common._M1(-40)));
		mBoilingLava.Update();
		if (mHP > 0f)
		{
			if (mUpdateCount % Common._M(15) == 0)
			{
				mWingIndex = (mWingIndex + 1) % NUM_WING_FRAMES;
			}
			if (mUpdateCount % Common._M(8) == 0 && mAnimateHands)
			{
				mLeftHandIndex = (mLeftHandIndex + 1) % NUM_HAND_FRAMES;
				mRightHandIndex = (mRightHandIndex + 1) % NUM_HAND_FRAMES;
				if (mLeftHandIndex == 0)
				{
					mAnimateHands = false;
				}
			}
			if (mJawRate == 0f && SexyFramework.Common.Rand(400) == 0)
			{
				mJawRate = Common._M(-1f);
			}
			if (SexyFramework.Common.Rand(100) == 0 && mLeftHandIndex == 0)
			{
				mAnimateHands = true;
			}
		}
		if (mJawRate != 0f)
		{
			mJawYOff += mJawRate;
			if (mJawRate < 0f && mJawYOff <= -8f)
			{
				mJawYOff = -8f;
				mJawRate *= -1f;
			}
			else if (mJawRate > 0f && mJawYOff >= 0f)
			{
				if (++mJawCount == 2)
				{
					mJawCount = 0;
					mJawYOff = (mJawRate = 0f);
				}
				else
				{
					mJawYOff = 0f;
					mJawRate *= -1f;
				}
			}
		}
		if (mIntro || !SexyFramework.Common._geq(mAlphaOverride, 255f))
		{
			return;
		}
		for (int i = 0; i < mBullets.Count; i++)
		{
			BossBullet bossBullet = mBullets[i];
			PIEffect pIEffect = (PIEffect)bossBullet.mData;
			if (pIEffect != null && !pIEffect.mEmitAfterTimeline)
			{
				pIEffect.mEmitAfterTimeline = true;
				Common.SetFXNumScale(pIEffect, 3f);
			}
			if (bossBullet.mState == 0 && bossBullet.mDelay <= 0)
			{
				bossBullet.mState++;
				bossBullet.mCanHitPlayer = false;
				bool flag = mX > mDestX;
				bossBullet.mX = mX + (float)(flag ? Common._M(5) : Common._M1(30));
				bossBullet.mY = mY + (float)Common._M(50);
				pIEffect.mDrawTransform.LoadIdentity();
				num = GameApp.DownScaleNum(1.4f);
				pIEffect.mDrawTransform.Scale(num, num);
				pIEffect.mDrawTransform.Scale(1f, -1f);
				pIEffect.mDrawTransform.Translate(Common._S(bossBullet.mX + (float)Common._M(0)), Common._S(bossBullet.mY + (float)Common._M1(0)));
				pIEffect.Update();
			}
			else if (bossBullet.mState == 1 && bossBullet.mY >= (float)(mLevel.mFrog.GetCenterY() - Common._M(0)))
			{
				bossBullet.mData = null;
				bossBullet.mState++;
				bossBullet.mCanHitPlayer = true;
				bossBullet.mVY = (bossBullet.mTargetVY = 0f);
				PIEffect pIEffect2 = mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_DEVIL_EXPLOSION").Duplicate();
				pIEffect2.mEmitAfterTimeline = true;
				pIEffect2.mDrawTransform.LoadIdentity();
				num = GameApp.DownScaleNum(1.4f);
				pIEffect2.mDrawTransform.Scale(num, num);
				pIEffect2.mDrawTransform.Translate(Common._S(bossBullet.mX + (float)Common._M(0)), Common._S(bossBullet.mY + (float)Common._M1(0)));
				pIEffect2.Update();
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BOSS_DEVIL_EXPLODES));
				bossBullet.mData = pIEffect2;
			}
			else if (bossBullet.mState == 1)
			{
				pIEffect.mDrawTransform.LoadIdentity();
				num = GameApp.DownScaleNum(1.4f);
				pIEffect.mDrawTransform.Scale(num, num);
				if (bossBullet.mOffscreenPause > 0)
				{
					bool flag2 = mX > mDestX;
					bossBullet.mX = mX + (float)(flag2 ? Common._M(5) : Common._M1(5));
					pIEffect.mDrawTransform.Scale(1f, -1f);
					pIEffect.mDrawTransform.Translate(Common._S(bossBullet.mX + (float)Common._M(0)), Common._S(bossBullet.mY + (float)Common._M1(-30)));
				}
				else
				{
					pIEffect.mDrawTransform.Scale(1f, 1f);
					pIEffect.mDrawTransform.Translate(Common._S(bossBullet.mX + (float)Common._M(0)), Common._S(bossBullet.mY + (float)Common._M1(0)));
				}
				pIEffect.Update();
			}
			else if (bossBullet.mState == 2)
			{
				PIEffect pIEffect3 = (PIEffect)bossBullet.mData;
				pIEffect3.Update();
				if (pIEffect3.mFrameNum > (float)(pIEffect3.mLastFrameNum - Common._M(20)))
				{
					bossBullet.mCanHitPlayer = false;
					mApp.ReleaseVolcanoEffect(pIEffect3);
					bossBullet.mData = null;
					BossBulletDestroyed(bossBullet, outofscreen: false);
					mBullets.RemoveAt(i);
					i--;
				}
			}
		}
	}

	public override void Init(Level l)
	{
		mWidth = Common._M(225);
		mHeight = Common._M(225);
		base.Init(l);
		for (int i = 0; i < Boss.NUM_HEARTS; i++)
		{
			mHeartCels[i] = 0;
		}
	}

	public override Boss Instantiate()
	{
		BossVolcano bossVolcano = new BossVolcano();
		bossVolcano.CopyFrom(this);
		return bossVolcano;
	}

	public override void SyncState(DataSync sync)
	{
		base.SyncState(sync);
		sync.SyncLong(ref mWingIndex);
		sync.SyncLong(ref mLeftHandIndex);
		sync.SyncLong(ref mRightHandIndex);
		sync.SyncLong(ref mJawCount);
		sync.SyncLong(ref mHitCel);
		sync.SyncFloat(ref mJawYOff);
		sync.SyncFloat(ref mJawRate);
		sync.SyncBoolean(ref mAnimateHands);
		Buffer buffer = sync.GetBuffer();
		if (sync.isWrite())
		{
			for (int i = 0; i < mBullets.Count; i++)
			{
				BossBullet bossBullet = mBullets[i];
				PIEffect s = (PIEffect)bossBullet.mData;
				buffer.WriteBoolean(bossBullet.mState == 2);
				Common.SerializePIEffect(s, sync);
			}
			return;
		}
		for (int j = 0; j < mBullets.Count; j++)
		{
			PIEffect pIEffect = null;
			pIEffect = ((!buffer.ReadBoolean()) ? mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_DEVIL_PROJECTILE").Duplicate() : mApp.mResourceManager.GetPIEffect("PIEFFECT_NONRESIZE_DEVIL_EXPLOSION").Duplicate());
			Common.DeserializePIEffect(pIEffect, sync);
			pIEffect.mEmitAfterTimeline = true;
			mBullets[j].mData = pIEffect;
		}
	}
}
