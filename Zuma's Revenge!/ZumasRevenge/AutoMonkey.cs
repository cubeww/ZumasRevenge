using System;
using System.Collections.Generic;
using System.Linq;
using SexyFramework;
using SexyFramework.Drivers;

namespace ZumasRevenge;

public class AutoMonkey
{
	public MonkeyMode mAutoMonkeyMode;

	public float mAutoMonkeyDelay;

	public bool mEnableAutoMonkey;

	protected GameApp mApp;

	protected List<MonkeyState> mStateList = new List<MonkeyState>();

	protected int mStateCount;

	protected List<GamepadButton> mAllowedButtons = new List<GamepadButton>();

	protected List<GamepadButton> mDirectionButtons = new List<GamepadButton>();

	protected int mLastButtonPress;

	protected int mMoveDir;

	protected int mRandomButtonPress;

	protected MonkeyMode mAllModesMode;

	public AutoMonkey(GameApp app)
	{
		mApp = app;
		mStateList.Add(MonkeyState.IntroScreen);
		mAllModesMode = MonkeyMode.PlayThroughGame;
		mAutoMonkeyMode = MonkeyMode.PlayThroughGame;
		mLastButtonPress = 0;
		mStateCount = 0;
		mRandomButtonPress = 0;
		mMoveDir = 2;
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_DPAD_UP);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_DPAD_DOWN);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_DPAD_LEFT);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_DPAD_RIGHT);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_UP);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_DOWN);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_LEFT);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_RIGHT);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_BACK);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_START);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_A);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_B);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_X);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_Y);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_LB);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_RB);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_LTRIGGER);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_RTRIGGER);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_LSTICK);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_RSTICK);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_DPAD_UP);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_DPAD_DOWN);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_DPAD_RIGHT);
		mAllowedButtons.Add(GamepadButton.GAMEPAD_BUTTON_DPAD_LEFT);
		mDirectionButtons.Add(GamepadButton.GAMEPAD_BUTTON_UP);
		mDirectionButtons.Add(GamepadButton.GAMEPAD_BUTTON_DOWN);
		mDirectionButtons.Add(GamepadButton.GAMEPAD_BUTTON_RIGHT);
		mDirectionButtons.Add(GamepadButton.GAMEPAD_BUTTON_LEFT);
		mAutoMonkeyDelay = 0.3f;
		mEnableAutoMonkey = false;
	}

	~AutoMonkey()
	{
	}

	public void Update()
	{
		mLastButtonPress++;
		mStateCount++;
		mRandomButtonPress++;
		switch (mStateList.Last())
		{
		case MonkeyState.IntroScreen:
			UpdateIntroScreen();
			break;
		case MonkeyState.MainMenu:
			UpdateMainMenu();
			break;
		case MonkeyState.ModalOkDialog:
			UpdateModalDialog();
			break;
		case MonkeyState.ModalYesNoDialog:
			UpdateYesNoDialog();
			break;
		case MonkeyState.Playing:
			UpdatePlaying();
			break;
		case MonkeyState.PauseDialog:
			break;
		}
	}

	public void SetState(MonkeyState state)
	{
		mStateList.Add(state);
		mStateCount = 0;
		mLastButtonPress = 0;
	}

	public void RemoveLastInstanceOfState(MonkeyState state)
	{
		bool flag = false;
		int num = mStateList.Count - 1;
		while (num >= 0 && !flag)
		{
			if (mStateList[num] == state)
			{
				mStateList.RemoveAt(num);
				flag = true;
			}
			num--;
		}
		if (!flag)
		{
			Console.WriteLine("Unable to find state '{0}' to remove from AutoMonkey!!", GetStateString(state));
		}
		mStateCount = 0;
		mLastButtonPress = 0;
	}

	public MonkeyMode GetMode()
	{
		return mAutoMonkeyMode;
	}

	public bool IsEnabled()
	{
		return GetMode() != MonkeyMode.Disabled;
	}

	protected void UpdateIntroScreen()
	{
		if (mAutoMonkeyDelay <= (float)mLastButtonPress / 100f)
		{
			PressButtonDown(GamepadButton.GAMEPAD_BUTTON_A, bResetTimer: true);
		}
	}

	protected void UpdateMainMenu()
	{
	}

	protected void UpdateModalDialog()
	{
		if (3f <= (float)mStateCount / 100f)
		{
			PressButton(GamepadButton.GAMEPAD_BUTTON_A, bResetTimer: true);
		}
	}

	protected void UpdateYesNoDialog()
	{
		if (3f <= (float)mStateCount / 100f)
		{
			if (SexyFramework.Common.Rand() % 2 == 0)
			{
				PressButton(GamepadButton.GAMEPAD_BUTTON_RIGHT, bResetTimer: true);
			}
			else
			{
				PressButton(GamepadButton.GAMEPAD_BUTTON_LEFT, bResetTimer: true);
			}
			PressButton(GamepadButton.GAMEPAD_BUTTON_A, bResetTimer: true);
		}
	}

	protected void UpdatePlaying()
	{
		if (mApp.mBoard == null)
		{
			return;
		}
		if (mApp.mMapScreen != null)
		{
			PressButton(GamepadButton.GAMEPAD_BUTTON_A, bResetTimer: true);
		}
		bool flag = !mApp.mBoard.mDoingFirstTimeIntro && !mApp.mBoard.mDoingIronFrogWin && mApp.mBoard.mZumaTips.Count() == 0 && mApp.mBoard.mLevelTransition == null && !mApp.mBoard.mShowMapScreen;
		if (mAutoMonkeyDelay <= (float)mLastButtonPress / 100f)
		{
			if (mApp.mBoard.mDoingFirstTimeIntro || mApp.mBoard.mDoingIronFrogWin || mApp.mBoard.mLevelTransition != null || mApp.mBoard.mShowMapScreen)
			{
				mApp.mBoard.MouseDown(GameApp.gApp.GetScreenRect().mWidth / 2, GameApp.gApp.GetScreenRect().mHeight / 2, 1);
				mApp.mBoard.MouseUp(GameApp.gApp.GetScreenRect().mWidth / 2, GameApp.gApp.GetScreenRect().mHeight / 2, 1);
				PressButton(GamepadButton.GAMEPAD_BUTTON_A, bResetTimer: true);
			}
			else if (mApp.mBoard.mZumaTips.Count != 0)
			{
				if (mApp.mBoard.mZumaTips[0].mId == ZumaProfile.FIRST_SHOT_HINT)
				{
					mApp.mBoard.mFrog.SetDestAngle(4.64f);
					mApp.mBoard.MouseUp((int)Common._S(mApp.mBoard.mFrog.mCurX - 150f), (int)Common._S(mApp.mBoard.mFrog.mCurY), 1);
				}
				else if (mApp.mBoard.mZumaTips[0].mId == ZumaProfile.ZUMA_BAR_HINT || mApp.mBoard.mZumaTips[0].mId == ZumaProfile.SKULL_PIT_HINT)
				{
					mApp.mBoard.MouseDown(GameApp.gApp.GetScreenRect().mWidth / 2, GameApp.gApp.GetScreenRect().mHeight / 2, 1);
					mApp.mBoard.MouseUp(GameApp.gApp.GetScreenRect().mWidth / 2, GameApp.gApp.GetScreenRect().mHeight / 2, 1);
					PressButton(GamepadButton.GAMEPAD_BUTTON_A, bResetTimer: true);
				}
				else if (mApp.mBoard.mZumaTips[0].mId == ZumaProfile.LILLY_PAD_HINT)
				{
					if (mApp.mBoard.mLevel != null)
					{
						int gunPointFromPos = mApp.mBoard.mLevel.GetGunPointFromPos((int)mApp.mBoard.mFrog.mCurX, (int)mApp.mBoard.mFrog.mCurY);
						int num;
						do
						{
							num = SexyFramework.Common.Rand() % mApp.mBoard.mLevel.mNumFrogPoints;
						}
						while (num == gunPointFromPos);
						if (num >= 0 && num != mApp.mBoard.mLevel.mCurFrogPoint)
						{
							mApp.mBoard.mLevel.mCurFrogPoint = num;
							mApp.mBoard.mFrog.SetDestPos(mApp.mBoard.mLevel.mFrogX[num], mApp.mBoard.mLevel.mFrogY[num], mApp.mBoard.mLevel.mMoveSpeed, doingHop: true);
							mApp.mBoard.mLevel.ChangedPad(num);
							mApp.mUserProfile.MarkHintAsSeen(ZumaProfile.LILLY_PAD_HINT);
							mApp.mBoard.mZumaTips.RemoveAt(0);
							if (mApp.mBoard.mZumaTips.Count() == 0)
							{
								mApp.mBoard.mPreventBallAdvancement = false;
							}
						}
					}
				}
				else if (mApp.mBoard.mZumaTips[0].mId == ZumaProfile.FRUIT_HINT)
				{
					mApp.mBoard.mFrog.SetDestAngle(4.2f);
					PressButton(GamepadButton.GAMEPAD_BUTTON_A, bResetTimer: true);
					mApp.mBoard.MouseUp((int)Common._S(mApp.mBoard.mFrog.mCurX - 150f), (int)Common._S(mApp.mBoard.mFrog.mCurY - 100f), 1);
				}
				else if (mApp.mBoard.mZumaTips[0].mId == ZumaProfile.SWAP_BALL_HINT)
				{
					mApp.mBoard.MouseDown((int)Common._S(mApp.mBoard.mFrog.mCurX), (int)Common._S(mApp.mBoard.mFrog.mCurY), 1);
					mApp.mBoard.MouseUp((int)Common._S(mApp.mBoard.mFrog.mCurX), (int)Common._S(mApp.mBoard.mFrog.mCurY), 1);
				}
			}
			else if (mApp.mCredits != null)
			{
				if (mApp.mCredits.mInitialDelay >= Common._M(300))
				{
					mApp.ReturnFromCredits();
				}
			}
			else if (mApp.mBoard.mLevel != null && mApp.mBoard.mLevel.mFinalLevel && mApp.mBoard.mAdventureWinScreen && mApp.mBoard.mAdventureWinAlpha > 0f)
			{
				if (mApp.mBoard.mAdvWinBtn != null)
				{
					mApp.mBoard.ButtonDepress(mApp.mBoard.mAdvWinBtn.mId);
				}
			}
			else
			{
				bool flag2 = true;
				for (int i = 0; i < mApp.mBoard.mLevel.mNumCurves; i++)
				{
					if (mApp.mBoard.mLevel.mCurveMgr[i].mBulletList.Count() != 0)
					{
						flag2 = false;
						break;
					}
				}
				if (flag2 && mApp.mBoard.mBulletList.Count() == 0)
				{
					mApp.mBoard.mFrog.UpdateAutoMonkeyShotCorrection();
					if (mApp.mBoard.mFrog.mShotCorrectionTarget.x != 0f && mApp.mBoard.mFrog.mShotCorrectionTarget.y != 0f)
					{
						mApp.mBoard.mFrog.SetDestAngle(mApp.mBoard.mFrog.mShotCorrectionRad + 1.570795f);
						mApp.mBoard.MouseUp((int)(Common._S(mApp.mBoard.mFrog.mCurX) + mApp.mBoard.mFrog.mShotCorrectionTarget.x), (int)(Common._S(mApp.mBoard.mFrog.mCurY) + mApp.mBoard.mFrog.mShotCorrectionTarget.y), 1);
					}
					else if (mApp.mBoard.mLevel.mNumFrogPoints > 1)
					{
						PressButton(GamepadButton.GAMEPAD_BUTTON_Y, bResetTimer: true);
					}
					else
					{
						PressButton(GamepadButton.GAMEPAD_BUTTON_B, bResetTimer: true);
						mApp.mBoard.SwapFrogBalls();
					}
				}
			}
		}
		if (flag && mApp.mBoard != null && mApp.mBoard.mLevel.mMoveType == 1 && mApp.mBoard.mLevel.mBoss != null)
		{
			int num2 = mApp.mBoard.mLevel.mFrogX[0];
			int num3 = num2 + mApp.mBoard.mLevel.mBarWidth;
			int curX = mApp.mBoard.mFrog.GetCurX();
			if (curX <= num2)
			{
				mMoveDir = 2;
				mApp.mBoard.mFrog.SetDestPos(num2 + mMoveDir, mApp.mBoard.mFrog.GetCurY(), mApp.mBoard.mLevel.mMoveSpeed, doingHop: true);
			}
			else if (curX >= num3)
			{
				mMoveDir = -2;
				mApp.mBoard.mFrog.SetDestPos(num3 + mMoveDir, mApp.mBoard.mFrog.GetCurY(), mApp.mBoard.mLevel.mMoveSpeed, doingHop: true);
			}
			mApp.mBoard.mFrog.SetDestPos(curX + mMoveDir, mApp.mBoard.mFrog.GetCurY(), mApp.mBoard.mLevel.mMoveSpeed, doingHop: true);
		}
		if (flag && mApp.mBoard != null && mApp.mBoard.mCheckpointEffect != null)
		{
			mApp.mBoard.mCheckpointEffect.ButtonDepress(0);
		}
		if (!flag && mApp.mBoard != null && mApp.mBoard.mStatsContinueBtn != null)
		{
			mApp.mBoard.ButtonDepress(2);
		}
	}

	protected void PressButtonDown(GamepadButton button, bool bResetTimer)
	{
		mApp.GamepadButtonDown(button, 0, 0u);
		if (bResetTimer)
		{
			mLastButtonPress = 0;
		}
	}

	protected void PressButtonUp(GamepadButton button, bool bResetTimer)
	{
		mApp.GamepadButtonUp(button, 0, 0u);
		if (bResetTimer)
		{
			mLastButtonPress = 0;
		}
	}

	protected void PressButton(GamepadButton button, bool bResetTimer)
	{
		PressButtonDown(button, bResetTimer);
		PressButtonUp(button, bResetTimer);
	}

	public string GetStateString(MonkeyState state)
	{
		return state switch
		{
			MonkeyState.IntroScreen => "IntroScreen", 
			MonkeyState.MainMenu => "MainMenu", 
			MonkeyState.ModalOkDialog => "ModalOkDialog", 
			MonkeyState.ModalYesNoDialog => "ModalYesNoDialog", 
			MonkeyState.PauseDialog => "PauseDialog", 
			MonkeyState.Playing => "Playing", 
			MonkeyState.None => "None", 
			_ => "", 
		};
	}

	public string GetButtonString(GamepadButton button)
	{
		return button switch
		{
			GamepadButton.GAMEPAD_BUTTON_DPAD_UP => "GAMEPAD_BUTTON_DPAD_UP", 
			GamepadButton.GAMEPAD_BUTTON_DPAD_DOWN => "GAMEPAD_BUTTON_DPAD_DOWN", 
			GamepadButton.GAMEPAD_BUTTON_DPAD_LEFT => "GAMEPAD_BUTTON_DPAD_LEFT", 
			GamepadButton.GAMEPAD_BUTTON_DPAD_RIGHT => "GAMEPAD_BUTTON_DPAD_RIGHT", 
			GamepadButton.GAMEPAD_BUTTON_UP => "GAMEPAD_BUTTON_UP", 
			GamepadButton.GAMEPAD_BUTTON_DOWN => "GAMEPAD_BUTTON_DOWN", 
			GamepadButton.GAMEPAD_BUTTON_LEFT => "GAMEPAD_BUTTON_LEFT", 
			GamepadButton.GAMEPAD_BUTTON_RIGHT => "GAMEPAD_BUTTON_RIGHT", 
			GamepadButton.GAMEPAD_BUTTON_BACK => "GAMEPAD_BUTTON_BACK", 
			GamepadButton.GAMEPAD_BUTTON_START => "GAMEPAD_BUTTON_START", 
			GamepadButton.GAMEPAD_BUTTON_A => "GAMEPAD_BUTTON_A", 
			GamepadButton.GAMEPAD_BUTTON_B => "GAMEPAD_BUTTON_B", 
			GamepadButton.GAMEPAD_BUTTON_X => "GAMEPAD_BUTTON_X", 
			GamepadButton.GAMEPAD_BUTTON_Y => "GAMEPAD_BUTTON_Y", 
			GamepadButton.GAMEPAD_BUTTON_LB => "GAMEPAD_BUTTON_LB", 
			GamepadButton.GAMEPAD_BUTTON_RB => "GAMEPAD_BUTTON_RB", 
			GamepadButton.GAMEPAD_BUTTON_LTRIGGER => "GAMEPAD_BUTTON_LTRIGGER", 
			GamepadButton.GAMEPAD_BUTTON_RTRIGGER => "GAMEPAD_BUTTON_RTRIGGER", 
			GamepadButton.GAMEPAD_BUTTON_LSTICK => "GAMEPAD_BUTTON_LSTICK", 
			GamepadButton.GAMEPAD_BUTTON_RSTICK => "GAMEPAD_BUTTON_RSTICK", 
			_ => "NONE", 
		};
	}
}
