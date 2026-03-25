namespace ZumasRevenge;

public class Boss_Param_Range
{
	public float mMin;

	public float mMax;

	public float mRatingMin;

	public float mRatingMax;

	public void Init()
	{
		mMin = 0f;
		mMax = 0f;
		mRatingMin = -1f;
		mRatingMax = -1f;
	}

	public bool InRange(float amt)
	{
		if (!(mRatingMin < 0f) && !(mRatingMax < 0f))
		{
			if (amt >= mRatingMin)
			{
				return amt < mRatingMax;
			}
			return false;
		}
		return true;
	}
}
