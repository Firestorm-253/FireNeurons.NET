using System.Diagnostics.CodeAnalysis;

namespace FireNeurons.NET.Indexes;

public readonly struct LayerIndex : IEquatable<LayerIndex>
{
    public int Index { get; init; }

    public LayerIndex(int index)
    {
        this.Index = index;
    }

    public override bool Equals([NotNullWhen(true)] object? value) => (value is LayerIndex index) && this.Index.Equals(index.Index);
    public bool Equals(LayerIndex other) => this.Index == other.Index;
    public override int GetHashCode() => this.Index.GetHashCode();

    public static bool operator ==(LayerIndex a, LayerIndex b) => a.Equals(b);
    public static bool operator !=(LayerIndex a, LayerIndex b) => !a.Equals(b);

    public static implicit operator LayerIndex(int index) => new(index);
}
