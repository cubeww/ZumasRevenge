using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class CurveData
{
	public static int gVersion = 15;

	public static float SUBPIXEL_MULT = 100f;

	public static float INV_SUBPIXEL_MULT = 1f / SUBPIXEL_MULT;

	public List<PathPoint> mPointList = new List<PathPoint>();

	public int mEditType;

	public int mVersion = 268435455;

	public string mErrorString;

	public BasicCurveVals mVals = new BasicCurveVals();

	public bool mDrawCurve;

	public bool mLinear;

	public CurveData()
	{
		Clear();
	}

	public virtual void Dispose()
	{
		mPointList = null;
		mVals = null;
	}

	protected bool Fail(string theString)
	{
		mErrorString = theString;
		return false;
	}

	public bool Save(string theFilePath)
	{
		return false;
	}

	public bool Load(string theFilePath)
	{
		Clear();
		SexyFramework.Misc.Buffer theBuffer = new SexyFramework.Misc.Buffer();
		if (!GameApp.gApp.ReadBufferFromStream(theFilePath + ".dat", ref theBuffer))
		{
			return false;
		}
		MemoryStream input = new MemoryStream(theBuffer.GetDataPtr());
		BinaryReader binaryReader = new BinaryReader(input);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < 4; i++)
		{
			stringBuilder.Append((char)binaryReader.ReadByte());
		}
		if (stringBuilder.ToString() != "CURV")
		{
			Console.WriteLine("Invalid file header");
			return false;
		}
		mVersion = binaryReader.ReadInt32();
		if (mVersion < 1 || mVersion > gVersion)
		{
			Console.WriteLine("Invalid file header");
			return false;
		}
		if (mVersion >= 8)
		{
			mLinear = binaryReader.ReadBoolean();
		}
		if (mVersion >= 7)
		{
			mVals.mStartDistance = (int)binaryReader.ReadUInt32();
			mVals.mNumBalls = (int)binaryReader.ReadUInt32();
			mVals.mBallRepeat = (int)binaryReader.ReadUInt32();
			mVals.mMaxSingle = (int)binaryReader.ReadUInt32();
			mVals.mNumColors = (int)binaryReader.ReadUInt32();
			if (mVersion <= 10)
			{
				binaryReader.ReadInt32();
				binaryReader.ReadSingle();
			}
			mVals.mSpeed = binaryReader.ReadSingle();
			mVals.mSlowDistance = (int)binaryReader.ReadUInt32();
			mVals.mAccelerationRate = binaryReader.ReadSingle();
			mVals.mOrgAccelerationRate = mVals.mAccelerationRate;
			mVals.mMaxSpeed = binaryReader.ReadSingle();
			mVals.mOrgMaxSpeed = mVals.mMaxSpeed;
			mVals.mScoreTarget = (int)binaryReader.ReadUInt32();
			mVals.mSkullRotation = (int)binaryReader.ReadUInt32();
			mVals.mZumaBack = (int)binaryReader.ReadUInt32();
			mVals.mZumaSlow = (int)binaryReader.ReadUInt32();
			if (mVersion >= 13)
			{
				mVals.mSlowFactor = binaryReader.ReadSingle();
			}
			else
			{
				mVals.mSlowFactor = 4f;
			}
			if (mVersion >= 14)
			{
				mVals.mMaxClumpSize = (int)binaryReader.ReadUInt32();
			}
			else
			{
				mVals.mMaxClumpSize = 10;
			}
			int num = (int)binaryReader.ReadUInt32();
			for (int j = 0; j < 14; j++)
			{
				mVals.mPowerUpFreq[j] = 0;
				mVals.mMaxNumPowerUps[j] = 100000000;
			}
			for (int k = 0; k < num && k < 14; k++)
			{
				if (Common.IsDeprecatedPowerUp((PowerType)k))
				{
					mVals.mMaxNumPowerUps[k] = 0;
					binaryReader.ReadInt32();
					if (mVersion >= 12)
					{
						binaryReader.ReadInt32();
					}
				}
				else
				{
					mVals.mPowerUpFreq[k] = (int)binaryReader.ReadUInt32();
					if (mVersion >= 12)
					{
						mVals.mMaxNumPowerUps[k] = (int)binaryReader.ReadUInt32();
					}
				}
			}
			if (mVersion >= 12)
			{
				mVals.mPowerUpChance = (int)binaryReader.ReadUInt32();
			}
			else
			{
				mVals.mPowerUpChance = 0;
			}
			mDrawCurve = binaryReader.ReadBoolean();
			mVals.mDrawTunnels = binaryReader.ReadBoolean();
			mVals.mDestroyAll = binaryReader.ReadBoolean();
			if (mVersion > 8)
			{
				mVals.mDrawPit = binaryReader.ReadBoolean();
			}
			if (mVersion > 9)
			{
				mVals.mDieAtEnd = binaryReader.ReadBoolean();
			}
		}
		bool flag = false;
		bool flag2 = true;
		if (mVersion >= 3)
		{
			flag = binaryReader.ReadBoolean();
			flag2 = binaryReader.ReadBoolean();
		}
		if (!flag)
		{
			mEditType = (int)binaryReader.ReadUInt32();
			int num2 = (int)binaryReader.ReadUInt32();
			if (num2 > 1000000)
			{
				Console.WriteLine("File is corrupt");
				return false;
			}
			binaryReader.ReadBytes(num2);
		}
		else
		{
			mEditType = 0;
		}
		int num3 = (int)binaryReader.ReadUInt32();
		if (num3 <= 0)
		{
			return true;
		}
		if (mVersion < 2)
		{
			for (int l = 0; l < num3; l++)
			{
				PathPoint pathPoint = new PathPoint();
				pathPoint.x = binaryReader.ReadSingle();
				pathPoint.y = binaryReader.ReadSingle();
				pathPoint.mInTunnel = binaryReader.ReadBoolean();
				pathPoint.mPriority = binaryReader.ReadByte();
				mPointList.Add(pathPoint);
			}
		}
		else if (mVersion < 4)
		{
			PathPoint pathPoint2 = new PathPoint();
			pathPoint2.x = binaryReader.ReadSingle();
			pathPoint2.y = binaryReader.ReadSingle();
			if (flag2)
			{
				pathPoint2.mInTunnel = binaryReader.ReadBoolean();
				pathPoint2.mPriority = binaryReader.ReadByte();
			}
			num3--;
			float x = pathPoint2.x;
			float y = pathPoint2.y;
			mPointList.Add(pathPoint2);
			for (int m = 0; m < num3; m++)
			{
				PathPoint pathPoint3 = new PathPoint();
				sbyte b = binaryReader.ReadSByte();
				sbyte b2 = binaryReader.ReadSByte();
				pathPoint3.x = x + (float)b * INV_SUBPIXEL_MULT;
				pathPoint3.y = y + (float)b2 * INV_SUBPIXEL_MULT;
				if (flag2)
				{
					pathPoint3.mInTunnel = binaryReader.ReadBoolean();
					pathPoint3.mPriority = binaryReader.ReadByte();
				}
				x = pathPoint3.x;
				y = pathPoint3.y;
				mPointList.Add(pathPoint3);
			}
		}
		else
		{
			float num4 = 0f;
			float num5 = 0f;
			for (int n = 0; n < num3; n++)
			{
				PathPoint pathPoint4 = new PathPoint();
				mPointList.Add(pathPoint4);
				byte b3 = binaryReader.ReadByte();
				pathPoint4.mInTunnel = (((b3 & 1) != 0) ? true : false);
				bool flag3 = (((b3 & 2) != 0) ? true : false);
				if (flag2 || mVersion >= 15)
				{
					pathPoint4.mPriority = binaryReader.ReadByte();
				}
				if (flag3)
				{
					pathPoint4.x = binaryReader.ReadSingle();
					pathPoint4.y = binaryReader.ReadSingle();
				}
				else
				{
					sbyte b4 = binaryReader.ReadSByte();
					sbyte b5 = binaryReader.ReadSByte();
					pathPoint4.x = num4 + (float)b4 * INV_SUBPIXEL_MULT;
					pathPoint4.y = num5 + (float)b5 * INV_SUBPIXEL_MULT;
				}
				num4 = pathPoint4.x;
				num5 = pathPoint4.y;
			}
		}
		return true;
	}

	public void Copy(CurveData dest)
	{
		dest.mDrawCurve = mDrawCurve;
		dest.mVals = mVals;
	}

	public void Clear()
	{
		mPointList.Clear();
		mEditType = 0;
		mLinear = false;
		mVals.mDieAtEnd = true;
		mVals.mStartDistance = 50;
		mVals.mNumBalls = 0;
		mVals.mBallRepeat = 50;
		mVals.mMaxSingle = 10;
		mVals.mNumColors = 4;
		mVals.mSlowDistance = 200;
		mVals.mScoreTarget = 1000;
		mVals.mSkullRotation = -1;
		mVals.mZumaBack = 300;
		mVals.mZumaSlow = 1100;
		mVals.mSlowFactor = 4f;
		mVals.mMaxClumpSize = 10;
		mVals.mSpeed = 0.5f;
		mVals.mAccelerationRate = 0f;
		mVals.mMaxSpeed = 100f;
		for (int i = 0; i < 14; i++)
		{
			mVals.mPowerUpFreq[i] = 0;
			mVals.mMaxNumPowerUps[i] = 100000000;
		}
		mVals.mPowerUpChance = 1200;
		mVals.mDrawPit = true;
		mDrawCurve = true;
		mVals.mDrawTunnels = true;
		mVals.mDestroyAll = true;
	}
}
