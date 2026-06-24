using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;
using YamlDotNet.Serialization.NamingConventions;

namespace Robust.Shared.Serialization.Manager.Definition;

public abstract class DataDefinition
{
	internal ImmutableArray<FieldDefinition> BaseFieldDefinitions { get; init; }

	internal bool IsRecord { get; init; }

	public abstract bool TryGetDuplicates([NotNullWhen(true)] out string[] duplicates);
}
internal sealed class DataDefinition<T> : DataDefinition where T : notnull
{
	private readonly struct FieldInterfaceInfo((bool Value, bool Sequence, bool Mapping) reader, bool writer, bool copier, bool copyCreator, (bool Value, bool Sequence, bool Mapping) validator)
	{
		public readonly (bool Value, bool Sequence, bool Mapping) Reader = reader;

		public readonly bool Writer = writer;

		public readonly bool Copier = copier;

		public readonly bool CopyCreator = copyCreator;

		public readonly (bool Value, bool Sequence, bool Mapping) Validator = validator;
	}

	public delegate void PopulateDelegateSignature(ref T target, MappingDataNode mappingDataNode, SerializationHookContext hookCtx, ISerializationContext? context);

	public delegate MappingDataNode SerializeDelegateSignature(T obj, ISerializationContext? context, bool alwaysWrite);

	public delegate void CopyDelegateSignature(T source, ref T target, SerializationHookContext hookCtx, ISerializationContext? context);

	private delegate ValidationNode ValidateFieldDelegate(DataNode node, ISerializationContext? context);

	internal readonly PopulateDelegateSignature Populate;

	internal readonly SerializeDelegateSignature Serialize;

	internal readonly CopyDelegateSignature CopyTo;

	private string[] Duplicates { get; }

	private object?[] DefaultValues { get; }

	private ImmutableArray<FieldInterfaceInfo> FieldInterfaceInfos { get; }

	private ImmutableArray<object> FieldAssigners { get; }

	private ImmutableArray<object> FieldAccessors { get; }

	private ImmutableArray<ValidateFieldDelegate> FieldValidators { get; }

