// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.AssemblyTypeChecker
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using ILVerify;
using Internal.TypeSystem.Ecma;
using Pidgin;
using Pidgin.Configuration;
using Robust.Shared.Log;
using Robust.Shared.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

#nullable enable
namespace Robust.Shared.ContentPack;

internal sealed class AssemblyTypeChecker
{
  private const string SystemAssemblyName = "System.Runtime";
  private readonly IResourceManager _res;
  public string[]? EngineModuleDirectories;
  private readonly ISawmill _sawmill;
  private readonly Task<AssemblyTypeChecker.SandboxConfig> _config;
  private static readonly Parser<char, PrimitiveTypeCode> VoidTypeParser = Parser.String("void").ThenReturn<PrimitiveTypeCode>(PrimitiveTypeCode.Void);
  private static readonly Parser<char, PrimitiveTypeCode> BooleanTypeParser = Parser.String("bool").ThenReturn<PrimitiveTypeCode>(PrimitiveTypeCode.Boolean);
  private static readonly Parser<char, PrimitiveTypeCode> CharTypeParser = Parser.String("char").ThenReturn<PrimitiveTypeCode>(PrimitiveTypeCode.Char);
  private static readonly Parser<char, PrimitiveTypeCode> SByteTypeParser = Parser.String("sbyte").ThenReturn<PrimitiveTypeCode>(PrimitiveTypeCode.SByte);
  private static readonly Parser<char, PrimitiveTypeCode> ByteTypeParser = Parser.String("byte").ThenReturn<PrimitiveTypeCode>(PrimitiveTypeCode.Byte);
  private static readonly Parser<char, PrimitiveTypeCode> Int16TypeParser = Parser.String("short").ThenReturn<PrimitiveTypeCode>(PrimitiveTypeCode.Int16);
  private static readonly Parser<char, PrimitiveTypeCode> UInt16TypeParser = Parser.String("ushort").ThenReturn<PrimitiveTypeCode>(PrimitiveTypeCode.UInt16);
  private static readonly Parser<char, PrimitiveTypeCode> Int32TypeParser = Parser.String("int").ThenReturn<PrimitiveTypeCode>(PrimitiveTypeCode.Int32);
  private static readonly Parser<char, PrimitiveTypeCode> UInt32TypeParser = Parser.String("uint").ThenReturn<PrimitiveTypeCode>(PrimitiveTypeCode.UInt32);
  private static readonly Parser<char, PrimitiveTypeCode> Int64TypeParser = Parser.String("long").ThenReturn<PrimitiveTypeCode>(PrimitiveTypeCode.Int64);
  private static readonly Parser<char, PrimitiveTypeCode> UInt64TypeParser = Parser.String("ulong").ThenReturn<PrimitiveTypeCode>(PrimitiveTypeCode.UInt64);
  private static readonly Parser<char, PrimitiveTypeCode> IntPtrTypeParser = Parser.String("nint").ThenReturn<PrimitiveTypeCode>(PrimitiveTypeCode.IntPtr);
  private static readonly Parser<char, PrimitiveTypeCode> UIntPtrTypeParser = Parser.String("nuint").ThenReturn<PrimitiveTypeCode>(PrimitiveTypeCode.UIntPtr);
  private static readonly Parser<char, PrimitiveTypeCode> SingleTypeParser = Parser.String("float").ThenReturn<PrimitiveTypeCode>(PrimitiveTypeCode.Single);
  private static readonly Parser<char, PrimitiveTypeCode> DoubleTypeParser = Parser.String("double").ThenReturn<PrimitiveTypeCode>(PrimitiveTypeCode.Double);
  private static readonly Parser<char, PrimitiveTypeCode> StringTypeParser = Parser.String("string").ThenReturn<PrimitiveTypeCode>(PrimitiveTypeCode.String);
  private static readonly Parser<char, PrimitiveTypeCode> ObjectTypeParser = Parser.String("object").ThenReturn<PrimitiveTypeCode>(PrimitiveTypeCode.Object);
  private static readonly Parser<char, PrimitiveTypeCode> TypedReferenceTypeParser = Parser.String("typedref").ThenReturn<PrimitiveTypeCode>(PrimitiveTypeCode.TypedReference);
  private static readonly Parser<char, AssemblyTypeChecker.MType> PrimitiveTypeParser = Parser.OneOf<char, PrimitiveTypeCode>(new Parser<char, PrimitiveTypeCode>[18]
  {
    Parser.Try<char, PrimitiveTypeCode>(AssemblyTypeChecker.VoidTypeParser),
    Parser.Try<char, PrimitiveTypeCode>(AssemblyTypeChecker.BooleanTypeParser),
    Parser.Try<char, PrimitiveTypeCode>(AssemblyTypeChecker.CharTypeParser),
    Parser.Try<char, PrimitiveTypeCode>(AssemblyTypeChecker.SByteTypeParser),
    Parser.Try<char, PrimitiveTypeCode>(AssemblyTypeChecker.ByteTypeParser),
    Parser.Try<char, PrimitiveTypeCode>(AssemblyTypeChecker.Int16TypeParser),
    Parser.Try<char, PrimitiveTypeCode>(AssemblyTypeChecker.UInt16TypeParser),
    Parser.Try<char, PrimitiveTypeCode>(AssemblyTypeChecker.Int32TypeParser),
    Parser.Try<char, PrimitiveTypeCode>(AssemblyTypeChecker.UInt32TypeParser),
    Parser.Try<char, PrimitiveTypeCode>(AssemblyTypeChecker.Int64TypeParser),
    Parser.Try<char, PrimitiveTypeCode>(AssemblyTypeChecker.UInt64TypeParser),
    Parser.Try<char, PrimitiveTypeCode>(AssemblyTypeChecker.IntPtrTypeParser),
    Parser.Try<char, PrimitiveTypeCode>(AssemblyTypeChecker.UIntPtrTypeParser),
    Parser.Try<char, PrimitiveTypeCode>(AssemblyTypeChecker.SingleTypeParser),
    Parser.Try<char, PrimitiveTypeCode>(AssemblyTypeChecker.DoubleTypeParser),
    Parser.Try<char, PrimitiveTypeCode>(AssemblyTypeChecker.StringTypeParser),
    Parser.Try<char, PrimitiveTypeCode>(AssemblyTypeChecker.ObjectTypeParser),
    AssemblyTypeChecker.TypedReferenceTypeParser
  }).Select<AssemblyTypeChecker.MType>((Func<PrimitiveTypeCode, AssemblyTypeChecker.MType>) (code => (AssemblyTypeChecker.MType) new AssemblyTypeChecker.MTypePrimitive(code))).Labelled("Primitive type");
  private static readonly Parser<char, string> NamespacedIdentifier = Parser.AtLeastOnceString<char>(Parser<char>.Token((Func<char, bool>) (c => char.IsLetterOrDigit(c) || c == '.' || c == '_' || c == '`'))).Labelled("valid identifier");
  private static readonly Parser<char, IEnumerable<AssemblyTypeChecker.MType>> GenericParametersParser = Parser.Rec<char, AssemblyTypeChecker.MType>((Func<Parser<char, AssemblyTypeChecker.MType>>) (() => AssemblyTypeChecker.MaybeArrayTypeParser)).Between<Unit>(Parser.SkipWhitespaces).Separated<char>(Parser.Char(',')).Between<char, char>(Parser.Char('<'), Parser.Char('>'));
  private static readonly Parser<char, AssemblyTypeChecker.MType> GenericMethodPlaceholderParser = Parser.String("!!").Then<string>(Parser.AtLeastOnceString<char>(Parser.Digit)).Select<AssemblyTypeChecker.MType>((Func<string, AssemblyTypeChecker.MType>) (p => (AssemblyTypeChecker.MType) new AssemblyTypeChecker.MTypeGenericMethodPlaceHolder(int.Parse(p, (IFormatProvider) CultureInfo.InvariantCulture))));
  private static readonly Parser<char, AssemblyTypeChecker.MType> GenericTypePlaceholderParser = Parser.String("!").Then<string>(Parser.AtLeastOnceString<char>(Parser.Digit)).Select<AssemblyTypeChecker.MType>((Func<string, AssemblyTypeChecker.MType>) (p => (AssemblyTypeChecker.MType) new AssemblyTypeChecker.MTypeGenericTypePlaceHolder(int.Parse(p, (IFormatProvider) CultureInfo.InvariantCulture))));
  private static readonly Parser<char, AssemblyTypeChecker.MType> GenericPlaceholderParser = Parser.Try<char, AssemblyTypeChecker.MType>(AssemblyTypeChecker.GenericTypePlaceholderParser).Or(Parser.Try<char, AssemblyTypeChecker.MType>(AssemblyTypeChecker.GenericMethodPlaceholderParser)).Labelled("Generic placeholder");
  private static readonly Parser<char, AssemblyTypeChecker.MTypeParsed> TypeNameParser = Parser.Map<char, string, IEnumerable<string>, AssemblyTypeChecker.MTypeParsed>((Func<string, IEnumerable<string>, AssemblyTypeChecker.MTypeParsed>) ((a, b) => b.Aggregate<string, AssemblyTypeChecker.MTypeParsed>(new AssemblyTypeChecker.MTypeParsed(a), (Func<AssemblyTypeChecker.MTypeParsed, string, AssemblyTypeChecker.MTypeParsed>) ((parsed, s) => new AssemblyTypeChecker.MTypeParsed(s, parsed)))), AssemblyTypeChecker.NamespacedIdentifier, Parser.Char('/').Then<string>(AssemblyTypeChecker.NamespacedIdentifier).Many());
  private static readonly Parser<char, AssemblyTypeChecker.MType> ConstructedObjectTypeParser = Parser.Map<char, AssemblyTypeChecker.MTypeParsed, Maybe<IEnumerable<AssemblyTypeChecker.MType>>, AssemblyTypeChecker.MType>((Func<AssemblyTypeChecker.MTypeParsed, Maybe<IEnumerable<AssemblyTypeChecker.MType>>, AssemblyTypeChecker.MType>) ((arg1, arg2) =>
  {
    AssemblyTypeChecker.MType GenericType = (AssemblyTypeChecker.MType) arg1;
    if (arg2.HasValue)
      GenericType = (AssemblyTypeChecker.MType) new AssemblyTypeChecker.MTypeGeneric(GenericType, arg2.Value.ToImmutableArray<AssemblyTypeChecker.MType>());
    return GenericType;
  }), AssemblyTypeChecker.TypeNameParser, AssemblyTypeChecker.GenericParametersParser.Optional());
  private static readonly Parser<char, AssemblyTypeChecker.MType> MaybeArrayTypeParser = Parser.Map<char, AssemblyTypeChecker.MType, IEnumerable<string>, AssemblyTypeChecker.MType>((Func<AssemblyTypeChecker.MType, IEnumerable<string>, AssemblyTypeChecker.MType>) ((a, b) => b.Aggregate<string, AssemblyTypeChecker.MType>(a, (Func<AssemblyTypeChecker.MType, string, AssemblyTypeChecker.MType>) ((type, _) => (AssemblyTypeChecker.MType) new AssemblyTypeChecker.MTypeSZArray(type)))), Parser.Try<char, AssemblyTypeChecker.MType>(AssemblyTypeChecker.GenericPlaceholderParser).Or(Parser.Try<char, AssemblyTypeChecker.MType>(AssemblyTypeChecker.PrimitiveTypeParser)).Or(AssemblyTypeChecker.ConstructedObjectTypeParser), Parser.String("[]").Many());
  private static readonly Parser<char, AssemblyTypeChecker.MType> ByRefTypeParser = Parser.String("ref").Then<Unit>(Parser.SkipWhitespaces).Then<AssemblyTypeChecker.MType>(AssemblyTypeChecker.MaybeArrayTypeParser).Select<AssemblyTypeChecker.MType>((Func<AssemblyTypeChecker.MType, AssemblyTypeChecker.MType>) (t => (AssemblyTypeChecker.MType) new AssemblyTypeChecker.MTypeByRef(t))).Labelled("ByRef type");
  private static readonly Parser<char, AssemblyTypeChecker.MType> TypeParser = Parser.Try<char, AssemblyTypeChecker.MType>(AssemblyTypeChecker.ByRefTypeParser).Or(AssemblyTypeChecker.MaybeArrayTypeParser);
  private static readonly Parser<char, ImmutableArray<AssemblyTypeChecker.MType>> MethodParamsParser = AssemblyTypeChecker.TypeParser.Between<Unit>(Parser.SkipWhitespaces).Separated<char>(Parser.Char(',')).Between<char, char>(Parser.Char('('), Parser.Char(')')).Select<ImmutableArray<AssemblyTypeChecker.MType>>((Func<IEnumerable<AssemblyTypeChecker.MType>, ImmutableArray<AssemblyTypeChecker.MType>>) (p => p.ToImmutableArray<AssemblyTypeChecker.MType>()));
  internal static readonly Parser<char, int> MethodGenericParameterCountParser = Parser.Try<char, int>(Parser.Char(',').Many().Select<int>((Func<IEnumerable<char>, int>) (p => p.Count<char>() + 1)).Between<char, char>(Parser.Char('<'), Parser.Char('>'))).Or(Parser<char>.Return<int>(0));
  internal static readonly Parser<char, AssemblyTypeChecker.WhitelistMethodDefine> MethodParser = Parser.Map<char, AssemblyTypeChecker.MType, string, int, ImmutableArray<AssemblyTypeChecker.MType>, AssemblyTypeChecker.WhitelistMethodDefine>((Func<AssemblyTypeChecker.MType, string, int, ImmutableArray<AssemblyTypeChecker.MType>, AssemblyTypeChecker.WhitelistMethodDefine>) ((a, b, d, c) => new AssemblyTypeChecker.WhitelistMethodDefine(b, a, c, d)), Parser.SkipWhitespaces.Then<AssemblyTypeChecker.MType>(AssemblyTypeChecker.TypeParser), Parser.SkipWhitespaces.Then<string>(AssemblyTypeChecker.NamespacedIdentifier), AssemblyTypeChecker.MethodGenericParameterCountParser, Parser.SkipWhitespaces.Then<ImmutableArray<AssemblyTypeChecker.MType>>(AssemblyTypeChecker.MethodParamsParser));
  internal static readonly Parser<char, AssemblyTypeChecker.WhitelistFieldDefine> FieldParser = Parser.Map<char, AssemblyTypeChecker.MType, string, AssemblyTypeChecker.WhitelistFieldDefine>((Func<AssemblyTypeChecker.MType, string, AssemblyTypeChecker.WhitelistFieldDefine>) ((a, b) => new AssemblyTypeChecker.WhitelistFieldDefine(b, a)), AssemblyTypeChecker.MaybeArrayTypeParser.Between<Unit>(Parser.SkipWhitespaces), AssemblyTypeChecker.NamespacedIdentifier);

