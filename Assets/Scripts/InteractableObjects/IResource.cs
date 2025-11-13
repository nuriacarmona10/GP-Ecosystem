using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResource
{
    string ResourceType { get; }
    float hydration { get; }
    float satiety { get; }

    float timeToConsumeIt { get; }

    public GameObject gameObjecto {  get; }  

    public void Consume();


}

