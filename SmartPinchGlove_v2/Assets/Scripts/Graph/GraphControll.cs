using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChartAndGraph;
using System.Linq;
using UnityEngine.UI;


//데이터 저장할 클래스 
public class MaxPowerData
{
    public float time { get; set; }
    public float data { get; set; }

    public MaxPowerData(float t, float d)
    {
        time = t;
        data = d;
    }
}
public class TrackingData
{
    public float time { get; set; }
    public float data { get; set; }
    public float guide { get; set; }

    public TrackingData(float t, float d, float g)
    {
        time = t;
        data = d;
        guide = g;
    }
}


public class GraphControll : MonoBehaviour
{
    GameObject inputField;
    GameObject legend;
    Text title;
    Text result;
    public GraphChart chart;
    public GameObject canvas;
    public int power = 0;
    public float halfPower = 0f;
    float timer;
    float startOffset = 2f;
    string measurementIndex = "";
    List<MaxPowerData> maxPowerDatas = new List<MaxPowerData>();
    List<TrackingData> trackingDatas = new List<TrackingData>();
    public AudioSource beep;
    float rmsetmp = 0f;
    

    void Start()
    {
        power = 0;
        halfPower = 0f;
        startOffset = 2f;
        canvas = GameObject.Find("Canvas").gameObject;
        canvas.transform.GetChild(0).gameObject.SetActive(true);
        canvas.transform.GetChild(1).gameObject.SetActive(false);
        canvas.transform.GetChild(2).gameObject.SetActive(false);
        legend = canvas.transform.GetChild(1).GetChild(5).gameObject;
        title = canvas.transform.GetChild(1).GetChild(1).GetComponent<Text>();
        result = canvas.transform.GetChild(2).GetChild(0).GetComponent<Text>();
        title.text = "Measurement";
        inputField = canvas.transform.GetChild(1).GetChild(4).GetChild(0).gameObject;
    }

    //그래프 설정
    public void SetGraph()
    {
        if (measurementIndex == "maxPower")
        {
            //메인패널 끄고 그래프 패널 켜기
            canvas.transform.GetChild(0).gameObject.SetActive(false);
            canvas.transform.GetChild(1).gameObject.SetActive(true);
            //레전드 세팅
            SetlLegend();
            title.text = "MaxPower";
            inputField.SetActive(false);
            chart.DataSource.ClearCategory("GuideLine");
            chart.DataSource.ClearCategory("Data");
            chart.DataSource.ClearCategory("RisingTIme1");
            chart.DataSource.ClearCategory("RisingTIme2");
            timer = 0f;
            maxPowerDatas.Clear();
            chart.DataSource.VerticalViewSize = 6000;
            chart.DataSource.HorizontalViewSize = 9;

            //StartCoroutine(MeasureMaxPower());
            //StartCoroutine(ControllBeep());
        }

        else if(measurementIndex == "DCTracking")
        {
            //메인패널 끄고 그래프 패널 켜기
            canvas.transform.GetChild(0).gameObject.SetActive(false);   
            canvas.transform.GetChild(1).gameObject.SetActive(true);
            //레전드 필요없는거 끄기
            SetlLegend();
            title.text = "DC Tracking";
            inputField.SetActive(true);
            chart.DataSource.StartBatch();
            chart.DataSource.ClearCategory("GuideLine");
            chart.DataSource.ClearCategory("Data");
            timer = 0f;
            rmsetmp = 0f;
            trackingDatas.Clear();
            chart.DataSource.VerticalViewSize = 200; //halfPower;
            chart.DataSource.HorizontalViewSize = 20 + startOffset;
            //가이드라인 그리기
            chart.DataSource.StartBatch();
            chart.DataSource.AddPointToCategory("GuideLine", 2, 100);
            chart.DataSource.AddPointToCategory("GuideLine", 20 + startOffset, 100);
            chart.DataSource.EndBatch();

            //StartCoroutine(MeasureDCTracking());
        }

        else if (measurementIndex == "SINTracking")
        {
            canvas.transform.GetChild(0).gameObject.SetActive(false);
            canvas.transform.GetChild(1).gameObject.SetActive(true);
            //레전드 세팅
            SetlLegend();
            title.text = "Sin Tracking";
            inputField.SetActive(true);
            chart.DataSource.ClearCategory("GuideLine");
            chart.DataSource.ClearCategory("Data");
            chart.DataSource.ClearCategory("RisingTIme1");
            chart.DataSource.ClearCategory("RisingTIme2");
            timer = 0f;
            rmsetmp = 0f;
            trackingDatas.Clear();
            chart.DataSource.VerticalViewSize = 200; //halfPower;
            chart.DataSource.HorizontalViewSize = 20 + startOffset;

            chart.DataSource.StartBatch();
            for (float i = startOffset; i < 20 + startOffset; i += 0.1f)
            {
                chart.DataSource.AddPointToCategory("GuideLine", i, Mathf.Sin((0.2f * (Mathf.PI)) * (i - startOffset) - (0.5f * (Mathf.PI))) * (50) + (50));//chart.DataSource.AddPointToCategory("GuideLine",i,Mathf.Sin((0.2f * (Mathf.PI)) * i-(0.5f* (Mathf.PI))) * (halfPower / 4f) + (halfPower / 4f));
            }
            chart.DataSource.EndBatch();
            //StartCoroutine(MeasureSinTracking());
        }
    }

