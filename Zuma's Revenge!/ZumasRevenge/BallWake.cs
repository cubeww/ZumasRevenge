using System.Collections.Generic;
using System.Linq;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace ZumasRevenge;

public class BallWake : Effect
{
	protected List<WakeStruct> mWake = new List<WakeStruct>();

	protected Transform mGlobalTranform = new Transform();

	private void DebugCheck()
	{
	}

	public BallWake()
	{
		mResGroup = "Underwater";
	}

	public override void Reset(string level_id)
	{
		base.Reset(level_id);
		mWake.Clear();
	}

	public override void Update()
	{
		List<WakeStruct> list = new List<WakeStruct>();
		for (int i = 0; i < mWake.Count(); i++)
		{
			WakeStruct wakeStruct = mWake[i];
			wakeStruct.mUpdateCount++;
			wakeStruct.mX += wakeStruct.mVel.x;
			wakeStruct.mY += wakeStruct.mVel.y;
			int num = (int)((float)wakeStruct.mImage.mWidth * wakeStruct.mSize);
			int num2 = (int)((float)wakeStruct.mImage.mHeight * wakeStruct.mSize);
			if (!new Rect((int)(wakeStruct.mX - (float)(num / 2)), (int)(wakeStruct.mY - (float)(num2 / 2)), num, num2).Intersects(new Rect(0, 0, Common._SS(GlobalMembers.gSexyApp.mWidth), Common._SS(GlobalMembers.gSexyApp.mHeight))))
			{
				mWake.RemoveAt(i);
				i--;
			}
			else if (wakeStruct.mExpanding || !wakeStruct.mIsHead)
			{
				if (wakeStruct.mIsHead)
				{
					wakeStruct.mVel.x -= wakeStruct.mVel.x / Common._M(2f);
					wakeStruct.mVel.y -= wakeStruct.mVel.y / Common._M(2f);
				}
				if (!wakeStruct.mIsHead)
				{
					if (wakeStruct.mAlphaInc > 0f)
					{
						wakeStruct.mAlpha += wakeStruct.mAlphaInc;
						float num3 = Common._M(255f);
						if (wakeStruct.mAlpha >= num3)
						{
							wakeStruct.mAlpha = num3;
							wakeStruct.mAlphaInc = 0f;
						}
					}
					else
					{
						wakeStruct.mAlpha -= Common._M(35f);
					}
				}
				else
				{
					wakeStruct.mAlpha -= Common._M(40f);
				}
				if (wakeStruct.mAlpha < 0f)
				{
					mWake.RemoveAt(i);
					i--;
				}
				else
				{
					wakeStruct.mSize += Common._M(0.03f);
				}
			}
			else if (wakeStruct.mIsHead && !wakeStruct.mExpanding && wakeStruct.mUpdateCount % Common._M(1) == 0)
			{
				WakeStruct wakeStruct2 = new WakeStruct();
				float num4 = Common._M(4f);
				float mAlphaInc = Common._M(40f);
				wakeStruct2.mBallId = wakeStruct.mBallId;
				wakeStruct2.mImage = Res.GetImageByID(ResID.IMAGE_FX_UNDERWATER_SIDEWAKE_DARK);
				wakeStruct2.mVel = wakeStruct.mVel.Perp() / num4;
				SexyVector2 sexyVector = wakeStruct2.mVel.Normalize();
				wakeStruct2.mAngle = wakeStruct.mAngle;
				wakeStruct2.mX = wakeStruct.mX + Common._M(40f) * sexyVector.x;
				wakeStruct2.mY = wakeStruct.mY + Common._M(40f) * sexyVector.y;
				wakeStruct2.mAlpha = 0f;
				wakeStruct2.mAlphaInc = mAlphaInc;
				list.Add(wakeStruct2);
				WakeStruct wakeStruct3 = new WakeStruct();
				wakeStruct3.mBallId = wakeStruct.mBallId;
				wakeStruct3.mImage = Res.GetImageByID(ResID.IMAGE_FX_UNDERWATER_SIDEWAKE_LIGHT);
				wakeStruct3.mAdditive = true;
				wakeStruct3.mVel = wakeStruct.mVel.Perp() / num4;
				sexyVector = wakeStruct3.mVel.Normalize();
				wakeStruct3.mAngle = wakeStruct.mAngle;
				wakeStruct3.mX = wakeStruct.mX + Common._M(15f) * sexyVector.x;
				wakeStruct3.mY = wakeStruct.mY + Common._M(15f) * sexyVector.y;
				wakeStruct3.mAlpha = 0f;
				wakeStruct3.mAlphaInc = mAlphaInc;
				list.Add(wakeStruct3);
				WakeStruct wakeStruct4 = new WakeStruct();
				wakeStruct4.mBallId = wakeStruct.mBallId;
				wakeStruct4.mImage = Res.GetImageByID(ResID.IMAGE_FX_UNDERWATER_SIDEWAKE_DARK);
				wakeStruct4.mVel = -wakeStruct.mVel.Perp() / num4;
				sexyVector = wakeStruct4.mVel.Normalize();
				wakeStruct4.mAngle = wakeStruct.mAngle;
				wakeStruct4.mX = wakeStruct.mX + (float)Common._M(40) * sexyVector.x;
				wakeStruct4.mY = wakeStruct.mY + (float)Common._M(40) * sexyVector.y;
				wakeStruct4.mAlpha = 0f;
				wakeStruct4.mAlphaInc = mAlphaInc;
				list.Add(wakeStruct4);
				WakeStruct wakeStruct5 = new WakeStruct();
				wakeStruct5.mBallId = wakeStruct.mBallId;
				wakeStruct5.mImage = Res.GetImageByID(ResID.IMAGE_FX_UNDERWATER_SIDEWAKE_LIGHT);
				wakeStruct5.mAdditive = true;
				wakeStruct5.mVel = -wakeStruct.mVel.Perp() / num4;
				sexyVector = wakeStruct5.mVel.Normalize();
				wakeStruct5.mAngle = wakeStruct.mAngle;
				wakeStruct5.mX = wakeStruct.mX + (float)Common._M(15) * sexyVector.x;
				wakeStruct5.mY = wakeStruct.mY + (float)Common._M(15) * sexyVector.y;
				wakeStruct5.mAlpha = 0f;
				wakeStruct5.mAlphaInc = mAlphaInc;
				list.Add(wakeStruct5);
			}
		}
		for (int j = 0; j < list.Count(); j++)
		{
			mWake.Add(list[j]);
		}
		list.Clear();
		DebugCheck();
	}

