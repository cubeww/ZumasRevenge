using System;
using System.Collections.Generic;
using System.Linq;
using JeffLib;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using ZumasRevenge.Achievement;

namespace ZumasRevenge;

public class CurveMgr
{
	public static bool[] gGotPowerUp = new bool[14];

	public static bool gStopSuckbackImmediately = false;

	public static int MAX_GAP_SIZE = 300;

	public GameApp mApp;

	public Board mBoard;

	public WayPointMgr mWayPointMgr;

	public Level mLevel;

	public CurveDesc mCurveDesc;

	public float mSpeedScale;

	public bool mIsLoaded;

	public Color mLastScoreColor = default(Color);

	public List<PathSparkle> mSparkles = new List<PathSparkle>();

	public List<WarningLight> mWarningLights = new List<WarningLight>();

	public List<Bullet> mBulletList = new List<Bullet>();

	public List<Ball> mBallList = new List<Ball>();

	public List<Ball> mPendingBalls = new List<Ball>();

	public int mPostZumaFlashTimer;

	public int mLastPowerupTime;

	public int[] mLastSpawnedPowerUpFrame = new int[14];

	public int[] mLastCompletedPowerUpFrame = new int[14];

	public int[] mNumPowerUpsThisLevel = new int[14];

	public int[] mNumPowerupsActivated = new int[14];

	public int[] mBallColorHasPowerup = new int[6];

	public int mNumBallsCreated;

	public int mCurveNum;

	public int mStopTime;

	public int mProxBombCounter;

	public int mSlowCount;

	public int mBackwardCount;

	public int mTotalBalls;

	public float mAdvanceSpeed;

	public float mSkullHilite;

	public float mSkullHiliteDir;

	public int mFirstChainEnd;

	public bool mFirstBallMovedBackwards;

	public bool mHaveSets;

	public bool mDoingClearCurveRollout;

	public bool mInitialPathHilite;

	public int mLastPathHiliteWP;

	public int mLastPathHilitePitch;

	public int mDangerPoint;

	public int mPathLightEndFrame;

	public int mLastClearedBallPoint;

	public float mOverrideSpeed;

	public bool mHadPowerUp;

	public bool mStopAddingBalls;

	public bool mInDanger;

	public bool mHasReachedCruisingSpeed;

	public bool mHasReachedRolloutPoint;

	public bool mNeedsSpeedup;

	public bool mCanCheckForSpeedup;

	public uint mLastPathShowTick;

	public int mNumMultBallsToSpawn;

	public List<InkBlot> mInkSpots = new List<InkBlot>();

	protected bool CanSpawnPowerUp(PowerType ptype)
	{
		if (mBoard.GauntletMode() && ptype == PowerType.PowerType_MoveBackwards && GetFarthestBallPercent() < 25)
		{
			return false;
		}
		if (ptype == PowerType.PowerType_ProximityBomb && mLevel.mBoss != null && mLevel.mBoss.mBombFreqMin > 0)
		{
			return false;
		}
		int num = ((Board.gDebugCurveData == null) ? mCurveDesc.mVals.mMaxNumPowerUps[(int)ptype] : Board.gDebugCurveData.mVals.mMaxNumPowerUps[(int)ptype]);
		if (ptype == PowerType.PowerType_ColorNuke && mApp.GetLevelMgr().mMaxZumaPctForColorNuke > 0f && mLevel.GetBarPercent() > mApp.GetLevelMgr().mMaxZumaPctForColorNuke)
		{
			return false;
		}
		if (ptype == PowerType.PowerType_ColorNuke && !mApp.GetLevelMgr().mAllowColorNukeAfterZuma && mBoard.HasAchievedZuma())
		{
			return false;
		}
		if ((ptype == PowerType.PowerType_ColorNuke || ptype == PowerType.PowerType_Cannon || ptype == PowerType.PowerType_Laser) && mBoard.HasAchievedZuma())
		{
			return false;
		}
		if ((ptype == PowerType.PowerType_ColorNuke && (mLevel.HasPowerup(PowerType.PowerType_Cannon) || mLevel.mFrog.CannonMode())) || (ptype == PowerType.PowerType_Cannon && (mLevel.HasPowerup(PowerType.PowerType_ColorNuke) || mLevel.mFrog.LightningMode())))
		{
			return false;
		}
		if (ptype == PowerType.PowerType_Accuracy)
		{
			return false;
		}
		if (!mApp.GetLevelMgr().mCapAffectsPowerupsSpawned)
		{
			return mNumPowerupsActivated[(int)ptype] < num;
		}
		return mNumPowerUpsThisLevel[(int)ptype] < num;
	}

	protected int GetNumPendingMatches(int t)
	{
		int num = 0;
		for (int num2 = mPendingBalls.Count() - 1; num2 >= 0; num2--)
		{
			Ball ball = mPendingBalls[num2];
			if (ball.GetColorType() != t)
			{
				break;
			}
			num++;
		}
		if (mPendingBalls.Count() > 0)
		{
			return num;
		}
		foreach (Ball mBall in mBallList)
		{
			if (mBall.GetColorType() == t)
			{
				num++;
				continue;
			}
			break;
		}
		return num;
	}

	protected int GetNumPendingMatches()
	{
		if (mPendingBalls.Count() == 0 && mBallList.Count() == 0)
		{
			return 0;
		}
		int t = ((mPendingBalls.Count() > 0) ? mPendingBalls.Last().GetColorType() : mBallList.First().GetColorType());
		return GetNumPendingMatches(t);
	}

	protected int GetNumPendingSingles(int theNumGroups)
	{
		int aNumGroups = 0;
		int aPrevColor = -1;
		int aNumSingles = 0;
		int aGroupCount = 0;
		int num = mPendingBalls.Count() - 1;
		while (num >= 0 && aNumGroups <= theNumGroups)
		{
			GetNumPendingSinglesHelper(mPendingBalls[num].GetColorType(), ref aNumGroups, ref aPrevColor, ref aNumSingles, ref aGroupCount);
			num--;
		}
		for (int i = 0; i < mBallList.Count(); i++)
		{
			if (aNumGroups > theNumGroups)
			{
				break;
			}
			GetNumPendingSinglesHelper(mBallList[i].GetColorType(), ref aNumGroups, ref aPrevColor, ref aNumSingles, ref aGroupCount);
		}
		return aNumSingles;
	}

	public void Reset()
	{
		mCurveDesc.mVals.mAccelerationRate = mCurveDesc.mVals.mOrgAccelerationRate;
		mHasReachedCruisingSpeed = false;
		mNumBallsCreated = 0;
		mNeedsSpeedup = false;
		mOverrideSpeed = -1f;
		mProxBombCounter = -1;
		mHasReachedRolloutPoint = false;
		mCanCheckForSpeedup = false;
		mLastPowerupTime = 0;
		mLastPathHiliteWP = 0;
		mLastPathHilitePitch = ((mCurveNum == 1) ? (-20) : 0);
		mInitialPathHilite = true;
		mSkullHilite = 0f;
		mDoingClearCurveRollout = false;
		mNumMultBallsToSpawn = 0;
		mSkullHiliteDir = 0f;
		for (int i = 0; i < 14; i++)
		{
			mNumPowerUpsThisLevel[i] = 0;
			mNumPowerupsActivated[i] = 0;
		}
		for (int j = 0; j < 6; j++)
		{
			mBallColorHasPowerup[j] = 0;
		}
		for (int k = 0; k < mWarningLights.Count(); k++)
		{
			mWarningLights[k].mState = -1;
			if (mWarningLights[k].mPulseRate > 0f)
			{
				mWarningLights[k].mPulseRate *= -1f;
			}
		}
	}

	private void DeleteBullet(Bullet theBullet)
	{
		if (theBullet != null)
		{
			theBullet.Dispose();
			mBulletList.Remove(theBullet);
		}
	}

	private void DeleteBall(Ball theBall)
	{
		Bullet bullet = theBall.GetBullet();
		if (bullet != null)
		{
			bullet.MergeFully();
			int theBulletItr = mBulletList.IndexOf(bullet);
			if (theBulletItr >= 0)
			{
				AdvanceMergingBullet(ref theBulletItr);
			}
		}
		DeleteBullet(theBall.GetBullet());
		mBoard.CheckShouldClearGuideBall(theBall);
		theBall.SetCollidesWithPrev(collidesWithPrev: false);
	}

	public void SyncState(DataSync theSync)
	{
		SexyFramework.Misc.Buffer buffer = theSync.GetBuffer();
		theSync.RegisterPointer(this);
		theSync.SyncLong(ref mProxBombCounter);
		theSync.SyncBoolean(ref mDoingClearCurveRollout);
		theSync.SyncFloat(ref mSkullHilite);
		theSync.SyncFloat(ref mSkullHiliteDir);
		theSync.SyncBoolean(ref mInitialPathHilite);
		theSync.SyncLong(ref mLastPathHiliteWP);
		if (theSync.isRead())
		{
			if (buffer.ReadBoolean())
			{
				mLevel.mHoleMgr.SetPctOpen(mCurveNum, buffer.ReadFloat());
			}
			DeleteBalls();
			short num = buffer.ReadShort();
			for (int i = 0; i < num; i++)
			{
				Bullet bullet = new Bullet();
				bullet.SyncState(theSync);
				mBulletList.Add(bullet);
			}
			num = buffer.ReadShort();
			for (int j = 0; j < num; j++)
			{
				Ball ball = new Ball();
				ball.SyncState(theSync);
				mPendingBalls.Add(ball);
			}
			num = buffer.ReadShort();
			for (int k = 0; k < num; k++)
			{
				Ball ball2 = new Ball();
				ball2.SyncState(theSync);
				ball2.InsertInList(mBallList, mBallList.Count, this);
			}
			buffer.ReadLong();
			num = (short)buffer.ReadLong();
			for (int l = 0; l < num; l++)
			{
				PathSparkle pathSparkle = new PathSparkle();
				pathSparkle.mCel = (int)buffer.ReadLong();
				pathSparkle.mX = (int)buffer.ReadLong();
				pathSparkle.mY = (int)buffer.ReadLong();
				pathSparkle.mUpdateCount = (int)buffer.ReadLong();
				pathSparkle.mPri = (int)buffer.ReadLong();
				mSparkles.Add(pathSparkle);
			}
			mInkSpots.Clear();
			int num2 = (int)buffer.ReadLong();
			for (int m = 0; m < num2; m++)
			{
				InkBlot inkBlot = new InkBlot();
				inkBlot.mX = buffer.ReadFloat();
				inkBlot.mY = buffer.ReadFloat();
				inkBlot.mRadius = buffer.ReadFloat();
				inkBlot.mAlpha = buffer.ReadFloat();
				inkBlot.mAlphaDec = buffer.ReadFloat();
				inkBlot.mAngle = buffer.ReadFloat();
				inkBlot.mDelay = (int)buffer.ReadLong();
				inkBlot.mFadeDelayTimer = (int)buffer.ReadLong();
				mInkSpots.Add(inkBlot);
			}
		}
		else
		{
			if (mBoard.GetGameState() == GameState.GameState_Losing)
			{
				buffer.WriteBoolean(theBool: true);
				float pctOpen = mLevel.mHoleMgr.GetHole(mCurveNum).GetPctOpen();
				buffer.WriteFloat(pctOpen);
			}
			else
			{
				buffer.WriteBoolean(theBool: false);
			}
			buffer.WriteShort((short)mBulletList.Count);
			for (int n = 0; n < mBulletList.Count; n++)
			{
				mBulletList[n].SyncState(theSync);
			}
			buffer.WriteShort((short)mPendingBalls.Count);
			for (int num3 = 0; num3 < mPendingBalls.Count; num3++)
			{
				mPendingBalls[num3].SyncState(theSync);
			}
			buffer.WriteShort((short)mBallList.Count);
			for (int num4 = 0; num4 < mBallList.Count; num4++)
			{
				mBallList[num4].SyncState(theSync);
			}
			buffer.WriteLong(mWarningLights.Count);
			buffer.WriteLong(mSparkles.Count);
			for (int num5 = 0; num5 < mSparkles.Count; num5++)
			{
				PathSparkle pathSparkle2 = mSparkles[num5];
				buffer.WriteLong(pathSparkle2.mCel);
				buffer.WriteLong(pathSparkle2.mX);
				buffer.WriteLong(pathSparkle2.mY);
				buffer.WriteLong(pathSparkle2.mUpdateCount);
				buffer.WriteLong(pathSparkle2.mPri);
			}
			buffer.WriteLong(mInkSpots.Count);
			for (int num6 = 0; num6 < mInkSpots.Count; num6++)
			{
				InkBlot inkBlot2 = mInkSpots[num6];
				buffer.WriteFloat(inkBlot2.mX);
				buffer.WriteFloat(inkBlot2.mY);
				buffer.WriteFloat(inkBlot2.mRadius);
				buffer.WriteFloat(inkBlot2.mAlpha);
				buffer.WriteFloat(inkBlot2.mAlphaDec);
				buffer.WriteFloat(inkBlot2.mAngle);
				buffer.WriteLong(inkBlot2.mDelay);
				buffer.WriteLong(inkBlot2.mFadeDelayTimer);
			}
		}
		theSync.SyncBoolean(ref mNeedsSpeedup);
		theSync.SyncBoolean(ref mCanCheckForSpeedup);
		theSync.SyncBoolean(ref mHasReachedRolloutPoint);
		for (int num7 = 0; num7 < mWarningLights.Count; num7++)
		{
			mWarningLights[num7].SyncState(theSync);
		}
		for (int num8 = 0; num8 < 14; num8++)
		{
			theSync.SyncLong(ref mLastSpawnedPowerUpFrame[num8]);
			theSync.SyncLong(ref mNumPowerUpsThisLevel[num8]);
			theSync.SyncLong(ref mNumPowerupsActivated[num8]);
		}
		for (int num9 = 0; num9 < 6; num9++)
		{
			theSync.SyncLong(ref mBallColorHasPowerup[num9]);
		}
		theSync.SyncLong(ref mStopTime);
		theSync.SyncLong(ref mSlowCount);
		theSync.SyncLong(ref mBackwardCount);
		theSync.SyncLong(ref mTotalBalls);
		theSync.SyncFloat(ref mAdvanceSpeed);
		theSync.SyncLong(ref mFirstChainEnd);
		theSync.SyncBoolean(ref mFirstBallMovedBackwards);
		theSync.SyncBoolean(ref mHaveSets);
		theSync.SyncLong(ref mPathLightEndFrame);
		theSync.SyncBoolean(ref mHadPowerUp);
		theSync.SyncLong(ref mLastPathShowTick);
		theSync.SyncLong(ref mLastClearedBallPoint);
		theSync.SyncBoolean(ref mStopAddingBalls);
		theSync.SyncBoolean(ref mInDanger);
		theSync.SyncBoolean(ref mHasReachedCruisingSpeed);
		theSync.SyncBoolean(ref mHadPowerUp);
		theSync.SyncLong(ref mNumBallsCreated);
		theSync.SyncFloat(ref mSpeedScale);
		theSync.SyncFloat(ref mOverrideSpeed);
	}

