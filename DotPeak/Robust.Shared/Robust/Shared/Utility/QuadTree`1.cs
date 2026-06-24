// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.QuadTree`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Utility;

public sealed class QuadTree<T> where T : class, IQuadObject
{
  private readonly bool sort;
  private readonly Vector2 minLeafSizeF;
  private readonly int maxObjectsPerLeaf;
  private QuadTree<T>.QuadNode root;
  private Dictionary<T, QuadTree<T>.QuadNode> objectToNodeLookup = new Dictionary<T, QuadTree<T>.QuadNode>();
  private Dictionary<T, int> objectSortOrder = new Dictionary<T, int>();
  private object syncLock = new object();
  private int objectSortId;

  public QuadTree<T>.QuadNode Root => this.root;

  public QuadTree(Vector2 minLeafSizeF, int maxObjectsPerLeaf)
  {
    this.minLeafSizeF = minLeafSizeF;
    this.maxObjectsPerLeaf = maxObjectsPerLeaf;
  }

  public int GetSortOrder(T quadObject)
  {
    lock (this.objectSortOrder)
      return !this.objectSortOrder.ContainsKey(quadObject) ? -1 : this.objectSortOrder[quadObject];
  }

  public QuadTree(Vector2 minLeafSizeF, int maxObjectsPerLeaf, bool sort)
    : this(minLeafSizeF, maxObjectsPerLeaf)
  {
    this.sort = sort;
  }

  public void Insert(T quadObject)
  {
    lock (this.syncLock)
    {
      if (this.sort && !this.objectSortOrder.ContainsKey(quadObject))
        this.objectSortOrder.Add(quadObject, this.objectSortId++);
      Box2 bounds1 = quadObject.Bounds;
      if (this.root == null)
      {
        Vector2 vector2 = new Vector2((float) Math.Ceiling((double) ((Box2) ref bounds1).Width / (double) this.minLeafSizeF.X), (float) Math.Ceiling((double) ((Box2) ref bounds1).Height / (double) this.minLeafSizeF.Y));
        double num = (double) Math.Max(vector2.X, vector2.Y);
        vector2 = new Vector2(this.minLeafSizeF.X * (float) num, this.minLeafSizeF.Y * (float) num);
        Vector2i vector2i1;
        // ISSUE: explicit constructor call
        ((Vector2i) ref vector2i1).\u002Ector((int) ((double) bounds1.Left + (double) ((Box2) ref bounds1).Width / 2.0), (int) ((double) bounds1.Top + (double) ((Box2) ref bounds1).Height / 2.0));
        Vector2i vector2i2;
        // ISSUE: explicit constructor call
        ((Vector2i) ref vector2i2).\u002Ector((int) ((double) vector2i1.X - (double) vector2.X / 2.0), (int) ((double) vector2i1.Y - (double) vector2.Y / 2.0));
        this.root = new QuadTree<T>.QuadNode(Box2.FromDimensions(Vector2i.op_Implicit(vector2i2), vector2));
      }
      while (true)
      {
        Box2 bounds2 = this.root.Bounds;
        if (!((Box2) ref bounds2).Encloses(ref bounds1))
          this.ExpandRoot(bounds1);
        else
          break;
      }
      this.InsertNodeObject(this.root, quadObject);
    }
  }

  public List<T> Query(Box2 bounds)
  {
    lock (this.syncLock)
    {
      List<T> results = new List<T>();
      if (this.root != null)
        this.Query(bounds, this.root, results);
      if (this.sort)
        results.Sort((Comparison<T>) ((a, b) => this.objectSortOrder[a].CompareTo(this.objectSortOrder[b])));
      return results;
    }
  }

