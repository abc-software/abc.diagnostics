using System;

namespace EnoughPI.Logging
{
    public struct Priority
    {
        public const int Lowest  = 0;
        public const int Low     = 1;
        public const int Normal  = 2;
        public const int High    = 3;
        public const int Highest = 4;
    }

    public struct Category
    {
        public const string ServiceModel = "System.ServiceModel";
        public const string General   = "General";
        public const string Log   = "Log";
        public const string Activity   = "Activity";
        public const string Trace   = "Trace";
        public const string ButtonClick   = "ButtonClick";
        public const string Execute   = "Execute";
    }
}
