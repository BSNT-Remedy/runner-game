using UnityEngine;

public class ModeManager : MonoBehaviour
{
    public enum Mode { None, Grab, Rotate, Scale }
    [Tooltip("Current manipulation mode. Set via UI buttons.")]
    public Mode currentMode = Mode.None;

    public void SetGrabMode()   { currentMode = Mode.Grab; }
    public void SetRotateMode() { currentMode = Mode.Rotate; }
    public void SetScaleMode()  { currentMode = Mode.Scale; }
    public void ClearMode()     { currentMode = Mode.None; }
}