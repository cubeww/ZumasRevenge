using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class FruitExplode
{
	protected PopAnim mAnim;

	protected Board mBoard;

	protected Transform mGlobalTranform = new Transform();

	public bool mDone;

	public FruitExplode(Board board)
	{
		mBoard = board;
		mAnim = null;
		Reset();
	}

	public virtual void Dispose()
	{
	}

	public void Reset()
	{
		mDone = false;
		if (mBoard.mLevel != null && mBoard.mCurTreasure != null)
		{
			switch (mBoard.mLevel.mZone)
			{
			case 1:
				mAnim = Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_PINEAPPLEMUSH);
				break;
			case 2:
				mAnim = Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_BANANAMUSH);
				break;
			case 3:
				mAnim = Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_COCOAMUSH);
				break;
			case 4:
				mAnim = Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_MANGOMUSH);
				break;
			case 5:
				mAnim = Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_COCONUTMUSH);
				break;
			case 6:
			case 7:
				mAnim = Res.GetPopAnimByID(ResID.POPANIM_NONRESIZE_ACORNMUSH);
				break;
			default:
				mAnim = null;
				break;
			}
			mAnim.Play("Main");
			int num = (int)Common._S((float)mBoard.mCurTreasure.x + ModVal.M(-130f));
			int num2 = (int)Common._S((float)mBoard.mCurTreasure.y + ModVal.M(-120f));
			mGlobalTranform.Reset();
			mGlobalTranform.Translate(num, num2);
			mAnim.SetTransform(mGlobalTranform.GetMatrix());
		}
	}

	public void Update()
	{
		if (!mDone && mAnim != null && mBoard.mCurTreasure != null)
		{
			int num = (int)Common._S((float)mBoard.mCurTreasure.x + ModVal.M(-130f));
			int num2 = (int)Common._S((float)mBoard.mCurTreasure.y + ModVal.M(-120f));
			mGlobalTranform.Reset();
			mGlobalTranform.Translate(num, num2);
			mAnim.SetTransform(mGlobalTranform.GetMatrix());
			mAnim.Update();
			if (!mAnim.IsActive() || mAnim.mMainSpriteInst.mFrameNum >= (float)(mAnim.mMainSpriteInst.mDef.mFrames.Count - 1))
			{
				mDone = true;
			}
		}
	}

	public void Draw(Graphics g)
	{
		if (mAnim != null)
		{
			mAnim.Draw(g);
		}
	}
}
