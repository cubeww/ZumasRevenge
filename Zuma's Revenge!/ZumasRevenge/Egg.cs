namespace ZumasRevenge;

public class Egg
{
	public float mAngle = 1.570795f;

	public float mSize;

	public void SyncState(DataSync sync)
	{
		sync.SyncFloat(ref mAngle);
		sync.SyncFloat(ref mSize);
	}
}
