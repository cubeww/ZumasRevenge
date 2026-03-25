using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace ZumasRevenge;

public class ZumaSlideBoxButton : Widget
{
	public ZumaSlideBox mSlideBox;

	public ZumaSlideBoxButton(ZumaSlideBox theSlideBox)
	{
		mSlideBox = theSlideBox;
	}

	public override void Draw(Graphics g)
	{
		Image imageByID = Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BOX_MAINMENU_GREEN_LIGHT);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BOX_MAINMENU_ON);
		Image imageByID3 = Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BOX_MAINMENU_RED_LIGHT);
		Image imageByID4 = Res.GetImageByID(ResID.IMAGE_GUI_DIALOG_BOX_MAINMENU_OFF);
		if (mSlideBox.IsOn())
		{
			g.DrawImage(imageByID, imageByID.GetWidth(), 0);
			g.DrawImage(imageByID2, imageByID.GetWidth() + (imageByID.GetWidth() - imageByID2.GetWidth()) / 2, (imageByID.GetHeight() - imageByID2.GetHeight()) / 2);
		}
		else
		{
			g.DrawImage(imageByID3, imageByID3.GetWidth(), 0);
			g.DrawImage(imageByID4, imageByID3.GetWidth() + (imageByID3.GetWidth() - imageByID4.GetWidth()) / 2, (imageByID3.GetHeight() - imageByID4.GetHeight()) / 2);
		}
	}
}
