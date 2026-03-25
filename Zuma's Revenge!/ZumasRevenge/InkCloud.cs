namespace ZumasRevenge;

public class InkCloud
{
	public bool mFadeIn;

	public float mAlpha;

	public float mSize;

	public float mX;

	public float mY;

	public void SyncState(DataSync s)
	{
		s.SyncBoolean(ref mFadeIn);
		s.SyncFloat(ref mAlpha);
		s.SyncFloat(ref mSize);
		s.SyncFloat(ref mX);
		s.SyncFloat(ref mY);
	}
}
