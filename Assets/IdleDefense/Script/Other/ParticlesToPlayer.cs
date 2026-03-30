using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesToPlayer : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private Transform target;
    [SerializeField] private float triggerTime = 2f;
    [SerializeField] private float attractSpeed = 5f;

    private ParticleSystem.Particle[] particles;
    private float timer;
    private bool attracting;
    private int count;

    private void Awake()
    {
        target = GameObject.Find("Player").transform;
        int max = ps.main.maxParticles;
        particles = new ParticleSystem.Particle[max];
    }
    

    private void Update()
    {
        count = ps.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            float remaining = particles[i].remainingLifetime;
            float totalLifetime = particles[i].startLifetime;

            // when particle is about to die
            if (remaining <= triggerTime * totalLifetime)
            {
                Vector3 dir = (target.position - particles[i].position).normalized;
                particles[i].position += dir * attractSpeed * Time.deltaTime;

                // OPTIONAL: keep particle alive longer so it can reach target
                float dist =  Vector3.Distance(particles[i].position, target.position);
                if (dist < 0.5f)
                {
                    particles[i].remainingLifetime = 0;
                }
                else
                {
                    particles[i].remainingLifetime += Time.deltaTime;
                }
                    
            }
        }

        ps.SetParticles(particles, count);
    }
}