  private void Query(Box2 bounds, QuadTree<T>.QuadNode node, List<T> results)
  {
    lock (this.syncLock)
    {
      if (node == null)
        return;
      ref Box2 local1 = ref bounds;
      Box2 bounds1 = node.Bounds;
      ref Box2 local2 = ref bounds1;
      if (!((Box2) ref local1).Intersects(ref local2))
        return;
      foreach (T obj in node.Objects)
      {
        ref Box2 local3 = ref bounds;
        bounds1 = obj.Bounds;
        ref Box2 local4 = ref bounds1;
        if (((Box2) ref local3).Intersects(ref local4))
          results.Add(obj);
      }
      foreach (QuadTree<T>.QuadNode node1 in node.Nodes)
        this.Query(bounds, node1, results);
    }
  }

  private void ExpandRoot(Box2 newChildBounds)
  {
    lock (this.syncLock)
    {
      int num1 = (double) this.root.Bounds.Top < (double) newChildBounds.Top ? 1 : 0;
      bool flag = (double) this.root.Bounds.Left < (double) newChildBounds.Left;
      DiRectangleFion diRectangleFion = num1 == 0 ? (flag ? DiRectangleFion.SW : DiRectangleFion.SE) : (flag ? DiRectangleFion.NW : DiRectangleFion.NE);
      Box2 bounds;
      double num2;
      if (diRectangleFion != DiRectangleFion.NW && diRectangleFion != DiRectangleFion.SW)
      {
        double left = (double) this.root.Bounds.Left;
        bounds = this.root.Bounds;
        double width = (double) ((Box2) ref bounds).Width;
        num2 = left - width;
      }
      else
        num2 = (double) this.root.Bounds.Left;
      double num3 = num2;
      double num4;
      if (diRectangleFion != DiRectangleFion.NW && diRectangleFion != DiRectangleFion.NE)
      {
        double top = (double) this.root.Bounds.Top;
        bounds = this.root.Bounds;
        double height = (double) ((Box2) ref bounds).Height;
        num4 = top - height;
      }
      else
        num4 = (double) this.root.Bounds.Top;
      double num5 = num4;
      double num6 = num3;
      double num7 = num5;
      bounds = this.root.Bounds;
      double num8 = (double) ((Box2) ref bounds).Width * 2.0;
      bounds = this.root.Bounds;
      double num9 = (double) ((Box2) ref bounds).Height * 2.0;
      QuadTree<T>.QuadNode node = new QuadTree<T>.QuadNode(Box2.FromDimensions((float) num6, (float) num7, (float) num8, (float) num9));
      this.SetupChildNodes(node);
      node[diRectangleFion] = this.root;
      this.root = node;
    }
  }

  private void InsertNodeObject(QuadTree<T>.QuadNode node, T quadObject)
  {
    lock (this.syncLock)
    {
      Box2 bounds1 = node.Bounds;
      ref Box2 local1 = ref bounds1;
      Box2 bounds2 = quadObject.Bounds;
      ref Box2 local2 = ref bounds2;
      if (!((Box2) ref local1).Encloses(ref local2))
        throw new Exception("This should not happen, child does not fit within node bounds");
      if (!node.HasChildNodes() && node.Objects.Count + 1 > this.maxObjectsPerLeaf)
      {
        this.SetupChildNodes(node);
        List<T> objList1 = new List<T>((IEnumerable<T>) node.Objects);
        List<T> objList2 = new List<T>();
        foreach (T obj in objList1)
        {
          foreach (QuadTree<T>.QuadNode node1 in node.Nodes)
          {
            if (node1 != null)
            {
              bounds2 = node1.Bounds;
              ref Box2 local3 = ref bounds2;
              Box2 bounds3 = obj.Bounds;
              ref Box2 local4 = ref bounds3;
              if (((Box2) ref local3).Encloses(ref local4))
                objList2.Add(obj);
            }
          }
        }
        foreach (T quadObject1 in objList2)
        {
          this.RemoveQuadObjectFromNode(quadObject1);
          this.InsertNodeObject(node, quadObject1);
        }
      }
      foreach (QuadTree<T>.QuadNode node2 in node.Nodes)
      {
        if (node2 != null)
        {
          Box2 bounds4 = node2.Bounds;
          ref Box2 local5 = ref bounds4;
          bounds2 = quadObject.Bounds;
          ref Box2 local6 = ref bounds2;
          if (((Box2) ref local5).Encloses(ref local6))
          {
            this.InsertNodeObject(node2, quadObject);
            return;
          }
        }
      }
      this.AddQuadObjectToNode(node, quadObject);
    }
  }

