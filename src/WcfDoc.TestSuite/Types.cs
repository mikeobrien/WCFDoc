using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ServiceModel.Web;

namespace WcfDoc.TestSuite.ServiceContracts
{
    /// <summary>
    /// This is a service contract class. You can pass a <paramref name="parameter"/> when you call <c>MethodWithNoResult(parameter, listParameter, serializableParameter)</c>. To learn more about this see <see cref="WcfDoc.TestSuite.TypesA.DataContractType">here</see>.
    /// </summary>
    /// <remarks><para>Ut ullamcorper euismod enim ut ullamcorper.</para><para>Morbi egestas magna eu elit ultrices eget accumsan nisi mattis. Pellentesque nec tempus justo. Curabitur ultricies vestibulum bibendum. In non magna sem, vel lacinia odio. Cras at sem urna. Nullam et nulla quis libero vestibulum viverra in id ipsum. Nunc et risus id lacus vulputate consectetur. Praesent fermentum dapibus augue vitae elementum. Nam lacinia varius ullamcorper. Praesent varius placerat luctus. Nullam lobortis, lectus vel sagittis blandit, erat magna accumsan enim, vitae ullamcorper eros dolor ut sapien.</para>
    /// <list type="bullet">
    /// <listheader><term>SomeTerm</term><description>This is some term</description></listheader>
    /// <item><term>Item #1</term><description>First item</description></item>
    /// <item><term>Item #2</term><description>Second item</description></item>
    /// </list>
    /// </remarks>
    /// <value>This is some info about a value.</value>
    /// <param name="parameter">This is a parameter.</param>
    /// <param name="listParameter">This is a generic list parameter.</param>
    /// <param name="serializableParameter">This is a serializable parameter.</param>
    /// <example>
    /// Here is how you use this:
    /// <code>MethodWithNoResult(...)</code>
    /// </example>
    /// <seealso cref="Method">Check this one out too.</seealso>
    /// <seealso cref="WcfDoc.TestSuite.TypesA.DataContractType">And this.</seealso>
    /// <exception cref="Exception">Bad things are happening.</exception>
    [ServiceContract]
    public class ServiceContractClass
    {
        /// <summary>
        /// This is a method with no parameters or return value.
        /// </summary>
        [OperationContract]
        public void MethodWithNoResultOrParameters() { }
        /// <summary>
        /// This is a method with not return value. You can pass a <paramref name="parameter"/> when you call <c>MethodWithNoResult(parameter, listParameter, serializableParameter)</c>. To learn more about this see <see cref="IServiceContractInterface"/>
        /// </summary>
        /// <remarks><para>Ut ullamcorper euismod enim ut ullamcorper.</para> Morbi egestas magna eu elit ultrices eget accumsan nisi mattis. Pellentesque nec tempus justo. Curabitur ultricies vestibulum bibendum. In non magna sem, vel lacinia odio. Cras at sem urna. Nullam et nulla quis libero vestibulum viverra in id ipsum. Nunc et risus id lacus vulputate consectetur. Praesent fermentum dapibus augue vitae elementum. Nam lacinia varius ullamcorper. Praesent varius placerat luctus. Nullam lobortis, lectus vel sagittis blandit, erat magna accumsan enim, vitae ullamcorper eros dolor ut sapien.
        /// <list type="number">
        /// <item><term>Item #1</term><description>First item</description></item>
        /// <item><term>Item #2</term><description>Second item</description></item>
        /// </list>
        /// </remarks>
        /// <value>This is some info about a value.</value>
        /// <param name="parameter">This is a parameter.</param>
        /// <param name="listParameter">This is a generic list parameter.</param>
        /// <param name="serializableParameter">This is a serializable parameter.</param>
        /// <example>
        /// Here is how you use this:
        /// <code>MethodWithNoResult(...)</code>
        /// </example>
        /// <seealso cref="Method">Check this one out too.</seealso>
        /// <exception cref="Exception">Bad things are happening.</exception>
        [OperationContract(Name="Method_With_No_Result")]
        public void MethodWithNoResult(string parameter, List<int> listParameter, TypesC.SerializableType serializableParameter) { }
        /// <summary>
        /// This is a method.
        /// </summary>
        /// <param name="parameter">This is a parameter.</param>
        /// <param name="listParameter">This is a generic list parameter.</param>
        /// <param name="serializableParameter">This is a serializable parameter.</param>
        /// <returns>This returns a string</returns>
        [OperationContract]
        public string Method(string parameter, List<int> listParameter, TypesC.SerializableType serializableParameter) { return null; }
        /// <summary>
        /// This is another method that returns a generic object.
        /// </summary>
        /// <param name="pocoParameter">This is a POCO parameter.</param>
        /// <returns>A generic type.</returns>
        [OperationContract]
        public TypesB.GenericDataContractType<string, int, List<DateTime>> MethodWithGenericReturn(TypesC.POCOType pocoParameter) { return null; }
    }
     
