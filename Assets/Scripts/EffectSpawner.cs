using UnityEngine;

// Spawns the visual feedback for the main game events: a spark burst when a meteor is destroyed, hits
// the ground or an interceptor arrives, and a floating score number for every kill.
public class EffectSpawner : MonoBehaviour
{
    [SerializeField] private Color _destroyedColor = new Color(1f, 0.55f, 0.15f);
    [SerializeField] private Color _impactColor = new Color(1f, 0.3f, 0.1f);
    [SerializeField] private Color _blastColor = new Color(0.2f, 0.9f, 1f);
    [SerializeField] private float _impactSizeMultiplier = 1.5f;
    [SerializeField] private float _blastSize = 1.5f;

    private void OnEnable()
    {
        Meteor.Destroyed += OnMeteorDestroyed;
        Meteor.Impacted += OnMeteorImpacted;
        Interceptor.Arrived += OnInterceptorArrived;
        GameManager.KillScored += OnKillScored;
    }

    private void OnDisable()
    {
        Meteor.Destroyed -= OnMeteorDestroyed;
        Meteor.Impacted -= OnMeteorImpacted;
        Interceptor.Arrived -= OnInterceptorArrived;
        GameManager.KillScored -= OnKillScored;
    }

    private void OnMeteorDestroyed(Vector3 position, float size)
    {
        Spawn(position, _destroyedColor, size);
    }

    private void OnMeteorImpacted(Vector3 position, float size)
    {
        Spawn(position, _impactColor, size * _impactSizeMultiplier);
    }

    private void OnInterceptorArrived(Vector3 position)
    {
        Spawn(position, _blastColor, _blastSize);
    }

    private void OnKillScored(Vector3 position, int points, int killNumber)
    {
        if (!PoolManager.Instance.HasPool(PoolType.Popup))
        {
            return;
        }

        GameObject popup = PoolManager.Instance.Get(PoolType.Popup, position, Quaternion.identity);
        popup.GetComponent<ScorePopup>().Show(points, killNumber);
    }

    private void Spawn(Vector3 position, Color color, float size)
    {
        if (!PoolManager.Instance.HasPool(PoolType.Effect))
        {
            return;
        }

        GameObject effect = PoolManager.Instance.Get(PoolType.Effect, position, Quaternion.identity);
        effect.GetComponent<ExplosionEffect>().Play(color, size);
    }
}
