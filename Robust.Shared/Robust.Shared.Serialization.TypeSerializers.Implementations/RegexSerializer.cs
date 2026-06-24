using System;
using System.Text.RegularExpressions;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class RegexSerializer : ITypeSerializer<Regex, ValueDataNode>, ITypeReader<Regex, ValueDataNode>, ITypeValidator<Regex, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Regex, ValueDataNode>, ITypeWriter<Regex>, BaseSerializerInterfaces.ITypeInterface<Regex>, ITypeCopyCreator<Regex>
{
	public Regex Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<Regex>? instanceProvider = null)
	{
		return new Regex(node.Value, RegexOptions.Compiled);
	}

	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		try
		{
			new Regex(node.Value);
		}
		catch (Exception)
		{
			return new ErrorNode(node, "Failed compiling regex.");
		}
		return new ValidatedValueNode(node);
	}

	public DataNode Write(ISerializationManager serializationManager, Regex value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new ValueDataNode(value.ToString());
	}

	public Regex CreateCopy(ISerializationManager serializationManager, Regex source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		return new Regex(source.ToString(), source.Options, source.MatchTimeout);
	}
}
