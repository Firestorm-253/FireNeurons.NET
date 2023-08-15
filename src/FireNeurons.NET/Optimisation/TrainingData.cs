namespace FireNeurons.NET.Optimisation;

using Indexes;
using Objects;

public readonly struct TrainingData
{
    public Data InputData { get; }
    public Data LossDerivativeArgs { get; }
    public NeuronIndex[] IgnoreNeurons { get; }

    public TrainingData()
    {
        this.InputData = new Data();
        this.LossDerivativeArgs = new Data();
        this.IgnoreNeurons = Array.Empty<NeuronIndex>();
    }
    public TrainingData(Data inputData, Data lossDerivativeArgs, NeuronIndex[]? ignoreNeurons = null)
    {
        this.InputData = inputData;
        this.LossDerivativeArgs = lossDerivativeArgs;
        this.IgnoreNeurons = ignoreNeurons ?? Array.Empty<NeuronIndex>();
    }
}
