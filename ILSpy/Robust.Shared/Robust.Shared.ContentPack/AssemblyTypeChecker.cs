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
using System.Text;
using System.Threading.Tasks;
using ILVerify;
using Internal.TypeSystem.Ecma;
using Pidgin;
using Pidgin.Configuration;
using Robust.Shared.Log;
using Robust.Shared.Utility;
using YamlDotNet.Serialization;

namespace Robust.Shared.ContentPack;

internal sealed class AssemblyTypeChecker
{
	private sealed class SandboxConfig
	{
		public string SystemAssemblyName;

		public HashSet<VerifierError> AllowedVerifierErrors;

		public List<string> WhitelistedNamespaces;

		public List<string> AllowedAssemblyPrefixes;

		public Dictionary<string, Dictionary<string, TypeConfig>> Types;
	}

	private sealed class TypeConfig
	{
		public static readonly TypeConfig DefaultAll = new TypeConfig
		{
			All = true
		};

		public bool All;

		public InheritMode Inherit;

		public string[]? Methods;

		[NonSerialized]
		public WhitelistMethodDefine[] MethodsParsed;

		public string[]? Fields;

		[NonSerialized]
		public WhitelistFieldDefine[] FieldsParsed;

		public Dictionary<string, TypeConfig>? NestedTypes;
	}

	private enum InheritMode : byte
	{
		Default,
		Allow,
		Block
	}

	private sealed class SandboxError
	{
		public string Message;

		public SandboxError(string message)
		{
			Message = message;
		}

		public SandboxError(UnsupportedMetadataException ume)
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
			_parent = parent;
			_diskLoadPaths = diskLoadPaths;
			_resLoadPaths = resLoadPaths;
		}

		private PEReader? ResolveCore(string simpleName)
		{
			string text = simpleName + ".dll";
			Stream stream = _parent.ExtraRobustLoader?.Invoke(text);
			if (stream != null)
			{
				return ModLoader.MakePEReader(stream);
			}
			string[] diskLoadPaths = _diskLoadPaths;
			for (int i = 0; i < diskLoadPaths.Length; i++)
			{
				if (FileHelper.TryOpenFileRead(Path.Combine(diskLoadPaths[i], text), out FileStream stream2))
				{
					return ModLoader.MakePEReader(stream2);
				}
			}
			ResPath[] resLoadPaths = _resLoadPaths;
			foreach (ResPath resPath in resLoadPaths)
			{
				try
				{
					ResPath path = resPath / text;
					return ModLoader.MakePEReader(_parent._res.ContentFileRead(path));
				}
				catch (FileNotFoundException)
				{
				}
			}
			return null;
		}

		public PEReader? ResolveAssembly(AssemblyNameInfo assemblyName)
		{
			return _dictionary.GetOrAdd(assemblyName.Name, ResolveCore);
		}

		public PEReader? ResolveModule(AssemblyNameInfo referencingAssembly, string fileName)
		{
			throw new NotSupportedException();
		}

