using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Reflection;

using UnityEngine.Experimental.Rendering.Universal;

public class LightningEffect : Effect
{
    bool _is_playing = false;
    bool is_fork = false;
    public override bool is_playing => _is_playing;

    public Color inner_color = Color.white, outer_color = Color.magenta;
    //TrailRenderer inner_line, outer_line;

    float _width = 1f;
    public float width { get { return _width; } set { _width = value; inner_color.a *= width; outer_color *= width; } }


    [SerializeField]
    float inner_width = 1f, outer_width = 1.5f, start_end_width_ratio = .2f;
    [SerializeField]
    FloatRange fork_distance_range = new FloatRange(), chaos_multiplier = new FloatRange();

    public Vector2 target;

    public int sorting_layer_order = 30;
    public string sorting_layer_name = "AboveCharacters";


    public float intensity = 1f, light_intensity_modifier = 1f;

    private void Update()
    {

    }

    public override void Play(Vector2 position)
    {
        base.Play(position);
        StartCoroutine(PlayStep(position));

    }
    LineRenderer _inner_line;
    LineRenderer inner_line { get { return _inner_line == null ? (_inner_line = gameObject.AddComponent<LineRenderer>()) : _inner_line; } }

    LineRenderer _outer_line;
    LineRenderer outer_line
    {
        get
        {
            if (_outer_line == null)
            {
                _outer_line = new GameObject("outer_line").AddComponent<LineRenderer>();
                _outer_line.transform.SetParent(transform);
            }
            return _outer_line;
        }
    }

    [SerializeField]
    Material material = null;
    IEnumerator PlayClone(Vector2 pos)
    {
        yield return null;
        Play(pos);
    }
    void InitLine()
    {
        inner_line.sortingLayerName = sorting_layer_name;
        inner_line.sortingOrder = sorting_layer_order + 1;
        inner_line.material = material;
        inner_line.startWidth = inner_width;
        inner_line.endWidth = inner_width * start_end_width_ratio;

        Color inner_c = inner_color;
        inner_c.a *= intensity;

        inner_line.startColor = inner_line.endColor = inner_c;

        outer_line.sortingLayerName = sorting_layer_name;
        outer_line.sortingOrder = sorting_layer_order;
        outer_line.material = material;
        outer_line.startWidth = outer_width;
        outer_line.endWidth = outer_width * start_end_width_ratio;

        Color outer_c = outer_color;
        outer_c.a *= intensity;

        outer_line.startColor = outer_line.endColor = outer_c;
    }

    void AddLinesPoint(Vector2 line_target)
    {
        LineFunctions.AddLinePoint(inner_line, line_target);
        LineFunctions.AddLinePoint(outer_line, line_target);
    }

    void RemoveFirstLinesPoint()
    {
        LineFunctions.RemoveFirstLinePoint(inner_line);
        LineFunctions.RemoveFirstLinePoint(outer_line);

    }



    [SerializeField]
    int remove_after_forks = 3, skip_yields = 0, skip_forks = 5;

    public bool lights = true;
    [System.NonSerialized]
    public int[] light_layers = null;

    void PlaceLight(Vector2 pos)
    {
        if (light_layers == null)
        {
            light_layers = new int[] {

                SortingLayer.NameToID("Default"),
                SortingLayer.NameToID("Grid"),
                SortingLayer.NameToID("Ground"),
                SortingLayer.NameToID("AboveGround"),
                SortingLayer.NameToID("InScene"),
                SortingLayer.NameToID("AboveCharacters"),
                SortingLayer.NameToID("UI"),

            };
        }
        if (!lights)
        {
            return;
        }
        Light2D llight = new GameObject("light").AddComponent<Light2D>();
        llight.lightType = Light2D.LightType.Point;
        llight.intensity = intensity * light_intensity_modifier;
        FieldInfo fieldInfo = llight.GetType().GetField("m_ApplyToSortingLayers", BindingFlags.NonPublic | BindingFlags.Instance);
        llight.pointLightOuterRadius = Random.Range(4f, 8f);
        llight.color = outer_color;

        fieldInfo.SetValue(llight, light_layers);
        llight.transform.position = pos;
        Destroy(llight.gameObject, .1f);
    }
    void CreateFork(Vector2 pos)
    {
        if (!create_forks)
        {
            return;
        }
        LightningEffect fork = Instantiate(gameObject).GetComponent<LightningEffect>();
        fork.transform.DestroyChildren();
        fork.inner_width = inner_width * .6f;
        fork.outer_width = outer_width * .6f;
        Destroy(fork.GetComponent<LineRenderer>());
        fork.target = pos + Random.insideUnitCircle.normalized * Random.Range(2f, 3f);
        fork.is_fork = true;
        fork.light_layers = light_layers;
        fork.StartCoroutine(fork.PlayClone(pos));
    }
    [SerializeField]
    float distance_limit = 2f;
    [SerializeField]
    bool create_forks = true;

    IEnumerator PlayStep(Vector2 origin)
    {
        _is_playing = true;
        int i = 0;
        InitLine();
        Vector2 current = origin;


        outer_line.positionCount = 1;
        outer_line.SetPositions(new Vector3[] { current });

        inner_line.positionCount = 0;


        float distance = Vector2.Distance(origin, target);
        float current_distance = 0f;
        LineFunctions.AddLinePoint(outer_line, current);

        while ((current_distance = Vector2.Distance(current, target)) > distance_limit)
        {
            Vector2 next_point = Vector2.MoveTowards(current, target, fork_distance_range);
            Vector2 chaos_modifier = Random.insideUnitCircle.normalized * chaos_multiplier * Mathf.InverseLerp(0f, distance, current_distance);
            if ((next_point - current).x.SignInt() != chaos_modifier.x.SignInt() && (next_point - current).y.SignInt() != chaos_modifier.y.SignInt())
            {
                if (Random.Range(0, 2) == 1)
                {
                    chaos_modifier.x *= -1;
                }
                else
                {
                    chaos_modifier.y *= -1;
                }
            }
            next_point += chaos_modifier;
            AddLinesPoint(next_point);
            if (i >= remove_after_forks)
            {
                RemoveFirstLinesPoint();
            }


            current = next_point;
            //Debug.Log(current);
            if (!is_fork && i % skip_forks == 0)
            {
                CreateFork(current);
            }
            if (i++ % skip_yields == 0)
            {
                PlaceLight(current);
                yield return null;// new WaitForSeconds(.5f);
            }

        }

        AddLinesPoint(target);

        while (inner_line.positionCount > 0 || outer_line.positionCount > 0)
        {
            RemoveFirstLinesPoint();
            if (i++ % skip_yields == 0)
            {
                yield return null;// new WaitForSeconds(.5f);
            }
        }
        _is_playing = false;
        if (is_fork || !reusable)
        {
            Destroy(gameObject);
        }
    }

}
