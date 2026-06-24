using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

[TypeSerializer]
public sealed class HashSetSerializer<T> : ITypeSerializer<HashSet<T>, SequenceDataNode>, ITypeReader<HashSet<T>, SequenceDataNode>, ITypeValidator<HashSet<T>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<HashSet<T>, SequenceDataNode>, ITypeWriter<HashSet<T>>, BaseSerializerInterfaces.ITypeInterface<HashSet<T>>, ITypeSerializer<FrozenSet<T>, SequenceDataNode>, ITypeReader<FrozenSet<T>, SequenceDataNode>, ITypeValidator<FrozenSet<T>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<FrozenSet<T>, SequenceDataNode>, ITypeWriter<FrozenSet<T>>, BaseSerializerInterfaces.ITypeInterface<FrozenSet<T>>, ITypeSerializer<ImmutableHashSet<T>, SequenceDataNode>, ITypeReader<ImmutableHashSet<T>, SequenceDataNode>, ITypeValidator<ImmutableHashSet<T>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<ImmutableHashSet<T>, SequenceDataNode>, ITypeWriter<ImmutableHashSet<T>>, BaseSerializerInterfaces.ITypeInterface<ImmutableHashSet<T>>, ITypeCopier<HashSet<T>>, ITypeCopyCreator<ImmutableHashSet<T>>, ITypeCopyCreator<FrozenSet<T>>
{
	HashSet<T> ITypeReader<HashSet<T>, SequenceDataNode>.Read(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<HashSet<T>>? instanceProvider)
	{
		HashSet<T> hashSet = ((instanceProvider != null) ? instanceProvider() : new HashSet<T>());
		foreach (DataNode item in node.Sequence)
		{
			hashSet.Add(serializationManager.Read<T>(item, hookCtx, context));
		}
		return hashSet;
	}

	public FrozenSet<T> Read(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<FrozenSet<T>>? instanceProvider = null)
	{
		if (instanceProvider != null)
		{
			dependencies.Resolve<ILogManager>().GetSawmill("szr").Warning("Provided value to a Read-call for a FrozenSet. Ignoring...");
		}
		T[] array = new T[node.Sequence.Count];
		int num = 0;
		foreach (DataNode item in node.Sequence)
		{
			array[num++] = serializationManager.Read<T>(item, hookCtx, context);
		}
		return array.ToFrozenSet();
	}

	ImmutableHashSet<T> ITypeReader<ImmutableHashSet<T>, SequenceDataNode>.Read(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<ImmutableHashSet<T>>? instanceProvider)
	{
		if (instanceProvider != null)
		{
			dependencies.Resolve<ILogManager>().GetSawmill("szr").Warning("Provided value to a Read-call for a ImmutableHashSet. Ignoring...");
		}
		ImmutableHashSet<T>.Builder builder = ImmutableHashSet.CreateBuilder<T>();
		foreach (DataNode item in node.Sequence)
		{
			builder.Add(serializationManager.Read<T>(item, hookCtx, context));
		}
		return builder.ToImmutable();
	}

	public ValidationNode Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		return Validate(serializationManager, node, context);
	}

	ValidationNode ITypeValidator<ImmutableHashSet<T>, SequenceDataNode>.Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return Validate(serializationManager, node, context);
	}

	ValidationNode ITypeValidator<HashSet<T>, SequenceDataNode>.Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return Validate(serializationManager, node, context);
	}

	private ValidationNode Validate(ISerializationManager serializationManager, SequenceDataNode node, ISerializationContext? context)
	{
		List<ValidationNode> list = new List<ValidationNode>();
		foreach (DataNode item in node.Sequence)
		{
			list.Add(serializationManager.ValidateNode<T>(item, context));
		}
		return new ValidatedSequenceNode(list);
	}

	public DataNode Write(ISerializationManager serializationManager, ImmutableHashSet<T> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return Write(serializationManager, value.ToHashSet(), dependencies, alwaysWrite, context);
	}

	public DataNode Write(ISerializationManager serializationManager, FrozenSet<T> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return Write(serializationManager, value.ToHashSet(), dependencies, alwaysWrite, context);
	}

	public DataNode Write(ISerializationManager serializationManager, HashSet<T> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		SequenceDataNode sequenceDataNode = new SequenceDataNode();
		foreach (T item in value)
		{
			sequenceDataNode.Add(serializationManager.WriteValue(item, alwaysWrite, context));
		}
		return sequenceDataNode;
	}

	public void CopyTo(ISerializationManager serializationManager, HashSet<T> source, ref HashSet<T> target, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		target.Clear();
		target.EnsureCapacity(source.Count);
		foreach (T item in source)
		{
			target.Add(serializationManager.CreateCopy(item, hookCtx, context));
		}
	}

	public ImmutableHashSet<T> CreateCopy(ISerializationManager serializationManager, ImmutableHashSet<T> source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HashSet<T> hashSet = new HashSet<T>(source.Count);
		foreach (T item in source)
		{
			hashSet.Add(serializationManager.CreateCopy(item, hookCtx, context));
		}
		return hashSet.ToImmutableHashSet();
	}

	public FrozenSet<T> CreateCopy(ISerializationManager serializationManager, FrozenSet<T> source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		T[] array = new T[source.Count];
		int num = 0;
		foreach (T item in source)
		{
			array[num++] = serializationManager.CreateCopy(item, hookCtx, context);
		}
		return array.ToFrozenSet();
	}
}
