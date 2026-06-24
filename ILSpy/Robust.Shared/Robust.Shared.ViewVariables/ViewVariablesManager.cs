using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.Markdown;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Robust.Shared.ViewVariables;

internal abstract class ViewVariablesManager : IViewVariablesManager, IPostInjectInit
{
	[DataDefinition]
	private sealed class VvTest : IEnumerable<object>, IEnumerable, ISerializationGenerated<VvTest>, ISerializationGenerated
	{
		[DataDefinition]
		private struct ComplexDataStructure : ISerializationGenerated<ComplexDataStructure>, ISerializationGenerated
		{
			[DataField("X", false, 1, false, false, null)]
			[ViewVariables(VVAccess.ReadWrite)]
			public int X = 5;

			[DataField("Y", false, 1, false, false, null)]
			[ViewVariables(VVAccess.ReadWrite)]
			public int Y = 15;

			public ComplexDataStructure()
			{
			}

			[Obsolete("Use ISerializationManager.CopyTo instead")]
			public void InternalCopy(ref ComplexDataStructure target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
			{
				if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
				{
					int target2 = 0;
					if (!serialization.TryCustomCopy(X, ref target2, hookCtx, hasHooks: false, context))
					{
						target2 = X;
					}
					int target3 = 0;
					if (!serialization.TryCustomCopy(Y, ref target3, hookCtx, hasHooks: false, context))
					{
						target3 = Y;
					}
					ComplexDataStructure complexDataStructure = target;
					complexDataStructure.X = target2;
					complexDataStructure.Y = target3;
					target = complexDataStructure;
				}
			}

			[Obsolete("Use ISerializationManager.CopyTo instead")]
			public void Copy(ref ComplexDataStructure target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
			{
				InternalCopy(ref target, serialization, hookCtx, context);
			}

			[Obsolete("Use ISerializationManager.CopyTo instead")]
			public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
			{
				ComplexDataStructure target2 = (ComplexDataStructure)target;
				Copy(ref target2, serialization, hookCtx, context);
				target = target2;
			}

			[Obsolete("Use ISerializationManager.CreateCopy instead")]
			public ComplexDataStructure Instantiate()
			{
				return new ComplexDataStructure();
			}
		}

		[DataField("x", false, 1, false, false, null)]
		[ViewVariables(VVAccess.ReadWrite)]
		private int X = 10;

		[ViewVariables]
		public Dictionary<object, object> Dict = new Dictionary<object, object>
		{
			{ "a", "b" },
			{ "c", "d" }
		};

		[DataField("multiDimensionalArray", false, 1, false, false, null)]
		public int[,] MultiDimensionalArray = new int[5, 2]
		{
			{ 1, 2 },
			{ 3, 4 },
			{ 5, 6 },
			{ 7, 8 },
			{ 9, 0 }
		};

		[DataField("vector", false, 1, false, false, null)]
		[ViewVariables(VVAccess.ReadWrite)]
		private Vector2 Vector = new Vector2(50f, 50f);

		[DataField("data", false, 1, false, false, null)]
		[ViewVariables(VVAccess.ReadWrite)]
		private ComplexDataStructure Data = new ComplexDataStructure();

		[ViewVariables]
		public List<object> List => new List<object>
		{
			1, 2, 3, 4, 5, 6, 7, 8, 9, X,
			11, 12, 13, 14, 15, this
		};

		public IEnumerator<object> GetEnumerator()
		{
			return List.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref VvTest target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
			{
				int target2 = 0;
				if (!serialization.TryCustomCopy(X, ref target2, hookCtx, hasHooks: false, context))
				{
					target2 = X;
				}
				target.X = target2;
				int[,] target3 = null;
				if (MultiDimensionalArray == null)
				{
					throw new NullNotAllowedException();
				}
				if (!serialization.TryCustomCopy(MultiDimensionalArray, ref target3, hookCtx, hasHooks: true, context))
				{
					target3 = serialization.CreateCopy(MultiDimensionalArray, hookCtx, context);
				}
				target.MultiDimensionalArray = target3;
				Vector2 target4 = default(Vector2);
				if (!serialization.TryCustomCopy(Vector, ref target4, hookCtx, hasHooks: false, context))
				{
					target4 = serialization.CreateCopy(Vector, hookCtx, context);
				}
				target.Vector = target4;
				ComplexDataStructure target5 = default(ComplexDataStructure);
				if (!serialization.TryCustomCopy(Data, ref target5, hookCtx, hasHooks: false, context))
				{
					serialization.CopyTo(Data, ref target5, hookCtx, context);
				}
				target.Data = target5;
			}
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref VvTest target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			VvTest target2 = (VvTest)target;
			Copy(ref target2, serialization, hookCtx, context);
			target = target2;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public VvTest Instantiate()
		{
			return new VvTest();
		}
	}

	internal sealed class DomainData
	{
		public readonly DomainResolveObject ResolveObject;

		public readonly DomainListPaths List;

		public DomainData(DomainResolveObject resolveObject, DomainListPaths list)
		{
			ResolveObject = resolveObject;
			List = list;
		}
	}

	[Dependency]
	private readonly ISerializationManager _serMan;

	[Dependency]
	private readonly IEntityManager _entMan;

	[Dependency]
	private readonly IComponentFactory _compFact;

	[Dependency]
	private readonly IPrototypeManager _protoMan;

	[Dependency]
	private readonly IReflectionManager _reflectionMan;

	[Dependency]
	private readonly INetManager _netMan;

	[Dependency]
	private readonly ILogManager _logMan;

	private readonly Dictionary<Type, HashSet<object>> _cachedTraits = new Dictionary<Type, HashSet<object>>();

	private const BindingFlags MembersBindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	protected ISawmill Sawmill;

	protected static readonly (ViewVariablesPath? Path, string[] Segments) EmptyResolve = (Path: null, Segments: Array.Empty<string>());

	private readonly Dictionary<string, DomainData> _registeredDomains = new Dictionary<string, DomainData>();

	protected readonly Dictionary<Guid, WeakReference<object>> _vvObjectStorage = new Dictionary<Guid, WeakReference<object>>();

	private static readonly Regex IndexerRegex = new Regex("\\[[^\\[]+\\]", RegexOptions.Compiled);

	private static readonly Regex TypeSpecifierRegex = new Regex("\\{[^\\{]+\\}", RegexOptions.Compiled);

	internal const int MaxListPathResponseLength = 500;

	private uint _nextReadRequestId;

	private uint _nextWriteRequestId;

	private uint _nextInvokeRequestId;

	private uint _nextListRequestId;

	private readonly Dictionary<uint, TaskCompletionSource<string?>> _readRequests = new Dictionary<uint, TaskCompletionSource<string>>();

	private readonly Dictionary<uint, TaskCompletionSource> _writeRequests = new Dictionary<uint, TaskCompletionSource>();

	private readonly Dictionary<uint, TaskCompletionSource<string?>> _invokeRequests = new Dictionary<uint, TaskCompletionSource<string>>();

	private readonly Dictionary<uint, TaskCompletionSource<IEnumerable<string>>> _listRequests = new Dictionary<uint, TaskCompletionSource<IEnumerable<string>>>();

	private readonly Dictionary<Type, ViewVariablesTypeHandler> _typeHandlers = new Dictionary<Type, ViewVariablesTypeHandler>();

	public virtual void Initialize()
	{
		InitializeDomains();
		InitializeTypeHandlers();
		InitializeRemote();
	}

	public object? ReadPath(string path)
	{
		return ResolvePath(path)?.Get();
	}

	public string? ReadPathSerialized(string path)
	{
		ViewVariablesPath viewVariablesPath = ResolvePath(path);
		if (viewVariablesPath == null)
		{
			return null;
		}
		object obj = viewVariablesPath.Get();
		if (obj == null)
		{
			return "null";
		}
		try
		{
			return SerializeValue(viewVariablesPath.Type, obj);
		}
		catch (Exception)
		{
			return obj.ToString();
		}
	}

	public void WritePath(string path, string value)
	{
		ViewVariablesPath viewVariablesPath = ResolvePath(path);
		viewVariablesPath?.Set(DeserializeValue(viewVariablesPath.Type, value));
	}

	public object? InvokePath(string path, string arguments)
	{
		ViewVariablesPath viewVariablesPath = ResolvePath(path);
		if (viewVariablesPath == null)
		{
			return null;
		}
		string[] arguments2 = ParseArguments(arguments);
		object[] parameters = DeserializeArguments(viewVariablesPath.InvokeParameterTypes, (int)viewVariablesPath.InvokeOptionalParameters, arguments2);
		return viewVariablesPath.Invoke(parameters);
	}

	public ICollection<object> TraitIdsFor(Type type)
	{
		if (!_cachedTraits.TryGetValue(type, out HashSet<object> value))
		{
			value = new HashSet<object>();
			_cachedTraits.Add(type, value);
			if (ViewVariablesUtility.TypeHasVisibleMembers(type))
			{
				value.Add(ViewVariablesTraits.Members);
			}
			if (typeof(IEnumerable).IsAssignableFrom(type))
			{
				value.Add(ViewVariablesTraits.Enumerable);
			}
			if (typeof(NetEntity).IsAssignableFrom(type))
			{
				value.Add(ViewVariablesTraits.Entity);
			}
		}
		return value;
	}

	void IPostInjectInit.PostInject()
	{
		Sawmill = _logMan.GetSawmill("vv");
	}

	public void RegisterDomain(string domain, DomainResolveObject resolveObject, DomainListPaths list)
	{
		_registeredDomains.Add(domain, new DomainData(resolveObject, list));
	}

	public bool UnregisterDomain(string domain)
	{
		return _registeredDomains.Remove(domain);
	}

	private void InitializeDomains()
	{
		RegisterDomain("ioc", ResolveIoCObject, ListIoCPaths);
		RegisterDomain("entity", ResolveEntityObject, ListEntityPaths);
		RegisterDomain("system", ResolveEntitySystemObject, ListEntitySystemPaths);
		RegisterDomain("prototype", ResolvePrototypeObject, ListPrototypePaths);
		RegisterDomain("object", ResolveStoredObject, ListStoredObjectPaths);
		RegisterDomain("vvtest", ResolveVvTestObject, ListVvTestObjectPaths);
	}

	private (ViewVariablesPath? Path, string[] Segments) ResolveIoCObject(string path)
	{
		(ViewVariablesInstancePath, string[]) tuple = (new ViewVariablesInstancePath(IoCManager.Instance), Array.Empty<string>());
		if (string.IsNullOrEmpty(path) || IoCManager.Instance == null)
		{
			(ViewVariablesInstancePath, string[]) tuple2 = tuple;
			return (Path: tuple2.Item1, Segments: tuple2.Item2);
		}
		string[] array = path.Split('/');
		if (array.Length == 0)
		{
			(ViewVariablesInstancePath, string[]) tuple2 = tuple;
			return (Path: tuple2.Item1, Segments: tuple2.Item2);
		}
		string name = array[0];
		if (!_reflectionMan.TryLooseGetType(name, out Type type))
		{
			return EmptyResolve;
		}
		if (!IoCManager.Instance.TryResolveType(type, out object instance))
		{
			return EmptyResolve;
		}
		return (Path: new ViewVariablesInstancePath(instance), Segments: array[1..]);
	}

	private IEnumerable<string>? ListIoCPaths(string[] segments)
	{
		if (segments.Length <= 1)
		{
			IDependencyCollection instance = IoCManager.Instance;
			if (instance != null)
			{
				if (segments.Length == 1 && _reflectionMan.TryLooseGetType(segments[0], out Type type) && instance.TryResolveType(type, out object _))
				{
					return null;
				}
				return from t in instance.GetRegisteredTypes()
					select t.Name;
			}
		}
		return null;
	}

	private (ViewVariablesPath? Path, string[] Segments) ResolveEntityObject(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return EmptyResolve;
		}
		string[] array = path.Split('/');
		if (array.Length == 0)
		{
			return EmptyResolve;
		}
		if (!int.TryParse(array[0], out var result) || result <= 0)
		{
			return EmptyResolve;
		}
		return (Path: new ViewVariablesInstancePath(new EntityUid(result)), Segments: array[1..]);
	}

