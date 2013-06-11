using System;

namespace SimpleWcf.Contracts
{
	public class Service : IService
	{
		public string HelloServer()
		{
			return "Hello client!";
		}

		public string GetData(string value)
		{
			return string.Format("You entered: {0}", value);
		}

		public CompositeType GetDataUsingDataContract(CompositeType composite)
		{
			if (composite == null)
			{
				throw new ArgumentNullException("composite");
			}

			if (composite.BoolValue)
			{
				composite.StringValue += "Suffix";
			}

			return composite;
		}
	}
}
