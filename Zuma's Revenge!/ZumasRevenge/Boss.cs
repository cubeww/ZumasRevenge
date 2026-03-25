using System;
using System.Collections.Generic;
using System.Linq;
using JeffLib;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public abstract class Boss : IDisposable
{
	public enum Sound
	{
		Sound_Die,
		Sound_Enrage,
		Sound_Fire,
		Sound_BossHit,
		Sound_PlayerHit,
		Sound_Intro,
		Sound_EatBalls,
		Sound_Roar,
		Sound_Teleport,
		Sound_ShieldHit,
		Max_Sounds
	}

	public enum ColorChange
	{
		ColorChange_Never,
		ColorChange_BombInRange,
		ColorChange_NotBehind
	}

	public static float gBerserkTextAlpha;

	public static float gBerserkTextY;

	public static float gImpatientTextAlpha;

	public static float gImpatientTextY;

	protected static int gWackColorFade = 0;

	protected static int gWackColorFadeDir = 2;

	protected static int NUM_HEARTS = 5;

	protected static int FNTT_last_idx = 0;

	protected static int last_idx = 0;

	protected Dictionary<string, ParamData<float>> mFParamPointerMap = new Dictionary<string, ParamData<float>>();

	protected Dictionary<string, ParamData<int>> mIParamPointerMap = new Dictionary<string, ParamData<int>>();

	protected Dictionary<string, ParamData<bool>> mBParamPointerMap = new Dictionary<string, ParamData<bool>>();

	protected ParamData<int> mDWallDownTime = new ParamData<int>();

	protected ParamData<float> mDHPDecPerHit = new ParamData<float>();

	protected ParamData<float> mDHPDecPerProxBomb = new ParamData<float>();

	protected ParamData<int> mDTikiHealthRespawnAmt = new ParamData<int>();

	public bool mShouldDoDeathExplosions;

	public bool mDoDeathExplosions;

	public bool mNeedsIntroSound;

	public bool mEatsBalls;

	public GameApp mApp;

	public Level mLevel;

	public bool mAllowCompacting;

	public int mShakeXAmt;

	public int mShakeYAmt;

	public int mHeartXOff;

	public int mHeartYOff;

	public bool mResetWallsOnBossHit;

	public bool mResetWallTimerOnTikiHit;

	public bool mAllowLevelDDS;

	public bool mDrawRadius;

	public int mRadiusColorChangeMode;

	public int mCurWallDownTime;

	public int mCurrTikiBossHealthRemoved;

	public int mImpatientTimer;

	public int mNum;

	public string mName = "";

	public string mResPrefix = "";

	public int mBombFreqMin;

	public int mBombFreqMax;

	public int mBombDuration;

	public int mProxBombRadius;

	public int mBossRadius;

	public int mBossRadiusYOff;

	public int mVolcanoOffscreenDelay;

	public List<BossText> mDeathText = new List<BossText>();

	public string mWordBubbleText = "";

	public string mSepiaImagePath = "";

	public string mResGroup = "";

	public List<TauntText> mTauntText = new List<TauntText>();

	public DeviceImage mSepiaImage;

	public PIEffect mHitEffect;

	public float mAlphaOverride;

	public List<Tiki> mTikis = new List<Tiki>();

	protected int mTauntTextYOff;

	protected Image mBandagedImg;

	protected int mBandagedXOff;

	protected int mBandagedYOff;

	protected List<HulaDancer> mHulaDancers = new List<HulaDancer>();

	protected List<HulaEntry> mHulaEntryVec = new List<HulaEntry>();

	protected HulaEntry mCurrentHulaEntry = new HulaEntry();

	protected List<BerserkTier> mBerserkTiers = new List<BerserkTier>();

	protected List<BossWall> mWalls = new List<BossWall>();

	protected List<PIEffect> mDeathExplosions = new List<PIEffect>();

	protected List<TauntText> mTauntQueue = new List<TauntText>();

	protected int[] mSounds = new int[10];

	protected int mExplosionRate;

	protected bool mDrawDeathBGTikis;

	protected float mX;

	protected float mY;

	protected float mMaxHP;

	protected float mHP;

	protected float mDeathTX;

	protected float mDeathTY;

	protected float mDeathVX;

	protected float mDeathVY;

	protected int mHulaAmnesty;

	protected int mWidth;

	protected int mHeight;

	protected int mShakeXOff;

	protected int mShakeYOff;

	protected int mUpdateCount;

	protected int mHeartPieceDecAmt;

	protected int mHeartPieceDecAmtProxBomb;

	protected int[] mHeartCels = new int[NUM_HEARTS];

	protected int mStunTime;

	protected int mDeathTimer;

	protected bool mDoExplosion;

	protected bool mNeedsCompacting;

	protected bool mIsBerserk;

	protected bool mBombInRange;

	protected int mWordBubbleTimer;

	protected bool mCleanHeart = true;

	public int mWallDownTime
	{
		get
		{
			return mDWallDownTime.value;
		}
		set
		{
			mDWallDownTime.value = value;
		}
	}

	public float mHPDecPerHit
	{
		get
		{
			return mDHPDecPerHit.value;
		}
		set
		{
			mDHPDecPerHit.value = value;
		}
	}

	public float mHPDecPerProxBomb
	{
		get
		{
			return mDHPDecPerProxBomb.value;
		}
		set
		{
			mDHPDecPerProxBomb.value = value;
		}
	}

	public int mTikiHealthRespawnAmt
	{
		get
		{
			return mDTikiHealthRespawnAmt.value;
		}
		set
		{
			mDTikiHealthRespawnAmt.value = value;
		}
	}

	private void InitParamPointers()
	{
		Dictionary<string, ParamData<float>>.Enumerator enumerator = mFParamPointerMap.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (GameApp.gDDS.HasBossParam(enumerator.Current.Key))
			{
				ParamData<float> paramData = new ParamData<float>();
				paramData.value = GameApp.gDDS.GetBossParam(enumerator.Current.Key);
				mFParamPointerMap[enumerator.Current.Key] = paramData;
			}
		}
		Dictionary<string, ParamData<int>>.Enumerator enumerator2 = mIParamPointerMap.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			if (GameApp.gDDS.HasBossParam(enumerator2.Current.Key))
			{
				ParamData<int> paramData2 = new ParamData<int>();
				paramData2.value = (int)GameApp.gDDS.GetBossParam(enumerator2.Current.Key);
				mIParamPointerMap[enumerator2.Current.Key] = paramData2;
			}
		}
		for (int i = 0; i < mBerserkTiers.size(); i++)
		{
			BerserkTier berserkTier = mBerserkTiers[i];
			for (int j = 0; j < berserkTier.mParams.size(); j++)
			{
				BerserkModifier berserkModifier = berserkTier.mParams[j];
				string key = berserkModifier.mParamName.ToLower();
				if (mFParamPointerMap.ContainsKey(key))
				{
					berserkModifier.AddPointerFloat(mFParamPointerMap[key]);
				}
				if (mIParamPointerMap.ContainsKey(key))
				{
					berserkModifier.AddPointerInt(mIParamPointerMap[key]);
				}
				if (mBParamPointerMap.ContainsKey(key))
				{
					berserkModifier.AddPointerBool(mBParamPointerMap[key]);
				}
			}
		}
	}

	protected virtual void DecHearts(int amount)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_HEARTS);
		for (int i = 0; i < NUM_HEARTS; i++)
		{
			if (mHeartCels[i] < imageByID.mNumCols - 1)
			{
				int num = mHeartCels[i];
				mHeartCels[i] += amount;
				if (mHeartCels[i] <= imageByID.mNumCols - 1)
				{
					break;
				}
				mHeartCels[i] = imageByID.mNumCols - 1;
				amount -= mHeartCels[i] - num;
			}
		}
	}

	protected virtual void ResetWallAndTikis(int wall_index)
	{
		if (!(mHP <= 0f) && mWalls.Count() == mTikis.Count())
		{
			mTikis[wall_index].mWasHit = false;
			mTikis[wall_index].mAlphaFadeDir = 1;
			mWalls[wall_index].mAlphaFadeDir = 1;
		}
	}

	protected virtual bool DoHit(Bullet b, bool from_prox_bomb)
	{
		float mPrevHP = mHP;
		float num = (from_prox_bomb ? mHPDecPerProxBomb : mHPDecPerHit);
		int amount = (from_prox_bomb ? mHeartPieceDecAmtProxBomb : mHeartPieceDecAmt);
		if (num <= 0f)
		{
			return false;
		}
		mHP -= num;
		if (mTikiHealthRespawnAmt > 0 && CanDecTikiHealthSpawnAmt())
		{
			mCurrTikiBossHealthRemoved += (int)num;
			if (mCurrTikiBossHealthRemoved >= mTikiHealthRespawnAmt)
			{
				mCurrTikiBossHealthRemoved = 0;
				for (int i = 0; i < mWalls.Count(); i++)
				{
					ResetWallAndTikis(i);
				}
			}
		}
		if (mHP <= 0f)
		{
			mHP = 0f;
			mDeathTimer = 0;
			PlaySound(0);
			mApp.GetBoard().BossDied();
		}
		else
		{
			PlaySound(3);
		}
		mDoExplosion = true;
		if (mAllowCompacting)
		{
			mNeedsCompacting = true;
		}
		DecHearts(amount);
		if (mHP > 0f)
		{
			CheckIfShouldGoBerserk(mPrevHP);
		}
		else
		{
			mTauntQueue.Clear();
		}
		return true;
	}

	protected virtual bool CompactCurves()
	{
		for (int i = 0; i < mLevel.mNumCurves; i++)
		{
			if (!mLevel.mCurveMgr[i].CanCompact())
			{
				return false;
			}
		}
		for (int j = 0; j < mLevel.mNumCurves; j++)
		{
			mLevel.mCurveMgr[j].CompactCurve(suck_back: false);
		}
		return true;
	}

	protected virtual void DrawHearts(Graphics g)
	{
		if (!(mHP <= 0f) && !mDoDeathExplosions && !mLevel.mBoard.DoingBossIntro())
		{
			g.PushState();
			if (mAlphaOverride <= 254f)
			{
				g.SetColor(255, 255, 255, (int)mAlphaOverride);
				g.SetColorizeImages(colorizeImages: true);
			}
			Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_HEARTS);
			for (int i = 0; i < NUM_HEARTS; i++)
			{
				g.DrawImageCel(imageByID, (int)(Common._S(mX + (float)mHeartXOff) + (float)(i * imageByID.GetCelWidth())), (int)Common._S(mY + (float)mHeartYOff), mHeartCels[i]);
			}
			g.PopState();
		}
	}

	protected virtual void DrawMisc(Graphics g)
	{
		if (mHP > 0f && !mDoDeathExplosions && !mLevel.mBoard.DoingBossIntro())
		{
			for (int i = 0; i < mTikis.size(); i++)
			{
				mTikis[i].Draw(g);
			}
			DrawWalls(g);
		}
		Font fontByID = Res.GetFontByID(ResID.FONT_SHAGLOUNGE28_STROKE);
		if (gBerserkTextAlpha > 0f)
		{
			g.SetFont(fontByID);
			g.SetColor(Common._M(255), Common._M1(0), Common._M2(0), (int)gBerserkTextAlpha);
			string theString = TextManager.getInstance().getString(150);
			int num = g.GetFont().StringWidth(theString);
			g.DrawString(theString, (mApp.mWidth - num) / 2, (int)gBerserkTextY);
		}
		if (gImpatientTextAlpha > 0f)
		{
			g.SetFont(fontByID);
			g.SetColor(Common._M(0), Common._M1(0), Common._M2(0), (int)gImpatientTextAlpha);
			string theString2 = TextManager.getInstance().getString(151);
			int num2 = g.GetFont().StringWidth(theString2);
			g.DrawString(theString2, (mApp.mWidth - num2) / 2, (int)gImpatientTextY);
		}
		if (mHP <= 0f && mBandagedImg != null)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 255, 255 - (int)mAlphaOverride);
			g.DrawImage(mBandagedImg, (int)(Common._S(mX) - (float)(mBandagedImg.mWidth / 2) + (float)mShakeXOff + (float)Common._S(mBandagedXOff)), (int)(Common._S(mY) - (float)(mBandagedImg.mHeight / 2) + (float)mShakeYOff + (float)Common._S(mBandagedYOff)));
			g.SetColorizeImages(colorizeImages: false);
		}
		if (mShouldDoDeathExplosions)
		{
			for (int j = 0; j < mDeathExplosions.size(); j++)
			{
				PIEffect pIEffect = mDeathExplosions[j];
				pIEffect.mDrawTransform.LoadIdentity();
				float num3 = GameApp.DownScaleNum(1f);
				pIEffect.mDrawTransform.Scale(num3, num3);
				pIEffect.mDrawTransform.Translate(Common._S(mX), Common._S(mY));
				pIEffect.Draw(g);
			}
		}
	}

	protected virtual bool BulletIntersectsBoss(Bullet b)
	{
		return MathUtils.CirclesIntersect(b.GetX(), b.GetY(), mX, mY + (float)mBossRadiusYOff, mBossRadius + b.GetRadius());
	}

	protected void AddParamPointer(string p, ParamData<float> v)
	{
		mFParamPointerMap[p.ToLower()] = v;
	}

	protected void AddParamPointer(string p, ParamData<int> v)
	{
		mIParamPointerMap[p.ToLower()] = v;
	}

	protected void AddParamPointer(string p, ParamData<bool> v)
	{
		mBParamPointerMap[p.ToLower()] = v;
	}

	protected void CheckIfShouldGoBerserk(float mPrevHP)
	{
		foreach (BerserkTier mBerserkTier in mBerserkTiers)
		{
			if (mPrevHP >= (float)mBerserkTier.mHealthLimit && mHP < (float)mBerserkTier.mHealthLimit)
			{
				for (int i = 0; i < mBerserkTier.mParams.Count(); i++)
				{
					mBerserkTier.mParams[i].ModifyVariable();
				}
				BerserkActivated(mBerserkTier.mHealthLimit);
				ReInit();
				break;
			}
		}
	}

	protected virtual void ReInit()
	{
		mHeartPieceDecAmt = (int)((float)(NUM_HEARTS * 4) / (mMaxHP / mHPDecPerHit));
		mHeartPieceDecAmtProxBomb = (int)((float)(NUM_HEARTS * 4) / (mMaxHP / mHPDecPerProxBomb));
	}

	protected virtual void BerserkActivated(int health_limit)
	{
		gBerserkTextAlpha = 255f;
		gBerserkTextY = mApp.mHeight / 2;
		mIsBerserk = true;
		PlaySound(1);
		foreach (HulaEntry item in mHulaEntryVec)
		{
			if (item.mBerserkAmt == health_limit)
			{
				mCurrentHulaEntry = item;
				break;
			}
		}
	}

	protected virtual void BallEaten(Bullet b)
	{
	}

	protected virtual bool CanSpawnHulaDancers()
	{
		return true;
	}

	protected virtual void DrawWalls(Graphics g)
	{
	}

	protected virtual Rect GetWallRect(BossWall w)
	{
		return new Rect(w.mX, w.mY, w.mWidth, w.mHeight);
	}

	protected virtual bool CollidesWithWall(Bullet b)
	{
		float num = (float)b.GetRadius() * 0.75f;
		Rect theTRect = new Rect((int)(b.GetX() - num), (int)(b.GetY() - num), (int)(num * 2f), (int)(num * 2f));
		foreach (BossWall mWall in mWalls)
		{
			if (mWall.mAlphaFadeDir >= 0 && GetWallRect(mWall).Intersects(theTRect))
			{
				return true;
			}
		}
		return false;
	}

	protected virtual bool CanDecTikiHealthSpawnAmt()
	{
		return true;
	}

	protected virtual bool CanTaunt()
	{
		return true;
	}

	protected virtual void TikiHit(int idx)
	{
	}

	public Boss()
		: this(null)
	{
	}

	public Boss(Level l)
	{
		mX = 0f;
		mY = 0f;
		mMaxHP = 0f;
		mHP = 0f;
		mWidth = 101;
		mHeight = 78;
		mUpdateCount = 0;
		mHPDecPerHit = 0f;
		mHPDecPerProxBomb = 0f;
		mLevel = l;
		mShakeXAmt = 0;
		mShakeYAmt = 0;
		mShouldDoDeathExplosions = true;
		mShakeXOff = 0;
		mShakeYOff = 0;
		mAllowLevelDDS = false;
		mDoExplosion = false;
		mNeedsCompacting = false;
		mAllowCompacting = false;
		mHeartXOff = 0;
		mHeartYOff = 150;
		mResetWallTimerOnTikiHit = false;
		mResetWallsOnBossHit = false;
		mWallDownTime = 0;
		mCurWallDownTime = 0;
		mStunTime = 0;
		mCurrTikiBossHealthRemoved = 0;
		mTikiHealthRespawnAmt = 0;
		mNum = 0;
		mIsBerserk = false;
		mApp = GameApp.gApp;
		mEatsBalls = false;
		mImpatientTimer = -1;
		mBombFreqMax = 0;
		mBombFreqMin = 0;
		mBombDuration = 0;
		mProxBombRadius = 80;
		mDrawRadius = false;
		mBossRadius = 70;
		mNeedsIntroSound = false;
		mBombInRange = false;
		mRadiusColorChangeMode = 1;
		mDoDeathExplosions = false;
		mDeathTimer = 0;
		mWordBubbleTimer = 300;
		mSepiaImage = null;
		mDeathTX = 0f;
		mDeathTY = 0f;
		mDeathVX = 0f;
		mDeathVY = 0f;
		mExplosionRate = 4;
		mBossRadiusYOff = 0;
		mHulaAmnesty = 0;
		mBandagedImg = null;
		mAlphaOverride = 255f;
		mBandagedXOff = 0;
		mBandagedYOff = 0;
		mDrawDeathBGTikis = true;
		mTauntTextYOff = 0;
		gBerserkTextAlpha = 0f;
		gBerserkTextY = 0f;
		gImpatientTextAlpha = 0f;
		gImpatientTextY = 0f;
		mResPrefix = "IMAGE_";
		mHitEffect = null;
	}

	public virtual void Dispose()
	{
		mSepiaImage = null;
		for (int i = 0; i < mHulaDancers.size(); i++)
		{
			mHulaDancers[i] = null;
		}
		for (int j = 0; j < mDeathExplosions.Count; j++)
		{
			if (mDeathExplosions[j] != null)
			{
				mDeathExplosions[j].Dispose();
			}
		}
		mDeathExplosions.Clear();
		if (mHitEffect != null)
		{
			mHitEffect.Dispose();
			mHitEffect = null;
		}
	}

	public void AddTiki(int x, int y, int id, int rail_w, int rail_h, int travel_time)
	{
		Tiki tiki = new Tiki();
		mTikis.Add(tiki);
		tiki.mId = id;
		tiki.mX = x;
		tiki.mY = y;
		tiki.mRailStartX = x;
		tiki.mRailStartY = y;
		tiki.mRailEndX = x + rail_w;
		tiki.mRailEndY = y + rail_h;
		tiki.mTravelTime = travel_time;
		if (travel_time != 0)
		{
			tiki.mVX = (float)(tiki.mRailEndX - tiki.mRailStartX) / (float)travel_time;
		}
	}

	public void AddTiki(int x, int y, int id)
	{
		AddTiki(x, y, id, 0, 0, 0);
	}

	public void AddWall(int x, int y, int w, int h, int id)
	{
		BossWall bossWall = new BossWall();
		bossWall.mX = x;
		bossWall.mY = y;
		bossWall.mWidth = w;
		bossWall.mHeight = h;
		bossWall.mId = id;
		bossWall.mAlphaFadeDir = 1;
		bossWall.mAlpha = 0;
		mWalls.Add(bossWall);
	}

	public List<BossWall> getWalls()
	{
		return mWalls;
	}

	public void ForceNextTauntText()
	{
		mTauntQueue.Clear();
		FNTT_last_idx = (FNTT_last_idx + 1) % mTauntText.Count();
		if (FNTT_last_idx > mTauntText.Count())
		{
			FNTT_last_idx = 0;
		}
		mTauntQueue.Add(mTauntText[FNTT_last_idx]);
	}

	public virtual void Init(Level l)
	{
		mMaxHP = (mHP = 100f);
		if (l != null)
		{
			mLevel = l;
			for (int i = 0; i < mTikis.size(); i++)
			{
				mTikis[i].Init(this);
			}
		}
		if (mResGroup.Length > 0 && !mApp.mResourceManager.IsGroupLoaded(mResGroup) && !mApp.mResourceManager.LoadResources(mResGroup))
		{
			mApp.ShowResourceError(doExit: true);
			mApp.Shutdown();
			return;
		}
		if (!mApp.mResourceManager.IsGroupLoaded("Bosses") && !mApp.mResourceManager.LoadResources("Bosses"))
		{
			mApp.ShowResourceError(doExit: true);
			mApp.Shutdown();
			return;
		}
		if (mNum == 6 && !mApp.mResourceManager.IsGroupLoaded("Boss6Common") && !mApp.mResourceManager.LoadResources("Boss6Common"))
		{
			mApp.ShowResourceError(doExit: true);
			mApp.Shutdown();
			return;
		}
		mHitEffect = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_DEATH_EXPLOSION).Duplicate();
		Common.SetFXNumScale(mHitEffect, mApp.Is3DAccelerated() ? 1f : Common._M(0.25f));
		Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_HEARTS);
		ReInit();
		for (int j = 0; j < NUM_HEARTS; j++)
		{
			mHeartCels[j] = imageByID.mNumCols - 1;
		}
		InitParamPointers();
		if (mWalls.size() == mTikis.size())
		{
			for (int k = 0; k < mWalls.size(); k++)
			{
				BossWall bossWall = mWalls[k];
				bossWall.mAlphaFadeDir = 1;
				bossWall.mAlpha = 0;
			}
		}
		if (mTikis.size() == 2)
		{
			mTikis[0].SetIsLeft(mTikis[0].mX < mTikis[1].mX);
			mTikis[1].SetIsLeft(mTikis[1].mX < mTikis[0].mX);
		}
		mSounds[6] = -1;
		mSounds[7] = -1;
		mSounds[8] = -1;
		mSounds[9] = -1;
		if (mNum < 6)
		{
			mSounds[0] = mApp.mResourceManager.LoadSound("SOUND_BOSS" + mNum + "_DIE");
			mSounds[1] = mApp.mResourceManager.LoadSound("SOUND_BOSS" + mNum + "_ENRAGE");
			mSounds[2] = mApp.mResourceManager.LoadSound("SOUND_BOSS" + mNum + "_FIRE");
			mSounds[3] = mApp.mResourceManager.LoadSound("SOUND_BOSS" + mNum + "_HIT");
			mSounds[4] = mApp.mResourceManager.LoadSound("SOUND_BOSS" + mNum + "_PLAYER_HIT");
			mSounds[5] = mApp.mResourceManager.LoadSound("SOUND_BOSS" + mNum + "_INTRO");
			if (mNum == 4)
			{
				mSounds[6] = mApp.mResourceManager.LoadSound("SOUND_BOSS4_EAT_BALL");
				mSounds[8] = mApp.mResourceManager.LoadSound("SOUND_BOSS4_TELEPORT");
			}
			else if (mNum == 1)
			{
				mSounds[7] = mApp.mResourceManager.LoadSound("SOUND_BOSS1_ROAR");
			}
			else if (mNum == 5)
			{
				mSounds[9] = mApp.mResourceManager.LoadSound("SOUND_BOSS5_SHIELD_HIT");
			}
		}
		else
		{
			mSounds[0] = mApp.mResourceManager.LoadSound("SOUND_BOSS_DIE" + (1 + SexyFramework.Common.Rand() % 3));
			mSounds[1] = -1;
			mSounds[2] = mApp.mResourceManager.LoadSound("SOUND_BOSS_FIRE");
			mSounds[3] = mApp.mResourceManager.LoadSound("SOUND_BOSS_HIT" + (1 + SexyFramework.Common.Rand() % 4));
			mSounds[4] = mApp.mResourceManager.LoadSound("SOUND_BULLET_HIT");
			mSounds[5] = mApp.mResourceManager.LoadSound("SOUND_BOSS_INTRO" + SexyFramework.Common.Rand() % 3);
		}
		for (int m = 0; m < mHulaEntryVec.size(); m++)
		{
			if (mHulaEntryVec[m].mBerserkAmt >= 100)
			{
				mCurrentHulaEntry = mHulaEntryVec[m];
				break;
			}
		}
		int num = -1;
		for (int n = 0; n < mTauntText.size(); n++)
		{
			if (mTauntText[n].mMinDeaths <= mApp.mUserProfile.GetAdvModeVars().mNumDeathsCurLevel && mTauntText[n].mMinDeaths > num)
			{
				num = mTauntText[n].mMinDeaths;
			}
		}
		for (int num2 = 0; num2 < mTauntText.size(); num2++)
		{
			TauntText tauntText = mTauntText[num2];
			if (tauntText.mCondition == 0 && tauntText.mMinDeaths == num)
			{
				mTauntQueue.Add(tauntText);
			}
		}
	}

	public virtual void Update(float f)
	{
		if (mHP <= 0f || mLevel.mBoard.GetGameState() == GameState.GameState_Losing)
		{
			float num = ((mHP <= 0f) ? Common._M(1f) : Common._M1(3f));
			mAlphaOverride -= num;
			if (mAlphaOverride < 0f)
			{
				mAlphaOverride = 0f;
			}
		}
		else if (mLevel.DoingInitialPathHilite() && mLevel.mBoard.GetGameState() != GameState.GameState_BossIntro && mUpdateCount % Common._M(8) == 0 && mCleanHeart)
		{
			for (int i = 0; i < NUM_HEARTS; i++)
			{
				if (mHeartCels[i] != 0)
				{
					mHeartCels[i]--;
					break;
				}
			}
		}
		if (mAlphaOverride < 255f && mLevel.mBoard.GetGameState() != GameState.GameState_Losing && mLevel.mBoard.GetGameState() != GameState.GameState_BossDead)
		{
			mAlphaOverride += Common._M(3f);
			if (mAlphaOverride > 255f)
			{
				mAlphaOverride = 255f;
			}
		}
		mUpdateCount++;
		if (mDoExplosion)
		{
			mHitEffect.Update();
			if (!mHitEffect.IsActive())
			{
				mHitEffect.ResetAnim();
				mDoExplosion = false;
			}
		}
		if (mCurWallDownTime > 0 && --mCurWallDownTime == 0)
		{
			for (int j = 0; j < mWalls.size(); j++)
			{
				ResetWallAndTikis(j);
			}
		}
		if (mWordBubbleTimer > 0 && !mLevel.mBoard.DoingBossIntro())
		{
			mWordBubbleTimer--;
		}
		if (mDoDeathExplosions && mShouldDoDeathExplosions && mHP <= 0f && mUpdateCount % Common._M(25) == 0)
		{
			PIEffect pIEffect = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_DEATH_EXPLOSION).Duplicate();
			mDeathExplosions.Add(pIEffect);
			Common.SetFXNumScale(pIEffect, mApp.Is3DAccelerated() ? 1f : Common._M(0.25f));
			SexyTransform2D sexyTransform2D = new SexyTransform2D(init: false);
			sexyTransform2D.Translate(Common._S(-mWidth / 3 + SexyFramework.Common.Rand() % (int)((double)mWidth / 1.5)), Common._S(-mHeight / 3 + SexyFramework.Common.Rand() % (int)((double)mHeight / 1.5)));
			pIEffect.mEmitterTransform.CopyFrom(sexyTransform2D);
		}
		for (int k = 0; k < mDeathExplosions.size(); k++)
		{
			PIEffect pIEffect2 = mDeathExplosions[k];
			pIEffect2.Update();
			if (!pIEffect2.IsActive())
			{
				pIEffect2.Dispose();
				mDeathExplosions.RemoveAt(k);
				k--;
			}
		}
		int num2;
		for (num2 = 0; num2 < mTauntQueue.size(); num2++)
		{
			TauntText tauntText = mTauntQueue[num2];
			tauntText.mUpdateCount++;
			if (tauntText.mUpdateCount < tauntText.mDelay)
			{
				break;
			}
			mTauntQueue.RemoveAt(num2);
			num2--;
		}
		if (mTauntQueue.size() == 0 && mApp.GetLevelMgr().mBossTauntChance > 0 && CanTaunt() && SexyFramework.Common._geq(mAlphaOverride, 255f))
		{
			List<int> list = new List<int>();
			for (int l = 0; l < mTauntText.size(); l++)
			{
				TauntText tauntText2 = mTauntText[l];
				if (mUpdateCount > tauntText2.mMinTime && SexyFramework.Common.Rand() % mApp.GetLevelMgr().mBossTauntChance == 0 && (tauntText2.mCondition != 1 || (SexyFramework.Common._eq(mHP, mMaxHP) && tauntText2.mCondition != 0)) && (tauntText2.mMinDeaths < 0 || tauntText2.mMinDeaths == mApp.mUserProfile.GetAdvModeVars().mNumDeathsCurLevel))
				{
					list.Add(l);
				}
			}
			if (list.size() > 0)
			{
				mTauntQueue.Add(mTauntText[list[SexyFramework.Common.Rand() % list.size()]]);
			}
		}
		if (mDoExplosion || mDoDeathExplosions)
		{
			mShakeXOff = SexyFramework.Common.IntRange(0, mShakeXAmt);
			mShakeYOff = SexyFramework.Common.IntRange(0, mShakeYAmt);
		}
		if (gBerserkTextAlpha > 0f)
		{
			gBerserkTextAlpha -= Common._M(1f);
			gBerserkTextY -= Common._M(1f);
		}
		if (gImpatientTextAlpha > 0f)
		{
			gImpatientTextAlpha -= Common._M(1f);
			gImpatientTextY -= Common._M(1f);
		}
		if (mLevel.mBoard.DoingBossIntro())
		{
			return;
		}
		if (mHP <= 0f)
		{
			if ((!mLevel.mFinalLevel || !mLevel.mBoard.mAdventureWinScreen) && last_idx >= 4)
			{
				last_idx = 0;
			}
			if (!mDoDeathExplosions)
			{
				for (int m = 0; m < mDeathText.size(); m++)
				{
					BossText bossText = mDeathText[m];
					if (bossText.mAlpha < 255f)
					{
						bool flag = m == mDeathText.size() - 1 && bossText.mAlpha < 255f;
						bossText.mAlpha = Math.Min(255f, bossText.mAlpha + 3f);
						if (flag && bossText.mAlpha >= 255f)
						{
							mApp.SetCursor(ECURSOR.CURSOR_HAND);
						}
					}
					if (bossText.mAlpha < (float)Common._M(200))
					{
						break;
					}
				}
			}
			mX += mDeathVX;
			mY += mDeathVY;
			if ((mDeathVX > 0f && mX >= mDeathTX) || (mDeathVX < 0f && mX <= mDeathTX))
			{
				mX = mDeathTX;
				mDeathVX = 0f;
			}
			if ((mDeathVY > 0f && mY >= mDeathTY) || (mDeathVY < 0f && mY <= mDeathTY))
			{
				mY = mDeathTY;
				mDeathVY = 0f;
			}
			return;
		}
		bool flag2 = mLevel.AllCurvesAtRolloutPoint();
		if (mNeedsIntroSound && flag2 && !mApp.GetBoard().DoingIntros())
		{
			mNeedsIntroSound = false;
			PlaySound(5);
		}
		if (IsStunned())
		{
			mStunTime--;
		}
		if (mNeedsCompacting && !IsStunned() && CompactCurves())
		{
			mNeedsCompacting = false;
		}
		if (mHulaAmnesty > 0)
		{
			mHulaAmnesty--;
		}
		else if (mCurrentHulaEntry.mSpawnRate > 0 && mUpdateCount % mCurrentHulaEntry.mSpawnRate == 0 && SexyFramework.Common._geq(mAlphaOverride, 255f) && CanSpawnHulaDancers())
		{
			HulaDancer hulaDancer = new HulaDancer();
			mHulaDancers.Add(hulaDancer);
			bool has_proj = SexyFramework.Common.Rand() % 100 < mCurrentHulaEntry.mProjChance;
			hulaDancer.Setup(has_proj, mCurrentHulaEntry.mSpawnY, mCurrentHulaEntry.mProjVY);
		}
		for (int n = 0; n < mHulaDancers.size(); n++)
		{
			HulaDancer hulaDancer2 = mHulaDancers[n];
			if (!SexyFramework.Common._eq(mAlphaOverride, 255f))
			{
				hulaDancer2.mFadeOut = true;
			}
			hulaDancer2.Update(mCurrentHulaEntry.mVX);
			if (hulaDancer2.CanRemove())
			{
				mHulaDancers[n].Dispose();
				mHulaDancers.RemoveAt(n);
				n--;
			}
			else if (hulaDancer2.ProjectileCollided(mLevel.mFrog.GetRect()))
			{
				if (!mLevel.mFrog.IsFuckedUp())
				{
					mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_SLOW));
					switch ((HulaEntry.AttackType)mCurrentHulaEntry.mAttackType)
					{
					case HulaEntry.AttackType.Attack_Hallucinate:
						mLevel.mBoard.SetHallucinateTimer(mCurrentHulaEntry.mAttackTime);
						break;
					case HulaEntry.AttackType.Attack_Poison:
						mLevel.mFrog.Poison(mCurrentHulaEntry.mAttackTime);
						break;
					case HulaEntry.AttackType.Attack_Slow:
						mLevel.mFrog.SetSlowTimer(mCurrentHulaEntry.mAttackTime);
						break;
					case HulaEntry.AttackType.Attack_Stun:
						mLevel.mFrog.Stun(mCurrentHulaEntry.mAttackTime);
						break;
					}
				}
				hulaDancer2.DestroyBullet();
			}
			else if (!hulaDancer2.HasFired() && CanSpawnHulaDancers() && hulaDancer2.GetX() > (float)(mLevel.mFrog.GetCenterX() + mCurrentHulaEntry.mProjRange))
			{
				hulaDancer2.Fire();
			}
		}
		if (mImpatientTimer > 0 && flag2 && --mImpatientTimer == 0)
		{
			gImpatientTextAlpha = 255f;
			gImpatientTextY = mApp.mHeight / 2;
		}
		if (mDrawRadius && flag2)
		{
			mBombInRange = false;
			int num3 = mProxBombRadius + 56 + Common.GetDefaultBallRadius();
			int num4 = num3 * num3;
			for (int num5 = 0; num5 < mLevel.mNumCurves; num5++)
			{
				if (mBombInRange)
				{
					break;
				}
				for (int num6 = 0; num6 < mLevel.mCurveMgr[num5].mBallList.Count; num6++)
				{
					Ball ball = mLevel.mCurveMgr[num5].mBallList[num6];
					if (ball.GetPowerOrDestType(include_grace_period: false) == PowerType.PowerType_ProximityBomb)
					{
						if ((mRadiusColorChangeMode != 2 || ball.GetY() > mY - (float)(mHeight / 2) + (float)Common._M(0)) && SexyFramework.Common.Distance(ball.GetX(), ball.GetY(), mX, mY, sqrt: false) <= (float)num4)
						{
							ball.mDoBossPulse = true;
							mBombInRange = true;
						}
						else
						{
							ball.mDoBossPulse = false;
						}
					}
				}
			}
		}
		if (IsImpatient())
		{
			for (int num7 = 0; num7 < mLevel.mNumCurves; num7++)
			{
				mLevel.mCurveMgr[num7].mSpeedScale += 0.000100000005f;
			}
		}
		for (int num8 = 0; num8 < mTikis.size(); num8++)
		{
			mTikis[num8].Update();
		}
		for (int num9 = 0; num9 < mWalls.size(); num9++)
		{
			BossWall bossWall = mWalls[num9];
			int mAlpha = bossWall.mAlpha;
			bossWall.mAlpha += bossWall.mAlphaFadeDir * Common._M(8);
			if (bossWall.mAlpha < 0)
			{
				bossWall.mAlpha = 0;
			}
			else if (bossWall.mAlpha > 255)
			{
				bossWall.mAlpha = 255;
			}
			if (bossWall.mAlphaFadeDir == 1 && bossWall.mAlpha >= 255 && mAlpha < bossWall.mAlpha)
			{
				bossWall.mAlphaFadeDir = 0;
				ResetWallAndTikis(num9);
			}
		}
		if (mBombInRange)
		{
			gWackColorFade += gWackColorFadeDir;
			if (gWackColorFade >= 255 && gWackColorFadeDir > 0)
			{
				gWackColorFade = 255;
				gWackColorFadeDir *= -1;
			}
			else if (gWackColorFade <= 0 && gWackColorFadeDir < 0)
			{
				gWackColorFade = 0;
				gWackColorFadeDir *= -1;
			}
		}
	}

	public virtual void Update()
	{
		Update(1f);
	}

	public virtual void DrawDeathBGTikis(Graphics g)
	{
		if (!(mHP <= 0f) || !mDrawDeathBGTikis)
		{
			return;
		}
		int num = (int)((255f - mAlphaOverride) / (float)Common._M(11));
		if (num > 255)
		{
			num = 255;
		}
		g.PushState();
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(255, 255, 255, num);
		for (int i = 0; i < 13; i++)
		{
			ResID id = (ResID)(1268 + i);
			Image imageByID = Res.GetImageByID(id);
			int num2 = Common._DS(Res.GetOffsetXByID(id) - 160);
			int theY = Common._DS(Res.GetOffsetYByID(id));
			g.DrawImage(imageByID, num2, theY);
			if (i != 7 && i != 5)
			{
				g.DrawImageMirror(imageByID, num2 + imageByID.GetWidth(), theY);
			}
		}
		g.PopState();
	}

	public virtual void Draw(Graphics g)
	{
		if (mHP > 0f && !mDoDeathExplosions && !mLevel.mBoard.DoingBossIntro())
		{
			for (int i = 0; i < mHulaDancers.size(); i++)
			{
				mHulaDancers[i].Draw(g);
			}
		}
	}

	public void DrawDeathText(Graphics g, int alpha_override)
	{
		bool flag = false;
		for (int i = 0; i < mDeathText.size(); i++)
		{
			BossText bossText = mDeathText[i];
			if (bossText.mAlpha <= 0f)
			{
				break;
			}
			if (i == mDeathText.size() - 1 && bossText.mAlpha >= (float)Common._M(200))
			{
				flag = true;
			}
			Font fontByID = Res.GetFontByID(ResID.FONT_BOSS_TAUNT);
			if (Localization.GetCurrentLanguage() == Localization.LanguageType.Language_CH)
			{
				fontByID.mAscent = 25;
			}
			g.SetFont(fontByID);
			int num = Common._S(Common._M(200)) + i * Common._S(Common._M1(30));
			g.SetColor(Common._M(255), Common._M1(255), Common._M2(255), (int)((alpha_override == -1) ? bossText.mAlpha : ((float)alpha_override)));
			g.WriteWordWrapped(new Rect(0, num + Localization.GetCurrentFontOffsetY() * i, mApp.mWidth, mApp.mHeight), bossText.mText, -1, 0);
		}
		if (flag)
		{
			if (alpha_override != -1)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, alpha_override);
			}
			Image imageByID = Res.GetImageByID(ResID.IMAGE_FROG_RIBBIT);
			g.DrawImage(imageByID, (mApp.mWidth - imageByID.mWidth) / 2, Common._S(Common._M(330)));
			g.SetColorizeImages(colorizeImages: false);
			Font fontByID2 = Res.GetFontByID(ResID.FONT_SHAGLOUNGE28_STROKE);
			g.SetFont(fontByID2);
			g.SetColor(Common._M(255), Common._M1(255), Common._M2(255));
			g.WriteString(TextManager.getInstance().getString(433), 0, Common._DS(Common._M(1170)), mApp.mWidth, 0);
		}
	}

	public void DrawDeathText(Graphics g)
	{
		DrawDeathText(g, -1);
	}

	public virtual void DrawTopLevel(Graphics g)
	{
	}

	public virtual void DrawBottomLevel(Graphics g)
	{
	}

	public virtual void DrawBelowBalls(Graphics g)
	{
		if (mDrawRadius && mHP > 0f && !mDoDeathExplosions && !mLevel.mBoard.DoingBossIntro())
		{
			Color color = new Color(0, 0, 255, Common._M(125));
			if (mRadiusColorChangeMode != 0 && mBombInRange)
			{
				color = new Color(255, 0, 0, Common._M(200));
			}
			g.SetColor(color);
			CommonGraphics.DrawCircle(g, Common._S(mX), Common._S(mY), mProxBombRadius, Common._S(Common._M(30)));
		}
	}

	public virtual void DrawWordBubble(Graphics g)
	{
		if (mTauntQueue.size() == 0)
		{
			return;
		}
		TauntText tauntText = mTauntQueue[0];
		int wordBubbleAlpha = GetWordBubbleAlpha(tauntText);
		if (wordBubbleAlpha >= 0)
		{
			Font fontByID = Res.GetFontByID(ResID.FONT_MAIN22);
			SetWordBubbleLayout(tauntText.mText, fontByID, out var outBubbleBkg, out var outBubble, out var outInset);
			g.SetFont(fontByID);
			g.SetColor(255, 255, 255, wordBubbleAlpha);
			if ((wordBubbleAlpha != 255 && mTauntQueue.size() == 1) || mAlphaOverride <= 254f)
			{
				g.SetColorizeImages(colorizeImages: true);
			}
			g.DrawImageBox(outBubble, outBubbleBkg);
			g.SetColor(0, 0, 0, wordBubbleAlpha);
			g.WriteWordWrapped(outInset, tauntText.mText, -1, 0);
			g.SetColorizeImages(colorizeImages: false);
		}
	}

	public int GetWordBubbleAlpha(TauntText inTauntText)
	{
		int num = inTauntText.mDelay - inTauntText.mUpdateCount;
		int num2 = 255;
		if (num <= 20)
		{
			num2 -= 26 * (20 - num);
		}
		if (mAlphaOverride <= 254f)
		{
			num2 = (int)Math.Min(num2, mAlphaOverride);
		}
		return num2;
	}

	public void SetWordBubbleLayout(string inText, Font inFont, out Image outBubbleBkg, out Rect outBubble, out Rect outInset)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_BOSSUI);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BOSS_WORD_BUBBLE);
		Image imageByID3 = Res.GetImageByID(ResID.IMAGE_BOSS_WORD_BUBBLE_MIRROR);
		int num = (int)((float)mApp.GetScreenRect().mWidth - (float)imageByID.GetWidth() * 1.5f);
		int num2 = (int)((float)(num - Common._S(mWidth)) * 0.4f);
		int num3 = Common._S(5);
		int num4 = num2 - num3 * 2;
		int num5 = Common._GetWordWrappedHeight(inText, inFont, num4);
		int num6 = num5 + num3 * 2;
		Image image = imageByID2;
		int num7 = (int)((float)image.GetWidth() * 0.23f);
		int num8 = (int)((float)image.GetHeight() * 0.22f);
		Rect rect = new Rect
		{
			mX = (int)Common._S(mX + (float)mWidth * 0.5f),
			mY = (int)(Common._S(mY - (float)mHeight * 0.5f) + (float)mTauntTextYOff),
			mWidth = num2 + num7 * 2,
			mHeight = num6 + num8 * 2
		};
		int num9 = mApp.GetScreenRect().mX + num;
		if (rect.mX + rect.mWidth >= num9)
		{
			int num10 = rect.mWidth + Common._S(mWidth);
			if (rect.mX - num10 >= 0)
			{
				image = imageByID3;
				rect.mX -= num10;
			}
		}
		outBubbleBkg = image;
		outBubble = rect;
		outInset = new Rect(rect.mX + num7 + num3, rect.mY + num8 + num3, num4, num5);
	}

	public virtual void FrogInitialized(Gun g)
	{
	}

	public virtual void MouseDownDuringNoFire(int x, int y)
	{
	}

	public virtual bool AllowFrogToFire()
	{
		return mLevel.HasReachedCruisingSpeed();
	}

	public virtual int GetFrogReloadType()
	{
		return -1;
	}

	public virtual void MoveToDeathPosition(float x, float y)
	{
		mDeathTX = x;
		mDeathTY = y;
		mDeathVX = (x - mX) / 200f;
		mDeathVY = (y - mY) / 200f;
	}

	public void ShowAllDeathText()
	{
		mApp.SetCursor(ECURSOR.CURSOR_HAND);
		for (int i = 0; i < mDeathText.Count(); i++)
		{
			mDeathText[i].mAlpha = 255f;
		}
	}

	public void AddHulaEntry(float vx, float projvy, int spawn, int spawny, int proj_chance, int berserk_amt, int proj_range, int atype, int atime, int amnesty)
	{
		HulaEntry hulaEntry = new HulaEntry();
		hulaEntry.mBerserkAmt = berserk_amt;
		hulaEntry.mAmnesty = amnesty;
		hulaEntry.mProjVY = projvy;
		hulaEntry.mSpawnRate = spawn;
		hulaEntry.mVX = vx;
		hulaEntry.mSpawnY = spawny;
		hulaEntry.mProjChance = proj_chance;
		hulaEntry.mAttackTime = atime;
		hulaEntry.mAttackType = atype;
		hulaEntry.mProjRange = proj_range;
		mHulaEntryVec.Add(hulaEntry);
	}

	public List<HulaEntry> getHulaEntryList()
	{
		return mHulaEntryVec;
	}

	public void PlaySound(int soundid)
	{
		if (!mApp.GetBoard().DoingIntros() && mSounds[soundid] != -1)
		{
			mApp.PlaySample(mSounds[soundid]);
		}
	}

	public virtual void ProximityBombActivated(float x, float y, int radius)
	{
		ForceActivation(from_prox_bomb: true);
	}

	public void AddBerserkValue(int health_limit, string param_name, string value, ref string minval, ref string maxval, bool _override)
	{
		BerserkModifier item = new BerserkModifier(param_name, value, minval, maxval, _override);
		bool flag = param_name.Length == 0;
		for (int i = 0; i < mBerserkTiers.Count(); i++)
		{
			if (mBerserkTiers[i].mHealthLimit == health_limit)
			{
				if (!flag)
				{
					mBerserkTiers[i].mParams.Add(item);
				}
				return;
			}
		}
		BerserkTier berserkTier = new BerserkTier(health_limit);
		if (!flag)
		{
			berserkTier.mParams.Add(item);
		}
		for (int j = 0; j < mBerserkTiers.Count(); j++)
		{
			if (health_limit > mBerserkTiers[j].mHealthLimit)
			{
				mBerserkTiers.Insert(j, berserkTier);
				return;
			}
		}
		mBerserkTiers.Add(berserkTier);
	}

	public List<BerserkTier> getBerserkTiers()
	{
		return mBerserkTiers;
	}

	public void AddBerserkValue(int health_limit, string param_name, string value)
	{
		string minval = "";
		AddBerserkValue(health_limit, param_name, value, ref minval, ref minval, _override: false);
	}

	public virtual void SyncState(DataSync sync)
	{
		sync.SyncBoolean(ref mEatsBalls);
		sync.SyncFloat(ref mX);
		sync.SyncFloat(ref mY);
		sync.SyncFloat(ref mMaxHP);
		sync.SyncFloat(ref mHP);
		sync.SyncLong(ref mHulaAmnesty);
		sync.SyncFloat(ref mDHPDecPerHit.value);
		sync.SyncFloat(ref mDHPDecPerProxBomb.value);
		sync.SyncBoolean(ref mNeedsIntroSound);
		sync.SyncBoolean(ref mIsBerserk);
		sync.SyncLong(ref mWidth);
		sync.SyncLong(ref mHeight);
		sync.SyncLong(ref mUpdateCount);
		sync.SyncBoolean(ref mBombInRange);
		sync.SyncBoolean(ref mDoExplosion);
		if (sync.isWrite())
		{
			Common.SerializePIEffect(mHitEffect, sync);
		}
		else
		{
			Common.DeserializePIEffect(mHitEffect, sync);
			Common.SetFXNumScale(mHitEffect, GameApp.gApp.Is3DAccelerated() ? 1f : 0.25f);
		}
		sync.SyncBoolean(ref mNeedsCompacting);
		sync.SyncLong(ref mStunTime);
		sync.SyncLong(ref mCurrTikiBossHealthRemoved);
		sync.SyncLong(ref mDTikiHealthRespawnAmt.value);
		sync.SyncFloat(ref mDeathVX);
		sync.SyncFloat(ref mDeathVY);
		sync.SyncFloat(ref mDeathTX);
		sync.SyncFloat(ref mDeathTY);
		sync.SyncLong(ref mWordBubbleTimer);
		sync.SyncLong(ref mDeathTimer);
		sync.SyncBoolean(ref mDoDeathExplosions);
		sync.SyncLong(ref mDWallDownTime.value);
		sync.SyncLong(ref mCurWallDownTime);
		sync.SyncLong(ref mImpatientTimer);
		sync.SyncLong(ref mCurrentHulaEntry.mBerserkAmt);
		sync.SyncFloat(ref mCurrentHulaEntry.mVX);
		sync.SyncFloat(ref mCurrentHulaEntry.mProjVY);
		sync.SyncLong(ref mCurrentHulaEntry.mSpawnRate);
		sync.SyncLong(ref mCurrentHulaEntry.mSpawnY);
		sync.SyncLong(ref mCurrentHulaEntry.mProjChance);
		sync.SyncLong(ref mCurrentHulaEntry.mAttackType);
		sync.SyncLong(ref mCurrentHulaEntry.mAttackTime);
		sync.SyncLong(ref mCurrentHulaEntry.mProjRange);
		sync.SyncLong(ref mCurrentHulaEntry.mAmnesty);
		SyncTauntTexts(sync, clear: true);
		SexyFramework.Misc.Buffer buffer = sync.GetBuffer();
		if (sync.isWrite())
		{
			buffer.WriteLong(mHulaDancers.Count);
			for (int i = 0; i < mHulaDancers.Count; i++)
			{
				mHulaDancers[i].SyncState(sync);
			}
			buffer.WriteLong(mDeathText.Count);
			for (int j = 0; j < mDeathText.Count; j++)
			{
				buffer.WriteFloat(mDeathText[j].mAlpha);
			}
			buffer.WriteLong(mDeathExplosions.Count);
			for (int k = 0; k < mDeathExplosions.Count; k++)
			{
				Common.SerializePIEffect(mDeathExplosions[k], sync);
			}
		}
		else
		{
			int num = (int)buffer.ReadLong();
			for (int l = 0; l < num; l++)
			{
				HulaDancer hulaDancer = new HulaDancer();
				hulaDancer.SyncState(sync);
				mHulaDancers.Add(hulaDancer);
			}
			int num2 = (int)buffer.ReadLong();
			for (int m = 0; m < num2; m++)
			{
				mDeathText[m].mAlpha = buffer.ReadFloat();
			}
			num2 = (int)buffer.ReadLong();
			for (int n = 0; n < num2; n++)
			{
				PIEffect pIEffect = Res.GetPIEffectByID(ResID.PIEFFECT_NONRESIZE_DEATH_EXPLOSION).Duplicate();
				mDeathExplosions.Add(pIEffect);
				Common.DeserializePIEffect(pIEffect, sync);
				Common.SetFXNumScale(pIEffect, GameApp.gApp.Is3DAccelerated() ? 1f : Common._M(0.25f));
			}
		}
		for (int num3 = 0; num3 < mWalls.Count; num3++)
		{
			sync.SyncLong(ref mWalls[num3].mAlpha);
			sync.SyncLong(ref mWalls[num3].mAlphaFadeDir);
		}
		for (int num4 = 0; num4 < mTikis.Count; num4++)
		{
			sync.SyncLong(ref mTikis[num4].mAlphaFadeDir);
			sync.SyncFloat(ref mTikis[num4].mX);
			sync.SyncFloat(ref mTikis[num4].mY);
			sync.SyncBoolean(ref mTikis[num4].mWasHit);
			sync.SyncLong(ref mTikis[num4].mAlpha);
		}
		for (int num5 = 0; num5 < NUM_HEARTS; num5++)
		{
			sync.SyncLong(ref mHeartCels[num5]);
		}
	}

	private void SyncTauntTexts(DataSync sync, bool clear)
	{
		if (sync.isRead())
		{
			if (clear)
			{
				mTauntQueue.Clear();
			}
			long num = sync.GetBuffer().ReadLong();
			for (int i = 0; i < num; i++)
			{
				TauntText tauntText = new TauntText();
				tauntText.SyncState(sync);
				mTauntQueue.Add(tauntText);
			}
			return;
		}
		sync.GetBuffer().WriteLong(mTauntQueue.Count);
		foreach (TauntText item in mTauntQueue)
		{
			item.SyncState(sync);
		}
	}

	public virtual bool Collides(Bullet b)
	{
		float num = (float)b.GetRadius() * Common._M(0.75f);
		Rect r = new Rect((int)(b.GetX() - num), (int)(b.GetY() - num), (int)(num * 2f), (int)(num * 2f));
		bool flag = false;
		if (AllowFrogToFire())
		{
			flag = BulletIntersectsBoss(b);
			if (flag && !mEatsBalls)
			{
				flag = DoHit(b, from_prox_bomb: false);
			}
			else if (flag && mEatsBalls)
			{
				BallEaten(b);
				PlaySound(6);
				return true;
			}
		}
		if (CollidesWithWall(b))
		{
			return true;
		}
		for (int i = 0; i < mHulaDancers.Count(); i++)
		{
			if (mHulaDancers[i].Collided(r))
			{
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_HULAGIRLHIT));
				mHulaAmnesty = mCurrentHulaEntry.mAmnesty;
				mHulaDancers[i].Disable();
				return true;
			}
		}
		bool result = false;
		if (AllowFrogToFire())
		{
			for (int j = 0; j < mTikis.Count(); j++)
			{
				if (mTikis[j].mWasHit || mTikis[j].mAlphaFadeDir < 0)
				{
					continue;
				}
				bool should_destroy = false;
				if (!mTikis[j].Collides(b, ref should_destroy))
				{
					continue;
				}
				result = true;
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_TIKI_HIT));
				if (mTikis.Count() == mWalls.Count())
				{
					BossWall bossWall = mWalls[j];
					bossWall.mAlphaFadeDir = -1;
					TikiHit(j);
					int num2 = 0;
					for (int k = 0; k < mTikis.Count(); k++)
					{
						if (mTikis[k].mWasHit)
						{
							num2++;
						}
					}
					if (num2 == mTikis.Count())
					{
						mCurWallDownTime = mWallDownTime;
					}
				}
				return true;
			}
		}
		if (flag && mResetWallsOnBossHit)
		{
			for (int l = 0; l < mWalls.Count(); l++)
			{
				mWalls[l].mAlphaFadeDir = 1;
			}
			for (int m = 0; m < mTikis.Count(); m++)
			{
				mTikis[m].mAlphaFadeDir = 1;
				mTikis[m].mWasHit = false;
			}
		}
		if (!flag)
		{
			return result;
		}
		return true;
	}

	public virtual void ForceActivation(bool from_prox_bomb)
	{
		DoHit(null, from_prox_bomb);
	}

	public abstract Boss Instantiate();

	public virtual void PostInstantiationHook(Boss source_boss)
	{
		mFParamPointerMap.Clear();
		mIParamPointerMap.Clear();
		mBParamPointerMap.Clear();
		AddParamPointer("WallDownTime", mDWallDownTime);
		AddParamPointer("HPDecPerHit", mDHPDecPerHit);
		AddParamPointer("HPDecPerProxBomb", mDHPDecPerProxBomb);
		AddParamPointer("TikiHealthRespawn", mDTikiHealthRespawnAmt);
		mTikis.Clear();
		foreach (Tiki mTiki in source_boss.mTikis)
		{
			AddTiki((int)mTiki.mX, (int)mTiki.mY, mTiki.mId, mTiki.mRailEndX - mTiki.mRailStartX, mTiki.mRailEndY - mTiki.mRailStartY, mTiki.mTravelTime);
		}
	}

	public virtual bool CanAdvanceBalls()
	{
		return true;
	}

	public virtual void PlayerStartedFiring()
	{
	}

	public virtual void SetXY(float x, float y)
	{
		mX = x;
		mY = y;
	}

	public virtual void SetX(float x)
	{
		mX = x;
	}

	public virtual void SetY(float y)
	{
		mY = y;
	}

	public virtual void SetHPDecPerHit(float hp)
	{
		mHPDecPerHit = hp;
	}

	public virtual void SetHPDecPerHitProxBomb(float hp)
	{
		mHPDecPerProxBomb = hp;
	}

	public virtual void Stun(int stime)
	{
		mStunTime = stime;
		mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BOSS_STUNNED));
	}

	public virtual void SetHP(float hp)
	{
		float num = mHP;
		mHP = hp;
		int num2 = (int)((num - mHP) / mHPDecPerHit);
		for (int i = 0; i < num2; i++)
		{
			DecHearts(mHeartPieceDecAmt);
		}
	}

	public bool IsStunned()
	{
		return mStunTime > 0;
	}

	public float GetHP()
	{
		return mHP;
	}

	public virtual int GetX()
	{
		return (int)mX;
	}

	public virtual int GetY()
	{
		return (int)mY;
	}

	public virtual int GetTopLeftX()
	{
		return (int)mX - mWidth / 2;
	}

	public virtual int GetTopLeftY()
	{
		return (int)mY - mHeight / 2;
	}

	public int GetWidth()
	{
		return mWidth;
	}

	public int GetHeight()
	{
		return mHeight;
	}

	public bool IsImpatient()
	{
		return mImpatientTimer == 0;
	}

	public bool IsHitByExplosion(float x, float y, int radius)
	{
		return MathUtils.CirclesIntersect(x, y, mX, mY, mProxBombRadius + 56 + Common.GetDefaultBallRadius());
	}

	public virtual void InitParam()
	{
	}

	public void CopyFrom(Boss rhs)
	{
		mX = rhs.mX;
		mY = rhs.mY;
		mMaxHP = rhs.mMaxHP;
		mHP = rhs.mHP;
		mWidth = rhs.mWidth;
		mHeight = rhs.mHeight;
		mUpdateCount = rhs.mUpdateCount;
		mHPDecPerHit = rhs.mHPDecPerHit;
		mHPDecPerProxBomb = rhs.mHPDecPerProxBomb;
		mShakeXAmt = rhs.mShakeXAmt;
		mShakeYAmt = rhs.mShakeYAmt;
		mShouldDoDeathExplosions = rhs.mShouldDoDeathExplosions;
		mShakeXOff = rhs.mShakeXOff;
		mShakeYOff = rhs.mShakeYOff;
		mAllowLevelDDS = rhs.mAllowLevelDDS;
		mDoExplosion = rhs.mDoExplosion;
		mNeedsCompacting = rhs.mNeedsCompacting;
		mAllowCompacting = rhs.mAllowCompacting;
		mHeartXOff = rhs.mHeartXOff;
		mHeartYOff = rhs.mHeartYOff;
		mResetWallTimerOnTikiHit = rhs.mResetWallTimerOnTikiHit;
		mResetWallsOnBossHit = rhs.mResetWallsOnBossHit;
		mWallDownTime = rhs.mWallDownTime;
		mCurWallDownTime = rhs.mCurWallDownTime;
		mStunTime = rhs.mStunTime;
		mCurrTikiBossHealthRemoved = rhs.mCurrTikiBossHealthRemoved;
		mTikiHealthRespawnAmt = rhs.mTikiHealthRespawnAmt;
		mNum = rhs.mNum;
		mIsBerserk = rhs.mIsBerserk;
		mApp = GameApp.gApp;
		mEatsBalls = rhs.mEatsBalls;
		mImpatientTimer = rhs.mImpatientTimer;
		mBombFreqMax = rhs.mBombFreqMax;
		mBombFreqMin = rhs.mBombFreqMin;
		mBombDuration = rhs.mBombDuration;
		mProxBombRadius = rhs.mProxBombRadius;
		mDrawRadius = rhs.mDrawRadius;
		mBossRadius = rhs.mBossRadius;
		mNeedsIntroSound = rhs.mNeedsIntroSound;
		mBombInRange = rhs.mBombInRange;
		mRadiusColorChangeMode = rhs.mRadiusColorChangeMode;
		mDoDeathExplosions = rhs.mDoDeathExplosions;
		mDeathTimer = rhs.mDeathTimer;
		mWordBubbleTimer = rhs.mWordBubbleTimer;
		mSepiaImage = rhs.mSepiaImage;
		mDeathTX = rhs.mDeathTX;
		mDeathTY = rhs.mDeathTY;
		mDeathVX = rhs.mDeathVX;
		mDeathVY = rhs.mDeathVY;
		mExplosionRate = rhs.mExplosionRate;
		mBossRadiusYOff = rhs.mBossRadiusYOff;
		mHulaAmnesty = rhs.mHulaAmnesty;
		mBandagedImg = rhs.mBandagedImg;
		mAlphaOverride = rhs.mAlphaOverride;
		mBandagedXOff = rhs.mBandagedXOff;
		mBandagedYOff = rhs.mBandagedYOff;
		mDrawDeathBGTikis = rhs.mDrawDeathBGTikis;
		mTauntTextYOff = rhs.mTauntTextYOff;
		mResPrefix = rhs.mResPrefix;
		mHitEffect = rhs.mHitEffect;
		mDeathText.Clear();
		mDeathText.AddRange(rhs.mDeathText.ToArray());
		mTauntText.Clear();
		mTauntText.AddRange(rhs.mTauntText.ToArray());
		mTikis.Clear();
		for (int i = 0; i < rhs.mTikis.Count; i++)
		{
			mTikis.Add(new Tiki(rhs.mTikis[i]));
		}
		mHulaDancers.Clear();
		for (int j = 0; j < rhs.mHulaDancers.Count; j++)
		{
			mHulaDancers.Add(new HulaDancer(rhs.mHulaDancers[j]));
		}
		mHulaEntryVec.Clear();
		for (int k = 0; k < rhs.mHulaEntryVec.Count; k++)
		{
			mHulaEntryVec.Add(new HulaEntry(rhs.mHulaEntryVec[k]));
		}
		if (rhs.mCurrentHulaEntry != null)
		{
			mCurrentHulaEntry = new HulaEntry(rhs.mCurrentHulaEntry);
		}
		Dictionary<string, ParamData<float>>.Enumerator enumerator = rhs.mFParamPointerMap.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (mFParamPointerMap[enumerator.Current.Key] != null)
			{
				mFParamPointerMap[enumerator.Current.Key].value = enumerator.Current.Value.value;
			}
		}
		Dictionary<string, ParamData<int>>.Enumerator enumerator2 = rhs.mIParamPointerMap.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			if (mIParamPointerMap[enumerator2.Current.Key] != null)
			{
				mIParamPointerMap[enumerator2.Current.Key].value = enumerator2.Current.Value.value;
			}
		}
		Dictionary<string, ParamData<bool>>.Enumerator enumerator3 = rhs.mBParamPointerMap.GetEnumerator();
		while (enumerator3.MoveNext())
		{
			if (mBParamPointerMap[enumerator3.Current.Key] != null)
			{
				mBParamPointerMap[enumerator3.Current.Key].value = enumerator3.Current.Value.value;
			}
		}
		mBerserkTiers.Clear();
		for (int l = 0; l < rhs.mBerserkTiers.Count; l++)
		{
			mBerserkTiers.Add(new BerserkTier(rhs.mBerserkTiers[l]));
		}
		mWalls.Clear();
		for (int m = 0; m < rhs.mWalls.Count; m++)
		{
			mWalls.Add(new BossWall(rhs.mWalls[m]));
		}
		mDeathExplosions.Clear();
		for (int n = 0; n < rhs.mDeathExplosions.Count; n++)
		{
			mDeathExplosions.Add(mDeathExplosions[n]);
		}
		mTauntQueue.Clear();
		for (int num = 0; num < rhs.mTauntQueue.Count; num++)
		{
			mTauntQueue.Add(new TauntText(mTauntQueue[num]));
		}
		for (int num2 = 0; num2 < rhs.mSounds.Length; num2++)
		{
			mSounds[num2] = rhs.mSounds[num2];
		}
		for (int num3 = 0; num3 < rhs.mHeartCels.Length; num3++)
		{
			mHeartCels[num3] = rhs.mHeartCels[num3];
		}
	}
}
