using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using static System.Convert;

class TimedEvent {

    DispatcherTimer T;
    Action Event;
    public Action<int> Tick = new Action<int>((_) => { });

    public readonly int TotalCount;
    public int Count {
        get;
        private set;
    }
    readonly bool StopA;

    public TimedEvent(TimeSpan update, int timer, bool stop, Action action) {
        TotalCount = Count = timer;
        StopA = stop;
        Event = action ?? throw new ArgumentNullException();
        T = new DispatcherTimer {
            Interval = update
        };
        T.Tick += Timer_Tick;
    }

    private void Timer_Tick(object sender, EventArgs e) {
        Tick.Invoke(Count);
        if (--Count <= 0) {
            Event.Invoke();
            Count = TotalCount;
            if (StopA) T.Stop();
        }
    }

    public void TryTigger() {
        Count = TotalCount;
        T.Start();
    }

    public void TriggerNow() => Event.Invoke();

    public void Stop() => T.Stop();

    public void Start() {
        Event.Invoke();
        T.Start();
    }
}