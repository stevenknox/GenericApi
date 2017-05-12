[![NuGet](https://img.shields.io/nuget/v/Nuget.Core.svg)](https://www.nuget.org/packages/GenericApi)  [![Twitter Follow](https://img.shields.io/twitter/follow/espadrine.svg?style=social&label=Follow)](https://twitter.com/stevenknox101)

# Generic WebAPI and Repository for AspNetCore

> Please note this is still in active development and hasn't been tested enough yet for Production systems. Currently perfect for prototyping and if you encounter any problems please open an issue or create a pull request.


Middleware to dynamically generate WebAPI controllers and Repository Layer for any Model that has been registered as a DBSet in EFCore. Simply decorate your Model class or inherit from a base GenericModel class, register the middleware in your startup class and it will create full a full API with the underlying CRUD repository layer.


**Get Started**

Install the package into your Asp.NetCore MVC project

    Install-Package GenericApi

Under ConfigureServices within the Startup.cs you can enable the generic service layer by adding the following:

     services.AddGenericServices();

You also need to specific the Authorization type by registering a Policy within the AddAuthorization extension and specifying if you wish to AllowAnonymous or Authorize.

     services.AddAuthorization(options =>
      {
			options.AddPolicy("SecureGenericApi", policy => 
			policy.Requirements.Add(new SecureGenericApiRequirement(ApiAuthorization.AllowAnonymous)));
       });

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
	    
			services.AddAuthorization(options =>
			{
				options.AddPolicy("SecureGenericApi", policy => 
				policy.Requirements.Add(new SecureGenericApiRequirement(ApiAuthorization.AllowAnonymous)));
			 });
        }

**Note:** Current version can only support generic services for one DbContext. If you have more than one DbContext within your application you must explicitly specify the DbContext to use during the middleware registration

    .AddGenericControllers(nameof(StoreWebApi), typeof(StoreDbContext));

To enable dynamic API and Repository generation for your Entity Framework entities ensure it inherits from the *GenericEntity* base class and has a primary key called *Id* . The Id can be any primative type.

    public class Product: GenericEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

This is everything you need to configure, simply launch your application and navigate to the api url in your browser. The url will be in the format */api/entity*, thus if you have an entity named product you can access your WebAPI service via standard Get, Post, Put, Delete requests. eg:

    /api/product
    /api/product/1

The underlying repository layer can be accessed using Dependency Injection:

	 private IGenericRepository<Product, StoreDbContext> _repo;
		
	 public HomeController(IGenericRepository<Product, StoreDbContext> repo)
	   {
	        _repo = repo;
	    }
	     
	 public IActionResult Index()
	 {
			var products = _repo.GetAll();
	
		return View(products);
	 }
	

If you have used a primative type other than an *Int* for your Id property you can pass in the type to the repository constructor, for example if you used a GUID:
     
private IGenericRepository<Product, Guid, StoreDbContext> _service;
	
    public HomeController(IGenericRepository<Product, Guid, StoreDbContext> service)
     {
         _service = service;
     }
    


I have included a full working sample MVC project along with the source code showing all of the configuration in place.

# What about ViewModels, InputModels and DTO's?

GenericApi can support different models for Input and Views by adding the GenericApi.ModelExtensions package from NuGet to a project that already has GenericApi

    Install-Package GenericApi.ModelExtensions

The extension currently has a dependency on AutoMapper and you must create your mapping profiles to allow the underlying mappings to function.

The extension package gives you extra options when configuring your Startup.cs class

      services.
               AddMvc().
                 AddGenericControllers(new OptionsBuilder
                 {
                     db = typeof(StoreDbContext),
                     DbContextAssemblyName = nameof(StoreWebApi),
                     EntityAssemblyName = nameof(StoreWebApi),
                     UseInputModels = true,
                     UseViewModels = true,
                 });

When this configuration is in place, GenericApi will scan your the assembly containing your entities for matching ViewModel and InputModel classes and if found will register those for use. For example a Product class with an Input and ViewModel would have 3 classes:

 - Product.cs
 - ProductViewModel.cs
 - ProductInputModel.cs

We would also have our corresponding AutoMapper Profile class with the following:

     CreateMap<Product, ProductViewModel>();
     CreateMap<ProductInputModel, Product>();

You dont have to have both the ViewModel and InputModel in place. If for example you have an Order entity that accepts an InputModel but returns the full entity rather than an Order ViewModel, you would have 2 classes:

 - Order.cs
 - OrderInputModel.cs

You can also use the same class for both Input and View Models by using the DTO format. Under Startup.cs use:

     services.
               AddMvc().
                 AddGenericControllers(new OptionsBuilder
                 {
                     db = typeof(StoreDbContext),
                     DbContextAssemblyName = nameof(StoreWebApi),
                     EntityAssemblyName = nameof(StoreWebApi),
                     UseDTOs = true
                 });

And you can create one additional class ProductDTO.cs that will be used for both Input and View Models.

EFCore does not support Lazy Loading yet so to allow you to return data from related entities you can use the *MapToEntity* decorator on your ViewModel. For example a Product entity with a related ProductType entity:

     public class Product: GenericEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }

        public int ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }
    }

To return the Product Type with your Product data you can update your ViewModel to look like:

     public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }

        [MapToEntity(typeof(ProductType))]
        public string ProductTypeName { get; set; }
    }

The above using strong typing to specify the related entity, however we can also pass in a string to access the related entity, or of we want to chain related entities. An example Order class could be:

    public class Order : GenericEntity
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int Total { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }

With an AutoMapper profile containing:

      CreateMap<Order, OrderViewModel>();
      CreateMap<OrderInputModel, Order>();

If we wanted the OrderViewModel to include both the Product and the Product Type we could update our ViewModel to :

    public class OrderViewModel
    {
        public int Id { get; set; }
        [MapToEntity("Product")]
        public string ProductName { get; set; }
        [MapToEntity("Product.ProductType")]
        public string ProductTypeName { get; set; }
        public int Quantity { get; set; }
    }

I have included a working example with DTOs along with the source.

Please feel free to dig into the code and open a pull request with improvements, bug fixes and new features! 
