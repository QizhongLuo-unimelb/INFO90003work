using UnityEngine;

public class TestExperienceController : MonoBehaviour
{
    [Header("Phase Timing")]
    public float flowDuration = 3f;
    public float influxDuration = 12f;
    public float suffocationDuration = 10f;

    [Header("Scene References")]
    public TestEmailSpawner emailSpawner;
    public TestLeafAtrophy leafAtrophy;
    public Light keyLight;
    public Camera sceneCamera;
    public Renderer[] treeRenderers;
    public Renderer[] pollutionRenderers;

    [Header("Tree Colors")]
    public Color healthyLeafColor = new Color(0.16f, 0.78f, 0.28f, 1f);
    public Color corruptedLeafColor = new Color(0.03f, 0.025f, 0.025f, 1f);
    public Color healthyTrunkColor = new Color(0.45f, 0.24f, 0.10f, 1f);
    public Color corruptedTrunkColor = new Color(0.08f, 0.055f, 0.04f, 1f);
    public Color pollutionColor = new Color(1f, 0.05f, 0.03f, 1f);

    [Header("Lighting")]
    public Color flowLightColor = new Color(0.72f, 0.95f, 0.78f, 1f);
    public Color overloadLightColor = new Color(1f, 0.48f, 0.42f, 1f);

    [Header("Minecraft Atmosphere")]
    public Color calmSkyColor = new Color(0.38f, 0.64f, 0.92f, 1f);
    public Color overloadSkyColor = new Color(0.12f, 0.18f, 0.24f, 1f);

    [Header("Mail Read Relief")]
    public float reliefSecondsPerRead = 0.65f;
    public float maxReadReliefSeconds = 8f;

    float elapsed;
    float readReliefSeconds;
    MaterialPropertyBlock propertyBlock;

    void Start()
    {
        propertyBlock = new MaterialPropertyBlock();
        CollectMissingRenderers();

        if (emailSpawner != null)
        {
            emailSpawner.ResetSpawnRate();
            emailSpawner.SetSpawning(false);
        }

        if (sceneCamera != null)
        {
            sceneCamera.clearFlags = CameraClearFlags.Skybox;
            sceneCamera.backgroundColor = calmSkyColor;
        }
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        float influxStart = flowDuration;
        float suffocationStart = flowDuration + influxDuration;
        bool shouldSpawn = elapsed >= influxStart && elapsed < suffocationStart + suffocationDuration;

        if (emailSpawner != null)
        {
            emailSpawner.SetSpawning(shouldSpawn);
        }

        float decayElapsed = Mathf.Max(0f, elapsed - readReliefSeconds);
        float decay = Mathf.InverseLerp(influxStart, suffocationStart + suffocationDuration, decayElapsed);
        if (leafAtrophy != null)
        {
            leafAtrophy.SetDecayProgress(decay);
        }

        ApplyAtmosphere(decay);
    }

    public void ApplyReadRelief(int readMessages)
    {
        if (readMessages <= 0)
        {
            return;
        }

        readReliefSeconds = Mathf.Min(maxReadReliefSeconds, readReliefSeconds + readMessages * reliefSecondsPerRead);
    }

    void ApplyAtmosphere(float decay)
    {
        if (keyLight != null)
        {
            keyLight.color = Color.Lerp(flowLightColor, overloadLightColor, decay);
            keyLight.intensity = Mathf.Lerp(1.15f, 0.42f, decay);
        }

        if (sceneCamera != null)
        {
            sceneCamera.clearFlags = CameraClearFlags.Skybox;
            sceneCamera.backgroundColor = Color.Lerp(calmSkyColor, overloadSkyColor, decay);
            sceneCamera.fieldOfView = Mathf.Lerp(46f, 36f, decay);
        }

        if (treeRenderers == null)
        {
            return;
        }

        foreach (Renderer treeRenderer in treeRenderers)
        {
            if (treeRenderer == null)
            {
                continue;
            }

            bool isLeaf = treeRenderer.name.ToLowerInvariant().Contains("leaf");
            Color targetColor = isLeaf
                ? Color.Lerp(healthyLeafColor, corruptedLeafColor, decay)
                : Color.Lerp(healthyTrunkColor, corruptedTrunkColor, decay);

            propertyBlock.SetColor("_BaseColor", targetColor);
            propertyBlock.SetColor("_Color", targetColor);
            propertyBlock.SetColor("_EmissionColor", Color.Lerp(Color.black, pollutionColor, Mathf.Clamp01((decay - 0.65f) / 0.35f)));
            treeRenderer.SetPropertyBlock(propertyBlock);
        }

        if (pollutionRenderers == null)
        {
            return;
        }

        float pollutionVisibility = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((decay - 0.2f) / 0.8f));
        foreach (Renderer pollutionRenderer in pollutionRenderers)
        {
            if (pollutionRenderer == null)
            {
                continue;
            }

            pollutionRenderer.enabled = pollutionVisibility > 0.02f;
            pollutionRenderer.transform.localScale = Vector3.one * Mathf.Lerp(0.05f, 1f, pollutionVisibility);
            propertyBlock.SetColor("_BaseColor", Color.Lerp(Color.black, pollutionColor, pollutionVisibility));
            propertyBlock.SetColor("_Color", Color.Lerp(Color.black, pollutionColor, pollutionVisibility));
            propertyBlock.SetColor("_EmissionColor", pollutionColor * Mathf.Lerp(0f, 1.8f, pollutionVisibility));
            pollutionRenderer.SetPropertyBlock(propertyBlock);
        }
    }

    void CollectMissingRenderers()
    {
        if ((treeRenderers == null || treeRenderers.Length == 0) && leafAtrophy != null)
        {
            treeRenderers = leafAtrophy.GetComponentsInChildren<Renderer>();
        }

        if ((pollutionRenderers == null || pollutionRenderers.Length == 0) && leafAtrophy != null)
        {
            Transform pollutionRoot = leafAtrophy.transform.Find("Pollution_Container");
            if (pollutionRoot != null)
            {
                pollutionRenderers = pollutionRoot.GetComponentsInChildren<Renderer>(true);
            }
        }
    }
}
