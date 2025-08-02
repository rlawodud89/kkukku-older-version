using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarMover : MonoBehaviour
{
    public float moveSpeed = 2f;
    public LayerMask obstacleLayer;

    public int roamMin = 1;
    public int roamMax = 5;

    private Animator anim;
    private Queue<Vector3> pathQueue = new Queue<Vector3>();

    private enum State { Entering, Roaming, Deciding, Paying, Exiting }
    private State state;

    private void Start()
    {
        anim = GetComponent<Animator>();
        state = State.Entering;

        // Step 1: 문 찾기
        Vector3 doorPos = FindClosestWithTag("Door");
        StartCoroutine(MoveTo(doorPos, () => StartRoaming()));
    }

    void StartRoaming()
    {
        state = State.Roaming;
        int roamCount = Random.Range(roamMin, roamMax + 1);
        StartCoroutine(RoamDisplays(roamCount));
    }

    IEnumerator RoamDisplays(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 display = FindClosestWithTag("Display", true);
            Vector3 near = GetOneStepBefore(display);

            yield return MoveTo(near);
        }

        state = State.Deciding;

        bool willBuy = Random.value < 0.8f;

        if (willBuy)
        {
            Vector3 cashier = FindClosestWithTag("Cashier");
            StartCoroutine(MoveTo(cashier, Exit));
        }
        else
        {
            Exit();
        }
    }

    void Exit()
    {
        state = State.Exiting;
        Vector3 door = FindClosestWithTag("Door");
        StartCoroutine(MoveTo(door, () => Destroy(gameObject)));
    }

    // ------------------ PATHFINDING ------------------

    IEnumerator MoveTo(Vector3 destination, System.Action onComplete = null)
    {
        Vector2Int start = WorldToGrid(transform.position);
        Vector2Int end = WorldToGrid(destination);

        List<Vector2Int> path = AStarPathfinder.FindPath(start, end, IsBlocked);

        if (path == null || path.Count == 0)
        {
            onComplete?.Invoke();
            yield break;
        }

        foreach (var point in path)
        {
            Vector3 world = GridToWorld(point);

            while (Vector2.Distance(transform.position, world) > 0.05f)
            {
                Vector2 dir = (world - transform.position).normalized;
                transform.position = Vector2.MoveTowards(transform.position, world, moveSpeed * Time.deltaTime);

                anim.SetFloat("MoveX", dir.x);
                anim.SetFloat("MoveY", dir.y);
                anim.SetFloat("Speed", 1f);

                yield return null;
            }

            yield return null;
        }

        // 도착 후: IDLE 대기
        anim.SetFloat("Speed", 0f);

        float waitTime = Random.Range(0f, 2f);
        yield return new WaitForSeconds(waitTime); //여기서 대기

        onComplete?.Invoke();
    }


    // ------------------ HELPERS ------------------

    Vector3 FindClosestWithTag(string tag, bool random = false)
    {
        var objs = GameObject.FindGameObjectsWithTag(tag);
        if (objs.Length == 0) return transform.position;

        if (random) return objs[Random.Range(0, objs.Length)].transform.position;

        float minDist = Mathf.Infinity;
        GameObject best = objs[0];
        foreach (var obj in objs)
        {
            float dist = Vector3.Distance(transform.position, obj.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                best = obj;
            }
        }
        return best.transform.position;
    }

    Vector3 GetOneStepBefore(Vector3 target)
    {
        Vector3 dir = (transform.position - target).normalized;
        Vector3 offset = dir * 1f;
        return target + offset;
    }

    Vector2Int WorldToGrid(Vector3 pos) => Vector2Int.RoundToInt(pos);
    Vector3 GridToWorld(Vector2Int grid)
    {
        return new Vector3(grid.x + 0.5f, grid.y + 0.5f, 0f);
    }

    bool IsBlocked(Vector2Int gridPos)
    {
        Vector3 world = GridToWorld(gridPos);
        return Physics2D.OverlapCircle(world, 0.3f, obstacleLayer);
    }
}

