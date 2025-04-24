using Catalog.API.Products.UpdateProduct;

namespace Catalog.API.Products.CreateProduct;

// Code Before make a class library for CQRS and MediatR

////Create a record as a DTO of creating product
//public record CreateProductCommand(string Name,List<string> Category, string Description, string ImageFile, decimal Price)
//    : IRequest<CreateProductResult>;

////record for returing result of creating product
//public record CreateProductResult(Guid Id);

//internal class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResult>
//{
//    public Task<CreateProductResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
//    {
//        // Business Logic
//        throw new NotImplementedException();
//    }
//}


//Create a record as a DTO of creating product
public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price)
    : ICommand<CreateProductResult>;

//record for returing result of creating product
public record CreateProductResult(Guid Id);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x=>x.Name).NotEmpty().WithMessage("Name Is Required");
        RuleFor(x=>x.Category).NotEmpty().WithMessage("Category Is Required");
        RuleFor(x=>x.ImageFile).NotEmpty().WithMessage("ImageFile Is Required");
        RuleFor(x=>x.Price).GreaterThan(0).WithMessage("Price Must Be Greater Than 0");
    }
}

internal class CreateProductCommandHandler(IDocumentSession session
    /*, IValidator<CreateProductCommand> validator*/)
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        //Validation (Instead of writing the same code for each Hundle we use MedaitR pipeline(BuildingBlocks -> Behaviors))
        //var result = await validator.ValidateAsync(command, cancellationToken);
        //var errors = result.Errors.Select(x => x.ErrorMessage).ToList();
        //if (errors.Any())
        //{
        //    throw new ValidationException(errors.FirstOrDefault());
        //}

        // Business Logic
        // Create Product
        var product = new Product
        {
            Name = command.Name,
            Category = command.Category,
            Description = command.Description,
            ImageFile = command.ImageFile,
            Price = command.Price
        };

        // save to db
        session.Store(product);
        await session.SaveChangesAsync(cancellationToken);

        // return result guid of the created product
        return new CreateProductResult(product.Id);
    }
}