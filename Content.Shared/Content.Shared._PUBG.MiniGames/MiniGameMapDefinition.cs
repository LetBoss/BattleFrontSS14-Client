using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._PUBG.MiniGames;

[DataDefinition]
public sealed class MiniGameMapDefinition : ISerializationGenerated<MiniGameMapDefinition>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string Id { get; private set; } = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	public string Name { get; private set; } = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	public string Path { get; private set; } = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public string Description { get; private set; } = string.Empty;

	public MiniGameMapDefinition()
	{
	}

	public MiniGameMapDefinition(string id, string name, string path, string description = "")
	{
		Id = id;
		Name = name;
		Path = path;
		Description = description;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MiniGameMapDefinition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<MiniGameMapDefinition>(this, ref target, hookCtx, false, context))
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
			target.Id = IdTemp;
			string NameTemp = null;
			if (Name == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Name, ref NameTemp, hookCtx, false, context))
			{
				NameTemp = Name;
			}
			target.Name = NameTemp;
			string PathTemp = null;
			if (Path == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Path, ref PathTemp, hookCtx, false, context))
			{
				PathTemp = Path;
			}
			target.Path = PathTemp;
			string DescriptionTemp = null;
			if (Description == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Description, ref DescriptionTemp, hookCtx, false, context))
			{
				DescriptionTemp = Description;
			}
			target.Description = DescriptionTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MiniGameMapDefinition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MiniGameMapDefinition cast = (MiniGameMapDefinition)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public MiniGameMapDefinition Instantiate()
	{
		return new MiniGameMapDefinition();
	}
}