	public void SetFarthestBall(int thePoint)
	{
		if (mBoard.GetGameState() != GameState.GameState_Losing)
		{
			int num = mDangerPoint;
			if (num < 0)
			{
				num = 0;
			}
			float pct_open = 0f;
			if (thePoint >= num)
			{
				pct_open = (float)(thePoint - num) / (float)(mWayPointMgr.GetNumPoints() - num);
			}
			mLevel.mHoleMgr.SetPctOpen(mCurveNum, pct_open);
		}
	}

	private int GetNumInARow(Ball theBall, int theColor, ref Ball theNextEnd, ref Ball thePrevEnd)
	{
		if (theBall.GetColorType() != theColor)
		{
			return 0;
		}
		Ball ball = theBall;
		int num = 1;
		while (true)
		{
			Ball nextBall = ball.GetNextBall(mustCollide: true);
			if (nextBall == null || nextBall.GetColorType() != theColor)
			{
				break;
			}
			ball = nextBall;
			num++;
		}
		Ball ball2 = theBall;
		while (true)
		{
			Ball prevBall = ball2.GetPrevBall(mustCollide: true);
			if (prevBall == null || prevBall.GetColorType() != theColor)
			{
				break;
			}
			ball2 = prevBall;
			num++;
		}
		theNextEnd = ball;
		thePrevEnd = ball2;
		return num;
	}

	private bool CheckSet(Ball theBall)
	{
		mHadPowerUp = false;
		Ball theNextEnd = null;
		Ball thePrevEnd = null;
		int comboCount = theBall.GetComboCount();
		int numInARow = GetNumInARow(theBall, theBall.GetColorType(), ref theNextEnd, ref thePrevEnd);
		if (numInARow >= 3)
		{
			mBoard.SetNumCleared(0);
			mBoard.SetCurComboCount(comboCount);
			mBoard.SetCurComboScore(theBall.GetComboScore());
			mBoard.mNeedComboCount.Clear();
			for (int i = 0; i < 14; i++)
			{
				Common.gGotPowerUp[i] = false;
			}
			int num = 0;
			int num2 = 0;
			Ball nextBall = theNextEnd.GetNextBall();
			for (Ball ball = thePrevEnd; ball != nextBall; ball = ball.GetNextBall())
			{
				if (ball.GetSuckPending())
				{
					ball.SetSuckPending(pending: false);
					mBoard.IncNumClearsInARow(1);
				}
				StartExploding(ball);
				num += ball.GetGapBonus();
				if (ball.GetNumGaps() > num2)
				{
					num2 = ball.GetNumGaps();
				}
				ball.SetGapBonus(0, 0);
			}
			DoScoring(theBall, mBoard.GetNumCleared(), comboCount, num, num2);
			GameStats levelStats = mBoard.GetLevelStats();
			if (mBoard.GetCurComboCount() > levelStats.mMaxCombo || (mBoard.GetCurComboCount() == levelStats.mMaxCombo && mBoard.GetCurComboScore() >= levelStats.mMaxComboScore))
			{
				levelStats.mMaxCombo = mBoard.GetCurComboCount();
				levelStats.mMaxComboScore = mBoard.GetCurComboScore();
				if (levelStats.mMaxCombo >= 4)
				{
					mBoard.UnlockAchievement(EAchievementType.POWER_PLAYER);
				}
			}
			for (Ball ball = thePrevEnd; ball != nextBall; ball = ball.GetNextBall())
			{
				ball.SetComboCount(comboCount, mBoard.GetCurComboScore());
			}
			foreach (Ball item in mBoard.mNeedComboCount)
			{
				item.SetComboCount(comboCount, mBoard.GetCurComboScore());
			}
			mBoard.mNeedComboCount.Clear();
			PlayComboSound(comboCount);
			mBoard.SetCurComboCount(0);
			mBoard.SetCurComboScore(0);
			return true;
		}
		return false;
	}

	private void DoScoring(Ball theBall, int theNumBalls, int theComboCount, int theGapBonus, int theNumGaps)
	{
		if (theNumBalls == 0)
		{
			return;
		}
		int num = theNumBalls * 10 + theGapBonus + theComboCount * 100;
		bool flag = false;
		int num2 = (Common.gSuckMode ? 10 : 4);
		int num3 = 0;
		if (mBoard.GetNumClearsInARow() > num2 && theComboCount == 0)
		{
			flag = true;
			num3 = (Common.gSuckMode ? 10 : 100) + 10 * (mBoard.GetNumClearsInARow() - (num2 + 1));
			num += num3;
			if (mLevel.AllowPointsFromBalls())
			{
				mBoard.IncCurInARowBonus(num3);
			}
		}
		if (mLevel.AllowPointsFromBalls())
		{
			mBoard.IncCurComboScore(num);
			mBoard.IncScore(num, from_balls: true);
			GameStats levelStats = mBoard.GetLevelStats();
			if (theComboCount > 0)
			{
				levelStats.mNumCombos++;
			}
			if (theGapBonus > 0)
			{
				levelStats.mNumGaps++;
			}
		}
		Board board = ((GameApp)GlobalMembers.gSexyApp).GetBoard();
		BonusTextElement bonusTextElement = null;
		if (mLevel.AllowPointsFromBalls())
		{
			bonusTextElement = board.AddText($"+{num}", (int)theBall.GetX(), (int)theBall.GetY());
			if (bonusTextElement != null)
			{
				bonusTextElement.mBonus.mSolidColor = (mLastScoreColor = new Color(Common.gBallColors[theBall.GetColorType()]));
			}
		}
		if (theComboCount > 0)
		{
			BonusText bonusText = null;
			if (mLevel.AllowPointsFromBalls())
			{
				float num4 = Common._M(1.5f);
				float num5 = 1f + (num4 - 1f) / 10f * (float)(theComboCount + 1);
				if (num5 < 1f)
				{
					num5 = 1f;
				}
				BonusTextElement bonusTextElement2 = board.AddText(TextManager.getInstance().getString(100) + $"x{theComboCount + 1}", (int)theBall.GetX(), (int)theBall.GetY(), num5, bonusTextElement?.mHandle ?? (-1), null);
				if (bonusTextElement2 != null)
				{
					bonusText = bonusTextElement2.mBonus;
				}
				mBoard.GetBetaStats().Combo(theComboCount * 100, theComboCount + 1);
			}
			if (bonusText != null)
			{
				bonusText.mSolidColor = mLastScoreColor;
			}
			mLevel.MadeCombo(theComboCount);
		}
		if (theNumGaps >= 1)
		{
			mBoard.GetBetaStats().GapShot(theGapBonus, theNumGaps);
			mBoard.UnlockAchievement(EAchievementType.THREAD_THE_NEEDLE);
		}
		if (theGapBonus > 0)
		{
			mLevel.MadeGapShot(theNumGaps);
			if (mLevel.AllowPointsFromBalls())
			{
				int num6 = Common._DS(20);
				BonusTextElement bonusTextElement3 = null;
				int attach_handle = ((bonusTextElement == null || theComboCount > 0) ? (-1) : bonusTextElement.mHandle);
				if (theNumGaps > 1)
				{
					if (theNumGaps > 3)
					{
						mApp.SetAchievement("double_gap");
						bonusTextElement3 = board.AddText(TextManager.getInstance().getString(101), (int)theBall.GetX(), (int)theBall.GetY() + num6, Common._M(1.5f), attach_handle, null);
						mApp.mUserProfile.mNumTripleGapShots++;
						mApp.mUserProfile.mNumDoubleGapShots++;
					}
					else if (theNumGaps > 2)
					{
						mApp.SetAchievement("double_gap");
						bonusTextElement3 = board.AddText(TextManager.getInstance().getString(102), (int)theBall.GetX(), (int)theBall.GetY() + num6, Common._M(1.35f), attach_handle, null);
						mApp.mUserProfile.mNumTripleGapShots++;
						mApp.mUserProfile.mNumDoubleGapShots++;
					}
					else
					{
						mApp.SetAchievement("double_gap");
						mApp.mUserProfile.mNumDoubleGapShots++;
						bonusTextElement3 = board.AddText(TextManager.getInstance().getString(103), (int)theBall.GetX(), (int)theBall.GetY() + num6, Common._M(1.2f), attach_handle, null);
					}
				}
				else
				{
					bonusTextElement3 = board.AddText(TextManager.getInstance().getString(104), (int)theBall.GetX(), (int)theBall.GetY() + num6, Common._M(1.1f), attach_handle, null);
				}
				if (bonusTextElement3 != null)
				{
					bonusTextElement3.mBonus.mSolidColor = mLastScoreColor;
				}
			}
			if (mLevel.mBoss == null && !mLevel.IsFinalBossLevel())
			{
				board.mApp.mSoundPlayer.Play(Res.GetSoundByID(ResID.SOUND_GAP_BONUS));
				for (int i = 1; i < theNumGaps; i++)
				{
					SoundAttribs soundAttribs = new SoundAttribs();
					soundAttribs.pitch = i * 2;
					soundAttribs.delay = i * 10;
					board.mApp.mSoundPlayer.Play(Res.GetSoundByID(ResID.SOUND_GAP_BONUS), soundAttribs);
				}
			}
		}
		mBoard.GetBetaStats().ChainShot(num3, mBoard.GetNumClearsInARow() + 1);
		if (flag && mLevel.AllowPointsFromBalls())
		{
			float num7 = Common._M(1.5f);
			float num8 = 1f + (num7 - 1f) / 10f * (float)(mBoard.GetNumClearsInARow() + 1 - num2);
			if (num8 < 1f)
			{
				num8 = 1f;
			}
			int attach_handle2 = ((bonusTextElement == null || theGapBonus > 0 || theComboCount > 0) ? (-1) : bonusTextElement.mHandle);
			BonusTextElement bonusTextElement4 = mBoard.AddText(TextManager.getInstance().getString(105) + $" x{mBoard.GetNumClearsInARow() + 1}", (int)theBall.GetX(), (int)theBall.GetY(), num8, attach_handle2, null);
			if (bonusTextElement4 != null)
			{
				bonusTextElement4.mBonus.mSolidColor = mLastScoreColor;
			}
			int num9 = mBoard.GetNumClearsInARow() - 5;
			if (num9 > 10)
			{
				num9 = 10;
			}
			if (mLevel.mBoss != null && !mLevel.IsFinalBossLevel())
			{
				SoundAttribs soundAttribs2 = new SoundAttribs();
				soundAttribs2.pitch = num9;
				soundAttribs2.delay = 1;
				mBoard.mApp.mSoundPlayer.Play(Res.GetSoundByID(ResID.SOUND_CHAIN_BONUS), soundAttribs2);
			}
		}
	}

	private void PlayComboSound(int inComboCount)
	{
		if (!mHadPowerUp)
		{
			SoundAttribs soundAttribs = new SoundAttribs();
			soundAttribs.pitch = 0f;
			soundAttribs.volume = 0.4f + 0.2f * (float)inComboCount;
			if (soundAttribs.volume > 1f)
			{
				soundAttribs.volume = 1f;
			}
			int soundByID = Res.GetSoundByID(ResID.SOUND_BALLDESTROYED5);
			int soundByID2 = Res.GetSoundByID(ResID.SOUND_COMBO);
			switch (inComboCount)
			{
			case 0:
				soundByID = Res.GetSoundByID(ResID.SOUND_BALLDESTROYED1);
				break;
			case 1:
				soundByID = Res.GetSoundByID(ResID.SOUND_BALLDESTROYED2);
				soundAttribs.pitch = 2f;
				break;
			case 2:
				soundByID = Res.GetSoundByID(ResID.SOUND_BALLDESTROYED3);
				soundByID2 = Res.GetSoundByID(ResID.SOUND_COMBO3X);
				break;
			case 3:
				soundByID = Res.GetSoundByID(ResID.SOUND_BALLDESTROYED4);
				break;
			default:
				soundByID2 = Res.GetSoundByID(ResID.SOUND_COMBO3X);
				soundAttribs.pitch = 2f;
				break;
			}
			mApp.mSoundPlayer.Play(soundByID);
			mApp.mSoundPlayer.Play(soundByID2, soundAttribs);
		}
	}

	private void AddPendingBall()
	{
		Ball ball = new Ball();
		int mNumColors = mCurveDesc.mVals.mNumColors;
		int num = ((mPendingBalls.Count() > 0) ? mPendingBalls[mPendingBalls.Count - 1].GetColorType() : ((mBallList.Count() <= 0) ? mLevel.GetRandomPendingBallColor(mNumColors) : mBallList.First().GetColorType()));
		if (num >= mNumColors)
		{
			num = mLevel.GetRandomPendingBallColor(mNumColors);
		}
		int numPendingMatches = GetNumPendingMatches();
		int num2 = (GameApp.gApp.Is3DAccelerated() ? Common._M(30) : Common._M1(33));
		int num3 = (GameApp.gApp.Is3DAccelerated() ? Common._M(4) : Common._M1(4));
		int num6;
		if (!mApp.mUserProfile.HasSeenHint(ZumaProfile.FIRST_SHOT_HINT) && mLevel.mNum == 1 && mLevel.mZone == 1 && mBallList.Count() == num2 - 1)
		{
			ball.SetColorType(1);
		}
		else if (!mApp.mUserProfile.HasSeenHint(ZumaProfile.FIRST_SHOT_HINT) && mLevel.mNum == 1 && mLevel.mZone == 1 && mBallList.Count() == num2 + num3 + 1)
		{
			ball.SetColorType(0);
		}
		else if (!mApp.mUserProfile.HasSeenHint(ZumaProfile.FIRST_SHOT_HINT) && mLevel.mNum == 1 && mLevel.mZone == 1 && mBallList.Count() >= num2 && mBallList.Count() <= num2 + num3)
		{
			ball.SetColorType(2);
		}
		else
		{
			int maxSingles = GameApp.gDDS.GetMaxSingles(mCurveNum);
			int num4 = GameApp.gDDS.GetBallRepeat(mCurveNum) * (int)GameApp.gDDS.mHandheldBalance.mChanceOfSameColorBallIncrease;
			int maxClumps = GameApp.gDDS.GetMaxClumps(mCurveNum);
			int num5 = MathUtils.SafeRand() % 100;
			if (num5 <= num4 && numPendingMatches < maxClumps)
			{
				num6 = num;
			}
			else if (maxSingles < 10 && GetNumPendingSingles(1) == 1 && (maxSingles == 0 || GetNumPendingSingles(10) > maxSingles))
			{
				num6 = num;
			}
			else
			{
				do
				{
					num6 = mLevel.GetRandomPendingBallColor(mNumColors);
				}
				while (num6 == num);
			}
			ball.SetColorType(num6);
		}
		num6 = ball.GetColorType();
		ball.RandomizeFrame();
		mPendingBalls.Add(ball);
		Ball guideBall = mBoard.GetGuideBall();
		if (guideBall != null && guideBall.GetColorType() == num6 && mBoard.GetGun().LightningMode())
		{
			ball.DoElectricOverlay(val: true);
		}
		mNumBallsCreated++;
		if (mCurveDesc.mVals.mNumBalls > 0 && mNumBallsCreated >= mCurveDesc.mVals.mNumBalls)
		{
			mStopAddingBalls = true;
		}
		mLevel.BallCreatedCallback(ball, mNumBallsCreated);
	}

