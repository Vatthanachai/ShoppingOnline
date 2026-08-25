namespace ShoppingOnline.Component.Abstractions.Handlers;

public interface IBaseHandler<in TRequest, TResult>
    where TRequest : class
    where TResult : class
{
    Task<TResult> Handler(TRequest request);
}