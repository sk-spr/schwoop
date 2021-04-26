/*
Skye Sprung 04/21 
*/

using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public float initialFallSpeed;
    public List<GameObject> WallTilesL;
    public List<GameObject> WallTilesR;
    public List<GameObject> obstaclePrefabs;
    public List<GameObject> otherObjects;
    public Text scoreText;
    public Text multText;
    public float depth;
    public float initialDifficulty;
    private float difficulty;
    [Header("Debug Fields")]
    [SerializeField]
    private List<GameObject> walls = new List<GameObject>();
    [SerializeField]
    private List<GameObject> obstacles;
    private float nextOffset;
    public float timePassed;
    public float currentSpeed;
    public float score;
    public float multiplier;

    public scoreKeeper keeper;
    void Start()
    {
        AddLayer(5);
        Debug.Log("start");
        currentSpeed = initialFallSpeed;
        timePassed = 0;
        nextOffset = 0;
        multiplier = 1;

        keeper = FindObjectOfType<scoreKeeper>();
        if (keeper == null)
        {
            var temp = Instantiate(new GameObject());
            keeper = temp.AddComponent<scoreKeeper>();
        }
        keeper.score = 0;
    }

    void FixedUpdate()
    {
        currentSpeed = initialFallSpeed * ((multiplier-1)*0.6f + 1);
        if (walls[0].transform.position.y > 5)
        {
            float currentLowest = walls[walls.Count - 1].transform.position.y;
            while (currentLowest > -5)
            {
                //trashy hack but i needed to make sure it checks every time
                currentLowest--;
                AddLayer(currentLowest);
            }
            DropLayer();
        }
        if(timePassed > 1)
        {
            bool canSpawn = true;
            if (obstacles.Count > 0)
                if (obstacles[obstacles.Count - 1].transform.position.y < (-5 + nextOffset))
                    canSpawn = false;
            if(canSpawn)
            {
                if(obstacles.Count < 1)
                {
                    SpawnObstacle(obstaclePrefabs[obstaclePrefabs.Count - 1]);
                    nextOffset = 10;
                } else
                {
                    if (Random.Range(0f, 1f) < 0.01)
                        SpawnObstacle(obstaclePrefabs[obstaclePrefabs.Count - 1]);
                    else
                        SpawnObstacle(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count-1)]);
                    nextOffset = Random.Range(3+multiplier, 2+2*multiplier);
                }
                
            }
        }
        foreach (var obstacle in obstacles)
            if (obstacle.transform.position.y > 20)
                CullObstacle(obstacle);
        foreach (var o in otherObjects)
            if (o.transform.position.y > 20)
                CullOther(o);


    }
    private void Update()
    {
        for (int i = 0; i < walls.Count; i++)
        {
            var pos = walls[i].transform.position;
            pos.y += currentSpeed * (Time.deltaTime * 60);
            walls[i].transform.position = pos;
        }
        for(int i = 0; i < obstacles.Count; i++)
        {
            var pos = obstacles[i].transform.position;
            pos.y += currentSpeed * (Time.deltaTime * 60);
            obstacles[i].transform.position = pos;
        }
        for (int i = 0; i < otherObjects.Count; i++)
        {
            var pos = otherObjects[i].transform.position;
            pos.y += currentSpeed * (Time.deltaTime * 60);
            otherObjects[i].transform.position = pos;
        }
        timePassed += Time.deltaTime;
        depth += Time.deltaTime * 60 * currentSpeed;
        keeper.score += Time.deltaTime * 60 * currentSpeed * multiplier;
        multiplier += Time.deltaTime * currentSpeed;
        scoreText.text = $"score: {(int)keeper.score}";
        multText.text = $"mult: x{multiplier.ToString().CapLen(3)}";
    }
    private void AddLayer(float y)
    {
        //this code relies on the walls only being added and removed in twos
        //dirty disgusting hack but i am on a deadline
        if (WallTilesL.Count < 1 || WallTilesR.Count < 1)
        {
            Debug.LogError("WallTiles list too short");
            return;
        }
        walls.Add(Instantiate(WallTilesL[0], new Vector3(-3f, y, 0), Quaternion.identity));
        walls.Add(Instantiate(WallTilesR[0], new Vector3(3f, y, 0), Quaternion.identity));
    }
    private void DropLayer()
    {
        try
        {
            for (int i = 0; i < 2; i++)
            {
                //gets first element of walls, destroys it and removes it from walls
                var temp = walls[0];
                Destroy(walls[0]);
                walls.Remove(temp);
            }
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    private void SpawnObstacle(GameObject obstacle)
    {
        var instance = Instantiate(obstacle);
        var pos = instance.transform.position;
        pos.y = -5;
        instance.transform.position = pos;
        obstacles.Add(instance);
    }
    private void CullObstacle(GameObject item)
    {
        try
        {
            obstacles.Remove(item);
            Destroy(item);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    private void CullOther(GameObject o)
    {
        try
        {
            otherObjects.Remove(o);
            Destroy(o);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void Reset()
    {
        depth = 0;
        difficulty = initialDifficulty;
        currentSpeed = initialFallSpeed;
        timePassed = 0;
        while(obstacles.Count > 0)
        {
            var temp = obstacles[0];
            Destroy(temp);
            obstacles.Remove(temp);
        }
        multiplier = 1;
        keeper.score = 0;
    }
}
public static class StringExtensions
{
    public static string CapLen(this string s, int l) => (s.Length < l) ? s : s.Substring(0, l);

}
