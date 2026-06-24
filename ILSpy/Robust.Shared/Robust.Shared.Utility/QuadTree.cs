using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;

namespace Robust.Shared.Utility;

public sealed class QuadTree<T> where T : class, IQuadObject
{
	public sealed class QuadNode
	{
		private static int _id;

		public readonly int ID = _id++;

		private QuadNode[] _nodes = new QuadNode[4];

		internal List<T> quadObjects = new List<T>();

		public QuadNode Parent { get; internal set; }

		public QuadNode this[DiRectangleFion diRectangleFion]
		{
			get
			{
				return diRectangleFion switch
				{
					DiRectangleFion.NW => _nodes[0], 
					DiRectangleFion.NE => _nodes[1], 
					DiRectangleFion.SW => _nodes[2], 
					DiRectangleFion.SE => _nodes[3], 
					_ => null, 
				};
			}
			set
			{
				switch (diRectangleFion)
				{
				case DiRectangleFion.NW:
					_nodes[0] = value;
					break;
				case DiRectangleFion.NE:
					_nodes[1] = value;
					break;
				case DiRectangleFion.SW:
					_nodes[2] = value;
					break;
				case DiRectangleFion.SE:
					_nodes[3] = value;
					break;
				}
				if (value != null)
				{
					value.Parent = this;
				}
			}
		}

		public ReadOnlyCollection<QuadNode> Nodes { get; set; }

		public Box2 Bounds { get; internal set; }

		public ReadOnlyCollection<T> Objects { get; set; }

		public bool HasChildNodes()
		{
			return _nodes[0] != null;
		}

		public QuadNode(Box2 bounds)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			Bounds = bounds;
			Nodes = new ReadOnlyCollection<QuadNode>(_nodes);
			Objects = new ReadOnlyCollection<T>(quadObjects);
		}

