// Decompiled with JetBrains decompiler
// Type: Content.Shared.StatusEffect.StatusEffectState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.StatusEffect;

[NetSerializable]
[Serializable]
public sealed class StatusEffectState
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public (TimeSpan, TimeSpan) Cooldown;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool CooldownRefresh = true;
  [Robust.Shared.ViewVariables.ViewVariables]
  public string? RelevantComponent;

  public StatusEffectState((TimeSpan, TimeSpan) cooldown, bool refresh, string? relevantComponent = null)
  {
    this.Cooldown = cooldown;
    this.CooldownRefresh = refresh;
    this.RelevantComponent = relevantComponent;
  }

  public StatusEffectState(StatusEffectState toCopy)
  {
    this.Cooldown = (toCopy.Cooldown.Item1, toCopy.Cooldown.Item2);
    this.CooldownRefresh = toCopy.CooldownRefresh;
    this.RelevantComponent = toCopy.RelevantComponent;
  }
}
