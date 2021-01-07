using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    public GameObject particlesPref;
    public WayPointStats stats; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Head") && ((!stats.firstWaypoint && stats.previousWaypoint == null) || stats.firstWaypoint))
        {
            SpawnParticles(transform.position);
            AudioManager.PlaySound(AudioManager.Sound.Reach);
            Destroy(gameObject);
        }
    }

    private void SpawnParticles(Vector3 position)
    {
        GameObject particles = Instantiate(particlesPref, position, Quaternion.identity);
        ParticleSystem.MainModule mod = particles.GetComponent<ParticleSystem>().main;
        Color color = transform.GetChild(1).GetComponent<SpriteRenderer>().color;
        color.a = 1f;
        mod.startColor = color;

    }

    public class WayPointStats 
    {
        public bool firstWaypoint; 
        public GameObject previousWaypoint;

        public WayPointStats(bool firstWaypoint, GameObject previousWaypoint)
        {
            this.firstWaypoint = firstWaypoint;
            this.previousWaypoint = previousWaypoint;
        }
    }
}
