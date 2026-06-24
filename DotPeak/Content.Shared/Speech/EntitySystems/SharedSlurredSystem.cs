// Decompiled with JetBrains decompiler
// Type: Content.Shared.Speech.EntitySystems.SharedSlurredSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared.Speech.EntitySystems;

public abstract class SharedSlurredSystem : EntitySystem
{
  public virtual void DoSlur(EntityUid uid, TimeSpan time, StatusEffectsComponent? status = null)
  {
  }
}
