using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Map Graph", menuName = "MapGraph")]
public class MapGraph : ScriptableObject
{
    [System.Serializable]
    public class MapGraphNode
    {
        public string ID;
        public List<string> Neighbors;
    }

    public List<MapGraphNode> Nodes;

    public MapGraphNode GetRootNode()
    {
        NormalizeConnections();
        return Nodes[0];
    }

    public int GetTotalRoomCount()
    {
        return Nodes.Count;
    }

    public int GetNodeDistanceBetweenNodes(string inNodeId_a, string inNodeId_b)
    {
        MapGraphNode startingNode = Nodes.FirstOrDefault(n => n.ID == inNodeId_a);

        Queue<MapGraphNode> queue = new Queue<MapGraphNode>();
        HashSet<MapGraphNode> visited = new HashSet<MapGraphNode>();
        Dictionary<string, int> distanceFromSpawn = new Dictionary<string, int>();

        queue.Enqueue(startingNode);
        visited.Add(startingNode);
        distanceFromSpawn.Add(startingNode.ID, 0);
        while (queue.Count > 0)
        {
            MapGraphNode parent = queue.Dequeue();
            if (parent.ID == inNodeId_b)
            {
                return distanceFromSpawn[parent.ID];
            }

            foreach (string childID in parent.Neighbors)
            {
                MapGraphNode child = GetNode(childID);
                if (!visited.Contains(child) && !queue.Contains(child))
                {
                    queue.Enqueue(child);
                    visited.Add(child);
                    distanceFromSpawn.Add(child.ID, distanceFromSpawn[parent.ID] + 1);
                }
            }
        }

        return -1;
    }

    public void NormalizeConnections()
    {
        // ensure back connections exist
        foreach (MapGraphNode node in Nodes)
        {            
            foreach (string neighbor_ID in node.Neighbors)
            {
                MapGraphNode neighborNode = Nodes.Find(x => x.ID == neighbor_ID);
                if (!neighborNode.Neighbors.Contains(node.ID))
                {
                    neighborNode.Neighbors.Add(node.ID);
                }
            }
        }
    }

    public void AddNode(string ID, List<string> neighbors)
    {
        MapGraphNode node = new MapGraphNode();
        node.ID = ID;
        node.Neighbors = neighbors;
        Nodes.Add(node);
    }

    public void RemoveNode(string ID)
    {
        MapGraphNode node = Nodes.Find(x => x.ID == ID);

        // remove all references to this node
        foreach (MapGraphNode otherNode in Nodes)
        {
            if (otherNode.Neighbors.Contains(ID))
                otherNode.Neighbors.Remove(ID);
        }
        
        Nodes.Remove(node);
    }

    public MapGraphNode GetNode(string ID)
    {
        return Nodes.Find(x => x.ID == ID);
    }
}