  private static AssemblyTypeChecker.SandboxConfig LoadConfig(ISawmill sawmill)
  {
    using (Stream manifestResourceStream = typeof (AssemblyTypeChecker).Assembly.GetManifestResourceStream("Robust.Shared.ContentPack.Sandbox.yml"))
    {
      AssemblyTypeChecker.SandboxConfig sandboxConfig = new Deserializer().Deserialize<AssemblyTypeChecker.SandboxConfig>((TextReader) new StreamReader(manifestResourceStream, Encoding.UTF8));
      foreach (AssemblyTypeChecker.TypeConfig cfg in sandboxConfig.Types.Values.SelectMany<Dictionary<string, AssemblyTypeChecker.TypeConfig>, AssemblyTypeChecker.TypeConfig>((Func<Dictionary<string, AssemblyTypeChecker.TypeConfig>, IEnumerable<AssemblyTypeChecker.TypeConfig>>) (p => (IEnumerable<AssemblyTypeChecker.TypeConfig>) p.Values)))
        AssemblyTypeChecker.ParseTypeConfig(cfg, sawmill);
      return sandboxConfig;
    }
  }

  private static void ParseTypeConfig(AssemblyTypeChecker.TypeConfig cfg, ISawmill sawmill)
  {
    if (cfg.Methods != null)
    {
      List<AssemblyTypeChecker.WhitelistMethodDefine> whitelistMethodDefineList = new List<AssemblyTypeChecker.WhitelistMethodDefine>();
      foreach (string method in cfg.Methods)
      {
        try
        {
          whitelistMethodDefineList.Add(ParserExtensions.ParseOrThrow<AssemblyTypeChecker.WhitelistMethodDefine>(AssemblyTypeChecker.MethodParser, method, (IConfiguration<char>) null));
        }
        catch (ParseException ex)
        {
          sawmill.Error($"Parse exception for '{method}': {ex}");
        }
      }
      cfg.MethodsParsed = whitelistMethodDefineList.ToArray();
    }
    else
      cfg.MethodsParsed = Array.Empty<AssemblyTypeChecker.WhitelistMethodDefine>();
    if (cfg.Fields != null)
    {
      List<AssemblyTypeChecker.WhitelistFieldDefine> whitelistFieldDefineList = new List<AssemblyTypeChecker.WhitelistFieldDefine>();
      foreach (string field in cfg.Fields)
      {
        try
        {
          whitelistFieldDefineList.Add(ParserExtensions.ParseOrThrow<AssemblyTypeChecker.WhitelistFieldDefine>(AssemblyTypeChecker.FieldParser, field, (IConfiguration<char>) null));
        }
        catch (ParseException ex)
        {
          sawmill.Error($"Parse exception for '{field}': {ex}");
        }
      }
      cfg.FieldsParsed = whitelistFieldDefineList.ToArray();
    }
    else
      cfg.FieldsParsed = Array.Empty<AssemblyTypeChecker.WhitelistFieldDefine>();
    if (cfg.NestedTypes == null)
      return;
    foreach (AssemblyTypeChecker.TypeConfig cfg1 in cfg.NestedTypes.Values)
      AssemblyTypeChecker.ParseTypeConfig(cfg1, sawmill);
  }

  public bool DisableTypeCheck { get; init; }

  public AssemblyTypeChecker.DumpFlags Dump { get; init; }

  public bool VerifyIL { get; init; } = true;

  private bool WouldNoOp
  {
    get
    {
      return this.Dump == AssemblyTypeChecker.DumpFlags.None && this.DisableTypeCheck && !this.VerifyIL;
    }
  }

  public Func<string, Stream?>? ExtraRobustLoader { get; init; }

  public AssemblyTypeChecker(IResourceManager res, ISawmill sawmill)
  {
    this._res = res;
    this._sawmill = sawmill;
    this._config = Task.Run<AssemblyTypeChecker.SandboxConfig>((Func<AssemblyTypeChecker.SandboxConfig>) (() => AssemblyTypeChecker.LoadConfig(sawmill)));
  }

  internal AssemblyTypeChecker.Resolver CreateResolver()
  {
    string directoryName = Path.GetDirectoryName(typeof (int).Assembly.Location);
    string location = typeof (AssemblyTypeChecker).Assembly.Location;
    List<string> stringList = new List<string>()
    {
      directoryName
    };
    if (string.IsNullOrEmpty(location))
      this._sawmill.Debug("Robust directory not available");
    else
      stringList.Add(Path.GetDirectoryName(location));
    if (this.EngineModuleDirectories != null)
    {
      foreach (string engineModuleDirectory in this.EngineModuleDirectories)
      {
        this._sawmill.Debug("Adding engine module directory: {ModuleDirectory}", (object) engineModuleDirectory);
        stringList.Add(engineModuleDirectory);
      }
    }
    return new AssemblyTypeChecker.Resolver(this, stringList.ToArray(), new ResPath[1]
    {
      new ResPath("/Assemblies/")
    });
  }

  public bool CheckAssembly(Stream assembly)
  {
    using (AssemblyTypeChecker.Resolver resolver = this.CreateResolver())
      return this.CheckAssembly(assembly, resolver);
  }

