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
using Robust.Shared.IoC;
using Robust.Shared.Utility;

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

	public Func<string, Stream?>? VerifierExtraLoadHandler { get; set; }

	public event ExtraModuleLoad ExtraModuleLoaders
	{
		add
		{
			_extraModuleLoads.Add(value);
		}
		remove
		{
			_extraModuleLoads.Remove(value);
		}
	}

	public ModLoader()
	{
		int value = Interlocked.Increment(ref _modLoaderId);
		_loadContext = new AssemblyLoadContext($"ModLoader-{value}");
		_loadContext.Resolving += ResolvingAssembly;
		AssemblyLoadContext.Default.Resolving += DefaultOnResolving;
	}

	public void SetUseLoadContext(bool useLoadContext)
	{
		_useLoadContext = useLoadContext;
		base.Sawmill.Debug("{0} assembly load context", useLoadContext ? "ENABLING" : "DISABLING");
	}

	public void SetEnableSandboxing(bool sandboxing)
	{
		_sandboxingEnabled = sandboxing;
		base.Sawmill.Debug("{0} sandboxing", sandboxing ? "ENABLING" : "DISABLING");
	}

	public void AddEngineModuleDirectory(string dir)
	{
		_engineModuleDirectories.Add(dir);
	}

	public bool TryLoadModulesFrom(ResPath mountPath, string filterPrefix)
	{
		List<ResPath> list = new List<ResPath>();
		foreach (ResPath item in from p in _res.ContentFindRelativeFiles(mountPath)
			where p.Filename.StartsWith(filterPrefix) && p.Extension == "dll"
			select p)
		{
			ResPath resPath = mountPath / item;
			base.Sawmill.Debug($"Found module '{resPath}'");
			list.Add(resPath);
		}
		return TryLoadModules(list);
	}

	public bool TryLoadModules(IEnumerable<ResPath> paths)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		base.Sawmill.Debug("LOADING modules");
		Dictionary<string, (ResPath, MemoryStream, string[])> dictionary = new Dictionary<string, (ResPath, MemoryStream, string[])>();
		foreach (ResPath path in paths)
		{
			using Stream stream = _res.ContentFileRead(path);
			MemoryStream memoryStream = new MemoryStream();
			stream.CopyTo(memoryStream);
			memoryStream.Position = 0L;
			(string[], string)? assemblyReferenceData = GetAssemblyReferenceData(memoryStream);
			if (assemblyReferenceData.HasValue)
			{
				var (item, text) = assemblyReferenceData.Value;
				if (!dictionary.TryAdd(text, (path, memoryStream, item)))
				{
					base.Sawmill.Error("Found multiple modules with the same assembly name " + $"'{text}', A: {dictionary[text].Item1}, B: {path}.");
					return false;
				}
			}
		}
		if (_sandboxingEnabled)
		{
			Stopwatch stopwatch2 = Stopwatch.StartNew();
			AssemblyTypeChecker typeChecker = MakeTypeChecker();
			AssemblyTypeChecker.Resolver resolver = typeChecker.CreateResolver();
			Parallel.ForEach<KeyValuePair<string, (ResPath, MemoryStream, string[])>>(dictionary, delegate(KeyValuePair<string, (ResPath Path, MemoryStream data, string[] references)> pair)
			{
				KeyValuePair<string, (ResPath, MemoryStream, string[])> keyValuePair = pair;
				keyValuePair.Deconstruct(out var key, out var value);
				(ResPath, MemoryStream, string[]) tuple3 = value;
				string text2 = key;
				MemoryStream item2 = tuple3.Item2;
				item2.Position = 0L;
				if (!typeChecker.CheckAssembly(item2, resolver))
				{
					throw new TypeCheckFailedException("Assembly " + text2 + " failed type checks.");
				}
			});
			base.Sawmill.Debug($"Verified assemblies in {stopwatch2.ElapsedMilliseconds}ms");
		}
		foreach (var (resPath, memoryStream2, _) in TopologicalSort.Sort(TopologicalSort.FromBeforeAfter<KeyValuePair<string, (ResPath, MemoryStream, string[])>, string, (ResPath, MemoryStream, string[])>(dictionary, (KeyValuePair<string, (ResPath Path, MemoryStream data, string[] references)> kv) => kv.Key, (KeyValuePair<string, (ResPath Path, MemoryStream data, string[] references)> kv) => kv.Value, (KeyValuePair<string, (ResPath Path, MemoryStream data, string[] references)> _) => Array.Empty<string>(), (KeyValuePair<string, (ResPath Path, MemoryStream data, string[] references)> kv) => kv.Value.references, allowMissing: true)))
		{
			base.Sawmill.Debug($"Loading module: '{resPath}'");
			try
			{
				if (_res.TryGetDiskFilePath(resPath, out string diskPath))
				{
					LoadGameAssembly(diskPath, skipVerify: true);
					continue;
				}
				memoryStream2.Position = 0L;
				using Stream symbols = _res.ContentFileReadOrNull(resPath.WithExtension("pdb"));
				LoadGameAssembly(memoryStream2, symbols, skipVerify: true);
			}
			catch (Exception exception)
			{
				base.Sawmill.Error($"Exception loading module '{resPath}':\n{exception.ToStringBetter()}");
				return false;
			}
		}
		base.Sawmill.Debug($"DONE loading modules: {stopwatch.Elapsed}");
		return true;
	}

	private (string[] refs, string name)? GetAssemblyReferenceData(Stream stream)
	{
		using PEReader peReader = MakePEReader(stream, leaveOpen: true);
		MetadataReader metaReader = peReader.GetMetadataReader();
		string text = metaReader.GetString(metaReader.GetAssemblyDefinition().Name);
		if (_sandboxingEnabled && TryFindSkipIfSandboxed(metaReader))
		{
			base.Sawmill.Debug("Module {ModuleName} has SkipIfSandboxedAttribute, ignoring.", text);
			return null;
		}
		return ((from a in metaReader.AssemblyReferences
			select metaReader.GetAssemblyReference(a) into a
			select metaReader.GetString(a.Name)).ToArray(), text);
	}

	private static bool TryFindSkipIfSandboxed(MetadataReader reader)
	{
		foreach (CustomAttributeHandle customAttribute2 in reader.CustomAttributes)
		{
			CustomAttribute customAttribute = reader.GetCustomAttribute(customAttribute2);
			if (customAttribute.Parent.Kind != HandleKind.AssemblyDefinition)
			{
				continue;
			}
			EntityHandle constructor = customAttribute.Constructor;
			if (constructor.Kind == HandleKind.MemberReference)
			{
				AssemblyTypeChecker.MTypeReferenced mTypeReferenced = AssemblyTypeChecker.ParseTypeReference(reader, (TypeReferenceHandle)reader.GetMemberReference((MemberReferenceHandle)constructor).Parent);
				if (mTypeReferenced.Namespace == "Robust.Shared.ContentPack" && mTypeReferenced.Name == "SkipIfSandboxedAttribute")
				{
					return true;
				}
			}
		}
		return false;
	}

	public void LoadGameAssembly(Stream assembly, Stream? symbols = null, bool skipVerify = false)
	{
		if (!skipVerify && !MakeTypeChecker().CheckAssembly(assembly))
		{
			throw new TypeCheckFailedException();
		}
		assembly.Position = 0L;
		Assembly assembly2 = ((!_useLoadContext) ? Assembly.Load(assembly.CopyToArray(), symbols?.CopyToArray()) : _loadContext.LoadFromStream(assembly, symbols));
		InitMod(assembly2);
	}

	public void LoadGameAssembly(string diskPath, bool skipVerify = false)
	{
		if (!skipVerify && !MakeTypeChecker().CheckAssembly(diskPath))
		{
			throw new TypeCheckFailedException();
		}
		Assembly assembly = ((!_useLoadContext) ? Assembly.LoadFrom(diskPath) : _loadContext.LoadFromAssemblyPath(diskPath));
		InitMod(assembly);
	}

	public bool TryLoadAssembly(string assemblyName)
	{
		ResPath resPath = new ResPath("/Assemblies/" + assemblyName + ".dll");
		if (_res.TryGetDiskFilePath(resPath, out string diskPath))
		{
			base.Sawmill.Debug("Loading " + assemblyName + " DLL");
			try
			{
				LoadGameAssembly(diskPath);
				return true;
			}
			catch (Exception exception)
			{
				base.Sawmill.Error("Exception loading DLL " + assemblyName + ".dll: " + exception.ToStringBetter());
				return false;
			}
		}
		if (_res.TryContentFileRead(resPath, out Stream fileStream))
		{
			using (fileStream)
			{
				base.Sawmill.Debug("Loading " + assemblyName + " DLL");
				if (_res.TryContentFileRead(new ResPath("/Assemblies/" + assemblyName + ".pdb"), out Stream fileStream2))
				{
					using (fileStream2)
					{
						try
						{
							LoadGameAssembly(fileStream, fileStream2);
							return true;
						}
						catch (Exception exception2)
						{
							base.Sawmill.Error("Exception loading DLL " + assemblyName + ".dll: " + exception2.ToStringBetter());
							return false;
						}
					}
				}
				try
				{
					LoadGameAssembly(fileStream);
					return true;
				}
				catch (Exception exception3)
				{
					base.Sawmill.Error("Exception loading DLL " + assemblyName + ".dll: " + exception3.ToStringBetter());
					return false;
				}
			}
		}
		base.Sawmill.Warning($"Could not load {assemblyName} DLL: {resPath} does not exist in the VFS.");
		return false;
	}

	private Assembly? ResolvingAssembly(AssemblyLoadContext context, AssemblyName name)
	{
		try
		{
			lock (_lock)
			{
				base.Sawmill.Verbose("ResolvingAssembly {0}", name);
				foreach (ModInfo mod in Mods)
				{
					if (mod.GameAssembly.FullName == name.FullName)
					{
						base.Sawmill.Verbose($"Found assembly in modloader ALC: {mod.GameAssembly}");
						return mod.GameAssembly;
					}
				}
				Assembly assembly = TryLoadExtra(name);
				if ((object)assembly != null)
				{
					base.Sawmill.Verbose($"Found assembly through extra loader: {assembly}");
					return assembly;
				}
				if (!_sandboxingEnabled)
				{
					foreach (Assembly sideModule in _sideModules)
					{
						if (sideModule.FullName == name.FullName)
						{
							base.Sawmill.Verbose($"Found assembly in existing side modules: {sideModule}");
							return sideModule;
						}
					}
					try
					{
						Assembly assembly2 = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(name.Name));
						base.Sawmill.Verbose($"Found assembly through default ALC (early): {assembly2}");
						return assembly2;
					}
					catch
					{
					}
					if (_res.TryContentFileRead("/Assemblies/" + name.Name + ".dll", out Stream fileStream))
					{
						Assembly assembly3 = _loadContext.LoadFromStream(fileStream);
						_sideModules.Add(assembly3);
						base.Sawmill.Verbose($"Found assembly in NEW side module: {assembly3}");
						return assembly3;
					}
				}
				base.Sawmill.Verbose("Did not find assembly directly. Should fall back to default ALC.");
				return null;
			}
		}
		catch (Exception ex)
		{
			base.Sawmill.Error("Exception in ResolvingAssembly: {0}", ex);
			ExceptionDispatchInfo.Capture(ex).Throw();
			throw null;
		}
	}

	public void Dispose()
	{
		AssemblyLoadContext.Default.Resolving -= DefaultOnResolving;
	}

	private Assembly? DefaultOnResolving(AssemblyLoadContext ctx, AssemblyName name)
	{
		if (_useLoadContext)
		{
			base.Sawmill.Verbose($"RESOLVING DEFAULT: {name}");
			foreach (Assembly loadedModule in base.LoadedModules)
			{
				if (loadedModule.GetName().Name == name.Name)
				{
					return loadedModule;
				}
			}
			foreach (Assembly sideModule in _sideModules)
			{
				if (sideModule.GetName().Name == name.Name)
				{
					return sideModule;
				}
			}
			Assembly assembly = TryLoadExtra(name);
			if ((object)assembly != null)
			{
				return assembly;
			}
		}
		return null;
	}

	private Assembly? TryLoadExtra(AssemblyName name)
	{
		foreach (ExtraModuleLoad extraModuleLoad in _extraModuleLoads)
		{
			Assembly assembly = extraModuleLoad(name);
			if ((object)assembly != null)
			{
				return assembly;
			}
		}
		return null;
	}

	private AssemblyTypeChecker MakeTypeChecker()
	{
		return new AssemblyTypeChecker(_res, LogManager.GetSawmill("res.typecheck"))
		{
			VerifyIL = _sandboxingEnabled,
			DisableTypeCheck = !_sandboxingEnabled,
			ExtraRobustLoader = VerifierExtraLoadHandler,
			EngineModuleDirectories = _engineModuleDirectories.ToArray()
		};
	}

	internal static PEReader MakePEReader(Stream stream, bool leaveOpen = false, PEStreamOptions options = PEStreamOptions.Default)
	{
		if (!stream.CanSeek)
		{
			stream = (leaveOpen ? stream.CopyToMemoryStream() : stream.ConsumeToMemoryStream());
		}
		if (leaveOpen)
		{
			options |= PEStreamOptions.LeaveOpen;
		}
		return new PEReader(stream, options);
	}
}
