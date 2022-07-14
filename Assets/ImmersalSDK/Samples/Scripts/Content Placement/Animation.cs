using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class Animation : MonoBehaviour
{
    private readonly double UpdateTexRate = 1;
    private DateTime current;
    private DateTime old;
    private bool flag = false;


    //string path = @"Assets/ImmersalSDK/Samples/Materials/Content Placement/MatPaintings/";

    public Shader basic;

    List<Material> mats = new List<Material>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (int value in Enumerable.Range(0, 10))
        {
            var loadImg = Resources.Load<Texture2D>("MatPaintings/" + this.gameObject.name.Substring(0, 2) + "/" + this.gameObject.name.Substring(0, 2) + value);
            //Debug.Log(value + "  ");
            Material mat = new Material(basic);
            mat.mainTexture = loadImg;
            mats.Add(mat);
        }
    }
    // Update is called once per frame
    void Update()
    {   
        Transform cameraTransform = Camera.main.transform;

        float dist = Vector3.Distance(cameraTransform.position, this.gameObject.transform.position);
        if (dist <= 2.0f)
        {
            if (!flag)
            {
                old = DateTime.Now;
                flag = true;
            }

            current = DateTime.Now;

            TimeSpan interval = current - old;
            if(interval.TotalSeconds > UpdateTexRate)
            {
                old = current;
                //Debug.Log("Now " + current + " UPDATE!");
                ChangeTex();

            }


            
        }
        else
        {
            flag = false;
        }

    }
    void ChangeTex()
    {
        MeshRenderer mr = this.gameObject.GetComponent<MeshRenderer>();
        mr.material = mats[0];
        mats.Add(mats[0]);
        mats.RemoveAt(0);

    }
}


