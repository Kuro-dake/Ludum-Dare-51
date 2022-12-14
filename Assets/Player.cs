using System;
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

    public bool out_of_hp => hp < 0; 
    public int hp
    {
        get => hpbar.hp;
        set => hpbar.hp = value;
    }
    [SerializeField]  HPBar hpbar;
    public static event System.EventHandler onFall;
    
    private float mul = 45f / 8f;

    void OnArrive()
    {
        waddler.jumping = false;
        if (!landed_on_platform)
        {
            Fall();
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
                if (!landed_on_platform)
                {
                    Game.DestroyGlyphs();
                }

                waddler.jumping = true;
                Make.The(gameObject).In(BeatTracker.beat_time).MoveTo(move_to_position).
                    then.MakeHappen(OnArrive).Happen();
                movement_indicator.PlaceAt(direction.none);
                
                controls_on = false;
                             
                
            }
            else
            {
                Fall();
                Game.DestroyGlyphs();
            }
            
        }
    }

    private bool controls_on = true;
    public Waddler waddler => GetComponentInChildren<Waddler>();
    public bool fallen = false;
    public void Fall()
    {
        if (!Game.story_mode)
        {
            fallen = true;            
        }
        
        Debug.Log($"fell at {DateTime.Now.Second}");
        Game.inst.transform.position = transform.position;
        hp--;
        Make.The(gameObject).In(.35f).MoveBy(Vector3.down * 2f).RotateBy(270f * Common.EitherOr())
            .ScaleTo(orig_scale * .4f).then.MakeHappen(() =>
            {
                //EnvManager.cam_follow_player = false;
                transform.localRotation = Quaternion.identity;
                if (!out_of_hp)
                {
                    Game.inst.SpawnPlatform(direction.none,1);                    
                }
                
            }).then
            .ScaleTo(orig_scale).MoveTo(Game.inst.transform.position).then.//MakeHappen(()=>EnvManager.cam_follow_player = true).
            Happen();
        
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

    [SerializeField] private MovementIndicator movement_indicator;
    public float val = .7f, dur = .1f,vy = 1.2f ;
    // Update is called once per frame
    private bool _rotation_right = true;
    private Sequence rotation_sequence;
    public bool rotation_right
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
            List<SpriteRenderer> srs = GetComponentsInChildren<SpriteRenderer>().ToList();

            Make.The(gameObject).In(dur)
                .ScaleTo(Common.MultiplyVectorBy(new Vector2(val, vy), orig_scale))
                .then.MakeHappen(() => srs.ForEach(sr=>sr.flipX = value))
                .ScaleTo(orig_scale).Happen();
        }
    }
    void Update()
    {

        if (!controls_on || !Game.game_started)
        {
            return;
        }
        foreach (KeyValuePair<KeyCode, direction> kv in keycode_directions)
        {
            if (Input.GetKeyDown(kv.Key))
            {
                Platform t = Game.GetDirectionPlatform(kv.Value);
                if (kv.Key == KeyCode.A || kv.Key == KeyCode.S)
                {
                    rotation_right = false;
                }
                else if (kv.Key == KeyCode.D || kv.Key == KeyCode.W)
                {
                    rotation_right = true;
                }
                movement_indicator.PlaceAt(kv.Value);
                current_direction = kv.Value;
                
                
                break;
            }

            
        }
    }
    
    
}
