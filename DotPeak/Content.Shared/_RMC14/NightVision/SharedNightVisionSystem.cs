// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.NightVision.SharedNightVisionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Scoping;
using Content.Shared._RMC14.Visor;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Alert;
using Content.Shared.IgnitionSource;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Content.Shared.Toggleable;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.NightVision;

public abstract class SharedNightVisionSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private VisorSystem _visor;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<NightVisionComponent, ComponentStartup>(new EntityEventRefHandler<NightVisionComponent, ComponentStartup>(this.OnNightVisionStartup));
    this.SubscribeLocalEvent<NightVisionComponent, MapInitEvent>(new EntityEventRefHandler<NightVisionComponent, MapInitEvent>(this.OnNightVisionMapInit));
    this.SubscribeLocalEvent<NightVisionComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<NightVisionComponent, AfterAutoHandleStateEvent>(this.OnNightVisionAfterHandle));
    this.SubscribeLocalEvent<NightVisionComponent, ComponentRemove>(new EntityEventRefHandler<NightVisionComponent, ComponentRemove>(this.OnNightVisionRemove));
    this.SubscribeLocalEvent<NightVisionComponent, ToggleNightVisionAlertEvent>(new EntityEventRefHandler<NightVisionComponent, ToggleNightVisionAlertEvent>(this.OnNightVisionToggle));
    this.SubscribeLocalEvent<NightVisionItemComponent, GetItemActionsEvent>(new EntityEventRefHandler<NightVisionItemComponent, GetItemActionsEvent>(this.OnNightVisionItemGetActions));
    this.SubscribeLocalEvent<NightVisionItemComponent, ToggleActionEvent>(new EntityEventRefHandler<NightVisionItemComponent, ToggleActionEvent>(this.OnNightVisionItemToggle));
    this.SubscribeLocalEvent<NightVisionItemComponent, GotEquippedEvent>(new EntityEventRefHandler<NightVisionItemComponent, GotEquippedEvent>(this.OnNightVisionItemGotEquipped));
    this.SubscribeLocalEvent<NightVisionItemComponent, GotUnequippedEvent>(new EntityEventRefHandler<NightVisionItemComponent, GotUnequippedEvent>(this.OnNightVisionItemGotUnequipped));
    this.SubscribeLocalEvent<NightVisionItemComponent, ActionRemovedEvent>(new EntityEventRefHandler<NightVisionItemComponent, ActionRemovedEvent>(this.OnNightVisionItemActionRemoved));
    this.SubscribeLocalEvent<NightVisionItemComponent, ComponentRemove>(new EntityEventRefHandler<NightVisionItemComponent, ComponentRemove>(this.OnNightVisionItemRemove));
    this.SubscribeLocalEvent<NightVisionItemComponent, EntityTerminatingEvent>(new EntityEventRefHandler<NightVisionItemComponent, EntityTerminatingEvent>(this.OnNightVisionItemTerminating));
    this.SubscribeLocalEvent<NightVisionVisorComponent, ActivateVisorEvent>(new EntityEventRefHandler<NightVisionVisorComponent, ActivateVisorEvent>(this.OnNightVisionActivate));
    this.SubscribeLocalEvent<NightVisionVisorComponent, DeactivateVisorEvent>(new EntityEventRefHandler<NightVisionVisorComponent, DeactivateVisorEvent>(this.OnNightVisionDeactivate));
    this.SubscribeLocalEvent<NightVisionVisorComponent, VisorRelayedEvent<ScopedEvent>>(new EntityEventRefHandler<NightVisionVisorComponent, VisorRelayedEvent<ScopedEvent>>(this.OnNightVisionScoped));
    this.SubscribeLocalEvent<RMCNightVisionVisibleOnIgniteComponent, IgnitionEvent>(new EntityEventRefHandler<RMCNightVisionVisibleOnIgniteComponent, IgnitionEvent>(this.OnNightVisionVisibleIgnition));
  }

  private void OnNightVisionStartup(Entity<NightVisionComponent> ent, ref ComponentStartup args)
  {
    this.NightVisionChanged(ent);
  }

  private void OnNightVisionAfterHandle(
    Entity<NightVisionComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this.NightVisionChanged(ent);
  }

  private void OnNightVisionMapInit(Entity<NightVisionComponent> ent, ref MapInitEvent args)
  {
    this.UpdateAlert(ent);
  }

  private void OnNightVisionRemove(Entity<NightVisionComponent> ent, ref ComponentRemove args)
  {
    ProtoId<AlertPrototype>? alert = ent.Comp.Alert;
    if (alert.HasValue)
    {
      ProtoId<AlertPrototype> valueOrDefault = alert.GetValueOrDefault();
      this._alerts.ClearAlert((EntityUid) ent, valueOrDefault);
    }
    this.NightVisionRemoved(ent);
  }

  private void OnNightVisionToggle(
    Entity<NightVisionComponent> ent,
    ref ToggleNightVisionAlertEvent args)
  {
    int num = (int) this.Toggle((Entity<NightVisionComponent>) ((EntityUid) ent, (NightVisionComponent) ent));
  }

  private void OnNightVisionItemGetActions(
    Entity<NightVisionItemComponent> ent,
    ref GetItemActionsEvent args)
  {
    if (!ent.Comp.ActionId.HasValue || args.InHands || !ent.Comp.Toggleable)
      return;
    int slotFlags1 = (int) ent.Comp.SlotFlags;
    SlotFlags? slotFlags2 = args.SlotFlags;
    int valueOrDefault1 = (int) slotFlags2.GetValueOrDefault();
    if (!(slotFlags1 == valueOrDefault1 & slotFlags2.HasValue))
      return;
    GetItemActionsEvent itemActionsEvent = args;
    ref EntityUid? local = ref ent.Comp.Action;
    EntProtoId? actionId = ent.Comp.ActionId;
    string valueOrDefault2 = actionId.HasValue ? (string) actionId.GetValueOrDefault() : (string) null;
    itemActionsEvent.AddAction(ref local, valueOrDefault2);
  }

  private void OnNightVisionItemToggle(
    Entity<NightVisionItemComponent> ent,
    ref ToggleActionEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    this.ToggleNightVisionItem(ent, args.Performer);
  }

  private void OnNightVisionItemGotEquipped(
    Entity<NightVisionItemComponent> ent,
    ref GotEquippedEvent args)
  {
    if (ent.Comp.SlotFlags != args.SlotFlags)
      return;
    this.EnableNightVisionItem(ent, args.Equipee);
  }

  private void OnNightVisionItemGotUnequipped(
    Entity<NightVisionItemComponent> ent,
    ref GotUnequippedEvent args)
  {
    if (ent.Comp.SlotFlags != args.SlotFlags)
      return;
    this.DisableNightVisionItem(ent, new EntityUid?(args.Equipee));
  }

  private void OnNightVisionItemActionRemoved(
    Entity<NightVisionItemComponent> ent,
    ref ActionRemovedEvent args)
  {
    this.DisableNightVisionItem(ent, ent.Comp.User);
  }

  private void OnNightVisionItemRemove(
    Entity<NightVisionItemComponent> ent,
    ref ComponentRemove args)
  {
    this.DisableNightVisionItem(ent, ent.Comp.User);
  }

  private void OnNightVisionItemTerminating(
    Entity<NightVisionItemComponent> ent,
    ref EntityTerminatingEvent args)
  {
    this.DisableNightVisionItem(ent, ent.Comp.User);
  }

  private void OnNightVisionActivate(
    Entity<NightVisionVisorComponent> ent,
    ref ActivateVisorEvent args)
  {
    if (args.User.HasValue && this.HasComp<ScopingComponent>(args.User))
    {
      this._popup.PopupClient("You cannot use the night vision optic while using optics.", args.User.Value, args.User, PopupType.SmallCaution);
    }
    else
    {
      args.Handled = true;
      if (this._timing.ApplyingState)
        return;
      NightVisionItemComponent component = new NightVisionItemComponent()
      {
        ActionId = new EntProtoId?(),
        SlotFlags = SlotFlags.HEAD,
        Green = true,
        BlockScopes = true
      };
      this.AddComp<NightVisionItemComponent>((EntityUid) args.CycleableVisor, component, true);
      this.Dirty((EntityUid) args.CycleableVisor, (IComponent) component);
      if (!this._inventory.InSlotWithFlags((Entity<TransformComponent, MetaDataComponent>) args.CycleableVisor.Owner, component.SlotFlags) || !args.User.HasValue)
        return;
      this.EnableNightVisionItem((Entity<NightVisionItemComponent>) ((EntityUid) args.CycleableVisor, component), args.User.Value);
      this._audio.PlayLocal(ent.Comp.SoundOn, (EntityUid) ent, args.User);
    }
  }

  private void OnNightVisionDeactivate(
    Entity<NightVisionVisorComponent> ent,
    ref DeactivateVisorEvent args)
  {
    if (this._timing.ApplyingState || this.TerminatingOrDeleted((EntityUid) ent))
      return;
    this.RemComp<NightVisionItemComponent>((EntityUid) args.CycleableVisor);
    this._audio.PlayLocal(ent.Comp.SoundOff, (EntityUid) ent, args.User);
  }

  private void OnNightVisionScoped(
    Entity<NightVisionVisorComponent> ent,
    ref VisorRelayedEvent<ScopedEvent> args)
  {
    this._visor.DeactivateVisor(args.CycleableVisor, (Entity<VisorComponent>) ent.Owner, args.Event.User);
  }

  private void OnNightVisionVisibleIgnition(
    Entity<RMCNightVisionVisibleOnIgniteComponent> ent,
    ref IgnitionEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    if (args.Ignite)
      this.EnsureComp<RMCNightVisionVisibleComponent>((EntityUid) ent);
    else
      this.RemCompDeferred<RMCNightVisionVisibleComponent>((EntityUid) ent);
  }

  public NightVisionState Toggle(Entity<NightVisionComponent?> ent)
  {
    if (!this.Resolve<NightVisionComponent>((EntityUid) ent, ref ent.Comp))
      return NightVisionState.Off;
    NightVisionComponent comp = ent.Comp;
    NightVisionState nightVisionState;
    switch (ent.Comp.State)
    {
      case NightVisionState.Off:
        nightVisionState = NightVisionState.Half;
        break;
      case NightVisionState.Half:
        nightVisionState = ent.Comp.OnlyHalf ? NightVisionState.Off : NightVisionState.Full;
        break;
      case NightVisionState.Full:
        nightVisionState = NightVisionState.Off;
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    comp.State = nightVisionState;
    this.Dirty<NightVisionComponent>(ent);
    this.UpdateAlert((Entity<NightVisionComponent>) ((EntityUid) ent, ent.Comp));
    return ent.Comp.State;
  }

  private void UpdateAlert(Entity<NightVisionComponent> ent)
  {
    ProtoId<AlertPrototype>? alert = ent.Comp.Alert;
    if (alert.HasValue)
    {
      ProtoId<AlertPrototype> valueOrDefault = alert.GetValueOrDefault();
      short state = (short) ent.Comp.State;
      short maxSeverity = this._alerts.GetMaxSeverity(valueOrDefault);
      short minSeverity = this._alerts.GetMinSeverity(valueOrDefault);
      short num = (int) state > (int) maxSeverity ? maxSeverity : ((int) state < (int) minSeverity ? minSeverity : state);
      this._alerts.ShowAlert((EntityUid) ent, valueOrDefault, new short?(num));
    }
    this.NightVisionChanged(ent);
  }

  private void ToggleNightVisionItem(Entity<NightVisionItemComponent> item, EntityUid user)
  {
    EntityUid? user1 = item.Comp.User;
    EntityUid entityUid = user;
    if ((user1.HasValue ? (user1.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0 && item.Comp.Toggleable)
    {
      this.DisableNightVisionItem(item, item.Comp.User);
      this._audio.PlayLocal(item.Comp.SoundOff, item.Owner, new EntityUid?(user));
    }
    else
    {
      this.EnableNightVisionItem(item, user);
      this._audio.PlayLocal(item.Comp.SoundOn, item.Owner, new EntityUid?(user));
    }
  }

  private void EnableNightVisionItem(Entity<NightVisionItemComponent> item, EntityUid user)
  {
    this.DisableNightVisionItem(item, item.Comp.User);
    if (item.Comp.Skills != null && !this._skills.HasAllSkills((Entity<SkillsComponent>) user, item.Comp.Skills))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-skills-hud-toggle"), user, new EntityUid?(user), PopupType.MediumCaution);
    }
    else
    {
      item.Comp.User = new EntityUid?(user);
      this.Dirty<NightVisionItemComponent>(item);
      this._appearance.SetData((EntityUid) item, (Enum) NightVisionItemVisuals.Active, (object) true);
      if (!this._timing.ApplyingState)
      {
        NightVisionComponent comp;
        if (this.TryComp<NightVisionComponent>(user, out comp))
        {
          comp = this.EnsureComp<NightVisionComponent>(user);
          comp.State = NightVisionState.Full;
          comp.Green = item.Comp.Green;
          comp.Mesons = item.Comp.Mesons;
          comp.BlockScopes = item.Comp.BlockScopes;
          this.Dirty(user, (IComponent) comp);
        }
        else
        {
          comp = new NightVisionComponent()
          {
            State = NightVisionState.Full,
            Green = item.Comp.Green,
            Mesons = item.Comp.Mesons,
            BlockScopes = item.Comp.BlockScopes
          };
          this.AddComp<NightVisionComponent>(user, comp, true);
          this.Dirty(user, (IComponent) comp);
        }
      }
      SharedActionsSystem actions = this._actions;
      EntityUid? action1 = item.Comp.Action;
      Entity<ActionComponent>? action2 = action1.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) action1.GetValueOrDefault()) : new Entity<ActionComponent>?();
      actions.SetToggled(action2, true);
    }
  }

  protected virtual void NightVisionChanged(Entity<NightVisionComponent> ent)
  {
  }

  protected virtual void NightVisionRemoved(Entity<NightVisionComponent> ent)
  {
  }

  public void DisableNightVisionItem(Entity<NightVisionItemComponent> item, EntityUid? user)
  {
    SharedActionsSystem actions = this._actions;
    EntityUid? action1 = item.Comp.Action;
    Entity<ActionComponent>? action2 = action1.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) action1.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actions.SetToggled(action2, false);
    item.Comp.User = new EntityUid?();
    this.Dirty<NightVisionItemComponent>(item);
    this._appearance.SetData((EntityUid) item, (Enum) NightVisionItemVisuals.Active, (object) false);
    NightVisionComponent comp;
    if (!this.TryComp<NightVisionComponent>(user, out comp) || comp.Innate)
      return;
    this.RemCompDeferred<NightVisionComponent>(user.Value);
  }

  public void SetSeeThroughContainers(Entity<NightVisionComponent?> ent, bool see)
  {
    if (!this.Resolve<NightVisionComponent>((EntityUid) ent, ref ent.Comp, false))
      return;
    ent.Comp.SeeThroughContainers = see;
    this.Dirty<NightVisionComponent>(ent);
  }
}
