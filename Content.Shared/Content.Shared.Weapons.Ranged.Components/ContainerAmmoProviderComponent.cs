using System;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[Access(new Type[] { typeof(SharedGunSystem) })]
public sealed class ContainerAmmoProviderComponent : AmmoProviderComponent, ISerializationGenerated<ContainerAmmoProviderComponent>, ISerializationGenerated
{
	[DataField("container", false, 1, true, false, null)]
	[ViewVariables]
	public string Container;

	[DataField("provider", false, 1, false, false, null)]
	[ViewVariables]
	public EntityUid? ProviderUid;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ContainerAmmoProviderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		AmmoProviderComponent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ContainerAmmoProviderComponent)definitionCast;
		if (!serialization.TryCustomCopy<ContainerAmmoProviderComponent>(this, ref target, hookCtx, false, context))
		{
			string ContainerTemp = null;
			if (Container == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Container, ref ContainerTemp, hookCtx, false, context))
			{
				ContainerTemp = Container;
			}
			target.Container = ContainerTemp;
			EntityUid? ProviderUidTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(ProviderUid, ref ProviderUidTemp, hookCtx, false, context))
			{
				ProviderUidTemp = serialization.CreateCopy<EntityUid?>(ProviderUid, hookCtx, context, false);
			}
			target.ProviderUid = ProviderUidTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ContainerAmmoProviderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref AmmoProviderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerAmmoProviderComponent cast = (ContainerAmmoProviderComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerAmmoProviderComponent cast = (ContainerAmmoProviderComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerAmmoProviderComponent def = (ContainerAmmoProviderComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ContainerAmmoProviderComponent Instantiate()
	{
		return new ContainerAmmoProviderComponent();
	}
}
