using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class BeatValidator
{

    public static BeatValidator GetValidator(int tempo, int beat_number)
    {
        switch (tempo)
        {

            case 2:
                return new BeatValidatorKiller(beat_number);
                break;
            
            case 6:
                return new BeatValidator6Tempo(beat_number);
                break;
            
            case 8:
                return new BeatValidator8Tempo(beat_number);
                break;
                
        }

        throw new Exception($"Non existent {tempo} tempo validator.");
    }
    
    protected int beat_number;
    public BeatValidator(int beat_number)
    {
        this.beat_number = beat_number;
    }
    public bool is_spawn_beat => spawn_beat && Game.countdown_over;
    public bool is_move_beat => move_beat && Game.countdown_over && Game.first_platform_created;
    
    public bool is_glyph_beat => glyph_beat && Game.countdown_over && Game.first_platform_created;
    public bool is_glyph_disappear_beat => glyph_disappear_beat && Game.countdown_over && Game.first_platform_created;

    public bool is_last_beat => beat_number % 8 == 7;
    
    protected virtual bool spawn_beat => beat_number % 8 == 0;
    protected abstract bool move_beat { get; }
    
    protected abstract bool glyph_beat { get; }
    protected abstract bool glyph_disappear_beat { get; }
    
}

public class BeatValidator6Tempo : BeatValidator
{
    public BeatValidator6Tempo(int beatNumber):base(beatNumber){}

    protected override bool move_beat => beat_number % 8 == 3;
    protected override bool glyph_beat => (beat_number % 8 == 1);
    protected override bool glyph_disappear_beat => (beat_number % 8) == 5;
}

public class BeatValidator8Tempo : BeatValidator
{
    
    public BeatValidator8Tempo(int beatNumber):base(beatNumber){}
    
    
    protected override bool move_beat => (beat_number % 8 == 4); // TODO check first platform created
    
    protected override bool glyph_beat => (beat_number % 8 == 2);
    protected override bool glyph_disappear_beat => (beat_number % 8) == 6;
}

public class BeatValidatorKiller : BeatValidator
{
    public BeatValidatorKiller(int beatNumber): base(beatNumber){}
    protected override bool spawn_beat => beat_number % 4 == 0; 
    protected override bool move_beat => (beat_number % 4 == 1); // TODO check first platform created
    
    protected override bool glyph_beat => (beat_number % 4 == 2);
    protected override bool glyph_disappear_beat => (beat_number % 4) == 3;
}