using UnityEngine;
using System.Collections;

public class CubeBehavior : MonoBehaviour {

	public int x,z;
	public GameControllerScript GameController;

	void OnMouseDown ()	{

		GameController.Activate (gameObject);
		GameController.Movement (gameObject);
	}
}