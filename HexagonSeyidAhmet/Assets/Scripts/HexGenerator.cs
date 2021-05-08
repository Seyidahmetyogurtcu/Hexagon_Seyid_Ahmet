using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGenerator : MonoBehaviour
{
    GridBrushBase gridBrushBase;
    public GridLayout gridLayout;
    public GameObject brushTarget;
  //public BoundsInt position;
    Grid grid;
    public Vector3 vec3;
    readonly float iterasyon = 0.748f;

    void Start()
    {
        // gridBrushBase =  component<GridBrushBase>();
        // gridBrushBase.BoxFill(gridLayout, brushTarget, position);
        grid = GetComponent<Grid>();
        grid.GetLayoutCellCenter();
        for (float y = 0; y < 10; y++)
        {
            float variable = 0.5f;
            float t = y;
            for (float x = 0; x < 15 * iterasyon; x += iterasyon)
            {
               Instantiate(brushTarget, new Vector3(x, y, 0), Quaternion.identity);
                variable = (-1) * variable;
                y = y + variable;
            }
            y = t;

        }
    }
    void Update()
    {
        //grid.cellGap = new Vector3(3.5f,0,0);
      //  Debug.Log(grid.LocalToCell(vec3));

    }
}
