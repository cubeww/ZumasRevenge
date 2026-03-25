using System.Collections.Generic;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public class SquidAnim
{
	public Image mImage;

	public List<SquidAnimCel> mCels = new List<SquidAnimCel>();

	public int mUpdateCount;

	public int mCurCel;

	public float mX;

	public float mY;

	public SquidAnim()
	{
	}

	public SquidAnim(SquidAnim rhs)
	{
		mImage = rhs.mImage;
		mUpdateCount = rhs.mUpdateCount;
		mCurCel = rhs.mCurCel;
		mX = rhs.mX;
		mY = rhs.mY;
		for (int i = 0; i < rhs.mCels.Count; i++)
		{
			SquidAnimCel item = new SquidAnimCel
			{
				mCelNum = rhs.mCels[i].mCelNum,
				mDelay = rhs.mCels[i].mDelay
			};
			mCels.Add(item);
		}
	}

	public void AddAnimInfo(int cel_num, int delay)
	{
		SquidAnimCel squidAnimCel = new SquidAnimCel();
		mCels.Add(squidAnimCel);
		squidAnimCel.mCelNum = cel_num;
		squidAnimCel.mDelay = delay;
	}

	public void Update()
	{
		if (++mUpdateCount >= mCels[mCurCel].mDelay)
		{
			mUpdateCount = 0;
			mCurCel = (mCurCel + 1) % mCels.Count;
		}
	}

	public void Draw(Graphics g, float x, float y)
	{
		g.DrawImageCel(mImage, (int)(x + Common._S(mX)), (int)(y + Common._S(mY)), mCels[mCurCel].mCelNum);
	}
}
