namespace UserService.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class SwaggerOneOfRequestAttribute(params Type[] types) : Attribute
{
    public Type[] Types => types;
}

[AttributeUsage(AttributeTargets.Method)]
public class SwaggerOneOfResponseAttribute(params Type[] types) : Attribute
{
    public Type[] Types => types;
}