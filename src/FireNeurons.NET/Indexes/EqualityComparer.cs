using FireNeurons.NET.Objects;

namespace FireNeurons.NET.Indexes;

public class LayerEqualityComparer : EqualityComparer<Layer>
{
    private readonly IEqualityComparer<LayerIndex> comparer = EqualityComparer<LayerIndex>.Default;

    public override bool Equals(Layer l, Layer r)
    {
        return this.comparer.Equals(l.LayerIndex, r.LayerIndex);
    }

    public override int GetHashCode(Layer rule)
    {
        return this.comparer.GetHashCode(rule.LayerIndex);
    }
}