// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Components.GetAccessTagsEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Access.Components;

[ByRefEvent]
public record struct GetAccessTagsEvent(
  HashSet<ProtoId<AccessLevelPrototype>> Tags,
  IPrototypeManager PrototypeManager)
{
  public void AddGroup(ProtoId<AccessGroupPrototype> group)
  {
    AccessGroupPrototype accessGroupPrototype;
    if (!this.PrototypeManager.TryIndex<AccessGroupPrototype>(group, ref accessGroupPrototype))
      return;
    this.Tags.UnionWith((IEnumerable<ProtoId<AccessLevelPrototype>>) accessGroupPrototype.Tags);
  }
}
