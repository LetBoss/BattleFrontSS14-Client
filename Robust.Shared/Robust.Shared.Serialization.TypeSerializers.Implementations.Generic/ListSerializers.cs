using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
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
public sealed class ListSerializers<T> : ITypeSerializer<List<T>, SequenceDataNode>, ITypeReader<List<T>, SequenceDataNode>, ITypeValidator<List<T>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<List<T>, SequenceDataNode>, ITypeWriter<List<T>>, BaseSerializerInterfaces.ITypeInterface<List<T>>, ITypeSerializer<IReadOnlyList<T>, SequenceDataNode>, ITypeReader<IReadOnlyList<T>, SequenceDataNode>, ITypeValidator<IReadOnlyList<T>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<IReadOnlyList<T>, SequenceDataNode>, ITypeWriter<IReadOnlyList<T>>, BaseSerializerInterfaces.ITypeInterface<IReadOnlyList<T>>, ITypeSerializer<IReadOnlyCollection<T>, SequenceDataNode>, ITypeReader<IReadOnlyCollection<T>, SequenceDataNode>, ITypeValidator<IReadOnlyCollection<T>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<IReadOnlyCollection<T>, SequenceDataNode>, ITypeWriter<IReadOnlyCollection<T>>, BaseSerializerInterfaces.ITypeInterface<IReadOnlyCollection<T>>, ITypeSerializer<ImmutableList<T>, SequenceDataNode>, ITypeReader<ImmutableList<T>, SequenceDataNode>, ITypeValidator<ImmutableList<T>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<ImmutableList<T>, SequenceDataNode>, ITypeWriter<ImmutableList<T>>, BaseSerializerInterfaces.ITypeInterface<ImmutableList<T>>, ITypeCopier<List<T>>, ITypeCopyCreator<IReadOnlyList<T>>, ITypeCopyCreator<IReadOnlyCollection<T>>, ITypeCopyCreator<ImmutableList<T>>
{
	private DataNode WriteInternal(ISerializationManager serializationManager, IEnumerable<T> value, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		SequenceDataNode sequenceDataNode = new SequenceDataNode();
		foreach (T item in value)
		{
			sequenceDataNode.Add(serializationManager.WriteValue(item, alwaysWrite, context));
		}
		return sequenceDataNode;
	}

	public DataNode Write(ISerializationManager serializationManager, ImmutableList<T> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return WriteInternal(serializationManager, value, alwaysWrite, context);
	}

	public DataNode Write(ISerializationManager serializationManager, List<T> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return WriteInternal(serializationManager, value, alwaysWrite, context);
	}

	public DataNode Write(ISerializationManager serializationManager, IReadOnlyCollection<T> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return WriteInternal(serializationManager, value, alwaysWrite, context);
	}

	public DataNode Write(ISerializationManager serializationManager, IReadOnlyList<T> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return WriteInternal(serializationManager, value, alwaysWrite, context);
	}

	List<T> ITypeReader<List<T>, SequenceDataNode>.Read(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<List<T>>? instanceProvider)
	{
		List<T> list = ((instanceProvider != null) ? instanceProvider() : new List<T>());
		foreach (DataNode item in node.Sequence)
		{
			list.Add(serializationManager.Read<T>(item, hookCtx, context));
		}
		return list;
	}

	ValidationNode ITypeValidator<ImmutableList<T>, SequenceDataNode>.Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return Validate(serializationManager, node, context);
	}

	ValidationNode ITypeValidator<IReadOnlyCollection<T>, SequenceDataNode>.Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return Validate(serializationManager, node, context);
	}

	ValidationNode ITypeValidator<IReadOnlyList<T>, SequenceDataNode>.Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return Validate(serializationManager, node, context);
	}

	ValidationNode ITypeValidator<List<T>, SequenceDataNode>.Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return Validate(serializationManager, node, context);
	}

	private ValidationNode Validate(ISerializationManager serializationManager, SequenceDataNode sequenceDataNode, ISerializationContext? context)
	{
		List<ValidationNode> list = new List<ValidationNode>();
		foreach (DataNode item in sequenceDataNode.Sequence)
		{
			list.Add(serializationManager.ValidateNode<T>(item, context));
		}
		return new ValidatedSequenceNode(list);
	}

	IReadOnlyList<T> ITypeReader<IReadOnlyList<T>, SequenceDataNode>.Read(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<IReadOnlyList<T>>? instanceProvider)
	{
		if (instanceProvider != null)
		{
			dependencies.Resolve<ILogManager>().GetSawmill("szr").Warning("Provided value to a Read-call for a IReadOnlySet. Ignoring...");
		}
		List<T> list = new List<T>();
		foreach (DataNode item in node.Sequence)
		{
			list.Add(serializationManager.Read<T>(item, hookCtx, context));
		}
		return list;
	}

	IReadOnlyCollection<T> ITypeReader<IReadOnlyCollection<T>, SequenceDataNode>.Read(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<IReadOnlyCollection<T>>? instanceProvider)
	{
		if (instanceProvider != null)
		{
			dependencies.Resolve<ILogManager>().GetSawmill("szr").Warning("Provided value to a Read-call for a IReadOnlyCollection. Ignoring...");
		}
		List<T> list = new List<T>();
		foreach (DataNode item in node.Sequence)
		{
			list.Add(serializationManager.Read<T>(item, hookCtx, context));
		}
		return list;
	}

	ImmutableList<T> ITypeReader<ImmutableList<T>, SequenceDataNode>.Read(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<ImmutableList<T>>? instanceProvider)
	{
		if (instanceProvider != null)
		{
			dependencies.Resolve<ILogManager>().GetSawmill("szr").Warning("Provided value to a Read-call for a ImmutableList. Ignoring...");
		}
		ImmutableList<T>.Builder builder = ImmutableList.CreateBuilder<T>();
		foreach (DataNode item in node.Sequence)
		{
			builder.Add(serializationManager.Read<T>(item, hookCtx, context));
		}
		return builder.ToImmutable();
	}

	public void CopyTo(ISerializationManager serializationManager, List<T> source, ref List<T> target, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		target.Clear();
		target.EnsureCapacity(source.Count);
		Span<T> span = CollectionsMarshal.AsSpan(source);
		for (int i = 0; i < span.Length; i++)
		{
			ref T reference = ref span[i];
			target.Add(serializationManager.CreateCopy(reference, hookCtx, context));
		}
	}

	public IReadOnlyList<T> CreateCopy(ISerializationManager serializationManager, IReadOnlyList<T> source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		List<T> list = new List<T>(source.Count);
		foreach (T item in source)
		{
			list.Add(serializationManager.CreateCopy(item, hookCtx, context));
		}
		return list;
	}

	public IReadOnlyCollection<T> CreateCopy(ISerializationManager serializationManager, IReadOnlyCollection<T> source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		List<T> list = new List<T>(source.Count);
		foreach (T item in source)
		{
			list.Add(serializationManager.CreateCopy(item, hookCtx, context));
		}
		return list;
	}

	public ImmutableList<T> CreateCopy(ISerializationManager serializationManager, ImmutableList<T> source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		List<T> list = new List<T>(source.Count);
		foreach (T item in source)
		{
			list.Add(serializationManager.CreateCopy(item, hookCtx, context));
		}
		return list.ToImmutableList();
	}
}
