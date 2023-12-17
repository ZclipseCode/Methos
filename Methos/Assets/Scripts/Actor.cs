using GoogleTextToSpeech.Scripts.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField] string actorName;
    [TextArea][SerializeField] string description;
    [SerializeField] VoiceScriptableObject voice;

    ActorClass actorClass;

    private void Start()
    {
        actorClass = new ActorClass(actorName, description, voice);
    }

    public ActorClass GetActorClass() => actorClass;
}