	internal DataDefinition(SerializationManager manager, bool isRecord)
	{
		base.IsRecord = isRecord;
		List<FieldDefinition> fieldDefinitions = GetFieldDefinitions(manager, isRecord);
		foreach (FieldDefinition item in fieldDefinitions)
		{
			if (item.Attribute is DataFieldAttribute { Tag: null } dataFieldAttribute)
			{
				dataFieldAttribute.Tag = DataDefinitionUtility.AutoGenerateTag(item.FieldInfo.Name);
			}
		}
		DataFieldAttribute[] dataFields = fieldDefinitions.Select((FieldDefinition f) => f.Attribute).OfType<DataFieldAttribute>().ToArray();
		Duplicates = (from f in dataFields
			where dataFields.Count((DataFieldAttribute df) => df.Tag == f.Tag) > 1
			select f.Tag).Distinct().ToArray();
		List<FieldDefinition> list = fieldDefinitions;
		list.Sort((FieldDefinition a, FieldDefinition b) => b.Attribute.Priority.CompareTo(a.Attribute.Priority));
		base.BaseFieldDefinitions = list.ToImmutableArray();
		DefaultValues = fieldDefinitions.Select((FieldDefinition f) => f.DefaultValue).ToArray();
		object[] array = new object[base.BaseFieldDefinitions.Length];
		object[] array2 = new object[base.BaseFieldDefinitions.Length];
		ValidateFieldDelegate[] array3 = new ValidateFieldDelegate[base.BaseFieldDefinitions.Length];
		FieldInterfaceInfo[] array4 = new FieldInterfaceInfo[base.BaseFieldDefinitions.Length];
		for (int num = 0; num < base.BaseFieldDefinitions.Length; num++)
		{
			FieldDefinition fieldDefinition = base.BaseFieldDefinitions[num];
			array[num] = InternalReflectionUtils.EmitFieldAssigner(typeof(T), fieldDefinition.BackingField);
			array2[num] = InternalReflectionUtils.EmitFieldAccessor(typeof(T), fieldDefinition);
			if (!(fieldDefinition.Attribute.CustomTypeSerializer != null))
			{
				continue;
			}
			(bool, bool, bool) reader = (false, false, false);
			bool flag = false;
			bool flag2 = false;
			bool copyCreator = false;
			(bool, bool, bool) validator = (false, false, false);
			Type[] interfaces = fieldDefinition.Attribute.CustomTypeSerializer.GetInterfaces();
			foreach (Type type in interfaces)
			{
				Type genericTypeDefinition = type.GetGenericTypeDefinition();
				if (genericTypeDefinition == typeof(ITypeWriter<>))
				{
					if (type.GenericTypeArguments[0].IsAssignableTo(fieldDefinition.FieldType))
					{
						flag = true;
					}
				}
				else if (genericTypeDefinition == typeof(ITypeCopier<>))
				{
					if (type.GenericTypeArguments[0].IsAssignableTo(fieldDefinition.FieldType))
					{
						flag2 = true;
					}
				}
				else if (genericTypeDefinition == typeof(ITypeCopyCreator<>))
				{
					if (type.GenericTypeArguments[0].IsAssignableTo(fieldDefinition.FieldType))
					{
						copyCreator = true;
					}
				}
				else if (genericTypeDefinition == typeof(ITypeReader<, >))
				{
					if (type.GenericTypeArguments[0].IsAssignableTo(fieldDefinition.FieldType))
					{
						if (type.GenericTypeArguments[1] == typeof(ValueDataNode))
						{
							reader.Item1 = true;
						}
						else if (type.GenericTypeArguments[1] == typeof(SequenceDataNode))
						{
							reader.Item2 = true;
						}
						else if (type.GenericTypeArguments[1] == typeof(MappingDataNode))
						{
							reader.Item3 = true;
						}
					}
				}
				else if (genericTypeDefinition == typeof(ITypeValidator<, >) && type.GenericTypeArguments[0].IsAssignableTo(fieldDefinition.FieldType))
				{
					if (type.GenericTypeArguments[1] == typeof(ValueDataNode))
					{
						validator.Item1 = true;
					}
					else if (type.GenericTypeArguments[1] == typeof(SequenceDataNode))
					{
						validator.Item2 = true;
					}
					else if (type.GenericTypeArguments[1] == typeof(MappingDataNode))
					{
						validator.Item3 = true;
					}
				}
			}
			if (!reader.Item1 && !reader.Item2 && !reader.Item3 && !flag && !flag2 && !validator.Item1 && !validator.Item2 && !validator.Item3)
			{
				throw new InvalidOperationException($"Could not find any fitting implementation of ITypeReader, ITypeWriter or ITypeCopier for field {fieldDefinition}({fieldDefinition.FieldType}) on type {typeof(T)} on CustomTypeSerializer {fieldDefinition.Attribute.CustomTypeSerializer}");
			}
			array4[num] = new FieldInterfaceInfo(reader, flag, flag2, copyCreator, validator);
		}
		FieldInterfaceInfos = array4.ToImmutableArray();
		FieldAssigners = array.ToImmutableArray();
		FieldAccessors = array2.ToImmutableArray();
		for (int num3 = 0; num3 < base.BaseFieldDefinitions.Length; num3++)
		{
			array3[num3] = EmitFieldValidationDelegate(manager, num3);
		}
		FieldValidators = array3.ToImmutableArray();
		Populate = EmitPopulateDelegate(manager);
		Serialize = EmitSerializeDelegate(manager);
		CopyTo = EmitCopyDelegate(manager);
	}

	private bool TryGetIndex(string tag, out int index)
	{
		for (index = 0; index < base.BaseFieldDefinitions.Length; index++)
		{
			if (base.BaseFieldDefinitions[index].Attribute is DataFieldAttribute dataFieldAttribute && dataFieldAttribute.Tag == tag)
			{
				return true;
			}
		}
		return false;
	}

	private bool TryGetIncludeMappingPair(List<ValidatedMappingNode> includeValidations, string key, out KeyValuePair<ValidationNode, ValidationNode> pair)
	{
		foreach (ValidatedMappingNode includeValidation in includeValidations)
		{
			if (includeValidation.Mapping.TryFirstOrNull<KeyValuePair<ValidationNode, ValidationNode>>((KeyValuePair<ValidationNode, ValidationNode> x) => x.Key is ValidatedValueNode { DataNode: ValueDataNode dataNode } && dataNode.Value == key, out var element))
			{
				pair = element.Value;
				return true;
			}
		}
		pair = default(KeyValuePair<ValidationNode, ValidationNode>);
		return false;
	}

