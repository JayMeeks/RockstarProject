using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySfxClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource _audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        
        _audioSource.clip = audioClip;
        _audioSource.volume = volume;
        _audioSource.Play();
        
        float _clipLength = _audioSource.clip.length;
        
        Destroy(_audioSource.gameObject, _clipLength);
    }
    
    public void PlayRandomSfxClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        int _rand = Random.Range(0, audioClip.Length); 
        
        AudioSource _audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        
        _audioSource.clip = audioClip[_rand];
        _audioSource.volume = volume;
        _audioSource.Play();
        
        float _clipLength = _audioSource.clip.length;
        
        Destroy(_audioSource.gameObject, _clipLength);
    }
}
