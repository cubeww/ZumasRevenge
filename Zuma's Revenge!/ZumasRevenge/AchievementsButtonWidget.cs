using JeffLib;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public class AchievementsButtonWidget : ExtraSexyButton
{
	public Achievements mAchievements;

	public AchievementsButtonWidget(int theId, Achievements theListener)
		: base(theId, theListener)
	{
		mUsesAnimators = false;
		mAchievements = theListener;
	}

	public override void Draw(Graphics g)
	{
		base.Draw(g);
	}
}
