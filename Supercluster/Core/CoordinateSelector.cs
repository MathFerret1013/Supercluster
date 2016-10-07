namespace Supercluster.Core
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// A static utility class which provides methods for creating a Coordinate Selector.
    /// </summary>
    /// <remarks>
    /// A "Coordinate Selector" is a <see cref="Func{T,TResult}"/> which accepts any type of object and returns and array of doubles.
    /// This double array is the "coordinate" for the given instance of T. Coordinate selectors are used which an algorithm or data-structure
    /// requires a specific coordinate representation for an instance of a generic type.
    /// </remarks>
    public static class CoordinateSelector
    {
        /// <summary>
        /// Compiles a coordinate selector using Expression trees.
        /// </summary>
        /// <typeparam name="T">The type of the object for which to compile a point selector for.</typeparam>
        /// <param name="item">An instance of <typeparamref name="T"/>.</param>
        /// <param name="coordinatePropertyOrFieldName">The name of the property or field that is the coordinate for <typeparamref name="T"/></param>
        /// <returns>A Func which can select the coordinate from any instance of <typeparamref name="T"/>.</returns>
        public static Func<T, double[]> CompilePointSelector<T>(T item, string coordinatePropertyOrFieldName)
        {
            // see if T has a public getter for a double array called coordinate
            var prop = item.GetType().GetProperty(coordinatePropertyOrFieldName) != null
                       || item.GetType().GetField(coordinatePropertyOrFieldName) != null;

            if (prop)
            {
                var parameter = Expression.Parameter(typeof(T), "parameter");
                var propertyExpression = Expression.PropertyOrField(parameter, coordinatePropertyOrFieldName);

                var coordinateSelectorExpression = Expression.Lambda<Func<T, double[]>>(propertyExpression, parameter);
                return coordinateSelectorExpression.Compile();
            }
            else
            {
                throw new ArgumentException($"The type {nameof(T)} does not have a property or field with the name {coordinatePropertyOrFieldName}.");
            }
        }

        /// <summary>
        /// Creates a coordinate selector that returns the value of a property or field with the name "Coordinate".
        /// </summary>
        /// <typeparam name="T">The type of the object for which to compile a point selector for.</typeparam>
        /// <returns>A Func which can select a the coordinate from any instance of <typeparamref name="T"/>.</returns>
        public static Func<T, double[]> CreateDefaultCoordinateSelector<T>()
        {
            var instance = (T)Activator.CreateInstance(typeof(T));
            return CompilePointSelector(instance, SuperclusterContants.COORDINATE_FIELD_OR_PROPERTY_NAME);
        }
    }
}
