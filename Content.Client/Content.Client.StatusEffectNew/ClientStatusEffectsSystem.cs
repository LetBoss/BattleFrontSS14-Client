using System;
using Content.Shared.StatusEffectNew;
using Content.Shared.StatusEffectNew.Components;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Client.StatusEffectNew;

public sealed class ClientStatusEffectsSystem : SharedStatusEffectsSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<StatusEffectContainerComponent, ComponentHandleState>((EntityEventRefHandler<StatusEffectContainerComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
	}

	private void OnHandleState(Entity<StatusEffectContainerComponent> ent, ref ComponentHandleState args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ComponentHandleState)(ref args)).Current is StatusEffectContainerComponentState statusEffectContainerComponentState))
		{
			return;
		}
		ValueList<EntityUid> val = default(ValueList<EntityUid>);
		NetEntity? val2 = default(NetEntity?);
		foreach (EntityUid activeStatusEffect in ent.Comp.ActiveStatusEffects)
		{
			if (((EntitySystem)this).TryGetNetEntity(activeStatusEffect, ref val2, (MetaDataComponent)null) && !statusEffectContainerComponentState.ActiveStatusEffects.Contains(val2.Value))
			{
				val.Add(activeStatusEffect);
			}
		}
		Enumerator<EntityUid> enumerator2 = val.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				EntityUid current2 = enumerator2.Current;
				ent.Comp.ActiveStatusEffects.Remove(current2);
				StatusEffectRemovedEvent statusEffectRemovedEvent = new StatusEffectRemovedEvent(Entity<StatusEffectContainerComponent>.op_Implicit(ent));
				((EntitySystem)this).RaiseLocalEvent<StatusEffectRemovedEvent>(current2, ref statusEffectRemovedEvent, false);
			}
		}
		finally
		{
			((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
		}
		foreach (NetEntity activeStatusEffect2 in statusEffectContainerComponentState.ActiveStatusEffects)
		{
			EntityUid entity = ((EntitySystem)this).GetEntity(activeStatusEffect2);
			if (!ent.Comp.ActiveStatusEffects.Contains(entity))
			{
				ent.Comp.ActiveStatusEffects.Add(entity);
				StatusEffectAppliedEvent statusEffectAppliedEvent = new StatusEffectAppliedEvent(Entity<StatusEffectContainerComponent>.op_Implicit(ent));
				((EntitySystem)this).RaiseLocalEvent<StatusEffectAppliedEvent>(entity, ref statusEffectAppliedEvent, false);
			}
		}
	}
}
