using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    [Header("Slowdown Settings")]
    [SerializeField] private float _slowdownFactor = 0.3f;
    [SerializeField] private float _slowdownDuration = 2f;

    private float _defaultFixedDeltaTime;

    private void Awake()
    {
        _defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void ActivateSlowdown()
    {
        StopAllCoroutines();
        StartCoroutine(SlowdownRoutine());
    }

    private IEnumerator SlowdownRoutine()
    {
        Time.timeScale = _slowdownFactor;
        Time.fixedDeltaTime = _defaultFixedDeltaTime * _slowdownFactor;

        yield return new WaitForSecondsRealtime(_slowdownDuration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = _defaultFixedDeltaTime;
    }
}