using System.Collections.Generic;
using Content.Shared._RMC14.Connection;
using Content.Shared._RMC14.Medical.HUD.Components;
using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared.Damage;
using Content.Shared.Mobs.Systems;
using Content.Shared.StatusIcon;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.Medical.HUD;

public sealed class CMHealthIconsSystem : EntitySystem
{
	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private RMCUnrevivableSystem _unrevivable;

	private static readonly ProtoId<HealthIconPrototype> BaseDeadIcon = ProtoId<HealthIconPrototype>.op_Implicit("CMHealthIconDead");

	public StatusIconData GetDeadIcon()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _prototype.Index<HealthIconPrototype>(BaseDeadIcon);
	}

	public IReadOnlyList<StatusIconData> GetIcons(Entity<DamageableComponent> damageable)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		List<StatusIconData> list = new List<StatusIconData>();
		RMCHealthIconTypes key = RMCHealthIconTypes.Healthy;
		RMCHealthIconsComponent rMCHealthIconsComponent = default(RMCHealthIconsComponent);
		if (!((EntitySystem)this).TryComp<RMCHealthIconsComponent>(Entity<DamageableComponent>.op_Implicit(damageable), ref rMCHealthIconsComponent))
		{
			return list;
		}
		if (_mobState.IsDead(Entity<DamageableComponent>.op_Implicit(damageable)))
		{
			int unrevivableStage = _unrevivable.GetUnrevivableStage(Entity<RMCRevivableComponent>.op_Implicit(damageable.Owner), 4);
			MindCheckComponent mindCheckComponent = default(MindCheckComponent);
			if (_unrevivable.IsUnrevivable(Entity<DamageableComponent>.op_Implicit(damageable)))
			{
				key = RMCHealthIconTypes.Dead;
			}
			else if (((EntitySystem)this).TryComp<MindCheckComponent>(Entity<DamageableComponent>.op_Implicit(damageable), ref mindCheckComponent) && !mindCheckComponent.ActiveMindOrGhost)
			{
				key = RMCHealthIconTypes.DeadDNR;
			}
			else if (unrevivableStage <= 1)
			{
				key = RMCHealthIconTypes.DeadDefib;
			}
			else
			{
				switch (unrevivableStage)
				{
				case 2:
					key = RMCHealthIconTypes.DeadClose;
					break;
				case 3:
					key = RMCHealthIconTypes.DeadAlmost;
					break;
				}
			}
		}
		if (rMCHealthIconsComponent.Icons.TryGetValue(key, out ProtoId<HealthIconPrototype> value))
		{
			list.Add(_prototype.Index<HealthIconPrototype>(value));
		}
		return list;
	}
}
