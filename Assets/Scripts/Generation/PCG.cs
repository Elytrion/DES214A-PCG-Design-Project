using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class PCG
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPos, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPos);
        Vector2Int prevPos = startPos;

        for (int i = 0; i < walkLength; i++)
        {
            Vector2Int newPos = prevPos + Direction2D.GetRandomCardinalDirection();
            path.Add(newPos);
            prevPos = newPos;
        }      
        return path;
    }

    public static T WeightedRandom<T>(Dictionary<T, float> weightedItems)
    {
        // Create a list to hold cumulative weights
        List<float> cumulativeWeights = new List<float>();
        float cumulativeWeight = 0.0f;

        // For each item, add the weight to cumulativeWeight and add it to the list
        foreach (float weight in weightedItems.Values)
        {
            cumulativeWeight += weight;
            cumulativeWeights.Add(cumulativeWeight);
        }

        // Choose a random number between 0 and cumulativeWeight
        float randomWeight = UnityEngine.Random.Range(0, cumulativeWeight);

        // Find the first item whose cumulative weight is greater than or equal to randomWeight
        int index = 0;
        while (randomWeight >= cumulativeWeights[index])
        {
            index++;
        }

        // Return the corresponding item
        return weightedItems.Keys.ElementAt(index);
    }

    public static T CoinFlip<T>(T first, T second)
    {
        System.Random rnd = new System.Random();
        return rnd.Next(2) == 0 ? first : second;
    }
}

public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>()
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }

    public static Vector2Int GetRandomCardinalDirection(Vector2Int inBannedDirection)
    {
        var culledDirectionList = cardinalDirectionsList.Where(d => d != inBannedDirection).ToList();
        return culledDirectionList[Random.Range(0, culledDirectionList.Count)];
    }

    public static Vector2Int GetRandomCardinalDirection(List<Vector2Int> inBannedDirections)
    {
        var culledDirectionList = cardinalDirectionsList.Where(d => !inBannedDirections.Contains(d)).ToList();
        return culledDirectionList[Random.Range(0, culledDirectionList.Count)];
    }
    
    public static DIRECTION DetermineDirection(Vector2Int inDirectionVec)
    {
        if (inDirectionVec == Vector2Int.up)
        {
            return DIRECTION.NORTH;
        }
        else if (inDirectionVec == Vector2Int.right)
        {
            return DIRECTION.EAST;
        }
        else if (inDirectionVec == Vector2Int.down)
        {
            return DIRECTION.SOUTH;
        }
        else if (inDirectionVec == Vector2Int.left)
        {
            return DIRECTION.WEST;
        }
        else
        {
            return DIRECTION.NONE;
        }
    }

    public static int DetermineDirectionLabel(Vector2Int inDirectionVec)
    {
        /*
           1 
         4 x 2
           3 
        */
        if (inDirectionVec == Vector2Int.up)
        {
            return 0b0001;
        }
        else if (inDirectionVec == Vector2Int.right)
        {
            return 0b0010;
        }
        else if (inDirectionVec == Vector2Int.down)
        {
            return 0b0100;
        }
        else if (inDirectionVec == Vector2Int.left)
        {
            return 0b1000;
        }
        else
        {
            return 0b0000;
        }
    }

    public enum DIRECTION
    {
        NORTH = 0,
        EAST = 1,
        SOUTH = 2,
        WEST = 3,
        NONE = 4
    };
}
