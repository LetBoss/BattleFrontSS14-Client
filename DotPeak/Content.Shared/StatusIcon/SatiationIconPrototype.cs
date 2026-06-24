// Decompiled with JetBrains decompiler
// Type: Content.Shared.StatusIcon.SatiationIconPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

#nullable enable
namespace Content.Shared.StatusIcon;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class SatiationIconPrototype : StatusIconPrototype, IInheritingPrototype
{
  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<SatiationIconPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [NeverPushInheritance]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }
}
