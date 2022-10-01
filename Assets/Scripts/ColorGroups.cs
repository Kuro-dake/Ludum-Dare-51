using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColorGroups : MonoBehaviour
{

    [SerializeField]
    public List<Pair<List<SpriteRenderer>, Color>> colors;
    List<Pair<List<SpriteRenderer>, Color>> previous_tick_colors;
    [SerializeField]
    List<Pair<SpriteRenderer, Color>> orig_colors;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }
    bool initialized = false;
    public void Initialize()
    {
        if (initialized)
        {
            return;
        }
        initialized = true;
        orig_colors = new List<Pair<SpriteRenderer, Color>>();
        colors.ForEach(l => l.first.ForEach(sr => orig_colors.Add(Pair.New(sr, sr.color))));
        previous_tick_colors = new List<Pair<List<SpriteRenderer>, Color>>(colors);
        colors.ForEach(p => previous_tick_colors.Add(Pair.New(p.first, p.second)));
        UpdateColors(true);
    }
    public void UpdateColors(bool force = false)
    {

        foreach (Pair<List<SpriteRenderer>, Color> p in previous_tick_colors)
        {
            Pair<List<SpriteRenderer>, Color> cp = colors.Find(ccp => ccp.first == p.first);
            if (cp.second != p.second || force)
            {
                cp.first.ForEach(sr => sr.color = orig_colors.Find(oc => oc.first == sr).second * cp.second);

                p.second = cp.second;
            }


        }
    }
    // Update is called once per frame
    void Update()
    {
        UpdateColors();
    }
}
