using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private const float HORIZONTAL_INPUT_SCALE = 50f; // m/s^2
    private const float VERTICAL_INPUT_SCALE = 50f; // m/s^2
    private const float MAX_MOVE_SPEED = 8f; // m/s
    private const float DECELERATION_RATE = 100f; // m/s^2

    private const float JUMP_SPEED = 5f; // m/s
    private const float MIN_JUMP_INTERVAL = 0.5f;

    private float hInput;
    private float vInput;
    private bool spaceHeld;
    private bool shiftPressed;

    public GameObject roomObject;
    public GameObject keyObject;
    public GameObject starObject;

    private float jumpTimer;

    private Rigidbody rigidBody;

    // Use this for initialization
    void Start () {
        this.rigidBody = transform.GetComponent<Rigidbody>();


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        spaceHeld = Input.GetKey(KeyCode.Space);
        shiftPressed = Input.GetKeyDown(KeyCode.LeftShift);

        bool leftClick = Input.GetMouseButtonDown(0); // left click

        CheckVision(leftClick);
    }

    void FixedUpdate() {
        // Normalize velocity as if we were at scale 1f for growth phases.
        Vector3 velocity = rigidBody.velocity / transform.localScale.x;
        velocity = CalculateHorizontalMovement(velocity, hInput, vInput);
        velocity = CalculateVerticalMovement(velocity, spaceHeld, shiftPressed);

        // Remap velocity to our current growth scale.
        rigidBody.velocity = velocity * transform.localScale.x;
    }

    public void CheckVision(bool leftClick) {
        RaycastHit hitInfo = new RaycastHit();
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 4f * transform.localScale.x, Color.red);
        bool hit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo);

        if (hit) {
            float distance = hitInfo.distance;
            
            if (distance < 4f * transform.localScale.x) {
                // Highlight the object if it's a cool, and click if mouse button was pressed.
                RaycastClickable clickable = hitInfo.collider.gameObject.GetComponent<RaycastClickable>();
                if (clickable) {
                    if (!clickable.RequiresItem() || keyObject.activeInHierarchy) {
                        clickable.Highlight();
                        if (leftClick) {
                            clickable.Click();
                        }
                    }
                }

                PlaceableArea placeableArea = hitInfo.collider.gameObject.GetComponent<PlaceableArea>();
                if (placeableArea && HoldingSomething()) {
                    placeableArea.Highlight();
                    if (leftClick) {
                        placeableArea.Click();
                    }
                }
            }
        }
    }

    public void RescaleSelf(float newScale) {
        float currentScale = transform.localScale.x;
        float ratio = newScale / currentScale;
        transform.localScale = Vector3.one * newScale;
        transform.position *= ratio;
        rigidBody.velocity *= ratio;
    }

    internal void HoldRoom() {
        roomObject.SetActive(true);
    }

    internal void HoldKey() {
        keyObject.SetActive(true);
    }

    internal void HoldStar() {
        starObject.SetActive(true);
    }

    internal void PlaceRoom() {
            roomObject.SetActive(false);
        }

    internal void UseKey() {
        keyObject.SetActive(false);
    }

    internal void PlaceStar() {
        starObject.SetActive(false);
    }

    public bool HoldingSomething() {
        return roomObject.activeInHierarchy || keyObject.activeInHierarchy || starObject.activeInHierarchy;
    }

    private Vector3 CalculateHorizontalMovement(Vector3 velocity, float hInput, float vInput) {
        Vector3 velocityChange = this.transform.right * hInput * HORIZONTAL_INPUT_SCALE + this.transform.forward * vInput * VERTICAL_INPUT_SCALE;
        velocityChange *= Time.deltaTime;

        // if flatVelocity is greater than MAX_MOVE_SPEED, add velocityChange and normalize to oldMagnitude
        // if newVelocity is greater than MAX_MOVE_SPEED decelerate towards 0

        Vector3 newVelocity = Vector3.zero;
        Vector3 flatVelocity = velocity;
        flatVelocity.y = 0f;
        newVelocity = flatVelocity + velocityChange;
        // we can't use walking to accelerate faster than our MAX_MOVE_SPEED
        float flatMagnitude = newVelocity.magnitude;
        if (flatMagnitude > MAX_MOVE_SPEED) {
            newVelocity = (newVelocity + velocityChange).normalized * flatMagnitude;
        }

        if (flatMagnitude > MAX_MOVE_SPEED || velocityChange.magnitude < 0.001f) {

            Vector3 decelerationVector = newVelocity.normalized * DECELERATION_RATE * Time.deltaTime;

            // If we started going in the opposite direction, just stop instead
            if (decelerationVector.magnitude > newVelocity.magnitude) {
                newVelocity = Vector3.zero;
            }
            else {
                newVelocity = newVelocity - decelerationVector;
            }

            // if we were goin faster than MAX_MOVE_SPEED and slowed down to less than it, but are still moving, then clamp
            if (newVelocity.magnitude < MAX_MOVE_SPEED && velocityChange.magnitude > 0.001f) {
                newVelocity = newVelocity.normalized * MAX_MOVE_SPEED;
            }
        }

        return new Vector3(newVelocity.x, velocity.y, newVelocity.z);
    }

    private Vector3 CalculateVerticalMovement(Vector3 velocity, bool spaceHeld, bool shiftPressed) {
        float verticalVelocity = 0f;

        // Apply gravity.
        verticalVelocity += Physics.gravity.y * Time.deltaTime; 

        jumpTimer += Time.deltaTime;
        if (spaceHeld && IsGrounded() && jumpTimer > MIN_JUMP_INTERVAL) {
            verticalVelocity += JUMP_SPEED;
            jumpTimer = 0f;
        }

        return velocity + verticalVelocity * transform.up;
    }

    private bool IsGrounded() {
        Debug.DrawRay(transform.position, -transform.up * 1.2f * transform.localScale.x, Color.red);
        return Physics.Raycast(transform.position, -transform.up * transform.localScale.x, 1.2f);
    }

    private void OnCollisionEnter(Collision collision) {

    }

    // Called when exiting this house, should switch to the next one.
    private void OnTriggerEnter(Collider other) {
        TimeHouseController house = other.GetComponentInParent<TimeHouseController>();
        if (house) {
            if (!house.HasExited()) {
                GameController.instance.CreateNewHouse();
                house.Exit();
            }
        }

        if (other.name == "WinTriggerZone") {
            GameController.instance.EndGame();
        }
    }
}