    /// <summary>
    /// This is a service contract interface. You can pass a <paramref name="parameter"/> when you call <c>MethodWithNoResult(parameter, listParameter, serializableParameter)</c>. To learn more about this see <see cref="IServiceContractInterface"/>
    /// </summary>
    /// <remarks><para>Ut ullamcorper euismod enim ut ullamcorper.</para> Morbi egestas magna eu elit ultrices eget accumsan nisi mattis. Pellentesque nec tempus justo. Curabitur ultricies vestibulum bibendum. In non magna sem, vel lacinia odio. Cras at sem urna. Nullam et nulla quis libero vestibulum viverra in id ipsum. Nunc et risus id lacus vulputate consectetur. Praesent fermentum dapibus augue vitae elementum. Nam lacinia varius ullamcorper. Praesent varius placerat luctus. Nullam lobortis, lectus vel sagittis blandit, erat magna accumsan enim, vitae ullamcorper eros dolor ut sapien.
    /// <list type="number">
    /// <item>First item</item>
    /// <item>Second item</item>
    /// </list>
    /// </remarks>
    /// <value>This is some info about a value.</value>
    /// <param name="parameter">This is a parameter.</param>
    /// <param name="listParameter">This is a generic list parameter.</param>
    /// <param name="serializableParameter">This is a serializable parameter.</param>
    /// <example>
    /// Here is how you use this:
    /// <code>MethodWithNoResult(...)</code>
    /// </example>
    /// <seealso cref="Method">Check this one out too.</seealso>
    /// <exception cref="Exception">Bad things are happening.</exception>
    [ServiceContract]
    public interface IRESTfulServiceContractInterface
    {
        /// <summary>
        /// This is a method with no parameters or return value.
        /// </summary>
        [OperationContract(Name="Method_With_No_Result_Or_Parameters")]
        [WebInvoke(UriTemplate="/", Method="DELETE")]
        void MethodWithNoResultOrParameters();
        /// <summary>
        /// This is a method with not return value.
        /// </summary>
        /// <param name="parameter">This is a parameter.</param>
        /// <param name="listParameter">This is a generic list parameter.</param>
        /// <param name="serializableParameter">This is a serializable parameter.</param>
        /// <example>
        /// Here is how you use this:
        /// <code>MethodWithNoResult(...)</code>
        /// </example>
        [OperationContract]
        [WebInvoke(UriTemplate="/", Method="POST")]
        void MethodWithNoResult(string parameter, List<int> listParameter, TypesC.SerializableType serializableParameter);
        /// <summary>
        /// This is a method.
        /// </summary>
        /// <param name="parameter">This is a parameter.</param>
        /// <param name="listParameter">This is a generic list parameter.</param>
        /// <param name="serializableParameter">This is a serializable parameter.</param>
        /// <returns>This returns a string</returns>
        [OperationContract]
        [WebGet(UriTemplate="/")]
        string Method(string parameter, List<int> listParameter, TypesC.SerializableType serializableParameter);
        /// <summary>
        /// This is another method that returns a generic object.
        /// </summary>
        /// <param name="pocoParameter">This is a POCO parameter.</param>
        /// <returns>A generic type.</returns>
        [OperationContract]
        [WebGet(UriTemplate="/yada")]
        TypesB.GenericDataContractType<string, int, List<DateTime>> MethodWithGenericReturn(TypesC.POCOType pocoParameter);
    }