	public override string GetName()
	{
		return "BallWake";
	}

	public override void DrawAboveBalls(Graphics g)
	{
		if (!g.Is3D())
		{
			return;
		}
		DebugCheck();
		for (int i = 0; i < mWake.Count(); i++)
		{
			WakeStruct wakeStruct = mWake[i];
			if (wakeStruct.mAdditive)
			{
				g.SetDrawMode(1);
			}
			if (wakeStruct.mAlpha != 255f)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(255, 255, 255, (int)wakeStruct.mAlpha);
			}
			mGlobalTranform.Reset();
			mGlobalTranform.RotateRad(wakeStruct.mAngle);
			if (wakeStruct.mSize != 1f)
			{
				mGlobalTranform.Scale(wakeStruct.mSize, wakeStruct.mSize);
			}
			if (g.Is3D())
			{
				g.DrawImageTransformF(wakeStruct.mImage, mGlobalTranform, Common._S(wakeStruct.mX), Common._S(wakeStruct.mY));
			}
			else
			{
				g.DrawImageTransform(wakeStruct.mImage, mGlobalTranform, Common._S(wakeStruct.mX), Common._S(wakeStruct.mY));
			}
			g.SetColorizeImages(colorizeImages: false);
			g.SetDrawMode(0);
		}
	}

	public override void BulletFired(Bullet b)
	{
		WakeStruct wakeStruct = new WakeStruct();
		mWake.Add(wakeStruct);
		wakeStruct.mBallId = (uint)b.GetId();
		wakeStruct.mExpanding = false;
		wakeStruct.mVel = new SexyVector2(b.mVelX, b.mVelY);
		wakeStruct.mSize = 1f;
		wakeStruct.mAlpha = 255f;
		wakeStruct.mAdditive = true;
		wakeStruct.mImage = Res.GetImageByID(ResID.IMAGE_FX_UNDERWATER_BALL_WAKE);
		GameApp.gApp.GetBoard().GetGun();
		wakeStruct.mAngle = b.mAngleFired;
		SexyVector2 sexyVector = wakeStruct.mVel.Normalize();
		wakeStruct.mX = b.GetX() - 20f * sexyVector.x;
		wakeStruct.mY = b.GetY() - 20f * sexyVector.y;
		wakeStruct.mIsHead = true;
	}

	public override void BulletHit(Bullet b)
	{
		for (int i = 0; i < mWake.size(); i++)
		{
			WakeStruct wakeStruct = mWake[i];
			if (wakeStruct.mBallId == b.GetId())
			{
				wakeStruct.mExpanding = true;
			}
		}
	}
}
