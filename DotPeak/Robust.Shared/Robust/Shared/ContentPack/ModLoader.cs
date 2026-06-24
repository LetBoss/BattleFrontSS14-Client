// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.ModLoader
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.ExceptionServices;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.ContentPack;

internal sealed class ModLoader : BaseModLoader, IModLoaderInternal, IModLoader, IDisposable
{
  [Dependency]
  private readonly IResourceManagerInternal _res;
  private readonly List<Assembly> _sideModules = new List<Assembly>();
  private readonly AssemblyLoadContext _loadContext;
  private readonly object _lock = new object();
  private static int _modLoaderId;
  private bool _useLoadContext = true;
  private bool _sandboxingEnabled;
  private readonly List<string> _engineModuleDirectories = new List<string>();
  private readonly List<ExtraModuleLoad> _extraModuleLoads = new List<ExtraModuleLoad>();

  public event ExtraModuleLoad ExtraModuleLoaders
  {
    add => this._extraModuleLoads.Add(value);
    remove => this._extraModuleLoads.Remove(value);
  }

  public ModLoader()
  {
    this._loadContext = new AssemblyLoadContext($"ModLoader-{Interlocked.Increment(ref ModLoader._modLoaderId)}");
    this._loadContext.Resolving += new Func<AssemblyLoadContext, AssemblyName, Assembly>(this.ResolvingAssembly);
    AssemblyLoadContext.Default.Resolving += new Func<AssemblyLoadContext, AssemblyName, Assembly>(this.DefaultOnResolving);
  }

  public void SetUseLoadContext(bool useLoadContext)
  {
    this._useLoadContext = useLoadContext;
    this.Sawmill.Debug("{0} assembly load context", useLoadContext ? (object) "ENABLING" : (object) "DISABLING");
  }

  public void SetEnableSandboxing(bool sandboxing)
  {
    this._sandboxingEnabled = sandboxing;
    this.Sawmill.Debug("{0} sandboxing", sandboxing ? (object) "ENABLING" : (object) "DISABLING");
  }

  public Func<string, Stream?>? VerifierExtraLoadHandler { get; set; }

  public void AddEngineModuleDirectory(string dir) => this._engineModuleDirectories.Add(dir);

  public bool TryLoadModulesFrom(ResPath mountPath, string filterPrefix)
  {
    List<ResPath> paths = new List<ResPath>();
    foreach (ResPath resPath1 in this._res.ContentFindRelativeFiles(mountPath).Where<ResPath>((Func<ResPath, bool>) (p => p.Filename.StartsWith(filterPrefix) && p.Extension == "dll")))
    {
      ResPath resPath2 = mountPath / resPath1;
      this.Sawmill.Debug($"Found module '{resPath2}'");
      paths.Add(resPath2);
    }
    return this.TryLoadModules((IEnumerable<ResPath>) paths);
  }

