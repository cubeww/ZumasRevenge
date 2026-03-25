namespace ZumasRevenge;

internal struct Song(int inID, bool inLoop, float inFadeSpeed)
{
	public int mID = inID;

	public bool mLoop = inLoop;

	public float mFadeSpeed = inFadeSpeed;

	public static Song DefaultSong = new Song(-1, inLoop: false, 1f);
}
