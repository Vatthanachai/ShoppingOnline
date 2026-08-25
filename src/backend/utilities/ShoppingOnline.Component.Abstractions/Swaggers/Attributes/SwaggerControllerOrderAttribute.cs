namespace ShoppingOnline.Component.Abstractions.Swaggers.Attributes;

public class SwaggerControllerOrderAttribute(uint order) : Attribute, ISwaggerAttributeOrder
{
    public uint Order { get; } = order;
    public Type AttributeType => typeof(SwaggerControllerOrderAttribute);
}