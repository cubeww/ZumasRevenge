using SexyFramework.Sound;

namespace ZumasRevenge.Sound;

internal class LoopingSound : BasicSound
{
	private SoundInstance mSoundInstance;

	public bool mPaused;

	private bool mUnloadSource;

	private float mVolume = 1f;

	private int mPan;

	private float mPitch;

	public LoopingSound(int inSoundID, SoundManager inSoundManager)
	{
		m_SoundID = inSoundID;
		m_SoundManager = inSoundManager;
		mVolume = (float)m_SoundManager.GetMasterVolume();
	}

	public override void Dispose()
	{
	}

	public override void Play()
	{
		if (mSoundInstance == null && FindFreeSoundInstance(ref mSoundInstance))
		{
			mSoundInstance.SetPan(mPan);
			mSoundInstance.AdjustPitch(mPitch);
			mSoundInstance.SetVolume(m_SoundManager.GetMasterVolume() * mVolume);
			mSoundInstance.Play(looping: true, autoRelease: false);
		}
	}

	protected override bool FindFreeSoundInstance(ref SoundInstance outInstance)
	{
		SoundInstance soundInstance = m_SoundManager.GetSoundInstance(m_SoundID);
		if (soundInstance != null)
		{
			outInstance = soundInstance;
		}
		return soundInstance != null;
	}

	public override void Fade()
	{
		if (mSoundInstance != null)
		{
			mSoundInstance.Stop();
		}
	}

	public override void Update()
	{
	}

	public override float GetOptionVolume()
	{
		return (float)m_SoundManager.GetMasterVolume();
	}

	public override void Pause(bool inPauseOn)
	{
		mPaused = inPauseOn;
		if (mSoundInstance != null)
		{
			if (mPaused)
			{
				mSoundInstance.Pause();
			}
			else
			{
				mSoundInstance.Resume();
			}
		}
	}

	public override bool IsFree()
	{
		return mSoundInstance == null;
	}

	public override bool IsFading()
	{
		return false;
	}

	public override bool IsLooping()
	{
		return mSoundInstance != null;
	}

	public override float GetVolume()
	{
		if (!mPaused)
		{
			return mVolume;
		}
		return 0f;
	}

	public override void SetPan(int inPan)
	{
		mPan = inPan;
		if (mSoundInstance != null)
		{
			mSoundInstance.SetPan(inPan);
		}
	}

	public override void SetPitch(float inPitch)
	{
		mPitch = inPitch;
		if (mSoundInstance != null)
		{
			mSoundInstance.AdjustPitch(inPitch);
		}
	}

	public override void SetVolume(float inVolume)
	{
		mVolume = inVolume;
		if (mSoundInstance != null)
		{
			mSoundInstance.SetVolume(m_SoundManager.GetMasterVolume() * inVolume);
		}
	}

	public override void EnableAutoUnload()
	{
		mUnloadSource = true;
	}
}
