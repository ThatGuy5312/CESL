using CESL.Attributes;

namespace CESL.Data;

public struct ShaderField
{
    public string Name { get; internal set; }

    public string Type { get; internal set; }

    public string? Attribute { get; internal set; }

    public string? AttributeName { get; internal set; }

    public bool IsPrivate { get; internal set; }

    public object GetAttribute()
    {
        if (Attribute == null)
            return null;

        var attr = CESL.ParseAttribute(Attribute);

        AttributeName = attr.name;

        if (attr.name == "Range")
        {
            var min = float.Parse(attr.args[0]);
            var max = float.Parse(attr.args[1]);
            var step = float.Parse(attr.args[2]);

            var range = new RangeValue(min, max, step);

            return range;
        }
        return null;
    }

    public override readonly string ToString() => $"Name: {Name}, Type: {Type}, Attribute Name: {AttributeName??"null"}, IsPrivate: {IsPrivate}";
}
