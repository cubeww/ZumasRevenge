using System.Collections.Generic;
using System.Globalization;
using JeffLib;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Resource;

namespace ZumasRevenge;

public class Credits
{
	public class CreditEntry
	{
		public string mTitle;

		public string mName;

		public Image mImage;

		public Font mTitleFont;

		public Font mNameFont;

		public int mXCenterOff;

		public int mYOff;

		public int mInitialY;

		public int mSpaceAfterTitle;

		public int mSpaceAfterName;

		public int mSpaceAfterImage;

		public Color mTitleColor;

		public Color mNameColor;

		public bool mAdvMode;

		public bool mAlwaysShow;

		public float mImgAlpha;

		public bool mDoPolaroid;

		public bool mXFlip;

		public CreditEntry()
		{
			mImage = null;
			mTitleFont = null;
			mXFlip = false;
			mImgAlpha = 0f;
			mDoPolaroid = true;
			mNameFont = null;
			mYOff = 0;
			mSpaceAfterTitle = 0;
			mSpaceAfterName = 0;
			mSpaceAfterImage = 0;
			mAdvMode = true;
			mAlwaysShow = true;
			mXCenterOff = 0;
			mInitialY = 0;
			mTitle = "";
			mName = "";
		}
	}

	public List<CreditEntry> mEntries;

	public float mYScrollAmt;

	public float mAlpha;

	public float mFFAlpha;

	public Font mTitleFont;

	public Font mNameFont;

	public int mSpaceAfterTitle;

	public int mSpaceAfterName;

	public int mSpaceAfterImage;

	public Color mTitleColor;

	public Color mNameColor;

	public int mRoll = -12;

	public float mScrollSpeed;

	public int mInitialDelay;

	public bool mSpeedUp;

	public bool mFromMainMenu;

	public bool mTapDown;

	private Font FONT_SHAGLOUNGE28_SHADOW;

	private Image IMAGE_CREDITS_IMAGES_POLAROID;

	public Credits(bool isFromMainMenu)
	{
		mYScrollAmt = 0f;
		mAlpha = 0f;
		mTitleFont = null;
		mNameFont = null;
		mSpaceAfterTitle = 0;
		mSpaceAfterName = 0;
		mSpaceAfterImage = 0;
		mScrollSpeed = 0f;
		mFFAlpha = 0f;
		mInitialDelay = 0;
		mSpeedUp = false;
		mFromMainMenu = isFromMainMenu;
		mEntries = new List<CreditEntry>();
		FONT_SHAGLOUNGE28_SHADOW = Res.GetFontByID(ResID.FONT_SHAGLOUNGE28_SHADOW);
		IMAGE_CREDITS_IMAGES_POLAROID = Res.GetImageByID(ResID.IMAGE_CREDITS_IMAGES_POLAROID);
	}

	public virtual void Dispose()
	{
		if (GameApp.gApp.mResourceManager.IsGroupLoaded("Credits"))
		{
			GameApp.gApp.mResourceManager.DeleteResources("Credits");
		}
	}

	private bool GetAttribute(XMLElement elem, string theName, ref string theValue)
	{
		if (elem.GetAttributeMap().ContainsKey(theName))
		{
			theValue = elem.GetAttributeMap()[theName];
			return true;
		}
		return false;
	}

