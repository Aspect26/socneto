namespace Domain.Models
{
    public class RegistrationRequestMessage
    {
        public string ComponentType { get; }
        public string ComponentId { get; }
        public RegistrationRequestMessage(string componentId, string componentType)
        {
            ComponentId = componentId;
            ComponentType = componentType;
        }
    }
}