using UnityEngine;


public class RandomMeteorSpin : MonoBehaviour
{
    [SerializeField]
    private float minTumbleSpeed = 20f; 

    [SerializeField]
    private float maxTumbleSpeed = 60f; 

    private Vector3 _tumbleAxis;
    private float _tumbleSpeed;

    private void OnEnable()
    {
        transform.rotation = Random.rotation;
        _tumbleAxis = Random.onUnitSphere;
        _tumbleSpeed = Random.Range(minTumbleSpeed, maxTumbleSpeed);
    }

    private void Update()
    {
        transform.Rotate(_tumbleAxis, _tumbleSpeed * Time.deltaTime, Space.World);
    }
}