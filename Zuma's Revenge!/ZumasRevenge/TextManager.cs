using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using SexyFramework.Drivers.App;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class TextManager
{
	protected static TextManager instance = new TextManager();

	private readonly string[] sLangFiles = new string[12]
	{
		"en-US", "fr-FR", "it-IT", "de-DE", "es-ES", "zh-CN", "ru-RU", "pl-PL", "pt-PT", "es-CO",
		"zh-TW", "pt-BR"
	};

	private readonly string[] sTextFiles = new string[12]
	{
		"text/text_EN.txt", "text/text_FR.txt", "text/text_IT.txt", "text/text_GR.txt", "text/text_SP.txt", "text/text_CH.txt", "text/text_RU.txt", "text/text_PL.txt", "text/text_PG.txt", "text/text_SPC.txt",
		"text/text_CHT.txt", "text/text_PGB.txt"
	};

	protected List<string> mStringList = new List<string>(300);

	public static TextManager getInstance()
	{
		return instance;
	}

	protected TextManager()
	{
	}

	public bool init()
	{
		Localization.LanguageType currentLanguage = Localization.GetCurrentLanguage();
		int languageIndex = GetLanguageIndex(currentLanguage);
		CultureInfo currentCulture = new CultureInfo(sLangFiles[languageIndex]);
		Thread.CurrentThread.CurrentCulture = currentCulture;
		Thread.CurrentThread.CurrentUICulture = currentCulture;
		AppResources.Culture = currentCulture;
		releaseTextKit();
		bool flag = LoadTextKit(sTextFiles[languageIndex]);
		if (!flag && currentLanguage != Localization.LanguageType.Language_EN)
		{
			flag = LoadTextKit(sTextFiles[(int)Localization.LanguageType.Language_EN]);
		}
		return flag;
	}

	public bool LoadTextKitFromStream(Stream s)
	{
		bool result = true;
		try
		{
			using StreamReader streamReader = new StreamReader(s);
			for (string text = streamReader.ReadLine(); text != null; text = streamReader.ReadLine())
			{
				text = text.Replace("\\n", "\n");
				text = text.Replace("&cr;", "\n");
				mStringList.Add(text);
			}
		}
		catch (Exception)
		{
			result = false;
		}
		return result;
	}

	public bool LoadTextKit(string file)
	{
		bool result = true;
		Stream stream = null;
		try
		{
			string text2 = WP7ContentManager.ResolveAssetStreamPath(file);
			stream = TitleContainer.OpenStream("Content/" + text2);
			using StreamReader streamReader = new StreamReader(stream);
			for (string text = streamReader.ReadLine(); text != null; text = streamReader.ReadLine())
			{
				text = text.Replace("\\n", "\n");
				text = text.Replace("&cr;", "\n");
				mStringList.Add(text);
			}
		}
		catch (Exception)
		{
			result = false;
		}
		finally
		{
			stream?.Close();
		}
		return result;
	}

	public void releaseTextKit()
	{
		mStringList.Clear();
	}

	public string getString(int id)
	{
		if (id >= 0 && id < mStringList.Count)
		{
			return mStringList[id];
		}
		string stringNameByID = StringID.GetStringNameByID(id);
		return AppResources.ResourceManager.GetString(stringNameByID, AppResources.Culture ?? Thread.CurrentThread.CurrentUICulture);
	}

	public int getIdByString(string s)
	{
		if (s == "")
		{
			return -1;
		}
		for (int i = 0; i < mStringList.Count; i++)
		{
			if (mStringList[i].Trim() == s.Trim())
			{
				return i;
			}
		}
		throw new Exception("failed to find string - " + s);
	}

	private int GetLanguageIndex(Localization.LanguageType languageType)
	{
		int num = (int)languageType;
		if (num < 0 || num >= sTextFiles.Length)
		{
			return (int)Localization.LanguageType.Language_EN;
		}
		return num;
	}
}
