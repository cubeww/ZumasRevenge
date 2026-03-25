using System;
using System.Collections.Generic;
using SexyFramework.Misc;
using SexyFramework.Resource;

namespace SexyFramework.Graphics;

public class FontData : DescParser
{
	public SexyAppBase mApp;

	public bool mInitialized;

	public int mRefCount;

	public int mDefaultPointSize;

	public Dictionary<char, char> mCharMap = new Dictionary<char, char>();

	public LinkedList<FontLayer> mFontLayerList = new LinkedList<FontLayer>();

	public Dictionary<string, FontLayer> mFontLayerMap = new Dictionary<string, FontLayer>();

	public string mSourceFile;

	public string mFontErrorHeader;

	public string mImagePathPrefix;

	public override bool Error(string theError)
	{
		return false;
	}

	public bool GetColorFromDataElement(DataElement theElement, ref Color theColor)
	{
		if (theElement.mIsList)
		{
			List<double> theDoubleVector = new List<double>();
			if (!DataToDoubleVector(theElement, ref theDoubleVector) && theDoubleVector.Count == 4)
			{
				return false;
			}
			theColor = new Color((int)(theDoubleVector[0] * 255.0), (int)(theDoubleVector[1] * 255.0), (int)(theDoubleVector[2] * 255.0), (int)(theDoubleVector[3] * 255.0));
			return true;
		}
		int theIntVal = 0;
		if (!Common.StringToInt(((SingleDataElement)theElement).mString.ToString(), ref theIntVal))
		{
			return false;
		}
		theColor = new Color(theIntVal);
		return true;
	}

	public bool DataToLayer(DataElement theSource, ref FontLayer theFontLayer)
	{
		theFontLayer = null;
		if (theSource.mIsList)
		{
			return false;
		}
		string key = ((SingleDataElement)theSource).mString.ToString().ToUpper();
		if (!mFontLayerMap.ContainsKey(key))
		{
			return false;
		}
		theFontLayer = mFontLayerMap[key];
		return true;
	}

