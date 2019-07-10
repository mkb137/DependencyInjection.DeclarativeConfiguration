using System;

namespace Microsoft.Extensions.DependencyInjection.DeclarativeConfiguration {

	/// <summary>
	/// When placed on a class, this indicates that the class is the implementation of the given service type or,
	/// if no interface is provided, whatever interfaces the class implements.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class, Inherited = false, AllowMultiple = true )]
	public sealed class ServiceImplementationAttribute : Attribute {

		/// <summary>
		/// The class will be assumed to be the default implementation of all interfaces it implements.
		/// The service lifetime will be assumed to be <see cref="ServiceLifetime.Scoped" />.
		/// </summary>
		public ServiceImplementationAttribute() : this( null, ServiceLifetime.Scoped ) {
		}

		/// <summary>
		/// The class is the default implementation of the given interface.
		/// The service lifetime will be assumed to be <see cref="ServiceLifetime.Scoped" />.
		/// </summary>
		public ServiceImplementationAttribute( Type serviceType ) : this( serviceType, ServiceLifetime.Scoped ) {
		}

		/// <summary>
		/// The class will be assumed to be the default implementation of all interfaces it implements,
		/// with the given lifetime.
		/// </summary>
		public ServiceImplementationAttribute( ServiceLifetime lifetime ) : this( null, lifetime ) {
		}

		/// <summary>
		///  The class is the default implementation of the given interface with the given lifetime.
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="lifetime"></param>
		public ServiceImplementationAttribute( Type serviceType, ServiceLifetime lifetime ) {
			ServiceType = serviceType;
			Lifetime = lifetime;
		}

		/// <summary>
		/// The interface implemented by the class.
		/// </summary>
		public Type ServiceType { get; }

		/// <summary>
		/// The service lifetime (Scoped, Singleton, or Transient).
		/// </summary>
		public ServiceLifetime Lifetime { get; }

	}

}
