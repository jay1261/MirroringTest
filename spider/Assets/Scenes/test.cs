using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ChartAndGraph;


public class test : MonoBehaviour
{
    public GraphChart chart;
    public float power = 2000f;
    float time = 0;
    float offset = 0f;

    // Start is called before the first frame update
    void Start()
    {
        offset = power;
        chart.DataSource.StartBatch();
        chart.DataSource.ClearCategory("test1");
        chart.DataSource.ClearCategory("test2");
        chart.DataSource.ClearCategory("test3");
        chart.DataSource.AddPointToCategory("test2", 0, 0);
        chart.DataSource.AddPointToCategory("test2", 1, 0);
        chart.DataSource.AddPointToCategory("test2", 2, offset);
        chart.DataSource.AddPointToCategory("test2", 3, offset);
        chart.DataSource.AddPointToCategory("test2", 4, offset);
        chart.DataSource.AddPointToCategory("test2", 5, offset);
        chart.DataSource.AddPointToCategory("test2", 6, offset);
        chart.DataSource.AddPointToCategory("test2", 7, 0);
        chart.DataSource.AddPointToCategory("test2", 8, 0);
        for (int i = 0; i < 10; i++) {
            chart.DataSource.AddPointToCategory("test3", i, offset);
        }
        chart.HeightRatio = 10;
        chart.DataSource.EndBatch();
        Serial.instance.SerialSendingStart();


    }

    // Update is called once per frame
    void Update()
    {
        if (time < 8)
        {
            chart.DataSource.HorizontalViewSize = 10;
            chart.DataSource.VerticalViewSize = 1000; 
            chart.DataSource.AddPointToCategoryRealtime("test1", time, Inputdata.index_F);
            time += Time.deltaTime;
        }
        else
        {
            time = 0;
            chart.DataSource.ClearCategory("test1");
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Serial.instance.SerialSendingStart();

        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Serial.instance.SerialSendingStop();
        }
    }

}

