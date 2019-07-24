using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameController : MonoBehaviour
{
    public static gameController instance;
    public List<Transform> patrolParents;
    public Transform guardParent;
    public Transform victimParent;
    public List<GameObject> guards;
    public List<GameObject> victims;
    public List<GameObject> players;
    public List<Transform> patrolNodes;
    public Transform AITargetsParent;
    public List<AvoidObjects> objectsToAvoid;
    public Object guardPrefab;
    public Object victimPrefab;
    public Object lastSeenPrefab;
    public Object relayedInfoPrefab;

    private int guardID;
    private int victimID;

    public List<GameObject> hearers;

    public float spawnDelay;
    public bool spawned;
    public float spawnTimer;

    public float viewChangeDistance;

    public float initDelay;
    public int initialGuardAmount;
    public int initialVictimAmount;
    // Start is called before the first frame update

    private void Awake()
    {
        foreach (Transform pParent in patrolParents)
        {
            pParent.GetComponent<patrolParent>().UpdateNodeList();
        }

        hearers = new List<GameObject>();
    }
    void Start()
    {
        guards = new List<GameObject>();
        victims = new List<GameObject>();
        guardID = 0;
        victimID = 0;
        instance = this;
        AITargetsParent = new GameObject("AI Targets").transform;
        guardParent = new GameObject("Guards").transform;
        victimParent = new GameObject("Victims").transform;
        objectsToAvoid = new List<AvoidObjects>();
        for (int i = 0; i < players.Count; i++)
        {
            objectsToAvoid.Add(new AvoidObjects(players[i], 8));
        }
        spawnTimer = 0;
    }

    public void MakeSound(Vector3 location, GameObject creator, string type, float sizeMod) {
        foreach (GameObject hearer in hearers)
        {
            if (Vector3.Distance(hearer.transform.position,location)<(hearer.GetComponent<canHearSound>().hearRange* sizeMod)) {
                hearer.GetComponent<canHearSound>().HearSound(location,creator,type,sizeMod);
            }
        }
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnDelay && !spawned) {
            spawned = true;
            foreach (Transform pParent in patrolParents)
            {
                SpawnGuardsOnPatrol(pParent);
                SpawnVictimsOnPatrol(pParent);
            }
        }
    }

    public int RandomPatrolNode(Transform pParent) {
        int rand = Random.Range(0, pParent.GetComponent<patrolParent>().nodes.Count);
        return rand;
    }

    public PNodeAndDirection NextPatrolNode(int currentNode,Transform pParent, bool forwards)
    {
        int boolToInt;
        if (forwards)
        {
            boolToInt = 1;
        }
        else
        {
            boolToInt = -1;
        }
        if (pParent.GetComponent<patrolParent>().loop)
        {

            return new PNodeAndDirection((int)Mathf.Repeat(currentNode + boolToInt,pParent.GetComponent<patrolParent>().nodes.Count), forwards);
        }
        else
        {
            if (currentNode == 0 || currentNode == pParent.GetComponent<patrolParent>().nodes.Count - 1)
            {
                return new PNodeAndDirection((int)Mathf.Repeat(currentNode - boolToInt, pParent.GetComponent<patrolParent>().nodes.Count), !forwards);
            }
            else
            {
                return new PNodeAndDirection(currentNode+boolToInt, forwards);
            }
        }
    }

    public struct PNodeAndDirection
    {
        public PNodeAndDirection(int node,bool forwards) {
            this.node = node;
            this.forwards = forwards;
        }
        public int node;
        public bool forwards;
    }

    public Transform NearestPlayer(Vector3 pos)
    {
        float minDist = float.PositiveInfinity;
        Transform minPlayer = null;
        foreach (GameObject player in players)
        {
            float currentDist = Vector3.Distance(pos, player.transform.position);
            if (currentDist < minDist)
            {
                minPlayer = player.transform;
                minDist = currentDist;
            }
        }
        return minPlayer;
    }

    public int NearestPatrolNode(Vector3 pos, Transform pParent)
    {
        float minDist = float.PositiveInfinity;
        int minPNode = -1;
        for (int i = 0; i < pParent.childCount; i++)
        {
            float currentDist = Vector3.Distance(pos, pParent.GetChild(i).position);
            if (currentDist < minDist)
            {
                minPNode = i;
                minDist = currentDist;
            }
        }
        return minPNode;
    }

    public GameObject NewAITarget(GameObject creator, string type)
    {
        return UpdateAITarget(null, creator,type);
    }

    public GameObject UpdateAITarget(GameObject newTarget, GameObject creator, string type) {
        if (newTarget) {
            Destroy(newTarget);
        }
        switch (type)
        {
            case "noticeableSound":
                newTarget = (GameObject)Instantiate<Object>(relayedInfoPrefab);
                newTarget.name = "InvestigatingSound: " + creator.name;
                newTarget.transform.SetParent(AITargetsParent);
                return newTarget;
            case "relayedInfo":
                newTarget = (GameObject)Instantiate<Object>(relayedInfoPrefab);
                newTarget.name = "RelayedInfo: " + creator.name;
                newTarget.transform.SetParent(AITargetsParent);
                return newTarget;
            case "seenTarget":
                newTarget = (GameObject)Instantiate<Object>(lastSeenPrefab);
                newTarget.name = "Last Seen: " + creator.name;
                newTarget.transform.SetParent(AITargetsParent);
                return newTarget;
            default:
                newTarget = (GameObject)Instantiate<Object>(lastSeenPrefab);
                newTarget.name = "UNKNOWNTARGETTYPE " + type + ": " + creator.name;
                return newTarget;
        }
    }

    public void SpawnGuardsOnPatrol(Transform pParent)
    {
        for (int i = 0; i < pParent.GetComponent<patrolParent>().randomGuardAmount; i++)
        {
            int patrolIndex;
            do
            {
                patrolIndex = Random.Range(0, pParent.transform.childCount);
            } while (NotInRange(pParent.transform.GetChild(patrolIndex)));
            GameObject newGuard = (GameObject)Instantiate<Object>(guardPrefab, pParent.GetChild(patrolIndex).position,Quaternion.identity,guardParent);
            newGuard.name = "Guard " + guardID++;
            newGuard.GetComponent<guardController>().patrolParent = pParent;
            guards.Add(newGuard);
        }
    }
    public void SpawnVictimsOnPatrol(Transform pParent)
    {
        for (int i = 0; i < pParent.GetComponent<patrolParent>().randomVictimAmount; i++)
        {
            int patrolIndex;
            do
            {
                patrolIndex = Random.Range(0, pParent.transform.childCount);
            } while (NotInRange(pParent.transform.GetChild(patrolIndex)));
            GameObject newVictim = (GameObject)Instantiate<Object>(victimPrefab, pParent.GetChild(patrolIndex).position, Quaternion.identity, victimParent);
            newVictim.name = "Target " + victimID++;
            victims.Add(newVictim);
            newVictim.GetComponent<victimController>().patrolParent = pParent;
        }
    }

    /*public void SpawnVictims()
    {
        for (int i = 0; i < initialVictimAmount; i++)
        {
            int patrolIndex;
            do
            {
                patrolIndex = Random.Range(0, patrolParent.transform.childCount);
            } while (NotInRange(patrolParent.transform.GetChild(patrolIndex)));
            GameObject newVictim = (GameObject)Instantiate<Object>(victimPrefab, patrolParent.GetChild(patrolIndex).position, Quaternion.identity,victimParent);
            newVictim.name = "Target " + victimID++;
            victims.Add(newVictim);
        }
    }*/

    public bool NotInRange(Transform point) {
        bool ret = false;
        foreach (AvoidObjects item in objectsToAvoid)
        {
            if (Vector3.Distance(item.obj.transform.position,point.position) < item.distance) {
                ret = true;
            }
        }
        return ret;
    }

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
        foreach (Transform pParent in patrolParents)
        {
            for (int i = 0; i < pParent.childCount; i++)
            {
                pParent.GetChild(i).name = "Patrol Node " + i;
            }
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
