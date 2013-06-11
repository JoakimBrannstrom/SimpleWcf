using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SimpleWcf.Contracts
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService
    {
		[OperationContract]
		[WebInvoke(Method = "GET")]
		string HelloServer();

        [OperationContract]
		[WebInvoke(Method = "GET", UriTemplate = "GetData/{value}")]
		string GetData(string value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);
    }

    [DataContract]
    public class CompositeType
    {
        bool _boolValue = true;
        string _stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return _boolValue; }
            set { _boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return _stringValue; }
            set { _stringValue = value; }
        }
    }
}
