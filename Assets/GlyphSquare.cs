using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    private bool mouse_over;
    public Sprite selected_glyph => mouse_over ? sprite : null;
    private void OnMouseEnter()
    {
        if (disappearing)
        {
            return;
        }
        ChangeColor(Color.white);
        mouse_over = true;
        Game.SelectGlyph(this);
    }
    
    private void OnMouseExit()
    {
        if (disappearing)
        {
            return;
        }

        mouse_over = false;
        ChangeColor(Color.white - Color.black * .6f);
        
    }

    public void Collect()
    {
        glyph_sr.transform.SetParent(null);
        cg.colors[0].first.Remove(glyph_sr);
        Make.The(glyph_sr).In(.4f).MoveTo(Player.inst.transform.position).RotateBy(540f * Common.EitherOr()).AlphaTo(0f)
            .ScaleTo(glyph_sr.transform.localScale * .6f).then.MakeHappen(() => Destroy(glyph_sr.gameObject)).Happen();
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
    
}
