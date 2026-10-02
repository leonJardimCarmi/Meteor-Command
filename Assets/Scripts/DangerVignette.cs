using UnityEngine;
using UnityEngine.UI;

// While only one city is left, or the turret is on its last hit point, the edges of the screen pulse red. The soft red frame is
// drawn in code, so there is no picture file to import.
[RequireComponent(typeof(Image))]
public class DangerVignette : MonoBehaviour
{
    private const int TextureSize = 128;
    private const float ClearUntil = 0.55f;
    private const float FullAt = 1.35f;

    [SerializeField] private Color _color = new Color(0.9f, 0.05f, 0.05f);
    [SerializeField] [Range(0f, 1f)] private float _minAlpha = 0.25f;
    [SerializeField] [Range(0f, 1f)] private float _maxAlpha = 0.7f;
    [SerializeField] private float _pulsesPerSecond = 1.2f;

    private Image _image;
    private bool _oneCityLeft;
    private bool _turretOnLastHit;

    private void Awake()
    {
        // The glow changes every frame while it pulses. On a canvas of its own, only the glow has to be redrawn
        // for that, not the whole HUD with it.
        gameObject.AddComponent<Canvas>();

        _image = GetComponent<Image>();
        _image.sprite = ProceduralSprite.Create(TextureSize, EdgeAlpha);
        _image.raycastTarget = false;
        FillScreen();

        // First in the canvas, so the HUD is drawn on top of it.
        transform.SetAsFirstSibling();
        SetAlpha(0f);
    }

    private void OnEnable()
    {
        City.Destroyed += OnCityDestroyed;
        TurretHealth.HitPointsChanged += OnTurretHitPointsChanged;
        GameManager.GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        City.Destroyed -= OnCityDestroyed;
        TurretHealth.HitPointsChanged -= OnTurretHitPointsChanged;
        GameManager.GameOver -= OnGameOver;
    }

    private void Update()
    {
        bool isDanger = _oneCityLeft || _turretOnLastHit;

        if (!isDanger || !GameManager.Instance.IsPlaying)
        {
            SetAlpha(0f);
            return;
        }

        float pulse = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * _pulsesPerSecond * 2f * Mathf.PI);
        SetAlpha(Mathf.Lerp(_minAlpha, _maxAlpha, pulse));
    }

    private void OnCityDestroyed()
    {
        _oneCityLeft = CityManager.Instance.CountAlive() == 1;
    }

    private void OnTurretHitPointsChanged(int hitPoints, int maxHitPoints)
    {
        _turretOnLastHit = hitPoints == 1;
    }

    private void OnGameOver()
    {
        _oneCityLeft = false;
        _turretOnLastHit = false;
    }

    private void SetAlpha(float alpha)
    {
        Color color = _color;
        color.a = alpha;
        _image.color = color;
    }

    private void FillScreen()
    {
        RectTransform rect = (RectTransform)transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    // Solid in the corners and along the edges, clear in the middle.
    private static float EdgeAlpha(float offsetX, float offsetY)
    {
        float distance = Mathf.Sqrt(offsetX * offsetX + offsetY * offsetY);
        return Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(ClearUntil, FullAt, distance));
    }
}
