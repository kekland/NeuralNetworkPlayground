using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour {

	public TripleLineSensor frontSensor;
	public TripleLineSensor backSensor;

	public float Velocity;
	public float AngularVelocity;

	public bool IsDead = false;
	Rigidbody robotRB;
	void Start()
	{
		robotRB = GetComponent<Rigidbody>();
	}

	float distance = 0;
	public void UpdateDistance(Vector3 startPosition)
	{
		distance = Vector3.Distance(transform.position, startPosition);
	}

	public void SetTravelDistance(float value)
	{
		distance = value;
	}

	public float GetTravelDistance()
	{
		return distance;
	}

	public void Move(float left, float right) {
		Move((left + right) / 2f);
		Turn((left - right) / 2f);
	}

	public void Move(float VelocityMultiplier)
	{
		if (IsDead) { return; }
		VelocityMultiplier = Mathf.Clamp(VelocityMultiplier, -1f, 1f);
		//robotRB.AddRelativeForce(new Vector3(0f, 0f, VelocityMultiplier * Velocity));
		robotRB.MovePosition(transform.position + (VelocityMultiplier * Velocity) * transform.forward * Time.deltaTime);
		//transform.position += (VelocityMultiplier * Velocity) * transform.forward * Time.deltaTime;
	}

	public void Turn(float AngularMultiplier)
	{
		if (IsDead) { return; }
		AngularMultiplier = Mathf.Clamp(AngularMultiplier, -1f, 1f);
		//robotRB.AddRelativeTorque(0f, AngularVelocity * AngularMultiplier, 0f);
		robotRB.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, AngularVelocity * AngularMultiplier * Time.deltaTime, 0f)));
		//transform.Rotate(0f, AngularVelocity * AngularMultiplier * Time.deltaTime, 0f);
	}

	public float[] GetSensorData() {
		bool[] front = frontSensor.ReadData();
		bool[] back = backSensor.ReadData();

		float[] data = new float[6];

		for (int i = 0; i < 3; i++) {
			data[i] = front[i] ? 1 : 0;
			data[i + 3] = back[i] ? 1 : 0;
		}

		return data;
	}

	public bool IsAnySensorHittingLine() {
		float[] data = GetSensorData();
		for (int i = 0; i < data.Length; i++) {
			if(data[i] > 0) {
				return true;
			}
		}
		return false;
	}

	public bool CheckRobotDeath() {
		Ray ray = new Ray(transform.position, new Vector3(0, -1, 0));
		return !Physics.Raycast(ray) && !IsAnySensorHittingLine();
	}
}
