// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tools.ToolQualityPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Shared.Tools;

[Robust.Shared.Prototypes.Prototype("tool", 1)]
public sealed class ToolQualityPrototype : IPrototype
{
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("name", false, 1, false, false, null)]
  public string Name { get; private set; } = string.Empty;

  [DataField("toolName", false, 1, false, false, null)]
  public string ToolName { get; private set; } = string.Empty;

  [DataField("icon", false, 1, false, false, null)]
  public SpriteSpecifier? Icon { get; private set; }

  [DataField("spawn", false, 1, true, false, typeof (PrototypeIdSerializer<Robust.Shared.Prototypes.EntityPrototype>))]
  public string Spawn { get; private set; } = string.Empty;
}
