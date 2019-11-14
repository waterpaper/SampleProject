public struct State
{
    public float MaxValue { get; set; }

    private float currentValue;
    public float CurrentValue {
        get
        {
            return currentValue;
        }
        set
        {
            if(value > currentValue)
            {
                currentValue = MaxValue;
            }
            else if(value < 0)
            {
                currentValue = 0;
            }
            else
            {
                currentValue = value;
            }
        }
    }

    public void Initialize(float cValue, float mValue)
    {
        currentValue = cValue;
        MaxValue = mValue;
    }
}