  public bool CheckAssembly(Stream assembly, AssemblyTypeChecker.Resolver resolver)
  {
    if (this.WouldNoOp)
      return true;
    this._sawmill.Debug("Checking assembly...");
    Stopwatch stopwatch = Stopwatch.StartNew();
    using (PEReader peReader = ModLoader.MakePEReader(assembly, true, PEStreamOptions.PrefetchEntireImage))
    {
      MetadataReader metadataReader = peReader.GetMetadataReader();
      string asmName = metadataReader.GetString(metadataReader.GetAssemblyDefinition().Name);
      DirectoryEntry? nativeHeaderDirectory = peReader.PEHeaders.CorHeader?.ManagedNativeHeaderDirectory;
      if (nativeHeaderDirectory.HasValue && nativeHeaderDirectory.GetValueOrDefault().Size != 0)
      {
        this._sawmill.Error($"Assembly {asmName} contains native code.");
        return false;
      }
      AssemblyTypeChecker.SandboxConfig result = this._config.Result;
      if (!result.AllowedAssemblyPrefixes.Any<string>((Func<string, bool>) (allowedNamePrefix => asmName.StartsWith(allowedNamePrefix))))
      {
        this._sawmill.Error($"Assembly name '{asmName}' is not allowed for a content assembly");
        return false;
      }
      if (this.VerifyIL && !this.DoVerifyIL(asmName, (IResolver) resolver, peReader, metadataReader))
        return false;
      ConcurrentBag<AssemblyTypeChecker.SandboxError> errors = new ConcurrentBag<AssemblyTypeChecker.SandboxError>();
      List<(TypeReferenceHandle handle, AssemblyTypeChecker.MTypeReferenced parsed)> referencedTypes = this.GetReferencedTypes(metadataReader, errors);
      List<(MemberReferenceHandle handle, AssemblyTypeChecker.MMemberRef parsed)> referencedMembers = this.GetReferencedMembers(metadataReader, errors);
      List<(AssemblyTypeChecker.MType type, AssemblyTypeChecker.MType parent, ArraySegment<AssemblyTypeChecker.MType> interfaceImpls)> externalInheritedTypes = this.GetExternalInheritedTypes(metadataReader, errors);
      this._sawmill.Debug($"References loaded... {stopwatch.ElapsedMilliseconds}ms");
      if ((this.Dump & AssemblyTypeChecker.DumpFlags.Types) != AssemblyTypeChecker.DumpFlags.None)
      {
        foreach ((TypeReferenceHandle handle, AssemblyTypeChecker.MTypeReferenced parsed) tuple in referencedTypes)
          this._sawmill.Debug($"RefType: {tuple.parsed}");
      }
      if ((this.Dump & AssemblyTypeChecker.DumpFlags.Members) != AssemblyTypeChecker.DumpFlags.None)
      {
        foreach ((MemberReferenceHandle handle, AssemblyTypeChecker.MMemberRef parsed) tuple in referencedMembers)
          this._sawmill.Debug($"RefMember: {tuple.parsed}");
      }
      if ((this.Dump & AssemblyTypeChecker.DumpFlags.Inheritance) != AssemblyTypeChecker.DumpFlags.None)
      {
        foreach ((AssemblyTypeChecker.MType type, AssemblyTypeChecker.MType parent, ArraySegment<AssemblyTypeChecker.MType> interfaceImpls) in externalInheritedTypes)
        {
          this._sawmill.Debug($"Inherit: {type} -> {parent}");
          foreach (AssemblyTypeChecker.MType mtype in interfaceImpls)
            this._sawmill.Debug($"  Interface: {mtype}");
        }
      }
      if (this.DisableTypeCheck)
        return true;
      ConcurrentBag<EntityHandle> badReferences = new ConcurrentBag<EntityHandle>();
      foreach ((TypeReferenceHandle handle, AssemblyTypeChecker.MTypeReferenced mtypeReferenced) in referencedTypes)
      {
        if (!this.IsTypeAccessAllowed(result, mtypeReferenced, out AssemblyTypeChecker.TypeConfig _))
        {
          errors.Add(new AssemblyTypeChecker.SandboxError($"Access to type not allowed: {mtypeReferenced}"));
          badReferences.Add((EntityHandle) handle);
        }
      }
      this._sawmill.Debug($"Types... {stopwatch.ElapsedMilliseconds}ms");
      this.CheckInheritance(result, externalInheritedTypes, errors);
      this._sawmill.Debug($"Inheritance... {stopwatch.ElapsedMilliseconds}ms");
      AssemblyTypeChecker.CheckNoUnmanagedMethodDefs(metadataReader, errors);
      this._sawmill.Debug($"Unmanaged methods... {stopwatch.ElapsedMilliseconds}ms");
      AssemblyTypeChecker.CheckNoTypeAbuse(metadataReader, errors);
      this._sawmill.Debug($"Type abuse... {stopwatch.ElapsedMilliseconds}ms");
      this.CheckMemberReferences(result, referencedMembers, errors, badReferences);
      foreach (AssemblyTypeChecker.SandboxError sandboxError in errors)
        this._sawmill.Error("Sandbox violation: " + sandboxError.Message);
      this._sawmill.Debug($"Checked assembly in {stopwatch.ElapsedMilliseconds}ms");
      return errors.IsEmpty;
    }
  }

  private bool DoVerifyIL(
    string name,
    IResolver resolver,
    PEReader peReader,
    MetadataReader reader)
  {
    this._sawmill.Debug(name + ": Verifying IL...");
    Stopwatch stopwatch = Stopwatch.StartNew();
    ConcurrentBag<VerificationResult> bag = new ConcurrentBag<VerificationResult>();
    Parallel.ForEach<IEnumerator<TypeDefinitionHandle>>((IEnumerable<IEnumerator<TypeDefinitionHandle>>) Partitioner.Create<TypeDefinitionHandle>((IEnumerable<TypeDefinitionHandle>) reader.TypeDefinitions).GetPartitions(Environment.ProcessorCount), (Action<IEnumerator<TypeDefinitionHandle>>) (handle =>
    {
      Verifier verifier = new Verifier(resolver);
      verifier.SetSystemModuleName(new AssemblyNameInfo("System.Runtime"));
      while (handle.MoveNext())
      {
        foreach (VerificationResult verificationResult in verifier.Verify(peReader, handle.Current, true))
          bag.Add(verificationResult);
      }
    }));
    AssemblyTypeChecker.SandboxConfig result = this._config.Result;
    bool flag = false;
    foreach (VerificationResult verificationResult in bag)
    {
      if (!result.AllowedVerifierErrors.Contains(verificationResult.Code))
      {
        string str1 = verificationResult.Args == null ? verificationResult.Message : string.Format(verificationResult.Message, verificationResult.Args);
        string message = $"{name}: ILVerify: {str1}";
        try
        {
          if (!verificationResult.Method.IsNil)
          {
            MethodDefinition methodDefinition = reader.GetMethodDefinition(verificationResult.Method);
            string str2 = AssemblyTypeChecker.FormatMethodName(reader, methodDefinition);
            message = $"{message}, method: {str2}";
          }
          if (!verificationResult.Type.IsNil)
          {
            AssemblyTypeChecker.MTypeDefined typeFromDefinition = AssemblyTypeChecker.GetTypeFromDefinition(reader, verificationResult.Type);
            message = $"{message}, type: {typeFromDefinition}";
          }
        }
        catch (AssemblyTypeChecker.UnsupportedMetadataException ex)
        {
          this._sawmill.Error($"{ex}");
        }
        flag = true;
        this._sawmill.Error(message);
      }
    }
    this._sawmill.Debug($"{name}: Verified IL in {stopwatch.Elapsed.TotalMilliseconds}ms");
    return !flag;
  }

  private static string FormatMethodName(MetadataReader reader, MethodDefinition method)
  {
    MethodSignature<AssemblyTypeChecker.MType> methodSignature = method.DecodeSignature<AssemblyTypeChecker.MType, int>((ISignatureTypeProvider<AssemblyTypeChecker.MType, int>) new AssemblyTypeChecker.TypeProvider(), 0);
    AssemblyTypeChecker.MTypeDefined typeFromDefinition = AssemblyTypeChecker.GetTypeFromDefinition(reader, method.GetDeclaringType());
    return $"{methodSignature.ReturnType} {typeFromDefinition}.{reader.GetString(method.Name)}({string.Join<AssemblyTypeChecker.MType>(", ", (IEnumerable<AssemblyTypeChecker.MType>) methodSignature.ParameterTypes)})";
  }

  private static void CheckNoUnmanagedMethodDefs(
    MetadataReader reader,
    ConcurrentBag<AssemblyTypeChecker.SandboxError> errors)
  {
    foreach (MethodDefinitionHandle methodDefinition1 in reader.MethodDefinitions)
    {
      MethodDefinition methodDefinition2 = reader.GetMethodDefinition(methodDefinition1);
      MethodImplAttributes implAttributes = methodDefinition2.ImplAttributes;
      MethodAttributes attributes = methodDefinition2.Attributes;
      bool flag1 = (implAttributes & MethodImplAttributes.ManagedMask) != 0;
      if (!flag1)
      {
        bool flag2;
        switch (implAttributes & MethodImplAttributes.CodeTypeMask)
        {
          case MethodImplAttributes.IL:
          case MethodImplAttributes.CodeTypeMask:
            flag2 = true;
            break;
          default:
            flag2 = false;
            break;
        }
        flag1 = !flag2;
      }
      if (flag1)
      {
        string message = "Method has illegal MethodImplAttributes: " + AssemblyTypeChecker.FormatMethodName(reader, methodDefinition2);
        errors.Add(new AssemblyTypeChecker.SandboxError(message));
      }
      if ((attributes & (MethodAttributes.UnmanagedExport | MethodAttributes.PinvokeImpl)) != MethodAttributes.PrivateScope)
      {
        string message = "Method has illegal MethodAttributes: " + AssemblyTypeChecker.FormatMethodName(reader, methodDefinition2);
        errors.Add(new AssemblyTypeChecker.SandboxError(message));
      }
    }
  }

  private static void CheckNoTypeAbuse(
    MetadataReader reader,
    ConcurrentBag<AssemblyTypeChecker.SandboxError> errors)
  {
    foreach (TypeDefinitionHandle typeDefinition1 in reader.TypeDefinitions)
    {
      TypeDefinition typeDefinition2 = reader.GetTypeDefinition(typeDefinition1);
      if ((typeDefinition2.Attributes & TypeAttributes.ExplicitLayout) != TypeAttributes.AnsiClass)
      {
        AssemblyTypeChecker.MTypeDefined typeFromDefinition = AssemblyTypeChecker.GetTypeFromDefinition(reader, typeDefinition1);
        if (typeDefinition2.GetFields().Count > 0)
        {
          string message = $"Explicit layout type {typeFromDefinition} may not have fields.";
          errors.Add(new AssemblyTypeChecker.SandboxError(message));
        }
      }
    }
  }

