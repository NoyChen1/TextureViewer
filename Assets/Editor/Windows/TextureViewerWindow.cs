using System;
using UnityEditor;
using UnityEngine;

public class TextureViewerWindow : EditorWindow
{
    private Texture2D texture;
    private Vector2 scrollPosition;
    private float zoom = 1.0f;
    private Rect textureRect;

    //limitz for the window size
    private const float MinWindowSize = 200f; // Minimum window size
    private const float MaxWindowSize = 1024f; // Maximum window size for very large textures
    
    
    public static void ShowWindow(Texture2D texture)
    {
        TextureViewerWindow window = GetWindow<TextureViewerWindow>("Texture Viewer");
        window.texture = texture;

        window.AdjustWindowSize(texture);

        window.minSize = new Vector2(MinWindowSize, MinWindowSize);

        window.CenterOnScreen();

    }

    private void CenterOnScreen()
    {
        // Calculate the center position based on the window size
        Resolution screenResolution = Screen.currentResolution;
        float windowWidth = this.position.width;
        float windowHeight = this.position.height;

        // Calculate the center position for the window
        float centerX = (screenResolution.width - windowWidth) / 2;
        float centerY = (screenResolution.height - windowHeight) / 2;

        // Set the new window position
        this.position = new Rect(centerX, centerY, windowWidth, windowHeight);
    }

    private void AdjustWindowSize(Texture2D texture)
    {
        if (texture != null)
        {
            // Calculate the window size based on the texture size
            float width = Mathf.Clamp(texture.width, MinWindowSize, MaxWindowSize);
            float height = Mathf.Clamp(texture.height, MinWindowSize, MaxWindowSize);

            // Set the window's position and size to fit the texture
            this.position = new Rect(this.position.x, this.position.y, width, height);
        }
    }

    private void OnGUI()
    {
        if (texture == null)
        {
            EditorGUILayout.LabelField("No texture available.");
            return;
        }

        HandleZoomAndPan();

        GUILayout.BeginArea(textureRect);
        scrollPosition = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), scrollPosition, new Rect(0, 0, texture.width * zoom, texture.height * zoom));
        GUI.DrawTexture(new Rect(0, 0, texture.width * zoom, texture.height * zoom), texture);
        GUI.EndScrollView();
        GUILayout.EndArea();
    }

    private void HandleZoomAndPan()
    {
        Event e = Event.current;

        // Zoom using mouse scroll
        if (e.type == EventType.ScrollWheel)
        {
            float zoomDelta = -e.delta.y * 0.05f;
            zoom = Mathf.Clamp(zoom + zoomDelta, 0.1f, 10f); // Limit zoom levels
            Repaint();
        }

        // Pan using left mouse button
        if (e.type == EventType.MouseDrag && e.button == 0)
        {
            scrollPosition -= e.delta;
            Repaint();
        }

        textureRect = new Rect(0, 0, texture.width * zoom, texture.height * zoom);
    }
}