		public void Dispose()
		{
			foreach (PEReader value in _dictionary.Values)
			{
				value?.Dispose();
			}
		}
	}

	internal sealed class TypeProvider : ISignatureTypeProvider<MType, int>, IConstructedTypeProvider<MType>, ISZArrayTypeProvider<MType>, ISimpleTypeProvider<MType>
	{
		public MType GetSZArrayType(MType elementType)
		{
			return new MTypeSZArray(elementType);
		}

		public MType GetArrayType(MType elementType, ArrayShape shape)
		{
			return new MTypeWackyArray(elementType, shape);
		}

		public MType GetByReferenceType(MType elementType)
		{
			return new MTypeByRef(elementType);
		}

		public MType GetGenericInstantiation(MType genericType, ImmutableArray<MType> typeArguments)
		{
			return new MTypeGeneric(genericType, typeArguments);
		}

		public MType GetPointerType(MType elementType)
		{
			return new MTypePointer(elementType);
		}

		public MType GetPrimitiveType(PrimitiveTypeCode typeCode)
		{
			return new MTypePrimitive(typeCode);
		}

		public MType GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
		{
			return AssemblyTypeChecker.GetTypeFromDefinition(reader, handle);
		}

		public MType GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
		{
			return ParseTypeReference(reader, handle);
		}

		public MType GetFunctionPointerType(MethodSignature<MType> signature)
		{
			throw new NotImplementedException();
		}

		public MType GetGenericMethodParameter(int genericContext, int index)
		{
			return new MTypeGenericMethodPlaceHolder(index);
		}

		public MType GetGenericTypeParameter(int genericContext, int index)
		{
			return new MTypeGenericTypePlaceHolder(index);
		}

		public MType GetModifiedType(MType modifier, MType unmodifiedType, bool isRequired)
		{
			return new MTypeModified(unmodifiedType, modifier, isRequired);
		}

		public MType GetPinnedType(MType elementType)
		{
			throw new NotImplementedException();
		}

		public MType GetTypeFromSpecification(MetadataReader reader, int genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			return reader.GetTypeSpecification(handle).DecodeSignature(this, 0);
		}
	}

	[Flags]
	public enum DumpFlags : byte
	{
		None = 0,
		Types = 1,
		Members = 2,
		Inheritance = 4,
		All = 7
	}

	internal sealed class WhitelistMethodDefine
	{
		public string Name { get; }

		public MType ReturnType { get; }

		public ImmutableArray<MType> ParameterTypes { get; }

		public int GenericParameterCount { get; }

		public WhitelistMethodDefine(string name, MType returnType, ImmutableArray<MType> parameterTypes, int genericParameterCount)
		{
			Name = name;
			ReturnType = returnType;
			ParameterTypes = parameterTypes;
			GenericParameterCount = genericParameterCount;
		}
	}

	internal sealed class WhitelistFieldDefine
	{
		public string Name { get; }

		public MType FieldType { get; }

		public WhitelistFieldDefine(string name, MType fieldType)
		{
			Name = name;
			FieldType = fieldType;
		}
	}

	internal abstract record MType
	{
		public virtual bool WhitelistEquals(MType other)
		{
			return false;
		}

		public virtual bool IsCoreTypeDefined()
		{
			return false;
		}

		public virtual string? WhitelistToString()
		{
			return ToString();
		}
	}

	internal abstract class MMemberRef
	{
		public readonly MType ParentType;

		public readonly string Name;

		protected MMemberRef(MType parentType, string name)
		{
			ParentType = parentType;
			Name = name;
		}
	}

	internal sealed class MMemberRefMethod : MMemberRef
	{
		public readonly MType ReturnType;

		public readonly int GenericParameterCount;

		public readonly ImmutableArray<MType> ParameterTypes;

		public MMemberRefMethod(MType parentType, string name, MType returnType, int genericParameterCount, ImmutableArray<MType> parameterTypes)
			: base(parentType, name)
		{
			ReturnType = returnType;
			GenericParameterCount = genericParameterCount;
			ParameterTypes = parameterTypes;
		}

		public override string ToString()
		{
			return $"{ReturnType} {ParentType}.{Name}({string.Join(", ", ParameterTypes)})";
		}
	}

	internal sealed class MMemberRefField : MMemberRef
	{
		public readonly MType FieldType;

		public MMemberRefField(MType parentType, string name, MType fieldType)
			: base(parentType, name)
		{
			FieldType = fieldType;
		}

		public override string ToString()
		{
			return $"{FieldType} {ParentType}.{Name}";
		}
	}

	internal sealed record MTypeParsed(string FullName, MTypeParsed? NestedParent = null) : MType
	{
		public override string ToString()
		{
			if (!(NestedParent != null))
			{
				return FullName;
			}
			return $"{NestedParent}/{FullName}";
		}

		public override bool WhitelistEquals(MType other)
		{
			if (!(other is MTypeParsed mTypeParsed))
			{
				if (other is MTypeReferenced mTypeReferenced)
				{
					if (NestedParent != null && (!(mTypeReferenced.ResolutionScope is MResScopeType mResScopeType) || !NestedParent.WhitelistEquals(mResScopeType.Type)))
					{
						return false;
					}
					string text = ((mTypeReferenced.Namespace == null) ? mTypeReferenced.Name : (mTypeReferenced.Namespace + "." + mTypeReferenced.Name));
					return FullName == text;
				}
				return false;
			}
			if (NestedParent != null && (mTypeParsed.NestedParent == null || !NestedParent.WhitelistEquals(mTypeParsed.NestedParent)))
			{
				return false;
			}
			return mTypeParsed.FullName == FullName;
		}
	}

	internal sealed record MTypePrimitive(PrimitiveTypeCode TypeCode) : MType
	{
		public override string ToString()
		{
			return TypeCode switch
			{
				PrimitiveTypeCode.Void => "void", 
				PrimitiveTypeCode.Boolean => "bool", 
				PrimitiveTypeCode.Char => "char", 
				PrimitiveTypeCode.SByte => "int8", 
				PrimitiveTypeCode.Byte => "unsigned int8", 
				PrimitiveTypeCode.Int16 => "int16", 
				PrimitiveTypeCode.UInt16 => "unsigned int16", 
				PrimitiveTypeCode.Int32 => "int32", 
				PrimitiveTypeCode.UInt32 => "unsigned int32", 
				PrimitiveTypeCode.Int64 => "int64", 
				PrimitiveTypeCode.UInt64 => "unsigned int64", 
				PrimitiveTypeCode.Single => "float32", 
				PrimitiveTypeCode.Double => "float64", 
				PrimitiveTypeCode.String => "string", 
				PrimitiveTypeCode.TypedReference => "typedref", 
				PrimitiveTypeCode.IntPtr => "native int", 
				PrimitiveTypeCode.UIntPtr => "unsigned native int", 
				PrimitiveTypeCode.Object => "object", 
				_ => "???", 
			};
		}

		public override string WhitelistToString()
		{
			return TypeCode switch
			{
				PrimitiveTypeCode.Void => "void", 
				PrimitiveTypeCode.Boolean => "bool", 
				PrimitiveTypeCode.Char => "char", 
				PrimitiveTypeCode.SByte => "sbyte", 
				PrimitiveTypeCode.Byte => "byte", 
				PrimitiveTypeCode.Int16 => "short", 
				PrimitiveTypeCode.UInt16 => "ushort", 
				PrimitiveTypeCode.Int32 => "int", 
				PrimitiveTypeCode.UInt32 => "uint", 
				PrimitiveTypeCode.Int64 => "long", 
				PrimitiveTypeCode.UInt64 => "ulong", 
				PrimitiveTypeCode.Single => "float", 
				PrimitiveTypeCode.Double => "double", 
				PrimitiveTypeCode.String => "string", 
				PrimitiveTypeCode.TypedReference => "typedref", 
				PrimitiveTypeCode.IntPtr => "nint", 
				PrimitiveTypeCode.UIntPtr => "unint", 
				PrimitiveTypeCode.Object => "object", 
				_ => "???", 
			};
		}

		public override bool WhitelistEquals(MType other)
		{
			return Equals(other);
		}
	}

	internal sealed record MTypeSZArray(MType ElementType) : MType
	{
		public override string ToString()
		{
			return $"{ElementType}[]";
		}

		public override string WhitelistToString()
		{
			return ElementType.WhitelistToString() + "[]";
		}

		public override bool WhitelistEquals(MType other)
		{
			if (other is MTypeSZArray mTypeSZArray)
			{
				return ElementType.WhitelistEquals(mTypeSZArray.ElementType);
			}
			return false;
		}
	}

	internal sealed record MTypeWackyArray(MType ElementType, ArrayShape Shape) : MType
	{
		public override string ToString()
		{
			return $"{ElementType}[TODO]";
		}

		public override string WhitelistToString()
		{
			return ElementType.WhitelistToString() + "[TODO]";
		}

		public override bool WhitelistEquals(MType other)
		{
			if (other is MTypeWackyArray mTypeWackyArray && ShapesEqual(Shape, mTypeWackyArray.Shape))
			{
				return ElementType.WhitelistEquals(mTypeWackyArray);
			}
			return false;
		}

		private static bool ShapesEqual(in ArrayShape a, in ArrayShape b)
		{
			if (a.Rank == b.Rank && a.LowerBounds.SequenceEqual(b.LowerBounds))
			{
				return a.Sizes.SequenceEqual(b.Sizes);
			}
			return false;
		}

		public override bool IsCoreTypeDefined()
		{
			return ElementType.IsCoreTypeDefined();
		}
	}

	internal sealed record MTypeByRef(MType ElementType) : MType
	{
		public override string ToString()
		{
			return $"{ElementType}&";
		}

		public override string WhitelistToString()
		{
			return "ref " + ElementType.WhitelistToString();
		}

		public override bool WhitelistEquals(MType other)
		{
			if (other is MTypeByRef mTypeByRef)
			{
				return ElementType.WhitelistEquals(mTypeByRef.ElementType);
			}
			return false;
		}
	}

	internal sealed record MTypePointer(MType ElementType) : MType
	{
		public override string ToString()
		{
			return $"{ElementType}*";
		}

		public override string WhitelistToString()
		{
			return ElementType.WhitelistToString() + "*";
		}

		public override bool WhitelistEquals(MType other)
		{
			if (other is MTypePointer mTypePointer)
			{
				return ElementType.WhitelistEquals(mTypePointer.ElementType);
			}
			return false;
		}
	}

	internal sealed record MTypeGeneric(MType GenericType, ImmutableArray<MType> TypeArguments) : MType
	{
		public override string ToString()
		{
			return $"{GenericType}<{string.Join(", ", TypeArguments)}>";
		}

		public override string WhitelistToString()
		{
			return GenericType.WhitelistToString() + "<" + string.Join(", ", TypeArguments.Select((MType t) => t.WhitelistToString())) + ">";
		}

		public override bool WhitelistEquals(MType other)
		{
			if (!(other is MTypeGeneric mTypeGeneric))
			{
				return false;
			}
			if (TypeArguments.Length != mTypeGeneric.TypeArguments.Length)
			{
				return false;
			}
			for (int i = 0; i < TypeArguments.Length; i++)
			{
				MType mType = TypeArguments[i];
				MType other2 = mTypeGeneric.TypeArguments[i];
				if (!mType.WhitelistEquals(other2))
				{
					return false;
				}
			}
			return GenericType.WhitelistEquals(mTypeGeneric.GenericType);
		}

		public bool Equals(MTypeGeneric? otherGeneric)
		{
			if (otherGeneric != null && GenericType.Equals(otherGeneric.GenericType))
			{
				return TypeArguments.SequenceEqual(otherGeneric.TypeArguments);
			}
			return false;
		}

		public override int GetHashCode()
		{
			HashCode hashCode = default(HashCode);
			hashCode.Add(GenericType);
			ImmutableArray<MType>.Enumerator enumerator = TypeArguments.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MType current = enumerator.Current;
				hashCode.Add(current);
			}
			return hashCode.ToHashCode();
		}

		public override bool IsCoreTypeDefined()
		{
			return GenericType.IsCoreTypeDefined();
		}
	}

	internal sealed record MTypeDefined(string Name, string? Namespace, MTypeDefined? Enclosing) : MType
	{
		public override string ToString()
		{
			string text = ((Namespace != null) ? (Namespace + "." + Name) : Name);
			if (Enclosing != null)
			{
				return $"{Enclosing}/{text}";
			}
			return text;
		}

		public override bool IsCoreTypeDefined()
		{
			return true;
		}
	}

	internal sealed record MTypeReferenced(MResScope ResolutionScope, string Name, string? Namespace) : MType
	{
		public override string ToString()
		{
			if (Namespace != null)
			{
				return $"{ResolutionScope}{Namespace}.{Name}";
			}
			return $"{ResolutionScope}{Name}";
		}

		public override string WhitelistToString()
		{
			if (Namespace == null)
			{
				return Name;
			}
			return Namespace + "." + Name;
		}

		public override bool WhitelistEquals(MType other)
		{
			if (!(other is MTypeParsed mTypeParsed))
			{
				if (other is MTypeReferenced mTypeReferenced)
				{
					return mTypeReferenced.Namespace == Namespace && mTypeReferenced.Name == Name && mTypeReferenced.ResolutionScope.Equals(ResolutionScope);
				}
				return false;
			}
			return mTypeParsed.WhitelistEquals(this);
		}
	}

	internal abstract record MResScope;

	internal sealed record MResScopeType(MType Type) : MResScope
	{
		public override string ToString()
		{
			return $"{Type}/";
		}
	}

	internal sealed record MResScopeAssembly(string Name) : MResScope
	{
		public override string ToString()
		{
			return "[" + Name + "]";
		}
	}

	internal sealed record MTypeGenericTypePlaceHolder(int Index) : MType
	{
		public override string ToString()
		{
			return $"!{Index}";
		}

		public override bool WhitelistEquals(MType other)
		{
			return Equals(other);
		}
	}

	internal sealed record MTypeGenericMethodPlaceHolder(int Index) : MType
	{
		public override string ToString()
		{
			return $"!!{Index}";
		}

		public override bool WhitelistEquals(MType other)
		{
			return Equals(other);
		}
	}

	internal sealed record MTypeModified(MType UnmodifiedType, MType ModifierType, bool Required) : MType
	{
		public override string ToString()
		{
			string value = (Required ? "modreq" : "modopt");
			return $"{UnmodifiedType} {value}({ModifierType})";
		}

		public override string? WhitelistToString()
		{
			return UnmodifiedType.WhitelistToString();
		}

		public override bool WhitelistEquals(MType other)
		{
			return UnmodifiedType.WhitelistEquals(other);
		}
	}

	private const string SystemAssemblyName = "System.Runtime";

	private readonly IResourceManager _res;

	public string[]? EngineModuleDirectories;

	private readonly ISawmill _sawmill;

	private readonly Task<SandboxConfig> _config;

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

	private static readonly Parser<char, MType> PrimitiveTypeParser = Parser.OneOf<char, PrimitiveTypeCode>(new Parser<char, PrimitiveTypeCode>[18]
	{
		Parser.Try<char, PrimitiveTypeCode>(VoidTypeParser),
		Parser.Try<char, PrimitiveTypeCode>(BooleanTypeParser),
		Parser.Try<char, PrimitiveTypeCode>(CharTypeParser),
		Parser.Try<char, PrimitiveTypeCode>(SByteTypeParser),
		Parser.Try<char, PrimitiveTypeCode>(ByteTypeParser),
		Parser.Try<char, PrimitiveTypeCode>(Int16TypeParser),
		Parser.Try<char, PrimitiveTypeCode>(UInt16TypeParser),
		Parser.Try<char, PrimitiveTypeCode>(Int32TypeParser),
		Parser.Try<char, PrimitiveTypeCode>(UInt32TypeParser),
		Parser.Try<char, PrimitiveTypeCode>(Int64TypeParser),
		Parser.Try<char, PrimitiveTypeCode>(UInt64TypeParser),
		Parser.Try<char, PrimitiveTypeCode>(IntPtrTypeParser),
		Parser.Try<char, PrimitiveTypeCode>(UIntPtrTypeParser),
		Parser.Try<char, PrimitiveTypeCode>(SingleTypeParser),
		Parser.Try<char, PrimitiveTypeCode>(DoubleTypeParser),
		Parser.Try<char, PrimitiveTypeCode>(StringTypeParser),
		Parser.Try<char, PrimitiveTypeCode>(ObjectTypeParser),
		TypedReferenceTypeParser
	}).Select<MType>((Func<PrimitiveTypeCode, MType>)((PrimitiveTypeCode code) => new MTypePrimitive(code))).Labelled("Primitive type");

	private static readonly Parser<char, string> NamespacedIdentifier = Parser.AtLeastOnceString<char>(Parser<char>.Token((Func<char, bool>)((char c) => char.IsLetterOrDigit(c) || c == '.' || c == '_' || c == '`'))).Labelled("valid identifier");

	private static readonly Parser<char, IEnumerable<MType>> GenericParametersParser = Parser.Rec<char, MType>((Func<Parser<char, MType>>)(() => MaybeArrayTypeParser)).Between<Unit>(Parser.SkipWhitespaces).Separated<char>(Parser.Char(','))
		.Between<char, char>(Parser.Char('<'), Parser.Char('>'));

	private static readonly Parser<char, MType> GenericMethodPlaceholderParser = Parser.String("!!").Then<string>(Parser.AtLeastOnceString<char>(Parser.Digit)).Select<MType>((Func<string, MType>)((string p) => new MTypeGenericMethodPlaceHolder(int.Parse(p, CultureInfo.InvariantCulture))));

	private static readonly Parser<char, MType> GenericTypePlaceholderParser = Parser.String("!").Then<string>(Parser.AtLeastOnceString<char>(Parser.Digit)).Select<MType>((Func<string, MType>)((string p) => new MTypeGenericTypePlaceHolder(int.Parse(p, CultureInfo.InvariantCulture))));

	private static readonly Parser<char, MType> GenericPlaceholderParser = Parser.Try<char, MType>(GenericTypePlaceholderParser).Or(Parser.Try<char, MType>(GenericMethodPlaceholderParser)).Labelled("Generic placeholder");

	private static readonly Parser<char, MTypeParsed> TypeNameParser = Parser.Map<char, string, IEnumerable<string>, MTypeParsed>((Func<string, IEnumerable<string>, MTypeParsed>)((string a, IEnumerable<string> b) => b.Aggregate(new MTypeParsed(a), (MTypeParsed parsed, string s) => new MTypeParsed(s, parsed))), NamespacedIdentifier, Parser.Char('/').Then<string>(NamespacedIdentifier).Many());

	private static readonly Parser<char, MType> ConstructedObjectTypeParser = Parser.Map<char, MTypeParsed, Maybe<IEnumerable<MType>>, MType>((Func<MTypeParsed, Maybe<IEnumerable<MType>>, MType>)delegate(MTypeParsed arg1, Maybe<IEnumerable<MType>> arg2)
	{
		MType mType = arg1;
		if (arg2.HasValue)
		{
			mType = new MTypeGeneric(mType, arg2.Value.ToImmutableArray());
		}
		return mType;
	}, TypeNameParser, GenericParametersParser.Optional());

	private static readonly Parser<char, MType> MaybeArrayTypeParser = Parser.Map<char, MType, IEnumerable<string>, MType>((Func<MType, IEnumerable<string>, MType>)((MType a, IEnumerable<string> b) => b.Aggregate(a, (MType type, string _) => new MTypeSZArray(type))), Parser.Try<char, MType>(GenericPlaceholderParser).Or(Parser.Try<char, MType>(PrimitiveTypeParser)).Or(ConstructedObjectTypeParser), Parser.String("[]").Many());

	private static readonly Parser<char, MType> ByRefTypeParser = Parser.String("ref").Then<Unit>(Parser.SkipWhitespaces).Then<MType>(MaybeArrayTypeParser)
		.Select<MType>((Func<MType, MType>)((MType t) => new MTypeByRef(t)))
		.Labelled("ByRef type");

	private static readonly Parser<char, MType> TypeParser = Parser.Try<char, MType>(ByRefTypeParser).Or(MaybeArrayTypeParser);

	private static readonly Parser<char, ImmutableArray<MType>> MethodParamsParser = TypeParser.Between<Unit>(Parser.SkipWhitespaces).Separated<char>(Parser.Char(',')).Between<char, char>(Parser.Char('('), Parser.Char(')'))
		.Select<ImmutableArray<MType>>((Func<IEnumerable<MType>, ImmutableArray<MType>>)((IEnumerable<MType> p) => p.ToImmutableArray()));

	internal static readonly Parser<char, int> MethodGenericParameterCountParser = Parser.Try<char, int>(Parser.Char(',').Many().Select<int>((Func<IEnumerable<char>, int>)((IEnumerable<char> p) => p.Count() + 1))
		.Between<char, char>(Parser.Char('<'), Parser.Char('>'))).Or(Parser<char>.Return<int>(0));

	internal static readonly Parser<char, WhitelistMethodDefine> MethodParser = Parser.Map<char, MType, string, int, ImmutableArray<MType>, WhitelistMethodDefine>((Func<MType, string, int, ImmutableArray<MType>, WhitelistMethodDefine>)((MType a, string b, int d, ImmutableArray<MType> c) => new WhitelistMethodDefine(b, a, c, d)), Parser.SkipWhitespaces.Then<MType>(TypeParser), Parser.SkipWhitespaces.Then<string>(NamespacedIdentifier), MethodGenericParameterCountParser, Parser.SkipWhitespaces.Then<ImmutableArray<MType>>(MethodParamsParser));

	internal static readonly Parser<char, WhitelistFieldDefine> FieldParser = Parser.Map<char, MType, string, WhitelistFieldDefine>((Func<MType, string, WhitelistFieldDefine>)((MType a, string b) => new WhitelistFieldDefine(b, a)), MaybeArrayTypeParser.Between<Unit>(Parser.SkipWhitespaces), NamespacedIdentifier);

	public bool DisableTypeCheck { get; init; }

	public DumpFlags Dump { get; init; }

	public bool VerifyIL { get; init; } = true;

	private bool WouldNoOp
	{
		get
		{
			if (Dump == DumpFlags.None && DisableTypeCheck)
			{
				return !VerifyIL;
			}
			return false;
		}
	}

	public Func<string, Stream?>? ExtraRobustLoader { get; init; }

	private static SandboxConfig LoadConfig(ISawmill sawmill)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		using Stream stream = typeof(AssemblyTypeChecker).Assembly.GetManifestResourceStream("Robust.Shared.ContentPack.Sandbox.yml");
		SandboxConfig sandboxConfig = new Deserializer().Deserialize<SandboxConfig>((TextReader)new StreamReader(stream, Encoding.UTF8));
		foreach (TypeConfig item in sandboxConfig.Types.Values.SelectMany((Dictionary<string, TypeConfig> p) => p.Values))
		{
			ParseTypeConfig(item, sawmill);
		}
		return sandboxConfig;
	}

	private static void ParseTypeConfig(TypeConfig cfg, ISawmill sawmill)
	{
		//IL_0036: Expected O, but got Unknown
		//IL_00d5: Expected O, but got Unknown
		if (cfg.Methods != null)
		{
			List<WhitelistMethodDefine> list = new List<WhitelistMethodDefine>();
			string[] methods = cfg.Methods;
			foreach (string text in methods)
			{
				try
				{
					list.Add(ParserExtensions.ParseOrThrow<WhitelistMethodDefine>(MethodParser, text, (IConfiguration<char>)null));
				}
				catch (ParseException ex)
				{
					ParseException value = ex;
					sawmill.Error($"Parse exception for '{text}': {value}");
				}
			}
			cfg.MethodsParsed = list.ToArray();
		}
		else
		{
			cfg.MethodsParsed = Array.Empty<WhitelistMethodDefine>();
		}
		if (cfg.Fields != null)
		{
			List<WhitelistFieldDefine> list2 = new List<WhitelistFieldDefine>();
			string[] methods = cfg.Fields;
			foreach (string text2 in methods)
			{
				try
				{
					list2.Add(ParserExtensions.ParseOrThrow<WhitelistFieldDefine>(FieldParser, text2, (IConfiguration<char>)null));
				}
				catch (ParseException ex2)
				{
					ParseException value2 = ex2;
					sawmill.Error($"Parse exception for '{text2}': {value2}");
				}
			}
			cfg.FieldsParsed = list2.ToArray();
		}
		else
		{
			cfg.FieldsParsed = Array.Empty<WhitelistFieldDefine>();
		}
		if (cfg.NestedTypes == null)
		{
			return;
		}
		foreach (TypeConfig value3 in cfg.NestedTypes.Values)
		{
			ParseTypeConfig(value3, sawmill);
		}
	}

	public AssemblyTypeChecker(IResourceManager res, ISawmill sawmill)
	{
		_res = res;
		_sawmill = sawmill;
		_config = Task.Run(() => LoadConfig(sawmill));
	}

	internal Resolver CreateResolver()
	{
		string directoryName = Path.GetDirectoryName(typeof(int).Assembly.Location);
		string location = typeof(AssemblyTypeChecker).Assembly.Location;
		List<string> list = new List<string> { directoryName };
		if (string.IsNullOrEmpty(location))
		{
			_sawmill.Debug("Robust directory not available");
		}
		else
		{
			list.Add(Path.GetDirectoryName(location));
		}
		if (EngineModuleDirectories != null)
		{
			string[] engineModuleDirectories = EngineModuleDirectories;
			foreach (string text in engineModuleDirectories)
			{
				_sawmill.Debug("Adding engine module directory: {ModuleDirectory}", text);
				list.Add(text);
			}
		}
		return new Resolver(this, list.ToArray(), new ResPath[1]
		{
			new ResPath("/Assemblies/")
		});
	}

	public bool CheckAssembly(Stream assembly)
	{
		using Resolver resolver = CreateResolver();
		return CheckAssembly(assembly, resolver);
	}

	public bool CheckAssembly(Stream assembly, Resolver resolver)
	{
		if (WouldNoOp)
		{
			return true;
		}
		_sawmill.Debug("Checking assembly...");
		Stopwatch stopwatch = Stopwatch.StartNew();
		using PEReader pEReader = ModLoader.MakePEReader(assembly, leaveOpen: true, PEStreamOptions.PrefetchEntireImage);
		MetadataReader metadataReader = pEReader.GetMetadataReader();
		string asmName = metadataReader.GetString(metadataReader.GetAssemblyDefinition().Name);
		DirectoryEntry? directoryEntry = pEReader.PEHeaders.CorHeader?.ManagedNativeHeaderDirectory;
		if (directoryEntry.HasValue && directoryEntry.GetValueOrDefault().Size != 0)
		{
			_sawmill.Error("Assembly " + asmName + " contains native code.");
			return false;
		}
		SandboxConfig result = _config.Result;
		if (!result.AllowedAssemblyPrefixes.Any((string allowedNamePrefix) => asmName.StartsWith(allowedNamePrefix)))
		{
			_sawmill.Error("Assembly name '" + asmName + "' is not allowed for a content assembly");
			return false;
		}
		if (VerifyIL && !DoVerifyIL(asmName, (IResolver)(object)resolver, pEReader, metadataReader))
		{
			return false;
		}
		ConcurrentBag<SandboxError> concurrentBag = new ConcurrentBag<SandboxError>();
		List<(TypeReferenceHandle, MTypeReferenced)> referencedTypes = GetReferencedTypes(metadataReader, concurrentBag);
		List<(MemberReferenceHandle, MMemberRef)> referencedMembers = GetReferencedMembers(metadataReader, concurrentBag);
		List<(MType, MType, ArraySegment<MType>)> externalInheritedTypes = GetExternalInheritedTypes(metadataReader, concurrentBag);
		_sawmill.Debug($"References loaded... {stopwatch.ElapsedMilliseconds}ms");
		if ((Dump & DumpFlags.Types) != DumpFlags.None)
		{
			foreach (var item3 in referencedTypes)
			{
				MTypeReferenced item = item3.Item2;
				_sawmill.Debug($"RefType: {item}");
			}
		}
		if ((Dump & DumpFlags.Members) != DumpFlags.None)
		{
			foreach (var item4 in referencedMembers)
			{
				MMemberRef item2 = item4.Item2;
				_sawmill.Debug($"RefMember: {item2}");
			}
		}
		if ((Dump & DumpFlags.Inheritance) != DumpFlags.None)
		{
			foreach (var (value, value2, arraySegment) in externalInheritedTypes)
			{
				_sawmill.Debug($"Inherit: {value} -> {value2}");
				foreach (MType item5 in arraySegment)
				{
					_sawmill.Debug($"  Interface: {item5}");
				}
			}
		}
		if (DisableTypeCheck)
		{
			return true;
		}
		ConcurrentBag<EntityHandle> concurrentBag2 = new ConcurrentBag<EntityHandle>();
		foreach (var (typeReferenceHandle, mTypeReferenced) in referencedTypes)
		{
			if (!IsTypeAccessAllowed(result, mTypeReferenced, out TypeConfig _))
			{
				concurrentBag.Add(new SandboxError($"Access to type not allowed: {mTypeReferenced}"));
				concurrentBag2.Add(typeReferenceHandle);
			}
		}
		_sawmill.Debug($"Types... {stopwatch.ElapsedMilliseconds}ms");
		CheckInheritance(result, externalInheritedTypes, concurrentBag);
		_sawmill.Debug($"Inheritance... {stopwatch.ElapsedMilliseconds}ms");
		CheckNoUnmanagedMethodDefs(metadataReader, concurrentBag);
		_sawmill.Debug($"Unmanaged methods... {stopwatch.ElapsedMilliseconds}ms");
		CheckNoTypeAbuse(metadataReader, concurrentBag);
		_sawmill.Debug($"Type abuse... {stopwatch.ElapsedMilliseconds}ms");
		CheckMemberReferences(result, referencedMembers, concurrentBag, concurrentBag2);
		foreach (SandboxError item6 in concurrentBag)
		{
			_sawmill.Error("Sandbox violation: " + item6.Message);
		}
		_sawmill.Debug($"Checked assembly in {stopwatch.ElapsedMilliseconds}ms");
		return concurrentBag.IsEmpty;
	}

	private bool DoVerifyIL(string name, IResolver resolver, PEReader peReader, MetadataReader reader)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		_sawmill.Debug(name + ": Verifying IL...");
		Stopwatch stopwatch = Stopwatch.StartNew();
		ConcurrentBag<VerificationResult> bag = new ConcurrentBag<VerificationResult>();
		Parallel.ForEach(Partitioner.Create(reader.TypeDefinitions).GetPartitions(Environment.ProcessorCount), delegate(IEnumerator<TypeDefinitionHandle> handle)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			Verifier val = new Verifier(resolver);
			val.SetSystemModuleName(new AssemblyNameInfo("System.Runtime"));
			while (handle.MoveNext())
			{
				foreach (VerificationResult item in val.Verify(peReader, handle.Current, true))
				{
					bag.Add(item);
				}
			}
		});
		SandboxConfig result = _config.Result;
		bool flag = false;
		foreach (VerificationResult item2 in bag)
		{
			if (result.AllowedVerifierErrors.Contains(item2.Code))
			{
				continue;
			}
			string text = ((item2.Args == null) ? item2.Message : string.Format(item2.Message, item2.Args));
			string text2 = name + ": ILVerify: " + text;
			try
			{
				if (!item2.Method.IsNil)
				{
					MethodDefinition methodDefinition = reader.GetMethodDefinition(item2.Method);
					string text3 = FormatMethodName(reader, methodDefinition);
					text2 = text2 + ", method: " + text3;
				}
				if (!item2.Type.IsNil)
				{
					MTypeDefined typeFromDefinition = GetTypeFromDefinition(reader, item2.Type);
					text2 = $"{text2}, type: {typeFromDefinition}";
				}
			}
			catch (UnsupportedMetadataException value)
			{
				_sawmill.Error($"{value}");
			}
			flag = true;
			_sawmill.Error(text2);
		}
		_sawmill.Debug($"{name}: Verified IL in {stopwatch.Elapsed.TotalMilliseconds}ms");
		if (flag)
		{
			return false;
		}
		return true;
	}

	private static string FormatMethodName(MetadataReader reader, MethodDefinition method)
	{
		MethodSignature<MType> methodSignature = method.DecodeSignature(new TypeProvider(), 0);
		MTypeDefined typeFromDefinition = GetTypeFromDefinition(reader, method.GetDeclaringType());
		return $"{methodSignature.ReturnType} {typeFromDefinition}.{reader.GetString(method.Name)}({string.Join(", ", methodSignature.ParameterTypes)})";
	}

	private static void CheckNoUnmanagedMethodDefs(MetadataReader reader, ConcurrentBag<SandboxError> errors)
	{
		foreach (MethodDefinitionHandle methodDefinition2 in reader.MethodDefinitions)
		{
			MethodDefinition methodDefinition = reader.GetMethodDefinition(methodDefinition2);
			MethodImplAttributes implAttributes = methodDefinition.ImplAttributes;
			MethodAttributes attributes = methodDefinition.Attributes;
			bool flag = (implAttributes & MethodImplAttributes.ManagedMask) != 0;
			if (!flag)
			{
				MethodImplAttributes methodImplAttributes = implAttributes & MethodImplAttributes.CodeTypeMask;
				bool flag2 = ((methodImplAttributes == MethodImplAttributes.IL || methodImplAttributes == MethodImplAttributes.CodeTypeMask) ? true : false);
				flag = !flag2;
			}
			if (flag)
			{
				string message = "Method has illegal MethodImplAttributes: " + FormatMethodName(reader, methodDefinition);
				errors.Add(new SandboxError(message));
			}
			if ((attributes & (MethodAttributes.PinvokeImpl | MethodAttributes.UnmanagedExport)) != MethodAttributes.PrivateScope)
			{
				string message2 = "Method has illegal MethodAttributes: " + FormatMethodName(reader, methodDefinition);
				errors.Add(new SandboxError(message2));
			}
		}
	}

	private static void CheckNoTypeAbuse(MetadataReader reader, ConcurrentBag<SandboxError> errors)
	{
		foreach (TypeDefinitionHandle typeDefinition2 in reader.TypeDefinitions)
		{
			TypeDefinition typeDefinition = reader.GetTypeDefinition(typeDefinition2);
			if ((typeDefinition.Attributes & TypeAttributes.ExplicitLayout) != TypeAttributes.NotPublic)
			{
				MTypeDefined typeFromDefinition = GetTypeFromDefinition(reader, typeDefinition2);
				if (typeDefinition.GetFields().Count > 0)
				{
					string message = $"Explicit layout type {typeFromDefinition} may not have fields.";
					errors.Add(new SandboxError(message));
				}
			}
		}
	}

	private void CheckMemberReferences(SandboxConfig sandboxConfig, List<(MemberReferenceHandle handle, MMemberRef parsed)> members, ConcurrentBag<SandboxError> errors, ConcurrentBag<EntityHandle> badReferences)
	{
		Parallel.ForEach<(MemberReferenceHandle, MMemberRef)>(members, delegate((MemberReferenceHandle handle, MMemberRef parsed) entry)
		{
			(MemberReferenceHandle handle, MMemberRef parsed) tuple = entry;
			MemberReferenceHandle item = tuple.handle;
			MMemberRef item2 = tuple.parsed;
			MType mType = item2.ParentType;
			while (!(mType is MTypeReferenced))
			{
				if (!(mType is MTypeGeneric mTypeGeneric))
				{
					if (mType is MTypeWackyArray || mType is MTypeDefined)
					{
						return;
					}
					throw new ArgumentOutOfRangeException();
				}
				mType = mTypeGeneric.GenericType;
			}
			MTypeReferenced mTypeReferenced = (MTypeReferenced)mType;
			if (!IsTypeAccessAllowed(sandboxConfig, mTypeReferenced, out TypeConfig cfg))
			{
				errors.Add(new SandboxError($"Access to type not allowed: {mTypeReferenced}"));
			}
			else if (!cfg.All)
			{
				if (!(item2 is MMemberRefField mMemberRefField))
				{
					if (!(item2 is MMemberRefMethod mMemberRefMethod))
					{
						throw new ArgumentOutOfRangeException("memberRef");
					}
					WhitelistMethodDefine[] methodsParsed = cfg.MethodsParsed;
					foreach (WhitelistMethodDefine whitelistMethodDefine in methodsParsed)
					{
						if (whitelistMethodDefine.Name == mMemberRefMethod.Name && mMemberRefMethod.ReturnType.WhitelistEquals(whitelistMethodDefine.ReturnType) && mMemberRefMethod.ParameterTypes.Length == whitelistMethodDefine.ParameterTypes.Length && mMemberRefMethod.GenericParameterCount == whitelistMethodDefine.GenericParameterCount)
						{
							int num = 0;
							while (true)
							{
								if (num >= mMemberRefMethod.ParameterTypes.Length)
								{
									return;
								}
								MType mType2 = mMemberRefMethod.ParameterTypes[num];
								MType other = whitelistMethodDefine.ParameterTypes[num];
								if (!mType2.WhitelistEquals(other))
								{
									break;
								}
								num++;
							}
						}
					}
					errors.Add(new SandboxError($"Access to method not allowed: {mMemberRefMethod}"));
					badReferences.Add(item);
				}
				else
				{
					WhitelistFieldDefine[] fieldsParsed = cfg.FieldsParsed;
					foreach (WhitelistFieldDefine whitelistFieldDefine in fieldsParsed)
					{
						if (whitelistFieldDefine.Name == mMemberRefField.Name && mMemberRefField.FieldType.WhitelistEquals(whitelistFieldDefine.FieldType))
						{
							return;
						}
					}
					errors.Add(new SandboxError($"Access to field not allowed: {mMemberRefField}"));
					badReferences.Add(item);
				}
			}
		});
	}

	private void CheckInheritance(SandboxConfig sandboxConfig, List<(MType type, MType parent, ArraySegment<MType> interfaceImpls)> inherited, ConcurrentBag<SandboxError> errors)
	{
		foreach (var (value, mType, arraySegment) in inherited)
		{
			if (!CanInherit(mType))
			{
				errors.Add(new SandboxError($"Inheriting of type not allowed: {mType} (by {value})"));
			}
			foreach (MType item in arraySegment)
			{
				if (!CanInherit(item))
				{
					errors.Add(new SandboxError($"Implementing of interface not allowed: {item} (by {value})"));
				}
			}
		}
		bool CanInherit(MType inheritType)
		{
			MTypeReferenced mTypeReferenced2;
			if (!(inheritType is MTypeGeneric mTypeGeneric))
			{
				if (!(inheritType is MTypeReferenced mTypeReferenced))
				{
					throw new InvalidOperationException();
				}
				mTypeReferenced2 = mTypeReferenced;
			}
			else
			{
				mTypeReferenced2 = (MTypeReferenced)mTypeGeneric.GenericType;
			}
			MTypeReferenced type = mTypeReferenced2;
			if (!IsTypeAccessAllowed(sandboxConfig, type, out TypeConfig cfg))
			{
				return false;
			}
			if (cfg.Inherit != InheritMode.Block)
			{
				if (cfg.Inherit != InheritMode.Allow)
				{
					return cfg.All;
				}
				return true;
			}
			return false;
		}
	}

	private bool IsTypeAccessAllowed(SandboxConfig sandboxConfig, MTypeReferenced type, [NotNullWhen(true)] out TypeConfig? cfg)
	{
		if (type.Namespace == null)
		{
			if (type.ResolutionScope is MResScopeType mResScopeType)
			{
				if (!IsTypeAccessAllowed(sandboxConfig, (MTypeReferenced)mResScopeType.Type, out TypeConfig cfg2))
				{
					cfg = null;
					return false;
				}
				if (cfg2.All)
				{
					cfg = TypeConfig.DefaultAll;
					return true;
				}
				if (cfg2.NestedTypes != null && cfg2.NestedTypes.TryGetValue(type.Name, out cfg))
				{
					return true;
				}
				cfg = null;
				return false;
			}
			cfg = null;
			return false;
		}
		foreach (string whitelistedNamespace in sandboxConfig.WhitelistedNamespaces)
		{
			if (type.Namespace.StartsWith(whitelistedNamespace))
			{
				cfg = TypeConfig.DefaultAll;
				return true;
			}
		}
		if (!sandboxConfig.Types.TryGetValue(type.Namespace, out Dictionary<string, TypeConfig> value))
		{
			cfg = null;
			return false;
		}
		return value.TryGetValue(type.Name, out cfg);
	}

	private List<(TypeReferenceHandle handle, MTypeReferenced parsed)> GetReferencedTypes(MetadataReader reader, ConcurrentBag<SandboxError> errors)
	{
		return (from p in reader.TypeReferences.Select(delegate(TypeReferenceHandle typeRefHandle)
			{
				try
				{
					return (typeRefHandle: typeRefHandle, ParseTypeReference(reader, typeRefHandle));
				}
				catch (UnsupportedMetadataException ume)
				{
					errors.Add(new SandboxError(ume));
					return ((TypeReferenceHandle typeRefHandle, MTypeReferenced))default((TypeReferenceHandle, MTypeReferenced));
				}
			})
			where p.Item2 != null
			select p).ToList();
	}

	private List<(MemberReferenceHandle handle, MMemberRef parsed)> GetReferencedMembers(MetadataReader reader, ConcurrentBag<SandboxError> errors)
	{
		return (from p in reader.MemberReferences.AsParallel().Select(delegate(MemberReferenceHandle memRefHandle)
			{
				MemberReference memberReference = reader.GetMemberReference(memRefHandle);
				string text = reader.GetString(memberReference.Name);
				MType mType;
				switch (memberReference.Parent.Kind)
				{
				case HandleKind.TypeReference:
					try
					{
						mType = ParseTypeReference(reader, (TypeReferenceHandle)memberReference.Parent);
					}
					catch (UnsupportedMetadataException ume)
					{
						errors.Add(new SandboxError(ume));
						return ((MemberReferenceHandle memRefHandle, MMemberRef memberRef))default((MemberReferenceHandle, MMemberRef));
					}
					break;
				case HandleKind.TypeDefinition:
					try
					{
						mType = GetTypeFromDefinition(reader, (TypeDefinitionHandle)memberReference.Parent);
					}
					catch (UnsupportedMetadataException ume2)
					{
						errors.Add(new SandboxError(ume2));
						return ((MemberReferenceHandle memRefHandle, MMemberRef memberRef))default((MemberReferenceHandle, MMemberRef));
					}
					break;
				case HandleKind.TypeSpecification:
				{
					TypeSpecification typeSpecification = reader.GetTypeSpecification((TypeSpecificationHandle)memberReference.Parent);
					TypeProvider provider = new TypeProvider();
					mType = typeSpecification.DecodeSignature(provider, 0);
					if (mType.IsCoreTypeDefined())
					{
						return ((MemberReferenceHandle memRefHandle, MMemberRef memberRef))default((MemberReferenceHandle, MMemberRef));
					}
					break;
				}
				case HandleKind.ModuleReference:
					errors.Add(new SandboxError("Module global variables and methods are unsupported. Name: " + text));
					return ((MemberReferenceHandle memRefHandle, MMemberRef memberRef))default((MemberReferenceHandle, MMemberRef));
				case HandleKind.MethodDefinition:
					errors.Add(new SandboxError("Vararg calls are unsupported. Name: " + text));
					return ((MemberReferenceHandle memRefHandle, MMemberRef memberRef))default((MemberReferenceHandle, MMemberRef));
				default:
					errors.Add(new SandboxError($"Unsupported member ref parent type: {memberReference.Parent.Kind}. Name: {text}"));
					return ((MemberReferenceHandle memRefHandle, MMemberRef memberRef))default((MemberReferenceHandle, MMemberRef));
				}
				MMemberRef item;
				switch (memberReference.GetKind())
				{
				case MemberReferenceKind.Method:
				{
					MethodSignature<MType> methodSignature = memberReference.DecodeMethodSignature(new TypeProvider(), 0);
					item = new MMemberRefMethod(mType, text, methodSignature.ReturnType, methodSignature.GenericParameterCount, methodSignature.ParameterTypes);
					break;
				}
				case MemberReferenceKind.Field:
				{
					MType fieldType = memberReference.DecodeFieldSignature(new TypeProvider(), 0);
					item = new MMemberRefField(mType, text, fieldType);
					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
				}
				return (memRefHandle: memRefHandle, memberRef: item);
			})
			where p.memberRef != null
			select p).ToList();
	}

	private List<(MType type, MType parent, ArraySegment<MType> interfaceImpls)> GetExternalInheritedTypes(MetadataReader reader, ConcurrentBag<SandboxError> errors)
	{
		List<(MType, MType, ArraySegment<MType>)> list = new List<(MType, MType, ArraySegment<MType>)>();
		foreach (TypeDefinitionHandle typeDefinition2 in reader.TypeDefinitions)
		{
			TypeDefinition typeDefinition = reader.GetTypeDefinition(typeDefinition2);
			MTypeDefined typeFromDefinition = GetTypeFromDefinition(reader, typeDefinition2);
			if (!ParseInheritType(typeFromDefinition, typeDefinition.BaseType, out var type))
			{
				continue;
			}
			InterfaceImplementationHandleCollection interfaceImplementations = typeDefinition.GetInterfaceImplementations();
			ArraySegment<MType> item;
			if (interfaceImplementations.Count == 0)
			{
				item = Array.Empty<MType>();
			}
			else
			{
				item = new MType[interfaceImplementations.Count];
				int count = 0;
				foreach (InterfaceImplementationHandle item2 in interfaceImplementations)
				{
					if (ParseInheritType(typeFromDefinition, reader.GetInterfaceImplementation(item2).Interface, out var type2))
					{
						item[count++] = type2;
					}
				}
				item = item.Slice(0, count);
			}
			list.Add((typeFromDefinition, type, item));
		}
		return list;
		bool ParseInheritType(MType ownerType, EntityHandle handle, [NotNullWhen(true)] out MType? reference)
		{
			reference = null;
			switch (handle.Kind)
			{
			case HandleKind.TypeDefinition:
				return false;
			case HandleKind.TypeReference:
				try
				{
					reference = ParseTypeReference(reader, (TypeReferenceHandle)handle);
					return true;
				}
				catch (UnsupportedMetadataException ume)
				{
					errors.Add(new SandboxError(ume));
					return false;
				}
			case HandleKind.TypeSpecification:
			{
				TypeSpecification typeSpecification = reader.GetTypeSpecification((TypeSpecificationHandle)handle);
				TypeProvider provider = new TypeProvider();
				reference = typeSpecification.DecodeSignature(provider, 0);
				if (reference.IsCoreTypeDefined())
				{
					return false;
				}
				reference = null;
				return false;
			}
			default:
				errors.Add(new SandboxError($"Unsupported BaseType of kind {handle.Kind} on type {ownerType}"));
				return false;
			}
		}
	}

	internal static MTypeReferenced ParseTypeReference(MetadataReader reader, TypeReferenceHandle handle)
	{
		TypeReference typeReference = reader.GetTypeReference(handle);
		string text = reader.GetString(typeReference.Name);
		string text2 = NilNullString(reader, typeReference.Namespace);
		if (typeReference.ResolutionScope.IsNil)
		{
			throw new UnsupportedMetadataException($"Null resolution scope on type Name: {text2}.{text}. This indicates exported/forwarded types");
		}
		return new MTypeReferenced(typeReference.ResolutionScope.Kind switch
		{
			HandleKind.AssemblyReference => new MResScopeAssembly(reader.GetString(reader.GetAssemblyReference((AssemblyReferenceHandle)typeReference.ResolutionScope).Name)), 
			HandleKind.TypeReference => new MResScopeType(ParseTypeReference(reader, (TypeReferenceHandle)typeReference.ResolutionScope)), 
			HandleKind.ModuleReference => throw new UnsupportedMetadataException($"Cross-module reference to type {text2}.{text}. "), 
			_ => throw new UnsupportedMetadataException($"TypeRef to {typeReference.ResolutionScope.Kind} for type {text2}.{text}"), 
		}, text, text2);
	}

	public bool CheckAssembly(string diskPath)
	{
		if (WouldNoOp)
		{
			return true;
		}
		using FileStream assembly = File.OpenRead(diskPath);
		return CheckAssembly(assembly);
	}

	private static string? NilNullString(MetadataReader reader, StringHandle handle)
	{
		if (!handle.IsNil)
		{
			return reader.GetString(handle);
		}
		return null;
	}

	private static MTypeDefined GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle)
	{
		TypeDefinition typeDefinition = reader.GetTypeDefinition(handle);
		string name = reader.GetString(typeDefinition.Name);
		string text = NilNullString(reader, typeDefinition.Namespace);
		MTypeDefined enclosing = null;
		if (typeDefinition.IsNested)
		{
			enclosing = GetTypeFromDefinition(reader, typeDefinition.GetDeclaringType());
		}
		return new MTypeDefined(name, text, enclosing);
	}

	public static IEnumerable<(string Value, bool IsField)> DumpMetaMembers(Type type)
	{
		string location = type.Assembly.Location;
		using FileStream fs = File.OpenRead(location);
		using PEReader peReader = new PEReader(fs);
		MetadataReader metaReader = peReader.GetMetadataReader();
		TypeDefinition typeDef = default(TypeDefinition);
		bool flag = false;
		foreach (TypeDefinitionHandle typeDefinition2 in metaReader.TypeDefinitions)
		{
			TypeDefinition typeDefinition = metaReader.GetTypeDefinition(typeDefinition2);
			string text = metaReader.GetString(typeDefinition.Name);
			string text2 = NilNullString(metaReader, typeDefinition.Namespace);
			if (text == type.Name && text2 == type.Namespace)
			{
				typeDef = typeDefinition;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			throw new InvalidOperationException("Type didn't exist??");
		}
		TypeProvider provider = new TypeProvider();
		foreach (FieldDefinitionHandle field in typeDef.GetFields())
		{
			FieldDefinition fieldDefinition = metaReader.GetFieldDefinition(field);
			if ((fieldDefinition.Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public)
			{
				string text3 = metaReader.GetString(fieldDefinition.Name);
				MType mType = fieldDefinition.DecodeSignature(provider, 0);
				yield return (Value: mType.WhitelistToString() + " " + text3, IsField: true);
			}
		}
		foreach (MethodDefinitionHandle method in typeDef.GetMethods())
		{
			MethodDefinition methodDefinition = metaReader.GetMethodDefinition(method);
			if (MetadataExtensions.IsPublic(methodDefinition.Attributes))
			{
				string value = metaReader.GetString(methodDefinition.Name);
				MethodSignature<MType> methodSignature = methodDefinition.DecodeSignature(provider, 0);
				string value2 = string.Join(", ", methodSignature.ParameterTypes.Select((MType t) => t.WhitelistToString()));
				int genericParameterCount = methodSignature.GenericParameterCount;
				string value3 = ((genericParameterCount == 0) ? "" : ("<" + new string(',', genericParameterCount - 1) + ">"));
				yield return (Value: $"{methodSignature.ReturnType.WhitelistToString()} {value}{value3}({value2})", IsField: false);
			}
		}
	}
}
