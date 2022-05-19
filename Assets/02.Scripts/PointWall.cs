using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointWall : MonoBehaviour
{
    public GameObject[] Point;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //this.gameObject.SetActive(false);

            for(int i = 0; i < 1; i++)
            {
                Point[i].SetActive(false);
                Point[i + 1].SetActive(true);
                Point[i + 2].SetActive(true);
                Point[i + 3].SetActive(true);
            }
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
