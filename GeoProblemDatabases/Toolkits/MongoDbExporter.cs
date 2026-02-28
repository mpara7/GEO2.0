

using MongoDB.Bson;

public class MongoDbExporter
{

    public static void ExportCollectionsToJson(string directoryName, string outputDirectory = "wwwroot/files")
    {
        outputDirectory = $"{outputDirectory}/{directoryName}";
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }
        foreach (var collectionName in GeoProblemDatabase.Get().GetCollectionNames())
        {
            var json = GeoProblemCollection.Get(collectionName).FindAll().ToJson();
            Directory.CreateDirectory($"{outputDirectory}/GeoProblem");
            var fileName = $"{outputDirectory}/GeoProblem/{collectionName}.json";
            File.WriteAllText(fileName, json);
        }
        foreach (var collectionName in ImageDatabase.Get().GetCollectionNames())
        {
            var json = ImageCollection.Get(collectionName).FindAll().ToJson();
            Directory.CreateDirectory($"{outputDirectory}/Image");
            var fileName = $"{outputDirectory}/Image/{collectionName}.json";
            File.WriteAllText(fileName, json);
        }
        foreach (var collectionName in InferenceInputDatabase.Get().GetCollectionNames())
        {
            var json = InferenceInputCollection.Get(collectionName).FindAll().ToJson();
            Directory.CreateDirectory($"{outputDirectory}/InferenceInput");
            var fileName = $"{outputDirectory}/InferenceInput/{collectionName}.json";
            File.WriteAllText(fileName, json);
        }
        foreach (var collectionName in InferenceOutputDatabase.Get().GetCollectionNames())
        {
            var json = InferenceOutputCollection.Get(collectionName).FindAll().ToJson();
            Directory.CreateDirectory($"{outputDirectory}/InferenceOutput");
            var fileName = $"{outputDirectory}/InferenceOutput/{collectionName}.json";
            File.WriteAllText(fileName, json);
        }
    }
}