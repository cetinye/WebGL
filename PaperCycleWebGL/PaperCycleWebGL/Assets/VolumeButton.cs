using UnityEngine;
using UnityEngine.UI;

public class VolumeButton : MonoBehaviour
{
	[SerializeField] private Sprite onSprite;
	[SerializeField] private Sprite offSprite;
	private Image image;
	private bool isVolumeOn = true;

	void Awake()
	{
		image = GetComponent<Image>();

		LoadSavedState();
	}

	/// <summary>
	/// Load saved state and change volume accordingly
	/// </summary>
	private void LoadSavedState()
	{
		isVolumeOn = PlayerPrefs.GetInt("isVolumeOn", 1) == 1;
		image.sprite = isVolumeOn ? onSprite : offSprite;
		AudioListener.volume = isVolumeOn ? 1 : 0;
	}

	/// <summary>
	/// Volume button function for toggling volume between on and off
	/// </summary>
	public void ToggleVolume()
	{
		isVolumeOn = !isVolumeOn;
		image.sprite = isVolumeOn ? onSprite : offSprite;
		AudioListener.volume = isVolumeOn ? 1 : 0;
		PlayerPrefs.SetInt("isVolumeOn", isVolumeOn ? 1 : 0);
	}
}