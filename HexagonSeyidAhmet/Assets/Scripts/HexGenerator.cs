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
    float clickDelayTime = 0.3f;
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
                sellectedHexagons = Selected(sellectedCorners[i + 3], i + 4);//i.corner is sellected,1. is left-bottom corner
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
                for (int i = 0; i < 3; i++) //  do this 3 time
                {     //rotate one time
                    Debug.LogError("hexRotationAngle reverse : " + hexRotationAngle);

                    Vector3 temp = sellectedHexagons[0].transform.position;
                    Vector3.Slerp(sellectedHexagons[0].transform.position, sellectedHexagons[1].transform.position,   Time.deltaTime);
                    Vector3.Slerp(sellectedHexagons[1].transform.position, sellectedHexagons[2].transform.position,   Time.deltaTime);
                    Vector3.Slerp(sellectedHexagons[2].transform.position, temp, 5 * Time.deltaTime);
                    //sellectedHexagons[0].transform.position = sellectedHexagons[1].transform.position;
                   // sellectedHexagons[1].transform.position = sellectedHexagons[2].transform.position;
                   // sellectedHexagons[2].transform.position = temp;

                    LookIfThreeSameColor(sellectedHexagons); //look if there are 3 or more same color
                }
            }
            if (hexRotationAngle < 0 && hexRotationAngle > -180)
            {
                for (int i = 0; i < 3; i++) //  do this 3 time
                {
                    Debug.LogError("hexRotationAngle clockwise : " + hexRotationAngle);
                    //rotate one time
                    Vector3 temp = sellectedHexagons[0].transform.position;
                    sellectedHexagons[0].transform.position = sellectedHexagons[2].transform.position;
                    sellectedHexagons[2].transform.position = sellectedHexagons[1].transform.position;
                    sellectedHexagons[1].transform.position = temp;

                    LookIfThreeSameColor(sellectedHexagons);//look if there are 3 or more same color
                }
            }
        }
    }

    private Collider2D[] Selected(Vector2 corner, int cornerNum)
    {
        if (Input.GetMouseButtonUp(0) && !isDraging && clicktime < Time.fixedTime)
        {
            clicktime = Time.time + clickDelayTime;
            /*racyast */
            sellectedNew = Physics2D.OverlapCircleAll(corner + (Vector2)touchedCellsCenterPositionWorld, 0.2499f);

            if (sellectedNew.Length >= 3)       //gives limit for selection (if it is not border)
            {
                if (sellcetedOld != null)
                {
                    foreach (Collider2D bbOld in sellcetedOld)
                    {
                        bbOld.transform.localScale = new Vector3(1, 1, 1);
                    }
                }
                sellcetedOld = sellectedNew;
                foreach (Collider2D bbNew in sellectedNew)
                {
                    bbNew.transform.localScale = new Vector3(1.2f, 1.2f, 1);
                }
                /*Light effcect */
                hexagonLightBorder.SetPositionAndRotation(corner + (Vector2)touchedCellsCenterPositionWorld, Quaternion.Euler(0, 0, 180 * cornerNum)); // //if corner number is odd ,then rotate
                return sellectedNew;
            }
            else
                return sellcetedOld;
        }
        else
            return sellcetedOld;
    }

    private void LookIfThreeSameColor(Collider2D[] sellectedHexagons)
    {
        for (int x = 0; x < 3 + 1; x++)
        {
            for (int corner = 0; corner < 6; corner++)
            {
                sellectedHexagons = Physics2D.OverlapCircleAll(sellectedCorners[corner] + (Vector2)sellectedHexagons[x].transform.position, 0.2499f);
                if (sellectedHexagons.Length >= 3)  //in normal conditions
                {
                    if (sellectedHexagons[0].GetComponent<SpriteRenderer>().color == sellectedHexagons[1].GetComponent<SpriteRenderer>().color && sellectedHexagons[0].GetComponent<SpriteRenderer>().color == sellectedHexagons[2].GetComponent<SpriteRenderer>().color)//gives limit for selection(if it is not border)
                    {
                        sellectedHexagons[0].gameObject.SetActive(false);
                        sellectedHexagons[1].gameObject.SetActive(false);
                        sellectedHexagons[2].gameObject.SetActive(false);
                    }
                    else
                    {
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

