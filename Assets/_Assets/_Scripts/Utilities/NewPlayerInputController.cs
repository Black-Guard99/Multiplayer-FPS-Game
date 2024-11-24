using UnityEngine;
using Baracuda.Monitoring;
using TABBB.Tools.Console;
using UnityEngine.EventSystems;
[DefaultExecutionOrder(-2)]
public class NewPlayerInputController : MonoBehaviour {
    [SerializeField] private bool pc = true;
    [SerializeField] private Joystick movementJoyStick;
    [SerializeField,Range(2,4)] private float lookInteractionArea = 2f;
    // private GameObject shootBtn;
    /* [Monitor]  */private int rightFingerId;
    /* [Monitor]  */private float halfScreenWidth;
    /* [Monitor]  */private Vector2 moveInput;
    /* [Monitor]  */private Vector2 lookInput;
    /* [Monitor]  */private bool Sprint;
    /* [Monitor]  */private bool isTouchingShootingBtn;
    /* [Monitor]  */private float crossHairExpandAmount;
    /* [Monitor]  */private bool isTouchingOnRightSide;
    /* [Monitor]  */private Vector2 moveTouchStartPosition;
    /* [Monitor]  */private int TouchCount;
    /* [Monitor]  */private int leftFingerId;
    // private Panel panel;
    private void Awake() {
        Monitor.StartMonitoring(this);
        // Or use this extension method:
        this.StartMonitoring();
    }

