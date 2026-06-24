using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

namespace Content.Shared.Decals;

[TypeSerializer]
public sealed class DecalGridChunkCollectionTypeSerializer : ITypeSerializer<DecalGridComponent.DecalGridChunkCollection, MappingDataNode>, ITypeReader<DecalGridComponent.DecalGridChunkCollection, MappingDataNode>, ITypeValidator<DecalGridComponent.DecalGridChunkCollection, MappingDataNode>, ITypeNodeInterface<DecalGridComponent.DecalGridChunkCollection, MappingDataNode>, ITypeWriter<DecalGridComponent.DecalGridChunkCollection>, ITypeInterface<DecalGridComponent.DecalGridChunkCollection>
{
	[DataDefinition]
	private readonly struct DecalData : IEquatable<DecalData>, IComparable<DecalData>, ISerializationGenerated<DecalData>, ISerializationGenerated
	{
		[DataField("id", false, 1, false, false, null)]
		public string Id { get; init; }

		[DataField("color", false, 1, false, false, null)]
		public Color? Color { get; init; }

		[DataField("angle", false, 1, false, false, null)]
		public Angle Angle { get; init; }

		[DataField("zIndex", false, 1, false, false, null)]
		public int ZIndex { get; init; }

		[DataField("cleanable", false, 1, false, false, null)]
		public bool Cleanable { get; init; }

		public DecalData(string id, Color? color, Angle angle, int zIndex, bool cleanable)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			Id = string.Empty;
			Angle = Angle.Zero;
			Id = id;
			Color = color;
			Angle = angle;
			ZIndex = zIndex;
			Cleanable = cleanable;
		}

