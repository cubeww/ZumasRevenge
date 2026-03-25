namespace SexyFramework.Misc;

public struct AutoCrit
{
	private CritSect mCritSec;

	public AutoCrit(CritSect theCritSect)
	{
		mCritSec = theCritSect;
		mCritSec.Lock();
	}

	public void Dispose()
	{
		mCritSec.Unlock();
	}
}
