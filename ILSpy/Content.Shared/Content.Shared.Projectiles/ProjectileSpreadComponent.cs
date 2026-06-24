using System;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Projectiles;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedGunSystem) })]
public sealed class ProjectileSpreadComponent : Component, ISerializationGenerated<ProjectileSpreadComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public EntProtoId Proto;

	[DataField(null, false, 1, false, false, null)]
	public Angle Spread = Angle.FromDegrees(5.0);

	[DataField(null, false, 1, false, false, null)]
	public int Count = 1;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ProjectileSpreadComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ProjectileSpreadComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ProjectileSpreadComponent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId ProtoTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Proto, ref ProtoTemp, hookCtx, false, context))
			{
				ProtoTemp = serialization.CreateCopy<EntProtoId>(Proto, hookCtx, context, false);
			}
			target.Proto = ProtoTemp;
			Angle SpreadTemp = default(Angle);
			if (!serialization.TryCustomCopy<Angle>(Spread, ref SpreadTemp, hookCtx, false, context))
			{
				SpreadTemp = serialization.CreateCopy<Angle>(Spread, hookCtx, context, false);
			}
			target.Spread = SpreadTemp;
			int CountTemp = 0;
			if (!serialization.TryCustomCopy<int>(Count, ref CountTemp, hookCtx, false, context))
			{
				CountTemp = Count;
			}
			target.Count = CountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ProjectileSpreadComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ProjectileSpreadComponent cast = (ProjectileSpreadComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ProjectileSpreadComponent cast = (ProjectileSpreadComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ProjectileSpreadComponent def = (ProjectileSpreadComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ProjectileSpreadComponent Instantiate()
	{
		return new ProjectileSpreadComponent();
	}
}
