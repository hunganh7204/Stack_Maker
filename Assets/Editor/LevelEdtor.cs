using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelEditorManager))]
public class LevelEdtor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelEditorManager manager = (LevelEditorManager)target;

        GUILayout.Space(15);
        EditorGUILayout.LabelField("PLAYTEST", EditorStyles.boldLabel);

        GUI.backgroundColor = manager.IsTesting ? Color.red : Color.green;
        string testBtnText = manager.IsTesting ? "Stop" : "Play";
        if (GUILayout.Button(testBtnText, GUILayout.Height(40)))
        {
            manager.ToggleTestPlay();
        }
        GUI.backgroundColor = Color.white; 

        if (manager.IsTesting) return;

        GUILayout.Space(15);
        EditorGUILayout.LabelField("BRUSHES", EditorStyles.boldLabel);

        // Hàng 1
        GUILayout.BeginHorizontal();
        DrawBrushButton(manager, "Start Pos (-1)", -1);
        DrawBrushButton(manager, "Floor (1)", 1);
        DrawBrushButton(manager, "Wall (0)", 0);
        GUILayout.EndHorizontal();

        // Hàng 2
        GUILayout.BeginHorizontal();
        DrawBrushButton(manager, "Top Left (2)", 2);
        DrawBrushButton(manager, "Top Right (3)", 3);
        GUILayout.EndHorizontal();

        // Hàng 3
        GUILayout.BeginHorizontal();
        DrawBrushButton(manager, "Bot Left (4)", 4);
        DrawBrushButton(manager, "Bot Right (5)", 5);
        GUILayout.EndHorizontal();

        // Hàng 4
        GUILayout.BeginHorizontal();
        DrawBrushButton(manager, "Bridge (6)", 6);
        DrawBrushButton(manager, "Win Pos (-2)", -2);
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        EditorGUILayout.HelpBox("Use R to rotate bridge or winpos", MessageType.Info);

        GUILayout.Space(15);
        EditorGUILayout.LabelField($"MAP MANAGEMENT (Editing Level: {manager.CurrentEditingLevel})", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Prev Map", GUILayout.Height(30))) manager.LoadPrevMap();
        if (GUILayout.Button("Next Map", GUILayout.Height(30))) manager.LoadNextMap();
        GUILayout.EndHorizontal();

        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("Save", GUILayout.Height(35))) manager.SaveMap();

        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("New blank map", GUILayout.Height(25))) manager.InitializeBlankMap();
        GUI.backgroundColor = Color.white;

        Repaint();
    }
    private void DrawBrushButton(LevelEditorManager manager, string label, int brushId)
    {
        if (manager.CurrentBrush == brushId) GUI.backgroundColor = Color.green;
        else GUI.backgroundColor = Color.white;

        if (GUILayout.Button(label, GUILayout.Height(30)))
        {
            manager.SetBrushType(brushId);
        }
        GUI.backgroundColor = Color.white;
    }
}