    public void StartMeasurement()
    {
        if (measurementIndex == "maxPower")
        {
            StartCoroutine(ControllBeep());
            StartCoroutine(MeasureMaxPower());
        }

        else if (measurementIndex == "DCTracking")
        {
            power = int.Parse(inputField.GetComponent<InputField>().text);
            halfPower = power / 2f;
            StartCoroutine(MeasureDCTracking());
        }

        else if (measurementIndex == "SINTracking")
        {
            power = int.Parse(inputField.GetComponent<InputField>().text);
            halfPower = power / 2f;
            StartCoroutine(MeasureSinTracking());
        }
    }
    //레전드 세팅
    void SetlLegend()
    {
        if (measurementIndex == "maxPower")
        {
            legend.transform.GetChild(0).gameObject.SetActive(false);
            legend.transform.GetChild(1).gameObject.SetActive(true);
            legend.transform.GetChild(2).gameObject.SetActive(true);
            legend.transform.GetChild(3).gameObject.SetActive(true);
        }

        else if (measurementIndex == "DCTracking")
        {
            legend.transform.GetChild(0).gameObject.SetActive(true);
            legend.transform.GetChild(1).gameObject.SetActive(true);
            legend.transform.GetChild(2).gameObject.SetActive(false);
            legend.transform.GetChild(3).gameObject.SetActive(false);
        }

        else if (measurementIndex == "SINTracking")
        {
            legend.transform.GetChild(0).gameObject.SetActive(true);
            legend.transform.GetChild(1).gameObject.SetActive(true);
            legend.transform.GetChild(2).gameObject.SetActive(false);
            legend.transform.GetChild(3).gameObject.SetActive(false);
        }
    }

    //Beep사운드
    IEnumerator ControllBeep()
    {
        yield return new WaitForSeconds(3f);
        beep.Play();
        yield return new WaitForSeconds(3f);
        beep.Play();
        yield return null;
    }

