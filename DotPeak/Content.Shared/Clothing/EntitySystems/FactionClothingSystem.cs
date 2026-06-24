// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.EntitySystems.FactionClothingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.Components;
using Content.Shared.Inventory.Events;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Prototypes;
using Content.Shared.NPC.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared.Clothing.EntitySystems;

public sealed class FactionClothingSystem : EntitySystem
{
  [Dependency]
  private NpcFactionSystem _faction;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FactionClothingComponent, GotEquippedEvent>(new EntityEventRefHandler<FactionClothingComponent, GotEquippedEvent>((object) this, __methodptr(OnEquipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FactionClothingComponent, GotUnequippedEvent>(new EntityEventRefHandler<FactionClothingComponent, GotUnequippedEvent>((object) this, __methodptr(OnUnequipped)), (Type[]) null, (Type[]) null);
  }

  private void OnEquipped(Entity<FactionClothingComponent> ent, ref GotEquippedEvent args)
  {
    NpcFactionMemberComponent factionMemberComponent;
    this.TryComp<NpcFactionMemberComponent>(args.Equipee, ref factionMemberComponent);
    (EntityUid, NpcFactionMemberComponent) valueTuple = (args.Equipee, factionMemberComponent);
    ent.Comp.AlreadyMember = this._faction.IsMember(Entity<NpcFactionMemberComponent>.op_Implicit(valueTuple), ProtoId<NpcFactionPrototype>.op_Implicit(ent.Comp.Faction));
    this._faction.AddFaction(Entity<NpcFactionMemberComponent>.op_Implicit(valueTuple), ProtoId<NpcFactionPrototype>.op_Implicit(ent.Comp.Faction));
  }

  private void OnUnequipped(Entity<FactionClothingComponent> ent, ref GotUnequippedEvent args)
  {
    if (ent.Comp.AlreadyMember)
      ent.Comp.AlreadyMember = false;
    else
      this._faction.RemoveFaction(Entity<NpcFactionMemberComponent>.op_Implicit(args.Equipee), ProtoId<NpcFactionPrototype>.op_Implicit(ent.Comp.Faction));
  }
}
