namespace SexyFramework.Graphics;

public class RenderSurface
{
	public int mData;

	public object mPtr;

	private uint mRefCount;

	public RenderSurface()
	{
		mRefCount = 0u;
		mData = 0;
		mPtr = null;
	}

	public virtual void Dispose()
	{
	}

	public void AddRef()
	{
		mRefCount++;
	}

	public void Release()
	{
		mRefCount--;
		_ = mRefCount;
		_ = 0;
	}
}
