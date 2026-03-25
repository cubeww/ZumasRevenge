using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class CheatWidget : Widget
{
	public string mCheatChars;

	public Widget mClient;

	public int mRows;

	public int mCols;

	public int mButtonsPerRow;

	public int mButtonSize;

	public bool mAlignment;

	public bool mEnable;

	private static int BUTTON_SIZE = Common._DS(80);

	public CheatWidget(Widget theTarget, string theCheats, Font theFont)
	{
		float num = ((GameApp.mGameRes == 768) ? 1f : ((GameApp.mGameRes == 640) ? 2f : 1.5f));
		mButtonSize = (int)((float)BUTTON_SIZE * num);
		mClient = theTarget;
		mCheatChars = theCheats;
		int length = theCheats.Length;
		mButtonsPerRow = (GameApp.gApp.GetScreenRect().mWidth + GameApp.gApp.GetScreenRect().mX - GameApp.gApp.mBoardOffsetX * 2) / mButtonSize;
		mCols = mButtonsPerRow;
		mRows = (length + mButtonsPerRow - 1) / mButtonsPerRow;
		mWidth = mCols * mButtonSize + 1;
		mHeight = mRows * mButtonSize + 1;
		mAlignment = true;
		mEnable = true;
	}

	public override void Draw(Graphics g)
	{
		if (!mEnable)
		{
			return;
		}
		g.SetColor(new Color(255, 200));
		g.FillRect(0, 0, mWidth, mHeight);
		int i = 0;
		for (int j = 0; j < mRows; j++)
		{
			for (int k = 0; k < mCols; k++)
			{
				Rect theRect = new Rect(k * mButtonSize + 1, j * mButtonSize + 1, mButtonSize - 2, mButtonSize - 2);
				g.SetColor(20, 20, 20);
				g.FillRect(theRect);
			}
			int num = 0;
			for (; i < mCheatChars.Length; i++)
			{
				if (num >= mCols)
				{
					break;
				}
				((GameMain)GameApp.gApp.mGameMain).DrawSysString(string.Concat(mCheatChars[i]), (float)(num * mButtonSize + mButtonSize / 2 - 10) * 800f / 1066f, (float)(j * mButtonSize + mY + mButtonSize / 2 - 10) * 800f / 1066f);
				num++;
			}
		}
	}

	public override void MouseDown(int x, int y, int theClickCount)
	{
		if (!mEnable)
		{
			return;
		}
		int num = y / mButtonSize;
		int num2 = x / mButtonSize;
		int num3 = num * mButtonsPerRow + num2;
		if (num3 < mCheatChars.Length)
		{
			if (mCheatChars[num3] == 'X')
			{
				GameApp.gApp.mStepMode = 0;
				GameApp.gApp.ClearUpdateBacklog(relaxForASecond: false);
				mEnable = false;
				SetVisible(isVisible: false);
			}
			else if (mCheatChars[num3] == 'j')
			{
				SwapAlignment();
			}
			else
			{
				mClient.KeyChar(mCheatChars[num3]);
			}
		}
	}

	public override void MouseUp(int x, int y, int theClickCount)
	{
	}

	public void SwapAlignment()
	{
		if (mAlignment)
		{
			Move(mX, GameApp.gApp.GetScreenRect().mHeight - mHeight);
			mAlignment = false;
		}
		else
		{
			Move(mX, 0);
			mAlignment = true;
		}
	}
}
