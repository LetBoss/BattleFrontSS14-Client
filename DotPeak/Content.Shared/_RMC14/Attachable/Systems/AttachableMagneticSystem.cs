// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Systems.AttachableMagneticSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Armor.Magnetic;
using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class AttachableMagneticSystem : EntitySystem
{
  [Dependency]
  private RMCMagneticSystem _magneticSystem;
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<AttachableMagneticComponent, AttachableAlteredEvent>(new EntityEventRefHandler<AttachableMagneticComponent, AttachableAlteredEvent>(this.OnAttachableAltered));
  }

  private void OnAttachableAltered(
    Entity<AttachableMagneticComponent> attachable,
    ref AttachableAlteredEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    switch (args.Alteration)
    {
      case AttachableAlteredType.Attached:
        RMCMagneticItemComponent magneticItemComponent = this.EnsureComp<RMCMagneticItemComponent>(args.Holder);
        this._magneticSystem.SetMagnetizeToSlots((Entity<RMCMagneticItemComponent>) (args.Holder, magneticItemComponent), attachable.Comp.MagnetizeToSlots);
        break;
      case AttachableAlteredType.Detached:
        this.RemCompDeferred<RMCMagneticItemComponent>(args.Holder);
        break;
    }
  }
}
