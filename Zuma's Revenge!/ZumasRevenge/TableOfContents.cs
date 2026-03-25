using System.Collections.Generic;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class TableOfContents : Widget, ButtonListener
{
	private enum ChallengeZonePages
	{
		ContentId_MettleOfTheMonkey = 10101,
		ContentId_RoosterRumble,
		ContentId_JackalJam,
		ContentId_MarshMadness,
		ContentId_UnderseaUndertaking,
		ContentId_SerpentScuffle,
		NUM_CHALLENGE_ZONE_PAGES
	}

	private bool mIsAwardingMedal;

	private bool mIsAwardAce;

	private float mMedalSize;

	private float mMedalAlpha;

	private int mAwardedMedal;

	private int mTimer;

	private IndexMedal[] mChallengeZoneBtns = new IndexMedal[GlobalChallenge.NUM_CHALLENGE_ZONES];

	private ChallengeMenu mChallengeMenu;

	private List<LTSmokeParticle> mSmokeParticles;

	public TableOfContents(ChallengeMenu aChallengeMenu)
	{
		mChallengeMenu = aChallengeMenu;
		for (int i = 0; i < GlobalChallenge.NUM_CHALLENGE_ZONES; i++)
		{
			mChallengeZoneBtns[i] = null;
		}
		Image imageByID = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES);
		Resize(0, 0, imageByID.GetWidth(), imageByID.GetHeight());
		mIsAwardingMedal = false;
		mMedalSize = 1f;
		mMedalAlpha = 255f;
		mAwardedMedal = -1;
		mIsAwardAce = false;
		mTimer = 0;
		mSmokeParticles = new List<LTSmokeParticle>();
	}

	public override void Dispose()
	{
		base.Dispose();
		RemoveAllWidgets(doDelete: false, recursive: true);
		for (int i = 0; i < GlobalChallenge.NUM_CHALLENGE_ZONES; i++)
		{
			if (mChallengeZoneBtns[i] != null)
			{
				mChallengeZoneBtns[i].Dispose();
			}
			mChallengeZoneBtns[i] = null;
		}
		for (int j = 0; j < mSmokeParticles.Count; j++)
		{
			mSmokeParticles[j] = null;
		}
		mSmokeParticles.Clear();
	}

	public void AwardMedal(int theZone, bool isAced)
	{
		mIsAwardingMedal = true;
		mMedalSize = 15f;
		mMedalAlpha = 0f;
		mAwardedMedal = theZone;
		mIsAwardAce = isAced;
		mTimer = 0;
		for (int i = 0; i < GlobalChallenge.NUM_CHALLENGE_ZONES; i++)
		{
			IndexMedal indexMedal = mChallengeZoneBtns[i];
			if (indexMedal != null)
			{
				indexMedal.SetVisible(isVisible: false);
				indexMedal.SetDisabled(isDisabled: true);
			}
		}
	}

	public void Init()
	{
		int[,] array = new int[6, 2]
		{
			{
				Common._DS(300),
				Common._DS(250)
			},
			{
				Common._DS(600),
				Common._DS(250)
			},
			{
				Common._DS(900),
				Common._DS(250)
			},
			{
				Common._DS(300),
				Common._DS(600)
			},
			{
				Common._DS(600),
				Common._DS(600)
			},
			{
				Common._DS(900),
				Common._DS(600)
			}
		};
		for (int i = 0; i < GlobalChallenge.NUM_CHALLENGE_ZONES; i++)
		{
			mChallengeZoneBtns[i] = new IndexMedal(mChallengeMenu.HasAcedZone(i), 10101 + i, this);
			Image image = null;
			image = ((!mChallengeMenu.HasAcedZone(i)) ? ((!mChallengeMenu.HasBeatZone(i)) ? Res.GetImageByID((ResID)(1192 + (i + 1) * 3)) : Res.GetImageByID((ResID)(1191 + (i + 1) * 3))) : Res.GetImageByID((ResID)(1190 + (i + 1) * 3)));
			mChallengeZoneBtns[i].mButtonImage = image;
			mChallengeZoneBtns[i].Resize(array[i, 0] + GameApp.gApp.GetScreenRect().mX / 2, array[i, 1], image.GetWidth(), image.GetHeight());
			mChallengeZoneBtns[i].SetVisible(isVisible: true);
			mChallengeZoneBtns[i].SetDisabled(isDisabled: false);
			mChallengeZoneBtns[i].Init();
			AddWidget(mChallengeZoneBtns[i]);
		}
	}

	public override void Update()
	{
		if (!mIsAwardingMedal || GameApp.gApp.mBambooTransition.IsInProgress())
		{
			return;
		}
		for (int i = 0; i < mSmokeParticles.Count; i++)
		{
			if (BambooTransition.UpdateSmokeParticle(mSmokeParticles[i]))
			{
				mSmokeParticles.RemoveAt(i);
				i--;
			}
		}
		MarkDirty();
		mTimer++;
		int num = Common._M(75) - mTimer;
		float num2 = 255f / (float)num;
		mMedalAlpha += num2;
		if (mMedalAlpha > 255f)
		{
			mMedalAlpha = 255f;
		}
		num2 = Common._M(15f) / (float)num;
		mMedalSize -= num2;
		if (!(mMedalSize <= 1f))
		{
			return;
		}
		if (mIsAwardAce)
		{
			GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_MINI_CROWN_IMPACT));
		}
		else
		{
			GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_ACE_MINI_CROWN_IMPACT));
		}
		GlobalChallenge.gScreenShakeTimer = Common._M(15);
		mMedalSize = 1f;
		mMedalAlpha = 255f;
		if (GameApp.gApp.mUserProfile.mDoChallengeAceCupComplete)
		{
			GameApp.gApp.mUserProfile.mDoChallengeAceCupComplete = false;
		}
		else if (GameApp.gApp.mUserProfile.mDoChallengeCupComplete)
		{
			GameApp.gApp.mUserProfile.mDoChallengeCupComplete = false;
		}
		Image image = null;
		if (mIsAwardAce)
		{
			image = Res.GetImageByID((ResID)(1190 + (mAwardedMedal + 1) * 3));
			mChallengeZoneBtns[mAwardedMedal].SetAced();
		}
		else
		{
			image = Res.GetImageByID((ResID)(1191 + (mAwardedMedal + 1) * 3));
		}
		mChallengeZoneBtns[mAwardedMedal].mButtonImage = image;
		mIsAwardingMedal = false;
		mIsAwardAce = false;
		mMedalSize = 1f;
		mMedalAlpha = 255f;
		for (int j = 0; j < 40; j++)
		{
			float x = (float)mChallengeZoneBtns[mAwardedMedal].mX + (float)mChallengeZoneBtns[mAwardedMedal].mWidth / 2f;
			float y = (float)mChallengeZoneBtns[mAwardedMedal].mY + (float)mChallengeZoneBtns[mAwardedMedal].mHeight / 2f;
			mSmokeParticles.Add(BambooTransition.SpawnSmokeParticle(x, y, fast: false, slow_fade: false));
		}
		for (int k = 0; k < GlobalChallenge.NUM_CHALLENGE_ZONES; k++)
		{
			ButtonWidget buttonWidget = mChallengeZoneBtns[k];
			if (buttonWidget != null)
			{
				buttonWidget.SetVisible(isVisible: true);
				buttonWidget.SetDisabled(isDisabled: false);
			}
		}
	}

	public override void Draw(Graphics g)
	{
		string theString = TextManager.getInstance().getString(426);
		g.SetFont(Res.GetFontByID(ResID.FONT_SHAGEXOTICA68_STROKE));
		g.SetColor(Color.White);
		float num = g.GetFont().StringWidth(theString);
		g.DrawString(theString, (int)((float)(GameApp.gApp.GetScreenRect().mX + mWidth) - num) / 2, Common._DS(150));
		float[,] array = new float[6, 2]
		{
			{
				Common._DS(5),
				Common._DS(-5)
			},
			{
				Common._DS(11),
				Common._DS(2)
			},
			{
				Common._DS(-8),
				Common._DS(5)
			},
			{
				Common._DS(-7),
				Common._DS(-1)
			},
			{
				Common._DS(0),
				Common._DS(0)
			},
			{
				Common._DS(0),
				Common._DS(0)
			}
		};
		for (int i = 0; i < GlobalChallenge.NUM_CHALLENGE_ZONES; i++)
		{
			if (mChallengeZoneBtns[i] != null)
			{
				Image imageByID = Res.GetImageByID((ResID)(1211 + i));
				float num2 = array[i, 0] + (float)mChallengeZoneBtns[i].mX - (float)((imageByID.GetWidth() - mChallengeZoneBtns[i].mButtonImage.GetWidth()) / 2);
				float num3 = array[i, 1] + (float)mChallengeZoneBtns[i].mY - (float)((imageByID.GetHeight() - mChallengeZoneBtns[i].mButtonImage.GetHeight()) / 2);
				g.DrawImage(imageByID, (int)num2, (int)num3);
				if (mIsAwardingMedal)
				{
					g.DrawImage(mChallengeZoneBtns[i].mButtonImage, mChallengeZoneBtns[i].mX, mChallengeZoneBtns[i].mY);
				}
				for (int j = 0; j < mSmokeParticles.Count; j++)
				{
					BambooTransition.DrawSmokeParticle(g, mSmokeParticles[j]);
				}
			}
		}
		if (mIsAwardingMedal)
		{
			g.PushState();
			g.ClearClipRect();
			Image image = null;
			image = ((!mIsAwardAce) ? Res.GetImageByID((ResID)(1191 + (mAwardedMedal + 1) * 3)) : Res.GetImageByID((ResID)(1190 + (mAwardedMedal + 1) * 3)));
			g.SetColor(new Color(255, 255, 255, (int)mMedalAlpha));
			g.SetColorizeImages(colorizeImages: true);
			SexyTransform2D sexyTransform2D = new SexyTransform2D(init: false);
			sexyTransform2D.Scale(mMedalSize, mMedalSize);
			sexyTransform2D.Translate((float)mChallengeZoneBtns[mAwardedMedal].mX + ((float)mChallengeZoneBtns[mAwardedMedal].mButtonImage.mWidth - (float)image.mWidth * mMedalSize) / 2f, (float)mChallengeZoneBtns[mAwardedMedal].mY + ((float)mChallengeZoneBtns[mAwardedMedal].mButtonImage.mHeight - (float)image.mHeight * mMedalSize) / 2f);
			g.DrawImageMatrix(image, sexyTransform2D, (float)image.mWidth * mMedalSize / 2f, (float)image.mHeight * mMedalSize / 2f);
			g.PopState();
		}
	}

	public virtual void ButtonDepress(int id)
	{
		if (GameApp.gApp.mBambooTransition == null || !GameApp.gApp.mBambooTransition.IsInProgress())
		{
			GameApp.gApp.PlaySample(Res.GetSoundByID(ResID.SOUND_BUTTON2));
			switch (id)
			{
			case 10101:
				mChallengeMenu.mChallengeScrollWidget.SetPageHorizontal(1, animated: true);
				break;
			case 10102:
				mChallengeMenu.mChallengeScrollWidget.SetPageHorizontal(2, animated: true);
				break;
			case 10103:
				mChallengeMenu.mChallengeScrollWidget.SetPageHorizontal(3, animated: true);
				break;
			case 10104:
				mChallengeMenu.mChallengeScrollWidget.SetPageHorizontal(4, animated: true);
				break;
			case 10105:
				mChallengeMenu.mChallengeScrollWidget.SetPageHorizontal(5, animated: true);
				break;
			case 10106:
				mChallengeMenu.mChallengeScrollWidget.SetPageHorizontal(6, animated: true);
				break;
			}
		}
	}

	public virtual void ButtonDownTick(int x)
	{
	}

	public virtual void ButtonMouseEnter(int x)
	{
	}

	public virtual void ButtonMouseLeave(int x)
	{
	}

	public virtual void ButtonMouseMove(int x, int y, int z)
	{
	}

	public virtual void ButtonPress(int id)
	{
	}

	public virtual void ButtonPress(int id, int count)
	{
	}
}