  private void CheckMemberReferences(
    AssemblyTypeChecker.SandboxConfig sandboxConfig,
    List<(MemberReferenceHandle handle, AssemblyTypeChecker.MMemberRef parsed)> members,
    ConcurrentBag<AssemblyTypeChecker.SandboxError> errors,
    ConcurrentBag<EntityHandle> badReferences)
  {
    Parallel.ForEach<(MemberReferenceHandle, AssemblyTypeChecker.MMemberRef)>((IEnumerable<(MemberReferenceHandle, AssemblyTypeChecker.MMemberRef)>) members, (Action<(MemberReferenceHandle, AssemblyTypeChecker.MMemberRef)>) (entry =>
    {
      (MemberReferenceHandle handle2, AssemblyTypeChecker.MMemberRef parsed2) = entry;
      AssemblyTypeChecker.MType mtype;
      AssemblyTypeChecker.MTypeGeneric mtypeGeneric;
      for (mtype = parsed2.ParentType; (object) (mtype as AssemblyTypeChecker.MTypeReferenced) == null; mtype = mtypeGeneric.GenericType)
      {
        mtypeGeneric = mtype as AssemblyTypeChecker.MTypeGeneric;
        if ((object) mtypeGeneric == null)
        {
          if ((object) (mtype as AssemblyTypeChecker.MTypeWackyArray) != null || (object) (mtype as AssemblyTypeChecker.MTypeDefined) != null)
            return;
          throw new ArgumentOutOfRangeException();
        }
      }
      AssemblyTypeChecker.MTypeReferenced type = (AssemblyTypeChecker.MTypeReferenced) mtype;
      AssemblyTypeChecker.TypeConfig cfg;
      if (!this.IsTypeAccessAllowed(sandboxConfig, type, out cfg))
      {
        errors.Add(new AssemblyTypeChecker.SandboxError($"Access to type not allowed: {type}"));
      }
      else
      {
        if (cfg.All)
          return;
        switch (parsed2)
        {
          case AssemblyTypeChecker.MMemberRefField mmemberRefField2:
            foreach (AssemblyTypeChecker.WhitelistFieldDefine whitelistFieldDefine in cfg.FieldsParsed)
            {
              if (whitelistFieldDefine.Name == mmemberRefField2.Name && mmemberRefField2.FieldType.WhitelistEquals(whitelistFieldDefine.FieldType))
                return;
            }
            errors.Add(new AssemblyTypeChecker.SandboxError($"Access to field not allowed: {mmemberRefField2}"));
            badReferences.Add((EntityHandle) handle2);
            break;
          case AssemblyTypeChecker.MMemberRefMethod mmemberRefMethod2:
            AssemblyTypeChecker.WhitelistMethodDefine[] methodsParsed = cfg.MethodsParsed;
label_26:
            for (int index1 = 0; index1 < methodsParsed.Length; ++index1)
            {
              AssemblyTypeChecker.WhitelistMethodDefine whitelistMethodDefine = methodsParsed[index1];
              if (whitelistMethodDefine.Name == mmemberRefMethod2.Name && mmemberRefMethod2.ReturnType.WhitelistEquals(whitelistMethodDefine.ReturnType) && mmemberRefMethod2.ParameterTypes.Length == whitelistMethodDefine.ParameterTypes.Length && mmemberRefMethod2.GenericParameterCount == whitelistMethodDefine.GenericParameterCount)
              {
                for (int index2 = 0; index2 < mmemberRefMethod2.ParameterTypes.Length; ++index2)
                {
                  if (!mmemberRefMethod2.ParameterTypes[index2].WhitelistEquals(whitelistMethodDefine.ParameterTypes[index2]))
                    goto label_26;
                }
                return;
              }
            }
            errors.Add(new AssemblyTypeChecker.SandboxError($"Access to method not allowed: {mmemberRefMethod2}"));
            badReferences.Add((EntityHandle) handle2);
            break;
          default:
            throw new ArgumentOutOfRangeException("memberRef");
        }
      }
    }));
  }

  private void CheckInheritance(
    AssemblyTypeChecker.SandboxConfig sandboxConfig,
    List<(AssemblyTypeChecker.MType type, AssemblyTypeChecker.MType parent, ArraySegment<AssemblyTypeChecker.MType> interfaceImpls)> inherited,
    ConcurrentBag<AssemblyTypeChecker.SandboxError> errors)
  {
    foreach ((AssemblyTypeChecker.MType type, AssemblyTypeChecker.MType mtype, ArraySegment<AssemblyTypeChecker.MType> interfaceImpls) in inherited)
    {
      if (!CanInherit(mtype))
        errors.Add(new AssemblyTypeChecker.SandboxError($"Inheriting of type not allowed: {mtype} (by {type})"));
      foreach (AssemblyTypeChecker.MType inheritType in interfaceImpls)
      {
        if (!CanInherit(inheritType))
          errors.Add(new AssemblyTypeChecker.SandboxError($"Implementing of interface not allowed: {inheritType} (by {type})"));
      }
    }

    bool CanInherit(AssemblyTypeChecker.MType inheritType)
    {
      AssemblyTypeChecker.MTypeGeneric mtypeGeneric = inheritType as AssemblyTypeChecker.MTypeGeneric;
      AssemblyTypeChecker.MTypeReferenced mtypeReferenced;
      if ((object) mtypeGeneric == null)
        mtypeReferenced = inheritType as AssemblyTypeChecker.MTypeReferenced ?? throw new InvalidOperationException();
      else
        mtypeReferenced = (AssemblyTypeChecker.MTypeReferenced) mtypeGeneric.GenericType;
      AssemblyTypeChecker.MTypeReferenced type = mtypeReferenced;
      AssemblyTypeChecker.TypeConfig cfg;
      if (!this.IsTypeAccessAllowed(sandboxConfig, type, out cfg) || cfg.Inherit == AssemblyTypeChecker.InheritMode.Block)
        return false;
      return cfg.Inherit == AssemblyTypeChecker.InheritMode.Allow || cfg.All;
    }
  }

  private bool IsTypeAccessAllowed(
    AssemblyTypeChecker.SandboxConfig sandboxConfig,
    AssemblyTypeChecker.MTypeReferenced type,
    [NotNullWhen(true)] out AssemblyTypeChecker.TypeConfig? cfg)
  {
    if (type.Namespace == null)
    {
      AssemblyTypeChecker.MResScopeType resolutionScope = type.ResolutionScope as AssemblyTypeChecker.MResScopeType;
      if ((object) resolutionScope != null)
      {
        AssemblyTypeChecker.TypeConfig cfg1;
        if (!this.IsTypeAccessAllowed(sandboxConfig, (AssemblyTypeChecker.MTypeReferenced) resolutionScope.Type, out cfg1))
        {
          cfg = (AssemblyTypeChecker.TypeConfig) null;
          return false;
        }
        if (cfg1.All)
        {
          cfg = AssemblyTypeChecker.TypeConfig.DefaultAll;
          return true;
        }
        if (cfg1.NestedTypes != null && cfg1.NestedTypes.TryGetValue(type.Name, out cfg))
          return true;
        cfg = (AssemblyTypeChecker.TypeConfig) null;
        return false;
      }
      cfg = (AssemblyTypeChecker.TypeConfig) null;
      return false;
    }
    foreach (string whitelistedNamespace in sandboxConfig.WhitelistedNamespaces)
    {
      if (type.Namespace.StartsWith(whitelistedNamespace))
      {
        cfg = AssemblyTypeChecker.TypeConfig.DefaultAll;
        return true;
      }
    }
    Dictionary<string, AssemblyTypeChecker.TypeConfig> dictionary;
    if (sandboxConfig.Types.TryGetValue(type.Namespace, out dictionary))
      return dictionary.TryGetValue(type.Name, out cfg);
    cfg = (AssemblyTypeChecker.TypeConfig) null;
    return false;
  }

  private List<(TypeReferenceHandle handle, AssemblyTypeChecker.MTypeReferenced parsed)> GetReferencedTypes(
    MetadataReader reader,
    ConcurrentBag<AssemblyTypeChecker.SandboxError> errors)
  {
    return reader.TypeReferences.Select<TypeReferenceHandle, (TypeReferenceHandle, AssemblyTypeChecker.MTypeReferenced)>((Func<TypeReferenceHandle, (TypeReferenceHandle, AssemblyTypeChecker.MTypeReferenced)>) (typeRefHandle =>
    {
      try
      {
        return (typeRefHandle, AssemblyTypeChecker.ParseTypeReference(reader, typeRefHandle));
      }
      catch (AssemblyTypeChecker.UnsupportedMetadataException ex)
      {
        errors.Add(new AssemblyTypeChecker.SandboxError(ex));
        return ();
      }
    })).Where<(TypeReferenceHandle, AssemblyTypeChecker.MTypeReferenced)>((Func<(TypeReferenceHandle, AssemblyTypeChecker.MTypeReferenced), bool>) (p => p.Item2 != (AssemblyTypeChecker.MTypeReferenced) null)).ToList<(TypeReferenceHandle, AssemblyTypeChecker.MTypeReferenced)>();
  }

