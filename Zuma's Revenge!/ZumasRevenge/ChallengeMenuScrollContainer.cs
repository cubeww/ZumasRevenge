using System.Collections.Generic;
using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class ChallengeMenuScrollContainer : Widget
{
	private ChallengeMenu mChallengeMenu;

	private List<ZoneFrame> mZoneFrames = new List<ZoneFrame>();

	private Image IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES;

	private Image IMAGE_UI_CHALLENGESCREEN_DIVIDER;

	private TableOfContents mTableOfContents;

	public ChallengeMenuScrollContainer(ChallengeMenu aChallengeMenu)
	{
		mChallengeMenu = aChallengeMenu;
		IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES);
		IMAGE_UI_CHALLENGESCREEN_DIVIDER = Res.GetImageByID(ResID.IMAGE_UI_CHALLENGESCREEN_DIVIDER);
		mTableOfContents = new TableOfContents(mChallengeMenu);
		mTableOfContents.Init();
		mTableOfContents.Resize(0, 0, IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetWidth(), IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetHeight());
		AddWidget(mTableOfContents);
		int num = 4095;
		int num2 = mTableOfContents.mWidth;
		int num3 = 0;
		for (int i = 0; i < GlobalChallenge.NUM_CHALLENGE_ZONES; i++)
		{
			ZoneFrame item = new ZoneFrame(mChallengeMenu, num3++, num);
			mZoneFrames.Add(item);
			num += num;
			mZoneFrames[i].Move(num2, 0);
			num2 += mZoneFrames[i].mWidth;
			AddWidget(mZoneFrames[i]);
		}
		Resize(0, 0, IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetWidth() * (mZoneFrames.Count + 1), IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetHeight());
	}

	public override void Dispose()
	{
		for (int i = 0; i < mZoneFrames.Count; i++)
		{
			RemoveWidget(mZoneFrames[i]);
			if (mZoneFrames[i] != null)
			{
				mZoneFrames[i].Dispose();
			}
			mZoneFrames[i] = null;
		}
		mZoneFrames.Clear();
		RemoveWidget(mTableOfContents);
		if (mTableOfContents != null)
		{
			mTableOfContents.Dispose();
		}
		mTableOfContents = null;
	}

	public void RehupChallengeButtons()
	{
		if (mZoneFrames.Count == 0)
		{
			return;
		}
		for (int i = 0; i < mZoneFrames.Count; i++)
		{
			if (mZoneFrames[i] != null)
			{
				mZoneFrames[i].RehupChallengeButtons();
			}
		}
	}

	public override void Draw(Graphics g)
	{
		int num = NumPages() - 1;
		int num2 = IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetWidth() + GameApp.gApp.GetScreenRect().mX / 2;
		int theY = (IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetHeight() - IMAGE_UI_CHALLENGESCREEN_DIVIDER.GetHeight()) / 2;
		for (int i = 0; i < num; i++)
		{
			g.DrawImage(IMAGE_UI_CHALLENGESCREEN_DIVIDER, num2 - IMAGE_UI_CHALLENGESCREEN_DIVIDER.GetWidth() / 2, theY);
			num2 += IMAGE_UI_CHALLENGESCREEN_TAPESTRY_LEAVES.GetWidth();
		}
	}

	public int NumPages()
	{
		return mZoneFrames.Count + 1;
	}

	public void AwardMedal(int theZone, bool isAce)
	{
		mTableOfContents.AwardMedal(theZone, isAce);
	}

	public void PreloadButtonImage(int theZone)
	{
		mZoneFrames[theZone]?.PreLoadButtonsImage();
	}
}
