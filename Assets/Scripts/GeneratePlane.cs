using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePlane : MonoBehaviour
{
    [SerializeField] private GameObject planeObject;
    [SerializeField] private GameObject obstacleObject;
    [SerializeField] private GameObject foodObject;
    [SerializeField]
    [Range(0, 10)]
    private float obstacleChance = 9.8f;
    [SerializeField]
    [Range(0, 10)]
    private float foodChance = 9.8f;

    private Queue<GameObject> planes = new Queue<GameObject>();
    private GameObject firstPlane;
    public static int PLANE_SIZE = 20;

    struct ObstacleInfo
    {
        public Vector3 position;
        public int radius;
    }

    public GameObject getLastPlane()
    {
        return this.planes.Peek();
    }

    public void destroy()
    {
        GameObject lastPlane = planes.Dequeue();
        Destroy(lastPlane);
    }

    public double euclideanDistance(Vector3 v1, Vector3 v2)
    {
        return Mathf.Sqrt(Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.y - v2.y, 2) + Mathf.Pow(v1.z - v2.z, 2));
    }

    private bool inRadius(Vector3 center, int radius, Vector3 point)
    {
        if(euclideanDistance(center, point) <= radius)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Randomly generate obstacles for the plane
    public void generateObstacles()
    {
        List<ObstacleInfo> obstacleList = new List<ObstacleInfo>();
        int count = 0;

        // Obstacle is generated when the random value is higher than a obstacleChance
        for (float x = firstPlane.transform.position.x - PLANE_SIZE/2 + 1.5f; x < firstPlane.transform.position.x + PLANE_SIZE/2; x++)
        {
            for (float z = firstPlane.transform.position.z - 4; z < firstPlane.transform.position.z + 4; z++)
            {
                count++;
                // Generate an obstacle
                if (Random.value < count / 190f * 0.5f)
                {
                    bool isValidPos = true;
                    foreach(ObstacleInfo info in obstacleList)
                    {
                        if(inRadius(info.position, info.radius, new Vector3(x, 0, z)))
                        {
                            isValidPos = false;
                            break;
                        }
                    }

                    if (isValidPos)
                    {
                        float rnd = Random.value;
                        // Randomly choose size
                        int size;
                        if (rnd < 0.33f)
                        {
                            size = 1;
                        }
                        else if (rnd < 0.66f)
                        {
                            size = 2;
                        }
                        else
                        {
                            size = 3;
                        }

                        GameObject generatedObstacle = Instantiate(obstacleObject);
                        generatedObstacle.transform.parent = firstPlane.transform;
                        generatedObstacle.transform.localScale = new Vector3(size / 2f, 3, size);
                        generatedObstacle.transform.position = new Vector3(x, 0, z);
                        generatedObstacle.GetComponent<Renderer>().material.color = new Color(1, 0, 0);

                        obstacleList.Add(new ObstacleInfo() { position = new Vector3(x, 0, z), radius = size * 3 });
                    }
                }
            }
        }

        for (float x = firstPlane.transform.position.x - PLANE_SIZE / 2 + 1.5f; x < firstPlane.transform.position.x + PLANE_SIZE / 2; x++)
        {
            for (float z = firstPlane.transform.position.z - 4; z < firstPlane.transform.position.z + 4; z++)
            {
                count++;
                if (Random.value < count / 190f * 0.3f)
                {
                    bool isValidPos = true;
                    foreach (ObstacleInfo info in obstacleList)
                    {
                        if (inRadius(info.position, info.radius, new Vector3(x, 0, z)))
                        {
                            isValidPos = false;
                            break;
                        }
                    }

                    if (isValidPos)
                    {
                        GameObject generatedObstacle = Instantiate(foodObject);
                        generatedObstacle.transform.parent = firstPlane.transform;
                        generatedObstacle.transform.localScale = new Vector3(0.5f, 5, 1);
                        generatedObstacle.transform.position = new Vector3(x, 0, z);
                        generatedObstacle.GetComponent<Renderer>().material.color = new Color(0, 1, 0);
                    }
                }
            }
        }
    }

    // Spawn a new plane
    public void spawnPlane()
    {
        Vector3 newPosition = firstPlane.transform.position + new Vector3(0, 0, 10);
        GameObject temp = Instantiate(planeObject, newPosition, Quaternion.identity);
        this.firstPlane = temp;
        generateObstacles();
        this.planes.Enqueue(temp);
    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject plane = Instantiate(planeObject, new Vector3(0, 0, 0), Quaternion.identity);
        planes.Enqueue(plane);
        firstPlane = plane;

        for (int i = 0; i < 15; i++)
        {
            spawnPlane();
        }
    }
}
