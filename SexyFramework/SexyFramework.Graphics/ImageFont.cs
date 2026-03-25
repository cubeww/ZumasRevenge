using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SexyFramework.Misc;
using SexyFramework.Resource;

namespace SexyFramework.Graphics;

public class ImageFont : Font
{
	public static bool mAlphaCorrectionEnabled;

	public static bool mOrderedHash;

	public FontData mFontData;

	public int mPointSize;

	public List<string> mTagVector = new List<string>();

	public bool mActivateAllLayers;

	public bool mActiveListValid;

	public List<ActiveFontLayer> mActiveLayerList = new List<ActiveFontLayer>();

	public double mScale;

	public bool mForceScaledImagesWhite;

	public bool mWantAlphaCorrection;

	public MemoryImage mFontImage;

	protected Dictionary<char, Dictionary<char, int>> mCachedCharPair = new Dictionary<char, Dictionary<char, int>>();

	public static void EnableAlphaCorrection()
	{
		EnableAlphaCorrection(alphaCorrect: true);
	}

	public static void EnableAlphaCorrection(bool alphaCorrect)
	{
		mAlphaCorrectionEnabled = alphaCorrect;
	}

	public static void SetOrderedHashing()
	{
		SetOrderedHashing(orderedHash: true);
	}

	public static void SetOrderedHashing(bool orderedHash)
	{
		mOrderedHash = orderedHash;
	}

	public static bool CheckCache(string theSrcFile, string theAltData)
	{
		return false;
	}

	public static bool SetCacheUpToDate(string theSrcFile, string theAltData)
	{
		return false;
	}

	public static ImageFont ReadFromCache(string theSrcFile, string theAltData)
	{
		return null;
	}

	private ImageFont()
	{
		new AutoCrit(GlobalMembers.gSexyAppBase.mImageSetCritSect);
		GlobalMembers.gSexyAppBase.mImageFontSet.Add(this);
		mFontImage = null;
		mScale = 1.0;
		mWantAlphaCorrection = false;
		mFontData = new FontData();
		mFontData.Ref();
	}

	public ImageFont(string fileName, byte[] buffer)
	{
		mFontImage = null;
		mScale = 1.0;
		mWantAlphaCorrection = false;
		mFontData = new FontData();
		mFontData.Ref();
		if (0 == 0)
		{
			mFontData.Load(buffer);
			mPointSize = mFontData.mDefaultPointSize;
			mActivateAllLayers = false;
			mActiveListValid = true;
			mForceScaledImagesWhite = false;
			GenerateActiveFontLayers();
		}
	}

	public ImageFont(SexyAppBase theSexyApp, string theFontDescFileName, string theImagePathPrefix)
	{
		new AutoCrit(GlobalMembers.gSexyAppBase.mImageSetCritSect);
		GlobalMembers.gSexyAppBase.mImageFontSet.Add(this);
		mFontImage = null;
		mScale = 1.0;
		mWantAlphaCorrection = false;
		mFontData = new FontData();
		mFontData.Ref();
		mFontData.mImagePathPrefix = theImagePathPrefix;
		string text = theFontDescFileName + ".cfw2";
		string text2 = "cached\\" + text;
		string theFileName = text2;
		SexyFramework.Misc.Buffer theBuffer = new SexyFramework.Misc.Buffer();
		bool flag = false;
		if (theSexyApp.ReadBufferFromStream(text, ref theBuffer) && theBuffer.GetDataLen() >= 16)
		{
			flag = true;
		}
		else if (theSexyApp.ReadBufferFromStream(text2, ref theBuffer) && theBuffer.GetDataLen() >= 16)
		{
			flag = true;
		}
		else if (theSexyApp.ReadBufferFromStream(theFileName, ref theBuffer) && theBuffer.GetDataLen() >= 16)
		{
			flag = true;
		}
		bool flag2 = false;
		if (flag)
		{
			if (theSexyApp.mResStreamsManager != null && theSexyApp.mResStreamsManager.IsInitialized())
			{
				flag2 = true;
			}
			else
			{
				SerializeRead(theBuffer.GetDataPtr(), theBuffer.GetDataLen() - 16, 16);
				flag2 = true;
			}
		}
		if (!flag2)
		{
			mFontData.Load(theSexyApp, theFontDescFileName);
			mPointSize = mFontData.mDefaultPointSize;
			mActivateAllLayers = false;
			GenerateActiveFontLayers();
			mActiveListValid = true;
			mForceScaledImagesWhite = false;
			_ = theSexyApp.mWriteFontCacheDir;
		}
	}

