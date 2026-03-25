using SexyFramework.Drivers.Profile;

namespace ZumasRevenge.Profile;

public class Profile
{
	private FilesystemProfileData fspd = new FilesystemProfileData(new UserProfile());

	private int aa;

	public void loadProfile()
	{
		fspd.LoadDetails();
	}

	public void saveAll()
	{
		fspd.SaveDetails();
	}
}
