// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityList.EntityListPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable
namespace Content.Shared.EntityList;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class EntityListPrototype : IPrototype
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("entities", false, 1, false, false, typeof (PrototypeIdListSerializer<EntityPrototype>))]
  public ImmutableList<string> EntityIds { get; private set; } = ImmutableList<string>.Empty;

  public IEnumerable<EntityPrototype> Entities(IPrototypeManager? prototypeManager = null)
  {
    if (prototypeManager == null)
      prototypeManager = IoCManager.Resolve<IPrototypeManager>();
    foreach (string entityId in this.EntityIds)
      yield return prototypeManager.Index<EntityPrototype>(entityId);
  }
}
