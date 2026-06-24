using System.Numerics;
using Content.Shared.Examine;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.Popups;

public sealed class PopupOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> UnshadedShader = ProtoId<ShaderPrototype>.op_Implicit("unshaded");

	private readonly IConfigurationManager _configManager;

	private readonly IEntityManager _entManager;

	private readonly IPlayerManager _playerMgr;

	private readonly IUserInterfaceManager _uiManager;

	private readonly PopupSystem _popup;

	private readonly PopupUIController _controller;

	private readonly ExamineSystemShared _examine;

	private readonly SharedTransformSystem _transform;

	private readonly ShaderInstance _shader;

	public override OverlaySpace Space => (OverlaySpace)2;

	public PopupOverlay(IConfigurationManager configManager, IEntityManager entManager, IPlayerManager playerMgr, IPrototypeManager protoManager, IUserInterfaceManager uiManager, PopupUIController controller, ExamineSystemShared examine, SharedTransformSystem transform, PopupSystem popup)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		_configManager = configManager;
		_entManager = entManager;
		_playerMgr = playerMgr;
		_uiManager = uiManager;
		_examine = examine;
		_transform = transform;
		_popup = popup;
		_controller = controller;
		_shader = protoManager.Index<ShaderPrototype>(UnshadedShader).Instance();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (args.ViewportControl != null)
		{
			DrawingHandleBase drawingHandle = args.DrawingHandle;
			Matrix3x2 identity = Matrix3x2.Identity;
			drawingHandle.SetTransform(ref identity);
			args.DrawingHandle.UseShader(_shader);
			float num = _configManager.GetCVar<float>(CVars.DisplayUIScale);
			if (num == 0f)
			{
				num = _uiManager.DefaultUIScale;
			}
			DrawWorld(((OverlayDrawArgs)(ref args)).ScreenHandle, args, num);
			args.DrawingHandle.UseShader((ShaderInstance)null);
		}
	}

	private void DrawWorld(DrawingHandleScreen worldHandle, OverlayDrawArgs args, float scale)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		if (_popup.WorldLabels.Count == 0 || args.ViewportControl == null)
		{
			return;
		}
		Matrix3x2 worldToScreenMatrix = args.ViewportControl.GetWorldToScreenMatrix();
		EntityUid? ourEntity = ((ISharedPlayerManager)_playerMgr).LocalEntity;
		MapCoordinates mapCoordinates = default(MapCoordinates);
		((MapCoordinates)(ref mapCoordinates))._002Ector(((Box2)(ref args.WorldAABB)).Center, args.MapId);
		Vector2 vector = ((Box2Rotated)(ref args.WorldBounds)).Center;
		if (ourEntity.HasValue)
		{
			mapCoordinates = _transform.GetMapCoordinates(ourEntity.Value, (TransformComponent)null);
			vector = mapCoordinates.Position;
		}
		foreach (PopupSystem.WorldPopupLabel popup in _popup.WorldLabels)
		{
			MapCoordinates val = _transform.ToMapCoordinates(popup.InitialPos, true);
			if (!(val.MapId != args.MapId))
			{
				float range = (val.Position - vector).Length();
				if (((Box2Rotated)(ref args.WorldBounds)).Contains(val.Position) && _examine.InRangeUnOccluded(mapCoordinates, val, range, delegate(EntityUid e)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0019: Unknown result type (might be due to invalid IL or missing references)
					//IL_0031: Unknown result type (might be due to invalid IL or missing references)
					//IL_0034: Unknown result type (might be due to invalid IL or missing references)
					if (!(e == popup.InitialPos.EntityId))
					{
						EntityUid? val2 = ourEntity;
						if (!val2.HasValue)
						{
							return false;
						}
						return e == val2.GetValueOrDefault();
					}
					return true;
				}, ignoreInsideBlocker: true, _entManager))
				{
					Vector2 position = Vector2.Transform(val.Position, worldToScreenMatrix);
					_controller.DrawPopup(popup, worldHandle, position, scale);
				}
			}
		}
	}
}
