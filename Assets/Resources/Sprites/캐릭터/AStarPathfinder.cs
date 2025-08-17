using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{
    public class Node { public Vector3Int cell; public Node parent; public int g, h; public int f => g + h; }

    public static List<Vector3Int> FindPath(
        Vector3Int start, Vector3Int goal,
        System.Func<Vector3Int, bool> isBlocked,
        bool allowDiagonal = false,
        bool preventCornerCut = true)
    {
        var open = new List<Node>();
        var closed = new HashSet<Vector3Int>();
        open.Add(new Node { cell = start, g = 0, h = Heu(start, goal) });

        Vector3Int[] dirs4 = { new(1, 0, 0), new(-1, 0, 0), new(0, 1, 0), new(0, -1, 0) };
        Vector3Int[] dirs8 = {
            new( 1,0,0), new(-1,0,0), new(0, 1,0), new(0,-1,0),
            new( 1,1,0), new( 1,-1,0), new(-1, 1,0), new(-1,-1,0)
        };
        var dirs = allowDiagonal ? dirs8 : dirs4;

        while (open.Count > 0)
        {
            open.Sort((a, b) => a.f.CompareTo(b.f));
            var cur = open[0]; open.RemoveAt(0);
            if (cur.cell == goal) return Reconstruct(cur);
            closed.Add(cur.cell);

            foreach (var d in dirs)
            {
                var next = cur.cell + d;
                if (closed.Contains(next)) continue;
                if (isBlocked(next)) continue;

                // 대각선 코너 끼기 방지
                if (allowDiagonal && preventCornerCut && d.x != 0 && d.y != 0)
                {
                    if (isBlocked(cur.cell + new Vector3Int(d.x, 0, 0))) continue;
                    if (isBlocked(cur.cell + new Vector3Int(0, d.y, 0))) continue;
                }

                int step = (d.x != 0 && d.y != 0) ? 14 : 10; // 대각 14, 직선 10
                int ng = cur.g + step;

                var exist = open.Find(n => n.cell == next);
                if (exist == null)
                    open.Add(new Node { cell = next, parent = cur, g = ng, h = Heu(next, goal) });
                else if (ng < exist.g) { exist.g = ng; exist.parent = cur; }
            }
        }
        return null;
    }

    static int Heu(Vector3Int a, Vector3Int b)
        => (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y)) * 10;

    static List<Vector3Int> Reconstruct(Node n)
    {
        var path = new List<Vector3Int>();
        for (; n != null; n = n.parent) path.Insert(0, n.cell);
        return path;
    }
}
