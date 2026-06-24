using System;
using Content.Shared.Climbing.Systems;
using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Climbing.Components;

[RegisterComponent]
[Access(new Type[] { typeof(ClimbSystem) })]
public sealed class GlassTableComponent : Component, ISerializationGenerated<GlassTableComponent>, ISerializationGenerated
{
	[DataField("climberDamage", false, 1, false, false, null)]
	public DamageSpecifier ClimberDamage;

	[DataField("tableDamage", false, 1, false, false, null)]
	public DamageSpecifier TableDamage;

	[DataField("tableMassLimit", false, 1, false, false, null)]
	public float MassLimit;

	public float StunTime = 2f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GlassTableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GlassTableComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<GlassTableComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		DamageSpecifier ClimberDamageTemp = null;
		if (ClimberDamage == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DamageSpecifier>(ClimberDamage, ref ClimberDamageTemp, hookCtx, false, context))
		{
			if (ClimberDamage == null)
			{
				ClimberDamageTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(ClimberDamage, ref ClimberDamageTemp, hookCtx, context, true);
			}
		}
		target.ClimberDamage = ClimberDamageTemp;
		DamageSpecifier TableDamageTemp = null;
		if (TableDamage == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DamageSpecifier>(TableDamage, ref TableDamageTemp, hookCtx, false, context))
		{
			if (TableDamage == null)
			{
				TableDamageTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(TableDamage, ref TableDamageTemp, hookCtx, context, true);
			}
		}
		target.TableDamage = TableDamageTemp;
		float MassLimitTemp = 0f;
		if (!serialization.TryCustomCopy<float>(MassLimit, ref MassLimitTemp, hookCtx, false, context))
		{
			MassLimitTemp = MassLimit;
		}
		target.MassLimit = MassLimitTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GlassTableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GlassTableComponent cast = (GlassTableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GlassTableComponent cast = (GlassTableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GlassTableComponent def = (GlassTableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GlassTableComponent Instantiate()
	{
		return new GlassTableComponent();
	}
}
