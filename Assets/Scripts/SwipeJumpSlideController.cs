using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeJumpSlideController : MonoBehaviour
{
    [SerializeField] GameObject cam;
    [SerializeField] GameObject subCam;
    
    [Header("Swipe Settings")]
    public float minSwipeDistance = 50f;
    private Vector2 startTouch;
    private bool isSwiping = false;

    [Header("Jump Settings (No Physics)")]
    public float jumpHeight = 3f;
    public float jumpDuration = 0.35f;
    private bool isJumping = false;
    
    [Header("Slide Settings")]
    public float slideDuration = 0.35f;
    private bool isSliding = false;

    private CapsuleCollider col;
    private float originalHeight;
    private Vector3 originalPosition;

    Vector3 freezeCamPos;
    Quaternion freezeCamRot;
    private Vector3 subCamInitialLocalPos;
    private Quaternion subCamInitialLocalRot;

    void Start()
    {
        col = GetComponent<CapsuleCollider>();
        originalHeight = col.height;
        originalPosition = transform.localPosition;
 
        subCamInitialLocalPos = subCam.transform.localPosition;
        subCamInitialLocalRot = subCam.transform.localRotation;
    }

    void Update()
    {
        DetectSwipe();
    }

    

    // -------------------------------------------------
    // SWIPE DETECTION (EDITOR + MOBILE)
    // -------------------------------------------------
    void DetectSwipe()
    {
        // Editor / Mouse
        if (Input.GetMouseButtonDown(0))
        {
            isSwiping = true;
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            Vector2 delta = (Vector2)Input.mousePosition - startTouch;
            HandleSwipe(delta);
            isSwiping = false;
        }

        // Mobile Touch
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                isSwiping = true;
                startTouch = t.position;
            }
            else if (t.phase == TouchPhase.Ended && isSwiping)
            {
                Vector2 delta = t.position - startTouch;
                HandleSwipe(delta);
                isSwiping = false;
            }
        }
    }

    // -------------------------------------------------
    // SWIPE LOGIC (UP = Jump, DOWN = Slide)
    // -------------------------------------------------
    void HandleSwipe(Vector2 delta)
    {
        if (delta.magnitude < minSwipeDistance)
            return;

        float x = Mathf.Abs(delta.x);
        float y = Mathf.Abs(delta.y);

        if (y > x)
        {
            if (delta.y > 0) Jump();
            else Slide();
        }
    }

    // -------------------------------------------------
    // JUMP (TRANSFORM MOVEMENT, NO PHYSICS)
    // -------------------------------------------------
    void Jump()
    {
        if (isJumping) return;

        StartCoroutine(JumpRoutine());
    }

    System.Collections.IEnumerator JumpRoutine()
    {
        isJumping = true;
        FreezeCamera();

        float half = jumpDuration / 2f;
        float timer = 0f;

        float baseY = transform.localPosition.y;
        float peakY = baseY + jumpHeight;

        Vector3 pos;

        // UPWARD PHASE
        while (timer < half)
        {
            timer += Time.deltaTime;
            float t = timer / half;

            pos = transform.localPosition;
            pos.y = Mathf.Lerp(baseY, peakY, t);
            transform.localPosition = pos;

            yield return null;
        }

        // DOWNWARD PHASE
        timer = 0f;
        while (timer < half)
        {
            timer += Time.deltaTime;
            float t = timer / half;

            pos = transform.localPosition;
            pos.y = Mathf.Lerp(peakY, baseY, t);
            transform.localPosition = pos;

            yield return null;
        }

        if(!isSliding){
            UnFreezeCamera();
        }
        
        isJumping = false;
    }



    void Slide()
    {
        if (isSliding) return;

        StartCoroutine(SlideRoutine());
    }

    System.Collections.IEnumerator SlideRoutine()
    {
        isSliding = true;
        FreezeCamera();

        // --- ROTATE TO -90 ---
        float duration = 0.2f; // how fast to tilt down
        float time = 0f;

        float startX = transform.localEulerAngles.x;
        float targetX = -90f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            Vector3 rot = transform.localEulerAngles;
            rot.x = Mathf.LerpAngle(startX, targetX, t);
            transform.localEulerAngles = rot;

            yield return null;
        }

        // stay sliding
        yield return new WaitForSeconds(slideDuration);

        // --- ROTATE BACK TO 0 ---
        duration = 0.2f;
        time = 0f;

        startX = transform.localEulerAngles.x;
        targetX = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            Vector3 rot = transform.localEulerAngles;
            rot.x = Mathf.LerpAngle(startX, targetX, t);
            transform.localEulerAngles = rot;

            yield return null;
        }

        if(!isJumping){
            UnFreezeCamera();
        }

        isSliding = false;
    }

    // void FreezeCamera()
    // {
    //     subCam.transform.SetParent(null, true);
    //     subCam.SetActive(true);
    //     cam.SetActive(false);
    // }

    // void UnFreezeCamera() {        
    //     subCam.transform.localPosition = subCamInitialLocalPos;
    //     subCam.transform.localRotation  = subCamInitialLocalRot;

    //     subCam.transform.SetParent(this.transform, false);
    //     cam.SetActive(true);
    //     subCam.SetActive(false);
    // }


    void FreezeCamera()
    {
        cam.SetActive(false);
    }

    void UnFreezeCamera() {        
        cam.SetActive(true);
    }

}