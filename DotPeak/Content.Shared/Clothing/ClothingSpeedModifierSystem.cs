// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.ClothingSpeedModifierSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Movement;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Clothing;

public sealed class ClothingSpeedModifierSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private ItemToggleSystem _toggle;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClothingSpeedModifierComponent, ComponentGetState>(new ComponentEventRefHandler<ClothingSpeedModifierComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClothingSpeedModifierComponent, ComponentHandleState>(new ComponentEventRefHandler<ClothingSpeedModifierComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClothingSpeedModifierComponent, InventoryRelayedEvent<RefreshMovementSpeedModifiersEvent>>(new ComponentEventHandler<ClothingSpeedModifierComponent, InventoryRelayedEvent<RefreshMovementSpeedModifiersEvent>>((object) this, __methodptr(OnRefreshMoveSpeed)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClothingSpeedModifierComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<ClothingSpeedModifierComponent, GetVerbsEvent<ExamineVerb>>((object) this, __methodptr(OnClothingVerbExamine)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClothingSpeedModifierComponent, ItemToggledEvent>(new EntityEventRefHandler<ClothingSpeedModifierComponent, ItemToggledEvent>((object) this, __methodptr(OnToggled)), (Type[]) null, (Type[]) null);
  }

  private void OnGetState(
    EntityUid uid,
    ClothingSpeedModifierComponent component,
    ref ComponentGetState args)
  {
    ((ComponentGetState) ref args).State = (IComponentState) new ClothingSpeedModifierComponentState(component.WalkModifier, component.SprintModifier);
  }

  private void OnHandleState(
    EntityUid uid,
    ClothingSpeedModifierComponent component,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is ClothingSpeedModifierComponentState current))
      return;
    int num = !MathHelper.CloseTo(component.SprintModifier, current.SprintModifier, 1E-07f) ? 1 : (!MathHelper.CloseTo(component.WalkModifier, current.WalkModifier, 1E-07f) ? 1 : 0);
    component.WalkModifier = current.WalkModifier;
    component.SprintModifier = current.SprintModifier;
    BaseContainer baseContainer;
    if (num == 0 || !this._container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((uid, (TransformComponent) null, (MetaDataComponent) null)), ref baseContainer))
      return;
    this._movementSpeed.RefreshMovementSpeedModifiers(baseContainer.Owner);
  }

  private void OnRefreshMoveSpeed(
    EntityUid uid,
    ClothingSpeedModifierComponent component,
    InventoryRelayedEvent<RefreshMovementSpeedModifiersEvent> args)
  {
    RMCMovementSpeedRefreshedEvent speedRefreshedEvent = new RMCMovementSpeedRefreshedEvent(component.WalkModifier, component.SprintModifier);
    this.RaiseLocalEvent<RMCMovementSpeedRefreshedEvent>(uid, ref speedRefreshedEvent, false);
    float walkModifier = speedRefreshedEvent.WalkModifier;
    float sprintModifier = speedRefreshedEvent.SprintModifier;
    if (!this._toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(uid)))
      return;
    args.Args.ModifySpeed(walkModifier, sprintModifier);
  }

  private void OnClothingVerbExamine(
    EntityUid uid,
    ClothingSpeedModifierComponent component,
    GetVerbsEvent<ExamineVerb> args)
  {
    if (!args.CanInteract || !args.CanAccess)
      return;
    float x1 = MathF.Round((float) ((1.0 - (double) component.WalkModifier) * 100.0), 1);
    float x2 = MathF.Round((float) ((1.0 - (double) component.SprintModifier) * 100.0), 1);
    if ((double) x1 == 0.0 && (double) x2 == 0.0)
      return;
    FormattedMessage message = new FormattedMessage();
    if (MathHelper.CloseTo(x1, x2, 0.5f))
    {
      if ((double) x1 < 0.0)
        message.AddMarkupOrThrow(this.Loc.GetString("clothing-speed-increase-equal-examine", ("walkSpeed", (object) (int) MathF.Abs(x1)), ("runSpeed", (object) (int) MathF.Abs(x2))));
      else
        message.AddMarkupOrThrow(this.Loc.GetString("clothing-speed-decrease-equal-examine", ("walkSpeed", (object) (int) x1), ("runSpeed", (object) (int) x2)));
    }
    else
    {
      if ((double) x2 < 0.0)
        message.AddMarkupOrThrow(this.Loc.GetString("clothing-speed-increase-run-examine", ("runSpeed", (object) (int) MathF.Abs(x2))));
      else if ((double) x2 > 0.0)
        message.AddMarkupOrThrow(this.Loc.GetString("clothing-speed-decrease-run-examine", ("runSpeed", (object) (int) x2)));
      if ((double) x1 != 0.0 && (double) x2 != 0.0)
        message.PushNewline();
      if ((double) x1 < 0.0)
        message.AddMarkupOrThrow(this.Loc.GetString("clothing-speed-increase-walk-examine", ("walkSpeed", (object) (int) MathF.Abs(x1))));
      else if ((double) x1 > 0.0)
        message.AddMarkupOrThrow(this.Loc.GetString("clothing-speed-decrease-walk-examine", ("walkSpeed", (object) (int) x1)));
    }
    this._examine.AddDetailedExamineVerb(args, (Component) component, message, this.Loc.GetString("clothing-speed-examinable-verb-text"), "/Textures/Interface/VerbIcons/outfit.svg.192dpi.png", this.Loc.GetString("clothing-speed-examinable-verb-message"));
  }

  private void OnToggled(Entity<ClothingSpeedModifierComponent> ent, ref ItemToggledEvent args)
  {
    this._movementSpeed.RefreshMovementSpeedModifiers(Entity<ClothingSpeedModifierComponent>.op_Implicit(ent));
    BaseContainer baseContainer;
    if (!this._container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ent.Owner, (TransformComponent) null, (MetaDataComponent) null)), ref baseContainer))
      return;
    this._movementSpeed.RefreshMovementSpeedModifiers(baseContainer.Owner);
  }
}
