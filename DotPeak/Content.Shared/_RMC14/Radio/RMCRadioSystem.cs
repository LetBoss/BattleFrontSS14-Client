// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Radio.RMCRadioSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Squads;
using Content.Shared.Inventory.Events;
using Content.Shared.Radio;
using Content.Shared.Radio.Components;
using Content.Shared.Radio.EntitySystems;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Radio;

public sealed class RMCRadioSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private EncryptionKeySystem _encryptionKey;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  private readonly HashSet<Entity<RMCHeadsetComponent, EncryptionKeyHolderComponent>> _toUpdate = new HashSet<Entity<RMCHeadsetComponent, EncryptionKeyHolderComponent>>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCHeadsetComponent, EncryptionChannelsChangedEvent>(new EntityEventRefHandler<RMCHeadsetComponent, EncryptionChannelsChangedEvent>(this.OnHeadsetEncryptionChannelsChanged), new Type[1]
    {
      typeof (SharedHeadsetSystem)
    });
    this.SubscribeLocalEvent<RMCRadioFilterComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<RMCRadioFilterComponent, GetVerbsEvent<AlternativeVerb>>(this.OnGetAltVerbs));
    this.SubscribeLocalEvent<HeadsetAutoSquadComponent, MapInitEvent>(new EntityEventRefHandler<HeadsetAutoSquadComponent, MapInitEvent>(this.OnHeadsetAutoSquadRefresh<MapInitEvent>));
    this.SubscribeLocalEvent<HeadsetAutoSquadComponent, GotEquippedEvent>(new EntityEventRefHandler<HeadsetAutoSquadComponent, GotEquippedEvent>(this.OnHeadsetAutoSquadRefresh<GotEquippedEvent>));
    this.SubscribeLocalEvent<HeadsetAutoSquadComponent, EncryptionChannelsChangedEvent>(new EntityEventRefHandler<HeadsetAutoSquadComponent, EncryptionChannelsChangedEvent>(this.OnHeadsetAutoSquadEncryptionChannelsChanged), new Type[1]
    {
      typeof (SharedHeadsetSystem)
    });
    this.Subs.BuiEvents<RMCRadioFilterComponent>((object) RMCRadioFilterUI.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<RMCRadioFilterComponent>) (subs => subs.Event<RMCRadioFilterBuiMsg>(new EntityEventRefHandler<RMCRadioFilterComponent, RMCRadioFilterBuiMsg>(this.OnRadioFilterBuiMsg))));
  }

  private void OnHeadsetEncryptionChannelsChanged(
    Entity<RMCHeadsetComponent> ent,
    ref EncryptionChannelsChangedEvent args)
  {
    this._toUpdate.Add((Entity<RMCHeadsetComponent, EncryptionKeyHolderComponent>) ((EntityUid) ent, (RMCHeadsetComponent) ent, args.Component));
  }

  private void OnGetAltVerbs(
    Entity<RMCRadioFilterComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.Hands == null)
      return;
    EntityUid user = args.User;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = "Tune Radio";
    alternativeVerb.IconEntity = new NetEntity?(this.GetNetEntity(ent.Owner));
    alternativeVerb.Act = (Action) (() => this._ui.OpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) RMCRadioFilterUI.Key, new EntityUid?(user)));
    verbs.Add(alternativeVerb);
  }

  private void OnHeadsetAutoSquadRefresh<T>(Entity<HeadsetAutoSquadComponent> ent, ref T args)
  {
    EncryptionKeyHolderComponent comp;
    if (!this.TryComp<EncryptionKeyHolderComponent>(ent.Owner, out comp) || comp.KeyContainer == null)
      return;
    this._encryptionKey.UpdateChannels((EntityUid) ent, comp);
  }

  private void OnHeadsetAutoSquadEncryptionChannelsChanged(
    Entity<HeadsetAutoSquadComponent> ent,
    ref EncryptionChannelsChangedEvent args)
  {
    BaseContainer container;
    SquadMemberComponent comp1;
    SquadTeamComponent comp2;
    if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) ent, (TransformComponent) null), out container) || !this.TryComp<SquadMemberComponent>(container.Owner, out comp1) || !this.TryComp<SquadTeamComponent>(comp1.Squad, out comp2))
      return;
    ProtoId<RadioChannelPrototype>? radio = comp2.Radio;
    if (!radio.HasValue)
      return;
    ProtoId<RadioChannelPrototype> valueOrDefault = radio.GetValueOrDefault();
    args.Component.Channels.Add((string) valueOrDefault);
  }

  private void OnRadioFilterBuiMsg(
    Entity<RMCRadioFilterComponent> ent,
    ref RMCRadioFilterBuiMsg args)
  {
    if (args.Toggle)
      ent.Comp.DisabledChannels.Remove(args.Channel);
    else
      ent.Comp.DisabledChannels.Add(args.Channel);
    this.Dirty<RMCRadioFilterComponent>(ent);
  }

  public override void Update(float frameTime)
  {
    try
    {
      foreach (Entity<RMCHeadsetComponent, EncryptionKeyHolderComponent> uid in this._toUpdate)
      {
        if (!this.TerminatingOrDeleted((EntityUid) uid) && uid.Comp1.Running && uid.Comp2.Running)
        {
          foreach (ProtoId<RadioChannelPrototype> channel in uid.Comp1.Channels)
          {
            uid.Comp2.Channels.Add((string) channel);
            this.Dirty((EntityUid) uid, (IComponent) uid.Comp2);
          }
        }
      }
    }
    finally
    {
      this._toUpdate.Clear();
    }
  }
}
