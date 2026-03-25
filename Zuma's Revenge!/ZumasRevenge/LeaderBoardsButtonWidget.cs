using JeffLib;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public class LeaderBoardsButtonWidget : ExtraSexyButton
{
	public LeaderBoards mLeaderBoards;

	public LeaderBoardsButtonWidget(int theId, LeaderBoards theListener)
		: base(theId, theListener)
	{
		mUsesAnimators = false;
		mLeaderBoards = theListener;
	}

	public override void Draw(Graphics g)
	{
		base.Draw(g);
	}
}
