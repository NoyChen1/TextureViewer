using UnityEditor;
using UnityEngine;
using System.IO;

[CustomEditor(typeof(TextureManager))]
public class TextureManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TextureManager manager = (TextureManager)target;

        if (GUILayout.Button("Open Texture Viewer"))
        {
            if (manager.currentTexture != null)
            {
                // if a texture is loaded, open the viewer window
                TextureViewerWindow.ShowWindow(manager.currentTexture);
            }
            else
            {
                // Show a dialog if no texture is loaded
                EditorUtility.DisplayDialog(
                    "Texture Viewer",
                    "No texture is loaded in the manager.",
                    "OK"
                );
            }
        }

        if (GUILayout.Button("Upload Image from Device"))
        {
            UploadImageToTexturesFolder();
        }
    }

    // upload an image from the user's device and save it to the Textures folder
    private void UploadImageToTexturesFolder()
    {
        string filePath = EditorUtility.OpenFilePanel("Select an Image", "", "png,jpg,jpeg");

        if (!string.IsNullOrEmpty(filePath))
        {
            Texture2D importedTexture = LoadTextureFromFile(filePath);

            if (importedTexture != null)
            {
                SaveTextureToFile(importedTexture);

                // Show a success dialog
                EditorUtility.DisplayDialog(
                    "Import Complete", 
                    "The image has been successfully imported and saved to the Textures folder!", 
                    "OK");
            }
            else
            {
                // Show an error dialog
                EditorUtility.DisplayDialog(
                    "Error", 
                    "Failed to load the image.", 
                    "OK");
            }
        }
        else
        {
            // Show the dialog if no photo has been selected
            EditorUtility.DisplayDialog(
                "No File Selected", 
                "No image was selected.", 
                "OK");
        }
    }

    // Load the texture from the selected file path
    private Texture2D LoadTextureFromFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);

        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(fileData)) 
        {
            return texture;
        }
        else
        {
            return null;
        }
    }

    // Save the imported texture as a in the "Assets/Textures" folder
    private void SaveTextureToFile(Texture2D texture)
    {
        string folderPath = Path.Combine(Application.dataPath, "Textures");

        // Check if the folder exists, if not, create it
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string fileName = "ImportedTexture_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filePath = Path.Combine(folderPath, fileName);

        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        // Refresh the Unity asset database to make the new file visible in the editor
        AssetDatabase.Refresh();

    }
}
