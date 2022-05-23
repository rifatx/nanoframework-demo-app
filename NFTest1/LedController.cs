using System.Device.Gpio;
using System.Threading;

namespace NFTest1
{
    internal interface ILedCommand
    {
        void Do(GpioPin pin);
    }

    internal class OnCommand : ILedCommand
    {
        public void Do(GpioPin pin)
        {
            pin.Write(PinValue.High);
        }
    }

    internal class OffCommand : ILedCommand
    {
        public void Do(GpioPin pin)
        {
            pin.Write(PinValue.Low);
        }
    }

    internal class DelayCommand : ILedCommand
    {
        private int _delayAmount { get; }

        public DelayCommand(int delayAmount)
        {
            _delayAmount = delayAmount;
        }

        public void Do(GpioPin _)
        {
            Thread.Sleep(_delayAmount);
        }
    }

    internal class LedController
    {
        private static readonly GpioController _gpioController;
        private static readonly ILedCommand[] _emptySequence;
        private static ILedCommand[] _ledSequence;
        private static ILedCommand[] _flashSequence;
        private static LedController _instance;

        private Thread _t;
        private readonly int _pin;

        public static LedController Instance => _instance ??= new LedController(2);

        static LedController()
        {
            _gpioController = new GpioController();
            _emptySequence = new[] { new DelayCommand(1000) };
            _ledSequence = new ILedCommand[] { };
            _flashSequence = new ILedCommand[] { };
        }

        private LedController(int pin)
        {
            _pin = pin;
            Start();
        }

        public void SinglePulse()
        {
            lock (_ledSequence)
            {
                _ledSequence = new ILedCommand[]
                {
                    new OnCommand(),
                    new DelayCommand(100),
                    new OffCommand(),
                    new DelayCommand(1000)
                };
            }
        }

        public void DoublePulse()
        {
            lock (_ledSequence)
            {
                _ledSequence = new ILedCommand[]
                {
                    new OnCommand(),
                    new DelayCommand(50),
                    new OffCommand(),
                    new DelayCommand(30),
                    new OnCommand(),
                    new DelayCommand(50),
                    new OffCommand(),
                    new DelayCommand(1000)
                };
            }
        }

        public void Flash(ILedCommand[] commands)
        {
            if (commands.Length < 1)
            {
                return;
            }

            lock (_flashSequence)
            {
                _flashSequence = commands;
            }
        }

        private void Start()
        {
            _t = new Thread(() =>
            {
                if (_gpioController.IsPinOpen(_pin))
                {
                    _gpioController.ClosePin(_pin);
                }

                var pin = _gpioController.OpenPin(_pin, PinMode.Output);

                while (true)
                {
                    lock (_ledSequence)
                    {
                        lock (_flashSequence)
                        {
                            foreach (var c in _flashSequence)
                            {
                                c.Do(pin);
                            }

                            if (_flashSequence.Length > 0)
                            {
                                _flashSequence = new ILedCommand[] { };
                            }
                        }

                        foreach (var c in _ledSequence.Length > 0 ? _ledSequence : _emptySequence)
                        {
                            c.Do(pin);
                        }
                    }
                }
            });

            _t.Start();
        }
    }
}
