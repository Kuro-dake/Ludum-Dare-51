using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    public abstract bool is_playing { get; }
    [SerializeField]
    public bool reusable;
    public virtual void Play(Vector2 position)
    {
        transform.position = position;
    }
    public virtual void Play(Vector2 position, float duration)
    {
        Play(position);
        StartCoroutine(LimitPlayStep(duration));
    }
    IEnumerator LimitPlayStep(float duration)
    {
        yield return new WaitForSeconds(duration);
        Stop();
    }
    public virtual void Stop() { }
    public IEnumerator WaitWhilePlaying()
    {
        while (is_playing)
        {
            yield return null;
        }
    }
}

