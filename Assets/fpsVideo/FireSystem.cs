using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FireSystem : MonoBehaviour
{
    static RaycastHit  hit;
    RaycastHit takeHit;
    public float reloadcooldown; // şarjor degisme sistemi
    public float AmmoInGun; //silah içindeki mermi
    public float AmmoInPocket; //cebimizdeki mermi sayisi
    public float AmmoMax; //silahın alabilecegi max mermi
    float AddableAmmo;//eklenebilir mermi sayisi
    float ReloadTimer;
    public TextMeshProUGUI AmmoCounter;
    public TextMeshProUGUI PocketAmmoCounter;
    public GameObject impactEffect;
    public GameObject RayPoint;
    public GameObject MainRayPoint;
    public CharacterController karakterController;
    Animator GunAnimset;
    public bool canFire;
    float gunTimer;
    public float gunCooldown;
    public ParticleSystem MuzzleFlash;
    AudioSource SesKaynak;
    public AudioClip FireSound;
    public float range; //range ne kadar verilirse o kadar uzaga ateş edilebilir.
    public float takeRange;//yerden ammo alma rangei
    public float respawnCooldown = 5f; // Ammo'nun tekrar spawnlanma süresi (saniye)
    float lastTakeHitTime; // Son Ammo'nun alındığı zaman
    public AudioClip ReloadSound;
    public GameObject cross;
    public GameObject Ammo_box; // ammo_box prefab'ını tutan değişken
    bool isAmmoHidden = false;
    int maxShotsToKillZombie = 10;
    int shotsFired = 0;
    float damagePerShot = 10f;
    public ParticleSystem patlamaEfekti;
    public AudioClip patlamaSesi;



    // Start is called before the first frame update
    void Start()
    {
        SesKaynak = GetComponent<AudioSource>();
        GunAnimset = GetComponent<Animator>();
        karakterController = GetComponent<CharacterController>();
        patlamaEfekti.Stop();
        SesKaynak.clip = patlamaSesi;
        if (karakterController == null)
        {
            Debug.LogError("CharacterController is null in Start.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(MainRayPoint.transform.position, MainRayPoint.transform.forward, out takeHit, takeRange))
        {

            if (takeHit.collider.gameObject.tag == "ammo")
            {



                cross.GetComponent<Image>().material.color = Color.red;



                if (Input.GetKeyDown(KeyCode.E))
                {
                    AmmoInPocket = AmmoInPocket + 60;
                    StartCoroutine(AmmoKutusunuGizle(takeHit.collider.gameObject));
                }
            }
            else
            {
                cross.GetComponent<Image>().material.color = Color.green;
            }
            // Kontrol et ve 120 saniye geçmişse Ammo'yu tekrar spawnla
            if (Time.time - lastTakeHitTime > respawnCooldown)
            {
                if (!isAmmoHidden)
                {
                    SpawnAmmo(takeHit.point);
                }
            }

            
        }

        Debug.Log("CharacterController value: " + karakterController);
        if (karakterController != null)
        {
            GunAnimset.SetFloat("hız", karakterController.velocity.magnitude);
            // Diğer kodlar...
        }
        else
        {
            Debug.LogError("CharacterController is null.");
        }


        AmmoCounter.text = AmmoInGun.ToString(); //text olarak yazar
        PocketAmmoCounter.text = AmmoInPocket.ToString();
        AddableAmmo = AmmoMax - AmmoInGun;
        if (AddableAmmo > AmmoInPocket)
        {
            AddableAmmo = AmmoInPocket;
        }
        if (Input.GetKeyDown(KeyCode.R) && AddableAmmo > 0 && AmmoInPocket > 0)
        {
            if (Time.time > ReloadTimer)
                StartCoroutine(Reload());
            ReloadTimer = Time.time + reloadcooldown;

        }



        if (Input.GetKey(KeyCode.Mouse0) && canFire == true && Time.time > gunTimer && AmmoInGun > 0)
        {
            Fire();
            gunTimer = Time.time + gunCooldown;
            shotsFired++;

        }


        zombiFollow zombie = hit.collider.GetComponent<zombiFollow>(); //ZombiFollowdan zombi can degerini cagırma
        if (zombiFollow.health==0)
        {
            KillZombie();

        }

    }

    void Fire()
    {

        AmmoInGun--;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range))
        {
            MuzzleFlash.Play();
            SesKaynak.Play();
            SesKaynak.clip = FireSound;
            Debug.Log(hit.transform.name);
            GunAnimset.Play("fire", -1, 0f);

            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            if (hit.collider.CompareTag("testi"))
            {
                // Patlama efektini instantiate et
                Instantiate(patlamaEfekti, hit.point, Quaternion.identity); //mermi testiye değince patlama efekti aktif olur.

                SesKaynak.PlayOneShot(patlamaSesi);
                // "testi" objesini yok et
                Destroy(hit.collider.gameObject);
            }
            // Eğer vurulan nesne bir zombi ise, zombiye hasar ver
            if (hit.collider.CompareTag("Zombi"))
            {
                impactEffect.SetActive(false);
                hit.collider.GetComponent<zombiFollow>().TakeDamage(damagePerShot); // Her bir mermi için 10 hasar // Hasar miktarını isteğinize göre ayarlayabilirsiniz
            }
            else
            {
                impactEffect.SetActive(true);
            } 
        }
    }
    IEnumerator Reload()
    {

        GunAnimset.SetBool("isReloading", true);

        SesKaynak.clip = ReloadSound;
        SesKaynak.Play();
        yield return new WaitForSeconds(0.3f);
        GunAnimset.SetBool("isReloading", false);
        yield return new WaitForSeconds(1.4f);
        AmmoInGun = AmmoInGun + AddableAmmo;
        AmmoInPocket = AmmoInPocket - AddableAmmo;
    }
    void SpawnAmmo(Vector3 spawnPosition)
    {
        // Ammo'nun prefab'ını kullanarak spawnla
        Instantiate(Ammo_box, spawnPosition, Quaternion.identity);

        // Yeniden spawnlandığı zamanı güncelle
        lastTakeHitTime = Time.time;
    }

    IEnumerator AmmoKutusunuGizle(GameObject ammoKutusu)
    {
        isAmmoHidden = true;
        ammoKutusu.SetActive(false); // Ammo kutusunu gizle
        yield return new WaitForSeconds(10f); // 10 saniye bekleyin
        ammoKutusu.SetActive(true); // Ammo kutusunu tekrar göster
        lastTakeHitTime = Time.time; // Ammo'nun son alındığı zamanı güncelle
        isAmmoHidden = false;
    }
     public static void KillZombie()
    {
        if (hit.collider.CompareTag("Zombi"))
        {
            zombiFollow.Die(hit.collider.gameObject);
        }
    }
}