namespace FireNeurons.NET.Dto;
using Indexes;
using Objects;

[Serializable]
public record ConnectionDto
{
    public ConnectionIndex Index { get; init; }

    public double Weight { get; init; }
    public NeuronIndex InputNeuron { get; init; }
    public NeuronIndex OutputNeuron { get; init; }

    public ConnectionDto() { }
    public ConnectionDto(Connection connection)
    {
        this.Index = connection.Index;
        this.Weight = connection.Weight;
        this.InputNeuron = connection.InputNeuron.Index;
        this.OutputNeuron = connection.OutputNeuron.Index;
    }
}
