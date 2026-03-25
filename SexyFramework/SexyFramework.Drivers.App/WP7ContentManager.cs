using System.Collections.Generic;
using System.IO;
using System;
using Microsoft.Xna.Framework.Content;
#if ANDROID
using Android.App;
using Android.Content.Res;
#endif

namespace SexyFramework.Drivers.App;

public class WP7ContentManager : ContentManager
{
	private Action<IDisposable> mCustom;

#if ANDROID
	private static readonly object sAssetMapLock = new object();

	private static Dictionary<string, string> sAssetNameMap;

	private static Dictionary<string, string> sAssetFilePathMap;
#endif

	public WP7ContentManager(IServiceProvider serviceProvider)
		: base(serviceProvider)
	{
		mCustom = CustomDispose;
	}

	public T LoadResDirectly<T>(string name)
	{
		return ReadAsset<T>(ResolveAssetName(name), mCustom);
	}

	public string ResolveAssetName(string name)
	{
#if ANDROID
		string key = NormalizeAssetKey(name);
		Dictionary<string, string> assetNameMap = GetOrBuildAssetMaps().Item1;
		if (assetNameMap.TryGetValue(key, out var value))
		{
			return value;
		}
		return key;
#else
		return name;
#endif
	}

	public static string ResolveAssetStreamPath(string name)
	{
#if ANDROID
		string key = NormalizeAssetKey(name);
		Dictionary<string, string> assetFilePathMap = GetOrBuildAssetMaps().Item2;
		if (assetFilePathMap.TryGetValue(key, out var value))
		{
			return value;
		}
		return (name ?? string.Empty).Replace('\\', '/').TrimStart('/');
#else
		return name;
#endif
	}

#if ANDROID
	private static Tuple<Dictionary<string, string>, Dictionary<string, string>> GetOrBuildAssetMaps()
	{
		if (sAssetNameMap != null && sAssetFilePathMap != null)
		{
			return Tuple.Create(sAssetNameMap, sAssetFilePathMap);
		}
		lock (sAssetMapLock)
		{
			if (sAssetNameMap != null && sAssetFilePathMap != null)
			{
				return Tuple.Create(sAssetNameMap, sAssetFilePathMap);
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			AssetManager assets = Application.Context?.Assets;
			if (assets != null)
			{
				AddAssetDirectoryEntries(assets, "Content", "", dictionary, dictionary2);
			}
			sAssetNameMap = dictionary;
			sAssetFilePathMap = dictionary2;
			return Tuple.Create(sAssetNameMap, sAssetFilePathMap);
		}
	}

	private static void AddAssetDirectoryEntries(AssetManager assets, string rootPath, string relativePath, Dictionary<string, string> assetNameMap, Dictionary<string, string> assetFilePathMap)
	{
		string text = string.IsNullOrEmpty(relativePath) ? rootPath : rootPath + "/" + relativePath;
		string[] array = assets.List(text) ?? Array.Empty<string>();
		foreach (string text2 in array)
		{
			string text3 = string.IsNullOrEmpty(relativePath) ? text2 : relativePath + "/" + text2;
			string[] array2 = assets.List(rootPath + "/" + text3) ?? Array.Empty<string>();
			if (array2.Length > 0)
			{
				AddAssetDirectoryEntries(assets, rootPath, text3, assetNameMap, assetFilePathMap);
				continue;
			}
			string text4 = Path.ChangeExtension(text3, null) ?? text3;
			assetNameMap[NormalizeAssetKey(text4)] = text4.Replace('\\', '/');
			assetFilePathMap[NormalizeAssetKey(text3)] = text3.Replace('\\', '/');
		}
	}

	private static string NormalizeAssetKey(string name)
	{
		return (name ?? string.Empty).Replace('\\', '/').TrimStart('/').ToLowerInvariant();
	}
#endif

	public void CustomDispose<IDisposable>(IDisposable obj)
	{
	}
}
