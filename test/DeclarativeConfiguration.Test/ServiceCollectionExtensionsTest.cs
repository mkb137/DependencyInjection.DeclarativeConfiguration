using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.DeclarativeConfiguration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using TestAssemblyOne;
using Xunit;

// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

namespace DeclarativeConfiguration.Test {

	/// <summary>
	/// Tests the <see cref="ServiceCollectionExtensions" /> class.
	/// </summary>
	public sealed class ServiceCollectionExtensionsTest {

		[Fact]
		public void TestConfigureDependenciesExecutesCorrectlyWhenAssemblyIsNotSpecified() {
			// Keep track of the added service descriptors
			var serviceDescriptors = new List<ServiceDescriptor>();
			// Create a mock service collection that wraps a real service collection
			var serviceCollection = new Mock<IServiceCollection>();
			var wrappedServiceCollection = new ServiceCollection();
			serviceCollection
				.Setup( _ => _.Add( It.IsAny<ServiceDescriptor>() ) )
				.Callback( ( ServiceDescriptor serviceDescriptor ) => {
					serviceDescriptors.Add( serviceDescriptor );
					wrappedServiceCollection.Add( serviceDescriptor );
				} );
			serviceCollection
				.Setup( _ => _.GetEnumerator() )
				.Returns( wrappedServiceCollection.GetEnumerator() );

			// Configure the service collection using attributes
			var output = serviceCollection.Object.ConfigureDependencies();

			// The same service collection should be returned
			Assert.Same( output, serviceCollection.Object );

			Assert.Equal( 4, serviceDescriptors.Count );
			VerifyServiceDescriptor<IAlpha, Alpha>( ServiceLifetime.Singleton, serviceDescriptors[0] );
			VerifyServiceDescriptor<IBeta, Beta>( ServiceLifetime.Scoped, serviceDescriptors[1] );
			VerifyServiceDescriptor<Delta, Delta>( ServiceLifetime.Scoped, serviceDescriptors[2] );
			VerifyServiceDescriptor<IGamma, Gamma>( ServiceLifetime.Scoped, serviceDescriptors[3] );
		}

		[Fact]
		public void TestConfigureDependenciesExecutesCorrectlyWhenAssemblyIsSpecified() {
			// Keep track of the added service descriptors
			var serviceDescriptors = new List<ServiceDescriptor>();
			// Create a mock service collection that wraps a real service collection
			var serviceCollection = new Mock<IServiceCollection>();
			var wrappedServiceCollection = new ServiceCollection();
			serviceCollection
				.Setup( _ => _.Add( It.IsAny<ServiceDescriptor>() ) )
				.Callback( ( ServiceDescriptor serviceDescriptor ) => {
					serviceDescriptors.Add( serviceDescriptor );
					wrappedServiceCollection.Add( serviceDescriptor );
				} );
			serviceCollection
				.Setup( _ => _.GetEnumerator() )
				.Returns( wrappedServiceCollection.GetEnumerator() );

			// Configure the service collection using attributes
			var output = serviceCollection.Object.ConfigureDependencies( typeof( One ).Assembly );

			// The same service collection should be returned
			Assert.Same( output, serviceCollection.Object );

			// We should expect all the dependencies in the assembly to be added
			Assert.Equal( 4, serviceDescriptors.Count );
			VerifyServiceDescriptor<IOne, One>( ServiceLifetime.Scoped, serviceDescriptors[0] );
			VerifyServiceDescriptor<IThreeA, Three>( ServiceLifetime.Singleton, serviceDescriptors[1] );
			VerifyServiceDescriptor<ITwoA, Two>( ServiceLifetime.Transient, serviceDescriptors[2] );
			VerifyServiceDescriptor<ITwoB, Two>( ServiceLifetime.Transient, serviceDescriptors[3] );
		}

		private static void VerifyServiceDescriptor<TService, TImplementation>( ServiceLifetime lifetime, ServiceDescriptor serviceDescriptor ) {
			var serviceType = typeof( TService );
			var implementationType = typeof( TImplementation );
			Assert.Equal( serviceType, serviceDescriptor.ServiceType );
			Assert.Equal( implementationType, serviceDescriptor.ImplementationType );
			Assert.Equal( lifetime, serviceDescriptor.Lifetime );
		}

		[ServiceImplementation( ServiceLifetime.Singleton )]
		public class Alpha : IAlpha {

		}

		[ServiceImplementation( typeof( IBeta ) )]
		public class Beta : IBeta {

		}

		[ServiceImplementation( typeof( Delta ) )]
		public class Delta {

		}

		[ServiceImplementation]
		public class Gamma : IGamma {

		}

		// Create some test classes and interfaces
		public interface IAlpha {

		}

		public interface IBeta {

		}

		public interface IGamma {

		}

	}

}
