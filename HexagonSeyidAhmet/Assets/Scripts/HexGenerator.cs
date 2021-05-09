using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexGenerator : MonoBehaviour
{
    GridBrushBase gridBrushBase;
    public GridLayout gridLayout;
    public GameObject brushTarget;
    //public BoundsInt position;
    Grid grid;
    //public 1Vector3 vec3;
    readonly float iterasyon = 0.748f;
    public int gridSizeX = 8, gridSizeY = 9;


    public Tilemap hexagonTileMap;
    Vector3 inputPosition;
    Vector3 touchPositionWorld=Vector3.zero;
    Vector3Int tileMapCordinate=Vector3Int.zero;
    Vector3 touchedCellsCenterPositionWorld = Vector3.zero;
    Vector3 a = Vector3.zero;
    void Start()
    {
       
        #region not used
        //// gridBrushBase =  component<GridBrushBase>();
        //// gridBrushBase.BoxFill(gridLayout, brushTarget, position);
        ////grid = GetComponent<Grid>();
        ////grid.GetLayoutCellCenter();

        ////this generates hexagons 
        //for (float y = 0; y < 10; y++)
        //{
        //    float variable = 0.5f;
        //    float t = y;
        //    for (float x = 0; x < 15 * iterasyon; x += iterasyon)
        //    {
        //     //  Instantiate(brushTarget, new Vector3(x, y, 0), Quaternion.identity);
        //        variable = (-1) * variable;
        //        y = y + variable;
        //    }
        //    y = t;
        //}
        #endregion

        //this generates hexagons with tilemap
        for (int x = 0; x <= gridSizeX; x++)
        {
            for (int y = 0; y <= gridSizeY; y++)
            {
                Instantiate(brushTarget, hexagonTileMap.CellToWorld(new Vector3Int(x, y, 0)), Quaternion.identity);
            }
        }
    }
    void Update()
    {
#if UNITY_EDITOR_64                                                      //Get the screen position input,
        inputPosition = Input.mousePosition;
#endif
#if UNITY_ANDROID && !UNITY_EDITOR_64
          inputPosition = Input.mousePosition;
#endif
        touchPositionWorld = Camera.main.ScreenToWorldPoint(inputPosition);//then convert it to World position, 
        tileMapCordinate = hexagonTileMap.WorldToCell(touchPositionWorld);//then convert it to tile map cordinates,
        touchedCellsCenterPositionWorld = hexagonTileMap.CellToWorld(tileMapCordinate);                   //then convert it to World point of the cell's center

        /*Calculate thesellected point*/
        a = touchPositionWorld - touchedCellsCenterPositionWorld;
       float theta= (Mathf.Atan2(a.y,a.x))* Mathf.Rad2Deg;
        //
        for (int i = -3; i < 3; i++)
        {
            Debug.LogWarning(theta);
            if (theta >= 60*i && theta < 60*(i+1))
            {
                //(i+1).corner is sellected
                Debug.Log("theta:"+ theta+"\n"+(i+4)+". corner sellected");
            }
        }
       // Debug.Log(hexagonTileMap.CellToWorld(tileMapCordinate));
    }
}
