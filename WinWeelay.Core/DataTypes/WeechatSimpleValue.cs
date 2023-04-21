namespace WinWeelay.Core.DataTypes
{
    public class WeechatSimpleValue<T> : WeechatRelayObject
    {
        public T Value { get; set; }

        public WeechatSimpleValue(T value, WeechatType type)
        {
            Value = value;
            Type = type;
        }
    }
}
