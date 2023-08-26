using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;

namespace FireNeurons.NET.Analyser;
using Indexes;
using Objects;
using Optimisation;
using Optimisation.Optimisers;

public partial class Form1 : Form
{
    const int MINIBATCH_SIZE = 1;
    const int EPOCHS = 1;

    NeuralNetwork network_1 = null!;
    NeuralNetwork network_2 = null!;

    IOptimiser otpimiser_1 = null!;
    IOptimiser otpimiser_2 = null!;

    (Data, object)[] trainingData;
    (Data, object)[] validationData;

    public Form1()
    {
        this.InitializeComponent();

        throw new NotImplementedException();
        this.trainingData = new (Data, object)[0];
        this.validationData = new (Data, object)[0];


        this.InitOptimisers();
        this.InitChart();
        this.network_1 = GenerateNework_1();
        this.network_2 = GenerateNework_2();

    }

    private void InitOptimisers()
    {
        this.otpimiser_1 = new Adam((n, go, lo) =>
        {
            throw new NotImplementedException();
        });
        this.otpimiser_2 = new Adam((n, go, lo) =>
        {
            throw new NotImplementedException();
        });
    }

    private static NeuralNetwork GenerateNework_1()
    {
        var network = new NeuralNetwork();

        throw new NotImplementedException();

        network.Randomize();
        return network;
    }
    private static NeuralNetwork GenerateNework_2()
    {
        var network = new NeuralNetwork();

        throw new NotImplementedException();

        network.Randomize();
        return network;
    }

    private void InitChart()
    {
        const double precision = 0.001;

        this.cartesianChart.Series = new SeriesCollection(Mappers.Xy<double>()
                .Y(v => Math.Log(((1 / precision) * v) + 1, 10)))
            {
                new LineSeries
                {
                    Title = "Training (L1 & L2)",
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
                    Title = "Validation (L1 & L2)",
                    Stroke = System.Windows.Media.Brushes.Blue,
                    StrokeDashArray = new System.Windows.Media.DoubleCollection(new [] { 4d }),

                    Fill = System.Windows.Media.Brushes.Transparent,
                    Values = new ChartValues<double>(),
                    LineSmoothness = 0.8,
                    PointGeometry = DefaultGeometries.Circle,
                },

                new LineSeries
                {
                    Title = "Training",
                    Stroke = System.Windows.Media.Brushes.Red,

                    Fill = System.Windows.Media.Brushes.Transparent,
                    Values = new ChartValues<double>(),
                    LineSmoothness = 0.8,
                    PointGeometry = DefaultGeometries.Circle,
                },
                new LineSeries
                {
                    Title = "Validation",
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
            LabelFormatter = value => (precision * (Math.Pow(10, value) - 1)).ToString("G3"),
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
            iterationsLabels[i] = (i * (this.trainingData.Length / MINIBATCH_SIZE)).ToString();
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
    private void Train()
    {
        var trainingDataSet = new List<TrainingData>();
        foreach (var data in this.trainingData)
        {
            var (inputData, lossDerivativeArgs) = this.GetTrainingData(data);
            trainingDataSet.Add(new(inputData, lossDerivativeArgs));
        }

        int epochs = int.Parse(this.textBox1.Text);
        for (int epoch = 0; epoch < epochs; epoch++)
        {
            this.network_1.Train(this.otpimiser_1, trainingDataSet, miniBatchSize: MINIBATCH_SIZE, epochs: EPOCHS);
            this.network_2.Train(this.otpimiser_2, trainingDataSet, miniBatchSize: MINIBATCH_SIZE, epochs: EPOCHS);


            double eval_1_training = Evaluate(this.network_1, this.trainingData);
            double eval_1_validation = Evaluate(this.network_1, this.validationData);

            double eval_2_training = Evaluate(this.network_2, this.trainingData);
            double eval_2_validation = Evaluate(this.network_2, this.validationData);

            this.cartesianChart.Series[0].Values.Add(eval_1_training);
            this.cartesianChart.Series[1].Values.Add(eval_1_validation);

            this.cartesianChart.Series[2].Values.Add(eval_2_training);
            this.cartesianChart.Series[3].Values.Add(eval_2_validation);
        }
    }

    private (Data inputData, Data<(object?, Dictionary<NeuronIndex, object>)> lossDerivativeArgs) GetTrainingData((Data, object) data)
    {
        throw new NotImplementedException();
    }

    private static double Evaluate(NeuralNetwork network, (Data, object)[] evaluationDataSet)
    {
        double sum = 0;

        foreach (var (inputData, lossArg) in evaluationDataSet)
        {
            var outputLayerIndex = network.Layers.Last().Value.Index;
            var outputs = network.Evaluate(inputData, false, outputLayerIndex)[outputLayerIndex];

            for (int index = 0; index < outputs.Length; index++)
            {
                //sum += lossArg & outputs;
            }

            sum /= outputs.Length;
        }

        return sum / evaluationDataSet.Length;
    }
}