	private IEnumerable<string>? ListEntityPaths(string[] segments)
	{
		if (segments.Length > 1)
		{
			return null;
		}
		if (segments.Length == 1 && NetEntity.TryParse(segments[0].AsSpan(), out var entity) && _entMan.TryGetEntity(entity, out var entity2) && _entMan.EntityExists(entity2))
		{
			return null;
		}
		return from uid in _entMan.GetEntities()
			select uid.ToString();
	}

	public (ViewVariablesPath? Path, string[] Segments) ResolveEntitySystemObject(string path)
	{
		IEntitySystemManager entitySysManager = _entMan.EntitySysManager;
		(ViewVariablesInstancePath, string[]) tuple = (new ViewVariablesInstancePath(entitySysManager), Array.Empty<string>());
		if (string.IsNullOrEmpty(path))
		{
			(ViewVariablesInstancePath, string[]) tuple2 = tuple;
			return (Path: tuple2.Item1, Segments: tuple2.Item2);
		}
		string[] array = path.Split('/');
		if (array.Length == 0)
		{
			(ViewVariablesInstancePath, string[]) tuple2 = tuple;
			return (Path: tuple2.Item1, Segments: tuple2.Item2);
		}
		string name = array[0];
		if (!_reflectionMan.TryLooseGetType(name, out Type type))
		{
			return EmptyResolve;
		}
		if (!entitySysManager.TryGetEntitySystem(type, out object system))
		{
			return EmptyResolve;
		}
		return (Path: new ViewVariablesInstancePath(system), Segments: array[1..]);
	}