	public ValidationNode Validate(ISerializationManager serialization, MappingDataNode mapping, ISerializationContext? context)
	{
		Dictionary<ValidationNode, ValidationNode> dictionary = new Dictionary<ValidationNode, ValidationNode>();
		List<ValidatedMappingNode> list = new List<ValidatedMappingNode>();
		for (int i = 0; i < base.BaseFieldDefinitions.Length; i++)
		{
			FieldDefinition fieldDefinition = base.BaseFieldDefinitions[i];
			if (!(fieldDefinition.Attribute is IncludeDataFieldAttribute))
			{
				continue;
			}
			ValidationNode validationNode = FieldValidators[i](mapping, context);
			if (validationNode is ErrorNode value)
			{
				dictionary.Add(new InconclusiveNode(new ValueDataNode($"<{"IncludeDataFieldAttribute"}={fieldDefinition.FieldInfo.Name}>")), value);
			}
			else
			{
				if (!(validationNode is ValidatedMappingNode item))
				{
					throw new InvalidValidationNodeReturnedException<ValidatedMappingNode>(validationNode);
				}
				list.Add(item);
			}
		}
		foreach (var (text2, dataNode2) in mapping.Children)
		{
			if (!TryGetIndex(text2, out var index))
			{
				if (TryGetIncludeMappingPair(list, text2, out KeyValuePair<ValidationNode, ValidationNode> pair))
				{
					dictionary.Add(pair.Key, pair.Value);
					continue;
				}
				FieldNotFoundErrorNode key = new FieldNotFoundErrorNode(mapping.GetKeyNode(text2), typeof(T));
				dictionary.Add(key, new InconclusiveNode(dataNode2));
				continue;
			}
			ValidationNode key2 = serialization.ValidateNode<string>(mapping.GetKeyNode(text2), context);
			ValidationNode validationNode2;
			if (SerializationManager.IsNull(dataNode2))
			{
				if (!NullableHelper.IsMarkedAsNullable(base.BaseFieldDefinitions[index].FieldInfo))
				{
					ErrorNode value2 = new ErrorNode(dataNode2, "Field \"" + text2 + "\" had null value despite not being annotated as nullable.");
					dictionary.Add(key2, value2);
					continue;
				}
				validationNode2 = new ValidatedValueNode(dataNode2);
			}
			else
			{
				validationNode2 = FieldValidators[index](dataNode2, context);
			}
			if (!(validationNode2 is ErrorNode) && TryGetIncludeMappingPair(list, text2, out KeyValuePair<ValidationNode, ValidationNode> pair2))
			{
				if (pair2.Value is ErrorNode)
				{
					dictionary.Add(pair2.Key, pair2.Value);
				}
			}
			else
			{
				dictionary.Add(key2, validationNode2);
			}
		}
		return new ValidatedMappingNode(dictionary);
	}

	public override bool TryGetDuplicates([NotNullWhen(true)] out string[] duplicates)
	{
		duplicates = Duplicates;
		return duplicates.Length != 0;
	}

	private bool GatherFieldData(AbstractFieldInfo fieldInfo, out DataFieldBaseAttribute? dataFieldBaseAttribute, [NotNullWhen(true)] out AbstractFieldInfo? backingField, [NotNullWhen(true)] ref InheritanceBehavior? inheritanceBehavior)
	{
		dataFieldBaseAttribute = null;
		backingField = fieldInfo;
		InheritanceBehavior valueOrDefault = inheritanceBehavior.GetValueOrDefault();
		if (!inheritanceBehavior.HasValue)
		{
			valueOrDefault = InheritanceBehavior.Default;
			inheritanceBehavior = valueOrDefault;
		}
		if (fieldInfo.HasAttribute<AlwaysPushInheritanceAttribute>(includeBacking: true))
		{
			inheritanceBehavior = InheritanceBehavior.Always;
		}
		else if (fieldInfo.HasAttribute<NeverPushInheritanceAttribute>(includeBacking: true))
		{
			inheritanceBehavior = InheritanceBehavior.Never;
		}
		if (fieldInfo is SpecificPropertyInfo specificPropertyInfo)
		{
			if (!specificPropertyInfo.IsMostOverridden(typeof(T)))
			{
				return false;
			}
			if (specificPropertyInfo.PropertyInfo.GetMethod == null)
			{
				Logger.ErrorS("serialization", $"Property {specificPropertyInfo} is annotated with DataFieldAttribute but has no getter");
				return false;
			}
		}
		if (fieldInfo.TryGetAttribute<DataFieldAttribute>(out DataFieldAttribute attribute, includeBacking: true))
		{
			return GatherDataFieldData(fieldInfo, out dataFieldBaseAttribute, ref backingField, attribute);
		}
		if (fieldInfo.TryGetAttribute<IncludeDataFieldAttribute>(out IncludeDataFieldAttribute attribute2, includeBacking: true))
		{
			dataFieldBaseAttribute = attribute2;
			return true;
		}
		if (!(fieldInfo is SpecificPropertyInfo))
		{
			return true;
		}
		SpecificFieldInfo backingField2 = fieldInfo.GetBackingField();
		if (backingField2 == null)
		{
			return false;
		}
		return GatherFieldData(backingField2, out dataFieldBaseAttribute, out backingField, ref inheritanceBehavior);
	}

