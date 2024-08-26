﻿namespace GPACalculator.Data
{
    public class StaticData
    {
        public static readonly Dictionary<string, double> GradePoints = new Dictionary<string, double>
        {
            {"A+", 4.3}, {"A", 4.0}, {"A-", 3.7},
            {"B+", 3.3}, {"B", 3.0}, {"B-", 2.7},
            {"C+", 2.3}, {"C", 2.0}, {"C-", 1.7},
            {"D+", 1.3}, {"D", 1.0}, {"D-", 0.7},
                         {"F", 0.0}
        };
    }
}
