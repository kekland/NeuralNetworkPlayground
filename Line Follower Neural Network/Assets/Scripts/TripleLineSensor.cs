using System;
using UnityEngine;
public class TripleLineSensor : MonoBehaviour {
	public GameObject[] Nodes;
	private MeshRenderer[] Renderers;

	public Material SeesBlack;
	public Material SeesWhite;
	void Start() {
		Renderers = new MeshRenderer[3];
		for (int i = 0; i < 3; i++) {
			Renderers[i] = Nodes[i].GetComponent<MeshRenderer>();

		}
	}
	public LayerMask lineMask;
	public bool[] ReadData() {
		bool[] result = new bool[3];
		for (int i = 0; i < 3; i++) {
			Ray ray = new Ray(Nodes[i].transform.position, new Vector3(0, -1, 0));
			result[i] = Physics.Raycast(ray, 100f, lineMask.value);
		}
		ColorSensors(result);
		return result;
	}

	public void ColorSensors(bool[] result) {
		for (int i = 0; i < 3; i++) {
			Renderers[i].material = (result[i]) ? SeesBlack : SeesWhite;
		}
	}
}