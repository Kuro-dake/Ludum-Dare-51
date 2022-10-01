using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IEnumRunner;
using UnityEngine;

public class PingEffect : Effect
{
    public override bool is_playing => s != null && s.is_running;
    Sequence s;
    public Color color;
    public FloatRange ping_duration = new FloatRange(.3f,.6f);
    public FloatRange away_scale = new FloatRange(.9f,1.3f);
    public FloatRange rotation;
    public float delay_between_pings;
    public int delay_each_nth_ping = 1;
    public int ping_number = 1;
    public override void Play(Vector2 position)
    {
        int current_ping_number = ping_number;
        s = Sequence.New();
        for(int i = 0; i<ping_number; i++)
        {
            s += () => Debug.Log("lorem"); //SC.ui.PingPosition(position, color, ping_duration, away_scale, rotation * Common.EitherOr());
            if(i%delay_each_nth_ping == 0)
            {
                s += delay_between_pings;

            }
        }
        
        s.Run();
    }

}
