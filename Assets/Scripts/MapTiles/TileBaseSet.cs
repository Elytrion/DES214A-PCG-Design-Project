using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu(fileName = "New TileBaseSet", menuName = "TileBaseSet")]
public class TileBaseSet : ScriptableObject
{
    /*
   1 
 4 x 2
   3 
*/

    public enum TileDirection
    {
        UP = 0b1111,
        LEFT = 0b0010,
        RIGHT = 0b1000,
        CORNER_LEFT_UP = 0b1100,
        CORNER_LEFT_DOWN = 0b1001,
        CORNER_RIGHT_UP = 0b0110,
        CORNER_RIGHT_DOWN = 0b0011,
        DOWN = 0b0001,
        NONE = 0b0000
    }

    [System.Serializable]
    public struct LabelledTiles
    {
        public TileDirection TileDirection;
        public TileBase TileToUse;
    }

    public List<LabelledTiles> TilesToUse;

    public TileBase GetTileWithLabel(int inLabel)
    {
        switch (inLabel)
        {
            case 0b0010:
                return GetTileBaseWithLabel(TileDirection.LEFT);
            case 0b1000:
                return GetTileBaseWithLabel(TileDirection.RIGHT);
            case 0b0001:
                return GetTileBaseWithLabel(TileDirection.DOWN);
            case 0b1100:
                return GetTileBaseWithLabel(TileDirection.CORNER_LEFT_UP);
            case 0b1001:
                return GetTileBaseWithLabel(TileDirection.CORNER_LEFT_DOWN);
            case 0b0110:
                return GetTileBaseWithLabel(TileDirection.CORNER_RIGHT_UP);
            case 0b0011:
                return GetTileBaseWithLabel(TileDirection.CORNER_RIGHT_DOWN);
            default:
                return GetTileBaseWithLabel(TileDirection.UP);
        }
    }

    public TileBase GetTileBaseWithLabel(TileDirection inLabel)
    {
        return TilesToUse.Find(x => x.TileDirection == inLabel).TileToUse;
    }
}
