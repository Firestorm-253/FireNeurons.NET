namespace FireNeurons.NET.Analyser;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        btnTrain = new Button();
        textBox1 = new TextBox();
        this.cartesianChart = new LiveCharts.WinForms.CartesianChart();
        this.SuspendLayout();
        // 
        // btnTrain
        // 
        btnTrain.Location = new Point(209, 427);
        btnTrain.Name = "btnTrain";
        btnTrain.Size = new Size(183, 38);
        btnTrain.TabIndex = 0;
        btnTrain.Text = "Train";
        btnTrain.UseVisualStyleBackColor = true;
        btnTrain.Click += this.btnTrain_Click;
        // 
        // textBox1
        // 
        textBox1.Location = new Point(331, 398);
        textBox1.Name = "textBox1";
        textBox1.Size = new Size(61, 23);
        textBox1.TabIndex = 10;
        textBox1.Text = "1";
        // 
        // cartesianChart
        // 
        this.cartesianChart.Location = new Point(398, 12);
        this.cartesianChart.Name = "cartesianChart";
        this.cartesianChart.Size = new Size(731, 489);
        this.cartesianChart.TabIndex = 11;
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(1141, 513);
        this.Controls.Add(this.cartesianChart);
        this.Controls.Add(textBox1);
        this.Controls.Add(btnTrain);
        this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        this.Name = "Form1";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private Button btnTrain;
    private TextBox textBox1;
    private LiveCharts.WinForms.CartesianChart cartesianChart;
}
