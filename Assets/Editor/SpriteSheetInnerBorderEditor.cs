using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class SpriteSheetInnerBorderEditor : EditorWindow
{
    private int borderSizeInt = 0;
    private Vector4 borderSize = Vector4.zero;
    private int innerBorderSizeInt = 0;
    private Vector4 innerBorderSize = Vector4.zero;
    private Texture2D selectedTexture;

    [MenuItem("Window/Sprite Sheet Inner Border Editor")]
    public static void ShowWindow()
    {
        GetWindow<SpriteSheetInnerBorderEditor>("Sprite Sheet Inner Border Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Set Inner Border for Sprite Sheet", EditorStyles.boldLabel);

        selectedTexture = (Texture2D)EditorGUILayout.ObjectField("Sprite Sheet Texture", selectedTexture, typeof(Texture2D), false);
        borderSizeInt = EditorGUILayout.IntField("Border Size Setter", borderSizeInt);
        if (borderSizeInt > 0)
            borderSize = new Vector4(borderSizeInt, borderSizeInt, borderSizeInt, borderSizeInt);
        else
            borderSize = EditorGUILayout.Vector4Field("Border Size", borderSize);

        innerBorderSizeInt = EditorGUILayout.IntField("Inner Border Size Setter", innerBorderSizeInt);
        if (innerBorderSizeInt > 0)
            innerBorderSize = new Vector4(innerBorderSizeInt, innerBorderSizeInt, innerBorderSizeInt, innerBorderSizeInt);
        else
            innerBorderSize = EditorGUILayout.Vector4Field("Inner Border Size", innerBorderSize);

        if (GUILayout.Button("Apply Inner Borders"))
        {
            ApplyInnerBorders();
        }
    }

    private void ApplyInnerBorders()
    {
        if (selectedTexture != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(selectedTexture);
            TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(assetPath);

            if (textureImporter != null)
            {
                textureImporter.isReadable = true;
                AssetDatabase.ImportAsset(assetPath);

                SpriteMetaData[] spritesheet = textureImporter.spritesheet;

                for (int i = 0; i < spritesheet.Length; i++)
                {
                    Rect rect = spritesheet[i].rect;
                    rect.x = rect.x - borderSize.x;
                    rect.y = rect.y - borderSize.y;
                    rect.width = rect.width - borderSize.z;
                    rect.height = rect.height - borderSize.w;

                    rect.position = new Vector2(rect.position.x + borderSize.x, rect.position.y + borderSize.y);

                    spritesheet[i].rect = rect;
                    spritesheet[i].border = innerBorderSize;
                }

                textureImporter.spritesheet = spritesheet;
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
            Debug.LogError("Please select a Sprite Sheet Texture2D.");
        }
    }
}