	public ImageFont(Image theFontImage)
	{
		new AutoCrit(GlobalMembers.gSexyAppBase.mImageSetCritSect);
		GlobalMembers.gSexyAppBase.mImageFontSet.Add(this);
		mScale = 1.0;
		mWantAlphaCorrection = false;
		mFontData = new FontData();
		mFontData.Ref();
		mFontData.mInitialized = true;
		mPointSize = mFontData.mDefaultPointSize;
		mActiveListValid = false;
		mForceScaledImagesWhite = false;
		mActivateAllLayers = false;
		mFontData.mFontLayerList.AddLast(new FontLayer(mFontData));
		FontLayer value = mFontData.mFontLayerList.Last.Value;
		mFontData.mFontLayerMap.Add("", value);
		mFontImage = (MemoryImage)theFontImage;
		value.mImage.mUnsharedImage = mFontImage;
		value.mDefaultHeight = value.mImage.GetImage().GetHeight();
		value.mAscent = value.mImage.GetImage().GetHeight();
	}

	public ImageFont(ImageFont theImageFont)
		: base(theImageFont)
	{
		mScale = theImageFont.mScale;
		mFontData = theImageFont.mFontData;
		mPointSize = theImageFont.mPointSize;
		mTagVector = theImageFont.mTagVector;
		mActiveListValid = theImageFont.mActiveListValid;
		mForceScaledImagesWhite = theImageFont.mForceScaledImagesWhite;
		mWantAlphaCorrection = theImageFont.mWantAlphaCorrection;
		mActivateAllLayers = theImageFont.mActivateAllLayers;
		mFontImage = theImageFont.mFontImage;
		new AutoCrit(GlobalMembers.gSexyAppBase.mImageSetCritSect);
		GlobalMembers.gSexyAppBase.mImageFontSet.Add(this);
		mFontData.Ref();
		if (mActiveListValid)
		{
			mActiveLayerList = theImageFont.mActiveLayerList;
		}
	}

	public ImageFont(Image theFontImage, string theFontDescFileName)
	{
		new AutoCrit(GlobalMembers.gSexyAppBase.mImageSetCritSect);
		GlobalMembers.gSexyAppBase.mImageFontSet.Add(this);
		mScale = 1.0;
		mFontImage = null;
		mFontData = new FontData();
		mFontData.Ref();
		mFontData.LoadLegacy(theFontImage, theFontDescFileName);
		mPointSize = mFontData.mDefaultPointSize;
		mActivateAllLayers = false;
		GenerateActiveFontLayers();
		mActiveListValid = true;
	}

	public override void Dispose()
	{
		new AutoCrit(GlobalMembers.gSexyAppBase.mImageSetCritSect);
		GlobalMembers.gSexyAppBase.mImageFontSet.Remove(this);
		mFontData.DeRef();
		base.Dispose();
	}

	public override Font Duplicate()
	{
		return new ImageFont(this);
	}

