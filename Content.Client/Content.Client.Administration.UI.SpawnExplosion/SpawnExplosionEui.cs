using System.Collections.Generic;
using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.SpawnExplosion;

public sealed class SpawnExplosionEui : BaseEui
{
	[Dependency]
	private EntityManager _entManager;

	[Dependency]
	private IOverlayManager _overlayManager;

	private readonly SpawnExplosionWindow _window;

	private ExplosionDebugOverlay? _debugOverlay;

	public SpawnExplosionEui()
	{
		IoCManager.InjectDependencies<SpawnExplosionEui>(this);
		_window = new SpawnExplosionWindow(this);
		((BaseWindow)_window).OnClose += SendClosedMessage;
	}

	public override void Opened()
	{
		base.Opened();
		((BaseWindow)_window).OpenCentered();
	}

	public override void Closed()
	{
		base.Closed();
		((BaseWindow)_window).OnClose -= SendClosedMessage;
		((BaseWindow)_window).Close();
		ClearOverlay();
	}

	public void SendClosedMessage()
	{
		SendMessage(new CloseEuiMessage());
	}

	public void ClearOverlay()
	{
		if (_overlayManager.HasOverlay<ExplosionDebugOverlay>())
		{
			_overlayManager.RemoveOverlay<ExplosionDebugOverlay>();
		}
		_debugOverlay = null;
	}

	public void RequestPreviewData(MapCoordinates epicenter, string typeId, float totalIntensity, float intensitySlope, float maxIntensity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		SpawnExplosionEuiMsg.PreviewRequest msg = new SpawnExplosionEuiMsg.PreviewRequest(epicenter, typeId, totalIntensity, intensitySlope, maxIntensity);
		SendMessage(msg);
	}

	public override void HandleMessage(EuiMessageBase msg)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		if (!(msg is SpawnExplosionEuiMsg.PreviewData previewData))
		{
			return;
		}
		if (_debugOverlay == null)
		{
			_debugOverlay = new ExplosionDebugOverlay();
			_overlayManager.AddOverlay((Overlay)(object)_debugOverlay);
		}
		Dictionary<EntityUid, Dictionary<int, List<Vector2i>>> dictionary = new Dictionary<EntityUid, Dictionary<int, List<Vector2i>>>();
		_debugOverlay.Tiles.Clear();
		foreach (var (val2, value) in previewData.Explosion.Tiles)
		{
			dictionary[_entManager.GetEntity(val2)] = value;
		}
		_debugOverlay.Tiles = dictionary;
		_debugOverlay.SpaceTiles = previewData.Explosion.SpaceTiles;
		_debugOverlay.Intensity = previewData.Explosion.Intensity;
		_debugOverlay.Slope = previewData.Slope;
		_debugOverlay.TotalIntensity = previewData.TotalIntensity;
		_debugOverlay.Map = previewData.Explosion.Epicenter.MapId;
		_debugOverlay.SpaceMatrix = previewData.Explosion.SpaceMatrix;
		_debugOverlay.SpaceTileSize = previewData.Explosion.SpaceTileSize;
	}
}
