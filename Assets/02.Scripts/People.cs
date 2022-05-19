using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class People : MonoBehaviour
{
    public Animator anim;
    public float speed = 0.3f;

    float creatTime = 40;
    float currentTime2;
    float creatTime2 = 32f;
    private Transform tr;

    float randomCurrentNum;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        tr = GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        randomCurrentNum = Random.Range(1, 20);

    }

    // Update is called once per frame
    void Update()
    {
        randomCurrentNum += Time.deltaTime;
        
        if(randomCurrentNum >= creatTime)
        {
            anim.SetBool("Move", true);
            Vector3 moveVec = transform.forward;
            tr.position += (moveVec * speed) * Time.deltaTime;

            currentTime2 += Time.deltaTime;
            if ( currentTime2 >= creatTime2)
            {
                anim.SetBool("Move", false);
                moveVec = Vector3.zero;
                tr.eulerAngles += new Vector3(0, 180, 0);
                randomCurrentNum = 0;
                Invoke("Turn", 0.5f);
            }
        }
    }

    void Turn()
    {
        currentTime2 = 0;
        randomCurrentNum = Random.Range(1, 20);
    }
}