	public virtual void GenerateActiveFontLayers()
	{
		if (!mFontData.mInitialized)
		{
			return;
		}
		mActiveLayerList.Clear();
		mAscent = 0;
		mAscentPadding = 0;
		mHeight = 0;
		mLineSpacingOffset = 0;
		LinkedList<FontLayer>.Enumerator enumerator = mFontData.mFontLayerList.GetEnumerator();
		bool flag = true;
		while (enumerator.MoveNext())
		{
			FontLayer current = enumerator.Current;
			if (mPointSize < current.mMinPointSize || (mPointSize > current.mMaxPointSize && current.mMaxPointSize != -1))
			{
				continue;
			}
			bool flag2 = true;
			for (int i = 0; i < current.mRequiredTags.Count; i++)
			{
				if (mTagVector.IndexOf(current.mRequiredTags[i]) == -1)
				{
					flag2 = false;
				}
			}
			for (int i = 0; i < mTagVector.Count; i++)
			{
				if (current.mExcludedTags.IndexOf(mTagVector[i]) != -1)
				{
					flag2 = false;
				}
			}
			if (flag2 | mActivateAllLayers)
			{
				mActiveLayerList.Add(new ActiveFontLayer());
				ActiveFontLayer activeFontLayer = mActiveLayerList.Last();
				activeFontLayer.mBaseFontLayer = current;
				activeFontLayer.mUseAlphaCorrection = mWantAlphaCorrection & current.mImageIsWhite;
				double num = 1.0;
				double num2 = mScale;
				if (mScale == 1.0 && (current.mPointSize == 0 || mPointSize == current.mPointSize))
				{
					activeFontLayer.mScaledImages[7] = current.mImage;
					if (mFontImage != null)
					{
						activeFontLayer.mScaledImages[7].mUnsharedImage = mFontImage;
					}
					int num3 = current.mCharDataHashTable.CharCount();
					CharData[] array = current.mCharDataHashTable.ToArray();
					for (int j = 0; j < num3; j++)
					{
						activeFontLayer.mScaledCharImageRects.Add((char)array[j].mChar, array[j].mImageRect);
					}
				}
				else
				{
					if (current.mPointSize != 0)
					{
						num = current.mPointSize;
						num2 = (double)mPointSize * mScale;
					}
					MemoryImage memoryImage = new MemoryImage();
					int num4 = 0;
					bool flag3 = true;
					int num5 = 0;
					int num6 = 0;
					int num7 = current.mCharDataHashTable.CharCount();
					CharData[] array2 = current.mCharDataHashTable.ToArray();
					for (int k = 0; k < num7; k++)
					{
						Rect mImageRect = array2[k].mImageRect;
						int mY = array2[k].mOffset.mY;
						int num8 = mY + mImageRect.mHeight;
						num5 = Math.Min(mY, num5);
						num6 = Math.Max(num8, num6);
						if (num5 != mY || num6 != num8)
						{
							flag3 = false;
						}
						num4 += mImageRect.mWidth + 2;
					}
					if (!flag3)
					{
						MemoryImage memoryImage2 = new MemoryImage();
						memoryImage2.Create(num4, num6 - num5);
						Graphics graphics = new Graphics(memoryImage2);
						num4 = 0;
						num7 = current.mCharDataHashTable.CharCount();
						array2 = current.mCharDataHashTable.ToArray();
						for (int l = 0; l < num7; l++)
						{
							Rect mImageRect2 = array2[l].mImageRect;
							if (current.mImage.GetImage() != null)
							{
								graphics.DrawImage(current.mImage.GetImage(), num4, array2[l].mOffset.mY - num5, mImageRect2);
							}
							array2[l].mOffset.mY = num5;
							array2[l].mOffset.mX--;
							num4 += new Rect(num4, 0, mImageRect2.mWidth + 2, num6 - num5).mWidth;
						}
						current.mImage.mUnsharedImage = memoryImage2;
						current.mImage.mOwnsUnshared = true;
						graphics.ClearRenderContext();
					}
					num4 = 0;
					int num9 = 0;
					num7 = current.mCharDataHashTable.CharCount();
					array2 = current.mCharDataHashTable.ToArray();
					for (int m = 0; m < num7; m++)
					{
						Rect mImageRect3 = array2[m].mImageRect;
						int num10 = (int)Math.Floor((double)array2[m].mOffset.mX * num2 / (double)(float)num);
						int num11 = (int)Math.Ceiling((double)(array2[m].mOffset.mX + mImageRect3.mWidth) * num2 / (double)(float)num);
						int theWidth = Math.Max(0, num11 - num10 - 1);
						int num12 = (int)Math.Floor((double)array2[m].mOffset.mY * num2 / (double)(float)num);
						int num13 = (int)Math.Ceiling((double)(array2[m].mOffset.mY + mImageRect3.mHeight) * num2 / (double)(float)num);
						int theHeight = Math.Max(0, num13 - num12 - 1);
						Rect value = new Rect(num4, 0, theWidth, theHeight);
						if (value.mHeight > num9)
						{
							num9 = value.mHeight;
						}
						activeFontLayer.mScaledCharImageRects.Add((char)array2[m].mChar, value);
						num4 += value.mWidth;
					}
					activeFontLayer.mScaledImages[7].mUnsharedImage = memoryImage;
					activeFontLayer.mScaledImages[7].mOwnsUnshared = true;
					memoryImage.Create(num4, num9);
					Graphics graphics2 = new Graphics(memoryImage);
					num7 = current.mCharDataHashTable.CharCount();
					array2 = current.mCharDataHashTable.ToArray();
					for (int n = 0; n < num7; n++)
					{
						if (current.mImage.GetImage() != null)
						{
							graphics2.DrawImage(current.mImage.GetImage(), activeFontLayer.mScaledCharImageRects[(char)array2[n].mChar], array2[n].mImageRect);
						}
					}
					if (mForceScaledImagesWhite)
					{
						int num14 = memoryImage.mWidth * memoryImage.mHeight;
						uint[] bits = memoryImage.GetBits();
						for (int num15 = 0; num15 < num14; num15++)
						{
							bits[num15] |= 0xFFFFFF;
						}
					}
					memoryImage.AddImageFlags(128u);
					memoryImage.Palletize();
					graphics2.ClearRenderContext();
				}
				int num16 = (((double)current.mAscent * num2 / (double)(float)num >= 0.0) ? ((int)((double)current.mAscent * num2 / (double)(float)num + 0.501)) : ((int)((double)current.mAscent * num2 / (double)(float)num - 0.501)));
				if (num16 > mAscent)
				{
					mAscent = num16;
				}
				if (current.mHeight != 0)
				{
					int num17 = (((double)current.mHeight * num2 / (double)(float)num >= 0.0) ? ((int)((double)current.mHeight * num2 / (double)(float)num + 0.501)) : ((int)((double)current.mHeight * num2 / (double)(float)num - 0.501)));
					if (num17 > mHeight)
					{
						mHeight = num17;
					}
				}
				else
				{
					int num18 = (((double)current.mDefaultHeight * num2 / (double)(float)num >= 0.0) ? ((int)((double)current.mDefaultHeight * num2 / (double)(float)num + 0.501)) : ((int)((double)current.mDefaultHeight * num2 / (double)(float)num - 0.501)));
					if (num18 > mHeight)
					{
						mHeight = num18;
					}
				}
				int num19 = (((double)current.mAscentPadding * num2 / (double)(float)num >= 0.0) ? ((int)((double)current.mAscentPadding * num2 / (double)(float)num + 0.501)) : ((int)((double)current.mAscentPadding * num2 / (double)(float)num - 0.501)));
				if (flag || num19 < mAscentPadding)
				{
					mAscentPadding = num19;
				}
				int num20 = (((double)current.mLineSpacingOffset * num2 / (double)(float)num >= 0.0) ? ((int)((double)current.mLineSpacingOffset * num2 / (double)(float)num + 0.501)) : ((int)((double)current.mLineSpacingOffset * num2 / (double)(float)num - 0.501)));
				if (flag || num20 > mLineSpacingOffset)
				{
					mLineSpacingOffset = num20;
				}
			}
			flag = false;
		}
	}