	private IEnumerable<string>? ListEntitySystemPaths(string[] segments)
	{
		if (segments.Length > 1)
		{
			return null;
		}
		IEntitySystemManager entitySysManager = _entMan.EntitySysManager;
		if (segments.Length == 1 && _reflectionMan.TryLooseGetType(segments[0], out Type type) && entitySysManager.TryGetEntitySystem(type, out object _))
		{
			return null;
		}
		return from t in _entMan.EntitySysManager.GetEntitySystemTypes()
			select t.Name;
	}

	private (ViewVariablesPath? Path, string[] Segments) ResolvePrototypeObject(string path)
	{
		(ViewVariablesInstancePath, string[]) tuple = (new ViewVariablesInstancePath(_protoMan), Array.Empty<string>());
		if (string.IsNullOrEmpty(path) || IoCManager.Instance == null)
		{
			(ViewVariablesInstancePath, string[]) tuple2 = tuple;
			return (Path: tuple2.Item1, Segments: tuple2.Item2);
		}
		string[] array = path.Split('/');
		if (array.Length <= 1)
		{
			(ViewVariablesInstancePath, string[]) tuple2 = tuple;
			return (Path: tuple2.Item1, Segments: tuple2.Item2);
		}
		string kind = array[0];
		string id = array[1];
		if (!_protoMan.TryGetKindType(kind, out Type prototype) || !_protoMan.TryIndex(prototype, id, out IPrototype prototype2))
		{
			return EmptyResolve;
		}
		return (Path: new ViewVariablesInstancePath(prototype2), Segments: array[2..]);
	}

	private IEnumerable<string>? ListPrototypePaths(string[] segments)
	{
		int num = segments.Length;
		if (num != 0)
		{
			if ((uint)(num - 1) > 1u)
			{
				return null;
			}
			string kind = segments[0];
			string id = ((segments.Length == 1) ? string.Empty : segments[1]);
			if (_protoMan.HasKind(kind) && !_protoMan.TryIndex(_protoMan.GetKindType(kind), id, out IPrototype _))
			{
				return from p in _protoMan.EnumeratePrototypes(kind)
					select kind + "/" + p.ID;
			}
		}
		return _protoMan.GetPrototypeKinds();
	}

	private (ViewVariablesPath? Path, string[] Segments) ResolveStoredObject(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return EmptyResolve;
		}
		string[] array = path.Split('/');
		if (array.Length == 0 || !Guid.TryParse(array[0], out var result) || !_vvObjectStorage.TryGetValue(result, out WeakReference<object> value) || !value.TryGetTarget(out var target))
		{
			return EmptyResolve;
		}
		return (Path: new ViewVariablesInstancePath(target), Segments: array[1..]);
	}

	private IEnumerable<string>? ListStoredObjectPaths(string[] segments)
	{
		if (segments.Length > 1)
		{
			return null;
		}
		if (segments.Length == 1 && Guid.TryParse(segments[0], out var result) && _vvObjectStorage.ContainsKey(result))
		{
			return null;
		}
		return _vvObjectStorage.Keys.Select((Guid g) => g.ToString());
	}

	private (ViewVariablesPath? path, string[] segments) ResolveVvTestObject(string path)
	{
		string[] item = path.Split('/');
		return (path: new ViewVariablesInstancePath(new VvTest()), segments: item);
	}

