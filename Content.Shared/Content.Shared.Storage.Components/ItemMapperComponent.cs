using System;
using System.Collections.Generic;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared.Storage.Components;

[RegisterComponent]
[Access(new Type[] { typeof(SharedItemMapperSystem) })]
public sealed class ItemMapperComponent : Component, ISerializationGenerated<ItemMapperComponent>, ISerializationGenerated
{
	[DataField("mapLayers", false, 1, false, false, null)]
	public Dictionary<string, SharedMapLayerData> MapLayers = new Dictionary<string, SharedMapLayerData>();

	[DataField("sprite", false, 1, false, false, null)]
	public ResPath? RSIPath;

	[DataField("containerWhitelist", false, 1, false, false, null)]
	public HashSet<string>? ContainerWhitelist;

	[DataField("spriteLayers", false, 1, false, false, null)]
	public List<string> SpriteLayers = new List<string>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ItemMapperComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ItemMapperComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ItemMapperComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<string, SharedMapLayerData> MapLayersTemp = null;
			if (MapLayers == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, SharedMapLayerData>>(MapLayers, ref MapLayersTemp, hookCtx, true, context))
			{
				MapLayersTemp = serialization.CreateCopy<Dictionary<string, SharedMapLayerData>>(MapLayers, hookCtx, context, false);
			}
			target.MapLayers = MapLayersTemp;
			ResPath? RSIPathTemp = null;
			if (!serialization.TryCustomCopy<ResPath?>(RSIPath, ref RSIPathTemp, hookCtx, false, context))
			{
				RSIPathTemp = serialization.CreateCopy<ResPath?>(RSIPath, hookCtx, context, false);
			}
			target.RSIPath = RSIPathTemp;
			HashSet<string> ContainerWhitelistTemp = null;
			if (!serialization.TryCustomCopy<HashSet<string>>(ContainerWhitelist, ref ContainerWhitelistTemp, hookCtx, true, context))
			{
				ContainerWhitelistTemp = serialization.CreateCopy<HashSet<string>>(ContainerWhitelist, hookCtx, context, false);
			}
			target.ContainerWhitelist = ContainerWhitelistTemp;
			List<string> SpriteLayersTemp = null;
			if (SpriteLayers == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(SpriteLayers, ref SpriteLayersTemp, hookCtx, true, context))
			{
				SpriteLayersTemp = serialization.CreateCopy<List<string>>(SpriteLayers, hookCtx, context, false);
			}
			target.SpriteLayers = SpriteLayersTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ItemMapperComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemMapperComponent cast = (ItemMapperComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemMapperComponent cast = (ItemMapperComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemMapperComponent def = (ItemMapperComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ItemMapperComponent Instantiate()
	{
		return new ItemMapperComponent();
	}
}
