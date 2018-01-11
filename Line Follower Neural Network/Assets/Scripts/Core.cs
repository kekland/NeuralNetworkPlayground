using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;

public class Core : MonoBehaviour
{
	public int RobotCount = 100;
	public List<RobotController> Robots;
	public List<NeuralNetwork> Networks;

	public GameObject RobotPrefab;
	public Vector3 RobotStartPosition;
	public Vector3 RobotOrientation;

	public Text InformationText;
	float startTime;
	public float maxTime;
	int generation = 0;

	public Material NewRobot;
	public Material OldRobot;

	public Vector3 StartPoint;

	public float MaximumEverDistance = 0f;
	void Start()
	{
		for (int i = 0; i < RobotCount; i++)
		{
			GameObject rob = Instantiate(RobotPrefab, RobotStartPosition, Quaternion.Euler(RobotOrientation));
			Robots.Add(rob.GetComponent<RobotController>());

			if (File.Exists(Application.persistentDataPath + "/Data/Network" + i + ".txt"))
			{
				StreamReader reader = new StreamReader(Application.persistentDataPath + "/Data/Network" + i + ".txt");
				Networks.Add(new NeuralNetwork(reader.ReadToEnd()));
			}
			else
			{
				Networks.Add(new NeuralNetwork(new int[] { 6, 12, 6, 2 }));
			}
		}

		Reset();
	}

	void Update()
	{
		if (Time.time - startTime > maxTime)
		{
			Evolve();
			Reset();
		}
		bool isEveryoneDead = true;
		for (int i = 0; i < RobotCount; i++) {
			if(!Robots[i].IsDead) {
				isEveryoneDead = false;
			}
		}

		if(isEveryoneDead) {
			Evolve();
			Reset();
		}

		float maxDistance = 0f;

		for (int i = 0; i < RobotCount; i++)
		{
			Robots[i].UpdateDistance(StartPoint);
			if (Robots[i].GetTravelDistance() > maxDistance)
			{
				maxDistance = Robots[i].GetTravelDistance();
			}
			if(Robots[i].GetTravelDistance() > MaximumEverDistance) {
				MaximumEverDistance = Robots[i].GetTravelDistance();
			}
		}

		string Data = "Evolution Step: " + generation + "\n";
		Data += "Time passed: " + (Time.time - startTime) + "\n";
		Data += "Time left: " + (maxTime - (Time.time - startTime)) + "\n";
		Data += "Maximum distance: " + maxDistance + "\n";
		Data += "Maximum ever achieved distance: " + MaximumEverDistance + "\n";

		InformationText.text = Data;

		for (int i = 0; i < RobotCount; i++)
		{
			ControlRobot(i);
			if (Robots[i].CheckRobotDeath())
			{
				Robots[i].IsDead = true;
			}
		}
	}

	void ControlRobot(int index)
	{
		float[] commands = Networks[index].FeedForward(Robots[index].GetSensorData());
		Debug.Log(commands[0] + " " + commands[1]);
		Robots[index].Move(commands[0], commands[1]);
	}

	void Reset()
	{
		for (int i = 0; i < RobotCount; i++)
		{
			Robots[i].transform.position = RobotStartPosition;
			Robots[i].transform.rotation = Quaternion.Euler(RobotOrientation);
			Robots[i].IsDead = false;
		}
		startTime = Time.time;
	}

	void Evolve()
	{
		for (int i = 0; i < RobotCount; i++)
		{
			Networks[i].SetFitness(Robots[i].GetTravelDistance());
		}

		Networks.Sort();
		for (int i = 0; i < RobotCount / 2; i++)
		{
			Networks[i] = new NeuralNetwork(Networks[i + RobotCount / 2]);
			Networks[i].Mutate();

			Networks[i + RobotCount / 2] = new NeuralNetwork(Networks[i + RobotCount / 2]);
			Robots[i].GetComponent<MeshRenderer>().material = NewRobot;
			Robots[i + RobotCount / 2].GetComponent<MeshRenderer>().material = OldRobot;
		}

		for (int i = 0; i < RobotCount; i++)
		{
			Networks[i].SetFitness(0f);
			Robots[i].SetTravelDistance(0f);
		}

		generation++;
	}

	void OnDisable()
	{
		if (!Directory.Exists(Application.persistentDataPath + "/Data"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/Data");
		}
		for (int i = 0; i < RobotCount; i++)
		{
			StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/Data/Network" + i + ".txt", false);
			writer.Write(Networks[i].ExportToString());
			writer.Close();
		}
	}
}