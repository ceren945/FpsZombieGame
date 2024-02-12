
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class zombiFollow : MonoBehaviour
{
    public Transform player;
    NavMeshAgent navMeshAgent;
    public static float health = 100;
    public static bool ZombieOlmeDenetle = false;

    // Can barını güncellemek için referans
    public Player playerScript;

    public float stoppingDistance = 2.0f;
    private float distanceToPlayer;
    public int eksilmeFrekansi = 2;
    private int canEksilmeSayaci = 0;
    public AudioClip deathAudioClip; // AudioClip olarak tanımlanan özellik
    private static AudioSource audioSource; // AudioSource özelliği

    void Start()
    {
        health = 100;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = 1.0f; //zombi hızı

        // AudioSource bileşenini ekle
        audioSource = gameObject.AddComponent<AudioSource>();

        
        audioSource.clip = deathAudioClip;

        // Player script'ini al
        playerScript = player.GetComponent<Player>();
    }

    void Update()
    {
        // Oyuncuya doğru hareket et
        if (health > 0)
        {
            navMeshAgent.destination = player.position;

            // Oyuncuya yaklaştığında can barından eksilt
            distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= stoppingDistance)
            {
                canEksilmeSayaci++;
                if (canEksilmeSayaci >= eksilmeFrekansi)
                {
                    playerScript.Hasar(5); // Player script'indeki Hasar fonksiyonunu çağırarak can barından eksilt
                    canEksilmeSayaci = 0; // Sayaç sıfırla
                }
            }
            // Küpün içerisinde olup olmadığını kontrol et
            GameObject patlamaAlani = GameObject.FindGameObjectWithTag("patlamaAlani");
          
            
            if (patlamaAlani != null)
            {
                if (transform.position.x >= patlamaAlani.transform.position.x - patlamaAlani.GetComponent<BoxCollider>().size.x &&
                    transform.position.x <= patlamaAlani.transform.position.x + patlamaAlani.GetComponent<BoxCollider>().size.x &&
                    transform.position.y >= patlamaAlani.transform.position.y - patlamaAlani.GetComponent<BoxCollider>().size.y &&
                    transform.position.y <= patlamaAlani.transform.position.y + patlamaAlani.GetComponent<BoxCollider>().size.y &&
                    transform.position.z >= patlamaAlani.transform.position.z - patlamaAlani.GetComponent<BoxCollider>().size.z &&
                    transform.position.z <= patlamaAlani.transform.position.z + patlamaAlani.GetComponent<BoxCollider>().size.z 
                    )
                {

                    // Zombiyi öldür
                    zombiFollow.Die(gameObject);
                }
            }
        }
    }

    // Hasar alındığında çağrılacak metod
    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        // Eğer canı sıfır veya daha azsa, zombiyi öldür
        if (health == 0)
        {
            zombiFollow.Die(gameObject);
        }
    }

    public static void Die(GameObject obj)
    {
        ZombieOlmeDenetle = true;
        // İsteğe bağlı: Ölüm animasyonları, ses efektleri veya diğer özel işlemleri burada ekleyebilirsiniz.
        Debug.Log("Zombi öldü!");

        // Ses çalma işlemi
        if (health == 0)
        {
            audioSource.Play();
        }

        // Zombiyi etkisiz hale getir
        Object.Destroy(obj);


    }
}