    //최대힘 측정 함수
    IEnumerator MeasureMaxPower()
    {
        while(timer <= 9)
        {
            maxPowerDatas.Add(new MaxPowerData(timer, SelectFinger.GetInputData()));
            chart.DataSource.AddPointToCategoryRealtime("Data", timer, SelectFinger.GetInputData());
            timer += Time.deltaTime;
            yield return null;
        }
        //Debug.Log( chart.DataSource.GetMaxValue(1,true));    
        var risingStart = maxPowerDatas.Max(x => x.data)* 0.1f;
        var risingEnd = maxPowerDatas.Max(x => x.data) * 0.9f;
        chart.DataSource.StartBatch();
        chart.DataSource.AddPointToCategory("RisingTime1", 0, risingStart);
        chart.DataSource.AddPointToCategory("RisingTime1", 9, risingStart);
        chart.DataSource.AddPointToCategory("RisingTime2", 0, risingEnd);
        chart.DataSource.AddPointToCategory("RisingTime2", 9, risingEnd);
        var risingTIme = maxPowerDatas.Where(x => x.data >= risingEnd).Min(x => x.time) - maxPowerDatas.Where(x => x.data >= risingStart).Min(x => x.time);
        Debug.Log("RT = " + risingTIme);
        chart.DataSource.EndBatch();

        canvas.transform.GetChild(2).gameObject.SetActive(true);
        result.text = "MaxPower : " + maxPowerDatas.Max(x => x.data) + "g\n" + "Rising Time : " + risingTIme ;
    }

    // DC 트래킹 측정
    IEnumerator MeasureDCTracking()
    {
        while (timer >= 0 && timer < startOffset)
        {
            var fingertmp = SelectFinger.GetInputData() / halfPower * 100;
            chart.DataSource.AddPointToCategoryRealtime("Data", timer, fingertmp);
            timer += Time.deltaTime;
            Debug.Log("timer : " + timer);
            yield return null;
        }
        while (timer >= startOffset && timer <= 20 + startOffset)
        {
            var DCtmp = 100f;
            var fingertmp = SelectFinger.GetInputData() / halfPower * 100;
            var rmsePow = Mathf.Pow(DCtmp - fingertmp, 2f);

            trackingDatas.Add(new TrackingData(timer, fingertmp, DCtmp));
            chart.DataSource.AddPointToCategoryRealtime("Data", timer, fingertmp);
            Debug.Log("DC : " + DCtmp + ", finger : " + fingertmp + " RMSE : " + rmsePow);

            rmsetmp += rmsePow;
            timer += Time.deltaTime;
            yield return null;
        }
        canvas.transform.GetChild(2).gameObject.SetActive(true);
        result.text = "RMSE \n" + Mathf.Sqrt(rmsetmp / trackingDatas.Count());
        Debug.Log("평균 : " + Mathf.Sqrt(rmsetmp / trackingDatas.Count()));
    }

    //사인파 트래킹 측정 
    IEnumerator MeasureSinTracking()
    {
        while (timer >= 0 && timer < startOffset)
        {
            var fingertmp = SelectFinger.GetInputData() / halfPower * 100;
            chart.DataSource.AddPointToCategoryRealtime("Data", timer, fingertmp);
            timer += Time.deltaTime;
            yield return null;
        }
        while (timer >= startOffset && timer <= 20 + startOffset)
        {
            var sintmp = Mathf.Sin((0.2f * (Mathf.PI)) * (timer - startOffset) - (0.5f * (Mathf.PI))) * (50) + (50);//var sintmp = Mathf.Sin((0.2f * (Mathf.PI)) * timer - (0.5f * (Mathf.PI))) * (halfPower / 4f) + (halfPower / 4f);
            var fingertmp = SelectFinger.GetInputData() / halfPower * 100;
            var rmsePow = Mathf.Pow(sintmp - fingertmp, 2f);
            trackingDatas.Add(new TrackingData(timer, fingertmp, sintmp));
            chart.DataSource.AddPointToCategoryRealtime("Data", timer, fingertmp);
            Debug.Log("sin : " + sintmp + ", finger : " + fingertmp + " RMSE : " + rmsePow);
            rmsetmp += rmsePow;
            timer += Time.deltaTime;
            yield return null;
        }
        canvas.transform.GetChild(2).gameObject.SetActive(true);
        result.text = "RMSE\n" + Mathf.Sqrt(rmsetmp / trackingDatas.Count());
        Debug.Log( "평균 : " + Mathf.Sqrt(rmsetmp / trackingDatas.Count()));
    }

    //버튼으로 모드 변경
    public void SetMeasurementIndex(string s)
    {
        measurementIndex = s;

        SetGraph();
    }
    
    
}
