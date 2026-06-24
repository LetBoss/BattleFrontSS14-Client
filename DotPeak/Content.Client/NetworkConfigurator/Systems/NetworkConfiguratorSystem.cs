// Decompiled with JetBrains decompiler
// Type: Content.Client.NetworkConfigurator.Systems.NetworkConfiguratorSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Actions;
using Content.Client.Items;
using Content.Client.Message;
using Content.Shared.Actions.Components;
using Content.Shared.DeviceNetwork.Components;
using Content.Shared.DeviceNetwork.Systems;
using Content.Shared.Input;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.NetworkConfigurator.Systems;

public sealed class NetworkConfiguratorSystem : SharedNetworkConfiguratorSystem
{
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IOverlayManager _overlay;
  [Dependency]
  private ActionsSystem _actions;
  [Dependency]
  private IInputManager _inputManager;
  private static readonly EntProtoId Action = EntProtoId.op_Implicit("ActionClearNetworkLinkOverlays");

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ClearAllOverlaysEvent>((EntityEventHandler<ClearAllOverlaysEvent>) (_ => this.ClearAllOverlays()), (Type[]) null, (Type[]) null);
    this.Subs.ItemStatus<NetworkConfiguratorComponent>(new Func<Entity<NetworkConfiguratorComponent>, Control>(this.OnCollectItemStatus));
  }

  private Control OnCollectItemStatus(Entity<NetworkConfiguratorComponent> entity)
  {
    IKeyBinding ikeyBinding;
    this._inputManager.TryGetKeyBinding(ContentKeyFunctions.AltUseItemInHand, ref ikeyBinding);
    return (Control) new NetworkConfiguratorSystem.StatusControl(Entity<NetworkConfiguratorComponent>.op_Implicit(entity), ikeyBinding?.GetKeyString() ?? "");
  }

  public bool ConfiguredListIsTracked(EntityUid uid, NetworkConfiguratorComponent? component = null)
  {
    return this.Resolve<NetworkConfiguratorComponent>(uid, ref component, true) && component.ActiveDeviceList.HasValue && this.HasComp<NetworkConfiguratorActiveLinkOverlayComponent>(component.ActiveDeviceList.Value);
  }

  public void ToggleVisualization(
    EntityUid uid,
    bool toggle,
    NetworkConfiguratorComponent? component = null)
  {
    if (!((ISharedPlayerManager) this._playerManager).LocalEntity.HasValue || !this.Resolve<NetworkConfiguratorComponent>(uid, ref component, true))
      return;
    EntityUid? nullable = component.ActiveDeviceList;
    if (!nullable.HasValue)
      return;
    if (!toggle)
    {
      nullable = component.ActiveDeviceList;
      this.RemComp<NetworkConfiguratorActiveLinkOverlayComponent>(nullable.Value);
      NetworkConfiguratorLinkOverlay configuratorLinkOverlay;
      if (!this._overlay.TryGetOverlay<NetworkConfiguratorLinkOverlay>(ref configuratorLinkOverlay))
        return;
      Dictionary<EntityUid, Color> colors = configuratorLinkOverlay.Colors;
      nullable = component.ActiveDeviceList;
      EntityUid key = nullable.Value;
      colors.Remove(key);
      if (configuratorLinkOverlay.Colors.Count > 0)
        return;
      ActionsSystem actions = this._actions;
      nullable = configuratorLinkOverlay.Action;
      Entity<ActionComponent>? action = nullable.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(nullable.GetValueOrDefault())) : new Entity<ActionComponent>?();
      actions.RemoveAction(action);
      this._overlay.RemoveOverlay<NetworkConfiguratorLinkOverlay>();
    }
    else
    {
      if (!this._overlay.HasOverlay<NetworkConfiguratorLinkOverlay>())
      {
        NetworkConfiguratorLinkOverlay configuratorLinkOverlay = new NetworkConfiguratorLinkOverlay();
        this._overlay.AddOverlay((Overlay) configuratorLinkOverlay);
        nullable = ((ISharedPlayerManager) this._playerManager).LocalEntity;
        EntityUid entityUid = nullable.Value;
        configuratorLinkOverlay.Action = new EntityUid?(this.Spawn(EntProtoId.op_Implicit(NetworkConfiguratorSystem.Action), (ComponentRegistry) null, true));
        this._actions.AddActionDirect(Entity<ActionsComponent>.op_Implicit(entityUid), new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(configuratorLinkOverlay.Action.Value)));
      }
      nullable = component.ActiveDeviceList;
      this.EnsureComp<NetworkConfiguratorActiveLinkOverlayComponent>(nullable.Value);
    }
  }

  public void ClearAllOverlays()
  {
    NetworkConfiguratorLinkOverlay configuratorLinkOverlay;
    if (!this._overlay.TryGetOverlay<NetworkConfiguratorLinkOverlay>(ref configuratorLinkOverlay))
      return;
    EntityQueryEnumerator<NetworkConfiguratorActiveLinkOverlayComponent> entityQueryEnumerator = this.EntityQueryEnumerator<NetworkConfiguratorActiveLinkOverlayComponent>();
    EntityUid entityUid;
    NetworkConfiguratorActiveLinkOverlayComponent overlayComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref overlayComponent))
      this.RemCompDeferred<NetworkConfiguratorActiveLinkOverlayComponent>(entityUid);
    ActionsSystem actions = this._actions;
    EntityUid? action1 = configuratorLinkOverlay.Action;
    Entity<ActionComponent>? action2 = action1.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(action1.GetValueOrDefault())) : new Entity<ActionComponent>?();
    actions.RemoveAction(action2);
    this._overlay.RemoveOverlay((Overlay) configuratorLinkOverlay);
  }

  private sealed class StatusControl : Control
  {
    private readonly RichTextLabel _label;
    private readonly NetworkConfiguratorComponent _configurator;
    private readonly string _keyBindingName;
    private bool? _linkModeActive;

    public StatusControl(NetworkConfiguratorComponent configurator, string keyBindingName)
    {
      this._configurator = configurator;
      this._keyBindingName = keyBindingName;
      RichTextLabel richTextLabel = new RichTextLabel();
      ((Control) richTextLabel).StyleClasses.Add("ItemStatus");
      this._label = richTextLabel;
      this.AddChild((Control) this._label);
    }

    protected virtual void FrameUpdate(FrameEventArgs args)
    {
      base.FrameUpdate(args);
      if (this._linkModeActive.HasValue)
      {
        bool? linkModeActive1 = this._linkModeActive;
        bool linkModeActive2 = this._configurator.LinkModeActive;
        if (linkModeActive1.GetValueOrDefault() == linkModeActive2 & linkModeActive1.HasValue)
          return;
      }
      this._linkModeActive = new bool?(this._configurator.LinkModeActive);
      this._label.SetMarkup(Loc.GetString("network-configurator-item-status-label", new (string, object)[2]
      {
        ("mode", (object) Loc.GetString(this._linkModeActive.GetValueOrDefault() ? "network-configurator-examine-mode-link" : "network-configurator-examine-mode-list")),
        ("keybinding", (object) this._keyBindingName)
      }));
    }
  }
}
