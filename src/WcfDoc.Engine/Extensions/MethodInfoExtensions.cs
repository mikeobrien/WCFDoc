using System.Linq;
using System.Reflection;
using System.ServiceModel;

namespace WcfDoc.Engine.Extensions
{
    public static class MethodInfoExtensions
    {
        public static ParameterInfo GetParameter(this MethodInfo method, string name)
        {
            return method.GetParameters().Where(p => p.Name == name).FirstOrDefault();
        }

        public static string GetOperationName(this MethodInfo method)
        {
            OperationContractAttribute operationContract = null;
            if (method.TryGetAttribute(ref operationContract) &&
                !string.IsNullOrEmpty(operationContract.Name)) return operationContract.Name;
            return method.Name;
        }
    }
}
