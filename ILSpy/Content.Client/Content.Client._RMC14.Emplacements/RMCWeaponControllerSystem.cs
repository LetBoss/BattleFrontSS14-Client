using System.Diagnostics.CodeAnalysis;
using Content.Shared._RMC14.Emplacements;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Emplacements;

public sealed class RMCWeaponControllerSystem : RMCSharedWeaponControllerSystem
{
	[Dependency]
	private IPlayerManager _playerManager;

	public bool TryGetControllingWeapon([NotNullWhen(true)] out EntityUid? weapon)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		weapon = null;
		if (!localEntity.HasValue)
		{
			return false;
		}
		GunComponent gunComp;
		return TryGetControlledWeapon(localEntity.Value, out weapon, out gunComp);
	}
}
