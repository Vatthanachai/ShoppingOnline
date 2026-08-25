namespace ShoppingOnline.Component.Abstractions.Swaggers.Attributes;

public class SwaggerActionOrderAttribute(uint order) : Attribute, ISwaggerAttributeOrder
{
    public uint Order { get; } = order;
    public Type AttributeType => typeof(SwaggerActionOrderAttribute);
}