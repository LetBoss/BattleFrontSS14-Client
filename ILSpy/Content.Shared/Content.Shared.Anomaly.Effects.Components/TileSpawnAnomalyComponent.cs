using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Anomaly.Effects.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedTileAnomalySystem) })]
public sealed class TileSpawnAnomalyComponent : Component, ISerializationGenerated<TileSpawnAnomalyComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<TileSpawnSettingsEntry> Entries = new List<TileSpawnSettingsEntry>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TileSpawnAnomalyComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (TileSpawnAnomalyComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<TileSpawnAnomalyComponent>(this, ref target, hookCtx, false, context))
		{
			List<TileSpawnSettingsEntry> EntriesTemp = null;
			if (Entries == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<TileSpawnSettingsEntry>>(Entries, ref EntriesTemp, hookCtx, true, context))
			{
				EntriesTemp = serialization.CreateCopy<List<TileSpawnSettingsEntry>>(Entries, hookCtx, context, false);
			}
			target.Entries = EntriesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TileSpawnAnomalyComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TileSpawnAnomalyComponent cast = (TileSpawnAnomalyComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TileSpawnAnomalyComponent cast = (TileSpawnAnomalyComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TileSpawnAnomalyComponent def = (TileSpawnAnomalyComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override TileSpawnAnomalyComponent Instantiate()
	{
		return new TileSpawnAnomalyComponent();
	}
}
