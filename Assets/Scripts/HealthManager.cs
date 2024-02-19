using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private GameObject heart;
    [SerializeField] private List<GameObject> hearts;


    public void PlaceHearts(int numberOfHearts)
    {
        for (int i = 0; i < numberOfHearts; i++)
        {
            GameObject h = Instantiate(heart, this.transform);
            hearts.Add(h);
        }
    }

    // Update is called once per frame
    public void UpdateHearts(int remainingHearts)
    {
        hearts[remainingHearts].SetActive(false); 
    }
}
