using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RatElectrocution : MonoBehaviour
{

    [SerializeField] private AudioClip normalSqueak;
    [SerializeField] private AudioClip wilhelmSqueak;
    [SerializeField] private AudioSource audioSource;
    [SerializeField][Range(0f, 1f)] private float wilhelmChance = 0.03f;

    // No need for zap prefab anymore since it's baked in
    [SerializeField] private GameObject smokePuffPrefab;

    [SerializeField] private Animator anim;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void TriggerZappyDeath()
    {
        StartCoroutine(ElectrocuteAndVanish());
    }

    public float TorqueRange = 15f;
    private IEnumerator ElectrocuteAndVanish()
    {
        // Release Z lock so the rat spins
        rb.constraints = RigidbodyConstraints2D.None;
        rb.velocity = Vector2.zero;

        // Apply randomized yeet force and spin
        rb.AddForce(new Vector2(Random.Range(-1f, 1f), 10f), ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-TorqueRange, TorqueRange), ForceMode2D.Impulse);

        // Trigger the baked-in zap animation
        if (anim != null)
            anim.SetTrigger("Zap");

        // Play the correct squeak sound
        AudioClip selectedClip = (Random.value < wilhelmChance) ? wilhelmSqueak : normalSqueak;
        if (selectedClip != null && audioSource != null)
            audioSource.PlayOneShot(selectedClip);

        // Let animation play through (adjust to match zap length)
        yield return new WaitForSeconds(0.4f);

        if (smokePuffPrefab)
            Instantiate(smokePuffPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }


}