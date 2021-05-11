namespace ApiModels {
    public class DecryptionSettings {
        public string PythonExecutable { get; set; }
        public string DecryptionScript { get; set; }
        public string TempFolder { get; set; }
        public string SenderPublicKeyPath { get; set; }
        public string RecipientPrivateKeyPath { get; set; }
    }
}