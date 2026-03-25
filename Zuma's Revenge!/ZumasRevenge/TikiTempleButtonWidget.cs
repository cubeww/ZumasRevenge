using JeffLib;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public class TikiTempleButtonWidget : ExtraSexyButton
{
	public TikiTemple mTikiTemple;

	public TikiTempleButtonWidget(int theId, TikiTemple theListener)
		: base(theId, theListener)
	{
		mUsesAnimators = false;
		mTikiTemple = theListener;
	}

	public override void Draw(Graphics g)
	{
		base.Draw(g);
	}
}
