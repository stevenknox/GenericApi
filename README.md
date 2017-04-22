## GenericApi
# Generic WebAPI, Service Layer and Repository for AspNetCore

Middleware to dynamically generate WebAPI controllers and a Service and Repository Layer for any Model that has been registered as a DBSet in EFCore. Simply decorate your Model class or inherit from a base GenericModel class, register the middleware in your startup class and it will create full a full API with the underlying CRUD Service layer.

