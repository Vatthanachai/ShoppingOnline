namespace ShoppingOnline.Component.Abstractions.Swaggers.Attributes;

public interface ISwaggerAttributeOrder
{
    uint Order { get; }
    Type AttributeType { get; }
}