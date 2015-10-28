using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {

	static public Hero S;

	public float gameRestartDelay = 2f;
	public float speed;
	public float rollMult;
	public float pitchMult;
	public GameObject lastTriggerGo = null;
	public delegate void WeaponFireDelegate ();
	public WeaponFireDelegate fireDelegate;
	public Weapon[] weapons;

	private Bounds bounds;
	private float _shieldLevel = 4;

	void Awake() {

		S = this;
		bounds = Util.CombineBoundsOfChildren (this.gameObject);

	}

	void Start(){

		ClearWeapons ();
		weapons [0].SetType (WeaponType.blaster);

	}
	
	// Update is called once per frame
	void Update () {
	
		float xAxis = Input.GetAxis ("Horizontal");
		float yAxis = Input.GetAxis ("Vertical");
		
		Vector3 pos = transform.position;
		if(pos.x + xAxis * speed * Time.deltaTime < 25 && pos.x + xAxis * speed * Time.deltaTime > -25)
			pos.x += xAxis * speed * Time.deltaTime;

		if(pos.y + yAxis * speed * Time.deltaTime > -36 && pos.y + yAxis * speed * Time.deltaTime < 36)
			pos.y += yAxis * speed * Time.deltaTime;

		transform.position = pos;

		transform.rotation = Quaternion.Euler (yAxis * pitchMult, xAxis * rollMult, 0);

		if (Input.GetAxis ("Jump") == 1 && fireDelegate != null)
			fireDelegate ();

	}

	void OnTriggerEnter(Collider Other){

		GameObject go = Util.FindTaggedParent (Other.gameObject);

		if (go != null) {

			if (go == lastTriggerGo)
				return;

			lastTriggerGo = go;

			if (go.tag == "Enemy") {

				shieldLevel--;
				Destroy (go);

			} else if ( go.tag == "PowerUp" ){

				AbsorbPowerUp(go);

			} else {

				print ("Triggered: " + go.name);

			}
		
		} else {

			print ("Triggered: " + Other.gameObject.name);

		}

	}

	public float shieldLevel {

		get{ return _shieldLevel; }

		set{ 

			_shieldLevel = Mathf.Min ( value, 4 );
		
			if(value < 0){ 

				Destroy(this.gameObject); 
				Main.S.DelayedRestart( gameRestartDelay );

			}
		
		}

	}

	public void AbsorbPowerUp( GameObject go){

		PowerUp pu = go.GetComponentInParent<PowerUp> ();
		switch (pu.type) {

		case WeaponType.shield:
			shieldLevel++;
			break;

		default:
			if( pu.type == weapons[0].type){

				Weapon w = GetEmptyWeaponsSlot();
				if(w != null)
					w.SetType(pu.type);

			} else {

				ClearWeapons();
				weapons[0].SetType(pu.type);

			}
			break;

		}
		pu.AbsorbedBy (this.gameObject);

	}

	Weapon GetEmptyWeaponsSlot(){

		for (int i = 0; i < weapons.Length; i++)
			if (weapons [i].type == WeaponType.none)
				return (weapons [i]);
		return null;

	}

	void ClearWeapons(){

		foreach (Weapon w in weapons)
			w.SetType (WeaponType.none);

	}

}