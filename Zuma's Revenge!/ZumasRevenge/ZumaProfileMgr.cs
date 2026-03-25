using System.Collections.Generic;
using SexyFramework.Drivers.Profile;

namespace ZumasRevenge;

public class ZumaProfileMgr : ProfileManager
{
	public ZumaProfileMgr()
		: base(GameApp.gApp)
	{
	}

	public void RenameTempProfile(string new_name)
	{
		if (GetProfile(".temp") == null || !RenameProfile(".temp", new_name))
		{
			AddProfile(new_name);
		}
	}

	public bool HasTempProfile()
	{
		return GetProfile(".temp") != null;
	}

	public void GetListOfUserNames(List<string> user_vec)
	{
		if (user_vec != null)
		{
			for (int i = 0; i < GetNumProfiles(); i++)
			{
				UserProfile profile = GetProfile(i);
				user_vec.Insert(user_vec.Count, profile.GetName());
			}
		}
	}
}
