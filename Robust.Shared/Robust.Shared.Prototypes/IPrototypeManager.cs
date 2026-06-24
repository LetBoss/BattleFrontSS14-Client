using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Robust.Shared.Analyzers;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Utility;

namespace Robust.Shared.Prototypes;

[NotContentImplementable]
public interface IPrototypeManager
{
	FrozenDictionary<ProtoId<EntityCategoryPrototype>, IReadOnlyList<EntityPrototype>> Categories { get; }

	event Action<PrototypesReloadedEventArgs> PrototypesReloaded;

	void Initialize();

	IEnumerable<string> GetPrototypeKinds();

	int Count<T>() where T : class, IPrototype;

	IEnumerable<T> EnumeratePrototypes<T>() where T : class, IPrototype;

	IEnumerable<IPrototype> EnumeratePrototypes(Type kind);

	IEnumerable<IPrototype> EnumeratePrototypes(string variant);

	IEnumerable<T> EnumerateParents<T>(T proto, bool includeSelf = false) where T : class, IPrototype, IInheritingPrototype;

	IEnumerable<T> EnumerateParents<T>(string id, bool includeSelf = false) where T : class, IPrototype, IInheritingPrototype;

	IEnumerable<IPrototype> EnumerateParents(Type kind, string id, bool includeSelf = false);

	IEnumerable<(string id, T?)> EnumerateAllParents<T>(string id, bool includeSelf = false) where T : class, IPrototype, IInheritingPrototype;

	IEnumerable<Type> EnumeratePrototypeKinds();

	T Index<T>([ForbidLiteral] string id) where T : class, IPrototype;

	EntityPrototype Index([ForbidLiteral] EntProtoId id);

	T Index<T>([ForbidLiteral] ProtoId<T> id) where T : class, IPrototype;

	IPrototype Index(Type kind, [ForbidLiteral] string id);

	bool HasIndex<T>([ForbidLiteral] string id) where T : class, IPrototype;

	bool HasIndex([ForbidLiteral] EntProtoId id);

	bool HasIndex<T>([ForbidLiteral] ProtoId<T> id) where T : class, IPrototype;

	bool HasIndex([ForbidLiteral] EntProtoId? id);

	bool HasIndex<T>([ForbidLiteral] ProtoId<T>? id) where T : class, IPrototype;

	bool TryIndex<T>([ForbidLiteral] string id, [NotNullWhen(true)] out T? prototype) where T : class, IPrototype;

	bool TryIndex(Type kind, [ForbidLiteral] string id, [NotNullWhen(true)] out IPrototype? prototype);

	bool TryGetInstances<T>([NotNullWhen(true)] out FrozenDictionary<string, T>? instances) where T : IPrototype;

	FrozenDictionary<string, T> GetInstances<T>() where T : IPrototype;

	bool Resolve([ForbidLiteral] EntProtoId id, [NotNullWhen(true)] out EntityPrototype? prototype);

	[Obsolete("Use Resolve() if you want to get a prototype without throwing but while still logging an error.")]
	bool TryIndex([ForbidLiteral] EntProtoId id, [NotNullWhen(true)] out EntityPrototype? prototype, bool logError = true);

	bool TryIndex([ForbidLiteral] EntProtoId id, [NotNullWhen(true)] out EntityPrototype? prototype);

	bool Resolve<T>([ForbidLiteral] ProtoId<T> id, [NotNullWhen(true)] out T? prototype) where T : class, IPrototype;

	[Obsolete("Use Resolve() if you want to get a prototype without throwing but while still logging an error.")]
	bool TryIndex<T>([ForbidLiteral] ProtoId<T> id, [NotNullWhen(true)] out T? prototype, bool logError = true) where T : class, IPrototype;

	bool TryIndex<T>([ForbidLiteral] ProtoId<T> id, [NotNullWhen(true)] out T? prototype) where T : class, IPrototype;

	bool Resolve([ForbidLiteral] EntProtoId? id, [NotNullWhen(true)] out EntityPrototype? prototype);

	[Obsolete("Use Resolve() if you want to get a prototype without throwing but while still logging an error.")]
	bool TryIndex([ForbidLiteral] EntProtoId? id, [NotNullWhen(true)] out EntityPrototype? prototype, bool logError = true);

	bool TryIndex([ForbidLiteral] EntProtoId? id, [NotNullWhen(true)] out EntityPrototype? prototype);

	bool Resolve<T>([ForbidLiteral] ProtoId<T>? id, [NotNullWhen(true)] out T? prototype) where T : class, IPrototype;

	[Obsolete("Use Resolve() if you want to get a prototype without throwing but while still logging an error.")]
	bool TryIndex<T>([ForbidLiteral] ProtoId<T>? id, [NotNullWhen(true)] out T? prototype, bool logError = true) where T : class, IPrototype;

	bool TryIndex<T>([ForbidLiteral] ProtoId<T>? id, [NotNullWhen(true)] out T? prototype) where T : class, IPrototype;

	bool HasMapping<T>(string id);

	bool TryGetMapping(Type kind, string id, [NotNullWhen(true)] out MappingDataNode? mappings);

	bool HasKind(string kind);

	Type GetKindType(string kind);

	bool TryGetKindType(string kind, [NotNullWhen(true)] out Type? prototype);

	bool TryGetKindFrom(Type type, [NotNullWhen(true)] out string? kind);

	bool TryGetKindFrom(IPrototype prototype, [NotNullWhen(true)] out string? kind);

	bool TryGetKindFrom<T>([NotNullWhen(true)] out string? kind) where T : class, IPrototype;

	void LoadDirectory(ResPath path, bool overwrite = false, Dictionary<Type, HashSet<string>>? changed = null);

	Dictionary<string, HashSet<ErrorNode>> ValidateDirectory(ResPath path);

	Dictionary<string, HashSet<ErrorNode>> ValidateDirectory(ResPath path, out Dictionary<Type, HashSet<string>> prototypes);

	List<string> ValidateStaticFields(Dictionary<Type, HashSet<string>> prototypes);

	List<string> ValidateStaticFields(Type type, Dictionary<Type, HashSet<string>> prototypes);

	Dictionary<Type, Dictionary<string, HashSet<ErrorNode>>> ValidateAllPrototypesSerializable(ISerializationContext? ctx);

	void LoadFromStream(TextReader stream, bool overwrite = false, Dictionary<Type, HashSet<string>>? changed = null);

	void LoadString(string str, bool overwrite = false, Dictionary<Type, HashSet<string>>? changed = null);

	void RemoveString(string prototypes);

	void Clear();

	void ReloadPrototypeKinds();

	void Reset();

	void LoadDefaultPrototypes(Dictionary<Type, HashSet<string>>? loaded = null);

	void ResolveResults();

	void ReloadPrototypes(Dictionary<Type, HashSet<string>> modified, Dictionary<Type, HashSet<string>>? removed = null);

	void RegisterIgnore(string name);

	bool IsIgnored(string name);

	void RegisterKind(params Type[] kinds);

	IReadOnlyDictionary<string, MappingDataNode> GetPrototypeData(EntityPrototype prototype);

	void AbstractFile(ResPath path);

	void AbstractDirectory(ResPath path);

	bool TryGetRandom<T>(IRobustRandom random, [NotNullWhen(true)] out IPrototype? prototype) where T : class, IPrototype;
}
