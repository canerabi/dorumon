﻿using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;
using doru;
using System.Text.RegularExpressions;
using System.Linq;

public class Shared : Base
{
    public bool isController { get { return selected == Network.player.GetHashCode(); } }
    public Vector3 syncPos;
    public Quaternion syncRot;
    public Vector3 syncVelocity;
    public Vector3 syncAngularVelocity;
    protected Vector3 spawnpos;
    public bool velSync = true, posSync = true, rotSync = true, angSync = true, Sync = true;
    public int selected = -1;
    public float tsendpackets;
    public bool shared = true;
    public Renderer[] renderers;
    [LoadPath("Collision1")]
    public AudioClip soundcollision;
    protected override void Awake()
    {
        renderers = this.GetComponentsInChildren<Renderer>().Distinct().ToArray();
        base.Awake();
    }
    public override void Init()
    {
        gameObject.isStatic = false;
        gameObject.AddOrGet<NetworkView>().observed = this;
        gameObject.AddOrGet<Rigidbody>();
        gameObject.AddOrGet<AudioSource>();        
        if (collider is MeshCollider)
        {
            ((MeshCollider)collider).convex = true;
            rigidbody.centerOfMass = transform.worldToLocalMatrix.MultiplyPoint(collider.bounds.center);
        }
        base.Init();
    }
    protected override void Start()
    {
        
        spawnpos = transform.position;
        if (shared)
            if (!Network.isServer)
                networkView.RPC("AddNetworkView", RPCMode.AllBuffered, Network.AllocateViewID());
        base.Start();
    }
    
    
    protected virtual void Update()
    {
        if (_TimerA.TimeElapsed(100))
            UpdateLightmap(renderers.SelectMany(a => a.materials));

        tsendpackets -= Time.deltaTime;
        if (_Game.bounds != null && !_Game.bounds.collider.bounds.Contains(this.transform.position) && enabled)
        {
            transform.position = SpawnPoint();
            rigidbody.velocity = Vector3.zero;
        }

        if (shared && Network.isServer)
            ControllerUpdate();
    }
    void ControllerUpdate()
    {
        float min = float.MaxValue;
        Destroible nearp = null;
        foreach (Destroible p in _Game.destroyables)
            if (p != null)
            {
                if (p.Alive && p.OwnerID != -1)
                {
                    float dist = Vector3.Distance(p.transform.position, this.transform.position);
                    if (min > dist)
                        nearp = p;
                    min = Math.Min(dist, min);
                }
            }

        if (nearp != null && nearp.OwnerID != -1 && selected != nearp.OwnerID)
            SetController(nearp.OwnerID);

    }
    public override void OnPlayerConnected1(NetworkPlayer np)
    {
        if (OwnerID != -1) RPCSetOwner(OwnerID);
        if (selected != -1) RPCSetController(selected);
        base.OnPlayerConnected1(np);
    }

    public void RPCSetOwner(int owner) { CallRPC("SetOwner", owner); }
    [RPC]
    void SetOwner(int owner)
    {
        SetController(owner);
        foreach (Base bas in GetComponentsInChildren(typeof(Base)))
        {
            bas.OwnerID = owner;
            bas.OnSetOwner();
        }
    }

    public void RPCSetController(int owner) { CallRPC("SetController", owner); }
    [RPC]
    public void SetController(int owner)
    {
        this.selected = owner;
    }
    public void RPCResetOwner() { CallRPC("ResetOwner"); }
    [RPC]
    public void ResetOwner()
    {
        Debug.Log("_ResetOwner");
        this.selected = -1;
        foreach (Base bas in GetComponentsInChildren(typeof(Base)))
            bas.OwnerID = -1;

    }
    [RPC]
    public void AddNetworkView(NetworkViewID id)
    {
        var ss = networkView.stateSynchronization;
        NetworkView nw = this.gameObject.AddComponent<NetworkView>();
        nw.group = (int)GroupNetwork.RPCAssignID;
        nw.observed = this;
        nw.stateSynchronization = ss;
        nw.viewID = id;
        name += "+" + Regex.Match(nw.viewID.ToString(), @"\d+").Value;
    }
    public void RPCSetOwner()
    {
        RPCSetOwner(Network.player.GetHashCode());
    }
    public virtual Vector3 SpawnPoint()
    {
        return spawnpos;
    }
    protected virtual void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (!enabled || !Sync) return;
        if (selected == Network.player.GetHashCode() || stream.isReading || (Network.isServer && info.networkView.owner.GetHashCode() == selected))
        {
            if (stream.isReading || this.GetType() != typeof(Zombie) || tsendpackets < 0)
                lock ("ser")
                {
                    tsendpackets = .3f;
                    if (stream.isWriting)
                    {
                        syncPos = pos;
                        syncRot = rot;
                        syncVelocity = rigidbody.velocity;
                        syncAngularVelocity = rigidbody.angularVelocity;
                    }
                    if (posSync) stream.Serialize(ref syncPos);
                    if (velSync) stream.Serialize(ref syncVelocity);
                    if (rotSync) stream.Serialize(ref syncRot);
                    if (angSync) stream.Serialize(ref syncAngularVelocity);
                    if (stream.isReading)//&& syncPos != default(Vector3)
                    {
                        if (posSync) pos = syncPos;
                        if (velSync) rigidbody.velocity = syncVelocity;
                        if (rotSync) rot = syncRot;
                        if (angSync) rigidbody.angularVelocity = syncAngularVelocity;
                    }
                }
        }
    }
}