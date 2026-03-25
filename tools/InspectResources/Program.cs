using Microsoft.Xna.Framework;
using SexyFramework.Drivers.App;
using SexyFramework.Resource;
using SexyFramework.Graphics;

string rootDirectory = args.Length > 0
	? args[0]
	: Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Zuma's Revenge!", "Content"));
string manifestAsset = args.Length > 1 ? args[1] : "properties/resources/resources_EN.xml";

Console.WriteLine("RootDirectory=" + rootDirectory);
Console.WriteLine("ManifestAsset=" + manifestAsset);

GameServiceContainer services = new GameServiceContainer();
WP7ContentManager content = new WP7ContentManager(services)
{
	RootDirectory = rootDirectory
};

ResourceManager manager = content.Load<ResourceManager>(manifestAsset);

string[] probes =
{
	@"fonts\640\shaglounge28_base_layer0",
	@"fonts\640\shaglounge28_base_layer1",
	@"fonts\640\shaglounge28_base_layer2",
	@"fonts\640\shaglounge28_base_layer3",
	@"fonts\640\shagexotica68_base_layer0",
	@"fonts\640\shagexotica38_base_layer0",
	@"fonts\640\shaglounge45_base_layer0"
};

foreach (string probe in probes)
{
	string id = manager.GetIdByPath(probe);
	Console.WriteLine($"{probe} => {id}");
	if (!string.IsNullOrEmpty(id))
	{
		ImageRes imageRes = manager.mResMaps[0][id] as ImageRes;
		if (imageRes != null)
		{
			Console.WriteLine($"  Path={imageRes.mPath}");
			Console.WriteLine($"  AtlasName={imageRes.mAtlasName}");
			Console.WriteLine($"  AtlasRect={imageRes.mAtlasX},{imageRes.mAtlasY},{imageRes.mAtlasW},{imageRes.mAtlasH}");
			Console.WriteLine($"  IsAtlas={imageRes.mIsAtlas}");
		}
	}
}

Console.WriteLine();
Console.WriteLine("Matches in mResFromPathMap:");
foreach (string key in manager.mResFromPathMap.Keys.OrderBy(x => x))
{
	if (key.Contains("SHAGLOUNGE28") || key.Contains("SHAGEXOTICA68") || key.Contains("SHAGEXOTICA38") || key.Contains("SHAGLOUNGE45"))
	{
		Console.WriteLine(key);
	}
}

Console.WriteLine();
Console.WriteLine("Atlas ids:");
foreach (string key in manager.mResMaps[0].Keys.OrderBy(x => x))
{
	if (key.Contains("ATLASIMAGE_ATLAS_TEXT_640_00") || key.Contains("ATLASIMAGE_ATLAS_TEXT_640_01"))
	{
		ImageRes imageRes = manager.mResMaps[0][key] as ImageRes;
		Console.WriteLine($"{key} => {imageRes?.mPath}");
	}
}

Console.WriteLine();
Console.WriteLine("Font ids:");
foreach (string key in manager.mResMaps[2].Keys.OrderBy(x => x))
{
	if (key.Contains("FONT_SHAGLOUNGE28_BASE") || key.Contains("FONT_SHAGEXOTICA68_BASE") || key.Contains("FONT_SHAGEXOTICA38_BASE") || key.Contains("FONT_SHAGLOUNGE45_BASE"))
	{
		FontRes fontRes = manager.mResMaps[2][key] as FontRes;
		Console.WriteLine($"{key} => path='{fontRes?.mPath}' image='{fontRes?.mImagePath}' sys={fontRes?.mSysFont}");
	}
}
