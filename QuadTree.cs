using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct XY
{
	public float x;
	public float y;

	public XY(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	public static XY operator+(XY a, XY b)
	{
		return new XY(a.x + b.x, a.y + b.y);
	}

	public static XY operator-(XY a, XY b)
	{
		return new XY(a.x - b.x, a.y - b.y);
	}

	public static XY operator*(XY a, XY b)
	{
		return new XY(a.x * b.x, a.y * b.y);
	}

	public static XY operator/(XY a, XY b)
	{
		return new XY(a.x / b.x, a.y / b.y);
	}
}

public struct AABB
{
	public XY center;
	public float halfDimension;

	public AABB(XY center, float halfDimension)
	{
		this.center = center;
		this.halfDimension = halfDimension;
	}

	public bool ContainsPoint(XY point)
	{
		return (point.x >= center.x - halfDimension && point.x <= center.x + halfDimension) &&
			   (point.y >= center.y - halfDimension && point.y <= center.y + halfDimension);
	}

	public bool IntersectsAABB(AABB other)
	{
		return (center.x - halfDimension <= other.center.x + other.halfDimension && center.x + halfDimension >= other.center.x - other.halfDimension) &&
			   (center.y - halfDimension <= other.center.y + other.halfDimension && center.y + halfDimension >= other.center.y - other.halfDimension);
	}
}

public class QuadTree
{
	private readonly int QT_NODE_CAPACITY = 4;
	private AABB boundary;
	private List<XY> points = new List<XY>();
	private QuadTree northWest;
	private QuadTree northEast;
	private QuadTree southWest;
	private QuadTree southEast;

	public QuadTree(AABB boundary)
	{
		this.boundary = boundary;
	}

	public bool Insert(XY point)
	{
		if (!boundary.ContainsPoint(point))
			return false;

		if(points.Count < QT_NODE_CAPACITY)
		{
			points.Add(point);
			return true;
		}

		if (northWest == null)
			Subdivide();

		if (northWest.Insert(point)) return true;
		if (northEast.Insert(point)) return true;
		if (southWest.Insert(point)) return true;
		if (southEast.Insert(point)) return true;

		return false;
	}

	public void Subdivide()
	{
		float quarterDimension = boundary.halfDimension * 0.5f;

		northWest = new QuadTree(new AABB(boundary.center + new XY(-quarterDimension, quarterDimension), quarterDimension));
		northEast = new QuadTree(new AABB(boundary.center + new XY(quarterDimension, quarterDimension), quarterDimension));
		southWest = new QuadTree(new AABB(boundary.center + new XY(-quarterDimension, -quarterDimension), quarterDimension));
		southEast = new QuadTree(new AABB(boundary.center + new XY(quarterDimension, -quarterDimension), quarterDimension));
	}

	public List<XY> QueryRange(AABB range)
	{
		List<XY> pointsInRange = new List<XY>();

		if (!boundary.IntersectsAABB(range))
			return pointsInRange;

		for(int p = 0; p < points.Count; p++)
		{
			if (range.ContainsPoint(points[p]))
				pointsInRange.Add(points[p]);
		}

		if (northWest == null)
			return pointsInRange;

		List<XY> pointsInRangeNorthWest = northWest.QueryRange(range);
		List<XY> pointsInRangeNorthEast = northEast.QueryRange(range);
		List<XY> pointsInRangeSouthWest = southWest.QueryRange(range);
		List<XY> pointsInRangeSouthEast = southEast.QueryRange(range);

		for (int i = 0; i < pointsInRangeNorthWest.Count; i++)
			pointsInRange.Add(pointsInRangeNorthWest[i]);

		for (int i = 0; i < pointsInRangeNorthEast.Count; i++)
			pointsInRange.Add(pointsInRangeNorthEast[i]);

		for (int i = 0; i < pointsInRangeSouthWest.Count; i++)
			pointsInRange.Add(pointsInRangeSouthWest[i]);

		for (int i = 0; i < pointsInRangeSouthEast.Count; i++)
			pointsInRange.Add(pointsInRangeSouthEast[i]);
		
		return pointsInRange;
	}

	public void DrawWithGizmos()
	{
		DrawBoundaryWithGizmos();
		DrawPointsWithGizmos();

		if(northWest != null)
		{
			northWest.DrawWithGizmos();
			northEast.DrawWithGizmos();
			southWest.DrawWithGizmos();
			southEast.DrawWithGizmos();
		}
	}

	private void DrawBoundaryWithGizmos()
	{
		Gizmos.color = Color.white;
		DrawQuadWithGizmos(boundary.center, boundary.halfDimension);
	}

	private void DrawPointsWithGizmos()
	{
		Gizmos.color = Color.green;
		for (int i = 0; i < points.Count; i++)
		{
			Gizmos.DrawSphere(new Vector3(points[i].x, 0.0f, points[i].y), 0.5f);
		}
	}

	private void DrawQuadWithGizmos(XY center, float halfDimension)
	{
		Vector3 topLeft = new Vector3(center.x - halfDimension, 0.0f, center.y + halfDimension);
		Vector3 topRight = new Vector3(center.x + halfDimension, 0.0f, center.y + halfDimension);
		Vector3 bottomLeft = new Vector3(center.x - halfDimension, 0.0f, center.y - halfDimension);
		Vector3 bottomRight = new Vector3(center.x + halfDimension, 0.0f, center.y - halfDimension);

		Gizmos.DrawLine(topLeft, topRight);
		Gizmos.DrawLine(topRight, bottomRight);
		Gizmos.DrawLine(bottomRight, bottomLeft);
		Gizmos.DrawLine(bottomLeft, topLeft);
	}
}
