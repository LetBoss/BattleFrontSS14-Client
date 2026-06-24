using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using YamlDotNet.RepresentationModel;

namespace Robust.Shared.Utility;

[Serializable]
[NetSerializable]
public abstract class SpriteSpecifier
{
	[Serializable]
	[NetSerializable]
	[DataDefinition]
	public sealed class Rsi : SpriteSpecifier, ISerializationGenerated<Rsi>, ISerializationGenerated
	{
		[DataField("sprite", false, 1, false, false, null)]
		public ResPath RsiPath { get; internal set; }

		[DataField("state", false, 1, false, false, null)]
		public string RsiState { get; internal set; }

		public Rsi(ResPath rsiPath, string rsiState)
		{
			RsiPath = rsiPath;
			RsiState = rsiState;
		}

		public override bool Equals(object? obj)
		{
			if (obj is Rsi rsi && rsi.RsiPath == RsiPath)
			{
				return rsi.RsiState == RsiState;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return RsiPath.GetHashCode() ^ RsiState.GetHashCode();
		}

		public Rsi()
		{
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref Rsi target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
			{
				ResPath target2 = default(ResPath);
				if (!serialization.TryCustomCopy(RsiPath, ref target2, hookCtx, hasHooks: false, context))
				{
					target2 = serialization.CreateCopy(RsiPath, hookCtx, context);
				}
				target.RsiPath = target2;
				string target3 = null;
				if (RsiState == null)
				{
					throw new NullNotAllowedException();
				}
				if (!serialization.TryCustomCopy(RsiState, ref target3, hookCtx, hasHooks: false, context))
				{
					target3 = RsiState;
				}
				target.RsiState = target3;
			}
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref Rsi target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			Rsi target2 = (Rsi)target;
			Copy(ref target2, serialization, hookCtx, context);
			target = target2;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public Rsi Instantiate()
		{
			return new Rsi();
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class Texture : SpriteSpecifier
	{
		public ResPath TexturePath { get; internal set; }

		private Texture()
		{
			TexturePath = default(ResPath);
		}

		public Texture(ResPath texturePath)
		{
			TexturePath = texturePath;
		}

		public override bool Equals(object? obj)
		{
			if (obj is Texture texture)
			{
				return texture.TexturePath == TexturePath;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return TexturePath.GetHashCode();
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class EntityPrototype : SpriteSpecifier
	{
		public readonly string EntityPrototypeId;

		public EntityPrototype(string entityPrototypeId)
		{
			EntityPrototypeId = entityPrototypeId;
		}

		public override bool Equals(object? obj)
		{
			if (obj is EntityPrototype entityPrototype)
			{
				return EntityPrototypeId == entityPrototype.EntityPrototypeId;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return EntityPrototypeId.GetHashCode();
		}
	}

	public static readonly SpriteSpecifier Invalid = new Texture(ResPath.Self);

	public static SpriteSpecifier FromYaml(YamlNode node)
	{
		if (node is YamlScalarNode)
		{
			return new Texture(node.AsResourcePath());
		}
		YamlMappingNode val = (YamlMappingNode)(object)((node is YamlMappingNode) ? node : null);
		if (val != null)
		{
			return new Rsi(((YamlNode)val)[YamlNode.op_Implicit("sprite")].AsResourcePath(), ((YamlNode)val)[YamlNode.op_Implicit("state")].AsString());
		}
		throw new InvalidOperationException();
	}
}
