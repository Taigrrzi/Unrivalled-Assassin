using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameController : MonoBehaviour
{
    public static gameController instance;
    public Transform patrolParent;
    public Transform guardParent;
    public List<GameObject> guards;
    public List<GameObject> victims;
    public List<Transform> patrolNodes;
    public Transform lastSeensParent;
    public List<AvoidObjects> objectsToAvoid;
    public Object guardPrefab;
    public Object victimPrefab;

    public float spawnDelay;
    public bool spawned;
    public float spawnTimer;

    public float initDelay;
    public int initialGuardAmount;
    public int initialVictimAmount;
    // Start is called before the first frame update

    private void Awake()
    {
        patrolNodes = new List<Transform>();
        for (int i = 0; i < patrolParent.transform.childCount; i++)
        {
            patrolNodes.Add(patrolParent.GetChild(i));
        }
    }
    void Start()
    {
        guards = new List<GameObject>();
        victims = new List<GameObject>();
        instance = this;
        lastSeensParent = new GameObject("LastSeenPositions").transform;
        guardParent = new GameObject("Guards").transform;
        objectsToAvoid = new List<AvoidObjects>();
        objectsToAvoid.Add(new AvoidObjects(playerController.instance.gameObject,8));
        spawnTimer = 0;
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnDelay && !spawned) {
            spawned = true;
            SpawnGuards();
            SpawnVictims();
        }
    }

    public Transform RandomPatrolNode() {
        int rand = Random.Range(0, patrolNodes.Count);
        return (patrolNodes[rand]);
    }

    public Transform NearestPatrolNode(Vector3 pos)
    {
        float minDist = float.PositiveInfinity;
        Transform minPNode = null;
        foreach (Transform pNode in patrolNodes)
        {
            float currentDist = Vector3.Distance(pos, pNode.transform.position);
            if (currentDist < minDist)
            {
                minPNode = pNode;
                minDist = currentDist;
            }
        }
        return minPNode;
    }

    public void SpawnGuards()
    {
        for (int i = 0; i < initialGuardAmount; i++)
        {
            int patrolIndex;
            do
            {
                patrolIndex = Random.Range(0,patrolParent.transform.childCount);
            } while (NotInRange(patrolParent.transform.GetChild(patrolIndex)));
            GameObject newGuard = (GameObject)Instantiate<Object>(guardPrefab,patrolParent.GetChild(patrolIndex).position,Quaternion.identity,guardParent);
            guards.Add(newGuard);
        }
    }

    public void SpawnVictims()
    {
        for (int i = 0; i < initialVictimAmount; i++)
        {
            int patrolIndex;
            do
            {
                patrolIndex = Random.Range(0, patrolParent.transform.childCount);
            } while (NotInRange(patrolParent.transform.GetChild(patrolIndex)));
            GameObject newVictim = (GameObject)Instantiate<Object>(victimPrefab, patrolParent.GetChild(patrolIndex).position, Quaternion.identity);
            victims.Add(newVictim);
        }
    }

    public bool NotInRange(Transform point) {
        bool ret = false;
        foreach (AvoidObjects item in objectsToAvoid)
        {
            if (Vector3.Distance(item.obj.transform.position,point.position) < item.distance) {
                ret = true;
            }
        }
        return ret;
    }//test

    public GameObject  NearestGuard(Vector3 pos) {
        float minDist = float.PositiveInfinity;
        GameObject minGuard=null;
        foreach (GameObject guard in guards)
        {
            float currentDist = Vector3.Distance(pos, guard.transform.position);
            if (currentDist<minDist)
            {
                minGuard = guard;
                minDist = currentDist;
            }
        }
        return minGuard;
    }

    public void RenamePatrolNodes() {
        for (int i = 0; i < patrolParent.transform.childCount; i++)
        {
            patrolParent.GetChild(i).name = "Patrol Node " + i;
        }
    }

    public struct AvoidObjects
    {
        public AvoidObjects(GameObject obj, float distance) {
            this.obj = obj;
            this.distance = distance;
        }
        public GameObject obj;
        public float distance;
    }
}
