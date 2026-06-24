// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.ViewVariablesManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.ViewVariables;

internal abstract class ViewVariablesManager : IViewVariablesManager, IPostInjectInit
{
  [Robust.Shared.IoC.Dependency]
  private readonly ISerializationManager _serMan;
  [Robust.Shared.IoC.Dependency]
  private readonly IEntityManager _entMan;
  [Robust.Shared.IoC.Dependency]
  private readonly IComponentFactory _compFact;
  [Robust.Shared.IoC.Dependency]
  private readonly IPrototypeManager _protoMan;
  [Robust.Shared.IoC.Dependency]
  private readonly IReflectionManager _reflectionMan;
  [Robust.Shared.IoC.Dependency]
  private readonly INetManager _netMan;
  [Robust.Shared.IoC.Dependency]
  private readonly ILogManager _logMan;
  private readonly Dictionary<Type, HashSet<object>> _cachedTraits = new Dictionary<Type, HashSet<object>>();
  private const BindingFlags MembersBindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
  protected ISawmill Sawmill;
  protected static readonly (ViewVariablesPath? Path, string[] Segments) EmptyResolve = ((ViewVariablesPath) null, Array.Empty<string>());
  private readonly Dictionary<string, ViewVariablesManager.DomainData> _registeredDomains = new Dictionary<string, ViewVariablesManager.DomainData>();
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
    this.InitializeDomains();
    this.InitializeTypeHandlers();
    this.InitializeRemote();
  }

  public object? ReadPath(string path) => this.ResolvePath(path)?.Get();

  public string? ReadPathSerialized(string path)
  {
    ViewVariablesPath viewVariablesPath = this.ResolvePath(path);
    if (viewVariablesPath == null)
      return (string) null;
    object obj = viewVariablesPath.Get();
    if (obj == null)
      return "null";
    try
    {
      return this.SerializeValue(viewVariablesPath.Type, obj);
    }
    catch (Exception ex)
    {
      return obj.ToString();
    }
  }

  public void WritePath(string path, string value)
  {
    ViewVariablesPath viewVariablesPath = this.ResolvePath(path);
    viewVariablesPath?.Set(this.DeserializeValue(viewVariablesPath.Type, value));
  }

  public object? InvokePath(string path, string arguments)
  {
    ViewVariablesPath viewVariablesPath = this.ResolvePath(path);
    if (viewVariablesPath == null)
      return (object) null;
    string[] arguments1 = ViewVariablesManager.ParseArguments(arguments);
    object[] parameters = this.DeserializeArguments(viewVariablesPath.InvokeParameterTypes, (int) viewVariablesPath.InvokeOptionalParameters, arguments1);
    return viewVariablesPath.Invoke(parameters);
  }

  public ICollection<object> TraitIdsFor(Type type)
  {
    HashSet<object> objectSet;
    if (!this._cachedTraits.TryGetValue(type, out objectSet))
    {
      objectSet = new HashSet<object>();
      this._cachedTraits.Add(type, objectSet);
      if (ViewVariablesUtility.TypeHasVisibleMembers(type))
        objectSet.Add((object) ViewVariablesTraits.Members);
      if (typeof (IEnumerable).IsAssignableFrom(type))
        objectSet.Add((object) ViewVariablesTraits.Enumerable);
      if (typeof (NetEntity).IsAssignableFrom(type))
        objectSet.Add((object) ViewVariablesTraits.Entity);
    }
    return (ICollection<object>) objectSet;
  }

  void IPostInjectInit.PostInject() => this.Sawmill = this._logMan.GetSawmill("vv");

  public void RegisterDomain(
    string domain,
    DomainResolveObject resolveObject,
    DomainListPaths list)
  {
    this._registeredDomains.Add(domain, new ViewVariablesManager.DomainData(resolveObject, list));
  }

  public bool UnregisterDomain(string domain) => this._registeredDomains.Remove(domain);

  private void InitializeDomains()
  {
    this.RegisterDomain("ioc", new DomainResolveObject(this.ResolveIoCObject), new DomainListPaths(this.ListIoCPaths));
    this.RegisterDomain("entity", new DomainResolveObject(this.ResolveEntityObject), new DomainListPaths(this.ListEntityPaths));
    this.RegisterDomain("system", new DomainResolveObject(this.ResolveEntitySystemObject), new DomainListPaths(this.ListEntitySystemPaths));
    this.RegisterDomain("prototype", new DomainResolveObject(this.ResolvePrototypeObject), new DomainListPaths(this.ListPrototypePaths));
    this.RegisterDomain("object", new DomainResolveObject(this.ResolveStoredObject), new DomainListPaths(this.ListStoredObjectPaths));
    this.RegisterDomain("vvtest", new DomainResolveObject(this.ResolveVvTestObject), new DomainListPaths(this.ListVvTestObjectPaths));
  }

  private (ViewVariablesPath? Path, string[] Segments) ResolveIoCObject(string path)
  {
    (ViewVariablesInstancePath, string[]) valueTuple1 = (new ViewVariablesInstancePath((object) IoCManager.Instance), Array.Empty<string>());
    if (string.IsNullOrEmpty(path) || IoCManager.Instance == null)
    {
      (ViewVariablesInstancePath, string[]) valueTuple2 = valueTuple1;
      return ((ViewVariablesPath) valueTuple2.Item1, valueTuple2.Item2);
    }
    string[] array = path.Split('/');
    if (array.Length == 0)
    {
      (ViewVariablesInstancePath, string[]) valueTuple3 = valueTuple1;
      return ((ViewVariablesPath) valueTuple3.Item1, valueTuple3.Item2);
    }
    Type type;
    object instance;
    return !this._reflectionMan.TryLooseGetType(array[0], out type) || !IoCManager.Instance.TryResolveType(type, out instance) ? ViewVariablesManager.EmptyResolve : ((ViewVariablesPath) new ViewVariablesInstancePath(instance), RuntimeHelpers.GetSubArray<string>(array, Range.StartAt((Index) 1)));
  }

  private IEnumerable<string>? ListIoCPaths(string[] segments)
  {
    if (segments.Length <= 1)
    {
      IDependencyCollection instance = IoCManager.Instance;
      if (instance != null)
      {
        Type type;
        return segments.Length == 1 && this._reflectionMan.TryLooseGetType(segments[0], out type) && instance.TryResolveType(type, out object _) ? (IEnumerable<string>) null : instance.GetRegisteredTypes().Select<Type, string>((Func<Type, string>) (t => t.Name));
      }
    }
    return (IEnumerable<string>) null;
  }

  private (ViewVariablesPath? Path, string[] Segments) ResolveEntityObject(string path)
  {
    if (string.IsNullOrEmpty(path))
      return ViewVariablesManager.EmptyResolve;
    string[] array = path.Split('/');
    int result;
    return array.Length == 0 || !int.TryParse(array[0], out result) || result <= 0 ? ViewVariablesManager.EmptyResolve : ((ViewVariablesPath) new ViewVariablesInstancePath((object) new EntityUid(result)), RuntimeHelpers.GetSubArray<string>(array, Range.StartAt((Index) 1)));
  }

  private IEnumerable<string>? ListEntityPaths(string[] segments)
  {
    if (segments.Length > 1)
      return (IEnumerable<string>) null;
    NetEntity entity1;
    EntityUid? entity2;
    return segments.Length == 1 && NetEntity.TryParse(segments[0].AsSpan(), out entity1) && this._entMan.TryGetEntity(entity1, out entity2) && this._entMan.EntityExists(entity2) ? (IEnumerable<string>) null : this._entMan.GetEntities().Select<EntityUid, string>((Func<EntityUid, string>) (uid => uid.ToString()));
  }

  public (ViewVariablesPath? Path, string[] Segments) ResolveEntitySystemObject(string path)
  {
    IEntitySystemManager entitySysManager = this._entMan.EntitySysManager;
    (ViewVariablesInstancePath, string[]) valueTuple1 = (new ViewVariablesInstancePath((object) entitySysManager), Array.Empty<string>());
    if (string.IsNullOrEmpty(path))
    {
      (ViewVariablesInstancePath, string[]) valueTuple2 = valueTuple1;
      return ((ViewVariablesPath) valueTuple2.Item1, valueTuple2.Item2);
    }
    string[] array = path.Split('/');
    if (array.Length == 0)
    {
      (ViewVariablesInstancePath, string[]) valueTuple3 = valueTuple1;
      return ((ViewVariablesPath) valueTuple3.Item1, valueTuple3.Item2);
    }
    Type type;
    object system;
    return !this._reflectionMan.TryLooseGetType(array[0], out type) || !entitySysManager.TryGetEntitySystem(type, out system) ? ViewVariablesManager.EmptyResolve : ((ViewVariablesPath) new ViewVariablesInstancePath(system), RuntimeHelpers.GetSubArray<string>(array, Range.StartAt((Index) 1)));
  }

  private IEnumerable<string>? ListEntitySystemPaths(string[] segments)
  {
    if (segments.Length > 1)
      return (IEnumerable<string>) null;
    IEntitySystemManager entitySysManager = this._entMan.EntitySysManager;
    Type type;
    return segments.Length == 1 && this._reflectionMan.TryLooseGetType(segments[0], out type) && entitySysManager.TryGetEntitySystem(type, out object _) ? (IEnumerable<string>) null : this._entMan.EntitySysManager.GetEntitySystemTypes().Select<Type, string>((Func<Type, string>) (t => t.Name));
  }

  private (ViewVariablesPath? Path, string[] Segments) ResolvePrototypeObject(string path)
  {
    (ViewVariablesInstancePath, string[]) valueTuple1 = (new ViewVariablesInstancePath((object) this._protoMan), Array.Empty<string>());
    if (string.IsNullOrEmpty(path) || IoCManager.Instance == null)
    {
      (ViewVariablesInstancePath, string[]) valueTuple2 = valueTuple1;
      return ((ViewVariablesPath) valueTuple2.Item1, valueTuple2.Item2);
    }
    string[] array = path.Split('/');
    if (array.Length <= 1)
    {
      (ViewVariablesInstancePath, string[]) valueTuple3 = valueTuple1;
      return ((ViewVariablesPath) valueTuple3.Item1, valueTuple3.Item2);
    }
    string kind = array[0];
    string id = array[1];
    Type prototype1;
    IPrototype prototype2;
    return !this._protoMan.TryGetKindType(kind, out prototype1) || !this._protoMan.TryIndex(prototype1, id, out prototype2) ? ViewVariablesManager.EmptyResolve : ((ViewVariablesPath) new ViewVariablesInstancePath((object) prototype2), RuntimeHelpers.GetSubArray<string>(array, Range.StartAt((Index) 2)));
  }

  private IEnumerable<string>? ListPrototypePaths(string[] segments)
  {
    switch (segments.Length)
    {
      case 0:
        return this._protoMan.GetPrototypeKinds();
      case 1:
      case 2:
        string kind = segments[0];
        string id = segments.Length == 1 ? string.Empty : segments[1];
        if (this._protoMan.HasKind(kind) && !this._protoMan.TryIndex(this._protoMan.GetKindType(kind), id, out IPrototype _))
          return this._protoMan.EnumeratePrototypes(kind).Select<IPrototype, string>((Func<IPrototype, string>) (p => $"{kind}/{p.ID}"));
        goto case 0;
      default:
        return (IEnumerable<string>) null;
    }
  }

  private (ViewVariablesPath? Path, string[] Segments) ResolveStoredObject(string path)
  {
    if (string.IsNullOrEmpty(path))
      return ViewVariablesManager.EmptyResolve;
    string[] array = path.Split('/');
    Guid result;
    WeakReference<object> weakReference;
    object target;
    return array.Length == 0 || !Guid.TryParse(array[0], out result) || !this._vvObjectStorage.TryGetValue(result, out weakReference) || !weakReference.TryGetTarget(out target) ? ViewVariablesManager.EmptyResolve : ((ViewVariablesPath) new ViewVariablesInstancePath(target), RuntimeHelpers.GetSubArray<string>(array, Range.StartAt((Index) 1)));
  }

  private IEnumerable<string>? ListStoredObjectPaths(string[] segments)
  {
    if (segments.Length > 1)
      return (IEnumerable<string>) null;
    Guid result;
    return segments.Length == 1 && Guid.TryParse(segments[0], out result) && this._vvObjectStorage.ContainsKey(result) ? (IEnumerable<string>) null : this._vvObjectStorage.Keys.Select<Guid, string>((Func<Guid, string>) (g => g.ToString()));
  }

  private (ViewVariablesPath? path, string[] segments) ResolveVvTestObject(string path)
  {
    return ((ViewVariablesPath) new ViewVariablesInstancePath((object) new ViewVariablesManager.VvTest()), path.Split('/'));
  }

  private IEnumerable<string>? ListVvTestObjectPaths(string[] segments)
  {
    return (IEnumerable<string>) null;
  }

  public IEnumerable<string> ListPath(string path, VVListPathOptions options)
  {
    if (path.StartsWith('/'))
    {
      string str = path;
      path = str.Substring(1, str.Length - 1);
    }
    if (string.IsNullOrEmpty(path))
      return Domains();
    string[] array = path.Split('/');
    if (array.Length == 0)
      return Domains();
    string key = array[0];
    ViewVariablesManager.DomainData domainData;
    if (!this._registeredDomains.TryGetValue(key, out domainData))
      return Domains();
    IEnumerable<string> relativePaths = domainData.List(RuntimeHelpers.GetSubArray<string>(array, Range.StartAt((Index) 1)));
    if (relativePaths != null)
      return Full("/" + key, relativePaths);
    ViewVariablesPath path1 = this.ResolvePath(path);
    object instance = path1?.Get();
    if (instance == null)
    {
      path = string.Join('/', RuntimeHelpers.GetSubArray<string>(array, Range.EndAt(new Index(1, true))));
      path1 = this.ResolvePath(path);
      object obj = path1?.Get();
      if (obj == null)
        return Enumerable.Empty<string>();
      instance = obj;
    }
    List<string> stringList = new List<string>();
    Type type = instance.GetType();
    foreach (ViewVariablesTypeHandler allTypeHandler in this.GetAllTypeHandlers(type))
      stringList.AddRange(allTypeHandler.ListPath(path1));
    HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) stringList);
    foreach (MemberInfo memberInfo in (IEnumerable<MemberInfo>) ((IEnumerable<MemberInfo>) type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)).OrderBy<MemberInfo, bool>((Func<MemberInfo, bool>) (m => m.DeclaringType == type)))
    {
      VVAccess? access;
      if (ViewVariablesUtility.TryGetViewVariablesAccess(memberInfo, out access))
      {
        VVAccess? nullable = access;
        VVAccess minimumAccess = options.MinimumAccess;
        if (!(nullable.GetValueOrDefault() < minimumAccess & nullable.HasValue))
        {
          string name = memberInfo.Name;
          if (!stringSet.Add(name))
            name = $"{name}{{{memberInfo.DeclaringType?.FullName ?? typeof (void).FullName}}}";
          stringList.Add(name);
          object obj = memberInfo.GetValue(instance);
          if (options.ListIndexers)
            this.ListIndexers(obj, name, stringList);
        }
      }
    }
    if (options.ListIndexers)
      this.ListIndexers(instance, string.Empty, stringList);
    return Full(path, (IEnumerable<string>) stringList);

    static IEnumerable<string> Full(string fullPath, IEnumerable<string> relativePaths)
    {
      if (!fullPath.StartsWith('/'))
        fullPath = "/" + fullPath;
      if (fullPath.EndsWith('/'))
      {
        string str = fullPath;
        fullPath = str.Substring(0, str.Length - 1);
      }
      return relativePaths.Select<string, string>((Func<string, string>) (p =>
      {
        if (p.StartsWith("["))
          return fullPath + p;
        \u003C\u003Ey__InlineArray2<string> buffer = new \u003C\u003Ey__InlineArray2<string>();
        // ISSUE: reference to a compiler-generated method
        \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray2<string>, string>(ref buffer, 0) = fullPath;
        // ISSUE: reference to a compiler-generated method
        \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray2<string>, string>(ref buffer, 1) = p;
        // ISSUE: reference to a compiler-generated method
        return string.Join('/', \u003CPrivateImplementationDetails\u003E.InlineArrayAsReadOnlySpan<\u003C\u003Ey__InlineArray2<string>, string>(in buffer, 2));
      }));
    }

    IEnumerable<string> Domains()
    {
      return this._registeredDomains.Keys.Select<string, string>((Func<string, string>) (d => "/" + d));
    }
  }

  private void ListIndexers(object? obj, string name, List<string> paths)
  {
    if (!(obj is IDictionary dictionary))
    {
      Array array = obj as Array;
      if (array == null)
      {
        if (!(obj is IList list))
          return;
        for (int index = 0; index < list.Count; ++index)
          paths.Add($"{name}[{index}]");
      }
      else
      {
        int[] array1 = Enumerable.Range(0, array.Rank).Select<int, int>((Func<int, int>) (i => array.GetLowerBound(i))).ToArray<int>();
        int[] array2 = Enumerable.Range(0, array.Rank).Select<int, int>((Func<int, int>) (i => array.GetUpperBound(i))).ToArray<int>();
        int[] values = new int[array.Rank];
        array1.CopyTo((Array) values, 0);
        bool flag;
        do
        {
          paths.Add($"{name}[{string.Join<int>(',', (IEnumerable<int>) values)}]");
          flag = false;
          for (int index = values.Length - 1; index >= -1; --index)
          {
            if (index == -1)
            {
              flag = true;
              break;
            }
            ref int local = ref values[index];
            ++local;
            if (local > array2[index])
              local = array1[index];
            else
              break;
          }
        }
        while (!flag);
      }
    }
    else
    {
      Type type1 = typeof (void);
      Type[] genericTypeArguments = dictionary.GetType().GenericTypeArguments;
      if (genericTypeArguments != null && genericTypeArguments.Length == 2)
        type1 = genericTypeArguments[0];
      foreach (object key in (IEnumerable) dictionary.Keys)
      {
        try
        {
          Type type2 = key.GetType();
          string nodeTag = (string) null;
          if (type2 != type1)
            nodeTag = "!type:" + type2.Name;
          string str = this.SerializeValue(type2, key, nodeTag);
          if (str != null)
          {
            if (str.Contains(' '))
              str = $"({str})";
            paths.Add($"{name}[{str}]");
          }
        }
        catch (Exception ex)
        {
        }
      }
    }
  }

  public ViewVariablesPath? ResolvePath(string path)
  {
    if (string.IsNullOrEmpty(path))
      return (ViewVariablesPath) null;
    if (path.StartsWith('/'))
    {
      string str = path;
      path = str.Substring(1, str.Length - 1);
    }
    if (path.EndsWith('/'))
    {
      string str = path;
      path = str.Substring(0, str.Length - 1);
    }
    string[] array = path.Split('/');
    if (array.Length == 0)
      return (ViewVariablesPath) null;
    ViewVariablesManager.DomainData domainData;
    if (!this._registeredDomains.TryGetValue(array[0], out domainData))
      return (ViewVariablesPath) null;
    (ViewVariablesPath, string[]) valueTuple = domainData.ResolveObject(string.Join('/', RuntimeHelpers.GetSubArray<string>(array, Range.StartAt((Index) 1))));
    return this.ResolveRelativePath(valueTuple.Item1, valueTuple.Item2);
  }

  private ViewVariablesPath? ResolveRelativePath(ViewVariablesPath? path, string[] segments)
  {
    while (true)
    {
      if (path is ViewVariablesComponentPath variablesComponentPath)
        path.ParentComponent = variablesComponentPath;
      ViewVariablesComponentPath parentComponent = path?.ParentComponent;
      if (segments.Length != 0)
      {
        object obj = path?.Get();
        if (obj != null)
        {
          string segment = segments[0];
          if (string.IsNullOrEmpty(segment))
          {
            segments = RuntimeHelpers.GetSubArray<string>(segments, Range.StartAt((Index) 1));
          }
          else
          {
            MatchCollection matchCollection1 = ViewVariablesManager.TypeSpecifierRegex.Matches(segment);
            MatchCollection matchCollection2 = ViewVariablesManager.IndexerRegex.Matches(segment);
            string str1 = ViewVariablesManager.TypeSpecifierRegex.Replace(ViewVariablesManager.IndexerRegex.Replace(segment, string.Empty), string.Empty);
            if (matchCollection1.Count <= 1)
            {
              VVAccess? access1 = new VVAccess?();
              if (matchCollection1.Count != 1)
              {
                ViewVariablesPath viewVariablesPath = this.ResolveTypeHandlers(path, str1);
                if (viewVariablesPath != null)
                {
                  path = viewVariablesPath;
                  access1 = new VVAccess?(VVAccess.ReadWrite);
                  goto label_24;
                }
              }
              Type declaringType = (Type) null;
              if (matchCollection1.Count == 1)
              {
                IReflectionManager reflectionMan = this._reflectionMan;
                string str2 = matchCollection1[0].Value;
                string name = str2.Substring(1, str2.Length - 1 - 1);
                Type type = reflectionMan.GetType(name);
                if ((object) type != null)
                  declaringType = type;
              }
              MemberInfo singleMember = obj.GetType().GetSingleMember(str1, declaringType);
              if (!(singleMember == (MemberInfo) null) && ViewVariablesUtility.TryGetViewVariablesAccess(singleMember, out access1))
              {
                ViewVariablesPath viewVariablesPath;
                if ((object) (singleMember as FieldInfo) == null && (object) (singleMember as PropertyInfo) == null)
                {
                  MethodInfo method = singleMember as MethodInfo;
                  if ((object) method != null)
                    viewVariablesPath = (ViewVariablesPath) new ViewVariablesMethodPath(obj, method);
                  else
                    goto label_21;
                }
                else
                  viewVariablesPath = (ViewVariablesPath) new ViewVariablesFieldOrPropertyPath(obj, singleMember, this._entMan);
                path = viewVariablesPath;
              }
              else
                goto label_16;
label_24:
              this.UpdateParentComp(path, ref parentComponent);
              foreach (Match match in matchCollection2)
              {
                ViewVariablesPath path1 = path;
                string str3 = match.Value;
                string[] arguments = ViewVariablesManager.ParseArguments(str3.Substring(1, str3.Length - 1 - 1));
                int access2 = (int) access1.Value;
                path = this.ResolveIndexing(path1, arguments, (VVAccess) access2);
                this.UpdateParentComp(path, ref parentComponent);
              }
              segments = RuntimeHelpers.GetSubArray<string>(segments, Range.StartAt((Index) 1));
            }
            else
              goto label_9;
          }
        }
        else
          goto label_5;
      }
      else
        break;
    }
    return path;
label_5:
    return (ViewVariablesPath) null;
label_9:
    return (ViewVariablesPath) null;
label_16:
    return (ViewVariablesPath) null;
label_21:
    throw new InvalidOperationException("Invalid member! Must be a property, field or method.");
  }

  private void UpdateParentComp(ViewVariablesPath? newPath, ref ViewVariablesComponentPath? oldPath)
  {
    switch (newPath)
    {
      case null:
        break;
      case ViewVariablesComponentPath variablesComponentPath:
        newPath.ParentComponent = variablesComponentPath;
        goto case null;
      case null:
        oldPath = newPath?.ParentComponent;
        break;
      default:
        ViewVariablesPath viewVariablesPath = newPath;
        if (viewVariablesPath.ParentComponent == null)
        {
          viewVariablesPath.ParentComponent = oldPath;
          goto case null;
        }
        goto case null;
    }
  }

  private ViewVariablesPath? ResolveIndexing(
    ViewVariablesPath? path,
    string[] arguments,
    VVAccess access)
  {
    object obj = path?.Get();
    if (obj == null || arguments.Length == 0)
      return (ViewVariablesPath) null;
    Type type1 = obj.GetType();
    if (type1.IsArray && type1.GetArrayRank() > 1)
    {
      MethodInfo getter = type1.GetSingleMember("Get") as MethodInfo;
      MethodInfo setter = type1.GetSingleMember("Set") as MethodInfo;
      if (getter == (MethodInfo) null && setter == (MethodInfo) null)
        return (ViewVariablesPath) null;
      MethodInfo methodInfo = getter;
      object[] p1 = this.DeserializeArguments(((object) methodInfo != null ? ((IEnumerable<ParameterInfo>) methodInfo.GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType)).ToArray<Type>() : (Type[]) null) ?? ((IEnumerable<ParameterInfo>) RuntimeHelpers.GetSubArray<ParameterInfo>(setter.GetParameters(), Range.StartAt((Index) 1))).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType)).ToArray<Type>(), 0, arguments);
      Func<object> getter1 = new Func<object>(Get);
      Action<object> setter1 = new Action<object>(Set);
      Type type2 = getter?.ReturnType;
      if ((object) type2 == null)
        type2 = setter.GetParameters()[0].ParameterType;
      return (ViewVariablesPath) new ViewVariablesFakePath(getter1, setter1, type: type2);

      object? Get() => getter?.Invoke(obj, p1);

      void Set(object? value)
      {
        if (p1 == null || access != VVAccess.ReadWrite)
          return;
        setter?.Invoke(obj, ((IEnumerable<object>) new object[1]
        {
          value
        }).Concat<object>((IEnumerable<object>) p1).ToArray<object>());
      }
    }
    PropertyInfo indexer = type1.GetIndexer();
    if ((object) indexer == null)
      return (ViewVariablesPath) null;
    ParameterInfo[] indexParameters = indexer.GetIndexParameters();
    object[] index = this.DeserializeArguments(((IEnumerable<ParameterInfo>) indexParameters).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType)).ToArray<Type>(), ((IEnumerable<ParameterInfo>) indexParameters).Count<ParameterInfo>((Func<ParameterInfo, bool>) (p => p.IsOptional)), arguments);
    return index == null ? (ViewVariablesPath) null : (ViewVariablesPath) new ViewVariablesIndexedPath(obj, indexer, index, new VVAccess?(access));
  }

  private ViewVariablesPath? ResolveTypeHandlers(ViewVariablesPath path, string relativePath)
  {
    object obj = path.Get();
    if (obj == null || string.IsNullOrEmpty(relativePath) || relativePath.Contains('/'))
      return (ViewVariablesPath) null;
    foreach (ViewVariablesTypeHandler allTypeHandler in this.GetAllTypeHandlers(obj.GetType()))
    {
      ViewVariablesPath viewVariablesPath = allTypeHandler.HandlePath(path, relativePath);
      if (viewVariablesPath != null)
        return viewVariablesPath;
    }
    return (ViewVariablesPath) null;
  }

  private void InitializeRemote()
  {
    this._netMan.RegisterNetMessage<MsgViewVariablesReadPathReq>(new ProcessMessage<MsgViewVariablesReadPathReq>(this.ReadRemotePathRequest));
    this._netMan.RegisterNetMessage<MsgViewVariablesWritePathReq>(new ProcessMessage<MsgViewVariablesWritePathReq>(this.WriteRemotePathRequest));
    this._netMan.RegisterNetMessage<MsgViewVariablesInvokePathReq>(new ProcessMessage<MsgViewVariablesInvokePathReq>(this.InvokeRemotePathRequest));
    this._netMan.RegisterNetMessage<MsgViewVariablesListPathReq>(new ProcessMessage<MsgViewVariablesListPathReq>(this.ListRemotePathRequest));
    this._netMan.RegisterNetMessage<MsgViewVariablesReadPathRes>(new ProcessMessage<MsgViewVariablesReadPathRes>(this.ReadRemotePathResponse));
    this._netMan.RegisterNetMessage<MsgViewVariablesWritePathRes>(new ProcessMessage<MsgViewVariablesWritePathRes>(this.WriteRemotePathResponse));
    this._netMan.RegisterNetMessage<MsgViewVariablesInvokePathRes>(new ProcessMessage<MsgViewVariablesInvokePathRes>(this.InvokeRemotePathResponse));
    this._netMan.RegisterNetMessage<MsgViewVariablesListPathRes>(new ProcessMessage<MsgViewVariablesListPathRes>(this.ListRemotePathResponse));
  }

  public Task<string?> ReadRemotePath(string path, ICommonSession? session = null)
  {
    if (!this._netMan.IsConnected || this._netMan.IsServer && session == null)
      return Task.FromResult<string>((string) null);
    MsgViewVariablesReadPathReq variablesReadPathReq = new MsgViewVariablesReadPathReq();
    variablesReadPathReq.RequestId = this._nextReadRequestId++;
    variablesReadPathReq.Path = path;
    NetUserId? userId = session?.UserId;
    variablesReadPathReq.Session = userId.HasValue ? (Guid) userId.GetValueOrDefault() : Guid.Empty;
    MsgViewVariablesReadPathReq msg = variablesReadPathReq;
    TaskCompletionSource<string> completionSource = new TaskCompletionSource<string>();
    this._readRequests.Add(msg.RequestId, completionSource);
    this.SendMessage((NetMessage) msg, session?.Channel);
    return completionSource.Task;
  }

  public Task WriteRemotePath(string path, string value, ICommonSession? session = null)
  {
    if (!this._netMan.IsConnected || this._netMan.IsServer && session == null)
      return Task.CompletedTask;
    MsgViewVariablesWritePathReq variablesWritePathReq = new MsgViewVariablesWritePathReq();
    variablesWritePathReq.RequestId = this._nextWriteRequestId++;
    variablesWritePathReq.Path = path;
    variablesWritePathReq.Value = value;
    NetUserId? userId = session?.UserId;
    variablesWritePathReq.Session = userId.HasValue ? (Guid) userId.GetValueOrDefault() : Guid.Empty;
    MsgViewVariablesWritePathReq msg = variablesWritePathReq;
    TaskCompletionSource completionSource = new TaskCompletionSource();
    this._writeRequests.Add(msg.RequestId, completionSource);
    this.SendMessage((NetMessage) msg, session?.Channel);
    return completionSource.Task;
  }

  public Task<string?> InvokeRemotePath(string path, string arguments, ICommonSession? session = null)
  {
    if (!this._netMan.IsConnected || this._netMan.IsServer && session == null)
      return Task.FromResult<string>((string) null);
    MsgViewVariablesInvokePathReq variablesInvokePathReq = new MsgViewVariablesInvokePathReq();
    variablesInvokePathReq.RequestId = this._nextInvokeRequestId++;
    variablesInvokePathReq.Path = path;
    variablesInvokePathReq.Value = arguments;
    NetUserId? userId = session?.UserId;
    variablesInvokePathReq.Session = userId.HasValue ? (Guid) userId.GetValueOrDefault() : Guid.Empty;
    MsgViewVariablesInvokePathReq msg = variablesInvokePathReq;
    TaskCompletionSource<string> completionSource = new TaskCompletionSource<string>();
    this._invokeRequests.Add(msg.RequestId, completionSource);
    this.SendMessage((NetMessage) msg, session?.Channel);
    return completionSource.Task;
  }

  public Task<IEnumerable<string>> ListRemotePath(
    string path,
    VVListPathOptions options,
    ICommonSession? session = null)
  {
    if (!this._netMan.IsConnected || this._netMan.IsServer && session == null)
      return Task.FromResult<IEnumerable<string>>(Enumerable.Empty<string>());
    MsgViewVariablesListPathReq variablesListPathReq = new MsgViewVariablesListPathReq();
    variablesListPathReq.RequestId = this._nextListRequestId++;
    variablesListPathReq.Path = path;
    variablesListPathReq.Options = options;
    NetUserId? userId = session?.UserId;
    variablesListPathReq.Session = userId.HasValue ? (Guid) userId.GetValueOrDefault() : Guid.Empty;
    MsgViewVariablesListPathReq msg = variablesListPathReq;
    TaskCompletionSource<IEnumerable<string>> completionSource = new TaskCompletionSource<IEnumerable<string>>();
    this._listRequests.Add(msg.RequestId, completionSource);
    this.SendMessage((NetMessage) msg, session?.Channel);
    return completionSource.Task;
  }

  private async void ReadRemotePathRequest(MsgViewVariablesReadPathReq req)
  {
    ViewVariablesManager variablesManager1 = this;
    if (!variablesManager1.CheckPermissions(req.MsgChannel, "vvread"))
    {
      ViewVariablesManager variablesManager2 = variablesManager1;
      MsgViewVariablesReadPathRes msg = new MsgViewVariablesReadPathRes(req);
      msg.ResponseCode = ViewVariablesResponseCode.NoAccess;
      INetChannel msgChannel = req.MsgChannel;
      variablesManager2.SendMessage((NetMessage) msg, msgChannel);
    }
    else
    {
      ICommonSession session;
      if (variablesManager1._netMan.IsServer && variablesManager1.TryGetSession(req.Session, out session))
      {
        // ISSUE: explicit non-virtual call
        string str = await __nonvirtual (variablesManager1.ReadRemotePath(req.Path, session));
        ViewVariablesManager variablesManager3 = variablesManager1;
        MsgViewVariablesReadPathRes variablesReadPathRes = new MsgViewVariablesReadPathRes(req);
        variablesReadPathRes.Response = new string[1]
        {
          str ?? "null"
        };
        MsgViewVariablesReadPathRes msg = variablesReadPathRes;
        INetChannel msgChannel = req.MsgChannel;
        variablesManager3.SendMessage((NetMessage) msg, msgChannel);
      }
      else
      {
        // ISSUE: explicit non-virtual call
        string str = __nonvirtual (variablesManager1.ReadPathSerialized(req.Path));
        if (str == null)
        {
          ViewVariablesManager variablesManager4 = variablesManager1;
          MsgViewVariablesReadPathRes msg = new MsgViewVariablesReadPathRes(req);
          msg.ResponseCode = ViewVariablesResponseCode.NoObject;
          INetChannel msgChannel = req.MsgChannel;
          variablesManager4.SendMessage((NetMessage) msg, msgChannel);
        }
        else
        {
          ViewVariablesManager variablesManager5 = variablesManager1;
          MsgViewVariablesReadPathRes variablesReadPathRes = new MsgViewVariablesReadPathRes(req);
          variablesReadPathRes.Response = new string[1]
          {
            str
          };
          MsgViewVariablesReadPathRes msg = variablesReadPathRes;
          INetChannel msgChannel = req.MsgChannel;
          variablesManager5.SendMessage((NetMessage) msg, msgChannel);
        }
      }
    }
  }

  private async void WriteRemotePathRequest(MsgViewVariablesWritePathReq req)
  {
    ViewVariablesManager variablesManager1 = this;
    if (!variablesManager1.CheckPermissions(req.MsgChannel, "vvwrite"))
    {
      INetManager netMan = variablesManager1._netMan;
      MsgViewVariablesWritePathRes message = new MsgViewVariablesWritePathRes(req);
      message.ResponseCode = ViewVariablesResponseCode.NoAccess;
      INetChannel msgChannel = req.MsgChannel;
      netMan.ServerSendMessage((NetMessage) message, msgChannel);
    }
    else
    {
      ICommonSession session;
      if (variablesManager1._netMan.IsServer && variablesManager1.TryGetSession(req.Session, out session))
      {
        // ISSUE: explicit non-virtual call
        await __nonvirtual (variablesManager1.WriteRemotePath(req.Path, req.Value ?? string.Empty, session));
        variablesManager1.SendMessage((NetMessage) new MsgViewVariablesWritePathRes(req), req.MsgChannel);
      }
      else
      {
        // ISSUE: explicit non-virtual call
        ViewVariablesPath viewVariablesPath = __nonvirtual (variablesManager1.ResolvePath(req.Path));
        if (viewVariablesPath == null)
        {
          ViewVariablesManager variablesManager2 = variablesManager1;
          MsgViewVariablesWritePathRes msg = new MsgViewVariablesWritePathRes(req);
          msg.ResponseCode = ViewVariablesResponseCode.NoObject;
          INetChannel msgChannel = req.MsgChannel;
          variablesManager2.SendMessage((NetMessage) msg, msgChannel);
        }
        else
        {
          object obj = req.Value != null ? variablesManager1.DeserializeValue(viewVariablesPath.Type, req.Value) : (object) null;
          try
          {
            viewVariablesPath.Set(obj);
          }
          catch (Exception ex)
          {
            ViewVariablesManager variablesManager3 = variablesManager1;
            MsgViewVariablesWritePathRes msg = new MsgViewVariablesWritePathRes(req);
            msg.ResponseCode = ViewVariablesResponseCode.InvalidRequest;
            INetChannel msgChannel = req.MsgChannel;
            variablesManager3.SendMessage((NetMessage) msg, msgChannel);
            return;
          }
          variablesManager1.SendMessage((NetMessage) new MsgViewVariablesWritePathRes(req), req.MsgChannel);
        }
      }
    }
  }

  private async void InvokeRemotePathRequest(MsgViewVariablesInvokePathReq req)
  {
    ViewVariablesManager variablesManager1 = this;
    if (!variablesManager1.CheckPermissions(req.MsgChannel, "vvinvoke"))
    {
      INetManager netMan = variablesManager1._netMan;
      MsgViewVariablesInvokePathRes message = new MsgViewVariablesInvokePathRes(req);
      message.Path = req.Path;
      message.ResponseCode = ViewVariablesResponseCode.NoAccess;
      INetChannel msgChannel = req.MsgChannel;
      netMan.ServerSendMessage((NetMessage) message, msgChannel);
    }
    else
    {
      ICommonSession session;
      if (variablesManager1._netMan.IsServer && variablesManager1.TryGetSession(req.Session, out session))
      {
        // ISSUE: explicit non-virtual call
        string str = await __nonvirtual (variablesManager1.InvokeRemotePath(req.Path, req.Value ?? string.Empty, session));
        ViewVariablesManager variablesManager2 = variablesManager1;
        MsgViewVariablesInvokePathRes variablesInvokePathRes = new MsgViewVariablesInvokePathRes(req);
        variablesInvokePathRes.Response = new string[1]
        {
          str ?? "null"
        };
        MsgViewVariablesInvokePathRes msg = variablesInvokePathRes;
        INetChannel msgChannel = req.MsgChannel;
        variablesManager2.SendMessage((NetMessage) msg, msgChannel);
      }
      else
      {
        // ISSUE: explicit non-virtual call
        ViewVariablesPath viewVariablesPath = __nonvirtual (variablesManager1.ResolvePath(req.Path));
        if (viewVariablesPath == null)
        {
          ViewVariablesManager variablesManager3 = variablesManager1;
          MsgViewVariablesInvokePathRes msg = new MsgViewVariablesInvokePathRes(req);
          msg.ResponseCode = ViewVariablesResponseCode.NoObject;
          INetChannel msgChannel = req.MsgChannel;
          variablesManager3.SendMessage((NetMessage) msg, msgChannel);
        }
        else
        {
          string[] arguments = req.Value != null ? ViewVariablesManager.ParseArguments(req.Value) : Array.Empty<string>();
          object[] parameters = variablesManager1.DeserializeArguments(viewVariablesPath.InvokeParameterTypes, (int) viewVariablesPath.InvokeOptionalParameters, arguments);
          object obj;
          try
          {
            obj = viewVariablesPath.Invoke(parameters);
          }
          catch (Exception ex)
          {
            ViewVariablesManager variablesManager4 = variablesManager1;
            MsgViewVariablesInvokePathRes msg = new MsgViewVariablesInvokePathRes(req);
            msg.ResponseCode = ViewVariablesResponseCode.InvalidRequest;
            INetChannel msgChannel = req.MsgChannel;
            variablesManager4.SendMessage((NetMessage) msg, msgChannel);
            return;
          }
          string str;
          try
          {
            str = variablesManager1.SerializeValue(viewVariablesPath.InvokeReturnType, obj) ?? obj?.ToString() ?? "null";
          }
          catch (Exception ex)
          {
            str = obj?.ToString() ?? "null";
          }
          ViewVariablesManager variablesManager5 = variablesManager1;
          MsgViewVariablesInvokePathRes variablesInvokePathRes = new MsgViewVariablesInvokePathRes(req);
          variablesInvokePathRes.Response = new string[1]
          {
            str
          };
          MsgViewVariablesInvokePathRes msg1 = variablesInvokePathRes;
          INetChannel msgChannel1 = req.MsgChannel;
          variablesManager5.SendMessage((NetMessage) msg1, msgChannel1);
        }
      }
    }
  }

  private async void ListRemotePathRequest(MsgViewVariablesListPathReq req)
  {
    ViewVariablesManager variablesManager1 = this;
    if (!variablesManager1.CheckPermissions(req.MsgChannel, "vv"))
    {
      INetManager netMan = variablesManager1._netMan;
      MsgViewVariablesListPathRes message = new MsgViewVariablesListPathRes(req);
      message.ResponseCode = ViewVariablesResponseCode.NoAccess;
      INetChannel msgChannel = req.MsgChannel;
      netMan.ServerSendMessage((NetMessage) message, msgChannel);
    }
    else
    {
      ICommonSession session;
      if (variablesManager1._netMan.IsServer && variablesManager1.TryGetSession(req.Session, out session))
      {
        // ISSUE: explicit non-virtual call
        IEnumerable<string> source = await __nonvirtual (variablesManager1.ListRemotePath(req.Path, req.Options, session));
        ViewVariablesManager variablesManager2 = variablesManager1;
        MsgViewVariablesListPathRes msg = new MsgViewVariablesListPathRes(req);
        msg.Response = source.ToArray<string>();
        INetChannel msgChannel = req.MsgChannel;
        variablesManager2.SendMessage((NetMessage) msg, msgChannel);
      }
      else
      {
        // ISSUE: explicit non-virtual call
        string[] array = __nonvirtual (variablesManager1.ListPath(req.Path, req.Options)).OrderByDescending<string, bool>((Func<string, bool>) (p => p.StartsWith(req.Path))).Take<string>(Math.Min(500, req.Options.RemoteListLength)).ToArray<string>();
        ViewVariablesManager variablesManager3 = variablesManager1;
        MsgViewVariablesListPathRes msg = new MsgViewVariablesListPathRes(req);
        msg.Response = array;
        INetChannel msgChannel = req.MsgChannel;
        variablesManager3.SendMessage((NetMessage) msg, msgChannel);
      }
    }
  }

  private void ReadRemotePathResponse(MsgViewVariablesReadPathRes res)
  {
    TaskCompletionSource<string> completionSource;
    if (!this._readRequests.Remove(res.RequestId, out completionSource))
      return;
    if (res.ResponseCode != ViewVariablesResponseCode.Ok)
      completionSource.TrySetResult((string) null);
    else if (res.Response.Length == 0)
      completionSource.TrySetResult((string) null);
    else
      completionSource.TrySetResult(res.Response[0]);
  }

  private void WriteRemotePathResponse(MsgViewVariablesWritePathRes res)
  {
    TaskCompletionSource completionSource;
    if (!this._writeRequests.Remove(res.RequestId, out completionSource))
      return;
    completionSource.SetResult();
  }

  private void InvokeRemotePathResponse(MsgViewVariablesInvokePathRes res)
  {
    TaskCompletionSource<string> completionSource;
    if (!this._invokeRequests.Remove(res.RequestId, out completionSource))
      return;
    if (res.ResponseCode != ViewVariablesResponseCode.Ok)
      completionSource.TrySetResult((string) null);
    else if (res.Response.Length == 0)
      completionSource.TrySetResult((string) null);
    else
      completionSource.TrySetResult(res.Response[0]);
  }

  private void ListRemotePathResponse(MsgViewVariablesListPathRes res)
  {
    TaskCompletionSource<IEnumerable<string>> completionSource;
    if (!this._listRequests.Remove(res.RequestId, out completionSource))
      return;
    if (res.ResponseCode != ViewVariablesResponseCode.Ok)
      completionSource.TrySetResult(Enumerable.Empty<string>());
    else
      completionSource.TrySetResult((IEnumerable<string>) res.Response);
  }

  private void SendMessage(NetMessage msg, INetChannel? channel = null)
  {
    if (this._netMan.IsServer)
    {
      if (channel == null)
        throw new ArgumentNullException(nameof (channel));
      this._netMan.ServerSendMessage(msg, channel);
    }
    else
      this._netMan.ClientSendMessage(msg);
  }

  protected abstract bool CheckPermissions(INetChannel channel, string command);

  protected abstract bool TryGetSession(Guid guid, [NotNullWhen(true)] out ICommonSession? session);

  private static string[] ParseArguments(string arguments)
  {
    List<string> stringList = new List<string>();
    bool flag = false;
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; index < arguments.Length; ++index)
    {
      char c = arguments[index];
      switch (c)
      {
        case '(':
          flag = true;
          break;
        case ')':
          if (flag)
          {
            flag = false;
            break;
          }
          goto default;
        case ',':
          if (!flag)
          {
            stringList.Add(stringBuilder.ToString());
            stringBuilder.Clear();
            break;
          }
          goto default;
        default:
          if (flag || !char.IsWhiteSpace(c))
          {
            stringBuilder.Append(c);
            break;
          }
          break;
      }
    }
    if (stringBuilder.Length != 0)
      stringList.Add(stringBuilder.ToString());
    return stringList.ToArray();
  }

  private object?[]? DeserializeArguments(
    Type[] argumentTypes,
    int optionalArguments,
    string[] arguments)
  {
    if (arguments.Length < argumentTypes.Length - optionalArguments || arguments.Length > argumentTypes.Length)
      return (object[]) null;
    List<object> objectList = new List<object>();
    for (int index = 0; index < arguments.Length; ++index)
    {
      string str = arguments[index];
      object obj = this.DeserializeValue(argumentTypes[index], str);
      objectList.Add(obj);
    }
    for (int index = 0; index < argumentTypes.Length - arguments.Length; ++index)
      objectList.Add(Type.Missing);
    return objectList.ToArray();
  }

  private object? DeserializeValue(Type type, string value)
  {
    object obj = this.ResolvePath(value)?.Get();
    if (obj != null && obj.GetType().IsAssignableTo(type))
      return obj;
    try
    {
      using (TextReader textReader = (TextReader) new StringReader(value))
      {
        YamlStream yamlStream = new YamlStream();
        yamlStream.Load(textReader);
        YamlNode rootNode = yamlStream.Documents[0].RootNode;
        return this._serMan.Read(type, rootNode.ToDataNode());
      }
    }
    catch (Exception ex)
    {
      return (object) null;
    }
  }

  private string? SerializeValue(Type type, object? value, string? nodeTag = null)
  {
    if (value == null || type == typeof (void))
      return (string) null;
    DataNode node = this._serMan.WriteValue(type, value, true);
    if (!string.IsNullOrEmpty(nodeTag))
      node.Tag = nodeTag;
    YamlDocument yamlDocument = new YamlDocument(node.ToYamlNode());
    YamlStream yamlStream = new YamlStream();
    yamlStream.Add(yamlDocument);
    using (StringWriter stringWriter = new StringWriter(new StringBuilder()))
    {
      yamlStream.Save((IEmitter) new YamlNoDocEndDotsFix((IEmitter) new YamlMappingFix((IEmitter) new Emitter((TextWriter) stringWriter))), false);
      return stringWriter.ToString();
    }
  }

  public ViewVariablesTypeHandler<T> GetTypeHandler<T>()
  {
    ViewVariablesTypeHandler typeHandler1;
    if (this._typeHandlers.TryGetValue(typeof (T), out typeHandler1))
      return (ViewVariablesTypeHandler<T>) typeHandler1;
    ViewVariablesTypeHandler<T> typeHandler2 = new ViewVariablesTypeHandler<T>(this.Sawmill);
    this._typeHandlers.Add(typeof (T), (ViewVariablesTypeHandler) typeHandler2);
    return typeHandler2;
  }

  private void InitializeTypeHandlers()
  {
    this.GetTypeHandler<EntityUid>().AddHandler(new HandleTypePath<EntityUid>(this.EntityComponentHandler), new ListTypeCustomPaths<EntityUid>(this.EntityComponentList)).AddPath("Delete", (PathHandler<EntityUid>) (uid => (ViewVariablesPath) ViewVariablesPath.FromInvoker((Action<object>) (_ => this._entMan.DeleteEntity(new EntityUid?(uid)))))).AddPath("QueueDelete", (PathHandler<EntityUid>) (uid => (ViewVariablesPath) ViewVariablesPath.FromInvoker((Action<object>) (_ => this._entMan.QueueDeleteEntity(new EntityUid?(uid))))));
  }

  private ViewVariablesPath? EntityComponentHandler(EntityUid uid, string relativePath)
  {
    ComponentRegistration registration;
    IComponent component;
    return !this._entMan.EntityExists(uid) || !this._compFact.TryGetRegistration(relativePath, out registration, true) || !this._entMan.TryGetComponent(uid, registration.Idx, out component) ? (ViewVariablesPath) null : (ViewVariablesPath) new ViewVariablesComponentPath(component, uid);
  }

  private IEnumerable<string> EntityComponentList(EntityUid uid)
  {
    return this._entMan.GetComponents(uid).Select<IComponent, string>((Func<IComponent, string>) (component => this._compFact.GetComponentName(component.GetType())));
  }

  private IEnumerable<ViewVariablesTypeHandler> GetAllTypeHandlers(Type origType)
  {
    for (Type type = origType; type != (Type) null; type = type.BaseType)
    {
      ViewVariablesTypeHandler allTypeHandler;
      if (this._typeHandlers.TryGetValue(type, out allTypeHandler))
        yield return allTypeHandler;
    }
    Type[] typeArray = origType.GetInterfaces();
    for (int index = 0; index < typeArray.Length; ++index)
    {
      ViewVariablesTypeHandler allTypeHandler;
      if (this._typeHandlers.TryGetValue(typeArray[index], out allTypeHandler))
        yield return allTypeHandler;
    }
    typeArray = (Type[]) null;
  }

  [DataDefinition]
  private sealed class VvTest : 
    IEnumerable<object>,
    IEnumerable,
    ISerializationGenerated<ViewVariablesManager.VvTest>,
    ISerializationGenerated
  {
    [DataField("x", false, 1, false, false, null)]
    [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
    private int X = 10;
    [Robust.Shared.ViewVariables.ViewVariables]
    public Dictionary<object, object> Dict = new Dictionary<object, object>()
    {
      {
        (object) "a",
        (object) "b"
      },
      {
        (object) "c",
        (object) "d"
      }
    };
    [DataField("multiDimensionalArray", false, 1, false, false, null)]
    public int[,] MultiDimensionalArray = new int[5, 2]
    {
      {
        1,
        2
      },
      {
        3,
        4
      },
      {
        5,
        6
      },
      {
        7,
        8
      },
      {
        9,
        0
      }
    };
    [DataField("vector", false, 1, false, false, null)]
    [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
    private Vector2 Vector = new Vector2(50f, 50f);
    [DataField("data", false, 1, false, false, null)]
    [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
    private ViewVariablesManager.VvTest.ComplexDataStructure Data = new ViewVariablesManager.VvTest.ComplexDataStructure();

    [Robust.Shared.ViewVariables.ViewVariables]
    public List<object> List
    {
      get
      {
        return new List<object>()
        {
          (object) 1,
          (object) 2,
          (object) 3,
          (object) 4,
          (object) 5,
          (object) 6,
          (object) 7,
          (object) 8,
          (object) 9,
          (object) this.X,
          (object) 11,
          (object) 12,
          (object) 13,
          (object) 14,
          (object) 15,
          (object) this
        };
      }
    }

    public IEnumerator<object> GetEnumerator() => (IEnumerator<object>) this.List.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref ViewVariablesManager.VvTest target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      if (serialization.TryCustomCopy<ViewVariablesManager.VvTest>(this, ref target, hookCtx, false, context))
        return;
      int target1 = 0;
      if (!serialization.TryCustomCopy<int>(this.X, ref target1, hookCtx, false, context))
        target1 = this.X;
      target.X = target1;
      int[,] target2 = (int[,]) null;
      if (this.MultiDimensionalArray == null)
        throw new NullNotAllowedException();
      if (!serialization.TryCustomCopy<int[,]>(this.MultiDimensionalArray, ref target2, hookCtx, true, context))
        target2 = serialization.CreateCopy<int[,]>(this.MultiDimensionalArray, hookCtx, context);
      target.MultiDimensionalArray = target2;
      Vector2 target3 = new Vector2();
      if (!serialization.TryCustomCopy<Vector2>(this.Vector, ref target3, hookCtx, false, context))
        target3 = serialization.CreateCopy<Vector2>(this.Vector, hookCtx, context);
      target.Vector = target3;
      ViewVariablesManager.VvTest.ComplexDataStructure target4 = new ViewVariablesManager.VvTest.ComplexDataStructure();
      if (!serialization.TryCustomCopy<ViewVariablesManager.VvTest.ComplexDataStructure>(this.Data, ref target4, hookCtx, false, context))
        serialization.CopyTo<ViewVariablesManager.VvTest.ComplexDataStructure>(this.Data, ref target4, hookCtx, context);
      target.Data = target4;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref ViewVariablesManager.VvTest target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      ViewVariablesManager.VvTest target1 = (ViewVariablesManager.VvTest) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    public ViewVariablesManager.VvTest Instantiate() => new ViewVariablesManager.VvTest();

    [DataDefinition]
    private struct ComplexDataStructure : 
      ISerializationGenerated<ViewVariablesManager.VvTest.ComplexDataStructure>,
      ISerializationGenerated
    {
      [DataField("X", false, 1, false, false, null)]
      [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
      public int X;
      [DataField("Y", false, 1, false, false, null)]
      [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
      public int Y;

      public ComplexDataStructure()
      {
        this.X = 5;
        this.Y = 15;
      }

      [Obsolete("Use ISerializationManager.CopyTo instead")]
      public void InternalCopy(
        ref ViewVariablesManager.VvTest.ComplexDataStructure target,
        ISerializationManager serialization,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null)
      {
        if (serialization.TryCustomCopy<ViewVariablesManager.VvTest.ComplexDataStructure>(this, ref target, hookCtx, false, context))
          return;
        int target1 = 0;
        if (!serialization.TryCustomCopy<int>(this.X, ref target1, hookCtx, false, context))
          target1 = this.X;
        int target2 = 0;
        if (!serialization.TryCustomCopy<int>(this.Y, ref target2, hookCtx, false, context))
          target2 = this.Y;
        target = target with { X = target1, Y = target2 };
      }

      [Obsolete("Use ISerializationManager.CopyTo instead")]
      public void Copy(
        ref ViewVariablesManager.VvTest.ComplexDataStructure target,
        ISerializationManager serialization,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null)
      {
        this.InternalCopy(ref target, serialization, hookCtx, context);
      }

      [Obsolete("Use ISerializationManager.CopyTo instead")]
      public void Copy(
        ref object target,
        ISerializationManager serialization,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null)
      {
        ViewVariablesManager.VvTest.ComplexDataStructure target1 = (ViewVariablesManager.VvTest.ComplexDataStructure) target;
        this.Copy(ref target1, serialization, hookCtx, context);
        target = (object) target1;
      }

      [Obsolete("Use ISerializationManager.CreateCopy instead")]
      public ViewVariablesManager.VvTest.ComplexDataStructure Instantiate()
      {
        return new ViewVariablesManager.VvTest.ComplexDataStructure();
      }
    }
  }

  internal sealed class DomainData
  {
    public readonly DomainResolveObject ResolveObject;
    public readonly DomainListPaths List;

    public DomainData(DomainResolveObject resolveObject, DomainListPaths list)
    {
      this.ResolveObject = resolveObject;
      this.List = list;
    }
  }
}
