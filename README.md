# DependencyInjection.DeclarativeConfiguration
Declarative dependency injection for Microsoft's Dependency Injection as used in AspNetCore

By default, dependencies are added to AspNetCore applications like this:
```csharp
   private void Configure( IServiceCollection services ) {
       services.AddScoped<IFoo,Foo>()
       services.AddTransient<IBar,Bar>()
   }
```

If you have many interfaces, this setup can get tedious.  Moreover, it can be easy to forget to register the classes you've created until you find that your application fails.

This library provides a `[ServiceImplementation]` attribute that can be used to declaratively declare that a class implements a service and should be registered with the service collection.  To accomplish the above configuration, for example, one would decorate the classes like so:
```csharp
    [ServiceImplementation]
    public class Foo : IFoo {
    }
    
    [ServiceImplementation(typeof(IBar), ServiceLifetime.Transient)]
    public class Bar : IBar {
    }
    
   private void Configure( IServiceCollection services ) {
       services.ConfigureDeclaratively();
   }    
```

The `[ServiceImplementation]`attribute takes as arguments:
* (Optional) Type serviceType : The implemented service type.  If not specified, it's assumed that we will register the type for all interfaces that it implements directly.
* (Optional) ServiceLifetime lifetime : The service lifetime (Scoped, Singleton, or Transient). The default is "Scoped".


The `ConfigureDeclaratively` extension method takes an optional list of assemblies as an argument.  The list should include all assemblies that should be checked for types that are decorated with the `[ServiceImplementation]`attribute.  If no assemblies are specified, it checks the calling assembly.
