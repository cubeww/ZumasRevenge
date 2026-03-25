using System;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class Checkpoint : Widget, ButtonListener, IDisposable
{
	public enum ButtonId
	{
		Button_Continue,
		Button_MainMenu,
		Max_Buttons
	}

	protected ButtonWidget[] mButtons = new ButtonWidget[2];

	protected string mZone;

	protected string mBossName;

	protected string mPostcardGroupName = "";

	protected int mScore;

	protected float mAlpha;

	protected float mSize;

	protected int mState;

	protected int mPostCardX;

	protected int mPostCardY;

	public int mLevelNum;

	public bool mFromGameOver;

	public bool mDone;

	public bool mContinuePressed;

	public bool mShowMap;

	private SexyTransform2D mTransform = new SexyTransform2D(init: false);

	public Checkpoint(Level l, int score, bool game_over)
	{
		mScore = score;
		mFromGameOver = game_over;
		mDone = false;
		mContinuePressed = false;
		mPostcardGroupName = "";
		mState = 1;
		mAlpha = 0f;
		mSize = Common._DS(Common._M(8f));
		mClip = false;
		mShowMap = false;
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_CHECKPOINT_POSTCARD_BACK);
		mPostCardX = (GameApp.gApp.GetScreenRect().mWidth - imageByID.mWidth) / 2 + GameApp.gApp.mWideScreenXOffset;
		mPostCardY = (GameApp.gApp.GetScreenRect().mHeight - imageByID.mHeight) / 2;
		if (mFromGameOver)
		{
			int levelIndex = GameApp.gApp.GetLevelMgr().GetLevelIndex(l.mId);
			levelIndex = ((l.mNum > 5) ? (levelIndex - (l.mNum - 6)) : (levelIndex - (l.mNum - 1)));
			Level levelByIndex = GameApp.gApp.GetLevelMgr().GetLevelByIndex(levelIndex);
			mZone = LevelMgr.GetZoneName(levelByIndex.mZone - 1);
			mLevelNum = (levelByIndex.mZone - 1) * 10 + levelByIndex.mNum;
			_ = (GameApp.gApp.GetScreenRect().mWidth + Common._S(Common._M(160)) - Res.GetImageByID(ResID.IMAGE_GUI_CHECKPOINT_POSTCARDTEXT_BASETEXT).mWidth) / 2;
			_ = (GameApp.gApp.GetScreenRect().mHeight - Res.GetImageByID(ResID.IMAGE_GUI_CHECKPOINT_POSTCARDTEXT_BASETEXT).mHeight) / 2;
			mButtons[0] = new ButtonWidget(0, this);
			mButtons[0].mDoFinger = true;
			mButtons[0].mButtonImage = Res.GetImageByID(ResID.IMAGE_GUI_CHECKPOINT_POSTCARDTEXT_CONTINUE_BUTTON);
			mButtons[0].mDownImage = Res.GetImageByID(ResID.IMAGE_GUI_CHECKPOINT_POSTCARDTEXT_CONTINUE_BUTTON_CLICK);
			mButtons[0].mOverImage = Res.GetImageByID(ResID.IMAGE_GUI_CHECKPOINT_POSTCARDTEXT_CONTINUE_BUTTON_CLICK);
			mButtons[0].mDisabled = true;
			mButtons[0].Resize(mPostCardX + Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GUI_CHECKPOINT_POSTCARDTEXT_CONTINUE_BUTTON)), mPostCardY + Common._DS(Res.GetOffsetYByID(ResID.IMAGE_GUI_CHECKPOINT_POSTCARDTEXT_CONTINUE_BUTTON)), mButtons[0].mOverImage.mWidth, mButtons[0].mOverImage.mHeight);
			AddWidget(mButtons[0]);
			mButtons[1] = new ButtonWidget(1, this);
			mButtons[1].mDoFinger = true;
			mButtons[1].mButtonImage = Res.GetImageByID(ResID.IMAGE_GUI_CHECKPOINT_POSTCARDTEXT_MM_BUTTON);
			mButtons[1].mDownImage = Res.GetImageByID(ResID.IMAGE_GUI_CHECKPOINT_POSTCARDTEXT_MM_BUTTON_CLICK);
			mButtons[1].mOverImage = Res.GetImageByID(ResID.IMAGE_GUI_CHECKPOINT_POSTCARDTEXT_MM_BUTTON_CLICK);
			mButtons[1].mDisabled = true;
			mButtons[1].Resize(mPostCardX + Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GUI_CHECKPOINT_POSTCARDTEXT_MM_BUTTON_CLICK)), mPostCardY + Common._DS(Res.GetOffsetYByID(ResID.IMAGE_GUI_CHECKPOINT_POSTCARDTEXT_MM_BUTTON_CLICK)), mButtons[1].mOverImage.mWidth, mButtons[1].mOverImage.mHeight);
			AddWidget(mButtons[1]);
		}
		else
		{
			mZone = LevelMgr.GetZoneName(l.mZone);
			mLevelNum = (l.mZone - 1) * 10 + l.mNum;
			if (l.mBoss != null)
			{
				mBossName = "\"" + l.mBoss.mName + "\"";
			}
		}
	}

	public override void Dispose()
	{
		base.RemoveAllWidgets(doDelete: true, recursive: false);
	}

	public override void Update()
	{
		int num = Common._M(75);
		mUpdateCnt++;
		if (mState == 1)
		{
			float num2 = 128f / (float)num;
			mAlpha += num2;
			if (mAlpha > 128f)
			{
				mAlpha = 128f;
			}
			num2 = Common._M(7f) / (float)num;
			mSize -= num2;
			if (mSize < 1f)
			{
				mSize = 1f;
			}
			if (mUpdateCnt < num)
			{
				return;
			}
			if (mFromGameOver)
			{
				for (int i = 0; i < 2; i++)
				{
					if (mButtons[i] != null)
					{
						mButtons[i].mDisabled = false;
					}
				}
			}
			mState = 0;
		}
		else if (mState == -1)
		{
			float num3 = 128f / (float)num;
			mAlpha -= num3;
			if (mAlpha <= 0f)
			{
				mAlpha = 0f;
				mDone = true;
				mState = 0;
			}
		}
	}

	public override void Draw(Graphics g)
	{
		int num = (int)(mAlpha * 2f);
		if (num > 255)
		{
			num = 255;
		}
		else if (num < 0)
		{
			num = 0;
		}
		if (num != 255)
		{
			g.SetColorizeImages(colorizeImages: true);
		}
		g.SetColor(255, 255, 255, num);
		GameApp gApp = GameApp.gApp;
		if (!mFromGameOver)
		{
			Font fontByID = Res.GetFontByID(ResID.FONT_SHAGLOUNGE45_GAUNTLET);
			Font fontByID2 = Res.GetFontByID(ResID.FONT_SHAGLOUNGE28_STROKE);
			int theX = gApp.GetScreenRect().mWidth / 2;
			int num2 = mHeight / 2;
			g.SetFont(fontByID);
			g.SetColor(255, 255, 255, num);
			string theString = TextManager.getInstance().getString(432);
			g.WriteString(theString, 0, num2 - g.GetFont().GetHeight() - Common._S(Common._M(-30)), mWidth, 0);
			int num3 = Common._S(Common._M(20));
			g.SetFont(fontByID2);
			g.SetColor(Common._M(240), Common._M1(200), Common._M2(0), num);
			g.WriteString(mZone, theX, num2 + Common._S(Common._M(35)), mWidth - num3 * 2, -1);
			g.WriteString((mLevelNum < int.MaxValue) ? (TextManager.getInstance().getString(683) + " " + mLevelNum) : mBossName, 0, num2 + Common._S(Common._M(15)), mWidth - num3 * 2, 0);
			g.WriteString(SexyFramework.Common.CommaSeperate(mScore), theX, num2 + Common._S(Common._M(30)), mWidth - num3 * 2, 1);
			g.DrawString(TextManager.getInstance().getString(433), theX, num2 + Common._S(75));
			g.SetFont(fontByID2);
			g.SetColor(Common._M(255), Common._M1(0), Common._M2(0), num);
			g.WriteString(TextManager.getInstance().getString(434), 0, num2 + Common._S(Common._M(60)), mWidth, 0);
		}
		else
		{
			Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_CHECKPOINT_POSTCARD_BACK);
			Image imageByID2 = Res.GetImageByID(ResID.IMAGE_GUI_CHECKPOINT_POSTCARDTEXT_BASETEXT);
			g.DrawImage(imageByID, mPostCardX, mPostCardY);
			g.DrawImage(imageByID2, mPostCardX + Common._DS(Res.GetOffsetXByID(ResID.IMAGE_GUI_CHECKPOINT_POSTCARDTEXT_BASETEXT)), mPostCardY + Common._DS(Res.GetOffsetYByID(ResID.IMAGE_GUI_CHECKPOINT_POSTCARDTEXT_BASETEXT)));
			int levelIndex = gApp.GetLevelMgr().GetLevelIndex(gApp.mBoard.mLevel.mId);
			if (gApp.mBoard.mLevel.mNum <= 5)
			{
				levelIndex -= gApp.mBoard.mLevel.mNum - 1;
			}
			else
			{
				levelIndex -= gApp.mBoard.mLevel.mNum - 6;
			}
			_ = gApp.mBoard.mLevel.mZone;
			int theLevelNum = mLevelNum - 1;
			g.PushState();
			Image levelThumbnail = gApp.GetLevelThumbnail(theLevelNum);
			float num4 = 2f;
			int value = 1225;
			int value2 = 332;
			int num5 = -27;
			int value3 = (int)(num4 * (float)levelThumbnail.mWidth);
			int value4 = (int)(num4 * (float)levelThumbnail.mHeight);
			if (GameApp.mGameRes != 768)
			{
				g.DrawImage(levelThumbnail, Common._DS(value) + gApp.GetScreenRect().mX, Common._DS(value2), Common._DS(value3), Common._DS(value4));
			}
			else
			{
				g.DrawImage(levelThumbnail, Common._DS(value) + gApp.GetScreenRect().mX + num5, Common._DS(value2), Common._DS(value3), Common._DS(value4));
			}
			Font fontByID3 = Res.GetFontByID(ResID.FONT_CHECKPOINT_CURSIVE);
			g.SetFont(fontByID3);
			g.SetColor(Common._M(0), Common._M1(0), Common._M2(0), num);
			int theX2 = Common._S(Common._M(480));
			int num6 = Common._S(Common._M(320));
			g.DrawString(mZone, theX2, num6);
			g.DrawString(TextManager.getInstance().getString(683) + " " + mLevelNum, theX2, num6 + Common._S(Common._M(25)));
			g.DrawString(SexyFramework.Common.CommaSeperate(mScore) + " pts", theX2, num6 + Common._S(Common._M(50)));
		}
		g.SetColorizeImages(colorizeImages: false);
	}

	public void ButtonDepress(int id)
	{
		if (GameApp.gApp.mBambooTransition == null || !GameApp.gApp.mBambooTransition.IsInProgress())
		{
			switch (id)
			{
			case 1:
				GameApp.gApp.mBambooTransition.mTransitionDelegate = GameApp.gApp.DoDeferredEndGame;
				GameApp.gApp.ToggleBambooTransition();
				break;
			case 0:
				mContinuePressed = true;
				mDone = true;
				break;
			}
		}
	}

	public override void MouseDown(int x, int y, int cc)
	{
		if (mState == 0 && !mFromGameOver)
		{
			mUpdateCnt = 0;
			mState = -1;
		}
	}

	public void Disable(bool d)
	{
		SetDisabled(d);
		SetVisible(!d);
		for (int i = 0; i < 2; i++)
		{
			if (mButtons[i] != null)
			{
				mButtons[i].SetDisabled(d);
				mButtons[i].SetVisible(!d);
			}
		}
	}

	public virtual void PreDraw(Graphics g)
	{
		g.SetColor(0, 0, 0, (int)mAlpha);
		g.FillRect(0, 0, mWidth, mHeight);
		Graphics3D graphics3D = g.Get3D();
		if (!MathUtils._eq(mSize, 1f) && graphics3D != null)
		{
			mTransform.Translate(-GameApp.gApp.mWidth / 2, -GameApp.gApp.mHeight / 2);
			mTransform.Scale(mSize, mSize);
			mTransform.Translate(GameApp.gApp.mWidth / 2, GameApp.gApp.mHeight / 2);
			graphics3D.PushTransform(mTransform);
		}
	}

	public override void DrawAll(ModalFlags theFlags, Graphics g)
	{
		PreDraw(g);
		Draw(g);
		for (int i = 0; i < 2; i++)
		{
			if (mButtons[i] != null)
			{
				g.Translate(mButtons[i].mX, mButtons[i].mY);
				mButtons[i].Draw(g);
				g.Translate(-mButtons[i].mX, -mButtons[i].mY);
			}
		}
		PostDraw(g);
	}

	public virtual void PostDraw(Graphics g)
	{
		Graphics3D graphics3D = g.Get3D();
		if (!MathUtils._eq(mSize, 1f))
		{
			graphics3D?.PopTransform();
		}
	}

	public void ButtonDownTick(int theId)
	{
	}

	public void ButtonMouseEnter(int theId)
	{
	}

	public void ButtonMouseLeave(int theId)
	{
	}

	public void ButtonMouseMove(int theId, int theX, int theY)
	{
	}

	public void ButtonPress(int theId)
	{
	}

	public void ButtonPress(int theId, int theClickCount)
	{
	}
}
