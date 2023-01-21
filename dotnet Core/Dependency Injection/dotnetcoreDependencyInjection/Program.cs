// This solution shows two concepts
// 1 - Dependency Injection - http://stackify.com/net-core-dependency-injection/
// 2 - Repository Pattern   - https://www.youtube.com/watch?v=rtXpYpZdOzM

using System;
using Microsoft.Extensions.DependencyInjection;
using Ebor.Repository;

namespace dotnetcoreDependencyInjection
{
    class Program
    {
        static void Main(string[] args)
        {
            // create new Inversion of Control Dependency Injection Container.
            ServiceCollection serviceCollection = new ServiceCollection();

            // Objects created by DI can be:
            // 1 - Used once and thrown away - these are called "Transient".
            // 2 - Reused a number of times on some related set of calls, these are called "Scoped".
            // 3 - Created once and reused over and over - these are called "Singletons".
            serviceCollection.AddScoped<ICourseRepository, CourseRepository>(); // You will define later exactly how long a "scope" lasts.

            // Calling .BuildServiceProvider() will "activate" the container and make it ready for requests.
            ServiceProvider sp = serviceCollection.BuildServiceProvider();
            // It will also inject an IServiceScopeFactory. Use this to configure exactly how long an object in a "scope" lasts.

            // Have the Container new up a new object like this....
            // This is refered as "Resolving the dependency in the root container".
            // The default constructor on the concrete implementation will be called during construction (as normal).
            var stuff = sp.GetRequiredService<ICourseRepository>();
            stuff.Add(new Course());
            // One only problem here is that if the DI object supports IDispose, it won't get called until the Container is disposed. Possible memory leak.

            // A better option is to resolve the dependency in a "scope".
            var factory = sp.GetRequiredService<IServiceScopeFactory>(); // the object that defines scopes.
            // The "scope" is nothing more than a Using statement.
            // Once this Using ends and the "scope" variable is disposed,
            // any DI objects created in the associated scope are also disposed.
            using (var scope = factory.CreateScope())
            {
                var somestuff = scope.ServiceProvider.GetService<ICourseRepository>();
                for (int i = 0; i < 10; i++)
                {
                    somestuff.Add(new Course());
                }

                somestuff.GetTopSellingCourses(5);

            }


        }
    }
}
