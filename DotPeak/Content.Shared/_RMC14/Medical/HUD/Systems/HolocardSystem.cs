// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.HUD.Systems.HolocardSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Medical.HUD.Components;
using Content.Shared._RMC14.Medical.HUD.Events;
using Content.Shared._RMC14.Medical.Scanner;
using Content.Shared._RMC14.Overwatch;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared._RMC14.Medical.HUD.Systems;

public sealed class HolocardSystem : EntitySystem
{
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  public const int MinimumRequiredSkill = 2;
  public static readonly EntProtoId<SkillDefinitionComponent> SkillType = (EntProtoId<SkillDefinitionComponent>) "RMCSkillMedical";

  public override void Initialize()
  {
    this.SubscribeLocalEvent<HolocardStateComponent, HolocardChangeEvent>(new EntityEventRefHandler<HolocardStateComponent, HolocardChangeEvent>(this.ChangeHolocard));
    this.SubscribeLocalEvent<HolocardStateComponent, GetVerbsEvent<ExamineVerb>>(new EntityEventRefHandler<HolocardStateComponent, GetVerbsEvent<ExamineVerb>>(this.OnHolocardExaminableVerb));
    this.SubscribeLocalEvent<HealthScannerComponent, OpenChangeHolocardUIEvent>(new ComponentEventRefHandler<HealthScannerComponent, OpenChangeHolocardUIEvent>(this.OpenChangeHolocardUI));
    this.SubscribeLocalEvent<HealthScannerComponent, RefreshEquipmentHudEvent<HealthScannerComponent>>(new EntityEventRefHandler<HealthScannerComponent, RefreshEquipmentHudEvent<HealthScannerComponent>>(this.OnRefreshEquipmentHud));
    this.SubscribeLocalEvent<HolocardContainerComponent, HolocardContainerStatusUpdateEvent>(new EntityEventRefHandler<HolocardContainerComponent, HolocardContainerStatusUpdateEvent>(this.OnHolocardContainerStatusUpdate));
    this.SubscribeLocalEvent<HolocardContainerComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<HolocardContainerComponent, EntInsertedIntoContainerMessage>(this.OnHolocardContainerEntInserted));
    this.SubscribeLocalEvent<HolocardContainerComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<HolocardContainerComponent, EntRemovedFromContainerMessage>(this.OnHolocardContainerEntRemoved));
  }

  private void ChangeHolocard(Entity<HolocardStateComponent> ent, ref HolocardChangeEvent args)
  {
    EntityUid? entity;
    if (!(args.UiKey is HolocardChangeUIKey.Key) || !this.TryGetEntity(args.Owner, out entity) || !this._transform.InRange((Entity<TransformComponent>) ent.Owner, (Entity<TransformComponent>) entity.Value, 15f) && !this.HasComp<OverwatchWatchingComponent>(entity.Value) || !this._skills.HasSkill((Entity<SkillsComponent>) entity.Value, HolocardSystem.SkillType, 2))
      return;
    ent.Comp.HolocardStatus = args.NewHolocardStatus;
    BaseContainer container;
    if (this._container.TryGetOuterContainer((EntityUid) ent, this.Transform((EntityUid) ent), out container))
    {
      HolocardContainerStatusUpdateEvent args1 = new HolocardContainerStatusUpdateEvent(args.NewHolocardStatus);
      this.RaiseLocalEvent<HolocardContainerStatusUpdateEvent>(container.Owner, ref args1);
    }
    this.Dirty<HolocardStateComponent>(ent);
  }

  private void OnHolocardExaminableVerb(
    Entity<HolocardStateComponent> entity,
    ref GetVerbsEvent<ExamineVerb> args)
  {
    if (!args.CanInteract || !this._skills.HasSkill((Entity<SkillsComponent>) args.User, HolocardSystem.SkillType, 2))
      return;
    HolocardScanEvent args1 = new HolocardScanEvent(false, SlotFlags.HEAD | SlotFlags.EYES);
    this.RaiseLocalEvent<HolocardScanEvent>(args.User, ref args1);
    if (!args1.CanScan)
      return;
    EntityUid target = args.Target;
    EntityUid user = args.User;
    ExamineVerb examineVerb1 = new ExamineVerb();
    examineVerb1.Act = (Action) (() => this._ui.OpenUi((Entity<UserInterfaceComponent>) target, (Enum) HolocardChangeUIKey.Key, new EntityUid?(user)));
    examineVerb1.Text = this.Loc.GetString("scannable-holocard-verb-text");
    examineVerb1.Message = this.Loc.GetString("scannable-holocard-verb-message");
    examineVerb1.Category = VerbCategory.Examine;
    examineVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/_RMC14/Interface/VerbIcons/ambulance.png"));
    ExamineVerb examineVerb2 = examineVerb1;
    args.Verbs.Add(examineVerb2);
  }

  private void OpenChangeHolocardUI(
    EntityUid entity,
    HealthScannerComponent comp,
    ref OpenChangeHolocardUIEvent args)
  {
    EntityUid entity1 = this.GetEntity(args.Owner);
    this._ui.OpenUi((Entity<UserInterfaceComponent>) this.GetEntity(args.Target), (Enum) HolocardChangeUIKey.Key, new EntityUid?(entity1));
  }

  private void OnRefreshEquipmentHud(
    Entity<HealthScannerComponent> ent,
    ref RefreshEquipmentHudEvent<HealthScannerComponent> args)
  {
    args.Active = true;
  }

  private void OnHolocardContainerStatusUpdate(
    Entity<HolocardContainerComponent> container,
    ref HolocardContainerStatusUpdateEvent args)
  {
    this._appearance.SetData((EntityUid) container, (Enum) HolocardContainerVisuals.State, (object) args.NewStatus);
  }

  private void OnHolocardContainerEntInserted(
    Entity<HolocardContainerComponent> container,
    ref EntInsertedIntoContainerMessage args)
  {
    HolocardStatus holocardStatus = HolocardStatus.None;
    HolocardStateComponent comp;
    if (this.TryComp<HolocardStateComponent>(args.Entity, out comp))
      holocardStatus = comp.HolocardStatus;
    this._appearance.SetData((EntityUid) container, (Enum) HolocardContainerVisuals.State, (object) holocardStatus);
  }

  private void OnHolocardContainerEntRemoved(
    Entity<HolocardContainerComponent> container,
    ref EntRemovedFromContainerMessage args)
  {
    this._appearance.SetData((EntityUid) container, (Enum) HolocardContainerVisuals.State, (object) HolocardStatus.None);
  }
}
