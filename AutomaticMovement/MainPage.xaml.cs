using System.Timers;

namespace AutomaticMovement;

public partial class MainPage : ContentPage
{
    private readonly Random _random = new Random();
    private DateTimeOffset _lastUpdate;
    private int _stepCount;

    public MainPage()
    {
        InitializeComponent();
    }

    private void StartButton_Clicked(object sender, EventArgs e)
    {
        // Inicia un temporizador que se ejecuta cada dos segundos
        var timer = new System.Timers.Timer(1000);
        timer.Elapsed += Timer_Elapsed;
        timer.Start();

        StartButton.IsEnabled = false;
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        // Genera una lectura del acelerómetro y actualiza la cantidad de pasos
        var reading = GenerateAccelerometerReading();
        UpdateStepCount(reading);

        // Actualiza la UI desde el hilo principal
        Device.BeginInvokeOnMainThread(() =>
        {
            XLabel.Text = "X: " + reading.Acceleration.X.ToString();
            YLabel.Text = "Y: " + reading.Acceleration.Y.ToString();
            ZLabel.Text = "Z: " + reading.Acceleration.Z.ToString();
            StepLabel.Text = "Pasos: " + _stepCount.ToString();
        });
    }

    private void UpdateStepCount(AccelerometerData reading)
    {
        // Si la suma de las tres coordenadas del acelerómetro supera un valor umbral,
        // se considera que se ha dado un paso
        var accelerationSum = reading.Acceleration.X + reading.Acceleration.Y + reading.Acceleration.Z;
        var threshold = 1.5;
        if (accelerationSum > threshold)
        {
            _stepCount++;
        }
    }

    private AccelerometerData GenerateAccelerometerReading()
    {
        var currentUpdate = DateTimeOffset.Now;
        var timeSinceLastUpdate = currentUpdate - _lastUpdate;
        var elapsedSeconds = timeSinceLastUpdate.TotalSeconds;

        var x = GenerateAccelerometerValue(elapsedSeconds);
        var y = GenerateAccelerometerValue(elapsedSeconds);
        var z = GenerateAccelerometerValue(elapsedSeconds);

        var reading = new AccelerometerData(x, y, z);

        _lastUpdate = currentUpdate;

        return reading;
    }

    private double GenerateAccelerometerValue(double elapsedTime)
    {
        // Genera un valor aleatorio entre -1 y 1
        var randomValue = (_random.NextDouble() * 2) - 1;

        // Multiplica por un factor para simular movimiento
        var movementFactor = 0.1;
        var movementValue = movementFactor * elapsedTime;

        // Agrega el movimiento al valor aleatorio
        var result = randomValue + movementValue;

        // Ajusta el resultado al rango de -1 a 1
        return Math.Max(-1, Math.Min(1, result));
    }
}

