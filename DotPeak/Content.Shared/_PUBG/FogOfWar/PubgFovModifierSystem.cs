// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.FogOfWar.PubgFovModifierSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared._PUBG.FogOfWar;

public sealed class PubgFovModifierSystem : EntitySystem
{
  [Dependency]
  private InventorySystem _inventory;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PubgFovModifierComponent, InventoryRelayedEvent<GetPubgFovEvent>>(new EntityEventRefHandler<PubgFovModifierComponent, InventoryRelayedEvent<GetPubgFovEvent>>(this.OnGetFov));
  }

  private void OnGetFov(
    Entity<PubgFovModifierComponent> ent,
    ref InventoryRelayedEvent<GetPubgFovEvent> args)
  {
    args.Args.Modifier += ent.Comp.ModifierDegrees;
  }

  public float GetEffectiveFov(EntityUid uid, PubgFogOfWarComponent? fog = null)
  {
    if (!this.Resolve<PubgFogOfWarComponent>(uid, ref fog, false))
      return 0.0f;
    GetPubgFovEvent args = new GetPubgFovEvent(fog.FovDegrees);
    InventoryComponent comp;
    if (this.TryComp<InventoryComponent>(uid, out comp))
      this._inventory.RelayEvent<GetPubgFovEvent>((Entity<InventoryComponent>) (uid, comp), ref args);
    else
      this.RaiseLocalEvent<GetPubgFovEvent>(uid, args);
    return Math.Clamp(args.BaseFov + args.Modifier, 1f, 360f);
  }
}