    public class RESTfulService : IRESTfulServiceContractInterface
    {
        public void MethodWithNoResultOrParameters()
        {
            throw new NotImplementedException();
        }

        public void MethodWithNoResult(string parameter, List<int> listParameter, WcfDoc.TestSuite.TypesC.SerializableType serializableParameter)
        {
            throw new NotImplementedException();
        }

        public string Method(string parameter, List<int> listParameter, WcfDoc.TestSuite.TypesC.SerializableType serializableParameter)
        {
            throw new NotImplementedException();
        }

        public WcfDoc.TestSuite.TypesB.GenericDataContractType<string, int, List<DateTime>> MethodWithGenericReturn(WcfDoc.TestSuite.TypesC.POCOType pocoParameter)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// This is a service contract interface with a custom name.
    /// </summary>
    [ServiceContract(Name="Service_Contract_Interface_With_Custom_Name_And_Namespace", Namespace="urn:UltravioletCatastrophe")]
    public interface IServiceContractInterfaceWithCustomNameAndNamespace
    {
        /// <summary>
        /// This is a method with no parameters or return value.
        /// </summary>
        [OperationContract]
        void MethodWithNoResultOrParameters();
        /// <summary>
        /// This is a method with not return value.
        /// </summary>
        /// <param name="parameter">This is a parameter.</param>
        /// <param name="listParameter">This is a generic list parameter.</param>
        /// <param name="serializableParameter">This is a serializable parameter.</param>
        /// <example>
        /// Here is how you use this:
        /// <code>MethodWithNoResult(...)</code>
        /// </example>
        [OperationContract]
        void MethodWithNoResult(string parameter, List<int> listParameter, TypesC.SerializableType serializableParameter);
        /// <summary>
        /// This is a method.
        /// </summary>
        /// <param name="parameter">This is a parameter.</param>
        /// <param name="listParameter">This is a generic list parameter.</param>
        /// <param name="serializableParameter">This is a serializable parameter.</param>
        /// <returns>This returns a string</returns>
        [OperationContract]
        string Method(string parameter, List<int> listParameter, TypesC.SerializableType serializableParameter);
        /// <summary>
        /// This is another method that returns a generic object.
        /// </summary>
        /// <param name="pocoParameter">This is a POCO parameter.</param>
        /// <returns>A generic type.</returns>
        [OperationContract(Name="Method_With_Generic_Return")]
        TypesB.GenericDataContractType<string, int, List<DateTime>> MethodWithGenericReturn(TypesC.POCOType pocoParameter, TypesA.DataContractType dataContractParameter);
    }

