using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CarRay : Agent
{
    /*
     * 1. 주변환경을 관측 (Observations)
     * 2. 정책에 의한 행동 (Actions)
     * 3. 행동에 따른 보상 (Reward)
     */

    // 관측 종류
    /*
     * - Rigidbody Velocity
     * - 타겟과의 거리
     * - 자신의 위치
     */

    private Transform tr;
    private Rigidbody rb;

    public float moveSpeed = 0.6f;
    public float turnSpeed = 20;

    bool isForward;
    float currenttTime;
    float creatTime = 1.0f;

    public GameObject[] PointWall;

    public override void Initialize()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

    }

    //학습이(Episode) 시작될 때 마다, 호출되는 함수
    public override void OnEpisodeBegin()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        tr.localPosition = new Vector3(-7, 0.5f, 75);
        tr.localRotation = Quaternion.Euler(new Vector3(0, -90, 0));

        PointWall[0].SetActive(true);


        for (int i = 1; i < PointWall.Length; i++)
        {
            PointWall[i].SetActive(false);
        }


    }

    //추변 환경을 관측하고, 관측 데이터를 브레인에게 전송하는 함수
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddOneHotObservation((isForward ? 0 : 1), 2);




    }

    //브레인으로부터 명령을 전달 받을때마다, 호출
    public override void OnActionReceived(ActionBuffers actions)
    {
        /*
         * 연속 (Continuese) -1.0f ~ 0.0f ~ +1.0f
         * 이산(Discrete     -1. 0, +1
         */

        var action = actions.DiscreteActions;

//        Debug.Log($"[0]={action[0]}, [1]={action[1]}");

        Vector3 dir = Vector3.zero;
        Vector3 rot = Vector3.zero;

        //Branch 0 => action[0]
        switch (action[0])
        {
            case 1: dir = tr.forward;   isForward = true; break;
            case 2: dir = -tr.forward;  isForward = false; break;
        }

        //Branch 1 => action[1]
        switch (action[1])
        {
            case 1: rot = tr.up; break;
            case 2: rot = -tr.up; break;
        }

        tr.Rotate(rot, Time.fixedDeltaTime * turnSpeed);
        rb.AddForce(dir * moveSpeed, ForceMode.VelocityChange);

        //움직임 유도를 위한 마이너스 패널티
        AddReward(-1.0f / (float)MaxStep); // -1/5000 = -0.005f

        //if(dir == tr.forward)
        //{
        //    AddReward(+0.005f);
        //}


    }

    //개발자(사용자)가 직접 명령을 내릴 때, 호출하는 메소드(주로 테스트용도 또는 모방학습에 사용)
    public override void Heuristic(in ActionBuffers actionsOut)
    {

        var actions = actionsOut.DiscreteActions; //Discreate(0, 1, 2, 3, ...)
        actions.Clear();

        //전진/후진 - Branch 0 = (0: 정지, 1:전진, 2: 후진) size = 3
        if (Input.GetKey(KeyCode.W))
        {
            actions[0] = 1; //전진
        }

        if (Input.GetKey(KeyCode.S))
        {
            actions[0] = 2; //후진
        }

        //좌우 회전 - Branch 1 = (0: 무회전, 1: 왼쪽회전, 2: 오른쪽회전) size = 3
        if (Input.GetKey(KeyCode.A))
        {
            actions[1] = 1; //왼쪽으로 회전
        }
        if (Input.GetKey(KeyCode.D))
        {
            actions[1] = 2; //오른쪽으로 회전
        }





    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("WALL"))
        {
            AddReward(-0.5f);
            //EndEpisode();
        }

        if (collision.collider.CompareTag("BAD_ITEM"))
        {
            AddReward(-1.0f);
            EndEpisode();
        }

        if (collision.collider.CompareTag("DEADZONE"))
        {
            AddReward(-0.5f);
        }

        if (collision.collider.CompareTag("PEOPLE"))
        {
            AddReward(-1.0f);
            EndEpisode();
        }



    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("WALL"))
        {
            currenttTime += Time.deltaTime;

            if (currenttTime >= creatTime)
            {
                AddReward(-1.0f);
                EndEpisode();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isForward == true)
        {
            if (other.gameObject.CompareTag("GOOD_ITEM"))
            {
                //print(+1);
                AddReward(+0.8f);
            }
        }

        if (isForward == false)
        {
            if (other.gameObject.CompareTag("GOOD_ITEM"))
            {
                //print(-1);
                AddReward(-1.0f);
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