	private static bool GatherDataFieldData(AbstractFieldInfo fieldInfo, out DataFieldBaseAttribute dataFieldBaseAttribute, ref AbstractFieldInfo backingField, DataFieldAttribute dataFieldAttribute)
	{
		dataFieldBaseAttribute = dataFieldAttribute;
		if (!(fieldInfo is SpecificPropertyInfo specificPropertyInfo) || dataFieldAttribute.ReadOnly || specificPropertyInfo.PropertyInfo.SetMethod != null)
		{
			return true;
		}
		if (!specificPropertyInfo.TryGetBackingField(out SpecificFieldInfo field))
		{
			Logger.ErrorS("serialization", $"Property {specificPropertyInfo} in type {specificPropertyInfo.DeclaringType} is annotated with DataFieldAttribute as non-readonly but has no auto-setter");
			return false;
		}
		backingField = field;
		return true;
	}

	private List<FieldDefinition> GetFieldDefinitions(SerializationManager manager, bool isRecord)
	{
		T val = manager.GetOrCreateInstantiator<T>(isRecord)();
		List<FieldDefinition> list = new List<FieldDefinition>();
		foreach (AbstractFieldInfo allPropertiesAndField in typeof(T).GetAllPropertiesAndFields())
		{
			if (allPropertiesAndField.IsBackingField() || (isRecord && allPropertiesAndField.IsAutogeneratedRecordMember()))
			{
				continue;
			}
			InheritanceBehavior? inheritanceBehavior = InheritanceBehavior.Default;
			if (!GatherFieldData(allPropertiesAndField, out DataFieldBaseAttribute dataFieldBaseAttribute, out AbstractFieldInfo backingField, ref inheritanceBehavior))
			{
				continue;
			}
			if (dataFieldBaseAttribute == null)
			{
				if (!isRecord)
				{
					continue;
				}
				dataFieldBaseAttribute = new DataFieldAttribute(CamelCaseNamingConvention.Instance.Apply(allPropertiesAndField.Name));
			}
			FieldDefinition item = new FieldDefinition(dataFieldBaseAttribute, allPropertiesAndField.GetValue(val), allPropertiesAndField, backingField, inheritanceBehavior.Value);
			list.Add(item);
		}
		return list;
	}

