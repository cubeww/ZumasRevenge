using System.Globalization;
using SexyFramework.Misc;

namespace SexyFramework.Drivers.App;

public enum AppLanguageMode
{
	FollowSystem,
	English,
	French,
	Italian,
	German,
	Spanish,
	SimplifiedChinese,
	Russian,
	Polish,
	Portuguese,
	LatinAmericanSpanish,
	TraditionalChinese,
	BrazilianPortuguese
}

public static class AppLanguageSettings
{
	// Default the game to Simplified Chinese for this project build.
	public static AppLanguageMode CurrentLanguageMode { get; set; } = AppLanguageMode.SimplifiedChinese;

	public static Localization.LanguageType ResolveLanguage()
	{
		return CurrentLanguageMode switch
		{
			AppLanguageMode.FollowSystem => ResolveSystemLanguage(),
			AppLanguageMode.English => Localization.LanguageType.Language_EN,
			AppLanguageMode.French => Localization.LanguageType.Language_FR,
			AppLanguageMode.Italian => Localization.LanguageType.Language_IT,
			AppLanguageMode.German => Localization.LanguageType.Language_GR,
			AppLanguageMode.Spanish => Localization.LanguageType.Language_SP,
			AppLanguageMode.SimplifiedChinese => Localization.LanguageType.Language_CH,
			AppLanguageMode.Russian => Localization.LanguageType.Language_RU,
			AppLanguageMode.Polish => Localization.LanguageType.Language_PL,
			AppLanguageMode.Portuguese => Localization.LanguageType.Language_PG,
			AppLanguageMode.LatinAmericanSpanish => Localization.LanguageType.Language_SPC,
			AppLanguageMode.TraditionalChinese => Localization.LanguageType.Language_CHT,
			AppLanguageMode.BrazilianPortuguese => Localization.LanguageType.Language_PGB,
			_ => Localization.LanguageType.Language_EN
		};
	}

	private static Localization.LanguageType ResolveSystemLanguage()
	{
		CultureInfo culture = CultureInfo.CurrentUICulture;
		string name = culture.Name.ToLowerInvariant();
		string language = culture.TwoLetterISOLanguageName.ToLowerInvariant();

		return name switch
		{
			"zh-cn" or "zh-sg" => Localization.LanguageType.Language_CH,
			"zh-tw" or "zh-hk" or "zh-mo" => Localization.LanguageType.Language_CHT,
			"pt-br" => Localization.LanguageType.Language_PGB,
			"pt-pt" => Localization.LanguageType.Language_PG,
			"es-co" or "es-mx" or "es-ar" or "es-cl" or "es-pe" or "es-ve" => Localization.LanguageType.Language_SPC,
			_ => language switch
			{
				"fr" => Localization.LanguageType.Language_FR,
				"it" => Localization.LanguageType.Language_IT,
				"de" => Localization.LanguageType.Language_GR,
				"es" => Localization.LanguageType.Language_SP,
				"zh" => Localization.LanguageType.Language_CH,
				"ru" => Localization.LanguageType.Language_RU,
				"pl" => Localization.LanguageType.Language_PL,
				"pt" => Localization.LanguageType.Language_PG,
				_ => Localization.LanguageType.Language_EN
			}
		};
	}
}
