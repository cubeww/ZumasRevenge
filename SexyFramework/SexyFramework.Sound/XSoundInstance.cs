using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace SexyFramework.Sound;

public class XSoundInstance : SoundInstance
{
	private SoundEffectInstance m_SoundInstance;

	private static Stack<XSoundInstance> unusedObjects = new Stack<XSoundInstance>(20);

	public int m_SoundID;

	public float mBaseVolume;

	public float mBasePan;

	private float mVolume;

	private float mPan;

	private float mPitch;

	private bool didPlay;

	private bool mIsReleased;

	private void ApplyAttributes()
	{
		if (m_SoundInstance == null)
		{
			return;
		}
		float volume = mBaseVolume * mVolume;
		if (volume < 0f)
		{
			volume = 0f;
		}
		else if (volume > 1f)
		{
			volume = 1f;
		}
		float pan = mBasePan + mPan;
		if (pan < -1f)
		{
			pan = -1f;
		}
		else if (pan > 1f)
		{
			pan = 1f;
		}
		float pitch = mPitch / 12f;
		if (pitch < -1f)
		{
			pitch = -1f;
		}
		else if (pitch > 1f)
		{
			pitch = 1f;
		}
		m_SoundInstance.Volume = volume;
		m_SoundInstance.Pan = pan;
		m_SoundInstance.Pitch = pitch;
	}

	public static XSoundInstance GetNewXSoundInstance(int id, SoundEffectInstance instance)
	{
		if (unusedObjects.Count > 0)
		{
			XSoundInstance xSoundInstance = unusedObjects.Pop();
			xSoundInstance.Reset(id, instance);
			return xSoundInstance;
		}
		return new XSoundInstance(id, instance);
	}

	public XSoundInstance(int id, SoundEffectInstance instance)
	{
		Reset(id, instance);
	}

	public void Reset(int id, SoundEffectInstance instance)
	{
		m_SoundInstance = instance;
		m_SoundID = id;
		didPlay = false;
		mBaseVolume = 1f;
		mVolume = 1f;
		mBasePan = 0f;
		mPan = 0f;
		mPitch = 0f;
		mIsReleased = false;
	}

	public override void Release()
	{
		if (m_SoundInstance != null)
		{
			m_SoundInstance.Stop();
			m_SoundInstance.Dispose();
			m_SoundInstance = null;
		}
		mIsReleased = true;
	}

	public override void SetBaseVolume(double theBaseVolume)
	{
		mBaseVolume = (float)theBaseVolume;
		ApplyAttributes();
	}

	public override void SetBasePan(int theBasePan)
	{
		mBasePan = (float)theBasePan / 100f;
		ApplyAttributes();
	}

	public override void SetBaseRate(double theBaseRate)
	{
	}

	public override void AdjustPitch(double theNumSteps)
	{
		mPitch = (float)theNumSteps;
		ApplyAttributes();
	}

	public override void SetVolume(double theVolume)
	{
		mVolume = (float)theVolume;
		ApplyAttributes();
	}

	public override void SetMasterVolumeIdx(int theVolumeIdx)
	{
	}

	public override void SetPan(int thePosition)
	{
		mPan = (float)thePosition / 10000f;
		ApplyAttributes();
	}

	public override bool Play(bool looping, bool autoRelease)
	{
		Stop();
		didPlay = true;
		if (m_SoundInstance.State == SoundState.Stopped)
		{
			m_SoundInstance.IsLooped = looping;
		}
		ApplyAttributes();
		m_SoundInstance.Play();
		return true;
	}

	public override void Stop()
	{
		if (m_SoundInstance != null)
		{
			m_SoundInstance.Stop();
		}
	}

	public override void Pause()
	{
		if (m_SoundInstance != null)
		{
			m_SoundInstance.Pause();
		}
	}

	public override void Resume()
	{
		if (m_SoundInstance != null)
		{
			m_SoundInstance.Resume();
		}
	}

	public override bool IsPlaying()
	{
		if (m_SoundInstance != null)
		{
			return m_SoundInstance.State == SoundState.Playing;
		}
		return false;
	}

	public override bool IsReleased()
	{
		return mIsReleased;
	}

	public override double GetVolume()
	{
		return mVolume;
	}

	public override bool IsDormant()
	{
		if (didPlay)
		{
			return m_SoundInstance.State == SoundState.Stopped;
		}
		return false;
	}

	public void PrepareForReuse()
	{
		unusedObjects.Push(this);
	}
}