	private PopulateDelegateSignature EmitPopulateDelegate(SerializationManager manager)
	{
		bool isServer = manager.DependencyCollection.Resolve<INetManager>().IsServer;
		ConstantExpression constantExpression = Expression.Constant(manager);
		ParameterExpression parameterExpression = Expression.Parameter(typeof(T).MakeByRefType());
		ParameterExpression parameterExpression2 = Expression.Parameter(typeof(MappingDataNode));
		ParameterExpression parameterExpression3 = Expression.Parameter(typeof(SerializationHookContext));
		ParameterExpression parameterExpression4 = Expression.Parameter(typeof(ISerializationContext));
		List<BlockExpression> list = new List<BlockExpression>();
		for (int i = 0; i < base.BaseFieldDefinitions.Length; i++)
		{
			FieldDefinition fieldDefinition = base.BaseFieldDefinitions[i];
			if (fieldDefinition.Attribute.ServerOnly && !isServer)
			{
				continue;
			}
			bool flag = NullableHelper.IsMarkedAsNullable(fieldDefinition.FieldInfo);
			ParameterExpression parameterExpression5 = Expression.Variable(typeof(DataNode));
			ParameterExpression parameterExpression6 = Expression.Variable(fieldDefinition.FieldType);
			Expression ifFalse;
			if (fieldDefinition.Attribute.CustomTypeSerializer != null && (FieldInterfaceInfos[i].Reader.Value || FieldInterfaceInfos[i].Reader.Sequence || FieldInterfaceInfos[i].Reader.Mapping))
			{
				List<SwitchCase> list2 = new List<SwitchCase>();
				bool nullable = fieldDefinition.FieldType.IsNullable();
				Type type = fieldDefinition.FieldType.EnsureNotNullableType();
				if (FieldInterfaceInfos[i].Reader.Value)
				{
					list2.Add(Expression.SwitchCase(Expression.Block(typeof(void), Expression.Assign(parameterExpression6, SerializationManager.WrapNullableIfNeededExpression(Expression.Call(constantExpression, "Read", new Type[3]
					{
						type,
						typeof(ValueDataNode),
						fieldDefinition.Attribute.CustomTypeSerializer
					}, Expression.Convert(parameterExpression5, typeof(ValueDataNode)), parameterExpression3, parameterExpression4, Expression.Constant(null, typeof(ISerializationManager.InstantiationDelegate<>).MakeGenericType(type)), Expression.Constant(!flag)), nullable))), Expression.Constant(typeof(ValueDataNode))));
				}
				if (FieldInterfaceInfos[i].Reader.Sequence)
				{
					list2.Add(Expression.SwitchCase(Expression.Block(typeof(void), Expression.Assign(parameterExpression6, SerializationManager.WrapNullableIfNeededExpression(Expression.Call(constantExpression, "Read", new Type[3]
					{
						type,
						typeof(SequenceDataNode),
						fieldDefinition.Attribute.CustomTypeSerializer
					}, Expression.Convert(parameterExpression5, typeof(SequenceDataNode)), parameterExpression3, parameterExpression4, Expression.Constant(null, typeof(ISerializationManager.InstantiationDelegate<>).MakeGenericType(type)), Expression.Constant(!flag)), nullable))), Expression.Constant(typeof(SequenceDataNode))));
				}
				if (FieldInterfaceInfos[i].Reader.Mapping)
				{
					list2.Add(Expression.SwitchCase(Expression.Block(typeof(void), Expression.Assign(parameterExpression6, SerializationManager.WrapNullableIfNeededExpression(Expression.Call(constantExpression, "Read", new Type[3]
					{
						type,
						typeof(MappingDataNode),
						fieldDefinition.Attribute.CustomTypeSerializer
					}, Expression.Convert(parameterExpression5, typeof(MappingDataNode)), parameterExpression3, parameterExpression4, Expression.Constant(null, typeof(ISerializationManager.InstantiationDelegate<>).MakeGenericType(type)), Expression.Constant(!flag)), nullable))), Expression.Constant(typeof(MappingDataNode))));
				}
				ifFalse = Expression.Switch(ExpressionUtils.GetTypeExpression(parameterExpression5), ExpressionUtils.ThrowExpression<InvalidOperationException>(new object[1] { $"Unable to read node for {fieldDefinition} as valid." }), list2.ToArray());
				ifFalse = Expression.IfThenElse(Expression.Call(typeof(SerializationManager), "IsNull", Type.EmptyTypes, parameterExpression5), flag ? ((Expression)Expression.Block(typeof(void), Expression.Assign(parameterExpression6, SerializationManager.GetNullExpression(constantExpression, type)))) : ((Expression)ExpressionUtils.ThrowExpression<NullNotAllowedException>(Array.Empty<object>())), ifFalse);
			}
			else
			{
				ifFalse = Expression.Assign(parameterExpression6, Expression.Call(constantExpression, "Read", new Type[1] { fieldDefinition.FieldType }, parameterExpression5, parameterExpression3, parameterExpression4, Expression.Constant(null, typeof(ISerializationManager.InstantiationDelegate<>).MakeGenericType(fieldDefinition.FieldType)), Expression.Constant(!flag)));
			}
			ifFalse = Expression.Block(new ParameterExpression[1] { parameterExpression6 }, ifFalse, AssignIfNotDefaultExpression(i, parameterExpression, parameterExpression6));
			if (fieldDefinition.Attribute is DataFieldAttribute dataFieldAttribute)
			{
				ConstantExpression constantExpression2 = Expression.Constant(dataFieldAttribute.Tag);
				list.Add(Expression.Block(new ParameterExpression[1] { parameterExpression5 }, Expression.IfThenElse(Expression.Call(parameterExpression2, typeof(MappingDataNode).GetMethod("TryGet", new Type[2]
				{
					typeof(string),
					typeof(DataNode).MakeByRefType()
				}), constantExpression2, parameterExpression5), ifFalse, dataFieldAttribute.Required ? ExpressionUtils.ThrowExpression<RequiredFieldNotMappedException>(new object[3]
				{
					fieldDefinition.FieldType,
					constantExpression2,
					typeof(T)
				}) : AssignIfNotDefaultExpression(i, parameterExpression, Expression.Constant(DefaultValues[i], fieldDefinition.FieldType)))));
			}
			else
			{
				list.Add(Expression.Block(new ParameterExpression[1] { parameterExpression5 }, Expression.Assign(parameterExpression5, parameterExpression2), ifFalse));
			}
		}
		return Expression.Lambda<PopulateDelegateSignature>(Expression.Block(list), new ParameterExpression[4] { parameterExpression, parameterExpression2, parameterExpression3, parameterExpression4 }).Compile();
	}

