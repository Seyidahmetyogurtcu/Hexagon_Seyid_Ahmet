using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class HexGenerator : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GridLayout gridLayout;
    public GameObject brushTarget;
    public int gridSizeX = 8, gridSizeY = 9;
    public Tilemap hexagonTileMap;
    Vector3 inputPosition;
    Vector3 touchPositionWorld = Vector3.zero;
    Vector3Int tileMapCordinate = Vector3Int.zero;
    Vector3 touchedCellsCenterPositionWorld = Vector3.zero;
    Vector3 touchPositionVector = Vector3.zero;
    GameObject[,] hexagons; //stores the data for all hex game objects 
    float clicktime = 0;
    float clickDelayTime = 0.1f;
    List<Collider2D> sellectedHexColliders;
    //private Dictionary<int, Vector2> aliveli; //!daha sona için!
    Vector2[] sellectedCorners = new[] { new Vector2(-0.5f, -0.25f), new Vector2(0, -0.5f), new Vector2(0.5f, -0.25f), new Vector2(0.5f, 0.25f), new Vector2(0, 0.5f), new Vector2(-0.5f, 0.25f) };//hex 1-6.corner position from hex center
    public Transform hexagonLightBorder;
    Collider2D[] sellcetedOld;
    Collider2D[] sellectedNew = null;
    bool isDraging = false;
    private Vector2 hexTouchEnd;
    private Vector2 hexTouchStart;
    private float hexRotationAngle;
    public Collider2D[] sellectedHexagons = new Collider2D[3];
    private Color[] colors = new Color[5] { Color.cyan, Color.green, Color.red, Color.blue, Color.yellow };
    private Vector2 hexRotationVectorStart;
    private float hexRotationStartAngle;
    private Vector2 hexRotationVectorEnd;
    private float hexRotationEndAngle;
    private bool isRotate = false;
    private Vector3Int[] sellectedCellPositionsNew = new Vector3Int[3];
    private Vector3Int[] sellectedCellPositionsOld = new Vector3Int[3];
    private Vector3Int[] sellectedCellPositions = new Vector3Int[3];
    private Vector3Int[] tempSameColorCellPositions = new Vector3Int[3];
    private List<Vector3Int> sameColorCellPositions = new List<Vector3Int>();
    void Start()
    {
        #region dictionary not used yet
        //aliveli = new Dictionary<int, Vector2>();
        //aliveli.Add(1, sellectedCorner1);
        //aliveli.Add(2, sellectedCorner2);
        //aliveli.Add(3, sellectedCorner3);
        //aliveli.Add(4, sellectedCorner4);
        //aliveli.Add(5, sellectedCorner5);
        //aliveli.Add(6, sellectedCorner6);
        #endregion
        hexagons = new GameObject[gridSizeX, gridSizeY];
        GenerateMap(); //this generates hexagons with tilemap
    }

    void FixedUpdate()
    {
#if UNITY_EDITOR_64                                                      //Get the screen position input,
        inputPosition = Input.mousePosition;
#endif
#if UNITY_ANDROID && !UNITY_EDITOR_64
        Touch touch = Input.GetTouch(0);
        inputPosition = touch.position  /*touch input*/;  
#endif
        touchPositionWorld = Camera.main.ScreenToWorldPoint(inputPosition);//then convert it to World position, 
        tileMapCordinate = hexagonTileMap.WorldToCell(touchPositionWorld);//then convert it to tile map cordinates,
        touchedCellsCenterPositionWorld = hexagonTileMap.CellToWorld(tileMapCordinate);  //then convert it to World point of the cell's center
        touchPositionVector = touchPositionWorld - touchedCellsCenterPositionWorld;//Calculate the sellected point
        float theta = (Mathf.Atan2(touchPositionVector.y, touchPositionVector.x)) * Mathf.Rad2Deg;   // from -180 to 180

        for (int i = -3; i < 3; i++)
        {
            if (theta >= 60 * i && theta < 60 * (i + 1) && !isDraging && !isRotate)
            {
                sellectedCellPositions = Selected(sellectedCorners[i + 3], i + 4);//i.corner is sellected,1. is left-bottom corner
            }
        }
    }

    void Update()
    {
        Rotate();
    }


    void Rotate()
    {
        if (isDraging && !isRotate)
        {
            isRotate = true;                    //in here it will rotate
            hexRotationVectorStart = hexTouchStart - (Vector2)touchedCellsCenterPositionWorld;
            hexRotationStartAngle = Mathf.Atan2(hexRotationVectorStart.y, hexRotationVectorStart.x) * Mathf.Rad2Deg;  // from -180 to 180

            hexRotationVectorEnd = hexTouchEnd - (Vector2)touchedCellsCenterPositionWorld;
            hexRotationEndAngle = Mathf.Atan2(hexRotationVectorEnd.y, hexRotationVectorEnd.x) * Mathf.Rad2Deg;  // from -180 to 180

            hexRotationAngle = hexRotationEndAngle - hexRotationStartAngle;
            if (hexRotationAngle > 0 && hexRotationAngle <= 180)
            {
                for (int i = 0; i < 1; i++) //  do this 3 time
                {     //rotate one time
                    Debug.LogWarning("hexRotationAngle reverse : " + hexRotationAngle);
                    Vector3 tempRe0 = hexagons[sellectedCellPositions[0].x, sellectedCellPositions[0].y].transform.position;//this is the one of the three sellected hexes tilemap cell possition
                    Vector3 tempRe1 = hexagons[sellectedCellPositions[1].x, sellectedCellPositions[1].y].transform.position;//this is the one of the three sellected hexes tilemap cell possition
                    Vector3 tempRe2 = hexagons[sellectedCellPositions[2].x, sellectedCellPositions[2].y].transform.position;//this is the one of the three sellected hexes tilemap cell possition

                    GameObject tempReGo = hexagons[sellectedCellPositions[0].x, sellectedCellPositions[0].y].gameObject;
                    hexagons[sellectedCellPositions[0].x, sellectedCellPositions[0].y] = hexagons[sellectedCellPositions[1].x, sellectedCellPositions[1].y].gameObject;
                    hexagons[sellectedCellPositions[1].x, sellectedCellPositions[1].y] = hexagons[sellectedCellPositions[2].x, sellectedCellPositions[2].y].gameObject;
                    hexagons[sellectedCellPositions[2].x, sellectedCellPositions[2].y] = tempReGo;

                    hexagons[sellectedCellPositions[0].x, sellectedCellPositions[0].y].transform.position = tempRe0;
                    hexagons[sellectedCellPositions[1].x, sellectedCellPositions[1].y].transform.position = tempRe1;
                    hexagons[sellectedCellPositions[2].x, sellectedCellPositions[2].y].transform.position = tempRe2;

                    StartCoroutine(Wait());
                }
            }
            if (hexRotationAngle < 0 && hexRotationAngle > -180)
            {
                for (int i = 0; i < 1; i++) //  do this 3 time
                {
                    //rotate one time
                    Debug.LogWarning("hexRotationAngle ClockWise : " + hexRotationAngle);
                    //First I keep first hex array's positoin and gameobject in temp.
                    //Then I change position of hexagons and game object of hex array to turn hexagons.
                    Vector3 tempCw0 = hexagons[sellectedCellPositions[0].x, sellectedCellPositions[0].y].transform.position;
                    Vector3 tempCw1 = hexagons[sellectedCellPositions[1].x, sellectedCellPositions[1].y].transform.position;//this is the one of the three sellected hexes tilemap cell possition
                    Vector3 tempCw2 = hexagons[sellectedCellPositions[2].x, sellectedCellPositions[2].y].transform.position;//this is the one of the three sellected hexes tilemap cell possition

                    GameObject tempCwGo = hexagons[sellectedCellPositions[0].x, sellectedCellPositions[0].y].gameObject;
                    hexagons[sellectedCellPositions[0].x, sellectedCellPositions[0].y] = hexagons[sellectedCellPositions[2].x, sellectedCellPositions[2].y].gameObject;
                    hexagons[sellectedCellPositions[2].x, sellectedCellPositions[2].y] = hexagons[sellectedCellPositions[1].x, sellectedCellPositions[1].y].gameObject;
                    hexagons[sellectedCellPositions[1].x, sellectedCellPositions[1].y] = tempCwGo;

                    hexagons[sellectedCellPositions[0].x, sellectedCellPositions[0].y].transform.position = tempCw0;
                    hexagons[sellectedCellPositions[1].x, sellectedCellPositions[1].y].transform.position = tempCw1;
                    hexagons[sellectedCellPositions[2].x, sellectedCellPositions[2].y].transform.position = tempCw2;
                    StartCoroutine(Wait());
                }
            }
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.1f);
        LookIfThreeSameColor(sellectedCellPositions);//look if there are 3 or more same color
    }
    void GenerateMap()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                hexagons[x, y] = Instantiate(brushTarget, hexagonTileMap.CellToWorld(new Vector3Int(x, y, 0)), Quaternion.identity, this.transform);
                hexagons[x, y].name = "Hexagon " + "X_" + x + " Y_" + y;
                hexagons[x, y].GetComponent<SpriteRenderer>().color = colors[UnityEngine.Random.Range(0, 5)];
            }
        }
    }
    private Vector3Int[] Selected(Vector2 corner, int cornerNum)
    {
        if (Input.GetMouseButtonUp(0) && !isDraging)
        {

            /*racyast */
            sellectedNew = Physics2D.OverlapCircleAll(corner + (Vector2)touchedCellsCenterPositionWorld, 0.2499f);
            Debug.Log(sellectedNew.Length);
            if (sellectedNew.Length >= 3)       //gives limit for selection (if it is not border)
            {
                for (int i = 0; i < sellectedNew.Length; i++)
                {
                    sellectedCellPositionsNew[i] = hexagonTileMap.WorldToCell(sellectedNew[i].transform.position);
                }
                if (sellcetedOld != null)
                {
                    //   foreach (Collider2D bbOld in sellcetedOld)
                    //   {
                    //     bbOld.transform.localScale = new Vector3(1, 1, 1);
                    //   }
                }
                sellcetedOld = sellectedNew;
                sellectedCellPositionsOld = sellectedCellPositionsNew;
                // foreach (Collider2D bbNew in sellectedNew)
                //   {
                //      bbNew.transform.localScale = new Vector3(1.2f, 1.2f, 1);
                // }
                /*Light effcect */
                hexagonLightBorder.SetPositionAndRotation(corner + (Vector2)touchedCellsCenterPositionWorld, Quaternion.Euler(0, 0, 180 * cornerNum)); // //if corner number is odd ,then rotate
                return sellectedCellPositionsNew;
            }
            else
                return sellectedCellPositionsOld;
        }
        else
            return sellectedCellPositionsOld;
    }
    Collider2D[] sameColorHexagons = new Collider2D[12];
    List<Collider2D> sameColorHexagons2 = new List<Collider2D>();

    private void LookIfThreeSameColor(Vector3Int[] sellectedCellPositions)
    {
        for (int i = 0; i < 3; i++)   
        {
            for (int corner = 0; corner < 6; corner++) 
            {
                sellectedHexagons = Physics2D.OverlapCircleAll(sellectedCorners[corner] + (Vector2)hexagonTileMap.CellToWorld(sellectedCellPositions[i]), 0.2499f); //for all 3 sellected hex's edges
                if (sellectedHexagons.Length >= 3)  //in normal conditions
                {
                    if (sellectedHexagons[0].GetComponent<SpriteRenderer>().color == sellectedHexagons[1].GetComponent<SpriteRenderer>().color && sellectedHexagons[0].GetComponent<SpriteRenderer>().color == sellectedHexagons[2].GetComponent<SpriteRenderer>().color)//gives limit for selection(if it is not border)
                    {
                        #region comment this
                        for (int j = 0; j < sellectedHexagons.Length; j++) // sellectedHexagons.Length=3
                        {
                            tempSameColorCellPositions[j] = hexagonTileMap.WorldToCell(sellectedHexagons[j].transform.position); // tileMap Cell Positions of 3 same color Hexagons
                            if (!sameColorCellPositions.Contains(tempSameColorCellPositions[j]))
                            {
                                sameColorCellPositions.Add(tempSameColorCellPositions[j]);
                            }
                        }
                        #endregion
                        #region and use this instead,continue to find "index out of the array" error.
                        //sameColorHexagons = Physics2D.OverlapCircleAll(sellectedCorners[corner] + (Vector2)hexagonTileMap.CellToWorld(sellectedCellPositions[i]), 1.1f); //for all 9 sellected hex's edges
                        //for (int j = 0; j < 12; j++)  // sellectedHexagons.Length=9
                        //{
                        //    if (sameColorHexagons[j].GetComponent<SpriteRenderer>().color == sellectedHexagons[0].GetComponent<SpriteRenderer>().color)
                        //    {
                        //        sameColorHexagons2.Add(sameColorHexagons[j]);
                        //        sameColorHexagons2.ForEach(c => sameColorCellPositions.Add(hexagonTileMap.WorldToCell(c.transform.position))); //c is each of the collider inside of the list. 
                        //    }
                        //} // look if 2. circle has same color
                        #endregion 

                        /*destroy same colors*/
                        for (int t = 0; t < sameColorCellPositions.Count; t++)
                        {
                            Destroy(hexagons[sameColorCellPositions[t].x, sameColorCellPositions[t].y]);
                        }  // Destroy objects

                        /*----------------------------------------------------------------------*/
                        /*shift left objects to right*/
                        /*instantiate new hex from left*/
                        /*----------------------------------------------------------------------*/
                        sameColorCellPositions.Clear();
                        sameColorHexagons2.Clear();
                        sameColorHexagons[0] = null;
                        sameColorHexagons[1] = null;
                        sameColorHexagons[2] = null;
                        sameColorHexagons[3] = null;
                        sameColorHexagons[4] = null;
                        sameColorHexagons[5] = null;
                        sameColorHexagons[6] = null;
                        sameColorHexagons[7] = null;
                        sameColorHexagons[8] = null;

                        sellectedHexagons[0] = null;
                        sellectedHexagons[1] = null;
                        sellectedHexagons[2] = null;
                    }
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        isDraging = true;
        hexTouchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        isDraging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDraging = false;
        hexTouchEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isRotate = false;
    }
}

