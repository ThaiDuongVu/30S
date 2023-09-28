using System;
using UnityEngine;

public class Timer
{
    public readonly float MaxProgress;
    public float Progress;

    private readonly bool _isInfTimer;

    public Timer(float max)
    {
        MaxProgress = max;

        if (float.IsPositiveInfinity(MaxProgress)) _isInfTimer = true;
        else Progress = MaxProgress;
    }

    public void Reset()
    {
        Progress = MaxProgress;
    }

    public bool IsReachedUnscaled()
    {
        if (_isInfTimer) return false;

        Progress -= Time.fixedUnscaledDeltaTime;
        return Progress <= 0f;
    }

    public bool IsReached()
    {
        if (_isInfTimer) return false;

        Progress -= Time.fixedDeltaTime;
        return Progress <= 0f;
    }

    public override string ToString()
    {
        var total = Convert.ToInt32(Progress);
        var minute = total / 60;
        var second = total % 60;
        return $"{minute}:{(second < 10 ? "0" : "")}{second}";
    }
}