    /// <summary>
    /// This is a restful service contract interface.
    /// </summary>
    [ServiceContract]
    public interface IRestfulServiceContractInterface
    {
        /// <summary>
        /// This is a restful method with no parameters or return value.
        /// </summary>
        [OperationContract(Name="Method_With_No_Result_Or_Parameters")]
        [WebGet(UriTemplate = "/widgets")]
        void MethodWithNoResultOrParameters();
        /// <summary>
        /// This is a restful method with no parameters or return value.
        /// </summary>
        [OperationContract(Name = "Method_With_One_Result")]
        [WebGet(UriTemplate = "/widgets/{Id}")]
        void MethodWithOneResult();
        /// <summary>
        /// This is a restful method with not return value.
        /// </summary>
        /// <param name="parameter">This is a parameter.</param>
        /// <param name="listParameter">This is a generic list parameter.</param>
        /// <param name="serializableParameter">This is a serializable parameter.</param>
        /// <example>
        /// Here is how you use this:
        /// <code>MethodWithNoResult(...)</code>
        /// </example>
        [OperationContract]
        [WebInvoke(UriTemplate="/widgets/{Id}?sort={Sort}", Method="PUT")]
        void MethodWithNoResult(string parameter, List<int> listParameter, TypesC.SerializableType serializableParameter);
        /// <summary>
        /// This is a restful method.
        /// </summary>
        /// <param name="parameter">This is a parameter.</param>
        /// <param name="listParameter">This is a generic list parameter.</param>
        /// <param name="serializableParameter">This is a serializable parameter.</param>
        /// <returns>This returns a string</returns>
        [OperationContract]
        [WebInvoke(UriTemplate="/widgets", Method="POST")]
        string Method(string parameter, List<TypesA.DataContractType> listParameter, TypesC.SerializableType serializableParameter);
        /// <summary>
        /// This is another restful method that returns a generic object.
        /// </summary>
        /// <param name="pocoParameter">This is a POCO parameter.</param>
        /// <returns>A generic type.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate="/widgets/{Id}", Method="DELETE")]
        TypesA.DataContractTypeWithNameAndNamespace[] MethodWithGenericReturn(TypesC.POCOType pocoParameter);
    }
}

namespace WcfDoc.TestSuite.TypesA
{
    /// <summary>
    /// This is a data contract type.
    /// </summary>
    [DataContract]
    public class DataContractType
    {
        /// <summary>
        /// This is a nice field.
        /// </summary>
        [DataMember]
        public string field;
        /// <summary>
        /// This is a nice field with a custom name.
        /// </summary>
        [DataMember(Name="field_With_A_Custom_Name")]
        public string fieldWithACustomName;
        /// <summary>
        /// This is a nice property.
        /// </summary>
        [DataMember]
        public string Property { get; set; }
        /// <summary>
        /// This is a nice property with a custom name.
        /// </summary>
        [DataMember(Name="Property_With_A_Custom_Name")]
        public string PropertyWithACustomName { get; set; }
        /// <summary>
        /// This is an array property.
        /// </summary>
        [DataMember]
        public string[] ArrayProperty { get; set; }
        /// <summary>
        /// This is a generic list property.
        /// </summary>
        [DataMember]
        public List<string> ListProperty { get; set; }
        /// <summary>
        /// This is a generic type property.
        /// </summary>
        [DataMember]
        public TypesB.GenericDataContractType<string, int, DataContractType> GenericProperty { get; set; }
        /// <summary>
        /// This is a serializable property.
        /// </summary>
        [DataMember]
        public TypesC.SerializableType SerializableProperty { get; set; }
        /// <summary>
        /// This is a poco property.
        /// </summary>
        [DataMember]
        public TypesC.POCOType POCOProperty { get; set; }
        /// <summary>
        /// This is an enum property.
        /// </summary>
        [DataMember]
        public TypesD.ColorsEnumeration ColorsEnumProperty { get; set; }
        /// <summary>
        /// This is data contract enum property.
        /// </summary>
        [DataMember]
        public TypesD.ColorsEnumerationDataContract ColorsDataContractEnumProperty { get; set;}
        /// <summary>
        /// This is a data contract enum property with a custom name.
        /// </summary>
        [DataMember]
        public TypesD.ColorsEnumerationDataContractWithCustomNames ColorsDataContractCustomNameEnumProperty { get; set;}
    }

