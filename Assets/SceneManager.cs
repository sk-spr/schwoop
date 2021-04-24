using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public float initialFallSpeed;
    public List<GameObject> WallTilesL;
    public List<GameObject> WallTilesR;
    [SerializeField]
    private List<GameObject> walls = new List<GameObject>();
    private Dictionary<Transform, GameObject> wallTiles = new Dictionary<Transform, GameObject>();

    public float currentSpeed;
    public float score;
    // Start is called before the first frame update
    void Start()
    {
        addLayer(4);
        //addLayer(5);
        Debug.Log("start");
        currentSpeed = initialFallSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (walls[0].transform.position.y > 5)
        {
            float currentLowest = walls[walls.Count - 1].transform.position.y;
            float oldLowest = currentLowest;
            while(currentLowest > -5)
            {
                //trashy hack but i needed to make sure it checks every time
                currentLowest--;
                addLayer(currentLowest);
            }
            dropLayer();
        }

        
    }
    private void Update()
    {
        for (int i = 0; i < walls.Count; i++)
        {
            var pos = walls[i].transform.position;
            pos.y += currentSpeed * (Time.deltaTime * 60);
            walls[i].transform.position = pos;
        }
    }
    private void addLayer(float y)
    {
        //this code relies on the walls only being added and removed in twos
        //dirty disgusting hack but i am on a deadline

        walls.Add(Instantiate(WallTilesL[0], new Vector3(-2.7f, y, 0), Quaternion.identity));
        walls.Add(Instantiate(WallTilesR[0], new Vector3(2.7f, y, 0), Quaternion.identity));
    }
    private void dropLayer()
    {
        for (int i = 0; i < 2; i++)
        {
            //gets first element of walls, destroys it and removes it from walls
            var temp = walls[0];
            Destroy(walls[0]);
            walls.Remove(temp);
        }
    }

    public void reset()
    {
        
    }

}
