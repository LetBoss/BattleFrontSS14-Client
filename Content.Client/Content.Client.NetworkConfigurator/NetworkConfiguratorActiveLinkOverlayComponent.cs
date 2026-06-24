using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.NetworkConfigurator;

[RegisterComponent]
public sealed class NetworkConfiguratorActiveLinkOverlayComponent : Component, ISerializationGenerated<NetworkConfiguratorActiveLinkOverlayComponent>, ISerializationGenerated
{
	public HashSet<EntityUid> Devices = new HashSet<EntityUid>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NetworkConfiguratorActiveLinkOverlayComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (NetworkConfiguratorActiveLinkOverlayComponent)(object)val;
		serialization.TryCustomCopy<NetworkConfiguratorActiveLinkOverlayComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NetworkConfiguratorActiveLinkOverlayComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NetworkConfiguratorActiveLinkOverlayComponent target2 = (NetworkConfiguratorActiveLinkOverlayComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NetworkConfiguratorActiveLinkOverlayComponent target2 = (NetworkConfiguratorActiveLinkOverlayComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NetworkConfiguratorActiveLinkOverlayComponent target2 = (NetworkConfiguratorActiveLinkOverlayComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NetworkConfiguratorActiveLinkOverlayComponent Instantiate()
	{
		return new NetworkConfiguratorActiveLinkOverlayComponent();
	}
}
