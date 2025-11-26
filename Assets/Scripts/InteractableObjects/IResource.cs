using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResource
{
    string ResourceType { get; }
    float Hydration { get; }
    float Satiety { get; }

    float InteractionDistance { get; }

    float TimeToConsumeIt { get; }

    public GameObject ResourceGameObject {  get; }

    public void DestroyGameobject();


}

