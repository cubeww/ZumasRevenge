using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JeffLib;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class DeathSkull : IDisposable
{
	public enum DrawType
	{
		Draw_BG,
		Draw_FG,
		Draw_Both
	}

	protected class SkullFrame
	{
		public float mPctOpen;

		public float mAngle;

		public float mSize;

		public float mAlpha;

		public float mX;

		public float mY;

		public bool mIsFinalFrame;

		public SkullFrame()
		{
			mPctOpen = 1f;
			mAngle = 0f;
			mSize = 0f;
			mAlpha = 128f;
			mIsFinalFrame = false;
			mX = 0f;
			mY = 0f;
		}

		public void SyncState(DataSync sync)
		{
			sync.SyncFloat(ref mPctOpen);
			sync.SyncFloat(ref mAngle);
			sync.SyncFloat(ref mSize);
			sync.SyncFloat(ref mAlpha);
			sync.SyncFloat(ref mX);
			sync.SyncFloat(ref mY);
			sync.SyncBoolean(ref mIsFinalFrame);
		}
	}

	private const float MIN_MOUTH_OPEN = 0.41f;

	private const float STARTING_SIZE = 0.31f;

	private const float MAX_SIZE = 2.2f;

	protected List<SkullFrame> mFrames = new List<SkullFrame>();

	protected float mCurAngle;

	protected float mStartingAngle;

	protected float mAngleInc;

	protected float mAngleOutInc;

	protected float mX;

	protected float mY;

	protected float mVX;

	protected float mVY;

	protected float mDestX;

	protected float mDestY;

	protected float mStartX;

	protected float mStartY;

	protected float mOpeningRate;

	protected float mTextAlpha;

	protected bool mShowText;

	protected bool mDisappearing;

	protected bool mOpeningMouth;

	protected string mText = "";

	protected FrogFlyOff mFrogFlyOff;

	protected int mCloseDelay;

	protected int mMouthOpenDelay;

	protected int mUpdateCount;

	protected int mMoveFrames;

	protected Transform mGlobalTranform = new Transform();

	public int mFrogTX;

	public int mFrogTY;

	public bool mDone;

	public float mLastFrogAngle;

	public int mFrogClipHeight;

	public DeathSkull()
	{
		mUpdateCount = 0;
		mFrogFlyOff = null;
	}

	public virtual void Dispose()
	{
		mFrogFlyOff = null;
	}

	public void Init(float start_angle, float sx, float sy, float dx, float dy)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_HOLE_BASE);
		mFrogTX = (mFrogTY = -1);
		mLastFrogAngle = 0f;
		mFrogFlyOff = null;
		mMouthOpenDelay = Common._M(10);
		mOpeningRate = Common._M(0.016f);
		mCurAngle = (mStartingAngle = start_angle);
		mX = (mStartX = Common._S(sx) + (float)(imageByID.mWidth / 2) - (float)Common._DS(Common._M(10)));
		mY = (mStartY = Common._S(sy) + (float)(imageByID.mHeight / 2) + (float)Common._DS(Common._M(45)));
		mDestX = Common._S(dx) - (float)Common._S(Common._M(0));
		mDestY = Common._S(dy) - (float)Common._S(Common._M(250));
		mOpeningMouth = false;
		mCloseDelay = 0;
		mTextAlpha = 255f;
		mDone = false;
		mShowText = false;
		mFrogClipHeight = 0;
		mMoveFrames = Common._M(50);
		mVX = (mDestX - mX) / (float)mMoveFrames;
		mVY = (mDestY - mY) / (float)mMoveFrames;
		mDisappearing = false;
		float canonicalAngleRad = Common.GetCanonicalAngleRad(mCurAngle);
		if ((float)Math.PI * 2f - canonicalAngleRad < canonicalAngleRad)
		{
			mAngleInc = ((float)Math.PI * 2f - canonicalAngleRad) / (float)mMoveFrames;
			mAngleOutInc = canonicalAngleRad / (float)mMoveFrames;
		}
		else
		{
			mAngleInc = (0f - canonicalAngleRad) / (float)mMoveFrames;
			mAngleOutInc = (0f - ((float)Math.PI * 2f - canonicalAngleRad)) / (float)mMoveFrames;
		}
	}

	public void Update()
	{
		if (mDone)
		{
			return;
		}
		GameApp gApp = GameApp.gApp;
		if (mCloseDelay > 0)
		{
			mCloseDelay--;
		}
		else if (++mUpdateCount == mMoveFrames && mDisappearing && !mShowText)
		{
			if (gApp.GetBoard().IronFrogMode() || gApp.GetBoard().GauntletMode() || gApp.GetBoard().mLevel.IsFinalBossLevel())
			{
				mDone = true;
				return;
			}
			Board board = gApp.GetBoard();
			mShowText = true;
			mTextAlpha = Common._M(500);
			int num = board.GetNumLives() - 1;
			if (board.mLevel.mBoss != null || board.mLevel.IsFinalBossLevel())
			{
				mText = TextManager.getInstance().getString(442);
			}
			else if (board.IsCheckpointLevel() && num < 3)
			{
				mText = TextManager.getInstance().getString(443);
			}
			else if (num > 1)
			{
				StringBuilder stringBuilder = new StringBuilder(TextManager.getInstance().getString(444));
				stringBuilder.Replace("$1", num.ToString());
				mText = stringBuilder.ToString();
			}
			else
			{
				if (num != 1)
				{
					mShowText = false;
					mDone = true;
					return;
				}
				mText = TextManager.getInstance().getString(445);
			}
		}
		if (mShowText && mFrogFlyOff == null && gApp.GetBoard().GetNumLives() > 0 && mTextAlpha < 255f && mTextAlpha > 0f && !mDone)
		{
			mFrogFlyOff = new FrogFlyOff();
			Gun gun = gApp.GetBoard().GetGun();
			if (mFrogTX == -1)
			{
				mFrogTX = gun.GetCenterX();
			}
			if (mFrogTY == -1)
			{
				mFrogTY = gun.GetCenterY();
			}
			mFrogFlyOff.JumpIn(gun, mFrogTX, mFrogTY, continue_from_jump_out: false);
		}
		if (mFrogFlyOff != null)
		{
			mFrogFlyOff.Update();
			if (mFrogFlyOff.mTimer > mFrogFlyOff.mFrogJumpTime)
			{
				mLastFrogAngle = mFrogFlyOff.mFrogAngle;
				mFrogFlyOff.Dispose();
				mFrogFlyOff = null;
				mDone = true;
			}
		}
		else if (gApp.GetBoard().GetNumLives() == 0 && mTextAlpha <= 0f)
		{
			mDone = true;
		}
		if (mShowText && mTextAlpha > 0f)
		{
			mTextAlpha -= Common._M(3.2f);
			if (mTextAlpha < 0f)
			{
				mTextAlpha = 0f;
			}
		}
		if (mCloseDelay <= 0)
		{
			mCurAngle += mAngleInc;
		}
		float num2 = (float)mUpdateCount / (float)mMoveFrames;
		if (mUpdateCount < mMoveFrames && mCloseDelay <= 0)
		{
			mX += mVX;
			mY += mVY;
		}
		if (mUpdateCount % Common._M(2) == 0 && mUpdateCount < mMoveFrames && mCloseDelay <= 0)
		{
			SkullFrame skullFrame = new SkullFrame();
			mFrames.Add(skullFrame);
			skullFrame.mAngle = mCurAngle;
			if (!mDisappearing)
			{
				skullFrame.mPctOpen = 0.41f;
				skullFrame.mSize = 0.31f + 1.8900001f * num2;
			}
			else
			{
				skullFrame.mPctOpen = 0.41f;
				skullFrame.mSize = 2.2f - 1.8900001f * num2;
			}
			skullFrame.mAlpha = 128f;
			skullFrame.mX = mX;
			skullFrame.mY = mY;
		}
		if (mUpdateCount >= mMoveFrames && (mFrames.Count == 0 || !mFrames.Last().mIsFinalFrame) && !mDisappearing)
		{
			SkullFrame skullFrame2 = new SkullFrame();
			mFrames.Add(skullFrame2);
			skullFrame2.mIsFinalFrame = true;
			skullFrame2.mPctOpen = 0.41f;
			skullFrame2.mAngle = (mCurAngle = 0f);
			mAngleInc = 0f;
			skullFrame2.mSize = 2.2f;
			skullFrame2.mAlpha = 255f;
			skullFrame2.mX = mX;
			skullFrame2.mY = mY;
		}
		for (int i = 0; i < mFrames.Count; i++)
		{
			SkullFrame skullFrame3 = mFrames[i];
			if ((!skullFrame3.mIsFinalFrame || mDisappearing) && mCloseDelay <= 0)
			{
				skullFrame3.mAlpha -= Common._M(10f);
				if (skullFrame3.mAlpha <= 0f)
				{
					mFrames.RemoveAt(i);
					i--;
				}
			}
			else if (mMouthOpenDelay > 0 && --mMouthOpenDelay == 0)
			{
				mOpeningMouth = true;
			}
			else if (mOpeningMouth)
			{
				mOpeningRate -= Common._M(7E-05f);
				skullFrame3.mPctOpen += mOpeningRate;
				if (skullFrame3.mPctOpen >= 1f)
				{
					skullFrame3.mPctOpen = 1f;
					mOpeningMouth = false;
					gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_DEATH_SKULL_CHOMP));
				}
			}
			else if (mMouthOpenDelay <= 0 && !mOpeningMouth)
			{
				float num3 = 0.41f - Common._M(0.04f);
				if (skullFrame3.mPctOpen > num3)
				{
					skullFrame3.mPctOpen -= Common._M(0.12f);
				}
				if (skullFrame3.mPctOpen < num3)
				{
					skullFrame3.mPctOpen = num3;
					gApp.GetBoard().ShakeScreen(Common._M(50), Common._M1(5), Common._M2(5));
					mDestX = mStartX;
					mDestY = mStartY;
					mVX = (mDestX - mX) / (float)mMoveFrames;
					mVY = (mDestY - mY) / (float)mMoveFrames;
					mUpdateCount = 0;
					mCloseDelay = Common._M(25);
					mAngleInc = mAngleOutInc;
					mDisappearing = true;
				}
				else
				{
					mFrogClipHeight -= (int)Common._M(40f);
				}
			}
		}
	}

	public void DrawAboveFrog(Graphics g)
	{
		if (!mOpeningMouth && mMouthOpenDelay <= 0)
		{
			int num = -1;
			for (int i = 0; i < mFrames.Count; i++)
			{
				if (mFrames[i].mIsFinalFrame)
				{
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				DrawFrogItem(g, mFrames[num], 1);
			}
		}
		if (mShowText && mTextAlpha > 0f)
		{
			g.SetColor(255, 0, 0, (mTextAlpha > 255f) ? 255 : ((int)mTextAlpha));
			g.SetFont(Res.GetFontByID(ResID.FONT_SHAGEXOTICA100_STROKE));
			g.WriteString(mText, -(int)g.mTransX, (GameApp.gApp.mHeight - g.GetFont().mHeight) / 2, GameApp.gApp.mWidth, 0);
		}
		if (mFrogFlyOff != null)
		{
			mFrogFlyOff.Draw(g);
		}
	}

	public void DrawBelowFrog(Graphics g)
	{
		int num = -1;
		int i = (mDisappearing ? (mFrames.Count - 1) : 0);
		for (int num2 = ((!mDisappearing) ? 1 : (-1)); mDisappearing ? (i >= 0) : (i < mFrames.Count); i += num2)
		{
			SkullFrame skullFrame = mFrames[i];
			if (skullFrame.mIsFinalFrame)
			{
				num = i;
			}
			else
			{
				DrawFrogItem(g, skullFrame, 2);
			}
		}
		if (num != -1)
		{
			SkullFrame s = mFrames[num];
			int frog_draw_mode = ((mOpeningMouth || mMouthOpenDelay > 0) ? 2 : 0);
			DrawFrogItem(g, s, frog_draw_mode);
		}
	}

	public void SyncState(DataSync sync)
	{
		sync.SyncLong(ref mFrogTX);
		sync.SyncLong(ref mFrogTY);
		sync.SyncBoolean(ref mDone);
		sync.SyncLong(ref mFrogClipHeight);
		sync.SyncFloat(ref mLastFrogAngle);
		sync.SyncLong(ref mMoveFrames);
		sync.SyncLong(ref mUpdateCount);
		sync.SyncLong(ref mMouthOpenDelay);
		sync.SyncLong(ref mCloseDelay);
		sync.SyncBoolean(ref mOpeningMouth);
		sync.SyncBoolean(ref mDisappearing);
		sync.SyncBoolean(ref mShowText);
		sync.SyncFloat(ref mTextAlpha);
		sync.SyncFloat(ref mOpeningRate);
		sync.SyncFloat(ref mStartX);
		sync.SyncFloat(ref mStartY);
		sync.SyncFloat(ref mDestX);
		sync.SyncFloat(ref mDestY);
		sync.SyncFloat(ref mX);
		sync.SyncFloat(ref mY);
		sync.SyncFloat(ref mVX);
		sync.SyncFloat(ref mVY);
		sync.SyncFloat(ref mAngleInc);
		sync.SyncFloat(ref mAngleOutInc);
		sync.SyncFloat(ref mCurAngle);
		sync.SyncFloat(ref mStartingAngle);
		SexyFramework.Misc.Buffer buffer = sync.GetBuffer();
		if (sync.isWrite())
		{
			buffer.WriteBoolean(mFrogFlyOff != null);
			if (mFrogFlyOff != null)
			{
				mFrogFlyOff.SyncState(sync);
			}
			buffer.WriteLong(mFrames.Count);
			for (int i = 0; i < mFrames.Count; i++)
			{
				mFrames[i].SyncState(sync);
			}
			return;
		}
		mFrogFlyOff = null;
		if (buffer.ReadBoolean())
		{
			mFrogFlyOff = new FrogFlyOff();
			mFrogFlyOff.SyncState(sync);
		}
		mFrames.Clear();
		int num = (int)buffer.ReadLong();
		for (int j = 0; j < num; j++)
		{
			SkullFrame skullFrame = new SkullFrame();
			skullFrame.SyncState(sync);
			mFrames.Add(skullFrame);
		}
		GameApp gApp = GameApp.gApp;
		int num2 = gApp.GetBoard().GetNumLives() - 1;
		if (gApp.GetBoard().mLevel.mBoss != null || gApp.GetBoard().mLevel.IsFinalBossLevel())
		{
			mText = TextManager.getInstance().getString(442);
		}
		else if (gApp.GetBoard().IsCheckpointLevel() && num2 < 3)
		{
			mText = TextManager.getInstance().getString(443);
		}
		else if (num2 > 1)
		{
			StringBuilder stringBuilder = new StringBuilder(TextManager.getInstance().getString(444));
			stringBuilder.Replace("$1", num2.ToString());
			mText = stringBuilder.ToString();
		}
		else if (num2 == 1)
		{
			mText = TextManager.getInstance().getString(445);
		}
		else
		{
			mText = "";
		}
	}

	protected void DrawFrogItem(Graphics g, SkullFrame s, int frog_draw_mode)
	{
		int[] array = new int[4]
		{
			Common._M(0),
			Common._M1(2),
			Common._M2(0),
			Common._M3(0)
		};
		int[] array2 = new int[4]
		{
			Common._M(167),
			Common._M1(200),
			Common._M2(200),
			Common._M3(0)
		};
		Image[] array3 = new Image[4]
		{
			Res.GetImageByID(ResID.IMAGE_DEATHSKULL_LARGE_BOTTOM),
			Res.GetImageByID(ResID.IMAGE_DEATHSKULL_LARGE_BLACK),
			Res.GetImageByID(ResID.IMAGE_DEATHSKULL_LARGE_MIDDLE),
			Res.GetImageByID(ResID.IMAGE_DEATHSKULL_LARGE_TOP)
		};
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(255, 255, 255, (int)s.mAlpha);
		int i;
		int num;
		switch ((DrawType)frog_draw_mode)
		{
		case DrawType.Draw_BG:
			i = 0;
			num = 2;
			break;
		case DrawType.Draw_FG:
			i = 2;
			num = 4;
			break;
		default:
			i = 0;
			num = 4;
			break;
		}
		for (; i < num; i++)
		{
			mGlobalTranform.Reset();
			mGlobalTranform.Scale(s.mSize, s.mSize);
			mGlobalTranform.RotateRad(s.mAngle);
			Rect celRect = array3[i].GetCelRect(0);
			float num2 = Common._DS(array[i]);
			float num3 = Common._DS(array2[i]);
			float num4 = 0f;
			if (i == 2 && (!s.mIsFinalFrame || s.mAlpha < 254f))
			{
				int num5 = Common._DS(Common._M(0));
				float num6 = 0.59000003f;
				float num7 = 1f - (s.mPctOpen - 0.41f) / num6;
				num4 = (float)num5 * num7;
				celRect.mY = (int)num4;
				celRect.mHeight -= (int)num4;
				num3 += num4;
			}
			num2 *= s.mSize;
			num3 *= s.mSize;
			if (i == 1 || i == 2)
			{
				num3 *= s.mPctOpen;
			}
			JeffLib.Common.RotatePoint(s.mAngle, ref num2, ref num3, 0f, 0f);
			mGlobalTranform.Translate(num2 + s.mX, num3 + s.mY);
			if (g.Is3D())
			{
				g.DrawImageTransformF(array3[i], mGlobalTranform, celRect, 0f, 0f);
			}
			else
			{
				g.DrawImageTransform(array3[i], mGlobalTranform, celRect, 0f, 0f);
			}
		}
		g.SetColorizeImages(colorizeImages: false);
	}
}
