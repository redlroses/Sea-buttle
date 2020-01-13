using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private string outline;
    private static int height = 10;
    private static int width = 10;
    private static int[,] PlayerGrid = new int[width, height];
    private static int[,] AIgrid = new int[width, height];
    private static Transform[,] HatchesGrid = new Transform[width, height];
    private static Transform[,] MissGrid = new Transform[width, height];
    public GameObject[] EnemyIndicationShips;
    public GameObject[] PlayerIndicationShips;
    private bool kill;
    private int shipLenght;
    private int playerDisabledShip;
    private int AIDisabledShip;

    private Vector2 mousePosition;

    public UIController UI;
    public RandomArrangeShips RAS;
    private GameObject wreck;
    public GameObject wreck_prefab;
    private GameObject missIndication;
    public GameObject miss_prefab;
    public GameObject hatches;
    public GameObject hatch_prefab;
    private GameObject hatch;
    public GameObject enemyField;
    private GameObject explode;
    public GameObject explode_prefab;
    public AIEnemy AI;
    [SerializeField] private GameObject[] PlayerShips;
    [SerializeField] private GameObject[] EnemyShips;
    [SerializeField] private Sprite[] broken;
    private GameObject currentShip;
    [SerializeField] private ParticleSystem smoke_prefab;
    private ParticleSystem smoke;

    [SerializeField] private AudioPlayer audioPlayer;

    enum Turn { Player, AI };
    [SerializeField] private Turn turn = Turn.AI;

    void Start()
    {
        playerDisabledShip = 0;
        AIDisabledShip = 0;
    }

    void Update()
    {

    }

    public void PlayerTurn()
    {
        turn = Turn.Player;
    }

    public bool ReadyToPlay()
    {
        int inc = 0;
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                if (ReadGrid(i, j) == 5)
                {
                    inc++;
                }
            }
        if (inc < 20)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void ClearAllGrid()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                ChangeGrid(i, j, 0);
                if (ReadHatchesGrig(i, j))
                    DestroyHatch(i, j);
                AIChangeGrid(i, j, 0);
            }
    }

    private int LenghtFromTag(GameObject ship)
    {
        int lenght = 0;
        switch (ship.tag)
        {
            case "4":
                lenght = 4;
                break;
            case "3":
                lenght = 3;
                break;
            case "2":
                lenght = 2;
                break;
            case "1":
                lenght = 1;
                break;
            default:
                Debug.LogError("Incorrect Tag");
                break;
        }
        return lenght;
    }

    private void EndGame()
    {
        if (turn == Turn.Player)
        {
            //Debug.LogError("PLAYER WIN");
            UI.EndGame(false);
        }
        else
        {
            //Debug.LogError("AI WIN (loooseeer)");
            UI.EndGame(true);
        }
    }

    private void IsEnd()
    {
        if (turn == Turn.Player)
        {
            playerDisabledShip++;
            if (playerDisabledShip > 9)
            {
                EndGame();
            }
        }
        else
        {
            AIDisabledShip++;
            if (AIDisabledShip > 9)
            {
                EndGame();
            }
        }
    }

    private void DisableKilledShipAI(int lenght)
    {
        //Debug.Log("AI");
        foreach (GameObject ship in EnemyIndicationShips)
        {
            //Debug.Log(ship.GetComponent<SpriteRenderer>().color.g);
            if (ship.GetComponent<SpriteRenderer>().color == Color.green)
                if (LenghtFromTag(ship) == lenght + 1)
                {
                    ship.GetComponent<SpriteRenderer>().color = Color.red;
                    break;
                }
        }
    }

    private void DisableKilledShipPlayer(int lenght)
    {
        foreach (GameObject ship in PlayerIndicationShips)
        {
            if (ship.GetComponent<SpriteRenderer>().color == Color.green)
                if (LenghtFromTag(ship) == lenght + 1)
                {
                    ship.GetComponent<SpriteRenderer>().color = Color.red;
                    break;
                }
        }
    }

    private void SpriteChange(int X, int Y, bool Player0vsAI1)
    {
        if (Player0vsAI1)
        {
            foreach (GameObject ship in PlayerShips)
            {
                if (ship.transform.position.x == X && ship.transform.position.y == Y)
                {
                    currentShip = ship;
                }
            }
        }
        else
        {
            foreach (GameObject ship in EnemyShips)
            {
                Debug.Log("X: " + X + " Y: " + Y);
                if (ship.transform.localPosition.x == X && ship.transform.localPosition.y == Y)
                {
                    Debug.Log("true");
                    ship.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }
    }

    private void CheckKillShip(int zeroX, int zeroY, int prevX, int prevY, bool Player0VsAI1)
    {
        //Debug.Log("In");
        for (int i = -1; i < 2; i++)
            for (int j = -1; j < 2; j++)
            {
                if (zeroX + i >= 0 && zeroX + i < width && zeroY + j >= 0 && zeroY + j < height)
                {
                    if (Player0VsAI1)
                    {
                        if (ReadGrid(zeroX + i, zeroY + j) == 4 && !(i == 0 && j == 0) && !(zeroX + i == prevX && zeroY + j == prevY))
                        {
                            //Debug.Log("Kill Ship X: " + (zeroY + j) + " X: " + (zeroX + i));
                            CheckKillShip(zeroX + i, zeroY + j, zeroX, zeroY, Player0VsAI1);
                        }
                        //Debug.Log(" ALL -> X: " + (zeroY + j) + " Y: " + (zeroX + i));

                        if (ReadGrid(zeroX + i, zeroY + j) == 5)
                        {
                            //Debug.Log(" IF 5 -> X: " + (zeroY + j) + " Y: " + (zeroX + i));
                            kill = false;
                        }
                    }
                    else
                    {
                        if (AIReadGrid(zeroX + i, zeroY + j) == 4 && !(i == 0 && j == 0) && !(zeroX + i == prevX && zeroY + j == prevY))
                        {
                            //Debug.Log("Kill Ship X: " + (zeroY + j) + " X: " + (zeroX + i));
                            CheckKillShip(zeroX + i, zeroY + j, zeroX, zeroY, Player0VsAI1);
                        }
                        //Debug.Log(" ALL -> X: " + (zeroY + j) + " Y: " + (zeroX + i));

                        if (AIReadGrid(zeroX + i, zeroY + j) == 5)
                        {
                            //Debug.Log(" IF 5 -> X: " + (zeroY + j) + " Y: " + (zeroX + i));
                            kill = false;
                        }
                    }
                }
            }
        //Debug.Log("Out");
    }

    private void KillShip(int zeroX, int zeroY, int prevX, int prevY, bool Player0VsAI1)
    {
        //Debug.Log("In");
        for (int i = -1; i < 2; i++)
            for (int j = -1; j < 2; j++)
            {
                if (zeroX + i >= 0 && zeroX + i < width && zeroY + j >= 0 && zeroY + j < height)
                {
                    if (Player0VsAI1)
                    {
                        if (ReadGrid(zeroX + i, zeroY + j) == 4 && !(zeroX + i == prevX && zeroY + j == prevY))
                        {
                            if (!(i == 0 && j == 0))
                            {
                                KillShip(zeroX + i, zeroY + j, zeroX, zeroY, Player0VsAI1);
                                //Debug.Log("Kill Ship X: " + (zeroY + j) + " X: " + (zeroX + i));
                                shipLenght++;
                                DestroyHatch(zeroY + j, zeroX + i);
                                //Debug.Log("destroy explode");
                            }
                            SpawnSmoke(zeroY + j, zeroX + i, false);
                            SpriteChange(zeroY + j, zeroX + i, Player0VsAI1);
                        }

                        if (ReadGrid(zeroX + i, zeroY + j) == 0 && !(i == 0 && j == 0))
                        {
                            SpawnWreck(zeroY + j, zeroX + i, false);
                            ChangeGrid(zeroX + i, zeroY + j, 1);
                        }
                        else if (ReadGrid(zeroX + i, zeroY + j) == 1 && !(i == 0 && j == 0))
                        {
                            DestroyHatch(zeroY + j, zeroX + i);
                            SpawnWreck(zeroY + j, zeroX + i, false);
                        }
                    }
                    else
                    {

                        if (AIReadGrid(zeroX + i, zeroY + j) == 4 && !(zeroX + i == prevX && zeroY + j == prevY))
                        {
                            if (!(i == 0 && j == 0))
                            {
                                KillShip(zeroX + i, zeroY + j, zeroX, zeroY, Player0VsAI1);
                                //Debug.Log("Kill Ship X: " + (zeroY + j) + " X: " + (zeroX + i));
                                shipLenght++;
                                DestroyMiss(zeroY + j, zeroX + i);
                                //Debug.Log("destroy explode");
                            }
                            SpawnSmoke(zeroY + j, zeroX + i, true);
                            SpriteChange(zeroY + j, zeroX + i, Player0VsAI1);
                        }

                        if (AIReadGrid(zeroX + i, zeroY + j) == 0 && !(i == 0 && j == 0))
                        {
                            SpawnWreck(zeroY + j, zeroX + i, true);
                            AIChangeGrid(zeroX + i, zeroY + j, 1);
                        }
                        else if (AIReadGrid(zeroX + i, zeroY + j) == 1 && !(i == 0 && j == 0))
                        {
                            DestroyMiss(zeroY + j, zeroX + i);
                            SpawnWreck(zeroY + j, zeroX + i, true);
                        }
                    }

                }
            }
        //Debug.Log("Out");
        if (Player0VsAI1)
        {
            ChangeGrid(zeroX, zeroY, 3);
        }
        else
        {
            AIChangeGrid(zeroX, zeroY, 3);
        }
    }

    public int CheckHit(int posX, int posY, bool Player0VsAI1)
    {
        if (Player0VsAI1)
        {
            if (ReadGrid(posX, posY) == 0)
            {
                audioPlayer.PlayBlup();
                Debug.Log("<color=blue> AI turn ->  Miss X: " + posY + " Y: " + posX + "</color>");
                UI.AILogs(posY, posX, true);
                ChangeGrid(posX, posY, 1);
                SpawnMiss(posY, posX, false);
                turn = Turn.Player;
                return 0;
            }
            else if (ReadGrid(posX, posY) == 5)
            {
                //Hit
                audioPlayer.PlayBoom();
                Debug.Log("<color=blue> AI turn -> Hit X: " + posY + " Y: " + posX + "</color>");
                UI.AILogs(posY, posX, false);
                ChangeGrid(posX, posY, 4);
                kill = true;
                CheckKillShip(posX, posY, posX, posY, Player0VsAI1);
                //Debug.Log(kill);
                if (kill)
                {
                    audioPlayer.PlayBigBoom();
                    shipLenght = 0;
                    KillShip(posX, posY, -9, -9, Player0VsAI1);
                    currentShip.GetComponent<SpriteRenderer>().sprite = broken[shipLenght];
                    UI.KillShip(shipLenght + 1, true);
                    DisableKilledShipPlayer(shipLenght);
                    IsEnd();
                }
                else
                {
                    SpawnExplode(posY, posX, true);
                }
                return 5;
            }
        }
        else
        {
            if (AIReadGrid(posX, posY) == 0)
            {
                //Miss
                audioPlayer.PlayBlup();
                Debug.Log("<color=green> Player turn ->  Miss X: " + posY + " Y: " + posX + "</color>");
                UI.PlayerLogs(posY, posX, true);
                AIChangeGrid(posX, posY, 1);
                SpawnMiss(posY, posX, true);
                turn = Turn.AI;
                StartCoroutine(AI.LowAIAlgorithm());
                //AI.LowAIAlgorithm();
                return 0;
            }
            else if (AIReadGrid(posX, posY) == 5)
            {
                //Hit
                audioPlayer.PlayBoom();
                Debug.Log("<color=green> Player turn ->  Hit X: " + posY + " Y: " + posX + "</color>");
                UI.PlayerLogs(posY, posX, false);
                AIChangeGrid(posX, posY, 4);
                kill = true;
                CheckKillShip(posX, posY, posX, posY, Player0VsAI1);
                //Debug.Log(kill);
                if (kill)
                {
                    audioPlayer.PlayBigBoom();
                    shipLenght = 0;
                    KillShip(posX, posY, -9, -9, Player0VsAI1);
                    DisableKilledShipAI(shipLenght);
                    UI.KillShip(shipLenght + 1, false);
                    IsEnd();
                }
                else
                {
                    SpawnExplode(posY, posX, false);
                }
                return 5;
            }
        }
        return 1;
    }

    public void AIOutputGrid()
    {
        /////////
        //DEBUG//
        /////////

        for (int i = width - 1; i >= 0; i--)
        {
            for (int j = 0; j < height; j++)
            {
                outline += AIReadGrid(i, j).ToString() + "      ";
            }
            Debug.Log(outline);
            outline = "";
        }
    }

    private void PlayerOutputGrid()
    {
        /////////
        //DEBUG//
        /////////

        for (int i = width - 1; i >= 0; i--)
        {
            for (int j = 0; j < height; j++)
            {
                outline += ReadGrid(i, j).ToString() + "      ";
            }
            Debug.Log(outline);
            outline = "";
        }
    }

    public void GetReadyGrids()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                if (PlayerGrid[i, j] != 5)
                    PlayerGrid[i, j] = 0;

                if (AIgrid[i, j] != 5)
                    AIgrid[i, j] = 0;

                if (ReadHatchesGrig(i, j))
                    DestroyHatch(i, j);
            }
    }

    public bool ReadHatchesGrig(int i, int j)
    {
        return HatchesGrid[i, j] != null;
    }

    public int ReadGrid(int i, int j)
    {
        if (i < 0 || i > width - 1 || j < 0 || j > height - 1)
        {
            Debug.Log("Out Of Index " + i + " " + j);
            return -1;
        }
        else
            return PlayerGrid[i, j];
    }

    public void ChangeGrid(int i, int j, int changeTo)
    {
        //Debug.Log("i: " + i + " j: " + j);
        PlayerGrid[i, j] = changeTo;
    }

    public void AugmentGrig(int i, int j)
    {
        PlayerGrid[i, j]++;
    }

    public int AIReadGrid(int i, int j)
    {
        return AIgrid[i, j];
    }

    public void AIChangeGrid(int i, int j, int changeTo)
    {
        AIgrid[i, j] = changeTo;
    }

    public void ReduceGrid(int i, int j)
    {
        PlayerGrid[i, j]--;
    }

    public void SpawnMiss(int X, int Y, bool miss)
    {
        missIndication = Instantiate(miss_prefab);
        if (!miss)
        {
            missIndication.transform.SetParent(hatches.transform);
            HatchesGrid[X, Y] = missIndication.transform;
        }
        else
        {
            missIndication.transform.SetParent(enemyField.transform);
            MissGrid[X, Y] = missIndication.transform;
        }
        Vector2 pos = new Vector2(X, Y);
        missIndication.transform.localPosition = pos;
    }

    public void SpawnWreck(int X, int Y, bool miss)
    {
        wreck = Instantiate(wreck_prefab);
        if (!miss)
        {
            wreck.transform.SetParent(hatches.transform);
            HatchesGrid[X, Y] = wreck.transform;
        }
        else
        {
            wreck.transform.SetParent(enemyField.transform);
            MissGrid[X, Y] = wreck.transform;
        }
        Vector2 pos = new Vector2(X, Y);
        wreck.transform.localPosition = pos;
    }

    public void SpawnHatch(int X, int Y, bool miss)
    {
        hatch = Instantiate(hatch_prefab);
        if (!miss)
        {
            hatch.transform.SetParent(hatches.transform);
            HatchesGrid[X, Y] = hatch.transform;
        }
        else
        {
            hatch.transform.SetParent(enemyField.transform);
        }
        Vector2 pos = new Vector2(X, Y); 
        hatch.transform.localPosition = pos;
    }

    private void SpawnExplode(int X, int Y, bool AI)
    {
        Vector2 pos = new Vector2(X, Y);
        explode = Instantiate(explode_prefab);
        if (AI)
        {
            explode.transform.SetParent(hatches.transform);
            HatchesGrid[X, Y] = explode.transform;
        }
        else
        {
            explode.transform.SetParent(enemyField.transform);
            MissGrid[X, Y] = explode.transform;
        }
        explode.transform.localPosition = pos;
        Debug.Log("explode");
    }

    private void SpawnSmoke(int X, int Y, bool AI)
    {
        Vector2 pos = new Vector2(X, Y - 0.5f);
        smoke = Instantiate(smoke_prefab);
        if (!AI)
        {
            smoke.transform.SetParent(hatches.transform);
        }
        else
        {
            smoke.transform.SetParent(enemyField.transform);
        }
        smoke.transform.localPosition = pos;
    }

    public void DestroyHatch(int i, int j)
    {
        if (HatchesGrid[i, j] != null)
        {
            Destroy(HatchesGrid[i, j].gameObject);
            HatchesGrid[i, j] = null;
        }
    }

    public void DestroyMiss(int i, int j)
    {
        if (MissGrid[i, j] != null)
        {
            Destroy(MissGrid[i, j].gameObject);
            MissGrid[i, j] = null;
        }
    }

    void OnMouseDown()
    {
        if (turn == Turn.Player)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int posY = Mathf.RoundToInt(mousePosition.x - 13f);
            int posX = Mathf.RoundToInt(mousePosition.y);
            CheckHit(posX, posY, false);
        }
    }
}
