using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Cargo.Prototypes;

[Prototype(null, 1)]
public sealed class CargoProductPrototype : IPrototype, IInheritingPrototype
{
	[DataField("name", false, 1, false, false, null)]
	private string _name = string.Empty;

	[DataField("description", false, 1, false, false, null)]
	private string _description = string.Empty;

	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<CargoProductPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[NeverPushInheritance]
	[AbstractDataField(1)]
	public bool Abstract { get; private set; }

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[ViewVariables]
	public string Name
	{
		get
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			if (_name.Trim().Length != 0)
			{
				return _name;
			}
			EntityPrototype prototype = default(EntityPrototype);
			if (IoCManager.Resolve<IPrototypeManager>().TryIndex(Product, ref prototype))
			{
				_name = prototype.Name;
			}
			return _name;
		}
	}

	[ViewVariables]
	public string Description
	{
		get
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			if (_description.Trim().Length != 0)
			{
				return _description;
			}
			EntityPrototype prototype = default(EntityPrototype);
			if (IoCManager.Resolve<IPrototypeManager>().TryIndex(Product, ref prototype))
			{
				_description = prototype.Description;
			}
			return _description;
		}
	}

	[DataField(null, false, 1, false, false, null)]
	public SpriteSpecifier Icon { get; private set; } = SpriteSpecifier.Invalid;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Product { get; private set; } = EntProtoId.op_Implicit(string.Empty);

	[DataField(null, false, 1, false, false, null)]
	public int Cost { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public string Category { get; private set; } = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<CargoMarketPrototype> Group { get; private set; } = ProtoId<CargoMarketPrototype>.op_Implicit("market");
}
