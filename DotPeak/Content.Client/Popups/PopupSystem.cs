// Decompiled with JetBrains decompiler
// Type: Content.Client.Popups.PopupSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Examine;
using Content.Shared.GameTicking;
using Content.Shared.Popups;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.Collections;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Replays;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Popups;

public sealed class PopupSystem : SharedPopupSystem
{
  [Dependency]
  private IConfigurationManager _configManager;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IOverlayManager _overlay;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IUserInterfaceManager _uiManager;
  [Dependency]
  private IReplayRecordingManager _replayRecording;
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  private SharedTransformSystem _transform;
  private readonly Dictionary<PopupSystem.WorldPopupData, PopupSystem.WorldPopupLabel> _aliveWorldLabels = new Dictionary<PopupSystem.WorldPopupData, PopupSystem.WorldPopupLabel>();
  private readonly Dictionary<PopupSystem.CursorPopupData, PopupSystem.CursorPopupLabel> _aliveCursorLabels = new Dictionary<PopupSystem.CursorPopupData, PopupSystem.CursorPopupLabel>();
  public const float MinimumPopupLifetime = 0.7f;
  public const float MaximumPopupLifetime = 5f;
  public const float PopupLifetimePerCharacter = 0.04f;

  public IReadOnlyCollection<PopupSystem.WorldPopupLabel> WorldLabels
  {
    get => (IReadOnlyCollection<PopupSystem.WorldPopupLabel>) this._aliveWorldLabels.Values;
  }

  public IReadOnlyCollection<PopupSystem.CursorPopupLabel> CursorLabels
  {
    get => (IReadOnlyCollection<PopupSystem.CursorPopupLabel>) this._aliveCursorLabels.Values;
  }

  public virtual void Initialize()
  {
    this.SubscribeNetworkEvent<PopupCursorEvent>(new EntityEventHandler<PopupCursorEvent>(this.OnPopupCursorEvent), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PopupCoordinatesEvent>(new EntityEventHandler<PopupCoordinatesEvent>(this.OnPopupCoordinatesEvent), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PopupEntityEvent>(new EntityEventHandler<PopupEntityEvent>(this.OnPopupEntityEvent), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestart), (Type[]) null, (Type[]) null);
    this._overlay.AddOverlay((Overlay) new PopupOverlay(this._configManager, (IEntityManager) this.EntityManager, this._playerManager, this._prototype, this._uiManager, this._uiManager.GetUIController<PopupUIController>(), this._examine, this._transform, this));
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._overlay.RemoveOverlay<PopupOverlay>();
  }

  private void WrapAndRepeatPopup(PopupSystem.PopupLabel existingLabel, string popupMessage)
  {
    existingLabel.TotalTime = 0.0f;
    ++existingLabel.Repeats;
    existingLabel.Text = this.Loc.GetString("popup-system-repeated-popup-stacking-wrap", ("popup-message", (object) popupMessage), ("count", (object) existingLabel.Repeats));
  }

  private void PopupMessage(
    string? message,
    PopupType type,
    EntityCoordinates coordinates,
    EntityUid? entity,
    bool recordReplay)
  {
    if (message == null)
      return;
    if (recordReplay && this._replayRecording.IsRecording)
    {
      if (entity.HasValue)
        this._replayRecording.RecordClientMessage((object) new PopupEntityEvent(message, type, this.GetNetEntity(entity.Value, (MetaDataComponent) null)));
      else
        this._replayRecording.RecordClientMessage((object) new PopupCoordinatesEvent(message, type, this.GetNetCoordinates(coordinates, (MetaDataComponent) null)));
    }
    PopupSystem.WorldPopupData key = new PopupSystem.WorldPopupData(message, type, coordinates, entity);
    PopupSystem.WorldPopupLabel existingLabel;
    if (this._aliveWorldLabels.TryGetValue(key, out existingLabel))
    {
      this.WrapAndRepeatPopup((PopupSystem.PopupLabel) existingLabel, key.Message);
    }
    else
    {
      PopupSystem.WorldPopupLabel worldPopupLabel1 = new PopupSystem.WorldPopupLabel(coordinates);
      worldPopupLabel1.Text = message;
      worldPopupLabel1.Type = type;
      PopupSystem.WorldPopupLabel worldPopupLabel2 = worldPopupLabel1;
      this._aliveWorldLabels.Add(key, worldPopupLabel2);
    }
  }

  public override void PopupCoordinates(
    string? message,
    EntityCoordinates coordinates,
    PopupType type = PopupType.Small)
  {
    this.PopupMessage(message, type, coordinates, new EntityUid?(), true);
  }

  public override void PopupCoordinates(
    string? message,
    EntityCoordinates coordinates,
    ICommonSession recipient,
    PopupType type = PopupType.Small)
  {
    if (((ISharedPlayerManager) this._playerManager).LocalSession != recipient)
      return;
    this.PopupMessage(message, type, coordinates, new EntityUid?(), true);
  }

  public override void PopupCoordinates(
    string? message,
    EntityCoordinates coordinates,
    EntityUid recipient,
    PopupType type = PopupType.Small)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    EntityUid entityUid = recipient;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this.PopupMessage(message, type, coordinates, new EntityUid?(), true);
  }

