namespace CESL.Attributes;

public class RangeValue
{
    public float Min { get; private set; }
    public float Max { get; private set; }
    public float Step { get; private set; }
    public bool IsInt { get; private set; }

    public RangeValue(float min, float max, float step)
    {
        Min = min;
        Max = max;
        Step = step;
        IsInt = min % 1 == 0 && max % 1 == 0 && step % 1 == 0;
    }

    public override string ToString() => $"Range(Min: {Min}, Max: {Max}, Step: {Step}, IsInt: {IsInt})";
}
