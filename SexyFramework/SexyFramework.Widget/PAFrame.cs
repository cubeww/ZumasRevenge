using System.Collections.Generic;

namespace SexyFramework.Widget;

public class PAFrame
{
	public List<PAObjectPos> mFrameObjectPosVector = new List<PAObjectPos>();

	public bool mHasStop;

	public List<PACommand> mCommandVector = new List<PACommand>();

	public PAFrame()
	{
		mHasStop = false;
	}
}