	private IEnumerable<string>? ListVvTestObjectPaths(string[] segments)
	{
		return null;
	}

	public IEnumerable<string> ListPath(string path, VVListPathOptions options)
	{
		if (path.StartsWith('/'))
		{
			string text = path;
			path = text.Substring(1, text.Length - 1);
		}
		if (string.IsNullOrEmpty(path))
		{
			return Domains();
		}
		string[] array = path.Split('/');
		if (array.Length == 0)
		{
			return Domains();
		}
		string text2 = array[0];
		if (!_registeredDomains.TryGetValue(text2, out DomainData value))
		{
			return Domains();
		}
		IEnumerable<string> enumerable = value.List(array[1..]);
		if (enumerable != null)
		{
			return Full("/" + text2, enumerable);
		}
		ViewVariablesPath viewVariablesPath = ResolvePath(path);
		object obj = viewVariablesPath?.Get();
		if (obj == null)
		{
			array = array[..^1];
			path = string.Join('/', array);
			viewVariablesPath = ResolvePath(path);
			object obj2 = viewVariablesPath?.Get();
			if (obj2 == null)
			{
				return Enumerable.Empty<string>();
			}
			obj = obj2;
		}
		List<string> list = new List<string>();
		Type type = obj.GetType();
		foreach (ViewVariablesTypeHandler allTypeHandler in GetAllTypeHandlers(type))
		{
			list.AddRange(allTypeHandler.ListPath(viewVariablesPath));
		}
		HashSet<string> hashSet = new HashSet<string>(list);
		foreach (MemberInfo item in from m in type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			orderby m.DeclaringType == type
			select m)
		{
			if (ViewVariablesUtility.TryGetViewVariablesAccess(item, out var access) && !((int?)access < (int?)options.MinimumAccess))
			{
				string text3 = item.Name;
				if (!hashSet.Add(text3))
				{
					text3 = text3 + "{" + (item.DeclaringType?.FullName ?? typeof(void).FullName) + "}";
				}
				list.Add(text3);
				object value2 = item.GetValue(obj);
				if (options.ListIndexers)
				{
					ListIndexers(value2, text3, list);
				}
			}
		}
		if (options.ListIndexers)
		{
			ListIndexers(obj, string.Empty, list);
		}
		return Full(path, list);
		IEnumerable<string> Domains()
		{
			return _registeredDomains.Keys.Select((string d) => "/" + d);
		}
		static IEnumerable<string> Full(string fullPath, IEnumerable<string> relativePaths)
		{
			if (!fullPath.StartsWith('/'))
			{
				fullPath = "/" + fullPath;
			}
			if (fullPath.EndsWith('/'))
			{
				string text4 = fullPath;
				fullPath = text4.Substring(0, text4.Length - 1);
			}
			return relativePaths.Select(delegate(string p)
			{
				if (!p.StartsWith("["))
				{
					global::_003C_003Ey__InlineArray2<string> buffer = default(global::_003C_003Ey__InlineArray2<string>);
					buffer[0] = fullPath;
					buffer[1] = p;
					return string.Join('/', (ReadOnlySpan<string?>)buffer);
				}
				return fullPath + p;
			});
		}
	}

	private void ListIndexers(object? obj, string name, List<string> paths)
	{
		if (!(obj is IDictionary dictionary))
		{
			Array array = obj as Array;
			if (array == null)
			{
				if (obj is IList list)
				{
					for (int i = 0; i < list.Count; i++)
					{
						paths.Add($"{name}[{i}]");
					}
				}
				return;
			}
			int[] array2 = (from dimension in Enumerable.Range(0, array.Rank)
				select array.GetLowerBound(dimension)).ToArray();
			int[] array3 = (from dimension in Enumerable.Range(0, array.Rank)
				select array.GetUpperBound(dimension)).ToArray();
			int[] array4 = new int[array.Rank];
			array2.CopyTo(array4, 0);
			bool flag;
			do
			{
				paths.Add(name + "[" + string.Join(',', array4) + "]");
				flag = false;
				for (int num = array4.Length - 1; num >= -1; num--)
				{
					if (num == -1)
					{
						flag = true;
						break;
					}
					ref int reference = ref array4[num];
					reference++;
					if (reference <= array3[num])
					{
						break;
					}
					reference = array2[num];
				}
			}
			while (!flag);
			return;
		}
		Type type = typeof(void);
		Type[] genericTypeArguments = dictionary.GetType().GenericTypeArguments;
		if (genericTypeArguments != null && genericTypeArguments.Length == 2)
		{
			type = genericTypeArguments[0];
		}
		foreach (object key in dictionary.Keys)
		{
			try
			{
				Type type2 = key.GetType();
				string nodeTag = null;
				if (type2 != type)
				{
					nodeTag = "!type:" + type2.Name;
				}
				string text = SerializeValue(type2, key, nodeTag);
				if (text != null)
				{
					if (text.Contains(' '))
					{
						text = "(" + text + ")";
					}
					paths.Add(name + "[" + text + "]");
				}
			}
			catch (Exception)
			{
			}
		}
	}

	public ViewVariablesPath? ResolvePath(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return null;
		}
		if (path.StartsWith('/'))
		{
			string text = path;
			path = text.Substring(1, text.Length - 1);
		}
		if (path.EndsWith('/'))
		{
			string text = path;
			path = text.Substring(0, text.Length - 1);
		}
		string[] array = path.Split('/');
		if (array.Length == 0)
		{
			return null;
		}
		string key = array[0];
		if (!_registeredDomains.TryGetValue(key, out DomainData value))
		{
			return null;
		}
		var (path2, segments) = value.ResolveObject(string.Join('/', array[1..]));
		return ResolveRelativePath(path2, segments);
	}

