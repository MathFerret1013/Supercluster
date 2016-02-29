namespace Supercluster.Algorithms
{
    using System;
    using System.Linq;

    /// <summary>
    /// Finds the n elements of a data set which are furthest apart
    /// </summary>
    public static class MostDistantPoints
    {

        public static int[] MaximallyDistantPoints<T>(T[] data, int points, Func<T, T, double> Metric)
        {
            // randomly choose n points
            var rand = new Random();

            // randomly choose n distinct points
            var distantPoints = new int[points];
            var randIndex = 0;
            while (randIndex < distantPoints.Length)
            {
                var index = rand.Next(0, data.Length);
                if (!distantPoints.Contains(index))
                {
                    distantPoints[randIndex] = index;
                    randIndex++;
                }
            }

            // TODO:  Create distance matrix


            // could use a matrix like data structure to hold distances
            while (true)
            {
                var lastSum = ComputeTotalDistance(data, distantPoints, Metric);
                var change = false;
                // for each point see if there is another point that increases distance
                for (int i = 0; i < distantPoints.Length; i++)
                {
                    for (int j = 0; j < data.Length; j++)
                    {
                        // calculate distance from test point to OTHER control points

                        var proposedPoints = new int[distantPoints.Length];
                        Array.Copy(distantPoints, proposedPoints, distantPoints.Length);
                        proposedPoints[i] = j;

                        var testDistance = ComputeTotalDistance(data, proposedPoints, Metric);

                        // if this point would increase overall distance then add it
                        if (testDistance > lastSum && !distantPoints.Contains(j))
                        {
                            change = true;
                            distantPoints[i] = j;
                            lastSum = testDistance;
                        }
                    }
                }

                if (!change)
                {
                    break;
                }
            }

            return distantPoints;
        }

        private static double ComputeTotalDistance<T>(T[] data, int[] points, Func<T, T, double> Metric)
        {
            var distance = 0.0;
            for (int i = 0; i < points.Length; i++)
            {
                // exclude self
                var otherPoints = points.ToList();
                otherPoints.RemoveAt(i);

                for (int j = 0; j < otherPoints.Count; j++)
                {
                    distance += Metric(data[points[i]], data[otherPoints[j]]);
                }
            }

            return distance;
        }
        }
    }
