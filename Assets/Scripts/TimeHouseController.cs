using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeHouseController : MonoBehaviour
{
    public GameObject houseLayout1;
    public GameObject houseLayout2;
    public GameObject houseLayout3;
    public GameObject houseLayout4Exit;
    public GameObject houseLayout4Noback;

    public GameObject exitDoorwayObject;
    public Light greenExitLight;
    public Light exitGreenLight;

    public GameObject newRoomButton;
    public GameObject newRoomObject1;
    public GameObject newRoomInteractableParent1;
    public GameObject newRoomObject2;
    public GameObject newRoomInteractableParent2;

    public GameObject keyObject;
    public GameObject keyCoverObject;
    public GameObject keyInteractableParent;
    public GameObject colorHint;
    public GameObject colorPuzzle;
    public Renderer colorDisplay1;
    public Renderer colorDisplay2;
    public Renderer colorDisplay3;
    public Renderer colorDisplay4;



    private PlayerController player;
    private bool exited = false;
    private float maxExitDistance = 0f;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (exited) {
            float zDist = (player.transform.position - exitDoorwayObject.transform.position).z;
            float xDist = 0f; // (player.transform.position - exitDoorwayObject.transform.position).x;

            float exitDistance = Mathf.Max(xDist, zDist);
            if (exitDistance > maxExitDistance) {
                maxExitDistance = exitDistance;
            }
        }
    }

    public bool HasExited() {
        return exited;
    }

    public void Exit() {
        exited = true;
    }

    public float MaxDistanceFromExit() {
        return maxExitDistance;
    }





    public void RoomButtonPressed() {
        GameController.instance.RoomButtonPressed();
    }

    public void HoldingRoom1() {
        GameController.instance.HoldingRoom1();
    }

    public void RoomPlacedP2() {
        GameController.instance.RoomPlacedP2();
    }

    public void HoldingRoom2() {
        GameController.instance.HoldingRoom2();
    }

    public void RoomPlacedP3() {
        GameController.instance.RoomPlacedP3();
    }

    public void HoldingKey() {
        GameController.instance.HoldingKey();
    }

    public void KeyUsed() {
        GameController.instance.KeyUsed();
    }

    public void ToggleColor1() {
        GameController.instance.ToggleColor1();
    }

    public void ToggleColor2() {
        GameController.instance.ToggleColor2();
    }

    public void ToggleColor3() {
        GameController.instance.ToggleColor3();
    }

    public void ToggleColor4() {
        GameController.instance.ToggleColor4();
    }

    public void LightsSolvedP4() {
        GameController.instance.LightsSolvedP4();
    }

    internal void Initialize(GameController.GameProgressParams houseParams, bool isCurrentRoom) {

        houseLayout1.SetActive(true);

        if (houseParams.roomButtonPressed) {
            newRoomInteractableParent1.SetActive(true);
        }

        if (houseParams.holdingRoom1) {
            newRoomObject1.SetActive(false);
        }

        if (houseParams.roomPlacedP2) {
            newRoomInteractableParent2.SetActive(true);
            keyCoverObject.SetActive(true);
            colorHint.SetActive(true);
            colorPuzzle.SetActive(true);
            houseLayout1.SetActive(false);
            houseLayout2.SetActive(true);
            // firstRoomStairs.SetActive(true);
        }

        if (houseParams.holdingRoom2) {
            newRoomObject2.SetActive(false);
        }

        if (houseParams.roomPlacedP3) {
            keyInteractableParent.SetActive(true);
            houseLayout2.SetActive(false);
            houseLayout3.SetActive(true);
        }

        if (houseParams.holdingKey) {
            keyObject.SetActive(false);
        }

        if (houseParams.keyUsed) {
            keyCoverObject.SetActive(false);
        }

        if (houseParams.lightsSolvedP4) {
            houseLayout2.SetActive(false);
            houseLayout3.SetActive(false);

            if (isCurrentRoom) {
                houseLayout4Noback.SetActive(false);
                houseLayout4Exit.SetActive(true);
            } else {
                houseLayout4Exit.SetActive(false);
                houseLayout4Noback.SetActive(true);
            }
        }

        if (houseParams.holdingStar) {

        }

        if (houseParams.holdingPyramid) {

        }

        AdjustButtons(houseParams);
    }

    internal void AdjustButtons(GameController.GameProgressParams houseParams) {

        if (houseParams.roomButtonPressed) {
            newRoomButton.SetActive(false);
        }

        if (houseParams.holdingRoom1) {
            newRoomObject1.SetActive(false);
        }

        if (houseParams.roomPlacedP2) {

        }

        if (colorDisplay1.gameObject.activeInHierarchy) {
            colorDisplay1.GetComponent<RaycastClickable>().SetColor(houseParams.button1Color);
        }
        if (colorDisplay2.gameObject.activeInHierarchy) {
            colorDisplay2.GetComponent<RaycastClickable>().SetColor(houseParams.button2Color);
        }
        if (colorDisplay3.gameObject.activeInHierarchy) {
            colorDisplay3.GetComponent<RaycastClickable>().SetColor(houseParams.button3Color);
        }
        if (colorDisplay4.gameObject.activeInHierarchy) {
            colorDisplay4.GetComponent<RaycastClickable>().SetColor(houseParams.button4Color);
        }

        if (houseParams.holdingRoom2) {
            newRoomObject2.SetActive(false);
        }

        if (houseParams.roomPlacedP3) {

        }

        if (houseParams.holdingKey) {
            keyObject.SetActive(false);
        }

        if (houseParams.keyUsed) {

        }

        if (houseParams.lightsSolvedP4) {

        }

        if (houseParams.holdingStar) {

        }

        if (houseParams.holdingPyramid) {

        }
    }

    public void RescaleHouse(float scale) {
        transform.localScale = Vector3.one * scale;
        greenExitLight.range = scale * 5f;
        exitGreenLight.range = scale * 2f;
    }
}
