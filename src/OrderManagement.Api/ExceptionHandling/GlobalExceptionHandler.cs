using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Domain.Common;

namespace OrderManagement.Api.ExceptionHandling;

internal sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            DomainException domainException => CreateBusinessRuleProblem(domainException),
            NotFoundException notFoundException => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found",
                Detail = notFoundException.Message
            },
            _ => null
        };

        if (problemDetails is null)
        {
            return false;
        }

        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });
    }

    private static ProblemDetails CreateBusinessRuleProblem(DomainException exception)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Business rule violation",
            Detail = "One or more business rules were violated."
        };

        problemDetails.Extensions["errors"] = exception.Errors;

        return problemDetails;
    }
}
