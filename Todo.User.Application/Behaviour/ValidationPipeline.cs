using FluentValidation;
using MediatR;
using Todo.SharedKernel.Results;

namespace Todo.User.Application.Behaviour;

public class ValidationPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipeline(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators ?? throw new ArgumentNullException(nameof(validators));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var validationResults = _validators
            .Select(v => v.Validate(context))
            .Where(r => !r.IsValid)
            .ToList();

        if (!validationResults.Any())
            return await next(cancellationToken);

        var errors = validationResults
            .SelectMany(r => r.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        if (!typeof(TResponse).IsGenericType ||
            typeof(TResponse).GetGenericTypeDefinition() != typeof(Result<>))
        {
            throw new InvalidOperationException("ValidationPipeline only works with Result<T> response types.");
        }

        var resultType = typeof(TResponse).GetGenericArguments()[0];

        var failMethod = typeof(Result)
            .GetMethods()
            .FirstOrDefault(m =>
                m is { Name: nameof(Result.Fail), IsGenericMethod: true } &&
                m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType == typeof(string));

        if (failMethod == null)
            throw new InvalidOperationException("Result.Fail<T>(string) method not found.");

        var failGenericMethod = failMethod.MakeGenericMethod(resultType);
        var failResult = failGenericMethod.Invoke(null, [string.Join(", ", errors)]);

        return (TResponse)failResult!;
    }
}