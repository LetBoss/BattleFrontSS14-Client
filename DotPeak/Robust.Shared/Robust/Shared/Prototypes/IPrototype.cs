// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Prototypes.IPrototype
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.ViewVariables;

#nullable enable
namespace Robust.Shared.Prototypes;

public interface IPrototype
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  string ID { get; }
}
