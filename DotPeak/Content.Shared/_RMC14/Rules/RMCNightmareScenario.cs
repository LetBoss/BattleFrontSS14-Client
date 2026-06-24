// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Rules.RMCNightmareScenario
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Rules;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed record RMCNightmareScenario : 
  ISerializationGenerated<RMCNightmareScenario>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string ScenarioName = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public float ScenarioProbability = 1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCNightmareScenario target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<RMCNightmareScenario>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.ScenarioName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ScenarioName, ref target1, hookCtx, false, context))
      target1 = this.ScenarioName;
    target.ScenarioName = target1;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ScenarioProbability, ref target2, hookCtx, false, context))
      target2 = this.ScenarioProbability;
    target.ScenarioProbability = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCNightmareScenario target,
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
    RMCNightmareScenario target1 = (RMCNightmareScenario) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RMCNightmareScenario Instantiate() => new RMCNightmareScenario();

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return (EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.ScenarioName)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.ScenarioProbability);
  }

  [CompilerGenerated]
  public bool Equals(RMCNightmareScenario? other)
  {
    if ((object) this == (object) other)
      return true;
    return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.ScenarioName, other.ScenarioName) && EqualityComparer<float>.Default.Equals(this.ScenarioProbability, other.ScenarioProbability);
  }
}
