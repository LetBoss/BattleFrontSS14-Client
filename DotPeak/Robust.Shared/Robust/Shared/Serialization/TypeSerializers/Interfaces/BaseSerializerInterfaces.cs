// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Interfaces.BaseSerializerInterfaces
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Markdown;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Interfaces;

public static class BaseSerializerInterfaces
{
  public interface ITypeInterface<TType>
  {
  }

  public interface ITypeNodeInterface<TType, TNode> where TNode : DataNode
  {
  }
}
