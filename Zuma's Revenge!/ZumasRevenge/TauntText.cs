namespace ZumasRevenge;

public class TauntText
{
	public string mText;

	public int mTextId = -1;

	public int mMinDeaths = -1;

	public int mDelay = 100;

	public int mCondition = -1;

	public int mMinTime;

	public int mUpdateCount;

	public void SyncState(DataSync sync)
	{
		sync.SyncLong(ref mTextId);
		sync.SyncLong(ref mMinDeaths);
		sync.SyncLong(ref mDelay);
		sync.SyncLong(ref mCondition);
		sync.SyncLong(ref mMinTime);
		sync.SyncLong(ref mUpdateCount);
		if (sync.isRead())
		{
			mText = TextManager.getInstance().getString(mTextId);
		}
	}

	public TauntText()
	{
	}

	public TauntText(TauntText rhs)
	{
		mText = rhs.mText;
		mMinDeaths = rhs.mMinDeaths;
		mDelay = rhs.mDelay;
		mCondition = rhs.mCondition;
		mMinTime = rhs.mMinTime;
		mUpdateCount = rhs.mUpdateCount;
	}
}
