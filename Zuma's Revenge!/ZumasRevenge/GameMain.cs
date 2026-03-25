using System;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using SexyFramework;
using SexyFramework.Drivers.App;
using SexyFramework.Drivers.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class GameMain : Game
{
	private GameApp SexyZuma;

	private Texture2D splashEA;

	private Texture2D splash;

	private Color mColor = new Color(255, 255, 255, 255);

	private float mAlpha = 255f;

	private float mAlphaInc = -6f;

	private double mAlphaDelay = 1.0;

	private int mSplashId = 1;

	private SpriteBatch spriteBatch;

	private bool isLoading = true;

	private bool mInitBegin;

	private int FirstLoad;

	private SpriteFont mSpriteFont;

	private double mElipseTime;

	private int mCurrentTouchId = -1;

	private static int frames = 0;

	private static DateTime now;

	private static DateTime preFPSTime;

	private static string fpsDisplayText = "";

	public PhoneApplicationService gApplicationService;

	private long totalBytes;

	private long currentBytes;

	private long peakBytes;

	private long limitBytes;

	private Vector2 mFPSPos = new Vector2(60f, 10f);

	private Rectangle mDisplayRect = new Rectangle(0, 0, 800, 480);

	private int mLastViewportWidth = -1;

	private int mLastViewportHeight = -1;

	private int mLastTargetWidth = -1;

	private int mLastTargetHeight = -1;

	private bool mViewportSizeAppliedToRenderer;

	private SexyAppBase.Touch touch = new SexyAppBase.Touch();

	private bool mMouseTouchActive;

	public GameMain()
	{
		base.Content = new WP7ContentManager(base.Services);
		base.Content.RootDirectory = "Content";
		base.TargetElapsedTime = TimeSpan.FromTicks(166666L);
		base.IsFixedTimeStep = true;
		base.IsMouseVisible = true;
		base.Window.AllowUserResizing = true;
		SexyZuma = new GameApp(this, from_reinit: false);
		GlobalMembers.gSexyApp = SexyZuma;
		GlobalMembers.gSexyAppBase = SexyZuma;
		gApplicationService = PhoneApplicationService.Current;
		gApplicationService.Deactivated += OnServiceDeactivated;
		gApplicationService.Activated += OnServiceActivated;
		base.Exiting += OnExiting;
		base.Components.Add(new GamerServicesComponent(this));
		Guide.SimulateTrialMode = false;
	}

	protected override void Initialize()
	{
		base.Initialize();
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		mSpriteFont = base.Content.Load<SpriteFont>("Arial_20");
		base.Window.OrientationChanged += OrientationChanged;
		UpdateDisplayRect();
		SexyZuma.InitText();
		if (Localization.GetCurrentLanguage() != Localization.LanguageType.Language_FR)
		{
			splash = base.Content.Load<Texture2D>("Default-Landscape");
		}
		else
		{
			splash = base.Content.Load<Texture2D>("LoadingImage_DarkFrog_French");
		}
	}

	protected override void LoadContent()
	{
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		UpdateDisplayRect();
		if (GameApp.mExit)
		{
			Exit();
		}
		_ = gameTime.IsRunningSlowly;
		try
		{
			if (!Guide.IsVisible)
			{
				base.Update(gameTime);
			}
		}
		catch (GameUpdateRequiredException ex)
		{
			if (GameApp.USE_XBOX_SERVICE)
			{
				SexyZuma.HandleGameUpdateRequired(ex);
			}
		}
		UpdateInput(gameTime);
		try
		{
			if (Guide.IsVisible)
			{
				return;
			}
		}
		catch (Exception)
		{
		}
		if (!isLoading)
		{
			SexyZuma.Update(gameTime.ElapsedGameTime.Seconds);
			return;
		}
		mElipseTime += gameTime.ElapsedGameTime.TotalSeconds;
		if (!mInitBegin)
		{
			GC.Collect();
			SexyZuma.StartThreadInit();
			mInitBegin = true;
		}
		else if (SexyZuma.mInitFinished && mElipseTime >= 4.0)
		{
			SexyZuma.ShowLoadingScreen();
			isLoading = false;
			mLastViewportWidth = -1;
			mLastViewportHeight = -1;
			mLastTargetWidth = -1;
			mLastTargetHeight = -1;
			mViewportSizeAppliedToRenderer = false;
		}
	}

	protected override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.Clear(Color.Black);
		if (isLoading)
		{
			spriteBatch.Begin();
			mColor = new Color((byte)MathHelper.Clamp(mAlpha, 0f, 255f), (byte)MathHelper.Clamp(mAlpha, 0f, 255f), (byte)MathHelper.Clamp(mAlpha, 0f, 255f), (byte)MathHelper.Clamp(mAlpha, 0f, 255f));
			spriteBatch.Draw(splash, mDisplayRect, mColor);
			spriteBatch.End();
		}
		else
		{
			if (splash != null)
			{
				splash.Dispose();
				splash = null;
			}
			SexyZuma.Draw(0);
		}
		base.Draw(gameTime);
	}

	private void OnExiting(object sender, EventArgs args)
	{
		SexyZuma.OnExiting();
	}

	protected override void OnActivated(object sender, EventArgs args)
	{
		mLastViewportWidth = -1;
		mLastViewportHeight = -1;
		mLastTargetWidth = -1;
		mLastTargetHeight = -1;
		mViewportSizeAppliedToRenderer = false;
		SexyZuma.OnActivated();
		base.OnActivated(sender, args);
	}

	protected override void OnDeactivated(object sender, EventArgs args)
	{
		if (!SexyZuma.mInitFinished)
		{
			mElipseTime -= 2.0;
		}
		SexyZuma.OnExiting();
		SexyZuma.OnDeactivated();
		base.OnDeactivated(sender, args);
	}

	protected void OnServiceActivated(object sender, EventArgs args)
	{
		mLastViewportWidth = -1;
		mLastViewportHeight = -1;
		mLastTargetWidth = -1;
		mLastTargetHeight = -1;
		mViewportSizeAppliedToRenderer = false;
		SexyZuma.OnServiceActivated();
	}

	protected void OnServiceDeactivated(object sender, EventArgs args)
	{
		SexyZuma.OnServiceDeactivated();
	}

	private void UpdateInput(GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
		{
			if (isLoading)
			{
				Exit();
			}
			else
			{
				SexyZuma.OnHardwareBackButtonPressed();
			}
		}
		TouchCollection state = TouchPanel.GetState();
		if (state.Count > 0)
		{
			foreach (TouchLocation item in state)
			{
				if (mCurrentTouchId == -1)
				{
					if (item.State != TouchLocationState.Pressed || !TryGetPointerLocation((int)item.Position.X, (int)item.Position.Y, clampToDisplayRect: false, out var loc2))
					{
						continue;
					}
					mCurrentTouchId = item.Id;
					touch.SetTouchInfo(loc2, _TouchPhase.TOUCH_BEGAN, DateTime.Now.TimeOfDay.TotalMilliseconds);
					SexyZuma.TouchBegan(touch);
					continue;
				}
				if (item.Id != mCurrentTouchId)
				{
					continue;
				}
				if (!TryGetPointerLocation((int)item.Position.X, (int)item.Position.Y, clampToDisplayRect: true, out var loc))
				{
					continue;
				}
				switch (item.State)
				{
				case TouchLocationState.Moved:
					touch.SetTouchInfo(loc, _TouchPhase.TOUCH_MOVED, DateTime.Now.TimeOfDay.TotalMilliseconds);
					SexyZuma.TouchMoved(touch);
					break;
				case TouchLocationState.Released:
					touch.SetTouchInfo(loc, _TouchPhase.TOUCH_ENDED, DateTime.Now.TimeOfDay.TotalMilliseconds);
					SexyZuma.TouchEnded(touch);
					mCurrentTouchId = -1;
					break;
				}
			}
			return;
		}
		mCurrentTouchId = -1;
		UpdateMouseInput();
	}

	private void UpdateMouseInput()
	{
		MouseState state = Mouse.GetState();
		if (!isLoading)
		{
			if (state.LeftButton == ButtonState.Pressed)
			{
				if (!mMouseTouchActive)
				{
					if (TryGetPointerLocation(state.X, state.Y, clampToDisplayRect: false, out var loc2))
					{
						SendMouseTouch(loc2, _TouchPhase.TOUCH_BEGAN);
						mMouseTouchActive = true;
					}
				}
				else if (TryGetPointerLocation(state.X, state.Y, clampToDisplayRect: true, out var loc3))
				{
					SendMouseTouch(loc3, _TouchPhase.TOUCH_MOVED);
				}
			}
			else if (mMouseTouchActive)
			{
				if (TryGetPointerLocation(state.X, state.Y, clampToDisplayRect: true, out var loc4))
				{
					SendMouseTouch(loc4, _TouchPhase.TOUCH_ENDED);
				}
				mMouseTouchActive = false;
			}
			else if (TryGetPointerLocation(state.X, state.Y, clampToDisplayRect: false, out var loc))
			{
				SexyZuma.mWidgetManager.MouseMove(loc.mX, loc.mY);
			}
		}
	}

	private void SendMouseTouch(SexyFramework.Misc.Point loc, _TouchPhase phase)
	{
		touch.SetTouchInfo(loc, phase, DateTime.Now.TimeOfDay.TotalMilliseconds);
		switch (phase)
		{
		case _TouchPhase.TOUCH_BEGAN:
			SexyZuma.TouchBegan(touch);
			break;
		case _TouchPhase.TOUCH_MOVED:
			SexyZuma.TouchMoved(touch);
			break;
		case _TouchPhase.TOUCH_ENDED:
			SexyZuma.TouchEnded(touch);
			break;
		}
	}

	private void UpdateDisplayRect()
	{
		int viewportWidth = GetViewportWidth();
		int viewportHeight = GetViewportHeight();
		SyncTouchPanelDisplaySize(viewportWidth, viewportHeight);
		int targetWidth = 800;
		int targetHeight = 480;
		if (SexyZuma != null && SexyZuma.mWidgetManager != null)
		{
			Rect screenRect = SexyZuma.GetScreenRect();
			targetWidth = Math.Max(screenRect.mWidth, 1);
			targetHeight = Math.Max(screenRect.mHeight, 1);
		}
		if (viewportWidth == mLastViewportWidth && viewportHeight == mLastViewportHeight && targetWidth == mLastTargetWidth && targetHeight == mLastTargetHeight)
		{
			if (!isLoading && SexyZuma?.mGraphicsDriver is XNAGraphicsDriver xNAGraphicsDriver2 && (!mViewportSizeAppliedToRenderer || !RendererMatchesViewport(xNAGraphicsDriver2, viewportWidth, viewportHeight)))
			{
				xNAGraphicsDriver2.WindowResize(viewportWidth, viewportHeight);
				mViewportSizeAppliedToRenderer = RendererMatchesViewport(xNAGraphicsDriver2, viewportWidth, viewportHeight);
			}
			return;
		}
		mLastViewportWidth = viewportWidth;
		mLastViewportHeight = viewportHeight;
		mLastTargetWidth = targetWidth;
		mLastTargetHeight = targetHeight;
		mViewportSizeAppliedToRenderer = false;
		float num = Math.Min((float)viewportWidth / (float)targetWidth, (float)viewportHeight / (float)targetHeight);
		int width = Math.Max(1, (int)Math.Round((float)targetWidth * num));
		int height = Math.Max(1, (int)Math.Round((float)targetHeight * num));
		mDisplayRect = new Rectangle((viewportWidth - width) / 2, (viewportHeight - height) / 2, width, height);
		if (!isLoading && SexyZuma?.mGraphicsDriver is XNAGraphicsDriver xNAGraphicsDriver)
		{
			xNAGraphicsDriver.WindowResize(viewportWidth, viewportHeight);
			mViewportSizeAppliedToRenderer = RendererMatchesViewport(xNAGraphicsDriver, viewportWidth, viewportHeight);
		}
	}

	private void SyncTouchPanelDisplaySize(int viewportWidth, int viewportHeight)
	{
		viewportWidth = Math.Max(viewportWidth, 1);
		viewportHeight = Math.Max(viewportHeight, 1);
		if (TouchPanel.DisplayWidth == viewportWidth && TouchPanel.DisplayHeight == viewportHeight)
		{
			return;
		}
		TouchPanel.DisplayWidth = viewportWidth;
		TouchPanel.DisplayHeight = viewportHeight;
	}

	private bool RendererMatchesViewport(XNAGraphicsDriver graphicsDriver, int viewportWidth, int viewportHeight)
	{
		if (graphicsDriver?.mXNARenderDevice?.mDevice?.GraphicsDevice == null)
		{
			return false;
		}
		int backBufferWidth = graphicsDriver.mXNARenderDevice.mDevice.GraphicsDevice.PresentationParameters.BackBufferWidth;
		int backBufferHeight = graphicsDriver.mXNARenderDevice.mDevice.GraphicsDevice.PresentationParameters.BackBufferHeight;
		return backBufferWidth == viewportWidth && backBufferHeight == viewportHeight;
	}

	private bool TryGetPointerLocation(int rawX, int rawY, bool clampToDisplayRect, out SexyFramework.Misc.Point point)
	{
		Rectangle rectangle = mDisplayRect;
		if (SexyZuma != null)
		{
			int x = 0;
			int y = 0;
			SexyZuma.GetTouchInputOffset(ref x, ref y);
			rectangle.Offset(x, y);
		}
		bool flag = rawX >= rectangle.Left && rawX < rectangle.Right && rawY >= rectangle.Top && rawY < rectangle.Bottom;
		if (!flag && !clampToDisplayRect)
		{
			point = default(SexyFramework.Misc.Point);
			return false;
		}
		int num = Math.Max(rectangle.Width, 1);
		int num2 = Math.Max(rectangle.Height, 1);
		int num3 = rawX;
		int num4 = rawY;
		if (clampToDisplayRect)
		{
			num3 = Math.Clamp(num3, rectangle.Left, rectangle.Right - 1);
			num4 = Math.Clamp(num4, rectangle.Top, rectangle.Bottom - 1);
		}
		int num5 = 800;
		int num6 = 480;
		if (SexyZuma != null && SexyZuma.mWidgetManager != null)
		{
			Rect screenRect = SexyZuma.GetScreenRect();
			num5 = Math.Max(screenRect.mWidth, 1);
			num6 = Math.Max(screenRect.mHeight, 1);
		}
		float x2 = (float)(num3 - rectangle.X) * (float)num5 / (float)num;
		float y2 = (float)(num4 - rectangle.Y) * (float)num6 / (float)num2;
		point = new SexyFramework.Misc.Point((int)Math.Clamp(x2, 0f, num5 - 1f), (int)Math.Clamp(y2, 0f, num6 - 1f));
		return true;
	}

	private int GetViewportWidth()
	{
		int backBufferWidth = 0;
		int viewportWidth = 0;
		if (base.GraphicsDevice != null)
		{
			backBufferWidth = Math.Max(base.GraphicsDevice.PresentationParameters.BackBufferWidth, 0);
			viewportWidth = Math.Max(base.GraphicsDevice.Viewport.Width, 0);
		}
		int clientWidth = Math.Max(base.Window.ClientBounds.Width, 0);
		return Math.Max(Math.Max(backBufferWidth, viewportWidth), Math.Max(clientWidth, 1));
	}

	private int GetViewportHeight()
	{
		int backBufferHeight = 0;
		int viewportHeight = 0;
		if (base.GraphicsDevice != null)
		{
			backBufferHeight = Math.Max(base.GraphicsDevice.PresentationParameters.BackBufferHeight, 0);
			viewportHeight = Math.Max(base.GraphicsDevice.Viewport.Height, 0);
		}
		int clientHeight = Math.Max(base.Window.ClientBounds.Height, 0);
		return Math.Max(Math.Max(backBufferHeight, viewportHeight), Math.Max(clientHeight, 1));
	}

	public void DrawSysString(string str, float x, float y)
	{
		spriteBatch.Begin();
		spriteBatch.DrawString(mSpriteFont, str, new Vector2(x, y), Color.Yellow);
		spriteBatch.End();
	}

	public void OrientationChanged(object sender, EventArgs e)
	{
		if (base.Window.CurrentOrientation == DisplayOrientation.LandscapeLeft)
		{
			if (SexyZuma != null)
			{
				SexyZuma.SetOrientation(0);
			}
		}
		else if (SexyZuma != null)
		{
			SexyZuma.SetOrientation(1);
		}
	}
}
