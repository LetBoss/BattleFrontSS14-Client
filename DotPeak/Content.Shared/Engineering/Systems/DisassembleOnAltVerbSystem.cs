// Decompiled with JetBrains decompiler
// Type: Content.Shared.Engineering.Systems.DisassembleOnAltVerbSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Content.Shared.Engineering.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared.Engineering.Systems;

public sealed class DisassembleOnAltVerbSystem : EntitySystem
{
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _handsSystem;
  [Dependency]
  private INetManager _net;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<DisassembleOnAltVerbComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<DisassembleOnAltVerbComponent, GetVerbsEvent<AlternativeVerb>>(this.AddDisassembleVerb));
    this.SubscribeLocalEvent<DisassembleOnAltVerbComponent, DisassembleDoAfterEvent>(new EntityEventRefHandler<DisassembleOnAltVerbComponent, DisassembleDoAfterEvent>(this.OnDisassembleDoAfter));
  }

  private void AddDisassembleVerb(
    Entity<DisassembleOnAltVerbComponent> entity,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanInteract || !args.CanAccess || args.Hands == null)
      return;
    DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager) this.EntityManager, args.User, entity.Comp.DisassembleTime, (DoAfterEvent) new DisassembleDoAfterEvent(), new EntityUid?((EntityUid) entity), new EntityUid?((EntityUid) entity))
    {
      BreakOnMove = true
    };
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Act = (Action) (() => this._doAfter.TryStartDoAfter(doAfterArgs));
    alternativeVerb1.Text = this.Loc.GetString("disassemble-system-verb-disassemble");
    alternativeVerb1.Priority = 2;
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    args.Verbs.Add(alternativeVerb2);
  }

  private void OnDisassembleDoAfter(
    Entity<DisassembleOnAltVerbComponent> entity,
    ref DisassembleDoAfterEvent args)
  {
    if (!this._net.IsServer || args.Cancelled)
      return;
    EntProtoId? prototypeToSpawn = entity.Comp.PrototypeToSpawn;
    EntityUid? uid;
    if (this.TrySpawnNextTo(prototypeToSpawn.HasValue ? (string) prototypeToSpawn.GetValueOrDefault() : (string) null, entity.Owner, out uid))
      this._handsSystem.TryPickup(args.User, uid.Value);
    this.QueueDel(new EntityUid?(entity.Owner));
  }
}
