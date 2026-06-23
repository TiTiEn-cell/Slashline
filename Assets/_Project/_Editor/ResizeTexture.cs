using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public class ResizeTexture : EditorWindow
{
    [SerializeField] List<Texture2D> ListTexture = new List<Texture2D>();
    [SerializeField] List<Texture2D> ListPasteTo = new List<Texture2D>();

    Texture2D texture2;
    Sprite[] sprites;
    Editor editor;

    int custom = 9; //Change pivot mode, Center = 0, TopLeft = 1, TopCenter = 2, TopRight = 3, LeftCenter = 4, RightCenter = 5, BottomLeft = 6, BottomCenter = 7, BottomRight = 8, Custom = 9.

    [MenuItem("Tools/Resize Texture")]

    public static void ShowWindown()
    {
        GetWindow(typeof(ResizeTexture));
    }

    private void OnGUI()
    {
        GUILayout.Label("Resize Texture", EditorStyles.boldLabel);
        if (!editor) { editor = Editor.CreateEditor(this); }
        if (editor) { editor.OnInspectorGUI(); }

        if (GUILayout.Button("Create texture"))
        {
            CreateTexture();
        }

        if (GUILayout.Button("Copy Paste"))
        {
            CopyPaste();
        }

        if (GUILayout.Button("Clear"))
        {
            Clear();
        }
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void CreateTexture()
    {
        foreach (Texture2D texture in ListTexture)
        {
            var width = texture.width;
            var height = texture.height;

            while (width % 4 != 0)
            {
                width++;
            }
            while (height % 4 != 0)
            {
                height++;
            }
            texture2 = new Texture2D(width, height, TextureFormat.ARGB32, false);
            DrawNewOne(texture, texture2);
        }

        AssetDatabase.Refresh();
    }

    private void DrawNewOne(Texture2D texture1, Texture2D texture2)
    {
        Color32[] color1 = texture1.GetPixels32();
        Color32[] color2 = texture2.GetPixels32();
        Color32 colorX = new Color32(0, 0, 0, 0); // a = 0
        Debug.LogError("color need :: " + color1[0]);
        int rowIndext = 0;
        int rowCount = 1;

        int diff = texture2.width - texture1.width;


        for (int i = 0; i < color2.Length; i++)
        {
            if (i - diff * rowIndext < texture1.width * rowCount)
            {
                if (i - diff * rowIndext < color1.Length)
                {
                    color2[i] = color1[i - diff * rowIndext];
                }
                else
                {
                    color2[i] = colorX;
                }
            }
            else
            {
                if (i < texture2.width * rowCount)
                {
                    color2[i] = colorX;
                }
            }

            if ((i + 1) % texture2.width == 0 && i > 0)
            {
                rowIndext++;
                rowCount++;
            }
        }

        texture2.SetPixels32(color2);

        Save(texture1);
    }

    public void Clear()
    {
        ListTexture.Clear();
        ListPasteTo.Clear();
    }

    private void Save(Texture2D texture1)
    {
        var bytes = texture2.EncodeToPNG();
        var folder = "ResizeTexture";

        var dirPath = Path.Combine(Application.dataPath, folder);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        var savePath = Application.dataPath + $"/{folder}/";

        File.WriteAllBytes(savePath + texture1.name + ".png", bytes);

    }

    private void CopyPaste()
    {
        for (int u = 0; u < ListTexture.Count; u++)
        {
            for (int j = 0; j < ListPasteTo.Count; j++)
            {
                if (ListTexture[u].name == ListPasteTo[j].name)
                {
                    UnityEngine.Object[] objects = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(ListTexture[u]));

                    sprites = new Sprite[objects.Length];

                    for (int i = 0; i < objects.Length; i++)
                    {
                        sprites[i] = objects[i] as Sprite;
                    }

                    string path = AssetDatabase.GetAssetPath(ListPasteTo[j]);

                    TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Multiple;

                    List<SpriteMetaData> data = new List<SpriteMetaData>();

                    for (int i = 0; i < objects.Length; i++)
                    {
                        SpriteMetaData _meta = new SpriteMetaData();

                        var w = sprites[i].rect.width; // width in pixesl
                        var h = sprites[i].rect.height; // heigth in pexesl
                        var xPivot = sprites[i].pivot.x; // in pixels
                        var yPivot = sprites[i].pivot.y; // in pixels
                        var x = xPivot / w;
                        var y = yPivot / h;

                        _meta.alignment = custom;
                        _meta.pivot.x = x;
                        _meta.pivot.y = y;
                        _meta.name = sprites[i].name;
                        _meta.rect = sprites[i].rect;
                        _meta.border = sprites[i].border;
                        data.Add(_meta);
                    }
                    importer.spritesheet = data.ToArray();
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    Debug.LogError($"Done :: {j + 1}/{ListPasteTo.Count}_{ListPasteTo[j].name} ");
                }
            }
        }
    }
}
#endif