  private List<(MemberReferenceHandle handle, AssemblyTypeChecker.MMemberRef parsed)> GetReferencedMembers(
    MetadataReader reader,
    ConcurrentBag<AssemblyTypeChecker.SandboxError> errors)
  {
    return reader.MemberReferences.AsParallel<MemberReferenceHandle>().Select<MemberReferenceHandle, (MemberReferenceHandle, AssemblyTypeChecker.MMemberRef)>((Func<MemberReferenceHandle, (MemberReferenceHandle, AssemblyTypeChecker.MMemberRef)>) (memRefHandle =>
    {
      MemberReference memberReference = reader.GetMemberReference(memRefHandle);
      string name = reader.GetString(memberReference.Name);
      AssemblyTypeChecker.MType parentType;
      switch (memberReference.Parent.Kind)
      {
        case HandleKind.TypeReference:
          try
          {
            parentType = (AssemblyTypeChecker.MType) AssemblyTypeChecker.ParseTypeReference(reader, (TypeReferenceHandle) memberReference.Parent);
            break;
          }
          catch (AssemblyTypeChecker.UnsupportedMetadataException ex)
          {
            errors.Add(new AssemblyTypeChecker.SandboxError(ex));
            return ();
          }
        case HandleKind.TypeDefinition:
          try
          {
            parentType = (AssemblyTypeChecker.MType) AssemblyTypeChecker.GetTypeFromDefinition(reader, (TypeDefinitionHandle) memberReference.Parent);
            break;
          }
          catch (AssemblyTypeChecker.UnsupportedMetadataException ex)
          {
            errors.Add(new AssemblyTypeChecker.SandboxError(ex));
            return ();
          }
        case HandleKind.MethodDefinition:
          errors.Add(new AssemblyTypeChecker.SandboxError("Vararg calls are unsupported. Name: " + name));
          return ();
        case HandleKind.ModuleReference:
          errors.Add(new AssemblyTypeChecker.SandboxError("Module global variables and methods are unsupported. Name: " + name));
          return ();
        case HandleKind.TypeSpecification:
          parentType = reader.GetTypeSpecification((TypeSpecificationHandle) memberReference.Parent).DecodeSignature<AssemblyTypeChecker.MType, int>((ISignatureTypeProvider<AssemblyTypeChecker.MType, int>) new AssemblyTypeChecker.TypeProvider(), 0);
          if (parentType.IsCoreTypeDefined())
            return ();
          break;
        default:
          errors.Add(new AssemblyTypeChecker.SandboxError($"Unsupported member ref parent type: {memberReference.Parent.Kind}. Name: {name}"));
          return ();
      }
      AssemblyTypeChecker.MMemberRef mmemberRef;
      switch (memberReference.GetKind())
      {
        case MemberReferenceKind.Method:
          MethodSignature<AssemblyTypeChecker.MType> methodSignature = memberReference.DecodeMethodSignature<AssemblyTypeChecker.MType, int>((ISignatureTypeProvider<AssemblyTypeChecker.MType, int>) new AssemblyTypeChecker.TypeProvider(), 0);
          mmemberRef = (AssemblyTypeChecker.MMemberRef) new AssemblyTypeChecker.MMemberRefMethod(parentType, name, methodSignature.ReturnType, methodSignature.GenericParameterCount, methodSignature.ParameterTypes);
          break;
        case MemberReferenceKind.Field:
          AssemblyTypeChecker.MType fieldType = memberReference.DecodeFieldSignature<AssemblyTypeChecker.MType, int>((ISignatureTypeProvider<AssemblyTypeChecker.MType, int>) new AssemblyTypeChecker.TypeProvider(), 0);
          mmemberRef = (AssemblyTypeChecker.MMemberRef) new AssemblyTypeChecker.MMemberRefField(parentType, name, fieldType);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      return (memRefHandle, mmemberRef);
    })).Where<(MemberReferenceHandle, AssemblyTypeChecker.MMemberRef)>((Func<(MemberReferenceHandle, AssemblyTypeChecker.MMemberRef), bool>) (p => p.memberRef != null)).ToList<(MemberReferenceHandle, AssemblyTypeChecker.MMemberRef)>();
  }

  private List<(AssemblyTypeChecker.MType type, AssemblyTypeChecker.MType parent, ArraySegment<AssemblyTypeChecker.MType> interfaceImpls)> GetExternalInheritedTypes(
    MetadataReader reader,
    ConcurrentBag<AssemblyTypeChecker.SandboxError> errors)
  {
    List<(AssemblyTypeChecker.MType, AssemblyTypeChecker.MType, ArraySegment<AssemblyTypeChecker.MType>)> externalInheritedTypes = new List<(AssemblyTypeChecker.MType, AssemblyTypeChecker.MType, ArraySegment<AssemblyTypeChecker.MType>)>();
    foreach (TypeDefinitionHandle typeDefinition1 in reader.TypeDefinitions)
    {
      TypeDefinition typeDefinition2 = reader.GetTypeDefinition(typeDefinition1);
      AssemblyTypeChecker.MTypeDefined typeFromDefinition = AssemblyTypeChecker.GetTypeFromDefinition(reader, typeDefinition1);
      AssemblyTypeChecker.MType type1;
      if (ParseInheritType((AssemblyTypeChecker.MType) typeFromDefinition, typeDefinition2.BaseType, out type1))
      {
        InterfaceImplementationHandleCollection interfaceImplementations = typeDefinition2.GetInterfaceImplementations();
        ArraySegment<AssemblyTypeChecker.MType> arraySegment1;
        if (interfaceImplementations.Count == 0)
        {
          arraySegment1 = (ArraySegment<AssemblyTypeChecker.MType>) Array.Empty<AssemblyTypeChecker.MType>();
        }
        else
        {
          ArraySegment<AssemblyTypeChecker.MType> arraySegment2 = (ArraySegment<AssemblyTypeChecker.MType>) new AssemblyTypeChecker.MType[interfaceImplementations.Count];
          int count = 0;
          foreach (InterfaceImplementationHandle handle in interfaceImplementations)
          {
            InterfaceImplementation interfaceImplementation = reader.GetInterfaceImplementation(handle);
            AssemblyTypeChecker.MType type2;
            if (ParseInheritType((AssemblyTypeChecker.MType) typeFromDefinition, interfaceImplementation.Interface, out type2))
              arraySegment2[count++] = type2;
          }
          arraySegment1 = arraySegment2.Slice(0, count);
        }
        externalInheritedTypes.Add(((AssemblyTypeChecker.MType) typeFromDefinition, type1, arraySegment1));
      }
    }
    return externalInheritedTypes;

    bool ParseInheritType(
      AssemblyTypeChecker.MType ownerType,
      EntityHandle handle,
      [NotNullWhen(true)] out AssemblyTypeChecker.MType? type)
    {
      type = (AssemblyTypeChecker.MType) null;
      switch (handle.Kind)
      {
        case HandleKind.TypeReference:
          try
          {
            type = (AssemblyTypeChecker.MType) AssemblyTypeChecker.ParseTypeReference(reader, (TypeReferenceHandle) handle);
            return true;
          }
          catch (AssemblyTypeChecker.UnsupportedMetadataException ex)
          {
            errors.Add(new AssemblyTypeChecker.SandboxError(ex));
            return false;
          }
        case HandleKind.TypeDefinition:
          return false;
        case HandleKind.TypeSpecification:
          TypeSpecification typeSpecification = reader.GetTypeSpecification((TypeSpecificationHandle) handle);
          AssemblyTypeChecker.TypeProvider provider = new AssemblyTypeChecker.TypeProvider();
          type = typeSpecification.DecodeSignature<AssemblyTypeChecker.MType, int>((ISignatureTypeProvider<AssemblyTypeChecker.MType, int>) provider, 0);
          if (type.IsCoreTypeDefined())
            return false;
          type = (AssemblyTypeChecker.MType) null;
          return false;
        default:
          errors.Add(new AssemblyTypeChecker.SandboxError($"Unsupported BaseType of kind {handle.Kind} on type {ownerType}"));
          return false;
      }
    }
  }

  internal static AssemblyTypeChecker.MTypeReferenced ParseTypeReference(
    MetadataReader reader,
    TypeReferenceHandle handle)
  {
    TypeReference typeReference = reader.GetTypeReference(handle);
    string Name = reader.GetString(typeReference.Name);
    string Namespace = AssemblyTypeChecker.NilNullString(reader, typeReference.Namespace);
    if (typeReference.ResolutionScope.IsNil)
      throw new AssemblyTypeChecker.UnsupportedMetadataException($"Null resolution scope on type Name: {Namespace}.{Name}. This indicates exported/forwarded types");
    EntityHandle resolutionScope = typeReference.ResolutionScope;
    AssemblyTypeChecker.MResScope ResolutionScope;
    switch (resolutionScope.Kind)
    {
      case HandleKind.TypeReference:
        ResolutionScope = (AssemblyTypeChecker.MResScope) new AssemblyTypeChecker.MResScopeType((AssemblyTypeChecker.MType) AssemblyTypeChecker.ParseTypeReference(reader, (TypeReferenceHandle) typeReference.ResolutionScope));
        break;
      case HandleKind.ModuleReference:
        throw new AssemblyTypeChecker.UnsupportedMetadataException($"Cross-module reference to type {Namespace}.{Name}. ");
      case HandleKind.AssemblyReference:
        AssemblyReference assemblyReference = reader.GetAssemblyReference((AssemblyReferenceHandle) typeReference.ResolutionScope);
        ResolutionScope = (AssemblyTypeChecker.MResScope) new AssemblyTypeChecker.MResScopeAssembly(reader.GetString(assemblyReference.Name));
        break;
      default:
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 3);
        interpolatedStringHandler.AppendLiteral("TypeRef to ");
        ref DefaultInterpolatedStringHandler local = ref interpolatedStringHandler;
        resolutionScope = typeReference.ResolutionScope;
        int kind = (int) resolutionScope.Kind;
        local.AppendFormatted<HandleKind>((HandleKind) kind);
        interpolatedStringHandler.AppendLiteral(" for type ");
        interpolatedStringHandler.AppendFormatted(Namespace);
        interpolatedStringHandler.AppendLiteral(".");
        interpolatedStringHandler.AppendFormatted(Name);
        throw new AssemblyTypeChecker.UnsupportedMetadataException(interpolatedStringHandler.ToStringAndClear());
    }
    return new AssemblyTypeChecker.MTypeReferenced(ResolutionScope, Name, Namespace);
  }

  public bool CheckAssembly(string diskPath)
  {
    if (this.WouldNoOp)
      return true;
    using (FileStream assembly = File.OpenRead(diskPath))
      return this.CheckAssembly((Stream) assembly);
  }

  private static string? NilNullString(MetadataReader reader, StringHandle handle)
  {
    return !handle.IsNil ? reader.GetString(handle) : (string) null;
  }

