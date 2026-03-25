using System.Collections.Generic;

namespace SexyFramework.Misc;

public class SexyMathHermite
{
	public class SPoint
	{
		public float mX;

		public float mFx;

		public float mFxPrime;

		public SPoint()
		{
		}

		public SPoint(float inX, float inFx, float inFxPrime)
		{
			mX = inX;
			mFx = inFx;
			mFxPrime = inFxPrime;
		}
	}

	protected class SPiece
	{
		public float[] mCoeffs = new float[4];
	}

	public List<SPoint> mPoints = new List<SPoint>();

	protected List<SPiece> mPieces = new List<SPiece>();

	protected bool mIsBuilt;

	public SexyMathHermite()
	{
		mIsBuilt = false;
	}

	public void Rebuild()
	{
		mIsBuilt = false;
	}

	public float Evaluate(float inX)
	{
		if (!mIsBuilt)
		{
			if (!BuildCurve())
			{
				return 0f;
			}
			mIsBuilt = true;
		}
		int count = mPieces.Count;
		for (int i = 0; i < count; i++)
		{
			if (inX < mPoints[i + 1].mX)
			{
				return EvaluatePiece(inX, new SPoint[2]
				{
					mPoints[i],
					mPoints[i + 1]
				}, mPieces[i]);
			}
		}
		return mPoints[mPoints.Count - 1].mFx;
	}

	protected void CreatePiece(SPoint[] inPoints, ref SPiece outPiece)
	{
		float[,] array = new float[4u, 4u];
		float[] array2 = new float[4];
		for (uint num = 0u; num <= 1; num++)
		{
			uint num2 = 2 * num;
			array2[num2] = inPoints[num].mX;
			array2[num2 + 1] = inPoints[num].mX;
			array[num2, 0u] = inPoints[num].mFx;
			array[num2 + 1, 0u] = inPoints[num].mFx;
			array[num2 + 1, 1u] = inPoints[num].mFxPrime;
			if (num != 0)
			{
				array[num2, 1u] = (array[num2, 0u] - array[num2 - 1, 0u]) / (array2[num2] - array2[num2 - 1]);
			}
		}
		for (uint num3 = 2u; num3 < 4; num3++)
		{
			for (uint num4 = 2u; num4 <= num3; num4++)
			{
				array[num3, num4] = (array[num3, num4 - 1] - array[num3 - 1, num4 - 1]) / (array2[num3] - array2[num3 - num4]);
			}
		}
		for (uint num5 = 0u; num5 < 4; num5++)
		{
			outPiece.mCoeffs[num5] = array[num5, num5];
		}
	}

	protected float EvaluatePiece(float inX, SPoint[] inPoints, SPiece inPiece)
	{
		float[] array = new float[2]
		{
			inX - inPoints[0].mX,
			inX - inPoints[1].mX
		};
		float num = 1f;
		float num2 = inPiece.mCoeffs[0];
		for (uint num3 = 1u; num3 < 4; num3++)
		{
			num *= array[(num3 - 1) / 2];
			num2 += num * inPiece.mCoeffs[num3];
		}
		return num2;
	}

	protected bool BuildCurve()
	{
		mPieces.Clear();
		uint count = (uint)mPoints.Count;
		if (count < 2)
		{
			return false;
		}
		uint num = count - 1;
		for (int i = 0; i < num; i++)
		{
			SPiece outPiece = new SPiece();
			CreatePiece(new SPoint[2]
			{
				mPoints[i],
				mPoints[i + 1]
			}, ref outPiece);
			mPieces.Add(outPiece);
		}
		return true;
	}
}
