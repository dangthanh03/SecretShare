namespace SecretShare.Models.Domains
{
    public class UploadedText
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public string FilePath { get; set; }
        public bool AutoDelete { get; set; }
        public DateTime UploadDate { get; set; }
        public bool HasbeenDowloaded { get; set; } = false;
    }
}
