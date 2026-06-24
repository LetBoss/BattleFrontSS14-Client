// Decompiled with JetBrains decompiler
// Type: Content.Shared.Interaction.Events.SuicideEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Shared.Interaction.Events;

public sealed class SuicideEvent : HandledEntityEventArgs
{
  public DamageSpecifier? DamageSpecifier;
  public ProtoId<DamageTypePrototype>? DamageType;

  public SuicideEvent(EntityUid victim) => this.Victim = victim;

  public EntityUid Victim { get; private set; }
}
