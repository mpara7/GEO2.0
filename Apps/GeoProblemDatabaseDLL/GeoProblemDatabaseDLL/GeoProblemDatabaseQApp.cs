using GeoInferenceEngine.Knowledges.Imps.IOs.Inputs;
using GeoProblemDatabases.Collections;
using GeoProblemDatabases.Databases;
using GeoProblemDatabases.Models;
using System.Xml.Linq;
using ZTool.Tools;

namespace GeoProblemDatabaseDLL
{
    public class GeoProblemDatabaseQApp
    {
        private static GeoProblemDatabaseQApp app;
        public static GeoProblemDatabaseQApp QApp
        {
            get
            {
                if (app == null)
                {
                    app = new GeoProblemDatabaseQApp();
                }
                return app;
            }
        }

        public string CurGeoProblemDatabase { get; set; } = "";
        GeoProblemDatabase geoProblemDatabase = null;
        InferenceInputDatabase inferenceInputDatabase = null;
        GeoProblemDatabaseQCollection DatabaseQCollection = null;
        Dictionary<string, GeoProblemDatabaseQCollection> collection = new();
        public GeoProblemDatabaseQApp()
        {
            
            geoProblemDatabase = GeoProblemDatabase.Get();
            inferenceInputDatabase = InferenceInputDatabase.Get();
        }

        // 索引器  
        public GeoProblemDatabaseQCollection this[string cName]
        {
            get
            {
                if (CurGeoProblemDatabase != cName)
                {
                    DatabaseQCollection = new GeoProblemDatabaseQCollection(GeoProblemCollection.Get(cName), InferenceInputCollection.Get(cName));
                    if (DatabaseQCollection is null)
                    {
                        throw new Exception($"没有名为{cName}的题目集");
                    }
                    CurGeoProblemDatabase = cName;
                }
                return DatabaseQCollection;
            }

        }
    }
    public class GeoProblemDatabaseQCollection
    {
        GeoProblemCollection geoProblemCollection;
        InferenceInputCollection inferenceInputCollection;

        public GeoProblemDatabaseQCollection(GeoProblemCollection geoProblemCollection, InferenceInputCollection inferenceInputCollection)
        {
            this.geoProblemCollection = geoProblemCollection;
            this.inferenceInputCollection = inferenceInputCollection;
        }
        // 索引器  
        public GeoProblemDatabaseQProblem this[string name]
        {
            get
            {
                var pdao = geoProblemCollection.FindOneWhere(d => d.Title == name);
                if(pdao is null)
                {
                    throw new Exception($"没有标题的{name}的题目");
                }
                var iidao = inferenceInputCollection.FindOneWhere(d => d.ProblemID == pdao.Id);
                
                GeoProblemDatabaseQProblem p = new GeoProblemDatabaseQProblem(iidao);
                return p;
            }
        }
    }
    public class GeoProblemDatabaseQProblem
    {
        InferenceInputDAO inputDAO;
        public GeoProblemDatabaseQProblem(InferenceInputDAO inputDAO)
        {
            this.inputDAO = inputDAO;
        }
        // 索引器  
        public string this[int index]
        {
            get
            {

                var yaml = inputDAO.Inputs[index][nameof(ZScriptInput)];
                var zscript = YAML.Deserialize<ZScriptInput>(yaml);
                return zscript.Content;
            }
        }
    }
}