  public override void PopupPredictedCoordinates(
    string? message,
    EntityCoordinates coordinates,
    EntityUid? recipient,
    PopupType type = PopupType.Small)
  {
    if (!recipient.HasValue || !this._timing.IsFirstTimePredicted)
      return;
    this.PopupCoordinates(message, coordinates, recipient.Value, type);
  }

  private void PopupCursorInternal(string? message, PopupType type, bool recordReplay)
  {
    if (message == null)
      return;
    if (recordReplay && this._replayRecording.IsRecording)
      this._replayRecording.RecordClientMessage((object) new PopupCursorEvent(message, type));
    PopupSystem.CursorPopupData key = new PopupSystem.CursorPopupData(message, type);
    PopupSystem.CursorPopupLabel existingLabel;
    if (this._aliveCursorLabels.TryGetValue(key, out existingLabel))
    {
      this.WrapAndRepeatPopup((PopupSystem.PopupLabel) existingLabel, key.Message);
    }
    else
    {
      PopupSystem.CursorPopupLabel cursorPopupLabel1 = new PopupSystem.CursorPopupLabel(this._inputManager.MouseScreenPosition);
      cursorPopupLabel1.Text = message;
      cursorPopupLabel1.Type = type;
      PopupSystem.CursorPopupLabel cursorPopupLabel2 = cursorPopupLabel1;
      this._aliveCursorLabels.Add(key, cursorPopupLabel2);
    }
  }