  public bool TryLoadModules(IEnumerable<ResPath> paths)
  {
    Stopwatch stopwatch1 = Stopwatch.StartNew();
    this.Sawmill.Debug("LOADING modules");
    Dictionary<string, (ResPath, MemoryStream, string[])> dictionary = new Dictionary<string, (ResPath, MemoryStream, string[])>();
    foreach (ResPath path in paths)
    {
      using (Stream stream = this._res.ContentFileRead(path))
      {
        MemoryStream destination = new MemoryStream();
        stream.CopyTo((Stream) destination);
        destination.Position = 0L;
        (string[] refs, string name)? assemblyReferenceData = this.GetAssemblyReferenceData((Stream) destination);
        if (assemblyReferenceData.HasValue)
        {
          (string[] refs, string str) = assemblyReferenceData.Value;
          if (!dictionary.TryAdd(str, (path, destination, refs)))
          {
            this.Sawmill.Error("Found multiple modules with the same assembly name " + $"'{str}', A: {dictionary[str].Item1}, B: {path}.");
            return false;
          }
        }
      }
    }
    if (this._sandboxingEnabled)
    {
      Stopwatch stopwatch2 = Stopwatch.StartNew();
      AssemblyTypeChecker typeChecker = this.MakeTypeChecker();
      AssemblyTypeChecker.Resolver resolver = typeChecker.CreateResolver();
      Parallel.ForEach<KeyValuePair<string, (ResPath, MemoryStream, string[])>>((IEnumerable<KeyValuePair<string, (ResPath, MemoryStream, string[])>>) dictionary, (Action<KeyValuePair<string, (ResPath, MemoryStream, string[])>>) (pair =>
      {
        (string key2, (ResPath _, MemoryStream memoryStream2, string[] _)) = pair;
        memoryStream2.Position = 0L;
        if (!typeChecker.CheckAssembly((Stream) memoryStream2, resolver))
          throw new TypeCheckFailedException($"Assembly {key2} failed type checks.");
      }));
      this.Sawmill.Debug($"Verified assemblies in {stopwatch2.ElapsedMilliseconds}ms");
    }
    foreach ((ResPath path, MemoryStream assembly, string[] _) in TopologicalSort.Sort<(ResPath, MemoryStream, string[])>(TopologicalSort.FromBeforeAfter<KeyValuePair<string, (ResPath, MemoryStream, string[])>, string, (ResPath, MemoryStream, string[])>((IEnumerable<KeyValuePair<string, (ResPath, MemoryStream, string[])>>) dictionary, (Func<KeyValuePair<string, (ResPath, MemoryStream, string[])>, string>) (kv => kv.Key), (Func<KeyValuePair<string, (ResPath, MemoryStream, string[])>, (ResPath, MemoryStream, string[])>) (kv => kv.Value), (Func<KeyValuePair<string, (ResPath, MemoryStream, string[])>, IEnumerable<string>>) (_ => (IEnumerable<string>) Array.Empty<string>()), (Func<KeyValuePair<string, (ResPath, MemoryStream, string[])>, IEnumerable<string>>) (kv => (IEnumerable<string>) kv.Value.references), true)))
    {
      this.Sawmill.Debug($"Loading module: '{path}'");
      try
      {
        string diskPath;
        if (this._res.TryGetDiskFilePath(path, out diskPath))
        {
          this.LoadGameAssembly(diskPath, true);
        }
        else
        {
          assembly.Position = 0L;
          using (Stream symbols = this._res.ContentFileReadOrNull(path.WithExtension("pdb")))
            this.LoadGameAssembly((Stream) assembly, symbols, true);
        }
      }
      catch (Exception ex)
      {
        this.Sawmill.Error($"Exception loading module '{path}':\n{ex.ToStringBetter()}");
        return false;
      }
    }
    this.Sawmill.Debug($"DONE loading modules: {stopwatch1.Elapsed}");
    return true;
  }

  private (string[] refs, string name)? GetAssemblyReferenceData(Stream stream)
  {
    using (PEReader peReader = ModLoader.MakePEReader(stream, true))
    {
      MetadataReader metaReader = peReader.GetMetadataReader();
      string str = metaReader.GetString(metaReader.GetAssemblyDefinition().Name);
      if (!this._sandboxingEnabled || !ModLoader.TryFindSkipIfSandboxed(metaReader))
        return new (string[], string)?((metaReader.AssemblyReferences.Select<AssemblyReferenceHandle, AssemblyReference>((Func<AssemblyReferenceHandle, AssemblyReference>) (a => metaReader.GetAssemblyReference(a))).Select<AssemblyReference, string>((Func<AssemblyReference, string>) (a => metaReader.GetString(a.Name))).ToArray<string>(), str));
      this.Sawmill.Debug("Module {ModuleName} has SkipIfSandboxedAttribute, ignoring.", (object) str);
      return new (string[], string)?();
    }
  }

