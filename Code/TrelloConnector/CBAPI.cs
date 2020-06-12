using PX.Api.ContractBased.Models;
using PX.Commerce.Core.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrelloConnector
{
	[Description("Case")]
	public partial class Case : CBAPIEntity
	{
		public GuidValue NoteID { get; set; }

		public DateTimeValue LastModifiedDateTime { get; set; }

		[Description("CaseCD")]
		public StringValue CaseCD { get; set; }

		[Description("Subject")]
		public StringValue Subject { get; set; }

		[Description("Description")]
		public StringValue Description { get; set; }
	}
}
