using UnityEngine;

public class MasterInfo : MonoBehaviour
{
    public static int coinCount = 0;
    public static int fruitCount = 0;
    [SerializeField] GameObject coinDisplay;
    public static int distanceRun;
    [SerializeField] int internalDistance;
    void Update()
    {
        internalDistance = distanceRun;
        coinDisplay.GetComponent<TMPro.TMP_Text>().text = "COINS: " + coinCount;
    }
}
