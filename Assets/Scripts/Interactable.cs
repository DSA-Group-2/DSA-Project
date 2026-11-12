using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class Interactable : MonoBehaviour
{
    //Add or remove an InteractionEvent component to this gameObject.
    public bool useEvents;

    //This is a message displayed to the player when looking at an interactable.
    [SerializeField]
    public string promptMessage;

    public void BaseInteract()
    {
        if (useEvents)
        {
            GetComponent<InteractionEvent>().OnInteract.Invoke();

        }

        Interact();
    }

    protected virtual void Interact()
    {
        //This is a template function to be overridden by subclasses to define specific interaction behavior.
    }

}