	public virtual void DrawStringEx(Graphics g, int theX, int theY, string theString, Color theColor, Rect theClipRect, LinkedList<Rect> theDrawnAreas, ref int theWidth)
	{
		new AutoCrit(GlobalMembers.gSexyAppBase.mImageSetCritSect);
		theDrawnAreas?.Clear();
		if (!mFontData.mInitialized)
		{
			theWidth = 0;
			return;
		}
		Prepare();
		bool colorizeImages = g.GetColorizeImages();
		g.SetColorizeImages(colorizeImages: true);
		int num = theX;
		int num2 = 0;
		for (int i = 0; i < theString.Length; i++)
		{
			char mappedChar = GetMappedChar(theString[i]);
			char c = '\0';
			if (i < theString.Length - 1)
			{
				c = GetMappedChar(theString[i + 1]);
			}
			int num3 = num;
			for (int j = 0; j < mActiveLayerList.Count; j++)
			{
				ActiveFontLayer activeFontLayer = mActiveLayerList[j];
				CharData charData = activeFontLayer.mBaseFontLayer.GetCharData(mappedChar);
				int num4 = num;
				int num5 = activeFontLayer.mBaseFontLayer.mPointSize;
				double num6 = mScale;
				if (num5 != 0)
				{
					num6 *= (double)mPointSize / (double)num5;
				}
				int num7;
				int num8;
				int num9;
				int num10;
				if (num6 == 1.0)
				{
					num7 = num4 + activeFontLayer.mBaseFontLayer.mOffset.mX + charData.mOffset.mX;
					num8 = theY - (activeFontLayer.mBaseFontLayer.mAscent - activeFontLayer.mBaseFontLayer.mOffset.mY - charData.mOffset.mY);
					num9 = charData.mWidth;
					if (c != 0)
					{
						num10 = activeFontLayer.mBaseFontLayer.mSpacing;
						if (charData.mKerningCount != 0)
						{
							int mKerningCount = charData.mKerningCount;
							for (int k = 0; k < mKerningCount; k++)
							{
								_ = activeFontLayer.mBaseFontLayer.mKerningData[k].mChar;
							}
						}
					}
					else
					{
						num10 = 0;
					}
				}
				else
				{
					num7 = num4 + (int)Math.Floor((double)(activeFontLayer.mBaseFontLayer.mOffset.mX + charData.mOffset.mX) * num6);
					num8 = theY - (int)Math.Floor((double)(activeFontLayer.mBaseFontLayer.mAscent - activeFontLayer.mBaseFontLayer.mOffset.mY - charData.mOffset.mY) * num6);
					num9 = (int)((double)charData.mWidth * num6);
					if (c != 0)
					{
						num10 = activeFontLayer.mBaseFontLayer.mSpacing;
						if (charData.mKerningCount != 0)
						{
							int mKerningCount2 = charData.mKerningCount;
							for (int l = 0; l < mKerningCount2; l++)
							{
								_ = activeFontLayer.mBaseFontLayer.mKerningData[l].mChar;
							}
						}
					}
					else
					{
						num10 = 0;
					}
				}
				Color mColor = default(Color);
				if (activeFontLayer.mColorStack.Count == 0)
				{
					mColor.mRed = Math.Min(theColor.mRed * activeFontLayer.mBaseFontLayer.mColorMult.mRed / 255 + activeFontLayer.mBaseFontLayer.mColorAdd.mRed, 255);
					mColor.mGreen = Math.Min(theColor.mGreen * activeFontLayer.mBaseFontLayer.mColorMult.mGreen / 255 + activeFontLayer.mBaseFontLayer.mColorAdd.mGreen, 255);
					mColor.mBlue = Math.Min(theColor.mBlue * activeFontLayer.mBaseFontLayer.mColorMult.mBlue / 255 + activeFontLayer.mBaseFontLayer.mColorAdd.mBlue, 255);
					mColor.mAlpha = Math.Min(theColor.mAlpha * activeFontLayer.mBaseFontLayer.mColorMult.mAlpha / 255 + activeFontLayer.mBaseFontLayer.mColorAdd.mAlpha, 255);
				}
				else
				{
					Color color = activeFontLayer.mColorStack[activeFontLayer.mColorStack.Count - 1];
					mColor.mRed = Math.Min(theColor.mRed * activeFontLayer.mBaseFontLayer.mColorMult.mRed * color.mRed / 65025 + activeFontLayer.mBaseFontLayer.mColorAdd.mRed * color.mRed / 255, 255);
					mColor.mGreen = Math.Min(theColor.mGreen * activeFontLayer.mBaseFontLayer.mColorMult.mGreen * color.mGreen / 65025 + activeFontLayer.mBaseFontLayer.mColorAdd.mGreen * color.mGreen / 255, 255);
					mColor.mBlue = Math.Min(theColor.mBlue * activeFontLayer.mBaseFontLayer.mColorMult.mBlue * color.mBlue / 65025 + activeFontLayer.mBaseFontLayer.mColorAdd.mBlue * color.mBlue / 255, 255);
					mColor.mAlpha = Math.Min(theColor.mAlpha * activeFontLayer.mBaseFontLayer.mColorMult.mAlpha * color.mAlpha / 65025 + activeFontLayer.mBaseFontLayer.mColorAdd.mAlpha * color.mAlpha / 255, 255);
				}
				int num11 = activeFontLayer.mBaseFontLayer.mBaseOrder + charData.mOrder;
				if (num2 >= 1024)
				{
					break;
				}
				RenderCommand renderCommand = GlobalImageFont.GetRenderCommandPool()[num2++];
				renderCommand.mFontLayer = activeFontLayer;
				renderCommand.mColor = mColor;
				renderCommand.mDest[0] = num7;
				renderCommand.mDest[1] = num8;
				Rect rect = activeFontLayer.mScaledCharImageRects[mappedChar];
				renderCommand.mSrc[0] = rect.mX;
				renderCommand.mSrc[1] = rect.mY;
				renderCommand.mSrc[2] = rect.mWidth;
				renderCommand.mSrc[3] = rect.mHeight;
				renderCommand.mMode = activeFontLayer.mBaseFontLayer.mDrawMode;
				int orderedZ = Math.Min(Math.Max(num11 + 128, 0), 255);
				GlobalImageFont.AddRenderCommand(renderCommand, orderedZ);
				if (theDrawnAreas != null)
				{
					Rect value = new Rect(num7, num8, rect.mWidth, rect.mHeight);
					theDrawnAreas.AddLast(value);
				}
				num4 += num9 + num10;
				if (num4 > num3)
				{
					num3 = num4;
				}
			}
			num = num3;
		}
		theWidth = num - theX;
		Color color2 = g.GetColor();
		GlobalImageFont.DrawAllRenderCommand(g, mAlphaCorrectionEnabled);
		GlobalImageFont.ClearRenderCommand();
		g.SetColor(color2);
		g.SetColorizeImages(colorizeImages);
	}

