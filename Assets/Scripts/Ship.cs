using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Ship : MonoBehaviour
{
    [SerializeField] private AudioPlayer audPlayer;
    private bool isActive = false;
    private Vector2 hatchPos;
    public GameController GC;
    [SerializeField] private Vector2 rotationPoint;
    private Vector2 startPosition;
    private Orientation startOrientation;
    private string outline;
    private int length;
    public static int height = 10;
    public static int width = 10;
    private Vector2 mousePosition;
    [SerializeField] private float offsetH = 0f, offsetV = 0f;
    private enum ShipTipes {None, ship_1, ship_2, ship_3, ship_4};
    [SerializeField] private ShipTipes shipTipe = ShipTipes.None;
    private enum Orientation {Vertical, Horizontal}
    [SerializeField] private Orientation orientation = Orientation.Horizontal;

    void Awake()
    {
        //Debug.Log(x = (int)ShipTipes.ship_2);
        //Debug.Log(x = (int)shipTipe);
    }
    void Start()
    {
        length = (int)shipTipe;
        startPosition = transform.position;
        startOrientation = Orientation.Horizontal;
    }

    void Update()
    {

    }

    public int GetLenght()
    {
        return length;
    }

    public void ActivateShip()
    {
        isActive = true;
    }

    public void SetShipByRandom(float X, float Y, int orient)
    {

        if (orient == 1)
        {
            orientation = Orientation.Horizontal;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            orientation = Orientation.Vertical;
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
            
        //Debug.Log("X: " + X + " Y: " + Y + " orient: " + orientation);
        FillAround(X, Y);
        transform.position = new Vector2(X, Y);
        Centering();
        //OutputGrid();
    }

    private void OutputGrid()

        /////////
        //DEBUG//
        /////////
    {
        for (int i = width - 1; i >= 0; i--)
        {
            for (int j = 0; j < height; j++)
            {
                outline += GC.ReadGrid(i,j).ToString() + "      ";
            }
            Debug.Log(outline);
            outline = "";
        }
    }
      
    public bool OnValidPosition(float X, float Y, int orient)
    {
        if (orient == 1)
        {
            orientation = Orientation.Horizontal;
        }
        else if (orient == 0)
        {
            orientation = Orientation.Vertical;
        }

        int posX = Mathf.RoundToInt(X);
        int posY = Mathf.RoundToInt(Y);

        if (posX > width - 1 || posX < 0 || posY > height - 1 || posY < 0)
        {
            return false;
        }

        for (int i = 0; i < length; i++)
        {
            if (orientation == Orientation.Horizontal)
            {
                //Debug.Log("i: " + i + " posX: " + (posX + i) + " posY: " + posY);
                if (posX + i > width - 1)
                    return false;
                else if (GC.ReadGrid(posY, posX + i) != 0)
                    return false;
            }
            else
            {
                //Debug.Log("i: " + i + " posX: " + posX + " posY: " + (posY + i));
                if (posY + i > height - 1)
                    return false;
                else if (GC.ReadGrid(posY + i, posX) != 0)
                    return false;
            }
        }
        return true;
    }

    private void FillAround(float X, float Y)
    {
        int posX = Mathf.RoundToInt(X);
        int posY = Mathf.RoundToInt(Y);

        for (int i = -1; i <= length; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (orientation == Orientation.Horizontal)
                {
                    if (posX + i < width && posX + i >= 0 && posY + j < height && posY + j >= 0)
                    {

                        if (i >= 0 && i < length && j == 0)
                            GC.ChangeGrid(posY + j, posX + i, 5);
                        else
                            GC.AugmentGrig(posY + j, posX + i);

                        if (GC.ReadGrid(posY + j, posX + i) == 1)
                        {
                            GC.SpawnHatch(posX + i, posY + j, false);
                        }
                    }
                }
                else
                {
                    if (posX + j < width && posX + j >= 0 && posY + i < height && posY + i >= 0)
                    {


                        if (i >= 0 && i < length && j == 0)
                            GC.ChangeGrid(posY + i, posX + j, 5);
                        else
                            GC.AugmentGrig(posY + i, posX + j);

                        if (GC.ReadGrid(posY + i, posX + j) == 1)
                        {
                            GC.SpawnHatch(posX + j, posY + i, false);
                        }

                        //if (GC.ReadGrid(posY + i, posX + j) != 5)
                        //    GC.AugmentGrig(posY + i, posX + j);

                        //if (GC.ReadGrid(posY + i, posX + j) == 1)
                        //{
                        //    hatchPos = new Vector2(posX + j, posY + i);
                        //    GC.SpawnHatch(hatchPos);
                        //}
                    }
                }

            }
        }
    }

    private void ClearGrig(float X, float Y)
    {
        int posX = Mathf.RoundToInt(X);
        int posY = Mathf.RoundToInt(Y);

        for (int i = -1; i <= length; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (orientation == Orientation.Horizontal)
                {
                    if (posX + i < width && posX + i >= 0 && posY + j < height && posY + j >= 0)
                    {
                        if (GC.ReadGrid(posY + j, posX + i) != 5)
                        {
                            GC.ReduceGrid(posY + j, posX + i);
                            if (GC.ReadGrid(posY + j, posX + i) == 0)
                                GC.DestroyHatch(posX + i, posY + j);
                        }
                        else
                            GC.ChangeGrid(posY + j, posX + i, 0);
                    }

                }
                else
                {
                    if (posX + j < width && posX + j >= 0 && posY + i < height && posY + i >= 0)
                    {
                        if (GC.ReadGrid(posY + i, posX + j) != 5)
                        {
                            GC.ReduceGrid(posY + i, posX + j);
                            if (GC.ReadGrid(posY + i, posX + j) == 0)
                                GC.DestroyHatch(posX + j, posY + i);
                        }
                        else
                            GC.ChangeGrid(posY + i, posX + j, 0);
                    }
                }
            }
        }

    }

    private void Centering()
    {
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0);
    }

    private void OnMouseDown()
    {
        audPlayer.PlayDragDrop();
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //offsetX = transform.position.x - mousePosition.x;
        //offsetY = transform.position.y - mousePosition.y;
        if (isActive)
            ClearGrig(transform.position.x, transform.position.y);
        //Debug.Log("click");
    }

    private void Rotation()
    {
        if (orientation == Orientation.Horizontal)
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), Vector3.forward, 90);
            orientation = Orientation.Vertical;
            //Centering();
        }
        else
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), Vector3.forward, -90);
            orientation = Orientation.Horizontal;
            //Centering();
        }
    }

    private void OnMouseDrag()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (orientation == Orientation.Horizontal)
            transform.position = new Vector3(mousePosition.x + offsetH, mousePosition.y, -0.5f);
        else
            transform.position = new Vector3(mousePosition.x, mousePosition.y + offsetV, -0.5f);

        if (Input.GetMouseButtonDown(1))
            Rotation();
    }

    private void OnMouseUp()
    {
        audPlayer.PlayDragDrop();
        Centering();
        if (!OnValidPosition(transform.position.x, transform.position.y, 2))
        {
            if (orientation != startOrientation)
            {
                Rotation();
            }
            transform.position = startPosition;
            if (isActive)
            {
                //AddToGrid(transform.position.x, transform.position.y);
                FillAround(transform.position.x, transform.position.y);
            }
        }
        else
        {
            isActive = true;
            //AddToGrid(transform.position.x, transform.position.y);
            FillAround(transform.position.x, transform.position.y);
            startPosition = transform.position;
            startOrientation = orientation;
        }
    }
}

