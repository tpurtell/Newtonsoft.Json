using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Tests.Smile
{
	[DataContract]
	class People
	{
		[DataMember]
		public Name name;
		[DataMember]
		public string gender;
		[DataMember]
		public bool verified;
		[DataMember]
		public int salary;
		[DataMember]
		public byte[] userImage;

		public class Name
		{
			public string first;
			public string last;
		}
	}
}