	private ViewVariablesPath? ResolveRelativePath(ViewVariablesPath? path, string[] segments)
	{
		while (true)
		{
			if (path is ViewVariablesComponentPath parentComponent)
			{
				path.ParentComponent = parentComponent;
			}
			ViewVariablesComponentPath oldPath = path?.ParentComponent;
			if (segments.Length == 0)
			{
				return path;
			}
			object obj = path?.Get();
			if (obj == null)
			{
				return null;
			}
			string text = segments[0];
			if (string.IsNullOrEmpty(text))
			{
				segments = segments[1..];
				continue;
			}
			MatchCollection matchCollection = TypeSpecifierRegex.Matches(text);
			MatchCollection matchCollection2 = IndexerRegex.Matches(text);
			string text2 = TypeSpecifierRegex.Replace(IndexerRegex.Replace(text, string.Empty), string.Empty);
			if (matchCollection.Count > 1)
			{
				return null;
			}
			VVAccess? access = null;
			if (matchCollection.Count != 1)
			{
				ViewVariablesPath viewVariablesPath = ResolveTypeHandlers(path, text2);
				if (viewVariablesPath != null)
				{
					path = viewVariablesPath;
					access = VVAccess.ReadWrite;
					goto IL_018a;
				}
			}
			Type declaringType = null;
			if (matchCollection.Count == 1)
			{
				IReflectionManager reflectionMan = _reflectionMan;
				string value = matchCollection[0].Value;
				Type type = reflectionMan.GetType(value.Substring(1, value.Length - 1 - 1));
				if ((object)type != null)
				{
					declaringType = type;
				}
			}
			MemberInfo singleMember = obj.GetType().GetSingleMember(text2, declaringType);
			if (singleMember == null || !ViewVariablesUtility.TryGetViewVariablesAccess(singleMember, out access))
			{
				return null;
			}
			ViewVariablesPath viewVariablesPath2;
			if (!(singleMember is FieldInfo) && !(singleMember is PropertyInfo))
			{
				if (!(singleMember is MethodInfo method))
				{
					break;
				}
				viewVariablesPath2 = new ViewVariablesMethodPath(obj, method);
			}
			else
			{
				viewVariablesPath2 = new ViewVariablesFieldOrPropertyPath(obj, singleMember, _entMan);
			}
			path = viewVariablesPath2;
			goto IL_018a;
			IL_018a:
			UpdateParentComp(path, ref oldPath);
			foreach (Match item in matchCollection2)
			{
				ViewVariablesPath? path2 = path;
				string value = item.Value;
				path = ResolveIndexing(path2, ParseArguments(value.Substring(1, value.Length - 1 - 1)), access.Value);
				UpdateParentComp(path, ref oldPath);
			}
			segments = segments[1..];
		}
		throw new InvalidOperationException("Invalid member! Must be a property, field or method.");
	}

	private void UpdateParentComp(ViewVariablesPath? newPath, ref ViewVariablesComponentPath? oldPath)
	{
		if (newPath == null)
		{
			return;
		}
		if (newPath is ViewVariablesComponentPath parentComponent)
		{
			newPath.ParentComponent = parentComponent;
		}
		else if (newPath != null)
		{
			if (newPath.ParentComponent == null)
			{
				newPath.ParentComponent = oldPath;
			}
		}
		oldPath = newPath?.ParentComponent;
	}

	private ViewVariablesPath? ResolveIndexing(ViewVariablesPath? path, string[] arguments, VVAccess access)
	{
		object obj = path?.Get();
		if (obj == null || arguments.Length == 0)
		{
			return null;
		}
		Type type = obj.GetType();
		MethodInfo getter;
		MethodInfo setter;
		object?[] p;
		if (type.IsArray && type.GetArrayRank() > 1)
		{
			getter = type.GetSingleMember("Get") as MethodInfo;
			setter = type.GetSingleMember("Set") as MethodInfo;
			if (getter == null && setter == null)
			{
				return null;
			}
			p = DeserializeArguments((from parameterInfo in getter?.GetParameters()
				select parameterInfo.ParameterType).ToArray() ?? setter.GetParameters()[1..].Select((ParameterInfo parameterInfo) => parameterInfo.ParameterType).ToArray(), 0, arguments);
			return new ViewVariablesFakePath((Func<object?>?)Get, (Action<object?>?)Set, (Func<object?, object?>?)null, getter?.ReturnType ?? setter.GetParameters()[0].ParameterType, (Type[]?)null, 0u, (Type?)null);
		}
		PropertyInfo indexer = type.GetIndexer();
		if ((object)indexer == null)
		{
			return null;
		}
		ParameterInfo[] indexParameters = indexer.GetIndexParameters();
		object[] array = DeserializeArguments(indexParameters.Select((ParameterInfo parameterInfo) => parameterInfo.ParameterType).ToArray(), indexParameters.Count((ParameterInfo parameterInfo) => parameterInfo.IsOptional), arguments);
		if (array == null)
		{
			return null;
		}
		return new ViewVariablesIndexedPath(obj, indexer, array, access);
		object? Get()
		{
			return getter?.Invoke(obj, p);
		}
		void Set(object? value)
		{
			if (p != null && access == VVAccess.ReadWrite)
			{
				setter?.Invoke(obj, new object[1] { value }.Concat<object>(p).ToArray());
			}
		}
	}

	private ViewVariablesPath? ResolveTypeHandlers(ViewVariablesPath path, string relativePath)
	{
		object obj = path.Get();
		if (obj == null || string.IsNullOrEmpty(relativePath) || relativePath.Contains('/'))
		{
			return null;
		}
		foreach (ViewVariablesTypeHandler allTypeHandler in GetAllTypeHandlers(obj.GetType()))
		{
			ViewVariablesPath viewVariablesPath = allTypeHandler.HandlePath(path, relativePath);
			if (viewVariablesPath != null)
			{
				return viewVariablesPath;
			}
		}
		return null;
	}

