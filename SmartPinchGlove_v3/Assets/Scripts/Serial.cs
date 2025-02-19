﻿using UnityEngine;
using System.IO.Ports;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;


public static class Inputdata // 다른 스크립트에서 사용을 위한 데이터 값 저장용 변수 생성
{
    public static int start;
    public static int index_F;
    public static int mid_F;
    public static int ring_F;
    public static int little_F;
    public static int thumb;
    public static int end;
}

public class Serial : MonoBehaviour
{
    bool sendingFlag;
    private SerialPort sp;
    public int[] data;
    string[] tempstr;

    string[] splitedTmp;
    string str = "";

    Queue<string> queue = new Queue<string>();


    public static Serial instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        ConnectSerial();
        //SerialSendingStop(); // 연결된 이후 게임 플레이 전까지 데이터 전송 stop시켜놓음
    }

    public void ConnectSerial() // 검색해서 자동 연결
    {
        string[] ports = SerialPort.GetPortNames();
        foreach (string p in ports)
        {
            sp = new SerialPort(p, 115200, Parity.None, 8, StopBits.One); // 초기화

            try
            {
                sp.WriteTimeout = 500;
                sp.Open(); // 프로그램 시작시 포트 열기
                sp.Write("b");
                sendingFlag = true;
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                continue;
            }

            Debug.Log("send message");
            Debug.Log(p);

            if (sp.ReadLine().Equals(""))
            {
                continue;
            }
            else break;
        }        
    }


    void Update()
    {
        if (sendingFlag)
        {
            MySerialReceived();
        }
    }
    


    private void MySerialReceived()  //데이터 가공
    {
        // 인큐 과정
       string tmp = sp.ReadExisting(); //업데이트 마다 현재 입력 버퍼에서 가져옴
       splitedTmp = tmp.Split('\n');   // 가져온거 줄바꿈 기준으로 잘라서 배열에 넣음
       foreach (string s in splitedTmp)    //잘라서 넣은 배열하나씩 돌면서 빈 배열이 아니면 queue에 넣어줌
       {
           if (s != "")
           {
               queue.Enqueue(s.Replace("\r",""));
               GetDataFromQueue();     // foreach 반복문에 디큐 과정을 같이 넣어서 큐가 쌓이는 상황 해결
           }
       }
    }

    void GetDataFromQueue() //디큐 과정
    {
        if (queue.Any())    //큐가 비어있지 않으면
        {
            if (str == "")  //str 초기화 상태인지 확인
                str = queue.Dequeue();  // queue에서 빼서 str에 넣어줌
            else if (!str.Contains('a'))    // 현재 조건에서 str의 맨 처음에 a가 없을 경우 받아온 데이터가 깨진 상태일 것으로 예상 
            {
                Debug.Log("a없음:" + str);
                str = "";
            }
            if (str.Contains('a') && str.Contains('b'))  //str이 a와 b를 포함하고 있으면 
            {                                                            // "a,0000,0000,0000,0000,0000,b\r" 이 형식이 필요한데 \r은 버려도 됨
                str = str.Replace('a', '2').Replace('b', '3');            //a를2로, b를3으로 바꿔주고 데이터 처리  a를 버리고 처리할까? 흠 굳이 이긴 한데 그럼 조금더 빠르긴 할듯?
                datareceive();  //데이터 처리
            }
            else if (queue.Any())    //위 if문 처럼 완벽한 포멧을 갖추고 있지 않으면 && queue가 비어있지 않으면 -> str에 queue에서 빼서 더해줌, 이후 다시 돌면서 포멧 갖춰지면 데이터 처리됨
            {
                str += queue.Dequeue();
            }
        }
    }

    void datareceive() 
    {
        tempstr = str.Split(','); // , 단위로 나눠서 배열에 순서대로 저장
        
        try
        {
            data = Array.ConvertAll(tempstr, int.Parse); // int 형으로 변환
            Inputdata.end = data[6];
        }
        catch (Exception e)
        {
            Debug.Log("error" + e);
            return; 
            str = "";
        }
        Inputdata.thumb = data[5];
        Inputdata.little_F = data[4];
        Inputdata.ring_F = data[3];
        Inputdata.mid_F = data[2];
        Inputdata.index_F = data[1];
        Inputdata.start = data[0];
        str = "";
    }


    public void SerialSendingStart()
    {
        sp.Write("b");
        sendingFlag = true;
    }
    public void SerialSendingStop()
    {
        sp.Write("t");
        sendingFlag = false;
    }

    public void Active()
    {
        if (!sp.IsOpen)
        {
            sp.Open();
        }    
        sp.Write("b");
        Debug.Log(sp.ReadLine());
    }

    public void End()
    {
        Debug.Log(sp.ReadLine());
        sp.Write("t");
        sp.Close();
        print("end");
    }

/*    private void OnDisable()
    {
        sp.Close();
    }*/

    private void OnApplicationQuit()
    {
        sp.Close(); // 프로그램 종료시 포트 닫기
    }
}



