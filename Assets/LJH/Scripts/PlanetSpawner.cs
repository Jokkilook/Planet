using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    [Header("PlanetSetting")]
    public GameObject[] planetPrefabs;

    [Header("Setting")]
    public int count = 5; 
    public float radius = 5f; 

    [Header("OrbitAxis")]
    public Vector3 orbitAxis = new Vector3(-0.5f, 1f, 0f);

    void Start()
    {
        SpawnPlanets();
    }

    void SpawnPlanets()
    {
        if (planetPrefabs.Length == 0)
        {
            Debug.LogError("행성 프리팹을 인스펙터에 넣어주세요!");
            return;
        }

        Quaternion tiltRotation = Quaternion.FromToRotation(Vector3.up, orbitAxis);
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep;

            Vector3 flatPos = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector3 tiltedPos = tiltRotation * flatPos * radius;
            Vector3 spawnPos = transform.position + tiltedPos;

            int index = i % planetPrefabs.Length;

            GameObject newPlanet = Instantiate(planetPrefabs[index], spawnPos, Quaternion.identity);

            OrbitMotion orbit = newPlanet.GetComponent<OrbitMotion>();
            if (orbit != null)
            {
                orbit.target = this.transform;
                orbit.axis = orbitAxis;
            }
        }
    }
}
