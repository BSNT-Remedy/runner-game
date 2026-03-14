using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    public GateManager manager;  // assign in Inspector
    public int gateNumber;       // 1, 2, or 3

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trigger"))
        {
            manager.ChooseGate(gateNumber);
        }
    }
}