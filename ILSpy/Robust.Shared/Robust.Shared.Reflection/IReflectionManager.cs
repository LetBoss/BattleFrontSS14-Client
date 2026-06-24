using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Reflection;

[NotContentImplementable]
public interface IReflectionManager
{
	IReadOnlyList<Assembly> Assemblies { get; }

	event EventHandler<ReflectionUpdateEventArgs>? OnAssemblyAdded;

	IEnumerable<Type> GetAllChildren<T>(bool inclusive = false);

	IEnumerable<Type> GetAllChildren(Type baseType, bool inclusive = false);

	Type? GetType(string name);

	Type LooseGetType(string name);

	bool TryLooseGetType(string name, [NotNullWhen(true)] out Type? type);

	IEnumerable<Type> FindTypesWithAttribute<T>() where T : Attribute;

	IEnumerable<Type> FindTypesWithAttribute(Type attributeType);

	void LoadAssemblies(IEnumerable<Assembly> assemblies);

	void LoadAssemblies(params Assembly[] args);

	bool TryParseEnumReference(string reference, [NotNullWhen(true)] out Enum? @enum, bool shouldThrow = true);

	string GetEnumReference(Enum @enum);

	Type? YamlTypeTagLookup(Type baseType, string typeName);

	IEnumerable<Type> FindAllTypes();

	void Initialize();
}
