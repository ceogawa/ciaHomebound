using UnityEngine;

public class RecordPlayer : MonoBehaviour
{
    public AudioClip[] songs;
    public AudioSource audioSource;
    private int songIndex = 0;

    private bool playable;
    void Start()
    {
        // songIndex = Random.Range(0, songs.Length - 1);
        // audioSource.clip = songs[songIndex];
        playable = true;
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
        // if (!audioSource.isPlaying)
        // {
        //     ChangeSong();
        // }
    }

    void OnCollisionEnter(Collision collision)
    {
        // set playing to true if the collidable is a record
        // string objectName = otherObject.name;

        // check for RECORD collision
        GameObject otherObject = collision.gameObject;
        string objectName = otherObject.name;

        if (otherObject.tag == "Record")
        {
            if (otherObject.name == "record_johnny_cash" && playable == true){ audioSource.clip = songs[0]; }
            else if (otherObject.name == "record_elvis_dont_be_cruel" && playable == true){ audioSource.clip = songs[1]; }
            else if (otherObject.name == "record_carl_perkins" && playable == true) { audioSource.clip = songs[2]; }
            else if (otherObject.name == "record_isley_brothers" && playable == true) { audioSource.clip = songs[3]; }
            else{ return; }

            audioSource.Play();
            playable = false;
        }

    }

    void OnCollisionExit(Collision collision)
    {
        // set playing to false
        audioSource.Pause();
        playable = true;
    }
}
