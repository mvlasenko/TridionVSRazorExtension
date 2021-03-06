﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TcmDebugger.Service
{	
	[Serializable]
	public class DebuggingStatusResult : IXmlSerializable
	{
		public String SessionId
		{
			get; 
			set;
		}

		public bool IsRunning
		{
			get; 
			set;
		}

		public String ExecutionStatus
		{
			get; 
			set;
		}

		public String Log
		{
			get;
			set;
		}

		public String Error
		{
			get;
			set;
		}

		/// <summary>
		/// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.
		/// </returns>
		public XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
		/// <exception cref="System.NotImplementedException"></exception>
		public void ReadXml(XmlReader reader)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
		/// <exception cref="System.NotImplementedException"></exception>
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteElementString("SessionId", SessionId);

			writer.WriteStartElement("IsRunning");
			writer.WriteValue(IsRunning);			
			writer.WriteEndElement();

			if (!String.IsNullOrEmpty(ExecutionStatus))
			{
				writer.WriteStartElement("ExecutionStatus");
				writer.WriteRaw(ExecutionStatus);
				writer.WriteEndElement();
			}

			if (!String.IsNullOrEmpty(Log))
			{
				writer.WriteStartElement("Log");
				writer.WriteRaw(Log);
				writer.WriteEndElement();
			}

			if (!String.IsNullOrEmpty(Error))
				writer.WriteElementString("Error", Error);
		}
	}
}
