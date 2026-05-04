using UnityEngine;
using UnityEngine.EventSystems;

public class LevelEditorManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelGenerator levelGenerator;
    [SerializeField] private PlayerController testPlayer;
    [SerializeField] private GameObject editorUIPanel;

    [Header("Tilemap Visuals")]
    [SerializeField] private Transform hoverCursor;

    [Header("Settings")]
    [SerializeField] private Vector2Int mapSize = new Vector2Int(10, 15);

    [Header("Cameras")]
    [SerializeField] private GameObject editorCamera;
    [SerializeField] private GameObject gameplayCamera;

    private LevelData editorData;
    private int currentBrush = 1;
    private float currentBrushRotation = 0f;
    private bool isTesting = false;
    private int currentEditingLevel = 0;

    public int CurrentBrush => currentBrush;
    public bool IsTesting => isTesting;
    public int CurrentEditingLevel => currentEditingLevel;

    void Start()
    {
        testPlayer.gameObject.SetActive(false);
        InitializeBlankMap();
    }

    public void InitializeBlankMap()
    {
        currentEditingLevel = 0;
        editorData = new LevelData();
        for (int z = 0; z < mapSize.y; z++)
        {
            MapRow row = new MapRow();
            for (int x = 0; x < mapSize.x; x++) row.columns.Add(new TileData(99, 0f));
            editorData.grid.Add(row);
        }
        levelGenerator.BuildLevelFromData(editorData);
    }

    void Update()
    {
        if (isTesting || EventSystem.current.IsPointerOverGameObject())
        {
            if (hoverCursor != null) hoverCursor.gameObject.SetActive(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentBrush == 6 || currentBrush == -2)
            {
                currentBrushRotation += 90f;
                if (currentBrushRotation >= 360f) currentBrushRotation = 0f;

                if (hoverCursor != null)
                {
                    hoverCursor.rotation = Quaternion.Euler(90, currentBrushRotation, 0);
                }
            }
            else
            {
                Debug.Log("Khoi nay khong duoc xoay");
            }
        }

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            int x = Mathf.RoundToInt(hitPoint.x / levelGenerator.tileSize);
            int z = Mathf.RoundToInt(hitPoint.z / levelGenerator.tileSize);
            int zIndex = mapSize.y - z - 1;

            if (x >= 0 && x < mapSize.x && z >= 0 && z < mapSize.y)
            {
                if (hoverCursor != null)
                {
                    hoverCursor.gameObject.SetActive(true);
                    hoverCursor.position = new Vector3(x * levelGenerator.tileSize, 0.1f, z * levelGenerator.tileSize);
                }
                if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                {
                    PaintTile(x, zIndex, Input.GetMouseButton(1));
                }
            }
            else
            {
                if (hoverCursor != null) hoverCursor.gameObject.SetActive(false);
            }
        }
    }

    private void PaintTile(int x, int zIndex, bool isErase)
    {
        int targetBrush = isErase ? 99 : currentBrush;
        float targetRot = (isErase || (targetBrush != 6 && targetBrush != -2)) ? 0f : currentBrushRotation;
        TileData currentTile = editorData.grid[zIndex].columns[x];
        if (currentTile.id != targetBrush || currentTile.rotY != targetRot)
        {
            currentTile.id = targetBrush;
            currentTile.rotY = targetRot;
            levelGenerator.BuildLevelFromData(editorData);
        }
    }

    public void SetBrushType(int brushType)
    {
        currentBrush = brushType;
        currentBrushRotation = 0f;
        if (hoverCursor != null) hoverCursor.rotation = Quaternion.Euler(90, 0, 0);
    }

    public void ToggleTestPlay()
    {
        if (!isTesting && !levelGenerator.HasStartPosition())
        {
            Debug.LogWarning("khong the test vi chua co diem bat dau");
            return;
        }

        isTesting = !isTesting;
        if (isTesting)
        {
            if (editorUIPanel != null) editorUIPanel.SetActive(false);
            if (hoverCursor != null) hoverCursor.gameObject.SetActive(false);

            if (editorCamera != null) editorCamera.SetActive(false);
            if (gameplayCamera != null) gameplayCamera.SetActive(true);

            testPlayer.gameObject.SetActive(true);
            testPlayer.OnInit(levelGenerator.GetStartPosition());
            Invoke(nameof(StartTestPlayer), 0.1f);
        }
        else
        {
            if (editorUIPanel != null) editorUIPanel.SetActive(true);
            if (editorCamera != null) editorCamera.SetActive(true);
            if (gameplayCamera != null) gameplayCamera.SetActive(false);
            testPlayer.gameObject.SetActive(false);

            levelGenerator.BuildLevelFromData(editorData);
        }
    }

    private void StartTestPlayer() { testPlayer.OnPlay(); }

    public void SaveMap()
    {
        string fileName;
        if (currentEditingLevel == 0) 
        {
            fileName = DataManager.Instance.GetNextLevelName(); 
            currentEditingLevel = int.Parse(fileName.Split('_')[1]);
            editorData.levelID = currentEditingLevel;
        }
        else 
        {
            fileName = "Level_" + currentEditingLevel;
        }

        DataManager.Instance.SaveLevel(editorData, fileName);
        Debug.Log("ĐÃ LƯU MAP: " + fileName);
    }

    public void LoadNextMap() { LoadSpecificMap(currentEditingLevel + 1); }
    public void LoadPrevMap() { if (currentEditingLevel > 1) LoadSpecificMap(currentEditingLevel - 1); }

    private void LoadSpecificMap(int levelToLoad)
    {
        string fileName = "Level_" + levelToLoad;
        LevelData loadedData = DataManager.Instance.LoadLevel(fileName);
        if (loadedData != null)
        {
            editorData = loadedData;
            currentEditingLevel = levelToLoad;
            mapSize.y = editorData.grid.Count;
            if (mapSize.y > 0) mapSize.x = editorData.grid[0].columns.Count;
            levelGenerator.BuildLevelFromData(editorData);
            Debug.Log("Đang xem bản đồ: " + fileName);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy " + fileName);
        }
    }

    private void OnDrawGizmos()
    {
        if (levelGenerator == null || isTesting) return;

        Gizmos.color = new Color(0.8f, 0.8f, 0.8f, 0.3f); 
        float size = levelGenerator.tileSize;

        for (int z = 0; z < mapSize.y; z++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                Vector3 center = new Vector3(x * size, 0, z * size);
                Gizmos.DrawWireCube(center, new Vector3(size, 0, size));
            }
        }
    }
}