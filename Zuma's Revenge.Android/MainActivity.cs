using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Window;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using ZumasRevenge;

namespace ZumasRevenge.AndroidHost;

[Activity(
	Label = "Zuma's Revenge!",
	Icon = "@mipmap/appicon",
	MainLauncher = true,
	AlwaysRetainTaskState = true,
	EnableOnBackInvokedCallback = true,
	LaunchMode = LaunchMode.SingleTask,
	ScreenOrientation = ScreenOrientation.SensorLandscape,
	ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AndroidGameActivity
{
	private GameMain mGame;

	private HardwareBackInvokedCallback mBackInvokedCallback;

	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		ApplyImmersiveMode();
		mGame = new GameMain();
		View view = (View)mGame.Services.GetService(typeof(View));
		ViewGroup.LayoutParams layoutParams = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
		view.LayoutParameters = layoutParams;
		SetContentView(view, layoutParams);
		RegisterBackCallback();
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
		UnregisterBackCallback();
		mGame?.Dispose();
		mGame = null;
		base.OnDestroy();
	}

	public override bool DispatchKeyEvent(KeyEvent e)
	{
		if (e != null && e.KeyCode == Keycode.Back)
		{
			if (e.Action == KeyEventActions.Down)
			{
				return true;
			}
			if (e.Action == KeyEventActions.Up && DispatchHardwareBackButton())
			{
				return true;
			}
		}
		return base.DispatchKeyEvent(e);
	}

	public override void OnBackPressed()
	{
		if (DispatchHardwareBackButton())
		{
			return;
		}
		base.OnBackPressed();
	}

	private bool DispatchHardwareBackButton()
	{
		if (mGame == null)
		{
			return false;
		}
		mGame.HandleHardwareBackButton();
		return true;
	}

	private void RegisterBackCallback()
	{
		if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu || mBackInvokedCallback != null)
		{
			return;
		}
		mBackInvokedCallback = new HardwareBackInvokedCallback(this);
		OnBackInvokedDispatcher?.RegisterOnBackInvokedCallback(IOnBackInvokedDispatcher.PriorityDefault, mBackInvokedCallback);
	}

	private void UnregisterBackCallback()
	{
		if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu || mBackInvokedCallback == null)
		{
			return;
		}
		OnBackInvokedDispatcher?.UnregisterOnBackInvokedCallback(mBackInvokedCallback);
		mBackInvokedCallback = null;
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

	private sealed class HardwareBackInvokedCallback : Java.Lang.Object, IOnBackInvokedCallback
	{
		private readonly MainActivity mActivity;

		public HardwareBackInvokedCallback(MainActivity activity)
		{
			mActivity = activity;
		}

		public void OnBackInvoked()
		{
			mActivity.DispatchHardwareBackButton();
		}
	}
}
