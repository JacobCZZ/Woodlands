using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DayNightCycle : MonoBehaviour
{
    [Header("Light Settings")]
    public Light sun;

    [Header("Durations (in seconds)")]
    public float dayDuration = 60f;
    public float nightDuration = 60f;
    public float transitionDuration = 3f;

    [Header("Ambient Light Settings")]
    public Color dayAmbientColor = new Color(0.6f, 0.6f, 0.6f);
    public Color nightAmbientColor = new Color(0.1f, 0.1f, 0.2f);

    [Header("Sun Settings")]
    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.1f, 0.1f, 0.35f);
    public float maxSunIntensity = 1f;
    public float minSunIntensity = 0.2f;

    [Header("Skybox Materials")]
    public Material daySkybox;
    public Material nightSkybox;

    [Header("UI Settings")]
    public RectTransform icon;
    public Image iconImage;
    public Image sunSprite, moonSprite;
    public RectTransform barStart, barEnd;

    [Header("Enemy Spawn Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int baseMaxEnemies = 5;
    public float enemySpawnInterval = 5f;

    private float cycleTime = 0f;
    public bool isDay = true;
    private float cycleDuration;

    private int dayCount = 0;
    private List<GameObject> currentEnemies = new List<GameObject>();
    private Coroutine enemySpawnCoroutine;

    public Text DeathTextDays;
    public Text DaysCountText;

    void Start()
    {
        if (isDay)
        {
            cycleDuration = dayDuration;
            RenderSettings.ambientLight = dayAmbientColor;
            RenderSettings.skybox = daySkybox;
            sunSprite.gameObject.SetActive(true);
            moonSprite.gameObject.SetActive(false);
        }
        else
        {
            cycleDuration = nightDuration;
            RenderSettings.ambientLight = nightAmbientColor;
            RenderSettings.skybox = nightSkybox;
            sunSprite.gameObject.SetActive(false);
            moonSprite.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        cycleTime += Time.deltaTime;

        float t = cycleTime / cycleDuration;
        icon.anchoredPosition = Vector2.Lerp(barStart.anchoredPosition, barEnd.anchoredPosition, t);

        if (cycleTime >= cycleDuration)
        {
            cycleTime = 0f;
            StartCoroutine(ChangeTimeOfDay());
        }
    }

    private IEnumerator ChangeTimeOfDay()
    {
        float timer = 0f;
        Color startAmbientColor = RenderSettings.ambientLight;
        Color targetAmbientColor;
        Color targetSunColor;
        float targetIntensity;
        Material targetSkybox;

        if (isDay)
        {
            targetAmbientColor = nightAmbientColor;
            targetSunColor = nightColor;
            targetIntensity = minSunIntensity;
            targetSkybox = nightSkybox;
        }
        else
        {
            targetAmbientColor = dayAmbientColor;
            targetSunColor = dayColor;
            targetIntensity = maxSunIntensity;
            targetSkybox = daySkybox;
        }

        Color startSunColor = sun.color;
        float startIntensity = sun.intensity;

        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            float t = timer / transitionDuration;

            sun.color = Color.Lerp(startSunColor, targetSunColor, t);
            sun.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
            RenderSettings.ambientLight = Color.Lerp(startAmbientColor, targetAmbientColor, t);

            yield return null;
        }

        isDay = !isDay;

        sunSprite.gameObject.SetActive(isDay);
        moonSprite.gameObject.SetActive(!isDay);

        if (isDay)
        {
            cycleDuration = dayDuration;
            RenderSettings.skybox = daySkybox;

            if (enemySpawnCoroutine != null)
            {
                StopCoroutine(enemySpawnCoroutine);
            }

            dayCount++;
            DeathTextDays.text = dayCount.ToString() + " Days";
            DaysCountText.text = dayCount.ToString();
        }
        else
        {
            cycleDuration = nightDuration;
            RenderSettings.skybox = nightSkybox;

            enemySpawnCoroutine = StartCoroutine(SpawnEnemies());
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while (!isDay)
        {
            if (currentEnemies.Count < baseMaxEnemies + dayCount)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                currentEnemies.Add(enemy);
            }

            yield return new WaitForSeconds(enemySpawnInterval);
        }
    }
}
