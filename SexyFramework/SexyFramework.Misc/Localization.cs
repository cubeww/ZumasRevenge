namespace SexyFramework.Misc;

public static class Localization
{
	public enum LanguageType
	{
		Language_EN,
		Language_FR,
		Language_IT,
		Language_GR,
		Language_SP,
		Language_CH,
		Language_RU,
		Language_PL,
		Language_PG,
		Language_SPC,
		Language_CHT,
		Language_PGB,
		Language_UNKNOWN
	}

	public static string[] LanguageSuffix = new string[13]
	{
		"_EN", "_FR", "_IT", "_GR", "_SP", "_CH", "_RU", "_PL", "_PG", "_SPC",
		"_CHT", "_PGB", ""
	};

	private static LanguageType sCurrentLanguage = LanguageType.Language_EN;

	public static void InitLanguage()
	{
		SexyAppBase gSexyAppBase = GlobalMembers.gSexyAppBase;
		sCurrentLanguage = gSexyAppBase.mAppDriver.GetAppLanguage();
	}

	public static void ChangeLanguage(LanguageType lan)
	{
		sCurrentLanguage = lan;
	}

	public static LanguageType GetCurrentLanguage()
	{
		return sCurrentLanguage;
	}

	public static int GetCurrentFontOffsetY()
	{
		if (sCurrentLanguage == LanguageType.Language_CH || sCurrentLanguage == LanguageType.Language_CHT)
		{
			return 10;
		}
		return 0;
	}

	public static string GetLanguageSuffix(LanguageType lan)
	{
		return LanguageSuffix[(int)lan];
	}

	public static string GetCurrentThousandSep()
	{
		if (sCurrentLanguage == LanguageType.Language_FR || sCurrentLanguage == LanguageType.Language_RU)
		{
			return " ";
		}
		if (sCurrentLanguage == LanguageType.Language_GR || sCurrentLanguage == LanguageType.Language_SP || sCurrentLanguage == LanguageType.Language_SPC || sCurrentLanguage == LanguageType.Language_IT || sCurrentLanguage == LanguageType.Language_PG || sCurrentLanguage == LanguageType.Language_PGB)
		{
			return ".";
		}
		if (sCurrentLanguage == LanguageType.Language_PL)
		{
			return "";
		}
		return ",";
	}

	public static int GetCurrentSeperateCount()
	{
		return 3;
	}
}