	public void Init(bool advmode)
	{
		if (!GameApp.gApp.mResourceManager.IsGroupLoaded("Credits"))
		{
			GameApp.gApp.mResourceManager.LoadResources("Credits");
		}
		XMLParser xMLParser = new XMLParser();
		string languageSuffix = Localization.GetLanguageSuffix(Localization.GetCurrentLanguage());
		string theFilename = "properties/credits/credits" + languageSuffix + ".xml";
		xMLParser.OpenFile(theFilename);
		XMLElement xMLElement = new XMLElement();
		while (!xMLParser.HasFailed() && xMLParser.NextElement(xMLElement))
		{
			if (xMLElement.mType != XMLElement.XMLElementType.TYPE_START)
			{
				continue;
			}
			if (xMLElement.mValue.ToString() != _S("Credits"))
			{
				break;
			}
			while (xMLParser.NextElement(xMLElement))
			{
				if (xMLElement.mType != XMLElement.XMLElementType.TYPE_START)
				{
					continue;
				}
				if (StrEquals(xMLElement.mValue.ToString(), _S("Defaults")))
				{
					string theValue = "";
					if (GetAttribute(xMLElement, _S("SpaceAfterTitle"), ref theValue))
					{
						mSpaceAfterTitle = StrToInt(theValue);
					}
					if (GetAttribute(xMLElement, _S("SpaceAfterName"), ref theValue))
					{
						mSpaceAfterName = StrToInt(theValue);
					}
					if (GetAttribute(xMLElement, _S("SpaceAfterPic"), ref theValue))
					{
						mSpaceAfterImage = StrToInt(theValue);
					}
					if (GetAttribute(xMLElement, _S("ScrollSpeed"), ref theValue))
					{
						mScrollSpeed = Common._DS(StrToFloat(theValue));
					}
					if (GetAttribute(xMLElement, _S("TitleColor"), ref theValue))
					{
						mTitleColor = new Color((int)JeffLib.Common.StrToHex(ToString(theValue)));
					}
					if (GetAttribute(xMLElement, _S("NameColor"), ref theValue))
					{
						mNameColor = new Color((int)JeffLib.Common.StrToHex(ToString(theValue)));
					}
					if (GetAttribute(xMLElement, _S("TitleFont"), ref theValue))
					{
						mTitleFont = GameApp.gApp.mResourceManager.LoadFont(theValue);
					}
					if (GetAttribute(xMLElement, _S("NameFont"), ref theValue))
					{
						mNameFont = GameApp.gApp.mResourceManager.LoadFont(theValue);
					}
				}
				else if (StrEquals(xMLElement.mValue.ToString(), _S("Text")))
				{
					string theValue2 = "";
					CreditEntry creditEntry = new CreditEntry();
					creditEntry.mSpaceAfterTitle = mSpaceAfterTitle;
					creditEntry.mSpaceAfterName = mSpaceAfterName;
					creditEntry.mSpaceAfterImage = mSpaceAfterImage;
					creditEntry.mTitleColor = mTitleColor;
					creditEntry.mNameColor = mNameColor;
					creditEntry.mTitleFont = mTitleFont;
					creditEntry.mNameFont = mNameFont;
					if (GetAttribute(xMLElement, _S("mode"), ref theValue2))
					{
						creditEntry.mAlwaysShow = false;
						creditEntry.mAdvMode = StrEquals(theValue2, _S("adventure"));
					}
					if (creditEntry.mAdvMode == advmode || creditEntry.mAlwaysShow)
					{
						if (GetAttribute(xMLElement, _S("Title"), ref theValue2))
						{
							creditEntry.mTitle = theValue2;
						}
						if (GetAttribute(xMLElement, _S("Name"), ref theValue2))
						{
							creditEntry.mName = theValue2;
						}
						if (GetAttribute(xMLElement, _S("TitleFont"), ref theValue2))
						{
							creditEntry.mTitleFont = GameApp.gApp.mResourceManager.LoadFont(theValue2);
						}
						if (GetAttribute(xMLElement, _S("NameFont"), ref theValue2))
						{
							creditEntry.mNameFont = GameApp.gApp.mResourceManager.LoadFont(theValue2);
						}
						if (GetAttribute(xMLElement, _S("YOff"), ref theValue2))
						{
							creditEntry.mYOff = StrToInt(theValue2);
						}
						if (GetAttribute(xMLElement, _S("XCenterOff"), ref theValue2))
						{
							creditEntry.mXCenterOff = Common._S(StrToInt(theValue2));
						}
						if (GetAttribute(xMLElement, _S("TitleColor"), ref theValue2))
						{
							creditEntry.mTitleColor = new Color((int)JeffLib.Common.StrToHex(ToString(theValue2)));
						}
						if (GetAttribute(xMLElement, _S("NameColor"), ref theValue2))
						{
							creditEntry.mNameColor = new Color((int)JeffLib.Common.StrToHex(ToString(theValue2)));
						}
						if (GetAttribute(xMLElement, _S("SpaceAfterTitle"), ref theValue2))
						{
							creditEntry.mSpaceAfterTitle = StrToInt(theValue2);
						}
						if (GetAttribute(xMLElement, _S("SpaceAfterName"), ref theValue2))
						{
							creditEntry.mSpaceAfterName = StrToInt(theValue2);
						}
						if (GetAttribute(xMLElement, _S("SpaceAfterPic"), ref theValue2))
						{
							creditEntry.mSpaceAfterImage = StrToInt(theValue2);
						}
						mEntries.Add(creditEntry);
					}
				}
				else
				{
					if (!StrEquals(xMLElement.mValue.ToString(), _S("Image")))
					{
						continue;
					}
					string theValue3 = "";
					CreditEntry creditEntry2 = new CreditEntry();
					if (GetAttribute(xMLElement, _S("resid"), ref theValue3))
					{
						creditEntry2.mImage = GameApp.gApp.mResourceManager.LoadImage(theValue3).GetImage();
					}
					if (GetAttribute(xMLElement, _S("YOff"), ref theValue3))
					{
						creditEntry2.mYOff = StrToInt(theValue3);
					}
					if (GetAttribute(xMLElement, _S("xflip"), ref theValue3))
					{
						creditEntry2.mXFlip = StrToBool(theValue3);
					}
					if (GetAttribute(xMLElement, _S("polaroid"), ref theValue3))
					{
						creditEntry2.mDoPolaroid = StrToBool(theValue3);
						if (!creditEntry2.mDoPolaroid)
						{
							creditEntry2.mImgAlpha = 255f;
						}
					}
					if (GetAttribute(xMLElement, _S("SpaceAfterPic"), ref theValue3))
					{
						creditEntry2.mSpaceAfterImage = StrToInt(theValue3);
					}
					if (GetAttribute(xMLElement, _S("x"), ref theValue3))
					{
						if (StrEquals(theValue3, _S("center")))
						{
							creditEntry2.mXCenterOff = -creditEntry2.mImage.mWidth / 2;
						}
						else
						{
							creditEntry2.mXCenterOff = -GameApp.gApp.mWidth / 2 + Common._S(StrToInt(theValue3));
						}
					}
					mEntries.Add(creditEntry2);
				}
			}
		}
		xMLParser.CloseFile();
		int num = GameApp.gApp.mHeight;
		for (int i = 0; i < mEntries.Count; i++)
		{
			CreditEntry creditEntry3 = mEntries[i];
			num = (creditEntry3.mInitialY = num + Common._S(creditEntry3.mYOff));
			if (creditEntry3.mImage == null)
			{
				if (creditEntry3.mTitle.Length > 0)
				{
					num += creditEntry3.mTitleFont.GetHeight() + Common._S(creditEntry3.mSpaceAfterTitle);
				}
				if (creditEntry3.mName.Length > 0)
				{
					num += creditEntry3.mNameFont.GetHeight() + Common._S(creditEntry3.mSpaceAfterName);
				}
			}
			else
			{
				num += Common._S(creditEntry3.mSpaceAfterImage) + creditEntry3.mImage.mHeight;
			}
		}
	}

