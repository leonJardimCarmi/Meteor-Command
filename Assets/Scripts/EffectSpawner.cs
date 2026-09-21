using UnityEngine;

// Spawns a spark burst for the main game events: a meteor destroyed, a meteor hitting the ground
// and an interceptor reaching its target.
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
    }

    private void OnDisable()
    {
        Meteor.Destroyed -= OnMeteorDestroyed;
        Meteor.Impacted -= OnMeteorImpacted;
        Interceptor.Arrived -= OnInterceptorArrived;
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
