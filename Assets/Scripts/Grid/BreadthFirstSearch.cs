using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace N_Grid
{
    public enum NodeSprite
    {
        NONE,
        HORIZONTAL,
        VERTICAL,
        ARROWUP,
        ARROWDOWN,
        ARROWLEFT,
        ARROWRIGHT,
        CORNER1,
        CORNER2,
        CORNER3,
        CORNER4
    }
    public enum PathEnd
    {
        EDIFICE,
        UNIT,
        ENEMY,
        INTERACTABLE
    }
    public struct ReturnNode
    {
        public NodeSprite NodeSprite { get; private set; }
        public (int, int) Self { get; private set; }
        public (int, int) PrevNode { get; private set; }
        public (int, int) NextNode { get; private set; }

        public ReturnNode(bool firstNode, bool lastNode, (int, int) self, (int, int) prev, (int, int) next)
        {
            Self = self;
            PrevNode = prev;
            NextNode = next;

            bool horizontal = false;
            bool vertical = false;

            NodeSprite = NodeSprite.NONE;

            if (firstNode)
            {
                NodeSprite = NodeSprite.NONE;
            }
            else if (lastNode)
            {
                (int, int) result = (PrevNode.Item1 - Self.Item1, PrevNode.Item2 - Self.Item2);
                Debug.Log(result);

                if (result == (0, -1))
                {
                    //up
                    NodeSprite = NodeSprite.ARROWUP;
                }
                else if (result == (0, 1))
                {
                    //down
                    NodeSprite = NodeSprite.ARROWDOWN;
                }
                else if (result == (1, 0))
                {
                    //left
                    NodeSprite = NodeSprite.ARROWLEFT;
                }
                else if (result == (-1, 0))
                {
                    //right
                    NodeSprite = NodeSprite.ARROWRIGHT;
                }
            }
            else
            {
                if (PrevNode.Item1 == Self.Item1)
                {
                    //same x value
                    //NodeSprite = NodeSprite.VERTICAL;
                    vertical = true;

                }
                if (PrevNode.Item2 == Self.Item2)
                {
                    //same y value
                    //NodeSprite = NodeSprite.HORIZONTAL;
                    horizontal = true;
                }

                if (Self.Item1 == NextNode.Item1)
                {
                    //same x value
                    //NodeSprite = NodeSprite.VERTICAL;
                    vertical = true;

                }
                if (Self.Item2 == NextNode.Item2)
                {
                    //same y value
                    //NodeSprite = NodeSprite.HORIZONTAL;
                    horizontal = true;
                }

                if (vertical && !horizontal)
                    NodeSprite = NodeSprite.VERTICAL;
                else if (horizontal && !vertical)
                    NodeSprite = NodeSprite.HORIZONTAL;
                else if (vertical && horizontal)
                {
                    (int, int) result = (PrevNode.Item1 - NextNode.Item1, PrevNode.Item2 - NextNode.Item2);

                    if (result.Item1 < 0 && result.Item2 < 0)
                    {
                        if (PrevNode.Item1 == Self.Item1)
                        {
                            NodeSprite = NodeSprite.CORNER3;
                        }
                        else if(PrevNode.Item2 == Self.Item2)
                        {
                            NodeSprite = NodeSprite.CORNER1;
                        }
                    }
                    else if (result.Item1 > 0 && result.Item2 < 0)
                    {
                        if (PrevNode.Item1 == Self.Item1)
                        {
                            NodeSprite = NodeSprite.CORNER4;
                        }
                        else if (PrevNode.Item2 == Self.Item2)
                        {
                            NodeSprite = NodeSprite.CORNER2;
                        }
                    }
                    else if (result.Item1 > 0 && result.Item2 > 0)
                    {
                        if (PrevNode.Item1 == Self.Item1)
                        {
                            NodeSprite = NodeSprite.CORNER1;
                        }
                        else if (PrevNode.Item2 == Self.Item2)
                        {
                            NodeSprite = NodeSprite.CORNER3;
                        }
                    }
                    else if (result.Item1 < 0 && result.Item2 > 0)
                    {
                        if (PrevNode.Item1 == Self.Item1)
                        {
                            NodeSprite = NodeSprite.CORNER2;
                        }
                        else if (PrevNode.Item2 == Self.Item2)
                        {
                            NodeSprite = NodeSprite.CORNER4;
                        }
                    }
                }
            }

            //Debug.Log("Self= " + self.Item1 + ", " + self.Item2);
            //Debug.Log("Next= " + next.Item1 + ", " + next.Item2);
            //Debug.Log("Prev= " + prev.Item1 + ", " + prev.Item2);
            //Debug.Log("First = " + firstNode + "   Last = " + lastNode);
            //Debug.Log("Vertical = " + vertical + "   Horizontal = " + horizontal);

        }
    }
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
                    if (grid.Tiles[w.Item1, w.Item2].OccupationState == OccupationStatus.ENEMYOCCUPIED ||
                        grid.Tiles[w.Item1, w.Item2].OccupationState == OccupationStatus.EDIFICE ||
                        grid.Tiles[w.Item1, w.Item2].OccupationState == OccupationStatus.OUTOFBOUNDS)
                        continue;

                    //Debug.Log(w.Item1 + " " + w.Item2);

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

        public List<ReturnNode> PathTo((int, int) v)
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

            List<(int, int)> stackList = stack.ToList<(int, int)>();
            //ReturnNode[] rtnAry = new ReturnNode[stack.Count];  //change to list
            List<ReturnNode> rtnList = new List<ReturnNode>();

            if (stackList.Count != 0)
            {
                for (int i = 0; i < stackList.Count; i++)
                {
                    if (i == 0)
                    {
                        //rtnAry[i] = new ReturnNode(true, false, stackList[i], (0, 0), stackList.Last());
                        rtnList.Add(new ReturnNode(true, false, stackList[i], (0, 0), stackList.Last()));
                    }
                    else if (i == stackList.Count - 1)
                    {
                        //rtnAry[i] = new ReturnNode(false, true, stackList[i], stackList[i - 1], (0, 0));
                        rtnList.Add(new ReturnNode(false, true, stackList[i], stackList[i - 1], (0, 0)));
                    }
                    else
                    {
                        //rtnAry[i] = new ReturnNode(false, false, stackList[i], stackList[i - 1], stackList[i+1]);
                        rtnList.Add(new ReturnNode(false, false, stackList[i], stackList[i - 1], stackList[i + 1]));
                    }
                }
            }

            return rtnList;
        }
    }
}
