using System;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class RollerScore
{
	public bool mAtTarget;

	private RollerDigit[] mDigits = new RollerDigit[7];

	private RollerDigit[] mTarget = new RollerDigit[7];

	private int mTargetNum;

	private int mCurrNum;

	private bool mGauntletMode;

	private Image mNumbersImg;

	private Image mRollerImg;

	private Point mRollerPos = new Point();

	private Point mNumberPos = new Point();

	private int GetCel(int num)
	{
		if (num < 0)
		{
			return 0;
		}
		if (num % 10 > 0)
		{
			return num;
		}
		return 10;
	}

	private int GetSpeed()
	{
		return 0;
	}

	private void CalculateOffsets()
	{
		int celWidth = mRollerImg.GetCelWidth();
		int celHeight = mRollerImg.GetCelHeight();
		int celWidth2 = mNumbersImg.GetCelWidth();
		int celHeight2 = mNumbersImg.GetCelHeight();
		int mX = (int)((float)(celWidth - celWidth2) * 0.5f);
		int mY = (int)((float)(celHeight - celHeight2) * 0.5f);
		if (mGauntletMode)
		{
			mRollerPos.mX = Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GUI_INGAME_CHALLENGE_UI_SLOTS)) - Common._S(4);
			mRollerPos.mY = Common._DS(Res.GetOffsetYByID(ResID.IMAGE_GUI_INGAME_CHALLENGE_UI_SLOTS));
		}
		else
		{
			mRollerPos.mX = (int)((float)Common._S(55) + (float)Res.GetImageByID(ResID.IMAGE_GUI_INGAME_UI_WOOD).mWidth * 0.05f);
			mRollerPos.mY = Common._S(10);
		}
		mRollerPos.mX = GameApp.gApp.GetWideScreenAdjusted(mRollerPos.mX);
		mNumberPos.mX = mX;
		mNumberPos.mY = mY;
	}

	public RollerScore(bool gauntlet_mode)
	{
		Reset(gauntlet_mode);
	}

	public virtual void Dispose()
	{
	}

	public void Reset(bool gauntlet_mode)
	{
		mGauntletMode = gauntlet_mode;
		mRollerImg = Res.GetImageByID(ResID.IMAGE_GUI_INGAME_CHALLENGE_UI_SLOTS);
		mNumbersImg = Res.GetImageByID(ResID.IMAGE_GAUNTLET_ROLLER_NUMBERS);
		CalculateOffsets();
		int num = (mGauntletMode ? (-mRollerImg.GetCelWidth()) : 0);
		for (int i = 0; i < mDigits.Length; i++)
		{
			mDigits[i] = new RollerDigit();
			mTarget[i] = new RollerDigit();
		}
		for (int num2 = 6; num2 >= 0; num2--)
		{
			mDigits[num2].mX = mRollerPos.mX + mRollerImg.GetCelWidth() * (6 - num2) + mNumberPos.mX + num;
			mDigits[num2].mY = 0f;
			mDigits[num2].mVY = 0f;
			mDigits[num2].mNum = -1;
			mDigits[num2].mDelay = 0;
			mDigits[num2].mBounceState = 0;
		}
		mDigits[0].mNum = (mTarget[0].mNum = 0);
		mTargetNum = (mCurrNum = 0);
		mAtTarget = true;
	}

	public void SetTargetScore(int num)
	{
		if (num > 9999999)
		{
			num = 9999999;
		}
		if (num == mCurrNum)
		{
			return;
		}
		int num2 = num - mCurrNum;
		int num3 = 1;
		while (num2 > 0)
		{
			num2 /= 10;
			if (num2 > 0)
			{
				num3++;
			}
		}
		mTargetNum = num;
		int num4 = 0;
		while (true)
		{
			int num5 = (int)Math.Pow(10.0, num4);
			int num6 = (int)Math.Pow(10.0, num4 + 1);
			int num7 = num % num6 / num5;
			mTarget[num4].mNum = num7;
			if (mDigits[num4].mNum != num7)
			{
				mDigits[num4].mDelay = MathUtils.SafeRand() % 25;
				if (mGauntletMode)
				{
					mDigits[num4].mVY = num3 + MathUtils.SafeRand() % 2;
					float num8 = 6f;
					if (mDigits[num4].mVY > num8)
					{
						mDigits[num4].mVY = num8;
					}
				}
				else
				{
					mDigits[num4].mVY = 1 + MathUtils.SafeRand() % 2;
				}
				mDigits[num4].mBounceState = 0;
			}
			if (num / num6 == 0)
			{
				break;
			}
			num4++;
		}
		for (int i = num4 + 1; i < 7; i++)
		{
			mTarget[i].mNum = -1;
		}
		mAtTarget = false;
	}

	public void Update()
	{
		if (mAtTarget)
		{
			return;
		}
		int num = mRollerImg.GetCelHeight() + mNumberPos.mY;
		Board board = GameApp.gApp.GetBoard();
		if (board.GauntletMode() && board.mEndGauntletTimer <= 0 && board.mGauntletModeOver && board.mGauntletMultBarAlpha <= 0f)
		{
			GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_NEW_CHALLENGE_TALLY_BONUS), Common._M(5));
		}
		bool flag = true;
		for (int num2 = 6; num2 >= 0; num2--)
		{
			RollerDigit rollerDigit = mDigits[num2];
			RollerDigit rollerDigit2 = mTarget[num2];
			if (--rollerDigit.mDelay > 0)
			{
				flag = false;
			}
			else
			{
				rollerDigit.mDelay = 0;
				if (rollerDigit.mVY == 0f)
				{
					if (rollerDigit.mNum != rollerDigit2.mNum)
					{
						flag = false;
					}
				}
				else
				{
					rollerDigit.mY += rollerDigit.mVY;
					if (rollerDigit.mY >= (float)num && rollerDigit.mBounceState == 0)
					{
						rollerDigit.mNum = ((rollerDigit.mNum == -1) ? 1 : ((rollerDigit.mNum + 1) % 10));
						flag = false;
						if (rollerDigit.mNum == rollerDigit2.mNum)
						{
							rollerDigit.mY = mNumberPos.mY;
							rollerDigit.mBounceState = 1;
						}
						else
						{
							rollerDigit.mY = (float)num - rollerDigit.mY;
						}
					}
					else if (rollerDigit.mBounceState == 1 && rollerDigit.mY >= (float)Common._S(4))
					{
						flag = false;
						rollerDigit.mBounceState++;
						rollerDigit.mVY *= -1f;
					}
					else if (rollerDigit.mBounceState == 2 && rollerDigit.mY <= (float)Common._S(-3))
					{
						flag = false;
						rollerDigit.mBounceState++;
						rollerDigit.mVY *= -1f;
						rollerDigit.mRestingY = mNumberPos.mY;
					}
					else if (rollerDigit.mBounceState == 3 && rollerDigit.mY >= (float)rollerDigit.mRestingY)
					{
						rollerDigit.mY = mNumberPos.mY;
						rollerDigit.mVY = 0f;
						rollerDigit.mBounceState = 0;
					}
					else
					{
						flag = false;
					}
				}
			}
		}
		mAtTarget = flag;
	}

	public void Draw(Graphics g)
	{
		g.PushState();
		int num = mRollerImg.GetCelHeight() + mNumberPos.mY;
		g.ClipRect(mRollerPos.mX, mRollerPos.mY, mRollerImg.GetWidth(), mRollerImg.GetHeight());
		g.DrawImage(mRollerImg, mRollerPos.mX, mRollerPos.mY);
		for (int i = 0; i < 7; i++)
		{
			RollerDigit rollerDigit = mDigits[i];
			g.DrawImageCel(mNumbersImg, (int)rollerDigit.mX, mRollerPos.mY + (int)rollerDigit.mY, GetCel(rollerDigit.mNum));
			if (rollerDigit.mY != (float)mNumberPos.mY)
			{
				g.DrawImageCel(mNumbersImg, (int)rollerDigit.mX, mRollerPos.mY + (int)rollerDigit.mY - num, (rollerDigit.mNum == -1) ? GetCel(1) : GetCel(rollerDigit.mNum + 1));
			}
		}
		g.PopState();
	}

	public void ForceScore(int score)
	{
		if (score > 9999999)
		{
			score = 9999999;
		}
		for (int i = 0; i < 7; i++)
		{
			mDigits[i].mNum = -1;
			mDigits[i].mVY = (mDigits[i].mY = 0f);
			mDigits[i].mDelay = (mDigits[i].mBounceState = 0);
		}
		for (int j = 0; j < 7; j++)
		{
			int num = (int)Math.Pow(10.0, j);
			int num2 = (int)Math.Pow(10.0, j + 1);
			mDigits[j].mNum = score % num2 / num;
			if (score / num2 == 0)
			{
				break;
			}
		}
		for (int k = 0; k < 7; k++)
		{
			mTarget[k] = mDigits[k];
		}
		mTargetNum = (mCurrNum = score);
		mAtTarget = true;
	}

	public void SyncState(DataSync sync)
	{
		sync.SyncLong(ref mTargetNum);
		sync.SyncLong(ref mCurrNum);
		sync.SyncBoolean(ref mAtTarget);
		sync.SyncBoolean(ref mGauntletMode);
		for (int i = 0; i < 7; i++)
		{
			mDigits[i].SyncState(sync);
			mTarget[i].SyncState(sync);
		}
		if (sync.isRead())
		{
			int score = mTargetNum;
			Reset(mGauntletMode);
			ForceScore(score);
		}
	}

	public int GetTargetScore()
	{
		return mTargetNum;
	}

	public int GetCurrentScore()
	{
		if (mCurrNum == mTargetNum)
		{
			return mCurrNum;
		}
		mCurrNum = 0;
		for (int i = 0; i < 7; i++)
		{
			RollerDigit rollerDigit = mDigits[i];
			if (rollerDigit.mNum == -1)
			{
				break;
			}
			mCurrNum += (int)(Math.Pow(10.0, i) * (double)mDigits[i].mNum);
		}
		return mCurrNum;
	}
}
