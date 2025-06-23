using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class XylophoneKey : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        // TODO maybe flag for collision exit so only one play per collision

        // check for mallet collision
        GameObject otherObject = collision.gameObject;

        // Access properties of the colliding object
        string objectName = otherObject.name;
        string objectTag = otherObject.tag;

        // Check if we collided with a specific object (e.g., "PowerCube")
        if (otherObject.tag == "Mallet")
        {
            // if (collision.relativeVelocity.magnitude > 2)
            // if(collision.)
            audioSource.Play();
            Debug.Log("Collided with: " + objectName);
        }
       
    }

    void OnCollisionExit(Collision collision){
        
    }
}
