using System.Collections;
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
    [SerializeField] [Range(0f, 2f)] private float _blastVolume = 0.6f;
    [SerializeField] private AudioClip _blastAlt;
    [SerializeField] [Range(0f, 2f)] private float _blastAltVolume = 1.6f;
    [SerializeField] private AudioClip _kill;
    [SerializeField] [Range(0f, 0.5f)] private float _killPitchStep = 0.12f;
    [SerializeField] private AudioClip _impact;
    [SerializeField] private AudioClip _cityLost;
    [SerializeField] private AudioClip _waveStart;
    [SerializeField] private AudioClip _gameStart;
    [SerializeField] private AudioClip _gameOver;

    [Header("Reload (after every shot, while the battery reloads)")]
    [SerializeField] private AudioClip _reload;
    [SerializeField] [Range(0f, 2f)] private float _reloadVolume = 1f;
    [SerializeField] private float _reloadSoundDelay = 0.1f;
    [SerializeField] private float _reloadClipSeconds = 0.7f;

    [Header("Menu click (any click while not playing)")]
    [SerializeField] private AudioClip _menuClick;
    [SerializeField] [Range(0f, 2f)] private float _menuClickVolume = 1.8f;

    [Header("Music")]
    [SerializeField] private AudioClip _music;
    [SerializeField] [Range(0f, 1f)] private float _musicVolume = 0.3f;

    private AudioSource _source;
    private AudioSource _musicSource;
    private AudioSource _menuClickSource;
    private AudioSource _killSource;
    private bool _nextBlastIsAlt;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();

        // Pausing sets AudioListener.pause, which would otherwise silence a click made on the pause panel
        // itself. This one source is told to ignore that, so pause-screen clicks are still heard.
        _menuClickSource = gameObject.AddComponent<AudioSource>();
        _menuClickSource.ignoreListenerPause = true;

        // The kill sound rises in pitch, and a source has one pitch, so it gets a source of its own.
        _killSource = gameObject.AddComponent<AudioSource>();

        PrepareClips();
    }

    private void OnEnable()
    {
        Battery.Fired += PlayLaunch;
        Battery.Fired += PlayReload;
        Interceptor.Arrived += PlayBlast;
        GameManager.KillScored += PlayKill;
        Meteor.Impacted += PlayImpact;
        City.Destroyed += PlayLoss;
        TurretHealth.Hit += PlayLoss;
        WaveSpawner.WaveStarted += PlayWaveStart;
        GameManager.GameStarted += PlayGameStart;
        GameManager.GameOver += PlayGameOver;
    }

    private void OnDisable()
    {
        Battery.Fired -= PlayLaunch;
        Battery.Fired -= PlayReload;
        Interceptor.Arrived -= PlayBlast;
        GameManager.KillScored -= PlayKill;
        Meteor.Impacted -= PlayImpact;
        City.Destroyed -= PlayLoss;
        TurretHealth.Hit -= PlayLoss;
        WaveSpawner.WaveStarted -= PlayWaveStart;
        GameManager.GameStarted -= PlayGameStart;
        GameManager.GameOver -= PlayGameOver;
    }

    private void Start()
    {
        StartMusic();
    }

    // Any click anywhere makes this sound whenever the game is not actually being played (the main menu,
    // paused, or game over), so it is not tied to any one button and still stays silent during a run.
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !GameManager.Instance.IsPlaying)
        {
            PlayMenuClick();
        }
    }

    private void PrepareClips()
    {
        _launch = Prepare(_launch);
        _blast = Prepare(_blast);
        _blastAlt = Prepare(_blastAlt);
        _kill = Prepare(_kill);
        _reload = Prepare(_reload, _reloadClipSeconds);
        _impact = Prepare(_impact);
        _cityLost = Prepare(_cityLost);
        _waveStart = Prepare(_waveStart);
        _gameStart = Prepare(_gameStart);
        _gameOver = Prepare(_gameOver, capLength: false);
        _menuClick = Prepare(_menuClick);
    }

    private AudioClip Prepare(AudioClip assigned, bool capLength = true)
    {
        if (assigned == null)
        {
            return null;
        }

        float maxSeconds = capLength ? _maxClipSeconds : assigned.length;
        return Prepare(assigned, maxSeconds);
    }

    private AudioClip Prepare(AudioClip assigned, float maxSeconds)
    {
        return assigned == null ? null : AudioTrim.Prepare(assigned, maxSeconds);
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
        Play(_source, clip, 1f);
    }

    // The volume multiplier lets one loud or quiet clip be balanced against the rest without changing the
    // master volume that every other sound still shares.
    private void Play(AudioClip clip, float volumeMultiplier)
    {
        Play(_source, clip, volumeMultiplier);
    }

    private void Play(AudioSource source, AudioClip clip, float volumeMultiplier)
    {
        if (clip == null)
        {
            return;
        }

        source.PlayOneShot(clip, _volume * volumeMultiplier);
    }

    private void PlayLaunch()
    {
        Play(_launch);
    }

    // Alternates between the two blast clips, so the same bang is not heard every single time.
    private void PlayBlast(Vector3 position)
    {
        if (_nextBlastIsAlt)
        {
            Play(_blastAlt, _blastAltVolume);
        }
        else
        {
            Play(_blast, _blastVolume);
        }

        _nextBlastIsAlt = !_nextBlastIsAlt;
    }

    // Every further kill in the same blast sounds a little higher, so a combo climbs.
    private void PlayKill(Vector3 position, int points, int killNumber)
    {
        _killSource.pitch = 1f + _killPitchStep * (killNumber - 1);
        Play(_killSource, _kill, 1f);
    }

    // The reload sound starts just after the launch, so the two can be told apart.
    private void PlayReload()
    {
        if (_reload != null)
        {
            StartCoroutine(PlayReloadAfterDelay());
        }
    }

    private IEnumerator PlayReloadAfterDelay()
    {
        yield return new WaitForSeconds(_reloadSoundDelay);
        Play(_reload, _reloadVolume);
    }

    private void PlayImpact(Vector3 position, float size)
    {
        Play(_impact);
    }

    private void PlayLoss()
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

    private void PlayMenuClick()
    {
        Play(_menuClickSource, _menuClick, _menuClickVolume);
    }
}
