namespace Supercluster.Core
{
    /// <summary>
    /// Contains various constants used throughout the library.
    /// </summary>
    public static class SuperclusterContants
    {
        /// <summary>
        /// The tolerance used when comparing <see cref="double"/> values.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names must not contain underscore", Justification = "Library constant")]
        public const double DOUBLE_TOLERANCE = 1E-13;

        /// <summary>
        /// The name of the property or field searched for trying to detect a type's coordinate representation.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names must not contain underscore", Justification = "Library constant")]
        public const string COORDINATE_FIELD_OR_PROPERTY_NAME = "Coordinate";
    }
}
