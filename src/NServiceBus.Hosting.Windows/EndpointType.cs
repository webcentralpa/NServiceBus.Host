﻿namespace NServiceBus.Hosting.Windows
{
    using System;
    using System.IO;
    using System.Linq;
    using Arguments;

    /// <summary>
    ///     Representation of an Endpoint Type with additional descriptive properties.
    /// </summary>
    class EndpointType
    {
        internal EndpointType(HostArguments arguments, Type type) : this(type)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            this.arguments = arguments;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EndpointType" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public EndpointType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            this.type = type;
            AssertIsValid();
        }

        internal Type Type
        {
            get { return type; }
        }

        public string EndpointConfigurationFile
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, type.Assembly.ManifestModule.Name + ".config"); }
        }

        public string EndpointVersion
        {
            get { return FileVersionRetriever.GetFileVersion(type); }
        }

        public string AssemblyQualifiedName
        {
            get { return type.AssemblyQualifiedName; }
        }

        public string EndpointName
        {
            get
            {
                var hostEndpointAttribute = (EndpointNameAttribute)type.GetCustomAttributes(typeof(EndpointNameAttribute), false)
                    .FirstOrDefault();
                var coreEndpointAttribute = (NServiceBus.EndpointNameAttribute)type.GetCustomAttributes(typeof(NServiceBus.EndpointNameAttribute), false)
                    .FirstOrDefault();

                if (hostEndpointAttribute != null && coreEndpointAttribute != null)
                {
                    throw new Exception("Please either define a [NServiceBus.EndpointNameAttribute] or a [NServiceBus.Hosting.Windows.EndpointNameAttribute], but not both.");
                }
                if (hostEndpointAttribute != null)
                {
                    return hostEndpointAttribute.Name;
                }
                if (coreEndpointAttribute != null)
                {
                    return coreEndpointAttribute.Name;
                }

                return arguments.EndpointName;
            }
        }

        public string ServiceName
        {
            get
            {
                var serviceName = type.Namespace ?? type.Assembly.GetName().Name;

                if (arguments.ServiceName != null)
                {
                    serviceName = arguments.ServiceName;
                }

                return serviceName;
            }
        }

        void AssertIsValid()
        {
            var constructor = type.GetConstructor(Type.EmptyTypes);

            if (constructor == null)
            {
                throw new InvalidOperationException(
                    "Endpoint configuration type needs to have a default constructor: " + type.FullName);
            }
        }

        readonly HostArguments arguments = new HostArguments(new string[0]);
        readonly Type type;
    }
}