using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ResolutionSettings : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions = new List<Resolution>();
    private int currentResolutionIndex = 0;

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int refreshRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (Mathf.RoundToInt((float)resolutions[i].refreshRateRatio.value) == refreshRate &&
                !filteredResolutions.Exists(r => r.width == resolutions[i].width && r.height == resolutions[i].height))
            {
                filteredResolutions.Add(resolutions[i]);

                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = filteredResolutions.Count - 1;
                }
            }
        }


        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = Screen.fullScreen;
    }

    public void ApplySettings()
    {
        Resolution res = filteredResolutions[resolutionDropdown.value];
        Screen.SetResolution(res.width, res.height, fullscreenToggle.isOn);
    }
}
