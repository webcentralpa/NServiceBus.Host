﻿namespace NServiceBus.Hosting.Profiles
{
    /// <summary>
    /// Called in order to configure logging.
    /// </summary>
    /// <remarks>
    /// If you want logging configured regardless of profiles, do not use this interface,
    /// instead configure logging before you call <see cref="Bus.Create"/> if you self hosting or configure logging the constructor of the class that implements <see cref="IConfigureThisEndpoint"/>.
    /// Implementors should work against the generic version of this interface in the host.
    /// </remarks>
    [ObsoleteEx(
        TreatAsErrorFromVersion = "7.0",
        Message = "Configure logging in the constructor of the class that implements IConfigureThisEndpoint.",
        RemoveInVersion = "8.0")]
    public interface IConfigureLogging
    {
        /// <summary>
        /// Performs all logging configuration.
        /// </summary>
        // ReSharper disable once UnusedParameter.Global            
        void Configure(IConfigureThisEndpoint specifier);
    }

    /// <summary>
    /// Called in order to configure logging for the given profile type.
    /// If an implementation isn't found for a given profile, then the search continues
    /// recursively up that profile's inheritance hierarchy.
    /// </summary>
    public interface IConfigureLoggingForProfile<T> : IConfigureLogging where T : IProfile {}
}