using UnityEngine;

public class RecordPlayer : MonoBehaviour
{
    public AudioClip[] songs;
    public AudioClip scratch;
    public AudioSource audioSource;
    private int songIndex = 0;
    public float rotationSpeed = 100f;

    private GameObject curRecord;

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
        if (playable == false && curRecord != null) {
            curRecord.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
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
            audioSource.clip = scratch;
            audioSource.Play();

            if (otherObject.name == "record_johnny_cash" && playable == true) { audioSource.clip = songs[0]; }
            else if (otherObject.name == "record_elvis_dont_be_cruel" && playable == true) { audioSource.clip = songs[1]; }
            else if (otherObject.name == "record_carl_perkins" && playable == true) { audioSource.clip = songs[2]; }
            else if (otherObject.name == "record_isley_brothers" && playable == true) { audioSource.clip = songs[3]; }
            else { return; }

            audioSource.Play();
            // assign current record
            curRecord = otherObject;
            Vector3 worldPosition = new Vector3(2.1f, 0.69f, 0.31f);
            curRecord.transform.position = worldPosition;
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
