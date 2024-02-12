using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float can;
    public Slider canbar;
    public Transform particleSystemTransform; // Particle system'in bulunduğu objenin transformu
    public float maxDistanceToParticleSystem = 5f; // Yakınlık kontrolü için maksimum mesafe
    public bool PatlamaGerceklesti;

    void Start()
    {
        GuncelleCanBar();
    }

    void Update()
    {
        if (can <= 0)
        {
            can = 0; // Can 0'dan küçükse 0 olarak ayarla
            GuncelleCanBar();
            DuraklatOyun();
        }
    }

    public void Hasar(float amount)
    {
        if (can > 0)
        {
            float distanceToParticleSystem = Vector3.Distance(transform.position, particleSystemTransform.position);

            Debug.Log("Player ile particle system arasındaki mesafe: " + distanceToParticleSystem);

            if (distanceToParticleSystem < maxDistanceToParticleSystem)
            {
                OyuncuyaYakinliktaZararVer(50);
            }
            else
            {
                can -= amount;
                GuncelleCanBar();
            }
        }
    }

    void GuncelleCanBar()
    {
        canbar.value = can;
    }

    public void Yeniden()
    {
        can = 100;
        GuncelleCanBar();
    }

    void OyuncuyaYakinliktaZararVer(float damageAmount)
    {
        if (can > 0)
        {
            can -= damageAmount;
            GuncelleCanBar();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("patlamaAlani"))
        {
            // patlamaAlani içindeki testi objesini kontrol et
            Transform testiTransform = other.transform.Find("testi");

            if (testiTransform != null && !PatlamaGerceklesti)
            {
                
               

                PatlamaGerceklesti = true; // testi patladı

                // Sadece testi patladığında zombiyi öldür
                FireSystem.KillZombie();
                
            }
        }
    }



    void DuraklatOyun()
    {
        Time.timeScale = 0f; // Oyun zamanını duraklat
        
    }
}
