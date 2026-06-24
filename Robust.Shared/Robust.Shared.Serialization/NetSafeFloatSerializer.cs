using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NetSerializer;

namespace Robust.Shared.Serialization;

internal sealed class NetSafeFloatSerializer : IStaticTypeSerializer, ITypeSerializer
{
	public bool Handles(Type type)
	{
		if (!(type == typeof(float)) && !(type == typeof(double)))
		{
			return type == typeof(Half);
		}
		return true;
	}

	public IEnumerable<Type> GetSubtypes(Type type)
	{
		return Array.Empty<Type>();
	}

	public MethodInfo GetStaticWriter(Type type)
	{
		return typeof(Primitives).GetMethod("WritePrimitive", BindingFlags.Static | BindingFlags.Public, new Type[2]
		{
			typeof(Stream),
			type
		});
	}

	public MethodInfo GetStaticReader(Type type)
	{
		return typeof(SafePrimitives).GetMethod("ReadPrimitive", BindingFlags.Static | BindingFlags.Public, new Type[2]
		{
			typeof(Stream),
			type.MakeByRefType()
		});
	}
}
