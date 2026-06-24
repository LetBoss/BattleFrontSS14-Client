using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(VehicleSystem) })]
public sealed class VehicleEnterComponent : Component, ISerializationGenerated<VehicleEnterComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public ResPath InteriorPath;

	[DataField(null, false, 1, false, false, null)]
	public int MaxPassengers;

	[DataField(null, false, 1, false, false, null)]
	public int MaxXenos;

	[DataField(null, false, 1, false, false, null)]
	public List<VehicleEntryPoint> EntryPoints = new List<VehicleEntryPoint>();

	[DataField(null, false, 1, false, false, null)]
	public float EnterDoAfter;

	[DataField(null, false, 1, false, false, null)]
	public float ExitDoAfter;

	[DataField(null, false, 1, false, false, null)]
	public Vector2 ExitOffset = Vector2.Zero;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleEnterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehicleEnterComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<VehicleEnterComponent>(this, ref target, hookCtx, false, context))
		{
			ResPath InteriorPathTemp = default(ResPath);
			if (!serialization.TryCustomCopy<ResPath>(InteriorPath, ref InteriorPathTemp, hookCtx, false, context))
			{
				InteriorPathTemp = serialization.CreateCopy<ResPath>(InteriorPath, hookCtx, context, false);
			}
			target.InteriorPath = InteriorPathTemp;
			int MaxPassengersTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxPassengers, ref MaxPassengersTemp, hookCtx, false, context))
			{
				MaxPassengersTemp = MaxPassengers;
			}
			target.MaxPassengers = MaxPassengersTemp;
			int MaxXenosTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxXenos, ref MaxXenosTemp, hookCtx, false, context))
			{
				MaxXenosTemp = MaxXenos;
			}
			target.MaxXenos = MaxXenosTemp;
			List<VehicleEntryPoint> EntryPointsTemp = null;
			if (EntryPoints == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<VehicleEntryPoint>>(EntryPoints, ref EntryPointsTemp, hookCtx, true, context))
			{
				EntryPointsTemp = serialization.CreateCopy<List<VehicleEntryPoint>>(EntryPoints, hookCtx, context, false);
			}
			target.EntryPoints = EntryPointsTemp;
			float EnterDoAfterTemp = 0f;
			if (!serialization.TryCustomCopy<float>(EnterDoAfter, ref EnterDoAfterTemp, hookCtx, false, context))
			{
				EnterDoAfterTemp = EnterDoAfter;
			}
			target.EnterDoAfter = EnterDoAfterTemp;
			float ExitDoAfterTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ExitDoAfter, ref ExitDoAfterTemp, hookCtx, false, context))
			{
				ExitDoAfterTemp = ExitDoAfter;
			}
			target.ExitDoAfter = ExitDoAfterTemp;
			Vector2 ExitOffsetTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(ExitOffset, ref ExitOffsetTemp, hookCtx, false, context))
			{
				ExitOffsetTemp = serialization.CreateCopy<Vector2>(ExitOffset, hookCtx, context, false);
			}
			target.ExitOffset = ExitOffsetTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleEnterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleEnterComponent cast = (VehicleEnterComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleEnterComponent cast = (VehicleEnterComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleEnterComponent def = (VehicleEnterComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleEnterComponent Instantiate()
	{
		return new VehicleEnterComponent();
	}
}
