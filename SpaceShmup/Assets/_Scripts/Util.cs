using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Util : MonoBehaviour {

	public static Bounds BoundsUnion ( Bounds b0, Bounds b1 ){

		if (b0.size == Vector3.zero && b1.size != Vector3.zero)
			return b1;
		else if (b0.size != Vector3.zero && b1.size == Vector3.zero)
			return b0;
		else if (b0.size == Vector3.zero && b1.size == Vector3.zero)
			return b0;

		b0.Encapsulate (b1.min);
		b0.Encapsulate (b1.max);

		return b0;

	}

	public static Bounds CombineBoundsOfChildren(GameObject go){

		Bounds b = new Bounds (Vector3.zero, Vector3.zero);

		if (go.renderer != null)
			b = BoundsUnion (b, go.renderer.bounds);

		if (go.collider != null)
			b = BoundsUnion (b, go.collider.bounds);
		foreach (Transform t in go.transform)
			b = BoundsUnion (b, CombineBoundsOfChildren (t.gameObject));

		return b;
		
	}

	static public Bounds camBounds{

		get{

			if (_camBounds.size == Vector3.zero)
				SetCameraBounds();

			return _camBounds;

		}

	}

	private static Bounds _camBounds;

	public static void SetCameraBounds(Camera cam = null){

		if (cam == null)
			cam = Camera.main;

		Vector3 topLeft = new Vector3 (0, 0, 0);
		Vector3 bottomRight = new Vector3 (Screen.width, Screen.height, 0);

		Vector3 boundTLN = cam.ScreenToWorldPoint (topLeft);
		Vector3 boundBRF = cam.ScreenToWorldPoint (bottomRight);

		boundTLN.z += cam.nearClipPlane;
		boundBRF.z += cam.farClipPlane;

		Vector3 center = (boundTLN + boundBRF)/2f;
		_camBounds = new Bounds( center, Vector3.zero);
		_camBounds.Encapsulate( boundTLN );
		_camBounds.Encapsulate( boundBRF );
		
	}

	public static Vector3 ScreenBoundsCheck( Bounds bnd, BoundsTest test = BoundsTest.center){

		return (BoundsInBoundsCheck ( camBounds, bnd, test ));

	}

	public static Vector3 BoundsInBoundsCheck( Bounds bigB, Bounds lilB, BoundsTest test = BoundsTest.onScreen){

		Vector3 pos = lilB.center;
		Vector3 off = Vector3.zero;

		switch (test) {

		case BoundsTest.center:
			if(bigB.Contains(pos))
				return (Vector3.zero);

			if(pos.x > bigB.max.x)
				off.x = pos.x - bigB.max.x;

			else if(pos.x < bigB.min.x)
				off.x = pos.x - bigB.min.x;

			if(pos.y > bigB.max.y)
				off.y = pos.y - bigB.max.y;

			else if( pos.y < bigB.min.y)
				off.y = pos.y - bigB.min.y;

			if(pos.z > bigB.max.z)
				off.z = pos.z - bigB.max.z;
			else if(pos.z < bigB.min.z)
				off.z = pos.z - bigB.min.z;

			return off;

		case BoundsTest.onScreen:
			if(bigB.Contains( lilB.min) && bigB.Contains(lilB.max))
				return Vector3.zero;

			if(lilB.max.x > bigB.max.x)
				off.x = lilB.max.x - bigB.max.x;
			else if(lilB.min.x < bigB.min.x)
				off.x = lilB.min.x - bigB.min.x;

			if(lilB.max.y > bigB.max.y)
				off.y = lilB.max.y - bigB.max.y;
			else if(lilB.min.y < bigB.max.y)
				off.y = lilB.min.y - bigB.min.y;

			if(lilB.max.z > bigB.max.z)
				off.z = lilB.max.z - bigB.max.z;
			else if(lilB.min.z < bigB.min.z)
				off.z = lilB.min.z - bigB.min.z;

			return off;

		case BoundsTest.offScreen:
			bool cMin = bigB.Contains(lilB.min);
			bool cMax = bigB.Contains(lilB.max);

			if(cMin || cMax)
				return Vector3.zero;

			if(lilB.min.x > bigB.max.x)
				off.x = lilB.min.x - bigB.max.x;
			else if(lilB.max.x < bigB.min.x)
				off.x = lilB.max.x - bigB.min.x;

			if(lilB.min.y > bigB.max.y)
				off.y = lilB.min.y - bigB.max.y;
			else if(lilB.max.y < bigB.min.y)
				off.y = lilB.max.y - bigB.min.y;

			if(lilB.min.z > bigB.max.z)
				off.z = lilB.min.z - bigB.max.z;
			else if(lilB.max.z < bigB.min.z)
				off.z = lilB.max.z - bigB.min.z;

			return off;

		}

		return Vector3.zero;

	}

	/*=================== Transform Functions ===================*/

	public static GameObject FindTaggedParent(GameObject go){

		if (go.tag != "Untagged")
			return go;

		if (go.transform.parent == null)
			return (null);

		return FindTaggedParent (go.transform.parent.gameObject);

	}

	public static GameObject FindTaggedParent(Transform t){

		return FindTaggedParent( t.gameObject );

	}

	/*=================== Materials Functions ===================*/

	static public Material[] GetAllMaterials(GameObject go){

		List<Material> mats = new List<Material>();
		if (go.renderer != null)
			mats.Add (go.renderer.material);

		foreach (Transform t in go.transform)
			mats.AddRange (GetAllMaterials (t.gameObject));

		return mats.ToArray();

	}

}


public enum BoundsTest{
	
	center, onScreen, offScreen
	
}