  private void ClearQuadObjectsFromNode(QuadTree<T>.QuadNode node)
  {
    lock (this.syncLock)
    {
      foreach (T quadObject in new List<T>((IEnumerable<T>) node.Objects))
        this.RemoveQuadObjectFromNode(quadObject);
    }
  }

  private void RemoveQuadObjectFromNode(T quadObject)
  {
    lock (this.syncLock)
    {
      this.objectToNodeLookup[quadObject].quadObjects.Remove(quadObject);
      this.objectToNodeLookup.Remove(quadObject);
    }
  }

  private void AddQuadObjectToNode(QuadTree<T>.QuadNode node, T quadObject)
  {
    lock (this.syncLock)
    {
      node.quadObjects.Add(quadObject);
      this.objectToNodeLookup.Add(quadObject, node);
    }
  }

  private void SetupChildNodes(QuadTree<T>.QuadNode node)
  {
    lock (this.syncLock)
    {
      double x1 = (double) this.minLeafSizeF.X;
      Box2 bounds = node.Bounds;
      double num1 = (double) ((Box2) ref bounds).Width / 2.0;
      if (x1 > num1)
        return;
      double y1 = (double) this.minLeafSizeF.Y;
      bounds = node.Bounds;
      double num2 = (double) ((Box2) ref bounds).Height / 2.0;
      if (y1 > num2)
        return;
      QuadTree<T>.QuadNode quadNode1 = node;
      double left1 = (double) node.Bounds.Left;
      double top1 = (double) node.Bounds.Top;
      bounds = node.Bounds;
      double width1 = (double) ((Box2) ref bounds).Width / 2.0;
      bounds = node.Bounds;
      double height1 = (double) ((Box2) ref bounds).Height / 2.0;
      QuadTree<T>.QuadNode quadNode2 = new QuadTree<T>.QuadNode((float) left1, (float) top1, (float) width1, (float) height1);
      quadNode1[DiRectangleFion.NW] = quadNode2;
      QuadTree<T>.QuadNode quadNode3 = node;
      double left2 = (double) node.Bounds.Left;
      bounds = node.Bounds;
      double num3 = (double) ((Box2) ref bounds).Width / 2.0;
      double x2 = left2 + num3;
      double top2 = (double) node.Bounds.Top;
      bounds = node.Bounds;
      double width2 = (double) ((Box2) ref bounds).Width / 2.0;
      bounds = node.Bounds;
      double height2 = (double) ((Box2) ref bounds).Height / 2.0;
      QuadTree<T>.QuadNode quadNode4 = new QuadTree<T>.QuadNode((float) x2, (float) top2, (float) width2, (float) height2);
      quadNode3[DiRectangleFion.NE] = quadNode4;
      QuadTree<T>.QuadNode quadNode5 = node;
      double left3 = (double) node.Bounds.Left;
      double top3 = (double) node.Bounds.Top;
      bounds = node.Bounds;
      double num4 = (double) ((Box2) ref bounds).Height / 2.0;
      double y2 = top3 + num4;
      bounds = node.Bounds;
      double width3 = (double) ((Box2) ref bounds).Width / 2.0;
      bounds = node.Bounds;
      double height3 = (double) ((Box2) ref bounds).Height / 2.0;
      QuadTree<T>.QuadNode quadNode6 = new QuadTree<T>.QuadNode((float) left3, (float) y2, (float) width3, (float) height3);
      quadNode5[DiRectangleFion.SW] = quadNode6;
      QuadTree<T>.QuadNode quadNode7 = node;
      double left4 = (double) node.Bounds.Left;
      bounds = node.Bounds;
      double num5 = (double) ((Box2) ref bounds).Width / 2.0;
      double x3 = left4 + num5;
      double top4 = (double) node.Bounds.Top;
      bounds = node.Bounds;
      double num6 = (double) ((Box2) ref bounds).Height / 2.0;
      double y3 = top4 + num6;
      bounds = node.Bounds;
      double width4 = (double) ((Box2) ref bounds).Width / 2.0;
      bounds = node.Bounds;
      double height4 = (double) ((Box2) ref bounds).Height / 2.0;
      QuadTree<T>.QuadNode quadNode8 = new QuadTree<T>.QuadNode((float) x3, (float) y3, (float) width4, (float) height4);
      quadNode7[DiRectangleFion.SE] = quadNode8;
    }
  }

