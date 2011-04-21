using UnityEngine;
using System.Collections;

public class Score : bs {

    public bs pl { get { return  _Player; } }
	void Start () {
        _Game.blues.Add(this);
        this.GetComponentInChildren<Animation>()["Score"].normalizedTime = Random.value;
	}
	
	void Update () {
        var dist = Vector3.Distance(pl.transform.position, this.transform.position);
        var d = 5;
        if (dist < d)
        {
            //Debug.Log("asd");
            var norm = (pl.transform.position - transform.position).normalized;
            transform.position += norm * (d - dist) * Time.deltaTime * 20;
            //animation.Stop();
            //transform.position += norm;
        }
        if (dist < .5f)
        {
            _Player.scores++;
            Destroy(this.gameObject);
        }
	}
    public void Destroy()
    {

    }
}
