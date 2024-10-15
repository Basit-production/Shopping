namespace Ahmed_mart.Configurations.v1
{
    public class MongoDbConnectionSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string StudentCoursesCollectionName { get; set; } = string.Empty;
    }
}