  private static bool TryFindSkipIfSandboxed(MetadataReader reader)
  {
    foreach (CustomAttributeHandle customAttribute1 in reader.CustomAttributes)
    {
      CustomAttribute customAttribute2 = reader.GetCustomAttribute(customAttribute1);
      if (customAttribute2.Parent.Kind == HandleKind.AssemblyDefinition)
      {
        EntityHandle constructor = customAttribute2.Constructor;
        if (constructor.Kind == HandleKind.MemberReference)
        {
          MemberReference memberReference = reader.GetMemberReference((MemberReferenceHandle) constructor);
          AssemblyTypeChecker.MTypeReferenced typeReference = AssemblyTypeChecker.ParseTypeReference(reader, (TypeReferenceHandle) memberReference.Parent);
          if (typeReference.Namespace == "Robust.Shared.ContentPack" && typeReference.Name == "SkipIfSandboxedAttribute")
            return true;
        }
      }
    }
    return false;
  }

  public void LoadGameAssembly(Stream assembly, Stream? symbols = null, bool skipVerify = false)
  {
    if (!skipVerify && !this.MakeTypeChecker().CheckAssembly(assembly))
      throw new TypeCheckFailedException();
    assembly.Position = 0L;
    this.InitMod(!this._useLoadContext ? Assembly.Load(assembly.CopyToArray(), symbols != null ? symbols.CopyToArray() : (byte[]) null) : this._loadContext.LoadFromStream(assembly, symbols));
  }

  public void LoadGameAssembly(string diskPath, bool skipVerify = false)
  {
    if (!skipVerify && !this.MakeTypeChecker().CheckAssembly(diskPath))
      throw new TypeCheckFailedException();
    this.InitMod(!this._useLoadContext ? Assembly.LoadFrom(diskPath) : this._loadContext.LoadFromAssemblyPath(diskPath));
  }

  public bool TryLoadAssembly(string assemblyName)
  {
    ResPath path = new ResPath($"/Assemblies/{assemblyName}.dll");
    string diskPath;
    if (this._res.TryGetDiskFilePath(path, out diskPath))
    {
      this.Sawmill.Debug($"Loading {assemblyName} DLL");
      try
      {
        this.LoadGameAssembly(diskPath, false);
        return true;
      }
      catch (Exception ex)
      {
        this.Sawmill.Error($"Exception loading DLL {assemblyName}.dll: {ex.ToStringBetter()}");
        return false;
      }
    }
    else
    {
      Stream fileStream1;
      if (this._res.TryContentFileRead(new ResPath?(path), out fileStream1))
      {
        using (fileStream1)
        {
          this.Sawmill.Debug($"Loading {assemblyName} DLL");
          Stream fileStream2;
          if (this._res.TryContentFileRead(new ResPath?(new ResPath($"/Assemblies/{assemblyName}.pdb")), out fileStream2))
          {
            using (fileStream2)
            {
              try
              {
                this.LoadGameAssembly(fileStream1, fileStream2, false);
                return true;
              }
              catch (Exception ex)
              {
                this.Sawmill.Error($"Exception loading DLL {assemblyName}.dll: {ex.ToStringBetter()}");
                return false;
              }
            }
          }
          else
          {
            try
            {
              this.LoadGameAssembly(fileStream1, (Stream) null, false);
              return true;
            }
            catch (Exception ex)
            {
              this.Sawmill.Error($"Exception loading DLL {assemblyName}.dll: {ex.ToStringBetter()}");
              return false;
            }
          }
        }
      }
      else
      {
        this.Sawmill.Warning($"Could not load {assemblyName} DLL: {path} does not exist in the VFS.");
        return false;
      }
    }
  }