    [DataContract(Name = "Data_Contract_Type_With_Name_And_Namespace", Namespace="urn:UltravioletCatastrophe")]
    public class DataContractTypeWithNameAndNamespace
    {
        /// <summary>
        /// This is a nice field.
        /// </summary>
        [DataMember]
        public string field;
        /// <summary>
        /// This is a nice property.
        /// </summary>
        [DataMember]
        public string Property { get; set; }
    }
}

namespace WcfDoc.TestSuite.TypesB
{
    /// <summary>
    /// This is a generic data contract.
    /// </summary>
    /// <typeparam name="A">This is a generic type parameter.</typeparam>
    /// <typeparam name="B">This is another generic type parameter.</typeparam>
    /// <typeparam name="C">And yet another type parameter.</typeparam>
    [DataContract]
    public class GenericDataContractType<A,B,C>
    {
        /// <summary>
        /// This is a nice property.
        /// </summary>
        [DataMember(IsRequired=true)]
        public A PropertyA { get; set; }
        /// <summary>
        /// This is a nice property.
        /// </summary>
        [DataMember]
        public B PropertyB { get; set; }
        /// <summary>
        /// This is a nice property.
        /// </summary>
        [DataMember]
        public C PropertyC { get; set; }
    }
    /// <summary>
    /// This is a generic data contract with a custom name and namespace.
    /// </summary>
    /// <typeparam name="A">This is a generic type parameter.</typeparam>
    /// <typeparam name="B">This is another generic type parameter.</typeparam>
    /// <typeparam name="C">And yet another type parameter.</typeparam>
    [DataContract(Name = "Data_Contract_Type_With_Name_And_Namespace", Namespace="urn:UltravioletCatastrophe")]
    public class GenericDataContractTypeWithNameAndNamespace<A,B,C>
    {
        /// <summary>
        /// This is a nice property.
        /// </summary>
        [DataMember]
        public A PropertyA { get; set; }
        /// <summary>
        /// This is a nice property.
        /// </summary>
        [DataMember]
        public B PropertyB { get; set; }
        /// <summary>
        /// This is a nice property.
        /// </summary>
        [DataMember]
        public C PropertyC { get; set; }
    }
    /// <summary>
    /// This is a generic data contract with a custom name layout.
    /// </summary>
    /// <typeparam name="A">This is a generic type parameter.</typeparam>
    /// <typeparam name="B">This is another generic type parameter.</typeparam>
    /// <typeparam name="C">And yet another type parameter.</typeparam>    [DataContract(Name = "Data_Contract_Type_With_Name_And_WithTypes_{0}_And_{1}_And_{2}")]
    public class GenericDataContractTypeWithNameAndCustomLayout<A,B,C>
    {
        /// <summary>
        /// This is a nice property.
        /// </summary>
        [DataMember]
        public A PropertyA { get; set; }
        /// <summary>
        /// This is a nice property.
        /// </summary>
        [DataMember]
        public B PropertyB { get; set; }
        /// <summary>
        /// This is a nice property.
        /// </summary>
        [DataMember]
        public C PropertyC { get; set; }
    }
}

namespace WcfDoc.TestSuite.TypesC
{
    /// <summary>
    /// This is a serializable type.
    /// </summary>
    [Serializable]
    public class SerializableType
    {
        /// <summary>
        /// This is a nice field.
        /// </summary>
        public string field;

        /// <summary>
        /// This is a nice backing field for a property.
        /// </summary>
        private string _propertyField;
        /// <summary>
        /// This is a nice property.
        /// </summary>
        public string Property { get { return _propertyField; } set { _propertyField = value; } }

        /// <summary>
        /// This is a nice backing field for an array property.
        /// </summary>
        private string[] _arrayPropertyField;
        /// <summary>
        /// This is a nice array property.
        /// </summary>
        public string[] ArrayProperty { get { return _arrayPropertyField;} set { _arrayPropertyField = value;} }

        /// <summary>
        /// This is a nice backing field for a generic list property.
        /// </summary>
        private List<string> _listPropertyField;
        /// <summary>
        /// This is a nice generic list property.
        /// </summary>
        public List<string> ListProperty { get { return _listPropertyField;} set { _listPropertyField = value;} }

        /// <summary>
        /// This is a nice backing field for a generic type property.
        /// </summary>
        private TypesB.GenericDataContractType<string, int, TypesA.DataContractType> _genericPropertyField;
        /// <summary>
        /// This is a nice generic type property.
        /// </summary>
        public TypesB.GenericDataContractType<string, int, TypesA.DataContractType> GenericProperty { get { return _genericPropertyField;} set { _genericPropertyField = value;} }