	private void AddBall()
	{
		if (!Common.gAddBalls || (mLevel.mNum == int.MaxValue && mLevel.mZone == 1 && mBoard.mPreventBallAdvancement))
		{
			return;
		}
		if (mPendingBalls.Count() == 0)
		{
			if (mStopAddingBalls || (mLevel.mBoss != null && !mLevel.mBoss.CanAdvanceBalls()))
			{
				return;
			}
			AddPendingBall();
		}
		Ball ball = mPendingBalls.First();
		mWayPointMgr.SetWayPoint(ball, 1f, mLevel.mLoopAtEnd);
		Ball ball2 = null;
		if (mBallList.Count() > 0)
		{
			ball2 = mBallList.First();
			if (mAdvanceSpeed > (float)ball2.GetRadius() && ball2.GetWayPoint() >= 0f)
			{
				if (ball2.GetWayPoint() - mAdvanceSpeed < 5f)
				{
					float num = ball2.GetWayPoint() - (float)ball2.GetRadius() - (float)ball.GetRadius() - 0.001f;
					ball.SetWayPoint(num, mWayPointMgr.InTunnel((int)num));
				}
			}
			else if (ball.GetWayPoint() > ball2.GetWayPoint() || ball.CollidesWith(ball2))
			{
				return;
			}
		}
		ball.InsertInList(mBallList, 0, this);
		ball.UpdateCollisionInfo(5 + (int)mAdvanceSpeed);
		ball.SetNeedCheckCollision(needCheck: true);
		ball.SetRotation(mWayPointMgr.GetRotationForPoint((int)ball.GetWayPoint()));
		ball.SetBackwardsCount(0);
		ball.SetSuckCount(0);
		ball.SetGapBonus(0, 0);
		ball.SetComboCount(0, 0);
		if (mProxBombCounter > 0 && --mProxBombCounter == 0)
		{
			mProxBombCounter = MathUtils.IntRange(mLevel.mBoss.mBombFreqMin, mLevel.mBoss.mBombFreqMax);
			ball.SetPowerType(PowerType.PowerType_ProximityBomb);
			ball.SetPowerCount(mLevel.mBoss.mBombDuration);
		}
		mPendingBalls.RemoveAt(0);
		if (ball.GetWayPoint() > 1f)
		{
			AddBall();
		}
	}

	private void UpdateBalls()
	{
		LevelMgr levelMgr = ((GameApp)GlobalMembers.gSexyApp).GetLevelMgr();
		foreach (Ball mBall in mBallList)
		{
			mBall.Update();
			if (levelMgr.mColorNukeTimeAfterZuma >= 0 && mBall.GetPowerOrDestType() == PowerType.PowerType_ColorNuke && mBoard.HasAchievedZuma() && mBall.GetPowerCount() > levelMgr.mColorNukeTimeAfterZuma)
			{
				mBall.SetPowerCount(levelMgr.mColorNukeTimeAfterZuma);
			}
		}
		foreach (Bullet mBullet in mBulletList)
		{
			mBullet.Update();
		}
	}

	private void AdvanceBalls()
	{
		if (mBallList.Count == 0)
		{
			return;
		}
		bool flag = mLevel.CanAdvanceBalls();
		int num = mDangerPoint;
		float num2 = 1f;
		if (!mBoard.GauntletMode())
		{
			int num3 = mLevel.mZone;
			if (num3 >= DDS.NUM_ADVENTURE_ZONES)
			{
				num3 = DDS.NUM_ADVENTURE_ZONES - 1;
			}
			num2 = GameApp.gDDS.mHandheldBalance.mAdventureModeSpeedDelta[num3];
		}
		else
		{
			int num4 = mLevel.mGauntletMultipliersEarned;
			if (num4 >= DDS.NUM_CHALLENGE_LEVELS)
			{
				num4 = DDS.NUM_CHALLENGE_LEVELS - 1;
			}
			num2 = GameApp.gDDS.mHandheldBalance.mChallengeModeSpeedDelta[num4];
		}
		float num5 = GameApp.gDDS.GetSpeed(mCurveNum) * mSpeedScale * num2;
		if (mCurveDesc.mVals.mAccelerationRate != 0f)
		{
			mCurveDesc.mCurAcceleration += mCurveDesc.mVals.mAccelerationRate;
			num5 += mCurveDesc.mCurAcceleration;
			if (num5 > mCurveDesc.mVals.mMaxSpeed)
			{
				num5 = mCurveDesc.mVals.mMaxSpeed;
			}
		}
		if (mSlowCount != 0 || (mBoard != null && mBoard.GetGun() != null && mBoard.GetGun().LaserMode()))
		{
			num5 /= 4f;
		}
		float slowFactor = GameApp.gDDS.GetSlowFactor(mCurveNum);
		float num6 = mLevel.mPostZumaTimeSpeedInc * num5 + num5;
		float num7 = slowFactor - mLevel.mPostZumaTimeSlowInc * mCurveDesc.mVals.mSlowFactor;
		if (num7 < 1f)
		{
			num7 = 1f;
		}
		if (mBoard.GauntletMode() || mApp.GetLevelMgr().mPostZumaTime == 0 || !mBoard.HasAchievedZuma())
		{
			num6 = num5;
			num7 = slowFactor;
		}
		if (Common.gDieAtEnd)
		{
			if (mFirstChainEnd < mDangerPoint - GameApp.gDDS.GetSlowDistance(mCurveNum) || !mHasReachedCruisingSpeed)
			{
				num5 = num6;
			}
			else if (mFirstChainEnd < mDangerPoint)
			{
				float num8 = (float)(mFirstChainEnd - (mDangerPoint - GameApp.gDDS.GetSlowDistance(mCurveNum))) / (float)GameApp.gDDS.GetSlowDistance(mCurveNum);
				num5 = (1f - num8) * num6 + num8 * num6 / num7;
			}
			else
			{
				num5 /= num7;
				mBoard.SetRollingInDangerZone();
			}
		}
		if (mBoard.GauntletMode() && mBoard.GetStateCount() > 300)
		{
			int farthestBallPercent = GetFarthestBallPercent(ignore_gaps: false);
			int gauntletHurryDist = GameApp.gDDS.GetGauntletHurryDist(mLevel.mNumCurves);
			if (farthestBallPercent < gauntletHurryDist)
			{
				Common._M(1.6f);
				num5 += (float)(gauntletHurryDist - farthestBallPercent) * GameApp.gDDS.GetGauntletHurryMaxSpeed(mLevel.mNumCurves) / (float)gauntletHurryDist;
				if (mAdvanceSpeed < num5)
				{
					mAdvanceSpeed += 0.03f;
				}
			}
		}
		bool flag2 = false;
		if (mDoingClearCurveRollout)
		{
			for (int num9 = mBallList.Count - 1; num9 >= 0; num9--)
			{
				Ball ball = mBallList[num9];
				if (!ball.GetIsExploding())
				{
					if (ball.GetWayPoint() / (float)mWayPointMgr.GetEndPoint() >= mApp.GetLevelMgr().mClearCurveRolloutPct)
					{
						flag2 = true;
					}
					break;
				}
			}
		}
		if (mAdvanceSpeed > num5 && mBoard != null && !mBoard.mPreventBallAdvancement && (!mDoingClearCurveRollout || flag2))
		{
			mDoingClearCurveRollout = false;
			mAdvanceSpeed -= 0.1f;
		}
		else if (mAdvanceSpeed <= num5 && flag2)
		{
			mDoingClearCurveRollout = false;
		}
		if (mAdvanceSpeed < num5)
		{
			mAdvanceSpeed += 0.005f;
			if (mAdvanceSpeed >= num5)
			{
				mAdvanceSpeed = num5;
			}
		}
		float num10 = (flag ? mAdvanceSpeed : 0f);
		if (mOverrideSpeed >= 0f)
		{
			num10 += mOverrideSpeed;
		}
		if (mBoard != null && mBoard.mPreventBallAdvancement)
		{
			num10 = 0f;
		}
		Ball ball2 = mBallList[0];
		float num11 = ball2.GetWayPoint();
		if (!mFirstBallMovedBackwards && mStopTime == 0)
		{
			mWayPointMgr.SetWayPoint(ball2, num11 + num10, mLevel.mLoopAtEnd);
		}
		if (Common.gSuckMode && mStopAddingBalls && mBallList.Count > 0)
		{
			Ball ball3 = mBallList[0];
			mAdvanceSpeed = num5;
			if (ball3.GetSpeedy())
			{
				Ball nextBall = ball3.GetNextBall();
				if (nextBall != null)
				{
					if (nextBall.GetSpeedy())
					{
						mAdvanceSpeed = 20f;
					}
					else
					{
						float num12 = nextBall.GetWayPoint() - ball3.GetWayPoint();
						mAdvanceSpeed = num12 / 10f;
					}
					if (mAdvanceSpeed > 20f)
					{
						mAdvanceSpeed = 20f;
					}
					else if (mAdvanceSpeed < num5)
					{
						mAdvanceSpeed = num5;
					}
				}
			}
		}
		bool flag3 = false;
		int num13 = 0;
		Ball ball4 = null;
		while (num13 != mBallList.Count())
		{
			Ball ball5 = mBallList[num13];
			Ball ball6 = null;
			if (++num13 >= mBallList.Count)
			{
				break;
			}
			ball6 = mBallList[num13];
			float wayPoint = ball6.GetWayPoint();
			float wayPoint2 = ball5.GetWayPoint();
			bool flag4 = false;
			if (wayPoint2 > wayPoint - (float)ball5.GetRadius() - (float)ball6.GetRadius())
			{
				mWayPointMgr.SetWayPoint(ball6, wayPoint2 + (float)ball5.GetRadius() + (float)ball6.GetRadius(), mLevel.mLoopAtEnd);
				flag4 = true;
			}
			if (flag4)
			{
				if (!ball5.GetCollidesWithNext())
				{
					if (ball5.GetSpeedy() && !ball6.GetSpeedy())
					{
						for (Ball ball7 = ball5; ball7 != null; ball7 = ball7.GetPrevBall(mustCollide: true))
						{
							ball7.SetSpeedy(speedy: false);
						}
					}
					ball5.SetCollidesWithNext(collidesWithNext: true);
					mBoard.PlayBallClick(Res.GetSoundByID(ResID.SOUND_BALLCLICK1));
				}
				ball6.GetWayPoint();
				ball5.SetNeedCheckCollision(needCheck: false);
			}
			if (ball4 == null && !ball5.GetCollidesWithNext())
			{
				ball4 = ball5;
				int startDistance = GameApp.gDDS.GetStartDistance(mCurveNum);
				if (ball4.GetWayPoint() < (float)startDistance / 100f * (float)GetCurveLength())
				{
					flag3 = true;
				}
			}
		}
		if (!flag3 && mLevel.mTempSpeedupTimer <= 0)
		{
			mCanCheckForSpeedup = true;
			mOverrideSpeed = -1f;
		}
		if (!mHasReachedRolloutPoint && mBackwardCount <= 0 && mBallList[mBallList.Count - 1].GetWayPoint() >= (float)GameApp.gDDS.GetStartDistance(mCurveNum) / 100f * (float)GetCurveLength())
		{
			mHasReachedRolloutPoint = true;
		}
		if (ball4 == null)
		{
			ball4 = mBallList[mBallList.Count - 1];
			mCanCheckForSpeedup = true;
			if (HasReachedCruisingSpeed())
			{
				flag3 = true;
			}
		}
		mFirstChainEnd = (int)ball4.GetWayPoint();
		if (flag3 && mCanCheckForSpeedup && mLevel.mHurryToRolloutAmt > 0f)
		{
			int startDistance2 = GameApp.gDDS.GetStartDistance(mCurveNum);
			if ((float)mFirstChainEnd < (float)startDistance2 / 100f * (float)GetCurveLength())
			{
				mCanCheckForSpeedup = false;
				mOverrideSpeed = mLevel.mHurryToRolloutAmt;
			}
			else
			{
				mCanCheckForSpeedup = true;
				mOverrideSpeed = -1f;
			}
		}
		if (mFirstChainEnd >= num && Common.gDieAtEnd)
		{
			uint boardTickCount = Common.GetBoardTickCount();
			float num14 = (float)(GetCurveLength() - mFirstChainEnd) / (float)(GetCurveLength() - mDangerPoint);
			uint num15 = (uint)(100f + 4000f * num14);
			int stateCount = mBoard.GetStateCount();
			if (stateCount >= mPathLightEndFrame && boardTickCount - mLastPathShowTick >= num15)
			{
				int farthest_curve = 0;
				mLevel.GetFarthestBallPercent(ref farthest_curve, ignore_gaps: false);
				if (farthest_curve == mCurveNum)
				{
					SoundAttribs soundAttribs = new SoundAttribs();
					soundAttribs.stagger = 10;
					mApp.mSoundPlayer.Play(Res.GetSoundByID(ResID.SOUND_WARNING), soundAttribs);
				}
				mLastPathShowTick = boardTickCount;
				mPathLightEndFrame = stateCount;
				if (mWarningLights.Count() > 0)
				{
					int num16 = mWarningLights.Count();
					for (int i = 0; i < mWarningLights.Count(); i++)
					{
						WarningLight warningLight = mWarningLights[i];
						Image imageByID = Res.GetImageByID(ResID.IMAGE_SKULL_PATH);
						if (warningLight.mWaypoint + (float)(imageByID.mWidth / 2) > (float)mFirstChainEnd)
						{
							num16 = i;
							break;
						}
					}
					if (num16 < mWarningLights.Count())
					{
						mWarningLights[num16].mPulseRate = Common._M(30f) * (1f - num14);
						float num17 = Common._M(10f);
						if (mWarningLights[num16].mPulseRate < num17)
						{
							mWarningLights[num16].mPulseRate = num17;
						}
					}
				}
			}
		}
		bool flag5 = mInDanger;
		mInDanger = mBallList[mBallList.Count - 1].GetWayPoint() >= (float)mDangerPoint && Common.gDieAtEnd;
		if (flag5 != mInDanger && mWarningLights.Count() > 0)
		{
			for (int j = 0; j < mWarningLights.Count(); j++)
			{
				mWarningLights[j].mState = (mInDanger ? 1 : (-1));
			}
		}
	}

