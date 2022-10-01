using UnityEngine;

[System.Serializable]
public class NamedAudioClip : Pair<string, AudioClip>
{
    public NamedAudioClip(string s, AudioClip a) : base(s, a) { }
}
