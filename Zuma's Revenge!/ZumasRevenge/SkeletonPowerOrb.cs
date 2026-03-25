namespace ZumasRevenge;

public class SkeletonPowerOrb
{
	public float mSize;

	public float mAlpha = 255f;

	public void SyncState(DataSync sync)
	{
		sync.SyncFloat(ref mSize);
		sync.SyncFloat(ref mAlpha);
	}
}