	private void AdvanceBackwardBalls()
	{
		if (mLevel.mBoss != null && !mLevel.mBoss.CanAdvanceBalls())
		{
			return;
		}
		mFirstBallMovedBackwards = false;
		if (mBallList.Count == 0)
		{
			return;
		}
		int num = mBallList.Count - 1;
		bool flag = false;
		float num2 = 0f;
		if (mBackwardCount > 0)
		{
			mBallList[mBallList.Count - 1].SetBackwardsSpeed(1f * mSpeedScale);
			mBallList[mBallList.Count - 1].SetBackwardsCount(1);
		}
		while (true)
		{
			Ball ball = mBallList[num];
			int backwardsCount = ball.GetBackwardsCount();
			if (backwardsCount > 0)
			{
				num2 = ball.GetBackwardsSpeed();
				mWayPointMgr.SetWayPoint(ball, ball.GetWayPoint() - num2, mLevel.mLoopAtEnd);
				ball.SetBackwardsCount(backwardsCount - 1);
				flag = true;
			}
			if (num == 0)
			{
				break;
			}
			Ball ball2 = mBallList[--num];
			if (!flag)
			{
				continue;
			}
			if (ball2.GetCollidesWithNext())
			{
				mWayPointMgr.SetWayPoint(ball2, ball2.GetWayPoint() - num2, mLevel.mLoopAtEnd);
				continue;
			}
			float num3 = ball.GetWayPoint() - (float)ball.GetRadius() - (float)ball2.GetRadius();
			if (ball2.GetWayPoint() > num3)
			{
				flag = true;
				if (!ball2.GetCollidesWithNext())
				{
					ball2.SetCollidesWithNext(collidesWithNext: true);
					mBoard.PlayBallClick(Res.GetSoundByID(ResID.SOUND_BALLCLICK1));
				}
				num2 = ball2.GetWayPoint() - num3;
				ball2.SetWayPoint(num3, mWayPointMgr.InTunnel((int)num3));
			}
			else
			{
				flag = false;
			}
		}
		if (flag)
		{
			mFirstBallMovedBackwards = true;
			if (mStopTime < 20)
			{
				mStopTime = 20;
			}
		}
	}

	private void UpdateSuckingBalls()
	{
		if (mLevel.mBoss != null && !mLevel.mBoss.CanAdvanceBalls())
		{
			return;
		}
		int num = 0;
		while (num != mBallList.Count())
		{
			Ball ball = mBallList[num];
			if (!ball.GetSuckBack() && ball.GetSuckCount() > 0)
			{
				UpdateForwardSuckingBalls();
				break;
			}
			int suckCount = ball.GetSuckCount();
			float num2 = (float)(suckCount >> 3) * mSpeedScale;
			if (ball.GetSuckCount() > 0)
			{
				Ball ball2 = null;
				while (num != mBallList.Count())
				{
					ball2 = mBallList[num++];
					ball2.SetSuckCount(0);
					mWayPointMgr.SetWayPoint(ball2, ball2.GetWayPoint() - num2, mLevel.mLoopAtEnd);
					Bullet bullet = ball2.GetBullet();
					if (bullet != null && !bullet.mDoNewMerge)
					{
						Ball pushBall = bullet.GetPushBall();
						if (pushBall != null)
						{
							mWayPointMgr.FindFreeWayPoint(pushBall, bullet, inFront: false, mLevel.mLoopAtEnd);
						}
						bullet.UpdateHitPos();
					}
					if (!ball2.GetCollidesWithNext())
					{
						break;
					}
				}
				Ball ball3 = ball2;
				ball.SetSuckCount(suckCount + 1);
				Ball prevBall = ball.GetPrevBall();
				if (mLevel.mZone == 5 && mLevel.mNum != 10 && mBoard.GetGameState() == GameState.GameState_Playing && mBoard.mUpdateCnt % Common._M(2) == 0)
				{
					for (int i = 0; i < Common._M(1); i++)
					{
						Bubble bubble = new Bubble();
						bubble.Init(Common._M(0), MathUtils.FloatRange(Common._M1(-1.5f), Common._M2(-0.75f)), MathUtils.FloatRange(Common._M3(0.05f), Common._M4(0.2f)), (int)MathUtils.FloatRange(Common._M5(15f), Common._M6(25f)));
						bubble.SetAlphaFade(Common._M(2f));
						bubble.SetX(ball.GetX() + (float)(-10 + MathUtils.SafeRand() % 20));
						bubble.SetY(ball.GetY());
						mLevel.mFrog.AddBubble(bubble);
					}
				}
				if (prevBall != null && prevBall.GetColorType() == ball.GetColorType() && prevBall.GetIsExploding())
				{
					while (prevBall != null)
					{
						prevBall = prevBall.GetPrevBall();
						if (prevBall != null && !prevBall.GetIsExploding())
						{
							break;
						}
					}
				}
				if (prevBall != null && prevBall.GetColorType() == ball.GetColorType())
				{
					bool flag = false;
					float num3 = ball.GetWayPoint() - (float)ball.GetRadius() - (float)prevBall.GetRadius();
					if (prevBall.GetWayPoint() > num3)
					{
						mWayPointMgr.SetWayPoint(prevBall, num3, mLevel.mLoopAtEnd);
						flag = true;
					}
					if (!flag)
					{
						continue;
					}
					mBoard.PlayBallClick(Res.GetSoundByID(ResID.SOUND_BALLCLICK1));
					prevBall.SetCollidesWithNext(collidesWithNext: true);
					ball.SetSuckCount(0);
					int num4 = 5 + 5 * ball.GetComboCount();
					bool flag2 = true;
					if (!CheckSet(ball) || ball.GetSuckFromCompacting())
					{
						ball.SetComboCount(0, 0);
					}
					if (num4 > 40)
					{
						num4 = 40;
					}
					num4 *= 3;
					if (flag2 && ball3.GetBackwardsCount() == 0)
					{
						ball3.SetBackwardsCount(30);
						float num5 = (float)ball.GetComboCount() * 1.5f;
						if (num5 <= 0.5f)
						{
							num5 = 0.5f;
						}
						ball3.SetBackwardsSpeed(num5);
					}
					ClearPendingSucks(ball3);
				}
				else
				{
					ball.SetSuckCount(0);
				}
			}
			else
			{
				num++;
			}
		}
	}

	private void UpdateForwardSuckingBalls()
	{
		int num = mBallList.Count();
		num--;
		while (true)
		{
			Ball ball = mBallList[num];
			int suckCount = ball.GetSuckCount();
			float num2 = (float)(suckCount >> 3) * mSpeedScale;
			if (ball.GetSuckCount() > 0)
			{
				Ball ball2 = null;
				while (true)
				{
					ball2 = mBallList[num];
					ball2.SetSuckCount(0, suck_back: false);
					mWayPointMgr.SetWayPoint(ball2, ball2.GetWayPoint() + num2, mLevel.mLoopAtEnd);
					Bullet bullet = ball2.GetBullet();
					if (bullet != null && !bullet.mDoNewMerge)
					{
						Ball pushBall = bullet.GetPushBall();
						if (pushBall != null)
						{
							mWayPointMgr.FindFreeWayPoint(pushBall, bullet, inFront: false, mLevel.mLoopAtEnd);
						}
						bullet.UpdateHitPos();
					}
					if (!ball2.GetCollidesWithPrev() || num == 0)
					{
						break;
					}
					num--;
				}
				Ball theEndBall = ball2;
				ball.SetSuckCount(suckCount + 1, suck_back: false);
				Ball nextBall = ball.GetNextBall();
				if (nextBall != null)
				{
					bool flag = false;
					float num3 = ball.GetWayPoint() + (float)ball.GetRadius() + (float)nextBall.GetRadius();
					if (nextBall.GetWayPoint() < num3)
					{
						mWayPointMgr.SetWayPoint(nextBall, num3, mLevel.mLoopAtEnd);
						flag = true;
					}
					if (flag)
					{
						nextBall.SetCollidesWithPrev(collidesWithPrev: true);
						ball.SetSuckCount(0, suck_back: true);
						if (!CheckSet(ball))
						{
							ball.SetComboCount(0, 0);
						}
						ClearPendingSucks(theEndBall);
					}
				}
				else
				{
					ball.SetSuckCount(0, suck_back: true);
				}
			}
			else
			{
				if (num == 0)
				{
					break;
				}
				num--;
			}
		}
	}

	public void AddInkSpots(int num, float minrad, float maxrad, float minfade, float maxfade, int fadedelay, int target_mode)
	{
		List<float> list = new List<float>();
		if (target_mode == 1)
		{
			foreach (Ball mBall in mBallList)
			{
				list.Add(mBall.GetWayPoint());
			}
		}
		for (int i = 0; i < num; i++)
		{
			InkBlot inkBlot = new InkBlot();
			inkBlot.mAlpha = 255f;
			inkBlot.mAlphaDec = MathUtils.FloatRange(minfade, maxfade);
			inkBlot.mRadius = Common._DS(MathUtils.FloatRange(minrad, maxrad));
			inkBlot.mDelay = MathUtils.SafeRand() % Common._M(100);
			inkBlot.mFadeDelayTimer = fadedelay;
			inkBlot.mAngle = MathUtils.DegreesToRadians(MathUtils.SafeRand() % 360);
			if (target_mode != 2)
			{
				float num2;
				if (target_mode != 1 || list.Count() == 0)
				{
					num2 = Common._M(100) + MathUtils.SafeRand() % (mWayPointMgr.GetNumPoints() - Common._M1(300));
				}
				else
				{
					int index = MathUtils.SafeRand() % list.Count();
					num2 = list[index];
					list.RemoveAt(index);
				}
				GetXYFromWaypoint((int)num2, out inkBlot.mX, out inkBlot.mY);
			}
			else
			{
				inkBlot.mX = inkBlot.mRadius + (float)(MathUtils.SafeRand() % (int)((float)GlobalMembers.gSexyApp.mWidth - inkBlot.mRadius));
				inkBlot.mY = inkBlot.mRadius + (float)(MathUtils.SafeRand() % (int)((float)GlobalMembers.gSexyApp.mHeight - inkBlot.mRadius));
			}
			mInkSpots.Add(inkBlot);
		}
	}

	public void QuicklyFadeInkSpots()
	{
		for (int i = 0; i < mInkSpots.Count; i++)
		{
			InkBlot inkBlot = mInkSpots[i];
			inkBlot.mDelay = (inkBlot.mFadeDelayTimer = 0);
			inkBlot.mAlphaDec *= Common._M(4);
		}
	}

	public void PowerupExpired(PowerType p)
	{
		mLastCompletedPowerUpFrame[(int)p] = mBoard.GetStateCount();
	}

	public void SetColorHasPowerup(int c, bool val)
	{
		if (val)
		{
			mBallColorHasPowerup[c]++;
		}
		else if (--mBallColorHasPowerup[c] < 0)
		{
			mBallColorHasPowerup[c] = 0;
		}
	}

