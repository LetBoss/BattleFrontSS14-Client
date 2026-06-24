using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Content.Client.Popups;

public sealed class PopupSystem : SharedPopupSystem
{
	public abstract class PopupLabel
	{
		public PopupType Type;

		public int Repeats = 1;

		public string Text { get; set; } = string.Empty;

		public float TotalTime { get; set; }
	}

	public sealed class WorldPopupLabel(EntityCoordinates coordinates) : PopupLabel
	{
		public EntityCoordinates InitialPos = coordinates;
	}

	public sealed class CursorPopupLabel(ScreenCoordinates screenCoords) : PopupLabel
	{
		public ScreenCoordinates InitialPos = screenCoords;
	}

	private record struct WorldPopupData(string Message, PopupType Type, EntityCoordinates Coordinates, EntityUid? Entity);

	private record struct CursorPopupData(string Message, PopupType Type);

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

	private readonly Dictionary<WorldPopupData, WorldPopupLabel> _aliveWorldLabels = new Dictionary<WorldPopupData, WorldPopupLabel>();

	private readonly Dictionary<CursorPopupData, CursorPopupLabel> _aliveCursorLabels = new Dictionary<CursorPopupData, CursorPopupLabel>();

	public const float MinimumPopupLifetime = 0.7f;

	public const float MaximumPopupLifetime = 5f;

	public const float PopupLifetimePerCharacter = 0.04f;

	public IReadOnlyCollection<WorldPopupLabel> WorldLabels => _aliveWorldLabels.Values;

	public IReadOnlyCollection<CursorPopupLabel> CursorLabels => _aliveCursorLabels.Values;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeNetworkEvent<PopupCursorEvent>((EntityEventHandler<PopupCursorEvent>)OnPopupCursorEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PopupCoordinatesEvent>((EntityEventHandler<PopupCoordinatesEvent>)OnPopupCoordinatesEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PopupEntityEvent>((EntityEventHandler<PopupEntityEvent>)OnPopupEntityEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestart, (Type[])null, (Type[])null);
		_overlay.AddOverlay((Overlay)(object)new PopupOverlay(_configManager, (IEntityManager)(object)((EntitySystem)this).EntityManager, _playerManager, _prototype, _uiManager, _uiManager.GetUIController<PopupUIController>(), _examine, _transform, this));
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlay.RemoveOverlay<PopupOverlay>();
	}

	private void WrapAndRepeatPopup(PopupLabel existingLabel, string popupMessage)
	{
		existingLabel.TotalTime = 0f;
		existingLabel.Repeats++;
		existingLabel.Text = ((EntitySystem)this).Loc.GetString("popup-system-repeated-popup-stacking-wrap", (ValueTuple<string, object>)("popup-message", popupMessage), (ValueTuple<string, object>)("count", existingLabel.Repeats));
	}

	private void PopupMessage(string? message, PopupType type, EntityCoordinates coordinates, EntityUid? entity, bool recordReplay)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (message == null)
		{
			return;
		}
		if (recordReplay && _replayRecording.IsRecording)
		{
			if (entity.HasValue)
			{
				_replayRecording.RecordClientMessage((object)new PopupEntityEvent(message, type, ((EntitySystem)this).GetNetEntity(entity.Value, (MetaDataComponent)null)));
			}
			else
			{
				_replayRecording.RecordClientMessage((object)new PopupCoordinatesEvent(message, type, ((EntitySystem)this).GetNetCoordinates(coordinates, (MetaDataComponent)null)));
			}
		}
		WorldPopupData key = new WorldPopupData(message, type, coordinates, entity);
		if (_aliveWorldLabels.TryGetValue(key, out WorldPopupLabel value))
		{
			WrapAndRepeatPopup(value, key.Message);
			return;
		}
		WorldPopupLabel value2 = new WorldPopupLabel(coordinates)
		{
			Text = message,
			Type = type
		};
		_aliveWorldLabels.Add(key, value2);
	}

