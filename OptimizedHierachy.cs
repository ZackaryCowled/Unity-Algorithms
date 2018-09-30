using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptimizedHierachy : MonoBehaviour
{
	public Text pointsLabel;
	public int startCount;
	public float pointsPerSecond;
	public bool runDemo = false;
	private int pointsCount = 0;
	private QuadTree quadTree;
	private float timer;

	private void Awake()
	{
		quadTree = new QuadTree(new AABB(new XY(0.0f, 0.0f), 50.0f));

		if (!runDemo)
		{
			for (int i = 0; i < startCount; i++)
			{
				quadTree.Insert(new XY(Random.Range(-50.0f, 50.0f), Random.Range(-50.0f, 50.0f)));
				pointsCount += 1;
			}

			pointsLabel.text = "Points: " + pointsCount.ToString();
		}
	}

	private void OnDrawGizmos()
	{
		if (quadTree != null)
			quadTree.DrawWithGizmos();
	}

	private void Update()
	{
		if (runDemo)
		{
			timer += pointsPerSecond * Time.deltaTime;

			while (timer >= 1.0f)
			{
				timer -= 1.0f;
				quadTree.Insert(new XY(Random.Range(-50.0f, 50.0f), Random.Range(-50.0f, 50.0f)));
				pointsCount += 1;
			}

			pointsLabel.text = "Points: " + pointsCount.ToString();
		}
	}
}