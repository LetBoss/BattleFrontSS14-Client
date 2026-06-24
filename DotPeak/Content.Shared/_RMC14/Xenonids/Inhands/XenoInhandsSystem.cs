// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Inhands.XenoInhandsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Inhands;

public sealed class XenoInhandsSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoInhandsComponent, DidEquipHandEvent>(new EntityEventRefHandler<XenoInhandsComponent, DidEquipHandEvent>(this.OnXenoSpritePickedUp));
    this.SubscribeLocalEvent<XenoInhandsComponent, DidUnequipHandEvent>(new EntityEventRefHandler<XenoInhandsComponent, DidUnequipHandEvent>(this.OnXenoSpriteDropped));
  }

  public void OnXenoSpritePickedUp(Entity<XenoInhandsComponent> xeno, ref DidEquipHandEvent args)
  {
    this.UpdateHand(args.User, args.Equipped, args.Hand, true);
  }

  public void OnXenoSpriteDropped(Entity<XenoInhandsComponent> xeno, ref DidUnequipHandEvent args)
  {
    this.UpdateHand(args.User, args.Unequipped, args.Hand, false);
  }

  private void UpdateHand(EntityUid user, EntityUid item, Hand hand, bool equipped)
  {
    if (!this.HasComp<XenoInhandsComponent>(user) || hand.Location == HandLocation.Middle)
      return;
    string str = string.Empty;
    XenoInhandSpriteComponent comp;
    if (equipped && this.TryComp<XenoInhandSpriteComponent>(item, out comp))
      str = comp.StateName ?? string.Empty;
    this._appearance.SetData(user, (Enum) (XenoInhandVisuals) (hand.Location == HandLocation.Left ? 0 : 1), (object) str);
  }
}
