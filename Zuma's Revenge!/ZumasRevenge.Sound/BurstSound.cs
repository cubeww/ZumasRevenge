using SexyFramework.Sound;

namespace ZumasRevenge.Sound;

internal class BurstSound : BasicSound
{
	private SoundInstance mSoundInstance;

	private bool mAutoRelease;

	private bool mPaused;

	private bool mUnloadSource;

	private int mPan;

	private float mPitch;

	private float mVolume = 1f;

	public BurstSound(int inSoundID, SoundManager inSoundManager, bool inAutoRelease)
	{
		m_SoundID = inSoundID;
		m_SoundManager = inSoundManager;
		mAutoRelease = inAutoRelease;
	}

	public override void Dispose()
	{
	}

	public override void Play()
	{
		if (!mPaused && ReleaseInstance() && FindFreeSoundInstance(ref mSoundInstance))
		{
			SetAttributes(mSoundInstance);
			mSoundInstance.Play(looping: false, mAutoRelease);
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

	private void SetAttributes(SoundInstance inInstance)
	{
		inInstance.SetPan(mPan);
		inInstance.AdjustPitch(mPitch);
		inInstance.SetVolume(m_SoundManager.GetMasterVolume() * mVolume);
	}

	private bool ReleaseInstance()
	{
		if (mSoundInstance != null && !mAutoRelease)
		{
			if (mSoundInstance.IsPlaying())
			{
				return false;
			}
			mSoundInstance.Release();
			mSoundInstance = null;
		}
		return true;
	}

	public override void Fade()
	{
	}

	public override void Update()
	{
	}

	public override float GetOptionVolume()
	{
		return 0f;
	}

	public override void Pause(bool inPauseOn)
	{
		mPaused = inPauseOn;
	}

	public override bool IsFree()
	{
		if (!mAutoRelease && mSoundInstance != null)
		{
			return !mSoundInstance.IsPlaying();
		}
		return true;
	}

	public override bool IsFading()
	{
		return false;
	}

	public override bool IsLooping()
	{
		return false;
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
			mSoundInstance.SetVolume(m_SoundManager.GetMasterVolume() * mVolume);
		}
	}

	public override void EnableAutoUnload()
	{
		mUnloadSource = true;
	}
}