	private void InitializeRemote()
	{
		_netMan.RegisterNetMessage<MsgViewVariablesReadPathReq>(ReadRemotePathRequest);
		_netMan.RegisterNetMessage<MsgViewVariablesWritePathReq>(WriteRemotePathRequest);
		_netMan.RegisterNetMessage<MsgViewVariablesInvokePathReq>(InvokeRemotePathRequest);
		_netMan.RegisterNetMessage<MsgViewVariablesListPathReq>(ListRemotePathRequest);
		_netMan.RegisterNetMessage<MsgViewVariablesReadPathRes>(ReadRemotePathResponse);
		_netMan.RegisterNetMessage<MsgViewVariablesWritePathRes>(WriteRemotePathResponse);
		_netMan.RegisterNetMessage<MsgViewVariablesInvokePathRes>(InvokeRemotePathResponse);
		_netMan.RegisterNetMessage<MsgViewVariablesListPathRes>(ListRemotePathResponse);
	}

	public Task<string?> ReadRemotePath(string path, ICommonSession? session = null)
	{
		if (!_netMan.IsConnected || (_netMan.IsServer && session == null))
		{
			return Task.FromResult<string>(null);
		}
		MsgViewVariablesReadPathReq msgViewVariablesReadPathReq = new MsgViewVariablesReadPathReq
		{
			RequestId = _nextReadRequestId++,
			Path = path,
			Session = (((Guid?)session?.UserId) ?? Guid.Empty)
		};
		TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();
		_readRequests.Add(msgViewVariablesReadPathReq.RequestId, taskCompletionSource);
		SendMessage(msgViewVariablesReadPathReq, session?.Channel);
		return taskCompletionSource.Task;
	}

	public Task WriteRemotePath(string path, string value, ICommonSession? session = null)
	{
		if (!_netMan.IsConnected || (_netMan.IsServer && session == null))
		{
			return Task.CompletedTask;
		}
		MsgViewVariablesWritePathReq msgViewVariablesWritePathReq = new MsgViewVariablesWritePathReq
		{
			RequestId = _nextWriteRequestId++,
			Path = path,
			Value = value,
			Session = (((Guid?)session?.UserId) ?? Guid.Empty)
		};
		TaskCompletionSource taskCompletionSource = new TaskCompletionSource();
		_writeRequests.Add(msgViewVariablesWritePathReq.RequestId, taskCompletionSource);
		SendMessage(msgViewVariablesWritePathReq, session?.Channel);
		return taskCompletionSource.Task;
	}

	public Task<string?> InvokeRemotePath(string path, string arguments, ICommonSession? session = null)
	{
		if (!_netMan.IsConnected || (_netMan.IsServer && session == null))
		{
			return Task.FromResult<string>(null);
		}
		MsgViewVariablesInvokePathReq msgViewVariablesInvokePathReq = new MsgViewVariablesInvokePathReq
		{
			RequestId = _nextInvokeRequestId++,
			Path = path,
			Value = arguments,
			Session = (((Guid?)session?.UserId) ?? Guid.Empty)
		};
		TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();
		_invokeRequests.Add(msgViewVariablesInvokePathReq.RequestId, taskCompletionSource);
		SendMessage(msgViewVariablesInvokePathReq, session?.Channel);
		return taskCompletionSource.Task;
	}

	public Task<IEnumerable<string>> ListRemotePath(string path, VVListPathOptions options, ICommonSession? session = null)
	{
		if (!_netMan.IsConnected || (_netMan.IsServer && session == null))
		{
			return Task.FromResult(Enumerable.Empty<string>());
		}
		MsgViewVariablesListPathReq msgViewVariablesListPathReq = new MsgViewVariablesListPathReq
		{
			RequestId = _nextListRequestId++,
			Path = path,
			Options = options,
			Session = (((Guid?)session?.UserId) ?? Guid.Empty)
		};
		TaskCompletionSource<IEnumerable<string>> taskCompletionSource = new TaskCompletionSource<IEnumerable<string>>();
		_listRequests.Add(msgViewVariablesListPathReq.RequestId, taskCompletionSource);
		SendMessage(msgViewVariablesListPathReq, session?.Channel);
		return taskCompletionSource.Task;
	}

	private async void ReadRemotePathRequest(MsgViewVariablesReadPathReq req)
	{
		if (!CheckPermissions(req.MsgChannel, "vvread"))
		{
			SendMessage(new MsgViewVariablesReadPathRes(req)
			{
				ResponseCode = ViewVariablesResponseCode.NoAccess
			}, req.MsgChannel);
			return;
		}
		if (_netMan.IsServer && TryGetSession(req.Session, out ICommonSession session))
		{
			string text = await ReadRemotePath(req.Path, session);
			SendMessage(new MsgViewVariablesReadPathRes(req)
			{
				Response = new string[1] { text ?? "null" }
			}, req.MsgChannel);
			return;
		}
		string text2 = ReadPathSerialized(req.Path);
		if (text2 == null)
		{
			SendMessage(new MsgViewVariablesReadPathRes(req)
			{
				ResponseCode = ViewVariablesResponseCode.NoObject
			}, req.MsgChannel);
		}
		else
		{
			SendMessage(new MsgViewVariablesReadPathRes(req)
			{
				Response = new string[1] { text2 }
			}, req.MsgChannel);
		}
	}

