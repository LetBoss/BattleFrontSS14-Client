using Robust.Client.Graphics;
using Robust.Shared.Graphics.RSI;
using Robust.Shared.Maths;

namespace Content.Client.Clickable;

public interface IClickMapManager
{
	bool IsOccluding(Texture texture, Vector2i pos);

	bool IsOccluding(RSI rsi, StateId state, RsiDirection dir, int frame, Vector2i pos);
}
