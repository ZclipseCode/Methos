using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleTextToSpeech.Scripts.Data;

public class ActorClass
{
    string actorName;
    string description;
    VoiceScriptableObject voice;

    public ActorClass(string actorName, string description, VoiceScriptableObject voice)
    {
        ActorName = actorName;
        Description = description;
        Voice = voice;
    }

    public string ActorName
    {
        get => actorName;
        set => actorName = value;
    }

    public string Description
    {
        get => description;
        set => description = value;
    }

    public VoiceScriptableObject Voice
    {
        get => voice;
        set => voice = value;
    }
}
