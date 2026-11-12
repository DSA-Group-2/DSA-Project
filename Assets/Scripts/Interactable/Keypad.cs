using UnityEngine;

public class Keypad : Interactable
{
    [SerializeField]
    private GameObject Door;
    private bool doorOpen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Interact()
    {

        doorOpen =!doorOpen;
        Door.GetComponent<Animator>().SetBool("isOpened", doorOpen);


    }




}
