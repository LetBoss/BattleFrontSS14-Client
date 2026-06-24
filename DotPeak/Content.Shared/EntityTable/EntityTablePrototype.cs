// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityTable.EntityTablePrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared.EntityTable;

[Prototype(null, 1)]
public sealed class EntityTablePrototype : IPrototype
{
  [DataField(null, false, 1, true, false, null)]
  public EntityTableSelector Table;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
