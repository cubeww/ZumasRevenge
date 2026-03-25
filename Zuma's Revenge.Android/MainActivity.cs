using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using ZumasRevenge;

namespace ZumasRevenge.AndroidHost;

[Activity(
	Label = "Zuma's Revenge!",
	MainLauncher = true,
	AlwaysRetainTaskState = true,
	LaunchMode = LaunchMode.SingleTask,
	ScreenOrientation = ScreenOrientation.SensorLandscape,
	ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AndroidGameActivity
{
	private GameMain mGame;

	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		ApplyImmersiveMode();
		mGame = new GameMain();
		View view = (View)mGame.Services.GetService(typeof(View));
		ViewGroup.LayoutParams layoutParams = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
		view.LayoutParameters = layoutParams;
		SetContentView(view, layoutParams);
		mGame.Run();
	}

	protected override void OnResume()
	{
		base.OnResume();
		ApplyImmersiveMode();
		PhoneApplicationService.Current.RaiseActivated();
	}

	public override void OnWindowFocusChanged(bool hasFocus)
	{
		base.OnWindowFocusChanged(hasFocus);
		if (hasFocus)
		{
			ApplyImmersiveMode();
		}
	}

	protected override void OnPause()
	{
		PhoneApplicationService.Current.RaiseDeactivated();
		base.OnPause();
	}

	protected override void OnDestroy()
	{
		mGame?.Dispose();
		mGame = null;
		base.OnDestroy();
	}

	private void ApplyImmersiveMode()
	{
		if (Window == null)
		{
			return;
		}
		Window.AddFlags(WindowManagerFlags.Fullscreen);
		Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
		if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
		{
			WindowManagerLayoutParams attributes = Window.Attributes;
			attributes.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;
			Window.Attributes = attributes;
		}
		if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
		{
			Window.SetDecorFitsSystemWindows(false);
		}
		if (Window.DecorView != null)
		{
			Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(SystemUiFlags.LayoutStable | SystemUiFlags.LayoutHideNavigation | SystemUiFlags.LayoutFullscreen | SystemUiFlags.HideNavigation | SystemUiFlags.Fullscreen | SystemUiFlags.ImmersiveSticky);
		}
	}
}
