#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using System.IO;

public static class DinoRunnerAutoSetup
{
    [MenuItem("Tools/Dino Runner/Run Complete Setup (Ground, Background, UI, App Icon)")]
    public static void RunFullSetup()
    {
        Debug.Log("<color=cyan><b>[DinoRunner Setup]</b> Starting automated setup...</color>");

        SetupSortingLayers();
        SetupPlayerSettingsAndIcon();
        SetupBackground();
        SetupGround();
        SetupUI();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("<color=green><b>[DinoRunner Setup]</b> Full setup completed and scene saved successfully!</color>");
        EditorUtility.DisplayDialog("Dino Runner Setup Complete", 
            "The Ground, Background, UI Buttons, Settings Icon, and App Icon have been successfully set up and saved!", "Awesome!");
    }

    private static void SetupSortingLayers()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty sortingLayers = tagManager.FindProperty("m_SortingLayers");

        string[] requiredLayers = new string[] { "Background", "Ground", "Obstacles", "Player", "UI" };

        foreach (string layerName in requiredLayers)
        {
            bool exists = false;
            for (int i = 0; i < sortingLayers.arraySize; i++)
            {
                SerializedProperty element = sortingLayers.GetArrayElementAtIndex(i);
                if (element.FindPropertyRelative("name").stringValue == layerName)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                sortingLayers.InsertArrayElementAtIndex(sortingLayers.arraySize);
                SerializedProperty newLayer = sortingLayers.GetArrayElementAtIndex(sortingLayers.arraySize - 1);
                newLayer.FindPropertyRelative("name").stringValue = layerName;
                newLayer.FindPropertyRelative("uniqueID").intValue = layerName.GetHashCode();
            }
        }

