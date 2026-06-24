using System;
using Content.Client.Hands.Systems;
using Content.Shared.Hands.Components;
using Content.Shared.RCD;
using Content.Shared.RCD.Components;
using Robust.Client.Placement;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.RCD;

public sealed class RCDConstructionGhostSystem : EntitySystem
{
	private const string PlacementMode = "AlignRCDConstruction";

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IPlacementManager _placementManager;

	[Dependency]
	private IPrototypeManager _protoManager;

	[Dependency]
	private HandsSystem _hands;

	private Direction _placementDirection;

	public override void Update(float frameTime)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Expected O, but got Unknown
		((EntitySystem)this).Update(frameTime);
		PlacementInformation currentPermission = _placementManager.CurrentPermission;
		EntityUid? val = ((currentPermission != null) ? new EntityUid?(currentPermission.MobUid) : ((EntityUid?)null));
		PlacementInformation currentPermission2 = _placementManager.CurrentPermission;
		string text = ((currentPermission2 != null) ? currentPermission2.EntityType : null);
		bool flag = ((EntitySystem)this).HasComp<RCDComponent>(val);
		if (_placementManager.Eraser || (val.HasValue && !flag))
		{
			return;
		}
		ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
		EntityUid? val2 = ((localSession != null) ? localSession.AttachedEntity : ((EntityUid?)null));
		if (!val2.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = val2.GetValueOrDefault();
		EntityUid? activeItem = _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(valueOrDefault));
		RCDComponent rCDComponent = default(RCDComponent);
		if (!((EntitySystem)this).TryComp<RCDComponent>(activeItem, ref rCDComponent))
		{
			if (flag)
			{
				_placementManager.Clear();
			}
			return;
		}
		RCDPrototype rCDPrototype = _protoManager.Index<RCDPrototype>(rCDComponent.ProtoId);
		if (_placementDirection != _placementManager.Direction)
		{
			_placementDirection = _placementManager.Direction;
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RCDConstructionGhostRotationEvent(((EntitySystem)this).GetNetEntity(activeItem.Value, (MetaDataComponent)null), _placementDirection));
		}
		val2 = activeItem;
		EntityUid? val3 = val;
		if (val2.HasValue != val3.HasValue || (val2.HasValue && !(val2.GetValueOrDefault() == val3.GetValueOrDefault())) || !(rCDPrototype.Prototype == text))
		{
			PlacementInformation val4 = new PlacementInformation
			{
				MobUid = activeItem.Value,
				PlacementOption = "AlignRCDConstruction",
				EntityType = rCDPrototype.Prototype,
				Range = (int)Math.Ceiling(1.5),
				IsTile = (rCDPrototype.Mode == RcdMode.ConstructTile),
				UseEditorContext = false
			};
			_placementManager.Clear();
			_placementManager.BeginPlacing(val4, (PlacementHijack)null);
		}
	}
}
