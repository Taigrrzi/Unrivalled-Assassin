﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;

public class victimController : MonoBehaviour
{
    public float viewRadiusIdleMax;
    public float viewRadiusIdleBase;
    public float viewRadiusAlarmMax;
    public float viewRadiusAlarmBase;

    public float currentViewRadius;
    public float currentViewRadiusBase;
    public float currentViewRadiusMax;

    [Range(0, 360)]
    public float viewAngleIdleMax;
    [Range(0, 360)]
    public float viewAngleIdleBase;
    [Range(0, 360)]
    public float viewAngleAlarmMax;
    [Range(0, 360)]
    public float viewAngleAlarmBase;

    public float currentViewAngle;
    public float currentViewAngleBase;
    public float currentViewAngleMax;

    public string aiState;

    public float alarmGracePeriod;
    public float alarmGraceTimer;

    public IAstarAI agent;

    public float idleSpeed;
    public float alarmSpeed;

    public Material idleViewMaterial;
    public Material alarmViewMaterial;

    public Transform currentPatrolNode;

    public GameObject targetLastSeen;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();


    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDstThreshold;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    void Awake()
    {
        agent = GetComponent<IAstarAI>();
    }

    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine("FindTargetsWithDelay", .2f);
        alarmGraceTimer = 0;
        EnterState("idlePatrol");
        currentPatrolNode = gameController.instance.RandomPatrolNode();
        Camera.main.GetComponent<cameraFollow>().targets.Add(gameObject);
    }

    void Update()
    {
        AdjustViewForPlayerSpeed();
        switch (aiState)
        {
            case "seenTarget":
                if (visibleTargets.Count > 0)
                {
                    targetLastSeen.transform.position = visibleTargets[0].transform.position;
                }
                break;
            case "idlePatrol":

                if (agent.reachedEndOfPath && !agent.pathPending)
                {
                    EnterState("lookAround");
                }
                if (visibleTargets.Count > 0)
                {
                    alarmGraceTimer += Time.deltaTime;
                    if (alarmGraceTimer > alarmGracePeriod * (Vector3.Distance(transform.position, visibleTargets[0].transform.position) / currentViewRadius))
                    {
                        EnterState("seenTarget");
                    }
                }
                else
                {
                    alarmGraceTimer = 0;
                }
                break;
            default:
                break;
            case "reporting":
                agent.destination = gameController.instance.NearestGuard(transform.position).transform.position;
                agent.SearchPath();
                if ((agent.reachedEndOfPath && !agent.pathPending)||Vector3.Distance(transform.position,agent.destination)<1)
                {
                    GameObject relayedInfo = new GameObject("RelayedInfo");
                    relayedInfo.transform.parent = gameController.instance.lastSeensParent;
                    relayedInfo.transform.position = targetLastSeen.transform.position;
                    gameController.instance.NearestGuard(transform.position).GetComponent<guardController>().targetLastSeen = relayedInfo;
                    gameController.instance.NearestGuard(transform.position).GetComponent<guardController>().EnterState("seenTarget");
                    EnterState("idlePatrol");
                }
                break;
        }
    }

    void LateUpdate()
    {
        DrawFieldOfView();
    }

    public void EnterState(string newState)
    {
        if (aiState != newState)
        {
            ExitState(aiState);
            switch (newState)
            {
                case "lookAround":
                    aiState = "lookAround";
                    EnterState("idlePatrol");
                    break;
                case "seenTarget":
                    aiState = "seenTarget";
                    if (targetLastSeen == null)
                    {
                        targetLastSeen = new GameObject("LastSeen");
                        targetLastSeen.transform.parent = gameController.instance.lastSeensParent;
                    }
                    targetLastSeen.transform.position = visibleTargets[0].transform.position;
                    BecomeAlarmed();
                    EnterState("reporting");
                    break;
                case "idlePatrol":
                    aiState = "idlePatrol";
                    BecomeIdle();
                    currentPatrolNode = gameController.instance.RandomPatrolNode();
                    agent.destination = currentPatrolNode.position;
                    agent.SearchPath();

                    break;
                case "reporting":
                    aiState = "reporting";
                    BecomeAlarmed();
                    break;
                default:
                    break;
            }
        }
    }

    public void ExitState(string stateToExit)
    {
        switch (stateToExit)
        {
            case "reporting":
                Destroy(targetLastSeen);
                break;
            case "idlePatrol":
                break;
            default:
                break;
        }
    }


    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void AdjustViewForPlayerSpeed()
    {
        float speedRatio = playerController.instance.rbd.velocity.magnitude / playerController.instance.maxSpeed;
        currentViewAngle = currentViewAngleBase + ((currentViewAngleMax - currentViewAngleBase) * speedRatio);
        currentViewRadius = currentViewRadiusBase + ((currentViewRadiusMax - currentViewRadiusBase) * speedRatio);
    }

    public void BecomeAlarmed()
    {
        currentViewRadiusBase = viewRadiusAlarmBase;
        currentViewRadiusMax = viewRadiusAlarmMax;
        currentViewAngleBase = viewAngleAlarmBase;
        currentViewAngleMax = viewAngleAlarmMax;
        agent.maxSpeed = alarmSpeed;
        transform.GetChild(2).GetComponent<MeshRenderer>().material = alarmViewMaterial;
    }

    public void BecomeIdle()
    {
        currentViewRadiusBase = viewRadiusIdleBase;
        currentViewRadiusMax = viewRadiusIdleMax;
        currentViewAngleBase = viewAngleIdleBase;
        currentViewAngleMax = viewAngleIdleMax;
        agent.maxSpeed = idleSpeed;
        transform.GetChild(2).GetComponent<MeshRenderer>().material = idleViewMaterial;
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, currentViewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.up, dirToTarget) < currentViewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }


    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.z + 90;
        }
        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(currentViewAngle * meshResolution);
        float stepAngleSize = currentViewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = (transform.eulerAngles.z + 90) - currentViewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }

            }


            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }


    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }


    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, currentViewRadius, obstacleMask);

        if (hit)
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * currentViewRadius, currentViewRadius, globalAngle);
        }
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector2 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector2 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}