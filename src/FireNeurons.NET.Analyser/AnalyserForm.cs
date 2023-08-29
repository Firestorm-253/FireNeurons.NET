using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;

namespace FireNeurons.NET.Analyser;
using Indexes;
using Objects;
using Optimisation;

public partial class AnalyserForm : Form
{
    private readonly double chartPrecision;
    private readonly int miniBatchSize;
    private readonly int epochsPerEpoch;

    private readonly Func<Neuron, object?, object, double> loss_getter;

    public NeuralNetwork Network_1 { get; init; }
    public NeuralNetwork Network_2 { get; init; }

    public IOptimiser Otpimiser_1 { get; init; }
    public IOptimiser Otpimiser_2 { get; init; }

    public string Label_1 { get; init; }
    public string Label_2 { get; init; }
    
    public List<TrainingData> TrainingData { get; init; }
    public List<TrainingData> ValidationData { get; init; }

    public AnalyserForm(
        (NeuralNetwork network, IOptimiser optimiser, string label) obj_1,
        (NeuralNetwork network, IOptimiser optimiser, string label) obj_2,
        List<TrainingData> trainingData,
        List<TrainingData> validationData,
        Func<Neuron, object?, object, double> loss_getter,
        double chartPrecision = 0.001,
        int miniBatchSize = 1,
        int epochsPerEpoch = 1)
    {
        this.InitializeComponent();

        this.Network_1 = obj_1.network;
        this.Otpimiser_1 = obj_1.optimiser;
        this.Label_1 = obj_1.label;

        this.Network_2 = obj_2.network;
        this.Otpimiser_2 = obj_2.optimiser;
        this.Label_2 = obj_2.label;

        this.TrainingData = trainingData;
        this.ValidationData = validationData;

        this.loss_getter = loss_getter;

        this.chartPrecision = chartPrecision;
        this.miniBatchSize = miniBatchSize;
        this.epochsPerEpoch = epochsPerEpoch;

        this.InitChart();
    }

    private void InitChart()
    {
        this.cartesianChart.Series = new SeriesCollection(Mappers.Xy<double>()
                .Y(v => Math.Log(((1 / this.chartPrecision) * v) + 1, 10)))
            {
                new LineSeries
                {
                    Title = $"{this.Label_1} (Training)",
                    Stroke = System.Windows.Media.Brushes.Blue,

                    Fill = System.Windows.Media.Brushes.Transparent,
                    Values = new ChartValues<double>(),
                    LineSmoothness = 0.8,
                    PointGeometry = DefaultGeometries.Circle,
                    //PointGeometrySize = 15,
                    //PointForeground = Brushes.Gray,
                },
                new LineSeries
                {
                    Title = $"{this.Label_1} (Validation)",
                    Stroke = System.Windows.Media.Brushes.Blue,
                    StrokeDashArray = new System.Windows.Media.DoubleCollection(new [] { 4d }),

                    Fill = System.Windows.Media.Brushes.Transparent,
                    Values = new ChartValues<double>(),
                    LineSmoothness = 0.8,
                    PointGeometry = DefaultGeometries.Circle,
                },

                new LineSeries
                {
                    Title = $"{this.Label_2} (Training)",
                    Stroke = System.Windows.Media.Brushes.Red,

                    Fill = System.Windows.Media.Brushes.Transparent,
                    Values = new ChartValues<double>(),
                    LineSmoothness = 0.8,
                    PointGeometry = DefaultGeometries.Circle,
                },
                new LineSeries
                {
                    Title = $"{this.Label_2} (Validation)",
                    Stroke = System.Windows.Media.Brushes.Red,
                    StrokeDashArray = new System.Windows.Media.DoubleCollection(new [] { 4d }),

                    Fill = System.Windows.Media.Brushes.Transparent,
                    Values = new ChartValues<double>(),
                    LineSmoothness = 0.8,
                    PointGeometry = DefaultGeometries.Circle,
                },
            };

        this.cartesianChart.AxisY.Add(new LogarithmicAxis
        {
            LabelFormatter = value => (this.chartPrecision * (Math.Pow(10, value) - 1)).ToString("G3"),
            Base = 10,
            MinValue = 0,
            Separator = new Separator
            {
                Stroke = System.Windows.Media.Brushes.LightGray
            }
        });

        var iterationsLabels = new string[1_000];
        for (int i = 0; i < iterationsLabels.Length; i++)
        {
            iterationsLabels[i] = (i * (this.TrainingData.Count / this.miniBatchSize)).ToString();
        }

        this.cartesianChart.AxisX.Add(new Axis
        {
            Title = "Iterations",
            Labels = iterationsLabels,
            MinValue = 0,
        });

        this.cartesianChart.LegendLocation = LegendLocation.Right;
    }


    private async void btnTrain_Click(object sender, EventArgs e)
    {
        this.btnTrain.Enabled = false;
        await Task.Run(this.Train);
        this.btnTrain.Enabled = true;
    }
    public void Train()
    {
        int epochs = int.Parse(this.textBox1.Text);
        for (int epoch = 0; epoch < epochs; epoch++)
        {
            this.Network_1.Train(this.Otpimiser_1, this.TrainingData, miniBatchSize: this.miniBatchSize, epochs: this.epochsPerEpoch);
            this.Network_2.Train(this.Otpimiser_2, this.TrainingData, miniBatchSize: this.miniBatchSize, epochs: this.epochsPerEpoch);


            double eval_1_training = this.Evaluate(this.Network_1, this.TrainingData);
            double eval_1_validation = this.Evaluate(this.Network_1, this.ValidationData);

            double eval_2_training = this.Evaluate(this.Network_2, this.TrainingData);
            double eval_2_validation = this.Evaluate(this.Network_2, this.ValidationData);

            this.cartesianChart.Series[0].Values.Add(eval_1_training);
            this.cartesianChart.Series[1].Values.Add(eval_1_validation);

            this.cartesianChart.Series[2].Values.Add(eval_2_training);
            this.cartesianChart.Series[3].Values.Add(eval_2_validation);
        }
    }

    public double Evaluate(NeuralNetwork network, List<TrainingData> evaluationDataSet)
    {
        double sum = 0;

        foreach (var trainingData in evaluationDataSet)
        {
            double _sum = 0;

            var outputLayer = network.Layers.Last().Value;
            var (globalArg, localArgs) = trainingData.LossDerivativeArgs[outputLayer];

            var outputs = network.Evaluate(trainingData.InputData, false, outputLayer)[outputLayer];

            for (int index = 0; index < outputs.Length; index++)
            {
                _sum += this.loss_getter(outputLayer.Neurons[index], globalArg, localArgs[outputLayer.Neurons[index].Index]);
            }

            sum += _sum / outputs.Length;
        }

        return sum / evaluationDataSet.Count;
    }
}
