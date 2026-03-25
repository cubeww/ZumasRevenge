namespace ZumasRevenge.Sound;

internal class FadedSound : UpdatedSound
{
	private bool mIsFree = true;

	private float mFadeInSpeed;

	private float mFadeOutSpeed;

	private float mTargetVolume;

	private float mLastTarget = -1f;

	private bool mIsPaused;

	private bool mIsFadeOut;

	public FadedSound(Sound inSound, float inFadeInSpeed, float inFadeOutSpeed)
	{
		mSound = inSound;
		mFadeInSpeed = inFadeInSpeed;
		mFadeOutSpeed = inFadeOutSpeed;
	}

	public override void Dispose()
	{
		mSound.Dispose();
	}

	public override void Play()
	{
		if (mIsFree)
		{
			mIsFree = false;
			mTargetVolume = mSound.GetVolume();
			mSound.SetVolume(0f);
			mSound.Play();
			mIsFadeOut = false;
			mIsPaused = false;
		}
	}

	public override void Fade()
	{
		mTargetVolume = 0f;
		mIsFadeOut = true;
	}

	public override void Update()
	{
		if (mIsPaused)
		{
			return;
		}
		float volume = mSound.GetVolume();
		if (volume == mTargetVolume && !mIsFadeOut)
		{
			return;
		}
		if (mTargetVolume == 0f || mIsFadeOut)
		{
			volume -= mFadeOutSpeed;
			if (volume < 0f)
			{
				volume = 0f;
				mIsFadeOut = false;
				mSound.Fade();
			}
		}
		else
		{
			volume += mFadeInSpeed;
			if (volume > mTargetVolume)
			{
				volume = mTargetVolume;
			}
		}
		if (volume == 0f)
		{
			mIsFree = true;
		}
		else
		{
			mSound.SetVolume(volume);
		}
	}

	public override void Pause(bool inPauseOn)
	{
		if (!inPauseOn)
		{
			RestoreTargetVolume();
		}
		mIsPaused = inPauseOn;
		mSound.Pause(inPauseOn);
	}

	public override bool IsFree()
	{
		return mIsFree;
	}

	public override bool IsFading()
	{
		if (mTargetVolume == 0f)
		{
			return mSound.GetVolume() > 0f;
		}
		return false;
	}

	public override bool IsLooping()
	{
		return mSound.IsLooping();
	}

	public override float GetVolume()
	{
		return mSound.GetVolume();
	}

	public override void SetPan(int inPan)
	{
		mSound.SetPan(inPan);
	}

	public override void SetPitch(float inPitch)
	{
		mSound.SetPitch(inPitch);
	}

	public override void SetVolume(float inVolume)
	{
		mSound.SetVolume(inVolume);
	}

	public override void EnableAutoUnload()
	{
		mSound.EnableAutoUnload();
	}

	public override float GetOptionVolume()
	{
		return 0f;
	}

	protected void CacheTargetVolume()
	{
		if (mTargetVolume != 0f)
		{
			mLastTarget = mTargetVolume;
			mTargetVolume = 0f;
		}
	}

	protected void RestoreTargetVolume()
	{
		if (!mIsFadeOut)
		{
			mTargetVolume = mSound.GetOptionVolume();
		}
	}
}