	public char GetMappedChar(char theChar)
	{
		if (mFontData.mCharMap.TryGetValue(theChar, out var value))
		{
			return value;
		}
		return theChar;
	}

	public override ImageFont AsImageFont()
	{
		return this;
	}

	public override int CharWidth(char theChar)
	{
		return CharWidthKern(theChar, '\0');
	}

	public override int CharWidthKern(char theChar, char thePrevChar)
	{
		Prepare();
		int value = 0;
		double num = (double)mPointSize * mScale;
		theChar = GetMappedChar(theChar);
		if (thePrevChar != 0)
		{
			thePrevChar = GetMappedChar(thePrevChar);
		}
		if (mCachedCharPair.TryGetValue(theChar, out var value2))
		{
			if (value2.TryGetValue(thePrevChar, out value))
			{
				return value;
			}
		}
		else
		{
			mCachedCharPair.Add(theChar, new Dictionary<char, int>());
		}
		for (int i = 0; i < mActiveLayerList.Count; i++)
		{
			ActiveFontLayer activeFontLayer = mActiveLayerList[i];
			CharData charData = activeFontLayer.mBaseFontLayer.GetCharData(theChar);
			int num2 = 0;
			int num3 = activeFontLayer.mBaseFontLayer.mPointSize;
			int num4;
			int num5;
			if (num3 == 0)
			{
				num4 = (((double)charData.mWidth * mScale >= 0.0) ? ((int)((double)charData.mWidth * mScale + 0.501)) : ((int)((double)charData.mWidth * mScale - 0.501)));
				if (thePrevChar != 0)
				{
					num5 = activeFontLayer.mBaseFontLayer.mSpacing;
					CharData charData2 = activeFontLayer.mBaseFontLayer.GetCharData(thePrevChar);
					if (charData2.mKerningCount != 0)
					{
						int mKerningCount = charData2.mKerningCount;
						for (int j = 0; j < mKerningCount; j++)
						{
							_ = activeFontLayer.mBaseFontLayer.mKerningData[j].mChar;
						}
					}
				}
				else
				{
					num5 = 0;
				}
			}
			else
			{
				num4 = (((double)charData.mWidth * num / (double)(float)num3 >= 0.0) ? ((int)((double)charData.mWidth * num / (double)(float)num3 + 0.501)) : ((int)((double)charData.mWidth * num / (double)(float)num3 - 0.501)));
				if (thePrevChar != 0)
				{
					num5 = activeFontLayer.mBaseFontLayer.mSpacing;
					CharData charData3 = activeFontLayer.mBaseFontLayer.GetCharData(thePrevChar);
					if (charData3.mKerningCount != 0)
					{
						int mKerningCount2 = charData3.mKerningCount;
						for (int k = 0; k < mKerningCount2; k++)
						{
							_ = activeFontLayer.mBaseFontLayer.mKerningData[k].mChar;
						}
					}
				}
				else
				{
					num5 = 0;
				}
			}
			num2 += num4 + num5;
			if (num2 > value)
			{
				value = num2;
			}
		}
		mCachedCharPair[theChar].Add(thePrevChar, value);
		return value;
	}

