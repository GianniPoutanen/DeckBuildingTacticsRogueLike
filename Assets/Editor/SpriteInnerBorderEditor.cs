using UnityEditor;
using UnityEngine;

public class SpriteInnerBorderEditor : EditorWindow
{
    private float innerBorderSize = 2f;

    [MenuItem("Window/Sprite Inner Border Editor")]
    public static void ShowWindow()
    {
        GetWindow<SpriteInnerBorderEditor>("Sprite Inner Border Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Set Inner Border", EditorStyles.boldLabel);

        innerBorderSize = EditorGUILayout.FloatField("Inner Border Size", innerBorderSize);

        if (GUILayout.Button("Apply Inner Borders"))
        {
            ApplyInnerBorders();
        }
    }

    private void ApplyInnerBorders()
    {
        // Get the currently selected sprite
        Object[] selectedObjects = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);

        foreach (Object selectedObject in selectedObjects)
        {
            if (selectedObject is Texture2D texture)
            {
                Texture2D selectedTexture = (Texture2D)selectedObject;
                if (selectedTexture != null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(selectedTexture);
                    TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(assetPath);

                    if (textureImporter != null)
                    {
                        textureImporter.isReadable = true;
                        AssetDatabase.ImportAsset(assetPath);

                        Color[] pixels = selectedTexture.GetPixels();

                        int width = selectedTexture.width;
                        int height = selectedTexture.height;

                        for (int y = (int)innerBorderSize; y < height - innerBorderSize; y++)
                        {
                            for (int x = (int)innerBorderSize; x < width - innerBorderSize; x++)
                            {
                                int index = y * width + x;
                                pixels[index] = Color.black; // Change this line to set the color of the inner border
                            }
                        }

                        selectedTexture.SetPixels(pixels);
                        selectedTexture.Apply();

                        textureImporter.isReadable = false;
                        AssetDatabase.ImportAsset(assetPath);
                    }
                    else
                    {
                        Debug.LogError("Texture Importer not found.");
                    }
                }
                else
                {
                    Debug.LogError("Please select a Texture2D.");
                }
            }
        }
        // Force a reimport of the modified sprites
        // Force a reimport of the modified sprites
        AssetDatabase.Refresh();
    }
}
