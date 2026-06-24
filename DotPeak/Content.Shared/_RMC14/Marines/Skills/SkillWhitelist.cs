// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Skills.SkillWhitelist
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Marines.Skills;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class SkillWhitelist : ISerializationGenerated<SkillWhitelist>, ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public Dictionary<EntProtoId<SkillDefinitionComponent>, int> All = new Dictionary<EntProtoId<SkillDefinitionComponent>, int>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SkillWhitelist target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<SkillWhitelist>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<EntProtoId<SkillDefinitionComponent>, int> target1 = (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null;
    if (this.All == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.All, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.All, hookCtx, context);
    target.All = target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SkillWhitelist target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SkillWhitelist target1 = (SkillWhitelist) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public SkillWhitelist Instantiate() => new SkillWhitelist();
}
