using UnityEngine;

public class PlayVoiceLine : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip voiceLine;

    private bool hasPlayed = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasPlayed)
            return;

        if (other.CompareTag("Player"))
        {
            if (audioSource != null && voiceLine != null)
            {
                audioSource.PlayOneShot(voiceLine);
            }

            hasPlayed = true;
        }
    }
}