namespace NServiceBus.Hosting.Tests
{
    namespace EndpointTypeTests
    {
        using System;
        using System.Diagnostics;
        using Windows;
        using Windows.Arguments;
        using NUnit.Framework;
        
        [TestFixture]
        class OtherProperty_Getter_Tests 
        {

            [Test]
            public void the_assemblyQualifiedName_getter_should_not_blow_up()
            {
                var endpointType = new EndpointType(typeof(TestEndpointType));
                Trace.WriteLine(endpointType.AssemblyQualifiedName);
            }

            [Test]
            public void the_endpointConfigurationFile_getter_should_not_blow_up()
            {
                var endpointType = new EndpointType(typeof(TestEndpointType));
                Trace.WriteLine(endpointType.EndpointConfigurationFile);
            }

            [Test]
            public void the_endpointVersion_getter_should_not_blow_up()
            {
                var endpointType = new EndpointType(typeof(TestEndpointType));
                Trace.WriteLine(endpointType.EndpointVersion);
            }
        }

        [TestFixture]
        class EndpointName_Getter_Tests 
        {

            [Test]
            public void when_endpointName_attribute_exists_it_should_have_first_priority()
            {
                var hostArguments = new HostArguments(new string[0])
                {
                    EndpointName = "EndpointNameFromHostArgs"
                };
                var endpointType = new EndpointType(hostArguments, typeof (TestEndpointTypeWithEndpointNameAttribute));

                Assert.AreEqual("EndpointNameFromAttribute", endpointType.EndpointName);
            }

            [EndpointName("EndpointNameFromAttribute")]
            class TestEndpointTypeWithEndpointNameAttribute
            {
            }

            [Test]
            public void when_core_endpointName_attribute_exists_it_should_have_first_priority()
            {
                var hostArguments = new HostArguments(new string[0])
                {
                    EndpointName = "EndpointNameFromHostArgs"
                };
                var endpointType = new EndpointType(hostArguments, typeof(TestEndpointTypeWithCoreEndpointNameAttribute));

                Assert.AreEqual("EndpointNameFromCoreAttribute", endpointType.EndpointName);
            }

            [NServiceBus.EndpointName("EndpointNameFromCoreAttribute")]
            class TestEndpointTypeWithCoreEndpointNameAttribute
            {
            }

            [Test]
            public void when_both_core_and_host_endpointName_attribute_exists_it_should_throw()
            {
                var hostArguments = new HostArguments(new string[0])
                                    {
                                        EndpointName = "EndpointNameFromHostArgs"
                                    };
                var endpointType = new EndpointType(hostArguments, typeof(TestEndpointTypeWithBothEndpointNameAttribute));
                var exception = Assert.Throws<Exception>(() =>
                {
                    // ReSharper disable once UnusedVariable
                    var endpointName = endpointType.EndpointName;
                });
                Assert.AreEqual("Please either define a [NServiceBus.EndpointNameAttribute] or a [NServiceBus.Hosting.Windows.EndpointNameAttribute], but not both.", exception.Message);
            }

            [NServiceBus.EndpointName("EndpointNameFromCoreAttribute")]
            [EndpointName("EndpointNameAttribute")]
            class TestEndpointTypeWithBothEndpointNameAttribute
            {
            }

            [Test]
            [Ignore("this hasn't been implemented yet as far as i can tell")]
            public void when_endpointName_is_provided_via_configuration_it_should_have_second_priority()
            {
                var hostArguments = new HostArguments(new string[0])
                {
                    EndpointName = "EndpointNameFromHostArgs"
                };
                var configuration = new BusConfiguration();

                configuration.EndpointName("EndpointNameFromConfiguration");

                Bus.Create(configuration);

                var endpointType = new EndpointType(hostArguments, typeof (TestEndpointType));

                Assert.AreEqual("EndpointNameFromConfiguration", endpointType.EndpointName);
            }

            [Test]
            public void when_endpointName_is_provided_via_hostArgs_it_should_have_third_priority()
            {
                var hostArguments = new HostArguments(new string[0])
                {
                    EndpointName = "EndpointNameFromHostArgs"
                };
                var endpointType = new EndpointType(hostArguments, typeof (TestEndpointType));

                Assert.AreEqual("EndpointNameFromHostArgs", endpointType.EndpointName);
            }

            [Test]
            [Ignore(
                "not sure how to test this when interface marked as obsolete - get build errors if interface is referenced"
                )]
            public void when_iNameThisEndpoint_is_implemented_it_should_have_second_priority()
            {
            }

            [Test]
            public void when_no_EndpointName_defined_it_should_return_null()
            {
                var hostArguments = new HostArguments(new string[0])
                {
                    EndpointName = "EndpointNameFromHostArgs"
                };
                hostArguments.EndpointName = null;
                var endpointType = new EndpointType(hostArguments, typeof (TestEndpointType));
                Assert.IsNull(endpointType.EndpointName);
            }
        }

        [TestFixture]
        class ServiceName_Getter_Tests 
        {

            [Test]
            public void when_serviceName_is_not_provided_via_hostArgs_and_endpoint_has_a_namespace_it_should_use_the_namespace()
            {
                var hostArguments = new HostArguments(new string[0]);
                var endpointType = new EndpointType(hostArguments, typeof (TestEndpointType));

                Assert.AreEqual("NServiceBus.Hosting.Tests.EndpointTypeTests", endpointType.ServiceName);
            }

            [Test]
            public void when_serviceName_is_not_provided_via_hostArgs_and_endpoint_has_no_namespace_it_should_use_the_assembly_name()
            {
                var hostArguments = new HostArguments(new string[0]);
                var endpointType = new EndpointType(hostArguments, typeof (TestEndpointTypeWithoutANamespace));

                Assert.AreEqual("NServiceBus.Hosting.Tests", endpointType.ServiceName);
            }

            [Test]
            public void when_serviceName_is_provided_via_hostArgs_it_should_have_first_priority()
            {
                var hostArguments = new HostArguments(new string[0])
                                              {
                                                  ServiceName = "ServiceNameFromHostArgs"
                                              };
                var endpointType = new EndpointType(hostArguments, typeof (TestEndpointType));

                Assert.AreEqual("ServiceNameFromHostArgs", endpointType.ServiceName);
            }
        }

        [TestFixture]
        public class Constructor_Tests
        {
            [Test]
            [ExpectedException(typeof (InvalidOperationException),
                ExpectedMessage = "Endpoint configuration type needs to have a default constructor",
                MatchType = MessageMatch.StartsWith)]
            public void When_type_does_not_have_empty_public_constructor_it_should_blow_up()
            {
                 new EndpointType(typeof (TypeWithoutEmptyPublicConstructor));
            }
        }

        class TypeWithoutEmptyPublicConstructor
        {
            // ReSharper disable once UnusedParameter.Local
            public TypeWithoutEmptyPublicConstructor(object foo)
            {
            }
        }

        class TestEndpointType
        {
        }

    }
}

class TestEndpointTypeWithoutANamespace
{
}