	public bool AtEnd()
	{
		CreditEntry creditEntry = mEntries[mEntries.Count - 1];
		if (creditEntry.mImage == null || !((float)creditEntry.mInitialY + mYScrollAmt <= (float)(GameApp.gApp.mHeight / 2 - creditEntry.mImage.mHeight / 2 - Common._DS(Common._M(200)))))
		{
			if (creditEntry.mImage == null)
			{
				return (float)creditEntry.mInitialY + mYScrollAmt <= (float)(GameApp.gApp.mHeight / 2 - creditEntry.mTitleFont.mHeight / 2);
			}
			return false;
		}
		return true;
	}

	public void Update()
	{
		if (GameApp.gApp.IsHardwareBackButtonPressed() && !mFromMainMenu)
		{
			ProcessHardwareBackButton();
		}
		if (mAlpha < 255f)
		{
			mAlpha += Common._M(8f);
			if (mAlpha > 255f)
			{
				mAlpha = 255f;
			}
			return;
		}
		for (int i = 0; i < mEntries.Count; i++)
		{
			CreditEntry creditEntry = mEntries[i];
			int num = (int)((float)creditEntry.mInitialY + mYScrollAmt);
			if (creditEntry.mImage != null && creditEntry.mDoPolaroid && num <= Common._DS(Common._M(900)))
			{
				if (creditEntry.mImgAlpha < 255f)
				{
					creditEntry.mImgAlpha += Common._M(0.5f);
				}
				if (creditEntry.mImgAlpha > 255f)
				{
					creditEntry.mImgAlpha = 255f;
				}
			}
		}
		if (!AtEnd())
		{
			if (++mInitialDelay >= Common._M(100))
			{
				mYScrollAmt -= mScrollSpeed * (float)((!mSpeedUp) ? 1 : Common._M(4));
			}
			if (mInitialDelay >= Common._M(300))
			{
				mFFAlpha += Common._M(2f) * (float)((!mSpeedUp) ? 1 : Common._M1(4));
				if (mFFAlpha > 255f)
				{
					mFFAlpha = 255f;
				}
			}
		}
		else
		{
			mFFAlpha -= Common._M(2f);
			if (mFFAlpha < 0f)
			{
				mFFAlpha = 0f;
			}
		}
	}

