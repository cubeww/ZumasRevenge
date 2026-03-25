using System;
using System.Collections.Generic;
using SexyFramework;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public class WaterShader1 : Effect
{
	protected List<WaterShaderImage> mImages = new List<WaterShaderImage>();

	protected override void Init()
	{
		for (int i = 0; i < mImages.Count; i++)
		{
			if (mImages[i].mFileName.Length > 0)
			{
				if (mImages[i].mImage != null)
				{
					mImages[i].mImage.Dispose();
					mImages[i].mImage = null;
				}
			}
			else
			{
				GameApp.gApp.mResourceManager.DeleteResources(mImages[i].mResId);
			}
			mImages[i].mImage = null;
		}
		for (int j = 0; j < mImages.Count; j++)
		{
			WaterShaderImage waterShaderImage = mImages[j];
			if (waterShaderImage.mImage != null)
			{
				continue;
			}
			if (waterShaderImage.mFileName.Length > 0)
			{
				waterShaderImage.mImage = GameApp.gApp.GetImage(GameApp.gApp.GetResImagesDir() + waterShaderImage.mFileName, commitBits: true, allowTriReps: true, isInAtlas: false);
				continue;
			}
			SharedImageRef sharedImageRef = GameApp.gApp.mResourceManager.LoadImage(waterShaderImage.mResId);
			if (sharedImageRef != null)
			{
				waterShaderImage.mImage = (DeviceImage)sharedImageRef.GetImage();
			}
		}
	}

	public override void Update()
	{
	}

	public override void DrawUnderBackground(Graphics g)
	{
		bool flag = GameApp.gApp.ShadersSupported();
		if (GameApp.gApp.mLoadingThreadStarted && !GameApp.gApp.mLoadingThreadCompleted)
		{
			flag = false;
		}
		flag = false;
		for (int i = 0; i < mImages.Count; i++)
		{
			if (!flag || mImages[i].mBypass)
			{
				int theX = (mImages[i].mScale ? Common._S(mImages[i].mX) : Common._DS(mImages[i].mX - 160));
				int theY = (mImages[i].mScale ? Common._S(mImages[i].mY) : Common._DS(mImages[i].mY));
				g.DrawImage(mImages[i].mImage, theX, theY);
			}
		}
	}

	public override void SetParams(string key, string value)
	{
		if (key.IndexOf("image") != 0 && key.IndexOf("resid") != 0)
		{
			return;
		}
		int i;
		for (i = 5; i < key.Length; i++)
		{
			try
			{
				float.Parse(string.Concat(key[i]));
			}
			catch (Exception)
			{
				break;
			}
		}
		string text = key.Substring(5, i - 5);
		int num = SexyFramework.Common.StrToInt(text);
		WaterShaderImage waterShaderImage = null;
		for (int j = 0; j < mImages.Count; j++)
		{
			if (mImages[j].mId == num)
			{
				waterShaderImage = mImages[j];
				break;
			}
		}
		if (waterShaderImage == null)
		{
			waterShaderImage = new WaterShaderImage();
			mImages.Add(waterShaderImage);
			waterShaderImage.mId = num;
		}
		char c = key[key.Length - 1];
		if (text.Length + 5 == key.Length)
		{
			if (key.IndexOf("image") == 0)
			{
				waterShaderImage.mFileName = value;
			}
			else
			{
				waterShaderImage.mResId = "IMAGE_LEVELS_" + value;
			}
			return;
		}
		switch (c)
		{
		case 'X':
		case 'x':
			waterShaderImage.mX = SexyFramework.Common.StrToInt(value);
			return;
		case 'Y':
		case 'y':
			waterShaderImage.mY = SexyFramework.Common.StrToInt(value);
			return;
		}
		if (SexyFramework.Common.StrEquals(key.Substring(text.Length + 5, key.Length), "scale"))
		{
			waterShaderImage.mScale = bool.Parse(value);
		}
		else if (SexyFramework.Common.StrEquals(key.Substring(text.Length + 5, key.Length), "bypass"))
		{
			waterShaderImage.mBypass = bool.Parse(value);
		}
	}

	public override void NukeParams()
	{
		for (int i = 0; i < mImages.Count; i++)
		{
			if (mImages[i].mFileName.Length > 0)
			{
				if (mImages[i].mImage != null)
				{
					mImages[i].mImage.Dispose();
					mImages[i].mImage = null;
				}
			}
			else
			{
				GameApp.gApp.mResourceManager.DeleteResources(mImages[i].mResId);
			}
		}
		mImages.Clear();
	}

	public override string GetName()
	{
		return "WaterShader1";
	}

	public override void CopyFrom(Effect e)
	{
	}
}
