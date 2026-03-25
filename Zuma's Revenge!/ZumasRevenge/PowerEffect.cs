using System.Collections.Generic;
using System.Linq;
using JeffLib;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class PowerEffect
{
	public enum Type
	{
		Bomb,
		Accuracy,
		Reverse,
		Stop,
		Cannon,
		Laser
	}

	public bool mDrawReverse;

	protected List<EffectItem> mItems = new List<EffectItem>();

	protected float mX;

	protected float mY;

	protected int mUpdateCount;

	protected bool mDone;

	protected int mType;

	protected int mColorType;

	protected Transform mGlobalTranform = new Transform();

	protected EffectItem AddItem(Image img, Color c, int cel)
	{
		EffectItem effectItem = new EffectItem();
		mItems.Add(effectItem);
		effectItem.mImage = img;
		effectItem.mCel = cel;
		effectItem.mColor = new Color(c);
		return effectItem;
	}

	protected EffectItem AddItem(Image img, Color c)
	{
		return AddItem(img, c, 0);
	}

	public PowerEffect(float x, float y)
	{
		mX = x;
		mY = y;
		mUpdateCount = 0;
		mDone = false;
		mDrawReverse = false;
		mType = -1;
		mColorType = -1;
	}

	public PowerEffect()
		: this(0f, 0f)
	{
	}

	public virtual void AddDefaultEffectType(int eff_type, int color_type, float init_rotation)
	{
		Color c = default(Color);
		Color c2 = default(Color);
		switch ((BallColors)color_type)
		{
		case BallColors.Blue_Ball:
			c = new Color(Common._M(150), Common._M1(150), Common._M2(255));
			c2 = new Color(Common._M3(75), Common._M4(75), Common._M5(255));
			break;
		case BallColors.Yellow_Ball:
			c = new Color(Common._M(255), Common._M1(255), Common._M2(50));
			c2 = new Color(Common._M3(255), Common._M4(255), Common._M5(0));
			break;
		case BallColors.Red_Ball:
			c = new Color(Common._M(250), Common._M1(140), Common._M2(0));
			c2 = new Color(Common._M3(250), Common._M4(50), Common._M5(1));
			break;
		case BallColors.Green_Ball:
			c = new Color(Common._M(200), Common._M1(200), Common._M2(0));
			c2 = new Color(Common._M3(0), Common._M4(185), Common._M5(118));
			break;
		case BallColors.Purple_Ball:
			c = new Color(Common._M(255), Common._M1(100), Common._M2(255));
			c2 = new Color(Common._M3(255), Common._M4(50), Common._M5(255));
			break;
		case BallColors.White_Ball:
			c = new Color(Common._M(255), Common._M1(255), Common._M2(255));
			c2 = new Color(Common._M3(200), Common._M4(200), Common._M5(200));
			break;
		}
		mType = eff_type;
		mColorType = color_type;
		Image imageByID = Res.GetImageByID(ResID.IMAGE_POWERUPS_PULSES);
		Image imageByID2 = Res.GetImageByID(ResID.IMAGE_BALL_GLOW);
		Image imageByID3 = Res.GetImageByID(ResID.IMAGE_BALL_RING);
		Image imageByID4 = Res.GetImageByID(ResID.IMAGE_BLOOM_STOP_OUTLINE);
		float num = 2f;
		switch (eff_type)
		{
		case 0:
		{
			int num8 = 83;
			float num9 = 4f;
			EffectItem effectItem4 = AddItem(imageByID, c2, 3);
			effectItem4.mScale.Add(new Component(1f * num, 1.63f * num, 83 - num8, 115 - num8));
			effectItem4.mOpacity.Add(new Component(255f, 0f, 100 - num8, 130 - num8));
			Image imageByID5 = Res.GetImageByID((ResID)(1400 + color_type));
			effectItem4 = AddItem(imageByID5, Color.White, 0);
			effectItem4.mScale.Add(new Component(0.2f * num9, 1f * num9, 83 - num8, 105 - num8));
			effectItem4.mAngle.Add(new Component(init_rotation, init_rotation + 3.14159f, 83 - num8, 105 - num8));
			effectItem4.mOpacity.Add(new Component(0f, 255f, 83 - num8, 105 - num8));
			effectItem4.mOpacity.Add(new Component(255f, 0f, 106 - num8, 120 - num8));
			effectItem4 = AddItem(imageByID5, Color.White, 0);
			effectItem4.mScale.Add(new Component(0.2f, 1f, 83 - num8, 131 - num8));
			effectItem4.mAngle.Add(new Component(init_rotation, init_rotation - 3.14159f, 83 - num8, 105 - num8));
			effectItem4.mOpacity.Add(new Component(0f, 128f, 83 - num8, 105 - num8));
			effectItem4.mOpacity.Add(new Component(128f, 0f, 106 - num8, 145 - num8));
			break;
		}
		case 1:
		{
			float num6 = 4f;
			int num7 = 35;
			EffectItem effectItem3 = AddItem(imageByID2, c, 0);
			effectItem3.mOpacity.Add(new Component(128f, 255f, 35 - num7, 50 - num7));
			effectItem3.mOpacity.Add(new Component(255f, 0f, 51 - num7, 95 - num7));
			effectItem3 = AddItem(imageByID, c, 0);
			effectItem3.mScale.Add(new Component(0.1f * num, 2f * num, 50 - num7, 65 - num7));
			effectItem3.mAngle.Add(new Component(init_rotation, init_rotation + 3.14159f, 55 - num7, 75 - num7));
			effectItem3.mOpacity.Add(new Component(25f, 255f, 35 - num7, 50 - num7));
			effectItem3.mOpacity.Add(new Component(255f, 0f, 36 - num7, 95 - num7));
			effectItem3 = AddItem(Res.GetImageByID((ResID)(1412 + color_type)), c, 0);
			effectItem3.mScale.Add(new Component(0.1f * num6, 1.1f * num6, 50 - num7, 95 - num7));
			effectItem3.mScale.Add(new Component(1.1f * num6, 1f * num6, 96 - num7, 101 - num7));
			effectItem3.mAngle.Add(new Component(init_rotation, init_rotation + 1.570795f, 55 - num7, 95 - num7));
			effectItem3.mOpacity.Add(new Component(0f, 0f, 35 - num7, 49 - num7));
			effectItem3.mOpacity.Add(new Component(25f, 255f, 50 - num7, 75 - num7));
			effectItem3.mOpacity.Add(new Component(255f, 0f, 115 - num7, 135 - num7));
			effectItem3 = AddItem(imageByID3, c, 0);
			effectItem3.mOpacity.Add(new Component(0f, 0f, 35 - num7, 79 - num7));
			effectItem3.mOpacity.Add(new Component(128f, 255f, 80 - num7, 90 - num7));
			effectItem3.mOpacity.Add(new Component(255f, 0f, 91 - num7, 135 - num7));
			effectItem3.mScale.Add(new Component(1f, 10f, 80 - num7, 135 - num7));
			break;
		}
		case 2:
		{
			float num10 = 4f;
			float num11 = Common._M(8);
			int num12 = (int)(70f / num11);
			EffectItem effectItem5 = AddItem(imageByID2, c, 0);
			effectItem5.mOpacity.Add(new Component(128f, 255f, (int)(70f / num11 - (float)num12), (int)(210f / num11 - (float)num12)));
			effectItem5.mOpacity.Add(new Component(255f, 0f, (int)(211f / num11 - (float)num12), (int)(310f / num11 - (float)num12)));
			effectItem5 = AddItem(imageByID, c2, 4);
			effectItem5.mScale.Add(new Component(1f * num, 2f * num, (int)(109f / num11 - (float)num12), (int)(385f / num11 - (float)num12)));
			effectItem5.mOpacity.Add(new Component(0f, 0f, (int)(70f / num11 - (float)num12), (int)(108f / num11 - (float)num12)));
			effectItem5.mOpacity.Add(new Component(255f, 255f, (int)(109f / num11 - (float)num12), (int)(360f / num11 - (float)num12)));
			effectItem5.mOpacity.Add(new Component(255f, 0f, (int)(361f / num11 - (float)num12), (int)(485f / num11 - (float)num12)));
			Image imageByID6 = Res.GetImageByID((ResID)(1406 + color_type));
			effectItem5 = AddItem(imageByID6, Color.White, 0);
			effectItem5.mOpacity.Add(new Component(0f, 0f, (int)(70f / num11 - (float)num12), (int)(160f / num11 - (float)num12)));
			effectItem5.mOpacity.Add(new Component(0f, 128f, (int)(161f / num11 - (float)num12), (int)(360f / num11 - (float)num12)));
			effectItem5.mOpacity.Add(new Component(128f, 153f, (int)(361f / num11 - (float)num12), (int)(485f / num11 - (float)num12)));
			effectItem5.mOpacity.Add(new Component(153f, 0f, (int)(486f / num11 - (float)num12), (int)(560f / num11 - (float)num12)));
			effectItem5.mScale.Add(new Component(0.2f * num10, 1f * num10, (int)(160f / num11 - (float)num12), (int)(360f / num11 - (float)num12)));
			effectItem5 = AddItem(imageByID6, Color.White, 0);
			effectItem5.mOpacity.Add(new Component(0f, 0f, (int)(70f / num11 - (float)num12), (int)(335f / num11 - (float)num12)));
			effectItem5.mOpacity.Add(new Component(0f, 255f, (int)(336f / num11 - (float)num12), (int)(585f / num11 - (float)num12)));
			effectItem5.mScale.Add(new Component(0.2f, 1f, (int)(335f / num11 - (float)num12), (int)(535f / num11 - (float)num12)));
			break;
		}
		case 3:
		{
			float num4 = init_rotation - 1.570795f;
			float target = ((!(num4 > 3.14159f)) ? 0f : 6.28318f);
			EffectItem effectItem2 = AddItem(imageByID, Color.White, 2);
			effectItem2.mOpacity.Add(new Component(255f, 255f, 0, 15));
			effectItem2.mOpacity.Add(new Component(255f, 0f, 16, 21));
			effectItem2.mScale.Add(new Component(1f * num, 1f * num, 0, 9));
			effectItem2.mScale.Add(new Component(1f * num, 2f * num, 10, 21));
			effectItem2.mAngle.Add(new Component(num4, target, 0, 20));
			float num5 = 2f;
			effectItem2 = AddItem(Res.GetImageByID((ResID)(1418 + color_type)), Color.White, 0);
			effectItem2.mOpacity.Add(new Component(0f, 0f, 0, 9));
			effectItem2.mOpacity.Add(new Component(128f, 255f, 10, 20));
			effectItem2.mOpacity.Add(new Component(255f, 0f, 40, 50));
			effectItem2.mScale.Add(new Component(0.5f * num5, 1.1f * num5, 10, 22));
			effectItem2.mScale.Add(new Component(1.1f * num5, 1f * num5, 23, 30));
			effectItem2.mScale.Add(new Component(1f * num5, 0.5f * num5, 40, 50));
			effectItem2.mYOffset.Add(new Component(0f, Common._M(-10f), 10, 20));
			effectItem2.mAngle.Add(new Component(num4, target, 0, 20));
			effectItem2 = AddItem(Res.GetImageByID((ResID)(1418 + color_type)), Color.White, 0);
			effectItem2.mOpacity.Add(new Component(0f, 0f, 0, 20));
			effectItem2.mOpacity.Add(new Component(0f, 255f, 21, 26));
			effectItem2.mOpacity.Add(new Component(255f, 0f, 27, 37));
			effectItem2.mScale.Add(new Component(1f * num5, 1.1f * num5, 20, 22));
			effectItem2.mScale.Add(new Component(1.1f * num5, 1f * num5, 23, 27));
			effectItem2.mYOffset.Add(new Component(-10f, -10f, 20, 20));
			effectItem2.mAngle.Add(new Component(num4, target, 0, 20));
			effectItem2 = AddItem(imageByID4, Color.White, 0);
			effectItem2.mOpacity.Add(new Component(0f, 0f, 0, 24));
			effectItem2.mOpacity.Add(new Component(255f, 0f, 25, 50));
			effectItem2.mScale.Add(new Component(1f * num5, 3f * num5, 25, 50));
			effectItem2.mAngle.Add(new Component(num4, target, 0, 20));
			break;
		}
		case 5:
		{
			float num2 = init_rotation - 1.570795f;
			float num3 = 4f;
			EffectItem effectItem = AddItem(imageByID2, c, 0);
			effectItem.mOpacity.Add(new Component(128f, 255f, 0, 15));
			effectItem.mOpacity.Add(new Component(255f, 0f, 16, 35));
			effectItem = AddItem(Res.GetImageByID((ResID)(870 + color_type)), Color.White, 6);
			effectItem.mOpacity.Add(new Component(25f, 255f, 0, 15));
			effectItem.mOpacity.Add(new Component(255f, 0f, 16, 35));
			effectItem.mAngle.Add(new Component(num2, num2 + 6.28318f, 15, 35));
			effectItem.mScale.Add(new Component(1f, 2f, 15, 20));
			effectItem = AddItem(Res.GetImageByID((ResID)(1400 + color_type)), Color.White, 0);
			effectItem.mOpacity.Add(new Component(0f, 255f, 0, 59));
			effectItem.mOpacity.Add(new Component(255f, 0f, 60, 80));
			effectItem.mAngle.Add(new Component(num2, num2, 0, 14));
			effectItem.mAngle.Add(new Component(num2, num2 + 6.28318f, 15, 35));
			effectItem.mScale.Add(new Component(0.4f * num3, 1f * num3, 15, 35));
			effectItem.mScale.Add(new Component(1f * num3, 0.1f * num3, 60, 80));
			effectItem = AddItem(Res.GetImageByID((ResID)(1400 + color_type)), Color.White, 0);
			effectItem.mAngle.Add(new Component(num2, num2, 0, 75));
			effectItem.mOpacity.Add(new Component(0f, 0f, 0, 34));
			effectItem.mOpacity.Add(new Component(25f, 255f, 35, 60));
			effectItem.mOpacity.Add(new Component(255f, 0f, 61, 75));
			effectItem.mScale.Add(new Component(2f * num3, 1f * num3, 30, 60));
			effectItem.mScale.Add(new Component(1f * num3, 0f * num3, 61, 71));
			break;
		}
		}
	}

	public virtual void AddDefaultEffectType(int eff_type, int color_type)
	{
		AddDefaultEffectType(eff_type, color_type, 0f);
	}

	public virtual void Update()
	{
		if (!mDone)
		{
			mUpdateCount++;
			bool flag = true;
			for (int i = 0; i < mItems.Count(); i++)
			{
				EffectItem effectItem = mItems[i];
				bool flag2 = Component.UpdateComponentVec(effectItem.mScale, mUpdateCount);
				bool flag3 = Component.UpdateComponentVec(effectItem.mAngle, mUpdateCount);
				bool flag4 = Component.UpdateComponentVec(effectItem.mOpacity, mUpdateCount);
				bool flag5 = Component.UpdateComponentVec(effectItem.mXOffset, mUpdateCount);
				bool flag6 = Component.UpdateComponentVec(effectItem.mYOffset, mUpdateCount);
				flag = flag && flag2 && flag3 && flag4 && flag5 && flag6;
			}
			mDone = flag;
		}
	}

	public virtual void Draw(Graphics g)
	{
		if (mDone)
		{
			return;
		}
		g.PushState();
		g.SetColorizeImages(colorizeImages: true);
		g.SetDrawMode(1);
		int num = (mDrawReverse ? (mItems.Count() - 1) : 0);
		int num2 = ((!mDrawReverse) ? mItems.Count() : 0);
		for (int i = num; mDrawReverse ? (i >= num2) : (i < num2); i += ((!mDrawReverse) ? 1 : (-1)))
		{
			EffectItem effectItem = mItems[i];
			Color mColor = effectItem.mColor;
			mColor.mAlpha = (int)Component.GetComponentValue(effectItem.mOpacity, 255f, mUpdateCount);
			if (mColor.mAlpha != 0)
			{
				float componentValue = Component.GetComponentValue(effectItem.mAngle, 0f, mUpdateCount);
				float componentValue2 = Component.GetComponentValue(effectItem.mScale, 1f, mUpdateCount);
				float tx = Common._S(Component.GetComponentValue(effectItem.mXOffset, 0f, mUpdateCount));
				float ty = Common._S(Component.GetComponentValue(effectItem.mYOffset, 0f, mUpdateCount));
				g.SetColor(mColor);
				mGlobalTranform.Reset();
				mGlobalTranform.RotateRad(componentValue);
				mGlobalTranform.Translate(tx, ty);
				mGlobalTranform.Scale(componentValue2, componentValue2);
				Rect celRect = effectItem.mImage.GetCelRect(effectItem.mCel);
				if (g.Is3D())
				{
					g.DrawImageTransformF(effectItem.mImage, mGlobalTranform, celRect, Common._S(mX), Common._S(mY));
				}
				else
				{
					g.DrawImageTransform(effectItem.mImage, mGlobalTranform, celRect, Common._S(mX), Common._S(mY));
				}
			}
		}
		g.PopState();
	}

	public virtual bool IsDone()
	{
		return mDone;
	}

	public int GetUpdateCount()
	{
		return mUpdateCount;
	}

	public new int GetType()
	{
		return mType;
	}

	public virtual void SyncState(DataSync sync)
	{
		sync.SyncLong(ref mType);
		sync.SyncLong(ref mColorType);
		if (sync.isRead())
		{
			mItems.Clear();
			AddDefaultEffectType(mType, mColorType);
		}
		sync.SyncBoolean(ref mDrawReverse);
		sync.SyncFloat(ref mX);
		sync.SyncFloat(ref mY);
		sync.SyncLong(ref mUpdateCount);
		sync.SyncBoolean(ref mDone);
		for (int i = 0; i < mItems.Count; i++)
		{
			mItems[i].SyncState(sync);
		}
	}
}
