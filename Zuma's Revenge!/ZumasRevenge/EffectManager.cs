using System;
using System.Collections.Generic;
using SexyFramework;

namespace ZumasRevenge;

public class EffectManager : IDisposable
{
	protected List<Effect> mEffects = new List<Effect>();

	public EffectManager()
	{
		Reset();
	}

	public virtual void Dispose()
	{
		for (int i = 0; i < mEffects.Count; i++)
		{
			if (mEffects[i] != null)
			{
				mEffects[i].Dispose();
				mEffects[i] = null;
			}
		}
		mEffects.Clear();
	}

	public void Reset()
	{
		if (GameApp.gApp != null && GameApp.gApp.mShutdown)
		{
			return;
		}
		for (int i = 0; i < mEffects.Count; i++)
		{
			if (mEffects[i] != null)
			{
				mEffects[i].Dispose();
				mEffects[i] = null;
			}
		}
		mEffects.Clear();
		mEffects.Add(new WaterEffect1());
		mEffects.Add(new WillOWisp());
		mEffects.Add(new BallWake());
		mEffects.Add(new Fog());
		mEffects.Add(new WaterShader1());
		mEffects.Add(new LavaShader());
	}

	public Effect GetEffect(string fx_name, string level_id, Level copy_effects_from)
	{
		for (int i = 0; i < mEffects.Count; i++)
		{
			Effect effect = mEffects[i];
			if (!SexyFramework.Common.StrEquals(effect.GetName(), fx_name, pIgnoreCase: true))
			{
				continue;
			}
			Effect effect2 = null;
			if (copy_effects_from != null)
			{
				for (int j = 0; j < copy_effects_from.mEffects.Count; j++)
				{
					if (SexyFramework.Common.StrEquals(copy_effects_from.mEffects[j].GetName(), fx_name, pIgnoreCase: true))
					{
						effect2 = copy_effects_from.mEffects[j];
						break;
					}
				}
			}
			if (effect2 != null)
			{
				return effect2;
			}
			effect.Reset(level_id);
			return effect;
		}
		return null;
	}

	private Effect GetEffect(string fx_name, string level_id)
	{
		return GetEffect(fx_name, level_id);
	}

	private void CopyFrom(EffectManager m)
	{
		Reset();
		for (int i = 0; i < m.mEffects.Count; i++)
		{
			mEffects[i].CopyFrom(m.mEffects[i]);
		}
	}
}
