using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IEnumRunner;
using IEnumRunner.Transitions;
using UnityEngine;

public class GlyphSquare : MonoBehaviour
{
    private Sprite _sprite;

    public Sprite sprite
    {
        get => _sprite;
        set
        {
            _sprite = value;
            glyph_sr.sprite = value;
        }  
    }

    [SerializeField] private SpriteRenderer glyph_sr;
    
    // Start is called before the first frame update
    void Start()
    {
        Game.onGlyphVerify += OnGlyphVerifyHandler;
        BeatTracker.onBeat += OnBeat; 
        Make.The(gameObject).instantly.ScaleTo(Vector3.up).then.In(.25f).ScaleTo(Vector3.one).Happen();
        ChangeColor(Color.white - Color.black * .6f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        Game.onGlyphVerify -= OnGlyphVerifyHandler;
        BeatTracker.onBeat -= OnBeat;
    }

    void OnGlyphVerifyHandler(object sender, object args)
    {
        Disappear();
        ChangeColor(Color.white - Color.black);
    }

    private bool disappearing = false;

    void Disappear()
    {
        if (disappearing)
        {
            return;
        }

        disappearing = true;
        Make.The(gameObject).In(.2f).ScaleTo(Vector2.up).then.MakeHappen(()=>Destroy(gameObject, .3f)).Happen();
    }
    
    void OnBeat(object sender, OnBeatArgs args)
    {
        if (disappearing)
        {
            return;
        }
        GetComponentInChildren<MakeTransitions>().Trigger("blob");
        
    }
    
    private ColorGroups cg => GetComponentInChildren<ColorGroups>();

    private Sequence color_change_sequence;
    
    private void OnMouseEnter()
    {
        if (disappearing)
        {
            return;
        }
        ChangeColor(Color.white);
        Game.SelectGlyph(glyph_sr.sprite);
    }
    
    private void OnMouseExit()
    {
        if (disappearing)
        {
            return;
        }
        ChangeColor(Color.white - Color.black * .6f);
        
    }

    private Coroutine cc_routine;
    void ChangeColor(Color to)
    {
        if (cc_routine != null)
        {
            StopCoroutine(cc_routine);
        }
//        Debug.Log("change to " + to);
        cc_routine = StartCoroutine(ChangeColorStep(to));
    }

    [SerializeField] private float color_change_duration = .2f;
    IEnumerator ChangeColorStep(Color to)
    {
        float speed = 1f / color_change_duration;
        float t = 0f;
        Color start_color = cg.colors[0].second;
        
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            cg.colors[0].second = Color.Lerp(start_color, to, t);
            yield return null;
        }

        cc_routine = null;
    }
    
    private void OnMouseUpAsButton()
    {
        if (disappearing)
        {
            return;
        }
        Game.SelectGlyph(sprite);
    }
}
