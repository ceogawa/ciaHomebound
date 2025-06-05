using UnityEngine;

public class multipleTriggerDialogue : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip recordDialogue;

    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private static bool[] hasPlayed = new bool[3];
    public objectType oType;
    public enum objectType {
        crayon,
        mallet,
        record
    }

    void Start()
    {
        for (int i = 0; i < 3; i++){
            hasPlayed[i] = false;
        }
        audioSource = GetComponentInParent<AudioSource>();
        grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    void OnGrabbed(UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs args)
    {
        if (!hasPlayed[(int)oType])
        {
            if (oType == objectType.record)
            {
                audioSource.clip = recordDialogue;
            }
            audioSource.Play();
            hasPlayed[(int)oType] = true;
        }

    }
    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        }
    }

}
