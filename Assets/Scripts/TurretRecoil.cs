using System.Collections;
using UnityEngine;

// The turret kicks back along its barrel every time the battery fires and then slides forward again.
public class TurretRecoil : MonoBehaviour
{
    [SerializeField] private float _kickDistance = 0.3f;
    [SerializeField] private float _seconds = 0.2f;

    private Vector3 _restPosition;
    private Coroutine _routine;

    private void Awake()
    {
        _restPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        Battery.Fired += Kick;
    }

    private void OnDisable()
    {
        Battery.Fired -= Kick;
    }

    private void Kick()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
        }

        _routine = StartCoroutine(KickBack());
    }

    // The kick is biggest at the first moment and eases off, which feels like the snap of a real shot.
    private IEnumerator KickBack()
    {
        for (float progress = 0f; progress < 1f; progress += Time.deltaTime / _seconds)
        {
            float remaining = 1f - progress;
            Vector3 barrel = transform.localRotation * Vector3.forward;
            transform.localPosition = _restPosition - barrel * (_kickDistance * remaining * remaining);
            yield return null;
        }

        transform.localPosition = _restPosition;
        _routine = null;
    }
}
