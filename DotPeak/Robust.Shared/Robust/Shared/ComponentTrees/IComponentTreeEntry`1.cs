// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ComponentTrees.IComponentTreeEntry`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Physics;

#nullable enable
namespace Robust.Shared.ComponentTrees;

public interface IComponentTreeEntry<TComp> where TComp : Component
{
  EntityUid? TreeUid { get; set; }

  DynamicTree<ComponentTreeEntry<TComp>>? Tree { get; set; }

  bool AddToTree { get; }

  bool TreeUpdateQueued { get; set; }
}
