using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection.DeclarativeConfiguration {

	/// <summary>
	/// Extension methods on <see cref="IServiceCollection" /> to enable declarative dependency injection.
	/// </summary>
	public static class ServiceCollectionExtensions {

		/// <summary>
		/// Adds a service to the collection.
		/// </summary>
		/// <param name="serviceCollection"></param>
		/// <param name="lifetime"></param>
		/// <param name="implementationType"></param>
		/// <param name="serviceType"></param>
		private static void AddService( IServiceCollection serviceCollection, ServiceLifetime lifetime, Type implementationType, Type serviceType ) {
			Console.WriteLine( "AddService - lifetime = {0}, implementationType = {1}, serviceType = {2}", lifetime, implementationType, serviceType );
			switch ( lifetime ) {
				case ServiceLifetime.Singleton:
					serviceCollection.AddSingleton( serviceType, implementationType );
					break;
				case ServiceLifetime.Scoped:
					serviceCollection.AddScoped( serviceType, implementationType );
					break;
				case ServiceLifetime.Transient:
					serviceCollection.AddTransient( serviceType, implementationType );
					break;
				default:
					throw new ArgumentOutOfRangeException( nameof(lifetime), lifetime, null );
			}
		}

		/// <summary>
		/// Adds the service types to the colleciton.
		/// </summary>
		/// <param name="serviceCollection"></param>
		/// <param name="lifetime"></param>
		/// <param name="implementationType"></param>
		/// <param name="serviceTypes"></param>
		private static void AddServices( IServiceCollection serviceCollection, ServiceLifetime lifetime, Type implementationType, IEnumerable<Type> serviceTypes ) {
			foreach ( var serviceType in serviceTypes ) {
				AddService( serviceCollection, lifetime, implementationType, serviceType );
			}
		}

		/// <summary>
		/// Searches the given assemblies (or, if none is specified, the calling assembly) for types marked with the
		/// <see cref="ServiceImplementationAttribute"/> and when found registers them with the service collection.
		/// </summary>
		/// <param name="serviceCollection">The service collection.</param>
		/// <param name="assemblies">The assemblies to be searched for </param>
		/// <returns></returns>
		public static IServiceCollection ConfigureDeclaratively( this IServiceCollection serviceCollection, params Assembly[] assemblies ) {
			// If no assemblies were specified, use the calling assembly
			if ( assemblies.Length == 0 ) {
				assemblies = new[] { Assembly.GetCallingAssembly() };
			}
			var attributeType = typeof( ServiceImplementationAttribute );
			// In the list of assmblies...
			var implementationTypes = assemblies
				// Get the types...
				.SelectMany( assembly => assembly.GetTypes() )
				// That have the custom attribute
				.Where( type => Attribute.IsDefined( type, attributeType, false ) )
				.OrderBy( type => type.FullName );
			// For each type...
			foreach ( var implementationType in implementationTypes ) {
				// Get the custom attributes 
				var defaultImplementationAtttributes = implementationType.GetCustomAttributes( typeof( ServiceImplementationAttribute ), false );
				foreach ( ServiceImplementationAttribute defaultImplementationAtttribute in defaultImplementationAtttributes ) {
					// If a service type is specified...
					if ( null != defaultImplementationAtttribute.ServiceType ) {
						// Add the type
						AddService( serviceCollection, defaultImplementationAtttribute.Lifetime, implementationType, defaultImplementationAtttribute.ServiceType );
					}
					// If no service type is specified...
					else if ( null == defaultImplementationAtttribute.ServiceType ) {
						// Get the types implemented by the interface
						var implementedInterfaces = GetInterfaceTypes( implementationType );
						// Register the types
						AddServices( serviceCollection, defaultImplementationAtttribute.Lifetime, implementationType, implementedInterfaces );
					}
				}
				Console.WriteLine( " - type = {0}", implementationType );
			}

			// Return the service collection to continue configuration
			return serviceCollection;
		}

		/// <summary>
		/// Get the interfaces implemented by a type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<Type> GetInterfaces( Type type ) {
			return null != type.BaseType ? type.GetInterfaces().Except( type.BaseType.GetInterfaces() ) : type.GetInterfaces();
		}

		/// <summary>
		/// Returns the interface type implemented by the given type.  Throws exceptions if the type implements no interfaces or more than one interface.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static IList<Type> GetInterfaceTypes( Type type ) {
			// Get the interfaces implemented by the type
			var interfaceTypes = new List<Type>( GetInterfaces( type ) );
			// If null, throw an error
			if ( null == interfaceTypes || interfaceTypes.Count == 0 ) {
				throw new InvalidOperationException( $"Type {type} does not directly implement any interfaces." );
			}
			// If it implements more than one interface...
			if ( interfaceTypes.Count > 1 ) {
				// This may be because the interface itself implements another interface.
				// Go through each interface
				for ( var i = 0; i < interfaceTypes.Count; i++ ) {
					var interfaceType = interfaceTypes[i];
					// Get the interface types it implements
					var extendedInterfaceTypes = new List<Type>( GetInterfaces( interfaceType ) );
					// Remove them from the list
					foreach ( var extendedInterfaceType in extendedInterfaceTypes ) {
						if ( interfaceTypes.Contains( extendedInterfaceType ) ) {
							interfaceTypes.Remove( extendedInterfaceType );
						}
					}
				}
			}
			// Return the one type
			return interfaceTypes;
		}

	}

}
