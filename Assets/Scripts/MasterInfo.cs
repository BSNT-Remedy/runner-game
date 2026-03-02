using UnityEngine;

public class MasterInfo : MonoBehaviour
{
    public static int coinCount = 0;
    public static int fruitCount = 0;
    [SerializeField] GameObject coinDisplay;
    public static int distanceRun;
    [SerializeField] int internalDistance;
    [SerializeField] GameObject runDisplay;
    [SerializeField] GameObject fruitDisplay;
    void Update()
    {
        internalDistance = distanceRun;
        coinDisplay.GetComponent<TMPro.TMP_Text>().text = "" + coinCount;
        runDisplay.GetComponent<TMPro.TMP_Text>().text = "" + distanceRun;
        fruitDisplay.GetComponent<TMPro.TMP_Text>().text = "" + fruitCount;
    }
}
