using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicAudioSource;

    private void Start()
    {
        TurnMusic(PlayerPrefs.GetInt("Music", 1) == 1);
    }

    public void TurnMusic(bool flag)
    {
        musicAudioSource.mute = !flag;
    }
}
