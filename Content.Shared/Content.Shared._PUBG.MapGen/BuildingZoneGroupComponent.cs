using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._PUBG.MapGen;

[RegisterComponent]
public sealed class BuildingZoneGroupComponent : Component, ISerializationGenerated<BuildingZoneGroupComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string GroupPath { get; set; } = "";

	[DataField(null, false, 1, false, false, null)]
	public bool CanRotate { get; set; } = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BuildingZoneGroupComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (BuildingZoneGroupComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<BuildingZoneGroupComponent>(this, ref target, hookCtx, false, context))
		{
			string GroupPathTemp = null;
			if (GroupPath == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(GroupPath, ref GroupPathTemp, hookCtx, false, context))
			{
				GroupPathTemp = GroupPath;
			}
			target.GroupPath = GroupPathTemp;
			bool CanRotateTemp = false;
			if (!serialization.TryCustomCopy<bool>(CanRotate, ref CanRotateTemp, hookCtx, false, context))
			{
				CanRotateTemp = CanRotate;
			}
			target.CanRotate = CanRotateTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BuildingZoneGroupComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BuildingZoneGroupComponent cast = (BuildingZoneGroupComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BuildingZoneGroupComponent cast = (BuildingZoneGroupComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BuildingZoneGroupComponent def = (BuildingZoneGroupComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override BuildingZoneGroupComponent Instantiate()
	{
		return new BuildingZoneGroupComponent();
	}
}
