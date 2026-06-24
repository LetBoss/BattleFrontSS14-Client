// Decompiled with JetBrains decompiler
// Type: Content.Shared.StatusIcon.JobIconPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.ViewVariables;

#nullable enable
namespace Content.Shared.StatusIcon;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class JobIconPrototype : StatusIconPrototype, IInheritingPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public bool AllowSelection = true;

  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<JobIconPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [NeverPushInheritance]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public string JobName { get; private set; } = string.Empty;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public string LocalizedJobName => Loc.GetString(this.JobName);
}
