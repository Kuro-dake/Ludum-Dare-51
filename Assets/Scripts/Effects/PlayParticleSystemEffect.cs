using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleSystemEffect : Effect
{
    public override bool is_playing
    {
        get
        {
            return ps.isPlaying;
        }
    }
    protected ParticleSystem ps { get { return GetComponent<ParticleSystem>(); } }

    public override void Play(Vector2 position)
    {
        base.Play(position);
        ps.Play();
        if (destroy_on_not_playing)
        {
            StartCoroutine(WaitToStop());
        }
    }

    public override void Stop()
    {
        base.Stop();
        ps.Stop();
    }

    [SerializeField]
    bool destroy_on_not_playing = false;

    IEnumerator WaitToStop()
    {
        while (is_playing)
        {
            yield return null;
        }

        Destroy(gameObject);
    }
}