	private async void WriteRemotePathRequest(MsgViewVariablesWritePathReq req)
	{
		if (!CheckPermissions(req.MsgChannel, "vvwrite"))
		{
			_netMan.ServerSendMessage(new MsgViewVariablesWritePathRes(req)
			{
				ResponseCode = ViewVariablesResponseCode.NoAccess
			}, req.MsgChannel);
			return;
		}
		if (_netMan.IsServer && TryGetSession(req.Session, out ICommonSession session))
		{
			await WriteRemotePath(req.Path, req.Value ?? string.Empty, session);
			SendMessage(new MsgViewVariablesWritePathRes(req), req.MsgChannel);
			return;
		}
		ViewVariablesPath viewVariablesPath = ResolvePath(req.Path);
		if (viewVariablesPath == null)
		{
			SendMessage(new MsgViewVariablesWritePathRes(req)
			{
				ResponseCode = ViewVariablesResponseCode.NoObject
			}, req.MsgChannel);
			return;
		}
		object value = ((req.Value != null) ? DeserializeValue(viewVariablesPath.Type, req.Value) : null);
		try
		{
			viewVariablesPath.Set(value);
		}
		catch (Exception)
		{
			SendMessage(new MsgViewVariablesWritePathRes(req)
			{
				ResponseCode = ViewVariablesResponseCode.InvalidRequest
			}, req.MsgChannel);
			return;
		}
		SendMessage(new MsgViewVariablesWritePathRes(req), req.MsgChannel);
	}

	private async void InvokeRemotePathRequest(MsgViewVariablesInvokePathReq req)
	{
		if (!CheckPermissions(req.MsgChannel, "vvinvoke"))
		{
			_netMan.ServerSendMessage(new MsgViewVariablesInvokePathRes(req)
			{
				Path = req.Path,
				ResponseCode = ViewVariablesResponseCode.NoAccess
			}, req.MsgChannel);
			return;
		}
		if (_netMan.IsServer && TryGetSession(req.Session, out ICommonSession session))
		{
			string text = await InvokeRemotePath(req.Path, req.Value ?? string.Empty, session);
			SendMessage(new MsgViewVariablesInvokePathRes(req)
			{
				Response = new string[1] { text ?? "null" }
			}, req.MsgChannel);
			return;
		}
		ViewVariablesPath viewVariablesPath = ResolvePath(req.Path);
		if (viewVariablesPath == null)
		{
			SendMessage(new MsgViewVariablesInvokePathRes(req)
			{
				ResponseCode = ViewVariablesResponseCode.NoObject
			}, req.MsgChannel);
			return;
		}
		string[] arguments = ((req.Value != null) ? ParseArguments(req.Value) : Array.Empty<string>());
		object[] parameters = DeserializeArguments(viewVariablesPath.InvokeParameterTypes, (int)viewVariablesPath.InvokeOptionalParameters, arguments);
		object obj;
		try
		{
			obj = viewVariablesPath.Invoke(parameters);
		}
		catch (Exception)
		{
			SendMessage(new MsgViewVariablesInvokePathRes(req)
			{
				ResponseCode = ViewVariablesResponseCode.InvalidRequest
			}, req.MsgChannel);
			return;
		}
		string text2;
		try
		{
			text2 = SerializeValue(viewVariablesPath.InvokeReturnType, obj) ?? obj?.ToString() ?? "null";
		}
		catch (Exception)
		{
			text2 = obj?.ToString() ?? "null";
		}
		SendMessage(new MsgViewVariablesInvokePathRes(req)
		{
			Response = new string[1] { text2 }
		}, req.MsgChannel);
	}

	private async void ListRemotePathRequest(MsgViewVariablesListPathReq req)
	{
		if (!CheckPermissions(req.MsgChannel, "vv"))
		{
			_netMan.ServerSendMessage(new MsgViewVariablesListPathRes(req)
			{
				ResponseCode = ViewVariablesResponseCode.NoAccess
			}, req.MsgChannel);
			return;
		}
		if (_netMan.IsServer && TryGetSession(req.Session, out ICommonSession session))
		{
			IEnumerable<string> source = await ListRemotePath(req.Path, req.Options, session);
			SendMessage(new MsgViewVariablesListPathRes(req)
			{
				Response = source.ToArray()
			}, req.MsgChannel);
			return;
		}
		string[] response = (from p in ListPath(req.Path, req.Options)
			orderby p.StartsWith(req.Path) descending
			select p).Take(Math.Min(500, req.Options.RemoteListLength)).ToArray();
		SendMessage(new MsgViewVariablesListPathRes(req)
		{
			Response = response
		}, req.MsgChannel);
	}

	private void ReadRemotePathResponse(MsgViewVariablesReadPathRes res)
	{
		if (_readRequests.Remove(res.RequestId, out TaskCompletionSource<string> value))
		{
			if (res.ResponseCode != ViewVariablesResponseCode.Ok)
			{
				value.TrySetResult(null);
			}
			else if (res.Response.Length == 0)
			{
				value.TrySetResult(null);
			}
			else
			{
				value.TrySetResult(res.Response[0]);
			}
		}
	}

	private void WriteRemotePathResponse(MsgViewVariablesWritePathRes res)
	{
		if (_writeRequests.Remove(res.RequestId, out TaskCompletionSource value))
		{
			value.SetResult();
		}
	}

	private void InvokeRemotePathResponse(MsgViewVariablesInvokePathRes res)
	{
		if (_invokeRequests.Remove(res.RequestId, out TaskCompletionSource<string> value))
		{
			if (res.ResponseCode != ViewVariablesResponseCode.Ok)
			{
				value.TrySetResult(null);
			}
			else if (res.Response.Length == 0)
			{
				value.TrySetResult(null);
			}
			else
			{
				value.TrySetResult(res.Response[0]);
			}
		}
	}

