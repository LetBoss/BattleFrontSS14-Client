using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.Input;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderLinesSystem : EntitySystem
{
	[Dependency]
	private IEyeManager _eye;

	[Dependency]
	private IInputManager _input;

	[Dependency]
	private IOverlayManager _overlays;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IUserInterfaceManager _ui;

	public const float EraseRadius = 2f;

	private readonly Dictionary<int, CivCommanderLineState> _lines = new Dictionary<int, CivCommanderLineState>();

	private readonly Dictionary<int, CivCommanderLabelState> _labels = new Dictionary<int, CivCommanderLabelState>();

	private CivCommanderLinesOverlay? _overlay;

	private CivCommanderLabelsOverlay? _labelsOverlay;

	public CivCommanderLineColor SelectedColor { get; set; }

	public bool IsDrawing { get; private set; }

	public MapCoordinates DrawStart { get; private set; }

	public bool IsPlacingLabel { get; private set; }

	public string PendingLabelText { get; private set; } = string.Empty;

	public float PendingLabelRotation { get; private set; }

	public IReadOnlyDictionary<int, CivCommanderLineState> Lines => _lines;

	public IReadOnlyDictionary<int, CivCommanderLabelState> Labels => _labels;

	public int LocalTeamId
	{
		get
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
			if (localEntity.HasValue)
			{
				EntityUid valueOrDefault = localEntity.GetValueOrDefault();
				CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
				if (((EntitySystem)this).TryComp<CivTeamMemberComponent>(valueOrDefault, ref civTeamMemberComponent))
				{
					return civTeamMemberComponent.TeamId;
				}
			}
			return 0;
		}
	}

	public override void Initialize()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<CivCommanderLineAddedEvent>((EntityEventHandler<CivCommanderLineAddedEvent>)OnLineAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CivCommanderLineRemovedEvent>((EntityEventHandler<CivCommanderLineRemovedEvent>)OnLineRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CivCommanderLinesSnapshotEvent>((EntityEventHandler<CivCommanderLinesSnapshotEvent>)OnSnapshot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CivCommanderLabelAddedEvent>((EntityEventHandler<CivCommanderLabelAddedEvent>)OnLabelAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CivCommanderLabelRemovedEvent>((EntityEventHandler<CivCommanderLabelRemovedEvent>)OnLabelRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CivCommanderLabelsSnapshotEvent>((EntityEventHandler<CivCommanderLabelsSnapshotEvent>)OnLabelsSnapshot, (Type[])null, (Type[])null);
		CommandBinds.Builder.Bind(CivKeyFunctions.CivCommanderDrawLine, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleDraw), true, true)).Bind(CivKeyFunctions.CivCommanderEraseLine, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleErase), true, true)).Bind(CivKeyFunctions.CivCommanderLabelRotate, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleRotate), true, true))
			.Register<CivCommanderLinesSystem>();
		_overlay = new CivCommanderLinesOverlay((IEntityManager)(object)base.EntityManager, this);
		_overlays.AddOverlay((Overlay)(object)_overlay);
		_labelsOverlay = new CivCommanderLabelsOverlay((IEntityManager)(object)base.EntityManager, this);
		_overlays.AddOverlay((Overlay)(object)_labelsOverlay);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<CivCommanderLinesSystem>();
		if (_overlay != null)
		{
			_overlays.RemoveOverlay((Overlay)(object)_overlay);
			_overlay = null;
		}
		if (_labelsOverlay != null)
		{
			_overlays.RemoveOverlay((Overlay)(object)_labelsOverlay);
			_labelsOverlay = null;
		}
		_lines.Clear();
		_labels.Clear();
		IsDrawing = false;
		IsPlacingLabel = false;
	}

	private void OnLineAdded(CivCommanderLineAddedEvent ev)
	{
		_lines[ev.Line.Id] = ev.Line;
	}

	private void OnLineRemoved(CivCommanderLineRemovedEvent ev)
	{
		_lines.Remove(ev.Id);
	}

	private void OnSnapshot(CivCommanderLinesSnapshotEvent ev)
	{
		_lines.Clear();
		foreach (CivCommanderLineState line in ev.Lines)
		{
			_lines[line.Id] = line;
		}
	}

	private void OnLabelAdded(CivCommanderLabelAddedEvent ev)
	{
		_labels[ev.Label.Id] = ev.Label;
	}

	private void OnLabelRemoved(CivCommanderLabelRemovedEvent ev)
	{
		_labels.Remove(ev.Id);
	}

	private void OnLabelsSnapshot(CivCommanderLabelsSnapshotEvent ev)
	{
		_labels.Clear();
		foreach (CivCommanderLabelState label in ev.Labels)
		{
			_labels[label.Id] = label;
		}
	}

	public void StartLabelPlacement(string text)
	{
		if (IsCommander() && !string.IsNullOrWhiteSpace(text))
		{
			IsPlacingLabel = true;
			PendingLabelText = text.Trim();
			PendingLabelRotation = 0f;
		}
	}

	public void CancelLabelPlacement()
	{
		IsPlacingLabel = false;
		PendingLabelText = string.Empty;
		PendingLabelRotation = 0f;
	}

	public void RequestClearAll()
	{
		if (IsCommander())
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivCommanderLinesClearRequestEvent());
		}
	}

	private bool HandleDraw(in PointerInputCmdArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		if ((int)args.State != 1)
		{
			return false;
		}
		if (!IsCommander() || !IsViewportHover())
		{
			return false;
		}
		MapCoordinates val = _eye.PixelToMap(_input.MouseScreenPosition);
		if (val.MapId == MapId.Nullspace)
		{
			return false;
		}
		if (IsPlacingLabel)
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivCommanderLabelCreateRequestEvent(SelectedColor, val.MapId, val.Position, PendingLabelRotation, PendingLabelText));
			CancelLabelPlacement();
			return true;
		}
		if (!IsDrawing)
		{
			IsDrawing = true;
			DrawStart = val;
			return true;
		}
		if (val.MapId != DrawStart.MapId)
		{
			IsDrawing = false;
			return true;
		}
		if ((val.Position - DrawStart.Position).LengthSquared() < 0.01f)
		{
			IsDrawing = false;
			return true;
		}
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivCommanderLineCreateRequestEvent(SelectedColor, DrawStart.MapId, DrawStart.Position, val.Position));
		IsDrawing = false;
		return true;
	}

	private bool HandleErase(in PointerInputCmdArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if ((int)args.State != 1)
		{
			return false;
		}
		if (!IsCommander() || !IsViewportHover())
		{
			return false;
		}
		if (IsPlacingLabel)
		{
			CancelLabelPlacement();
			return true;
		}
		if (IsDrawing)
		{
			IsDrawing = false;
			return true;
		}
		MapCoordinates val = _eye.PixelToMap(_input.MouseScreenPosition);
		if (val.MapId == MapId.Nullspace)
		{
			return false;
		}
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivCommanderLineDeleteNearestRequestEvent(val.MapId, val.Position, 2f));
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivCommanderLabelDeleteNearestRequestEvent(val.MapId, val.Position, 2f));
		return true;
	}

	private bool HandleRotate(in PointerInputCmdArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)args.State != 1)
		{
			return false;
		}
		if (!IsCommander() || !IsPlacingLabel)
		{
			return false;
		}
		PendingLabelRotation = NormalizeAngle(PendingLabelRotation + MathF.PI / 2f);
		return true;
	}

	private static float NormalizeAngle(float a)
	{
		a %= MathF.PI * 2f;
		if (a < 0f)
		{
			a += MathF.PI * 2f;
		}
		return a;
	}

	public Vector2 GetCursorWorldPosition()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return _eye.PixelToMap(_input.MouseScreenPosition).Position;
	}

	public bool IsCommander()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
			if (((EntitySystem)this).TryComp<CivTeamMemberComponent>(valueOrDefault, ref civTeamMemberComponent))
			{
				return civTeamMemberComponent.IsCommander;
			}
		}
		return false;
	}

	private bool IsViewportHover()
	{
		if (_ui.CurrentlyHovered != null)
		{
			return _ui.CurrentlyHovered is IViewportControl;
		}
		return true;
	}
}
