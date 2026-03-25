using SexyFramework;
using SexyFramework.Misc;
using SexyFramework.Resource;

namespace ZumasRevenge;

public class BXMLParser
{
	private Buffer mSexyBuffer;

	protected string UnpackString()
	{
		string text = "";
		for (int num = mSexyBuffer.ReadInt32(); num != 0; num = mSexyBuffer.ReadInt32())
		{
			text += (char)num;
		}
		return text;
	}

	protected short UnpackShort()
	{
		return mSexyBuffer.ReadShort();
	}

	public BXMLParser()
	{
		mSexyBuffer = null;
	}

	public virtual void Dispose()
	{
		mSexyBuffer = null;
	}

	public virtual bool OpenFile(string filename)
	{
		PFILE pFILE = new PFILE(filename, "rb");
		if (!pFILE.Open())
		{
			return false;
		}
		byte[] data = pFILE.GetData();
		mSexyBuffer = new Buffer();
		mSexyBuffer.SetData(data, data.Length);
		return true;
	}

	public virtual bool OpenStream(string filename)
	{
		mSexyBuffer = new Buffer();
		if (!GlobalMembers.gSexyApp.ReadBufferFromStream(filename, ref mSexyBuffer))
		{
			return false;
		}
		return true;
	}

	public virtual bool OpenBuffer(Buffer buffer)
	{
		mSexyBuffer = buffer;
		return true;
	}

	public virtual bool NextElement(ref BXMLElement theElement)
	{
		if (mSexyBuffer.AtEnd())
		{
			return false;
		}
		theElement.mType = 0;
		theElement.mValue = "";
		theElement.mAttributes.Clear();
		theElement.mType = UnpackShort();
		theElement.mValue = UnpackString();
		int num = UnpackShort();
		while (num-- > 0)
		{
			string key = UnpackString().ToLower();
			string value = UnpackString();
			theElement.mAttributes[key] = value;
		}
		return true;
	}

	public static bool CompileXML(string theSrcName, string theSrcDestName)
	{
		return true;
	}

	public bool HasFailed()
	{
		return false;
	}
}
