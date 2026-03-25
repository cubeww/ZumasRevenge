using SexyFramework.Graphics;

namespace ZumasRevenge;

public class BallDrawer
{
	public int mMaxBallPriority;

	public int[] mNumBalls = new int[5];

	public int[] mNumShadows = new int[5];

	public int[] mNumOverlays = new int[5];

	public int[] mNumUnderlays = new int[5];

	private Ball[,] mBalls = new Ball[5, 1024];

	private Ball[,] mShadows = new Ball[5, 1024];

	private Ball[,] mOverlays = new Ball[5, 1024];

	private Ball[,] mUnderlays = new Ball[5, 1024];

	public void Reset()
	{
		mMaxBallPriority = 0;
		for (int i = 0; i < 5; i++)
		{
			mNumBalls[i] = 0;
			mNumShadows[i] = 0;
			mNumOverlays[i] = 0;
			mNumUnderlays[i] = 0;
		}
	}

	public void AddBall(Ball theBall, int thePriority)
	{
		int num = mNumBalls[thePriority]++;
		mBalls[thePriority, num] = theBall;
	}

	public void AddShadow(Ball theBall, int thePriority)
	{
		int num = mNumShadows[thePriority]++;
		mShadows[thePriority, num] = theBall;
	}

	public void AddOverlay(Ball theBall, int thePriority)
	{
		int num = mNumOverlays[thePriority]++;
		mOverlays[thePriority, num] = theBall;
	}

	public void AddUnderlay(Ball theBall, int thePriority)
	{
		int num = mNumUnderlays[thePriority]++;
		mUnderlays[thePriority, num] = theBall;
	}

	public void Draw(Graphics g, Board theBoard)
	{
		g.Get3D();
		for (int i = 0; i < 5; i++)
		{
			if (i != 0)
			{
				theBoard.DrawTunnels(g, i, below_shadow: true);
			}
			theBoard.mLevel.DrawPriority(g, i);
			for (int j = 0; j < theBoard.mLevel.mNumCurves; j++)
			{
				theBoard.mLevel.mCurveMgr[j].DrawMisc(g, i);
				theBoard.mLevel.mCurveMgr[j].DrawSkullPathShit(g, i);
			}
			if (Board.gHideBalls)
			{
				continue;
			}
			int num = mNumShadows[i];
			for (int j = 0; j < num; j++)
			{
				mShadows[i, j].DrawShadow(g);
			}
			theBoard.DrawTunnels(g, i, below_shadow: false);
			num = mNumUnderlays[i];
			for (int j = 0; j < num; j++)
			{
				mUnderlays[i, j].DrawBottomLayer(g);
			}
			num = mNumBalls[i];
			for (int j = 0; j < num; j++)
			{
				mBalls[i, j].DrawBase(g, 0, 0);
			}
			for (int j = 0; j < num; j++)
			{
				mBalls[i, j].DrawAdditive(g, 0, 0);
			}
			if (g.Is3D())
			{
				num = mNumOverlays[i];
				for (int j = 0; j < num; j++)
				{
					mOverlays[i, j].DrawTopLayer(g);
				}
			}
		}
		for (int j = 0; j < theBoard.mLevel.mNumCurves; j++)
		{
			theBoard.mLevel.mCurveMgr[j].DrawAboveBalls(g);
		}
	}
}