  public void Remove(T quadObject)
  {
    lock (this.syncLock)
    {
      if (this.sort && this.objectSortOrder.ContainsKey(quadObject))
        this.objectSortOrder.Remove(quadObject);
      QuadTree<T>.QuadNode quadNode = this.objectToNodeLookup.ContainsKey(quadObject) ? this.objectToNodeLookup[quadObject] : throw new KeyNotFoundException("QuadObject not found in dictionary for removal");
      this.RemoveQuadObjectFromNode(quadObject);
      if (quadNode.Parent == null)
        return;
      this.CheckChildNodes(quadNode.Parent);
    }
  }

  private void CheckChildNodes(QuadTree<T>.QuadNode node)
  {
    lock (this.syncLock)
    {
      if (this.GetQuadObjectCount(node) > this.maxObjectsPerLeaf)
        return;
      foreach (T childObject in this.GetChildObjects(node))
      {
        if (!node.Objects.Contains(childObject))
        {
          this.RemoveQuadObjectFromNode(childObject);
          this.AddQuadObjectToNode(node, childObject);
        }
      }
      if (node[DiRectangleFion.NW] != null)
      {
        node[DiRectangleFion.NW].Parent = (QuadTree<T>.QuadNode) null;
        node[DiRectangleFion.NW] = (QuadTree<T>.QuadNode) null;
      }
      if (node[DiRectangleFion.NE] != null)
      {
        node[DiRectangleFion.NE].Parent = (QuadTree<T>.QuadNode) null;
        node[DiRectangleFion.NE] = (QuadTree<T>.QuadNode) null;
      }
      if (node[DiRectangleFion.SW] != null)
      {
        node[DiRectangleFion.SW].Parent = (QuadTree<T>.QuadNode) null;
        node[DiRectangleFion.SW] = (QuadTree<T>.QuadNode) null;
      }
      if (node[DiRectangleFion.SE] != null)
      {
        node[DiRectangleFion.SE].Parent = (QuadTree<T>.QuadNode) null;
        node[DiRectangleFion.SE] = (QuadTree<T>.QuadNode) null;
      }
      if (node.Parent != null)
      {
        this.CheckChildNodes(node.Parent);
      }
      else
      {
        int num = 0;
        QuadTree<T>.QuadNode quadNode = (QuadTree<T>.QuadNode) null;
        foreach (QuadTree<T>.QuadNode node1 in node.Nodes)
        {
          if (node1 != null && this.GetQuadObjectCount(node1) > 0)
          {
            ++num;
            quadNode = node1;
            if (num > 1)
              break;
          }
        }
        if (num != 1)
          return;
        foreach (QuadTree<T>.QuadNode node2 in node.Nodes)
        {
          if (node2 != quadNode)
            node2.Parent = (QuadTree<T>.QuadNode) null;
        }
        this.root = quadNode;
      }
    }
  }

  private List<T> GetChildObjects(QuadTree<T>.QuadNode node)
  {
    lock (this.syncLock)
    {
      List<T> childObjects = new List<T>();
      childObjects.AddRange((IEnumerable<T>) node.quadObjects);
      foreach (QuadTree<T>.QuadNode node1 in node.Nodes)
      {
        if (node1 != null)
          childObjects.AddRange((IEnumerable<T>) this.GetChildObjects(node1));
      }
      return childObjects;
    }
  }