	private SerializeDelegateSignature EmitSerializeDelegate(SerializationManager manager)
	{
		ConstantExpression instance = Expression.Constant(manager);
		bool isServer = manager.DependencyCollection.Resolve<INetManager>().IsServer;
		ParameterExpression parameterExpression = Expression.Parameter(typeof(T));
		ParameterExpression parameterExpression2 = Expression.Parameter(typeof(ISerializationContext));
		ParameterExpression parameterExpression3 = Expression.Parameter(typeof(bool));
		List<Expression> list = new List<Expression>();
		ParameterExpression parameterExpression4 = Expression.Variable(typeof(MappingDataNode));
		list.Add(Expression.Assign(parameterExpression4, ExpressionUtils.NewExpression<MappingDataNode>(Array.Empty<object>())));
		for (int num = base.BaseFieldDefinitions.Length - 1; num >= 0; num--)
		{
			FieldDefinition fieldDefinition = base.BaseFieldDefinitions[num];
			if (!fieldDefinition.Attribute.ReadOnly && (!fieldDefinition.Attribute.ServerOnly || isServer))
			{
				bool flag = NullableHelper.IsMarkedAsNullable(fieldDefinition.FieldInfo);
				ParameterExpression parameterExpression5 = Expression.Variable(fieldDefinition.FieldType);
				Expression expression2;
				if (fieldDefinition.Attribute.CustomTypeSerializer != null && FieldInterfaceInfos[num].Writer)
				{
					Type type = fieldDefinition.FieldType.EnsureNotNullableType();
					Expression expression = ((fieldDefinition.FieldType.IsValueType && flag) ? ((Expression)Expression.Variable(type)) : ((Expression)Expression.Convert(parameterExpression5, type)));
					expression2 = Expression.Call(instance, "WriteValue", new Type[2]
					{
						type,
						fieldDefinition.Attribute.CustomTypeSerializer
					}, expression, parameterExpression3, parameterExpression2, Expression.Constant(!flag));
					if (fieldDefinition.FieldType.IsValueType && flag)
					{
						ParameterExpression parameterExpression6 = Expression.Variable(typeof(DataNode));
						expression2 = Expression.Block(new ParameterExpression[1] { parameterExpression6 }, Expression.IfThenElse(SerializationManager.StructNullHasValue(parameterExpression5), Expression.Block(new ParameterExpression[1] { (ParameterExpression)expression }, Expression.Assign(expression, Expression.Convert(parameterExpression5, type)), Expression.Assign(parameterExpression6, SerializationManager.WrapNullableIfNeededExpression(expression2, nullable: true))), flag ? ((Expression)Expression.Assign(parameterExpression6, Expression.Constant(ValueDataNode.Null()))) : ((Expression)ExpressionUtils.ThrowExpression<NullNotAllowedException>(Array.Empty<object>()))), parameterExpression6);
					}
				}
				else
				{
					expression2 = Expression.Call(instance, "WriteValue", new Type[1] { fieldDefinition.FieldType }, parameterExpression5, parameterExpression3, parameterExpression2, Expression.Constant(!flag));
				}
				ParameterExpression parameterExpression7 = Expression.Variable(typeof(DataNode));
				Expression expression3 = ((!(fieldDefinition.Attribute is DataFieldAttribute dataFieldAttribute)) ? Expression.IfThenElse(Expression.TypeIs(parameterExpression7, typeof(MappingDataNode)), Expression.Call(parameterExpression4, "Insert", Type.EmptyTypes, Expression.Convert(parameterExpression7, typeof(MappingDataNode)), Expression.Constant(true)), ExpressionUtils.ThrowExpression<InvalidOperationException>(new object[1] { $"Writing field {fieldDefinition} for type {typeof(T)} did not return a {"MappingDataNode"} but was annotated to be included." })) : Expression.IfThen(Expression.Not(Expression.Call(parameterExpression4, typeof(MappingDataNode).GetMethod("Has", new Type[1] { typeof(string) }), Expression.Constant(dataFieldAttribute.Tag))), Expression.Call(parameterExpression4, typeof(MappingDataNode).GetMethod("Add", new Type[2]
				{
					typeof(string),
					typeof(DataNode)
				}), Expression.Constant(dataFieldAttribute.Tag), parameterExpression7)));
				expression3 = Expression.Block(new ParameterExpression[1] { parameterExpression7 }, Expression.Assign(parameterExpression7, expression2), expression3);
				if (fieldDefinition.Attribute is DataFieldAttribute { Required: not false })
				{
					list.Add(Expression.Block(new ParameterExpression[1] { parameterExpression5 }, Expression.Assign(parameterExpression5, AccessExpression(num, parameterExpression)), expression3));
				}
				else
				{
					list.Add(Expression.Block(new ParameterExpression[1] { parameterExpression5 }, Expression.Assign(parameterExpression5, AccessExpression(num, parameterExpression)), Expression.IfThen(Expression.OrElse(parameterExpression3, Expression.Not(IsDefault(num, parameterExpression5, fieldDefinition))), expression3)));
				}
			}
		}
		list.Add(parameterExpression4);
		return Expression.Lambda<SerializeDelegateSignature>(Expression.Block(new ParameterExpression[1] { parameterExpression4 }, list), new ParameterExpression[3] { parameterExpression, parameterExpression2, parameterExpression3 }).Compile();
	}

