using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Data;
using System.Globalization;

namespace WpfClockControl
{
    /// <summary>
    /// ========================================
    /// WinFX Custom Control
    /// ========================================
    ///
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Untamed.MMA.View.WPF.Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Untamed.MMA.View.WPF.Controls;assembly=Untamed.MMA.View.WPF.Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file. Note that Intellisense in the
    /// XML editor does not currently work on custom controls and its child elements.
    ///
    ///     <MyNamespace:Clock/>
    ///
    /// </summary>

    public class ClockControl : Control
    {



        /// <summary>
        /// Static constructor. Initialize the theme's default style. See generic.xaml.
        /// </summary>
        static ClockControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClockControl), new FrameworkPropertyMetadata(typeof(ClockControl)));
        }



        /// <summary>
        /// Initialize the clock control. Create and start the timer.
        /// </summary>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            UpdateDateTime(DateTime.Now);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            //timer.Interval = TimeSpan.FromMilliseconds(1000);
            //timer.Interval = TimeSpan.FromMilliseconds(100 - DateTime.Now.Millisecond / 10);
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }



        /// <summary>
        /// Handler for the timer's Tick event.
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            if(!IsDiscrete || now.Second != _lastTick.Second)
                UpdateDateTime(now);
        }



        /// <summary>
        /// Update this.DateTime to the current time.
        /// </summary>
        private void UpdateDateTime(DateTime newVal)
        {
            _lastTick = newVal;
            this.DateTime = (IsDiscrete) ? newVal.AddMilliseconds(-newVal.Millisecond) : newVal;
        }



        /// <summary>
        /// Get or set the clock's current time.
        /// </summary>
        public DateTime DateTime
        {
            get
            {
                return (DateTime)GetValue(DateTimeProperty);
            }
            private set
            {
                SetValue(DateTimeProperty, value);
            }
        }



        /// <summary>
        /// Get or set whether the clock operates in discrete or continuous mode.
        /// </summary>
        public bool IsDiscrete
        {
            get
            {
                return (bool)GetValue(IsDiscreteProperty);
            }
            set
            {
                SetValue(IsDiscreteProperty, value);
            }
        }



        /// <summary>
        /// Register the "DateTime" property as a formal dependency property.
        /// </summary>
        public static DependencyProperty DateTimeProperty = DependencyProperty.Register(
                "DateTime",
                typeof(DateTime),
                typeof(ClockControl),
                new PropertyMetadata(DateTime.Now, new PropertyChangedCallback(OnDateTimeInvalidated)));



        /// <summary>
        /// Register the "IsDiscrete" property as a formal dependency property.
        /// </summary>
        public static DependencyProperty IsDiscreteProperty = DependencyProperty.Register(
                "IsDiscrete",
                typeof(bool),
                typeof(ClockControl),
                new PropertyMetadata(true, new PropertyChangedCallback(OnIsDiscreteInvalidated)));



        /// <summary>
        /// Set up a DateTimeChanged event.
        /// </summary>
        public static readonly RoutedEvent DateTimeChangedEvent = 
            EventManager.RegisterRoutedEvent( 
                "DateTimeChanged", 
                RoutingStrategy.Bubble, 
                typeof(RoutedPropertyChangedEventHandler<DateTime>), 
                typeof(ClockControl));



        /// <summary>
        /// Fire the DateTimeChanged event when the time changes.
        /// </summary>
        protected virtual void OnDateTimeChanged(DateTime oldValue, DateTime newValue)
        {
            RoutedPropertyChangedEventArgs<DateTime> args = new RoutedPropertyChangedEventArgs<DateTime>(oldValue, newValue);
            args.RoutedEvent = ClockControl.DateTimeChangedEvent;
            RaiseEvent(args);
        }



        /// <summary>
        /// Fire the DateTimeChanged event when the time changes.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnIsDiscreteChanged(bool oldValue, bool newValue)
        {
            RoutedPropertyChangedEventArgs<bool> args = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue);
            args.RoutedEvent = ClockControl.DateTimeChangedEvent;
            RaiseEvent(args);
        }



        /// <summary>
        /// Will be called every time the ClockControl.DateTime property changes.
        /// </summary>
        private static void OnDateTimeInvalidated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ClockControl clock = (ClockControl)d;

            DateTime oldValue = (DateTime)e.OldValue;
            DateTime newValue = (DateTime)e.NewValue;

            if (oldValue.Second != newValue.Second) {
                clock.OnDateTimeChanged(oldValue, newValue.AddMilliseconds(-newValue.Millisecond));
            }
        }



        /// <summary>
        /// Will be called every time the ClockControl.IsDiscrete property changes.
        /// </summary>
        private static void OnIsDiscreteInvalidated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ClockControl clock = (ClockControl)d;

            bool oldValue = (bool)e.OldValue;
            bool newValue = (bool)e.NewValue;

            clock.OnIsDiscreteChanged(oldValue, newValue);
        }



        /// <summary>
        /// Timer used to drive the clock.
        /// </summary>
        private DispatcherTimer timer;



        /// <summary>
        /// Cache the clock's "last tick time".
        /// </summary>
        DateTime _lastTick = DateTime.Now;
    }



    [ValueConversion(typeof(DateTime), typeof(double))]
    public class MilliSecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return date.Second * 6 + (date.Millisecond * .006);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }



    [ValueConversion(typeof(DateTime), typeof(int))]
    public class SecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ClockControl c = parameter as ClockControl;
            DateTime date = (DateTime)value;
            return date.Second * 6;// (date.Millisecond * .006);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(DateTime), typeof(int))]
    public class MinutesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return date.Minute * 6 + (date.Second * .1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(DateTime), typeof(int))]
    public class HoursConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return (date.Hour * 30) + (date.Minute / 2);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(DateTime), typeof(string))]
    public class WeekdayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return date.DayOfWeek.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }





}
