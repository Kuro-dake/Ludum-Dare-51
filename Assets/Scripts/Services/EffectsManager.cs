using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectsManager", menuName = "Manager/EffectsManager", order = 0)]

public class EffectsManager : Service
{
    [SerializeField]
    List<NamedEffect> effects = new List<NamedEffect>();

    [SerializeField]
    List<NamedEffect> prefab_effects = new List<NamedEffect>();

    Dictionary<string, List<Effect>> instances = new Dictionary<string, List<Effect>>();

    Dictionary<object, List<Effect>> persistent_effects = new Dictionary<object, List<Effect>>();

    public override void GameStartInitialize()
    {
        base.GameStartInitialize();

        prefab_effects.Clear();

        foreach (Effect e in Resources.LoadAll<Effect>("Effects"))
        {
            prefab_effects.Add(new NamedEffect(e.name, e));
        }

    }
    public Effect this[string n]
    {
        get
        {
            Effect inst = null;

            if (instances.ContainsKey(n))
            {
                instances[n].RemoveAll(delegate (Effect e) { return e == null; });
                foreach (Effect e in instances[n])
                {
                    if (!e.is_playing)
                    {
                        inst = e;
                        break;
                    }
                }
            }
            if (inst == null)
            {
                inst = Instantiate(prefab_effects.Find(delegate (NamedEffect ne) { return ne.first == n; }).second);
                inst.transform.SetParent(service_transform);
                if (!instances.ContainsKey(n))
                {
                    instances.Add(n, new List<Effect>());
                }
                if (inst.reusable)
                {
                    instances[n].Add(inst);
                }

            }


            return inst;
        }
    }
}
[System.Serializable]
public class NamedEffect : Pair<string, Effect>
{
    public NamedEffect(string s, Effect e) : base(s, e) { }
}