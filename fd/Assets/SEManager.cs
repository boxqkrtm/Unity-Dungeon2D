using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEManager : MonoBehaviour
{
    public GameObject cam;
    public List<GameObject> SEs = new List<GameObject>();
    public void Play(int i)
    {
        GameObject SE = SEs[i];
        Instantiate(SE, cam.transform.position, Quaternion.identity);
    }
}