  public override void PopupCursor(string? message, PopupType type = PopupType.Small)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    this.PopupCursorInternal(message, type, true);
  }

  public override void PopupCursor(string? message, ICommonSession recipient, PopupType type = PopupType.Small)
  {
    if (((ISharedPlayerManager) this._playerManager).LocalSession != recipient)
      return;
    this.PopupCursor(message, type);
  }

  public override void PopupCursor(string? message, EntityUid recipient, PopupType type = PopupType.Small)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    EntityUid entityUid = recipient;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this.PopupCursor(message, type);
  }

  public override void PopupPredictedCursor(
    string? message,
    ICommonSession recipient,
    PopupType type = PopupType.Small)
  {
    this.PopupCursor(message, recipient, type);
  }

  public override void PopupPredictedCursor(string? message, EntityUid recipient, PopupType type = PopupType.Small)
  {
    this.PopupCursor(message, recipient, type);
  }

  public override void PopupCoordinates(
    string? message,
    EntityCoordinates coordinates,
    Filter filter,
    bool replayRecord,
    PopupType type = PopupType.Small)
  {
    this.PopupCoordinates(message, coordinates, type);
  }

  public override void PopupEntity(
    string? message,
    EntityUid uid,
    EntityUid recipient,
    PopupType type = PopupType.Small)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    EntityUid entityUid = recipient;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this.PopupEntity(message, uid, type);
  }

  public override void PopupEntity(
    string? message,
    EntityUid uid,
    ICommonSession recipient,
    PopupType type = PopupType.Small)
  {
    if (((ISharedPlayerManager) this._playerManager).LocalSession != recipient)
      return;
    this.PopupEntity(message, uid, type);
  }

  public override void PopupEntity(
    string? message,
    EntityUid uid,
    Filter filter,
    bool recordReplay,
    PopupType type = PopupType.Small)
  {
    if (!filter.Recipients.Contains<ICommonSession>(((ISharedPlayerManager) this._playerManager).LocalSession))
      return;
    this.PopupEntity(message, uid, type);
  }

  public override void PopupClient(string? message, EntityUid? recipient, PopupType type = PopupType.Small)
  {
    if (!recipient.HasValue || !this._timing.IsFirstTimePredicted)
      return;
    this.PopupCursor(message, recipient.Value, type);
  }

  public override void PopupClient(
    string? message,
    EntityUid uid,
    EntityUid? recipient,
    PopupType type = PopupType.Small)
  {
    if (!recipient.HasValue || !this._timing.IsFirstTimePredicted)
      return;
    this.PopupEntity(message, uid, recipient.Value, type);
  }

  public override void PopupClient(
    string? message,
    EntityCoordinates coordinates,
    EntityUid? recipient,
    PopupType type = PopupType.Small)
  {
    if (!recipient.HasValue || !this._timing.IsFirstTimePredicted)
      return;
    this.PopupCoordinates(message, coordinates, recipient.Value, type);
  }

  public override void PopupEntity(string? message, EntityUid uid, PopupType type = PopupType.Small)
  {
    TransformComponent transformComponent;
    if (!this.TryComp(uid, ref transformComponent))
      return;
    this.PopupMessage(message, type, transformComponent.Coordinates, new EntityUid?(uid), true);
  }

  public override void PopupPredicted(
    string? message,
    EntityUid uid,
    EntityUid? recipient,
    PopupType type = PopupType.Small)
  {
    if (!recipient.HasValue || !this._timing.IsFirstTimePredicted)
      return;
    this.PopupEntity(message, uid, recipient.Value, type);
  }

  public override void PopupPredicted(
    string? message,
    EntityUid uid,
    EntityUid? recipient,
    Filter filter,
    bool recordReplay,
    PopupType type = PopupType.Small)
  {
    if (!recipient.HasValue || !this._timing.IsFirstTimePredicted)
      return;
    this.PopupEntity(message, uid, recipient.Value, type);
  }

  public override void PopupPredicted(
    string? recipientMessage,
    string? othersMessage,
    EntityUid uid,
    EntityUid? recipient,
    PopupType type = PopupType.Small)
  {
    if (!recipient.HasValue || !this._timing.IsFirstTimePredicted)
      return;
    this.PopupEntity(recipientMessage, uid, recipient.Value, type);
  }

  private void OnPopupCursorEvent(PopupCursorEvent ev)
  {
    this.PopupCursorInternal(ev.Message, ev.Type, false);
  }

  private void OnPopupCoordinatesEvent(PopupCoordinatesEvent ev)
  {
    this.PopupMessage(ev.Message, ev.Type, this.GetCoordinates(ev.Coordinates), new EntityUid?(), false);
  }

  private void OnPopupEntityEvent(PopupEntityEvent ev)
  {
    EntityUid entity = this.GetEntity(ev.Uid);
    TransformComponent transformComponent;
    if (!this.TryComp(entity, ref transformComponent))
      return;
    this.PopupMessage(ev.Message, ev.Type, transformComponent.Coordinates, new EntityUid?(entity), false);
  }

  private void OnRoundRestart(RoundRestartCleanupEvent ev)
  {
    this._aliveCursorLabels.Clear();
    this._aliveWorldLabels.Clear();
  }

  public static float GetPopupLifetime(PopupSystem.PopupLabel label)
  {
    return Math.Clamp(0.04f * (float) label.Text.Length, 0.7f, 5f);
  }

  public virtual void FrameUpdate(float frameTime)
  {
    if (this._aliveWorldLabels.Count == 0 && this._aliveCursorLabels.Count == 0)
      return;
    if (this._aliveWorldLabels.Count > 0)
    {
      ValueList<PopupSystem.WorldPopupData> valueList = new ValueList<PopupSystem.WorldPopupData>();
      foreach ((PopupSystem.WorldPopupData key, PopupSystem.WorldPopupLabel label) in this._aliveWorldLabels)
      {
        label.TotalTime += frameTime;
        if ((double) label.TotalTime > (double) PopupSystem.GetPopupLifetime((PopupSystem.PopupLabel) label) || this.Deleted(label.InitialPos.EntityId, (MetaDataComponent) null))
          valueList.Add(key);
      }
      foreach (PopupSystem.WorldPopupData key in valueList)
        this._aliveWorldLabels.Remove(key);
    }
    if (this._aliveCursorLabels.Count <= 0)
      return;
    ValueList<PopupSystem.CursorPopupData> valueList1 = new ValueList<PopupSystem.CursorPopupData>();
    foreach ((PopupSystem.CursorPopupData key, PopupSystem.CursorPopupLabel label) in this._aliveCursorLabels)
    {
      label.TotalTime += frameTime;
      if ((double) label.TotalTime > (double) PopupSystem.GetPopupLifetime((PopupSystem.PopupLabel) label))
        valueList1.Add(key);
    }
    foreach (PopupSystem.CursorPopupData key in valueList1)
      this._aliveCursorLabels.Remove(key);
  }

  public abstract class PopupLabel
  {
    public PopupType Type;
    public int Repeats = 1;

    public string Text { get; set; } = string.Empty;

    public float TotalTime { get; set; }
  }

  public sealed class WorldPopupLabel(EntityCoordinates coordinates) : PopupSystem.PopupLabel
  {
    public EntityCoordinates InitialPos = coordinates;
  }

  public sealed class CursorPopupLabel(ScreenCoordinates screenCoords) : PopupSystem.PopupLabel
  {
    public ScreenCoordinates InitialPos = screenCoords;
  }

  private record struct WorldPopupData(
    string Message,
    PopupType Type,
    EntityCoordinates Coordinates,
    EntityUid? Entity)
  ;

  private record struct CursorPopupData(string Message, PopupType Type);
}
