﻿using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
[AddComponentMenu("Game/Wall")]
public class Wall : bs
{
    
    public bool die;
    public bool attachRope = true;
    public Vector3 RopeForce = new Vector3(1, 1f, 1);
    public float RopeLength = 1f;
    public float SpeedTrackVell;
    public Vector3 bounchyForce;
    public bool SpeedTrack;

    void Start()
    {
        //if (transform.parent != null && transform.parent.name == "Level" && transform.Find("Cube") != null)
        //    Destroy(transform.Find("Cube").gameObject);

    }
    //public override void Init()
    //{
    //    //if (networkView == null) gameObject.AddComponent<NetworkView>();

    //    base.Init();
    //}        
}
