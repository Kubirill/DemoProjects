using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeCretor : MonoBehaviour
{
    public GameObject tube;
    public GameObject[] tubesPrefub;
    public Transform next;
    public Transform vector;
    public int count;
    void Start()
    {
        CreateNewTube();
    }

    public void CreateNewTube()
    {
        int r = Random.Range(0, 4);
        tube=Instantiate(tubesPrefub[r], next.position, next.rotation);
        r = Random.Range(0, 4);
        tube.transform.RotateAround(next.position, vector.position - next.position, r*90);
        tube.transform.localScale = Vector3.one * 50;
        next = tube.transform.Find("Next");
        vector = tube.transform.Find("Vector");
        foreach (Light l in tube.GetComponentsInChildren<Light>())
        {
            l.range = 100;
            l.intensity = 500;
        }
        count = count - 1;
        if (count>0) CreateNewTube();
        
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) CreateNewTube();
    }
}
