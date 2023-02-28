using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace N_Grid
{
    public class BreadthFirstSearch
    {
        bool[,] _marked;
        (int, int)[,] _edgeTo;
        (int, int) _source;

        public BreadthFirstSearch(G_Grid grid, (int, int) source)
        {
            _marked = new bool[grid.Width, grid.Height];
            _edgeTo = new (int, int)[grid.Width, grid.Height];
            _source = source;
            Bfs(grid, source);
        }

        private void Bfs(G_Grid grid, (int, int) source)
        {
            Queue<(int, int)> queue = new Queue<(int, int)>();
            _marked[source.Item1, source.Item2] = true;
            queue.Enqueue(source);
            while (queue.Count > 0)
            {
                (int, int) v = queue.Dequeue();
                foreach (var w in grid.GetAdjacency(v))
                {
                    Debug.Log(w.Item1 + " " + w.Item2);

                    if (!_marked[w.Item1, w.Item2])
                    {
                        _edgeTo[w.Item1, w.Item2] = v;
                        _marked[w.Item1, w.Item2] = true;
                        queue.Enqueue(w);
                    }
                }
            }
        }

        public bool HasPathTo((int, int) v)
        {
            return _marked[v.Item1, v.Item2];
        }

        public IEnumerable<(int, int)> PathTo((int, int) v)
        {
            Stack<(int, int)> stack = new Stack<(int, int)>();

            if (HasPathTo(v))
            {
                for ((int, int) i = v; i != _source; i = _edgeTo[i.Item1, i.Item2])
                {
                    stack.Push(i);
                }
                stack.Push(_source);
            }

            return stack;
        }
    }
}
