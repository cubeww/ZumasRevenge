namespace ZumasRevenge;

public class HulaEntry
{
	public enum AttackType
	{
		Attack_None,
		Attack_Stun,
		Attack_Poison,
		Attack_Hallucinate,
		Attack_Slow
	}

	public int mBerserkAmt;

	public int mAmnesty;

	public float mVX;

	public float mProjVY;

	public int mSpawnY;

	public int mSpawnRate;

	public int mProjChance;

	public int mAttackType;

	public int mAttackTime;

	public int mProjRange;

	public HulaEntry()
	{
	}

	public HulaEntry(HulaEntry rhs)
	{
		mBerserkAmt = rhs.mBerserkAmt;
		mAmnesty = rhs.mAmnesty;
		mVX = rhs.mVX;
		mProjVY = rhs.mProjVY;
		mSpawnY = rhs.mSpawnY;
		mSpawnRate = rhs.mSpawnRate;
		mProjChance = rhs.mProjChance;
		mAttackType = rhs.mAttackType;
		mAttackTime = rhs.mAttackTime;
		mProjRange = rhs.mProjRange;
	}
}
