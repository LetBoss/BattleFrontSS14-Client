// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Alerts.Controls.AlertControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Actions.UI;
using Content.Client.Cooldown;
using Content.Shared.Alert;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Systems.Alerts.Controls;

public sealed class AlertControl : BaseButton
{
  [Dependency]
  private IEntityManager _entityManager;
  private readonly SpriteSystem _sprite;
  private (TimeSpan Start, TimeSpan End)? _cooldown;
  private string? _dynamicMessage;
  private short? _severity;
  private readonly SpriteView _icon;
  private readonly CooldownGraphic _cooldownGraphic;
  private EntityUid _spriteViewEntity;

  public AlertPrototype Alert { get; }

  public (TimeSpan Start, TimeSpan End)? Cooldown
  {
    get => this._cooldown;
    set
    {
      this._cooldown = value;
      if (!(((Control) this).SuppliedTooltip is ActionAlertTooltip suppliedTooltip))
        return;
      suppliedTooltip.Cooldown = value;
    }
  }

  public string? DynamicMessage
  {
    get => this._dynamicMessage;
    set
    {
      this._dynamicMessage = value;
      if (!(((Control) this).SuppliedTooltip is ActionAlertTooltip suppliedTooltip))
        return;
      suppliedTooltip.DynamicMessage = value;
    }
  }

  public AlertControl(AlertPrototype alert, short? severity)
  {
    this.MuteSounds = true;
    IoCManager.InjectDependencies<AlertControl>(this);
    this._sprite = this._entityManager.System<SpriteSystem>();
    // ISSUE: method pointer
    ((Control) this).TooltipSupplier = new TooltipSupplier((object) this, __methodptr(SupplyTooltip));
    this.Alert = alert;
    ((Control) this).HorizontalAlignment = (Control.HAlignment) 1;
    this._severity = severity;
    SpriteView spriteView = new SpriteView();
    spriteView.Scale = new Vector2(2f, 2f);
    ((Control) spriteView).MaxSize = new Vector2(64f, 64f);
    spriteView.Stretch = (SpriteView.StretchMode) 0;
    ((Control) spriteView).HorizontalAlignment = (Control.HAlignment) 1;
    this._icon = spriteView;
    this.SetupIcon();
    ((Control) this).Children.Add((Control) this._icon);
    CooldownGraphic cooldownGraphic = new CooldownGraphic();
    cooldownGraphic.MaxSize = new Vector2(64f, 64f);
    this._cooldownGraphic = cooldownGraphic;
    ((Control) this).Children.Add((Control) this._cooldownGraphic);
  }

  private Control SupplyTooltip(Control? sender)
  {
    return (Control) new ActionAlertTooltip(FormattedMessage.FromMarkupOrThrow(Loc.GetString(this.Alert.Name)), FormattedMessage.FromMarkupOrThrow(Loc.GetString(this.Alert.Description)))
    {
      Cooldown = this.Cooldown,
      DynamicMessage = this.DynamicMessage
    };
  }

  public void SetSeverity(short? severity)
  {
    short? severity1 = this._severity;
    int? nullable1 = severity1.HasValue ? new int?((int) severity1.GetValueOrDefault()) : new int?();
    short? nullable2 = severity;
    int? nullable3 = nullable2.HasValue ? new int?((int) nullable2.GetValueOrDefault()) : new int?();
    if (nullable1.GetValueOrDefault() == nullable3.GetValueOrDefault() & nullable1.HasValue == nullable3.HasValue)
      return;
    this._severity = severity;
    SpriteComponent spriteComponent;
    if (!this._entityManager.TryGetComponent<SpriteComponent>(this._spriteViewEntity, ref spriteComponent))
      return;
    SpriteSpecifier icon = this.Alert.GetIcon(this._severity);
    int num;
    if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((this._spriteViewEntity, spriteComponent)), (Enum) AlertVisualLayers.Base, ref num, false))
      return;
    this._sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((this._spriteViewEntity, spriteComponent)), num, icon);
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    ((Control) this).FrameUpdate(args);
    ((Control) this).UserInterfaceManager.GetUIController<AlertsUIController>().UpdateAlertSpriteEntity(this._spriteViewEntity, this.Alert);
    if (!this.Cooldown.HasValue)
    {
      this._cooldownGraphic.Visible = false;
      this._cooldownGraphic.Progress = 0.0f;
    }
    else
    {
      CooldownGraphic cooldownGraphic = this._cooldownGraphic;
      (TimeSpan, TimeSpan)? cooldown = this.Cooldown;
      TimeSpan start = cooldown.Value.Item1;
      cooldown = this.Cooldown;
      TimeSpan end = cooldown.Value.Item2;
      cooldownGraphic.FromTime(start, end);
    }
  }

  private void SetupIcon()
  {
    if (!this._entityManager.Deleted(this._spriteViewEntity))
      this._entityManager.QueueDeleteEntity(new EntityUid?(this._spriteViewEntity));
    this._spriteViewEntity = this._entityManager.Spawn(EntProtoId.op_Implicit(this.Alert.AlertViewEntity), (ComponentRegistry) null, true);
    SpriteComponent spriteComponent;
    if (this._entityManager.TryGetComponent<SpriteComponent>(this._spriteViewEntity, ref spriteComponent))
    {
      SpriteSpecifier icon = this.Alert.GetIcon(this._severity);
      int num;
      if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((this._spriteViewEntity, spriteComponent)), (Enum) AlertVisualLayers.Base, ref num, false))
        this._sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((this._spriteViewEntity, spriteComponent)), num, icon);
    }
    this._icon.SetEntity(new EntityUid?(this._spriteViewEntity));
  }

  protected virtual void EnteredTree()
  {
    ((Control) this).EnteredTree();
    this.SetupIcon();
  }

  protected virtual void ExitedTree()
  {
    ((Control) this).ExitedTree();
    if (this._entityManager.Deleted(this._spriteViewEntity))
      return;
    this._entityManager.QueueDeleteEntity(new EntityUid?(this._spriteViewEntity));
  }

  [Obsolete]
  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (this._entityManager.Deleted(this._spriteViewEntity))
      return;
    this._entityManager.QueueDeleteEntity(new EntityUid?(this._spriteViewEntity));
  }
}