	public override void PopupCoordinates(string? message, EntityCoordinates coordinates, PopupType type = PopupType.Small)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		PopupMessage(message, type, coordinates, null, recordReplay: true);
	}

	public override void PopupCoordinates(string? message, EntityCoordinates coordinates, ICommonSession recipient, PopupType type = PopupType.Small)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (((ISharedPlayerManager)_playerManager).LocalSession == recipient)
		{
			PopupMessage(message, type, coordinates, null, recordReplay: true);
		}
	}

	public override void PopupCoordinates(string? message, EntityCoordinates coordinates, EntityUid recipient, PopupType type = PopupType.Small)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == recipient)
		{
			PopupMessage(message, type, coordinates, null, recordReplay: true);
		}
	}

	public override void PopupPredictedCoordinates(string? message, EntityCoordinates coordinates, EntityUid? recipient, PopupType type = PopupType.Small)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (recipient.HasValue && _timing.IsFirstTimePredicted)
		{
			PopupCoordinates(message, coordinates, recipient.Value, type);
		}
	}

	private void PopupCursorInternal(string? message, PopupType type, bool recordReplay)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (message != null)
		{
			if (recordReplay && _replayRecording.IsRecording)
			{
				_replayRecording.RecordClientMessage((object)new PopupCursorEvent(message, type));
			}
			CursorPopupData key = new CursorPopupData(message, type);
			if (_aliveCursorLabels.TryGetValue(key, out CursorPopupLabel value))
			{
				WrapAndRepeatPopup(value, key.Message);
				return;
			}
			CursorPopupLabel value2 = new CursorPopupLabel(_inputManager.MouseScreenPosition)
			{
				Text = message,
				Type = type
			};
			_aliveCursorLabels.Add(key, value2);
		}
	}

	public override void PopupCursor(string? message, PopupType type = PopupType.Small)
	{
		if (_timing.IsFirstTimePredicted)
		{
			PopupCursorInternal(message, type, recordReplay: true);
		}
	}

	public override void PopupCursor(string? message, ICommonSession recipient, PopupType type = PopupType.Small)
	{
		if (((ISharedPlayerManager)_playerManager).LocalSession == recipient)
		{
			PopupCursor(message, type);
		}
	}

	public override void PopupCursor(string? message, EntityUid recipient, PopupType type = PopupType.Small)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == recipient)
		{
			PopupCursor(message, type);
		}
	}

	public override void PopupPredictedCursor(string? message, ICommonSession recipient, PopupType type = PopupType.Small)
	{
		PopupCursor(message, recipient, type);
	}

	public override void PopupPredictedCursor(string? message, EntityUid recipient, PopupType type = PopupType.Small)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		PopupCursor(message, recipient, type);
	}

	public override void PopupCoordinates(string? message, EntityCoordinates coordinates, Filter filter, bool replayRecord, PopupType type = PopupType.Small)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		PopupCoordinates(message, coordinates, type);
	}

	public override void PopupEntity(string? message, EntityUid uid, EntityUid recipient, PopupType type = PopupType.Small)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == recipient)
		{
			PopupEntity(message, uid, type);
		}
	}

	public override void PopupEntity(string? message, EntityUid uid, ICommonSession recipient, PopupType type = PopupType.Small)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (((ISharedPlayerManager)_playerManager).LocalSession == recipient)
		{
			PopupEntity(message, uid, type);
		}
	}

	public override void PopupEntity(string? message, EntityUid uid, Filter filter, bool recordReplay, PopupType type = PopupType.Small)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (filter.Recipients.Contains(((ISharedPlayerManager)_playerManager).LocalSession))
		{
			PopupEntity(message, uid, type);
		}
	}

	public override void PopupClient(string? message, EntityUid? recipient, PopupType type = PopupType.Small)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (recipient.HasValue && _timing.IsFirstTimePredicted)
		{
			PopupCursor(message, recipient.Value, type);
		}
	}

	public override void PopupClient(string? message, EntityUid uid, EntityUid? recipient, PopupType type = PopupType.Small)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (recipient.HasValue && _timing.IsFirstTimePredicted)
		{
			PopupEntity(message, uid, recipient.Value, type);
		}
	}

	public override void PopupClient(string? message, EntityCoordinates coordinates, EntityUid? recipient, PopupType type = PopupType.Small)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (recipient.HasValue && _timing.IsFirstTimePredicted)
		{
			PopupCoordinates(message, coordinates, recipient.Value, type);
		}
	}

	public override void PopupEntity(string? message, EntityUid uid, PopupType type = PopupType.Small)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent val = default(TransformComponent);
		if (((EntitySystem)this).TryComp(uid, ref val))
		{
			PopupMessage(message, type, val.Coordinates, uid, recordReplay: true);
		}
	}

	public override void PopupPredicted(string? message, EntityUid uid, EntityUid? recipient, PopupType type = PopupType.Small)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (recipient.HasValue && _timing.IsFirstTimePredicted)
		{
			PopupEntity(message, uid, recipient.Value, type);
		}
	}

	public override void PopupPredicted(string? message, EntityUid uid, EntityUid? recipient, Filter filter, bool recordReplay, PopupType type = PopupType.Small)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (recipient.HasValue && _timing.IsFirstTimePredicted)
		{
			PopupEntity(message, uid, recipient.Value, type);
		}
	}

	public override void PopupPredicted(string? recipientMessage, string? othersMessage, EntityUid uid, EntityUid? recipient, PopupType type = PopupType.Small)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (recipient.HasValue && _timing.IsFirstTimePredicted)
		{
			PopupEntity(recipientMessage, uid, recipient.Value, type);
		}
	}

	private void OnPopupCursorEvent(PopupCursorEvent ev)
	{
		PopupCursorInternal(ev.Message, ev.Type, recordReplay: false);
	}

	private void OnPopupCoordinatesEvent(PopupCoordinatesEvent ev)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		PopupMessage(ev.Message, ev.Type, ((EntitySystem)this).GetCoordinates(ev.Coordinates), null, recordReplay: false);
	}

	private void OnPopupEntityEvent(PopupEntityEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity = ((EntitySystem)this).GetEntity(ev.Uid);
		TransformComponent val = default(TransformComponent);
		if (((EntitySystem)this).TryComp(entity, ref val))
		{
			PopupMessage(ev.Message, ev.Type, val.Coordinates, entity, recordReplay: false);
		}
	}

	private void OnRoundRestart(RoundRestartCleanupEvent ev)
	{
		_aliveCursorLabels.Clear();
		_aliveWorldLabels.Clear();
	}

	public static float GetPopupLifetime(PopupLabel label)
	{
		return Math.Clamp(0.04f * (float)label.Text.Length, 0.7f, 5f);
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		if (_aliveWorldLabels.Count == 0 && _aliveCursorLabels.Count == 0)
		{
			return;
		}
		if (_aliveWorldLabels.Count > 0)
		{
			ValueList<WorldPopupData> val = default(ValueList<WorldPopupData>);
			foreach (var (worldPopupData2, worldPopupLabel2) in _aliveWorldLabels)
			{
				worldPopupLabel2.TotalTime += frameTime;
				if (worldPopupLabel2.TotalTime > GetPopupLifetime(worldPopupLabel2) || ((EntitySystem)this).Deleted(worldPopupLabel2.InitialPos.EntityId, (MetaDataComponent)null))
				{
					val.Add(worldPopupData2);
				}
			}
			Enumerator<WorldPopupData> enumerator2 = val.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					WorldPopupData current = enumerator2.Current;
					_aliveWorldLabels.Remove(current);
				}
			}
			finally
			{
				((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
			}
		}
		if (_aliveCursorLabels.Count <= 0)
		{
			return;
		}
		ValueList<CursorPopupData> val2 = default(ValueList<CursorPopupData>);
		foreach (var (cursorPopupData2, cursorPopupLabel2) in _aliveCursorLabels)
		{
			cursorPopupLabel2.TotalTime += frameTime;
			if (cursorPopupLabel2.TotalTime > GetPopupLifetime(cursorPopupLabel2))
			{
				val2.Add(cursorPopupData2);
			}
		}
		Enumerator<CursorPopupData> enumerator4 = val2.GetEnumerator();
		try
		{
			while (enumerator4.MoveNext())
			{
				CursorPopupData current2 = enumerator4.Current;
				_aliveCursorLabels.Remove(current2);
			}
		}
		finally
		{
			((IDisposable)enumerator4/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
