using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MinimapDrawer : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile whiteSquare;

    //[ContextMenu("Paint")]

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Paint(int x, int y, Color color)
    {
        Vector3Int pos = new Vector3Int(x/4, y/4, 0);
        tilemap.SetTile(pos, whiteSquare);
        tilemap.SetTileFlags(pos, TileFlags.None);
        tilemap.SetColor(pos, color);
    }
}