  public int GetQuadObjectCount()
  {
    lock (this.syncLock)
      return this.root == null ? 0 : this.GetQuadObjectCount(this.root);
  }

  private int GetQuadObjectCount(QuadTree<T>.QuadNode node)
  {
    lock (this.syncLock)
    {
      int count = node.Objects.Count;
      foreach (QuadTree<T>.QuadNode node1 in node.Nodes)
      {
        if (node1 != null)
          count += this.GetQuadObjectCount(node1);
      }
      return count;
    }
  }

  public int GetQuadNodeCount()
  {
    lock (this.syncLock)
      return this.root == null ? 0 : this.GetQuadNodeCount(this.root, 1);
  }

  private int GetQuadNodeCount(QuadTree<T>.QuadNode node, int count)
  {
    lock (this.syncLock)
    {
      if (node == null)
        return count;
      foreach (QuadTree<T>.QuadNode node1 in node.Nodes)
      {
        if (node1 != null)
          ++count;
      }
      return count;
    }
  }

  public List<QuadTree<T>.QuadNode> GetAllNodes()
  {
    lock (this.syncLock)
    {
      List<QuadTree<T>.QuadNode> results = new List<QuadTree<T>.QuadNode>();
      if (this.root != null)
      {
        results.Add(this.root);
        this.GetChildNodes(this.root, (ICollection<QuadTree<T>.QuadNode>) results);
      }
      return results;
    }
  }

  private void GetChildNodes(QuadTree<T>.QuadNode node, ICollection<QuadTree<T>.QuadNode> results)
  {
    lock (this.syncLock)
    {
      foreach (QuadTree<T>.QuadNode node1 in node.Nodes)
      {
        if (node1 != null)
        {
          results.Add(node1);
          this.GetChildNodes(node1, results);
        }
      }
    }
  }

  public sealed class QuadNode
  {
    private static int _id;
    public readonly int ID = QuadTree<T>.QuadNode._id++;
    private QuadTree<T>.QuadNode[] _nodes = new QuadTree<T>.QuadNode[4];
    internal List<T> quadObjects = new List<T>();

    public QuadTree<T>.QuadNode Parent { get; internal set; }

    public QuadTree<T>.QuadNode this[DiRectangleFion diRectangleFion]
    {
      get
      {
        switch (diRectangleFion)
        {
          case DiRectangleFion.NW:
            return this._nodes[0];
          case DiRectangleFion.NE:
            return this._nodes[1];
          case DiRectangleFion.SW:
            return this._nodes[2];
          case DiRectangleFion.SE:
            return this._nodes[3];
          default:
            return (QuadTree<T>.QuadNode) null;
        }
      }
      set
      {
        switch (diRectangleFion)
        {
          case DiRectangleFion.NW:
            this._nodes[0] = value;
            break;
          case DiRectangleFion.NE:
            this._nodes[1] = value;
            break;
          case DiRectangleFion.SW:
            this._nodes[2] = value;
            break;
          case DiRectangleFion.SE:
            this._nodes[3] = value;
            break;
        }
        if (value == null)
          return;
        value.Parent = this;
      }
    }

    public ReadOnlyCollection<QuadTree<T>.QuadNode> Nodes { get; set; }

    public Box2 Bounds { get; internal set; }

    public ReadOnlyCollection<T> Objects { get; set; }

    public bool HasChildNodes() => this._nodes[0] != null;

    public QuadNode(Box2 bounds)
    {
      this.Bounds = bounds;
      this.Nodes = new ReadOnlyCollection<QuadTree<T>.QuadNode>((IList<QuadTree<T>.QuadNode>) this._nodes);
      this.Objects = new ReadOnlyCollection<T>((IList<T>) this.quadObjects);
    }

    public QuadNode(float x, float y, float width, float height)
      : this(Box2.FromDimensions(x, y, width, height))
    {
    }
  }
}
