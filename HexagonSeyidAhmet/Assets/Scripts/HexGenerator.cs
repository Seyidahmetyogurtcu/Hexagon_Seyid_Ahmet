using System;
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
    public float xx, yy;

    public Tilemap hexagonTileMap;
    Vector3 inputPosition;
    Vector3 touchPositionWorld = Vector3.zero;
    Vector3Int tileMapCordinate = Vector3Int.zero;
    Vector3 touchedCellsCenterPositionWorld = Vector3.zero;
    Vector3 a = Vector3.zero;
    GameObject[,] hexagons; //stores the data for all hex game objects 
    float clicktime = 0;
    float clickDelayTime = 0.1f;
    List<Collider2D> sellectedHexColliders;
    ContactFilter2D aaa;
    private Dictionary<int, Vector2> aliveli; //!daha sona için!
    Vector2 sellectedCorner1 = new Vector3(-0.5f, -0.25f); // hex 1.corner position from hex center
    Vector2 sellectedCorner2 = new Vector3(0, -0.5f); // hex 2.corner position from hex center
    Vector2 sellectedCorner3 = new Vector3(0.5f, -0.25f); // hex 3.corner position from hex center
    Vector2 sellectedCorner4 = new Vector3(0.5f, 0.25f); // hex 4.corner position from hex center
    Vector2 sellectedCorner5 = new Vector3(0, 0.5f); // hex 5.corner position from hex center
    Vector2 sellectedCorner6 = new Vector3(-0.5f, 0.25f); // hex 6.corner position from hex center
    public Transform hexagonLightBorder;
    Collider2D[] bbbOld;
    Collider2D[] bbbNew =null;



    void Start()
    {
        hexagons = new GameObject[gridSizeX, gridSizeY];

        #region not used

        //aliveli = new Dictionary<int, Vector2>();
        //aliveli.Add(1, sellectedCorner1);
        //aliveli.Add(2, sellectedCorner2);
        //aliveli.Add(3, sellectedCorner3);
        //aliveli.Add(4, sellectedCorner4);
        //aliveli.Add(5, sellectedCorner5);
        //aliveli.Add(6, sellectedCorner6);


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
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                //hexagons[x][y] = brushTarget;
                hexagons[x, y] = Instantiate(brushTarget, hexagonTileMap.CellToWorld(new Vector3Int(x, y, 0)), Quaternion.identity, this.transform);
                hexagons[x, y].name = "Hexagon " + "X_" + x + " Y_" + y;
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
        touchedCellsCenterPositionWorld = hexagonTileMap.CellToWorld(tileMapCordinate);  //then convert it to World point of the cell's center

        /*Calculate thesellected point*/
        a = touchPositionWorld - touchedCellsCenterPositionWorld;
        float theta = (Mathf.Atan2(a.y, a.x)) * Mathf.Rad2Deg;
        //
        for (int i = -3; i < 3; i++)
        {
            Debug.LogWarning(theta);
            if (theta >= 60 * i && theta < 60 * (i + 1))
            {
                //(i+4).corner is sellected
                switch (i + 4)
                {
                    case 1:
                        Selected(sellectedCorner1, 1);
                        break;
                    case 2:
                        Selected(sellectedCorner2, 2);
                        break;
                    case 3:
                        Selected(sellectedCorner3, 3);
                        break;
                    case 4:
                        Selected(sellectedCorner4, 4);
                        break;
                    case 5:
                        Selected(sellectedCorner5, 5);
                        break;
                    case 6:
                        Selected(sellectedCorner6, 6);
                        break;

                    default:
                        break;
                }
                Debug.Log("theta:" + theta + "\n" + (i + 4) + ". corner sellected");
            }

        }
        // Debug.Log(hexagonTileMap.CellToWorld(tileMapCordinate));
    }

    private void Selected(Vector2 corner, int cornerNum)
    {
        if (Input.GetMouseButton(0) && clicktime<Time.time)
        {
            clicktime = Time.time + clickDelayTime;
            /*racyast */
            bbbNew = Physics2D.OverlapCircleAll(corner + (Vector2)touchedCellsCenterPositionWorld, 0.2499f);
            if (bbbNew.Length >= 3)       //gives limit for selection (if it is not border)
            {

                if (bbbOld != null)
                {
                    foreach (var bbOld in bbbOld)
                    {
                        bbOld.transform.localScale = new Vector3(1, 1, 1);
                    }
                }
                bbbOld = bbbNew;
                foreach (Collider2D bbNew in bbbNew)
                {
                    bbNew.transform.localScale = new Vector3(1.2f, 1.2f, 1);
                }
                /*Light effcect */
                hexagonLightBorder.SetPositionAndRotation(corner + (Vector2)touchedCellsCenterPositionWorld, Quaternion.Euler(0, 0, 180 * cornerNum)); // //if corner number is odd ,then rotate
            }
        }
    }
}

