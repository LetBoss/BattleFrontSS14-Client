using System;
using Content.Shared._RMC14.Stealth;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Aura;

public abstract class SharedAuraSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	public void GiveAura(EntityUid ent, Color auraColor, TimeSpan? duration, float outlineWidth = 2f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<EntityActiveInvisibleComponent>(ent))
		{
			AuraComponent aura = ((EntitySystem)this).EnsureComp<AuraComponent>(ent);
			aura.Color = auraColor;
			aura.ExpiresAt = _timing.CurTime + duration;
			aura.OutlineWidth = outlineWidth;
			((EntitySystem)this).Dirty(ent, (IComponent)(object)aura, (MetaDataComponent)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<AuraComponent> auraQuery = ((EntitySystem)this).EntityQueryEnumerator<AuraComponent>();
		EntityUid uid = default(EntityUid);
		AuraComponent aura = default(AuraComponent);
		while (auraQuery.MoveNext(ref uid, ref aura))
		{
			if (aura.ExpiresAt.HasValue)
			{
				TimeSpan value = time;
				TimeSpan? expiresAt = aura.ExpiresAt;
				if (!(value < expiresAt))
				{
					((EntitySystem)this).RemCompDeferred<AuraComponent>(uid);
				}
			}
		}
	}
}
