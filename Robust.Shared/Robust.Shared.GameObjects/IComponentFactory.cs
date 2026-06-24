using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Analyzers;
using Robust.Shared.Prototypes;

namespace Robust.Shared.GameObjects;

[NotContentImplementable]
public interface IComponentFactory
{
	IEnumerable<Type> AllRegisteredTypes { get; }

	IReadOnlyList<ComponentRegistration>? NetworkedComponents { get; }

	event Action<ComponentRegistration[]> ComponentsAdded;

	event Action<string> ComponentIgnoreAdded;

	ComponentAvailability GetComponentAvailability(string componentName, bool ignoreCase = false);

	void RegisterNetworkedFields<T>(params string[] fields) where T : IComponent;

	void RegisterNetworkedFields(ComponentRegistration compReg, params string[] fields);

	CompIdx GetIndex(Type type);

	int GetArrayIndex(Type type);

	void RegisterClass<T>(bool overwrite = false) where T : IComponent, new();

	void RegisterTypes(params Type[] type);

	void RegisterIgnore(params string[] names);

	void IgnoreMissingComponents(string postfix = "");

	IComponent GetComponent(EntityPrototype.ComponentRegistryEntry entry);

	IComponent GetComponent(Type componentType);

	IComponent GetComponent(CompIdx componentType);

	T GetComponent<T>() where T : IComponent, new();

	IComponent GetComponent(ComponentRegistration reg);

	IComponent GetComponent(string componentName, bool ignoreCase = false);

	IComponent GetComponent(ushort netId);

	string GetComponentName(Type componentType);

	string GetComponentName<T>() where T : IComponent, new();

	string GetComponentName(ushort netID);

	ComponentRegistration GetRegistration(string componentName, bool ignoreCase = false);

	ComponentRegistration GetRegistration(Type reference);

	ComponentRegistration GetRegistration<T>() where T : IComponent, new();

	ComponentRegistration GetRegistration(ushort netID);

	ComponentRegistration GetRegistration(IComponent component);

	ComponentRegistration GetRegistration(CompIdx idx);

	bool IsIgnored(string componentName);

	bool TryGetRegistration(string componentName, [NotNullWhen(true)] out ComponentRegistration? registration, bool ignoreCase = false);

	bool TryGetRegistration(Type reference, [NotNullWhen(true)] out ComponentRegistration? registration);

	bool TryGetRegistration<T>([NotNullWhen(true)] out ComponentRegistration? registration) where T : IComponent, new();

	bool TryGetRegistration(ushort netID, [NotNullWhen(true)] out ComponentRegistration? registration);

	bool TryGetRegistration(IComponent component, [NotNullWhen(true)] out ComponentRegistration? registration);

	void DoAutoRegistrations();

	IEnumerable<ComponentRegistration> GetAllRegistrations();

	IEnumerable<CompIdx> GetAllRefTypes();

	void GenerateNetIds();

	Type IdxToType(CompIdx idx);

	byte[] GetHash(bool networkedOnly);
}
