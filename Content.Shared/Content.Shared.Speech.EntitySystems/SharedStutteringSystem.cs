using System;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Speech.EntitySystems;

public abstract class SharedStutteringSystem : EntitySystem
{
	public static readonly ProtoId<StatusEffectPrototype> StutterKey = ProtoId<StatusEffectPrototype>.op_Implicit("Stutter");

	[Dependency]
	private StatusEffectsSystem _statusEffectsSystem;

	public virtual void DoStutter(EntityUid uid, TimeSpan time, bool refresh, StatusEffectsComponent? status = null)
	{
	}

	public virtual void DoRemoveStutterTime(EntityUid uid, double timeRemoved)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_statusEffectsSystem.TryRemoveTime(uid, ProtoId<StatusEffectPrototype>.op_Implicit(StutterKey), TimeSpan.FromSeconds(timeRemoved));
	}

	public void DoRemoveStutter(EntityUid uid, double timeRemoved)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_statusEffectsSystem.TryRemoveStatusEffect(uid, ProtoId<StatusEffectPrototype>.op_Implicit(StutterKey));
	}
}
