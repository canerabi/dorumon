using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Player : Shared
{
    [FindTransform]
    public Trigger trigger;
    
    public AnimationState idle { get { return an["idle"]; } }
    public AnimationState run { get { return an["run"]; } }
    public AnimationState walk { get { return an["walk"]; } }
    public AnimationState jump { get { return an["jump"]; } } //todo
    public AnimationState fll { get { return an["midair"]; } }
    public AnimationState land { get { return an["landing"]; } }
    public AnimationState hit { get { return an["pawhit1"]; } }
    public AnimationState jumphit { get { return an["aerialattack1"]; } }
    public List<Transform> hands = new List<Transform>();
    public List<Transform> legs = new List<Transform>();
    public Transform HitEffectTrail;
    public bool scndJump;

    public override void Start()
    {
        _Game.shareds.Add(this);
        base.Start();
        
        fll.wrapMode = WrapMode.Loop;
        idle.wrapMode = WrapMode.Loop;
        run.wrapMode = WrapMode.Loop;
        jumphit.wrapMode = hit.wrapMode = land.wrapMode = jump.wrapMode = WrapMode.Clamp;
        
        hit.layer = land.layer = fll.layer = jump.layer = 1;
        jumphit.layer = 2;

        foreach (var a in legs.Union(hands))
            ((Transform)Instantiate(HitEffectTrail, a.transform.position, a.transform.rotation)).parent = a;
    }
    public override void  Update()
    {
        base.Update();
        HitEffect();

        run.speed = 1.5f;
        land.speed = 1.4f;
        bool attackbtn = Input.GetMouseButton(0) && !hit.enabled;
        bool jumpbtn = Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space);
        
        if (scndJump && controller.isGrounded)
            scndJump = false;

        if (fll.enabled && controller.isGrounded)
            fll.enabled = false;

        if (controller.isGrounded)
            vel *= .86f;

        if (jumpbtn && (controller.isGrounded || (!scndJump && _Game.powerType == PowerType.doubleJump)))
        {
            if (!controller.isGrounded)
                scndJump = true;
            vel = Vector3.up * JumpPower * (_Game.powerType == PowerType.HighJump ? 1.5f : 1);
        }

        UpdateNive(attackbtn);
        UpdateAtack(attackbtn);
    }
    private void HitEffect()
    {
        foreach (var a in hands)
            foreach (Transform b in a)
                b.gameObject.active = hit.enabled;

        foreach (var a in legs)
            foreach (Transform b in a)
                b.gameObject.active = jumphit.enabled;
    }
    private void UpdateAtack(bool attackbtn)
    {
        if (attackbtn)
        {
            if (controller.isGrounded)
                an.CrossFade(hit.name);
            else
                an.CrossFade(jumphit.name);

            foreach (Barrel br in trigger.colliders.Where(a => a is Barrel))
                if (br != null)
                    br.Hit();
            foreach (Raccoon r in trigger.colliders.Where(a => a is Raccoon))
                r.Hit();
        }
    }
    public override void AnimationsUpdate()
    {
        var l = controller.velocity.normalized;
        l.y = 0;
        if (l != Vector3.zero)
            an.CrossFade(run.name);
        else
            an.CrossFade(idle.name);

        if (vel.y > .5f)
            an.CrossFade(fll.name);

        base.AnimationsUpdate();
    }
    private void UpdateNive(bool attackbtn)
    {
        
        var keydir = _Cam.rot * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        keydir.y = 0;
        keydir = keydir.normalized;
        var atackdir = keydir;
        if (attackbtn)
            atackdir = _Cam.rot * Vector3.forward;
        atackdir.y =  0;

        if (keydir != Vector3.zero || attackbtn)
            rot = Quaternion.LookRotation(atackdir);

        var move = Vector3.zero;
        move += keydir * Time.deltaTime * 6;
        move += vel * Time.deltaTime;
        vel += Physics.gravity * Time.deltaTime;
        if (!land.enabled)
            controller.Move(move);
    }
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (Mathf.Abs(controller.velocity.y) > 8 && !controller.isGrounded)
        {
            an.CrossFade(land.name);
            vel = Vector3.zero;
        }
    }
}