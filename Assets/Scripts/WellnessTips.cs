using UnityEngine;

public class WellnessTips : MonoBehaviour
{
    [SerializeField] GameObject[] wellnessTips;
    [SerializeField] GameObject wellnessPanel;

    public int wellnessTipsIndex = 0;

    private int lastDisplayCount = 0;

    void Update()
    {
        if (MasterInfo.fruitCount >= 5 && MasterInfo.fruitCount / 5 > lastDisplayCount)
        {
            lastDisplayCount = MasterInfo.fruitCount / 5;
            DisplayWellness();
        }
    }

    public void DisplayWellness()
    {
        if (wellnessTipsIndex < wellnessTips.Length)
        {
            wellnessPanel.SetActive(true);
            wellnessTips[wellnessTipsIndex].SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void CloseWellness()
    {
        wellnessPanel.SetActive(false);
        wellnessTips[wellnessTipsIndex].SetActive(false);
        wellnessTipsIndex++;
        Time.timeScale = 1f;
    }
}