using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ResolutionSettings : MonoBehaviour
{
    public TMPro.TMP_Dropdown dropdown;

    [SerializeField] private Toggle fScreen;

    private Resolution[] resolutions;
    private Resolution curRes;
    private bool curIsFullscreen;

    void Start()
    {
        resolutions = Screen.resolutions.Distinct().ToArray();
        string[] strRes = new string[resolutions.Length];

        for (int i = resolutions.Length-1; i >= 0; i--)
        {
            strRes[i] = resolutions[i].width.ToString() + "x" + resolutions[i].height.ToString();
        }

        if (PlayerPrefs.GetInt("IsFullScreen") == 1)
        {
            curIsFullscreen = true;
        }
        else
        {
            curIsFullscreen = false;
        }

        fScreen.isOn = curIsFullscreen;
        curRes = resolutions[PlayerPrefs.GetInt("Resolution")];

        Screen.SetResolution(curRes.width, curRes.height, curIsFullscreen);

        dropdown.ClearOptions();
        dropdown.AddOptions(strRes.ToList());
    }

    public void SetResolution()
    {
        Screen.SetResolution(resolutions[dropdown.value].width, resolutions[dropdown.value].height, curIsFullscreen);
        PlayerPrefs.SetInt("Resolution", dropdown.value);
    }

    public void FullScreenChange()
    {
        Screen.fullScreen = fScreen.isOn;
        curIsFullscreen = fScreen.isOn;
        if (fScreen.isOn)
        {
            PlayerPrefs.SetInt("IsFullScreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("IsFullScreen", 0);
        }
    }
}
