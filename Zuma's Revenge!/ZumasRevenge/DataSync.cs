using System.Collections.Generic;
using JeffLib;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class DataSync : DataSyncBase
{
	private Buffer m_buffer;

	private bool m_isRead = true;

	private int mCurPointerIndex;

	private Dictionary<CurveMgr, int> mPointerToIntMap_CurveMgr = new Dictionary<CurveMgr, int>();

	private Dictionary<int, CurveMgr> mIntToPointerMap_CurveMgr = new Dictionary<int, CurveMgr>();

	private List<ReversePowerEffect> mPointerSyncList_ReversePowerEffect = new List<ReversePowerEffect>();

	private Dictionary<Ball, int> mPointerToIntMap_Ball = new Dictionary<Ball, int>();

	private Dictionary<int, Ball> mIntToPointerMap_Ball = new Dictionary<int, Ball>();

	private List<Bullet> mPointerSyncList_Bullet = new List<Bullet>();

	private Dictionary<Bullet, int> mPointerToIntMap_Bullet = new Dictionary<Bullet, int>();

	private Dictionary<int, Bullet> mIntToPointerMap_Bullet = new Dictionary<int, Bullet>();

	private List<Ball> mPointerSyncList_Ball = new List<Ball>();

	public DataSync(Buffer buffer, bool isRead)
	{
		ResetPointerTable();
		m_buffer = buffer;
		m_isRead = isRead;
	}

	public Buffer GetBuffer()
	{
		return m_buffer;
	}

	public bool isRead()
	{
		return m_isRead;
	}

	public bool isWrite()
	{
		return !isRead();
	}

	public void SyncBoolean(ref bool theBool)
	{
		if (m_isRead)
		{
			theBool = m_buffer.ReadBoolean();
		}
		else
		{
			m_buffer.WriteBoolean(theBool);
		}
	}

	public void SyncShort(ref short theInt)
	{
		if (m_isRead)
		{
			theInt = m_buffer.ReadShort();
		}
		else
		{
			m_buffer.WriteShort(theInt);
		}
	}

	public override void SyncLong(ref int theInt)
	{
		if (m_isRead)
		{
			theInt = (int)m_buffer.ReadLong();
		}
		else
		{
			m_buffer.WriteLong(theInt);
		}
	}

	public void SyncLong(ref uint theInt)
	{
		if (m_isRead)
		{
			theInt = (uint)m_buffer.ReadLong();
		}
		else
		{
			m_buffer.WriteLong(theInt);
		}
	}

	public void SyncLong(ref ushort theInt)
	{
		if (m_isRead)
		{
			theInt = (ushort)m_buffer.ReadLong();
		}
		else
		{
			m_buffer.WriteLong(theInt);
		}
	}

	public void SyncLong(ref long theLong)
	{
		if (m_isRead)
		{
			theLong = m_buffer.ReadLong();
		}
		else
		{
			m_buffer.WriteLong(theLong);
		}
	}

	public override void SyncFloat(ref float theFloat)
	{
		if (m_isRead)
		{
			theFloat = m_buffer.ReadFloat();
		}
		else
		{
			m_buffer.WriteFloat(theFloat);
		}
	}

	public override void SyncListInt(List<int> theList)
	{
		if (m_isRead)
		{
			theList.Clear();
			long num = m_buffer.ReadLong();
			for (int i = 0; i < num; i++)
			{
				theList.Add((int)m_buffer.ReadLong());
			}
			return;
		}
		m_buffer.WriteLong(theList.Count);
		foreach (int the in theList)
		{
			m_buffer.WriteLong(the);
		}
	}

	public override void SyncListFloat(List<float> theList)
	{
		if (m_isRead)
		{
			theList.Clear();
			long num = m_buffer.ReadLong();
			for (int i = 0; i < num; i++)
			{
				theList.Add(m_buffer.ReadFloat());
			}
			return;
		}
		m_buffer.WriteLong(theList.Count);
		foreach (float the in theList)
		{
			float theFloat = the;
			m_buffer.WriteFloat(theFloat);
		}
	}

	private void ResetPointerTable()
	{
		mCurPointerIndex = 2;
		mIntToPointerMap_CurveMgr.Clear();
		mIntToPointerMap_Ball.Clear();
		mIntToPointerMap_Bullet.Clear();
		mPointerToIntMap_CurveMgr.Clear();
		mPointerToIntMap_Ball.Clear();
		mPointerToIntMap_Bullet.Clear();
		mPointerSyncList_ReversePowerEffect.Clear();
		mPointerSyncList_Ball.Clear();
		mPointerSyncList_Bullet.Clear();
		mIntToPointerMap_CurveMgr.Add(0, null);
		mIntToPointerMap_Ball.Add(0, null);
		mIntToPointerMap_Bullet.Add(0, null);
	}

	public bool RegisterPointer(CurveMgr thePtr)
	{
		if (!mPointerToIntMap_CurveMgr.ContainsKey(thePtr))
		{
			int num = mCurPointerIndex++;
			mPointerToIntMap_CurveMgr.Add(thePtr, num);
			mIntToPointerMap_CurveMgr.Add(num, thePtr);
			return true;
		}
		return false;
	}

	public bool RegisterPointer(Ball thePtr)
	{
		if (!mPointerToIntMap_Ball.ContainsKey(thePtr))
		{
			int num = mCurPointerIndex++;
			mPointerToIntMap_Ball.Add(thePtr, num);
			mIntToPointerMap_Ball.Add(num, thePtr);
			return true;
		}
		return false;
	}

	public bool RegisterPointer(Bullet thePtr)
	{
		if (!mPointerToIntMap_Bullet.ContainsKey(thePtr))
		{
			int num = mCurPointerIndex++;
			mPointerToIntMap_Bullet.Add(thePtr, num);
			mIntToPointerMap_Bullet.Add(num, thePtr);
			return true;
		}
		return false;
	}

	public void SyncPointer(ReversePowerEffect thePtr)
	{
		mPointerSyncList_ReversePowerEffect.Add(thePtr);
	}

	public void SyncPointer(Ball thePtr)
	{
		mPointerSyncList_Ball.Add(thePtr);
	}

	public void SyncPointer(Bullet thePtr)
	{
		mPointerSyncList_Bullet.Add(thePtr);
	}

	public void SyncPointers()
	{
		if (m_isRead)
		{
			foreach (ReversePowerEffect item in mPointerSyncList_ReversePowerEffect)
			{
				int key = (int)m_buffer.ReadLong();
				item.mCurve = mIntToPointerMap_CurveMgr[key];
			}
			foreach (Ball item2 in mPointerSyncList_Ball)
			{
				int key2 = (int)m_buffer.ReadLong();
				item2.mBullet = mIntToPointerMap_Bullet[key2];
			}
			foreach (Bullet item3 in mPointerSyncList_Bullet)
			{
				int key3 = (int)m_buffer.ReadLong();
				item3.mHitBall = mIntToPointerMap_Ball[key3];
			}
		}
		else
		{
			foreach (ReversePowerEffect item4 in mPointerSyncList_ReversePowerEffect)
			{
				int num = 0;
				if (item4.mCurve != null && mPointerToIntMap_CurveMgr.ContainsKey(item4.mCurve))
				{
					num = mPointerToIntMap_CurveMgr[item4.mCurve];
				}
				m_buffer.WriteLong(num);
			}
			foreach (Ball item5 in mPointerSyncList_Ball)
			{
				int num2 = 0;
				if (item5.mBullet != null && mPointerToIntMap_Bullet.ContainsKey(item5.mBullet))
				{
					num2 = mPointerToIntMap_Bullet[item5.mBullet];
				}
				m_buffer.WriteLong(num2);
			}
			foreach (Bullet item6 in mPointerSyncList_Bullet)
			{
				int num3 = 0;
				if (item6.mHitBall != null && mPointerToIntMap_Ball.ContainsKey(item6.mHitBall))
				{
					num3 = mPointerToIntMap_Ball[item6.mHitBall];
				}
				m_buffer.WriteLong(num3);
			}
		}
		ResetPointerTable();
	}
}