        /// <summary>
        /// This is a nice backing field for a serializable property.
        /// </summary>
        private SerializableType _serializablePropertyField;
        /// <summary>
        /// This is a nice property that returns a serializable type.
        /// </summary>
        public SerializableType SerializableProperty { get { return _serializablePropertyField;} set { _serializablePropertyField = value;} }

        /// <summary>
        /// This is a nice backing field for a property that returns a POCO.
        /// </summary>
        private POCOType _pocoPropertyField;
        /// <summary>
        /// This is a nice property that retrns a POCO.
        /// </summary>
        public POCOType POCOProperty { get { return _pocoPropertyField;} set { _pocoPropertyField = value;} }
    }

    /// <summary>
    /// This is a nice POCO type.
    /// </summary>
    public class POCOType
    {
        /// <summary>
        /// This is a nice field.
        /// </summary>
        public string field;
        /// <summary>
        /// This is a nice property.
        /// </summary>
        public string Property { get; set; }
        /// <summary>
        /// This is a nice array property.
        /// </summary>
        public string[] ArrayProperty { get; set; }
        /// <summary>
        /// This is a nice generic list property.
        /// </summary>
        public List<string> ListProperty { get; set; }
        /// <summary>
        /// This is a nice generic type property.
        /// </summary>
        public TypesB.GenericDataContractType<string, int, TypesA.DataContractType> GenericProperty { get; set; }
        /// <summary>
        /// This is a nice property that returns a serializable type.
        /// </summary>
        public SerializableType SerializableProperty { get; set; }
        /// <summary>
        /// A nice property that returns a data contract.
        /// </summary>
        public TypesA.DataContractType DataContractProperty { get; set; }
        /// <summary>
        /// A nice property that returns an enum.
        /// </summary>
        public TypesD.ColorsEnumeration ColorsEnumProperty { get; set; }
        /// <summary>
        /// A nice property that returns an enum.
        /// </summary>
        public TypesD.ColorsEnumerationDataContract ColorsDataContractEnumProperty { get; set;}
        /// <summary>
        /// A nice property that returns an enum.
        /// </summary>
        public TypesD.ColorsEnumerationDataContractWithCustomNames ColorsDataContractCustomNameEnumProperty { get; set;}
    }
}

namespace WcfDoc.TestSuite.TypesD
{
    /// <summary>
    /// A nice data contract enum of colors.
    /// </summary>
    [DataContract]
    public enum ColorsEnumerationDataContract
    {
        /// <summary>
        /// Angry people turn red
        /// </summary>
        [EnumMember]
        Red,
        /// <summary>
        /// Its not easy being green
        /// </summary>
        [EnumMember]
        Green,
        /// <summary>
        /// Its not easy being blue either
        /// </summary>
        [EnumMember]
        Blue
    }

    /// <summary>
    /// A nice data contract enum with a custom name
    /// </summary>
    [DataContract(Name="Colors_Enumeration_Data_Contract_With_CustomNames")]
    public enum ColorsEnumerationDataContractWithCustomNames
    {
        /// <summary>
        /// Red buttons do bad things
        /// </summary>
        [EnumMember(Value="R_e_d")]
        Red,
        /// <summary>
        /// Green sea monsters are troubling
        /// </summary>
        [EnumMember(Value="G_r_e_e_n")]
        Green,
        /// <summary>
        /// Blue dragons are even more troubling
        /// </summary>
        [EnumMember]
        Blue
    }

    /// <summary>
    /// A nice plain old enum
    /// </summary>
    public enum ColorsEnumeration
    {
        /// <summary>
        /// Is Robert Redford red?
        /// </summary>
        Red,
        /// <summary>
        /// Is Alan Greenspan green?
        /// </summary>
        Green,
        /// <summary>
        /// Are Blue Dog democrats blue? 
        /// </summary>
        Blue
    }
}
