using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{

    public List<Transform> patrolPoints = new List<Transform>();
    public List<Transform> randomPoints = new List<Transform>();

    public Graph<Transform> patrolGraph = new Graph<Transform>();

    private Transform currentTarget;

    public float speed;
    Rigidbody rigidbody;
    public Transform player;

    public float aggroDistance;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        int pointcount = patrolPoints.Count;
        pointcount = patrolPoints.Count;

        if (speed <= 0)
        {
            speed = 5.0f;

            //Debug.Log("Enemy Speed not set on " + name + " defaulting to " + speed);
        }

        if (aggroDistance <= 0)
        {
            aggroDistance = 10.0f;

            Debug.Log("AggroDistance not set on " + name + " defaulting to " + aggroDistance);
        }


        for (int i = 0; i < pointcount; i++)
        {
            //PickRandomNode();
            int randNode = UnityEngine.Random.Range(0, patrolPoints.Count);
            randomPoints.Add(patrolPoints[randNode]);
            patrolPoints.Remove(patrolPoints[randNode]);
            patrolGraph.AddNode(randomPoints[i]);
        }

        for (int i = 0; i < randomPoints.Count; i++)
        {
            if (i == randomPoints.Count - 1)
            {
                patrolGraph.AddEdge(randomPoints[i], randomPoints[0]);
            }
            else
            {
                patrolGraph.AddEdge(randomPoints[i], randomPoints[i + 1]);
            }
        }

        currentTarget = patrolGraph.FindNode(randomPoints[0]).GetData();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
        Vector3 playerPosition = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

        if (currentTarget && Vector3.Distance(transform.position, player.position) > aggroDistance)
            rigidbody.MovePosition(position);
        else if (currentTarget && Vector3.Distance(transform.position, player.position) <= aggroDistance)
            rigidbody.MovePosition(playerPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Patrol"))
        {
            currentTarget = patrolGraph.FindNode(other.transform).GetOutgoing()[0].GetData();
        }
    }
}