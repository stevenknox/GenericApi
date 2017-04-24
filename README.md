[![NuGet](https://img.shields.io/nuget/v/Nuget.Core.svg)](https://www.nuget.org/packages/GenericApi/0.2.0)  [![Twitter Follow](https://img.shields.io/twitter/follow/espadrine.svg?style=social&label=Follow)](https://twitter.com/stevenknox101)

# Generic WebAPI, Service Layer and Repository for AspNetCore

Middleware to dynamically generate WebAPI controllers and a Service and Repository Layer for any Model that has been registered as a DBSet in EFCore. Simply decorate your Model class or inherit from a base GenericModel class, register the middleware in your startup class and it will create full a full API with the underlying CRUD Service layer.


**Get Started**

Install the package into your Asp.NetCore MVC project

    Install-Package GenericApi

Under ConfigureServices within the Startup.cs you can enable the generic service layer by adding the following:

     services.AddGenericServices();

In the same startup method you can register dynamic WebApi controllers by adding the following method to AddMvc(). Replace *SampleWebApi* with the name of the Assembly containing your EF Entities.

    services.AddMvc().AddGenericControllers(nameof(StoreWebApi));

A complete ConfigureServices method that includes adding an Entity Framework DbContext may look like:

     public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<StoreDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.
                AddMvc().
                AddGenericControllers(nameof(StoreWebApi));

            services.AddGenericServices();
        }

**Note:** Current version can only support generic services for one DbContext. If you have more than one DbContext within your application you must explicitly specify the DbContext to use during the middleware registration

    .AddGenericControllers(nameof(StoreWebApi), typeof(StoreDbContext));

To able dynamic API and Service generation for your Entity Framework entities ensure it inherits from the *GenericEntity* base class and has a primary key called *Id* . The Id can be any primative type.

    public class Product: GenericEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

This is everything you need to configure, simply launch your application and navigate to the api url in your browser. The url will be in the format */api/entity*, thus if you have an entity named product you can access your WebAPI service via standard Get, Post, Put, Delete requests. eg:

    /api/product
    /api/product/1

The underlying service layer can be accessed using Dependency Injection:

	 private IGenericService<Product, StoreDbContext> _service;
		
	 public HomeController(IGenericService<Product, StoreDbContext> service)
	   {
	        _service = service;
	    }
	     
	 public IActionResult Index()
	 {
			var products = _service.GetAll();
	
		return View(products);
	 }
	

If you have used a primative type other than an *Int* for your Id property you can pass in the type to the service constructor, for example if you used a GUID:
     
private IGenericService<Product, Guid, StoreDbContext> _service;
	
    public HomeController(IGenericService<Product, Guid, StoreDbContext> service)
     {
         _service = service;
     }
    


I have included a full working sample MVC project along with the source code showing all of the configuration in place.
