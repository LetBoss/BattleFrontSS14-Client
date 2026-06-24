using Content.Shared.CombatMode;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Weapons.Melee;

public sealed class MeleeSpreadCommand : LocalizedEntityCommands
{
	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IOverlayManager _overlay;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private MeleeWeaponSystem _meleeSystem;

	[Dependency]
	private SharedCombatModeSystem _combatSystem;

	[Dependency]
	private SharedTransformSystem _transformSystem;

	public override string Command => "showmeleespread";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (!_overlay.RemoveOverlay<MeleeArcOverlay>())
		{
			_overlay.AddOverlay((Overlay)(object)new MeleeArcOverlay((IEntityManager)(object)base.EntityManager, _eyeManager, _inputManager, _playerManager, _meleeSystem, _combatSystem, _transformSystem));
		}
	}
}
