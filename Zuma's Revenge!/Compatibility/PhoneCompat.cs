using System;
using System.Diagnostics;

namespace Microsoft.Phone.Shell
{
	public class PhoneApplicationService
	{
		private static readonly PhoneApplicationService s_current = new PhoneApplicationService();

		public static PhoneApplicationService Current => s_current;

		public event EventHandler Activated;

		public event EventHandler Deactivated;

		public void RaiseActivated()
		{
			Activated?.Invoke(this, EventArgs.Empty);
		}

		public void RaiseDeactivated()
		{
			Deactivated?.Invoke(this, EventArgs.Empty);
		}
	}
}

namespace Microsoft.Phone.Tasks
{
	public enum MarketplaceContentType
	{
		Applications = 1
	}

	public class WebBrowserTask
	{
		public Uri Uri { get; set; }

		public void Show()
		{
			LaunchUri(Uri);
		}

		internal static void LaunchUri(Uri uri)
		{
			if (uri == null)
			{
				return;
			}
			try
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = uri.ToString(),
					UseShellExecute = true
				});
			}
			catch
			{
			}
		}
	}

	public class MarketplaceDetailTask
	{
		public MarketplaceContentType ContentType { get; set; }

		public string ContentIdentifier { get; set; }

		public void Show()
		{
			if (!string.IsNullOrWhiteSpace(ContentIdentifier))
			{
				WebBrowserTask.LaunchUri(new Uri($"https://www.microsoft.com/store/productid/{ContentIdentifier}"));
			}
		}
	}
}
