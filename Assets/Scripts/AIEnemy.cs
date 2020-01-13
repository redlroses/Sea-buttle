using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemy : MonoBehaviour

{
    public GameController GC;
    private enum Orientation { Vertical, Horizontal }
    private Orientation orientation = Orientation.Horizontal;
    public static int height = 10;
    public static int width = 10;
    private int[] length = new int[] { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
    private static int[,] shipCoord = new int[100,3];
    private string outline;
    private int startPosX;
    private int startPosY;
    private int startRandPosX;
    private int startRandPosY;
    public bool goBack = false;
    public bool isHit = false;
    private bool nextShip = false;
    private int startPlusX;
    private int startPlusY;
    private int last = 100;
    [SerializeField] private float timeBetweenAIturns;
    public GameObject[] Ships;
    [SerializeField] private Sprite[] broken;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddShipCoord()
    {
        int inc = 0;
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                 //Debug.Log("inc: " + inc);
                 shipCoord[inc, 0] = 1;
                 shipCoord[inc, 1] = i;
                 shipCoord[inc, 2] = j;
                 inc++;
            }
    }

    public void DeleteShipCoord()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                if (!(GC.ReadGrid(i, j) == 0 || GC.ReadGrid(i, j) == 5))
                {
                    for (int k = 0; k < 100; k++)
                    {
                        if (shipCoord[k, 1] == i && shipCoord[k, 2] == j)
                        {
                            shipCoord[k, 0] = 0;
                            //Debug.Log("Ship was detele -> " + shipCoord[k, 0] + " X: " + shipCoord[k, 2] + " Y: " + shipCoord[k, 1]);
                            Sort();
                        }
                    }
                }
            }
    }

    private void Sort()
    {
        for (int i = 0; i < last; i++)
        {
            if(shipCoord[i,0] == 0)
            {
                for (int j = i; j < last - 1; j++)
                {
                    shipCoord[j, 0] = shipCoord[j + 1, 0];
                    shipCoord[j, 1] = shipCoord[j + 1, 1];
                    shipCoord[j, 2] = shipCoord[j + 1, 2];
                }
                last--;
            }
        }
    }

    public IEnumerator LowAIAlgorithm()
    {
        int randX;
        int randY;

        int randNumber;

        if (isHit)
        {
            randX = startRandPosX;
            randY = startRandPosY;
            //Debug.Log("Start Coord -> X: " + startRandPosY + " Y: " + startRandPosX);
            StartCoroutine(HitAround(randX, randY));
        }
        else
        {
            DeleteShipCoord();
            Sort();
            //Debug.Log("leng: " + last);
            randNumber = Random.Range(0, last);
            randX = shipCoord[randNumber, 1];
            randY = shipCoord[randNumber, 2];
            //Debug.Log("randX: " + randY + " randY: " + randX);

            yield return new WaitForSecondsRealtime(timeBetweenAIturns);
            if (GC.CheckHit(randX, randY, true) == 5)
                {
                if (IsHitAround(randX, randY))
                {
                    startRandPosX = randX;
                    startRandPosY = randY;
                    //Debug.Log("Start Coord is equaling -> X: " + startRandPosY + " Y: " + startRandPosX);
                    isHit = true;
                    StartCoroutine(HitAround(randX, randY));
                }
                else
                {
                    isHit = false;
                    StartCoroutine(LowAIAlgorithm());
                }
            }
            //}
            //else
            //{
                //Debug.Log("!= 0 / 5");
                //LowAIAlgorithm();
            //}
        }
    }

    private IEnumerator HitAround(int posX, int posY)
    {
        //Debug.Log("GoBack In HitAround = " + goBack);
        if (goBack)
        {
            StartCoroutine(HitInLine(startRandPosX, startRandPosY, startPlusX, startPlusY));
        }
        else
        {
            if (IsHitAround(posX, posY))
            {
                 int randX = 0;
                 int randY = 0;

                 randX = Mathf.RoundToInt(Random.Range(-1.4f, 1.4f));
                 if (randX == 0)
                     randY = Mathf.RoundToInt(Random.Range(-1.4f, 1.4f));
                 else
                     randY = 0;
                 while ((randX == 0 && randY == 0) || !(GC.ReadGrid(posX + randX, posY + randY) == 0 || GC.ReadGrid(posX + randX, posY + randY) == 5))
                 {
                     randX = Mathf.RoundToInt(Random.Range(-1.4f, 1.4f));
                     if (randX == 0)
                         randY = Mathf.RoundToInt(Random.Range(-1.4f, 1.4f));
                     else
                         randY = 0;
                    //Debug.Log("While -> randX: " + randY + " randY: " + randX + " X: " + (posY + randY) + " Y: " + (posX + randX));
                 }

                yield return new WaitForSecondsRealtime(timeBetweenAIturns);
                if (GC.CheckHit(posX + randX, posY + randY, true) == 5)
                {
                    //Debug.Log("InHitAround Is 5 -> X: " + (posY + randY) + " Y: " + (posX + randX));
                    startPlusX = randX;
                    startPlusY = randY;
                    //Debug.Log("InHitAround start plus -> X: " + startPlusY + " Y: " + startPlusX);
                    StartCoroutine(HitInLine(posX + randX, posY + randY, randX, randY));
                }
                else
                {
                     //Debug.Log("InHitAround Is NOT 5 -> X: " + (posY + randY) + " Y: " + (posX + randX));
                }
            }
            else
            {
                isHit = false;
                goBack = false;
                StartCoroutine(LowAIAlgorithm());
            }
        }
    }

    private IEnumerator HitInLine(int posX, int posY, int plusX, int plusY)
    {
        //Debug.Log("GoBack in HitInLine = " + goBack);
        //Debug.Log("posX : " + (posY) + " posY: " + (posX));
        //Debug.Log("plusX: " + plusY + " plusY: " + plusX);
        if (goBack)
        {
            plusX = startPlusX;
            plusY = startPlusY;
            //Debug.Log("plusX After: " + plusY + " plusY After: " + plusX);
            if (GC.ReadGrid(posX - plusX, posY - plusY) == 1 || (posX - plusX) < 0 || (posX - plusX) > width - 1 || (posY - plusY) < 0 || (posY - plusY) > height - 1)
            {
                goBack = false;
                isHit = false;
                StartCoroutine(LowAIAlgorithm());
            }
            else
            {
                yield return new WaitForSecondsRealtime(timeBetweenAIturns);
                if (GC.CheckHit(posX - plusX, posY - plusY, true) == 5)
                {
                    StartCoroutine(HitInLine(posX - plusX, posY - plusY, plusX, plusY));
                }
                else
                {
                    isHit = false;
                    goBack = false;
                    StartCoroutine(LowAIAlgorithm());
                }
            }
        }
        else
        {
            if (GC.ReadGrid(posX + plusX, posY + plusY) == 1 || (posX - plusX) < 0 || (posX - plusX) > width - 1 || (posY - plusY) < 0 || (posY - plusY) > height - 1)
            {
                goBack = true;
                StartCoroutine(LowAIAlgorithm());
            }
            else
            {
                yield return new WaitForSecondsRealtime(timeBetweenAIturns);
                if (GC.CheckHit(posX + plusX, posY + plusY, true) == 5)
                {
                    //Debug.Log("If 5 -> posX : " + (posY + plusY) + " posY: " + (posX + plusX));
                    if (!IsHitAround(startRandPosX, startRandPosY))
                    {
                        isHit = false;
                        goBack = false;
                        StartCoroutine(LowAIAlgorithm());
                    }
                    else
                    {
                        StartCoroutine(HitInLine(posX + plusX, posY + plusY, plusX, plusY));
                    }
                }
                else
                {
                    //Debug.Log("If NOT 5 -> posX : " + (posY + plusY) + " posY: " + (posX + plusX));
                    if (!IsHitAround(startRandPosX, startRandPosY))
                    {
                        isHit = false;
                        goBack = false;
                        StartCoroutine(LowAIAlgorithm());
                    }
                    else
                    {
                        goBack = true;
                        if (GC.ReadGrid(posX + plusX, posY + plusY) == -1)
                            StartCoroutine(LowAIAlgorithm());
                    }
                }
            }
        }
    }

    public bool IsHitAround(int zeroX, int zeroY)
    {
        bool check = false;
        for (int i = -1; i < 2; i++)
            for (int j = -1; j < 2; j++)
            {
                if (zeroX + i >= 0 && zeroX + i < width && zeroY + j >= 0 && zeroY + j < height)
                {
                    if (!(i == 0 && j == 0))
                        if (GC.ReadGrid(zeroX + i, zeroY + j) == 5 || GC.ReadGrid(zeroX + i, zeroY + j) == 0)
                            check = true;
                }
            }
        //Debug.Log("X; " + zeroY + " Y: " + zeroX + " Check: " + check);
        return check;
    }

    public void GenerateDisposition()
    {
        int randX = Mathf.RoundToInt(Random.Range(0f, 9f));
        int randY = Mathf.RoundToInt(Random.Range(0f, 9f));

        int orient = Mathf.RoundToInt(randX * randY) % 2;
        OrientationChange(orient);

        int count = 0;

        while (count < length.Length)
        {
            while (!OnValidPosition(randX, randY, length[count]))
            {
                randX = Mathf.RoundToInt(Random.Range(0f, 9f));
                randY = Mathf.RoundToInt(Random.Range(0f, 9f));
                orient = Mathf.RoundToInt(randX * randY) % 2;
                OrientationChange(orient);
            }
            PlaceShip(randX, randY, length[count]);
            count++;
        }
    }

    private void AIOutputGrid()
    {

        /////////
        //DEBUG//
        /////////
        
        for (int i = width - 1; i >= 0; i--)
        {
            for (int j = 0; j < height; j++)
            {
                outline += GC.AIReadGrid(i, j).ToString() + "      ";
            }
            Debug.Log(outline);
            outline = "";
        }
    }

    private void OrientationChange(int orient)
    {
        if (orient == 1)
        {
            orientation = Orientation.Horizontal;
        }
        else
        {
            orientation = Orientation.Vertical;
        }
    }

    public bool OnValidPosition(int posX, int posY, int length)
    {
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
                else if (GC.AIReadGrid(posY, posX + i) != 0)
                    return false;
            }
            else
            {
                //Debug.Log("i: " + i + " posX: " + posX + " posY: " + (posY + i));
                if (posY + i > height - 1)
                    return false;
                else if (GC.AIReadGrid(posY + i, posX) != 0)
                    return false;
            }
        }
        return true;
    }

    private void PlaceShip(int posX, int posY, int length)
    {
        foreach (GameObject ship in Ships)
        {
            if(ship.tag == length.ToString())
            {
                if (orientation == Orientation.Vertical)
                {
                    ship.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                ship.GetComponent<SpriteRenderer>().enabled = false;
                ship.GetComponent<SpriteRenderer>().sprite = broken[length - 1];
                ship.transform.localPosition = new Vector2(posX, posY);
                ship.transform.localScale = new Vector3 (1,-1,1);
                ship.tag = "enemyShips";
                break;
            }
        }

        for (int i = -1; i <= length; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (orientation == Orientation.Horizontal)
                {
                    if (posX + i < width && posX + i >= 0 && posY + j < height && posY + j >= 0)
                    {
                        if (i >= 0 && i < length && j == 0)
                            GC.AIChangeGrid(posY + j, posX + i, 5);
                        else if (GC.AIReadGrid(posY + j, posX + i) != 1)
                            GC.AIChangeGrid(posY + j, posX + i, 1);
                    }
                }
                else
                {
                    if (posX + j < width && posX + j >= 0 && posY + i < height && posY + i >= 0)
                    {
                        if (i >= 0 && i < length && j == 0)
                            GC.AIChangeGrid(posY + i, posX + j, 5);
                        else if (GC.AIReadGrid(posY + i, posX + j) != 1)
                            GC.AIChangeGrid(posY + i, posX + j, 1);
                    }
                }

            }
        }
    }
}
