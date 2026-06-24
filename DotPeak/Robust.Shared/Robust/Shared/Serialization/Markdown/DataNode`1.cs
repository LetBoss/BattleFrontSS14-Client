// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Markdown.DataNode`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Serialization.Markdown;

public abstract class DataNode<T>(NodeMark start, NodeMark end) : DataNode(start, end) where T : DataNode<T>
{
  [PreserveBaseOverrides]
  abstract T DataNode.Copy();

  public abstract T? Except(T node);

  [Obsolete("Use SerializationManager.PushComposition()")]
  public abstract T PushInheritance(T node);

  public override DataNode? Except(DataNode node)
  {
    return node is T node1 ? (DataNode) this.Except(node1) : throw new InvalidNodeTypeException();
  }

  [Obsolete("Use SerializationManager.PushComposition()")]
  public override DataNode PushInheritance(DataNode parent)
  {
    return parent is T node ? (DataNode) this.PushInheritance(node) : throw new InvalidNodeTypeException();
  }
}
