using System;
using UnityEngine;

public class City : MonoBehaviour
{
    public static event Action Destroyed;

    public bool IsAlive => gameObject.activeSelf;

    public void Collapse()
    {
        gameObject.SetActive(false);
        Destroyed?.Invoke();
    }
}