	private void ListRemotePathResponse(MsgViewVariablesListPathRes res)
	{
		if (_listRequests.Remove(res.RequestId, out TaskCompletionSource<IEnumerable<string>> value))
		{
			if (res.ResponseCode != ViewVariablesResponseCode.Ok)
			{
				value.TrySetResult(Enumerable.Empty<string>());
			}
			else
			{
				value.TrySetResult(res.Response);
			}
		}
	}

	private void SendMessage(NetMessage msg, INetChannel? channel = null)
	{
		if (_netMan.IsServer)
		{
			if (channel == null)
			{
				throw new ArgumentNullException("channel");
			}
			_netMan.ServerSendMessage(msg, channel);
		}
		else
		{
			_netMan.ClientSendMessage(msg);
		}
	}

	protected abstract bool CheckPermissions(INetChannel channel, string command);

	protected abstract bool TryGetSession(Guid guid, [NotNullWhen(true)] out ICommonSession? session);

	private static string[] ParseArguments(string arguments)
	{
		List<string> list = new List<string>();
		bool flag = false;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in arguments)
		{
			switch (c)
			{
			case '(':
				flag = true;
				continue;
			case ')':
				if (flag)
				{
					flag = false;
					continue;
				}
				break;
			case ',':
				if (!flag)
				{
					list.Add(stringBuilder.ToString());
					stringBuilder.Clear();
					continue;
				}
				break;
			}
			if (flag || !char.IsWhiteSpace(c))
			{
				stringBuilder.Append(c);
			}
		}
		if (stringBuilder.Length != 0)
		{
			list.Add(stringBuilder.ToString());
		}
		return list.ToArray();
	}

	private object?[]? DeserializeArguments(Type[] argumentTypes, int optionalArguments, string[] arguments)
	{
		if (arguments.Length < argumentTypes.Length - optionalArguments || arguments.Length > argumentTypes.Length)
		{
			return null;
		}
		List<object> list = new List<object>();
		for (int i = 0; i < arguments.Length; i++)
		{
			string value = arguments[i];
			Type type = argumentTypes[i];
			object item = DeserializeValue(type, value);
			list.Add(item);
		}
		for (int j = 0; j < argumentTypes.Length - arguments.Length; j++)
		{
			list.Add(Type.Missing);
		}
		return list.ToArray();
	}

	private object? DeserializeValue(Type type, string value)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		object obj = ResolvePath(value)?.Get();
		if (obj != null && obj.GetType().IsAssignableTo(type))
		{
			return obj;
		}
		try
		{
			using TextReader textReader = new StringReader(value);
			YamlStream val = new YamlStream();
			val.Load(textReader);
			YamlNode rootNode = val.Documents[0].RootNode;
			return _serMan.Read(type, rootNode.ToDataNode());
		}
		catch (Exception)
		{
			return null;
		}
	}

	private string? SerializeValue(Type type, object? value, string? nodeTag = null)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		if (value == null || type == typeof(void))
		{
			return null;
		}
		DataNode dataNode = _serMan.WriteValue(type, value, alwaysWrite: true);
		if (!string.IsNullOrEmpty(nodeTag))
		{
			dataNode.Tag = nodeTag;
		}
		YamlDocument val = new YamlDocument(dataNode.ToYamlNode());
		YamlStream val2 = new YamlStream();
		val2.Add(val);
		YamlStream val3 = val2;
		using StringWriter stringWriter = new StringWriter(new StringBuilder());
		val3.Save((IEmitter)(object)new YamlNoDocEndDotsFix((IEmitter)(object)new YamlMappingFix((IEmitter)new Emitter((TextWriter)stringWriter))), false);
		return stringWriter.ToString();
	}

	public ViewVariablesTypeHandler<T> GetTypeHandler<T>()
	{
		if (_typeHandlers.TryGetValue(typeof(T), out ViewVariablesTypeHandler value))
		{
			return (ViewVariablesTypeHandler<T>)value;
		}
		ViewVariablesTypeHandler<T> viewVariablesTypeHandler = new ViewVariablesTypeHandler<T>(Sawmill);
		_typeHandlers.Add(typeof(T), viewVariablesTypeHandler);
		return viewVariablesTypeHandler;
	}

	private void InitializeTypeHandlers()
	{
		GetTypeHandler<EntityUid>().AddHandler(EntityComponentHandler, EntityComponentList).AddPath("Delete", (EntityUid uid) => ViewVariablesPath.FromInvoker(delegate
		{
			_entMan.DeleteEntity(uid);
		})).AddPath("QueueDelete", (EntityUid uid) => ViewVariablesPath.FromInvoker(delegate
		{
			_entMan.QueueDeleteEntity(uid);
		}));
	}

	private ViewVariablesPath? EntityComponentHandler(EntityUid uid, string relativePath)
	{
		if (!_entMan.EntityExists(uid) || !_compFact.TryGetRegistration(relativePath, out ComponentRegistration registration, ignoreCase: true) || !_entMan.TryGetComponent(uid, registration.Idx, out IComponent component))
		{
			return null;
		}
		return new ViewVariablesComponentPath(component, uid);
	}

	private IEnumerable<string> EntityComponentList(EntityUid uid)
	{
		return from component in _entMan.GetComponents(uid)
			select _compFact.GetComponentName(component.GetType());
	}

	private IEnumerable<ViewVariablesTypeHandler> GetAllTypeHandlers(Type origType)
	{
		Type type = origType;
		while (type != null)
		{
			if (_typeHandlers.TryGetValue(type, out ViewVariablesTypeHandler value))
			{
				yield return value;
			}
			type = type.BaseType;
		}
		Type[] interfaces = origType.GetInterfaces();
		foreach (Type key in interfaces)
		{
			if (_typeHandlers.TryGetValue(key, out ViewVariablesTypeHandler value2))
			{
				yield return value2;
			}
		}
	}
}
