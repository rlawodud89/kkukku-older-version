using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{
    public class Node
    {
        public Vector2Int pos;
        public Node parent;
        public int g, h;
        public int f => g + h;
    }

    public static List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, System.Func<Vector2Int, bool> isBlocked)
    {
        var open = new List<Node>();
        var closed = new HashSet<Vector2Int>();

        open.Add(new Node { pos = start, g = 0, h = Heuristic(start, end) });

        while (open.Count > 0)
        {
            open.Sort((a, b) => a.f.CompareTo(b.f));
            var current = open[0];
            open.RemoveAt(0);

            if (current.pos == end)
                return Reconstruct(current);

            closed.Add(current.pos);

            foreach (var dir in Directions)
            {
                Vector2Int next = current.pos + dir;
                if (closed.Contains(next)) continue;
                if (isBlocked(next)) continue;

                var neighbor = new Node
                {
                    pos = next,
                    parent = current,
                    g = current.g + 1,
                    h = Heuristic(next, end)
                };

                var existing = open.Find(n => n.pos == next);
                if (existing == null)
                {
                    open.Add(neighbor);
                }
                else if (neighbor.g < existing.g)
                {
                    existing.parent = current;
                    existing.g = neighbor.g;
                }
            }
        }

        return null;
    }

    static int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // Manhattan distance
    }

    static List<Vector2Int> Reconstruct(Node endNode)
    {
        var path = new List<Vector2Int>();
        var current = endNode;
        while (current != null)
        {
            path.Insert(0, current.pos);
            current = current.parent;
        }
        return path;
    }

    static readonly Vector2Int[] Directions = {
        new Vector2Int(1, 0), new Vector2Int(-1, 0),
        new Vector2Int(0, 1), new Vector2Int(0, -1)
    };
}
