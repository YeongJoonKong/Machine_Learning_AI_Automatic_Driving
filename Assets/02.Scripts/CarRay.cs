using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CarRay : Agent
{
    /*
     * 1. �ֺ�ȯ���� ���� (Observations)
     * 2. ��å�� ���� �ൿ (Actions)
     * 3. �ൿ�� ���� ���� (Reward)
     */

    // ���� ����
    /*
     * - Rigidbody Velocity
     * - Ÿ�ٰ��� �Ÿ�
     * - �ڽ��� ��ġ
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

    //�н���(Episode) ���۵� �� ����, ȣ��Ǵ� �Լ�
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

    //�ߺ� ȯ���� �����ϰ�, ���� �����͸� �극�ο��� �����ϴ� �Լ�
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddOneHotObservation((isForward ? 0 : 1), 2);




    }

    //�극�����κ��� ����� ���� ����������, ȣ��
    public override void OnActionReceived(ActionBuffers actions)
    {
        /*
         * ���� (Continuese) -1.0f ~ 0.0f ~ +1.0f
         * �̻�(Discrete     -1. 0, +1
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

        //������ ������ ���� ���̳ʽ� �г�Ƽ
        AddReward(-1.0f / (float)MaxStep); // -1/5000 = -0.005f

        //if(dir == tr.forward)
        //{
        //    AddReward(+0.005f);
        //}


    }

    //������(�����)�� ���� ����� ���� ��, ȣ���ϴ� �޼ҵ�(�ַ� �׽�Ʈ�뵵 �Ǵ� ����н��� ���)
    public override void Heuristic(in ActionBuffers actionsOut)
    {

        var actions = actionsOut.DiscreteActions; //Discreate(0, 1, 2, 3, ...)
        actions.Clear();

        //����/���� - Branch 0 = (0: ����, 1:����, 2: ����) size = 3
        if (Input.GetKey(KeyCode.W))
        {
            actions[0] = 1; //����
        }

        if (Input.GetKey(KeyCode.S))
        {
            actions[0] = 2; //����
        }

        //�¿� ȸ�� - Branch 1 = (0: ��ȸ��, 1: ����ȸ��, 2: ������ȸ��) size = 3
        if (Input.GetKey(KeyCode.A))
        {
            actions[1] = 1; //�������� ȸ��
        }
        if (Input.GetKey(KeyCode.D))
        {
            actions[1] = 2; //���������� ȸ��
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
