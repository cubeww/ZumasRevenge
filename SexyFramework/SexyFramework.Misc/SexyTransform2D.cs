using System;
using Microsoft.Xna.Framework;

namespace SexyFramework.Misc;

public struct SexyTransform2D : SexyMatrix3
{
	public Matrix mMatrix;

	public static SexyTransform2D DefaultMatrix;

	public float m00
	{
		get
		{
			return mMatrix.M11;
		}
		set
		{
			mMatrix.M11 = value;
		}
	}

	public float m01
	{
		get
		{
			return mMatrix.M21;
		}
		set
		{
			mMatrix.M21 = value;
		}
	}

	public float m02
	{
		get
		{
			return mMatrix.M41;
		}
		set
		{
			mMatrix.M41 = value;
		}
	}

	public float m10
	{
		get
		{
			return mMatrix.M12;
		}
		set
		{
			mMatrix.M12 = value;
		}
	}

	public float m11
	{
		get
		{
			return mMatrix.M22;
		}
		set
		{
			mMatrix.M22 = value;
		}
	}

	public float m12
	{
		get
		{
			return mMatrix.M42;
		}
		set
		{
			mMatrix.M42 = value;
		}
	}

	public float m20
	{
		get
		{
			return mMatrix.M13;
		}
		set
		{
			mMatrix.M13 = value;
		}
	}

	public float m21
	{
		get
		{
			return mMatrix.M23;
		}
		set
		{
			mMatrix.M23 = value;
		}
	}

	public float m22
	{
		get
		{
			return mMatrix.M33;
		}
		set
		{
			mMatrix.M33 = value;
		}
	}

	public SexyTransform2D(bool init)
	{
		mMatrix = Matrix.Identity;
	}

	public SexyTransform2D(Matrix mat)
	{
		mMatrix = mat;
	}

	public void Swap(SexyTransform2D lhs)
	{
	}

	public void CopyTo(SexyTransform2D lhs)
	{
		lhs.mMatrix = mMatrix;
	}

	public SexyTransform2D(SexyMatrix3 rhs)
	{
		mMatrix = ((SexyTransform2D)(object)rhs).mMatrix;
	}

	public SexyTransform2D(float in00, float in01, float in02, float in10, float in11, float in12, float in20, float in21, float in22)
	{
		mMatrix = new Matrix(in00, in10, in20, 0f, in01, in11, in21, 0f, 0f, 0f, in22, 0f, in02, in12, 0f, 1f);
	}

	public void ZeroMatrix()
	{
		float num = (m22 = 0f);
		float num3 = (m21 = num);
		float num5 = (m20 = num3);
		float num7 = (m12 = num5);
		float num9 = (m11 = num7);
		float num11 = (m10 = num9);
		float num13 = (m02 = num11);
		float num15 = (m01 = num13);
		m00 = num15;
	}

	public void LoadIdentity()
	{
		mMatrix = Matrix.Identity;
	}

	public void CopyFrom(SexyMatrix3 theMatrix)
	{
		mMatrix = ((SexyTransform2D)(object)theMatrix).mMatrix;
	}

	public static SexyVector2 operator *(SexyTransform2D ImpliedObject, SexyVector2 theVec)
	{
		SexyVector2 result = new SexyVector2(init: false);
		result.mVector = Vector2.Transform(theVec.mVector, ImpliedObject.mMatrix);
		return result;
	}

	public static Vector2 operator *(SexyTransform2D ImpliedObject, Vector2 theVec)
	{
		return Vector2.Transform(theVec, ImpliedObject.mMatrix);
	}

	public static SexyVector3 operator *(SexyTransform2D ImpliedObject, SexyVector3 theVec)
	{
		return new SexyVector3
		{
			mVector = Vector3.Transform(theVec.mVector, ImpliedObject.mMatrix)
		};
	}

	public void MulSelf(SexyMatrix3 theMat)
	{
		mMatrix *= ((SexyTransform2D)(object)theMat).mMatrix;
	}

	public static SexyMatrix3 operator *(SexyTransform2D ImpliedObject, SexyMatrix3 theMat)
	{
		SexyTransform2D sexyTransform2D = new SexyTransform2D(init: false);
		sexyTransform2D.mMatrix = ((SexyTransform2D)(object)theMat).mMatrix * ImpliedObject.mMatrix;
		return sexyTransform2D;
	}

	public static void Multiply(ref SexyTransform2D pOut, SexyMatrix3 pM1, SexyMatrix3 pM2)
	{
		pOut.mMatrix = ((SexyTransform2D)(object)pM2).mMatrix * ((SexyTransform2D)(object)pM1).mMatrix;
	}

	public void Translate(float tx, float ty)
	{
		mMatrix.M41 += tx;
		mMatrix.M42 += ty;
	}

	public void RotateRad(float rot)
	{
		mMatrix = Matrix.Multiply(mMatrix, Matrix.CreateRotationZ(0f - rot));
	}

	public void RotateDeg(float rot)
	{
		RotateRad(MathHelper.ToRadians(rot));
	}

	public void Scale(float sx, float sy)
	{
		mMatrix.M11 *= sx;
		mMatrix.M21 *= sx;
		mMatrix.M41 *= sx;
		mMatrix.M22 *= sy;
		mMatrix.M12 *= sy;
		mMatrix.M42 *= sy;
	}

	public void SkewRad(float sx, float sy)
	{
		SexyTransform2D sexyTransform2D = new SexyTransform2D(init: false);
		sexyTransform2D.LoadIdentity();
		sexyTransform2D.m01 = (float)Math.Tan(sx);
		sexyTransform2D.m02 = (float)Math.Tan(sy);
		(sexyTransform2D * this).Swap(this);
	}

	public static SexyTransform2D operator *(SexyTransform2D ImpliedObject, SexyTransform2D theMat)
	{
		return new SexyTransform2D(theMat.mMatrix * ImpliedObject.mMatrix);
	}

	static SexyTransform2D()
	{
		DefaultMatrix = new SexyTransform2D(init: false);
	}
}
