using System;

namespace SexyFramework.Graphics;

public class RenderCommand : IDisposable
{
	public Color mColor = default(Color);

	public ActiveFontLayer mFontLayer;

	public int[] mDest = new int[2];

	public int[] mSrc = new int[4];

	public int mMode;

	public RenderCommand()
	{
		mFontLayer = null;
	}

	public virtual void Dispose()
	{
		mFontLayer = null;
		mDest = null;
		mSrc = null;
	}
}
