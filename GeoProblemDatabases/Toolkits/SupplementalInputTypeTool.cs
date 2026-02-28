using ZTool.Tools;

namespace GeoProblemDatabases.Toolkits
{
    public class SupplementalInputTypeTool
    {
        /// <summary>
        /// List<Type> types = new List<Type>() { typeof(ZScriptInput),typeof(AlphaGeoInput),typeof(FormalGeoInput)};
        /// SupplementalInputTypeTool.Do(types);
        /// </summary>
        /// <param name="inputTypes"></param>
        public static void Do(List<Type> inputTypes)
        {
            int i = 0;
            var pdb = GeoProblemDatabase.Get();
            var cNames = pdb.GetCollectionNames();
            foreach (var cName in cNames)
            {
                var inputC = InferenceInputCollection.Get(cName);
                var inputs = inputC.FindAll();
                foreach (var input in inputs)
                {
                    foreach (var qInputDict in input.Inputs)
                    {
                        foreach (var inputType in inputTypes)
                        {
                            if (!qInputDict.ContainsKey(inputType.Name))
                            {
                                var newInput = Activator.CreateInstance(inputType);
                                var yaml = YAML.Serialize(newInput);
                                qInputDict.Add(inputType.Name, yaml);
                                i++;

                            }
                        }
                    }

                    inputC.Update(input);
                }
            }


        }
    }
}
