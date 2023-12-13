using GoogleTextToSpeech.Scripts.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField] string actorName;
    [TextArea][SerializeField] string description;
    [SerializeField] VoiceScriptableObject voice;

    public string GetActorName() => actorName;

    public VoiceScriptableObject GetVoice() => voice;
}
