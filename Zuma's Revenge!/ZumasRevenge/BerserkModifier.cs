using System;
using System.Globalization;

namespace ZumasRevenge;

public class BerserkModifier
{
	public enum DataType
	{
		Type_Int,
		Type_Float,
		Type_Bool
	}

	public string mParamName;

	public string mStringValue;

	public string mMinStr;

	public string mMaxStr;

	public bool mOverride;

	public int mParamType;

	protected object mValue;

	protected object mVariablePtr;

	protected object mMin;

	protected object mMax;

	protected bool mHasMin;

	protected bool mHasMax;

	public BerserkModifier(BerserkModifier rhs)
	{
		mParamName = rhs.mParamName;
		mStringValue = rhs.mStringValue;
		mMinStr = rhs.mMinStr;
		mMaxStr = rhs.mMaxStr;
		mOverride = rhs.mOverride;
		mParamType = rhs.mParamType;
		mHasMin = rhs.mHasMin;
		mHasMax = rhs.mHasMax;
	}

	public BerserkModifier(string p, string value, string minval, string maxval, bool _override)
	{
		mParamName = p;
		mStringValue = value;
		mHasMin = (mHasMax = false);
		mOverride = _override;
		if (minval != null && minval.Length > 0)
		{
			mHasMin = true;
			mMinStr = minval;
		}
		if (maxval != null && maxval.Length > 0)
		{
			mHasMax = true;
			mMaxStr = maxval;
		}
	}

	public BerserkModifier(string p, string value)
	{
		mParamName = p;
		mStringValue = value;
		mHasMin = (mHasMax = false);
		mOverride = false;
	}

	public void AddPointerFloat(object fptr)
	{
		mParamType = 1;
		mVariablePtr = fptr;
		if (mStringValue[0] == '.')
		{
			mStringValue = "0" + mStringValue;
		}
		double result = 0.0;
		double.TryParse(mStringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
		mValue = result;
		if (mHasMin)
		{
			mMin = Convert.ToSingle(mMinStr);
		}
		if (mHasMax)
		{
			mMax = Convert.ToSingle(mMaxStr);
		}
	}

	public void AddPointerInt(object iptr)
	{
		mParamType = 0;
		mVariablePtr = iptr;
		try
		{
			mValue = Convert.ToInt32(mStringValue);
		}
		catch (Exception)
		{
			mValue = 0;
		}
		if (mHasMin)
		{
			mMin = Convert.ToInt32(mMinStr);
		}
		if (mHasMax)
		{
			mMax = Convert.ToInt32(mMaxStr);
		}
	}

	public void AddPointerBool(object bptr)
	{
		mParamType = 2;
		mVariablePtr = bptr;
		mValue = Convert.ToBoolean(mStringValue);
		if (mHasMin)
		{
			mMin = Convert.ToBoolean(mMinStr);
		}
		if (mHasMax)
		{
			mMax = Convert.ToBoolean(mMaxStr);
		}
	}

	public void ModifyVariable()
	{
		if (mParamType == 1)
		{
			ParamData<float> paramData = mVariablePtr as ParamData<float>;
			if (mOverride)
			{
				paramData.value = Convert.ToSingle(mValue);
				return;
			}
			paramData.value += Convert.ToSingle(mValue);
			if (mHasMin && paramData.value < Convert.ToSingle(mMin))
			{
				paramData.value = Convert.ToSingle(mMin);
			}
			else if (mHasMax && paramData.value > Convert.ToSingle(mMax))
			{
				paramData.value = Convert.ToSingle(mMax);
			}
		}
		else if (mParamType == 0)
		{
			ParamData<int> paramData2 = mVariablePtr as ParamData<int>;
			if (mOverride)
			{
				paramData2.value = Convert.ToInt32(mValue);
				return;
			}
			paramData2.value += Convert.ToInt32(mValue);
			if (mHasMin && paramData2.value < Convert.ToInt32(mMin))
			{
				paramData2.value = Convert.ToInt32(mMin);
			}
			else if (mHasMax && paramData2.value > Convert.ToInt32(mMax))
			{
				paramData2.value = Convert.ToInt32(mMax);
			}
		}
		else if (mParamType == 2)
		{
			ParamData<bool> paramData3 = mVariablePtr as ParamData<bool>;
			paramData3.value = Convert.ToBoolean(mValue);
		}
	}
}