  private static AssemblyTypeChecker.MTypeDefined GetTypeFromDefinition(
    MetadataReader reader,
    TypeDefinitionHandle handle)
  {
    TypeDefinition typeDefinition = reader.GetTypeDefinition(handle);
    string Name = reader.GetString(typeDefinition.Name);
    string str = AssemblyTypeChecker.NilNullString(reader, typeDefinition.Namespace);
    AssemblyTypeChecker.MTypeDefined mtypeDefined = (AssemblyTypeChecker.MTypeDefined) null;
    if (typeDefinition.IsNested)
      mtypeDefined = AssemblyTypeChecker.GetTypeFromDefinition(reader, typeDefinition.GetDeclaringType());
    string Namespace = str;
    AssemblyTypeChecker.MTypeDefined Enclosing = mtypeDefined;
    return new AssemblyTypeChecker.MTypeDefined(Name, Namespace, Enclosing);
  }

  public static IEnumerable<(string Value, bool IsField)> DumpMetaMembers(Type type)
  {
    using (FileStream fs = File.OpenRead(type.Assembly.Location))
    {
      using (PEReader peReader = new PEReader((Stream) fs))
      {
        MetadataReader metaReader = peReader.GetMetadataReader();
        TypeDefinition typeDef = new TypeDefinition();
        bool flag = false;
        foreach (TypeDefinitionHandle typeDefinition1 in metaReader.TypeDefinitions)
        {
          TypeDefinition typeDefinition2 = metaReader.GetTypeDefinition(typeDefinition1);
          string str1 = metaReader.GetString(typeDefinition2.Name);
          string str2 = AssemblyTypeChecker.NilNullString(metaReader, typeDefinition2.Namespace);
          string name = type.Name;
          if (str1 == name && str2 == type.Namespace)
          {
            typeDef = typeDefinition2;
            flag = true;
            break;
          }
        }
        if (!flag)
          throw new InvalidOperationException("Type didn't exist??");
        AssemblyTypeChecker.TypeProvider provider = new AssemblyTypeChecker.TypeProvider();
        foreach (FieldDefinitionHandle field in typeDef.GetFields())
        {
          FieldDefinition fieldDefinition = metaReader.GetFieldDefinition(field);
          if ((fieldDefinition.Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public)
          {
            string str = metaReader.GetString(fieldDefinition.Name);
            yield return ($"{fieldDefinition.DecodeSignature<AssemblyTypeChecker.MType, int>((ISignatureTypeProvider<AssemblyTypeChecker.MType, int>) provider, 0).WhitelistToString()} {str}", true);
          }
        }
        foreach (MethodDefinitionHandle method in typeDef.GetMethods())
        {
          MethodDefinition methodDefinition = metaReader.GetMethodDefinition(method);
          if (MetadataExtensions.IsPublic(methodDefinition.Attributes))
          {
            string str3 = metaReader.GetString(methodDefinition.Name);
            MethodSignature<AssemblyTypeChecker.MType> methodSignature = methodDefinition.DecodeSignature<AssemblyTypeChecker.MType, int>((ISignatureTypeProvider<AssemblyTypeChecker.MType, int>) provider, 0);
            string str4 = string.Join(", ", methodSignature.ParameterTypes.Select<AssemblyTypeChecker.MType, string>((Func<AssemblyTypeChecker.MType, string>) (t => t.WhitelistToString())));
            int genericParameterCount = methodSignature.GenericParameterCount;
            string str5 = genericParameterCount == 0 ? "" : $"<{new string(',', genericParameterCount - 1)}>";
            yield return ($"{methodSignature.ReturnType.WhitelistToString()} {str3}{str5}({str4})", false);
          }
        }
      }
    }
  }

  private sealed class SandboxConfig
  {
    public string SystemAssemblyName;
    public HashSet<VerifierError> AllowedVerifierErrors;
    public List<string> WhitelistedNamespaces;
    public List<string> AllowedAssemblyPrefixes;
    public Dictionary<string, Dictionary<string, AssemblyTypeChecker.TypeConfig>> Types;
  }

  private sealed class TypeConfig
  {
    public static readonly AssemblyTypeChecker.TypeConfig DefaultAll = new AssemblyTypeChecker.TypeConfig()
    {
      All = true
    };
    public bool All;
    public AssemblyTypeChecker.InheritMode Inherit;
    public string[]? Methods;
    [NonSerialized]
    public AssemblyTypeChecker.WhitelistMethodDefine[] MethodsParsed;
    public string[]? Fields;
    [NonSerialized]
    public AssemblyTypeChecker.WhitelistFieldDefine[] FieldsParsed;
    public Dictionary<string, AssemblyTypeChecker.TypeConfig>? NestedTypes;
  }

  private enum InheritMode : byte
  {
    Default,
    Allow,
    Block,
  }

  private sealed class SandboxError
  {
    public string Message;

    public SandboxError(string message) => this.Message = message;

    public SandboxError(
      AssemblyTypeChecker.UnsupportedMetadataException ume)
      : this("Unsupported metadata: " + ume.Message)
    {
    }
  }

  private sealed class UnsupportedMetadataException : Exception
  {
    public UnsupportedMetadataException()
    {
    }

    public UnsupportedMetadataException(string message)
      : base(message)
    {
    }

    public UnsupportedMetadataException(string message, Exception inner)
      : base(message, inner)
    {
    }
  }

  internal sealed class Resolver : IResolver, IDisposable
  {
    private readonly ConcurrentDictionary<string, PEReader?> _dictionary = new ConcurrentDictionary<string, PEReader>();
    private readonly AssemblyTypeChecker _parent;
    private readonly string[] _diskLoadPaths;
    private readonly ResPath[] _resLoadPaths;

    public Resolver(AssemblyTypeChecker parent, string[] diskLoadPaths, ResPath[] resLoadPaths)
    {
      this._parent = parent;
      this._diskLoadPaths = diskLoadPaths;
      this._resLoadPaths = resLoadPaths;
    }

    private PEReader? ResolveCore(string simpleName)
    {
      string path2 = simpleName + ".dll";
      Func<string, Stream> extraRobustLoader = this._parent.ExtraRobustLoader;
      Stream stream1 = extraRobustLoader != null ? extraRobustLoader(path2) : (Stream) null;
      if (stream1 != null)
        return ModLoader.MakePEReader(stream1);
      foreach (string diskLoadPath in this._diskLoadPaths)
      {
        FileStream stream2;
        if (FileHelper.TryOpenFileRead(Path.Combine(diskLoadPath, path2), out stream2))
          return ModLoader.MakePEReader((Stream) stream2);
      }
      foreach (ResPath resLoadPath in this._resLoadPaths)
      {
        try
        {
          return ModLoader.MakePEReader(this._parent._res.ContentFileRead(resLoadPath / path2));
        }
        catch (FileNotFoundException ex)
        {
        }
      }
      return (PEReader) null;
    }

    public PEReader? ResolveAssembly(AssemblyNameInfo assemblyName)
    {
      return this._dictionary.GetOrAdd(assemblyName.Name, new Func<string, PEReader>(this.ResolveCore));
    }

    public PEReader? ResolveModule(AssemblyNameInfo referencingAssembly, string fileName)
    {
      throw new NotSupportedException();
    }

    public void Dispose()
    {
      foreach (PEReader peReader in (IEnumerable<PEReader>) this._dictionary.Values)
        peReader?.Dispose();
    }
  }

  internal sealed class TypeProvider : 
    ISignatureTypeProvider<AssemblyTypeChecker.MType, int>,
    IConstructedTypeProvider<AssemblyTypeChecker.MType>,
    ISZArrayTypeProvider<AssemblyTypeChecker.MType>,
    ISimpleTypeProvider<AssemblyTypeChecker.MType>
  {
    public AssemblyTypeChecker.MType GetSZArrayType(AssemblyTypeChecker.MType elementType)
    {
      return (AssemblyTypeChecker.MType) new AssemblyTypeChecker.MTypeSZArray(elementType);
    }

    public AssemblyTypeChecker.MType GetArrayType(
      AssemblyTypeChecker.MType elementType,
      ArrayShape shape)
    {
      return (AssemblyTypeChecker.MType) new AssemblyTypeChecker.MTypeWackyArray(elementType, shape);
    }

    public AssemblyTypeChecker.MType GetByReferenceType(AssemblyTypeChecker.MType elementType)
    {
      return (AssemblyTypeChecker.MType) new AssemblyTypeChecker.MTypeByRef(elementType);
    }

    public AssemblyTypeChecker.MType GetGenericInstantiation(
      AssemblyTypeChecker.MType genericType,
      ImmutableArray<AssemblyTypeChecker.MType> typeArguments)
    {
      return (AssemblyTypeChecker.MType) new AssemblyTypeChecker.MTypeGeneric(genericType, typeArguments);
    }

    public AssemblyTypeChecker.MType GetPointerType(AssemblyTypeChecker.MType elementType)
    {
      return (AssemblyTypeChecker.MType) new AssemblyTypeChecker.MTypePointer(elementType);
    }

    public AssemblyTypeChecker.MType GetPrimitiveType(PrimitiveTypeCode typeCode)
    {
      return (AssemblyTypeChecker.MType) new AssemblyTypeChecker.MTypePrimitive(typeCode);
    }

    public AssemblyTypeChecker.MType GetTypeFromDefinition(
      MetadataReader reader,
      TypeDefinitionHandle handle,
      byte rawTypeKind)
    {
      return (AssemblyTypeChecker.MType) AssemblyTypeChecker.GetTypeFromDefinition(reader, handle);
    }

    public AssemblyTypeChecker.MType GetTypeFromReference(
      MetadataReader reader,
      TypeReferenceHandle handle,
      byte rawTypeKind)
    {
      return (AssemblyTypeChecker.MType) AssemblyTypeChecker.ParseTypeReference(reader, handle);
    }

    public AssemblyTypeChecker.MType GetFunctionPointerType(
      MethodSignature<AssemblyTypeChecker.MType> signature)
    {
      throw new NotImplementedException();
    }

    public AssemblyTypeChecker.MType GetGenericMethodParameter(int genericContext, int index)
    {
      return (AssemblyTypeChecker.MType) new AssemblyTypeChecker.MTypeGenericMethodPlaceHolder(index);
    }

    public AssemblyTypeChecker.MType GetGenericTypeParameter(int genericContext, int index)
    {
      return (AssemblyTypeChecker.MType) new AssemblyTypeChecker.MTypeGenericTypePlaceHolder(index);
    }

    public AssemblyTypeChecker.MType GetModifiedType(
      AssemblyTypeChecker.MType modifier,
      AssemblyTypeChecker.MType unmodifiedType,
      bool isRequired)
    {
      return (AssemblyTypeChecker.MType) new AssemblyTypeChecker.MTypeModified(unmodifiedType, modifier, isRequired);
    }

    public AssemblyTypeChecker.MType GetPinnedType(AssemblyTypeChecker.MType elementType)
    {
      throw new NotImplementedException();
    }

    public AssemblyTypeChecker.MType GetTypeFromSpecification(
      MetadataReader reader,
      int genericContext,
      TypeSpecificationHandle handle,
      byte rawTypeKind)
    {
      return reader.GetTypeSpecification(handle).DecodeSignature<AssemblyTypeChecker.MType, int>((ISignatureTypeProvider<AssemblyTypeChecker.MType, int>) this, 0);
    }
  }

  [Flags]
  public enum DumpFlags : byte
  {
    None = 0,
    Types = 1,
    Members = 2,
    Inheritance = 4,
    All = Inheritance | Members | Types, // 0x07
  }

  internal sealed class WhitelistMethodDefine
  {
    public string Name { get; }

    public AssemblyTypeChecker.MType ReturnType { get; }

    public ImmutableArray<AssemblyTypeChecker.MType> ParameterTypes { get; }

    public int GenericParameterCount { get; }

    public WhitelistMethodDefine(
      string name,
      AssemblyTypeChecker.MType returnType,
      ImmutableArray<AssemblyTypeChecker.MType> parameterTypes,
      int genericParameterCount)
    {
      this.Name = name;
      this.ReturnType = returnType;
      this.ParameterTypes = parameterTypes;
      this.GenericParameterCount = genericParameterCount;
    }
  }

  internal sealed class WhitelistFieldDefine
  {
    public string Name { get; }

    public AssemblyTypeChecker.MType FieldType { get; }

    public WhitelistFieldDefine(string name, AssemblyTypeChecker.MType fieldType)
    {
      this.Name = name;
      this.FieldType = fieldType;
    }
  }

  internal abstract record MType()
  {
    public virtual bool WhitelistEquals(AssemblyTypeChecker.MType other) => false;

    public virtual bool IsCoreTypeDefined() => false;

    public virtual string? WhitelistToString() => this.ToString();
  }

  internal abstract class MMemberRef
  {
    public readonly AssemblyTypeChecker.MType ParentType;
    public readonly string Name;

    protected MMemberRef(AssemblyTypeChecker.MType parentType, string name)
    {
      this.ParentType = parentType;
      this.Name = name;
    }
  }

  internal sealed class MMemberRefMethod : AssemblyTypeChecker.MMemberRef
  {
    public readonly AssemblyTypeChecker.MType ReturnType;
    public readonly int GenericParameterCount;
    public readonly ImmutableArray<AssemblyTypeChecker.MType> ParameterTypes;

    public MMemberRefMethod(
      AssemblyTypeChecker.MType parentType,
      string name,
      AssemblyTypeChecker.MType returnType,
      int genericParameterCount,
      ImmutableArray<AssemblyTypeChecker.MType> parameterTypes)
      : base(parentType, name)
    {
      this.ReturnType = returnType;
      this.GenericParameterCount = genericParameterCount;
      this.ParameterTypes = parameterTypes;
    }

    public override string ToString()
    {
      return $"{this.ReturnType} {this.ParentType}.{this.Name}({string.Join<AssemblyTypeChecker.MType>(", ", (IEnumerable<AssemblyTypeChecker.MType>) this.ParameterTypes)})";
    }
  }

  internal sealed class MMemberRefField : AssemblyTypeChecker.MMemberRef
  {
    public readonly AssemblyTypeChecker.MType FieldType;

    public MMemberRefField(
      AssemblyTypeChecker.MType parentType,
      string name,
      AssemblyTypeChecker.MType fieldType)
      : base(parentType, name)
    {
      this.FieldType = fieldType;
    }

    public override string ToString() => $"{this.FieldType} {this.ParentType}.{this.Name}";
  }

  internal sealed record MTypeParsed(string FullName, AssemblyTypeChecker.MTypeParsed? NestedParent = null) : 
    AssemblyTypeChecker.MType
  {
    public override string ToString()
    {
      if (!(this.NestedParent != (AssemblyTypeChecker.MTypeParsed) null))
        return this.FullName;
      return $"{this.NestedParent}/{this.FullName}";
    }

    public override bool WhitelistEquals(AssemblyTypeChecker.MType other)
    {
      AssemblyTypeChecker.MTypeParsed mtypeParsed = other as AssemblyTypeChecker.MTypeParsed;
      if ((object) mtypeParsed == null)
      {
        AssemblyTypeChecker.MTypeReferenced mtypeReferenced = other as AssemblyTypeChecker.MTypeReferenced;
        if ((object) mtypeReferenced == null)
          return false;
        if (this.NestedParent != (AssemblyTypeChecker.MTypeParsed) null)
        {
          AssemblyTypeChecker.MResScopeType resolutionScope = mtypeReferenced.ResolutionScope as AssemblyTypeChecker.MResScopeType;
          if ((object) resolutionScope == null || !this.NestedParent.WhitelistEquals(resolutionScope.Type))
            return false;
        }
        return this.FullName == (mtypeReferenced.Namespace == null ? mtypeReferenced.Name : $"{mtypeReferenced.Namespace}.{mtypeReferenced.Name}");
      }
      return (!(this.NestedParent != (AssemblyTypeChecker.MTypeParsed) null) || !(mtypeParsed.NestedParent == (AssemblyTypeChecker.MTypeParsed) null) && this.NestedParent.WhitelistEquals((AssemblyTypeChecker.MType) mtypeParsed.NestedParent)) && mtypeParsed.FullName == this.FullName;
    }

    [CompilerGenerated]
    public sealed override bool Equals(AssemblyTypeChecker.MType? other)
    {
      return this.Equals((object) other);
    }
  }

  internal sealed record MTypePrimitive(PrimitiveTypeCode TypeCode) : AssemblyTypeChecker.MType
  {
    public override string ToString()
    {
      string str;
      switch (this.TypeCode)
      {
        case PrimitiveTypeCode.Void:
          str = "void";
          break;
        case PrimitiveTypeCode.Boolean:
          str = "bool";
          break;
        case PrimitiveTypeCode.Char:
          str = "char";
          break;
        case PrimitiveTypeCode.SByte:
          str = "int8";
          break;
        case PrimitiveTypeCode.Byte:
          str = "unsigned int8";
          break;
        case PrimitiveTypeCode.Int16:
          str = "int16";
          break;
        case PrimitiveTypeCode.UInt16:
          str = "unsigned int16";
          break;
        case PrimitiveTypeCode.Int32:
          str = "int32";
          break;
        case PrimitiveTypeCode.UInt32:
          str = "unsigned int32";
          break;
        case PrimitiveTypeCode.Int64:
          str = "int64";
          break;
        case PrimitiveTypeCode.UInt64:
          str = "unsigned int64";
          break;
        case PrimitiveTypeCode.Single:
          str = "float32";
          break;
        case PrimitiveTypeCode.Double:
          str = "float64";
          break;
        case PrimitiveTypeCode.String:
          str = "string";
          break;
        case PrimitiveTypeCode.TypedReference:
          str = "typedref";
          break;
        case PrimitiveTypeCode.IntPtr:
          str = "native int";
          break;
        case PrimitiveTypeCode.UIntPtr:
          str = "unsigned native int";
          break;
        case PrimitiveTypeCode.Object:
          str = "object";
          break;
        default:
          str = "???";
          break;
      }
      return str;
    }

    public override string WhitelistToString()
    {
      string str;
      switch (this.TypeCode)
      {
        case PrimitiveTypeCode.Void:
          str = "void";
          break;
        case PrimitiveTypeCode.Boolean:
          str = "bool";
          break;
        case PrimitiveTypeCode.Char:
          str = "char";
          break;
        case PrimitiveTypeCode.SByte:
          str = "sbyte";
          break;
        case PrimitiveTypeCode.Byte:
          str = "byte";
          break;
        case PrimitiveTypeCode.Int16:
          str = "short";
          break;
        case PrimitiveTypeCode.UInt16:
          str = "ushort";
          break;
        case PrimitiveTypeCode.Int32:
          str = "int";
          break;
        case PrimitiveTypeCode.UInt32:
          str = "uint";
          break;
        case PrimitiveTypeCode.Int64:
          str = "long";
          break;
        case PrimitiveTypeCode.UInt64:
          str = "ulong";
          break;
        case PrimitiveTypeCode.Single:
          str = "float";
          break;
        case PrimitiveTypeCode.Double:
          str = "double";
          break;
        case PrimitiveTypeCode.String:
          str = "string";
          break;
        case PrimitiveTypeCode.TypedReference:
          str = "typedref";
          break;
        case PrimitiveTypeCode.IntPtr:
          str = "nint";
          break;
        case PrimitiveTypeCode.UIntPtr:
          str = "unint";
          break;
        case PrimitiveTypeCode.Object:
          str = "object";
          break;
        default:
          str = "???";
          break;
      }
      return str;
    }

    public override bool WhitelistEquals(AssemblyTypeChecker.MType other) => this.Equals(other);

    [CompilerGenerated]
    public sealed override bool Equals(AssemblyTypeChecker.MType? other)
    {
      return this.Equals((object) other);
    }
  }

  internal sealed record MTypeSZArray(AssemblyTypeChecker.MType ElementType) : 
    AssemblyTypeChecker.MType
  {
    public override string ToString() => $"{this.ElementType}[]";

    public override string WhitelistToString() => this.ElementType.WhitelistToString() + "[]";

    public override bool WhitelistEquals(AssemblyTypeChecker.MType other)
    {
      AssemblyTypeChecker.MTypeSZArray mtypeSzArray = other as AssemblyTypeChecker.MTypeSZArray;
      return (object) mtypeSzArray != null && this.ElementType.WhitelistEquals(mtypeSzArray.ElementType);
    }

    [CompilerGenerated]
    public sealed override bool Equals(AssemblyTypeChecker.MType? other)
    {
      return this.Equals((object) other);
    }
  }

  internal sealed record MTypeWackyArray(AssemblyTypeChecker.MType ElementType, ArrayShape Shape) : 
    AssemblyTypeChecker.MType
  {
    public override string ToString() => $"{this.ElementType}[TODO]";

    public override string WhitelistToString() => this.ElementType.WhitelistToString() + "[TODO]";

    public override bool WhitelistEquals(AssemblyTypeChecker.MType other)
    {
      AssemblyTypeChecker.MTypeWackyArray other1 = other as AssemblyTypeChecker.MTypeWackyArray;
      return (object) other1 != null && AssemblyTypeChecker.MTypeWackyArray.ShapesEqual(this.Shape, other1.Shape) && this.ElementType.WhitelistEquals((AssemblyTypeChecker.MType) other1);
    }

    private static bool ShapesEqual(in ArrayShape a, in ArrayShape b)
    {
      return a.Rank == b.Rank && a.LowerBounds.SequenceEqual<int, int>(b.LowerBounds) && a.Sizes.SequenceEqual<int, int>(b.Sizes);
    }

    public override bool IsCoreTypeDefined() => this.ElementType.IsCoreTypeDefined();

    [CompilerGenerated]
    public sealed override bool Equals(AssemblyTypeChecker.MType? other)
    {
      return this.Equals((object) other);
    }
  }

  internal sealed record MTypeByRef(AssemblyTypeChecker.MType ElementType) : 
    AssemblyTypeChecker.MType
  {
    public override string ToString() => $"{this.ElementType}&";

    public override string WhitelistToString() => "ref " + this.ElementType.WhitelistToString();

    public override bool WhitelistEquals(AssemblyTypeChecker.MType other)
    {
      AssemblyTypeChecker.MTypeByRef mtypeByRef = other as AssemblyTypeChecker.MTypeByRef;
      return (object) mtypeByRef != null && this.ElementType.WhitelistEquals(mtypeByRef.ElementType);
    }

    [CompilerGenerated]
    public sealed override bool Equals(AssemblyTypeChecker.MType? other)
    {
      return this.Equals((object) other);
    }
  }

  internal sealed record MTypePointer(AssemblyTypeChecker.MType ElementType) : 
    AssemblyTypeChecker.MType
  {
    public override string ToString() => $"{this.ElementType}*";

    public override string WhitelistToString() => this.ElementType.WhitelistToString() + "*";

    public override bool WhitelistEquals(AssemblyTypeChecker.MType other)
    {
      AssemblyTypeChecker.MTypePointer mtypePointer = other as AssemblyTypeChecker.MTypePointer;
      return (object) mtypePointer != null && this.ElementType.WhitelistEquals(mtypePointer.ElementType);
    }

    [CompilerGenerated]
    public sealed override bool Equals(AssemblyTypeChecker.MType? other)
    {
      return this.Equals((object) other);
    }
  }

  internal sealed record MTypeGeneric(
    AssemblyTypeChecker.MType GenericType,
    ImmutableArray<AssemblyTypeChecker.MType> TypeArguments) : AssemblyTypeChecker.MType
  {
    public override string ToString()
    {
      return $"{this.GenericType}<{string.Join<AssemblyTypeChecker.MType>(", ", (IEnumerable<AssemblyTypeChecker.MType>) this.TypeArguments)}>";
    }

    public override string WhitelistToString()
    {
      return $"{this.GenericType.WhitelistToString()}<{string.Join(", ", this.TypeArguments.Select<AssemblyTypeChecker.MType, string>((Func<AssemblyTypeChecker.MType, string>) (t => t.WhitelistToString())))}>";
    }

    public override bool WhitelistEquals(AssemblyTypeChecker.MType other)
    {
      AssemblyTypeChecker.MTypeGeneric mtypeGeneric = other as AssemblyTypeChecker.MTypeGeneric;
      if ((object) mtypeGeneric == null || this.TypeArguments.Length != mtypeGeneric.TypeArguments.Length)
        return false;
      int index = 0;
      while (true)
      {
        int num = index;
        ImmutableArray<AssemblyTypeChecker.MType> typeArguments = this.TypeArguments;
        int length = typeArguments.Length;
        if (num < length)
        {
          typeArguments = this.TypeArguments;
          AssemblyTypeChecker.MType mtype = typeArguments[index];
          typeArguments = mtypeGeneric.TypeArguments;
          AssemblyTypeChecker.MType other1 = typeArguments[index];
          if (mtype.WhitelistEquals(other1))
            ++index;
          else
            break;
        }
        else
          goto label_7;
      }
      return false;
label_7:
      return this.GenericType.WhitelistEquals(mtypeGeneric.GenericType);
    }

    public bool Equals(AssemblyTypeChecker.MTypeGeneric? otherGeneric)
    {
      return otherGeneric != (AssemblyTypeChecker.MTypeGeneric) null && this.GenericType.Equals(otherGeneric.GenericType) && this.TypeArguments.SequenceEqual<AssemblyTypeChecker.MType, AssemblyTypeChecker.MType>(otherGeneric.TypeArguments);
    }

    public override int GetHashCode()
    {
      HashCode hashCode = new HashCode();
      hashCode.Add<AssemblyTypeChecker.MType>(this.GenericType);
      foreach (AssemblyTypeChecker.MType typeArgument in this.TypeArguments)
        hashCode.Add<AssemblyTypeChecker.MType>(typeArgument);
      return hashCode.ToHashCode();
    }

    public override bool IsCoreTypeDefined() => this.GenericType.IsCoreTypeDefined();

    [CompilerGenerated]
    public sealed override bool Equals(AssemblyTypeChecker.MType? other)
    {
      return this.Equals((object) other);
    }
  }

  internal sealed record MTypeDefined(
    string Name,
    string? Namespace,
    AssemblyTypeChecker.MTypeDefined? Enclosing) : AssemblyTypeChecker.MType
  {
    public override string ToString()
    {
      string str = this.Namespace != null ? $"{this.Namespace}.{this.Name}" : this.Name;
      if (!(this.Enclosing != (AssemblyTypeChecker.MTypeDefined) null))
        return str;
      return $"{this.Enclosing}/{str}";
    }

    public override bool IsCoreTypeDefined() => true;

    [CompilerGenerated]
    public sealed override bool Equals(AssemblyTypeChecker.MType? other)
    {
      return this.Equals((object) other);
    }
  }

  internal sealed record MTypeReferenced(
    AssemblyTypeChecker.MResScope ResolutionScope,
    string Name,
    string? Namespace) : AssemblyTypeChecker.MType
  {
    public override string ToString()
    {
      if (this.Namespace == null)
        return $"{this.ResolutionScope}{this.Name}";
      return $"{this.ResolutionScope}{this.Namespace}.{this.Name}";
    }

    public override string WhitelistToString()
    {
      return this.Namespace == null ? this.Name : $"{this.Namespace}.{this.Name}";
    }

    public override bool WhitelistEquals(AssemblyTypeChecker.MType other)
    {
      AssemblyTypeChecker.MTypeParsed mtypeParsed = other as AssemblyTypeChecker.MTypeParsed;
      bool flag;
      if ((object) mtypeParsed == null)
      {
        AssemblyTypeChecker.MTypeReferenced mtypeReferenced = other as AssemblyTypeChecker.MTypeReferenced;
        flag = (object) mtypeReferenced != null && mtypeReferenced.Namespace == this.Namespace && mtypeReferenced.Name == this.Name && mtypeReferenced.ResolutionScope.Equals(this.ResolutionScope);
      }
      else
        flag = mtypeParsed.WhitelistEquals((AssemblyTypeChecker.MType) this);
      return flag;
    }

    [CompilerGenerated]
    public sealed override bool Equals(AssemblyTypeChecker.MType? other)
    {
      return this.Equals((object) other);
    }
  }

  internal abstract record MResScope();

  internal sealed record MResScopeType(AssemblyTypeChecker.MType Type) : 
    AssemblyTypeChecker.MResScope
  {
    public override string ToString() => $"{this.Type}/";

    [CompilerGenerated]
    public sealed override bool Equals(AssemblyTypeChecker.MResScope? other)
    {
      return this.Equals((object) other);
    }
  }

  internal sealed record MResScopeAssembly(string Name) : AssemblyTypeChecker.MResScope
  {
    public override string ToString() => $"[{this.Name}]";

    [CompilerGenerated]
    public sealed override bool Equals(AssemblyTypeChecker.MResScope? other)
    {
      return this.Equals((object) other);
    }
  }

  internal sealed record MTypeGenericTypePlaceHolder(int Index) : AssemblyTypeChecker.MType
  {
    public override string ToString() => $"!{this.Index}";

    public override bool WhitelistEquals(AssemblyTypeChecker.MType other) => this.Equals(other);

    [CompilerGenerated]
    public sealed override bool Equals(AssemblyTypeChecker.MType? other)
    {
      return this.Equals((object) other);
    }
  }

  internal sealed record MTypeGenericMethodPlaceHolder(int Index) : AssemblyTypeChecker.MType
  {
    public override string ToString() => $"!!{this.Index}";

    public override bool WhitelistEquals(AssemblyTypeChecker.MType other) => this.Equals(other);

    [CompilerGenerated]
    public sealed override bool Equals(AssemblyTypeChecker.MType? other)
    {
      return this.Equals((object) other);
    }
  }

  internal sealed record MTypeModified(
    AssemblyTypeChecker.MType UnmodifiedType,
    AssemblyTypeChecker.MType ModifierType,
    bool Required) : AssemblyTypeChecker.MType
  {
    public override string ToString()
    {
      return $"{this.UnmodifiedType} {(this.Required ? "modreq" : "modopt")}({this.ModifierType})";
    }

    public override string? WhitelistToString() => this.UnmodifiedType.WhitelistToString();

    public override bool WhitelistEquals(AssemblyTypeChecker.MType other)
    {
      return this.UnmodifiedType.WhitelistEquals(other);
    }

    [CompilerGenerated]
    public sealed override bool Equals(AssemblyTypeChecker.MType? other)
    {
      return this.Equals((object) other);
    }
  }
}
