using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public static readonly float LARGE_HOUSE_SCALE = 10f;
    public static readonly float DISTANCE_TO_FINISH_RESCALE = 7f;

    public static readonly Color[] WALL_PUZZLE_COLORS = { Color.red, Color.green, Color.blue };

    public GameObject housePrefab;
    public TimeHouseController tinyHouse;
    public TimeHouseController miniHouse;
    public TimeHouseController currentHouse;
    public TimeHouseController largeHouse;
    public TimeHouseController giantHouse;
    public GameObject endGameUI;

    private PlayerController player;

    private bool inSizeTransition = false;
    private bool gameEnded = false;

    public class GameProgressParams {

        public bool roomButtonPressed = false;
        public bool holdingRoom1 = false;
        public bool roomPlacedP2 = false;

        public bool holdingRoom2 = false;
        public bool roomPlacedP3 = false;

        public bool holdingKey = false;
        public bool keyUsed = false;
        public Color button1Color = Color.red;
        public Color button2Color = Color.green;
        public Color button3Color = Color.blue;
        public Color button4Color = Color.red;
        public bool lightsSolvedP4 = false;

        public bool holdingStar = false;
        public bool holdingPyramid = false;

        public bool holdingDoor = false;
    }
    private GameProgressParams houseParams = new GameProgressParams();

    private void Awake() {
        if (instance) {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        // Initialize a new largehouse if one is unset in the current scene.
        if (!largeHouse) {
            GameObject newHouse = Instantiate(housePrefab);
            newHouse.transform.position = Vector3.zero;
            // current house should be scaled to 1f, make this one X times bigger.
            newHouse.transform.localScale = Vector3.one * LARGE_HOUSE_SCALE;
            largeHouse = newHouse.GetComponent<TimeHouseController>();
        }

        if (!giantHouse) {
            GameObject newHouse = Instantiate(housePrefab);
            newHouse.transform.position = Vector3.zero;
            // current house should be scaled to 1f, make this one X^2 times bigger.
            newHouse.transform.localScale = Vector3.one * LARGE_HOUSE_SCALE * LARGE_HOUSE_SCALE;
            giantHouse = newHouse.GetComponent<TimeHouseController>();
        }
    }

    // Called when player is halfway out the door of "current room"
    public void CreateNewHouse() {
        GameObject newHouse = Instantiate(housePrefab);
        newHouse.transform.position = Vector3.zero;
        // current house should be scaled to 1f, make this one X times bigger.
        newHouse.transform.localScale = Vector3.one * LARGE_HOUSE_SCALE;

        if (tinyHouse) {
            Destroy(tinyHouse.gameObject);
        }
        tinyHouse = miniHouse;
        miniHouse = currentHouse;
        currentHouse = largeHouse;
        largeHouse = giantHouse;
        giantHouse = newHouse.GetComponent<TimeHouseController>();

        // HACK: Set it so that only the current room displays exit.
        // Large room will be created with a hole in the back.
        // miniHouse.Initialize(houseParams, true);
        currentHouse.Initialize(houseParams, true);
        largeHouse.Initialize(houseParams, false);
        giantHouse.Initialize(houseParams, false);

        RescaleToNextRoom();
    }

    private void LockIntoCurrentRoom() {
        player.RescaleSelf(1f);

        // Prep the large room so that we don't see through the hole in the back wall.
        // At this point we certainly won't be leaving through the exit in current room so it should be fine?
        largeHouse.Initialize(houseParams, true);
        inSizeTransition = false;
    }

    private void RescaleToNextRoom() {
        float giantRoomScale = LARGE_HOUSE_SCALE * LARGE_HOUSE_SCALE;
        float largeRoomScale = LARGE_HOUSE_SCALE;
        float currentRoomScale = 1f;
        float miniRoomScale = 1f / LARGE_HOUSE_SCALE;
        float tinyRoomScale = 1f / (LARGE_HOUSE_SCALE * LARGE_HOUSE_SCALE);

        giantHouse.RescaleHouse(giantRoomScale);
        largeHouse.RescaleHouse(largeRoomScale);
        currentHouse.RescaleHouse(currentRoomScale);
        miniHouse.RescaleHouse(miniRoomScale);
        if (tinyHouse) {
            tinyHouse.RescaleHouse(tinyRoomScale);
            // Make the previous house inert by removing script so that we're only "exiting" from mini house.
            // Tiny house is still visually present.
            tinyHouse.enabled = false;
        }

        player.RescaleSelf(miniRoomScale);

        inSizeTransition = true;
    }

    void Update() {

#if !UNITY_WEBGL
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
#endif

        if (Input.GetMouseButtonDown(0) && !gameEnded) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (miniHouse && inSizeTransition) {
            float exitDistance = miniHouse.MaxDistanceFromExit();
            if (exitDistance < DISTANCE_TO_FINISH_RESCALE) {
                // Normalize distance traveled away from the door and then remap to scale each room accordingly.
                float exitProgress = Mathf.InverseLerp(0f, DISTANCE_TO_FINISH_RESCALE, miniHouse.MaxDistanceFromExit());

                //float largeRoomScale = Mathf.Lerp(LARGE_HOUSE_SCALE, 1f, exitProgress);
                //float miniRoomScale = Mathf.Lerp(1f, 1f / LARGE_HOUSE_SCALE, exitProgress);

                //currentHouse.transform.localScale = Vector3.one * largeRoomScale;
                //miniHouse.transform.localScale = Vector3.one * miniRoomScale;

                player.transform.localScale = Vector3.one * Mathf.Lerp(1f/LARGE_HOUSE_SCALE, 1f, exitProgress);
            }
            else {
                LockIntoCurrentRoom();
            }
        }
        


    }


    public void RoomButtonPressed() {
        houseParams.roomButtonPressed = true;
        RefreshHouseContents();
        ProgressFX();
    }

    public void HoldingRoom1() {
        player.HoldRoom();
        houseParams.holdingRoom1 = true;
        RefreshHouseContents();
    }

    public void RoomPlacedP2() {
        player.PlaceRoom();
        houseParams.roomPlacedP2 = true;
        RefreshHouseContents();
        ProgressFX();
    }

    public void HoldingRoom2() {
        player.HoldRoom();
        houseParams.holdingRoom2 = true;
        RefreshHouseContents();
    }

    public void RoomPlacedP3() {
        player.PlaceRoom();
        houseParams.roomPlacedP3 = true;
        RefreshHouseContents();
        ProgressFX();
    }

    public void HoldingKey() {
        player.HoldKey();
        houseParams.holdingKey = true;
        RefreshHouseContents();
    }

    public void KeyUsed() {
        player.UseKey();
        houseParams.keyUsed = true;
        RefreshHouseContents();
        ProgressFX();
    }

    private Color CalculateNextColor(Color color) {
        for (int i = 0; i < WALL_PUZZLE_COLORS.Length; i++) {
            if (WALL_PUZZLE_COLORS[i] == color) {
                int nextIndex = (i+1) % WALL_PUZZLE_COLORS.Length;
                return WALL_PUZZLE_COLORS[nextIndex];
            }
        }
        return Color.black;
    }
    public void ToggleColor1() {
        if (ColorPuzzleSolved()) return;
        houseParams.button1Color = CalculateNextColor(houseParams.button1Color);
        if (ColorPuzzleSolved()) {
            LightsSolvedP4();
        }
        RefreshHouseContents();
    }

    public void ToggleColor2() {
        if (ColorPuzzleSolved()) return;
        houseParams.button2Color = CalculateNextColor(houseParams.button2Color);
        if (ColorPuzzleSolved()) {
            LightsSolvedP4();
        }
        RefreshHouseContents();
    }

    public void ToggleColor3() {
        if (ColorPuzzleSolved()) return;
        houseParams.button3Color = CalculateNextColor(houseParams.button3Color);
        if (ColorPuzzleSolved()) {
            LightsSolvedP4();
        }
        RefreshHouseContents();
    }

    public void ToggleColor4() {
        if (ColorPuzzleSolved()) return;
        houseParams.button4Color = CalculateNextColor(houseParams.button4Color);
        if (ColorPuzzleSolved()) {
            LightsSolvedP4();
        }
        RefreshHouseContents();
    }

    public void LightsSolvedP4() {
        houseParams.lightsSolvedP4 = true;
        RefreshHouseContents();
        ProgressFX();
    }

    private void ProgressFX() {
        AudioController.instance.PlayGlitchSound();

        //ShaderEffect_CorruptedVram glitchScatter = Camera.main.GetComponent<ShaderEffect_CorruptedVram>();
        //glitchScatter.enabled = true;
        //ShaderEffect_BleedingColors glitchColors = Camera.main.GetComponent<ShaderEffect_BleedingColors>();
        //glitchColors.enabled = true;

        //LeanTween.delayedCall(0.1f, () => glitchScatter.enabled = false);
        //LeanTween.delayedCall(0.1f, () => glitchColors.enabled = false);
    }

    private bool ColorPuzzleSolved() {
        return
            (houseParams.button1Color == Color.green
            && houseParams.button2Color == Color.green
            && houseParams.button3Color == Color.red
            && houseParams.button4Color == Color.red)
            ||
            (houseParams.button1Color == Color.red
            && houseParams.button2Color == Color.red
            && houseParams.button3Color == Color.green
            && houseParams.button4Color == Color.green);
        }

    private void RefreshHouseContents() {
        miniHouse.Initialize(houseParams, false);
        currentHouse.AdjustButtons(houseParams);
        // currentHouse.Initialize(houseParams);

        // hackiest hack to make it so that if we pick anything else up in the final phase,
        // it won't make the back wall of the current room disappear.
        if (houseParams.lightsSolvedP4) {
            largeHouse.Initialize(houseParams, true);
        }
        else {
            largeHouse.Initialize(houseParams, false);
        }
        giantHouse.Initialize(houseParams, false);
    }



    public void EndGame() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        gameEnded = true;
        endGameUI.SetActive(true);
    }

    public void RestartGame() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        gameEnded = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
