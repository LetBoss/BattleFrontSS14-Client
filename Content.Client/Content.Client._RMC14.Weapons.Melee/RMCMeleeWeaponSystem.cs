using Content.Client.Weapons.Melee;
using Content.Shared._RMC14.Input;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared.Weapons.Melee;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Weapons.Melee;

public sealed class RMCMeleeWeaponSystem : SharedRMCMeleeWeaponSystem
{
	[Dependency]
	private IEyeManager _eye;

	[Dependency]
	private IInputManager _input;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private MapSystem _map;

	[Dependency]
	private MeleeWeaponSystem _melee;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private TransformSystem _transform;

	public override void Initialize()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		base.Initialize();
		CommandBinds.Builder.Bind(CMKeyFunctions.CMXenoWideSwing, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate(ICommonSession? session)
		{
			if (session != null && session.AttachedEntity.HasValue)
			{
				TryPrimaryHeavyAttack();
			}
		}, (StateInputCmdDelegate)null, false, true)).Register<RMCMeleeWeaponSystem>();
	}

	private void TryPrimaryHeavyAttack()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		MapCoordinates val = _eye.PixelToMap(_input.MouseScreenPosition);
		EntityUid val2 = default(EntityUid);
		MapGridComponent val3 = default(MapGridComponent);
		EntityUid val4;
		if (_mapManager.TryFindGridAt(val, ref val2, ref val3))
		{
			val4 = val2;
		}
		else
		{
			EntityUid? val5 = default(EntityUid?);
			if (!((SharedMapSystem)_map).TryGetMap((MapId?)val.MapId, ref val5))
			{
				return;
			}
			val4 = val5.Value;
		}
		EntityCoordinates coordinates = ((SharedTransformSystem)_transform).ToCoordinates(Entity<TransformComponent>.op_Implicit(val4), val);
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			if (_melee.TryGetWeapon(valueOrDefault, out EntityUid weaponUid, out MeleeWeaponComponent melee) && melee.WidePrimary)
			{
				_melee.ClientHeavyAttack(valueOrDefault, coordinates, weaponUid, melee);
			}
		}
	}
}
