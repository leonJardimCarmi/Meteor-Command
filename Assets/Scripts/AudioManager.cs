using UnityEngine;

// Plays a sound for each game event. A clip slot left empty in the Inspector simply stays silent.
// Assigned clips have their leading silence removed and their length capped (see AudioTrim).
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] private float _volume = 0.6f;
    [SerializeField] private float _maxClipSeconds = 2f;

    [Header("Clips (optional)")]
    [SerializeField] private AudioClip _launch;
    [SerializeField] private AudioClip _blast;
    [SerializeField] private AudioClip _kill;
    [SerializeField] private AudioClip _impact;
    [SerializeField] private AudioClip _cityLost;
    [SerializeField] private AudioClip _waveStart;
    [SerializeField] private AudioClip _gameStart;
    [SerializeField] private AudioClip _gameOver;

    [Header("Music")]
    [SerializeField] private AudioClip _music;
    [SerializeField] [Range(0f, 1f)] private float _musicVolume = 0.3f;

    private AudioSource _source;
    private AudioSource _musicSource;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        PrepareClips();
    }

    private void OnEnable()
    {
        Battery.Fired += PlayLaunch;
        Interceptor.Arrived += PlayBlast;
        Meteor.Destroyed += PlayKill;
        Meteor.Impacted += PlayImpact;
        City.Destroyed += PlayCityLost;
        WaveSpawner.WaveStarted += PlayWaveStart;
        GameManager.GameStarted += PlayGameStart;
        GameManager.GameOver += PlayGameOver;
    }

    private void OnDisable()
    {
        Battery.Fired -= PlayLaunch;
        Interceptor.Arrived -= PlayBlast;
        Meteor.Destroyed -= PlayKill;
        Meteor.Impacted -= PlayImpact;
        City.Destroyed -= PlayCityLost;
        WaveSpawner.WaveStarted -= PlayWaveStart;
        GameManager.GameStarted -= PlayGameStart;
        GameManager.GameOver -= PlayGameOver;
    }

    private void Start()
    {
        StartMusic();
    }

    private void PrepareClips()
    {
        _launch = Prepare(_launch);
        _blast = Prepare(_blast);
        _kill = Prepare(_kill);
        _impact = Prepare(_impact);
        _cityLost = Prepare(_cityLost);
        _waveStart = Prepare(_waveStart);
        _gameStart = Prepare(_gameStart);
        _gameOver = Prepare(_gameOver, capLength: false);
    }

    private AudioClip Prepare(AudioClip assigned, bool capLength = true)
    {
        if (assigned == null)
        {
            return null;
        }

        float maxSeconds = capLength ? _maxClipSeconds : assigned.length;
        return AudioTrim.Prepare(assigned, maxSeconds);
    }

    private void StartMusic()
    {
        if (_music == null)
        {
            return;
        }

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.clip = _music;
        _musicSource.volume = _musicVolume;
        _musicSource.loop = true;
        _musicSource.Play();
    }

    private void StopMusic()
    {
        if (_musicSource != null)
        {
            _musicSource.Stop();
        }
    }

    private void Play(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        _source.PlayOneShot(clip, _volume);
    }

    private void PlayLaunch()
    {
        Play(_launch);
    }

    private void PlayBlast(Vector3 position)
    {
        Play(_blast);
    }

    private void PlayKill(Vector3 position, float size)
    {
        Play(_kill);
    }

    private void PlayImpact(Vector3 position, float size)
    {
        Play(_impact);
    }

    private void PlayCityLost()
    {
        Play(_cityLost);
    }

    private void PlayWaveStart(int wave)
    {
        // Wave 1 already has the game start sound.
        if (wave > 1)
        {
            Play(_waveStart);
        }
    }

    private void PlayGameStart()
    {
        Play(_gameStart);
    }

    private void PlayGameOver()
    {
        StopMusic();
        Play(_gameOver);
    }
}
