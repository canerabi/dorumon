﻿using System;
using UnityEngine;

public class Cam : bs
{
    
    public bs cursor;
    public Vector3 fakeCursor;
    public Camera cam;
    public bs player { get { return Game.iplayer; } }

    public void Start()
    {
        if (!Application.isEditor)
            Screen.lockCursor = true;
        cam = Camera.main;
        cam.transform.parent = this.transform;
        cam.transform.position = cam.transform.parent.position;
        cam.transform.rotation = cam.transform.parent.rotation;
        cam.GetComponent<GUILayer>().enabled = true;
        cursor.pos = Player.pos;
    }

    public bool disableTips;
    public void Update()
    {       
        if (disableTips != _MenuWindow.Disable_Tips)
        {
            disableTips = _MenuWindow.Disable_Tips;
            if (_MenuWindow.Disable_Tips)
                cam.cullingMask = ~(1 << LayerMask.NameToLayer("Tips"));
            else
                cam.cullingMask = ~0;
        }
        if (Input.GetKeyDown(KeyCode.Tab)) Screen.lockCursor = !Screen.lockCursor;

        Vector2 v =  new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * _MenuWindow.MouseSensivity;
        //if (v.magnitude < 20)
        cursorpos = Vector3.ClampMagnitude(cursorpos + v, 25);
    }
    float fake;
    float fakescale;
    float camoffset= 30;
    public void FixedUpdate()
    {

        camoffset += Input.GetAxis("Mouse ScrollWheel") * -20;
        camoffset = Mathf.Min(Mathf.Max(camoffset, 30), 200);
        fakescale = Mathf.Lerp(camoffset, fakescale, .8f);

        fake = Mathf.Lerp(fake, Player.rigidbody.velocity.sqrMagnitude, 0.095f);
        var pv = player.pos;
        pos = new Vector3(pv.x, pv.y + 10, -fakescale - Mathf.Sqrt(fake));
        cam.transform.LookAt(fakeCursor);
        fakeCursor = Vector3.Lerp(cursor.pos, fakeCursor, 0.95f);
        
        if (!Screen.lockCursor)
            cursor.pos2 = player.pos2;
        else
            cursor.pos2 = player.pos2 + cursorpos;
    }
    Vector2 cursorpos;
} 