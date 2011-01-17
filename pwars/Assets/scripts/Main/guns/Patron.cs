using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Patron : Base
{    
    public bool decalhole = true;
    public Vector3 Force = new Vector3(0, 0, 80);
    [FindAsset("Detonator-Base")]
    public GameObject detonator;    
    [FindAsset(overide = true)]
    public GameObject Explosion;
    public bool DestroyOnHit;
    public bool explodeOnDestroy;
    public int detonatorsize = 8;
    internal float ExpForce = 2000;
    internal int damage = 60;
    public AnimationCurve ExpDamage;
    internal int probivaemost = 0;
    public float gravitate = 0;
    public float radius = 6;
    public bool breakwall;
    public float timeToDestroy = 5;
    public float tm;
    protected Vector3 previousPosition;
    protected override void Start()
    {
        Force = transform.rotation * Force;
        previousPosition = transform.position;
        base.Start();
    }
    public override void Init()
    {
        base.Init();
    }
    public AnimationCurve SamoNavod;
    protected virtual void Update()
    {
        if (Force != default(Vector3))
            this.transform.position += Force * Time.deltaTime;

        if (gravitate > 0)
            Gravitate();

        if (SamoNavod.length > 0)
            foreach (Destroible p in _Game.players.Union(_Game.zombies.Cast<Destroible>()))
                if (p != null && p.isEnemy(OwnerID))
                    Force += (this.pos - _localPlayer.pos).normalized * Time.deltaTime * this.Force.sqrMagnitude * SamoNavod.Evaluate(Vector3.Distance(this.pos, _localPlayer.pos));

        tm += Time.deltaTime;
        if (tm > timeToDestroy)
        {
            if (explodeOnDestroy)
                Explode(this.transform.position);
            else
                Destroy(gameObject);
        }        

        if (DestroyOnHit)
        {
            Vector3 movementThisStep = transform.position - previousPosition;
            RaycastHit hitInfo;
            Ray ray = new Ray(previousPosition, movementThisStep);


            if (Physics.Raycast(ray, out hitInfo, movementThisStep.magnitude + 1, GetMask()))
            {
                ExplodeOnHit(hitInfo);
            }
        }
        previousPosition = transform.position;        
    }

    private int GetMask()
    {
        int mask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Glass") | 1 << LayerMask.NameToLayer("Level") | 1 << (_localPlayer.isEnemy(OwnerID) ? LayerMask.NameToLayer("Ally") : LayerMask.NameToLayer("Enemy"));
        return mask;
    }
    
    private void Gravitate()
    {
        foreach (Patron p in _Game.patrons)
        {
            if (p.Force.magnitude != 0 && p != this)
                p.Force += (pos - p.pos).normalized * gravitate * 500 * Time.deltaTime * p.Force.sqrMagnitude / Vector3.Distance(p.pos, pos) / 2000;
        }

        foreach (var b in _Game.players.Where(p => p != null && p.isEnemy(OwnerID)).Cast<Base>().Union(_Game.boxes.Cast<Base>()).Union(_Game.patrons.Where(a => a.rigidbody != null).Cast<Base>()).Union(_Game.zombies.Cast<Base>()))
            if (b != null && b != this)
            {
                if (b is Zombie)
                {
                    var z = ((Zombie)b);    
                    z.ResetSpawnTm();
                    z.RPCSetFrozen(true);
                }
                b.rigidbody.AddExplosionForce(-gravitate * 300 * b.rigidbody.mass * fdt, transform.position, 15);
                b.rigidbody.velocity *= .97f;
            }

    }
    protected virtual void ExplodeOnHit(RaycastHit hit)
    {
        var g = hit.collider.gameObject;
        var t = hit.collider.transform;

        transform.position = hit.point + transform.rotation * Vector3.forward;
        bool glass =g.name.Contains("glass");
        
        if (!explodeOnDestroy)
        {
            Transform b = t.root;
            
            if (b.rigidbody != null)
                b.rigidbody.AddForceAtPosition(transform.rotation * new Vector3(0, 0, ExpForce) * fdt , hit.point);
        }

        if (g.layer == LayerMask.NameToLayer("Level") || g.layer == LayerMask.NameToLayer("Glass"))
            _Game.AddDecal(glass ? DecalTypes.glass : (decalhole ? DecalTypes.Hole : DecalTypes.Decal),
                hit.point - rot * Vector3.forward * 0.12f, hit.normal, t);

        Destroible destroible = t.GetMonoBehaviorInParrent() as Destroible;
        

        if (destroible is Zombie && _SettingsWindow.Blood)
        {
            _Game.particles[(int)ParticleTypes.BloodSplatters].Emit(hit.point, transform.rotation);
            RaycastHit h;
            if (Physics.Raycast(new Ray(pos, new Vector3(0, -1, 0)), out h, 10, 1 << LayerMask.NameToLayer("Level") | LayerMask.NameToLayer("MapItem")))
            {
                _Game.AddDecal(
                    DecalTypes.Blood,
                    h.point - new Vector3(0, -1, 0) * 0.1f,
                    h.normal, _Game.decals.transform);
            }
        }
        else
        {
            _Game.particles[(int)ParticleTypes.particle_metal].Emit(hit.point, transform.rotation);
        }
        if (destroible != null && destroible.isController && !destroible.dead)
        {
            destroible.RPCSetLife(destroible.Life - damage, OwnerID);
        }
        
        bool staticfield = g.name.ToLower().StartsWith("staticfield");
        if (staticfield)
            Force = Quaternion.LookRotation(hit.point - g.transform.position) * Vector3.forward * Force.magnitude;
        if (!glass && !staticfield)
        {
            probivaemost--;

            if (explodeOnDestroy)
                Explode(hit.point);
        }
        if(probivaemost<0)
            Destroy(gameObject);
    }
    
    private void Explode(Vector3 pos)
    {
        Vector3 vector3 = pos - this.transform.rotation * new Vector3(0, 0, 2);
        GameObject o;
        Destroy(o = (GameObject)Instantiate(detonator, vector3, Quaternion.identity), .4f);
        GameObject exp = (GameObject)Instantiate(Explosion, o.transform.position, Quaternion.identity);
        exp.transform.parent = o.transform;
        var e = exp.GetComponent<Explosion>();        
        e.exp = ExpForce;
        e.radius = radius;
        if (ExpDamage.length != 0)
            e.damage = ExpDamage;        
        e.OwnerID = OwnerID;
        Destroy(gameObject);
        var dt = detonator.GetComponent<Detonator>();
        dt.size = detonatorsize;
        dt.autoCreateForce = false;
    }
}