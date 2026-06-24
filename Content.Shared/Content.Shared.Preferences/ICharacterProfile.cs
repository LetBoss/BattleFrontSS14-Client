using Content.Shared.Humanoid;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Shared.Preferences;

public interface ICharacterProfile
{
	string Name { get; }

	ICharacterAppearance CharacterAppearance { get; }

	bool MemberwiseEquals(ICharacterProfile other);

	void EnsureValid(ICommonSession session, IDependencyCollection collection);

	ICharacterProfile Validated(ICommonSession session, IDependencyCollection collection);
}
