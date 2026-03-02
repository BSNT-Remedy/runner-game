using UnityEngine;

public class CollectFruit : MonoBehaviour
{
    
    [SerializeField] AudioSource fruitFX;

    void OnTriggerEnter(Collider other)
    {
        fruitFX.Play();
        MasterInfo.fruitCount += 1;
        this.gameObject.SetActive(false);
    }
}
