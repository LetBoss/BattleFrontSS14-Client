// Decompiled with JetBrains decompiler
// Type: Content.Shared.Speech.EntitySystems.SharedStutteringSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared.Speech.EntitySystems;

public abstract class SharedStutteringSystem : EntitySystem
{
  public static readonly ProtoId<StatusEffectPrototype> StutterKey = (ProtoId<StatusEffectPrototype>) "Stutter";
  [Dependency]
  private StatusEffectsSystem _statusEffectsSystem;

  public virtual void DoStutter(
    EntityUid uid,
    TimeSpan time,
    bool refresh,
    StatusEffectsComponent? status = null)
  {
  }

  public virtual void DoRemoveStutterTime(EntityUid uid, double timeRemoved)
  {
    this._statusEffectsSystem.TryRemoveTime(uid, (string) SharedStutteringSystem.StutterKey, TimeSpan.FromSeconds(timeRemoved));
  }

  public void DoRemoveStutter(EntityUid uid, double timeRemoved)
  {
    this._statusEffectsSystem.TryRemoveStatusEffect(uid, (string) SharedStutteringSystem.StutterKey);
  }
}
