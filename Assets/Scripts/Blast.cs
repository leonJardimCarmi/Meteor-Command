using System.Collections;
using UnityEngine;

public class Blast : MonoBehaviour
{
    [SerializeField] private float _radius = 3f;
    [SerializeField] private float _expandTime = 0.35f;
    [SerializeField] private float _holdTime = 0.15f;
    [SerializeField] private float _shrinkTime = 0.35f;

    private WaitForSeconds _hold;
    private static int _nextId = 1;
    private int _kills;

    public int Id { get; private set; }

    private void Awake()
    {
        _hold = new WaitForSeconds(_holdTime);
    }

    private void OnEnable()
    {
        Id = _nextId++;
        _kills = 0;
        StartCoroutine(Detonate());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Meteor meteor) && meteor.Kill(Id))
        {
            RegisterKill();
        }
    }

    private void RegisterKill()
    {
        _kills++;
        GameManager.Instance.AddKill(_kills);
    }

    private IEnumerator Detonate()
    {
        yield return StartCoroutine(ChangeRadius(0f, _radius, _expandTime));
        yield return _hold;
        yield return StartCoroutine(ChangeRadius(_radius, 0f, _shrinkTime));
        PoolManager.Instance.Release(gameObject);
    }

    private IEnumerator ChangeRadius(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetRadius(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }

        SetRadius(to);
    }

    private void SetRadius(float radius)
    {
        transform.localScale = Vector3.one * (radius * 2f);
    }

}