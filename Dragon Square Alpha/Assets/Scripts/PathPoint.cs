using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoint : MonoBehaviour
{
    public GameObject particlesPref;

    private void Start()
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = PlayerStats.secondaryColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Head"))
        {
            SpawnParticles(transform.position);
            Destroy(gameObject);
        }
    }

    private void SpawnParticles(Vector3 position)
    {
        GameObject particles = Instantiate(particlesPref, position, Quaternion.identity);
        ParticleSystem.MainModule mod = particles.GetComponent<ParticleSystem>().main;
        mod.startColor = PlayerStats.secondaryColor;

    }
}
