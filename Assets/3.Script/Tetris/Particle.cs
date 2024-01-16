using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [SerializeField] public ParticleSystem[] particles;
    private Vector3 offset = new Vector3(-0.5f, 0, -0.5f);

    public void PlayParticleEffect(int column)
    {
        transform.position = new Vector3(0,0, column) + offset;

        foreach(ParticleSystem particle in particles)
        {
            particle.Play();
        }
    }
}