	private CopyDelegateSignature EmitCopyDelegate(SerializationManager manager)
	{
		ConstantExpression constantExpression = Expression.Constant(manager);
		bool isServer = manager.DependencyCollection.Resolve<INetManager>().IsServer;
		ParameterExpression parameterExpression = Expression.Parameter(typeof(T));
		ParameterExpression parameterExpression2 = Expression.Parameter(typeof(T).MakeByRefType());
		ParameterExpression parameterExpression3 = Expression.Parameter(typeof(ISerializationContext));
		ParameterExpression parameterExpression4 = Expression.Parameter(typeof(SerializationHookContext));
		List<Expression> list = new List<Expression>();
		for (int i = 0; i < base.BaseFieldDefinitions.Length; i++)
		{
			FieldDefinition fieldDefinition = base.BaseFieldDefinitions[i];
			if (fieldDefinition.Attribute.ServerOnly && !isServer)
			{
				continue;
			}
			bool flag = NullableHelper.IsMarkedAsNullable(fieldDefinition.FieldInfo);
			Expression value;
			if (!(fieldDefinition.Attribute.CustomTypeSerializer != null) || !FieldInterfaceInfos[i].Copier)
			{
				value = ((!(fieldDefinition.Attribute.CustomTypeSerializer != null) || !FieldInterfaceInfos[i].CopyCreator) ? Expression.Call(constantExpression, "CreateCopy", new Type[1] { fieldDefinition.FieldType }, AccessExpression(i, parameterExpression), parameterExpression4, parameterExpression3, Expression.Constant(!flag)) : Expression.Call(constantExpression, "CreateCopy", new Type[2]
				{
					fieldDefinition.FieldType,
					fieldDefinition.Attribute.CustomTypeSerializer
				}, AccessExpression(i, parameterExpression), parameterExpression4, parameterExpression3, Expression.Constant(!flag)));
			}
			else
			{
				ParameterExpression parameterExpression5 = Expression.Variable(fieldDefinition.FieldType.EnsureNotNullableType());
				ParameterExpression parameterExpression6 = Expression.Variable(fieldDefinition.FieldType);
				Type type = fieldDefinition.FieldType.EnsureNotNullableType();
				Expression expression = ((type.IsValueType && flag) ? Expression.Variable(type) : AccessExpression(i, parameterExpression));
				value = Expression.Block(Expression.Call(constantExpression, "CopyTo", new Type[2]
				{
					type,
					fieldDefinition.Attribute.CustomTypeSerializer
				}, expression, parameterExpression5, parameterExpression4, parameterExpression3, Expression.Constant(!flag)), Expression.Assign(parameterExpression6, Expression.Convert(parameterExpression5, fieldDefinition.FieldType)));
				if (flag && type.IsValueType)
				{
					ParameterExpression parameterExpression7 = Expression.Variable(fieldDefinition.FieldType);
					value = Expression.Block(new ParameterExpression[2]
					{
						parameterExpression7,
						(ParameterExpression)expression
					}, Expression.Assign(parameterExpression7, AccessExpression(i, parameterExpression)), Expression.IfThenElse(SerializationManager.StructNullHasValue(parameterExpression7), Expression.Block(Expression.Assign(expression, Expression.Convert(parameterExpression7, type)), value), Expression.Assign(parameterExpression6, SerializationManager.GetNullExpression(constantExpression, type))));
				}
				value = Expression.Block(new ParameterExpression[2] { parameterExpression6, parameterExpression5 }, Expression.Assign(parameterExpression5, manager.InstantiationExpression(constantExpression, fieldDefinition.FieldType.EnsureNotNullableType())), value, parameterExpression6);
			}
			list.Add(AssignIfNotDefaultExpression(i, parameterExpression2, value));
		}
		return Expression.Lambda<CopyDelegateSignature>(Expression.Block(list), new ParameterExpression[4] { parameterExpression, parameterExpression2, parameterExpression4, parameterExpression3 }).Compile();
	}