	public void UpdateSets()
	{
		mHaveSets = false;
		int num = 0;
		while (num < mBallList.Count())
		{
			Ball ball = mBallList[num];
			bool isExploding = ball.GetIsExploding();
			if (isExploding)
			{
				mHaveSets = true;
			}
			if (ball.GetShouldRemove())
			{
				Ball nextBall = ball.GetNextBall();
				Ball prevBall = ball.GetPrevBall();
				if (nextBall != null && !nextBall.GetIsExploding() && prevBall != null && !prevBall.GetShouldRemove() && nextBall.GetColorType() == prevBall.GetColorType())
				{
					nextBall.SetSuckCount(10);
					nextBall.SetComboCount(ball.GetComboCount() + 1, ball.GetComboScore());
					if (mLevel.mZone == 5)
					{
						mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_UNDERWATER_ROLL));
					}
				}
				if (num == 0)
				{
					mAdvanceSpeed = 0f;
					if (mStopTime < 40)
					{
						mStopTime = 40;
					}
				}
				DeleteBall(ball);
				mBallList.RemoveAt(num);
			}
			else
			{
				if (isExploding)
				{
					ball.UpdateExplosion();
				}
				else
				{
					mBoard.mBallColorMap[ball.GetColorType()]++;
				}
				num++;
			}
		}
	}

	public void UpdatePowerUps()
	{
		if (mBallList.Count == 0 || mBoard.mPreventBallAdvancement)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < mNumMultBallsToSpawn; i++)
		{
			Ball ball = AddPowerUp(PowerType.PowerType_GauntletMultBall);
			if (ball != null)
			{
				flag = true;
				mNumMultBallsToSpawn--;
				mBoard.MultiplierBallAdded(ball);
			}
		}
		if (flag)
		{
			return;
		}
		int stateCount = mBoard.GetStateCount();
		if (stateCount < mApp.GetLevelMgr().mPowerDelay)
		{
			return;
		}
		int mPowerupSpawnDelay = mApp.GetLevelMgr().mPowerupSpawnDelay;
		if (mPowerupSpawnDelay > 0 && stateCount - mLastPowerupTime < mPowerupSpawnDelay)
		{
			return;
		}
		int overallPowerupChance = GameApp.gDDS.GetOverallPowerupChance(mCurveNum);
		if (overallPowerupChance == 0)
		{
			return;
		}
		overallPowerupChance -= (int)((float)overallPowerupChance * mLevel.GetPowerIncPct());
		if (overallPowerupChance <= 0)
		{
			overallPowerupChance = 1;
		}
		if (mApp.GetLevelMgr().mUniquePowerupColor)
		{
			int num = 0;
			for (int j = 0; j < mCurveDesc.mVals.mNumColors; j++)
			{
				if (mBallColorHasPowerup[j] > 0)
				{
					num++;
				}
			}
			if (num == mCurveDesc.mVals.mNumColors)
			{
				return;
			}
		}
		if (MathUtils.SafeRand() % overallPowerupChance != 0)
		{
			return;
		}
		int num2 = 0;
		for (int k = 0; k < 14; k++)
		{
			if (CanSpawnPowerUp((PowerType)k))
			{
				num2 += GameApp.gDDS.GetPowerFreq(k, mCurveNum);
			}
		}
		if (num2 == 0)
		{
			return;
		}
		int num3 = MathUtils.SafeRand() % num2;
		int num4 = 0;
		for (int l = 0; l < 14; l++)
		{
			if (!CanSpawnPowerUp((PowerType)l))
			{
				continue;
			}
			int powerFreq = GameApp.gDDS.GetPowerFreq(l, mCurveNum);
			if (num3 < num4 + powerFreq)
			{
				int num5 = mLastSpawnedPowerUpFrame[l];
				bool flag2 = l == 0 && mLevel.GetBossBombDelay() > 0;
				if ((l != 8 || !mLevel.HasPowerup((PowerType)l)) && (!HasPowerup((PowerType)l) || flag2))
				{
					int num6 = mApp.GetLevelMgr().mPowerCooldown;
					if (flag2)
					{
						num6 = mLevel.GetBossBombDelay();
					}
					if (stateCount - num5 >= num6 && (stateCount - mLastCompletedPowerUpFrame[l] >= num6 || mLastCompletedPowerUpFrame[l] <= 0) && mBoard.GetGun().CanSpawnPowerUp((PowerType)l))
					{
						AddPowerUp((PowerType)l);
						mBoard.GetBetaStats().SpawnedPowerup(l);
						mLastPowerupTime = stateCount;
						mLastSpawnedPowerUpFrame[l] = stateCount;
						mNumPowerUpsThisLevel[l]++;
					}
					break;
				}
			}
			else
			{
				num4 += powerFreq;
			}
		}
	}

	public void RemoveBallsAtEnd()
	{
		if (Common.gDieAtEnd || mLevel.mLoopAtEnd || mBallList.Count() == 0)
		{
			return;
		}
		int endPoint = mWayPointMgr.GetEndPoint();
		int count = mBallList.Count;
		count--;
		bool flag = false;
		while (!flag)
		{
			Ball ball = mBallList[count];
			if (!(ball.GetWayPoint() >= (float)endPoint))
			{
				break;
			}
			if (!mLevel.mLoopAtEnd || ball.GetIsExploding())
			{
				if (count != 0)
				{
					count--;
				}
				else
				{
					flag = true;
				}
				DeleteBullet(ball.GetBullet());
				ball.RemoveFromList();
				DeleteBall(ball);
			}
		}
	}

	public void RemoveBallsAtFront()
	{
		if (!mHasReachedCruisingSpeed || mBoard.GauntletMode())
		{
			return;
		}
		int num = 0;
		while (num < mBallList.Count())
		{
			Ball ball = mBallList[num];
			if ((!(ball.GetWayPoint() < (float)mCurveDesc.mCutoffPoint) || !mStopAddingBalls) && !(ball.GetWayPoint() < 1f))
			{
				break;
			}
			num++;
			DeleteBullet(ball.GetBullet());
			ball.RemoveFromList();
			if (!ball.GetIsExploding() && !mStopAddingBalls)
			{
				mPendingBalls.Add(ball);
				continue;
			}
			if (mStopAddingBalls)
			{
				mBoard.IncScore(10, from_balls: true);
			}
			DeleteBall(ball);
			if (mBallList.Count() == 0 && mStopAddingBalls)
			{
				mLastClearedBallPoint = 0;
			}
		}
	}

	public void AdvanceMergingBullet(ref int theBulletItr)
	{
		Bullet bullet = mBulletList[theBulletItr];
		DoMerge(bullet);
		if ((double)bullet.GetHitPercent() >= 1.0)
		{
			Ball hitBall = bullet.GetHitBall();
			int num = hitBall.GetListItr();
			if (bullet.GetHitInFront())
			{
				num++;
			}
			Ball ball = new Ball();
			ball.SetRotation(bullet.GetRotation());
			ball.SetColorType(bullet.GetColorType());
			ball.SetPowerType(bullet.GetPowerType(), delay: false);
			mWayPointMgr.SetWayPoint(ball, bullet.GetWayPoint(), mLevel.mLoopAtEnd);
			ball.ForceFrame(-ball.GetFrame(Res.GetImageByID(ResID.IMAGE_BLUE_BALL)));
			ball.InsertInList(mBallList, num, this);
			int minGapDist = bullet.GetMinGapDist();
			int numGaps = bullet.GetNumGaps();
			bullet.Dispose();
			mBulletList.RemoveAt(theBulletItr++);
			mTotalBalls++;
			Ball prevBall = ball.GetPrevBall();
			Ball nextBall = ball.GetNextBall();
			ball.UpdateCollisionInfo(5);
			ball.SetNeedCheckCollision(needCheck: true);
			if (prevBall != null && ball.GetCollidesWithPrev())
			{
				prevBall.SetNeedCheckCollision(needCheck: true);
			}
			if (minGapDist > 0)
			{
				minGapDist -= 64;
				if (minGapDist < 0)
				{
					minGapDist = 0;
				}
				int num2 = (mBoard.IsEndless() ? 250 : 500);
				int num3 = num2 * (MAX_GAP_SIZE - minGapDist) / MAX_GAP_SIZE;
				num3 = num3 / 10 * 10;
				if (num3 < 10)
				{
					num3 = 10;
				}
				if (num3 > 0)
				{
					if (numGaps > 1)
					{
						num3 *= numGaps;
					}
					ball.SetGapBonus(num3, numGaps);
				}
			}
			if (!CheckSet(ball))
			{
				if (prevBall != null && !prevBall.GetCollidesWithNext() && prevBall.GetColorType() == ball.GetColorType() && prevBall.GetBullet() == null && !prevBall.GetIsExploding())
				{
					ball.SetSuckPending(pending: true);
					ball.SetSuckCount(1);
				}
				else if (nextBall != null && !ball.GetCollidesWithNext() && nextBall.GetColorType() == ball.GetColorType() && nextBall.GetBullet() == null && !nextBall.GetIsExploding())
				{
					ball.SetSuckPending(pending: true);
					if (nextBall.GetSuckCount() <= 0)
					{
						nextBall.SetSuckCount(1);
					}
				}
				else
				{
					mBoard.ResetInARowBonus();
					ball.SetGapBonus(0, 0);
				}
			}
			else
			{
				mBoard.IncNumClearsInARow(1);
			}
		}
		else
		{
			mBoard.mBallColorMap[bullet.GetColorType()]++;
			theBulletItr++;
		}
	}

	public void DoMerge(Bullet theBullet)
	{
		theBullet.CheckSetHitBallToPrevBall();
		Ball hitBall = theBullet.GetHitBall();
		float rotation = theBullet.GetRotation();
		mWayPointMgr.SetWayPoint(theBullet, hitBall.GetWayPoint(), mLevel.mLoopAtEnd);
		mWayPointMgr.FindFreeWayPoint(hitBall, theBullet, theBullet.GetHitInFront(), mLevel.mLoopAtEnd);
		theBullet.SetDestPos(theBullet.GetX(), theBullet.GetY());
		theBullet.SetRotation(rotation, immediate: true);
		theBullet.SetRotation(mWayPointMgr.GetRotationForPoint((int)theBullet.GetWayPoint()), immediate: false);
		theBullet.Update();
		Ball pushBall = theBullet.GetPushBall();
		if (pushBall == null)
		{
			return;
		}
		float num = 1f - theBullet.GetHitPercent();
		int thePad = (int)((float)(-theBullet.GetRadius()) * num / 2f);
		float wayPoint = pushBall.GetWayPoint();
		float num2 = theBullet.GetHitPercent() * theBullet.GetHitPercent() * (float)(pushBall.GetRadius() + theBullet.GetRadius());
		mWayPointMgr.FindFreeWayPoint(theBullet, theBullet.GetPushBall(), inFront: true, mLevel.mLoopAtEnd, thePad);
		if (pushBall.GetWayPoint() - theBullet.GetWayPoint() > num2)
		{
			float num3 = theBullet.GetWayPoint() + num2;
			if (num3 > wayPoint)
			{
				mWayPointMgr.SetWayPoint(pushBall, num3, mLevel.mLoopAtEnd);
			}
			else
			{
				mWayPointMgr.SetWayPoint(pushBall, wayPoint, mLevel.mLoopAtEnd);
			}
		}
		pushBall.SetNeedCheckCollision(needCheck: true);
		if (!gStopSuckbackImmediately)
		{
			Ball nextBall = hitBall.GetNextBall();
			bool hitInFront = theBullet.GetHitInFront();
			if (hitInFront && nextBall != null && nextBall.GetSuckBack() && nextBall.GetSuckCount() > 0 && nextBall.GetColorType() != theBullet.GetColorType())
			{
				nextBall.SetSuckCount(0);
			}
			else if (!hitInFront && hitBall != null && hitBall.GetSuckBack() && hitBall.GetSuckCount() > 0 && hitBall.GetColorType() != theBullet.GetColorType())
			{
				hitBall.SetSuckCount(0);
			}
		}
	}

	public void AdvanceBullets()
	{
		int theBulletItr = 0;
		while (theBulletItr < mBulletList.Count)
		{
			AdvanceMergingBullet(ref theBulletItr);
		}
	}

	public void StartExploding(Ball theBall, bool from_lightning_frog, bool record_levelstats)
	{
		if (!theBall.GetIsExploding())
		{
			if (record_levelstats)
			{
				GameStats levelStats = mBoard.GetLevelStats();
				levelStats.mNumBallsCleared++;
				mBoard.IncNumCleared(1);
				mLevel.BallExploded(theBall.GetColorType());
			}
			mLastClearedBallPoint = (int)theBall.GetWayPoint();
			if (theBall.GetSuckPending())
			{
				theBall.SetSuckPending(pending: false);
			}
			theBall.Explode(mWayPointMgr.InTunnel((int)theBall.GetWayPoint()), from_lightning_frog);
			if (theBall.GetPowerOrDestType() != PowerType.PowerType_Max)
			{
				mNumPowerupsActivated[(int)theBall.GetPowerOrDestType()]++;
				mBoard.ActivatePower(theBall);
				mLastCompletedPowerUpFrame[(int)theBall.GetPowerOrDestType()] = mBoard.GetStateCount();
				mHadPowerUp = true;
			}
		}
	}

	public void StartExploding(Ball theBall)
	{
		StartExploding(theBall, from_lightning_frog: false, record_levelstats: true);
	}

	public void ActivateFrontBomb()
	{
		int num = 0;
		int num2 = mBallList.Count - 1;
		while (num2 >= 0 && num < 7)
		{
			num++;
			Ball ball = mBallList[num2];
			if (!ball.GetIsExploding())
			{
				ball.SetComboCount(mBoard.GetCurComboCount(), mBoard.GetCurComboScore());
				mBoard.mNeedComboCount.Add(ball);
				StartExploding(ball);
			}
			num2--;
		}
	}

	public void ClearPendingSucks(Ball theEndBall)
	{
		Ball ball = theEndBall;
		bool flag = true;
		while (ball != null)
		{
			if (ball.GetSuckPending())
			{
				ball.SetSuckPending(pending: false);
				mBoard.ResetInARowBonus();
				ball.SetGapBonus(0, 0);
			}
			ball = ball.GetPrevBall();
			if (ball == null)
			{
				break;
			}
			if (!ball.GetCollidesWithNext())
			{
				flag = false;
			}
			if (!flag && ball.GetSuckCount() > 0)
			{
				break;
			}
		}
	}

	public void ChangeBallColors(Ball theBall)
	{
	}

	public void DestroyBallsInRegion(int x, int y, int width, int height)
	{
		foreach (Ball mBall in mBallList)
		{
			if (!mBall.GetIsExploding())
			{
				int radius = mBall.GetRadius();
				int num = (int)mBall.GetX() - radius;
				int num2 = (int)mBall.GetY() - radius;
				if (num + radius >= x && num <= x + width && num2 + radius >= y && num2 <= y + height)
				{
					StartExploding(mBall, from_lightning_frog: false, record_levelstats: false);
				}
			}
		}
	}

	public void CompactCurve(bool suck_back)
	{
		foreach (Ball mBall in mBallList)
		{
			if (!mBall.GetIsExploding())
			{
				Ball nextBall = mBall.GetNextBall();
				if (nextBall != null && !mBall.GetCollidesWithNext() && nextBall.GetBullet() == null && !nextBall.GetIsExploding())
				{
					Ball ball = ((!suck_back) ? mBall : nextBall);
					ball.SetSuckPending(pending: true, compact: true);
					ball.SetSuckCount(1, suck_back);
				}
				else if (mBall.GetSuckCount() > 0)
				{
					mBall.SetSuckCount(0);
				}
			}
		}
	}

	public void CompactCurve()
	{
		CompactCurve(suck_back: true);
	}

	public void RollBallsIn(bool from_load)
	{
		mHasReachedCruisingSpeed = false;
		float num = GameApp.gDDS.GetSpeed(mCurveNum) * mSpeedScale;
		int num2 = (mBoard.IsEndless() ? 50 : GameApp.gDDS.GetStartDistance(mCurveNum));
		float num3 = GetCurveLength() * num2 / 100;
		if (mFirstChainEnd > 0)
		{
			num3 -= (float)mFirstChainEnd / (float)GetCurveLength();
			if (num3 <= 0f)
			{
				mAdvanceSpeed = GameApp.gDDS.GetSpeed(mCurveNum) * mSpeedScale;
				return;
			}
		}
		float num4 = 20f * num + 1f;
		float num5 = -20f * num3;
		int num6 = (int)(((double)(0f - num4) + Math.Sqrt(num4 * num4 - 4f * num5)) / 2.0);
		mAdvanceSpeed = num + (float)num6 * 0.1f;
		if (!Common.gAddBalls)
		{
			mHasReachedCruisingSpeed = true;
			mAdvanceSpeed = GameApp.gDDS.GetSpeed(mCurveNum) * mSpeedScale;
		}
	}

	public void RollBallsIn()
	{
		RollBallsIn(from_load: false);
	}

	public CurveMgr(Board theBoard, int curve_num)
	{
		mApp = GameApp.gApp;
		mBoard = theBoard;
		mCurveNum = curve_num;
		mCurveDesc = new CurveDesc();
		mWayPointMgr = new WayPointMgr();
		mIsLoaded = false;
		mPostZumaFlashTimer = 0;
		Reset();
	}

	public virtual void Dispose()
	{
		DeleteBalls();
		mWayPointMgr = null;
		mCurveDesc = null;
	}

	public void Copy(CurveMgr src, Level l, Board b)
	{
		mCurveDesc = null;
		mWayPointMgr = null;
		mBoard = b;
		mWayPointMgr = ((src.mWayPointMgr != null) ? new WayPointMgr(src.mWayPointMgr) : null);
		mCurveDesc = ((src.mCurveDesc != null) ? new CurveDesc(src.mCurveDesc) : null);
		mLevel = l;
		mBulletList.Clear();
		mBallList.Clear();
		mPendingBalls.Clear();
		mIsLoaded = src.mIsLoaded;
		mApp = src.mApp;
		mSpeedScale = src.mSpeedScale;
		mLastScoreColor = src.mLastScoreColor;
		mSparkles.Clear();
		mSparkles.AddRange(src.mSparkles.ToArray());
		mWarningLights.Clear();
		mWarningLights.AddRange(src.mWarningLights.ToArray());
		mPostZumaFlashTimer = src.mPostZumaFlashTimer;
		mLastPowerupTime = src.mLastPowerupTime;
		Array.Copy(src.mLastSpawnedPowerUpFrame, mLastSpawnedPowerUpFrame, src.mLastSpawnedPowerUpFrame.Length);
		Array.Copy(src.mLastCompletedPowerUpFrame, mLastCompletedPowerUpFrame, src.mLastCompletedPowerUpFrame.Length);
		Array.Copy(src.mNumPowerUpsThisLevel, mNumPowerUpsThisLevel, src.mNumPowerUpsThisLevel.Length);
		Array.Copy(src.mNumPowerupsActivated, mNumPowerupsActivated, src.mNumPowerupsActivated.Length);
		Array.Copy(src.mBallColorHasPowerup, mBallColorHasPowerup, src.mBallColorHasPowerup.Length);
		mNumBallsCreated = src.mNumBallsCreated;
		mCurveNum = src.mCurveNum;
		mStopTime = src.mStopTime;
		mProxBombCounter = src.mProxBombCounter;
		mSlowCount = src.mSlowCount;
		mBackwardCount = src.mBackwardCount;
		mTotalBalls = src.mTotalBalls;
		mAdvanceSpeed = src.mAdvanceSpeed;
		mSkullHilite = src.mSkullHilite;
		mSkullHiliteDir = src.mSkullHiliteDir;
		mFirstChainEnd = src.mFirstChainEnd;
		mFirstBallMovedBackwards = src.mFirstBallMovedBackwards;
		mHaveSets = src.mHaveSets;
		mDoingClearCurveRollout = src.mDoingClearCurveRollout;
		mInitialPathHilite = src.mInitialPathHilite;
		mLastPathHiliteWP = src.mLastPathHiliteWP;
		mLastPathHilitePitch = src.mLastPathHilitePitch;
		mDangerPoint = src.mDangerPoint;
		mPathLightEndFrame = src.mPathLightEndFrame;
		mLastClearedBallPoint = src.mLastClearedBallPoint;
		mOverrideSpeed = src.mOverrideSpeed;
		mHadPowerUp = src.mHadPowerUp;
		mStopAddingBalls = src.mStopAddingBalls;
		mInDanger = src.mInDanger;
		mHasReachedCruisingSpeed = src.mHasReachedCruisingSpeed;
		mHasReachedRolloutPoint = src.mHasReachedRolloutPoint;
		mNeedsSpeedup = src.mNeedsSpeedup;
		mCanCheckForSpeedup = src.mCanCheckForSpeedup;
		mLastPathShowTick = src.mLastPathShowTick;
		mNumMultBallsToSpawn = src.mNumMultBallsToSpawn;
		mInkSpots.Clear();
		mInkSpots.AddRange(src.mInkSpots.ToArray());
	}

	public void CopyCurveDataFrom(CurveMgr src)
	{
		string mPath = mCurveDesc.mPath;
		mCurveDesc = new CurveDesc(src.mCurveDesc);
		mCurveDesc.mPath = mPath;
	}

	public void SetLosing()
	{
		mBulletList.Clear();
		foreach (Ball mBall in mBallList)
		{
			mBall.CleanUpMultiplierOverlays();
			mBall.SetSuckCount((int)mAdvanceSpeed * 4, suck_back: true);
		}
	}

	public bool LoadCurve(MirrorType theMirror)
	{
		mSpeedScale = 1f;
		float num = 1f;
		float num2 = 1f;
		float num3 = 0.5f;
		float num4 = 0.67f;
		float num5 = (num4 - num2) / (num3 - num);
		if (mSpeedScale < 1.25f)
		{
			mSpeedScale = num5 * mSpeedScale - num5 * num + num2;
		}
		string curvePath = mLevel.GetCurvePath(mCurveNum);
		if (!mWayPointMgr.LoadCurve(curvePath, mCurveDesc, theMirror))
		{
			return false;
		}
		List<WayPoint> wayPointList = mWayPointMgr.GetWayPointList();
		int x = 0;
		int y = 0;
		float num6 = mCurveDesc.mVals.mSkullRotation;
		if (num6 >= 0f)
		{
			num6 = SexyMath.DegToRad(num6);
		}
		if (wayPointList.Count > 0)
		{
			mWayPointMgr.CalcPerpendicularForPoint(wayPointList.Count - 1);
			WayPoint wayPoint = wayPointList.Last();
			x = (int)wayPoint.x;
			y = (int)wayPoint.y;
			if (num6 < 0f)
			{
				num6 = wayPoint.mRotation;
			}
		}
		mLevel.mHoleMgr.PlaceHole(mCurveNum, x, y, num6, mCurveDesc.mVals.mDrawPit);
		mDangerPoint = mWayPointMgr.GetNumPoints() - mCurveDesc.mDangerDistance;
		if (mDangerPoint >= mWayPointMgr.GetNumPoints())
		{
			mDangerPoint = mWayPointMgr.GetNumPoints() - 1;
		}
		int num7 = Common._M(0);
		for (int i = (int)((float)mDangerPoint * mLevel.mPotPct); i <= mWayPointMgr.GetEndPoint() - Common._M(50); i += Common._M1(50))
		{
			SexyVector2 pointPos = mWayPointMgr.GetPointPos(i);
			SexyVector3 sexyVector = new SexyVector3(pointPos.x, pointPos.y, 0f);
			SexyVector3 sexyVector2 = mWayPointMgr.CalcPerpendicular(i) * num7;
			SexyVector3 sexyVector3 = sexyVector + sexyVector2;
			WarningLight warningLight = new WarningLight(sexyVector3.x, sexyVector3.y);
			mWarningLights.Add(warningLight);
			warningLight.mAngle = mWayPointMgr.GetRotationForPoint(i);
			warningLight.mWaypoint = i;
			warningLight.mPriority = mWayPointMgr.GetPriority(i);
		}
		mIsLoaded = true;
		return true;
	}

	public bool LoadCurve()
	{
		return LoadCurve(MirrorType.MirrorType_None);
	}

	public void StartLevel(bool from_load)
	{
		if ((!from_load || mProxBombCounter <= 0) && mLevel.mBoss != null && mLevel.mBoss.mBombFreqMin > 0)
		{
			mProxBombCounter = MathUtils.IntRange(mLevel.mBoss.mBombFreqMin, mLevel.mBoss.mBombFreqMax);
		}
		mPathLightEndFrame = 0;
		mLastPathShowTick = Math.Min(0u, Common.GetBoardTickCount() - 1000000);
		mLastClearedBallPoint = 0;
		for (int i = 0; i < 14; i++)
		{
			mLastSpawnedPowerUpFrame[i] = mBoard.GetStateCount() - 1000;
			mLastCompletedPowerUpFrame[i] = mBoard.GetStateCount() - 1000;
		}
		DeleteBalls();
		int num = ((mCurveDesc.mVals.mNumBalls > 0 && mCurveDesc.mVals.mNumBalls < 10) ? mCurveDesc.mVals.mNumBalls : 10);
		for (int i = 0; i < num; i++)
		{
			AddPendingBall();
		}
		mStopTime = 0;
		mSlowCount = 0;
		mBackwardCount = 0;
		mTotalBalls = num;
		mNumBallsCreated = mPendingBalls.Count;
		mStopAddingBalls = false;
		if (mCurveDesc.mVals.mNumBalls > 0 && mNumBallsCreated >= mCurveDesc.mVals.mNumBalls)
		{
			mStopAddingBalls = true;
		}
		mInDanger = false;
		mFirstChainEnd = 0;
		mFirstBallMovedBackwards = false;
		Common.gDieAtEnd = !mLevel.mLoopAtEnd && mCurveDesc.mVals.mDieAtEnd;
		RollBallsIn(from_load);
	}

	public void StartLevel()
	{
		StartLevel(from_load: false);
	}

	public bool UpdatePlaying()
	{
		if (mStopAddingBalls && mPostZumaFlashTimer > 0)
		{
			mPostZumaFlashTimer--;
		}
		bool result = false;
		int num = ((mBallList.Count != 0) ? ((int)mBallList.Last().GetWayPoint()) : 0);
		bool flag = mBallList.Count == 0 || num < mCurveDesc.mCutoffPoint;
		if (mStopTime > 0)
		{
			mStopTime--;
			if (flag)
			{
				mStopTime = 0;
			}
			if (mStopTime == 0)
			{
				mAdvanceSpeed = 0f;
			}
		}
		for (int i = 0; i < mInkSpots.Count; i++)
		{
			InkBlot inkBlot = mInkSpots[i];
			if (inkBlot.mDelay > 0)
			{
				inkBlot.mDelay--;
				continue;
			}
			if (inkBlot.mFadeDelayTimer > 0)
			{
				inkBlot.mFadeDelayTimer--;
				continue;
			}
			inkBlot.mAlpha -= inkBlot.mAlphaDec;
			if (inkBlot.mAlpha <= 0f)
			{
				mInkSpots.RemoveAt(i);
				i--;
			}
		}
		if (mInitialPathHilite && !mBoard.mPreventBallAdvancement && mLastPathHiliteWP < mWayPointMgr.GetNumPoints() && mSkullHiliteDir == 0f)
		{
			PathSparkle pathSparkle = new PathSparkle();
			mSparkles.Add(pathSparkle);
			GetPoint(mLastPathHiliteWP, out pathSparkle.mX, out pathSparkle.mY, out pathSparkle.mPri);
			mLastPathHiliteWP += Common._M(10);
			if (mLastPathHiliteWP >= mWayPointMgr.GetNumPoints())
			{
				result = true;
				mSkullHiliteDir = Common._M(12f);
			}
			if (mBoard.mUpdateCnt % Common._M(25) == 0)
			{
				if (mCurveNum != 1 && mLastPathHilitePitch > -20)
				{
					mLastPathHilitePitch--;
				}
				else if (mCurveNum == 1 && mLastPathHilitePitch < 0)
				{
					mLastPathHilitePitch++;
				}
				SoundAttribs soundAttribs = new SoundAttribs();
				soundAttribs.pitch = mLastPathHilitePitch;
				mApp.mSoundPlayer.Play(Res.GetSoundByID(ResID.SOUND_TRAIL_LIGHT), soundAttribs);
			}
			if (mLastPathHiliteWP >= mWayPointMgr.GetNumPoints())
			{
				mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_TRAIL_LIGHT_END));
			}
		}
		Image imageByID = Res.GetImageByID(ResID.IMAGE_PATH_SPARKLES);
		for (int j = 0; j < mSparkles.Count; j++)
		{
			PathSparkle pathSparkle2 = mSparkles[j];
			pathSparkle2.mUpdateCount++;
			if (pathSparkle2.mUpdateCount % Common._M(3) == 0 && ++pathSparkle2.mCel >= imageByID.mNumCols)
			{
				mSparkles.RemoveAt(j);
				j--;
			}
		}
		mSkullHilite += mSkullHiliteDir;
		if (mSkullHiliteDir > 0f && mSkullHilite >= 255f)
		{
			mSkullHilite = 255f;
			mSkullHiliteDir *= -1f;
		}
		else if (mSkullHiliteDir < 0f && mSkullHilite <= 0f)
		{
			mSkullHilite = (mSkullHiliteDir = 0f);
		}
		bool flag2 = false;
		for (int k = 0; k < mWarningLights.Count; k++)
		{
			if (flag2)
			{
				mWarningLights[k].mPulseRate = 0f - mWarningLights[k - 1].mPulseRate;
			}
			flag2 = mWarningLights[k].Update();
			if (mInitialPathHilite)
			{
				mWarningLights[k].mPulseAlpha -= Common._M(5);
				if (mWarningLights[k].mPulseAlpha < 0f)
				{
					mWarningLights[k].mPulseAlpha = 0f;
				}
			}
		}
		if (mInitialPathHilite)
		{
			return result;
		}
		if (mSlowCount > 0)
		{
			mSlowCount--;
			if (flag && !mStopAddingBalls)
			{
				mSlowCount = 0;
			}
		}
		if (mBackwardCount > 0)
		{
			mBackwardCount--;
			if (flag && !mStopAddingBalls)
			{
				mBackwardCount = 0;
			}
		}
		if (Common.gAddBalls)
		{
			AddBall();
		}
		UpdateBalls();
		AdvanceBullets();
		UpdateSuckingBalls();
		if (!mDoingClearCurveRollout && !mLevel.IsFinalBossLevel() && !mBoard.HasAchievedZuma() && !mLevel.mIsEndless && mLevel.mBoss == null && mHasReachedCruisingSpeed && !mInitialPathHilite && !mBoard.mPreventBallAdvancement)
		{
			bool flag3 = mBallList.Count == 0;
			if (!flag3)
			{
				Ball ball = null;
				for (int num2 = mBallList.Count - 1; num2 >= 0; num2--)
				{
					if (!mBallList[num2].GetIsExploding())
					{
						ball = mBallList[num2];
						break;
					}
				}
				if (ball != null)
				{
					int num3 = (int)(ball.GetWayPoint() + (float)ball.GetRadius());
					if (GetXYFromWaypoint(num3, out var x, out var y))
					{
						if (!new Rect(0, 0, Common._SS(mApp.mWidth), Common._SS(mApp.mHeight)).Contains((int)x, (int)y))
						{
							flag3 = true;
						}
						else if (!mLevel.mOffscreenClearBonus)
						{
							int num4 = -1;
							List<WayPoint> wayPointList = mWayPointMgr.GetWayPointList();
							for (int l = 0; l < wayPointList.Count && wayPointList[l].mInTunnel; l++)
							{
								num4 = l;
							}
							if (num4 != -1 && num3 <= num4)
							{
								flag3 = true;
							}
						}
					}
				}
				else
				{
					flag3 = true;
				}
			}
			if (flag3)
			{
				mDoingClearCurveRollout = true;
				mBoard.DoClearCurveBonus(mCurveNum);
				mSlowCount = (mBackwardCount = 0);
				mAdvanceSpeed = GameApp.gDDS.GetSpeed(mCurveNum) * mSpeedScale * mApp.GetLevelMgr().mClearCurveSpeedMult;
			}
		}
		AdvanceBalls();
		AdvanceBackwardBalls();
		RemoveBallsAtFront();
		RemoveBallsAtEnd();
		UpdateSets();
		UpdatePowerUps();
		if (!mHasReachedRolloutPoint && !mBoard.DisplayingTip() && mBoard.GetGameState() == GameState.GameState_Playing && mBallList.Count > 0 && !mHasReachedCruisingSpeed && mLevel.mZone == 5 && mLevel.mNum != 10 && mBoard.mUpdateCnt % Common._M(2) == 0)
		{
			Ball ball2 = mBallList.Last();
			Bubble bubble = new Bubble();
			bubble.Init(0f, MathUtils.FloatRange(Common._M(-0.75f), Common._M1(-0.5f)), MathUtils.FloatRange(Common._M2(0.05f), Common._M3(0.2f)), (int)MathUtils.FloatRange(Common._M4(15), Common._M5(25)));
			bubble.SetAlphaFade(Common._M(1f));
			bubble.SetX(ball2.GetX());
			bubble.SetY(ball2.GetY());
			mLevel.mFrog.AddBubble(bubble);
		}
		if (mBallList.Count > 0)
		{
			SetFarthestBall((int)mBallList.Last().GetWayPoint());
		}
		else
		{
			SetFarthestBall(0);
		}
		if (!mHasReachedCruisingSpeed && mAdvanceSpeed - GameApp.gDDS.GetSpeed(mCurveNum) * mSpeedScale < 0.1f)
		{
			mHasReachedCruisingSpeed = true;
		}
		return result;
	}

	public void UpdateLosing()
	{
		int num = 0;
		int endPoint = mWayPointMgr.GetEndPoint();
		while (num < mBallList.Count())
		{
			Ball ball = mBallList[num];
			if (ball.GetWayPoint() >= (float)endPoint)
			{
				int suckCount = ball.GetSuckCount();
				if (suckCount >= 0)
				{
					mBallList.RemoveAt(num);
					continue;
				}
				ball.SetSuckCount(suckCount + 1);
			}
			else
			{
				mWayPointMgr.SetWayPoint(ball, ball.GetWayPoint() + (float)(ball.GetSuckCount() >> 2), mLevel.mLoopAtEnd);
				ball.SetSuckCount(ball.GetSuckCount() + 1);
				if (ball.GetWayPoint() >= (float)endPoint)
				{
					ball.SetSuckCount(0);
				}
			}
			num++;
		}
		if (mBallList.Count() > 0)
		{
			SetFarthestBall((int)mBallList[mBallList.Count - 1].GetWayPoint());
		}
	}

	public void DrawMisc(Graphics g, int thePriority, bool highest_priority)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_PATH_SPARKLES);
		for (int i = 0; i < mSparkles.Count(); i++)
		{
			g.SetDrawMode(1);
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(255, 255, 0);
			PathSparkle pathSparkle = mSparkles[i];
			if (pathSparkle.mPri == thePriority)
			{
				g.DrawImageCel(imageByID, Common._S(pathSparkle.mX) - imageByID.GetCelWidth() / 2, Common._S(pathSparkle.mY) - imageByID.GetCelHeight() / 2, pathSparkle.mCel);
			}
			g.SetColorizeImages(colorizeImages: false);
			g.SetDrawMode(0);
		}
	}

	public void DrawMisc(Graphics g, int thePriority)
	{
		DrawMisc(g, thePriority, highest_priority: false);
	}

	public void DrawBalls(BallDrawer theDrawer)
	{
		int num = 0;
		foreach (Ball mBall in mBallList)
		{
			int priority = mWayPointMgr.GetPriority(mBall);
			int thePriority = priority;
			Ball nextBall = mBall.GetNextBall(mustCollide: true);
			if (nextBall != null && mWayPointMgr.GetPriority(nextBall) < priority)
			{
				thePriority = mWayPointMgr.GetPriority(nextBall);
			}
			theDrawer.AddBall(mBall, priority);
			theDrawer.AddShadow(mBall, thePriority);
			if (priority > num)
			{
				num = priority;
			}
			if (mBall.HasOverlays())
			{
				theDrawer.AddOverlay(mBall, priority);
			}
			if (mBall.HasUnderlays())
			{
				theDrawer.AddUnderlay(mBall, priority);
			}
		}
		foreach (Bullet mBullet in mBulletList)
		{
			int priority2 = mWayPointMgr.GetPriority(mBullet);
			theDrawer.AddBall(mBullet, priority2);
			theDrawer.AddShadow(mBullet, priority2);
		}
		theDrawer.mMaxBallPriority = num;
	}

	public void DrawAboveBalls(Graphics g)
	{
		if (mBoard.IsPaused())
		{
			return;
		}
		foreach (Ball mBall in mBallList)
		{
			mBall.DrawAboveBalls(g);
		}
	}

	public void DrawUnderBalls(Graphics g)
	{
	}

	public void DrawTopLevel(Graphics g)
	{
		for (int i = 0; i < mInkSpots.Count(); i++)
		{
			InkBlot inkBlot = mInkSpots[i];
			if (inkBlot.mDelay <= 0)
			{
				int num = (int)inkBlot.mAlpha;
				if (mLevel.mBoss != null && !MathUtils._eq(mLevel.mBoss.mAlphaOverride, 255f))
				{
					num = Math.Min(num, (int)mLevel.mBoss.mAlphaOverride);
				}
				if (num != 255)
				{
					g.SetColor(255, 255, 255, num);
					g.SetColorizeImages(colorizeImages: true);
				}
				Transform transform = new Transform();
				if (g.Is3D())
				{
					transform.RotateRad(inkBlot.mAngle);
				}
				Image imageByID = Res.GetImageByID(ResID.IMAGE_BOSS_SQUID_SPLAT);
				float num2 = inkBlot.mRadius / (float)imageByID.mWidth;
				float num3 = (255f - inkBlot.mAlpha) / 255f * Common._M(1f);
				transform.Scale(num2, num2 + num3);
				if (g.Is3D())
				{
					g.DrawImageTransformF(imageByID, transform, inkBlot.mX, inkBlot.mY + num3 * (float)imageByID.mHeight / 2f);
				}
				else
				{
					g.DrawImageTransform(imageByID, transform, inkBlot.mX, inkBlot.mY);
				}
				g.SetColorizeImages(colorizeImages: false);
			}
		}
	}

	public void DrawSkullPathShit(Graphics g, int priority)
	{
		foreach (WarningLight mWarningLight in mWarningLights)
		{
			if (mWarningLight.mPriority == priority)
			{
				mWarningLight.Draw(g);
			}
		}
	}

	public bool CheckCollision(Bullet theBullet, bool should_add)
	{
		Ball ball = null;
		bool flag = false;
		int num = -1;
		int num2 = -1;
		for (num2 = 0; num2 != mBulletList.Count(); num2++)
		{
			Bullet bullet = mBulletList[num2];
			if (theBullet.CollidesWithPhysically(bullet))
			{
				bullet.Update();
				AdvanceMergingBullet(ref num2);
				break;
			}
		}
		bool flag2 = false;
		for (num = 0; num != mBallList.Count(); num++)
		{
			ball = mBallList[num];
			if (ball.CollidesWithPhysically(theBullet, 0) && ball.GetBullet() == null && !ball.GetIsExploding())
			{
				Ball prevBall = ball.GetPrevBall(mustCollide: true);
				if (prevBall == null || prevBall.GetBullet() == null)
				{
					Ball nextBall = ball.GetNextBall(mustCollide: true);
					if (nextBall == null || nextBall.GetBullet() == null)
					{
						SexyVector3 sexyVector = new SexyVector3(ball.GetX(), ball.GetY(), 0f);
						SexyVector3 sexyVector2 = new SexyVector3(theBullet.GetX(), theBullet.GetY(), 0f);
						SexyVector3 v = mWayPointMgr.CalcPerpendicular(ball.GetWayPoint());
						flag = (sexyVector2 - sexyVector).Cross(v).z < 0f;
						if (!mWayPointMgr.InTunnel(ball, flag))
						{
							if (!theBullet.GetIsCannon())
							{
								break;
							}
							mBoard.GetBetaStats().BallExplodedFromPowerup(7);
							mBoard.IncScore(10, from_balls: true);
							if (ball.GetPowerOrDestType() != PowerType.PowerType_Max)
							{
								mApp.SetAchievement("trigger_powerup");
							}
							StartExploding(ball);
							mBoard.PlaySmallExplosionSound();
						}
					}
				}
			}
			if (flag2)
			{
				return true;
			}
		}
		if (num != mBallList.Count())
		{
			if (theBullet.GetIsCannon())
			{
				return true;
			}
			theBullet.SetHitBall(ball, flag);
			theBullet.SetMergeSpeed(mCurveDesc.mMergeSpeed);
			Ball nextBall2 = ball.GetNextBall();
			if (!flag)
			{
				theBullet.RemoveGapInfoForBall(ball.GetId());
			}
			else if (nextBall2 != null)
			{
				theBullet.RemoveGapInfoForBall(nextBall2.GetId());
			}
			if (gStopSuckbackImmediately)
			{
				if (flag && nextBall2 != null && nextBall2.GetSuckBack() && nextBall2.GetSuckCount() > 0 && nextBall2.GetColorType() != theBullet.GetColorType())
				{
					nextBall2.SetSuckCount(0);
				}
				else if (!flag && ball != null && ball.GetSuckBack() && ball.GetSuckCount() > 0 && ball.GetColorType() != theBullet.GetColorType())
				{
					ball.SetSuckCount(0);
				}
			}
			mApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BALLCLICK2));
			float wayPoint = ball.GetWayPoint();
			int num3 = Common._M(80);
			if (mWayPointMgr.CheckDiscontinuity((int)wayPoint - num3, 2 * num3))
			{
				theBullet.mDoNewMerge = true;
			}
			mBulletList.Add(theBullet);
			return true;
		}
		return false;
	}

	public bool CheckCollision(Bullet theBullet)
	{
		return CheckCollision(theBullet, should_add: true);
	}

	public bool CheckGapShot(Bullet theBullet)
	{
		int num = theBullet.GetRadius() * 2;
		float num2 = theBullet.GetRadius();
		float num3 = num2 * num2;
		float x = theBullet.GetX();
		float y = theBullet.GetY();
		List<WayPoint> wayPointList = mWayPointMgr.GetWayPointList();
		int num4 = wayPointList.Count();
		int curCurvePoint = theBullet.GetCurCurvePoint(mCurveNum);
		if (curCurvePoint > 0 && curCurvePoint < num4)
		{
			WayPoint wayPoint = wayPointList[curCurvePoint];
			float num5 = wayPoint.x - x;
			float num6 = wayPoint.y - y;
			if (num5 * num5 + num6 * num6 < num3)
			{
				return false;
			}
			theBullet.SetCurCurvePoint(mCurveNum, 0);
		}
		for (int i = 1; i < num4; i += num)
		{
			WayPoint wayPoint2 = wayPointList[i];
			if (wayPoint2.mInTunnel)
			{
				continue;
			}
			float num7 = wayPoint2.x - x;
			float num8 = wayPoint2.y - y;
			if (!(num7 * num7 + num8 * num8 < num3))
			{
				continue;
			}
			theBullet.SetCurCurvePoint(mCurveNum, i);
			int num9 = 0;
			int theBallId = 0;
			for (int j = 0; j < mBallList.Count; j++)
			{
				Ball ball = mBallList[j];
				if (!(ball.GetWayPoint() > (float)i))
				{
					continue;
				}
				Ball prevBall = ball.GetPrevBall();
				if (prevBall == null)
				{
					break;
				}
				if (ball.GetIsExploding())
				{
					int num10 = j;
					num10++;
					bool flag = false;
					for (; num10 != mBallList.Count; num10++)
					{
						Ball ball2 = mBallList[num10];
						if (!ball2.GetIsExploding())
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						break;
					}
				}
				num9 = (int)(ball.GetWayPoint() - prevBall.GetWayPoint());
				theBallId = ball.GetId();
				break;
			}
			if (num9 > 0)
			{
				return theBullet.AddGapInfo(mCurveNum, num9, theBallId);
			}
			return false;
		}
		return false;
	}

	public bool RemoveBall(Ball theBall)
	{
		if (theBall.GetBullet() != null || theBall.GetIsExploding())
		{
			return false;
		}
		if (mBallList.IndexOf(theBall) != -1)
		{
			Ball nextBall = theBall.GetNextBall();
			Ball prevBall = theBall.GetPrevBall();
			nextBall?.SetCollidesWithPrev(collidesWithPrev: false);
			prevBall?.SetCollidesWithNext(collidesWithNext: false);
			theBall.RemoveFromList();
			DeleteBall(theBall);
			return true;
		}
		return false;
	}

	public void AddPendingBall(Ball theBall)
	{
		mPendingBalls.Add(theBall);
	}

	public void DoBackwards()
	{
		if (mBallList.Count() != 0)
		{
			mBackwardCount = 300;
		}
	}

	public void DoSlowdown()
	{
		if (mSlowCount < 1000)
		{
			mSlowCount = 800;
		}
	}

	public int GetRandomPendingBallColor(int theMaxNumBalls)
	{
		int num = MathUtils.SafeRand() % theMaxNumBalls;
		if (num >= mPendingBalls.Count())
		{
			num = mPendingBalls.Count() - 1;
		}
		return mPendingBalls[num].GetColorType();
	}

	public bool HasPendingBallOfType(int theType, int theMaxNumBalls)
	{
		int num = 0;
		int num2 = 0;
		while (num2 != mPendingBalls.Count() && num < theMaxNumBalls)
		{
			if (mPendingBalls[num2].GetColorType() == theType)
			{
				return true;
			}
			num2++;
			num++;
		}
		return false;
	}

	public bool IsLosing()
	{
		if (mHaveSets || mBallList.Count() == 0 || mLevel.mLoopAtEnd || mBallList[mBallList.Count() - 1].GetWayPoint() < (float)mWayPointMgr.GetEndPoint() || mBulletList.Count() != 0 || mBackwardCount > 0)
		{
			return false;
		}
		for (Ball ball = mBallList[mBallList.Count() - 1]; ball != null; ball = ball.GetPrevBall(mustCollide: true))
		{
			if (ball.GetSuckCount() > 0)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsWinning()
	{
		if (mBallList.Count() == 0 && mPendingBalls.Count() == 0 && mLevel.mBoss == null)
		{
			return true;
		}
		return false;
	}

	public bool CanRestart()
	{
		return mBallList.Count() == 0;
	}

	public bool CanFire()
	{
		if (mBallList.Count() == 0)
		{
			return true;
		}
		if (!(mBallList[mBallList.Count() - 1].GetWayPoint() < (float)mWayPointMgr.GetEndPoint()))
		{
			return mLevel.mLoopAtEnd;
		}
		return true;
	}

	public bool CanCompact()
	{
		foreach (Ball mBall in mBallList)
		{
			if (mBall.GetIsExploding() || mBall.GetBullet() != null)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsStationary()
	{
		foreach (Ball mBall in mBallList)
		{
			if (mBall.GetIsExploding() || mBall.GetSuckCount() > 0 || mBall.GetBackwardsCount() > 0 || mBall.GetSuckPending())
			{
				return false;
			}
		}
		return true;
	}

	public bool IsInDanger()
	{
		return mInDanger;
	}

	public bool HasGaps()
	{
		int numPoints = mWayPointMgr.GetNumPoints();
		foreach (Ball mBall in mBallList)
		{
			Ball nextBall = mBall.GetNextBall();
			Ball prevBall = mBall.GetPrevBall();
			if (nextBall != null && (int)nextBall.GetWayPoint() % numPoints > (int)mBall.GetWayPoint() % numPoints && (!nextBall.CollidesWithPhysically(mBall, 1) || nextBall.GetIsExploding()) && nextBall.GetBullet() == null)
			{
				return true;
			}
			if (prevBall != null && (int)prevBall.GetWayPoint() % numPoints < (int)mBall.GetWayPoint() % numPoints && (!prevBall.CollidesWithPhysically(mBall, 1) || prevBall.GetIsExploding()) && prevBall.GetBullet() == null)
			{
				return true;
			}
		}
		return false;
	}

	public int GetDistanceToDeath()
	{
		if (!mInDanger || mBallList.Count() == 0)
		{
			return -1;
		}
		int num = mWayPointMgr.GetNumPoints() - (int)mBallList[mBallList.Count() - 1].GetWayPoint();
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	public int GetDangerDistance()
	{
		return mWayPointMgr.GetNumPoints() - mDangerPoint;
	}

	public void SetPath(string s)
	{
		mCurveDesc.mPath = s;
	}

	public string GetPath()
	{
		return mCurveDesc.mPath;
	}

	public Ball CheckBallIntersection(SexyVector3 p1, SexyVector3 v1, ref float t, bool skip_exploding)
	{
		Ball result = null;
		int num = 0;
		int num2 = 0;
		while (num2 != mBallList.Count())
		{
			Ball ball = mBallList[num2];
			if (!mWayPointMgr.InTunnel((int)ball.GetWayPoint()))
			{
				float t2 = 0f;
				if ((!ball.GetIsExploding() || !skip_exploding) && ball.Intersects(p1, v1, ref t2) && t2 < t && t2 > 0f)
				{
					result = ball;
					t = t2;
				}
			}
			num2++;
			num++;
		}
		return result;
	}

	public Ball CheckBallIntersection(SexyVector3 p1, SexyVector3 v1, ref float t)
	{
		return CheckBallIntersection(p1, v1, ref t, skip_exploding: false);
	}

	public void ActivateProximityBomb(Ball theBall)
	{
		int num = 56;
		foreach (Ball mBall in mBallList)
		{
			if (!mBall.GetIsExploding() && mBall.CollidesWithPhysically(theBall, num))
			{
				mBall.SetComboCount(mBoard.GetCurComboCount(), mBoard.GetCurComboScore());
				mBoard.mNeedComboCount.Add(mBall);
				StartExploding(mBall);
				mBoard.GetBetaStats().BallExplodedFromPowerup(0);
			}
		}
		if (mLevel.mZone == 5 && mLevel.mNum != 10 && mBoard.GetGameState() == GameState.GameState_Playing)
		{
			int num2 = (int)Common._M(15f);
			float num3 = 6.28318f / (float)num2;
			for (int i = 0; i < num2; i++)
			{
				Bubble bubble = new Bubble();
				float vx = (float)Math.Cos((double)num3 * (double)i) * MathUtils.FloatRange(Common._M(1f), Common._M1(1.25f));
				float vy = ((Common._M(0f) == 0f) ? MathUtils.FloatRange(Common._M(-0.75f), Common._M1(-0.5f)) : ((0f - (float)Math.Sin((double)num3 * (double)i)) * MathUtils.FloatRange(Common._M(0.75f), Common._M1(1.25f))));
				bubble.Init(vx, vy, MathUtils.FloatRange(Common._M(0.05f), Common._M1(0.2f)), (int)MathUtils.FloatRange(Common._M2(15f), Common._M3(25f)));
				bubble.SetAlphaFade(Common._M(3f));
				if (Common._M(0f) != 0f)
				{
					bubble.SetX(theBall.GetX() + (float)(-10 + MathUtils.SafeRand() % 20));
					bubble.SetY(theBall.GetY());
				}
				else
				{
					float num4 = MathUtils.FloatRange(0f, 6.28318f);
					float num5 = MathUtils.SafeRand() % num;
					bubble.SetX(num5 * (float)Math.Cos(num4) + theBall.GetX());
					bubble.SetY(num5 * (float)Math.Sin(num4) * (float)Common._M(1) + theBall.GetY());
				}
				bubble.SetDelay(Common._M(15));
				mLevel.mFrog.AddBubble(bubble);
			}
		}
		mLevel.ProximityBombActivated(theBall.GetX(), theBall.GetY(), 56);
	}

	public void ActivateProximityBomb(int centerx, int centery, int radius)
	{
	}

	public void ActivatePower(Ball theBall)
	{
		PowerType powerOrDestType = theBall.GetPowerOrDestType();
		gGotPowerUp[(int)powerOrDestType] = true;
		switch (powerOrDestType)
		{
		case PowerType.PowerType_ProximityBomb:
			ActivateProximityBomb(theBall);
			break;
		case PowerType.PowerType_MoveBackwards:
			DoBackwards();
			break;
		case PowerType.PowerType_SlowDown:
			DoSlowdown();
			break;
		}
	}

	public bool DoLazerExplosion(Ball b)
	{
		if (b.GetBullet() != null || b.GetIsExploding())
		{
			return false;
		}
		if (mBallList.IndexOf(b) != -1)
		{
			if (mLevel.AllowPointsFromBalls())
			{
				mBoard.IncScore(10, from_balls: true);
			}
			mBoard.GetBetaStats().BallExplodedFromPowerup(9);
			if (b.GetPowerOrDestType() != PowerType.PowerType_Max)
			{
				mApp.SetAchievement("trigger_powerup");
			}
			StartExploding(b);
			return true;
		}
		return false;
	}

	public void DrawCurve(Graphics g)
	{
		mWayPointMgr.DrawCurve(g, new Color(255, 0, 0), mDangerPoint);
	}

	public void DrawTunnel(Graphics g, int priority)
	{
		mWayPointMgr.DrawTunnel(g, priority);
	}

	public void DeleteBalls()
	{
		mBallList.Clear();
		mPendingBalls.Clear();
		mBulletList.Clear();
	}

	public void GetPoint(int thePoint, out int x, out int y, out int pri)
	{
		List<WayPoint> wayPointList = mWayPointMgr.GetWayPointList();
		if (thePoint < 0)
		{
			thePoint = 0;
		}
		if (thePoint >= wayPointList.Count())
		{
			thePoint = wayPointList.Count() - 1;
		}
		WayPoint wayPoint = wayPointList[thePoint];
		x = (int)wayPoint.x;
		y = (int)wayPoint.y;
		pri = wayPoint.mPriority;
	}

	public int GetCurveLength()
	{
		return mWayPointMgr.GetNumPoints();
	}

	public int GetTotalBalls()
	{
		if (mCurveDesc.mVals.mNumBalls == 0)
		{
			return 0;
		}
		return mTotalBalls;
	}

	public void ZumaAchieved(bool stop)
	{
		if (mStopAddingBalls == stop)
		{
			return;
		}
		if (GetFarthestBallPercent() > GameApp.gDDS.GetRollbackPct(mCurveNum))
		{
			mBackwardCount = GameApp.gDDS.GetZumaBack(mCurveNum);
			if (!mBoard.GauntletMode())
			{
				mSlowCount = GameApp.gDDS.GetZumaSlow(mCurveNum);
			}
		}
		if (!mBoard.GauntletMode())
		{
			mStopAddingBalls = stop;
			mPostZumaFlashTimer = Common._M(50);
			if (stop)
			{
				mPendingBalls.Clear();
			}
		}
		else
		{
			mHasReachedCruisingSpeed = false;
			mHasReachedRolloutPoint = false;
			mCanCheckForSpeedup = false;
		}
	}

	public void DoEndlessZumaEffect()
	{
		if (GetFarthestBallPercent() > 50)
		{
			mBackwardCount = GameApp.gDDS.GetZumaBack(mCurveNum);
		}
	}

	private static void GetNumPendingSinglesHelper(int aColor, ref int aNumGroups, ref int aPrevColor, ref int aNumSingles, ref int aGroupCount)
	{
		if (aColor != aPrevColor)
		{
			if (aGroupCount == 1)
			{
				aNumSingles++;
			}
			aGroupCount = 1;
			aNumGroups++;
			aPrevColor = aColor;
		}
		else
		{
			aGroupCount++;
		}
	}

	public void DetonateBalls(int theType, bool from_lightning_frog, bool allow_powerups)
	{
		foreach (Ball mBall in mBallList)
		{
			if (mBall.GetIsExploding() || (theType != -1 && mBall.GetColorType() != theType))
			{
				continue;
			}
			if (!allow_powerups)
			{
				mBall.Explode(in_tunnel: true, from_lightning_frog);
			}
			else
			{
				if (mBall.GetPowerOrDestType() != PowerType.PowerType_Max)
				{
					mApp.SetAchievement("trigger_powerup");
				}
				StartExploding(mBall, from_lightning_frog, record_levelstats: true);
			}
			if (mLevel.AllowPointsFromBalls() && from_lightning_frog)
			{
				mBoard.IncScore(10, from_balls: true);
			}
			if (from_lightning_frog)
			{
				mLevel.IncNumBallsExploded(1);
			}
			if (from_lightning_frog)
			{
				mBoard.GetBetaStats().BallExplodedFromPowerup(8);
			}
		}
	}

	public void DetonateBalls()
	{
		DetonateBalls(-1, from_lightning_frog: false, allow_powerups: false);
	}

	public int GetFarthestBallPercent(bool ignore_gaps)
	{
		if (mBallList.Count() == 0)
		{
			return 0;
		}
		float num = mBallList.Last().GetWayPoint();
		if (!ignore_gaps)
		{
			foreach (Ball mBall in mBallList)
			{
				if (!mBall.GetCollidesWithNext())
				{
					num = mBall.GetWayPoint();
					break;
				}
			}
		}
		return (int)(num * 100f / (float)mWayPointMgr.GetNumPoints());
	}

	public int GetFarthestBallPercent()
	{
		return GetFarthestBallPercent(ignore_gaps: true);
	}

	public int GetNearestBallPercent()
	{
		if (mBallList.Count() == 0)
		{
			return 0;
		}
		float wayPoint = mBallList.First().GetWayPoint();
		return (int)(wayPoint * 100f / (float)mWayPointMgr.GetNumPoints());
	}

	public void GetNearestBallXY(out float x, out float y)
	{
		if (mBallList.Count() == 0)
		{
			x = (y = -2.1474836E+09f);
			return;
		}
		x = mBallList.First().GetX();
		y = mBallList.First().GetY();
	}

	public void GetFarthestBallXY(out float x, out float y)
	{
		if (mBallList.Count() == 0)
		{
			x = (y = -2.1474836E+09f);
			return;
		}
		x = mBallList.Last().GetX();
		y = mBallList.Last().GetY();
	}

	public int GetLastClearedBallPoint()
	{
		return mLastClearedBallPoint;
	}

	public bool HasReachedCruisingSpeed()
	{
		return mHasReachedCruisingSpeed;
	}

	public bool HasReachedRolloutPoint()
	{
		return mHasReachedRolloutPoint;
	}

	public Ball AddPowerUp(PowerType thePower)
	{
		if (!mApp.GetLevelMgr().mUniquePowerupColor && thePower != PowerType.PowerType_GauntletMultBall)
		{
			int index = MathUtils.SafeRand() % mBallList.Count();
			Ball ball = mBallList[index];
			if (ball.GetPowerType() == PowerType.PowerType_Max && ball.GetDestPowerType() == PowerType.PowerType_Max)
			{
				ball.SetPowerType(thePower);
				return ball;
			}
			return null;
		}
		List<Ball> list = new List<Ball>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < mCurveDesc.mVals.mNumColors; i++)
		{
			if (mBallColorHasPowerup[i] == 0 || thePower == PowerType.PowerType_GauntletMultBall)
			{
				list2.Add(i);
			}
		}
		Ball ball2 = null;
		while (list2.Count() > 0 && ball2 == null)
		{
			int index2 = MathUtils.SafeRand() % list2.Count();
			int num = list2[index2];
			foreach (Ball mBall in mBallList)
			{
				if (mBall.GetPowerType() == PowerType.PowerType_Max && mBall.GetDestPowerType() == PowerType.PowerType_Max && mBall.GetColorType() == num && !mBall.GetIsExploding() && (thePower != PowerType.PowerType_GauntletMultBall || (thePower == PowerType.PowerType_GauntletMultBall && !mWayPointMgr.InTunnel(mBall, inFront: true) && !mWayPointMgr.InTunnel(mBall, inFront: false) && mBall.GetWayPoint() / (float)mWayPointMgr.GetEndPoint() >= mApp.GetLevelMgr().mMinMultBallDistance)))
				{
					list.Add(mBall);
				}
			}
			if (mLevel.mPowerupRegions.Count() > 0)
			{
				foreach (PowerupRegion mPowerupRegion in mLevel.mPowerupRegions)
				{
					if (mPowerupRegion.mCurveNum != mCurveNum || SexyFramework.Common.Rand(mPowerupRegion.mChance) != 0)
					{
						continue;
					}
					int num2 = (int)mPowerupRegion.mCurvePctStart * mWayPointMgr.GetNumPoints();
					int num3 = (int)mPowerupRegion.mCurvePctEnd * mWayPointMgr.GetNumPoints();
					foreach (Ball item in list)
					{
						if (item.GetWayPoint() >= (float)num2 && item.GetWayPoint() <= (float)num3)
						{
							ball2 = item;
							break;
						}
					}
					break;
				}
			}
			if (list.Count() == 0)
			{
				list2.RemoveAt(index2);
			}
			else if (ball2 == null)
			{
				ball2 = list[MathUtils.SafeRand() % list.Count()];
			}
		}
		if (ball2 != null)
		{
			ball2.SetPowerType(thePower);
			if (thePower == PowerType.PowerType_GauntletMultBall)
			{
				ball2.SetPowerCount(mApp.GetLevelMgr().mMultBallLife);
			}
		}
		return ball2;
	}

	public void ElectrifyBalls(int theType, bool val)
	{
		foreach (Ball mBall in mBallList)
		{
			if (!mBall.GetIsExploding() && (mBall.GetColorType() == theType || theType == -1))
			{
				mBall.DoElectricOverlay(val);
			}
		}
		foreach (Ball mPendingBall in mPendingBalls)
		{
			if (!mPendingBall.GetIsExploding() && (mPendingBall.GetColorType() == theType || theType == -1))
			{
				mPendingBall.DoElectricOverlay(val);
			}
		}
	}

	public Ball GetBallFromWaypoint(int wp)
	{
		List<WayPoint> wayPointList = mWayPointMgr.GetWayPointList();
		WayPoint wayPoint = wayPointList[wp];
		foreach (Ball mBall in mBallList)
		{
			if (mBall.Contains((int)wayPoint.x, (int)wayPoint.y))
			{
				return mBall;
			}
		}
		return null;
	}

	public bool AtRest()
	{
		if (mBallList.Count() == 0)
		{
			return true;
		}
		foreach (Ball mBall in mBallList)
		{
			if (mBall.GetIsExploding() || mBall.GetSuckCount() > 0 || mBall.GetBullet() != null)
			{
				return false;
			}
		}
		return true;
	}

	public int GetWaypointFromXY(float x, float y, ref bool is_tunnel, uint skip_amt, uint dist_squared)
	{
		List<WayPoint> wayPointList = mWayPointMgr.GetWayPointList();
		int num;
		for (num = 1; num < wayPointList.Count(); num++)
		{
			WayPoint wayPoint = wayPointList[num];
			float num2 = wayPoint.x - x;
			float num3 = wayPoint.y - y;
			if (num2 * num2 + num3 * num3 <= (float)dist_squared)
			{
				is_tunnel = wayPoint.mInTunnel;
				return num;
			}
			num += (int)skip_amt;
		}
		return -1;
	}

	public int GetWaypointFromXY(float x, float y, ref bool is_tunnel)
	{
		return GetWaypointFromXY(x, y, ref is_tunnel, 0u, 25u);
	}

	public bool GetXYFromWaypoint(int waypoint, out float x, out float y)
	{
		List<WayPoint> wayPointList = mWayPointMgr.GetWayPointList();
		if (waypoint >= wayPointList.Count() || waypoint < 0)
		{
			x = 0f;
			y = 0f;
			return false;
		}
		WayPoint wayPoint = wayPointList[waypoint];
		x = wayPoint.x;
		y = wayPoint.y;
		return true;
	}

	public bool AllowsPowerup(PowerType p)
	{
		return mCurveDesc.mVals.mPowerUpFreq[(int)p] > 0;
	}

	public bool HasPowerup(PowerType p)
	{
		foreach (Ball mBall in mBallList)
		{
			if (mBall.GetPowerOrDestType() == p)
			{
				return true;
			}
		}
		return false;
	}

	public void StopAddingBalls()
	{
		mStopAddingBalls = true;
	}

	public bool HasBall(Ball b)
	{
		return mBallList.IndexOf(b) != -1;
	}

	public int GetBallIndex(Ball b)
	{
		if (!HasBall(b))
		{
			return -1;
		}
		int result = 0;
		foreach (Ball mBall in mBallList)
		{
			if (mBall == b)
			{
				return result;
			}
		}
		return -1;
	}
}
