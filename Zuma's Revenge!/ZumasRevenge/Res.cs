using System;
using System.Collections.Generic;
using System.Reflection;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Resource;
using SexyFramework.Widget;

namespace ZumasRevenge;

public static class Res
{
	private static ResGlobalPtr[] mGlobalRes = new ResGlobalPtr[1850];

	private static Point[] mGlobalResOffset = new Point[1850];

	private static GameApp mApp = null;

	private static ResourceManager mResMgr = null;

	public static void InitResources(GameApp app)
	{
		mApp = app;
		mResMgr = mApp.mResourceManager;
	}

	public static Image GetImageByID(ResID id)
	{
		if (mGlobalRes[(int)id] != null && mGlobalRes[(int)id].mResObject != null)
		{
			return mGlobalRes[(int)id].mResObject as Image;
		}
		string text = id.ToString();
		mGlobalRes[(int)id] = mResMgr.RegisterGlobalPtr(text);
		if (mGlobalRes[(int)id] == null)
		{
			List<string> allEnum = GetAllEnum(id);
			for (int i = 0; i < allEnum.Count; i++)
			{
				string theId = allEnum[i];
				mGlobalRes[(int)id] = mResMgr.RegisterGlobalPtr(theId);
				if (mGlobalRes[(int)id] != null)
				{
					break;
				}
			}
		}
		if (mGlobalRes[(int)id] != null)
		{
			mResMgr.LoadImage(text);
		}
		return mGlobalRes[(int)id].mResObject as Image;
	}

	public static List<string> GetAllEnum(ResID id)
	{
		List<string> list = new List<string>();
		FieldInfo[] fields = typeof(ResID).GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.IsLiteral && (object)typeof(ResID).GetType() == typeof(ResID).GetType() && (int)fieldInfo.GetRawConstantValue() == (int)typeof(ResID).GetField(id.ToString()).GetRawConstantValue())
			{
				list.Add(fieldInfo.Name);
			}
		}
		return list;
	}

	public static int GetIDByImage(Image img)
	{
		for (int i = 0; i < mGlobalRes.Length; i++)
		{
			if (mGlobalRes[i] != null && mGlobalRes[i].mResObject == img)
			{
				return i;
			}
		}
		return -1;
	}

	public static Font GetFontByID(ResID id)
	{
		if (mGlobalRes[(int)id] != null && mGlobalRes[(int)id].mResObject != null)
		{
			return mGlobalRes[(int)id].mResObject as Font;
		}
		string text = id.ToString();
		mGlobalRes[(int)id] = mResMgr.RegisterGlobalPtr(text);
		if (mGlobalRes[(int)id] != null)
		{
			mResMgr.LoadFont(text);
		}
		return mGlobalRes[(int)id].mResObject as Font;
	}

	public static int GetSoundByID(ResID id)
	{
		if (mGlobalRes[(int)id] != null && mGlobalRes[(int)id].mResObject != null)
		{
			return (int)mGlobalRes[(int)id].mResObject;
		}
		string text = id.ToString();
		mGlobalRes[(int)id] = mResMgr.RegisterGlobalPtr(text);
		if (mGlobalRes[(int)id] != null)
		{
			mResMgr.LoadSound(text);
		}
		return (int)mGlobalRes[(int)id].mResObject;
	}

	public static PIEffect GetPIEffectByID(ResID id)
	{
		if (mGlobalRes[(int)id] != null && mGlobalRes[(int)id].mResObject != null)
		{
			return mGlobalRes[(int)id].mResObject as PIEffect;
		}
		string text = id.ToString();
		mGlobalRes[(int)id] = mResMgr.RegisterGlobalPtr(text);
		if (mGlobalRes[(int)id] != null)
		{
			mResMgr.LoadPIEffect(text);
		}
		return mGlobalRes[(int)id].mResObject as PIEffect;
	}

	public static Effect GetEffectByID(ResID id)
	{
		throw new NotImplementedException();
	}

	public static PopAnim GetPopAnimByID(ResID id)
	{
		if (mGlobalRes[(int)id] != null)
		{
			return mGlobalRes[(int)id].mResObject as PopAnim;
		}
		string text = id.ToString();
		mGlobalRes[(int)id] = mResMgr.RegisterGlobalPtr(text);
		if (mGlobalRes[(int)id] != null)
		{
			mResMgr.LoadPopAnim(text);
		}
		return mGlobalRes[(int)id].mResObject as PopAnim;
	}

	public static int GetOffsetXByID(ResID id)
	{
		if (mGlobalResOffset[(int)id] != null)
		{
			return mGlobalResOffset[(int)id].mX;
		}
		string theId = id.ToString();
		Point offsetOfImage = mResMgr.GetOffsetOfImage(theId);
		if (offsetOfImage != null)
		{
			mGlobalResOffset[(int)id] = new Point(offsetOfImage);
			return mGlobalResOffset[(int)id].mX;
		}
		return 0;
	}

	public static int GetOffsetYByID(ResID id)
	{
		if (mGlobalResOffset[(int)id] != null)
		{
			return mGlobalResOffset[(int)id].mY;
		}
		string theId = id.ToString();
		Point offsetOfImage = mResMgr.GetOffsetOfImage(theId);
		if (offsetOfImage != null)
		{
			mGlobalResOffset[(int)id] = new Point(offsetOfImage);
			return mGlobalResOffset[(int)id].mY;
		}
		return 0;
	}
}
