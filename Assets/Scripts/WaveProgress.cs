using UnityEngine;

// A thin bar under the wave label that empties as the meteors of the wave are dealt with.
// The fill is stretched across the bar, and only its right anchor moves.
public class WaveProgress : MonoBehaviour
{
    [SerializeField] private RectTransform _fill;

    private void OnEnable()
    {
        WaveSpawner.ProgressChanged += Show;
    }

    private void OnDisable()
    {
        WaveSpawner.ProgressChanged -= Show;
    }

    private void Show(float remaining)
    {
        _fill.anchorMax = new Vector2(remaining, 1f);
    }
}
