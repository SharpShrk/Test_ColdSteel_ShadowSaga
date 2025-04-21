using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    private float _defaultFixedDeltaTime;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        _defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void ActivateSlowdown(float slowdownFactor, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(SlowdownRoutine(slowdownFactor, duration));
    }

    private IEnumerator SlowdownRoutine(float factor, float duration)
    {
        Time.timeScale = factor;
        Time.fixedDeltaTime = _defaultFixedDeltaTime * factor;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = _defaultFixedDeltaTime;
    }
}