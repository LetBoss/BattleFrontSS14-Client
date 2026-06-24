// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Systems.AttachablePreventDropSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared.Interaction.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class AttachablePreventDropSystem : EntitySystem
{
  public override void Initialize()
  {
    this.SubscribeLocalEvent<AttachablePreventDropToggleableComponent, AttachableAlteredEvent>(new EntityEventRefHandler<AttachablePreventDropToggleableComponent, AttachableAlteredEvent>(this.OnAttachableAltered));
  }

  private void OnAttachableAltered(
    Entity<AttachablePreventDropToggleableComponent> attachable,
    ref AttachableAlteredEvent args)
  {
    switch (args.Alteration)
    {
      case AttachableAlteredType.Activated:
        UnremoveableComponent unremoveableComponent = this.EnsureComp<UnremoveableComponent>(args.Holder);
        unremoveableComponent.DeleteOnDrop = false;
        this.Dirty(args.Holder, (IComponent) unremoveableComponent);
        break;
      case AttachableAlteredType.Deactivated:
        this.RemCompDeferred<UnremoveableComponent>(args.Holder);
        break;
      case AttachableAlteredType.DetachedDeactivated:
        this.RemCompDeferred<UnremoveableComponent>(args.Holder);
        break;
    }
  }
}
