// Decompiled with JetBrains decompiler
// Type: Content.Shared.NameIdentifier.NameIdentifierGroupPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared.NameIdentifier;

[Prototype(null, 1)]
public sealed class NameIdentifierGroupPrototype : IPrototype
{
  [DataField("fullName", false, 1, false, false, null)]
  public bool FullName;
  [DataField("prefix", false, 1, false, false, null)]
  public string? Prefix;
  [DataField("maxValue", false, 1, false, false, null)]
  public int MaxValue = 1000;
  [DataField("minValue", false, 1, false, false, null)]
  public int MinValue;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
