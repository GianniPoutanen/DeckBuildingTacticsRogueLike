using UnityEngine;

public class SoundManager : MonoBehaviour
{
	#region Singleton Pattern

	private static SoundManager instance;

	public static SoundManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<SoundManager>();
				if (instance == null)
				{
					GameObject obj = new GameObject();
					obj.name = typeof(SoundManager).Name;
					instance = obj.AddComponent<SoundManager>();
				}
			}
			return instance;
		}
	}

	#endregion

	// Add your sound-related variables and state here
	public AudioClip backgroundMusic;
	public AudioClip playerHitSound;
	public AudioClip gameOverSound;

	private AudioSource audioSource;

	private void Start()
	{
		// Initialize the audio source
		audioSource = gameObject.AddComponent<AudioSource>();

		// Play background music
		PlayBackgroundMusic();
	}

	private void OnDestroy()
	{
		instance = null;
	}

	// Example method to play background music
	private void PlayBackgroundMusic()
	{
		if (backgroundMusic != null)
		{
			audioSource.clip = backgroundMusic;
			audioSource.loop = true;
			audioSource.Play();
		}
	}

	// Example method to play a sound effect when the player is hit
	public void PlayPlayerHitSound()
	{
		if (playerHitSound != null)
		{
			audioSource.PlayOneShot(playerHitSound);
		}
	}

	// Example method to play a sound effect when the game is over
	public void PlayGameOverSound()
	{
		if (gameOverSound != null)
		{
			audioSource.PlayOneShot(gameOverSound);
		}
	}

	// You can add more methods and functionality as needed for your sound manager
}
