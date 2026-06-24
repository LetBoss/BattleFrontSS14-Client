// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Prototypes.ParentDataFieldAttribute
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Robust.Shared.Prototypes;

public sealed class ParentDataFieldAttribute(Type prototypeIdSerializer, int priority = 1) : 
  DataFieldAttribute("parent", priority: priority, customTypeSerializer: prototypeIdSerializer)
{
  public const string Name = "parent";
}
