using System;
using UnityEngine;

public class CycleJourney : MonoBehaviour
{
    [SerializeField] private float speedCycle = 5f;
    [SerializeField] private bool dayTime;
    
    private Transform sun;
    private bool isCycling;
    private float angle;

    private void Awake()
    {
        sun = transform;
    }

    private void Update()
    {
        print(sun.eulerAngles);
        if (!isCycling) return;

        float rotateAmount = (dayTime ? 1 : -1) * speedCycle * Time.deltaTime;
        angle += rotateAmount;
        if (angle is > 180 or < -180)
        {
            rotateAmount = angle > 0 ? angle - 180 : angle + 180;
            isCycling = false;
            angle = 0;
        }
        sun.Rotate(new Vector3(rotateAmount, 0, 0));
    }

    public void StartCycle(bool day)
    {
        if (day == dayTime || isCycling) return;
        dayTime = day;
        isCycling = true;
    }
}
