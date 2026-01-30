using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class StabieleZijliggingSceneGenerator
{
    private const string DocxFileName = "Game Design Document.docx";
    private const string BackgroundSpritePath = "Assets/assets/IMG/Voorgrond.png";
    private const string PlaceholderSpritePath = "Assets/assets/Basic figures/Basic Square.png";
    private const string StepSpriteFolder = "Assets/assets/IMG/StabieleZijligging";
    private const string PositieSpritePrefix = "Positie_";
    private const string ScenePath = "Assets/Scenes/LevelStabieleZijligging.unity";
    private const int DefaultStepCount = 5;
    private const float StepImageScale = 0.2f;
    private static readonly Vector3 PositieDisplayPosition = new Vector3(-0.91f, -1.92f, 0f);
    private const int PositieDisplaySortingOrder = 0;

    [MenuItem("Tools/Generate/Level Stabiele Zijligging")]
    public static void Generate()
    {
        var spec = LoadLevelSpec();
        int stepCount = Mathf.Max(1, spec.StepCount);

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var cameraGO = CreateCamera();
        CreateBackground();
        var boardGO = CreateStepBoard(stepCount, cameraGO != null ? cameraGO.GetComponent<Camera>() : null);

        if (boardGO != null)
        {
            var imageLayout = boardGO.GetComponentInChildren<StepRowLayout>();
            imageLayout?.Layout(false);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.Refresh();

        Debug.Log($"Generated scene: {ScenePath}");
    }

    private static GameObject CreateCamera()
    {
        var cameraGO = new GameObject("Main Camera");
        cameraGO.tag = "MainCamera";
        cameraGO.transform.position = new Vector3(0f, 0f, -10f);

        var camera = cameraGO.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.black;

        cameraGO.AddComponent<AudioListener>();
        AddUrpCameraData(cameraGO);

        return cameraGO;
    }

    private static GameObject CreateBackground()
    {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundSpritePath);
        if (sprite == null)
        {
            Debug.LogWarning($"Background sprite not found at {BackgroundSpritePath}");
            return null;
        }

        var backgroundGO = new GameObject("Background");
        var renderer = backgroundGO.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = -10;
        backgroundGO.transform.position = Vector3.zero;
        return backgroundGO;
    }

    private static GameObject CreateStepBoard(int stepCount, Camera targetCamera)
    {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(PlaceholderSpritePath);
        if (sprite == null)
        {
            Debug.LogWarning($"Placeholder sprite not found at {PlaceholderSpritePath}");
        }

        var board = new GameObject("StepBoard");

        var imagesRoot = new GameObject("StepImages");
        imagesRoot.transform.SetParent(board.transform, false);
        var imageLayout = imagesRoot.AddComponent<StepRowLayout>();
        imageLayout.SetTargetCamera(targetCamera);

        var positieSprites = LoadPositieSprites(stepCount);
        var displayRenderer = CreatePositieDisplay(board.transform, positieSprites, sprite);

        var zijliggingManager = board.AddComponent<ZijliggingGameManager>();
        zijliggingManager.SetTargetRenderer(displayRenderer);
        zijliggingManager.SetStepSprites(positieSprites);

        var placeholders = new StepPlaceholder[stepCount];
        for (int i = 0; i < stepCount; i++)
        {
            var image = new GameObject($"StepImage_{i + 1}");
            image.transform.SetParent(imagesRoot.transform, false);
            image.transform.localScale = new Vector3(StepImageScale, StepImageScale, 1f);

            var renderer = image.AddComponent<SpriteRenderer>();
            var stepSprite = LoadStepSprite(i + 1);
            renderer.sprite = stepSprite != null ? stepSprite : sprite;
            renderer.sortingOrder = 20;

            var collider = image.AddComponent<BoxCollider2D>();
            FitColliderToSprite(collider, renderer);

            var placeholder = image.AddComponent<StepPlaceholder>();
            placeholder.orderIndex = i + 1;
            placeholders[i] = placeholder;

            var stepButton = image.AddComponent<ZijliggingStepButton>();
            stepButton.SetStepNumber(i + 1);
            stepButton.SetManager(zijliggingManager);
        }

        imageLayout.SetItems(placeholders);

        var popup = new GameObject("LevelCompletionPopup");
        popup.transform.SetParent(board.transform, false);
        return board;
    }

    private static Sprite LoadStepSprite(int stepIndex)
    {
        string[] extensions = { ".png", ".jpg", ".jpeg" };
        foreach (string extension in extensions)
        {
            string path = $"{StepSpriteFolder}/Stap {stepIndex}{extension}";
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null)
            {
                return sprite;
            }
        }

        return null;
    }

    private static Sprite[] LoadPositieSprites(int stepCount)
    {
        var sprites = new Sprite[stepCount];
        for (int i = 0; i < stepCount; i++)
        {
            sprites[i] = LoadPositieSprite(i + 1);
            if (sprites[i] == null)
            {
                Debug.LogWarning($"Positie sprite not found for step {i + 1} in {StepSpriteFolder}.");
            }
        }

        return sprites;
    }

    private static Sprite LoadPositieSprite(int positionIndex)
    {
        string[] extensions = { ".png", ".jpg", ".jpeg" };
        foreach (string extension in extensions)
        {
            string path = $"{StepSpriteFolder}/{PositieSpritePrefix}{positionIndex}{extension}";
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null)
            {
                return sprite;
            }
        }

        return null;
    }

    private static SpriteRenderer CreatePositieDisplay(Transform parent, Sprite[] sprites, Sprite fallback)
    {
        var display = new GameObject("PositieDisplay");
        display.transform.SetParent(parent, false);
        display.transform.position = PositieDisplayPosition;

        var renderer = display.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = PositieDisplaySortingOrder;

        var sprite = GetFirstSpriteOrFallback(sprites, fallback);
        renderer.sprite = sprite;

        return renderer;
    }

    private static Sprite GetFirstSpriteOrFallback(Sprite[] sprites, Sprite fallback)
    {
        if (sprites != null)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i] != null)
                {
                    return sprites[i];
                }
            }
        }

        return fallback;
    }

    private static void FitColliderToSprite(BoxCollider2D collider, SpriteRenderer renderer)
    {
        if (collider == null || renderer == null || renderer.sprite == null)
        {
            return;
        }

        collider.size = renderer.sprite.bounds.size;
        collider.offset = renderer.sprite.bounds.center;
    }

    private static void AddUrpCameraData(GameObject cameraGO)
    {
        var type = Type.GetType("UnityEngine.Rendering.Universal.UniversalAdditionalCameraData, Unity.RenderPipelines.Universal.Runtime");
        if (type == null)
        {
            return;
        }

        if (cameraGO.GetComponent(type) == null)
        {
            cameraGO.AddComponent(type);
        }
    }

    private static DocxSection LoadLevelSpec()
    {
        string docxPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", DocxFileName));
        if (!File.Exists(docxPath))
        {
            Debug.LogWarning($"Docx not found at {docxPath}. Using defaults.");
            return DocxSection.Default(DefaultStepCount);
        }

        List<string> paragraphs = ReadDocxParagraphs(docxPath);
        if (paragraphs.Count == 0)
        {
            Debug.LogWarning("Docx had no readable paragraphs. Using defaults.");
            return DocxSection.Default(DefaultStepCount);
        }

        int index = FindSectionIndex(paragraphs);
        if (index < 0)
        {
            Debug.LogWarning("Section for Stabiele Zijligging not found. Using defaults.");
            return DocxSection.Default(DefaultStepCount);
        }

        string title = paragraphs[index].Trim();
        var description = new List<string>();
        for (int i = index + 1; i < paragraphs.Count; i++)
        {
            string line = paragraphs[i].Trim();
            if (IsSectionHeading(line))
            {
                break;
            }

            description.Add(line);
        }

        int stepCount = ExtractStepCount(description);
        if (stepCount <= 0)
        {
            stepCount = DefaultStepCount;
        }

        return new DocxSection(title, description.ToArray(), stepCount);
    }

    private static List<string> ReadDocxParagraphs(string path)
    {
        using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var archive = new ZipArchive(fileStream, ZipArchiveMode.Read);
        var entry = archive.GetEntry("word/document.xml");
        if (entry == null)
        {
            return new List<string>();
        }

        using var stream = entry.Open();
        var doc = XDocument.Load(stream);
        XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        return doc.Descendants(w + "p")
            .Select(p => string.Concat(p.Descendants(w + "t").Select(t => (string)t)))
            .Select(text => text?.Trim())
            .Where(text => !string.IsNullOrEmpty(text))
            .ToList();
    }

    private static int FindSectionIndex(List<string> paragraphs)
    {
        int index = paragraphs.FindIndex(line =>
            line.IndexOf("Stabiele", StringComparison.OrdinalIgnoreCase) >= 0 &&
            line.IndexOf("Zijligging", StringComparison.OrdinalIgnoreCase) >= 0);

        if (index >= 0)
        {
            return index;
        }

        return paragraphs.FindIndex(line =>
            line.IndexOf("Action Sequence", StringComparison.OrdinalIgnoreCase) >= 0);
    }

    private static bool IsSectionHeading(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return false;
        }

        return Regex.IsMatch(line, @"^\\d+(\\.\\d+)+");
    }

    private static int ExtractStepCount(IEnumerable<string> lines)
    {
        foreach (string line in lines)
        {
            var match = Regex.Match(line, @"\\b(\\d+)\\b");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int value))
            {
                return value;
            }
        }

        return 0;
    }

    private readonly struct DocxSection
    {
        public string Title { get; }
        public string[] DescriptionLines { get; }
        public int StepCount { get; }

        public DocxSection(string title, string[] descriptionLines, int stepCount)
        {
            Title = string.IsNullOrWhiteSpace(title) ? "Stabiele Zijligging" : title;
            DescriptionLines = descriptionLines ?? Array.Empty<string>();
            StepCount = stepCount;
        }

        public static DocxSection Default(int stepCount)
        {
            return new DocxSection("Stabiele Zijligging", Array.Empty<string>(), stepCount);
        }
    }
}