		public DecalData(Decal decal)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			Id = string.Empty;
			Angle = Angle.Zero;
			Id = decal.Id;
			Color = decal.Color;
			Angle = decal.Angle;
			ZIndex = decal.ZIndex;
			Cleanable = decal.Cleanable;
		}

		public bool Equals(DecalData other)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			if (Id == other.Id && Nullable.Equals<Color>(Color, other.Color))
			{
				Angle angle = Angle;
				if (((Angle)(ref angle)).Equals(other.Angle) && ZIndex == other.ZIndex)
				{
					return Cleanable == other.Cleanable;
				}
			}
			return false;
		}

		public override bool Equals(object? obj)
		{
			if (obj is DecalData other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			return HashCode.Combine<string, Color?, Angle, int, bool>(Id, Color, Angle, ZIndex, Cleanable);
		}

		public int CompareTo(DecalData other)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			int idComparison = string.Compare(Id, other.Id, StringComparison.Ordinal);
			if (idComparison != 0)
			{
				return idComparison;
			}
			Color? color = Color;
			object strA;
			Color valueOrDefault;
			if (!color.HasValue)
			{
				strA = null;
			}
			else
			{
				valueOrDefault = color.GetValueOrDefault();
				strA = ((Color)(ref valueOrDefault)).ToHex();
			}
			color = other.Color;
			object strB;
			if (!color.HasValue)
			{
				strB = null;
			}
			else
			{
				valueOrDefault = color.GetValueOrDefault();
				strB = ((Color)(ref valueOrDefault)).ToHex();
			}
			int colorComparison = string.Compare((string?)strA, (string?)strB, StringComparison.Ordinal);
			if (colorComparison != 0)
			{
				return colorComparison;
			}
			int angleComparison = Angle.Theta.CompareTo(other.Angle.Theta);
			if (angleComparison != 0)
			{
				return angleComparison;
			}
			int zIndexComparison = ZIndex.CompareTo(other.ZIndex);
			if (zIndexComparison != 0)
			{
				return zIndexComparison;
			}
			return Cleanable.CompareTo(other.Cleanable);
		}

		public DecalData()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			Color = null;
			ZIndex = 0;
			Cleanable = false;
			Id = string.Empty;
			Angle = Angle.Zero;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref DecalData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			if (!serialization.TryCustomCopy<DecalData>(this, ref target, hookCtx, false, context))
			{
				string IdTemp = null;
				if (Id == null)
				{
					throw new NullNotAllowedException();
				}
				if (!serialization.TryCustomCopy<string>(Id, ref IdTemp, hookCtx, false, context))
				{
					IdTemp = Id;
				}
				Color? ColorTemp = null;
				if (!serialization.TryCustomCopy<Color?>(Color, ref ColorTemp, hookCtx, false, context))
				{
					ColorTemp = serialization.CreateCopy<Color?>(Color, hookCtx, context, false);
				}
				Angle AngleTemp = default(Angle);
				if (!serialization.TryCustomCopy<Angle>(Angle, ref AngleTemp, hookCtx, false, context))
				{
					AngleTemp = serialization.CreateCopy<Angle>(Angle, hookCtx, context, false);
				}
				int ZIndexTemp = 0;
				if (!serialization.TryCustomCopy<int>(ZIndex, ref ZIndexTemp, hookCtx, false, context))
				{
					ZIndexTemp = ZIndex;
				}
				bool CleanableTemp = false;
				if (!serialization.TryCustomCopy<bool>(Cleanable, ref CleanableTemp, hookCtx, false, context))
				{
					CleanableTemp = Cleanable;
				}
				DecalData decalData = target;
				decalData.Id = IdTemp;
				decalData.Color = ColorTemp;
				decalData.Angle = AngleTemp;
				decalData.ZIndex = ZIndexTemp;
				decalData.Cleanable = CleanableTemp;
				target = decalData;
			}
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref DecalData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			DecalData cast = (DecalData)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public DecalData Instantiate()
		{
			return new DecalData();
		}
	}

	public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		return serializationManager.ValidateNode<Dictionary<Vector2i, Dictionary<uint, Decal>>>((DataNode)(object)node, context);
	}

	public DecalGridComponent.DecalGridChunkCollection Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, InstantiationDelegate<DecalGridComponent.DecalGridChunkCollection>? _ = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		DataNode versionNode = default(DataNode);
		node.TryGetValue("version", ref versionNode);
		ValueDataNode val = (ValueDataNode)versionNode;
		int num = (((int)val == 0) ? 1 : val.AsInt());
		uint nextIndex = 0u;
		HashSet<uint> ids = new HashSet<uint>();
		Dictionary<Vector2i, DecalGridComponent.DecalChunk> dictionary;
		if (num > 1)
		{
			SequenceDataNode val2 = (SequenceDataNode)node["nodes"];
			dictionary = new Dictionary<Vector2i, DecalGridComponent.DecalChunk>();
			foreach (MappingDataNode item in val2)
			{
				MappingDataNode aNode = item;
				DecalData data = serializationManager.Read<DecalData>(aNode["node"], hookCtx, context, (InstantiationDelegate<DecalData>)null, false);
				foreach (KeyValuePair<string, DataNode> item2 in (MappingDataNode)aNode["decals"])
				{
					item2.Deconstruct(out var key, out var value);
					string s = key;
					DataNode decalData = value;
					uint dUid = uint.Parse(s, CultureInfo.InvariantCulture);
					Vector2 vector = serializationManager.Read<Vector2>(decalData, hookCtx, context, (InstantiationDelegate<Vector2>)null, false);
					Vector2i chunkOrigin = SharedMapSystem.GetChunkIndices(vector, 32);
					DecalGridComponent.DecalChunk chunk = Extensions.GetOrNew<Vector2i, DecalGridComponent.DecalChunk>(dictionary, chunkOrigin);
					Decal decal = new Decal(vector, data.Id, data.Color, data.Angle, data.ZIndex, data.Cleanable);
					nextIndex = Math.Max(nextIndex, dUid);
					if (!ids.Add(dUid))
					{
						dUid = nextIndex++;
						ids.Add(dUid);
					}
					chunk.Decals[dUid] = decal;
				}
			}
		}
		else
		{
			dictionary = serializationManager.Read<Dictionary<Vector2i, DecalGridComponent.DecalChunk>>((DataNode)(object)node, hookCtx, context, (InstantiationDelegate<Dictionary<Vector2i, DecalGridComponent.DecalChunk>>)null, true);
			foreach (DecalGridComponent.DecalChunk value2 in dictionary.Values)
			{
				foreach (uint key2 in value2.Decals.Keys)
				{
					nextIndex = Math.Max(key2, nextIndex);
				}
			}
		}
		nextIndex++;
		return new DecalGridComponent.DecalGridChunkCollection(dictionary)
		{
			NextDecalId = nextIndex
		};
	}

	public DataNode Write(ISerializationManager serializationManager, DecalGridComponent.DecalGridChunkCollection value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		Dictionary<DecalData, List<uint>> lookup = new Dictionary<DecalData, List<uint>>();
		Dictionary<uint, Decal> decalLookup = new Dictionary<uint, Decal>();
		MappingDataNode allData = new MappingDataNode();
		SequenceDataNode nodes = new SequenceDataNode();
		foreach (DecalGridComponent.DecalChunk value3 in value.ChunkCollection.Values)
		{
			foreach (KeyValuePair<uint, Decal> decal3 in value3.Decals)
			{
				decal3.Deconstruct(out var key, out var value2);
				uint uid = key;
				Decal decal = value2;
				DecalData data = new DecalData(decal);
				Extensions.GetOrNew<DecalData, List<uint>>(lookup, data).Add(uid);
				decalLookup[uid] = decal;
			}
		}
		List<DecalData> list = lookup.Keys.ToList();
		list.Sort();
		foreach (DecalData data2 in list)
		{
			List<uint> list2 = lookup[data2];
			MappingDataNode val = new MappingDataNode();
			val.Add("node", serializationManager.WriteValue<DecalData>(data2, alwaysWrite, context, false));
			MappingDataNode lookupNode = val;
			MappingDataNode decks = new MappingDataNode();
			list2.Sort();
			foreach (uint uid2 in list2)
			{
				Decal decal2 = decalLookup[uid2];
				decks.Add(uid2.ToString(), serializationManager.WriteValue<Vector2>(decal2.Coordinates, alwaysWrite, context, false));
			}
			lookupNode.Add("decals", (DataNode)(object)decks);
			nodes.Add((DataNode)(object)lookupNode);
		}
		MappingDataNodeExtensions.Add(allData, "version", 2.ToString(CultureInfo.InvariantCulture));
		allData.Add("nodes", (DataNode)(object)nodes);
		return (DataNode)(object)allData;
	}
}
