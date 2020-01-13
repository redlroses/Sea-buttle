using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameController GC;
    public AIEnemy AI;
    public RandomArrangeShips RAS;
    public GameObject endGamePanel;
    public GameObject AIwin;
    public GameObject YOUwin;
    public GameObject notReady;
    public BoxCollider2D enemyColl;
    public Material blur;
    public GameObject shipIndication;
    public GameObject enemyShips;

    private bool ready;
    private bool end;
    private float prevTime;
    private bool isPlay;

    [SerializeField] private TextMeshProUGUI PlayerLogsText;
    [SerializeField] private TextMeshProUGUI AILogsText;

    [SerializeField] [Range(0f, 5f)] private float maxBlur = 0f;
    [SerializeField] [Range(0.1f, 3f)] private float speedBlur = 0f;
    [SerializeField] [Range(0.01f, 0.1f)] private float smoothBlur = 0f;

    private void Awake()
    {

    }

    private void Start()
    {
        blur.SetFloat("_Size", 0f);
        isPlay = false;
        ready = true;
        enemyColl.enabled = false;
    }

    private void Update()
    {
        if (!ready)
        {
            if (Time.time - prevTime > 2f)
            {
                notReady.SetActive(false);
            }
        }
    }

    public void Play()
    {
        if (!isPlay)
        {
            if (GC.ReadyToPlay())
            {
                isPlay = true;
                enemyColl.enabled = true;
                ready = true;
                notReady.SetActive(false);
                //enemyShips.SetActive(false);
                shipIndication.SetActive(true);
                GC.PlayerTurn();
                AI.GenerateDisposition();
                GC.GetReadyGrids();
                GC.AIOutputGrid();
                AI.AddShipCoord();
                RAS.DisableAllShips();
                ClearLogs();
            }
            else
            {
                prevTime = Time.time;
                ready = false;
                notReady.SetActive(true);
            }
        }
    }

    public void Reload()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        GC.ClearAllGrid();
    }

    public void AutoSet()
    {
        if (!isPlay)
        {
            GC.ClearAllGrid();
            RAS.RandomArrangeShip();
        }
    }

    public void EndGame(bool Player0VSAI1)
    {
        ClearLogs();
        StartCoroutine("Blur");
        endGamePanel.SetActive(true);
        if (Player0VSAI1)
            AIwin.SetActive(true);
        else
            YOUwin.SetActive(true);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    private IEnumerator Blur()
    {
        float value = 0;
        while(value < maxBlur)
        {
            value += smoothBlur;
            blur.SetFloat("_Size", value);
            yield return new WaitForSeconds(1 / 100 * speedBlur);
        }
        StopAllCoroutines();
    }

    public void PlayerLogs(int X, int Y, bool hit0OrMiss1)
    {
        if (!hit0OrMiss1)
        {
            PlayerLogsText.text += "<#FFBF02>Hit in coord -> X: " + X + " Y: " + Y + "</color>\n";
        }
        else
        {
            PlayerLogsText.text += "<#92FFA9>Miss in coord -> X: " + X + " Y: " + Y + "</color>\n";
        }
    }

    public void AILogs(int X, int Y, bool hit0OrMiss1)
    {
        if (!hit0OrMiss1)
        {
            AILogsText.text += "<#FFBF02>Hit in coord -> X: " + X + " Y: " + Y + "</color>\n";
        }
        else
        {
            AILogsText.text += "<#92FFA9>Miss in coord -> X: " + X + " Y: " + Y + "</color>\n";
        }
    }

    private void ClearLogs()
    {
        //PlayerLogsText.text += "\n";
        //PlayerLogsText.text += "\n";
        //PlayerLogsText.text += "\n";
        //AILogsText.text += "\n";
        //AILogsText.text += "\n";
        //AILogsText.text += "\n";

        PlayerLogsText.text = "";
        AILogsText.text = "";
    }

    public void KillShip(int lenght, bool Player0vsAI1)
    {
        if (Player0vsAI1)
        {
            AILogsText.text += "<#FF4040>Killed " + lenght + " deck ship</color>\n";
        }
        else
        {
            PlayerLogsText.text += "<#FF4040>Killed " + lenght + " deck ship</color>\n";
        }
    }
}
