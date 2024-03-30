using CoreSLAM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LidarDrawer : MonoBehaviour
{

    public RawImage targetImage;
    private Texture2D texture;
    private int Size = 100;
    public Color color = Color.cyan;
    public LidarScan Lidar;
    private CoreSLAMProcessor SLAMProcessor;
    // Start is called before the first frame update
    void Start()
    {
        Size = Lidar.MapSize;
        texture = new Texture2D(Size, Size);
        targetImage.texture = texture;
    }

    // Update is called once per frame
    void Update()
    {
        ushort[] GrayValues = Lidar.MapData;

        for (int x = 0; x < texture.width; x++)
        {
            for(int y = 0; y < texture.height;y++)
            {

                ushort alpha = GrayValues[x + Size * y];
                byte alphaByte= (byte)(alpha>>8);
                
                texture.SetPixel(x, y,new Color32(255,255,255, alphaByte));
            }
        }
        texture.Apply();
    }   
}
