using UnityEngine;
using UnityEditor;
using System.IO;

public class HanGangBackgroundGenerator
{
    private const int WIDTH = 1080;
    private const int HEIGHT = 1920;
    private const string OUTPUT_PATH = "Assets/Sprites/Background/hangang_bg.png";

    [MenuItem("Tools/Generate Han River Background")]
    public static void GenerateBackground()
    {
        // Create output directory if it doesn't exist
        string directory = Path.GetDirectoryName(OUTPUT_PATH);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Create texture
        Texture2D texture = new Texture2D(WIDTH, HEIGHT, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;

        // Fill pixels
        Color[] pixels = new Color[WIDTH * HEIGHT];

        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                int pixelIndex = y * WIDTH + x;
                float normalizedY = (float)y / HEIGHT;

                // Sky (top 25%)
                if (normalizedY < 0.25f)
                {
                    pixels[pixelIndex] = GetSkyColor(normalizedY, x);
                }
                // Mountains and horizon (25% - 30%)
                else if (normalizedY < 0.30f)
                {
                    pixels[pixelIndex] = GetHorizonColor(normalizedY, x);
                }
                // River (30% - 75%)
                else if (normalizedY < 0.75f)
                {
                    pixels[pixelIndex] = GetWaterColor(normalizedY, x, y);
                }
                // Foreground (75% - 100%)
                else
                {
                    pixels[pixelIndex] = GetForegroundColor(normalizedY, x);
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        // Save as PNG
        byte[] png = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(OUTPUT_PATH, png);

        // Clean up
        Object.DestroyImmediate(texture);

        // Import the texture
        AssetDatabase.ImportAsset(OUTPUT_PATH);

        // Configure import settings
        TextureImporter importer = AssetImporter.GetAtPath(OUTPUT_PATH) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            // Set pixels per unit
            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spritePixelsPerUnit = 100;
            importer.SetTextureSettings(settings);

            importer.SaveAndReimport();
        }

        AssetDatabase.Refresh();
        Debug.Log($"Han River background generated at {OUTPUT_PATH}");
    }

    private static Color GetSkyColor(float normalizedY, int x)
    {
        // Sky gradient: deep blue at top -> pale cyan at horizon
        float gradientT = normalizedY / 0.25f;
        Color topColor = new Color(0.4f, 0.6f, 0.95f);      // Deep sky blue
        Color horizonColor = new Color(0.7f, 0.85f, 1f);    // Pale cyan

        Color baseColor = Color.Lerp(topColor, horizonColor, gradientT);

        // Add clouds
        float cloudNoise = Mathf.PerlinNoise(x * 0.002f, normalizedY * 2f);
        if (cloudNoise > 0.55f)
        {
            float cloudIntensity = Mathf.Clamp01((cloudNoise - 0.55f) / 0.25f);
            baseColor = Color.Lerp(baseColor, Color.white, cloudIntensity * 0.6f);
        }

        return baseColor;
    }

    private static Color GetHorizonColor(float normalizedY, int x)
    {
        // Mountains
        float mountainNoise = Mathf.PerlinNoise(x * 0.001f, 2f);
        Color mountainColor = new Color(0.45f, 0.65f, 0.75f);

        // Buildings/apartments (pale pink/white)
        float buildingNoise = Mathf.PerlinNoise(x * 0.003f, 3f);
        if (buildingNoise > 0.6f)
        {
            mountainColor = Color.Lerp(mountainColor, new Color(0.9f, 0.85f, 0.8f), 0.5f);
        }

        // Bridge piers
        float bridgeT = (float)(x - WIDTH * 0.3f) / (WIDTH * 0.4f);
        if (bridgeT >= 0 && bridgeT <= 1)
        {
            float pierPattern = Mathf.Abs(Mathf.Sin(bridgeT * 15f));
            if (pierPattern > 0.7f)
            {
                mountainColor = Color.Lerp(mountainColor, new Color(0.6f, 0.6f, 0.6f), 0.3f);
            }
        }

        return mountainColor;
    }

    private static Color GetWaterColor(float normalizedY, int x, int y)
    {
        // Water gradient: teal to lighter blue
        float waterGradient = (normalizedY - 0.30f) / 0.45f;
        Color darkWater = new Color(0.2f, 0.6f, 0.7f);
        Color lightWater = new Color(0.4f, 0.75f, 0.85f);
        Color baseWater = Color.Lerp(darkWater, lightWater, waterGradient);

        // Water ripples (horizontal streaks)
        float ripplePattern = Mathf.Sin(y * 0.01f + x * 0.0005f) * 0.5f + 0.5f;
        baseWater = Color.Lerp(baseWater, baseWater * 0.8f, ripplePattern * 0.2f);

        // Sparkles (bright highlights)
        float sparkleNoise = Mathf.PerlinNoise(x * 0.005f, y * 0.003f);
        if (sparkleNoise > 0.8f)
        {
            baseWater = Color.Lerp(baseWater, Color.white, (sparkleNoise - 0.8f) * 2f);
        }

        // Bridge reflection
        float bridgeT = (float)(x - WIDTH * 0.3f) / (WIDTH * 0.4f);
        if (bridgeT >= 0 && bridgeT <= 1)
        {
            float reflection = Mathf.Abs(Mathf.Sin(bridgeT * 15f)) * 0.3f;
            baseWater = Color.Lerp(baseWater, new Color(0.7f, 0.7f, 0.7f), reflection * 0.15f);
        }

        return baseWater;
    }

    private static Color GetForegroundColor(float normalizedY, int x)
    {
        float normalizedX = (float)x / WIDTH;

        // Gradient to darker at bottom
        float darkenT = (normalizedY - 0.75f) / 0.25f;
        Color waterColor = new Color(0.2f, 0.55f, 0.65f);
        Color darkColor = new Color(0.15f, 0.4f, 0.5f);
        Color baseColor = Color.Lerp(waterColor, darkColor, darkenT);

        // Left and right edges: rocks
        if (normalizedX < 0.15f || normalizedX > 0.85f)
        {
            float rockNoise = Mathf.PerlinNoise(x * 0.01f, normalizedY * 5f);
            if (rockNoise > 0.45f)
            {
                Color rockColor = new Color(0.5f, 0.45f, 0.4f);
                baseColor = Color.Lerp(baseColor, rockColor, Mathf.Clamp01((rockNoise - 0.45f) / 0.3f));
            }
        }

        // Grass tufts at edges
        if ((normalizedX < 0.2f || normalizedX > 0.8f) && normalizedY > 0.85f)
        {
            float grassNoise = Mathf.PerlinNoise(x * 0.015f, normalizedY * 10f);
            if (grassNoise > 0.5f)
            {
                Color grassColor = new Color(0.3f, 0.5f, 0.2f);
                baseColor = Color.Lerp(baseColor, grassColor, Mathf.Clamp01((grassNoise - 0.5f) / 0.25f));
            }
        }

        return baseColor;
    }
}
