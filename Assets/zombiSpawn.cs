using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static UnityEngine.UIElements.VisualElement;

public class zombiSpawn : MonoBehaviour
{

   
    public GameObject Zombie; 
    
    void Start()
    {
        GameObject yeniZombie = Instantiate(Zombie, new Vector3(21.0558224f, 0.992489576f, 3.57288933f), Quaternion.identity);
    }

    
    void Update()
    {
        if (zombiFollow.ZombieOlmeDenetle == true)
        {
            Invoke("zombiSpawnla", 2f);

            zombiFollow.ZombieOlmeDenetle = false;

        }
    }

    void zombiSpawnla() 
    {
       
        GameObject yeniZombie = Instantiate(Zombie,new Vector3(21.0558224f, 0.992489576f, 3.57288933f), Quaternion.identity);


        
       
    }
}
