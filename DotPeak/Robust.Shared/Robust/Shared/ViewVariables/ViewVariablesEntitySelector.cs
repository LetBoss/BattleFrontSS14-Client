// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.ViewVariablesEntitySelector
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Robust.Shared.ViewVariables;

[NetSerializable]
[Virtual]
[Serializable]
public class ViewVariablesEntitySelector : ViewVariablesObjectSelector
{
  public ViewVariablesEntitySelector(NetEntity entity) => this.Entity = entity;

  public NetEntity Entity { get; }
}