		public QuadNode(float x, float y, float width, float height)
			: this(Box2.FromDimensions(x, y, width, height))
		{
		}//IL_0006: Unknown result type (might be due to invalid IL or missing references)

	}

	private readonly bool sort;

	private readonly Vector2 minLeafSizeF;

	private readonly int maxObjectsPerLeaf;

	private QuadNode root;

	private Dictionary<T, QuadNode> objectToNodeLookup = new Dictionary<T, QuadNode>();

	private Dictionary<T, int> objectSortOrder = new Dictionary<T, int>();

	private object syncLock = new object();

	private int objectSortId;

	public QuadNode Root => root;

	public QuadTree(Vector2 minLeafSizeF, int maxObjectsPerLeaf)
	{
		this.minLeafSizeF = minLeafSizeF;
		this.maxObjectsPerLeaf = maxObjectsPerLeaf;
	}

	public int GetSortOrder(T quadObject)
	{
		lock (objectSortOrder)
		{
			if (!objectSortOrder.ContainsKey(quadObject))
			{
				return -1;
			}
			return objectSortOrder[quadObject];
		}
	}

	public QuadTree(Vector2 minLeafSizeF, int maxObjectsPerLeaf, bool sort)
		: this(minLeafSizeF, maxObjectsPerLeaf)
	{
		this.sort = sort;
	}

	public void Insert(T quadObject)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		lock (syncLock)
		{
			if (sort && !objectSortOrder.ContainsKey(quadObject))
			{
				objectSortOrder.Add(quadObject, objectSortId++);
			}
			Box2 bounds = quadObject.Bounds;
			if (root == null)
			{
				Vector2 vector = new Vector2((float)Math.Ceiling(((Box2)(ref bounds)).Width / minLeafSizeF.X), (float)Math.Ceiling(((Box2)(ref bounds)).Height / minLeafSizeF.Y));
				double num = Math.Max(vector.X, vector.Y);
				vector = new Vector2((float)((double)minLeafSizeF.X * num), (float)((double)minLeafSizeF.Y * num));
				Unsafe.SkipInit(out Vector2i val);
				((Vector2i)(ref val))._002Ector((int)(bounds.Left + ((Box2)(ref bounds)).Width / 2f), (int)(bounds.Top + ((Box2)(ref bounds)).Height / 2f));
				Unsafe.SkipInit(out Vector2i val2);
				((Vector2i)(ref val2))._002Ector((int)((float)val.X - vector.X / 2f), (int)((float)val.Y - vector.Y / 2f));
				root = new QuadNode(Box2.FromDimensions(Vector2i.op_Implicit(val2), vector));
			}
			while (true)
			{
				Box2 bounds2 = root.Bounds;
				if (((Box2)(ref bounds2)).Encloses(ref bounds))
				{
					break;
				}
				ExpandRoot(bounds);
			}
			InsertNodeObject(root, quadObject);
		}
	}

	public List<T> Query(Box2 bounds)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		lock (syncLock)
		{
			List<T> list = new List<T>();
			if (root != null)
			{
				Query(bounds, root, list);
			}
			if (sort)
			{
				list.Sort((T a, T b) => objectSortOrder[a].CompareTo(objectSortOrder[b]));
			}
			return list;
		}
	}

	private void Query(Box2 bounds, QuadNode node, List<T> results)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		lock (syncLock)
		{
			if (node == null)
			{
				return;
			}
			Box2 bounds2 = node.Bounds;
			if (!((Box2)(ref bounds)).Intersects(ref bounds2))
			{
				return;
			}
			foreach (T @object in node.Objects)
			{
				bounds2 = @object.Bounds;
				if (((Box2)(ref bounds)).Intersects(ref bounds2))
				{
					results.Add(@object);
				}
			}
			foreach (QuadNode node2 in node.Nodes)
			{
				Query(bounds, node2, results);
			}
		}
	}

	private void ExpandRoot(Box2 newChildBounds)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		lock (syncLock)
		{
			bool num = root.Bounds.Top < newChildBounds.Top;
			bool flag = root.Bounds.Left < newChildBounds.Left;
			DiRectangleFion diRectangleFion = ((!num) ? (flag ? DiRectangleFion.SW : DiRectangleFion.SE) : ((!flag) ? DiRectangleFion.NE : DiRectangleFion.NW));
			Box2 bounds;
			float num2;
			if (diRectangleFion != DiRectangleFion.NW && diRectangleFion != DiRectangleFion.SW)
			{
				float left = root.Bounds.Left;
				bounds = root.Bounds;
				num2 = left - ((Box2)(ref bounds)).Width;
			}
			else
			{
				num2 = root.Bounds.Left;
			}
			double num3 = num2;
			float num4;
			if (diRectangleFion != DiRectangleFion.NW && diRectangleFion != DiRectangleFion.NE)
			{
				float top = root.Bounds.Top;
				bounds = root.Bounds;
				num4 = top - ((Box2)(ref bounds)).Height;
			}
			else
			{
				num4 = root.Bounds.Top;
			}
			double num5 = num4;
			float num6 = (float)num3;
			float num7 = (float)num5;
			bounds = root.Bounds;
			float num8 = ((Box2)(ref bounds)).Width * 2f;
			bounds = root.Bounds;
			QuadNode quadNode = new QuadNode(Box2.FromDimensions(num6, num7, num8, ((Box2)(ref bounds)).Height * 2f));
			SetupChildNodes(quadNode);
			quadNode[diRectangleFion] = root;
			root = quadNode;
		}
	}

	private void InsertNodeObject(QuadNode node, T quadObject)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		lock (syncLock)
		{
			Box2 bounds = node.Bounds;
			Box2 bounds2 = quadObject.Bounds;
			if (!((Box2)(ref bounds)).Encloses(ref bounds2))
			{
				throw new Exception("This should not happen, child does not fit within node bounds");
			}
			if (!node.HasChildNodes() && node.Objects.Count + 1 > maxObjectsPerLeaf)
			{
				SetupChildNodes(node);
				List<T> list = new List<T>(node.Objects);
				List<T> list2 = new List<T>();
				foreach (T item in list)
				{
					foreach (QuadNode node2 in node.Nodes)
					{
						if (node2 != null)
						{
							bounds2 = node2.Bounds;
							bounds = item.Bounds;
							if (((Box2)(ref bounds2)).Encloses(ref bounds))
							{
								list2.Add(item);
							}
						}
					}
				}
				foreach (T item2 in list2)
				{
					RemoveQuadObjectFromNode(item2);
					InsertNodeObject(node, item2);
				}
			}
			foreach (QuadNode node3 in node.Nodes)
			{
				if (node3 != null)
				{
					bounds = node3.Bounds;
					bounds2 = quadObject.Bounds;
					if (((Box2)(ref bounds)).Encloses(ref bounds2))
					{
						InsertNodeObject(node3, quadObject);
						return;
					}
				}
			}
			AddQuadObjectToNode(node, quadObject);
		}
	}

	private void ClearQuadObjectsFromNode(QuadNode node)
	{
		lock (syncLock)
		{
			foreach (T item in new List<T>(node.Objects))
			{
				RemoveQuadObjectFromNode(item);
			}
		}
	}

	private void RemoveQuadObjectFromNode(T quadObject)
	{
		lock (syncLock)
		{
			objectToNodeLookup[quadObject].quadObjects.Remove(quadObject);
			objectToNodeLookup.Remove(quadObject);
		}
	}

	private void AddQuadObjectToNode(QuadNode node, T quadObject)
	{
		lock (syncLock)
		{
			node.quadObjects.Add(quadObject);
			objectToNodeLookup.Add(quadObject, node);
		}
	}

	private void SetupChildNodes(QuadNode node)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		lock (syncLock)
		{
			float x = minLeafSizeF.X;
			Box2 bounds = node.Bounds;
			if (x <= ((Box2)(ref bounds)).Width / 2f)
			{
				float y = minLeafSizeF.Y;
				bounds = node.Bounds;
				if (y <= ((Box2)(ref bounds)).Height / 2f)
				{
					float left = node.Bounds.Left;
					float top = node.Bounds.Top;
					bounds = node.Bounds;
					float width = ((Box2)(ref bounds)).Width / 2f;
					bounds = node.Bounds;
					node[DiRectangleFion.NW] = new QuadNode(left, top, width, ((Box2)(ref bounds)).Height / 2f);
					float left2 = node.Bounds.Left;
					bounds = node.Bounds;
					float x2 = left2 + ((Box2)(ref bounds)).Width / 2f;
					float top2 = node.Bounds.Top;
					bounds = node.Bounds;
					float width2 = ((Box2)(ref bounds)).Width / 2f;
					bounds = node.Bounds;
					node[DiRectangleFion.NE] = new QuadNode(x2, top2, width2, ((Box2)(ref bounds)).Height / 2f);
					float left3 = node.Bounds.Left;
					float top3 = node.Bounds.Top;
					bounds = node.Bounds;
					float y2 = top3 + ((Box2)(ref bounds)).Height / 2f;
					bounds = node.Bounds;
					float width3 = ((Box2)(ref bounds)).Width / 2f;
					bounds = node.Bounds;
					node[DiRectangleFion.SW] = new QuadNode(left3, y2, width3, ((Box2)(ref bounds)).Height / 2f);
					float left4 = node.Bounds.Left;
					bounds = node.Bounds;
					float x3 = left4 + ((Box2)(ref bounds)).Width / 2f;
					float top4 = node.Bounds.Top;
					bounds = node.Bounds;
					float y3 = top4 + ((Box2)(ref bounds)).Height / 2f;
					bounds = node.Bounds;
					float width4 = ((Box2)(ref bounds)).Width / 2f;
					bounds = node.Bounds;
					node[DiRectangleFion.SE] = new QuadNode(x3, y3, width4, ((Box2)(ref bounds)).Height / 2f);
				}
			}
		}
	}

	public void Remove(T quadObject)
	{
		lock (syncLock)
		{
			if (sort && objectSortOrder.ContainsKey(quadObject))
			{
				objectSortOrder.Remove(quadObject);
			}
			if (!objectToNodeLookup.ContainsKey(quadObject))
			{
				throw new KeyNotFoundException("QuadObject not found in dictionary for removal");
			}
			QuadNode quadNode = objectToNodeLookup[quadObject];
			RemoveQuadObjectFromNode(quadObject);
			if (quadNode.Parent != null)
			{
				CheckChildNodes(quadNode.Parent);
			}
		}
	}

	private void CheckChildNodes(QuadNode node)
	{
		lock (syncLock)
		{
			if (GetQuadObjectCount(node) > maxObjectsPerLeaf)
			{
				return;
			}
			foreach (T childObject in GetChildObjects(node))
			{
				if (!node.Objects.Contains(childObject))
				{
					RemoveQuadObjectFromNode(childObject);
					AddQuadObjectToNode(node, childObject);
				}
			}
			if (node[DiRectangleFion.NW] != null)
			{
				node[DiRectangleFion.NW].Parent = null;
				node[DiRectangleFion.NW] = null;
			}
			if (node[DiRectangleFion.NE] != null)
			{
				node[DiRectangleFion.NE].Parent = null;
				node[DiRectangleFion.NE] = null;
			}
			if (node[DiRectangleFion.SW] != null)
			{
				node[DiRectangleFion.SW].Parent = null;
				node[DiRectangleFion.SW] = null;
			}
			if (node[DiRectangleFion.SE] != null)
			{
				node[DiRectangleFion.SE].Parent = null;
				node[DiRectangleFion.SE] = null;
			}
			if (node.Parent != null)
			{
				CheckChildNodes(node.Parent);
				return;
			}
			int num = 0;
			QuadNode quadNode = null;
			foreach (QuadNode node2 in node.Nodes)
			{
				if (node2 != null && GetQuadObjectCount(node2) > 0)
				{
					num++;
					quadNode = node2;
					if (num > 1)
					{
						break;
					}
				}
			}
			if (num != 1)
			{
				return;
			}
			foreach (QuadNode node3 in node.Nodes)
			{
				if (node3 != quadNode)
				{
					node3.Parent = null;
				}
			}
			root = quadNode;
		}
	}

	private List<T> GetChildObjects(QuadNode node)
	{
		lock (syncLock)
		{
			List<T> list = new List<T>();
			list.AddRange(node.quadObjects);
			foreach (QuadNode node2 in node.Nodes)
			{
				if (node2 != null)
				{
					list.AddRange(GetChildObjects(node2));
				}
			}
			return list;
		}
	}

	public int GetQuadObjectCount()
	{
		lock (syncLock)
		{
			if (root == null)
			{
				return 0;
			}
			return GetQuadObjectCount(root);
		}
	}

	private int GetQuadObjectCount(QuadNode node)
	{
		lock (syncLock)
		{
			int num = node.Objects.Count;
			foreach (QuadNode node2 in node.Nodes)
			{
				if (node2 != null)
				{
					num += GetQuadObjectCount(node2);
				}
			}
			return num;
		}
	}

	public int GetQuadNodeCount()
	{
		lock (syncLock)
		{
			if (root == null)
			{
				return 0;
			}
			return GetQuadNodeCount(root, 1);
		}
	}

	private int GetQuadNodeCount(QuadNode node, int count)
	{
		lock (syncLock)
		{
			if (node == null)
			{
				return count;
			}
			foreach (QuadNode node2 in node.Nodes)
			{
				if (node2 != null)
				{
					count++;
				}
			}
			return count;
		}
	}

	public List<QuadNode> GetAllNodes()
	{
		lock (syncLock)
		{
			List<QuadNode> list = new List<QuadNode>();
			if (root != null)
			{
				list.Add(root);
				GetChildNodes(root, list);
			}
			return list;
		}
	}

	private void GetChildNodes(QuadNode node, ICollection<QuadNode> results)
	{
		lock (syncLock)
		{
			foreach (QuadNode node2 in node.Nodes)
			{
				if (node2 != null)
				{
					results.Add(node2);
					GetChildNodes(node2, results);
				}
			}
		}
	}
}
