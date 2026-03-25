using System;
using Android.App;
using Android.Runtime;

namespace ZumasRevenge.AndroidHost;

[Application(Icon = "@mipmap/appicon", Label = "Zuma's Revenge!")]
public class AppApplication : Application
{
	public AppApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}
}
