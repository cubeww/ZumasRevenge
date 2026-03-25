using System.Collections.Generic;
using JeffLib;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public class EffectItem
{
	public Image mImage;

	public List<Component> mScale = new List<Component>();

	public List<Component> mOpacity = new List<Component>();

	public List<Component> mAngle = new List<Component>();

	public List<Component> mXOffset = new List<Component>();

	public List<Component> mYOffset = new List<Component>();

	public int mCel;

	public Color mColor = default(Color);

	public void SyncState(DataSync sync)
	{
		sync.SyncLong(ref mCel);
		sync.SyncLong(ref mColor.mRed);
		sync.SyncLong(ref mColor.mGreen);
		sync.SyncLong(ref mColor.mBlue);
		sync.SyncLong(ref mColor.mAlpha);
		SyncListComponents(sync, mScale, clear: true);
		SyncListComponents(sync, mOpacity, clear: true);
		SyncListComponents(sync, mAngle, clear: true);
		SyncListComponents(sync, mXOffset, clear: true);
		SyncListComponents(sync, mYOffset, clear: true);
	}

	private void SyncListComponents(DataSync sync, List<Component> theList, bool clear)
	{
		if (sync.isRead())
		{
			if (clear)
			{
				theList.Clear();
			}
			long num = sync.GetBuffer().ReadLong();
			for (int i = 0; i < num; i++)
			{
				Component component = new Component();
				component.SyncState(sync);
				theList.Add(component);
			}
			return;
		}
		sync.GetBuffer().WriteLong(theList.Count);
		foreach (Component the in theList)
		{
			the.SyncState(sync);
		}
	}
}