	public override bool HandleCommand(ListDataElement theParams)
	{
		string text = ((SingleDataElement)theParams.mElementVector[0]).mString.ToString();
		if (text[0] == '\ufeff')
		{
			text = text.Substring(1);
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		switch (text)
		{
		case "Define":
			if (theParams.mElementVector.Count == 3)
			{
				if (!theParams.mElementVector[1].mIsList)
				{
					string text3 = ((SingleDataElement)theParams.mElementVector[1]).mString.ToString().ToUpper();
					if (!IsImmediate(text3))
					{
						if (mDefineMap.ContainsKey(text3))
						{
							mDefineMap.Remove(text3);
						}
						if (theParams.mElementVector[2].mIsList)
						{
							ListDataElement listDataElement3 = new ListDataElement();
							if (!GetValues((ListDataElement)theParams.mElementVector[2], listDataElement3))
							{
								listDataElement3?.Dispose();
								return false;
							}
							mDefineMap.Add(text3, listDataElement3);
						}
						else
						{
							SingleDataElement singleDataElement = (SingleDataElement)theParams.mElementVector[2];
							DataElement dataElement = Dereference(singleDataElement.mString.ToString());
							if (dataElement != null)
							{
								mDefineMap.Add(text3, dataElement.Duplicate());
							}
							else
							{
								mDefineMap.Add(text3, singleDataElement.Duplicate());
							}
						}
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag2 = true;
				}
			}
			else
			{
				flag = true;
			}
			break;
		case "CreateHorzSpanRectList":
			if (theParams.mElementVector.Count == 4)
			{
				List<int> theIntVector7 = new List<int>();
				List<int> theIntVector8 = new List<int>();
				if (!theParams.mElementVector[1].mIsList && DataToIntVector(theParams.mElementVector[2], ref theIntVector7) && theIntVector7.Count == 4 && DataToIntVector(theParams.mElementVector[3], ref theIntVector8))
				{
					string key = ((SingleDataElement)theParams.mElementVector[1]).mString.ToString().ToUpper();
					int num11 = 0;
					ListDataElement listDataElement = new ListDataElement();
					for (int num12 = 0; num12 < theIntVector8.Count; num12++)
					{
						ListDataElement listDataElement2 = new ListDataElement();
						listDataElement.mElementVector.Add(listDataElement2);
						string text2 = "";
						text2 = (theIntVector7[0] + num11).ToString();
						listDataElement2.mElementVector.Add(new SingleDataElement(text2));
						text2 = theIntVector7[1].ToString();
						listDataElement2.mElementVector.Add(new SingleDataElement(text2));
						text2 = theIntVector8[num12].ToString();
						listDataElement2.mElementVector.Add(new SingleDataElement(text2));
						text2 = theIntVector7[3].ToString();
						listDataElement2.mElementVector.Add(new SingleDataElement(text2));
						num11 += theIntVector8[num12];
					}
					if (mDefineMap.ContainsKey(key))
					{
						mDefineMap.Remove(key);
					}
					mDefineMap.Add(key, listDataElement);
				}
				else
				{
					flag2 = true;
				}
			}
			else
			{
				flag = true;
			}
			break;
		case "SetDefaultPointSize":
			if (theParams.mElementVector.Count == 2)
			{
				int theIntVal10 = 0;
				if (!theParams.mElementVector[1].mIsList && Common.StringToInt(((SingleDataElement)theParams.mElementVector[1]).mString.ToString(), ref theIntVal10))
				{
					mDefaultPointSize = theIntVal10;
				}
				else
				{
					flag2 = true;
				}
			}
			else
			{
				flag = true;
			}
			break;
		case "SetCharMap":
			if (theParams.mElementVector.Count == 3)
			{
				List<string> theStringVector12 = new List<string>();
				List<string> theStringVector13 = new List<string>();
				if (DataToStringVector(theParams.mElementVector[1], ref theStringVector12) && DataToStringVector(theParams.mElementVector[2], ref theStringVector13))
				{
					if (theStringVector12.Count == theStringVector13.Count)
					{
						for (int num10 = 0; num10 < theStringVector12.Count; num10++)
						{
							if (theStringVector12[num10].Length == 1 && theStringVector13[num10].Length == 1)
							{
								mCharMap.Add(theStringVector12[num10][0], theStringVector13[num10][0]);
							}
							else
							{
								flag2 = true;
							}
						}
					}
					else
					{
						flag4 = true;
					}
				}
				else
				{
					flag2 = true;
				}
			}
			else
			{
				flag = true;
			}
			break;
		case "CreateLayer":
			if (theParams.mElementVector.Count == 2)
			{
				if (!theParams.mElementVector[1].mIsList)
				{
					string text4 = ((SingleDataElement)theParams.mElementVector[1]).mString.ToString().ToUpper();
					mFontLayerList.AddLast(new FontLayer(this));
					FontLayer value = mFontLayerList.Last.Value;
					value.mLayerName = text4;
					value.mBaseOrder = mFontLayerList.Count - 1;
					mFontLayerMap.Add(text4, value);
				}
				else
				{
					flag2 = true;
				}
			}
			else
			{
				flag = true;
			}
			break;
		case "CreateLayerFrom":
			if (theParams.mElementVector.Count == 3)
			{
				FontLayer theFontLayer25 = new FontLayer();
				if (!theParams.mElementVector[1].mIsList && DataToLayer(theParams.mElementVector[2], ref theFontLayer25))
				{
					string text5 = ((SingleDataElement)theParams.mElementVector[1]).mString.ToString().ToUpper();
					mFontLayerList.AddLast(new FontLayer(theFontLayer25));
					FontLayer value2 = mFontLayerList.Last.Value;
					value2.mLayerName = text5;
					value2.mBaseOrder = mFontLayerList.Count - 1;
					mFontLayerMap.Add(text5, value2);
				}
				else
				{
					flag2 = true;
				}
			}
			else
			{
				flag = true;
			}
			break;
		case "LayerRequireTags":
			if (theParams.mElementVector.Count == 3)
			{
				FontLayer theFontLayer20 = null;
				List<string> theStringVector10 = new List<string>();
				if (DataToLayer(theParams.mElementVector[1], ref theFontLayer20) && DataToStringVector(theParams.mElementVector[2], ref theStringVector10))
				{
					for (int num8 = 0; num8 < theStringVector10.Count; num8++)
					{
						theFontLayer20.mRequiredTags.Add(theStringVector10[num8].ToUpper());
					}
				}
				else
				{
					flag2 = true;
				}
			}
			else
			{
				flag = true;
			}
			break;
		case "LayerExcludeTags":
			if (theParams.mElementVector.Count == 3)
			{
				FontLayer theFontLayer21 = null;
				List<string> theStringVector11 = new List<string>();
				if (DataToLayer(theParams.mElementVector[1], ref theFontLayer21) && DataToStringVector(theParams.mElementVector[2], ref theStringVector11))
				{
					for (int num9 = 0; num9 < theStringVector11.Count; num9++)
					{
						theFontLayer21.mExcludedTags.Add(theStringVector11[num9].ToUpper());
					}
				}
				else
				{
					flag2 = true;
				}
			}
			else
			{
				flag = true;
			}
			break;
		case "LayerPointRange":
			if (theParams.mElementVector.Count == 4)
			{
				FontLayer theFontLayer22 = null;
				if (DataToLayer(theParams.mElementVector[1], ref theFontLayer22) && !theParams.mElementVector[2].mIsList && !theParams.mElementVector[3].mIsList)
				{
					int theIntVal8 = 0;
					int theIntVal9 = 0;
					if (Common.StringToInt(((SingleDataElement)theParams.mElementVector[2]).mString.ToString(), ref theIntVal8) && Common.StringToInt(((SingleDataElement)theParams.mElementVector[3]).mString.ToString(), ref theIntVal9))
					{
						theFontLayer22.mMinPointSize = theIntVal8;
						theFontLayer22.mMaxPointSize = theIntVal9;
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag2 = true;
				}
			}
			else
			{
				flag = true;
			}
			break;
		case "LayerSetPointSize":
			if (theParams.mElementVector.Count == 3)
			{
				FontLayer theFontLayer19 = null;
				if (DataToLayer(theParams.mElementVector[1], ref theFontLayer19) && !theParams.mElementVector[2].mIsList)
				{
					int theIntVal7 = 0;
					if (Common.StringToInt(((SingleDataElement)theParams.mElementVector[2]).mString.ToString(), ref theIntVal7))
					{
						theFontLayer19.mPointSize = theIntVal7;
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag2 = true;
				}
			}
			else
			{
				flag = true;
			}
			break;
		case "LayerSetHeight":
			if (theParams.mElementVector.Count == 3)
			{
				FontLayer theFontLayer24 = null;
				if (DataToLayer(theParams.mElementVector[1], ref theFontLayer24) && !theParams.mElementVector[2].mIsList)
				{
					int theIntVal11 = 0;
					if (Common.StringToInt(((SingleDataElement)theParams.mElementVector[2]).mString.ToString(), ref theIntVal11))
					{
						theFontLayer24.mHeight = theIntVal11;
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag2 = true;
				}
			}
			else
			{
				flag = true;
			}
			break;
		case "LayerSetImage":
			if (theParams.mElementVector.Count == 3)
			{
				FontLayer theFontLayer23 = null;
				string theString = "";
				if (DataToLayer(theParams.mElementVector[1], ref theFontLayer23) && DataToString(theParams.mElementVector[2], ref theString))
				{
					string pathFrom = Common.GetPathFrom(theString, Common.GetFileDir(mSourceFile, withSlash: false));
					bool mWriteToSexyCache = false;
					if (GlobalMembers.gSexyAppBase != null)
					{
						mWriteToSexyCache = GlobalMembers.gSexyAppBase.mWriteToSexyCache;
						GlobalMembers.gSexyAppBase.mWriteToSexyCache = false;
					}
					bool isNew = false;
					bool flag5 = false;
					SharedImageRef sharedImageRef = new SharedImageRef();
					if (GlobalMembers.gSexyAppBase != null && GlobalMembers.gSexyAppBase.mResourceManager != null && string.IsNullOrEmpty(mImagePathPrefix))
					{
						string idByPath = GlobalMembers.gSexyAppBase.mResourceManager.GetIdByPath(pathFrom);
						if (!string.IsNullOrEmpty(idByPath))
						{
							sharedImageRef = GlobalMembers.gSexyAppBase.mResourceManager.GetImage(idByPath);
							if (sharedImageRef.GetDeviceImage() == null)
							{
								sharedImageRef = GlobalMembers.gSexyAppBase.mResourceManager.LoadImage(idByPath);
							}
							if (sharedImageRef.GetDeviceImage() != null)
							{
								flag5 = true;
							}
						}
					}
					if (GlobalMembers.gSexyAppBase != null && !flag5)
					{
						sharedImageRef = GlobalMembers.gSexyAppBase.GetSharedImage(mImagePathPrefix + pathFrom, "", ref isNew, allowTriReps: false, isInAtlas: false);
					}
					theFontLayer23.mImageFileName = pathFrom;
					if (GlobalMembers.gSexyAppBase != null)
					{
						GlobalMembers.gSexyAppBase.mWriteToSexyCache = mWriteToSexyCache;
					}
					if (sharedImageRef.GetImage() == null)
					{
						Error("Failed to Load Image");
						return false;
					}
					if (!isNew && sharedImageRef.GetMemoryImage().mColorTable != null)
					{
						theFontLayer23.mImageIsWhite = true;
						for (int num13 = 0; num13 < 256; num13++)
						{
							if ((sharedImageRef.GetMemoryImage().mColorTable[num13] & 0xFFFFFF) != 16777215 && sharedImageRef.GetMemoryImage().mColorTable[num13] != 0)
							{
								theFontLayer23.mImageIsWhite = false;
								break;
							}
						}
					}
					theFontLayer23.mImage = new SharedImageRef(sharedImageRef);
				}
				else
				{
					flag2 = true;
				}
			}
			else
			{
				flag = true;
			}
			break;
		default:
			if (text.Equals("LayerSetDrawMode"))
			{
				if (theParams.mElementVector.Count == 3)
				{
					FontLayer theFontLayer = new FontLayer();
					if (DataToLayer(theParams.mElementVector[1], ref theFontLayer) && !theParams.mElementVector[2].mIsList)
					{
						int theIntVal = 0;
						if (Common.StringToInt(((SingleDataElement)theParams.mElementVector[2]).mString.ToString(), ref theIntVal) && theIntVal >= 0 && theIntVal <= 1)
						{
							theFontLayer.mDrawMode = theIntVal;
						}
						else
						{
							flag2 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag = true;
				}
				break;
			}
			if (text.Equals("LayerSetColorMult"))
			{
				if (theParams.mElementVector.Count == 3)
				{
					FontLayer theFontLayer2 = new FontLayer();
					if (DataToLayer(theParams.mElementVector[1], ref theFontLayer2))
					{
						if (!GetColorFromDataElement(theParams.mElementVector[2], ref theFontLayer2.mColorMult))
						{
							flag2 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag = true;
				}
				break;
			}
			if (text.Equals("LayerSetColorAdd"))
			{
				if (theParams.mElementVector.Count == 3)
				{
					FontLayer theFontLayer3 = new FontLayer();
					if (DataToLayer(theParams.mElementVector[1], ref theFontLayer3))
					{
						if (!GetColorFromDataElement(theParams.mElementVector[2], ref theFontLayer3.mColorAdd))
						{
							flag2 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag = true;
				}
				break;
			}
			if (text.Equals("LayerSetAscent"))
			{
				if (theParams.mElementVector.Count == 3)
				{
					FontLayer theFontLayer4 = new FontLayer();
					if (DataToLayer(theParams.mElementVector[1], ref theFontLayer4) && !theParams.mElementVector[2].mIsList)
					{
						int theIntVal2 = 0;
						if (Common.StringToInt(((SingleDataElement)theParams.mElementVector[2]).mString.ToString(), ref theIntVal2))
						{
							theFontLayer4.mAscent = theIntVal2;
						}
						else
						{
							flag2 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag = true;
				}
				break;
			}
			if (text.Equals("LayerSetAscentPadding"))
			{
				if (theParams.mElementVector.Count == 3)
				{
					FontLayer theFontLayer5 = new FontLayer();
					if (DataToLayer(theParams.mElementVector[1], ref theFontLayer5) && !theParams.mElementVector[2].mIsList)
					{
						int theIntVal3 = 0;
						if (Common.StringToInt(((SingleDataElement)theParams.mElementVector[2]).mString.ToString(), ref theIntVal3))
						{
							theFontLayer5.mAscentPadding = theIntVal3;
						}
						else
						{
							flag2 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag = true;
				}
				break;
			}
			if (text.Equals("LayerSetLineSpacingOffset"))
			{
				if (theParams.mElementVector.Count == 3)
				{
					FontLayer theFontLayer6 = new FontLayer();
					if (DataToLayer(theParams.mElementVector[1], ref theFontLayer6) && !theParams.mElementVector[2].mIsList)
					{
						int theIntVal4 = 0;
						if (Common.StringToInt(((SingleDataElement)theParams.mElementVector[2]).mString.ToString(), ref theIntVal4))
						{
							theFontLayer6.mLineSpacingOffset = theIntVal4;
						}
						else
						{
							flag2 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag = true;
				}
				break;
			}
			if (text.Equals("LayerSetOffset"))
			{
				if (theParams.mElementVector.Count == 3)
				{
					FontLayer theFontLayer7 = new FontLayer();
					List<int> theIntVector = new List<int>();
					if (DataToLayer(theParams.mElementVector[1], ref theFontLayer7) && DataToIntVector(theParams.mElementVector[2], ref theIntVector) && theIntVector.Count == 2)
					{
						theFontLayer7.mOffset.mX = theIntVector[0];
						theFontLayer7.mOffset.mY = theIntVector[1];
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag = true;
				}
				break;
			}
			if (text.Equals("LayerSetCharWidths"))
			{
				if (theParams.mElementVector.Count == 4)
				{
					FontLayer theFontLayer8 = new FontLayer();
					List<string> theStringVector = new List<string>();
					List<int> theIntVector2 = new List<int>();
					if (DataToLayer(theParams.mElementVector[1], ref theFontLayer8) && DataToStringVector(theParams.mElementVector[2], ref theStringVector) && DataToIntVector(theParams.mElementVector[3], ref theIntVector2))
					{
						if (theStringVector.Count == theIntVector2.Count)
						{
							for (int i = 0; i < theStringVector.Count; i++)
							{
								if (theStringVector[i].Length == 1)
								{
									theFontLayer8.GetCharData(theStringVector[i][0]).mWidth = theIntVector2[i];
								}
								else
								{
									flag2 = true;
								}
							}
						}
						else
						{
							flag4 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag = true;
				}
				break;
			}
			if (text.Equals("LayerSetSpacing"))
			{
				if (theParams.mElementVector.Count == 3)
				{
					FontLayer theFontLayer9 = new FontLayer();
					new List<int>();
					if (DataToLayer(theParams.mElementVector[1], ref theFontLayer9) && !theParams.mElementVector[2].mIsList)
					{
						int theIntVal5 = 0;
						if (Common.StringToInt(((SingleDataElement)theParams.mElementVector[2]).mString.ToString(), ref theIntVal5))
						{
							theFontLayer9.mSpacing = theIntVal5;
						}
						else
						{
							flag2 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag = true;
				}
				break;
			}
			if (text.Equals("LayerSetImageMap"))
			{
				if (theParams.mElementVector.Count == 4)
				{
					FontLayer theFontLayer10 = null;
					List<string> theStringVector2 = new List<string>();
					ListDataElement theValues = new ListDataElement();
					if (DataToLayer(theParams.mElementVector[1], ref theFontLayer10) && DataToStringVector(theParams.mElementVector[2], ref theStringVector2) && DataToList(theParams.mElementVector[3], ref theValues))
					{
						if (theStringVector2.Count == theValues.mElementVector.Count)
						{
							for (int j = 0; j < theStringVector2.Count; j++)
							{
								List<int> theIntVector3 = new List<int>();
								if (theStringVector2[j].Length == 1 && DataToIntVector(theValues.mElementVector[j], ref theIntVector3) && theIntVector3.Count == 4)
								{
									Rect mImageRect = new Rect(theIntVector3[0], theIntVector3[1], theIntVector3[2], theIntVector3[3]);
									theFontLayer10.GetCharData(theStringVector2[j][0]).mImageRect = mImageRect;
								}
								else
								{
									flag2 = true;
								}
							}
							theFontLayer10.mDefaultHeight = 0;
							int num = theFontLayer10.mCharDataHashTable.CharCount();
							CharData[] array = theFontLayer10.mCharDataHashTable.ToArray();
							for (int k = 0; k < num; k++)
							{
								if (array[k].mImageRect.mHeight + array[k].mOffset.mY > theFontLayer10.mDefaultHeight)
								{
									theFontLayer10.mDefaultHeight = array[k].mImageRect.mHeight + array[k].mOffset.mY;
								}
							}
						}
						else
						{
							flag4 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				else if (theParams.mElementVector.Count == 3)
				{
					FontLayer theFontLayer11 = null;
					List<string> theStringVector3 = new List<string>();
					if (DataToLayer(theParams.mElementVector[1], ref theFontLayer11) && DataToStringVector(theParams.mElementVector[2], ref theStringVector3))
					{
						for (int l = 0; l < theStringVector3.Count; l++)
						{
							new List<int>();
							if (theStringVector3[l].Length == 1)
							{
								theFontLayer11.GetCharData(theStringVector3[l][0]).mImageRect = new Rect(0, 0, 0, 0);
							}
							else
							{
								flag2 = true;
							}
						}
						theFontLayer11.mDefaultHeight = 0;
						int num2 = theFontLayer11.mCharDataHashTable.CharCount();
						CharData[] array2 = theFontLayer11.mCharDataHashTable.ToArray();
						for (int m = 0; m < num2; m++)
						{
							if (array2[m].mImageRect.mHeight + array2[m].mOffset.mY > theFontLayer11.mDefaultHeight)
							{
								theFontLayer11.mDefaultHeight = array2[m].mImageRect.mHeight + array2[m].mOffset.mY;
							}
						}
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
				break;
			}
			if (text.Equals("LayerSetCharOffsets"))
			{
				if (theParams.mElementVector.Count == 4)
				{
					FontLayer theFontLayer12 = new FontLayer();
					List<string> theStringVector4 = new List<string>();
					ListDataElement theValues2 = new ListDataElement();
					if (DataToLayer(theParams.mElementVector[1], ref theFontLayer12) && DataToStringVector(theParams.mElementVector[2], ref theStringVector4) && DataToList(theParams.mElementVector[3], ref theValues2))
					{
						if (theStringVector4.Count == theValues2.mElementVector.Count)
						{
							for (int n = 0; n < theStringVector4.Count; n++)
							{
								List<int> theIntVector4 = new List<int>();
								if (theStringVector4[n].Length == 1 && DataToIntVector(theValues2.mElementVector[n], ref theIntVector4) && theIntVector4.Count == 2)
								{
									theFontLayer12.GetCharData(theStringVector4[n][0]).mOffset = new Point(theIntVector4[0], theIntVector4[1]);
								}
								else
								{
									flag2 = true;
								}
							}
						}
						else
						{
							flag4 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				else if (theParams.mElementVector.Count == 3)
				{
					FontLayer theFontLayer13 = null;
					List<string> theStringVector5 = new List<string>();
					if (DataToLayer(theParams.mElementVector[1], ref theFontLayer13) && DataToStringVector(theParams.mElementVector[2], ref theStringVector5))
					{
						for (int num3 = 0; num3 < theStringVector5.Count; num3++)
						{
							new List<int>();
							theFontLayer13.GetCharData(theStringVector5[num3][0]).mOffset = new Point(0, 0);
						}
					}
				}
				else
				{
					flag = true;
				}
				break;
			}
			if (text.Equals("LayerSetKerningPairs"))
			{
				if (theParams.mElementVector.Count == 4)
				{
					FontLayer theFontLayer14 = new FontLayer();
					List<string> theStringVector6 = new List<string>();
					List<int> theIntVector5 = new List<int>();
					if (DataToLayer(theParams.mElementVector[1], ref theFontLayer14) && DataToStringVector(theParams.mElementVector[2], ref theStringVector6) && DataToIntVector(theParams.mElementVector[3], ref theIntVector5))
					{
						if (theStringVector6.Count == theIntVector5.Count)
						{
							List<SortedKern> list = new List<SortedKern>();
							for (int num4 = 0; num4 < theStringVector6.Count; num4++)
							{
								if (theStringVector6[num4].Length == 2)
								{
									list.Add(new SortedKern(theStringVector6[num4][0], theStringVector6[num4][1], theIntVector5[num4]));
								}
								else
								{
									flag2 = true;
								}
							}
							if (list.Count != 0)
							{
								list.Sort();
							}
							theFontLayer14.mKerningData.Clear();
							for (int num5 = 0; num5 < list.Count; num5++)
							{
								SortedKern sortedKern = list[num5];
								FontLayer.KerningValue item = default(FontLayer.KerningValue);
								item.mChar = sortedKern.mValue;
								item.mOffset = (short)sortedKern.mOffset;
								item.mInt = (item.mChar << 16) | (ushort)item.mOffset;
								theFontLayer14.mKerningData.Add(item);
								CharData charData = theFontLayer14.GetCharData(sortedKern.mKey);
								if (charData.mKerningCount == 0)
								{
									charData.mKerningFirst = (ushort)num5;
								}
								charData.mKerningCount++;
							}
						}
						else
						{
							flag4 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag = true;
				}
				break;
			}
			if (text.Equals("LayerSetBaseOrder"))
			{
				if (theParams.mElementVector.Count == 3)
				{
					FontLayer theFontLayer15 = new FontLayer();
					if (DataToLayer(theParams.mElementVector[1], ref theFontLayer15) && !theParams.mElementVector[2].mIsList)
					{
						int theIntVal6 = 0;
						if (Common.StringToInt(((SingleDataElement)theParams.mElementVector[2]).mString.ToString(), ref theIntVal6))
						{
							theFontLayer15.mBaseOrder = theIntVal6;
						}
						else
						{
							flag2 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag = true;
				}
				break;
			}
			if (text.Equals("LayerSetCharOrders"))
			{
				if (theParams.mElementVector.Count == 4)
				{
					FontLayer theFontLayer16 = new FontLayer();
					List<string> theStringVector7 = new List<string>();
					List<int> theIntVector6 = new List<int>();
					if (DataToLayer(theParams.mElementVector[1], ref theFontLayer16) && DataToStringVector(theParams.mElementVector[2], ref theStringVector7) && DataToIntVector(theParams.mElementVector[3], ref theIntVector6))
					{
						if (theStringVector7.Count == theIntVector6.Count)
						{
							for (int num6 = 0; num6 < theStringVector7.Count; num6++)
							{
								if (theStringVector7[num6].Length == 1)
								{
									theFontLayer16.GetCharData(theStringVector7[num6][0]).mOrder = theIntVector6[num6];
								}
								else
								{
									flag2 = true;
								}
							}
						}
						else
						{
							flag4 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag = true;
				}
				break;
			}
			if (text.Equals("LayerSetExInfo"))
			{
				if (theParams.mElementVector.Count == 4)
				{
					FontLayer theFontLayer17 = new FontLayer();
					List<string> theStringVector8 = new List<string>();
					List<string> theStringVector9 = new List<string>();
					if (DataToLayer(theParams.mElementVector[1], ref theFontLayer17) && DataToStringVector(theParams.mElementVector[2], ref theStringVector8) && DataToStringVector(theParams.mElementVector[3], ref theStringVector9))
					{
						if (theStringVector8.Count == theStringVector9.Count)
						{
							for (int num7 = 0; num7 < theStringVector8.Count; num7++)
							{
								theFontLayer17.mExtendedInfo.Add(theStringVector8[num7], theStringVector9[num7]);
							}
						}
						else
						{
							flag4 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag = true;
				}
				break;
			}
			if (text.Equals("LayerSetAlphaCorrection"))
			{
				if (theParams.mElementVector.Count == 3)
				{
					FontLayer theFontLayer18 = new FontLayer();
					int theInt = 0;
					if (DataToLayer(theParams.mElementVector[1], ref theFontLayer18) && DataToInt(theParams.mElementVector[2], ref theInt))
					{
						theFontLayer18.mUseAlphaCorrection = theInt != 0;
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag = true;
				}
				break;
			}
			Error("Unknown Command");
			return false;
		}
		if (flag)
		{
			Error("Invalid Number of Parameters");
			return false;
		}
		if (flag2)
		{
			Error("Invalid Paramater Type");
			return false;
		}
		if (flag3)
		{
			Error("Undefined Value");
			return false;
		}
		if (flag4)
		{
			Error("List Size Mismatch");
			return false;
		}
		return true;
	}

	public FontData()
	{
		mInitialized = false;
		mApp = null;
		mRefCount = 0;
		mDefaultPointSize = 0;
	}

	public override void Dispose()
	{
		Dictionary<string, DataElement>.Enumerator enumerator = mDefineMap.GetEnumerator();
		while (enumerator.MoveNext())
		{
			_ = enumerator.Current.Key;
			enumerator.Current.Value?.Dispose();
		}
	}

	public void Ref()
	{
		mRefCount++;
	}

	public void DeRef()
	{
		if (--mRefCount == 0)
		{
			Dispose();
		}
	}

	public bool Load(byte[] buffer)
	{
		if (mInitialized)
		{
			return false;
		}
		bool flag = false;
		mCurrentLine.Clear();
		mFontErrorHeader = "Font Descriptor Error in Load\r\n";
		mSourceFile = "";
		mInitialized = LoadDescriptor(buffer);
		return !flag;
	}

	public bool Load(SexyAppBase theSexyApp, string theFontDescFileName)
	{
		if (mInitialized)
		{
			return false;
		}
		mApp = theSexyApp;
		bool flag = false;
		mCurrentLine.Clear();
		mFontErrorHeader = "Font Descriptor Error in " + theFontDescFileName + "\r\n";
		mSourceFile = theFontDescFileName;
		mInitialized = LoadDescriptor(theFontDescFileName);
		return !flag;
	}

	public bool LoadLegacy(Image theFontImage, string theFontDescFileName)
	{
		if (mInitialized)
		{
			return false;
		}
		mFontLayerList.AddLast(new FontLayer(this));
		FontLayer value = mFontLayerList.Last.Value;
		mFontLayerMap.Add("MAIN", value);
		value.mImage.mUnsharedImage = (MemoryImage)theFontImage;
		value.mDefaultHeight = value.mImage.GetImage().GetHeight();
		value.mAscent = value.mImage.GetImage().GetHeight();
		int num = 0;
		PFILE pFILE = new PFILE(theFontDescFileName, "rb");
		if (pFILE == null)
		{
			return false;
		}
		mSourceFile = theFontDescFileName;
		pFILE.Open();
		byte[] data = pFILE.GetData();
		int num2 = 0;
		value.GetCharData(' ').mWidth = BitConverter.ToInt32(data, num2);
		num2 += 4;
		value.mAscent = BitConverter.ToInt32(data, num2);
		num2 += 4;
		while (num2 < data.Length)
		{
			byte b = 0;
			int num3 = 0;
			byte b2 = data[num2];
			num2++;
			num3 = BitConverter.ToInt32(data, num2);
			num2 += 4;
			b = b2;
			if (b == 0)
			{
				break;
			}
			value.GetCharData((char)b).mImageRect = new Rect(num, 0, num3, value.mImage.GetImage().GetHeight());
			value.GetCharData((char)b).mWidth = num3;
			num += num3;
		}
		for (char c = 'A'; c <= 'Z'; c = (char)(c + 1))
		{
			if (value.GetCharData(c).mWidth == 0 && value.GetCharData((char)(c - 65 + 97)).mWidth != 0)
			{
				mCharMap.Add(c, (char)(c - 65 + 97));
			}
		}
		for (char c = 'a'; c <= 'z'; c = (char)(c + 1))
		{
			if (value.GetCharData(c).mWidth == 0 && value.GetCharData((char)(c - 97 + 65)).mWidth != 0)
			{
				mCharMap.Add(c, (char)(c - 97 + 65));
			}
		}
		mInitialized = true;
		pFILE.Close();
		return true;
	}
}
