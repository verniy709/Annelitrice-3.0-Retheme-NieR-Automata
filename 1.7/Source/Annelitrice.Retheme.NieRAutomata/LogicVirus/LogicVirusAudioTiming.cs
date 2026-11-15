namespace Annelitrice.Retheme.NieRAutomata
{
    // Game ticks determine when the visual is over; the final pause is perceptual real time.
    public sealed class LogicVirusAudioTiming
    {
        public const double PauseSeconds = 0.18;
        // Lengths of the supplied/edited assets; these do not stop playback.
        public const double MainSeconds = 2.10;
        public const double TailSeconds = 0.38266666666666665;
        public bool VisualComplete { get; private set; }
        public bool Finished { get; private set; }
        public double TailDueTime { get; private set; }
        public bool TailStarted { get; private set; }
        public double CompletionTime { get; private set; }
        private readonly double mainEndTime;

        public LogicVirusAudioTiming(double startRealTime = 0, int mainSpeed = 1)
        {
            mainEndTime = startRealTime + MainSeconds / mainSpeed;
        }

        public bool Advance(int ageTicks, double realTime)
        {
            if (Finished) return false;
            if (!VisualComplete && ageTicks >= LogicVirusGeometry.DurationTicks)
            {
                VisualComplete = true;
                TailDueTime = realTime + PauseSeconds;
            }
            if (VisualComplete && !TailStarted && realTime >= TailDueTime)
            {
                TailStarted = true;
                CompletionTime = System.Math.Max(mainEndTime, realTime + TailSeconds);
                return true;
            }
            Finished = TailStarted && realTime >= CompletionTime;
            return false;
        }

        public static int MainSpeed(float tickRate)
        {
            if (tickRate >= 10f) return 15;
            if (tickRate >= 4.5f) return 6;
            if (tickRate >= 2f) return 3;
            return 1;
        }
    }
}
