using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IEnumRunner;
using IEnumRunner.Transitions;

using UnityEngine;
using UnityEngine.UI;

public class RequiredGlyphDisplay : MonoBehaviour
{
    private Sprite _sprite;
    public Sprite sprite
    {
        get => _sprite;
        set
        {
            _sprite = value;
            if (value == null)
            {
                Make.The(gameObject).In(.2f).ScaleTo(0f).Happen();
            }
            else
            {
                sr.sprite = value;
                transform.localScale = Vector3.zero;
                Make.The(gameObject).In(.2f).ScaleTo(orig_scale).Happen();
            }
        }
    }

    private SpriteRenderer sr => GetComponentInChildren<SpriteRenderer>();
    
    private Vector3 orig_scale;
    // Start is called before the first frame update
    void Start()
    {
        BeatTracker.onBeat += OnBeat;
        orig_scale = transform.localScale;
    }

    void OnBeat(object sender, object args)
    {
        return;
        float dur = .2f;
        Make.The(gameObject).In(dur * .5f).ScaleTo(Common.MultiplyVectorBy(orig_scale, new Vector3(1.2f, .8f))).then
            .In(dur*.5f).ScaleTo(orig_scale).Happen();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
