using System.Collections.Generic;

namespace SexyFramework.Resource;

public class PropertiesParser
{
	public SexyAppBase mApp;

	public string mError = "";

	public bool mHasFailed;

	private XMLParser mXMLParser;

	private XMLElement mXMLElement = new XMLElement();

	protected void Fail(string theErrorText)
	{
		if (!mHasFailed)
		{
			mHasFailed = true;
			int currentLineNum = mXMLParser.GetCurrentLineNum();
			mError = theErrorText;
			if (currentLineNum > 0)
			{
				mError = mError + " on Line " + currentLineNum;
			}
			if (mXMLParser.GetFileName().Length <= 0)
			{
				mError = mError + " in File " + mXMLParser.GetFileName();
			}
		}
	}

	protected bool ParseSingleElement(string aString)
	{
		aString = "";
		while (true)
		{
			if (!mXMLParser.NextElement(mXMLElement))
			{
				return false;
			}
			if (mXMLElement.mType == XMLElement.XMLElementType.TYPE_START)
			{
				Fail(string.Concat("Unexpected Section: '", mXMLElement.mValue, "'"));
				return false;
			}
			if (mXMLElement.mType == XMLElement.XMLElementType.TYPE_ELEMENT)
			{
				aString = mXMLElement.mValue.ToString();
			}
			else if (mXMLElement.mType == XMLElement.XMLElementType.TYPE_END)
			{
				break;
			}
		}
		return true;
	}

	protected bool ParseStringArray(List<string> theStringVector)
	{
		theStringVector.Clear();
		while (true)
		{
			if (!mXMLParser.NextElement(mXMLElement))
			{
				return false;
			}
			if (mXMLElement.mType == XMLElement.XMLElementType.TYPE_START)
			{
				if (!(mXMLElement.mValue.ToString() == "String"))
				{
					Fail(string.Concat("Invalid Section '", mXMLElement.mValue, "'"));
					return false;
				}
				string text = "";
				if (!ParseSingleElement(text))
				{
					return false;
				}
				theStringVector.Add(text);
			}
			else
			{
				if (mXMLElement.mType == XMLElement.XMLElementType.TYPE_ELEMENT)
				{
					Fail(string.Concat("Element Not Expected '", mXMLElement.mValue, "'"));
					return false;
				}
				if (mXMLElement.mType == XMLElement.XMLElementType.TYPE_END)
				{
					break;
				}
			}
		}
		return true;
	}

	protected bool ParseProperties()
	{
		while (true)
		{
			if (!mXMLParser.NextElement(mXMLElement))
			{
				return false;
			}
			if (mXMLElement.mType == XMLElement.XMLElementType.TYPE_START)
			{
				if (mXMLElement.mValue.ToString() == "String")
				{
					string text = "";
					if (!ParseSingleElement(text))
					{
						return false;
					}
					string attribute = mXMLElement.GetAttribute("id");
					mApp.SetString(attribute, text);
				}
				else if (mXMLElement.mValue.ToString() == "StringArray")
				{
					List<string> list = new List<string>();
					if (!ParseStringArray(list))
					{
						return false;
					}
					string attribute2 = mXMLElement.GetAttribute("id");
					mApp.mStringVectorProperties[attribute2] = list;
				}
				else if (mXMLElement.mValue.ToString() == "Boolean")
				{
					string text2 = "";
					if (!ParseSingleElement(text2))
					{
						return false;
					}
					text2 = text2.ToUpper();
					bool flag = false;
					switch (text2)
					{
					case "1":
					case "YES":
					case "ON":
					case "TRUE":
						flag = true;
						break;
					case "0":
					case "NO":
					case "OFF":
					case "FALSE":
						flag = false;
						break;
					default:
						Fail("Invalid Boolean Value: '" + text2 + "'");
						return false;
					}
					string attribute3 = mXMLElement.GetAttribute("id");
					mApp.SetBoolean(attribute3, flag);
				}
				else if (mXMLElement.mValue.ToString() == "Integer")
				{
					string text3 = "";
					if (!ParseSingleElement(text3))
					{
						return false;
					}
					int theIntVal = 0;
					if (!Common.StringToInt(text3, ref theIntVal))
					{
						Fail("Invalid Integer Value: '" + text3 + "'");
						return false;
					}
					string attribute4 = mXMLElement.GetAttribute("id");
					mApp.SetInteger(attribute4, theIntVal);
				}
				else
				{
					if (!(mXMLElement.mValue.ToString() == "Double"))
					{
						Fail(string.Concat("Invalid Section '", mXMLElement.mValue, "'"));
						return false;
					}
					string text4 = "";
					if (!ParseSingleElement(text4))
					{
						return false;
					}
					double theDouble = 0.0;
					if (!Common.StringToDouble(text4, ref theDouble))
					{
						Fail("Invalid Double Value: '" + text4 + "'");
						return false;
					}
					string attribute5 = mXMLElement.GetAttribute("id");
					mApp.SetDouble(attribute5, theDouble);
				}
			}
			else
			{
				if (mXMLElement.mType == XMLElement.XMLElementType.TYPE_ELEMENT)
				{
					Fail(string.Concat("Element Not Expected '", mXMLElement.mValue, "'"));
					return false;
				}
				if (mXMLElement.mType == XMLElement.XMLElementType.TYPE_END)
				{
					break;
				}
			}
		}
		return true;
	}

	protected bool DoParseProperties()
	{
		if (!mXMLParser.HasFailed())
		{
			while (true)
			{
				XMLElement xMLElement = new XMLElement();
				if (!mXMLParser.NextElement(xMLElement))
				{
					break;
				}
				if (xMLElement.mType == XMLElement.XMLElementType.TYPE_START)
				{
					if (!(xMLElement.mValue.ToString() == "Properties"))
					{
						Fail(string.Concat("Invalid Section '", xMLElement.mValue, "'"));
						break;
					}
					if (!ParseProperties())
					{
						break;
					}
				}
				else if (xMLElement.mType == XMLElement.XMLElementType.TYPE_ELEMENT)
				{
					Fail(string.Concat("Element Not Expected '", xMLElement.mValue, "'"));
					break;
				}
			}
		}
		if (mXMLParser.HasFailed())
		{
			Fail(mXMLParser.GetErrorText());
		}
		mXMLParser = null;
		return !mHasFailed;
	}

	public PropertiesParser(SexyAppBase theApp)
	{
		mApp = theApp;
		mHasFailed = false;
	}

	public virtual void Dispose()
	{
	}

	public bool ParsePropertiesFile(string theFilename)
	{
		mXMLParser = new XMLParser();
		mXMLParser.OpenFile(theFilename);
		return DoParseProperties();
	}

	public bool ParsePropertiesBuffer(byte[] theBuffer)
	{
		mXMLParser = new XMLParser();
		mXMLParser.SetBytes(theBuffer);
		return DoParseProperties();
	}

	public string GetErrorText()
	{
		return mError;
	}
}
