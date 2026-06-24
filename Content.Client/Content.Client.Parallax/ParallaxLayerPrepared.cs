using Content.Client.Parallax.Data;
using Robust.Client.Graphics;

namespace Content.Client.Parallax;

public struct ParallaxLayerPrepared
{
	public Texture Texture { get; set; }

	public ParallaxLayerConfig Config { get; set; }
}