	public void Draw(Graphics g)
	{
		g.SetColor(new Color(0, 0, 0, (int)mAlpha));
		g.FillRect(Common._S(-80), 0, GameApp.gApp.mWidth + Common._S(160), GameApp.gApp.mHeight);
		for (int i = 0; i < mEntries.Count; i++)
		{
			CreditEntry creditEntry = mEntries[i];
			int num = (int)((float)creditEntry.mInitialY + mYScrollAmt);
			if (AtEnd() && i == mEntries.Count - 1)
			{
				num = ((creditEntry.mImage != null) ? ((GameApp.gApp.mHeight - creditEntry.mImage.mHeight) / 2 - Common._DS(Common._M(200))) : ((GameApp.gApp.mHeight - creditEntry.mTitleFont.mHeight) / 2));
			}
			g.PushState();
			if ((GameApp.gApp.mUserProfile.mAdvModeVars.mHighestZoneBeat >= 6 || !mFromMainMenu) && creditEntry.mImage != null)
			{
				if (num > -350 && num < 700)
				{
					g.PushState();
					float num2 = (creditEntry.mXFlip ? Common._DS(Common._M(mRoll)) : 0);
					if (creditEntry.mDoPolaroid)
					{
						g.DrawImageMirror(IMAGE_CREDITS_IMAGES_POLAROID, (int)((float)(GameApp.gApp.mWidth / 2 + creditEntry.mXCenterOff - Common._DS(Common._M(60))) - num2), num - Common._DS(Common._M1(36)), creditEntry.mXFlip);
						g.SetColorizeImages(colorizeImages: true);
						g.SetColor(new Color(255, 255, 255, (int)creditEntry.mImgAlpha));
					}
					g.DrawImageMirror(creditEntry.mImage, GameApp.gApp.mWidth / 2 + creditEntry.mXCenterOff, num, creditEntry.mXFlip);
					g.PopState();
				}
			}
			else if (num > -100 && num < 700)
			{
				int theAlpha = 255;
				if (creditEntry.mTitle.Length > 0)
				{
					g.SetFont(creditEntry.mTitleFont);
					g.SetColor(new Color(creditEntry.mTitleColor.mRed, creditEntry.mTitleColor.mGreen, creditEntry.mTitleColor.mBlue, theAlpha));
					g.WriteString(creditEntry.mTitle, creditEntry.mXCenterOff, num + creditEntry.mTitleFont.GetAscent(), GameApp.gApp.mWidth, 0);
					num += creditEntry.mSpaceAfterTitle + creditEntry.mTitleFont.GetHeight();
				}
				if (creditEntry.mName.Length > 0)
				{
					g.SetFont(creditEntry.mNameFont);
					g.SetColor(new Color(creditEntry.mNameColor.mRed, creditEntry.mNameColor.mGreen, creditEntry.mNameColor.mBlue, theAlpha));
					g.WriteString(creditEntry.mName, creditEntry.mXCenterOff, num + creditEntry.mNameFont.GetAscent(), GameApp.gApp.mWidth, 0);
				}
			}
			g.PopState();
		}
		g.SetFont(FONT_SHAGLOUNGE28_SHADOW);
		if (!AtEnd())
		{
			g.SetColor(new Color(Common._M(255), Common._M1(255), Common._M2(255), (int)(mFFAlpha * Common._M3(0.5f))));
			g.DrawString(TextManager.getInstance().getString(435), Common._DS(Common._M(750)), Common._DS(Common._M1(1176)));
		}
		else
		{
			g.SetColor(new Color(Common._M(255), Common._M1(255), Common._M2(255), 200));
			g.DrawString(TextManager.getInstance().getString(433), Common._DS(Common._M(750)), Common._DS(Common._M1(1176)));
		}
	}

	public void ProcessHardwareBackButton()
	{
		GameApp.gApp.ReturnFromCredits();
		GameApp.gApp.OnHardwareBackButtonPressProcessed();
	}

	private float StrToFloat(string str)
	{
		if (str.Length == 0)
		{
			return 0f;
		}
		return float.Parse(str, NumberStyles.Float, CultureInfo.InvariantCulture);
	}

	private int StrToInt(string str)
	{
		if (str.Length == 0)
		{
			return 0;
		}
		return int.Parse(str);
	}

	private bool StrToBool(string str)
	{
		if (str.Length == 0)
		{
			return false;
		}
		return bool.Parse(str);
	}

	private string ToString(string str)
	{
		return str;
	}

	private string _S(string str)
	{
		return str;
	}

	private int sexyatoi(string str)
	{
		return StrToInt(str);
	}

	private float sexyatof(string str)
	{
		return StrToFloat(str);
	}

	private bool StrEquals(string str, string cmp)
	{
		return str == cmp;
	}

	private string StringToUpper(string str)
	{
		return str.ToUpper();
	}

	private string StringToLower(string str)
	{
		return str.ToLower();
	}
}
