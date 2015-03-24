﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Tests.Smile
{
	/***
	 * NOTICE!! Jackson puts "non-named" property in the front.
	 ***/
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
		public long deposit;
		[DataMember]
		public byte[] userImage;
		[DataMember]
		public float weight;
		[DataMember]
		public double height;
		[DataMember]
		public string notes;
		[DataMember]
		public string notes_chinese;
		[DataMember]
		public string null_string;
		[DataMember]
		public short small_integer;
		[DataMember(Name = "j1234567890123456789012345678901234567890123456789012345678901234")]
		public string long_column_65;
		[DataMember(Name = "j123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234")]
		public string long_column_175;
		[DataMember(Name = "s")]
		public string short_column;

		public class Name
		{
			public string first;
			public string last;
		}
	}

	public class SmileTestData
	{
		public static string User1 = "3A-29-0A-01-FA-83-6E-61-6D-65-FA-84-66-69-72-73-74-42-4A-6F-65-83-6C-61-73-74-46-53-69-78-70-61-63-6B-FB-85-67-65-6E-64-65-72-43-4D-41-4C-45-87-76-65-72-69-66-69-65-64-22-85-73-61-6C-61-72-79-24-03-01-B2-86-64-65-70-6F-73-69-74-24-03-01-B2-88-75-73-65-72-49-6D-61-67-65-E8-87-23-1B-6D-76-13-05-64-21-85-77-65-69-67-68-74-28-04-14-1D-1E-00-85-68-65-69-67-68-74-29-00-40-33-29-70-10-31-13-3A-2F-84-6E-6F-74-65-73-E0-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-FC-8C-6E-6F-74-65-73-5F-63-68-69-6E-65-73-65-E4-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-FC-8A-6E-75-6C-6C-5F-73-74-72-69-6E-67-21-8C-73-6D-61-6C-6C-5F-69-6E-74-65-67-65-72-DF-34-6A-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-FC-44-61-62-63-36-35-34-6A-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-FC-45-61-62-63-31-37-35-80-73-41-73-41-FB";
		public static string User2 = "3A-29-0A-01-FA-83-6E-61-6D-65-FA-84-66-69-72-73-74-43-4D-61-72-79-83-6C-61-73-74-43-4A-61-6E-65-FB-85-67-65-6E-64-65-72-45-46-45-4D-41-4C-45-87-76-65-72-69-66-69-65-64-23-85-73-61-6C-61-72-79-24-10-49-A4-86-64-65-70-6F-73-69-74-25-01-13-16-01-37-94-88-75-73-65-72-49-6D-61-67-65-E8-8B-24-11-29-44-62-3D-2E-4F-29-13-08-42-01-85-77-65-69-67-68-74-28-04-11-25-43-65-85-68-65-69-67-68-74-29-00-40-32-46-0F-62-41-10-16-78-84-6E-6F-74-65-73-E0-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-FC-8C-6E-6F-74-65-73-5F-63-68-69-6E-65-73-65-E4-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-FC-8A-6E-75-6C-6C-5F-73-74-72-69-6E-67-21-8C-73-6D-61-6C-6C-5F-69-6E-74-65-67-65-72-D8-34-6A-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-FC-44-64-65-66-36-35-34-6A-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-FC-45-64-65-66-31-37-35-80-73-41-73-42-FB";
		public static string Users = "3A-29-0A-01-F8-FA-83-6E-61-6D-65-FA-84-66-69-72-73-74-42-4A-6F-65-83-6C-61-73-74-46-53-69-78-70-61-63-6B-FB-85-67-65-6E-64-65-72-43-4D-41-4C-45-87-76-65-72-69-66-69-65-64-22-85-73-61-6C-61-72-79-24-03-01-B2-86-64-65-70-6F-73-69-74-24-03-01-B2-88-75-73-65-72-49-6D-61-67-65-E8-87-23-1B-6D-76-13-05-64-21-85-77-65-69-67-68-74-28-04-14-1D-1E-00-85-68-65-69-67-68-74-29-00-40-33-29-70-10-31-13-3A-2F-84-6E-6F-74-65-73-E0-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-31-FC-8C-6E-6F-74-65-73-5F-63-68-69-6E-65-73-65-E4-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-E7-94-B7-FC-8A-6E-75-6C-6C-5F-73-74-72-69-6E-67-21-8C-73-6D-61-6C-6C-5F-69-6E-74-65-67-65-72-DF-34-6A-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-FC-44-61-62-63-36-35-34-6A-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-35-36-37-38-39-30-31-32-33-34-FC-45-61-62-63-31-37-35-80-73-41-73-41-FB-FA-40-FA-41-43-4D-61-72-79-42-43-4A-61-6E-65-FB-43-45-46-45-4D-41-4C-45-44-23-45-24-10-49-A4-46-25-01-13-16-01-37-94-47-E8-8B-24-11-29-44-62-3D-2E-4F-29-13-08-42-01-48-28-04-11-25-43-65-49-29-00-40-32-46-0F-62-41-10-16-78-4A-E0-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-32-FC-4B-E4-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-E5-A5-B3-FC-4C-21-4D-D8-4E-44-64-65-66-36-35-4F-45-64-65-66-31-37-35-50-41-73-42-FB-F9";
	}
}