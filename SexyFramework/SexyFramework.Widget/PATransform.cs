using SexyFramework.Misc;

namespace SexyFramework.Widget;

public class PATransform
{
	public SexyTransform2D mMatrix = new SexyTransform2D(init: false);

	public PATransform Clone()
	{
		PATransform pATransform = new PATransform();
		mMatrix.CopyTo(pATransform.mMatrix);
		return pATransform;
	}

	public PATransform()
	{
		mMatrix.LoadIdentity();
	}

	public void CopyFrom(PATransform rhs)
	{
		mMatrix.CopyFrom(rhs.mMatrix);
	}

	public void TransformSrc(PATransform theSrcTransform, ref PATransform outTran)
	{
		outTran.mMatrix.CopyFrom(mMatrix * theSrcTransform.mMatrix);
	}

	public void InterpolateTo(PATransform theNextTransform, float thePct, ref PATransform outTran)
	{
		outTran.mMatrix.mMatrix = mMatrix.mMatrix * (1f - thePct) + theNextTransform.mMatrix.mMatrix * thePct;
	}
}
