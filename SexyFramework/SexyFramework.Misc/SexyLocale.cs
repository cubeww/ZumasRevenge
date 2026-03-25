namespace SexyFramework.Misc;

public static class SexyLocale
{
	public static string gGrouping = "\\3";

	public static string gThousandSep = ",";

	private static char CHAR_MAX = '\u007f';

	public static void SetSeperators(string theGrouping, string theSeperator)
	{
		gGrouping = theGrouping;
		gThousandSep = theSeperator;
	}

	public static void SetLocale(string theLocale)
	{
	}

	public static string StringToUpper(string theString)
	{
		return theString.ToUpper();
	}

	public static string StringToLower(string theString)
	{
		return theString.ToLower();
	}

	public static bool isalnum(char theChar)
	{
		return char.IsNumber(theChar);
	}

	public static string CommaSeparate(int theValue)
	{
		if (theValue < 0)
		{
			return "-" + UCommaSeparate((uint)(-theValue));
		}
		return UCommaSeparate((uint)theValue);
	}

	public static string UCommaSeparate(uint theValue)
	{
		char[] array = new char[64];
		if (theValue == 0)
		{
			return "0";
		}
		string text = gGrouping;
		int num = 64;
		int num2 = 0;
		if (text[num2] != CHAR_MAX && text[num2] > '\0')
		{
			char c = gThousandSep[0];
			int num3 = 0;
			while (theValue != 0)
			{
				array[--num] = (char)(48 + theValue % 10);
				theValue /= 10;
				if (theValue != 0 && ++num3 == text[num2])
				{
					array[--num] = c;
					num3 = 0;
					if (text[num2 + 1] > '\0')
					{
						num2++;
					}
				}
			}
		}
		else
		{
			while (theValue != 0)
			{
				array[--num] = (char)(48 + theValue % 10);
				theValue /= 10;
			}
		}
		return new string(array, num, 64 - num);
	}

	public static string CommaSeparate64(long theValue)
	{
		if (theValue < 0)
		{
			return "-" + UCommaSeparate64((ulong)(-theValue));
		}
		return UCommaSeparate64((ulong)theValue);
	}

	public static string UCommaSeparate64(ulong theValue)
	{
		return "";
	}
}
