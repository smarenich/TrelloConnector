using PX.Commerce.Core;
using PX.Commerce.Objects;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrelloConnector
{
	public class TrelloStoreMaint : BCStoreMaint
	{
		public TrelloStoreMaint()
		{
			base.Bindings.WhereAnd<Where<BCBinding.connectorType, Equal<TrelloConnector.trelloConnectorType>>>();
		}

		#region BCBinding Events
		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXCustomizeBaseAttribute(typeof(BCConnectorsAttribute), "DefaultConnector", TrelloConnector.TYPE)]
		public virtual void _(Events.CacheAttached<BCBinding.connectorType> e) { }
		#endregion
	}
}
