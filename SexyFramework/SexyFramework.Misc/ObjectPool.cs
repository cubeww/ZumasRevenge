using System;
using System.Collections.Generic;

namespace SexyFramework.Misc;

public class ObjectPool<T> where T : IDisposable, new()
{
	public int mPoolSize;

	public int mNumWant;

	public int mNumAvailObjects;

	public List<T> mFreePools;

	public int mNextAvailIndex;

	public ObjectPool(int size)
	{
		mNumWant = 0;
		mNumAvailObjects = 0;
		mNextAvailIndex = 0;
		mPoolSize = size;
		mFreePools = new List<T>();
		mNumAvailObjects += mPoolSize;
	}

	public virtual void Dispose()
	{
		for (int i = 0; i < mFreePools.Count; i++)
		{
			if (mFreePools[i] != null)
			{
				mFreePools[i].Dispose();
			}
		}
		mFreePools.Clear();
	}

	public T Alloc()
	{
		if (mFreePools.Count > 0)
		{
			T result = mFreePools[mFreePools.Count - 1];
			mFreePools.RemoveAt(mFreePools.Count - 1);
			return result;
		}
		mNumWant++;
		return new T();
	}

	public void Free(T thePtr)
	{
		mFreePools.Add(thePtr);
	}
}
