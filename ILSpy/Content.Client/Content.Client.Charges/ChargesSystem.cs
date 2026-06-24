using System.Collections.Generic;
using Content.Client.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Charges;

public sealed class ChargesSystem : SharedChargesSystem
{
	[Dependency]
	private ActionsSystem _actions;

	private Dictionary<EntityUid, int> _lastCharges = new Dictionary<EntityUid, int>();

	private Dictionary<EntityUid, int> _tempLastCharges = new Dictionary<EntityUid, int>();

	public override void Update(float frameTime)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		AllEntityQueryEnumerator<AutoRechargeComponent, LimitedChargesComponent> val = ((EntitySystem)this).AllEntityQuery<AutoRechargeComponent, LimitedChargesComponent>();
		EntityUid val2 = default(EntityUid);
		AutoRechargeComponent item = default(AutoRechargeComponent);
		LimitedChargesComponent item2 = default(LimitedChargesComponent);
		while (val.MoveNext(ref val2, ref item, ref item2))
		{
			Entity<ActionComponent>? action = _actions.GetAction(Entity<ActionComponent>.op_Implicit(val2), logError: false);
			if (action.HasValue)
			{
				Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
				int currentCharges = GetCurrentCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit((val2, item2, item)));
				if (!_lastCharges.TryGetValue(val2, out var value) || currentCharges != value)
				{
					_actions.UpdateAction(valueOrDefault);
				}
				_tempLastCharges[val2] = currentCharges;
			}
		}
		_lastCharges.Clear();
		foreach (var (key, value2) in _tempLastCharges)
		{
			_lastCharges[key] = value2;
		}
		_tempLastCharges.Clear();
	}
}