  private Assembly? ResolvingAssembly(AssemblyLoadContext context, AssemblyName name)
  {
    try
    {
      lock (this._lock)
      {
        this.Sawmill.Verbose("ResolvingAssembly {0}", (object) name);
        foreach (BaseModLoader.ModInfo mod in this.Mods)
        {
          if (mod.GameAssembly.FullName == name.FullName)
          {
            this.Sawmill.Verbose($"Found assembly in modloader ALC: {mod.GameAssembly}");
            return mod.GameAssembly;
          }
        }
        Assembly assembly1 = this.TryLoadExtra(name);
        if ((object) assembly1 != null)
        {
          this.Sawmill.Verbose($"Found assembly through extra loader: {assembly1}");
          return assembly1;
        }
        if (!this._sandboxingEnabled)
        {
          foreach (Assembly sideModule in this._sideModules)
          {
            if (sideModule.FullName == name.FullName)
            {
              this.Sawmill.Verbose($"Found assembly in existing side modules: {sideModule}");
              return sideModule;
            }
          }
          try
          {
            Assembly assembly2 = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(name.Name));
            this.Sawmill.Verbose($"Found assembly through default ALC (early): {assembly2}");
            return assembly2;
          }
          catch
          {
          }
          Stream fileStream;
          if (this._res.TryContentFileRead($"/Assemblies/{name.Name}.dll", out fileStream))
          {
            Assembly assembly3 = this._loadContext.LoadFromStream(fileStream);
            this._sideModules.Add(assembly3);
            this.Sawmill.Verbose($"Found assembly in NEW side module: {assembly3}");
            return assembly3;
          }
        }
        this.Sawmill.Verbose("Did not find assembly directly. Should fall back to default ALC.");
        return (Assembly) null;
      }
    }
    catch (Exception ex)
    {
      this.Sawmill.Error("Exception in ResolvingAssembly: {0}", (object) ex);
      ExceptionDispatchInfo.Capture(ex).Throw();
      throw null;
    }
  }

  public void Dispose()
  {
    AssemblyLoadContext.Default.Resolving -= new Func<AssemblyLoadContext, AssemblyName, Assembly>(this.DefaultOnResolving);
  }

  private Assembly? DefaultOnResolving(AssemblyLoadContext ctx, AssemblyName name)
  {
    if (this._useLoadContext)
    {
      this.Sawmill.Verbose($"RESOLVING DEFAULT: {name}");
      foreach (Assembly loadedModule in this.LoadedModules)
      {
        if (loadedModule.GetName().Name == name.Name)
          return loadedModule;
      }
      foreach (Assembly sideModule in this._sideModules)
      {
        if (sideModule.GetName().Name == name.Name)
          return sideModule;
      }
      Assembly assembly = this.TryLoadExtra(name);
      if ((object) assembly != null)
        return assembly;
    }
    return (Assembly) null;
  }

  private Assembly? TryLoadExtra(AssemblyName name)
  {
    foreach (ExtraModuleLoad extraModuleLoad in this._extraModuleLoads)
    {
      Assembly assembly = extraModuleLoad(name);
      if ((object) assembly != null)
        return assembly;
    }
    return (Assembly) null;
  }

  private AssemblyTypeChecker MakeTypeChecker()
  {
    return new AssemblyTypeChecker((IResourceManager) this._res, this.LogManager.GetSawmill("res.typecheck"))
    {
      VerifyIL = this._sandboxingEnabled,
      DisableTypeCheck = !this._sandboxingEnabled,
      ExtraRobustLoader = this.VerifierExtraLoadHandler,
      EngineModuleDirectories = this._engineModuleDirectories.ToArray()
    };
  }

  internal static PEReader MakePEReader(Stream stream, bool leaveOpen = false, PEStreamOptions options = PEStreamOptions.Default)
  {
    if (!stream.CanSeek)
      stream = leaveOpen ? (Stream) stream.CopyToMemoryStream() : (Stream) stream.ConsumeToMemoryStream();
    if (leaveOpen)
      options |= PEStreamOptions.LeaveOpen;
    return new PEReader(stream, options);
  }
}
