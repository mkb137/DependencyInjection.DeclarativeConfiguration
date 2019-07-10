using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.DeclarativeConfiguration;

namespace TestAssemblyOne {

	[ServiceImplementation( typeof( IThreeA ), ServiceLifetime.Singleton )]
	public class Three : IThreeA, IThreeB {

	}

}
