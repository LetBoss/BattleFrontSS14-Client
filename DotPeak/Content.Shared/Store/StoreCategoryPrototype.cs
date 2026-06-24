// Decompiled with JetBrains decompiler
// Type: Content.Shared.Store.StoreCategoryPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared.Store;

[Prototype(null, 1)]
public sealed class StoreCategoryPrototype : IPrototype
{
  private string _name = string.Empty;

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("name", false, 1, false, false, null)]
  public string Name { get; private set; } = "";

  [DataField("priority", false, 1, false, false, null)]
  public int Priority { get; private set; }
}
