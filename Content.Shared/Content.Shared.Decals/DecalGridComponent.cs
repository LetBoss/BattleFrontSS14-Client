using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Decals;

[RegisterComponent]
[Access(new Type[] { typeof(SharedDecalSystem) })]
[NetworkedComponent]
public sealed class DecalGridComponent : Component, ISerializationGenerated<DecalGridComponent>, ISerializationGenerated
{
	[Serializable]
	[DataDefinition]
	[NetSerializable]
	public sealed class DecalChunk : ISerializationGenerated<DecalChunk>, ISerializationGenerated
	{
		[IncludeDataField(false, 1, false, typeof(DictionarySerializer<uint, Decal>))]
		public Dictionary<uint, Decal> Decals;

		[NonSerialized]
		public GameTick LastModified;

		public DecalChunk()
		{
			Decals = new Dictionary<uint, Decal>();
		}

		public DecalChunk(Dictionary<uint, Decal> decals)
		{
			Decals = decals;
		}

		public DecalChunk(DecalChunk chunk)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			Decals = Extensions.ShallowClone<uint, Decal>(chunk.Decals);
			LastModified = chunk.LastModified;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref DecalChunk target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			if (!serialization.TryCustomCopy<DecalChunk>(this, ref target, hookCtx, false, context))
			{
				if (Decals == null)
				{
					throw new NullNotAllowedException();
				}
				Dictionary<uint, Decal> DecalsTemp = null;
				Dictionary<uint, Decal> DecalsTempCopyTo = null;
				serialization.CopyTo<Dictionary<uint, Decal>, DictionarySerializer<uint, Decal>>(Decals, ref DecalsTempCopyTo, hookCtx, context, true);
				DecalsTemp = DecalsTempCopyTo;
				target.Decals = DecalsTemp;
			}
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref DecalChunk target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			DecalChunk cast = (DecalChunk)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public DecalChunk Instantiate()
		{
			return new DecalChunk();
		}
	}

	[Serializable]
	[DataRecord]
	[NetSerializable]
	public record DecalGridChunkCollection(Dictionary<Vector2i, DecalChunk> ChunkCollection)
	{
		public uint NextDecalId;
	}

	[Access(/*Could not decode attribute arguments.*/)]
	[DataField(null, false, 1, false, true, null)]
	public DecalGridChunkCollection ChunkCollection = new DecalGridChunkCollection(new Dictionary<Vector2i, DecalChunk>());

	public readonly Dictionary<uint, Vector2i> DecalIndex = new Dictionary<uint, Vector2i>();

	public GameTick ForceTick { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DecalGridComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DecalGridComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<DecalGridComponent>(this, ref target, hookCtx, false, context))
		{
			DecalGridChunkCollection ChunkCollectionTemp = null;
			if (ChunkCollection == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<DecalGridChunkCollection>(ChunkCollection, ref ChunkCollectionTemp, hookCtx, true, context))
			{
				ChunkCollectionTemp = serialization.CreateCopy<DecalGridChunkCollection>(ChunkCollection, hookCtx, context, false);
			}
			target.ChunkCollection = ChunkCollectionTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DecalGridComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DecalGridComponent cast = (DecalGridComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DecalGridComponent cast = (DecalGridComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DecalGridComponent def = (DecalGridComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DecalGridComponent Instantiate()
	{
		return new DecalGridComponent();
	}
}
