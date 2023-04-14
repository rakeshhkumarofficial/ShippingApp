namespace ShippingApp.Models
{
    public class CheckpointModel
    {
        public Guid checkpointId { get; set; }
        public string checkpointName { get; set; } 
        public float latitude { get; set; }
        public float longitude { get; set; }
    }
}