	public override int StringWidth(string theString)
	{
		int num = 0;
		char thePrevChar = '\0';
		foreach (char c in theString)
		{
			num += CharWidthKern(c, thePrevChar);
			thePrevChar = c;
		}
		return num;
	}

	public override void DrawString(Graphics g, int theX, int theY, string theString, Color theColor, Rect theClipRect)
	{
		int theWidth = 0;
		DrawStringEx(g, theX, theY, theString, theColor, theClipRect, null, ref theWidth);
	}

	public virtual void SetPointSize(int thePointSize)
	{
		mPointSize = thePointSize;
		mActiveListValid = false;
	}

	public virtual int GetPointSize()
	{
		return mPointSize;
	}

	public virtual void SetScale(double theScale)
	{
		mScale = theScale;
		mActiveListValid = false;
	}

	public virtual int GetDefaultPointSize()
	{
		return mFontData.mDefaultPointSize;
	}

	public virtual bool AddTag(string theTagName)
	{
		if (HasTag(theTagName))
		{
			return false;
		}
		string item = theTagName.ToUpper();
		mTagVector.Add(item);
		mActiveListValid = false;
		return true;
	}

	public virtual bool RemoveTag(string theTagName)
	{
		string item = theTagName.ToUpper();
		if (mTagVector.Remove(item))
		{
			mActiveListValid = false;
			return true;
		}
		return false;
	}

	public virtual bool HasTag(string theTagName)
	{
		return mTagVector.Contains(theTagName);
	}

	public virtual string GetDefine(string theName)
	{
		DataElement dataElement = mFontData.Dereference(theName);
		if (dataElement == null)
		{
			return "";
		}
		return mFontData.DataElementToString(dataElement, enclose: true);
	}

	public virtual void Prepare()
	{
		if (!mActiveListValid)
		{
			GenerateActiveFontLayers();
			mActiveListValid = true;
		}
	}

	public virtual void WriteToCache(string theSrcFile, string theAltData)
	{
	}

