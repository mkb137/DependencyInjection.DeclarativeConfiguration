using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.DeclarativeConfiguration;

namespace TestAssemblyOne {

	[ServiceImplementation( ServiceLifetime.Transient )]
	public class Two : ITwoA, ITwoB {

	}

}
