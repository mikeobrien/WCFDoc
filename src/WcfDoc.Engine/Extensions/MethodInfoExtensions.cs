using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
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
            if (method.TryGetAttribute<OperationContractAttribute>(ref operationContract) &&
                !string.IsNullOrEmpty(operationContract.Name)) return operationContract.Name;
            else
                return method.Name;
        }
    }
}
