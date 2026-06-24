using Robust.Shared.Serialization.Markdown;

namespace Robust.Shared.Serialization.TypeSerializers.Interfaces;

public interface ITypeSerializer<TType, TNode> : ITypeReader<TType, TNode>, ITypeValidator<TType, TNode>, BaseSerializerInterfaces.ITypeNodeInterface<TType, TNode>, ITypeWriter<TType>, BaseSerializerInterfaces.ITypeInterface<TType> where TNode : DataNode
{
}
