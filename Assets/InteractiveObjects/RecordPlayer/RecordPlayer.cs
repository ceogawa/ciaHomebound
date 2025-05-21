using UnityEngine;

public class RecordPlayer : MonoBehaviour
{
    public AudioClip[] songs;
    public AudioSource audioSource;
    private int songIndex = 0;
    void Start()
    {
        songIndex = Random.Range(0, songs.Length - 1);
        audioSource.clip = songs[songIndex];

    }

    // Update is called once per frame
    void ChangeSong()
    {
        //songIndex = (songIndex + 1) % songs.Length;  // Cycle through the list
        songIndex = Random.Range(0, songs.Length - 1);
        audioSource.clip = songs[songIndex];
        audioSource.Play();
    }
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            ChangeSong();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // set playing to true if the collidable is a record
        // string objectName = otherObject.name;
    }

    void OnCollisionExit(Collision collision)
    {
        // set playing to false
        
    }
}
