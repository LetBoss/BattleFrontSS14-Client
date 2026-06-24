// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Tools.SharedRMCHandLabelerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Interaction;
using Content.Shared.Labels.Components;
using Content.Shared.Labels.EntitySystems;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared._RMC14.Tools;

public abstract class SharedRMCHandLabelerSystem : EntitySystem
{
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private LabelSystem _labelSystem;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private TagSystem _tag;
  private const string PillCanisterTag = "PillCanister";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCHandLabelerComponent, BeforeRangedInteractEvent>(new EntityEventRefHandler<RMCHandLabelerComponent, BeforeRangedInteractEvent>(this.OnBeforeRangedInteract));
    this.SubscribeLocalEvent<RMCHandLabelerComponent, AfterInteractEvent>(new EntityEventRefHandler<RMCHandLabelerComponent, AfterInteractEvent>(this.OnAfterInteract), new Type[1]
    {
      typeof (SharedHandLabelerSystem)
    });
  }

  private void OnBeforeRangedInteract(
    Entity<RMCHandLabelerComponent> ent,
    ref BeforeRangedInteractEvent args)
  {
    if (args.Handled || !args.CanReach)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    target = args.Target;
    EntityUid entityUid = target.Value;
    HandLabelerComponent comp1;
    if (!this._tag.HasTag(entityUid, (ProtoId<TagPrototype>) "PillCanister") || !this.TryComp<HandLabelerComponent>((EntityUid) ent, out comp1))
      return;
    this.OnPillBottleInteract(ent, entityUid, args.User);
    string assignedLabel = comp1.AssignedLabel;
    if (!string.IsNullOrEmpty(assignedLabel))
    {
      if (this._whitelist.IsWhitelistFail(comp1.Whitelist, entityUid))
      {
        args.Handled = true;
        return;
      }
      this.ApplyLabel(entityUid, assignedLabel);
      this._audio.PlayPredicted(ent.Comp.LabelSound, (EntityUid) ent, new EntityUid?(args.User));
    }
    else
    {
      LabelComponent comp2;
      if (this.TryComp<LabelComponent>(entityUid, out comp2) && !string.IsNullOrEmpty(comp2.CurrentLabel))
      {
        this.ApplyLabel(entityUid, (string) null);
        this._audio.PlayPredicted(ent.Comp.RemoveLabelSound, (EntityUid) ent, new EntityUid?(args.User));
      }
    }
    args.Handled = true;
  }

  private void OnAfterInteract(Entity<RMCHandLabelerComponent> ent, ref AfterInteractEvent args)
  {
    if (args.Handled || !args.CanReach)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    target = args.Target;
    EntityUid uid = target.Value;
    HandLabelerComponent comp1;
    if (!this.TryComp<HandLabelerComponent>((EntityUid) ent, out comp1))
      return;
    if (string.IsNullOrEmpty(comp1.AssignedLabel))
    {
      LabelComponent comp2;
      if (!this.TryComp<LabelComponent>(uid, out comp2) || string.IsNullOrEmpty(comp2.CurrentLabel))
        return;
      this._audio.PlayPredicted(ent.Comp.RemoveLabelSound, (EntityUid) ent, new EntityUid?(args.User));
    }
    else
    {
      if (this._whitelist.IsWhitelistFail(comp1.Whitelist, uid))
        return;
      this._audio.PlayPredicted(ent.Comp.LabelSound, (EntityUid) ent, new EntityUid?(args.User));
    }
  }

  protected virtual void OnPillBottleInteract(
    Entity<RMCHandLabelerComponent> labeler,
    EntityUid pillBottle,
    EntityUid user)
  {
  }

  private void ApplyLabel(EntityUid target, string? labelText)
  {
    if (!this._net.IsServer)
      return;
    this._labelSystem.Label(target, labelText);
  }
}
