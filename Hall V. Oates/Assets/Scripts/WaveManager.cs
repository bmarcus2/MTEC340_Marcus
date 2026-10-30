

using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int startingEnemies = 5;
    [SerializeField] private float spawnInterval = 0.8f;
    [SerializeField] private float spawnY = 5f;
    [SerializeField] private float horizontalLimit = 7.5f;

    [Header("Difficulty")]
    [SerializeField] private float startingSpeed = 1f;
    [SerializeField] private float speedIncreasePerWave = 0.25f;
    [SerializeField] private int extraEnemiesPerWave = 2;

    [Header("Zigzag Enemies")]
    [SerializeField] private Sprite zigzagEnemySprite;
    [SerializeField] private int zigzagEnemiesOnWave2 = 1;
    [SerializeField] private int extraZigzagEnemiesPerWave = 1;

    [Header("Power-Up Drops")]
    [SerializeField] private GameObject rapidFirePowerUpPrefab;
    [SerializeField] private GameObject shieldPowerUpPrefab;

    [SerializeField, Range(0f, 1f)]
    private float powerUpDropChance = 0.25f;

    [Header("Power-Down Drops")]
    [SerializeField] private GameObject slowPowerDownPrefab;

    [SerializeField, Range(0f, 1f)]
    private float powerDownDropChance = 0.20f;

    private Coroutine waveRoutine;

    public void StartWaves()
    {
        StopWaves();
        ClearEnemies();

        if (enemyPrefab == null)
        {
            Debug.LogError(
                "Assign the Enemy Prefab in WaveManager!");
            return;
        }

        waveRoutine = StartCoroutine(RunWaves());
    }

    public void StopWaves()
    {
        if (waveRoutine != null)
        {
            StopCoroutine(waveRoutine);
            waveRoutine = null;
        }
    }

    private IEnumerator RunWaves()
    {
        while (GameManager.Instance != null &&
               !GameManager.Instance.IsPlaying)
        {
            yield return null;
        }

        int wave = 0;

        while (GameManager.Instance != null &&
               GameManager.Instance.IsPlaying)
        {
            wave++;

            GameManager.Instance.SetWave(wave);

            int enemyCount = startingEnemies +
                (wave - 1) * extraEnemiesPerWave;

            float speed = startingSpeed +
                (wave - 1) * speedIncreasePerWave;

            // Zigzag enemies start on wave 2.
            int zigzagCount = wave >= 2
                ? zigzagEnemiesOnWave2 +
                  (wave - 2) * extraZigzagEnemiesPerWave
                : 0;

            for (int i = 0; i < enemyCount + zigzagCount; i++)
            {
                if (GameManager.Instance == null ||
                    !GameManager.Instance.IsPlaying)
                {
                    waveRoutine = null;
                    yield break;
                }

                bool useZigzag = i >= enemyCount;

                SpawnEnemy(speed, useZigzag);

                yield return new WaitForSeconds(spawnInterval);
            }

            // Wait until all enemies are destroyed.
            while (GameManager.Instance != null &&
                   GameManager.Instance.IsPlaying &&
                   GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
            {
                yield return new WaitForSeconds(0.2f);
            }

            if (GameManager.Instance == null ||
                !GameManager.Instance.IsPlaying)
            {
                waveRoutine = null;
                yield break;
            }

            yield return new WaitForSeconds(1.5f);
        }

        waveRoutine = null;
    }

    private void SpawnEnemy(float speed, bool useZigzag)
    {
        float x = Random.Range(
            -horizontalLimit, horizontalLimit);

        Vector3 spawnPosition = new Vector3(x, spawnY, 0f);

        GameObject enemy = Instantiate(
            enemyPrefab,
            spawnPosition,
            Quaternion.identity);

        Enemy enemyScript = enemy.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            enemyScript.SetSpeed(speed);

            if (useZigzag)
            {
                enemyScript.SetZigzag(zigzagEnemySprite);
            }
        }
    }

    public void TryDropPowerUp(Vector3 position)
    {
        // Regular power-ups retain their existing drop chance.
        if (Random.value < powerUpDropChance)
        {
            GameObject selectedPrefab = Random.value < 0.5f
                ? rapidFirePowerUpPrefab
                : shieldPowerUpPrefab;

            if (selectedPrefab != null)
            {
                Instantiate(
                    selectedPrefab,
                    position,
                    Quaternion.identity);
            }
        }

        // Independently roll for the slow power-down.
        if (Random.value < powerDownDropChance)
        {
            if (slowPowerDownPrefab != null)
            {
                Instantiate(
                    slowPowerDownPrefab,
                    position,
                    Quaternion.identity);
            }
            else
            {
                Debug.LogWarning(
                    "Assign the Slow Power Down Prefab in WaveManager.");
            }
        }
    }

    private void ClearEnemies()
    {
        foreach (GameObject enemy in
                 GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }
    }
}
