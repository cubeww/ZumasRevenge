using JeffLib;
using SexyFramework.Graphics;

namespace ZumasRevenge;

public class MapGenericButton : ExtraSexyButton
{
	public MapScreen mMapScreen;

	public MapGenericButton(int theId, MapScreen theListener)
		: base(theId, theListener)
	{
		mUsesAnimators = false;
		mMapScreen = theListener;
	}

	public override void Draw(Graphics g)
	{
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(mMapScreen.mAlpha);
		base.Draw(g);
	}
}
