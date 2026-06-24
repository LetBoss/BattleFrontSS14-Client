// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Laws.SiliconLawset
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Silicons.Laws;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class SiliconLawset : ISerializationGenerated<SiliconLawset>, ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public List<SiliconLaw> Laws = new List<SiliconLaw>();
  [DataField(null, false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string ObeysTo = string.Empty;

  public string LoggingString()
  {
    List<string> values = new List<string>(this.Laws.Count);
    foreach (SiliconLaw law in this.Laws)
      values.Add($"{law.Order}: {Loc.GetString(law.LawString)}");
    return string.Join(" / ", (IEnumerable<string>) values);
  }

  public SiliconLawset Clone()
  {
    List<SiliconLaw> siliconLawList = new List<SiliconLaw>(this.Laws.Count);
    foreach (SiliconLaw law in this.Laws)
      siliconLawList.Add(law.ShallowClone());
    return new SiliconLawset()
    {
      Laws = siliconLawList,
      ObeysTo = this.ObeysTo
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SiliconLawset target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<SiliconLawset>(this, ref target, hookCtx, false, context))
      return;
    List<SiliconLaw> target1 = (List<SiliconLaw>) null;
    if (this.Laws == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<SiliconLaw>>(this.Laws, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<List<SiliconLaw>>(this.Laws, hookCtx, context);
    target.Laws = target1;
    string target2 = (string) null;
    if (this.ObeysTo == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ObeysTo, ref target2, hookCtx, false, context))
      target2 = this.ObeysTo;
    target.ObeysTo = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SiliconLawset target,
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
    SiliconLawset target1 = (SiliconLawset) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public SiliconLawset Instantiate() => new SiliconLawset();
}
