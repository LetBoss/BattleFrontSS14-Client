// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Visor.VisorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Scoping;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Clothing;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Coordinates;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Popups;
using Content.Shared.PowerCell;
using Content.Shared.PowerCell.Components;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Tools;
using Content.Shared.Tools.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Visor;

public sealed class VisorSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedPowerCellSystem _powerCell;
  [Dependency]
  private SharedToolSystem _tool;
  [Dependency]
  private SharedItemSystem _item;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SharedAudioSystem _audio;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<InventoryComponent, ScopedEvent>(new EntityEventRefHandler<InventoryComponent, ScopedEvent>(this.OnInventoryScoped));
    this.SubscribeLocalEvent<CycleableVisorComponent, GetEquipmentVisualsEvent>(new EntityEventRefHandler<CycleableVisorComponent, GetEquipmentVisualsEvent>(this.OnCycleableVisorGetEquipmentVisuals), after: new Type[1]
    {
      typeof (ClothingSystem)
    });
    this.SubscribeLocalEvent<CycleableVisorComponent, GetItemActionsEvent>(new EntityEventRefHandler<CycleableVisorComponent, GetItemActionsEvent>(this.OnCycleableVisorGetItemActions));
    this.SubscribeLocalEvent<CycleableVisorComponent, CycleVisorActionEvent>(new EntityEventRefHandler<CycleableVisorComponent, CycleVisorActionEvent>(this.OnCycleableVisorAction));
    this.SubscribeLocalEvent<CycleableVisorComponent, InteractUsingEvent>(new EntityEventRefHandler<CycleableVisorComponent, InteractUsingEvent>(this.OnCycleableVisorInteractUsing), new Type[1]
    {
      typeof (SharedStorageSystem)
    });
    this.SubscribeLocalEvent<CycleableVisorComponent, InventoryRelayedEvent<ScopedEvent>>(new EntityEventRefHandler<CycleableVisorComponent, InventoryRelayedEvent<ScopedEvent>>(this.OnCycleableVisorScoped));
    this.SubscribeLocalEvent<CycleableVisorComponent, ExaminedEvent>(new EntityEventRefHandler<CycleableVisorComponent, ExaminedEvent>(this.OnCycleableVisorExamined));
    this.SubscribeLocalEvent<CycleableVisorComponent, GotEquippedEvent>(new EntityEventRefHandler<CycleableVisorComponent, GotEquippedEvent>(this.OnCycleableVisorEquipped));
    this.SubscribeLocalEvent<VisorComponent, ActivateVisorAttemptEvent>(new EntityEventRefHandler<VisorComponent, ActivateVisorAttemptEvent>(this.OnVisorAttemptActivate));
    this.SubscribeLocalEvent<VisorComponent, ActivateVisorEvent>(new EntityEventRefHandler<VisorComponent, ActivateVisorEvent>(this.OnVisorActivate));
    this.SubscribeLocalEvent<VisorComponent, DeactivateVisorEvent>(new EntityEventRefHandler<VisorComponent, DeactivateVisorEvent>(this.OnVisorDeactivate));
    this.SubscribeLocalEvent<VisorComponent, PowerCellChangedEvent>(new EntityEventRefHandler<VisorComponent, PowerCellChangedEvent>(this.OnCycleableVisorPowerCellChanged), after: new Type[1]
    {
      typeof (SharedPowerCellSystem)
    });
    this.SubscribeLocalEvent<ToggleVisorComponent, ActivateVisorAttemptEvent>(new EntityEventRefHandler<ToggleVisorComponent, ActivateVisorAttemptEvent>(this.OnToggleVisorAttemptActivate));
    this.SubscribeLocalEvent<ToggleVisorComponent, ActivateVisorEvent>(new EntityEventRefHandler<ToggleVisorComponent, ActivateVisorEvent>(this.OnToggleVisorActivate));
    this.SubscribeLocalEvent<ToggleVisorComponent, DeactivateVisorEvent>(new EntityEventRefHandler<ToggleVisorComponent, DeactivateVisorEvent>(this.OnToggleVisorDeactivate));
    this.SubscribeLocalEvent<IntegratedVisorsComponent, MapInitEvent>(new EntityEventRefHandler<IntegratedVisorsComponent, MapInitEvent>(this.OnIntegratedVisorsInit), after: new Type[1]
    {
      typeof (SharedItemSystem)
    });
  }

  private void OnInventoryScoped(Entity<InventoryComponent> ent, ref ScopedEvent args)
  {
    this._inventory.RelayEvent<ScopedEvent>(ent, ref args);
  }

  private void OnCycleableVisorGetItemActions(
    Entity<CycleableVisorComponent> ent,
    ref GetItemActionsEvent args)
  {
    args.AddAction(ref ent.Comp.Action, (string) ent.Comp.ActionId);
    string id;
    BaseContainer container;
    VisorComponent comp;
    if (!ent.Comp.CurrentVisor.HasValue || !ent.Comp.Containers.TryGetValue<string>(ent.Comp.CurrentVisor.Value, out id) || !this._container.TryGetContainer((EntityUid) ent, id, out container) || !this.TryComp<VisorComponent>(container.ContainedEntities.FirstOrDefault<EntityUid>(), out comp))
      return;
    EntityUid? action = ent.Comp.Action;
    if (!action.HasValue)
      return;
    this._actions.SetIcon((Entity<ActionComponent>) action.GetValueOrDefault(), (SpriteSpecifier) comp.OnIcon);
  }

  private void OnCycleableVisorGetEquipmentVisuals(
    Entity<CycleableVisorComponent> ent,
    ref GetEquipmentVisualsEvent args)
  {
    string id;
    BaseContainer container;
    VisorComponent comp;
    SlotDefinition slotDefinition;
    if (!ent.Comp.CurrentVisor.HasValue || !ent.Comp.Containers.TryGetValue<string>(ent.Comp.CurrentVisor.Value, out id) || !this._container.TryGetContainer((EntityUid) ent, id, out container) || !this.TryComp<VisorComponent>(container.ContainedEntities.FirstOrDefault<EntityUid>(), out comp) || comp.ToggledSprite == null || this._inventory.TryGetSlot(args.Equipee, args.Slot, out slotDefinition) && (slotDefinition.SlotFlags & comp.Slot) == SlotFlags.NONE)
      return;
    string layer = $"enum.{"VisorVisualLayers"}.{VisorVisualLayers.Base}";
    if (args.Layers.Any<(string, PrototypeLayerData)>((Func<(string, PrototypeLayerData), bool>) (l => l.Item1 == layer)))
      return;
    args.Layers.Add((layer, new PrototypeLayerData()
    {
      RsiPath = comp.ToggledSprite.RsiPath.ToString(),
      State = comp.ToggledSprite.RsiState,
      Visible = new bool?(true)
    }));
    EntityUid? action = ent.Comp.Action;
    if (!action.HasValue)
      return;
    this._actions.SetIcon((Entity<ActionComponent>) action.GetValueOrDefault(), (SpriteSpecifier) comp.OnIcon);
  }

  private unsafe void OnCycleableVisorAction(
    Entity<CycleableVisorComponent> ent,
    ref CycleVisorActionEvent args)
  {
    List<ContainerSlot> containerSlotList = new List<ContainerSlot>();
    foreach (string container in ent.Comp.Containers)
      containerSlotList.Add(this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, container));
    if (containerSlotList.Count == 0)
      return;
    if (containerSlotList.All<ContainerSlot>((Func<ContainerSlot, bool>) (c => !c.ContainedEntity.HasValue)))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-no-visors-to-swap"), (EntityUid) ent, new EntityUid?(args.Performer), PopupType.SmallCaution);
    }
    else
    {
      args.Handled = true;
      ref int? local1 = ref ent.Comp.CurrentVisor;
      ContainerSlot containerSlot;
      EntityUid? nullable1;
      if (local1.HasValue && containerSlotList.TryGetValue<ContainerSlot>(local1.Value, out containerSlot))
      {
        nullable1 = containerSlot.ContainedEntity;
        if (nullable1.HasValue)
        {
          EntityUid valueOrDefault = nullable1.GetValueOrDefault();
          DeactivateVisorEvent args1 = new DeactivateVisorEvent(ent, new EntityUid?(args.Performer));
          this.RaiseLocalEvent<DeactivateVisorEvent>(valueOrDefault, ref args1);
        }
      }
      bool flag = !local1.HasValue;
      do
      {
        ref int? local2 = ref local1;
        int? nullable2;
        int? nullable3;
        if (local1.HasValue)
        {
          nullable2 = local1;
          nullable3 = nullable2.HasValue ? new int?(nullable2.GetValueOrDefault() + 1) : new int?();
        }
        else
          nullable3 = new int?(0);
        local2 = nullable3;
        this.Dirty<CycleableVisorComponent>(ent);
        nullable2 = local1;
        int count = containerSlotList.Count;
        if (nullable2.GetValueOrDefault() >= count & nullable2.HasValue)
        {
          *(int?*) ref local1 = new int?();
          break;
        }
        if (local1.HasValue && containerSlotList.TryGetValue<ContainerSlot>(local1.Value, out containerSlot))
        {
          nullable1 = containerSlot.ContainedEntity;
          if (nullable1.HasValue)
          {
            EntityUid valueOrDefault = nullable1.GetValueOrDefault();
            if (this._powerCell.HasDrawCharge(valueOrDefault, user: new EntityUid?(args.Performer)))
            {
              ActivateVisorAttemptEvent args2 = new ActivateVisorAttemptEvent(args.Performer);
              this.RaiseLocalEvent<ActivateVisorAttemptEvent>(valueOrDefault, ref args2);
              if (!args2.Cancelled)
              {
                ActivateVisorEvent args3 = new ActivateVisorEvent(ent, new EntityUid?(args.Performer));
                this.RaiseLocalEvent<ActivateVisorEvent>(valueOrDefault, ref args3);
                if (!args3.Handled)
                  *(int?*) ref local1 = new int?();
                else
                  break;
              }
            }
          }
        }
      }
      while (local1.HasValue);
      if (flag && !local1.HasValue)
        this._popup.PopupClient(this.Loc.GetString("rmc-no-visors-to-swap"), (EntityUid) ent, new EntityUid?(args.Performer), PopupType.SmallCaution);
      nullable1 = ent.Comp.Action;
      if (nullable1.HasValue)
      {
        EntityUid valueOrDefault = nullable1.GetValueOrDefault();
        if (!local1.HasValue)
          this._actions.SetIcon((Entity<ActionComponent>) valueOrDefault, (SpriteSpecifier) ent.Comp.OffIcon);
      }
      this._item.VisualsChanged((EntityUid) ent);
    }
  }

  private unsafe void OnCycleableVisorEquipped(
    Entity<CycleableVisorComponent> ent,
    ref GotEquippedEvent args)
  {
    List<ContainerSlot> list = new List<ContainerSlot>();
    foreach (string container in ent.Comp.Containers)
      list.Add(this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, container));
    if (list.Count == 0)
      return;
    ref int? local = ref ent.Comp.CurrentVisor;
    ContainerSlot containerSlot;
    if (!local.HasValue || !list.TryGetValue<ContainerSlot>(local.Value, out containerSlot))
      return;
    EntityUid? nullable = containerSlot.ContainedEntity;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    ActivateVisorAttemptEvent args1 = new ActivateVisorAttemptEvent(args.Equipee);
    this.RaiseLocalEvent<ActivateVisorAttemptEvent>(valueOrDefault, ref args1);
    if (!args1.Cancelled)
      return;
    this.DeactivateVisor(ent, (Entity<VisorComponent>) valueOrDefault, args.Equipee);
    *(int?*) ref local = new int?();
    this._item.VisualsChanged((EntityUid) ent);
    nullable = ent.Comp.Action;
    if (nullable.HasValue)
      this._actions.SetIcon((Entity<ActionComponent>) nullable.GetValueOrDefault(), (SpriteSpecifier) ent.Comp.OffIcon);
    this._popup.PopupClient(this.Loc.GetString("rmc-skills-no-training", ("target", (object) valueOrDefault)), args.Equipee, new EntityUid?(args.Equipee), PopupType.SmallCaution);
  }

  private void OnCycleableVisorInteractUsing(
    Entity<CycleableVisorComponent> ent,
    ref InteractUsingEvent args)
  {
    VisorComponent comp;
    if (this.TryComp<VisorComponent>(args.Used, out comp))
    {
      if (!this.AttachVisor(ent, (Entity<VisorComponent>) (args.Used, comp), new EntityUid?(args.User)))
        return;
      args.Handled = true;
    }
    else
    {
      foreach (ProtoId<ToolQualityPrototype> quality in ent.Comp.RemoveQuality)
      {
        if (!this._tool.HasQuality(args.Used, (string) quality))
          return;
      }
      args.Handled = true;
      string id;
      BaseContainer container1;
      if (ent.Comp.CurrentVisor.HasValue && ent.Comp.Containers.TryGetValue<string>(ent.Comp.CurrentVisor.Value, out id) && this._container.TryGetContainer((EntityUid) ent, id, out container1))
      {
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container1.ContainedEntities)
        {
          DeactivateVisorEvent args1 = new DeactivateVisorEvent(ent, new EntityUid?(args.User));
          this.RaiseLocalEvent<DeactivateVisorEvent>(containedEntity, ref args1);
        }
      }
      bool flag1 = false;
      foreach (string container2 in ent.Comp.Containers)
      {
        BaseContainer container3;
        if (this._container.TryGetContainer((EntityUid) ent, container2, out container3))
        {
          bool flag2 = true;
          foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container3.ContainedEntities)
          {
            if (this.HasComp<UnremovableVisorComponent>(containedEntity))
              flag2 = false;
          }
          if (flag2 && this._container.EmptyContainer(container3).Count > 0)
            flag1 = true;
        }
      }
      if (flag1)
        this._popup.PopupClient("You remove the inserted visors", args.Target, new EntityUid?(args.User));
      else
        this._popup.PopupClient("There are no visors left to take out!", args.Target, new EntityUid?(args.User));
      ent.Comp.CurrentVisor = new int?();
      this.Dirty<CycleableVisorComponent>(ent);
    }
  }

  private void OnCycleableVisorScoped(
    Entity<CycleableVisorComponent> ent,
    ref InventoryRelayedEvent<ScopedEvent> args)
  {
    VisorRelayedEvent<ScopedEvent> args1 = new VisorRelayedEvent<ScopedEvent>(ent, args.Args);
    foreach (string container1 in ent.Comp.Containers)
    {
      BaseContainer container2;
      if (this._container.TryGetContainer((EntityUid) ent, container1, out container2))
      {
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container2.ContainedEntities)
          this.RaiseLocalEvent<VisorRelayedEvent<ScopedEvent>>(containedEntity, ref args1);
      }
    }
    args.Args = args1.Event;
  }

  private void OnCycleableVisorExamined(Entity<CycleableVisorComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("CycleableVisorComponent"))
    {
      if (ent.Comp.CurrentVisor.HasValue)
      {
        string id;
        BaseContainer container;
        if (!ent.Comp.Containers.TryGetValue<string>(ent.Comp.CurrentVisor.Value, out id) || !this._container.TryGetContainer((EntityUid) ent, id, out container))
          return;
        args.PushMarkup(this.Loc.GetString("rmc-visor-down", ("visor", (object) container.ContainedEntities.FirstOrDefault<EntityUid>())));
      }
      args.PushMarkup("Use a [color=cyan]screwdriver[/color] on this to take out any visors!");
    }
  }

  private void OnVisorAttemptActivate(
    Entity<VisorComponent> ent,
    ref ActivateVisorAttemptEvent args)
  {
    if (ent.Comp.SkillsRequired == null || this._skills.HasSkills((Entity<SkillsComponent>) args.User, ent.Comp.SkillsRequired))
      return;
    args.Cancel();
  }

  private void OnVisorActivate(Entity<VisorComponent> ent, ref ActivateVisorEvent args)
  {
    EntityUid? action = args.CycleableVisor.Comp.Action;
    if (action.HasValue)
      this._actions.SetIcon((Entity<ActionComponent>) action.GetValueOrDefault(), (SpriteSpecifier) ent.Comp.OnIcon);
    if (!this.HasComp<PowerCellSlotComponent>((EntityUid) ent))
      return;
    this._powerCell.SetDrawEnabled((Entity<PowerCellDrawComponent>) ent.Owner, true);
  }

  private void OnVisorDeactivate(Entity<VisorComponent> ent, ref DeactivateVisorEvent args)
  {
    this._powerCell.SetDrawEnabled((Entity<PowerCellDrawComponent>) ent.Owner, false);
  }

  private void OnCycleableVisorPowerCellChanged(
    Entity<VisorComponent> ent,
    ref PowerCellChangedEvent args)
  {
    if (!args.Ejected && this._powerCell.HasDrawCharge((EntityUid) ent))
      return;
    PowerCellDrawComponent comp1;
    if (this.TryComp<PowerCellDrawComponent>((EntityUid) ent, out comp1))
    {
      bool flag1 = !args.Ejected && this._powerCell.HasDrawCharge((EntityUid) ent, comp1);
      bool flag2 = !args.Ejected && this._powerCell.HasDrawCharge((EntityUid) ent, comp1);
      comp1.CanDraw = flag1;
      comp1.CanUse = flag2;
      this.Dirty((EntityUid) ent, (IComponent) comp1);
    }
    BaseContainer container;
    CycleableVisorComponent comp2;
    if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) ent, (TransformComponent) null), out container) || !this.TryComp<CycleableVisorComponent>(container.Owner, out comp2))
      return;
    DeactivateVisorEvent args1 = new DeactivateVisorEvent((Entity<CycleableVisorComponent>) (container.Owner, comp2), new EntityUid?());
    this.RaiseLocalEvent<DeactivateVisorEvent>((EntityUid) ent, ref args1);
    int? currentVisor = comp2.CurrentVisor;
    if (!currentVisor.HasValue)
      return;
    int valueOrDefault = currentVisor.GetValueOrDefault();
    string str;
    if (valueOrDefault < 0 || !comp2.Containers.TryGetValue<string>(valueOrDefault, out str) || !(container.ID == str))
      return;
    comp2.CurrentVisor = new int?();
    this.Dirty(container.Owner, (IComponent) comp2);
  }

  private bool AttachVisor(
    Entity<CycleableVisorComponent> cycleable,
    Entity<VisorComponent> visor,
    EntityUid? user)
  {
    if (!this.HasComp<ItemComponent>((EntityUid) visor))
      return false;
    foreach (string container1 in cycleable.Comp.Containers)
    {
      ContainerSlot container2 = this._container.EnsureContainer<ContainerSlot>((EntityUid) cycleable, container1);
      if (this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) visor.Owner, (BaseContainer) container2))
      {
        this._popup.PopupClient($"You connect the {this.Name((EntityUid) visor)} to {this.Name((EntityUid) cycleable)}.", (EntityUid) cycleable, user);
        return true;
      }
    }
    this._popup.PopupClient(this.Name((EntityUid) cycleable) + " has used all of its visor attachment sockets.", (EntityUid) cycleable, user, PopupType.SmallCaution);
    return true;
  }

  public unsafe void DeactivateVisor(
    Entity<CycleableVisorComponent> cycleable,
    Entity<VisorComponent?> visor,
    EntityUid user)
  {
    ref int? local = ref cycleable.Comp.CurrentVisor;
    if (!local.HasValue)
      return;
    int? nullable1 = local;
    int num = 0;
    if (nullable1.GetValueOrDefault() < num & nullable1.HasValue)
      return;
    int? nullable2 = local;
    int count = cycleable.Comp.Containers.Count;
    if (nullable2.GetValueOrDefault() >= count & nullable2.HasValue)
      return;
    string container1 = cycleable.Comp.Containers[local.Value];
    BaseContainer container2;
    if (!this._container.TryGetContainer((EntityUid) cycleable, container1, out container2))
      return;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container2.ContainedEntities)
    {
      if (containedEntity == visor.Owner)
      {
        DeactivateVisorEvent args = new DeactivateVisorEvent(cycleable, new EntityUid?(user));
        this.RaiseLocalEvent<DeactivateVisorEvent>(containedEntity, ref args);
        *(int?*) ref local = new int?();
        this.Dirty<CycleableVisorComponent>(cycleable);
        break;
      }
    }
  }

  public void OnToggleVisorAttemptActivate(
    Entity<ToggleVisorComponent> visor,
    ref ActivateVisorAttemptEvent args)
  {
    ComponentTogglerComponent comp1;
    VisorComponent comp2;
    if (visor.Comp.IgnoreRedundancy || !this.TryComp<ComponentTogglerComponent>((EntityUid) visor, out comp1) || !this.TryComp<VisorComponent>((EntityUid) visor, out comp2))
      return;
    foreach (Robust.Shared.Prototypes.EntityPrototype.ComponentRegistryEntry componentRegistryEntry in comp1.Components.Values)
    {
      Type type = componentRegistryEntry.Component.GetType();
      if (!this.HasComp(args.User, type))
      {
        InventorySystem.InventorySlotEnumerator containerSlotEnumerator;
        if (!this._inventory.TryGetContainerSlotEnumerator((Entity<InventoryComponent>) args.User, out containerSlotEnumerator))
          return;
        bool flag = false;
        EntityUid uid;
        SlotDefinition slot;
        while (containerSlotEnumerator.NextItem(out uid, out slot))
        {
          if ((slot.SlotFlags & comp2.Slot) == SlotFlags.NONE && this.HasComp(uid, type))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          return;
      }
    }
    args.Cancel();
  }

  public void OnToggleVisorActivate(Entity<ToggleVisorComponent> visor, ref ActivateVisorEvent args)
  {
    ComponentTogglerComponent comp;
    if (!this.TryComp<ComponentTogglerComponent>((EntityUid) visor, out comp))
      return;
    args.Handled = true;
    this.EntityManager.AddComponents((EntityUid) args.CycleableVisor, comp.Components, true);
    if (!args.User.HasValue)
      return;
    this._audio.PlayLocal(visor.Comp.SoundOn, (EntityUid) visor, args.User);
  }

  public void OnToggleVisorDeactivate(
    Entity<ToggleVisorComponent> visor,
    ref DeactivateVisorEvent args)
  {
    ComponentTogglerComponent comp;
    if (!this.TryComp<ComponentTogglerComponent>((EntityUid) visor, out comp))
      return;
    this.EntityManager.RemoveComponents((EntityUid) args.CycleableVisor, comp.RemoveComponents ?? comp.Components);
    if (!args.User.HasValue)
      return;
    this._audio.PlayLocal(visor.Comp.SoundOff, (EntityUid) visor, args.User);
  }

  private unsafe void OnIntegratedVisorsInit(
    Entity<IntegratedVisorsComponent> integrated,
    ref MapInitEvent args)
  {
    CycleableVisorComponent comp1;
    if (!this.TryComp<CycleableVisorComponent>((EntityUid) integrated, out comp1))
      return;
    List<ContainerSlot> list = new List<ContainerSlot>();
    foreach (string container in comp1.Containers)
      list.Add(this._container.EnsureContainer<ContainerSlot>((EntityUid) integrated, container));
    foreach (EntProtoId prototype in integrated.Comp.VisorsToAdd)
    {
      EntityUid uid = this.SpawnAtPosition((string) prototype, integrated.Owner.ToCoordinates());
      VisorComponent comp2;
      if (!this.TryComp<VisorComponent>(uid, out comp2))
        this.QueueDel(new EntityUid?(uid));
      else if (!this.AttachVisor((Entity<CycleableVisorComponent>) (integrated.Owner, comp1), (Entity<VisorComponent>) (uid, comp2), new EntityUid?()))
        this.QueueDel(new EntityUid?(uid));
    }
    ref int? local1 = ref comp1.CurrentVisor;
    if (!integrated.Comp.StartToggled || list.Count <= 0)
      return;
    ref int? local2 = ref local1;
    int? nullable1;
    if (local1.HasValue)
    {
      int? nullable2 = local1;
      nullable1 = nullable2.HasValue ? new int?(nullable2.GetValueOrDefault() + 1) : new int?();
    }
    else
      nullable1 = new int?(0);
    local2 = nullable1;
    this.Dirty(integrated.Owner, (IComponent) comp1);
    int? nullable3 = local1;
    int count = list.Count;
    if (nullable3.GetValueOrDefault() >= count & nullable3.HasValue)
      *(int?*) ref local1 = new int?();
    ContainerSlot containerSlot;
    if (local1.HasValue && list.TryGetValue<ContainerSlot>(local1.Value, out containerSlot))
    {
      EntityUid? containedEntity = containerSlot.ContainedEntity;
      if (containedEntity.HasValue)
      {
        EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
        if (!this._powerCell.HasDrawCharge(valueOrDefault))
        {
          *(int?*) ref local1 = new int?();
          return;
        }
        ActivateVisorEvent args1 = new ActivateVisorEvent((Entity<CycleableVisorComponent>) (integrated.Owner, comp1), new EntityUid?());
        this.RaiseLocalEvent<ActivateVisorEvent>(valueOrDefault, ref args1);
        if (!args1.Handled)
          *(int?*) ref local1 = new int?();
      }
    }
    this._item.VisualsChanged(integrated.Owner);
  }
}
