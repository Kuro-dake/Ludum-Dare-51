using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IEnumRunner.Transitions;
using Unity.Mathematics;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using TMPro;
public class Game : MonoBehaviour
{
    [SerializeField] private Platform platform_prefab;
    [SerializeField] private GlyphSquare glyphSquare_prefab;
    [SerializeField] private List<Pair<direction, Transform>> _direction_points;
    private PairList<direction, Transform> direction_points = new PairList<direction, Transform>();
    [SerializeField] private SpriteRenderer current_glyph_display;
    public static Game inst { get; protected set; }
    public static Transform GetDirectionTransform(direction dir) => inst.direction_points[dir];

    [SerializeField] private List<Sprite> glyph_sprites = new List<Sprite>();
    
    public static Platform GetDirectionPlatform(direction dir) => ReferenceEquals(next_platform, null) ? null : (next_platform.dir_in == dir ? next_platform : null);
    public static bool IsPlatformInDirection(direction dir) => ReferenceEquals(next_platform, null) ? false : next_platform.dir_in == dir;
    Pair<direction, Transform> GetRandomDirection() => direction_points.RandomElement();

    
    private static int _spawn_platform_every_nth_beat = 0;
    private static int _spawn_platform_every_nth_beat_half = 0;
    public static int spawn_platform_every_nth_beat_half => _spawn_platform_every_nth_beat_half; 
    public static int spawn_platform_every_nth_beat { get => _spawn_platform_every_nth_beat;
        protected set
        {
            _spawn_platform_every_nth_beat = value;
            _spawn_platform_every_nth_beat_half = value / 2;
            if (value % 2 == 1)
            {
                throw new Exception("The beat divider has to be an even number");
            }
        } 
    }
    // Start is called before the first frame update
    void Start()
    {
        inst = this;
        direction_points.AddRange(_direction_points);
        BeatTracker.onBeat += OnBeat;
        spawn_platform_every_nth_beat = 6;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static event System.EventHandler onGlyphVerify;

    public static Platform next_platform { get; protected set; }
    public static bool first_platform_created { get; protected set; }

    void ShowGlyph()
    {
        required_glyph = glyph_sprites.RandomElement();
    }
    [SerializeField] private TextMeshProUGUI countdown_text;
    private MakeTransitions cdt_transitions => countdown_text.transform.parent.GetComponent<MakeTransitions>();
    int _start_countdown = 5;
    public static int start_countdown { get=>inst._start_countdown;
        protected set
        {
            inst._start_countdown = value;
            inst.countdown_text.text = "" + value;

        } }

    public bool _game_started;

    public static bool game_started
    {
        get => inst._game_started;
        set => inst._game_started = value;
    }

    public static bool countdown_over => start_countdown < 1;
    void OnBeat(object sender, OnBeatArgs args)
    {
        if (!countdown_over && game_started)
        {
            if (args.GetBeatNumber() % 2 == 0)
            {
                start_countdown -= 1;                    
            }
                
            if (start_countdown > 0)
            {
                cdt_transitions.Trigger("blob");                    
            }
            else
            {
                cdt_transitions.Trigger("disappear");
            }
        }
        if (args.validator.is_spawn_beat)
        {
            first_platform_created = true;
            next_platform = Instantiate(platform_prefab);

            var dir = GetRandomDirection();

            next_platform.dir_in = dir.first;

            next_platform.transform.position = dir.second.position;
            ShowGlyph();
        }

        if (args.validator.is_glyph_beat)
        {
            int glyph_num = 2;
            List<Sprite> distribute_glyphs = new List<Sprite>(glyph_sprites);
            distribute_glyphs.Remove(required_glyph);
            distribute_glyphs.Shuffle();
            distribute_glyphs.Insert(0, required_glyph);

            Queue<Sprite> glyph_queue = new Queue<Sprite>(distribute_glyphs);
            
            List<int> positions = new List<int>() { 0, 1, 2, 3 };
            positions.Shuffle();
            Queue<int> pos_queue = new Queue<int>(positions);

            positions = new List<int>();
            for (int i = 0; i < glyph_num; i++)
            {
                positions.Add(pos_queue.Dequeue());
            }

            List<Sprite> used_glyphs = new List<Sprite>();
            foreach (int i in positions)
            {
                GlyphSquare gs = Instantiate(glyphSquare_prefab);
                
                Sprite used = glyph_queue.Dequeue();

                gs.sprite = used;
                
                used_glyphs.Add(used);
                
                gs.transform.SetParent(glyph_parent);
                gs.transform.localPosition = Vector3.zero;
                gs.transform.localRotation = Quaternion.Euler(0f,0f,90f * i);
            }

            
        }

        if (args.validator.is_glyph_disappear_beat)
        {
            if (selected_glyph != required_glyph)
            {
                Player.inst.Die();
            }

            selected_glyph = null;
            required_glyph = null;
            onGlyphVerify?.Invoke(this, null);
        }
        
    }

    private Transform glyph_parent => Camera.main.transform.Find("glyph_parent");
    private Sprite _required_glyph;
    private Sprite required_glyph
    {
        get => _required_glyph;
        set
        {
            _required_glyph = value;
            current_glyph_display.sprite = value;
            if (ReferenceEquals(value, null))
            {
                
            }
        }
    }

    private Sprite selected_glyph;
    public static void SelectGlyph(Sprite s)
    {
        inst.selected_glyph = s;
        
    }
    
    public void SetPosToNextPlatform()
    {
        transform.position = next_platform.position;
    }
    
}

public enum direction
{
    up,
    down,
    left,
    right,
    none
}