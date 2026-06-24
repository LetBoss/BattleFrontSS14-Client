using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using NetSerializer;
using Robust.Shared.RichText;

namespace Robust.Shared.Serialization;

internal sealed class NetFormattedStringSerializer : IStaticTypeSerializer, ITypeSerializer
{
	public bool Handles(Type type)
	{
		return type == typeof(FormattedString);
	}

	public IEnumerable<Type> GetSubtypes(Type type)
	{
		return new _003C_003Ez__ReadOnlySingleElementList<Type>(typeof(string));
	}

	public MethodInfo GetStaticWriter(Type type)
	{
		return typeof(NetFormattedStringSerializer).GetMethod("Write", BindingFlags.Static | BindingFlags.NonPublic);
	}

	public MethodInfo GetStaticReader(Type type)
	{
		return typeof(NetFormattedStringSerializer).GetMethod("Read", BindingFlags.Static | BindingFlags.NonPublic);
	}

	private static void Write(Stream stream, FormattedString value)
	{
		Primitives.WritePrimitive(stream, value.Markup);
	}

	private static void Read(Stream stream, out FormattedString value)
	{
		Unsafe.SkipInit(out string markup);
		Primitives.ReadPrimitive(stream, ref markup);
		value = FormattedString.FromMarkup(markup);
	}
}