	public string SerializeReadStr(byte[] thePtr, int theStartIndex, int size)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < size; i++)
		{
			stringBuilder.Append((char)thePtr[theStartIndex + i]);
		}
		return stringBuilder.ToString();
	}

	public bool SerializeRead(byte[] thePtr, int theSize, int theStartIndex)
	{
		if (thePtr == null)
		{
			return false;
		}
		bool result = false;
		int num = theStartIndex;
		mAscent = BitConverter.ToInt32(thePtr, num);
		num += 4;
		mAscentPadding = BitConverter.ToInt32(thePtr, num);
		num += 4;
		mHeight = BitConverter.ToInt32(thePtr, num);
		num += 4;
		mLineSpacingOffset = BitConverter.ToInt32(thePtr, num);
		num += 4;
		mFontData.mApp = GlobalMembers.gSexyAppBase;
		mFontData.mInitialized = BitConverter.ToBoolean(thePtr, num);
		num++;
		mFontData.mDefaultPointSize = BitConverter.ToInt32(thePtr, num);
		num += 4;
		int num2 = BitConverter.ToInt32(thePtr, num);
		num += 4;
		for (int i = 0; i < num2; i++)
		{
			ushort key = BitConverter.ToUInt16(thePtr, num);
			num += 2;
			ushort value = BitConverter.ToUInt16(thePtr, num);
			num += 2;
			mFontData.mCharMap.Add((char)key, (char)value);
		}
		int num3 = BitConverter.ToInt32(thePtr, num);
		num += 4;
		for (int j = 0; j < num3; j++)
		{
			mFontData.mFontLayerList.AddLast(new FontLayer(mFontData));
			FontLayer value2 = mFontData.mFontLayerList.Last.Value;
			int num4 = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mLayerName = SerializeReadStr(thePtr, num, num4);
			num += num4;
			mFontData.mFontLayerMap.Add(value2.mLayerName, value2);
			int num5 = BitConverter.ToInt32(thePtr, num);
			num += 4;
			for (int k = 0; k < num5; k++)
			{
				int num6 = BitConverter.ToInt32(thePtr, num);
				num += 4;
				string item = SerializeReadStr(thePtr, num, num6);
				num += num6;
				value2.mRequiredTags.Add(item);
			}
			num5 = BitConverter.ToInt32(thePtr, num);
			num += 4;
			for (int l = 0; l < num5; l++)
			{
				int num7 = BitConverter.ToInt32(thePtr, num);
				num += 4;
				string item2 = SerializeReadStr(thePtr, num, num7);
				num += num7;
				value2.mExcludedTags.Add(item2);
			}
			int num8 = BitConverter.ToInt32(thePtr, num);
			num += 4;
			if (num8 != 0)
			{
				value2.mKerningData.Clear();
				for (int m = 0; m < num8; m++)
				{
					FontLayer.KerningValue item3 = new FontLayer.KerningValue
					{
						mInt = BitConverter.ToInt32(thePtr, num)
					};
					num += 4;
					item3.mChar = (ushort)((item3.mInt >> 16) & 0xFF);
					item3.mOffset = (short)(item3.mInt & 0xFF);
					value2.mKerningData.Add(item3);
				}
			}
			int num9 = BitConverter.ToInt32(thePtr, num);
			num += 4;
			for (int n = 0; n < num9; n++)
			{
				ushort inChar = BitConverter.ToUInt16(thePtr, num);
				num += 2;
				CharData charData = value2.mCharDataHashTable.GetCharData((char)inChar, inAllowAdd: true);
				charData.mImageRect.mX = BitConverter.ToInt32(thePtr, num);
				num += 4;
				charData.mImageRect.mY = BitConverter.ToInt32(thePtr, num);
				num += 4;
				charData.mImageRect.mWidth = BitConverter.ToInt32(thePtr, num);
				num += 4;
				charData.mImageRect.mHeight = BitConverter.ToInt32(thePtr, num);
				num += 4;
				charData.mOffset.mX = BitConverter.ToInt32(thePtr, num);
				num += 4;
				charData.mOffset.mY = BitConverter.ToInt32(thePtr, num);
				num += 4;
				charData.mKerningFirst = BitConverter.ToUInt16(thePtr, num);
				num += 2;
				charData.mKerningCount = BitConverter.ToUInt16(thePtr, num);
				num += 2;
				charData.mWidth = BitConverter.ToInt32(thePtr, num);
				num += 4;
				charData.mOrder = BitConverter.ToInt32(thePtr, num);
				num += 4;
			}
			value2.mColorMult.mRed = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mColorMult.mGreen = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mColorMult.mBlue = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mColorMult.mAlpha = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mColorAdd.mRed = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mColorAdd.mGreen = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mColorAdd.mBlue = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mColorAdd.mAlpha = BitConverter.ToInt32(thePtr, num);
			num += 4;
			int num10 = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mImageFileName = SerializeReadStr(thePtr, num, num10);
			num += num10;
			bool flag = false;
			SharedImageRef sharedImageRef = new SharedImageRef();
			if (GlobalMembers.gSexyAppBase.mResourceManager != null && string.IsNullOrEmpty(mFontData.mImagePathPrefix))
			{
				string idByPath = GlobalMembers.gSexyAppBase.mResourceManager.GetIdByPath(value2.mImageFileName);
				if (!string.IsNullOrEmpty(idByPath))
				{
					sharedImageRef = GlobalMembers.gSexyAppBase.mResourceManager.GetImage(idByPath);
					if (sharedImageRef.GetDeviceImage() == null)
					{
						sharedImageRef = GlobalMembers.gSexyAppBase.mResourceManager.LoadImage(idByPath);
					}
					if (sharedImageRef.GetDeviceImage() != null)
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				sharedImageRef = GlobalMembers.gSexyAppBase.GetSharedImage(mFontData.mImagePathPrefix + value2.mImageFileName);
			}
			value2.mImage = new SharedImageRef(sharedImageRef);
			if (value2.mImage.GetDeviceImage() == null)
			{
				result = true;
			}
			value2.mDrawMode = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mOffset.mX = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mOffset.mY = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mSpacing = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mMinPointSize = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mMaxPointSize = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mPointSize = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mAscent = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mAscentPadding = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mHeight = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mDefaultHeight = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mLineSpacingOffset = BitConverter.ToInt32(thePtr, num);
			num += 4;
			value2.mBaseOrder = BitConverter.ToInt32(thePtr, num);
			num += 4;
		}
		int num11 = BitConverter.ToInt32(thePtr, num);
		num += 4;
		mFontData.mSourceFile = SerializeReadStr(thePtr, num, num11);
		num += num11;
		int num12 = BitConverter.ToInt32(thePtr, num);
		num += 4;
		mFontData.mFontErrorHeader = SerializeReadStr(thePtr, num, num12);
		num += num12;
		mPointSize = BitConverter.ToInt32(thePtr, num);
		num += 4;
		int num13 = BitConverter.ToInt32(thePtr, num);
		num += 4;
		for (int num14 = 0; num14 < num13; num14++)
		{
			int num15 = BitConverter.ToInt32(thePtr, num);
			num += 4;
			string item4 = SerializeReadStr(thePtr, num, num15);
			num += num15;
			mTagVector.Add(item4);
		}
		mScale = BitConverter.ToDouble(thePtr, num);
		num += 8;
		mForceScaledImagesWhite = BitConverter.ToBoolean(thePtr, num);
		num++;
		mActivateAllLayers = BitConverter.ToBoolean(thePtr, num);
		num++;
		mActiveListValid = false;
		return result;
	}

	public bool SerializeReadEndian(IntPtr thePtr, int theSize)
	{
		return false;
	}

	public bool SerializeWrite(IntPtr thePtr)
	{
		return SerializeWrite(thePtr, 0);
	}

	public bool SerializeWrite(IntPtr thePtr, int theSizeIfKnown)
	{
		return false;
	}

	public int GetLayerCount()
	{
		LinkedList<FontLayer>.Enumerator enumerator = mFontData.mFontLayerList.GetEnumerator();
		int num = 0;
		while (enumerator.MoveNext())
		{
			FontLayer current = enumerator.Current;
			if (current.mLayerName.Length < 6 || current.mLayerName.Substring(current.mLayerName.Length - 5) != "__MOD")
			{
				num++;
			}
		}
		return num;
	}

	public void PushLayerColor(string theLayerName, Color theColor)
	{
		Prepare();
		string text = theLayerName + "__MOD";
		for (int i = 0; i < mActiveLayerList.Count; i++)
		{
			ActiveFontLayer activeFontLayer = mActiveLayerList[i];
			if (activeFontLayer.mBaseFontLayer.mLayerName.ToLower() == theLayerName.ToLower() || activeFontLayer.mBaseFontLayer.mLayerName.ToLower() == text.ToLower())
			{
				activeFontLayer.PushColor(theColor);
			}
		}
	}

	public void PushLayerColor(int theLayer, Color theColor)
	{
		Prepare();
		LinkedList<FontLayer>.Enumerator enumerator = mFontData.mFontLayerList.GetEnumerator();
		int num = 0;
		while (enumerator.MoveNext())
		{
			FontLayer current = enumerator.Current;
			if (current.mLayerName.Length < 6 || current.mLayerName.Substring(current.mLayerName.Length - 5) != "__MOD")
			{
				if (num == theLayer)
				{
					PushLayerColor(current.mLayerName, theColor);
					break;
				}
				num++;
			}
		}
	}

	public void PopLayerColor(string theLayerName)
	{
		string text = theLayerName + "__MOD";
		for (int i = 0; i < mActiveLayerList.Count; i++)
		{
			ActiveFontLayer activeFontLayer = mActiveLayerList[i];
			if (activeFontLayer.mBaseFontLayer.mLayerName.ToLower() == theLayerName.ToLower() || activeFontLayer.mBaseFontLayer.mLayerName.ToLower() == text.ToLower())
			{
				activeFontLayer.PopColor();
			}
		}
	}

	public void PopLayerColor(int theLayer)
	{
		LinkedList<FontLayer>.Enumerator enumerator = mFontData.mFontLayerList.GetEnumerator();
		int num = 0;
		while (enumerator.MoveNext())
		{
			FontLayer current = enumerator.Current;
			if (current.mLayerName.Length < 6 || current.mLayerName.Substring(current.mLayerName.Length - 5) != "__MOD")
			{
				if (num == theLayer)
				{
					PopLayerColor(current.mLayerName);
					break;
				}
				num++;
			}
		}
	}
}
