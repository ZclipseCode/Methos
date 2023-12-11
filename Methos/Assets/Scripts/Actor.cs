using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField] string actorName;
    [TextArea][SerializeField] string description;

    public string GetActorName() => actorName;
}
