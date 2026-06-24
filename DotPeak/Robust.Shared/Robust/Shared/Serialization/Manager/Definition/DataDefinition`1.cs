// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Manager.Definition.DataDefinition`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

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
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YamlDotNet.Serialization.NamingConventions;

#nullable enable
namespace Robust.Shared.Serialization.Manager.Definition;

internal sealed class DataDefinition<T> : DataDefinition where T : notnull
{
  internal readonly DataDefinition<
  #nullable disable
  T>.PopulateDelegateSignature Populate;
  internal readonly 
  #nullable enable
  DataDefinition<
  #nullable disable
  T>.SerializeDelegateSignature Serialize;
  internal readonly 
  #nullable enable
  DataDefinition<
  #nullable disable
  T>.CopyDelegateSignature CopyTo;

  internal DataDefinition(
  #nullable enable
  SerializationManager manager, bool isRecord)
  {
    this.IsRecord = isRecord;
    List<FieldDefinition> fieldDefinitions1 = this.GetFieldDefinitions(manager, isRecord);
    foreach (FieldDefinition fieldDefinition in fieldDefinitions1)
    {
      if (fieldDefinition.Attribute is DataFieldAttribute attribute && attribute.Tag == null)
        attribute.Tag = DataDefinitionUtility.AutoGenerateTag(fieldDefinition.FieldInfo.Name);
    }
    DataFieldAttribute[] dataFields = fieldDefinitions1.Select<FieldDefinition, DataFieldBaseAttribute>((Func<FieldDefinition, DataFieldBaseAttribute>) (f => f.Attribute)).OfType<DataFieldAttribute>().ToArray<DataFieldAttribute>();
    this.Duplicates = ((IEnumerable<DataFieldAttribute>) dataFields).Where<DataFieldAttribute>((Func<DataFieldAttribute, bool>) (f => ((IEnumerable<DataFieldAttribute>) dataFields).Count<DataFieldAttribute>((Func<DataFieldAttribute, bool>) (df => df.Tag == f.Tag)) > 1)).Select<DataFieldAttribute, string>((Func<DataFieldAttribute, string>) (f => f.Tag)).Distinct<string>().ToArray<string>();
    List<FieldDefinition> items1 = fieldDefinitions1;
    items1.Sort((Comparison<FieldDefinition>) ((a, b) => b.Attribute.Priority.CompareTo(a.Attribute.Priority)));
    this.BaseFieldDefinitions = items1.ToImmutableArray<FieldDefinition>();
    this.DefaultValues = fieldDefinitions1.Select<FieldDefinition, object>((Func<FieldDefinition, object>) (f => f.DefaultValue)).ToArray<object>();
    ImmutableArray<FieldDefinition> fieldDefinitions2 = this.BaseFieldDefinitions;
    object[] items2 = new object[fieldDefinitions2.Length];
    fieldDefinitions2 = this.BaseFieldDefinitions;
    object[] items3 = new object[fieldDefinitions2.Length];
    fieldDefinitions2 = this.BaseFieldDefinitions;
    DataDefinition<T>.ValidateFieldDelegate[] items4 = new DataDefinition<T>.ValidateFieldDelegate[fieldDefinitions2.Length];
    fieldDefinitions2 = this.BaseFieldDefinitions;
    DataDefinition<T>.FieldInterfaceInfo[] items5 = new DataDefinition<T>.FieldInterfaceInfo[fieldDefinitions2.Length];
    int index = 0;
    FieldDefinition fieldDefinition1;
    while (true)
    {
      int num = index;
      fieldDefinitions2 = this.BaseFieldDefinitions;
      int length = fieldDefinitions2.Length;
      if (num < length)
      {
        fieldDefinitions2 = this.BaseFieldDefinitions;
        fieldDefinition1 = fieldDefinitions2[index];
        items2[index] = InternalReflectionUtils.EmitFieldAssigner(typeof (T), fieldDefinition1.BackingField);
        items3[index] = InternalReflectionUtils.EmitFieldAccessor(typeof (T), fieldDefinition1);
        if (fieldDefinition1.Attribute.CustomTypeSerializer != (Type) null)
        {
          (bool, bool, bool) reader = (false, false, false);
          bool writer = false;
          bool copier = false;
          bool copyCreator = false;
          (bool, bool, bool) validator = (false, false, false);
          foreach (Type type in fieldDefinition1.Attribute.CustomTypeSerializer.GetInterfaces())
          {
            Type genericTypeDefinition = type.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof (ITypeWriter<>))
            {
              if (type.GenericTypeArguments[0].IsAssignableTo(fieldDefinition1.FieldType))
                writer = true;
            }
            else if (genericTypeDefinition == typeof (ITypeCopier<>))
            {
              if (type.GenericTypeArguments[0].IsAssignableTo(fieldDefinition1.FieldType))
                copier = true;
            }
            else if (genericTypeDefinition == typeof (ITypeCopyCreator<>))
            {
              if (type.GenericTypeArguments[0].IsAssignableTo(fieldDefinition1.FieldType))
                copyCreator = true;
            }
            else if (genericTypeDefinition == typeof (ITypeReader<,>))
            {
              if (type.GenericTypeArguments[0].IsAssignableTo(fieldDefinition1.FieldType))
              {
                if (type.GenericTypeArguments[1] == typeof (ValueDataNode))
                  reader.Item1 = true;
                else if (type.GenericTypeArguments[1] == typeof (SequenceDataNode))
                  reader.Item2 = true;
                else if (type.GenericTypeArguments[1] == typeof (MappingDataNode))
                  reader.Item3 = true;
              }
            }
            else if (genericTypeDefinition == typeof (ITypeValidator<,>) && type.GenericTypeArguments[0].IsAssignableTo(fieldDefinition1.FieldType))
            {
              if (type.GenericTypeArguments[1] == typeof (ValueDataNode))
                validator.Item1 = true;
              else if (type.GenericTypeArguments[1] == typeof (SequenceDataNode))
                validator.Item2 = true;
              else if (type.GenericTypeArguments[1] == typeof (MappingDataNode))
                validator.Item3 = true;
            }
          }
          if (reader.Item1 || reader.Item2 || reader.Item3 || writer || copier || validator.Item1 || validator.Item2 || validator.Item3)
            items5[index] = new DataDefinition<T>.FieldInterfaceInfo(reader, writer, copier, copyCreator, validator);
          else
            break;
        }
        ++index;
      }
      else
        goto label_40;
    }
    throw new InvalidOperationException($"Could not find any fitting implementation of ITypeReader, ITypeWriter or ITypeCopier for field {fieldDefinition1}({fieldDefinition1.FieldType}) on type {typeof (T)} on CustomTypeSerializer {fieldDefinition1.Attribute.CustomTypeSerializer}");
label_40:
    this.FieldInterfaceInfos = ((ReadOnlySpan<DataDefinition<T>.FieldInterfaceInfo>) items5).ToImmutableArray<DataDefinition<T>.FieldInterfaceInfo>();
    this.FieldAssigners = ((ReadOnlySpan<object>) items2).ToImmutableArray<object>();
    this.FieldAccessors = ((ReadOnlySpan<object>) items3).ToImmutableArray<object>();
    int i = 0;
    while (true)
    {
      int num = i;
      fieldDefinitions2 = this.BaseFieldDefinitions;
      int length = fieldDefinitions2.Length;
      if (num < length)
      {
        items4[i] = this.EmitFieldValidationDelegate(manager, i);
        ++i;
      }
      else
        break;
    }
    this.FieldValidators = ((ReadOnlySpan<DataDefinition<T>.ValidateFieldDelegate>) items4).ToImmutableArray<DataDefinition<T>.ValidateFieldDelegate>();
    this.Populate = this.EmitPopulateDelegate(manager);
    this.Serialize = this.EmitSerializeDelegate(manager);
    this.CopyTo = this.EmitCopyDelegate(manager);
  }

  private string[] Duplicates { get; }

  private object?[] DefaultValues { get; }

  private ImmutableArray<DataDefinition<
  #nullable disable
  T>.FieldInterfaceInfo> FieldInterfaceInfos { get; }

  private ImmutableArray<
  #nullable enable
  object> FieldAssigners { get; }

  private ImmutableArray<object> FieldAccessors { get; }

  private ImmutableArray<DataDefinition<
  #nullable disable
  T>.ValidateFieldDelegate> FieldValidators { get; }

  private bool TryGetIndex(
  #nullable enable
  string tag, out int index)
  {
    index = 0;
    while (true)
    {
      int num = index;
      ImmutableArray<FieldDefinition> fieldDefinitions = this.BaseFieldDefinitions;
      int length = fieldDefinitions.Length;
      if (num < length)
      {
        fieldDefinitions = this.BaseFieldDefinitions;
        if (!(fieldDefinitions[index].Attribute is DataFieldAttribute attribute) || !(attribute.Tag == tag))
          ++index;
        else
          break;
      }
      else
        goto label_5;
    }
    return true;
label_5:
    return false;
  }

  private bool TryGetIncludeMappingPair(
    List<ValidatedMappingNode> includeValidations,
    string key,
    out KeyValuePair<ValidationNode, ValidationNode> pair)
  {
    foreach (ValidatedMappingNode includeValidation in includeValidations)
    {
      KeyValuePair<ValidationNode, ValidationNode>? element;
      if (includeValidation.Mapping.TryFirstOrNull<KeyValuePair<ValidationNode, ValidationNode>>((Func<KeyValuePair<ValidationNode, ValidationNode>, bool>) (x => x.Key is ValidatedValueNode key1 && key1.DataNode is ValueDataNode dataNode && dataNode.Value == key), out element))
      {
        pair = element.Value;
        return true;
      }
    }
    pair = new KeyValuePair<ValidationNode, ValidationNode>();
    return false;
  }

  public ValidationNode Validate(
    ISerializationManager serialization,
    MappingDataNode mapping,
    ISerializationContext? context)
  {
    Dictionary<ValidationNode, ValidationNode> mapping1 = new Dictionary<ValidationNode, ValidationNode>();
    List<ValidatedMappingNode> includeValidations = new List<ValidatedMappingNode>();
    int index1 = 0;
    ImmutableArray<FieldDefinition> fieldDefinitions;
    ImmutableArray<DataDefinition<T>.ValidateFieldDelegate> fieldValidators;
    ValidationNode validationNode1;
    while (true)
    {
      int num = index1;
      fieldDefinitions = this.BaseFieldDefinitions;
      int length = fieldDefinitions.Length;
      if (num < length)
      {
        fieldDefinitions = this.BaseFieldDefinitions;
        FieldDefinition fieldDefinition = fieldDefinitions[index1];
        if (fieldDefinition.Attribute is IncludeDataFieldAttribute)
        {
          fieldValidators = this.FieldValidators;
          validationNode1 = fieldValidators[index1]((DataNode) mapping, context);
          ErrorNode errorNode = validationNode1 as ErrorNode;
          if ((object) errorNode != null)
            mapping1.Add((ValidationNode) new InconclusiveNode((DataNode) new ValueDataNode($"<{"IncludeDataFieldAttribute"}={fieldDefinition.FieldInfo.Name}>")), (ValidationNode) errorNode);
          else if (validationNode1 is ValidatedMappingNode validatedMappingNode)
            includeValidations.Add(validatedMappingNode);
          else
            break;
        }
        ++index1;
      }
      else
        goto label_9;
    }
    throw new InvalidValidationNodeReturnedException<ValidatedMappingNode>(validationNode1);
label_9:
    foreach ((string str, DataNode dataNode) in (IEnumerable<KeyValuePair<string, DataNode>>) mapping.Children)
    {
      int index2;
      if (!this.TryGetIndex(str, out index2))
      {
        KeyValuePair<ValidationNode, ValidationNode> pair;
        if (this.TryGetIncludeMappingPair(includeValidations, str, out pair))
        {
          mapping1.Add(pair.Key, pair.Value);
        }
        else
        {
          FieldNotFoundErrorNode key = new FieldNotFoundErrorNode(mapping.GetKeyNode(str), typeof (T));
          mapping1.Add((ValidationNode) key, (ValidationNode) new InconclusiveNode(dataNode));
        }
      }
      else
      {
        ValidationNode key = serialization.ValidateNode<string>((DataNode) mapping.GetKeyNode(str), context);
        ValidationNode validationNode2;
        if (SerializationManager.IsNull(dataNode))
        {
          fieldDefinitions = this.BaseFieldDefinitions;
          if (!NullableHelper.IsMarkedAsNullable(fieldDefinitions[index2].FieldInfo))
          {
            ErrorNode errorNode = new ErrorNode(dataNode, $"Field \"{str}\" had null value despite not being annotated as nullable.");
            mapping1.Add(key, (ValidationNode) errorNode);
            continue;
          }
          validationNode2 = (ValidationNode) new ValidatedValueNode(dataNode);
        }
        else
        {
          fieldValidators = this.FieldValidators;
          validationNode2 = fieldValidators[index2](dataNode, context);
        }
        KeyValuePair<ValidationNode, ValidationNode> pair;
        if ((object) (validationNode2 as ErrorNode) == null && this.TryGetIncludeMappingPair(includeValidations, str, out pair))
        {
          if ((object) (pair.Value as ErrorNode) != null)
            mapping1.Add(pair.Key, pair.Value);
        }
        else
          mapping1.Add(key, validationNode2);
      }
    }
    return (ValidationNode) new ValidatedMappingNode(mapping1);
  }

  public override bool TryGetDuplicates([NotNullWhen(true)] out string[] duplicates)
  {
    duplicates = this.Duplicates;
    return duplicates.Length != 0;
  }

  private bool GatherFieldData(
    AbstractFieldInfo fieldInfo,
    out DataFieldBaseAttribute? dataFieldBaseAttribute,
    [NotNullWhen(true)] out AbstractFieldInfo? backingField,
    [NotNullWhen(true)] ref InheritanceBehavior? inheritanceBehavior)
  {
    dataFieldBaseAttribute = (DataFieldBaseAttribute) null;
    backingField = fieldInfo;
    inheritanceBehavior.GetValueOrDefault();
    if (!inheritanceBehavior.HasValue)
    {
      InheritanceBehavior inheritanceBehavior1 = InheritanceBehavior.Default;
      inheritanceBehavior = new InheritanceBehavior?(inheritanceBehavior1);
    }
    if (fieldInfo.HasAttribute<AlwaysPushInheritanceAttribute>(true))
      inheritanceBehavior = new InheritanceBehavior?(InheritanceBehavior.Always);
    else if (fieldInfo.HasAttribute<NeverPushInheritanceAttribute>(true))
      inheritanceBehavior = new InheritanceBehavior?(InheritanceBehavior.Never);
    if (fieldInfo is SpecificPropertyInfo specificPropertyInfo)
    {
      if (!specificPropertyInfo.IsMostOverridden(typeof (T)))
        return false;
      if (specificPropertyInfo.PropertyInfo.GetMethod == (MethodInfo) null)
      {
        Logger.ErrorS("serialization", $"Property {specificPropertyInfo} is annotated with DataFieldAttribute but has no getter");
        return false;
      }
    }
    DataFieldAttribute attribute1;
    if (fieldInfo.TryGetAttribute<DataFieldAttribute>(out attribute1, true))
      return DataDefinition<T>.GatherDataFieldData(fieldInfo, out dataFieldBaseAttribute, ref backingField, attribute1);
    IncludeDataFieldAttribute attribute2;
    if (fieldInfo.TryGetAttribute<IncludeDataFieldAttribute>(out attribute2, true))
    {
      dataFieldBaseAttribute = (DataFieldBaseAttribute) attribute2;
      return true;
    }
    if (!(fieldInfo is SpecificPropertyInfo))
      return true;
    SpecificFieldInfo backingField1 = fieldInfo.GetBackingField();
    return backingField1 != null && this.GatherFieldData((AbstractFieldInfo) backingField1, out dataFieldBaseAttribute, out backingField, ref inheritanceBehavior);
  }

  private static bool GatherDataFieldData(
    AbstractFieldInfo fieldInfo,
    out DataFieldBaseAttribute dataFieldBaseAttribute,
    ref AbstractFieldInfo backingField,
    DataFieldAttribute dataFieldAttribute)
  {
    dataFieldBaseAttribute = (DataFieldBaseAttribute) dataFieldAttribute;
    if (!(fieldInfo is SpecificPropertyInfo specificPropertyInfo) || dataFieldAttribute.ReadOnly || specificPropertyInfo.PropertyInfo.SetMethod != (MethodInfo) null)
      return true;
    SpecificFieldInfo field;
    if (!specificPropertyInfo.TryGetBackingField(out field))
    {
      Logger.ErrorS("serialization", $"Property {specificPropertyInfo} in type {specificPropertyInfo.DeclaringType} is annotated with DataFieldAttribute as non-readonly but has no auto-setter");
      return false;
    }
    backingField = (AbstractFieldInfo) field;
    return true;
  }

  private List<FieldDefinition> GetFieldDefinitions(SerializationManager manager, bool isRecord)
  {
    T obj = manager.GetOrCreateInstantiator<T>(isRecord)();
    List<FieldDefinition> fieldDefinitions = new List<FieldDefinition>();
    foreach (AbstractFieldInfo propertiesAndField in typeof (T).GetAllPropertiesAndFields())
    {
      if (!propertiesAndField.IsBackingField() && (!isRecord || !propertiesAndField.IsAutogeneratedRecordMember()))
      {
        InheritanceBehavior? inheritanceBehavior = new InheritanceBehavior?(InheritanceBehavior.Default);
        DataFieldBaseAttribute dataFieldBaseAttribute;
        AbstractFieldInfo backingField;
        if (this.GatherFieldData(propertiesAndField, out dataFieldBaseAttribute, out backingField, ref inheritanceBehavior))
        {
          if (dataFieldBaseAttribute == null)
          {
            if (isRecord)
              dataFieldBaseAttribute = (DataFieldBaseAttribute) new DataFieldAttribute(CamelCaseNamingConvention.Instance.Apply(propertiesAndField.Name));
            else
              continue;
          }
          FieldDefinition fieldDefinition = new FieldDefinition(dataFieldBaseAttribute, propertiesAndField.GetValue((object) obj), propertiesAndField, backingField, inheritanceBehavior.Value);
          fieldDefinitions.Add(fieldDefinition);
        }
      }
    }
    return fieldDefinitions;
  }

  private DataDefinition<
  #nullable disable
  T>.PopulateDelegateSignature EmitPopulateDelegate(
  #nullable enable
  SerializationManager manager)
  {
    bool isServer = manager.DependencyCollection.Resolve<INetManager>().IsServer;
    ConstantExpression constantExpression1 = Expression.Constant((object) manager);
    ParameterExpression parameterExpression5 = Expression.Parameter(typeof (T).MakeByRefType());
    ParameterExpression parameterExpression6 = Expression.Parameter(typeof (MappingDataNode));
    ParameterExpression parameterExpression7 = Expression.Parameter(typeof (SerializationHookContext));
    ParameterExpression parameterExpression8 = Expression.Parameter(typeof (ISerializationContext));
    List<BlockExpression> blockExpressionList1 = new List<BlockExpression>();
    int index = 0;
    while (true)
    {
      int num = index;
      ImmutableArray<FieldDefinition> fieldDefinitions = this.BaseFieldDefinitions;
      int length = fieldDefinitions.Length;
      if (num < length)
      {
        fieldDefinitions = this.BaseFieldDefinitions;
        FieldDefinition fieldDefinition = fieldDefinitions[index];
        if (!fieldDefinition.Attribute.ServerOnly || isServer)
        {
          bool flag = NullableHelper.IsMarkedAsNullable(fieldDefinition.FieldInfo);
          ParameterExpression left1 = Expression.Variable(typeof (DataNode));
          ParameterExpression left2 = Expression.Variable(fieldDefinition.FieldType);
          Expression expression1;
          if (fieldDefinition.Attribute.CustomTypeSerializer != (Type) null && (this.FieldInterfaceInfos[index].Reader.Value || this.FieldInterfaceInfos[index].Reader.Sequence || this.FieldInterfaceInfos[index].Reader.Mapping))
          {
            List<SwitchCase> switchCaseList = new List<SwitchCase>();
            bool nullable = fieldDefinition.FieldType.IsNullable();
            Type type = fieldDefinition.FieldType.EnsureNotNullableType();
            if (this.FieldInterfaceInfos[index].Reader.Value)
              switchCaseList.Add(Expression.SwitchCase((Expression) Expression.Block(typeof (void), (Expression) Expression.Assign((Expression) left2, SerializationManager.WrapNullableIfNeededExpression((Expression) Expression.Call((Expression) constantExpression1, "Read", new Type[3]
              {
                type,
                typeof (ValueDataNode),
                fieldDefinition.Attribute.CustomTypeSerializer
              }, (Expression) Expression.Convert((Expression) left1, typeof (ValueDataNode)), (Expression) parameterExpression7, (Expression) parameterExpression8, (Expression) Expression.Constant((object) null, typeof (ISerializationManager.InstantiationDelegate<>).MakeGenericType(type)), (Expression) Expression.Constant((object) !flag)), nullable))), (Expression) Expression.Constant((object) typeof (ValueDataNode))));
            if (this.FieldInterfaceInfos[index].Reader.Sequence)
              switchCaseList.Add(Expression.SwitchCase((Expression) Expression.Block(typeof (void), (Expression) Expression.Assign((Expression) left2, SerializationManager.WrapNullableIfNeededExpression((Expression) Expression.Call((Expression) constantExpression1, "Read", new Type[3]
              {
                type,
                typeof (SequenceDataNode),
                fieldDefinition.Attribute.CustomTypeSerializer
              }, (Expression) Expression.Convert((Expression) left1, typeof (SequenceDataNode)), (Expression) parameterExpression7, (Expression) parameterExpression8, (Expression) Expression.Constant((object) null, typeof (ISerializationManager.InstantiationDelegate<>).MakeGenericType(type)), (Expression) Expression.Constant((object) !flag)), nullable))), (Expression) Expression.Constant((object) typeof (SequenceDataNode))));
            if (this.FieldInterfaceInfos[index].Reader.Mapping)
              switchCaseList.Add(Expression.SwitchCase((Expression) Expression.Block(typeof (void), (Expression) Expression.Assign((Expression) left2, SerializationManager.WrapNullableIfNeededExpression((Expression) Expression.Call((Expression) constantExpression1, "Read", new Type[3]
              {
                type,
                typeof (MappingDataNode),
                fieldDefinition.Attribute.CustomTypeSerializer
              }, (Expression) Expression.Convert((Expression) left1, typeof (MappingDataNode)), (Expression) parameterExpression7, (Expression) parameterExpression8, (Expression) Expression.Constant((object) null, typeof (ISerializationManager.InstantiationDelegate<>).MakeGenericType(type)), (Expression) Expression.Constant((object) !flag)), nullable))), (Expression) Expression.Constant((object) typeof (MappingDataNode))));
            Expression expression2 = (Expression) Expression.Switch((Expression) ExpressionUtils.GetTypeExpression((Expression) left1), (Expression) ExpressionUtils.ThrowExpression<InvalidOperationException>((object) $"Unable to read node for {fieldDefinition} as valid."), switchCaseList.ToArray());
            MethodCallExpression test = Expression.Call(typeof (SerializationManager), "IsNull", Type.EmptyTypes, (Expression) left1);
            Expression ifTrue;
            if (!flag)
              ifTrue = (Expression) ExpressionUtils.ThrowExpression<NullNotAllowedException>();
            else
              ifTrue = (Expression) Expression.Block(typeof (void), (Expression) Expression.Assign((Expression) left2, SerializationManager.GetNullExpression((Expression) constantExpression1, type)));
            Expression ifFalse = expression2;
            expression1 = (Expression) Expression.IfThenElse((Expression) test, ifTrue, ifFalse);
          }
          else
            expression1 = (Expression) Expression.Assign((Expression) left2, (Expression) Expression.Call((Expression) constantExpression1, "Read", new Type[1]
            {
              fieldDefinition.FieldType
            }, (Expression) left1, (Expression) parameterExpression7, (Expression) parameterExpression8, (Expression) Expression.Constant((object) null, typeof (ISerializationManager.InstantiationDelegate<>).MakeGenericType(fieldDefinition.FieldType)), (Expression) Expression.Constant((object) !flag)));
          Expression expression3 = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
          {
            left2
          }, expression1, this.AssignIfNotDefaultExpression(index, (Expression) parameterExpression5, (Expression) left2));
          if (fieldDefinition.Attribute is DataFieldAttribute attribute)
          {
            ConstantExpression constantExpression2 = Expression.Constant((object) attribute.Tag);
            List<BlockExpression> blockExpressionList2 = blockExpressionList1;
            ParameterExpression[] variables = new ParameterExpression[1]
            {
              left1
            };
            Expression[] expressionArray = new Expression[1];
            MethodCallExpression test = Expression.Call((Expression) parameterExpression6, typeof (MappingDataNode).GetMethod("TryGet", new Type[2]
            {
              typeof (string),
              typeof (DataNode).MakeByRefType()
            }), (Expression) constantExpression2, (Expression) left1);
            Expression ifTrue = expression3;
            Expression ifFalse;
            if (!attribute.Required)
              ifFalse = this.AssignIfNotDefaultExpression(index, (Expression) parameterExpression5, (Expression) Expression.Constant(this.DefaultValues[index], fieldDefinition.FieldType));
            else
              ifFalse = (Expression) ExpressionUtils.ThrowExpression<RequiredFieldNotMappedException>((object) fieldDefinition.FieldType, (object) constantExpression2, (object) typeof (T));
            expressionArray[0] = (Expression) Expression.IfThenElse((Expression) test, ifTrue, ifFalse);
            BlockExpression blockExpression = Expression.Block((IEnumerable<ParameterExpression>) variables, expressionArray);
            blockExpressionList2.Add(blockExpression);
          }
          else
            blockExpressionList1.Add(Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
            {
              left1
            }, (Expression) Expression.Assign((Expression) left1, (Expression) parameterExpression6), expression3));
        }
        ++index;
      }
      else
        break;
    }
    return ((Expression<DataDefinition<T>.PopulateDelegateSignature>) ((parameterExpression1, parameterExpression2, parameterExpression3, parameterExpression4) => Expression.Block((IEnumerable<Expression>) blockExpressionList1))).Compile();
  }

  private DataDefinition<
  #nullable disable
  T>.SerializeDelegateSignature EmitSerializeDelegate(
  #nullable enable
  SerializationManager manager)
  {
    ConstantExpression instance = Expression.Constant((object) manager);
    bool isServer = manager.DependencyCollection.Resolve<INetManager>().IsServer;
    ParameterExpression parameterExpression4 = Expression.Parameter(typeof (T));
    ParameterExpression parameterExpression5 = Expression.Parameter(typeof (ISerializationContext));
    ParameterExpression left1 = Expression.Parameter(typeof (bool));
    List<Expression> expressionList = new List<Expression>();
    ParameterExpression parameterExpression6 = Expression.Variable(typeof (MappingDataNode));
    expressionList.Add((Expression) Expression.Assign((Expression) parameterExpression6, (Expression) ExpressionUtils.NewExpression<MappingDataNode>()));
    for (int index = this.BaseFieldDefinitions.Length - 1; index >= 0; --index)
    {
      FieldDefinition baseFieldDefinition = this.BaseFieldDefinitions[index];
      if (!baseFieldDefinition.Attribute.ReadOnly && (!baseFieldDefinition.Attribute.ServerOnly || isServer))
      {
        bool flag = NullableHelper.IsMarkedAsNullable(baseFieldDefinition.FieldInfo);
        ParameterExpression parameterExpression7 = Expression.Variable(baseFieldDefinition.FieldType);
        Expression expression1;
        if (baseFieldDefinition.Attribute.CustomTypeSerializer != (Type) null && this.FieldInterfaceInfos[index].Writer)
        {
          Type type = baseFieldDefinition.FieldType.EnsureNotNullableType();
          Expression left2 = baseFieldDefinition.FieldType.IsValueType & flag ? (Expression) Expression.Variable(type) : (Expression) Expression.Convert((Expression) parameterExpression7, type);
          expression1 = (Expression) Expression.Call((Expression) instance, "WriteValue", new Type[2]
          {
            type,
            baseFieldDefinition.Attribute.CustomTypeSerializer
          }, left2, (Expression) left1, (Expression) parameterExpression5, (Expression) Expression.Constant((object) !flag));
          if (baseFieldDefinition.FieldType.IsValueType & flag)
          {
            ParameterExpression left3 = Expression.Variable(typeof (DataNode));
            expression1 = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
            {
              left3
            }, (Expression) Expression.IfThenElse(SerializationManager.StructNullHasValue((Expression) parameterExpression7), (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
            {
              (ParameterExpression) left2
            }, (Expression) Expression.Assign(left2, (Expression) Expression.Convert((Expression) parameterExpression7, type)), (Expression) Expression.Assign((Expression) left3, SerializationManager.WrapNullableIfNeededExpression(expression1, true))), flag ? (Expression) Expression.Assign((Expression) left3, (Expression) Expression.Constant((object) ValueDataNode.Null())) : (Expression) ExpressionUtils.ThrowExpression<NullNotAllowedException>()), (Expression) left3);
          }
        }
        else
          expression1 = (Expression) Expression.Call((Expression) instance, "WriteValue", new Type[1]
          {
            baseFieldDefinition.FieldType
          }, (Expression) parameterExpression7, (Expression) left1, (Expression) parameterExpression5, (Expression) Expression.Constant((object) !flag));
        ParameterExpression left4 = Expression.Variable(typeof (DataNode));
        Expression expression2;
        if (baseFieldDefinition.Attribute is DataFieldAttribute attribute1)
          expression2 = (Expression) Expression.IfThen((Expression) Expression.Not((Expression) Expression.Call((Expression) parameterExpression6, typeof (MappingDataNode).GetMethod("Has", new Type[1]
          {
            typeof (string)
          }), (Expression) Expression.Constant((object) attribute1.Tag))), (Expression) Expression.Call((Expression) parameterExpression6, typeof (MappingDataNode).GetMethod("Add", new Type[2]
          {
            typeof (string),
            typeof (DataNode)
          }), (Expression) Expression.Constant((object) attribute1.Tag), (Expression) left4));
        else
          expression2 = (Expression) Expression.IfThenElse((Expression) Expression.TypeIs((Expression) left4, typeof (MappingDataNode)), (Expression) Expression.Call((Expression) parameterExpression6, "Insert", Type.EmptyTypes, (Expression) Expression.Convert((Expression) left4, typeof (MappingDataNode)), (Expression) Expression.Constant((object) true)), (Expression) ExpressionUtils.ThrowExpression<InvalidOperationException>((object) $"Writing field {baseFieldDefinition} for type {typeof (T)} did not return a {"MappingDataNode"} but was annotated to be included."));
        Expression ifTrue = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
        {
          left4
        }, (Expression) Expression.Assign((Expression) left4, expression1), expression2);
        if (!(baseFieldDefinition.Attribute is DataFieldAttribute attribute2) || !attribute2.Required)
          expressionList.Add((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
          {
            parameterExpression7
          }, (Expression) Expression.Assign((Expression) parameterExpression7, this.AccessExpression(index, (Expression) parameterExpression4)), (Expression) Expression.IfThen((Expression) Expression.OrElse((Expression) left1, (Expression) Expression.Not(this.IsDefault(index, (Expression) parameterExpression7, baseFieldDefinition))), ifTrue)));
        else
          expressionList.Add((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
          {
            parameterExpression7
          }, (Expression) Expression.Assign((Expression) parameterExpression7, this.AccessExpression(index, (Expression) parameterExpression4)), ifTrue));
      }
    }
    expressionList.Add((Expression) parameterExpression6);
    return ((Expression<DataDefinition<T>.SerializeDelegateSignature>) ((parameterExpression1, parameterExpression2, parameterExpression3) => Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      parameterExpression6
    }, (IEnumerable<Expression>) expressionList))).Compile();
  }

  private DataDefinition<
  #nullable disable
  T>.CopyDelegateSignature EmitCopyDelegate(
  #nullable enable
  SerializationManager manager)
  {
    ConstantExpression constantExpression = Expression.Constant((object) manager);
    bool isServer = manager.DependencyCollection.Resolve<INetManager>().IsServer;
    ParameterExpression parameterExpression5 = Expression.Parameter(typeof (T));
    ParameterExpression parameterExpression6 = Expression.Parameter(typeof (T).MakeByRefType());
    ParameterExpression parameterExpression7 = Expression.Parameter(typeof (ISerializationContext));
    ParameterExpression parameterExpression8 = Expression.Parameter(typeof (SerializationHookContext));
    List<Expression> expressionList = new List<Expression>();
    int num1 = 0;
    while (true)
    {
      int num2 = num1;
      ImmutableArray<FieldDefinition> fieldDefinitions = this.BaseFieldDefinitions;
      int length = fieldDefinitions.Length;
      if (num2 < length)
      {
        fieldDefinitions = this.BaseFieldDefinitions;
        FieldDefinition fieldDefinition = fieldDefinitions[num1];
        if (!fieldDefinition.Attribute.ServerOnly || isServer)
        {
          bool flag = NullableHelper.IsMarkedAsNullable(fieldDefinition.FieldInfo);
          Expression expression1;
          if (fieldDefinition.Attribute.CustomTypeSerializer != (Type) null && this.FieldInterfaceInfos[num1].Copier)
          {
            ParameterExpression left1 = Expression.Variable(fieldDefinition.FieldType.EnsureNotNullableType());
            ParameterExpression left2 = Expression.Variable(fieldDefinition.FieldType);
            Type type = fieldDefinition.FieldType.EnsureNotNullableType();
            Expression left3 = type.IsValueType & flag ? (Expression) Expression.Variable(type) : this.AccessExpression(num1, (Expression) parameterExpression5);
            Expression expression2 = (Expression) Expression.Block((Expression) Expression.Call((Expression) constantExpression, "CopyTo", new Type[2]
            {
              type,
              fieldDefinition.Attribute.CustomTypeSerializer
            }, left3, (Expression) left1, (Expression) parameterExpression8, (Expression) parameterExpression7, (Expression) Expression.Constant((object) !flag)), (Expression) Expression.Assign((Expression) left2, (Expression) Expression.Convert((Expression) left1, fieldDefinition.FieldType)));
            if (flag && type.IsValueType)
            {
              ParameterExpression parameterExpression9 = Expression.Variable(fieldDefinition.FieldType);
              expression2 = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[2]
              {
                parameterExpression9,
                (ParameterExpression) left3
              }, (Expression) Expression.Assign((Expression) parameterExpression9, this.AccessExpression(num1, (Expression) parameterExpression5)), (Expression) Expression.IfThenElse(SerializationManager.StructNullHasValue((Expression) parameterExpression9), (Expression) Expression.Block((Expression) Expression.Assign(left3, (Expression) Expression.Convert((Expression) parameterExpression9, type)), expression2), (Expression) Expression.Assign((Expression) left2, SerializationManager.GetNullExpression((Expression) constantExpression, type))));
            }
            expression1 = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[2]
            {
              left2,
              left1
            }, (Expression) Expression.Assign((Expression) left1, (Expression) manager.InstantiationExpression(constantExpression, fieldDefinition.FieldType.EnsureNotNullableType())), expression2, (Expression) left2);
          }
          else if (fieldDefinition.Attribute.CustomTypeSerializer != (Type) null && this.FieldInterfaceInfos[num1].CopyCreator)
            expression1 = (Expression) Expression.Call((Expression) constantExpression, "CreateCopy", new Type[2]
            {
              fieldDefinition.FieldType,
              fieldDefinition.Attribute.CustomTypeSerializer
            }, this.AccessExpression(num1, (Expression) parameterExpression5), (Expression) parameterExpression8, (Expression) parameterExpression7, (Expression) Expression.Constant((object) !flag));
          else
            expression1 = (Expression) Expression.Call((Expression) constantExpression, "CreateCopy", new Type[1]
            {
              fieldDefinition.FieldType
            }, this.AccessExpression(num1, (Expression) parameterExpression5), (Expression) parameterExpression8, (Expression) parameterExpression7, (Expression) Expression.Constant((object) !flag));
          expressionList.Add(this.AssignIfNotDefaultExpression(num1, (Expression) parameterExpression6, expression1));
        }
        ++num1;
      }
      else
        break;
    }
    return ((Expression<DataDefinition<T>.CopyDelegateSignature>) ((parameterExpression1, parameterExpression2, parameterExpression3, parameterExpression4) => Expression.Block((IEnumerable<Expression>) expressionList))).Compile();
  }

  private DataDefinition<
  #nullable disable
  T>.ValidateFieldDelegate EmitFieldValidationDelegate(
  #nullable enable
  SerializationManager manager, int i)
  {
    ConstantExpression instance = Expression.Constant((object) manager);
    ParameterExpression parameterExpression3 = Expression.Parameter(typeof (DataNode));
    ParameterExpression parameterExpression4 = Expression.Parameter(typeof (ISerializationContext));
    FieldDefinition baseFieldDefinition = this.BaseFieldDefinitions[i];
    DataDefinition<T>.FieldInterfaceInfo fieldInterfaceInfo = this.FieldInterfaceInfos[i];
    Type type = baseFieldDefinition.FieldType.EnsureNotNullableType();
    List<SwitchCase> switchCaseList = new List<SwitchCase>();
    if (fieldInterfaceInfo.Validator.Value)
      switchCaseList.Add(Expression.SwitchCase((Expression) Expression.Call((Expression) instance, "ValidateNode", new Type[3]
      {
        type,
        typeof (ValueDataNode),
        baseFieldDefinition.Attribute.CustomTypeSerializer
      }, (Expression) Expression.Convert((Expression) parameterExpression3, typeof (ValueDataNode)), (Expression) parameterExpression4), (Expression) Expression.Constant((object) typeof (ValueDataNode))));
    if (fieldInterfaceInfo.Validator.Sequence)
      switchCaseList.Add(Expression.SwitchCase((Expression) Expression.Call((Expression) instance, "ValidateNode", new Type[3]
      {
        type,
        typeof (SequenceDataNode),
        baseFieldDefinition.Attribute.CustomTypeSerializer
      }, (Expression) Expression.Convert((Expression) parameterExpression3, typeof (SequenceDataNode)), (Expression) parameterExpression4), (Expression) Expression.Constant((object) typeof (SequenceDataNode))));
    if (fieldInterfaceInfo.Validator.Mapping)
      switchCaseList.Add(Expression.SwitchCase((Expression) Expression.Call((Expression) instance, "ValidateNode", new Type[3]
      {
        type,
        typeof (MappingDataNode),
        baseFieldDefinition.Attribute.CustomTypeSerializer
      }, (Expression) Expression.Convert((Expression) parameterExpression3, typeof (MappingDataNode)), (Expression) parameterExpression4), (Expression) Expression.Constant((object) typeof (MappingDataNode))));
    return ((Expression<DataDefinition<T>.ValidateFieldDelegate>) ((parameterExpression1, parameterExpression2) => Expression.Switch((Expression) ExpressionUtils.GetTypeExpression((Expression) parameterExpression3), Expression.Call((Expression) instance, "ValidateNode", new Type[1]
    {
      type
    }, (Expression) parameterExpression3, (Expression) parameterExpression4), switchCaseList.ToArray()))).Compile();
  }

  private Expression AssignIfNotDefaultExpression(int i, Expression obj, Expression value)
  {
    object fieldAssigner = this.FieldAssigners[i];
    FieldInfo field = fieldAssigner as FieldInfo;
    Expression ifTrue;
    if ((object) field != null)
    {
      ifTrue = (Expression) Expression.Assign((Expression) Expression.Field(obj, field), value);
    }
    else
    {
      MethodInfo method = fieldAssigner as MethodInfo;
      if ((object) method != null)
        ifTrue = (Expression) Expression.Call(obj, method, value);
      else
        ifTrue = (Expression) Expression.Invoke((Expression) Expression.Constant(fieldAssigner), obj, value);
    }
    return (Expression) Expression.IfThen((Expression) Expression.Not(ExpressionUtils.EqualExpression((Expression) Expression.Constant(this.DefaultValues[i], this.BaseFieldDefinitions[i].FieldType), value)), ifTrue);
  }

  private Expression AccessExpression(int i, Expression obj)
  {
    object fieldAccessor = this.FieldAccessors[i];
    FieldInfo field = fieldAccessor as FieldInfo;
    if ((object) field != null)
      return (Expression) Expression.Field(obj, field);
    MethodInfo method = fieldAccessor as MethodInfo;
    if ((object) method != null)
      return (Expression) Expression.Call(obj, method);
    return (Expression) Expression.Invoke((Expression) Expression.Constant(fieldAccessor), obj);
  }

  private Expression IsDefault(int i, Expression left, FieldDefinition fieldDefinition)
  {
    return ExpressionUtils.EqualExpression(left, (Expression) Expression.Constant(this.DefaultValues[i], fieldDefinition.FieldType));
  }

  private readonly struct FieldInterfaceInfo(
    (bool Value, bool Sequence, bool Mapping) reader,
    bool writer,
    bool copier,
    bool copyCreator,
    (bool Value, bool Sequence, bool Mapping) validator)
  {
    public readonly (bool Value, bool Sequence, bool Mapping) Reader = reader;
    public readonly bool Writer = writer;
    public readonly bool Copier = copier;
    public readonly bool CopyCreator = copyCreator;
    public readonly (bool Value, bool Sequence, bool Mapping) Validator = validator;
  }

  public delegate void PopulateDelegateSignature(
    ref T target,
    MappingDataNode mappingDataNode,
    SerializationHookContext hookCtx,
    ISerializationContext? context);

  public delegate MappingDataNode SerializeDelegateSignature(
    T obj,
    ISerializationContext? context,
    bool alwaysWrite);

  public delegate void CopyDelegateSignature(
    T source,
    ref T target,
    SerializationHookContext hookCtx,
    ISerializationContext? context);

  private delegate ValidationNode ValidateFieldDelegate(
    DataNode node,
    ISerializationContext? context);
}
