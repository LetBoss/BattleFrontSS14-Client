// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Collision.Shapes.ChainShape
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics.Collision.Shapes;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class ChainShape : 
  IPhysShape,
  IEquatable<IPhysShape>,
  ISerializationGenerated<ChainShape>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Vector2[] Vertices = Array.Empty<Vector2>();
  [DataField(null, false, 1, false, false, null)]
  public Vector2 PrevVertex;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 NextVertex;

  public int Count => this.Vertices.Length - 1;

  public int ChildCount => this.Count - 1;

  [DataField(null, false, 1, false, false, null)]
  public float Radius { get; set; } = 0.01f;

  public ShapeType ShapeType => ShapeType.Chain;

  public void Clear() => this.Vertices = Array.Empty<Vector2>();

  public void CreateLoop(Vector2 position, float radius, bool outer = true, float count = 16f)
  {
    int num1 = Math.Max(16 /*0x10*/, (int) ((double) radius * (double) count));
    float num2 = 6.28318548f / (float) num1;
    Span<Vector2> vertices = stackalloc Vector2[num1];
    for (int index = 0; index < num1; ++index)
    {
      int num3 = outer ? index : -index;
      Vector2 vector2 = new Vector2(MathF.Cos(num2 * (float) num3) * radius, MathF.Sin(num2 * (float) num3) * radius);
      vertices[index] = vector2;
    }
    this.CreateLoop((ReadOnlySpan<Vector2>) vertices);
  }

  public void CreateLoop(ReadOnlySpan<Vector2> vertices)
  {
    int length = vertices.Length;
    if (length < 3)
      return;
    Array.Resize<Vector2>(ref this.Vertices, length + 1);
    vertices.CopyTo((Span<Vector2>) this.Vertices);
    this.Vertices[length] = this.Vertices[0];
    this.PrevVertex = this.Vertices[this.Count - 2];
    this.NextVertex = this.Vertices[1];
  }

  public void CreateChain(ReadOnlySpan<Vector2> vertices, Vector2 prevVertex, Vector2 nextVertex)
  {
    Array.Resize<Vector2>(ref this.Vertices, vertices.Length);
    vertices.CopyTo((Span<Vector2>) this.Vertices);
    this.PrevVertex = prevVertex;
    this.NextVertex = nextVertex;
  }

  public EdgeShape GetChildEdge(ref EdgeShape edge, int index)
  {
    Vector2 v0 = index <= 0 ? this.PrevVertex : this.Vertices[index - 1];
    Vector2 v3 = index >= this.Count - 2 ? this.NextVertex : this.Vertices[index + 2];
    edge.SetOneSided(v0, this.Vertices[index], this.Vertices[index + 1], v3);
    return edge;
  }

  public bool Equals(IPhysShape? other)
  {
    return other is ChainShape otherChain && this.Equals(otherChain);
  }

  public bool Equals(ChainShape otherChain)
  {
    return this.Count == otherChain.Count && this.NextVertex == otherChain.NextVertex && this.PrevVertex == otherChain.PrevVertex && ((ReadOnlySpan<Vector2>) this.Vertices).SequenceEqual<Vector2>((ReadOnlySpan<Vector2>) otherChain.Vertices);
  }

  public Box2 ComputeAABB(Transform transform, int childIndex)
  {
    int index1 = childIndex;
    int index2 = childIndex + 1;
    if (index2 == this.Count)
      index2 = 0;
    Vector2 vector2_1 = Transform.Mul(in transform, in this.Vertices[index1]);
    Vector2 vector2_2 = Transform.Mul(in transform, in this.Vertices[index2]);
    Vector2 vector2_3 = Vector2.Min(vector2_1, vector2_2);
    Vector2 vector2_4 = Vector2.Max(vector2_1, vector2_2);
    Vector2 vector2_5 = new Vector2(this.Radius, this.Radius);
    return new Box2(vector2_3 - vector2_5, vector2_4 + vector2_5);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ChainShape target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ChainShape>(this, ref target, hookCtx, false, context))
      return;
    Vector2[] target1 = (Vector2[]) null;
    if (this.Vertices == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Vector2[]>(this.Vertices, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<Vector2[]>(this.Vertices, hookCtx, context);
    target.Vertices = target1;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Radius, ref target2, hookCtx, false, context))
      target2 = this.Radius;
    target.Radius = target2;
    Vector2 target3 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.PrevVertex, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Vector2>(this.PrevVertex, hookCtx, context);
    target.PrevVertex = target3;
    Vector2 target4 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.NextVertex, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Vector2>(this.NextVertex, hookCtx, context);
    target.NextVertex = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ChainShape target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ChainShape target1 = (ChainShape) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public ChainShape Instantiate() => new ChainShape();
}
