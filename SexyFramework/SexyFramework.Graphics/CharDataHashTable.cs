using System.Collections.Generic;
using System.Linq;

namespace SexyFramework.Graphics;

public class CharDataHashTable
{
	private Dictionary<char, CharData> mCharDataHash = new Dictionary<char, CharData>();

	public bool mOrderedHash;

	public CharDataHashTable()
	{
		mOrderedHash = false;
	}

	public CharData GetCharData(char inChar, bool inAllowAdd)
	{
		if (mCharDataHash.TryGetValue(inChar, out var value))
		{
			return value;
		}
		if (!inAllowAdd)
		{
			return null;
		}
		CharData charData = new CharData();
		mCharDataHash[inChar] = charData;
		charData.mChar = inChar;
		return charData;
	}

	public int CharCount()
	{
		return mCharDataHash.Count;
	}

	public CharData[] ToArray()
	{
		return mCharDataHash.Values.ToArray();
	}
}
