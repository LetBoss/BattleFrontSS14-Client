using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Materials;

[RegisterComponent]
public sealed class CollideMaterialReclaimerComponent : Component, ISerializationGenerated<CollideMaterialReclaimerComponent>, ISerializationGenerated
{
	[DataField("fixtureId", false, 1, false, false, null)]
	public string FixtureId = "brrt";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CollideMaterialReclaimerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CollideMaterialReclaimerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CollideMaterialReclaimerComponent>(this, ref target, hookCtx, false, context))
		{
			string FixtureIdTemp = null;
			if (FixtureId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(FixtureId, ref FixtureIdTemp, hookCtx, false, context))
			{
				FixtureIdTemp = FixtureId;
			}
			target.FixtureId = FixtureIdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CollideMaterialReclaimerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CollideMaterialReclaimerComponent cast = (CollideMaterialReclaimerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CollideMaterialReclaimerComponent cast = (CollideMaterialReclaimerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CollideMaterialReclaimerComponent def = (CollideMaterialReclaimerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CollideMaterialReclaimerComponent Instantiate()
	{
		return new CollideMaterialReclaimerComponent();
	}
}
