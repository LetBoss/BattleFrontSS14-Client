using System;
using Content.Shared.Alert;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;

namespace Content.Shared.EntityEffects.Effects;

public sealed class AdjustAlert : EntityEffect, ISerializationGenerated<AdjustAlert>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public ProtoId<AlertPrototype> AlertType;

	[DataField(null, false, 1, false, false, null)]
	public bool Clear;

	[DataField(null, false, 1, false, false, null)]
	public bool ShowCooldown;

	[DataField(null, false, 1, false, false, null)]
	public float Time;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return null;
	}

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		AlertsSystem alertSys = args.EntityManager.EntitySysManager.GetEntitySystem<AlertsSystem>();
		if (!args.EntityManager.HasComponent<AlertsComponent>(args.TargetEntity))
		{
			return;
		}
		if (Clear && Time <= 0f)
		{
			alertSys.ClearAlert(args.TargetEntity, AlertType);
			return;
		}
		IGameTiming timing = IoCManager.Resolve<IGameTiming>();
		(TimeSpan, TimeSpan)? cooldown = null;
		if ((ShowCooldown || Clear) && Time > 0f)
		{
			cooldown = (timing.CurTime, timing.CurTime + TimeSpan.FromSeconds(Time));
		}
		EntityUid targetEntity = args.TargetEntity;
		ProtoId<AlertPrototype> alertType = AlertType;
		(TimeSpan, TimeSpan)? cooldown2 = cooldown;
		bool clear = Clear;
		bool showCooldown = ShowCooldown;
		alertSys.ShowAlert(targetEntity, alertType, null, cooldown2, clear, showCooldown);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AdjustAlert target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AdjustAlert)definitionCast;
		if (!serialization.TryCustomCopy<AdjustAlert>(this, ref target, hookCtx, false, context))
		{
			ProtoId<AlertPrototype> AlertTypeTemp = default(ProtoId<AlertPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(AlertType, ref AlertTypeTemp, hookCtx, false, context))
			{
				AlertTypeTemp = serialization.CreateCopy<ProtoId<AlertPrototype>>(AlertType, hookCtx, context, false);
			}
			target.AlertType = AlertTypeTemp;
			bool ClearTemp = false;
			if (!serialization.TryCustomCopy<bool>(Clear, ref ClearTemp, hookCtx, false, context))
			{
				ClearTemp = Clear;
			}
			target.Clear = ClearTemp;
			bool ShowCooldownTemp = false;
			if (!serialization.TryCustomCopy<bool>(ShowCooldown, ref ShowCooldownTemp, hookCtx, false, context))
			{
				ShowCooldownTemp = ShowCooldown;
			}
			target.ShowCooldown = ShowCooldownTemp;
			float TimeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Time, ref TimeTemp, hookCtx, false, context))
			{
				TimeTemp = Time;
			}
			target.Time = TimeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AdjustAlert target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AdjustAlert cast = (AdjustAlert)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AdjustAlert cast = (AdjustAlert)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AdjustAlert Instantiate()
	{
		return new AdjustAlert();
	}
}
