using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;

namespace Content.Shared.Explosion;

[ByRefEvent]
public record struct BeforeExplodeEvent(DamageSpecifier Damage, string Id, List<EntityUid> Contents)
{
	public DamageSpecifier Damage = Damage;

	public readonly string Id = Id;

	public float DamageCoefficient = 1f;

	public readonly List<EntityUid> Contents = Contents;

	[CompilerGenerated]
	public readonly void Deconstruct(out DamageSpecifier Damage, out string Id, out List<EntityUid> Contents)
	{
		Damage = this.Damage;
		Id = this.Id;
		Contents = this.Contents;
	}
}
