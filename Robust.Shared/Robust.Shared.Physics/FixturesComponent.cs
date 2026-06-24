using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Physics;

[RegisterComponent]
[NetworkedComponent]
public sealed class FixturesComponent : Component, ISerializationGenerated<FixturesComponent>, ISerializationGenerated
{
	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("fixtures", false, 1, false, false, typeof(FixtureSerializer))]
	[NeverPushInheritance]
	[Access(new Type[] { typeof(FixtureSystem) }, Other = AccessPermissions.ReadExecute)]
	public Dictionary<string, Fixture> Fixtures = new Dictionary<string, Fixture>();

	[ViewVariables]
	public int FixtureCount => Fixtures.Count;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FixturesComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (FixturesComponent)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			if (Fixtures == null)
			{
				throw new NullNotAllowedException();
			}
			Dictionary<string, Fixture> dictionary = null;
			Dictionary<string, Fixture> target3 = null;
			serialization.CopyTo<Dictionary<string, Fixture>, FixtureSerializer>(Fixtures, ref target3, hookCtx, context, notNullableOverride: true);
			dictionary = target3;
			target.Fixtures = dictionary;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FixturesComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FixturesComponent target2 = (FixturesComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FixturesComponent target2 = (FixturesComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FixturesComponent target2 = (FixturesComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FixturesComponent Instantiate()
	{
		return new FixturesComponent();
	}
}