	private ValidateFieldDelegate EmitFieldValidationDelegate(SerializationManager manager, int i)
	{
		ConstantExpression instance = Expression.Constant(manager);
		ParameterExpression parameterExpression = Expression.Parameter(typeof(DataNode));
		ParameterExpression parameterExpression2 = Expression.Parameter(typeof(ISerializationContext));
		FieldDefinition fieldDefinition = base.BaseFieldDefinitions[i];
		FieldInterfaceInfo fieldInterfaceInfo = FieldInterfaceInfos[i];
		Type type = fieldDefinition.FieldType.EnsureNotNullableType();
		List<SwitchCase> list = new List<SwitchCase>();
		if (fieldInterfaceInfo.Validator.Value)
		{
			list.Add(Expression.SwitchCase(Expression.Call(instance, "ValidateNode", new Type[3]
			{
				type,
				typeof(ValueDataNode),
				fieldDefinition.Attribute.CustomTypeSerializer
			}, Expression.Convert(parameterExpression, typeof(ValueDataNode)), parameterExpression2), Expression.Constant(typeof(ValueDataNode))));
		}
		if (fieldInterfaceInfo.Validator.Sequence)
		{
			list.Add(Expression.SwitchCase(Expression.Call(instance, "ValidateNode", new Type[3]
			{
				type,
				typeof(SequenceDataNode),
				fieldDefinition.Attribute.CustomTypeSerializer
			}, Expression.Convert(parameterExpression, typeof(SequenceDataNode)), parameterExpression2), Expression.Constant(typeof(SequenceDataNode))));
		}
		if (fieldInterfaceInfo.Validator.Mapping)
		{
			list.Add(Expression.SwitchCase(Expression.Call(instance, "ValidateNode", new Type[3]
			{
				type,
				typeof(MappingDataNode),
				fieldDefinition.Attribute.CustomTypeSerializer
			}, Expression.Convert(parameterExpression, typeof(MappingDataNode)), parameterExpression2), Expression.Constant(typeof(MappingDataNode))));
		}
		return Expression.Lambda<ValidateFieldDelegate>(Expression.Switch(ExpressionUtils.GetTypeExpression(parameterExpression), Expression.Call(instance, "ValidateNode", new Type[1] { type }, parameterExpression, parameterExpression2), list.ToArray()), new ParameterExpression[2] { parameterExpression, parameterExpression2 }).Compile();
	}

	private Expression AssignIfNotDefaultExpression(int i, Expression obj, Expression value)
	{
		object obj2 = FieldAssigners[i];
		return Expression.IfThen(ifTrue: (!(obj2 is FieldInfo field)) ? ((!(obj2 is MethodInfo method)) ? ((Expression)Expression.Invoke(Expression.Constant(obj2), obj, value)) : ((Expression)Expression.Call(obj, method, value))) : Expression.Assign(Expression.Field(obj, field), value), test: Expression.Not(ExpressionUtils.EqualExpression(Expression.Constant(DefaultValues[i], base.BaseFieldDefinitions[i].FieldType), value)));
	}

	private Expression AccessExpression(int i, Expression obj)
	{
		object obj2 = FieldAccessors[i];
		if (obj2 is FieldInfo field)
		{
			return Expression.Field(obj, field);
		}
		if (obj2 is MethodInfo method)
		{
			return Expression.Call(obj, method);
		}
		return Expression.Invoke(Expression.Constant(obj2), obj);
	}

	private Expression IsDefault(int i, Expression left, FieldDefinition fieldDefinition)
	{
		return ExpressionUtils.EqualExpression(left, Expression.Constant(DefaultValues[i], fieldDefinition.FieldType));
	}
}
