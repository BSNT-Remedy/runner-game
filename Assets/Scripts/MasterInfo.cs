using UnityEngine;

public class MasterInfo : MonoBehaviour
{
    public static int coinCount = 0;
    public static int fruitCount = 0;
    [SerializeField] GameObject coinDisplay;
    public static int distanceRun = 0;
    // [SerializeField] int internalDistance;
    public static int internalDistance = 0;
    [SerializeField] GameObject runDisplay;
    [SerializeField] GameObject fruitDisplay;

    void Start()
    {
        coinCount = 0;
        fruitCount = 0;
        distanceRun = 0;
    }
    void Update()
    {
        internalDistance = distanceRun;
        coinDisplay.GetComponent<TMPro.TMP_Text>().text = "" + coinCount;
        runDisplay.GetComponent<TMPro.TMP_Text>().text = "" + distanceRun;
        fruitDisplay.GetComponent<TMPro.TMP_Text>().text = "" + fruitCount;
    }
}
