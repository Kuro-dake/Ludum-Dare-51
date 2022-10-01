using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IEnumRunner.Transitions;
using UnityEngine;

public class Platform : MonoBehaviour
{
    // Start is called before the first frame update
    public direction dir_in;
    public Vector3 position => transform.position;
    public bool is_next_platform => this == Game.next_platform;
    
    void Start()
    {
        BeatTracker.onBeat += OnBeat;
        Player.onFall += OnFall;
        transform.localScale = Vector3.one * .5f;
        GetComponent<SpriteRenderer>().color -= Color.black;
        transform.localPosition += Vector3.down;
        Make.The(gameObject).In(.3f).MoveBy(Vector3.up).ScaleTo(1f).AlphaTo(1f)
            .Happen();
    }

    void OnFall(object sender, object args)
    {
        DestroyPlatform();
    }

    private bool is_falling = false;
    void OnBeat(object sender, OnBeatArgs args)
    {
        if (args.validator.is_move_beat && !is_next_platform)
        {
            DestroyPlatform();
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyPlatform()
    {
        if (is_falling)
        {
            return;
        }

        is_falling = true;
        Make.The(gameObject).In(.5f).MoveBy(Vector3.down).ScaleTo(.5f).AlphaTo(0f)
            .then.MakeHappen(()=>Destroy(gameObject)).Happen();
        
    }

    private void OnDestroy()
    {
        BeatTracker.onBeat -= OnBeat;
    }
}
