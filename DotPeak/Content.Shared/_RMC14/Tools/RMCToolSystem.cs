// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Tools.RMCToolSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Storage;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Tools;

public sealed class RMCToolSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private SharedStackSystem _stack;
  [Dependency]
  private SharedToolSystem _tool;
  [Dependency]
  private SkillsSystem _skills;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCRefinableComponent, ExaminedEvent>(new EntityEventRefHandler<RMCRefinableComponent, ExaminedEvent>(this.OnRefinableExamined));
    this.SubscribeLocalEvent<RMCRefinableComponent, InteractUsingEvent>(new EntityEventRefHandler<RMCRefinableComponent, InteractUsingEvent>(this.OnRefinableInteractUsing));
    this.SubscribeLocalEvent<RMCRefinableComponent, RMCRefinableDoAfterEvent>(new EntityEventRefHandler<RMCRefinableComponent, RMCRefinableDoAfterEvent>(this.OnRefinableDoAfter));
    this.SubscribeLocalEvent<ToolComponent, RMCToolUseEvent>(new EntityEventRefHandler<ToolComponent, RMCToolUseEvent>(this.OnToolUse));
  }

  private void OnRefinableExamined(Entity<RMCRefinableComponent> ent, ref ExaminedEvent args)
  {
    ToolQualityPrototype prototype;
    if (!this._prototypes.TryIndex<ToolQualityPrototype>(ent.Comp.Tool, out prototype))
      return;
    string str = this.Loc.GetString(prototype.ToolName);
    using (args.PushGroup("RMCRefinableComponent"))
      args.PushMarkup(this.Loc.GetString("rmc-refinable-can-be-refined", ("tool", (object) str)));
  }

  private void OnRefinableInteractUsing(
    Entity<RMCRefinableComponent> ent,
    ref InteractUsingEvent args)
  {
    if (args.Handled || !this.HasComp<ToolComponent>(args.Used))
      return;
    args.Handled = true;
    if (ent.Comp.Amount > this._stack.GetCount((EntityUid) ent))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-refinable-not-enough", ("amount", (object) ent.Comp.Amount), ("name", (object) this.Name((EntityUid) ent))), (EntityUid) ent, new EntityUid?(args.User));
    }
    else
    {
      RMCRefinableDoAfterEvent doAfterEv = new RMCRefinableDoAfterEvent();
      float totalSeconds = (float) ent.Comp.Delay.TotalSeconds;
      this._tool.UseTool(args.Used, args.User, new EntityUid?((EntityUid) ent), totalSeconds, (string) ent.Comp.Tool, (DoAfterEvent) doAfterEv, ent.Comp.Fuel);
    }
  }

  private void OnRefinableDoAfter(
    Entity<RMCRefinableComponent> ent,
    ref RMCRefinableDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    if (this.EntityManager.IsQueuedForDeletion((EntityUid) ent))
      return;
    if (this.HasComp<StackComponent>((EntityUid) ent))
    {
      if (!this._stack.Use((EntityUid) ent, ent.Comp.Amount))
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-refinable-not-enough", ("amount", (object) ent.Comp.Amount), ("name", (object) this.Name((EntityUid) ent))), (EntityUid) ent, new EntityUid?(args.User));
        return;
      }
    }
    else
    {
      if (this._net.IsClient)
        return;
      this.QueueDel(new EntityUid?((EntityUid) ent));
    }
    if (this._net.IsClient)
      return;
    foreach (string spawn in EntitySpawnCollection.GetSpawns((IEnumerable<EntitySpawnEntry>) ent.Comp.Spawn))
      this.SpawnAtPosition(spawn, ent.Owner.ToCoordinates());
  }

  private void OnToolUse(Entity<ToolComponent> ent, ref RMCToolUseEvent args)
  {
    if (!this.TryComp<SkillsComponent>(args.User, out SkillsComponent _) || args.Handled)
      return;
    args.Delay *= (double) this._skills.GetSkillDelayMultiplier((Entity<SkillsComponent>) args.User, ent.Comp.Skill);
    args.Handled = true;
  }
}