        tagManager.ApplyModifiedProperties();
        Debug.Log("Sorting Layers verified (Background, Ground, Obstacles, Player, UI).");
    }

    private static void SetupPlayerSettingsAndIcon()
    {
        PlayerSettings.productName = "Dino Runner";
        if (PlayerSettings.companyName == "DefaultCompany")
            PlayerSettings.companyName = "Dino Studio";

        Texture2D appIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/AppIcon.png");
        if (appIcon != null)
        {
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] { appIcon });
            Debug.Log("App Icon assigned to PlayerSettings.");
        }
    }

    private static void SetupBackground()
    {
        Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/CartoonBackground.png");
        if (bgSprite == null)
        {
            Debug.LogWarning("CartoonBackground.png sprite not found. Make sure it is imported.");
            return;
        }

        GameObject bgRoot = GameObject.Find("BackgroundEnvironment");
        if (bgRoot == null)
        {
            bgRoot = new GameObject("BackgroundEnvironment");
            bgRoot.transform.position = Vector3.zero;
        }

        // Camera height is 10 units for Ortho size 5 (Aspect 16:9 -> width 17.78)
        float targetWidth = 17.78f;
        float targetHeight = 10f;
        float scaleX = targetWidth / (bgSprite.rect.width / bgSprite.pixelsPerUnit);
        float scaleY = targetHeight / (bgSprite.rect.height / bgSprite.pixelsPerUnit);
        Vector3 bgScale = new Vector3(scaleX, scaleY, 1f);

        // Tile 1
        Transform bg1 = bgRoot.transform.Find("Background_1");
        if (bg1 == null)
        {
            GameObject go = new GameObject("Background_1");
            go.transform.SetParent(bgRoot.transform);
            bg1 = go.transform;
        }
        bg1.position = new Vector3(0, 0, 0);
        bg1.localScale = bgScale;

        SpriteRenderer sr1 = bg1.GetComponent<SpriteRenderer>();
        if (sr1 == null) sr1 = bg1.gameObject.AddComponent<SpriteRenderer>();
        sr1.sprite = bgSprite;
        sr1.sortingLayerName = "Background";
        sr1.sortingOrder = 0;

        BackgroundScroller bs1 = bg1.GetComponent<BackgroundScroller>();
        if (bs1 == null) bs1 = bg1.gameObject.AddComponent<BackgroundScroller>();
        bs1.speed = 1.5f;
        bs1.width = targetWidth;

        // Tile 2
        Transform bg2 = bgRoot.transform.Find("Background_2");
        if (bg2 == null)
        {
            GameObject go = new GameObject("Background_2");
            go.transform.SetParent(bgRoot.transform);
            bg2 = go.transform;
        }
        bg2.position = new Vector3(targetWidth, 0, 0);
        bg2.localScale = bgScale;

        SpriteRenderer sr2 = bg2.GetComponent<SpriteRenderer>();
        if (sr2 == null) sr2 = bg2.gameObject.AddComponent<SpriteRenderer>();
        sr2.sprite = bgSprite;
        sr2.sortingLayerName = "Background";
        sr2.sortingOrder = 0;

        BackgroundScroller bs2 = bg2.GetComponent<BackgroundScroller>();
        if (bs2 == null) bs2 = bg2.gameObject.AddComponent<BackgroundScroller>();
        bs2.speed = 1.5f;
        bs2.width = targetWidth;

        Debug.Log("Parallax background configured with CartoonBackground.png.");
    }

    private static void SetupGround()
    {
        Sprite groundSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/CartoonGround.png");

        if (groundSprite == null)
        {
            Object[] sheetAssets = AssetDatabase.LoadAllAssetsAtPath("Assets/Art/Sprite_Sheet.png");
            foreach (Object obj in sheetAssets)
            {
                if (obj is Sprite s && (s.name == "Sprite_Sheet_19" || s.name == "Sprite_Sheet_18"))
                {
                    groundSprite = s;
                    break;
                }
            }
        }

        GameObject ground1 = GameObject.Find("Ground");
        GameObject ground2 = GameObject.Find("Ground2");

        if (ground1 == null) ground1 = new GameObject("Ground");
        if (ground2 == null) ground2 = new GameObject("Ground2");

        ConfigureGroundTile(ground1, new Vector3(0, -3.1f, 0), groundSprite);
        ConfigureGroundTile(ground2, new Vector3(25f, -3.1f, 0), groundSprite);

        Debug.Log("Ground & Ground2 configured with CartoonGround sprite, colliders, tag, and GroundScroller.");
    }

    private static void ConfigureGroundTile(GameObject go, Vector3 pos, Sprite sprite)
    {
        go.tag = "Ground";
        go.transform.position = pos;

        // Cleanup accidental UI components if present
        CanvasRenderer cr = go.GetComponent<CanvasRenderer>();
        if (cr != null) Object.DestroyImmediate(cr);

        Image img = go.GetComponent<Image>();
        if (img != null) Object.DestroyImmediate(img);

        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr == null) sr = go.AddComponent<SpriteRenderer>();
        if (sprite != null) sr.sprite = sprite;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(25f, 2f);
        sr.sortingLayerName = "Ground";
        sr.sortingOrder = 0;

        BoxCollider2D box = go.GetComponent<BoxCollider2D>();
        if (box == null) box = go.AddComponent<BoxCollider2D>();
        box.isTrigger = false;
        box.size = new Vector2(25f, 1.8f);
        box.offset = new Vector2(0f, -0.1f);

        GroundScroller scroller = go.GetComponent<GroundScroller>();
        if (scroller == null) scroller = go.AddComponent<GroundScroller>();
        scroller.speed = 7f;
        scroller.tileWidth = 25f;
    }

    private static void SetupUI()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null) scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // Find sliced UI sprites from Sprite_Sheet
        Sprite buttonSprite = null;
        Sprite settingsIconSprite = null;

        Object[] sheetAssets = AssetDatabase.LoadAllAssetsAtPath("Assets/Art/Sprite_Sheet.png");
        foreach (Object obj in sheetAssets)
        {
            if (obj is Sprite s)
            {
                if (s.name == "Sprite_Sheet_13" || s.name == "Sprite_Sheet_14")
                    buttonSprite = s;
                else if (s.name == "Sprite_Sheet_6" || s.name == "Sprite_Sheet_7")
                    settingsIconSprite = s;
            }
        }

        // Style RestartButton inside GameOverPanel
        GameObject restartBtnObj = GameObject.Find("RestartButton");
        if (restartBtnObj != null && buttonSprite != null)
        {
            Image btnImg = restartBtnObj.GetComponent<Image>();
            if (btnImg != null)
            {
                btnImg.sprite = buttonSprite;
                btnImg.color = Color.white;
                btnImg.type = Image.Type.Simple;
            }
        }

        // Add or configure SettingsButton in top-right
        Transform existingSettings = canvas.transform.Find("SettingsButton");
        GameObject settingsBtnObj = existingSettings != null ? existingSettings.gameObject : null;

        if (settingsBtnObj == null)
        {
            settingsBtnObj = new GameObject("SettingsButton");
            settingsBtnObj.transform.SetParent(canvas.transform, false);
            RectTransform rt = settingsBtnObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(1, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(-30, -30);
            rt.sizeDelta = new Vector2(64, 64);

            Image sImg = settingsBtnObj.AddComponent<Image>();
            if (settingsIconSprite != null) sImg.sprite = settingsIconSprite;
            sImg.preserveAspect = true;

            Button btn = settingsBtnObj.AddComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            cb.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
            btn.colors = cb;
        }

        Debug.Log("UI configured (CanvasScaler, Restart Button Sprite, and Settings Icon Button).");
    }
}
#endif
