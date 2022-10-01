using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IEnumRunner;
using IEnumRunner.Transitions;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 orig_scale;
    public static Player inst;
    void Start()
    {
        BeatTracker.onBeat += OnBeat;
        inst = this;
        orig_scale = transform.localScale;
    }

    public static event System.EventHandler onFall;
    
    private float mul = 45f / 8f;

    void OnArrive()
    {
        if (!landed_on_platform)
        {
            Die();
        }
        else
        {
            Game.inst.SetPosToNextPlatform();
            current_direction = direction.none;
        }
        controls_on = true;
    }

    private bool landed_on_platform = false;
    
    private Vector3 move_to_position => Game.GetDirectionTransform(current_direction).position;
    void OnBeat(object sender, OnBeatArgs args)
    {
        SC.waddler.Waddle(false);
        
        if (args.validator.is_move_beat)
        {
            if (current_direction != direction.none)
            {
                landed_on_platform = Game.IsPlatformInDirection(current_direction);
                Make.The(gameObject).In(BeatTracker.beat_time).MoveTo(move_to_position).
                    then.MakeHappen(OnArrive).Happen();
                movement_indicator.transform.position = transform.position;
                
                controls_on = false;
                             
                
            }
            else
            {
                Die();
            }
            
        }
    }

    private bool controls_on = true;

    public void Die()
    {
//        Debug.Log("Died");
        Game.inst.transform.position = transform.position;
        onFall?.Invoke(this, null);
    }

    //private Platform next_move_to_platform;
    private direction current_direction = direction.none;
    
    private bool nps;

    private bool next_position_set
    {
        get => nps;
        set
        {
            nps = value;
            Debug.Log($"set to {value}");
        }
    }

    private Dictionary<KeyCode, direction> keycode_directions = new Dictionary<KeyCode, direction>()
    {
        { KeyCode.W, direction.up },
        { KeyCode.A, direction.left },
        { KeyCode.S, direction.down },
        { KeyCode.D, direction.right }
    };

    private Transform movement_indicator => transform.Find("MovementIndicator");
    public float val = .7f, dur = .1f,vy = 1.2f ;
    // Update is called once per frame
    private bool _rotation_right;
    private Sequence rotation_sequence;
    private bool rotation_right
    {
        get => _rotation_right;
        set
        {
            if (value == _rotation_right)
            {
                return;
            }
            _rotation_right = value;
            rotation_sequence?.Stop();
            SpriteRenderer sr = transform.Find("bunni").GetComponent<SpriteRenderer>();

            Make.The(gameObject).In(dur)
                .ScaleTo(Common.MultiplyVectorBy(new Vector2(val, vy), orig_scale))
                .then.MakeHappen(() => sr.flipX = value)
                .ScaleTo(orig_scale).Happen();
        }
    }
    void Update()
    {

        if (!controls_on)
        {
            return;
        }
        foreach (KeyValuePair<KeyCode, direction> kv in keycode_directions)
        {
            if (Input.GetKeyDown(kv.Key))
            {
                Platform t = Game.GetDirectionPlatform(kv.Value);
                if (kv.Key == KeyCode.A)
                {
                    rotation_right = false;
                }
                else if (kv.Key == KeyCode.D)
                {
                    rotation_right = true;
                }
                movement_indicator.position = Game.GetDirectionTransform(kv.Value).position;
                current_direction = kv.Value;
                
                
                break;
            }

            
        }
    }
    
    
}
