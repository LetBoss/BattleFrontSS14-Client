using System;
using System.Collections.Generic;
using Content.Shared.Maps;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Shared.Tiles;

[RegisterComponent]
[NetworkedComponent]
public sealed class FloorTileComponent : Component, ISerializationGenerated<FloorTileComponent>, ISerializationGenerated
{
	[DataField("outputs", false, 1, false, false, typeof(PrototypeIdListSerializer<ContentTileDefinition>))]
	public List<string>? OutputTiles;

	[DataField("placeTileSound", false, 1, false, false, null)]
	public SoundSpecifier PlaceTileSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Items/genhit.ogg", (AudioParams?)null)
	{
		Params = ((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.125f)
	};

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FloorTileComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FloorTileComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<FloorTileComponent>(this, ref target, hookCtx, false, context))
		{
			List<string> OutputTilesTemp = null;
			if (!serialization.TryCustomCopy<List<string>>(OutputTiles, ref OutputTilesTemp, hookCtx, true, context))
			{
				OutputTilesTemp = serialization.CreateCopy<List<string>>(OutputTiles, hookCtx, context, false);
			}
			target.OutputTiles = OutputTilesTemp;
			SoundSpecifier PlaceTileSoundTemp = null;
			if (PlaceTileSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(PlaceTileSound, ref PlaceTileSoundTemp, hookCtx, true, context))
			{
				PlaceTileSoundTemp = serialization.CreateCopy<SoundSpecifier>(PlaceTileSound, hookCtx, context, false);
			}
			target.PlaceTileSound = PlaceTileSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FloorTileComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FloorTileComponent cast = (FloorTileComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FloorTileComponent cast = (FloorTileComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FloorTileComponent def = (FloorTileComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FloorTileComponent Instantiate()
	{
		return new FloorTileComponent();
	}
}