    private void OnDestroy() {
        Monitor.StopMonitoring(this);
        // Or use this extension method:
        this.StopMonitoring();
    }
    private void Start(){
        // Create a new panel called "Demo Panel" and automatically select it
        // panel = Console.I.AddPanel("My Pannel", true);
        // id = -1 means the finger is not being tracked
        rightFingerId = -1;

        // only calculate once
        halfScreenWidth = Screen.width / lookInteractionArea;
    }
    public Vector2 GetMoveVector{
        get{
            return moveInput;
        }
    }
    public Vector2 GetLookVector{
        get{
            return lookInput;
        }
    }
    private void Update(){
        if(pc){
            GetPcInput();
        }else{
            GetTouchInput();
        }
        if(moveInput.magnitude >= 0.01f){
            crossHairExpandAmount = (Sprinting ? 1f: 0.5f) * moveInput.magnitude;
        }else{
            if(lookInput.magnitude >= .01f){
                if(GetPcTap || isTouchingShootingBtn){
                    crossHairExpandAmount = 1f;
                } else{
                    crossHairExpandAmount = .5f;
                }
            }else{
                crossHairExpandAmount = 0f;
            }
            
            
        }
        CrossHairMovement.Current?.SetCrossHairLines(crossHairExpandAmount);
    }
    private void GetPcInput(){
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
        // if(!EventSystem.current.IsPointerOverGameObject())
        lookInput = new Vector2(Input.GetAxisRaw("Mouse X"),Input.GetAxisRaw("Mouse Y"));
        
        // CrossHair Movment.............
        if(crossHairExpandAmount >= 0.01f){
            // CrossHairMovement.Current.SetCrossHairLines(crossHairExpandAmount);
        }
        
    }
    private void GetTouchInput() {
        // Iterate through all the detected touches
        moveInput = new Vector2(movementJoyStick.Horizontal,movementJoyStick.Vertical);
        Sprint = moveInput.y >= 0.85f;
#region  UnUsed Code......
        // for (int i = 0; i < Input.touchCount; i++) {
            // Touch t = Input.GetTouch(i);
            // if (t.position.x > halfScreenWidth && rightFingerId == -1) {
            //     // Start tracking the rightfinger if it was not previously being tracked
            //     rightFingerId = t.fingerId;
            // }
            // if(EventSystem.current.IsPointerOverGameObject(t.fingerId)){
            //     if(!isTouchingShootingBtn){
            //         return;
            //     }else{
            //         lookInput = Vector2.zero;
            //         panel.AddInfo("Look Input ",lookInput.ToString());
            //         lookInput = Vector2.zero;
            //     }
            // }
            // // Check each touch's phase
            // switch (t.phase) {
            //     case TouchPhase.Began:
            //         if(t.position.x < halfScreenWidth && rightFingerId != -1){
            //             rightFingerId = -1;
            //         }
            //         if(rightFingerId == -1){
            //             lookInput = Vector2.zero;
            //             return;
            //         }
            //         if (t.fingerId == rightFingerId) {
            //             /* float xDelta = t.deltaPosition.x;
            //             float yDelta = t.deltaPosition.y;
            //             xDelta = Mathf.Clamp(xDelta,-1,1);
            //             yDelta = Mathf.Clamp(yDelta,-1,1);
            //             lookInput = new Vector2(xDelta,yDelta); */
            //             lookInput = t.deltaPosition;
            //         }
            //         break;
            //     case TouchPhase.Ended:
            //     case TouchPhase.Canceled:
            //         lookInput = Vector2.zero;
            //         if (t.fingerId == rightFingerId) {
            //             // Stop tracking the right finger
            //             rightFingerId = -1;
            //             lookInput = Vector2.zero;
            //             panel.AddInfo("Stopped tracking right finger","");
            //         }
            //         break;
            //     case TouchPhase.Moved:
            //         // Get input for looking around
            //         if(t.position.x < halfScreenWidth && rightFingerId != -1){
            //             rightFingerId = -1;
            //         }
            //         if(rightFingerId == -1){
            //             lookInput = Vector2.zero;
            //             return;
            //         }
            //         if (t.fingerId == rightFingerId) {
            //             /* float xDelta = t.deltaPosition.x;
            //             float yDelta = t.deltaPosition.y;
            //             xDelta = Mathf.Clamp(xDelta,-1,1);
            //             yDelta = Mathf.Clamp(yDelta,-1,1);
            //             lookInput = new Vector2(xDelta,yDelta); */
            //             lookInput = t.deltaPosition;
            //             panel.AddInfo("Look Input ",lookInput.ToString());
            //         }
            //     break;
            //     case TouchPhase.Stationary:
            //         // Set the look input to zero if the finger is still
            //         lookInput = Vector2.zero;
            //         /* if (t.fingerId == rightFingerId) {
            //         } */
            //     break;
            // }
        // }
#endregion
        // Iterate through all the detected touches
        TouchCount = Input.touchCount;
        for (int i = 0; i < Input.touchCount; i++) {
            Touch t = Input.GetTouch(i);
            // Check each touch's phase
            switch (t.phase) {
                case TouchPhase.Began:
                    if (t.position.x < halfScreenWidth && leftFingerId == -1) {
                        // Start tracking the left finger if it was not previously being tracked
                        leftFingerId = t.fingerId;

                        // Set the start position for the movement control finger
                        moveTouchStartPosition = t.position;
                        isTouchingOnRightSide = false;
                    } else if (t.position.x > halfScreenWidth && rightFingerId == -1) {
                        isTouchingOnRightSide = true;
                        // Start tracking the rightfinger if it was not previously being tracked
                        rightFingerId = t.fingerId;
                    }
                break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (t.fingerId == leftFingerId) {
                        // Stop tracking the left finger
                        leftFingerId = -1;
                        // panel.AddInfo("Stopped tracking left finger","Tracking Finger");
                        // moveInput = Vector2.zero;
                    } else if (t.fingerId == rightFingerId) {
                        // Stop tracking the right finger
                        rightFingerId = -1;
                        lookInput = Vector2.zero;
                        
                        // panel.AddInfo("Stopped tracking right finger","Tracking Finger");
                    }
                    lookInput = Vector2.zero;
                break;
                case TouchPhase.Moved:
                    // Get input for looking around
                    if (t.fingerId == rightFingerId) {
                        if(EventSystem.current.IsPointerOverGameObject(rightFingerId)){
                            if(isTouchingShootingBtn){
                                lookInput = t.deltaPosition;
                            }else{
                                lookInput = Vector2.zero;
                            }
                        }else{
                            lookInput = t.deltaPosition;
                        }
                    } else if (t.fingerId == leftFingerId) {
                        // calculating the position delta from the start position
                        // moveInput = t.position - moveTouchStartPosition;
                    }

                break;
                case TouchPhase.Stationary:
                    // Set the look input to zero if the finger is still
                    if (t.fingerId == rightFingerId) {
                        lookInput = Vector2.zero;
                    }
                break;
            }
        }


    }

    public bool GetPcAimToggle{
        get{
            return Input.GetMouseButtonDown(1) && pc;
        }
    }
    public bool GetPcTap{
        get{
            return Input.GetMouseButtonDown(0) && pc && !EventSystem.current.IsPointerOverGameObject();
        }
    }
    public bool GetPcTapHold{
        get{
            return Input.GetMouseButton(0) && pc && !EventSystem.current.IsPointerOverGameObject();
        }
    }
    public bool GetPcTapEnd{
        get{
            return Input.GetMouseButtonUp(0) && pc && !EventSystem.current.IsPointerOverGameObject();
        }
    }
    public bool RelaodingInput{
        get{
            return Input.GetKeyDown(KeyCode.R);
        }
    }
    public bool GetPcInputPress(KeyCode jumpKey){
        return Input.GetKeyDown(jumpKey);
        
    }
    public bool Sprinting{
        get{
            if(pc){
                return Input.GetKey(KeyCode.LeftShift);
            }
            return Sprint;
        }
    }
    public bool GetPcInputPressing(KeyCode key){
        return Input.GetKey(key);
    }
    
    public void SetOnPointerOnShootBtn(bool isOnShoot,GameObject shootButn){
        this.isTouchingShootingBtn = isOnShoot;
        // this.shootBtn = shootButn;
    }
    public bool OnPc{
        get{
            return pc;
        }
    }
    
}