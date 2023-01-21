using System;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

using System.IdentityModel.Protocols.WSTrust;

namespace ClientOmAuth
{
	class RequestBodyWriter : BodyWriter
	{
		WSTrustRequestSerializer _serializer;
		RequestSecurityToken _rst;

		/// <summary>
		/// Constructs the Body Writer.
		/// </summary>
		/// <param name="serializer">Serializer to use for serializing the rst.</param>
		/// <param name="rst">The RequestSecurityToken object to be serialized to the outgoing Message.</param>
		public RequestBodyWriter(WSTrustRequestSerializer serializer, RequestSecurityToken rst)
			: base(false)
		{
			if (serializer == null)
				throw new ArgumentNullException("serializer");

			this._serializer = serializer;
			this._rst = rst;
		}


		/// <summary>
		/// Override of the base class method. Serializes the rst to the outgoing stream.
		/// </summary>
		/// <param name="writer">Writer to which the rst should be written.</param>
		protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
		{
			_serializer.WriteXml(_rst, writer, new WSTrustSerializationContext());
		}
	}
}
